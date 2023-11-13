using System.Collections.Generic;
using UnityEngine;
public class BigMerchantCustomer : MonoBehaviour
{
	[SerializeField]
	[Header("キャラモデル")]
	private BigMerchantCustomerCharacter[] character;
	[SerializeField]
	[Header("カウンタ\u30fc位置")]
	private Transform anchorCounter;
	[SerializeField]
	[Header("要求リスト")]
	private BigMerchantCustomerOrderList orderList;
	[SerializeField]
	[Header("コインエフェクト")]
	private ParticleSystem psCoinEffect;
	private float enterTime;
	private BigMerchantCustomerCharacter.State enterState;
	private int ONE_GAME_ITEM_MAX = 5;
	private List<FireworksBall.ItemType> listColorType = new List<FireworksBall.ItemType>();
	public List<FireworksBall.ItemType> ListColorType
	{
		get
		{
			return listColorType;
		}
		set
		{
			listColorType = value;
		}
	}
	public bool IsPassItem
	{
		get;
		set;
	}
	public Vector3 CounterPos => anchorCounter.position;
	public void Init()
	{
		for (int i = 0; i < character.Length; i++)
		{
			character[i].Init();
		}
		orderList.SetCustomer(this);
		EnterCheck();
	}
	public void OnResult()
	{
		for (int i = 0; i < character.Length; i++)
		{
			character[i].OnResult();
		}
	}
	public void UpdateMethod()
	{
		if (enterTime > 0f)
		{
			enterTime -= Time.deltaTime;
		}
		else
		{
			EnterCheck();
		}
		for (int i = 0; i < character.Length; i++)
		{
			character[i].UpdateMethod();
		}
	}
	public void EnterCheck()
	{
		enterState = BigMerchantCustomerCharacter.State.Wait;
		for (int i = 0; i < character.Length; i++)
		{
			if ((character[i].CurrentState == BigMerchantCustomerCharacter.State.Move_Counter || character[i].CurrentState == BigMerchantCustomerCharacter.State.Move_Wait1 || character[i].CurrentState == BigMerchantCustomerCharacter.State.Move_Wait2) && enterState < character[i].CurrentState)
			{
				enterState = character[i].CurrentState;
			}
		}
		for (int j = 0; j < character.Length; j++)
		{
			if (character[j].CurrentState == BigMerchantCustomerCharacter.State.Wait)
			{
				switch (enterState)
				{
				case BigMerchantCustomerCharacter.State.Wait:
					character[j].EnterShop(BigMerchantCustomerCharacter.State.Move_Counter);
					break;
				case BigMerchantCustomerCharacter.State.Move_Counter:
					character[j].EnterShop(BigMerchantCustomerCharacter.State.Move_Wait1);
					break;
				case BigMerchantCustomerCharacter.State.Move_Wait1:
					character[j].EnterShop(BigMerchantCustomerCharacter.State.Move_Wait2);
					break;
				}
				break;
			}
		}
		enterTime = 5f + UnityEngine.Random.Range(0f, 1f);
	}
	public void OnClosedShop()
	{
		listColorType.Clear();
		orderList.Hide();
	}
	public void Set(int _playerIdx, FireworksBall.ItemType _type, int _score)
	{
		SingletonCustom<FireworksUIManager>.Instance.ShowPoint(SingletonCustom<FireworksPlayerManager>.Instance.GetPlayerAtIdx(_playerIdx), base.transform.position, _score, _isTopViewCamera: false);
		listColorType.Remove(_type);
		if (listColorType.Count > 0)
		{
			for (int i = 0; i < character.Length; i++)
			{
				if (character[i].CurrentState == BigMerchantCustomerCharacter.State.Move_Counter)
				{
					character[i].ResetStayingTime();
				}
			}
			orderList.Show();
		}
		else
		{
			orderList.Hide();
			for (int j = 0; j < character.Length; j++)
			{
				if (character[j].CurrentState == BigMerchantCustomerCharacter.State.Move_Counter)
				{
					character[j].ClosedShop(0.55f);
				}
			}
			SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
		}
		psCoinEffect.Play();
		SingletonCustom<AudioManager>.Instance.SePlay("se_purchase ");
		SingletonCustom<AudioManager>.Instance.SePlay("se_goeraser_item_0");
		for (int k = 0; k < character.Length; k++)
		{
			if (character[k].CurrentState == BigMerchantCustomerCharacter.State.Move_Counter)
			{
				character[k].PlayBuyAnim();
			}
		}
	}
	public void NextMove()
	{
		for (int i = 0; i < character.Length; i++)
		{
			switch (character[i].CurrentState)
			{
			case BigMerchantCustomerCharacter.State.Move_Wait1:
				character[i].EnterShop(BigMerchantCustomerCharacter.State.Move_Counter);
				break;
			case BigMerchantCustomerCharacter.State.Move_Wait2:
				character[i].EnterShop(BigMerchantCustomerCharacter.State.Move_Wait1);
				break;
			}
		}
	}
	public bool IsMatchList(FireworksBall[] _targetItem)
	{
		if (listColorType.Count > 0)
		{
			for (int i = 0; i < _targetItem.Length; i++)
			{
				if (!_targetItem[i].gameObject.activeSelf)
				{
					continue;
				}
				for (int j = 0; j < listColorType.Count; j++)
				{
					if (_targetItem[i].Color == listColorType[j])
					{
						return true;
					}
				}
			}
		}
		return false;
	}
	public void CreateOrder()
	{
		int num = UnityEngine.Random.Range(2, 4);
		listColorType.Clear();
		for (int i = 0; i < num; i++)
		{
			listColorType.Add(SingletonCustom<BigMerchantSupplyManager>.Instance.ListItemType[Random.Range(0, ONE_GAME_ITEM_MAX)]);
		}
		orderList.Show();
		IsPassItem = true;
		SingletonCustom<AudioManager>.Instance.SePlay("se_customize_parts_8");
	}
}
