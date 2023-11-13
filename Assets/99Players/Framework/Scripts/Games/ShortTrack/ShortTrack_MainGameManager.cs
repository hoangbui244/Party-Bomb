using System;
using UnityEngine;
public class ShortTrack_MainGameManager : SingletonCustom<ShortTrack_MainGameManager>
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
	[Header("順位：")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("3Dワ\u30fcルドを映すカメラ")]
	private Camera world3DCamera;
	[SerializeField]
	[Header("UIのオブジェクト")]
	private GameObject UI;
	private GameStateType currentGameState;
	private float lastOneWaitTime;
	private readonly float LAST_ONE_RESULT_TIME = 20f;
	public int lastOneResultCount;
	private float currentStateWaitTime;
	private readonly float GAME_END_TO_RESULT_TIME = 5f;
	private float gameEndToResultTime;
	private readonly float RESULT_WORLD_CAMERA_HIDE_TIME = 2f;
	private float gameTime;
	private void Awake()
	{
		gameEndToResultTime = GAME_END_TO_RESULT_TIME;
	}
	public void Init()
	{
		currentGameState = GameStateType.StartWait;
	}
	public void RoundStart_CountDown()
	{
		SingletonCustom<CommonStartProduction>.Instance.Play(GameStart);
		LeanTween.delayedCall(base.gameObject, 3f, (Action)delegate
		{
			SHORTTRACK.MCM.StartStay();
		});
		LeanTween.delayedCall(base.gameObject, 4f, (Action)delegate
		{
			for (int i = 0; i < SHORTTRACK.MCM.PData.Length; i++)
			{
				if (SHORTTRACK.MCM.IsNowRunnerControlePlayer(SHORTTRACK.MCM.PData[i].characters))
				{
					SingletonCustom<ShortTrack_UIManager>.Instance.GetDashButtonPress(i).ShowInfoControlAnimation();
				}
			}
		});
	}
	public void UpdateMethot()
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
			SingletonCustom<AudioManager>.Instance.SeStop();
			break;
		case GameStateType.DuringResult:
			GameState_DuringResult();
			break;
		}
	}
	private void GameStart()
	{
		currentGameState = GameStateType.DuringGame;
		SHORTTRACK.MCM.StartRun();
		LeanTween.delayedCall(base.gameObject, 1.5f, (Action)delegate
		{
			for (int i = 0; i < SHORTTRACK.MCM.PData.Length; i++)
			{
				if (SHORTTRACK.MCM.IsNowRunnerControlePlayer(SHORTTRACK.MCM.PData[i].characters))
				{
					SingletonCustom<ShortTrack_UIManager>.Instance.GetDashButtonPress(i).HideInfoControlAnimation();
				}
			}
		});
	}
	public void GameEnd()
	{
		currentGameState = GameStateType.GameEndWait;
		SingletonCustom<AudioManager>.Instance.SeStop();
	}
	private void GameState_StartWait()
	{
	}
	private void GameState_DuringGame()
	{
		gameTime += Time.deltaTime;
		gameTime = Mathf.Clamp(gameTime, 0f, 599.99f);
		if (lastOneResultCount == SHORTTRACK.MCM.PData.Length - 1 && SingletonCustom<GameSettingManager>.Instance.PlayerNum != 1)
		{
			lastOneWaitTime += Time.deltaTime;
		}
		if (lastOneWaitTime >= LAST_ONE_RESULT_TIME)
		{
			UI.SetActive(value: false);
			ToResult();
		}
	}
	private void GameState_GameEndWait()
	{
		if (currentStateWaitTime > gameEndToResultTime)
		{
			UI.SetActive(value: false);
			ToResult();
		}
		else
		{
			currentStateWaitTime += Time.deltaTime;
		}
	}
	private void ToResult()
	{
		int num = 0;
		while (num < SHORTTRACK.MCM.PData.Length)
		{
			if (SHORTTRACK.MCM.PData[num].isGoal)
			{
				num++;
				continue;
			}
			SHORTTRACK.MCM.PData[num].isGoal = true;
			SHORTTRACK.MCM.PData[num].goalTime = -1f;
		}
		currentGameState = GameStateType.DuringResult;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			int num2 = 1;
			while (num2 < SHORTTRACK.MCM.PData.Length && SHORTTRACK.MCM.PData[0].goalTime < SHORTTRACK.MCM.PData[num2].goalTime)
			{
				num2++;
				if (SHORTTRACK.MCM.PData[0].goalTime == -1f)
				{
					break;
				}
				if (num2 >= SHORTTRACK.MCM.PData.Length)
				{
					if (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength == 0)
					{
						UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.MOLE_HAMMER);
					}
					if (1 == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
					{
						UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.MOLE_HAMMER);
					}
					if (2 == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
					{
						UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.MOLE_HAMMER);
					}
				}
			}
		}
		ResultGameDataParams.SetPoint();
		float[] array = new float[SHORTTRACK.MCM.PData.Length];
		int[] array2 = new int[SHORTTRACK.MCM.PData.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array[i] = SHORTTRACK.MCM.PData[i].goalTime;
			array2[i] = SHORTTRACK.MCM.PData[i].userType;
		}
		for (int j = 0; j < SHORTTRACK.MCM.PData.Length; j++)
		{
			CalcManager.ConvertTimeToRecordString(array[j], array2[j]);
		}
		ResultGameDataParams.SetRecord_Float(array, array2, _isGroup1Record: true, _isAscendingOrder: true);
		rankingResult.ShowResult_Time();
		currentStateWaitTime = 0f;
	}
	private void GameState_DuringResult()
	{
		if (world3DCamera.gameObject.activeSelf)
		{
			if (currentStateWaitTime > RESULT_WORLD_CAMERA_HIDE_TIME)
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
		return gameTime;
	}
	public bool IsDuringGame()
	{
		return currentGameState == GameStateType.DuringGame;
	}
	public bool IsRunCharacter()
	{
		if (currentGameState != GameStateType.DuringGame)
		{
			return currentGameState == GameStateType.GameEndWait;
		}
		return true;
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
	public void DebugGameEnd()
	{
		currentGameState = GameStateType.GameEndWait;
		for (int i = 0; i < SHORTTRACK.MCM.PData.Length; i++)
		{
			SHORTTRACK.MCM.PData[i].goalTime = UnityEngine.Random.Range(60f, 120f);
			SHORTTRACK.MCM.PData[i].isGoal = true;
		}
	}
}
