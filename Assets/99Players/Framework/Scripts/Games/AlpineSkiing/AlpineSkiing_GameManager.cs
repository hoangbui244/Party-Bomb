using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AlpineSkiing_GameManager : SingletonCustom<AlpineSkiing_GameManager> {
    private enum GameStateType {
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
    [Header("ゴ\u30fcルの位置(仮ゴ\u30fcル時間設定用_1P~4P)")]
    private Transform[] goalPos;
    [SerializeField]
    [Header("距離1m当たりの追加時間(仮ゴ\u30fcル時間設定用)")]
    private float timePerLenge = 0.2f;
    [SerializeField]
    [Header("スタ\u30fcト地点のゲ\u30fcト")]
    private AlpineSkiing_Props_Gate_Anime[] gateAnime;
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
    private void Awake() {
        GameEndToResultTime = 1f;
    }
    public void AccelSEManager(bool _set = false) {
        if (_set) {
            if (!SingletonCustom<AudioManager>.Instance.IsSePlaying("se_alpineskiing_slide_2")) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_slide_2", _loop: true);
            }
            if (!SingletonCustom<AudioManager>.Instance.IsSePlaying("se_snowboard_air")) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_snowboard_air", _loop: true);
            }
            return;
        }
        for (int i = 0; i < AlpineSkiing_Define.MEMBER_NUM && (AlpineSkiing_Define.PM.Players[i].UserType > AlpineSkiing_Define.UserType.PLAYER_4 || AlpineSkiing_Define.PM.Players[i].SkiBoard.processType != AlpineSkiing_SkiBoard.SkiBoardProcessType.ACCEL); i++) {
        }
        if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_alpineskiing_slide_2")) {
            SingletonCustom<AudioManager>.Instance.SeStop("se_alpineskiing_slide_2");
        }
        if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_snowboard_air")) {
            SingletonCustom<AudioManager>.Instance.SeStop("se_snowboard_air");
        }
    }
    public void Init() {
        currentGameState = GameStateType.StartWait;
        currentGameTime = 0f;
        AlpineSkiing_Define.UIM.SetGameTime(currentGameTime);
        SingletonCustom<AlpineSkiing_CourseManager>.Instance.Init();
        SingletonCustom<AlpineSkiing_RankingManager>.Instance.Init();
    }
    public void StartCountDown() {
        SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
    }
    public void UpdateMethod() {
        switch (currentGameState) {
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
    private void GameStart() {
        currentGameState = GameStateType.DuringGame;
        AlpineSkiing_Define.UIM.ShowControlInfoBalloon();
        AlpineSkiing_Define.PM.PlayerGameStart();
        for (int i = 0; i < gateAnime.Length; i++) {
            gateAnime[i].AnimeStart();
        }
    }
    public void GameEnd() {
        if (currentGameState != GameStateType.DuringGame) {
            return;
        }
        StopCoroutine(ForcedGoal());
        for (int i = 0; i < AlpineSkiing_Define.MEMBER_NUM; i++) {
            if (AlpineSkiing_Define.PM.UserData_Group1[i].player.SkiBoard.processType != AlpineSkiing_SkiBoard.SkiBoardProcessType.GOAL) {
                if (AlpineSkiing_Define.PM.UserData_Group1[i].userType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                    AlpineSkiing_Define.PM.SetGoalTime(AlpineSkiing_Define.PM.UserData_Group1[i].player.UserType, -1f);
                    forcedGoalFlg = true;
                } else {
                    float num = Vector3.Distance(goalPos[i].position, AlpineSkiing_Define.PM.UserData_Group1[i].player.SkiBoard.gameObject.transform.position) * timePerLenge;
                    AlpineSkiing_Define.PM.SetGoalTime(AlpineSkiing_Define.PM.UserData_Group1[i].player.UserType, AlpineSkiing_Define.GM.GetGameTime() + num);
                }
            }
        }
        if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_alpineskiing_slide")) {
            SingletonCustom<AudioManager>.Instance.SeStop("se_alpineskiing_slide");
        }
        if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_alpineskiing_slide_2")) {
            SingletonCustom<AudioManager>.Instance.SeStop("se_alpineskiing_slide_2");
        }
        if (SingletonCustom<AudioManager>.Instance.IsSePlaying("se_snowboard_air")) {
            SingletonCustom<AudioManager>.Instance.SeStop("se_snowboard_air");
        }
        AlpineSkiing_Define.UIM.GameEnd();
        currentGameState = GameStateType.GameEndWait;
        if (forcedGoalFlg) {
            ToResult();
        } else {
            StartCoroutine(ToResultWait());
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
    }
    private void GameState_StartWait() {
    }
    private void GameState_DuringGame() {
        currentGameTime += Time.deltaTime;
        currentGameTime = Mathf.Clamp(currentGameTime, 0f, 599.99f);
        AlpineSkiing_Define.UIM.SetGameTime(currentGameTime);
    }
    private void GameState_GameEndWait() {
        if (!(currentStateWaitTime > GameEndToResultTime)) {
            currentStateWaitTime += Time.deltaTime;
        }
    }
    private void ToResult() {
        currentGameState = GameStateType.DuringResult;
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
            if (AlpineSkiing_Define.PM.GetUserData(AlpineSkiing_Define.UserType.PLAYER_1).goalTime <= AlpineSkiing_Define.MEDAL_BRONZE_POINT) {
                UnityEngine.Debug.Log("<color=#ac6b25>銅メダル獲得</color>");
                SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.CANNON_SHOT);
            }
            if (AlpineSkiing_Define.PM.GetUserData(AlpineSkiing_Define.UserType.PLAYER_1).goalTime <= AlpineSkiing_Define.MEDAL_SILVER_POINT) {
                UnityEngine.Debug.Log("<color=#c0c0c0>銀メダル獲得</color>");
                SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.CANNON_SHOT);
            }
            if (AlpineSkiing_Define.PM.GetUserData(AlpineSkiing_Define.UserType.PLAYER_1).goalTime <= AlpineSkiing_Define.MEDAL_GOLD_POINT) {
                UnityEngine.Debug.Log("<color=#e6b422>金メダル獲得</color>");
                SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.CANNON_SHOT);
            }
        }
        for (int i = 0; i < AlpineSkiing_Define.MEMBER_NUM; i++) {
            CalcManager.ConvertTimeToRecordString(AlpineSkiing_Define.PM.UserData_Group1[i].goalTime, (int)AlpineSkiing_Define.PM.UserData_Group1[i].userType);
        }
        ResultGameDataParams.SetPoint();
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            ResultGameDataParams.SetRecord_Float(AlpineSkiing_Define.PM.GetAllUserRecordArray(), AlpineSkiing_Define.PM.GetAllUserNoArray(), _isGroup1Record: true, _isAscendingOrder: true);
            rankingResult.ShowResult_Time();
        }
        currentStateWaitTime = 0f;
        UnityEngine.Debug.Log("ゲ\u30fcム終了処理!!");
    }
    private void GameState_DuringResult() {
        if (world3DCamera.gameObject.activeSelf) {
            if (currentStateWaitTime > 2f) {
                world3DCamera.gameObject.SetActive(value: false);
                currentStateWaitTime = 0f;
            } else {
                currentStateWaitTime += Time.deltaTime;
            }
        }
    }
    private IEnumerator ToResultWait() {
        yield return new WaitForSeconds(5f);
        ToResult();
    }
    public float GetGameTime() {
        return currentGameTime;
    }
    public bool IsDuringGame() {
        return currentGameState == GameStateType.DuringGame;
    }
    public bool IsGameEndWait() {
        return currentGameState == GameStateType.GameEndWait;
    }
    public bool IsGameEnd() {
        if (currentGameState != GameStateType.GameEnd && currentGameState != GameStateType.GameEndWait) {
            return currentGameState == GameStateType.DuringResult;
        }
        return true;
    }
    private IEnumerator ForcedGoal() {
        yield return new WaitForSeconds(20f);
        GameEnd();
    }
    public void StartForcedGoal() {
        if (!startForcedGoalFlg) {
            startForcedGoalFlg = true;
            StartCoroutine(ForcedGoal());
        }
    }
}
