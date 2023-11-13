using UnityEngine;
public class BeachVolley_Define
{
	public enum AiStrength
	{
		WEAK,
		COMMON,
		STRONG
	}
	public enum AiTacticsType
	{
		BALANCE,
		OFFENSIVE,
		PRUDENCE,
		TECHNIC,
		MAX
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
		MAX
	}
	public enum PositionType
	{
		BR,
		FR,
		FC,
		FL,
		BL,
		BC,
		LIBERO,
		MAX
	}
	public struct StatusType
	{
		public int offense;
		public int defense;
		public int speed;
		public int jump;
	}
	public enum BallSpeedType
	{
		SLOW,
		NORMAL,
		FAST,
		VERY_FAST,
		MAX
	}
	public enum CameraMode
	{
		VERTICAL,
		HORIZONTAL
	}
	public enum Rarity
	{
		NORMAL,
		RARE,
		SUPER_RARE,
		MAX
	}
	public const int SCHOOL_NUM = 8;
	public static int PLAYER_NUM = 1;
	public static int CPU_NUM = 3;
	public static int MEMBER_NUM = 4;
	public static readonly float GAME_TIME = 120f;
	public static float MEDAL_BRONZE_POINT = 3000f;
	public static float MEDAL_SILVER_POINT = 4000f;
	public static float MEDAL_GOLD_POINT = 5000f;
	public static readonly Color32[] PLAYER_COLOR = new Color32[4]
	{
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue),
		new Color32(16, 85, 251, byte.MaxValue),
		new Color32(254, 202, 47, byte.MaxValue)
	};
	public static readonly Color32[] TEAM_COLOR = new Color32[2]
	{
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue)
	};
	public static readonly int TEAM_STATUS_SCALE_NUM = 10;
	public static readonly float[] BALL_SPEED_TABLE = new float[5]
	{
		0.7f,
		1f,
		1.1f,
		0.5f,
		1f
	};
	public static readonly int TEAM_MEMBER_NUM_MAX = 7;
	private static int[] PLAY_SET_NUM = new int[3]
	{
		1,
		3,
		5
	};
	private static int[] PLAY_POINT_NUM = new int[4]
	{
		5,
		10,
		15,
		25
	};
	public static int[] PLAY_POINT_LAST_SET = new int[4]
	{
		3,
		5,
		10,
		15
	};
	public static readonly float GAUGE_POWER = 0.7f;
	public static readonly string TAG_BALL = "Ball";
	public static readonly string TAG_FIELD = "Field";
	public static readonly string TAG_GOAL = "Goal";
	public static readonly string TAG_CHARACTER = "Character";
	public static readonly string TAG_OBJECT = "Object";
	public static readonly string TAG_NET = "Net";
	public static readonly string TAG_CHECK_COLLIDER = "CheckCollider";
	public static readonly string TAG_VERTICAL_WALL = "VerticalWall";
	public static readonly string TAG_HORIZONTAL_WALL = "HorizontalWall";
	public static readonly string TAG_ANTENNA = "Antenna";
	public static readonly string LAYER_OBJECT = "Object";
	public static readonly string LAYER_CHARACTER = "Character";
	public static readonly string LAYER_CHARA_IGNORE_BALL = "CharaIgnoreBall";
	public static readonly string LAYER_GOAL = "Goal";
	public static readonly string LAYER_NO_HIT = "NoHit";
	public static readonly string LAYER_INVISIBLE_CHARA = "InvisibleChara";
	public static readonly string LAYER_CHARA_WALL = "CharaWall";
	private static CameraMode selectCameraMode = CameraMode.HORIZONTAL;
	private static bool isFirstPlayerFirstAttack = true;
	public static BeachVolley_ControllerManager CM => SingletonCustom<BeachVolley_ControllerManager>.Instance;
	public static BeachVolley_BallManager BM => SingletonCustom<BeachVolley_BallManager>.Instance;
	public static BeachVolley_Ball Ball => SingletonCustom<BeachVolley_BallManager>.Instance.GetBall();
	public static BeachVolley_MainGameManager MGM => SingletonCustom<BeachVolley_MainGameManager>.Instance;
	public static BeachVolley_MainCharacterManager MCM => SingletonCustom<BeachVolley_MainCharacterManager>.Instance;
	public static BeachVolley_FieldManager FM => SingletonCustom<BeachVolley_FieldManager>.Instance;
	public static BeachVolley_GameUiManager GUM => SingletonCustom<BeachVolley_GameUiManager>.Instance;
	public static BeachVolley_MetaAI MAI => SingletonCustom<BeachVolley_MainCharacterManager>.Instance.MetaAi;
	public static int Return_team_infield_num()
	{
		return 2;
	}
	public static int ConvertLayerNo(string _layerName)
	{
		return LayerMask.NameToLayer(_layerName);
	}
	public static float GetBallSpeedMag()
	{
		return BALL_SPEED_TABLE[0];
	}
	public static void SetSelectCameraMode(CameraMode _mode)
	{
		selectCameraMode = _mode;
	}
	public static CameraMode GetSelectCameraMode()
	{
		return selectCameraMode;
	}
	public static bool CheckSelectCameraMode(CameraMode _mode)
	{
		return selectCameraMode == _mode;
	}
	public static void SetSelectPointNumIndex(int _selectPointNum, int _gameMode)
	{
	}
	public static int GetSelectPointNum(bool _indexConvert = false)
	{
		return 10;
	}
	public static void SetSelectSetNumIndex(int _selectSetNum, int _gameMode)
	{
	}
	public static int GetSelectSetNum(bool _indexConvert = false)
	{
		return 1;
	}
	public static int GetPlaySetNum(int _no)
	{
		return PLAY_SET_NUM[_no];
	}
	public static void SetFirstPlayerFirstAttack(bool _flg)
	{
		isFirstPlayerFirstAttack = _flg;
	}
	public static bool IsFirstPlayerFirstAttack()
	{
		return isFirstPlayerFirstAttack;
	}
	public static float GetBallSize()
	{
		return 1f;
	}
	public static float GetBallAngleDrag()
	{
		return 0.2f;
	}
	public static float GetBallDrag()
	{
		return 0.05f;
	}
}
