using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HandSeal_GameManager : SingletonCustom<HandSeal_GameManager>
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
	[Header("タイムボ\u30fcナス(Pt/sec)")]
	private float timeBonus = 50f;
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
	private bool isStartCountDown;
	public bool IsStartCountDown => isStartCountDown;
	private void Awake()
	{
		GameEndToResultTime = 1f;
	}
	public void Init()
	{
		currentGameState = GameStateType.StartWait;
		currentGameTime = HandSeal_Define.GAME_TIME;
		HandSeal_Define.UIM.SetGameTime(currentGameTime);
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
		HandSeal_Define.UIM.ShowControlInfoBalloon();
		HandSeal_Define.PM.PlayerGameStart();
	}
	public void GameEnd()
	{
		if (currentGameState == GameStateType.DuringGame)
		{
			HandSeal_Define.UIM.GameEnd();
			for (int i = 0; i < HandSeal_Define.MEMBER_NUM; i++)
			{
				HandSeal_Define.PM.UserData_Group1[i].player.Hand.ProcessTypeChange(HandSeal_Hand.GameProcessType.END);
			}
			currentGameState = GameStateType.GameEndWait;
			StartCoroutine(ToResultWait());
			SingletonCustom<AudioManager>.Instance.SeStop("se_run");
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
			HandSeal_Define.UIM.SetGameTime(currentGameTime);
			GameEnd();
		}
		float currentGameTime2 = currentGameTime;
		currentGameTime = Mathf.Clamp(currentGameTime, 0f, 599.99f);
		HandSeal_Define.UIM.SetGameTime(currentGameTime);
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
			if ((float)HandSeal_Define.PM.GetUserData(HandSeal_Define.UserType.PLAYER_1).point >= HandSeal_Define.MEDAL_BRONZE_POINT)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.DELIVERY_ORDER);
			}
			if ((float)HandSeal_Define.PM.GetUserData(HandSeal_Define.UserType.PLAYER_1).point >= HandSeal_Define.MEDAL_SILVER_POINT)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.DELIVERY_ORDER);
			}
			if ((float)HandSeal_Define.PM.GetUserData(HandSeal_Define.UserType.PLAYER_1).point >= HandSeal_Define.MEDAL_GOLD_POINT)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.DELIVERY_ORDER);
			}
		}
		ResultGameDataParams.SetPoint();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			ResultGameDataParams.SetRecord_Int(HandSeal_Define.PM.GetAllUserRecordArray(), HandSeal_Define.PM.GetAllUserNoArray());
			rankingResult.ShowResult_Score();
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
	private IEnumerator ToResultWait()
	{
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < HandSeal_Define.MEMBER_NUM; i++)
		{
			HandSeal_Define.PM.UserData_Group1[i].player.Hand.timeBonusUIAnchor.SetActive(value: false);
		}
		SingletonCustom<CommonEndSimple>.Instance.Show(delegate
		{
			ToResult();
		});
	}
	public float GetGameTime()
	{
		return currentGameTime;
	}
	public int GetTimeBonusPoint()
	{
		return (int)Mathf.Clamp((float)Mathf.CeilToInt(currentGameTime) * timeBonus, 0f, 3000f);
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
}
