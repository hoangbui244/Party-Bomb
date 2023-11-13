using System.Collections.Generic;
using UnityEngine;
public class FindMask_CharacterManager : SingletonCustom<FindMask_CharacterManager>
{
	[SerializeField]
	[Header("キャラクタ\u30fcカ\u30fcソル")]
	private FindMask_CharacterCursor[] arrayCharaCursor;
	[SerializeField]
	[Header("フィ\u30fcルドカメラ")]
	private Camera fieldCamera;
	private int sameTypeMaskNo;
	private List<FindMask_MaskData> aiMemoryMaskList = new List<FindMask_MaskData>();
	private int maskMemoryCapacity;
	public int SameTypeMaskNo => sameTypeMaskNo;
	public void Init()
	{
		DataInit();
		SetAIMemoryCapacity();
	}
	public void SecondGroupInit()
	{
		DataInit();
		aiMemoryMaskList.Clear();
	}
	private void DataInit()
	{
		for (int i = 0; i < arrayCharaCursor.Length; i++)
		{
			arrayCharaCursor[i].gameObject.SetActive(value: false);
		}
		arrayCharaCursor[SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo].Init();
	}
	private void SetAIMemoryCapacity()
	{
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 0:
			maskMemoryCapacity = FindMask_Define.WEAK_MEMORY_CAPACITY;
			break;
		case 1:
			maskMemoryCapacity = FindMask_Define.NORMAL_MEMORY_CAPACITY;
			break;
		case 2:
			maskMemoryCapacity = FindMask_Define.STRONG_MEMORY_CAPACITY;
			break;
		}
	}
	public void CursorMove(Vector3 _vec)
	{
		arrayCharaCursor[SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo].CursorMove(_vec);
	}
	public void ResetCharacterCursor()
	{
		for (int i = 0; i < arrayCharaCursor.Length; i++)
		{
			arrayCharaCursor[i].gameObject.SetActive(value: false);
		}
		arrayCharaCursor[SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo].Init();
	}
	public void RememberSelectMask(FindMask_MaskData _selectMask)
	{
		if (aiMemoryMaskList.Count != 0)
		{
			for (int i = 0; i < aiMemoryMaskList.Count; i++)
			{
				if (aiMemoryMaskList[i].gameObject == _selectMask.gameObject)
				{
					aiMemoryMaskList.RemoveAt(i);
					aiMemoryMaskList.Add(_selectMask);
					return;
				}
			}
			if (aiMemoryMaskList.Count < maskMemoryCapacity)
			{
				aiMemoryMaskList.Add(_selectMask);
				return;
			}
			aiMemoryMaskList.RemoveAt(0);
			aiMemoryMaskList.Add(_selectMask);
		}
		else
		{
			aiMemoryMaskList.Add(_selectMask);
		}
	}
	public void RemoveMemoryMask(FindMask_MaskData _mask)
	{
		for (int i = 0; i < aiMemoryMaskList.Count; i++)
		{
			if (aiMemoryMaskList[i].gameObject == _mask.gameObject)
			{
				aiMemoryMaskList.RemoveAt(i);
			}
		}
	}
	public bool CheckSameMemoryMaskType(FindMask_MaskData _mask)
	{
		for (int i = 0; i < aiMemoryMaskList.Count; i++)
		{
			if (aiMemoryMaskList[i].gameObject != _mask.gameObject && aiMemoryMaskList[i].TypeNo == _mask.TypeNo)
			{
				sameTypeMaskNo = aiMemoryMaskList[i].ObjNo;
				return true;
			}
		}
		return false;
	}
	public bool CheckSameMemoryMaskObj(FindMask_MaskData _mask)
	{
		for (int i = 0; i < aiMemoryMaskList.Count; i++)
		{
			if (aiMemoryMaskList[i].gameObject == _mask.gameObject)
			{
				return true;
			}
		}
		return false;
	}
	public void SetCursorActive(bool _active)
	{
		arrayCharaCursor[SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo].SetCursorActive(_active);
	}
	public GameObject GetCursor()
	{
		return arrayCharaCursor[SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo].GetCursor();
	}
	public Vector3 GetCursorPos()
	{
		return arrayCharaCursor[SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo].GetCursorPos();
	}
	public int GetSelectMaskeObjNum()
	{
		return arrayCharaCursor[SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo].GetSelectMaskeObjNum();
	}
	public FindMask_MaskData GetSelectMaskeObj()
	{
		return arrayCharaCursor[SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo].GetSelectMaskeObj();
	}
	public Camera GetFieldCamera()
	{
		return fieldCamera;
	}
}
