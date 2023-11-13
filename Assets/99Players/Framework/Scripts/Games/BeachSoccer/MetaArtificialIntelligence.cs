using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class MetaArtificialIntelligence
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
			GOAL_KICK,
			CORNER_KICK,
			KEEPER_KEEP,
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
		private float UPDATA_INTERVAl;
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
			charaDistributionSplitPer.x = 1f / (float)charaDistribution[0].charaDistribution.GetLength(0);
			charaDistributionSplitPer.y = 1f / (float)charaDistribution[0].charaDistribution.GetLength(1);
			charaDistributionSplitSize.x = SingletonCustom<FieldManager>.Instance.GetFieldData().halfSize.x / (float)charaDistribution[0].charaDistribution.GetLength(0);
			charaDistributionSplitSize.y = SingletonCustom<FieldManager>.Instance.GetFieldData().halfSize.z * 2f / (float)charaDistribution[0].charaDistribution.GetLength(1);
			UpdateMethod();
		}
		public void UpdateMethod()
		{
			if (updataInterval <= 0f)
			{
				updataInterval = UPDATA_INTERVAl;
				UpdateCharaDistribution();
				CalcDistanceFromBall();
				CalcDistanceFromGoal();
				for (int i = 0; i < ballPosPer.Length; i++)
				{
					ballPosPer[i] = SingletonCustom<FieldManager>.Instance.ConvertLocalPosPer(SingletonCustom<BallManager>.Instance.GetBallPos(), i);
				}
				for (int j = 0; j < distanceFromGoalToBall.Length; j++)
				{
					distanceFromGoalToBall[j] = SingletonCustom<BallManager>.Instance.GetBallDistance(SingletonCustom<FieldManager>.Instance.GetMyGoal(j).position);
				}
			}
			else
			{
				updataInterval -= Time.deltaTime;
			}
		}
		private void UpdateCharaDistribution()
		{
			for (int i = 0; i < SingletonCustom<MainCharacterManager>.Instance.CharaList.Length; i++)
			{
				for (int j = 0; j < charaDistribution[i].charaDistribution.GetLength(0); j++)
				{
					for (int k = 0; k < charaDistribution[i].charaDistribution.GetLength(1); k++)
					{
						charaDistribution[i].charaDistribution[j, k] = 0;
					}
				}
				for (int l = 0; l < SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas.Length; l++)
				{
					Vector3 vector = SingletonCustom<FieldManager>.Instance.ConvertLocalPosPer(SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[l].GetPos(), 0);
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
			for (int i = 0; i < SingletonCustom<MainCharacterManager>.Instance.CharaList.Length; i++)
			{
				distanceFromBallOrder[i].Clear();
				for (int j = 1; j < SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas.Length; j++)
				{
					num = SingletonCustom<BallManager>.Instance.GetBallDistance(SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].GetPos());
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
				num = SingletonCustom<BallManager>.Instance.GetBallDistance(SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[0].GetPos());
				distanceFromBall[i, 0] = num;
				distanceFromBallOrder[i].Add(new CharacterData(0, num));
			}
		}
		private void CalcDistanceFromGoal()
		{
			for (int i = 0; i < SingletonCustom<MainCharacterManager>.Instance.CharaList.Length; i++)
			{
				for (int j = 0; j < SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas.Length; j++)
				{
					distanceFromMyGoal[i, j] = CalcManager.Length(SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].GetPos(), SingletonCustom<FieldManager>.Instance.GetMyGoal(i).position);
					distanceFromOpponentGoal[i, j] = CalcManager.Length(SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].GetPos(), SingletonCustom<FieldManager>.Instance.GetOpponentGoal(i).position);
				}
			}
		}
		public void UpdateRestrictedAreaData()
		{
			for (int i = 0; i < restrictedArea.Length; i++)
			{
				restrictedArea[i].type = RestrictedAreaType.NONE;
				switch (SingletonCustom<BallManager>.Instance.GetBallState())
				{
				case BallManager.BallState.KEEP:
					if (SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().CheckPositionType(GameDataParams.PositionType.GK))
					{
						if (SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().TeamNo != i)
						{
							if (SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().IsBallCatch)
							{
								restrictedArea[i].type = RestrictedAreaType.KEEPER_KEEP;
								restrictedArea[i].pos = SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().GetPos();
								restrictedArea[i].size = 5f;
							}
						}
						else
						{
							restrictedArea[i].type = RestrictedAreaType.KEEPER_KEEP;
							restrictedArea[i].pos = SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().GetPos();
							restrictedArea[i].size = 4f;
						}
					}
					else if (SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().TeamNo == i)
					{
						restrictedArea[i].type = RestrictedAreaType.ALLY_KEEP;
						restrictedArea[i].pos = SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().GetPos();
						restrictedArea[i].size = CharacterScript.NEAR_DISTANCE;
					}
					break;
				case BallManager.BallState.CORNER_KICK:
					if (SingletonCustom<MainGameManager>.Instance.GetSetPlayTeamNo() != i && SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara() != null)
					{
						restrictedArea[i].type = RestrictedAreaType.CORNER_KICK;
						restrictedArea[i].pos = SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().GetPos();
						restrictedArea[i].size = 5f;
					}
					break;
				case BallManager.BallState.GOAL_KICK:
					if (SingletonCustom<MainGameManager>.Instance.GetSetPlayTeamNo() != i)
					{
						restrictedArea[i].type = RestrictedAreaType.GOAL_KICK;
						restrictedArea[i].pos = SingletonCustom<FieldManager>.Instance.GetOpponentGoal(i).position;
						restrictedArea[i].size = SingletonCustom<FieldManager>.Instance.GetFieldData().penaltyAreaSize.z * 2f;
					}
					break;
				}
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
		public float GetDistanceFromBall(int _teamNo, int _charaNo)
		{
			return distanceFromBall[_teamNo, _charaNo];
		}
		public float GetDistanceFromMyGoal(int _teamNo, int _charaNo)
		{
			return distanceFromMyGoal[_teamNo, _charaNo];
		}
		public float GetDistanceFromOpponentGoal(int _teamNo, int _charaNo)
		{
			return distanceFromOpponentGoal[_teamNo, _charaNo];
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
			Vector3 vector = SingletonCustom<FieldManager>.Instance.ConvertLocalPosPer(_centerPos, 0);
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
			int index = Random.Range(0, list.Count);
			Vector3 position = SingletonCustom<FieldManager>.Instance.GetAnchors().front.position;
			position.x -= charaDistributionSplitSize.x * (float)DISTRIBUTION_SPLIT_NUM_X * 0.5f;
			position.x += charaDistributionSplitSize.x * ((float)list[index][0] + 0.5f);
			position.z += charaDistributionSplitSize.y * ((float)list[index][1] + 0.5f);
			return position;
		}
	}
}
