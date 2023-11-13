using UnityEngine;
public class CursorButtonObject : MonoBehaviour {
    [SerializeField]
    [Header("上移動時のオブジェクト番号")]
    public int topNo;
    [SerializeField]
    [Header("下移動時のオブジェクト番号")]
    public int downNo;
    [SerializeField]
    [Header("左移動時のオブジェクト番号")]
    public int leftNo;
    [SerializeField]
    [Header("右移動時のオブジェクト番号")]
    public int rightNo;
    [SerializeField]
    [Header("カ\u30fcソルタイプ")]
    public int cursorType;
    [SerializeField]
    [Header("スケ\u30fcリング")]
    public bool isScale = true;
    [SerializeField]
    [Header("表示矩形情報")]
    private Rect rectSetting;
    public Rect RectSetting {
        get {
            return rectSetting;
        }
        set {
            rectSetting = value;
        }
    }
    public void CallEvent() {
    }
    public void SetData(CursorButtonObject _data) {
        topNo = _data.topNo;
        downNo = _data.downNo;
        leftNo = _data.leftNo;
        rightNo = _data.rightNo;
        cursorType = _data.cursorType;
        isScale = _data.isScale;
    }
}
