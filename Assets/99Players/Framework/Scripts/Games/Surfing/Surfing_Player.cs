using UnityEngine;
public class Surfing_Player : MonoBehaviour
{
	[SerializeField]
	[Header("Surfing_Surfer")]
	private Surfing_Surfer surfer;
	[SerializeField]
	[Header("AI処理")]
	private Surfing_AI ai;
	[SerializeField]
	[Header("CharacterStyle")]
	public CharacterStyle characterStyle;
	[SerializeField]
	[Header("サ\u30fcフボ\u30fcドのメッシュレンダラ\u30fc")]
	private MeshRenderer surfboardMesh;
	[SerializeField]
	[Header("サ\u30fcフボ\u30fcドのマテリアル一覧")]
	private Material[] surfboardMatList;
	private Surfing_Define.UserType userType;
	private float stickHorizontal;
	private float stickVertical;
	private int npadId;
	public Surfing_Surfer Surfer => surfer;
	public Surfing_Define.UserType UserType => userType;
	public void Init(Surfing_Define.UserType _userType)
	{
		userType = _userType;
		characterStyle.SetGameStyle(GS_Define.GameType.GET_BALL, (int)userType);
		surfboardMesh.material = surfboardMatList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
		surfer.PlayerInit(this);
		ai.Init();
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			npadId = 0;
		}
		else if (userType <= Surfing_Define.UserType.PLAYER_4)
		{
			npadId = surfer.PlayerNo;
		}
	}
	public void UpdateMethod()
	{
		if (surfer.processType == Surfing_Surfer.SurferProcessType.GOAL)
		{
			return;
		}
		if (userType <= Surfing_Define.UserType.PLAYER_4)
		{
			if (Surfing_Define.CM.IsStickTiltDirection(userType, Surfing_ControllerManager.StickType.L, Surfing_ControllerManager.StickDirType.UP))
			{
				surfer.MoveCursor(Surfing_Surfer.MoveCursorDirection.UP, 0f);
			}
			else if (Surfing_Define.CM.IsStickTiltDirection(userType, Surfing_ControllerManager.StickType.L, Surfing_ControllerManager.StickDirType.RIGHT))
			{
				surfer.MoveCursor(Surfing_Surfer.MoveCursorDirection.RIGHT, Surfing_Define.CM.GetStickDir_L(userType).x);
			}
			else if (Surfing_Define.CM.IsStickTiltDirection(userType, Surfing_ControllerManager.StickType.L, Surfing_ControllerManager.StickDirType.LEFT))
			{
				surfer.MoveCursor(Surfing_Surfer.MoveCursorDirection.LEFT, Surfing_Define.CM.GetStickDir_L(userType).x);
			}
			else if (Surfing_Define.CM.IsStickTiltDirection(userType, Surfing_ControllerManager.StickType.L, Surfing_ControllerManager.StickDirType.DOWN))
			{
				surfer.MoveCursor(Surfing_Surfer.MoveCursorDirection.DOWN, 0f);
			}
			if (!Surfing_Define.CM.IsStickTilt(userType, Surfing_ControllerManager.StickType.L) && !Surfing_Define.CM.IsPushCrossKey(userType, Surfing_ControllerManager.CrossKeyType.UP, Surfing_ControllerManager.ButtonPushType.DOWN) && !Surfing_Define.CM.IsPushCrossKey(userType, Surfing_ControllerManager.CrossKeyType.UP, Surfing_ControllerManager.ButtonPushType.HOLD))
			{
				if (Surfing_Define.CM.IsPushCrossKey(userType, Surfing_ControllerManager.CrossKeyType.RIGHT, Surfing_ControllerManager.ButtonPushType.DOWN) || Surfing_Define.CM.IsPushCrossKey(userType, Surfing_ControllerManager.CrossKeyType.RIGHT, Surfing_ControllerManager.ButtonPushType.HOLD))
				{
					surfer.MoveCursor(Surfing_Surfer.MoveCursorDirection.RIGHT, 1f);
				}
				else if (Surfing_Define.CM.IsPushCrossKey(userType, Surfing_ControllerManager.CrossKeyType.LEFT, Surfing_ControllerManager.ButtonPushType.DOWN) || Surfing_Define.CM.IsPushCrossKey(userType, Surfing_ControllerManager.CrossKeyType.LEFT, Surfing_ControllerManager.ButtonPushType.HOLD))
				{
					surfer.MoveCursor(Surfing_Surfer.MoveCursorDirection.LEFT, -1f);
				}
				else if (!Surfing_Define.CM.IsPushCrossKey(userType, Surfing_ControllerManager.CrossKeyType.DOWN, Surfing_ControllerManager.ButtonPushType.DOWN))
				{
					Surfing_Define.CM.IsPushCrossKey(userType, Surfing_ControllerManager.CrossKeyType.DOWN, Surfing_ControllerManager.ButtonPushType.HOLD);
				}
			}
			if (SingletonCustom<JoyConManager>.Instance.IsJoyButtonFull(npadId) && !Surfing_Define.CM.IsStickTiltDirection(userType, Surfing_ControllerManager.StickType.R, Surfing_ControllerManager.StickDirType.UP))
			{
				if (Surfing_Define.CM.IsStickTiltDirection(userType, Surfing_ControllerManager.StickType.R, Surfing_ControllerManager.StickDirType.RIGHT))
				{
					surfer.MoveCameraDeg(1f);
				}
				else if (Surfing_Define.CM.IsStickTiltDirection(userType, Surfing_ControllerManager.StickType.R, Surfing_ControllerManager.StickDirType.LEFT))
				{
					surfer.MoveCameraDeg(-1f);
				}
				else
				{
					Surfing_Define.CM.IsStickTiltDirection(userType, Surfing_ControllerManager.StickType.R, Surfing_ControllerManager.StickDirType.DOWN);
				}
			}
			if (!Surfing_Define.CM.IsPushButton_A(userType, Surfing_ControllerManager.ButtonPushType.DOWN) && Surfing_Define.CM.IsPushButton_A(userType, Surfing_ControllerManager.ButtonPushType.HOLD))
			{
				surfer.MoveCameraDeg(1f);
			}
			if (Surfing_Define.CM.IsPushButton_B(userType, Surfing_ControllerManager.ButtonPushType.DOWN))
			{
				surfer.InputSuperJumpAction();
			}
			if (!Surfing_Define.CM.IsPushButton_Y(userType, Surfing_ControllerManager.ButtonPushType.DOWN))
			{
				if (Surfing_Define.CM.IsPushButton_Y(userType, Surfing_ControllerManager.ButtonPushType.HOLD))
				{
					surfer.MoveCameraDeg(-1f);
				}
				else
				{
					Surfing_Define.CM.IsPushButton_Y(userType, Surfing_ControllerManager.ButtonPushType.UP);
				}
			}
		}
		else
		{
			ai.UpdateMethod();
		}
	}
}
