using System;
using UnityEngine;
public class SpearBattle_CharacterScript : MonoBehaviour
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
		HAND_L,
		HAND_R,
		UP_LEG_L,
		UP_LEG_R,
		LEG_L,
		LEG_R,
		SPEAR,
		SHIELD
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
	private const string HORSE_SPEED_ANIM = "Speed_f";
	[SerializeField]
	private Transform allAnchor;
	[SerializeField]
	private Transform cameraAnchor;
	[SerializeField]
	private Transform rideCharaAnchor;
	[SerializeField]
	private CharacterStyle charaStyle;
	[SerializeField]
	private Transform charaParent;
	[SerializeField]
	private Transform fallHorseAnchor;
	[SerializeField]
	private GameObject[] horseModelObjs;
	[SerializeField]
	private MeshRenderer spearRenderer;
	[SerializeField]
	private MeshRenderer shieldRenderer;
	[SerializeField]
	private Animator moveAnimator;
	[SerializeField]
	private Animator horseAnimator;
	[SerializeField]
	private ParticleSystem charaBreakEffect;
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	[SerializeField]
	private SpearBattle_CharacterAnimation charaAnimation;
	private bool isStartInit;
	private Vector3 startPos;
	private Vector3 startEuler;
	private Vector3 startCharaPos;
	private Vector3 startCharaEuler;
	private int playerNo;
	public void Init()
	{
		moveAnimator.Play("Wait");
		moveAnimator.speed = 0f;
		if (!isStartInit)
		{
			isStartInit = true;
			startPos = allAnchor.position;
			startEuler = allAnchor.eulerAngles;
			startCharaPos = rideCharaAnchor.localPosition;
			startCharaEuler = rideCharaAnchor.localEulerAngles;
		}
		else
		{
			allAnchor.position = startPos;
			allAnchor.eulerAngles = startEuler;
		}
		RideHorse();
	}
	public void PlayAnimation(bool _isLeft, bool _isSpeedStart)
	{
		if (_isLeft)
		{
			moveAnimator.Play("spearbattle_left_anim", 0, 0f);
		}
		else
		{
			moveAnimator.Play("spearbattle_right_anim", 0, 0f);
		}
		if (_isSpeedStart)
		{
			LeanTween.value(base.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
			{
				moveAnimator.speed = _value;
				horseAnimator.SetFloat("Speed_f", _value);
			});
		}
	}
	public void StopAnimation(bool _isHorseLerpStop)
	{
		moveAnimator.speed = 0f;
		if (_isHorseLerpStop)
		{
			horseAnimator.SetFloat("Speed_f", 0f);
			return;
		}
		float @float = horseAnimator.GetFloat("Speed_f");
		LeanTween.value(horseAnimator.gameObject, @float, 0f, 0.5f).setOnUpdate(delegate(float _value)
		{
			horseAnimator.SetFloat("Speed_f", _value);
		});
	}
	public void ResetAnimation()
	{
		moveAnimator.Play("Wait");
	}
	public void PlayBreakEffect()
	{
		charaBreakEffect.Play();
	}
	public void FallHorse()
	{
		rideCharaAnchor.parent = allAnchor.parent;
		Vector3 aPos = rideCharaAnchor.position;
		Quaternion aRot = rideCharaAnchor.rotation;
		Vector3 bPos = fallHorseAnchor.position;
		Quaternion bRot = fallHorseAnchor.rotation;
		charaAnimation.StopAnimation();
		LeanTween.value(charaAnimation.gameObject, 0f, 1f, 0.5f).setEaseOutQuad().setOnUpdate(delegate(float _value)
		{
			rideCharaAnchor.position = Vector3.Lerp(aPos, bPos, _value);
			rideCharaAnchor.rotation = Quaternion.Lerp(aRot, bRot, _value);
		});
	}
	public void RideHorse()
	{
		rideCharaAnchor.parent = charaParent;
		rideCharaAnchor.localPosition = startCharaPos;
		rideCharaAnchor.localEulerAngles = startCharaEuler;
	}
	public void StayDirection()
	{
		LeanTween.value(base.gameObject, 1f, 0.4f, 0.5f).setOnUpdate(delegate(float _value)
		{
			moveAnimator.speed = _value;
			horseAnimator.SetFloat("Speed_f", _value);
		});
	}
	public void SelectEndDirection()
	{
		LeanTween.value(base.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
		{
			moveAnimator.speed = _value;
			horseAnimator.SetFloat("Speed_f", _value);
		});
	}
	public void AttackAnimDirection()
	{
		charaAnimation.PoseDirection();
		LeanTween.delayedCall(base.gameObject, 0.2f, (Action)delegate
		{
			charaAnimation.AttackDirection();
			LeanTween.delayedCall(base.gameObject, 0.3f, (Action)delegate
			{
				charaAnimation.StayDirection();
			});
		});
	}
	public void PoseAnimDirection()
	{
		charaAnimation.PoseDirection();
		LeanTween.delayedCall(base.gameObject, 0.4f, (Action)delegate
		{
			charaAnimation.StayDirection();
		});
	}
	public Transform GetAllAnchor()
	{
		return allAnchor;
	}
	public Transform GetCameraAnchor()
	{
		return cameraAnchor;
	}
	public Transform GetRideCharaAnchor()
	{
		return rideCharaAnchor;
	}
	public BodyParts GetBodyParts()
	{
		return bodyParts;
	}
	public void SetSpearMaterial(Material _mat)
	{
		spearRenderer.sharedMaterial = _mat;
	}
	public void SetShieldMaterial(Material _mat)
	{
		shieldRenderer.sharedMaterial = _mat;
	}
	public void SetSpearAndShieldMaterial(Material _mat)
	{
		spearRenderer.sharedMaterial = _mat;
		shieldRenderer.sharedMaterial = _mat;
	}
	public void SetCharaStyle(SpearBattle_GameManager.CharaData _charaData)
	{
		playerNo = _charaData.playerNo;
		charaStyle.DisableAddModel();
		charaStyle.SetGameStyle(GS_Define.GameType.GET_BALL, _charaData.playerNo);
		charaStyle.SetMainCharacterFaceDiff(playerNo, StyleTextureManager.MainCharacterFaceType.NORMAL);
		int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_charaData.playerNo];
		for (int i = 0; i < horseModelObjs.Length; i++)
		{
			horseModelObjs[i].SetActive(num == i);
		}
	}
	public void SetCharaFace(StyleTextureManager.MainCharacterFaceType _faceType)
	{
		charaStyle.SetMainCharacterFaceDiff(playerNo, _faceType);
	}
}
