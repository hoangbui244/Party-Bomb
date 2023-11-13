using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MakingPotion_TargetDataStorage : MonoBehaviour
{
	[Serializable]
	public struct SugarData
	{
		public MakingPotion_PlayerScript.SugarColorType[] sugarTypeArray;
	}
	[Serializable]
	public struct TurnData
	{
		public bool[] isRightArray;
	}
	[Serializable]
	public struct TimeData
	{
		public float[] timeArray;
	}
	private static readonly int[][] SUGAR_CONVERSION_TABLE = new int[24][]
	{
		new int[4]
		{
			0,
			1,
			2,
			3
		},
		new int[4]
		{
			0,
			1,
			3,
			2
		},
		new int[4]
		{
			0,
			2,
			1,
			3
		},
		new int[4]
		{
			0,
			2,
			3,
			1
		},
		new int[4]
		{
			0,
			3,
			1,
			2
		},
		new int[4]
		{
			0,
			3,
			2,
			1
		},
		new int[4]
		{
			1,
			0,
			2,
			3
		},
		new int[4]
		{
			1,
			0,
			3,
			2
		},
		new int[4]
		{
			1,
			2,
			0,
			3
		},
		new int[4]
		{
			1,
			2,
			3,
			0
		},
		new int[4]
		{
			1,
			3,
			0,
			2
		},
		new int[4]
		{
			1,
			3,
			2,
			0
		},
		new int[4]
		{
			2,
			0,
			1,
			3
		},
		new int[4]
		{
			2,
			0,
			3,
			1
		},
		new int[4]
		{
			2,
			1,
			0,
			3
		},
		new int[4]
		{
			2,
			1,
			3,
			1
		},
		new int[4]
		{
			2,
			3,
			0,
			1
		},
		new int[4]
		{
			2,
			3,
			1,
			0
		},
		new int[4]
		{
			3,
			0,
			1,
			2
		},
		new int[4]
		{
			3,
			0,
			2,
			1
		},
		new int[4]
		{
			3,
			1,
			0,
			2
		},
		new int[4]
		{
			3,
			1,
			2,
			0
		},
		new int[4]
		{
			3,
			2,
			0,
			1
		},
		new int[4]
		{
			3,
			2,
			1,
			0
		}
	};
	private static readonly int[][] SUGAR_SAME_NUMS_FIRST_THREE = new int[3][]
	{
		new int[3]
		{
			1,
			1,
			2
		},
		new int[3]
		{
			1,
			1,
			3
		},
		new int[3]
		{
			1,
			2,
			2
		}
	};
	private static readonly int[][] SUGAR_SAME_NUMS_THREE = new int[3][]
	{
		new int[3]
		{
			1,
			1,
			3
		},
		new int[3]
		{
			1,
			2,
			2
		},
		new int[3]
		{
			1,
			2,
			3
		}
	};
	private static readonly int[][] SUGAR_SAME_NUMS_FOUR = new int[5][]
	{
		new int[4]
		{
			1,
			1,
			2,
			2
		},
		new int[4]
		{
			1,
			2,
			2,
			2
		},
		new int[4]
		{
			1,
			1,
			2,
			3
		},
		new int[4]
		{
			1,
			2,
			2,
			3
		},
		new int[4]
		{
			1,
			2,
			3,
			3
		}
	};
	private static readonly int[][] SUGAR_SAME_NUMS_FIVE = new int[5][]
	{
		new int[5]
		{
			1,
			1,
			2,
			2,
			3
		},
		new int[5]
		{
			1,
			2,
			2,
			2,
			3
		},
		new int[5]
		{
			1,
			1,
			2,
			3,
			3
		},
		new int[5]
		{
			1,
			2,
			2,
			3,
			3
		},
		new int[5]
		{
			1,
			2,
			3,
			3,
			3
		}
	};
	[SerializeField]
	[Header("色設定")]
	private SugarData[] threeSugarDataArray;
	[SerializeField]
	private SugarData[] fourSugarDataArray;
	[SerializeField]
	private SugarData[] fiveSugarDataArray;
	[SerializeField]
	private TimeData[] threeSugarTimeArray;
	[SerializeField]
	private TimeData[] fourSugarTimeArray;
	[SerializeField]
	private TimeData[] fiveSugarTimeArray;
	[SerializeField]
	[Header("回転設定")]
	private TurnData[] twoTurnDataArray;
	[SerializeField]
	private TurnData[] threeTurnDataArray;
	[SerializeField]
	private TimeData[] twoTurnTimeArray;
	[SerializeField]
	private TimeData[] threeTurnTimeArray;
	[SerializeField]
	[Header("回転速度設定")]
	private AnimationCurve[] rotSpeedCurveArray;
	public MakingPotion_PlayerScript.SugarColorType[] GetRandomSugarTypeArray(int _length)
	{
		MakingPotion_PlayerScript.SugarColorType[] array = new MakingPotion_PlayerScript.SugarColorType[_length];
		switch (_length)
		{
		case 3:
		{
			int num = UnityEngine.Random.Range(0, threeSugarDataArray.Length);
			for (int j = 0; j < _length; j++)
			{
				array[j] = threeSugarDataArray[num].sugarTypeArray[j];
			}
			break;
		}
		case 4:
		{
			int num = UnityEngine.Random.Range(0, fourSugarDataArray.Length);
			for (int k = 0; k < _length; k++)
			{
				array[k] = fourSugarDataArray[num].sugarTypeArray[k];
			}
			break;
		}
		case 5:
		{
			int num = UnityEngine.Random.Range(0, fiveSugarDataArray.Length);
			for (int i = 0; i < _length; i++)
			{
				array[i] = fiveSugarDataArray[num].sugarTypeArray[i];
			}
			break;
		}
		}
		int num2 = UnityEngine.Random.Range(0, SUGAR_CONVERSION_TABLE.Length);
		for (int l = 0; l < _length; l++)
		{
			int num3 = (int)(array[l] - 1);
			array[l] = (MakingPotion_PlayerScript.SugarColorType)(SUGAR_CONVERSION_TABLE[num2][num3] + 1);
		}
		return array;
	}
	public float[] GetRandomSugarTimeArray(int _length)
	{
		switch (_length)
		{
		case 3:
			return threeSugarTimeArray[UnityEngine.Random.Range(0, threeSugarTimeArray.Length)].timeArray;
		case 4:
			return fourSugarTimeArray[UnityEngine.Random.Range(0, fourSugarTimeArray.Length)].timeArray;
		case 5:
			return fiveSugarTimeArray[UnityEngine.Random.Range(0, fiveSugarTimeArray.Length)].timeArray;
		default:
			return null;
		}
	}
	public int[] GetRandomSugarSameNumArray(int _length, bool _isFirst)
	{
		if (_isFirst && _length == 3)
		{
			return SUGAR_SAME_NUMS_FIRST_THREE[UnityEngine.Random.Range(0, SUGAR_SAME_NUMS_FIRST_THREE.Length)];
		}
		switch (_length)
		{
		case 3:
		{
			List<int> list2 = new List<int>();
			list2.AddRange(SUGAR_SAME_NUMS_THREE[UnityEngine.Random.Range(0, SUGAR_SAME_NUMS_THREE.Length)]);
			return (from a in list2
				orderby Guid.NewGuid()
				select a).ToList().ToArray();
		}
		case 4:
		{
			List<int> list3 = new List<int>();
			list3.AddRange(SUGAR_SAME_NUMS_FOUR[UnityEngine.Random.Range(0, SUGAR_SAME_NUMS_FOUR.Length)]);
			return (from a in list3
				orderby Guid.NewGuid()
				select a).ToList().ToArray();
		}
		case 5:
		{
			List<int> list = new List<int>();
			list.AddRange(SUGAR_SAME_NUMS_FIVE[UnityEngine.Random.Range(0, SUGAR_SAME_NUMS_FIVE.Length)]);
			return (from a in list
				orderby Guid.NewGuid()
				select a).ToList().ToArray();
		}
		default:
			return null;
		}
	}
	public bool[] GetRandomTurnFlagArray(int _length)
	{
		switch (_length)
		{
		case 2:
			return twoTurnDataArray[UnityEngine.Random.Range(0, twoTurnDataArray.Length)].isRightArray;
		case 3:
			return threeTurnDataArray[UnityEngine.Random.Range(0, threeTurnDataArray.Length)].isRightArray;
		default:
			return null;
		}
	}
	public float[] GetRandomTurnTimeArray(int _length)
	{
		switch (_length)
		{
		case 2:
			return twoTurnTimeArray[UnityEngine.Random.Range(0, twoTurnTimeArray.Length)].timeArray;
		case 3:
			return threeTurnTimeArray[UnityEngine.Random.Range(0, threeTurnTimeArray.Length)].timeArray;
		default:
			return null;
		}
	}
	public AnimationCurve GetRandomRotSpeedCurve()
	{
		return rotSpeedCurveArray[UnityEngine.Random.Range(0, rotSpeedCurveArray.Length)];
	}
	public AnimationCurve GetEasyRandomRotSpeedCurve()
	{
		return rotSpeedCurveArray[UnityEngine.Random.Range(0, 2)];
	}
}
