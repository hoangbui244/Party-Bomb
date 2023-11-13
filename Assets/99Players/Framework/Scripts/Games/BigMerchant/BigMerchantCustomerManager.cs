using System.Collections.Generic;
using UnityEngine;
public class BigMerchantCustomerManager : SingletonCustom<BigMerchantCustomerManager>
{
	[SerializeField]
	[Header("お客クラス")]
	private BigMerchantCustomer[] arrayCustomer;
	[SerializeField]
	[Header("待機ポイント")]
	private Transform[] arrayWaitPoint;
	private List<BigMerchantCustomer> listCustomer = new List<BigMerchantCustomer>();
	private List<FireworksBall.ItemType> listNeedColor = new List<FireworksBall.ItemType>();
	private int CHARACTER_MAT_MAX = 16;
	private int idx;
	private float distance;
	private List<int> listMatIdx = new List<int>();
	private int matIdx;
	public void Init()
	{
		for (int i = 0; i < CHARACTER_MAT_MAX; i++)
		{
			listMatIdx.Add(i);
		}
		listMatIdx.Shuffle();
		matIdx = 0;
		for (int j = 0; j < arrayCustomer.Length; j++)
		{
			arrayCustomer[j].Init();
		}
	}
	public int GetCharacterMatIdx()
	{
		int result = listMatIdx[matIdx];
		matIdx++;
		if (matIdx >= CHARACTER_MAT_MAX)
		{
			listMatIdx.Shuffle();
			matIdx = 0;
		}
		return result;
	}
	public void OnResult()
	{
		for (int i = 0; i < arrayCustomer.Length; i++)
		{
			arrayCustomer[i].OnResult();
		}
	}
	public Vector3 GetWaitPoint(int _idx)
	{
		return arrayWaitPoint[_idx].position;
	}
	public FireworksBall.ItemType[] GetNeedType()
	{
		listNeedColor.Clear();
		for (int i = 0; i < arrayCustomer.Length; i++)
		{
			for (int j = 0; j < arrayCustomer[i].ListColorType.Count; j++)
			{
				listNeedColor.Add(arrayCustomer[i].ListColorType[j]);
			}
		}
		return listNeedColor.ToArray();
	}
	public BigMerchantCustomer GetCustomer(Vector3 _playerPos, FireworksBall[] _targetItem, bool _isNear)
	{
		listCustomer.Clear();
		for (int i = 0; i < arrayCustomer.Length; i++)
		{
			if (arrayCustomer[i].ListColorType.Count <= 0)
			{
				continue;
			}
			for (int j = 0; j < arrayCustomer[i].ListColorType.Count; j++)
			{
				for (int k = 0; k < _targetItem.Length; k++)
				{
					if (_targetItem[k].gameObject.activeSelf && arrayCustomer[i].ListColorType[j] == _targetItem[k].Color)
					{
						listCustomer.Add(arrayCustomer[i]);
					}
				}
			}
		}
		if (listCustomer.Count > 0)
		{
			if (_isNear)
			{
				idx = 0;
				distance = 99999f;
				for (int l = 0; l < listCustomer.Count; l++)
				{
					CalcManager.mCalcFloat = Vector3.Distance(_playerPos, listCustomer[l].transform.position);
					if (distance > CalcManager.mCalcFloat)
					{
						distance = CalcManager.mCalcFloat;
						idx = l;
					}
				}
				return listCustomer[idx];
			}
			return listCustomer[Random.Range(0, listCustomer.Count)];
		}
		return null;
	}
	public void UpdateMethod()
	{
		FireworksGameManager.State currentState = SingletonCustom<FireworksGameManager>.Instance.CurrentState;
		if ((uint)(currentState - 1) <= 1u)
		{
			for (int i = 0; i < arrayCustomer.Length; i++)
			{
				arrayCustomer[i].UpdateMethod();
			}
		}
	}
}
