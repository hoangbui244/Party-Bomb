using UnityEngine;
public class Skijump_GameManager : SingletonCustom<Skijump_GameManager>
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
		Skijump_Define.MCM.GameStartProcess();
	}
	public void GameEnd()
	{
		currentGameState = GameStateType.GameEndWait;
		Skijump_Define.MCM.GameEndProcess();
		SingletonCustom<Skijump_UIManager>.Instance.CloseJumpScore();
		SingletonCustom<CommonEndSimple>.Instance.Show(ToResult);
		SingletonCustom<Skijump_WindManager>.Instance.ToResult();
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
	}
	private void ToResult()
	{
		currentGameState = GameStateType.DuringResult;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (Skijump_Define.GUIM.GetTotalScore(Skijump_Define.UserType.PLAYER_1) >= (float)Skijump_Define.MEDAL_BRONZE_POINT)
			{
				UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.ATTACK_BALL);
			}
			if (Skijump_Define.GUIM.GetTotalScore(Skijump_Define.UserType.PLAYER_1) >= (float)Skijump_Define.MEDAL_SILVER_POINT)
			{
				UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.ATTACK_BALL);
			}
			if (Skijump_Define.GUIM.GetTotalScore(Skijump_Define.UserType.PLAYER_1) >= (float)Skijump_Define.MEDAL_GOLD_POINT)
			{
				UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.ATTACK_BALL);
			}
		}
		ResultGameDataParams.SetPoint();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			for (int i = 0; i < Skijump_Define.MCM.GetAllUserTotalScore().Length; i++)
			{
				UnityEngine.Debug.Log(" ユ\u30fcザ\u30fc：" + Skijump_Define.MCM.GetAllUserNo()[i].ToString() + " スコア：" + Skijump_Define.MCM.GetAllUserTotalScore()[i].ToString());
			}
			ResultGameDataParams.SetRecord_Int(Skijump_Define.MCM.GetAllUserTotalScore(), Skijump_Define.MCM.GetAllUserNo());
			rankingResult.ShowResult_Score();
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
