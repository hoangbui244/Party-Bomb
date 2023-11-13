using System;
using UnityEngine;
using UnityEngine.Extension;
using UnityStandardAssets.Utility;
public class Biathlon_Character : MonoBehaviour
{
	public enum Mode
	{
		CrossCountry,
		Shooting
	}
	private const string HillZone = "DownhillStart";
	private const string HillZoneForceEnd = "DownhillEnd";
	[SerializeField]
	private Biathlon_CharacterMovement movement;
	[SerializeField]
	private Biathlon_CharacterShooting shooting;
	[SerializeField]
	private Biathlon_CharacterRenderer renderer;
	[SerializeField]
	private Biathlon_CharacterAudio audio;
	[SerializeField]
	private Biathlon_SpurRenderer spur;
	[SerializeField]
	[DisplayName("順位判定用")]
	private WaypointProgressTracker placeTracker;
	[SerializeField]
	[DisplayName("AIの移動用")]
	private WaypointProgressTracker aiMoveTracker;
	private Mode mode;
	private Biathlon_CharacterCamera camera;
	private Biathlon_Course course;
	private Biathlon_Target target;
	private bool isSlopeArea;
	private float allowSpeedEffectTime;
	private LTDescr autoMoveForAI;
	public int PlayerNo
	{
		get;
		private set;
	}
	public Biathlon_Definition.ControlUser ControlUser
	{
		get;
		private set;
	}
	public Mode ActionMode => mode;
	public bool IsPlayer => ControlUser < Biathlon_Definition.ControlUser.Cpu1;
	public bool IsLockPlacement
	{
		get;
		private set;
	}
	public int Placement
	{
		get;
		private set;
	}
	public int CurrentLap
	{
		get;
		private set;
	}
	public bool IsGoal
	{
		get;
		private set;
	}
	public WaypointProgressTracker PlacementTracker => placeTracker;
	public WaypointProgressTracker AITracker => aiMoveTracker;
	public float CurrentRunDistance => movement.CurrentRunDistance;
	public float MaxRunDistance => movement.MaxRunDistance;
	public bool IsForwardDownward
	{
		get;
		private set;
	}
	public bool IsForwardUpward
	{
		get;
		private set;
	}
	public bool IsUpward
	{
		get
		{
			if (isSlopeArea)
			{
				return IsForwardUpward;
			}
			return false;
		}
	}
	public bool IsDownward
	{
		get
		{
			if (isSlopeArea)
			{
				return IsForwardDownward;
			}
			return false;
		}
	}
	public float Speed => movement.Speed;
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("DownhillStart"))
		{
			isSlopeArea = !isSlopeArea;
			UpdateSlope();
		}
		else if (other.gameObject.CompareTag("DownhillEnd"))
		{
			isSlopeArea = false;
			UpdateSlope();
		}
	}
	public void Init(int no, int user)
	{
		PlayerNo = no;
		ControlUser = (Biathlon_Definition.ControlUser)user;
		Placement = no;
		mode = Mode.CrossCountry;
		course = SingletonCustom<Biathlon_Courses>.Instance.Current;
		course.IgnoreCollision(this, GetComponent<Collider>());
		placeTracker.setCircuit = course.GetPlaceCircuit();
		movement.Init(this, course);
		shooting.Init(this);
		renderer.Init(this);
		audio.Init(this);
		if (IsPlayer)
		{
			camera = SingletonCustom<Biathlon_CameraRegistry>.Instance.GetCamera(no);
			camera.SetCharacter(this);
			aiMoveTracker.setCircuit = course.GetAiCircuit(0);
			return;
		}
		if (!SingletonCustom<Biathlon_GameMain>.Instance.IsSinglePlay)
		{
			camera = SingletonCustom<Biathlon_CameraRegistry>.Instance.GetCamera(no);
			camera.SetCharacter(this);
		}
		aiMoveTracker.setCircuit = course.GetAiCircuit();
	}
	public void UpdateMethod()
	{
		if (!SingletonCustom<Biathlon_GameMain>.Instance.IsGameStarted)
		{
			PlacementTracker.progressDistance = 0f;
			AITracker.progressDistance = 0f;
			return;
		}
		SearchSlope();
		movement.UpdateMethod();
		shooting.UpdateMethod();
		audio.UpdateMethod();
		if (!(camera == null))
		{
			if (Time.time > allowSpeedEffectTime && IsDownward)
			{
				camera.ShowSpeedEffect();
				audio.StartWindSfx();
			}
			else
			{
				camera.HideSpeedEffect();
				audio.StopWindSfx();
			}
			camera.UpdateMethod();
		}
	}
	public void UpdateAudio()
	{
		audio.UpdateMethod();
	}
	public void FixedUpdateMethod()
	{
		movement.FixedUpdateMethod();
	}
	public void GameEnd()
	{
		audio.GameEnd();
	}
	public void UpdatePlacement(int placement)
	{
		Placement = placement;
		SingletonCustom<Biathlon_UI>.Instance.UpdatePlacement(PlayerNo, Placement);
	}
	public void TransitionShootingMode(Biathlon_Target enterTarget)
	{
		if (mode != Mode.Shooting)
		{
			mode = Mode.Shooting;
			target = enterTarget;
			target.Activate();
			target.IsUsing = true;
			Vector3 standingPoint = target.GetStandingPoint();
			Vector3 position = base.transform.position;
			position.y = standingPoint.y;
			float time = Vector3.Distance(position, standingPoint) / 5f;
			Vector3 forward = standingPoint - position;
			Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
			movement.Disable();
			spur.DeactivateNormalRenderer();
			spur.StopSnowDust();
			course.DeactivateShootingGuide(PlayerNo);
			course.ActivateReverseGuide(PlayerNo);
			LeanTween.move(base.gameObject, standingPoint, time).setOnUpdate((Action<float>)delegate
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, targetRotation, Time.deltaTime * 30f);
			}).setOnComplete((Action)delegate
			{
				shooting.StartShooting(target, CurrentLap);
			});
		}
	}
	public void TransitionShootingModeForAI()
	{
		if (mode != Mode.Shooting)
		{
			mode = Mode.Shooting;
			TransitionShootingModeRecursive();
		}
	}
	public void TransitionCrossCountryMode()
	{
		if (mode != 0)
		{
			target.IsUsing = false;
			movement.Enable();
			spur.ActivateNormalRenderer();
			mode = Mode.CrossCountry;
		}
	}
	public void NextLap()
	{
		CurrentLap++;
		if (CurrentLap < SingletonCustom<Biathlon_Courses>.Instance.Current.RaceLap)
		{
			course.ActivateShootingGuide(PlayerNo);
			course.DeactivateReverseGuide(PlayerNo);
			SingletonCustom<Biathlon_UI>.Instance.SetLapLabel(PlayerNo, CurrentLap);
			SingletonCustom<Biathlon_UI>.Instance.ShowFinalLap(PlayerNo);
			if (IsPlayer || SingletonCustom<Biathlon_GameMain>.Instance.PlayerNum == 3)
			{
				SingletonCustom<Biathlon_GameMain>.Instance.PlayLastLapSfx();
			}
		}
		else
		{
			IsGoal = true;
			IsLockPlacement = true;
			course.DeactivateReverseGuide(PlayerNo);
			SingletonCustom<Biathlon_GameMain>.Instance.CharacterGoal(PlayerNo);
		}
	}
	private void SearchSlope()
	{
		bool isForwardDownward = IsForwardDownward;
		bool isForwardUpward = IsForwardUpward;
		Vector3 position = base.transform.position;
		Vector3 forward = base.transform.forward;
		Vector3 vector = position + forward;
		vector.y += 0.3f;
		Ray ray = new Ray(vector, Vector3.down);
		UnityEngine.Debug.DrawRay(vector, Vector3.down * 0.5f, Color.red);
		if (!Physics.Raycast(ray, out RaycastHit hitInfo, 1f, 8388608))
		{
			IsForwardUpward = false;
			IsForwardDownward = false;
		}
		else
		{
			float num = hitInfo.point.y - position.y;
			if (num > 0.13f)
			{
				IsForwardUpward = true;
				IsForwardDownward = false;
			}
			if (num < -0.2f)
			{
				IsForwardDownward = true;
				IsForwardUpward = false;
			}
		}
		if (isForwardDownward != IsForwardDownward || isForwardUpward != IsForwardUpward)
		{
			UpdateSlope();
		}
	}
	private void UpdateSlope()
	{
		if (IsUpward)
		{
			movement.StartUpwardSlope();
			audio.StopGlidingSfx();
			if (camera != null)
			{
				camera.ActivateUpwardCamera();
			}
		}
		else if (IsDownward)
		{
			movement.StartDownwardSlope();
			audio.StartGlidingSfx();
			if (camera != null)
			{
				camera.ActivateDownwardCamera();
				allowSpeedEffectTime = Time.time + 1f;
			}
		}
		else
		{
			movement.EndSlope();
			audio.StartGlidingSfx();
			if (camera != null)
			{
				camera.ActivateCrossCountryCamera();
			}
		}
	}
	private void TransitionShootingModeRecursive()
	{
		Biathlon_Target preparedTarget = course.GetTargetForPrepare();
		preparedTarget.IsPrepare = true;
		Vector3 standPoint = preparedTarget.GetStandingPoint();
		Vector3 position = base.transform.position;
		position.y = standPoint.y;
		float time = Vector3.Distance(position, standPoint) / 10f;
		Vector3 forward = standPoint - position;
		Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
		movement.Disable();
		autoMoveForAI = LeanTween.move(base.gameObject, standPoint, time);
		autoMoveForAI.setOnUpdate((Action<float>)delegate
		{
			if (preparedTarget.IsUsing)
			{
				LeanTween.cancel(autoMoveForAI.id);
				preparedTarget.IsPrepare = false;
				autoMoveForAI = null;
				TransitionShootingModeRecursive();
			}
			else
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, targetRotation, Time.deltaTime * 30f);
			}
		});
		autoMoveForAI.setOnComplete((Action)delegate
		{
			if (preparedTarget.IsUsing)
			{
				autoMoveForAI = null;
				preparedTarget.IsPrepare = false;
				TransitionShootingModeRecursive();
			}
			else
			{
				target = preparedTarget;
				preparedTarget.IsPrepare = false;
				target.Activate();
				target.IsUsing = true;
				spur.DeactivateNormalRenderer();
				spur.StopSnowDust();
				course.DeactivateShootingGuide(PlayerNo);
				course.ActivateReverseGuide(PlayerNo);
				Vector3 relayPoint = preparedTarget.GetRelayPoint();
				LeanTween.rotate(to: Quaternion.LookRotation(standPoint - relayPoint, Vector3.up).eulerAngles, gameObject: base.gameObject, time: 0.5f).setOnComplete((Action)delegate
				{
					shooting.StartShooting(target, CurrentLap);
				});
			}
		});
	}
}
