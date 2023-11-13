using UnityEngine;
public class Takoyaki_GameManager : SingletonCustom<Takoyaki_GameManager>
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
	[SerializeField]
	[Header("勝敗：リザルト")]
	private WinOrLoseResultManager winOrLoseResult;
	[SerializeField]
	[Header("順位：リザルト")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("3Dワ\u30fcルドを映すカメラ")]
	private Camera world3DCamera;
	private GameStateType currentGameState;
	private float currentStateWaitTime;
	private const float GAME_END_TO_RESULT_TIME = 1f;
	private float GameEndToResultTime;
	private const float RESULT_WORLD_CAMERA_HIDE_TIME = 2f;
	private float currentGameTime;
	private bool isGroup1GameEnd;
	private bool isGroup2GameEnd;
	private void Awake()
	{
		GameEndToResultTime = 1f;
	}
	public void Init()
	{
		currentGameState = GameStateType.StartWait;
		currentGameTime = Takoyaki_Define.GAME_TIME;
		Takoyaki_Define.UIM.SetGameTime(currentGameTime);
	}
	public void StartCountDown()
	{
		SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
		Takoyaki_Define.PM.SetGroupVibration();
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
		Takoyaki_Define.UIM.ShowControlInfoBalloon();
	}
	private void GameEnd()
	{
		currentGameState = GameStateType.GameEndWait;
		if (!isGroup1GameEnd)
		{
			isGroup1GameEnd = true;
		}
		else if (!isGroup2GameEnd)
		{
			isGroup2GameEnd = true;
		}
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
		if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_takoyaki_baking"))
		{
			SingletonCustom<AudioManager>.Instance.SeStop("se_takoyaki_baking");
		}
	}
	private void Group2GameStartWait()
	{
		currentGameState = GameStateType.StartWait;
		Takoyaki_Define.UIM.NextGroup2Fade(delegate
		{
			currentGameTime = Takoyaki_Define.GAME_TIME;
			Takoyaki_Define.UIM.SetGameTime(currentGameTime);
			Takoyaki_Define.PM.SetPreparationGroup2Data();
		}, delegate
		{
			StartCountDown();
		});
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
			Takoyaki_Define.UIM.SetGameTime(currentGameTime);
			GameEnd();
		}
		Takoyaki_Define.UIM.SetGameTime(currentGameTime);
	}
	private void GameState_GameEndWait()
	{
		if (currentStateWaitTime > GameEndToResultTime)
		{
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP && SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
			{
				if (isGroup1GameEnd && isGroup2GameEnd)
				{
					ToResult();
					return;
				}
				currentStateWaitTime = 0f;
				Group2GameStartWait();
			}
			else
			{
				ToResult();
			}
		}
		else
		{
			currentStateWaitTime += Time.deltaTime;
		}
	}
	private void ToResult()
	{
		currentGameState = GameStateType.DuringResult;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (Takoyaki_Define.PM.GetUserData(Takoyaki_Define.UserType.PLAYER_1).takoyakiCnt >= Takoyaki_Define.MEDAL_BRONZE_POINT)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BOMB_ROULETTE);
			}
			if (Takoyaki_Define.PM.GetUserData(Takoyaki_Define.UserType.PLAYER_1).takoyakiCnt >= Takoyaki_Define.MEDAL_SILVER_POINT)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BOMB_ROULETTE);
			}
			if (Takoyaki_Define.PM.GetUserData(Takoyaki_Define.UserType.PLAYER_1).takoyakiCnt >= Takoyaki_Define.MEDAL_GOLD_POINT)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BOMB_ROULETTE);
			}
		}
		ResultGameDataParams.SetPoint();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			ResultGameDataParams.SetRecord_Int(Takoyaki_Define.PM.GetAllUserRecordArray(), Takoyaki_Define.PM.GetAllUserNoArray());
			rankingResult.ShowResult_Score();
		}
		else if (Takoyaki_Define.PLAYER_NUM > 2 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			ResultGameDataParams.SetRecord_Int(Takoyaki_Define.PM.GetAllUserRecordArray(), Takoyaki_Define.PM.GetAllUserNoArray());
			ResultGameDataParams.SetRecord_Int(Takoyaki_Define.PM.GetAllUserRecordArray(_isGroup1: false), Takoyaki_Define.PM.GetAllUserNoArray(_isGroup1: false), _isGroup1Record: false);
			rankingResult.ShowResult_Score();
		}
		else
		{
			ResultGameDataParams.SetRecord_WinOrLose(Takoyaki_Define.PM.GetTeamTotalRecord(Takoyaki_Define.TeamType.TEAM_A));
			ResultGameDataParams.SetRecord_WinOrLose(Takoyaki_Define.PM.GetTeamTotalRecord(Takoyaki_Define.TeamType.TEAM_B), 1);
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
			{
				if (Takoyaki_Define.PM.GetTeamTotalRecord(Takoyaki_Define.TeamType.TEAM_A) > Takoyaki_Define.PM.GetTeamTotalRecord(Takoyaki_Define.TeamType.TEAM_B))
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 0);
				}
				else if (Takoyaki_Define.PM.GetTeamTotalRecord(Takoyaki_Define.TeamType.TEAM_A) < Takoyaki_Define.PM.GetTeamTotalRecord(Takoyaki_Define.TeamType.TEAM_B))
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
				if (Takoyaki_Define.PM.GetTeamTotalRecord(Takoyaki_Define.TeamType.TEAM_A) > Takoyaki_Define.PM.GetTeamTotalRecord(Takoyaki_Define.TeamType.TEAM_B))
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 0);
				}
				else if (Takoyaki_Define.PM.GetTeamTotalRecord(Takoyaki_Define.TeamType.TEAM_A) < Takoyaki_Define.PM.GetTeamTotalRecord(Takoyaki_Define.TeamType.TEAM_B))
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 1);
				}
				else
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Draw, 0);
				}
			}
		}
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
