using GamepadInput;
using UnityEngine;
public class MakingPotion_Controller
{
	public const float NEUTRAL_STICK_MAGNITUDE = 0.5f;
	public static Vector2 GetStickDir(int _no)
	{
		float num = 0f;
		float num2 = 0f;
		Vector2 vector = CalcManager.mVector3Zero;
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		vector = new Vector2(num, num2);
		if (vector.sqrMagnitude < 0.25f)
		{
			return Vector2.zero;
		}
		return vector.normalized;
	}
	public static Vector2 GetEditorStickDir(Camera _camera, Vector3 _centerWorldPos)
	{
		if (!Input.GetMouseButton(0))
		{
			return Vector2.zero;
		}
		return ((Vector2)(UnityEngine.Input.mousePosition - _camera.WorldToScreenPoint(_centerWorldPos))).normalized;
	}
	public static bool GetRedButtonDown(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B);
	}
	public static bool GetRedButton(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.B);
	}
	public static bool GetRedButtonUp(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.B);
	}
	public static bool GetYellowButtonDown(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
	}
	public static bool GetYellowButton(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.A);
	}
	public static bool GetYellowButtonUp(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.A);
	}
	public static bool GetGreenButtonDown(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.X);
	}
	public static bool GetGreenButton(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.X);
	}
	public static bool GetGreenButtonUp(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.X);
	}
	public static bool GetBlueButtonDown(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Y);
	}
	public static bool GetBlueButton(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Y);
	}
	public static bool GetBlueButtonUp(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.Y);
	}
}
