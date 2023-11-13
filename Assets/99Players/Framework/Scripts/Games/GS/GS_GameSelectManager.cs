using GamepadInput;
using SaveDataDefine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GS_GameSelectManager : SingletonCustom<GS_GameSelectManager> {
    public enum DLC_Layout {
        DLC1,
        DLC2,
        DLC1_2,
        DLC3,
        DLC1_2_3,
        DLC1_3,
        DLC2_3
    }
    [Serializable]
    public class ObjSetting {
        public GameObject[] objs;
    }
    public enum SelectType {
        Soro,
        Team
    }
    public float DLCButtonPositionY = 425f;
    private readonly int PAGE_IN_BUTTON_NUM = 6;
    [SerializeField]
    [Header("カ\u30fcソル参照先のゲ\u30fcム定義配列")]
    private GS_Define.GameType[] arrayCursorGameType;
    [SerializeField]
    [Header("カ\u30fcソル：プレイヤ\u30fc設定")]
    private CursorManager cursorPlayerSetting;
    [SerializeField]
    [Header("カ\u30fcソル：モ\u30fcドセレクト")]
    private CursorManager cursorModeSelect;
    [SerializeField]
    [Header("ゲ\u30fcムセレクトの設定ボタン配列")]
    private GameObject[] arrayObjSetting;
    [SerializeField]
    [Header("カ\u30fcソル：ゲ\u30fcムセレクト")]
    private CursorManager cursorGameSelect;
    [SerializeField]
    [Header("フリ\u30fcモ\u30fcド")]
    private GameObject objMapMenu;
    [SerializeField]
    [Header("フリ\u30fcモ\u30fcドプレハブ名")]
    private string objMapMenuName;
    [SerializeField]
    [Header("フリ\u30fcモ\u30fcドプレハブ名(DLC)")]
    private string[] arrayObjMapMenuNameDLC;
    [SerializeField]
    [Header("プレイヤ\u30fc設定クラス")]
    private GS_PlayerSetting playerSetting;
    [SerializeField]
    [Header("モ\u30fcドセレクトクラス")]
    private GS_ModeSelect modeSelect;
    [SerializeField]
    [Header("画面下部設定オブジェクト")]
    private GameObject objSetting;
    [SerializeField]
    [Header("画面下部コントロ\u30fcラ\u30fcオブジェクト")]
    private GameObject objController;
    [SerializeField]
    [Header("ゲ\u30fcム名テキスト")]
    private SpriteRenderer textGame;
    [SerializeField]
    [Header("ゲ\u30fcム説明（日本語）")]
    private TextMeshPro textGameInfo;
    [SerializeField]
    [Header("ゲ\u30fcムサムネイル")]
    private SpriteRenderer thumbnailGame;
    [SerializeField]
    [Header("説明オブジェクト")]
    private GameObject explanationCardObj;
    [SerializeField]
    [Header("DLC1購入ダイアログ")]
    private Dlc1ReleaseDialog dlc1ReleaseDialog;
    [SerializeField]
    [Header("DLC2購入ダイアログ")]
    private Dlc1ReleaseDialog dlc2ReleaseDialog;
    [SerializeField]
    [Header("DLC3購入ダイアログ")]
    private Dlc1ReleaseDialog dlc3ReleaseDialog;
    [SerializeField]
    [Header("追加コンテンツ表示")]
    private GS_AddOnContents aoc;
    [SerializeField]
    [Header("マップフェ\u30fcド対象画像")]
    private SpriteRenderer[] arrayMapObjFadeTarget;
    [SerializeField]
    [Header("スティック描画")]
    private SpriteRenderer rendererStick;
    [SerializeField]
    [Header("スティック画像")]
    private Sprite[] arraySpStick;
    [SerializeField]
    [Header("Aボタン描画")]
    private SpriteRenderer rendererButtonA;
    [SerializeField]
    [Header("Aボタン画像")]
    private Sprite[] arraySpButtonA;
    [SerializeField]
    [Header("Bボタン描画")]
    private SpriteRenderer rendererButtonB;
    [SerializeField]
    [Header("Bボタン画像")]
    private Sprite[] arraySpButtonB;
    [SerializeField]
    [Header("協力アイコン")]
    private GameObject objCoopIcon;
    [SerializeField]
    [Header("ハイスコア表示")]
    private GameObject objHiscore;
    [SerializeField]
    [Header("ハイスコアテキスト")]
    private TextMeshPro textHiscore;
    [SerializeField]
    [Header("パ\u30fcティセレクト5レイアウト")]
    private GS_PartySelect partySelectRed;
    [SerializeField]
    [Header("パ\u30fcティセレクト10レイアウト")]
    private GS_PartySelect partySelectBlue;
    [SerializeField]
    [Header("パ\u30fcティセレクト15レイアウト")]
    private GS_PartySelect partySelectYellow;
    [SerializeField]
    [Header("パ\u30fcティセレクト5レイアウトDLC1")]
    private GS_PartySelect partySelect5_DLC;
    [SerializeField]
    [Header("パ\u30fcティセレクト15レイアウトDLC1")]
    private GS_PartySelect partySelect15_DLC;
    [SerializeField]
    [Header("パ\u30fcティセレクト25レイアウトDLC1")]
    private GS_PartySelect partySelect25_DLC;
    [SerializeField]
    [Header("パ\u30fcティセレクト5レイアウトDLC2")]
    private GS_PartySelect partySelect5_DLC2;
    [SerializeField]
    [Header("パ\u30fcティセレクト15レイアウトDLC2")]
    private GS_PartySelect partySelect15_DLC2;
    [SerializeField]
    [Header("パ\u30fcティセレクト25レイアウトDLC2")]
    private GS_PartySelect partySelect25_DLC2;
    [SerializeField]
    [Header("パ\u30fcティセレクト5レイアウトDLC1_2")]
    private GS_PartySelect partySelect5_DLC1_2;
    [SerializeField]
    [Header("パ\u30fcティセレクト10レイアウトDLC1_2")]
    private GS_PartySelect partySelect10_DLC1_2;
    [SerializeField]
    [Header("パ\u30fcティセレクト15レイアウトDLC1_2")]
    private GS_PartySelect partySelect15_DLC1_2;
    [SerializeField]
    [Header("パ\u30fcティセレクト25レイアウトDLC1_2")]
    private GS_PartySelect partySelect25_DLC1_2;
    [SerializeField]
    [Header("パ\u30fcティセレクト35レイアウトDLC1_2")]
    private GS_PartySelect partySelect35_DLC1_2;
    [SerializeField]
    [Header("パ\u30fcティセレクト5レイアウトDLC3")]
    private GS_PartySelect partySelect5_DLC3;
    [SerializeField]
    [Header("パ\u30fcティセレクト15レイアウトDLC3")]
    private GS_PartySelect partySelect15_DLC3;
    [SerializeField]
    [Header("パ\u30fcティセレクト25レイアウトDLC3")]
    private GS_PartySelect partySelect25_DLC3;
    [SerializeField]
    [Header("パ\u30fcティセレクト5レイアウトDLC1_2_3")]
    private GS_PartySelect partySelect5_DLC1_2_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト10レイアウトDLC1_2_3")]
    private GS_PartySelect partySelect10_DLC1_2_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト15レイアウトDLC1_2_3")]
    private GS_PartySelect partySelect15_DLC1_2_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト25レイアウトDLC1_2_3")]
    private GS_PartySelect partySelect25_DLC1_2_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト35レイアウトDLC1_2_3")]
    private GS_PartySelect partySelect35_DLC1_2_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト41レイアウトDLC1_2_3")]
    private GS_PartySelect partySelect41_DLC1_2_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト5レイアウトDLC1_3")]
    private GS_PartySelect partySelect5_DLC1_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト10レイアウトDLC1_3")]
    private GS_PartySelect partySelect10_DLC1_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト15レイアウトDLC1_3")]
    private GS_PartySelect partySelect15_DLC1_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト25レイアウトDLC1_3")]
    private GS_PartySelect partySelect25_DLC1_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト35レイアウトDLC1_3")]
    private GS_PartySelect partySelect35_DLC1_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト5レイアウトDLC2_3")]
    private GS_PartySelect partySelect5_DLC2_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト10レイアウトDLC2_3")]
    private GS_PartySelect partySelect10_DLC2_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト15レイアウトDLC2_3")]
    private GS_PartySelect partySelect15_DLC2_3;
    [SerializeField]
    [Header("パ\u30fcティセレクト25レイアウトDLC2_3")]
    private GS_PartySelect partySelect25_DLC2_3;
    [SerializeField]
    [Header("フリ\u30fcモ\u30fcドAボタンル\u30fcト")]
    private GameObject objRootButtonA;
    [SerializeField]
    [Header("フリ\u30fcモ\u30fcドBボタンル\u30fcト")]
    private GameObject objRootButtonB;
    private readonly float STICK_NEUTRAL_TIME = 0.25f;
    private float stickNeutralTiem;
    private float buttonANeturalTime;
    private float buttonBNeturalTime;
    private SelectType selectType;
    private readonly float[] POS_TEAM_ICON_X = new float[8] {
        -346f,
        -252f,
        -158f,
        -64f,
        64f,
        158f,
        252f,
        346f
    };
    private readonly int TEAM_PLAYER_NUM = 4;
    private readonly float POS_DEFAULT_CONTROLLER = 618f;
    private readonly float POS_DEFAULT_SETTING = -812f;
    private readonly float POS_OUT_CONTROLLER = 1900f;
    private readonly float POS_OUT_SETTING = -1812f;
    private List<int>[] playerGroupList;
    private bool isCursorInit;
    private int frameLockCnt;
    private double debugAddTime;
    private DateTime dt;
    private int pageInIndex;
    private int pageScrollIdx;
    private bool isCharaMoveInput;
    private int BUTTON_NUM = 10;
    private int SCROLL_MAX = 4;
    private bool isMapFade;
    private GameObject instanceSelectMenu;
    [SerializeField]
    [Header("パ\u30fcティセレクト用のLocalizeManager")]
    private Localize_Manager localizeManager;
    private bool isStickInput;
    public bool IsPlayerSetting => playerSetting.gameObject.activeSelf;
    public bool IsDLC1Release {
        get {
            if (dlc1ReleaseDialog.gameObject.activeSelf) {
                return dlc1ReleaseDialog.IsOpen;
            }
            return false;
        }
    }
    public bool IsMapFade => isMapFade;
    public bool IsPartySelect {
        get {
            if (!modeSelect.gameObject.activeSelf) {
                return !playerSetting.gameObject.activeSelf;
            }
            return false;
        }
    }
    public GS_Define.GameType[] ArrayCursorGameType => arrayCursorGameType;
    public CursorManager CursorGameSelect => cursorGameSelect;
    public void Init() {
        DirectDetailClose();
        SetLayout();
        objMapMenu.SetActive(value: false);
        cursorPlayerSetting.IsStop = false;
        cursorModeSelect.IsStop = true;
        cursorGameSelect.IsStop = false;
        SingletonCustom<GS_Setting>.Instance.IsActive = false;
        playerSetting.Show();
        modeSelect.Hide();
        partySelectRed.Hide();
        partySelectBlue.Hide();
        partySelectYellow.Hide();
        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1) {
            selectType = SelectType.Soro;
        }
        UpdateGameMode();
        StartTeamAssignment();
        isCharaMoveInput = false;
        UpdateGameInfo();
        pageScrollIdx = 0;
        pageInIndex = 0;
        if (aoc.IsOpen) {
            aoc.DirectClose();
        }
        if (dlc1ReleaseDialog.IsOpen) {
            dlc1ReleaseDialog.DirectClose();
        }
        if (dlc2ReleaseDialog.IsOpen) {
            dlc2ReleaseDialog.DirectClose();
        }
        if (dlc3ReleaseDialog.IsOpen) {
            dlc3ReleaseDialog.DirectClose();
        }
        CheckReleaseDialog();
    }
    private void InitSetting() {
        if (SingletonCustom<SceneManager>.Instance.IsSceneChange && SingletonCustom<SceneManager>.Instance.GetPrevScene() != 0) {
            SingletonCustom<SceneManager>.Instance.IsSceneChange = false;
            OnModeSelect();
            if (SingletonCustom<GameSettingManager>.Instance.SelectFreeModeIdx != -1) {
                cursorGameSelect.SetSelectNo(SingletonCustom<GameSettingManager>.Instance.SelectFreeModeIdx);
                cursorGameSelect.SetCursorPos(0, SingletonCustom<GameSettingManager>.Instance.SelectFreeModeIdx);
            }
        }
        else {
            SingletonCustom<GameSettingManager>.Instance.SelectFreeModeIdx = -1;
            SingletonCustom<GameSettingManager>.Instance.SelectPartyModeIdx = -1;
            SingletonCustom<GameSettingManager>.Instance.SelectPartyNumIdx = -1;
        }
        objCoopIcon.gameObject.SetActive(CheckCoopGame(cursorGameSelect.GetSelectNo()));
        UpdateHiscore(cursorGameSelect.GetSelectNo());
        if (Localize_Define.Language == Localize_Define.LanguageType.English) {
            textGameInfo.gameObject.SetActive(value: false);
        }
        if (textGameInfo.gameObject.activeSelf) {
            textGameInfo.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, (int)(342 + arrayCursorGameType[cursorGameSelect.GetSelectNo()])));
            textGameInfo.transform.SetLocalPositionX(167.4f - textGameInfo.preferredWidth * 0.5f);
        }
    }
    private void SetLayout() {
        SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.CheckDLCGameSelectNum();
        if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            if (instanceSelectMenu == null) {
                instanceSelectMenu = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/FreeSelect/" + arrayObjMapMenuNameDLC[0]), base.transform);
            }
            GS_FreeSelect component = instanceSelectMenu.GetComponent<GS_FreeSelect>();
            objMapMenu = instanceSelectMenu;
            cursorGameSelect = component.cursorGameSelect;
            arrayObjSetting = component.arrayObjSetting_DLC;
            textGame = component.textGame;
            textGameInfo = component.textGameInfo;
            thumbnailGame = component.thumbnailGame;
            rendererStick = component.rendererStick;
            rendererButtonA = component.rendererButtonA;
            rendererButtonB = component.rendererButtonB;
            objCoopIcon = component.objCoopIcon;
            objHiscore = component.objHiscore;
            textHiscore = component.textHiscore;
            if (partySelectRed == null) {
                partySelectRed = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_5_DLC_1"), base.transform)).GetComponent<GS_PartySelect>();
                partySelectBlue = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_15_DLC_1"), base.transform)).GetComponent<GS_PartySelect>();
                partySelectYellow = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_25_DLC_1"), base.transform)).GetComponent<GS_PartySelect>();
            }
            objSetting = component.objSetting;
            objController = component.objController;
            objRootButtonA = component.objRootButtonA;
            objRootButtonB = component.objRootButtonB;
        }
        else if (!SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            if (instanceSelectMenu == null) {
                instanceSelectMenu = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/FreeSelect/" + arrayObjMapMenuNameDLC[1]), base.transform);
            }
            GS_FreeSelect component2 = instanceSelectMenu.GetComponent<GS_FreeSelect>();
            objMapMenu = instanceSelectMenu;
            cursorGameSelect = component2.cursorGameSelect;
            arrayObjSetting = component2.arrayObjSetting_DLC;
            textGame = component2.textGame;
            textGameInfo = component2.textGameInfo;
            thumbnailGame = component2.thumbnailGame;
            rendererStick = component2.rendererStick;
            rendererButtonA = component2.rendererButtonA;
            rendererButtonB = component2.rendererButtonB;
            objCoopIcon = component2.objCoopIcon;
            objHiscore = component2.objHiscore;
            textHiscore = component2.textHiscore;
            if (partySelectRed == null) {
                partySelectRed = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_5_DLC_2"), base.transform)).GetComponent<GS_PartySelect>();
                partySelectBlue = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_15_DLC_2"), base.transform)).GetComponent<GS_PartySelect>();
                partySelectYellow = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_25_DLC_2"), base.transform)).GetComponent<GS_PartySelect>();
            }
            objSetting = component2.objSetting;
            objController = component2.objController;
            objRootButtonA = component2.objRootButtonA;
            objRootButtonB = component2.objRootButtonB;
        }
        else if (!SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX) && SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            if (instanceSelectMenu == null) {
                instanceSelectMenu = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/FreeSelect/" + arrayObjMapMenuNameDLC[3]), base.transform);
            }
            GS_FreeSelect component3 = instanceSelectMenu.GetComponent<GS_FreeSelect>();
            objMapMenu = instanceSelectMenu;
            cursorGameSelect = component3.cursorGameSelect;
            arrayObjSetting = component3.arrayObjSetting_DLC;
            textGame = component3.textGame;
            textGameInfo = component3.textGameInfo;
            thumbnailGame = component3.thumbnailGame;
            rendererStick = component3.rendererStick;
            rendererButtonA = component3.rendererButtonA;
            rendererButtonB = component3.rendererButtonB;
            objCoopIcon = component3.objCoopIcon;
            objHiscore = component3.objHiscore;
            textHiscore = component3.textHiscore;
            if (partySelectRed == null) {
                partySelectRed = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_5_DLC_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelectBlue = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_15_DLC_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelectYellow = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_25_DLC_3"), base.transform)).GetComponent<GS_PartySelect>();
            }
            objSetting = component3.objSetting;
            objController = component3.objController;
            objRootButtonA = component3.objRootButtonA;
            objRootButtonB = component3.objRootButtonB;
        }
        else if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX) && SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            if (instanceSelectMenu == null) {
                instanceSelectMenu = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/FreeSelect/" + arrayObjMapMenuNameDLC[4]), base.transform);
            }
            GS_FreeSelect component4 = instanceSelectMenu.GetComponent<GS_FreeSelect>();
            objMapMenu = instanceSelectMenu;
            cursorGameSelect = component4.cursorGameSelect;
            arrayObjSetting = component4.arrayObjSetting_DLC;
            textGame = component4.textGame;
            textGameInfo = component4.textGameInfo;
            thumbnailGame = component4.thumbnailGame;
            rendererStick = component4.rendererStick;
            rendererButtonA = component4.rendererButtonA;
            rendererButtonB = component4.rendererButtonB;
            objCoopIcon = component4.objCoopIcon;
            objHiscore = component4.objHiscore;
            textHiscore = component4.textHiscore;
            if (partySelectRed == null) {
                partySelect5_DLC1_2_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_5_DLC_1_2_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect10_DLC1_2_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_10_DLC_1_2_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect15_DLC1_2_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_15_DLC_1_2_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect25_DLC1_2_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_25_DLC_1_2_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect35_DLC1_2_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_35_DLC_1_2_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect41_DLC1_2_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_41_DLC_1_2_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect5_DLC1_2_3.gameObject.SetActive(value: false);
                partySelect10_DLC1_2_3.gameObject.SetActive(value: false);
                partySelect15_DLC1_2_3.gameObject.SetActive(value: false);
                partySelect25_DLC1_2_3.gameObject.SetActive(value: false);
                partySelect35_DLC1_2_3.gameObject.SetActive(value: false);
                partySelect41_DLC1_2_3.gameObject.SetActive(value: false);
            }
            SystemData systemData = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData;
            switch (GS_Setting.GameSelectNum[systemData.gameSelectNumRed]) {
                case 5:
                    partySelectRed = partySelect5_DLC1_2_3;
                    break;
                case 10:
                    partySelectRed = partySelect10_DLC1_2_3;
                    break;
                case 15:
                    partySelectRed = partySelect15_DLC1_2_3;
                    break;
                case 25:
                    partySelectRed = partySelect25_DLC1_2_3;
                    break;
                case 35:
                    partySelectRed = partySelect35_DLC1_2_3;
                    break;
                case 41:
                    partySelectRed = partySelect41_DLC1_2_3;
                    break;
            }
            switch (GS_Setting.GameSelectNum[systemData.gameSelectNumBlue]) {
                case 5:
                    partySelectBlue = partySelect5_DLC1_2_3;
                    break;
                case 10:
                    partySelectBlue = partySelect10_DLC1_2_3;
                    break;
                case 15:
                    partySelectBlue = partySelect15_DLC1_2_3;
                    break;
                case 25:
                    partySelectBlue = partySelect25_DLC1_2_3;
                    break;
                case 35:
                    partySelectBlue = partySelect35_DLC1_2_3;
                    break;
                case 41:
                    partySelectBlue = partySelect41_DLC1_2_3;
                    break;
            }
            switch (GS_Setting.GameSelectNum[systemData.gameSelectNumYellow]) {
                case 5:
                    partySelectYellow = partySelect5_DLC1_2_3;
                    break;
                case 10:
                    partySelectYellow = partySelect10_DLC1_2_3;
                    break;
                case 15:
                    partySelectYellow = partySelect15_DLC1_2_3;
                    break;
                case 25:
                    partySelectYellow = partySelect25_DLC1_2_3;
                    break;
                case 35:
                    partySelectYellow = partySelect35_DLC1_2_3;
                    break;
                case 41:
                    partySelectYellow = partySelect41_DLC1_2_3;
                    break;
            }
            objSetting = component4.objSetting;
            objController = component4.objController;
            objRootButtonA = component4.objRootButtonA;
            objRootButtonB = component4.objRootButtonB;
        }
        else if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            if (instanceSelectMenu == null) {
                instanceSelectMenu = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/FreeSelect/" + arrayObjMapMenuNameDLC[2]), base.transform);
            }
            GS_FreeSelect component5 = instanceSelectMenu.GetComponent<GS_FreeSelect>();
            objMapMenu = instanceSelectMenu;
            cursorGameSelect = component5.cursorGameSelect;
            arrayObjSetting = component5.arrayObjSetting_DLC;
            textGame = component5.textGame;
            textGameInfo = component5.textGameInfo;
            thumbnailGame = component5.thumbnailGame;
            rendererStick = component5.rendererStick;
            rendererButtonA = component5.rendererButtonA;
            rendererButtonB = component5.rendererButtonB;
            objCoopIcon = component5.objCoopIcon;
            objHiscore = component5.objHiscore;
            textHiscore = component5.textHiscore;
            if (partySelectRed == null) {
                partySelect5_DLC1_2 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_5_DLC_1_2"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect10_DLC1_2 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_10_DLC_1_2"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect15_DLC1_2 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_15_DLC_1_2"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect25_DLC1_2 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_25_DLC_1_2"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect35_DLC1_2 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_35_DLC_1_2"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect5_DLC1_2.gameObject.SetActive(value: false);
                partySelect10_DLC1_2.gameObject.SetActive(value: false);
                partySelect15_DLC1_2.gameObject.SetActive(value: false);
                partySelect25_DLC1_2.gameObject.SetActive(value: false);
                partySelect35_DLC1_2.gameObject.SetActive(value: false);
            }
            SystemData systemData2 = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData;
            switch (GS_Setting.GameSelectNum[systemData2.gameSelectNumRed]) {
                case 5:
                    partySelectRed = partySelect5_DLC1_2;
                    break;
                case 10:
                    partySelectRed = partySelect10_DLC1_2;
                    break;
                case 15:
                    partySelectRed = partySelect15_DLC1_2;
                    break;
                case 25:
                    partySelectRed = partySelect25_DLC1_2;
                    break;
                case 35:
                    partySelectRed = partySelect35_DLC1_2;
                    break;
            }
            switch (GS_Setting.GameSelectNum[systemData2.gameSelectNumBlue]) {
                case 5:
                    partySelectBlue = partySelect5_DLC1_2;
                    break;
                case 10:
                    partySelectBlue = partySelect10_DLC1_2;
                    break;
                case 15:
                    partySelectBlue = partySelect15_DLC1_2;
                    break;
                case 25:
                    partySelectBlue = partySelect25_DLC1_2;
                    break;
                case 35:
                    partySelectBlue = partySelect35_DLC1_2;
                    break;
            }
            UnityEngine.Debug.Log("yellowCnt:" + GS_Setting.GameSelectNum[systemData2.gameSelectNumYellow].ToString());
            switch (GS_Setting.GameSelectNum[systemData2.gameSelectNumYellow]) {
                case 5:
                    partySelectYellow = partySelect5_DLC1_2;
                    break;
                case 10:
                    partySelectYellow = partySelect10_DLC1_2;
                    break;
                case 15:
                    partySelectYellow = partySelect15_DLC1_2;
                    break;
                case 25:
                    partySelectYellow = partySelect25_DLC1_2;
                    break;
                case 35:
                    partySelectYellow = partySelect35_DLC1_2;
                    break;
            }
            objSetting = component5.objSetting;
            objController = component5.objController;
            objRootButtonA = component5.objRootButtonA;
            objRootButtonB = component5.objRootButtonB;
        }
        else if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX) && SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            if (instanceSelectMenu == null) {
                instanceSelectMenu = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/FreeSelect/" + arrayObjMapMenuNameDLC[5]), base.transform);
            }
            GS_FreeSelect component6 = instanceSelectMenu.GetComponent<GS_FreeSelect>();
            objMapMenu = instanceSelectMenu;
            cursorGameSelect = component6.cursorGameSelect;
            arrayObjSetting = component6.arrayObjSetting_DLC;
            textGame = component6.textGame;
            textGameInfo = component6.textGameInfo;
            thumbnailGame = component6.thumbnailGame;
            rendererStick = component6.rendererStick;
            rendererButtonA = component6.rendererButtonA;
            rendererButtonB = component6.rendererButtonB;
            objCoopIcon = component6.objCoopIcon;
            objHiscore = component6.objHiscore;
            textHiscore = component6.textHiscore;
            if (partySelectRed == null) {
                partySelect5_DLC1_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_5_DLC_1_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect10_DLC1_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_10_DLC_1_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect15_DLC1_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_15_DLC_1_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect25_DLC1_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_25_DLC_1_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect35_DLC1_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_35_DLC_1_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect5_DLC1_3.gameObject.SetActive(value: false);
                partySelect10_DLC1_3.gameObject.SetActive(value: false);
                partySelect15_DLC1_3.gameObject.SetActive(value: false);
                partySelect25_DLC1_3.gameObject.SetActive(value: false);
                partySelect35_DLC1_3.gameObject.SetActive(value: false);
            }
            SystemData systemData3 = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData;
            switch (GS_Setting.GameSelectNum[systemData3.gameSelectNumRed]) {
                case 5:
                    partySelectRed = partySelect5_DLC1_3;
                    break;
                case 10:
                    partySelectRed = partySelect10_DLC1_3;
                    break;
                case 15:
                    partySelectRed = partySelect15_DLC1_3;
                    break;
                case 25:
                    partySelectRed = partySelect25_DLC1_3;
                    break;
                case 35:
                    partySelectRed = partySelect35_DLC1_3;
                    break;
            }
            switch (GS_Setting.GameSelectNum[systemData3.gameSelectNumBlue]) {
                case 5:
                    partySelectBlue = partySelect5_DLC1_3;
                    break;
                case 10:
                    partySelectBlue = partySelect10_DLC1_3;
                    break;
                case 15:
                    partySelectBlue = partySelect15_DLC1_3;
                    break;
                case 25:
                    partySelectBlue = partySelect25_DLC1_3;
                    break;
                case 35:
                    partySelectBlue = partySelect35_DLC1_3;
                    break;
            }
            switch (GS_Setting.GameSelectNum[systemData3.gameSelectNumYellow]) {
                case 5:
                    partySelectYellow = partySelect5_DLC1_3;
                    break;
                case 10:
                    partySelectYellow = partySelect10_DLC1_3;
                    break;
                case 15:
                    partySelectYellow = partySelect15_DLC1_3;
                    break;
                case 25:
                    partySelectYellow = partySelect25_DLC1_3;
                    break;
                case 35:
                    partySelectYellow = partySelect35_DLC1_3;
                    break;
            }
            objSetting = component6.objSetting;
            objController = component6.objController;
            objRootButtonA = component6.objRootButtonA;
            objRootButtonB = component6.objRootButtonB;
        }
        else if (!SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX) && SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            if (instanceSelectMenu == null) {
                instanceSelectMenu = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/FreeSelect/" + arrayObjMapMenuNameDLC[6]), base.transform);
            }
            GS_FreeSelect component7 = instanceSelectMenu.GetComponent<GS_FreeSelect>();
            objMapMenu = instanceSelectMenu;
            cursorGameSelect = component7.cursorGameSelect;
            arrayObjSetting = component7.arrayObjSetting_DLC;
            textGame = component7.textGame;
            textGameInfo = component7.textGameInfo;
            thumbnailGame = component7.thumbnailGame;
            rendererStick = component7.rendererStick;
            rendererButtonA = component7.rendererButtonA;
            rendererButtonB = component7.rendererButtonB;
            objCoopIcon = component7.objCoopIcon;
            objHiscore = component7.objHiscore;
            textHiscore = component7.textHiscore;
            if (partySelectRed == null) {
                partySelect5_DLC2_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_5_DLC_2_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect10_DLC2_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_10_DLC_2_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect15_DLC2_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_15_DLC_2_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect25_DLC2_3 = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_25_DLC_2_3"), base.transform)).GetComponent<GS_PartySelect>();
                partySelect5_DLC2_3.gameObject.SetActive(value: false);
                partySelect10_DLC2_3.gameObject.SetActive(value: false);
                partySelect15_DLC2_3.gameObject.SetActive(value: false);
                partySelect25_DLC2_3.gameObject.SetActive(value: false);
            }
            SystemData systemData4 = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData;
            switch (GS_Setting.GameSelectNum[systemData4.gameSelectNumRed]) {
                case 5:
                    partySelectRed = partySelect5_DLC2_3;
                    break;
                case 10:
                    partySelectRed = partySelect10_DLC2_3;
                    break;
                case 15:
                    partySelectRed = partySelect15_DLC2_3;
                    break;
                case 25:
                    partySelectRed = partySelect25_DLC2_3;
                    break;
            }
            switch (GS_Setting.GameSelectNum[systemData4.gameSelectNumBlue]) {
                case 5:
                    partySelectBlue = partySelect5_DLC2_3;
                    break;
                case 10:
                    partySelectBlue = partySelect10_DLC2_3;
                    break;
                case 15:
                    partySelectBlue = partySelect15_DLC2_3;
                    break;
                case 25:
                    partySelectBlue = partySelect25_DLC2_3;
                    break;
            }
            switch (GS_Setting.GameSelectNum[systemData4.gameSelectNumYellow]) {
                case 5:
                    partySelectYellow = partySelect5_DLC2_3;
                    break;
                case 10:
                    partySelectYellow = partySelect10_DLC2_3;
                    break;
                case 15:
                    partySelectYellow = partySelect15_DLC2_3;
                    break;
                case 25:
                    partySelectYellow = partySelect25_DLC2_3;
                    break;
            }
            objSetting = component7.objSetting;
            objController = component7.objController;
            objRootButtonA = component7.objRootButtonA;
            objRootButtonB = component7.objRootButtonB;
        }
        else {
            if (instanceSelectMenu == null) {
                instanceSelectMenu = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/FreeSelect/" + objMapMenuName), base.transform);
            }
            GS_FreeSelect component8 = instanceSelectMenu.GetComponent<GS_FreeSelect>();
            arrayObjSetting = component8.arrayObjSetting_DLC;
            objMapMenu = instanceSelectMenu;
            cursorGameSelect = component8.cursorGameSelect;
            textGame = component8.textGame;
            textGameInfo = component8.textGameInfo;
            thumbnailGame = component8.thumbnailGame;
            rendererStick = component8.rendererStick;
            rendererButtonA = component8.rendererButtonA;
            rendererButtonB = component8.rendererButtonB;
            objCoopIcon = component8.objCoopIcon;
            objHiscore = component8.objHiscore;
            textHiscore = component8.textHiscore;
            if (partySelectRed == null) {
                partySelectRed = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_5"), base.transform)).GetComponent<GS_PartySelect>();
                partySelectBlue = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_10"), base.transform)).GetComponent<GS_PartySelect>();
                partySelectYellow = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/PartySelect/GameSelect_Party_15"), base.transform)).GetComponent<GS_PartySelect>();
            }
            objSetting = component8.objSetting;
            objController = component8.objController;
            objRootButtonA = component8.objRootButtonA;
            objRootButtonB = component8.objRootButtonB;
        }
        if (Localize_Define.Language == Localize_Define.LanguageType.English) {
            localizeManager.Set();
        }
    }
    private void CheckReleaseDialog() {
        if (!CheckDlc1Dialog() && !CheckDlc2Dialog() && !CheckDlc3Dialog()) {
            InitSetting();
            playerSetting.Resume();
        }
    }
    public bool CheckDlc1Dialog() {
        if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && !SingletonCustom<SaveDataManager>.Instance.SaveData.isRelease_DLC1 && !modeSelect.gameObject.activeSelf) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.isRelease_DLC1 = true;
            SingletonCustom<AudioManager>.Instance.SePlay("se_result_win");
            objSetting.SetActive(value: false);
            objController.SetActive(value: false);
            playerSetting.Stop();
            dlc1ReleaseDialog.Show(delegate { CheckReleaseDialog(); });
            return true;
        }
        return false;
    }
    public bool CheckDlc2Dialog() {
        if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX) && !SingletonCustom<SaveDataManager>.Instance.SaveData.isRelease_DLC2 && !modeSelect.gameObject.activeSelf) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.isRelease_DLC2 = true;
            SingletonCustom<AudioManager>.Instance.SePlay("se_result_win");
            objSetting.SetActive(value: false);
            objController.SetActive(value: false);
            playerSetting.Stop();
            dlc2ReleaseDialog.Show(delegate { CheckReleaseDialog(); });
            return true;
        }
        return false;
    }
    public bool CheckDlc3Dialog() {
        if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX) && !SingletonCustom<SaveDataManager>.Instance.SaveData.isRelease_DLC3 && !modeSelect.gameObject.activeSelf) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.isRelease_DLC3 = true;
            SingletonCustom<AudioManager>.Instance.SePlay("se_result_win");
            objSetting.SetActive(value: false);
            objController.SetActive(value: false);
            playerSetting.Stop();
            dlc3ReleaseDialog.Show(delegate { CheckReleaseDialog(); });
            return true;
        }
        return false;
    }
    private void UpdateHiscore(int _idx) {
        _idx = (int)arrayCursorGameType[_idx];
        switch (_idx) {
            default:
                objHiscore.SetActive(value: false);
                break;
            case 12:
            case 16:
            case 23:
            case 47:
                objHiscore.SetActive(value: true);
                if (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup || SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.characterNumSetting == 1 || SingletonCustom<GameSettingManager>.Instance.PlayerNum > GS_Define.PLAYER_SMALL_MAX) {
                    if (SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.IsDataEmptySix((GS_Define.GameType)_idx)) {
                        textHiscore.SetText("----");
                    }
                    else {
                        textHiscore.SetText(SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.GetSix((GS_Define.GameType)_idx, RecordData.InitType.Time));
                    }
                }
                else if (SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.IsDataEmpty((GS_Define.GameType)_idx)) {
                    textHiscore.SetText("----");
                }
                else {
                    textHiscore.SetText(SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Get((GS_Define.GameType)_idx, RecordData.InitType.Time));
                }
                break;
            case 1:
            case 5:
            case 22:
            case 48:
            case 55:
            case 56:
            case 58:
            case 59:
                objHiscore.SetActive(value: true);
                if (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup || SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.characterNumSetting == 1 || SingletonCustom<GameSettingManager>.Instance.PlayerNum > GS_Define.PLAYER_SMALL_MAX) {
                    if (SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.IsDataEmptySix((GS_Define.GameType)_idx)) {
                        textHiscore.SetText("----");
                    }
                    else {
                        textHiscore.SetText(SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.GetSix((GS_Define.GameType)_idx, RecordData.InitType.Score));
                    }
                }
                else if (SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.IsDataEmpty((GS_Define.GameType)_idx)) {
                    textHiscore.SetText("----");
                }
                else {
                    textHiscore.SetText(SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Get((GS_Define.GameType)_idx, RecordData.InitType.Score));
                }
                break;
        }
    }
    public void FadeInit() {
        SingletonCustom<SceneManager>.Instance.FadeExec(delegate { Init(); });
    }
    public void SetModeSelect() {
        Init();
        OnPlayerSetting(_isReturn: true);
        SingletonCustom<AudioManager>.Instance.PlayTitleBgm(_isUpdate: false);
    }
    public void SetCharacterSelect() {
        Init();
        OnModeSelect();
    }
    public void OnPlayerSetting(bool _isReturn = false) {
        cursorPlayerSetting.IsStop = true;
        cursorModeSelect.IsStop = false;
        SingletonCustom<GameSettingManager>.Instance.AllocNpadId();
        playerSetting.Hide();
        modeSelect.Show(_isReturn);
        UpdateGameMode();
        StartTeamAssignment();
        isCursorInit = false;
    }
    public void OnModeSelect() {
        cursorModeSelect.IsStop = true;
        cursorGameSelect.IsStop = false;
        playerSetting.Hide();
        modeSelect.Hide();
        OnCharacterSelect();
        if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying("bgm_party_select")) {
            SingletonCustom<AudioManager>.Instance.BgmStop();
            SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_party_select", _loop: true);
        }
    }
    public void OnCharacterSelect() {
        cursorModeSelect.IsStop = true;
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType) {
            case GameSettingManager.GameProgressType.SINGLE:
                objMapMenu.SetActive(value: true);
                if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 1) {
                    arrayObjSetting[0].SetActive(value: false);
                    arrayObjSetting[1].SetActive(value: true);
                }
                else {
                    arrayObjSetting[0].SetActive(value: true);
                    arrayObjSetting[1].SetActive(value: false);
                }
                break;
            case GameSettingManager.GameProgressType.ALL_SPORTS: {
                int num = modeSelect.GetPartySelectIdx();
                UnityEngine.Debug.Log("GameSettingManager.Instance.SelectPartyNumIdx:" + SingletonCustom<GameSettingManager>.Instance.SelectPartyNumIdx.ToString());
                if (SingletonCustom<GameSettingManager>.Instance.SelectPartyNumIdx != -1) {
                    num = SingletonCustom<GameSettingManager>.Instance.SelectPartyNumIdx;
                }
                UnityEngine.Debug.Log("idx>>>>>>>>>>>>>>>>>>>:" + num.ToString());
                switch (num) {
                    case 0:
                        partySelectRed.Show();
                        break;
                    case 1:
                        partySelectBlue.Show();
                        break;
                    case 2:
                        partySelectYellow.Show();
                        break;
                }
                SingletonCustom<GameSettingManager>.Instance.SelectPartyNumIdx = num;
                break;
            }
        }
        SetGameSelectLimit(_isCursorSet: true);
        StartCoroutine(_DealyCall());
        if (!SingletonCustom<SaveDataManager>.Instance.SaveData.isTalkGameOpen) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.isTalkGameOpen = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenTalk(SingletonCustom<TextDataBaseManager>.Instance.GetTalkData(TalkDataTable.TalkType.TALK_FIRST_CONTACT), delegate { frameLockCnt = 1; });
        }
        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1) {
            selectType = SelectType.Soro;
        }
        UpdateGameMode();
        StartTeamAssignment();
        UpdateCollection();
    }
    private void UpdateCollection() {
    }
    private void SetGameSelectLimit(bool _isCursorSet) {
        if (isCursorInit) {
            _isCursorSet = false;
        }
        else if (_isCursorSet) {
            _isCursorSet = true;
        }
        UpdateGameInfo();
        if (SingletonCustom<SaveDataManager>.Instance.SaveData.isFirstGamePlayed && SingletonCustom<SaveDataManager>.Instance.SaveData.isTalkGameOpen && !SingletonCustom<SaveDataManager>.Instance.SaveData.isTalkAllGameRelease) {
            StartCoroutine(_DealyTalk());
        }
    }
    private IEnumerator _DealyTalk() {
        yield return new WaitForEndOfFrame();
        SingletonCustom<SaveDataManager>.Instance.SaveData.isTalkAllGameRelease = true;
        SingletonCustom<CommonNotificationManager>.Instance.OpenTalk(SingletonCustom<TextDataBaseManager>.Instance.GetTalkData(TalkDataTable.TalkType.TALK_GAME_RELEASE), delegate { frameLockCnt = 1; });
    }
    private IEnumerator _DealyCall() {
        yield return new WaitForEndOfFrame();
        SetBottomUIEase();
    }
    private void StartTeamAssignment() {
        if (!SingletonCustom<GameSettingManager>.Instance.CheckTeamAssignment()) {
            SingletonCustom<GameSettingManager>.Instance.AutoPlayerNumSetting();
        }
        playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
    }
    public void Select(int _idx) {
        if (_idx == -1) {
            return;
        }
        UnityEngine.Debug.Log("_idx:" + _idx.ToString());
        UnityEngine.Debug.Log("gameType:" + arrayCursorGameType[_idx].ToString());
        _idx = (int)arrayCursorGameType[_idx];
        UnityEngine.Debug.Log("select:" + _idx.ToString());
        switch (selectType) {
            case SelectType.Soro:
                for (int j = 0; j < playerGroupList.Length; j++) {
                    playerGroupList[j].Clear();
                }
                for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerNum; k++) {
                    playerGroupList[k].Add(k);
                }
                break;
            case SelectType.Team:
                for (int i = 0; i < playerGroupList.Length; i++) {
                    playerGroupList[i].Clear();
                }
                if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4 && GetLeftTeamNum() == 2) {
                    SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.BATTLE_AND_COOP;
                }
                break;
        }
        SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.GET_BALL;
        switch (selectType) {
            case SelectType.Soro:
                SingletonCustom<GameSettingManager>.Instance.TeamNum = Mathf.Clamp(SingletonCustom<GameSettingManager>.Instance.PlayerNum, 2, 6);
                SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.BATTLE;
                break;
            case SelectType.Team:
                SingletonCustom<GameSettingManager>.Instance.TeamNum = 2;
                if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4 && playerGroupList[0].Count == 2) {
                    SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.BATTLE_AND_COOP;
                }
                else {
                    SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.COOP;
                }
                break;
        }
        UnityEngine.Debug.Log("★selectType:" + SingletonCustom<GameSettingManager>.Instance.TeamNum.ToString());
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_enter");
        GS_Define.GameType gameType = (GS_Define.GameType)_idx;
        UnityEngine.Debug.Log("load GS_Define.GameType:" + gameType.ToString());
        SingletonCustom<GameSettingManager>.Instance.SelectFreeModeIdx = cursorGameSelect.GetSelectNo();
        SingletonCustom<SceneManager>.Instance.NextScene((SceneManager.SceneType)(_idx + 2));
    }
    public void OnDetailBack() {
        frameLockCnt = 1;
        SetBottomUIEase(_isInputWait: false);
    }
    public void DetailUpdateData() {
    }
    public void DirectDetailClose() {
    }
    public void OnSettingBack() {
        SetLayout();
        if (modeSelect.gameObject.activeSelf) {
            modeSelect.OnSettingBack();
            return;
        }
        if (playerSetting.gameObject.activeSelf) {
            playerSetting.OnSettingBack();
            return;
        }
        UnityEngine.Debug.Log("red:" + partySelectRed.gameObject.activeSelf.ToString());
        if (partySelectRed.gameObject.activeSelf) {
            partySelectRed.OnSettingBack();
            return;
        }
        UnityEngine.Debug.Log("blue:" + partySelectBlue.gameObject.activeSelf.ToString());
        if (partySelectBlue.gameObject.activeSelf) {
            partySelectBlue.OnSettingBack();
            return;
        }
        UnityEngine.Debug.Log("yellow:" + partySelectYellow.gameObject.activeSelf.ToString());
        if (partySelectYellow.gameObject.activeSelf) {
            partySelectYellow.OnSettingBack();
            return;
        }
        cursorGameSelect.IsStop = false;
        frameLockCnt = 1;
        objMapMenu.SetActive(value: true);
        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 1) {
            arrayObjSetting[0].SetActive(value: false);
            arrayObjSetting[1].SetActive(value: true);
        }
        else {
            arrayObjSetting[0].SetActive(value: true);
            arrayObjSetting[1].SetActive(value: false);
        }
        UpdateGameMode();
        StartTeamAssignment();
        SingletonCustom<GameSettingManager>.Instance.AutoPlayerNumSetting();
        UpdateHiscore(cursorGameSelect.GetSelectNo());
        thumbnailGame.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetThumbnail(cursorGameSelect.GetSelectNo());
    }
    public DateTime GetDateTime() {
        dt = DateTime.Now;
        dt = dt.AddMinutes(debugAddTime);
        return dt;
    }
    public void Refresh() {
    }
    public void Repaint() {
        SetGameSelectLimit(_isCursorSet: false);
    }
    private new void OnEnable() {
        base.OnEnable();
        UpdateCollection();
        UpdateHiscore(cursorGameSelect.GetSelectNo());
        debugAddTime = 0.0;
        if (SingletonCustom<SceneManager>.Instance.IsModeSelectSetting) {
            SetModeSelect();
            SingletonCustom<SceneManager>.Instance.IsModeSelectSetting = false;
        }
        SingletonCustom<GameSettingManager>.Instance.RemoveCpuToPlayerGroupList();
        LeanTween.value(430f, 425f, 1f).setEaseInOutQuad().setLoopPingPong()
            .setOnUpdate(delegate(float _value) { DLCButtonPositionY = _value; });
        SetBottomUIEase();
        UpdateGameMode(_inputUpdate: true);
        StartTeamAssignment();
    }
    private void Start() {
        UpdateGameInfo();
    }
    private void OnMapInputChange() {
        frameLockCnt++;
        isStickInput = true;
    }
    private void Update() {
        if (stickNeutralTiem > 0f) {
            stickNeutralTiem -= Time.deltaTime;
            if (stickNeutralTiem <= 0f) {
                rendererStick.sprite = arraySpStick[0];
            }
        }
        if (buttonANeturalTime > 0f) {
            buttonANeturalTime -= Time.deltaTime;
            if (buttonANeturalTime <= 0f) {
                rendererButtonA.sprite = arraySpButtonA[0];
            }
        }
        if (buttonBNeturalTime > 0f) {
            buttonBNeturalTime -= Time.deltaTime;
            if (buttonBNeturalTime <= 0f) {
                rendererButtonB.sprite = arraySpButtonB[0];
            }
        }
        if (!dlc1ReleaseDialog.IsOpen && !aoc.IsOpen && !dlc2ReleaseDialog.IsOpen && !playerSetting.gameObject.activeSelf && !SingletonCustom<GS_Setting>.Instance.IsActive && !modeSelect.gameObject.activeSelf && !isMapFade) {
            int frameLockCnt2 = frameLockCnt;
        }
    }
    public Vector3 GetMoveDir(int _no) {
        float num = 0f;
        float num2 = 0f;
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
        JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
        num = axisInput.Stick_L.x;
        num2 = axisInput.Stick_L.y;
        return (Vector2)new Vector3(num, num2);
    }
    private void LateUpdate() {
        if (!IsButtonInteractable()) {
            return;
        }
        if (cursorGameSelect.IsPushMovedButtonMoment()) {
            OnMoveButtonDown();
        }
        if (cursorGameSelect.IsOkButton()) {
            OnSelectButtonDown();
        }
        else if (cursorGameSelect.IsReturnButton()) {
            OnReturnButtonDown();
        }
        else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Start) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Back)) {
            cursorGameSelect.IsStop = true;
            SingletonCustom<GS_Setting>.Instance.Open();
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        }
        else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Y)) {
            SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = SingletonCustom<GS_GameSelectManager>.Instance.ArrayCursorGameType[cursorGameSelect.GetSelectNo()];
            cursorGameSelect.IsStop = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenOperationInfoAtGameSelect();
            frameLockCnt = 1;
            SingletonCustom<AudioManager>.Instance.SePlay("se_pause_open");
        }
        else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X)) {
            OnRandomGameButtonDown();
        }
        else if ((SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.RightShoulder) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.RightTrigger)) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX)) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
            SingletonCustom<SceneManager>.Instance.FadeExec(delegate {
                aoc.Show(delegate {
                    OnDetailBack();
                    frameLockCnt = 1;
                });
                cursorGameSelect.IsStop = true;
                objSetting.SetActive(value: false);
                objController.SetActive(value: false);
                frameLockCnt = 1;
            });
        }
    }
    private bool IsButtonInteractable() {
        if (dlc1ReleaseDialog.IsOpen) {
            return false;
        }
        if (frameLockCnt > 0) {
            frameLockCnt--;
            if (frameLockCnt == 0) {
                cursorGameSelect.IsStop = false;
            }
            return false;
        }
        if (SceneManager.Instance.IsFade) {
            return false;
        }
        if (CommonNotificationManager.Instance.IsOpen) {
            return false;
        }
        if (DM.Instance.IsActive()) {
            return false;
        }
        if (isMapFade) {
            return false;
        }
        if (playerSetting.gameObject.activeSelf) {
            return false;
        }
        if (modeSelect.gameObject.activeSelf) {
            return false;
        }
        if (partySelectRed.gameObject.activeSelf) {
            return false;
        }
        if (partySelectBlue.gameObject.activeSelf) {
            return false;
        }
        if (partySelectYellow.gameObject.activeSelf) {
            return false;
        }
        if (SingletonCustom<GS_Setting>.Instance.IsActive) {
            return false;
        }
        if (aoc.IsOpen) {
            return false;
        }
        return true;
    }
    public void OnMoveButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        if (cursorGameSelect.IsPushMovedButtonMoment(CursorManager.MoveDir.TOP)) {
            rendererStick.sprite = arraySpStick[1];
        }
        else if (cursorGameSelect.IsPushMovedButtonMoment(CursorManager.MoveDir.DOWN)) {
            rendererStick.sprite = arraySpStick[2];
        }
        else if (cursorGameSelect.IsPushMovedButtonMoment(CursorManager.MoveDir.LEFT)) {
            rendererStick.sprite = arraySpStick[3];
        }
        else if (cursorGameSelect.IsPushMovedButtonMoment(CursorManager.MoveDir.RIGHT)) {
            rendererStick.sprite = arraySpStick[4];
        }
        if (stickNeutralTiem <= 0f) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_stick_move", _loop: false, 0f, 1f, 1f, 0.05f);
        }
        stickNeutralTiem = STICK_NEUTRAL_TIME;
        textGame.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetGameTitle(cursorGameSelect.GetSelectNo());
        thumbnailGame.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetThumbnail(cursorGameSelect.GetSelectNo());
        objCoopIcon.gameObject.SetActive(CheckCoopGame(cursorGameSelect.GetSelectNo()));
        if (textGameInfo.gameObject.activeSelf) {
            textGameInfo.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, (int)(342 + arrayCursorGameType[cursorGameSelect.GetSelectNo()])));
            textGameInfo.transform.SetLocalPositionX(167.4f - textGameInfo.preferredWidth * 0.5f);
        }
        UpdateHiscore(cursorGameSelect.GetSelectNo());
    }
    public void OnSelectButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        if (selectType == SelectType.Team && SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4 && CheckPlayerGroupSetting(_isShowDialog: true)) {
            return;
        }
        buttonANeturalTime = STICK_NEUTRAL_TIME;
        rendererButtonA.sprite = arraySpButtonA[1];
