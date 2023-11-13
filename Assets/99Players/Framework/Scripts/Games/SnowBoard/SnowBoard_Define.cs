using UnityEngine;
public class SnowBoard_Define
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
		CPU_5,
		CPU_6,
		CPU_7
	}
	public static int PLAYER_NUM = 1;
	public static int CPU_NUM = 7;
	public static int MEMBER_NUM = 8;
	public static readonly float GAME_TIME = 180f;
	public static float MEDAL_BRONZE_POINT = 85f;
	public static float MEDAL_SILVER_POINT = 75f;
	public static float MEDAL_GOLD_POINT = 65f;
	public static readonly Color32[] PLAYER_COLOR = new Color32[4]
	{
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue),
		new Color32(16, 85, 251, byte.MaxValue),
		new Color32(254, 202, 47, byte.MaxValue)
	};
	public static SnowBoard_GameManager GM => SingletonCustom<SnowBoard_GameManager>.Instance;
	public static SnowBoard_PlayerManager PM => SingletonCustom<SnowBoard_PlayerManager>.Instance;
	public static SnowBoard_UIManager UIM => SingletonCustom<SnowBoard_UIManager>.Instance;
	public static SnowBoard_ControllerManager CM => SingletonCustom<SnowBoard_ControllerManager>.Instance;
}
