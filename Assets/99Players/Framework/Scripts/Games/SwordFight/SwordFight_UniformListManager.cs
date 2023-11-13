using System.Collections.Generic;
using UnityEngine;
public class SwordFight_UniformListManager : SingletonCustom<SwordFight_UniformListManager>
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
	public List<Material> materialsGirlList;
	public List<int> modelList;
	public List<Material> hairMaterialsList;
	public List<int> colorTypeList;
	public List<int> colorTypeFemaleList;
	public Mesh girlFaceMesh;
	public Material girlFaceMaterial;
	private List<int> noneHairList = new List<int>
	{
		40,
		41,
		42,
		43,
		44,
		45,
		46,
		47,
		48,
		49,
		50,
		54,
		58,
		60,
		61
	};
	private List<OrderInsertData> selectUniformOrderInsertData = new List<OrderInsertData>
	{
		new OrderInsertData(37, new int[16]
		{
			51,
			52,
			53,
			56,
			57,
			59,
			40,
			41,
			42,
			43,
			50,
			55,
			54,
			58,
			60,
			61
		})
	};
	private List<int> selectUniformOrderTemp = new List<int>();
	private int[] selectUniformOrder;
	public List<int>[] whiteColorNo = new List<int>[2]
	{
		new List<int>(),
		new List<int>()
	};
	public List<int>[] blackColorNo = new List<int>[2]
	{
		new List<int>(),
		new List<int>()
	};
	public List<int>[] redColorNo = new List<int>[2]
	{
		new List<int>(),
		new List<int>()
	};
	public List<int>[] blueColorNo = new List<int>[2]
	{
		new List<int>(),
		new List<int>()
	};
	public List<int>[] greenColorNo = new List<int>[2]
	{
		new List<int>(),
		new List<int>()
	};
	public List<int>[] yellowColorNo = new List<int>[2]
	{
		new List<int>(),
		new List<int>()
	};
	public List<int>[] pinkColorNo = new List<int>[2]
	{
		new List<int>(),
		new List<int>()
	};
	public List<int>[] orangeColorNo = new List<int>[2]
	{
		new List<int>(),
		new List<int>()
	};
	public List<int>[] grayColorNo = new List<int>[2]
	{
		new List<int>(),
		new List<int>()
	};
	public List<int>[] purpleColorNo = new List<int>[2]
	{
		new List<int>(),
		new List<int>()
	};
	private void Start()
	{
		for (int i = 0; i < GetNum(); i++)
		{
			for (int j = 0; j < 2; j++)
			{
				switch (GetColorTypeNo(i, j))
				{
				case 0:
					whiteColorNo[j].Add(i);
					break;
				case 1:
					blackColorNo[j].Add(i);
					break;
				case 2:
					redColorNo[j].Add(i);
					break;
				case 3:
					blueColorNo[j].Add(i);
					break;
				case 4:
					greenColorNo[j].Add(i);
					break;
				case 5:
					yellowColorNo[j].Add(i);
					break;
				case 6:
					pinkColorNo[j].Add(i);
					break;
				case 7:
					orangeColorNo[j].Add(i);
					break;
				case 8:
					grayColorNo[j].Add(i);
					break;
				case 9:
					purpleColorNo[j].Add(i);
					break;
				default:
					whiteColorNo[j].Add(i);
					break;
				}
			}
		}
	}
	public Material GetMaterial(int _no, bool _isGirl = false)
	{
		if (_no >= GetNum())
		{
			UnityEngine.Debug.LogError("ユニフォ\u30fcム番号が存在しません！ --> " + _no.ToString());
			_no = 0;
		}
		if (_isGirl)
		{
			return materialsGirlList[_no];
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
	public int GetColorTypeNo(int _uniformNo, int _genderNo)
	{
		if (_genderNo == 0)
		{
			if (_uniformNo >= colorTypeList.Count)
			{
				UnityEngine.Debug.LogError("ユニフォ\u30fcム番号が存在しません！ --> " + _uniformNo.ToString());
				_uniformNo = 0;
			}
			return colorTypeList[_uniformNo];
		}
		if (_uniformNo >= colorTypeFemaleList.Count)
		{
			UnityEngine.Debug.LogError("ユニフォ\u30fcム番号が存在しません！ --> " + _uniformNo.ToString());
			_uniformNo = 0;
		}
		return colorTypeFemaleList[_uniformNo];
	}
	public int GetNum()
	{
		return materialsList.Count;
	}
	public bool CheckOverlapColor(int _uniformNo1, int _uniformNo2, int _genderNo1, int _genderNo2)
	{
		return GetColorTypeNo(_uniformNo1, _genderNo1) == GetColorTypeNo(_uniformNo2, _genderNo2);
	}
	public bool CheckNoneHair(int _no)
	{
		return noneHairList.Contains(_no);
	}
	public void ChangeEnemyUniformTournament()
	{
	}
	public int[] GetMyUniformColorArray(int _no, int _genderNo)
	{
		for (int i = 0; i < SwordFight_Define.uniformColorType.Length; i++)
		{
			switch (i)
			{
			case 0:
				if (whiteColorNo[_genderNo].Contains(_no))
				{
					return DeepCopyHelper.DeepCopy(whiteColorNo[_genderNo]).ToArray();
				}
				break;
			case 1:
				if (blackColorNo[_genderNo].Contains(_no))
				{
					return DeepCopyHelper.DeepCopy(blackColorNo[_genderNo]).ToArray();
				}
				break;
			case 2:
				if (redColorNo[_genderNo].Contains(_no))
				{
					return DeepCopyHelper.DeepCopy(redColorNo[_genderNo]).ToArray();
				}
				break;
			case 3:
				if (blueColorNo[_genderNo].Contains(_no))
				{
					return DeepCopyHelper.DeepCopy(blueColorNo[_genderNo]).ToArray();
				}
				break;
			case 4:
				if (greenColorNo[_genderNo].Contains(_no))
				{
					return DeepCopyHelper.DeepCopy(greenColorNo[_genderNo]).ToArray();
				}
				break;
			case 5:
				if (yellowColorNo[_genderNo].Contains(_no))
				{
					return DeepCopyHelper.DeepCopy(yellowColorNo[_genderNo]).ToArray();
				}
				break;
			case 6:
				if (pinkColorNo[_genderNo].Contains(_no))
				{
					return DeepCopyHelper.DeepCopy(pinkColorNo[_genderNo]).ToArray();
				}
				break;
			case 7:
				if (orangeColorNo[_genderNo].Contains(_no))
				{
					return DeepCopyHelper.DeepCopy(orangeColorNo[_genderNo]).ToArray();
				}
				break;
			case 8:
				if (grayColorNo[_genderNo].Contains(_no))
				{
					return DeepCopyHelper.DeepCopy(grayColorNo[_genderNo]).ToArray();
				}
				break;
			case 9:
				if (purpleColorNo[_genderNo].Contains(_no))
				{
					return DeepCopyHelper.DeepCopy(purpleColorNo[_genderNo]).ToArray();
				}
				break;
			default:
				if (whiteColorNo[_genderNo].Contains(_no))
				{
					return DeepCopyHelper.DeepCopy(whiteColorNo[_genderNo]).ToArray();
				}
				break;
			}
		}
		return new int[1]
		{
			_no
		};
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
	public Mesh GetGirlFaceMesh()
	{
		return girlFaceMesh;
	}
	public Material GetGirlFaceMaterial()
	{
		return girlFaceMaterial;
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
