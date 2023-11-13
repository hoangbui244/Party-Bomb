using GamepadInput;
using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_Input : SingletonMonoBehaviour<Shuriken_Input>
{
	public bool IsPressDownButtonA(Shuriken_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : Shuriken_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
	}
	public Vector2 GetLStickVector(Shuriken_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : Shuriken_Definition.ControlUser.Player1);
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		return new Vector2(axisInput.Stick_L.x, axisInput.Stick_L.y);
	}
}
