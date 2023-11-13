using ControlFreak2;
using GamepadInput;
using System;
namespace io.ninenine.players.party3d.games.common {
    public class TouchPanelManager : SingletonCustom<TouchPanelManager> {
        public TouchControlPanel TouchControlPanel;
        public TouchJoystick TouchLJoystick;
        public TouchButton TouchButtonA;
        public TouchButton TouchButtonB;
        public TouchButton TouchButtonX;
        public TouchButton TouchButtonY;
        #region TouchControlPanel
        public void SetTouchPanelEnable(bool enable) {
// Always false if it not on mobile
#if !UNITY_ANDROID && !UNITY_IOS
        enable = false;
#endif
            TouchControlPanel.gameObject.SetActive(enable);
        }
        public void SetTouchControlEnable(SatGamePad.Button button, bool enable) {
// Always false if it not on mobile
#if !UNITY_ANDROID && !UNITY_IOS
        enable = false;
#endif
            switch (button) {
                case SatGamePad.Button.A:
                    TouchButtonA.gameObject.SetActive(enable);
                    break;
                case SatGamePad.Button.B:
                    TouchButtonB.gameObject.SetActive(enable);
                    break;
                case SatGamePad.Button.Y:
                    TouchButtonY.gameObject.SetActive(enable);
                    break;
                case SatGamePad.Button.X:
                    TouchButtonX.gameObject.SetActive(enable);
                    break;
                //case SatGamePad.Button.RightShoulder:
                //    break;
                //case SatGamePad.Button.LeftShoulder:
                //    break;
                //case SatGamePad.Button.RightStick:
                //    break;
                case SatGamePad.Button.LeftStick:
                    TouchLJoystick.gameObject.SetActive(enable);
                    break;
                //case SatGamePad.Button.Back:
                //    break;
                //case SatGamePad.Button.Start:
                //    break;
                //case SatGamePad.Button.Dpad_Up:
                //    break;
                //case SatGamePad.Button.Dpad_Down:
                //    break;
                //case SatGamePad.Button.Dpad_Left:
                //    break;
                //case SatGamePad.Button.Dpad_Right:
                //    break;
                //case SatGamePad.Button.LeftTrigger:
                //    break;
                //case SatGamePad.Button.RightTrigger:
                //    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
        }
        public void SetAllTouchControlEnable(bool enable) {
// Always false if it not on mobile
#if !UNITY_ANDROID && !UNITY_IOS
        enable = false;
#endif
            TouchButtonA.gameObject.SetActive(enable);
            TouchButtonB.gameObject.SetActive(enable);
            TouchButtonY.gameObject.SetActive(enable);
            TouchButtonX.gameObject.SetActive(enable);
            TouchLJoystick.gameObject.SetActive(enable);
        }
        #endregion
    }
}