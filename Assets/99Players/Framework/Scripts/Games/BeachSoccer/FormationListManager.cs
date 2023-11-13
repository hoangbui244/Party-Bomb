using System;
using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class FormationListManager : SingletonCustom<FormationListManager>
	{
		public struct OrderInsertData
		{
			public int orderNo;
			public int[] insertNo;
			public OrderInsertData(int _orderNo, int[] _insertNo)
			{
				orderNo = _orderNo;
				insertNo = _insertNo;
			}
		}
		[SerializeField]
		[Header("フォ\u30fcメ\u30fcションリスト")]
		public List<FormationList> formationList;
		private List<OrderInsertData> selectFormationOrderInsertData = new List<OrderInsertData>
		{
			new OrderInsertData(0, new int[1]
			{
				1
			})
		};
		private List<int> selectFormationOrderTemp = new List<int>();
		private int[] selectFormationOrder;
		private void Awake()
		{
		}
		public FormationData GetData(int _formationNo)
		{
			if (_formationNo >= GetNum())
			{
				UnityEngine.Debug.LogError("フォ\u30fcメ\u30fcション番号が存在しません！ --> " + _formationNo.ToString());
				_formationNo = 0;
			}
			return formationList[_formationNo].formationData;
		}
		public PlayerData[] GetPlayerData(int _formationNo)
		{
			if (_formationNo >= GetNum())
			{
				UnityEngine.Debug.LogError("フォ\u30fcメ\u30fcション番号が存在しません！ --> " + _formationNo.ToString());
				_formationNo = 0;
			}
			return formationList[_formationNo].formationData.playerData;
		}
		public GameDataParams.PositionType GetPosType(int _formationNo, int _posNo)
		{
			if (_formationNo >= GetNum())
			{
				UnityEngine.Debug.LogError("フォ\u30fcメ\u30fcション番号が存在しません！ --> " + _formationNo.ToString());
				_formationNo = 0;
			}
			if (_posNo >= 11)
			{
				UnityEngine.Debug.LogError("ポジション番号が存在しません！ --> " + _posNo.ToString());
				_posNo = 0;
			}
			return formationList[_formationNo].formationData.playerData[_posNo].posType;
		}
		public Vector2 GetPosition(int _formationNo, int _posNo)
		{
			if (_formationNo >= GetNum())
			{
				UnityEngine.Debug.LogError("フォ\u30fcメ\u30fcション番号が存在しません！ --> " + _formationNo.ToString());
				_formationNo = 0;
			}
			if (_posNo >= 11)
			{
				UnityEngine.Debug.LogError("ポジション番号が存在しません！ --> " + _posNo.ToString());
				_posNo = 0;
			}
			return formationList[_formationNo].formationData.playerData[_posNo].pos;
		}
		public string GetName(int _formationNo)
		{
			if (_formationNo >= GetNum())
			{
				UnityEngine.Debug.LogError("フォ\u30fcメ\u30fcション番号が存在しません！ --> " + _formationNo.ToString());
				_formationNo = 0;
			}
			return formationList[_formationNo].formationData.name;
		}
		public string GetInfo(int _formationNo)
		{
			if (_formationNo >= GetNum())
			{
				UnityEngine.Debug.LogError("フォ\u30fcメ\u30fcション番号が存在しません！ --> " + _formationNo.ToString());
				_formationNo = 0;
			}
			return formationList[_formationNo].formationData.info;
		}
		public GameDataParams.Rarity GetRarity(int _formationNo)
		{
			if (_formationNo >= GetNum())
			{
				UnityEngine.Debug.LogError("フォ\u30fcメ\u30fcション番号が存在しません！ --> " + _formationNo.ToString());
				_formationNo = 0;
			}
			return formationList[_formationNo].formationData.rarity;
		}
		public int GetNum()
		{
			return formationList.Count;
		}
		public int[] GetSelectFormationOrder()
		{
			if (selectFormationOrder == null || selectFormationOrder.Length == 0)
			{
				selectFormationOrder = new int[formationList.Count];
				for (int i = 0; i < formationList.Count; i++)
				{
					selectFormationOrderTemp.Add(i);
				}
				int num = -1;
				for (int j = 0; j < selectFormationOrder.Length; j++)
				{
					selectFormationOrder[j] = selectFormationOrderTemp[0];
					selectFormationOrderTemp.RemoveAt(0);
					num = CheckOrderInsertData(selectFormationOrder[j]);
					if (num >= 0)
					{
						for (int k = 0; k < selectFormationOrderInsertData[num].insertNo.Length; k++)
						{
							j++;
							selectFormationOrder[j] = selectFormationOrderInsertData[num].insertNo[k];
							selectFormationOrderTemp.Remove(selectFormationOrderInsertData[num].insertNo[k]);
						}
					}
				}
			}
			return selectFormationOrder;
		}
		private int CheckOrderInsertData(int _orderNo)
		{
			for (int i = 0; i < selectFormationOrderInsertData.Count; i++)
			{
				if (selectFormationOrderInsertData[i].orderNo == _orderNo)
				{
					return i;
				}
			}
			return -1;
		}
		public string GetConvertText(int _formationNo, bool _isNewLine)
		{
			string text = GetData(_formationNo).name;
			if (text.IndexOf("(A)") != -1)
			{
				text = text.Replace("(A)", (_isNewLine ? Environment.NewLine : " ") + "<#b1f682>A");
			}
			if (text.IndexOf("(B)") != -1)
			{
				text = text.Replace("(B)", (_isNewLine ? Environment.NewLine : " ") + "<#f6f582>B");
			}
			if (text.IndexOf("(C)") != -1)
			{
				text = text.Replace("(C)", (_isNewLine ? Environment.NewLine : " ") + "<#eb917c>C");
			}
			if (text.IndexOf("(D)") != -1)
			{
				text = text.Replace("(D)", (_isNewLine ? Environment.NewLine : " ") + "<#82c6f6>D");
			}
			if (text.IndexOf("(E)") != -1)
			{
				text = text.Replace("(E)", (_isNewLine ? Environment.NewLine : " ") + "<#c68bf3>E");
			}
			return text;
		}
	}
}
