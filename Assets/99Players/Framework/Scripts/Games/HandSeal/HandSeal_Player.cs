using UnityEngine;
public class HandSeal_Player : MonoBehaviour
{
	[SerializeField]
	[Header("HandSeal_Hand")]
	private HandSeal_Hand hand;
	[SerializeField]
	[Header("AI処理")]
	private HandSeal_AI ai;
	[SerializeField]
	[Header("CharacterStyle")]
	public CharacterStyle characterStyle;
	[SerializeField]
	[Header("手のスキンメッシュレンダラ\u30fc")]
	private SkinnedMeshRenderer handMesh;
	[SerializeField]
	[Header("ヒナ専用の袖のスキンメッシュレンダラ\u30fc")]
	private SkinnedMeshRenderer hinaMesh;
	[SerializeField]
	[Header("籠手1のメッシュレンダラ\u30fc")]
	private MeshRenderer[] kote1Mesh = new MeshRenderer[2];
	[SerializeField]
	[Header("籠手1の装飾のメッシュレンダラ\u30fc")]
	private MeshRenderer[] kote1LineMesh = new MeshRenderer[2];
	[SerializeField]
	[Header("籠手2のメッシュレンダラ\u30fc")]
	private MeshRenderer[] kote2Mesh = new MeshRenderer[2];
	[SerializeField]
	[Header("キャラ別のマテリアル一覧")]
	private Material[] handMatList;
	private HandSeal_Define.UserType userType;
	private float stickHorizontal;
	private float stickVertical;
	private int npadId;
	public HandSeal_Hand Hand => hand;
	public HandSeal_Define.UserType UserType => userType;
	public void Init(HandSeal_Define.UserType _userType)
	{
		userType = _userType;
		characterStyle.SetGameStyle(GS_Define.GameType.CANNON_SHOT, (int)userType);
		handMesh.material = handMatList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
		switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType])
		{
		case 0:
			kote1Mesh[0].gameObject.SetActive(value: true);
			kote1Mesh[1].gameObject.SetActive(value: true);
			kote1LineMesh[0].gameObject.SetActive(value: false);
			kote1LineMesh[1].gameObject.SetActive(value: false);
			kote2Mesh[0].gameObject.SetActive(value: false);
			kote2Mesh[1].gameObject.SetActive(value: false);
			hinaMesh.gameObject.SetActive(value: false);
			break;
		case 1:
			kote1Mesh[0].gameObject.SetActive(value: false);
			kote1Mesh[1].gameObject.SetActive(value: false);
			kote1LineMesh[0].gameObject.SetActive(value: false);
			kote1LineMesh[1].gameObject.SetActive(value: false);
			kote2Mesh[0].gameObject.SetActive(value: false);
			kote2Mesh[1].gameObject.SetActive(value: false);
			hinaMesh.gameObject.SetActive(value: true);
			break;
		case 2:
			kote1Mesh[0].gameObject.SetActive(value: true);
			kote1Mesh[1].gameObject.SetActive(value: true);
			kote1LineMesh[0].gameObject.SetActive(value: false);
			kote1LineMesh[1].gameObject.SetActive(value: false);
			kote2Mesh[0].gameObject.SetActive(value: true);
			kote2Mesh[1].gameObject.SetActive(value: true);
			hinaMesh.gameObject.SetActive(value: false);
			break;
		case 3:
			kote1Mesh[0].gameObject.SetActive(value: false);
			kote1Mesh[1].gameObject.SetActive(value: false);
			kote1LineMesh[0].gameObject.SetActive(value: false);
			kote1LineMesh[1].gameObject.SetActive(value: false);
			kote2Mesh[0].gameObject.SetActive(value: false);
			kote2Mesh[1].gameObject.SetActive(value: false);
			hinaMesh.gameObject.SetActive(value: false);
			break;
		case 4:
			kote1Mesh[0].gameObject.SetActive(value: true);
			kote1Mesh[1].gameObject.SetActive(value: true);
			kote1LineMesh[0].gameObject.SetActive(value: false);
			kote1LineMesh[1].gameObject.SetActive(value: false);
			kote2Mesh[0].gameObject.SetActive(value: false);
			kote2Mesh[1].gameObject.SetActive(value: false);
			hinaMesh.gameObject.SetActive(value: false);
			break;
		case 5:
			kote1Mesh[0].gameObject.SetActive(value: true);
			kote1Mesh[1].gameObject.SetActive(value: true);
			kote1LineMesh[0].gameObject.SetActive(value: false);
			kote1LineMesh[1].gameObject.SetActive(value: false);
			kote2Mesh[0].gameObject.SetActive(value: true);
			kote2Mesh[1].gameObject.SetActive(value: true);
			hinaMesh.gameObject.SetActive(value: false);
			break;
		case 6:
			kote1Mesh[0].gameObject.SetActive(value: true);
			kote1Mesh[1].gameObject.SetActive(value: true);
			kote1LineMesh[0].gameObject.SetActive(value: true);
			kote1LineMesh[1].gameObject.SetActive(value: true);
			kote2Mesh[0].gameObject.SetActive(value: true);
			kote2Mesh[1].gameObject.SetActive(value: true);
			hinaMesh.gameObject.SetActive(value: false);
			break;
		case 7:
			kote1Mesh[0].gameObject.SetActive(value: true);
			kote1Mesh[1].gameObject.SetActive(value: true);
			kote1LineMesh[0].gameObject.SetActive(value: false);
			kote1LineMesh[1].gameObject.SetActive(value: false);
			kote2Mesh[0].gameObject.SetActive(value: false);
			kote2Mesh[1].gameObject.SetActive(value: false);
			hinaMesh.gameObject.SetActive(value: false);
			break;
		}
		kote1Mesh[0].material = handMatList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
		kote1Mesh[1].material = handMatList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
		kote1LineMesh[0].material = handMatList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
		kote1LineMesh[1].material = handMatList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
		kote2Mesh[0].material = handMatList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
		kote2Mesh[1].material = handMatList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
		hand.PlayerInit(this);
		ai.Init();
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			npadId = 0;
		}
		else if (userType <= HandSeal_Define.UserType.PLAYER_4)
		{
			npadId = hand.PlayerNo;
		}
	}
	public void UpdateMethod()
	{
		if (hand.processType == HandSeal_Hand.GameProcessType.END)
		{
			return;
		}
		if (userType <= HandSeal_Define.UserType.PLAYER_4)
		{
			if (!HandSeal_Define.CM.IsStickTiltDirection(userType, HandSeal_ControllerManager.StickType.L, HandSeal_ControllerManager.StickDirType.UP))
			{
				if (HandSeal_Define.CM.IsStickTiltDirection(userType, HandSeal_ControllerManager.StickType.L, HandSeal_ControllerManager.StickDirType.RIGHT))
				{
					hand.MoveCursor(HandSeal_Hand.MoveCursorDirection.RIGHT);
				}
				else if (HandSeal_Define.CM.IsStickTiltDirection(userType, HandSeal_ControllerManager.StickType.L, HandSeal_ControllerManager.StickDirType.LEFT))
				{
					hand.MoveCursor(HandSeal_Hand.MoveCursorDirection.LEFT);
				}
				else
				{
					HandSeal_Define.CM.IsStickTiltDirection(userType, HandSeal_ControllerManager.StickType.L, HandSeal_ControllerManager.StickDirType.DOWN);
				}
			}
			if (!HandSeal_Define.CM.IsStickTilt(userType, HandSeal_ControllerManager.StickType.L) && !HandSeal_Define.CM.IsPushCrossKey(userType, HandSeal_ControllerManager.CrossKeyType.UP, HandSeal_ControllerManager.ButtonPushType.DOWN) && !HandSeal_Define.CM.IsPushCrossKey(userType, HandSeal_ControllerManager.CrossKeyType.UP, HandSeal_ControllerManager.ButtonPushType.HOLD))
			{
				if (HandSeal_Define.CM.IsPushCrossKey(userType, HandSeal_ControllerManager.CrossKeyType.RIGHT, HandSeal_ControllerManager.ButtonPushType.DOWN) || HandSeal_Define.CM.IsPushCrossKey(userType, HandSeal_ControllerManager.CrossKeyType.RIGHT, HandSeal_ControllerManager.ButtonPushType.HOLD))
				{
					hand.MoveCursor(HandSeal_Hand.MoveCursorDirection.RIGHT);
				}
				else if (HandSeal_Define.CM.IsPushCrossKey(userType, HandSeal_ControllerManager.CrossKeyType.LEFT, HandSeal_ControllerManager.ButtonPushType.DOWN) || HandSeal_Define.CM.IsPushCrossKey(userType, HandSeal_ControllerManager.CrossKeyType.LEFT, HandSeal_ControllerManager.ButtonPushType.HOLD))
				{
					hand.MoveCursor(HandSeal_Hand.MoveCursorDirection.LEFT);
				}
				else if (!HandSeal_Define.CM.IsPushCrossKey(userType, HandSeal_ControllerManager.CrossKeyType.DOWN, HandSeal_ControllerManager.ButtonPushType.DOWN))
				{
					HandSeal_Define.CM.IsPushCrossKey(userType, HandSeal_ControllerManager.CrossKeyType.DOWN, HandSeal_ControllerManager.ButtonPushType.HOLD);
				}
			}
			if (HandSeal_Define.CM.IsPushButton_A(userType, HandSeal_ControllerManager.ButtonPushType.DOWN))
			{
				hand.InputSeal(HandSeal_Hand.InputButton.A);
			}
			if (HandSeal_Define.CM.IsPushButton_B(userType, HandSeal_ControllerManager.ButtonPushType.DOWN))
			{
				hand.InputSeal(HandSeal_Hand.InputButton.B);
			}
			if (HandSeal_Define.CM.IsPushButton_Y(userType, HandSeal_ControllerManager.ButtonPushType.DOWN))
			{
				hand.InputSeal(HandSeal_Hand.InputButton.Y);
			}
			if (HandSeal_Define.CM.IsPushButton_X(userType, HandSeal_ControllerManager.ButtonPushType.DOWN))
			{
				hand.InputSeal(HandSeal_Hand.InputButton.X);
			}
			if (HandSeal_Define.CM.IsPushButton_R(userType, HandSeal_ControllerManager.ButtonPushType.DOWN))
			{
				hand.SecretStyleAction();
			}
			if (HandSeal_Define.CM.IsPushButton_ZR(userType, HandSeal_ControllerManager.ButtonPushType.DOWN) && SingletonCustom<JoyConManager>.Instance.IsJoyButtonFull(npadId))
			{
				hand.SecretStyleAction();
			}
			if (HandSeal_Define.CM.IsPushButton_L(userType, HandSeal_ControllerManager.ButtonPushType.DOWN))
			{
				hand.SecretStyleAction();
			}
			if (HandSeal_Define.CM.IsPushButton_ZL(userType, HandSeal_ControllerManager.ButtonPushType.DOWN) && SingletonCustom<JoyConManager>.Instance.IsJoyButtonFull(npadId))
			{
				hand.SecretStyleAction();
			}
		}
		else
		{
			ai.UpdateMethod();
		}
		hand.UpdateMethod();
	}
}
