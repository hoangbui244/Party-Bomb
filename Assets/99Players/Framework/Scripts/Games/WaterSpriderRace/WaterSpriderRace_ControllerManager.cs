using GamepadInput;
using UnityEngine;
public class WaterSpriderRace_ControllerManager : SingletonCustom<WaterSpriderRace_ControllerManager>
{
	public enum ButtonPushType
	{
		DOWN,
		HOLD,
		UP
	}
	public enum StickType
	{
		R,
		L
	}
	public enum StickDirType
	{
		UP,
		RIGHT,
		LEFT,
		DOWN
	}
	public enum CrossKeyType
	{
		UP,
		RIGHT,
		LEFT,
		DOWN
	}
	private bool isHoldInputIntervalMode;
	private const int MAX_PLAYER_NUM = 4;
	private float[] intervalInputHold_LStick = new float[4];
	private float[] intervalInputHold_RStick = new float[4];
	private float[] intervalInputHold_CrossKey = new float[4];
	private const float HOLD_INPUT_INTERVAL = 0.2f;
	public bool IsPushButton_A(WaterSpriderRace_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_A(_userType, _buttonPushType);
	}
	public bool IsPushButton_B(WaterSpriderRace_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_B(_userType, _buttonPushType);
	}
	public bool IsPushButton_X(WaterSpriderRace_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_X(_userType, _buttonPushType);
	}
	public bool IsPushButton_Y(WaterSpriderRace_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_Y(_userType, _buttonPushType);
	}
	public bool IsPushButton_L(WaterSpriderRace_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_L(_userType, _buttonPushType);
	}
	public bool IsPushButton_ZL(WaterSpriderRace_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_ZL(_userType, _buttonPushType);
	}
	public bool IsPushButton_R(WaterSpriderRace_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_R(_userType, _buttonPushType);
	}
	public bool IsPushButton_ZR(WaterSpriderRace_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_ZR(_userType, _buttonPushType);
	}
	public bool IsPushCrossKey(WaterSpriderRace_Define.UserType _userType, CrossKeyType _keyType, ButtonPushType _buttonPushType)
	{
		if (_buttonPushType == ButtonPushType.HOLD && isHoldInputIntervalMode && ProcessCrossKey(_userType, _keyType, _buttonPushType))
		{
			if (IsReturnInputHold_CrossKey(_userType))
			{
				ContinueInputHold_CrossKey(_userType);
				return true;
			}
			return false;
		}
		if (ProcessCrossKey(_userType, _keyType, _buttonPushType))
		{
			ContinueInputHold_CrossKey(_userType);
			return true;
		}
		return false;
	}
	public bool IsStickTiltDirection(WaterSpriderRace_Define.UserType _userType, StickType _stickType, StickDirType _stickDirType, float _stickNeutralValue = 0.5f)
	{
		Vector3 zero = Vector3.zero;
		zero = ((_stickType != 0) ? GetStickDir_L(_userType) : GetStickDir_R(_userType));
		if ((_stickDirType == StickDirType.UP && zero.z > _stickNeutralValue) || (_stickDirType == StickDirType.RIGHT && zero.x > _stickNeutralValue) || (_stickDirType == StickDirType.LEFT && zero.x < 0f - _stickNeutralValue) || (_stickDirType == StickDirType.DOWN && zero.z < 0f - _stickNeutralValue))
		{
			if (isHoldInputIntervalMode)
			{
				if ((_stickType == StickType.R) ? IsReturnInputHold_Stick_R(_userType) : IsReturnInputHold_Stick_L(_userType))
				{
					if (_stickType == StickType.R)
					{
						ContinueInputHold_Stick_R(_userType);
					}
					else
					{
						ContinueInputHold_Stick_L(_userType);
					}
					return true;
				}
				return false;
			}
			return true;
		}
		return false;
	}
	public bool IsPushButton_StickR(WaterSpriderRace_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_StickR(_userType, _buttonPushType);
	}
	public bool IsPushButton_StickL(WaterSpriderRace_Define.UserType _userType, ButtonPushType _buttonPushType)
	{
		return ProcessPushButton_StickL(_userType, _buttonPushType);
	}
	public bool IsStickTilt(WaterSpriderRace_Define.UserType _userType, StickType _stickType)
	{
		return GetStickTilt(_userType, _stickType) > 0.01f;
	}
	private bool ProcessPushButton_A(WaterSpriderRace_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
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
	private bool ProcessPushButton_B(WaterSpriderRace_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
		switch (_type)
		{
		case ButtonPushType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B);
		case ButtonPushType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.B);
		case ButtonPushType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.B);
		default:
			return false;
		}
	}
	private bool ProcessPushButton_X(WaterSpriderRace_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
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
	private bool ProcessPushButton_Y(WaterSpriderRace_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
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
	private bool ProcessPushButton_L(WaterSpriderRace_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
		switch (_type)
		{
		case ButtonPushType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftShoulder);
		case ButtonPushType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.LeftShoulder);
		case ButtonPushType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.LeftShoulder);
		default:
			return false;
		}
	}
	private bool ProcessPushButton_ZL(WaterSpriderRace_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
		switch (_type)
		{
		case ButtonPushType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftTrigger);
		case ButtonPushType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.LeftTrigger);
		case ButtonPushType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.LeftTrigger);
		default:
			return false;
		}
	}
	private bool ProcessPushButton_R(WaterSpriderRace_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
		switch (_type)
		{
		case ButtonPushType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightShoulder);
		case ButtonPushType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightShoulder);
		case ButtonPushType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.RightShoulder);
		default:
			return false;
		}
	}
	private bool ProcessPushButton_ZR(WaterSpriderRace_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
		switch (_type)
		{
		case ButtonPushType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightTrigger);
		case ButtonPushType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.RightTrigger);
		case ButtonPushType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.RightTrigger);
		default:
			return false;
		}
	}
	public Vector3 GetStickDir_R(WaterSpriderRace_Define.UserType _userType)
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_R.x;
		num2 = axisInput.Stick_R.y;
		return new Vector3(num, 0f, num2);
	}
	public Vector3 GetStickDir_L(WaterSpriderRace_Define.UserType _userType)
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		return new Vector3(num, 0f, num2);
	}
	private bool ProcessPushButton_StickR(WaterSpriderRace_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
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
	private bool ProcessPushButton_StickL(WaterSpriderRace_Define.UserType _userType, ButtonPushType _type)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
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
	public float GetStickTilt(WaterSpriderRace_Define.UserType _userType, StickType _stickType)
	{
		if (_stickType == StickType.L)
		{
			return GetStickDir_L(_userType).magnitude;
		}
		return GetStickDir_R(_userType).magnitude;
	}
	private bool ProcessCrossKey(WaterSpriderRace_Define.UserType _userType, CrossKeyType _keyType, ButtonPushType _type)
	{
		SatGamePad.Button button = SatGamePad.Button.A;
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _userType : WaterSpriderRace_Define.UserType.PLAYER_1);
		switch (_keyType)
		{
		case CrossKeyType.UP:
			button = SatGamePad.Button.Dpad_Up;
			break;
		case CrossKeyType.RIGHT:
			button = SatGamePad.Button.Dpad_Right;
			break;
		case CrossKeyType.LEFT:
			button = SatGamePad.Button.Dpad_Left;
			break;
		case CrossKeyType.DOWN:
			button = SatGamePad.Button.Dpad_Down;
			break;
		}
		switch (_type)
		{
		case ButtonPushType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, button);
		case ButtonPushType.HOLD:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, button);
		case ButtonPushType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, button);
		default:
			return false;
		}
	}
	public void SetttingHoldInputIntervalMode()
	{
		isHoldInputIntervalMode = true;
	}
	public void ReleaseHoldInputIntervalMode()
	{
		isHoldInputIntervalMode = false;
	}
	private void Update()
	{
		for (int i = 0; i < 4; i++)
		{
			if (intervalInputHold_LStick[i] > 0f)
			{
				intervalInputHold_LStick[i] -= Time.deltaTime;
			}
			if (intervalInputHold_RStick[i] > 0f)
			{
				intervalInputHold_RStick[i] -= Time.deltaTime;
			}
			if (intervalInputHold_CrossKey[i] > 0f)
			{
				intervalInputHold_CrossKey[i] -= Time.deltaTime;
			}
		}
	}
	private void ContinueInputHold_Stick_L(WaterSpriderRace_Define.UserType _userType)
	{
		intervalInputHold_LStick[(int)_userType] = 0.2f;
	}
	private void ContinueInputHold_Stick_R(WaterSpriderRace_Define.UserType _userType)
	{
		intervalInputHold_RStick[(int)_userType] = 0.2f;
	}
	private void ContinueInputHold_CrossKey(WaterSpriderRace_Define.UserType _userType)
	{
		intervalInputHold_CrossKey[(int)_userType] = 0.2f;
	}
	private void ResetInputHold_Stick_L(WaterSpriderRace_Define.UserType _userType)
	{
		intervalInputHold_LStick[(int)_userType] = 0f;
	}
	private void ResetInputHold_Stick_R(WaterSpriderRace_Define.UserType _userType)
	{
		intervalInputHold_RStick[(int)_userType] = 0f;
	}
	private bool IsReturnInputHold_Stick_L(WaterSpriderRace_Define.UserType _userType)
	{
		return intervalInputHold_LStick[(int)_userType] <= 0f;
	}
	private bool IsReturnInputHold_Stick_R(WaterSpriderRace_Define.UserType _userType)
	{
		return intervalInputHold_RStick[(int)_userType] <= 0f;
	}
	private bool IsReturnInputHold_CrossKey(WaterSpriderRace_Define.UserType _userType)
	{
		return intervalInputHold_CrossKey[(int)_userType] <= 0f;
	}
}
