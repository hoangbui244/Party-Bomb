using UnityEngine;
public class WaterSpriderRace_Player : MonoBehaviour
{
	[SerializeField]
	[Header("WaterSpriderRace_WaterSprider")]
	private WaterSpriderRace_WaterSprider waterSprider;
	[SerializeField]
	[Header("AI処理")]
	private WaterSpriderRace_AI ai;
	[SerializeField]
	[Header("CharacterStyle")]
	public CharacterStyle characterStyle;
	[SerializeField]
	[Header("水蜘蛛(左)のメッシュ")]
	private MeshFilter waterSpriderMesh_Left;
	[SerializeField]
	[Header("水蜘蛛(右)のメッシュ")]
	private MeshFilter waterSpriderMesh_Right;
	[SerializeField]
	[Header("水蜘蛛(左)のメッシュ一覧")]
	private Mesh[] waterSpriderMesh_LeftList;
	[SerializeField]
	[Header("水蜘蛛(右)のメッシュ一覧")]
	private Mesh[] waterSpriderMesh_RightList;
	private WaterSpriderRace_Define.UserType userType;
	private float stickHorizontal;
	private float stickVertical;
	private int npadId;
	public WaterSpriderRace_WaterSprider WaterSprider => waterSprider;
	public WaterSpriderRace_Define.UserType UserType => userType;
	public void Init(WaterSpriderRace_Define.UserType _userType)
	{
		userType = _userType;
		characterStyle.SetGameStyle(GS_Define.GameType.CANNON_SHOT, (int)userType);
		waterSprider.PlayerInit(this);
		ai.Init();
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			npadId = 0;
		}
		else if (userType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			npadId = waterSprider.PlayerNo;
		}
	}
	public void UpdateMethod()
	{
		if (waterSprider.processType == WaterSpriderRace_WaterSprider.SkiBoardProcessType.GOAL)
		{
			return;
		}
		if (userType <= WaterSpriderRace_Define.UserType.PLAYER_4)
		{
			if (!WaterSpriderRace_Define.CM.IsStickTiltDirection(userType, WaterSpriderRace_ControllerManager.StickType.L, WaterSpriderRace_ControllerManager.StickDirType.UP))
			{
				if (WaterSpriderRace_Define.CM.IsStickTiltDirection(userType, WaterSpriderRace_ControllerManager.StickType.L, WaterSpriderRace_ControllerManager.StickDirType.RIGHT))
				{
					waterSprider.MoveCursor(WaterSpriderRace_WaterSprider.MoveCursorDirection.RIGHT, WaterSpriderRace_Define.CM.GetStickDir_L(userType).x);
				}
				else if (WaterSpriderRace_Define.CM.IsStickTiltDirection(userType, WaterSpriderRace_ControllerManager.StickType.L, WaterSpriderRace_ControllerManager.StickDirType.LEFT))
				{
					waterSprider.MoveCursor(WaterSpriderRace_WaterSprider.MoveCursorDirection.LEFT, WaterSpriderRace_Define.CM.GetStickDir_L(userType).x);
				}
				else
				{
					WaterSpriderRace_Define.CM.IsStickTiltDirection(userType, WaterSpriderRace_ControllerManager.StickType.L, WaterSpriderRace_ControllerManager.StickDirType.DOWN);
				}
			}
			if (!WaterSpriderRace_Define.CM.IsStickTilt(userType, WaterSpriderRace_ControllerManager.StickType.L) && !WaterSpriderRace_Define.CM.IsPushCrossKey(userType, WaterSpriderRace_ControllerManager.CrossKeyType.UP, WaterSpriderRace_ControllerManager.ButtonPushType.DOWN) && !WaterSpriderRace_Define.CM.IsPushCrossKey(userType, WaterSpriderRace_ControllerManager.CrossKeyType.UP, WaterSpriderRace_ControllerManager.ButtonPushType.HOLD))
			{
				if (WaterSpriderRace_Define.CM.IsPushCrossKey(userType, WaterSpriderRace_ControllerManager.CrossKeyType.RIGHT, WaterSpriderRace_ControllerManager.ButtonPushType.DOWN) || WaterSpriderRace_Define.CM.IsPushCrossKey(userType, WaterSpriderRace_ControllerManager.CrossKeyType.RIGHT, WaterSpriderRace_ControllerManager.ButtonPushType.HOLD))
				{
					waterSprider.MoveCursor(WaterSpriderRace_WaterSprider.MoveCursorDirection.RIGHT, 1f);
				}
				else if (WaterSpriderRace_Define.CM.IsPushCrossKey(userType, WaterSpriderRace_ControllerManager.CrossKeyType.LEFT, WaterSpriderRace_ControllerManager.ButtonPushType.DOWN) || WaterSpriderRace_Define.CM.IsPushCrossKey(userType, WaterSpriderRace_ControllerManager.CrossKeyType.LEFT, WaterSpriderRace_ControllerManager.ButtonPushType.HOLD))
				{
					waterSprider.MoveCursor(WaterSpriderRace_WaterSprider.MoveCursorDirection.LEFT, -1f);
				}
				else if (!WaterSpriderRace_Define.CM.IsPushCrossKey(userType, WaterSpriderRace_ControllerManager.CrossKeyType.DOWN, WaterSpriderRace_ControllerManager.ButtonPushType.DOWN))
				{
					WaterSpriderRace_Define.CM.IsPushCrossKey(userType, WaterSpriderRace_ControllerManager.CrossKeyType.DOWN, WaterSpriderRace_ControllerManager.ButtonPushType.HOLD);
				}
			}
			if (WaterSpriderRace_Define.CM.IsPushButton_A(userType, WaterSpriderRace_ControllerManager.ButtonPushType.DOWN))
			{
				waterSprider.AccelInput();
			}
			if (WaterSpriderRace_Define.CM.IsPushButton_R(userType, WaterSpriderRace_ControllerManager.ButtonPushType.DOWN))
			{
				waterSprider.CameraPosTypeChange(_set: true);
			}
			else if (WaterSpriderRace_Define.CM.IsPushButton_ZR(userType, WaterSpriderRace_ControllerManager.ButtonPushType.DOWN) && SingletonCustom<JoyConManager>.Instance.IsJoyButtonFull(npadId))
			{
				waterSprider.CameraPosTypeChange(_set: true);
			}
			if (WaterSpriderRace_Define.CM.IsPushButton_L(userType, WaterSpriderRace_ControllerManager.ButtonPushType.DOWN))
			{
				waterSprider.CameraPosTypeChange(_set: false);
			}
			else if (WaterSpriderRace_Define.CM.IsPushButton_ZL(userType, WaterSpriderRace_ControllerManager.ButtonPushType.DOWN) && SingletonCustom<JoyConManager>.Instance.IsJoyButtonFull(npadId))
			{
				waterSprider.CameraPosTypeChange(_set: false);
			}
		}
		else
		{
			ai.UpdateMethod();
		}
	}
}
