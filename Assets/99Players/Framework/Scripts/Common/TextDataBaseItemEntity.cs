using System;
[Serializable]
public class TextDataBaseItemEntity {
    public enum DATABASE_NAME {
        COMMON,
        GET_BALL,
        CANNON_SHOT,
        BLOCK_WIPER,
        MONSTER_RACE,
        MAKING_POTION,
        RECEIVE_PON,
        BLACKSMITH,
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
        AIR_BALLOON
    }
    public enum TAG_NAME {
        NUMBER,
        NAME
    }
    public int id;
    public string text;
    public static readonly int TAG_PARAM_CNT = 3;
    public static string[] TAG_NAME_STR = new string[6]
    {
        "<number_0>",
        "<number_1>",
        "<number_2>",
        "<name_0>",
        "<name_1>",
        "<name_2>"
    };
}
