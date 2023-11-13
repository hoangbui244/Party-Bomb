using UnityEngine;
public class FireworksDefine
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
		CPU_5
	}
	public enum TeamType
	{
		TEAM_A,
		TEAM_B,
		TEAM_C,
		TEAM_D
	}
	public const float GAME_TIME = 90f;
	public const float NEUTRAL_STICK_MAGNITUDE = 0.2f;
	public static readonly int COLLECTION_BRONZE = 4000;
	public static readonly int COLLECTION_SILVER = 4500;
	public static readonly int COLLECTION_GOLD = 6000;
	public static readonly int BALL_MAX = 3;
	public static readonly int SCORE_SET = 150;
	public static readonly int SCORE_SET_LAST = 50;
	public static readonly int SCORE_SET_DOUBLE = 200;
	public static readonly int SCORE_SET_TRIPLE = 250;
	public static readonly int MAX_JOIN_MEMBER_NUM = 4;
	public static readonly int SCORE_MAX = 9999;
	public static readonly Color[] CHARA_COLORS = new Color[8]
	{
		new Color(86f / 255f, 0.8862746f, 0.1686275f),
		new Color(1f, 0.3058824f, 32f / 85f),
		new Color(0.3568628f, 172f / 255f, 1f),
		new Color(1f, 0.882353f, 0.04705883f),
		new Color(29f / 51f, 0.3294118f, 41f / 51f),
		new Color(0.01568628f, 0.9490197f, 1f),
		new Color(0.6320754f, 0.6320754f, 0.6320754f),
		new Color(1f, 2f / 3f, 0.1215686f)
	};
}
