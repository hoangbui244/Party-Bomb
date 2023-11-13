using UnityEngine;
public class StyleTest : MonoBehaviour {
    [SerializeField]
    [Header("スタイル")]
    private CharacterStyle style;
    [SerializeField]
    [Header("番号")]
    private int idx;
    [SerializeField]
    [Header("ゲ\u30fcムスタイル設定")]
    private bool isGameStyle;
    [SerializeField]
    [Header("ゲ\u30fcムタイプ")]
    private GS_Define.GameType gameType;
    private void Start() {
        if (isGameStyle) {
            style.SetGameStyle(gameType, idx);
        } else {
            style.SetMainCharacterStyle(idx);
        }
    }
}
