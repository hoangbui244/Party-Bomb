using SaveDataDefine;
using System;
using System.Collections.Generic;
using UnityEngine;
public class DragonBattleGameManager : SingletonCustom<DragonBattleGameManager>
{
	public enum State
	{
		StartWait,
		InGame,
		EndGame,
		Result
	}
	private struct RankingData
	{
		public int playerNo;
		public int score;
	}
	[SerializeField]
	[Header("順位：リザルト")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("ワ\u30fcルドル\u30fcト")]
	private GameObject root3Dworld;
	private State currentState;
	private List<RankingData> listRankData = new List<RankingData>();
	private bool isDebugResult;
	private float waitTime;
	public State CurrentState => currentState;
	public void Init()
	{
		SetState(State.StartWait);
		SingletonCustom<DragonBattleFieldManager>.Instance.Init();
		SingletonCustom<DragonBattleCameraMover>.Instance.Init();
	}
	public void OnGameStart()
	{
		SingletonCustom<CommonStartSimple>.Instance.Show(delegate
		{
			SetState(State.InGame);
			SingletonCustom<DragonBattleCameraMover>.Instance.OnGameStart();
		});
	}
	private void SetState(State _state)
	{
		currentState = _state;
	}
	public void UpdateMethod()
	{
		switch (currentState)
		{
		case State.InGame:
			if (SingletonCustom<DragonBattlePlayerManager>.Instance.IsAllPlayerGoal())
			{
				waitTime = 3f;
				SetState(State.EndGame);
			}
			break;
		case State.EndGame:
			waitTime -= Time.deltaTime;
			if (waitTime <= 0f)
			{
				SingletonCustom<CommonEndSimple>.Instance.Show(delegate
				{
					ToResult();
				});
				SetState(State.Result);
			}
			break;
		}
		SingletonCustom<DragonBattleFieldManager>.Instance.UpdateMethod();
	}
	public void LateUpdateMethod()
	{
		SingletonCustom<DragonBattleFieldManager>.Instance.LateUpdateMethod();
	}
	private void ToResult()
	{
		ResultGameDataParams.SetPoint();
		DragonBattlePlayer[] arrayPlayer = SingletonCustom<DragonBattlePlayerManager>.Instance.GetArrayPlayer();
		for (int i = 0; i < arrayPlayer.Length; i++)
		{
			RankingData item = default(RankingData);
			item.playerNo = (arrayPlayer[i].IsCpu ? (4 + (arrayPlayer[i].PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : arrayPlayer[i].PlayerIdx);
			item.score = arrayPlayer[i].Score;
			if (isDebugResult)
			{
				item.score = UnityEngine.Random.Range(0, 9999);
			}
			listRankData.Add(item);
		}
		listRankData.Sort((RankingData a, RankingData b) => b.score - a.score);
		int[] array = new int[arrayPlayer.Length];
		int[] array2 = new int[arrayPlayer.Length];
		for (int j = 0; j < arrayPlayer.Length; j++)
		{
			array[j] = listRankData[j].score;
			array2[j] = listRankData[j].playerNo;
		}
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1 && array2[0] == 0)
		{
			switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
			{
			case SystemData.AiStrength.WEAK:
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.ATTACK_BALL);
				break;
			case SystemData.AiStrength.NORAML:
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.ATTACK_BALL);
				break;
			case SystemData.AiStrength.STRONG:
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.ATTACK_BALL);
				break;
			}
		}
		ResultGameDataParams.SetRecord_Int(array, array2);
		rankingResult.ShowResult_Score();
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			root3Dworld.SetActive(value: false);
		});
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(base.gameObject);
	}
	public void DebugGoal()
	{
		SingletonCustom<DragonBattleCameraMover>.Instance.DebugGoal();
		for (int i = 0; i < SingletonCustom<DragonBattlePlayerManager>.Instance.GetArrayPlayer().Length; i++)
		{
			SingletonCustom<DragonBattlePlayerManager>.Instance.GetArrayPlayer()[i].transform.SetPositionZ(SingletonCustom<DragonBattleFieldManager>.Instance.GetGoal().transform.position.z);
			SingletonCustom<DragonBattlePlayerManager>.Instance.GetArrayPlayer()[i].transform.SetLocalPositionX(Mathf.Clamp(SingletonCustom<DragonBattlePlayerManager>.Instance.GetArrayPlayer()[i].transform.localPosition.x, -6f, 6f));
		}
		for (int j = 0; j < SingletonCustom<DragonBattlePlayerManager>.Instance.GetArrayPlayer().Length; j++)
		{
			SingletonCustom<DragonBattlePlayerManager>.Instance.GetArrayPlayer()[j].Score = 10 * UnityEngine.Random.Range(50, 300);
		}
	}
}
