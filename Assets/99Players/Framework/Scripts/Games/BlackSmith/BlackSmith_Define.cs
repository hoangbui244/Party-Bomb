using UnityEngine;
public class BlackSmith_Define : MonoBehaviour
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
		CPU_7,
		MAX
	}
	public static float GAME_TIME = 90f;
	public static float[][] CPU_INPUT_PERFECT_PROBABILITY = new float[3][]
	{
		new float[2]
		{
			0f,
			0.2f
		},
		new float[2]
		{
			0.2f,
			0.6f
		},
		new float[2]
		{
			0.6f,
			1f
		}
	};
	public static float[][] CPU_INPUT_NICE_PROBABILITY = new float[3][]
	{
		new float[2]
		{
			0f,
			0.2f
		},
		new float[2]
		{
			0.2f,
			0.6f
		},
		new float[2]
		{
			0.6f,
			1f
		}
	};
	public static float[][] CPU_INPUT_BAD_PROBABILITY = new float[3][]
	{
		new float[2]
		{
			0f,
			0.2f
		},
		new float[2]
		{
			0.2f,
			0.6f
		},
		new float[2]
		{
			0.6f,
			1f
		}
	};
	public static float[][] CPU_INPUT_BETTER_EVALUATION_PROBABILITY = new float[3][]
	{
		new float[2]
		{
			0f,
			0.2f
		},
		new float[2]
		{
			0.2f,
			0.6f
		},
		new float[2]
		{
			0.6f,
			1f
		}
	};
	public static int BRONZE_BORDER = 8;
	public static int SILVER_BORDER = 10;
	public static int GOLD_BORDER = 12;
}
