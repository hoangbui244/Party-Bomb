using UnityEngine;
public class AlpineSkiing_Player : MonoBehaviour {
    [SerializeField]
    [Header("AlpineSkiing_SkiBoard")]
    private AlpineSkiing_SkiBoard skiBoard;
    [SerializeField]
    [Header("AI処理")]
    private AlpineSkiing_AI ai;
    [SerializeField]
    [Header("CharacterStyle")]
    public CharacterStyle characterStyle;
    [SerializeField]
    [Header("スキ\u30fc板(左)のメッシュ")]
    private MeshFilter skiboardMesh_Left;
    [SerializeField]
    [Header("スキ\u30fc板(右)のメッシュ")]
    private MeshFilter skiboardMesh_Right;
    [SerializeField]
    [Header("スキ\u30fc板(左)のメッシュ一覧")]
    private Mesh[] skiboardMesh_LeftList;
    [SerializeField]
    [Header("スキ\u30fc板(右)のメッシュ一覧")]
    private Mesh[] skiboardMesh_RightList;
    [SerializeField]
    [Header("スキ\u30fcポ\u30fcル(左)のメッシュ")]
    private MeshFilter skiPoleMesh_Left;
    [SerializeField]
    [Header("スキ\u30fcポ\u30fcル(右)のメッシュ")]
    private MeshFilter skiPoleMesh_Right;
    [SerializeField]
    [Header("スキ\u30fcポ\u30fcルのメッシュ一覧")]
    private Mesh[] skiPoleMeshList;
    private AlpineSkiing_Define.UserType userType;
    private float stickHorizontal;
    private float stickVertical;
    private int npadId;
    public AlpineSkiing_SkiBoard SkiBoard => skiBoard;
    public AlpineSkiing_Define.UserType UserType => userType;
    public void Init(AlpineSkiing_Define.UserType _userType) {
        userType = _userType;
        characterStyle.SetGameStyle(GS_Define.GameType.CANNON_SHOT, (int)userType);
        skiboardMesh_Left.mesh = skiboardMesh_LeftList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
        skiboardMesh_Right.mesh = skiboardMesh_RightList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
        skiPoleMesh_Left.mesh = skiPoleMeshList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
        skiPoleMesh_Right.mesh = skiPoleMeshList[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
        skiBoard.PlayerInit(this);
        ai.Init();
        if (SingletonCustom<JoyConManager>.Instance.IsSingleMode()) {
            npadId = 0;
        } else if (userType <= AlpineSkiing_Define.UserType.PLAYER_4) {
            npadId = skiBoard.PlayerNo;
        }
    }
    public void UpdateMethod() {
        if (skiBoard.processType == AlpineSkiing_SkiBoard.SkiBoardProcessType.GOAL) {
            return;
        }
        if (userType <= AlpineSkiing_Define.UserType.PLAYER_4) {
            if (!AlpineSkiing_Define.CM.IsStickTiltDirection(userType, AlpineSkiing_ControllerManager.StickType.L, AlpineSkiing_ControllerManager.StickDirType.UP)) {
                if (AlpineSkiing_Define.CM.IsStickTiltDirection(userType, AlpineSkiing_ControllerManager.StickType.L, AlpineSkiing_ControllerManager.StickDirType.RIGHT)) {
                    skiBoard.MoveCursor(AlpineSkiing_SkiBoard.MoveCursorDirection.RIGHT, AlpineSkiing_Define.CM.GetStickDir_L(userType).x);
                } else if (AlpineSkiing_Define.CM.IsStickTiltDirection(userType, AlpineSkiing_ControllerManager.StickType.L, AlpineSkiing_ControllerManager.StickDirType.LEFT)) {
                    skiBoard.MoveCursor(AlpineSkiing_SkiBoard.MoveCursorDirection.LEFT, AlpineSkiing_Define.CM.GetStickDir_L(userType).x);
                } else {
                    AlpineSkiing_Define.CM.IsStickTiltDirection(userType, AlpineSkiing_ControllerManager.StickType.L, AlpineSkiing_ControllerManager.StickDirType.DOWN);
                }
            }
            if (!AlpineSkiing_Define.CM.IsStickTilt(userType, AlpineSkiing_ControllerManager.StickType.L) && !AlpineSkiing_Define.CM.IsPushCrossKey(userType, AlpineSkiing_ControllerManager.CrossKeyType.UP, AlpineSkiing_ControllerManager.ButtonPushType.DOWN) && !AlpineSkiing_Define.CM.IsPushCrossKey(userType, AlpineSkiing_ControllerManager.CrossKeyType.UP, AlpineSkiing_ControllerManager.ButtonPushType.HOLD)) {
                if (AlpineSkiing_Define.CM.IsPushCrossKey(userType, AlpineSkiing_ControllerManager.CrossKeyType.RIGHT, AlpineSkiing_ControllerManager.ButtonPushType.DOWN) || AlpineSkiing_Define.CM.IsPushCrossKey(userType, AlpineSkiing_ControllerManager.CrossKeyType.RIGHT, AlpineSkiing_ControllerManager.ButtonPushType.HOLD)) {
                    skiBoard.MoveCursor(AlpineSkiing_SkiBoard.MoveCursorDirection.RIGHT, 1f);
                } else if (AlpineSkiing_Define.CM.IsPushCrossKey(userType, AlpineSkiing_ControllerManager.CrossKeyType.LEFT, AlpineSkiing_ControllerManager.ButtonPushType.DOWN) || AlpineSkiing_Define.CM.IsPushCrossKey(userType, AlpineSkiing_ControllerManager.CrossKeyType.LEFT, AlpineSkiing_ControllerManager.ButtonPushType.HOLD)) {
                    skiBoard.MoveCursor(AlpineSkiing_SkiBoard.MoveCursorDirection.LEFT, -1f);
                } else if (!AlpineSkiing_Define.CM.IsPushCrossKey(userType, AlpineSkiing_ControllerManager.CrossKeyType.DOWN, AlpineSkiing_ControllerManager.ButtonPushType.DOWN)) {
                    AlpineSkiing_Define.CM.IsPushCrossKey(userType, AlpineSkiing_ControllerManager.CrossKeyType.DOWN, AlpineSkiing_ControllerManager.ButtonPushType.HOLD);
                }
            }
            if (AlpineSkiing_Define.CM.IsPushButton_A(userType, AlpineSkiing_ControllerManager.ButtonPushType.DOWN)) {
                skiBoard.ProcessTypeChange(AlpineSkiing_SkiBoard.SkiBoardProcessType.CURVE);
            } else if (AlpineSkiing_Define.CM.IsPushButton_A(userType, AlpineSkiing_ControllerManager.ButtonPushType.UP)) {
                skiBoard.ProcessTypeChange(AlpineSkiing_SkiBoard.SkiBoardProcessType.SLIDING);
            }
            if (AlpineSkiing_Define.CM.IsPushButton_R(userType, AlpineSkiing_ControllerManager.ButtonPushType.DOWN)) {
                skiBoard.CameraPosTypeChange(_set: true);
            }
            if (AlpineSkiing_Define.CM.IsPushButton_ZR(userType, AlpineSkiing_ControllerManager.ButtonPushType.DOWN) && SingletonCustom<JoyConManager>.Instance.IsJoyButtonFull(npadId)) {
                skiBoard.CameraPosTypeChange(_set: true);
            }
            if (AlpineSkiing_Define.CM.IsPushButton_L(userType, AlpineSkiing_ControllerManager.ButtonPushType.DOWN)) {
                skiBoard.CameraPosTypeChange(_set: false);
            }
            if (AlpineSkiing_Define.CM.IsPushButton_ZL(userType, AlpineSkiing_ControllerManager.ButtonPushType.DOWN) && SingletonCustom<JoyConManager>.Instance.IsJoyButtonFull(npadId)) {
                skiBoard.CameraPosTypeChange(_set: false);
            }
        } else {
            ai.UpdateMethod();
        }
        skiBoard.UpdateMethod();
    }
}
