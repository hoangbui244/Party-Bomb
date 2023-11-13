using System;
using UnityEngine;
using UnityStandardAssets.Utility;
public class MikoshiRaceCarScript : MonoBehaviour
{
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
		public Transform Parts(BodyPartsList _parts)
		{
			return rendererList[(int)_parts].transform;
		}
		public Transform Parts(int _parts)
		{
			return rendererList[_parts].transform;
		}
	}
	private const float MAX_SPEED = 5f;
	private const float MAX_BACK_SPEED = 2f;
	private const float WAKE_UP_ACCEL_SPEED = 10f;
	private const float ACCEL_SPEED = 5f;
	private const float ACCEL_BACK_SPEED = 5f;
	private const float AUTO_BRAKE_SPEED = 4f;
	private const float STEER_BRAKE_SPEED = 4f;
	private const float STEER_ANGLE_NEUTORAL_SPEED = 3f;
	private const float STEER_ANGLE_ADD_SPEED = 2f;
	private const float DIRT_SPEED_MAG = 0.8f;
	private const float STOP_ANGLE_SPEED = 2f;
	private const float MOVE_ANGLE_SPEED = 1.5f;
	private const float GRIP_ANGLE_SPEED = 0.2f;
	private const float RUN_DISTANCE_ADD_MAX_VALUE = 1f;
	private const float CAMERA_ROT_SPEED = 1.5f;
	private const float CAMERA_LEAVE_MOVE_DISTANCE = 2f;
	private const float SHAKE_ADD_CTRL_POWER = 0.5f;
	private const float SHAKE_MINUS_CTRL_POWER = 1.5f;
	private const float SHAKE_MIN_CTRL_POWER = 0.5f;
	private const float SHAKE_MAX_CTRL_POWER = 2f;
	private const int SHAKE_ACTION_NUM = 10;
	private const int SHAKE_PUSH_NUM = 40;
	private const float SHAKE_PUSH_VALUE_BASE_SPEED = 1.5f;
	private const float SHAKE_PUSH_VALUE_DIFF_SPEED = 1.5f;
	public const float SHAKE_CTRL_DELAY = 1f;
	private const float WALL_RESIST_MAX = 0.02f;
	private const float WATER_ADD = 0.3f;
	private const float WATER_MINUS = 0.3f;
	private const float WATER_SPEED_UP = 1f;
	private const float DIRT_SPEED_CORR = 0.3f;
	private const float AI_RETURN_TIME = 10f;
	private const float GRAVITY = 9.8f;
	private const float MAX_ANGLE_X = 30f;
	private const float HIP_LOCAL_POS_Y = 0.05483828f;
	private const float HEAVE_SPEED = 1.5f;
	private const float HEAVE_MIKOSHI_VALUE = 0.015f;
	private const float HEAVE_CHARA_VALUE = 0.015f;
	private const float MAX_BOUND_POWER = 8f;
	private const float MAX_BOUND_SPEED = 10f;
	private const string TAG_FLOOR = "Floor";
	private const string TAG_WALL = "VerticalWall";
	private const string TAG_FIELD = "Field";
	private const string TAG_OBJECT = "Object";
	private const string TAG_GOAL = "Goal";
	public const string TAG_REVERSE_IN = "Airplane";
	public const string TAG_REVERSE_OUT = "School";
	private const string TAG_MIKOSHI_POINT = "GoalHoop";
	private const string TAG_MIKOSHI_POINT_ROAD = "GoalBarrier";
	public const string TAG_CAR = "Player";
	private const string TAG_CPU_REVERSE_GUARD = "GoalNet";
	private const int LAYER_GROUND = 8388608;
	private const int LAYER_AI_DODGE = 67108864;
	private const float ANIM_SHOULDER_DEFAULT_LOCAL_POS_Y = 0.1297733f;
	private const float ANIM_SHOULDER_SHAKE_LOCAL_POS_Y = 0.18f;
	private const float ANIM_SHOULDER_L_DEFAULT_LOCAL_EULER_Z = 140f;
	private const float ANIM_SHOULDER_L_SHAKE_LOCAL_EULER_Z = 155f;
	private const float ANIM_SHOULDER_R_DEFAULT_LOCAL_EULER_Z = 120f;
	private const float ANIM_SHOULDER_R_SHAKE_LOCAL_EULER_Z = 135f;
	[SerializeField]
	[Header("順位判定用")]
	private WaypointProgressTracker wptPos;
	[SerializeField]
	[Header("AI操作用")]
	private WaypointProgressTracker wptAi;
	[SerializeField]
	[Header("逆走チェック")]
	private MikoshiRaceCarReverseChecker reverseChecker;
	[SerializeField]
	[Header("神輿アンカ\u30fc")]
	private Transform mikoshiAnchor;
	[SerializeField]
	[Header("カメラタ\u30fcゲット")]
	private Transform cameraTarget;
	[SerializeField]
	[Header("カメラ視点切り換え用")]
	private Transform[] cameraPoints;
	[SerializeField]
	[Header("カメラLookAt位置")]
	private Transform[] cameraLookAtPoints;
	[SerializeField]
	[Header("走るエフェクト")]
	private ParticleSystem[] runEffects;
	[SerializeField]
	[Header("影アンカ\u30fc")]
	private Transform shadowAnchor;
	[SerializeField]
	[Header("キャラスタイル")]
	private CharacterStyle[] charaStyles;
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts[] bodyPartsCharas;
	[SerializeField]
	[Header("神輿の揺れ曲線")]
	private AnimationCurve heaveCurve;
	[SerializeField]
	[Header("揺れアンカ\u30fc")]
	private Transform heaveAnchor;
	private bool isPlayer;
	private int playerNo;
	private int carNo;
	private int teamNo;
	private bool isRankLock;
	private int nowRank;
	private int nowLap;
	private bool isGoal;
	private bool isGoalMove;
	private float viewTime = -1f;
	private float goalTime = -1f;
	public float nowRunDistance;
	public float maxRunDistance;
	private Rigidbody rigid;
	private Vector3 prevPos;
	private Vector3 nowPos;
	private float prevEulerY;
	private float nowEulerY;
	private Vector3 moveDir;
	private Vector3 moveVel;
	private bool isMoveBack;
	private float inputSteerDir;
	private bool isInputAccel;
	private bool isInputBack;
	private float maxSpeedInit = 5f;
	private float gripAngleSpeedInit = 0.2f;
	private float anglePower;
	private float waterGauge;
	private float heaveTime;
	private bool isMultiShadow;
	private Transform cameraRotAnchor;
	private int cameraPointNo;
	private float cameraAngleY;
	private float cameraSpeedZValue;
	private int mikoshiPointNo = 1;
	private float mikoshiPointArrivalTime;
	private int mikoshiPointSpaceNo;
	private Vector3 mikoshiPointCameraDistance;
	private Vector3 mikoshiPointCameraLeaveVec;
	private Vector3 mikoshiPointDir;
	private float mikoshiPointCircuitDistance;
	private bool isMikoshiPointRoad;
	private int mikoshiPointRoadNo;
	private Vector3 mikoshiPointRoadDir;
	private Vector3 mikoshiPointRoadPos;
	private float mikoshiPointSpaceSearchTime;
	private Transform mikoshiPointSpaceTrans;
	private float mikoshiPointBetweenTime;
	private bool isShake;
	private bool isShakeCtrl;
	private bool isMikoshiPointCamera;
	private float shakeValue;
	private float shakeCtrlPower;
	private int shakePushCount;
	private bool isPlayRunEffect;
	private bool isCpuReverseGuard;
	private float reverseGuardEnterDis;
	private float runAnimationSpeed = 40f;
	private float runAnimationTime;
	private int playSeRunCnt;
	private float runSeInterval;
	private bool isChangeAnimationNeutral;
	private static readonly float[][] CPU_ADD_MAX_SPEED = new float[3][]
	{
		new float[3]
		{
			-0.5f,
			-0.4f,
			-0.3f
		},
		new float[3]
		{
			-0.3f,
			-0.1f,
			0.1f
		},
		new float[3]
		{
			0f,
			0.2f,
			0.4f
		}
	};
	private static readonly float[][] CPU_MAG_MAX_STEER_ANGLE = new float[3][]
	{
		new float[3]
		{
			1.1f,
			1f,
			0.9f
		},
		new float[3]
		{
			1.3f,
			1.2f,
			1.1f
		},
		new float[3]
		{
			1.5f,
			1.4f,
			1.3f
		}
	};
	private static readonly float[][] CPU_SHAKE_ACTION_INTERVAL = new float[3][]
	{
		new float[3]
		{
			0.24f,
			0.27f,
			0.3f
		},
		new float[3]
		{
			0.18f,
			0.2f,
			0.22f
		},
		new float[3]
		{
			0.125f,
			0.143f,
			0.167f
		}
	};
	public float aiX;
	public Vector3 aiDir;
	public float aiDis;
	public Vector3 aiCross;
	private float aiStopTime;
	private bool isAiBack;
	private float aiBackTime;
	private float aiShakeActionInterval;
	private float aiShakeActionTime;
	public bool IsPlayer => isPlayer;
	public int PlayerNo => playerNo;
	public int CarNo => carNo;
	public int TeamNo => teamNo;
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
	public float RunDistance
	{
		get
		{
			if (IsOneRoadGo)
			{
				return wptPos.progressDistance + 10000f - (mikoshiPointRoadPos - base.transform.position).sqrMagnitude;
			}
			return nowRunDistance;
		}
	}
	public bool IsReverseRun
	{
		get
		{
			if (isMikoshiPointRoad)
			{
				return false;
			}
			float num = maxRunDistance - nowRunDistance;
			if (num > 12f)
			{
				if (num > 16f)
				{
					maxRunDistance = nowRunDistance + 13f;
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
			return Mathf.Clamp01(velocity.magnitude / 5f);
		}
	}
	public bool IsFirstPersonCameraPoint => cameraPointNo == 2;
	public int MikoshiPointNo => mikoshiPointNo;
	public float MikoshiPointArrivalTime => mikoshiPointArrivalTime;
	public float MikoshiPointBetweenTime => mikoshiPointBetweenTime;
	public bool IsOneRoadGo
	{
		get
		{
			if (isMikoshiPointRoad)
			{
				return mikoshiPointRoadNo > mikoshiPointNo;
			}
			return false;
		}
	}
	public bool IsShake => isShake;
	public bool IsShakeCtrl => isShakeCtrl;
	public bool IsMikoshiPointCamera => isMikoshiPointCamera;
	public float ShakeValueLerp => Mathf.Clamp01(shakeValue / 10f);
	public void Init(int _carNo)
	{
		int teamNum = SingletonCustom<MikoshiRaceGameManager>.Instance.TeamNum;
		carNo = _carNo;
		if (SingletonCustom<MikoshiRaceGameManager>.Instance.IsEightBattle)
		{
			teamNo = _carNo / 4;
			playerNo = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[teamNo][_carNo % 4];
		}
		else if (teamNum == 2)
		{
			if (SingletonCustom<MikoshiRaceGameManager>.Instance.PlayerNum == 4)
			{
				playerNo = _carNo;
				if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0].Contains(_carNo))
				{
					teamNo = 0;
				}
				else
				{
					teamNo = 1;
				}
			}
			else
			{
				teamNo = _carNo / 2;
				playerNo = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[teamNo][_carNo % 2];
			}
		}
		else
		{
			teamNo = _carNo;
			playerNo = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[teamNo][0];
		}
		isPlayer = (playerNo < 4);
		charaStyles[0].SetMainCharacterStyle(playerNo);
		for (int i = 1; i < charaStyles.Length; i++)
		{
			StyleTextureManager.GenderType genderType = (StyleTextureManager.GenderType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(StyleTextureManager.GenderType)).Length);
			StyleTextureManager.FaceType faceType = (StyleTextureManager.FaceType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(StyleTextureManager.FaceType)).Length);
			StyleTextureManager.HairColorType hairColor = (StyleTextureManager.HairColorType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(StyleTextureManager.HairColorType)).Length);
			CharacterStyle.ShapeType shape = (CharacterStyle.ShapeType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(CharacterStyle.ShapeType)).Length);
			charaStyles[i].SetStyle(genderType, faceType, hairColor, shape);
			for (int j = 1; j < bodyPartsCharas[i].rendererList.Length; j++)
			{
				bodyPartsCharas[i].rendererList[j].sharedMaterials = bodyPartsCharas[0].rendererList[j].sharedMaterials;
			}
		}
		if (SingletonCustom<MikoshiRaceGameManager>.Instance.PlayerNum > 1)
		{
			shadowAnchor.gameObject.SetActive(value: true);
		}
		else
		{
			shadowAnchor.gameObject.SetActive(value: false);
		}
		rigid = GetComponent<Rigidbody>();
		InitWpt();
		DataInit();
	}
	public void SecondGroupInit()
	{
		teamNo = carNo / 2;
		playerNo = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[teamNo][carNo % 2 + 2];
		isPlayer = (playerNo < 4);
		charaStyles[0].SetMainCharacterStyle(playerNo);
		for (int i = 1; i < charaStyles.Length; i++)
		{
			StyleTextureManager.GenderType genderType = (StyleTextureManager.GenderType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(StyleTextureManager.GenderType)).Length);
			StyleTextureManager.FaceType faceType = (StyleTextureManager.FaceType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(StyleTextureManager.FaceType)).Length);
			StyleTextureManager.HairColorType hairColor = (StyleTextureManager.HairColorType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(StyleTextureManager.HairColorType)).Length);
			CharacterStyle.ShapeType shape = (CharacterStyle.ShapeType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(CharacterStyle.ShapeType)).Length);
			charaStyles[i].SetStyle(genderType, faceType, hairColor, shape);
			for (int j = 1; j < bodyPartsCharas[i].rendererList.Length; j++)
			{
				bodyPartsCharas[i].rendererList[j].sharedMaterials = bodyPartsCharas[0].rendererList[j].sharedMaterials;
			}
		}
		DataInit();
	}
	private void DataInit()
	{
		isRankLock = false;
		nowRank = 0;
		nowLap = 0;
		isGoal = false;
		isGoalMove = false;
		viewTime = -1f;
		goalTime = -1f;
		nowRunDistance = 0f;
		maxRunDistance = 0f;
		wptPos.progressDistance = 0f;
		wptAi.progressDistance = 0f;
		wptAi.gameObject.SetActive(!isPlayer);
		reverseChecker.Init();
		nowPos = (prevPos = base.transform.position);
		nowEulerY = (prevEulerY = base.transform.eulerAngles.y);
		moveDir = Vector3.zero;
		moveVel = Vector3.zero;
		isMoveBack = false;
		inputSteerDir = 0f;
		isInputAccel = false;
		isInputBack = false;
		maxSpeedInit = 5f;
		gripAngleSpeedInit = 0.2f;
		anglePower = 0f;
		waterGauge = 0f;
		heaveTime = 0f;
		cameraPointNo = 0;
		cameraAngleY = 0f;
		cameraSpeedZValue = 0f;
		cameraTarget.position = cameraPoints[cameraPointNo].position;
		InitCameraTransData();
		mikoshiPointNo = 1;
		mikoshiPointArrivalTime = 0f;
		isMikoshiPointRoad = false;
		mikoshiPointRoadNo = 0;
		mikoshiPointBetweenTime = 0f;
		isShake = false;
		isShakeCtrl = false;
		shakeValue = 0f;
		shakeCtrlPower = 0f;
		shakePushCount = 0;
		isMikoshiPointCamera = false;
		SingletonCustom<MikoshiRaceCarManager>.Instance.GuideArrowChange(carNo);
		SingletonCustom<MikoshiRaceCarManager>.Instance.GuideArrowColorSetting(carNo);
		isCpuReverseGuard = false;
		runAnimationTime = 0f;
		playSeRunCnt = 0;
		runSeInterval = 0f;
		isChangeAnimationNeutral = true;
		StopRunEffect();
		ResetAnimation();
		if (!isPlayer)
		{
			AiDataInit();
		}
	}
	public void InitWpt()
	{
		wptPos.setCircuit = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.GetPosCircuit();
		if (carNo < 4)
		{
			wptAi.setCircuit = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.GetAiCircuit(carNo);
		}
		else if (carNo == 5)
		{
			wptAi.setCircuit = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.GetAiCircuit(0);
		}
		else if (carNo == 6)
		{
			wptAi.setCircuit = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.GetAiCircuit(UnityEngine.Random.Range(0, 4));
		}
		else if (carNo == 7)
		{
			wptAi.setCircuit = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.GetAiCircuit(2);
		}
		else
		{
			wptAi.setCircuit = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.GetAiCircuit(0);
		}
	}
	public void UpdateMethod()
	{
		if (isMikoshiPointRoad)
		{
			if (IsOneRoadGo && mikoshiPointCircuitDistance < wptPos.progressDistance)
			{
				wptPos.progressDistance = mikoshiPointCircuitDistance;
			}
			else if (!IsOneRoadGo && mikoshiPointCircuitDistance > wptPos.progressDistance)
			{
				wptPos.progressDistance = mikoshiPointCircuitDistance;
			}
		}
		else
		{
			mikoshiPointCircuitDistance = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.GetMikoshiPointCircuitDistance(mikoshiPointNo);
		}
		if (wptPos.runDistance - nowRunDistance > 1f)
		{
			nowRunDistance += 1f;
		}
		else
		{
			nowRunDistance = wptPos.runDistance;
		}
		if (isMikoshiPointRoad)
		{
			maxRunDistance = nowRunDistance;
		}
		else if (maxRunDistance < nowRunDistance)
		{
			maxRunDistance = nowRunDistance;
		}
		if (!SingletonCustom<MikoshiRaceGameManager>.Instance.IsGameStart)
		{
			wptPos.progressDistance = 0f;
			wptAi.progressDistance = 0f;
			Vector3 velocity = rigid.velocity;
			velocity.y -= 9.8f * Time.deltaTime;
			velocity.x = (velocity.z = 0f);
			rigid.velocity = velocity;
		}
		else
		{
			if (isCpuReverseGuard)
			{
				wptAi.progressDistance = reverseGuardEnterDis - 1f;
			}
			MoveUpdate();
			ShakeUpdate();
		}
	}
	private void MoveUpdate()
	{
		if (!isShake)
		{
			prevPos = nowPos;
			nowPos = base.transform.position;
			prevEulerY = nowEulerY;
			nowEulerY = base.transform.eulerAngles.y;
			mikoshiPointBetweenTime += Time.deltaTime;
			waterGauge -= 0.3f * Time.deltaTime;
			if (waterGauge < 0f)
			{
				waterGauge = 0f;
			}
			Vector3 velocity = rigid.velocity;
			velocity.y -= 9.8f * Time.deltaTime;
			rigid.velocity = velocity;
			if (isGoalMove)
			{
				SetControlData(0f, _isAccel: true, _isBack: false);
			}
			Move();
			MoveRot();
			float speedLerp = SpeedLerp;
			heaveTime += 1.5f * speedLerp * Time.deltaTime;
			heaveAnchor.SetLocalPositionY(heaveCurve.Evaluate(heaveTime) * 0.015f);
			for (int i = 0; i < bodyPartsCharas.Length; i++)
			{
				bodyPartsCharas[i].Parts(BodyPartsList.BODY).SetLocalPositionY(heaveCurve.Evaluate(heaveTime) * 0.015f);
			}
		}
	}
	private void ShakeUpdate()
	{
		if (!isShakeCtrl)
		{
			return;
		}
		float num = (float)shakePushCount / 40f * 10f;
		float num2 = 1.5f;
		if (num > shakeValue)
		{
			num2 += (num - shakeValue) * 1.5f;
		}
		shakeValue += num2 * Time.deltaTime;
		if (shakeValue > num)
		{
			shakeValue = num;
		}
		float num3 = shakeValue % 1f;
		num3 *= 2f;
		if (num3 > 1f)
		{
			num3 = 2f - num3;
		}
		ShakeAnimation(num3);
		if (isPlayer)
		{
			SingletonCustom<MikoshiRaceUiManager>.Instance.SetShakeGauge(carNo, ShakeValueLerp);
		}
		if (!(shakeValue > 9.9999f))
		{
			return;
		}
		ShakeAnimation(0f);
		if (mikoshiPointNo == SingletonCustom<MikoshiRaceGameManager>.Instance.Course.mikoshiPointColliders.Length)
		{
			ShakeGoal();
			return;
		}
		if (isPlayer)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_perfect");
		}
		ShakeEnd();
	}
	private void FixedUpdate()
	{
		if (SingletonCustom<MikoshiRaceGameManager>.Instance.IsGameStart && !isShake && !isGoal)
		{
			UpdateCameraTransData();
		}
	}
	public void SetControlData(float _steerDir, bool _isAccel, bool _isBack)
	{
		inputSteerDir = _steerDir;
		isInputAccel = _isAccel;
		isInputBack = _isBack;
	}
	public void Move()
	{
		if (rigid.isKinematic)
		{
			return;
		}
		Vector3 vector = rigid.velocity;
		float y = vector.y;
		vector.y = 0f;
		Vector3 forward = base.transform.forward;
		bool flag = false;
		if (isInputAccel)
		{
			float d = 5f;
			if (vector.sqrMagnitude < 1f)
			{
				d = 10f;
			}
			vector += forward * d * Time.deltaTime;
			float num = maxSpeedInit + 1f * waterGauge;
			if (vector.sqrMagnitude > num * num)
			{
				vector = vector.normalized * num;
			}
			flag = true;
		}
		isMoveBack = false;
		if (isInputBack)
		{
			vector -= forward * 5f * Time.deltaTime;
			isMoveBack = (Vector3.Dot(forward, vector) < 0f);
			if (isMoveBack && vector.sqrMagnitude > 4f)
			{
				vector = vector.normalized * 2f;
			}
			flag = true;
		}
		if (!flag)
		{
			vector = ((!(vector.sqrMagnitude < 0.01f)) ? (vector - vector.normalized * 4f * Time.deltaTime) : Vector3.zero);
		}
		float num2 = Vector3.Angle(vector, forward) * (float)((!(Vector3.Cross(vector, forward).y < 0f)) ? 1 : (-1));
		vector = Quaternion.Euler(0f, num2 * gripAngleSpeedInit * Time.deltaTime, 0f) * vector;
		moveDir = forward;
		moveVel = vector;
		vector.y = y;
		rigid.velocity = vector;
		MoveAnimation();
	}
	private void MoveRot()
	{
		float num = inputSteerDir;
		if (isMoveBack)
		{
			num *= -1f;
		}
		if (anglePower > 0f)
		{
			float num2 = (num > 0f) ? 2f : 3f;
			if (anglePower <= num)
			{
				anglePower += num2 * Time.deltaTime;
			}
			else
			{
				anglePower -= num2 * Time.deltaTime;
				if (anglePower < 0f)
				{
					anglePower = 0f;
				}
			}
		}
		else if (anglePower < 0f)
		{
			float num3 = (num < 0f) ? 2f : 3f;
			if (anglePower >= num)
			{
				anglePower -= num3 * Time.deltaTime;
			}
			else
			{
				anglePower += num3 * Time.deltaTime;
				if (anglePower > 0f)
				{
					anglePower = 0f;
				}
			}
		}
		else
		{
			anglePower += num * Time.deltaTime;
		}
		Vector3 zero = Vector3.zero;
		zero.y = CalcManager.Rot(Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f) * new Vector3(anglePower, 0f, 1f), CalcManager.AXIS.Y);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(zero), Mathf.Lerp(2f, 1.5f, SpeedLerp) * Time.deltaTime);
	}
	private void ShakeDirection()
	{
		isShake = true;
		isMikoshiPointCamera = true;
		rigid.isKinematic = true;
		if (isPlayer)
		{
			SingletonCustom<MikoshiRaceUiManager>.Instance.ViewShakeControlInfo(carNo);
		}
		MikoshiPointRotateDirection(mikoshiPointDir, delegate
		{
			StopRunEffect();
		});
		mikoshiPointSpaceNo = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.mikoshiPointSpaceData[mikoshiPointNo - 1].SearchNearestEmptySpaceNo(base.transform.position);
		SingletonCustom<MikoshiRaceGameManager>.Instance.Course.mikoshiPointSpaceData[mikoshiPointNo - 1].AddCount(mikoshiPointSpaceNo);
		mikoshiPointSpaceTrans = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.mikoshiPointSpaceData[mikoshiPointNo - 1].GetSpaceTrans(mikoshiPointSpaceNo);
		Vector3 position = mikoshiPointSpaceTrans.position;
		position.y = base.transform.position.y;
		LeanTween.move(base.gameObject, position, 0.5f).setEaseOutQuad().setOnUpdate((Action<float>)delegate
		{
			MoveAnimation();
		});
		LeanTween.delayedCall(1f, (Action)delegate
		{
			isShakeCtrl = true;
		});
	}
	public void ShakeControlAction()
	{
		if (shakePushCount < 40)
		{
			shakePushCount++;
			if (isPlayer)
			{
				SingletonCustom<MikoshiRaceUiManager>.Instance.ShakePush(carNo);
			}
		}
	}
	private void ShakeEnd()
	{
		isShakeCtrl = false;
		mikoshiPointBetweenTime = 0f;
		PlayRunEffect();
		cameraAngleY = 90f - Mathf.Atan2(0f - mikoshiPointDir.z, 0f - mikoshiPointDir.x) * 57.29578f;
		if (isPlayer)
		{
			SingletonCustom<MikoshiRaceUiManager>.Instance.HideShakeControlInfo(carNo);
		}
		isMikoshiPointCamera = false;
		MikoshiPointRotateDirection(-mikoshiPointDir, delegate
		{
			isShake = false;
			shakeValue = 0f;
			shakeCtrlPower = 0f;
			shakePushCount = 0;
			rigid.isKinematic = false;
			SingletonCustom<MikoshiRaceGameManager>.Instance.Course.mikoshiPointSpaceData[mikoshiPointNo - 1].RemoveCount(mikoshiPointSpaceNo);
		});
	}
	private void ShakeGoal()
	{
		if (isPlayer)
		{
			SingletonCustom<MikoshiRaceUiManager>.Instance.HideShakeControlInfo(carNo);
		}
		isShake = false;
		isShakeCtrl = false;
		rigid.isKinematic = false;
		LeanTween.delayedCall(1f, (Action)delegate
		{
			isGoalMove = true;
			LeanTween.delayedCall(0.5f, (Action)delegate
			{
				isGoalMove = false;
			});
		});
		SingletonCustom<MikoshiRaceCarManager>.Instance.CarGoal(carNo);
	}
	private void ResetAnimation(bool _isCommonShoulder = false)
	{
		for (int i = 0; i < bodyPartsCharas.Length; i++)
		{
			bool flag = i % 2 == 1;
			bodyPartsCharas[i].Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 0f, 0f);
			bodyPartsCharas[i].Parts(BodyPartsList.HIP).SetLocalPosition(0f, 0.05483828f, 0f);
			bodyPartsCharas[i].Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
			bodyPartsCharas[i].Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
			bodyPartsCharas[i].Parts(BodyPartsList.LEG_L).SetLocalPosition(-0.054f, -0.0483f, 0f);
			bodyPartsCharas[i].Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
			bodyPartsCharas[i].Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyPartsCharas[i].Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			if (_isCommonShoulder)
			{
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 0f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 0f);
			}
			else if (flag)
			{
				bodyPartsCharas[i].Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, -15f, 0f);
				bodyPartsCharas[i].Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 30f, 0f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.130856f, 0.1297733f, 0.07554801f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(0.01932816f, 0.1297733f, 0.1266301f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(320f, 150f, 240f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(320f, 150f, 220f);
			}
			else
			{
				bodyPartsCharas[i].Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 15f, 0f);
				bodyPartsCharas[i].Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, -30f, 0f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(-0.01932815f, 0.1297733f, 0.1266301f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.130856f, 0.1297733f, 0.07554803f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(320f, 210f, 120f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(320f, 210f, 140f);
			}
			bodyPartsCharas[i].Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
			bodyPartsCharas[i].Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
		}
	}
	private void MoveAnimation()
	{
		for (int i = 0; i < bodyPartsCharas.Length; i++)
		{
			bodyPartsCharas[i].Parts(BodyPartsList.LEG_L).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * 30f);
			bodyPartsCharas[i].Parts(BodyPartsList.LEG_R).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * -30f);
		}
		bool flag = false;
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
		else if (Mathf.Abs(anglePower) > 0.01f)
		{
			runAnimationTime += Mathf.Abs(anglePower) * runAnimationSpeed * 0.05f * Time.deltaTime;
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
			if (!isChangeAnimationNeutral)
			{
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
			flag = true;
		}
		if (!isShake && isPlayRunEffect == flag)
		{
			if (isPlayRunEffect)
			{
				StopRunEffect();
			}
			else
			{
				PlayRunEffect();
			}
		}
	}
	private void ShakeAnimationTween()
	{
		LeanTween.value(base.gameObject, 0f, 2f, 1.5f).setOnUpdate(delegate(float _value)
		{
			float num = _value % 1f;
			num *= 2f;
			if (num > 1f)
			{
				num = 2f - num;
			}
			ShakeAnimation(num);
		}).setOnComplete((Action)delegate
		{
			ShakeEnd();
		});
	}
	private void ShakeAnimation(float _animLerp)
	{
		heaveAnchor.SetLocalPositionY(Mathf.Lerp(0f, 0.1f, _animLerp));
		for (int i = 0; i < bodyPartsCharas.Length; i++)
		{
			bodyPartsCharas[i].Parts(BodyPartsList.BODY).SetLocalEulerAnglesX(Mathf.Lerp(0f, -10f, _animLerp));
			if (i % 2 == 1)
			{
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.130856f, Mathf.Lerp(0.1297733f, 0.18f, _animLerp), 0.07554801f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(0.01932816f, Mathf.Lerp(0.1297733f, 0.18f, _animLerp), 0.1266301f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(320f, 150f, 360f - Mathf.Lerp(120f, 135f, _animLerp));
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(320f, 150f, 360f - Mathf.Lerp(140f, 155f, _animLerp));
			}
			else
			{
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(-0.01932815f, Mathf.Lerp(0.1297733f, 0.18f, _animLerp), 0.1266301f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.130856f, Mathf.Lerp(0.1297733f, 0.18f, _animLerp), 0.07554803f);
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(320f, 210f, Mathf.Lerp(120f, 135f, _animLerp));
				bodyPartsCharas[i].Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(320f, 210f, Mathf.Lerp(140f, 155f, _animLerp));
			}
		}
	}
	private void MikoshiPointRotateDirection(Vector3 _rotateForward, Action _endCallback = null)
	{
		Vector3 forward = base.transform.forward;
		Vector3 vector = Vector3.Cross(forward, _rotateForward);
		float num = Vector3.Angle(forward, _rotateForward) * (float)((!(vector.y < 0f)) ? 1 : (-1));
		float time = Mathf.Abs(num) / 180f;
		LeanTween.rotateAround(base.gameObject, Vector3.up, num, time).setOnUpdate((Action<float>)delegate
		{
			if (!isMikoshiPointCamera)
			{
				MoveAnimation();
			}
			if (SingletonCustom<MikoshiRaceGameManager>.Instance.IsGameEnd)
			{
				LeanTween.cancel(base.gameObject);
			}
		}).setOnComplete((Action)delegate
		{
			_endCallback();
		});
	}
	private void PlaySeRun()
	{
		if (isPlayer && !isGoal)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.5f);
		}
	}
	public void RaceSePlay(string _seName)
	{
		if (isGoal)
		{
			return;
		}
		if (isPlayer)
		{
			SingletonCustom<MikoshiRaceCarManager>.Instance.AllSePlayCheck(_seName);
		}
		else if (SingletonCustom<MikoshiRaceGameManager>.Instance.PlayerNum == 1)
		{
			float num = Vector3.Distance(base.transform.position, SingletonCustom<MikoshiRaceCarManager>.Instance.GetPlayerOnePosition());
			if (num < 4f)
			{
				SingletonCustom<MikoshiRaceCarManager>.Instance.AllSePlayCheck(_seName, 0.9f - num / 4f);
			}
		}
	}
	public void PlayRunEffect()
	{
		isPlayRunEffect = true;
		for (int i = 0; i < runEffects.Length; i++)
		{
			runEffects[i].Play();
		}
	}
	public void StopRunEffect()
	{
		isPlayRunEffect = false;
		for (int i = 0; i < runEffects.Length; i++)
		{
			runEffects[i].Stop();
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
		cameraAngleY += angleDiff * 1.5f * Time.fixedDeltaTime;
		if (cameraAngleY < 0f)
		{
			cameraAngleY += 360f;
		}
		else if (cameraAngleY >= 360f)
		{
			cameraAngleY -= 360f;
		}
		float num = 0f;
		Vector3 velocity = rigid.velocity;
		velocity.y = 0f;
		num = (0f - velocity.magnitude) / 5f;
		float num2 = num - cameraSpeedZValue;
		if (Mathf.Abs(num2) < 0.02f)
		{
			cameraSpeedZValue = num;
		}
		else
		{
			cameraSpeedZValue += num2 * Time.fixedDeltaTime * 3f;
		}
	}
	public void ChangeCameraPointRight()
	{
		cameraPointNo++;
		if (cameraPointNo >= cameraPoints.Length)
		{
			cameraPointNo -= cameraPoints.Length;
		}
		cameraTarget.position = cameraPoints[cameraPointNo].position;
	}
	public void ChangeCameraPointLeft()
	{
		cameraPointNo--;
		if (cameraPointNo < 0)
		{
			cameraPointNo += cameraPoints.Length;
		}
		cameraTarget.position = cameraPoints[cameraPointNo].position;
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
	public void AddWaterGauge()
	{
		waterGauge += 0.3f;
		if (waterGauge > 1f)
		{
			waterGauge = 1f;
		}
	}
	private int LapCalc()
	{
		return Mathf.FloorToInt(nowRunDistance / SingletonCustom<MikoshiRaceGameManager>.Instance.CircuitLength + 0.5f);
	}
	private void OnTriggerEnter(Collider _collider)
	{
		if (_collider.gameObject.tag == "Goal" && !isGoal && mikoshiPointNo == SingletonCustom<MikoshiRaceGameManager>.Instance.Course.mikoshiPointColliders.Length)
		{
			SingletonCustom<MikoshiRaceCarManager>.Instance.CarGoal(carNo);
		}
		if (_collider.gameObject.tag == "GoalHoop" && SingletonCustom<MikoshiRaceGameManager>.Instance.Course.SearchMikoshiPointNo(_collider) == mikoshiPointNo)
		{
			mikoshiPointCameraDistance = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.GetMikoshiPointCameraTrans(mikoshiPointNo).position - _collider.transform.position;
			mikoshiPointDir = _collider.transform.forward;
			Vector3 normalized = mikoshiPointCameraDistance;
			normalized.y = 0f;
			normalized = normalized.normalized;
			float d = Vector3.Dot(normalized, mikoshiPointDir);
			if (Vector3.Cross(normalized, mikoshiPointDir).y > 0f)
			{
				float z = normalized.z;
				normalized.z = normalized.x;
				normalized.x = 0f - z;
			}
			else
			{
				float z2 = normalized.z;
				normalized.z = 0f - normalized.x;
				normalized.x = z2;
			}
			mikoshiPointCameraLeaveVec = normalized * d;
			mikoshiPointNo++;
			mikoshiPointArrivalTime = Time.time;
			SingletonCustom<MikoshiRaceCarManager>.Instance.GuideArrowChange(carNo);
			if (isPlayer)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
			}
			ShakeDirection();
		}
		if (_collider.gameObject.tag == "GoalBarrier" && Vector3.Dot(_collider.transform.forward, rigid.velocity) > 0f)
		{
			int num = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.SearchMikoshiPointRoadNo(_collider);
			if (num == mikoshiPointNo)
			{
				isMikoshiPointRoad = true;
				mikoshiPointRoadNo = num + 1;
				mikoshiPointRoadDir = _collider.transform.forward;
				mikoshiPointRoadPos = _collider.transform.position;
				mikoshiPointSpaceSearchTime = 0f;
				mikoshiPointSpaceTrans = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.mikoshiPointSpaceData[mikoshiPointNo].SearchNearestEmptySpaceTrans(base.transform.position);
			}
		}
		if (!isPlayer && _collider.gameObject.tag == "GoalNet" && Vector3.Dot(_collider.transform.forward, rigid.velocity) < 0f)
		{
			isCpuReverseGuard = true;
			reverseGuardEnterDis = wptAi.progressDistance;
		}
	}
	private void OnTriggerExit(Collider _collider)
	{
		if (_collider.gameObject.tag == "GoalBarrier" && Vector3.Dot(_collider.transform.forward, rigid.velocity) < 0f)
		{
			isMikoshiPointRoad = false;
		}
		if (!isPlayer && _collider.gameObject.tag == "GoalNet")
		{
			isCpuReverseGuard = false;
		}
	}
	private void OnCollisionEnter(Collision _collision)
	{
		if (_collision.gameObject.tag == "Floor" || _collision.gameObject.tag == "Field")
		{
			return;
		}
		if (isPlayer && !isGoal && SingletonCustom<MikoshiRaceGameManager>.Instance.IsGameNow)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerNo);
		}
		if (_collision.gameObject.tag == "VerticalWall" && !isGoal)
		{
			RaceSePlay("se_bump01");
		}
		if (_collision.gameObject.tag == "Player")
		{
			if (!isGoal)
			{
				RaceSePlay("se_bump01");
			}
			if (!_collision.rigidbody.isKinematic)
			{
				Bound(_collision);
			}
		}
	}
	private void Bound(Collision _collision)
	{
		Vector3 a = base.transform.position - _collision.contacts[0].point;
		a.y = 0f;
		a = a.normalized;
		float num = _collision.relativeVelocity.magnitude;
		if (num > 8f)
		{
			num = 8f;
		}
		rigid.velocity += a * num;
		if (rigid.velocity.sqrMagnitude > 100f)
		{
			rigid.velocity = rigid.velocity.normalized * 10f;
		}
	}
	public bool CheckInCameraLeaveRange(Vector3 _pos)
	{
		Vector3 vector = _pos - base.transform.position;
		vector.y = 0f;
		return vector.sqrMagnitude <= 4f;
	}
	public bool CheckInCameraLeaveRange(Vector3 _pos, out float _sqrLerp)
	{
		Vector3 vector = _pos - base.transform.position;
		vector.y = 0f;
		_sqrLerp = vector.sqrMagnitude / 4f;
		return _sqrLerp <= 1f;
	}
	public bool CheckInCameraMikoshiPointLeaveRange(Vector3 _pos)
	{
		Vector3 lhs = _pos - base.transform.position;
		lhs.y = 0f;
		if (Vector3.Dot(lhs, mikoshiPointDir) < 0f)
		{
			return lhs.sqrMagnitude <= 4f;
		}
		return false;
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
	public Transform GetCameraTarget()
	{
		return cameraTarget;
	}
	public Vector3 GetCameraStartDirectionPos(float _lerp)
	{
		Vector3 point = Vector3.Lerp(new Vector3(0f, 0.75f, -1.5f), cameraTarget.localPosition, _lerp);
		return base.transform.position + Quaternion.Euler(0f, cameraAngleY + (1f - _lerp) * 180f, 0f) * point * cameraTarget.lossyScale.x;
	}
	public Vector3 GetCameraPosition()
	{
		Vector3 localPosition = cameraTarget.localPosition;
		if (cameraPointNo != 2)
		{
			localPosition.z += cameraSpeedZValue;
		}
		return base.transform.position + Quaternion.Euler(0f, cameraAngleY, 0f) * localPosition * cameraTarget.lossyScale.x;
	}
	public Vector3 GetCameraLookAtPos()
	{
		return cameraLookAtPoints[cameraPointNo].position;
	}
	public Vector3 GetCameraLookAtDefaultPos()
	{
		return cameraLookAtPoints[0].position;
	}
	public Vector3 GetCameraMikoshiPointPos()
	{
		return GetCameraLookAtDefaultPos() + mikoshiPointCameraDistance;
	}
	public float GetCameraMikoshiPointDirDot(Vector3 _pos)
	{
		Vector3 vector = base.transform.position - _pos;
		vector.y = 0f;
		return Vector3.Dot(vector.normalized, mikoshiPointDir);
	}
	public Vector3 GetCameraMikoshiPointLeaveVec()
	{
		return mikoshiPointCameraLeaveVec;
	}
	public void SetCameraRotAnchor(Transform _anchor)
	{
		cameraRotAnchor = _anchor;
	}
	public void AiDataInit()
	{
		if (!isPlayer)
		{
			int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
			int num = (carNo - 1) % 3;
			maxSpeedInit += CPU_ADD_MAX_SPEED[aiStrength][num];
			gripAngleSpeedInit *= CPU_MAG_MAX_STEER_ANGLE[aiStrength][num];
			aiShakeActionInterval = CPU_SHAKE_ACTION_INTERVAL[aiStrength][num];
		}
	}
	public void AiMove()
	{
		if (!SingletonCustom<MikoshiRaceGameManager>.Instance.IsGameStart)
		{
			return;
		}
		if (isGoal)
		{
			SetControlData(0f, _isAccel: false, _isBack: false);
			return;
		}
		if (isShakeCtrl)
		{
			aiShakeActionTime += Time.deltaTime;
			if (aiShakeActionTime > aiShakeActionInterval)
			{
				aiShakeActionTime -= aiShakeActionInterval;
				ShakeControlAction();
			}
		}
		if (isShake)
		{
			return;
		}
		if (IsOneRoadGo)
		{
			mikoshiPointSpaceSearchTime += Time.deltaTime;
			if (mikoshiPointSpaceSearchTime > 0.5f)
			{
				mikoshiPointSpaceSearchTime -= 0.5f;
				mikoshiPointSpaceTrans = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.mikoshiPointSpaceData[mikoshiPointNo].SearchNearestEmptySpaceTrans(base.transform.position);
			}
			aiDir = Quaternion.Euler(0f, 0f - base.transform.eulerAngles.y, 0f) * mikoshiPointRoadDir;
			float num = aiX = aiDir.x;
			Vector3 lhs = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f) * Vector3.forward;
			Vector3 rhs = mikoshiPointRoadDir;
			if (mikoshiPointSpaceTrans != null)
			{
				rhs = mikoshiPointSpaceTrans.position - base.transform.position;
			}
			rhs.y = 0f;
			aiDis = rhs.magnitude;
			aiCross = Vector3.Cross(lhs, rhs);
		}
		else
		{
			aiDir = Quaternion.Euler(0f, 0f - base.transform.eulerAngles.y, 0f) * wptAi.target.forward;
			float num2 = aiX = aiDir.x;
			Vector3 lhs2 = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f) * Vector3.forward;
			Vector3 rhs2 = wptAi.progressPoint.position - base.transform.position;
			rhs2.y = 0f;
			aiDis = rhs2.magnitude;
			aiCross = Vector3.Cross(lhs2, rhs2);
		}
		float num3 = Mathf.Clamp(aiX * 2.5f + aiCross.y * 1f, -1f, 1f);
		if (!isAiBack)
		{
			Vector3 velocity = rigid.velocity;
			velocity.y = 0f;
			if (!isShake && velocity.sqrMagnitude < 0.25f)
			{
				aiStopTime += Time.deltaTime;
				if (aiStopTime > 1f)
				{
					isAiBack = true;
					aiStopTime = 0f;
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
				num3 = 0f;
			}
		}
		if (!isAiBack)
		{
			SetControlData(num3, _isAccel: true, _isBack: false);
		}
		else
		{
			SetControlData(0f - num3, _isAccel: false, _isBack: true);
		}
	}
}
