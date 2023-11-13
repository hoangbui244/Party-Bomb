using UnityEngine;
public class Takoyaki_Player : MonoBehaviour
{
	[SerializeField]
	[Header("たこ焼き機(１人プレイ)")]
	private Takoyaki_TakoyakiMachine takoMachine_Single;
	[SerializeField]
	[Header("たこ焼き機(複数人プレイ)")]
	private Takoyaki_TakoyakiMachine takoMachine_Multi;
	[SerializeField]
	[Header("AI処理")]
	private Takoyaki_AI ai;
	[SerializeField]
	[Header("１人プレイ時の他の機材オブジェクト")]
	private GameObject otherTools_Single;
	[SerializeField]
	[Header("複数人プレイ時の他の機材オブジェクト")]
	private GameObject otherTools_Multi;
	private Takoyaki_Define.UserType userType;
	private Takoyaki_Define.TeamType teamType;
	private float stickHorizontal;
	private float stickVertical;
	private Takoyaki_TakoyakiMachine takoyakiMachine;
	public Takoyaki_Define.UserType UserType => userType;
	private Takoyaki_Define.TeamType TeamType => teamType;
	public void Init(Takoyaki_Define.UserType _userType, Takoyaki_Define.TeamType _teamType)
	{
		userType = _userType;
		teamType = _teamType;
		if (Takoyaki_Define.PLAYER_NUM == 1)
		{
			takoMachine_Single.gameObject.SetActive(value: true);
			takoMachine_Multi.gameObject.SetActive(value: false);
			otherTools_Single.SetActive(value: true);
			otherTools_Multi.SetActive(value: false);
			base.transform.SetLocalPosition(-0.0565f, 0.6f, 0f);
			takoyakiMachine = takoMachine_Single;
		}
		else
		{
			takoMachine_Single.gameObject.SetActive(value: false);
			takoMachine_Multi.gameObject.SetActive(value: true);
			otherTools_Single.SetActive(value: false);
			otherTools_Multi.SetActive(value: true);
			base.transform.SetLocalPosition(0.02f, 0.6f, -0.045f);
			takoyakiMachine = takoMachine_Multi;
		}
		takoyakiMachine.Init(this);
		ai.Init(this, takoyakiMachine);
	}
	public void UpdateMethod()
	{
		if (userType <= Takoyaki_Define.UserType.PLAYER_4)
		{
			if (Takoyaki_Define.CM.IsStickTiltDirection(userType, Takoyaki_ControllerManager.StickType.L, Takoyaki_ControllerManager.StickDirType.UP))
			{
				takoyakiMachine.MoveCursor(Takoyaki_TakoyakiMachine.MoveCursorDirection.UP);
			}
			else if (Takoyaki_Define.CM.IsStickTiltDirection(userType, Takoyaki_ControllerManager.StickType.L, Takoyaki_ControllerManager.StickDirType.RIGHT))
			{
				takoyakiMachine.MoveCursor(Takoyaki_TakoyakiMachine.MoveCursorDirection.RIGHT);
			}
			else if (Takoyaki_Define.CM.IsStickTiltDirection(userType, Takoyaki_ControllerManager.StickType.L, Takoyaki_ControllerManager.StickDirType.LEFT))
			{
				takoyakiMachine.MoveCursor(Takoyaki_TakoyakiMachine.MoveCursorDirection.LEFT);
			}
			else if (Takoyaki_Define.CM.IsStickTiltDirection(userType, Takoyaki_ControllerManager.StickType.L, Takoyaki_ControllerManager.StickDirType.DOWN))
			{
				takoyakiMachine.MoveCursor(Takoyaki_TakoyakiMachine.MoveCursorDirection.DOWN);
			}
			if (!Takoyaki_Define.CM.IsStickTilt(userType, Takoyaki_ControllerManager.StickType.L))
			{
				if (Takoyaki_Define.CM.IsPushCrossKey(userType, Takoyaki_ControllerManager.CrossKeyType.UP, Takoyaki_ControllerManager.ButtonPushType.DOWN) || Takoyaki_Define.CM.IsPushCrossKey(userType, Takoyaki_ControllerManager.CrossKeyType.UP, Takoyaki_ControllerManager.ButtonPushType.HOLD))
				{
					takoyakiMachine.MoveCursor(Takoyaki_TakoyakiMachine.MoveCursorDirection.UP);
				}
				else if (Takoyaki_Define.CM.IsPushCrossKey(userType, Takoyaki_ControllerManager.CrossKeyType.RIGHT, Takoyaki_ControllerManager.ButtonPushType.DOWN) || Takoyaki_Define.CM.IsPushCrossKey(userType, Takoyaki_ControllerManager.CrossKeyType.RIGHT, Takoyaki_ControllerManager.ButtonPushType.HOLD))
				{
					takoyakiMachine.MoveCursor(Takoyaki_TakoyakiMachine.MoveCursorDirection.RIGHT);
				}
				else if (Takoyaki_Define.CM.IsPushCrossKey(userType, Takoyaki_ControllerManager.CrossKeyType.LEFT, Takoyaki_ControllerManager.ButtonPushType.DOWN) || Takoyaki_Define.CM.IsPushCrossKey(userType, Takoyaki_ControllerManager.CrossKeyType.LEFT, Takoyaki_ControllerManager.ButtonPushType.HOLD))
				{
					takoyakiMachine.MoveCursor(Takoyaki_TakoyakiMachine.MoveCursorDirection.LEFT);
				}
				else if (Takoyaki_Define.CM.IsPushCrossKey(userType, Takoyaki_ControllerManager.CrossKeyType.DOWN, Takoyaki_ControllerManager.ButtonPushType.DOWN) || Takoyaki_Define.CM.IsPushCrossKey(userType, Takoyaki_ControllerManager.CrossKeyType.DOWN, Takoyaki_ControllerManager.ButtonPushType.HOLD))
				{
					takoyakiMachine.MoveCursor(Takoyaki_TakoyakiMachine.MoveCursorDirection.DOWN);
				}
			}
			if (Takoyaki_Define.CM.IsPushButton_A(userType, Takoyaki_ControllerManager.ButtonPushType.DOWN))
			{
				takoyakiMachine.TakoyakiProcessAdvance(_isHoldMode: false);
			}
			else if (Takoyaki_Define.CM.IsPushButton_A(userType, Takoyaki_ControllerManager.ButtonPushType.HOLD))
			{
				takoyakiMachine.TakoyakiProcessAdvance(_isHoldMode: true);
			}
			if (Takoyaki_Define.CM.IsPushButton_X(userType, Takoyaki_ControllerManager.ButtonPushType.DOWN) || Takoyaki_Define.CM.IsPushButton_X(userType, Takoyaki_ControllerManager.ButtonPushType.HOLD))
			{
				takoyakiMachine.TakoBallBoxed();
			}
		}
		else
		{
			ai.UpdateMethod();
		}
		takoyakiMachine.UpdateMethod();
	}
}
