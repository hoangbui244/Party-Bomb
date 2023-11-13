using System;
using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class GameDataParams
	{
		public enum Rarity
		{
			NORMAL,
			RARE,
			SUPER_RARE,
			MAX
		}
		public enum DeskType
		{
			NORMAL,
			SCIENCE,
			CRAFT,
			PINGPONG_TABLE
		}
		public enum PositionType__
		{
			PITCHER,
			CATCHER,
			FIRST,
			SECOND,
			THIRD,
			SHORT,
			LEFT,
			CENTER,
			RIGHT,
			MAX
		}
		public enum PitchType
		{
			SLIDER,
			CURVE,
			STRAIGHT,
			SINKER,
			SHOOT,
			MAX
		}
		public enum TeamStatusType
		{
			OFFENSE,
			DEFENSE,
			SPEED,
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
		public enum PositionType
		{
			GK,
			DF,
			MF,
			FW,
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
		public const int MAX_PLAYER_NUM = 2;
		public const int SCHOOL_NAME_CHAR_LIMIT = 8;
		public const int CHARACTER_NAME_CHAR_LIMIT = 5;
		public const int TEAM_MEMBER_NUM_MAX = 11;
		public static readonly Color[] PLAYER_COLOR = new Color[4]
		{
			new Color(0f, 255f, 0f),
			new Color(255f, 0f, 0f),
			new Color(0f, 33f, 255f),
			new Color(255f, 227f, 0f)
		};
		private static int[] PLAY_MATCH_TIME = new int[4]
		{
			1,
			3,
			5,
			10
		};
		private static int[] PLEASURE_GET_POINT = new int[6]
		{
			300,
			50,
			0,
			100,
			0,
			50
		};
		public static readonly string[] POSITION_TYPE_NAME = new string[9]
		{
			"投手",
			"捕手",
			"一塁手",
			"二塁手",
			"三塁手",
			"遊撃手",
			"左翼手",
			"中堅手",
			"右翼手"
		};
		public static readonly Color[] POSITION_TYPE_COLOR = new Color[9]
		{
			new Color32(byte.MaxValue, 128, 128, byte.MaxValue),
			new Color32(128, 185, byte.MaxValue, byte.MaxValue),
			new Color32(byte.MaxValue, byte.MaxValue, 128, byte.MaxValue),
			new Color32(byte.MaxValue, byte.MaxValue, 128, byte.MaxValue),
			new Color32(byte.MaxValue, byte.MaxValue, 128, byte.MaxValue),
			new Color32(byte.MaxValue, byte.MaxValue, 128, byte.MaxValue),
			new Color32(27, 203, 35, byte.MaxValue),
			new Color32(27, 203, 35, byte.MaxValue),
			new Color32(27, 203, 35, byte.MaxValue)
		};
		public const int TOURNAMENT_PT_SCHOOLS_NUM = 16;
		public const int TOURNAMENT_BATTLE_NUM = 4;
		public const int TEAM_STATUS_SCALE_NUM = 10;
		public const int TEAM_STATUS_POINT_MAX = 40;
		public const int TEAM_STATUS_POINT_MIN = 12;
		public const string SOUND_ON_TEX_NAME = "btn_sound_on";
		public const string SOUND_OFF_TEX_NAME = "btn_sound_off";
		public const float BGM_VOLUME = 0.5f;
		public const float SE_VOLUME = 0.5f;
		public const float VOICE_VOLUME = 0.5f;
		public const int VIDEO_AD_POINT = 100;
		public const string TAG_BALL = "Ball";
		public const string TAG_DESK = "Desk";
		public const string TAG_FLOOR = "Floor";
		public const string TAG_FIELD = "Field";
		public const string TAG_GOAL = "Goal";
		public const string TAG_GOAL_NET = "GoalNet";
		public const string TAG_GOALPOST = "Goalpost";
		public const string TAG_CHARACTER = "Character";
		public const string TAG_OBJECT = "Object";
		public const string TAG_CHECK_COLLIDER = "CheckCollider";
		public const string TAG_VERTICAL_WALL = "VerticalWall";
		public const string TAG_HORIZONTAL_WALL = "HorizontalWall";
		public const string TAG_WALL = "Wall";
		public const string TAG_NO_HIT = "NoHit";
		public const string TAG_MINIGAME_TARGET = "MiniGameTarget";
		public const string LAYER_BALL = "Ball";
		public const string LAYER_FLOOR = "Floor";
		public const string LAYER_CHECK_COLLIDER = "CheckCollider";
		public const string LAYER_OBJECT = "Object";
		public const string LAYER_CAPSULE = "Capsule";
		public const string LAYER_CHARACTER = "Character";
		public const string LAYER_FIELD = "Field";
		public const string LAYER_WALL = "Wall";
		public const string LAYER_GOAL = "Goal";
		public const string LAYER_NO_HIT = "NoHit";
		public const string LAYER_KEEPER = "Keeper";
		public const string LAYER_PENALTY_AREA = "PenaltyArea";
		public const string LAYER_INVISIBLE_CHARA = "InvisibleChara";
		public static string[] charaModelNames = new string[3]
		{
			"fbx_Character_0",
			"fbx_Character_1",
			"fbx_Character_2"
		};
		public static string[] uniformColorType = new string[8]
		{
			"WHITE",
			"BLACK",
			"RED",
			"BLUE",
			"GREEN",
			"YELLOW",
			"PINK",
			"ORANGE"
		};
		public const int BAT_HAVE_NUM_MAX = 72;
		public const float MIN_POWER = 100f;
		public const float MAX_POWER = 200f;
		public const float MIN_SPEED = 100f;
		public const float MAX_SPEED = 300f;
		public const int ITEM_HAVE_NUM_MAX = 99;
		public static readonly Dictionary<int, int> BAT_UNLOCK_NEED_NUM = new Dictionary<int, int>();
		public static readonly Dictionary<int, string> BAT_UNLOCK_DESC = new Dictionary<int, string>();
		public static readonly Dictionary<int, string> UNIFORM_REWARD_UNLOCK_DESC = new Dictionary<int, string>
		{
			{
				30,
				"友達対戦の２人対戦をプレイすると" + Environment.NewLine + "獲得できます"
			},
			{
				28,
				"練習試合で「強い」CPUに" + Environment.NewLine + "勝利すると獲得できます"
			},
			{
				48,
				"練習試合で「とても強い」CPUに" + Environment.NewLine + "勝利すると獲得できます"
			}
		};
		public static readonly Dictionary<int, int> UNIFORM_TOURNAMENT_UNLOCK_NEED_NUM = new Dictionary<int, int>
		{
			{
				35,
				1
			},
			{
				29,
				5
			},
			{
				37,
				10
			}
		};
		public static readonly Dictionary<int, string> UNIFORM_TOURNAMENT_UNLOCK_DESC = new Dictionary<int, string>
		{
			{
				35,
				"ト\u30fcナメントで" + UNIFORM_TOURNAMENT_UNLOCK_NEED_NUM[35].ToString() + "回優勝すると" + Environment.NewLine + "獲得できます"
			},
			{
				29,
				"ト\u30fcナメントで" + UNIFORM_TOURNAMENT_UNLOCK_NEED_NUM[29].ToString() + "回優勝すると" + Environment.NewLine + "獲得できます"
			},
			{
				37,
				"ト\u30fcナメントで" + UNIFORM_TOURNAMENT_UNLOCK_NEED_NUM[37].ToString() + "回優勝すると" + Environment.NewLine + "獲得できます"
			}
		};
		public static readonly Dictionary<int, int> UNIFORM_ITEM_UNLOCK_NEED_NUM = new Dictionary<int, int>
		{
			{
				38,
				10
			},
			{
				36,
				10
			},
			{
				47,
				10
			}
		};
		public static readonly Dictionary<int, string> UNIFORM_ITEM_UNLOCK_DESC = new Dictionary<int, string>
		{
			{
				38,
				"強化アイテムを" + UNIFORM_ITEM_UNLOCK_NEED_NUM[38].ToString() + "種類解放すると" + Environment.NewLine + "獲得できます"
			},
			{
				36,
				"ユニフォ\u30fcムを" + UNIFORM_ITEM_UNLOCK_NEED_NUM[36].ToString() + "種類解放すると" + Environment.NewLine + "獲得できます"
			},
			{
				47,
				"フォ\u30fcメ\u30fcションを" + UNIFORM_ITEM_UNLOCK_NEED_NUM[47].ToString() + "種類解放すると" + Environment.NewLine + "獲得できます"
			}
		};
		public static readonly Dictionary<int, string> UNIFORM_DLC1_UNLOCK_DESC = new Dictionary<int, string>
		{
			{
				63,
				"追加コンテンツ" + Environment.NewLine + "「スペシャルパック」" + Environment.NewLine + "で獲得できます"
			},
			{
				64,
				"追加コンテンツ" + Environment.NewLine + "「スペシャルパック」" + Environment.NewLine + "で獲得できます"
			},
			{
				65,
				"追加コンテンツ" + Environment.NewLine + "「スペシャルパック」" + Environment.NewLine + "で獲得できます"
			},
			{
				66,
				"追加コンテンツ" + Environment.NewLine + "「スペシャルパック」" + Environment.NewLine + "で獲得できます"
			},
			{
				67,
				"追加コンテンツ" + Environment.NewLine + "「スペシャルパック」" + Environment.NewLine + "で獲得できます"
			},
			{
				68,
				"追加コンテンツ" + Environment.NewLine + "「スペシャルパック」" + Environment.NewLine + "で獲得できます"
			}
		};
		public static readonly Dictionary<int, string> UNIFORM_DLC2_UNLOCK_DESC = new Dictionary<int, string>
		{
			{
				69,
				"追加コンテンツ" + Environment.NewLine + "「ユニフォ\u30fcムパックA」" + Environment.NewLine + "で獲得できます"
			},
			{
				70,
				"追加コンテンツ" + Environment.NewLine + "「ユニフォ\u30fcムパックA」" + Environment.NewLine + "で獲得できます"
			},
			{
				71,
				"追加コンテンツ" + Environment.NewLine + "「ユニフォ\u30fcムパックA」" + Environment.NewLine + "で獲得できます"
			},
			{
				72,
				"追加コンテンツ" + Environment.NewLine + "「ユニフォ\u30fcムパックA」" + Environment.NewLine + "で獲得できます"
			},
			{
				73,
				"追加コンテンツ" + Environment.NewLine + "「ユニフォ\u30fcムパックA」" + Environment.NewLine + "で獲得できます"
			},
			{
				74,
				"追加コンテンツ" + Environment.NewLine + "「ユニフォ\u30fcムパックA」" + Environment.NewLine + "で獲得できます"
			}
		};
		public static readonly Dictionary<int, string> FORMATION_UNLOCK_DESC = new Dictionary<int, string>();
		public static readonly Dictionary<int, string> FORMATION_TOURNAMENT_UNLOCK_DESC = new Dictionary<int, string>();
		public static readonly Dictionary<int, string> ITEM_REWORD_UNLOCK_DESC = new Dictionary<int, string>();
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
		public static int GetPleasureGetPoint(int _no)
		{
			if (_no <= PLEASURE_GET_POINT.Length - 1)
			{
				return PLEASURE_GET_POINT[_no];
			}
			return 0;
		}
		public static bool IsLayer(int _layer, string _layerName)
		{
			return _layer == LayerMask.NameToLayer(_layerName);
		}
		public static int ConvertLayerNo(string _layerName)
		{
			return LayerMask.NameToLayer(_layerName);
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
}
