using GamepadInput;
using UnityEngine;
public class FindMask_CharacterController : MonoBehaviour
{
	private const float NEUTRAL_STICK_MAGNITUDE = 0.2f;
	public void UpdateMethod()
	{
		if (SingletonCustom<FindMask_GameManager>.Instance.State == FindMask_GameManager.STATE.CHARA_MOVE)
		{
			CursorMove();
			if (GetDecisionMaskButtonDown(SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo) && !SingletonCustom<FindMask_MaskManager>.Instance.IsSecondSelect && GetSelectMaskeObjNum() >= 0)
			{
				SingletonCustom<FindMask_MaskManager>.Instance.SelectMask(GetSelectMaskeObjNum());
			}
		}
	}
	private void CursorMove()
	{
		Vector3 stickDir = GetStickDir(SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo);
		SingletonCustom<FindMask_CharacterManager>.Instance.CursorMove(stickDir);
	}
	public int GetSelectMaskeObjNum()
	{
		return SingletonCustom<FindMask_CharacterManager>.Instance.GetSelectMaskeObjNum();
	}
	public FindMask_MaskData GetSelectMaskeObj()
	{
		return SingletonCustom<FindMask_CharacterManager>.Instance.GetSelectMaskeObj();
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
		mVector3Zero = new Vector3(num, num2, 0f);
		if (mVector3Zero.sqrMagnitude < 0.0400000028f)
		{
			mVector3Zero = Vector3.zero;
		}
		return mVector3Zero;
	}
	public bool GetDecisionMaskButtonDown(int _no)
	{
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
	}
}
