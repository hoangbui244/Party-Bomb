using UnityEngine;
public class Canoe_Define : MonoBehaviour
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
	public static float[][] CPU_MOVE_SPEED_MAG = new float[3][]
	{
		new float[2]
		{
			0.9f,
			0.95f
		},
		new float[2]
		{
			0.95f,
			0.98f
		},
		new float[2]
		{
			0.98f,
			1f
		}
	};
	public static float[][] CPU_ROT_SPEED_MAG = new float[3][]
	{
		new float[2]
		{
			0.9f,
			0.95f
		},
		new float[2]
		{
			0.95f,
			0.98f
		},
		new float[2]
		{
			0.98f,
			1f
		}
	};
	public static float[][] CPU_AVOID_OBSTACLE_TIME = new float[3][]
	{
		new float[2]
		{
			3.5f,
			4f
		},
		new float[2]
		{
			3f,
			3.5f
		},
		new float[2]
		{
			2.5f,
			3f
		}
	};
	public static float BRONZE_TIME = 80f;
	public static float SILVER_TIME = 75f;
	public static float GOLD_TIME = 70f;
}
