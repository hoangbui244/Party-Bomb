using System;
using System.Collections.Generic;
using UnityEngine;
public class FindMask_CharacterControllerAI : MonoBehaviour
{
	private float inputInterval;
	private int firstMaskNo = -1;
	private int secondMaskNo = -1;
	private readonly float MIN_INPUT_TIME = 0.7f;
	private readonly float MAX_INPUT_TIME = 1f;
	private bool isMove;
	private readonly float CURSOR_SPEED = 1.4f;
	private int aiProbability;
	private int findPairCount;
	private const int LAST_MASK_NUM = 2;
	private const int LIMIT_MASK_NUM = 4;
	private const int CONTINUOUS_LIMIT = 4;
	private int lastSelectFirstMaskNo = -1;
	private int lastSelectSecondMaskNo = -1;
	public void Init()
	{
		SetPairMaskProbability();
	}
	public void UpdateMethod()
	{
		if (SingletonCustom<FindMask_GameManager>.Instance.State != FindMask_GameManager.STATE.CHARA_MOVE || SingletonCustom<FindMask_MaskManager>.Instance.IsFindAllPair || SingletonCustom<FindMask_MaskManager>.Instance.PlayEffect)
		{
			return;
		}
		if (inputInterval == 0f)
		{
			InitInputInterval();
		}
		if (!isMove)
		{
			inputInterval -= Time.deltaTime;
		}
		if (inputInterval < 0f && !isMove)
		{
			if (!SingletonCustom<FindMask_MaskManager>.Instance.IsFirstSelect && !isMove)
			{
				firstMaskNo = UnityEngine.Random.Range(0, SingletonCustom<FindMask_MaskManager>.Instance.GetMaskListCount());
				AIMaskSelect(firstMaskNo);
			}
			else if (SingletonCustom<FindMask_MaskManager>.Instance.IsFirstSelect && !isMove)
			{
				AIChooseSecondSelectMaskNo();
				AIMaskSelect(secondMaskNo);
			}
			inputInterval = 0f;
		}
	}
	private void SetPairMaskProbability()
	{
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 0:
			aiProbability = FindMask_Define.WEAK_SELECT_PROBABILITY;
			break;
		case 1:
			aiProbability = FindMask_Define.NORMAL_SELECT_PROBABILITY;
			break;
		case 2:
			aiProbability = FindMask_Define.STRONG_SELECT_PROBABILITY;
			break;
		}
	}
	private void InitInputInterval()
	{
		inputInterval = UnityEngine.Random.Range(MIN_INPUT_TIME, MAX_INPUT_TIME);
	}
	private bool SelectMemoryPairMask()
	{
		if (new List<FindMask_MaskData>(SingletonCustom<FindMask_MaskManager>.Instance.GetMaskObjList()).Count <= 2)
		{
			return true;
		}
		if (findPairCount > 4)
		{
			findPairCount = 0;
			return false;
		}
		int num = UnityEngine.Random.Range(0, 100);
		if (aiProbability > num)
		{
			return true;
		}
		return false;
	}
	public void AddFindPairCount()
	{
		findPairCount++;
	}
	public void ResetFindPairCount()
	{
		findPairCount = 0;
	}
	public void RecordLastSelectMaskNo(int _firstMaskNo, int _secondMaskNo)
	{
		lastSelectFirstMaskNo = _firstMaskNo;
		lastSelectSecondMaskNo = _secondMaskNo;
	}
	private void AIChooseSecondSelectMaskNo()
	{
		if (SingletonCustom<FindMask_CharacterManager>.Instance.CheckSameMemoryMaskType(SingletonCustom<FindMask_MaskManager>.Instance.GetMask(firstMaskNo)) && SelectMemoryPairMask())
		{
			secondMaskNo = SingletonCustom<FindMask_CharacterManager>.Instance.SameTypeMaskNo;
			return;
		}
		List<FindMask_MaskData> list = new List<FindMask_MaskData>(SingletonCustom<FindMask_MaskManager>.Instance.GetMaskObjList());
		if (list.Count <= 2)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (firstMaskNo == list[i].ObjNo)
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		else
		{
			for (int j = 0; j < list.Count; j++)
			{
				if (firstMaskNo == list[j].ObjNo)
				{
					list.RemoveAt(j);
					j--;
				}
				else if (SingletonCustom<FindMask_MaskManager>.Instance.GetMask(firstMaskNo).TypeNo == list[j].TypeNo)
				{
					list.RemoveAt(j);
					j--;
				}
				else if (lastSelectFirstMaskNo == list[j].ObjNo && list.Count > 4)
				{
					list.RemoveAt(j);
					j--;
				}
				else if (lastSelectSecondMaskNo == list[j].ObjNo && list.Count > 4)
				{
					list.RemoveAt(j);
					j--;
				}
			}
		}
		int index = UnityEngine.Random.Range(0, list.Count - 1);
		secondMaskNo = list[index].ObjNo;
	}
	private void AIMaskSelect(int _maskObjNum)
	{
		Vector3 position = SingletonCustom<FindMask_MaskManager>.Instance.GetMask(_maskObjNum).transform.position;
		GameObject cursor = SingletonCustom<FindMask_CharacterManager>.Instance.GetCursor();
		Vector3 vector = new Vector3(position.x, position.y, cursor.transform.position.z);
		float time = (vector - cursor.transform.position).magnitude * CURSOR_SPEED;
		isMove = true;
		LeanTween.move(cursor, vector, time).setOnComplete((Action)delegate
		{
			if (SingletonCustom<FindMask_CharacterManager>.Instance.GetSelectMaskeObjNum() >= 0)
			{
				SingletonCustom<FindMask_MaskManager>.Instance.SelectMask(SingletonCustom<FindMask_CharacterManager>.Instance.GetSelectMaskeObjNum());
				isMove = false;
			}
		});
	}
}
