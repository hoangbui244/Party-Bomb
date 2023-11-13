using GamepadInput;
using UnityEngine;
namespace io.ninenine.players.party3d.games.common {
    /// <summary>
    /// 
    /// </summary>
    public class BlockWiperControllerManager : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_no"></param>
        /// <returns></returns>
        public static Vector2 GetStickDir(int _no) {
            float xAxis = 0f;
            float yAxis = 0f;
            int playerIdx = JoyConManager.Instance.IsSingleMode() ? 0 : _no;
            playerIdx = JoyConManager.Instance.GetCurrentNpadId(playerIdx);
            // Axis input (stick)
            JoyConManager.AXIS_INPUT axisInput = JoyConManager.Instance.GetAxisInput(playerIdx);
            xAxis = axisInput.Stick_L.x;
            yAxis = axisInput.Stick_L.y;
            // TODO: Disable dpad input as currently it make controller stick also effect keyboard somehow
            // Axis input (dpad)
            if (Mathf.Abs(xAxis) < 0.2f && Mathf.Abs(yAxis) < 0.2f) {
                xAxis = 0f;
                yAxis = 0f;
                if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Right)) {
                    xAxis = 1f;
                } else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Left)) {
                    xAxis = -1f;
                }
                if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Up)) {
                    yAxis = 1f;
                } else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Down)) {
                    yAxis = -1f;
                }
            }
            // Return results
            Vector2 outputAxis = new Vector2(xAxis, yAxis);
            if (outputAxis.sqrMagnitude < 0.0400000028f) {
                return Vector2.zero;
            }
            return outputAxis.normalized;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_no"></param>
        /// <param name="_button"></param>
        /// <returns></returns>
        public static bool GetButtonDown(int _no, SatGamePad.Button _button) {
            return SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(_no), _button);
        }
    }
}