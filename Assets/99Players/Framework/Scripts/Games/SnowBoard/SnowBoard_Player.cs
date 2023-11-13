using UnityEngine;
public class SnowBoard_Player : MonoBehaviour
{
	[SerializeField]
	[Header("SnowBoard_SkiBoard")]
	private SnowBoard_SkiBoard skiBoard;
	[SerializeField]
	[Header("AI処理")]
	private SnowBoard_AI ai;
	[SerializeField]
	[Header("CharacterStyle")]
	public CharacterStyle characterStyle;
	[SerializeField]
	[Header("スノ\u30fcボ\u30fcドのメッシュ")]
	private MeshFilter snowboardMesh;
	[SerializeField]
	[Header("スノ\u30fcボ\u30fcドのメッシュ一覧")]
	private Mesh[] snowboardMeshList;
	private SnowBoard_Define.UserType userType;
	private float stickHorizontal;
	private float stickVertical;
	private int npadId;
	public SnowBoard_SkiBoard SkiBoard => skiBoard;
	public SnowBoard_Define.UserType UserType => userType;
	public void Init(SnowBoard_Define.UserType _userType)
	{
		userType = _userType;
		skiBoard.PlayerInit(this);
		characterStyle.SetGameStyle(GS_Define.GameType.GET_BALL, (int)userType);
		snowboardMesh.mesh = snowboardMeshList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
		ai.Init();
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			npadId = 0;
		}
		else if (userType <= SnowBoard_Define.UserType.PLAYER_4)
		{
			npadId = skiBoard.PlayerNo;
		}
	}
	public void UpdateMethod()
	{
		if (skiBoard.processType == SnowBoard_SkiBoard.SkiBoardProcessType.GOAL)
		{
			return;
		}
		if (userType <= SnowBoard_Define.UserType.PLAYER_4)
		{
			if (SnowBoard_Define.CM.IsStickTiltDirection(userType, SnowBoard_ControllerManager.StickType.L, SnowBoard_ControllerManager.StickDirType.UP))
			{
				skiBoard.MoveCursor(SnowBoard_SkiBoard.MoveCursorDirection.UP, 0f);
			}
			else if (SnowBoard_Define.CM.IsStickTiltDirection(userType, SnowBoard_ControllerManager.StickType.L, SnowBoard_ControllerManager.StickDirType.RIGHT))
			{
				skiBoard.MoveCursor(SnowBoard_SkiBoard.MoveCursorDirection.RIGHT, SnowBoard_Define.CM.GetStickDir_L(userType).x);
			}
			else if (SnowBoard_Define.CM.IsStickTiltDirection(userType, SnowBoard_ControllerManager.StickType.L, SnowBoard_ControllerManager.StickDirType.LEFT))
			{
				skiBoard.MoveCursor(SnowBoard_SkiBoard.MoveCursorDirection.LEFT, SnowBoard_Define.CM.GetStickDir_L(userType).x);
			}
			else if (SnowBoard_Define.CM.IsStickTiltDirection(userType, SnowBoard_ControllerManager.StickType.L, SnowBoard_ControllerManager.StickDirType.DOWN))
			{
				skiBoard.MoveCursor(SnowBoard_SkiBoard.MoveCursorDirection.DOWN, 0f);
			}
			if (!SnowBoard_Define.CM.IsStickTilt(userType, SnowBoard_ControllerManager.StickType.L))
			{
				if (SnowBoard_Define.CM.IsPushCrossKey(userType, SnowBoard_ControllerManager.CrossKeyType.UP, SnowBoard_ControllerManager.ButtonPushType.DOWN) || SnowBoard_Define.CM.IsPushCrossKey(userType, SnowBoard_ControllerManager.CrossKeyType.UP, SnowBoard_ControllerManager.ButtonPushType.HOLD))
				{
					skiBoard.MoveCursor(SnowBoard_SkiBoard.MoveCursorDirection.UP, 0f);
				}
				else if (SnowBoard_Define.CM.IsPushCrossKey(userType, SnowBoard_ControllerManager.CrossKeyType.RIGHT, SnowBoard_ControllerManager.ButtonPushType.DOWN) || SnowBoard_Define.CM.IsPushCrossKey(userType, SnowBoard_ControllerManager.CrossKeyType.RIGHT, SnowBoard_ControllerManager.ButtonPushType.HOLD))
				{
					skiBoard.MoveCursor(SnowBoard_SkiBoard.MoveCursorDirection.RIGHT, 1f);
				}
				else if (SnowBoard_Define.CM.IsPushCrossKey(userType, SnowBoard_ControllerManager.CrossKeyType.LEFT, SnowBoard_ControllerManager.ButtonPushType.DOWN) || SnowBoard_Define.CM.IsPushCrossKey(userType, SnowBoard_ControllerManager.CrossKeyType.LEFT, SnowBoard_ControllerManager.ButtonPushType.HOLD))
				{
					skiBoard.MoveCursor(SnowBoard_SkiBoard.MoveCursorDirection.LEFT, -1f);
				}
				else if (SnowBoard_Define.CM.IsPushCrossKey(userType, SnowBoard_ControllerManager.CrossKeyType.DOWN, SnowBoard_ControllerManager.ButtonPushType.DOWN) || SnowBoard_Define.CM.IsPushCrossKey(userType, SnowBoard_ControllerManager.CrossKeyType.DOWN, SnowBoard_ControllerManager.ButtonPushType.HOLD))
				{
					skiBoard.MoveCursor(SnowBoard_SkiBoard.MoveCursorDirection.DOWN, 0f);
				}
			}
			if (SnowBoard_Define.CM.IsPushButton_A(userType, SnowBoard_ControllerManager.ButtonPushType.DOWN))
			{
				skiBoard.ActionInput();
			}
			if (SnowBoard_Define.CM.IsPushButton_B(userType, SnowBoard_ControllerManager.ButtonPushType.DOWN))
			{
				skiBoard.IsTurnChange(_set: true);
			}
			else if (SnowBoard_Define.CM.IsPushButton_B(userType, SnowBoard_ControllerManager.ButtonPushType.UP))
			{
				skiBoard.IsTurnChange(_set: false);
			}
			if (SnowBoard_Define.CM.IsPushButton_R(userType, SnowBoard_ControllerManager.ButtonPushType.DOWN))
			{
				skiBoard.CameraPosTypeChange(_set: true);
			}
			if (SnowBoard_Define.CM.IsPushButton_ZR(userType, SnowBoard_ControllerManager.ButtonPushType.DOWN) && SingletonCustom<JoyConManager>.Instance.IsJoyButtonFull(npadId))
			{
				skiBoard.CameraPosTypeChange(_set: true);
			}
			if (SnowBoard_Define.CM.IsPushButton_L(userType, SnowBoard_ControllerManager.ButtonPushType.DOWN))
			{
				skiBoard.CameraPosTypeChange(_set: false);
			}
			if (SnowBoard_Define.CM.IsPushButton_ZL(userType, SnowBoard_ControllerManager.ButtonPushType.DOWN) && SingletonCustom<JoyConManager>.Instance.IsJoyButtonFull(npadId))
			{
				skiBoard.CameraPosTypeChange(_set: false);
			}
		}
		else
		{
			ai.UpdateMethod();
		}
		skiBoard.UpdateMethod();
	}
}
