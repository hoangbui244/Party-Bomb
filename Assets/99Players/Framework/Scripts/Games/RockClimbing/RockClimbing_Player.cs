using GamepadInput;
using System;
using UnityEngine;
public class RockClimbing_Player : MonoBehaviour
{
	private RockClimbing_PlayerManager.State state;
	private RockClimbing_Character character;
	[SerializeField]
	[Header("鉤縄クラス")]
	private RockClimbing_GrapplingHook grapplingHook;
	[SerializeField]
	[Header("縄の輪っかクラス")]
	private RockClimbing_RopeRing ropeRing;
	private RockClimbing_ClimbingWall climbingWall;
	private RockClimbing_ClimbOnFoundation climbOnFoundation;
	private Rigidbody rigid;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private CapsuleCollider collider;
	private float collider_radius;
	private int playerNo;
	private RockClimbing_Define.UserType userType;
	private int npadId;
	private bool isGoal;
	private float goalTime;
	private float inputInterval;
	private float beforeInputTime;
	private bool isWalkSe;
	private Vector3 nowPos;
	private Vector3 prevPos;
	private float climbingSpeed;
	private int climbingCount;
	private bool isOnceThrow;
	private bool isOnceClimbing;
	private int climbingMoveCount_R;
	private int climbingMoveCount_L;
	private Vector3 moveDir;
	private Vector3 climbingVec;
	private bool isHitObstacle;
	private int obstacleLineIdx;
	private Transform throwGrapplingHookRoot;
	private Transform throwAnchor;
	private Vector3 throwAnchorDiffVec;
	private int checkFoundationLayerMask;
	private string[] checkFoundationMaskNameList = new string[1]
	{
		"Default"
	};
	private Vector3 throwGrapplingHookPos;
	private RaycastHit[] checkFoundationRaycastHit = new RaycastHit[4];
	private RockClimbing_AI cpuAI;
	private bool DebugIsThrowVec;
	private Vector3 DebugThrowPos;
	public void Init(int _playerNo, int _userType)
	{
		state = RockClimbing_PlayerManager.State.MOVE;
		playerNo = _playerNo;
		userType = (RockClimbing_Define.UserType)_userType;
		character = GetComponent<RockClimbing_Character>();
		rigid = GetComponent<Rigidbody>();
		character.Init(this);
		character.SetArrayClimbOnPosElement();
		character.SetStyle(_userType);
		SetColliderLayer(LayerMask.NameToLayer("Character"));
		collider_radius = collider.radius * collider.transform.localScale.x * base.transform.localScale.x;
		grapplingHook.Init(this);
		ropeRing.Init(this);
		climbOnFoundation = climbingWall.GetStartClimbOnFoundation();
		SetMoveRigidStatus();
		throwGrapplingHookRoot = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetThrowGrapplingHookRoot(playerNo);
		throwAnchor = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetThrowAnchor(playerNo);
		nowPos = (prevPos = base.transform.position);
		throwAnchorDiffVec = base.transform.position - throwAnchor.position;
		checkFoundationLayerMask = LayerMask.GetMask(checkFoundationMaskNameList);
		if (GetIsCpu())
		{
			cpuAI = base.gameObject.AddComponent<RockClimbing_AI>();
			cpuAI.Init(this);
		}
	}
	public void SetCpuFirstClimbOnFoundation(RockClimbing_ClimbOnFoundation _firstClimbOnFoundation)
	{
		cpuAI.SetFirstClimbOnFoundation(_firstClimbOnFoundation);
	}
	public void UpdateMethod()
	{
		throwAnchor.position = base.transform.position - throwAnchorDiffVec;
		inputInterval = Time.time - beforeInputTime;
		if (!GetIsCpu())
		{
			npadId = ((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? playerNo : 0);
		}
		switch (state)
		{
		case RockClimbing_PlayerManager.State.CLIMB_ON:
			break;
		case RockClimbing_PlayerManager.State.MOVE:
			if (isHitObstacle)
			{
				break;
			}
			if (!GetIsCpu())
			{
				SetMove();
				Move();
				if (CheckThrowGrapplingHook())
				{
					SingletonCustom<RockClimbing_UIManager>.Instance.SetThrowControllerBalloonActive(playerNo, _isActive: true);
					SingletonCustom<RockClimbing_UIManager>.Instance.SetThrowUIPos(playerNo, SingletonCustom<RockClimbing_CameraManager>.Instance.GetCamera(playerNo).GetCamera().WorldToScreenPoint(throwGrapplingHookPos));
					SetThrowGrapplingHook();
				}
				else
				{
					SingletonCustom<RockClimbing_UIManager>.Instance.SetThrowControllerBalloonActive(playerNo, _isActive: false);
				}
			}
			else if (!cpuAI.GetIsMoveEnd())
			{
				if (cpuAI.GetIsAvoid())
				{
					cpuAI.Avoid();
				}
				else
				{
					cpuAI.SetMove();
					if (!cpuAI.GetIsNotAvoid())
					{
						UnityEngine.Debug.DrawRay(base.transform.position, moveDir, Color.white);
						cpuAI.ChangeMoveTargetPos(moveDir);
						UnityEngine.Debug.DrawRay(base.transform.position, moveDir, Color.yellow);
					}
					else
					{
						cpuAI.NotAvoid();
					}
				}
				Move();
				if (CheckThrowGrapplingHook())
				{
					cpuAI.StopMove();
					SetThrowGrapplingHookRigidStatus();
				}
			}
			else
			{
				cpuAI.SetThrowGrapplingHook();
			}
			break;
		case RockClimbing_PlayerManager.State.THROW:
			if (grapplingHook.GetState() == RockClimbing_GrapplingHook.State.THROW)
			{
				grapplingHook.UpdateMethod();
			}
			break;
		case RockClimbing_PlayerManager.State.CLIMBING:
			if (!isHitObstacle)
			{
				if (!GetIsCpu())
				{
					SetClimbing();
				}
				else
				{
					cpuAI.SetClimbing();
				}
			}
			break;
		}
	}
	public void GameStartAnimation()
	{
	}
	private void SetMove()
	{
		SetMoveDir(GetStickDir());
	}
	public void Move()
	{
		prevPos = nowPos;
		nowPos = base.transform.position;
		Vector3 vector = rigid.velocity;
		if (moveDir == Vector3.zero)
		{
			vector.x = 0f;
			if (vector.y > 0f)
			{
				vector.y = 0f;
			}
			vector.z = 0f;
			rigid.velocity = vector;
		}
		else
		{
			float y = vector.y;
			float moveSpeed = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetMoveSpeed();
			float baseMoveSpeed = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetBaseMoveSpeed();
			float maxMoveSpeed = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetMaxMoveSpeed();
			float correctionMoveSpeed = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetCorrectionMoveSpeed();
			Vector3 a = moveDir * baseMoveSpeed * moveSpeed;
			vector += a * Time.deltaTime * correctionMoveSpeed;
			if (vector.magnitude > maxMoveSpeed * moveSpeed)
			{
				vector = vector.normalized * maxMoveSpeed * moveSpeed;
			}
			vector.y = y;
			rigid.velocity = vector;
			UnityEngine.Debug.Log("rigid + " + rigid.velocity.ToString());
			Rot();
		}
		character.MoveAnimation();
		if (!GetIsCpu() && !isWalkSe)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.2f);
			isWalkSe = true;
			LeanTween.delayedCall(base.gameObject, 0.35f, (Action)delegate
			{
				isWalkSe = false;
			});
		}
	}
	public void Rot(bool _immediate = false)
	{
		Vector3 zero = Vector3.zero;
		zero.y = CalcManager.Rot(moveDir, CalcManager.AXIS.Y);
		if (_immediate)
		{
			base.transform.rotation = Quaternion.Euler(zero);
		}
		else
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(zero), 20f * Time.deltaTime);
		}
	}
	public bool CheckThrowGrapplingHook()
	{
		RockClimbing_ClimbOnFoundation[] arrayClimbOnFoundation = climbingWall.GetArrayClimbOnFoundation();
		for (int i = 0; i < arrayClimbOnFoundation.Length; i++)
		{
			RockClimbing_GrapplingHookPoint_Group grapplingHookPointGroup = arrayClimbOnFoundation[i].GetGrapplingHookPointGroup(playerNo);
			for (int j = 0; j < grapplingHookPointGroup.GetArrayGrapplingHookPoint().Length && (arrayClimbOnFoundation[i].GetClimbOnFoundationType() == RockClimbing_ClimbingWallManager.ClimbOnFoundationType.Roof_4 || j < 1); j++)
			{
				RockClimbing_GrapplingHookPoint grapplingHookPoint = arrayClimbOnFoundation[i].GetGrapplingHookPoint(playerNo, j);
				if ((GetIsCpu() && grapplingHookPoint != cpuAI.GetTargetHookPoint()) || !(throwAnchor.transform.position.y < grapplingHookPoint.transform.position.y))
				{
					continue;
				}
				Vector3 position = grapplingHookPoint.transform.position;
				position.x = throwAnchor.transform.position.x;
				if (!(Mathf.Abs(throwAnchor.transform.position.y - position.y) < SingletonCustom<RockClimbing_PlayerManager>.Instance.GetThrowGrapplingHookDistance()))
				{
					continue;
				}
				Vector3 normalized = (position - throwAnchor.transform.position).normalized;
				float num = Vector3.Angle(Vector3.up, normalized);
				if (!SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsNoneFoundationType(climbOnFoundation.GetClimbOnFoundationType()) && !SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsStoneWallFoundationType(climbOnFoundation.GetClimbOnFoundationType()) && climbOnFoundation.GetClimbOnFoundationType() != RockClimbing_ClimbingWallManager.ClimbOnFoundationType.Roof_1 && (!SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsRoofFoundationType(climbOnFoundation.GetClimbOnFoundationType(), _isCheckThrowGrapplingHook: true) || !(num <= SingletonCustom<RockClimbing_PlayerManager>.Instance.GetThrowAngleOnRoof())))
				{
					continue;
				}
				Collider collider = grapplingHookPoint.GetCollider();
				float num2 = collider.bounds.min.x;
				float num3 = collider.bounds.max.x;
				if (GetIsCpu())
				{
					num2 += SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
					num3 -= SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHookPointColliderRange();
					UnityEngine.Debug.DrawLine(new Vector3(num2, collider.transform.position.y, collider.transform.position.z), new Vector3(num2, collider.transform.position.y - 6f, collider.transform.position.z), Color.black, 10f);
					UnityEngine.Debug.DrawLine(new Vector3(num3, collider.transform.position.y, collider.transform.position.z), new Vector3(num3, collider.transform.position.y - 6f, collider.transform.position.z), Color.black, 10f);
				}
				if (!(throwAnchor.transform.position.x >= num2) || !(throwAnchor.transform.position.x <= num3))
				{
					continue;
				}
				int num4 = Physics.SphereCastNonAlloc(throwAnchor.transform.position, grapplingHook.GetHookColliderRadius(), normalized, checkFoundationRaycastHit, SingletonCustom<RockClimbing_PlayerManager>.Instance.GetThrowGrapplingHookDistance(), checkFoundationLayerMask);
				if (num4 <= 0)
				{
					continue;
				}
				for (int k = 0; k < num4; k++)
				{
					RockClimbing_GrapplingHookPoint component = checkFoundationRaycastHit[k].collider.GetComponent<RockClimbing_GrapplingHookPoint>();
					if (component != null && component.CheckClimbPlayerType(playerNo) && component.gameObject.layer == LayerMask.NameToLayer("Default"))
					{
						throwGrapplingHookPos = throwAnchor.transform.position + normalized * (checkFoundationRaycastHit[0].distance + grapplingHook.GetHookColliderRadius());
						DebugIsThrowVec = true;
						DebugThrowPos = throwGrapplingHookPos;
						return true;
					}
				}
			}
		}
		DebugIsThrowVec = false;
		return false;
	}
	private void SetThrowGrapplingHook()
	{
		if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A))
		{
			if (!GetIsCpu())
			{
				SingletonCustom<RockClimbing_UIManager>.Instance.SetThrowControllerBalloonActive(playerNo, _isActive: false);
			}
			SetThrowGrapplingHookRigidStatus();
			ThrowGrapplingHook();
		}
	}
	public void SetMoveRigidStatus()
	{
		rigid.useGravity = true;
		rigid.isKinematic = false;
		UnityEngine.Debug.Log("climbOnFoundation.GetClimbOnFoundationType() " + climbOnFoundation.GetClimbOnFoundationType().ToString());
		if (SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsNoneFoundationType(climbOnFoundation.GetClimbOnFoundationType()) || SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetIsStoneWallFoundationType(climbOnFoundation.GetClimbOnFoundationType()))
		{
			rigid.constraints = (RigidbodyConstraints)116;
		}
		else
		{
			rigid.constraints = RigidbodyConstraints.FreezeRotation;
		}
	}
	private void SetThrowGrapplingHookRigidStatus()
	{
		rigid.useGravity = false;
		rigid.isKinematic = true;
		rigid.velocity = Vector3.zero;
		rigid.constraints = RigidbodyConstraints.FreezeAll;
	}
	public void ThrowGrapplingHook()
	{
		state = RockClimbing_PlayerManager.State.THROW;
		SetColliderLayer(LayerMask.NameToLayer("HitCharacterDefaultOnly"));
		base.transform.SetLocalEulerAnglesY(0f);
		character.ThrowAnimation(delegate
		{
			if (!GetIsCpu())
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_curling_throw");
			}
			grapplingHook.Throw(throwGrapplingHookPos);
		});
	}
	public void GrapplingHook()
	{
		ropeRing.PutOnGround();
		if (!GetIsCpu())
		{
			SingletonCustom<RockClimbing_UIManager>.Instance.ChangeControllerUIType(playerNo, RockClimbing_UIManager.ControllerUIType.Climbing);
			if (!isOnceClimbing)
			{
				SingletonCustom<RockClimbing_UIManager>.Instance.SetControllerBalloonActive(playerNo, RockClimbing_UIManager.ControllerUIType.Climbing, _isFade: true, _isActive: true);
			}
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType);
		}
		Vector3 vector = base.transform.position;
		vector.x = grapplingHook.transform.position.x;
		vector += SingletonCustom<RockClimbing_PlayerManager>.Instance.GetClimbingRopeDiffPos();
		LeanTween.move(base.gameObject, vector, SingletonCustom<RockClimbing_CharacterManager>.Instance.GetReadyClimbingAnimationTime());
		character.ReadyClimbingAnimation(SingletonCustom<RockClimbing_CharacterManager>.Instance.GetReadyClimbingAnimationTime());
		LeanTween.delayedCall(base.gameObject, SingletonCustom<RockClimbing_CharacterManager>.Instance.GetReadyClimbingAnimationTime(), (Action)delegate
		{
			state = RockClimbing_PlayerManager.State.CLIMBING;
		});
	}
	private void SetClimbing()
	{
		if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A))
		{
			Climbing();
		}
	}
	public void Climbing()
	{
		LeanTween.cancel(base.gameObject);
		climbingVec = (throwGrapplingHookPos + SingletonCustom<RockClimbing_PlayerManager>.Instance.GetClimbingRopeDiffPos() - base.transform.position).normalized;
		beforeInputTime = Time.time;
		float intervalLerp = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetIntervalLerp(inputInterval);
		climbingSpeed = SingletonCustom<RockClimbing_PlayerManager>.Instance.ClampClimbiingSpeed(intervalLerp);
		rigid.isKinematic = false;
		rigid.constraints &= (RigidbodyConstraints)(-13);
		rigid.AddForce(climbingVec * climbingSpeed * SingletonCustom<RockClimbing_PlayerManager>.Instance.GetClimbingBasePower(), ForceMode.Acceleration);
		PlayClimbingAnimation(intervalLerp);
		LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
		{
			rigid.isKinematic = true;
			rigid.constraints |= (RigidbodyConstraints)12;
		});
		if (!GetIsCpu())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.35f);
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType);
		}
	}
	private void PlayClimbingAnimation(float _intervalLerp)
	{
		climbingCount++;
		character.ClimbingAnimation(climbingCount % 2 == 1, _intervalLerp, delegate
		{
			if (!GetIsCpu() && !isOnceClimbing)
			{
				isOnceClimbing = true;
				SingletonCustom<RockClimbing_UIManager>.Instance.SetControllerBalloonActive(playerNo, RockClimbing_UIManager.ControllerUIType.Climbing, _isFade: true, _isActive: false);
			}
		});
	}
	public void HitObstacle()
	{
		if (!isHitObstacle)
		{
			isHitObstacle = true;
			LeanTween.delayedCall(base.gameObject, SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHitObstacleInterval(), (Action)delegate
			{
				isHitObstacle = false;
			});
			character.SetBlink();
			if (state == RockClimbing_PlayerManager.State.CLIMBING)
			{
				rigid.isKinematic = false;
				rigid.AddForce(-climbingVec * SingletonCustom<RockClimbing_PlayerManager>.Instance.GetHitObstacleDownPower(), ForceMode.Acceleration);
				LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
				{
					rigid.isKinematic = true;
				});
			}
			if (!GetIsCpu())
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_icehockey_puck");
				SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Normal, 0.5f);
			}
			else
			{
				cpuAI.ResetClimbingLerpTime();
			}
		}
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
	public void SetMoveState()
	{
		character.ResetAnimation(0.25f);
		LeanTween.delayedCall(base.gameObject, 0.25f, (Action)delegate
		{
			state = RockClimbing_PlayerManager.State.MOVE;
			if (!GetIsCpu())
			{
				SingletonCustom<RockClimbing_UIManager>.Instance.ChangeControllerUIType(playerNo, RockClimbing_UIManager.ControllerUIType.Move);
			}
			else
			{
				cpuAI.ResetMoveSetting();
			}
		});
	}
	public RockClimbing_PlayerManager.State GetState()
	{
		return state;
	}
	public int GetPlayerNo()
	{
		return playerNo;
	}
	public RockClimbing_Define.UserType GetUserType()
	{
		return userType;
	}
	public bool GetIsCpu()
	{
		return userType >= RockClimbing_Define.UserType.CPU_1;
	}
	public Rigidbody GetRigidbody()
	{
		return rigid;
	}
	public CapsuleCollider GetCollider()
	{
		return collider;
	}
	public void SetColliderLayer(int _layer)
	{
		collider.gameObject.layer = _layer;
	}
	public float GetColliderRadius()
	{
		return collider_radius;
	}
	public Transform GetThrowAnchor()
	{
		return throwAnchor;
	}
	public RockClimbing_GrapplingHook GetGrapplingHook()
	{
		return grapplingHook;
	}
	public Transform GetThrowGrapplingHookAnchor()
	{
		return throwGrapplingHookRoot;
	}
	public void SetClimbingWall(RockClimbing_ClimbingWall _climbingWall)
	{
		climbingWall = _climbingWall;
	}
	public RockClimbing_ClimbingWall GetClimbingWall()
	{
		return climbingWall;
	}
	public void SetClimbOnFoundation(RockClimbing_ClimbOnFoundation _climbOnFoundation)
	{
		climbOnFoundation = _climbOnFoundation;
	}
	public RockClimbing_ClimbOnFoundation GetClimbOnFoundation()
	{
		return climbOnFoundation;
	}
	public Vector3 GetMoveDir()
	{
		return moveDir;
	}
	public void SetMoveDir(Vector3 _moveDir)
	{
		moveDir = _moveDir;
	}
	public void SetClimbingVec(Vector3 _climbingVec)
	{
		climbingVec = _climbingVec;
	}
	public Vector3 GetClimbingVec()
	{
		return climbingVec;
	}
	public bool GetIsGoal()
	{
		return isGoal;
	}
	public float GetGoalTime()
	{
		return goalTime;
	}
	public void SetGoal(float _time)
	{
		isGoal = true;
		goalTime = _time;
		UnityEngine.Debug.Log("（下２桁切り捨て）playerNo : " + playerNo.ToString() + " ゴ\u30fcルタイム " + goalTime.ToString());
	}
	public Vector3 GetNowPos()
	{
		return nowPos;
	}
	public Vector3 GetPrevPos()
	{
		return prevPos;
	}
	public Transform GetHeadTop()
	{
		return character.GetHeadTop();
	}
	public Transform GetHaveRopeRing()
	{
		return character.GetHaveRopeRing();
	}
	public Transform GetHaveGrapplingHook()
	{
		return character.GetHaveGrapplingHook();
	}
	public int GetObstacleLineIdx()
	{
		return obstacleLineIdx;
	}
	public void SetObstacleLineIdx(int _obstacleLineIdx)
	{
		obstacleLineIdx = _obstacleLineIdx;
	}
	public bool GetIsCpuMoveEnd()
	{
		return cpuAI.GetIsMoveEnd();
	}
	public bool GetIsCpuNotAvoid()
	{
		return cpuAI.GetIsNotAvoid();
	}
	private void OnTriggerEnter(Collider other)
	{
		RockClimbing_ClimbOnCollider component = other.GetComponent<RockClimbing_ClimbOnCollider>();
		if (component != null && component.CheckClimbPlayerType(playerNo))
		{
			state = RockClimbing_PlayerManager.State.CLIMB_ON;
			character.SetClimbOnPos();
			component.SetColliderActive(_isActive: false);
			rigid.isKinematic = true;
			collider.enabled = false;
			if (component.GetClimbOnFoundation().GetIsGoal())
			{
				SetGoal(CalcManager.ConvertDecimalSecond(SingletonCustom<RockClimbing_GameManager>.Instance.GetGameTime()));
				int goalCnt = SingletonCustom<RockClimbing_GameManager>.Instance.GetGoalCnt();
				SingletonCustom<RockClimbing_GameManager>.Instance.SetIsGoal(playerNo, (int)userType);
				character.ClimbOnAnimation(_isGoal: false, delegate
				{
					Vector3 position = climbingWall.GetCastle().GetGoalMoveAnchor(goalCnt).position;
					float value = CalcManager.Length(base.transform.position, position);
					UnityEngine.Debug.Log("distance : " + value.ToString());
					float goalPosMinDistance = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetGoalPosMinDistance();
					float goalPosMaxDistance = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetGoalPosMaxDistance();
					float num = goalPosMaxDistance - goalPosMinDistance;
					value = Mathf.Clamp(value, goalPosMinDistance, goalPosMaxDistance);
					float num2 = (goalPosMaxDistance - value) / num;
					float goalPosMinMoveTime = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetGoalPosMinMoveTime();
					float goalPosMaxMoveTime = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetGoalPosMaxMoveTime();
					float num3 = goalPosMaxMoveTime - goalPosMinMoveTime;
					float time = goalPosMaxMoveTime - num3 * num2;
					moveDir = (position - base.transform.position).normalized;
					LeanTween.move(base.gameObject, position, time).setOnUpdate((Action<float>)delegate
					{
						prevPos = nowPos;
						nowPos = base.transform.position;
						Rot();
						character.MoveAnimation();
					}).setOnComplete((Action)delegate
					{
						LeanTween.rotateLocal(base.gameObject, new Vector3(0f, 180f, 0f), 0.5f);
						int rank = SingletonCustom<RockClimbing_GameManager>.Instance.GetRank(playerNo);
						if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(playerNo))
						{
							SingletonCustom<RockClimbing_CameraManager>.Instance.GetCamera(playerNo).SetIsStop();
							SingletonCustom<RockClimbing_CameraManager>.Instance.GoalAnimation(playerNo);
						}
						character.GoalRankAnimation(SingletonCustom<RockClimbing_CharacterManager>.Instance.GetGoalAfterAnimationTime(), rank);
					});
				});
			}
			else
			{
				character.ClimbOnAnimation(_isGoal: false, delegate
				{
					character.SetCollectRopeAnimation(delegate
					{
						grapplingHook.SetCollectRope();
						ropeRing.SetCollectRope();
					}, delegate
					{
						SetMoveState();
						rigid.useGravity = true;
						rigid.isKinematic = false;
						SetMoveRigidStatus();
						collider.enabled = true;
						SetColliderLayer(LayerMask.NameToLayer("Character"));
					});
				});
			}
		}
	}
	private void OnDrawGizmos()
	{
		if (DebugIsThrowVec)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(throwAnchor.position, DebugThrowPos);
			Gizmos.DrawWireSphere(DebugThrowPos, grapplingHook.GetHookColliderRadius());
		}
	}
}
