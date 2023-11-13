using SaveDataDefine;
using System;
using UnityEngine;
public class Skijump_CpuArtificialIntelligence
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
		public int startSuccessPer;
		public RandomSeed<float> timingValue;
		public RandomSeed<float> balanceControllInterval;
		public RandomSeed<float> balanceControllTime;
		public StrengthParams(int _startSuccessPer, RandomSeed<float> _timingValue, RandomSeed<float> _balanceControllInterval, RandomSeed<float> _balanceControllTime)
		{
			startSuccessPer = _startSuccessPer;
			timingValue = _timingValue;
			balanceControllInterval = _balanceControllInterval;
			balanceControllTime = _balanceControllTime;
		}
		public void Copy(StrengthParams _data)
		{
			startSuccessPer = _data.startSuccessPer;
			timingValue = _data.timingValue;
			balanceControllInterval = _data.balanceControllInterval;
			balanceControllTime = _data.balanceControllTime;
		}
	}
	private static readonly RandomSeed<float> RANGE_PERFECT_RAITO = new RandomSeed<float>(0.1f, 0.8f);
	private StrengthParams[] strengthParamsList = new StrengthParams[5]
	{
		new StrengthParams(30, new RandomSeed<float>(0.75f, 1f), new RandomSeed<float>(0.7f, 85f), new RandomSeed<float>(0.1f, 0.2f)),
		new StrengthParams(50, new RandomSeed<float>(0.3f, 0.5f), new RandomSeed<float>(0.65f, 0.8f), new RandomSeed<float>(0.1f, 0.3f)),
		new StrengthParams(70, new RandomSeed<float>(0.25f, 0.5f), new RandomSeed<float>(0.5f, 0.75f), new RandomSeed<float>(0.1f, 0.3f)),
		new StrengthParams(80, new RandomSeed<float>(0.1f, 0.35f), new RandomSeed<float>(0.3f, 0.75f), new RandomSeed<float>(0.3f, 0.4f)),
		new StrengthParams(90, new RandomSeed<float>(0.1f, 0.25f), new RandomSeed<float>(0.3f, 0.5f), new RandomSeed<float>(0.3f, 0.4f))
	};
	private StrengthParams strengthParams;
	private Skijump_GameDataParams.AiTacticsType aiTacticsType;
	private Skijump_GameDataParams.AiStrength aiStrength;
	private float timingValue;
	private bool isStartSuccess;
	private float balanceControllInterval;
	private float balanceControllTime;
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
	public void Init()
	{
		aiTacticsType = Skijump_GameDataParams.AiTacticsType.BALANCE;
		aiStrength = Skijump_GameDataParams.AiStrength.COMMON;
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
		{
		case SystemData.AiStrength.WEAK:
			aiStrength = Skijump_GameDataParams.AiStrength.NOOB;
			break;
		case SystemData.AiStrength.NORAML:
			aiStrength = Skijump_GameDataParams.AiStrength.COMMON;
			break;
		case SystemData.AiStrength.STRONG:
			aiStrength = Skijump_GameDataParams.AiStrength.STRONG;
			break;
		}
		UnityEngine.Debug.Log("強さ = " + aiStrength.ToString() + " : タイプ = " + aiTacticsType.ToString());
		strengthParams.Copy(strengthParamsList[(int)aiStrength]);
		switch (aiTacticsType)
		{
		case Skijump_GameDataParams.AiTacticsType.BALANCE:
			break;
		case Skijump_GameDataParams.AiTacticsType.OFFENSIVE:
			strengthParams.startSuccessPer = Mathf.Max(strengthParams.startSuccessPer - 10, 5);
			strengthParams.timingValue.minValue = Mathf.Max(strengthParams.timingValue.minValue - 0.1f, 0f);
			strengthParams.timingValue.maxValue = Mathf.Max(strengthParams.timingValue.maxValue - 0.1f, 0.1f);
			break;
		case Skijump_GameDataParams.AiTacticsType.PRUDENCE:
			strengthParams.startSuccessPer = Mathf.Min(strengthParams.startSuccessPer + 10, 95);
			strengthParams.timingValue.minValue = Mathf.Min(strengthParams.timingValue.minValue + 0.1f, 0.9f);
			strengthParams.timingValue.maxValue = Mathf.Min(strengthParams.timingValue.maxValue + 0.1f, 1f);
			break;
		case Skijump_GameDataParams.AiTacticsType.TECHNIC:
			strengthParams.startSuccessPer = Mathf.Min(strengthParams.startSuccessPer + 5, 95);
			strengthParams.timingValue.minValue = Mathf.Max(strengthParams.timingValue.minValue - 0.1f, 0f);
			strengthParams.timingValue.maxValue = Mathf.Max(strengthParams.timingValue.maxValue - 0.1f, 0.1f);
			break;
		}
	}
	public void UpdateAutoAction(int _teamNo, int _charaNo)
	{
	}
	public void SettingStartData()
	{
	}
	public float GetStartValue()
	{
		return timingValue;
	}
	public void SettingGaugeData()
	{
		timingValue = RandomValue(strengthParams.timingValue);
	}
	public float GetTimingValue()
	{
		return 1f - timingValue;
	}
	public void SettingBalanceControllData()
	{
		balanceControllInterval = RandomValue(strengthParams.balanceControllInterval);
		balanceControllTime = RandomValue(strengthParams.balanceControllTime);
	}
	public float GetBalanceControllInterval()
	{
		return balanceControllInterval;
	}
	public float GetBalanceControllTime()
	{
		return balanceControllTime;
	}
}
