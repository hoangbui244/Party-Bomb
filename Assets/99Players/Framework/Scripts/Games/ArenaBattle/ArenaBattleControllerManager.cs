using GamepadInput;
using UnityEngine;
public class ArenaBattleControllerManager : MonoBehaviour
{
	public static Vector2 GetStickDir(int _no)
	{
		float num = 0f;
		float num2 = 0f;
		Vector2 vector = CalcManager.mVector3Zero;
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		if (true && Mathf.Abs(num) < 0.2f && Mathf.Abs(num2) < 0.2f)
		{
			num = 0f;
			num2 = 0f;
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Right))
			{
				num = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Left))
			{
				num = -1f;
			}
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Up))
			{
				num2 = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Down))
			{
				num2 = -1f;
			}
		}
		vector = new Vector2(num, num2);
		if (vector.sqrMagnitude < 0.0400000028f)
		{
			return Vector2.zero;
		}
		return vector.normalized;
	}
}
