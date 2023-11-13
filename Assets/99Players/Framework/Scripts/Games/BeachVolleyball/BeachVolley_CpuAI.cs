using SaveDataDefine;
using System;
using UnityEngine;
public class BeachVolley_CpuAI
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
	public struct ServeData
	{
		public int jumpPer;
		public RandomSeed<float> targetRangeSide;
		public RandomSeed<float> targetRangeDepth;
		public RandomSeed<float> power;
		public int changePosPer;
		public ServeData(int _jumpPer, RandomSeed<float> _targetRangeSide, RandomSeed<float> _targetRangeDepth, RandomSeed<float> _power, int _changePosPer)
		{
			jumpPer = _jumpPer;
			targetRangeSide = _targetRangeSide;
			targetRangeDepth = _targetRangeDepth;
			power = _power;
			changePosPer = _changePosPer;
		}
		public void Copy(ServeData _data)
		{
			jumpPer = _data.jumpPer;
			targetRangeSide = _data.targetRangeSide;
			targetRangeDepth = _data.targetRangeDepth;
			power = _data.power;
			changePosPer = _data.changePosPer;
		}
	}
	public struct RecieveData
	{
		public int[] delayPer;
		public int[] missPer;
		public int[] divingPer;
		public RandomSeed<float> delayTime;
		public RandomSeed<float> missDelayTime;
		public RandomSeed<float> tossPower;
		public int twoAttackPer;
		public float backAttackPer;
		public RecieveData(int[] _delayPer, int[] _missPer, int[] _divingPer, RandomSeed<float> _delayTime, RandomSeed<float> _missDelayTime, RandomSeed<float> _tossPower, int _twoAttackPer, float _backAttackPer)
		{
			delayPer = _delayPer;
			missPer = _missPer;
			divingPer = _divingPer;
			delayTime = _delayTime;
			missDelayTime = _missDelayTime;
			tossPower = _tossPower;
			twoAttackPer = _twoAttackPer;
			backAttackPer = _backAttackPer;
		}
		public void Copy(RecieveData _data)
		{
			delayPer = _data.delayPer;
			missPer = _data.missPer;
			divingPer = _data.divingPer;
			delayTime = _data.delayTime;
			missDelayTime = _data.missDelayTime;
			tossPower = _data.tossPower;
			twoAttackPer = _data.twoAttackPer;
			backAttackPer = _data.backAttackPer;
		}
	}
	public struct AttackData
	{
		public Vector2 attackTargetRange;
		public RandomSeed<float> spikeTargetRangeSide;
		public RandomSeed<float> spikeTargetRangeDepth;
		public int delayPer;
		public int missPer;
		public int overPer;
		public RandomSeed<float> delayTime;
		public RandomSeed<float> spikePower;
		public AttackData(Vector2 _attackTargetRange, RandomSeed<float> _spikeTargetRangeSide, RandomSeed<float> _spikeTargetRangeDepth, int _delayPer, int _missPer, int _overPer, RandomSeed<float> _delayTime, RandomSeed<float> _spikePower)
		{
			attackTargetRange = _attackTargetRange;
			spikeTargetRangeSide = _spikeTargetRangeSide;
			spikeTargetRangeDepth = _spikeTargetRangeDepth;
			delayPer = _delayPer;
			missPer = _missPer;
			overPer = _overPer;
			delayTime = _delayTime;
			spikePower = _spikePower;
		}
		public void Copy(AttackData _data)
		{
			attackTargetRange = _data.attackTargetRange;
			spikeTargetRangeSide = _data.spikeTargetRangeSide;
			spikeTargetRangeDepth = _data.spikeTargetRangeDepth;
			delayPer = _data.delayPer;
			missPer = _data.missPer;
			overPer = _data.overPer;
			delayTime = _data.delayTime;
			spikePower = _data.spikePower;
		}
	}
	public struct BlockData
	{
		public int delayPer;
		public int missPer;
		public RandomSeed<float> delayTime;
		public Vector2 targetRange;
		public RandomSeed<float> power;
		public BlockData(int _delayPer, int _missPer, RandomSeed<float> _delayTime, Vector2 _targetRange, RandomSeed<float> _power)
		{
			delayPer = _delayPer;
			missPer = _missPer;
			delayTime = _delayTime;
			targetRange = _targetRange;
			power = _power;
		}
		public void Copy(BlockData _data)
		{
			delayPer = _data.delayPer;
			missPer = _data.missPer;
			delayTime = _data.delayTime;
			targetRange = _data.targetRange;
			power = _data.power;
		}
	}
	public struct StrengthParams
	{
		public RandomSeed<float> ballSearchInterval;
		public ServeData serveData;
		public RecieveData recieveData;
		public AttackData attackData;
		public BlockData blockData;
		public StrengthParams(RandomSeed<float> _ballSearchInterval, ServeData _serveData, RecieveData _recieveData, AttackData _attackData, BlockData _blockData)
		{
			ballSearchInterval = _ballSearchInterval;
			serveData = _serveData;
			recieveData = _recieveData;
			attackData = _attackData;
			blockData = _blockData;
		}
		public void Copy(StrengthParams _data)
		{
			ballSearchInterval = _data.ballSearchInterval;
			serveData = _data.serveData;
			recieveData = _data.recieveData;
			attackData = _data.attackData;
			blockData = _data.blockData;
		}
	}
	private StrengthParams[] strengthParamsList = new StrengthParams[3]
	{
		new StrengthParams(new RandomSeed<float>(0.8f, 1f), new ServeData(0, new RandomSeed<float>(-0.25f, 0.25f), new RandomSeed<float>(0.5f, 0.75f), new RandomSeed<float>(0f, 0.3f), 0), new RecieveData(new int[3]
		{
			100,
			30,
			10
		}, new int[3]
		{
			50,
			10,
			10
		}, new int[3]
		{
			0,
			0,
			100
		}, new RandomSeed<float>(0.4f, 1f), new RandomSeed<float>(0.75f, 1f), new RandomSeed<float>(1f, 1f), 0, 0f), new AttackData(new Vector2(0.1f, 0.1f), new RandomSeed<float>(-0.25f, 0.25f), new RandomSeed<float>(0.5f, 1f), 80, 50, 20, new RandomSeed<float>(0f, 0.35f), new RandomSeed<float>(0f, 0f)), new BlockData(80, 80, new RandomSeed<float>(0.35f, 1f), new Vector2(0.1f, 0.1f), new RandomSeed<float>(0f, 0f))),
		new StrengthParams(new RandomSeed<float>(0.5f, 1f), new ServeData(0, new RandomSeed<float>(-0.25f, 0.25f), new RandomSeed<float>(0.5f, 0.75f), new RandomSeed<float>(0f, 0.5f), 0), new RecieveData(new int[3]
		{
			100,
			30,
			10
		}, new int[3]
		{
			60,
			10,
			30
		}, new int[3]
		{
			0,
			0,
			100
		}, new RandomSeed<float>(0.1f, 0.4f), new RandomSeed<float>(0.5f, 1f), new RandomSeed<float>(0.75f, 1f), 0, 0f), new AttackData(new Vector2(0.1f, 0.1f), new RandomSeed<float>(-0.25f, 0.25f), new RandomSeed<float>(0.5f, 1f), 80, 40, 20, new RandomSeed<float>(0f, 0.35f), new RandomSeed<float>(0f, 0.05f)), new BlockData(80, 80, new RandomSeed<float>(0.3f, 1f), new Vector2(0.1f, 0.1f), new RandomSeed<float>(0f, 0f))),
		new StrengthParams(new RandomSeed<float>(0.25f, 0.5f), new ServeData(50, new RandomSeed<float>(-0.75f, 0.75f), new RandomSeed<float>(0f, 0.9f), new RandomSeed<float>(0.5f, 1f), 0), new RecieveData(new int[3]
		{
			20,
			10,
			5
		}, new int[3]
		{
			10,
			0,
			5
		}, new int[3]
		{
			50,
			70,
			100
		}, new RandomSeed<float>(0f, 0.2f), new RandomSeed<float>(0.5f, 0.75f), new RandomSeed<float>(0.5f, 1f), 5, 10f), new AttackData(new Vector2(0.5f, 0.5f), new RandomSeed<float>(-0.75f, 0.75f), new RandomSeed<float>(-0.5f, 1f), 30, 15, 10, new RandomSeed<float>(0f, 0.3f), new RandomSeed<float>(0.25f, 0.5f)), new BlockData(50, 50, new RandomSeed<float>(0.2f, 0.75f), new Vector2(0.3f, 0.3f), new RandomSeed<float>(0f, 0.5f)))
	};
	private StrengthParams tutorialParam = new StrengthParams(new RandomSeed<float>(0f, 0.5f), new ServeData(0, new RandomSeed<float>(0f, 0f), new RandomSeed<float>(0.75f, 0.75f), new RandomSeed<float>(0f, 0.2f), 0), new RecieveData(new int[3]
	{
		100,
		0,
		0
	}, new int[3]
	{
		100,
		0,
		0
	}, new int[3], new RandomSeed<float>(0.5f, 0.5f), new RandomSeed<float>(0.75f, 0.75f), new RandomSeed<float>(1f, 1f), 0, 0f), new AttackData(new Vector2(1f, 1f), new RandomSeed<float>(1f, 1f), new RandomSeed<float>(0f, 0f), 0, 0, 0, new RandomSeed<float>(0f, 0.25f), new RandomSeed<float>(0.5f, 1f)), new BlockData(10, 0, new RandomSeed<float>(0.2f, 0.5f), new Vector2(1f, 0.5f), new RandomSeed<float>(0.5f, 1f)));
	private StrengthParams[] strengthParams = new StrengthParams[2];
	private BeachVolley_Define.AiTacticsType[] aiTacticsType = new BeachVolley_Define.AiTacticsType[2];
	private SystemData.AiStrength[] aiStrength = new SystemData.AiStrength[2]
	{
		SystemData.AiStrength.WEAK,
		SystemData.AiStrength.STRONG
	};
	public BeachVolley_Ball Ball => SingletonCustom<BeachVolley_BallManager>.Instance.GetBall();
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
	public bool CheckJumpServe(int _teamNo)
	{
		return CalcManager.IsPerCheck(strengthParams[_teamNo].serveData.jumpPer);
	}
	public Vector3 GetServeVec(int _teamNo)
	{
		Vector3 normalized = default(Vector3);
		normalized.x = RandomValue(strengthParams[_teamNo].serveData.targetRangeSide);
		normalized.y = 0f;
		normalized.z = RandomValue(strengthParams[_teamNo].serveData.targetRangeDepth);
		normalized = normalized.normalized;
		return normalized;
	}
	public float GetServePower(int _teamNo)
	{
		return RandomValue(strengthParams[_teamNo].serveData.power);
	}
	public bool CheckChangeServePos(int _teamNo)
	{
		return CalcManager.IsPerCheck(strengthParams[_teamNo].serveData.jumpPer);
	}
	public bool CheckRecieveDelayPer(int _teamNo, bool _recieve, bool _serve)
	{
		if (_serve)
		{
			return CalcManager.IsPerCheck(strengthParams[_teamNo].recieveData.delayPer[2]);
		}
		if (_recieve)
		{
			return CalcManager.IsPerCheck(strengthParams[_teamNo].recieveData.delayPer[0]);
		}
		return CalcManager.IsPerCheck(strengthParams[_teamNo].recieveData.delayPer[1]);
	}
	public float GetRecieveDelayTime(int _teamNo)
	{
		return RandomValue(strengthParams[_teamNo].recieveData.delayTime);
	}
	public float GetTossPower(int _teamNo)
	{
		return RandomValue(strengthParams[_teamNo].recieveData.tossPower);
	}
	public bool CheckRecieveDivingPer(int _teamNo)
	{
		return CalcManager.IsPerCheck(strengthParams[_teamNo].recieveData.divingPer[0]);
	}
	public bool CheckRecieveMissPer(int _teamNo, bool _recieve, bool _serve)
	{
		if (_serve)
		{
			return CalcManager.IsPerCheck(strengthParams[_teamNo].recieveData.missPer[2]);
		}
		if (_recieve)
		{
			return CalcManager.IsPerCheck(strengthParams[_teamNo].recieveData.missPer[0]);
		}
		return CalcManager.IsPerCheck(strengthParams[_teamNo].recieveData.missPer[1]);
	}
	public float GetRecieveDelayMissTime(int _teamNo)
	{
		return RandomValue(strengthParams[_teamNo].recieveData.missDelayTime);
	}
	public bool CheckBackAttackPer(int _teamNo)
	{
		return CalcManager.IsPerCheck(strengthParams[_teamNo].recieveData.backAttackPer);
	}
	public bool CheckTwoAttackPer(int _teamNo)
	{
		return CalcManager.IsPerCheck(strengthParams[_teamNo].recieveData.twoAttackPer);
	}
	public Vector3 GetAttackVec(int _teamNo)
	{
		Vector3 vector = strengthParams[_teamNo].attackData.attackTargetRange;
		vector.x = UnityEngine.Random.Range(0f, Mathf.Abs(vector.x)) * CalcManager.RandomPlusOrMinus();
		vector.y = 0f;
		vector.z = UnityEngine.Random.Range(0f, Mathf.Abs(vector.y)) * CalcManager.RandomPlusOrMinus();
		vector = vector.normalized;
		return vector;
	}
	public Vector3 GetSpikeVec(int _teamNo)
	{
		Vector3 normalized = default(Vector3);
		normalized.x = RandomValue(strengthParams[_teamNo].attackData.spikeTargetRangeSide);
		normalized.y = 0f;
		normalized.z = RandomValue(strengthParams[_teamNo].attackData.spikeTargetRangeDepth);
		normalized = normalized.normalized;
		return normalized;
	}
	public float GetSpikePower(int _teamNo)
	{
		return RandomValue(strengthParams[_teamNo].attackData.spikePower);
	}
	public bool CheckSpikeOverPer(int _teamNo)
	{
		return CalcManager.IsPerCheck(strengthParams[_teamNo].attackData.overPer);
	}
	public bool CheckSpikeMissPer(int _teamNo)
	{
		return CalcManager.IsPerCheck(strengthParams[_teamNo].attackData.missPer);
	}
	public bool CheckSpikeDelayPer(int _teamNo)
	{
		return CalcManager.IsPerCheck(strengthParams[_teamNo].attackData.delayPer);
	}
	public float GetSpikeDelayTime(int _teamNo)
	{
		return RandomValue(strengthParams[_teamNo].attackData.delayTime);
	}
	public float GetSpikeDelayMissTime(int _teamNo)
	{
		return strengthParams[_teamNo].attackData.delayTime.maxValue;
	}
	public bool CheckBlockDelayPer(int _teamNo)
	{
		return CalcManager.IsPerCheck(strengthParams[_teamNo].blockData.delayPer);
	}
	public float GetBlockDelayTime(int _teamNo)
	{
		return RandomValue(strengthParams[_teamNo].blockData.delayTime);
	}
	public bool CheckBlockMissPer(int _teamNo)
	{
		return CalcManager.IsPerCheck(strengthParams[_teamNo].blockData.missPer);
	}
	public float GetBlockDelayMissTime(int _teamNo)
	{
		return strengthParams[_teamNo].blockData.delayTime.maxValue;
	}
	public Vector3 GetBlockVec(int _teamNo)
	{
		Vector3 vector = strengthParams[_teamNo].blockData.targetRange;
		vector.x = UnityEngine.Random.Range(0f, Mathf.Abs(vector.x)) * CalcManager.RandomPlusOrMinus();
		vector.y = 0f;
		vector.y = UnityEngine.Random.Range(0f, Mathf.Abs(vector.y)) * CalcManager.RandomPlusOrMinus();
		vector = vector.normalized;
		return vector;
	}
	public float GetBlockPower(int _teamNo)
	{
		return RandomValue(strengthParams[_teamNo].blockData.power);
	}
	public void Init()
	{
		for (int i = 0; i < aiStrength.Length; i++)
		{
			if (BeachVolley_Define.MCM.teamUserList[i][0] <= 3)
			{
				aiTacticsType[i] = BeachVolley_Define.AiTacticsType.BALANCE;
				aiStrength[i] = SystemData.AiStrength.NORAML;
				UnityEngine.Debug.Log("プレイヤ\u30fcがいるチ\u30fcム");
			}
			else
			{
				aiTacticsType[i] = BeachVolley_Define.AiTacticsType.BALANCE;
				aiStrength[i] = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting();
				UnityEngine.Debug.Log("CPUのみのチ\u30fcム");
			}
			UnityEngine.Debug.Log("チ\u30fcム + " + i.ToString() + " : 強さ = " + aiStrength[i].ToString() + " : タイプ = " + aiTacticsType[i].ToString());
			strengthParams[i].Copy(strengthParamsList[(int)aiStrength[i]]);
			switch (aiTacticsType[i])
			{
			case BeachVolley_Define.AiTacticsType.BALANCE:
				strengthParams[i].attackData.delayPer -= 5;
				strengthParams[i].attackData.delayPer = Mathf.Max(strengthParams[i].attackData.delayPer, 0);
				for (int k = 0; k < strengthParams[i].recieveData.delayPer.Length; k++)
				{
					strengthParams[i].recieveData.delayPer[k] -= 5;
					strengthParams[i].recieveData.delayPer[k] = Mathf.Max(strengthParams[i].recieveData.delayPer[k], 0);
				}
				strengthParams[i].blockData.delayPer -= 5;
				strengthParams[i].blockData.delayPer = Mathf.Max(strengthParams[i].blockData.delayPer, 0);
				break;
			case BeachVolley_Define.AiTacticsType.OFFENSIVE:
				strengthParams[i].attackData.missPer -= 5;
				strengthParams[i].attackData.missPer = Mathf.Max(strengthParams[i].attackData.missPer, 0);
				strengthParams[i].attackData.overPer -= 5;
				strengthParams[i].attackData.overPer = Mathf.Max(strengthParams[i].attackData.overPer, 0);
				strengthParams[i].attackData.delayPer -= 5;
				strengthParams[i].attackData.delayPer = Mathf.Max(strengthParams[i].attackData.delayPer, 0);
				strengthParams[i].serveData.jumpPer += 5;
				strengthParams[i].serveData.jumpPer = Mathf.Min(strengthParams[i].serveData.jumpPer, 90);
				break;
			case BeachVolley_Define.AiTacticsType.PRUDENCE:
				strengthParams[i].attackData.missPer -= 5;
				strengthParams[i].attackData.missPer = Mathf.Max(strengthParams[i].attackData.missPer, 0);
				strengthParams[i].attackData.overPer -= 5;
				strengthParams[i].attackData.overPer = Mathf.Max(strengthParams[i].attackData.overPer, 0);
				for (int j = 0; j < strengthParams[i].recieveData.delayPer.Length; j++)
				{
					strengthParams[i].recieveData.delayPer[j] -= 5;
					strengthParams[i].recieveData.delayPer[j] = Mathf.Max(strengthParams[i].recieveData.delayPer[j], 0);
				}
				strengthParams[i].recieveData.twoAttackPer -= 5;
				strengthParams[i].recieveData.twoAttackPer = Mathf.Max(strengthParams[i].recieveData.twoAttackPer, 0);
				break;
			case BeachVolley_Define.AiTacticsType.TECHNIC:
				strengthParams[i].recieveData.twoAttackPer += 5;
				strengthParams[i].recieveData.twoAttackPer = Mathf.Min(strengthParams[i].recieveData.twoAttackPer, 90);
				strengthParams[i].recieveData.divingPer[0] += 5;
				strengthParams[i].recieveData.divingPer[0] = Mathf.Min(strengthParams[i].recieveData.divingPer[0], 90);
				strengthParams[i].blockData.delayPer -= 5;
				strengthParams[i].blockData.delayPer = Mathf.Max(strengthParams[i].blockData.delayPer, 0);
				break;
			}
		}
	}
	public void UpdateAutoAction(int _teamNo, int _charaNo)
	{
		if (!GetChara(_teamNo, _charaNo).CheckActionState(BeachVolley_Character.ActionState.STANDARD) || _charaNo == BeachVolley_Define.MCM.GetBenchCharaIndex())
		{
			return;
		}
		if (BeachVolley_Define.MGM.CheckGameState(BeachVolley_MainGameManager.GameState.IN_PLAY))
		{
			if (BeachVolley_Define.MCM.BallControllTeam == _teamNo)
			{
				SettingOffenseAi(_teamNo, _charaNo);
			}
			else
			{
				SettingDefenseAi(_teamNo, _charaNo);
			}
		}
		else
		{
			GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiMoveFormationPosition, _immediate: true, _forcibly: true);
		}
	}
	private void SettingOffenseAi(int _teamNo, int _charaNo)
	{
		if (GetChara(_teamNo, _charaNo).playerNo >= 0)
		{
			if (!GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiHandleBall))
			{
				GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiHandleBall, _immediate: true, _forcibly: true);
			}
		}
		else if (!GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiAttackMovePosition))
		{
			GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiAttackMovePosition, _immediate: true, _forcibly: true);
		}
	}
	private void SettingDefenseAi(int _teamNo, int _charaNo)
	{
		if (GetChara(_teamNo, _charaNo).playerNo >= 0)
		{
			if (!GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiBlockStandby))
			{
				GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiBlockStandby, _immediate: true, _forcibly: true);
			}
		}
		else if (!GetChara(_teamNo, _charaNo).CheckAiAction(GetChara(_teamNo, _charaNo).AiMoveBlockPosition))
		{
			GetChara(_teamNo, _charaNo).SetAction(GetChara(_teamNo, _charaNo).AiMoveBlockPosition, _immediate: true, _forcibly: true);
		}
	}
	public BeachVolley_Character GetChara(int _teamNo, int _charaNo)
	{
		return SingletonCustom<BeachVolley_MainCharacterManager>.Instance.TeamList[_teamNo].charas[_charaNo];
	}
}
