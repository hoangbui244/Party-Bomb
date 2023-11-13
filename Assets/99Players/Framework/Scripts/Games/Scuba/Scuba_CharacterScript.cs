using System;
using System.Collections;
using UnityEngine;
public class Scuba_CharacterScript : MonoBehaviour
{
	public enum ActionType
	{
		Default,
		RotY
	}
	private enum CharaAnimState
	{
		Idle,
		Swim,
		Shot
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
	private const float GRAVITY = 1.6f;
	private const float MASS = 50f;
	public const float SLOW_WALK_SPEED = 0.25f;
	public const float WALK_SPEED = 0.5f;
	public const float RUN_SPEED = 0.9f;
	public const float RUN_SPEED_MAG_WEAK = 0.9f;
	public const float RUN_SPEED_MAG_NORMAL = 1f;
	public const float RUN_SPEED_MAG_STRONG = 1.1f;
	private const float ROT_SPEED = 15f;
	private const float JUMP_POWER = 6f;
	private const float RISE_POWER = 1f;
	private const float DIVE_POWER = -1f;
	private const float STAY_POWER = 1.5f;
	private const float MAX_VELOCITY_Y = 1f;
	private const float MIN_VELOCITY_Y = -1f;
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
	private const float DAMAGE_DURATION = 1f;
	private const float DAMAGE_INTERVAL = 2f;
	private const float DAMAGE_ANIMATION_LERP_TIME = 0.2f;
	private const float PICTURE_CAMERA_ROTATE_X = 3.5f;
	private const string ANIM_IDLE_TRIGGER_NAME = "ToIdle";
	private const string ANIM_SWIM_TRIGGER_NAME = "ToSwim";
	private const string ANIM_SHOT_TRIGGER_NAME = "ToShot";
	private const string TAG_PLAYER = "Player";
	private const string TAG_FIELD = "Respawn";
	private const string TAG_OBJECT = "EditorOnly";
	private const string TAG_WALL = "Finish";
	private const string TAG_DAMAGE = "Failure";
	private const string TAG_TRANSPARENT_WALL = "CharaWall";
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
	private bool isTakePicture;
	private Rigidbody rigid;
	private Vector3[] calcVec = new Vector3[2];
	private Vector3 prevPos;
	private Vector3 nowPos;
	private Vector3 nowMoveDir;
	private float staminaValue;
	private float moveSpeed = 2.5f;
	private float moveSpeedMax = 1.25f;
	private float nowMoveSpeed;
	private float cpuRunSpeedMag = 1f;
	private ActionType nowActionType;
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
	[Header("キャラの視点位置アンカ\u30fc")]
	private Transform eyeAnchor;
	[SerializeField]
	[Header("カメラタ\u30fcゲット")]
	private Transform cameraTarget;
	private Transform cameraRotAnchor;
	private Transform ctrlDirAnchor;
	private bool isStartDashCharaRot;
	private bool isEndDashCharaRot = true;
	private bool isGrounded = true;
	private bool isGravity;
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
	private Scuba_CharacterScript targetTouchChara;
	[SerializeField]
	[Header("カメラのモデルレンダラ\u30fc")]
	private MeshRenderer cameraModelRenderer;
	[SerializeField]
	[Header("カメラフラッシュ用レンダラ\u30fc")]
	private MeshRenderer cameraViewRenderer;
	[SerializeField]
	[Header("カメラフラッシュのAlpha曲線")]
	private AnimationCurve cameraViewAlphaCurve;
	[SerializeField]
	[Header("カメラフラッシュ演出時間")]
	private float cameraViewTime = 0.5f;
	private MaterialPropertyBlock cameraViewPb;
	private float cameraViewMaxAlpha;
	[SerializeField]
	[Header("TPS視点でRenderTextureを使うカメラ")]
	private Camera tpsPictureCamera;
	[SerializeField]
	[Header("見えない壁用アニメ\u30fcション")]
	private Animation transparentWallAnim;
	private CharaAnimState charaAnimState;
	[SerializeField]
	[Header("キャラアニメ\u30fcタ\u30fc")]
	private Animator charaAnimator;
	[SerializeField]
	[Header("キャラスタイル")]
	private CharacterStyle charaStyle;
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	private float runAnimationSpeed = 90f;
	private float runAnimationTime;
	private int playSeRunCnt;
	private float runSeInterval;
	private bool isChangeAnimationNeutral;
	private float actionChangeInterval;
	private float movePosChangeInterval;
	private bool isTouchAnim;
	private float touchAnimStartTime;
	private const float AI_ITEM_ORDER_LERP_MIN_WEAK = 0.2f;
	private const float AI_ITEM_ORDER_LERP_MAX_WEAK = 1f;
	private const float AI_ITEM_ORDER_LERP_MIN_NORMAL = 0f;
	private const float AI_ITEM_ORDER_LERP_MAX_NORMAL = 0.8f;
	private const float AI_ITEM_ORDER_LERP_MIN_STRONG = 0f;
	private const float AI_ITEM_ORDER_LERP_MAX_STRONG = 0.3f;
	private const float AI_STOP_DISTANCE = 0.5f;
	private CpuType cpuType;
	private float aiItemOrderLerpMin;
	private float aiItemOrderLerpMax;
	private float aiMoveTimer;
	private float aiStopTimer;
	private float aiInViewportTimer;
	private Scuba_ItemObject aiTargetItem;
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
	public bool IsCameraChara
	{
		get
		{
			if (!isPlayer)
			{
				return isCpuCamera;
			}
			return true;
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
	public bool IsTakePicture
	{
		get
		{
			return isTakePicture;
		}
		set
		{
			isTakePicture = value;
		}
	}
	public bool IsChangePos => ChangePosVecZeroY.sqrMagnitude > 5E-05f;
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
	public Scuba_CharacterScript TargetTouchChara => targetTouchChara;
	public Camera TpsPictureCamera => tpsPictureCamera;
	public void Init(int _charaNo)
	{
		rigid = GetComponent<Rigidbody>();
		charaCollider = obj.GetComponent<CapsuleCollider>();
		charaRadius = charaCollider.radius;
		charaBodySize = charaRadius * obj.localScale.x * 2f;
		charaHeight = charaCollider.height * obj.localScale.y;
		base.gameObject.name = base.gameObject.name + "_" + _charaNo.ToString();
		charaNo = _charaNo;
		playerNo = -1;
		runAnimationTime = 0f;
		playSeRunCnt = 0;
		nowPos = (prevPos = base.transform.position);
		staminaValue = 100f;
		transparentWallAnim.transform.parent = base.transform.parent;
		SettingCharacterBaseLayer();
		nowFieldChangeTime = Time.time + 360000f;
	}
	public void UpdateMethod()
	{
		prevPos = nowPos;
		nowPos = base.transform.position;
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
			}
			else
			{
				staminaValue -= Time.deltaTime * 20f;
				if (staminaValue < 0f)
				{
					staminaValue = 0f;
				}
			}
		}
		runSeInterval -= Time.deltaTime;
		if (cameraRotAnchor != null && !isTakePicture)
		{
			MoveRot(nowMoveDir);
		}
		if (!rigid.isKinematic)
		{
			if (charaAnimState != 0 && !IsMove)
			{
				charaAnimator.SetTrigger("ToIdle");
				charaAnimState = CharaAnimState.Idle;
			}
			else if (charaAnimState != CharaAnimState.Swim && IsMove)
			{
				charaAnimator.SetTrigger("ToSwim");
				charaAnimState = CharaAnimState.Swim;
			}
		}
		nowMoveSpeed = 0f;
		if (transparentWallAnim.gameObject.activeSelf && !transparentWallAnim.isPlaying)
		{
			transparentWallAnim.gameObject.SetActive(value: false);
		}
		tpsPictureCamera.transform.eulerAngles = new Vector3(3.5f, eyeAnchor.eulerAngles.y, 0f);
	}
	public void UpdateObjRotAnchor()
	{
		objRotAnchor.SetLocalEulerAnglesY(cameraRotAnchor.localEulerAngles.y);
	}
	public void ActionMarkUpdate()
	{
	}
	public void TouchMarkUpdate()
	{
	}
	public void MarkObjEnd()
	{
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
				SwimAnimation();
			}
			else
			{
				SwimAnimation();
			}
		}
	}
	public void MoveRot(Vector3 _moveDir, bool _immediate = false, bool _isForce = false)
	{
		if (_isForce || (nowActionType == ActionType.Default && IsMove))
		{
			calcVec[0] = _moveDir;
			Vector3 zero = Vector3.zero;
			zero.y = CalcManager.Rot(calcVec[0], CalcManager.AXIS.Y);
			if (_immediate)
			{
				GetRotTrans().rotation = Quaternion.Euler(zero);
			}
			else
			{
				GetRotTrans().rotation = Quaternion.Lerp(GetRotTrans().rotation, Quaternion.Euler(zero), 15f * Time.deltaTime);
			}
		}
	}
	private bool CheckActionMove(Vector3 _moveDir, float _moveSpeed)
	{
		ActionType nowActionType2 = nowActionType;
		return nowActionType != ActionType.Default;
	}
	private void ResetAnimation()
	{
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalPosition(0f, 0f, 0f);
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
	private void SwimAnimation()
	{
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalPosition(0f, 0.1735753f, 0f);
		bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(330.6531f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(56.9805f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 328.7279f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 37.65128f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * 30f + 30f, -5f, -15f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * -30f + 30f, 5f, 15f);
		if (IsChangePos)
		{
			runAnimationTime += ChangePosVecZeroY.magnitude * runAnimationSpeed * Time.deltaTime;
			if (runAnimationTime >= 1f)
			{
				runAnimationTime = 0f;
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
	}
	public void Rise()
	{
		Vector3 velocity = rigid.velocity;
		velocity.y += 1f * Time.deltaTime;
		if (velocity.y > 1f)
		{
			velocity.y = 1f;
		}
		rigid.velocity = velocity;
	}
	public void Dive()
	{
		Vector3 velocity = rigid.velocity;
		velocity.y += -1f * Time.deltaTime;
		if (velocity.y < -1f)
		{
			velocity.y = -1f;
		}
		rigid.velocity = velocity;
	}
	public void Stay()
	{
		Vector3 velocity = rigid.velocity;
		velocity.y -= velocity.y * 1.5f * Time.deltaTime;
		rigid.velocity = velocity;
	}
	private void PlaySeRun()
	{
		if (isPlayer)
		{
			bool isGrounded2 = isGrounded;
		}
	}
	private void OnCollisionStay(Collision _col)
	{
		if (!SingletonCustom<Scuba_GameManager>.Instance.IsGameEnd && _col.gameObject.tag == "CharaWall")
		{
			if (!transparentWallAnim.gameObject.activeSelf)
			{
				transparentWallAnim.gameObject.SetActive(value: true);
				transparentWallAnim.Play();
			}
			transparentWallAnim.transform.position = base.transform.position - _col.contacts[0].normal * 0.3f;
			transparentWallAnim.transform.AddPositionY(0.25f);
			transparentWallAnim.transform.forward = _col.contacts[0].normal;
		}
	}
	private IEnumerator _DamageDirection()
	{
		yield return new WaitForSeconds(1f);
		isDamage = false;
		charaStyle.SetMainCharacterFaceDiff(playerNo, StyleTextureManager.MainCharacterFaceType.NORMAL);
		ResetAnimation();
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
	public void SetCharaGameStyle(int _playerNo)
	{
		charaStyle.SetGameStyle(GS_Define.GameType.BLOCK_WIPER, _playerNo);
	}
	public void SetCameraModelMaterial(Material _mat)
	{
		cameraModelRenderer.sharedMaterial = _mat;
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
				base.transform.parent = SingletonCustom<Scuba_CharacterManager>.Instance.GetCharacterAnchor();
				rigid.constraints = RigidbodyConstraints.FreezeRotation;
				ResetAnimation();
				break;
			}
			nowActionType = _type;
			if (nowActionType != 0)
			{
			}
		}
	}
	public void ShotAnimTrigger()
	{
		if (charaAnimState != CharaAnimState.Shot)
		{
			charaAnimator.SetTrigger("ToShot");
			charaAnimState = CharaAnimState.Shot;
		}
	}
	public void CameraFlashView()
	{
		cameraViewRenderer.gameObject.SetActive(value: true);
		if (cameraViewPb == null)
		{
			cameraViewPb = new MaterialPropertyBlock();
			cameraViewMaxAlpha = cameraViewRenderer.sharedMaterial.GetFloat("_Alpha");
		}
		cameraViewPb.SetFloat("_Alpha", 0f);
		cameraViewRenderer.SetPropertyBlock(cameraViewPb);
		LeanTween.value(cameraViewRenderer.gameObject, 0f, 1f, cameraViewTime).setOnUpdate(delegate(float _value)
		{
			cameraViewPb.SetFloat("_Alpha", cameraViewAlphaCurve.Evaluate(_value / cameraViewTime) * cameraViewMaxAlpha);
			cameraViewRenderer.SetPropertyBlock(cameraViewPb);
		}).setOnComplete((Action)delegate
		{
			cameraViewRenderer.gameObject.SetActive(value: false);
		});
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
		int num = 28;
		switch (charaNo)
		{
		case 1:
			num = 29;
			break;
		case 2:
			num = 30;
			break;
		case 3:
			num = 31;
			break;
		}
		SettingChildLayer(bodyParts.Parts(BodyPartsList.HIP), num);
		int num2 = tpsPictureCamera.cullingMask;
		int num3 = 1 << num;
		if ((num2 & num3) == num3)
		{
			num2 -= num3;
		}
		tpsPictureCamera.cullingMask = num2;
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
			aiItemOrderLerpMin = 0.2f;
			aiItemOrderLerpMax = 1f;
			break;
		case 1:
			cpuRunSpeedMag = 1f;
			cpuType = CpuType.Normal;
			aiItemOrderLerpMin = 0f;
			aiItemOrderLerpMax = 0.8f;
			break;
		case 2:
			cpuRunSpeedMag = 1.1f;
			cpuType = CpuType.Strong;
			aiItemOrderLerpMin = 0f;
			aiItemOrderLerpMax = 0.3f;
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
	public bool CheckCanGameEndStop()
	{
		return nowActionType == ActionType.Default;
	}
	public Rigidbody GetRigid()
	{
		return rigid;
	}
	public Transform GetRotTrans()
	{
		if (!isPlayer)
		{
			return base.transform;
		}
		return obj;
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
	public Transform GetCameraTarget()
	{
		return cameraTarget;
	}
	public Transform GetObjTrans()
	{
		return obj;
	}
	public Transform GetEyeAnchor()
	{
		return eyeAnchor;
	}
	public void SetCameraRotAnchor(Transform _anchor)
	{
		cameraRotAnchor = _anchor;
	}
	public void SetVelocity(Vector3 _velocity)
	{
		rigid.velocity = _velocity;
	}
	public void SetVelocityY(float _velocityY)
	{
		Vector3 velocity = rigid.velocity;
		velocity.y = _velocityY;
		rigid.velocity = velocity;
	}
	public void SetActionButtonFlg(bool _flg)
	{
		isActioButton = _flg;
	}
	public void SetCameraViewMaterial(Material _mat)
	{
		cameraViewRenderer.sharedMaterial = _mat;
	}
	public void AiMove()
	{
		if (rigid.isKinematic)
		{
			return;
		}
		if (aiTargetItem == null || aiTargetItem.GetIsFound(charaNo))
		{
			AiSearchSortItem();
		}
		aiMoveTimer += Time.deltaTime;
		if (aiMoveTimer > 10f)
		{
			AiSearchSortItem();
			aiMoveTimer = 0f;
		}
		Vector3 vector = aiTargetItem.transform.position - eyeAnchor.position;
		if (!IsChangePos)
		{
			Rise();
			aiStopTimer += Time.deltaTime;
			if (aiStopTimer > 3f)
			{
				AiSearchReverseItem();
				aiMoveTimer = 0f;
				aiStopTimer = 0f;
			}
		}
		else if (vector.y > 0f)
		{
			if (vector.y > rigid.velocity.y)
			{
				Rise();
			}
			else
			{
				Stay();
			}
			aiStopTimer = 0f;
		}
		else
		{
			if (vector.y < rigid.velocity.y)
			{
				Dive();
			}
			else
			{
				Stay();
			}
			aiStopTimer = 0f;
		}
		vector.y = 0f;
		if (SingletonCustom<Scuba_ItemManager>.Instance.CheckInRangeViewport(aiTargetItem.GetCharaViewportPoint(charaNo)))
		{
			aiInViewportTimer += Time.deltaTime;
			if (aiInViewportTimer > 1f)
			{
				SingletonCustom<Scuba_CharacterManager>.Instance.TakePicture(charaNo);
				aiMoveTimer = 0f;
				aiInViewportTimer = 0f;
				AiSearchSortItem();
				return;
			}
		}
		else
		{
			aiInViewportTimer = 0f;
		}
		if (vector.sqrMagnitude < 0.25f)
		{
			vector *= -1f;
		}
		else
		{
			MoveRot(vector);
		}
		Move(vector.normalized, 0.5f);
	}
	private void AiSearchSortItem()
	{
		aiTargetItem = SingletonCustom<Scuba_ItemManager>.Instance.SearchAiSortItem(charaNo, UnityEngine.Random.Range(aiItemOrderLerpMin, aiItemOrderLerpMax));
	}
	private void AiSearchReverseItem()
	{
		aiTargetItem = SingletonCustom<Scuba_ItemManager>.Instance.SearchAiSortItem(charaNo, UnityEngine.Random.Range(aiItemOrderLerpMin, aiItemOrderLerpMax));
	}
}
