using System.Collections.Generic;
using UnityEngine;
public class BeachVolley_MetaAI
{
	public struct CharacterData
	{
		public int no;
		public float value;
		public string name;
		public CharacterData(int _no, float _value, string _name = "")
		{
			no = _no;
			value = _value;
			name = _name;
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
	private CharaDistribution[] charaDistribution = new CharaDistribution[2]
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
	private List<CharacterData>[] distanceFromBallDropPrediPosOrder = new List<CharacterData>[2]
	{
		new List<CharacterData>(),
		new List<CharacterData>()
	};
	private float[,] distanceFromBall = new float[2, 11];
	private float[,] distanceFromBallDropPrediPos = new float[2, 11];
	private Vector3[] ballPosPer = new Vector3[2];
	private float[] distanceFromGoalToBall = new float[2];
	private InRestrictedArea[] restrictedArea = new InRestrictedArea[2];
	public void Init()
	{
		charaDistributionSplitPer.x = 1f / (float)charaDistribution[0].charaDistribution.GetLength(0);
		charaDistributionSplitPer.y = 1f / (float)charaDistribution[0].charaDistribution.GetLength(1);
		charaDistributionSplitSize.x = BeachVolley_Define.FM.GetFieldData().HalfCourtSize.x / (float)charaDistribution[0].charaDistribution.GetLength(0);
		charaDistributionSplitSize.y = BeachVolley_Define.FM.GetFieldData().HalfCourtSize.z * 2f / (float)charaDistribution[0].charaDistribution.GetLength(1);
		UpdateMethod();
	}
	public void UpdateMethod()
	{
		if (updataInterval <= 0f)
		{
			updataInterval = UPDATA_INTERVAl;
			UpdateData();
		}
		else
		{
			updataInterval -= Time.deltaTime;
		}
		DebugUpdate();
	}
	private void DebugUpdate()
	{
	}
	public void ResetUpdateInterval()
	{
		updataInterval = 0f;
	}
	public void UpdateData()
	{
		CalcDistanceFromBall();
		for (int i = 0; i < ballPosPer.Length; i++)
		{
			ballPosPer[i] = BeachVolley_Define.FM.ConvertLocalPosPer(SingletonCustom<BeachVolley_BallManager>.Instance.GetBallPos(), i);
		}
		if (BeachVolley_Define.BM.CheckBallState(BeachVolley_BallManager.BallState.FREE))
		{
			CalcDistanceFromBallDropPrediPos();
		}
	}
	private void UpdateCharaDistribution()
	{
		for (int i = 0; i < BeachVolley_Define.MCM.TeamList.Length; i++)
		{
			for (int j = 0; j < charaDistribution[i].charaDistribution.GetLength(0); j++)
			{
				for (int k = 0; k < charaDistribution[i].charaDistribution.GetLength(1); k++)
				{
					charaDistribution[i].charaDistribution[j, k] = 0;
				}
			}
			for (int l = 0; l < BeachVolley_Define.MCM.TeamList[i].charas.Length; l++)
			{
				Vector3 vector = BeachVolley_Define.FM.ConvertLocalPosPer(BeachVolley_Define.MCM.TeamList[i].charas[l].GetPos(), 0);
				int num = (int)((vector.x + 0.5f) / charaDistributionSplitPer.x);
				int num2 = (int)(vector.z / charaDistributionSplitPer.y);
				for (int m = -3; m <= 3; m++)
				{
					for (int n = -3; n <= 3; n++)
					{
						if (num + m >= 0 && num + m < charaDistribution[i].charaDistribution.GetLength(0) && num2 + n >= 0 && num2 + n < charaDistribution[i].charaDistribution.GetLength(1))
						{
							charaDistribution[i].charaDistribution[num + m, num2 + n] += 4 - ((Mathf.Abs(m) > Mathf.Abs(n)) ? Mathf.Abs(m) : Mathf.Abs(n));
						}
					}
				}
			}
		}
	}
	public void DrawCharaDistribution()
	{
	}
	private void CalcDistanceFromBall()
	{
		float num = 0f;
		for (int i = 0; i < BeachVolley_Define.MCM.TeamList.Length; i++)
		{
			distanceFromBallOrder[i].Clear();
			for (int j = 0; j < BeachVolley_Define.MCM.TeamList[i].charas.Length; j++)
			{
				num = SingletonCustom<BeachVolley_BallManager>.Instance.GetBallDistance(BeachVolley_Define.MCM.TeamList[i].charas[j].GetPos());
				distanceFromBall[i, j] = num;
				if (distanceFromBallOrder[i].Count == 0)
				{
					distanceFromBallOrder[i].Add(new CharacterData(j, num));
					continue;
				}
				if (num < distanceFromBallOrder[i][0].value)
				{
					distanceFromBallOrder[i].Insert(0, new CharacterData(j, num));
					continue;
				}
				for (int k = 0; k < distanceFromBallOrder[i].Count; k++)
				{
					if (num < distanceFromBallOrder[i][k].value)
					{
						distanceFromBallOrder[i].Insert(k, new CharacterData(j, num));
						break;
					}
					if (k == distanceFromBallOrder[i].Count - 1)
					{
						distanceFromBallOrder[i].Add(new CharacterData(j, num));
						break;
					}
				}
			}
		}
	}
	public void CalcDistanceFromBallDropPrediPos()
	{
		float num = 0f;
		for (int i = 0; i < BeachVolley_Define.MCM.TeamList.Length; i++)
		{
			distanceFromBallDropPrediPosOrder[i].Clear();
			for (int j = 0; j < BeachVolley_Define.Return_team_infield_num(); j++)
			{
				num = SingletonCustom<BeachVolley_BallManager>.Instance.GetBallDropPrediPosGroundDistance(BeachVolley_Define.MCM.TeamList[i].charas[j].GetPos());
				distanceFromBallDropPrediPos[i, j] = num;
				if (distanceFromBallDropPrediPosOrder[i].Count == 0)
				{
					distanceFromBallDropPrediPosOrder[i].Add(new CharacterData(j, num, BeachVolley_Define.MCM.TeamList[i].charas[j].gameObject.name));
					continue;
				}
				if (num < distanceFromBallDropPrediPosOrder[i][0].value)
				{
					distanceFromBallDropPrediPosOrder[i].Insert(0, new CharacterData(j, num, BeachVolley_Define.MCM.TeamList[i].charas[j].gameObject.name));
					continue;
				}
				for (int k = 0; k < distanceFromBallDropPrediPosOrder[i].Count; k++)
				{
					if (num < distanceFromBallDropPrediPosOrder[i][k].value)
					{
						distanceFromBallDropPrediPosOrder[i].Insert(k, new CharacterData(j, num, BeachVolley_Define.MCM.TeamList[i].charas[j].gameObject.name));
						break;
					}
					if (k == distanceFromBallDropPrediPosOrder[i].Count - 1)
					{
						distanceFromBallDropPrediPosOrder[i].Add(new CharacterData(j, num, BeachVolley_Define.MCM.TeamList[i].charas[j].gameObject.name));
						break;
					}
				}
			}
		}
	}
	public void UpdateRestrictedAreaData()
	{
	}
	public InRestrictedArea GetRestrictedArea(int _teamNo)
	{
		return restrictedArea[_teamNo];
	}
	public int GetDistanceFromBallDropPrediPosOrder(int _teamNo, int _order)
	{
		return distanceFromBallDropPrediPosOrder[_teamNo][_order].no;
	}
	public float GetDistanceFromBallDropPrediPos(int _teamNo, int _charaNo)
	{
		return distanceFromBallDropPrediPos[_teamNo, _charaNo];
	}
	public int GetDistanceFromBallOrder(int _teamNo, int _order)
	{
		return distanceFromBallOrder[_teamNo][_order].no;
	}
	public float GetDistanceFromBall(int _teamNo, int _order, bool _my)
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
	public Vector3 GetBallPosPer(int _teamNo)
	{
		return ballPosPer[_teamNo];
	}
	public Vector3 GetSpacePos(Vector3 _centerPos, int _xRangeMin, int _xRangeMax, int _yRangeMin, int _yRangeMax, int _teamNo)
	{
		Vector3 vector = BeachVolley_Define.FM.ConvertLocalPosPer(_centerPos, 0);
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
					int num4 = charaDistribution[0].charaDistribution[num + i, num2 + j] + charaDistribution[1].charaDistribution[num + i, num2 + j];
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
		Vector3 position = BeachVolley_Define.FM.GetFieldData().frontLeft.position;
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
