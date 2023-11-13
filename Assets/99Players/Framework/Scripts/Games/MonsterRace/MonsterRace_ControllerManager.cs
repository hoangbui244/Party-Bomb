using GamepadInput;
using UnityEngine;
public class MonsterRace_ControllerManager : SingletonCustom<MonsterRace_ControllerManager>
{
	public Vector3 GetMoveDir(int _no)
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		return new Vector3(num, 0f, num2);
	}
	public bool GetAccelButton(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.A);
	}
	public bool GetBackButton(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.X);
	}
	public bool GetLookRearButton(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Y);
	}
	public bool GetJumpButton(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.B);
	}
	public bool GetJumpButtonDown(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B);
	}
	public bool GetDashButton(int _no)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			if (!SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightTrigger);
			}
			return true;
		}
		if (!SingletonCustom<JoyConManager>.Instance.GetButton(_no, SatGamePad.Button.RightShoulder))
		{
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.RightTrigger);
		}
		return true;
	}
	public bool GetDashButtonDown(int _no)
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
	public bool GetCameraAngleRightButton(int _no)
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
	public bool GetCameraAngleLeftButton(int _no)
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
	public bool GetDebugButtonXDown(int _no)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.X);
		}
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.X);
	}
	public bool GetDebugButtonYDown(int _no)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Y);
		}
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.Y);
	}
}
