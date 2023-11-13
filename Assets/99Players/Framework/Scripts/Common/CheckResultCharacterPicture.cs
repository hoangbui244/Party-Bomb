using GamepadInput;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class CheckResultCharacterPicture : SingletonCustom<CheckResultCharacterPicture> {
    private enum SelectModeType {
        Layout,
        Character,
        FaceType
    }
    private enum SelectLayoutType {
        Single,
        Two,
        Four
    }
    [Serializable]
    public struct SingleLayout_Data {
        [Header("レイアウトアンカ\u30fc")]
        public GameObject layoutAnchor;
        [Header("キャラクタ\u30fcアンカ\u30fc")]
        [NonReorderable]
        public GameObject[] characterAnchor;
        [Header("キャラクタ\u30fcデ\u30fcタ")]
        [NonReorderable]
        public SingleLayout_CharacterData[] charaData;
    }
    [Serializable]
    public struct SingleLayout_CharacterData {
        [Header("キャラクタ\u30fc")]
        [NonReorderable]
        public GameObject[] character;
    }
    [Serializable]
    public struct ButtonData_Select {
        [Header("ボタンアンカ\u30fc")]
        public GameObject buttonAnchor;
        [Header("ボタンデ\u30fcタ【選択】")]
        public CheckResultCharacterButton button_Select;
        [Header("ボタンデ\u30fcタ【もどる】")]
        public CheckResultCharacterButton button_Back;
        [Header("ボタンデ\u30fcタ【編集】")]
        public CheckResultCharacterButton button_Edit;
        [Header("ボタンデ\u30fcタ【コピ\u30fc】")]
        public CheckResultCharacterButton button_Copy;
    }
    [Serializable]
    public struct ButtonData_Edit {
        [Header("ボタンアンカ\u30fc")]
        public GameObject buttonAnchor;
        [Header("ボタンデ\u30fcタ【元に戻す】")]
        public CheckResultCharacterButton button_Undo;
        [Header("ボタンデ\u30fcタ【更新を進める】")]
        public CheckResultCharacterButton button_Redo;
        [Header("ボタンデ\u30fcタ【キャラ移動】")]
        public CheckResultCharacterButton button_Move;
        [Header("ボタンデ\u30fcタ【編集をやめる】")]
        public CheckResultCharacterButton button_Exit;
    }
    [Serializable]
    public struct ButtonData_Copy {
        [Header("ボタンアンカ\u30fc")]
        public GameObject buttonAnchor;
        [Header("ボタンデ\u30fcタ【コピ\u30fcを止める】")]
        public CheckResultCharacterButton button_CopyExit;
        [Header("ボタンデ\u30fcタ【コピ\u30fcする】")]
        public CheckResultCharacterButton button_Copy;
    }
    public enum ButtonPushType {
        DOWN,
        HOLD,
        UP
    }
    public enum StickType {
        R,
        L
    }
    public enum StickDirType {
        UP,
        RIGHT,
        LEFT,
        DOWN
    }
    public enum CrossKeyType {
        UP,
        RIGHT,
        LEFT,
        DOWN
    }
    [SerializeField]
    [Header("レイアウト選択時のカ\u30fcソルマネ\u30fcジャ\u30fc")]
    private CursorManager cursorManager_Layout;
    [SerializeField]
    [Header("キャラクタ\u30fc選択時のカ\u30fcソルマネ\u30fcジャ\u30fc")]
    private CursorManager cursorManager_Character;
    [SerializeField]
    [Header("表情選択時のカ\u30fcソルマネ\u30fcジャ\u30fc")]
    private CursorManager cursorManager_FaceType;
    [SerializeField]
    [Header("各モ\u30fcドごとのボタンアンカ\u30fc")]
    private GameObject[] modeButtonAnchor;
    [SerializeField]
    [Header("写真のオブジェクト")]
    private GameObject photo;
    [SerializeField]
    [Header("１人表示時のデ\u30fcタ")]
    private SingleLayout_Data singleLayout_Data;
    [SerializeField]
    [Header("２人表示時のデ\u30fcタ")]
    private SingleLayout_Data twoLayout_Data;
    [SerializeField]
    [Header("４人表示時のデ\u30fcタ")]
    private SingleLayout_Data fourLayout_Data;
    [SerializeField]
    [Header("ボタンデ\u30fcタ【選択中】")]
    private ButtonData_Select buttonData_Select;
    [SerializeField]
    [Header("ボタンデ\u30fcタ【編集中】")]
    private ButtonData_Edit buttonData_Edit;
    [SerializeField]
    [Header("ボタンデ\u30fcタ【コピ\u30fc中】")]
    private ButtonData_Copy buttonData_Copy;
    [SerializeField]
    [Header("座標情報表示オブジェクト")]
    private GameObject posInfoObject;
    [SerializeField]
    [Header("座標情報表示テキスト")]
    private TextMeshPro posInfoText;
    private SelectLayoutType selectLayoutType;
    private int selectCharacterNo;
    private int selectFaceTypeNo;
    private SelectModeType nowSelectModeType;
    private bool isEdit;
    private List<Vector3> undoVecList = new List<Vector3>();
    private bool isCopy;
    private int copyOriginCharacterNo;
    private int copyOriginFaceTypeNo;
    private int undoNo;
    private bool isHoldInputIntervalMode;
    private const int MAX_PLAYER_NUM = 4;
    private float[] intervalInputHold_LStick = new float[4];
    private float[] intervalInputHold_RStick = new float[4];
    private float[] intervalInputHold_CrossKey = new float[4];
    private const float HOLD_INPUT_INTERVAL = 0f;
    private void Start() {
        for (int i = 0; i < modeButtonAnchor.Length; i++) {
            modeButtonAnchor[i].SetActive(value: false);
        }
        modeButtonAnchor[(int)nowSelectModeType].SetActive(value: true);
        for (int j = 0; j < singleLayout_Data.charaData.Length; j++) {
            for (int k = 0; k < singleLayout_Data.charaData[j].character.Length; k++) {
                if (k == 0) {
                    singleLayout_Data.charaData[j].character[k].transform.localPosition = ResultCharacterPosData.GetCharacterPosition(ResultCharacterPosData.ShowCharaNumType.Single, j, ResultCharacterManager.FaceType.Normal, 0);
                } else if (k <= 2) {
                    singleLayout_Data.charaData[j].character[k].transform.localPosition = ResultCharacterPosData.GetCharacterPosition(ResultCharacterPosData.ShowCharaNumType.Single, j, ResultCharacterManager.FaceType.Happy, (k != 1) ? 1 : 0);
                } else {
                    singleLayout_Data.charaData[j].character[k].transform.localPosition = ResultCharacterPosData.GetCharacterPosition(ResultCharacterPosData.ShowCharaNumType.Single, j, ResultCharacterManager.FaceType.Sad, (k != 3) ? 1 : 0);
                }
            }
        }
        for (int l = 0; l < twoLayout_Data.charaData.Length; l++) {
            for (int m = 0; m < twoLayout_Data.charaData[l].character.Length; m++) {
                if (m == 0) {
                    twoLayout_Data.charaData[l].character[m].transform.localPosition = ResultCharacterPosData.GetCharacterPosition(ResultCharacterPosData.ShowCharaNumType.Two, l, ResultCharacterManager.FaceType.Normal, 0);
                } else if (m <= 2) {
                    twoLayout_Data.charaData[l].character[m].transform.localPosition = ResultCharacterPosData.GetCharacterPosition(ResultCharacterPosData.ShowCharaNumType.Two, l, ResultCharacterManager.FaceType.Happy, (m != 1) ? 1 : 0);
                } else {
                    twoLayout_Data.charaData[l].character[m].transform.localPosition = ResultCharacterPosData.GetCharacterPosition(ResultCharacterPosData.ShowCharaNumType.Two, l, ResultCharacterManager.FaceType.Sad, (m != 3) ? 1 : 0);
                }
            }
        }
        for (int n = 0; n < fourLayout_Data.charaData.Length; n++) {
            for (int num = 0; num < fourLayout_Data.charaData[n].character.Length; num++) {
                if (num == 0) {
                    fourLayout_Data.charaData[n].character[num].transform.localPosition = ResultCharacterPosData.GetCharacterPosition(ResultCharacterPosData.ShowCharaNumType.Four, n, ResultCharacterManager.FaceType.Normal, 0);
                } else if (num <= 2) {
                    fourLayout_Data.charaData[n].character[num].transform.localPosition = ResultCharacterPosData.GetCharacterPosition(ResultCharacterPosData.ShowCharaNumType.Four, n, ResultCharacterManager.FaceType.Happy, (num != 1) ? 1 : 0);
                } else {
                    fourLayout_Data.charaData[n].character[num].transform.localPosition = ResultCharacterPosData.GetCharacterPosition(ResultCharacterPosData.ShowCharaNumType.Four, n, ResultCharacterManager.FaceType.Sad, (num != 3) ? 1 : 0);
                }
            }
        }
        ResetUndoList();
        UpdateCharacter();
        SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_menu", _loop: true);
    }
    private void Update() {
        selectLayoutType = (SelectLayoutType)cursorManager_Layout.GetSelectNo();
        selectCharacterNo = cursorManager_Character.GetSelectNo();
        selectFaceTypeNo = cursorManager_FaceType.GetSelectNo();
        if (IsPushButton_Plus(0, ButtonPushType.DOWN)) {
            posInfoObject.SetActive(!posInfoObject.activeSelf);
        }
        if (!isEdit) {
            if (IsPushButton_A(0, ButtonPushType.DOWN)) {
                if (isCopy) {
                    CopyCharacter();
                    AddUndoList();
                    isCopy = false;
                    SingletonCustom<AudioManager>.Instance.SePlay("se_button_enter");
                } else {
                    switch (nowSelectModeType) {
                        case SelectModeType.Layout:
                            modeButtonAnchor[(int)nowSelectModeType].SetActive(value: false);
                            nowSelectModeType = SelectModeType.Character;
                            modeButtonAnchor[(int)nowSelectModeType].SetActive(value: true);
                            SingletonCustom<AudioManager>.Instance.SePlay("se_button_enter");
                            break;
                        case SelectModeType.Character:
                            modeButtonAnchor[(int)nowSelectModeType].SetActive(value: false);
                            nowSelectModeType = SelectModeType.FaceType;
                            modeButtonAnchor[(int)nowSelectModeType].SetActive(value: true);
                            SingletonCustom<AudioManager>.Instance.SePlay("se_button_enter");
                            break;
                    }
                }
            } else if (IsPushButton_B(0, ButtonPushType.DOWN)) {
                if (isCopy) {
                    isCopy = false;
                    SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
                } else {
                    switch (nowSelectModeType) {
                        case SelectModeType.Character:
                            if (isCopy) {
                                return;
                            }
                            modeButtonAnchor[(int)nowSelectModeType].SetActive(value: false);
                            nowSelectModeType = SelectModeType.Layout;
                            modeButtonAnchor[(int)nowSelectModeType].SetActive(value: true);
                            SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
                            break;
                        case SelectModeType.FaceType:
                            modeButtonAnchor[(int)nowSelectModeType].SetActive(value: false);
                            nowSelectModeType = SelectModeType.Character;
                            modeButtonAnchor[(int)nowSelectModeType].SetActive(value: true);
                            SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
                            break;
                    }
                }
            } else if (IsPushButton_X(0, ButtonPushType.DOWN)) {
                if (nowSelectModeType == SelectModeType.FaceType) {
                    isEdit = true;
                    cursorManager_Layout.IsStop = true;
                    cursorManager_Character.IsStop = true;
                    cursorManager_FaceType.IsStop = true;
                    SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                }
            } else if (IsPushButton_Y(0, ButtonPushType.DOWN) && nowSelectModeType == SelectModeType.FaceType) {
                isCopy = true;
                copyOriginCharacterNo = selectCharacterNo;
                copyOriginFaceTypeNo = selectFaceTypeNo;
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
            }
        } else {
            if (!IsStickTilt(0, StickType.L)) {
                if (IsPushCrossKey(0, CrossKeyType.LEFT, ButtonPushType.HOLD)) {
                    EditCharacter(StickDirType.LEFT);
                } else if (IsPushCrossKey(0, CrossKeyType.RIGHT, ButtonPushType.HOLD)) {
                    EditCharacter(StickDirType.RIGHT);
                } else if (IsPushCrossKey(0, CrossKeyType.UP, ButtonPushType.HOLD)) {
                    EditCharacter(StickDirType.UP);
                } else if (IsPushCrossKey(0, CrossKeyType.DOWN, ButtonPushType.HOLD)) {
                    EditCharacter(StickDirType.DOWN);
                } else {
                    AddUndoList();
                }
            } else if (IsStickTiltDirection(0, StickType.L, StickDirType.LEFT)) {
                EditCharacter(StickDirType.LEFT);
            } else if (IsStickTiltDirection(0, StickType.L, StickDirType.RIGHT)) {
                EditCharacter(StickDirType.RIGHT);
            } else if (IsStickTiltDirection(0, StickType.L, StickDirType.UP)) {
                EditCharacter(StickDirType.UP);
            } else if (IsStickTiltDirection(0, StickType.L, StickDirType.DOWN)) {
                EditCharacter(StickDirType.DOWN);
            } else {
                AddUndoList();
            }
            if (IsPushButton_B(0, ButtonPushType.DOWN)) {
                isEdit = false;
                cursorManager_Layout.IsStop = false;
                cursorManager_Character.IsStop = false;
                cursorManager_FaceType.IsStop = false;
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
            } else if (IsPushButton_L(0, ButtonPushType.DOWN)) {
                Undo();
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
            } else if (IsPushButton_R(0, ButtonPushType.DOWN)) {
                Redo();
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
            }
        }
        if (cursorManager_Layout.IsPushMovedButtonMoment() || cursorManager_Character.IsPushMovedButtonMoment() || cursorManager_FaceType.IsPushMovedButtonMoment()) {
            UpdateCharacter();
        }
        UpdateJoycon();
        UpdateButton();
        PosInfoUpdate();
    }
    private void UpdateCharacter() {
        singleLayout_Data.layoutAnchor.SetActive(value: false);
        twoLayout_Data.layoutAnchor.SetActive(value: false);
        fourLayout_Data.layoutAnchor.SetActive(value: false);
        switch (selectLayoutType) {
            case SelectLayoutType.Single:
                singleLayout_Data.layoutAnchor.SetActive(value: true);
                for (int m = 0; m < singleLayout_Data.characterAnchor.Length; m++) {
                    singleLayout_Data.characterAnchor[m].SetActive(value: false);
                }
                singleLayout_Data.characterAnchor[selectCharacterNo].SetActive(value: true);
                for (int n = 0; n < singleLayout_Data.charaData[selectCharacterNo].character.Length; n++) {
                    singleLayout_Data.charaData[selectCharacterNo].character[n].SetActive(value: false);
                }
                singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].SetActive(value: true);
                photo.SetActive(value: true);
                break;
            case SelectLayoutType.Two:
                twoLayout_Data.layoutAnchor.SetActive(value: true);
                for (int k = 0; k < twoLayout_Data.characterAnchor.Length; k++) {
                    twoLayout_Data.characterAnchor[k].SetActive(value: false);
                }
                twoLayout_Data.characterAnchor[selectCharacterNo].SetActive(value: true);
                for (int l = 0; l < twoLayout_Data.charaData[selectCharacterNo].character.Length; l++) {
                    twoLayout_Data.charaData[selectCharacterNo].character[l].SetActive(value: false);
                }
                twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].SetActive(value: true);
                photo.SetActive(value: false);
                break;
            case SelectLayoutType.Four:
                fourLayout_Data.layoutAnchor.SetActive(value: true);
                for (int i = 0; i < fourLayout_Data.characterAnchor.Length; i++) {
                    fourLayout_Data.characterAnchor[i].SetActive(value: false);
                }
                fourLayout_Data.characterAnchor[selectCharacterNo].SetActive(value: true);
                for (int j = 0; j < fourLayout_Data.charaData[selectCharacterNo].character.Length; j++) {
                    fourLayout_Data.charaData[selectCharacterNo].character[j].SetActive(value: false);
                }
                fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].SetActive(value: true);
                photo.SetActive(value: false);
                break;
        }
        ResetUndoList();
    }
    private void UpdateButton() {
        switch (nowSelectModeType) {
            case SelectModeType.Layout:
                buttonData_Select.button_Back.NonActive();
                buttonData_Select.button_Copy.NonActive();
                buttonData_Select.button_Edit.NonActive();
                break;
            case SelectModeType.Character:
                buttonData_Select.button_Select.Active();
                buttonData_Select.button_Back.Active();
                buttonData_Select.button_Copy.NonActive();
                buttonData_Select.button_Edit.NonActive();
                break;
            case SelectModeType.FaceType:
                if (isEdit) {
                    buttonData_Select.buttonAnchor.SetActive(value: false);
                    buttonData_Edit.buttonAnchor.SetActive(value: true);
                    buttonData_Copy.buttonAnchor.SetActive(value: false);
                    if (undoNo == 0) {
                        buttonData_Edit.button_Undo.NonActive();
                    } else {
                        buttonData_Edit.button_Undo.Active();
                    }
                    if (undoNo == undoVecList.Count - 1) {
                        buttonData_Edit.button_Redo.NonActive();
                    } else {
                        buttonData_Edit.button_Redo.Active();
                    }
                } else if (isCopy) {
                    buttonData_Select.buttonAnchor.SetActive(value: false);
                    buttonData_Edit.buttonAnchor.SetActive(value: false);
                    buttonData_Copy.buttonAnchor.SetActive(value: true);
                } else {
                    buttonData_Select.buttonAnchor.SetActive(value: true);
                    buttonData_Edit.buttonAnchor.SetActive(value: false);
                    buttonData_Copy.buttonAnchor.SetActive(value: false);
                    buttonData_Select.button_Select.NonActive();
                    buttonData_Select.button_Back.Active();
                    buttonData_Select.button_Copy.Active();
                    buttonData_Select.button_Edit.Active();
                }
                break;
        }
    }
    public void SaveData() {
        UnityEngine.Debug.Log("セ\u30fcブ");
    }
    private void EditCharacter(StickDirType _dir) {
        switch (_dir) {
            case StickDirType.LEFT:
                switch (selectLayoutType) {
                    case SelectLayoutType.Single:
                        singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionX(-1f);
                        break;
                    case SelectLayoutType.Two:
                        twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionX(-0.005f);
                        break;
                    case SelectLayoutType.Four:
                        fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionX(-0.005f);
                        break;
                }
                break;
            case StickDirType.RIGHT:
                switch (selectLayoutType) {
                    case SelectLayoutType.Single:
                        singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionX(1f);
                        break;
                    case SelectLayoutType.Two:
                        twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionX(0.005f);
                        break;
                    case SelectLayoutType.Four:
                        fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionX(0.005f);
                        break;
                }
                break;
            case StickDirType.UP:
                switch (selectLayoutType) {
                    case SelectLayoutType.Single:
                        singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionY(1f);
                        break;
                    case SelectLayoutType.Two:
                        twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionY(0.005f);
                        break;
                    case SelectLayoutType.Four:
                        fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionY(0.005f);
                        break;
                }
                break;
            case StickDirType.DOWN:
                switch (selectLayoutType) {
                    case SelectLayoutType.Single:
                        singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionY(-1f);
                        break;
                    case SelectLayoutType.Two:
                        twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionY(-0.005f);
                        break;
                    case SelectLayoutType.Four:
                        fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.AddLocalPositionY(-0.005f);
                        break;
                }
                break;
        }
    }
    private void CopyCharacter() {
        switch (selectLayoutType) {
            case SelectLayoutType.Single:
                singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition = singleLayout_Data.charaData[copyOriginCharacterNo].character[copyOriginFaceTypeNo].transform.localPosition;
                break;
            case SelectLayoutType.Two:
                twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition = twoLayout_Data.charaData[copyOriginCharacterNo].character[copyOriginFaceTypeNo].transform.localPosition;
                break;
            case SelectLayoutType.Four:
                fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition = fourLayout_Data.charaData[copyOriginCharacterNo].character[copyOriginFaceTypeNo].transform.localPosition;
                break;
        }
    }
    private void PosInfoUpdate() {
        switch (selectLayoutType) {
            case SelectLayoutType.Single: {
                    TextMeshPro textMeshPro3 = posInfoText;
                    string[] obj3 = new string[7]
                    {
                "Pos(",
                null,
                null,
                null,
                null,
                null,
                null
                    };
                    Vector3 localPosition = singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition;
                    obj3[1] = localPosition.x.ToString();
                    obj3[2] = ", ";
                    localPosition = singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition;
                    obj3[3] = localPosition.y.ToString();
                    obj3[4] = ", ";
                    localPosition = singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition;
                    obj3[5] = localPosition.z.ToString();
                    obj3[6] = ")";
                    textMeshPro3.text = string.Concat(obj3);
                    break;
                }
            case SelectLayoutType.Two: {
                    TextMeshPro textMeshPro2 = posInfoText;
                    string[] obj2 = new string[7]
                    {
                "Pos(",
                null,
                null,
                null,
                null,
                null,
                null
                    };
                    Vector3 localPosition = twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition;
                    obj2[1] = localPosition.x.ToString();
                    obj2[2] = ", ";
                    localPosition = twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition;
                    obj2[3] = localPosition.y.ToString();
                    obj2[4] = ", ";
                    localPosition = twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition;
                    obj2[5] = localPosition.z.ToString();
                    obj2[6] = ")";
                    textMeshPro2.text = string.Concat(obj2);
                    break;
                }
            case SelectLayoutType.Four: {
                    TextMeshPro textMeshPro = posInfoText;
                    string[] obj = new string[7]
                    {
                "Pos(",
                null,
                null,
                null,
                null,
                null,
                null
                    };
                    Vector3 localPosition = fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition;
                    obj[1] = localPosition.x.ToString();
                    obj[2] = ", ";
                    localPosition = fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition;
                    obj[3] = localPosition.y.ToString();
                    obj[4] = ", ";
                    localPosition = fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition;
                    obj[5] = localPosition.z.ToString();
                    obj[6] = ")";
                    textMeshPro.text = string.Concat(obj);
                    break;
                }
        }
    }
    private void ResetUndoList() {
        undoVecList.Clear();
        switch (selectLayoutType) {
            case SelectLayoutType.Single:
                undoVecList.Add(singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition);
                break;
            case SelectLayoutType.Two:
                undoVecList.Add(twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition);
                break;
            case SelectLayoutType.Four:
                undoVecList.Add(fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition);
                break;
        }
        undoNo = 0;
    }
    private void AddUndoList() {
        switch (selectLayoutType) {
            case SelectLayoutType.Single:
                if (undoVecList[undoNo] == singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition) {
                    return;
                }
                break;
            case SelectLayoutType.Two:
                if (undoVecList[undoNo] == twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition) {
                    return;
                }
                break;
            case SelectLayoutType.Four:
                if (undoVecList[undoNo] == fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition) {
                    return;
                }
                break;
        }
        if (undoNo != undoVecList.Count - 1) {
            int num = undoVecList.Count - 1 - undoNo;
            for (int i = 0; i < num; i++) {
                undoVecList.RemoveAt(undoVecList.Count - 1);
            }
        }
        switch (selectLayoutType) {
            case SelectLayoutType.Single:
                undoVecList.Add(singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition);
                break;
            case SelectLayoutType.Two:
                undoVecList.Add(twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition);
                break;
            case SelectLayoutType.Four:
                undoVecList.Add(fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition);
                break;
        }
        undoNo++;
    }
    private void Undo() {
        if (undoNo != 0) {
            undoNo--;
            switch (selectLayoutType) {
                case SelectLayoutType.Single:
                    singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition = undoVecList[undoNo];
                    break;
                case SelectLayoutType.Two:
                    twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition = undoVecList[undoNo];
                    break;
                case SelectLayoutType.Four:
                    fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition = undoVecList[undoNo];
                    break;
            }
        }
    }
    private void Redo() {
        if (undoNo != undoVecList.Count - 1) {
            undoNo++;
            switch (selectLayoutType) {
                case SelectLayoutType.Single:
                    singleLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition = undoVecList[undoNo];
                    break;
                case SelectLayoutType.Two:
                    twoLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition = undoVecList[undoNo];
                    break;
                case SelectLayoutType.Four:
                    fourLayout_Data.charaData[selectCharacterNo].character[selectFaceTypeNo].transform.localPosition = undoVecList[undoNo];
                    break;
            }
        }
    }
    public bool IsPushButton_A(int _userNo, ButtonPushType _buttonPushType) {
        return ProcessPushButton_A(_userNo, _buttonPushType);
    }
    public bool IsPushButton_B(int _userNo, ButtonPushType _buttonPushType) {
        return ProcessPushButton_B(_userNo, _buttonPushType);
    }
    public bool IsPushButton_X(int _userNo, ButtonPushType _buttonPushType) {
        return ProcessPushButton_X(_userNo, _buttonPushType);
    }
    public bool IsPushButton_Y(int _userNo, ButtonPushType _buttonPushType) {
        return ProcessPushButton_Y(_userNo, _buttonPushType);
    }
    public bool IsPushButton_L(int _userNo, ButtonPushType _buttonPushType) {
        return ProcessPushButton_L(_userNo, _buttonPushType);
    }
    public bool IsPushButton_R(int _userNo, ButtonPushType _buttonPushType) {
        return ProcessPushButton_R(_userNo, _buttonPushType);
    }
    public bool IsPushButton_Plus(int _userNo, ButtonPushType _buttonPushType) {
        return ProcessPushButton_Plus(_userNo, _buttonPushType);
    }
    public bool IsPushCrossKey(int _userNo, CrossKeyType _keyType, ButtonPushType _buttonPushType) {
        if (_buttonPushType == ButtonPushType.HOLD && isHoldInputIntervalMode && ProcessCrossKey(_userNo, _keyType, _buttonPushType)) {
            if (IsReturnInputHold_CrossKey(_userNo)) {
                ContinueInputHold_CrossKey(_userNo);
                return true;
            }
            return false;
        }
        if (ProcessCrossKey(_userNo, _keyType, _buttonPushType)) {
            ContinueInputHold_CrossKey(_userNo);
            return true;
        }
        return false;
    }
    public bool IsStickTiltDirection(int _userNo, StickType _stickType, StickDirType _stickDirType, float _stickNeutralValue = 0.5f) {
        Vector3 zero = Vector3.zero;
        zero = ((_stickType != 0) ? GetStickDir_L(_userNo) : GetStickDir_R(_userNo));
        if ((_stickDirType == StickDirType.UP && zero.z > _stickNeutralValue) || (_stickDirType == StickDirType.RIGHT && zero.x > _stickNeutralValue) || (_stickDirType == StickDirType.LEFT && zero.x < 0f - _stickNeutralValue) || (_stickDirType == StickDirType.DOWN && zero.z < 0f - _stickNeutralValue)) {
            if (isHoldInputIntervalMode) {
                if ((_stickType == StickType.R) ? IsReturnInputHold_Stick_R(_userNo) : IsReturnInputHold_Stick_L(_userNo)) {
                    if (_stickType == StickType.R) {
                        ContinueInputHold_Stick_R(_userNo);
                    } else {
                        ContinueInputHold_Stick_L(_userNo);
                    }
                    return true;
                }
                return false;
            }
            return true;
        }
        return false;
    }
    public bool IsPushButton_StickR(int _userNo, ButtonPushType _buttonPushType) {
        return ProcessPushButton_StickR(_userNo, _buttonPushType);
    }
    public bool IsPushButton_StickL(int _userNo, ButtonPushType _buttonPushType) {
        return ProcessPushButton_StickL(_userNo, _buttonPushType);
    }
    public bool IsStickTilt(int _userNo, StickType _stickType) {
        return GetStickTilt(_userNo, _stickType) > 0.01f;
    }
    private bool ProcessPushButton_A(int _userNo, ButtonPushType _type) {
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        switch (_type) {
            case ButtonPushType.DOWN:
                return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
            case ButtonPushType.HOLD:
                return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.A);
            case ButtonPushType.UP:
                return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.A);
            default:
                return false;
        }
    }
    private bool ProcessPushButton_B(int _userNo, ButtonPushType _type) {
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        switch (_type) {
            case ButtonPushType.DOWN:
                return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B);
            case ButtonPushType.HOLD:
                return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.B);
            case ButtonPushType.UP:
                return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.B);
            default:
                return false;
        }
    }
    private bool ProcessPushButton_X(int _userNo, ButtonPushType _type) {
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        switch (_type) {
            case ButtonPushType.DOWN:
                return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.X);
            case ButtonPushType.HOLD:
                return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.X);
            case ButtonPushType.UP:
                return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.X);
            default:
                return false;
        }
    }
    private bool ProcessPushButton_Y(int _userNo, ButtonPushType _type) {
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        switch (_type) {
            case ButtonPushType.DOWN:
                return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Y);
            case ButtonPushType.HOLD:
                return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Y);
            case ButtonPushType.UP:
                return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.Y);
            default:
                return false;
        }
    }
    private bool ProcessPushButton_L(int _userNo, ButtonPushType _type) {
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        switch (_type) {
            case ButtonPushType.DOWN:
                return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftShoulder);
            case ButtonPushType.HOLD:
                return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.LeftShoulder);
            case ButtonPushType.UP:
                return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.LeftShoulder);
            default:
                return false;
        }
    }
    private bool ProcessPushButton_R(int _userNo, ButtonPushType _type) {
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        switch (_type) {
            case ButtonPushType.DOWN:
                return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightShoulder);
            case ButtonPushType.HOLD:
                return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightShoulder);
            case ButtonPushType.UP:
                return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.RightShoulder);
            default:
                return false;
        }
    }
    private bool ProcessPushButton_Plus(int _userNo, ButtonPushType _type) {
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        switch (_type) {
            case ButtonPushType.DOWN:
                return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Start);
            case ButtonPushType.HOLD:
                return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Start);
            case ButtonPushType.UP:
                return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.Start);
            default:
                return false;
        }
    }
    public Vector3 GetStickDir_R(int _userNo) {
        float num = 0f;
        float num2 = 0f;
        Vector3 mVector3Zero = CalcManager.mVector3Zero;
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
        num = axisInput.Stick_R.x;
        num2 = axisInput.Stick_R.y;
        return new Vector3(num, 0f, num2);
    }
    public Vector3 GetStickDir_L(int _userNo) {
        float num = 0f;
        float num2 = 0f;
        Vector3 mVector3Zero = CalcManager.mVector3Zero;
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
        num = axisInput.Stick_L.x;
        num2 = axisInput.Stick_L.y;
        return new Vector3(num, 0f, num2);
    }
    private bool ProcessPushButton_StickR(int _userNo, ButtonPushType _type) {
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        switch (_type) {
            case ButtonPushType.DOWN:
                return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightStick);
            case ButtonPushType.HOLD:
                return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightStick);
            case ButtonPushType.UP:
                return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.RightStick);
            default:
                return false;
        }
    }
    private bool ProcessPushButton_StickL(int _userNo, ButtonPushType _type) {
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        switch (_type) {
            case ButtonPushType.DOWN:
                return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftStick);
            case ButtonPushType.HOLD:
                return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.LeftStick);
            case ButtonPushType.UP:
                return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.LeftStick);
            default:
                return false;
        }
    }
    public float GetStickTilt(int _userNo, StickType _stickType) {
        if (_stickType == StickType.L) {
            return GetStickDir_L(_userNo).magnitude;
        }
        return GetStickDir_R(_userNo).magnitude;
    }
    private bool ProcessCrossKey(int _userNo, CrossKeyType _keyType, ButtonPushType _type) {
        SatGamePad.Button button = SatGamePad.Button.A;
        int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userNo : 0;
        switch (_keyType) {
            case CrossKeyType.UP:
                button = SatGamePad.Button.Dpad_Up;
                break;
            case CrossKeyType.RIGHT:
                button = SatGamePad.Button.Dpad_Right;
                break;
            case CrossKeyType.LEFT:
                button = SatGamePad.Button.Dpad_Left;
                break;
            case CrossKeyType.DOWN:
                button = SatGamePad.Button.Dpad_Down;
                break;
        }
        switch (_type) {
            case ButtonPushType.DOWN:
                return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, button);
            case ButtonPushType.HOLD:
                return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, button);
            case ButtonPushType.UP:
                return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, button);
            default:
                return false;
        }
    }
    public void SetttingHoldInputIntervalMode() {
        isHoldInputIntervalMode = true;
    }
    public void ReleaseHoldInputIntervalMode() {
        isHoldInputIntervalMode = false;
    }
    private void UpdateJoycon() {
        for (int i = 0; i < 4; i++) {
            if (intervalInputHold_LStick[i] > 0f) {
                intervalInputHold_LStick[i] -= Time.deltaTime;
            }
            if (intervalInputHold_RStick[i] > 0f) {
                intervalInputHold_RStick[i] -= Time.deltaTime;
            }
            if (intervalInputHold_CrossKey[i] > 0f) {
                intervalInputHold_CrossKey[i] -= Time.deltaTime;
            }
        }
    }
    private void ContinueInputHold_Stick_L(int _userNo) {
        intervalInputHold_LStick[_userNo] = 0f;
    }
    private void ContinueInputHold_Stick_R(int _userNo) {
        intervalInputHold_RStick[_userNo] = 0f;
    }
    private void ContinueInputHold_CrossKey(int _userNo) {
        intervalInputHold_CrossKey[_userNo] = 0f;
    }
    private void ResetInputHold_Stick_L(int _userNo) {
        intervalInputHold_LStick[_userNo] = 0f;
    }
    private void ResetInputHold_Stick_R(int _userNo) {
        intervalInputHold_RStick[_userNo] = 0f;
    }
    private bool IsReturnInputHold_Stick_L(int _userNo) {
        return intervalInputHold_LStick[_userNo] <= 0f;
    }
    private bool IsReturnInputHold_Stick_R(int _userNo) {
        return intervalInputHold_RStick[_userNo] <= 0f;
    }
    private bool IsReturnInputHold_CrossKey(int _userNo) {
        return intervalInputHold_CrossKey[_userNo] <= 0f;
    }
}
