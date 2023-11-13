using GamepadInput;
using System;
using ControlFreak2;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
[DefaultExecutionOrder(-1)]
public class JoyConManager : SingletonCustom<JoyConManager> {
    public enum ControlMode {
        Gamepad,
        Keyboard,
        Mix,
        None
    }
    [Serializable]
    public class ControllerCallback : UnityEvent<bool> {
    }
    public enum MappingType {
        Menu,
        MainGame,
        GoEraser
    }
    public struct AXIS_INPUT {
        public Vector2 Stick_L;
        public Vector2 Stick_R;
        public SixAxisSensorInfo SixAxisSensor_L;
        public SixAxisSensorInfo SixAxisSensor_R;
    }
    public struct SixAxisSensorInfo {
        public Vector3 angle;
        public Vector3 angularVelocity;
        public Vector3 acceleration;
        public Quaternion quaternion;
    }
    public const int MaxSupportedController = 6;
    private readonly float LOST_TIME = 3f;
    private readonly float KEY_POWER_FIX = 1f;
    private readonly float FIRST_REPEAT = 0.5f;
    private readonly float NORMAL_REPEAT = 0.2f;
    private readonly int REPEAT_BUTTON_MAX = 28;
    private readonly float STICK_DOWN_THRESHOLD = 0.75f;
    private readonly float STICK_DISTANCE_SCALE = 450f;
    private int[] usageSettingId = new int[6] {
        0,
        1,
        2,
        3,
        4,
        5
    };
    private ControlMode[] previousControlMode = new ControlMode[6] {
        ControlMode.Keyboard,
        ControlMode.None,
        ControlMode.None,
        ControlMode.None,
        ControlMode.None,
        ControlMode.None
    };
    public ControlMode[] controlMode = new ControlMode[6] {
        ControlMode.Keyboard,
        ControlMode.None,
        ControlMode.None,
        ControlMode.None,
        ControlMode.None,
        ControlMode.None
    };
    public ControlMode[] defaultControlMode = new ControlMode[6] {
        ControlMode.Keyboard,
        ControlMode.None,
        ControlMode.None,
        ControlMode.None,
        ControlMode.None,
        ControlMode.None
    };
    [SerializeField]
    [Header("接続変動時のコ\u30fcルバック")]
    public ControllerCallback OnChangeConnect = new ControllerCallback();
    private readonly UnityEvent<ControlMode> mainPlayerControlModeChangeEvent = new UnityEvent<ControlMode>();
    private readonly UnityEvent<int, ControlMode> anyPlayerControlModeChangeEvent = new UnityEvent<int, ControlMode>();
    private MappingType mappingType;
    private AXIS_INPUT[] axisInputs;
    private AXIS_INPUT[] prevAxisInputs;
    private long[] currentButtons;
    private long[] preButtons;
    private float[,] repeatCnt;
    private bool isLock;
    private bool prevConnection;
    private bool isCheckConnection;
    private float lostCheckTime;
    private int singleJoyConCnt;
    private int enableCnt;
    private GameManager.PlayModeType currentPlayMode;
    private GameManager.PlayModeType settingPlayMode;
    private bool[] arrayCurrentRightStickUp = new bool[6];
    private bool[] arrayCurrentRightStickDown = new bool[6];
    private bool[] arrayCurrentRightStickLeft = new bool[6];
    private bool[] arrayCurrentRightStickRight = new bool[6];
    private bool[] arrayPrevRightStickUp = new bool[6];
    private bool[] arrayPrevRightStickDown = new bool[6];
    private bool[] arrayPrevRightStickLeft = new bool[6];
    private bool[] arrayPrevRightStickRight = new bool[6];
    private bool[] arrayCurrentLeftStickUp = new bool[6];
    private bool[] arrayCurrentLeftStickDown = new bool[6];
    private bool[] arrayCurrentLeftStickLeft = new bool[6];
    private bool[] arrayCurrentLeftStickRight = new bool[6];
    private bool[] arrayPrevLeftStickUp = new bool[6];
    private bool[] arrayPrevLeftStickDown = new bool[6];
    private bool[] arrayPrevLeftStickLeft = new bool[6];
    private bool[] arrayPrevLeftStickRight = new bool[6];
    private bool[] arrayCurrentDpadUp = new bool[6];
    private bool[] arrayCurrentDpadDown = new bool[6];
    private bool[] arrayCurrentDpadLeft = new bool[6];
    private bool[] arrayCurrentDpadRight = new bool[6];
    private bool[] arrayPrevDpadUp = new bool[6];
    private bool[] arrayPrevDpadDown = new bool[6];
    private bool[] arrayPrevDpadLeft = new bool[6];
    private bool[] arrayPrevDpadRight = new bool[6];
    private bool[] arrayCurrentLeftTrigger = new bool[6];
    private bool[] arrayCurrentRightTrigger = new bool[6];
    private bool[] arrayPrevLeftTrigger = new bool[6];
    private bool[] arrayPrevRightTrigger = new bool[6];
    private Vector2 _axisLStick;
    private Vector2 _axisRStick;
    private Vector2 _axisDPad;
    private float _triggerFloat;
    private bool isDebugButtonMultiCheck;
    private bool isDebugJoyConConnection = true;
    private bool isDebugAppletSetting = true;
    [SerializeField]
    private string[] showInspectorGamepadArray = new string[6];
    public bool IsHorizontal { get; set; }
    public int[] UsageSettingId {
        get { return usageSettingId; }
        set { usageSettingId = value; }
    }
    public int CurrentEnableCnt => enableCnt;
    public GameManager.PlayModeType SettingPlayMode {
        get { return settingPlayMode; }
        set { settingPlayMode = value; }
    }
    public GameManager.PlayModeType CurrentPlayMode => currentPlayMode;
    /// <summary>
    /// 
    /// </summary>
    public bool IsLock => isLock;
    /// <summary>
    /// 
    /// </summary>
    private static bool Init { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public event UnityAction<ControlMode> OnMainPlayerControlModeChanged;
    /// <summary>
    /// 
    /// </summary>
    public event UnityAction<int, ControlMode> OnAnyPlayerControlModeChanged;
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsCheckConnection() {
        if (isCheckConnection) {
            return lostCheckTime == 0f;
        }
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsCheckConnectionQuick() {
        return isCheckConnection;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="controllerNum"></param>
    /// <returns></returns>
    public bool IsCheckConnectionQuick(int controllerNum) {
        return SatGamePad.GetGamePadCount() >= controllerNum;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsMatchPlayMode() {
        return currentPlayMode == settingPlayMode;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_value"></param>
    public void SetLock(bool _value) {
        isLock = _value;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool GetAnyButtonDown() {
        if (isLock) {
            return false;
        }
        if (SatGamePad.GetGamePadCount() == 0) {
            return Input.anyKeyDown;
        }
        if (!SatGamePad.GetButtonDown(SatGamePad.Button.A, SatGamePad.Index.One) && !SatGamePad.GetButtonDown(SatGamePad.Button.B, SatGamePad.Index.One) && !SatGamePad.GetButtonDown(SatGamePad.Button.X, SatGamePad.Index.One) && !SatGamePad.GetButtonDown(SatGamePad.Button.Y, SatGamePad.Index.One) && !SatGamePad.GetButtonDown(SatGamePad.Button.RightShoulder, SatGamePad.Index.One) && !SatGamePad.GetButtonDown(SatGamePad.Button.LeftShoulder, SatGamePad.Index.One) && !SatGamePad.GetButtonDown(SatGamePad.Button.RightStick, SatGamePad.Index.One) && !SatGamePad.GetButtonDown(SatGamePad.Button.LeftStick, SatGamePad.Index.One) && !SatGamePad.GetButtonDown(SatGamePad.Button.Start, SatGamePad.Index.One)) {
            return SatGamePad.GetButtonDown(SatGamePad.Button.Back, SatGamePad.Index.One);
        }
        return true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_button"></param>
    /// <returns></returns>
    public bool GetMainPlayerButton(SatGamePad.Button _button) {
        bool result = false;
        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerNum; i++) {
            if (GetButton(i, _button)) {
                result = true;
            }
        }
        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_button"></param>
    /// <param name="useOnlyArrow"></param>
    /// <returns></returns>
    private bool GetMainPlayerButtonDownRepeat(SatGamePad.Button _button, bool useOnlyArrow) {
        return GetButtonDownRepeat(0, _button, useOnlyArrow);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_button"></param>
    /// <param name="_isRepeat"></param>
    /// <param name="overrideCode"></param>
    /// <param name="useOnlyArrow"></param>
    /// <param name="_isTimeMoving"></param>
    /// <returns></returns>
    public bool GetMainPlayerButtonDown(SatGamePad.Button _button, bool _isRepeat = false, KeyCode overrideCode = KeyCode.None, bool useOnlyArrow = false, bool _isTimeMoving = false) {
        if (!_isTimeMoving && Time.timeScale < 0.1f) {
            return false;
        }
        bool result = false;
        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerNum; i++) {
            if (_isRepeat && GetButtonDownRepeat(i, _button, useOnlyArrow)) {
                return true;
            }
            if (GetButtonDown(i, _button, _isRepeat, overrideCode, useOnlyArrow, _isTimeMoving)) {
                result = true;
            }
        }
        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_button"></param>
    /// <returns></returns>
    public bool GetMainPlayerButtonUp(SatGamePad.Button _button) {
        bool result = false;
        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerNum; i++) {
            if (GetButtonUp(i, _button)) {
                result = true;
            }
        }
        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerIdx"></param>
    /// <param name="_button"></param>
    /// <param name="overrideCode"></param>
    /// <returns></returns>
    public bool GetButtonUp(int _playerIdx, SatGamePad.Button _button, KeyCode overrideCode = KeyCode.None) {
        if (isLock) {
            return false;
        }
        switch (controlMode[_playerIdx]) {
            case ControlMode.Keyboard: {
                KeyCode key = KeyCode.C;
                switch (_button) {
                    case SatGamePad.Button.A:
                        key = KeyCode.Z;
                        break;
                    case SatGamePad.Button.B:
                        key = KeyCode.X;
                        break;
                    case SatGamePad.Button.X:
                        key = KeyCode.V;
                        break;
                    case SatGamePad.Button.Y:
                        key = KeyCode.C;
                        break;
                    case SatGamePad.Button.Dpad_Up:
                        return UnityEngine.Input.GetKeyUp(KeyCode.W)
                               || UnityEngine.Input.GetKeyUp(KeyCode.UpArrow)
                               || ControlFreak2.CF2Input.GetKeyUp(KeyCode.W)
                               || ControlFreak2.CF2Input.GetKeyUp(KeyCode.UpArrow);
                    case SatGamePad.Button.Dpad_Down:
                        return UnityEngine.Input.GetKeyUp(KeyCode.S)
                               || UnityEngine.Input.GetKeyUp(KeyCode.DownArrow)
                               || ControlFreak2.CF2Input.GetKeyUp(KeyCode.S)
                               || ControlFreak2.CF2Input.GetKeyUp(KeyCode.DownArrow);
                    case SatGamePad.Button.Dpad_Left:
                        return UnityEngine.Input.GetKeyUp(KeyCode.A)
                               || UnityEngine.Input.GetKeyUp(KeyCode.LeftArrow)
                               || ControlFreak2.CF2Input.GetKeyUp(KeyCode.A)
                               || ControlFreak2.CF2Input.GetKeyUp(KeyCode.LeftArrow);
                    case SatGamePad.Button.Dpad_Right:
                        return UnityEngine.Input.GetKeyUp(KeyCode.D)
                               || UnityEngine.Input.GetKeyUp(KeyCode.RightArrow)
                               || ControlFreak2.CF2Input.GetKeyUp(KeyCode.D)
                               || ControlFreak2.CF2Input.GetKeyUp(KeyCode.RightArrow);
                    case SatGamePad.Button.Start:
                        key = KeyCode.Escape;
                        break;
                    case SatGamePad.Button.Back:
                        key = KeyCode.Escape;
                        break;
                    case SatGamePad.Button.LeftShoulder:
                        key = KeyCode.R;
                        break;
                    case SatGamePad.Button.RightShoulder:
                        key = KeyCode.T;
                        break;
                    case SatGamePad.Button.LeftTrigger:
                        key = KeyCode.LeftShift;
                        break;
                    case SatGamePad.Button.RightTrigger:
                        key = KeyCode.RightShift;
                        break;
                }
                if (overrideCode != 0) {
                    key = overrideCode;
                }
                return UnityEngine.Input.GetKeyUp(key) || ControlFreak2.CF2Input.GetKeyUp(key);
            }
            case ControlMode.Gamepad:
                switch (_button) {
                    case SatGamePad.Button.A:
                        return SatGamePad.GetButtonUp(SatGamePad.Button.A, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.B:
                        return SatGamePad.GetButtonUp(SatGamePad.Button.B, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.X:
                        return SatGamePad.GetButtonUp(SatGamePad.Button.X, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.Y:
                        return SatGamePad.GetButtonUp(SatGamePad.Button.Y, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.Dpad_Up:
                        if (!arrayPrevLeftStickUp[_playerIdx] || arrayCurrentLeftStickUp[_playerIdx]) {
                            if (arrayPrevDpadUp[_playerIdx]) {
                                return !arrayCurrentDpadUp[_playerIdx];
                            }
                            return false;
                        }
                        return true;
                    case SatGamePad.Button.Dpad_Down:
                        if (!arrayPrevLeftStickDown[_playerIdx] || arrayCurrentLeftStickDown[_playerIdx]) {
                            if (arrayPrevDpadDown[_playerIdx]) {
                                return !arrayCurrentDpadDown[_playerIdx];
                            }
                            return false;
                        }
                        return true;
                    case SatGamePad.Button.Dpad_Left:
                        if (!arrayPrevLeftStickLeft[_playerIdx] || arrayCurrentLeftStickLeft[_playerIdx]) {
                            if (arrayPrevDpadLeft[_playerIdx]) {
                                return !arrayCurrentDpadLeft[_playerIdx];
                            }
                            return false;
                        }
                        return true;
                    case SatGamePad.Button.Dpad_Right:
                        if (!arrayPrevLeftStickRight[_playerIdx] || arrayCurrentLeftStickRight[_playerIdx]) {
                            if (arrayPrevDpadRight[_playerIdx]) {
                                return !arrayCurrentDpadRight[_playerIdx];
                            }
                            return false;
                        }
                        return true;
                    case SatGamePad.Button.Start:
                        return SatGamePad.GetButtonUp(SatGamePad.Button.Start, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.Back:
                        return SatGamePad.GetButtonUp(SatGamePad.Button.Back, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.LeftShoulder:
                        return SatGamePad.GetButtonUp(SatGamePad.Button.LeftShoulder, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.RightShoulder:
                        return SatGamePad.GetButtonUp(SatGamePad.Button.RightShoulder, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.RightStick:
                        return SatGamePad.GetButtonUp(SatGamePad.Button.RightStick, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.LeftStick:
                        return SatGamePad.GetButtonUp(SatGamePad.Button.LeftStick, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.LeftTrigger:
                        if (arrayPrevLeftTrigger[_playerIdx]) {
                            return !arrayCurrentLeftTrigger[_playerIdx];
                        }
                        return false;
                    case SatGamePad.Button.RightTrigger:
                        if (arrayPrevRightTrigger[_playerIdx]) {
                            return !arrayCurrentRightTrigger[_playerIdx];
                        }
                        return false;
                }
                break;
        }
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerIdx"></param>
    /// <param name="_button"></param>
    /// <param name="useOnlyArrow"></param>
    /// <returns></returns>
    private bool GetButtonDownRepeat(int _playerIdx, SatGamePad.Button _button, bool useOnlyArrow = false) {
        if (isLock) {
            return false;
        }
        if (GetButton(_playerIdx, _button, KeyCode.None, useOnlyArrow)) {
            if (repeatCnt[GetNpadRepeatButtonIdx(_button), _playerIdx] <= 0f) {
                if (GetButtonDown(_playerIdx, _button, _isRepeat: false, KeyCode.None, useOnlyArrow: false, _isTimeMoving: true)) {
                    repeatCnt[GetNpadRepeatButtonIdx(_button), _playerIdx] = FIRST_REPEAT;
                }
                else {
                    repeatCnt[GetNpadRepeatButtonIdx(_button), _playerIdx] = NORMAL_REPEAT;
                }
                return true;
            }
            repeatCnt[GetNpadRepeatButtonIdx(_button), _playerIdx] -= Time.unscaledDeltaTime;
            return false;
        }
        repeatCnt[GetNpadRepeatButtonIdx(_button), _playerIdx] = 0f;
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerIdx"></param>
    /// <param name="_button"></param>
    /// <param name="_isRepeat"></param>
    /// <param name="overrideCode"></param>
    /// <param name="useOnlyArrow"></param>
    /// <param name="_isTimeMoving"></param>
    /// <returns></returns>
    public bool GetButtonDown(int _playerIdx, SatGamePad.Button _button, bool _isRepeat = false, KeyCode overrideCode = KeyCode.None, bool useOnlyArrow = false, bool _isTimeMoving = false) {
        if (!_isTimeMoving && Time.timeScale < 0.1f) {
            return false;
        }
        if (_isRepeat) {
            return GetButtonDownRepeat(_playerIdx, _button, useOnlyArrow);
        }
        if (isLock) {
            return false;
        }
        switch (controlMode[_playerIdx]) {
            case ControlMode.Keyboard: {
                KeyCode key = KeyCode.C;
                switch (_button) {
                    case SatGamePad.Button.A:
                        key = KeyCode.Z;
                        break;
                    case SatGamePad.Button.B:
                        key = KeyCode.X;
                        break;
                    case SatGamePad.Button.X:
                        key = KeyCode.V;
                        break;
                    case SatGamePad.Button.Y:
                        key = KeyCode.C;
                        break;
                    case SatGamePad.Button.Dpad_Up:
                        if (useOnlyArrow) {
                            key = KeyCode.UpArrow;
                            break;
                        }
                        return UnityEngine.Input.GetKeyDown(KeyCode.W)
                               || UnityEngine.Input.GetKeyDown(KeyCode.UpArrow)
                               || ControlFreak2.CF2Input.GetKeyDown(KeyCode.W)
                               || ControlFreak2.CF2Input.GetKeyDown(KeyCode.UpArrow);
                    case SatGamePad.Button.Dpad_Down:
                        if (useOnlyArrow) {
                            key = KeyCode.DownArrow;
                            break;
                        }
                        return UnityEngine.Input.GetKeyDown(KeyCode.S)
                               || UnityEngine.Input.GetKeyDown(KeyCode.DownArrow)
                               || ControlFreak2.CF2Input.GetKeyDown(KeyCode.S)
                               || ControlFreak2.CF2Input.GetKeyDown(KeyCode.DownArrow);
                    case SatGamePad.Button.Dpad_Left:
                        if (useOnlyArrow) {
                            key = KeyCode.LeftArrow;
                            break;
                        }
                        return UnityEngine.Input.GetKeyDown(KeyCode.A)
                               || UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow)
                               || ControlFreak2.CF2Input.GetKeyDown(KeyCode.A)
                               || ControlFreak2.CF2Input.GetKeyDown(KeyCode.LeftArrow);
                    case SatGamePad.Button.Dpad_Right:
                        if (useOnlyArrow) {
                            key = KeyCode.RightArrow;
                            break;
                        }
                        return UnityEngine.Input.GetKeyDown(KeyCode.D)
                               || UnityEngine.Input.GetKeyDown(KeyCode.RightArrow)
                               || ControlFreak2.CF2Input.GetKeyDown(KeyCode.D)
                               || ControlFreak2.CF2Input.GetKeyDown(KeyCode.RightArrow);
                    case SatGamePad.Button.Start:
                        key = KeyCode.Escape;
                        break;
                    case SatGamePad.Button.Back:
                        key = KeyCode.Escape;
                        break;
                    case SatGamePad.Button.LeftShoulder:
                        key = KeyCode.R;
                        break;
                    case SatGamePad.Button.RightShoulder:
                        key = KeyCode.T;
                        break;
                    case SatGamePad.Button.LeftTrigger:
                        key = KeyCode.LeftShift;
                        break;
                    case SatGamePad.Button.RightTrigger:
                        key = KeyCode.RightShift;
                        break;
                }
                if (overrideCode != 0) {
                    key = overrideCode;
                }
                return UnityEngine.Input.GetKeyDown(key) || ControlFreak2.CF2Input.GetKeyDown(key);
            }
            case ControlMode.Gamepad:
                switch (_button) {
                    case SatGamePad.Button.A:
                        return SatGamePad.GetButtonDown(SatGamePad.Button.A, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.B:
                        return SatGamePad.GetButtonDown(SatGamePad.Button.B, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.X:
                        return SatGamePad.GetButtonDown(SatGamePad.Button.X, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.Y:
                        return SatGamePad.GetButtonDown(SatGamePad.Button.Y, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.Dpad_Up:
                        if (arrayPrevLeftStickUp[_playerIdx] || !arrayCurrentLeftStickUp[_playerIdx]) {
                            if (!arrayPrevDpadUp[_playerIdx]) {
                                return arrayCurrentDpadUp[_playerIdx];
                            }
                            return false;
                        }
                        return true;
                    case SatGamePad.Button.Dpad_Down:
                        if (arrayPrevLeftStickDown[_playerIdx] || !arrayCurrentLeftStickDown[_playerIdx]) {
                            if (!arrayPrevDpadDown[_playerIdx]) {
                                return arrayCurrentDpadDown[_playerIdx];
                            }
                            return false;
                        }
                        return true;
                    case SatGamePad.Button.Dpad_Left:
                        if (arrayPrevLeftStickLeft[_playerIdx] || !arrayCurrentLeftStickLeft[_playerIdx]) {
                            if (!arrayPrevDpadLeft[_playerIdx]) {
                                return arrayCurrentDpadLeft[_playerIdx];
                            }
                            return false;
                        }
                        return true;
                    case SatGamePad.Button.Dpad_Right:
                        if (arrayPrevLeftStickRight[_playerIdx] || !arrayCurrentLeftStickRight[_playerIdx]) {
                            if (!arrayPrevDpadRight[_playerIdx]) {
                                return arrayCurrentDpadRight[_playerIdx];
                            }
                            return false;
                        }
                        return true;
                    case SatGamePad.Button.Start:
                        return SatGamePad.GetButtonDown(SatGamePad.Button.Start, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.Back:
                        return SatGamePad.GetButtonDown(SatGamePad.Button.Back, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.LeftShoulder:
                        return SatGamePad.GetButtonDown(SatGamePad.Button.LeftShoulder, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.RightShoulder:
                        return SatGamePad.GetButtonDown(SatGamePad.Button.RightShoulder, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.RightStick:
                        return SatGamePad.GetButtonDown(SatGamePad.Button.RightStick, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.LeftStick:
                        return SatGamePad.GetButtonDown(SatGamePad.Button.LeftStick, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.LeftTrigger:
                        if (!arrayPrevLeftTrigger[_playerIdx]) {
                            return arrayCurrentLeftTrigger[_playerIdx];
                        }
                        return false;
                    case SatGamePad.Button.RightTrigger:
                        if (!arrayPrevRightTrigger[_playerIdx]) {
                            return arrayCurrentRightTrigger[_playerIdx];
                        }
                        return false;
                }
                break;
        }
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerIdx"></param>
    /// <param name="_button"></param>
    /// <param name="overrideCode"></param>
    /// <param name="useOnlyArrow"></param>
    /// <returns></returns>
    public bool GetButton(int _playerIdx, SatGamePad.Button _button, KeyCode overrideCode = KeyCode.None, bool useOnlyArrow = false) {
        if (isLock) {
            return false;
        }
        switch (controlMode[_playerIdx]) {
            case ControlMode.Keyboard: {
                KeyCode key = KeyCode.C;
                switch (_button) {
                    case SatGamePad.Button.A:
                        key = KeyCode.Z;
                        break;
                    case SatGamePad.Button.B:
                        key = KeyCode.X;
                        break;
                    case SatGamePad.Button.X:
                        key = KeyCode.V;
                        break;
                    case SatGamePad.Button.Y:
                        key = KeyCode.C;
                        break;
                    case SatGamePad.Button.Dpad_Up:
                        if (useOnlyArrow) {
                            key = KeyCode.UpArrow;
                            break;
                        }
                        return UnityEngine.Input.GetKey(KeyCode.W)
                               || UnityEngine.Input.GetKey(KeyCode.UpArrow)
                               || ControlFreak2.CF2Input.GetKey(KeyCode.W)
                               || ControlFreak2.CF2Input.GetKey(KeyCode.UpArrow);
                    case SatGamePad.Button.Dpad_Down:
                        if (useOnlyArrow) {
                            key = KeyCode.DownArrow;
                            break;
                        }
                        return UnityEngine.Input.GetKey(KeyCode.S)
                               || UnityEngine.Input.GetKey(KeyCode.DownArrow)
                               || ControlFreak2.CF2Input.GetKey(KeyCode.S)
                               || ControlFreak2.CF2Input.GetKey(KeyCode.DownArrow);
                    case SatGamePad.Button.Dpad_Left:
                        if (useOnlyArrow) {
                            key = KeyCode.LeftArrow;
                            break;
                        }
                        return UnityEngine.Input.GetKey(KeyCode.A)
                               || UnityEngine.Input.GetKey(KeyCode.LeftArrow)
                               || ControlFreak2.CF2Input.GetKey(KeyCode.A)
                               || ControlFreak2.CF2Input.GetKey(KeyCode.LeftArrow);
                    case SatGamePad.Button.Dpad_Right:
                        if (useOnlyArrow) {
                            key = KeyCode.RightArrow;
                            break;
                        }
                        return UnityEngine.Input.GetKey(KeyCode.D)
                               || UnityEngine.Input.GetKey(KeyCode.RightArrow)
                               || ControlFreak2.CF2Input.GetKey(KeyCode.D)
                               || ControlFreak2.CF2Input.GetKey(KeyCode.RightArrow);
                    case SatGamePad.Button.Start:
                        key = KeyCode.Escape;
                        break;
                    case SatGamePad.Button.Back:
                        key = KeyCode.Q;
                        break;
                    case SatGamePad.Button.LeftShoulder:
                        key = KeyCode.R;
                        break;
                    case SatGamePad.Button.RightShoulder:
                        key = KeyCode.T;
                        break;
                    case SatGamePad.Button.LeftTrigger:
                        key = KeyCode.LeftShift;
                        break;
                    case SatGamePad.Button.RightTrigger:
                        key = KeyCode.RightShift;
                        break;
                }
                if (overrideCode != 0) {
                    key = overrideCode;
                }
                return UnityEngine.Input.GetKey(key) || ControlFreak2.CF2Input.GetKey(key);
            }
            case ControlMode.Gamepad:
                switch (_button) {
                    case SatGamePad.Button.A:
                        return SatGamePad.GetButton(SatGamePad.Button.A, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.B:
                        return SatGamePad.GetButton(SatGamePad.Button.B, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.X:
                        return SatGamePad.GetButton(SatGamePad.Button.X, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.Y:
                        return SatGamePad.GetButton(SatGamePad.Button.Y, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.Dpad_Up:
                        if (!arrayCurrentLeftStickUp[_playerIdx]) {
                            return arrayCurrentDpadUp[_playerIdx];
                        }
                        return true;
                    case SatGamePad.Button.Dpad_Down:
                        if (!arrayCurrentLeftStickDown[_playerIdx]) {
                            return arrayCurrentDpadDown[_playerIdx];
                        }
                        return true;
                    case SatGamePad.Button.Dpad_Left:
                        if (!arrayCurrentLeftStickLeft[_playerIdx]) {
                            return arrayCurrentDpadLeft[_playerIdx];
                        }
                        return true;
                    case SatGamePad.Button.Dpad_Right:
                        if (!arrayCurrentLeftStickRight[_playerIdx]) {
                            return arrayCurrentDpadRight[_playerIdx];
                        }
                        return true;
                    case SatGamePad.Button.Start:
                        return SatGamePad.GetButton(SatGamePad.Button.Start, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.Back:
                        return SatGamePad.GetButton(SatGamePad.Button.Back, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.LeftShoulder:
                        return SatGamePad.GetButton(SatGamePad.Button.LeftShoulder, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.RightShoulder:
                        return SatGamePad.GetButton(SatGamePad.Button.RightShoulder, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.RightStick:
                        return SatGamePad.GetButton(SatGamePad.Button.RightStick, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.LeftStick:
                        return SatGamePad.GetButton(SatGamePad.Button.LeftStick, (SatGamePad.Index)_playerIdx);
                    case SatGamePad.Button.LeftTrigger:
                        return arrayCurrentLeftTrigger[_playerIdx];
                    case SatGamePad.Button.RightTrigger:
                        return arrayCurrentRightTrigger[_playerIdx];
                }
                break;
        }
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerIdx"></param>
    /// <returns></returns>
    public AXIS_INPUT GetPrevAxisInput(int _playerIdx) {
        AXIS_INPUT result = prevAxisInputs[_playerIdx];
        result.Stick_L.x /= KEY_POWER_FIX;
        result.Stick_L.y /= KEY_POWER_FIX;
        result.Stick_R.x /= KEY_POWER_FIX;
        result.Stick_R.y /= KEY_POWER_FIX;
        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerIdx"></param>
    /// <returns></returns>
    public AXIS_INPUT GetAxisInput(int _playerIdx) {
        AXIS_INPUT result = axisInputs[_playerIdx];
        result.Stick_L.x /= KEY_POWER_FIX;
        result.Stick_L.y /= KEY_POWER_FIX;
        result.Stick_R.x /= KEY_POWER_FIX;
        result.Stick_R.y /= KEY_POWER_FIX;
        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playMode"></param>
    /// <param name="_isInitUsageSetting"></param>
    public void SetPlayMode(GameManager.PlayModeType _playMode, bool _isInitUsageSetting = true) {
        if (_isInitUsageSetting) {
            usageSettingId = new int[6] {
                0,
                1,
                2,
                3,
                4,
                5
            };
        }
        singleJoyConCnt = 0;
        currentPlayMode = _playMode;
        if (_playMode != GameManager.PlayModeType.JOYCON_SINGLE) {
            settingPlayMode = _playMode;
        }
        repeatCnt = new float[REPEAT_BUTTON_MAX, 6];
        axisInputs = new AXIS_INPUT[6];
        prevAxisInputs = new AXIS_INPUT[6];
        Update();
        UnityEngine.Debug.Log("setting:" + settingPlayMode.ToString());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool ShowControllerSupport() {
        return true;
    }
    /// <summary>
    /// 
    /// </summary>
    public void ManualOnChangeConnect() {
        OnChangeConnect.Invoke(arg0: true);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_type"></param>
    public void SetMappingType(MappingType _type) {
        UnityEngine.Debug.Log("setMappingType:" + _type.ToString());
        mappingType = _type;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_npadButton"></param>
    /// <returns></returns>
    private int GetNpadRepeatButtonIdx(SatGamePad.Button _npadButton) {
        switch (_npadButton) {
            case SatGamePad.Button.A:
                return 0;
            case SatGamePad.Button.B:
                return 1;
            case SatGamePad.Button.X:
                return 2;
            case SatGamePad.Button.Y:
                return 3;
            case SatGamePad.Button.Dpad_Up:
                return 4;
            case SatGamePad.Button.Dpad_Down:
                return 5;
            case SatGamePad.Button.Dpad_Left:
                return 6;
            case SatGamePad.Button.Dpad_Right:
                return 7;
            case SatGamePad.Button.Start:
                return 8;
            case SatGamePad.Button.Back:
                return 9;
            case SatGamePad.Button.LeftShoulder:
                return 10;
            case SatGamePad.Button.RightShoulder:
                return 11;
            case SatGamePad.Button.LeftTrigger:
                return 12;
            case SatGamePad.Button.RightTrigger:
                return 13;
            default:
                return 0;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void Awake() {
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        isCheckConnection = false;
#if UNITY_EDITOR
        //BKB force four player during editor mode to testing
        SetPlayMode(GameManager.PlayModeType.JOYCON_FOUR);
#else
        SetPlayMode(GameManager.PlayModeType.SINGLE);
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public void ConnectUpdate() {
        enableCnt = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        if (enableCnt > arrayCurrentLeftTrigger.Length) {
            enableCnt = arrayCurrentLeftTrigger.Length;
        }
        // Axis
        for (int num3 = 0; num3 < enableCnt; num3++) {
            prevAxisInputs[num3] = axisInputs[num3];
            axisInputs[num3].Stick_L.x = 0f;
            axisInputs[num3].Stick_L.y = 0f;
            axisInputs[num3].Stick_R.x = 0f;
            axisInputs[num3].Stick_R.y = 0f;
            switch (controlMode[num3]) {
                case ControlMode.Keyboard:
                    axisInputs[num3].Stick_L.x = ControlFreak2.CF2Input.GetAxis("Keyboard_Horizontal");
                    axisInputs[num3].Stick_L.x = axisInputs[num3].Stick_L.x == 0 ? UnityEngine.Input.GetAxis("Keyboard_Horizontal") : axisInputs[num3].Stick_L.x;
                    axisInputs[num3].Stick_L.y = ControlFreak2.CF2Input.GetAxis("Keyboard_Vertical");
                    axisInputs[num3].Stick_L.y = axisInputs[num3].Stick_L.y == 0 ? UnityEngine.Input.GetAxis("Keyboard_Vertical") : axisInputs[num3].Stick_L.y;
                    axisInputs[num3].Stick_R = Vector2.zero;
                    break;
                case ControlMode.Gamepad:
                    axisInputs[num3].Stick_L.x = SatGamePad.GetAxis(SatGamePad.Axis.LeftStick, (SatGamePad.Index)num3).x;
                    axisInputs[num3].Stick_L.y = SatGamePad.GetAxis(SatGamePad.Axis.LeftStick, (SatGamePad.Index)num3).y;
                    axisInputs[num3].Stick_R.x = SatGamePad.GetAxis(SatGamePad.Axis.RightStick, (SatGamePad.Index)num3).x;
                    axisInputs[num3].Stick_R.y = SatGamePad.GetAxis(SatGamePad.Axis.RightStick, (SatGamePad.Index)num3).y;
                    break;
            }
        }
        // Right-stick
        for (int i = 0; i < arrayCurrentRightStickDown.Length; i++) {
            arrayPrevRightStickUp[i] = arrayCurrentRightStickUp[i];
            arrayPrevRightStickDown[i] = arrayCurrentRightStickDown[i];
            arrayPrevRightStickLeft[i] = arrayCurrentRightStickLeft[i];
            arrayPrevRightStickRight[i] = arrayCurrentRightStickRight[i];
        }
        for (int j = 0; j < enableCnt; j++) {
            _axisRStick = axisInputs[j].Stick_R;
            arrayCurrentRightStickUp[j] = (_axisRStick.y > 0.8f);
            arrayCurrentRightStickDown[j] = (_axisRStick.y < -0.8f);
            arrayCurrentRightStickLeft[j] = (_axisRStick.x < -0.8f);
            arrayCurrentRightStickRight[j] = (_axisRStick.x > 0.8f);
        }
        // Left-stick
        for (int k = 0; k < arrayCurrentLeftStickDown.Length; k++) {
            arrayPrevLeftStickUp[k] = arrayCurrentLeftStickUp[k];
            arrayPrevLeftStickDown[k] = arrayCurrentLeftStickDown[k];
            arrayPrevLeftStickLeft[k] = arrayCurrentLeftStickLeft[k];
            arrayPrevLeftStickRight[k] = arrayCurrentLeftStickRight[k];
        }
        for (int l = 0; l < enableCnt; l++) {
            _axisLStick = axisInputs[l].Stick_L;
            arrayCurrentLeftStickUp[l] = (_axisLStick.y > 0.8f);
            arrayCurrentLeftStickDown[l] = (_axisLStick.y < -0.8f);
            arrayCurrentLeftStickLeft[l] = (_axisLStick.x < -0.8f);
            arrayCurrentLeftStickRight[l] = (_axisLStick.x > 0.8f);
        }
        // Dpad
        for (int m = 0; m < arrayCurrentDpadUp.Length; m++) {
            arrayPrevDpadUp[m] = arrayCurrentDpadUp[m];
            arrayPrevDpadDown[m] = arrayCurrentDpadDown[m];
            arrayPrevDpadLeft[m] = arrayCurrentDpadLeft[m];
            arrayPrevDpadRight[m] = arrayCurrentDpadRight[m];
        }
        for (int n = 0; n < enableCnt; n++) {
            arrayCurrentDpadUp[n] = SatGamePad.GetButton(SatGamePad.Button.Dpad_Up, (SatGamePad.Index)n);
            arrayCurrentDpadDown[n] = SatGamePad.GetButton(SatGamePad.Button.Dpad_Down, (SatGamePad.Index)n);
            arrayCurrentDpadLeft[n] = SatGamePad.GetButton(SatGamePad.Button.Dpad_Left, (SatGamePad.Index)n);
            arrayCurrentDpadRight[n] = SatGamePad.GetButton(SatGamePad.Button.Dpad_Right, (SatGamePad.Index)n);
        }
        // Triggers
        for (int num = 0; num < arrayCurrentLeftTrigger.Length; num++) {
            arrayPrevLeftTrigger[num] = arrayCurrentLeftTrigger[num];
            arrayPrevRightTrigger[num] = arrayCurrentRightTrigger[num];
        }
        for (int num2 = 0; num2 < enableCnt; num2++) {
            arrayCurrentLeftTrigger[num2] = SatGamePad.GetButton(SatGamePad.Button.LeftTrigger, (SatGamePad.Index)num2);
            arrayCurrentRightTrigger[num2] = SatGamePad.GetButton(SatGamePad.Button.RightTrigger, (SatGamePad.Index)num2);
        }
        isCheckConnection = true;
    }
    /// <summary>
    /// 
    /// </summary>
    private void Update() {
        SatGamePad.Init();
        SatGamePad.UpdateConnect();
        ConnectSettings(_isInit: true);
        ConnectUpdate();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_isInit"></param>
    public void ConnectSettings(bool _isInit = false) {
        for (int i = 0; i < showInspectorGamepadArray.Length; i++) {
            if (SatGamePad.gamepadArray[i] != null) {
                showInspectorGamepadArray[i] = SatGamePad.gamepadArray[i].displayName;
            }
            else {
                showInspectorGamepadArray[i] = "";
            }
        }
        for (int j = 0; j < controlMode.Length; j++) {
            controlMode[j] = ControlMode.None;
            defaultControlMode[j] = ControlMode.None;
        }
        for (int k = 0; k < controlMode.Length; k++) {
            if (_isInit && k < GameSettingManager.Instance.PlayerNum) {
                // Only map keyboard to last player if it not null (and Gamepad is not available)
                bool isLastPlayer = k == GameSettingManager.Instance.PlayerNum - 1;
                bool isKeyboardAvailable = Keyboard.current != null;
                bool isGamepadAvailable = SatGamePad.gamepadArray[k] != null;
                controlMode[k] = isLastPlayer && isKeyboardAvailable && !isGamepadAvailable
                    ? ControlMode.Keyboard
                    : ControlMode.Gamepad;
                defaultControlMode[k] = controlMode[k];
            }
            controlMode[k] = defaultControlMode[k];
        }
        for (int l = 0; l < controlMode.Length; l++) {
            if (previousControlMode[l] != controlMode[l]) {
                previousControlMode[l] = controlMode[l];
                if (l == 0) {
                    mainPlayerControlModeChangeEvent.Invoke(previousControlMode[0]);
                }
                anyPlayerControlModeChangeEvent.Invoke(l, previousControlMode[l]);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetControllerCount() {
        int count = SatGamePad.GetGamePadCount();
        count += Keyboard.current != null ? 1 : 0;
        return count;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetEditorTargetInt() {
        return 0;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsSingleMode() {
        return settingPlayMode == GameManager.PlayModeType.SINGLE;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerNo"></param>
    /// <param name="_isDefaultAccess"></param>
    /// <returns></returns>
    public int GetCurrentNpadId(int _playerNo, bool _isDefaultAccess = false) {
        if (SingletonCustom<GameSettingManager>.Instance.IsSingleController) {
            return 0;
        }
        return _playerNo;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetMainPlayerNpadId() {
        return 0;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerIdx"></param>
    /// <param name="_isMainPlayer"></param>
    /// <returns></returns>
    public float GetMoveLength(int _playerIdx, bool _isMainPlayer = false) {
        if (SingletonCustom<GameSettingManager>.Instance.ControllerNum == 1) {
            _playerIdx = 0;
        }
        AXIS_INPUT prevAxisInput = GetPrevAxisInput(_playerIdx);
        return Vector2.Distance(Vector2.zero, new Vector2(prevAxisInput.Stick_L.y * STICK_DISTANCE_SCALE, prevAxisInput.Stick_L.x * STICK_DISTANCE_SCALE));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerNo"></param>
    /// <param name="_isMainPlayer"></param>
    /// <returns></returns>
    public Vector3 GetMoveDir(int _playerNo, bool _isMainPlayer = false) {
        if (SingletonCustom<GameSettingManager>.Instance.ControllerNum == 1) {
            _playerNo = 0;
        }
        AXIS_INPUT prevAxisInput = GetPrevAxisInput(_playerNo);
        if (IsHorizontal) {
            return Vector3.Normalize(new Vector3(0f - prevAxisInput.Stick_L.y, prevAxisInput.Stick_L.x, 0f));
        }
        return Vector3.Normalize(new Vector3(prevAxisInput.Stick_L.x, prevAxisInput.Stick_L.y, 0f));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsJoyButtonFull() {
        return true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerIdx"></param>
    /// <returns></returns>
    public bool IsJoyButtonFull(int _playerIdx) {
        return true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerIdx"></param>
    /// <returns></returns>
    public bool IsKeyboardInput(int _playerIdx) {
        if (_playerIdx < 0) {
            return false;
        }
        return controlMode[_playerIdx] == ControlMode.Keyboard;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public bool IsMainPlayerControlMode(ControlMode mode) {
        return controlMode[0] == mode;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerIdx"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public bool IsPlayerControlMode(int _playerIdx, ControlMode mode) {
        if (_playerIdx < 0) {
            return false;
        }
        return controlMode[_playerIdx] == mode;
    }
}