using UnityEngine;
using UnityEngine.Extension;
using UnityStandardAssets.Utility;
public class Biathlon_CharacterMovement : MonoBehaviour
{
	private const string GoalTag = "Goal";
	[SerializeField]
	[DisplayName("設定")]
	private Biathlon_CharacterMovementConfig config;
	[SerializeField]
	private Rigidbody rigidbody;
	[SerializeField]
	private Biathlon_CharacterAnimator animator;
	[SerializeField]
	private Transform rotateRoot;
	[SerializeField]
	private Biathlon_SpurRenderer spur;
	[SerializeField]
	private Transform slipstreamSphereCenter;
	[SerializeField]
	private float maxSpeedLog;
	[SerializeField]
	private float slipstreamBonusLog;
	[SerializeField]
	private float slopeCorrectionLog;
	[SerializeField]
	private float lastMaxSpeedLog;
	private Biathlon_Character playingCharacter;
	private WaypointProgressTracker placeTracker;
	private Vector3 targetAngle;
	private Quaternion targetRotation;
	private float currentRunDistance;
	private float maxRunDistance;
	private float lastBonusChangeTime;
	private float maxBonus;
	private float speedBonus = 1f;
	private bool isChargingSlipstream;
	private bool isConsumeSlipstream;
	private float slipstreamPower;
	private float slipstreamBonusSpeed;
	private float slopeSpeedCorrection;
	private readonly Collider[] otherCharacterColliders = new Collider[8];
	private readonly RaycastHit[] otherCharacters = new RaycastHit[8];
	private WaypointProgressTracker aiTracker;
	public float CurrentRunDistance => currentRunDistance;
	public float MaxRunDistance => maxRunDistance;
	public bool IsReverseRun
	{
		get
		{
			float num = maxRunDistance - currentRunDistance;
			if (num > 8f)
			{
				if (num > 12f)
				{
					maxRunDistance = currentRunDistance + 9f;
				}
				return true;
			}
			return false;
		}
	}
	public float Speed => rigidbody.velocity.magnitude / config.MoveSpeed;
	private void OnTriggerEnter(Collider other)
	{
		if (!playingCharacter.IsGoal && other.gameObject.CompareTag("Goal"))
		{
			int num = CurrentLap();
			if (playingCharacter.CurrentLap < num)
			{
				playingCharacter.NextLap();
			}
		}
	}
	public void Init(Biathlon_Character character, Biathlon_Course course)
	{
		playingCharacter = character;
		placeTracker = playingCharacter.PlacementTracker;
		aiTracker = playingCharacter.AITracker;
		base.transform.position = course.GetCharacterStartPosition(playingCharacter.PlayerNo);
		rigidbody.maxDepenetrationVelocity = 1f;
		animator.Init(character);
		spur.Init(character);
		lastBonusChangeTime = -1000f;
	}
	public void Enable()
	{
		rigidbody.velocity = Vector3.zero;
		rigidbody.isKinematic = false;
	}
	public void Disable()
	{
		rigidbody.velocity = Vector3.zero;
		rigidbody.isKinematic = true;
	}
	public void UpdateMethod()
	{
		if (playingCharacter.ActionMode != Biathlon_Character.Mode.Shooting)
		{
			currentRunDistance = placeTracker.runDistance;
			if (maxRunDistance < currentRunDistance)
			{
				maxRunDistance = currentRunDistance;
			}
			animator.UpdateMethod();
			spur.UpdateMethod();
			if (IsReverseRun)
			{
				SingletonCustom<Biathlon_UI>.Instance.ShowReverseRun(playingCharacter.PlayerNo);
			}
			else
			{
				SingletonCustom<Biathlon_UI>.Instance.HideReverseRun(playingCharacter.PlayerNo);
			}
		}
	}
	public void FixedUpdateMethod()
	{
		if (playingCharacter.ActionMode != Biathlon_Character.Mode.Shooting)
		{
			SlipstreamCharge();
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
	public void StartUpwardSlope()
	{
		animator.EnterUphill();
		animator.ExitDownhill();
		spur.ActivateUpwardSlopeRenderer();
	}
	public void StartDownwardSlope()
	{
		animator.EnterDownhill();
		animator.ExitUphill();
		spur.ActivateNormalRenderer();
	}
	public void EndSlope()
	{
		animator.ExitUphill();
		animator.ExitDownhill();
		spur.ActivateNormalRenderer();
	}
	private void UpdateMethodForPlayer()
	{
		if (playingCharacter.IsGoal)
		{
			UpdateMethodForAI();
		}
		else if (SingletonCustom<Biathlon_GameMain>.Instance.IsDuringGame)
		{
			bool acceleration = SingletonCustom<Biathlon_Input>.Instance.IsPressButtonA(playingCharacter.ControlUser);
			float angular = SingletonCustom<Biathlon_Input>.Instance.GetAngular(playingCharacter.ControlUser);
			UpdateRigidbody(acceleration, angular);
		}
	}
	private void UpdateMethodForAI()
	{
		if (SingletonCustom<Biathlon_GameMain>.Instance.IsGameStarted)
		{
			float x = (Quaternion.Euler(0f, 0f - base.transform.eulerAngles.y, 0f) * aiTracker.target.forward).x;
			Vector3 lhs = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f) * Vector3.forward;
			Vector3 rhs = aiTracker.progressPoint.position - base.transform.position;
			rhs.y = 0f;
			Vector3 vector = Vector3.Cross(lhs, rhs);
			float angular = Mathf.Clamp(x * 2.5f + vector.y * 1f, -1f, 1f);
			UpdateRigidbody(acceleration: true, angular);
		}
	}
	private void UpdateRigidbody(bool acceleration, float angular)
	{
		UpdateRotation();
		Vector3 vector = rigidbody.velocity;
		float num = vector.y += Physics.gravity.y * Time.fixedDeltaTime;
		if (acceleration)
		{
			float num2 = (maxSpeedLog = GetMaxSpeed()) + (slipstreamBonusLog = GetSlipstreamBonus());
			num2 += (slopeCorrectionLog = GetSlopeCorrection());
			if (!playingCharacter.IsPlayer)
			{
				switch (SingletonCustom<Biathlon_GameMain>.Instance.AIStrength)
				{
				case Biathlon_Definition.AIStrength.Easy:
					num2 *= config.EasyCpuMoveSpeedMultiplier;
					break;
				case Biathlon_Definition.AIStrength.Normal:
					num2 *= config.NormalCpuMoveSpeedMultiplier;
					break;
				case Biathlon_Definition.AIStrength.Hard:
					num2 *= config.HardCpuMoveSpeedMultiplier;
					break;
				}
			}
			lastMaxSpeedLog = num2;
			Vector3 a = base.transform.forward * num2;
			a *= Time.fixedDeltaTime;
			if (a.magnitude < 0.1f)
			{
				a *= 2f;
			}
			vector += a;
			if (vector.magnitude > num2)
			{
				vector = num2 * vector.normalized;
			}
			rigidbody.velocity = vector;
			spur.PlaySnowDust();
			animator.PlayRun();
		}
		else
		{
			if (vector.magnitude < 0.1f)
			{
				vector = Vector3.zero;
			}
			else
			{
				Vector3 rhs = vector;
				Vector3 normalized = vector.normalized;
				vector -= normalized * 2f * Time.fixedDeltaTime;
				if (Vector3.Dot(vector, rhs) < 0f)
				{
					vector = Vector3.zero;
				}
			}
			rigidbody.velocity = vector;
			if (vector.magnitude < 1f)
			{
				spur.StopSnowDust();
			}
			animator.StopRun();
		}
		if (!Mathf.Approximately(angular, 0f))
		{
			float angle = angular * config.AngularSpeed * Time.fixedDeltaTime;
			base.transform.Rotate(base.transform.up, angle);
		}
		float magnitude = vector.magnitude;
		float animationSpeed = Mathf.Clamp01(magnitude * 2f / config.MoveSpeed);
		animator.AnimationSpeed = animationSpeed;
		if (!(magnitude < 1.5f))
		{
			float num3 = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			float num4 = base.transform.eulerAngles.y - num3;
			if (num4 < -180f)
			{
				num4 += 360f;
			}
			else if (num4 > 180f)
			{
				num4 -= 360f;
			}
			if (!acceleration)
			{
				num4 *= 1.5f;
			}
			rigidbody.velocity = Quaternion.Euler(0f, num4 * Time.fixedDeltaTime, 0f) * vector;
		}
	}
	private void UpdateRotation()
	{
		if (Physics.Raycast(new Ray(base.transform.position, Vector3.down), out RaycastHit hitInfo, 10f, 8388608))
		{
			Quaternion lhs = Quaternion.FromToRotation(rotateRoot.up, hitInfo.normal);
			Quaternion rotation = rotateRoot.rotation;
			targetRotation = lhs * rotation;
			Quaternion rotation2 = Quaternion.RotateTowards(rotation, targetRotation, Time.deltaTime * config.RotationSpeed);
			rotateRoot.rotation = rotation2;
		}
	}
	private void SlipstreamCharge()
	{
		if (!isConsumeSlipstream)
		{
			if (playingCharacter.IsDownward)
			{
				isChargingSlipstream = false;
				slipstreamPower = 0f;
			}
			else if (Physics.OverlapSphereNonAlloc(slipstreamSphereCenter.position, config.SlipstreamSphereRadius, otherCharacterColliders, 1073741824) > 0)
			{
				isChargingSlipstream = true;
				slipstreamPower += Time.deltaTime * config.SlipstreamChargeSpeed;
				slipstreamPower = Mathf.Max(slipstreamPower, config.SlipstreamChargeMax);
			}
			else if (Physics.SphereCastNonAlloc(new Ray(slipstreamSphereCenter.position, slipstreamSphereCenter.forward), config.SlipstreamSphereRadius, otherCharacters, config.SlipstreamSphereLength, 1073741824) > 0)
			{
				isChargingSlipstream = true;
				slipstreamPower += Time.deltaTime * config.SlipstreamChargeSpeed;
				slipstreamPower = Mathf.Max(slipstreamPower, config.SlipstreamChargeMax);
			}
			else
			{
				isChargingSlipstream = false;
			}
		}
	}
	private int CurrentLap()
	{
		return Mathf.FloorToInt(currentRunDistance / SingletonCustom<Biathlon_Courses>.Instance.Current.Length + 0.5f);
	}
	private float GetMaxSpeed()
	{
		if (Time.time > lastBonusChangeTime + 5f)
		{
			maxBonus = config.MoveSpeedCorrectionsByPlacement[playingCharacter.Placement];
			lastBonusChangeTime = Time.time;
		}
		float maxDelta = maxBonus * Time.deltaTime * config.MoveSpeedCorrectionChangeSpeed;
		speedBonus = Mathf.MoveTowards(speedBonus, maxBonus, maxDelta);
		speedBonus = Mathf.Max(speedBonus, 1f);
		return config.MoveSpeed * speedBonus;
	}
	private float GetSlipstreamBonus()
	{
		if (isChargingSlipstream)
		{
			return 0f;
		}
		if (isConsumeSlipstream)
		{
			slipstreamPower -= Time.deltaTime;
			if (slipstreamPower < 0f)
			{
				slipstreamPower = 0f;
				isConsumeSlipstream = false;
			}
		}
		else
		{
			if (playingCharacter.IsUpward)
			{
				return 0f;
			}
			if (slipstreamPower >= config.SlipStreamBonusUseThreshold)
			{
				isConsumeSlipstream = true;
			}
		}
		float target = isConsumeSlipstream ? config.SlipStreamBonusSpeed : 0f;
		slipstreamBonusSpeed = Mathf.MoveTowards(slipstreamBonusSpeed, target, Time.deltaTime * config.SlipStreamBonusSpeed);
		return Mathf.Max(slipstreamBonusSpeed, 0f);
	}
	private float GetSlopeCorrection()
	{
		if (playingCharacter.IsUpward)
		{
			slopeSpeedCorrection = Mathf.MoveTowards(slopeSpeedCorrection, config.UpwardSpeedCorrection, Mathf.Abs(config.UpwardSpeedCorrection) * Time.deltaTime);
		}
		else if (playingCharacter.IsDownward)
		{
			slopeSpeedCorrection = Mathf.MoveTowards(slopeSpeedCorrection, config.DownwardSpeedCorrection, config.DownwardSpeedCorrection * Time.deltaTime);
		}
		else
		{
			slopeSpeedCorrection = Mathf.MoveTowards(slopeSpeedCorrection, 0f, Time.deltaTime);
		}
		return Mathf.Max(slopeSpeedCorrection, 0f);
	}
}
