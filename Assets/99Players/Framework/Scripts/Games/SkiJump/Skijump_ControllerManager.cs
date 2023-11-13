using GamepadInput;
using UnityEngine;
public class Skijump_ControllerManager : SingletonCustom<Skijump_ControllerManager>
{
	public enum ButtonPushType
	{
		DOWN,
		HOLD,
		UP
	}
	public bool IsPushButton_A(Skijump_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_A(_userType, _buttonPushType);
	}
	public bool IsPushButton_X(Skijump_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_X(_userType, _buttonPushType);
	}
	public bool IsPushButton_Y(Skijump_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_Y(_userType, _buttonPushType);
	}
	public bool IsPushButton_StickR(Skijump_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_StickR(_userType, _buttonPushType);
	}
	public bool IsPushButton_StickL(Skijump_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_StickL(_userType, _buttonPushType);
	}
	public bool IsStickTilt(Skijump_Define.UserType _userType)
	{
		return GetStickTilt(_userType) > 0.01f;
	}
	private bool ProcessPushButton_A(Skijump_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : Skijump_Define.UserType.PLAYER_1);
		switch (_type)
		{
		case ButtonPushType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
		case ButtonPushType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.A);
		case ButtonPushType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.A);
		default:
			return false;
		}
	}
	private bool ProcessPushButton_X(Skijump_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : Skijump_Define.UserType.PLAYER_1);
		switch (_type)
		{
		case ButtonPushType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.X);
		case ButtonPushType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.X);
		case ButtonPushType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.X);
		default:
			return false;
		}
	}
	private bool ProcessPushButton_Y(Skijump_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : Skijump_Define.UserType.PLAYER_1);
		switch (_type)
		{
		case ButtonPushType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Y);
		case ButtonPushType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Y);
		case ButtonPushType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.Y);
		default:
			return false;
		}
	}
	public Vector3 GetRStickDir(Skijump_Define.UserType _userType)
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : Skijump_Define.UserType.PLAYER_1);
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_R.x;
		num2 = axisInput.Stick_R.y;
		return new Vector3(num, 0f, num2);
	}
	public Vector3 GetLStickDir(Skijump_Define.UserType _userType)
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : Skijump_Define.UserType.PLAYER_1);
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		return new Vector3(num, 0f, num2);
	}
	private bool ProcessPushButton_StickR(Skijump_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : Skijump_Define.UserType.PLAYER_1);
		switch (_type)
		{
		case ButtonPushType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightStick);
		case ButtonPushType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightStick);
		case ButtonPushType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.RightStick);
		default:
			return false;
		}
	}
	private bool ProcessPushButton_StickL(Skijump_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : Skijump_Define.UserType.PLAYER_1);
		switch (_type)
		{
		case ButtonPushType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftStick);
		case ButtonPushType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.LeftStick);
		case ButtonPushType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.LeftStick);
		default:
			return false;
		}
	}
	public float GetStickTilt(Skijump_Define.UserType _userType)
	{
		return GetLStickDir(_userType).magnitude;
	}
}
