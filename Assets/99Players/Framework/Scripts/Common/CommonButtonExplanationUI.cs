using System;
using System.Collections.Generic;
using UnityEngine;
public class CommonButtonExplanationUI : MonoBehaviour {
    public struct ButtonExplanation {
        public int idx;
        public ButtonType leftButtonType;
        public ButtonType rightButtonType;
        public string text;
    }
    public enum ButtonType {
        ZL,
        ZR,
        LStick,
        RStick,
        A,
        B,
        X,
        Y,
        All,
        Up,
        Down,
        Left,
        Right,
        None
    }
    private CommonButtonGroup[] arrayButtonGroup;
    private float underlayBoundsMax;
    private List<ButtonExplanation> buttonExplanationList = new List<ButtonExplanation>();
    private ButtonExplanation buttonExplanation;
    [SerializeField]
    [Header("ボタン画像（Gamepad）")]
    private Sprite[] arrayGamepadButtonSprite;
    [SerializeField]
    [Header("ボタン画像（Keyboard）")]
    private Sprite[] arrayKeyboardButtonSprite;
    public void Init() {
        arrayButtonGroup = GetComponentsInChildren<CommonButtonGroup>();
        for (int i = 0; i < arrayButtonGroup.Length; i++) {
            arrayButtonGroup[i].Init(this);
        }
    }
    public void AddButtonExplanation(int _idx, string _text, ButtonType _leftButtonType, ButtonType _rightButtonType) {
        if (_rightButtonType == ButtonType.None) {
            UnityEngine.Debug.Log("※ 右側のボタンが設定されていません");
            return;
        }
        if (_text == "") {
            UnityEngine.Debug.Log("※ テキストが設定れていません");
            return;
        }
        buttonExplanation.idx = _idx;
        buttonExplanation.leftButtonType = _leftButtonType;
        buttonExplanation.rightButtonType = _rightButtonType;
        buttonExplanation.text = _text;
        buttonExplanationList.Add(buttonExplanation);
    }
    public void SetButtonExplanation() {
        LeanTween.delayedCall(base.gameObject, (float)buttonExplanationList.Count * 0.01f, (Action)delegate {
            for (int i = 0; i < buttonExplanationList.Count; i++) {
                if (buttonExplanationList[i].leftButtonType == ButtonType.None) {
                    if (buttonExplanationList[i].rightButtonType != ButtonType.All) {
                        SetButton(buttonExplanationList[i].idx, buttonExplanationList[i].rightButtonType, buttonExplanationList[i].text);
                    } else {
                        SetButton(buttonExplanationList[i].idx, buttonExplanationList[i].text);
                    }
                } else {
                    SetButton(buttonExplanationList[i].idx, buttonExplanationList[i].leftButtonType, buttonExplanationList[i].rightButtonType, buttonExplanationList[i].text);
                }
            }
        });
    }
    private void SetButton(int _idx, ButtonType _buttonType, string _text) {
        Sprite sprite = (SingletonCustom<JoyConManager>.Instance.controlMode[0] == JoyConManager.ControlMode.Gamepad) ? arrayGamepadButtonSprite[(int)_buttonType] : arrayKeyboardButtonSprite[(int)_buttonType];
        float rot = 0f;
        arrayButtonGroup[_idx].SetButton(sprite, rot, _text);
        arrayButtonGroup[_idx].gameObject.SetActive(value: true);
    }
    private void SetButton(int _idx, ButtonType _leftButtonType, ButtonType _rightButtonType, string _text) {
        Sprite leftButtonSprite = (SingletonCustom<JoyConManager>.Instance.controlMode[0] == JoyConManager.ControlMode.Gamepad) ? arrayGamepadButtonSprite[(int)_leftButtonType] : arrayKeyboardButtonSprite[(int)_leftButtonType];
        Sprite rightButtonSprite = (SingletonCustom<JoyConManager>.Instance.controlMode[0] == JoyConManager.ControlMode.Gamepad) ? arrayGamepadButtonSprite[(int)_rightButtonType] : arrayKeyboardButtonSprite[(int)_rightButtonType];
        arrayButtonGroup[_idx].SetButton(leftButtonSprite, rightButtonSprite, _text);
    }
    private void SetButton(int _idx, string _text) {
        arrayButtonGroup[_idx].SetButton(_text);
        arrayButtonGroup[_idx].gameObject.SetActive(value: true);
    }
    public void SetUnderlayBoundsMax(float _underlaySize) {
        if (underlayBoundsMax < _underlaySize) {
            underlayBoundsMax = _underlaySize;
        }
    }
    private void OnEnable() {
        SingletonCustom<JoyConManager>.Instance.OnMainPlayerControlModeChanged += OnChanged;
        OnChanged(SingletonCustom<JoyConManager>.Instance.controlMode[0]);
    }
    private void OnDisable() {
        SingletonCustom<JoyConManager>.Instance.OnMainPlayerControlModeChanged -= OnChanged;
    }
    private void OnChanged(JoyConManager.ControlMode mode) {
        SetButtonExplanation();
    }
}
