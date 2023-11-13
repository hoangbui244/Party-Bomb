using UnityEngine;
public class Skijump_Define : MonoBehaviour
{
	public enum AiStrength
	{
		WEAK,
		COMMON,
		STRONG
	}
	public enum UserType
	{
		PLAYER_1,
		PLAYER_2,
		PLAYER_3,
		PLAYER_4,
		CPU_1,
		CPU_2,
		CPU_3,
		CPU_4,
		CPU_5
	}
	public enum TeamType
	{
		TEAM_A,
		TEAM_B,
		TEAM_C,
		TEAM_D
	}
	public enum TimingResult
	{
		PERFECT,
		GOOD,
		BAD
	}
	public enum CameraWorkType
	{
		NORMAL,
		LOOKING_DOWN_R,
		LOOKING_DOWN_L,
		FIXED_R,
		FIXED_L,
		MAX
	}
	public enum OperationState
	{
		NONE,
		SLIDE_STANDBY,
		SLIDE,
		JUMP,
		LANDING,
		JUMP_FINISH,
		STOP,
		NEXT
	}
	public enum StageType
	{
		NORMAL,
		LONG
	}
	public enum LandingMotionType
	{
		PERFECT,
		MISS,
		FALL
	}
	public static int PLAYER_NUM = 1;
	public static int CPU_NUM = 3;
	public static int MEMBER_NUM = 4;
	public static readonly int MAX_COOP_TEAM_NUM = 2;
	public static int MEDAL_BRONZE_POINT = 2200;
	public static int MEDAL_SILVER_POINT = 2500;
	public static int MEDAL_GOLD_POINT = 2800;
	public static readonly Color32[] USER_COLOR = new Color32[8]
	{
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue),
		new Color32(16, 85, 251, byte.MaxValue),
		new Color32(254, 202, 47, byte.MaxValue),
		new Color32(160, 90, 168, byte.MaxValue),
		new Color32(0, 229, 211, byte.MaxValue),
		new Color32(149, 153, 130, byte.MaxValue),
		new Color32(250, 120, 0, byte.MaxValue)
	};
	public static readonly Color32[] TEAM_COLOR = new Color32[2]
	{
		new Color32(105, 195, 81, byte.MaxValue),
		new Color32(205, 56, 59, byte.MaxValue)
	};
	public static readonly Color32[] CHARA_COLOR = new Color32[8]
	{
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue),
		new Color32(16, 85, 251, byte.MaxValue),
		new Color32(254, 202, 47, byte.MaxValue),
		new Color32(160, 90, 168, byte.MaxValue),
		new Color32(0, 229, 211, byte.MaxValue),
		new Color32(149, 153, 130, byte.MaxValue),
		new Color32(250, 120, 0, byte.MaxValue)
	};
	public static float MAX_JUMP_NUM = 2f;
	public static float SLIDE_BASE = 0f;
	public static float TAKE_OFF_BASE = 0.5f;
	public static float TAKE_OFF_VELOCITY_CORR = 0.575f;
	public static float LIFT_POWER_BASE = 780f;
	public static readonly float RAY_DISTANCE_MAX = 10f;
	public static float MOTION_TIME_STANDARD = 0.2f;
	public static float MOTION_TIME_MISS = 0.075f;
	public static float MOTION_TIME_FALL = 0.2f;
	public const string LAYER_COLLISION_OBJ_1 = "Collision_Obj_1";
	public const string LAYER_FIELD = "Field";
	public const string LAYER_CHARACTER = "Character";
	public const string LAYER_GOAL = "Goal";
	public static Skijump_GameManager MGM => SingletonCustom<Skijump_GameManager>.Instance;
	public static Skijump_CharacterManager MCM => SingletonCustom<Skijump_CharacterManager>.Instance;
	public static Skijump_StageManager MSM => SingletonCustom<Skijump_StageManager>.Instance;
	public static Skijump_UIManager GUIM => SingletonCustom<Skijump_UIManager>.Instance;
	public static Skijump_ControllerManager CM => SingletonCustom<Skijump_ControllerManager>.Instance;
	public static int GetLayerMask(string _layerName)
	{
		return 1 << LayerMask.NameToLayer(_layerName);
	}
}
