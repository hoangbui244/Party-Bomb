using TMPro;
using UnityEngine;
public class CommonCurrentTotalPointUI : MonoBehaviour {
    [SerializeField]
    [Header("カラ\u30fc設定")]
    private Color[] arrayGaugeColor;
    private bool isCommonColor;
    [SerializeField]
    [Header("プレイヤ\u30fcポイント描画元")]
    private SpriteRenderer spPlayerPoint;
    [SerializeField]
    [Header("各プレイヤ\u30fcポイント描画元")]
    private SpriteRenderer[] arraySpPlayerPoint;
    [SerializeField]
    [Header("目標ポイント分割値テキスト")]
    private TextMeshPro[] arrayTextSplitPoint;
    [SerializeField]
    [Header("目標ポイントテキスト")]
    private TextMeshPro textTotalPoint;
    [SerializeField]
    [Header("コンプリ\u30fcト描画")]
    private SpriteRenderer spComplete;
    [SerializeField]
    [Header("コンプリ\u30fcトカラ\u30fc設定")]
    private Color completeColor;
    private float pointTotalWidth;
    private int targetPoint;
    private float[] arrayPlayerPoint;
    private int totalPoint;
    private string lastScoreText;
    private bool isScale;
    public void AddCurrentPoint(int _point, int _playerNo) {
        isCommonColor = (_playerNo < 0);
        if (isCommonColor) {
            _playerNo = 0;
        }
        switch (_playerNo) {
            case 4:
                _playerNo = 5;
                break;
            case 5:
                _playerNo = 4;
                break;
        }
        arrayPlayerPoint[_playerNo] += _point;
        totalPoint += _point;
        if (totalPoint < targetPoint) {
            float num = 0f;
            float num2 = 0f;
            float num3 = pointTotalWidth;
            for (int i = 0; i < arrayPlayerPoint.Length; i++) {
                float num4 = arrayPlayerPoint[i] / (float)targetPoint;
                float num9 = ((float)targetPoint - num2) / (float)targetPoint;
                float num5 = Mathf.Clamp(pointTotalWidth * num4, 0f, num3);
                arraySpPlayerPoint[i].size = new Vector2(num5, arraySpPlayerPoint[i].size.y);
                arraySpPlayerPoint[i].color = arrayGaugeColor[isCommonColor ? (arrayGaugeColor.Length - 1) : i];
                arraySpPlayerPoint[i].transform.SetLocalPositionX(spPlayerPoint.transform.localPosition.x + num);
                arraySpPlayerPoint[i].gameObject.SetActive(value: true);
                num2 = Mathf.Clamp(num2 + (float)_point, 0f, targetPoint);
                num += pointTotalWidth * num4;
                num3 -= num5;
            }
            spComplete.color = Color.white;
            textTotalPoint.color = Color.white;
            if (isScale) {
                LeanTween.cancel(spComplete.gameObject);
                isScale = false;
                spComplete.transform.SetLocalScale(1f, 1f, 1f);
                textTotalPoint.SetText(lastScoreText);
            }
        } else {
            float num6 = 0f;
            float num7 = 0f;
            for (int j = 0; j < arrayPlayerPoint.Length; j++) {
                float num8 = arrayPlayerPoint[j] / (float)totalPoint;
                float num10 = ((float)totalPoint - num7) / (float)totalPoint;
                float x = Mathf.Clamp(pointTotalWidth * num8, 0f, pointTotalWidth);
                arraySpPlayerPoint[j].size = new Vector2(x, arraySpPlayerPoint[j].size.y);
                arraySpPlayerPoint[j].color = arrayGaugeColor[isCommonColor ? (arrayGaugeColor.Length - 1) : j];
                arraySpPlayerPoint[j].transform.SetLocalPositionX(spPlayerPoint.transform.localPosition.x + num6);
                arraySpPlayerPoint[j].gameObject.SetActive(value: true);
                num7 = Mathf.Clamp(num7 + (float)_point, 0f, totalPoint);
                num6 += pointTotalWidth * num8;
            }
            if (!isScale) {
                spComplete.color = completeColor;
                textTotalPoint.color = completeColor;
                isScale = false;
                LeanTween.scale(spComplete.gameObject, Vector3.one * 1.1f, 0.5f).setEaseInOutSine().setLoopPingPong();
                isScale = true;
            }
            textTotalPoint.SetText(totalPoint.ToString());
        }
    }
    public void SetCurrentPoint(int _point, int _prevPoint = -1) {
    }
    public void SetTargetTotalPoint(int _point) {
        targetPoint = _point;
        int num = _point / 5;
        for (int i = 0; i < arrayTextSplitPoint.Length; i++) {
            arrayTextSplitPoint[i].SetText((num * (i + 1)).ToString());
        }
        lastScoreText = _point.ToString();
        textTotalPoint.SetText(_point.ToString());
    }
    private void Awake() {
        arrayPlayerPoint = new float[GS_Define.PLAYER_MAX];
        pointTotalWidth = spPlayerPoint.size.x;
    }
    private void OnDestroy() {
        LeanTween.cancel(spComplete.gameObject);
    }
}
