using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BeachVolley_Character : MonoBehaviour
{
	public struct ActionTimeData
	{
		public float tossTime;
		public float attackTime;
		public float spikeAfterTime;
		public float freezeTimeLanding;
		public float freezeTimeToss;
		public float freezeTimeAttack;
		public float freezeTimeBlock;
		public float freezeTimeMiss;
		public float freezeTimeKnockBack;
		public ActionTimeData(float _tossTime, float _attackTime, float _spikeAfterTime, float _freezeTimeLanding, float _freezeTimeToss, float _freezeTimeAttack, float _freezeTimeBlock, float _freezeTimeMiss, float _freezeTimeKnockBack)
		{
			tossTime = _tossTime;
			attackTime = _attackTime;
			spikeAfterTime = _spikeAfterTime;
			freezeTimeLanding = _freezeTimeLanding;
			freezeTimeToss = _freezeTimeToss;
			freezeTimeAttack = _freezeTimeAttack;
			freezeTimeBlock = _freezeTimeBlock;
			freezeTimeMiss = _freezeTimeMiss;
			freezeTimeKnockBack = _freezeTimeKnockBack;
		}
	}
	public enum ActionState
	{
		SERVE_STANDBY,
		SERVE_WAIT,
		STAND_SERVE,
		JUMP_SERVE,
		STANDARD,
		TOSS,
		DIVING_TOSS,
		SPIKE,
		SPIKE_AFTER,
		DIVING_ATTACK,
		ATTACK,
		JUMP_BALL_STANDBY,
		JUMP_BALL_JUMPER,
		JUMP_BALL_JUMP,
		JUMP_BALL_END,
		CATCH_SUCCESS,
		MOVE_POS,
		MOVE_FIELD,
		JUMP,
		FREEZE,
		GOAL_PRODUCTION,
		BENCH,
		NEXT_STAY
	}
	public enum PositionState
	{
		FRONT_ZONE,
		BACK_ZONE,
		BENCH,
		MAX
	}
	public enum BodyPartsList
	{
		HEAD,
		BODY,
		HIP,
		SHOULDER_L,
		SHOULDER_R,
		ARM_L,
		ARM_R,
		LEG_L,
		LEG_R
	}
	[Serializable]
	public struct BodyParts
	{
		[SerializeField]
		[Header("体アンカ\u30fc")]
		public GameObject bodyAnchor;
		public MeshRenderer[] rendererList;
		public MeshFilter[] filterList;
		public Transform Parts(BodyPartsList _parts)
		{
			return rendererList[(int)_parts].transform;
		}
		public Transform Parts(int _parts)
		{
			return rendererList[_parts].transform;
		}
	}
	public delegate void AiActionMethod();
	[Serializable]
	public struct StatusData
	{
		public float moveSpeed;
		public float moveSpeedMax;
		public float moveDelay;
		public float jumpServeShufflePer;
		public float blockMissPer;
		public float blockNicePer;
		public float blockMissAddPer;
		public float blockNiceReducePer;
		public float jumpHeight;
	}
	public struct ParamColcData
	{
		public float[] moveSpeed;
		public float moveSpeedMaxDef;
		public float[] moveDelay;
		public float[] jumpServeShufflePer;
		public float[] blockMissPerBase;
		public float[] blockNicePerBase;
		public float[] blockMissAddPerBase;
		public float[] blockNiceReducePerBase;
		public float[] jumpHeightCorr;
		public ParamColcData(float[] _moveSpeed, float _moveSpeedMaxDef, float[] _moveDelay, float[] _jumpServeShufflePer, float[] _blockMissPerBase, float[] _blockNicePerBase, float[] _blockMissAddPerBase, float[] _blockNiceReducePerBase, float[] _jumpHeightCorr)
		{
			moveSpeed = _moveSpeed;
			moveSpeedMaxDef = _moveSpeedMaxDef;
			moveDelay = _moveDelay;
			jumpServeShufflePer = _jumpServeShufflePer;
			blockMissPerBase = _blockMissPerBase;
			blockNicePerBase = _blockNicePerBase;
			blockMissAddPerBase = _blockMissAddPerBase;
			blockNiceReducePerBase = _blockNiceReducePerBase;
			jumpHeightCorr = _jumpHeightCorr;
		}
	}
	[Serializable]
	public struct DebguShowData
	{
		public bool positionStopCheck;
		public bool deffenceArea;
		public bool coverArea;
		public bool ballNearDistance;
		public bool personalSpace;
		public bool moveTargetPos;
		public bool spacePos;
		public bool other;
		public bool ai;
		public DebguShowData(bool _positionStopCheck, bool _deffenceArea, bool _coverArea, bool _ballNearDistance, bool _personalSpace, bool _moveTargetPos, bool _spacePos, bool _other, bool _ai)
		{
			positionStopCheck = _positionStopCheck;
			deffenceArea = _deffenceArea;
			coverArea = _coverArea;
			ballNearDistance = _ballNearDistance;
			personalSpace = _personalSpace;
			moveTargetPos = _moveTargetPos;
			spacePos = _spacePos;
			other = _other;
			ai = _ai;
		}
	}
	protected float JUMP_POWER = 570f;
	protected float JUMP_POWER_SPIKE = 600f;
	protected float JUMP_POWER_BACK_ATTACK = 550f;
	protected float MINI_GAME_JUMP_POWER_BACK_ATTACK = 200f;
	protected float JUMP_POWER_BLOCK = 650f;
	protected float DIVING_DISTANCE_CORR = 1f;
	protected float DIVING_POWER = 4f;
	protected float DIVING_POWER_MAX = 5.5f;
	protected float ADD_GRAVITY = 400f;
	protected float SYSTEM_SPEED_MIN = 1.5f;
	protected float SYSTEM_SPEED_MAX = 3f;
	public static readonly float WALK_SPEED = 0.5f;
	protected float RUN_SPEED = 1f;
	protected static readonly float SYSTEM_MOVE_SPEED = 1f;
	public static readonly float NEAR_DISTANCE = 1.5f;
	public static readonly float NEAR_DISTANCE_LIBERO = 4.5f;
	public static readonly float SHOOT_BORDER_POWER = 0.5f;
	public static readonly ActionTimeData actionTimeData = new ActionTimeData(0.7f, 0.35f, 0.25f, 0.5f, 0.25f, 0.9f, 0.5f, 1.2f, 2.5f);
	protected float[] STAMINA_RECOVRY_MAG = new float[3]
	{
		0.25f,
		0.5f,
		0.75f
	};
	protected const int REBOUND_UP_PER = 50;
	protected const float REBOUND_UP_SUPER_CORR = 1f;
	protected const int REBOUND_UP_STATUS_ADD_PER = 2;
	protected const float REBOUND_DOWN_POWER = -1f;
	protected const float REBOUND_DOWN_POWER_SUPER = -10f;
	protected const float REBOUND_UP_POWER_MIN = 3f;
	protected const float REBOUND_UP_POWER_MAX = 5f;
	protected const float REBOUND_UP_POWER_MIN_SUPER = 3f;
	protected const float REBOUND_UP_POWER_MAX_SUPER = 5f;
	protected const float REBOUND_REFLECT_POWER_MIN = 0.2f;
	protected const float REBOUND_REFLECT_POWER_MAX = 0.35f;
	protected const float REBOUND_REFLECT_POWER_MIN_SUPER = 0.1f;
	protected const float REBOUND_REFLECT_POWER_MAX_SUPER = 0.15f;
	protected Vector3[] calcVec = new Vector3[2];
	protected Vector3 rot;
	protected ActionState actionState;
	protected PositionState positionState;
	[SerializeField]
	[Header("オブジェクト")]
	protected Transform obj;
	[SerializeField]
	[Header("コライダ\u30fc")]
	protected Collider[] collider;
	protected Rigidbody rigid;
	protected Vector3 defPos;
	protected Vector3 prevPos;
	protected Vector3 nowPos;
	protected Vector3 gameStartStandbyPos;
	protected Vector3 returnBenchPos;
	protected Vector3 movePos;
	protected bool isMovePos;
	protected float moveSpeedMag;
	protected bool isMoveReturn;
	protected Vector3 moveReturnPos;
	protected float actionInterval;
	protected float controlInterval;
	protected float moveDelay;
	protected float triggerCheckInterval;
	protected bool isBallAction;
	protected bool isOverToss;
	protected bool isRival;
	protected bool isDelayInput;
	protected IEnumerator delayCantactBallMethod;
	[SerializeField]
	[Header("走るエフェクト")]
	protected ParticleSystem runEffect;
	protected ParticleSystem slidingEffect;
	[SerializeField]
	[Header("汗エフェクト")]
	protected ParticleSystem sweatEffect;
	protected int[] StealEffectNo = new int[16]
	{
		0,
		0,
		0,
		1,
		2,
		3,
		0,
		2,
		2,
		0,
		2,
		0,
		0,
		0,
		0,
		0
	};
	[SerializeField]
	[Header("吹き飛ばしエフェクト")]
	protected ParticleSystem breakEffect;
	protected GameObject superCharaEffect;
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	protected BodyParts bodyParts;
	[SerializeField]
	[Header("スタイル")]
	private CharacterStyle style;
	private BeachVolley_Define.UserType userType;
	protected float runAnimationSpeed = 20f;
	protected float runAnimationTime;
	protected float throwAnimationTime;
	protected int playSeRunCnt;
	protected float runSeInterval;
	protected bool isChangeAnimationNeutral;
	protected float actionChangeInterval;
	protected float movePosChangeInterval;
	protected float changeFormationInterval;
	protected List<AiActionMethod> aiActionMethod = new List<AiActionMethod>();
	protected float aiActionTime;
	protected float actionLagTime;
	protected bool isInitAiAction;
	protected bool isCallAiAction;
	protected RaycastHit raycastHit;
	protected BeachVolley_Character frontCharacter;
	protected float ignoreObstacleTime;
	protected float joyJumpInterval;
	private bool isMoveOverNet;
	protected int teamNo;
	protected int opponentTeamNo;
	protected int charaNo;
	protected bool isPlayer;
	public int playerNo = -1;
	protected new string name;
	protected int uniformNumber;
	protected int formationNo;
	protected Transform formationAnchor;
	protected Vector3 formationPos;
	protected BeachVolley_Define.PositionType positionType;
	protected CapsuleCollider charaCollider;
	protected float charaBodySize;
	protected float charaHeight;
	protected BeachVolley_Define.StatusType charaParam;
	private bool attackStayFlg;
	protected StatusData statusData;
	protected ParamColcData paramColcData = new ParamColcData(new float[2]
	{
		350f,
		100f
	}, 0.2f, new float[2]
	{
		0.2f,
		0.1f
	}, new float[2]
	{
		10f,
		10f
	}, new float[2]
	{
		30f,
		20f
	}, new float[2]
	{
		10f,
		20f
	}, new float[2]
	{
		5f,
		10f
	}, new float[2]
	{
		5f,
		10f
	}, new float[2]
	{
		2f,
		0.4f
	});
	protected float gaugeValue;
	protected float cursorRotSpeed = 0.5f;
	protected float cursorRotTime;
	protected Action setPlayMethod;
	protected int miniGameServerNo;
	protected Vector3 minigameServePos;
	private float changeCharaMoveTime;
	private bool isHightJumped;
	private float walkDiff;
	protected float STOP_CHECK_DISTANCE = 0.1f;
	protected float AVOID_ROT = 25f;
	protected float runTime;
	protected Vector3 lookBallPos;
	protected float ballSearchInterval;
	protected string AiState;
	protected List<Vector3> movePoslist = new List<Vector3>();
	protected float freezeTime;
	protected float spikeAfterTime;
	private int blockJumpPlayer = -1;
	protected bool isAttack;
	protected BeachVolley_Character targetChara;
	private float delay;
	private Vector3 characterPos = Vector3.zero;
	public DebguShowData debugShowData = new DebguShowData(_positionStopCheck: false, _deffenceArea: false, _coverArea: false, _ballNearDistance: false, _personalSpace: false, _moveTargetPos: false, _spacePos: false, _other: false, _ai: false);
	[Header("AI行動")]
	public string debugActionState;
	public bool IsMovePos => isMovePos;
	public bool IsMoveReturn => isMoveReturn;
	public bool IsRival
	{
		get
		{
			return isRival;
		}
		set
		{
			isRival = value;
		}
	}
	public bool IsDelayInput => isDelayInput;
	public CharacterStyle Style => style;
	public BeachVolley_Define.UserType UserType => userType;
	public float IgnoreObstacleTime
	{
		get
		{
			return ignoreObstacleTime;
		}
		set
		{
			ignoreObstacleTime = value;
		}
	}
	public int TeamNo
	{
		get
		{
			return teamNo;
		}
		set
		{
			teamNo = value;
		}
	}
	public int CharaNo
	{
		get
		{
			return charaNo;
		}
		set
		{
			charaNo = value;
		}
	}
	public bool IsPlayer
	{
		get
		{
			if (BeachVolley_Define.MCM.teamUserList[teamNo][0] <= 3)
			{
				return true;
			}
			return isPlayer;
		}
		set
		{
			isPlayer = value;
		}
	}
	public int FormationNo
	{
		get
		{
			return formationNo;
		}
		set
		{
			formationNo = value;
		}
	}
	public int MiniGameServerNo
	{
		get
		{
			return miniGameServerNo;
		}
		set
		{
			miniGameServerNo = value;
		}
	}
	protected BeachVolley_MainCharacterManager MCM => SingletonCustom<BeachVolley_MainCharacterManager>.Instance;
	protected BeachVolley_CpuAI CpuAi => MCM.CpuAi;
	protected BeachVolley_FieldManager FM => SingletonCustom<BeachVolley_FieldManager>.Instance;
	protected BeachVolley_BallManager BM => SingletonCustom<BeachVolley_BallManager>.Instance;
	protected BeachVolley_Ball Ball => SingletonCustom<BeachVolley_BallManager>.Instance.GetBall();
	public Transform GetBodyObj()
	{
		return obj;
	}
	public void SetAttackStayFlg(bool _value)
	{
		attackStayFlg = _value;
	}
	public void Init(int _teamNo, int _charaNo, int _formationNo, Transform _formationAnchor, PositionState _positionState, BeachVolley_Define.PositionType _positionType, int _userType)
	{
		rigid = GetComponent<Rigidbody>();
		rigid.maxAngularVelocity = JUMP_POWER;
		charaCollider = obj.GetComponent<CapsuleCollider>();
		charaBodySize = charaCollider.radius * obj.localScale.x * 2f;
		charaHeight = charaCollider.height * obj.localScale.y;
		base.gameObject.name = base.gameObject.name + _teamNo.ToString() + "_" + _charaNo.ToString();
		teamNo = _teamNo;
		opponentTeamNo = ((teamNo == 0) ? 1 : 0);
		charaNo = _charaNo;
		IsPlayer = (_userType <= 3);
		if (charaNo < 2)
		{
			userType = (BeachVolley_Define.UserType)_userType;
		}
		runAnimationTime = 0f;
		playSeRunCnt = 0;
		formationNo = _formationNo;
		formationAnchor = _formationAnchor;
		positionState = _positionState;
		base.transform.localPosition = formationAnchor.localPosition;
		defPos = (nowPos = (prevPos = base.transform.position));
		base.transform.rotation = formationAnchor.rotation;
		SettingLayer(BeachVolley_Define.LAYER_CHARACTER);
		UnityEngine.Debug.Log("セット時のポジションタイプ：" + _positionType.ToString());
		positionType = _positionType;
		SettingCharaParameter();
		if (positionState == PositionState.BENCH)
		{
			ShowCharacter(_show: false);
			base.transform.position = BeachVolley_Define.FM.GetBenchAnchor(teamNo).position;
		}
		style.SetGameStyle(GS_Define.GameType.MOLE_HAMMER, (int)userType);
	}
	protected void SettingCharaParameter()
	{
		charaParam.defense = 4;
		charaParam.offense = 3;
		charaParam.jump = 6;
		charaParam.speed = 3;
		name = "hoge";
		uniformNumber = 0;
		statusData.moveSpeed = paramColcData.moveSpeed[0] + paramColcData.moveSpeed[1] * ((float)charaParam.speed / (float)BeachVolley_Define.TEAM_STATUS_SCALE_NUM);
		statusData.moveSpeedMax = paramColcData.moveSpeedMaxDef * (statusData.moveSpeed / paramColcData.moveSpeed[0]);
		statusData.moveDelay = paramColcData.moveDelay[0] - paramColcData.moveDelay[1] * ((float)charaParam.defense / (float)BeachVolley_Define.TEAM_STATUS_SCALE_NUM);
		statusData.jumpServeShufflePer = paramColcData.jumpServeShufflePer[0] - paramColcData.jumpServeShufflePer[1] * ((float)charaParam.offense / (float)BeachVolley_Define.TEAM_STATUS_SCALE_NUM);
		statusData.blockMissPer = paramColcData.blockMissPerBase[0] - paramColcData.blockMissPerBase[1] * ((float)charaParam.defense / (float)BeachVolley_Define.TEAM_STATUS_SCALE_NUM);
		statusData.blockNicePer = paramColcData.blockNicePerBase[0] + paramColcData.blockNicePerBase[1] * ((float)charaParam.defense / (float)BeachVolley_Define.TEAM_STATUS_SCALE_NUM);
		statusData.blockMissAddPer = paramColcData.blockMissAddPerBase[0] + paramColcData.blockMissAddPerBase[1] * ((float)charaParam.defense / (float)BeachVolley_Define.TEAM_STATUS_SCALE_NUM);
		statusData.blockNiceReducePer = paramColcData.blockNiceReducePerBase[0] + paramColcData.blockNiceReducePerBase[1] * ((float)charaParam.defense / (float)BeachVolley_Define.TEAM_STATUS_SCALE_NUM);
		statusData.jumpHeight = (paramColcData.jumpHeightCorr[0] + paramColcData.jumpHeightCorr[1] * ((float)charaParam.jump / (float)BeachVolley_Define.TEAM_STATUS_SCALE_NUM)) * GetCharaHeight();
	}
	public void ResetPosData()
	{
		nowPos = (prevPos = base.transform.position);
	}
	public void ResetControlInterval(float _time = 0.01f)
	{
		controlInterval = _time;
	}
	public void ResetMoveInterval(float _delay = -1f)
	{
		if (_delay < 0f)
		{
			moveDelay = statusData.moveDelay;
		}
		else
		{
			moveDelay = _delay;
		}
	}
	public void SetChangeCharaMoveTime(float _time)
	{
		changeCharaMoveTime = _time;
	}
	public float GetChengeCharaMoveTime()
	{
		return changeCharaMoveTime;
	}
	public void UpdateMethod()
	{
		if (base.transform.localPosition.z > 8f)
		{
			base.transform.SetLocalPositionZ(8f);
		}
		if (BeachVolley_Define.MGM.GetGameState() == BeachVolley_MainGameManager.GameState.SERVE && actionState == ActionState.STANDARD && base.transform.localPosition.z < 0.7f)
		{
			base.transform.SetLocalPositionZ(0.7f);
		}
		triggerCheckInterval -= Time.deltaTime;
		UpdateDebugActionState();
		prevPos = nowPos;
		nowPos = base.transform.position;
		if (BeachVolley_Define.MGM.CheckInPlay())
		{
			if (teamNo == 0)
			{
				if (GetPos().z > BeachVolley_Define.FM.GetFieldData().GetCenterPos().z)
				{
					isMoveOverNet = true;
					SettingLayer(BeachVolley_Define.LAYER_INVISIBLE_CHARA);
				}
				else if (isMoveOverNet)
				{
					SettingLayer(BeachVolley_Define.LAYER_CHARACTER);
				}
			}
			else if (GetPos().z < BeachVolley_Define.FM.GetFieldData().GetCenterPos().z)
			{
				isMoveOverNet = true;
				SettingLayer(BeachVolley_Define.LAYER_INVISIBLE_CHARA);
			}
			else if (isMoveOverNet)
			{
				SettingLayer(BeachVolley_Define.LAYER_CHARACTER);
			}
		}
		if (throwAnimationTime > 0f)
		{
			throwAnimationTime -= Time.deltaTime;
			if (throwAnimationTime <= 0f)
			{
				ResetMotion();
			}
			return;
		}
		switch (actionState)
		{
		case ActionState.SERVE_WAIT:
			StateServeWait();
			break;
		case ActionState.TOSS:
			StateTossAction();
			if (playerNo >= 0)
			{
				MovePosToss();
				UnityEngine.Debug.Log("トス！");
			}
			break;
		case ActionState.DIVING_TOSS:
		case ActionState.DIVING_ATTACK:
			StateDivingTossAction();
			if (playerNo >= 0)
			{
				MoveBallDropPorediPos();
				UnityEngine.Debug.Log("ダイビング！");
			}
			break;
		case ActionState.ATTACK:
			StateAttackAction();
			if (playerNo >= 0)
			{
				MoveBallDropPorediPos();
				UnityEngine.Debug.Log("アタック！");
			}
			break;
		case ActionState.SPIKE_AFTER:
			StateSpikeAfter();
			break;
		case ActionState.FREEZE:
			StateFreeze();
			break;
		case ActionState.NEXT_STAY:
			StateStayAction();
			break;
		default:
			MoveAnimation();
			break;
		}
		AddGravity();
		ParticleSystem.MainModule main = runEffect.main;
		main.loop = (MCM.CheckHaveBall(this) && GetMoveVec().magnitude >= 0.01f);
		if (main.loop && !runEffect.isPlaying)
		{
			runEffect.Play();
		}
		if (GetPos(_isLocal: true).y > statusData.jumpHeight && rigid.velocity.y > 0f)
		{
			Vector3 velocity = rigid.velocity;
			velocity.y *= 0.2f;
			rigid.velocity = velocity;
			isHightJumped = true;
		}
		if (isHightJumped)
		{
			Vector3 velocity2 = rigid.velocity;
			velocity2.x *= 0.88f;
			velocity2.z *= 0.88f;
			rigid.velocity = velocity2;
		}
		if (isHightJumped && GetPos().y < BeachVolley_Define.FM.GetFieldData().GetCenterPos().y + GetCharaHeight() * 0.5f)
		{
			isHightJumped = false;
		}
		if (base.transform.position.y < BeachVolley_Define.FM.GetFieldData().GetCenterPos().y - GetCharaHeight() * 0.5f)
		{
			base.transform.SetPositionY(BeachVolley_Define.FM.GetFieldData().GetCenterPos().y);
		}
		controlInterval -= Time.deltaTime;
		moveDelay -= Time.deltaTime;
		runSeInterval -= Time.deltaTime;
		changeFormationInterval -= Time.deltaTime;
		movePosChangeInterval -= Time.deltaTime;
		if (playerNo < 0 && BeachVolley_Define.MCM.BallTouchCnt > 0 && actionState == ActionState.STANDARD && playerNo < 0 && Vector3.Distance(GetPos(), BeachVolley_Define.BM.GetBallDropPrediPosGround()) > 1f)
		{
			LookBall();
		}
		if (delay > 0f)
		{
			delay -= Time.deltaTime;
			if (delay < 0f)
			{
				JumpAction(CpuAi.GetBlockPower(teamNo), -1);
			}
		}
		if (isDelayInput && actionState != ActionState.STANDARD)
		{
			UnityEngine.Debug.Log("遅延入力：" + actionState.ToString());
			isDelayInput = false;
			if (delayCantactBallMethod != null)
			{
				StopCoroutine(delayCantactBallMethod);
			}
			delayCantactBallMethod = null;
			delayCantactBallMethod = DelayCantactBall(0f);
			StartCoroutine(delayCantactBallMethod);
		}
	}
	public void AddGravity(float _mag = 1f)
	{
		rigid.AddForce(0f, (0f - ADD_GRAVITY) * _mag * Time.deltaTime, 0f, ForceMode.Impulse);
	}
	public void RigidZero()
	{
		rigid.velocity = Vector3.zero;
	}
	public void RigidStop()
	{
		rigid.isKinematic = true;
	}
	public void Move(Vector3 _moveDir, float _moveSpeed, float _speedMag = 1f, bool _moveRot = true)
	{
		if ((!CheckAir() || !FM.CheckInCourt(GetPos(), GetCharaBodySize() * 0.4f)) && !(throwAnimationTime > 0f))
		{
			Vector3 vector = _moveDir;
			vector.y = 0f;
			if (_moveSpeed <= WALK_SPEED * 0.5f)
			{
				_moveSpeed = WALK_SPEED * 0.5f;
			}
			else if (_moveSpeed <= WALK_SPEED)
			{
				_moveSpeed = WALK_SPEED;
			}
			vector *= statusData.moveSpeed * _moveSpeed * _speedMag;
			if (CheckAir())
			{
				vector *= 0.1f;
			}
			vector = ConvertServeMoveLimit(vector);
			if (BeachVolley_Define.MGM.CheckTutorialServe())
			{
				vector = Vector3.zero;
			}
			rigid.AddForce(vector + Vector3.down, ForceMode.Acceleration);
			MoveRot(_moveDir, _immediate: false, _isCanStop: true);
			MoveAnimation();
			if (rigid.velocity.magnitude > statusData.moveSpeedMax * _speedMag)
			{
				Vector3 velocity = rigid.velocity.normalized * statusData.moveSpeedMax * _speedMag;
				velocity.y = rigid.velocity.y;
				rigid.velocity = velocity;
			}
		}
	}
	private Vector3 ConvertServeMoveLimit(Vector3 _vec)
	{
		if (CheckActionState(ActionState.SERVE_WAIT))
		{
			if (GetPos().x > BeachVolley_Define.FM.GetFieldData().backRight.transform.position.x && _vec.x > 0f)
			{
				_vec.x = 0f;
			}
			else if (GetPos().x < BeachVolley_Define.FM.GetFieldData().frontLeft.transform.position.x && _vec.x < 0f)
			{
				_vec.x = 0f;
			}
			if (GetPos(_isLocal: true).z > 0f && ConvertLocalVec(_vec).z > 0f)
			{
				_vec.z = 0f;
			}
			if (GetPos(_isLocal: true).z < BeachVolley_Define.FM.GetServeAnchor(teamNo).localPosition.z && ConvertLocalVec(_vec).z < 0f)
			{
				_vec.z = 0f;
			}
			if (BeachVolley_Define.MGM.IsTutorial)
			{
				walkDiff += Vector3.Distance(prevPos, GetPos());
				if (walkDiff > 5f)
				{
					BeachVolley_Define.MGM.TutorialServeMove(teamNo);
				}
			}
		}
		return _vec;
	}
	protected void MoveRot(Vector3 _moveDir, bool _immediate = false, bool _isCanStop = false)
	{
		if (_isCanStop || CalcManager.Length(nowPos, prevPos) > 0.01f)
		{
			calcVec[0] = _moveDir;
			rot.x = 0f;
			rot.y = CalcManager.Rot(calcVec[0], CalcManager.AXIS.Y);
			rot.z = 0f;
			if (_immediate)
			{
				rigid.MoveRotation(Quaternion.Euler(rot));
			}
			else
			{
				rigid.MoveRotation(Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(rot), 20f * Time.deltaTime));
			}
		}
	}
	public void ServeStandby()
	{
		SetAction(AiServeStandby, _immediate: true, _forcibly: true);
		UnityEngine.Debug.Log("サ\u30fcブの準備に来た！");
		BeachVolley_Define.MCM.SetLayer1(25);
		movePos = BeachVolley_Define.FM.GetServeAnchor(teamNo).position;
		movePoslist.Clear();
		movePoslist.Add(movePos);
		ServeStandbyMotion();
		ignoreObstacleTime = 0.5f;
		moveSpeedMag = 1.5f;
		if (!BeachVolley_Define.MGM.CheckTutorialServe())
		{
			actionState = ActionState.SERVE_STANDBY;
		}
	}
	public int GetMoveListCount()
	{
		return movePoslist.Count;
	}
	protected void AiServeStandby()
	{
		if (!MoveTarget(SYSTEM_MOVE_SPEED * 0.7f, STOP_CHECK_DISTANCE, Mathf.Max(Mathf.Min((movePos - GetPos()).magnitude * moveSpeedMag, SYSTEM_SPEED_MAX), SYSTEM_SPEED_MIN)))
		{
			BeachVolley_Define.MCM.CatchBall(this, teamNo);
			if (playerNo >= 0 && !BeachVolley_Define.MGM.IsTutorial)
			{
				BeachVolley_Define.GUM.StartTimeLimit(playerNo, 8f);
			}
			BeachVolley_Define.Ball.SetGhost(_flg: false);
			rigid.velocity = CalcManager.mVector3Zero;
			UnityEngine.Debug.Log("フォワ\u30fcド：" + base.gameObject.name);
			LookForward();
			ServeWait();
		}
	}
	public void ServeWait()
	{
		if (CheckActionState(ActionState.SERVE_STANDBY))
		{
			UnityEngine.Debug.Log("サ\u30fcブの最初");
			BeachVolley_Define.MCM.SetLayer1(13);
			actionState = ActionState.SERVE_WAIT;
			SetAction(AiServeWait, _immediate: true, _forcibly: true);
		}
	}
	private void StateServeWait()
	{
		rigid.velocity = ConvertServeMoveLimit(rigid.velocity);
	}
	public void AiServeWait()
	{
		if (aiActionTime >= 1f)
		{
			if (CpuAi.CheckJumpServe(teamNo))
			{
				BeachVolley_Define.MCM.ServeToss(this, 0.25f + 0.85f * CpuAi.GetServePower(teamNo) - 0.01f);
			}
			else
			{
				BeachVolley_Define.MCM.ServeToss(this, 0.25f * CpuAi.GetServePower(teamNo));
			}
		}
	}
	public void StandServe(float _gaugeValue)
	{
		if (CheckActionState(ActionState.SERVE_WAIT))
		{
			gaugeValue = _gaugeValue;
			CharaStop();
			LookForward(_isImmediately: true);
			SpikeChargeMotion();
			actionState = ActionState.STAND_SERVE;
			SetAction(AiStandServe, _immediate: true, _forcibly: true);
		}
	}
	public void MiniGameStandServe(float _gaugeValue, Vector3 _servePos)
	{
		if (CheckActionState(ActionState.SERVE_WAIT))
		{
			gaugeValue = _gaugeValue;
			CharaStop();
			LookForward(_isImmediately: true);
			SpikeChargeMotion();
			actionState = ActionState.STAND_SERVE;
			minigameServePos = _servePos;
			SetAction(AiMiniGameStandServe, _immediate: true, _forcibly: true);
		}
	}
	public void AiStandServe()
	{
		BeachVolley_Define.MCM.UpdateCursorDir((BeachVolley_Define.BM.CalcServeTagetPos(this, BeachVolley_Define.CM.GetMoveDir(playerNo) * BeachVolley_Define.CM.GetMoveLength(playerNo)) - GetPos()).normalized, playerNo);
		if (BeachVolley_Define.Ball.transform.position.y - BeachVolley_Define.Ball.BallSize() < GetPos().y + GetCharaHeight() && BeachVolley_Define.Ball.GetRigid().velocity.y < 0f)
		{
			if (IsPlayer)
			{
				SingletonCustom<HidVibration>.Instance.SetCustomVibration(playerNo, HidVibration.VibrationType.Weak, 0.2f);
				BeachVolley_Define.MCM.ServeShot(this, gaugeValue, BeachVolley_Define.CM.GetMoveDir(playerNo) * BeachVolley_Define.CM.GetMoveLength(playerNo), _isStand: true);
			}
			else
			{
				BeachVolley_Define.MCM.ServeShot(this, gaugeValue, ConvertLocalVec(CpuAi.GetServeVec(teamNo)), _isStand: true);
			}
			BeachVolley_Define.GUM.SetLimitPassCnt(1 - teamNo, BeachVolley_Define.MCM.BALL_TOUCH_LIMIT, playerNo);
			BeachVolley_Define.Ball.SetLastHitChara(this);
			SpikeShotMotion();
			ChangeStandardState(_resetMotion: false);
		}
	}
	public void AiMiniGameStandServe()
	{
	}
	public void JumpServe(float _gaugeValue)
	{
		if (CheckActionState(ActionState.SERVE_WAIT))
		{
			gaugeValue = _gaugeValue;
			CharaStop();
			LookForward(_isImmediately: true);
			actionState = ActionState.JUMP_SERVE;
			SetAction(AiJumpServeJump, _immediate: true, _forcibly: true);
		}
	}
	public void AiJumpServeJump()
	{
		BeachVolley_Define.MCM.UpdateCursorDir((BeachVolley_Define.BM.CalcServeTagetPos(this, BeachVolley_Define.CM.GetMoveDir(playerNo) * BeachVolley_Define.CM.GetMoveLength(playerNo)) - GetPos()).normalized, playerNo);
		if (BeachVolley_Define.Ball.GetRigid().velocity.y < 0f)
		{
			rigid.AddForce((base.transform.forward * 0.4f + base.transform.up * (1f + (float)GetCharaParam().jump * 0.05f)) * JUMP_POWER, ForceMode.Impulse);
			SpikeChargeMotion();
			SetAction(AiJumpServeShot, _immediate: true, _forcibly: true);
		}
	}
	public void AiJumpServeShot()
	{
		if (BeachVolley_Define.Ball.GetRigid().velocity.y < 0f && (BeachVolley_Define.Ball.transform.position.y < GetPos().y + GetCharaHeight() || GetRigid().velocity.y <= 0f))
		{
			if (IsPlayer)
			{
				SingletonCustom<HidVibration>.Instance.SetCustomVibration(playerNo, HidVibration.VibrationType.Normal, 0.38f);
				BeachVolley_Define.MCM.ServeShot(this, gaugeValue, BeachVolley_Define.CM.GetMoveDir(playerNo) * BeachVolley_Define.CM.GetMoveLength(playerNo), _isStand: false);
			}
			else
			{
				BeachVolley_Define.MCM.ServeShot(this, gaugeValue, ConvertLocalVec(CpuAi.GetServeVec(teamNo)), _isStand: false);
			}
			Vector3 velocity = rigid.velocity;
			velocity.y *= 0.1f;
			rigid.velocity = velocity;
			BeachVolley_Define.GUM.SetLimitPassCnt(1 - teamNo, BeachVolley_Define.MCM.BALL_TOUCH_LIMIT, playerNo);
			BeachVolley_Define.Ball.SetLastHitChara(this);
			SpikeShotMotion();
			ChangeStandardState(_resetMotion: false);
		}
	}
	public void AiMiniGameJumpServeJump()
	{
	}
	public void AiMiniGameJumpServeShot()
	{
	}
	protected void ResetMotion()
	{
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, 0.145756f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalPosition(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(270f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(270f, 0f, 0f);
	}
	protected void MoveAnimation()
	{
		if ((!CheckActionState(ActionState.STANDARD) && !CheckActionState(ActionState.SERVE_WAIT)) || throwAnimationTime > 0f)
		{
			return;
		}
		if (!CheckActionState(ActionState.SERVE_WAIT))
		{
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * 30f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * -30f);
		}
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * 30f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * -30f);
		if (CalcManager.Length(nowPos, prevPos) > 0.01f)
		{
			runAnimationTime += CalcManager.Length(nowPos, prevPos) * runAnimationSpeed * Time.deltaTime;
			if (runAnimationTime >= (float)playSeRunCnt * 0.5f)
			{
				playSeRunCnt++;
				PlaySeRun();
			}
			if (runAnimationTime >= 1f)
			{
				runAnimationTime = 0f;
				playSeRunCnt = 1;
			}
			isChangeAnimationNeutral = false;
		}
		else
		{
			if (isChangeAnimationNeutral)
			{
				return;
			}
			if (runAnimationTime <= 0.25f)
			{
				runAnimationTime -= 1f * Time.deltaTime;
				if (runAnimationTime <= 0f)
				{
					runAnimationTime = 0f;
					isChangeAnimationNeutral = true;
				}
			}
			else if (runAnimationTime <= 0.5f)
			{
				runAnimationTime += 1f * Time.deltaTime;
				if (runAnimationTime >= 0.5f)
				{
					runAnimationTime = 0.5f;
					isChangeAnimationNeutral = true;
				}
			}
			else if (runAnimationTime <= 0.75f)
			{
				runAnimationTime -= 1f * Time.deltaTime;
				if (runAnimationTime <= 0.5f)
				{
					runAnimationTime = 0.5f;
					isChangeAnimationNeutral = true;
				}
			}
			else
			{
				runAnimationTime += 1f * Time.deltaTime;
				if (runAnimationTime >= 1f)
				{
					runAnimationTime = 1f;
					isChangeAnimationNeutral = true;
				}
			}
		}
	}
	public void ServeStandbyMotion()
	{
		ResetMotion();
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(270f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAnglesX(270f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAnglesX(0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAnglesX(0f);
	}
	public void BlockMotion()
	{
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(-170f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(-170f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
	}
	public void SpikeChargeMotion()
	{
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(10f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(-15f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(-40f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(-115f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(-230f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
	}
	public void SpikeShotMotion()
	{
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(-15f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(10f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(30f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(15f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(-50f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
		throwAnimationTime = 0.25f;
	}
	public void TossStandbyMotion()
	{
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(-15f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(-17f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalPosition(0f, 0.15f, 0.005f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(205f, 0f, -33f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 40f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(205f, 0f, 33f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, -40f);
	}
	public void TossAfterMotion()
	{
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(-170f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(-170f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
	}
	public void DivingTossStandbyMotion(bool _isRightDiving)
	{
		obj.SetLocalPosition(0f, GetCharaBodySize() * obj.localScale.x, 0f);
		if (_isRightDiving)
		{
			obj.SetLocalEulerAngles(225f, 90f, 90f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(220f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(30f, 0f, 0f);
		}
		else
		{
			obj.SetLocalEulerAngles(315f, 90f, 90f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(30f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(220f, 0f, 0f);
		}
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
	}
	public void UnderTossMotion()
	{
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(-40f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, 0.16f, -0.117f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(45f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalPosition(0f, -0.013f, -0.009f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(-20f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(-20f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(270f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(270f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
	}
	public void UnderTossAfterMotion()
	{
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(-15f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(15f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(260f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(260f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
	}
	protected void PlaySeRun()
	{
		if (playerNo >= 0)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.2f);
		}
	}
	public void SetActionState(ActionState _state)
	{
		actionState = _state;
	}
	public void ResetObjPosition()
	{
		obj.SetLocalPosition(0f, 0f, 0f);
		obj.SetLocalEulerAngles(0f, 0f, 0f);
		base.transform.SetEulerAngles(0f, base.transform.rotation.eulerAngles.y, 0f);
		ResetMotion();
		rigid.constraints = (RigidbodyConstraints)80;
	}
	protected bool CheckOutOfBounds(Vector3 _pos)
	{
		return false;
	}
	protected void RingCheckAction(string _tag)
	{
	}
	protected IEnumerator _ResetKinematic(float _delay)
	{
		yield return new WaitForSeconds(_delay);
		rigid.isKinematic = false;
	}
	protected virtual void Toss(bool _isNice = false)
	{
		if (playerNo >= 0 && IsPlayer)
		{
			BeachVolley_Define.MCM.Toss(this, gaugeValue, BeachVolley_Define.CM.GetMoveDir(playerNo) * BeachVolley_Define.CM.GetMoveLength(playerNo), isOverToss);
		}
		else if (targetChara != null)
		{
			BeachVolley_Define.MCM.Toss(this, gaugeValue, (targetChara.GetPos() - BeachVolley_Define.FM.GetAttackLineAnchor(teamNo).position).normalized, isOverToss);
		}
		prevPos = (nowPos = base.transform.position);
		if (CheckActionState(ActionState.TOSS))
		{
			if (isOverToss)
			{
				TossAfterMotion();
			}
			else
			{
				UnderTossAfterMotion();
			}
		}
		ChangeFreezeState(actionTimeData.freezeTimeToss);
		SingletonCustom<HidVibration>.Instance.SetCustomVibration(playerNo, HidVibration.VibrationType.Weak, 0.15f);
	}
	protected virtual void MiniGameToss(int _ballNo)
	{
		BeachVolley_Define.MCM.MiniGameToss(_ballNo, this, 1f, new Vector3(0f, 0f, 1f), _isOver: false);
		SingletonCustom<HidVibration>.Instance.SetCustomVibration(playerNo, HidVibration.VibrationType.Weak, 0.15f);
	}
	protected virtual void Spike()
	{
		UnityEngine.Debug.Log("スパイク");
		if (BeachVolley_Define.BM.GetBallPos(_offset: false).y < BeachVolley_Define.FM.GetFieldData().GetNetTopPos().y + BeachVolley_Define.BM.GetBallSize() * 1.5f)
		{
			Vector3 ballPos = BeachVolley_Define.BM.GetBallPos(_offset: false);
			ballPos.y = BeachVolley_Define.FM.GetFieldData().GetNetTopPos().y + BeachVolley_Define.BM.GetBallSize() * 1.5f;
			BeachVolley_Define.Ball.transform.position = ballPos;
		}
		else
		{
			Vector3 position = GetPos() + base.transform.forward * GetCharaBodySize() * 1.25f;
			position.y += GetCharaHeight();
			BeachVolley_Define.Ball.transform.position = position;
		}
		gaugeValue = 1f;
		UnityEngine.Debug.Log("ゲ\u30fcジ：" + gaugeValue.ToString());
		if (playerNo >= 0 && IsPlayer)
		{
			BeachVolley_Define.MCM.Spike(this, gaugeValue, BeachVolley_Define.CM.GetMoveDir(playerNo) * BeachVolley_Define.CM.GetMoveLength(playerNo), _miss: false);
		}
		else
		{
			BeachVolley_Define.MCM.Spike(this, gaugeValue, ConvertLocalVec(CpuAi.GetSpikeVec(teamNo)), _miss: false);
		}
		SpikeShotMotion();
		prevPos = (nowPos = base.transform.position);
		Vector3 velocity = rigid.velocity;
		velocity.y = 0f;
		rigid.velocity = velocity;
		actionState = ActionState.SPIKE_AFTER;
		spikeAfterTime = actionTimeData.spikeAfterTime;
		SettingLayer(BeachVolley_Define.LAYER_CHARA_WALL);
		SingletonCustom<HidVibration>.Instance.SetCustomVibration(playerNo, HidVibration.VibrationType.Strong, 0.4f);
	}
	protected virtual void Attack(bool _isNice = false)
	{
		CharaStop();
		if (IsPlayer)
		{
			BeachVolley_Define.MCM.Attack(this, gaugeValue, BeachVolley_Define.CM.GetMoveDir(playerNo) * BeachVolley_Define.CM.GetMoveLength(playerNo), _miss: false);
		}
		else
		{
			BeachVolley_Define.MCM.Attack(this, gaugeValue, Vector3.back * 0.75f, _miss: false);
		}
		prevPos = (nowPos = base.transform.position);
		Vector3 velocity = rigid.velocity;
		velocity.y = 0f;
		rigid.velocity = velocity;
		ChangeFreezeState(1f);
		SingletonCustom<HidVibration>.Instance.SetCustomVibration(playerNo, HidVibration.VibrationType.Weak, 0.15f);
	}
	protected virtual void Block(bool _isNice = false, bool _isMiss = false)
	{
		if (playerNo >= 0 && IsPlayer)
		{
			BeachVolley_Define.MCM.Block(this, gaugeValue, BeachVolley_Define.CM.GetMoveDir(playerNo) * BeachVolley_Define.CM.GetMoveLength(playerNo), _isNice, _isMiss);
		}
		else
		{
			BeachVolley_Define.MCM.Block(this, gaugeValue, CpuAi.GetBlockVec(teamNo), _isNice, _isMiss);
		}
		prevPos = (nowPos = base.transform.position);
		Vector3 velocity = rigid.velocity;
		velocity.y = 0f;
		rigid.velocity = velocity;
		ChangeFreezeState(1f);
	}
	protected virtual void MissTouch()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_kick", _loop: false, 0f, 0.5f);
		BeachVolley_Define.MCM.MissTouch(this, CheckActionState(ActionState.JUMP) || CheckActionState(ActionState.SPIKE) || CheckActionState(ActionState.SPIKE_AFTER));
		CharaKnockBack();
		if (BeachVolley_Define.Ball.GetLastHitChara().teamNo != teamNo)
		{
			CharaKnockBack();
		}
		else
		{
			ChangeFreezeState(actionTimeData.freezeTimeMiss);
		}
		SingletonCustom<HidVibration>.Instance.SetCustomVibration(playerNo, HidVibration.VibrationType.Weak, 0.25f);
	}
	public bool CheckPositionType(BeachVolley_Define.PositionType _type)
	{
		return positionType == _type;
	}
	public bool CheckCharaHide()
	{
		return !bodyParts.bodyAnchor.activeSelf;
	}
	protected bool CheckBallSteal(BeachVolley_Character _chara, bool _ballHit, bool _backPress = false)
	{
		return false;
	}
	protected virtual bool CheckKnockBackChara()
	{
		if (BeachVolley_Define.Ball.GetLastHitChara().teamNo == teamNo)
		{
			return false;
		}
		if (CheckActionState(ActionState.FREEZE) || CheckActionState(ActionState.SPIKE_AFTER))
		{
			return true;
		}
		if (playerNo >= 0)
		{
			bool isPlayer2 = IsPlayer;
			return false;
		}
		return false;
	}
	public bool CheckKickoffPosition()
	{
		if (CalcManager.Length(gameStartStandbyPos, GetPos()) <= STOP_CHECK_DISTANCE)
		{
			return true;
		}
		return false;
	}
	public bool CheckObj(GameObject _obj)
	{
		if (!(_obj == obj.gameObject))
		{
			return base.gameObject == _obj;
		}
		return true;
	}
	public bool CheckActionState(ActionState _state)
	{
		return actionState == _state;
	}
	public bool CheckPositionState(PositionState _state)
	{
		return positionState == _state;
	}
	public Vector3 CheckCanIntrusionArea(Vector3 _pos)
	{
		return _pos;
	}
	public Rigidbody GetRigid()
	{
		return rigid;
	}
	public Vector3 GetMoveVec()
	{
		return nowPos - prevPos;
	}
	public Vector3 GetPos(bool _isLocal = false)
	{
		if (_isLocal)
		{
			return base.transform.localPosition;
		}
		return base.transform.position;
	}
	public CapsuleCollider GetCollider()
	{
		return charaCollider;
	}
	public float GetCharaBodySize()
	{
		return charaBodySize;
	}
	public float GetCharaHeight()
	{
		return charaHeight;
	}
	public Vector3 GetFormationPos(bool _world = true)
	{
		Vector3 vector = formationPos = formationAnchor.localPosition;
		if (BeachVolley_Define.MGM.CheckGameState(BeachVolley_MainGameManager.GameState.IN_PLAY) || BeachVolley_Define.MGM.CheckGameState(BeachVolley_MainGameManager.GameState.SERVE) || BeachVolley_Define.MGM.CheckGameState(BeachVolley_MainGameManager.GameState.SERVE_STANDBY))
		{
			if (BeachVolley_Define.MCM.BallControllTeam != teamNo)
			{
				if (!BeachVolley_Define.Ball.IsServeBall && BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.FREE))
				{
					if (BeachVolley_Define.BM.GetBallControlTime() <= 0.75f)
					{
						vector = BeachVolley_Define.FM.ConvertLocalPosPer(BeachVolley_Define.FM.ConvertWorldPos(formationPos, teamNo), teamNo);
						vector.z += (0.5f - vector.z) * 0.5f;
						formationPos = BeachVolley_Define.FM.ConvertPosPerToLocal(vector);
					}
					else if (CheckPositionState(PositionState.FRONT_ZONE))
					{
						formationPos.z = BeachVolley_Define.FM.ConvertLocalPos(BeachVolley_Define.FM.GetFieldData().GetCenterPos() + ConvertLocalVec(Vector3.back) * GetCharaBodySize(), teamNo).z;
					}
				}
			}
			else if (BeachVolley_Define.Ball.IsServeBall)
			{
				if (CheckPositionState(PositionState.FRONT_ZONE))
				{
					vector = BeachVolley_Define.FM.ConvertLocalPosPer(BeachVolley_Define.FM.ConvertWorldPos(formationPos, teamNo), teamNo);
					vector.z += (0.5f - vector.z) * 0.5f;
					formationPos = BeachVolley_Define.FM.ConvertPosPerToLocal(vector);
				}
				else if (debugShowData.other)
				{
					UnityEngine.Debug.Log("teamNo = " + teamNo.ToString() + " : No." + charaNo.ToString() + " : name = " + base.gameObject.name);
				}
			}
			else if (BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.FREE))
			{
				if (BeachVolley_Define.Ball.GetLastHitChara().teamNo != teamNo)
				{
					if (CheckPositionState(PositionState.FRONT_ZONE))
					{
						vector = BeachVolley_Define.FM.ConvertLocalPosPer(BeachVolley_Define.FM.ConvertWorldPos(formationPos, teamNo), teamNo);
						vector.z += (0.5f - vector.z) * 0.5f;
						formationPos = BeachVolley_Define.FM.ConvertPosPerToLocal(vector);
					}
				}
				else
				{
					Vector3 ballDropPrediPosGround = BeachVolley_Define.BM.GetBallDropPrediPosGround();
					ballDropPrediPosGround.y = 0f;
					if (attackStayFlg && BeachVolley_Define.MCM.BallTouchCnt != 2 && Vector3.Distance(GetPos(), ballDropPrediPosGround) > 1f)
					{
						Vector3 ballDropPrediPosGround2 = BeachVolley_Define.BM.GetBallDropPrediPosGround();
						vector = BeachVolley_Define.FM.ConvertLocalPosPer((ballDropPrediPosGround2 + BeachVolley_Define.FM.ConvertWorldPos(formationPos, teamNo)) / 2f, teamNo);
						formationPos = BeachVolley_Define.FM.ConvertPosPerToLocal(vector);
					}
				}
			}
		}
		if (CheckPositionState(PositionState.BACK_ZONE))
		{
			vector = BeachVolley_Define.FM.ConvertLocalPosPer(BeachVolley_Define.FM.ConvertWorldPos(formationPos, teamNo), teamNo);
			vector.z = Mathf.Min(vector.z, BeachVolley_Define.FM.ConvertLocalPosPer(BeachVolley_Define.FM.GetAttackLineAnchor(teamNo).position, teamNo).z - 0.05f);
			formationPos = BeachVolley_Define.FM.ConvertPosPerToLocal(vector);
			if (debugShowData.other)
			{
				UnityEngine.Debug.Log("teamNo = " + teamNo.ToString() + " : No." + charaNo.ToString() + " : name = " + base.gameObject.name + " : formationPos(" + formationPos.x.ToString() + "," + formationPos.y.ToString() + "," + formationPos.x.ToString() + ")");
			}
		}
		if (_world)
		{
			formationPos = formationAnchor.parent.TransformPoint(formationPos);
		}
		return formationPos;
	}
	public ActionState GetActionState()
	{
		return actionState;
	}
	public float GetControlInterval()
	{
		return controlInterval;
	}
	public float GetMoveInterval()
	{
		return moveDelay;
	}
	public string GetName()
	{
		return name;
	}
	public int GetUniformNumber()
	{
		return uniformNumber;
	}
	public BeachVolley_Define.PositionType GetPositionType()
	{
		return positionType;
	}
	public BeachVolley_Define.StatusType GetCharaParam()
	{
		return charaParam;
	}
	public StatusData GetStatusData()
	{
		return statusData;
	}
	public float GetStaminaPer()
	{
		return 1f;
	}
	public PositionState GetPositionState()
	{
		return positionState;
	}
	public Transform GetFormationAnchor()
	{
		return formationAnchor;
	}
	public Vector3 GetLookBallPos()
	{
		return lookBallPos;
	}
	public bool IsBallAction()
	{
		return isBallAction;
	}
	public Vector3 CheckNoEntryArea(Vector3 _pos)
	{
		return _pos;
	}
	public void SetPositionType(BeachVolley_Define.PositionType _type)
	{
		positionType = _type;
	}
	public void SetPositionState(PositionState _state)
	{
		positionState = _state;
	}
	public void SetFormation(int _formationNo, Transform _formationAnchor)
	{
		formationNo = _formationNo;
		formationAnchor = _formationAnchor;
	}
	public void SetFormationAnchor(Transform _formationAnchor)
	{
		formationAnchor = _formationAnchor;
	}
	protected bool FirstActionCall()
	{
		bool num = isInitAiAction;
		isInitAiAction = true;
		return !num;
	}
	public void SetAction(AiActionMethod _action, bool _immediate = true, bool _forcibly = false)
	{
		if ((aiActionMethod.Count <= 0 || !(aiActionMethod[0] == _action) || _forcibly) && ((actionChangeInterval <= 0f) | _immediate))
		{
			aiActionMethod.Clear();
			aiActionMethod.Add(_action);
			aiActionTime = 0f;
			actionChangeInterval = 1f;
			isCallAiAction = false;
			isInitAiAction = false;
			movePoslist.Clear();
			movePoslist.Add(CalcManager.mVector3Zero);
		}
	}
	public void SetNextAction(AiActionMethod _action, bool _immediate = true)
	{
		if ((aiActionMethod.Count <= 0 || !(aiActionMethod[aiActionMethod.Count - 1] == _action)) && ((actionChangeInterval <= 0f) | _immediate))
		{
			aiActionMethod.Add(_action);
			aiActionTime = 0f;
			actionChangeInterval = 0.5f;
			isCallAiAction = false;
			isInitAiAction = false;
		}
	}
	public void AiAction()
	{
		if (ballSearchInterval <= 0f || BM.CheckBallState(BeachVolley_BallManager.BallState.FREE))
		{
			lookBallPos = BM.GetBallPos() + Ball.GetMoveVec();
			ballSearchInterval = CpuAi.GetBallSearchInterval(teamNo);
		}
		else
		{
			ballSearchInterval -= Time.deltaTime;
		}
		if (aiActionMethod != null && aiActionMethod.Count > 0 && aiActionMethod[aiActionMethod.Count - 1] != null)
		{
			AiState = aiActionMethod[aiActionMethod.Count - 1].Method.Name;
			aiActionMethod[aiActionMethod.Count - 1]();
			isCallAiAction = true;
			aiActionTime += Time.deltaTime;
			ignoreObstacleTime -= Time.deltaTime;
		}
		actionChangeInterval -= Time.deltaTime;
	}
	protected bool MoveTarget(float _maxSpeed, float _stopDistance, float _speedMag = 1f)
	{
		if (CalcManager.Length(movePoslist[movePoslist.Count - 1], GetPos()) > _stopDistance)
		{
			if (movePosChangeInterval <= 0f && ignoreObstacleTime <= 0f)
			{
				for (int num = movePoslist.Count - 1; num > 0; num--)
				{
					if (CalcManager.Length(movePoslist[num], GetPos()) <= GetCharaBodySize())
					{
						movePoslist.RemoveAt(num);
					}
				}
				if (movePoslist.Count >= 2)
				{
					int num2 = movePoslist.Count;
					for (int i = 0; i < movePoslist.Count; i++)
					{
						Vector3 dir = movePoslist[i] - GetPos();
						if (!CheckCharacterFront(dir, dir.magnitude))
						{
							num2 = i;
							break;
						}
					}
					for (int num3 = movePoslist.Count - 1; num3 > num2; num3--)
					{
						movePoslist.RemoveAt(num3);
					}
				}
				Vector3 vector2 = movePoslist[movePoslist.Count - 1];
			}
			Vector3 vector = movePoslist[movePoslist.Count - 1] - GetPos();
			UnityEngine.Debug.DrawLine(GetPos(), movePoslist[movePoslist.Count - 1], Color.magenta);
			Move(vector.normalized, Mathf.Min(CalcManager.Length(movePoslist[movePoslist.Count - 1], GetPos()), _maxSpeed), _speedMag);
		}
		else
		{
			if (movePoslist.Count <= 1)
			{
				return false;
			}
			movePoslist.RemoveAt(movePoslist.Count - 1);
		}
		return true;
	}
	public void StartMovePos(Vector3 _pos, float _moveTime, float _moveSpeedMag = 1f, bool _isReturn = false)
	{
		SetAction(AiMovePos, _immediate: true, _forcibly: true);
		ResetObjPosition();
		movePos = _pos;
		movePos.y = BM.GetBallPos().y;
		movePoslist.Clear();
		movePoslist.Add(movePos);
		isMovePos = true;
		if (_isReturn)
		{
			isMoveReturn = true;
			moveReturnPos = base.transform.position;
		}
		rigid.isKinematic = false;
		moveSpeedMag = _moveSpeedMag;
		Vector3 vector = _pos;
		UnityEngine.Debug.Log("移動先 = " + vector.ToString());
		actionState = ActionState.MOVE_POS;
	}
	public void AiMovePos()
	{
		if (isMovePos && !MoveTarget(SYSTEM_MOVE_SPEED, STOP_CHECK_DISTANCE, Mathf.Max(Mathf.Min((movePos - GetPos()).magnitude * moveSpeedMag, SYSTEM_SPEED_MAX), SYSTEM_SPEED_MIN)))
		{
			isMovePos = false;
			rigid.MovePosition(movePos);
			if (isMoveReturn)
			{
				isMoveReturn = false;
				StartMovePos(moveReturnPos, 0f, moveSpeedMag);
			}
		}
	}
	public void SettingGameStartPos(Vector3 _pos)
	{
		ResetObjPosition();
		gameStartStandbyPos = _pos;
		base.transform.position = gameStartStandbyPos;
		ResetPosData();
	}
	protected bool RunTowardTarget(Vector3 _pos, float _maxSpeed, float _speedMag = 1f)
	{
		if (CalcManager.Length(_pos, GetPos()) > STOP_CHECK_DISTANCE)
		{
			Move((_pos - GetPos()).normalized, Mathf.Min(CalcManager.Length(_pos, GetPos()), _maxSpeed), _speedMag);
			return false;
		}
		return true;
	}
	protected bool RunTargetCertainTime(Vector3 _pos, float _time, float _moveSpeed)
	{
		runTime += Time.deltaTime;
		if (runTime <= _time)
		{
			Move((_pos - GetPos()).normalized, _moveSpeed);
			return true;
		}
		runTime = 0f;
		return false;
	}
	public void SettingReturnBench(Vector3 _benchPos)
	{
		rigid.isKinematic = false;
		ResetObjPosition();
		returnBenchPos = _benchPos;
		SetAction(AiReturnBench, _immediate: true, _forcibly: true);
	}
	public void AiReturnBench()
	{
		if (FirstActionCall())
		{
			movePoslist.Clear();
			movePoslist.Add(returnBenchPos);
			SettingLayer(BeachVolley_Define.LAYER_INVISIBLE_CHARA);
		}
		if (CalcManager.Length(returnBenchPos, GetPos()) <= GetCharaBodySize() * 1.8f)
		{
			ShowCharacter(_show: false);
		}
		else
		{
			Move((returnBenchPos - GetPos()).normalized, Mathf.Min(CalcManager.Length(returnBenchPos, GetPos()), SYSTEM_MOVE_SPEED), Mathf.Max(Mathf.Min((returnBenchPos - GetPos()).magnitude, SYSTEM_SPEED_MAX), SYSTEM_SPEED_MIN));
		}
	}
	public void AiReturnFromBench()
	{
		movePoslist[0] = GetFormationPos();
		if (!MoveTarget(RUN_SPEED, STOP_CHECK_DISTANCE, Mathf.Max(Mathf.Min((movePoslist[0] - GetPos()).magnitude, SYSTEM_SPEED_MAX), SYSTEM_SPEED_MIN)))
		{
			LookForward();
		}
	}
	public void SetGoalProduction(int _goalTeam)
	{
		actionState = ActionState.GOAL_PRODUCTION;
		ResetObjPosition();
		if (_goalTeam == teamNo)
		{
			joyJumpInterval = UnityEngine.Random.Range(0.5f, 0.75f);
			SetAction(AiJoy, _immediate: true, _forcibly: true);
		}
		else
		{
			SetAction(AiDespair, _immediate: true, _forcibly: true);
		}
	}
	public void AiDespair()
	{
	}
	public void AiJoy()
	{
		joyJumpInterval -= Time.deltaTime;
		if (joyJumpInterval <= 0f)
		{
			joyJumpInterval = UnityEngine.Random.Range(1f, 2f);
			rigid.AddForce(Vector3.up * 300f, ForceMode.Impulse);
		}
	}
	public bool CheckMoveBallDropPorediPos()
	{
		if (BeachVolley_Define.MGM.CheckGameState(BeachVolley_MainGameManager.GameState.IN_PLAY) && !BeachVolley_Define.MCM.CheckLastTouched() && ((BeachVolley_Define.Ball.GetLastHitChara().TeamNo == teamNo && !BeachVolley_Define.FM.CheckInCourt(BeachVolley_Define.BM.GetBallDropPrediPos(), 1 - teamNo, 0f)) || BeachVolley_Define.FM.CheckInCourt(BeachVolley_Define.BM.GetBallDropPrediPos(), teamNo, 0f)) && (BeachVolley_Define.Ball.GetLastControlChara().teamNo != teamNo || !BeachVolley_Define.Ball.IsServeBall) && (GetMoveInterval() <= 0f || BeachVolley_Define.Ball.IsServeBall))
		{
			return true;
		}
		return false;
	}
	public void MovePosToss()
	{
		Vector3 value = BeachVolley_Define.BM.GetBallDropPrediPos() + (BeachVolley_Define.BM.GetBallPos() - GetPos()) * 0.1f;
		movePoslist[0] = value;
		if (MoveTarget(Mathf.Min((movePoslist[0] - GetPos()).magnitude, SYSTEM_MOVE_SPEED), STOP_CHECK_DISTANCE))
		{
			LookTarget(BeachVolley_Define.BM.GetBallDropPrediPos());
		}
	}
	public void MoveBallDropPorediPos()
	{
		Vector3 value = BeachVolley_Define.BM.GetBallDropPrediPos() + (BeachVolley_Define.FM.GetFieldData().CenterCircle.transform.position - GetPos()).normalized * (GetCharaBodySize() * 0.1f);
		movePoslist[0] = value;
		if (!BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.KEEP))
		{
			MoveTarget(Mathf.Min((movePoslist[0] - GetPos()).magnitude, SYSTEM_MOVE_SPEED), STOP_CHECK_DISTANCE);
		}
	}
	public void MiniGameMoveBallDropPorediPos()
	{
	}
	public void MoveBlockingPos()
	{
		Vector3 value = default(Vector3);
		if (BeachVolley_Define.FM.CheckInCourt(BeachVolley_Define.BM.GetBallDropPrediPosGround()) && BeachVolley_Define.FM.CheckInFrontZone(BeachVolley_Define.FM.ConvertLocalPos(BeachVolley_Define.BM.GetBallDropPrediPosGround(), BeachVolley_Define.MCM.BallControllTeam), BeachVolley_Define.MCM.BallControllTeam))
		{
			value.x = BeachVolley_Define.BM.GetBallDropPrediPos().x;
			value.y = GetPos().y;
			value.z = (BeachVolley_Define.FM.GetFieldData().GetCenterPos() + ConvertLocalVec(Vector3.back) * GetCharaBodySize()).z;
		}
		else
		{
			value = GetFormationPos();
		}
		movePoslist[0] = value;
		if (!MoveTarget(Mathf.Min((movePoslist[0] - GetPos()).magnitude, SYSTEM_MOVE_SPEED), STOP_CHECK_DISTANCE))
		{
			LookTarget(BeachVolley_Define.BM.GetBallPos());
		}
		else
		{
			LookTarget(BeachVolley_Define.BM.GetBallDropPrediPos());
		}
	}
	public void MoveFormationPos()
	{
		movePoslist[0] = GetFormationPos();
		if (!MoveTarget(Mathf.Min((movePoslist[0] - GetPos()).magnitude, SYSTEM_MOVE_SPEED), STOP_CHECK_DISTANCE))
		{
			if (!BeachVolley_Define.MGM.CheckGameState(BeachVolley_MainGameManager.GameState.SERVE))
			{
				LookTarget(BeachVolley_Define.BM.GetBallPos());
			}
			else
			{
				LookForward();
			}
		}
		else
		{
			LookTarget(BeachVolley_Define.BM.GetBallDropPrediPos());
		}
	}
	public void AiMoveFormationPosition()
	{
		if (BeachVolley_Define.MGM.CheckGameState(BeachVolley_MainGameManager.GameState.SERVE_STANDBY))
		{
			if (FirstActionCall())
			{
				moveSpeedMag = 2f;
			}
			if (debugShowData.other)
			{
				UnityEngine.Debug.Log("フォ\u30fcメ\u30fcションポジション移動");
			}
			movePoslist[0] = GetFormationPos();
			if (!MoveTarget(RUN_SPEED * 1.2f, STOP_CHECK_DISTANCE, Mathf.Max(Mathf.Min((movePoslist[0] - GetPos()).magnitude, SYSTEM_SPEED_MAX), SYSTEM_SPEED_MIN)))
			{
				LookForward();
			}
		}
	}
	public void AiStandby()
	{
	}
	protected void ChangeStandardState(bool _resetMotion = true)
	{
		actionState = ActionState.STANDARD;
		ResetObjPosition();
		SetAction(AiStandby, _immediate: true, _forcibly: true);
	}
	protected void ChangeFreezeState(float _time)
	{
		freezeTime = _time;
		actionState = ActionState.FREEZE;
		SetAction(null, _immediate: true, _forcibly: true);
		UnityEngine.Debug.Log("行動をフリ\u30fcズに変更");
	}
	public void Gimmick_HitSnowStorm(float _time)
	{
		ChangeFreezeState(_time);
	}
	public void EndFreeze()
	{
		if (actionState == ActionState.FREEZE)
		{
			freezeTime = 0f;
			ChangeStandardState();
		}
	}
	public void StateFreeze()
	{
		freezeTime -= Time.deltaTime;
		if (freezeTime <= 0f)
		{
			ChangeStandardState();
		}
	}
	public void AiAttackMovePosition()
	{
		movePoslist[0] = GetFormationPos();
		if (BeachVolley_Define.BM.GetBallDropPrediPosGroundDistance(movePoslist[0]) <= GetCharaBodySize() * 2f)
		{
			if (BeachVolley_Define.Ball.GetLastHitChara() == this && playerNo >= 0)
			{
				movePoslist[0] = BeachVolley_Define.MCM.GetControlChara(playerNo).GetFormationPos();
			}
			else
			{
				List<Vector3> list = movePoslist;
				list[0] -= (BeachVolley_Define.BM.GetBallDropPrediPosGround() - movePoslist[0]).normalized * GetCharaBodySize() * 2f;
			}
		}
		if (!MoveTarget(Mathf.Min((movePoslist[0] - GetPos()).magnitude, SYSTEM_MOVE_SPEED), STOP_CHECK_DISTANCE))
		{
			LookForward();
		}
	}
	public void AttackAction(float _gaugeValue, bool _canNotSpike = false)
	{
		if (!CheckActionState(ActionState.STANDARD))
		{
			UnityEngine.Debug.Log("STANDARDでひっかかった：" + actionState.ToString());
			return;
		}
		gaugeValue = _gaugeValue;
		if ((BeachVolley_Define.BM.CheckBallUpperNet() && BeachVolley_Define.MCM.BallTouchCnt != 0) || BeachVolley_Define.MCM.CheckLastTouch())
		{
			UnityEngine.Debug.Log("ifの中に入った");
			CharaStop();
			actionState = ActionState.SPIKE;
			SpikeChargeMotion();
			BeachVolley_Define.MAI.GetDistanceFromBallDropPrediPos(teamNo, charaNo);
			Vector3 vector = BeachVolley_Define.BM.GetBallPos() - GetPos();
			vector.y = 0f;
			Vector3 ballPos = BeachVolley_Define.BM.GetBallPos();
			ballPos.y = 0f;
			Vector3 pos = GetPos();
			pos.y = 0f;
			Vector3 ballDropPrediPosGround = BeachVolley_Define.BM.GetBallDropPrediPosGround();
			ballDropPrediPosGround.y = 0f;
			float num = Vector3.Distance(ballPos, pos);
			float num2 = Vector3.Distance(ballDropPrediPosGround, pos);
			float num3 = Vector3.Distance(ballDropPrediPosGround, ballPos);
			if (num2 > 1f)
			{
				Vector3 a;
				if (Vector3.Distance(BeachVolley_Define.BM.GetBallPos(), GetPos()) < 1f)
				{
					UnityEngine.Debug.Log("近い：ボ\u30fcルに向かってとんだ");
					a = ballPos - pos;
				}
				else if (num2 > num3)
				{
					UnityEngine.Debug.Log("着地地点に向かってとんだ");
					a = ballDropPrediPosGround - pos;
				}
				else if (num > num3)
				{
					UnityEngine.Debug.Log("ボ\u30fcルと落下地点の間にとんだ１");
					a = (ballDropPrediPosGround + ballPos) / 2f - pos;
				}
				else if (num2 < num)
				{
					UnityEngine.Debug.Log("落下地点とボ\u30fcルの間にとんだ２");
					a = (ballDropPrediPosGround + ballPos) / 2f - pos;
				}
				else
				{
					UnityEngine.Debug.Log("落下地点と自分の間にとんだ");
					a = (ballDropPrediPosGround + pos) / 2f - pos;
				}
				if (a.magnitude > 2f)
				{
					a = a.normalized * 2f;
				}
				if (BeachVolley_Define.FM.CheckInFrontZone(this))
				{
					rigid.AddForce((a * 0.3f + base.transform.up) * (1f + (float)GetCharaParam().jump * 0.05f) * JUMP_POWER_SPIKE, ForceMode.Impulse);
				}
				else
				{
					rigid.AddForce((a * 0.3f + base.transform.up) * (1f + (float)GetCharaParam().jump * 0.05f) * JUMP_POWER_BACK_ATTACK, ForceMode.Impulse);
				}
			}
			else if (BeachVolley_Define.FM.CheckInFrontZone(this))
			{
				rigid.AddForce(((BeachVolley_Define.BM.GetBallPos() - GetPos()).normalized * 0.15f + base.transform.up * (1f + (float)GetCharaParam().jump * 0.05f)) * JUMP_POWER_SPIKE, ForceMode.Impulse);
			}
			else
			{
				rigid.AddForce(((BeachVolley_Define.BM.GetBallPos() - GetPos()).normalized * 0.15f + base.transform.up * (1f + (float)GetCharaParam().jump * 0.05f)) * JUMP_POWER_BACK_ATTACK, ForceMode.Impulse);
			}
			LookForward();
			SetAction(AiSpike, _immediate: true, _forcibly: true);
		}
		else
		{
			actionState = ActionState.ATTACK;
			actionInterval = actionTimeData.attackTime;
			UnderTossMotion();
			gaugeValue = 0.5f;
			SetAction(AiAttack, _immediate: true, _forcibly: true);
			UnityEngine.Debug.Log("アンダ\u30fcのアタック");
		}
	}
	public bool CheckDivingAttack()
	{
		if (CalcManager.Length(BeachVolley_Define.BM.GetBallDropPrediPos(), GetPos()) >= GetCharaBodySize() * DIVING_DISTANCE_CORR)
		{
			return BeachVolley_Define.BM.GetBallPos(_offset: false).y < BeachVolley_Define.FM.GetFieldData().GetNetTopPos().y;
		}
		return false;
	}
	public void DivingAttack(float _gaugeValue)
	{
		if (CheckActionState(ActionState.STANDARD))
		{
			gaugeValue = _gaugeValue;
			CalcManager.mCalcVector3 = ((BeachVolley_Define.BM.GetBallDropPrediPosGround() - GetPos()).normalized + Vector3.up * 0.3f) * Mathf.Min((BeachVolley_Define.BM.GetBallDropPrediPosGround() - GetPos()).magnitude * DIVING_POWER, DIVING_POWER_MAX);
			rigid.velocity = CalcManager.mCalcVector3;
			DivingTossStandbyMotion(ConvertLocalVec(CalcManager.mCalcVector3).x > 0f);
			LookTarget(BeachVolley_Define.BM.GetBallDropPrediPosGround());
			actionInterval *= 0.5f;
			actionState = ActionState.DIVING_ATTACK;
		}
	}
	public void AiAttack()
	{
	}
	protected void StateAttackAction()
	{
		actionInterval -= Time.deltaTime;
		if (actionInterval <= 0f)
		{
			ChangeFreezeState(actionTimeData.freezeTimeToss);
		}
	}
	protected void StateStayAction()
	{
		actionInterval -= Time.deltaTime;
		if (actionInterval <= 0f)
		{
			movePoslist.Clear();
			ChangeStandardState();
		}
	}
	public void AiSpike()
	{
	}
	private void StateSpikeAfter()
	{
		spikeAfterTime -= Time.deltaTime;
		if (spikeAfterTime <= 0f)
		{
			SettingLayer(BeachVolley_Define.LAYER_CHARACTER);
			ChangeFreezeState(0.75f);
		}
	}
	public void TossAction(float _gaugeValue)
	{
		if (CheckActionState(ActionState.STANDARD) && (BeachVolley_Define.BM.GetBallState() != BeachVolley_BallManager.BallState.KEEP || BeachVolley_Define.MGM.GetGameState() != BeachVolley_MainGameManager.GameState.SERVE))
		{
			gaugeValue = 1f;
			actionInterval = actionTimeData.tossTime;
			if (CalcManager.Length(BeachVolley_Define.BM.GetBallDropPrediPos(), GetPos()) >= GetCharaBodySize() * DIVING_DISTANCE_CORR && CalcManager.Length(BeachVolley_Define.BM.GetBallDropPrediPosGround(), GetPos()) >= GetCharaBodySize() * DIVING_DISTANCE_CORR)
			{
				CalcManager.mCalcVector3 = ((BeachVolley_Define.BM.GetBallDropPrediPosGround() - GetPos()).normalized + Vector3.up * 0.2f) * Mathf.Min((BeachVolley_Define.BM.GetBallDropPrediPosGround() - GetPos()).magnitude * DIVING_POWER, DIVING_POWER_MAX);
				UnityEngine.Debug.Log("ダイビング力 = " + ((BeachVolley_Define.BM.GetBallDropPrediPosGround() - GetPos()).magnitude * DIVING_POWER * 3f).ToString());
				rigid.velocity = CalcManager.mCalcVector3;
				DivingTossStandbyMotion(ConvertLocalVec(CalcManager.mCalcVector3).x > 0f);
				actionInterval *= 0.5f;
				actionState = ActionState.DIVING_TOSS;
				isOverToss = false;
			}
			else if (BeachVolley_Define.FM.CheckInFrontZone(this) && !CheckPositionType(BeachVolley_Define.PositionType.LIBERO) && BeachVolley_Define.Ball.GetLastHitChara().teamNo == teamNo)
			{
				CalcManager.mCalcVector3 = ((BeachVolley_Define.BM.GetBallDropPrediPosGround() - GetPos()).normalized + Vector3.up * 0.2f) * Mathf.Min((BeachVolley_Define.BM.GetBallDropPrediPosGround() - GetPos()).magnitude * 2.5f, DIVING_POWER_MAX);
				rigid.velocity = CalcManager.mCalcVector3;
				TossStandbyMotion();
				isOverToss = true;
				actionState = ActionState.TOSS;
				UnityEngine.Debug.Log("謎のトス");
			}
			else
			{
				CalcManager.mCalcVector3 = ((BeachVolley_Define.BM.GetBallDropPrediPosGround() - GetPos()).normalized + Vector3.up * 0.2f) * Mathf.Min((BeachVolley_Define.BM.GetBallDropPrediPosGround() - GetPos()).magnitude * 2.7f, DIVING_POWER_MAX);
				UnityEngine.Debug.Log("アンダ\u30fcトス");
				UnderTossMotion();
				isOverToss = false;
				actionState = ActionState.TOSS;
			}
			SetAction(AiToss, _immediate: true, _forcibly: true);
		}
	}
	public void MiniGameTossAction(float _gaugeValue)
	{
	}
	protected void StateTossAction()
	{
		actionInterval -= Time.deltaTime;
		if (actionInterval <= 0f)
		{
			ChangeFreezeState(actionTimeData.freezeTimeToss);
		}
	}
	protected void StateDivingTossAction()
	{
		actionInterval -= Time.deltaTime;
		if (actionInterval <= 0f)
		{
			ChangeFreezeState(actionTimeData.freezeTimeToss);
		}
	}
	public void AiToss()
	{
	}
	public void AiMoveBlockPosition()
	{
		FirstActionCall();
		movePoslist[0] = GetFormationPos();
		if (!MoveTarget(Mathf.Min((movePoslist[0] - GetPos()).magnitude, SYSTEM_MOVE_SPEED), STOP_CHECK_DISTANCE))
		{
			LookForward();
		}
	}
	public void SetActionInterval(float _actionInterval)
	{
		actionInterval = _actionInterval;
	}
	public void JumpAction(float _gaugeValue, int _playerNo)
	{
		if (CheckActionState(ActionState.STANDARD))
		{
			actionState = ActionState.JUMP;
			blockJumpPlayer = _playerNo;
			BlockMotion();
			CharaStop();
			LookForward();
			gaugeValue = _gaugeValue;
			rigid.AddForce(base.transform.up * (1f + (float)GetCharaParam().jump * 0.05f) * JUMP_POWER_BLOCK, ForceMode.Impulse);
			actionInterval = 0.8f;
			SetAction(AiJump, _immediate: true, _forcibly: true);
		}
	}
	public void AiJump()
	{
	}
	public void AiHandleBall()
	{
		if (FirstActionCall())
		{
			if (BeachVolley_Define.Ball.GetLastHitChara().teamNo != teamNo || (!BeachVolley_Define.FM.CheckInCourt(BeachVolley_Define.BM.GetBallDropPrediPos()) && !BeachVolley_Define.MCM.CheckLastTouch()))
			{
				isAttack = false;
			}
			else
			{
				isAttack = (CpuAi.CheckTwoAttackPer(teamNo) || BeachVolley_Define.MCM.CheckLastTouch());
			}
			if (isAttack)
			{
				if (CpuAi.CheckSpikeDelayPer(teamNo))
				{
					if (CpuAi.CheckSpikeMissPer(teamNo))
					{
						actionInterval = CpuAi.GetSpikeDelayMissTime(teamNo);
					}
					else
					{
						actionInterval = CpuAi.GetSpikeDelayTime(teamNo);
					}
				}
				else
				{
					actionInterval = 0f;
				}
			}
			else if (CpuAi.CheckRecieveDelayPer(teamNo, BeachVolley_Define.Ball.GetLastHitChara().teamNo != teamNo, BeachVolley_Define.Ball.IsServeBall))
			{
				if (CpuAi.CheckRecieveMissPer(teamNo, BeachVolley_Define.Ball.GetLastHitChara().teamNo != teamNo, BeachVolley_Define.Ball.IsServeBall))
				{
					actionInterval = CpuAi.GetRecieveDelayMissTime(teamNo);
				}
				else
				{
					actionInterval = CpuAi.GetRecieveDelayTime(teamNo);
				}
			}
			else
			{
				actionInterval = 0f;
			}
		}
		if (BeachVolley_Define.Ball.IsPassingOutAntenna || (BeachVolley_Define.Ball.IsServeBall && BeachVolley_Define.MGM.GetSetPlayTeamNo() == teamNo) || (BeachVolley_Define.MCM.BallControllTeam == teamNo && BeachVolley_Define.MCM.CheckLastTouched()))
		{
			return;
		}
		if (CheckMoveBallDropPorediPos())
		{
			MoveBallDropPorediPos();
		}
		if (!BeachVolley_Define.FM.CheckInCourt(BeachVolley_Define.BM.GetBallDropPrediPos(), teamNo, 0f) && BeachVolley_Define.Ball.GetLastHitChara().teamNo != teamNo)
		{
			return;
		}
		if (isAttack)
		{
			actionInterval -= Time.deltaTime;
			if (actionInterval <= 0f && BeachVolley_Define.Ball.GetRigid().velocity.y < 0f && BeachVolley_Define.BM.GetBallPos(_offset: false, _local: true).y < GetStatusData().jumpHeight + GetCharaHeight() + BeachVolley_Define.Ball.BallSize() * 1.5f && CalcManager.Length(GetPos(), BeachVolley_Define.BM.GetBallPos()) <= NEAR_DISTANCE)
			{
				if (CheckPositionType(BeachVolley_Define.PositionType.LIBERO) && !BeachVolley_Define.MCM.CheckLastTouch())
				{
					ChoiceTossTarget();
					gaugeValue = 1f;
					TossAction(gaugeValue);
				}
				else if (CpuAi.CheckSpikeOverPer(teamNo))
				{
					UnityEngine.Debug.Log("オ\u30fcバ\u30fc");
					AttackAction(1f);
				}
				else
				{
					AttackAction(1.1f * CpuAi.GetSpikePower(teamNo));
				}
			}
		}
		else
		{
			actionInterval -= Time.deltaTime;
			if (actionInterval <= 0f && BeachVolley_Define.Ball.GetRigid().velocity.y < 0f && BeachVolley_Define.BM.GetBallPos(_offset: false, _local: true).y < GetCharaHeight() + BeachVolley_Define.Ball.BallSize())
			{
				ChoiceTossTarget();
				gaugeValue = CpuAi.GetTossPower(teamNo);
				TossAction(gaugeValue);
			}
		}
	}
	private void ChoiceTossTarget()
	{
		if (CpuAi.CheckBackAttackPer(teamNo))
		{
			if (CheckPositionState(PositionState.BACK_ZONE))
			{
				targetChara = BeachVolley_Define.MCM.GetBackZoneChara(teamNo, this)[UnityEngine.Random.Range(0, BeachVolley_Define.Return_team_infield_num() / 2 - 1)];
			}
			else
			{
				targetChara = BeachVolley_Define.MCM.GetBackZoneChara(teamNo, this)[UnityEngine.Random.Range(0, BeachVolley_Define.Return_team_infield_num() / 2)];
			}
		}
		else if (CheckPositionState(PositionState.FRONT_ZONE))
		{
			targetChara = BeachVolley_Define.MCM.GetFrontZoneChara(teamNo, this)[UnityEngine.Random.Range(0, BeachVolley_Define.Return_team_infield_num() / 2 - 1)];
		}
		else
		{
			targetChara = BeachVolley_Define.MCM.GetFrontZoneChara(teamNo, this)[UnityEngine.Random.Range(0, BeachVolley_Define.Return_team_infield_num() / 2)];
		}
	}
	public void AiBlockStandby()
	{
		if (FirstActionCall())
		{
			if (CpuAi.CheckBlockDelayPer(teamNo))
			{
				if (CpuAi.CheckBlockMissPer(teamNo))
				{
					actionInterval = CpuAi.GetBlockDelayMissTime(teamNo);
				}
				else
				{
					actionInterval = CpuAi.GetBlockDelayTime(teamNo);
				}
			}
			else
			{
				actionInterval = 0f;
			}
		}
		if (BeachVolley_Define.Ball.IsServeBall && BeachVolley_Define.MGM.GetSetPlayTeamNo() == teamNo)
		{
			return;
		}
		MoveBlockingPos();
		if (BeachVolley_Define.MCM.CheckCharacterState(1 - teamNo, ActionState.SPIKE))
		{
			actionInterval -= Time.deltaTime;
			if (actionInterval <= 0f && BeachVolley_Define.FM.CheckInFrontZone(this))
			{
				delay = UnityEngine.Random.Range(0f, 0.1f);
			}
		}
	}
	public bool CheckAiAction(AiActionMethod _action, int _index = 0)
	{
		if (aiActionMethod.Count > 0)
		{
			return _action == aiActionMethod[_index];
		}
		return false;
	}
	protected bool CheckCharacterFront(Vector3 _dir, float _checkDistance, bool _avoidBall = false)
	{
		int layer = obj.gameObject.layer;
		SettingLayer(BeachVolley_Define.LAYER_NO_HIT);
		if (debugShowData.moveTargetPos)
		{
			UnityEngine.Debug.DrawRay(base.transform.position + Vector3.up * GetCharaHeight() * 0.5f, _dir, ColorPalet.purple, _checkDistance);
		}
		if (Physics.CapsuleCast(base.transform.position + Vector3.up * GetCharaBodySize(), base.transform.position + Vector3.up * GetCharaHeight() * 2f, GetCharaBodySize() * 0.75f, _dir, out raycastHit, _checkDistance, LayerMask.GetMask(BeachVolley_Define.LAYER_CHARACTER, BeachVolley_Define.LAYER_INVISIBLE_CHARA)))
		{
			obj.gameObject.layer = layer;
			if (raycastHit.collider.tag == BeachVolley_Define.TAG_CHARACTER)
			{
				frontCharacter = raycastHit.collider.transform.parent.GetComponent<BeachVolley_Character>();
				return true;
			}
			return false;
		}
		obj.gameObject.layer = layer;
		return false;
	}
	protected bool CheckChangeTargetPos(ref Vector3 _movePos, Vector3 _endPos, bool _avoidBall = false)
	{
		Vector3 vector = _movePos - GetPos();
		if (CheckCharacterFront(vector, Mathf.Max(vector.magnitude, GetCharaBodySize() * 3f), _avoidBall))
		{
			if (debugShowData.moveTargetPos)
			{
				UnityEngine.Debug.Log("前方に選手検知(" + teamNo.ToString() + "," + charaNo.ToString() + ") : チ\u30fcム" + frontCharacter.teamNo.ToString() + "No." + frontCharacter.charaNo.ToString());
			}
			float num = CalcManager.Length(frontCharacter.GetPos(), GetPos());
			Vector3 vector2 = CalcManager.PosRotation2D(frontCharacter.GetMoveVec(), Vector3.zero, CalcManager.Rot(GetMoveVec(), CalcManager.AXIS.Y), CalcManager.AXIS.Y);
			if (frontCharacter.GetMoveVec().magnitude >= 0.1f)
			{
				if (vector2.x > 0f)
				{
					if (num <= GetCharaBodySize() * 3f)
					{
						vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, -90f, CalcManager.AXIS.Y);
						if (!debugShowData.moveTargetPos)
						{
						}
					}
					else
					{
						vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, 0f - AVOID_ROT, CalcManager.AXIS.Y);
						if (!debugShowData.moveTargetPos)
						{
						}
					}
				}
				else if (num <= GetCharaBodySize() * 3f)
				{
					vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, 90f, CalcManager.AXIS.Y);
					if (!debugShowData.moveTargetPos)
					{
					}
				}
				else
				{
					vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, AVOID_ROT, CalcManager.AXIS.Y);
					if (!debugShowData.moveTargetPos)
					{
					}
				}
			}
			else if (ConvertLocalPos(frontCharacter.GetPos()).x > 0f)
			{
				if (num <= GetCharaBodySize() * 1.5f)
				{
					vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, -90f, CalcManager.AXIS.Y);
					if (!debugShowData.moveTargetPos)
					{
					}
				}
				else
				{
					vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, 0f - AVOID_ROT, CalcManager.AXIS.Y);
					if (!debugShowData.moveTargetPos)
					{
					}
				}
			}
			else if (num <= GetCharaBodySize() * 1.5f)
			{
				vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, 90f, CalcManager.AXIS.Y);
				if (!debugShowData.moveTargetPos)
				{
				}
			}
			else
			{
				vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, AVOID_ROT, CalcManager.AXIS.Y);
				bool moveTargetPo = debugShowData.moveTargetPos;
			}
			_movePos = GetPos() + vector.normalized * Mathf.Max(vector.magnitude, GetCharaBodySize() * 4f);
			return true;
		}
		return false;
	}
	public bool CheckAir()
	{
		return GetPos(_isLocal: true).y >= GetCharaHeight() * 0.15f;
	}
	protected void LookForward(bool _isImmediately = false)
	{
		if (teamNo == 0)
		{
			if (_isImmediately)
			{
				base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
			else
			{
				rigid.MoveRotation(Quaternion.Euler(0f, 0f, 0f));
			}
		}
		else if (_isImmediately)
		{
			base.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		}
		else
		{
			rigid.MoveRotation(Quaternion.Euler(0f, 180f, 0f));
		}
	}
	protected void LookCursorDir()
	{
		MCM.GetCursor(playerNo).transform.parent = base.transform.parent;
		base.transform.parent = MCM.GetCursor(playerNo).transform;
		base.transform.SetLocalEulerAnglesY(0f);
		base.transform.parent = MCM.GetCursor(playerNo).transform.parent;
		base.transform.SetLocalScale(1f, 1f, 1f);
		MCM.GetCursor(playerNo).transform.parent = base.transform;
		MCM.GetCursor(playerNo).transform.SetLocalEulerAnglesY(0f);
		MCM.GetCursor(playerNo).transform.SetLocalScale(1f, 1f, 1f);
	}
	protected void LookBall(bool _immediate = false, bool _ballPosUpdate = false)
	{
		if (_ballPosUpdate)
		{
			lookBallPos = BM.GetBallPos() + Ball.GetMoveVec();
		}
		rot.x = 0f;
		rot.y = CalcManager.Rot(GetLookBallPos() - GetPos(), CalcManager.AXIS.Y);
		rot.z = 0f;
		if (_immediate)
		{
			rigid.MoveRotation(Quaternion.Euler(rot));
		}
		else
		{
			rigid.MoveRotation(Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(rot), 20f * Time.deltaTime));
		}
	}
	protected void LookTarget(Vector3 _pos, bool _immediate = false)
	{
		rot.x = 0f;
		rot.y = CalcManager.Rot(_pos - GetPos(), CalcManager.AXIS.Y);
		rot.z = 0f;
		if (!float.IsNaN(rot.y))
		{
			if (_immediate)
			{
				rigid.MoveRotation(Quaternion.Euler(rot));
			}
			else
			{
				rigid.MoveRotation(Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(rot), 20f * Time.deltaTime));
			}
		}
	}
	public void ShowCharacter(bool _show = true)
	{
		base.gameObject.SetActive(_show);
	}
	public bool IsShow()
	{
		return base.gameObject.activeSelf;
	}
	public void SettingLayer(string _layer)
	{
		obj.gameObject.layer = BeachVolley_Define.ConvertLayerNo(_layer);
	}
	public bool CheckLayer(string _layer)
	{
		return obj.gameObject.layer == BeachVolley_Define.ConvertLayerNo(_layer);
	}
	protected void CharaStop()
	{
		rigid.isKinematic = true;
		rigid.velocity = CalcManager.mVector3Zero;
		rigid.angularVelocity = CalcManager.mVector3Zero;
		rigid.isKinematic = false;
	}
	public Vector3 ConvertLocalPos(Vector3 _pos)
	{
		return base.transform.InverseTransformPoint(_pos);
	}
	public Vector3 ConvertLocalVec(Vector3 _vec)
	{
		if (teamNo == 1)
		{
			_vec.x *= -1f;
			_vec.z *= -1f;
		}
		return _vec;
	}
	public Vector3 ConvertWordVec(Vector3 _vec)
	{
		if (teamNo == 1)
		{
			_vec.x *= -1f;
			_vec.z *= -1f;
		}
		return _vec;
	}
	private void OnCollisionEnter(Collision _col)
	{
		if (!SingletonCustom<BeachVolley_MainGameManager>.Instance.CheckInPlayOrEndWait() || !(_col.transform.tag == BeachVolley_Define.TAG_BALL) || BeachVolley_Define.MGM.GetGameState() == BeachVolley_MainGameManager.GameState.SERVE)
		{
			return;
		}
		if (BeachVolley_Define.Ball.GetLastHitChara() == null)
		{
			UnityEngine.Debug.Log("最後に当たったキャラがいない");
			return;
		}
		if (this == BeachVolley_Define.Ball.GetLastHitChara())
		{
			UnityEngine.Debug.Log("最後に当たったキャラ");
			if (BeachVolley_Define.Ball.GetLastHitTime() > 0.3f)
			{
				BeachVolley_Define.MGM.RuleViolation(BeachVolley_MainGameManager.RuleViolationType.DOUBLE_CONTACT, BeachVolley_Define.Ball.GetLastHitChara().TeamNo);
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_beachvolley_double_contact");
			}
			return;
		}
		UnityEngine.Debug.Log("ボ\u30fcル当たったキャラのactionState " + actionState.ToString());
		if (actionState == ActionState.TOSS || actionState == ActionState.DIVING_TOSS || actionState == ActionState.ATTACK || actionState == ActionState.DIVING_ATTACK || actionState == ActionState.SPIKE || !IsPlayer)
		{
			_DelayCantactBall();
			return;
		}
		UnityEngine.Debug.Log("ボ\u30fcル一時停止");
		BeachVolley_Define.BM.GetBall().GetRigid().constraints = RigidbodyConstraints.FreezeAll;
		BeachVolley_Define.BM.GetBall().transform.SetLocalPositionY(BeachVolley_Define.BM.GetBall().transform.localPosition.y - 0.1f);
		isDelayInput = true;
		delayCantactBallMethod = null;
		delayCantactBallMethod = DelayCantactBall(0.1f);
		StartCoroutine(delayCantactBallMethod);
	}
	private IEnumerator DelayCantactBall(float _delay)
	{
		yield return new WaitForSeconds(_delay);
		_DelayCantactBall();
	}
	private void _DelayCantactBall()
	{
		isDelayInput = false;
		UnityEngine.Debug.Log("ボ\u30fcルが動くように");
		BeachVolley_Define.BM.GetBall().GetRigid().constraints = RigidbodyConstraints.None;
		ContactBall();
		BeachVolley_Define.BM.BoundFlgReset();
		BeachVolley_Define.MCM.AutoPlayChangeControlleChara(this);
	}
	private void OnTriggerStay(Collider _col)
	{
		if (_col.transform.tag == BeachVolley_Define.LAYER_CHARACTER)
		{
			characterPos = _col.transform.position;
		}
	}
	private void OnTriggerEnter(Collider _col)
	{
		if (!(_col.transform.tag == BeachVolley_Define.TAG_GOAL))
		{
			return;
		}
		UnityEngine.Debug.Log("AirFieldと当たった");
		if (!(rigid.velocity.y < -0.1f))
		{
			return;
		}
		switch (actionState)
		{
		case ActionState.JUMP:
			ResetObjPosition();
			if (SingletonCustom<BeachVolley_MainGameManager>.Instance.CheckInPlayOrEndWait())
			{
				ChangeFreezeState(actionTimeData.freezeTimeLanding);
			}
			break;
		case ActionState.SPIKE:
			ResetObjPosition();
			if (SingletonCustom<BeachVolley_MainGameManager>.Instance.CheckInPlayOrEndWait())
			{
				ChangeFreezeState(actionTimeData.freezeTimeLanding);
			}
			break;
		case ActionState.SPIKE_AFTER:
			ResetObjPosition();
			if (CheckLayer(BeachVolley_Define.LAYER_CHARA_IGNORE_BALL))
			{
				SettingLayer(BeachVolley_Define.LAYER_CHARACTER);
			}
			break;
		}
	}
	protected virtual void ContactBall(bool _isLate = false)
	{
		UnityEngine.Debug.Log("ボ\u30fcルに触った：" + base.gameObject.name);
		if (BeachVolley_Define.Ball.GetLastHitChara() == null)
		{
			UnityEngine.Debug.Log("最後に当たったキャラがいない");
			return;
		}
		if (this == BeachVolley_Define.Ball.GetLastHitChara())
		{
			UnityEngine.Debug.Log("最後に当たったキャラ");
			if (BeachVolley_Define.Ball.GetLastHitTime() > 0.3f)
			{
				BeachVolley_Define.MGM.RuleViolation(BeachVolley_MainGameManager.RuleViolationType.DOUBLE_CONTACT, BeachVolley_Define.Ball.GetLastHitChara().TeamNo);
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_beachvolley_double_contact");
			}
			return;
		}
		BeachVolley_Define.BM.PlusTouchCount();
		BeachVolley_Define.BM.ResetGravity();
		if (BeachVolley_Define.Ball.IsPassingOutAntenna)
		{
			BeachVolley_Define.MGM.RuleViolation(BeachVolley_MainGameManager.RuleViolationType.OUT_OF_BOUNDS, BeachVolley_Define.Ball.GetLastHitChara().TeamNo);
			return;
		}
		if (BeachVolley_Define.Ball.IsServeBall && BeachVolley_Define.Ball.GetLastHitChara().teamNo == teamNo)
		{
			BeachVolley_Define.MGM.ServeMiss();
			return;
		}
		BeachVolley_Define.Ball.IsServeBall = false;
		if (BeachVolley_Define.Ball.GetLastHitChara().teamNo != teamNo)
		{
			if (CheckActionState(ActionState.JUMP))
			{
				BeachVolley_Define.MCM.BallTouchCnt = 1;
			}
			else
			{
				BeachVolley_Define.MCM.BallTouchCnt = 1;
			}
		}
		else
		{
			if (BeachVolley_Define.MCM.CheckLastTouched())
			{
				BeachVolley_Define.MGM.RuleViolation(BeachVolley_MainGameManager.RuleViolationType.FOUR_HITS, BeachVolley_Define.Ball.GetLastHitChara().TeamNo);
				return;
			}
			BeachVolley_Define.MCM.BallTouchCnt++;
		}
		BeachVolley_Define.GUM.SetLimitPassCnt(teamNo, Mathf.Max(BeachVolley_Define.MCM.BALL_TOUCH_LIMIT - BeachVolley_Define.MCM.BallTouchCnt, 0), playerNo);
		if (CheckActionState(ActionState.FREEZE))
		{
			UnityEngine.Debug.Log("フリ\u30fcズ中");
			if (CheckPositionType(BeachVolley_Define.PositionType.LIBERO))
			{
				if (BeachVolley_Define.MCM.CheckLastTouched())
				{
					AttackAction(0f, _canNotSpike: true);
				}
				else
				{
					ChoiceTossTarget();
					Toss(_isNice: true);
				}
			}
			else
			{
				MissTouch();
			}
		}
		else if (CheckActionState(ActionState.SPIKE))
		{
			UnityEngine.Debug.Log("スパイクの判定");
			if (BeachVolley_Define.BM.GetBallPos(_offset: false).y < BeachVolley_Define.FM.GetFieldData().GetNetTopPos().y + BeachVolley_Define.BM.GetBallSize() * 0.5f)
			{
				if (BeachVolley_Define.MCM.CheckLastTouched())
				{
					Spike();
					UnityEngine.Debug.Log("スパイク");
				}
				else
				{
					gaugeValue = 1f;
					Toss();
					UnityEngine.Debug.Log("とっす");
				}
			}
			else
			{
				Spike();
			}
		}
		else if (CheckActionState(ActionState.DIVING_ATTACK))
		{
			UnityEngine.Debug.Log("ダイビングアタック中");
			Attack();
		}
		else if (CheckActionState(ActionState.TOSS) || CheckActionState(ActionState.DIVING_TOSS))
		{
			UnityEngine.Debug.Log("トス、ダイビングトス中");
			if (CheckKnockBackChara())
			{
				MissTouch();
			}
			else if (BeachVolley_Define.MCM.CheckLastTouched())
			{
				Attack();
			}
			else
			{
				Toss(_isNice: true);
			}
		}
		else if (CheckActionState(ActionState.ATTACK))
		{
			UnityEngine.Debug.Log("アタック");
			Attack();
		}
		else if (CheckActionState(ActionState.JUMP))
		{
			UnityEngine.Debug.Log("ブロック中");
			bool flag = false;
			bool flag2 = false;
			SingletonCustom<HidVibration>.Instance.SetCustomVibration(blockJumpPlayer, HidVibration.VibrationType.Normal, 0.1f);
			float num = GetStatusData().blockMissPer + BeachVolley_Define.Ball.GetLastHitChara().GetStatusData().blockMissAddPer;
			float num2 = GetStatusData().blockNicePer - BeachVolley_Define.Ball.GetLastHitChara().GetStatusData().blockNiceReducePer;
			if (playerNo >= 0 && !IsPlayer)
			{
				num *= 0.5f;
				num2 *= 1.5f;
			}
			flag = CalcManager.IsPerCheck(num);
			flag2 = CalcManager.IsPerCheck(num2);
			UnityEngine.Debug.Log("ブロック : _isNice(" + num.ToString() + ") = " + flag.ToString() + " : _isMiss(" + num2.ToString() + ") = " + flag2.ToString());
			Block(flag, flag2);
		}
		else
		{
			UnityEngine.Debug.Log("その他判定");
			if (CheckPositionType(BeachVolley_Define.PositionType.LIBERO))
			{
				if (BeachVolley_Define.MCM.CheckLastTouched())
				{
					AttackAction(0f, _canNotSpike: true);
				}
				else
				{
					if (playerNo >= 0 && !IsPlayer)
					{
						ChoiceTossTarget();
					}
					Toss(_isNice: true);
				}
			}
			else
			{
				MissTouch();
			}
		}
		BeachVolley_Define.Ball.SetLastHitChara(this);
	}
	protected virtual void CharaKnockBack()
	{
		CalcManager.mCalcVector3 = ((GetPos() - Ball.transform.position).normalized * 1.5f + Ball.GetMoveVec().normalized).normalized * 10f;
		CalcManager.mCalcVector3.y = rigid.velocity.y;
		rigid.velocity = CalcManager.mCalcVector3;
		obj.SetLocalPosition(0f, 0.11f, 0.374f);
		obj.SetLocalEulerAngles(-45f, 0f, 0f);
		rigid.constraints = (RigidbodyConstraints)96;
		breakEffect.Play();
		SingletonCustom<AudioManager>.Instance.SePlay("se_chara_break");
		ChangeFreezeState(actionTimeData.freezeTimeKnockBack);
	}
	protected virtual void MiniGameCharaKnockBack(BeachVolley_Ball _ball)
	{
		CalcManager.mCalcVector3 = ((GetPos() - _ball.transform.position).normalized * 1.5f + _ball.GetMoveVec().normalized).normalized * 10f;
		CalcManager.mCalcVector3.y = rigid.velocity.y;
		rigid.velocity = CalcManager.mCalcVector3;
		obj.SetLocalPosition(0f, 0.11f, 0.374f);
		obj.SetLocalEulerAngles(-45f, 0f, 0f);
		rigid.constraints = (RigidbodyConstraints)96;
		breakEffect.Play();
		SingletonCustom<AudioManager>.Instance.SePlay("se_chara_break");
		ChangeFreezeState(0.7f);
	}
	protected void UpdateDebugActionState()
	{
		if (aiActionMethod != null && aiActionMethod.Count > 0 && aiActionMethod[aiActionMethod.Count - 1] != null)
		{
			debugActionState = aiActionMethod[aiActionMethod.Count - 1].Method.Name;
		}
		else
		{
			debugActionState = "行動が設定されていません : チ\u30fcム " + teamNo.ToString() + " : No." + charaNo.ToString();
		}
	}
	private void OnDrawGizmos()
	{
		if (!(rigid == null) && debugShowData.moveTargetPos && movePoslist.Count > 0)
		{
			for (int i = 0; i < movePoslist.Count; i++)
			{
				Gizmos.color = new Color(1f / (float)movePoslist.Count * (float)i, 1f / (float)movePoslist.Count * (float)i, 1f / (float)movePoslist.Count * (float)i);
				Gizmos.DrawWireSphere(movePoslist[i], 0.5f);
			}
		}
	}
}
