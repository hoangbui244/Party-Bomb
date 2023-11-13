using GamepadInput;
using UnityEngine;
public class ShortTrack_ControllerManeger : SingletonCustom<ShortTrack_ControllerManeger>
{
	private enum InputButtonType
	{
		Button_A,
		Button_B,
		None
	}
	[NonReorderable]
	private InputButtonType[] inputBtnType = new InputButtonType[4];
	public void Init()
	{
		for (int i = 0; i < inputBtnType.Length; i++)
		{
			inputBtnType[i] = InputButtonType.Button_B;
		}
	}
	public void Init_PushButton(int _no)
	{
		inputBtnType[_no] = InputButtonType.Button_B;
	}
	public bool IsPushButton_A(int _no)
	{
		if (inputBtnType[_no] == InputButtonType.None)
		{
			if (PushButton_A(_no))
			{
				inputBtnType[_no] = InputButtonType.Button_A;
				return true;
			}
			return false;
		}
		if (PushButton_A(_no))
		{
			if (inputBtnType[_no] == InputButtonType.Button_B)
			{
				inputBtnType[_no] = InputButtonType.Button_A;
				return true;
			}
			return false;
		}
		return false;
	}
	public bool IsPushButton_B(int _no)
	{
		if (inputBtnType[_no] == InputButtonType.None)
		{
			if (PushButton_B(_no))
			{
				inputBtnType[_no] = InputButtonType.Button_B;
				return true;
			}
			return false;
		}
		if (PushButton_B(_no))
		{
			if (inputBtnType[_no] == InputButtonType.Button_A)
			{
				inputBtnType[_no] = InputButtonType.Button_B;
				return true;
			}
			return false;
		}
		return false;
	}
	public bool PushButton_A(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
	}
	public bool PushButtonUP_A(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.A);
	}
	private bool PushButton_B(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B);
	}
	public bool AnyKey(int _no)
	{
		SingletonCustom<JoyConManager>.Instance.IsSingleMode();
		return SingletonCustom<JoyConManager>.Instance.GetAnyButtonDown();
	}
	public bool IsMove(int _no)
	{
		return GetMoveLength(_no) > 0.01f;
	}
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
}
