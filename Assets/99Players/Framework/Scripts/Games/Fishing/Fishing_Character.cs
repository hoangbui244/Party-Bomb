using System;
using System.Collections.Generic;
using UnityEngine;
public class Fishing_Character : MonoBehaviour
{
	[Serializable]
	private struct FishingData
	{
		public FishingDefinition.FishType biteFishType;
		public FishingDefinition.FishSizeType biteFishSizeType;
		public FishingDefinition.BiteDifficulty biteDifficulty;
		public Fishing_FishShadow fishingShadowData;
		public float fishBiteWaitTime;
		public float currentDecreaseFishBiteWaitTime;
		public float shakeUkiIntervalTime;
		public int shakeUkiCount;
		public float fishBiteTime;
		public GameObject fishingFishObj;
		public bool isFishStartedToBite;
		public bool isBite;
		public List<Fishing_FishShadow> fishShadowList;
		public List<Fishing_FishShadow> biteFishShadowList;
		public void SetUpData()
		{
			biteFishType = FishingDefinition.FishType.Ayu;
			biteFishSizeType = FishingDefinition.FishSizeType.Small;
			biteDifficulty = FishingDefinition.BiteDifficulty.Easy;
			fishingShadowData = null;
			fishBiteWaitTime = 0f;
			currentDecreaseFishBiteWaitTime = 0f;
			shakeUkiIntervalTime = 0f;
			shakeUkiCount = 0;
			fishBiteTime = 0f;
			isFishStartedToBite = false;
			isBite = false;
			fishingFishObj = null;
			fishShadowList = new List<Fishing_FishShadow>();
			biteFishShadowList = new List<Fishing_FishShadow>();
		}
		public void ResetData()
		{
			biteFishType = FishingDefinition.FishType.Ayu;
			biteFishSizeType = FishingDefinition.FishSizeType.Small;
			biteDifficulty = FishingDefinition.BiteDifficulty.Easy;
			fishingShadowData = null;
			fishBiteWaitTime = 0f;
			currentDecreaseFishBiteWaitTime = 0f;
			shakeUkiIntervalTime = 0f;
			shakeUkiCount = 0;
			fishBiteTime = 0f;
			isFishStartedToBite = false;
			isBite = false;
			if (fishingFishObj != null)
			{
				UnityEngine.Object.Destroy(fishingFishObj);
				fishingFishObj = null;
			}
			fishShadowList.Clear();
			biteFishShadowList.Clear();
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
	public enum AnimationType
	{
		Wait,
		RodSetUp,
		RodCast,
		FishFight,
		Fishing,
		RodCancel,
		None
	}
	[SerializeField]
	[Header("キャラスタイル")]
	private CharacterStyle charaStyle;
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	[SerializeField]
	[Header("キャラクタ\u30fcカ\u30fcソル")]
	private CharacterCursorAnimation characterCursor;
	private FishingDefinition.User characterUser;
	private int userDataNo = -1;
	private Fishing_AI ai;
	private bool isPlayer;
	[SerializeField]
	[Header("モ\u30fcションデ\u30fcタ")]
	private CharaLeanTweenMotion motion;
	private AnimationType currentAnimType = AnimationType.None;
	private bool isDuringMotionAnimation;
	[SerializeField]
	[Header("RigidBody")]
	private Rigidbody rigid;
	private const float GRAVITY = -1.6f;
	private RaycastHit checkAirHit;
	private float checkAirDistance = 0.12f;
	private const float RUN_SPEED = 2.4f;
	private const float WALK_SPEED = 1f;
	private const float MAX_SPEED = 0.2f;
	private Vector3 prevPos;
	private Vector3 nowPos;
	private Vector3 walkPrevPos = Vector3.zero;
	private Vector3 walkNowPos = Vector3.zero;
	private Vector3[] calcVec = new Vector3[2];
	private Vector3 rot;
	private float runAnimationTime;
	private int playSeRunCnt;
	private bool isChangeAnimationNeutral;
	private float runAnimationSpeed = 50f;
	private bool isWalkMode;
	private Vector3 rodCastOriginPos = Vector3.zero;
	private const float ROD_CAST_WALK_LIMIT = 0.25f;
	[SerializeField]
	[Header("釣り竿のメッシュレンダラ\u30fc")]
	private MeshRenderer[] meshFishingRod;
	[SerializeField]
	[Header("釣り竿のマテリアル")]
	private Material[] fishingRodMat;
	[SerializeField]
	[Header("ウキ")]
	private Fishing_RodUki uki;
	[SerializeField]
	[Header("ウキを投げる前のアンカ\u30fc")]
	private Transform rodUkiNotThrowAnchor;
	[SerializeField]
	[Header("ウキの着地点")]
	private Fishing_LandingPoint landingPoint;
	[SerializeField]
	[Header("魚が食いつくアンカ\u30fc")]
	private Transform fishBiteAnchor;
	[SerializeField]
	[Header("魚を持つアンカ\u30fc")]
	private Transform fishCatchAnchor;
	[SerializeField]
	[Header("魚を見せるアンカ\u30fc")]
	private Transform fishShowAnchor;
	private FishingData fishingData;
	private const float BASE_FISH_BITE_WAIT_INTERVAL_TIME = 0.5f;
	private const float SMALL_FISH_BITE_TIME = 3f;
	private const float MEDIUM_FISH_BITE_TIME = 2.5f;
	private const float LARGE_FISH_BITE_TIME = 2f;
	private const int UKI_SHAKE_COUNT_MIN = 2;
	private const int UKI_SHAKE_COUNT_MAX = 5;
	private float[] CONST_UKI_SHAKE_INTERVAL_TIME = new float[6]
	{
		0.2f,
		0.3f,
		0.4f,
		0.5f,
		0.6f,
		0.7f
	};
	private Vector3 fishingDir = Vector3.zero;
	private bool throwingRod;
	private readonly float MOVE_CTRL_FRAME_TIME = 0.015f;
	private float currentMoveCtrlFrameTime;
	private Vector3 moveDir = Vector3.zero;
	private Vector3 velocityP;
	public void Init(bool isPlayer, FishingDefinition.User user, int userNo)
	{
		prevPos = nowPos;
		nowPos = base.transform.position;
		userDataNo = userNo;
		characterUser = user;
		fishingData.SetUpData();
		landingPoint.Init(characterUser);
		uki.Init(characterUser);
		this.isPlayer = isPlayer;
		if (!this.isPlayer)
		{
			ai = GetComponent<Fishing_AI>();
			ai.Init(this, userDataNo);
		}
		SetMainCharaStyle(user);
		if (characterUser <= FishingDefinition.User.Player4)
		{
			characterCursor.SetColor((int)characterUser);
		}
		else
		{
			characterCursor.ShowCircle(_show: false);
			characterCursor.ShowArrow(_show: false);
		}
		for (int i = 0; i < meshFishingRod.Length; i++)
		{
			meshFishingRod[i].material = fishingRodMat[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)characterUser]];
		}
		uki.SetUkiMaterial(fishingRodMat[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)characterUser]]);
		motion.Init();
		SetAnimation(AnimationType.Wait);
	}
	public void UpdateMethod()
	{
		prevPos = nowPos;
		nowPos = base.transform.position;
		switch (currentAnimType)
		{
		case AnimationType.Wait:
			landingPoint.UpdateMethod();
			break;
		case AnimationType.RodSetUp:
		case AnimationType.FishFight:
			uki.UpdateMethod();
			break;
		}
		if (!FishingDefinition.MGM.IsDuringGame())
		{
			if (currentAnimType == AnimationType.RodSetUp)
			{
				RodCancel();
			}
		}
		else
		{
			if (isDuringMotionAnimation || throwingRod)
			{
				return;
			}
			if (IsRodSetUp())
			{
				UpdateRodSetUpData();
			}
			if (isPlayer)
			{
				if (FishingDefinition.CM.IsPushButton_A(characterUser))
				{
					RodAction();
				}
			}
			else
			{
				ai.UpdateMethod();
			}
		}
	}
	public void FixedUpdateMethod()
	{
		if (!FishingDefinition.MGM.IsDuringGame())
		{
			rigid.velocity = Vector3.zero;
		}
		else if (isPlayer)
		{
			if (FishingDefinition.CM.IsMove(characterUser))
			{
				if (currentMoveCtrlFrameTime > MOVE_CTRL_FRAME_TIME * 5f)
				{
					Move(FishingDefinition.CM.GetMoveDir(characterUser), FishingDefinition.CM.GetMoveLength(characterUser));
					return;
				}
				Move(FishingDefinition.CM.GetMoveDir(characterUser), FishingDefinition.CM.GetMoveLength(characterUser), _moveRot: true, _moveAllow: false);
				currentMoveCtrlFrameTime += Time.deltaTime;
			}
			else
			{
				currentMoveCtrlFrameTime = 0f;
				rigid.velocity = Vector3.zero;
			}
		}
		else
		{
			ai.FixedUpdateMethod();
		}
	}
	public void SetMainCharaStyle(FishingDefinition.User user)
	{
		charaStyle.SetMainCharacterStyle((int)user);
	}
	public void SetCharaFacial(StyleTextureManager.MainCharacterFaceType _facialType, bool _isResetFacial = false)
	{
		charaStyle.SetMainCharacterFaceDiff((int)characterUser, _facialType);
		if (_isResetFacial)
		{
			Invoke("ResetCharaFacial", 0.5f);
		}
	}
	public void ResetCharaFacial()
	{
		charaStyle.SetMainCharacterFaceDiff((int)characterUser, StyleTextureManager.MainCharacterFaceType.NORMAL);
	}
	public void SetAnimation(AnimationType _animType, bool _isMotion = false)
	{
		if (_animType != currentAnimType)
		{
			currentAnimType = _animType;
			switch (currentAnimType)
			{
			case AnimationType.Wait:
				WaitAnimation(_isMotion);
				break;
			case AnimationType.RodCast:
				RodCastAnimation(_isMotion);
				break;
			case AnimationType.RodSetUp:
				RodSetUpAnimation(_isMotion);
				break;
			case AnimationType.FishFight:
				FishFightAnimation(_isMotion);
				break;
			case AnimationType.Fishing:
				FishingAnimation(_isMotion);
				break;
			case AnimationType.RodCancel:
				RodCancelAnimation(_isMotion);
				break;
			}
		}
	}
	private void RodCastAnimation(bool _isMotion)
	{
		if (currentAnimType == AnimationType.RodCast)
		{
			isDuringMotionAnimation = true;
			CharaLeanTweenMotionData.MotionData rodCastMotionData = CharaLeanTweenMotionData.GetRodCastMotionData();
			if (!_isMotion)
			{
				rodCastMotionData.motionTime = 0f;
			}
			motion.StartMotion(rodCastMotionData, delegate
			{
				isDuringMotionAnimation = false;
			});
		}
	}
	private void RodSetUpAnimation(bool _isMotion)
	{
		if (currentAnimType == AnimationType.RodSetUp)
		{
			CharaLeanTweenMotionData.MotionData rodSetUpMotionData = CharaLeanTweenMotionData.GetRodSetUpMotionData();
			if (!_isMotion)
			{
				rodSetUpMotionData.motionTime = 0f;
			}
			motion.StartMotion(rodSetUpMotionData);
		}
	}
	private void FishFightAnimation(bool _isMotion)
	{
		if (currentAnimType == AnimationType.FishFight)
		{
			isDuringMotionAnimation = true;
			CharaLeanTweenMotionData.MotionData fishingFightMotionData = CharaLeanTweenMotionData.GetFishingFightMotionData();
			if (!_isMotion)
			{
				fishingFightMotionData.motionTime = 0f;
			}
			motion.StartMotion(fishingFightMotionData, delegate
			{
				isDuringMotionAnimation = false;
			});
		}
	}
	private void FishingAnimation(bool _isMotion)
	{
		if (currentAnimType == AnimationType.Fishing)
		{
			isDuringMotionAnimation = true;
			CharaLeanTweenMotionData.MotionData fishingMotionData = CharaLeanTweenMotionData.GetFishingMotionData();
			if (!_isMotion)
			{
				fishingMotionData.motionTime = 0f;
			}
			motion.StartMotion(fishingMotionData, delegate
			{
				isDuringMotionAnimation = false;
			});
		}
	}
	private void RodCancelAnimation(bool _isMotion)
	{
		if (currentAnimType == AnimationType.RodCancel)
		{
			isDuringMotionAnimation = true;
			CharaLeanTweenMotionData.MotionData rodCancelMotionData = CharaLeanTweenMotionData.GetRodCancelMotionData();
			if (!_isMotion)
			{
				rodCancelMotionData.motionTime = 0f;
			}
			motion.StartMotion(rodCancelMotionData, delegate
			{
				SetAnimation(AnimationType.Wait);
				isDuringMotionAnimation = false;
			});
		}
	}
	private void WaitAnimation(bool _isMotion)
	{
		CharaLeanTweenMotionData.MotionData fishingWaitMotionData = CharaLeanTweenMotionData.GetFishingWaitMotionData();
		isDuringMotionAnimation = true;
		if (!_isMotion)
		{
			fishingWaitMotionData.motionTime = 0f;
		}
		motion.StartMotion(fishingWaitMotionData, delegate
		{
			isDuringMotionAnimation = false;
		});
	}
	public void Move(Vector3 _moveDir, float _speedMag = 1f, bool _moveRot = true, bool _moveAllow = true)
	{
		if (currentAnimType != 0 && currentAnimType != AnimationType.RodSetUp)
		{
			return;
		}
		moveDir = _moveDir;
		if (isWalkMode)
		{
			if (Vector3.Distance(walkNowPos, rodCastOriginPos) <= 0.25f)
			{
				moveDir.y = 0f;
				moveDir *= 1f * _speedMag;
				if (_moveAllow)
				{
					rigid.AddForce(moveDir + Vector3.down, ForceMode.Impulse);
					if (rigid.velocity.magnitude > 0.2f * _speedMag)
					{
						rigid.velocity = rigid.velocity.normalized * 0.2f * _speedMag;
					}
					MoveAnimation();
				}
				walkPrevPos = walkNowPos;
				walkNowPos = base.transform.position;
			}
			else
			{
				walkNowPos = walkPrevPos;
				base.transform.position = walkPrevPos;
			}
			return;
		}
		moveDir *= 2.4f * _speedMag;
		if (_moveAllow)
		{
			rigid.AddForce(moveDir + Vector3.down, ForceMode.Impulse);
			velocityP = rigid.velocity;
			if (rigid.velocity.magnitude > 0.2f * _speedMag)
			{
				rigid.velocity = rigid.velocity.normalized * 0.2f * _speedMag;
			}
			MoveAnimation();
		}
		if (_moveRot)
		{
			MoveRot(_moveDir);
		}
	}
	private void MoveRot(Vector3 _moveDir, bool _immediate = false)
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
			rigid.MoveRotation(Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(rot), 30f * Time.deltaTime));
		}
	}
	private void MoveAnimation()
	{
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * 30f);
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
	private void PlaySeRun()
	{
		if (isPlayer)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.2f);
		}
	}
	public void AddGravity()
	{
		rigid.AddForce(Vector3.down * -1.6f, ForceMode.Impulse);
	}
	private bool CheckAir()
	{
		UnityEngine.Debug.DrawRay(new Vector3(base.transform.position.x, base.transform.position.y + 0.1f, base.transform.position.z), Vector3.down * checkAirDistance, Color.red);
		if (Physics.Raycast(new Vector3(base.transform.position.x, base.transform.position.y + 0.1f, base.transform.position.z), Vector3.down, out checkAirHit, checkAirDistance, LayerMask.GetMask(FishingDefinition.LayerField)))
		{
			if (checkAirHit.collider.gameObject.tag == FishingDefinition.TagField || checkAirHit.collider.gameObject.tag == FishingDefinition.TagNoHit)
			{
				return false;
			}
			return true;
		}
		return true;
	}
	private void UpdateRodSetUpData()
	{
		if (fishingData.isBite)
		{
			if (fishingData.fishBiteTime < 0f)
			{
				EscapeFish();
			}
			else
			{
				fishingData.fishBiteTime -= Time.deltaTime;
			}
		}
		else if (!fishingData.isFishStartedToBite)
		{
			fishingData.biteDifficulty = GetBiteDifficulty();
			if (uki.GetFishingAreaInShadows() != 0)
			{
				if (fishingData.fishBiteWaitTime < 0f)
				{
					CheckFishBiteStart();
					fishingData.fishBiteWaitTime = GetFishBiteWaitIntervalTime();
				}
				else
				{
					fishingData.fishBiteWaitTime -= Time.deltaTime;
				}
			}
			else
			{
				fishingData.fishBiteWaitTime = GetFishBiteWaitIntervalTime();
			}
		}
		else if (uki.GetFishingAreaInShadows() == 0)
		{
			fishingData.isFishStartedToBite = false;
			SetShakeUkiIntervalTime();
		}
		else if (fishingData.shakeUkiIntervalTime < 0f)
		{
			if (CheckBiteFish())
			{
				SetBiteFishType();
				if (isPlayer)
				{
					VibrationController();
				}
				return;
			}
			SetShakeUkiIntervalTime();
			RodShake();
			if (isPlayer)
			{
				VibrationController();
			}
		}
		else
		{
			fishingData.shakeUkiIntervalTime -= Time.deltaTime;
		}
	}
	public void RodAction()
	{
		switch (currentAnimType)
		{
		case AnimationType.RodCast:
		case AnimationType.FishFight:
		case AnimationType.Fishing:
		case AnimationType.RodCancel:
			break;
		case AnimationType.Wait:
			rigid.velocity = Vector3.zero;
			if (landingPoint.IsActiveLandingPoint())
			{
				RodCast();
				isWalkMode = true;
			}
			break;
		case AnimationType.RodSetUp:
			if (fishingData.isBite)
			{
				FishFight();
			}
			else
			{
				RodCancel();
			}
			isWalkMode = false;
			break;
		}
	}
	private void RodCast()
	{
		if (currentAnimType == AnimationType.Wait)
		{
			rigid.velocity = Vector3.zero;
			rigid.mass = 5000f;
			rigid.drag = 5000f;
			rodCastOriginPos = (walkNowPos = nowPos);
			SetAnimation(AnimationType.RodCast, _isMotion: true);
			throwingRod = true;
			LeanTween.delayedCall(0.25f, (Action)delegate
			{
				uki.GetUkiOrbitCalculation().SetEndPoint(uki.GetTargetCursorAnchor().position);
				uki.GetUkiOrbitCalculation().StartOrbitMove();
			});
			LeanTween.delayedCall(0.75f, (Action)delegate
			{
				uki.LandingUki();
				uki.SetTargetCursorAnimationActive(_active: false);
				uki.transform.parent = base.transform.parent;
			});
			LeanTween.delayedCall(1f, (Action)delegate
			{
				SetAnimation(AnimationType.RodSetUp, _isMotion: true);
				throwingRod = false;
			});
		}
	}
	public void RodShake()
	{
		if (currentAnimType == AnimationType.RodSetUp)
		{
			uki.ShakeUki();
		}
	}
	public void RodBite()
	{
		if (currentAnimType == AnimationType.RodSetUp)
		{
			fishingData.isBite = true;
			uki.SinkUki();
		}
	}
	public void EscapeFish()
	{
		if (currentAnimType == AnimationType.RodSetUp)
		{
			for (int i = 0; i < fishingData.biteFishShadowList.Count; i++)
			{
				fishingData.biteFishShadowList[i].SetStopBiteMode();
			}
			uki.FloatUki();
			fishingData.ResetData();
		}
	}
	private void FishFight()
	{
		if (currentAnimType == AnimationType.RodSetUp)
		{
			uki.transform.parent = base.transform;
			uki.transform.localPosition = Vector3.zero;
			uki.transform.localEulerAngles = Vector3.zero;
			SetAnimation(AnimationType.FishFight, _isMotion: true);
			fishingData.biteFishShadowList.Remove(fishingData.fishingShadowData);
			for (int i = 0; i < fishingData.biteFishShadowList.Count; i++)
			{
				fishingData.biteFishShadowList[i].SetStopBiteMode();
			}
			fishingData.biteFishShadowList.Clear();
			uki.AroundMoveUki();
			switch (fishingData.biteFishSizeType)
			{
			}
			LeanTween.delayedCall(1f, (Action)delegate
			{
				if (isPlayer)
				{
					StopVibrationController();
				}
				SetAnimation(AnimationType.Fishing, _isMotion: true);
				uki.PlayFishingEffect();
				uki.ResetUki();
				uki.GetUkiOrbitCalculation().SetEndPoint(rodUkiNotThrowAnchor.position);
				uki.GetUkiOrbitCalculation().StartOrbitMove();
				fishingData.fishingFishObj.transform.parent = base.transform;
				fishingData.fishingFishObj.SetActive(value: true);
				fishingData.fishingFishObj.GetComponent<OrbitCalculation>().SetEndPoint(fishCatchAnchor.position);
				fishingData.fishingFishObj.GetComponent<OrbitCalculation>().StartOrbitMove();
				fishingData.fishingShadowData.SetFishShadowInactive();
				switch (fishingData.biteFishSizeType)
				{
				default:
					LeanTween.delayedCall(0.5f, (Action)delegate
					{
						if (isPlayer)
						{
							fishingDir = base.transform.forward;
							calcVec[0] = (FishingDefinition.FM.GetCameraTransform().position - GetPos(_isLocal: false)).normalized;
							rot.x = 0f;
							rot.y = CalcManager.Rot(calcVec[0], CalcManager.AXIS.Y);
							rot.z = 0f;
							base.transform.eulerAngles = rot;
							bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAnglesX(315f);
							bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(280f);
							fishingData.fishingFishObj.transform.parent = fishShowAnchor;
							fishingData.fishingFishObj.transform.localPosition = Vector3.zero;
							fishingData.fishingFishObj.transform.localEulerAngles = Vector3.zero;
						}
						switch (fishingData.biteFishSizeType)
						{
						case FishingDefinition.FishSizeType.Small:
							SetCharaFacial(StyleTextureManager.MainCharacterFaceType.NORMAL);
							break;
						case FishingDefinition.FishSizeType.Medium:
							SetCharaFacial(StyleTextureManager.MainCharacterFaceType.SMILE);
							break;
						case FishingDefinition.FishSizeType.Large:
							SetCharaFacial(StyleTextureManager.MainCharacterFaceType.HAPPY);
							break;
						case FishingDefinition.FishSizeType.Garbage:
							SetCharaFacial(StyleTextureManager.MainCharacterFaceType.SAD);
							break;
						}
						FishingDefinition.MCM.SetFishResult(userDataNo, fishingData.biteFishType, fishingData.biteFishSizeType);
						FishingDefinition.CUIM.ShowFishGetWindow(userDataNo, fishingData.biteFishType);
						LeanTween.delayedCall(0.5f, (Action)delegate
						{
							base.transform.SetEulerAnglesY(CalcManager.Rot(fishingDir, CalcManager.AXIS.Y));
							rigid.mass = 1f;
							rigid.drag = 1f;
							SetAnimation(AnimationType.Wait);
							uki.SetTargetCursorAnimationActive(_active: true);
							fishingData.ResetData();
							SetCharaFacial(StyleTextureManager.MainCharacterFaceType.NORMAL);
							LeanTween.delayedCall(0.5f, (Action)delegate
							{
								FishingDefinition.CUIM.HideFishGetWindow(userDataNo);
							});
						});
					});
					break;
				}
			});
		}
	}
	private void RodCancel()
	{
		if (currentAnimType == AnimationType.RodSetUp)
		{
			uki.transform.parent = base.transform;
			uki.transform.localPosition = Vector3.zero;
			uki.transform.localEulerAngles = Vector3.zero;
			SetAnimation(AnimationType.RodCancel, _isMotion: true);
			throwingRod = true;
			uki.ResetUki();
			uki.SetTargetCursorAnimationActive(_active: true);
			fishingData.ResetData();
			uki.GetUkiOrbitCalculation().SetEndPoint(rodUkiNotThrowAnchor.position);
			uki.GetUkiOrbitCalculation().StartOrbitMove();
			LeanTween.delayedCall(0.5f, (Action)delegate
			{
				SetAnimation(AnimationType.Wait, _isMotion: true);
				throwingRod = false;
				rigid.mass = 1f;
				rigid.drag = 1f;
			});
		}
	}
	public void SetFishObject(GameObject _fishObject)
	{
		fishingData.fishingFishObj = UnityEngine.Object.Instantiate(_fishObject, Vector3.zero, Quaternion.identity, fishBiteAnchor);
		fishingData.fishingFishObj.transform.SetLocalPosition(0f, 0f, 0f);
		fishingData.fishingFishObj.SetActive(value: false);
	}
	private void SetShakeUkiIntervalTime()
	{
		fishingData.shakeUkiIntervalTime = CONST_UKI_SHAKE_INTERVAL_TIME[UnityEngine.Random.Range(0, CONST_UKI_SHAKE_INTERVAL_TIME.Length)];
	}
	private void SetBiteFishType()
	{
		fishingData.isBite = true;
		fishingData.fishingShadowData = fishingData.biteFishShadowList[UnityEngine.Random.Range(0, fishingData.biteFishShadowList.Count)];
		fishingData.biteFishType = fishingData.fishingShadowData.GetFishType();
		fishingData.biteFishSizeType = fishingData.fishingShadowData.GetFishSizeType();
		RodBite();
		SetFishObject(FishingDefinition.FDM.GetFishObject(fishingData.biteFishType));
		switch (fishingData.biteFishSizeType)
		{
		case FishingDefinition.FishSizeType.Small:
			fishingData.fishBiteTime = 3f;
			break;
		case FishingDefinition.FishSizeType.Medium:
			fishingData.fishBiteTime = 2.5f;
			break;
		case FishingDefinition.FishSizeType.Large:
			fishingData.fishBiteTime = 2f;
			break;
		case FishingDefinition.FishSizeType.Garbage:
			fishingData.fishBiteTime = 3f;
			break;
		}
	}
	public Vector3 GetPos(bool _isLocal = true)
	{
		if (_isLocal)
		{
			return base.transform.localPosition;
		}
		return base.transform.position;
	}
	public Vector3 GetAngle(bool _isLocal = true)
	{
		if (_isLocal)
		{
			return base.transform.localEulerAngles;
		}
		return base.transform.eulerAngles;
	}
	public Vector3 GetScale()
	{
		return base.transform.localScale;
	}
	public Rigidbody GetRigid()
	{
		return rigid;
	}
	public bool IsStartBite()
	{
		return fishingData.isFishStartedToBite;
	}
	public bool IsBite()
	{
		return fishingData.isBite;
	}
	public bool IsRodSetUp()
	{
		return currentAnimType == AnimationType.RodSetUp;
	}
	public FishingDefinition.BiteDifficulty GetBiteDifficulty()
	{
		return uki.GetBiteDifficulty();
	}
	public AnimationType GetAnimationType()
	{
		return currentAnimType;
	}
	private float GetFishBiteWaitIntervalTime()
	{
		fishingData.currentDecreaseFishBiteWaitTime = 0.05f * (float)fishingData.biteFishShadowList.Count;
		if (fishingData.currentDecreaseFishBiteWaitTime > 0.4f)
		{
			fishingData.currentDecreaseFishBiteWaitTime = 0.4f;
		}
		return 0.5f - fishingData.currentDecreaseFishBiteWaitTime;
	}
	public int GetUserDataNo()
	{
		return userDataNo;
	}
	private void CheckFishBiteStart()
	{
		fishingData.fishShadowList.Clear();
		for (int i = 0; i < uki.GetAllFishShadows().Count; i++)
		{
			fishingData.fishShadowList.Add(uki.GetAllFishShadows()[i]);
		}
		fishingData.biteFishShadowList.Clear();
		for (int j = 0; j < fishingData.fishShadowList.Count; j++)
		{
			if (fishingData.fishShadowList[j].GetFishState() == Fishing_FishShadow.FishState.BITE_MODE || fishingData.fishShadowList[j].GetFishState() == Fishing_FishShadow.FishState.ESCAPE)
			{
				continue;
			}
			switch (fishingData.biteDifficulty)
			{
			case FishingDefinition.BiteDifficulty.Easy:
				if (UnityEngine.Random.Range(0, 100) >= 20)
				{
					fishingData.fishShadowList[j].SetStartBiteMode(uki);
					fishingData.biteFishShadowList.Add(fishingData.fishShadowList[j]);
				}
				break;
			case FishingDefinition.BiteDifficulty.Normal:
				if (UnityEngine.Random.Range(0, 100) >= 50)
				{
					fishingData.fishShadowList[j].SetStartBiteMode(uki);
					fishingData.biteFishShadowList.Add(fishingData.fishShadowList[j]);
				}
				break;
			case FishingDefinition.BiteDifficulty.Hard:
				if (UnityEngine.Random.Range(0, 100) >= 80)
				{
					fishingData.fishShadowList[j].SetStartBiteMode(uki);
					fishingData.biteFishShadowList.Add(fishingData.fishShadowList[j]);
				}
				break;
			}
		}
		if (fishingData.biteFishShadowList.Count > 0)
		{
			SetShakeUkiIntervalTime();
			fishingData.isFishStartedToBite = true;
		}
	}
	private bool CheckBiteFish()
	{
		if (fishingData.shakeUkiCount >= 2)
		{
			if (fishingData.shakeUkiCount++ == 5)
			{
				return true;
			}
			if (UnityEngine.Random.Range(0, 3) == 0)
			{
				return true;
			}
			fishingData.shakeUkiCount++;
			return false;
		}
		fishingData.shakeUkiCount++;
		return false;
	}
	private void VibrationController()
	{
		if (fishingData.isBite)
		{
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)characterUser, HidVibration.VibrationType.Strong, fishingData.fishBiteTime);
		}
		else if (UnityEngine.Random.Range(0, 2) == 0)
		{
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)characterUser, HidVibration.VibrationType.Weak, 0.1f);
		}
		else
		{
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)characterUser, HidVibration.VibrationType.Normal, 0.05f);
		}
	}
	private void StopVibrationController()
	{
		SingletonCustom<HidVibration>.Instance.StopVibration((int)characterUser);
	}
}
