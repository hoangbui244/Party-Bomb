using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WaterSpriderRace_GameManager : SingletonCustom<WaterSpriderRace_GameManager>
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
		currentGameTime = 0f;
		WaterSpriderRace_Define.UIM.SetGameTime(currentGameTime);
		SingletonCustom<WaterSpriderRace_CourseManager>.Instance.Init();
		SingletonCustom<WaterSpriderRace_RankingManager>.Instance.Init();
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
		WaterSpriderRace_Define.UIM.ShowControlInfoBalloon();
		WaterSpriderRace_Define.PM.PlayerGameStart();
	}
	public void GameEnd()
	{
		if (currentGameState != GameStateType.DuringGame)
		{
			return;
		}
		StopCoroutine(ForcedGoal());
		for (int i = 0; i < WaterSpriderRace_Define.MEMBER_NUM; i++)
		{
			if (WaterSpriderRace_Define.PM.UserData_Group1[i].player.WaterSprider.processType != WaterSpriderRace_WaterSprider.SkiBoardProcessType.GOAL)
			{
				if (WaterSpriderRace_Define.PM.UserData_Group1[i].userType <= WaterSpriderRace_Define.UserType.PLAYER_4)
				{
					WaterSpriderRace_Define.PM.SetGoalTime(WaterSpriderRace_Define.PM.UserData_Group1[i].player.UserType, -1f);
					forcedGoalFlg = true;
				}
				else
				{
					float num = Mathf.Abs(goalPos.position.z - WaterSpriderRace_Define.PM.UserData_Group1[i].player.WaterSprider.gameObject.transform.position.z) * timePerLenge;
					WaterSpriderRace_Define.PM.SetGoalTime(WaterSpriderRace_Define.PM.UserData_Group1[i].player.UserType, WaterSpriderRace_Define.GM.GetGameTime() + num);
				}
			}
		}
		WaterSpriderRace_Define.UIM.GameEnd();
		currentGameState = GameStateType.GameEndWait;
		if (forcedGoalFlg)
		{
			SingletonCustom<CommonEndSimple>.Instance.Show(delegate
			{
				ToResult();
			});
		}
		else
		{
			StartCoroutine(ToResultWait());
		}
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
	}
	private void GameState_StartWait()
	{
	}
	private void GameState_DuringGame()
	{
		currentGameTime += Time.deltaTime;
		currentGameTime = Mathf.Clamp(currentGameTime, 0f, 599.99f);
		WaterSpriderRace_Define.UIM.SetGameTime(currentGameTime);
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
			if (WaterSpriderRace_Define.PM.GetUserData(WaterSpriderRace_Define.UserType.PLAYER_1).goalTime <= WaterSpriderRace_Define.MEDAL_BRONZE_POINT)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.CANNON_SHOT);
			}
			if (WaterSpriderRace_Define.PM.GetUserData(WaterSpriderRace_Define.UserType.PLAYER_1).goalTime <= WaterSpriderRace_Define.MEDAL_SILVER_POINT)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.CANNON_SHOT);
			}
			if (WaterSpriderRace_Define.PM.GetUserData(WaterSpriderRace_Define.UserType.PLAYER_1).goalTime <= WaterSpriderRace_Define.MEDAL_GOLD_POINT)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.CANNON_SHOT);
			}
		}
		for (int i = 0; i < WaterSpriderRace_Define.MEMBER_NUM; i++)
		{
			CalcManager.ConvertTimeToRecordString(WaterSpriderRace_Define.PM.UserData_Group1[i].goalTime, (int)WaterSpriderRace_Define.PM.UserData_Group1[i].userType);
		}
		ResultGameDataParams.SetPoint();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			ResultGameDataParams.SetRecord_Float(WaterSpriderRace_Define.PM.GetAllUserRecordArray(), WaterSpriderRace_Define.PM.GetAllUserNoArray(), _isGroup1Record: true, _isAscendingOrder: true);
			rankingResult.ShowResult_Time();
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
