using UnityEngine;
public class LegendarySword_Define : MonoBehaviour
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
	public static LegendarySword_GameManager GM => SingletonCustom<LegendarySword_GameManager>.Instance;
	public static LegendarySword_PlayerManager PM => SingletonCustom<LegendarySword_PlayerManager>.Instance;
	public static LegendarySword_UiManager UIM => SingletonCustom<LegendarySword_UiManager>.Instance;
	public static LegendarySword_ControllerManager CM => SingletonCustom<LegendarySword_ControllerManager>.Instance;
	public static LegendarySword_CameraManager CaM => SingletonCustom<LegendarySword_CameraManager>.Instance;
}
