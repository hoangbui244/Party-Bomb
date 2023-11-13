using UnityEngine;
public class Takoyaki_Define
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
	public enum TeamType
	{
		TEAM_A,
		TEAM_B,
		TEAM_C,
		TEAM_D
	}
	public enum TakoBallBakeStatus
	{
		HalfBake,
		Bake,
		OverBake
	}
	public static int PLAYER_NUM = 1;
	public static int CPU_NUM = 3;
	public static int MEMBER_NUM = 4;
	public static readonly int MAX_COOP_TEAM_NUM = 2;
	public static readonly int MAX_JOIN_MEMBER_NUM = 4;
	public static readonly float GAME_TIME = 120f;
	public static int MEDAL_BRONZE_POINT = 10;
	public static int MEDAL_SILVER_POINT = 30;
	public static int MEDAL_GOLD_POINT = 50;
	public static readonly Color32[] PLAYER_COLOR = new Color32[4]
	{
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue),
		new Color32(16, 85, 251, byte.MaxValue),
		new Color32(254, 202, 47, byte.MaxValue)
	};
	public static readonly Color32[] TEAM_COLOR = new Color32[2]
	{
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue)
	};
	public static string TAG_BOWLING_BALL = "Bowling_Ball";
	public static string TAG_BOWLING_HOLE = "Bowling_Hole";
	public static string TAG_BOWLING_PIN = "Bowling_Pin";
	public static string TAG_BOWLING_GUTTER = "Bowling_Gutter";
	public static string TAG_BOWLING_LANE = "Bowling_Lane";
	public static string LAYER_NO_HIT = "NoHit";
	public static Takoyaki_GameManager GM => SingletonCustom<Takoyaki_GameManager>.Instance;
	public static Takoyaki_PlayerManager PM => SingletonCustom<Takoyaki_PlayerManager>.Instance;
	public static Takoyaki_UIManager UIM => SingletonCustom<Takoyaki_UIManager>.Instance;
	public static Takoyaki_ControllerManager CM => SingletonCustom<Takoyaki_ControllerManager>.Instance;
}
