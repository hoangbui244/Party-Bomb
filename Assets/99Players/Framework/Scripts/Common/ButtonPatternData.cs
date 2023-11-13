using System;
using UnityEngine;
public class ButtonPatternData : MonoBehaviour {
    [Serializable]
    public struct ButtonUIData {
        [Header("ボタンUI表示アンカ\u30fc")]
        public Transform buttonAnchor;
        [Header("演出するボタン配列")]
        public GameObject[] arrayAnimButton;
    }
    [SerializeField]
    [Header("ボタンパタ\u30fcンデ\u30fcタ")]
    private ButtonUIData buttonUIData;
    private float[] defPosX_OperationButton;
    private void Awake() {
    }
    public void ShowButtonUI() {
        buttonUIData.buttonAnchor.gameObject.SetActive(value: true);
        defPosX_OperationButton = new float[buttonUIData.arrayAnimButton.Length];
        for (int i = 0; i < defPosX_OperationButton.Length; i++) {
            defPosX_OperationButton[i] = buttonUIData.arrayAnimButton[i].transform.localPosition.x;
        }
        for (int j = 0; j < defPosX_OperationButton.Length; j++) {
            buttonUIData.arrayAnimButton[j].transform.SetLocalPositionX(defPosX_OperationButton[j] + 2000f);
        }
    }
    public void ButtonAnimation(float _animTime, Action _action) {
        int num = 0;
        num = buttonUIData.arrayAnimButton.Length - 1;
        for (int i = 0; i < num; i++) {
            LeanTween.moveLocalX(buttonUIData.arrayAnimButton[i], defPosX_OperationButton[i], _animTime).setDelay(_animTime + (float)i * _animTime);
        }
        LeanTween.moveLocalX(buttonUIData.arrayAnimButton[num], defPosX_OperationButton[num], _animTime).setDelay(_animTime + (float)num * _animTime).setOnComplete(_action);
    }
    public bool IsActive() {
        return buttonUIData.buttonAnchor.gameObject.activeSelf;
    }
}
