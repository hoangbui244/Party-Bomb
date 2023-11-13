using System;
using UnityEngine;
public class Biathlon_GameMain : SingletonCustom<Biathlon_GameMain>
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
	private const float GameEndDelayTime = 6f;
	private const float LastPlayerWaitTime = 20f;
	public const float MaxGameTime = 599.99f;
	[SerializeField]
	private RankingResultManager rankingResult;
	private GameState currentGameState;
	private float currentGameTime;
	private float[] currentGameTimes;
	private bool lastLapPlayed;
	public bool IsGameStarted => currentGameState >= GameState.DuringGame;
	public bool IsDuringGame => currentGameState == GameState.DuringGame;
	public bool IsSinglePlay => SingletonCustom<GameSettingManager>.Instance.IsSinglePlay;
	public Biathlon_Definition.AIStrength AIStrength
	{
		get;
		private set;
	}
	public int CharacterNum => SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length;
	public int PlayerNum => SingletonCustom<GameSettingManager>.Instance.PlayerNum;
	public void Init()
	{
		currentGameState = GameState.StartWait;
		AIStrength = (Biathlon_Definition.AIStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		currentGameTimes = new float[CharacterNum];
	}
	public void PlayGame()
	{
		SingletonCustom<CommonStartProduction>.Instance.Play(GameStart);
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
	public void CharacterGoal(int no)
	{
		Biathlon_Character character = SingletonCustom<Biathlon_Characters>.Instance.GetCharacter(no);
		CalcManager.ConvertTimeToRecordString(currentGameTimes[no], (int)character.ControlUser);
		SingletonCustom<Biathlon_UI>.Instance.ShowResultPlacement(no, character.Placement);
		SingletonCustom<Biathlon_UI>.Instance.HidePlacement(no);
		if (character.IsPlayer)
		{
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal_girl");
		}
		else if (PlayerNum == 3)
		{
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal_boy");
		}
		if (character.IsPlayer)
		{
			if (SingletonCustom<Biathlon_Characters>.Instance.IsPlayerAllGoal)
			{
				DelayedGameEnd(6f);
			}
			else if (SingletonCustom<Biathlon_Characters>.Instance.IsLastOnePlayer)
			{
				DelayedGameEnd(20f);
			}
		}
	}
	public void CharacterForceGoal(int playerNo, int no, float goalTime)
	{
		if (SingletonCustom<Biathlon_Characters>.Instance.GetCharacter(playerNo).IsPlayer)
		{
			currentGameTimes[playerNo] = -1f;
			return;
		}
		goalTime = Mathf.Min(goalTime, 599.99f);
		currentGameTimes[playerNo] = goalTime;
		CalcManager.ConvertTimeToRecordString(goalTime, no);
	}
	public void PlayLastLapSfx()
	{
		if (!lastLapPlayed)
		{
			lastLapPlayed = true;
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_radio_control_lap_3_1", _loop: false, 0f, 1f, 1f, 0.35f);
		}
	}
	private void GameStart()
	{
		currentGameState = GameState.DuringGame;
		SingletonCustom<Biathlon_UI>.Instance.ShowPlacement();
	}
	private void DelayedGameEnd(float delay)
	{
		if (currentGameState == GameState.DuringGame)
		{
			LeanTween.delayedCall(base.gameObject, delay, GameEnd);
		}
	}
	private void GameEnd()
	{
		if (currentGameState == GameState.DuringGame)
		{
			currentGameState = GameState.GameEnd;
			SingletonCustom<Biathlon_Characters>.Instance.ForceGoal(currentGameTime);
			SingletonCustom<Biathlon_Characters>.Instance.GameEnd();
			SingletonCustom<Biathlon_UI>.Instance.HideUI();
			LeanTween.delayedCall(1.5f, (Action)delegate
			{
				currentGameState = GameState.GameEndWait;
			});
		}
	}
	private void GoToResult()
	{
		currentGameState = GameState.DuringResult;
		AcquisitionTrophyIfAble();
		SetResultTimes();
		rankingResult.ShowResult_Time();
	}
	private void StartWait()
	{
	}
	private void DuringGame()
	{
		currentGameTime += Time.deltaTime;
		currentGameTime = Mathf.Min(currentGameTime, 599.99f);
		for (int i = 0; i < CharacterNum; i++)
		{
			if (!SingletonCustom<Biathlon_Characters>.Instance.GetCharacter(i).IsGoal)
			{
				currentGameTimes[i] = currentGameTime;
			}
		}
		SingletonCustom<Biathlon_UI>.Instance.UpdateGameTime(currentGameTimes);
		SingletonCustom<Biathlon_UI>.Instance.UpdateCharactersPositionAll();
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
			float num = currentGameTimes[0];
			if (num <= Biathlon_Definition.BronzeTime)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.DELIVERY_ORDER);
			}
			if (num <= Biathlon_Definition.SilverTime)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.DELIVERY_ORDER);
			}
			if (num <= Biathlon_Definition.GoldTime)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.DELIVERY_ORDER);
			}
		}
	}
	private void SetResultTimes()
	{
		ResultGameDataParams.SetPoint();
		float[] array = new float[currentGameTimes.Length];
		Array.Copy(currentGameTimes, array, currentGameTimes.Length);
		int[] users = SingletonCustom<Biathlon_Characters>.Instance.GetUsers();
		ResultGameDataParams.SetRecord_Float(array, users, _isGroup1Record: true, _isAscendingOrder: true);
	}
}
