using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class UniformListManager : SingletonCustom<UniformListManager>
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
		public List<Material> materialsList;
		public List<int> modelList;
		public List<Material> keeperMaterialsList;
		public List<Material> hairMaterialsList;
		public List<int> colorTypeList;
		private List<int> noneHairList = new List<int>
		{
			35,
			36,
			37,
			38,
			47,
			48,
			60,
			61,
			62,
			63,
			64
		};
		private List<OrderInsertData> selectUniformOrderInsertData = new List<OrderInsertData>
		{
			new OrderInsertData(0, new int[62]
			{
				1,
				2,
				45,
				42,
				43,
				3,
				32,
				5,
				6,
				7,
				11,
				9,
				53,
				8,
				57,
				10,
				4,
				41,
				12,
				58,
				14,
				15,
				51,
				52,
				55,
				56,
				46,
				44,
				39,
				49,
				20,
				19,
				22,
				18,
				16,
				17,
				21,
				50,
				23,
				24,
				40,
				33,
				54,
				59,
				27,
				25,
				13,
				31,
				26,
				34,
				60,
				61,
				62,
				30,
				28,
				48,
				38,
				36,
				47,
				35,
				29,
				37
			})
		};
		private List<int> selectUniformOrderTemp = new List<int>();
		private int[] selectUniformOrder;
		public List<int> whiteColorNo = new List<int>();
		public List<int> blackColorNo = new List<int>();
		public List<int> redColorNo = new List<int>();
		public List<int> blueColorNo = new List<int>();
		public List<int> greenColorNo = new List<int>();
		public List<int> yellowColorNo = new List<int>();
		public List<int> pinkColorNo = new List<int>();
		public List<int> orangeColorNo = new List<int>();
		private void Start()
		{
			for (int i = 0; i < GetNum(); i++)
			{
				switch (GetColorTypeNo(i))
				{
				case 0:
					whiteColorNo.Add(i);
					break;
				case 1:
					blackColorNo.Add(i);
					break;
				case 2:
					redColorNo.Add(i);
					break;
				case 3:
					blueColorNo.Add(i);
					break;
				case 4:
					greenColorNo.Add(i);
					break;
				case 5:
					yellowColorNo.Add(i);
					break;
				case 6:
					pinkColorNo.Add(i);
					break;
				case 7:
					orangeColorNo.Add(i);
					break;
				default:
					whiteColorNo.Add(i);
					break;
				}
			}
		}
		public Material GetMaterial(int _no, bool _isGK = false)
		{
			if (_no >= GetNum())
			{
				UnityEngine.Debug.LogError("ユニフォ\u30fcム番号が存在しません！ --> " + _no.ToString());
				_no = 0;
			}
			if (_isGK)
			{
				return keeperMaterialsList[_no];
			}
			return materialsList[_no];
		}
		public Material GetHairMaterial(int _no)
		{
			if (_no >= hairMaterialsList.Count)
			{
				UnityEngine.Debug.LogError("髪マテリアル番号が存在しません！ --> " + _no.ToString());
				_no = 0;
			}
			return hairMaterialsList[_no];
		}
		public int GetColorTypeNo(int _uniformNo)
		{
			if (_uniformNo >= colorTypeList.Count)
			{
				UnityEngine.Debug.LogError("ユニフォ\u30fcム番号が存在しません！ --> " + _uniformNo.ToString());
				_uniformNo = 0;
			}
			return colorTypeList[_uniformNo];
		}
		public int GetNum()
		{
			return materialsList.Count;
		}
		public bool CheckOverlapColor(int _uniformNo1, int _uniformNo2)
		{
			return GetColorTypeNo(_uniformNo1) == GetColorTypeNo(_uniformNo2);
		}
		public bool CheckNoneHair(int _no)
		{
			return noneHairList.Contains(_no);
		}
		public void ChangeEnemyUniformTournament()
		{
			if (GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.TOURNAMENT) && SingletonCustom<UniformListManager>.Instance.CheckOverlapColor(SchoolData.GetCommonUniformNo(0), SchoolData.GetCommonUniformNo(1)))
			{
				int num;
				do
				{
					num = Random.Range(0, SingletonCustom<UniformListManager>.Instance.GetNum());
				}
				while (SingletonCustom<UniformListManager>.Instance.CheckOverlapColor(num, SchoolData.GetCommonUniformNo(0)) || num == SchoolData.GetCommonUniformNo(0) || GameDataParams.UNIFORM_TOURNAMENT_UNLOCK_DESC.ContainsKey(num) || GameDataParams.UNIFORM_ITEM_UNLOCK_DESC.ContainsKey(num) || GameDataParams.UNIFORM_REWARD_UNLOCK_DESC.ContainsKey(num) || GameDataParams.UNIFORM_DLC1_UNLOCK_DESC.ContainsKey(num) || GameDataParams.UNIFORM_DLC2_UNLOCK_DESC.ContainsKey(num));
				SchoolData.SetUniformNo(0, num, SchoolData.DataType.BATTLE_OPPONENT);
				UnityEngine.Debug.Log("ユニフォ\u30fcムの色かぶり 相手のユニフォ\u30fcム変更");
			}
		}
		public int[] GetMyUniformColorArray(int _no)
		{
			for (int i = 0; i < 8; i++)
			{
				switch (i)
				{
				case 0:
					if (whiteColorNo.Contains(_no))
					{
						return DeepCopyHelper.DeepCopy(whiteColorNo).ToArray();
					}
					break;
				case 1:
					if (blackColorNo.Contains(_no))
					{
						return DeepCopyHelper.DeepCopy(blackColorNo).ToArray();
					}
					break;
				case 2:
					if (redColorNo.Contains(_no))
					{
						return DeepCopyHelper.DeepCopy(redColorNo).ToArray();
					}
					break;
				case 3:
					if (blueColorNo.Contains(_no))
					{
						return DeepCopyHelper.DeepCopy(blueColorNo).ToArray();
					}
					break;
				case 4:
					if (greenColorNo.Contains(_no))
					{
						return DeepCopyHelper.DeepCopy(greenColorNo).ToArray();
					}
					break;
				case 5:
					if (yellowColorNo.Contains(_no))
					{
						return DeepCopyHelper.DeepCopy(yellowColorNo).ToArray();
					}
					break;
				case 6:
					if (pinkColorNo.Contains(_no))
					{
						return DeepCopyHelper.DeepCopy(pinkColorNo).ToArray();
					}
					break;
				case 7:
					if (orangeColorNo.Contains(_no))
					{
						return DeepCopyHelper.DeepCopy(orangeColorNo).ToArray();
					}
					break;
				default:
					if (whiteColorNo.Contains(_no))
					{
						return DeepCopyHelper.DeepCopy(whiteColorNo).ToArray();
					}
					break;
				}
			}
			return DeepCopyHelper.DeepCopy(whiteColorNo).ToArray();
		}
		public int[] GetSelectuniformOrder()
		{
			if (selectUniformOrder == null || selectUniformOrder.Length == 0)
			{
				selectUniformOrder = new int[materialsList.Count];
				for (int i = 0; i < materialsList.Count; i++)
				{
					selectUniformOrderTemp.Add(i);
				}
				int num = -1;
				for (int j = 0; j < selectUniformOrder.Length; j++)
				{
					selectUniformOrder[j] = selectUniformOrderTemp[0];
					selectUniformOrderTemp.RemoveAt(0);
					num = CheckOrderInsertData(selectUniformOrder[j]);
					if (num >= 0)
					{
						for (int k = 0; k < selectUniformOrderInsertData[num].insertNo.Length; k++)
						{
							j++;
							selectUniformOrder[j] = selectUniformOrderInsertData[num].insertNo[k];
							selectUniformOrderTemp.Remove(selectUniformOrderInsertData[num].insertNo[k]);
						}
					}
				}
			}
			return selectUniformOrder;
		}
		private int CheckOrderInsertData(int _orderNo)
		{
			for (int i = 0; i < selectUniformOrderInsertData.Count; i++)
			{
				if (selectUniformOrderInsertData[i].orderNo == _orderNo)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
