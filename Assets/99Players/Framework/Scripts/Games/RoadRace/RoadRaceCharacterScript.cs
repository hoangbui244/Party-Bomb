using System;
using System.Collections;
using UnityEngine;
public class RoadRaceCharacterScript : MonoBehaviour
{
	public enum GravityType
	{
		NORMAL,
		AIR,
		DOWN
	}
	public enum BRAKE_TYPE
	{
		NORMAL,
		AIR,
		OVER_SPEED,
		GOAL,
		CONTACT_WALL
	}
	[Serializable]
	public struct EffectData
	{
		[Header("エフェクトアンカ\u30fc")]
		public Transform effectAnchor;
		[Header("高速エフェクト")]
		public ParticleSystem speedEffect;
		private ParticleSystem.EmissionModule speedEffectEM;
		private ParticleSystem.MainModule speedEffectMM;
		private float speedUpEffectRateDef;
		[Header("アクションエフェクト")]
		public ParticleSystem actionEffect;
		[Header("衝撃波エフェクト")]
		public ParticleSystem shockWaveEffect;
		[Header("アクションダッシュ")]
		public ParticleSystem actionDash;
		public void Init()
		{
			speedEffectEM = speedEffect.emission;
			speedUpEffectRateDef = speedEffectEM.rateOverTime.constant;
			speedEffectEM.rateOverTime = speedUpEffectRateDef;
			speedEffectMM = speedEffect.main;
			speedEffectMM.loop = false;
		}
		public void SetSpeedUpEffectRate(float _overSpeedValue)
		{
			if (_overSpeedValue <= 0f)
			{
				_overSpeedValue = 0f;
				speedEffectMM.loop = false;
				return;
			}
			speedEffectEM.rateOverTime = _overSpeedValue;
			if (!speedEffectMM.loop)
			{
				speedEffect.Play();
				speedEffectMM.loop = true;
			}
		}
		public void PlaySpeedUp()
		{
			speedEffect.Play();
			speedEffectMM.loop = true;
		}
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
	public enum ActionType
	{
		HAND_FREE,
		WHEELIE,
		TURN_AROUND,
		GUTS_POSE,
		V_POSE,
		MAX
	}
	[Serializable]
	public struct AiStrengthData
	{
		public int dashActionPer;
		public float waterSpeedUpCorr;
		public float speedCorr;
		public AiStrengthData(int _dashActionPer, float _speedCorr, float _waterSpeedUpCorr)
		{
			dashActionPer = _dashActionPer;
			speedCorr = _speedCorr;
			waterSpeedUpCorr = _waterSpeedUpCorr;
		}
		public void Copy(AiStrengthData _data)
		{
			dashActionPer = _data.dashActionPer;
			waterSpeedUpCorr = _data.waterSpeedUpCorr;
			speedCorr = _data.speedCorr;
		}
	}
	protected float[] GRAVITY = new float[3]
	{
		9.8f,
		9.8f,
		7.35000038f
	};
	protected const float JUMP_POWER = 2f;
	protected float[] DASH_POWER = new float[2]
	{
		7.5f,
		10f
	};
	protected float CRASH_SE_INTERVAL = 0.1f;
	protected const float WHEEL_DIAMETER = 0.15f;
	protected float MOVE_SPEED = 2.625f;
	protected float MOVE_SPEED_MAX_MAG = 1.5f;
	protected float MOVE_OVER_SPEED_MAX_MAG = 2.5f;
	protected float MOVE_SPEED_MIN = 0.5f;
	protected float[] CALL_SPEED_MAG = new float[3]
	{
		1f,
		1.2f,
		1.35f
	};
	protected float[] CALL_ROT_MAG = new float[3]
	{
		1f,
		1.1f,
		1.25f
	};
	protected float[] BRAKE_MAG = new float[5]
	{
		0.985f,
		0.975f,
		0.985f,
		0.9f,
		0.85f
	};
	protected float BRAKE_INTERVAL = 0.0051f;
	protected float BRAKE_EFFECT_TIME = 1f;
	protected float ACtION_TIME = 1f;
	protected float ACtION_INTERVAL = 0.2f;
	protected float DASH_INTERVAL = 0.1f;
	protected float[] OVEAR_SPEED_TIME = new float[2]
	{
		1f,
		0.25f
	};
	protected float TILT_BODY_SPEED = 5f;
	protected float ROT_SPEED_ACCEL = 1.75f;
	protected float ROT_SPEED_ACCEL_MAX = 1f;
	protected float ROT_SPEED_ACCEL_INTERVAL = 0.2f;
	protected float CONTROLL_TOLE_ROT = 15f;
	public readonly Vector3 TILT_BODY_ROT = new Vector3(0f, 15f, 5f);
	protected float TILT_HANDLE_ROT = 30f;
	protected float TILT_BICYCLE_ROT = 15f;
	protected int playerNo;
	protected int charaNo;
	private RoadRaceDefine.UserType userType;
	private bool isCpu;
	[SerializeField]
	[Header("リジッドボディ")]
	protected Rigidbody rigid;
	protected Transform transformIns;
	protected Transform startAnchor;
	[SerializeField]
	[Header("重心アンカ\u30fc")]
	protected Transform centerOffMassAnchor;
	protected Vector3[] calcVec = new Vector3[2];
	protected Vector3 rot;
	protected Vector3 prevPos;
	protected Vector3 nowPos;
	protected Vector3 prevVec;
	protected Vector3 nowVec;
	protected float moveAccel = 1f;
	protected float moveMaxSpeedMag = 1f;
	protected float moveMaxSpeed = 1f;
	protected float moveMaxOverSpeed = 1f;
	protected float rotSpeedMag = 1.25f;
	protected float rotDir = 1f;
	protected float rotSpeedAccel;
	protected float rotSpeedAccelInterval;
	protected CapsuleCollider charaCollider;
	protected float charaBodySize;
	protected float charaHeight;
	[SerializeField]
	[Header("オブジェクト")]
	protected Transform obj;
	protected bool isNotControll = true;
	protected float notControllTime;
	protected bool isGrounded = true;
	protected int dirtColCount;
	protected bool isRunSe;
	protected float reboundTime = -1f;
	protected float speed;
	protected Vector3 v3speed;
	public bool isDontMove = true;
	[SerializeField]
	[Header("エフェクトデ\u30fcタ")]
	private EffectData effectData;
	protected int uniformNo = -1;
	protected int byicycleNo = -1;
	protected string charaName;
	[SerializeField]
	[Header("アクションアンカ\u30fc")]
	protected Transform actionAnchor;
	[SerializeField]
	[Header("スタイル")]
	private CharacterStyle style;
	[SerializeField]
	[Header("アニメ\u30fcション")]
	protected RoadRaceAnimation animation;
	protected RoadRaceBicycleScript bicycle;
	protected RoadRaceDefine.PlayBicycleType bicycleType;
	[SerializeField]
	[Header("一輪車アンカ\u30fc")]
	protected Transform unicycleAnchor;
	[SerializeField]
	[Header("ペダル位置アンカ\u30fc")]
	protected Transform[] pedalPosAnchor;
	protected float wheelDistance;
	[SerializeField]
	[Header("タイヤトレイル")]
	protected TrailRenderer trailRenderer;
	protected TrailRenderer[] trailRenderers;
	[SerializeField]
	[Header("接地確認用アンカ\u30fc")]
	protected BoxCollider rayAnchor;
	[SerializeField]
	[Header("キャラサイズアンカ\u30fc")]
	protected SphereCollider bodySizeAnchor;
	protected int lapNum;
	protected Collider[] raceCheckPoint;
	protected int checkPointNo;
	protected int checkPointNext;
	protected int checkPointIdx;
	protected int nowRank;
	protected float goalTime = -1f;
	protected bool isReverse;
	protected float reverseTime;
	protected bool reverseCheckPos;
	protected bool isGoal;
	protected bool isRearLook;
	protected ActionType actionType;
	protected bool isAction;
	protected bool isJump;
	protected float actionTime;
	protected float actionInterval;
	protected bool isDash;
	protected bool isOverSpeed;
	protected float dashInterval;
	protected float overSpeedTime;
	protected GameObject hitActionPanel;
	protected float ACTION_PANEL_RESET_TIME = 0.5f;
	protected float actionPanelResetTime;
	protected const float WATER_REDUCE_TIME = 2f;
	protected const float WATER_USE = 0.05f;
	protected const float WATER_SPEED_UP_MAG = 0.25f;
	protected RoadRaceWaters waterCreater;
	protected float waterGage;
	protected bool isWaterRide;
	protected int lastCheckPointNo;
	protected float checkPointDistPrev;
	protected float checkPointDistNow;
	protected float reverseRunTime;
	protected float crashSeTime;
	protected float brakeTime;
	protected float afterGoalTime;
	protected bool isInertiaAfterGoal;
	protected bool isPlaySe = true;
	protected bool isAiBrake;
	protected Vector3 aiPointPos = Vector3.zero;
	protected int aiPointNo;
	protected AiStrengthData[] aiStrengthDatas = new AiStrengthData[3]
	{
		new AiStrengthData(50, 0.85f, 0.65f),
		new AiStrengthData(70, 0.925f, 0.75f),
		new AiStrengthData(100, 1f, 0.85f)
	};
	private AiStrengthData aiStrengthData;
	private GameObject aiActionDashPanel;
	protected int aiStrength;
	public int PlayerNo
	{
		get
		{
			return playerNo;
		}
		set
		{
			playerNo = value;
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
	public RoadRaceDefine.UserType UserType => userType;
	public bool IsCpu => isCpu;
	public Rigidbody Rigid => rigid;
	public Transform StartAnchor => startAnchor;
	public float MoveMaxSpeed => moveMaxSpeed * WaterSpeedUpMag();
	public float MoveMaxOverSpeed => moveMaxOverSpeed;
	public float RotSpeedAccel
	{
		set
		{
			rotSpeedAccel = value;
		}
	}
	public string CharaName
	{
		get
		{
			return charaName;
		}
		set
		{
			charaName = value;
		}
	}
	public CharacterStyle Style => style;
	public TrailRenderer[] TrailRenderers => trailRenderers;
	public int LapNum => lapNum;
	public int CheckPointNo
	{
		get
		{
			return checkPointNo;
		}
		set
		{
			checkPointNo = value;
		}
	}
	public int CheckPointNoIdx => checkPointNo % raceCheckPoint.Length;
	public int CheckPointNext
	{
		get
		{
			return checkPointNext;
		}
		set
		{
			checkPointNext = value;
		}
	}
	public int CheckPointIdx => checkPointIdx;
	public bool IsGoal => isGoal;
	public bool IsRearLook
	{
		get
		{
			return isRearLook;
		}
		set
		{
			isRearLook = value;
		}
	}
	public bool IsInertiaAfterGoal
	{
		get
		{
			return isInertiaAfterGoal;
		}
		set
		{
			isInertiaAfterGoal = value;
		}
	}
	public bool IsPlaySe
	{
		set
		{
			isPlaySe = value;
		}
	}
	public float GetGoalTime()
	{
		return goalTime;
	}
	public int GetNowRank()
	{
		return nowRank;
	}
	public void Init(int _charaNo, Collider[] _raceCheckPoint, Transform _startAnchor, int _class = 0)
	{
		UnityEngine.Debug.Log("Init : " + _charaNo.ToString());
		charaNo = _charaNo;
		playerNo = charaNo;
		isCpu = (CharaNo >= SingletonCustom<GameSettingManager>.Instance.PlayerNum);
		userType = (RoadRaceDefine.UserType)(IsCpu ? (4 + (CharaNo - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : CharaNo);
		if (isCpu)
		{
			playerNo = -1;
		}
		uniformNo = _charaNo;
		ChangeUniform();
		CreateByicycle(SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]);
		base.gameObject.name = base.gameObject.name + "_" + _charaNo.ToString();
		transformIns = base.transform;
		startAnchor = _startAnchor;
		transformIns.position = startAnchor.position;
		transformIns.eulerAngles = startAnchor.eulerAngles;
		nowPos = (prevPos = transformIns.position);
		prevVec = (nowVec = CalcManager.mVector3Zero);
		Rigid.centerOfMass *= 0.75f;
		Rigid.maxAngularVelocity = 1000f;
		if (bodySizeAnchor != null)
		{
			charaBodySize = bodySizeAnchor.radius * 2f;
		}
		if (IsCpu)
		{
			AiInit();
		}
		else
		{
			aiStrengthData.speedCorr = 1f;
			aiStrengthData.waterSpeedUpCorr = 1f;
		}
		raceCheckPoint = _raceCheckPoint;
		InitCheckPointData();
		nowRank = -1;
		waterGage = 0f;
		goalTime = 0f;
		moveMaxSpeedMag = 1f;
		moveAccel = 1f;
		MOVE_SPEED *= CALL_SPEED_MAG[_class];
		moveMaxSpeed = MOVE_SPEED * MOVE_SPEED_MAX_MAG;
		moveMaxSpeed *= moveMaxSpeedMag;
		MOVE_SPEED *= aiStrengthData.speedCorr;
		moveMaxSpeed *= aiStrengthData.speedCorr;
		MOVE_SPEED_MIN = moveMaxSpeed * MOVE_SPEED_MIN;
		moveMaxOverSpeed = moveMaxSpeed * MOVE_OVER_SPEED_MAX_MAG;
		ROT_SPEED_ACCEL *= CALL_ROT_MAG[_class];
		ROT_SPEED_ACCEL *= moveMaxSpeedMag;
		GRAVITY[1] *= CALL_SPEED_MAG[_class];
		SetLayerRecursively(unicycleAnchor.gameObject, RoadRaceDefine.ConvertLayerNo("Character"));
		InitAnimation();
		if (waterCreater != null)
		{
			waterCreater.Init();
		}
		effectData.Init();
		charaHeight = CalcManager.Length(transformIns.position, animation.Part(CharacterParts.BodyPartsList.HEAD).position);
	}
	public void SetLayerRecursively(GameObject _self, int _layer)
	{
		_self.layer = _layer;
		foreach (Transform item in _self.transform)
		{
			SetLayerRecursively(item.gameObject, _layer);
		}
	}
	public void UpdateMethod(bool _isTutorial = false)
	{
		crashSeTime -= Time.deltaTime;
		prevPos = nowPos;
		nowPos = transformIns.position;
		prevVec = nowVec;
		nowVec = Rigid.velocity;
		Vector3 vector = nowPos - prevPos;
		vector.y = 0f;
		Vector3 up = Vector3.up;
		Vector3 from = Vector3.ProjectOnPlane(vector, up);
		Vector3 to = Vector3.ProjectOnPlane(base.transform.forward, up);
		if (Vector3.Angle(from, to) < 90f)
		{
			wheelDistance += vector.magnitude;
		}
		else
		{
			wheelDistance -= vector.magnitude;
		}
		float num = 213f / 452f;
		float x = wheelDistance % num / num * 360f;
		for (int i = 0; i < bicycle.wheels.Length; i++)
		{
			bicycle.wheels[i].SetLocalEulerAngles(x, 0f, 0f);
		}
		float num2 = 4f;
		x = wheelDistance % (num * num2) / (num * num2) * 360f;
		for (int j = 0; j < bicycle.cranks.Length; j++)
		{
			if (!(bicycle.cranks[j] == null))
			{
				bicycle.cranks[j].SetLocalEulerAngles(x, 0f, 0f);
			}
		}
		AnimPedal();
		if (!isWaterRide)
		{
			waterGage -= 0.5f * Time.deltaTime;
			if (waterGage < 0f)
			{
				waterGage = 0f;
			}
		}
		isWaterRide = false;
		CheckGrounded();
		LimitRotation();
		if (isNotControll)
		{
			notControllTime += Time.deltaTime;
		}
		else
		{
			notControllTime = 0f;
		}
		if (isAction)
		{
			actionTime -= Time.deltaTime;
			if (actionTime <= 0f)
			{
				ResetActionMotion();
			}
			actionInterval -= Time.deltaTime;
		}
		dashInterval -= Time.deltaTime;
		if (waterCreater != null)
		{
			waterCreater.UpdateMethod();
		}
		if (Scene_RoadRace.Floor(Rigid.velocity.magnitude, 2f) > Scene_RoadRace.Floor(moveMaxSpeed * WaterSpeedUpMag(), 2f) || Rigid.velocity.normalized.y < -0.25f)
		{
			isOverSpeed = true;
			if (Rigid.velocity.magnitude > moveMaxOverSpeed)
			{
				Rigid.velocity = Rigid.velocity.normalized * moveMaxOverSpeed;
			}
		}
		else
		{
			overSpeedTime -= Time.deltaTime;
			if (overSpeedTime <= 0f)
			{
				isDash = false;
				isOverSpeed = false;
			}
		}
		if (IsGoal)
		{
			afterGoalTime += Time.deltaTime;
		}
		brakeTime -= Time.deltaTime;
		if (brakeTime <= 0f)
		{
			if (isGrounded)
			{
				if (IsGoal)
				{
					if (!IsInertiaAfterGoal && afterGoalTime >= 3f)
					{
						Rigid.velocity *= GetBrakeMag(BRAKE_TYPE.GOAL);
					}
				}
				else if (!isNotControll || notControllTime >= BRAKE_EFFECT_TIME)
				{
					if (isOverSpeed)
					{
						Rigid.velocity *= GetBrakeMag(BRAKE_TYPE.OVER_SPEED);
					}
					else
					{
						Rigid.velocity *= GetBrakeMag(BRAKE_TYPE.NORMAL);
					}
				}
			}
			else
			{
				Rigid.velocity *= GetBrakeMag(BRAKE_TYPE.AIR);
			}
			brakeTime = BRAKE_INTERVAL;
		}
		effectData.SetSpeedUpEffectRate(IsGoal ? 0f : Rigid.velocity.sqrMagnitude);
		rotSpeedAccelInterval -= Time.deltaTime;
		CheckShowReverse();
		if (GetPos().y < Scene_RoadRace.FM.StageData.transform.position.y - GetCharaHeight())
		{
			rigid.velocity = CalcManager.mVector3Zero;
			rigid.angularVelocity = CalcManager.mVector3Zero;
			rigid.position = raceCheckPoint[lastCheckPointNo].transform.position + Vector3.up * GetCharaHeight() * 1.5f;
			UnityEngine.Debug.Log("No." + charaNo.ToString() + " : 場外に落ちた : " + lastCheckPointNo.ToString());
		}
		actionPanelResetTime -= Time.deltaTime;
		if (actionPanelResetTime <= 0f)
		{
			hitActionPanel = null;
		}
	}
	private void AnimPedal()
	{
		for (int i = 0; i < bicycle.pedals.Length; i++)
		{
			if (!(bicycle.pedals[i] == null))
			{
				bicycle.pedals[i].forward = actionAnchor.forward;
				float num = CalcManager.Rot(animation.Part(7 + i).parent.InverseTransformPoint(bicycle.pedalAnchors[i].position), CalcManager.AXIS.X);
				animation.Part(7 + i).SetLocalEulerAngles(315f + (0f - Mathf.Min((bicycle.pedals[i].position.y - bicycle.cranks[i].position.y) / 0.03f, 1f)) * 45f, 0f, 0f);
				num = CalcManager.Rot(animation.LegSubParts[i].parent.InverseTransformPoint(bicycle.pedalAnchors[i].position), CalcManager.AXIS.X);
				animation.LegSubParts[i].SetLocalEulerAngles(num + 180f, 0f, 0f);
			}
		}
	}
	private void FixedUpdate()
	{
		if (isGrounded)
		{
			if (Rigid.velocity.y >= 0f)
			{
				rigid.AddForce(Vector3.down * GRAVITY[0], ForceMode.Acceleration);
			}
			else
			{
				rigid.AddForce(Vector3.down * GRAVITY[2], ForceMode.Acceleration);
			}
		}
		else
		{
			rigid.AddForce(Vector3.down * GRAVITY[1], ForceMode.Acceleration);
		}
	}
	protected float WaterSpeedUpMag()
	{
		return 1f + 0.25f * waterGage;
	}
	protected float WaterSpeedUpMagMax()
	{
		return 1.25f;
	}
	public void AddWaterGage(float _per)
	{
		isWaterRide = true;
		waterGage += 0.05f * _per;
		if (waterGage > aiStrengthData.waterSpeedUpCorr)
		{
			waterGage = aiStrengthData.waterSpeedUpCorr;
		}
	}
	public void Move(Vector3 _moveDir, bool _isAccel)
	{
		if (isDontMove || !isGrounded)
		{
			return;
		}
		isNotControll = !_isAccel;
		MoveRot(_moveDir, isNotControll);
		Vector3 vector = Rigid.velocity;
		if (_isAccel)
		{
			float y = vector.y;
			vector.y = 0f;
			Vector3 a = transformIns.forward.normalized;
			a.y = 0f;
			a = a.normalized;
			float magnitude = Rigid.velocity.magnitude;
			vector = (vector.normalized + a * 0.25f).normalized * Rigid.velocity.magnitude;
			vector.y = y;
			float num = WaterSpeedUpMag();
			if (!isOverSpeed)
			{
				float num2 = RoadRaceDefine.easeInQuad(0f, 1f, Mathf.Min(Mathf.Max(0f, magnitude / moveMaxSpeed), 1f)) * moveMaxSpeed;
				float num3 = MOVE_SPEED * (moveAccel + num2);
				num3 *= num;
				a *= num3;
				vector += a * Time.deltaTime;
				if (vector.magnitude > moveMaxSpeed * WaterSpeedUpMag())
				{
					vector = vector.normalized * moveMaxSpeed * WaterSpeedUpMag();
				}
			}
			if (transformIns.forward.y > 0f && vector.magnitude < MOVE_SPEED_MIN)
			{
				vector = vector.normalized * MOVE_SPEED_MIN;
			}
			Rigid.velocity = vector;
			return;
		}
		if (vector.sqrMagnitude < Mathf.Pow(0.1f, 2f))
		{
			vector = Vector3.zero;
		}
		else
		{
			Vector3 to = vector;
			float y2 = vector.y;
			vector.y = 0f;
			Vector3 vector2 = transformIns.forward.normalized;
			vector2.y = 0f;
			vector2 = vector2.normalized;
			if (Vector3.Angle(vector2, to) >= 90f)
			{
				vector2 = -vector2;
			}
			float num4 = Rigid.velocity.magnitude * 0.975f;
			if (num4 < 0f)
			{
				vector = Vector3.zero;
			}
			else
			{
				vector = (vector.normalized + vector2 * 0.25f).normalized * num4;
				vector.y = y2;
				if (!isOverSpeed && vector.magnitude > moveMaxSpeed * WaterSpeedUpMag())
				{
					vector = vector.normalized * moveMaxSpeed * WaterSpeedUpMag();
				}
			}
		}
		Rigid.velocity = vector;
	}
	protected void MoveRot(Vector3 _moveDir, bool _notControll = false, bool _immediate = false)
	{
		bool flag = false;
		if (Mathf.Abs(_moveDir.x) >= 0.01f)
		{
			rot.y = CalcManager.Rot(_moveDir, CalcManager.AXIS.Y);
			if (!IsCpu)
			{
				rot.y += transformIns.eulerAngles.y;
			}
			float f = Mathf.DeltaAngle(rot.y, transformIns.eulerAngles.y);
			if (Mathf.Abs(f) >= CONTROLL_TOLE_ROT)
			{
				float num = 1f;
				num = ((!(Mathf.Sign(f) > 0f)) ? (-1f) : 1f);
				actionAnchor.transform.SetLocalEulerAnglesZ(Mathf.LerpAngle(actionAnchor.transform.localEulerAngles.z, (TILT_BICYCLE_ROT + GetRotSpeed()) * num, TILT_BODY_SPEED * Time.deltaTime));
				bicycle.handle.SetLocalEulerAnglesY(Mathf.LerpAngle(bicycle.handle.localEulerAngles.y, (0f - TILT_HANDLE_ROT) * num, TILT_BODY_SPEED * Time.deltaTime));
				animation.Part(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesY(Mathf.LerpAngle(animation.Part(CharacterParts.BodyPartsList.BODY).localEulerAngles.y, (0f - (TILT_BODY_ROT.y + GetRotSpeed())) * num, TILT_BODY_SPEED * Time.deltaTime));
				if (effectData.effectAnchor != null)
				{
					effectData.effectAnchor.SetLocalEulerAnglesZ(Mathf.LerpAngle(effectData.effectAnchor.localEulerAngles.z, (0f - (TILT_BODY_ROT.z + GetRotSpeed())) * num, TILT_BODY_SPEED * Time.deltaTime));
				}
			}
			else
			{
				flag = true;
			}
			if (_immediate)
			{
				transformIns.SetEulerAnglesY(rot.y);
			}
			else
			{
				transformIns.SetEulerAnglesY(Mathf.LerpAngle(transformIns.eulerAngles.y, rot.y, GetRotSpeed() * Time.deltaTime));
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			actionAnchor.transform.SetLocalEulerAnglesZ(Mathf.LerpAngle(actionAnchor.transform.localEulerAngles.z, 0f, TILT_BODY_SPEED * Time.deltaTime));
			bicycle.handle.SetLocalEulerAnglesY(Mathf.LerpAngle(bicycle.handle.localEulerAngles.y, 0f, TILT_BODY_SPEED * Time.deltaTime));
			animation.Part(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesY(Mathf.LerpAngle(animation.Part(CharacterParts.BodyPartsList.BODY).localEulerAngles.y, 0f, TILT_BODY_SPEED * Time.deltaTime));
			if (effectData.effectAnchor != null)
			{
				effectData.effectAnchor.SetLocalEulerAnglesY(Mathf.LerpAngle(effectData.effectAnchor.localEulerAngles.y, 0f, TILT_BODY_SPEED * Time.deltaTime));
			}
		}
	}
	protected void LimitRotation()
	{
		float num = Vector3.SignedAngle(Vector3.up, transformIns.forward, transformIns.right);
		num = Mathf.DeltaAngle(0f, (num + 270f) % 360f);
		if (Mathf.Abs(num) > 1f)
		{
			Vector3 vector = default(Vector3);
			vector.x = num * 0.1f;
			vector.y = 0f;
			vector.z = 0f;
			vector = transformIns.rotation * vector;
			vector += Rigid.angularVelocity;
			if (StateBaseClass.CheckPauseFlg(StateBaseClass.PAUSE_LEVEL.NONE))
			{
				Rigid.AddTorque(-vector);
			}
		}
	}
	public void UnicycleRotZ()
	{
	}
	public void UnicycleRotX()
	{
	}
	protected void CheckGrounded()
	{
		Physics.Raycast(rayAnchor.transform.position, Vector3.down, out RaycastHit _, 10f, RoadRaceDefine.ConvertLayerMask("Collision_Obj_1"));
		if (Physics.CheckBox(rayAnchor.transform.position, rayAnchor.size * 0.5f, Quaternion.identity, RoadRaceDefine.ConvertLayerMask("Collision_Obj_1")))
		{
			isGrounded = true;
			for (int i = 0; i < trailRenderers.Length; i++)
			{
				trailRenderers[i].emitting = true;
			}
		}
		else
		{
			isGrounded = false;
			for (int j = 0; j < trailRenderers.Length; j++)
			{
				trailRenderers[j].emitting = false;
			}
		}
	}
	public void Action(bool _actionDash = false)
	{
		if (!(actionInterval > 0f))
		{
			ResetActionMotion();
			notControllTime = 0f;
			actionType = (ActionType)UnityEngine.Random.Range(0, 5);
			switch (actionType)
			{
			case ActionType.HAND_FREE:
			case ActionType.TURN_AROUND:
			case ActionType.GUTS_POSE:
			case ActionType.V_POSE:
				animation.ActionAnimation((int)actionType, 0.1f);
				break;
			case ActionType.WHEELIE:
				Wheelie();
				break;
			}
			isAction = true;
			actionTime = ACtION_TIME;
			actionInterval = ACtION_INTERVAL;
			if (_actionDash || Physics.CheckBox(rayAnchor.transform.position, rayAnchor.size * 0.5f, Quaternion.identity, RoadRaceDefine.ConvertLayerMask("Collision_Obj_2")))
			{
				Dash(null, _action: true);
				effectData.actionEffect.Play();
				effectData.actionDash.Play();
			}
			else if (isGrounded)
			{
				Rigid.velocity += Vector3.up * 2f;
				isJump = true;
			}
		}
	}
	public void ResetActionMotion()
	{
		isAction = false;
		animation.ResetAnimation();
		actionAnchor.SetLocalPosition(0f, 0f, 0f);
		actionAnchor.SetLocalEulerAngles(0f, 0f, 0f);
	}
	public void ResetTiltBody()
	{
		actionAnchor.transform.SetLocalEulerAnglesZ(0f);
		obj.SetLocalEulerAnglesZ(0f);
		if (effectData.effectAnchor != null)
		{
			effectData.effectAnchor.SetLocalEulerAnglesZ(0f);
		}
	}
	protected void Wheelie()
	{
		actionAnchor.RotateAround(trailRenderers[0].transform.position, actionAnchor.right, -45f);
	}
	public void Dash(GameObject _panel = null, bool _action = false)
	{
		if (hitActionPanel != null && _panel == hitActionPanel)
		{
			return;
		}
		if ((bool)_panel)
		{
			actionPanelResetTime = ACTION_PANEL_RESET_TIME;
			hitActionPanel = _panel;
		}
		if (dashInterval > 0f && !_action)
		{
			return;
		}
		dashInterval = DASH_INTERVAL;
		if (_action)
		{
			Rigid.velocity = Rigid.velocity.normalized * DASH_POWER[1] * moveAccel;
		}
		else
		{
			Rigid.velocity = Rigid.velocity.normalized * DASH_POWER[0] * moveAccel;
		}
		if (!IsCpu)
		{
			if (_action)
			{
				SePlay("se_bicycle_action");
			}
			else
			{
				SePlay("se_eraserrace_dash");
			}
		}
		if (!_action)
		{
			effectData.shockWaveEffect.Play();
		}
		effectData.PlaySpeedUp();
		isOverSpeed = true;
		if (_action)
		{
			overSpeedTime = OVEAR_SPEED_TIME[1];
		}
		else
		{
			overSpeedTime = OVEAR_SPEED_TIME[0];
		}
		isDash = true;
		if ((IsCpu || (IsGoal && Scene_RoadRace.GM.IsLapRace)) && !Scene_RoadRace.FM.StageData.GetPoint(_panel).IsCpuNotAction && UnityEngine.Random.Range(0, 100) <= aiStrengthData.dashActionPer)
		{
			StartCoroutine(_DelayAction());
		}
	}
	private IEnumerator _DelayAction()
	{
		yield return new WaitForSeconds(0.1f);
		Action(_actionDash: true);
	}
	private void Goal()
	{
		effectData.SetSpeedUpEffectRate(0f);
		Scene_RoadRace.CM.Goal(charaNo);
	}
	protected void CheckShowReverse()
	{
		if (raceCheckPoint == null || IsCpu || IsGoal)
		{
			return;
		}
		float num = 0.5f;
		checkPointDistPrev = checkPointDistNow;
		checkPointDistNow = Vector3.Distance(transformIns.position, raceCheckPoint[(lastCheckPointNo + 1) % raceCheckPoint.Length].transform.position);
		if (checkPointDistNow <= checkPointDistPrev)
		{
			reverseRunTime -= Time.deltaTime;
			if (reverseRunTime < 0f - num)
			{
				reverseRunTime = 0f - num;
			}
		}
		else if (checkPointDistNow > checkPointDistPrev)
		{
			reverseRunTime += Time.deltaTime;
			if (reverseRunTime > num)
			{
				reverseRunTime = num;
			}
		}
		if (reverseRunTime >= num)
		{
			Scene_RoadRace.UM.ShowReverseRun(charaNo, _isShow: true);
		}
		if (reverseRunTime <= 0f - num)
		{
			Scene_RoadRace.UM.ShowReverseRun(charaNo, _isShow: false);
		}
	}
	public float GetDisNextCheck()
	{
		return Vector3.Distance(transformIns.position, raceCheckPoint[checkPointIdx].transform.position);
	}
	protected void OnTriggerEnter(Collider _collider)
	{
		if (_collider.tag == "CheckPoint" && !IsGoal)
		{
			if (_collider.gameObject == raceCheckPoint[checkPointIdx].gameObject)
			{
				checkPointNo = checkPointNext;
				if (checkPointNo == Scene_RoadRace.CM.CheckPointTotalNum - 1)
				{
					Goal();
				}
				else
				{
					checkPointNext++;
					checkPointIdx = checkPointNext % raceCheckPoint.Length;
					if (!IsGoal && checkPointNo != 0 && CheckPointNoIdx == raceCheckPoint.Length - 1)
					{
						lapNum++;
						if (!IsCpu)
						{
							Scene_RoadRace.CM.UpdateLap(CharaNo);
						}
					}
				}
			}
			int num = Scene_RoadRace.FM.GetCheckPointNo(_collider.gameObject);
			if (lastCheckPointNo != num)
			{
				lastCheckPointNo = num;
				if (CharaNo == 0)
				{
					UnityEngine.Debug.Log("No." + charaNo.ToString() + "通過したチェックポイント : " + lastCheckPointNo.ToString());
				}
			}
		}
		if (_collider.tag == "ActionPoint")
		{
			Dash(_collider.gameObject);
		}
		if (_collider.tag == "AiPoint")
		{
			Scene_RoadRace.CM.SetAiPoint(CharaNo, Scene_RoadRace.FM.GetAiPointNo(_collider));
		}
		if (_collider.tag == "Finish")
		{
			IsInertiaAfterGoal = false;
		}
	}
	protected void OnCollisionEnter(Collision _collision)
	{
		if (!IsCpu)
		{
			if (_collision.contacts[0].point.y <= Rigid.position.y + GetCharaBodySize() * 0.25f)
			{
				if (prevVec.y <= -1.5f || isJump)
				{
					SePlay("se_snowball_steal");
					isJump = false;
				}
			}
			else if (crashSeTime <= 0f && Rigid.velocity.sqrMagnitude >= 1f)
			{
				Vector3 normalized = (_collision.contacts[0].point - Rigid.position).normalized;
				Vector3 normalized2 = prevVec.normalized;
				normalized.y = (normalized2.y = 0f);
				if (Mathf.Abs(Vector3.Angle(normalized, normalized2)) <= 60f)
				{
					crashSeTime = CRASH_SE_INTERVAL;
					SePlay("se_collision_character_1");
				}
			}
		}
		if (_collision.gameObject.tag == "CharaWall")
		{
			Rigid.velocity *= GetBrakeMag(BRAKE_TYPE.CONTACT_WALL);
		}
	}
	protected void ChangeUniform()
	{
		style.SetGameStyle(GS_Define.GameType.ARCHER_BATTLE, (int)userType);
	}
	protected void CreateByicycle(int _no)
	{
		if (byicycleNo < 0)
		{
			byicycleNo = _no;
			bicycle = UnityEngine.Object.Instantiate(Scene_RoadRace.CM.BicycleBasePrefs, actionAnchor.position, actionAnchor.rotation, actionAnchor).GetComponent<RoadRaceBicycleScript>();
			bicycle.SetMaterial(Scene_RoadRace.CM.BicycleMats[byicycleNo]);
			bicycleType = RoadRaceDefine.PlayBicycleType.ROAD;
			if (bicycle.wheels.Length >= 3)
			{
				trailRenderers = new TrailRenderer[2];
				trailRenderers[0] = trailRenderer;
				trailRenderers[1] = UnityEngine.Object.Instantiate(trailRenderer, trailRenderer.transform.position, trailRenderer.transform.rotation, trailRenderer.transform.parent);
			}
			else
			{
				trailRenderers = new TrailRenderer[1];
				trailRenderers[0] = trailRenderer;
			}
			for (int i = 0; i < trailRenderers.Length; i++)
			{
				trailRenderers[i].transform.SetPosition(bicycle.wheels[bicycle.wheels.Length - 1 - i].position.x, trailRenderers[i].transform.position.y, bicycle.wheels[bicycle.wheels.Length - 1 - i].position.z);
			}
		}
	}
	protected void InitAnimation()
	{
		Vector3 vector = bicycle.GetRideAnchor() - animation.Part(CharacterParts.BodyPartsList.HIP).position;
		obj.AddPosition(vector.x, vector.y, vector.z);
		obj.AddLocalPositionY(0.0421675f);
		obj.AddLocalPositionZ(0.07f);
		switch (bicycleType)
		{
		case RoadRaceDefine.PlayBicycleType.BMX:
			animation.Part(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesX(15f);
			animation.Part(CharacterParts.BodyPartsList.HIP).SetLocalEulerAnglesX(15f);
			break;
		case RoadRaceDefine.PlayBicycleType.MOUNTAIN:
			animation.Part(CharacterParts.BodyPartsList.HIP).SetLocalEulerAnglesX(15f);
			animation.Part(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesX(40.88f);
			break;
		case RoadRaceDefine.PlayBicycleType.ROAD:
			animation.Part(CharacterParts.BodyPartsList.HIP).SetLocalEulerAnglesX(15f);
			animation.Part(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesX(55f);
			break;
		case RoadRaceDefine.PlayBicycleType.UNIQUE:
			animation.Part(CharacterParts.BodyPartsList.BODY).SetLocalEulerAnglesX(0.01f);
			break;
		}
		for (int i = 0; i < bicycle.pedals.Length; i++)
		{
			if (!(bicycle.pedals[i] == null))
			{
				animation.Part(7 + i).SetLocalPositionX((i == 0) ? (-0.1f) : 0.1f);
				bicycle.pedalAnchors[i].parent = bicycle.pedals[i];
				float x = bicycle.pedals[i].InverseTransformPoint(animation.Part(7 + i).position).x;
				bicycle.pedalAnchors[i].SetLocalPosition(x, 0f, 0f);
			}
		}
		if (bicycle.handle != null)
		{
			Vector3 vector2 = transformIns.InverseTransformPoint(animation.Part(CharacterParts.BodyPartsList.SHOULDER_L).position);
			Vector3 a = transformIns.InverseTransformPoint(bicycle.HandleAnchor.position);
			a.x = vector2.x;
			float num = Vector3.Angle(a - vector2, Vector3.up);
			Vector3 point = default(Vector3);
			point.x = 180f + num;
			point.y = 0f;
			point.z = 0f;
			animation.Part(CharacterParts.BodyPartsList.SHOULDER_L).rotation = Quaternion.Euler(transformIns.rotation * point);
			animation.Part(CharacterParts.BodyPartsList.SHOULDER_R).rotation = Quaternion.Euler(transformIns.rotation * point);
		}
		else
		{
			animation.Part(CharacterParts.BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 0f);
			animation.Part(CharacterParts.BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 0f);
			animation.Part(CharacterParts.BodyPartsList.ARM_L).SetLocalEulerAnglesX(300f);
			animation.Part(CharacterParts.BodyPartsList.ARM_R).SetLocalEulerAnglesX(300f);
		}
		animation.Part(CharacterParts.BodyPartsList.HEAD).transform.forward = (animation.Part(CharacterParts.BodyPartsList.BODY).transform.forward + transformIns.forward) * 0.5f;
		for (int j = 0; j < animation.LegAnchors.Length; j++)
		{
			animation.LegAnchors[j].SetLocalScaleY(1f / animation.LegAnchors[j].parent.localScale.y);
			animation.LegSubParts[j].parent = animation.LegAnchors[j];
		}
		animation.Init();
		AnimPedal();
	}
	public bool CheckObj(GameObject _obj)
	{
		if (!(_obj == obj.gameObject))
		{
			return base.gameObject == _obj;
		}
		return true;
	}
	public Vector3 GetScreenPos(int _no)
	{
		return SingletonCustom<RoadRaceCameraManager>.Instance.Get3dCamera(_no).WorldToScreenPoint(GetPos() + Vector3.up * GetCharaHeight() * 2f);
	}
	public Vector3 GetMoveVec()
	{
		return nowPos - prevPos;
	}
	public Vector3 GetPos(bool _isLocal = false)
	{
		if (_isLocal)
		{
			return transformIns.localPosition;
		}
		return transformIns.position;
	}
	public Vector3 GetDir()
	{
		return Quaternion.Euler(0f, transformIns.eulerAngles.y, 0f) * Vector3.forward;
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
	public float GetRotSpeedAccelPer()
	{
		bool isCpu2 = IsCpu;
		return 1f;
	}
	public float GetTiltBodyRot()
	{
		return obj.localEulerAngles.z;
	}
	public float GetRotSpeed()
	{
		if (IsCpu)
		{
			return rotSpeedMag * 2f;
		}
		return rotSpeedMag;
	}
	public float GetBrakeMag(BRAKE_TYPE _type)
	{
		return BRAKE_MAG[(int)_type];
	}
	public float GetOverSpeedValue()
	{
		if (isOverSpeed)
		{
			return Scene_RoadRace.Floor(Rigid.velocity.magnitude, 2f) - Scene_RoadRace.Floor(moveMaxSpeed * WaterSpeedUpMag(), 2f);
		}
		return 0f;
	}
	public void SetIsGoal(bool _isGoal)
	{
		isGoal = _isGoal;
	}
	public void SetGoalTime(float _time)
	{
		goalTime = _time;
	}
	public void SetNowRank(int _rank)
	{
		nowRank = _rank;
	}
	public void InitCheckPointData()
	{
		checkPointNo = -1;
		checkPointNext = 0;
		checkPointIdx = 0;
		lastCheckPointNo = 0;
	}
	public void AiInit()
	{
		aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		switch (aiStrength)
		{
		case 0:
			UnityEngine.Debug.Log("弱い");
			break;
		case 1:
			UnityEngine.Debug.Log("普通");
			break;
		case 2:
			UnityEngine.Debug.Log("強い");
			break;
		}
		aiStrengthData.Copy(aiStrengthDatas[aiStrength]);
		aiActionDashPanel = null;
		float num = 0.01f;
		aiStrengthData.speedCorr *= UnityEngine.Random.Range(1f - num, 1f + num);
		aiStrengthData.waterSpeedUpCorr *= UnityEngine.Random.Range(1f - num, 1f + num);
	}
	private void NetworkCpuStrength()
	{
	}
	public void AiMove()
	{
		if (!isDontMove)
		{
			Vector3 vector = aiPointPos - GetPos();
			vector.y = 0f;
			GetDir();
			Move(vector.normalized, _isAccel: true);
			UnicycleRotZ();
			UnicycleRotX();
		}
	}
	public int GetAiPointNo()
	{
		return aiPointNo;
	}
	public void SetAIPosition(Vector3 pos)
	{
		aiPointPos = pos;
	}
	public void SetAiPointNo(int _aiPointNo)
	{
		aiPointNo = _aiPointNo;
	}
	private void SePlay(string _seName)
	{
		if (isPlaySe)
		{
			SingletonCustom<AudioManager>.Instance.SePlay(_seName);
		}
	}
}
