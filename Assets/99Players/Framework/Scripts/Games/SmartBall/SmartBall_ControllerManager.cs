using GamepadInput;
using UnityEngine;
public class SmartBall_ControllerManager : SingletonCustom<SmartBall_ControllerManager>
{
	public enum ButtonStateType
	{
		DOWN,
		HOLD,
		UP
	}
	public bool IsStickLeft(int _no)
	{
		return GetStickDir(_no).x < -0.8f;
	}
	public bool IsStickRight(int _no)
	{
		return GetStickDir(_no).x > 0.8f;
	}
	public bool IsStickUp(int _no)
	{
		return GetStickDir(_no).y > 0.8f;
	}
	public bool IsStickDown(int _no)
	{
		return GetStickDir(_no).y < -0.8f;
	}
	public bool IsButton_A(int _no, ButtonStateType _type)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		switch (_type)
		{
		case ButtonStateType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
		case ButtonStateType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.A);
		case ButtonStateType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.A);
		default:
			if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A))
			{
				if (_type != 0 && _type != ButtonStateType.HOLD)
				{
					return _type == ButtonStateType.UP;
				}
				return true;
			}
			return false;
		}
	}
	public bool IsButton_X(int _no, ButtonStateType _type)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		switch (_type)
		{
		case ButtonStateType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.X);
		case ButtonStateType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.X);
		case ButtonStateType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.X);
		default:
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.X))
			{
				if (_type != 0 && _type != ButtonStateType.HOLD)
				{
					return _type == ButtonStateType.UP;
				}
				return true;
			}
			return false;
		}
	}
	public bool IsButton_B(int _no, ButtonStateType _type)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		switch (_type)
		{
		case ButtonStateType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B);
		case ButtonStateType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.B);
		case ButtonStateType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.B);
		default:
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.B))
			{
				if (_type != 0 && _type != ButtonStateType.HOLD)
				{
					return _type == ButtonStateType.UP;
				}
				return true;
			}
			return false;
		}
	}
	public bool IsButton_Y(int _no, ButtonStateType _type)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		switch (_type)
		{
		case ButtonStateType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Y);
		case ButtonStateType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Y);
		case ButtonStateType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.Y);
		default:
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Y))
			{
				if (_type != 0 && _type != ButtonStateType.HOLD)
				{
					return _type == ButtonStateType.UP;
				}
				return true;
			}
			return false;
		}
	}
	public bool IsButton_R(int _no, ButtonStateType _type)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		switch (_type)
		{
		case ButtonStateType.DOWN:
			if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightTrigger);
			}
			return true;
		case ButtonStateType.HOLD:
			if (!SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightTrigger);
			}
			return true;
		case ButtonStateType.UP:
			if (!SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.RightShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.RightTrigger);
			}
			return true;
		default:
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightShoulder) || SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightTrigger))
			{
				if (_type != 0 && _type != ButtonStateType.HOLD)
				{
					return _type == ButtonStateType.UP;
				}
				return true;
			}
			return false;
		}
	}
	public bool IsButton_L(int _no, ButtonStateType _type)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		switch (_type)
		{
		case ButtonStateType.DOWN:
			if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftTrigger);
			}
			return true;
		case ButtonStateType.HOLD:
			if (!SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.LeftShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.LeftTrigger);
			}
			return true;
		case ButtonStateType.UP:
			if (!SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.LeftShoulder))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.LeftTrigger);
			}
			return true;
		default:
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.LeftShoulder) || SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.LeftTrigger))
			{
				if (_type != 0 && _type != ButtonStateType.HOLD)
				{
					return _type == ButtonStateType.UP;
				}
				return true;
			}
			return false;
		}
	}
	public bool IsButton_Stick(int _no, ButtonStateType _type)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		switch (_type)
		{
		case ButtonStateType.DOWN:
			if (!SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftStick))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightStick);
			}
			return true;
		case ButtonStateType.HOLD:
			if (!SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.LeftStick))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightStick);
			}
			return true;
		case ButtonStateType.UP:
			if (!SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.LeftStick))
			{
				return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.RightStick);
			}
			return true;
		default:
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.LeftStick) || SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightStick))
			{
				if (_type != 0 && _type != ButtonStateType.HOLD)
				{
					return _type == ButtonStateType.UP;
				}
				return true;
			}
			return false;
		}
	}
	public bool IsButton_Plus(int _no, ButtonStateType _type)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		switch (_type)
		{
		case ButtonStateType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Start);
		case ButtonStateType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Start);
		case ButtonStateType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.Start);
		default:
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Start))
			{
				if (_type != 0 && _type != ButtonStateType.HOLD)
				{
					return _type == ButtonStateType.UP;
				}
				return true;
			}
			return false;
		}
	}
	public bool IsButton_Minus(int _no, ButtonStateType _type)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		switch (_type)
		{
		case ButtonStateType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Back);
		case ButtonStateType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Back);
		case ButtonStateType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.Back);
		default:
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Back))
			{
				if (_type != 0 && _type != ButtonStateType.HOLD)
				{
					return _type == ButtonStateType.UP;
				}
				return true;
			}
			return false;
		}
	}
	public Vector3 GetStickDir(int _no)
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		return new Vector3(num, num2, 0f);
	}
}
