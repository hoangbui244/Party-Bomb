using System;
using UnityEngine;
public class ShavedIce_GameManager : SingletonCustom<ShavedIce_GameManager>
{
	private enum GameStateType
	{
		StartWait,
		Start,
		DuringGame,
		GameEnd,
		GameEndWait,
		DuringHeightCalc,
		TowerHeightCalcWait,
		DuringResult
	}
	[SerializeField]
	[Header("順位：リザルト")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("勝敗：リザルト")]
	private WinOrLoseResultManager winOrLoseResult;
	private GameStateType currentGameState;
	private const float TOWER_HEIGHT_CALC_END_TO_RESULT_TIME = 4f;
	private float towerHeightCalcEndToResultTime;
	private bool isGroup1GameEnd;
	private bool isGroup2GameEnd;
	private bool isGroup1CalcHeightEnd;
	private bool isGroup2CalcHeightEnd;
	private bool controlInformationBalloonDisplay;
	private float duringGameTime;
	public float DuringGameTime => duringGameTime;
	public void Init()
	{
		currentGameState = GameStateType.StartWait;
		duringGameTime = 0f;
	}
	public void UpdateMethod()
	{
		switch (currentGameState)
		{
		case GameStateType.StartWait:
			GameState_StartWait();
			break;
		case GameStateType.DuringGame:
			GameState_DuringGame();
			break;
		case GameStateType.DuringHeightCalc:
			GameState_TowerHeightCalc();
			break;
		case GameStateType.DuringResult:
			GameState_DuringResult();
			break;
		}
	}
	public void StartCountDown()
	{
		SingletonCustom<CommonStartProduction>.Instance.Play(GameStart);
		ShavedIce_Define.PM.SetControlInfomationBalloonFadeIn();
		ShavedIce_Define.PM.SetGroupVibration();
		controlInformationBalloonDisplay = true;
	}
	private void GameStart()
	{
		currentGameState = GameStateType.DuringGame;
		ShavedIce_Define.PM.SetGameStart();
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
		ShavedIce_Define.PM.SetGameEnd();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP && SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
		{
			if (isGroup2GameEnd)
			{
				SingletonCustom<CommonEndSimple>.Instance.Show(delegate
				{
					currentGameState = GameStateType.TowerHeightCalcWait;
					ToTowerHeightCalcWait();
				});
			}
			else
			{
				LeanTween.delayedCall(2f, (Action)delegate
				{
					Group2GameStartWait();
				});
			}
		}
		else
		{
			SingletonCustom<CommonEndSimple>.Instance.Show(delegate
			{
				currentGameState = GameStateType.TowerHeightCalcWait;
				ToTowerHeightCalcWait();
			});
		}
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
	}
	private void Group2GameStartWait()
	{
		currentGameState = GameStateType.StartWait;
		duringGameTime = 0f;
		ShavedIce_Define.UIM.StartScreenFade(delegate
		{
			ShavedIce_Define.PM.SetPreparationGroup2Data();
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
		if (controlInformationBalloonDisplay)
		{
			controlInformationBalloonDisplay = false;
			ShavedIce_Define.PM.SetControlInfomationBalloonFadeOut();
		}
		duringGameTime += Time.deltaTime;
		if (duringGameTime > ShavedIce_Define.GAME_TIME)
		{
			GameEnd();
		}
	}
	private void ToTowerHeightCalcWait()
	{
		ShavedIce_Define.UIM.StartScreenFade(delegate
		{
			ShavedIce_Define.PM.InitTowerHeightCalc((!isGroup1CalcHeightEnd) ? true : false);
		}, delegate
		{
			ShavedIce_Define.UIM.StartTowerHeightCalc();
			LeanTween.delayedCall(0.6f, ToTowerHeightCalc);
		});
		SingletonCustom<AudioManager>.Instance.BgmStop();
		SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_matsuriking_op", _loop: true);
	}
	private void ToTowerHeightCalc()
	{
		currentGameState = GameStateType.DuringHeightCalc;
		ShavedIce_Define.PM.StartTowerHeightCalc((!isGroup1CalcHeightEnd) ? true : false);
	}
	private void GameState_TowerHeightCalc()
	{
		if (!ShavedIce_Define.PM.IsTowerHeightCalcEnd((!isGroup1CalcHeightEnd) ? true : false))
		{
			return;
		}
		if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_drum_roll"))
		{
			SingletonCustom<AudioManager>.Instance.SeStop("se_drum_roll");
		}
		if (towerHeightCalcEndToResultTime > 4f)
		{
			if (!isGroup1CalcHeightEnd)
			{
				isGroup1CalcHeightEnd = true;
			}
			else
			{
				isGroup2CalcHeightEnd = true;
			}
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP && SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
			{
				if (isGroup1CalcHeightEnd && isGroup2CalcHeightEnd)
				{
					ToResult();
					return;
				}
				towerHeightCalcEndToResultTime = 0f;
				ToTowerHeightCalcWait();
			}
			else
			{
				ToResult();
			}
		}
		else
		{
			towerHeightCalcEndToResultTime += Time.deltaTime;
		}
	}
	private void ToResult()
	{
		currentGameState = GameStateType.DuringResult;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (ShavedIce_Define.PM.GetTowerHeight(0, _isGroup1: true) >= ShavedIce_Define.MEDAL_BRONZE_HEIGHT)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.RECEIVE_PON);
			}
			if (ShavedIce_Define.PM.GetTowerHeight(0, _isGroup1: true) >= ShavedIce_Define.MEDAL_SILVER_HEIGHT)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.RECEIVE_PON);
			}
			if (ShavedIce_Define.PM.GetTowerHeight(0, _isGroup1: true) >= ShavedIce_Define.MEDAL_GOLD_HEIGHT)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.RECEIVE_PON);
			}
		}
		ResultGameDataParams.SetPoint();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			ResultGameDataParams.SetRecord_Float(ShavedIce_Define.PM.GetTeamTowerHeightData(_isGroup: true), ShavedIce_Define.PM.GetTeamUserNoData(_isGroup1: true));
			rankingResult.ShowResult_DecimalScore();
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			ResultGameDataParams.SetRecord_Float_Ranking8Numbers(ShavedIce_Define.PM.GetAllTowerHeightData(), ShavedIce_Define.PM.GetAllUserNoData());
			for (int i = 0; i < ShavedIce_Define.PM.GetAllTowerHeightData().Length; i++)
			{
				UnityEngine.Debug.Log("記録：" + ShavedIce_Define.PM.GetAllTowerHeightData()[i].ToString());
				UnityEngine.Debug.Log("ユ\u30fcザ\u30fc番号：" + ShavedIce_Define.PM.GetAllUserNoData()[i].ToString());
			}
			rankingResult.ShowResult_DecimalScore();
		}
		else
		{
			ResultGameDataParams.SetRecord_WinOrLose(ShavedIce_Define.PM.GetTeamTotalRecord(0));
			ResultGameDataParams.SetRecord_WinOrLose(ShavedIce_Define.PM.GetTeamTotalRecord(1), 1);
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
			{
				if (ShavedIce_Define.PM.GetTeamTotalRecord(0) > ShavedIce_Define.PM.GetTeamTotalRecord(1))
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 0);
				}
				else if (ShavedIce_Define.PM.GetTeamTotalRecord(0) < ShavedIce_Define.PM.GetTeamTotalRecord(1))
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
				if (ShavedIce_Define.PM.GetTeamTotalRecord(0) > ShavedIce_Define.PM.GetTeamTotalRecord(1))
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 0);
				}
				else if (ShavedIce_Define.PM.GetTeamTotalRecord(0) < ShavedIce_Define.PM.GetTeamTotalRecord(1))
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, 1);
				}
				else
				{
					winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Draw, 0);
				}
			}
		}
		LeanTween.delayedCall(3f, ShavedIce_Define.PM.HideAllGameCamera);
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
	public bool IsDuringHeightCalc()
	{
		return currentGameState == GameStateType.DuringHeightCalc;
	}
	public bool IsDuringResult()
	{
		return currentGameState == GameStateType.DuringResult;
	}
}
