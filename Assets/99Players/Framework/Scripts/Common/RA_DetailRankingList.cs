using UnityEngine;
public class RA_DetailRankingList : MonoBehaviour {
    [SerializeField]
    [Header("あそび名")]
    private SpriteRenderer playName;
    [SerializeField]
    [Header("背景色")]
    private SpriteRenderer backColor;
    [SerializeField]
    [Header("フレ\u30fcム画像")]
    private SpriteRenderer frame;
    [SerializeField]
    [Header("順位")]
    private SpriteRenderer[] arrayTeamRank;
    [SerializeField]
    [Header("チ\u30fcム名")]
    private SpriteRenderer[] arrayTeamIcon;
    [SerializeField]
    [Header("キャラクタ\u30fcアイコン")]
    private SpriteRenderer[] arrayCharacterIcon;
    private void Awake() {
        playName.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        backColor.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        frame.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        for (int i = 0; i < arrayTeamRank.Length; i++) {
            arrayTeamRank[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        for (int j = 0; j < arrayTeamIcon.Length; j++) {
            arrayTeamIcon[j].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        for (int k = 0; k < arrayCharacterIcon.Length; k++) {
            arrayCharacterIcon[k].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }
    public void SetPlayName(Sprite _playNameSp) {
        playName.sprite = _playNameSp;
    }
    public void SetRankingData(int _idx, int _rank) {
        arrayTeamRank[_idx].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_rank_details_0" + _rank.ToString());
    }
    public void SetTeamData(int _idx, int _teamNo) {
        arrayTeamIcon[_idx].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, (_teamNo == 0) ? "_A_team" : "_B_team");
    }
    public void SetCharacterIconData(int _idx, int _rank, int _characterNo) {
        switch (_characterNo) {
            case 0:
                arrayCharacterIcon[_idx].transform.SetLocalPositionY(-30f);
                arrayCharacterIcon[_idx].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_yuto_0" + _rank.ToString());
                break;
            case 1:
                arrayCharacterIcon[_idx].transform.SetLocalPositionY(-30f);
                arrayCharacterIcon[_idx].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_hina_0" + _rank.ToString());
                break;
            case 2:
                arrayCharacterIcon[_idx].transform.SetLocalPositionY(-30f);
                arrayCharacterIcon[_idx].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_ituki_0" + _rank.ToString());
                break;
            case 3:
                arrayCharacterIcon[_idx].transform.SetLocalPositionY(-30f);
                arrayCharacterIcon[_idx].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_souta_0" + _rank.ToString());
                break;
            case 4:
                arrayCharacterIcon[_idx].transform.SetLocalPositionY(-30f);
                arrayCharacterIcon[_idx].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_takumi_0" + _rank.ToString());
                break;
            case 6:
                arrayCharacterIcon[_idx].transform.SetLocalPositionY(-30f);
                arrayCharacterIcon[_idx].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_akira_0" + _rank.ToString());
                break;
            case 5:
                arrayCharacterIcon[_idx].transform.SetLocalPositionY(-30f);
                arrayCharacterIcon[_idx].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_rin_0" + _rank.ToString());
                break;
            case 7:
                arrayCharacterIcon[_idx].transform.SetLocalPositionY(-30f);
                arrayCharacterIcon[_idx].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_rui_0" + _rank.ToString());
                break;
        }
    }
}
