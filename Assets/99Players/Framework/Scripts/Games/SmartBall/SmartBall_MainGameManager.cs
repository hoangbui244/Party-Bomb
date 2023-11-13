using UnityEngine;
public class SmartBall_MainGameManager : SingletonCustom<SmartBall_MainGameManager>
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
	[Header("順位：リザルト")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("勝敗：リザルト")]
	private WinOrLoseResultManager winOrLoseResult;
	private GameStateType currentGameState;
	private float currentStateWaitTime;
	private const float GAME_END_TO_RESULT_TIME = 4f;
	private float GameEndToResultTime;
	private const float RESULT_WORLD_CAMERA_HIDE_TIME = 2f;
	private bool isGroup1GameEnd;
	private bool isGroup2GameEnd;
	private const float CONTROL_INFOMATION_BALLOON_DISPLAY_TIME = 1f;
	private float currentControlInfomationBalloonDisplayTime;
	private bool controlInformationBalloonDisplay;
	public void Init()
	{
		GameEndToResultTime = 4f;
		currentGameState = GameStateType.StartWait;
		currentControlInfomationBalloonDisplayTime = 0f;
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
			if (Application.isEditor && UnityEngine.Input.GetKeyDown(KeyCode.R))
			{
				GameEnd();
			}
			break;
		case GameStateType.GameEndWait:
			GameState_GameEndWait();
			break;
		case GameStateType.DuringResult:
			GameState_DuringResult();
			break;
		}
	}
	public void StartCountDown()
	{
		SingletonCustom<CommonStartProduction>.Instance.Play(GameStart);
		SB.MCM.SetControlInfomationBalloonFadeIn();
		SB.MCM.SetGroupVibration();
		controlInformationBalloonDisplay = true;
	}
	private void GameStart()
	{
		currentGameState = GameStateType.DuringGame;
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
	}
	private void Group2GameStartWait()
	{
		currentGameState = GameStateType.StartWait;
		SB.GUIM.NextGroup2Fade(delegate
		{
			SB.MCM.SetPreparationGroup2Data();
			SB.GUIM.HideSkipControlInfo();
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
		if (SB.MCM.IsAllPlayerTeamFailed())
		{
			if (SB.MCM.IsAllTeamFailed())
			{
				GameEnd();
			}
			else
			{
				SB.GUIM.ViewSkipControlInfo();
				if (SB.CM.IsButton_X(0, SmartBall_ControllerManager.ButtonStateType.DOWN))
				{
					GameEnd();
				}
			}
		}
		if (controlInformationBalloonDisplay)
		{
			controlInformationBalloonDisplay = false;
			SB.MCM.SetControlInfomationBalloonFadeOut();
		}
	}
	private void GameState_GameEndWait()
	{
		if (currentStateWaitTime > GameEndToResultTime)
		{
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP && SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
			{
				if (isGroup1GameEnd && isGroup2GameEnd)
				{
					SB.MCM.SetAutoCPURecord(_isGroup1: false);
					ToResult();
				}
				else
				{
					currentStateWaitTime = 0f;
					SB.MCM.SetAutoCPURecord(_isGroup1: true);
					Group2GameStartWait();
				}
			}
			else
			{
				SB.MCM.SetAutoCPURecord(_isGroup1: true);
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
			if (SB.MCM.GetPoint(0) >= SB.MEDAL_BRONZE_POINT)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.ATTACK_BALL);
			}
			if (SB.MCM.GetPoint(0) >= SB.MEDAL_SILVER_POINT)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.ATTACK_BALL);
			}
			if (SB.MCM.GetPoint(0) >= SB.MEDAL_GOLD_POINT)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.ATTACK_BALL);
			}
		}
		ResultGameDataParams.SetPoint();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			ResultGameDataParams.SetRecord_Int(SB.MCM.GetTeamPointData(_isGroup: true), SB.MCM.GetTeamUserNoData(_isGroup1: true));
			rankingResult.ShowResult_Score();
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			ResultGameDataParams.SetRecord_Int(SB.MCM.GetTeamPointData(_isGroup: true), SB.MCM.GetTeamUserNoData(_isGroup1: true));
			ResultGameDataParams.SetRecord_Int(SB.MCM.GetTeamPointData(_isGroup: false), SB.MCM.GetTeamUserNoData(_isGroup1: false), _isGroup1Record: false);
			rankingResult.ShowResult_Score();
		}
		else
		{
			ResultGameDataParams.SetRecord_WinOrLose(SB.MCM.GetTeamTotalRecord(0));
			ResultGameDataParams.SetRecord_WinOrLose(SB.MCM.GetTeamTotalRecord(1), 1);
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
			{
				if (SB.MCM.GetTeamTotalRecord(0) > SB.MCM.GetTeamTotalRecord(1))
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 0);
				}
				else if (SB.MCM.GetTeamTotalRecord(0) < SB.MCM.GetTeamTotalRecord(1))
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
				if (SB.MCM.GetTeamTotalRecord(0) > SB.MCM.GetTeamTotalRecord(1))
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 0);
				}
				else if (SB.MCM.GetTeamTotalRecord(0) < SB.MCM.GetTeamTotalRecord(1))
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
	public bool IsDuringResult()
	{
		return currentGameState == GameStateType.DuringResult;
	}
}
