using UnityEngine;
public class ControllerOperationBalloonUI : MonoBehaviour {
    private ControllerBalloonUI[] arrayControllerUI;
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fcUI（JP）")]
    private ControllerBalloonUI[] arrayControllerUI_JP;
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fcUI（EN）")]
    private ControllerBalloonUI[] arrayControllerUI_EN;
    public void Init(bool _isPlayer) {
        for (int i = 0; i < arrayControllerUI_JP.Length; i++) {
            if (!(arrayControllerUI_JP[i] == null)) {
                arrayControllerUI_JP[i].gameObject.SetActive(value: false);
            }
        }
        for (int j = 0; j < arrayControllerUI_EN.Length; j++) {
            if (!(arrayControllerUI_EN[j] == null)) {
                arrayControllerUI_EN[j].gameObject.SetActive(value: false);
            }
        }
        if (!_isPlayer) {
            return;
        }
        arrayControllerUI = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? arrayControllerUI_JP : arrayControllerUI_EN);
        for (int k = 0; k < arrayControllerUI.Length; k++) {
            if (!(arrayControllerUI[k] == null)) {
                arrayControllerUI[k].Init();
            }
        }
    }
    public void SetControllerBalloonActive(int _balloonIdx, bool _isFade, bool _isActive) {
        if (arrayControllerUI != null && !(arrayControllerUI[_balloonIdx] == null)) {
            if (_isFade) {
                arrayControllerUI[_balloonIdx].FadeProcess_ControlInfomationUI(_isActive);
            } else {
                arrayControllerUI[_balloonIdx].SetControlInfomationUIActive(_isActive);
            }
        }
    }
    public void HideUI() {
        if (arrayControllerUI == null) {
            return;
        }
        for (int i = 0; i < arrayControllerUI.Length; i++) {
            if (!(arrayControllerUI[i] == null)) {
                arrayControllerUI[i].SetControlInfomationUIActive(_isActive: false);
            }
        }
    }
}
