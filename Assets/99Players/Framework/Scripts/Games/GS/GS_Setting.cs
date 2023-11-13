using GamepadInput;
using System;
using UnityEngine;
public class GS_Setting : SingletonCustom<GS_Setting> {
    public enum SettingType {
        BGM,
        SE,
        VOICE,
        AiStrength,
        Vibration,
        StartHelp,
        CharacterNum,
        Crown,
        GameSelectNumRed,
        GameSelectNumBlue,
        GameSelectNumYellow,
        Map,
        GameSelectIcon,
        Style
    }
    [SerializeField]
    [Header("表示ル\u30fcト")]
    private GameObject root;
    [SerializeField]
    [Header("カ\u30fcソル")]
    private CursorManager cursorManager;
    [SerializeField]
    [Header("設定項目")]
    private GS_SettingRow[] arraySetting;
    [SerializeField]
    [Header("項目説明")]
    private GameObject[] arrayCaption;
    [SerializeField]
    [Header("ボタンのル\u30fcトオブジェクト")]
    private GameObject objButtonRoot;
    [SerializeField]
    [Header("上矢印")]
    private GameObject objTopArrow;
    [SerializeField]
    [Header("下矢印")]
    private GameObject objBottomArrow;
    [SerializeField]
    [Header("無効表示オブジェクト")]
    private GameObject[] arrayDisableObj;
    private static readonly int PAGE_ROW_NUM = 5;
    private static float MOVE_ROW = 135f;
    private bool isMainCall;
    private int scrollOffset;
    private int objLength;
    public static int[] GameSelectNum = new int[6] {
        5,
        10,
        15,
        25,
        35,
        41
    };
    public bool IsActive {
        get { return root.activeSelf; }
        set { root.SetActive(value); }
    }
    public void Open() {
        root.SetActive(value: true);
        cursorManager.SetCursorPos(0, 0);
        objButtonRoot.transform.SetLocalPositionY(25f);
        UpdateGuide();
        scrollOffset = cursorManager.GetSelectNo() - (PAGE_ROW_NUM - 1);
        cursorManager.IsStop = false;
        if (SingletonCustom<SceneManager>.Instance.GetNowScene() == SceneManager.SceneType.MAIN) {
            isMainCall = true;
            for (int i = 0; i < arrayDisableObj.Length; i++) {
                arrayDisableObj[i].SetActive(value: false);
            }
            if (SingletonCustom<GS_GameSelectManager>.Instance.IsPartySelect) {
                arrayDisableObj[2].SetActive(value: true);
                arrayDisableObj[3].SetActive(value: true);
                arrayDisableObj[4].SetActive(value: true);
            }
        }
        else {
            isMainCall = false;
            for (int j = 0; j < arrayDisableObj.Length; j++) {
                arrayDisableObj[j].SetActive(value: true);
            }
        }
        int layerId = 0;
        if (SingletonCustom<AocAssetBundleManager>.Instance.GetDlcCount() >= 2) {
            objLength = cursorManager.GetButtonObjLength(layerId) - 1;
            cursorManager.GetButtonObjs(layerId)[0].topNo = objLength;
            cursorManager.GetButtonObjs(layerId)[objLength].downNo = 0;
        }
        else {
            objLength = cursorManager.GetButtonObjLength(layerId) - 4;
        }
    }
    public void Close() {
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
        if (SingletonCustom<SceneManager>.Instance.GetNowScene() == SceneManager.SceneType.MAIN) {
            SingletonCustom<GS_GameSelectManager>.Instance.OnSettingBack();
        }
        else {
            SingletonCustom<CommonNotificationManager>.Instance.AddLockCnt();
        }
        root.SetActive(value: false);
        cursorManager.IsStop = true;
    }
    public void Click() {
    }
    private void Update() {
        if (SingletonCustom<DM>.Instance.IsActive() || !root.activeSelf) {
            return;
        }
        if (cursorManager.IsPushMovedButtonMoment(CursorManager.MoveDir.LEFT)) {
            MoveLeft();
        }
        else if (cursorManager.IsPushMovedButtonMoment(CursorManager.MoveDir.RIGHT)) {
            MoveRight();
        }
        else if (!cursorManager.IsPushMovedButtonMoment(CursorManager.MoveDir.TOP) && !cursorManager.IsPushMovedButtonMoment(CursorManager.MoveDir.DOWN)) {
            if (cursorManager.IsReturnButton()) {
                Close();
            }
            else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Y, _isRepeat: false, KeyCode.None, useOnlyArrow: false, _isTimeMoving: true)) {
                OpenRestoreDefault();
            }
        }
        if (cursorManager.IsPushMovedButtonMoment(CursorManager.MoveDir.TOP)) {
            MoveUp();
        }
        else if (cursorManager.IsPushMovedButtonMoment(CursorManager.MoveDir.DOWN)) {
            MoveDown();
        }
    }
    public void CancelReset() {
    }
    public void ConfirmReset() {
        SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.InitSetting(isMainCall, isMainCall && SingletonCustom<GS_GameSelectManager>.Instance.IsPartySelect);
        for (int i = 0; i < arraySetting.Length; i++) {
            if (!isMainCall && (i == 3 || i == 5 || i == 7 || i == 8 || i == 9)) {
                UnityEngine.Debug.Log("[b]isMain:" + isMainCall.ToString() + " i:" + i.ToString());
            }
            else if (!isMainCall || !SingletonCustom<GS_GameSelectManager>.Instance.IsPartySelect || (i != 7 && i != 8 && i != 9)) {
                UnityEngine.Debug.Log("[a]isMain:" + isMainCall.ToString() + " i:" + i.ToString());
                arraySetting[i].Reset();
            }
        }
    }
    private void UpdateGuide() {
        for (int i = 0; i < arrayCaption.Length; i++) {
            arrayCaption[i].gameObject.SetActive(i == cursorManager.GetSelectNo());
        }
    }
    public void OpenRestoreDefault() {
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 55), DM.PARAM_LIST.SHOW_BACK_FADE, true, DM.PARAM_LIST.DIALOG_SIZE_H, 380f, DM.PARAM_LIST.DIALOG_SIZE_W, 630f, DM.PARAM_LIST.CHOICE, "", DM.PARAM_LIST.CALL_BACK, DM.List(delegate { ConfirmReset(); }, delegate { CancelReset(); }), DM.PARAM_LIST.BUTTON_TEXT, DM.List(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 56), SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 57)));
    }
    public void MoveUp() {
        UnityEngine.Debug.Log("offset:" + scrollOffset.ToString());
        if (cursorManager.GetSelectNo() < scrollOffset) {
            scrollOffset--;
            UnityEngine.Debug.Log("offset:" + scrollOffset.ToString());
            LeanTween.cancel(objButtonRoot);
            LeanTween.moveLocalY(objButtonRoot, 25f + MOVE_ROW * (float)scrollOffset, 0.075f).setOnComplete((Action)delegate { UpdateGuide(); }).setIgnoreTimeScale(useUnScaledTime: true);
        }
        else if (cursorManager.GetSelectNo() >= objLength) {
            scrollOffset = cursorManager.GetSelectNo() - (PAGE_ROW_NUM - 1);
            UnityEngine.Debug.Log("offset:" + scrollOffset.ToString());
            LeanTween.cancel(objButtonRoot);
            LeanTween.moveLocalY(objButtonRoot, 25f + MOVE_ROW * (float)scrollOffset, 0.075f).setOnComplete((Action)delegate { UpdateGuide(); }).setIgnoreTimeScale(useUnScaledTime: true);
        }
        else {
            UpdateGuide();
        }
    }
    public void MoveDown() {
        UnityEngine.Debug.Log("offset:" + scrollOffset.ToString());
        if (cursorManager.GetSelectNo() == 0) {
            scrollOffset = 0;
            UnityEngine.Debug.Log("offsetA:" + scrollOffset.ToString());
            LeanTween.cancel(objButtonRoot);
            LeanTween.moveLocalY(objButtonRoot, 25f, 0.075f).setOnComplete((Action)delegate { UpdateGuide(); }).setIgnoreTimeScale(useUnScaledTime: true);
        }
        else if (cursorManager.GetSelectNo() >= PAGE_ROW_NUM + Mathf.Clamp(scrollOffset, 0, scrollOffset)) {
            scrollOffset = cursorManager.GetSelectNo() - (PAGE_ROW_NUM - 1);
            UnityEngine.Debug.Log("offsetB:" + scrollOffset.ToString());
            LeanTween.cancel(objButtonRoot);
            LeanTween.moveLocalY(objButtonRoot, 25f + MOVE_ROW * (float)scrollOffset, 0.075f).setOnComplete((Action)delegate { UpdateGuide(); }).setIgnoreTimeScale(useUnScaledTime: true);
        }
        else {
            UpdateGuide();
        }
    }
    public void MoveRight() {
        if ((isMainCall || (cursorManager.GetSelectNo() != 3 && cursorManager.GetSelectNo() != 5 && cursorManager.GetSelectNo() != 7 && cursorManager.GetSelectNo() != 8 && cursorManager.GetSelectNo() != 9)) && (!isMainCall || !SingletonCustom<GS_GameSelectManager>.Instance.IsPartySelect || (cursorManager.GetSelectNo() != 7 && cursorManager.GetSelectNo() != 8 && cursorManager.GetSelectNo() != 9))) {
            arraySetting[cursorManager.GetSelectNo()].InputRight();
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        }
    }
    public void MoveLeft() {
        if ((isMainCall || (cursorManager.GetSelectNo() != 3 && cursorManager.GetSelectNo() != 5 && cursorManager.GetSelectNo() != 7 && cursorManager.GetSelectNo() != 8 && cursorManager.GetSelectNo() != 9)) && (!isMainCall || !SingletonCustom<GS_GameSelectManager>.Instance.IsPartySelect || (cursorManager.GetSelectNo() != 7 && cursorManager.GetSelectNo() != 8 && cursorManager.GetSelectNo() != 9))) {
            arraySetting[cursorManager.GetSelectNo()].InputLeft();
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        }
    }
    public void MoveSpecificRight(int number) {
        arraySetting[number].InputRight();
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
    }
    public void MoveSpecificLeft(int number) {
        arraySetting[number].InputLeft();
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
    }
}