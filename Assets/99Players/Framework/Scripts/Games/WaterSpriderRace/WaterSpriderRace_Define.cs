using UnityEngine;
public class WaterSpriderRace_Define
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
	public static readonly float GAME_TIME = 180f;
	public static float MEDAL_BRONZE_POINT = 80f;
	public static float MEDAL_SILVER_POINT = 70f;
	public static float MEDAL_GOLD_POINT = 60f;
	public static readonly Color32[] PLAYER_COLOR = new Color32[4]
	{
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue),
		new Color32(16, 85, 251, byte.MaxValue),
		new Color32(254, 202, 47, byte.MaxValue)
	};
	public static WaterSpriderRace_GameManager GM => SingletonCustom<WaterSpriderRace_GameManager>.Instance;
	public static WaterSpriderRace_PlayerManager PM => SingletonCustom<WaterSpriderRace_PlayerManager>.Instance;
	public static WaterSpriderRace_UIManager UIM => SingletonCustom<WaterSpriderRace_UIManager>.Instance;
	public static WaterSpriderRace_ControllerManager CM => SingletonCustom<WaterSpriderRace_ControllerManager>.Instance;
}
