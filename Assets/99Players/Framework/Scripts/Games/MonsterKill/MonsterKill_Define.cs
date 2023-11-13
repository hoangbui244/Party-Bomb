using UnityEngine;
public class MonsterKill_Define : MonoBehaviour
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
	public static float GAME_TIME = 90f;
	public const float NEUTRAL_STICK_MAGNITUDE = 0.2f;
	public static float[][] CPU_NEXT_SEARCH_TARGET_ENEMY_INTERVAL = new float[3][]
	{
		new float[2]
		{
			0.5f,
			0.7f
		},
		new float[2]
		{
			0.3f,
			0.5f
		},
		new float[2]
		{
			0.1f,
			0.3f
		}
	};
	public static float[][] CPU_MAGIC_ATTACK_INPUT_INTERVAL = new float[3][]
	{
		new float[2]
		{
			15f,
			30f
		},
		new float[2]
		{
			10f,
			20f
		},
		new float[2]
		{
			5f,
			10f
		}
	};
	public static float[][] CPU_MAGIC_ATTACK_INPUT_PROBABILITY = new float[3][]
	{
		new float[2]
		{
			0.4f,
			0.6f
		},
		new float[2]
		{
			0.6f,
			0.8f
		},
		new float[2]
		{
			0.8f,
			1f
		}
	};
	public static float[][] CPU_SWORD_ATTACK_INPUT_INTERVAL = new float[3][]
	{
		new float[2]
		{
			0.3f,
			0.5f
		},
		new float[2]
		{
			0.2f,
			0.4f
		},
		new float[2]
	};
	public static float[][] CPU_DASH_INPUT_PROBABILITY = new float[3][]
	{
		new float[2]
		{
			0.2f,
			0.5f
		},
		new float[2]
		{
			0.4f,
			0.7f
		},
		new float[2]
		{
			0.8f,
			1f
		}
	};
	public static float[][] CPU_STOP_DASH_STAMINA = new float[3][]
	{
		new float[2],
		new float[2]
		{
			0f,
			0.2f
		},
		new float[2]
		{
			0.05f,
			0.1f
		}
	};
	public static float[][] CPU_IN_ATTACK_RANGE_ATTACK_PROBABILITY = new float[3][]
	{
		new float[2]
		{
			0.3f,
			0.5f
		},
		new float[2]
		{
			0.5f,
			0.7f
		},
		new float[2]
		{
			0.8f,
			1f
		}
	};
	public static float[] CPU_LIMIT_MOVE_SPEED = new float[3]
	{
		0.9f,
		0.95f,
		1f
	};
	public static int BRONZE_BORDER = 4500;
	public static int SILVER_BORDER = 5500;
	public static int GOLD_BORDER = 6500;
}
