using UnityEngine;
public class GS_Define {
    public enum GameType {
        GET_BALL,
        CANNON_SHOT,
        BLOCK_WIPER,
        MOLE_HAMMER,
        BOMB_ROULETTE,
        RECEIVE_PON,
        DELIVERY_ORDER,
        ARCHER_BATTLE,
        ATTACK_BALL,
        BLOW_AWAY_TANK,
        SCROLL_JUMP,
        CLIMB_BLOCK,
        MAKE_SAME_DOT,
        TIMING_STAMPING,
        WATER_PISTOL_BATTLE,
        KURUKURU_CUBE,
        THREE_LEGGED,
        REVERSI,
        BUNBUN_JUMP,
        PUSH_LANE,
        JAMMING_DIVING,
        AWAY_HOIHOI,
        THROWING_BALLS,
        ANSWERS_RUN,
        ANSWER_DROP,
        BLOCK_CHANGER,
        PERFECT_DICE_ROLL,
        TRAP_RACE,
        MARUTA_JUMP,
        TIME_STOP,
        WOBBLY_DISK,
        ROBOT_WATCH,
        MINISCAPE_RACE,
        BELT_CONVEYOR_RUN,
        PUSH_IN_BOXING,
        BOMB_LIFTING,
        BATTLE_AIR_HOCKEY,
        BLOCK_SLICER,
        ESCAPE_ZONE,
        LABYRINTH,
        DROP_BLOCK,
        GIRIGIRI_WATER,
        DROP_PANEL,
        GIRIGIRI_STOP,
        ROCK_PAPER_SCISSORS,
        NONSTOP_PICTURE,
        ROPE_JUMPING,
        TRAIN_GUIDE,
        BREAK_BLOCK,
        GUARD_ZONE,
        HOME_RUN_DERBY,
        FLYING_HAMMER,
        COLORFUL_SHOOT,
        CLIMB_WALL,
        TREASURE_CATCHER,
        COIN_DROP,
        HUNDRED_CHALLENGE,
        JANJAN_FISHING,
        EVERYONE_KEEPER,
        AIR_BALLOON,
        BOOKSQUIRM,
        PUSHY_PENGUINS,
        CATCH_YOU_LETTER,
        TOAD_QUICK_DRAW,
        JACKAL,
        BOMBERMAN,
        TETRIS,
        GUN_SMOKE,
        IKARI_WARRIORS,
        DINO_RIKI,
        DONKEY_KONG,
        GAUNTLET,
        MAX
    }
    public enum GameFormat {
        BATTLE,
        COOP,
        BATTLE_AND_COOP
    }
    public enum GameMode {
        BATTLE,
        TIME_ATTACK
    }
    public enum InningNum {
        FIVE,
        SEVEN
    }
    public enum TeamMode {
        PERSONAL,
        TEAM
    }
    public enum UserType {
        PLAYER_1,
        PLAYER_2,
        PLAYER_3,
        PLAYER_4,
        PLAYER_5,
        PLAYER_6,
        CPU_1,
        CPU_2,
        CPU_3,
        CPU_4,
        CPU_5
    }
    public static int FIRST_GAME_NUM = 10;
    public static int PLAYER_SMALL_MAX = 4;
    public static int PLAYER_MAX = 6;
    public static int CHARACTER_MAX = 8;
    public const int GAME_5 = 5;
    public const int GAME_10 = 10;
    public const int GAME_15 = 15;
    public const int GAME_25 = 25;
    public const int GAME_35 = 35;
    public const int GAME_41 = 41;
    public static GameFormat[] GAME_FORMAT_TABLE = new GameFormat[14]
    {
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP,
        GameFormat.BATTLE_AND_COOP
    };
    public static readonly Color COLOR_SELECT_TEXT_FOCUS = Color.white;
    public static readonly Color COLOR_SELECT_TEXT_DISABLE = new Color32(63, 116, 33, byte.MaxValue);
}
