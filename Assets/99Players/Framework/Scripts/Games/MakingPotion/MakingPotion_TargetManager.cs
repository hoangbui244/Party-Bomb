using System;
using UnityEngine;
public class MakingPotion_TargetManager : SingletonCustom<MakingPotion_TargetManager>
{
	[Serializable]
	public class TargetData
	{
		public SugarTimeData[] sugarTimeDataArray;
		public TurnTimeData[] turnTimeDataArray;
		public AnimationCurve rotSpeedCurve;
	}
	[Serializable]
	public struct SugarTimeData
	{
		public float time;
		public int sameNum;
		public MakingPotion_PlayerScript.SugarColorType sugarType;
		public MakingPotion_PlayerScript.SugarColorType sugarType_Two;
		public MakingPotion_PlayerScript.SugarColorType sugarType_Three;
	}
	[Serializable]
	public struct TurnTimeData
	{
		public float time;
		public bool isRight;
	}
	public const int CREATE_DATA_NUM = 5;
	private static readonly int[] SUGAR_COUNT_ARRAY = new int[5]
	{
		3,
		3,
		4,
		4,
		4
	};
	private static readonly int[] TURN_COUNT_ARRAY = new int[5]
	{
		2,
		2,
		2,
		3,
		3
	};
	[SerializeField]
	private TargetData[] targetDataArray = new TargetData[5];
	[SerializeField]
	private MakingPotion_TargetDataStorage dataStorage;
	public void Init()
	{
		DataInit();
	}
	public void SecondGroupInit()
	{
		DataInit();
	}
	private void DataInit()
	{
		for (int i = 0; i < 5; i++)
		{
			MakingPotion_PlayerScript.SugarColorType[] randomSugarTypeArray = dataStorage.GetRandomSugarTypeArray(SUGAR_COUNT_ARRAY[i]);
			float[] randomSugarTimeArray = dataStorage.GetRandomSugarTimeArray(SUGAR_COUNT_ARRAY[i]);
			int[] randomSugarSameNumArray = dataStorage.GetRandomSugarSameNumArray(SUGAR_COUNT_ARRAY[i], i == 0);
			targetDataArray[i].sugarTimeDataArray = new SugarTimeData[SUGAR_COUNT_ARRAY[i]];
			for (int j = 0; j < SUGAR_COUNT_ARRAY[i]; j++)
			{
				targetDataArray[i].sugarTimeDataArray[j].sugarType = randomSugarTypeArray[j];
				targetDataArray[i].sugarTimeDataArray[j].sugarType_Two = (MakingPotion_PlayerScript.SugarColorType)UnityEngine.Random.Range(1, 5);
				targetDataArray[i].sugarTimeDataArray[j].sugarType_Three = (MakingPotion_PlayerScript.SugarColorType)UnityEngine.Random.Range(1, 5);
				if (targetDataArray[i].sugarTimeDataArray[j].sugarType == targetDataArray[i].sugarTimeDataArray[j].sugarType_Two && targetDataArray[i].sugarTimeDataArray[j].sugarType == targetDataArray[i].sugarTimeDataArray[j].sugarType_Three)
				{
					int sugarType_Three = (int)targetDataArray[i].sugarTimeDataArray[j].sugarType_Three;
					sugarType_Three += UnityEngine.Random.Range(1, 4);
					if (sugarType_Three > 4)
					{
						sugarType_Three -= 4;
					}
					targetDataArray[i].sugarTimeDataArray[j].sugarType_Three = (MakingPotion_PlayerScript.SugarColorType)sugarType_Three;
				}
				targetDataArray[i].sugarTimeDataArray[j].time = randomSugarTimeArray[j];
				targetDataArray[i].sugarTimeDataArray[j].sameNum = randomSugarSameNumArray[j];
			}
			bool[] randomTurnFlagArray = dataStorage.GetRandomTurnFlagArray(TURN_COUNT_ARRAY[i]);
			float[] randomTurnTimeArray = dataStorage.GetRandomTurnTimeArray(TURN_COUNT_ARRAY[i]);
			targetDataArray[i].turnTimeDataArray = new TurnTimeData[TURN_COUNT_ARRAY[i]];
			for (int k = 0; k < TURN_COUNT_ARRAY[i]; k++)
			{
				targetDataArray[i].turnTimeDataArray[k].isRight = randomTurnFlagArray[k];
				targetDataArray[i].turnTimeDataArray[k].time = randomTurnTimeArray[k];
				if (i == 0)
				{
					targetDataArray[i].rotSpeedCurve = dataStorage.GetEasyRandomRotSpeedCurve();
				}
				else
				{
					targetDataArray[i].rotSpeedCurve = dataStorage.GetRandomRotSpeedCurve();
				}
			}
		}
	}
	public TargetData GetData(int _createNo)
	{
		return targetDataArray[_createNo % targetDataArray.Length];
	}
}
