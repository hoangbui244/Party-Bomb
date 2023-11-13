using UnityEngine;
public class ControllerOperationExplanationUI : MonoBehaviour {
    private int controllerTypeIdx;
    private GameObject[] arrayControllerUI;
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fcUI（JP）")]
    private GameObject[] arrayControllerUI_JP;
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fcUI（EN）")]
    private GameObject[] arrayControllerUI_EN;
    public void Init(bool _isPlayer) {
        for (int i = 0; i < arrayControllerUI_JP.Length; i++) {
            arrayControllerUI_JP[i].SetActive(value: false);
        }
        for (int j = 0; j < arrayControllerUI_EN.Length; j++) {
            arrayControllerUI_EN[j].SetActive(value: false);
        }
        if (_isPlayer) {
            arrayControllerUI = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? arrayControllerUI_JP : arrayControllerUI_EN);
            controllerTypeIdx = 0;
            arrayControllerUI[controllerTypeIdx].SetActive(value: true);
        }
    }
    public void ChangeControllerUIType(int _controllerTypeIdx) {
        arrayControllerUI[controllerTypeIdx].SetActive(value: false);
        controllerTypeIdx = _controllerTypeIdx;
        arrayControllerUI[controllerTypeIdx].SetActive(value: true);
    }
}
