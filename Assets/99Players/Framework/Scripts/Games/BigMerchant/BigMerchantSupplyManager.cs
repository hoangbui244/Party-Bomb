using System.Collections.Generic;
using UnityEngine;
public class BigMerchantSupplyManager : SingletonCustom<BigMerchantSupplyManager>
{
	[SerializeField]
	[Header("補給ポイントクラス")]
	private FireworksSlatbox[] arraySupply;
	private List<FireworksSlatbox> listSupply = new List<FireworksSlatbox>();
	private FireworksBall.ItemType[] needList;
	private List<FireworksBall.ItemType> listItemType = new List<FireworksBall.ItemType>();
	private FireworksBall.ItemType targetType;
	private int idx;
	private float distance;
	public List<FireworksBall.ItemType> ListItemType => listItemType;
	public void Init()
	{
		listItemType.Add(FireworksBall.ItemType.MEDICAL_HERB);
		listItemType.Add(FireworksBall.ItemType.NECKLACE);
		listItemType.Add(FireworksBall.ItemType.SWORD);
		listItemType.Add(FireworksBall.ItemType.SHIELD);
		listItemType.Add(FireworksBall.ItemType.PORTION);
		listItemType.Add(FireworksBall.ItemType.BOOK);
		listItemType.Add(FireworksBall.ItemType.CANE);
		listItemType.Add(FireworksBall.ItemType.RING);
		listItemType.Add(FireworksBall.ItemType.ARROW);
		listItemType.Shuffle();
		for (int i = 0; i < arraySupply.Length; i++)
		{
			for (int j = 0; j < listItemType.Count; j++)
			{
				if (arraySupply[i].gameObject.name.Contains("Base" + j.ToString()))
				{
					arraySupply[i].Init(listItemType[j]);
				}
			}
		}
	}
	public FireworksSlatbox GetReturnPos(Vector3 _playerPos, FireworksBall[] _arrayItem, bool _isNear)
	{
		listSupply.Clear();
		for (int i = 0; i < _arrayItem.Length; i++)
		{
			if (!_arrayItem[i].gameObject.activeSelf)
			{
				continue;
			}
			for (int j = 0; j < arraySupply.Length; j++)
			{
				if (_arrayItem[i].Color == arraySupply[j].ColorType)
				{
					listSupply.Add(arraySupply[j]);
				}
			}
		}
		if (listSupply.Count > 0)
		{
			idx = 0;
			distance = 99999f;
			for (int k = 0; k < listSupply.Count; k++)
			{
				CalcManager.mCalcFloat = Vector3.Distance(_playerPos, listSupply[k].transform.position);
				if (distance > CalcManager.mCalcFloat)
				{
					distance = CalcManager.mCalcFloat;
					idx = k;
				}
			}
			return listSupply[idx];
		}
		return null;
	}
	public FireworksSlatbox GetNeedSupplyPos(Vector3 _playerPos, FireworksBall[] _arrayItem, bool _isNear)
	{
		needList = SingletonCustom<BigMerchantCustomerManager>.Instance.GetNeedType();
		listSupply.Clear();
		if (_isNear)
		{
			targetType = needList[Random.Range(0, needList.Length)];
			for (int i = 0; i < arraySupply.Length; i++)
			{
				if (targetType == arraySupply[i].ColorType)
				{
					listSupply.Add(arraySupply[i]);
				}
			}
			if (Random.Range(0, 100) <= 50)
			{
				for (int j = 0; j < listSupply.Count; j++)
				{
					for (int k = 0; k < _arrayItem.Length; k++)
					{
						if (_arrayItem[k].gameObject.activeSelf && listSupply[j].ColorType == _arrayItem[k].Color)
						{
							listSupply.RemoveAt(j);
							j = 0;
							k = _arrayItem.Length;
						}
					}
				}
			}
		}
		else
		{
			for (int l = 0; l < needList.Length; l++)
			{
				for (int m = 0; m < arraySupply.Length; m++)
				{
					if (needList[l] == arraySupply[m].ColorType)
					{
						listSupply.Add(arraySupply[m]);
					}
				}
			}
		}
		if (listSupply.Count > 0)
		{
			if (_isNear)
			{
				idx = 0;
				distance = 99999f;
				for (int n = 0; n < listSupply.Count; n++)
				{
					CalcManager.mCalcFloat = Vector3.Distance(_playerPos, listSupply[n].transform.position);
					if (distance > CalcManager.mCalcFloat)
					{
						distance = CalcManager.mCalcFloat;
						idx = n;
					}
				}
				UnityEngine.Debug.Log("distance:" + distance.ToString() + " idx:" + idx.ToString());
				return listSupply[idx];
			}
			return listSupply[Random.Range(0, listSupply.Count)];
		}
		return null;
	}
	public FireworksSlatbox GetRandomSupplyPos()
	{
		return arraySupply[Random.Range(0, arraySupply.Length)];
	}
}
