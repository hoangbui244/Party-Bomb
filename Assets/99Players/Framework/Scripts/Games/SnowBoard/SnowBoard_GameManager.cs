using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SnowBoard_GameManager : SingletonCustom<SnowBoard_GameManager>
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
	[SerializeField]
	[Header("スタ\u30fcト地点のゲ\u30fcト")]
	private AlpineSkiing_Props_Gate_Anime[] gateAnime;
	[SerializeField]
	[Header("全競技モ\u30fcドで使うオブジェクト(※プレイヤ\u30fc以外)")]
	private GameObject[] obj_ALL;
	[SerializeField]
	[Header("好きな競技モ\u30fcドで使うオブジェクト(※プレイヤ\u30fc以外)")]
	private GameObject[] obj_SINGLE;
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
	private float[] arrayGoalTime = new float[9];
	private List<int> firstSetPlayer = new List<int>();
	private bool isEightRun;
	public bool IsEightRun => isEightRun;
	private void Awake()
	{
		GameEndToResultTime = 1f;
	}
	public void Init()
	{
		currentGameState = GameStateType.StartWait;
		currentGameTime = 0f;
		SnowBoard_Define.UIM.SetGameTime(currentGameTime);
		SingletonCustom<SnowBoard_CourseManager>.Instance.Init();
		SingletonCustom<SnowBoard_RankingManager>.Instance.Init();
		isEightRun = SingletonCustom<GameSettingManager>.Instance.IsEightBattle;
		SnowBoard_Define.PLAYER_NUM = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE && SnowBoard_Define.GM.IsEightRun)
		{
			SnowBoard_Define.MEMBER_NUM = 8;
			SnowBoard_Define.CPU_NUM = SnowBoard_Define.MEMBER_NUM - SnowBoard_Define.PLAYER_NUM;
		}
		else
		{
			SnowBoard_Define.MEMBER_NUM = 4;
			SnowBoard_Define.CPU_NUM = SnowBoard_Define.MEMBER_NUM - SnowBoard_Define.PLAYER_NUM;
		}
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE && isEightRun)
		{
			for (int i = 0; i < obj_ALL.Length; i++)
			{
				obj_ALL[i].SetActive(value: false);
			}
		}
		else
		{
			for (int j = 0; j < obj_SINGLE.Length; j++)
			{
				obj_SINGLE[j].SetActive(value: false);
			}
		}
		ResultGameDataParams.SetPoint();
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
		SnowBoard_Define.UIM.ShowControlInfoBalloon();
		SnowBoard_Define.PM.PlayerGameStart();
		for (int i = 0; i < gateAnime.Length; i++)
		{
			gateAnime[i].AnimeStart();
		}
	}
	public void GameEnd()
	{
		if (currentGameState != GameStateType.DuringGame)
		{
			return;
		}
		StopCoroutine(ForcedGoal());
		for (int i = 0; i < SnowBoard_Define.MEMBER_NUM; i++)
		{
			if (SnowBoard_Define.PM.UserData_Group1[i].player.SkiBoard.processType != SnowBoard_SkiBoard.SkiBoardProcessType.GOAL)
			{
				if (SnowBoard_Define.PM.UserData_Group1[i].userType <= SnowBoard_Define.UserType.PLAYER_4)
				{
					SnowBoard_Define.PM.SetGoalTime(SnowBoard_Define.PM.UserData_Group1[i].player.UserType, -1f);
					forcedGoalFlg = true;
				}
				else
				{
					float num = Vector3.Distance(goalPos.position, SnowBoard_Define.PM.UserData_Group1[i].player.SkiBoard.gameObject.transform.position) * timePerLenge;
					SnowBoard_Define.PM.SetGoalTime(SnowBoard_Define.PM.UserData_Group1[i].player.UserType, SnowBoard_Define.GM.GetGameTime() + num);
				}
			}
		}
		if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_alpineskiing_slide"))
		{
			SingletonCustom<AudioManager>.Instance.SeStop("se_alpineskiing_slide");
		}
		if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_alpineskiing_slide_2"))
		{
			SingletonCustom<AudioManager>.Instance.SeStop("se_alpineskiing_slide_2");
		}
		if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_snowboard_air"))
		{
			SingletonCustom<AudioManager>.Instance.SeStop("se_snowboard_air");
		}
		SnowBoard_Define.UIM.GameEnd();
		currentGameState = GameStateType.GameEndWait;
		if (forcedGoalFlg)
		{
			ToResult();
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
		SnowBoard_Define.UIM.SetGameTime(currentGameTime);
	}
	private void GameState_GameEndWait()
	{
		if (!(currentStateWaitTime > GameEndToResultTime))
		{
			currentStateWaitTime += Time.deltaTime;
		}
	}
	private void ToResult()
	{
		currentGameState = GameStateType.DuringResult;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (SnowBoard_Define.PM.GetUserData(SnowBoard_Define.UserType.PLAYER_1).goalTime <= SnowBoard_Define.MEDAL_BRONZE_POINT)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.GET_BALL);
			}
			if (SnowBoard_Define.PM.GetUserData(SnowBoard_Define.UserType.PLAYER_1).goalTime <= SnowBoard_Define.MEDAL_SILVER_POINT)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.GET_BALL);
			}
			if (SnowBoard_Define.PM.GetUserData(SnowBoard_Define.UserType.PLAYER_1).goalTime <= SnowBoard_Define.MEDAL_GOLD_POINT)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.GET_BALL);
			}
		}
		for (int i = 0; i < SnowBoard_Define.MEMBER_NUM; i++)
		{
			CalcManager.ConvertTimeToRecordString(SnowBoard_Define.PM.UserData_Group1[i].goalTime, (int)SnowBoard_Define.PM.UserData_Group1[i].userType);
		}
		List<int>[] playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		float[] array = new float[SnowBoard_Define.MEMBER_NUM];
		int[] array2 = new int[SnowBoard_Define.MEMBER_NUM];
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = SnowBoard_Define.PM.UserData_Group1[j].goalTime;
			array2[j] = (int)SnowBoard_Define.PM.UserData_Group1[j].userType;
			if (j > 3)
			{
				playerGroupList[j % 4].Add((int)SnowBoard_Define.PM.UserData_Group1[j].userType);
			}
		}
		if (IsEightRun)
		{
			ResultGameDataParams.SetRecord_Float_Ranking8Numbers(array, array2, _isAscendingOrder: true);
		}
		else
		{
			ResultGameDataParams.SetRecord_Float(array, array2, _isGroup1Record: true, _isAscendingOrder: true);
		}
		rankingResult.ShowResult_Time();
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
	private IEnumerator ToResultWait()
	{
		yield return new WaitForSeconds(5f);
		ToResult();
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
