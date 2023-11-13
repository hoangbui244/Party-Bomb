public class SB {
    public enum AiStrength {
        WEAK,
        COMMON,
        STRONG
    }
    public enum UserType {
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
    public enum TeamType {
        TEAM_A,
        TEAM_B,
        TEAM_C,
        TEAM_D
    }
    public enum StandType {
        Stand_0,
        Stand_1,
        Stand_2,
        Stand_3,
        MAX
    }
    public static int MAX_PLAYER_NUM = 4;
    public static int MAX_TEAM_NUM = 4;
    public static int LAST_STAND_NO = -1;
    public static int PREVIOUS_LAST_STAND_NO = -1;
    public static int MEDAL_BRONZE_POINT = 200;
    public static int MEDAL_SILVER_POINT = 350;
    public static int MEDAL_GOLD_POINT = 500;
    public static float WEAK_CHARGE_TIME_ERROR = 0.09f;
    public static float COMMON_CHARGE_TIME_ERROR = 0.045f;
    public static float STRONG_CHARGE_TIME_ERROR = 0.01f;
    public static SmartBall_MainGameManager MGM => SingletonCustom<SmartBall_MainGameManager>.Instance;
    public static SmartBall_MainCharacterManager MCM => SingletonCustom<SmartBall_MainCharacterManager>.Instance;
    public static SmartBall_ControllerManager CM => SingletonCustom<SmartBall_ControllerManager>.Instance;
    public static SmartBall_GameUIManager GUIM => SingletonCustom<SmartBall_GameUIManager>.Instance;
}
