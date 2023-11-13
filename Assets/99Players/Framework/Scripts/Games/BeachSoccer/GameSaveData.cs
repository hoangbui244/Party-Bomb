using System;
using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class GameSaveData
	{
		public enum MainGameMode
		{
			SINGLE,
			MULTI,
			NETWORK,
			TUTORIAL,
			SCHOOL_EDIT,
			MINI_GAME,
			TOURNAMENT,
			NONE
		}
		public enum CameraMode
		{
			VERTICAL,
			HORIZONTAL
		}
		public struct BatUseNumList
		{
			public int[,] batUseNumList;
			public void Init(int _batNum)
			{
				batUseNumList = new int[_batNum, 3];
			}
		}
		public struct ItemUseNumList
		{
			public int[] itemUseNumList;
			public void Init(int _itemNum)
			{
				itemUseNumList = new int[_itemNum];
			}
		}
		public enum TournamentType
		{
			PREFECTURAL_COMPE,
			REGIONAL_COMPE,
			NATIONAL_COMPE,
			MAX
		}
		public enum RACKET_DATA_IDX
		{
			NO,
			COLOR
		}
		public enum CharacterType
		{
			GIRL,
			BOY,
			RANDOM
		}
		public enum BAT_COLOR_TYPE
		{
			COLOR_0,
			COLOR_1,
			COLOR_2,
			MAX
		}
		private const string NONE_DATA = " ";
		private static MainGameMode selectMainGameMode = MainGameMode.SINGLE;
		private static CameraMode selectCameraMode = CameraMode.VERTICAL;
		private static bool isEditSchoolData = false;
		private static int selectMatchTimeNum = 0;
		private static List<int>[] selectMultiPlayerList = new List<int>[2];
		private static int selectAreaNo = 0;
		private const string SELECT_BAT_NO = "SELECT_RACKET_NO";
		private static int schoolEditSelectBatNo = 0;
		private static BAT_COLOR_TYPE schoolEditSelectBatColor = BAT_COLOR_TYPE.COLOR_0;
		public static BatUseNumList[] batUseNumList = new BatUseNumList[8];
		private const string SELECT_ITEM_NO = "SELECT_ITEM_NO";
		private static int schoolEditSelectItemNo = 0;
		public static ItemUseNumList[] itemUseNumList = new ItemUseNumList[8];
		public static int[,] uniformUseNumList = new int[8, SingletonCustom<UniformListManager>.Instance.GetNum()];
		private const string SELECT_UNIFORM_NO = "SELECT_UNIFORM_NO";
		private static int schoolEditSelectUniformNo = 0;
		private static int uniformSelectPlayrNo = 0;
		public static int[,] formationUseNumList = new int[8, SingletonCustom<FormationListManager>.Instance.GetNum()];
		private const string SELECT_FORMATION_NO = "SELECT_FORMATION_NO";
		private static int schoolEditSelectFormationNo = 0;
		private static int formationSelectPlayrNo = 0;
		private const string SELECT_TEAM_STATUS = "SELECT_TEAM_STATUS";
		private static int[] schoolEditSelectTeamStatus = new int[4];
		private static int teamStatusSelectPlayrNo = 0;
		private static SchoolData.CharacterData[] schoolEditSelectCharaData = new SchoolData.CharacterData[11];
		private static bool isFirstPlayerFirstAttack = true;
		private static bool isVsCpu = true;
		private static GameDataParams.AiStrength cpuStrength = GameDataParams.AiStrength.WEAK;
		private const string SELECT_CPU_STRENGTH = "SELECT_CPU_STRENGTH";
		private static GameDataParams.AiTacticsType cpuTacticsType = GameDataParams.AiTacticsType.BALANCE;
		private const string NETWORK_BATTLE_RESULT = "NETWORK_BATTLE_RESULT";
		private const string NETWORK_BATTLE_WEEKLY_WIN_ACHIEVE = "NETWORK_BATTLE_WEEKLY_WIN_ACHIEVE";
		private const string NETWORK_BATTLE_WEEKLY_HIT_ACHIEVE = "NETWORK_BATTLE_WEEKLY_HIT_ACHIEVE";
		private const string NETWORK_BATTLE_WEEKLY_HOMERUN_ACHIEVE = "NETWORK_BATTLE_WEEKLY_HOMERUN_ACHIEVE";
		private const string NETWORK_BATTLE_WEEKLY_STRUCKOUT_ACHIEVE = "NETWORK_BATTLE_WEEKLY_STRUCKOUT_ACHIEVE";
		private const string TOURNAMENT_TYPE = "TOURNAMENT_TYPE";
		private const string TOURNAMENT_PT = "TOURNAMENT_PT";
		private const string TOURNAMENT_BATTLE_NUM = "TOURNAMENT_BATTLE_NUM";
		private const string TOURNAMENT_BATTLE_RESULT = "TOURNAMENT_BATTLE_RESULT";
		private const string TOURNAMENT_OPEN_DAY = "TOURNAMENT_OPEN_DAY";
		private const string TOURNAMENT_VICTORY_NUM = "TOURNAMENT_VICTORY_NUM";
		private const string TOURNAMENT_CONTINUITY_VICTORY_NUM = "TOURNAMENT_CONTINUITY_VICTORY_NUM";
		private static DateTime tournamentOpenDay = default(DateTime);
		private static string[] dayData;
		private const string TOURNAMENT_LOAD_OPPONENT = "TOURNAMENT_LOAD_OPPONENT";
		private const string TOURNAMENT_PT_NO = "TOURNAMENT_PT_NO";
		private const string TOURNAMENT_LOSE = "TOURNAMENT_LOSE";
		private const string TOURNAMENT_BATTLED = "TOURNAMENT_BATTLED";
		private static int tournamentBenchNo = 0;
		private static int selectChallengeStageNo = 0;
		private static int selectChallengeAreaNo = 0;
		private const string HAVE_POINT = "HAVE_POINT";
		private const string IS_BAT_UNLOCK = "IS_CLUB_UNLOCK";
		private const string IS_UNIFORM_UNLOCK = "IS_UNIFORM_UNLOCK";
		private const string IS_FORMATION_UNLOCK = "IS_FORMATION_UNLOCK";
		private const string IS_ITEM_UNLOCK = "IS_ITEM_UNLOCK";
		private const string RECORD_SCORE = "RECORD_SCORE";
		private const string SELECT_CHARACTER = "SELECT_CHARACTER";
		private const string PREF_TEAM_STATUS_POINT = "PREF_TEAM_STATUS_POINT";
		private const string PLAYERS_NAME = "PLAYERS_NAME";
		public static int NOT_PLAY_SCORE = 0;
		public static int[] INIT_OPEN_NO = new int[2]
		{
			0,
			1
		};
		private const string MAIN_GAME_TUTORIAL = "MAIN_GAME_TUTORIAL";
		private const string SINGLE_SUSPEND_DATA = "SINGLE_SUSPEND_DATA";
		private const string SINGLE_SUSPEND_AREA_OBJ = "SINGLE_SUSPEND_AREA_OBJ";
		public static void SetSelectMainGameMode(MainGameMode _mode)
		{
			selectMainGameMode = _mode;
		}
		public static MainGameMode GetSelectMainGameMode()
		{
			return selectMainGameMode;
		}
		public static bool CheckSelectMainGameMode(MainGameMode _mode)
		{
			return selectMainGameMode == _mode;
		}
		public static void SetSelectCameraMode(CameraMode _mode)
		{
			selectCameraMode = _mode;
		}
		public static CameraMode GetSelectCameraMode()
		{
			return selectCameraMode;
		}
		public static bool CheckSelectCameraMode(CameraMode _mode)
		{
			return selectCameraMode == _mode;
		}
		public static void SetEditSchoolData(bool _isEditSchoolData)
		{
			isEditSchoolData = _isEditSchoolData;
		}
		public static bool IsEditSchoolData()
		{
			return isEditSchoolData;
		}
		public static void SetSelectMatchTimeIndex(int _selectMatchTimeIndex)
		{
			selectMatchTimeNum = _selectMatchTimeIndex;
		}
		public static int GetSelectMatchTimeNum(bool _indexConvert = false)
		{
			if (_indexConvert)
			{
				return selectMatchTimeNum;
			}
			return GameDataParams.GetPlayMatchTime(selectMatchTimeNum);
		}
		public static void SetSelectMultiPlayerList(List<int>[] _selectMultiPlayerList)
		{
			selectMultiPlayerList = _selectMultiPlayerList;
			for (int i = 0; i < selectMultiPlayerList.Length; i++)
			{
				selectMultiPlayerList[i].Sort();
			}
		}
		public static List<int>[] GetSelectMultiPlayerList()
		{
			return selectMultiPlayerList;
		}
		public static List<int> GetSelectMultiTeamPlayer(int _teamNo)
		{
			return selectMultiPlayerList[_teamNo];
		}
		public static int GetSelectMultiPlayerNum()
		{
			int num = 0;
			for (int i = 0; i < selectMultiPlayerList.Length; i++)
			{
				num += selectMultiPlayerList[i].Count;
			}
			return num;
		}
		public static int GetSelectArea()
		{
			return selectAreaNo;
		}
		public static void SetSelectArea(int _areaNo, bool _save = true)
		{
			UnityEngine.Debug.Log("STAGE_no : " + _areaNo.ToString());
			selectAreaNo = _areaNo;
		}
		public static void SetIsVsCpu(bool _flg)
		{
			isVsCpu = _flg;
		}
		public static bool IsVsCpu()
		{
			return isVsCpu;
		}
		public static void SetUniformSelectPlayrNo(int _playerNo)
		{
			uniformSelectPlayrNo = _playerNo;
		}
		public static int GetUniformSelectPlayrNo()
		{
			return uniformSelectPlayrNo;
		}
		public static void SetFormationSelectPlayrNo(int _playerNo)
		{
			formationSelectPlayrNo = _playerNo;
		}
		public static int GetFormationSelectPlayrNo()
		{
			return formationSelectPlayrNo;
		}
		public static void SetTeamStatusSelectPlayrNo(int _playerNo)
		{
			teamStatusSelectPlayrNo = _playerNo;
		}
		public static int GetTeamStatusSelectPlayrNo()
		{
			return teamStatusSelectPlayrNo;
		}
		public static void SetFirstPlayerFirstAttack(bool _flg)
		{
			isFirstPlayerFirstAttack = _flg;
		}
		public static bool IsFirstPlayerFirstAttack()
		{
			return isFirstPlayerFirstAttack;
		}
		public static void SetCpuStrength(int _no, bool _save = true)
		{
			cpuStrength = (GameDataParams.AiStrength)_no;
		}
		public static void SetCpuStrength(GameDataParams.AiStrength _strength, bool _save = true)
		{
			cpuStrength = _strength;
		}
		public static int GetCpuStrength()
		{
			return (int)cpuStrength;
		}
		public static bool CheckCpuStrength(GameDataParams.AiStrength _strength)
		{
			return cpuStrength == _strength;
		}
		public static void SetCpuTacticsType(int _tacticsType)
		{
			cpuTacticsType = (GameDataParams.AiTacticsType)_tacticsType;
		}
		public static void SetCpuTacticsType(GameDataParams.AiTacticsType _tacticsType)
		{
			cpuTacticsType = _tacticsType;
		}
		public static int GetCpuTacticsType()
		{
			return (int)cpuTacticsType;
		}
		public static bool CheckCpuTacticsType(GameDataParams.AiTacticsType _tacticsType)
		{
			return cpuTacticsType == _tacticsType;
		}
		public static void SetSelectBatNo(int _no, BAT_COLOR_TYPE _color, int _playerNo = -1, bool _save = true)
		{
			string key = "SELECT_RACKET_NO" + ((_playerNo == -1) ? "" : _playerNo.ToString());
			string str = _no.ToString();
			int num = (int)_color;
			PlayerPrefs.SetString(key, str + "," + num.ToString());
		}
		public static int GetSelectBatNo(int _playerNo = -1)
		{
			return int.Parse(PlayerPrefs.GetString("SELECT_RACKET_NO" + ((_playerNo == -1) ? "" : _playerNo.ToString()), "0,0").Split(',')[0]);
		}
		public static BAT_COLOR_TYPE GetSelectBatColor(int _playerNo = -1)
		{
			return (BAT_COLOR_TYPE)int.Parse(PlayerPrefs.GetString("SELECT_RACKET_NO" + ((_playerNo == -1) ? "" : _playerNo.ToString()), "0,0").Split(',')[1]);
		}
		public static void SetSchoolEditSelectBatData(int _no, BAT_COLOR_TYPE _color)
		{
			schoolEditSelectBatNo = _no;
			schoolEditSelectBatColor = _color;
		}
		public static int GetSchoolEditSelectBatNo()
		{
			return schoolEditSelectBatNo;
		}
		public static BAT_COLOR_TYPE GetSchoolEditSelectBatColor()
		{
			return schoolEditSelectBatColor;
		}
		public static void ResetBatUseNumList()
		{
			for (int i = 0; i < batUseNumList.Length; i++)
			{
				for (int j = 0; j < batUseNumList[i].batUseNumList.GetLength(0); j++)
				{
					for (int k = 0; k < batUseNumList[i].batUseNumList.GetLength(1); k++)
					{
						batUseNumList[i].batUseNumList[j, k] = 0;
					}
				}
			}
		}
		public static void SetBatUseNumList(int _schoolNo, int _batNo, int _batColor)
		{
			batUseNumList[_schoolNo].batUseNumList[_batNo, _batColor]++;
		}
		public static BatUseNumList GetBatUseNumList(int _schoolNo)
		{
			return batUseNumList[_schoolNo];
		}
		public static BatUseNumList[] GetBatUseNumList()
		{
			return batUseNumList;
		}
		public static int GetBatUseNumList(int _batNo, int _batColor)
		{
			int num = 0;
			for (int i = 0; i < batUseNumList.Length; i++)
			{
				num += batUseNumList[i].batUseNumList[_batNo, _batColor];
			}
			return num;
		}
		public static void SetSelectItemNo(int _no, int _playerNo = -1, bool _save = true)
		{
			PlayerPrefs.SetInt("SELECT_ITEM_NO" + ((_playerNo == -1) ? "" : _playerNo.ToString()), _no);
		}
		public static int GetSelectItemNo(int _playerNo = -1)
		{
			return PlayerPrefs.GetInt("SELECT_ITEM_NO" + ((_playerNo == -1) ? "" : _playerNo.ToString()), 0);
		}
		public static void SetSchoolEditSelectItemData(int _no)
		{
			schoolEditSelectItemNo = _no;
		}
		public static int GetSchoolEditSelectItemNo()
		{
			return schoolEditSelectItemNo;
		}
		public static void ResetItemUseNumList()
		{
			for (int i = 0; i < itemUseNumList.Length; i++)
			{
				for (int j = 0; j < itemUseNumList[i].itemUseNumList.Length; j++)
				{
					itemUseNumList[i].itemUseNumList[j] = 0;
				}
			}
		}
		public static void SetItemUseNumList(int _schoolNo, int _itemNo)
		{
			itemUseNumList[_schoolNo].itemUseNumList[_itemNo]++;
		}
		public static ItemUseNumList GetItemUseNumList(int _schoolNo)
		{
			return itemUseNumList[_schoolNo];
		}
		public static ItemUseNumList[] GetItemUseNumList()
		{
			return itemUseNumList;
		}
		public static int GetItemUseNum(int _itemNo)
		{
			int num = 0;
			for (int i = 0; i < itemUseNumList.Length; i++)
			{
				num += itemUseNumList[i].itemUseNumList[_itemNo];
			}
			return num;
		}
		public static void SetSelectUniformNo(int _no, int _playerNo = -1, bool _save = true)
		{
			PlayerPrefs.SetInt("SELECT_UNIFORM_NO" + ((_playerNo == -1) ? "" : _playerNo.ToString()), _no);
		}
		public static int GetSelectUniformNo(int _playerNo = -1)
		{
			return PlayerPrefs.GetInt("SELECT_UNIFORM_NO" + ((_playerNo == -1) ? "" : _playerNo.ToString()), 0);
		}
		public static void SetSchoolEditSelectUniformNo(int _no)
		{
			schoolEditSelectUniformNo = _no;
		}
		public static int GetSchoolEditSelectUniformNo()
		{
			return schoolEditSelectUniformNo;
		}
		public static void ResetUniformUseNumList()
		{
			for (int i = 0; i < uniformUseNumList.GetLength(0); i++)
			{
				for (int j = 0; j < uniformUseNumList.GetLength(1); j++)
				{
					uniformUseNumList[i, j] = 0;
				}
			}
		}
		public static void AddUniformUseNumList(int _schoolNo, int _uniformNo)
		{
			uniformUseNumList[_schoolNo, _uniformNo]++;
		}
		public static int[,] GetUniformUseNumList()
		{
			return uniformUseNumList;
		}
		public static int GetUniformUseNumList(int _uniformNo)
		{
			int num = 0;
			for (int i = 0; i < uniformUseNumList.GetLength(0); i++)
			{
				num += uniformUseNumList[i, _uniformNo];
			}
			return num;
		}
		public static void SetSelectFormationNo(int _no, int _playerNo = -1, bool _save = true)
		{
			PlayerPrefs.SetInt("SELECT_FORMATION_NO" + ((_playerNo == -1) ? "" : _playerNo.ToString()), _no);
		}
		public static int GetSelectFormationNo(int _playerNo = -1)
		{
			return PlayerPrefs.GetInt("SELECT_FORMATION_NO" + ((_playerNo == -1) ? "" : _playerNo.ToString()), 0);
		}
		public static void SetSchoolEditSelectFormationNo(int _no)
		{
			schoolEditSelectFormationNo = _no;
		}
		public static int GetSchoolEditSelectFormationNo()
		{
			return schoolEditSelectFormationNo;
		}
		public static void ResetFormationUseNumList()
		{
			for (int i = 0; i < formationUseNumList.GetLength(0); i++)
			{
				for (int j = 0; j < formationUseNumList.GetLength(1); j++)
				{
					formationUseNumList[i, j] = 0;
				}
			}
		}
		public static void AddFormationUseNumList(int _schoolNo, int _formationNo)
		{
			formationUseNumList[_schoolNo, _formationNo]++;
		}
		public static int[,] GetFormationUseNumList()
		{
			return formationUseNumList;
		}
		public static int GetFormationUseNumList(int _ballNo)
		{
			int num = 0;
			for (int i = 0; i < formationUseNumList.GetLength(0); i++)
			{
				num += formationUseNumList[i, _ballNo];
			}
			return num;
		}
		public static void SetTeamStatus(int[] _no, int _playerNo = -1, bool _save = true)
		{
			for (int i = 0; i < 4; i++)
			{
				PlayerPrefs.SetInt("SELECT_TEAM_STATUS" + i.ToString() + ((_playerNo == -1) ? "" : _playerNo.ToString()), _no[i]);
			}
		}
		public static int[] GetTeamStatus(int _playerNo = -1)
		{
			int[] array = new int[4];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = PlayerPrefs.GetInt("SELECT_TEAM_STATUS" + i.ToString() + ((_playerNo == -1) ? "" : _playerNo.ToString()), 0);
			}
			return array;
		}
		public static void SetSchoolEditTeamStatus(int[] _no)
		{
			_no.CopyTo(schoolEditSelectTeamStatus, 0);
		}
		public static int[] GetSchoolEditTeamStatus()
		{
			return schoolEditSelectTeamStatus;
		}
		public static void SetSchoolEditSelectCharaData(SchoolData.CharacterData[] _charaData)
		{
			_charaData.CopyTo(schoolEditSelectCharaData, 0);
		}
		public static SchoolData.CharacterData[] GetSchoolEditSelectCharaData()
		{
			return schoolEditSelectCharaData;
		}
		public static void SetSelectChallengeAreaNo(int _no)
		{
			selectChallengeAreaNo = _no;
		}
		public static int GetSelectChallengeAreaNo()
		{
			return selectChallengeAreaNo;
		}
		public static void SetSelectChallengeStageNo(int _no)
		{
			selectChallengeStageNo = _no;
		}
		public static int GetSelectChallengeStageNo()
		{
			return selectChallengeStageNo;
		}
		public static int GetNetworkResult()
		{
			return PlayerPrefs.GetInt("NETWORK_BATTLE_RESULT", 0);
		}
		public static void SetNetworkResult(int _playerNo)
		{
			PlayerPrefs.SetInt("NETWORK_BATTLE_RESULT", _playerNo);
		}
		public static void SetTournamentOpenDay(DateTime _day, bool _save = true)
		{
			PlayerPrefs.SetString("TOURNAMENT_OPEN_DAY", _day.Year.ToString() + "/" + _day.Month.ToString() + "/" + _day.Day.ToString());
		}
		public static bool CheckStartTournamentOpenDay(DateTime _day)
		{
			return _day.Subtract(GetTournamentOpenDay()).Days >= 3;
		}
		public static int CheckTournamentOpenElapsedDay(DateTime _day)
		{
			return _day.Subtract(GetTournamentOpenDay()).Days;
		}
		public static bool CheckNextTournamentOpenDay(DateTime _day)
		{
			return _day.Subtract(GetTournamentOpenDay()).Days != (int)GetNowTournamentType();
		}
		public static int CheckNextTournamentOpenDays(DateTime _day)
		{
			return 3 - _day.Subtract(GetTournamentOpenDay()).Days;
		}
		public static DateTime GetTournamentOpenDay()
		{
			dayData = PlayerPrefs.GetString("TOURNAMENT_OPEN_DAY", "1/1/1").Split('/');
			tournamentOpenDay = new DateTime(int.Parse(dayData[0]), int.Parse(dayData[1]), int.Parse(dayData[2]));
			return tournamentOpenDay;
		}
		public static void ResetTournamentOpenDay(bool _save = true)
		{
			PlayerPrefs.SetString("TOURNAMENT_OPEN_DAY", "1/1/1");
		}
		public static void SetNowTournamentType(TournamentType _type = TournamentType.MAX, bool _save = true)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			PlayerPrefs.SetInt("TOURNAMENT_TYPE", (int)_type);
		}
		public static bool CheckNowTournamentType(TournamentType _type)
		{
			return PlayerPrefs.GetInt("TOURNAMENT_TYPE", 0) == (int)_type;
		}
		public static TournamentType GetNowTournamentType()
		{
			return TournamentType.NATIONAL_COMPE;
		}
		public static void SetTournamentPt(bool _pt, bool _save = true)
		{
			PlayerPrefs.SetInt("TOURNAMENT_PT", _pt ? 1 : 0);
		}
		public static bool IsTournamentPt()
		{
			return PlayerPrefs.GetInt("TOURNAMENT_PT", 0) == 1;
		}
		public static void SetTournamentBattleNum(int _battleNum, TournamentType _type = TournamentType.MAX, bool _save = true)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			PlayerPrefs.SetInt("TOURNAMENT_BATTLE_NUM_" + _type.ToString(), _battleNum);
		}
		public static void AddTournamentBattleNum(TournamentType _type = TournamentType.MAX, int _addBattleNum = 1, bool _save = true)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			PlayerPrefs.SetInt("TOURNAMENT_BATTLE_NUM_" + _type.ToString(), GetTournamentBattleNum(_type) + _addBattleNum);
		}
		public static int GetTournamentBattleNum(TournamentType _type = TournamentType.MAX)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			return PlayerPrefs.GetInt("TOURNAMENT_BATTLE_NUM_" + _type.ToString(), 0);
		}
		public static void SetTournamentBattleResult(int _myPoint, int _OpponentPoint, int _battleNum = -1, int _battleNo = -1, TournamentType _type = TournamentType.MAX, bool _playerResult = true, bool _save = true)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			if (_battleNum == -1)
			{
				_battleNum = GetTournamentBattleNum(_type);
			}
			if (_battleNo == -1)
			{
				_battleNo = ConvertPtNoToBattleNo(GetTournamentPtNo(_type), _battleNum, _type);
			}
			if (_playerResult)
			{
				UnityEngine.Debug.Log(_battleNum.ToString() + "回戦：第" + _battleNo.ToString() + "試合");
				UnityEngine.Debug.Log("ベンチ番号：" + ConvertPtNoToBenchNo(GetTournamentPtNo(_type), _battleNum, _type).ToString());
				UnityEngine.Debug.Log("得点：" + ((ConvertPtNoToBenchNo(GetTournamentPtNo(_type), _battleNum, _type) == 1) ? (_OpponentPoint.ToString() + "," + _myPoint.ToString()) : (_myPoint.ToString() + "," + _OpponentPoint.ToString())));
			}
			PlayerPrefs.SetString("TOURNAMENT_BATTLE_RESULT_" + _type.ToString() + "_" + _battleNum.ToString() + "_" + _battleNo.ToString(), (_playerResult && ConvertPtNoToBenchNo(GetTournamentPtNo(_type), _battleNum, _type) == 1) ? (_OpponentPoint.ToString() + "," + _myPoint.ToString()) : (_myPoint.ToString() + "," + _OpponentPoint.ToString()));
		}
		public static void TournamentRetire(int _battleNum = -1, int _battleNo = -1, TournamentType _type = TournamentType.MAX, bool _playerResult = true, bool _save = true)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			if (_battleNum == -1)
			{
				_battleNum = GetTournamentBattleNum(_type);
			}
			if (_battleNo == -1)
			{
				_battleNo = ConvertPtNoToBattleNo(GetTournamentPtNo(_type), _battleNum, _type);
			}
			PlayerPrefs.SetString("TOURNAMENT_BATTLE_RESULT_" + _type.ToString() + "_" + _battleNum.ToString() + "_" + _battleNo.ToString(), (_playerResult && ConvertPtNoToBenchNo(GetTournamentPtNo(_type), _battleNum, _type) == 1) ? "-1,-2" : "-2,-1");
		}
		public static int GetTournamentResultPoint(int _playerNo, int _battleNum = -1, int _battleNo = -1, TournamentType _type = TournamentType.MAX)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			if (_battleNum == -1)
			{
				_battleNum = GetTournamentBattleNum(_type);
			}
			if (_battleNo == -1)
			{
				_battleNo = ConvertPtNoToBattleNo(GetTournamentPtNo(_type), _battleNum, _type);
			}
			string[] array = new string[2]
			{
				PlayerPrefs.GetString("TOURNAMENT_BATTLE_RESULT_" + _type.ToString() + "_" + _battleNum.ToString() + "_" + _battleNo.ToString(), " "),
				null
			};
			if (array[0] == " ")
			{
				return -1;
			}
			array = array[0].Split(',');
			return int.Parse(array[_playerNo]);
		}
		public static int GetTournamentBattleResult(int _battleNum = -1, int _battleNo = -1, TournamentType _type = TournamentType.MAX)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			if (_battleNum == -1)
			{
				_battleNum = GetTournamentBattleNum(_type);
			}
			if (_battleNo == -1)
			{
				_battleNo = ConvertPtNoToBattleNo(GetTournamentPtNo(_type), _battleNum, _type);
			}
			string[] array = new string[2]
			{
				PlayerPrefs.GetString("TOURNAMENT_BATTLE_RESULT_" + _type.ToString() + "_" + _battleNum.ToString() + "_" + _battleNo.ToString(), " "),
				null
			};
			if (array[0] == " ")
			{
				return -1;
			}
			array = array[0].Split(',');
			if (array[0] == array[1])
			{
				return 2;
			}
			if (int.Parse(array[0]) > int.Parse(array[1]))
			{
				return 0;
			}
			return 1;
		}
		public static void ResetTournamentBattleResult(int _battleNum, TournamentType _type = TournamentType.MAX, bool _save = true)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			for (int i = 0; i < 16; i++)
			{
				PlayerPrefs.SetString("TOURNAMENT_BATTLE_RESULT_" + _type.ToString() + "_" + _battleNum.ToString() + "_" + i.ToString(), " ");
			}
		}
		public static void ResetTournamentBattleResult(TournamentType _type = TournamentType.MAX, bool _save = true)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			for (int i = 0; i < 4; i++)
			{
				ResetTournamentBattleResult(i, _type, _save: false);
			}
		}
		public static void SetLoadTournamentOpponent(bool _load, TournamentType _type = TournamentType.MAX, bool _save = true)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			PlayerPrefs.SetInt("TOURNAMENT_LOAD_OPPONENT_" + _type.ToString(), _load ? 1 : 0);
		}
		public static bool IsLoadTournamentOpponent(TournamentType _type = TournamentType.MAX)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			return PlayerPrefs.GetInt("TOURNAMENT_LOAD_OPPONENT_" + _type.ToString(), 0) == 1;
		}
		public static void SetTournamentPtNo(int _no, TournamentType _type = TournamentType.MAX, bool _save = true)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			PlayerPrefs.SetInt("TOURNAMENT_PT_NO_" + _type.ToString(), _no);
		}
		public static int GetTournamentPtNo(TournamentType _type = TournamentType.MAX)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			return PlayerPrefs.GetInt("TOURNAMENT_PT_NO_" + _type.ToString(), 0);
		}
		public static void SetTournamentBenchNo(int _no)
		{
			tournamentBenchNo = _no;
		}
		public static int GetTournamentBenchNo()
		{
			return tournamentBenchNo;
		}
		public static void SetTournamentBattled(bool _battled, int _battleNum = -1, int _battleNo = -1, TournamentType _type = TournamentType.MAX, bool _save = true)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			if (_battleNum == -1)
			{
				_battleNum = GetTournamentBattleNum(_type);
			}
			if (_battleNo == -1)
			{
				_battleNo = ConvertPtNoToBattleNo(GetTournamentPtNo(_type), _battleNum, _type);
			}
			PlayerPrefs.SetInt("TOURNAMENT_BATTLED_" + _type.ToString() + "_" + _battleNum.ToString() + "_" + _battleNo.ToString(), _battled ? 1 : 0);
		}
		public static bool IsTournamentBattled(int _battleNum = -1, int _battleNo = -1, TournamentType _type = TournamentType.MAX)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			if (_battleNum == -1)
			{
				_battleNum = GetTournamentBattleNum(_type);
			}
			if (_battleNo == -1)
			{
				_battleNo = ConvertPtNoToBattleNo(GetTournamentPtNo(_type), _battleNum, _type);
			}
			return PlayerPrefs.GetInt("TOURNAMENT_BATTLED_" + _type.ToString() + "_" + _battleNum.ToString() + "_" + _battleNo.ToString(), 0) == 1;
		}
		public static void SetTournamentLose(bool _lose, bool _save = true)
		{
			PlayerPrefs.SetInt("TOURNAMENT_LOSE", _lose ? 1 : 0);
		}
		public static bool IsTournamentLose()
		{
			return PlayerPrefs.GetInt("TOURNAMENT_LOSE", 0) == 1;
		}
		public static void ResetTournamentBattled(int _battleNum = -1, TournamentType _type = TournamentType.MAX, bool _save = true)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			if (_battleNum == -1)
			{
				_battleNum = GetTournamentBattleNum(_type);
			}
			for (int i = 0; i < 8 / (_battleNum + 1); i++)
			{
				SetTournamentBattled(_battled: false, _battleNum, i, _type, _save: false);
			}
		}
		public static TournamentType CheckNeedTournamentOpponent(DateTime _time)
		{
			if (CheckNextTournamentOpenDay(_time))
			{
				if (IsTournamentPt() && !IsTournamentLose() && !CalcManager.CheckRange(GetTournamentBattleResult(3, 0), 0, 1))
				{
					return TournamentType.MAX;
				}
				if (IsTournamentPt() && CalcManager.CheckRange(GetTournamentBattleResult(3, 0), 0, 1) && !IsTournamentBattled(3, 0))
				{
					return TournamentType.MAX;
				}
				if (CheckStartTournamentOpenDay(_time))
				{
					if (IsTournamentPt() && !IsTournamentLose())
					{
						return GetNowTournamentType() + 1;
					}
					UnityEngine.Debug.Log("新しい大会へ進める");
					return TournamentType.PREFECTURAL_COMPE;
				}
				if (IsTournamentLose())
				{
					UnityEngine.Debug.Log("敗退しているかを判断");
					return TournamentType.MAX;
				}
				return GetNowTournamentType() + 1;
			}
			return TournamentType.MAX;
		}
		public static int ConvertPtNoToBattleNo(int _ptNo = -1, int _battleNum = -1, TournamentType _type = TournamentType.MAX)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			if (_ptNo == -1)
			{
				_ptNo = GetTournamentPtNo(_type);
			}
			if (_battleNum == -1)
			{
				_battleNum = GetTournamentBattleNum(_type);
			}
			return (int)((float)_ptNo / Mathf.Pow(2f, _battleNum + 1));
		}
		public static int ConvertPtNoToBenchNo(int _ptNo = -1, int _battleNum = -1, TournamentType _type = TournamentType.MAX)
		{
			if (_type == TournamentType.MAX)
			{
				_type = GetNowTournamentType();
			}
			if (_ptNo == -1)
			{
				_ptNo = GetTournamentPtNo(_type);
			}
			if (_battleNum == -1)
			{
				_battleNum = GetTournamentBattleNum(_type);
			}
			return (int)((float)_ptNo / Mathf.Pow(2f, _battleNum)) % 2;
		}
		public static void SetTournamentVictoryNum(int _num, TournamentType _type, bool _save = true)
		{
			PlayerPrefs.SetInt("TOURNAMENT_VICTORY_NUM" + _type.ToString(), _num);
		}
		public static void AddTournamentVictoryNum(int _num, TournamentType _type, bool _save = true)
		{
			SetTournamentVictoryNum(GetTournamentVictoryNum(_type) + _num, _type, _save);
		}
		public static int GetTournamentVictoryNum(TournamentType _type)
		{
			return PlayerPrefs.GetInt("TOURNAMENT_VICTORY_NUM" + _type.ToString(), 0);
		}
		public static void SetTournamentContinuityVictoryNum(int _num, TournamentType _type, bool _save = true)
		{
			PlayerPrefs.SetInt("TOURNAMENT_CONTINUITY_VICTORY_NUM" + _type.ToString(), _num);
			if (_num > GetTournamentContinuityVictoryBestNum(_type))
			{
				SetTournamentContinuityVictoryBestNum(_num, _type);
			}
		}
		public static void AddTournamentContinuityVictoryNum(int _num, TournamentType _type, bool _save = true)
		{
			SetTournamentContinuityVictoryNum(GetTournamentContinuityVictoryNum(_type) + _num, _type, _save);
		}
		public static int GetTournamentContinuityVictoryNum(TournamentType _type)
		{
			return PlayerPrefs.GetInt("TOURNAMENT_CONTINUITY_VICTORY_NUM" + _type.ToString(), 0);
		}
		public static void ResetTournamentContinuityVictoryNum(TournamentType _type)
		{
			PlayerPrefs.SetInt("TOURNAMENT_CONTINUITY_VICTORY_NUM" + _type.ToString(), 0);
		}
		public static void SetTournamentContinuityVictoryBestNum(int _num, TournamentType _type)
		{
			PlayerPrefs.SetInt("TOURNAMENT_CONTINUITY_VICTORY_NUM_BEST_" + _type.ToString(), _num);
		}
		public static int GetTournamentContinuityVictoryBestNum(TournamentType _type)
		{
			return PlayerPrefs.GetInt("TOURNAMENT_CONTINUITY_VICTORY_NUM_BEST_" + _type.ToString(), 0);
		}
		public static void ResetTournamentData(TournamentType _type = TournamentType.MAX)
		{
			SetLoadTournamentOpponent(_load: false, _type, _save: false);
			SetTournamentBattleNum(0, _type, _save: false);
			ResetTournamentBattleResult(_type, _save: false);
			for (int i = 0; i < 4; i++)
			{
				ResetTournamentBattled(i, _type, _save: false);
			}
			SetNowTournamentType(TournamentType.NATIONAL_COMPE, _save: false);
			SetTournamentLose(_lose: false);
		}
		public static int GetPlayerNum()
		{
			if (IsVsCpu())
			{
				return 1;
			}
			return 2;
		}
		public static int GetCpuNum()
		{
			if (IsVsCpu())
			{
				return 1;
			}
			return 0;
		}
		public static void SetHavePoint(int _point, bool _save = true)
		{
			PlayerPrefs.SetInt("HAVE_POINT", (_point > 9999999) ? 9999999 : _point);
		}
		public static void AddHavePoint(int _point, bool _save = true)
		{
			int num = PlayerPrefs.GetInt("HAVE_POINT", 0) + _point;
			PlayerPrefs.SetInt("HAVE_POINT", (num > 9999999) ? 9999999 : num);
		}
		public static void ReduceHavePoint(int _point, bool _save = true)
		{
			int num = PlayerPrefs.GetInt("HAVE_POINT", 0) - _point;
			PlayerPrefs.SetInt("HAVE_POINT", (num >= 0) ? num : 0);
		}
		public static int GetHavePoint()
		{
			return PlayerPrefs.GetInt("HAVE_POINT", 0);
		}
		public static void SetRecordScore(int _score, bool _save = true)
		{
			PlayerPrefs.SetInt("RECORD_SCORE", _score);
		}
		public static int GetRecordScore()
		{
			return PlayerPrefs.GetInt("RECORD_SCORE", NOT_PLAY_SCORE);
		}
		public static bool UpdateRecordScore(int _score, bool _save = true)
		{
			if (_score < GetRecordScore())
			{
				SetRecordScore(_score, _save);
				return true;
			}
			return false;
		}
		public static void SetSelectCharacter(CharacterType _type, bool _save = true)
		{
			PlayerPrefs.SetInt("SELECT_CHARACTER", (int)_type);
		}
		public static CharacterType GetSelectCharacter()
		{
			return (CharacterType)PlayerPrefs.GetInt("SELECT_CHARACTER", 0);
		}
		public static void SetBatHaveNum(int _no, int _num, bool _save = true, BAT_COLOR_TYPE _color = BAT_COLOR_TYPE.COLOR_0)
		{
			PlayerPrefs.SetInt("IS_CLUB_UNLOCK" + _no.ToString() + "_" + _color.ToString(), Mathf.Min(_num, 72));
		}
		public static void SetUnlockBat(int _no, bool _unlock, bool _save = true, BAT_COLOR_TYPE _color = BAT_COLOR_TYPE.COLOR_0)
		{
			PlayerPrefs.SetInt("IS_CLUB_UNLOCK" + _no.ToString() + "_" + _color.ToString(), _unlock ? 1 : 0);
		}
		public static void AddBatHaveNum(int _no, int _num, bool _save = true, BAT_COLOR_TYPE _color = BAT_COLOR_TYPE.COLOR_0)
		{
			PlayerPrefs.SetInt("IS_CLUB_UNLOCK" + _no.ToString() + "_" + _color.ToString(), Mathf.Min(GetBatHaveNum(_no, _color) + _num, 72));
		}
		public static bool IsUnlockBat(int _no, BAT_COLOR_TYPE _color = BAT_COLOR_TYPE.COLOR_0)
		{
			return GetBatHaveNum(_no, _color) >= 1;
		}
		public static int GetBatHaveNum(int _no, BAT_COLOR_TYPE _color = BAT_COLOR_TYPE.COLOR_0)
		{
			return PlayerPrefs.GetInt("IS_CLUB_UNLOCK" + _no.ToString() + "_" + _color.ToString(), (_no == 0 && _color == BAT_COLOR_TYPE.COLOR_0) ? 40 : ((_no == 1 && _color == BAT_COLOR_TYPE.COLOR_0) ? 32 : 0));
		}
		public static int GetBatHaveNum(int _no, int _color = 0)
		{
			return GetBatHaveNum(_no, (BAT_COLOR_TYPE)_color);
		}
		public static int GetUnlockBatOtherColorNum(int _no)
		{
			int num = 0;
			for (int i = 1; i < 3; i++)
			{
				if (IsUnlockBat(_no, (BAT_COLOR_TYPE)i))
				{
					num++;
				}
			}
			return num;
		}
		public static void SetUniformHaveNum(int _no, int _num, bool _save = true)
		{
			PlayerPrefs.SetInt("IS_UNIFORM_UNLOCK" + _no.ToString(), _num);
		}
		public static void AddUniformHaveNum(int _no, int _num, bool _save = true)
		{
			SetUniformHaveNum(_no, Mathf.Min(GetUniformHaveNum(_no) + _num, 8), _save: false);
		}
		private static bool IsInitOpen(int _no)
		{
			for (int i = 0; i < INIT_OPEN_NO.Length; i++)
			{
				if (INIT_OPEN_NO[i] == _no)
				{
					return true;
				}
			}
			return false;
		}
		public static int GetUniformHaveNum(int _no)
		{
			return PlayerPrefs.GetInt("IS_UNIFORM_UNLOCK" + _no.ToString(), IsInitOpen(_no) ? ((_no >= 2) ? 1 : 2) : 0);
		}
		public static bool IsUnlockUniform(int _no)
		{
			return GetUniformHaveNum(_no) > 0;
		}
		public static int GetNumUnlockUniform()
		{
			int num = 0;
			for (int i = 0; i < SingletonCustom<UniformListManager>.Instance.GetNum(); i++)
			{
				if (GetUniformHaveNum(i) > 0)
				{
					num++;
				}
			}
			return num;
		}
		public static void SetFormationHaveNum(int _no, int _num, bool _save = true)
		{
			PlayerPrefs.SetInt("IS_FORMATION_UNLOCK" + _no.ToString(), _num);
		}
		public static void AddFormationHaveNum(int _no, int _num, bool _save = true)
		{
			SetFormationHaveNum(_no, Mathf.Min(GetFormationHaveNum(_no) + _num, 8), _save: false);
		}
		public static int GetFormationHaveNum(int _no)
		{
			return PlayerPrefs.GetInt("IS_FORMATION_UNLOCK" + _no.ToString(), (_no < 3) ? 8 : 0);
		}
		public static bool IsUnlockFormation(int _no)
		{
			return GetFormationHaveNum(_no) > 0;
		}
		public static int GetNumUnlockFormation()
		{
			int num = 0;
			for (int i = 0; i < SingletonCustom<FormationListManager>.Instance.GetNum(); i++)
			{
				if (GetFormationHaveNum(i) > 0)
				{
					num++;
				}
			}
			return num;
		}
		public static void SetItemHaveNum(int _no, int _num, bool _save = true)
		{
			PlayerPrefs.SetInt("IS_ITEM_UNLOCK" + _no.ToString(), Mathf.Min(_num, 99));
		}
		public static void SetUnlockItem(int _no, bool _unlock, bool _save = true)
		{
			PlayerPrefs.SetInt("IS_ITEM_UNLOCK" + _no.ToString(), _unlock ? 1 : 0);
		}
		public static void AddItemHaveNum(int _no, int _num, bool _save = true)
		{
			PlayerPrefs.SetInt("IS_ITEM_UNLOCK" + _no.ToString(), Mathf.Min(GetItemHaveNum(_no) + _num, 99));
		}
		public static bool IsUnlockItem(int _no)
		{
			return GetItemHaveNum(_no) >= 1;
		}
		public static int GetItemHaveNum(int _no)
		{
			return PlayerPrefs.GetInt("IS_ITEM_UNLOCK" + _no.ToString(), (_no == 0) ? 99 : 0);
		}
		public static int GetNumUnlockItem()
		{
			return 0;
		}
		public static void SetPlayersName(int _teamNo, int _playerNo, string _name, bool _save = true)
		{
			PlayerPrefs.SetString("PLAYERS_NAME_" + _teamNo.ToString() + "_" + _playerNo.ToString(), _name);
		}
		public static string GetPlayersName(int _teamNo, int _playerNo)
		{
			return PlayerPrefs.GetString("PLAYERS_NAME_" + _teamNo.ToString() + "_" + _playerNo.ToString(), "名無し");
		}
		public static void SetTeamStatusPoint(int _num, bool _save = true)
		{
			PlayerPrefs.SetInt("PREF_TEAM_STATUS_POINT", _num);
		}
		public static void AddTeamStatusPoint(int _num, bool _save = true)
		{
			SetTeamStatusPoint(Mathf.Min(GetTeamStatusPoint() + _num, 40), _save: false);
		}
		public static int GetTeamStatusPoint()
		{
			return PlayerPrefs.GetInt("PREF_TEAM_STATUS_POINT", 12);
		}
		public static void SetTutorial(bool _flg)
		{
			PlayerPrefs.SetInt("MAIN_GAME_TUTORIAL", _flg ? 1 : 0);
		}
		public static bool GetTutorial()
		{
			if (PlayerPrefs.GetInt("MAIN_GAME_TUTORIAL", 0) == 0)
			{
				return true;
			}
			return false;
		}
		public static string GetSuspendData()
		{
			return PlayerPrefs.GetString("SINGLE_SUSPEND_DATA", "");
		}
		public static void SetSuspendData(string _suspend_data)
		{
			PlayerPrefs.SetString("SINGLE_SUSPEND_DATA", _suspend_data);
		}
		public static string GetSuspendAreaObj()
		{
			return PlayerPrefs.GetString("SINGLE_SUSPEND_AREA_OBJ", "");
		}
		public static void SetSuspendAreaObj(string _suspend_area_obj)
		{
			PlayerPrefs.SetString("SINGLE_SUSPEND_AREA_OBJ", _suspend_area_obj);
		}
	}
}
