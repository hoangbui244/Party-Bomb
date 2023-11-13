using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class Hidden_CharacterScript : MonoBehaviour
{
	public enum OniType
	{
		Oni,
		Escaper
	}
	public enum ActionType
	{
		Default,
		RotY,
		Door,
		RotFloor,
		Scroll
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
		public Transform Parts(BodyPartsList _parts)
		{
			return rendererList[(int)_parts].transform;
		}
		public Transform Parts(int _parts)
		{
			return rendererList[_parts].transform;
		}
	}
	public enum CpuType
	{
		Weak,
		Normal,
		Strong
	}
	public enum AiMoveType
	{
		None,
		Oni_Attack,
		Oni_Search,
		Escaper_Standby,
		Escaper_Walk,
		Escaper_Run
	}
	private const float GRAVITY = 9.8f;
	private const float MASS = 50f;
	public const float SLOW_WALK_SPEED = 0.25f;
	public const float WALK_SPEED = 0.5f;
	public const float RUN_SPEED = 0.9f;
	public const float RUN_SPEED_MAG_WEAK = 0.9f;
	public const float RUN_SPEED_MAG_NORMAL = 1f;
	public const float RUN_SPEED_MAG_STRONG = 1.05f;
	private const float ONI_SPEED_MAG = 1.1f;
	private const float JUMP_POWER = 6f;
	private const float ATTACK_INIT_SPEED = 4.5f;
	public const float NEAR_DISTANCE = 2f;
	private const float STAMINA_MAX = 100f;
	private const float STAMINA_RUN_COST_SPEED = 20f;
	private const float JUMP_STAMINA_COST = 0f;
	private const float ATTACK_STAMINA_COST = 10f;
	private const float DODGE_STAMINA_COST = 20f;
	private static readonly float[] STAMINA_RECOVERY_VALUE = new float[3]
	{
		15f,
		20f,
		30f
	};
	private const float INVINCIBLE_DURATION = 5f;
	private const float DAMAGE_DURATION = 1f;
	private const float DAMAGE_INTERVAL = 2f;
	private const float DAMAGE_ANIMATION_LERP_TIME = 0.2f;
	private const float HEAVE_RUN_SPEED = 3f;
	private const float HEAVE_WALK_SPEED = 1.5f;
	private const float HEAVE_RUN_VALUE = 0.01f;
	private const float HEAVE_WALK_VALUE = 0.01f;
	private const string TAG_PLAYER = "Player";
	private const string TAG_FIELD = "Respawn";
	private const string TAG_OBJECT = "EditorOnly";
	private const string TAG_WALL = "Finish";
	private const string TAG_DAMAGE = "Failure";
	private const string TAG_ACTION_OBJECT_ROT_Y = "Bowling_Pin";
	private const string TAG_ACTION_OBJECT_DOOR = "Bowling_Gutter";
	private const string TAG_ACTION_OBJECT_ROT_FLOOR = "Bowling_Lane";
	private const string TAG_ACTION_OBJECT_SCROLL = "Bowling_Ball";
	private const string TAG_AI_JUMP = "Airplane";
	public const int LAYER_FIELD = 1048576;
	private const int LAYER_NO_PLAYER_1 = 8;
	private const int LAYER_NO_PLAYER_2 = 9;
	private const int LAYER_NO_PLAYER_3 = 10;
	private const int LAYER_NO_PLAYER_4 = 11;
	public const int LAYER_NO_CHARA_1 = 28;
	public const int LAYER_NO_CHARA_2 = 29;
	public const int LAYER_NO_CHARA_3 = 30;
	public const int LAYER_NO_CHARA_4 = 31;
	private int playerNo;
	private bool isPlayer;
	private bool isCpuCamera;
	private int charaNo;
	private int styleCharaNo;
	private int score;
	private int nowFieldNo = -1;
	private int prevFieldNo = -1;
	private float nowFieldChangeTime;
	private Rigidbody rigid;
	private Vector3[] calcVec = new Vector3[2];
	private Vector3 prevPos;
	private Vector3 nowPos;
	private Vector3 nowMoveDir;
	private float staminaValue;
	private float moveSpeed = 50f;
	private float moveSpeedMax = 7.5f;
	private float nowMoveSpeed;
	private float cpuRunSpeedMag = 1f;
	private float heaveTime;
	private float startBodyLocalPosY;
	[SerializeField]
	private AnimationCurve runHeaveCurve;
	private OniType oniType;
	private ActionType nowActionType;
	private ActionType onTriggerActionType;
	private Collider onTriggerActionCollider;
	private bool isOnActionObject;
	private Hidden_ActionObjectBase actionObjectBase;
	protected CapsuleCollider charaCollider;
	private float charaRadius;
	protected float charaBodySize;
	protected float charaHeight;
	[SerializeField]
	[Header("オブジェクト")]
	protected Transform obj;
	[SerializeField]
	[Header("オブジェクト回転アンカ\u30fc")]
	private Transform objRotAnchor;
	[SerializeField]
	[Header("アクション表示")]
	private SpriteRenderer actionMark;
	[SerializeField]
	[Header("アクションボタン表示")]
	private JoyConButton actionButton;
	[SerializeField]
	[Header("アクションテキスト表示")]
	private TextMeshPro actionText;
	[SerializeField]
	[Header("タッチ表示")]
	private SpriteRenderer touchMark;
	[SerializeField]
	[Header("カメラタ\u30fcゲット")]
	private Transform cameraTarget;
	private Transform cameraRotAnchor;
	private Transform ctrlDirAnchor;
	private bool isStartDashCharaRot;
	private bool isEndDashCharaRot = true;
	private bool isGrounded = true;
	private bool isGravity = true;
	private bool isAttack;
	private bool isDodge;
	private bool isJump;
	private bool isActionAfter;
	private bool isActioButton;
	private Vector3 inputDirInAttack;
	private bool isSwitchOnAction;
	private float actionAnimLength;
	private float actionAnimStartTime;
	private bool isActionObjectForward;
	private bool isFreeze;
	private bool isDamage;
	private float damageStartTime;
	public bool isInvincible;
	private Hidden_CharacterScript targetTouchChara;
	[SerializeField]
	[Header("移動エフェクト")]
	private ParticleSystem moveEffect;
	[SerializeField]
	[Header("汗エフェクト")]
	private ParticleSystem sweatEffect;
	[SerializeField]
	[Header("吹き飛ばしエフェクト")]
	private ParticleSystem breakEffect;
	[SerializeField]
	[Header("キャラスタイル")]
	private CharacterStyle charaStyle;
	[SerializeField]
	[Header("鬼オブジェクト")]
	private GameObject oniObj;
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	private float runAnimationSpeed = 30f;
	private float runAnimationTime;
	private int playSeRunCnt;
	private float runSeInterval;
	private bool isChangeAnimationNeutral;
	private float actionChangeInterval;
	private float movePosChangeInterval;
	private bool isTouchAnim;
	private float touchAnimStartTime;
	private const float AI_ROT_SPEED = 10f;
	private const float AI_STOP_DISTANCE = 0.2f;
	private const float AI_STANDBY_UPDATE_INTERVAL = 3f;
	private const float AI_RUN_IMMEDIATE_ROT_INTERVAL = 1f;
	private const float AI_TOUCH_WAIT_TIME_WEAK = 0.2f;
	private const float AI_TOUCH_WAIT_TIME_NORMAL = 0.1f;
	private const float AI_TOUCH_WAIT_TIME_STRONG = 0.01f;
	private static readonly Vector3 AI_HALF_EXTENTS = new Vector3(0.25f, 0.01f, 0.01f);
	private const float AI_STAMINA_RECOVERY_START_VALUE = 10f;
	private const float AI_STAMINA_RECOVERY_END_VALUE = 90f;
	private bool isAiStaminaRecovery;
	private Vector3 aiTargetPos;
	private bool isAiFreeze;
	private float aiFreezeTime;
	private float aiUpdateInterval = -1f;
	private const float AI_ACTION_CHECK_INTERVAL = 0.1f;
	private float aiActionCheckInterval;
	private float aiStandbyUpdateInterval = -1f;
	private int aiTargetCharaNo = -1;
	private int aiTargetFieldNo = -1;
	private Transform aiTargetTrans;
	private bool isAiTargetChara;
	private float aiStrengthTouchWaitTime;
	private float aiTouchWaitTimer;
	private float aiRunImmediateRotTime;
	private CpuType cpuType;
	private bool isAiJump;
	private AiMoveType aiMoveType;
	private float aiMoveTypeChangeTime;
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
	public bool IsCpuCamera
	{
		get
		{
			return isCpuCamera;
		}
		set
		{
			isCpuCamera = value;
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
	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			score = value;
		}
	}
	public int NowFieldNo
	{
		get
		{
			return nowFieldNo;
		}
		set
		{
			nowFieldNo = value;
		}
	}
	public int PrevFieldNo
	{
		get
		{
			return prevFieldNo;
		}
		set
		{
			prevFieldNo = value;
		}
	}
	public float NowFieldChangeTime
	{
		get
		{
			return nowFieldChangeTime;
		}
		set
		{
			nowFieldChangeTime = value;
		}
	}
	public bool IsChangePos => ChangePosVecZeroY.sqrMagnitude > 0.0001f;
	private Vector3 ChangePosVecZeroY
	{
		get
		{
			Vector3 result = nowPos - prevPos;
			result.y = 0f;
			return result;
		}
	}
	public bool IsMove => nowMoveDir.sqrMagnitude > 0.001f;
	public bool IsFreeze => isFreeze;
	public bool IsInvincible => isInvincible;
	public Hidden_CharacterScript TargetTouchChara => targetTouchChara;
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
	public int AiTargetCharaNo
	{
		get
		{
			return aiTargetCharaNo;
		}
		set
		{
			aiTargetCharaNo = value;
		}
	}
	public int AiTargetFieldNo
	{
		get
		{
			return aiTargetFieldNo;
		}
		set
		{
			aiTargetFieldNo = value;
		}
	}
	public Transform AiTargetTrans
	{
		get
		{
			return aiTargetTrans;
		}
		set
		{
			aiTargetTrans = value;
		}
	}
	public bool IsAiTargetChara
	{
		get
		{
			return isAiTargetChara;
		}
		set
		{
			isAiTargetChara = value;
		}
	}
	public float AiMoveTypeChangeTime => aiMoveTypeChangeTime;
	public void Init(int _charaNo, OniType _oniType)
	{
		rigid = GetComponent<Rigidbody>();
		charaCollider = obj.GetComponent<CapsuleCollider>();
		charaRadius = charaCollider.radius;
		charaBodySize = charaRadius * obj.localScale.x * 2f;
		charaHeight = charaCollider.height * obj.localScale.y;
		base.gameObject.name = base.gameObject.name + "_" + _charaNo.ToString();
		charaNo = _charaNo;
		playerNo = -1;
		oniType = _oniType;
		runAnimationTime = 0f;
		playSeRunCnt = 0;
		nowPos = (prevPos = base.transform.position);
		staminaValue = 100f;
		startBodyLocalPosY = bodyParts.bodyAnchor.transform.localPosition.y;
		switch (charaNo)
		{
		case 0:
			actionMark.gameObject.layer = 8;
			for (int k = 0; k < actionMark.transform.childCount; k++)
			{
				actionMark.transform.GetChild(k).gameObject.layer = 8;
			}
			touchMark.gameObject.layer = 8;
			actionButton.SetPlayerType(JoyConButton.PlayerType.PLAYER_1);
			break;
		case 1:
			actionMark.gameObject.layer = 9;
			for (int j = 0; j < actionMark.transform.childCount; j++)
			{
				actionMark.transform.GetChild(j).gameObject.layer = 9;
			}
			touchMark.gameObject.layer = 9;
			actionButton.SetPlayerType(JoyConButton.PlayerType.PLAYER_2);
			break;
		case 2:
			actionMark.gameObject.layer = 10;
			for (int l = 0; l < actionMark.transform.childCount; l++)
			{
				actionMark.transform.GetChild(l).gameObject.layer = 10;
			}
			touchMark.gameObject.layer = 10;
			actionButton.SetPlayerType(JoyConButton.PlayerType.PLAYER_3);
			break;
		case 3:
			actionMark.gameObject.layer = 11;
			for (int i = 0; i < actionMark.transform.childCount; i++)
			{
				actionMark.transform.GetChild(i).gameObject.layer = 11;
			}
			touchMark.gameObject.layer = 11;
			actionButton.SetPlayerType(JoyConButton.PlayerType.PLAYER_4);
			break;
		}
		actionButton.CheckJoyconButton();
		if (Localize_Define.Language != 0)
		{
			actionText.characterSpacing = 0f;
		}
		SettingCharacterBaseLayer();
		nowFieldChangeTime = Time.time + 360000f;
		aiTargetTrans = base.transform;
		aiMoveTypeChangeTime = UnityEngine.Random.Range(-1f, 0f);
	}
	public void UpdateMethod()
	{
		prevPos = nowPos;
		nowPos = base.transform.position;
		UpdateFieldNo();
		if (isPlayer && SingletonCustom<Hidden_ControllerManager>.Instance.GetJumpButtonDown(playerNo) && nowActionType == ActionType.Default)
		{
			Jump();
		}
		if (isPlayer && SingletonCustom<Hidden_ControllerManager>.Instance.GetActionButtonDown(playerNo) && isOnActionObject && nowActionType == ActionType.Default && isGrounded && !isAttack && !isDodge && !isActionAfter)
		{
			actionObjectBase = onTriggerActionCollider.GetComponent<Hidden_ActionObjectBase>();
			if (actionObjectBase != null && !actionObjectBase.IsAction)
			{
				PlayAction();
			}
		}
		bodyParts.bodyAnchor.transform.SetLocalPositionY(startBodyLocalPosY);
		if (!isAttack && !isDodge)
		{
			if (nowMoveSpeed <= 0.01f)
			{
				staminaValue += Time.deltaTime * STAMINA_RECOVERY_VALUE[2];
				if (staminaValue > 100f)
				{
					staminaValue = 100f;
				}
				if (!isActionAfter)
				{
					Vector3 velocity = rigid.velocity;
					velocity.x = (velocity.z = 0f);
					rigid.velocity = velocity;
				}
			}
			if (nowMoveSpeed <= 0.25f)
			{
				staminaValue += Time.deltaTime * STAMINA_RECOVERY_VALUE[1];
				if (staminaValue > 100f)
				{
					staminaValue = 100f;
				}
			}
			else if (nowMoveSpeed <= 0.5f)
			{
				staminaValue += Time.deltaTime * STAMINA_RECOVERY_VALUE[0];
				if (staminaValue > 100f)
				{
					staminaValue = 100f;
				}
				if (isGrounded && IsChangePos)
				{
					heaveTime += 1.5f * Time.deltaTime;
					bodyParts.bodyAnchor.transform.AddLocalPositionY(runHeaveCurve.Evaluate(heaveTime) * 0.01f);
				}
			}
			else
			{
				staminaValue -= Time.deltaTime * 20f;
				if (staminaValue < 0f)
				{
					staminaValue = 0f;
				}
				else if (isGrounded && IsChangePos)
				{
					heaveTime += 3f * Time.deltaTime;
					bodyParts.bodyAnchor.transform.AddLocalPositionY(runHeaveCurve.Evaluate(heaveTime) * 0.01f);
				}
			}
		}
		ParticleSystem.MainModule main = sweatEffect.main;
		if (staminaValue <= 0f || (isAiStaminaRecovery && staminaValue < 50f))
		{
			main.loop = true;
			if (main.loop && !sweatEffect.isPlaying)
			{
				sweatEffect.Play();
			}
		}
		else
		{
			main.loop = false;
		}
		runSeInterval -= Time.deltaTime;
		if (cameraRotAnchor != null && !isAttack && !isDodge && !isActionAfter && nowActionType == ActionType.Default)
		{
			if (IsChangePos && IsMove)
			{
				Vector3 zero = Vector3.zero;
				zero.y = CalcManager.Rot(obj.forward, CalcManager.AXIS.Y);
				Vector3 vec = nowMoveDir;
				vec.y = 0f;
				Vector3 zero2 = Vector3.zero;
				zero2.y = CalcManager.Rot(vec, CalcManager.AXIS.Y);
				obj.rotation = Quaternion.Lerp(Quaternion.Euler(zero), Quaternion.Euler(zero2), 30f * Time.deltaTime);
			}
			UpdateObjRotAnchor();
			if (onTriggerActionType != 0)
			{
				int childCount = onTriggerActionCollider.transform.childCount;
			}
		}
		nowMoveSpeed = 0f;
		if (nowActionType == ActionType.Door)
		{
			float num = Mathf.Clamp01((Time.time - actionAnimStartTime) / actionAnimLength);
			if (isSwitchOnAction)
			{
				num = 1f - num;
			}
			ActionDoorAnimation(num);
		}
		else if (nowActionType == ActionType.RotFloor)
		{
			float lerp = Mathf.Clamp01((Time.time - actionAnimStartTime) / actionAnimLength * 2f);
			ActionRotFloorAnimation(lerp);
		}
		else if (nowActionType == ActionType.Scroll)
		{
			float num2 = Mathf.Clamp01((Time.time - actionAnimStartTime) / actionAnimLength);
			if (isSwitchOnAction)
			{
				num2 = 1f - num2;
			}
			ActionScrollAnimation(num2, isActionObjectForward);
		}
	}
	public void UpdateFieldNo()
	{
		int num = SingletonCustom<Hidden_FieldManager>.Instance.SearchFieldNo(nowPos);
		if (nowFieldNo != num)
		{
			prevFieldNo = nowFieldNo;
			nowFieldNo = num;
			nowFieldChangeTime = Time.time;
		}
	}
	public void UpdateObjRotAnchor()
	{
	}
	public void ActionMarkUpdate()
	{
		bool flag = isPlayer && isOnActionObject && nowActionType == ActionType.Default;
		if (actionMark.gameObject.activeSelf != flag)
		{
			actionMark.gameObject.SetActive(flag);
		}
		if (flag)
		{
			actionMark.transform.forward = actionMark.transform.position - SingletonCustom<Hidden_CharacterManager>.Instance.GetCamera(charaNo).transform.position;
		}
	}
	public void TouchMarkUpdate()
	{
		bool flag = oniType == OniType.Oni && targetTouchChara != null;
		if (touchMark.gameObject.activeSelf != flag)
		{
			touchMark.gameObject.SetActive(flag);
		}
		if (flag)
		{
			touchMark.transform.forward = touchMark.transform.position - SingletonCustom<Hidden_CharacterManager>.Instance.GetCamera(charaNo).transform.position;
		}
	}
	public void MarkObjEnd()
	{
		actionMark.gameObject.SetActive(value: false);
		touchMark.gameObject.SetActive(value: false);
	}
	public void Move(Vector3 _moveDir, float _moveSpeed)
	{
		nowMoveDir = ctrlDirAnchor.localRotation * _moveDir;
		if (isDamage)
		{
			nowMoveDir = Vector3.zero;
			_moveSpeed = 0f;
			Vector3 velocity = rigid.velocity;
			velocity.x = 0f;
			velocity.z = 0f;
			rigid.velocity = velocity;
			float num = Time.time - damageStartTime;
			DamageAnimation(Mathf.Clamp01(num / 0.2f), num);
		}
		else if (isAttack)
		{
			inputDirInAttack = ctrlDirAnchor.localRotation * nowMoveDir;
			AttackAnimation();
		}
		else
		{
			if (CheckActionMove(_moveDir, _moveSpeed) || isActionAfter || rigid.isKinematic)
			{
				return;
			}
			Vector3 a = nowMoveDir;
			nowMoveSpeed = _moveSpeed;
			_moveSpeed = ((_moveSpeed <= 0.25f) ? 0.25f : ((!(_moveSpeed <= 0.5f)) ? 0.9f : 0.5f));
			if (staminaValue <= 0f && _moveSpeed >= 0.5f)
			{
				_moveSpeed = 0.25f;
			}
			if (oniType == OniType.Oni)
			{
				_moveSpeed *= 1.1f;
			}
			if (!isPlayer)
			{
				_moveSpeed *= cpuRunSpeedMag;
			}
			a *= moveSpeed * _moveSpeed;
			Vector3 vector = rigid.velocity;
			float y = vector.y;
			vector.y = 0f;
			rigid.velocity = vector;
			vector += a * Time.deltaTime;
			if (vector.magnitude > moveSpeedMax * _moveSpeed)
			{
				vector = vector.normalized * moveSpeedMax * _moveSpeed;
			}
			vector.y = y;
			rigid.velocity = vector;
			if (isTouchAnim)
			{
				if (Time.time - touchAnimStartTime < 0.1f)
				{
					AttackAnimation();
					return;
				}
				isTouchAnim = false;
				ResetAnimation();
				MoveAnimation();
			}
			else
			{
				MoveAnimation();
			}
		}
	}
	private void MoveRot(Vector3 _moveDir, bool _immediate = false)
	{
		if (!isAttack && nowActionType == ActionType.Default && IsMove)
		{
			calcVec[0] = _moveDir;
			Vector3 zero = Vector3.zero;
			zero.y = CalcManager.Rot(calcVec[0], CalcManager.AXIS.Y);
			if (_immediate)
			{
				base.transform.rotation = Quaternion.Euler(zero);
			}
			else
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(zero), 30f * Time.deltaTime);
			}
		}
	}
	private bool CheckActionMove(Vector3 _moveDir, float _moveSpeed)
	{
		switch (nowActionType)
		{
		default:
			return nowActionType != ActionType.Default;
		}
	}
	private void ResetAnimation()
	{
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, 0.05483828f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalPosition(-0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.1511168f, 0.1297733f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.1511168f, 0.1297733f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
	}
	private void MoveAnimation()
	{
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * -30f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * 30f, 0f, 0f);
		if (isDodge)
		{
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAnglesZ(180f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAnglesZ(180f);
		}
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * 30f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * -30f);
		if (IsChangePos)
		{
			runAnimationTime += ChangePosVecZeroY.magnitude * runAnimationSpeed * Time.deltaTime;
			if (runAnimationTime >= (float)playSeRunCnt * 0.5f)
			{
				playSeRunCnt++;
				if (isGrounded && staminaValue > 0f && nowMoveSpeed > 0.5f)
				{
					moveEffect.Emit(1);
				}
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
	private void AttackAnimation()
	{
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, -30f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(10.735f, -11.385f, -6.521f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(55.651f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(288.906f, 6.106f, -12.504f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(-78.65701f, 72.608f, -35.664f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(-8.587001f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * 30f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * -30f);
		if (IsChangePos)
		{
			runAnimationTime += ChangePosVecZeroY.magnitude * runAnimationSpeed * Time.deltaTime;
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
	private void SitAnimation()
	{
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(-90f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAnglesX(-90f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalPosition(-0.054f, -0.014f, 0.094f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.014f, 0.094f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAnglesX(-90f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAnglesX(-90f);
	}
	private void SwingSitAnimation()
	{
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 290.4674f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 69.5326f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAnglesX(270f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAnglesX(270f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalPosition(-0.054f, -0.014f, 0.094f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.014f, 0.094f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAnglesX(-90f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAnglesX(-90f);
	}
	private void DamageAnimation(float _lerpA, float _lerpB)
	{
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, 0.05483828f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 15f * Mathf.Sin(_lerpB * 4f * (float)Math.PI), 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(-15f * _lerpA, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalPosition(-0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.1511168f, 0.1297733f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.1511168f, 0.1297733f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 135f * _lerpA);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, -135f * _lerpA);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
	}
	private void ActionDoorAnimation(float _lerp)
	{
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, 0.05483828f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalPosition(-0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(270f, 45f * _lerp, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(270f, -45f * _lerp, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
	}
	private void ActionRotYAnimation()
	{
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, 0.05483828f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalPosition(-0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 60f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, -60f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
	}
	private void ActionRotFloorAnimation(float _lerp)
	{
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, 0.05483828f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(30f - 30f * _lerp, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalPosition(-0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).localRotation = new Quaternion(Mathf.Lerp(0.5f, 0.8660254f, _lerp), 0f, 0f, Mathf.Lerp(-0.8660254f, -0.5f, _lerp));
		bodyParts.Parts(BodyPartsList.SHOULDER_L).localRotation = new Quaternion(Mathf.Lerp(0.5f, 0.8660254f, _lerp), 0f, 0f, Mathf.Lerp(-0.8660254f, -0.5f, _lerp));
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
	}
	private void ActionScrollAnimation(float _lerp, bool _isForward)
	{
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, 0.05483828f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalPosition(-0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
		if (_isForward)
		{
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(15f - 15f * _lerp, 25f - 25f * _lerp, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.1511168f, 0.1297733f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.1511168f, 0.1297733f, 0.1f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 0f);
			Quaternion a = Quaternion.Euler(270f, 20f, 0f);
			Quaternion b = Quaternion.Euler(300f, 150f, 180f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).localRotation = Quaternion.Lerp(a, b, _lerp);
		}
		else
		{
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f + 15f * _lerp, -25f + 25f * _lerp, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.1511168f, 0.1297733f, 0.1f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.1511168f, 0.1297733f, 0f);
			Quaternion a2 = Quaternion.Euler(270f, -20f, 0f);
			Quaternion b2 = Quaternion.Euler(300f, 210f, 180f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).localRotation = Quaternion.Lerp(a2, b2, _lerp);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 0f);
		}
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
	}
	public void Jump()
	{
		if (isGrounded && !isFreeze && !isDamage && !isActionAfter && !(staminaValue < 0f))
		{
			Vector3 velocity = rigid.velocity;
			velocity.y = 6f;
			rigid.velocity = velocity;
			isGrounded = false;
			isJump = true;
			if (isPlayer)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_run");
			}
			staminaValue -= 0f;
		}
	}
	public void Attack()
	{
		if (!isAttack && !isActionAfter && !(staminaValue < 10f))
		{
			isAttack = true;
			AttackAnimation();
			Vector3 moveDir = GetMoveDir();
			obj.SetEulerAnglesY(CalcManager.Rot(moveDir, CalcManager.AXIS.Y));
			if (isPlayer)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_oni_sliding");
			}
			StartCoroutine(_Attack(moveDir));
			staminaValue -= 10f;
		}
	}
	private IEnumerator _Attack(Vector3 _dir)
	{
		float attackTime = 1f;
		float timer = 0f;
		while (timer < attackTime)
		{
			float num = LeanTween.easeInQuad(0f, 1f, attackTime - timer * 0.5f / attackTime);
			Vector3 vector = Quaternion.Euler(0f, 90f, 0f) * _dir;
			float d = Vector3.Dot(vector, inputDirInAttack);
			inputDirInAttack = Vector3.zero;
			Vector3 b = vector * d * 2f * num;
			Vector3 a = _dir;
			a *= num * 4.5f;
			a.y = rigid.velocity.y;
			rigid.velocity = a + b;
			timer += Time.deltaTime;
			yield return null;
		}
		if (!isPlayer)
		{
			obj.localRotation = Quaternion.identity;
		}
		isAttack = false;
		ResetAnimation();
	}
	public void Dodge()
	{
		if (!isDodge && !isActionAfter && !(staminaValue < 20f))
		{
			isDodge = true;
			charaCollider.radius = charaRadius / 2f;
			LeanTween.rotateAround(obj.gameObject, Vector3.up, 360f, 0.6f).setOnComplete((Action)delegate
			{
				isDodge = false;
				charaCollider.radius = charaRadius;
				if (!isPlayer)
				{
					obj.SetLocalEulerAnglesY(0f);
				}
			});
			if (isPlayer)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_oni_dodge");
			}
			staminaValue -= 20f;
		}
	}
	private void PlaySeRun()
	{
		if (isPlayer && isGrounded)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_run");
		}
	}
	private void FixedUpdate()
	{
		if (isGravity)
		{
			rigid.AddForce(Vector3.down * 9.8f, ForceMode.Acceleration);
		}
	}
	private void OnCollisionStay(Collision _col)
	{
		if (SingletonCustom<Hidden_GameManager>.Instance.IsGameEnd)
		{
			return;
		}
		if (_col.gameObject.tag == "Player")
		{
			if (CheckCanTouchControl())
			{
				OniTouch(_col.gameObject.GetComponent<Hidden_CharacterScript>());
			}
		}
		else
		{
			if (!(_col.gameObject.tag == "Respawn") && !(_col.gameObject.tag == "EditorOnly"))
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < _col.contacts.Length)
				{
					if (Vector3.Dot(_col.contacts[num].normal, Vector3.up) > 0.7f)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			if (isGrounded)
			{
				isJump = false;
			}
			isGrounded = true;
		}
	}
	private void OnCollisionExit(Collision _col)
	{
		if (_col.gameObject.tag == "Respawn" || _col.gameObject.tag == "EditorOnly")
		{
			isGrounded = false;
		}
	}
	private void OnTriggerEnter(Collider _col)
	{
		if (_col.gameObject.tag == "Bowling_Pin")
		{
			isOnActionObject = true;
			onTriggerActionType = ActionType.RotY;
			onTriggerActionCollider = _col;
		}
		else if (_col.gameObject.tag == "Bowling_Gutter")
		{
			isOnActionObject = true;
			onTriggerActionType = ActionType.Door;
			onTriggerActionCollider = _col;
		}
		else if (_col.gameObject.tag == "Bowling_Lane")
		{
			isOnActionObject = true;
			onTriggerActionType = ActionType.RotFloor;
			onTriggerActionCollider = _col;
			if (!isPlayer && cpuType == CpuType.Strong)
			{
				if (actionObjectBase == null)
				{
					actionObjectBase = onTriggerActionCollider.GetComponent<Hidden_ActionObjectBase>();
				}
				if (actionObjectBase.IsSwitchOn)
				{
					isAiJump = true;
				}
			}
		}
		else if (_col.gameObject.tag == "Bowling_Ball")
		{
			isOnActionObject = true;
			onTriggerActionType = ActionType.Scroll;
			onTriggerActionCollider = _col;
		}
		else if (_col.gameObject.tag == "Failure")
		{
			if (!isDamage && Time.time - damageStartTime > 2f)
			{
				isDamage = true;
				damageStartTime = Time.time;
				charaStyle.SetMainCharacterFaceDiff(playerNo, StyleTextureManager.MainCharacterFaceType.SAD);
				if (isPlayer || SingletonCustom<Hidden_CharacterManager>.Instance.CheckViewCpuCarNo())
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_bad");
				}
				if (isPlayer)
				{
					SingletonCustom<HidVibration>.Instance.SetCustomVibration(playerNo, HidVibration.VibrationType.Normal);
				}
				StartCoroutine(_DamageDirection());
			}
		}
		else if (!isPlayer && _col.gameObject.tag == "Airplane")
		{
			isAiJump = true;
			aiMoveTypeChangeTime = Time.time;
		}
	}
	private void OnTriggerExit(Collider _col)
	{
		if (_col.gameObject.tag == "Bowling_Pin")
		{
			isOnActionObject = false;
			onTriggerActionType = ActionType.Default;
			actionObjectBase = null;
		}
		else if (_col.gameObject.tag == "Bowling_Gutter")
		{
			isOnActionObject = false;
			onTriggerActionType = ActionType.Default;
			actionObjectBase = null;
		}
		else if (_col.gameObject.tag == "Bowling_Lane")
		{
			isOnActionObject = false;
			onTriggerActionType = ActionType.Default;
			actionObjectBase = null;
		}
		else if (_col.gameObject.tag == "Bowling_Ball")
		{
			isOnActionObject = false;
			onTriggerActionType = ActionType.Default;
			actionObjectBase = null;
		}
	}
	public void OniTouch(Hidden_CharacterScript _targetChara)
	{
		if (!_targetChara.CheckCanTouchReceive())
		{
			return;
		}
		_targetChara.ChangeOni();
		ChangeEscaper();
		targetTouchChara = null;
		if (isPlayer || _targetChara.isPlayer || SingletonCustom<Hidden_CharacterManager>.Instance.CheckViewCpuCarNo())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_perfect");
			if (isPlayer)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerNo);
			}
			if (_targetChara.isPlayer)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(_targetChara.playerNo);
			}
		}
		SingletonCustom<Hidden_UiManager>.Instance.SetOniView(_targetChara.charaNo);
		SettingTouchAnim();
	}
	private IEnumerator _DamageDirection()
	{
		yield return new WaitForSeconds(1f);
		isDamage = false;
		charaStyle.SetMainCharacterFaceDiff(playerNo, StyleTextureManager.MainCharacterFaceType.NORMAL);
		ResetAnimation();
	}
	private IEnumerator _InvincibleDirection()
	{
		yield return new WaitForSeconds(5f);
		isInvincible = false;
		charaStyle.SetMainCharacterFaceDiff(playerNo, StyleTextureManager.MainCharacterFaceType.NORMAL);
	}
	private IEnumerator _LimitActionAfter(float _limitTime)
	{
		float timer = 0f;
		while (isActionAfter && timer < _limitTime)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		rigid.drag = 0f;
		isActionAfter = false;
		ResetAnimation();
	}
	public void ChangeOni()
	{
		oniType = OniType.Oni;
		if (!IsPlayer)
		{
			SetAiMoveType(AiMoveType.Oni_Search);
			aiTargetFieldNo = -1;
		}
		nowFieldChangeTime = Time.time;
		breakEffect.Play();
		oniObj.SetActive(value: true);
		SingletonCustom<Hidden_CharacterManager>.Instance.EscaperTouchScoreDown(charaNo);
		if (!isPlayer)
		{
			aiUpdateInterval = -1f;
			SetAiFreeze(1f);
		}
		else
		{
			StartCoroutine(_ChangeOniFreeze());
		}
	}
	private IEnumerator _ChangeOniFreeze()
	{
		isFreeze = true;
		rigid.isKinematic = true;
		yield return new WaitForSeconds(1f);
		isFreeze = false;
		rigid.isKinematic = false;
	}
	public void ChangeEscaper()
	{
		oniType = OniType.Escaper;
		if (!IsPlayer)
		{
			SetAiMoveType(AiMoveType.Escaper_Run);
		}
		nowFieldChangeTime = Time.time;
		isInvincible = true;
		charaStyle.SetMainCharacterFaceDiff(playerNo, StyleTextureManager.MainCharacterFaceType.HAPPY);
		StartCoroutine(_InvincibleDirection());
		oniObj.SetActive(value: false);
		SingletonCustom<Hidden_CharacterManager>.Instance.OniTouchScoreUp(charaNo);
	}
	public void SetCharaGameStyle(int _playerNo)
	{
		charaStyle.SetGameStyle(GS_Define.GameType.BLOCK_WIPER, _playerNo);
	}
	private void PlayAction()
	{
		actionObjectBase.PlayAction(this);
		onTriggerActionType = actionObjectBase.GetCharaActionType();
		ChangeActionType(onTriggerActionType);
		isSwitchOnAction = actionObjectBase.IsSwitchOn;
		actionAnimLength = actionObjectBase.ActionTime;
		actionAnimStartTime = Time.time;
		if (!isPlayer)
		{
			aiMoveTypeChangeTime = Time.time;
		}
	}
	public void EndAction()
	{
		ChangeActionType(ActionType.Default);
	}
	public void ChangeActionType(ActionType _type)
	{
		if (nowActionType != _type)
		{
			switch (nowActionType)
			{
			case ActionType.Default:
				obj.SetLocalEulerAnglesY(0f);
				break;
			case ActionType.RotY:
				base.transform.parent = SingletonCustom<Hidden_CharacterManager>.Instance.GetCharacterAnchor();
				rigid.constraints = RigidbodyConstraints.FreezeRotation;
				ResetAnimation();
				break;
			case ActionType.Door:
				base.transform.parent = SingletonCustom<Hidden_CharacterManager>.Instance.GetCharacterAnchor();
				rigid.constraints = RigidbodyConstraints.FreezeRotation;
				ResetAnimation();
				break;
			case ActionType.RotFloor:
				base.transform.parent = SingletonCustom<Hidden_CharacterManager>.Instance.GetCharacterAnchor();
				rigid.constraints = RigidbodyConstraints.FreezeRotation;
				ResetAnimation();
				break;
			case ActionType.Scroll:
				base.transform.parent = SingletonCustom<Hidden_CharacterManager>.Instance.GetCharacterAnchor();
				rigid.constraints = RigidbodyConstraints.FreezeRotation;
				ResetAnimation();
				break;
			}
			nowActionType = _type;
			switch (nowActionType)
			{
			case ActionType.Default:
				break;
			case ActionType.RotY:
				base.transform.parent = actionObjectBase.GetActionTrans(this);
				base.transform.SetLocalPositionX(0f);
				base.transform.SetLocalPositionZ(0f);
				base.transform.localRotation = Quaternion.identity;
				objRotAnchor.localRotation = Quaternion.identity;
				rigid.constraints = RigidbodyConstraints.FreezeAll;
				ActionRotYAnimation();
				break;
			case ActionType.Door:
				base.transform.parent = actionObjectBase.GetActionTrans(this);
				base.transform.SetLocalPositionX(0f);
				base.transform.SetLocalPositionZ(0f);
				base.transform.localRotation = Quaternion.identity;
				objRotAnchor.localRotation = Quaternion.identity;
				rigid.constraints = RigidbodyConstraints.FreezeAll;
				ActionDoorAnimation(isSwitchOnAction ? 1 : 0);
				break;
			case ActionType.RotFloor:
				base.transform.parent = actionObjectBase.GetActionTrans(this);
				base.transform.SetLocalPositionX(0f);
				base.transform.SetLocalPositionZ(0f);
				base.transform.localRotation = Quaternion.identity;
				objRotAnchor.localRotation = Quaternion.identity;
				rigid.constraints = RigidbodyConstraints.FreezeAll;
				ActionRotFloorAnimation(0f);
				break;
			case ActionType.Scroll:
				base.transform.parent = actionObjectBase.GetActionTrans(this);
				base.transform.SetLocalPositionX(0f);
				base.transform.SetLocalPositionZ(0f);
				base.transform.localRotation = Quaternion.identity;
				objRotAnchor.localRotation = Quaternion.identity;
				rigid.constraints = RigidbodyConstraints.FreezeAll;
				isActionObjectForward = (Vector3.Dot(GetDir(), actionObjectBase.transform.forward) > 0f);
				ActionScrollAnimation(isSwitchOnAction ? 1 : 0, isActionObjectForward);
				break;
			}
		}
	}
	public void AddScore(int _add)
	{
		score += _add;
	}
	public void RemoveScore(int _remove)
	{
		score -= _remove;
	}
	public void SettingStartRotation(float _angleY)
	{
		base.transform.SetEulerAnglesY(_angleY);
	}
	public void SettingCtrlDirAnchor()
	{
		if (isPlayer)
		{
			ctrlDirAnchor = cameraRotAnchor;
		}
		else
		{
			ctrlDirAnchor = objRotAnchor;
		}
	}
	public void SettingActionMarkSprite()
	{
	}
	public void SettingTouchAnim()
	{
		isTouchAnim = true;
		touchAnimStartTime = Time.time;
	}
	private void SettingCharacterBaseLayer()
	{
		int layerNo = 28;
		switch (charaNo)
		{
		case 1:
			layerNo = 29;
			break;
		case 2:
			layerNo = 30;
			break;
		case 3:
			layerNo = 31;
			break;
		}
		SettingChildLayer(bodyParts.Parts(BodyPartsList.HIP), layerNo);
	}
	private void SettingChildLayer(Transform _trans, int _layerNo)
	{
		_trans.gameObject.layer = _layerNo;
		for (int i = 0; i < _trans.childCount; i++)
		{
			SettingChildLayer(_trans.GetChild(i), _layerNo);
		}
	}
	public void SettingCpuStrength()
	{
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 0:
			cpuRunSpeedMag = 0.9f;
			cpuType = CpuType.Weak;
			aiStrengthTouchWaitTime = 0.2f;
			break;
		case 1:
			cpuRunSpeedMag = 1f;
			cpuType = CpuType.Normal;
			aiStrengthTouchWaitTime = 0.1f;
			break;
		case 2:
			cpuRunSpeedMag = 1.05f;
			cpuType = CpuType.Strong;
			aiStrengthTouchWaitTime = 0.01f;
			break;
		}
	}
	public bool CheckObj(GameObject _obj)
	{
		if (!(_obj == obj.gameObject))
		{
			return base.gameObject == _obj;
		}
		return true;
	}
	public bool CheckOniType(OniType _oniType)
	{
		return oniType == _oniType;
	}
	public bool CheckCanTouchControl()
	{
		if (CheckOniType(OniType.Oni) && nowActionType == ActionType.Default && !isFreeze)
		{
			return !isDamage;
		}
		return false;
	}
	public bool CheckCanTouchReceive()
	{
		if (CheckOniType(OniType.Escaper) && !isDodge)
		{
			return !isInvincible;
		}
		return false;
	}
	public bool CheckCanGameEndStop()
	{
		return nowActionType == ActionType.Default;
	}
	public Rigidbody GetRigid()
	{
		return rigid;
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
		return obj.forward;
	}
	public Vector3 GetMoveDir()
	{
		Vector3 velocity = rigid.velocity;
		velocity.y = 0f;
		if (velocity.sqrMagnitude < 0.1f)
		{
			return GetDir();
		}
		return velocity.normalized;
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
	public string GetName()
	{
		return base.name;
	}
	public float GetStaminaPer()
	{
		return Mathf.Max(Mathf.Min(staminaValue / 100f, 100f), 0f);
	}
	public OniType GetOniType()
	{
		return oniType;
	}
	public GameObject GetOniObj()
	{
		return oniObj;
	}
	public Transform GetCameraTarget()
	{
		return cameraTarget;
	}
	public void SetOniType(OniType _oniType)
	{
		oniType = _oniType;
	}
	public void SetCameraRotAnchor(Transform _anchor)
	{
		cameraRotAnchor = _anchor;
	}
	public void SetActionButtonFlg(bool _flg)
	{
		isActioButton = _flg;
	}
	public void AiMove()
	{
		if (isAiFreeze)
		{
			aiFreezeTime -= Time.deltaTime;
			if (!(aiFreezeTime < 0f))
			{
				return;
			}
			isFreeze = false;
			isAiFreeze = false;
			rigid.isKinematic = false;
		}
		if (isAiJump && nowActionType == ActionType.Default)
		{
			isAiJump = false;
			Jump();
		}
		switch (aiMoveType)
		{
		case AiMoveType.Oni_Attack:
			AiActionObjectCheck();
			if (cpuType != 0)
			{
				AiTargetRun();
			}
			else
			{
				AiTargetWalk();
			}
			break;
		case AiMoveType.Oni_Search:
			AiActionObjectCheck();
			AiTargetWalk();
			break;
		case AiMoveType.Escaper_Standby:
			AiStandby();
			break;
		case AiMoveType.Escaper_Walk:
			AiActionObjectCheck();
			AiTargetWalk();
			break;
		case AiMoveType.Escaper_Run:
			AiActionObjectCheck();
			if (cpuType != 0)
			{
				AiTargetRun();
			}
			else
			{
				AiTargetWalk();
			}
			break;
		}
		aiActionCheckInterval += Time.deltaTime;
		if (aiActionCheckInterval > 0.1f)
		{
			aiActionCheckInterval -= 0.1f;
			AiActionCheck();
		}
	}
	public void AiTargetRun()
	{
		if (staminaValue < 10f)
		{
			isAiStaminaRecovery = true;
		}
		if (isAiStaminaRecovery)
		{
			if (staminaValue < 90f)
			{
				AiTargetWalk();
				return;
			}
			isAiStaminaRecovery = false;
		}
		Vector3 vector = aiTargetTrans.position - GetPos();
		vector.y = 0f;
		if (!(vector.magnitude < 0.2f))
		{
			if (isDodge || !isGrounded)
			{
				vector = GetDir();
			}
			Vector3 dir = GetDir();
			float num = Vector3.Angle(dir, vector);
			if (num < 80f)
			{
				Vector3 vector2 = Vector3.Cross(dir, vector);
				num *= (float)((vector2.y > 0f) ? 1 : (-1));
				vector = Quaternion.Euler(0f, num * 10f * Time.deltaTime, 0f) * dir;
			}
			Move(vector.normalized, 0.9f);
			aiRunImmediateRotTime += Time.time;
			if (aiRunImmediateRotTime > 1f)
			{
				aiRunImmediateRotTime = 0f;
				MoveRot(vector, _immediate: true);
			}
			else
			{
				MoveRot(vector);
			}
		}
	}
	public void AiTargetWalk()
	{
		Vector3 vector = aiTargetTrans.position - GetPos();
		vector.y = 0f;
		if (!(vector.magnitude < 0.2f))
		{
			Vector3 dir = GetDir();
			float num = Vector3.Angle(dir, vector);
			if (num < 80f)
			{
				Vector3 vector2 = Vector3.Cross(dir, vector);
				num *= (float)((vector2.y > 0f) ? 1 : (-1));
				vector = Quaternion.Euler(0f, num * 10f * Time.deltaTime, 0f) * dir;
			}
			Move(vector.normalized, 0.5f);
			MoveRot(vector);
		}
	}
	public void AiStandby()
	{
	}
	public void AiActionCheck()
	{
		Vector3 position = base.transform.position;
		position.y += 0.5f;
		Vector3 dir = GetDir();
		if (!Physics.BoxCast(position, AI_HALF_EXTENTS, dir, out RaycastHit hitInfo, Quaternion.identity, nowMoveSpeed * 1.5f))
		{
			return;
		}
		if (hitInfo.transform.tag == "Player")
		{
			if (CheckOniType(OniType.Escaper) && UnityEngine.Random.Range(0, 10) == 0)
			{
				Jump();
			}
		}
		else if (hitInfo.transform.tag == "EditorOnly" && nowActionType == ActionType.Default)
		{
			Jump();
		}
	}
	public void AiActionObjectCheck()
	{
		if (!isOnActionObject || !isGrounded || isAttack || isDodge || isActionAfter || aiMoveType == AiMoveType.Escaper_Standby || nowActionType != 0)
		{
			return;
		}
		if (actionObjectBase == null)
		{
			actionObjectBase = onTriggerActionCollider.GetComponent<Hidden_ActionObjectBase>();
		}
		if (IsCanAiActionObject())
		{
			PlayAction();
			isOnActionObject = false;
			actionObjectBase = null;
			if (onTriggerActionType == ActionType.RotFloor)
			{
				isAiJump = true;
			}
		}
	}
	private bool IsCanAiActionObject()
	{
		switch (onTriggerActionType)
		{
		case ActionType.RotY:
			if (actionObjectBase != null && !actionObjectBase.IsAction && actionObjectBase.ContainsFieldNo(nowFieldNo))
			{
				return actionObjectBase.ContainsFieldNo(aiTargetFieldNo);
			}
			return false;
		case ActionType.Door:
		case ActionType.Scroll:
			if (actionObjectBase != null && !actionObjectBase.IsAction && !actionObjectBase.IsSwitchOn && actionObjectBase.ContainsFieldNo(nowFieldNo))
			{
				return actionObjectBase.ContainsFieldNo(aiTargetFieldNo);
			}
			return false;
		case ActionType.RotFloor:
			if (actionObjectBase != null && !actionObjectBase.IsAction && !actionObjectBase.IsSwitchOn && (cpuType == CpuType.Normal || cpuType == CpuType.Strong))
			{
				if (aiMoveType != AiMoveType.Escaper_Walk)
				{
					return aiMoveType == AiMoveType.Oni_Search;
				}
				return true;
			}
			return false;
		default:
			return false;
		}
	}
	private void AiTouchCheck()
	{
		if (CheckCanTouchControl())
		{
			if (targetTouchChara != null)
			{
				aiTouchWaitTimer += Time.deltaTime;
				if (aiTouchWaitTimer > aiStrengthTouchWaitTime)
				{
					aiTouchWaitTimer = 0f;
					OniTouch(targetTouchChara);
					SettingTouchAnim();
				}
			}
			else
			{
				aiTouchWaitTimer = 0f;
			}
		}
		else
		{
			aiTouchWaitTimer = 0f;
		}
	}
	public void AiLookTarget()
	{
		Vector3 moveDir = aiTargetPos - GetPos();
		moveDir.y = 0f;
		MoveRot(moveDir, _immediate: true);
	}
	public AiMoveType GetAiMoveType()
	{
		return aiMoveType;
	}
	public void SetAiMoveType(AiMoveType _aiMoveType)
	{
		aiMoveType = _aiMoveType;
		aiMoveTypeChangeTime = Time.time;
	}
	public void SetAiTargetFieldNoAndTrans(int _targetFieldNo)
	{
		aiTargetFieldNo = _targetFieldNo;
		aiTargetTrans = SingletonCustom<Hidden_FieldManager>.Instance.SearchNearestConnectPoint(nowFieldNo, aiTargetFieldNo, GetPos());
	}
	public void SetAiTargetPos(Vector3 _pos)
	{
		aiTargetPos = _pos;
	}
	public void SetAiFreeze(float _freezeTime)
	{
		isFreeze = true;
		isAiFreeze = true;
		rigid.isKinematic = true;
		aiFreezeTime = _freezeTime;
	}
}
