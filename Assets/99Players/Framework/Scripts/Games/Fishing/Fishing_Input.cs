using GamepadInput;
using UnityEngine;
public class Fishing_Input : SingletonCustom<Fishing_Input>
{
	private enum ButtonStateType
	{
		DOWN,
		HOLD,
		UP
	}
	private float[] attackButtonPressTime = new float[4];
	public bool IsMove(FishingDefinition.User user)
	{
		return GetMoveLength(user) > 0.01f;
	}
	public bool IsPushButton_A(FishingDefinition.User user)
	{
		return ProcessPushButton_A(user, ButtonStateType.DOWN);
	}
	private bool ProcessPushButton_A(FishingDefinition.User user, ButtonStateType _type)
	{
		if (_type == ButtonStateType.HOLD)
		{
			attackButtonPressTime[(int)user] += Time.deltaTime;
		}
		else
		{
			attackButtonPressTime[(int)user] = 0f;
		}
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : FishingDefinition.User.Player1);
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
	public Vector3 GetMoveDir(FishingDefinition.User user)
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : FishingDefinition.User.Player1);
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		return new Vector3(num, 0f, num2);
	}
	public float GetMoveLength(FishingDefinition.User user)
	{
		return GetMoveDir(user).magnitude;
	}
}
