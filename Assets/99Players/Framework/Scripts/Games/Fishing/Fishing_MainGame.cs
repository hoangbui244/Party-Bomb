using UnityEngine;
public class Fishing_MainGame : SingletonCustom<Fishing_MainGame>
{
	private enum GameStateType
	{
		StartWait,
		Start,
		DuringGame,
		GameEnd,
		GameEndWait,
		DuringResult
	}
	private const float GameEndToResultTime = 1f;
	private const float ResultWorldCameraHideTime = 2f;
	private const float GameTime = 90f;
	[SerializeField]
	[Header("順位：リザルト")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("3Dワ\u30fcルドを映すカメラ")]
	private Camera world3DCamera;
	private GameStateType currentGameState;
	private float currentStateWaitTime;
	private float currentGameTime;
	public void Init()
	{
		currentGameState = GameStateType.StartWait;
		currentGameTime = 90f;
	}
	public void StartCountDown()
	{
		SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
	}
	public void UpdateMethod()
	{
		switch (currentGameState)
		{
		case GameStateType.Start:
		case GameStateType.GameEnd:
			break;
		case GameStateType.StartWait:
			GameState_StartWait();
			break;
		case GameStateType.DuringGame:
			GameState_DuringGame();
			break;
		case GameStateType.GameEndWait:
			GameState_GameEndWait();
			break;
		case GameStateType.DuringResult:
			GameState_DuringResult();
			break;
		}
	}
	private void GameStart()
	{
		currentGameState = GameStateType.DuringGame;
		FishingDefinition.GUIM.HideInfoControlBalloon();
	}
	private void GameEnd()
	{
		currentGameState = GameStateType.GameEndWait;
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
	}
	private void GameState_StartWait()
	{
	}
	private void GameState_DuringGame()
	{
		currentGameTime -= Time.deltaTime;
		if (currentGameTime <= 0f)
		{
			currentGameTime = 0f;
			FishingDefinition.GUIM.UpdateGameTime(currentGameTime);
			GameEnd();
		}
		FishingDefinition.GUIM.UpdateGameTime(currentGameTime);
	}
	private void GameState_GameEndWait()
	{
		if (FishingDefinition.MCM.CheckAllCharacterWait())
		{
			if (currentStateWaitTime > 1f)
			{
				ToResult();
			}
			else
			{
				currentStateWaitTime += Time.deltaTime;
			}
		}
	}
	private void ToResult()
	{
		currentGameState = GameStateType.DuringResult;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (FishingDefinition.MCM.GetUserData(0).nowPoint >= FishingDefinition.MedalBronzePoint)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BOMB_ROULETTE);
			}
			if (FishingDefinition.MCM.GetUserData(0).nowPoint >= FishingDefinition.MedalSilverPoint)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BOMB_ROULETTE);
			}
			if (FishingDefinition.MCM.GetUserData(0).nowPoint >= FishingDefinition.MedalGoldPoint)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BOMB_ROULETTE);
			}
		}
		ResultGameDataParams.SetPoint();
		ResultGameDataParams.SetRecord_Int(FishingDefinition.MCM.GetUserRecordArray(), FishingDefinition.MCM.GetUserNoArray());
		rankingResult.ShowResult_Score();
		currentStateWaitTime = 0f;
	}
	private void GameState_DuringResult()
	{
		if (world3DCamera.gameObject.activeSelf)
		{
			if (currentStateWaitTime > 2f)
			{
				world3DCamera.gameObject.SetActive(value: false);
				currentStateWaitTime = 0f;
			}
			else
			{
				currentStateWaitTime += Time.deltaTime;
			}
		}
	}
	public float GetGameTime()
	{
		return currentGameTime;
	}
	public bool IsDuringGame()
	{
		return currentGameState == GameStateType.DuringGame;
	}
	public bool IsGameEnd()
	{
		if (currentGameState != GameStateType.GameEnd)
		{
			return currentGameState == GameStateType.GameEndWait;
		}
		return true;
	}
}
