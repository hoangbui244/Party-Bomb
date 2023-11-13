using UnityEngine;
public class Shooting_Define : MonoBehaviour
{
	public static string TAG_OBJECT = "Object";
	public static string TAG_RUBBERGUN_TARGET = "RubberGun_Target";
	public const int LAYER_LOOK_WALL = 134217728;
	public const int LAYER_AI_TARGET = 536870912;
	public const float GAME_TIME = 60f;
	public static int MAX_PLAYER_NUM = 4;
	public static int MAX_TEAM_NUM = 2;
	public static bool IS_BATTLE_MODE = true;
	public static bool IS_TEAM_MODE = false;
	public const int MAX_BULLET_NUM = 12;
	public const float BULLET_DIFF_DROP_SPEED = 3.5f;
	public const float BULLET_BASE_POWER = 2f;
	public const float BULLET_SPEED = 120f;
	public const float WEAK_HIT_POWER = 1f;
	public const float NORMAL_HIT_POWER = 0.6f;
	public const float BAD_HIT_POWER = 0.2f;
	public const float TARGET_FALL_ANGLE = 60f;
	public const float TARGET_DIFF_FALL_DROP_SPEED = 2.5f;
	public const float SHOT_INTERVAL = 1.2f;
	public const float NEUTRAL_STICK_MAGNITUDE = 0.2f;
	public const float CURSOR_MOVE_SPEED = 1f;
	public const float CURSOR_MOVE_VALUE = 500f;
	public const float CURSOR_INPUT_TIME_VALUE = 1f;
	public const float CURSOR_SINGLE_MAG = 1.5f;
	public const float SINGLE_GUN_BASE_POS_X = 0.3f;
	public const float SINGLE_GUN_RANGE_POS_X = -0.6f;
	public const float SINGLE_GUN_BASE_POS_Y = -0.45f;
	public const float SINGLE_GUN_RANGE_POS_Y = -0.15f;
	public const float SINGLE_CURSOR_START_LOCAL_SCALE = 3f;
	public const float SINGLE_CURSOR_ZOOM_LOCAL_SCALE = 6f;
	public const float SINGLE_CPU_VOLUME = 0.5f;
	public static readonly float[] AI_AIM_COLLIDER_SCALE_MAGS = new float[3]
	{
		1.8f,
		1.6f,
		1.2f
	};
	public static readonly float[] AI_AIM_OK_TIMES = new float[3]
	{
		0.025f,
		0.025f,
		0.025f
	};
	public static readonly float[] AI_MOVE_UPDATE_INTERVALS = new float[3]
	{
		0.15f,
		0.05f,
		0f
	};
	public static readonly float[] AI_FUTURE_MOVE_TIMES = new float[3]
	{
		0.8f,
		0.5f,
		0.3f
	};
	public static readonly float[] AI_SUCK_TARGET = new float[3]
	{
		0.05f,
		0.1f,
		0.5f
	};
	public const float AI_NEAR_SCREEN_DIS = 10f;
	public const float AI_FAR_SCREEN_DIS = 200f;
	public static readonly float[] AI_SKIP_ONE_SHOT_TIMES = new float[3]
	{
		6f,
		5f,
		4f
	};
	public static readonly int[] AI_SKIP_ONE_SHOT_DROP_PER = new int[3]
	{
		30,
		60,
		90
	};
	public static readonly int[] AI_SKIP_ONE_SHOT_DROP_MINUS = new int[3]
	{
		30,
		5,
		0
	};
	public static readonly int[] AI_MAXSCORE_SHOT = new int[3]
	{
		900,
		2000,
		3500
	};
	public static readonly Color[] CHARA_COLORS = new Color[8]
	{
		new Color(86f / 255f, 0.8862746f, 0.1686275f),
		new Color(1f, 0.3058824f, 32f / 85f),
		new Color(0.3568628f, 172f / 255f, 1f),
		new Color(1f, 0.882353f, 0.04705883f),
		new Color(29f / 51f, 0.3294118f, 41f / 51f),
		new Color(0.01568628f, 0.9490197f, 1f),
		new Color(0.3921569f, 23f / 51f, 0.4235294f),
		new Color(1f, 2f / 3f, 0.1215686f)
	};
	public const string PLAYER_SPRITE_SINGLE_NAME = "_common_c_you";
	public static readonly string[] PLAYER_SPRITE_NAMES = new string[9]
	{
		"_screen_1p",
		"_screen_2p",
		"_screen_3p",
		"_screen_4p",
		"_screen_cp1",
		"_screen_cp2",
		"_screen_cp3",
		"_screen_cp4",
		"_screen_cp5"
	};
	public static readonly string[] CHARA_DEFAULT_SPRITE_NAMES = new string[8]
	{
		"character_yuto_02",
		"character_hina_02",
		"character_ituki_02",
		"character_souta_02",
		"character_takumi_02",
		"character_rin_02",
		"character_akira_02",
		"character_rui_02"
	};
	public static readonly string[] CHARA_OUT_SPRITE_NAMES = new string[8]
	{
		"character_yuto_03",
		"character_hina_03",
		"character_ituki_03",
		"character_souta_03",
		"character_takumi_03",
		"character_rin_03",
		"character_akira_03",
		"character_rui_03"
	};
	public const float GET_POINT_UI_OFFSET_Y = 50f;
	public const float GET_POINT_UI_SINGLE_SCALE = 2f;
	public const float GET_POINT_UI_MULTI_SCALE = 1.5f;
	public const float GET_POINT_UI_MAX_FORWARD_SCALE = 1f;
	public const float GET_POINT_UI_MIN_FORWARD_SCALE = 0.5f;
	public static int TROPHY_BRONZE_SCORE = 1000;
	public static int TROPHY_SILVER_SCORE = 2000;
	public static int TROPHY_GOLD_SCORE = 3500;
}
