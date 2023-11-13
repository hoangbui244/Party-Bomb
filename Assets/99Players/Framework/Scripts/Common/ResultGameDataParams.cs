using System;
using System.Collections.Generic;
using UnityEngine;
public class ResultGameDataParams {
    [Serializable]
    public struct UserTypeList {
        public UserType[] userType;
    }
    [Serializable]
    public struct Record_Int {
        [Header("記録保持者のリスト")]
        public UserTypeList[] userTypeList;
        [Header("Int型の記録")]
        public int[] intData;
    }
    [Serializable]
    public struct Record_Float {
        [Header("記録保持者のリスト")]
        public UserTypeList[] userTypeList;
        [Header("Float型の記録")]
        public float[] floatData;
    }
    [Serializable]
    public struct Record_String {
        [Header("記録保持者のリスト")]
        public UserTypeList[] userTypeList;
        [Header("String型の記録")]
        public string[] stringData;
    }
    public struct Record_CavalryBattle {
        [Header("記録保持者のリスト")]
        public UserTypeList[] userTypeList;
        [Header("残機の記録")]
        public int[] lifeData;
        [Header("取った帽子の数の記録")]
        public int[] capData;
    }
    [Serializable]
    public struct RankingRecord {
        public bool recordSetFlg;
        public UserTypeList[] userTypeList;
        public int[] rankNo;
        public int[] point;
        public int[] intData;
        public string[] stringData;
        public float[] floatData;
        public int[] cavalry_LifeData;
        public int[] cavalry_CapData;
        public int[] tournamentWinnerData;
        public string[] tournamentMatchData;
    }
    [Serializable]
    public struct ButtonUIData {
        [Header("ボタンUI表示アンカ\u30fc")]
        public Transform buttonAnchor;
        [Header("演出するボタン配列")]
        public GameObject[] arrayAnimButton;
        [Header("通しモ\u30fcドでの最後のゲ\u30fcム時に隠すボタン")]
        public GameObject[] arrayLastGameHideButton;
    }
    [Serializable]
    public struct PlayKingRankingData {
        public List<int> rankNoList;
    }
    [Serializable]
    public struct PlayKingDetailsData {
        public int[] rankNoArray;
        public int[] teamNoArray;
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
        CPU_5,
        CPU_6,
        CPU_7
    }
    public enum ShowResultType {
        Record_None,
        Record_Score,
        Record_DoubleDecimalScore,
        Record_DecimalScore,
        Record_Time,
        Record_SecondTime,
        Record_Score_Tournament,
        Record_Score_Four_Digit,
        Record_CavalryBattle
    }
    public enum ResultType {
        Win,
        Lose,
        Draw
    }
    public enum CharaType {
        Yuto,
        Hina,
        Itsuki,
        Souta,
        Takumi,
        Rin,
        Akira,
        Rui
    }
    public struct IntFloatData {
        public int data1;
        public float data2;
        public int idx;
    }
    private static bool IsResult = false;
    private static bool isDebugStart = false;
    private static SceneManager.SceneType nowSceneType;
    private static SceneManager.SceneType lastPlayType = SceneManager.SceneType.BLOW_AWAY_TANK;
    private static RankingRecord rankPlayerRecords_Group1;
    private static RankingRecord rankPlayerRecords_Group2;
    private static int[] pointData;
    private static int[] winOrLoseGetPointData = new int[4];
    private static int[] record_WinOrLoseResul_Int = new int[2];
    private static float[] record_WinOrLoseResul_Float = new float[2];
    public static int singleShowCharaNo;
    public static PlayKingRankingData[] rankingDatas = new PlayKingRankingData[6];
    public static PlayKingDetailsData[] playKingDetailsDatas = new PlayKingDetailsData[10];
    public static bool isAddFloatSort = false;
    public static void SetResultMode(bool _isResult) {
        IsResult = _isResult;
    }
    public static bool GetResultMode() {
        return IsResult;
    }
    public static void SetNowSceneType(SceneManager.SceneType _sceneType) {
        nowSceneType = _sceneType;
        if (nowSceneType == SceneManager.SceneType.TITLE || nowSceneType == SceneManager.SceneType.MAIN) {
            InitPlayKingRankData();
            UnityEngine.Debug.Log("<color=cyan>リザルト順位デ\u30fcタ 初期化</color>");
        } else if (!isDebugStart) {
            isDebugStart = true;
            InitPlayKingRankData();
            UnityEngine.Debug.Log("<color=cyan>リザルト順位デ\u30fcタ 初期化</color>");
        }
    }
    public static SceneManager.SceneType GetNowSceneType() {
        return nowSceneType;
    }
    public static GS_Define.GameType GetNowSceneGameType() {
        return (GS_Define.GameType)(nowSceneType - 2);
    }
    public static bool IsLastPlayType() {
        return nowSceneType == SingletonCustom<GameSettingManager>.Instance.GetLastGameType();
    }
    public static void SetPoint() {
        GS_Define.GameType lastSelectGameType = SingletonCustom<GameSettingManager>.Instance.LastSelectGameType;
        SetPoint_Ranking();
    }
    private static void SetPoint_WinOrLose(int _winnerPoint, int _loserPoint, int _drawPoint) {
        pointData = new int[3];
        pointData[0] = _winnerPoint;
        pointData[1] = _loserPoint;
        pointData[2] = _drawPoint;
        UnityEngine.Debug.Log("ポイントを設定");
    }
    private static void SetPoint_Ranking() {
        if (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup) {
            pointData = new int[6];
            pointData[0] = 100;
            pointData[1] = 80;
            pointData[2] = 70;
            pointData[3] = 60;
            pointData[4] = 50;
            pointData[5] = 30;
        } else {
            pointData = new int[4];
            pointData[0] = 100;
            pointData[1] = 80;
            pointData[2] = 70;
            pointData[3] = 60;
        }
        UnityEngine.Debug.Log("ポイントを設定");
    }
    private static void SetPoint_Ranking_8Numbers(int _1stPoint, int _2ndPoint, int _3rdPoint, int _4thPoint, int _5thPoint, int _6thPoint, int _7thPoint, int _8thPoint) {
        pointData = new int[8];
        pointData[0] = _1stPoint;
        pointData[1] = _2ndPoint;
        pointData[2] = _3rdPoint;
        pointData[3] = _4thPoint;
        pointData[4] = _5thPoint;
        pointData[5] = _6thPoint;
        pointData[6] = _7thPoint;
        pointData[7] = _8thPoint;
        UnityEngine.Debug.Log("ポイントを設定");
    }
    public static int GetTeamTotalPoint(int _teamNo) {
        int num = 0;
        GS_Define.GameType lastSelectGameType = SingletonCustom<GameSettingManager>.Instance.LastSelectGameType;
        if (rankPlayerRecords_Group1.point != null) {
            num += rankPlayerRecords_Group1.point[_teamNo];
        }
        if (rankPlayerRecords_Group2.point != null) {
            for (int i = 0; i < rankPlayerRecords_Group2.point.Length; i++) {
                for (int j = 0; j < rankPlayerRecords_Group2.userTypeList[i].userType.Length; j++) {
                    if (IsUserNoBelongTeam(_teamNo, (int)rankPlayerRecords_Group2.userTypeList[i].userType[j])) {
                        num += rankPlayerRecords_Group2.point[i];
                    }
                }
            }
        }
        return num;
    }
    public static void SetRecord_Int(int[] _intData, int[] _userNo, bool _isGroup1Record = true, bool _isAscendingOrder = false) {
        if (_intData.Length != _userNo.Length) {
            UnityEngine.Debug.LogError("<color=yellow>記録とユ\u30fcザ\u30fc番号の配列要素数が違います！！</color>");
            return;
        }
        if (_isGroup1Record && rankPlayerRecords_Group1.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に１組目の記録は設定されています！</color>");
            return;
        }
        if (!_isGroup1Record && rankPlayerRecords_Group2.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に２組目の記録は設定されています！</color>");
            return;
        }
        Record_Int intData = default(Record_Int);
        intData.intData = _intData;
        UserTypeList[] array = new UserTypeList[_intData.Length];
        for (int i = 0; i < array.Length; i++) {
            array[i].userType = new UserType[1];
            array[i].userType[0] = (UserType)_userNo[i];
        }
        intData.userTypeList = array;
        if (_isGroup1Record) {
            rankPlayerRecords_Group1 = GetRankingSortData(intData, default(Record_String), default(Record_Float), default(Record_CavalryBattle), _isAscendingOrder);
            rankPlayerRecords_Group1.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: true);
        } else {
            rankPlayerRecords_Group2 = GetRankingSortData(intData, default(Record_String), default(Record_Float), default(Record_CavalryBattle), _isAscendingOrder);
            rankPlayerRecords_Group2.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: false);
        }
    }
    public static void SetRecord_Int_Float(int[] _intData, float[] _floatData, int[] _userNo, bool _isGroup1Record = true, bool _isAscendingOrder = false) {
        if (_intData.Length != _userNo.Length) {
            UnityEngine.Debug.LogError("<color=yellow>記録とユ\u30fcザ\u30fc番号の配列要素数が違います！！</color>");
            return;
        }
        if (_isGroup1Record && rankPlayerRecords_Group1.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に１組目の記録は設定されています！</color>");
            return;
        }
        if (!_isGroup1Record && rankPlayerRecords_Group2.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に２組目の記録は設定されています！</color>");
            return;
        }
        Record_Int intData = default(Record_Int);
        intData.intData = _intData;
        Record_Float floatData = default(Record_Float);
        floatData.floatData = _floatData;
        UserTypeList[] array = new UserTypeList[_intData.Length];
        for (int i = 0; i < array.Length; i++) {
            array[i].userType = new UserType[1];
            array[i].userType[0] = (UserType)_userNo[i];
        }
        intData.userTypeList = array;
        isAddFloatSort = true;
        if (_isGroup1Record) {
            rankPlayerRecords_Group1 = GetRankingSortData(intData, default(Record_String), floatData, default(Record_CavalryBattle), _isAscendingOrder);
            rankPlayerRecords_Group1.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: true);
        } else {
            rankPlayerRecords_Group2 = GetRankingSortData(intData, default(Record_String), default(Record_Float), default(Record_CavalryBattle), _isAscendingOrder);
            rankPlayerRecords_Group2.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: false);
        }
    }
    public static void SetRecord_Int(int[] _intData, int[][] _userNo, bool _isGroup1Record = true, bool _isAscendingOrder = false) {
        if (_intData.Length != _userNo.Length) {
            UnityEngine.Debug.LogError("<color=yellow>記録とユ\u30fcザ\u30fc番号の配列要素数が違います！！</color>");
            return;
        }
        if (_isGroup1Record && rankPlayerRecords_Group1.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に１組目の記録は設定されています！</color>");
            return;
        }
        if (!_isGroup1Record && rankPlayerRecords_Group2.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に２組目の記録は設定されています！</color>");
            return;
        }
        Record_Int intData = default(Record_Int);
        intData.intData = _intData;
        UserTypeList[] array = new UserTypeList[_intData.Length];
        for (int i = 0; i < array.Length; i++) {
            array[i].userType = new UserType[_userNo[i].Length];
            for (int j = 0; j < _userNo[i].Length; j++) {
                array[i].userType[j] = (UserType)_userNo[i][j];
            }
        }
        intData.userTypeList = array;
        if (_isGroup1Record) {
            rankPlayerRecords_Group1 = GetRankingSortData(intData, default(Record_String), default(Record_Float), default(Record_CavalryBattle), _isAscendingOrder);
            rankPlayerRecords_Group1.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: true);
        } else {
            rankPlayerRecords_Group2 = GetRankingSortData(intData, default(Record_String), default(Record_Float), default(Record_CavalryBattle), _isAscendingOrder);
            rankPlayerRecords_Group2.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: false);
        }
    }
    public static void SetRecord_Int_Ranking8Numbers(int[] _intData, int[] _userNo, bool _isAscendingOrder = false) {
        if (_intData.Length != _userNo.Length) {
            UnityEngine.Debug.LogError("<color=yellow>記録とユ\u30fcザ\u30fc番号の配列要素数が違います！！</color>");
            return;
        }
        if (rankPlayerRecords_Group1.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に１組目の記録は設定されています！</color>");
            return;
        }
        if (rankPlayerRecords_Group2.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に２組目の記録は設定されています！</color>");
            return;
        }
        Record_Int intData = default(Record_Int);
        intData.intData = _intData;
        UserTypeList[] array = new UserTypeList[_intData.Length];
        for (int i = 0; i < array.Length; i++) {
            array[i].userType = new UserType[1];
            array[i].userType[0] = (UserType)_userNo[i];
        }
        intData.userTypeList = array;
        RankingRecord rankingSortData = GetRankingSortData(intData, default(Record_String), default(Record_Float), default(Record_CavalryBattle), _isAscendingOrder);
        int num = 0;
        int num2 = 0;
        rankPlayerRecords_Group1.intData = new int[4];
        rankPlayerRecords_Group1.rankNo = new int[4];
        rankPlayerRecords_Group1.userTypeList = new UserTypeList[4];
        rankPlayerRecords_Group2.intData = new int[4];
        rankPlayerRecords_Group2.rankNo = new int[4];
        rankPlayerRecords_Group2.userTypeList = new UserTypeList[4];
        for (int j = 0; j < rankingSortData.intData.Length; j++) {
            if (j < rankingSortData.intData.Length / 2) {
                rankPlayerRecords_Group2.intData[num2] = rankingSortData.intData[j];
                rankPlayerRecords_Group2.rankNo[num2] = rankingSortData.rankNo[j];
                rankPlayerRecords_Group2.userTypeList[num2] = rankingSortData.userTypeList[j];
                num2++;
            } else {
                rankPlayerRecords_Group1.intData[num] = rankingSortData.intData[j];
                rankPlayerRecords_Group1.rankNo[num] = rankingSortData.rankNo[j];
                rankPlayerRecords_Group1.userTypeList[num] = rankingSortData.userTypeList[j];
                num++;
            }
        }
        rankPlayerRecords_Group1.recordSetFlg = true;
        rankPlayerRecords_Group2.recordSetFlg = true;
        DebugLog_RecordData(_isGroup1: true);
        DebugLog_RecordData(_isGroup1: false);
    }
    public static void SetRecord_Float(float[] _floatData, int[] _userNo, bool _isGroup1Record = true, bool _isAscendingOrder = false) {
        if (_floatData.Length != _userNo.Length) {
            UnityEngine.Debug.LogError("<color=yellow>記録とユ\u30fcザ\u30fc番号の配列要素数が違います！！</color>" + Environment.NewLine + (_isGroup1Record ? "<color=yellow>1組目のデ\u30fcタが設定できません</color>" : "<color=yellow>2組目のデ\u30fcタが設定できません</color>"));
            return;
        }
        if (_isGroup1Record && rankPlayerRecords_Group1.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に１組目の記録は設定されています！</color>");
            return;
        }
        if (!_isGroup1Record && rankPlayerRecords_Group2.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に２組目の記録は設定されています！</color>");
            return;
        }
        UserTypeList[] array = new UserTypeList[_floatData.Length];
        Record_Float record_Float = default(Record_Float);
        float[] array2 = new float[_floatData.Length];
        switch (GetNowSceneType()) {
            case SceneManager.SceneType.MINISCAPE_RACE:
                for (int j = 0; j < _floatData.Length; j++) {
                    if (_floatData[j] == -1f) {
                        array2[j] = 600f;
                    } else {
                        array2[j] = _floatData[j];
                    }
                }
                break;
            case SceneManager.SceneType.CLIMB_WALL:
                for (int k = 0; k < _floatData.Length; k++) {
                    if (_floatData[k] != -1f) {
                        array2[k] = _floatData[k];
                    }
                }
                break;
            default:
                for (int i = 0; i < _floatData.Length; i++) {
                    if (_floatData[i] > CalcManager.ConvertRecordStringToTime("9:59.99")) {
                        array2[i] = CalcManager.ConvertRecordStringToTime("9:59.99");
                    } else {
                        array2[i] = _floatData[i];
                    }
                }
                break;
        }
        record_Float.floatData = array2;
        for (int l = 0; l < record_Float.floatData.Length; l++) {
            record_Float.floatData[l] = CalcManager.ConvertDecimalSecond(record_Float.floatData[l]);
        }
        for (int m = 0; m < record_Float.floatData.Length; m++) {
            record_Float.floatData[m] = CalcManager.ConvertDecimalSecond(record_Float.floatData[m]);
        }
        for (int n = 0; n < array.Length; n++) {
            array[n].userType = new UserType[1];
            array[n].userType[0] = (UserType)_userNo[n];
        }
        record_Float.userTypeList = array;
        if (_isGroup1Record) {
            rankPlayerRecords_Group1 = GetRankingSortData(default(Record_Int), default(Record_String), record_Float, default(Record_CavalryBattle), _isAscendingOrder, _isNotRecordBestRank: true);
            rankPlayerRecords_Group1.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: true);
        } else {
            rankPlayerRecords_Group2 = GetRankingSortData(default(Record_Int), default(Record_String), record_Float, default(Record_CavalryBattle), _isAscendingOrder, _isNotRecordBestRank: true);
            rankPlayerRecords_Group2.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: false);
        }
    }
    public static void SetRecord_Float(float[] _floatData, int[][] _userNo, bool _isGroup1Record = true, bool _isAscendingOrder = false) {
        if (_floatData.Length != _userNo.Length) {
            UnityEngine.Debug.LogError("<color=yellow>記録とユ\u30fcザ\u30fc番号の配列要素数が違います！！</color>");
            return;
        }
        if (_isGroup1Record && rankPlayerRecords_Group1.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に１組目の記録は設定されています！</color>");
            return;
        }
        if (!_isGroup1Record && rankPlayerRecords_Group2.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に２組目の記録は設定されています！</color>");
            return;
        }
        UserTypeList[] array = new UserTypeList[_floatData.Length];
        Record_Float record_Float = default(Record_Float);
        float[] array2 = new float[_floatData.Length];
        for (int i = 0; i < _floatData.Length; i++) {
            if (_floatData[i] > CalcManager.ConvertRecordStringToTime("9:59.99")) {
                array2[i] = CalcManager.ConvertRecordStringToTime("9:59.99");
            } else {
                array2[i] = _floatData[i];
            }
        }
        record_Float.floatData = array2;
        for (int j = 0; j < record_Float.floatData.Length; j++) {
            record_Float.floatData[j] = CalcManager.ConvertDecimalFirst(record_Float.floatData[j]);
        }
        for (int k = 0; k < record_Float.floatData.Length; k++) {
            record_Float.floatData[k] = CalcManager.ConvertDecimalFirst(record_Float.floatData[k]);
        }
        for (int l = 0; l < array.Length; l++) {
            array[l].userType = new UserType[_userNo[l].Length];
            for (int m = 0; m < _userNo[l].Length; m++) {
                array[l].userType[m] = (UserType)_userNo[l][m];
            }
        }
        record_Float.userTypeList = array;
        if (_isGroup1Record) {
            rankPlayerRecords_Group1 = GetRankingSortData(default(Record_Int), default(Record_String), record_Float, default(Record_CavalryBattle), _isAscendingOrder);
            rankPlayerRecords_Group1.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: true);
        } else {
            rankPlayerRecords_Group2 = GetRankingSortData(default(Record_Int), default(Record_String), record_Float, default(Record_CavalryBattle), _isAscendingOrder);
            rankPlayerRecords_Group2.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: false);
        }
    }
    public static void SetRecord_Float_Ranking8Numbers(float[] _floatData, int[] _userNo, bool _isAscendingOrder = false) {
        if (_floatData.Length != _userNo.Length) {
            UnityEngine.Debug.LogError("<color=yellow>記録とユ\u30fcザ\u30fc番号の配列要素数が違います！！</color>");
            return;
        }
        if (rankPlayerRecords_Group1.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に１組目の記録は設定されています！</color>");
            return;
        }
        if (rankPlayerRecords_Group2.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に２組目の記録は設定されています！</color>");
            return;
        }
        Record_Float floatData = default(Record_Float);
        UserTypeList[] array = new UserTypeList[_floatData.Length];
        float[] array2 = new float[_floatData.Length];
        for (int i = 0; i < _floatData.Length; i++) {
            if (_floatData[i] > CalcManager.ConvertRecordStringToTime("9:59.99")) {
                array2[i] = CalcManager.ConvertRecordStringToTime("9:59.99");
            } else {
                array2[i] = _floatData[i];
            }
        }
        floatData.floatData = array2;
        for (int j = 0; j < array.Length; j++) {
            array[j].userType = new UserType[1];
            array[j].userType[0] = (UserType)_userNo[j];
        }
        floatData.userTypeList = array;
        RankingRecord rankingSortData = GetRankingSortData(default(Record_Int), default(Record_String), floatData, default(Record_CavalryBattle), _isAscendingOrder);
        int num = 0;
        int num2 = 0;
        rankPlayerRecords_Group1.floatData = new float[4];
        rankPlayerRecords_Group1.rankNo = new int[4];
        rankPlayerRecords_Group1.userTypeList = new UserTypeList[4];
        rankPlayerRecords_Group1.point = new int[4];
        rankPlayerRecords_Group2.floatData = new float[4];
        rankPlayerRecords_Group2.rankNo = new int[4];
        rankPlayerRecords_Group2.userTypeList = new UserTypeList[4];
        rankPlayerRecords_Group2.point = new int[4];
        for (int k = 0; k < rankingSortData.floatData.Length; k++) {
            if (k < rankingSortData.floatData.Length / 2) {
                rankPlayerRecords_Group2.floatData[num2] = rankingSortData.floatData[k];
                rankPlayerRecords_Group2.rankNo[num2] = rankingSortData.rankNo[k];
                rankPlayerRecords_Group2.userTypeList[num2] = rankingSortData.userTypeList[k];
                rankPlayerRecords_Group2.point[num2] = rankingSortData.point[k];
                num2++;
            } else {
                rankPlayerRecords_Group1.floatData[num] = rankingSortData.floatData[k];
                rankPlayerRecords_Group1.rankNo[num] = rankingSortData.rankNo[k];
                rankPlayerRecords_Group1.userTypeList[num] = rankingSortData.userTypeList[k];
                rankPlayerRecords_Group1.point[num] = rankingSortData.point[k];
                num++;
            }
        }
        rankPlayerRecords_Group1.recordSetFlg = true;
        rankPlayerRecords_Group2.recordSetFlg = true;
        DebugLog_RecordData(_isGroup1: true);
        DebugLog_RecordData(_isGroup1: false);
    }
    public static void SetRecord_CavalryBattle(int[] _lifeData, int[] _capData, int[] _userNo, bool _isGroup1Record = true, bool _isAscendingOrder = false) {
        if (_lifeData.Length != _userNo.Length) {
            UnityEngine.Debug.LogError("<color=yellow>記録とユ\u30fcザ\u30fc番号の配列要素数が違います！！</color>");
            return;
        }
        if (_isGroup1Record && rankPlayerRecords_Group1.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に１組目の記録は設定されています！</color>");
            return;
        }
        if (!_isGroup1Record && rankPlayerRecords_Group2.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に２組目の記録は設定されています！</color>");
            return;
        }
        Record_CavalryBattle cavalryData = default(Record_CavalryBattle);
        cavalryData.lifeData = _lifeData;
        cavalryData.capData = _capData;
        UserTypeList[] array = new UserTypeList[_lifeData.Length];
        for (int i = 0; i < array.Length; i++) {
            array[i].userType = new UserType[1];
            array[i].userType[0] = (UserType)_userNo[i];
        }
        cavalryData.userTypeList = array;
        if (_isGroup1Record) {
            rankPlayerRecords_Group1 = GetRankingSortData(default(Record_Int), default(Record_String), default(Record_Float), cavalryData, _isAscendingOrder);
            rankPlayerRecords_Group1.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: true);
        } else {
            rankPlayerRecords_Group2 = GetRankingSortData(default(Record_Int), default(Record_String), default(Record_Float), cavalryData, _isAscendingOrder);
            rankPlayerRecords_Group2.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: false);
        }
    }
    public static void SetRecord_Int_Tournament(int[] _intData, int[] _userNo, int[] _tournamentWinnerNoArray, string[] _tournamentMatch, bool _isGroup1Record = true, bool _isAscendingOrder = false) {
        if (_intData.Length != _userNo.Length) {
            UnityEngine.Debug.LogError("<color=yellow>記録とユ\u30fcザ\u30fc番号の配列要素数が違います！！</color>");
            return;
        }
        if (_isGroup1Record && rankPlayerRecords_Group1.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に１組目の記録は設定されています！</color>");
            return;
        }
        if (!_isGroup1Record && rankPlayerRecords_Group2.recordSetFlg) {
            UnityEngine.Debug.LogError("<color=yellow>既に２組目の記録は設定されています！</color>");
            return;
        }
        Record_Int intData = default(Record_Int);
        intData.intData = _intData;
        UserTypeList[] array = new UserTypeList[_intData.Length];
        for (int i = 0; i < array.Length; i++) {
            array[i].userType = new UserType[1];
            array[i].userType[0] = (UserType)_userNo[i];
        }
        intData.userTypeList = array;
        if (_isGroup1Record) {
            rankPlayerRecords_Group1 = GetRankingSortData(intData, default(Record_String), default(Record_Float), default(Record_CavalryBattle), _isAscendingOrder);
            rankPlayerRecords_Group1.tournamentWinnerData = _tournamentWinnerNoArray;
            rankPlayerRecords_Group1.tournamentMatchData = _tournamentMatch;
            rankPlayerRecords_Group1.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: true);
        } else {
            rankPlayerRecords_Group2 = GetRankingSortData(intData, default(Record_String), default(Record_Float), default(Record_CavalryBattle), _isAscendingOrder);
            rankPlayerRecords_Group2.tournamentWinnerData = _tournamentWinnerNoArray;
            rankPlayerRecords_Group2.tournamentMatchData = _tournamentMatch;
            rankPlayerRecords_Group2.recordSetFlg = true;
            DebugLog_RecordData(_isGroup1: false);
        }
    }
    public static void SetRecord_WinOrLose(int _record, int _teamNo = 0) {
        record_WinOrLoseResul_Int[_teamNo] = _record;
    }
    public static void SetRecord_WinOrLose(float _record, int _teamNo = 0) {
        record_WinOrLoseResul_Float[_teamNo] = _record;
    }
    private static RankingRecord GetRankingSortData(Record_Int _intData, Record_String _stringData, Record_Float _floatData, Record_CavalryBattle _cavalryData, bool _isAscendingOrder, bool _isNotRecordBestRank = false) {
        RankingRecord rankingRecord = default(RankingRecord);
        if (_intData.intData != null) {
            rankingRecord.intData = _intData.intData;
            if (isAddFloatSort) {
                rankingRecord.floatData = _floatData.floatData;
            }
            rankingRecord.userTypeList = _intData.userTypeList;
            rankingRecord.rankNo = new int[_intData.intData.Length];
            rankingRecord.point = new int[_intData.intData.Length];
        } else if (_stringData.stringData != null) {
            rankingRecord.stringData = _stringData.stringData;
            rankingRecord.userTypeList = _stringData.userTypeList;
            rankingRecord.rankNo = new int[_stringData.stringData.Length];
            rankingRecord.point = new int[_stringData.stringData.Length];
        } else if (_floatData.floatData != null) {
            rankingRecord.floatData = _floatData.floatData;
            rankingRecord.userTypeList = _floatData.userTypeList;
            rankingRecord.rankNo = new int[_floatData.floatData.Length];
            rankingRecord.point = new int[_floatData.floatData.Length];
        } else {
            if (_cavalryData.lifeData == null) {
                UnityEngine.Debug.LogError("デ\u30fcタが無いので記録のソ\u30fcトができません！！");
                return default(RankingRecord);
            }
            rankingRecord.cavalry_LifeData = _cavalryData.lifeData;
            rankingRecord.cavalry_CapData = _cavalryData.capData;
            rankingRecord.userTypeList = _cavalryData.userTypeList;
            rankingRecord.rankNo = new int[_cavalryData.lifeData.Length];
            rankingRecord.point = new int[_cavalryData.lifeData.Length];
        }
        if (rankingRecord.intData != null) {
            List<int> list = new List<int>(rankingRecord.intData);
            List<UserTypeList> list2 = new List<UserTypeList>(rankingRecord.userTypeList);
            bool[] array = new bool[list2.Count];
            bool[] array2 = new bool[list.Count];
            if (isAddFloatSort) {
                List<float> list3 = new List<float>(rankingRecord.floatData);
                isAddFloatSort = false;
                IntFloatData[] array3 = new IntFloatData[rankingRecord.intData.Length];
                IntFloatData intFloatData = default(IntFloatData);
                for (int i = 0; i < rankingRecord.intData.Length; i++) {
                    intFloatData.data1 = rankingRecord.intData[i];
                    intFloatData.data2 = rankingRecord.floatData[i];
                    intFloatData.idx = i;
                    array3[i] = intFloatData;
                }
                array3.Sort((IntFloatData c) => c.data2, (IntFloatData c) => c.data1);
                for (int j = 0; j < array3.Length; j++) {
                    rankingRecord.intData[j] = array3[j].data1;
                    rankingRecord.floatData[j] = array3[j].data2;
                    UnityEngine.Debug.Log("順位:" + j.ToString() + " idx:" + array3[j].idx.ToString() + " data1:" + array3[j].data1.ToString() + " data2:" + array3[j].data2.ToString());
                }
                for (int k = 0; k < rankingRecord.intData.Length; k++) {
                    for (int l = 0; l < rankingRecord.intData.Length; l++) {
                        if (rankingRecord.intData[k] == list[l] && rankingRecord.floatData[k] == list3[l] && !array[l]) {
                            rankingRecord.userTypeList[k] = list2[l];
                            array[l] = true;
                            break;
                        }
                    }
                }
                int num = 0;
                for (int m = 0; m < rankingRecord.intData.Length; m++) {
                    for (int n = 0; n < rankingRecord.intData.Length; n++) {
                        if (rankingRecord.intData[m] == rankingRecord.intData[n] && rankingRecord.floatData[m] == rankingRecord.floatData[n] && !array2[n]) {
                            rankingRecord.rankNo[n] = num;
                            array2[n] = true;
                        }
                    }
                    num = 0;
                    for (int num2 = 0; num2 < array2.Length; num2++) {
                        if (array2[num2]) {
                            num++;
                        }
                    }
                }
            } else {
                CalcManager.QuickSort(rankingRecord.intData, _isAscendingOrder);
                for (int num3 = 0; num3 < rankingRecord.intData.Length; num3++) {
                    for (int num4 = 0; num4 < rankingRecord.intData.Length; num4++) {
                        if (rankingRecord.intData[num3] == list[num4] && !array[num4]) {
                            rankingRecord.userTypeList[num3] = list2[num4];
                            array[num4] = true;
                            break;
                        }
                    }
                }
                int num5 = 0;
                for (int num6 = 0; num6 < rankingRecord.intData.Length; num6++) {
                    for (int num7 = 0; num7 < rankingRecord.intData.Length; num7++) {
                        if (rankingRecord.intData[num6] == rankingRecord.intData[num7] && !array2[num7]) {
                            rankingRecord.rankNo[num7] = num5;
                            array2[num7] = true;
                        }
                    }
                    num5 = 0;
                    for (int num8 = 0; num8 < array2.Length; num8++) {
                        if (array2[num8]) {
                            num5++;
                        }
                    }
                }
            }
            if (pointData != null) {
                UnityEngine.Debug.Log("sortRankRecord.rankNo：" + rankingRecord.rankNo.Length.ToString());
                UnityEngine.Debug.Log("sortRankRecord.point：" + rankingRecord.point.Length.ToString());
                UnityEngine.Debug.Log("pointData：" + pointData.Length.ToString());
                for (int num9 = 0; num9 < rankingRecord.rankNo.Length; num9++) {
                    rankingRecord.point[num9] = pointData[rankingRecord.rankNo[num9]];
                }
                for (int num10 = 0; num10 < rankingRecord.point.Length; num10++) {
                    UnityEngine.Debug.Log("順位ポイント:" + num10.ToString() + ":" + rankingRecord.point[num10].ToString());
                }
            }
        } else if (rankingRecord.stringData != null) {
            List<string> list4 = new List<string>(rankingRecord.stringData);
            List<UserTypeList> list5 = new List<UserTypeList>(rankingRecord.userTypeList);
            bool[] array4 = new bool[list5.Count];
            bool[] array5 = new bool[list4.Count];
            int num11 = 0;
            for (int num12 = 0; num12 < rankingRecord.stringData.Length; num12++) {
                if (rankingRecord.stringData[num12] != "-1") {
                    num11++;
                }
            }
            string[] array6 = new string[num11];
            int num13 = 0;
            for (int num14 = 0; num14 < rankingRecord.stringData.Length; num14++) {
                if (rankingRecord.stringData[num14] != "-1") {
                    array6[num13] = rankingRecord.stringData[num14];
                    num13++;
                }
            }
            CalcManager.QuickSort(array6, _isAscendingOrder);
            for (int num15 = 0; num15 < rankingRecord.stringData.Length; num15++) {
                if (num15 < array6.Length) {
                    rankingRecord.stringData[num15] = array6[num15];
                } else {
                    rankingRecord.stringData[num15] = "-1";
                }
            }
            for (int num16 = 0; num16 < rankingRecord.stringData.Length; num16++) {
                for (int num17 = 0; num17 < rankingRecord.stringData.Length; num17++) {
                    if (rankingRecord.stringData[num16] == list4[num17] && !array4[num17]) {
                        rankingRecord.userTypeList[num16] = list5[num17];
                        array4[num17] = true;
                        break;
                    }
                }
            }
            int num18 = 0;
            bool flag = false;
            for (int num19 = 0; num19 < rankingRecord.stringData.Length; num19++) {
                for (int num20 = 0; num20 < rankingRecord.stringData.Length; num20++) {
                    if (!(rankingRecord.stringData[num20] == "-1") && rankingRecord.stringData[num19] == rankingRecord.stringData[num20] && !array5[num20]) {
                        rankingRecord.rankNo[num20] = num18;
                        flag = true;
                        array5[num20] = true;
                    }
                }
                if (!flag) {
                    continue;
                }
                num18 = 0;
                for (int num21 = 0; num21 < array5.Length; num21++) {
                    if (array5[num21]) {
                        num18++;
                    }
                }
                flag = false;
            }
            for (int num22 = 0; num22 < array5.Length; num22++) {
                if (!array5[num22]) {
                    rankingRecord.rankNo[num22] = num18;
                    array5[num22] = true;
                }
            }
            if (pointData != null) {
                int num23 = 0;
                bool flag2 = false;
                int num24 = 0;
                int num25 = -1;
                for (int num26 = 0; num26 < rankingRecord.rankNo.Length; num26++) {
                    for (int num27 = 0; num27 < rankingRecord.rankNo.Length; num27++) {
                        if (rankingRecord.rankNo[num26] == rankingRecord.rankNo[num27]) {
                            num23++;
                        }
                    }
                    if (num23 > 1 && !flag2) {
                        flag2 = true;
                        num24 = num26;
                    }
                    num23 = 0;
                }
                if (flag2) {
                    for (int num28 = 0; num28 < rankingRecord.rankNo.Length; num28++) {
                        if (rankingRecord.rankNo[num28] <= num24) {
                            rankingRecord.point[num28] = pointData[rankingRecord.rankNo[num28]];
                            continue;
                        }
                        if (num25 == -1) {
                            num25 = rankingRecord.rankNo[num28];
                        }
                        if (num25 == rankingRecord.rankNo[num28]) {
                            rankingRecord.point[num28] = pointData[num24 + 1];
                        } else {
                            rankingRecord.point[num28] = pointData[num24 + 2];
                        }
                    }
                } else {
                    for (int num29 = 0; num29 < rankingRecord.rankNo.Length; num29++) {
                        rankingRecord.point[num29] = pointData[rankingRecord.rankNo[num29]];
                    }
                }
            }
        } else if (_floatData.floatData != null) {
            List<float> list6 = new List<float>(rankingRecord.floatData);
            List<UserTypeList> list7 = new List<UserTypeList>(rankingRecord.userTypeList);
            bool[] array7 = new bool[list7.Count];
            bool[] array8 = new bool[list6.Count];
            int num30 = 0;
            if (_isNotRecordBestRank) {
                for (int num31 = 0; num31 < rankingRecord.floatData.Length; num31++) {
                    if (rankingRecord.floatData[num31] != -1f) {
                        num30++;
                    }
                }
                float[] array9 = new float[num30];
                int num32 = 0;
                for (int num33 = 0; num33 < rankingRecord.floatData.Length; num33++) {
                    if (rankingRecord.floatData[num33] != -1f) {
                        array9[num32] = rankingRecord.floatData[num33];
                        num32++;
                    }
                }
                CalcManager.QuickSort(array9, _isAscendingOrder);
                int num34 = 0;
                for (int num35 = 0; num35 < rankingRecord.floatData.Length; num35++) {
                    if (num35 < rankingRecord.floatData.Length - array9.Length) {
                        rankingRecord.floatData[num35] = -1f;
                        continue;
                    }
                    rankingRecord.floatData[num35] = array9[num34];
                    num34++;
                }
            } else {
                for (int num36 = 0; num36 < rankingRecord.floatData.Length; num36++) {
                    if (rankingRecord.floatData[num36] != -1f) {
                        num30++;
                    }
                }
                float[] array10 = new float[num30];
                int num37 = 0;
                for (int num38 = 0; num38 < rankingRecord.floatData.Length; num38++) {
                    if (rankingRecord.floatData[num38] != -1f) {
                        array10[num37] = rankingRecord.floatData[num38];
                        num37++;
                    }
                }
                CalcManager.QuickSort(array10, _isAscendingOrder);
                for (int num39 = 0; num39 < rankingRecord.floatData.Length; num39++) {
                    if (num39 < array10.Length) {
                        rankingRecord.floatData[num39] = array10[num39];
                    } else {
                        rankingRecord.floatData[num39] = -1f;
                    }
                }
            }
            for (int num40 = 0; num40 < rankingRecord.floatData.Length; num40++) {
                for (int num41 = 0; num41 < rankingRecord.floatData.Length; num41++) {
                    if (rankingRecord.floatData[num40] == list6[num41] && !array7[num41]) {
                        rankingRecord.userTypeList[num40] = list7[num41];
                        array7[num41] = true;
                        break;
                    }
                }
            }
            int num42 = 0;
            bool flag3 = false;
            for (int num43 = 0; num43 < rankingRecord.floatData.Length; num43++) {
                for (int num44 = 0; num44 < rankingRecord.floatData.Length; num44++) {
                    if (rankingRecord.floatData[num44] == -1f) {
                        if (_isNotRecordBestRank && !array8[num44]) {
                            rankingRecord.rankNo[num44] = num42;
                            flag3 = true;
                            array8[num44] = true;
                        }
                    } else if (rankingRecord.floatData[num43] == rankingRecord.floatData[num44] && !array8[num44]) {
                        rankingRecord.rankNo[num44] = num42;
                        flag3 = true;
                        array8[num44] = true;
                    }
                }
                if (!flag3) {
                    continue;
                }
                num42 = 0;
                for (int num45 = 0; num45 < array8.Length; num45++) {
                    if (array8[num45]) {
                        num42++;
                    }
                }
                flag3 = false;
            }
            for (int num46 = 0; num46 < array8.Length; num46++) {
                if (!array8[num46]) {
                    rankingRecord.rankNo[num46] = num42;
                    array8[num46] = true;
                }
            }
            if (pointData != null) {
                int num47 = 0;
                bool flag4 = false;
                int num48 = 0;
                int num49 = -1;
                List<int> list8 = new List<int>();
                int num50 = 0;
                for (int num51 = 0; num51 < rankingRecord.rankNo.Length; num51++) {
                    for (int num52 = 0; num52 < rankingRecord.rankNo.Length; num52++) {
                        if (rankingRecord.rankNo[num51] == rankingRecord.rankNo[num52]) {
                            num47++;
                        }
                    }
                    if (num47 > 1 && !flag4) {
                        flag4 = true;
                        num48 = num51;
                    }
                    num47 = 0;
                }
                if (flag4) {
                    for (int num53 = 0; num53 < pointData.Length; num53++) {
                        list8.Add(pointData[num53]);
                    }
                    for (int num54 = 0; num54 < rankingRecord.rankNo.Length; num54++) {
                        if ((GetNowSceneType() == SceneManager.SceneType.MAX && SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2) || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
                            if (rankingRecord.rankNo[num54] != num50) {
                                UnityEngine.Debug.Log("順位変動");
                                UnityEngine.Debug.Log("除外ポイント：" + list8[0].ToString());
                                list8.RemoveAt(0);
                                UnityEngine.Debug.Log("確認順位 前：" + num50.ToString());
                                num50 = rankingRecord.rankNo[num54];
                                UnityEngine.Debug.Log("確認順位 後：" + num50.ToString());
                            }
                            rankingRecord.point[num54] = list8[0];
                            UnityEngine.Debug.Log(rankingRecord.rankNo[num54].ToString() + "位：ポイント" + list8[0].ToString());
                        } else if (rankingRecord.rankNo[num54] <= num48) {
                            rankingRecord.point[num54] = pointData[rankingRecord.rankNo[num54]];
                        } else {
                            if (num49 == -1) {
                                num49 = rankingRecord.rankNo[num54];
                            }
                            if (num49 == rankingRecord.rankNo[num54]) {
                                rankingRecord.point[num54] = pointData[num48 + 1];
                            } else {
                                rankingRecord.point[num54] = pointData[num48 + 2];
                            }
                        }
                    }
                } else {
                    UnityEngine.Debug.Log("len:" + rankingRecord.rankNo.Length.ToString());
                    UnityEngine.Debug.Log("point:" + rankingRecord.point.Length.ToString());
                    UnityEngine.Debug.Log("pointData:" + pointData.Length.ToString());
                    for (int num55 = 0; num55 < rankingRecord.rankNo.Length; num55++) {
                        rankingRecord.point[num55] = pointData[rankingRecord.rankNo[num55]];
                    }
                }
            }
        } else if (rankingRecord.cavalry_LifeData != null && rankingRecord.cavalry_CapData != null) {
            List<int> list9 = new List<int>(rankingRecord.cavalry_LifeData);
            List<int> list10 = new List<int>(rankingRecord.cavalry_CapData);
            List<UserTypeList> list11 = new List<UserTypeList>(rankingRecord.userTypeList);
            bool[] array11 = new bool[list11.Count];
            bool[] array12 = new bool[list9.Count];
            CalcManager.QuickSort(rankingRecord.cavalry_LifeData, _isAscendingOrder);
            for (int num56 = 0; num56 < rankingRecord.cavalry_LifeData.Length; num56++) {
                for (int num57 = 0; num57 < rankingRecord.cavalry_LifeData.Length; num57++) {
                    if (rankingRecord.cavalry_LifeData[num56] == list9[num57] && !array11[num57]) {
                        rankingRecord.cavalry_CapData[num56] = list10[num57];
                        rankingRecord.userTypeList[num56] = list11[num57];
                        array11[num57] = true;
                        break;
                    }
                }
            }
            for (int num58 = 0; num58 < rankingRecord.cavalry_LifeData.Length; num58++) {
                for (int num59 = 0; num59 < rankingRecord.cavalry_LifeData.Length; num59++) {
                    if (rankingRecord.cavalry_LifeData[num58] == rankingRecord.cavalry_LifeData[num59] && !array12[num59]) {
                        rankingRecord.rankNo[num59] = num58;
                        array12[num59] = true;
                    }
                }
            }
            for (int num60 = 0; num60 < array11.Length; num60++) {
                array11[num60] = false;
            }
            for (int num61 = 0; num61 < rankingRecord.rankNo.Length; num61++) {
                List<int> list12 = new List<int>();
                List<int> list13 = new List<int>();
                List<UserTypeList> list14 = new List<UserTypeList>();
                for (int num62 = 0; num62 < rankingRecord.cavalry_CapData.Length; num62++) {
                    if (num61 == rankingRecord.rankNo[num62]) {
                        list12.Add(rankingRecord.cavalry_CapData[num62]);
                        list14.Add(rankingRecord.userTypeList[num62]);
                    }
                }
                if (list12.Count <= 1) {
                    continue;
                }
                list13 = new List<int>(list12);
                int[] array13 = list12.ToArray();
                CalcManager.QuickSort(array13, _isAscendingOrder: false);
                list12 = new List<int>(array13);
                for (int num63 = 0; num63 < rankingRecord.cavalry_CapData.Length; num63++) {
                    if (num61 != rankingRecord.rankNo[num63]) {
                        continue;
                    }
                    rankingRecord.cavalry_CapData[num63] = list12[0];
                    for (int num64 = 0; num64 < list13.Count; num64++) {
                        if (rankingRecord.cavalry_CapData[num63] == list13[num64] && !array11[num64]) {
                            rankingRecord.userTypeList[num63] = list14[num64];
                            array11[num64] = true;
                            break;
                        }
                    }
                    list12.RemoveAt(0);
                }
            }
            for (int num65 = 0; num65 < array12.Length; num65++) {
                array12[num65] = false;
            }
            for (int num66 = 0; num66 < rankingRecord.cavalry_LifeData.Length; num66++) {
                for (int num67 = 0; num67 < rankingRecord.cavalry_LifeData.Length; num67++) {
                    if (rankingRecord.cavalry_LifeData[num66] == rankingRecord.cavalry_LifeData[num67] && rankingRecord.cavalry_CapData[num66] == rankingRecord.cavalry_CapData[num67] && !array12[num67]) {
                        rankingRecord.rankNo[num67] = num66;
                        array12[num67] = true;
                    }
                }
            }
            if (pointData != null) {
                int num68 = 0;
                bool flag5 = false;
                int num69 = 0;
                int num70 = -1;
                for (int num71 = 0; num71 < rankingRecord.rankNo.Length; num71++) {
                    for (int num72 = 0; num72 < rankingRecord.rankNo.Length; num72++) {
                        if (rankingRecord.rankNo[num71] == rankingRecord.rankNo[num72]) {
                            num68++;
                        }
                    }
                    if (num68 > 1 && !flag5) {
                        flag5 = true;
                        num69 = num71;
                    }
                    num68 = 0;
                }
                if (flag5) {
                    for (int num73 = 0; num73 < rankingRecord.rankNo.Length; num73++) {
                        if (rankingRecord.rankNo[num73] <= num69) {
                            rankingRecord.point[num73] = pointData[rankingRecord.rankNo[num73]];
                            continue;
                        }
                        if (num70 == -1) {
                            num70 = rankingRecord.rankNo[num73];
                        }
                        if (num70 == rankingRecord.rankNo[num73]) {
                            rankingRecord.point[num73] = pointData[num69 + 1];
                        } else {
                            rankingRecord.point[num73] = pointData[num69 + 2];
                        }
                    }
                } else {
                    for (int num74 = 0; num74 < rankingRecord.rankNo.Length; num74++) {
                        rankingRecord.point[num74] = pointData[rankingRecord.rankNo[num74]];
                    }
                }
            }
        }
        return rankingRecord;
    }
    public static RankingRecord GetRecord(bool _isGroup1Record = true) {
        if (_isGroup1Record) {
            return rankPlayerRecords_Group1;
        }
        return rankPlayerRecords_Group2;
    }
    public static int GetTeamTotalRecord(int _teamNo) {
        int num = 0;
        for (int i = 0; i < rankPlayerRecords_Group1.intData.Length; i++) {
            for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++) {
                for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j].Count; k++) {
                    if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][k] == (int)rankPlayerRecords_Group1.userTypeList[i].userType[0]) {
                        if (j == _teamNo) {
                            num += rankPlayerRecords_Group1.intData[i];
                        }
                        break;
                    }
                }
            }
        }
        for (int l = 0; l < rankPlayerRecords_Group2.intData.Length; l++) {
            for (int m = 0; m < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; m++) {
                for (int n = 0; n < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[m].Count; n++) {
                    if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[m][n] == (int)rankPlayerRecords_Group2.userTypeList[l].userType[0]) {
                        if (m == _teamNo) {
                            num += rankPlayerRecords_Group2.intData[l];
                        }
                        break;
                    }
                }
            }
        }
        return num;
    }
    public static int[] GetRankPlayerNo(int _teamNo, int _checkRankNo, bool _isGroup1) {
        List<int> list = new List<int>();
        if (_isGroup1) {
            for (int i = 0; i < rankPlayerRecords_Group1.rankNo.Length; i++) {
                if (rankPlayerRecords_Group1.rankNo[i] != _checkRankNo) {
                    continue;
                }
                for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++) {
                    for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j].Count; k++) {
                        if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][k] == (int)rankPlayerRecords_Group1.userTypeList[i].userType[0] && rankPlayerRecords_Group1.userTypeList[_teamNo].userType[0] >= UserType.PLAYER_1 && rankPlayerRecords_Group1.userTypeList[_teamNo].userType[0] <= UserType.PLAYER_4 && _teamNo == j) {
                            UnityEngine.Debug.Log("指定順位[" + _checkRankNo.ToString() + "] <- チ\u30fcム番号[" + _teamNo.ToString() + "]が入賞！\u3000所属ユ\u30fcザ\u30fc[" + rankPlayerRecords_Group1.userTypeList[i].userType[0].ToString() + "]");
                            list.Add((int)rankPlayerRecords_Group1.userTypeList[i].userType[0]);
                            break;
                        }
                    }
                }
            }
        } else {
            if (rankPlayerRecords_Group2.rankNo == null) {
                return null;
            }
            for (int l = 0; l < rankPlayerRecords_Group2.rankNo.Length; l++) {
                if (rankPlayerRecords_Group2.rankNo[l] != _checkRankNo) {
                    continue;
                }
                for (int m = 0; m < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; m++) {
                    for (int n = 0; n < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[m].Count; n++) {
                        if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[m][n] == (int)rankPlayerRecords_Group2.userTypeList[l].userType[0] && rankPlayerRecords_Group2.userTypeList[l].userType[0] >= UserType.PLAYER_1 && rankPlayerRecords_Group2.userTypeList[l].userType[0] <= UserType.PLAYER_4 && _teamNo == m) {
                            UnityEngine.Debug.Log("指定順位[" + _checkRankNo.ToString() + "] <- チ\u30fcム番号[" + _teamNo.ToString() + "]が入賞！\u3000所属ユ\u30fcザ\u30fc[" + rankPlayerRecords_Group2.userTypeList[l].userType[0].ToString() + "]");
                            list.Add((int)rankPlayerRecords_Group2.userTypeList[l].userType[0]);
                            break;
                        }
                    }
                }
            }
        }
        return list.ToArray();
    }
    public static int[] GetFirstRankCPUNo(int _teamNo, int _checkRankNo, bool _isGroup1) {
        List<int> list = new List<int>();
        if (_isGroup1) {
            for (int i = 0; i < rankPlayerRecords_Group1.rankNo.Length; i++) {
                if (rankPlayerRecords_Group1.rankNo[i] != _checkRankNo) {
                    continue;
                }
                for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++) {
                    for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j].Count; k++) {
                        if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][k] == (int)rankPlayerRecords_Group1.userTypeList[i].userType[0] && rankPlayerRecords_Group1.userTypeList[_teamNo].userType[0] >= UserType.CPU_1 && rankPlayerRecords_Group1.userTypeList[_teamNo].userType[0] <= UserType.CPU_7 && _teamNo == j) {
                            UnityEngine.Debug.Log("指定順位[" + _checkRankNo.ToString() + "] <- チ\u30fcム番号[" + _teamNo.ToString() + "]が入賞！\u3000所属ユ\u30fcザ\u30fc[" + rankPlayerRecords_Group1.userTypeList[i].userType[0].ToString() + "]");
                            list.Add((int)rankPlayerRecords_Group1.userTypeList[i].userType[0]);
                            break;
                        }
                    }
                }
            }
        } else {
            if (rankPlayerRecords_Group2.rankNo == null) {
                return null;
            }
            for (int l = 0; l < rankPlayerRecords_Group2.rankNo.Length; l++) {
                if (rankPlayerRecords_Group2.rankNo[l] != _checkRankNo) {
                    continue;
                }
                for (int m = 0; m < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; m++) {
                    for (int n = 0; n < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[m].Count; n++) {
                        if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[m][n] == (int)rankPlayerRecords_Group2.userTypeList[l].userType[0] && rankPlayerRecords_Group2.userTypeList[l].userType[0] >= UserType.CPU_1 && rankPlayerRecords_Group2.userTypeList[l].userType[0] <= UserType.CPU_7 && _teamNo == m) {
                            UnityEngine.Debug.Log("指定順位[" + _checkRankNo.ToString() + "] <- チ\u30fcム番号[" + _teamNo.ToString() + "]が入賞！\u3000所属ユ\u30fcザ\u30fc[" + rankPlayerRecords_Group2.userTypeList[l].userType[0].ToString() + "]");
                            list.Add((int)rankPlayerRecords_Group2.userTypeList[l].userType[0]);
                            break;
                        }
                    }
                }
            }
        }
        return list.ToArray();
    }
    public static int GetTeamNum() {
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
            return 2;
        }
        return SingletonCustom<GameSettingManager>.Instance.TeamNum;
    }
    public static int GetPlayerRank(int _playerNo, bool _isGroup1) {
        if (_isGroup1) {
            for (int i = 0; i < rankPlayerRecords_Group1.rankNo.Length; i++) {
                if (rankPlayerRecords_Group1.userTypeList[i].userType[0] == (UserType)_playerNo) {
                    return rankPlayerRecords_Group1.rankNo[i];
                }
            }
        } else {
            for (int j = 0; j < rankPlayerRecords_Group2.rankNo.Length; j++) {
                if (rankPlayerRecords_Group2.userTypeList[j].userType[0] == (UserType)_playerNo) {
                    return rankPlayerRecords_Group2.rankNo[j];
                }
            }
        }
        UnityEngine.Debug.LogError("指定のプレイヤ\u30fc番号は現在の記録に該当しません！ →\u3000" + (_isGroup1 ? "１" : "２") + "組目\u3000[" + _playerNo.ToString() + "]");
        if (_isGroup1) {
            for (int k = 0; k < rankPlayerRecords_Group1.userTypeList.Length; k++) {
                UnityEngine.Debug.LogError("１組目に設定されているユ\u30fcザ\u30fc番号：[" + k.ToString() + "]番目：" + rankPlayerRecords_Group1.userTypeList[k].userType[0].ToString());
            }
        } else {
            for (int l = 0; l < rankPlayerRecords_Group2.userTypeList.Length; l++) {
                UnityEngine.Debug.LogError("２組目に設定されているユ\u30fcザ\u30fc番号：[" + l.ToString() + "]番目：" + rankPlayerRecords_Group2.userTypeList[l].userType[0].ToString());
            }
        }
        return -1;
    }
    public static int GetPlayer8Rank(int _playerNo) {
        for (int i = 0; i < rankPlayerRecords_Group1.rankNo.Length; i++) {
            if (rankPlayerRecords_Group1.userTypeList[i].userType[0] == (UserType)_playerNo) {
                return rankPlayerRecords_Group1.rankNo[i];
            }
        }
        for (int j = 0; j < rankPlayerRecords_Group2.rankNo.Length; j++) {
            if (rankPlayerRecords_Group2.userTypeList[j].userType[0] == (UserType)_playerNo) {
                return rankPlayerRecords_Group2.rankNo[j];
            }
        }
        UnityEngine.Debug.LogError("指定のプレイヤ\u30fc番号は現在の記録に該当しません！ →\u3000[" + _playerNo.ToString() + "]");
        for (int k = 0; k < rankPlayerRecords_Group1.userTypeList.Length; k++) {
            UnityEngine.Debug.LogError("１組目に設定されているユ\u30fcザ\u30fc番号：[" + k.ToString() + "]番目：" + rankPlayerRecords_Group1.userTypeList[k].userType[0].ToString());
        }
        for (int l = 0; l < rankPlayerRecords_Group2.userTypeList.Length; l++) {
            UnityEngine.Debug.LogError("２組目に設定されているユ\u30fcザ\u30fc番号：[" + l.ToString() + "]番目：" + rankPlayerRecords_Group2.userTypeList[l].userType[0].ToString());
        }
        return -1;
    }
    public static int GetPointData_WinOrLose(ResultType _resultType) {
        return pointData[(int)_resultType];
    }
    public static int GetWinOrLoseRecord_Int(int _teamNo) {
        return record_WinOrLoseResul_Int[_teamNo];
    }
    public static float GetWinOrLoseRecord_Float(int _teamNo) {
        return record_WinOrLoseResul_Float[_teamNo];
    }
    public static PlayKingRankingData GetPlayKingRankingData(int _teamNo) {
        return rankingDatas[_teamNo];
    }
    public static PlayKingRankingData[] GetAllPlayKingRankingData() {
        return rankingDatas;
    }
    public static PlayKingDetailsData[] GetPlayKingDetailsData(bool _isSort = true) {
        UnityEngine.Debug.Log("playKingDetailsDatas[Len]:" + playKingDetailsDatas.Length.ToString());
        for (int i = 0; i < playKingDetailsDatas.Length; i++) {
            playKingDetailsDatas[i].rankNoArray = new int[SingletonCustom<GameSettingManager>.Instance.TeamNum];
            playKingDetailsDatas[i].teamNoArray = new int[SingletonCustom<GameSettingManager>.Instance.TeamNum];
            UnityEngine.Debug.Log("[i]:" + i.ToString());
            for (int j = 0; j < playKingDetailsDatas[i].rankNoArray.Length; j++) {
                UnityEngine.Debug.Log("rankingDatas[j].rankNoList[Len]:" + rankingDatas[j].rankNoList.Count.ToString());
                playKingDetailsDatas[i].rankNoArray[j] = rankingDatas[j].rankNoList[i];
            }
            for (int k = 0; k < playKingDetailsDatas[i].teamNoArray.Length; k++) {
                playKingDetailsDatas[i].teamNoArray[k] = k;
            }
            if (_isSort) {
                CalcManager.QuickSort(playKingDetailsDatas[i].rankNoArray, playKingDetailsDatas[i].teamNoArray, _isAscendingOrder: true);
            }
        }
        return playKingDetailsDatas;
    }
    public static int[] GetPlayKingFinalRanking() {
        int[] array = new int[4];
        int[] array2 = new int[4];
        for (int i = 0; i < array.Length; i++) {
            array[i] = i;
            array2[i] = SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(i);
            UnityEngine.Debug.Log("i" + i.ToString());
            UnityEngine.Debug.Log("score:" + array2[i].ToString());
        }
        CalcManager.QuickSort(array2, array, _isAscendingOrder: false);
        return array;
    }
    public static void ResetData() {
        rankPlayerRecords_Group1 = default(RankingRecord);
        rankPlayerRecords_Group2 = default(RankingRecord);
    }
    public static void SetWinOrLosePointData_Draw() {
        winOrLoseGetPointData[0] = pointData[2];
        winOrLoseGetPointData[1] = pointData[2];
        winOrLoseGetPointData[2] = pointData[2];
        winOrLoseGetPointData[3] = pointData[2];
    }
    public static void SetWinOrLosePointData(List<int> _winTeamIdx) {
        for (int i = 0; i < winOrLoseGetPointData.Length; i++) {
            winOrLoseGetPointData[i] = pointData[1];
        }
        for (int j = 0; j < _winTeamIdx.Count; j++) {
            for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; k++) {
                if (_winTeamIdx[j] == SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[k][0]) {
                    winOrLoseGetPointData[k] = pointData[0];
                }
            }
        }
    }
    public static void SetWinOrLosePointData(bool _isWhiteTeamWin, bool _isRedTeamWin) {
        if (_isWhiteTeamWin) {
            winOrLoseGetPointData[0] = pointData[0];
            winOrLoseGetPointData[1] = pointData[1];
        } else if (_isRedTeamWin) {
            winOrLoseGetPointData[0] = pointData[1];
            winOrLoseGetPointData[1] = pointData[0];
        } else {
            winOrLoseGetPointData[0] = pointData[2];
            winOrLoseGetPointData[1] = pointData[2];
        }
    }
    public static void SetRankingPointData_Ranking8Numbers(bool _isWinner_TeamA, bool _isWinner_TeamB) {
        if (_isWinner_TeamA) {
            winOrLoseGetPointData[0] = 100;
            winOrLoseGetPointData[1] = 50;
        } else if (_isWinner_TeamB) {
            winOrLoseGetPointData[0] = 50;
            winOrLoseGetPointData[1] = 100;
        } else {
            winOrLoseGetPointData[0] = 75;
            winOrLoseGetPointData[1] = 75;
        }
    }
    public static void SetPlayKingRankData(int _teamNo, int _rank) {
        UnityEngine.Debug.Log("teamNo:" + _teamNo.ToString() + " len:" + rankingDatas.Length.ToString());
        rankingDatas[_teamNo].rankNoList.Add(_rank);
    }
    public static void InitPlayKingRankData() {
        if (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup) {
            rankingDatas = new PlayKingRankingData[6];
        } else {
            rankingDatas = new PlayKingRankingData[6];
        }
        for (int i = 0; i < rankingDatas.Length; i++) {
            rankingDatas[i].rankNoList = new List<int>();
        }
    }
    public static void SetDebugPlayKingRankData() {
        InitPlayKingRankData();
        for (int i = 0; i < 12; i++) {
            for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.TeamNum; j++) {
                rankingDatas[j].rankNoList.Add(j);
            }
        }
    }
    public static bool IsUserNoBelongTeam(int _teamNo, int _userNo) {
        if (_teamNo >= SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length) {
            UnityEngine.Debug.LogError("グル\u30fcプ番号が配列外です！！");
            return false;
        }
        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_teamNo].Count; i++) {
            if (_userNo == SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_teamNo][i]) {
                return true;
            }
        }
        return false;
    }
    public static bool IsUserNoJoinGroup(int _groupNo, int _userNo) {
        if (_groupNo >= 2) {
            UnityEngine.Debug.LogError("グル\u30fcプ番号が範囲外です！！");
            return false;
        }
        if (_groupNo == 0) {
            for (int i = 0; i < rankPlayerRecords_Group1.userTypeList.Length; i++) {
                for (int j = 0; j < rankPlayerRecords_Group1.userTypeList[i].userType.Length; j++) {
                    if (_userNo == (int)rankPlayerRecords_Group1.userTypeList[i].userType[j]) {
                        return true;
                    }
                }
            }
        } else {
            for (int k = 0; k < rankPlayerRecords_Group2.userTypeList.Length; k++) {
                for (int l = 0; l < rankPlayerRecords_Group2.userTypeList[k].userType.Length; l++) {
                    if (_userNo == (int)rankPlayerRecords_Group2.userTypeList[k].userType[l]) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public static void DebugLog_RecordData(bool _isGroup1) {
        UnityEngine.Debug.Log("---------------記録デ\u30fcタ：ログ出力[" + (_isGroup1 ? "１組目" : "２組目") + "]---------------");
        if (_isGroup1) {
            for (int i = 0; i < rankPlayerRecords_Group1.rankNo.Length; i++) {
                UnityEngine.Debug.Log("要素番号[" + i.ToString() + "]\u3000順位：" + (rankPlayerRecords_Group1.rankNo[i] + 1).ToString() + "位");
            }
            for (int j = 0; j < rankPlayerRecords_Group1.userTypeList.Length; j++) {
                for (int k = 0; k < rankPlayerRecords_Group1.userTypeList[j].userType.Length; k++) {
                    UnityEngine.Debug.Log("要素番号[" + j.ToString() + "]\u3000記録保持ユ\u30fcザ\u30fc番号：" + rankPlayerRecords_Group1.userTypeList[j].userType[k].ToString());
                }
            }
            for (int l = 0; l < rankPlayerRecords_Group1.userTypeList.Length; l++) {
                for (int m = 0; m < rankPlayerRecords_Group1.userTypeList[l].userType.Length; m++) {
                    if (IsUserNoBelongTeam(0, (int)rankPlayerRecords_Group1.userTypeList[l].userType[m])) {
                        UnityEngine.Debug.Log("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group1.userTypeList[l].userType[m].ToString() + "]\u3000所属：チ\u30fcムA");
                    } else if (IsUserNoBelongTeam(1, (int)rankPlayerRecords_Group1.userTypeList[l].userType[m])) {
                        UnityEngine.Debug.Log("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group1.userTypeList[l].userType[m].ToString() + "]\u3000所属：チ\u30fcムB");
                    } else if (IsUserNoBelongTeam(2, (int)rankPlayerRecords_Group1.userTypeList[l].userType[m])) {
                        UnityEngine.Debug.Log("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group1.userTypeList[l].userType[m].ToString() + "]\u3000所属：チ\u30fcムC");
                    } else if (IsUserNoBelongTeam(3, (int)rankPlayerRecords_Group1.userTypeList[l].userType[m])) {
                        UnityEngine.Debug.Log("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group1.userTypeList[l].userType[m].ToString() + "]\u3000所属：チ\u30fcムD");
                    } else if (IsUserNoBelongTeam(4, (int)rankPlayerRecords_Group1.userTypeList[l].userType[m])) {
                        UnityEngine.Debug.Log("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group1.userTypeList[l].userType[m].ToString() + "]\u3000所属：チ\u30fcムD");
                    } else if (IsUserNoBelongTeam(5, (int)rankPlayerRecords_Group1.userTypeList[l].userType[m])) {
                        UnityEngine.Debug.Log("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group1.userTypeList[l].userType[m].ToString() + "]\u3000所属：チ\u30fcムD");
                    } else {
                        UnityEngine.Debug.LogError("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group1.userTypeList[l].userType[m].ToString() + "]はPlayerGroupListに登録されていません！");
                    }
                }
            }
            if (rankPlayerRecords_Group1.point != null) {
                for (int n = 0; n < rankPlayerRecords_Group1.point.Length; n++) {
                    UnityEngine.Debug.Log("要素番号[" + n.ToString() + "]\u3000獲得ポイント：" + rankPlayerRecords_Group1.point[n].ToString() + "P");
                }
            } else {
                UnityEngine.Debug.Log("<color=red>獲得ポイントデ\u30fcタなし</color>");
            }
            if (rankPlayerRecords_Group1.intData != null) {
                for (int num = 0; num < rankPlayerRecords_Group1.intData.Length; num++) {
                    UnityEngine.Debug.Log("要素番号[" + num.ToString() + "]\u3000Int型記録：" + rankPlayerRecords_Group1.intData[num].ToString());
                }
            } else {
                UnityEngine.Debug.Log("<color=red>Int型デ\u30fcタ記録無し</color>");
            }
            if (rankPlayerRecords_Group1.stringData != null) {
                for (int num2 = 0; num2 < rankPlayerRecords_Group1.stringData.Length; num2++) {
                    UnityEngine.Debug.Log("要素番号[" + num2.ToString() + "]\u3000String型記録：" + rankPlayerRecords_Group1.stringData[num2]);
                }
            } else {
                UnityEngine.Debug.Log("<color=red>String型デ\u30fcタ記録無し</color>");
            }
            if (rankPlayerRecords_Group1.floatData != null) {
                for (int num3 = 0; num3 < rankPlayerRecords_Group1.floatData.Length; num3++) {
                    UnityEngine.Debug.Log("要素番号[" + num3.ToString() + "]\u3000Float型記録：" + rankPlayerRecords_Group1.floatData[num3].ToString());
                }
            } else {
                UnityEngine.Debug.Log("<color=red>Float型デ\u30fcタ記録無し</color>");
            }
            if (rankPlayerRecords_Group1.cavalry_LifeData != null && rankPlayerRecords_Group1.cavalry_CapData != null) {
                for (int num4 = 0; num4 < rankPlayerRecords_Group1.cavalry_LifeData.Length; num4++) {
                    UnityEngine.Debug.Log("要素番号[" + num4.ToString() + "]\u3000騎馬戦記録(残機)：" + rankPlayerRecords_Group1.cavalry_LifeData[num4].ToString());
                }
                for (int num5 = 0; num5 < rankPlayerRecords_Group1.cavalry_CapData.Length; num5++) {
                    UnityEngine.Debug.Log("要素番号[" + num5.ToString() + "]\u3000騎馬戦記録(帽子の数)：" + rankPlayerRecords_Group1.cavalry_CapData[num5].ToString());
                }
            } else {
                UnityEngine.Debug.Log("<color=red>騎馬戦専用デ\u30fcタ記録無し</color>");
            }
            if (rankPlayerRecords_Group1.tournamentWinnerData != null && rankPlayerRecords_Group1.tournamentMatchData != null) {
                for (int num6 = 0; num6 < rankPlayerRecords_Group1.tournamentWinnerData.Length; num6++) {
                    UnityEngine.Debug.Log("要素番号[" + num6.ToString() + "]\u3000ト\u30fcナメント第" + (num6 + 1).ToString() + "試合 勝利者：" + rankPlayerRecords_Group1.tournamentWinnerData[num6].ToString());
                }
                for (int num7 = 0; num7 < rankPlayerRecords_Group1.tournamentMatchData.Length; num7++) {
                    UnityEngine.Debug.Log("要素番号[" + num7.ToString() + "]\u3000ト\u30fcナメント" + ((num7 == 0) ? "左" : "右") + "側 組み合わせ：" + rankPlayerRecords_Group1.tournamentMatchData[num7]);
                }
            } else {
                UnityEngine.Debug.Log("<color=red>ト\u30fcナメント専用デ\u30fcタ記録無し</color>");
            }
        } else {
            for (int num8 = 0; num8 < rankPlayerRecords_Group2.rankNo.Length; num8++) {
                UnityEngine.Debug.Log("要素番号[" + num8.ToString() + "]\u3000順位：" + (rankPlayerRecords_Group2.rankNo[num8] + 1).ToString() + "位");
            }
            for (int num9 = 0; num9 < rankPlayerRecords_Group2.userTypeList.Length; num9++) {
                for (int num10 = 0; num10 < rankPlayerRecords_Group2.userTypeList[num9].userType.Length; num10++) {
                    UnityEngine.Debug.Log("要素番号[" + num9.ToString() + "]\u3000記録保持ユ\u30fcザ\u30fc番号：" + rankPlayerRecords_Group2.userTypeList[num9].userType[num10].ToString());
                }
            }
            for (int num11 = 0; num11 < rankPlayerRecords_Group2.userTypeList.Length; num11++) {
                for (int num12 = 0; num12 < rankPlayerRecords_Group2.userTypeList[num11].userType.Length; num12++) {
                    if (IsUserNoBelongTeam(0, (int)rankPlayerRecords_Group2.userTypeList[num11].userType[num12])) {
                        UnityEngine.Debug.Log("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group2.userTypeList[num11].userType[num12].ToString() + "]\u3000所属：チ\u30fcムA");
                    } else if (IsUserNoBelongTeam(1, (int)rankPlayerRecords_Group2.userTypeList[num11].userType[num12])) {
                        UnityEngine.Debug.Log("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group2.userTypeList[num11].userType[num12].ToString() + "]\u3000所属：チ\u30fcムB");
                    } else if (IsUserNoBelongTeam(2, (int)rankPlayerRecords_Group2.userTypeList[num11].userType[num12])) {
                        UnityEngine.Debug.Log("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group2.userTypeList[num11].userType[num12].ToString() + "]\u3000所属：チ\u30fcムC");
                    } else if (IsUserNoBelongTeam(3, (int)rankPlayerRecords_Group2.userTypeList[num11].userType[num12])) {
                        UnityEngine.Debug.Log("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group2.userTypeList[num11].userType[num12].ToString() + "]\u3000所属：チ\u30fcムD");
                    } else {
                        UnityEngine.Debug.LogError("記録保持ユ\u30fcザ\u30fc番号[" + rankPlayerRecords_Group2.userTypeList[num11].userType[num12].ToString() + "]はPlayerGroupListに登録されていません！");
                    }
                }
            }
            if (rankPlayerRecords_Group2.point != null) {
                for (int num13 = 0; num13 < rankPlayerRecords_Group2.point.Length; num13++) {
                    UnityEngine.Debug.Log("要素番号[" + num13.ToString() + "]\u3000獲得ポイント：" + rankPlayerRecords_Group2.point[num13].ToString() + "P");
                }
            } else {
                UnityEngine.Debug.Log("<color=red>獲得ポイントデ\u30fcタなし</color>");
            }
            if (rankPlayerRecords_Group2.intData != null) {
                for (int num14 = 0; num14 < rankPlayerRecords_Group2.intData.Length; num14++) {
                    UnityEngine.Debug.Log("要素番号[" + num14.ToString() + "]\u3000Int型記録：" + rankPlayerRecords_Group2.intData[num14].ToString());
                }
            } else {
                UnityEngine.Debug.Log("<color=red>Int型デ\u30fcタ記録無し</color>");
            }
            if (rankPlayerRecords_Group2.stringData != null) {
                for (int num15 = 0; num15 < rankPlayerRecords_Group2.stringData.Length; num15++) {
                    UnityEngine.Debug.Log("要素番号[" + num15.ToString() + "]\u3000String型記録：" + rankPlayerRecords_Group2.stringData[num15]);
                }
            } else {
                UnityEngine.Debug.Log("<color=red>String型デ\u30fcタ記録無し</color>");
            }
            if (rankPlayerRecords_Group2.floatData != null) {
                for (int num16 = 0; num16 < rankPlayerRecords_Group2.floatData.Length; num16++) {
                    UnityEngine.Debug.Log("要素番号[" + num16.ToString() + "]\u3000Float型記録：" + rankPlayerRecords_Group2.floatData[num16].ToString());
                }
            } else {
                UnityEngine.Debug.Log("<color=red>Float型デ\u30fcタ記録無し</color>");
            }
            if (rankPlayerRecords_Group2.cavalry_LifeData != null && rankPlayerRecords_Group2.cavalry_CapData != null) {
                for (int num17 = 0; num17 < rankPlayerRecords_Group2.cavalry_LifeData.Length; num17++) {
                    UnityEngine.Debug.Log("要素番号[" + num17.ToString() + "]\u3000騎馬戦記録(残機)：" + rankPlayerRecords_Group2.cavalry_LifeData[num17].ToString());
                }
                for (int num18 = 0; num18 < rankPlayerRecords_Group2.cavalry_CapData.Length; num18++) {
                    UnityEngine.Debug.Log("要素番号[" + num18.ToString() + "]\u3000騎馬戦記録(帽子の数)：" + rankPlayerRecords_Group2.cavalry_CapData[num18].ToString());
                }
            } else {
                UnityEngine.Debug.Log("<color=red>騎馬戦専用デ\u30fcタ記録無し</color>");
            }
            if (rankPlayerRecords_Group2.tournamentWinnerData != null && rankPlayerRecords_Group2.tournamentMatchData != null) {
                for (int num19 = 0; num19 < rankPlayerRecords_Group2.tournamentWinnerData.Length; num19++) {
                    UnityEngine.Debug.Log("要素番号[" + num19.ToString() + "]\u3000ト\u30fcナメント第" + (num19 + 1).ToString() + "試合 勝利者：" + rankPlayerRecords_Group2.tournamentWinnerData[num19].ToString());
                }
                for (int num20 = 0; num20 < rankPlayerRecords_Group2.tournamentMatchData.Length; num20++) {
                    UnityEngine.Debug.Log("要素番号[" + num20.ToString() + "]\u3000ト\u30fcナメント" + ((num20 == 0) ? "左" : "右") + "側 組み合わせ：" + rankPlayerRecords_Group2.tournamentMatchData[num20]);
                }
            } else {
                UnityEngine.Debug.Log("<color=red>ト\u30fcナメント専用デ\u30fcタ記録無し</color>");
            }
        }
        UnityEngine.Debug.Log("-----------------------------------------------");
    }
}
