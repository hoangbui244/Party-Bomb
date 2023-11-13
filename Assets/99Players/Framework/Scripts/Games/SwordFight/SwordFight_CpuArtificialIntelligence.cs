using System;
using UnityEngine;
public class SwordFight_CpuArtificialIntelligence
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
		public float runSpeed;
		public float attackInterval;
		public int deffencePer;
		public int deffenceCounterPer;
		public int attackPer_2nd;
		public int attackPer_Last;
		public StrengthParams(float _runSpeed, float _attackInterval, int _deffencePer, int _deffenceCounterPer, int _attackPer_2nd, int _attackPer_Last)
		{
			runSpeed = _runSpeed;
			attackInterval = _attackInterval;
			deffencePer = _deffencePer;
			deffenceCounterPer = _deffenceCounterPer;
			attackPer_2nd = _attackPer_2nd;
			attackPer_Last = _attackPer_Last;
		}
		public void Copy(StrengthParams _data)
		{
			runSpeed = _data.runSpeed;
			attackInterval = _data.attackInterval;
			deffencePer = _data.deffencePer;
			deffenceCounterPer = _data.deffenceCounterPer;
			attackPer_2nd = _data.attackPer_2nd;
			attackPer_Last = _data.attackPer_Last;
		}
	}
	private StrengthParams[] strengthParamsList = new StrengthParams[5]
	{
		new StrengthParams(0.6f, 1f, 0, 0, 0, 0),
		new StrengthParams(0.7f, 0.75f, 20, 30, 40, 0),
		new StrengthParams(0.8f, 0.5f, 40, 50, 60, 40),
		new StrengthParams(0.9f, 0.25f, 60, 80, 80, 60),
		new StrengthParams(1f, 0.1f, 80, 100, 100, 80)
	};
	private StrengthParams strengthParams;
	private SwordFight_Define.AiTacticsType aiTacticsType;
	private SwordFight_Define.AiStrength aiStrength = SwordFight_Define.AiStrength.COMMON;
	private float nextDeffenceCheckTime;
	private const float NEXT_DEFFENCE_CHECK_TIME_CONST = 1f;
	private float currentDeffenceTime;
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
	public SwordFight_Define.AiStrength GetAiStrength()
	{
		return aiStrength;
	}
	public float GetRunSpeed()
	{
		return strengthParams.runSpeed;
	}
	public float GetAttackInterval()
	{
		return strengthParams.attackInterval;
	}
	public int GetDeffencePer()
	{
		return strengthParams.deffencePer;
	}
	public int GetDeffenceCounterPer()
	{
		return strengthParams.deffenceCounterPer;
	}
	public void Init()
	{
		aiTacticsType = SwordFight_Define.AiTacticsType.BALANCE;
		if (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength == 0)
		{
			aiStrength = SwordFight_Define.AiStrength.WEAK;
		}
		else if (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength == 1)
		{
			aiStrength = SwordFight_Define.AiStrength.COMMON;
		}
		else if (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength == 2)
		{
			aiStrength = SwordFight_Define.AiStrength.VERY_STRONG;
		}
		UnityEngine.Debug.Log("強さ = " + aiStrength.ToString() + " : タイプ = " + aiTacticsType.ToString());
		strengthParams.Copy(strengthParamsList[(int)aiStrength]);
		switch (aiTacticsType)
		{
		}
		UnityEngine.Debug.Log("強さ = " + aiStrength.ToString() + " : 作戦 = " + aiTacticsType.ToString());
	}
	public void UpdateAutoAction(int _charaNo)
	{
		if (!SingletonCustom<SwordFight_MainGameManager>.Instance.IsAnimationDebugMode)
		{
			switch (GetChara(_charaNo).GetActionState())
			{
			case SwordFight_CharacterScript.ActionState.DEATH:
				break;
			case SwordFight_CharacterScript.ActionState.STANDARD:
				SetAction_SetMoveToTarget(_charaNo);
				nextDeffenceCheckTime += Time.deltaTime;
				break;
			case SwordFight_CharacterScript.ActionState.MOVE:
				SetAction_Move(_charaNo);
				nextDeffenceCheckTime += Time.deltaTime;
				break;
			case SwordFight_CharacterScript.ActionState.ATTACK:
				SetAction_Attack(_charaNo);
				break;
			case SwordFight_CharacterScript.ActionState.DEFENCE:
				SetAction_Deffence(_charaNo);
				break;
			case SwordFight_CharacterScript.ActionState.FREEZE:
				SetAction_Freeze(_charaNo);
				nextDeffenceCheckTime += Time.deltaTime;
				break;
			case SwordFight_CharacterScript.ActionState.REPEL:
				nextDeffenceCheckTime += Time.deltaTime;
				break;
			}
		}
	}
	private void SetAction_SetMoveToTarget(int _charaNo)
	{
		GetChara(_charaNo).AiSearchTarget();
		if (!(GetChara(_charaNo).GetTargetDistance() > 1.5f) || !(GetChara(_charaNo).GetTargetDistance() < 1.75f))
		{
			return;
		}
		GetChara(_charaNo).LookTargetCharacter();
		if (!GetChara(_charaNo).CheckUseDeffence())
		{
			return;
		}
		switch (aiStrength)
		{
		case SwordFight_Define.AiStrength.NOOB:
		case SwordFight_Define.AiStrength.WEAK:
			break;
		case SwordFight_Define.AiStrength.COMMON:
			if (!GetChara(_charaNo).IsJumpInput && UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
			{
				GetChara(_charaNo).AiDeffence();
			}
			else if (UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
			{
				GetChara(_charaNo).AiJump();
			}
			break;
		case SwordFight_Define.AiStrength.STRONG:
		case SwordFight_Define.AiStrength.VERY_STRONG:
			if (!GetChara(_charaNo).IsJumpInput && UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
			{
				GetChara(_charaNo).AiDeffence();
			}
			else if (UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
			{
				GetChara(_charaNo).AiJump();
			}
			break;
		}
	}
	private void SetAction_Move(int _charaNo)
	{
		GetChara(_charaNo).AiMoveToTarget();
		if (!GetChara(_charaNo).IsTargetCharaActive() || !(GetChara(_charaNo).GetTargetDistance() < 2.05f))
		{
			return;
		}
		GetChara(_charaNo).LookTargetCharacter();
		if (GetChara(_charaNo).CheckUseDeffence())
		{
			switch (aiStrength)
			{
			case SwordFight_Define.AiStrength.NOOB:
			case SwordFight_Define.AiStrength.WEAK:
				if (!GetChara(_charaNo).IsJumpInput && UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
				{
					GetChara(_charaNo).AiDeffence();
					return;
				}
				if (UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
				{
					GetChara(_charaNo).AiJump();
					return;
				}
				break;
			case SwordFight_Define.AiStrength.COMMON:
				if (GetChara(_charaNo).CheckNowPosStatus(SwordFight_CharacterScript.NowPosStatus.CAUTION))
				{
					if (!GetChara(_charaNo).IsJumpInput && UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
					{
						GetChara(_charaNo).AiDeffence();
						return;
					}
					if (UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
					{
						GetChara(_charaNo).AiJump();
						return;
					}
				}
				break;
			case SwordFight_Define.AiStrength.STRONG:
			case SwordFight_Define.AiStrength.VERY_STRONG:
				if (!GetChara(_charaNo).CheckNowPosStatus(SwordFight_CharacterScript.NowPosStatus.CAUTION) && !GetChara(_charaNo).CheckNowPosStatus(SwordFight_CharacterScript.NowPosStatus.DANGER))
				{
					break;
				}
				if (GetChara(_charaNo).GetActionState() == SwordFight_CharacterScript.ActionState.ATTACK)
				{
					if (!GetChara(_charaNo).IsJumpInput && !GetChara(_charaNo).CheckTargetNowPosStatusAdvantage())
					{
						GetChara(_charaNo).AiDeffence();
						return;
					}
					break;
				}
				if (GetChara(_charaNo).IsMoveDown && GetChara(_charaNo).CheckAttackAllow())
				{
					int num = UnityEngine.Random.Range(0, 100);
					GetChara(_charaNo).SetAiContinusAttackData(num < strengthParams.attackPer_2nd, num < strengthParams.attackPer_Last);
				}
				if (!GetChara(_charaNo).IsJumpInput && UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
				{
					GetChara(_charaNo).AiDeffence();
					return;
				}
				if (UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
				{
					GetChara(_charaNo).AiJump();
					return;
				}
				break;
			}
		}
		if (GetChara(_charaNo).IsMoveDown && GetChara(_charaNo).CheckAttackAllow())
		{
			int num2 = UnityEngine.Random.Range(0, 100);
			GetChara(_charaNo).SetAiContinusAttackData(num2 < strengthParams.attackPer_2nd, num2 < strengthParams.attackPer_Last);
		}
	}
	private void SetAction_Attack(int _charaNo)
	{
		if (GetChara(_charaNo).IsTargetCharaActive())
		{
			GetChara(_charaNo).AiAttack();
		}
		else
		{
			GetChara(_charaNo).AiWait();
		}
	}
	private void SetAction_Deffence(int _charaNo)
	{
		SwordFight_Define.AiStrength aiStrength = this.aiStrength;
		if ((uint)(aiStrength - 3) <= 1u && !GetChara(_charaNo).GetTargetChara().CheckTargetAngleCharacter(-45f, 45f) && GetChara(_charaNo).CheckAttackAllow())
		{
			int num = UnityEngine.Random.Range(0, 100);
			GetChara(_charaNo).SetAiContinusAttackData(num < strengthParams.attackPer_2nd, num < strengthParams.attackPer_Last);
			return;
		}
		if (GetChara(_charaNo).GetTargetChara().GetActionState() == SwordFight_CharacterScript.ActionState.REPEL && UnityEngine.Random.Range(0, 100) < strengthParams.deffenceCounterPer)
		{
			int num2 = UnityEngine.Random.Range(0, 100);
			GetChara(_charaNo).SetAiContinusAttackData(num2 < strengthParams.attackPer_2nd, num2 < strengthParams.attackPer_Last);
			return;
		}
		currentDeffenceTime += Time.deltaTime;
		if (currentDeffenceTime > SwordFight_Define.DEFFENCE_TIME)
		{
			GetChara(_charaNo).ResetDeffenceMotion();
			currentDeffenceTime = 0f;
		}
	}
	private void SetAction_Freeze(int _charaNo)
	{
		if (!GetChara(_charaNo).CheckUseDeffence() || !GetChara(_charaNo).IsTargetCharaActive() || !(nextDeffenceCheckTime > 1f))
		{
			return;
		}
		nextDeffenceCheckTime = 0f;
		switch (aiStrength)
		{
		case SwordFight_Define.AiStrength.WEAK:
			if (UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
			{
				GetChara(_charaNo).AiDeffence();
			}
			break;
		case SwordFight_Define.AiStrength.COMMON:
			if (GetChara(_charaNo).CheckNowPosStatus(SwordFight_CharacterScript.NowPosStatus.DANGER) && UnityEngine.Random.Range(0, 100) < strengthParams.deffencePer)
			{
				GetChara(_charaNo).AiDeffence();
			}
			break;
		case SwordFight_Define.AiStrength.STRONG:
		case SwordFight_Define.AiStrength.VERY_STRONG:
			if (GetChara(_charaNo).CheckNowPosStatus(SwordFight_CharacterScript.NowPosStatus.DANGER))
			{
				GetChara(_charaNo).AiDeffence();
			}
			break;
		}
	}
	public SwordFight_CharacterScript GetChara(int _charaNo)
	{
		return SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[_charaNo];
	}
}
