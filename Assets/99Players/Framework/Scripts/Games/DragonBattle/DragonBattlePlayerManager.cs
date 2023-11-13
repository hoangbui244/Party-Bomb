using UnityEngine;
public class DragonBattlePlayerManager : SingletonCustom<DragonBattlePlayerManager>
{
	[SerializeField]
	[Header("プレイヤ\u30fc配列")]
	private DragonBattlePlayer[] arrayPlayer;
	private int goalCnt;
	private bool isGoalVoice;
	private bool isProhibitAttack;
	[SerializeField]
	[Header("ドラゴンマテリアル")]
	private Material[] dragonMats;
	public bool IsProhibitAttack
	{
		get
		{
			return isProhibitAttack;
		}
		set
		{
			isProhibitAttack = value;
		}
	}
	public Material[] DragonMats => dragonMats;
	public void Init()
	{
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			arrayPlayer[i].Init(i);
		}
	}
	public DragonBattlePlayer GetPlayerAtIdx(int _idx)
	{
		return arrayPlayer[_idx];
	}
	public DragonBattlePlayer[] GetArrayPlayer()
	{
		return arrayPlayer;
	}
	public int OnGoal(DragonBattlePlayer _player)
	{
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (arrayPlayer[i] == _player)
			{
				if (!isGoalVoice && !arrayPlayer[i].IsCpu)
				{
					SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal");
					isGoalVoice = true;
				}
				SingletonCustom<AudioManager>.Instance.SePlay("se_cracker");
				SingletonCustom<AudioManager>.Instance.SePlay("se_cheer");
				goalCnt++;
				return goalCnt - 1;
			}
		}
		return -1;
	}
	public bool IsAllPlayerGoal()
	{
		CalcManager.mCalcBool = true;
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (arrayPlayer[i].GoalNo == -1)
			{
				CalcManager.mCalcBool = false;
			}
		}
		return CalcManager.mCalcBool;
	}
	public DragonBattlePlayer Search1stScorePlayer(bool _isDeathExclusion = false)
	{
		if (CheckAllPlayerDeath())
		{
			_isDeathExclusion = false;
		}
		int num = 0;
		for (int i = 1; i < arrayPlayer.Length; i++)
		{
			if ((!_isDeathExclusion || !arrayPlayer[i].IsDeath()) && arrayPlayer[i].Score > arrayPlayer[num].Score)
			{
				num = i;
			}
		}
		return arrayPlayer[num];
	}
	public DragonBattlePlayer SearchRandomPlayer(bool _isDeathExclusion = false)
	{
		if (CheckAllPlayerDeath())
		{
			_isDeathExclusion = false;
		}
		int num = 0;
		do
		{
			num = Random.Range(0, arrayPlayer.Length);
		}
		while (_isDeathExclusion && arrayPlayer[num].IsDeath());
		return arrayPlayer[num];
	}
	public bool CheckAllPlayerDeath()
	{
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (!arrayPlayer[i].IsDeath())
			{
				return false;
			}
		}
		return true;
	}
	public float GetEndLineCharacterLocalZ()
	{
		CalcManager.mCalcFloat = 9999f;
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (arrayPlayer[i].transform.localPosition.z < CalcManager.mCalcFloat)
			{
				CalcManager.mCalcFloat = arrayPlayer[i].transform.localPosition.z;
			}
		}
		return CalcManager.mCalcFloat;
	}
	public void UpdateMethod()
	{
		DragonBattleGameManager.State currentState = SingletonCustom<DragonBattleGameManager>.Instance.CurrentState;
		if ((uint)(currentState - 1) <= 1u)
		{
			for (int i = 0; i < arrayPlayer.Length; i++)
			{
				arrayPlayer[i].UpdateMethod();
			}
		}
	}
	public DragonBattlePlayer CheckTargetedEnemy(DragonBattleEnemyNinja _enemy)
	{
		return CheckTargetedEnemy(_enemy.gameObject);
	}
	public DragonBattlePlayer CheckTargetedEnemy(GameObject _enemy)
	{
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (arrayPlayer[i].IsCpu && (((bool)arrayPlayer[i].Ai.AttackTarget && arrayPlayer[i].Ai.AttackTarget.gameObject == _enemy) || (arrayPlayer[i].Ai.ListEnemyLog.Length != 0 && (bool)arrayPlayer[i].Ai.ListEnemyLog[0].enemy && arrayPlayer[i].Ai.ListEnemyLog[0].enemy.gameObject == _enemy)))
			{
				return arrayPlayer[i];
			}
		}
		return null;
	}
	public DragonBattlePlayer CheckPlayer(GameObject _colObj)
	{
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (arrayPlayer[i].gameObject == _colObj)
			{
				return arrayPlayer[i];
			}
			for (int j = 0; j < arrayPlayer[i].ColObj.Length; j++)
			{
				if (arrayPlayer[i].ColObj[j] == _colObj)
				{
					return arrayPlayer[i];
				}
			}
		}
		return null;
	}
}
