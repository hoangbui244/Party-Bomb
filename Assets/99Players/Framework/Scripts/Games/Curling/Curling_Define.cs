public class Curling_Define
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
	public static int GAME_CNT = 1;
	public static int THROW_CNT = 4;
	public static int SAME_TEAM_PLAYER_CNT = 2;
	public const float NEUTRAL_STICK_MAGNITUDE = 0.2f;
	public static float[] CPU_TARGET_ANGLE = new float[3]
	{
		0.5f,
		0.35f,
		0.2f
	};
	public static float[] CPU_TARGET_POS_DIFF = new float[3]
	{
		0.75f,
		0.5f,
		0.25f
	};
	public static float[] CPU_TARGET_POWER_DIFF = new float[3]
	{
		0.7f,
		0.85f,
		1f
	};
	public static float[] TARGET_STONE_PROBABILITY = new float[3]
	{
		0.1f,
		0.5f,
		0.95f
	};
	public static float[] HOUSE_SWEEP_PROBABILITY = new float[3]
	{
		0.1f,
		0.5f,
		0.95f
	};
	public static float[] HOUSE_SWEEP_INTERVAL = new float[3]
	{
		0.3f,
		0.2f,
		0.1f
	};
}
