using GamepadInput;
using UnityEngine;
public class Hidden_ControllerManager : SingletonCustom<Hidden_ControllerManager>
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
	public float GetMoveLength(int _no)
	{
		return GetMoveDir(_no).magnitude;
	}
	public int GetMoveTypeNum(int _no)
	{
		int num = 0;
		float moveLength = GetMoveLength(_no);
		if (moveLength > 0.8f)
		{
			num = 2;
		}
		else if (moveLength > 0.1f)
		{
			num = 1;
		}
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			if (num != 0 && SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.A))
			{
				num = 3;
			}
		}
		else if (num != 0 && SingletonCustom<JoyConManager>.Instance.GetButton(_no, SatGamePad.Button.A))
		{
			num = 3;
		}
		return num;
	}
	public bool GetTouchButtonDown(int _no)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
		}
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.A);
	}
	public bool GetJumpButtonDown(int _no)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B);
		}
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.B);
	}
	public bool GetAttackDodgeButtonDown(int _no)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			if (!SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightTrigger);
			}
			return true;
		}
		if (!SingletonCustom<JoyConManager>.Instance.GetButton(_no, SatGamePad.Button.RightShoulder))
		{
			return SingletonCustom<JoyConManager>.Instance.GetButton(_no, SatGamePad.Button.RightTrigger);
		}
		return true;
	}
	public bool GetActionButtonDown(int _no)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.X);
		}
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.X);
	}
	public bool CheckNpadStyleLorR(int _no)
	{
		return false;
	}
	public bool GetDebugCameraButtonDown(int _no)
	{
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			int playerIdx = 0;
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightStick);
		}
		if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.RightStick))
		{
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(_no, SatGamePad.Button.LeftStick);
		}
		return true;
	}
}
