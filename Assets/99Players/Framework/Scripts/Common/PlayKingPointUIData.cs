using System;
using UnityEngine;
public class PlayKingPointUIData : MonoBehaviour {
    [Serializable]
    public struct UIData {
        [Header("順位")]
        public SpriteRenderer rank;
        [Header("チ\u30fcム名")]
        public SpriteRenderer teamName;
        [Header("キャラクタ\u30fcアイコン")]
        public SpriteRenderer characterIcon;
        [Header("プレイヤ\u30fcアイコン")]
        public SpriteRenderer playerIcon;
        [Header("CPUアイコン")]
        public SpriteRenderer cpuIcon;
        [Header("ポイント")]
        public SpriteNumbers point;
        [Header("種目結果シ\u30fcル")]
        public SpriteRenderer[] arrayStickerSprite;
    }
    [SerializeField]
    [Header("UIデ\u30fcタ")]
    private UIData uiData;
    private void Awake() {
        uiData.teamName.gameObject.SetActive(value: false);
        uiData.characterIcon.gameObject.SetActive(value: false);
        uiData.playerIcon.gameObject.SetActive(value: false);
        uiData.cpuIcon.gameObject.SetActive(value: false);
        for (int i = 0; i < uiData.arrayStickerSprite.Length; i++) {
            uiData.arrayStickerSprite[i].gameObject.SetActive(value: false);
        }
    }
    public void SetRankData(int _rank) {
        string text = "_rank0" + _rank.ToString();
        if (Localize_Define.Language == Localize_Define.LanguageType.English) {
            text = "en" + text;
        }
        uiData.rank.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, text);
    }
    public void SetCharacterIcon(int _teamNo, int _rank) {
        uiData.characterIcon.gameObject.SetActive(value: true);
        switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_teamNo][0]]) {
            case 0:
                uiData.characterIcon.transform.SetLocalPositionY(-30f);
                uiData.characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_yuto_0" + _rank.ToString());
                break;
            case 1:
                uiData.characterIcon.transform.SetLocalPositionY(-30f);
                uiData.characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_hina_0" + _rank.ToString());
                break;
            case 2:
                uiData.characterIcon.transform.SetLocalPositionY(-30f);
                uiData.characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_ituki_0" + _rank.ToString());
                break;
            case 3:
                uiData.characterIcon.transform.SetLocalPositionY(-30f);
                uiData.characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_souta_0" + _rank.ToString());
                break;
            case 4:
                uiData.characterIcon.transform.SetLocalPositionY(-30f);
                uiData.characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_takumi_0" + _rank.ToString());
                break;
            case 6:
                uiData.characterIcon.transform.SetLocalPositionY(-30f);
                uiData.characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_akira_0" + _rank.ToString());
                break;
            case 5:
                uiData.characterIcon.transform.SetLocalPositionY(-30f);
                uiData.characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_rin_0" + _rank.ToString());
                break;
            case 7:
                uiData.characterIcon.transform.SetLocalPositionY(-30f);
                uiData.characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_rui_0" + _rank.ToString());
                break;
        }
    }
    public void SetTeamNameIcon(int _teamNo) {
        uiData.teamName.gameObject.SetActive(value: true);
        string str = "";
        if (Localize_Define.Language == Localize_Define.LanguageType.English) {
            str = "en";
        }
        switch (_teamNo) {
            case 0:
                str += "_A_team";
                uiData.teamName.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                break;
            case 1:
                str += "_B_team";
                uiData.teamName.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                break;
        }
    }
    public void SetPlayerIcon(int _teamNo) {
        if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_teamNo][0] < 6) {
            uiData.playerIcon.gameObject.SetActive(value: true);
            uiData.playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_" + (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_teamNo][0] + 1).ToString() + "p");
            return;
        }
        uiData.cpuIcon.gameObject.SetActive(value: true);
        switch (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_teamNo][0]) {
            case 6:
                uiData.cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp1");
                break;
            case 7:
                uiData.cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp2");
                break;
            case 8:
                uiData.cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp3");
                break;
            case 9:
                uiData.cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp4");
                break;
            case 10:
                uiData.cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp5");
                break;
            case 11:
                uiData.cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp6");
                break;
            case 12:
                uiData.cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp7");
                break;
        }
    }
    public void SetPoint(int _point) {
        uiData.point.Set(_point);
    }
    public void SetSticker(int _stickerNo, int _rank) {
        uiData.arrayStickerSprite[_stickerNo].gameObject.SetActive(value: true);
        switch (_rank) {
            case 0:
                uiData.arrayStickerSprite[_stickerNo].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "sticker_0" + _stickerNo.ToString() + "_gold");
                break;
            case 1:
                uiData.arrayStickerSprite[_stickerNo].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "sticker_0" + _stickerNo.ToString() + "_silver");
                break;
            case 2:
                uiData.arrayStickerSprite[_stickerNo].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "sticker_0" + _stickerNo.ToString() + "_copper");
                break;
            case 3:
                uiData.arrayStickerSprite[_stickerNo].gameObject.SetActive(value: false);
                break;
        }
    }
}
