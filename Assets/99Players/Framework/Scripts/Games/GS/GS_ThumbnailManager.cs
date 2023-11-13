using UnityEngine;
public class GS_ThumbnailManager : SingletonCustom<GS_ThumbnailManager> {
    [SerializeField]
    [Header("ゲ\u30fcムサムネイル画像")]
    private Sprite[] arraySpGameThumbnail;
    [SerializeField]
    [Header("ゲ\u30fcムサムネイル画像（英語）")]
    private Sprite[] arraySpGameThumbnail_En;
    [SerializeField]
    [Header("ゲ\u30fcムサムネイル画像：6人")]
    private Sprite[] arraySpGameThumbnail6;
    [SerializeField]
    [Header("ゲ\u30fcムサムネイル画像：6人（英語）")]
    private Sprite[] arraySpGameThumbnail6_En;
    [SerializeField]
    [Header("ゲ\u30fcム名画像")]
    private Sprite[] arraySpGameTitle;
    [SerializeField]
    [Header("ゲ\u30fcム名画像（英語）")]
    private Sprite[] arraySpGameTitleEn;
    [SerializeField]
    [Header("選択番号画像")]
    private Sprite[] arraySpSelectNumber;
    [SerializeField]
    [Header("空ゲ\u30fcムボタン画像")]
    private Sprite[] arraySpEmptyButton;
    [SerializeField]
    [Header("ゲ\u30fcムボタン画像")]
    private Sprite[] arraySpGameButton;
    public Sprite GetThumbnail(int _idx) {
        if (Localize_Define.Language == Localize_Define.LanguageType.Japanese) {
            if (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup) {
                return arraySpGameThumbnail6[_idx];
            }
            return arraySpGameThumbnail[_idx];
        }
        if (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup) {
            if (!(arraySpGameThumbnail6_En[_idx] == null)) {
                return arraySpGameThumbnail6_En[_idx];
            }
            return arraySpGameThumbnail6[_idx];
        }
        if (!(arraySpGameThumbnail_En[_idx] == null)) {
            return arraySpGameThumbnail_En[_idx];
        }
        return arraySpGameThumbnail[_idx];
    }
    public Sprite GetGameTitle(int _idx) {
        UnityEngine.Debug.Log("■■[GetGameTitle]: " + _idx + ", " + GS_GameSelectManager.Instance.ArrayCursorGameType[_idx]);
        if (Localize_Define.Language == Localize_Define.LanguageType.English) {
            return arraySpGameTitleEn[_idx];
        }
        return arraySpGameTitle[_idx];
    }
    public Sprite GetSelectNumber(int _idx) {
        return arraySpSelectNumber[_idx];
    }
    public Sprite GetEmptyButton(int _idx) {
        return arraySpEmptyButton[_idx];
    }
    public Sprite GetGameButton(int _idx) {
        return arraySpGameButton[_idx];
    }
}
