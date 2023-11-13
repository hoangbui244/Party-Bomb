using System.Collections.Generic;
using UnityEngine;
public class SpriteNumbers : MonoBehaviour {
    private enum VERTICAL_CENTER {
        RIGHT,
        CENTER,
        LEFT
    }
    [SerializeField]
    [Header("数値画像")]
    private SpriteRenderer[] arraySpriteRendererNumber;
    [SerializeField]
    [Header("画像名")]
    private string resSpriteName = "number_";
    [SerializeField]
    [Header("画像が含まれているSpriteAtlasの種類")]
    private SAType atlasType;
    [SerializeField]
    [Header("オブジェクトル\u30fcト")]
    private GameObject root;
    [SerializeField]
    [Header("基準位置オブジェクト")]
    private GameObject startPosObj;
    [SerializeField]
    [Header("寄せ位置")]
    private VERTICAL_CENTER center;
    [SerializeField]
    [Header("寄せ位置をrootのスケ\u30fcルを考慮するかどうか")]
    private bool isAdjustScale;
    [SerializeField]
    [Header("表示間隔")]
    private float numberSpace;
    [SerializeField]
    [Header("左側追従表示オブジェクト")]
    private GameObject objLeftAlign;
    [SerializeField]
    [Header("右側追従表示オブジェクト")]
    private GameObject objRightAlign;
    [SerializeField]
    [Header("0埋め")]
    private bool isZeroFill;
    [SerializeField]
    [Header("マイナス表記")]
    private bool isNumberAllowMinus;
    [SerializeField]
    [Header("マイナススプライト名")]
    private string resMinusSpriteName;
    [SerializeField]
    [Header("0以下を記録無し表示")]
    private bool isNumberLessThanZero;
    [SerializeField]
    [Header("記録無しのスプライト名")]
    private string resNoRecordSpriteName;
    [SerializeField]
    [Header("小数点以下の数値")]
    private bool isNumberAfterDecimal;
    private int max;
    private float defaultPosX;
    private bool isInit;
    private float leftAlign;
    private float rightAlign;
    private int currentValue;
    private List<char> strNumArray = new List<char>();
    private int length;
    private float offset;
    public int CurrentValue => currentValue;
    public float NumSpace => numberSpace;
    public string SpriteName => resSpriteName;
    public bool IsZeroFill {
        get {
            return isZeroFill;
        }
        set {
            isZeroFill = value;
        }
    }
    public int LimitLength {
        get;
        set;
    }
    private void Awake() {
        Init();
    }
    private void Init() {
        if (!isInit) {
            isInit = true;
            defaultPosX = root.transform.localPosition.x;
            CalcManager.mCalcInt = 9;
            for (int i = 0; i < arraySpriteRendererNumber.Length; i++) {
                max += CalcManager.mCalcInt;
                CalcManager.mCalcInt *= 10;
            }
            if (objLeftAlign != null) {
                leftAlign = objLeftAlign.transform.localPosition.x;
            }
            if (objRightAlign != null) {
                rightAlign = objRightAlign.transform.localPosition.x;
            }
            LimitLength = -1;
        }
    }
    public void Set(int _num) {
        Init();
        int num = _num.ToString().Length;
        _num = Mathf.Abs(_num);
        currentValue = _num;
        if (_num > max) {
            _num = max;
        }
        for (int i = 0; i < arraySpriteRendererNumber.Length; i++) {
            if (arraySpriteRendererNumber[i] == null) {
                return;
            }
            arraySpriteRendererNumber[i].gameObject.SetActive(value: false);
        }
        for (int j = 0; j < arraySpriteRendererNumber.Length; j++) {
            if (isNumberAfterDecimal) {
                arraySpriteRendererNumber[j].gameObject.SetActive(value: true);
            } else {
                arraySpriteRendererNumber[j].gameObject.SetActive(j < num);
            }
            if (arraySpriteRendererNumber[j].gameObject.activeSelf) {
                if (_num < 0 && isNumberLessThanZero) {
                    arraySpriteRendererNumber[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resNoRecordSpriteName);
                } else if (isNumberAllowMinus) {
                    if (_num < 0) {
                        if (j < num - 1) {
                            arraySpriteRendererNumber[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resSpriteName + (_num % 10).ToString());
                            _num /= 10;
                        } else {
                            arraySpriteRendererNumber[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resMinusSpriteName);
                        }
                    } else {
                        arraySpriteRendererNumber[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resSpriteName + (_num % 10).ToString());
                        _num /= 10;
                    }
                } else {
                    arraySpriteRendererNumber[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resSpriteName + (_num % 10).ToString());
                    _num /= 10;
                }
            } else if ((_num > 0 || !isNumberLessThanZero) && isZeroFill) {
                arraySpriteRendererNumber[j].gameObject.SetActive(value: true);
                arraySpriteRendererNumber[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resSpriteName + 0.ToString());
            }
            if (LimitLength != -1 && j >= LimitLength) {
                arraySpriteRendererNumber[j].gameObject.SetActive(value: false);
            }
        }
        switch (center) {
            case VERTICAL_CENTER.RIGHT:
                root.transform.SetLocalPositionX(defaultPosX);
                break;
            case VERTICAL_CENTER.CENTER:
                if (isAdjustScale) {
                    float x2 = defaultPosX - numberSpace * 0.5f * (float)(arraySpriteRendererNumber.Length - num) * root.transform.localScale.x;
                    root.transform.SetLocalPositionX(x2);
                } else {
                    root.transform.SetLocalPositionX(defaultPosX + numberSpace * 0.5f * (float)(num - 1));
                }
                break;
            case VERTICAL_CENTER.LEFT:
                if (isAdjustScale) {
                    float x = defaultPosX - numberSpace * (float)(arraySpriteRendererNumber.Length - num) * root.transform.localScale.x;
                    root.transform.SetLocalPositionX(x);
                } else {
                    root.transform.SetLocalPositionX(defaultPosX + numberSpace * (float)(num - 1));
                }
                break;
        }
        if (objLeftAlign != null) {
            float num2 = numberSpace * (float)(num - 1);
            if (center == VERTICAL_CENTER.CENTER) {
                num2 *= 0.5f;
            }
            objLeftAlign.transform.SetLocalPositionX(leftAlign - num2);
        }
        if (objRightAlign != null) {
            objRightAlign.transform.SetLocalPositionX(rightAlign);
        }
    }
    public void SetNumbers(string _numStr) {
        Init();
        strNumArray.Clear();
        for (int i = 0; i < _numStr.Length && i <= max; i++) {
            strNumArray.Add(_numStr[i]);
        }
        if (isZeroFill && strNumArray.Count == 1 && arraySpriteRendererNumber.Length > 1) {
            strNumArray.Add('0');
        }
        length = strNumArray.Count;
        if (isNumberLessThanZero && strNumArray[0] == '-') {
            for (int j = 0; j < arraySpriteRendererNumber.Length; j++) {
                arraySpriteRendererNumber[j].gameObject.SetActive(j < 2);
                if (arraySpriteRendererNumber[j].gameObject.activeSelf) {
                    arraySpriteRendererNumber[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resNoRecordSpriteName);
                }
            }
        } else {
            for (int k = 0; k < arraySpriteRendererNumber.Length; k++) {
                arraySpriteRendererNumber[k].gameObject.SetActive(k < strNumArray.Count);
                if (arraySpriteRendererNumber[k].gameObject.activeSelf) {
                    if (isNumberAllowMinus) {
                        if (strNumArray[k] < '\0') {
                            if (k < length - 1) {
                                arraySpriteRendererNumber[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resSpriteName + strNumArray[length - 1 - k].ToString());
                            } else {
                                arraySpriteRendererNumber[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resMinusSpriteName);
                            }
                        } else {
                            arraySpriteRendererNumber[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resSpriteName + strNumArray[length - 1 - k].ToString());
                        }
                    } else if (k != 0) {
                        if (k == arraySpriteRendererNumber.Length - 1 && strNumArray[length - 1 - k].ToString() == "0" && !isZeroFill) {
                            arraySpriteRendererNumber[k].gameObject.SetActive(value: false);
                            length--;
                        } else {
                            arraySpriteRendererNumber[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resSpriteName + strNumArray[length - 1 - k].ToString());
                        }
                    } else {
                        arraySpriteRendererNumber[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resSpriteName + strNumArray[length - 1 - k].ToString());
                    }
                } else if (isZeroFill) {
                    arraySpriteRendererNumber[k].gameObject.SetActive(value: true);
                    arraySpriteRendererNumber[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(atlasType, resSpriteName + 0.ToString());
                }
            }
        }
        switch (center) {
            case VERTICAL_CENTER.RIGHT:
                root.transform.SetLocalPositionX(defaultPosX);
                break;
            case VERTICAL_CENTER.CENTER:
                if (isAdjustScale) {
                    float x2 = defaultPosX - numberSpace * 0.5f * (float)(arraySpriteRendererNumber.Length - length) * root.transform.localScale.x;
                    root.transform.SetLocalPositionX(x2);
                } else {
                    root.transform.SetLocalPositionX(defaultPosX + numberSpace * 0.5f * (float)(length - 1));
                }
                break;
            case VERTICAL_CENTER.LEFT:
                if (isAdjustScale) {
                    float x = defaultPosX - numberSpace * (float)(arraySpriteRendererNumber.Length - length) * root.transform.localScale.x;
                    root.transform.SetLocalPositionX(x);
                } else {
                    root.transform.SetLocalPositionX(defaultPosX + numberSpace * (float)(length - 1));
                }
                break;
        }
        if (objLeftAlign != null) {
            offset = numberSpace * (float)(length - 1);
            if (center == VERTICAL_CENTER.CENTER) {
                offset *= 0.5f;
            }
            objLeftAlign.transform.SetLocalPositionX(leftAlign - offset);
        }
        if (objRightAlign != null) {
            objRightAlign.transform.SetLocalPositionX(rightAlign);
        }
    }
    public void SetAlpha(float _alpha) {
        for (int i = 0; i < arraySpriteRendererNumber.Length; i++) {
            Color color = arraySpriteRendererNumber[i].color;
            color.a = _alpha;
            arraySpriteRendererNumber[i].color = color;
        }
    }
    public void SetColor(Color _color) {
        for (int i = 0; i < arraySpriteRendererNumber.Length; i++) {
            arraySpriteRendererNumber[i].color = _color;
        }
    }
    public void SetScale(float _scale) {
        for (int i = 0; i < arraySpriteRendererNumber.Length; i++) {
            arraySpriteRendererNumber[i].transform.localScale = Vector3.one * _scale;
        }
    }
    public void SetZeroFillMode() {
        isZeroFill = true;
    }
    public void SetNoRecordMode() {
        isNumberLessThanZero = true;
    }
    public void SetMinusAllowMode() {
        isNumberAllowMinus = true;
    }
    public SpriteRenderer[] GetArraySpriteNumbers() {
        return arraySpriteRendererNumber;
    }
    public void SetNumbersSpriteName(string _spriteName) {
        resSpriteName = _spriteName;
    }
    public void SetMinusSpriteName(string _spriteName) {
        resMinusSpriteName = _spriteName;
    }
    public void SetNoRecordSpriteName(string _spriteName) {
        resNoRecordSpriteName = _spriteName;
    }
    public void SetSpriteRendererMaskInteraciton(SpriteMaskInteraction _maskInteraction) {
        for (int i = 0; i < arraySpriteRendererNumber.Length; i++) {
            arraySpriteRendererNumber[i].maskInteraction = _maskInteraction;
        }
    }
}
