using GamepadInput;
using System;
using UnityEngine;
using UnityStandardAssets.Utility;
public class Canoe_Player : MonoBehaviour
{
	private Rigidbody rigid;
	[SerializeField]
	[Header("キャラクラス")]
	private Canoe_Character character;
	[SerializeField]
	[Header("エフェクトクラス")]
	private Canoe_PlayerEffect playerEffect;
	[SerializeField]
	[Header("AnimationEventor")]
	private Canoe_AnimationEventor animationEventor;
	[SerializeField]
	[Header("カヌ\u30fc")]
	private MeshRenderer canoe;
	[SerializeField]
	[Header("カヌ\u30fcパドル")]
	private MeshRenderer canoePaddle;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private CapsuleCollider collider;
	private float colliderRadius;
	private int onObjectLayerMask;
	private string[] onObjectLayerMaskNameList = new string[1]
	{
		"Wall, HitCharacterOnly"
	};
	private Vector3 characterOriginPos;
	private Vector3 canoeOriginPos;
	private int playerNo;
	private Canoe_Define.UserType userType;
	private int npadId;
	private bool isGoal;
	private float goalTime;
	private bool isOnceInput;
	private Vector3 moveDir;
	private float acceleInputLerp;
	private float acceleInputContinueTime;
	private float acceleMoveSpeed;
	private float fallBackInputLerp;
	private float fallBackInputContinueTime;
	private float fallBackMoveSpeed;
	private float diffMoveSpeed;
	private int acceleMoveDir;
	private Canoe_AddSpeedUp addSpeedUp;
	private Canoe_AddSpeedUp.Type addSpeedUpType;
	private float addSpeed;
	private float addSlipStreamSpeed;
	private bool isSlipStream;
	private bool isFall;
	private bool isCanFall;
	private Vector3 beforeFallVelocity;
	private float fallVelocityY;
	private bool isMoveSe;
	[SerializeField]
	[Header("WaypointProgressTracker（順位判定用）")]
	private WaypointProgressTracker wptPos;
	private bool isFallCollisionWater;
	[SerializeField]
	[Header("水判定用のコライダ\u30fc")]
	private CapsuleCollider waterCollider;
	private bool isMoveGoalAnchor;
	private Vector3 moveGoalAnchorPos;
	private bool isUseStaminaDash;
	private bool isUseUpStamina;
	private float staminaValue;
	private float addStaminaUseAcceleSpeed;
	private float staminaHealWaitTime;
	private bool isCpu;
	private Canoe_AI cpuAI;
	[SerializeField]
	[Header("WaypointProgressTracker（AI用）")]
	private WaypointProgressTracker wptAI;
	[SerializeField]
	[Header("障害物判定用のアンカ\u30fc")]
	private Transform obstacleCheckAnchor;
	public void Init(int _playerNo, int _userType)
	{
		playerNo = _playerNo;
		userType = (Canoe_Define.UserType)_userType;
		isCpu = (userType >= Canoe_Define.UserType.CPU_1);
		rigid = GetComponent<Rigidbody>();
		animationEventor.Init(this);
		character.Init(this);
		character.SetStyle(_userType);
		characterOriginPos = character.transform.localPosition;
		canoeOriginPos = canoe.transform.localPosition;
		colliderRadius = collider.radius * collider.transform.localScale.x * base.transform.localScale.x;
		onObjectLayerMask = LayerMask.GetMask(onObjectLayerMaskNameList);
		playerEffect.Init();
		Transform playerAnchor = SingletonCustom<Canoe_CourseManager>.Instance.GetPlayerAnchor(playerNo);
		base.transform.position = playerAnchor.position;
		base.transform.rotation = playerAnchor.rotation;
		wptPos.setCircuit = SingletonCustom<Canoe_CourseManager>.Instance.GetWaypointCircuitPos();
		wptPos.progressDistance = 0f;
		isUseUpStamina = false;
		staminaValue = 1f;
		if (!GetIsCpu())
		{
			wptAI.gameObject.SetActive(value: false);
			return;
		}
		cpuAI = base.gameObject.AddComponent<Canoe_AI>();
		cpuAI.Init(this);
		cpuAI.SetWaypointCircuitAI(wptAI);
	}
	public void SetCanoeMaterial(Material _mat)
	{
		canoe.material = _mat;
		canoePaddle.material = _mat;
	}
	public void SetStartAnimation(float _startTime)
	{
		animationEventor.SetStartAnimation(_startTime);
	}
	public void FixedUpdateMethod()
	{
		if (isGoal)
		{
			Vector3 vector = moveGoalAnchorPos - base.transform.position;
			vector.y = 0f;
			moveDir = vector.normalized;
		}
		else
		{
			if (isFall)
			{
				fallVelocityY = rigid.velocity.y;
			}
			waterCollider.transform.SetLocalEulerAnglesX(0f - base.transform.localEulerAngles.x);
			if (diffMoveSpeed == 0f)
			{
				return;
			}
		}
		Move();
	}
	public void UpdateMethod()
	{
		if (isGoal)
		{
			float num = 0.75f;
			if (acceleInputLerp != num)
			{
				if (acceleInputLerp > num)
				{
					acceleInputLerp -= Time.deltaTime;
					acceleInputLerp = Mathf.Clamp(acceleInputLerp, num, 1f);
				}
				else if (acceleInputLerp < num)
				{
					acceleInputLerp += Time.deltaTime;
					acceleInputLerp = Mathf.Clamp(acceleInputLerp, 0f, num);
				}
				diffMoveSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.ClampAcceleMoveSpeed(acceleInputLerp);
			}
			return;
		}
		if (!GetIsCpu())
		{
			npadId = ((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? playerNo : 0);
			SetUseStaminaInput();
			SetCameraTypeChangeInput();
			SetMoveInput();
			SetMoveDir(GetStickDir());
		}
		else
		{
			cpuAI.UpdateMethod();
		}
		if (addStaminaUseAcceleSpeed > acceleMoveSpeed)
		{
			acceleMoveSpeed = addStaminaUseAcceleSpeed;
			SingletonCustom<Canoe_CameraManager>.Instance.PlayAddUseStaminaDashEffect(playerNo);
		}
		else
		{
			SingletonCustom<Canoe_CameraManager>.Instance.StopAddUseStaminaDashEffect(playerNo);
		}
		diffMoveSpeed = Mathf.Abs(acceleMoveSpeed - fallBackMoveSpeed);
		acceleMoveDir = ((acceleMoveSpeed >= fallBackMoveSpeed) ? 1 : (-1));
		playerEffect.SetMoveEffectDir(acceleMoveDir);
		playerEffect.SetPaddleEffectDir(acceleMoveDir);
		if (!isFall)
		{
			if (acceleMoveDir == 1)
			{
				animationEventor.PlayRowingAnimation();
			}
			else
			{
				animationEventor.PlayRowingBackAnimation();
			}
		}
		if (addSpeedUp == null)
		{
			if (addSpeed > 0f)
			{
				addSpeed -= Time.deltaTime;
				if (addSpeed < 0f)
				{
					addSpeed = 0f;
				}
			}
		}
		else
		{
			addSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxAddSpeed(addSpeedUpType);
		}
		if (diffMoveSpeed > 0f && acceleMoveDir == 1)
		{
			SetSlipStreamAddSpeed();
		}
		if (!isSlipStream && addSlipStreamSpeed > 0f)
		{
			addSlipStreamSpeed -= Time.deltaTime;
			if (addSlipStreamSpeed < 0f)
			{
				addSlipStreamSpeed = 0f;
			}
		}
		if (!isUseStaminaDash && addStaminaUseAcceleSpeed > 0f)
		{
			addStaminaUseAcceleSpeed -= Time.deltaTime;
			if (addStaminaUseAcceleSpeed < 0f)
			{
				addStaminaUseAcceleSpeed = 0f;
			}
		}
		if (addSpeed > 0f || addSlipStreamSpeed > 0f)
		{
			SingletonCustom<Canoe_CameraManager>.Instance.PlayAddSpeedEffect(playerNo);
		}
		else
		{
			SingletonCustom<Canoe_CameraManager>.Instance.StopAddSpeedEffect(playerNo);
		}
		if (diffMoveSpeed == 0f)
		{
			animationEventor.SetRowingAnimationSpeed(0f);
		}
		if (SingletonCustom<Canoe_GameManager>.Instance.GetIsViewCamera(playerNo))
		{
			SingletonCustom<Canoe_UIManager>.Instance.SetStaminaGauge(playerNo, staminaValue / 1f, isUseUpStamina);
		}
	}
	private void SetUseStaminaInput()
	{
		if (!isUseUpStamina && SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.X))
		{
			UseStaminaDash();
		}
		else
		{
			NoneUseStaminaDash();
		}
	}
	public void UseStaminaDash()
	{
		if (!GetIsCpu() && !isUseStaminaDash)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_shorttrack_maxspeed");
			SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
			playerEffect.PlayAccelerationEffect();
		}
		isUseStaminaDash = true;
		addStaminaUseAcceleSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.GetStaminaUseAcceleMoveSpeed();
		staminaValue -= Time.deltaTime * SingletonCustom<Canoe_PlayerManager>.Instance.GetStaminaUseSpeed();
		if (staminaValue < 0f)
		{
			isUseStaminaDash = false;
			staminaValue = 0f;
			isUseUpStamina = true;
			staminaHealWaitTime = SingletonCustom<Canoe_PlayerManager>.Instance.GetUseUpStaminaHealWaitTime();
			playerEffect.PlaySweatEffect();
		}
	}
	public void NoneUseStaminaDash()
	{
		isUseStaminaDash = false;
		if (isUseUpStamina)
		{
			if (staminaHealWaitTime > 0f)
			{
				staminaHealWaitTime -= Time.deltaTime;
			}
			else
			{
				staminaValue += SingletonCustom<Canoe_PlayerManager>.Instance.GetStaminaHealSpeed() * SingletonCustom<Canoe_PlayerManager>.Instance.GetUseUpStaminaHealSpeedMag() * Time.deltaTime;
			}
		}
		else
		{
			staminaValue += SingletonCustom<Canoe_PlayerManager>.Instance.GetStaminaHealSpeed() * Time.deltaTime;
		}
		if (staminaValue > 1f)
		{
			staminaValue = 1f;
		}
		if (isUseUpStamina && staminaValue > SingletonCustom<Canoe_PlayerManager>.Instance.GetUseUpStaminaReUseValue())
		{
			isUseUpStamina = false;
			playerEffect.StopSweatEffect();
		}
	}
	private void SetCameraTypeChangeInput()
	{
		bool flag = false;
		bool flag2 = false;
		flag = (SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.LeftTrigger) || SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.LeftShoulder));
		flag2 = (SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.RightTrigger) || SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.RightShoulder));
		if (flag | flag2)
		{
			SingletonCustom<Canoe_CameraManager>.Instance.ChangeCameraType(playerNo, (!flag) ? 1 : (-1));
		}
	}
	private void SetMoveInput()
	{
		if (!isOnceInput && SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A))
		{
			SetIsOnceInput();
		}
		if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.A))
		{
			AcceleMoveInput();
		}
		else
		{
			AcceleMoveNoneInput();
		}
		if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.B))
		{
			FallBackMoveInput();
		}
		else
		{
			FallBackMoveNoneInput();
		}
	}
	public void AcceleMoveInput()
	{
		acceleInputContinueTime += Time.deltaTime;
		if (acceleInputContinueTime > SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxInputContinueTime())
		{
			acceleInputContinueTime = SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxInputContinueTime();
		}
		acceleInputLerp = SingletonCustom<Canoe_PlayerManager>.Instance.GetInputContinueLerp(acceleInputContinueTime);
		acceleMoveSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.ClampAcceleMoveSpeed(acceleInputLerp);
	}
	public void AcceleMoveNoneInput()
	{
		acceleInputContinueTime -= Time.deltaTime;
		if (acceleInputContinueTime < 0f)
		{
			acceleInputContinueTime = 0f;
		}
		acceleInputLerp = SingletonCustom<Canoe_PlayerManager>.Instance.GetInputContinueLerp(acceleInputContinueTime);
		acceleMoveSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.ClampAcceleMoveSpeed(acceleInputLerp);
	}
	public void FallBackMoveInput()
	{
		fallBackInputContinueTime += Time.deltaTime;
		if (fallBackInputContinueTime > SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxInputContinueTime())
		{
			fallBackInputContinueTime = SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxInputContinueTime();
		}
		fallBackInputLerp = SingletonCustom<Canoe_PlayerManager>.Instance.GetInputContinueLerp(fallBackInputContinueTime);
		fallBackMoveSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.ClampFallBackMoveSpeed(fallBackInputLerp);
	}
	public void FallBackMoveNoneInput()
	{
		fallBackInputContinueTime -= Time.deltaTime;
		if (fallBackInputContinueTime < 0f)
		{
			fallBackInputContinueTime = 0f;
		}
		fallBackInputLerp = SingletonCustom<Canoe_PlayerManager>.Instance.GetInputContinueLerp(fallBackInputContinueTime);
		fallBackMoveSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.ClampFallBackMoveSpeed(fallBackInputLerp);
	}
	public void SetMoveDir(Vector3 _vec)
	{
		moveDir = _vec;
	}
	public void Move()
	{
		if (!isFall)
		{
			Vector3 vector = rigid.velocity;
			float y = vector.y;
			float baseMoveSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.GetBaseMoveSpeed();
			float correctionUpDiffMoveSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.GetCorrectionUpDiffMoveSpeed();
			Vector3 a = Quaternion.Euler(new Vector3(0f, moveDir.x, 0f) * SingletonCustom<Canoe_PlayerManager>.Instance.GetRotSpeed() * ((!isGoal) ? 1f : 3f)) * base.transform.forward;
			vector += a * baseMoveSpeed * diffMoveSpeed * correctionUpDiffMoveSpeed * Time.deltaTime * acceleMoveDir;
			float maxAcceleMoveSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxAcceleMoveSpeed();
			if (addStaminaUseAcceleSpeed > maxAcceleMoveSpeed)
			{
				maxAcceleMoveSpeed = addStaminaUseAcceleSpeed;
			}
			float num = diffMoveSpeed * maxAcceleMoveSpeed + addSpeed + addSlipStreamSpeed;
			if (vector.magnitude > num)
			{
				vector = vector.normalized * num;
			}
			float num2 = diffMoveSpeed / ((acceleMoveDir == 1) ? maxAcceleMoveSpeed : SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxFallBackMoveSpeed());
			playerEffect.SetMoveEffectParam(num2 * ((acceleMoveDir == 1) ? 1f : 0.5f));
			animationEventor.SetRowingAnimationSpeed(num2 + (addSpeed / SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxAddSpeed(addSpeedUpType) + addSlipStreamSpeed / SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxSlipStreamAddSpeed()) * 0.5f);
			if (GetIsCpu())
			{
				vector *= cpuAI.GetMoveSpeedMag();
			}
			vector.y = y;
			rigid.velocity = vector;
		}
		if (rigid.velocity.magnitude > 0.2f)
		{
			Rot(rigid.velocity.normalized * acceleMoveDir);
			if (!isFall)
			{
				playerEffect.PlayMoveEffect();
			}
		}
		else
		{
			playerEffect.StopMoveEffect();
		}
	}
	public void PlayPaddleRowingEffect(int _paddleIdx)
	{
		playerEffect.PlayPaddleRowingEffect(_paddleIdx);
		if (!isGoal && !GetIsCpu())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_canoe_paddle_rowing");
		}
	}
	private void SetSlipStreamAddSpeed()
	{
		float num = Mathf.Atan2(base.transform.forward.z, base.transform.forward.x) * 57.29578f;
		int num2 = 5;
		float num3 = num - (float)(num2 / 2);
		for (int i = 0; i < num2; i++)
		{
			float f = (num3 + (float)i) * ((float)Math.PI / 180f);
			Vector3 vector = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
			UnityEngine.Debug.DrawRay(base.transform.position, vector * 5f, Color.red, 1f);
			if (Physics.Raycast(base.transform.position, vector, out RaycastHit hitInfo, 5f) && hitInfo.collider.tag == "Player")
			{
				isSlipStream = true;
				addSlipStreamSpeed += Time.deltaTime;
				if (addSlipStreamSpeed > SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxSlipStreamAddSpeed())
				{
					addSlipStreamSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxSlipStreamAddSpeed();
				}
				return;
			}
		}
		isSlipStream = false;
	}
	public void StopMove()
	{
		rigid.velocity = Vector3.zero;
		acceleInputLerp = 0f;
		acceleInputContinueTime = 0f;
		acceleMoveSpeed = 0f;
		fallBackInputLerp = 0f;
		fallBackInputContinueTime = 0f;
		fallBackMoveSpeed = 0f;
		diffMoveSpeed = 0f;
		acceleMoveDir = 1;
	}
	public void Rot(Vector3 _vec, bool _immediate = false)
	{
		Quaternion quaternion = Quaternion.LookRotation(_vec);
		if (_immediate)
		{
			base.transform.rotation = quaternion;
		}
		else
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, quaternion, Time.deltaTime * SingletonCustom<Canoe_PlayerManager>.Instance.GetRotSpeed());
		}
		float num = base.transform.localEulerAngles.x;
		if (num < 180f && num > SingletonCustom<Canoe_PlayerManager>.Instance.GetLimitRotX())
		{
			num = SingletonCustom<Canoe_PlayerManager>.Instance.GetLimitRotX();
		}
		else if (num > 180f && num < 360f - SingletonCustom<Canoe_PlayerManager>.Instance.GetLimitRotX())
		{
			num = 360f - SingletonCustom<Canoe_PlayerManager>.Instance.GetLimitRotX();
		}
		base.transform.SetLocalEulerAnglesX(num);
	}
	private Vector3 GetStickDir()
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(npadId);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		if (true && Mathf.Abs(num) < 0.2f && Mathf.Abs(num2) < 0.2f)
		{
			num = 0f;
			num2 = 0f;
			if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Right))
			{
				num = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Left))
			{
				num = -1f;
			}
			if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Up))
			{
				num2 = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Down))
			{
				num2 = -1f;
			}
		}
		mVector3Zero = new Vector3(num, 0f, num2);
		if (mVector3Zero.sqrMagnitude < 0.0400000028f)
		{
			return Vector3.zero;
		}
		return mVector3Zero.normalized;
	}
	public int GetPlayerNo()
	{
		return playerNo;
	}
	public Canoe_Define.UserType GetUserType()
	{
		return userType;
	}
	public bool GetIsCpu()
	{
		return isCpu;
	}
	public Rigidbody GetRigidbody()
	{
		return rigid;
	}
	public CapsuleCollider GetCollider()
	{
		return collider;
	}
	public Transform GetObstacleCheckAnchor()
	{
		return obstacleCheckAnchor;
	}
	public bool GetIsOnceInput()
	{
		return isOnceInput;
	}
	public float GetAddSpeed()
	{
		return addSpeed;
	}
	public float GetAddSlipStreamSpeed()
	{
		return addSlipStreamSpeed;
	}
	public float GetAddStaminaUseAcceleSpeed()
	{
		return addStaminaUseAcceleSpeed;
	}
	public void SetIsOnceInput()
	{
		isOnceInput = true;
	}
	public void SetRowingAnimationSpeed(float _rowingAnimationSpeed)
	{
		animationEventor.SetRowingAnimationSpeed(_rowingAnimationSpeed);
	}
	public bool GetIsGoal()
	{
		return isGoal;
	}
	public void SetGoal()
	{
		SetGoal(SingletonCustom<Canoe_GameManager>.Instance.GetGameTime());
		int rank = SingletonCustom<Canoe_GameManager>.Instance.GetRank(playerNo);
		SingletonCustom<Canoe_GameManager>.Instance.SetIsGoal(playerNo, (int)userType, rank);
		character.SetGoalFace(rank);
		if (SingletonCustom<Canoe_GameManager>.Instance.GetIsViewCamera(playerNo))
		{
			SingletonCustom<Canoe_CameraManager>.Instance.SetGoalAnchor(playerNo);
		}
		acceleMoveDir = 1;
		playerEffect.SetMoveEffectDir(acceleMoveDir);
		playerEffect.SetPaddleEffectDir(acceleMoveDir);
		addSpeed = 0f;
		addSlipStreamSpeed = 0f;
		SingletonCustom<Canoe_CameraManager>.Instance.StopAddSpeedEffect(playerNo);
		moveGoalAnchorPos = SingletonCustom<Canoe_CourseManager>.Instance.GetNearGoalAnchorPos(base.transform.position);
	}
	private void FinishMoveGoalAnchor()
	{
		isMoveGoalAnchor = true;
		animationEventor.PlayGoalAnimation();
		animationEventor.SetRowingAnimationSpeed(0f);
		playerEffect.StopMoveEffect();
	}
	public float GetGoalTime()
	{
		return goalTime;
	}
	public void SetGoal(float _time)
	{
		isGoal = true;
		goalTime = _time;
		isUseStaminaDash = false;
		wptPos.gameObject.SetActive(value: false);
		if (GetIsCpu())
		{
			wptAI.gameObject.SetActive(value: false);
		}
	}
	public void SetDebugGoal(float _time)
	{
		SetGoal(_time);
		isMoveGoalAnchor = true;
	}
	public void SetAutoGoalTime()
	{
		UnityEngine.Debug.Log("<color=red>CPUの自動タイムを設定</color>");
		float time = SingletonCustom<Canoe_CourseManager>.Instance.GetDistanceToGoal() / wptPos.progressDistance * SingletonCustom<Canoe_GameManager>.Instance.GetGameTime();
		time = CalcManager.ConvertDecimalSecond(time);
		UnityEngine.Debug.Log("（下２桁切り捨て）_playerNo : " + playerNo.ToString() + " ゴ\u30fcルタイム " + time.ToString());
		SetGoal(time);
		isMoveGoalAnchor = true;
	}
	public bool GetIsMoveGoalAnchor()
	{
		return isMoveGoalAnchor;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Finish")
		{
			FinishMoveGoalAnchor();
		}
		else if (other.tag == "Goal")
		{
			SetGoal();
		}
		else
		{
			if (!(other.tag == "CheckPoint"))
			{
				return;
			}
			addSpeedUp = other.GetComponent<Canoe_AddSpeedUp>();
			if (addSpeedUp != null)
			{
				addSpeedUpType = addSpeedUp.GetAddSpeedUpType();
				addSpeed = SingletonCustom<Canoe_PlayerManager>.Instance.GetMaxAddSpeed(addSpeedUpType);
				playerEffect.PlayAccelerationEffect();
				if (!GetIsCpu())
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_shorttrack_maxspeed");
					SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Strong, 0.5f);
				}
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "CheckPoint")
		{
			addSpeedUp = null;
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (SingletonCustom<Canoe_GameManager>.Instance == null || !SingletonCustom<Canoe_GameManager>.Instance.GetIsGameStart())
		{
			return;
		}
		if (collision.gameObject.tag == "CharaWall" && GetIsCpu())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_collision_character_10");
		}
		if (!(collision.gameObject.tag == "Field"))
		{
			return;
		}
		if (!isCanFall && !isFallCollisionWater)
		{
			UnityEngine.Debug.Log("OnCollisionEnter");
			isFall = false;
			UnityEngine.Debug.Log("fallVelocityY : " + fallVelocityY.ToString());
			if (fallVelocityY < SingletonCustom<Canoe_PlayerManager>.Instance.GetFallVelocityY())
			{
				isFallCollisionWater = true;
				LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
				{
					isFallCollisionWater = false;
				});
			}
			rigid.velocity = beforeFallVelocity;
			playerEffect.SetMoveEffectActive(_isActive: true);
			playerEffect.SetPaddleEffectActive(_isActive: true);
			playerEffect.SetLandingEffectPos(rigid.velocity, collision.contacts[0]);
			playerEffect.PlayLandingEffect();
			if (!GetIsCpu())
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_morphingrace_out_water");
			}
			float time = 0.5f;
			LeanTween.moveLocalY(character.gameObject, characterOriginPos.y - 0.1f, time);
			LeanTween.moveLocalY(canoe.gameObject, canoeOriginPos.y - 0.1f, time);
			LeanTween.delayedCall(base.gameObject, time, (Action)delegate
			{
				LeanTween.moveLocalY(character.gameObject, characterOriginPos.y, time);
				LeanTween.moveLocalY(canoe.gameObject, canoeOriginPos.y, time);
			});
		}
	}
	private void OnCollisionStay(Collision collision)
	{
		if (!(SingletonCustom<Canoe_GameManager>.Instance == null) && SingletonCustom<Canoe_GameManager>.Instance.GetIsGameStart() && collision.gameObject.tag == "Field" && !isCanFall && !isFallCollisionWater)
		{
			isFall = false;
			playerEffect.SetMoveEffectActive(_isActive: true);
			playerEffect.SetPaddleEffectActive(_isActive: true);
		}
	}
	private void OnCollisionExit(Collision collision)
	{
		if (!(SingletonCustom<Canoe_GameManager>.Instance == null) && SingletonCustom<Canoe_GameManager>.Instance.GetIsGameStart() && collision.gameObject.tag == "Field" && !isFallCollisionWater)
		{
			UnityEngine.Debug.Log("OnCollisionExit");
			isFall = true;
			fallVelocityY = 0f;
			isCanFall = true;
			LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
			{
				isCanFall = false;
			});
			playerEffect.SetMoveEffectParam(0f);
			playerEffect.SetMoveEffectActive(_isActive: false);
			playerEffect.SetPaddleEffectActive(_isActive: false);
			beforeFallVelocity = rigid.velocity;
		}
	}
}
