using UnityEngine;
public class Bowling_MainGameManager : SingletonCustom<Bowling_MainGameManager>
{
	public enum GameStateType
	{
		StartWait,
		Start,
		DuringGame,
		GameEnd,
		GameEndWait,
		DuringResult
	}
	[SerializeField]
	[Header("順位：リザルト")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("勝敗：リザルト")]
	private WinOrLoseResultManager winOrLoseResult;
	[SerializeField]
	[Header("3Dワ\u30fcルドを映すカメラ")]
	private Camera world3DCamera;
	private GameStateType currentGameState;
	private float currentStateWaitTime;
	private const float GAME_END_TO_RESULT_TIME = 2f;
	private const float RESULT_WORLD_CAMERA_HIDE_TIME = 2f;
	private const float GAME_TIME = 30f;
	private float currentGameTime;
	public void Init()
	{
		currentGameState = GameStateType.StartWait;
		currentGameTime = 30f;
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
	public void StartProduction()
	{
		SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
	}
	private void GameStart()
	{
		currentGameState = GameStateType.DuringGame;
		Bowling_Define.MPM.GameStartProcess();
		Bowling_Define.MSM.GameStartProcess();
	}
	public void GameEnd()
	{
		currentGameState = GameStateType.GameEndWait;
		Bowling_Define.MPM.GameEndProcess();
		Bowling_Define.MSM.GameEndProcess();
		SingletonCustom<CommonEndSimple>.Instance.Show(ToResult);
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
	}
	private void ToResult()
	{
		currentGameState = GameStateType.DuringResult;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (Bowling_Define.GUIM.GetTotalScore(Bowling_Define.UserType.PLAYER_1) >= Bowling_Define.MEDAL_BRONZE_POINT)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.MOLE_HAMMER);
			}
			if (Bowling_Define.GUIM.GetTotalScore(Bowling_Define.UserType.PLAYER_1) >= Bowling_Define.MEDAL_SILVER_POINT)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.MOLE_HAMMER);
			}
			if (Bowling_Define.GUIM.GetTotalScore(Bowling_Define.UserType.PLAYER_1) >= Bowling_Define.MEDAL_GOLD_POINT)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.MOLE_HAMMER);
			}
		}
		ResultGameDataParams.SetPoint();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			ResultGameDataParams.SetRecord_Int(Bowling_Define.MPM.GetAllUserTotalScore(), Bowling_Define.MPM.GetAllUserNo());
			rankingResult.ShowResult_Score();
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			ResultGameDataParams.SetRecord_Int_Ranking8Numbers(Bowling_Define.MPM.GetAllUserTotalScore(), Bowling_Define.MPM.GetAllUserNo());
			rankingResult.ShowResult_Score();
		}
		else
		{
			ResultGameDataParams.SetRecord_WinOrLose(Bowling_Define.MPM.GetTeamTotalScore(Bowling_Define.TeamType.TEAM_A));
			ResultGameDataParams.SetRecord_WinOrLose(Bowling_Define.MPM.GetTeamTotalScore(Bowling_Define.TeamType.TEAM_B), 1);
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
			{
				if (Bowling_Define.MPM.GetTeamTotalScore(Bowling_Define.TeamType.TEAM_A) > Bowling_Define.MPM.GetTeamTotalScore(Bowling_Define.TeamType.TEAM_B))
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 0);
				}
				else if (Bowling_Define.MPM.GetTeamTotalScore(Bowling_Define.TeamType.TEAM_A) < Bowling_Define.MPM.GetTeamTotalScore(Bowling_Define.TeamType.TEAM_B))
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Lose, 0);
				}
				else
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Draw, -1);
				}
			}
			else if (Bowling_Define.MPM.GetTeamTotalScore(Bowling_Define.TeamType.TEAM_A) > Bowling_Define.MPM.GetTeamTotalScore(Bowling_Define.TeamType.TEAM_B))
			{
				winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 0);
			}
			else if (Bowling_Define.MPM.GetTeamTotalScore(Bowling_Define.TeamType.TEAM_A) < Bowling_Define.MPM.GetTeamTotalScore(Bowling_Define.TeamType.TEAM_B))
			{
				winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 1);
			}
			else
			{
				winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Draw, -1);
			}
		}
		currentStateWaitTime = 0f;
	}
	private void GameState_StartWait()
	{
	}
	private void GameState_DuringGame()
	{
	}
	private void GameState_GameEndWait()
	{
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
	public GameStateType GetGameState()
	{
		return currentGameState;
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
