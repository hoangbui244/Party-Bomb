using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Utility;
public class MonsterRace_CarScript : MonoBehaviour
{
	private const float MAX_SPEED = 4f;
	private const float MAX_BACK_SPEED = 2f;
	private const float ACCEL_SPEED = 2f;
	private const float ACCEL_BACK_SPEED = 2f;
	private const float BRAKE_SPEED = 2f;
	private const float DIRT_SPEED_MAG = 0.8f;
	private const float GRIP_ANGLE_SPEED = 3f;
	private const float MAX_STEER_ANGLE = 110f;
	private const float WALL_RESIST_MAX = 0.02f;
	private const float WATER_RESIST_MAX = 0.009f;
	private const float RESIST_BASE = 0.94f;
	private const float RESIST_DASH = 0.955f;
	private const float WATER_ADD = 0.2f;
	private const float WATER_MINUS = 0.3f;
	private const float WATER_SPEED_UP = 0.5f;
	private const float DASH_ADD_MAX_SPEED = 3.2f;
	private const float DASH_ACCEL_SPEED = 3.2f;
	private const float DASH_TIME = 2f;
	private const float DASH_AFTER_TIME = 1f;
	private const float DIRT_SPEED_CORR = 0.3f;
	private const float AI_RETURN_TIME = 10f;
	private const float JUMP_POWER = 3.2f;
	private float AIR_CHECK_TIME = 0.1f;
	private const float GRAVITY = 9.8f;
	private const float MAX_ANGLE_X = 85f;
	private const float WHEEL_DIAMETER = 0.2f;
	private const float WHEEL_ADD_HEIGHT = 0.03f;
	private const float ROUGH_SHAKE_INTERVAL = 0.2f;
	private const float NORMAL_SHAKE_INTERVAL = 0.5f;
	public const float WIND_MAX_POWER = 6f;
	public const float WIND_MINUS_POWER_ONE = 2f;
	public const float WIND_ORIGIN_LOCAL_POS_Z = -4f;
	private const float CAMERA_ROT_SPEED = 1.5f;
	private const int CAMERA_MOST_NEAR_NO = 2;
	private const float CAMERA_MOST_NEAR_ROT_SPEED = 4.5f;
	private const string TAG_FLOOR = "Floor";
	public const string TAG_WALL = "Respawn";
	private const string TAG_FIELD = "Field";
	private const string TAG_OBJECT = "Object";
	private const string TAG_STEP = "CharaWall";
	private const string TAG_ROUGH = "GoalNet";
	private const string TAG_DIRT = "School";
	public const string TAG_WIND = "Finish";
	private const string TAG_DASH = "Coin";
	private const string TAG_CART_BALL = "Character";
	private const string TAG_BALL = "Ball";
	public const string TAG_PLAYER = "Player";
	private const string TAG_BALANCEBEAM = "GoalHoop";
	private const string TAG_GOAL = "Goal";
	private const string TAG_TUNNEL = "GoalBarrier";
	public const string TAG_DIRT_CHECK_WALL = "VerticalWall";
	public const string TAG_CAMERA_CHECK_AREA = "HorizontalWall";
	private const string TAG_NOT_SMOKE_AREA = "PenaltyArea";
	private const string TAG_WATER = "Water";
	private const string TAG_BARREL = "Ball";
	private const string TAG_AUDIENCE_AREA = "CheckPoint";
	private const string TAG_AI_CHECK_POINT = "EditorOnly";
	private const string TAG_AI_CHECK_POINT_BACK = "GameController";
	private const string TAG_AI_JUMP = "Airplane";
	private const string TAG_AI_BRAKE = "NoHit";
	public const int LAYER_GROUND = 8388608;
	public const int LAYER_WATER = 16;
	private const int LAYER_AI_DODGE = 67108864;
	private const int LAYER_NO_PLAYER_1 = 8;
	private const int LAYER_NO_PLAYER_2 = 9;
	private const int LAYER_NO_PLAYER_3 = 10;
	private const int LAYER_NO_PLAYER_4 = 11;
	public const int LAYER_NO_CHARA_1 = 28;
	public const int LAYER_NO_CHARA_2 = 29;
	public const int LAYER_NO_CHARA_3 = 31;
	public const int LAYER_NO_CHARA_4 = 27;
	private static readonly float BODY_ANGLE_ROT = -10f;
	private static readonly float BODY_ANGLE_ROT_LEFT = 30f;
	private static readonly float BODY_ANGLE_ROT_RIGHT = -30f;
	private static readonly Vector3 BODY_POS = new Vector3(0f, 0.05f, 0f);
	private int playerNo;
	private int playerRealNo = -1;
	private int charaNo;
	private int teamNo;
	protected int styleCharaNo;
	private Rigidbody rigid;
	private Vector3 rot;
	private Vector3 prevPos;
	private Vector3 nowPos;
	private float prevEulerY;
	private float nowEulerY;
	private float nowTiltAngle;
	private float nowRollAngle;
	private float moveDistance;
	private int playSeRunCnt;
	private float hitSeTime;
	private bool isPlayer;
	private bool isRankLock;
	private int nowRank;
	private int nowLap;
	private bool isGoal;
	private float viewTime = -1f;
	private float goalTime = -1f;
	public float nowRunDistance;
	public float maxRunDistance;
	private float wheelDistance;
	private float fixedSteerDir_x;
	private float fixedSteerDir_z;
	private bool isFixedAccel;
	private bool isFixedBack;
	private bool isBack;
	private float nowSteerAngle;
	private float maxSpeedInit = 4f;
	private float maxSteerAngleInit = 110f;
	private float gripAngleSpeedInit = 3f;
	private float slipStreamMag = 1f;
	private float waterGauge;
	private bool isMultiShadow;
	[SerializeField]
	private MonsterRace_CarAudio carAudio;
	[SerializeField]
	private CharacterStyle charaStyle;
	[SerializeField]
	private SkinnedMeshRenderer[] renderers;
	[SerializeField]
	private GameObject wolfObj;
	[SerializeField]
	private GameObject llamaObj;
	[SerializeField]
	private SkinnedMeshRenderer wolfRenderer;
	[SerializeField]
	private SkinnedMeshRenderer llamaRenderer;
	[SerializeField]
	private Animator wolfAnimator;
	[SerializeField]
	private Animator llamaAnimator;
	private Animator animator;
	[SerializeField]
	private Transform llamaCharaParent;
	[SerializeField]
	[Header("スタミナの最大数")]
	private int maxStaminaCount;
	private int staminaCount;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private Collider collider;
	[SerializeField]
	[Header("ホイ\u30fcルコライダ\u30fc")]
	private WheelCollider[] wheelColliders;
	[SerializeField]
	[Header("ホイ\u30fcルと連動して動くオブジェクト")]
	private GameObject[] wheelChainObjs;
	private float wheelAngleX;
	public float[] wheelDefaultChainObjPosesY;
	private Vector2 heavePerlinVec2;
	private float heaveSpeed = 6f;
	private float heaveHeight = 0.03f;
	private bool isHeaveAddForce;
	private Vector3 heaveForceVec = new Vector3(0f, 1f, 0f);
	[SerializeField]
	[Header("着地時の揺れの曲線")]
	private AnimationCurve landingHeaveCurve;
	[SerializeField]
	[Header("本体の傾きの曲線")]
	private AnimationCurve bodyAngleCurve;
	private float landingHeavePower = 0.1f;
	private float landingHeaveLength = 1f;
	[SerializeField]
	[Header("カメラタ\u30fcゲット")]
	private Transform cameraTarget;
	[SerializeField]
	[Header("カメラ視点切り換え用")]
	private Transform[] cameraPoints;
	private int cameraPointNo;
	private int cameraPrevPointNo = -1;
	private bool isCameraPointMoving;
	private bool isCameraPointPrevMoving;
	private float cameraPointMovingLerp;
	private Vector3 cameraPointMovingPrevPos;
	[SerializeField]
	[Header("カメラLookAt位置")]
	private Transform[] cameraLookAtPoints;
	private Transform cameraRotAnchor;
	private float cameraAngleY;
	private float cameraRotSpeed = 1.5f;
	private float cameraSpeedZValue;
	private float cameraHeaveValue;
	private bool isInvincible;
	private float invincibleTime;
	private bool isStrong;
	private float strongTime;
	private bool isDrift;
	private bool isGrounded;
	private Vector3 updateGroundedNormal;
	private float airTime;
	private bool isLandingWait;
	private bool isLandingAfter;
	private float landingAfterTime;
	private bool isEdgeDirt;
	private int dirtColCount;
	private bool isRough;
	private float roughTime;
	private bool isDash;
	private float dashTime = -1f;
	private bool isDashAfter;
	private float dashAfterTime;
	private bool isTunnel;
	private bool isTunnelCameraMove;
	private float tunnelHeight;
	[SerializeField]
	[Header("砂煙エフェクト")]
	private ParticleSystem smokeEffect;
	[SerializeField]
	[Header("入水エフェクト")]
	private ParticleSystem waterEffect;
	private float waterEffectTime = -1f;
	[SerializeField]
	[Header("水中移動エフェクトアンカ\u30fc")]
	private Transform waterMoveEffectAnchor;
	[SerializeField]
	[Header("水中移動エフェクト")]
	private ParticleSystem[] waterMoveEffects;
	[SerializeField]
	[Header("着地エフェクト")]
	private ParticleSystem landingEffect;
	[SerializeField]
	[Header("後ろの水飛沫")]
	private ParticleSystem backJetWaterSplash;
	[SerializeField]
	private float backJetWaterSplashEffectNormalRateMin = 50f;
	[SerializeField]
	private float backJetWaterSplashEffectNormalRateMax = 100f;
	[SerializeField]
	private float baxkJetWaterSplashEffectDriftRate = 150f;
	[SerializeField]
	[Header("高速エフェクト")]
	private ParticleSystem speedEffect;
	[SerializeField]
	[Header("ダッシュ開始エフェクト")]
	private ParticleSystem dashStartEffect;
	[SerializeField]
	[Header("ダッシュ中エフェクト")]
	private ParticleSystem dashMoveEffect;
	[SerializeField]
	[Header("地面角度用アンカ\u30fc")]
	private Transform groundRayForwardAnchor;
	[SerializeField]
	private Transform groundRayBackAnchor;
	[SerializeField]
	[Header("CPU前方確認用アンカ\u30fc")]
	private Transform aiRayAnchor;
	[SerializeField]
	[Header("CPU前方確認用アンカ\u30fc(右)")]
	private Transform aiRayAnchorR;
	[SerializeField]
	[Header("CPU前方確認用アンカ\u30fc(左)")]
	private Transform aiRayAnchorL;
	[SerializeField]
	[Header("順位判定用")]
	private WaypointProgressTracker wptPos;
	[SerializeField]
	[Header("AI操作用")]
	private WaypointProgressTracker wptAi;
	public Vector3 forwa;
	public Vector3 wptdir;
	public Vector3 cross;
	private bool isJump;
	private float jumpTime;
	private float bodyMoveTime;
	private float curveLerp = 0.5f;
	private float bodyAngleRot_x;
	private float streagePos_y;
	private bool isLanding;
	private float splasVelocityTime;
	private float bodyAngleRot_z;
	private float animatorTime = 0.5f;
	private bool isOnNotSmokeArea = true;
	private float onNotSmokeAreaTime;
	private bool isOnWaterArea;
	private float onWaterAreaTime;
	public static readonly float[][] CPU_IS_DASH_INTERVAL = new float[3][]
	{
		new float[5]
		{
			6f,
			6.5f,
			7f,
			7.5f,
			8f
		},
		new float[5]
		{
			4f,
			4.5f,
			5f,
			5.5f,
			6f
		},
		new float[5]
		{
			2f,
			2.5f,
			3f,
			3.5f,
			4f
		}
	};
	private float aiUpdateInterval = -1f;
	private bool isAiStraightRun = true;
	private float aiStraightTime = 2.5f;
	private bool isAiBrake;
	private Vector3 aiTargetPos;
	private float aiTargetUpdateTime = 1f;
	public float aiX;
	public float aiZ;
	public Vector3 aiDir;
	public float aiDis;
	public Vector3 aiCross;
	private float aiStopTime;
	private bool isAiBack;
	private float aiBackTime;
	private float aiDashIntervalTimer;
	private float[] aiDashIntervalTimes;
	private int aiDashIntervalCount;
	public int PlayerNo
	{
		get
		{
			return playerNo;
		}
		set
		{
			playerNo = value;
			if (playerRealNo == -1)
			{
				playerRealNo = playerNo;
			}
		}
	}
	public int PlayerRealNo
	{
		get
		{
			return playerRealNo;
		}
		set
		{
			playerRealNo = value;
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
	public int StyleCharaNo
	{
		get
		{
			return styleCharaNo;
		}
		set
		{
			styleCharaNo = value;
		}
	}
	public bool IsPlayer
	{
		get
		{
			return isPlayer;
		}
		set
		{
			isPlayer = value;
		}
	}
	public bool IsRankLock
	{
		get
		{
			return isRankLock;
		}
		set
		{
			isRankLock = value;
		}
	}
	public int NowRank
	{
		get
		{
			return nowRank;
		}
		set
		{
			nowRank = value;
		}
	}
	public int NowLap
	{
		get
		{
			return nowLap;
		}
		set
		{
			nowLap = value;
		}
	}
	public bool IsGoal
	{
		get
		{
			return isGoal;
		}
		set
		{
			isGoal = value;
		}
	}
	public float ViewTime
	{
		get
		{
			return viewTime;
		}
		set
		{
			viewTime = value;
		}
	}
	public float GoalTime
	{
		get
		{
			return goalTime;
		}
		set
		{
			goalTime = value;
		}
	}
	public bool IsReverseRun
	{
		get
		{
			if (isGoal)
			{
				return false;
			}
			float num = maxRunDistance - nowRunDistance;
			if (num > 8f)
			{
				if (num > 12f)
				{
					maxRunDistance = nowRunDistance + 9f;
				}
				return true;
			}
			return false;
		}
	}
	public float SpeedLerp
	{
		get
		{
			Vector3 velocity = rigid.velocity;
			velocity.y = 0f;
			return Mathf.Clamp01(velocity.magnitude / 4f);
		}
	}
	public int StaminaCount
	{
		get
		{
			return staminaCount;
		}
		set
		{
			staminaCount = value;
		}
	}
	public int MaxStaminaCount => maxStaminaCount;
	public bool IsDrift => isDrift;
	public float DriftPower
	{
		get
		{
			if (!isDrift)
			{
				return 0f;
			}
			return Mathf.Abs(fixedSteerDir_x) * SpeedLerp;
		}
	}
	public bool IsGrounded => isGrounded;
	public bool IsEdgeDirt
	{
		get
		{
			return isEdgeDirt;
		}
		set
		{
			isEdgeDirt = value;
		}
	}
	public float WheelDefaultHeight => 0.03f;
	public float AiUpdateInterval
	{
		get
		{
			return aiUpdateInterval;
		}
		set
		{
			aiUpdateInterval = value;
		}
	}
	public void Init(int _teamNo, int _charaNo)
	{
		rigid = GetComponent<Rigidbody>();
		base.gameObject.name = base.gameObject.name + "_" + _teamNo.ToString() + "_" + _charaNo.ToString();
		charaNo = _charaNo;
		teamNo = _teamNo;
		playerNo = -1;
		if (SingletonCustom<MonsterRace_CarManager>.Instance.PlayerNum == 2)
		{
			cameraLookAtPoints[0].AddLocalPositionY(0.15f);
		}
		wheelDefaultChainObjPosesY = new float[wheelColliders.Length];
		for (int i = 0; i < wheelColliders.Length; i++)
		{
			wheelColliders[i].GetWorldPose(out Vector3 _, out Quaternion _);
		}
		InitTrans();
		InitWpt();
		viewTime = 0f;
		heavePerlinVec2 = new Vector2(UnityEngine.Random.Range(-10000f, 10000f), UnityEngine.Random.Range(-10000f, 10000f));
		if (!isPlayer)
		{
			AiInit();
		}
	}
	public void InitTrans()
	{
		nowPos = (prevPos = base.transform.position);
		nowEulerY = (prevEulerY = base.transform.eulerAngles.y);
		InitCameraTransData();
	}
	public void InitWpt()
	{
		wptPos.setCircuit = SingletonCustom<MonsterRace_CarManager>.Instance.Course.GetPosCircuit();
		if (charaNo < 4)
		{
			if (charaNo > 0)
			{
				wptAi.setCircuit = SingletonCustom<MonsterRace_CarManager>.Instance.Course.GetAiCircuit(charaNo - 1);
			}
			else
			{
				wptAi.setCircuit = SingletonCustom<MonsterRace_CarManager>.Instance.Course.GetAiCircuit(0);
			}
		}
		else if (charaNo == 5)
		{
			wptAi.setCircuit = SingletonCustom<MonsterRace_CarManager>.Instance.Course.GetAiCircuit(0);
		}
		else if (charaNo == 6)
		{
			wptAi.setCircuit = SingletonCustom<MonsterRace_CarManager>.Instance.Course.GetAiCircuit(UnityEngine.Random.Range(0, 4));
		}
		else if (charaNo == 7)
		{
			wptAi.setCircuit = SingletonCustom<MonsterRace_CarManager>.Instance.Course.GetAiCircuit(2);
		}
		else
		{
			wptAi.setCircuit = SingletonCustom<MonsterRace_CarManager>.Instance.Course.GetAiCircuit(0);
		}
	}
	public void InitAudio()
	{
		carAudio.Init();
	}
	public void UpdateMethod()
	{
		animator.SetFloat("Speed", SpeedLerp);
		nowRunDistance = wptPos.runDistance;
		if (maxRunDistance < nowRunDistance)
		{
			maxRunDistance = nowRunDistance;
		}
		if (!SingletonCustom<MonsterRace_GameManager>.Instance.IsGameStart)
		{
			wptPos.progressDistance = 0f;
			wptAi.progressDistance = 0f;
			return;
		}
		CheckGrounded();
		Vector3 velocity = rigid.velocity;
		velocity.y = 0f;
		if (isGrounded)
		{
			if (velocity.magnitude > 1f)
			{
				heavePerlinVec2.x += heaveSpeed * Time.deltaTime;
				float t = Mathf.PerlinNoise(heavePerlinVec2.x, heavePerlinVec2.y);
				cameraHeaveValue = Mathf.Lerp(0f - heaveHeight, heaveHeight, t);
			}
			if (isEdgeDirt && isPlayer && !isGoal && !isDash && !SingletonCustom<CommonNotificationManager>.Instance.IsPause && velocity.sqrMagnitude > 0.25f)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerRealNo);
			}
		}
		else
		{
			airTime += Time.deltaTime;
		}
		if (!isLandingWait && airTime > AIR_CHECK_TIME)
		{
			isLandingWait = true;
			isLandingAfter = false;
		}
		else if (isLandingWait && isGrounded)
		{
			isLandingWait = false;
			isLandingAfter = true;
			isLanding = true;
			airTime = 0f;
			landingAfterTime = 0f;
		}
		else if (isLandingAfter)
		{
			landingAfterTime += Time.deltaTime;
			if (landingAfterTime > landingHeaveLength)
			{
				streagePos_y = 0f;
				isLandingAfter = false;
				isLanding = false;
			}
		}
		if (isDash)
		{
			dashTime -= Time.deltaTime;
			if (dashTime < 0f)
			{
				isDash = false;
				dashMoveEffect.Stop();
				if (isPlayer || SingletonCustom<MonsterRace_CarManager>.Instance.PlayerNum == 3)
				{
					SingletonCustom<MonsterRace_CarManager>.Instance.SpeedLineEnd(charaNo);
				}
				isDashAfter = true;
				dashAfterTime = 1f;
			}
		}
		else if (isDashAfter)
		{
			dashAfterTime -= Time.deltaTime;
			if (dashAfterTime < 0f)
			{
				isDashAfter = false;
			}
		}
		if (isStrong)
		{
			strongTime -= Time.deltaTime;
			if (strongTime < 0f)
			{
				isStrong = false;
			}
		}
		if (isTunnelCameraMove)
		{
			if (isTunnel)
			{
				tunnelHeight -= Time.deltaTime * 3f;
				if (tunnelHeight < -1f)
				{
					tunnelHeight = -1f;
					isTunnelCameraMove = false;
				}
			}
			else
			{
				tunnelHeight += Time.deltaTime * 3f;
				if (tunnelHeight > 0f)
				{
					tunnelHeight = 0f;
					isTunnelCameraMove = false;
				}
			}
		}
		if (isOnNotSmokeArea && Time.time - onNotSmokeAreaTime > 0.1f)
		{
			isOnNotSmokeArea = false;
		}
		if (isOnWaterArea)
		{
			waterMoveEffectAnchor.transform.SetLocalEulerAnglesY(isBack ? 180 : 0);
			if (Time.time - onWaterAreaTime > 0.05f)
			{
				isOnWaterArea = false;
				for (int i = 0; i < waterMoveEffects.Length; i++)
				{
					waterMoveEffects[i].Stop();
				}
			}
		}
		carAudio.UpdateMethod();
	}
	private void FixedUpdate()
	{
		if (!SingletonCustom<MonsterRace_GameManager>.Instance.IsGameStart)
		{
			Vector3 velocity = rigid.velocity;
			velocity.y -= 9.8f * Time.fixedDeltaTime;
			velocity.x = (velocity.z = 0f);
			rigid.velocity = velocity;
			WheelUpdate();
			return;
		}
		prevPos = nowPos;
		nowPos = base.transform.position;
		prevEulerY = nowEulerY;
		nowEulerY = base.transform.eulerAngles.y;
		waterGauge -= 0.3f * Time.fixedDeltaTime;
		if (waterGauge < 0f)
		{
			waterGauge = 0f;
		}
		Vector3 vector = nowPos - prevPos;
		vector.y = 0f;
		moveDistance += vector.magnitude;
		if (moveDistance > (float)playSeRunCnt * 1.7f)
		{
			playSeRunCnt++;
			PlaySeRun();
		}
		FixedMove();
		CartRotX();
		base.transform.SetLocalEulerAnglesZ(0f);
		UpdateCameraTransData();
		UpdateCaterpillarOffset();
	}
	public void SetControlData(float _steerDir_x, float _steerDir_z, bool _isAccel, bool _isBack)
	{
		fixedSteerDir_x = _steerDir_x;
		fixedSteerDir_z = _steerDir_z;
		isFixedAccel = _isAccel;
		isFixedBack = _isBack;
	}
	private void FixedMove()
	{
		Vector3 vector = rigid.velocity;
		vector.y -= 9.8f * Time.fixedDeltaTime;
		isDrift = false;
		Vector3 forward = base.transform.forward;
		if (isDash)
		{
			Vector3 a = base.transform.rotation * Vector3.forward * 3.2f;
			vector += a * Time.fixedDeltaTime;
			float y = vector.y;
			vector.y = 0f;
			float num = maxSpeedInit + 0.5f * waterGauge + 3.2f;
			if (vector.sqrMagnitude > Pow2(num))
			{
				vector = vector.normalized * num;
			}
			vector.y = y;
		}
		bool flag = false;
		if (isFixedAccel)
		{
			Vector3 a2 = base.transform.rotation * Vector3.forward * 2f;
			if (new Vector2(vector.x, vector.z).magnitude < 3f)
			{
				a2 *= 10f;
			}
			if (isDrift)
			{
				a2 *= 0.5f;
			}
			vector += a2 * Time.fixedDeltaTime;
			float y2 = vector.y;
			vector.y = 0f;
			float num2 = maxSpeedInit + 0.5f * waterGauge;
			if (isDash)
			{
				num2 += 3.2f;
			}
			else if (isDashAfter)
			{
				num2 += 3.2f * (dashAfterTime / 1f);
			}
			else if (isEdgeDirt)
			{
				num2 *= 0.8f;
			}
			if (vector.sqrMagnitude > Pow2(num2))
			{
				vector = vector.normalized * num2;
			}
			vector.y = y2;
			flag = true;
		}
		else if (isFixedBack)
		{
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude > 1f && Vector3.Dot(forward, rigid.velocity) > 0f)
			{
				Vector3 a3 = vector.normalized * 2f;
				vector -= a3 * Time.fixedDeltaTime;
			}
			else if (sqrMagnitude < Pow2(2f))
			{
				Vector3 vector2 = base.transform.rotation * Vector3.back * 2f;
				if (sqrMagnitude < Pow2(2f))
				{
					vector2 *= 2f;
					if (sqrMagnitude < 1f)
					{
						vector2 *= 3f;
					}
				}
				if (vector2.y > 0f)
				{
					vector2.y *= 2f;
				}
				vector += vector2 * Time.fixedDeltaTime;
				float y3 = vector.y;
				vector.y = 0f;
				if (vector.sqrMagnitude > Pow2(2f))
				{
					vector = vector.normalized * 2f;
				}
				vector.y = y3;
			}
			flag = true;
		}
		if (!flag)
		{
			if (vector.sqrMagnitude < Pow2(0.1f))
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
		}
		if (!isGrounded || isOnNotSmokeArea)
		{
			smokeEffect.Stop();
			backJetWaterSplash.Stop();
		}
		bool flag2 = Vector3.Dot(forward, vector) < 0f;
		if (isBack)
		{
			if (!flag2)
			{
				isBack = false;
			}
		}
		else if (flag2 && isFixedBack && !isDrift && vector.sqrMagnitude <= Pow2(2f))
		{
			isBack = true;
		}
		if (!isBack && isGrounded && vector.magnitude > 0.5f && !isJump)
		{
			if (!isOnNotSmokeArea && !smokeEffect.isPlaying)
			{
				smokeEffect.Play();
			}
			if (!isOnNotSmokeArea && !backJetWaterSplash.isPlaying)
			{
				backJetWaterSplash.Play();
			}
			ParticleSystem.EmissionModule emission = backJetWaterSplash.emission;
			if (isEdgeDirt || isDrift)
			{
				curveLerp = Mathf.Clamp01(curveLerp);
			}
			else
			{
				curveLerp = Mathf.Clamp(curveLerp, 0.25f, 0.75f);
				bodyMoveTime = bodyAngleCurve.Evaluate(SpeedLerp);
				emission.rateOverTime = Mathf.Lerp(backJetWaterSplashEffectNormalRateMin, backJetWaterSplashEffectNormalRateMax, SpeedLerp);
			}
			if (IsCurveLeft())
			{
				curveLerp += Time.deltaTime;
			}
			else if (IsCurveRight())
			{
				curveLerp -= Time.deltaTime;
			}
			else
			{
				splasVelocityTime = 0f;
			}
		}
		else if (smokeEffect.isPlaying)
		{
			smokeEffect.Stop();
		}
		float magnitude = vector.magnitude;
		if (magnitude > 1f || isFixedAccel || isFixedBack)
		{
			nowSteerAngle = maxSteerAngleInit * fixedSteerDir_x;
			Vector3 up = Vector3.up;
			float value = magnitude / 4f;
			value = Mathf.Clamp01(value);
			value = (2f + value) / 3f;
			float num3 = nowSteerAngle * value;
			float num4 = Mathf.Clamp01(streagePos_y);
			float num7 = num4 / 2f;
			if (isBack)
			{
				num3 *= -1f;
			}
			if (isDrift)
			{
				num3 *= 1.5f;
			}
			if (!isGrounded && isJump)
			{
				num3 *= 0.3f;
			}
			base.transform.Rotate(up, num3 * Time.fixedDeltaTime);
			float num5 = (!isBack || isDrift) ? (Mathf.Atan2(vector.x, vector.z) * 57.29578f) : (Mathf.Atan2(0f - vector.x, 0f - vector.z) * 57.29578f);
			float num6 = base.transform.eulerAngles.y - num5;
			if (num6 < -180f)
			{
				num6 += 360f;
			}
			else if (num6 > 180f)
			{
				num6 -= 360f;
			}
			if (!isFixedAccel)
			{
				num6 *= 1.5f;
			}
			if (isBack && !isDrift)
			{
				num6 *= 3f;
			}
			if (isDrift)
			{
				num6 *= 0.8f;
			}
			vector = Quaternion.Euler(0f, num6 * gripAngleSpeedInit * Time.fixedDeltaTime, 0f) * vector;
		}
		rigid.velocity = vector;
		WheelUpdate();
	}
	private void WheelUpdate()
	{
		for (int i = 0; i < wheelColliders.Length; i++)
		{
			wheelColliders[i].GetWorldPose(out Vector3 _, out Quaternion _);
		}
	}
	private float GetAngleDiff(float _angle1, float _angle2)
	{
		if (_angle1 > 0f)
		{
			if (_angle2 < _angle1 - 180f)
			{
				return _angle2 + 360f - _angle1;
			}
			return _angle2 - _angle1;
		}
		if (_angle2 > _angle1 + 180f)
		{
			return _angle2 - 360f - _angle1;
		}
		return _angle2 - _angle1;
	}
	public void CartRotZ()
	{
		GetAngleDiff(nowEulerY, prevEulerY);
		float num = base.gameObject.transform.localEulerAngles.z;
		if (num > 180f)
		{
			num -= 360f;
		}
		nowRollAngle = NewGroundRoll();
		float num2 = 4f;
		base.transform.SetLocalEulerAnglesZ(Mathf.Lerp(num, 0f, num2 * Time.deltaTime));
	}
	public void CartRotX()
	{
		float num = 0f;
		float num2 = NewGroundTilt() * 2.5f;
		if (Mathf.Abs(num2) > 85f)
		{
			num2 = Mathf.Sign(num2) * 85f;
		}
		nowTiltAngle = num2;
		float num3 = base.transform.localEulerAngles.x;
		if (num3 > 180f)
		{
			num3 -= 360f;
		}
		if (Mathf.Abs(num3) > 85f)
		{
			num3 = Mathf.Sign(num3) * 85f;
		}
		if (num2 > 0f)
		{
			num2 /= 1f;
		}
		float f = num2 - num3;
		if (Mathf.Abs(f) < 0.5f)
		{
			base.transform.SetLocalEulerAnglesX(num2);
			return;
		}
		float num4 = 30f;
		if (Mathf.Abs(num2) < 1f)
		{
			num4 = 35f;
		}
		else if (num2 > 0f != num3 > 0f)
		{
			num4 = 45f;
		}
		num = Mathf.Clamp(num3 + Mathf.Sign(f) * Time.deltaTime * num4, -90f, 10f);
		base.transform.SetLocalEulerAnglesX(num);
	}
	public float CalcGroundTilt()
	{
		Vector3 a = Vector3.forward;
		Vector3 b = Vector3.zero;
		if (Physics.Raycast(groundRayForwardAnchor.position, Vector3.down, out RaycastHit hitInfo, 15f, 8388608))
		{
			a = hitInfo.point;
		}
		if (Physics.Raycast(groundRayBackAnchor.position, Vector3.down, out hitInfo, 15f, 8388608))
		{
			b = hitInfo.point;
		}
		Vector3 vector = Quaternion.Euler(0f, 0f - base.transform.eulerAngles.y, 0f) * (a - b);
		return Mathf.Atan2(vector.y, vector.z) * 57.29578f * -1f;
	}
	private float NewGroundTilt()
	{
		Vector3 point = Quaternion.Euler(0f, 0f - base.transform.eulerAngles.y, 0f) * updateGroundedNormal;
		point = Quaternion.Euler(90f, 0f, 0f) * point;
		return Mathf.Atan2(point.y, point.z) * 57.29578f * -1f;
	}
	private float NewGroundRoll()
	{
		Vector3 vector = Quaternion.Euler(0f, 0f - base.transform.eulerAngles.y, 0f) * updateGroundedNormal;
		return Mathf.Atan2(vector.x, vector.y) * 57.29578f * -0.8f;
	}
	private void CheckGrounded()
	{
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(groundRayBackAnchor.position, -groundRayBackAnchor.up, out hitInfo, 10f, 8388608);
		if (flag)
		{
			updateGroundedNormal = hitInfo.normal;
		}
		else
		{
			updateGroundedNormal = Vector3.up;
		}
		if (flag && groundRayBackAnchor.position.y - hitInfo.point.y < 0.25f)
		{
			if (!isGrounded)
			{
				airTime = 0f;
			}
			isGrounded = true;
			if (isJump && Time.time - jumpTime > 0.5f)
			{
				isJump = false;
				animator.SetBool("Jump", isJump);
			}
		}
		else
		{
			bool isGrounded2 = isGrounded;
			isGrounded = false;
		}
		if (isJump && flag)
		{
			Vector3 point = hitInfo.point;
			point.y += 0.005f;
		}
	}
	private void InitCameraTransData()
	{
		Vector3 vector = cameraTarget.position - GetCameraLookAtPos();
		float angle = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		float angleDiff = GetAngleDiff(cameraAngleY - 180f, angle);
		cameraAngleY += angleDiff;
		if (cameraAngleY < 0f)
		{
			cameraAngleY += 360f;
		}
		else if (cameraAngleY >= 360f)
		{
			cameraAngleY -= 360f;
		}
		cameraSpeedZValue = 0f;
	}
	private void UpdateCameraTransData()
	{
		Vector3 vector = cameraTarget.position - GetCameraLookAtPos();
		float angle = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		float angleDiff = GetAngleDiff(cameraAngleY - 180f, angle);
		float num = 1.5f;
		if (cameraPointNo == 2)
		{
			num = 4.5f;
		}
		cameraAngleY += angleDiff * num * Time.fixedDeltaTime;
		if (cameraAngleY < 0f)
		{
			cameraAngleY += 360f;
		}
		else if (cameraAngleY >= 360f)
		{
			cameraAngleY -= 360f;
		}
		float num2 = 0f;
		Vector3 velocity = rigid.velocity;
		velocity.y = 0f;
		num2 = ((!(Vector3.Dot(velocity, base.transform.forward) > 0f) && !isDrift) ? ((0f - velocity.magnitude) / 2f) : ((0f - velocity.magnitude) / 4f));
		if (isDash)
		{
			num2 *= 2f;
		}
		float num3 = num2 - cameraSpeedZValue;
		if (Mathf.Abs(num3) < 0.02f)
		{
			cameraSpeedZValue = num2;
		}
		else
		{
			cameraSpeedZValue += num3 * Time.fixedDeltaTime * 3f;
		}
	}
	public void ChangeCameraPointRight()
	{
		if (isCameraPointMoving)
		{
			Vector3 localPosition = cameraTarget.localPosition;
			if (cameraPointNo != 2)
			{
				localPosition.z += cameraSpeedZValue;
			}
			if (isCameraPointPrevMoving)
			{
				cameraPointMovingPrevPos = Vector3.Lerp(cameraPointMovingPrevPos, localPosition, cameraPointMovingLerp);
			}
			else
			{
				cameraPointMovingPrevPos = Vector3.Lerp(cameraPoints[cameraPrevPointNo].localPosition, localPosition, cameraPointMovingLerp);
				isCameraPointPrevMoving = true;
			}
		}
		else
		{
			cameraPointMovingPrevPos = cameraPoints[cameraPointNo].localPosition;
			cameraPointMovingPrevPos.z += cameraSpeedZValue;
			isCameraPointPrevMoving = false;
		}
		cameraPrevPointNo = cameraPointNo;
		cameraPointNo++;
		if (cameraPointNo >= cameraPoints.Length)
		{
			cameraPointNo -= cameraPoints.Length;
		}
		cameraTarget.position = cameraPoints[cameraPointNo].position;
		bool isOnce = false;
		LeanTween.cancel(cameraTarget.gameObject);
		isCameraPointMoving = true;
		cameraPointMovingLerp = 0f;
		LeanTween.value(cameraTarget.gameObject, 0f, 1f, 1f).setEaseOutQuart().setOnUpdate(delegate(float _value)
		{
			cameraPointMovingLerp = _value;
			if (!isOnce)
			{
				if (cameraPointNo == 2 && cameraPointMovingLerp >= 0.8f)
				{
					isOnce = true;
					SettingCharacterBaseLayer();
				}
				else if (cameraPointNo != 2 && cameraPointMovingLerp >= 0.25f)
				{
					isOnce = true;
					DisplayCharacterBaseLayer();
				}
			}
		})
			.setOnComplete((Action)delegate
			{
				isCameraPointMoving = false;
			});
	}
	public void ChangeCameraPointLeft()
	{
		if (isCameraPointMoving)
		{
			Vector3 localPosition = cameraTarget.localPosition;
			if (cameraPointNo != 2)
			{
				localPosition.z += cameraSpeedZValue;
			}
			if (isCameraPointPrevMoving)
			{
				cameraPointMovingPrevPos = Vector3.Lerp(cameraPointMovingPrevPos, localPosition, cameraPointMovingLerp);
			}
			else
			{
				cameraPointMovingPrevPos = Vector3.Lerp(cameraPoints[cameraPrevPointNo].localPosition, localPosition, cameraPointMovingLerp);
				isCameraPointPrevMoving = true;
			}
		}
		else
		{
			cameraPointMovingPrevPos = cameraPoints[cameraPointNo].localPosition;
			cameraPointMovingPrevPos.z += cameraSpeedZValue;
			isCameraPointPrevMoving = false;
		}
		cameraPrevPointNo = cameraPointNo;
		cameraPointNo--;
		if (cameraPointNo < 0)
		{
			cameraPointNo += cameraPoints.Length;
		}
		cameraTarget.position = cameraPoints[cameraPointNo].position;
		bool isOnce = false;
		LeanTween.cancel(cameraTarget.gameObject);
		isCameraPointMoving = true;
		cameraPointMovingLerp = 0f;
		LeanTween.value(cameraTarget.gameObject, 0f, 1f, 1f).setEaseOutQuart().setOnUpdate(delegate(float _value)
		{
			cameraPointMovingLerp = _value;
			if (!isOnce)
			{
				if (cameraPointNo == 2 && cameraPointMovingLerp >= 0.8f)
				{
					isOnce = true;
					SettingCharacterBaseLayer();
				}
				else if (cameraPointNo != 2 && cameraPointMovingLerp >= 0.25f)
				{
					isOnce = true;
					DisplayCharacterBaseLayer();
				}
			}
		})
			.setOnComplete((Action)delegate
			{
				isCameraPointMoving = false;
			});
	}
	private void UpdateCaterpillarOffset()
	{
		float num = 0f;
		Vector3 velocity = rigid.velocity;
		velocity.y = 0f;
		num = velocity.magnitude;
		if (isBack)
		{
			num *= -1f;
		}
	}
	public void AddWaterGauge()
	{
		waterGauge += 0.2f;
		if (waterGauge > slipStreamMag)
		{
			waterGauge = slipStreamMag;
		}
	}
	private int LapCalc()
	{
		return Mathf.FloorToInt(nowRunDistance / SingletonCustom<MonsterRace_CarManager>.Instance.CircuitLength + 0.5f);
	}
	public void ResultInit()
	{
		carAudio.EndAudio();
	}
	public void PlayerJump()
	{
		if (isJump)
		{
			return;
		}
		isJump = true;
		jumpTime = Time.time;
		animator.SetBool("Jump", isJump);
		if (isPlayer)
		{
			rigid.AddForce(Vector3.up * 3.2f, ForceMode.VelocityChange);
			switch (UnityEngine.Random.Range(0, 4))
			{
			case 0:
				RaceSePlay("se_monster_race_run_0");
				break;
			case 1:
				RaceSePlay("se_monster_race_run_1");
				break;
			case 2:
				RaceSePlay("se_monster_race_run_2");
				break;
			case 3:
				RaceSePlay("se_monster_race_run_3");
				break;
			}
		}
		else
		{
			rigid.AddForce(Vector3.up * 3.2f, ForceMode.VelocityChange);
		}
	}
	public void PlayerDash()
	{
		if (staminaCount != 0 && !isDash)
		{
			if (isPlayer || SingletonCustom<MonsterRace_CarManager>.Instance.PlayerNum == 3)
			{
				SingletonCustom<MonsterRace_CarManager>.Instance.SpeedLineStart(charaNo);
			}
			RaceSePlay("se_eraserrace_dash");
			staminaCount = Mathf.Clamp(staminaCount - 1, 0, maxStaminaCount);
			dashTime = 2f;
			isDash = true;
			dashStartEffect.Play();
			dashMoveEffect.Play();
		}
	}
	private void PlaySeRun()
	{
		if (isPlayer && isGrounded)
		{
			switch (UnityEngine.Random.Range(0, 4))
			{
			case 0:
				RaceSePlay("se_monster_race_run_0");
				break;
			case 1:
				RaceSePlay("se_monster_race_run_1");
				break;
			case 2:
				RaceSePlay("se_monster_race_run_2");
				break;
			case 3:
				RaceSePlay("se_monster_race_run_3");
				break;
			}
		}
	}
	private IEnumerator CollReset()
	{
		yield return new WaitForSeconds(3f);
	}
	private void OnTriggerEnter(Collider _collider)
	{
		if (_collider.gameObject.tag == "NoHit")
		{
			isAiBrake = true;
		}
		if (_collider.gameObject.tag == "Water" && Time.time - waterEffectTime > 0.5f)
		{
			waterEffectTime = Time.time;
			waterEffect.transform.SetPositionY(_collider.transform.position.y);
			waterEffect.Play();
			RaceSePlay("se_monster_race_water");
		}
		if (_collider.gameObject.tag == "GoalBarrier")
		{
			isTunnel = true;
			isTunnelCameraMove = true;
		}
		if (!(_collider.gameObject.tag == "Goal") || isGoal)
		{
			return;
		}
		int num = LapCalc();
		if (nowLap < num)
		{
			nowLap = num;
			if (nowLap == SingletonCustom<MonsterRace_CarManager>.Instance.RaceLap)
			{
				SingletonCustom<MonsterRace_CarManager>.Instance.CarGoal(charaNo);
			}
			else
			{
				SingletonCustom<MonsterRace_CarManager>.Instance.CarNextLap(charaNo);
			}
		}
	}
	private void OnTriggerStay(Collider _collider)
	{
		if (!isRough && _collider.gameObject.tag == "GoalNet")
		{
			isRough = true;
		}
		if (_collider.gameObject.tag == "PenaltyArea")
		{
			onNotSmokeAreaTime = Time.time;
			isOnNotSmokeArea = true;
		}
		if (_collider.gameObject.tag == "Water")
		{
			onWaterAreaTime = Time.time;
			isOnWaterArea = true;
			waterMoveEffectAnchor.transform.SetPositionY(_collider.transform.position.y);
			for (int i = 0; i < waterMoveEffects.Length; i++)
			{
				if (!waterMoveEffects[i].isPlaying)
				{
					waterMoveEffects[i].Play();
				}
			}
		}
		if (isPlayer && !isGoal && _collider.gameObject.tag == "CheckPoint" && nowRunDistance > 100f)
		{
			SingletonCustom<MonsterRace_GameManager>.Instance.AudienceSeAreaIn();
		}
		if ((!isPlayer || isGoal) && _collider.gameObject.tag == "Airplane")
		{
			PlayerJump();
		}
	}
	private void OnTriggerExit(Collider _collider)
	{
		if (_collider.gameObject.tag == "Coin")
		{
			StartCoroutine(CollReset());
		}
		if (_collider.gameObject.tag == "GoalBarrier")
		{
			isTunnel = false;
			isTunnelCameraMove = true;
		}
		if (_collider.gameObject.tag == "NoHit")
		{
			isAiBrake = false;
		}
	}
	private void OnCollisionEnter(Collision _collision)
	{
		if (_collision.gameObject.tag != "Respawn" || _collision.gameObject.tag != "Object")
		{
			smokeEffect.Stop();
		}
		if (_collision.gameObject.tag == "Ball")
		{
			rigid.velocity = Vector3.zero;
			RaceSePlay("se_breakbox");
		}
		if (!(_collision.gameObject.tag == "Floor") && !(_collision.gameObject.tag == "Field") && !(_collision.gameObject.tag == "Character") && !isGoal && _collision.gameObject.tag == "Player" && !SingletonCustom<MonsterRace_GameManager>.Instance.IsGameEnd && Time.time - hitSeTime > 0.5f)
		{
			hitSeTime = Time.time;
			RaceSePlay("se_collision_character_10");
		}
	}
	public void Standby()
	{
		if (!isPlayer)
		{
			base.transform.SetLocalEulerAnglesY(270f);
		}
	}
	public void ChangeSmokeColorGray()
	{
		ParticleSystem.MainModule main = smokeEffect.main;
		ParticleSystem.MinMaxGradient startColor = main.startColor;
		Color color = main.startColor.color;
		color.r = (color.g = (color.b = 0.8f));
		startColor.color = color;
		main.startColor = startColor;
	}
	public bool IsCurveLeft()
	{
		if (!isPlayer)
		{
			return aiX <= -0.1f;
		}
		return SingletonCustom<MonsterRace_ControllerManager>.Instance.GetMoveDir(playerNo).x <= -0.1f;
	}
	public bool IsCurveRight()
	{
		if (!isPlayer)
		{
			return aiX >= 0.1f;
		}
		return SingletonCustom<MonsterRace_ControllerManager>.Instance.GetMoveDir(playerNo).x >= 0.1f;
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
	public Vector3 GetDir()
	{
		return Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f) * Vector3.forward;
	}
	public Collider GetCollider()
	{
		return collider;
	}
	public string GetName()
	{
		return base.name;
	}
	public Transform GetCameraTarget()
	{
		return cameraTarget;
	}
	public Vector3 GetCameraPosition()
	{
		Vector3 vector = cameraTarget.localPosition;
		if (cameraPointNo != 2)
		{
			vector.z += cameraSpeedZValue;
		}
		else
		{
			vector.y += cameraHeaveValue;
		}
		if (isCameraPointMoving)
		{
			Vector3 a = cameraPointMovingPrevPos;
			if (cameraPrevPointNo == 0 && (isTunnel || isTunnelCameraMove))
			{
				a.y += tunnelHeight;
			}
			vector = Vector3.Lerp(a, vector, cameraPointMovingLerp);
		}
		return base.transform.position + Quaternion.Euler(0f, cameraAngleY, 0f) * vector * cameraTarget.lossyScale.x;
	}
	public Vector3 GetFirstCameraPosition()
	{
		Vector3 point = cameraPoints[0].localPosition * 2f;
		return base.transform.position + Quaternion.Euler(0f, cameraAngleY, 0f) * point * cameraTarget.lossyScale.x;
	}
	public Vector3 GetZoomCameraPosition()
	{
		Vector3 point = cameraPoints[0].localPosition * 0.8f;
		return base.transform.position + Quaternion.Euler(0f, cameraAngleY, 0f) * point * cameraTarget.lossyScale.x;
	}
	public Vector3 GetCameraLookAtPos()
	{
		if (cameraPointNo == 2)
		{
			Vector3 b = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f) * cameraLookAtPoints[cameraPointNo].localPosition;
			b.y += cameraHeaveValue;
			return base.transform.position + b;
		}
		return cameraLookAtPoints[cameraPointNo].position;
	}
	private void SettingCharacterBaseLayer()
	{
		int num = 28;
		switch (charaNo)
		{
		case 1:
			num = 29;
			break;
		case 2:
			num = 31;
			break;
		case 3:
			num = 27;
			break;
		}
		SettingChildLayer(wolfObj.gameObject.transform, num);
		SettingChildLayer(llamaObj.gameObject.transform, num);
		int num2 = SingletonCustom<MonsterRace_CarManager>.Instance.TpsCamera[charaNo].cullingMask;
		int num3 = 1 << num;
		if ((num2 & num3) == num3)
		{
			num2 -= num3;
		}
		SingletonCustom<MonsterRace_CarManager>.Instance.TpsCamera[charaNo].cullingMask = num2;
	}
	private void DisplayCharacterBaseLayer()
	{
		int layerNo = 0;
		SettingChildLayer(wolfObj.gameObject.transform, layerNo);
		SettingChildLayer(llamaObj.gameObject.transform, layerNo);
	}
	private void SettingChildLayer(Transform _trans, int _layerNo)
	{
		_trans.gameObject.layer = _layerNo;
		for (int i = 0; i < _trans.childCount; i++)
		{
			SettingChildLayer(_trans.GetChild(i), _layerNo);
		}
	}
	public void SetCameraRotAnchor(Transform _anchor)
	{
		cameraRotAnchor = _anchor;
	}
	public void SetCarMaterial(Material _mat)
	{
		Material[] sharedMaterials = wolfRenderer.sharedMaterials;
		sharedMaterials[0] = _mat;
		wolfRenderer.sharedMaterials = sharedMaterials;
		sharedMaterials = llamaRenderer.sharedMaterials;
		sharedMaterials[0] = _mat;
		llamaRenderer.sharedMaterials = sharedMaterials;
	}
	public void ChangeRandomCharaStyle(int _texNo)
	{
		StyleTextureManager.GenderType genderType = (StyleTextureManager.GenderType)UnityEngine.Random.Range(0, 2);
		_texNo %= SingletonCustom<StyleTextureManager>.Instance.GetBaseTextureTotal(genderType);
		charaStyle.SetStyle(genderType, (StyleTextureManager.FaceType)UnityEngine.Random.Range(0, 4), (StyleTextureManager.HairColorType)UnityEngine.Random.Range(0, 3), (CharacterStyle.ShapeType)UnityEngine.Random.Range(0, 3), _texNo);
	}
	public void SetCharaGameStyle(int _playerNo, int _teamNo)
	{
		charaStyle.SetGameStyle(GS_Define.GameType.MOLE_HAMMER, _playerNo, _teamNo);
		int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[styleCharaNo];
		if (num == 1 || num == 5)
		{
			wolfObj.SetActive(value: false);
			llamaObj.SetActive(value: true);
			animator = llamaAnimator;
			charaStyle.transform.parent = llamaCharaParent;
			charaStyle.transform.localPosition = Vector3.zero;
		}
		else
		{
			wolfObj.SetActive(value: true);
			llamaObj.SetActive(value: false);
			animator = wolfAnimator;
		}
		animator.SetFloat("Speed", 0f);
		animator.SetBool("Jump", value: false);
	}
	public void AiInit()
	{
		int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		aiDashIntervalTimes = CPU_IS_DASH_INTERVAL[aiStrength];
	}
	public void AiMove()
	{
		if (!SingletonCustom<MonsterRace_GameManager>.Instance.IsGameStart)
		{
			return;
		}
		aiDir = Quaternion.Euler(0f, 0f - base.transform.eulerAngles.y, 0f) * wptAi.target.forward;
		float x = aiDir.x;
		float z = aiDir.z;
		aiX = x;
		aiZ = z;
		Vector3 lhs = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f) * Vector3.forward;
		Vector3 rhs = wptAi.progressPoint.position - base.transform.position;
		rhs.y = 0f;
		aiDis = rhs.magnitude;
		aiCross = Vector3.Cross(lhs, rhs);
		float num = Mathf.Clamp(x * 2.5f + aiCross.y * 1f, -1f, 1f);
		if (isAiStraightRun)
		{
			num *= 0.02f;
			aiStraightTime -= Time.deltaTime;
			if (aiStraightTime < 0f)
			{
				isAiStraightRun = false;
			}
		}
		bool flag = Mathf.Abs(num) > 0.9f;
		if (!isAiBack)
		{
			Vector3 velocity = rigid.velocity;
			velocity.y = 0f;
			if (velocity.magnitude < 0.5f)
			{
				aiStopTime += Time.deltaTime;
				if (aiStopTime > 1f)
				{
					isAiBack = true;
					aiStopTime = 0f;
					float num2 = wptPos.progressDistance / wptPos.getCircuit.Length;
					wptAi.progressDistance = wptAi.getCircuit.Length * num2;
				}
			}
			else
			{
				aiStopTime = 0f;
			}
		}
		else
		{
			aiBackTime += Time.deltaTime;
			if (aiBackTime > 0.5f)
			{
				isAiBack = false;
				aiBackTime = 0f;
			}
			Vector3 velocity2 = rigid.velocity;
			velocity2.y = 0f;
			if (velocity2.magnitude < 1f)
			{
				num = 0f;
			}
		}
		if (aiDashIntervalTimer >= aiDashIntervalTimes[aiDashIntervalCount])
		{
			if (nowRank != 0 && !isGoal)
			{
				PlayerDash();
			}
			aiDashIntervalCount = UnityEngine.Random.Range(0, aiDashIntervalTimes.Length);
			aiDashIntervalTimer = 0f;
		}
		else
		{
			aiDashIntervalTimer += Time.deltaTime;
		}
		if (!isAiBack)
		{
			SetControlData(num, aiZ, _isAccel: true, flag);
		}
		else
		{
			SetControlData(0f - num, aiZ, _isAccel: false, _isBack: true);
		}
	}
	public void SetCpuStrengthData(float _speed, float _angle, float _slip)
	{
		maxSpeedInit += _speed;
		maxSteerAngleInit *= _angle;
		gripAngleSpeedInit *= _angle;
		slipStreamMag = _slip;
	}
	public void RaceSePlay(string _seName)
	{
		if (SingletonCustom<MonsterRace_GameManager>.Instance.IsGameEnd || isGoal)
		{
			return;
		}
		if (isPlayer)
		{
			SingletonCustom<MonsterRace_CarManager>.Instance.AllSePlayCheck(_seName, 1f, _isForcePlay: true);
		}
		else if (SingletonCustom<MonsterRace_CarManager>.Instance.PlayerNum == 1)
		{
			float num = Vector3.Distance(base.transform.position, SingletonCustom<MonsterRace_CarManager>.Instance.GetPlayerOnePosition());
			if (num < 4f)
			{
				SingletonCustom<MonsterRace_CarManager>.Instance.AllSePlayCheck(_seName, 0.9f - num / 4f);
			}
		}
	}
	private static float Pow2(float _value)
	{
		return _value * _value;
	}
}
