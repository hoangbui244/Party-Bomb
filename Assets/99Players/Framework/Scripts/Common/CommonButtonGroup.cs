using System;
using TMPro;
using UnityEngine;
public class CommonButtonGroup : MonoBehaviour {
    private CommonButtonExplanationUI explanationUI;
    [SerializeField]
    private GameObject buttonRoot;
    [SerializeField]
    private SpriteRenderer leftButton;
    [SerializeField]
    private SpriteRenderer rightButton;
    [SerializeField]
    private GameObject allButtonRoot;
    [SerializeField]
    private SpriteRenderer underlay;
    [SerializeField]
    private TextMeshPro text;
    private float defButtonRootDiffUnderlayX;
    private const float UNDERLAY_SIDE_SPACE = 31.55f;
    private float defTextPosX;
    private float defTextSizeDeltaX;
    private float defTextBoundsMaxX;
    private float originFontSize;
    private readonly int DEF_FONT_SIZE_TEXT_LENGTH = 17;
    private readonly int CHNAGE_FONT_SIZE_TEXT_LENGTH = 19;
    [Header("※ デバッグ用 ※")]
    public bool isDebug;
    public string debugText;
    public float DEBUG_DEF_BUTTON_ROOT_DIFF_UNDERLAY_X = -43f;
    public float DEBUG_UNDERLAY_SIDE_SPACE = 31.55f;
    public float DEBUG_DEF_TEXT_POS_X = 55.3f;
    public float DEBUG_DEF_TEXT_SIZE_DELTA_X = 81.53f;
    public float DEBUG_DEF_TEXT_BOUNDS_MAX_X = 69.66347f;
    public void Init(CommonButtonExplanationUI _explanationUI) {
        explanationUI = _explanationUI;
        defButtonRootDiffUnderlayX = buttonRoot.transform.position.x - underlay.bounds.min.x;
        defTextPosX = text.transform.localPosition.x;
        defTextSizeDeltaX = text.rectTransform.sizeDelta.x;
        originFontSize = text.fontSize;
        LeanTween.delayedCall(base.gameObject, 0.01f, (Action)delegate {
            defTextBoundsMaxX = text.bounds.max.x;
            base.gameObject.SetActive(value: false);
        });
    }
    public void SetButton(Sprite _sprite, float _rot, string _text) {
        leftButton.gameObject.SetActive(value: false);
        rightButton.sprite = _sprite;
        rightButton.transform.SetLocalEulerAnglesZ(_rot);
        rightButton.gameObject.SetActive(value: true);
        allButtonRoot.SetActive(value: false);
        SetAdjustLayout(_text);
    }
    public void SetButton(Sprite _leftButtonSprite, Sprite _rightButtonSprite, string _text) {
        leftButton.sprite = _leftButtonSprite;
        leftButton.gameObject.SetActive(value: true);
        rightButton.sprite = _rightButtonSprite;
        rightButton.gameObject.SetActive(value: true);
        allButtonRoot.SetActive(value: false);
        SetAdjustLayout(_text);
    }
    public void SetButton(string _text) {
        leftButton.gameObject.SetActive(value: false);
        rightButton.gameObject.SetActive(value: false);
        allButtonRoot.SetActive(value: true);
        SetAdjustLayout(_text);
    }
    private void SetAdjustLayout(string _text) {
        if (_text.Length < DEF_FONT_SIZE_TEXT_LENGTH) {
            text.fontSize = originFontSize;
        } else if (_text.Length <= CHNAGE_FONT_SIZE_TEXT_LENGTH) {
            text.fontSize = originFontSize * 0.9f;
        }
        text.text = _text;
        if (text.preferredWidth > defTextSizeDeltaX) {
            text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
            float num = defTextSizeDeltaX - text.rectTransform.sizeDelta.x;
            text.rectTransform.SetLocalPositionX(defTextPosX + num);
            LeanTween.delayedCall(base.gameObject, 0.01f, (Action)delegate {
                underlay.size = new Vector2(31.55f + text.bounds.max.x + 31.55f, underlay.size.y);
                buttonRoot.transform.SetPositionX(underlay.bounds.min.x + defButtonRootDiffUnderlayX);
            });
        } else {
            text.rectTransform.sizeDelta = new Vector2(defTextSizeDeltaX, text.preferredHeight);
            text.rectTransform.SetLocalPositionX(defTextPosX);
            LeanTween.delayedCall(base.gameObject, 0.01f, (Action)delegate {
                underlay.size = new Vector2(31.55f + defTextBoundsMaxX + 31.55f, underlay.size.y);
                buttonRoot.transform.SetPositionX(underlay.bounds.min.x + defButtonRootDiffUnderlayX);
            });
        }
    }
    private void OnDrawGizmos() {
        if (isDebug) {
            text.text = debugText;
            Vector3 max;
            if (text.preferredWidth > DEBUG_DEF_TEXT_SIZE_DELTA_X) {
                text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
                float num = DEBUG_DEF_TEXT_SIZE_DELTA_X - text.rectTransform.sizeDelta.x;
                UnityEngine.Debug.Log("diffSizeX " + num.ToString());
                text.rectTransform.SetLocalPositionX(DEBUG_DEF_TEXT_POS_X + num);
                Vector2 sizeDelta = text.rectTransform.sizeDelta;
                string str = sizeDelta.x.ToString();
                sizeDelta = text.rectTransform.sizeDelta;
                UnityEngine.Debug.Log("size x : " + str + " y : " + sizeDelta.y.ToString());
                max = text.bounds.max;
                string str2 = max.x.ToString();
                max = text.bounds.min;
                UnityEngine.Debug.Log("text.bounds.max.x : " + str2 + " min.x : " + max.x.ToString());
                underlay.size = new Vector2(DEBUG_UNDERLAY_SIDE_SPACE + text.bounds.max.x + DEBUG_UNDERLAY_SIDE_SPACE, underlay.size.y);
            } else {
                text.rectTransform.sizeDelta = new Vector2(DEBUG_DEF_TEXT_SIZE_DELTA_X, text.preferredHeight);
                text.rectTransform.SetLocalPositionX(DEBUG_DEF_TEXT_POS_X);
                underlay.size = new Vector2(DEBUG_UNDERLAY_SIDE_SPACE + DEBUG_DEF_TEXT_BOUNDS_MAX_X + DEBUG_UNDERLAY_SIDE_SPACE, underlay.size.y);
            }
            max = underlay.bounds.max;
            string str3 = max.x.ToString();
            max = underlay.bounds.min;
            UnityEngine.Debug.Log("underlay.bounds.max.x : " + str3 + " min.x : " + max.x.ToString());
            buttonRoot.transform.SetPositionX(underlay.bounds.min.x + DEBUG_DEF_BUTTON_ROOT_DIFF_UNDERLAY_X);
        }
    }
}
