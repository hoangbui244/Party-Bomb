using TMPro;
using UnityEngine;
public class GS_FreeSelect : MonoBehaviour {
    [SerializeField]
    [Header("ゲ\u30fcムセレクトの設定ボタン配列")]
    public GameObject[] arrayObjSetting_DLC;
    [SerializeField]
    [Header("カ\u30fcソル：ゲ\u30fcムセレクト")]
    public CursorManager cursorGameSelect;
    [SerializeField]
    [Header("画面下部設定オブジェクト")]
    public GameObject objSetting;
    [SerializeField]
    [Header("画面下部コントロ\u30fcラ\u30fcオブジェクト")]
    public GameObject objController;
    [SerializeField]
    [Header("ゲ\u30fcム名テキスト")]
    public SpriteRenderer textGame;
    [SerializeField]
    [Header("ゲ\u30fcム説明（日本語）")]
    public TextMeshPro textGameInfo;
    [SerializeField]
    [Header("ゲ\u30fcムサムネイル")]
    public SpriteRenderer thumbnailGame;
    [SerializeField]
    [Header("スティック描画")]
    public SpriteRenderer rendererStick;
    [SerializeField]
    [Header("Aボタン描画")]
    public SpriteRenderer rendererButtonA;
    [SerializeField]
    [Header("Bボタン描画")]
    public SpriteRenderer rendererButtonB;
    [SerializeField]
    [Header("協力アイコン")]
    public GameObject objCoopIcon;
    [SerializeField]
    [Header("ハイスコア表示")]
    public GameObject objHiscore;
    [SerializeField]
    [Header("ハイスコアテキスト")]
    public TextMeshPro textHiscore;
    [SerializeField]
    [Header("フリ\u30fcモ\u30fcドAボタンル\u30fcト")]
    public GameObject objRootButtonA;
    [SerializeField]
    [Header("フリ\u30fcモ\u30fcドBボタンル\u30fcト")]
    public GameObject objRootButtonB;
    public void OnSelectButtonDown() {
        GS_GameSelectManager.Instance.OnSelectButtonDown();
    }
    public void OnReturnButtonDown() {
        GS_GameSelectManager.Instance.OnReturnButtonDown();
    }
    public void OnRandomGameButtonDown() {
        GS_GameSelectManager.Instance.OnRandomGameButtonDown();
    }
}
