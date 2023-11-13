using System.Collections.Generic;
using UnityEngine;
public class ArenaBattlePlayerManager : SingletonCustom<ArenaBattlePlayerManager>
{
	[SerializeField]
	[Header("プレイヤ\u30fc配列")]
	private ArenaBattlePlayer[] arrayPlayer;
	private int livePlayerCnt;
	private int calcIdx;
	private float calcDistance;
	private ArenaBattlePlayer rootCharacter;
	private List<ArenaBattlePlayer> tempListPlayer = new List<ArenaBattlePlayer>();
	public int LivePlayerCnt => livePlayerCnt;
	public void Init()
	{
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			arrayPlayer[i].Init(i);
		}
	}
	public void Appearance()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			arrayPlayer[i].Appearance();
		}
	}
	public void UpdateMethod()
	{
		ArenaBattleGameManager.State currentState = SingletonCustom<ArenaBattleGameManager>.Instance.CurrentState;
		if ((uint)(currentState - 1) > 1u)
		{
			return;
		}
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			arrayPlayer[i].UpdateMethod();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.G))
		{
			for (int j = 0; j < arrayPlayer.Length; j++)
			{
				arrayPlayer[j].KnockBack(Vector3.zero, 1f, 0.5f);
			}
		}
	}
	public bool IsGameEnd()
	{
		livePlayerCnt = arrayPlayer.Length;
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (arrayPlayer[i].Hp <= 0f)
			{
				livePlayerCnt--;
			}
		}
		return livePlayerCnt <= 1;
	}
	public void SetLivePlayerTime()
	{
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (arrayPlayer[i].Hp > 0f)
			{
				arrayPlayer[i].OnVictory();
				SingletonCustom<ArenaBattleGameManager>.Instance.SetTime(arrayPlayer[i].PlayerIdx, -999f);
			}
		}
	}
	public ArenaBattlePlayer GetPlayerAtIdx(int _idx)
	{
		return arrayPlayer[_idx];
	}
	public ArenaBattlePlayer GetOpponentCharacter(int _playerNo)
	{
		calcIdx = -1;
		calcDistance = 999f;
		rootCharacter = null;
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (_playerNo == arrayPlayer[i].PlayerIdx)
			{
				rootCharacter = arrayPlayer[i];
			}
		}
		for (int j = 0; j < arrayPlayer.Length; j++)
		{
			if (arrayPlayer[j].gameObject.activeSelf && _playerNo != arrayPlayer[j].PlayerIdx && arrayPlayer[j].Hp > 0f && (calcIdx == -1 || CalcManager.Length(arrayPlayer[j].transform.position, rootCharacter.transform.position) < calcDistance))
			{
				calcIdx = j;
				calcDistance = CalcManager.Length(arrayPlayer[j].transform.position, rootCharacter.transform.position);
			}
		}
		if (calcIdx == -1)
		{
			return null;
		}
		return arrayPlayer[calcIdx];
	}
	public ArenaBattlePlayer GetOpponentCharacterAtHp(int _playerNo)
	{
		calcIdx = -1;
		calcDistance = 0f;
		rootCharacter = null;
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (_playerNo == arrayPlayer[i].PlayerIdx)
			{
				rootCharacter = arrayPlayer[i];
			}
		}
		for (int j = 0; j < arrayPlayer.Length; j++)
		{
			if (arrayPlayer[j].gameObject.activeSelf && _playerNo != arrayPlayer[j].PlayerIdx && arrayPlayer[j].Hp > 0f && (calcIdx == -1 || arrayPlayer[j].Hp > calcDistance))
			{
				calcIdx = j;
				calcDistance = arrayPlayer[j].Hp;
			}
		}
		if (calcIdx == -1)
		{
			return null;
		}
		return arrayPlayer[calcIdx];
	}
	public ArenaBattlePlayer GetOpponentCharacterAtRandom(int _playerNo)
	{
		calcIdx = -1;
		calcDistance = 999f;
		rootCharacter = null;
		tempListPlayer.Clear();
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			if (_playerNo == arrayPlayer[i].PlayerIdx)
			{
				rootCharacter = arrayPlayer[i];
			}
		}
		for (int j = 0; j < arrayPlayer.Length; j++)
		{
			if (arrayPlayer[j].gameObject.activeSelf && _playerNo != arrayPlayer[j].PlayerIdx && arrayPlayer[j].Hp > 0f)
			{
				tempListPlayer.Add(arrayPlayer[j]);
			}
		}
		if (tempListPlayer.Count <= 0)
		{
			return null;
		}
		return tempListPlayer[Random.Range(0, tempListPlayer.Count)];
	}
}
