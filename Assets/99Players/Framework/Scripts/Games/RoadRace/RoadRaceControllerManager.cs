using GamepadInput;
using UnityEngine;
public class RoadRaceControllerManager : SingletonCustom<RoadRaceControllerManager>
{
	private static readonly float STICK_MOVE_MAX = 85f;
	private static readonly float FINGER_MOVE_MAX = 85f;
	public void Init()
	{
	}
	public void UpdateMethod()
	{
	}
	public Vector2 GetStickDir(int _no)
	{
		float num = 0f;
		float num2 = 0f;
		Vector2 vector = CalcManager.mVector3Zero;
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		if (true && Mathf.Abs(num) < 0.2f && Mathf.Abs(num2) < 0.2f)
		{
			num = 0f;
			num2 = 0f;
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Right))
			{
				num = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Left))
			{
				num = -1f;
			}
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Up))
			{
				num2 = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Down))
			{
				num2 = -1f;
			}
		}
		vector = new Vector2(num, num2);
		if (vector.sqrMagnitude < 0.0400000028f)
		{
			return Vector2.zero;
		}
		return vector.normalized;
	}
	public bool IsAccelBtn(int _no)
	{
		return SingletonCustom<JoyConManager>.Instance.GetButton(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(_no), SatGamePad.Button.A);
	}
	public bool IsActionBtnDown(int _no)
	{
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(_no), SatGamePad.Button.B);
	}
	public bool IsLookRearBtn(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.X);
	}
	public bool IsCameraAngleRightBtnDown(int _no)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightTrigger);
			}
			return true;
		}
		if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.RightShoulder))
		{
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.RightTrigger);
		}
		return true;
	}
	public bool IsCameraAngleLeftBtnDown(int _no)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftTrigger);
			}
			return true;
		}
		if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.LeftShoulder))
		{
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.LeftTrigger);
		}
		return true;
	}
}
