public class RockClimbing_Define
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
	public static float[] CPU_CLIMBING_INTERVAL = new float[3]
	{
		0.5f,
		0.475f,
		0.4f
	};
	public static float[] CPU_CLIMBING_MIN_INTERVAL = new float[3]
	{
		0.225f,
		0.175f,
		0.125f
	};
	public static float[] CPU_CLIMBING_MAX_SPEED_TIME = new float[3]
	{
		2f,
		1.5f,
		1f
	};
	public static float[] CPU_GRAPPLINNG_HOOK_NEAR_POINT = new float[3]
	{
		0.3f,
		0.5f,
		0.7f
	};
	public static float[] CPU_GRAPPLINNG_HOOK_THROW_WAIT_TIME = new float[3]
	{
		0.6f,
		0.5f,
		0.4f
	};
	public static float[] CPU_AVOID_PROBABILITY = new float[3]
	{
		0.3f,
		0.5f,
		0.7f
	};
	public static float BRONZE_TIME = 65f;
	public static float SILVER_TIME = 60f;
	public static float GOLD_TIME = 55f;
}
