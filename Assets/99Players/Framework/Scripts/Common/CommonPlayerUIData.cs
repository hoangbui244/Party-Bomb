using TMPro;
using UnityEngine;
public class CommonPlayerUIData : MonoBehaviour {
    private CommonUIManager.UIType uiType;
    [SerializeField]
    [Header("プレイヤ\u30fcアイコンの背景色画像")]
    private SpriteRenderer playerIconBack;
    [SerializeField]
    [Header("プレイヤ\u30fcアイコン")]
    private TextMeshPro playerText;
    [SerializeField]
    [Header("ポイントテキスト")]
    private TextMeshPro pointText;
    [SerializeField]
    [Header("順位")]
    private TextMeshPro rankText;
    [SerializeField]
    [Header("タイマ\u30fcテキスト")]
    private TextMeshPro timeText;
    public void Init(CommonUIManager.UIType _uiType) {
        uiType = _uiType;
        switch (uiType) {
            case CommonUIManager.UIType.UpperUI_Point:
            case CommonUIManager.UIType.SideUI_Point:
            case CommonUIManager.UIType.BottomUI_Point:
                pointText.text = "0";
                pointText.gameObject.SetActive(value: true);
                rankText.gameObject.SetActive(value: false);
                timeText.gameObject.SetActive(value: false);
                break;
            case CommonUIManager.UIType.UpperUI_Rank:
            case CommonUIManager.UIType.SideUI_Rank:
            case CommonUIManager.UIType.BottomUI_Rank:
                pointText.gameObject.SetActive(value: false);
                rankText.gameObject.SetActive(value: false);
                timeText.gameObject.SetActive(value: false);
                break;
            case CommonUIManager.UIType.UpperUI_Time:
            case CommonUIManager.UIType.SideUI_Time:
            case CommonUIManager.UIType.BottomUI_Time:
                pointText.gameObject.SetActive(value: false);
                rankText.gameObject.SetActive(value: false);
                timeText.text = "- -.- -";
                timeText.gameObject.SetActive(value: true);
                break;
            case CommonUIManager.UIType.UpperUI_Empty:
            case CommonUIManager.UIType.SideUI_Empty:
            case CommonUIManager.UIType.BottomUI_Empty:
                pointText.gameObject.SetActive(value: false);
                rankText.gameObject.SetActive(value: false);
                timeText.gameObject.SetActive(value: false);
                break;
        }
    }
    public void SetPlayerIcon(int playerId) {
        if (playerId < 6) {
            playerText.text = (playerId + 1) + "P";
        }
        else {
            playerText.text = "CP" + (playerId - 5);
        }
    }
    public void SetPlayerIconBack(Color _color) {
        playerIconBack.color = _color;
    }
    public void SetPoint(int _point, int _prevPoint = -1) {
        if (_prevPoint == -1) {
            pointText.text = _point.ToString();
            return;
        }
        LeanTween.cancel(base.gameObject);
        LeanTween.value(base.gameObject, _prevPoint, _point, 0.25f).setOnUpdate(delegate (float _value) {
            pointText.text = ((int)_value).ToString();
        });
    }
    public void HidePoint(float _delay) {
        Vector3 b = Vector3.zero;
        switch (uiType) {
            case CommonUIManager.UIType.UpperUI_Point:
            case CommonUIManager.UIType.UpperUI_Rank:
            case CommonUIManager.UIType.UpperUI_Time:
                b = new Vector3(0f, 150f, 0f);
                break;
            case CommonUIManager.UIType.SideUI_Point:
            case CommonUIManager.UIType.SideUI_Rank:
            case CommonUIManager.UIType.SideUI_Time:
                b = new Vector3(-300f, 0f, 0f);
                break;
            case CommonUIManager.UIType.BottomUI_Point:
            case CommonUIManager.UIType.BottomUI_Rank:
            case CommonUIManager.UIType.BottomUI_Time:
                b = new Vector3(0f, -150f, 0f);
                break;
        }
        LeanTween.moveLocal(base.gameObject, base.transform.localPosition + b, 1.25f).setEaseInQuint().setDelay(_delay);
    }
    public void SetRank(int rank) {
        // Parameter 'rank' counted from 0
        switch (rank) {
            case 0:
                rankText.text = "1st";
                break;
            case 1:
                rankText.text = "2nd";
                break;
            case 2:
                rankText.text = "3rd";
                break;
            default:
                rankText.text = rank + "th";
                break;
        }
        rankText.gameObject.SetActive(value: true);
    }
    public void SetTime(float _time) {
        if (_time == -1f) {
            timeText.text = "- -.- -";
        } else {
            timeText.text = _time.ToString("F2");
        }
    }
}
