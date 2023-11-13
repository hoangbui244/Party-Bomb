public class MorphingRace_Define
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
	public const float NEUTRAL_STICK_MAGNITUDE = 0.2f;
	public static float[] CPU_MOVE_INPUT_INTERVAL = new float[3]
	{
		0.55f,
		0.525f,
		0.5f
	};
	public static float[] CPU_MOVE_INPUT_MIN_INTERVAL = new float[3]
	{
		0.225f,
		0.2f,
		0.175f
	};
	public static float[] CPU_MOVE_INPUT_MIN_INTERVAL_UNTIL_TIME = new float[3]
	{
		3f,
		2.5f,
		2f
	};
	public static float[] CPU_OBSTACLE_DECELERATE_MAGNIFICATION = new float[3]
	{
		0.4f,
		0.5f,
		0.6f
	};
	public static float[] CPU_MOUSE_NAV_MESH_OBSTACLE_ACTIVE_PROBABILITY = new float[3]
	{
		0.8f,
		0.5f,
		0.2f
	};
	public static float[] CPU_EAGLE_TARGET_DIFF_RANGE = new float[3]
	{
		1f,
		0.75f,
		0.5f
	};
	public static float[] CPU_FISH_TARGET_DIFF_RANGE = new float[3]
	{
		0.45f,
		0.3f,
		0.15f
	};
	public static float[] CPU_DOG_WHETHER_JUMP_PROBABILITY = new float[3]
	{
		0.3f,
		0.5f,
		0.7f
	};
	public static float[] CPU_DOG_OBSTACLE_WAIT_JUMP_TIME = new float[3]
	{
		0.5f,
		0.35f,
		0.2f
	};
	public static float BRONZE_TIME = 55f;
	public static float SILVER_TIME = 50f;
	public static float GOLD_TIME = 45f;
}
