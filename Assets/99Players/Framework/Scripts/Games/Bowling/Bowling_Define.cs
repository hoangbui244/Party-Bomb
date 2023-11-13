using UnityEngine;
public class Bowling_Define : MonoBehaviour
{
	public enum ThrowType
	{
		RIGHT_S,
		STRAIGHT,
		LEFT_S,
		MAX
	}
	public enum OperationState
	{
		NONE,
		BALL_MOVE,
		VECTOR_SELECT,
		THROW
	}
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
	public enum ThrowResultEffectType
	{
		Strike,
		Spare,
		Gutter,
		Miss
	}
	public static int PLAYER_NUM = 1;
	public static int CPU_NUM = 3;
	public static int MEMBER_NUM = 4;
	public static readonly int MAX_COOP_TEAM_NUM = 2;
	public static readonly int PLAY_FRAME_NUM = 3;
	public static int BALL_POUND = 11;
	public static int[] BALL_PARAM = new int[3]
	{
		1,
		1,
		1
	};
	public static int MEDAL_BRONZE_POINT = 30;
	public static int MEDAL_SILVER_POINT = 60;
	public static int MEDAL_GOLD_POINT = 90;
	public static readonly Color32[] USER_COLOR = new Color32[8]
	{
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue),
		new Color32(16, 85, 251, byte.MaxValue),
		new Color32(254, 202, 47, byte.MaxValue),
		new Color32(160, 90, 168, byte.MaxValue),
		new Color32(0, 229, 211, byte.MaxValue),
		new Color32(149, 153, 130, byte.MaxValue),
		new Color32(250, 120, 0, byte.MaxValue)
	};
	public static readonly Color32[] TEAM_COLOR = new Color32[2]
	{
		new Color32(105, 195, 81, byte.MaxValue),
		new Color32(205, 56, 59, byte.MaxValue)
	};
	public static readonly Color32[] CHARA_COLOR = new Color32[8]
	{
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue),
		new Color32(16, 85, 251, byte.MaxValue),
		new Color32(254, 202, 47, byte.MaxValue),
		new Color32(160, 90, 168, byte.MaxValue),
		new Color32(0, 229, 211, byte.MaxValue),
		new Color32(149, 153, 130, byte.MaxValue),
		new Color32(250, 120, 0, byte.MaxValue)
	};
	public static string TAG_BOWLING_BALL = "Bowling_Ball";
	public static string TAG_BOWLING_HOLE = "Bowling_Hole";
	public static string TAG_BOWLING_PIN = "Bowling_Pin";
	public static string TAG_BOWLING_GUTTER = "Bowling_Gutter";
	public static string TAG_BOWLING_LANE = "Bowling_Lane";
	public static string LAYER_NO_HIT = "NoHit";
	public static Bowling_MainGameManager MGM => SingletonCustom<Bowling_MainGameManager>.Instance;
	public static Bowling_MainStageManager MSM => SingletonCustom<Bowling_MainStageManager>.Instance;
	public static Bowling_MainPlayerManager MPM => SingletonCustom<Bowling_MainPlayerManager>.Instance;
	public static Bowling_ControllerManager CM => SingletonCustom<Bowling_ControllerManager>.Instance;
	public static Bowling_GameUIManager GUIM => SingletonCustom<Bowling_GameUIManager>.Instance;
	public static Color GetUserColor(UserType _userType)
	{
		Color result = Color.white;
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			if (_userType <= UserType.PLAYER_4)
			{
				result = USER_COLOR[(int)_userType];
			}
			else if (PLAYER_NUM == 1)
			{
				switch (_userType)
				{
				case UserType.CPU_1:
					result = USER_COLOR[1];
					break;
				case UserType.CPU_2:
					result = USER_COLOR[2];
					break;
				case UserType.CPU_3:
					result = USER_COLOR[3];
					break;
				}
			}
			else if (PLAYER_NUM == 2)
			{
				switch (_userType)
				{
				case UserType.CPU_1:
					result = USER_COLOR[2];
					break;
				case UserType.CPU_2:
					result = USER_COLOR[3];
					break;
				}
			}
			else if (PLAYER_NUM == 3 && _userType == UserType.CPU_1)
			{
				result = USER_COLOR[3];
			}
		}
		else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			if (_userType <= UserType.PLAYER_4)
			{
				result = USER_COLOR[(int)_userType];
			}
			else if (PLAYER_NUM == 2)
			{
				switch (_userType)
				{
				case UserType.CPU_1:
					result = USER_COLOR[2];
					break;
				case UserType.CPU_2:
					result = USER_COLOR[3];
					break;
				}
			}
			else if (PLAYER_NUM == 3)
			{
				switch (_userType)
				{
				case UserType.CPU_1:
					result = USER_COLOR[3];
					break;
				case UserType.CPU_2:
					result = USER_COLOR[4];
					break;
				case UserType.CPU_3:
					result = USER_COLOR[5];
					break;
				case UserType.CPU_4:
					result = USER_COLOR[6];
					break;
				case UserType.CPU_5:
					result = USER_COLOR[7];
					break;
				}
			}
			else if (PLAYER_NUM == 4)
			{
				switch (_userType)
				{
				case UserType.CPU_1:
					result = USER_COLOR[4];
					break;
				case UserType.CPU_2:
					result = USER_COLOR[5];
					break;
				case UserType.CPU_3:
					result = USER_COLOR[6];
					break;
				case UserType.CPU_4:
					result = USER_COLOR[7];
					break;
				}
			}
		}
		else
		{
			result = USER_COLOR[(int)_userType];
		}
		return result;
	}
	public static int GetConvertCPUNo(UserType _userType)
	{
		int result = (int)_userType;
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			if (_userType <= UserType.PLAYER_4)
			{
				result = (int)_userType;
			}
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
			{
				switch (_userType)
				{
				case UserType.CPU_1:
					result = 1;
					break;
				case UserType.CPU_2:
					result = 2;
					break;
				case UserType.CPU_3:
					result = 3;
					break;
				}
			}
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2)
			{
				switch (_userType)
				{
				case UserType.CPU_1:
					result = 2;
					break;
				case UserType.CPU_2:
					result = 3;
					break;
				}
			}
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && _userType == UserType.CPU_1)
			{
				result = 3;
			}
		}
		else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			if (_userType <= UserType.PLAYER_4)
			{
				result = (int)_userType;
			}
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2)
			{
				switch (_userType)
				{
				case UserType.CPU_1:
					result = 2;
					break;
				case UserType.CPU_2:
					result = 3;
					break;
				}
			}
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
			{
				switch (_userType)
				{
				case UserType.CPU_1:
					result = 3;
					break;
				case UserType.CPU_2:
					result = 4;
					break;
				case UserType.CPU_3:
					result = 5;
					break;
				case UserType.CPU_4:
					result = 6;
					break;
				case UserType.CPU_5:
					result = 7;
					break;
				}
			}
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4)
			{
				switch (_userType)
				{
				case UserType.CPU_1:
					result = 4;
					break;
				case UserType.CPU_2:
					result = 5;
					break;
				case UserType.CPU_3:
					result = 6;
					break;
				case UserType.CPU_4:
					result = 7;
					break;
				}
			}
		}
		else
		{
			result = (int)_userType;
		}
		return result;
	}
}
