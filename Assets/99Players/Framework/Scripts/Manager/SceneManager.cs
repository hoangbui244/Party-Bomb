using GamepadInput;
using io.ninenine.players.party3d.games.common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;
/// <summary>
/// 
/// </summary>
public class SceneManager : SingletonCustom<SceneManager> {
    /// <summary>
    /// 
    /// </summary>
    public enum SceneType {
        TITLE,
        MAIN,
        GET_BALL,
        CANNON_SHOT,
        BLOCK_WIPER,
        MOLE_HAMMER,
        BOMB_ROULETTE,
        RECEIVE_PON,
        BLACKSMITH,
        ARCHER_BATTLE,
        ATTACK_BALL,
        BLOW_AWAY_TANK,
        SCROLL_JUMP,
        CLIMB_BLOCK,
        MAKE_SAME_DOT,
        TIMING_STAMPING,
        WATER_PISTOL_BATTLE,
        KURUKURU_CUBE,
        THREE_LEGGED,
        REVERSI,
        BUNBUN_JUMP,
        PUSH_LANE,
        JAMMING_DIVING,
        AWAY_HOIHOI,
        THROWING_BALLS,
        ANSWERS_RUN,
        ANSWER_DROP,
        BLOCK_CHANGER,
        PERFECT_DICE_ROLL,
        TRAP_RACE,
        MARUTA_JUMP,
        TIME_STOP,
        WOBBLY_DISK,
        ROBOT_WATCH,
        MINISCAPE_RACE,
        BELT_CONVEYOR_RUN,
        PUSH_IN_BOXING,
        BOMB_LIFTING,
        BATTLE_AIR_HOCKEY,
        BLOCK_SLICER,
        ESCAPE_ZONE,
        LABYRINTH,
        DROP_BLOCK,
        GIRIGIRI_WATER,
        DROP_PANEL,
        GIRIGIRI_STOP,
        ROCK_PAPER_SCISSORS,
        NONSTOP_PICTURE,
        ROPE_JUMPING,
        TRAIN_GUIDE,
        BREAK_BLOCK,
        GUARD_ZONE,
        HOME_RUN_DERBY,
        FLYING_HAMMER,
        COLORFUL_SHOOT,
        CLIMB_WALL,
        TREASURE_CATCHER,
        COIN_DROP,
        HUNDRED_CHALLENGE,
        JANJAN_FISHING,
        EVERYONE_KEEPER,
        /// <summary>
        /// 61
        /// </summary>
        AIR_BALLOON,
        /// <summary>
        /// 62
        /// </summary>
        BOOKSQUIRM,
        /// <summary>
        /// 63
        /// </summary>
        PUSHY_PENGUINS,
        /// <summary>
        /// 64
        /// </summary>
        CATCH_YOU_LETTER,
        /// <summary>
        /// </summary>
        TOAD_QUICK_DRAW,
        /// <summary>
        /// Jackal game, all 2D games start from here
        /// 65
        /// </summary>
        JACKAL,
        /// <summary>
        /// 66
        /// </summary>
        BOMBERMAN,
        /// <summary>
        /// 67
        /// </summary>
        TETRIS,
        /// <summary>
        /// 68
        /// </summary>
        GUN_SMOKE,
        /// <summary>
        /// 69
        /// </summary>
        IKARI_WARRIORS,
        /// <summary>
        /// 70
        /// </summary>
        DINO_RIKI,
        /// <summary>
        /// 71
        /// </summary>
        DONKEY_KONG,
        /// <summary>
        /// 72
        /// </summary>
        GAUNTLET,
        PLAY_KING_OP,
        CHEER_BATTLE,
        RESULT_ANNOUNCEMENT,
        MAX
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_closeType"></param>
    public delegate void LayerClose(LayerCloseType _closeType);
    /// <summary>
    /// 
    /// </summary>
    public enum LayerCloseType {
        /// <summary>
        /// 
        /// </summary>
        NORMAL,
        /// <summary>
        /// 
        /// </summary>
        CHANGE_SCENE,
        /// <summary>
        /// 
        /// </summary>
        BACK_KEY
    }
    /// <summary>
    /// 
    /// </summary>
    private bool isLoadAocDLC;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("メインカメラ")]
    private Camera mainCamera;
    /// <summary>
    /// 
    /// </summary>
    [Header("エディタ専用：DLC1所持フラグ")]
    public bool editorDlc1EnableFlg;
    /// <summary>
    /// 
    /// </summary>
    [Header("エディタ専用：DLC2所持フラグ")]
    public bool editorDlc2EnableFlg;
    /// <summary>
    /// 
    /// </summary>
    [Header("エディタ専用：DLC3所持フラグ")]
    public bool editorDlc3EnableFlg;
    /// <summary>
    /// 
    /// </summary>
    [Header("ロ\u30fcディングマネ\u30fcジャ\u30fc")]
    public LoadingManager mLoadingManager;
    /// <summary>
    /// 
    /// </summary>
    private SceneType mPrevScene;
    /// <summary>
    /// 
    /// </summary>
    private SceneType mNowScene;
    /// <summary>
    /// 
    /// </summary>
    private SceneType mNextScene;
    /// <summary>
    /// 
    /// </summary>
    private SceneType mBeforLastScene;
    /// <summary>
    /// 
    /// </summary>
    private GameObject[] mSceneObject;
    /// <summary>
    /// 
    /// </summary>
    private GameObject[] mSceneObjectPref;
    /// <summary>
    /// 
    /// </summary>
    private string[] LOAD_SCENE_NAME = new string[]
    {
        "Scene_Title",
        "Scene_Main",
        "Scene_RadioControlParty_Soccer",
        "Scene_CannonShot",
        "Scene_BlockWiper",
        "Scene_MoleHammer",
        "Scene_BombRoulette",
        "Scene_ReceivePon",
        "Scene_BlackSmith",
        "Scene_ArcherBattle",
        "Scene_AttackBall",
        "Scene_BlowAwayTank",
        "Scene_ScrollJump",
        "Scene_ClimbBlock",
        "Scene_MakeSameDot",
        "Scene_TimingStamping",
        "Scene_WaterPistolBattle",
        "Scene_KurukuruCube",
        "Scene_ThreeLegged",
        "Scene_Reversi",
        "Scene_BunbunJump",
        "Scene_PushLane",
        "Scene_JammingDiving",
        "Scene_AwayHoihoi",
        "Scene_ThrowingBalls",
        "Scene_AnswersRun",
        "Scene_AnswerDrop",
        "Scene_BlockChanger",
        "Scene_PerfectDiceRoll",
        "",
        "Scene_MarutaJump",
        "Scene_TimeStop",
        "Scene_WobblyDisk",
        "Scene_RobotWatch",
        "Scene_MiniscapeRace",
        "Scene_BeltConveyorRun",
        "Scene_PushInBoxing",
        "Scene_BombLifting",
        "Scene_BattleAirHockey",
        "Scene_BlockSlicer",
        "Scene_EscapeZone",
        "Scene_Labyrinth",
        "Scene_DropBlock",
        "Scene_GirigiriWater",
        "Scene_DropPanel",
        "Scene_GirigiriStop",
        "Scene_RockPaperScissors",
        "Scene_NonstopPicture",
        "Scene_RopeJumping",
        "Scene_TrainGuide",
        "Scene_BreakBlock",
        "Scene_GuardZone",
        "Scene_HomeRunDerby",
        "Scene_FlyingHammer",
        "Scene_ColorfulShoot",
        "Scene_ClimbWall",
        "Scene_TreasureCatcher",
        "Scene_CoinDrop",
        "Scene_HundredChallenge",
        "Scene_JanjanFishing",
        "Scene_EveryoneKeeper",
        "Scene_AirBalloon",
        "Scene_BookSquirm",
        "Scene_PushyPenguins",
        "Scene_CatchYouLetter",
        "Scene_ToadQuickDraw",
        "Scene_Jackal",
        "Scene_Bomberman",
        "Scene_Tetris",
        "Scene_GunSmoke",
        "Scene_IkariWarriors",
        "Scene_DinoRiki",
        "Scene_DonkeyKong",
        "Scene_Gauntlet",
        "Scene_PlayKingOpening",
        "Scene_CheerBattle",
        "Scene_ResultAnnouncement"
    };
    /// <summary>
    /// 
    /// </summary>
    private List<LayerClose> mLayerCloseList = new List<LayerClose>();
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("開始シ\u30fcン")]
    private SceneType START_SCENE;
    /// <summary>
    /// 
    /// </summary>
    [Header("FPS表示")]
    public bool displayFPS;
    /// <summary>
    /// 
    /// </summary>
    private bool nowJoyConConnection = true;
    /// <summary>
    /// 
    /// </summary>
    private bool prevJoyConConnection = true;
    /// <summary>
    /// 
    /// </summary>
    private bool isFade;
    /// <summary>
    /// 
    /// </summary>
    private Camera tempCamera;
    /// <summary>
    /// 
    /// </summary>
    private UniversalAdditionalCameraData tempCameraData;
    /// <summary>
    /// 
    /// </summary>
    private OverlayMainCamera overlayMainCamera;
    /// <summary>
    /// 
    /// </summary>
    public bool IsLoading {
        get;
        set;
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsFade {
        get {
            if (!isFade) {
                return mLoadingManager.GetFadeFlg();
            }
            return true;
        }
        set {
            isFade = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsModeSelectSetting {
        get;
        set;
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsAutoSave {
        get;
        set;
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsSceneChange {
        get;
        set;
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsMainSceneSetting {
        get;
        set;
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsMainSceneJoyConLost {
        get;
        set;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_act"></param>
    public void FadeExec(Action _act) {
        ChangeBackQuad();
        IsFade = true;
        mLoadingManager.STATE_FADE(LoadingManager.FADE_TYPE.OUT, _loadingflg: false, 0.5f);
        LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate {
            _act();
            IsFade = false;
            mLoadingManager.STATE_FADE(LoadingManager.FADE_TYPE.IN, _loadingflg: false, 0.5f);
        });
    }
    /// <summary>
    /// 
    /// </summary>
    private void Start() {
        if (Application.isEditor && START_SCENE != 0) {
            switch (START_SCENE) {
                case SceneType.GET_BALL:
                    SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.GET_BALL;
                    break;
                case SceneType.ARCHER_BATTLE:
                    SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.CANNON_SHOT;
                    break;
                case SceneType.BLOCK_WIPER:
                    SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.BLOCK_WIPER;
                    break;
                case SceneType.MOLE_HAMMER:
                    SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.MOLE_HAMMER;
                    break;
                case SceneType.BOMB_ROULETTE:
                    SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.BOMB_ROULETTE;
                    break;
                case SceneType.RECEIVE_PON:
                    SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.RECEIVE_PON;
                    break;
                case SceneType.BLACKSMITH:
                    SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.DELIVERY_ORDER;
                    break;
                case SceneType.CANNON_SHOT:
                    SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.ARCHER_BATTLE;
                    break;
                case SceneType.ATTACK_BALL:
                    SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.ATTACK_BALL;
                    break;
                case SceneType.BLOW_AWAY_TANK:
                    SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.BLOW_AWAY_TANK;
                    break;
            }
        }
        mSceneObject = new GameObject[(int)SceneType.MAX];
        mSceneObjectPref = new GameObject[(int)SceneType.MAX];
        mBeforLastScene = (mPrevScene = (mNowScene = (mNextScene = START_SCENE)));
        if (mNextScene != SceneType.MAX) {
            mLoadingManager.gameObject.SetActive(value: true);
            NextScene(mNextScene, LoadingManager.FADE_TYPE.IN_INIT);
        }
        QualitySettings.vSyncCount = 1;
        KeyBoardManager.Initialize();
        ChangeBackQuad();
    }
    /// <summary>
    /// 
    /// </summary>
    public override void Resume() {
        base.Resume();
    }
    /// <summary>
    /// 
    /// </summary>
    private void LateUpdate() {
        if (mLoadingManager.GetFadeFlg()) {
            return;
        }
        nowJoyConConnection = SingletonCustom<JoyConManager>.Instance.IsCheckConnection();
        if (!nowJoyConConnection) {
            UnityEngine.Debug.Log("コントロ\u30fcラ\u30fcサポ\u30fcトアプレット呼び出し");
            if (!SingletonCustom<JoyConManager>.Instance.ShowControllerSupport()) {
                SingletonCustom<DM>.Instance.Close();
                SingletonCustom<AudioManager>.Instance.SeStopCoroutine();
                switch (mNowScene) {
                    default:
                        IsMainSceneJoyConLost = true;
                        if (Time.timeScale < 1f) {
                            Time.timeScale = 1f;
                        }
                        NextScene(SceneType.MAIN);
                        break;
                    case SceneType.MAIN: {
                            Scene_Main component = mSceneObject[(int)mNowScene].GetComponent<Scene_Main>();
                            if (!component.IsPlayerSetting()) {
                                SingletonCustom<CommonNotificationManager>.Instance.Close();
                                component.FadeInit();
                                if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying("bgm_menu")) {
                                    SingletonCustom<AudioManager>.Instance.BgmStop();
                                    SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_menu", _loop: true);
                                }
                            }
                            break;
                        }
                    case SceneType.TITLE:
                        break;
                }
            }
        }
        prevJoyConConnection = nowJoyConConnection;
        if (mNowScene != 0 && mNowScene != SceneType.MAIN && mNowScene != SceneType.PLAY_KING_OP && mNowScene != SceneType.RESULT_ANNOUNCEMENT && mNowScene != SceneType.CHEER_BATTLE && mNowScene != SceneType.MAX && !IsLoading && !IsFade && !SingletonCustom<CommonNotificationManager>.Instance.IsOpen && !ResultGameDataParams.GetResultMode() && (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Start) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Back))) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenOperationInfoAtGamePause();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void HideLoadingText() {
        mLoadingManager.HideLoadingText();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_scene"></param>
    /// <param name="_type"></param>
    /// <param name="_NonLoadingScene"></param>
    /// <param name="_loadingDelay"></param>
    public void NextScene(SceneType _scene, LoadingManager.FADE_TYPE _type = LoadingManager.FADE_TYPE.OUT, bool _NonLoadingScene = false, float _loadingDelay = 0f) {
        if (!mLoadingManager.GetFadeFlg()) {
            ChangeBackQuad();
            mNextScene = _scene;
            if (mNextScene > SceneType.MAIN && mNextScene < SceneType.PLAY_KING_OP) {
                SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = (GS_Define.GameType)(mNextScene - 2);
                UnityEngine.Debug.Log("GameType:" + SingletonCustom<GameSettingManager>.Instance.LastSelectGameType.ToString());
            }
            Application.targetFrameRate = 60;
            if (mNextScene == SceneType.TITLE) {
                SingletonCustom<JoyConManager>.Instance.SetMappingType(JoyConManager.MappingType.Menu);
            } else {
                SingletonCustom<JoyConManager>.Instance.SetMappingType(JoyConManager.MappingType.MainGame);
            }
            if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE) {
                SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx = 0;
            }
            if ((bool)mSceneObject[(int)_scene] | _NonLoadingScene) {
                mLoadingManager.STATE_FADE(_type, _loadingflg: true, 0.5f, _loadingDelay);
            } else {
                mLoadingManager.STATE_FADE(_type, _loadingflg: true, 0.5f, _loadingDelay);
            }
            StateBaseClass.SetPauseFlg(StateBaseClass.PAUSE_LEVEL.GAME);
            IsFade = true;
            IsSceneChange = true;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void ChangeBackQuad() {
        SingletonCustom<GameSettingManager>.Instance.BackFrameTexIdx = UnityEngine.Random.Range(0, 2);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_scene"></param>
    public void DestroyScene(SceneType _scene = SceneType.MAX) {
        if (mSceneObject[(int)mNowScene] != null) {
            SceneType sceneType = mNowScene;
            if (sceneType < SceneType.MAX) {
                mSceneObject[(int)mNowScene].SetActive(value: false);
                UnityEngine.Object.Destroy(mSceneObject[(int)mNowScene]);
                mSceneObject[(int)mNowScene] = null;
                mSceneObjectPref[(int)mNowScene] = null;
                Resources.UnloadUnusedAssets();
                GC.Collect();
            } else {
                mSceneObject[(int)mNowScene].SetActive(value: false);
            }
            LightingSettings.ChangeDefaultLighting();
            ResultGameDataParams.SetResultMode(_isResult: false);
            ResultGameDataParams.ResetData();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_obj"></param>
    private void CheckCameraOverlay(GameObject _obj) {
        overlayMainCamera = _obj.GetComponent<OverlayMainCamera>();
        if (overlayMainCamera != null) {
            mainCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            tempCameraData = overlayMainCamera.BaseCamera.GetUniversalAdditionalCameraData();
            tempCameraData.cameraStack.Add(mainCamera);
        } else {
            mainCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Base;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_scene"></param>
    /// <returns></returns>
    public IEnumerator LoadScene(SceneType _scene = SceneType.MAX) {
        if (_scene == SceneType.MAX) {
            _scene = mNextScene;
        }
        SingletonCustom<GameSettingManager>.Instance.RemoveCpuToPlayerGroupList();
        mLayerCloseList.Clear();
        SingletonCustom<AudioManager>.Instance.SeStop();
        CancelInvoke();
        LeanTween.cancelAll();
        IsLoading = true;
        mBeforLastScene = mPrevScene;
        mPrevScene = mNowScene;
        mNowScene = mNextScene;
        ResultGameDataParams.SetNowSceneType(mNowScene);
        if (!SingletonCustom<GameSettingManager>.Instance.IsCpuFixSelect && SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE) {
            SingletonCustom<GameSettingManager>.Instance.ShuffleCpuCharacterSelect();
        }
        if (mSceneObject[(int)_scene] != null) {
            mSceneObject[(int)_scene].SetActive(value: true);
            CheckCameraOverlay(mSceneObject[(int)_scene]);
            if (mNowScene == SceneType.MAIN) {
                mSceneObject[(int)_scene].GetComponent<Scene_Main>().Repaint();
                if (!IsMainSceneSetting) {
                    if (mPrevScene == SceneType.TITLE) {
                        mSceneObject[(int)_scene].GetComponent<Scene_Main>().Init();
                    } else {
                        mSceneObject[(int)_scene].GetComponent<Scene_Main>().CloseDetail();
                    }
                } else {
                    mSceneObject[(int)_scene].GetComponent<Scene_Main>().UpdateData();
                }
                if (IsMainSceneJoyConLost) {
                    mSceneObject[(int)_scene].GetComponent<Scene_Main>().Init();
                }
            }
        } else {
            if (mSceneObjectPref[(int)_scene] == null) {
                ResourceRequest resReq = Resources.LoadAsync<GameObject>("Prefab/" + LOAD_SCENE_NAME[(int)_scene]);
                while (!resReq.isDone) {
                    yield return new WaitForEndOfFrame();
                }
                mSceneObjectPref[(int)_scene] = (resReq.asset as GameObject);
                if (!isLoadAocDLC) {
                    if (_scene == SceneType.TITLE) {
                        UnityEngine.Debug.Log("AOC読込開始");
                        SingletonCustom<AocAssetBundleManager>.Instance.LoadAoc();
                        while (!SingletonCustom<AocAssetBundleManager>.Instance.IsLoaded) {
                            yield return new WaitForEndOfFrame();
                        }
                        UnityEngine.Debug.Log("AOC読込完了");
                    }
                    isLoadAocDLC = true;
                }
            }
            int num = (int)_scene;
            UnityEngine.Debug.Log("シ\u30fcン:" + num.ToString());
            UnityEngine.Debug.Log("シ\u30fcン配列:" + mSceneObject.Length.ToString());
            mSceneObject[(int)_scene] = UnityEngine.Object.Instantiate(mSceneObjectPref[(int)_scene], base.transform.position, Quaternion.identity);
            mSceneObject[(int)_scene].transform.parent = base.transform;
            CheckCameraOverlay(mSceneObject[(int)_scene]);
            if (_scene == SceneType.MAIN) {
                mSceneObject[(int)_scene].transform.SetLocalPositionZ(100f);
                if (IsMainSceneJoyConLost) {
                    mSceneObject[(int)_scene].GetComponent<Scene_Main>().Init();
                }
                TouchPanelManager.Instance.SetTouchPanelEnable(false);
            }
            // If it not main scene then it probably game scene
            else {
                TouchPanelManager.Instance.SetTouchPanelEnable(true);
            }
        }
        SingletonCustom<CommonNotificationManager>.Instance.Close();
        if (IsAutoSave) {
            SingletonCustom<SaveDataManager>.Instance._Save();
            IsAutoSave = false;
        }
        IsLoading = false;
        IsMainSceneSetting = false;
        IsMainSceneJoyConLost = false;
        LightingSettings.SetFog();
        SingletonCustom<GameSettingManager>.Instance.DebugLogPlayerGroupList();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool GetFadeFlg() {
        return mLoadingManager.GetFadeFlg();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_scene"></param>
    /// <returns></returns>
    public bool CheckNowScene(SceneType _scene) {
        return mNowScene == _scene;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_scene"></param>
    /// <returns></returns>
    public bool CheckPrevScene(SceneType _scene) {
        return mPrevScene == _scene;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_scene"></param>
    /// <returns></returns>
    public bool CheckNextScene(SceneType _scene) {
        return mNextScene == _scene;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_scene"></param>
    /// <returns></returns>
    public bool CheckBeforePrevScene(SceneType _scene) {
        return mBeforLastScene == _scene;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public SceneType GetNowScene() {
        return mNowScene;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public SceneType GetPrevScene() {
        return mPrevScene;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public SceneType GetNextScene() {
        return mNextScene;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public SceneType GetBeforePrevScene() {
        return mBeforLastScene;
    }
    /// <summary>
    /// 
    /// </summary>
    public void CloseComplete() {
        mLayerCloseList.RemoveAt(mLayerCloseList.Count - 1);
    }
    /// <summary>
    /// 
    /// </summary>
    public void FadeClose() {
        IsFade = false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_callBack"></param>
    public void AddNowLayerCloseCallBack(LayerClose _callBack) {
        mLayerCloseList.Add(_callBack);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public LayerClose GetNowLayerCloseCallBack() {
        if (mLayerCloseList.Count == 0) {
            return null;
        }
        return mLayerCloseList[mLayerCloseList.Count - 1];
    }
}
