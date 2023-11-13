using GamepadInput;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DM : SingletonCustom<DM> {
    private enum SHOW_LOG_TYPE {
        VERVOSE,
        WARNING,
        ERROR,
        NONE
    }
    public static class PARAM_LIST {
        public static readonly string DIALOG_TEXT = "dialog_text";
        public static readonly string CALL_BACK = "call_back";
        public static readonly string BUTTON_TEXT = "button_text";
        public static readonly string CHAR_SIZE = "char_size";
        public static readonly string POS = "pos";
        public static readonly string POS_X = "pos_x";
        public static readonly string POS_Y = "pos_y";
        public static readonly string POS_Z = "pos_z";
        public static readonly string POS_L = "pos_l";
        public static readonly string POS_X_L = "pos_x_l";
        public static readonly string POS_Y_L = "pos_y_l";
        public static readonly string POS_Z_L = "pos_z_l";
        public static readonly string POS_WH = "pos_wh";
        public static readonly string POS_W = "pos_w";
        public static readonly string POS_H = "pos_h";
        public static readonly string DIALOG_SIZE = "dialog_size";
        public static readonly string DIALOG_SIZE_H = "dialog_size_h";
        public static readonly string DIALOG_SIZE_W = "dialog_size_w";
        public static readonly string SHOW_BACK_FADE = "show_back_fade";
        public static readonly string FROME_POS = "frome_pos";
        public static readonly string FROME_POS_X = "frome_pos";
        public static readonly string FROME_POS_Y = "frome_pos";
        public static readonly string FROME_POS_Z = "frome_pos";
        public static readonly string CREATE_BUTTON = "create_button";
        public static readonly string NON_BUTTON = "non_button";
        public static readonly string BUTTON_CHAR_SIZE = "button_char_size";
        public static readonly string CHOICE = "choice";
        public static readonly string ESHOP_LINK = "eshop_link";
    }
    private enum DIALOG_STATE {
        NON,
        EXPAND,
        OPEN,
        DEDUCTION,
        CLOSE
    }
    public static readonly string BUTTON_BASE_NAME = "DialogButton";
    [SerializeField]
    [Header("表示するログ")]
    private SHOW_LOG_TYPE SHOW_LOG;
    [SerializeField]
    [Header("ダイアログ")]
    private GameObject mDialogRoot;
    [SerializeField]
    [Header("ダイアログ文字")]
    private TextMeshPro mDialogText;
    [SerializeField]
    [Header("ダイアログ表示領域")]
    private RectTransform mDialogRect;
    [SerializeField]
    [Header("背景")]
    private SpriteRenderer backGround;
    private Color colorTemp;
    [SerializeField]
    [Header("背景フェ\u30fcドAlpha値")]
    private float FADE_ALPHA = 0.5f;
    private int mClickButtonNo;
    private float mButtonCharSize;
    private float SCALING_TIME = 0.5f;
    private float mScalingTime;
    [SerializeField]
    [Header("テキストサイズによってダイアログサイズを変更する境界")]
    private Vector3 DIALOG_RESIZE_BODER;
    [SerializeField]
    [Header("通常とじるボタンレイアウト")]
    private GameObject normalButtonLayout;
    [SerializeField]
    [Header("選択（YES/NO）ボタンレイアウト")]
    private GameObject choiceButtonLayout;
    [SerializeField]
    [Header("eShopボタンレイアウト")]
    private GameObject eShopButtonLayout;
    private DIALOG_STATE mDialogState;
    private bool mInitListFlg;
    private List<Action> mCallBack;
    private static List<Action> mActionListTemp;
    private static List<string> mStringListTemp;
    private List<string> mBtnText;
    private float mDefDialogTextSize;
    private Vector3 mDialogTextShowSize;
    private float BUTTON_UNDER_SPACE = 50f;
    private Vector3 mVector3Temp;
    private Vector2 mVector2Temp;
    private string[] mStrListTemp;
    private Hashtable mDialogParameter;
    private bool mIsButtonCreate;
    private bool IsShowLog(SHOW_LOG_TYPE _data = SHOW_LOG_TYPE.VERVOSE) {
        if (SHOW_LOG != _data) {
            return SHOW_LOG == SHOW_LOG_TYPE.VERVOSE;
        }
        return true;
    }
    private void Awake() {
        mDialogState = DIALOG_STATE.CLOSE;
    }
    private void Update() {
        switch (mDialogState) {
            case DIALOG_STATE.CLOSE:
                break;
            case DIALOG_STATE.EXPAND:
                mScalingTime += Time.unscaledDeltaTime;
                if (mScalingTime >= SCALING_TIME) {
                    mDialogRoot.transform.SetLocalScaleX(1f);
                    mDialogRoot.transform.SetLocalScaleY(1f);
                    mScalingTime = 0f;
                    mDialogState = DIALOG_STATE.OPEN;
                } else {
                    mDialogRoot.transform.SetLocalScaleX(easeOutBack(0f, 1f, mScalingTime / SCALING_TIME));
                    mDialogRoot.transform.SetLocalScaleY(easeOutBack(0f, 1f, mScalingTime / SCALING_TIME));
                }
                break;
            case DIALOG_STATE.OPEN:
                if (normalButtonLayout.activeSelf) {
                    if (IsButton(SatGamePad.Button.B)) {
                        ClickButton(0, SatGamePad.Button.B);
                    }
                } else if (eShopButtonLayout.activeSelf) {
                    if (IsButton(SatGamePad.Button.B)) {
                        ClickButton(0, SatGamePad.Button.B);
                    } else if (IsButton(SatGamePad.Button.Y)) {
                        StartCoroutine(_eShopWait());
                    }
                } else if (choiceButtonLayout.activeSelf) {
                    if (IsButton(SatGamePad.Button.A)) {
                        ClickButton(0, SatGamePad.Button.A);
                    } else if (IsButton(SatGamePad.Button.B)) {
                        ClickButton(1, SatGamePad.Button.B);
                    }
                }
                break;
            case DIALOG_STATE.DEDUCTION:
                mScalingTime += Time.unscaledDeltaTime;
                if (mScalingTime >= SCALING_TIME) {
                    mDialogRoot.transform.SetLocalScaleX(0f);
                    mDialogRoot.transform.SetLocalScaleY(0f);
                    mDialogState = DIALOG_STATE.CLOSE;
                    DialogClose(SceneManager.LayerCloseType.NORMAL);
                } else {
                    mDialogRoot.transform.SetLocalScaleX(easeInBack(1f, 0f, mScalingTime / SCALING_TIME));
                    mDialogRoot.transform.SetLocalScaleY(easeInBack(1f, 0f, mScalingTime / SCALING_TIME));
                }
                break;
        }
    }
    private IEnumerator _eShopWait() {
        SingletonCustom<AudioManager>.Instance.BgmVolumeChange(0f);
        yield return new WaitForSeconds(0.1f);
        eShopManager.ShowShopAddOnContentDetailPage();
        SingletonCustom<AudioManager>.Instance.BgmVolumeChange();
    }
    public void OpenDialog(params object[] _paramList) {
        if (IsActive()) {
            if (IsShowLog(SHOW_LOG_TYPE.WARNING)) {
                UnityEngine.Debug.LogWarning("ダイアログがActive状態");
            }
            return;
        }
        if (IsShowLog()) {
            UnityEngine.Debug.Log("ダイアログを表示する");
        }
        if (mDialogParameter != null) {
            mDialogParameter.Clear();
        }
        mDialogParameter = Hash(_paramList);
        mDialogParameter = CleanArgs(mDialogParameter);
        LeanTween.cancel(backGround.gameObject);
        colorTemp = backGround.color;
        backGround.gameObject.SetActive(value: true);
        LeanTween.value(backGround.gameObject, 0f, FADE_ALPHA, SCALING_TIME * 0.5f).setEaseLinear().setOnUpdate(delegate (float _value) {
            colorTemp.a = _value;
            backGround.color = colorTemp;
        })
            .setIgnoreTimeScale(useUnScaledTime: true);
        if (!mInitListFlg) {
            mCallBack = new List<Action>();
            mBtnText = new List<string>();
            mDefDialogTextSize = mDialogText.preferredWidth;
            mInitListFlg = true;
        }
        if (IsShowLog()) {
            foreach (DictionaryEntry item in mDialogParameter) {
                UnityEngine.Debug.Log("mDialogParameter : " + mDialogParameter.Count.ToString() + " : Key = " + item.Key?.ToString() + " : Data = " + item.Value?.ToString());
            }
        }
        mDialogText.text = "";
        if (mDialogParameter.Contains(PARAM_LIST.DIALOG_TEXT)) {
            if (mDialogParameter[PARAM_LIST.DIALOG_TEXT].GetType() == typeof(string[])) {
                for (int i = 0; i < ((string[])mDialogParameter[PARAM_LIST.DIALOG_TEXT]).Length; i++) {
                    if (i != 0) {
                        mDialogText.text += Environment.NewLine;
                    }
                    mDialogText.text += ((string[])mDialogParameter[PARAM_LIST.DIALOG_TEXT])[i];
                }
            } else if (mDialogParameter[PARAM_LIST.DIALOG_TEXT].GetType() == typeof(string)) {
                mDialogText.text = (string)mDialogParameter[PARAM_LIST.DIALOG_TEXT];
            } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.DIALOG_TEXT]?.ToString());
            }
        }
        if (mDialogParameter.Contains(PARAM_LIST.CHAR_SIZE)) {
            bool flag = mDialogParameter[PARAM_LIST.CHAR_SIZE].GetType() == typeof(float);
        }
        mCallBack.Clear();
        if (mDialogParameter.Contains(PARAM_LIST.CALL_BACK)) {
            if (mDialogParameter[PARAM_LIST.CALL_BACK].GetType() == typeof(Action[])) {
                mCallBack.AddRange((Action[])mDialogParameter[PARAM_LIST.CALL_BACK]);
            } else if (mDialogParameter[PARAM_LIST.CALL_BACK].GetType() == typeof(Action)) {
                mCallBack.Add((Action)mDialogParameter[PARAM_LIST.CALL_BACK]);
            } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.CALL_BACK]?.ToString());
            }
        }
        mBtnText.Clear();
        if (mDialogParameter.Contains(PARAM_LIST.BUTTON_TEXT)) {
            if (mDialogParameter[PARAM_LIST.BUTTON_TEXT].GetType() == typeof(string[])) {
                mBtnText.AddRange((string[])mDialogParameter[PARAM_LIST.BUTTON_TEXT]);
            } else if (mDialogParameter[PARAM_LIST.BUTTON_TEXT].GetType() == typeof(string)) {
                mBtnText.Add((string)mDialogParameter[PARAM_LIST.BUTTON_TEXT]);
            } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.BUTTON_TEXT]?.ToString());
            }
        }
        if (mDialogParameter.Contains(PARAM_LIST.CHOICE)) {
            normalButtonLayout.SetActive(value: false);
            eShopButtonLayout.SetActive(value: false);
            choiceButtonLayout.SetActive(value: true);
        } else if (mDialogParameter.Contains(PARAM_LIST.ESHOP_LINK)) {
            normalButtonLayout.SetActive(value: false);
            eShopButtonLayout.SetActive(value: true);
            choiceButtonLayout.SetActive(value: false);
        } else {
            normalButtonLayout.SetActive(value: true);
            eShopButtonLayout.SetActive(value: false);
            choiceButtonLayout.SetActive(value: false);
        }
        mIsButtonCreate = ((!mDialogParameter.Contains(PARAM_LIST.NON_BUTTON) && mCallBack.Count > 1) || mBtnText.Count > 0);
        float num = 0f;
        string[] array = mDialogText.text.Split('\n');
        for (int j = 0; j < array.Length; j++) {
            if (array[j].Length > 12 && (float)(array[j].Length - 12) * 47f > num) {
                num = (float)(array[j].Length - 12) * 47f;
            }
        }
        mDialogRoot.transform.SetLocalPosition(0f, 0f, 0f);
        if (mDialogParameter.Contains(PARAM_LIST.POS)) {
            if (mDialogParameter[PARAM_LIST.POS].GetType() == typeof(Vector3)) {
                mVector3Temp = (Vector3)mDialogParameter[PARAM_LIST.POS];
                mDialogRoot.transform.SetLocalPosition(mVector3Temp.x - SingletonCustom<GlobalCameraManager>.Instance.GetWidth() * 0.5f, mVector3Temp.y - SingletonCustom<GlobalCameraManager>.Instance.GetHeight() * 0.5f, mVector3Temp.z);
            } else if (mDialogParameter[PARAM_LIST.POS].GetType() == typeof(Vector2)) {
                mVector2Temp = (Vector2)mDialogParameter[PARAM_LIST.POS];
                mDialogRoot.transform.SetLocalPosition(mVector2Temp.x - SingletonCustom<GlobalCameraManager>.Instance.GetWidth() * 0.5f, mVector2Temp.y - SingletonCustom<GlobalCameraManager>.Instance.GetHeight() * 0.5f, 0f);
            } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.POS]?.ToString());
            }
        } else {
            if (mDialogParameter.Contains(PARAM_LIST.POS_X)) {
                if (mDialogParameter[PARAM_LIST.POS_X].GetType() == typeof(float)) {
                    mDialogRoot.transform.SetLocalPositionX((float)mDialogParameter[PARAM_LIST.POS_X] - SingletonCustom<GlobalCameraManager>.Instance.GetWidth() * 0.5f);
                } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                    UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.POS_X]?.ToString());
                }
            }
            if (mDialogParameter.Contains(PARAM_LIST.POS_Y)) {
                if (mDialogParameter[PARAM_LIST.POS_Y].GetType() == typeof(float)) {
                    mDialogRoot.transform.SetLocalPositionY((float)mDialogParameter[PARAM_LIST.POS_Y] - SingletonCustom<GlobalCameraManager>.Instance.GetHeight() * 0.5f);
                } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                    UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.POS_Y]?.ToString());
                }
            }
            if (mDialogParameter.Contains(PARAM_LIST.POS_Z)) {
                if (mDialogParameter[PARAM_LIST.POS_Z].GetType() == typeof(float)) {
                    mDialogRoot.transform.SetLocalPositionZ((float)mDialogParameter[PARAM_LIST.POS_Z]);
                } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                    UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.POS_Z]?.ToString());
                }
            }
        }
        if (mDialogParameter.Contains(PARAM_LIST.POS_L)) {
            if (mDialogParameter[PARAM_LIST.POS_L].GetType() == typeof(Vector3)) {
                mVector3Temp = (Vector3)mDialogParameter[PARAM_LIST.POS_L];
                mDialogRoot.transform.SetLocalPosition(mVector3Temp.x, mVector3Temp.y, mVector3Temp.z);
            } else if (mDialogParameter[PARAM_LIST.POS_L].GetType() == typeof(Vector2)) {
                mVector2Temp = (Vector2)mDialogParameter[PARAM_LIST.POS_L];
                mDialogRoot.transform.SetLocalPosition(mVector2Temp.x, mVector2Temp.y, 0f);
            } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.POS_L]?.ToString());
            }
        } else {
            if (mDialogParameter.Contains(PARAM_LIST.POS_X_L)) {
                if (mDialogParameter[PARAM_LIST.POS_X_L].GetType() == typeof(float)) {
                    mDialogRoot.transform.SetLocalPositionX((float)mDialogParameter[PARAM_LIST.POS_X_L]);
                } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                    UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.POS_X_L]?.ToString());
                }
            }
            if (mDialogParameter.Contains(PARAM_LIST.POS_Y_L)) {
                if (mDialogParameter[PARAM_LIST.POS_Y_L].GetType() == typeof(float)) {
                    mDialogRoot.transform.SetLocalPositionY((float)mDialogParameter[PARAM_LIST.POS_Y_L]);
                } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                    UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.POS_Y_L]?.ToString());
                }
            }
            if (mDialogParameter.Contains(PARAM_LIST.POS_Z_L)) {
                if (mDialogParameter[PARAM_LIST.POS_Z_L].GetType() == typeof(float)) {
                    mDialogRoot.transform.SetLocalPositionZ((float)mDialogParameter[PARAM_LIST.POS_Z_L]);
                } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                    UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.POS_Z_L]?.ToString());
                }
            }
        }
        if (mDialogParameter.Contains(PARAM_LIST.POS_WH)) {
            if (mDialogParameter[PARAM_LIST.POS_WH].GetType() == typeof(Vector3)) {
                mVector3Temp = (Vector3)mDialogParameter[PARAM_LIST.POS_WH];
                mDialogRoot.transform.SetLocalPosition(SingletonCustom<GlobalCameraManager>.Instance.GetWidth() * mVector3Temp.x - SingletonCustom<GlobalCameraManager>.Instance.GetWidth() * 0.5f, SingletonCustom<GlobalCameraManager>.Instance.GetHeight() * mVector3Temp.y - SingletonCustom<GlobalCameraManager>.Instance.GetHeight() * 0.5f, mVector3Temp.z);
            } else if (mDialogParameter[PARAM_LIST.POS_WH].GetType() == typeof(Vector2)) {
                mVector2Temp = (Vector2)mDialogParameter[PARAM_LIST.POS_WH];
                mDialogRoot.transform.SetLocalPosition(SingletonCustom<GlobalCameraManager>.Instance.GetWidth() * mVector2Temp.x - SingletonCustom<GlobalCameraManager>.Instance.GetWidth() * 0.5f, SingletonCustom<GlobalCameraManager>.Instance.GetHeight() * mVector2Temp.y - SingletonCustom<GlobalCameraManager>.Instance.GetHeight() * 0.5f, 0f);
            } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.POS_WH]?.ToString());
            }
        } else {
            if (mDialogParameter.Contains(PARAM_LIST.POS_W)) {
                if (mDialogParameter[PARAM_LIST.POS_W].GetType() == typeof(float)) {
                    mDialogRoot.transform.SetLocalPositionX(SingletonCustom<GlobalCameraManager>.Instance.GetWidth() * (float)mDialogParameter[PARAM_LIST.POS_W] - SingletonCustom<GlobalCameraManager>.Instance.GetWidth() * 0.5f);
                } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                    UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.POS_W]?.ToString());
                }
            }
            if (mDialogParameter.Contains(PARAM_LIST.POS_H)) {
                if (mDialogParameter[PARAM_LIST.POS_H].GetType() == typeof(float)) {
                    mDialogRoot.transform.SetLocalPositionY(SingletonCustom<GlobalCameraManager>.Instance.GetHeight() * (float)mDialogParameter[PARAM_LIST.POS_H] - SingletonCustom<GlobalCameraManager>.Instance.GetHeight() * 0.5f);
                } else if (IsShowLog(SHOW_LOG_TYPE.ERROR)) {
                    UnityEngine.Debug.LogError("型が違います : " + mDialogParameter[PARAM_LIST.POS_H]?.ToString());
                }
            }
        }
        CalcManager.mCalcVector3.x = 0f - mDialogRoot.transform.localPosition.x;
        CalcManager.mCalcVector3.y = 0f - mDialogRoot.transform.localPosition.y;
        CalcManager.mCalcVector3.z = 0f;
        mDialogRoot.gameObject.SetActive(value: true);
        mDialogRoot.transform.SetLocalScaleX(1f);
        mDialogRoot.transform.SetLocalScaleY(1f);
        mDialogTextShowSize = mDialogText.GetComponent<Renderer>().bounds.size;
        mDialogRoot.gameObject.SetActive(value: false);
        mDialogRect.sizeDelta = new Vector2(mDialogText.preferredWidth, mDialogRect.sizeDelta.y);
        mDialogRoot.transform.SetLocalScaleX(0f);
        mDialogRoot.transform.SetLocalScaleY(0f);
        mDialogRoot.gameObject.SetActive(value: true);
        mScalingTime = 0f;
        mDialogState = DIALOG_STATE.EXPAND;
        SingletonCustom<SceneManager>.Instance.AddNowLayerCloseCallBack(OtherClose);
    }
    public bool ClickButton(int _no, SatGamePad.Button _pad) {
        if (IsShowLog()) {
            UnityEngine.Debug.Log("ボタンを押したとき");
        }
        if (!IsOpen()) {
            return false;
        }
        if (IsShowLog()) {
            UnityEngine.Debug.Log("ボタンを押したときの処理");
        }
        mClickButtonNo = _no;
        mDialogState = DIALOG_STATE.DEDUCTION;
        LeanTween.cancel(backGround.gameObject);
        colorTemp = backGround.color;
        LeanTween.value(backGround.gameObject, FADE_ALPHA, 0f, SCALING_TIME).setEaseLinear().setOnUpdate(delegate (float _value) {
            colorTemp.a = _value;
            backGround.color = colorTemp;
        })
            .setIgnoreTimeScale(useUnScaledTime: true);
        SingletonCustom<AudioManager>.Instance.SePlay((_pad == SatGamePad.Button.A) ? "se_button_common" : "se_button_cancel");
        return true;
    }
    public void ClickDialogButton() {
        LeanTween.cancel(backGround.gameObject);
        ClickButton(0, SatGamePad.Button.B);
    }
    private bool IsButton(SatGamePad.Button _button) {
        return SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(_button, _isRepeat: false, KeyCode.None, useOnlyArrow: false, _isTimeMoving: true);
    }
    public void Close() {
        backGround.color = new Color(0f, 0f, 0f, 0f);
        backGround.gameObject.SetActive(value: false);
        DialogClose(SceneManager.LayerCloseType.CHANGE_SCENE);
    }
    public void DialogClose(SceneManager.LayerCloseType _closeType) {
        if (IsShowLog()) {
            UnityEngine.Debug.Log("ダイアログを閉じる");
        }
        mDialogRoot.gameObject.SetActive(value: false);
        if (IsShowLog()) {
            UnityEngine.Debug.Log("DialogState : " + mDialogState.ToString());
        }
        if (_closeType != SceneManager.LayerCloseType.CHANGE_SCENE) {
            SingletonCustom<SceneManager>.Instance.CloseComplete();
            if (mCallBack != null && mCallBack[mClickButtonNo] != null) {
                mCallBack[mClickButtonNo]();
            }
        }
    }
    private void OtherClose(SceneManager.LayerCloseType _closeType) {
        if (IsShowLog()) {
            UnityEngine.Debug.Log("OtherClose : " + _closeType.ToString());
        }
        if (_closeType == SceneManager.LayerCloseType.CHANGE_SCENE) {
            DialogClose(_closeType);
        }
    }
    public bool IsActive() {
        if (mDialogRoot.gameObject.activeSelf) {
            return SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack() == new SceneManager.LayerClose(OtherClose);
        }
        return false;
    }
    public bool IsOpen() {
        if (mDialogState == DIALOG_STATE.EXPAND || mDialogState == DIALOG_STATE.OPEN) {
            return IsActive();
        }
        return false;
    }
    public float GetDefDialogTextSize() {
        if (mDefDialogTextSize <= 0f) {
            mDefDialogTextSize = mDialogText.preferredWidth;
        }
        return mDefDialogTextSize;
    }
    public static Hashtable Hash(params object[] args) {
        Hashtable hashtable = new Hashtable(args.Length / 2);
        if (args.Length % 2 != 0) {
            UnityEngine.Debug.LogError("Tween Error: Hash requires an even number of arguments!");
            return null;
        }
        for (int i = 0; i < args.Length - 1; i += 2) {
            hashtable.Add(args[i], args[i + 1]);
        }
        return hashtable;
    }
    private Hashtable CleanArgs(Hashtable args) {
        Hashtable hashtable = new Hashtable(args.Count);
        Hashtable hashtable2 = new Hashtable(args.Count);
        foreach (DictionaryEntry arg in args) {
            hashtable.Add(arg.Key, arg.Value);
        }
        foreach (DictionaryEntry item in hashtable) {
            if (item.Value.GetType() == typeof(int)) {
                float num = (int)item.Value;
                args[item.Key] = num;
            }
            if (item.Value.GetType() == typeof(double)) {
                float num2 = (float)(double)item.Value;
                args[item.Key] = num2;
            }
        }
        foreach (DictionaryEntry arg2 in args) {
            hashtable2.Add(arg2.Key.ToString().ToLower(), arg2.Value);
        }
        args = hashtable2;
        return args;
    }
    public static Action[] List(params Action[] _methodList) {
        return _methodList;
    }
    public static string[] List(params string[] _strList) {
        return _strList;
    }
    private float easeOutBack(float start, float end, float value) {
        float num = 1.70158f;
        end -= start;
        value = value / 1f - 1f;
        return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
    }
    private float easeInBack(float start, float end, float value) {
        end -= start;
        value /= 1f;
        float num = 1.70158f;
        return end * value * value * ((num + 1f) * value - num) + start;
    }
    public static float GetTextMeshWidth(TextMesh mesh) {
        float num = 0f;
        string text = mesh.text;
        foreach (char ch in text) {
            if (mesh.font.GetCharacterInfo(ch, out CharacterInfo info)) {
                num += info.width;
            }
        }
        return num * mesh.characterSize * 0.1f * mesh.transform.lossyScale.x;
    }
}
