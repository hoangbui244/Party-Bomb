public class BeachFlag_Define
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
	public static BeachFlag_GameManager GM => SingletonCustom<BeachFlag_GameManager>.Instance;
	public static BeachFlag_PlayerManager PM => SingletonCustom<BeachFlag_PlayerManager>.Instance;
	public static BeachFlag_UIManager UIM => SingletonCustom<BeachFlag_UIManager>.Instance;
	public static BeachFlag_ControllerManager CM => SingletonCustom<BeachFlag_ControllerManager>.Instance;
	public static BeachFlag_CameraManager CaM => SingletonCustom<BeachFlag_CameraManager>.Instance;
}
