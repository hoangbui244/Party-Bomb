using GamepadInput;
using UnityEngine;
public class SwordFight_ControllerManager : SingletonCustom<SwordFight_ControllerManager>
{
	private enum ButtonStateType
	{
		DOWN,
		HOLD,
		UP
	}
	private float[] attackButtonPressTime = new float[SwordFight_Define.MAX_GAME_PLAYER_NUM];
	public bool IsMove(int _no)
	{
		return GetMoveLength(_no) > 0.01f;
	}
	public bool IsVerticalSlash(int _no)
	{
		return IsSlashVerticalButton(_no, ButtonStateType.DOWN);
	}
	public bool IsHorizontalLeftSlash(int _no)
	{
		return IsSlashLeftHorizontalButton(_no, ButtonStateType.DOWN);
	}
	public bool IsHorizontalRightSlash(int _no)
	{
		return IsSlashRightHorizontalButton(_no, ButtonStateType.DOWN);
	}
	public bool IsDeffence_BtnDown(int _no)
	{
		return IsDeffenceButton(_no, ButtonStateType.DOWN);
	}
	public bool IsJump_BtnDown(int _no)
	{
		return IsJumpButton(_no, ButtonStateType.DOWN);
	}
	public bool IsDeffence_BtnHold(int _no)
	{
		return IsDeffenceButton(_no, ButtonStateType.HOLD);
	}
	public bool IsDeffence_BtnUp(int _no)
	{
		return IsDeffenceButton(_no, ButtonStateType.UP);
	}
	public bool IsDebugBtn(int _no)
	{
		return IsDebugButton(_no, ButtonStateType.DOWN);
	}
	private bool IsSlashVerticalButton(int _no, ButtonStateType _type)
	{
		Convert(ref _no);
		if (_type == ButtonStateType.HOLD)
		{
			attackButtonPressTime[_no] += Time.deltaTime;
		}
		else
		{
			attackButtonPressTime[_no] = 0f;
		}
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.X))
		{
			if (_type != 0 && _type != ButtonStateType.HOLD)
			{
				return _type == ButtonStateType.UP;
			}
			return true;
		}
		return false;
	}
	private bool IsSlashLeftHorizontalButton(int _no, ButtonStateType _type)
	{
		Convert(ref _no);
		if (_type == ButtonStateType.HOLD)
		{
			attackButtonPressTime[_no] += Time.deltaTime;
		}
		else
		{
			attackButtonPressTime[_no] = 0f;
		}
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
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
	private bool IsSlashRightHorizontalButton(int _no, ButtonStateType _type)
	{
		Convert(ref _no);
		if (_type == ButtonStateType.HOLD)
		{
			attackButtonPressTime[_no] += Time.deltaTime;
		}
		else
		{
			attackButtonPressTime[_no] = 0f;
		}
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
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
	private bool IsDeffenceButton(int _no, ButtonStateType _type)
	{
		Convert(ref _no);
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		switch (_type)
		{
		case ButtonStateType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Y);
		case ButtonStateType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.Y);
		default:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Y);
		}
	}
	private bool IsJumpButton(int _no, ButtonStateType _type)
	{
		Convert(ref _no);
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		switch (_type)
		{
		case ButtonStateType.DOWN:
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.B);
		case ButtonStateType.UP:
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.B);
		default:
			return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.B);
		}
	}
	private bool IsDebugButton(int _no, ButtonStateType _type)
	{
		Convert(ref _no);
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Start) || SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.Back))
		{
			return _type == ButtonStateType.DOWN;
		}
		return false;
	}
	public Vector3 GetMoveDir(int _no)
	{
		Convert(ref _no);
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		mVector3Zero = new Vector3(num, 0f, num2);
		if (SingletonCustom<GameSettingManager>.Instance.CameraDir != GameSettingManager.CameraDirType.VERTICAL)
		{
			mVector3Zero = CalcManager.PosRotation2D(mVector3Zero, CalcManager.mVector3Zero, -90f, CalcManager.AXIS.Y);
		}
		return mVector3Zero;
	}
	public float GetMoveLength(int _no)
	{
		return GetMoveDir(_no).magnitude;
	}
	public float GetAttackButtonPressTime(int _no)
	{
		Convert(ref _no);
		return attackButtonPressTime[_no];
	}
	private void Convert(ref int _no)
	{
		switch (_no)
		{
		case 0:
			_no = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).memberPlayerNoList[0];
			break;
		case 1:
			_no = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).memberPlayerNoList[0];
			break;
		}
	}
	public int Convert(int _no)
	{
		switch (_no)
		{
		case 0:
			return SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).memberPlayerNoList[0];
		case 1:
			return SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).memberPlayerNoList[0];
		default:
			return 0;
		}
	}
}
