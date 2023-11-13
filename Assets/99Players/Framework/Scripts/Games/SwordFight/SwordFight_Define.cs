using System.Collections.Generic;
public class SwordFight_Define
{
	public enum AiTacticsType
	{
		BALANCE,
		OFFENSIVE,
		PRUDENCE,
		TECHNIC,
		MAX
	}
	public enum AiStrength
	{
		NOOB,
		WEAK,
		COMMON,
		STRONG,
		VERY_STRONG
	}
	public enum PositionType
	{
		C,
		PG,
		SG,
		SF,
		PF,
		MAX
	}
	public enum TeamStatusType
	{
		OFFENSE,
		DEFENSE,
		SPEED,
		STAMINA,
		MAX
	}
	public static string[] uniformColorType = new string[10]
	{
		"WHITE",
		"BLACK",
		"RED",
		"BLUE",
		"GREEN",
		"YELLOW",
		"PINK",
		"ORANGE",
		"GRAY",
		"PURPLE"
	};
	public const string LAYER_CHARACTER = "Character";
	public const string LAYER_GOAL = "Goal";
	public const string LAYER_OBJECT = "Object";
	public const string LAYER_NO_HIT = "NoHit";
	public const string LAYER_BALL = "Ball";
	public const string TAG_GOALPOST = "Goalpost";
	public const string TAG_OBJECT = "Object";
	public const string TAG_BALL = "Ball";
	public const string TAG_GOAL_RING = "GoalHoop";
	public const string TAG_GOAL_NET = "GoalNet";
	public const string TAG_BACKBOARD = "Backboard";
	public const string TAG_FIELD = "Field";
	public const string TAG_CHARACTER = "Character";
	public const string TAG_GOAL = "Goal";
	public const string TAG_GOAL_BARRIER = "GoalBarrier";
	public const string TAG_HORIZONTAL_WALL = "HorizontalWall";
	public const string TAG_VERTICAL_WALL = "VerticalWall";
	public static int MAX_GAME_PLAYER_NUM = 4;
	public static int MAX_GAME_TEAM_NUM = 2;
	public static int MAX_GAME_TEAM_PLAYER_NUM = 2;
	public static int CHAMBARA_CHARACTER_MAX = 2;
	private static int NOW_WINNING_NUM = 0;
	public static bool ALLOW_FRENDLYFIRE = false;
	public static int DEFFENCE_USE_COUNT = 3;
	public static float GAME_TIME_TOTAL = 60f;
	public static int HI_SCORE = 0;
	public static bool IS_TEAM_MODE = false;
	public static int TROPHY_BRONZE_WIN_COUNT = 10;
	public static int TROPHY_SILVER_WIN_COUNT = 20;
	public static int TROPHY_GOLD_WIN_COUNT = 30;
	public static List<int>[] PlayerGroupList = new List<int>[2];
	public static float DEFFENCE_TIME = 1f;
	public static int ConvertLayerNo(string _layerName)
	{
		return 0;
	}
	public static void SetNowWinningNum(int _winningNum)
	{
		NOW_WINNING_NUM = _winningNum;
	}
	public static void AddNowWinningNum()
	{
		NOW_WINNING_NUM++;
	}
	public static int GetNowWinningNum()
	{
		return NOW_WINNING_NUM;
	}
	public static void SetRecordHiScore(int _hiScore)
	{
		HI_SCORE = _hiScore;
	}
	public static int GetRecordHiScore()
	{
		return HI_SCORE;
	}
}
