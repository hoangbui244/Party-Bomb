using System;
using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class SchoolData
	{
		public class SchoolTeamData
		{
			public string schoolName;
			public int uniformNo;
			public int ballNo;
			public int formationNo;
			public CharacterData[] characters;
			public TeamStatusData teamStatus;
			public int foundingYear;
			public int prefectures;
			public int tacticsType;
			public string[] gameRecord;
			public int gameRecordWin;
			public int gameRecordLose;
			public int gameRecordDraw;
			public int isUseNationalExpedition;
			public int isUseTournament;
			public SchoolTeamData()
			{
				uniformNo = -1;
				ballNo = -1;
				formationNo = -1;
				foundingYear = -1;
				prefectures = -1;
				tacticsType = -1;
				isUseNationalExpedition = -1;
				isUseTournament = -1;
			}
			public bool EqualsValue(SchoolTeamData _data, bool _serverData = false)
			{
				if (schoolName != _data.schoolName)
				{
					UnityEngine.Debug.Log("学校名");
					return false;
				}
				if (uniformNo != _data.uniformNo)
				{
					UnityEngine.Debug.Log("ユニフォ\u30fcム番号 : 全国遠征用 = " + uniformNo.ToString() + " : 元 = " + _data.uniformNo.ToString());
					return false;
				}
				if (ballNo != _data.ballNo)
				{
					UnityEngine.Debug.Log("ボ\u30fcル番号 : 全国遠征用 = " + ballNo.ToString() + " : 元 = " + _data.ballNo.ToString());
					return false;
				}
				if (formationNo != _data.formationNo)
				{
					UnityEngine.Debug.Log("フォ\u30fcメ\u30fcション番号 : 全国遠征用 = " + formationNo.ToString() + " : 元 = " + _data.formationNo.ToString());
					return false;
				}
				for (int i = 0; i < characters.Length; i++)
				{
					if (!characters[i].EqualsValue(_data.characters[i], _serverData))
					{
						UnityEngine.Debug.Log("キャラ[" + i.ToString() + "]");
						return false;
					}
				}
				if (!teamStatus.EqualsValue(_data.teamStatus))
				{
					UnityEngine.Debug.Log("チ\u30fcムステ\u30fcタス");
					return false;
				}
				if (isUseNationalExpedition != _data.isUseNationalExpedition)
				{
					UnityEngine.Debug.Log("全国遠征中 : プロフィ\u30fcル = " + isUseNationalExpedition.ToString() + " : 元" + _data.isUseNationalExpedition.ToString());
					return false;
				}
				if (isUseTournament != _data.isUseTournament)
				{
					UnityEngine.Debug.Log("ト\u30fcナメント大会中");
					return false;
				}
				if (_serverData)
				{
					if (foundingYear != _data.foundingYear)
					{
						UnityEngine.Debug.Log("創立年");
						return false;
					}
					if (prefectures != _data.prefectures)
					{
						UnityEngine.Debug.Log("都道府県");
						return false;
					}
					if (tacticsType != _data.tacticsType)
					{
						UnityEngine.Debug.Log("作戦番号");
						return false;
					}
					if (gameRecordWin != _data.gameRecordWin)
					{
						UnityEngine.Debug.Log("戦績(勝ち)");
						return false;
					}
					if (gameRecordLose != _data.gameRecordLose)
					{
						UnityEngine.Debug.Log("戦績(負け)");
						return false;
					}
					if (gameRecordDraw != _data.gameRecordDraw)
					{
						UnityEngine.Debug.Log("戦績(引き分け)");
						return false;
					}
				}
				return true;
			}
			public void Copy(SchoolTeamData _data, bool _serverData = false)
			{
				schoolName = _data.schoolName;
				uniformNo = _data.uniformNo;
				ballNo = _data.ballNo;
				formationNo = _data.formationNo;
				if (characters == null)
				{
					characters = new CharacterData[_data.characters.Length];
					for (int i = 0; i < characters.Length; i++)
					{
						characters[i] = new CharacterData();
					}
				}
				for (int j = 0; j < characters.Length; j++)
				{
					characters[j].Copy(_data.characters[j]);
				}
				if (teamStatus == null)
				{
					teamStatus = new TeamStatusData();
				}
				teamStatus.Copy(_data.teamStatus);
				isUseNationalExpedition = _data.isUseNationalExpedition;
				isUseTournament = _data.isUseTournament;
				if (_serverData)
				{
					foundingYear = _data.foundingYear;
					prefectures = _data.prefectures;
					tacticsType = _data.tacticsType;
					gameRecordWin = _data.gameRecordWin;
					gameRecordLose = _data.gameRecordLose;
					gameRecordDraw = _data.gameRecordDraw;
				}
			}
		}
		[Serializable]
		public class CharacterData
		{
			public string name;
			public int uniformNumber;
			public int itemNo;
			public void Copy(CharacterData _data)
			{
				name = _data.name;
				uniformNumber = _data.uniformNumber;
				itemNo = _data.itemNo;
			}
			public bool EqualsValue(CharacterData _data, bool _serverData = false)
			{
				if (name != _data.name)
				{
					UnityEngine.Debug.Log("選手名");
					return false;
				}
				if (uniformNumber != _data.uniformNumber)
				{
					UnityEngine.Debug.Log("背番号");
					return false;
				}
				if (itemNo != _data.itemNo)
				{
					UnityEngine.Debug.Log("強化アイテム番号");
					return false;
				}
				return true;
			}
		}
		public class TeamStatusData
		{
			public int[] statusParams;
			public TeamStatusData()
			{
				statusParams = new int[4];
			}
			public void Copy(TeamStatusData _data)
			{
				_data.statusParams.CopyTo(statusParams, 0);
			}
			public void Reset(int _resetValue = 0)
			{
				for (int i = 0; i < statusParams.Length; i++)
				{
					statusParams[i] = _resetValue;
				}
			}
			public bool EqualsValue(TeamStatusData _data)
			{
				for (int i = 0; i < statusParams.Length; i++)
				{
					if (statusParams[i] != _data.statusParams[i])
					{
						return false;
					}
				}
				return true;
			}
		}
		public enum ParamType
		{
			SCHOOL_NAME,
			UNIFORM_NO,
			BALL_NO,
			FORMATION_NO,
			CHARACTERS,
			CHARACTER_NAME,
			CHARACTER_UNIFORM_NUMBER,
			CHARACTER_ITEM_NO,
			TEAM_STATUS,
			TEAM_STATUS_PARAMS,
			FOUNDING_YEAR,
			PREFECTURES,
			TACTICS_TYPE,
			GAME_RECORD,
			IS_USE_NATIONAL_EXPEDITION,
			IS_USE_TOURNAMENT,
			ALL
		}
		public enum DataType
		{
			OWN,
			NETWORK_PROFILE,
			BATTLE_OPPONENT,
			TOURNAMENT,
			TOURNAMENT_OWN,
			MAX
		}
		public enum SchoolNameType
		{
			NORMAL,
			SHORT,
			INPUT_NAME,
			SCHOOL_TYPE,
			MAX
		}
		public enum DelimiterType
		{
			INDEX,
			PARAMETER,
			DATA
		}
		private struct ServerData
		{
			public string[] datas;
			public string[] characters;
			public string[] characterParams;
			public string[] teamStatusParams;
		}
		public const int SCHOOL_NUM = 8;
		public const int SCHOOL_MEMBER_NUM = 11;
		private static string PREF_SELECT_SCHOOL_NO = "PREF_SELECT_SCHOOL_NO";
		private static int[] selectSchoolNo = new int[3]
		{
			-1,
			-1,
			-1
		};
		private static Vector3 calcVec;
		private static int calcInt;
		private static bool[] init = new bool[5];
		private static SchoolTeamData[] schoolTeamData = new SchoolTeamData[8];
		private static SchoolTeamData[] tournamentSchoolTeamData = new SchoolTeamData[17];
		private static SchoolTeamData tournamentOwnSchoolTeamData;
		private static SchoolTeamData battleOpponentSchoolTeamData;
		private static SchoolTeamData networkSchoolTeamData;
		private static SchoolTeamData schoolTeamDataTemp;
		private static GameDataParams.PositionType[,] defPositionTypeList = new GameDataParams.PositionType[8, 11]
		{
			{
				GameDataParams.PositionType.GK,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.FW,
				GameDataParams.PositionType.FW
			},
			{
				GameDataParams.PositionType.GK,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.FW,
				GameDataParams.PositionType.FW
			},
			{
				GameDataParams.PositionType.GK,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.FW,
				GameDataParams.PositionType.FW
			},
			{
				GameDataParams.PositionType.GK,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.FW,
				GameDataParams.PositionType.FW
			},
			{
				GameDataParams.PositionType.GK,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.FW,
				GameDataParams.PositionType.FW
			},
			{
				GameDataParams.PositionType.GK,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.FW,
				GameDataParams.PositionType.FW
			},
			{
				GameDataParams.PositionType.GK,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.FW,
				GameDataParams.PositionType.FW
			},
			{
				GameDataParams.PositionType.GK,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.DF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.MF,
				GameDataParams.PositionType.FW,
				GameDataParams.PositionType.FW
			}
		};
		private static readonly string[] DELIMITER_CHAR = new string[3]
		{
			"區,",
			"區/",
			"區|"
		};
		private static string convertData;
		private static ServerData serverData;
		private static SchoolTeamData tournamentDamySchoolTeamData;
		private static string PREF_AFTER_A_WEEK = "PREF_AFTER_A_WEEK";
		private static bool isAfterAWeek = true;
		private static string[] schoolNameList = null;
		private static string[] firstNameList = null;
		private static string[] lastNameList = null;
		public static string[] schoolNameTypeList = new string[11]
		{
			"学校",
			"高校",
			"学院",
			"学園",
			"農林高校",
			"農業高校",
			"商業高校",
			"工業高校",
			"水産高校",
			"実業高校",
			"師範学校"
		};
		private static string[] prefecturesList = new string[47]
		{
			"北海道",
			"青森",
			"岩手",
			"宮城",
			"秋田",
			"山形",
			"福島",
			"茨城",
			"栃木",
			"群馬",
			"埼玉",
			"千葉",
			"東京",
			"神奈川",
			"新潟",
			"富山",
			"石川",
			"福井",
			"山梨",
			"長野",
			"岐阜",
			"静岡",
			"愛知",
			"三重",
			"滋賀",
			"京都",
			"大阪",
			"兵庫",
			"奈良",
			"和歌山",
			"鳥取",
			"島根",
			"岡山",
			"広島",
			"山口",
			"徳島",
			"香川",
			"愛媛",
			"高知",
			"福岡",
			"佐賀",
			"長崎",
			"熊本",
			"大分",
			"宮崎",
			"鹿児島",
			"沖縄"
		};
		private static string[] prefectoralCapitalList = new string[47]
		{
			"札幌",
			"青森",
			"盛岡",
			"仙台",
			"秋田",
			"山形",
			"福島",
			"水戸",
			"宇都宮",
			"前橋",
			"さいたま",
			"千葉",
			"新宿",
			"横浜",
			"新潟",
			"富山",
			"金沢",
			"福井",
			"甲府",
			"長野",
			"岐阜",
			"静岡",
			"名古屋",
			"津",
			"大津",
			"京都",
			"大阪",
			"神戸",
			"奈良",
			"和歌山",
			"鳥取",
			"松江",
			"岡山",
			"広島",
			"山口",
			"徳島",
			"高松",
			"松山",
			"高知",
			"福岡",
			"佐賀",
			"長崎",
			"熊本",
			"大分",
			"宮崎",
			"鹿児島",
			"那覇"
		};
		private static string[] directionList = new string[4]
		{
			"東",
			"西",
			"南",
			"北"
		};
		private static string[] numberingList = new string[3]
		{
			"第一",
			"第二",
			"第三"
		};
		private static string NAME_OPTION_MANSION = "館";
		private static int teamStatusPoint;
		private static SchoolTeamData enemySchoolTeamData;
		private static string[] NEW_LINE_SPLIT = new string[1]
		{
			Environment.NewLine
		};
		private static List<SchoolTeamData> leaveSchoolTeamData = new List<SchoolTeamData>();
		private static int[] randomUniformNoList;
		public static void SetSelectSchoolNo(int _schoolNo, int _playerNo = 2, bool _save = true)
		{
			selectSchoolNo[_playerNo] = _schoolNo;
			PlayerPrefs.SetInt(PREF_SELECT_SCHOOL_NO + "_" + _playerNo.ToString(), _schoolNo);
		}
		public static int GetSelectSchoolNo(int _playerNo = 2)
		{
			if (selectSchoolNo[_playerNo] == -1)
			{
				selectSchoolNo[_playerNo] = PlayerPrefs.GetInt(PREF_SELECT_SCHOOL_NO + "_" + _playerNo.ToString(), 0);
			}
			return selectSchoolNo[_playerNo];
		}
		private static SchoolTeamData GetSchoolData(ParamType _paramType, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (!init[(int)_type])
			{
				switch (_type)
				{
				case DataType.OWN:
					for (int i = 0; i < schoolTeamData.Length; i++)
					{
						if (schoolTeamData[i] == null)
						{
							schoolTeamData[i] = new SchoolTeamData();
						}
					}
					break;
				case DataType.TOURNAMENT:
					for (int j = 0; j < tournamentSchoolTeamData.Length; j++)
					{
						if (tournamentSchoolTeamData[j] == null)
						{
							tournamentSchoolTeamData[j] = new SchoolTeamData();
						}
					}
					break;
				case DataType.TOURNAMENT_OWN:
					if (tournamentOwnSchoolTeamData == null)
					{
						tournamentOwnSchoolTeamData = new SchoolTeamData();
					}
					break;
				case DataType.BATTLE_OPPONENT:
					if (battleOpponentSchoolTeamData == null)
					{
						battleOpponentSchoolTeamData = new SchoolTeamData();
					}
					break;
				case DataType.NETWORK_PROFILE:
					if (networkSchoolTeamData == null)
					{
						networkSchoolTeamData = new SchoolTeamData();
					}
					break;
				}
				init[(int)_type] = true;
				LoadNameList();
			}
			switch (_type)
			{
			case DataType.OWN:
				schoolTeamDataTemp = schoolTeamData[_schoolNo];
				break;
			case DataType.TOURNAMENT:
				schoolTeamDataTemp = tournamentSchoolTeamData[_schoolNo];
				break;
			case DataType.TOURNAMENT_OWN:
				schoolTeamDataTemp = tournamentOwnSchoolTeamData;
				break;
			case DataType.BATTLE_OPPONENT:
				schoolTeamDataTemp = battleOpponentSchoolTeamData;
				break;
			case DataType.NETWORK_PROFILE:
				schoolTeamDataTemp = networkSchoolTeamData;
				break;
			default:
				if (schoolTeamDataTemp == null)
				{
					UnityEngine.Debug.Log("schoolTeamDataTemp = null");
				}
				break;
			}
			switch (_paramType)
			{
			case ParamType.SCHOOL_NAME:
				LoadSchoolName(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.UNIFORM_NO:
				LoadUniformNo(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.BALL_NO:
				LoadBallNo(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.FORMATION_NO:
				LoadFormationNo(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.CHARACTERS:
			case ParamType.CHARACTER_NAME:
			case ParamType.CHARACTER_UNIFORM_NUMBER:
			case ParamType.CHARACTER_ITEM_NO:
				LoadCharacterData(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.TEAM_STATUS:
			case ParamType.TEAM_STATUS_PARAMS:
				LoadTeamStatus(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.FOUNDING_YEAR:
				LoadFoundingYear(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.PREFECTURES:
				LoadPrefectures(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.TACTICS_TYPE:
				LoadTacticsType(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.GAME_RECORD:
				LoadGameRecord(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.IS_USE_NATIONAL_EXPEDITION:
				LoadIsUseNationalExpedition(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.IS_USE_TOURNAMENT:
				LoadIsUseTournament(schoolTeamDataTemp, _schoolNo, _type);
				break;
			case ParamType.ALL:
				LoadSchoolName(schoolTeamDataTemp, _schoolNo, _type);
				LoadUniformNo(schoolTeamDataTemp, _schoolNo, _type);
				LoadBallNo(schoolTeamDataTemp, _schoolNo, _type);
				LoadFormationNo(schoolTeamDataTemp, _schoolNo, _type);
				LoadCharacterData(schoolTeamDataTemp, _schoolNo, _type);
				LoadTeamStatus(schoolTeamDataTemp, _schoolNo, _type);
				LoadFoundingYear(schoolTeamDataTemp, _schoolNo, _type);
				LoadPrefectures(schoolTeamDataTemp, _schoolNo, _type);
				LoadTacticsType(schoolTeamDataTemp, _schoolNo, _type);
				LoadGameRecord(schoolTeamDataTemp, _schoolNo, _type);
				LoadIsUseNationalExpedition(schoolTeamDataTemp, _schoolNo, _type);
				LoadIsUseTournament(schoolTeamDataTemp, _schoolNo, _type);
				break;
			}
			return schoolTeamDataTemp;
		}
		public static SchoolTeamData GetSchoolData(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.ALL, _schoolNo, _type);
		}
		public static void SetSchoolData(int _schoolNo, DataType _data, int _saveSchoolNo, DataType _type = DataType.OWN, bool _isProfile = false)
		{
			SetSchoolName(_saveSchoolNo, GetSchoolData(ParamType.SCHOOL_NAME, _schoolNo, _data).schoolName, _type, _save: false);
			SetUniformNo(_saveSchoolNo, GetSchoolData(ParamType.UNIFORM_NO, _schoolNo, _data).uniformNo, _type, _save: false);
			SetBallNo(_saveSchoolNo, GetSchoolData(ParamType.BALL_NO, _schoolNo, _data).ballNo, _type, _save: false);
			SetFormationNo(_saveSchoolNo, GetSchoolData(ParamType.FORMATION_NO, _schoolNo, _data).formationNo, _type, _save: false);
			SetCharacters(_saveSchoolNo, GetSchoolData(ParamType.CHARACTERS, _schoolNo, _data).characters, _type, _save: false);
			SetTeamStatusData(_saveSchoolNo, GetSchoolData(ParamType.TEAM_STATUS, _schoolNo, _data).teamStatus, _type, _save: false);
			SetIsUseNationalExpedition(_saveSchoolNo, GetSchoolData(ParamType.IS_USE_NATIONAL_EXPEDITION, _schoolNo, _data).isUseNationalExpedition == 1, _type, _save: false);
			SetIsUseTournament(_saveSchoolNo, GetSchoolData(ParamType.IS_USE_TOURNAMENT, _schoolNo, _data).isUseTournament == 1, _type, _save: false);
			if (_isProfile)
			{
				SetFoundingYear(_saveSchoolNo, GetSchoolData(ParamType.FOUNDING_YEAR, _schoolNo, _data).foundingYear, _type, _save: false);
				SetPrefectures(_saveSchoolNo, GetSchoolData(ParamType.PREFECTURES, _schoolNo, _data).prefectures, _type, _save: false);
				SetTacticsType(_saveSchoolNo, GetSchoolData(ParamType.TACTICS_TYPE, _schoolNo, _data).tacticsType, _type, _save: false);
				SetGameRecord(_saveSchoolNo, GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _data).gameRecordWin, GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _data).gameRecordLose, GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _data).gameRecordDraw, _type, _save: false);
			}
		}
		public static void SetSchoolData(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN, bool _isProfile = false)
		{
			SetSchoolName(_schoolNo, _data.schoolName, _type, _save: false);
			SetUniformNo(_schoolNo, _data.uniformNo, _type, _save: false);
			SetBallNo(_schoolNo, _data.ballNo, _type, _save: false);
			SetFormationNo(_schoolNo, _data.formationNo, _type, _save: false);
			SetCharacters(_schoolNo, _data.characters, _type, _save: false);
			SetTeamStatusData(_schoolNo, _data.teamStatus, _type, _save: false);
			SetIsUseNationalExpedition(_schoolNo, _data.isUseNationalExpedition == 1, _type, _save: false);
			SetIsUseTournament(_schoolNo, _data.isUseTournament == 1, _type, _save: false);
			if (_isProfile)
			{
				SetFoundingYear(_schoolNo, _data.foundingYear, _type, _save: false);
				SetPrefectures(_schoolNo, _data.prefectures, _type, _save: false);
				SetTacticsType(_schoolNo, _data.tacticsType, _type, _save: false);
				SetGameRecord(_schoolNo, _data.gameRecordWin, _data.gameRecordLose, _data.gameRecordDraw, _type, _save: false);
			}
		}
		public static void SetSchoolName(int _schoolNo, string _schoolName, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.SCHOOL_NAME, _schoolNo, _type).schoolName = _schoolName;
			PlayerPrefs.SetString(_type.ToString() + "_" + ParamType.SCHOOL_NAME.ToString() + "_" + _schoolNo.ToString(), _schoolName);
		}
		public static string GetSchoolName(int _schoolNo, SchoolNameType _nameType = SchoolNameType.NORMAL, DataType _type = DataType.OWN)
		{
			switch (_nameType)
			{
			case SchoolNameType.SHORT:
				if (GetSchoolData(ParamType.SCHOOL_NAME, _schoolNo, _type).schoolName.Length > 4)
				{
					return GetSchoolData(ParamType.SCHOOL_NAME, _schoolNo, _type).schoolName.Substring(0, 3);
				}
				return GetSchoolData(ParamType.SCHOOL_NAME, _schoolNo, _type).schoolName.Substring(0, GetSchoolData(ParamType.SCHOOL_NAME, _schoolNo, _type).schoolName.Length - 2);
			case SchoolNameType.INPUT_NAME:
				return GetSchoolData(ParamType.SCHOOL_NAME, _schoolNo, _type).schoolName.Substring(0, GetSchoolData(ParamType.SCHOOL_NAME, _schoolNo, _type).schoolName.Length - 2);
			case SchoolNameType.SCHOOL_TYPE:
				return GetSchoolData(ParamType.SCHOOL_NAME, _schoolNo, _type).schoolName.Substring(GetSchoolData(ParamType.SCHOOL_NAME, _schoolNo, _type).schoolName.Length - 2, 2);
			default:
				return GetSchoolData(ParamType.SCHOOL_NAME, _schoolNo, _type).schoolName;
			}
		}
		private static void LoadSchoolName(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.schoolName == null || _data.schoolName == "")
			{
				_data.schoolName = PlayerPrefs.GetString(_type.ToString() + "_" + ParamType.SCHOOL_NAME.ToString() + "_" + _schoolNo.ToString(), " ");
				if (_data.schoolName == " ")
				{
					SetSchoolName(_schoolNo, GetRandomSchoolName(), _type);
				}
			}
		}
		public static void SetUniformNo(int _schoolNo, int _uniformNo, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.UNIFORM_NO, _schoolNo, _type).uniformNo = _uniformNo;
			PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.UNIFORM_NO.ToString() + "_" + _schoolNo.ToString(), _uniformNo);
		}
		public static int GetUniformNo(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.UNIFORM_NO, _schoolNo, _type).uniformNo;
		}
		private static void LoadUniformNo(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.uniformNo != -1)
			{
				return;
			}
			_data.uniformNo = PlayerPrefs.GetInt(_type.ToString() + "_" + ParamType.UNIFORM_NO.ToString() + "_" + _schoolNo.ToString(), 9999);
			if (_data.uniformNo != 9999)
			{
				return;
			}
			int[] array = new int[GameSaveData.INIT_OPEN_NO.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = GameSaveData.GetUniformHaveNum(GameSaveData.INIT_OPEN_NO[i]);
			}
			for (int j = 0; j < 8; j++)
			{
				for (int k = 0; k < GameSaveData.INIT_OPEN_NO.Length; k++)
				{
					if (j != _schoolNo && GameSaveData.INIT_OPEN_NO[k] == GetUniformNo(j))
					{
						array[k]--;
					}
				}
			}
			bool flag = true;
			for (int l = 0; l < array.Length; l++)
			{
				if (array[l] > 0)
				{
					flag = false;
					break;
				}
			}
			int num = UnityEngine.Random.Range(0, GameSaveData.INIT_OPEN_NO.Length);
			if (!flag)
			{
				while (array[num] <= 0)
				{
					num = UnityEngine.Random.Range(0, GameSaveData.INIT_OPEN_NO.Length);
				}
			}
			SetUniformNo(_schoolNo, GameSaveData.INIT_OPEN_NO[num], _type);
		}
		public static void SetBallNo(int _schoolNo, int _ballNo, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.BALL_NO, _schoolNo, _type).ballNo = _ballNo;
			PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.BALL_NO.ToString() + "_" + _schoolNo.ToString(), _ballNo);
		}
		public static int GetBallNo(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.BALL_NO, _schoolNo, _type).ballNo;
		}
		private static void LoadBallNo(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.ballNo == -1)
			{
				_data.ballNo = PlayerPrefs.GetInt(_type.ToString() + "_" + ParamType.BALL_NO.ToString() + "_" + _schoolNo.ToString(), 9999);
				if (_data.ballNo == 9999)
				{
					SetBallNo(_schoolNo, 0, _type);
				}
			}
		}
		public static void SetFormationNo(int _schoolNo, int _formationNo, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.FORMATION_NO, _schoolNo, _type).formationNo = _formationNo;
			PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.FORMATION_NO.ToString() + "_" + _schoolNo.ToString(), _formationNo);
		}
		public static int GetFormationNo(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.FORMATION_NO, _schoolNo, _type).formationNo;
		}
		private static void LoadFormationNo(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.formationNo == -1)
			{
				_data.formationNo = PlayerPrefs.GetInt(_type.ToString() + "_" + ParamType.FORMATION_NO.ToString() + "_" + _schoolNo.ToString(), 9999);
				if (_data.formationNo == 9999)
				{
					SetFormationNo(_schoolNo, UnityEngine.Random.Range(0, 3), _type);
				}
			}
		}
		public static void SetCharacters(int _schoolNo, CharacterData[] _data, DataType _type = DataType.OWN, bool _save = true)
		{
			for (int i = 0; i < GetSchoolData(ParamType.CHARACTERS, _schoolNo, _type).characters.Length; i++)
			{
				GetSchoolData(ParamType.CHARACTERS, _schoolNo, _type).characters[i].Copy(_data[i]);
			}
			for (int j = 0; j < 11; j++)
			{
				SetCharacterName(_schoolNo, j, _data[j].name, _type, _save: false);
				SetCharacterUniformNumber(_schoolNo, j, _data[j].uniformNumber, _type, _save: false);
				SetCharacterItemNo(_schoolNo, j, _data[j].itemNo, _type, _save: false);
			}
		}
		public static void SetCharacterData(int _schoolNo, int _characterNo, string _name, int _uniformNumber, int _itemNo, DataType _type = DataType.OWN, bool _save = true)
		{
			SetCharacterName(_schoolNo, _characterNo, _name, _type, _save: false);
			SetCharacterUniformNumber(_schoolNo, _characterNo, _uniformNumber, _type, _save: false);
			SetCharacterItemNo(_schoolNo, _characterNo, _itemNo, _type, _save: false);
		}
		public static CharacterData[] GetCharacters(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.CHARACTERS, _schoolNo, _type).characters;
		}
		public static void SetCharacterName(int _schoolNo, int _characterNo, string _name, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.CHARACTER_NAME, _schoolNo, _type).characters[_characterNo].name = _name;
			PlayerPrefs.SetString(_type.ToString() + "_" + ParamType.CHARACTER_NAME.ToString() + "_" + _schoolNo.ToString() + "_" + _characterNo.ToString(), _name);
		}
		public static string GetCharacterName(int _schoolNo, int _characterNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.CHARACTER_NAME, _schoolNo, _type).characters[_characterNo].name;
		}
		public static void SetCharacterUniformNumber(int _schoolNo, int _characterNo, int _uniformNumber, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.CHARACTER_UNIFORM_NUMBER, _schoolNo, _type).characters[_characterNo].uniformNumber = _uniformNumber;
			PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.CHARACTER_UNIFORM_NUMBER.ToString() + "_" + _schoolNo.ToString() + "_" + _characterNo.ToString(), _uniformNumber);
		}
		public static int GetCharacterUniformNumber(int _schoolNo, int _characterNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.CHARACTER_UNIFORM_NUMBER, _schoolNo, _type).characters[_characterNo].uniformNumber;
		}
		public static void SetCharacterItemNo(int _schoolNo, int _characterNo, int _itemNo, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.CHARACTER_ITEM_NO, _schoolNo, _type).characters[_characterNo].itemNo = _itemNo;
			PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.CHARACTER_ITEM_NO.ToString() + "_" + _schoolNo.ToString() + "_" + _characterNo.ToString(), _itemNo);
		}
		public static int GetCharacterItemNo(int _schoolNo, int _characterNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.CHARACTER_ITEM_NO, _schoolNo, _type).characters[_characterNo].itemNo;
		}
		private static void LoadCharacterData(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.characters != null)
			{
				return;
			}
			_data.characters = new CharacterData[11];
			for (int i = 0; i < _data.characters.Length; i++)
			{
				_data.characters[i] = new CharacterData();
				_data.characters[i].name = PlayerPrefs.GetString(_type.ToString() + "_" + ParamType.CHARACTER_NAME.ToString() + "_" + _schoolNo.ToString() + "_" + i.ToString(), " ");
				if (_data.characters[i].name == " ")
				{
					_data.characters[i].name = GetRandomCharacterName();
					PlayerPrefs.SetString(_type.ToString() + "_" + ParamType.CHARACTER_NAME.ToString() + "_" + _schoolNo.ToString() + "_" + i.ToString(), _data.characters[i].name);
				}
				_data.characters[i].uniformNumber = PlayerPrefs.GetInt(_type.ToString() + "_" + ParamType.CHARACTER_UNIFORM_NUMBER.ToString() + "_" + _schoolNo.ToString() + "_" + i.ToString(), i + 1);
				_data.characters[i].itemNo = PlayerPrefs.GetInt(_type.ToString() + "_" + ParamType.CHARACTER_ITEM_NO.ToString() + "_" + _schoolNo.ToString() + "_" + i.ToString(), 0);
			}
		}
		public static void SetTeamStatusData(int _schoolNo, TeamStatusData _data, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.TEAM_STATUS, _schoolNo, _type).teamStatus.Copy(_data);
			SetTeamStatusParams(_schoolNo, _data.statusParams, _type);
		}
		public static TeamStatusData GetTeamStatusData(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.TEAM_STATUS, _schoolNo, _type).teamStatus;
		}
		public static void SetTeamStatusParams(int _schoolNo, int[] _teamStatus, DataType _type = DataType.OWN, bool _save = false)
		{
			_teamStatus.CopyTo(GetSchoolData(ParamType.TEAM_STATUS, _schoolNo, _type).teamStatus.statusParams, 0);
			for (int i = 0; i < GetSchoolData(ParamType.TEAM_STATUS, _schoolNo, _type).teamStatus.statusParams.Length; i++)
			{
				PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.TEAM_STATUS_PARAMS.ToString() + "_" + _schoolNo.ToString() + "_" + i.ToString(), GetSchoolData(ParamType.TEAM_STATUS, _schoolNo, _type).teamStatus.statusParams[i]);
			}
		}
		public static void SetTeamStatusParam(int _schoolNo, int _typeIndex, int _typeParam, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.TEAM_STATUS_PARAMS, _schoolNo, _type).teamStatus.statusParams[_typeIndex] = _typeParam;
			PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.TEAM_STATUS_PARAMS.ToString() + "_" + _schoolNo.ToString() + "_" + _typeIndex.ToString(), GetSchoolData(ParamType.TEAM_STATUS_PARAMS, _schoolNo, _type).teamStatus.statusParams[_typeIndex]);
		}
		public static int[] GetTeamStatusParams(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.TEAM_STATUS_PARAMS, _schoolNo, _type).teamStatus.statusParams;
		}
		public static int GetTeamStatusParam(int _schoolNo, int _typeIndex, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.TEAM_STATUS_PARAMS, _schoolNo, _type).teamStatus.statusParams[_typeIndex];
		}
		private static void LoadTeamStatus(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.teamStatus == null)
			{
				_data.teamStatus = new TeamStatusData();
				for (int i = 0; i < _data.teamStatus.statusParams.Length; i++)
				{
					_data.teamStatus.statusParams[i] = PlayerPrefs.GetInt(_type.ToString() + "_" + ParamType.TEAM_STATUS_PARAMS.ToString() + "_" + _schoolNo.ToString() + "_" + i.ToString(), 3);
				}
			}
		}
		public static StatusType GetTeamStatus(int _schoolNo, DataType _type = DataType.OWN)
		{
			int[] statusParams = GetSchoolData(ParamType.TEAM_STATUS, _schoolNo, _type).teamStatus.statusParams;
			StatusType result = default(StatusType);
			result.offense = statusParams[0];
			result.defense = statusParams[1];
			result.speed = statusParams[2];
			result.stamina = statusParams[3];
			return result;
		}
		public static StatusType[] GetCharaStatus(int _schoolNo, DataType _type = DataType.OWN)
		{
			StatusType[] array = new StatusType[11];
			int[] statusParams = GetSchoolData(ParamType.TEAM_STATUS, _schoolNo, _type).teamStatus.statusParams;
			StatusType[] array2 = new StatusType[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = default(StatusType);
				array[i].offense = statusParams[0] + array2[i].offense;
				array[i].defense = statusParams[1] + array2[i].defense;
				array[i].speed = statusParams[2] + array2[i].speed;
				array[i].stamina = statusParams[3] + array2[i].stamina;
			}
			return array;
		}
		public static StatusType GetCharaStatus(int _schoolNo, int _charaNo, DataType _type = DataType.OWN)
		{
			int[] statusParams = GetSchoolData(ParamType.TEAM_STATUS, _schoolNo, _type).teamStatus.statusParams;
			StatusType statusType = default(StatusType);
			StatusType result = default(StatusType);
			result.offense = statusParams[0] + statusType.offense;
			result.defense = statusParams[1] + statusType.defense;
			result.speed = statusParams[2] + statusType.speed;
			result.stamina = statusParams[3] + statusType.stamina;
			return result;
		}
		public static void SetFoundingYear(int _schoolNo, int _foundingYear, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.FOUNDING_YEAR, _schoolNo, _type).foundingYear = _foundingYear;
			PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.FOUNDING_YEAR.ToString() + "_" + _schoolNo.ToString(), _foundingYear);
		}
		public static int GetFoundingYear(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.FOUNDING_YEAR, _schoolNo, _type).foundingYear;
		}
		private static void LoadFoundingYear(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.foundingYear == -1)
			{
				_data.foundingYear = PlayerPrefs.GetInt(_type.ToString() + "_" + ParamType.FOUNDING_YEAR.ToString() + "_" + _schoolNo.ToString(), 0);
			}
		}
		public static void SetPrefectures(int _schoolNo, int _prefectures, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.PREFECTURES, _schoolNo, _type).prefectures = _prefectures;
			PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.PREFECTURES.ToString() + "_" + _schoolNo.ToString(), _prefectures);
		}
		public static int GetPrefectures(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.PREFECTURES, _schoolNo, _type).prefectures;
		}
		private static void LoadPrefectures(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.prefectures == -1)
			{
				_data.prefectures = PlayerPrefs.GetInt(_type.ToString() + "_" + ParamType.PREFECTURES.ToString() + "_" + _schoolNo.ToString(), 0);
			}
		}
		public static void SetTacticsType(int _schoolNo, int _tacticsType, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.TACTICS_TYPE, _schoolNo, _type).tacticsType = _tacticsType;
			PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.TACTICS_TYPE.ToString() + "_" + _schoolNo.ToString(), _tacticsType);
		}
		public static int GetTacticsType(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.TACTICS_TYPE, _schoolNo, _type).tacticsType;
		}
		private static void LoadTacticsType(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.tacticsType == -1)
			{
				_data.tacticsType = PlayerPrefs.GetInt(_type.ToString() + "_" + ParamType.TACTICS_TYPE.ToString() + "_" + _schoolNo.ToString(), 0);
			}
		}
		public static void SetGameRecord(int _schoolNo, string _gameRecord, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecord = _gameRecord.Split(',');
			GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordWin = int.Parse(GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecord[0]);
			GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordLose = int.Parse(GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecord[1]);
			GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordDraw = int.Parse(GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecord[2]);
			PlayerPrefs.SetString(_type.ToString() + "_" + ParamType.GAME_RECORD.ToString() + "_" + _schoolNo.ToString(), _gameRecord);
		}
		public static void SetGameRecord(int _schoolNo, int _win, int _lose, int _draw, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecord = new string[3]
			{
				_win.ToString(),
				_lose.ToString(),
				_draw.ToString()
			};
			GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordWin = int.Parse(GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecord[0]);
			GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordLose = int.Parse(GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecord[1]);
			GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordDraw = int.Parse(GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecord[2]);
			PlayerPrefs.SetString(_type.ToString() + "_" + ParamType.GAME_RECORD.ToString() + "_" + _schoolNo.ToString(), _win.ToString() + "," + _lose.ToString() + "," + _draw.ToString());
		}
		public static string GetGameRecord(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordWin.ToString() + "," + GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordLose.ToString() + "," + GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordDraw.ToString();
		}
		public static int GetGameRecordWin(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordWin;
		}
		public static int GetGameRecordLose(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordLose;
		}
		public static int GetGameRecordDraw(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordDraw;
		}
		public static float GetWinningPercentage(int _schoolNo, DataType _type = DataType.OWN)
		{
			if (GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordWin + GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordLose + GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordDraw == 0)
			{
				return -1f;
			}
			return (float)GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordWin / ((float)GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordWin + (float)GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordLose + (float)GetSchoolData(ParamType.GAME_RECORD, _schoolNo, _type).gameRecordDraw);
		}
		private static void LoadGameRecord(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.gameRecord == null)
			{
				_data.gameRecord = PlayerPrefs.GetString(_type.ToString() + "_" + ParamType.GAME_RECORD.ToString() + "_" + _schoolNo.ToString(), " ").Split(',');
				if (_data.gameRecord.Length != 3)
				{
					SetGameRecord(_schoolNo, "0,0,0", _type);
					_data.gameRecordWin = 0;
					_data.gameRecordLose = 0;
					_data.gameRecordDraw = 0;
				}
				else
				{
					_data.gameRecordWin = int.Parse(_data.gameRecord[0]);
					_data.gameRecordLose = int.Parse(_data.gameRecord[1]);
					_data.gameRecordDraw = int.Parse(_data.gameRecord[2]);
				}
			}
		}
		public static void SetIsUseNationalExpedition(int _schoolNo, bool _isUse, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.IS_USE_NATIONAL_EXPEDITION, _schoolNo, _type).isUseNationalExpedition = (_isUse ? 1 : 0);
			PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.IS_USE_NATIONAL_EXPEDITION.ToString() + "_" + _schoolNo.ToString(), _isUse ? 1 : 0);
		}
		public static bool IsUseNationalExpedition(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.IS_USE_NATIONAL_EXPEDITION, _schoolNo, _type).isUseNationalExpedition == 1;
		}
		private static void LoadIsUseNationalExpedition(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.isUseNationalExpedition == -1)
			{
				_data.isUseNationalExpedition = PlayerPrefs.GetInt(_type.ToString() + "_" + ParamType.IS_USE_NATIONAL_EXPEDITION.ToString() + "_" + _schoolNo.ToString(), 0);
			}
		}
		public static void SetIsUseTournament(int _schoolNo, bool _isUse, DataType _type = DataType.OWN, bool _save = true)
		{
			GetSchoolData(ParamType.IS_USE_TOURNAMENT, _schoolNo, _type).isUseTournament = (_isUse ? 1 : 0);
			PlayerPrefs.SetInt(_type.ToString() + "_" + ParamType.IS_USE_TOURNAMENT.ToString() + "_" + _schoolNo.ToString(), _isUse ? 1 : 0);
		}
		public static bool IsUseTournament(int _schoolNo, DataType _type = DataType.OWN)
		{
			return GetSchoolData(ParamType.IS_USE_TOURNAMENT, _schoolNo, _type).isUseTournament == 1;
		}
		private static void LoadIsUseTournament(SchoolTeamData _data, int _schoolNo, DataType _type = DataType.OWN)
		{
			if (_data.isUseTournament == -1)
			{
				_data.isUseTournament = PlayerPrefs.GetInt(_type.ToString() + "_" + ParamType.IS_USE_TOURNAMENT.ToString() + "_" + _schoolNo.ToString(), 0);
			}
		}
		private static string GetDelimiterChar(DelimiterType _type)
		{
			return DELIMITER_CHAR[(int)_type];
		}
		public static string ConvertServerHumanDatas(int _schoolNo, DataType _type = DataType.OWN)
		{
			return ConvertServerHumanDatas(GetSchoolData(_schoolNo, _type));
		}
		public static string ConvertServerHumanDatas(SchoolTeamData _schoolTeamData)
		{
			convertData = "";
			for (int i = 0; i < 11; i++)
			{
				convertData += _schoolTeamData.characters[i].name;
				convertData += GetDelimiterChar(DelimiterType.PARAMETER);
				convertData += _schoolTeamData.characters[i].uniformNumber.ToString();
				convertData += GetDelimiterChar(DelimiterType.PARAMETER);
				convertData += _schoolTeamData.characters[i].itemNo.ToString();
				if (i < 10)
				{
					convertData += GetDelimiterChar(DelimiterType.INDEX);
				}
			}
			convertData += GetDelimiterChar(DelimiterType.DATA);
			for (int j = 0; j < 4; j++)
			{
				convertData += _schoolTeamData.teamStatus.statusParams[j].ToString();
				if (j < 3)
				{
					convertData += GetDelimiterChar(DelimiterType.INDEX);
				}
			}
			return convertData;
		}
		public static void RestoreServerDatas(int _schoolNo, string _schoolName, int _uniformNo, int _formationNo, string _serverData, int _foundingYear, int _prefectures, int _tacticsType, int _gameRecordWin, int _gameRecordLose, int _gameRecordDraw, DataType _type = DataType.OWN)
		{
			SetSchoolName(_schoolNo, _schoolName, _type, _save: false);
			SetUniformNo(_schoolNo, _uniformNo, _type, _save: false);
			SetFormationNo(_schoolNo, _formationNo, _type, _save: false);
			RestoreServerHumanDatas(_serverData, _schoolNo, _type);
			SetFoundingYear(_schoolNo, _foundingYear, _type, _save: false);
			SetPrefectures(_schoolNo, _prefectures, _type, _save: false);
			SetTacticsType(_schoolNo, _tacticsType, _type, _save: false);
			SetGameRecord(_schoolNo, _gameRecordWin, _gameRecordLose, _gameRecordDraw, _type, _save: false);
		}
		public static void RestoreServerHumanDatas(string _serverData, int _schoolNo, DataType _type = DataType.OWN)
		{
			serverData.datas = _serverData.Split(new string[1]
			{
				GetDelimiterChar(DelimiterType.DATA)
			}, StringSplitOptions.RemoveEmptyEntries);
			serverData.characters = serverData.datas[0].Split(new string[1]
			{
				GetDelimiterChar(DelimiterType.INDEX)
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < serverData.characters.Length; i++)
			{
				serverData.characterParams = serverData.characters[i].Split(new string[1]
				{
					GetDelimiterChar(DelimiterType.PARAMETER)
				}, StringSplitOptions.RemoveEmptyEntries);
				SetCharacterData(_schoolNo, i, serverData.characterParams[0], int.Parse(serverData.characterParams[1]), int.Parse(serverData.characterParams[2]), _type, _save: false);
			}
			serverData.teamStatusParams = serverData.datas[1].Split(new string[1]
			{
				GetDelimiterChar(DelimiterType.INDEX)
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int j = 0; j < serverData.teamStatusParams.Length; j++)
			{
				SetTeamStatusParam(_schoolNo, j, int.Parse(serverData.teamStatusParams[j]), _type, _save: false);
			}
		}
		public static void CreateTournamentDamySchoolData(int _prefectures = -1)
		{
			LoadNameList();
			if (tournamentDamySchoolTeamData == null)
			{
				tournamentDamySchoolTeamData = new SchoolTeamData();
			}
			tournamentDamySchoolTeamData.schoolName = GetRandomSchoolName(_tournament: true);
			if (tournamentDamySchoolTeamData.characters == null)
			{
				tournamentDamySchoolTeamData.characters = new CharacterData[11];
			}
			for (int i = 0; i < tournamentDamySchoolTeamData.characters.Length; i++)
			{
				if (tournamentDamySchoolTeamData.characters[i] == null)
				{
					tournamentDamySchoolTeamData.characters[i] = new CharacterData();
				}
				tournamentDamySchoolTeamData.characters[i].name = GetRandomCharacterName();
				tournamentDamySchoolTeamData.characters[i].uniformNumber = GetRandomUniformNumber(i);
				tournamentDamySchoolTeamData.characters[i].itemNo = GetRandomItemNo();
			}
			teamStatusPoint = 10;
			switch (GameSaveData.GetNowTournamentType())
			{
			case GameSaveData.TournamentType.PREFECTURAL_COMPE:
				teamStatusPoint = 10;
				break;
			case GameSaveData.TournamentType.REGIONAL_COMPE:
				teamStatusPoint = 20;
				break;
			case GameSaveData.TournamentType.NATIONAL_COMPE:
				teamStatusPoint = 30;
				break;
			}
			if (tournamentDamySchoolTeamData.teamStatus == null)
			{
				tournamentDamySchoolTeamData.teamStatus = new TeamStatusData();
			}
			tournamentDamySchoolTeamData.teamStatus.Reset();
			tournamentDamySchoolTeamData.gameRecordWin = UnityEngine.Random.Range(10, 100);
			tournamentDamySchoolTeamData.gameRecordLose = UnityEngine.Random.Range(10, 100);
			tournamentDamySchoolTeamData.gameRecordDraw = UnityEngine.Random.Range(10, 50);
			tournamentDamySchoolTeamData.tacticsType = UnityEngine.Random.Range(0, 4);
			if (_prefectures == -1)
			{
				tournamentDamySchoolTeamData.prefectures = UnityEngine.Random.Range(0, 47);
			}
			else
			{
				tournamentDamySchoolTeamData.prefectures = _prefectures;
			}
		}
		public static SchoolTeamData GetTournamentDamySchoolData()
		{
			return tournamentDamySchoolTeamData;
		}
		private static void LoadNameList()
		{
			if (schoolNameList == null)
			{
				schoolNameList = new string[1];
				schoolNameList[0] = "チ\u30fcム";
			}
			if (firstNameList == null)
			{
				firstNameList = new string[1];
				firstNameList[0] = "SAT";
			}
			if (lastNameList == null)
			{
				lastNameList = new string[1];
				lastNameList[0] = "BOX";
			}
		}
		public static void CreateEnemyData(int _playerUniformNo = -1)
		{
			LoadNameList();
			if (enemySchoolTeamData == null)
			{
				enemySchoolTeamData = new SchoolTeamData();
			}
			enemySchoolTeamData.schoolName = GetRandomSchoolName();
			if (enemySchoolTeamData.characters == null)
			{
				enemySchoolTeamData.characters = new CharacterData[11];
			}
			for (int i = 0; i < enemySchoolTeamData.characters.Length; i++)
			{
				if (enemySchoolTeamData.characters[i] == null)
				{
					enemySchoolTeamData.characters[i] = new CharacterData();
				}
				enemySchoolTeamData.characters[i].name = GetRandomCharacterName();
				enemySchoolTeamData.characters[i].uniformNumber = GetRandomUniformNumber(i);
				enemySchoolTeamData.characters[i].itemNo = GetRandomItemNo();
			}
			teamStatusPoint = 12;
			switch (GameSaveData.GetCpuStrength())
			{
			case 3:
				teamStatusPoint = 20;
				break;
			case 4:
				teamStatusPoint = 30;
				break;
			}
			if (enemySchoolTeamData.teamStatus == null)
			{
				enemySchoolTeamData.teamStatus = new TeamStatusData();
			}
			enemySchoolTeamData.teamStatus.Reset();
		}
		public static SchoolTeamData GetEnemySchoolData()
		{
			return enemySchoolTeamData;
		}
		public static string GetEnemySchoolName(bool _short = true)
		{
			if (_short)
			{
				if (enemySchoolTeamData.schoolName.Length > 3)
				{
					return enemySchoolTeamData.schoolName.Substring(0, 3);
				}
				return enemySchoolTeamData.schoolName;
			}
			return enemySchoolTeamData.schoolName;
		}
		public static int GetEnemyUniformNo()
		{
			return enemySchoolTeamData.uniformNo;
		}
		public static int GetEnemyBallNo()
		{
			return enemySchoolTeamData.ballNo;
		}
		public static int GetEnemyFormationNo()
		{
			return enemySchoolTeamData.formationNo;
		}
		public static CharacterData[] GetEnemyCharacters()
		{
			return enemySchoolTeamData.characters;
		}
		public static CharacterData GetEnemyCharacter(int _characterNo)
		{
			return enemySchoolTeamData.characters[_characterNo];
		}
		public static string GetEnemyCharacterName(int _characterNo)
		{
			return enemySchoolTeamData.characters[_characterNo].name;
		}
		public static int GetEnemyCharacterUniformNumber(int _characterNo)
		{
			return enemySchoolTeamData.characters[_characterNo].uniformNumber;
		}
		public static int GetEnemyCharacterItemNo(int _characterNo)
		{
			return enemySchoolTeamData.characters[_characterNo].itemNo;
		}
		public static TeamStatusData GetEnemyTeamStatusData()
		{
			return enemySchoolTeamData.teamStatus;
		}
		public static int[] GetEnemyTeamStatusParams()
		{
			return enemySchoolTeamData.teamStatus.statusParams;
		}
		public static int GetEnemyTeamStatusParam(int _typeIndex)
		{
			return enemySchoolTeamData.teamStatus.statusParams[_typeIndex];
		}
		public static StatusType GetEnemyTeamStatus()
		{
			int[] statusParams = enemySchoolTeamData.teamStatus.statusParams;
			StatusType result = default(StatusType);
			result.offense = statusParams[0];
			result.defense = statusParams[1];
			result.speed = statusParams[2];
			result.stamina = statusParams[3];
			return result;
		}
		public static StatusType[] GetEnemyCharaStatus()
		{
			StatusType[] array = new StatusType[11];
			int[] statusParams = enemySchoolTeamData.teamStatus.statusParams;
			StatusType[] array2 = new StatusType[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = default(StatusType);
				array[i].offense = statusParams[0] + array2[i].offense;
				array[i].defense = statusParams[1] + array2[i].defense;
				array[i].speed = statusParams[2] + array2[i].speed;
				array[i].stamina = statusParams[3] + array2[i].stamina;
			}
			return array;
		}
		public static StatusType GetEnemyCharaStatus(int _charaNo)
		{
			int[] statusParams = enemySchoolTeamData.teamStatus.statusParams;
			StatusType statusType = default(StatusType);
			StatusType result = default(StatusType);
			result.offense = statusParams[0] + statusType.offense;
			result.defense = statusParams[1] + statusType.defense;
			result.speed = statusParams[2] + statusType.speed;
			result.stamina = statusParams[3] + statusType.stamina;
			return result;
		}
		public static void CreateLeaveData(int _createNum)
		{
			LoadNameList();
			UnityEngine.Debug.Log("CreateLeaveData:" + _createNum.ToString());
			for (int i = 0; i < _createNum; i++)
			{
				if (leaveSchoolTeamData.Count < _createNum)
				{
					leaveSchoolTeamData.Add(new SchoolTeamData());
				}
				if (GetSelectSchoolNo(i) != 8)
				{
					continue;
				}
				leaveSchoolTeamData[i].schoolName = GetRandomSchoolName();
				if (leaveSchoolTeamData[i].characters == null)
				{
					leaveSchoolTeamData[i].characters = new CharacterData[11];
				}
				for (int j = 0; j < leaveSchoolTeamData[i].characters.Length; j++)
				{
					if (leaveSchoolTeamData[i].characters[j] == null)
					{
						leaveSchoolTeamData[i].characters[j] = new CharacterData();
					}
					leaveSchoolTeamData[i].characters[j].name = GetRandomCharacterName();
					leaveSchoolTeamData[i].characters[j].uniformNumber = GetRandomUniformNumber(j);
					leaveSchoolTeamData[i].characters[j].itemNo = GetRandomItemNo();
				}
				teamStatusPoint = GameSaveData.GetTeamStatusPoint();
				if (leaveSchoolTeamData[i].teamStatus == null)
				{
					leaveSchoolTeamData[i].teamStatus = new TeamStatusData();
				}
				leaveSchoolTeamData[i].teamStatus.Reset();
				UnityEngine.Debug.Log("ステ\u30fcタス設定:" + i.ToString());
			}
		}
		public static SchoolTeamData GetCommonSchoolData(int _player, ParamType _type, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			if (_mode == GameSaveData.MainGameMode.NONE)
			{
				_mode = GameSaveData.GetSelectMainGameMode();
			}
			switch (_mode)
			{
			case GameSaveData.MainGameMode.SINGLE:
				if (_player == 0)
				{
					if (GetSelectSchoolNo(_player) == 8)
					{
						return leaveSchoolTeamData[_player];
					}
					return GetSchoolData(_type, GetSelectSchoolNo(_player));
				}
				if (GetSelectSchoolNo(_player) == 8)
				{
					return GetEnemySchoolData();
				}
				return GetSchoolData(_type, GetSelectSchoolNo(_player));
			case GameSaveData.MainGameMode.NETWORK:
				if (_player == 0)
				{
					return GetSchoolData(_type, 0, DataType.NETWORK_PROFILE);
				}
				return GetSchoolData(_type, 0, DataType.BATTLE_OPPONENT);
			case GameSaveData.MainGameMode.TOURNAMENT:
				if (_player == 0)
				{
					return GetSchoolData(_type, 0, DataType.TOURNAMENT_OWN);
				}
				return GetSchoolData(_type, 0, DataType.BATTLE_OPPONENT);
			default:
				if (GetSelectSchoolNo(_player) == 8)
				{
					return leaveSchoolTeamData[_player];
				}
				return GetSchoolData(_type, GetSelectSchoolNo(_player));
			}
		}
		public static string GetCommonSchoolName(int _player, bool _short = true, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			if (_short)
			{
				if (GetCommonSchoolData(_player, ParamType.SCHOOL_NAME, _mode).schoolName.Length > 4)
				{
					return GetCommonSchoolData(_player, ParamType.SCHOOL_NAME, _mode).schoolName.Substring(0, 3);
				}
				return GetCommonSchoolData(_player, ParamType.SCHOOL_NAME, _mode).schoolName.Substring(0, GetCommonSchoolData(_player, ParamType.SCHOOL_NAME, _mode).schoolName.Length - 2);
			}
			return GetCommonSchoolData(_player, ParamType.SCHOOL_NAME, _mode).schoolName;
		}
		public static int GetCommonUniformNo(int _player, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			return GetCommonSchoolData(_player, ParamType.UNIFORM_NO, _mode).uniformNo;
		}
		public static int GetCommonBallNo(int _player, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			return GetCommonSchoolData(_player, ParamType.BALL_NO, _mode).ballNo;
		}
		public static int GetCommonFormationNo(int _player, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			return GetCommonSchoolData(_player, ParamType.FORMATION_NO, _mode).formationNo;
		}
		public static CharacterData[] GetCommonCharacters(int _player, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			return GetCommonSchoolData(_player, ParamType.CHARACTERS, _mode).characters;
		}
		public static CharacterData GetCommonCharacter(int _player, int _characterNo, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			return GetCommonSchoolData(_player, ParamType.CHARACTERS, _mode).characters[_characterNo];
		}
		public static string GetCommonCharacterName(int _player, int _characterNo, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			UnityEngine.Debug.Log("プレイヤ\u30fc." + _player.ToString() + "番 : " + _characterNo.ToString() + "番 = " + GetCommonCharacter(_player, _characterNo, _mode).name);
			return GetCommonCharacter(_player, _characterNo, _mode).name;
		}
		public static int GetCommonCharacterUniformNumber(int _player, int _characterNo, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			return GetCommonCharacter(_player, _characterNo, _mode).uniformNumber;
		}
		public static int GetCommonCharacterItemNo(int _player, int _characterNo, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			return GetCommonCharacter(_player, _characterNo, _mode).itemNo;
		}
		public static TeamStatusData GetCommonTeamStatusData(int _player, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			return GetCommonSchoolData(_player, ParamType.TEAM_STATUS, _mode).teamStatus;
		}
		public static int[] GetCommonTeamStatusParams(int _player, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			return GetCommonSchoolData(_player, ParamType.TEAM_STATUS_PARAMS, _mode).teamStatus.statusParams;
		}
		public static int GetCommonTeamStatusParam(int _player, int _typeIndex, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			return GetCommonSchoolData(_player, ParamType.TEAM_STATUS_PARAMS, _mode).teamStatus.statusParams[_typeIndex];
		}
		public static StatusType GetCommonTeamStatus(int _player, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			int[] statusParams = GetCommonSchoolData(_player, ParamType.TEAM_STATUS_PARAMS, _mode).teamStatus.statusParams;
			StatusType result = default(StatusType);
			result.offense = statusParams[0];
			result.defense = statusParams[1];
			result.speed = statusParams[2];
			result.stamina = statusParams[3];
			return result;
		}
		public static StatusType[] GetCommonCharaStatus(int _player, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			StatusType[] array = new StatusType[11];
			int[] statusParams = GetCommonSchoolData(_player, ParamType.TEAM_STATUS_PARAMS, _mode).teamStatus.statusParams;
			StatusType[] array2 = new StatusType[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = default(StatusType);
				array[i].offense = statusParams[0] + array2[i].offense;
				array[i].defense = statusParams[1] + array2[i].defense;
				array[i].speed = statusParams[2] + array2[i].speed;
				array[i].stamina = statusParams[3] + array2[i].stamina;
			}
			return array;
		}
		public static StatusType GetCommonCharaStatus(int _player, int _charaNo, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			int[] statusParams = GetCommonSchoolData(_player, ParamType.TEAM_STATUS_PARAMS, _mode).teamStatus.statusParams;
			StatusType statusType = default(StatusType);
			StatusType result = default(StatusType);
			result.offense = statusParams[0] + statusType.offense;
			result.defense = statusParams[1] + statusType.defense;
			result.speed = statusParams[2] + statusType.speed;
			result.stamina = statusParams[3] + statusType.stamina;
			return result;
		}
		public static float GetCommonWinningPercentage(int _player, GameSaveData.MainGameMode _mode = GameSaveData.MainGameMode.NONE)
		{
			if (GetCommonSchoolData(_player, ParamType.GAME_RECORD, _mode).gameRecordWin + GetCommonSchoolData(_player, ParamType.GAME_RECORD, _mode).gameRecordLose + GetCommonSchoolData(_player, ParamType.GAME_RECORD, _mode).gameRecordDraw == 0)
			{
				return 0.5f;
			}
			return (float)GetCommonSchoolData(_player, ParamType.GAME_RECORD, _mode).gameRecordWin / ((float)GetCommonSchoolData(_player, ParamType.GAME_RECORD, _mode).gameRecordWin + (float)GetCommonSchoolData(_player, ParamType.GAME_RECORD, _mode).gameRecordLose + (float)GetCommonSchoolData(_player, ParamType.GAME_RECORD, _mode).gameRecordDraw);
		}
		private static string GetRandomSchoolName(bool _tournament = false)
		{
			string text = "";
			if (!(IsPer(70) | _tournament))
			{
				text = ((!IsPer(30)) ? prefectoralCapitalList[UnityEngine.Random.Range(0, prefectoralCapitalList.Length)] : prefecturesList[UnityEngine.Random.Range(0, prefecturesList.Length)]);
			}
			else
			{
				text = schoolNameList[UnityEngine.Random.Range(0, schoolNameList.Length)];
				if (IsPer(5))
				{
					text += NAME_OPTION_MANSION;
				}
			}
			if (IsPer(20) && !CheckDirectionChar(text))
			{
				text += directionList[UnityEngine.Random.Range(0, directionList.Length)];
			}
			if (IsPer(20) && text.Length <= 6)
			{
				text += numberingList[UnityEngine.Random.Range(0, numberingList.Length)];
			}
			if (text.Length <= 4)
			{
				return text + schoolNameTypeList[UnityEngine.Random.Range(0, schoolNameTypeList.Length)];
			}
			return text + schoolNameTypeList[UnityEngine.Random.Range(0, 3)];
		}
		public static bool CheckDirectionChar(string _text)
		{
			for (int i = 0; i < directionList.Length; i++)
			{
				if (_text.IndexOf(directionList[i]) >= 0)
				{
					return true;
				}
			}
			return false;
		}
		private static string GetRandomCharacterName()
		{
			GetRandomLastName();
			return "SAT-BOX";
		}
		private static string GetRandomLastName()
		{
			return lastNameList[UnityEngine.Random.Range(0, lastNameList.Length)];
		}
		private static string GetRandomFirstName()
		{
			return firstNameList[UnityEngine.Random.Range(0, firstNameList.Length)];
		}
		private static bool IsPer(int _per)
		{
			return UnityEngine.Random.Range(0, 100) <= _per;
		}
		private static int GetRandomUniformNumber(int _no)
		{
			if (_no == 0)
			{
				randomUniformNoList = new int[99];
				for (int i = 0; i < randomUniformNoList.Length; i++)
				{
					randomUniformNoList[i] = i;
				}
				for (int j = 0; j < randomUniformNoList.Length; j++)
				{
					int num = UnityEngine.Random.Range(0, randomUniformNoList.Length);
					int num2 = randomUniformNoList[j];
					randomUniformNoList[j] = randomUniformNoList[num];
					randomUniformNoList[num] = num2;
				}
			}
			return randomUniformNoList[_no] + 1;
		}
		private static int GetRandomItemNo()
		{
			return 0;
		}
	}
}
