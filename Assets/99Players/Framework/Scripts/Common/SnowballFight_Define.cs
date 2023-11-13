public class SnowballFight_Define {
    public enum AiTacticsType {
        BALANCE,
        OFFENSIVE,
        PRUDENCE,
        TECHNIC,
        MAX
    }
    public enum AiStrength {
        WEAK,
        COMMON,
        STRONG
    }
    public enum PositionType {
        IN_FIELD,
        OUT_FIELD,
        MAX
    }
    public const string TAG_BALL = "Ball";
    public const string TAG_CHARACTER = "Character";
    public const string TAG_PREV_HIT_COLLIDER = "GoalHoop";
    public const string TAG_DEAD_SNOW_AREA = "GoalNet";
    public const string TAG_FIELD = "Field";
    public const string TAG_OBJECT = "Object";
    public const string TAG_AI_ROOT = "Airplane";
    public const string LAYER_CHARACTER = "Character";
    public const string LAYER_NO_HIT = "NoHit";
    public const string LAYER_INVISIBLE_CHARA = "InvisibleChara";
    public const string LAYER_DODGE_CHARA = "DodgeCharacter";
    public const int ONE_HIT_POINT = 100;
    public const int MAX_HP = 5;
    public const int MAX_BALL_COUNT = 5;
    public const int TEAM_NUM = 4;
    public const int FREE_PLAY_TEAM_NUM = 8;
    public const int TEAM_MEMBER_NUM_MAX = 1;
    public const int TEAM_INFIELD_NUM_MAX = 1;
    public const int TEAM_OUTFIELD_NUM_MIN = 2;
    private static int[] PLAY_MATCH_TIME = new int[3]
    {
        3,
        5,
        7
    };
    public static readonly string[] PositionTypeName = new string[3]
    {
        "内野",
        "外野",
        "MAX"
    };
    public static int GetPlayMatchTime(int _no) {
        return PLAY_MATCH_TIME[_no];
    }
}
