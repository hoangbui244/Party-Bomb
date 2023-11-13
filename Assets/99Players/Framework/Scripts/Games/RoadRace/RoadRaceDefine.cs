using UnityEngine;
public class RoadRaceDefine : MonoBehaviour
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
	public enum BicycleRaceClass
	{
		JUNIOR,
		NORMAL,
		EXPERT,
		MAX
	}
	public enum PlayBicycleType
	{
		BMX,
		MOUNTAIN,
		ROAD,
		UNIQUE,
		MAX
	}
	public enum HelmetType
	{
		NONE_HELMET,
		NORMAL_HELMET,
		BMX_HELMET,
		CHILDREN_HELMET,
		CHAPLIN_HELMET,
		MOHICAN_HELMET,
		STUDENT_HAT,
		TRACKRACER_HELMET,
		TRIANGLE_WIDTH,
		MAX
	}
	public enum GoggleType
	{
		NONE_GOGGLE,
		NORMAL_GOGGLE_0,
		NORMAL_GOGGLE_1,
		BMX_GOGLE,
		MAX
	}
	public const string LAYER_FLOOR = "Collision_Obj_1";
	public const string LAYER_ACTION_POINT = "Collision_Obj_2";
	public const string LAYER_OBJECT = "Object";
	public const string TAG_CHARA = "Character";
	public const string TAG_CHECK_POINT = "CheckPoint";
	public const string TAG_ACTION_POINT = "ActionPoint";
	public const string TAG_AI_POINT = "AiPoint";
	public const string TAG_FINISH = "Finish";
	public const string TAG_CHARA_WALL = "CharaWall";
	public static float BRONZE_TIME = 55f;
	public static float SILVER_TIME = 50f;
	public static float GOLD_TIME = 45f;
	public static int ConvertLayerMask(string _layerName)
	{
		return 1 << LayerMask.NameToLayer(_layerName);
	}
	public static int ConvertLayerNo(string _layerName)
	{
		return LayerMask.NameToLayer(_layerName);
	}
	public static float easeInQuad(float start, float end, float value)
	{
		end -= start;
		return end * value * value + start;
	}
}
