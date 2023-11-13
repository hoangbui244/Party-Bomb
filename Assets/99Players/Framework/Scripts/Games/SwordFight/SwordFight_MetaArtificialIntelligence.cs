using System.Collections.Generic;
using UnityEngine;
public class SwordFight_MetaArtificialIntelligence
{
	public struct CharacterData
	{
		public int no;
		public float value;
		public CharacterData(int _no, float _value)
		{
			no = _no;
			value = _value;
		}
	}
	public struct CharaDistribution
	{
		public int[,] charaDistribution;
		public CharaDistribution(int _xSplit, int _ySplit)
		{
			charaDistribution = new int[_xSplit, _ySplit];
		}
	}
	public enum RestrictedAreaType
	{
		NONE,
		THROW_IN,
		ALLY_KEEP
	}
	public struct InRestrictedArea
	{
		public RestrictedAreaType type;
		public Vector3 pos;
		public float size;
		public InRestrictedArea(RestrictedAreaType _type, Vector3 _pos, float _size)
		{
			type = _type;
			pos = _pos;
			size = _size;
		}
	}
	private static readonly int DISTRIBUTION_SPLIT_NUM_X = 25;
	private static readonly int DISTRIBUTION_SPLIT_NUM_Y = 41;
	public Vector2 charaDistributionSplitPer;
	public Vector2 charaDistributionSplitSize;
	private CharaDistribution[] charaDistributionList = new CharaDistribution[2]
	{
		new CharaDistribution(DISTRIBUTION_SPLIT_NUM_X, DISTRIBUTION_SPLIT_NUM_Y),
		new CharaDistribution(DISTRIBUTION_SPLIT_NUM_X, DISTRIBUTION_SPLIT_NUM_Y)
	};
	private float UPDATA_INTERVAl = 0.1f;
	private float updataInterval;
	private List<CharacterData>[] distanceFromBallOrder = new List<CharacterData>[2]
	{
		new List<CharacterData>(),
		new List<CharacterData>()
	};
	private float[,] distanceFromBall = new float[2, 11];
	private float[,] distanceFromMyGoal = new float[2, 11];
	private float[,] distanceFromOpponentGoal = new float[2, 11];
	private Vector3[] ballPosPer = new Vector3[2];
	private float[] distanceFromGoalToBall = new float[2];
	private InRestrictedArea[] restrictedArea = new InRestrictedArea[2];
	public void Init()
	{
		charaDistributionList = new CharaDistribution[SingletonCustom<GameSettingManager>.Instance.PlayerNum];
		for (int i = 0; i < charaDistributionList.Length; i++)
		{
			charaDistributionList[i] = new CharaDistribution(DISTRIBUTION_SPLIT_NUM_X, DISTRIBUTION_SPLIT_NUM_Y);
		}
		charaDistributionSplitPer.x = 1f / (float)charaDistributionList[0].charaDistribution.GetLength(0);
		charaDistributionSplitPer.y = 1f / (float)charaDistributionList[0].charaDistribution.GetLength(1);
		charaDistributionSplitSize.x = SingletonCustom<SwordFight_FieldManager>.Instance.GetFieldData().AreaSize.x / (float)charaDistributionList[0].charaDistribution.GetLength(0);
		charaDistributionSplitSize.y = SingletonCustom<SwordFight_FieldManager>.Instance.GetFieldData().AreaSize.z * 2f / (float)charaDistributionList[0].charaDistribution.GetLength(1);
		UpdateMethod();
	}
	public void UpdateMethod()
	{
		if (updataInterval <= 0f)
		{
			updataInterval = UPDATA_INTERVAl;
			UpdateCharaDistribution();
		}
		else
		{
			updataInterval -= Time.deltaTime;
		}
	}
	private void UpdateCharaDistribution()
	{
		for (int i = 0; i < SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList.Length; i++)
		{
			for (int j = 0; j < charaDistributionList[i].charaDistribution.GetLength(0); j++)
			{
				for (int k = 0; k < charaDistributionList[i].charaDistribution.GetLength(1); k++)
				{
					charaDistributionList[i].charaDistribution[j, k] = 0;
				}
			}
			if (SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList[i] == null)
			{
				continue;
			}
			Vector3 vector = SingletonCustom<SwordFight_FieldManager>.Instance.ConvertLocalPosPer(SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList[i].GetPos(), 0);
			int num = (int)((vector.x + 0.5f) / charaDistributionSplitPer.x);
			int num2 = (int)(vector.z / charaDistributionSplitPer.y);
			for (int l = -3; l <= 3; l++)
			{
				for (int m = -3; m <= 3; m++)
				{
					if (num + l >= 0 && num + l < charaDistributionList[i].charaDistribution.GetLength(0) && num2 + m >= 0 && num2 + m < charaDistributionList[i].charaDistribution.GetLength(1))
					{
						charaDistributionList[i].charaDistribution[num + l, num2 + m] += 4 - ((Mathf.Abs(l) > Mathf.Abs(m)) ? Mathf.Abs(l) : Mathf.Abs(m));
					}
				}
			}
		}
	}
	public void UpdateRestrictedAreaData()
	{
		for (int i = 0; i < restrictedArea.Length; i++)
		{
			restrictedArea[i].type = RestrictedAreaType.NONE;
		}
	}
	public InRestrictedArea GetRestrictedArea(int _teamNo)
	{
		return restrictedArea[_teamNo];
	}
	public int GetDistanceFromBallOrder(int _teamNo, int _order)
	{
		return distanceFromBallOrder[_teamNo][_order].no;
	}
	public float GetDistanceFromBall(int _teamNo, int _order, bool _my = true)
	{
		if (_my)
		{
			return distanceFromBallOrder[_teamNo][_order].value;
		}
		return distanceFromBallOrder[(_teamNo == 0) ? 1 : 0][_order].value;
	}
	public float GetDistanceFromBall(int _teamNo, int _charaNo)
	{
		return distanceFromBall[_teamNo, _charaNo];
	}
	public float GetDistanceFromMyGoal(int _teamNo, int _charaNo)
	{
		return distanceFromMyGoal[_teamNo, _charaNo];
	}
	public float GetDistanceFromMyGoal(int _teamNo, int _order, bool _my = true)
	{
		if (_my)
		{
			return distanceFromMyGoal[_teamNo, _order];
		}
		return distanceFromMyGoal[(_teamNo == 0) ? 1 : 0, _order];
	}
	public float GetDistanceFromOpponentGoal(int _teamNo, int _charaNo)
	{
		return distanceFromOpponentGoal[_teamNo, _charaNo];
	}
	public float GetDistanceFromOpponentGoal(int _teamNo, SwordFight_CharacterScript _chara)
	{
		return distanceFromOpponentGoal[_teamNo, _chara.CharaNo];
	}
	public float GetDistanceFromGoalToBall(int _teamNo)
	{
		return distanceFromGoalToBall[_teamNo];
	}
	public Vector3 GetBallPosPer(int _teamNo)
	{
		return ballPosPer[_teamNo];
	}
	public Vector3 GetSpacePos(Vector3 _centerPos, int _xRangeMin, int _xRangeMax, int _yRangeMin, int _yRangeMax, int _teamNo)
	{
		Vector3 vector = SingletonCustom<SwordFight_FieldManager>.Instance.ConvertLocalPosPer(_centerPos, 0);
		int num = (int)((vector.x + 0.5f) / charaDistributionSplitPer.x);
		int num2 = (int)(vector.z / charaDistributionSplitPer.y);
		if (num + _xRangeMin < 0)
		{
			_xRangeMin += Mathf.Abs(num + _xRangeMin);
			_xRangeMax += Mathf.Abs(num + _xRangeMin);
		}
		if (num + _xRangeMax >= DISTRIBUTION_SPLIT_NUM_X)
		{
			_xRangeMin += DISTRIBUTION_SPLIT_NUM_X - 1 - (num + _xRangeMax);
			_xRangeMax += DISTRIBUTION_SPLIT_NUM_X - 1 - (num + _xRangeMax);
		}
		if (num2 + _yRangeMin < 0)
		{
			_yRangeMin += Mathf.Abs(num2 + _yRangeMin);
			_yRangeMax += Mathf.Abs(num2 + _yRangeMin);
		}
		if (num2 + _yRangeMax >= DISTRIBUTION_SPLIT_NUM_Y)
		{
			_yRangeMin += DISTRIBUTION_SPLIT_NUM_Y - 1 - (num2 + _yRangeMax);
			_yRangeMax += DISTRIBUTION_SPLIT_NUM_Y - 1 - (num2 + _yRangeMax);
		}
		List<int[]> list = new List<int[]>();
		int num3 = 99;
		for (int i = _xRangeMin; i <= _xRangeMax; i++)
		{
			for (int j = _yRangeMin; j <= _yRangeMax; j++)
			{
				if (num + i >= 0 && num + i < DISTRIBUTION_SPLIT_NUM_X && num2 + j >= 0 && num2 + j < DISTRIBUTION_SPLIT_NUM_Y)
				{
					int num4 = charaDistributionList[0].charaDistribution[num + i, num2 + j] + charaDistributionList[1].charaDistribution[num + i, num2 + j];
					if (num4 < num3)
					{
						list.Clear();
						num3 = num4;
						list.Add(new int[2]
						{
							num + i,
							num2 + j
						});
					}
					else if (num4 == num3)
					{
						list.Add(new int[2]
						{
							num + i,
							num2 + j
						});
					}
				}
			}
		}
		Vector3 position = SingletonCustom<SwordFight_FieldManager>.Instance.GetAnchors().CenterAnchor.position;
		if (list.Count >= 1)
		{
			int index = Random.Range(0, list.Count);
			position.x -= charaDistributionSplitSize.x * (float)DISTRIBUTION_SPLIT_NUM_X * 0.5f;
			position.x += charaDistributionSplitSize.x * ((float)list[index][0] + 0.5f);
			position.z += charaDistributionSplitSize.y * ((float)list[index][1] + 0.5f);
		}
		return position;
	}
}
