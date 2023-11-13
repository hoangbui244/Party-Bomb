using GamepadInput;
using SaveDataDefine;
using System;
using System.Collections;
using UnityEngine;
public class Skijump_Character : MonoBehaviour
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
		[SerializeField]
		[Header("レンダラ\u30fcリスト")]
		public MeshRenderer[] rendererList;
		private Transform[] transformList;
		public MeshFilter[] filterList;
		public Transform Parts(BodyPartsList _parts)
		{
			return rendererList[(int)_parts].transform;
		}
		public Transform Parts(int _parts)
		{
			return rendererList[_parts].transform;
		}
		public Transform[] Parts()
		{
			if (transformList == null || transformList.Length == 0)
			{
				transformList = new Transform[rendererList.Length];
				for (int i = 0; i < transformList.Length; i++)
				{
					transformList[i] = rendererList[i].transform;
				}
			}
			return transformList;
		}
	}
	[Serializable]
	public struct StatusData
	{
		public float slidePower;
		public float takeOffPower;
		public float liftPower;
		public float gaugeAreaMag;
	}
	public struct BrakingData
	{
		public bool isBraking;
		public bool isAfterLanding;
		public float speed;
		public float brakingTime;
	}
	public struct ParamColcData
	{
		public float[] slidePower;
		public float[] takeOffPower;
		public float[] liftPower;
		public float[] gaugeAreaMag;
		public ParamColcData(float[] _slidePower, float[] _takeOffPower, float[] _liftPower, float[] _gaugeSpeedMag)
		{
			slidePower = _slidePower;
			takeOffPower = _takeOffPower;
			liftPower = _liftPower;
			gaugeAreaMag = _gaugeSpeedMag;
		}
	}
	[SerializeField]
	[Header("オブジェクト")]
	protected Transform obj;
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	protected BodyParts bodyParts;
	[SerializeField]
	[Header("アニメ\u30fcション再生クラス")]
	protected CharacterPlayAnimation playAni;
	[SerializeField]
	[Header("飛び込みエフェクト")]
	protected ParticleSystem diveEffect;
	[SerializeField]
	[Header("泳ぐエフェクトを集めたアンカ\u30fc")]
	protected GameObject swimEffectAnchor;
	[SerializeField]
	[Header("滑走エフェクト")]
	protected SlideEffect[] slideEffects;
	[SerializeField]
	[Header("着地エフェクト")]
	protected ParticleSystem[] landingEffects;
	protected int effectNo;
	[SerializeField]
	[Header("リジッドボディ")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private CapsuleCollider charaCollider;
	[SerializeField]
	[Header("パラメ\u30fcタ：スピ\u30fcド")]
	private int speedParameter;
	[SerializeField]
	[Header("パラメ\u30fcタ：テクニック")]
	private int techniqueParameter;
	[SerializeField]
	[Header("パラメ\u30fcタ：アクセル")]
	private int accelParameter;
	[SerializeField]
	[Header("パラメ\u30fcタ：スタミナ")]
	private int staminaParameter;
	[SerializeField]
	[Header("キャラスタイル")]
	private CharacterStyle style;
	[SerializeField]
	[Header("ダッシュエフェクト")]
	private ParticleSystem psDashEffect;
	[SerializeField]
	[Header("空中ダッシュエフェクト")]
	private ParticleSystem psAirDashEffect;
	[SerializeField]
	[Header("左スキ\u30fc板モデル")]
	private MeshFilter leftMeshSki;
	[SerializeField]
	[Header("左スキ\u30fc板モデル配列")]
	private Mesh[] arrayLeftMeshSki;
	[SerializeField]
	[Header("右スキ\u30fc板モデル")]
	private MeshFilter rightMeshSki;
	[SerializeField]
	[Header("右スキ\u30fc板モデル配列")]
	private Mesh[] arrayRightMeshSki;
	[SerializeField]
	[Header("スキ\u30fc板トレイル")]
	private TrailRenderer[] arraySkiTrail;
	[SerializeField]
	[Header("着地失敗トレイル")]
	private TrailRenderer landingFailTrail;
	[SerializeField]
	[Header("ジャンプ中トレイル")]
	private TrailRenderer[] arrayAirTrail;
	private static float STOP_CHECK_SPEED = 0.1f;
	private float ANGULAR_VELOCITY_MAX = 250f;
	private Skijump_Define.OperationState operationState;
	private Skijump_Define.UserType userType;
	private Skijump_Define.TeamType teamType;
	private ParamColcData paramColcData = new ParamColcData(new float[2]
	{
		0f,
		2.5f
	}, new float[2]
	{
		0f,
		0.1f
	}, new float[2]
	{
		0f,
		0.1f
	}, new float[2]
	{
		1.5f,
		7.5f
	});
	private Vector3 defPos;
	private Vector3 prevPos;
	private Vector3 nowPos;
	private Quaternion defRot;
	private StatusData statusData;
	private BrakingData brakingData;
	private RaycastHit rayHit;
	private Skijump_Define.LandingMotionType landingMotionType;
	private Coroutine missLandingMotion;
	private float motionTime;
	private float stateTime;
	private float timeLimit;
	private float slideLimitTime = 10f;
	private float stopTime;
	private int slideStateNo;
	private int standingStandbyNo;
	private float skiBoardThickness;
	private float takeOffDistance;
	private float jumpPosHeight;
	private float jumpDistance;
	private float jumpDistanceStatusCorr;
	private float glideSpeedMin = 0.75f;
	private float glideRotSpeed = 4f;
	private float balanceControllCor = 2.5f;
	private float balanceRotCor = 20f;
	private Vector3 balanceAddRot;
	private float glidingRotX = -13f;
	private Vector3 descentStartPos;
	private float descentStartHeight;
	private float descentSpeed = 100f;
	private Vector3 landingPos;
	private float landingStartPos;
	private float standingGaugeOffset = 0.8f;
	private float brakeSpeedMin = 1.75f;
	private float heightAdjustSpeed = 1.5f;
	private float stopTimingValue;
	private float charaHeight;
	private float windCorrDistance;
	private bool isPlayer;
	private bool isSlideGaugeStart;
	private bool isTakeOff;
	private bool isGlide;
	private bool isOverApproachEnd;
	private bool isHeightFixed;
	protected float balanceControllInterval;
	protected float balanceControllTime;
	protected float seSlideVolume = 0.2f;
	private float balanceCheckTime;
	private Vector3 tempBrakePoint;
	private float aiActionTime;
	private bool isInputTakeOff;
	private float balancePoint = 100f;
	private bool isSkip;
	public bool IsCpu
	{
		get;
		set;
	}
	public Rigidbody Rigid => rigid;
	public Transform[] BodyPartsObj => bodyParts.Parts();
	public CharacterPlayAnimation PlayAni => playAni;
	protected SlideEffect SlideEffect => slideEffects[effectNo];
	protected ParticleSystem LandingEffect => landingEffects[effectNo];
	public Skijump_Define.OperationState OperationState
	{
		get
		{
			return operationState;
		}
		set
		{
			operationState = value;
		}
	}
	public Skijump_Define.UserType UserType => userType;
	public float JumpDistance => jumpDistance;
	public void Init(Skijump_Define.UserType _userType, Skijump_Define.TeamType _teamType, bool _isPlayer)
	{
		userType = _userType;
		IsCpu = ((int)userType >= SingletonCustom<GameSettingManager>.Instance.PlayerNum);
		teamType = _teamType;
		isPlayer = _isPlayer;
		style.SetGameStyle(GS_Define.GameType.ATTACK_BALL, (int)(IsCpu ? (4 + userType - SingletonCustom<GameSettingManager>.Instance.PlayerNum) : userType));
		leftMeshSki.mesh = arrayLeftMeshSki[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)(IsCpu ? (4 + userType - SingletonCustom<GameSettingManager>.Instance.PlayerNum) : userType)]];
		rightMeshSki.mesh = arrayRightMeshSki[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)(IsCpu ? (4 + userType - SingletonCustom<GameSettingManager>.Instance.PlayerNum) : userType)]];
		rigid.maxAngularVelocity = ANGULAR_VELOCITY_MAX;
		charaHeight = charaCollider.height * charaCollider.transform.lossyScale.y;
		skiBoardThickness = 0.01f;
		defPos = (nowPos = (prevPos = base.transform.position));
		defRot = base.transform.rotation;
		SettingCharaParameter();
		rigid.isKinematic = true;
		if (Physics.Raycast(base.transform.position, Vector3.down, out rayHit, Skijump_Define.RAY_DISTANCE_MAX, Skijump_Define.GetLayerMask("Collision_Obj_1")))
		{
			string name = rayHit.collider.name;
			Vector3 point = rayHit.point;
			UnityEngine.Debug.Log("接触 : " + name + " : 高さ = " + point.y.ToString());
			base.transform.SetPositionY(rayHit.point.y + GetCharaHeight() * 0.7965f);
		}
		else
		{
			UnityEngine.Debug.LogError("接触なし");
		}
		playAni.InitMethod(obj, this);
		playAni.SetMotion(CharacterPlayAnimation.MotionType.NONE);
		SlideEffect.Init();
		Skijump_Define.MSM.SetCameraWorkType(Skijump_Define.CameraWorkType.NORMAL);
		isInputTakeOff = false;
		timeLimit = -1.075f;
		balancePoint = 100f;
		balanceCheckTime = 0f;
		isSkip = false;
	}
	public void UpdateMethod()
	{
		if (!Skijump_Define.MGM.IsDuringGame())
		{
			return;
		}
		switch (operationState)
		{
		case Skijump_Define.OperationState.SLIDE_STANDBY:
			SlideStandbyState();
			break;
		case Skijump_Define.OperationState.SLIDE:
			SlideState();
			break;
		case Skijump_Define.OperationState.JUMP:
			JumpState();
			break;
		case Skijump_Define.OperationState.LANDING:
			LandingState();
			break;
		case Skijump_Define.OperationState.JUMP_FINISH:
			JumpFinishState();
			break;
		case Skijump_Define.OperationState.STOP:
			StopState();
			break;
		}
		if (isPlayer)
		{
			PlayerOperation();
		}
		else
		{
			switch (operationState)
			{
			case Skijump_Define.OperationState.SLIDE_STANDBY:
				if (!isSkip && SingletonCustom<Skijump_UIManager>.Instance.IsShowSkip && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X))
				{
					isSkip = true;
					SingletonCustom<Skijump_UIManager>.Instance.FadePrevFrame(0.5f, 0.25f, 0f, delegate
					{
						if (SingletonCustom<Skijump_UIManager>.Instance.IsShowSkip)
						{
							SingletonCustom<Skijump_UIManager>.Instance.CloseSkip();
						}
						CngStateSlide();
						SingletonCustom<Skijump_WindManager>.Instance.SetSkipWind();
						TakeOff();
						switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
						{
						case SystemData.AiStrength.WEAK:
							jumpDistance = UnityEngine.Random.Range(11f, 11.3f);
							break;
						case SystemData.AiStrength.NORAML:
							jumpDistance = UnityEngine.Random.Range(12f, 13f);
							break;
						case SystemData.AiStrength.STRONG:
							jumpDistance = UnityEngine.Random.Range(13f, 13.3f);
							break;
						}
						base.transform.SetPositionZ(SingletonCustom<Skijump_StageManager>.Instance.GetApproachEndAnchor().position.z + jumpDistance * 0.85f);
						base.transform.SetPositionY(GetLandingPos().y + 0.3f);
						balancePoint -= UnityEngine.Random.Range(0f, 15f);
						CngStateLanding();
						rigid.velocity = rigid.velocity.normalized * glideSpeedMin;
						rigid.velocity = Vector3.zero;
						psDashEffect.Stop();
						SlideEffect.Stop();
					});
				}
				break;
			case Skijump_Define.OperationState.SLIDE:
				if (isSlideGaugeStart && SingletonCustom<Skijump_UIManager>.Instance.Jump.GetTimiingLocalPosXPer() >= stopTimingValue)
				{
					TakeOff();
				}
				if (!isSkip && SingletonCustom<Skijump_UIManager>.Instance.IsShowSkip && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X))
				{
					isSkip = true;
					SingletonCustom<Skijump_UIManager>.Instance.FadePrevFrame(0.5f, 0.25f, 0f, delegate
					{
						if (SingletonCustom<Skijump_UIManager>.Instance.IsShowSkip)
						{
							SingletonCustom<Skijump_UIManager>.Instance.CloseSkip();
						}
						CngStateSlide();
						SingletonCustom<Skijump_WindManager>.Instance.SetSkipWind();
						TakeOff();
						switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
						{
						case SystemData.AiStrength.WEAK:
							jumpDistance = UnityEngine.Random.Range(11f, 11.3f);
							break;
						case SystemData.AiStrength.NORAML:
							jumpDistance = UnityEngine.Random.Range(12f, 13f);
							break;
						case SystemData.AiStrength.STRONG:
							jumpDistance = UnityEngine.Random.Range(13f, 13.5f);
							break;
						}
						base.transform.SetPositionZ(SingletonCustom<Skijump_StageManager>.Instance.GetApproachEndAnchor().position.z + jumpDistance * 0.85f);
						base.transform.SetPositionY(GetLandingPos().y + 0.3f);
						balancePoint -= UnityEngine.Random.Range(0f, 15f);
						CngStateLanding();
						rigid.velocity = rigid.velocity.normalized * glideSpeedMin;
						rigid.velocity = Vector3.zero;
						psDashEffect.Stop();
						SlideEffect.Stop();
					});
				}
				break;
			case Skijump_Define.OperationState.JUMP:
				if (!SingletonCustom<Skijump_UIManager>.Instance.Balance.IsControll())
				{
					break;
				}
				balanceControllInterval -= Time.deltaTime;
				if (!(balanceControllInterval <= 0f))
				{
					break;
				}
				balanceControllTime -= Time.deltaTime;
				if (SingletonCustom<Skijump_UIManager>.Instance.Balance.GetControllValue() >= 0.1f)
				{
					if (SingletonCustom<Skijump_UIManager>.Instance.Balance.GetMarkPosPer() > 0f)
					{
						SingletonCustom<Skijump_UIManager>.Instance.Balance.MarkControll(Vector3.left);
					}
					else
					{
						SingletonCustom<Skijump_UIManager>.Instance.Balance.MarkControll(Vector3.right);
					}
				}
				if (balanceControllTime <= 0f)
				{
					SingletonCustom<Skijump_CharacterManager>.Instance.CpuAi.SettingBalanceControllData();
					balanceControllInterval = SingletonCustom<Skijump_CharacterManager>.Instance.CpuAi.GetBalanceControllInterval();
					balanceControllTime = SingletonCustom<Skijump_CharacterManager>.Instance.CpuAi.GetBalanceControllTime();
				}
				break;
			}
			if (SingletonCustom<Skijump_UIManager>.Instance.Landing.CheckState(Skijump_TimingGauge.State.MOVE) && SingletonCustom<Skijump_UIManager>.Instance.Landing.GetTimiingLocalPosXPer() >= stopTimingValue)
			{
				Standing();
			}
		}
		playAni.UpdateMethod();
	}
	public void SetJumpReady()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		rigid.velocity = Vector3.zero;
		rigid.angularVelocity = Vector3.zero;
		rigid.isKinematic = true;
		base.transform.position = defPos;
		base.transform.rotation = defRot;
		if (Physics.Raycast(base.transform.position, Vector3.down, out rayHit, Skijump_Define.RAY_DISTANCE_MAX, Skijump_Define.GetLayerMask("Collision_Obj_1")))
		{
			string name = rayHit.collider.name;
			Vector3 point = rayHit.point;
			UnityEngine.Debug.Log("接触 : " + name + " : 高さ = " + point.y.ToString());
			base.transform.SetPositionY(rayHit.point.y + GetCharaHeight() * 0.7965f);
		}
		else
		{
			UnityEngine.Debug.LogError("接触なし");
		}
		Skijump_Define.GUIM.Jump.SetGaugeAreaMag(statusData.gaugeAreaMag);
		Skijump_Define.GUIM.Landing.SetGaugeAreaMag(statusData.gaugeAreaMag * 2.5f);
		Skijump_Define.MSM.SetCameraWorkType(Skijump_Define.CameraWorkType.NORMAL);
		if (!isPlayer)
		{
			operationState = Skijump_Define.OperationState.SLIDE_STANDBY;
		}
		else
		{
			operationState = Skijump_Define.OperationState.SLIDE_STANDBY;
			LeanTween.delayedCall(base.gameObject, 0.75f, (Action)delegate
			{
				SingletonCustom<Skijump_UIManager>.Instance.ShowSlideStartInfo();
			});
		}
		LeanTween.delayedCall(base.gameObject, 0.95f, (Action)delegate
		{
			SingletonCustom<Skijump_WindManager>.Instance.Show();
		});
		timeLimit = -1.075f;
		aiActionTime = 0f;
		playAni.SetMotion(CharacterPlayAnimation.MotionType.NONE);
		SingletonCustom<AudioManager>.Instance.SeStop("SE_seien");
		LeanTween.delayedCall(base.gameObject, 1.05f, (Action)delegate
		{
			if (IsCpu)
			{
				SingletonCustom<Skijump_UIManager>.Instance.ShowSkip();
			}
			SingletonCustom<Skijump_UIManager>.Instance.ShowTimeLimit();
		});
		for (int i = 0; i < arraySkiTrail.Length; i++)
		{
			arraySkiTrail[i].enabled = false;
		}
		landingFailTrail.enabled = false;
		isInputTakeOff = false;
		balancePoint = 100f;
		balanceCheckTime = 0f;
		isSkip = false;
	}
	private void SlideStandbyState()
	{
		timeLimit += Time.deltaTime;
		timeLimit = Mathf.Min(timeLimit, slideLimitTime);
		if (timeLimit >= 0f)
		{
			SingletonCustom<Skijump_UIManager>.Instance.SetTimeLimit(timeLimit);
		}
		if (timeLimit >= slideLimitTime)
		{
			CngStateSlide();
		}
		if (IsCpu)
		{
			aiActionTime = Mathf.Clamp(aiActionTime + Time.deltaTime, 0f, 2.5f);
			if (aiActionTime >= 2.5f && SingletonCustom<Skijump_WindManager>.Instance.GetWindValue() > 0.45f)
			{
				CngStateSlide();
			}
		}
	}
	public void CngStateSlide()
	{
		SingletonCustom<Skijump_UIManager>.Instance.CloseTimeLimit();
		operationState = Skijump_Define.OperationState.SLIDE;
		rigid.isKinematic = false;
		rigid.WakeUp();
		CalcManager.mCalcVector3 = base.transform.forward * (Skijump_Define.SLIDE_BASE + statusData.slidePower);
		rigid.AddForce(CalcManager.mCalcVector3, ForceMode.Impulse);
		slideStateNo = 0;
		Skijump_Define.GUIM.WindManager.SetChangeWindDir(_flg: false);
		motionTime = Skijump_Define.MOTION_TIME_STANDARD;
		playAni.SetMotion(CharacterPlayAnimation.MotionType.SLIDE, motionTime);
		if (!isPlayer)
		{
			SingletonCustom<Skijump_CharacterManager>.Instance.CpuAi.SettingGaugeData();
			stopTimingValue = SingletonCustom<Skijump_CharacterManager>.Instance.CpuAi.GetTimingValue();
		}
		else
		{
			SingletonCustom<Skijump_UIManager>.Instance.HideSlideStartInfo();
			SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
		}
		UnityEngine.Debug.Log("stopTimingValue = " + stopTimingValue.ToString());
		SlideEffect.Play();
		isSlideGaugeStart = false;
		isTakeOff = false;
		if (!isSkip)
		{
			LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
			{
				psDashEffect.Play();
			});
		}
		SingletonCustom<Skijump_CameraWorkManager>.Instance.SetState(Skijump_CameraWorkManager.State.SLIDE);
		SingletonCustom<Skijump_CameraWorkManager>.Instance.SetSpeedEffect(_isEnable: true);
		SingletonCustom<AudioManager>.Instance.SePlay("se_slide", _loop: true, 0f, seSlideVolume);
	}
	private void SlideState()
	{
		switch (slideStateNo)
		{
		case 0:
			if (Skijump_Define.MSM.CheckTakeOffDistancePer(GetPos(), Skijump_Define.MSM.GetApproachStartPosZ(rigid.velocity.magnitude) * 4f))
			{
				Skijump_Define.GUIM.Jump.CngStateGaugeShow(_show: true);
				slideStateNo++;
			}
			break;
		case 1:
			if (!isPlayer)
			{
				SlideGaugeStart();
			}
			break;
		}
		if (Skijump_Define.MSM.CheckTakeOffDistancePer(GetPos(), Skijump_Define.MSM.GetApproachStartPosZ(rigid.velocity.magnitude) * 8f) && SingletonCustom<Skijump_UIManager>.Instance.IsShowSkip)
		{
			SingletonCustom<Skijump_UIManager>.Instance.CloseSkip();
		}
		if (isSlideGaugeStart && !isInputTakeOff)
		{
			Skijump_Define.GUIM.Jump.SetMarkPos(Mathf.Min(1f - Skijump_Define.MSM.GetTakeOffDistance(GetPos()) / takeOffDistance, 1.24f));
		}
		if (Skijump_Define.MSM.CheckOverApproachEnd(GetPos()))
		{
			if (!isInputTakeOff)
			{
				TakeOff();
			}
			ToJump();
		}
	}
	public void ToJump()
	{
		operationState = Skijump_Define.OperationState.JUMP;
		Skijump_Define.GUIM.Jump.CngStateHide();
		CalcManager.mCalcVector3 = base.transform.forward * (Skijump_Define.TAKE_OFF_BASE + statusData.takeOffPower + rigid.velocity.magnitude * Skijump_Define.TAKE_OFF_VELOCITY_CORR);
		rigid.velocity = CalcManager.mCalcVector3;
		standingStandbyNo = 0;
		rigid.constraints |= RigidbodyConstraints.FreezeRotationX;
		isGlide = true;
		rigid.useGravity = false;
		isOverApproachEnd = false;
		jumpDistanceStatusCorr = 0f;
		motionTime = Skijump_Define.MOTION_TIME_STANDARD;
		playAni.SetMotion(CharacterPlayAnimation.MotionType.JUMP, motionTime);
		isHeightFixed = false;
		SlideEffect.Stop();
		psDashEffect.Stop();
		SingletonCustom<Skijump_CameraWorkManager>.Instance.SetState(Skijump_CameraWorkManager.State.JUMP);
		if (!isSkip)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_wind_storm", _loop: true);
			SingletonCustom<AudioManager>.Instance.SeStop("se_slide");
			SingletonCustom<AudioManager>.Instance.SePlay("se_jump");
		}
	}
	public void CngStateJump()
	{
		bool flag;
		if (Skijump_Define.GUIM.Jump.CheckState(Skijump_TimingGauge.State.MOVE_STOP))
		{
			flag = true;
		}
		else
		{
			flag = false;
			Skijump_Define.GUIM.Jump.CngStateMoveStop(flag, isSkip);
		}
		Skijump_Define.GUIM.Jump.StopState(Skijump_TimingGauge.State.MOVE_STOP);
		UnityEngine.Debug.Log("ジャンプ状態に変更 : success = " + flag.ToString());
		float num = 1f;
		stopTimingValue = Skijump_Define.GUIM.Jump.GetTimingValue();
		UnityEngine.Debug.Log("stopTimingValue = " + stopTimingValue.ToString());
		if (isSkip)
		{
			stopTimingValue = UnityEngine.Random.Range(0.9f, 1f);
		}
		num += stopTimingValue;
		UnityEngine.Debug.Log("【jumpDistanceStatusCorr_A】:" + jumpDistanceStatusCorr.ToString());
		jumpDistanceStatusCorr += Skijump_Define.MSM.GetJumpDistanceRange() * num;
		UnityEngine.Debug.Log("【風補正値】:" + Skijump_Define.GUIM.WindManager.GetWindCorr().ToString());
		windCorrDistance = Skijump_Define.GUIM.WindManager.GetWindCorr() * Skijump_Define.MSM.GetJumpDistanceRange() * 0.4f;
		jumpDistanceStatusCorr += windCorrDistance;
		UnityEngine.Debug.Log("【jumpDistanceStatusCorr】:" + jumpDistanceStatusCorr.ToString());
		jumpDistance = Skijump_Define.MSM.GetKPointDistance() + jumpDistanceStatusCorr / Skijump_Define.MSM.GetKPointDistanceData() * Skijump_Define.MSM.GetKPointDistance();
		if (flag)
		{
			jumpDistance -= Mathf.Abs(GetPos().z - Skijump_Define.MSM.GetApproachEndAnchor().position.z) * 0.5f;
		}
		jumpDistance *= 0.85f;
		UnityEngine.Debug.Log("【jumpDistance】:" + jumpDistance.ToString());
		if (!isPlayer)
		{
			SingletonCustom<Skijump_CharacterManager>.Instance.CpuAi.SettingGaugeData();
			stopTimingValue = SingletonCustom<Skijump_CharacterManager>.Instance.CpuAi.GetTimingValue();
		}
		UnityEngine.Debug.Log("stopTimingValue = " + stopTimingValue.ToString());
		Skijump_Define.GUIM.Jump.CngStateGaugeShow(_show: false);
		Skijump_Define.GUIM.Jump.ShowDescription(_show: false, 1);
		Skijump_Define.GUIM.Jump.ShowDescription(_show: false, 2);
		for (int i = 0; i < arrayAirTrail.Length; i++)
		{
			arrayAirTrail[i].enabled = true;
		}
	}
	private void JumpState()
	{
		if (rigid.velocity.magnitude < glideSpeedMin)
		{
			rigid.velocity = rigid.velocity.normalized * glideSpeedMin;
		}
		if (GetPos().z <= Skijump_Define.MSM.GetApproachEndAnchor().position.z)
		{
			isOverApproachEnd = false;
			CalcManager.mCalcVector3 = rigid.velocity;
			CalcManager.mCalcVector3.y = 0f;
			rigid.velocity = CalcManager.mCalcVector3;
			if (GetDistanceToGround() >= GetCharaHeight() * 2f)
			{
				rigid.position = Vector3.Lerp(rigid.position, rigid.position + Vector3.down * (GetDistanceToGround() - GetCharaHeight() * 2f), glideSpeedMin * Time.deltaTime);
			}
			CalcManager.mCalcVector3 = CalcManager.mVector3Zero;
			CalcManager.mCalcVector3.x = glidingRotX;
			rigid.rotation = Quaternion.Lerp(rigid.rotation, Quaternion.Euler(CalcManager.mCalcVector3), glideRotSpeed * Time.deltaTime);
			return;
		}
		if (!isOverApproachEnd)
		{
			rigid.useGravity = false;
			isOverApproachEnd = true;
			Skijump_Define.GUIM.Jump.StopFadeDescription();
			if (!isSkip)
			{
				Skijump_Define.GUIM.Balance.Show(0.5f);
				Skijump_Define.GUIM.Balance.MoveMark(1f);
			}
		}
		if (Skijump_Define.GUIM.Balance.IsControll())
		{
			balanceCheckTime += Time.deltaTime;
			if (balanceCheckTime >= 1.25f)
			{
				jumpDistance += (1f - Skijump_Define.GUIM.Balance.GetControllValue()) * balanceControllCor * statusData.liftPower * Time.deltaTime;
				if (Skijump_Define.GUIM.Balance.GetControllValue() <= 0.1f)
				{
					SingletonCustom<Skijump_UIManager>.Instance.Balance.ShowEffect(Skijump_Define.TimingResult.PERFECT);
					SingletonCustom<AudioManager>.Instance.SePlay("se_shorttrack_maxspeed", _loop: false, 0f, 1f, 1f, 0.15f);
					psAirDashEffect.Emit(10);
				}
				else if (Skijump_Define.GUIM.Balance.GetControllValue() <= 0.35f)
				{
					SingletonCustom<Skijump_UIManager>.Instance.Balance.ShowEffect(Skijump_Define.TimingResult.GOOD);
					SingletonCustom<AudioManager>.Instance.SePlay("se_shorttrack_maxspeed", _loop: false, 0f, 1f, 1f, 0.15f);
					psAirDashEffect.Emit(5);
					balancePoint -= 5f;
				}
				else
				{
					SingletonCustom<Skijump_UIManager>.Instance.Balance.ShowEffect(Skijump_Define.TimingResult.BAD);
					balancePoint -= 10f;
				}
				balanceCheckTime = 0f;
			}
			balanceAddRot = Skijump_Define.GUIM.Balance.GetBalanceRot() * balanceRotCor;
		}
		else
		{
			balanceAddRot = CalcManager.mVector3Zero;
		}
		if (isGlide)
		{
			if (isHeightFixed)
			{
				CalcManager.mCalcVector3 = CalcManager.mVector3Zero;
				CalcManager.mCalcVector3.y = jumpPosHeight - GetDistanceToGround();
				rigid.position = Vector3.Lerp(rigid.position, rigid.position + CalcManager.mCalcVector3, heightAdjustSpeed);
			}
			else if (Skijump_Define.MSM.CheckStandingAnchor(GetPos()))
			{
				isHeightFixed = true;
				jumpPosHeight = GetDistanceToGround();
			}
			CalcManager.mCalcVector3 = balanceAddRot;
			CalcManager.mCalcVector3.x += glidingRotX;
			rigid.rotation = Quaternion.Lerp(rigid.rotation, Quaternion.Euler(CalcManager.mCalcVector3), glideRotSpeed * Time.deltaTime);
			if (Skijump_Define.MSM.CalcJumpDistance(GetLandingPos()) >= jumpDistance * 0.4f)
			{
				isGlide = false;
				rigid.useGravity = false;
				descentStartPos = GetPos();
				descentStartPos.y -= GetCharaHeight() * 0.5f + skiBoardThickness;
				descentStartHeight = GetDistanceToGround();
				landingPos = Skijump_Define.MSM.CalcLandingPos(jumpDistance);
				landingPos.y += GetCharaHeight() * 0.5f + skiBoardThickness;
				landingStartPos = Skijump_Define.MSM.CalcJumpDistance(GetLandingPos()) / jumpDistance;
				Float();
			}
		}
		else
		{
			Float();
			if (Skijump_Define.MSM.CalcJumpDistance(GetLandingPos()) >= jumpDistance - GetCharaHeight())
			{
				rigid.rotation = Quaternion.Euler(Mathf.LerpAngle(rigid.rotation.eulerAngles.x, 25f, 20f * Time.deltaTime), 0f, 0f);
				motionTime = Skijump_Define.MOTION_TIME_STANDARD;
				playAni.SetMotion(CharacterPlayAnimation.MotionType.LANDING, motionTime);
			}
			else
			{
				CalcManager.mCalcVector3 = CalcManager.mVector3Zero;
				CalcManager.mCalcVector3.x = glidingRotX;
				CalcManager.mCalcVector3 += balanceAddRot;
				rigid.rotation = Quaternion.Lerp(rigid.rotation, Quaternion.Euler(CalcManager.mCalcVector3), glideRotSpeed * Time.deltaTime);
			}
		}
		switch (standingStandbyNo)
		{
		case 0:
			if (Skijump_Define.MSM.CalcJumpDistance(GetLandingPos()) >= jumpDistance * 0.7f)
			{
				Skijump_Define.GUIM.Balance.Hide();
				standingStandbyNo++;
			}
			break;
		case 1:
			if (Skijump_Define.MSM.CalcJumpDistance(GetLandingPos()) >= jumpDistance * 0.75f)
			{
				Skijump_Define.GUIM.Landing.CngStateGaugeShow(_show: true);
				Skijump_Define.GUIM.Landing.ShowDescription(_show: true, 0);
				standingStandbyNo++;
				UnityEngine.Debug.Log("タイミングマ\u30fcクゲ\u30fcジ表示");
				SingletonCustom<Skijump_CameraWorkManager>.Instance.SetState(Skijump_CameraWorkManager.State.AIR_BALANCE);
			}
			break;
		case 2:
			if (Skijump_Define.GUIM.Landing.CheckState(Skijump_TimingGauge.State.NONE) && Skijump_Define.MSM.CalcJumpDistance(GetLandingPos()) >= jumpDistance - (standingGaugeOffset + rigid.velocity.magnitude * 0.75f))
			{
				UnityEngine.Debug.Log("タイミングマ\u30fcク移動");
				Skijump_Define.GUIM.Landing.CngStateMove();
				standingStandbyNo++;
			}
			break;
		}
	}
	public void CngStateLanding()
	{
		rigid.useGravity = true;
		if (!isSkip)
		{
			if (Skijump_Define.GUIM.Landing.CheckState(Skijump_TimingGauge.State.MOVE_STOP))
			{
				bool flag = true;
			}
			else
			{
				bool flag = false;
				Skijump_Define.GUIM.Landing.CngStateMoveStop(flag);
			}
			for (int i = 0; i < arrayAirTrail.Length; i++)
			{
				arrayAirTrail[i].enabled = false;
			}
			Skijump_Define.GUIM.Landing.StopState(Skijump_TimingGauge.State.MOVE_STOP);
			Skijump_Define.GUIM.Landing.CngStateGaugeShow(_show: false, 0.5f);
			Skijump_Define.GUIM.Landing.ShowDescription(_show: false, 0, 0.5f);
		}
		SingletonCustom<Skijump_CameraWorkManager>.Instance.SetState(Skijump_CameraWorkManager.State.LANDING);
		SingletonCustom<Skijump_CameraWorkManager>.Instance.SetSpeedEffect(_isEnable: false);
		operationState = Skijump_Define.OperationState.LANDING;
		rigid.constraints &= (RigidbodyConstraints)(-17);
		jumpDistance = Skijump_Define.MSM.CalcJumpDistance(GetLandingPos(), _jumping: false);
		UnityEngine.Debug.Log("飛距離 = " + jumpDistance.ToString());
		stopTimingValue = Skijump_Define.GUIM.Landing.GetTimingValue();
		if (isSkip)
		{
			stopTimingValue = UnityEngine.Random.Range(0.9f, 1f);
		}
		UnityEngine.Debug.Log("stopTimingValue = " + stopTimingValue.ToString());
		balancePoint -= 10f * (1f - stopTimingValue);
		if (Skijump_Define.GUIM.Landing.GetTimingResult() == Skijump_Define.TimingResult.PERFECT)
		{
			landingMotionType = Skijump_Define.LandingMotionType.PERFECT;
		}
		else if (Skijump_Define.GUIM.Landing.GetTimingResult() == Skijump_Define.TimingResult.GOOD)
		{
			landingMotionType = Skijump_Define.LandingMotionType.MISS;
		}
		else
		{
			landingMotionType = Skijump_Define.LandingMotionType.FALL;
		}
		if (isSkip)
		{
			switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
			{
			case SystemData.AiStrength.WEAK:
				if (UnityEngine.Random.Range(0, 100) <= 70)
				{
					landingMotionType = Skijump_Define.LandingMotionType.MISS;
				}
				else
				{
					landingMotionType = Skijump_Define.LandingMotionType.PERFECT;
				}
				break;
			case SystemData.AiStrength.NORAML:
				landingMotionType = Skijump_Define.LandingMotionType.PERFECT;
				break;
			case SystemData.AiStrength.STRONG:
				landingMotionType = Skijump_Define.LandingMotionType.PERFECT;
				break;
			}
		}
		switch (landingMotionType)
		{
		case Skijump_Define.LandingMotionType.PERFECT:
			motionTime = Skijump_Define.MOTION_TIME_STANDARD;
			playAni.SetMotion(CharacterPlayAnimation.MotionType.LANDING, motionTime);
			SingletonCustom<AudioManager>.Instance.SePlay("SE_seien", _loop: false, 0f, 1f, 1f, 0.5f);
			for (int k = 0; k < arraySkiTrail.Length; k++)
			{
				arraySkiTrail[k].enabled = true;
			}
			break;
		case Skijump_Define.LandingMotionType.MISS:
			motionTime = Skijump_Define.MOTION_TIME_MISS;
			missLandingMotion = StartCoroutine(_MissLandingMotion());
			SingletonCustom<AudioManager>.Instance.SePlay("SE_seien", _loop: false, 0f, 1f, 1f, 0.5f);
			for (int j = 0; j < arraySkiTrail.Length; j++)
			{
				arraySkiTrail[j].enabled = true;
			}
			break;
		case Skijump_Define.LandingMotionType.FALL:
			motionTime = Skijump_Define.MOTION_TIME_FALL;
			playAni.SetMotion(CharacterPlayAnimation.MotionType.LANDING_FALL, motionTime);
			landingFailTrail.enabled = true;
			balancePoint -= 20f;
			break;
		}
		Skijump_Define.MSM.SettingBrakeAnchorData((landingMotionType != Skijump_Define.LandingMotionType.FALL) ? (-1) : 0);
		stateTime = 0f;
		SlideEffect.Play();
		LandingEffect.Play();
		brakingData.isBraking = false;
		brakingData.isAfterLanding = true;
		SingletonCustom<AudioManager>.Instance.SePlay("se_slide", _loop: true, 0f, seSlideVolume);
		SingletonCustom<AudioManager>.Instance.SePlay("se_land");
		SingletonCustom<AudioManager>.Instance.SeStop("se_wind_storm");
	}
	private void LandingState()
	{
		stateTime += Time.deltaTime;
		if (isSkip)
		{
			if (stateTime >= 1f)
			{
				stateTime = 0f;
				isSkip = false;
			}
			return;
		}
		if (landingMotionType != Skijump_Define.LandingMotionType.FALL && stateTime >= 2f)
		{
			if (missLandingMotion != null)
			{
				StopCoroutine(missLandingMotion);
				missLandingMotion = null;
			}
			motionTime = Skijump_Define.MOTION_TIME_STANDARD;
			playAni.SetMotion(CharacterPlayAnimation.MotionType.NONE, motionTime);
		}
		SlideEffect.SetEmission((rigid.velocity.sqrMagnitude - 2f) / 4f);
		if (brakingData.isAfterLanding)
		{
			if (GetPos().z >= Skijump_Define.MSM.GetSlopeEndAnchor().position.z)
			{
				brakingData.speed = rigid.velocity.magnitude;
				brakingData.isAfterLanding = false;
				rigid.constraints &= (RigidbodyConstraints)(-3);
				SingletonCustom<Skijump_UIManager>.Instance.ShowJumpScore(SingletonCustom<Skijump_CharacterManager>.Instance.NowJumpUserType, SingletonCustom<Skijump_CharacterManager>.Instance.NowJumpCount, (int)(jumpDistance * 1.415f * 10f), (int)balancePoint);
				CalcManager.mCalcVector3 = Skijump_Define.MSM.GetBrakeAnchorPos() - GetPos();
				CalcManager.mCalcVector3.y = 0f;
				CalcManager.mCalcVector3 = CalcManager.mCalcVector3.normalized;
				tempBrakePoint = CalcManager.mCalcVector3;
			}
		}
		else if (!brakingData.isBraking)
		{
			if (rigid.velocity.magnitude < brakeSpeedMin)
			{
				rigid.velocity = rigid.velocity.normalized * brakeSpeedMin;
			}
			if (Skijump_Define.MSM.CheckOverBrakeAnchorPos(GetPos()))
			{
				if (Skijump_Define.MSM.IsBrakeLastPos())
				{
					brakingData.isBraking = true;
				}
				else
				{
					Skijump_Define.MSM.NextBrakePos();
				}
			}
			else
			{
				CalcManager.mCalcVector3 = Skijump_Define.MSM.GetBrakeAnchorPos() - GetPos();
				CalcManager.mCalcVector3.y = 0f;
				CalcManager.mCalcVector3 = CalcManager.mCalcVector3.normalized;
				rigid.transform.forward = Vector3.Lerp(rigid.transform.forward, Skijump_Define.MSM.GetBrakeAnchorForward(), CalcManager.Length(rigid.transform.forward, Skijump_Define.MSM.GetBrakeAnchorForward()) * 5f * brakingData.speed * Time.deltaTime);
				rigid.velocity = Vector3.Lerp(rigid.velocity, CalcManager.mCalcVector3 * rigid.velocity.magnitude, 30f * brakingData.speed * Time.deltaTime);
			}
		}
		if (rigid.velocity.sqrMagnitude <= STOP_CHECK_SPEED)
		{
			CngStateJumpFinish();
		}
	}
	public void CngStateJumpFinish()
	{
		operationState = Skijump_Define.OperationState.JUMP_FINISH;
		stopTime = 0f;
		SingletonCustom<AudioManager>.Instance.SeStop("se_slide");
		SlideEffect.Stop();
		rigid.constraints |= RigidbodyConstraints.FreezePositionX;
	}
	private void JumpFinishState()
	{
		stopTime += Time.deltaTime;
		if (stopTime >= 1f)
		{
			CngStateStop();
		}
	}
	private void StopState()
	{
		stopTime += Time.deltaTime;
		if (stopTime >= 2f)
		{
			operationState = Skijump_Define.OperationState.NEXT;
		}
	}
	public void CngStateStop()
	{
		operationState = Skijump_Define.OperationState.STOP;
		stopTime = 0f;
	}
	private void PlayerOperation()
	{
		switch (operationState)
		{
		case Skijump_Define.OperationState.SLIDE_STANDBY:
			if (timeLimit >= 0f && Skijump_Define.CM.IsPushButton_A(userType, Skijump_ControllerManager.ButtonPushType.DOWN))
			{
				if (!IsCpu)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
				}
				CngStateSlide();
			}
			break;
		case Skijump_Define.OperationState.SLIDE:
			if (Skijump_Define.GUIM.Jump.IsShow && !isTakeOff)
			{
				SlideGaugeStart();
			}
			if (isSlideGaugeStart && !isInputTakeOff && Skijump_Define.CM.IsPushButton_A(userType, Skijump_ControllerManager.ButtonPushType.DOWN))
			{
				if (!IsCpu)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
				}
				TakeOff();
			}
			break;
		case Skijump_Define.OperationState.JUMP:
			if (Skijump_Define.GUIM.Balance.IsControll())
			{
				if (Skijump_Define.CM.IsStickTilt(userType))
				{
					CalcManager.mCalcVector3.x = Skijump_Define.CM.GetLStickDir(userType).x;
					if (Skijump_Define.CM.GetLStickDir(userType).x > 0f)
					{
						if (Skijump_Define.GUIM.Balance.IsArrowNeutral(0))
						{
							Skijump_Define.GUIM.Balance.SetArrowAnimation(0, Skijump_BalanceController.ArrowAniType.EXPAND);
						}
						Skijump_Define.GUIM.Balance.SetArrowAnimation(1, Skijump_BalanceController.ArrowAniType.NEUTRAL);
					}
					else
					{
						if (Skijump_Define.GUIM.Balance.IsArrowNeutral(1))
						{
							Skijump_Define.GUIM.Balance.SetArrowAnimation(1, Skijump_BalanceController.ArrowAniType.EXPAND);
						}
						Skijump_Define.GUIM.Balance.SetArrowAnimation(0, Skijump_BalanceController.ArrowAniType.NEUTRAL);
					}
					Skijump_Define.GUIM.Balance.MarkControll(CalcManager.mCalcVector3);
				}
			}
			else
			{
				for (int i = 0; i < 2; i++)
				{
					Skijump_Define.GUIM.Balance.SetArrowAnimation(i, Skijump_BalanceController.ArrowAniType.NEUTRAL);
				}
			}
			if (Skijump_Define.GUIM.Landing.CheckState(Skijump_TimingGauge.State.MOVE) && Skijump_Define.CM.IsPushButton_A(userType, Skijump_ControllerManager.ButtonPushType.DOWN))
			{
				Standing();
			}
			break;
		case Skijump_Define.OperationState.LANDING:
			LandingState();
			break;
		case Skijump_Define.OperationState.JUMP_FINISH:
			JumpFinishState();
			break;
		}
	}
	private void SlideGaugeStart()
	{
		if (!isSlideGaugeStart && slideStateNo > 0)
		{
			isSlideGaugeStart = true;
			takeOffDistance = Skijump_Define.MSM.GetTakeOffDistance(GetPos());
			Skijump_Define.GUIM.Landing.ShowDescription(_show: false, 1);
			Skijump_Define.GUIM.Jump.ShowDescription(_show: true, 2);
			UnityEngine.Debug.Log("SlideGaugeStart");
		}
	}
	private void TakeOff()
	{
		UnityEngine.Debug.Log("TakeOff");
		isTakeOff = false;
		Skijump_Define.GUIM.Jump.CngStateMoveStop(_success: true, isSkip);
		slideStateNo++;
		Skijump_Define.GUIM.Jump.ShowDescription(_show: false, 1);
		isSlideGaugeStart = false;
		CngStateJump();
		isInputTakeOff = true;
	}
	public void RetryLanding()
	{
		standingStandbyNo = 1;
		isGlide = false;
		motionTime = Skijump_Define.MOTION_TIME_STANDARD;
		playAni.SetMotion(CharacterPlayAnimation.MotionType.JUMP, motionTime);
	}
	public void Standing()
	{
		Skijump_Define.GUIM.Landing.CngStateMoveStop();
		Skijump_Define.GUIM.Landing.ShowDescription(_show: false, 0);
		if (!IsCpu)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
		}
	}
	private void Float()
	{
		float num = Skijump_Define.MSM.CalcJumpDistance(GetLandingPos()) / jumpDistance;
		num = Mathf.Clamp(1f - (num - landingStartPos) / (1f - landingStartPos), 0f, 1f);
		num = LeanTween.easeOutQuint(1f, 0f, num);
		CalcManager.mCalcVector3 = rigid.position;
		CalcManager.mCalcVector3.y = Mathf.Lerp(descentStartHeight, 0f, num);
		CalcManager.mCalcVector3.y += GetLandingPos().y + (skiBoardThickness + GetCharaHeight() * 0.5f);
		rigid.position = Vector3.Lerp(rigid.position, CalcManager.mCalcVector3, descentSpeed * (2f - num) * Time.deltaTime);
	}
	private IEnumerator _MissLandingMotion()
	{
		float corr4 = 0f;
		float add = 0.05f;
		while (true)
		{
			playAni.SetMotion(CharacterPlayAnimation.MotionType.LANDING_MISS_0, motionTime + corr4);
			yield return new WaitForSeconds(motionTime + corr4);
			corr4 += add;
			playAni.SetMotion(CharacterPlayAnimation.MotionType.LANDING_MISS_1, motionTime + corr4);
			yield return new WaitForSeconds(motionTime + corr4);
			corr4 += add;
			playAni.SetMotion(CharacterPlayAnimation.MotionType.LANDING_MISS_2, motionTime + corr4);
			yield return new WaitForSeconds(motionTime + corr4);
			corr4 += add;
			playAni.SetMotion(CharacterPlayAnimation.MotionType.LANDING_MISS_3, motionTime + corr4);
			yield return new WaitForSeconds(motionTime + corr4);
			corr4 += add;
		}
	}
	public void HideCharacter()
	{
		base.gameObject.SetActive(value: false);
	}
	protected void SettingCharaParameter()
	{
		statusData.slidePower = paramColcData.slidePower[0] + paramColcData.slidePower[1] * (float)speedParameter / 10f;
		statusData.gaugeAreaMag = paramColcData.gaugeAreaMag[0] + paramColcData.gaugeAreaMag[1] * (float)techniqueParameter / 10f;
		statusData.takeOffPower = paramColcData.takeOffPower[0] + paramColcData.takeOffPower[1] * (float)accelParameter / 10f;
		statusData.liftPower = paramColcData.liftPower[0] + paramColcData.liftPower[1] * (float)staminaParameter / 10f;
	}
	private float GetDistanceToGround()
	{
		if (Physics.Raycast(GetPos(), Vector3.down, out rayHit, Skijump_Define.RAY_DISTANCE_MAX, Skijump_Define.GetLayerMask("Collision_Obj_1") | Skijump_Define.GetLayerMask("Field")))
		{
			return Mathf.Abs(GetPos().y - GetCharaHeight() * 0.5f - skiBoardThickness - rayHit.point.y);
		}
		return 0f;
	}
	private Vector3 GetLandingPos()
	{
		if (Physics.Raycast(GetPos(), Vector3.down, out rayHit, Skijump_Define.RAY_DISTANCE_MAX, Skijump_Define.GetLayerMask("Collision_Obj_1") | Skijump_Define.GetLayerMask("Field")))
		{
			return rayHit.point;
		}
		return GetPos();
	}
	public float GetLimitTime()
	{
		if (timeLimit >= slideLimitTime)
		{
			return 0f;
		}
		return Mathf.Clamp(slideLimitTime - timeLimit + 1f, 0f, slideLimitTime);
	}
	public Vector3 GetPos(bool _isLocal = false)
	{
		if (_isLocal)
		{
			return base.transform.localPosition;
		}
		return base.transform.position;
	}
	public float GetCharaHeight()
	{
		return charaHeight + skiBoardThickness;
	}
	private void OnCollisionEnter(Collision _col)
	{
		if (operationState == Skijump_Define.OperationState.JUMP && standingStandbyNo != 0)
		{
			CngStateLanding();
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
