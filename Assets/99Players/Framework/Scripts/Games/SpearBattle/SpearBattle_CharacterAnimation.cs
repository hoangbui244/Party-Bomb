using System;
using UnityEngine;
public class SpearBattle_CharacterAnimation : MonoBehaviour
{
	public enum AnimType
	{
		Stay,
		Pose,
		Attack
	}
	[Serializable]
	public class AnimData
	{
		public AnimType animType;
		public TransData[] transDatas;
	}
	[Serializable]
	public class TransData
	{
		public SpearBattle_CharacterScript.BodyPartsList bodyParts;
		public Vector3 position;
		public Vector3 eulerAngles;
		public Quaternion Rotation => Quaternion.Euler(eulerAngles);
	}
	[SerializeField]
	private SpearBattle_CharacterScript chara;
	[SerializeField]
	private AnimData[] animDatas;
	public void StayDirection()
	{
		PlayAnimation(0.2f, AnimType.Pose, AnimType.Stay);
	}
	public void PoseDirection()
	{
		PlayAnimation(0.2f, AnimType.Stay, AnimType.Pose);
	}
	public void AttackDirection()
	{
		PlayAnimation(0.1f, AnimType.Pose, AnimType.Attack, delegate
		{
			LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
			{
				PlayAnimation(0.1f, AnimType.Attack, AnimType.Pose);
			});
		});
	}
	public void PlayAnimation(float _time, AnimType _beforeAnimType, AnimType _afterAnimType, Action _callBack = null)
	{
		LeanTween.value(base.gameObject, 0f, 1f, _time).setOnUpdate(delegate(float _value)
		{
			LerpChangeBodyTrans(_value, _beforeAnimType, _afterAnimType);
		}).setOnComplete((Action)delegate
		{
			if (_callBack != null)
			{
				_callBack();
			}
		});
	}
	public void StopAnimation()
	{
		LeanTween.cancel(base.gameObject);
	}
	public void LerpChangeBodyTrans(float _lerp, AnimType _beforeAnimType, AnimType _afterAnimType)
	{
		int num = 15;
		SpearBattle_CharacterScript.BodyParts bodyParts = chara.GetBodyParts();
		for (int i = 0; i < num; i++)
		{
			SpearBattle_CharacterScript.BodyPartsList bodyPartsType = (SpearBattle_CharacterScript.BodyPartsList)i;
			TransData transData = GetTransData(_beforeAnimType, bodyPartsType);
			TransData transData2 = GetTransData(_afterAnimType, bodyPartsType);
			bodyParts.rendererList[i].transform.localPosition = Vector3.Lerp(transData.position, transData2.position, _lerp);
			bodyParts.rendererList[i].transform.localRotation = Quaternion.Lerp(transData.Rotation, transData2.Rotation, _lerp);
		}
	}
	public void ChangeBodyTrans(AnimType _animType)
	{
		int num = 15;
		SpearBattle_CharacterScript.BodyParts bodyParts = chara.GetBodyParts();
		for (int i = 0; i < num; i++)
		{
			SpearBattle_CharacterScript.BodyPartsList bodyPartsType = (SpearBattle_CharacterScript.BodyPartsList)i;
			TransData transData = GetTransData(_animType, bodyPartsType);
			if (transData != null)
			{
				bodyParts.rendererList[i].transform.localPosition = transData.position;
				bodyParts.rendererList[i].transform.localEulerAngles = transData.eulerAngles;
			}
			else
			{
				bodyParts.rendererList[i].transform.localPosition = Vector3.zero;
				bodyParts.rendererList[i].transform.localEulerAngles = Vector3.zero;
			}
		}
	}
	public void SaveNowBodyParts(AnimType _animType)
	{
		int num = 15;
		SpearBattle_CharacterScript.BodyParts bodyParts = chara.GetBodyParts();
		AnimData animData = GetAnimData(_animType);
		animData.transDatas = new TransData[num];
		for (int i = 0; i < num; i++)
		{
			animData.transDatas[i] = new TransData();
			animData.transDatas[i].bodyParts = (SpearBattle_CharacterScript.BodyPartsList)i;
			animData.transDatas[i].position = bodyParts.rendererList[i].transform.localPosition;
			animData.transDatas[i].eulerAngles = bodyParts.rendererList[i].transform.localEulerAngles;
		}
	}
	public TransData GetTransData(AnimType _animType, SpearBattle_CharacterScript.BodyPartsList _bodyPartsType)
	{
		int animIdx = GetAnimIdx(_animType);
		for (int i = 0; i < animDatas[animIdx].transDatas.Length; i++)
		{
			if (animDatas[animIdx].transDatas[i].bodyParts == _bodyPartsType)
			{
				return animDatas[animIdx].transDatas[i];
			}
		}
		UnityEngine.Debug.Log("SpearBattle_CharacterAnimation: GetTransData関数 null が返っている");
		return null;
	}
	public int GetAnimIdx(AnimType _animType)
	{
		for (int i = 0; i < animDatas.Length; i++)
		{
			if (animDatas[i].animType == _animType)
			{
				return i;
			}
		}
		UnityEngine.Debug.Log("SpearBattle_CharacterAnimation: GetAnimIdx関数 -1 が返っている");
		return -1;
	}
	public AnimData GetAnimData(AnimType _animType)
	{
		for (int i = 0; i < animDatas.Length; i++)
		{
			if (animDatas[i].animType == _animType)
			{
				return animDatas[i];
			}
		}
		UnityEngine.Debug.Log("SpearBattle_CharacterAnimation: GetAnimData関数 null が返っている");
		return null;
	}
}
