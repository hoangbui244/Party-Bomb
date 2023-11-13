using GamepadInput;
public class FlyingSquirrelRace_Input : SingletonCustom<FlyingSquirrelRace_Input>
{
	public bool IsHoldButtonA(FlyingSquirrelRace_Definition.Controller user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : FlyingSquirrelRace_Definition.Controller.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.A);
	}
	public bool IsDownButtonA(FlyingSquirrelRace_Definition.Controller user)
	{
		int playerIdx = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? user : FlyingSquirrelRace_Definition.Controller.Player1);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
	}
}
