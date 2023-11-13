using UnityEngine;
public class ShavedIce_Define
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
	public enum PutOnDifficulty
	{
		Easy,
		Normal,
		Hard
	}
	public enum StoneSize
	{
		Small,
		Medium,
		Large,
		ExtraLarge
	}
	public enum StoneType
	{
		Stone_0,
		Stone_1,
		Stone_2,
		Stone_3,
		Stone_4,
		Stone_5,
		Stone_6,
		MAX
	}
	public enum StoneShape
	{
		Flat,
		Distorted
	}
	public enum SoundType
	{
		LIGHT,
		HEAVY
	}
	public static readonly bool IS_DEBUG = true;
	public static int MAX_PLAYER_NUM = 4;
	public static int MAX_TEAM_NUM = 4;
	public static readonly float GAME_TIME = 60f;
	public static readonly float ICE_FX_MOVE_START_TIME = 2f;
	public static readonly float ICE_FX_MOVE_INTERVAL_TIME = 0.01f;
	public static readonly float ICE_FX_STOP_ADVANCE_TIME = 2f;
	public static float MEDAL_BRONZE_HEIGHT = 200f;
	public static float MEDAL_SILVER_HEIGHT = 250f;
	public static float MEDAL_GOLD_HEIGHT = 300f;
	public static string LAYER_FIELD = "Field";
	public static string TAG_OBJECT = "Object";
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
		new Color32(52, 199, 60, byte.MaxValue),
		new Color32(226, 33, 50, byte.MaxValue)
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
	public static ShavedIce_GameManager GM => SingletonCustom<ShavedIce_GameManager>.Instance;
	public static ShavedIce_PlayerManager PM => SingletonCustom<ShavedIce_PlayerManager>.Instance;
	public static ShavedIce_ControllerManager CM => SingletonCustom<ShavedIce_ControllerManager>.Instance;
	public static ShavedIce_UIManager UIM => SingletonCustom<ShavedIce_UIManager>.Instance;
	public static Color GetUserColor(UserType _userType)
	{
		Color result = Color.white;
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			if (_userType <= UserType.PLAYER_4)
			{
				result = USER_COLOR[(int)_userType];
			}
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
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
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2)
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
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && _userType == UserType.CPU_1)
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
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2)
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
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
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
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4)
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
