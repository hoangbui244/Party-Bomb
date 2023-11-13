using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Surfing_GameManager : SingletonCustom<Surfing_GameManager>
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
	[Header("3Dワ\u30fcルドを映すカメラ")]
	private Camera world3DCamera;
	[SerializeField]
	[Header("ゴ\u30fcルの位置(仮ゴ\u30fcル時間設定用)")]
	private Transform goalPos;
	[SerializeField]
	[Header("距離1m当たりの追加時間(仮ゴ\u30fcル時間設定用)")]
	private float timePerLenge = 0.2f;
	private GameStateType currentGameState;
	private float currentStateWaitTime;
	private const float GAME_END_TO_RESULT_TIME = 1f;
	private float GameEndToResultTime;
	private const float RESULT_WORLD_CAMERA_HIDE_TIME = 2f;
	private float currentGameTime;
	private bool isGroup1GameEnd;
	private bool isGroup2GameEnd;
	private float gameTime;
	private bool startForcedGoalFlg;
	private bool forcedGoalFlg;
	private bool isStartCountDown;
	private bool isOnceGoalVoice;
	private float[] arrayGoalTime = new float[9];
	private List<int> firstSetPlayer = new List<int>();
	public bool IsStartCountDown => isStartCountDown;
	private void Awake()
	{
		GameEndToResultTime = 1f;
	}
	public void Init()
	{
		currentGameState = GameStateType.StartWait;
		currentGameTime = Surfing_Define.GAME_TIME;
		Surfing_Define.UIM.SetGameTime(currentGameTime);
	}
	public void StartCountDown()
	{
		SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
		isStartCountDown = true;
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
		Surfing_Define.UIM.ShowControlInfoBalloon();
		Surfing_Define.PM.PlayerGameStart();
	}
	public void GameEnd()
	{
		if (currentGameState == GameStateType.DuringGame)
		{
			for (int i = 0; i < Surfing_Define.MEMBER_NUM; i++)
			{
				Surfing_Define.PM.UserData_Group1[i].player.Surfer.ProcessTypeChange(Surfing_Surfer.SurferProcessType.GOAL);
			}
			Surfing_Define.UIM.GameEnd();
			currentGameState = GameStateType.GameEndWait;
			SingletonCustom<CommonEndSimple>.Instance.Show(delegate
			{
				ToResult();
			});
		}
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
			Surfing_Define.UIM.SetGameTime(currentGameTime);
			GameEnd();
		}
		float currentGameTime2 = currentGameTime;
		currentGameTime = Mathf.Clamp(currentGameTime, 0f, 599.99f);
		Surfing_Define.UIM.SetGameTime(currentGameTime);
	}
	private void GameState_GameEndWait()
	{
		if (!(currentStateWaitTime > GameEndToResultTime))
		{
			currentStateWaitTime += Time.deltaTime;
		}
	}
	public void GoalSEPlay()
	{
		if (!isOnceGoalVoice)
		{
			isOnceGoalVoice = true;
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal");
		}
	}
	private void ToResult()
	{
		currentGameState = GameStateType.DuringResult;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if ((float)Surfing_Define.PM.GetUserData(Surfing_Define.UserType.PLAYER_1).point >= Surfing_Define.MEDAL_BRONZE_POINT)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.GET_BALL);
			}
			if ((float)Surfing_Define.PM.GetUserData(Surfing_Define.UserType.PLAYER_1).point >= Surfing_Define.MEDAL_SILVER_POINT)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.GET_BALL);
			}
			if ((float)Surfing_Define.PM.GetUserData(Surfing_Define.UserType.PLAYER_1).point >= Surfing_Define.MEDAL_GOLD_POINT)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.GET_BALL);
			}
		}
		ResultGameDataParams.SetPoint();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			ResultGameDataParams.SetRecord_Int(Surfing_Define.PM.GetAllUserRecordArray(), Surfing_Define.PM.GetAllUserNoArray());
			rankingResult.ShowResult_Score();
		}
		currentStateWaitTime = 0f;
		UnityEngine.Debug.Log("ゲ\u30fcム終了処理!!");
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
	private IEnumerator ToResultWait()
	{
		yield return new WaitForSeconds(5f);
		SingletonCustom<CommonEndSimple>.Instance.Show(delegate
		{
			ToResult();
		});
	}
	public float GetGameTime()
	{
		return currentGameTime;
	}
	public bool IsDuringGame()
	{
		return currentGameState == GameStateType.DuringGame;
	}
	public bool IsGameEndWait()
	{
		return currentGameState == GameStateType.GameEndWait;
	}
	public bool IsGameEnd()
	{
		if (currentGameState != GameStateType.GameEnd && currentGameState != GameStateType.GameEndWait)
		{
			return currentGameState == GameStateType.DuringResult;
		}
		return true;
	}
	private IEnumerator ForcedGoal()
	{
		yield return new WaitForSeconds(20f);
		GameEnd();
	}
	public void StartForcedGoal()
	{
		if (!startForcedGoalFlg)
		{
			startForcedGoalFlg = true;
			StartCoroutine(ForcedGoal());
		}
	}
}
