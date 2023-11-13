using System;
using System.Collections.Generic;
using UnityEngine;
public class Skijump_GameDataParams
{
	public enum Rarity
	{
		NORMAL,
		RARE,
		SUPER_RARE,
		MAX
	}
	public enum BallSpeedType
	{
		SLOW,
		NORMAL,
		FAST,
		MAX
	}
	public enum JumpStandType
	{
		NORMAL_HILL,
		LARGE_HILL,
		MAX
	}
	public enum TeamStatusType
	{
		SPEED,
		TECHNIQUE,
		ACCEL,
		STAMINA,
		MAX
	}
	public enum AiTacticsType
	{
		BALANCE,
		OFFENSIVE,
		PRUDENCE,
		TECHNIC,
		MAX
	}
	public enum AiStrength
	{
		NOOB,
		WEAK,
		COMMON,
		STRONG,
		VERY_STRONG
	}
	public enum SuspendDataDefine
	{
		AREA_NUM,
		PLAYER_POINT,
		CPU_POINT
	}
	public enum SwimType
	{
		BACK,
		BREAST,
		BUTTERFLY,
		FREE,
		MAX
	}
	public struct FormationData
	{
		public string name;
		public string info;
		public Rarity rarity;
		public FormationData(string _name, string _info, Rarity _rarity)
		{
			name = _name;
			info = _info;
			rarity = _rarity;
		}
	}
	public enum ReceiveSeType
	{
		NORMAL,
		LIGHT,
		HEAVY,
		WOOD,
		METAL
	}
	public enum BattingType
	{
		RIGHT_HANDED,
		LEFT_HANDED,
		SWITCH_HITTER,
		MAX
	}
	public enum BatMainColorType
	{
		RED,
		WHITE,
		PURPLE,
		GREEN,
		YELLOW,
		BLUE,
		BROWN,
		LIGHT_BLUE,
		PINK,
		ORANGE,
		BLACK,
		RAINBOW,
		THREE
	}
	public struct RacketData
	{
		public float[] power;
		public float[] control;
		public float offsetY;
		public int gripType;
		public ReceiveSeType seType;
		public Rarity rarity;
		public int price;
		public string name;
		public Vector3 mainScale;
		public BatMainColorType[] ballTrailColor;
		public Vector3 buttonOffset;
		public Vector3 buttonSize;
		public Vector3 eulerAngles;
		public Vector3 selectScale;
		public Vector3 selectOffset;
		public string message;
		public int hitEffect;
		public RacketData(float[] _power, float[] _control, float _offset_y, int _gripType, ReceiveSeType _seType, Rarity _rarity, int _price, string _name, Vector3 _mainScale, BatMainColorType[] _ball_color_type, Vector3 _buttonOffset, Vector3 _buttonSize, Vector3 _eulerAngles, Vector3 _selectScale, Vector3 _selectOffset, string _message, int _hitEffect)
		{
			power = _power;
			control = _control;
			offsetY = _offset_y;
			gripType = _gripType;
			seType = _seType;
			rarity = _rarity;
			price = _price;
			name = _name;
			mainScale = _mainScale;
			ballTrailColor = _ball_color_type;
			buttonOffset = _buttonOffset;
			buttonSize = _buttonSize;
			eulerAngles = _eulerAngles;
			selectScale = _selectScale;
			selectOffset = _selectOffset;
			message = _message;
			hitEffect = _hitEffect;
		}
		public float GetTrajectory(bool _normalize = false)
		{
			return 0f;
		}
	}
	public enum AREA_OBJ_TYPE
	{
		NONE,
		PLACE_DESKPAD_ONE,
		PLACE_DESKPAD_TWO,
		PLACE_DESKPAD_THREE,
		PLACE_NOTE_ONE,
		PLACE_NOTE_TWO,
		PLACE_NOTE_THREE,
		DROP_ERASER,
		DROP_DICE,
		DROP_DUCK,
		DROP_RULER,
		DROP_PENCIL,
		DROP_GLUE_STICK,
		DROP_CORN
	}
	public const int MAX_HAVE_POINT = 9999999;
	public const int MAX_PLAYER_NUM = 4;
	public const int PLAY_TEAM_NUM = 4;
	public const int SCHOOL_NAME_CHAR_LIMIT = 8;
	public const int CHARACTER_NAME_CHAR_LIMIT = 5;
	public const int TEAM_MEMBEAR_NUM = 4;
	public static readonly string[] BALL_SPEED_TYPE_NAME = new string[4]
	{
		"おそい",
		"ふつう",
		"はやい",
		"MAX"
	};
	public static readonly float[] BALL_SPEED_TABLE = new float[4]
	{
		0.8f,
		1f,
		1.15f,
		1f
	};
	public static readonly Color[] BALL_SPEED_TYPE_COLOR = new Color[4]
	{
		new Color32(0, 234, byte.MaxValue, byte.MaxValue),
		new Color32(108, byte.MaxValue, 0, byte.MaxValue),
		new Color32(byte.MaxValue, 228, 77, byte.MaxValue),
		ColorPalet.white
	};
	private static int[] PLAY_MATCH_TIME = new int[3]
	{
		1,
		3,
		5
	};
	private static int[] PLAY_POINT = new int[3]
	{
		5,
		15,
		25
	};
	public static int[] PLAY_POINT_LAST_SET = new int[3]
	{
		3,
		10,
		15
	};
	private static int[] PLAY_SET_NUM = new int[3]
	{
		1,
		3,
		5
	};
	private static int[] PLAY_OPPONENT = new int[2]
	{
		0,
		1
	};
	private static int[] PLAY_PLAYER_NUM = new int[3]
	{
		2,
		3,
		4
	};
	private static int[] PLAY_SKI_JUMPER_NUM = new int[2]
	{
		1,
		4
	};
	public static readonly string[] JumpStandTypeName = new string[3]
	{
		"ノ\u30fcマルヒル",
		"ラ\u30fcジヒル",
		""
	};
	private static int[] PLAY_JUMP_NUM = new int[2]
	{
		1,
		2
	};
	private static int[] PLAY_POOL_SIZE = new int[2]
	{
		25,
		50
	};
	private static int[] PLAY_SWIMMING_DISTANCE = new int[4]
	{
		50,
		100,
		200,
		400
	};
	public static string[] JUMP_STAND_NAME = new string[2]
	{
		"ノ\u30fcマルヒル",
		"ラ\u30fcジヒル"
	};
	public static float[] FRONT_POINT_DISTANCE = new float[2]
	{
		70f,
		90f
	};
	public static float[] K_POINT_DISTANCE = new float[2]
	{
		90f,
		120f
	};
	public static float[] HS_POINT_DISTANCE = new float[2]
	{
		100f,
		130f
	};
	public const int TOURNAMENT_PT_SCHOOLS_NUM = 8;
	public const int TOURNAMENT_BATTLE_NUM = 3;
	public const int TEAM_STATUS_SCALE_NUM = 10;
	public const int TEAM_STATUS_POINT_MAX = 40;
	public const int TEAM_STATUS_POINT_MIN = 12;
	public static readonly Vector3 DEF_GRAVITY = new Vector3(0f, -9.81f, 0f);
	public static readonly string[] StatusName = new string[4]
	{
		"スピ\u30fcド",
		"テクニック",
		"パワ\u30fc",
		"バランス"
	};
	public const string SOUND_ON_TEX_NAME = "btn_sound_on";
	public const string SOUND_OFF_TEX_NAME = "btn_sound_off";
	public const float BGM_VOLUME = 0.75f;
	public const int VIDEO_AD_POINT = 100;
	public const string TAG_BALL = "Ball";
	public const string TAG_DESK = "Desk";
	public const string TAG_FLOOR = "Floor";
	public const string TAG_STAGE = "Stage";
	public const string TAG_FIELD = "Field";
	public const string TAG_GOAL = "Goal";
	public const string TAG_BACKBOARD = "Backboard";
	public const string TAG_CHARACTER = "Character";
	public const string TAG_OBJECT = "Object";
	public const string TAG_CHECK_COLLIDER = "CheckCollider";
	public const string TAG_VERTICAL_WALL = "VerticalWall";
	public const string TAG_HORIZONTAL_WALL = "HorizontalWall";
	public const string TAG_WALL = "Wall";
	public const string TAG_NO_HIT = "NoHit";
	public const string TAG_MINIGAME_TARGET = "MiniGameTarget";
	public const string TAG_SNOW_SLIDER = "SnowSlider";
	public const string TAG_SNOW_STAND = "SnowStand";
	public const string LAYER_BALL = "Ball";
	public const string LAYER_FLOOR = "Floor";
	public const string LAYER_STAGE = "Stage";
	public const string LAYER_CHECK_COLLIDER = "CheckCollider";
	public const string LAYER_OBJECT = "Object";
	public const string LAYER_CAPSULE = "Capsule";
	public const string LAYER_CHARACTER = "Character";
	public const string LAYER_FIELD = "Field";
	public const string LAYER_WALL = "Wall";
	public const string LAYER_GOAL = "Goal";
	public const string LAYER_BACKBOARD = "Backboard";
	public const string LAYER_NO_HIT = "NoHit";
	public const string LAYER_INVISIBLE_CHARA = "InvisibleChara";
	public static readonly string[] SwimTypeName = new string[5]
	{
		"背泳ぎ",
		"平泳ぎ",
		"バタフライ",
		"自由形",
		"メドレ\u30fc"
	};
	public static readonly string[,] PositionTypeTexName = new string[8, 2]
	{
		{
			"formation_player_br",
			"formation_player_br_s"
		},
		{
			"formation_player_fr",
			"formation_player_fr_s"
		},
		{
			"formation_player_fc",
			"formation_player_fc_s"
		},
		{
			"formation_player_fl",
			"formation_player_fl_s"
		},
		{
			"formation_player_bl",
			"formation_player_bl_s"
		},
		{
			"formation_player_bc",
			"formation_player_bc_s"
		},
		{
			"formation_player_li",
			"formation_player_li_s"
		},
		{
			"MAX",
			"MAX"
		}
	};
	public static string[] charaModelNames = new string[3]
	{
		"fbx_Character_0",
		"fbx_Character_1",
		"fbx_Character_2"
	};
	public static string[] uniformColorType = new string[10]
	{
		"WHITE",
		"BLACK",
		"RED",
		"BLUE",
		"GREEN",
		"YELLOW",
		"PINK",
		"ORANGE",
		"GRAY",
		"PURPLE"
	};
	public const float MIN_POWER = 100f;
	public const float MAX_POWER = 200f;
	public const float MIN_SPEED = 100f;
	public const float MAX_SPEED = 300f;
	public const int ITEM_HAVE_NUM_MAX = 99;
	public static readonly Dictionary<int, int> BAT_UNLOCK_NEED_NUM = new Dictionary<int, int>();
	public static readonly Dictionary<int, string> BAT_UNLOCK_DESC = new Dictionary<int, string>();
	public static readonly Dictionary<int, int> UNIFORM_UNLOCK_PRESENT_LIST = new Dictionary<int, int>
	{
		{
			50,
			0
		}
	};
	public static readonly Dictionary<int, int> UNIFORM_UNLOCK_NEED_NUM = new Dictionary<int, int>
	{
		{
			24,
			20
		},
		{
			25,
			50
		},
		{
			26,
			100
		},
		{
			27,
			200
		},
		{
			28,
			500
		},
		{
			29,
			1000
		}
	};
	public static readonly Dictionary<int, int> UNIFORM_1VS1_UNLOCK_NEED_NUM = new Dictionary<int, int>
	{
		{
			149,
			20
		},
		{
			150,
			50
		},
		{
			151,
			100
		},
		{
			152,
			200
		},
		{
			153,
			500
		},
		{
			154,
			1000
		}
	};
	public static readonly Dictionary<int, string> UNIFORM_UNLOCK_DESC = new Dictionary<int, string>
	{
		{
			24,
			"全国遠征で" + UNIFORM_UNLOCK_NEED_NUM[24].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		},
		{
			25,
			"全国遠征で" + UNIFORM_UNLOCK_NEED_NUM[25].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		},
		{
			26,
			"全国遠征で" + UNIFORM_UNLOCK_NEED_NUM[26].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		},
		{
			27,
			"全国遠征で" + UNIFORM_UNLOCK_NEED_NUM[27].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		},
		{
			28,
			"全国遠征で" + UNIFORM_UNLOCK_NEED_NUM[28].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		},
		{
			29,
			"全国遠征で" + UNIFORM_UNLOCK_NEED_NUM[29].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		}
	};
	public static readonly Dictionary<int, string> UNIFORM_1VS1_UNLOCK_DESC = new Dictionary<int, string>
	{
		{
			149,
			"1VS1対戦で" + UNIFORM_1VS1_UNLOCK_NEED_NUM[149].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		},
		{
			150,
			"1VS1対戦で" + UNIFORM_1VS1_UNLOCK_NEED_NUM[150].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		},
		{
			151,
			"1VS1対戦で" + UNIFORM_1VS1_UNLOCK_NEED_NUM[151].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		},
		{
			152,
			"1VS1対戦で" + UNIFORM_1VS1_UNLOCK_NEED_NUM[152].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		},
		{
			153,
			"1VS1対戦で" + UNIFORM_1VS1_UNLOCK_NEED_NUM[153].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		},
		{
			154,
			"1VS1対戦で" + UNIFORM_1VS1_UNLOCK_NEED_NUM[154].ToString() + "勝すると" + Environment.NewLine + "解放されます"
		}
	};
	public static readonly Dictionary<int, int> UNIFORM_TOURNAMENT_UNLOCK_NEED_NUM = new Dictionary<int, int>
	{
		{
			22,
			5
		},
		{
			23,
			10
		}
	};
	public static readonly Dictionary<int, string> UNIFORM_TOURNAMENT_UNLOCK_DESC = new Dictionary<int, string>
	{
		{
			22,
			"ト\u30fcナメント全国大会で" + Environment.NewLine + UNIFORM_TOURNAMENT_UNLOCK_NEED_NUM[22].ToString() + "回優勝すると" + Environment.NewLine + "解放されます"
		},
		{
			23,
			"ト\u30fcナメント全国大会で" + Environment.NewLine + UNIFORM_TOURNAMENT_UNLOCK_NEED_NUM[23].ToString() + "回優勝すると" + Environment.NewLine + "解放されます"
		}
	};
	public static readonly Dictionary<int, string> FORMATION_UNLOCK_DESC = new Dictionary<int, string>();
	public static readonly Dictionary<int, string> FORMATION_TOURNAMENT_UNLOCK_DESC = new Dictionary<int, string>();
	public static readonly Dictionary<int, string> ITEM_REWORD_UNLOCK_DESC = new Dictionary<int, string>
	{
		{
			21,
			"ト\u30fcナメント大会で" + Environment.NewLine + "全国優勝すると" + Environment.NewLine + "報酬で獲得できます"
		},
		{
			17,
			"全国遠征のランキング" + Environment.NewLine + "300位以内に入ると" + Environment.NewLine + "報酬で獲得できます"
		},
		{
			18,
			"全国遠征のランキング" + Environment.NewLine + "100位以内に入ると" + Environment.NewLine + "報酬で獲得できます"
		},
		{
			19,
			"全国遠征のランキング" + Environment.NewLine + "10位以内に入ると" + Environment.NewLine + "報酬で獲得できます"
		},
		{
			20,
			"全国遠征のランキング" + Environment.NewLine + "1位になると" + Environment.NewLine + "報酬で獲得できます"
		}
	};
	public static readonly Dictionary<int, int> RANKING_REWARD_POINT_RANK = new Dictionary<int, int>
	{
		{
			1,
			2000
		},
		{
			10,
			1500
		},
		{
			100,
			1000
		},
		{
			300,
			500
		}
	};
	public static readonly Dictionary<int, int> RANKING_REWARD_ACHIEVE_RANK = new Dictionary<int, int>
	{
		{
			1,
			0
		},
		{
			10,
			1
		},
		{
			100,
			2
		}
	};
	public static readonly Dictionary<int, int> RANKING_REWARD_ITEM_RANK = new Dictionary<int, int>
	{
		{
			1,
			20
		},
		{
			10,
			19
		},
		{
			100,
			18
		},
		{
			300,
			17
		}
	};
	public static string[] BATTING_TYPE_NAME = new string[3]
	{
		"右打ち",
		"左打ち",
		"両打ち"
	};
	public static readonly Color[] BAT_MAIN_COLOR = new Color[13]
	{
		ColorPalet.red,
		ColorPalet.white,
		ColorPalet.purple,
		ColorPalet.green,
		ColorPalet.yellow,
		ColorPalet.blue,
		ColorPalet.brown,
		ColorPalet.lightblue,
		ColorPalet.pink,
		ColorPalet.orange,
		ColorPalet.black,
		Color.white,
		Color.white
	};
	private static RacketData[] racketData = new RacketData[1]
	{
		new RacketData(new float[3]
		{
			1f,
			1.1f,
			1.1f
		}, new float[3]
		{
			1f,
			1f,
			1f
		}, 0f, 0, ReceiveSeType.NORMAL, Rarity.NORMAL, 0, "シェ\u30fcクハンドA", new Vector3(1f, 1f, 1f), new BatMainColorType[3]
		{
			BatMainColorType.RED,
			BatMainColorType.PINK,
			BatMainColorType.GREEN
		}, new Vector3(40f, -50f, -100f), new Vector3(100f, 100f, 100f), new Vector3(130f, -75f, 130f), new Vector3(320f, 320f, 320f), new Vector3(0f, 0f, 0f), "まずはここから練習開始！\n基本から練習しよう！", 0)
	};
	public static readonly Dictionary<int, string[]> REGIONAL_DATA_LIST = new Dictionary<int, string[]>
	{
		{
			0,
			new string[7]
			{
				"北海道",
				"青森県",
				"岩手県",
				"宮城県",
				"秋田県",
				"山形県",
				"福島県"
			}
		},
		{
			1,
			new string[8]
			{
				"茨城県",
				"栃木県",
				"群馬県",
				"埼玉県",
				"千葉県",
				"東京都",
				"神奈川県",
				"山梨県"
			}
		},
		{
			2,
			new string[5]
			{
				"富山県",
				"石川県",
				"福井県",
				"長野県",
				"新潟県"
			}
		},
		{
			3,
			new string[4]
			{
				"愛知県",
				"岐阜県",
				"三重県",
				"静岡県"
			}
		},
		{
			4,
			new string[6]
			{
				"大阪府",
				"京都府",
				"兵庫県",
				"奈良県",
				"滋賀県",
				"和歌山県"
			}
		},
		{
			5,
			new string[5]
			{
				"鳥取県",
				"島根県",
				"岡山県",
				"広島県",
				"山口県"
			}
		},
		{
			6,
			new string[4]
			{
				"徳島県",
				"香川県",
				"愛媛県",
				"高知県"
			}
		},
		{
			7,
			new string[8]
			{
				"福岡県",
				"佐賀県",
				"長崎県",
				"熊本県",
				"大分県",
				"宮崎県",
				"鹿児島県",
				"沖縄県"
			}
		}
	};
	public static readonly string[] REGIONAL_LIST = new string[8]
	{
		"東北",
		"関東",
		"北信越",
		"東海",
		"近畿",
		"中国",
		"四国",
		"九州"
	};
	public static int[] arraySingleModeCpuUseRacket = new int[56]
	{
		0,
		0,
		0,
		1,
		1,
		2,
		3,
		3,
		4,
		4,
		5,
		6,
		6,
		7,
		7,
		10,
		8,
		8,
		9,
		9,
		11,
		12,
		12,
		14,
		14,
		13,
		15,
		15,
		16,
		16,
		17,
		20,
		20,
		21,
		21,
		18,
		19,
		19,
		20,
		20,
		21,
		22,
		22,
		23,
		23,
		24,
		26,
		26,
		27,
		27,
		34,
		35,
		36,
		37,
		38,
		39
	};
	public static string[] arraySingleModeDeskString = new string[56]
	{
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_0",
		"Desk_3"
	};
	public static string[] arraySingleAreaObjRes = new string[14]
	{
		"",
		"Deskpad_0",
		"Deskpad_1",
		"Deskpad_2",
		"Note_0",
		"Note_1",
		"Note_2",
		"Eraser",
		"Dice",
		"Duck",
		"Ruler",
		"Pencil",
		"GlueStick",
		"Corn"
	};
	public static AiStrength[] areaReleaseBattleAiStrength = new AiStrength[11]
	{
		AiStrength.NOOB,
		AiStrength.NOOB,
		AiStrength.NOOB,
		AiStrength.WEAK,
		AiStrength.WEAK,
		AiStrength.COMMON,
		AiStrength.COMMON,
		AiStrength.STRONG,
		AiStrength.STRONG,
		AiStrength.VERY_STRONG,
		AiStrength.VERY_STRONG
	};
	public static AREA_OBJ_TYPE[,] arraySingleAreaObj = new AREA_OBJ_TYPE[56, 1]
	{
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.DROP_RULER
		},
		{
			AREA_OBJ_TYPE.DROP_ERASER
		},
		{
			AREA_OBJ_TYPE.DROP_GLUE_STICK
		},
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.DROP_DICE
		},
		{
			AREA_OBJ_TYPE.DROP_DUCK
		},
		{
			AREA_OBJ_TYPE.DROP_PENCIL
		},
		{
			AREA_OBJ_TYPE.PLACE_DESKPAD_ONE
		},
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.DROP_ERASER
		},
		{
			AREA_OBJ_TYPE.DROP_CORN
		},
		{
			AREA_OBJ_TYPE.DROP_DUCK
		},
		{
			AREA_OBJ_TYPE.DROP_RULER
		},
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.PLACE_NOTE_ONE
		},
		{
			AREA_OBJ_TYPE.DROP_DICE
		},
		{
			AREA_OBJ_TYPE.DROP_PENCIL
		},
		{
			AREA_OBJ_TYPE.PLACE_DESKPAD_TWO
		},
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.DROP_DUCK
		},
		{
			AREA_OBJ_TYPE.DROP_GLUE_STICK
		},
		{
			AREA_OBJ_TYPE.PLACE_NOTE_ONE
		},
		{
			AREA_OBJ_TYPE.DROP_ERASER
		},
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.DROP_DUCK
		},
		{
			AREA_OBJ_TYPE.DROP_RULER
		},
		{
			AREA_OBJ_TYPE.DROP_CORN
		},
		{
			AREA_OBJ_TYPE.PLACE_DESKPAD_THREE
		},
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.DROP_ERASER
		},
		{
			AREA_OBJ_TYPE.DROP_CORN
		},
		{
			AREA_OBJ_TYPE.DROP_DICE
		},
		{
			AREA_OBJ_TYPE.DROP_GLUE_STICK
		},
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.PLACE_NOTE_ONE
		},
		{
			AREA_OBJ_TYPE.PLACE_NOTE_TWO
		},
		{
			AREA_OBJ_TYPE.DROP_ERASER
		},
		{
			AREA_OBJ_TYPE.PLACE_DESKPAD_ONE
		},
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.DROP_DICE
		},
		{
			AREA_OBJ_TYPE.DROP_DUCK
		},
		{
			AREA_OBJ_TYPE.PLACE_DESKPAD_TWO
		},
		{
			AREA_OBJ_TYPE.DROP_ERASER
		},
		{
			AREA_OBJ_TYPE.NONE
		},
		{
			AREA_OBJ_TYPE.DROP_PENCIL
		},
		{
			AREA_OBJ_TYPE.DROP_RULER
		},
		{
			AREA_OBJ_TYPE.DROP_ERASER
		},
		{
			AREA_OBJ_TYPE.PLACE_NOTE_THREE
		},
		{
			AREA_OBJ_TYPE.DROP_ERASER
		},
		{
			AREA_OBJ_TYPE.PLACE_DESKPAD_THREE
		},
		{
			AREA_OBJ_TYPE.DROP_ERASER
		},
		{
			AREA_OBJ_TYPE.PLACE_DESKPAD_THREE
		},
		{
			AREA_OBJ_TYPE.NONE
		}
	};
	public static int GetPlayMatchTime(int _no)
	{
		return PLAY_MATCH_TIME[_no];
	}
	public static int GetPlayPoint(int _no)
	{
		return PLAY_POINT[_no];
	}
	public static int GetPlaySetNum(int _no)
	{
		return PLAY_SET_NUM[_no];
	}
	public static int GetPlayOpponent(int _no)
	{
		return PLAY_OPPONENT[_no];
	}
	public static int GetPlayPlayerNum(int _no)
	{
		return PLAY_PLAYER_NUM[_no];
	}
	public static int GetPlaySkiJumperNum(int _no)
	{
		return PLAY_SKI_JUMPER_NUM[_no];
	}
	public static int GetPlayJumpNum(int _no)
	{
		return PLAY_JUMP_NUM[_no];
	}
	public static int GetPlayPoolSize(int _no)
	{
		return PLAY_POOL_SIZE[_no];
	}
	public static int GetPlaySwimmingDistance(int _no)
	{
		return PLAY_SWIMMING_DISTANCE[_no];
	}
	public static float GetJumpDistanceRange(int _no, bool _add = true)
	{
		if (_add)
		{
			return HS_POINT_DISTANCE[_no] - K_POINT_DISTANCE[_no];
		}
		return K_POINT_DISTANCE[_no] - FRONT_POINT_DISTANCE[_no];
	}
	public static bool IsLayer(int _layer, string _layerName)
	{
		return _layer == LayerMask.NameToLayer(_layerName);
	}
	public static int ConvertLayerNo(string _layerName)
	{
		return LayerMask.NameToLayer(_layerName);
	}
	public static int GetLayerMask(string _layerName)
	{
		return 1 << LayerMask.NameToLayer(_layerName);
	}
	public static RacketData GetRacketData(int _no)
	{
		if (_no < GetRacketNum() && _no >= 0)
		{
			return racketData[_no];
		}
		return default(RacketData);
	}
	public static int GetRacketNum()
	{
		return racketData.Length;
	}
}
