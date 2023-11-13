using GamepadInput;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public abstract class StateBaseClass : MonoBehaviour {
    public enum PAUSE_LEVEL {
        NONE = 0,
        ALL = 1,
        GAME = 2,
        EFFECT_CUT_IN = 4,
        EFFECT = 8,
        CHARACTER = 0x10,
        OBJECT = 0x20,
        MAX = 0x40
    }
    public delegate void StateMethod();
    protected float mTotalTime;
    protected float mStateTime;
    protected bool mStateInitFlg;
    protected float mIntervalTime;
    private static PAUSE_LEVEL mPauseFlg;
    protected bool mStateDebugFlg;
    protected PAUSE_LEVEL mPauseLevel;
    protected bool isAnimationWait;
    protected bool mIsUnscaledDeltaTime;
    protected long mCommonEnterPadButton;
    protected long mCommonBackPadButton = 1L;
    private StateMethod mStateMethod;
    private StateMethod mStateInitMethod;
    private StateMethod mStateMethodBackUp;
    private bool isProcessOnEnable;
    protected float DELTA_TIME {
        get {
            if (mIsUnscaledDeltaTime) {
                return Time.unscaledDeltaTime;
            }
            if (mPauseFlg == PAUSE_LEVEL.NONE) {
                return Time.deltaTime;
            }
            if ((mPauseFlg & PAUSE_LEVEL.ALL) != 0) {
                return 0f;
            }
            if ((mPauseFlg & mPauseLevel) != 0) {
                return 0f;
            }
            return Time.deltaTime;
        }
    }
    public static void SetPauseFlg(PAUSE_LEVEL _level) {
        if (_level == PAUSE_LEVEL.NONE) {
            mPauseFlg = PAUSE_LEVEL.NONE;
            Time.timeScale = 1f;
        }
        mPauseFlg |= _level;
        if ((mPauseFlg & PAUSE_LEVEL.ALL) != 0 || (mPauseFlg & PAUSE_LEVEL.GAME) != 0) {
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
    }
    public static bool CheckPauseFlg(PAUSE_LEVEL _level) {
        return (mPauseFlg & _level) != PAUSE_LEVEL.NONE;
    }
    protected void Awake() {
        mTotalTime = 0f;
        mStateTime = 0f;
        mStateInitFlg = false;
        mIntervalTime = 0f;
        mPauseLevel = PAUSE_LEVEL.NONE;
        mStateMethodBackUp = null;
        Init();
    }
    protected void Start() {
        Resume();
    }
    protected void OnEnable() {
        if (isProcessOnEnable) {
            Resume();
            isProcessOnEnable = true;
        }
    }
    protected void Update() {
        mTotalTime += DELTA_TIME;
        if (mStateDebugFlg) {
            UnityEngine.Debug.Log("mTotalTime = " + mTotalTime.ToString());
        }
        if (mIntervalTime > 0f) {
            mIntervalTime -= DELTA_TIME;
        } else {
            mStateTime += DELTA_TIME;
            if (!mStateInitFlg) {
                if (mStateInitMethod == null) {
                    if (mStateDebugFlg) {
                        UnityEngine.Debug.Log(base.gameObject.name + " > mStateInitMethod = null");
                    }
                } else {
                    mStateInitMethod();
                    if (mStateDebugFlg) {
                        UnityEngine.Debug.Log(base.gameObject.name + " > mStateInitMethod = " + mStateMethod.ToString());
                    }
                }
                mStateInitFlg = true;
            } else if (mStateMethod == null) {
                if (mStateDebugFlg) {
                    UnityEngine.Debug.Log(base.gameObject.name + " > mStateMethod = null");
                }
            } else {
                mStateMethod();
                if (mStateDebugFlg) {
                    UnityEngine.Debug.Log(base.gameObject.name + " > mStateInitMethod = " + mStateMethod.ToString());
                }
            }
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Scene_Blank" && !SingletonCustom<SceneManager>.Instance.GetFadeFlg() && IsActive() && !isAnimationWait) {
            if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown((SatGamePad.Button)mCommonEnterPadButton)) {
                InputCommonMenuEnter();
            }
            if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown((SatGamePad.Button)mCommonBackPadButton)) {
                InputCommonMenuBack();
            }
        }
    }
    protected virtual void LayerEnd(SceneManager.LayerCloseType _closeType) {
    }
    protected void OpenLayer() {
        SingletonCustom<SceneManager>.Instance.AddNowLayerCloseCallBack(LayerEnd);
    }
    protected bool IsActive() {
        return SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack() == new SceneManager.LayerClose(LayerEnd);
    }
    protected virtual void ReturnScene() {
    }
    protected virtual void InputCommonMenuEnter() {
    }
    protected virtual void InputCommonMenuBack() {
    }
    public virtual void Init() {
    }
    public virtual void Resume() {
    }
    protected void StatePause() {
    }
    protected void StateResume() {
    }
    protected void SetState(StateMethod _Initmethod, StateMethod _method, float _interval = 0f) {
        mStateMethodBackUp = mStateMethod;
        mStateTime = 0f;
        mStateInitFlg = false;
        mIntervalTime = _interval;
        mStateInitMethod = _Initmethod;
        if (mIntervalTime == 0f) {
            mStateInitFlg = true;
            if (mStateInitMethod != null) {
                mStateInitMethod();
            }
        }
        mStateMethod = _method;
    }
    protected void SetState(StateMethod _method, float _interval = 0f, bool _isInit = false) {
        SetState(_isInit ? mStateInitMethod : null, _method, _interval);
    }
    protected void ReduceState() {
        mStateInitMethod = null;
        mStateMethod = null;
    }
    protected StateMethod GetState() {
        if (!mStateInitFlg) {
            return mStateMethodBackUp;
        }
        return mStateMethod;
    }
    protected bool IsState(StateMethod _method) {
        return (mStateInitFlg ? mStateMethod : mStateMethodBackUp) == _method;
    }
    protected void ResetStateInitFlg() {
        mStateInitFlg = false;
    }
    protected void CalcTranslate(GameObject _obj, Vector3 _dir) {
        _obj.transform.Translate(_dir.x * DELTA_TIME, _dir.y * DELTA_TIME, _dir.z * DELTA_TIME);
    }
    protected void CalcRotate(GameObject _obj, Vector3 _rot) {
        _obj.transform.Rotate(_rot.x * DELTA_TIME, _rot.y * DELTA_TIME, _rot.z * DELTA_TIME);
    }
    protected void CalcScale(GameObject _obj, Vector3 _scale) {
        _obj.transform.localScale = _obj.transform.localScale + _scale * DELTA_TIME;
    }
    protected void ResetState() {
        mStateMethod = (StateMethod)Delegate.Remove(mStateMethod, mStateMethod);
        mStateInitMethod = (StateMethod)Delegate.Remove(mStateInitMethod, mStateInitMethod);
        mStateMethod = null;
        mStateInitMethod = null;
    }
    protected void ResetStateInit() {
        mStateInitMethod = (StateMethod)Delegate.Remove(mStateInitMethod, mStateInitMethod);
        mStateInitMethod = null;
    }
}
