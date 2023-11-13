using System;
using UnityEngine;
using UnityEngine.InputSystem;
#if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_STANDALONE_LINUX
using UnityEngine.InputSystem.Switch;
#endif
namespace GamepadInput {
    /// <summary>
    /// 
    /// </summary>
    public static class SatGamePad {
        /// <summary>
        /// 
        /// </summary>
        public enum Button {
            A,
            B,
            Y,
            X,
            RightShoulder,
            LeftShoulder,
            RightStick,
            LeftStick,
            Back,
            Start,
            Dpad_Up,
            Dpad_Down,
            Dpad_Left,
            Dpad_Right,
            LeftTrigger,
            RightTrigger
        }
        /// <summary>
        /// 
        /// </summary>
        public enum Trigger {
            LeftTrigger,
            RightTrigger
        }
        public enum Axis {
            LeftStick,
            RightStick,
            Dpad
        }
        /// <summary>
        /// 
        /// </summary>
        public enum Index {
            One,
            Two,
            Three,
            Four,
            Five,
            Six
        }
        /// <summary>
        /// 
        /// </summary>
        public const float AXIS_THRESHOLD = 0.8f;
        /// <summary>
        /// 
        /// </summary>
        public static Gamepad[] gamepadArray = new Gamepad[6];
        /// <summary>
        /// 
        /// </summary>
        private static int prevGamePadCount = -1;
        /// <summary>
        /// 
        /// </summary>
        private static bool isInit = false;
        /// <summary>
        /// 
        /// </summary>
        public static void Init() {
            if (!isInit) {
                for (int i = 0; i < Mathf.Min(gamepadArray.Length, Gamepad.all.Count); i++) {
                    gamepadArray[i] = Gamepad.all[i];
                }
                prevGamePadCount = Gamepad.all.Count;
                isInit = true;
                SingletonCustom<JoyConManager>.Instance.ConnectSettings(_isInit: true);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void UpdateConnect() {
            if (!isInit) {
                return;
            }
            if (prevGamePadCount > Gamepad.all.Count) {
                for (int i = 0; i < gamepadArray.Length; i++) {
                    if (gamepadArray[i] == null) {
                        continue;
                    }
                    bool flag = false;
                    for (int j = 0; j < Gamepad.all.Count; j++) {
                        if (gamepadArray[i].deviceId == Gamepad.all[j].deviceId) {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag) {
                        gamepadArray[i] = null;
                        UnityEngine.Debug.Log($"[{Time.frameCount}] 接続が減った {i + 1}P");
                    }
                }
            } else if (prevGamePadCount < Gamepad.all.Count) {
                for (int k = 0; k < Gamepad.all.Count; k++) {
                    Gamepad gamepad = Gamepad.all[k];
                    bool flag2 = false;
                    for (int l = 0; l < gamepadArray.Length; l++) {
                        if (gamepadArray[l] != null && gamepadArray[l].deviceId == gamepad.deviceId) {
                            flag2 = true;
                            break;
                        }
                    }
                    if (flag2) {
                        continue;
                    }
                    for (int m = 0; m < gamepadArray.Length; m++) {
                        if (gamepadArray[m] == null) {
                            gamepadArray[m] = gamepad;
                            UnityEngine.Debug.Log($"[{Time.frameCount}] 接続が増えた {m + 1}P__{gamepad}");
                            break;
                        }
                    }
                }
            } else {
                for (int n = 0; n < gamepadArray.Length; n++) {
                    Gamepad gamepad2 = gamepadArray[n];
                }
            }
            prevGamePadCount = Gamepad.all.Count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetGamePadCount() {
            return Gamepad.all.Count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controlIndex"></param>
        /// <returns></returns>
        public static bool GetButtonDown(Button button, Index controlIndex) {
            Gamepad gamepad = gamepadArray[(int)controlIndex];
            if (gamepad == null || GetGamePadCount() <= 0) {
                return false;
            }
#if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_STANDALONE_LINUX
            if (gamepad is SwitchProControllerHID) {
                switch (button) {
                    case Button.A:
                        return gamepad.bButton.wasPressedThisFrame;
                    case Button.B:
                        return gamepad.aButton.wasPressedThisFrame;
                    case Button.Y:
                        return gamepad.xButton.wasPressedThisFrame;
                    case Button.X:
                        return gamepad.yButton.wasPressedThisFrame;
                    case Button.RightShoulder:
                        return gamepad.rightShoulder.wasPressedThisFrame;
                    case Button.LeftShoulder:
                        return gamepad.leftShoulder.wasPressedThisFrame;
                    case Button.RightStick:
                        return gamepad.rightStickButton.wasPressedThisFrame;
                    case Button.LeftStick:
                        return gamepad.leftStickButton.wasPressedThisFrame;
                    case Button.Back:
                        return gamepad.selectButton.wasPressedThisFrame;
                    case Button.Start:
                        return gamepad.startButton.wasPressedThisFrame;
                    case Button.Dpad_Up:
                        return gamepad.dpad.up.wasPressedThisFrame;
                    case Button.Dpad_Down:
                        return gamepad.dpad.down.wasPressedThisFrame;
                    case Button.Dpad_Left:
                        return gamepad.dpad.left.wasPressedThisFrame;
                    case Button.Dpad_Right:
                        return gamepad.dpad.right.wasPressedThisFrame;
                    case Button.LeftTrigger:
                        return gamepad.leftTrigger.wasPressedThisFrame;
                    case Button.RightTrigger:
                        return gamepad.rightTrigger.wasPressedThisFrame;
                }
            } else
#endif
            {
                switch (button) {
                    case Button.A:
                        return gamepad.aButton.wasPressedThisFrame;
                    case Button.B:
                        return gamepad.bButton.wasPressedThisFrame;
                    case Button.Y:
                        return gamepad.yButton.wasPressedThisFrame;
                    case Button.X:
                        return gamepad.xButton.wasPressedThisFrame;
                    case Button.RightShoulder:
                        return gamepad.rightShoulder.wasPressedThisFrame;
                    case Button.LeftShoulder:
                        return gamepad.leftShoulder.wasPressedThisFrame;
                    case Button.RightStick:
                        return gamepad.rightStickButton.wasPressedThisFrame;
                    case Button.LeftStick:
                        return gamepad.leftStickButton.wasPressedThisFrame;
                    case Button.Back:
                        return gamepad.selectButton.wasPressedThisFrame;
                    case Button.Start:
                        return gamepad.startButton.wasPressedThisFrame;
                    case Button.Dpad_Up:
                        return gamepad.dpad.up.wasPressedThisFrame;
                    case Button.Dpad_Down:
                        return gamepad.dpad.down.wasPressedThisFrame;
                    case Button.Dpad_Left:
                        return gamepad.dpad.left.wasPressedThisFrame;
                    case Button.Dpad_Right:
                        return gamepad.dpad.right.wasPressedThisFrame;
                    case Button.LeftTrigger:
                        return gamepad.leftTrigger.wasPressedThisFrame;
                    case Button.RightTrigger:
                        return gamepad.rightTrigger.wasPressedThisFrame;
                }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controlIndex"></param>
        /// <returns></returns>
        public static bool GetButtonUp(Button button, Index controlIndex) {
            Gamepad gamepad = gamepadArray[(int)controlIndex];
            if (gamepad == null || GetGamePadCount() <= 0) {
                return false;
            }
#if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_STANDALONE_LINUX
            if (gamepad is SwitchProControllerHID) {
                switch (button) {
                    case Button.A:
                        return gamepad.bButton.wasReleasedThisFrame;
                    case Button.B:
                        return gamepad.aButton.wasReleasedThisFrame;
                    case Button.Y:
                        return gamepad.xButton.wasReleasedThisFrame;
                    case Button.X:
                        return gamepad.yButton.wasReleasedThisFrame;
                    case Button.RightShoulder:
                        return gamepad.rightShoulder.wasReleasedThisFrame;
                    case Button.LeftShoulder:
                        return gamepad.leftShoulder.wasReleasedThisFrame;
                    case Button.RightStick:
                        return gamepad.rightStickButton.wasReleasedThisFrame;
                    case Button.LeftStick:
                        return gamepad.leftStickButton.wasReleasedThisFrame;
                    case Button.Back:
                        return gamepad.selectButton.wasReleasedThisFrame;
                    case Button.Start:
                        return gamepad.startButton.wasReleasedThisFrame;
                    case Button.Dpad_Up:
                        return gamepad.dpad.up.wasReleasedThisFrame;
                    case Button.Dpad_Down:
                        return gamepad.dpad.down.wasReleasedThisFrame;
                    case Button.Dpad_Left:
                        return gamepad.dpad.left.wasReleasedThisFrame;
                    case Button.Dpad_Right:
                        return gamepad.dpad.right.wasReleasedThisFrame;
                    case Button.LeftTrigger:
                        return gamepad.leftTrigger.wasReleasedThisFrame;
                    case Button.RightTrigger:
                        return gamepad.rightTrigger.wasReleasedThisFrame;
                }
            } else
#endif
            {
                switch (button) {
                    case Button.A:
                        return gamepad.aButton.wasReleasedThisFrame;
                    case Button.B:
                        return gamepad.bButton.wasReleasedThisFrame;
                    case Button.Y:
                        return gamepad.yButton.wasReleasedThisFrame;
                    case Button.X:
                        return gamepad.xButton.wasReleasedThisFrame;
                    case Button.RightShoulder:
                        return gamepad.rightShoulder.wasReleasedThisFrame;
                    case Button.LeftShoulder:
                        return gamepad.leftShoulder.wasReleasedThisFrame;
                    case Button.RightStick:
                        return gamepad.rightStickButton.wasReleasedThisFrame;
                    case Button.LeftStick:
                        return gamepad.leftStickButton.wasReleasedThisFrame;
                    case Button.Back:
                        return gamepad.selectButton.wasReleasedThisFrame;
                    case Button.Start:
                        return gamepad.startButton.wasReleasedThisFrame;
                    case Button.Dpad_Up:
                        return gamepad.dpad.up.wasReleasedThisFrame;
                    case Button.Dpad_Down:
                        return gamepad.dpad.down.wasReleasedThisFrame;
                    case Button.Dpad_Left:
                        return gamepad.dpad.left.wasReleasedThisFrame;
                    case Button.Dpad_Right:
                        return gamepad.dpad.right.wasReleasedThisFrame;
                    case Button.LeftTrigger:
                        return gamepad.leftTrigger.wasReleasedThisFrame;
                    case Button.RightTrigger:
                        return gamepad.rightTrigger.wasReleasedThisFrame;
                }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controlIndex"></param>
        /// <returns></returns>
        public static bool GetButton(Button button, Index controlIndex) {
            Gamepad gamepad = gamepadArray[(int)controlIndex];
            if (gamepad == null || GetGamePadCount() <= 0) {
                return false;
            }
#if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_STANDALONE_LINUX
            if (gamepad is SwitchProControllerHID) {
                switch (button) {
                    case Button.A:
                        return gamepad.bButton.isPressed;
                    case Button.B:
                        return gamepad.aButton.isPressed;
                    case Button.Y:
                        return gamepad.xButton.isPressed;
                    case Button.X:
                        return gamepad.yButton.isPressed;
                    case Button.RightShoulder:
                        return gamepad.rightShoulder.isPressed;
                    case Button.LeftShoulder:
                        return gamepad.leftShoulder.isPressed;
                    case Button.RightStick:
                        return gamepad.rightStickButton.isPressed;
                    case Button.LeftStick:
                        return gamepad.leftStickButton.isPressed;
                    case Button.Back:
                        return gamepad.selectButton.isPressed;
                    case Button.Start:
                        return gamepad.startButton.isPressed;
                    case Button.Dpad_Up:
                        return gamepad.dpad.up.isPressed;
                    case Button.Dpad_Down:
                        return gamepad.dpad.down.isPressed;
                    case Button.Dpad_Left:
                        return gamepad.dpad.left.isPressed;
                    case Button.Dpad_Right:
                        return gamepad.dpad.right.isPressed;
                    case Button.LeftTrigger:
                        return gamepad.leftTrigger.isPressed;
                    case Button.RightTrigger:
                        return gamepad.rightTrigger.isPressed;
                }
            } else
#endif
            {
                switch (button) {
                    case Button.A:
                        return gamepad.aButton.isPressed;
                    case Button.B:
                        return gamepad.bButton.isPressed;
                    case Button.Y:
                        return gamepad.yButton.isPressed;
                    case Button.X:
                        return gamepad.xButton.isPressed;
                    case Button.RightShoulder:
                        return gamepad.rightShoulder.isPressed;
                    case Button.LeftShoulder:
                        return gamepad.leftShoulder.isPressed;
                    case Button.RightStick:
                        return gamepad.rightStickButton.isPressed;
                    case Button.LeftStick:
                        return gamepad.leftStickButton.isPressed;
                    case Button.Back:
                        return gamepad.selectButton.isPressed;
                    case Button.Start:
                        return gamepad.startButton.isPressed;
                    case Button.Dpad_Up:
                        return gamepad.dpad.up.isPressed;
                    case Button.Dpad_Down:
                        return gamepad.dpad.down.isPressed;
                    case Button.Dpad_Left:
                        return gamepad.dpad.left.isPressed;
                    case Button.Dpad_Right:
                        return gamepad.dpad.right.isPressed;
                    case Button.LeftTrigger:
                        return gamepad.leftTrigger.isPressed;
                    case Button.RightTrigger:
                        return gamepad.rightTrigger.isPressed;
                }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="controlIndex"></param>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static Vector2 GetAxis(Axis axis, Index controlIndex, bool raw = false) {
            Vector2 result = Vector3.zero;
            switch (axis) {
                case Axis.LeftStick:
                    if (gamepadArray[(int)controlIndex] != null && GetGamePadCount() >= 1) {
                        result = gamepadArray[(int)controlIndex].leftStick.ReadValue();
                    }
                    break;
                case Axis.RightStick:
                    if (gamepadArray[(int)controlIndex] != null && GetGamePadCount() >= 1) {
                        result = gamepadArray[(int)controlIndex].rightStick.ReadValue();
                    }
                    break;
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="controlIndex"></param>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static float GetTrigger(Trigger trigger, Index controlIndex, bool raw = false) {
            string axisName = "";
            switch (trigger) {
                case Trigger.LeftTrigger: {
                        int num = (int)controlIndex;
                        axisName = "TriggersL_" + num.ToString();
                        break;
                    }
                case Trigger.RightTrigger: {
                        int num = (int)controlIndex;
                        axisName = "TriggersR_" + num.ToString();
                        break;
                    }
            }
            float result = 0f;
            try {
                if (raw) {
                    result = ControlFreak2.CF2Input.GetAxisRaw(axisName);
                    result = result == 0 ? UnityEngine.Input.GetAxisRaw(axisName) : result;
                }
                else {
                    result = ControlFreak2.CF2Input.GetAxis(axisName);
                    result = result == 0 ? UnityEngine.Input.GetAxis(axisName) : result;
                }
                return result;
            } catch (Exception message) {
                UnityEngine.Debug.LogError(message);
                UnityEngine.Debug.LogWarning("Have you set up all axes correctly? \nThe easiest solution is to replace the InputManager.asset with version located in the GamepadInput package. \nWarning: do so will overwrite any existing input");
                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controlIndex"></param>
        /// <returns></returns>
        private static KeyCode GetKeycode(Button button, Index controlIndex) {
            switch (controlIndex) {
                case Index.One:
                    switch (button) {
                        case Button.A:
                            return KeyCode.Joystick1Button0;
                        case Button.B:
                            return KeyCode.Joystick1Button1;
                        case Button.X:
                            return KeyCode.Joystick1Button2;
                        case Button.Y:
                            return KeyCode.Joystick1Button3;
                        case Button.RightShoulder:
                            return KeyCode.Joystick1Button5;
                        case Button.LeftShoulder:
                            return KeyCode.Joystick1Button4;
                        case Button.Back:
                            return KeyCode.Joystick1Button6;
                        case Button.Start:
                            return KeyCode.Joystick1Button7;
                        case Button.LeftStick:
                            return KeyCode.Joystick1Button8;
                        case Button.RightStick:
                            return KeyCode.Joystick1Button9;
                    }
                    break;
                case Index.Two:
                    switch (button) {
                        case Button.A:
                            return KeyCode.Joystick2Button0;
                        case Button.B:
                            return KeyCode.Joystick2Button1;
                        case Button.X:
                            return KeyCode.Joystick2Button2;
                        case Button.Y:
                            return KeyCode.Joystick2Button3;
                        case Button.RightShoulder:
                            return KeyCode.Joystick2Button5;
                        case Button.LeftShoulder:
                            return KeyCode.Joystick2Button4;
                        case Button.Back:
                            return KeyCode.Joystick2Button6;
                        case Button.Start:
                            return KeyCode.Joystick2Button7;
                        case Button.LeftStick:
                            return KeyCode.Joystick2Button8;
                        case Button.RightStick:
                            return KeyCode.Joystick2Button9;
                    }
                    break;
                case Index.Three:
                    switch (button) {
                        case Button.A:
                            return KeyCode.Joystick3Button0;
                        case Button.B:
                            return KeyCode.Joystick3Button1;
                        case Button.X:
                            return KeyCode.Joystick3Button2;
                        case Button.Y:
                            return KeyCode.Joystick3Button3;
                        case Button.RightShoulder:
                            return KeyCode.Joystick3Button5;
                        case Button.LeftShoulder:
                            return KeyCode.Joystick3Button4;
                        case Button.Back:
                            return KeyCode.Joystick3Button6;
                        case Button.Start:
                            return KeyCode.Joystick3Button7;
                        case Button.LeftStick:
                            return KeyCode.Joystick3Button8;
                        case Button.RightStick:
                            return KeyCode.Joystick3Button9;
                    }
                    break;
                case Index.Four:
                    switch (button) {
                        case Button.A:
                            return KeyCode.Joystick4Button0;
                        case Button.B:
                            return KeyCode.Joystick4Button1;
                        case Button.X:
                            return KeyCode.Joystick4Button2;
                        case Button.Y:
                            return KeyCode.Joystick4Button3;
                        case Button.RightShoulder:
                            return KeyCode.Joystick4Button5;
                        case Button.LeftShoulder:
                            return KeyCode.Joystick4Button4;
                        case Button.Back:
                            return KeyCode.Joystick4Button6;
                        case Button.Start:
                            return KeyCode.Joystick4Button7;
                        case Button.LeftStick:
                            return KeyCode.Joystick4Button8;
                        case Button.RightStick:
                            return KeyCode.Joystick4Button9;
                    }
                    break;
            }
            return KeyCode.None;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlIndex"></param>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static GamepadState GetState(Index controlIndex, bool raw = false) {
            GamepadState gamepadState = new GamepadState();
            gamepadState.A = GetButton(Button.A, controlIndex);
            gamepadState.B = GetButton(Button.B, controlIndex);
            gamepadState.Y = GetButton(Button.Y, controlIndex);
            gamepadState.X = GetButton(Button.X, controlIndex);
            gamepadState.RightShoulder = GetButton(Button.RightShoulder, controlIndex);
            gamepadState.LeftShoulder = GetButton(Button.LeftShoulder, controlIndex);
            gamepadState.RightStick = GetButton(Button.RightStick, controlIndex);
            gamepadState.LeftStick = GetButton(Button.LeftStick, controlIndex);
            gamepadState.Start = GetButton(Button.Start, controlIndex);
            gamepadState.Back = GetButton(Button.Back, controlIndex);
            gamepadState.LeftStickAxis = GetAxis(Axis.LeftStick, controlIndex, raw);
            gamepadState.rightStickAxis = GetAxis(Axis.RightStick, controlIndex, raw);
            gamepadState.dPadAxis = GetAxis(Axis.Dpad, controlIndex, raw);
            gamepadState.Left = (gamepadState.dPadAxis.x < 0f);
            gamepadState.Right = (gamepadState.dPadAxis.x > 0f);
            gamepadState.Up = (gamepadState.dPadAxis.y > 0f);
            gamepadState.Down = (gamepadState.dPadAxis.y < 0f);
            gamepadState.LeftTrigger = GetTrigger(Trigger.LeftTrigger, controlIndex, raw);
            gamepadState.RightTrigger = GetTrigger(Trigger.RightTrigger, controlIndex, raw);
            return gamepadState;
        }
    }
}
