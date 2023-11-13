using GamepadInput;
using UnityEngine;
public class SmeltFishing_Input : SingletonCustom<SmeltFishing_Input>
{
	public bool IsPressButtonA(SmeltFishing_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : SmeltFishing_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
	}
	public bool IsHoldButtonA(SmeltFishing_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : SmeltFishing_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.A);
	}
	public bool IsPressButtonB(SmeltFishing_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : SmeltFishing_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B);
	}
	public bool IsPressButtonX(SmeltFishing_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : SmeltFishing_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.X);
	}
	public bool IsMove(SmeltFishing_Definition.ControlUser user)
	{
		return GetMoveMagnitude(user) > 0.01f;
	}
	public Vector3 GetMoveVector(SmeltFishing_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : SmeltFishing_Definition.ControlUser.Player1);
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		return new Vector3(axisInput.Stick_L.x, 0f, axisInput.Stick_L.y);
	}
	public float GetMoveMagnitude(SmeltFishing_Definition.ControlUser user)
	{
		return GetMoveVector(user).magnitude;
	}
	private bool IsPressButtonAForEditor(SmeltFishing_Definition.ControlUser user)
	{
		return UnityEngine.Input.GetKeyDown(KeyCode.E);
	}
	private bool IsHoldButtonAForEditor(SmeltFishing_Definition.ControlUser user)
	{
		return UnityEngine.Input.GetKey(KeyCode.E);
	}
	private bool IsPressButtonBForEditor(SmeltFishing_Definition.ControlUser user)
	{
		return UnityEngine.Input.GetKeyDown(KeyCode.Q);
	}
	private bool IsPressButtonXForEditor(SmeltFishing_Definition.ControlUser user)
	{
		return UnityEngine.Input.GetKeyDown(KeyCode.X);
	}
	private Vector3 GetMoveVectorForEditor(SmeltFishing_Definition.ControlUser user)
	{
		float num = 0f;
		float num2 = 0f;
		if (UnityEngine.Input.GetKey(KeyCode.A))
		{
			num -= 1f;
		}
		if (UnityEngine.Input.GetKey(KeyCode.D))
		{
			num += 1f;
		}
		if (UnityEngine.Input.GetKey(KeyCode.W))
		{
			num2 += 1f;
		}
		if (UnityEngine.Input.GetKey(KeyCode.S))
		{
			num2 -= 1f;
		}
		return new Vector3(num, 0f, num2).normalized;
	}
}
