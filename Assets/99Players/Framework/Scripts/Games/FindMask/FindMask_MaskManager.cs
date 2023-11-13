using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FindMask_MaskManager : SingletonCustom<FindMask_MaskManager>
{
	private enum MASK_TYPE
	{
		OKAME,
		HYOTTKO,
		RED_FOX,
		BLUE_FOX,
		RED_OGRE,
		BLUE_OGRE,
		RED_TENGU,
		WHITE_TENGU,
		WHITE_HANNYA,
		RED_HANNYA,
		RED_RANGER,
		BLUE_RENGER,
		YELLOW_RENGER,
		PINK_RENGER,
		GREEN_RENGER,
		KAMENRIDER_STYLE,
		ULTRAMAN_STYLE,
		GUNDAM_STYLE,
		KESIGOMUKUN,
		ICHIGOCHAN,
		MAX
	}
	[SerializeField]
	[Header("お面")]
	private FindMask_MaskData masks;
	[SerializeField]
	[Header("お面のアンカ\u30fc")]
	private Transform[] maskAnchor;
	private List<FindMask_MaskData> maskObjList = new List<FindMask_MaskData>();
	private List<FindMask_MaskData> pairMaskObjList = new List<FindMask_MaskData>();
	private List<int> maskTypeList = new List<int>();
	private float MASK_INTERVAL = 0.2f;
	private Vector3 MASK_LOCAL_SCALE = new Vector3(1f, 1f, 1f);
	private FindMask_MaskData firstSelectMask;
	private FindMask_MaskData secondSelectMask;
	private float emphasisScale = 1.5f;
	private bool isFirstSelect;
	private bool isSecondSelect;
	private bool isFindPair;
	private bool isFindAllPair;
	private readonly int MASK_POINT = 10;
	private int RARE_MASK_NUM = 2;
	private List<int> rareMaskTypeList = new List<int>();
	private readonly int SHUFFLE_NUM = 5;
	private readonly float EFFECT_INTERVAL = 0.5f;
	private readonly float EFFECT_PLAY_TIME = 0.75f;
	private bool playEffect;
	[SerializeField]
	[Header("高得点お面ペア成功のエフェクト")]
	private ParticleSystem findRarePairEffect;
	[SerializeField]
	[Header("お面ペア成功のエフェクト")]
	private ParticleSystem findPairEffect;
	[SerializeField]
	[Header("お面ペア失敗のエフェクト")]
	private ParticleSystem noFindPairEffect;
	public bool IsFirstSelect => isFirstSelect;
	public bool IsSecondSelect => isSecondSelect;
	public bool IsFindPair => isFindPair;
	public bool IsFindAllPair => isFindAllPair;
	public bool PlayEffect => playEffect;
	public void Init()
	{
		GenerateMask();
	}
	public void SecondGroupInit()
	{
		ResetMask();
		GenerateMask();
	}
	private void GenerateMask()
	{
		for (int i = 0; i < FindMask_Define.TOTAL_MASK_NUM; i++)
		{
			maskTypeList.Add(i % FindMask_Define.TOTAL_MASK_NUM / 2);
		}
		for (int j = 0; j < FindMask_Define.TOTAL_MASK_NUM; j++)
		{
			ShuffleMaskTypeList();
		}
		for (int k = 0; k < maskAnchor.Length; k++)
		{
			for (int l = 0; l < FindMask_Define.TOTAL_MASK_NUM / maskAnchor.Length; l++)
			{
				int num = k * (FindMask_Define.TOTAL_MASK_NUM / maskAnchor.Length);
				maskObjList.Add(UnityEngine.Object.Instantiate(masks, maskAnchor[k]));
				maskObjList[num + l].transform.localPosition = Vector3.zero;
				maskObjList[num + l].transform.SetLocalPositionX(MASK_INTERVAL * (float)l);
				maskObjList[num + l].transform.localScale = MASK_LOCAL_SCALE;
			}
		}
		for (int m = 0; m < FindMask_Define.TOTAL_MASK_NUM; m++)
		{
			maskObjList[m].SetMaskObjNo(m);
			maskObjList[m].SetMaskTypeNo(maskTypeList[m]);
			maskObjList[m].SetMaskSilhouette();
		}
		SetHightScoreMask();
		for (int n = 0; n < maskObjList.Count; n++)
		{
			for (int num2 = 0; num2 < rareMaskTypeList.Count; num2++)
			{
				if (maskObjList[n].TypeNo == rareMaskTypeList[num2])
				{
					maskObjList[n].IsRareMask();
				}
			}
		}
	}
	public IEnumerator _StartDirecting()
	{
		for (int j = 0; j < FindMask_Define.TOTAL_MASK_NUM; j++)
		{
			MaskTurnOver(j, 180f);
		}
		for (int k = 0; k < FindMask_Define.TOTAL_MASK_NUM; k++)
		{
			maskObjList[k].MaskChangeSilhouette();
		}
		yield return new WaitForSeconds(4f);
		for (int l = 0; l < FindMask_Define.TOTAL_MASK_NUM; l++)
		{
			MaskTurnOver(l, 0f);
		}
		for (int m = 0; m < FindMask_Define.TOTAL_MASK_NUM; m++)
		{
			maskObjList[m].SetMaskSilhouette();
		}
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < SHUFFLE_NUM; i++)
		{
			for (int n = 0; n < maskAnchor.Length; n++)
			{
				SideShuffleMask(n);
			}
			yield return new WaitForSeconds(0.26f);
			for (int num = 0; num < maskAnchor.Length; num++)
			{
				if (num % 2 != 0)
				{
					VerticalShuffleMask(num);
				}
			}
			yield return new WaitForSeconds(0.26f);
		}
		for (int num2 = 0; num2 < FindMask_Define.TOTAL_MASK_NUM; num2++)
		{
			maskObjList[num2].SetMaskObjNo(num2);
		}
		SingletonCustom<CommonStartSimple>.Instance.Show(SingletonCustom<FindMask_GameManager>.Instance.GameStart);
		yield return new WaitForSeconds(2f);
		SingletonCustom<FindMask_UIManager>.Instance.ShowTurnCutIn(SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo, 0.5f);
	}
	private void SideShuffleMask(int _maskAnchorNum)
	{
		int num = maskObjList.Count / maskAnchor.Length;
		int num2 = UnityEngine.Random.Range(num * _maskAnchorNum, num + _maskAnchorNum * num);
		if (num2 + 1 < num + _maskAnchorNum * num)
		{
			FindMask_MaskData value = maskObjList[num2];
			maskObjList[num2] = maskObjList[num2 + 1];
			maskObjList[num2 + 1] = value;
			SwapMaskPosition(maskObjList[num2 + 1], maskObjList[num2]);
		}
		else
		{
			FindMask_MaskData value2 = maskObjList[num2];
			maskObjList[num2] = maskObjList[num2 - 1];
			maskObjList[num2 - 1] = value2;
			SwapMaskPosition(maskObjList[num2 - 1], maskObjList[num2]);
		}
	}
	private void VerticalShuffleMask(int _maskAnchorNum)
	{
		int num = maskObjList.Count / maskAnchor.Length;
		int num2 = UnityEngine.Random.Range(num * _maskAnchorNum, num + _maskAnchorNum * num);
		FindMask_MaskData value = maskObjList[num2];
		maskObjList[num2] = maskObjList[num2 - num];
		maskObjList[num2 - num] = value;
		SwapMaskPosition(maskObjList[num2 - num], maskObjList[num2]);
	}
	private void SwapMaskPosition(FindMask_MaskData _maskA, FindMask_MaskData _maskB)
	{
		Vector3 position = _maskA.transform.position;
		Vector3 position2 = _maskB.transform.position;
		LeanTween.move(_maskA.gameObject, position2, 0.25f);
		LeanTween.move(_maskB.gameObject, position, 0.25f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_findmask_shuffle");
	}
	private void SetHightScoreMask()
	{
		rareMaskTypeList.Add(18);
		rareMaskTypeList.Add(19);
	}
	private void ShuffleMaskTypeList()
	{
		for (int i = 0; i < maskTypeList.Count; i++)
		{
			int num = UnityEngine.Random.Range(0, FindMask_Define.TOTAL_MASK_NUM / 2);
			if (i != num)
			{
				int value = maskTypeList[i];
				maskTypeList[i] = maskTypeList[num];
				maskTypeList[num] = value;
			}
		}
	}
	public void MaskTurnOver(int _maskObjNo, float _angle)
	{
		LeanTween.rotateAround(maskObjList[_maskObjNo].gameObject, Vector3.right, 180f, 0.5f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_findmask_turn_1");
	}
	public void SelectMask(int _maskObjNo)
	{
		if (!isSecondSelect && !maskObjList[_maskObjNo].IsSelect && !maskObjList[_maskObjNo].IsFindPair && !playEffect)
		{
			maskObjList[_maskObjNo].MaskChangeSilhouette();
			MaskTurnOver(_maskObjNo, 180f);
			maskObjList[_maskObjNo].SelectMask();
			SingletonCustom<FindMask_CharacterManager>.Instance.RememberSelectMask(maskObjList[_maskObjNo]);
			if (!isFirstSelect)
			{
				isFirstSelect = true;
				firstSelectMask = maskObjList[_maskObjNo];
			}
			if (isFirstSelect && firstSelectMask.ObjNo != _maskObjNo)
			{
				isSecondSelect = true;
				SingletonCustom<FindMask_UIManager>.Instance.HideRemainCountDown();
				secondSelectMask = maskObjList[_maskObjNo];
				CheckMaskPair();
				SingletonCustom<FindMask_UIManager>.Instance.CharacterScoreUIUpdate();
			}
		}
	}
	public void CheckMaskPair()
	{
		if (!isSecondSelect)
		{
			return;
		}
		if (firstSelectMask.TypeNo == secondSelectMask.TypeNo)
		{
			SelectMaskSetPairFlag();
			PairMaskRemoveMaskMemory();
			maskObjList.Remove(maskObjList[firstSelectMask.ObjNo]);
			for (int i = 0; i < maskObjList.Count; i++)
			{
				maskObjList[i].SetMaskObjNo(i);
			}
			maskObjList.Remove(maskObjList[secondSelectMask.ObjNo]);
			for (int j = 0; j < maskObjList.Count; j++)
			{
				maskObjList[j].SetMaskObjNo(j);
			}
			pairMaskObjList.Add(firstSelectMask);
			pairMaskObjList.Add(secondSelectMask);
			if (firstSelectMask.IsRare)
			{
				SingletonCustom<FindMask_ScoreManager>.Instance.AddCharaScore(MASK_POINT * 3);
			}
			else
			{
				SingletonCustom<FindMask_ScoreManager>.Instance.AddCharaScore(MASK_POINT);
			}
			CheckAllFindPair();
			SingletonCustom<FindMask_ControllerManager>.Instance.AddFindPairCount();
			SingletonCustom<FindMask_GameManager>.Instance.ResetTimer();
			playEffect = true;
			int nowPlayerNo = SingletonCustom<FindMask_GameManager>.Instance.ArrayPlayerElement[SingletonCustom<FindMask_GameManager>.Instance.CurrentTurnNum];
			LeanTween.delayedCall(EFFECT_INTERVAL, (Action)delegate
			{
				if (firstSelectMask.IsRare)
				{
					MaskPairEffect(FindMask_EffectManager.EFFECT_TYPE.FIND_RARE_PAIR_0);
					LeanTween.delayedCall(EFFECT_INTERVAL, (Action)delegate
					{
						SingletonCustom<FindMask_UIManager>.Instance.ViewGetPoint(nowPlayerNo, MASK_POINT * 3, secondSelectMask.transform.position);
					});
					SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_perfect");
				}
				else
				{
					MaskPairEffect(FindMask_EffectManager.EFFECT_TYPE.FIND_PAIR_0);
					LeanTween.delayedCall(EFFECT_INTERVAL, (Action)delegate
					{
						SingletonCustom<FindMask_UIManager>.Instance.ViewGetPoint(nowPlayerNo, MASK_POINT, secondSelectMask.transform.position);
					});
					SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
				}
				ResetSelect();
			});
		}
		else
		{
			StartCoroutine(NoFindPair());
			ResetSelect();
			SingletonCustom<FindMask_ControllerManager>.Instance.ResetFindPairCount();
			SingletonCustom<FindMask_GameManager>.Instance.State = FindMask_GameManager.STATE.CHARA_FINDMASK;
		}
	}
	private void PairMaskRemoveMaskMemory()
	{
		SingletonCustom<FindMask_CharacterManager>.Instance.RemoveMemoryMask(maskObjList[firstSelectMask.ObjNo]);
		SingletonCustom<FindMask_CharacterManager>.Instance.RemoveMemoryMask(maskObjList[secondSelectMask.ObjNo]);
	}
	private void MaskPairEffect(FindMask_EffectManager.EFFECT_TYPE _effectType)
	{
		SingletonCustom<FindMask_EffectManager>.Instance.PlayMaskEffect(firstSelectMask, _effectType);
		SingletonCustom<FindMask_EffectManager>.Instance.PlayMaskEffect(secondSelectMask, _effectType + 1);
		LeanTween.delayedCall(EFFECT_PLAY_TIME, (Action)delegate
		{
			playEffect = false;
		});
	}
	private void SelectMaskSetPairFlag()
	{
		maskObjList[firstSelectMask.ObjNo].SelectMaskFindPair();
		maskObjList[secondSelectMask.ObjNo].SelectMaskFindPair();
	}
	private void SelectMaskResetFlag()
	{
		maskObjList[firstSelectMask.ObjNo].ResetMaskFlag();
		maskObjList[secondSelectMask.ObjNo].ResetMaskFlag();
	}
	private void SelectMaskSetSilhouette()
	{
		maskObjList[firstSelectMask.ObjNo].SetMaskSilhouette();
		maskObjList[secondSelectMask.ObjNo].SetMaskSilhouette();
	}
	public void CheckAllFindPair()
	{
		if (FindMask_Define.TOTAL_MASK_NUM == pairMaskObjList.Count)
		{
			isFindAllPair = true;
			SingletonCustom<FindMask_GameManager>.Instance.State = FindMask_GameManager.STATE.FIND_ALL;
		}
	}
	private IEnumerator NoFindPair()
	{
		SelectMaskResetFlag();
		SingletonCustom<FindMask_ControllerManager>.Instance.RecordLastSelectMaskNo(firstSelectMask.ObjNo, secondSelectMask.ObjNo);
		yield return new WaitForSeconds(1f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_miss");
		yield return new WaitForSeconds(1f);
		MaskTurnOver(firstSelectMask.ObjNo, 0f);
		MaskTurnOver(secondSelectMask.ObjNo, 0f);
		yield return new WaitForSeconds(0.25f);
		SelectMaskSetSilhouette();
		SingletonCustom<FindMask_GameManager>.Instance.NextTurnPlayer();
	}
	public void ResetSelect()
	{
		isFirstSelect = false;
		isSecondSelect = false;
	}
	public IEnumerator SelectMaskFaild()
	{
		SingletonCustom<FindMask_UIManager>.Instance.HideRemainCountDown();
		if (isFirstSelect)
		{
			maskObjList[firstSelectMask.ObjNo].ResetMaskFlag();
			yield return new WaitForSeconds(1f);
			MaskTurnOver(firstSelectMask.ObjNo, 0f);
			yield return new WaitForSeconds(0.25f);
			maskObjList[firstSelectMask.ObjNo].SetMaskSilhouette();
		}
		yield return new WaitForSeconds(0.5f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_miss");
		yield return new WaitForSeconds(1f);
		SingletonCustom<FindMask_GameManager>.Instance.NextTurnPlayer();
	}
	private void ResetMask()
	{
		isFindAllPair = false;
		pairMaskObjList.Clear();
		maskObjList.Clear();
		maskTypeList.Clear();
		for (int i = 0; i < maskAnchor.Length; i++)
		{
			foreach (Transform item in maskAnchor[i].transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
	}
	public FindMask_MaskData GetMask(int _maskObjNo)
	{
		return maskObjList[_maskObjNo];
	}
	public List<FindMask_MaskData> GetMaskObjList()
	{
		return maskObjList;
	}
	public int GetMaskListCount()
	{
		return maskObjList.Count;
	}
}
