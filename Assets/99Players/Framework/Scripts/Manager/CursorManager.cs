using GamepadInput;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class CursorManager : MonoBehaviour {
    public enum SelectMoveType {
        MOVABLE = 0,
        RETURN = -1,
        MOVELESS = -2
    }
    public enum MoveDir {
        NONE,
        TOP,
        DOWN,
        LEFT,
        RIGHT
    }
    [SerializeField]
    [Header("移動アニメ\u30fcション")]
    protected bool isAnimation = true;
    [SerializeField]
    [Header("入力リピ\u30fcト設定")]
    private bool isInputRepeat = true;
    [SerializeField]
    [Header("フォ\u30fcカス動作")]
    protected bool isFocus;
    [SerializeField]
    [Header("フォ\u30fcカス倍率")]
    protected float focusScale = 1f;
    [SerializeField]
    [Header("項目を移動しない入力判定を許可")]
    protected bool isNoMoveInput;
    [SerializeField]
    [Header("停止状態の際にカ\u30fcソルアニメを非表示にする")]
    protected bool isStopToAnimObjDisable;
    [SerializeField]
    [Header("カ\u30fcソルの移動時にz座標をロ\u30fcカル値基準にする")]
    protected bool isMovedCursorFixLocalPosZ;
    [SerializeField]
    [Header("レイヤ\u30fc")]
    protected CursorLayer[] cursorLayer;
    [SerializeField]
    private bool useOnlyArrow;
    public static readonly int EXTRA_UP_IDX = 1000;
    public static readonly int EXTRA_DOWN_IDX = 2000;
    public UnityEvent OnExtraUp = new UnityEvent();
    public UnityEvent OnExtraDown = new UnityEvent();
    protected MoveDir moveDir;
    protected bool isStop;
    private bool isProvEnter;
    protected int[] nowCursorNo;
    protected int[] prevCursorNo;
    protected bool isPushMoveButtonNow;
    protected bool isPushMoveButtonPrev;
    protected int layerNo;
    protected bool isReturn;
    protected CursorAnimation cursorAnimObj;
    private int targetPadId;
    private int no;
    protected int framePrevCursorNo;
    public SceneManager.LayerClose LayerEnd {
        get;
        set;
    }
    public Action CallBackEnter {
        get;
        set;
    }
    public Transform CursorObj => cursorLayer[layerNo].cursorObj[cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].cursorType];
    public int TargetPadId {
        get {
            return targetPadId;
        }
        set {
            targetPadId = value;
        }
    }
    public int LookFrame {
        get;
        set;
    }
    public bool IsInit {
        get;
        set;
    }
    public bool IsStop {
        get {
            return isStop;
        }
        set {
            isStop = value;
            if (isStopToAnimObjDisable && cursorAnimObj != null) {
                cursorAnimObj.SetDisable(!isStop);
            }
        }
    }
    private void Awake() {
        if (!IsInit) {
            layerNo = 0;
            int num = 0;
            nowCursorNo = new int[cursorLayer.Length];
            prevCursorNo = new int[cursorLayer.Length];
            for (int i = 0; i < cursorLayer.Length; i++) {
                nowCursorNo[i] = 0;
                prevCursorNo[i] = 0;
                cursorLayer[i].buttonObj = new CursorButtonObject[cursorLayer[i].button.Length];
                for (int j = 0; j < cursorLayer[i].buttonObj.Length; j++) {
                    if (cursorLayer[i].button[j] != null) {
                        CursorButtonObject[] components = cursorLayer[i].button[j].GetComponents<CursorButtonObject>();
                        int num2 = (components.Length >= num + 1) ? num : 0;
                        cursorLayer[i].buttonObj[j] = components[num2];
                    }
                }
            }
        }
        IsInit = true;
    }
    private void OnEnable() {
        isPushMoveButtonNow = false;
        isPushMoveButtonPrev = false;
        if (cursorLayer[layerNo].buttonObj.Length != 0) {
            CursorButtonObject cursorButtonObject = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]];
            cursorAnimObj = cursorLayer[layerNo].cursorObj[cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].cursorType].GetComponent<CursorAnimation>();
            cursorAnimObj.SetRectSetting(cursorButtonObject.RectSetting);
        }
        if (isFocus) {
            StartCoroutine(_WaitFrame(delegate {
                for (int i = 0; i < cursorLayer[layerNo].buttonObj.Length; i++) {
                    cursorLayer[layerNo].buttonObj[i].transform.SetLocalScale(1f, 1f, 1f);
                }
                cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].transform.SetLocalScale(focusScale, focusScale, 1f);
            }));
        }
    }
    private IEnumerator _WaitFrame(Action _act) {
        yield return new WaitForEndOfFrame();
        _act?.Invoke();
    }
    public void SetCursorPlayerIcon(int _idx) {
        cursorAnimObj.SetPlayerIconPos(_idx);
    }
    public void SetEnable(bool _isEnable) {
        base.gameObject.SetActive(_isEnable);
        for (int i = 0; i < cursorLayer.Length; i++) {
            for (int j = 0; j < cursorLayer[i].cursorObj.Length; j++) {
                cursorLayer[i].cursorObj[j].gameObject.SetActive(value: false);
            }
        }
        if (IsInit) {
            CursorObj.gameObject.SetActive(_isEnable);
        }
    }
    private void Update() {
        if (LookFrame > 0) {
            if (Time.timeScale >= 1f && !SingletonCustom<CommonNotificationManager>.Instance.IsOpen) {
                int num = --LookFrame;
            }
            return;
        }
        if (SingletonCustom<SceneManager>.Instance.GetFadeFlg()) {
            isPushMoveButtonPrev = isPushMoveButtonNow;
            isPushMoveButtonNow = false;
            return;
        }
        if (IsStop) {
            isPushMoveButtonPrev = isPushMoveButtonNow;
            isPushMoveButtonNow = false;
            return;
        }
        if (LayerEnd != null && !IsActive()) {
            isPushMoveButtonPrev = isPushMoveButtonNow;
            isPushMoveButtonNow = false;
            return;
        }
        if (SingletonCustom<DM>.Instance.IsActive()) {
            isPushMoveButtonPrev = isPushMoveButtonNow;
            isPushMoveButtonNow = false;
            return;
        }
        isReturn = false;
        moveDir = MoveDir.NONE;
        no = 0;
        if (IsUpButton()) {
            if (cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].topNo == EXTRA_UP_IDX) {
                OnExtraUp.Invoke();
                return;
            }
            no = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].topNo;
            isPushMoveButtonNow = true;
            moveDir = MoveDir.TOP;
        } else if (IsDownButton()) {
            if (cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].downNo == EXTRA_DOWN_IDX) {
                OnExtraDown.Invoke();
                return;
            }
            no = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].downNo;
            isPushMoveButtonNow = true;
            moveDir = MoveDir.DOWN;
        } else if (IsLeftButton()) {
            no = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].leftNo;
            isPushMoveButtonNow = true;
            moveDir = MoveDir.LEFT;
        } else if (IsRightButton()) {
            no = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].rightNo;
            isPushMoveButtonNow = true;
            moveDir = MoveDir.RIGHT;
        } else if (IsOkButton()) {
            if (CallBackEnter != null) {
                CallBackEnter();
            }
            cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].CallEvent();
        }
        framePrevCursorNo = nowCursorNo[layerNo];
        if (isPushMoveButtonNow && !isPushMoveButtonPrev) {
            CursorButtonObject cursorButtonObject = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]];
            int num2 = 0;
            if (cursorButtonObject != null) {
                num2 = cursorButtonObject.cursorType;
            }
            if (no >= 0) {
                if (cursorLayer[layerNo].buttonObj.Length <= no) {
                    no = -2;
                    UnityEngine.Debug.Log("idx over");
                    isPushMoveButtonNow = false;
                } else if (cursorLayer[layerNo].buttonObj[no] == null) {
                    no = -2;
                    UnityEngine.Debug.Log("no cursor object");
                    isPushMoveButtonNow = false;
                }
            }
            if (no >= 0) {
                prevCursorNo[layerNo] = nowCursorNo[layerNo];
                nowCursorNo[layerNo] = no;
            } else if (no == -1) {
                int num3 = nowCursorNo[layerNo];
                nowCursorNo[layerNo] = prevCursorNo[layerNo];
                prevCursorNo[layerNo] = num3;
            }
            if (no != -2) {
                if (prevCursorNo[layerNo] != nowCursorNo[layerNo]) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
                }
                int cursorType = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].cursorType;
                cursorButtonObject = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]];
                cursorLayer[layerNo].cursorObj[cursorType].parent = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].transform;
                if (isFocus) {
                    cursorLayer[layerNo].buttonObj[prevCursorNo[layerNo]].transform.SetLocalScale(1f, 1f, 1f);
                    cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].transform.SetLocalScale(focusScale, focusScale, 1f);
                    Vector3 lossyScale = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].transform.lossyScale;
                    cursorLayer[layerNo].cursorObj[cursorType].transform.SetLocalScale(lossyScale.x / lossyScale.x * 1f, lossyScale.y / lossyScale.y * 1f, lossyScale.z / lossyScale.z * 1f);
                }
                if (cursorType == num2) {
                    if (isAnimation) {
                        LeanTween.moveLocal(cursorLayer[layerNo].cursorObj[cursorType].gameObject, new Vector3(0f, 0f, 1f), 0.05f).setEaseOutQuad().setIgnoreTimeScale(useUnScaledTime: true);
                    } else {
                        cursorLayer[layerNo].cursorObj[cursorType].gameObject.transform.SetLocalPositionX(0f);
                        cursorLayer[layerNo].cursorObj[cursorType].gameObject.transform.SetLocalPositionY(0f);
                    }
                } else {
                    cursorLayer[layerNo].cursorObj[cursorType].SetLocalPosition(0f, 0f, cursorLayer[layerNo].cursorObj[cursorType].localPosition.z);
                    if (cursorAnimObj != null) {
                        cursorAnimObj = cursorLayer[layerNo].cursorObj[cursorType].GetComponent<CursorAnimation>();
                    }
                }
                if (isStopToAnimObjDisable && cursorAnimObj != null) {
                    cursorAnimObj.SetDisable(!isStop);
                }
                for (int i = 0; i < cursorLayer[layerNo].cursorObj.Length; i++) {
                    cursorLayer[layerNo].cursorObj[i].gameObject.SetActive(cursorType == i);
                }
                cursorAnimObj.SetRectSetting(cursorButtonObject.RectSetting);
                if (isMovedCursorFixLocalPosZ) {
                    cursorAnimObj.transform.SetLocalPositionZ(-1f);
                }
            }
        }
        if (!isNoMoveInput && framePrevCursorNo == nowCursorNo[layerNo]) {
            isPushMoveButtonNow = false;
        }
        isPushMoveButtonPrev = isPushMoveButtonNow;
        isPushMoveButtonNow = false;
    }
    public void SetSpriteAllFrame(string _name) {
        for (int i = 0; i < cursorLayer.Length; i++) {
            for (int j = 0; j < cursorLayer[i].cursorObj.Length; j++) {
                cursorLayer[i].cursorObj[j].gameObject.GetComponent<SpriteRenderer>().sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, _name);
            }
        }
    }
    public bool IsOkButton() {
        if (SingletonCustom<SceneManager>.Instance.GetFadeFlg()) {
            return false;
        }
        return SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A, _isRepeat: false, KeyCode.None, useOnlyArrow, _isTimeMoving: true);
    }
    public bool IsReturnButton() {
        if (SingletonCustom<SceneManager>.Instance.GetFadeFlg()) {
            return false;
        }
        return SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B, _isRepeat: false, KeyCode.None, useOnlyArrow, _isTimeMoving: true);
    }
    protected bool IsUpButton() {
        if (SingletonCustom<SceneManager>.Instance.GetFadeFlg()) {
            return false;
        }
        if (isProvEnter) {
            return false;
        }
        return SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Dpad_Up, isInputRepeat, KeyCode.None, useOnlyArrow, _isTimeMoving: true);
    }
    protected bool IsDownButton() {
        if (SingletonCustom<SceneManager>.Instance.GetFadeFlg()) {
            return false;
        }
        if (isProvEnter) {
            return false;
        }
        return SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Dpad_Down, isInputRepeat, KeyCode.None, useOnlyArrow, _isTimeMoving: true);
    }
    protected bool IsLeftButton() {
        if (SingletonCustom<SceneManager>.Instance.GetFadeFlg()) {
            return false;
        }
        if (isProvEnter) {
            return false;
        }
        return SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Dpad_Left, isInputRepeat, KeyCode.None, useOnlyArrow, _isTimeMoving: true);
    }
    protected bool IsRightButton() {
        if (SingletonCustom<SceneManager>.Instance.GetFadeFlg()) {
            return false;
        }
        if (isProvEnter) {
            return false;
        }
        return SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Dpad_Right, isInputRepeat, KeyCode.None, useOnlyArrow, _isTimeMoving: true);
    }
    public void ChangeLayer(int _layerNo, int _cursorNo = -1, bool isChangeAnimCursor = true, bool isPrevCurSorSave = false) {
        cursorLayer[layerNo].cursorObj[cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].cursorType].gameObject.SetActive(value: false);
        if (isPrevCurSorSave) {
            prevCursorNo[_layerNo] = prevCursorNo[layerNo];
        }
        layerNo = _layerNo;
        if (_cursorNo != -1) {
            nowCursorNo[layerNo] = _cursorNo;
        }
        Transform obj = cursorLayer[layerNo].cursorObj[cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].cursorType];
        CursorButtonObject cursorButtonObject = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]];
        obj.SetPositionX(cursorButtonObject.transform.position.x);
        obj.SetPositionY(cursorButtonObject.transform.position.y);
        obj.gameObject.SetActive(value: true);
    }
    public int GetLayerNo() {
        return layerNo;
    }
    public void SetCursorPos(int _layerNo, int _cursorNo) {
        if (nowCursorNo == null || nowCursorNo.Length == 0) {
            Awake();
        }
        if (isFocus) {
            UnityEngine.Debug.Log("prev:" + cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].name);
            cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].transform.SetLocalScale(1f, 1f, 1f);
        }
        cursorLayer[layerNo].cursorObj[cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].cursorType].gameObject.SetActive(value: false);
        layerNo = _layerNo;
        nowCursorNo[layerNo] = _cursorNo;
        Transform obj = cursorLayer[layerNo].cursorObj[cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].cursorType];
        CursorButtonObject cursorButtonObject = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]];
        obj.parent = cursorButtonObject.transform;
        obj.SetLocalPositionX(0f);
        obj.SetLocalPositionY(0f);
        obj.gameObject.SetActive(value: true);
        if (isFocus) {
            UnityEngine.Debug.Log("focus:" + cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].name);
            cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].transform.SetLocalScale(focusScale, focusScale, 1f);
            int cursorType = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].cursorType;
            Vector3 lossyScale = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].transform.lossyScale;
            cursorLayer[layerNo].cursorObj[cursorType].transform.SetLocalScale(lossyScale.x / lossyScale.x * 1f, lossyScale.y / lossyScale.y * 1f, lossyScale.z / lossyScale.z * 1f);
        }
        if (cursorLayer[layerNo].buttonObj.Length != 0) {
            CursorButtonObject cursorButtonObject2 = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]];
            cursorAnimObj = cursorLayer[layerNo].cursorObj[cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].cursorType].GetComponent<CursorAnimation>();
            cursorAnimObj.SetRectSetting(cursorButtonObject2.RectSetting);
        }
    }
    public int GetSelectNo() {
        if (nowCursorNo == null) {
            return 0;
        }
        return nowCursorNo[layerNo];
    }
    public void SetSelectNo(int _no) {
        if (nowCursorNo == null) {
            Awake();
        }
        nowCursorNo[layerNo] = _no;
        UnityEngine.Debug.Log("設定ボタンNo" + _no.ToString());
    }
    public void SetCursorParent() {
        UnityEngine.Debug.Log("フレ\u30fcムの親変更");
        int cursorType = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].cursorType;
        cursorLayer[layerNo].cursorObj[cursorType].parent = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].transform;
        cursorLayer[layerNo].cursorObj[cursorType].SetLocalPosition(0f, 0f, -1f);
    }
    public bool IsPushMovedButtonMoment(MoveDir _moveDir = MoveDir.NONE) {
        if (_moveDir == MoveDir.NONE) {
            return isPushMoveButtonPrev;
        }
        if (moveDir != _moveDir) {
            return false;
        }
        return isPushMoveButtonPrev;
    }
    public void SetButtonObj(int _layerNo, GameObject[] _buttonObj) {
        nowCursorNo[_layerNo] = 0;
        prevCursorNo[_layerNo] = 0;
        cursorLayer[_layerNo].buttonObj = new CursorButtonObject[_buttonObj.Length];
        for (int i = 0; i < _buttonObj.Length; i++) {
            CursorButtonObject[] components = _buttonObj[i].GetComponents<CursorButtonObject>();
            cursorLayer[_layerNo].buttonObj[i] = components[0];
        }
        if (cursorLayer[layerNo].buttonObj.Length != 0) {
            CursorButtonObject cursorButtonObject = cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]];
            cursorAnimObj = cursorLayer[layerNo].cursorObj[cursorLayer[layerNo].buttonObj[nowCursorNo[layerNo]].cursorType].GetComponent<CursorAnimation>();
            cursorAnimObj.SetRectSetting(cursorButtonObject.RectSetting);
        }
    }
    public void SetButtonObj_NotChangeCursorNo(int _layerNo, GameObject[] _buttonObj) {
        cursorLayer[_layerNo].buttonObj = new CursorButtonObject[_buttonObj.Length];
        for (int i = 0; i < _buttonObj.Length; i++) {
            CursorButtonObject[] components = _buttonObj[i].GetComponents<CursorButtonObject>();
            cursorLayer[_layerNo].buttonObj[i] = components[0];
        }
    }
    public void SetMoveNo(int _layerNo, int _buttonNo, int _topNo, int _downNo, int _leftNo, int _rightNo) {
        cursorLayer[_layerNo].buttonObj[_buttonNo].topNo = _topNo;
        cursorLayer[_layerNo].buttonObj[_buttonNo].downNo = _downNo;
        cursorLayer[_layerNo].buttonObj[_buttonNo].leftNo = _leftNo;
        cursorLayer[_layerNo].buttonObj[_buttonNo].rightNo = _rightNo;
    }
    public void AddButtonObject(CursorButtonObject[] _obj, int _layerNo = 0) {
        int num = 0;
        Array.Resize(ref cursorLayer[_layerNo].button, cursorLayer[_layerNo].button.Length + _obj.Length);
        for (int i = 0; i < cursorLayer[_layerNo].button.Length; i++) {
            if (cursorLayer[_layerNo].button[i] == null) {
                cursorLayer[_layerNo].button[i] = _obj[num].gameObject;
                num++;
            }
        }
        num = 0;
        Array.Resize(ref cursorLayer[_layerNo].buttonObj, cursorLayer[_layerNo].buttonObj.Length + _obj.Length);
        for (int j = 0; j < cursorLayer[_layerNo].buttonObj.Length; j++) {
            if (cursorLayer[_layerNo].buttonObj[j] == null) {
                cursorLayer[_layerNo].buttonObj[j] = _obj[num];
                num++;
            }
        }
    }
    public CursorButtonObject GetNowSelectObject() {
        return cursorLayer[layerNo].buttonObj[GetSelectNo()];
    }
    public CursorButtonObject[] GetButtonObjs(int _layerNo) {
        return cursorLayer[layerNo].buttonObj;
    }
    private void OnDrawGizmos() {
    }
    protected bool IsActive() {
        return SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack() == LayerEnd;
    }
    public void SetProvEnter(bool _enable) {
        isProvEnter = _enable;
    }
    public void StopCurosriTween(float _value = 0f) {
        LeanTween.cancel(CursorObj.gameObject);
        cursorAnimObj.OnUpdateAnim(_value);
    }
    public int GetButtonObjLength(int _layerNo) {
        return cursorLayer[_layerNo].button.Length;
    }
    public int GetNo() {
        return no;
    }
}
