using System;
using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class CpuArtificialIntelligence
	{
		public struct ValueLimit<T>
		{
			public T min;
			public T max;
			public ValueLimit(T _min, T _max)
			{
				min = _min;
				max = _max;
			}
		}
		public struct RandomSeed<T>
		{
			public T minValue;
			public T maxValue;
			public RandomSeed(T _value1, T _value2, bool _mirror = false)
			{
				if (_mirror)
				{
					if (typeof(T) == typeof(int))
					{
						minValue = (T)Convert.ChangeType((int)Convert.ChangeType(_value1, typeof(int)) - (int)Convert.ChangeType(_value2, typeof(int)), typeof(T));
						maxValue = (T)Convert.ChangeType((int)Convert.ChangeType(_value1, typeof(int)) + (int)Convert.ChangeType(_value2, typeof(int)), typeof(T));
					}
					else if (typeof(T) == typeof(float))
					{
						minValue = (T)Convert.ChangeType((float)Convert.ChangeType(_value1, typeof(float)) - (float)Convert.ChangeType(_value2, typeof(float)), typeof(T));
						maxValue = (T)Convert.ChangeType((float)Convert.ChangeType(_value1, typeof(float)) + (float)Convert.ChangeType(_value2, typeof(float)), typeof(T));
					}
					else
					{
						minValue = _value1;
						maxValue = _value2;
					}
				}
				else
				{
					minValue = _value1;
					maxValue = _value2;
				}
			}
		}
		public struct StrengthParams
		{
			public RandomSeed<float> ballSearchInterval;
			public float stealPerMag;
			public float stolenPerMag;
			public float runSpeed;
			public int passPerWhenNoStamina;
			public int shootPerWhenInPenalty;
			public int shootPerFrontKeeper;
			public int overShootPer;
			public int shootMissPer;
			public int unconditionalPassPer;
			public int passPerWhenFrontOpponent;
			public int shootPerWhenFrontGoal;
			public int divingDelayPer;
			public int divingDelayPerWhenFastBall;
			public int freezePerWhenFastBall;
			public int divingMissPer;
			public float keeperFreezeTimeCorr;
			public StrengthParams(RandomSeed<float> _ballSearchInterval, float _stealPerMag, float _stolenPerMag, float _runSpeed, int _passPerWhenNoStamina, int _shootPerWhenInPenalty, int _shootPerFrontKeeper, int _overShootPer, int _shootMissPer, int _unconditionalPassPer, int _passPerWhenFrontOpponent, int _shootPerWhenFrontGoal, int _divingDelayPer, int _divingDelayPerWhenFastBall, int _freezePerWhenFastBall, int _divingMissPer, float _keeperFreezeTimeCorr)
			{
				ballSearchInterval = _ballSearchInterval;
				stealPerMag = _stealPerMag;
				stolenPerMag = _stolenPerMag;
				runSpeed = _runSpeed;
				passPerWhenNoStamina = _passPerWhenNoStamina;
				shootPerWhenInPenalty = _shootPerWhenInPenalty;
				shootPerFrontKeeper = _shootPerFrontKeeper;
				overShootPer = _overShootPer;
				shootMissPer = _shootMissPer;
				unconditionalPassPer = _unconditionalPassPer;
				passPerWhenFrontOpponent = _passPerWhenFrontOpponent;
				shootPerWhenFrontGoal = _shootPerWhenFrontGoal;
				divingDelayPer = _divingDelayPer;
				divingDelayPerWhenFastBall = _divingDelayPerWhenFastBall;
				freezePerWhenFastBall = _freezePerWhenFastBall;
				divingMissPer = _divingMissPer;
				keeperFreezeTimeCorr = _keeperFreezeTimeCorr;
			}
			public void Copy(StrengthParams _data)
			{
				ballSearchInterval = _data.ballSearchInterval;
				stealPerMag = _data.stealPerMag;
				stolenPerMag = _data.stolenPerMag;
				runSpeed = _data.runSpeed;
				passPerWhenNoStamina = _data.passPerWhenNoStamina;
				shootPerWhenInPenalty = _data.shootPerWhenInPenalty;
				shootPerFrontKeeper = _data.shootPerFrontKeeper;
				shootMissPer = _data.shootMissPer;
				overShootPer = _data.overShootPer;
				unconditionalPassPer = _data.unconditionalPassPer;
				passPerWhenFrontOpponent = _data.passPerWhenFrontOpponent;
				shootPerWhenFrontGoal = _data.shootPerWhenFrontGoal;
				divingDelayPer = _data.divingDelayPer;
				divingDelayPerWhenFastBall = _data.divingDelayPerWhenFastBall;
				freezePerWhenFastBall = _data.freezePerWhenFastBall;
				divingMissPer = _data.divingMissPer;
				keeperFreezeTimeCorr = _data.keeperFreezeTimeCorr;
			}
		}
		private StrengthParams[] strengthParamsList = new StrengthParams[5]
		{
			new StrengthParams(new RandomSeed<float>(0.8f, 1f), 0.3f, 1.5f, 0.05f, 10, 5, 50, 50, 30, 0, 10, 20, 50, 80, 50, 80, 0f),
			new StrengthParams(new RandomSeed<float>(0.7f, 0.9f), 0.325f, 1.4f, 0.05f, 15, 5, 30, 30, 20, 0, 15, 30, 50, 50, 50, 50, 0f),
			new StrengthParams(new RandomSeed<float>(0.5f, 0.7f), 0.35f, 1.3f, 0.05f, 20, 10, 20, 20, 10, 0, 20, 40, 30, 30, 30, 30, 0.25f),
			new StrengthParams(new RandomSeed<float>(0.2f, 0.3f), 1f, 1f, 0.25f, 25, 20, 10, 10, 5, 10, 25, 50, 5, 10, 5, 5, 0.5f),
			new StrengthParams(new RandomSeed<float>(0f, 0.2f), 1.5f, 0.5f, 0.4f, 30, 25, 5, 5, 0, 15, 30, 80, 0, 0, 0, 0, 1f)
		};
		private StrengthParams[] strengthParams = new StrengthParams[2];
		private GameDataParams.AiTacticsType[] aiTacticsType = new GameDataParams.AiTacticsType[2];
		private GameDataParams.AiStrength[] aiStrength = new GameDataParams.AiStrength[2]
		{
			GameDataParams.AiStrength.NOOB,
			GameDataParams.AiStrength.VERY_STRONG
		};
		public MainCharacterManager CM => SingletonCustom<MainCharacterManager>.Instance;
		public BallManager BM => SingletonCustom<BallManager>.Instance;
		public BallScript Ball => SingletonCustom<BallManager>.Instance.GetBall();
		public MainCharacterManager.CharacterList[] CharaList => SingletonCustom<MainCharacterManager>.Instance.CharaList;
		public float RandomValue(RandomSeed<float> _seed)
		{
			return UnityEngine.Random.Range(_seed.minValue, _seed.maxValue);
		}
		public int RandomValue(RandomSeed<int> _seed)
		{
			return UnityEngine.Random.Range(_seed.minValue, _seed.maxValue);
		}
		private RandomSeed<float> RandomValue(RandomSeed<float> _seed, float _mag)
		{
			_seed.minValue *= _mag;
			_seed.maxValue *= _mag;
			return _seed;
		}
		private RandomSeed<int> RandomValue(RandomSeed<int> _seed, float _mag)
		{
			_seed.minValue = (int)((float)_seed.minValue * _mag);
			_seed.maxValue = (int)((float)_seed.maxValue * _mag);
			return _seed;
		}
		private int LimitValue(int _value, int _min, int _max)
		{
			return Mathf.Min(Mathf.Max(_value, _min), _max);
		}
		private float LimitValue(float _value, float _min, float _max)
		{
			return Mathf.Min(Mathf.Max(_value, _min), _max);
		}
		public float GetBallSearchInterval(int _teamNo)
		{
			return RandomValue(strengthParams[_teamNo].ballSearchInterval);
		}
		public float GetStealPerMag(int _teamNo)
		{
			return strengthParams[_teamNo].stealPerMag;
		}
		public float GetStolenPerMag(int _teamNo)
		{
			return strengthParams[_teamNo].stolenPerMag;
		}
		public float GetRunSpeed(int _teamNo)
		{
			return strengthParams[_teamNo].runSpeed;
		}
		public int GetPassPerWhenNoStamina(int _teamNo)
		{
			return strengthParams[_teamNo].passPerWhenNoStamina;
		}
		public int GetShootPerWhenInPenalty(int _teamNo)
		{
			return strengthParams[_teamNo].shootPerWhenInPenalty;
		}
		public int GetShootPerFrontKeeper(int _teamNo)
		{
			return strengthParams[_teamNo].shootPerFrontKeeper;
		}
		public int GetOverShootPer(int _teamNo)
		{
			return strengthParams[_teamNo].overShootPer;
		}
		public int GetShootMissPer(int _teamNo)
		{
			return strengthParams[_teamNo].shootMissPer;
		}
		public int GetUnconditionalPassPer(int _teamNo)
		{
			return strengthParams[_teamNo].unconditionalPassPer;
		}
		public int GetPassPerWhenFrontOpponent(int _teamNo)
		{
			return strengthParams[_teamNo].passPerWhenFrontOpponent;
		}
		public int GetShootPerWhenFrontGoal(int _teamNo)
		{
			return strengthParams[_teamNo].shootPerWhenFrontGoal;
		}
		public int GetDivingDelayPer(int _teamNo)
		{
			return strengthParams[_teamNo].divingDelayPer;
		}
		public int GetDivingDelayPerWhenFastBall(int _teamNo)
		{
			return strengthParams[_teamNo].divingDelayPerWhenFastBall;
		}
		public int GetFreezePerWhenFastBall(int _teamNo)
		{
			return strengthParams[_teamNo].freezePerWhenFastBall;
		}
		public int GetDivingMissPer(int _teamNo)
		{
			return strengthParams[_teamNo].divingMissPer;
		}
		public float GetKeeperFreezeTimeCorr(int _teamNo)
		{
			return strengthParams[_teamNo].keeperFreezeTimeCorr;
		}
		public void Init()
		{
			List<int>[] selectMultiPlayerList = GameSaveData.GetSelectMultiPlayerList();
			for (int i = 0; i < aiTacticsType.Length; i++)
			{
				bool flag = SingletonCustom<MainCharacterManager>.Instance.IsPlayer(i);
				if (GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.MULTI) && i >= 0 && i < selectMultiPlayerList.Length && selectMultiPlayerList[i].Count <= 0)
				{
					flag = false;
				}
				if (flag)
				{
					aiTacticsType[i] = GameDataParams.AiTacticsType.BALANCE;
					aiStrength[i] = GameDataParams.AiStrength.VERY_STRONG;
					UnityEngine.Debug.Log("チ\u30fcム[" + i.ToString() + "] PLAYER");
				}
				else
				{
					aiTacticsType[i] = (GameDataParams.AiTacticsType)GameSaveData.GetCpuTacticsType();
					aiStrength[i] = (GameDataParams.AiStrength)GameSaveData.GetCpuStrength();
					UnityEngine.Debug.Log("チ\u30fcム[" + i.ToString() + "] CPU");
				}
				UnityEngine.Debug.Log("チ\u30fcム + " + i.ToString() + " : 強さ = " + aiStrength[i].ToString() + " : タイプ = " + aiTacticsType[i].ToString());
				strengthParams[i].Copy(strengthParamsList[(int)aiStrength[i]]);
				switch (aiTacticsType[i])
				{
				case GameDataParams.AiTacticsType.OFFENSIVE:
					strengthParams[i].ballSearchInterval.minValue += 0.2f;
					strengthParams[i].ballSearchInterval.maxValue += 0.2f;
					strengthParams[i].stolenPerMag += 0.2f;
					strengthParams[i].runSpeed += 0.1f;
					strengthParams[i].passPerWhenNoStamina -= 5;
					strengthParams[i].shootPerWhenInPenalty += 5;
					strengthParams[i].shootPerFrontKeeper += 5;
					strengthParams[i].shootPerWhenFrontGoal += 5;
					strengthParams[i].unconditionalPassPer -= 5;
					break;
				case GameDataParams.AiTacticsType.PRUDENCE:
					strengthParams[i].ballSearchInterval.minValue -= 0.2f;
					strengthParams[i].ballSearchInterval.maxValue -= 0.2f;
					strengthParams[i].stolenPerMag -= 0.1f;
					strengthParams[i].shootPerWhenInPenalty -= 5;
					strengthParams[i].passPerWhenNoStamina += 5;
					strengthParams[i].overShootPer -= 5;
					strengthParams[i].passPerWhenFrontOpponent += 5;
					strengthParams[i].shootPerFrontKeeper -= 5;
					break;
				case GameDataParams.AiTacticsType.TECHNIC:
					strengthParams[i].stolenPerMag += 0.2f;
					strengthParams[i].stolenPerMag -= 0.2f;
					strengthParams[i].passPerWhenNoStamina += 5;
					strengthParams[i].shootPerFrontKeeper -= 5;
					strengthParams[i].overShootPer -= 5;
					strengthParams[i].passPerWhenFrontOpponent += 5;
					break;
				}
				if (SingletonCustom<MainCharacterManager>.Instance.IsPlayer(i))
				{
					strengthParams[i].divingDelayPer = 0;
					strengthParams[i].divingDelayPerWhenFastBall = 0;
					strengthParams[i].freezePerWhenFastBall = 0;
					strengthParams[i].divingMissPer = 0;
				}
			}
			UnityEngine.Debug.Log("強さ = " + aiStrength.ToString() + " : 作戦 = " + aiTacticsType.ToString());
		}
		public void UpdateAutoAction(int _teamNo, int _charaNo)
		{
			if (GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiFreeze) || GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiReturnBench) || GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiKickOffStandby) || GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiKickOffStandbyKicker) || GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiMovePos) || GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiThrowInStandby) || GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiThrowIn) || GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiCornerKickStandby) || GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiCornerKick) || !GetChara(_teamNo, _charaNo).CheckActionState(CharacterScript.ActionState.STANDARD))
			{
				return;
			}
			if (GetChara(_teamNo, _charaNo).CheckPositionType(GameDataParams.PositionType.GK))
			{
				if (!GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiGoalKickStandby) && !GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiGoalKick))
				{
					SettingKeeperAi(_teamNo, _charaNo);
				}
			}
			else if (SingletonCustom<MainCharacterManager>.Instance.CheckOpponentAttack(_teamNo))
			{
				SettingDefenseAi(_teamNo, _charaNo);
			}
			else
			{
				SettingOffenseAi(_teamNo, _charaNo);
			}
		}
		private void SettingKeeperAi(int _teamNo, int _charaNo)
		{
			if (CM.CheckHaveBall(CharaList[_teamNo].charas[_charaNo]))
			{
				GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiKeeperPass);
			}
			else
			{
				GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiGoalProtect);
			}
		}
		private void SettingDefenseAi(int _teamNo, int _charaNo)
		{
			if (SingletonCustom<MainGameManager>.Instance.CheckInPlay())
			{
				if (BM.CheckBallState(BallManager.BallState.FREE))
				{
					if (CM.CheckBallNearest(GetChara(_teamNo, _charaNo)))
					{
						GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiRunTowardBall);
					}
					else
					{
						GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiDownPosition);
					}
				}
				else if (CM.GetHaveBallChara().CheckPositionType(GameDataParams.PositionType.GK) && CM.GetHaveBallChara().IsBallCatch)
				{
					GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiReturnPosition);
				}
				else if (CM.CheckBallNearestDefenseChara(GetChara(_teamNo, _charaNo)))
				{
					GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiPutPressure);
				}
				else
				{
					GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiDownPosition);
				}
				return;
			}
			switch (SingletonCustom<BallManager>.Instance.GetBallState())
			{
			case BallManager.BallState.GOAL_KICK:
				GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiReturnPosition);
				break;
			case BallManager.BallState.CORNER_KICK:
				GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiDownPosition);
				break;
			case BallManager.BallState.THROW_IN:
				GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiDownPosition);
				break;
			}
		}
		private void SettingOffenseAi(int _teamNo, int _charaNo)
		{
			if (BM.CheckBallState(BallManager.BallState.FREE))
			{
				if (CM.CheckBallNearest(GetChara(_teamNo, _charaNo)))
				{
					GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiRunTowardBall);
				}
				else
				{
					GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiUpPosition);
				}
			}
			else if (BM.CheckBallState(BallManager.BallState.KEEP))
			{
				if (CM.CheckHaveBall(GetChara(_teamNo, _charaNo)))
				{
					GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiFrontDribble);
				}
				else
				{
					GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiUpPosition);
				}
			}
			else if (!CM.CheckHaveBall(GetChara(_teamNo, _charaNo)))
			{
				switch (SingletonCustom<BallManager>.Instance.GetBallState())
				{
				case BallManager.BallState.GOAL_KICK:
					GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiReturnPosition);
					break;
				case BallManager.BallState.CORNER_KICK:
					GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiUpPosition);
					break;
				case BallManager.BallState.THROW_IN:
					GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiUpPosition);
					break;
				}
			}
		}
		public CharacterScript GetChara(int _teamNo, int _charaNo)
		{
			return SingletonCustom<MainCharacterManager>.Instance.CharaList[_teamNo].charas[_charaNo];
		}
	}
}
