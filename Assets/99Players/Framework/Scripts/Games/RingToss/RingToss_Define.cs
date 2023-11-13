using UnityEngine;
public class RingToss_Define : MonoBehaviour
{
	public const string TAG_OBJECT = "Object";
	public const string TAG_RUBBERGUN_TARGET = "RubberGun_Target";
	public const string TAG_TABLE = "School";
	public const string TAG_TRAIN = "Airplane";
	public const int LAYER_LOOK_WALL = 134217728;
	public const int LAYER_AI_TARGET = 536870912;
	public static bool DEBUG_FLAG = false;
	public const bool CONTROL_RING_MOVE = true;
	public const bool CONTROL_AIM_USE_POWER = false;
	public const float GAME_TIME = 60f;
	public static int MAX_PLAYER_NUM = 4;
	public static int MAX_TEAM_NUM = 2;
	public static bool IS_BATTLE_MODE = true;
	public static bool IS_TEAM_MODE = false;
	public const int MAX_RING_NUM = 10;
	public const float RING_GRAVITY = -98.1f;
	public const float RING_RADIUS = 1.5f;
	public const float RING_HEIGHT = 0.2f;
	public const float RING_THROW_SPEED = 30f;
	public const float RING_CHARGE_SPEED = 0.8f;
	public const float RING_MOVE_SPEED = 8f;
	public const float AIM_MOVE_SPEED = 8f;
	public const float MAX_THROW_INTERVAL = 5f;
	public const float RING_STOP_MAGNITUDE = 0.1f;
	public const float RING_STOP_END_TIME = 0.5f;
	public const float TARGET_APPEAR_TIME = 1f;
	public const float TARGET_TRAIN_APPEAR_TIME = 5f;
	public const float TARGET_APPEAR_OFFSET_Y = 0.5f;
	public const float TARGET_SHOW_RANDOM_MOVE_LENGTH = 1f;
	public const int TARGET_SHOW_MAX_NUM = 12;
	public const int TARGET_SHOW_GROUP_POINT_MAX_NUM = 4;
	public const int TARGET_HERO_TOY_SHOW_MAX_NUM = 2;
	public const float TRAIN_SPEED_UPDATE_INTERVAL = 3f;
	public const float TRAIN_SLOW_SPEED = 3f;
	public const float TRAIN_NORMAL_SPEED = 5f;
	public const float TRAIN_FAST_SPEED = 7f;
	public const float NEUTRAL_STICK_MAGNITUDE = 0.2f;
	public const float AI_AIM_OK_DISTANCE = 0.02f;
	public const float SINGLE_CPU_VOLUME = 0.5f;
	public static readonly float[] AI_AIM_MAX_RANDOM_DISTANCES = new float[3]
	{
		1.5f,
		1f,
		0.5f
	};
	public static readonly float[] AI_AIM_MIN_RANDOM_DISTANCES = new float[3]
	{
		1f,
		0.5f,
		0f
	};
	public static readonly float[] AI_LR_MOVE_SPEED_MAGS = new float[3]
	{
		0.6f,
		0.8f,
		1f
	};
	public static readonly float[] AI_THROW_DELAY_TIMES = new float[3]
	{
		2f,
		1.5f,
		1f
	};
	public static readonly float[] AI_SKIP_ONE_THROW_TIMES = new float[3]
	{
		6f,
		5f,
		4f
	};
	public static readonly int[] AI_SKIP_ONE_THROW_GET_PER = new int[3]
	{
		40,
		60,
		80
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
	public const string WORLD_PLAYER_SPRITE_SINGLE_NAME = "_common_c_you";
	public static readonly string[] WORLD_PLAYER_SPRITE_NAMES = new string[9]
	{
		"_common_c_1P",
		"_common_c_2P",
		"_common_c_3P",
		"_common_c_4P",
		"_common_c_cp1",
		"_common_c_cp2",
		"_common_c_cp3",
		"_common_c_cp4",
		"_common_c_cp5"
	};
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
	public const float WORLD_PLAYER_SPRITE_VIEW_TIME = 3f;
	public const float WORLD_PLAYER_SPRITE_FADE_TIME = 1f;
	public const float GET_POINT_UI_OFFSET_Y = 50f;
	public const float GET_POINT_UI_SCALE = 1.5f;
	public static int TROPHY_BRONZE_SCORE = 300;
	public static int TROPHY_SILVER_SCORE = 700;
	public static int TROPHY_GOLD_SCORE = 1100;
}