#if UNITY_EDITOR
        //BKB force loading Jackal game (65) to testing
        Select(60);
#else
        Select(cursorGameSelect.GetSelectNo());
#endif
    }
    public void OnReturnButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
        SingletonCustom<SceneManager>.Instance.FadeExec(delegate { SetModeSelect(); });
        buttonBNeturalTime = STICK_NEUTRAL_TIME;
        rendererButtonB.sprite = arraySpButtonB[1];
    }
    public void OnRandomGameButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        int num = UnityEngine.Random.Range(0, arrayCursorGameType.Length);
        UnityEngine.Debug.Log("設定:" + Localize_Define.Language.ToString());
        while (CheckDisableGame(num)) {
            num = UnityEngine.Random.Range(0, arrayCursorGameType.Length);
        }
        cursorGameSelect.SetSelectNo(num);
        cursorGameSelect.SetCursorPos(0, num);
        buttonANeturalTime = STICK_NEUTRAL_TIME;
        rendererButtonA.sprite = arraySpButtonA[1];
        Select(cursorGameSelect.GetSelectNo());
        UpdateGameInfo();
        objCoopIcon.gameObject.SetActive(CheckCoopGame(cursorGameSelect.GetSelectNo()));
        if (textGameInfo.gameObject.activeSelf) {
            textGameInfo.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, (int)(342 + arrayCursorGameType[cursorGameSelect.GetSelectNo()])));
            textGameInfo.transform.SetLocalPositionX(167.4f - textGameInfo.preferredWidth * 0.5f);
        }
    }
    public bool CheckDisableGame(int _idx) {
        _idx = (int)arrayCursorGameType[_idx];
        GS_Define.GameType gameType = (GS_Define.GameType)_idx;
        if (gameType == GS_Define.GameType.DELIVERY_ORDER || gameType == GS_Define.GameType.TRAP_RACE) {
            return true;
        }
        if (!SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX)) {
            gameType = (GS_Define.GameType)_idx;
            if ((uint)(gameType - 30) <= 9u) {
                return true;
            }
        }
        if (!SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX)) {
            gameType = (GS_Define.GameType)_idx;
            if ((uint)(gameType - 40) <= 9u) {
                return true;
            }
        }
        if (!SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            gameType = (GS_Define.GameType)_idx;
            if ((uint)(gameType - 50) <= 9u) {
                return true;
            }
        }
        return false;
    }
    public bool CheckCoopGame(int _idx) {
        _idx = (int)arrayCursorGameType[_idx];
        switch (_idx) {
            case 1:
            case 5:
            case 11:
            case 12:
            case 15:
            case 16:
            case 22:
            case 23:
            case 24:
            case 46:
            case 47:
            case 48:
            case 49:
            case 55:
            case 56:
            case 58:
            case 59:
                return true;
            default:
                return false;
        }
    }
    public void BackModeSelect() {
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
        SingletonCustom<SceneManager>.Instance.FadeExec(delegate { SetModeSelect(); });
    }
    private void UpdateGameMode(bool _inputUpdate = false) {
        switch (selectType) {
            case SelectType.Soro:
                SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.BATTLE;
                break;
            case SelectType.Team:
                SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.COOP;
                break;
        }
        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1) {
            selectType = SelectType.Soro;
        }
        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum != 4 || selectType != SelectType.Team || SingletonCustom<GameSettingManager>.Instance.IsSingleController) {
        }
    }
    public void UpdateGameInfo() {
        textGame.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetGameTitle(cursorGameSelect.GetSelectNo());
        thumbnailGame.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetThumbnail(cursorGameSelect.GetSelectNo());
    }
    private void SetBottomUIEase(bool _isInputWait = true) {
        objSetting.SetActive(value: true);
        objController.SetActive(value: true);
        LeanTween.cancel(objSetting);
        LeanTween.cancel(objController);
        objController.transform.SetLocalPositionX(POS_OUT_CONTROLLER);
        objSetting.transform.SetLocalPositionX(POS_OUT_SETTING);
        if (isMapFade) {
            LeanTween.moveLocalX(objController, 695f, 1.05f).setEaseOutQuint().setDelay(1f);
            objRootButtonB.transform.SetLocalPositionX(-12f);
            objRootButtonA.transform.SetLocalPositionX(-23f);
            LeanTween.moveLocalX(objSetting, POS_DEFAULT_SETTING, 1.05f).setEaseOutQuint().setDelay(1f);
        }
        else {
            LeanTween.moveLocalX(objController, 695f, 1.05f).setEaseOutQuint();
            objRootButtonB.transform.SetLocalPositionX(-12f);
            objRootButtonA.transform.SetLocalPositionX(-23f);
            LeanTween.moveLocalX(objSetting, POS_DEFAULT_SETTING, 1.05f).setEaseOutQuint();
        }
    }
    private bool CheckPlayerGroupSetting(bool _isShowDialog) {
        if (GetLeftTeamNum() != 2 && GetLeftTeamNum() != 4) {
            if (_isShowDialog) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 61), DM.PARAM_LIST.SHOW_BACK_FADE, true, DM.PARAM_LIST.DIALOG_SIZE_H, 380f, DM.PARAM_LIST.DIALOG_SIZE_W, 630f, DM.PARAM_LIST.CALL_BACK, DM.List(delegate { }), DM.PARAM_LIST.BUTTON_TEXT, DM.List(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 56), SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 57)));
            }
            return true;
        }
        return false;
    }
    private int GetRightTeamNum() {
        CalcManager.mCalcInt = 0;
        return CalcManager.mCalcInt;
    }
    private int GetLeftTeamNum() {
        CalcManager.mCalcInt = 0;
        return CalcManager.mCalcInt;
    }
    private new void OnDestroy() {
        base.OnDestroy();
        LeanTween.cancel(base.gameObject);
        LeanTween.cancel(objSetting);
        LeanTween.cancel(objController);
        if (instanceSelectMenu != null) {
            UnityEngine.Object.Destroy(instanceSelectMenu);
            instanceSelectMenu = null;
        }
    }
}