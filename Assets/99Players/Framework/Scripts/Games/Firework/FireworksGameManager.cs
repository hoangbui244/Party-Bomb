using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FireworksGameManager : SingletonCustom<FireworksGameManager>
{
	public enum State
	{
		StartWait,
		InGame,
		EndGame,
		Result
	}
	[Serializable]
	private struct RankingData
	{
		public int playerNo;
		public int score;
	}
	[SerializeField]
	[Header("順位：リザルト")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("勝敗：リザルト")]
	private WinOrLoseResultManager winOrLoseResult;
	[SerializeField]
	[Header("ワ\u30fcルドル\u30fcト")]
	private GameObject root3Dworld;
	private State currentState;
	[SerializeField]
	[Header("ランクデ\u30fcタ")]
	private List<RankingData> listRankData = new List<RankingData>();
	[SerializeField]
	[Header("一組ランクデ\u30fcタ")]
	private List<RankingData> firstListRankData = new List<RankingData>();
	private float gameTime;
	private bool isGroup1GameEnd;
	private bool isGroup2GameEnd;
	private bool isChangeBgmPitch;
	private bool isDebugResult;
	private float waitTime;
	public State CurrentState => currentState;
	public float GameTime => gameTime;
	public void Init()
	{
		gameTime = 90f;
		SetState(State.StartWait);
	}
	public void OnGameStart()
	{
		SingletonCustom<CommonStartSimple>.Instance.Show(delegate
		{
			SetState(State.InGame);
		});
		SingletonCustom<FireworksPlayerManager>.Instance.SetGroupVibration();
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
			gameTime = Mathf.Clamp(gameTime -= Time.deltaTime, 0f, 90f);
			SingletonCustom<FireworksUIManager>.Instance.SetGameTime(gameTime);
			if (gameTime <= 0f)
			{
				waitTime = 0f;
				SetState(State.EndGame);
			}
			if (!isChangeBgmPitch && gameTime <= 10f)
			{
				StartCoroutine(_ChangePitchBgm());
				isChangeBgmPitch = true;
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
	}
	private IEnumerator _ChangePitchBgm()
	{
		yield return null;
	}
	private void ToResult()
	{
		SingletonCustom<BigMerchantCustomerManager>.Instance.OnResult();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP && SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
		{
			if (isGroup1GameEnd)
			{
				isGroup2GameEnd = true;
			}
			if (!isGroup1GameEnd || !isGroup2GameEnd)
			{
				FireworksPlayer[] arrayPlayer = SingletonCustom<FireworksPlayerManager>.Instance.GetArrayPlayer();
				isGroup1GameEnd = true;
				for (int i = 0; i < arrayPlayer.Length; i++)
				{
					RankingData item = default(RankingData);
					item.playerNo = (int)arrayPlayer[i].UserType;
					item.score = arrayPlayer[i].Score;
					if (isDebugResult)
					{
						item.score = UnityEngine.Random.Range(0, 9999);
					}
					firstListRankData.Add(item);
				}
				firstListRankData.Sort((RankingData a, RankingData b) => b.score - a.score);
				Init();
				SingletonCustom<SceneManager>.Instance.FadeExec(delegate
				{
					SingletonCustom<FireworksPlayerManager>.Instance.NextGame();
					SingletonCustom<FireworksUIManager>.Instance.Init();
					OnGameStart();
				});
				return;
			}
		}
		ResultGameDataParams.SetPoint();
		FireworksPlayer[] arrayPlayer2 = SingletonCustom<FireworksPlayerManager>.Instance.GetArrayPlayer();
		for (int j = 0; j < arrayPlayer2.Length; j++)
		{
			RankingData item2 = default(RankingData);
			item2.playerNo = (int)arrayPlayer2[j].UserType;
			item2.score = arrayPlayer2[j].Score;
			if (isDebugResult)
			{
				item2.score = UnityEngine.Random.Range(0, 9999);
			}
			listRankData.Add(item2);
		}
		listRankData.Sort((RankingData a, RankingData b) => b.score - a.score);
		int[] array = new int[arrayPlayer2.Length];
		int[] array2 = new int[arrayPlayer2.Length];
		for (int k = 0; k < arrayPlayer2.Length; k++)
		{
			array[k] = listRankData[k].score;
			array2[k] = listRankData[k].playerNo;
		}
		UnityEngine.Debug.Log("判定");
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			int num = 0;
			for (int l = 0; l < array2.Length; l++)
			{
				if (array2[l] == 0)
				{
					num = l;
					break;
				}
			}
			UnityEngine.Debug.Log("判定A" + array[num].ToString());
			if (array[num] >= FireworksDefine.COLLECTION_BRONZE)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.ARCHER_BATTLE);
			}
			if (array[num] >= FireworksDefine.COLLECTION_SILVER)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.ARCHER_BATTLE);
			}
			if (array[num] >= FireworksDefine.COLLECTION_GOLD)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.ARCHER_BATTLE);
			}
		}
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			ResultGameDataParams.SetRecord_Int(array, array2);
			rankingResult.ShowResult_Score();
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			int[] array3 = new int[arrayPlayer2.Length];
			int[] array4 = new int[arrayPlayer2.Length];
			for (int m = 0; m < arrayPlayer2.Length; m++)
			{
				array3[m] = firstListRankData[m].score;
				array4[m] = firstListRankData[m].playerNo;
			}
			ResultGameDataParams.SetRecord_Int(array3, array4);
			ResultGameDataParams.SetRecord_Int(array, array2, _isGroup1Record: false);
			rankingResult.ShowResult_Score();
		}
		else
		{
			int num2 = 0;
			int num3 = 0;
			for (int n = 0; n < arrayPlayer2.Length; n++)
			{
				if (n < 2)
				{
					num2 += arrayPlayer2[n].Score;
					if (isDebugResult)
					{
						num2 += UnityEngine.Random.Range(0, 4500);
					}
				}
				else
				{
					num3 += arrayPlayer2[n].Score;
					if (isDebugResult)
					{
						num3 += UnityEngine.Random.Range(0, 4500);
					}
				}
			}
			UnityEngine.Debug.Log("Ascore:" + num2.ToString());
			UnityEngine.Debug.Log("Bscore:" + num3.ToString());
			num2 = Mathf.Clamp(num2, 0, FireworksDefine.SCORE_MAX);
			num3 = Mathf.Clamp(num3, 0, FireworksDefine.SCORE_MAX);
			ResultGameDataParams.SetRecord_WinOrLose(num2);
			ResultGameDataParams.SetRecord_WinOrLose(num3, 1);
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
			{
				if (num2 > num3)
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 0);
				}
				else if (num2 < num3)
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Lose, 0);
				}
				else
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Draw, 0);
				}
			}
			else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP)
			{
				if (num2 > num3)
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 0);
				}
				else if (num2 < num3)
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 1);
				}
				else
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Draw, 0);
				}
			}
		}
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
}
