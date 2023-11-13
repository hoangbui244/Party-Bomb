using UnityEngine;
public class HandSeal_Define
{
	public enum AiStrength
	{
		WEAK,
		COMMON,
		STRONG
	}
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
		CPU_5
	}
	public static int PLAYER_NUM = 1;
	public static int CPU_NUM = 3;
	public static int MEMBER_NUM = 4;
	public static readonly float GAME_TIME = 120f;
	public static float MEDAL_BRONZE_POINT = 3000f;
	public static float MEDAL_SILVER_POINT = 4000f;
	public static float MEDAL_GOLD_POINT = 5000f;
	public static readonly Color32[] PLAYER_COLOR = new Color32[4]
	{
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue),
		new Color32(16, 85, 251, byte.MaxValue),
		new Color32(254, 202, 47, byte.MaxValue)
	};
	public static HandSeal_GameManager GM => SingletonCustom<HandSeal_GameManager>.Instance;
	public static HandSeal_PlayerManager PM => SingletonCustom<HandSeal_PlayerManager>.Instance;
	public static HandSeal_UIManager UIM => SingletonCustom<HandSeal_UIManager>.Instance;
	public static HandSeal_ControllerManager CM => SingletonCustom<HandSeal_ControllerManager>.Instance;
}
