using GamepadInput;
using System.Collections.Generic;
using UnityEngine;
public class GS_PartySelect : MonoBehaviour {
    [SerializeField]
    [Header("ボタンクラス")]
    private GS_PartySelectButton partySelectButton;
    private CursorManager cursor;
    [SerializeField]
    [Header("ゲ\u30fcム名表示")]
    private SpriteRenderer renderGameTitle;
    [SerializeField]
    [Header("設定ボタン配列")]
    private GameObject[] arrayObjSetting;
    [SerializeField]
    [Header("ゲ\u30fcムサムネイル表示")]
    private SpriteRenderer renderGameThumbnail;
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
    [Header("選択ゲ\u30fcムボタン")]
    private SpriteRenderer[] arraySelectGameButton;
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fc表示")]
    private GameObject objController;
    [SerializeField]
    [Header("ランダム選択")]
    private GameObject objRandom;
    private readonly float STICK_NEUTRAL_TIME = 0.25f;
    private float stickNeutralTiem;
    private float buttonANeturalTime;
    private float buttonBNeturalTime;
    private List<int>[] playerGroupList;
    private List<int> listSelectGameNo = new List<int>();
    private int frameLockCnt;
    public CursorManager Cursor => cursor;
    public void Show() {
        UnityEngine.Debug.Log("Show===============>>");
        base.gameObject.SetActive(value: true);
        listSelectGameNo.Clear();
        playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
        cursor = partySelectButton.GetCursorManager();
        if (SingletonCustom<GameSettingManager>.Instance.SelectPartyModeIdx != -1) {
            cursor.SetSelectNo(SingletonCustom<GameSettingManager>.Instance.SelectPartyModeIdx);
            cursor.SetCursorPos(0, SingletonCustom<GameSettingManager>.Instance.SelectPartyModeIdx);
        }
        else {
            cursor.SetSelectNo(0);
            cursor.SetCursorPos(0, 0);
        }
        for (int i = 0; i < partySelectButton.GetSelectFadeLen(); i++) {
            partySelectButton.GetSelectFade(i).SetActive(value: false);
        }
        for (int j = 0; j < partySelectButton.GetRenderSelectNumberLen(); j++) {
            partySelectButton.GetRenderSelectNumber(j).gameObject.SetActive(value: false);
        }
        for (int k = 0; k < arraySelectGameButton.Length; k++) {
            arraySelectGameButton[k].sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetEmptyButton(k);
        }
        renderGameTitle.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetGameTitle(cursor.GetSelectNo());
        //renderGameTitle.transform.SetLocalPositionX(-496.6f + renderGameTitle.bounds.size.x * 0.5f);
        renderGameThumbnail.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetThumbnail(cursor.GetSelectNo());
        //objRandom.transform.SetLocalPositionX(-1900f);
        //LeanTween.moveLocalX(objRandom, -812f, 1.05f).setEaseOutQuint();
        //objController.transform.SetLocalPositionX(1900f);
        //LeanTween.moveLocalX(objController, 618f, 1.05f).setEaseOutQuint();
        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 1) {
            arrayObjSetting[0].SetActive(value: false);
            arrayObjSetting[1].SetActive(value: true);
        }
        else {
            arrayObjSetting[0].SetActive(value: true);
            arrayObjSetting[1].SetActive(value: false);
        }
    }
    public void OnSettingBack() {
        SingletonCustom<GameSettingManager>.Instance.AutoPlayerNumSetting();
        renderGameThumbnail.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetThumbnail(cursor.GetSelectNo());
    }
    public void OnEnable() {
        Show();
    }
    public void Hide() {
        UnityEngine.Debug.Log("Hide");
        base.gameObject.SetActive(value: false);
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
        if (!IsButtonInteractable()) {
            return;
        }
        if (cursor.IsPushMovedButtonMoment()) {
            OnMoveButtonDown();
        }
        if (cursor.IsOkButton()) {
            OnSelectButtonDown();
        }
        else if (cursor.IsReturnButton()) {
            OnReturnButtonDown();
        }
        else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Start) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Back)) {
            SingletonCustom<GS_Setting>.Instance.Open();
            cursor.IsStop = true;
            frameLockCnt = 1;
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        }
        else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Y)) {
            SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = SingletonCustom<GS_GameSelectManager>.Instance.ArrayCursorGameType[cursor.GetSelectNo()];
            SingletonCustom<CommonNotificationManager>.Instance.OpenOperationInfoAtGameSelect();
            cursor.IsStop = true;
            frameLockCnt = 1;
            SingletonCustom<AudioManager>.Instance.SePlay("se_pause_open");
        }
        else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X)) {
            OnRandomGameButtonDown();
        }
    }
    private bool IsButtonInteractable() {
        if (frameLockCnt > 0) {
            frameLockCnt--;
            if (frameLockCnt == 0) {
                cursor.IsStop = false;
            }
            return false;
        }
        if (CommonNotificationManager.Instance.IsOpen) {
            frameLockCnt = 1;
            return false;
        }
        if (SceneManager.Instance.IsFade) {
            return false;
        }
        if (GS_Setting.Instance.IsActive) {
            return false;
        }
        if (!gameObject.activeSelf) {
            return false;
        }
        return true;
    }
    public void OnMoveButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        if (cursor.IsPushMovedButtonMoment(CursorManager.MoveDir.TOP)) {
            rendererStick.sprite = arraySpStick[1];
        }
        else if (cursor.IsPushMovedButtonMoment(CursorManager.MoveDir.DOWN)) {
            rendererStick.sprite = arraySpStick[2];
        }
        else if (cursor.IsPushMovedButtonMoment(CursorManager.MoveDir.LEFT)) {
            rendererStick.sprite = arraySpStick[3];
        }
        else if (cursor.IsPushMovedButtonMoment(CursorManager.MoveDir.RIGHT)) {
            rendererStick.sprite = arraySpStick[4];
        }
        if (stickNeutralTiem <= 0f && !SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX)) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_stick_move", _loop: false, 0f, 1f, 1f, 0.05f);
        }
        stickNeutralTiem = STICK_NEUTRAL_TIME;
        renderGameTitle.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetGameTitle(cursor.GetSelectNo());
        //renderGameTitle.transform.SetLocalPositionX(-496.6f + renderGameTitle.bounds.size.x * 0.5f);
        renderGameThumbnail.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetThumbnail(cursor.GetSelectNo());
    }
    public void OnSelectButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        if (listSelectGameNo.Contains(cursor.GetSelectNo())) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
            return;
        }
        buttonANeturalTime = STICK_NEUTRAL_TIME;
        rendererButtonA.sprite = arraySpButtonA[1];
        Select(cursor.GetSelectNo());
    }
    public void OnReturnButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
        if (listSelectGameNo.Count > 0) {
            cursor.SetSelectNo(listSelectGameNo[listSelectGameNo.Count - 1]);
            cursor.SetCursorPos(0, listSelectGameNo[listSelectGameNo.Count - 1]);
            renderGameTitle.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetGameTitle(cursor.GetSelectNo());
            //renderGameTitle.transform.SetLocalPositionX(-496.6f + renderGameTitle.bounds.size.x * 0.5f);
            renderGameThumbnail.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetThumbnail(cursor.GetSelectNo());
            listSelectGameNo.RemoveAt(listSelectGameNo.Count - 1);
            for (int i = 0; i < partySelectButton.GetSelectFadeLen(); i++) {
                partySelectButton.GetSelectFade(i).SetActive(value: false);
            }
            for (int j = 0; j < partySelectButton.GetRenderSelectNumberLen(); j++) {
                partySelectButton.GetRenderSelectNumber(j).gameObject.SetActive(value: false);
            }
            for (int k = 0; k < arraySelectGameButton.Length; k++) {
                arraySelectGameButton[k].sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetEmptyButton(k);
            }
            for (int l = 0; l < listSelectGameNo.Count; l++) {
                partySelectButton.GetSelectFade(listSelectGameNo[l]).SetActive(value: true);
                partySelectButton.GetRenderSelectNumber(listSelectGameNo[l]).gameObject.SetActive(value: true);
                partySelectButton.GetRenderSelectNumber(listSelectGameNo[l]).sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetSelectNumber(l);
                arraySelectGameButton[l].sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetGameButton(listSelectGameNo[l]);
            }
        }
        else {
            SingletonCustom<SceneManager>.Instance.FadeExec(delegate { SingletonCustom<GS_GameSelectManager>.Instance.SetModeSelect(); });
        }
        buttonBNeturalTime = STICK_NEUTRAL_TIME;
        rendererButtonB.sprite = arraySpButtonB[1];
    }
    public void OnRandomGameButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        int num = SingletonCustom<GameSettingManager>.Instance.SelectGameNum - listSelectGameNo.Count;
        for (int m = 0; m < num; m++) {
            List<int> list = new List<int>();
            for (int n = 0; n < cursor.GetButtonObjLength(0); n++) {
                list.Add(n);
            }
            for (int num2 = 0; num2 < listSelectGameNo.Count; num2++) {
                list.Remove(listSelectGameNo[num2]);
            }
            list.Shuffle();
            int num3 = list[0];
            int num4 = 0;
            while (SingletonCustom<GS_GameSelectManager>.Instance.CheckDisableGame(num3) || SingletonCustom<GS_GameSelectManager>.Instance.CheckCoopGame(num3) || cursor.GetButtonObjLength(0) <= num3 || listSelectGameNo.Contains(num3)) {
                list.Shuffle();
                num3 = list[0];
                num4++;
                UnityEngine.Debug.Log("idx:" + num3.ToString());
                if (num4 >= 1000) {
                    break;
                }
            }
            if (num4 >= 1000) {
                UnityEngine.Debug.Log("break");
            }
            cursor.SetSelectNo(num3);
            cursor.SetCursorPos(0, num3);
            renderGameTitle.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetGameTitle(cursor.GetSelectNo());
            //renderGameTitle.transform.SetLocalPositionX(-496.6f + renderGameTitle.bounds.size.x * 0.5f);
            renderGameThumbnail.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetThumbnail(cursor.GetSelectNo());
            buttonANeturalTime = STICK_NEUTRAL_TIME;
            rendererButtonA.sprite = arraySpButtonA[1];
            Select(cursor.GetSelectNo(), _isRandom: true, _isSe: false);
        }
    }
    public void Select(int _idx, bool _isRandom = false, bool _isSe = true) {
        UnityEngine.Debug.Log("_idx:" + _idx.ToString());
        UnityEngine.Debug.Log("gameType:" + SingletonCustom<GS_GameSelectManager>.Instance.ArrayCursorGameType[_idx].ToString());
        listSelectGameNo.Add(_idx);
        UnityEngine.Debug.Log("ListLen:" + listSelectGameNo.Count.ToString());
        _idx = (int)SingletonCustom<GS_GameSelectManager>.Instance.ArrayCursorGameType[_idx];
        UnityEngine.Debug.Log("select:" + _idx.ToString());
        for (int i = 0; i < partySelectButton.GetSelectFadeLen(); i++) {
            partySelectButton.GetSelectFade(i).SetActive(value: false);
        }
        for (int j = 0; j < partySelectButton.GetRenderSelectNumberLen(); j++) {
            partySelectButton.GetRenderSelectNumber(j).gameObject.SetActive(value: false);
        }
        for (int k = 0; k < arraySelectGameButton.Length; k++) {
            arraySelectGameButton[k].sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetEmptyButton(k);
        }
        for (int l = 0; l < listSelectGameNo.Count; l++) {
            partySelectButton.GetSelectFade(listSelectGameNo[l]).SetActive(value: true);
            partySelectButton.GetRenderSelectNumber(listSelectGameNo[l]).gameObject.SetActive(value: true);
            partySelectButton.GetRenderSelectNumber(listSelectGameNo[l]).sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetSelectNumber(l);
            arraySelectGameButton[l].sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetGameButton(listSelectGameNo[l]);
        }
        if (listSelectGameNo.Count < SingletonCustom<GameSettingManager>.Instance.SelectGameNum) {
            if (_isSe) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
            }
            if (_isRandom) {
                return;
            }
            int num = 0;
            CursorButtonObject nowSelectObject = cursor.GetNowSelectObject();
            CursorButtonObject firstStepRightObj = partySelectButton.GetFirstStepRightObj();
            CursorButtonObject secondStepRightObj = partySelectButton.GetSecondStepRightObj();
            CursorButtonObject thirdStepRightObj = partySelectButton.GetThirdStepRightObj();
            if (nowSelectObject == firstStepRightObj) {
                num = secondStepRightObj.rightNo;
            }
            else if (!(nowSelectObject == secondStepRightObj)) {
                num = ((!(thirdStepRightObj != null) || !(nowSelectObject == thirdStepRightObj)) ? nowSelectObject.rightNo : firstStepRightObj.rightNo);
            }
            else {
                num = firstStepRightObj.rightNo;
                if (thirdStepRightObj != null) {
                    num = thirdStepRightObj.rightNo;
                }
            }
            while (listSelectGameNo.Contains(num)) {
                cursor.SetSelectNo(num);
                nowSelectObject = cursor.GetNowSelectObject();
                if (nowSelectObject == firstStepRightObj) {
                    num = secondStepRightObj.rightNo;
                }
                else if (nowSelectObject == secondStepRightObj) {
                    num = firstStepRightObj.rightNo;
                    if (thirdStepRightObj != null) {
                        num = thirdStepRightObj.rightNo;
                    }
                }
                else {
                    num = ((!(thirdStepRightObj != null) || !(nowSelectObject == thirdStepRightObj)) ? nowSelectObject.rightNo : firstStepRightObj.rightNo);
                }
            }
            cursor.SetSelectNo(num);
            cursor.SetCursorPos(0, num);
            renderGameTitle.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetGameTitle(cursor.GetSelectNo());
            //renderGameTitle.transform.SetLocalPositionX(-496.6f + renderGameTitle.bounds.size.x * 0.5f);
            renderGameThumbnail.sprite = SingletonCustom<GS_ThumbnailManager>.Instance.GetThumbnail(cursor.GetSelectNo());
        }
        else {
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_enter");
            for (int m = 0; m < playerGroupList.Length; m++) {
                playerGroupList[m].Clear();
            }
            for (int n = 0; n < SingletonCustom<GameSettingManager>.Instance.PlayerNum; n++) {
                playerGroupList[n].Add(n);
            }
            SingletonCustom<GameSettingManager>.Instance.PlayerGroupList = playerGroupList;
            SingletonCustom<GameSettingManager>.Instance.LastSelectGameType = GS_Define.GameType.GET_BALL;
            SingletonCustom<GameSettingManager>.Instance.TeamNum = Mathf.Clamp(SingletonCustom<GameSettingManager>.Instance.PlayerNum, 2, 6);
            SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.BATTLE;
            UnityEngine.Debug.Log("★selectType:" + SingletonCustom<GameSettingManager>.Instance.TeamNum.ToString());
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_enter");
            GS_Define.GameType gameType = (GS_Define.GameType)_idx;
            UnityEngine.Debug.Log("load GS_Define.GameType:" + gameType.ToString());
            SingletonCustom<GameSettingManager>.Instance.SetSelectGame(listSelectGameNo);
            SingletonCustom<GameSettingManager>.Instance.InitSportsDay();
            SingletonCustom<GameSettingManager>.Instance.SelectPartyModeIdx = cursor.GetSelectNo();
            SingletonCustom<SceneManager>.Instance.NextScene(SingletonCustom<GameSettingManager>.Instance.NextTable());
        }
    }
    private void OnDestroy() {
        LeanTween.cancel(objRandom);
        LeanTween.cancel(objController);
    }
}