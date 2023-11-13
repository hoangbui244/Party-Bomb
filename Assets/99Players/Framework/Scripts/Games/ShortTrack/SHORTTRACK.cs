public class SHORTTRACK
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
	public enum AiStrength
	{
		WEAK,
		COMMON,
		STRONG
	}
	public const int TOTAL_LAP_CNT = 9;
	public static string LAYER_CHARACTER = "Character";
	public static string MEDAL_BRONZE_TIME = "1:20.0";
	public static string MEDAL_SILVER_TIME = "1:10.0";
	public static string MEDAL_GOLD_TIME = "1:00.0";
	public static ShortTrack_MainGameManager MGM => SingletonCustom<ShortTrack_MainGameManager>.Instance;
	public static ShortTrack_MainCharacterManager MCM => SingletonCustom<ShortTrack_MainCharacterManager>.Instance;
	public static ShortTrack_ControllerManeger CM => SingletonCustom<ShortTrack_ControllerManeger>.Instance;
	public static ShortTrack_CameraManager CAM => SingletonCustom<ShortTrack_CameraManager>.Instance;
	public static ShortTrack_UIManager UIM => SingletonCustom<ShortTrack_UIManager>.Instance;
}
