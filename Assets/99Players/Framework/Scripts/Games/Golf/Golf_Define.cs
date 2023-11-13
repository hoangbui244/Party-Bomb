using UnityEngine;
public class Golf_Define : MonoBehaviour
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
	public static int BEFORE_HOLE_IDX = -1;
	public static int BEFORE_HOLE_FIELD_IDX = -1;
	public static int TOTAL_GAME_CNT = 8;
	public static float CONVERSION_YARD = 1f;
	public static float[][] CPU_SHOT_ANGLE = new float[3][]
	{
		new float[2]
		{
			-7.5f,
			7.5f
		},
		new float[2]
		{
			-5f,
			5f
		},
		new float[2]
		{
			-2.5f,
			2.5f
		}
	};
	public static float[][] CPU_SHOT_POWER = new float[3][]
	{
		new float[2]
		{
			0.7f,
			0.95f
		},
		new float[2]
		{
			0.75f,
			0.9f
		},
		new float[2]
		{
			0.8f,
			0.85f
		}
	};
	public static float[][] CPU_SHOT_IMPACT = new float[3][]
	{
		new float[2]
		{
			0.5f,
			1f
		},
		new float[2]
		{
			0.2f,
			0.5f
		},
		new float[2]
		{
			0f,
			0.25f
		}
	};
	public static int BRONZE_POINT = 3200;
	public static int SILVER_POINT = 3500;
	public static int GOLD_POINT = 3800;
}
