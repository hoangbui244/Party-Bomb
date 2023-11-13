using GamepadInput;
using Satbox.PlayerSetting;
using System;
using UnityEngine;
public class GS_PlayerSetting : MonoBehaviour {
    public enum State {
        PlayModeSelect,
        MultiSetting
    }
    [SerializeField]
    [Header("カ\u30fcソルマネ\u30fcジャ\u30fc")]
    private CursorManager cursor;
    [SerializeField]
    [Header("カ\u30fcソル：みんなで")]
    private CursorManager cursorMulti;
    [SerializeField]
    [Header("ひとりで")]
    private GameObject objSingle;
    [SerializeField]
    [Header("みんなで")]
    private GameObject objMulti;
    [SerializeField]
    [Header("みんなで：人数コントロ\u30fcラ\u30fc数設定")]
    private GameObject objMultiSetting;
    [SerializeField]
    [Header("みんなで：人数コントロ\u30fcラ\u30fc数設定：中央フレ\u30fcム")]
    private GameObject multiCenterFrame;
    [SerializeField]
    [Header("【ひとりで】拡縮ル\u30fcト")]
    private GameObject objSingleRoot;
    [SerializeField]
    [Header("【みんなで】拡縮ル\u30fcト")]
    private GameObject objMultiRoot;
    [SerializeField]
    [Header("アイコン拡縮値")]
    private float iconAnimScale;
    [SerializeField]
    [Header("アイコン拡縮時間")]
    private float iconAnimTime;
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fc表示フレ\u30fcム")]
    private GameObject objControllerFrame;
    [SerializeField]
    [Header("設定フレ\u30fcム")]
    private GameObject objSettingFrame;
    [SerializeField]
    [Header("モ\u30fcド説明画像")]
    private SpriteRenderer spInfoMode;
    [SerializeField]
    [Header("シングルキャラクタ\u30fc")]
    private CharacterManager singleCharacter;
    [SerializeField]
    [Header("マルチキャラクタ\u30fc")]
    private CharacterManager multiCharacter;
    [SerializeField]
    [Header("DLC1詳細ボタン")]
    [Header("DLC_1 ---------------------------------")]
    private GameObject dlc1DetailButton;
    private readonly string EXPLANATION_SPRITE_NAME = "_explanation_0";
    [SerializeField]
    [Header("追加コンテンツ表示")]
    private GS_AddOnContents aoc;
    [SerializeField]
    [Header("フリ\u30fcモ\u30fcドAボタンル\u30fcト")]
    private GameObject objRootButtonA;
    [SerializeField]
    [Header("フリ\u30fcモ\u30fcドBボタンル\u30fcト")]
    private GameObject objRootButtonB;
    private int playerIdx;
    private int controllerIdx;
    private bool isWaiting;
    private bool isDlcDetailOpenFlg;
    private int frameLockCnt;
    private State currentState;
    public void Show() {
        currentState = State.PlayModeSelect;
        cursor.IsStop = true;
        cursorMulti.IsStop = true;
        cursor.SetCursorPos(0, 0);
        cursorMulti.SetCursorPos(0, 0);
        singleCharacter.ResetAnimation(active: true);
        multiCharacter.ResetAnimation(active: false);
        LeanTween.cancel(objSingleRoot);
        LeanTween.cancel(objMultiRoot);
        objSingleRoot.transform.SetLocalScale(1f, 1f, 1f);
        objMultiRoot.transform.SetLocalScale(1f, 1f, 1f);
        LeanTween.scale(objSingleRoot, new Vector3(iconAnimScale, iconAnimScale, 1f), iconAnimTime).setEaseInOutQuad().setLoopPingPong();
        string text = EXPLANATION_SPRITE_NAME + (cursor.GetSelectNo() + 1).ToString();
        if (Localize_Define.Language != 0) {
            text = "en" + text;
        }
        spInfoMode.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.PlayerSetting, text);
        objSingle.SetActive(value: true);
        objMultiSetting.SetActive(value: false);
        objSingleRoot.SetActive(value: true);
        base.gameObject.SetActive(value: true);
        LeanTween.cancel(objSettingFrame);
        //LeanTween.cancel(objControllerFrame);
        //objControllerFrame.transform.SetLocalPositionX(1145f);
        //if (Localize_Define.Language == Localize_Define.LanguageType.English) {
        //    objRootButtonB.transform.SetLocalPositionX(-12f);
        //    objRootButtonA.transform.SetLocalPositionX(-23f);
        //}
        //LeanTween.moveLocalX(objControllerFrame, (Localize_Define.Language == Localize_Define.LanguageType.English) ? 695f : 618f, 0.55f).setEaseOutQuint();
        dlc1DetailButton.SetActive(value: true);
        dlc1DetailButton.transform.SetLocalPositionY(-150);
        LeanTween.cancel(dlc1DetailButton);
        LeanTween.moveLocalY(dlc1DetailButton, 45f, 1.05f).setEaseOutQuint().setDelay(0.1f);
        isDlcDetailOpenFlg = false;
        cursor.IsStop = false;
    }
    public void Stop() {
        cursor.IsStop = true;
    }
    public void Resume() {
        cursor.IsStop = false;
        frameLockCnt = 1;
    }
    public void Hide() {
        base.gameObject.SetActive(value: false);
    }
    private void Update() {
        if (!IsButtonInteractable()) {
            return;
        }
        if (cursor.IsPushMovedButtonMoment() || cursorMulti.IsPushMovedButtonMoment()) {
            OnMoveButtonDown();
        }
        else if (cursor.IsOkButton() || cursorMulti.IsOkButton()) {
            OnSelectButtonDown();
        }
        else if (cursor.IsReturnButton() || cursorMulti.IsReturnButton()) {
            OnReturnButtonDown();
        }
        else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Start) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Back)) {
            OpenSettingPanel();
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
        if (CommonNotificationManager.Instance.IsOpen) {
            return false;
        }
        if (GS_GameSelectManager.Instance.IsDLC1Release) {
            return false;
        }
        if (DM.Instance.IsActive()) {
            return false;
        }
        if (isWaiting) {
            return false;
        }
        if (cursor.IsStop && cursorMulti.IsStop) {
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
            case State.PlayModeSelect:
                int selectId = cursor.GetSelectNo();
                cursor.SetCursorPos(cursor.GetLayerNo(), selectId);
                LeanTween.cancel(objSingleRoot);
                LeanTween.cancel(objMultiRoot);
                objSingleRoot.transform.SetLocalScale(1f, 1f, 1f);
                objMultiRoot.transform.SetLocalScale(1f, 1f, 1f);
                switch (selectId) {
                    case 0:
                        LeanTween.scale(objSingleRoot, new Vector3(iconAnimScale, iconAnimScale, 1f), iconAnimTime).setEaseInOutQuad().setLoopPingPong();
                        break;
                    case 1:
                        LeanTween.scale(objMultiRoot, new Vector3(iconAnimScale, iconAnimScale, 1f), iconAnimTime).setEaseInOutQuad().setLoopPingPong();
                        break;
                }
                string text3 = EXPLANATION_SPRITE_NAME + (selectId + 1).ToString();
                if (Localize_Define.Language != 0) {
                    text3 = "en" + text3;
                }
                spInfoMode.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.PlayerSetting, text3);
                singleCharacter.SetActive(selectId == 0);
                multiCharacter.SetActive(selectId == 1);
                break;
            case State.MultiSetting:
                cursorMulti.SetCursorPos(cursorMulti.GetLayerNo(), cursorMulti.GetSelectNo());
                break;
        }
    }
    public void OnSelectButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        switch (currentState) {
            case State.PlayModeSelect:
                if (cursor.GetSelectNo() == 0) {
                    OnButtonDownSingle();
                }
                else {
                    OnButtonDownMulti();
                }
                break;
            case State.MultiSetting:
                // Select 2 players icon will result cursorMulti.GetSelectNo() to 0 (same as 3 -> 1, 4 -> 2,...)
                OnButtonDownMultiVariant(cursorMulti.GetSelectNo());
                break;
        }
    }
    public void OnButtonDownSingle() {
        if (!IsButtonInteractable()) {
            return;
        }
        cursor.SetCursorPos(cursor.GetLayerNo(), 0);
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        SingletonCustom<GameSettingManager>.Instance.PlayerNum = 1;
        SingletonCustom<GameSettingManager>.Instance.ControllerNum = 1;
        SingletonCustom<GameSettingManager>.Instance.AutoPlayerNumSetting();
        SingletonCustom<JoyConManager>.Instance.SetPlayMode(GameManager.PlayModeType.SINGLE);
        if (SingletonCustom<JoyConManager>.Instance.IsCheckConnection()) {
            SingletonCustom<GS_GameSelectManager>.Instance.OnPlayerSetting();
        }
    }
    public void OnButtonDownMulti() {
        if (!IsButtonInteractable()) {
            return;
        }
        cursor.SetCursorPos(cursor.GetLayerNo(), 1);
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        isWaiting = true;
        cursor.IsStop = true;
        playerIdx = 0;
        controllerIdx = 1;
        currentState = State.MultiSetting;
        LeanTween.cancel(objMultiRoot);
        objMultiRoot.transform.SetLocalScale(1f, 1f, 1f);
        isWaiting = false;
        cursorMulti.IsStop = false;
        objMultiSetting.SetActive(value: true);
        string text4 = EXPLANATION_SPRITE_NAME + "0";
        if (Localize_Define.Language != 0) {
            text4 = "en" + text4;
        }
        spInfoMode.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.PlayerSetting, text4);
    }
    public void OnButtonDownMultiVariant(int selectId) {
        if (!IsButtonInteractable()) {
            return;
        }
        cursorMulti.SetCursorPos(cursorMulti.GetLayerNo(), selectId);
        int selectPlayerCount = selectId + 2;
        AudioManager.Instance.SePlay("se_button_enter");
        int controllerCount = JoyConManager.Instance.GetControllerCount();
        GameSettingManager.Instance.PlayerNum = selectPlayerCount;
        // TODO: Not sure what is this for? (WTF?)
        #region WTF?
        playerIdx = selectPlayerCount - 2;
        controllerIdx = controllerCount;
        #endregion
        // Controller count (all types)
        GameSettingManager.Instance.ControllerNum = controllerCount;
        GameSettingManager.Instance.AutoPlayerNumSetting();
        // Play mode
        GameManager.PlayModeType playMode = selectPlayerCount switch {
            2 => GameManager.PlayModeType.JOYCON_TWO,
            3 => GameManager.PlayModeType.JOYCON_THREE,
            4 => GameManager.PlayModeType.JOYCON_FOUR,
            5 => GameManager.PlayModeType.JOYCON_FIVE,
            6 => GameManager.PlayModeType.JOYCON_SIX,
            _ => throw new ArgumentOutOfRangeException()
        };
        if (JoyConManager.Instance.CurrentPlayMode != playMode) {
            JoyConManager.Instance.SetPlayMode(playMode);
        }
        // We had enough controllers for selected mode
        if (selectPlayerCount == JoyConManager.Instance.GetControllerCount()) {
            GS_GameSelectManager.Instance.OnPlayerSetting();
            return;
        }
        string arg = controllerCount >= 2 ? "are" : "is";
        string arg2 = controllerCount >= 2 ? "controllers" : "controller";
        string text = Localize_Define.Language == Localize_Define.LanguageType.English
            ? "Insufficient number of controllers." + Environment.NewLine +
              $"Currently there {arg} {controllerCount} {arg2}" + Environment.NewLine +
              "connected." + Environment.NewLine +
              "*The keyboard/touch-screen" + Environment.NewLine +
              "can be used as " + controllerCount + "P."
            : "コントロ\u30fcラの数が足りません" + Environment.NewLine +
              $"現在接続されている" + Environment.NewLine +
              $"コントロ\u30fcラは{controllerCount}個です" + Environment.NewLine +
              "*The keyboard/touch-screen" + Environment.NewLine +
              "can be used as " + controllerCount + "P.";
        SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT, text, DM.PARAM_LIST.SHOW_BACK_FADE, true, DM.PARAM_LIST.CALL_BACK, DM.List(delegate { }));
    }
    public void OnReturnButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        switch (currentState) {
            case State.PlayModeSelect:
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
                SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.TITLE);
                break;
            case State.MultiSetting:
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
                isWaiting = true;
                objMultiSetting.SetActive(value: false);
                cursorMulti.IsStop = true;
                playerIdx = 0;
                controllerIdx = 0;
                currentState = State.PlayModeSelect;
                LeanTween.cancel(objMultiRoot);
                objMultiRoot.transform.SetLocalScale(1f, 1f, 1f);
                LeanTween.scale(objMultiRoot, new Vector3(iconAnimScale, iconAnimScale, 1f), iconAnimTime).setEaseInOutQuad().setLoopPingPong();
                cursor.IsStop = false;
                isWaiting = false;
                objSingleRoot.SetActive(value: true);
                string text2 = EXPLANATION_SPRITE_NAME + (cursor.GetSelectNo() + 1).ToString();
                if (Localize_Define.Language != 0) {
                    text2 = "en" + text2;
                }
                spInfoMode.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.PlayerSetting, text2);
                break;
        }
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
    public void OpenSettingPanel() {
        if (!IsButtonInteractable()) {
            return;
        }
        SingletonCustom<GS_Setting>.Instance.Open();
        cursor.IsStop = true;
        cursorMulti.IsStop = true;
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
    }
    public void OnSettingBack() {
        switch (currentState) {
            case State.PlayModeSelect:
                cursor.IsStop = false;
                break;
            case State.MultiSetting:
                cursorMulti.IsStop = false;
                break;
        }
        frameLockCnt = 1;
    }
    private void OnDestroy() {
        LeanTween.cancel(objSingleRoot);
        LeanTween.cancel(objMultiRoot);
        LeanTween.cancel(objControllerFrame);
        LeanTween.cancel(objSettingFrame);
        LeanTween.cancel(dlc1DetailButton);
    }
}