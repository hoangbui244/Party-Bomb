using UnityEngine;
public class DragonBattleDefine
{
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
		CPU_5,
		CPU_6,
		CPU_7
	}
	public enum EnemyType
	{
		BAT,
		DRAGON,
		DRAGON_BOSS,
		BEHOLDER,
		FLYING_DEMON,
		SPECTER,
		EVILMAGE,
		MAX
	}
	public enum ModelType
	{
		RPG_MONSTER_WAVE,
		RPG_MONSTER_WAVE_02,
		OTHER,
		MAX
	}
	public const float NEUTRAL_STICK_MAGNITUDE = 0.2f;
	public const float STEP_HEIGHT = 0f;
	public static readonly int[] POINT_TYPE_LIST = new int[9]
	{
		2000,
		1000,
		500,
		300,
		250,
		200,
		150,
		100,
		50
	};
	public const int GOAL_SCORE_0 = 500;
	public const int GOAL_SCORE_1 = 300;
	public const int GOAL_SCORE_2 = 100;
	public const int GOAL_SCORE_3 = 50;
	public static readonly ModelType[] MODE_TYPE_ENEMY = new ModelType[7]
	{
		ModelType.RPG_MONSTER_WAVE,
		ModelType.RPG_MONSTER_WAVE,
		ModelType.OTHER,
		ModelType.RPG_MONSTER_WAVE_02,
		ModelType.RPG_MONSTER_WAVE_02,
		ModelType.RPG_MONSTER_WAVE_02,
		ModelType.RPG_MONSTER_WAVE
	};
	public static readonly int[] SCORE_KILL_ENEMY = new int[7]
	{
		100,
		200,
		2000,
		100,
		1000,
		300,
		200
	};
	public const int SCORE_BOSS_HIT_BASE = 150;
	public static readonly int[] SCORE_BOSS_HIT_LIST = new int[4]
	{
		150,
		200,
		250,
		300
	};
	public static readonly int[] SCORE_ITEM = new int[4]
	{
		50,
		100,
		150,
		300
	};
	public const int SCORE_SHURIKEN_HIT = 50;
	public const int SCORE_SWORD_HIT = 50;
	public const string LAYER_CHARACTER = "Character";
	public const string LAYER_WATER = "Water";
	public const string LAYER_FIELD = "Field";
	public const string LAYER_HIT_CHARA_ONLY = "HitCharacterOnly";
	public const string TAG_CHECK_POINT = "CheckPoint";
	public const string TAG_COIN = "Coin";
	public const string TAG_NINJA = "Ninja";
	public const string TAG_GOLD_BOX = "Box";
	public const string TAG_SHURIKEN = "Ball";
	public const string TAG_PLAYER = "Player";
	public const string TAG_FAILURE = "Failure";
	public const string TAG_GOAL = "Goal";
	public const string TAG_VERTICAL_WALL = "VerticalWall";
	public const string TAG_HORIZONTAL_WALL = "HorizontalWall";
	public const string TAG_LONG_RANGE_ATTACK = "Ball";
	public const string OBJ_NAME_DESTROY_WAIT = "DestroyWait";
	public const string OBJ_NAME_WATER = "Water";
	public static int ConvertLayerNo(string _layerName)
	{
		return LayerMask.NameToLayer(_layerName);
	}
	public static int ConvertLayerMask(string _layerName)
	{
		return 1 << LayerMask.NameToLayer(_layerName);
	}
}
