using System;
using UnityEngine;
public class SmeltFishing_GameMain : SingletonCustom<SmeltFishing_GameMain>
{
	private enum GameState
	{
		StartWait,
		Start,
		DuringGame,
		GameEnd,
		GameEndWait,
		DuringResult
	}
	public const float GameTime = 120f;
	private const float GameEndToResultTime = 1f;
	private const float ResultWorldCameraHideTime = 2f;
	[SerializeField]
	private RankingResultManager rankingResult;
	private GameState currentGameState;
	private float currentStateWaitTime;
	private float currentGameTime;
	public bool IsDuringGame => currentGameState == GameState.DuringGame;
	public SmeltFishing_Definition.AIStrength AIStrength
	{
		get;
		private set;
	}
	public void Init()
	{
		currentGameState = GameState.StartWait;
		AIStrength = (SmeltFishing_Definition.AIStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		currentGameTime = 120f;
	}
	public void PlayGame()
	{
		SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
	}
	public void UpdateMethod()
	{
		switch (currentGameState)
		{
		case GameState.Start:
		case GameState.GameEnd:
			break;
		case GameState.StartWait:
			StartWait();
			break;
		case GameState.DuringGame:
			DuringGame();
			break;
		case GameState.GameEndWait:
			GameEndWait();
			break;
		case GameState.DuringResult:
			DuringResult();
			break;
		}
	}
	private void GameStart()
	{
		currentGameState = GameState.DuringGame;
		SingletonCustom<SmeltFishing_UI>.Instance.ShowAssistCommentAll(0);
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			SingletonCustom<SmeltFishing_Characters>.Instance.GameStart();
		});
	}
	private void GameEnd()
	{
		currentGameState = GameState.GameEnd;
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
		SingletonCustom<CommonEndSimple>.Instance.Show(delegate
		{
			currentGameState = GameState.GameEndWait;
		});
		SingletonCustom<SmeltFishing_Characters>.Instance.GameEnd();
	}
	private void GoToResult()
	{
		currentGameState = GameState.DuringResult;
		AcquisitionTrophyIfAble();
		SetResultScore();
		rankingResult.ShowResult_Score();
		currentStateWaitTime = 0f;
	}
	private void StartWait()
	{
	}
	private void DuringGame()
	{
		currentGameTime -= Time.deltaTime;
		currentGameTime = Mathf.Max(currentGameTime, 0f);
		SingletonCustom<SmeltFishing_UI>.Instance.UpdateGameTime(currentGameTime);
		if (!(currentGameTime > 0f))
		{
			GameEnd();
		}
	}
	private void GameEndWait()
	{
		GoToResult();
	}
	private void DuringResult()
	{
	}
	private void AcquisitionTrophyIfAble()
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (SingletonCustom<SmeltFishing_Characters>.Instance.GetPlayer(0).Score >= FishingDefinition.MedalBronzePoint)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BOMB_ROULETTE);
			}
			if (SingletonCustom<SmeltFishing_Characters>.Instance.GetPlayer(0).Score >= FishingDefinition.MedalSilverPoint)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BOMB_ROULETTE);
			}
			if (SingletonCustom<SmeltFishing_Characters>.Instance.GetPlayer(0).Score >= FishingDefinition.MedalGoldPoint)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BOMB_ROULETTE);
			}
		}
	}
	private void SetResultScore()
	{
		ResultGameDataParams.SetPoint();
		(int[], int[]) result = SingletonCustom<SmeltFishing_Characters>.Instance.GetResult();
		int[] item = result.Item1;
		int[] item2 = result.Item2;
		ResultGameDataParams.SetRecord_Int(item, item2);
	}
}
