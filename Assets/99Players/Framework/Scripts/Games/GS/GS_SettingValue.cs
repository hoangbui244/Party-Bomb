using I2.Loc;
using TMPro;
using UnityEngine;
public class GS_SettingValue : GS_SettingRow {
    #region Inspector
    [field: Header("References")]
    [field: SerializeField] public Localize ValueTextLocalize { get; private set; }
    #endregion
    [SerializeField]
    [Header("項目タイプ")]
    private GS_Setting.SettingType settingType;
    [SerializeField]
    [Header("左矢印")]
    private GameObject arrowLeft;
    [SerializeField]
    [Header("右矢印")]
    private GameObject arrowRight;
    [SerializeField]
    [Header("テキスト")]
    private TextMeshPro text;
    [SerializeField]
    [Header("画像1")]
    private SpriteRenderer img1;
    [SerializeField]
    [Header("画像2")]
    private SpriteRenderer img2;
    [SerializeField]
    [Header("▼初期値")]
    private int DEF_VALUE = 2;
    [SerializeField]
    [Header("▼最小値")]
    private int VALUE_MIN = 1;
    [SerializeField]
    [Header("▼最大値")]
    private int VALUE_MAX = 5;
    [SerializeField]
    [Header("カ\u30fcソル画像")]
    private Transform charaCursor;
    [SerializeField]
    [Header("カ\u30fcソルキャラ")]
    private Transform[] arrayCursorCharacter;
    private int value;
    public void Start() {
        switch (settingType) {
            case GS_Setting.SettingType.Vibration:
            case GS_Setting.SettingType.StartHelp:
            case GS_Setting.SettingType.Crown:
                break;
            case GS_Setting.SettingType.AiStrength:
                value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
                switch (value) {
                    case 0:
                        ValueTextLocalize.SetTerm("Text_Weak");
                        break;
                    case 1:
                        ValueTextLocalize.SetTerm("Text_Normal");
                        break;
                    case 2:
                        ValueTextLocalize.SetTerm("Text_Strong");
                        break;
                }
                break;
            case GS_Setting.SettingType.CharacterNum:
                value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.characterNumSetting;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 169));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 170));
                        break;
                }
                break;
            case GS_Setting.SettingType.Map:
                value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.map;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 159));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 160));
                        break;
                }
                break;
            case GS_Setting.SettingType.Style:
                value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.style;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 164));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 165));
                        break;
                    case 2:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 166));
                        break;
                    case 3:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 167));
                        break;
                }
                break;
            case GS_Setting.SettingType.GameSelectIcon:
                value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.resultCharacterNo;
                charaCursor.SetPositionX(arrayCursorCharacter[value].position.x);
                SingletonCustom<GS_GameSelectManager>.Instance.Refresh();
                break;
            case GS_Setting.SettingType.GameSelectNumRed:
                value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumRed;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                SetGameSelectNumMax();
                break;
            case GS_Setting.SettingType.GameSelectNumBlue:
                value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumBlue;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                SetGameSelectNumMax();
                break;
            case GS_Setting.SettingType.GameSelectNumYellow:
                value = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumYellow;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                SetGameSelectNumMax();
                break;
        }
    }
    private void SetGameSelectNumMax() {
        int num = 19;
        if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_1_IDX)) {
            num += 10;
        }
        if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_2_IDX)) {
            num += 6;
        }
        if (SingletonCustom<AocAssetBundleManager>.Instance.IsDlcEnableId(DlcDataDefine.DLC_3_IDX)) {
            num += 6;
        }
        if (num < GS_Setting.GameSelectNum[4]) {
            VALUE_MAX = 3;
        }
        else if (num < GS_Setting.GameSelectNum[5]) {
            VALUE_MAX = 4;
        }
        else {
            VALUE_MAX = 5;
        }
    }
    public override void InputRight() {
        value++;
        if (value > VALUE_MAX) {
            value = 0;
        }
        switch (settingType) {
            case GS_Setting.SettingType.AiStrength:
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength = value;
                switch (value) {
                    case 0:
                        ValueTextLocalize.SetTerm("Text_Weak");
                        break;
                    case 1:
                        ValueTextLocalize.SetTerm("Text_Normal");
                        break;
                    case 2:
                        ValueTextLocalize.SetTerm("Text_Strong");
                        break;
                }
                break;
            case GS_Setting.SettingType.CharacterNum:
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.characterNumSetting = value;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 169));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 170));
                        break;
                }
                break;
            case GS_Setting.SettingType.Map:
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.map = value;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 159));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 160));
                        break;
                }
                break;
            case GS_Setting.SettingType.Style:
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.style = value;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 164));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 165));
                        break;
                    case 2:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 166));
                        break;
                    case 3:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 167));
                        break;
                }
                break;
            case GS_Setting.SettingType.GameSelectIcon:
                value = (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.resultCharacterNo = value);
                charaCursor.SetPositionX(arrayCursorCharacter[value].position.x);
                SingletonCustom<GS_GameSelectManager>.Instance.Refresh();
                break;
            case GS_Setting.SettingType.GameSelectNumRed:
                while (value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumBlue || value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumYellow) {
                    value++;
                    if (value > VALUE_MAX) {
                        value = 0;
                    }
                }
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumRed = value;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                break;
            case GS_Setting.SettingType.GameSelectNumBlue:
                while (value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumRed || value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumYellow) {
                    value++;
                    if (value > VALUE_MAX) {
                        value = 0;
                    }
                }
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumBlue = value;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                break;
            case GS_Setting.SettingType.GameSelectNumYellow:
                while (value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumRed || value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumBlue) {
                    value++;
                    if (value > VALUE_MAX) {
                        value = 0;
                    }
                }
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumYellow = value;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                break;
        }
        if (arrowRight.activeSelf) {
            arrowRight.transform.SetLocalScale(1f, 1f, 1f);
            LeanTween.scale(arrowRight, new Vector3(1.1f, 1.1f), 0.15f).setLoopType(LeanTweenType.pingPong).setLoopCount(1);
        }
    }
    public override void InputLeft() {
        value--;
        if (value < VALUE_MIN) {
            value = VALUE_MAX;
        }
        switch (settingType) {
            case GS_Setting.SettingType.AiStrength:
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength = value;
                switch (value) {
                    case 0:
                        ValueTextLocalize.SetTerm("Text_Weak");
                        break;
                    case 1:
                        ValueTextLocalize.SetTerm("Text_Normal");
                        break;
                    case 2:
                        ValueTextLocalize.SetTerm("Text_Strong");
                        break;
                }
                break;
            case GS_Setting.SettingType.CharacterNum:
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.characterNumSetting = value;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 169));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 170));
                        break;
                }
                break;
            case GS_Setting.SettingType.Map:
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.map = value;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 159));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 160));
                        break;
                }
                break;
            case GS_Setting.SettingType.Style:
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.style = value;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 164));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 165));
                        break;
                    case 2:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 166));
                        break;
                    case 3:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 167));
                        break;
                }
                break;
            case GS_Setting.SettingType.GameSelectIcon:
                value = (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.resultCharacterNo = value);
                charaCursor.SetPositionX(arrayCursorCharacter[value].position.x);
                SingletonCustom<GS_GameSelectManager>.Instance.Refresh();
                break;
            case GS_Setting.SettingType.GameSelectNumRed:
                while (value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumBlue || value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumYellow) {
                    value--;
                    if (value < VALUE_MIN) {
                        value = VALUE_MAX;
                    }
                }
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumRed = value;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                break;
            case GS_Setting.SettingType.GameSelectNumBlue:
                while (value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumRed || value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumYellow) {
                    value--;
                    if (value < VALUE_MIN) {
                        value = VALUE_MAX;
                    }
                }
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumBlue = value;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                break;
            case GS_Setting.SettingType.GameSelectNumYellow:
                while (value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumRed || value == SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumBlue) {
                    value--;
                    if (value < VALUE_MIN) {
                        value = VALUE_MAX;
                    }
                }
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumYellow = value;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                break;
        }
        if (arrowLeft.activeSelf) {
            arrowLeft.transform.SetLocalScale(1f, 1f, 1f);
            LeanTween.scale(arrowLeft, new Vector3(1.1f, 1.1f), 0.15f).setLoopType(LeanTweenType.pingPong).setLoopCount(1);
        }
    }
    public override void Reset() {
        base.Reset();
        switch (settingType) {
            case GS_Setting.SettingType.Vibration:
            case GS_Setting.SettingType.StartHelp:
            case GS_Setting.SettingType.Crown:
                break;
            case GS_Setting.SettingType.AiStrength:
                value = 1;
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength = value;
                switch (value) {
                    case 0:
                        ValueTextLocalize.SetTerm("Text_Weak");
                        break;
                    case 1:
                        ValueTextLocalize.SetTerm("Text_Normal");
                        break;
                    case 2:
                        ValueTextLocalize.SetTerm("Text_Strong");
                        break;
                }
                break;
            case GS_Setting.SettingType.CharacterNum:
                value = DEF_VALUE;
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.characterNumSetting = value;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 169));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 170));
                        break;
                }
                break;
            case GS_Setting.SettingType.Map:
                value = 0;
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.map = value;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 159));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 160));
                        break;
                }
                break;
            case GS_Setting.SettingType.Style:
                value = 0;
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.style = value;
                switch (value) {
                    case 0:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 164));
                        break;
                    case 1:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 165));
                        break;
                    case 2:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 166));
                        break;
                    case 3:
                        text.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 167));
                        break;
                }
                break;
            case GS_Setting.SettingType.GameSelectIcon:
                value = 1;
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.resultCharacterNo = value;
                charaCursor.SetPositionX(arrayCursorCharacter[value].position.x);
                SingletonCustom<GS_GameSelectManager>.Instance.Refresh();
                break;
            case GS_Setting.SettingType.GameSelectNumRed:
                value = 0;
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumRed = value;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                break;
            case GS_Setting.SettingType.GameSelectNumBlue:
                value = 3;
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumBlue = value;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                break;
            case GS_Setting.SettingType.GameSelectNumYellow:
                value = 4;
                SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.gameSelectNumYellow = value;
                text.SetText(GS_Setting.GameSelectNum[value].ToString());
                break;
        }
    }
}