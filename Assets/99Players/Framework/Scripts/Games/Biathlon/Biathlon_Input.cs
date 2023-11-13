using GamepadInput;
using UnityEngine;
public class Biathlon_Input : SingletonCustom<Biathlon_Input>
{
	public bool IsPressButtonA(Biathlon_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : Biathlon_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.A);
	}
	public bool IsPressDownButtonA(Biathlon_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : Biathlon_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
	}
	public bool IsPressDownButtonB(Biathlon_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : Biathlon_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B);
	}
	public bool IsPressDownButtonL(Biathlon_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : Biathlon_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftShoulder);
	}
	public bool IsPressDownButtonZL(Biathlon_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : Biathlon_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.LeftTrigger);
	}
	public bool IsPressDownButtonR(Biathlon_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : Biathlon_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightShoulder);
	}
	public bool IsPressDownButtonZR(Biathlon_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : Biathlon_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.RightTrigger);
	}
	public float GetAngular(Biathlon_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : Biathlon_Definition.ControlUser.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx).Stick_L.x;
	}
	public Vector2 GetMoveVector(Biathlon_Definition.ControlUser user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : Biathlon_Definition.ControlUser.Player1);
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		return new Vector2(axisInput.Stick_L.x, axisInput.Stick_L.y);
	}
	private bool IsPressButtonAForEditor(Biathlon_Definition.ControlUser user)
	{
		return UnityEngine.Input.GetKey(KeyCode.E);
	}
	private bool IsPressDownButtonAForEditor(Biathlon_Definition.ControlUser user)
	{
		return UnityEngine.Input.GetKeyDown(KeyCode.E);
	}
	private bool IsPressButtonBForEditor(Biathlon_Definition.ControlUser user)
	{
		return UnityEngine.Input.GetKeyDown(KeyCode.Q);
	}
	private float GetAngularForEditor(Biathlon_Definition.ControlUser user)
	{
		float num = 0f;
		if (UnityEngine.Input.GetKey(KeyCode.A))
		{
			num -= 1f;
		}
		if (UnityEngine.Input.GetKey(KeyCode.D))
		{
			num += 1f;
		}
		return num;
	}
	private Vector2 GetMoveVectorForEditor(Biathlon_Definition.ControlUser user)
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
		return new Vector3(num, num2).normalized;
	}
}
