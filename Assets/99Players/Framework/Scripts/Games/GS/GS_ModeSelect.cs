using GamepadInput;
using Satbox.ModeSelet;
using SaveDataDefine;
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public class GS_ModeSelect : MonoBehaviour {
    /// <summary>
    /// 
    /// </summary>
    public enum State {
        /// <summary>
        /// 
        /// </summary>
        ModeSelect,
        /// <summary>
        /// 
        /// </summary>
        PartySetting
    }
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("カ\u30fcソルマネ\u30fcジャ\u30fc")]
    private CursorManager cursor;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("パ\u30fcティ設定カ\u30fcソル")]
    private CursorManager cursorParty;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("チ\u30fcム分け管理クラス")]
    private GS_TeamAssignment teamAssignment;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("パ\u30fcティ設定オブジェクト")]
    private GameObject objPartySetting;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("プレイヤ\u30fcアイコン")]
    private GameObject[] teamPlayerIcon;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("ゲ\u30fcムモ\u30fcドテキスト")]
    private SpriteRenderer[] arrayGameModeText;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("大運動会モ\u30fcドボタン")]
    private GameObject objSportsDayButton;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("4人設定レイアウト")]
    private GameObject objTeamSetting;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("モ\u30fcド説明画像")]
    private SpriteRenderer spInfoMode;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("大運動会モ\u30fcド拡縮ル\u30fcト")]
    private GameObject objSportsDayRoot;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("フリ\u30fc競技モ\u30fcド拡縮ル\u30fcト")]
    private GameObject objFreeRoot;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("アイコン拡縮値")]
    private float iconAnimScale;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("アイコン拡縮時間")]
    private float iconAnimTime;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("大運動会モ\u30fcド内ゲ\u30fcムモ\u30fcド選択ル\u30fcト")]
    private GameObject objSportsDayGameModeRoot;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("大運動会モ\u30fcドアイコン")]
    private SpriteRenderer[] arraSpSportsDayIcon;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("設定フレ\u30fcム")]
    private GameObject objSettingFrame;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fc表示フレ\u30fcム")]
    private GameObject objControllerFrame;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("Free Mode画面")]
    private DisplayManager freeModeDisplay;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("Party Mode画面")]
    private DisplayManager partyModeDisplay;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("DLC1詳細ボタン")]
    [Header("DLC_1 ---------------------------------")]
    private GameObject dlc1DetailButton;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("追加コンテンツ表示")]
    private GS_AddOnContents aoc;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("ゲ\u30fcム数画像（白）")]
    private SpriteRenderer[] arrayGameNumRendererWhite;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("ゲ\u30fcム数画像（グレ\u30fc）")]
    private SpriteRenderer[] arrayGameNumRendererGray;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("ゲ\u30fcム数画像差分")]
    private Sprite[] arraySpGameNum;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("ゲ\u30fcム数表示ル\u30fcト")]
    private GameObject[] arrayGameNum;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("フリ\u30fcモ\u30fcドAボタンル\u30fcト")]
    private GameObject objRootButtonA;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("フリ\u30fcモ\u30fcドBボタンル\u30fcト")]
    private GameObject objRootButtonB;
    /// <summary>
    /// 
    /// </summary>
    private readonly string EXPLANATION_SPRITE_NAME_FREE = "_explanation_free";
    /// <summary>
    /// 
    /// </summary>
    private readonly string EXPLANATION_SPRITE_NAME_PARTY = "_explanation_party_0";
    /// <summary>
    /// 
    /// </summary>
    private readonly float[] POS_TEAM_ICON_X = new float[8] {
        -352f,
        -258f,
        -164f,
        -70f,
        70f,
        164f,
        258f,
        352f
    };
    /// <summary>
    /// 
    /// </summary>
    private readonly int TEAM_PLAYER_NUM = 4;
    /// <summary>
    /// 
    /// </summary>
    private List<int>[] playerGroupList;
    /// <summary>
    /// 
    /// </summary>
    private State currentState;
    /// <summary>
    /// 
    /// </summary>
    private bool isWait;
    /// <summary>
    /// 
    /// </summary>
    private int frameLockCnt;
    /// <summary>
    /// 
    /// </summary>
    private bool isDlcDetailOpenFlg;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_isReturn"></param>
    public void Show(bool _isReturn) {
        UnityEngine.Debug.Log("================show::" + _isReturn.ToString());
        currentState = State.ModeSelect;
        SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.BATTLE;
        cursor.SetCursorPos(0, _isReturn ? ((SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.ALL_SPORTS) ? 1 : 0) : 0);
        cursor.IsStop = true;
        freeModeDisplay.SetActive(cursor.GetSelectNo() == 0);
        partyModeDisplay.SetActive(cursor.GetSelectNo() == 1);
        cursorParty.IsStop = false;
        string text = EXPLANATION_SPRITE_NAME_FREE;
        if (Localize_Define.Language != 0) {
            text = "en" + text;
        }
        spInfoMode.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ModeSelect, text);
        LeanTween.cancel(objSportsDayRoot);
        LeanTween.cancel(objFreeRoot);
        objSportsDayRoot.transform.SetLocalScale(1f, 1f, 1f);
        objFreeRoot.transform.SetLocalScale(1f, 1f, 1f);
        LeanTween.scale(objSportsDayRoot, new Vector3(iconAnimScale, iconAnimScale, 1f), iconAnimTime).setEaseInOutQuad().setLoopPingPong();
        base.gameObject.SetActive(value: true);
        isWait = false;
        LeanTween.cancel(objSportsDayButton);
        objFreeRoot.SetActive(value: true);
        objTeamSetting.SetActive(value: false);
        teamAssignment.SetDisable();
        StartTeamAssignment();
        UpdateGameModeDisp();
        SingletonCustom<GS_Setting>.Instance.IsActive = false;
        if (!SingletonCustom<GameSettingManager>.Instance.IsSingleController || SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
            for (int i = 0; i < arraSpSportsDayIcon.Length; i++) {
                arraSpSportsDayIcon[i].color = Color.white;
            }
        }
        else {
            for (int j = 0; j < arraSpSportsDayIcon.Length; j++) {
                arraSpSportsDayIcon[j].color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }
        //LeanTween.cancel(objControllerFrame);
        //objControllerFrame.transform.SetLocalPositionX(1145f);
        //if (Localize_Define.Language == Localize_Define.LanguageType.English) {
        //    objRootButtonB.transform.SetLocalPositionX(-12f);
        //    objRootButtonA.transform.SetLocalPositionX(-23f);
        //}
        //LeanTween.moveLocalX(objControllerFrame, (Localize_Define.Language == Localize_Define.LanguageType.English) ? 695f : 618f, 0.55f).setEaseOutQuint();
        frameLockCnt = 1;
        dlc1DetailButton.SetActive(value: true);
        dlc1DetailButton.transform.SetLocalPositionY(-700f);
        LeanTween.cancel(dlc1DetailButton);
        LeanTween.moveLocalY(dlc1DetailButton, -501f, 1.05f).setEaseOutQuint().setDelay(0.1f);
        SetGameNum();
        isDlcDetailOpenFlg = false;
        cursor.IsStop = false;
        objPartySetting.SetActive(value: false);
        if (SingletonCustom<GameSettingManager>.Instance.SelectPartyNumIdx != -1) {
            cursorParty.SetSelectNo(SingletonCustom<GameSettingManager>.Instance.SelectPartyNumIdx);
            cursorParty.SetCursorPos(0, SingletonCustom<GameSettingManager>.Instance.SelectPartyNumIdx);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void OnEnable() {
        string text = "";
        LeanTween.cancel(objSportsDayRoot);
        LeanTween.cancel(objFreeRoot);
        objSportsDayRoot.transform.SetLocalScale(1f, 1f, 1f);
        objFreeRoot.transform.SetLocalScale(1f, 1f, 1f);
        switch (cursor.GetSelectNo()) {
            case 0:
                text = EXPLANATION_SPRITE_NAME_FREE;
                if (Localize_Define.Language != 0) {
                    text = "en" + text;
                }
                spInfoMode.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ModeSelect, text);
                LeanTween.scale(objSportsDayRoot, new Vector3(iconAnimScale, iconAnimScale, 1f), iconAnimTime).setEaseInOutQuad().setLoopPingPong();
                break;
            case 1:
                text = EXPLANATION_SPRITE_NAME_PARTY + "0";
                if (Localize_Define.Language != 0) {
                    text = "en" + text;
                }
                spInfoMode.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ModeSelect, text);
                LeanTween.scale(objFreeRoot, new Vector3(iconAnimScale, iconAnimScale, 1f), iconAnimTime).setEaseInOutQuad().setLoopPingPong();
                break;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void Hide() {
        base.gameObject.SetActive(value: false);
    }
    /// <summary>
    /// 
    /// </summary>
    private void ChangeGameMode() {
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.COOP;
                if (!SingletonCustom<GameSettingManager>.Instance.CheckTeamAssignment()) {
                    SingletonCustom<GameSettingManager>.Instance.AutoPlayerNumSetting();
                }
                isWait = true;
                if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4) {
                    LeanTween.moveLocalY(objSportsDayGameModeRoot, -19f, 0.2f).setEaseOutQuad().setOnComplete((Action)delegate {
                        teamAssignment.SetEnable();
                        isWait = false;
                    });
                }
                else {
                    isWait = false;
                }
                break;
            case GS_Define.GameFormat.COOP:
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                teamAssignment.SetDisable();
                SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.BATTLE;
                isWait = true;
                if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4) {
                    LeanTween.moveLocalY(objSportsDayGameModeRoot, -135f, 0.2f).setEaseOutQuad().setOnComplete((Action)delegate { isWait = false; });
                }
                else {
                    isWait = false;
                }
                break;
        }
        UpdateGameModeDisp();
    }
    /// <summary>
    /// 
    /// </summary>
    private void UpdateGameModeDisp() {
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                arrayGameModeText[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, "_m_Individual_match_s");
                arrayGameModeText[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, "_m_team_battle");
                if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2) {
                    arrayGameModeText[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, "_m_team_battle_02");
                }
                else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3) {
                    arrayGameModeText[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, "_m_team_battle_03");
                }
                else {
                    arrayGameModeText[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, "_m_team_battle");
                }
                break;
            case GS_Define.GameFormat.COOP:
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                arrayGameModeText[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, "_m_Individual_match");
                arrayGameModeText[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, "_m_team_battle_s");
                if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2) {
                    arrayGameModeText[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, "_m_team_battle_s_02");
                }
                else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3) {
                    arrayGameModeText[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, "_m_team_battle_s_03");
                }
                else {
                    arrayGameModeText[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, "_m_team_battle_s");
                }
                break;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void StartTeamAssignment() {
    }
    /// <summary>
    /// 
    /// </summary>
    public void OnSettingBack() {
        switch (currentState) {
            case State.ModeSelect:
                cursor.IsStop = false;
                break;
            case State.PartySetting:
                cursorParty.IsStop = false;
                break;
        }
        frameLockCnt = 1;
        SetGameNum();
    }
    /// <summary>
    /// 
    /// </summary>
    private void SetGameNum() {
        if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            arrayGameNumRendererWhite[0].sprite = arraySpGameNum[0];
            arrayGameNumRendererWhite[1].sprite = arraySpGameNum[2];
            arrayGameNumRendererWhite[2].sprite = arraySpGameNum[3];
            arrayGameNumRendererGray[0].sprite = arraySpGameNum[0];
            arrayGameNumRendererGray[1].sprite = arraySpGameNum[2];
            arrayGameNumRendererGray[2].sprite = arraySpGameNum[3];
        }
        else if (!SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            arrayGameNumRendererWhite[0].sprite = arraySpGameNum[0];
            arrayGameNumRendererWhite[1].sprite = arraySpGameNum[2];
            arrayGameNumRendererWhite[2].sprite = arraySpGameNum[3];
            arrayGameNumRendererGray[0].sprite = arraySpGameNum[0];
            arrayGameNumRendererGray[1].sprite = arraySpGameNum[2];
            arrayGameNumRendererGray[2].sprite = arraySpGameNum[3];
        }
        else if (!SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX) && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX) && SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            arrayGameNumRendererWhite[0].sprite = arraySpGameNum[0];
            arrayGameNumRendererWhite[1].sprite = arraySpGameNum[2];
            arrayGameNumRendererWhite[2].sprite = arraySpGameNum[3];
            arrayGameNumRendererGray[0].sprite = arraySpGameNum[0];
            arrayGameNumRendererGray[1].sprite = arraySpGameNum[2];
            arrayGameNumRendererGray[2].sprite = arraySpGameNum[3];
        }
        else if (SingletonCustom<AocAssetBundleManager>.Instance.GetDlcCount() >= 2) {
            SystemData systemData = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData;
            SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.CheckDLCGameSelectNum();
            arrayGameNumRendererWhite[0].sprite = arraySpGameNum[systemData.gameSelectNumRed];
            arrayGameNumRendererWhite[1].sprite = arraySpGameNum[systemData.gameSelectNumBlue];
            arrayGameNumRendererWhite[2].sprite = arraySpGameNum[systemData.gameSelectNumYellow];
            arrayGameNumRendererGray[0].sprite = arraySpGameNum[systemData.gameSelectNumRed];
            arrayGameNumRendererGray[1].sprite = arraySpGameNum[systemData.gameSelectNumBlue];
            arrayGameNumRendererGray[2].sprite = arraySpGameNum[systemData.gameSelectNumYellow];
        }
        for (int i = 0; i < arrayGameNum.Length; i++) {
            arrayGameNum[i].gameObject.SetActive(i != cursorParty.GetSelectNo());
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void Update() {
        if (!IsButtonInteractable()) {
            return;
        }
        if (cursor.IsPushMovedButtonMoment() || cursorParty.IsPushMovedButtonMoment()) {
            OnMoveButtonDown();
        }
        else if (cursor.IsOkButton() || cursorParty.IsOkButton()) {
            OnSelectButtonDown();
        }
        else if (cursor.IsReturnButton() || cursor.IsReturnButton()) {
            OnReturnButtonDown();
        }
        else if (JoyConManager.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Start) || JoyConManager.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Back)) {
            OnEscapeButtonDown();
        }
        else if (dlc1DetailButton.activeSelf && (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.RightShoulder) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.RightTrigger))) {
            OnDlcButtonDown();
        }
    }
    private bool IsButtonInteractable() {
        if (isDlcDetailOpenFlg) {
            return false;
        }
        if (frameLockCnt > 0) {
            frameLockCnt--;
            return false;
        }
        if (SceneManager.Instance.IsFade) {
            return false;
        }
        if (SceneManager.Instance.IsLoading) {
            return false;
        }
        if (CommonNotificationManager.Instance.IsOpen) {
            return false;
        }
        if (DM.Instance.IsActive()) {
            return false;
        }
        if (isWait) {
            return false;
        }
        if (cursor.IsStop && GS_Setting.Instance.IsActive) {
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
        switch (currentState) {
            case State.ModeSelect: {
                OnMoveButtonDownModeSelect(cursor.GetSelectNo());
                break;
            }
            case State.PartySetting: {
                OnMoveButtonDownPartyVariant(cursorParty.GetSelectNo());
                break;
            }
        }
    }
    public void OnSelectButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        switch (currentState) {
            case State.ModeSelect:
                if (cursor.GetSelectNo() == 0) {
                    OnButtonDownFreeMode();
                }
                else {
                    OnButtonDownPartyMode();
                }
                break;
            case State.PartySetting:
                OnSelectButtonDownPartyVariant(cursorParty.GetSelectNo());
                break;
        }
    }
    public void OnReturnButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        switch (currentState) {
            case State.ModeSelect: {
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
                SingletonCustom<GS_GameSelectManager>.Instance.Init();
                Hide();
                break;
            }
            case State.PartySetting: {
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
                cursor.IsStop = false;
                cursorParty.IsStop = true;
                currentState = State.ModeSelect;
                objPartySetting.SetActive(value: false);
                objFreeRoot.transform.SetLocalScale(1f, 1f, 1f);
                LeanTween.scale(objFreeRoot, new Vector3(iconAnimScale, iconAnimScale, 1f), iconAnimTime).setEaseInOutQuad().setLoopPingPong();
                string text = EXPLANATION_SPRITE_NAME_PARTY + "0";
                if (Localize_Define.Language != 0) {
                    text = "en" + text;
                }
                spInfoMode.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ModeSelect, text);
                LeanTween.scale(objFreeRoot, new Vector3(iconAnimScale, iconAnimScale, 1f), iconAnimTime).setEaseInOutQuad().setLoopPingPong();
                break;
            }
        }
    }
    public void OnEscapeButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        SingletonCustom<GS_Setting>.Instance.Open();
        cursor.IsStop = true;
        cursorParty.IsStop = true;
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
    }
    public void OnMoveButtonDownModeSelect(int selectId) {
        if (!IsButtonInteractable()) {
            return;
        }
        cursor.SetCursorPos(cursor.GetLayerNo(), selectId);
        string text2 = "";
        switch (cursor.GetSelectNo()) {
            case 0:
                text2 = EXPLANATION_SPRITE_NAME_FREE;
                if (Localize_Define.Language != 0) {
                    text2 = "en" + text2;
                }
                spInfoMode.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ModeSelect, text2);
                break;
            case 1:
                text2 = EXPLANATION_SPRITE_NAME_PARTY + "0";
                if (Localize_Define.Language != 0) {
                    text2 = "en" + text2;
                }
                spInfoMode.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ModeSelect, text2);
                break;
        }
        LeanTween.cancel(objSportsDayRoot);
        LeanTween.cancel(objFreeRoot);
        objSportsDayRoot.transform.SetLocalScale(1f, 1f, 1f);
        objFreeRoot.transform.SetLocalScale(1f, 1f, 1f);
        switch (cursor.GetSelectNo()) {
            case 0:
                LeanTween.scale(objSportsDayRoot, new Vector3(iconAnimScale, iconAnimScale, 1f), iconAnimTime).setEaseInOutQuad().setLoopPingPong();
                break;
            case 1:
                LeanTween.scale(objFreeRoot, new Vector3(iconAnimScale, iconAnimScale, 1f), iconAnimTime).setEaseInOutQuad().setLoopPingPong();
                break;
        }
        freeModeDisplay.SetActive(cursor.GetSelectNo() == 0);
        partyModeDisplay.SetActive(cursor.GetSelectNo() == 1);
    }
    public void OnButtonDownFreeMode() {
        if (!IsButtonInteractable()) {
            return;
        }
        OnMoveButtonDownModeSelect(0);
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType = GameSettingManager.GameProgressType.SINGLE;
        SingletonCustom<GameSettingManager>.Instance.AutoPlayerNumSetting();
        SingletonCustom<SceneManager>.Instance.FadeExec(delegate { SingletonCustom<GS_GameSelectManager>.Instance.OnModeSelect(); });
    }
    public void OnButtonDownPartyMode() {
        if (!IsButtonInteractable()) {
            return;
        }
        OnMoveButtonDownModeSelect(1);
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        currentState = State.PartySetting;
        cursor.IsStop = true;
        cursorParty.IsStop = false;
        objPartySetting.SetActive(value: true);
        LeanTween.cancel(objFreeRoot);
        string text3 = EXPLANATION_SPRITE_NAME_PARTY + "1";
        if (Localize_Define.Language != 0) {
            text3 = "en" + text3;
        }
        spInfoMode.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ModeSelect, text3);
    }
    public void OnMoveButtonDownPartyVariant(int selectId) {
        if (!IsButtonInteractable()) {
            return;
        }
        cursorParty.SetCursorPos(cursorParty.GetLayerNo(), selectId);
        for (int i = 0; i < arrayGameNum.Length; i++) {
            arrayGameNum[i].gameObject.SetActive(i != selectId);
        }
    }
    public void OnSelectButtonDownPartyVariant(int selectId) {
        if (!IsButtonInteractable()) {
            return;
        }
        OnMoveButtonDownPartyVariant(selectId);
        bool isDlc1Enable = AocAssetBundleManager.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX);
        bool isDlc2Enable = AocAssetBundleManager.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX);
        bool isDlc3Enable = AocAssetBundleManager.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX);
        if (isDlc1Enable && !isDlc2Enable && !isDlc3Enable) {
            switch (selectId) {
                case 0:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 5;
                    break;
                case 1:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 15;
                    break;
                case 2:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 25;
                    break;
            }
        }
        else if (!isDlc1Enable && isDlc2Enable && !isDlc3Enable) {
            switch (selectId) {
                case 0:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 5;
                    break;
                case 1:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 15;
                    break;
                case 2:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 25;
                    break;
            }
        }
        else if (!isDlc1Enable && !isDlc2Enable && isDlc3Enable) {
            switch (selectId) {
                case 0:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 5;
                    break;
                case 1:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 15;
                    break;
                case 2:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 25;
                    break;
            }
        }
        else if (SingletonCustom<AocAssetBundleManager>.Instance.GetDlcCount() >= 2) {
            SystemData systemData = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData;
            switch (selectId) {
                case 0:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = GS_Setting.GameSelectNum[systemData.gameSelectNumRed];
                    break;
                case 1:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = GS_Setting.GameSelectNum[systemData.gameSelectNumBlue];
                    break;
                case 2:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = GS_Setting.GameSelectNum[systemData.gameSelectNumYellow];
                    break;
            }
        }
        else {
            switch (selectId) {
                case 0:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 5;
                    break;
                case 1:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 10;
                    break;
                case 2:
                    SingletonCustom<GameSettingManager>.Instance.SelectGameNum = 15;
                    break;
            }
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType = GameSettingManager.GameProgressType.ALL_SPORTS;
        SingletonCustom<GameSettingManager>.Instance.AutoPlayerNumSetting();
        SingletonCustom<SceneManager>.Instance.FadeExec(delegate { SingletonCustom<GS_GameSelectManager>.Instance.OnModeSelect(); });
    }
    public void OnDlcButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        SingletonCustom<SceneManager>.Instance.FadeExec(delegate {
            aoc.Show(delegate {
                isDlcDetailOpenFlg = false;
                cursor.IsStop = false;
                frameLockCnt = 1;
            });
            cursor.IsStop = true;
            isDlcDetailOpenFlg = true;
        });
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected bool IsLeftButton() {
        if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Dpad_Left)) {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected bool IsRightButton() {
        if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Dpad_Right)) {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetPartySelectIdx() {
        return cursorParty.GetSelectNo();
    }
    /// <summary>
    /// 
    /// </summary>
    public void StartPlayKing() {
        for (int i = 0; i < playerGroupList.Length; i++) {
            playerGroupList[i].Clear();
        }
        SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.BATTLE;
        for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerNum; j++) {
            playerGroupList[j].Add(j);
        }
        SingletonCustom<GameSettingManager>.Instance.TeamNum = 4;
        UnityEngine.Debug.Log("★selectType:" + SingletonCustom<GameSettingManager>.Instance.TeamNum.ToString());
        SingletonCustom<GameSettingManager>.Instance.PlayerGroupList = playerGroupList;
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_enter");
        SingletonCustom<SceneManager>.Instance.FadeExec(delegate { SingletonCustom<GS_GameSelectManager>.Instance.OnModeSelect(); });
        SingletonCustom<GameSettingManager>.Instance.InitSportsDay();
        SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType = GameSettingManager.GameProgressType.ALL_SPORTS;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_isShowDialog"></param>
    /// <returns></returns>
    private bool CheckPlayerGroupSetting(bool _isShowDialog) {
        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4 && GetLeftTeamNum() != 2 && GetLeftTeamNum() != 4) {
            if (_isShowDialog) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 61), DM.PARAM_LIST.SHOW_BACK_FADE, true, DM.PARAM_LIST.DIALOG_SIZE_H, 380f, DM.PARAM_LIST.DIALOG_SIZE_W, 630f, DM.PARAM_LIST.CALL_BACK, DM.List(delegate { }), DM.PARAM_LIST.BUTTON_TEXT, DM.List(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 56), SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 57)));
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private int GetRightTeamNum() {
        CalcManager.mCalcInt = 0;
        for (int i = 0; i < teamPlayerIcon.Length; i++) {
            if (teamPlayerIcon[i].activeSelf && teamPlayerIcon[i].transform.localPosition.x > 0f) {
                CalcManager.mCalcInt++;
            }
        }
        return CalcManager.mCalcInt;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private int GetLeftTeamNum() {
        CalcManager.mCalcInt = 0;
        for (int i = 0; i < teamPlayerIcon.Length; i++) {
            if (teamPlayerIcon[i].activeSelf && teamPlayerIcon[i].transform.localPosition.x < 0f) {
                CalcManager.mCalcInt++;
            }
        }
        return CalcManager.mCalcInt;
    }
    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy() {
        LeanTween.cancel(objSportsDayButton);
        LeanTween.cancel(objSportsDayRoot);
        LeanTween.cancel(objFreeRoot);
        LeanTween.cancel(objSportsDayGameModeRoot);
        LeanTween.cancel(objControllerFrame);
        LeanTween.cancel(dlc1DetailButton);
    }
}