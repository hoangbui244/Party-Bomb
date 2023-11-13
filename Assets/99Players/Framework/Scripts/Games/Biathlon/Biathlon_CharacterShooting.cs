using System;
using UnityEngine;
using UnityEngine.Extension;
public class Biathlon_CharacterShooting : MonoBehaviour
{
	private static readonly Vector2 FullResolution = new Vector2(1920f, 1080f);
	private static readonly Vector3[] AimingPositions = new Vector3[5]
	{
		new Vector3(0.52f, 0f, 0f),
		new Vector3(0.26f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(-0.26f, 0f, 0f),
		new Vector3(-0.52f, 0f, 0f)
	};
	private static readonly Vector3[] ShootingCameraPosition = new Vector3[8]
	{
		new Vector3(1.9f, 6.3f, -102f),
		new Vector3(3.95f, 6.3f, -102f),
		new Vector3(5.85f, 6.3f, -102f),
		new Vector3(7.75f, 6.3f, -102f),
		new Vector3(9.65f, 6.3f, -102f),
		new Vector3(11.55f, 6.3f, -102f),
		new Vector3(13.45f, 6.3f, -102f),
		new Vector3(15.35f, 6.3f, -102f)
	};
	[SerializeField]
	[DisplayName("設定")]
	private Biathlon_CharacterShootingConfig config;
	[SerializeField]
	private Biathlon_CharacterAnimator animator;
	[SerializeField]
	private Biathlon_CharacterRenderer renderer;
	[SerializeField]
	private ParticleSystem hitEffect;
	[SerializeField]
	private Biathlon_CharacterAudio audio;
	[SerializeField]
	private GameObject bullet;
	[SerializeField]
	private bool debugAim;
	[SerializeField]
	private Vector3 debugAimPosition;
	private Biathlon_Target target;
	private int hitCount;
	private Biathlon_CharacterCamera camera;
	private Biathlon_Character playingCharacter;
	private Vector3 targetPosition;
	private Vector3 targetFollowPosition;
	private bool isShooting;
	private bool isStand;
	private Vector3 aimPosition;
	private Vector2 centerPosition;
	private Vector2 movePosition;
	private Vector2 cameraSize;
	private Rect moveLimit;
	private Vector2 noiseSeed;
	private float nextShootTime;
	private float shootingStartTime;
	private int prevShootingLap = -999;
	private void OnTriggerEnter(Collider other)
	{
		if (!playingCharacter.IsGoal)
		{
			if (other.gameObject.CompareTag("CheckPoint"))
			{
				TransitionToShootingMode(other);
			}
			else if (other.gameObject.CompareTag("GoalHoop"))
			{
				TransitionToShootingModeForAI(other);
			}
		}
	}
	public void Init(Biathlon_Character character)
	{
		playingCharacter = character;
		camera = SingletonCustom<Biathlon_CameraRegistry>.Instance.GetCamera(playingCharacter.PlayerNo);
		centerPosition = FullResolution / 2f;
		aimPosition = centerPosition;
		noiseSeed = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
		nextShootTime = -999f;
		bullet.SetActive(value: false);
	}
	public void StartShooting(Biathlon_Target shootTarget, int lap)
	{
		target = shootTarget;
		targetPosition = target.GetStandingPoint();
		isStand = (lap == 0);
		SingletonCustom<Biathlon_UI>.Instance.PlayFade(playingCharacter.PlayerNo, fadeOut: ActivateShootingMode, duration: 0.5f, keep: 0.5f, fadeIn: Zooming);
		hitCount = 0;
		shootingStartTime = Time.time;
	}
	public void UpdateMethod()
	{
		if (playingCharacter.ActionMode != 0 && isShooting)
		{
			if (playingCharacter.IsPlayer)
			{
				UpdateMethodForPlayer();
			}
			else
			{
				UpdateMethodForAI();
			}
		}
	}
	private void ActivateShootingMode()
	{
		Vector3 position = targetPosition;
		base.transform.position = position;
		camera.ActivateRifleShootingCamera();
		if (isStand)
		{
			base.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
			animator.PoseStandShooting();
			renderer.SetAsStandRifleShootingMode();
		}
		else
		{
			base.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			animator.PoseLieDownShooting();
			renderer.SetAsLieRifleShootingMode();
		}
		aimPosition = centerPosition;
		camera.ShootingFollowTarget.parent.position = target.AimSupport.position;
		camera.ShootingFollowTarget.localPosition = AimingPositions[0];
		camera.ShootingCamera.transform.localPosition = ShootingCameraPosition[target.Number];
		SingletonCustom<Biathlon_UI>.Instance.UpdateReticlePosition(playingCharacter.PlayerNo, aimPosition);
		SingletonCustom<Biathlon_UI>.Instance.ActivateShootingModeUI(playingCharacter.PlayerNo);
		SingletonCustom<Biathlon_UI>.Instance.HideReverseRun(playingCharacter.PlayerNo);
	}
	private void DeactivateShootingMode()
	{
		Vector3 escapePoint = target.GetEscapePoint();
		base.transform.position = escapePoint;
		Vector3 lookAtPosition = SingletonCustom<Biathlon_Courses>.Instance.Current.LookAtPosition;
		lookAtPosition.y = base.transform.position.y;
		Vector3 forward = lookAtPosition - escapePoint;
		base.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
		SingletonCustom<Biathlon_UI>.Instance.ActivateCrossCountryModeUI(playingCharacter.PlayerNo);
		camera.ActivateCrossCountryCamera();
		renderer.SetAsCrossCountryMode();
		animator.StopPose();
	}
	private void Zooming()
	{
		Biathlon_CinemachineCameraZoom zoom = camera.CameraZoom;
		LeanTween.value(base.gameObject, 60f, 10f, 1f).setOnUpdate(delegate(float v)
		{
			zoom.Fov = v;
		}).setOnComplete((Action)delegate
		{
			isShooting = true;
		});
	}
	private void UpdateMethodForPlayer()
	{
		Transform shootingFollowTarget = camera.ShootingFollowTarget;
		Vector2 vector = SingletonCustom<Biathlon_Input>.Instance.GetMoveVector(playingCharacter.ControlUser) * Time.deltaTime * config.MoveSpeed;
		Vector3 localPosition = shootingFollowTarget.localPosition;
		localPosition.x += 0f - vector.x;
		localPosition.x = Mathf.Clamp(localPosition.x, -0.7f, 0.7f);
		localPosition.y += vector.y;
		localPosition.y = Mathf.Clamp(localPosition.y, -0.154f, 0.154f);
		targetFollowPosition = localPosition;
		Vector3 a = Vector3.MoveTowards(localPosition, targetFollowPosition, Time.deltaTime * config.DeltaSpeed);
		shootingFollowTarget.localPosition = a + GetNoise();
		if (SingletonCustom<Biathlon_Input>.Instance.IsPressDownButtonA(playingCharacter.ControlUser))
		{
			Shot();
		}
	}
	private void UpdateMethodForAI()
	{
		if (hitCount >= 5)
		{
			return;
		}
		Transform shootingFollowTarget = camera.ShootingFollowTarget;
		Vector3 localPosition = shootingFollowTarget.localPosition;
		Vector3 vector = AimingPositions[hitCount];
		Vector3 noise = GetNoise();
		Vector3 b = localPosition + noise * config.CpuNoiseCanceler;
		Vector2 vector2 = (Vector2)(vector - b).normalized * Time.deltaTime * config.MoveSpeed;
		localPosition.x += vector2.x;
		localPosition.x = Mathf.Clamp(localPosition.x, -0.7f, 0.7f);
		localPosition.y += vector2.y;
		localPosition.y = Mathf.Clamp(localPosition.y, -0.154f, 0.154f);
		targetFollowPosition = localPosition;
		Vector3 a = Vector3.MoveTowards(localPosition, targetFollowPosition, Time.deltaTime * config.DeltaSpeed);
		shootingFollowTarget.localPosition = a + GetNoise();
		if (Time.time < nextShootTime || Vector3.Distance(shootingFollowTarget.localPosition, vector) > 0.03f)
		{
			return;
		}
		if (SingletonCustom<Biathlon_GameMain>.Instance.PlayerNum == 3)
		{
			Shot();
			return;
		}
		Collider hitCollider = target.GetHitCollider(hitCount);
		if (!target.TryBreak(hitCollider.transform))
		{
			nextShootTime = Time.time + config.ShootIntervalByMiss;
			LeanTween.move(bullet, shootingFollowTarget.position, config.ShootTime).setOnComplete((Action)delegate
			{
				bullet.SetActive(value: false);
			});
		}
		else
		{
			nextShootTime = Time.time + config.ShootInterval;
			LeanTween.move(bullet, shootingFollowTarget.position, config.ShootTime).setOnComplete((Action)delegate
			{
				bullet.SetActive(value: false);
				hitCount++;
				if (hitCount >= 5)
				{
					isShooting = false;
					LeanTween.delayedCall(0.5f, (Action)delegate
					{
						DeactivateShootingMode();
						LeanTween.delayedCall(1f, (Action)delegate
						{
							playingCharacter.TransitionCrossCountryMode();
						});
					});
				}
			});
		}
	}
	private void Shot()
	{
		if (Time.time < nextShootTime)
		{
			return;
		}
		if (playingCharacter.IsPlayer)
		{
			SingletonCustom<HidVibration>.Instance.SetCustomVibration(playingCharacter.PlayerNo, HidVibration.VibrationType.Strong, 0.05f);
		}
		audio.PlayShotSfx();
		Vector3 scaledScreenPoint = camera.GetScaledScreenPoint(aimPosition);
		if (debugAim)
		{
			scaledScreenPoint = debugAimPosition;
		}
		Ray ray = camera.ScreenPointToRay(scaledScreenPoint);
		UnityEngine.Debug.DrawRay(ray.origin, ray.direction * 60f, Color.red, 5f);
		bullet.SetActive(value: true);
		bullet.transform.position = camera.transform.position;
		bullet.transform.AddPositionY(-0.2f);
		Transform followTarget = camera.ShootingFollowTarget;
		if (Raycast(ray, out Biathlon_Target biathlon_Target, out Transform col))
		{
			if (biathlon_Target == null)
			{
				nextShootTime = Time.time + config.ShootIntervalByMiss;
				LeanTween.move(bullet, followTarget.position, config.ShootTime).setOnComplete((Action)delegate
				{
					bullet.SetActive(value: false);
					hitEffect.transform.position = followTarget.position;
					hitEffect.Play(withChildren: true);
					audio.PlayMissHitSfx();
					SingletonCustom<Biathlon_UI>.Instance.ShowMiss(playingCharacter.PlayerNo);
					SingletonCustom<Biathlon_UI>.Instance.ShowReloadGauge(playingCharacter.PlayerNo, nextShootTime);
				});
			}
			else if (!biathlon_Target.TryBreak(col))
			{
				nextShootTime = Time.time + config.ShootIntervalByMiss;
				LeanTween.move(bullet, followTarget.position, config.ShootTime).setOnComplete((Action)delegate
				{
					bullet.SetActive(value: false);
					hitEffect.transform.position = followTarget.position;
					hitEffect.Play(withChildren: true);
					audio.PlayMissHitSfx();
					SingletonCustom<Biathlon_UI>.Instance.ShowMiss(playingCharacter.PlayerNo);
					SingletonCustom<Biathlon_UI>.Instance.ShowReloadGauge(playingCharacter.PlayerNo, nextShootTime);
				});
			}
			else
			{
				nextShootTime = Time.time + config.ShootInterval;
				LeanTween.move(bullet, followTarget.position, config.ShootTime).setOnComplete((Action)delegate
				{
					bullet.SetActive(value: false);
					hitEffect.transform.position = followTarget.position;
					hitEffect.Play(withChildren: true);
					audio.PlayHitSfx();
					SingletonCustom<Biathlon_UI>.Instance.ShowHit(playingCharacter.PlayerNo);
					SingletonCustom<Biathlon_UI>.Instance.ShowReloadGauge(playingCharacter.PlayerNo, nextShootTime);
					hitCount++;
					if (hitCount >= 5)
					{
						isShooting = false;
						SingletonCustom<Biathlon_UI>.Instance.PlayFade(playingCharacter.PlayerNo, fadeOut: DeactivateShootingMode, duration: 0.5f, keep: 0.5f, fadeIn: playingCharacter.TransitionCrossCountryMode);
					}
				});
			}
		}
		else
		{
			nextShootTime = Time.time + config.ShootIntervalByMiss;
			LeanTween.move(bullet, followTarget.position, config.ShootTime).setOnComplete((Action)delegate
			{
				bullet.SetActive(value: false);
				SingletonCustom<Biathlon_UI>.Instance.ShowMiss(playingCharacter.PlayerNo);
				SingletonCustom<Biathlon_UI>.Instance.ShowReloadGauge(playingCharacter.PlayerNo, nextShootTime);
				if (col != null)
				{
					audio.PlayMissHitSfx();
				}
			});
		}
	}
	private bool Raycast(Ray ray, out Biathlon_Target target, out Transform col)
	{
		target = null;
		col = null;
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 60f))
		{
			col = hitInfo.transform;
			target = hitInfo.transform.GetComponentInParent<Biathlon_Target>();
		}
		return target != null;
	}
	private Vector3 GetNoise()
	{
		float x = Time.time * config.NoiseSpeed;
		float x2 = Mathf.PerlinNoise(x, noiseSeed.x) - 0.5f;
		float y = Mathf.PerlinNoise(x, noiseSeed.y) - 0.5f;
		float t = Mathf.InverseLerp(20f, 0f, Time.time - shootingStartTime);
		float num = Mathf.Lerp(0.5f, 1f, t);
		return new Vector3(x2, y, 0f) * (config.NoiseScale * num);
	}
	private void TransitionToShootingMode(Collider other)
	{
		if (playingCharacter.IsPlayer && prevShootingLap < playingCharacter.CurrentLap)
		{
			Biathlon_TargetEnterPoint component = other.GetComponent<Biathlon_TargetEnterPoint>();
			if (component.CanEnter())
			{
				prevShootingLap = playingCharacter.CurrentLap;
				playingCharacter.TransitionShootingMode(component.Target);
			}
		}
	}
	private void TransitionToShootingModeForAI(Collider other)
	{
		if (!playingCharacter.IsPlayer && prevShootingLap < playingCharacter.CurrentLap)
		{
			playingCharacter.TransitionShootingModeForAI();
		}
	}
}
