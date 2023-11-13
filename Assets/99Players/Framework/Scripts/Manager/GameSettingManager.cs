using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameSettingManager : SingletonCustom<GameSettingManager> {
    [Serializable]
    public class TeamData {
        public int teamNo;
        public int score;
        public TeamData(int _teamNo, int _score) {
            teamNo = _teamNo;
            score = _score;
        }
    }
    [Serializable]
    public class ResultGameData {
        public List<TeamData> listTeamData;
        public ResultGameData() {
            listTeamData = new List<TeamData>();
        }
    }
    private class TableList : IEnumerable {
        private List<SceneManager.SceneType> _tableList = new List<SceneManager.SceneType>();
        public void Add(SceneManager.SceneType _item) {
            _tableList.Add(_item);
        }
        public SceneManager.SceneType Get(int _idx) {
            return _tableList[_idx];
        }
        public IEnumerator GetEnumerator() {
            throw new NotImplementedException();
        }
        public void Clear() {
            _tableList.Clear();
        }
        public int Count() {
            return _tableList.Count;
        }
    }
    public enum CameraDirType {
        HORIZONTAL,
        VERTICAL
    }
    public enum GameProgressType {
        SINGLE,
        ALL_SPORTS
    }
    public ResultGameDataParams.PlayKingRankingData[] rankingDatas;
    public ResultGameDataParams.PlayKingDetailsData[] playKingDetailsDatas;
    public GS_Define.GameType LastSelectGameType;
    public readonly int SPORTS_DAY_SPORTS_NUM = 10;
    [SerializeField]
    [Header("スクロ\u30fcルx")]
    private float scrollX = -0.01f;
    [SerializeField]
    [Header("スクロ\u30fcルy")]
    private float scrollY = -0.01f;
    public float GlobalScrollX;
    public float GlobalScrollY;
    private List<int>[] playerGroupList;
    private int[] playerTeamAssignment;
    private int[] playerNpadId;
    [SerializeField]
    private int[] arraySelectCharacter;
    private TableList sportsDayTable;
    private int playKingTableCnt;
    private bool isCheerBattle;
    private List<int> listSelectIdxAlloc = new List<int>();
    private List<ResultGameData> listResultGameData;
    private int m_HighestCpuId;
    public int[] ArraySelectChracterIdx {
        get {
            return arraySelectCharacter;
        }
        set {
            arraySelectCharacter = value;
        }
    }
    public bool IsCpuFixSelect {
        get;
        set;
    }
    public GameProgressType SelectGameProgressType {
        get;
        set;
    }
    public GS_Define.GameFormat SelectGameFormat {
        get;
        set;
    }
    public int SelectGameNum {
        get;
        set;
    }
    public int PlayerNum {
        get;
        set;
    }
    public int TeamNum {
        get;
        set;
    }
    public int PlayKingTableCnt => playKingTableCnt;
    public int SportsDayTotalCnt => sportsDayTable.Count();
    public bool IsCheerBattle => isCheerBattle;
    public List<ResultGameData> ListResultGameData => listResultGameData;
    public int ShootingStageIdx {
        get;
        set;
    }
    public int BackFrameTexIdx {
        get;
        set;
    }
    public int ResultDecoIdx {
        get;
        set;
    }
    public int SelectGameMode {
        get;
        set;
    }
    public int SelectInningNum {
        get;
        set;
    }
    public int SelectMatchTimeIdx {
        get;
        set;
    }
    public int SelectCourseIdx {
        get;
        set;
    }
    public int SelectStageIdx {
        get;
        set;
    }
    public int ControllerNum {
        get;
        set;
    }
    public bool IsSinglePlay => PlayerNum == 1;
    public bool IsSingleController {
        get {
            if (!IsSinglePlay) {
                if (PlayerNum > 1) {
                    return ControllerNum == 1;
                }
                return false;
            }
            return true;
        }
    }
    public List<int>[] PlayerGroupList {
        get {
            return playerGroupList;
        }
        set {
            playerGroupList = value;
        }
    }
    public int[] PlayerTeamAssignment {
        get {
            return playerTeamAssignment;
        }
        set {
            playerTeamAssignment = value;
        }
    }
    public bool IsSixPlayerGroup => PlayerGroupList.Length != GS_Define.PLAYER_SMALL_MAX;
    public bool IsEightBattle => false;
    public CameraDirType CameraDir {
        get;
        set;
    }
    public int SelectFreeModeIdx {
        get;
        set;
    }
    public int SelectPartyModeIdx {
        get;
        set;
    }
    public int SelectPartyNumIdx {
        get;
        set;
    }
    public void ResetDetailSetting() {
        SelectGameMode = 0;
        SelectInningNum = 0;
        SelectMatchTimeIdx = 0;
        SelectCourseIdx = 0;
        SelectStageIdx = 0;
    }
    public void ShuffleCpuCharacterSelect() {
        listSelectIdxAlloc.Clear();
        for (int i = 0; i < 8; i++) {
            listSelectIdxAlloc.Add(i);
        }
        for (int j = 0; j < PlayerNum; j++) {
            listSelectIdxAlloc.Remove(ArraySelectChracterIdx[j]);
        }
        listSelectIdxAlloc.Shuffle();
        for (int k = 0; k < listSelectIdxAlloc.Count; k++) {
            ArraySelectChracterIdx[PlayerNum + k] = listSelectIdxAlloc[k];
        }
        ArraySelectChracterIdx[8] = ArraySelectChracterIdx[3];
        ArraySelectChracterIdx[9] = ArraySelectChracterIdx[2];
        ArraySelectChracterIdx[10] = ArraySelectChracterIdx[1];
    }
    public void SetSelectGame(List<int> list) {
        sportsDayTable.Clear();
        for (int i = 0; i < list.Count; i++) {
            int num = (int)SingletonCustom<GS_GameSelectManager>.Instance.ArrayCursorGameType[list[i]];
            sportsDayTable.Add((SceneManager.SceneType)(num + 2));
        }
    }
    public SceneManager.SceneType GetLastGameType() {
        return sportsDayTable.Get(sportsDayTable.Count() - 1);
    }
    public void InitSportsDay() {
        playKingTableCnt = 0;
        isCheerBattle = false;
        for (int i = 0; i < listResultGameData.Count; i++) {
            listResultGameData[i].listTeamData.Clear();
        }
    }
    public int GetSportsDayTotalTeamScore(int _idx) {
        int num = 0;
        for (int i = 0; i < listResultGameData.Count; i++) {
            for (int j = 0; j < listResultGameData[i].listTeamData.Count; j++) {
                if (listResultGameData[i].listTeamData[j].teamNo == _idx) {
                    UnityEngine.Debug.Log("idx:" + _idx.ToString() + "score:" + listResultGameData[i].listTeamData[j].score.ToString());
                    num += listResultGameData[i].listTeamData[j].score;
                }
            }
        }
        return num;
    }
    public int[] GetCrownPlayerIdxArray() {
        List<int> list = new List<int>();
        int num = 0;
        int num2 = IsSixPlayerGroup ? GS_Define.PLAYER_MAX : GS_Define.PLAYER_SMALL_MAX;
        for (int i = 0; i < num2; i++) {
            int sportsDayTotalTeamScore = GetSportsDayTotalTeamScore(i);
            if (sportsDayTotalTeamScore > num) {
                num = sportsDayTotalTeamScore;
            }
        }
        if (!SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isCrown) {
            return list.ToArray();
        }
        if (num == 0) {
            return list.ToArray();
        }
        for (int j = 0; j < num2; j++) {
            if (GetSportsDayTotalTeamScore(j) == num) {
                list.Add(j);
            }
        }
        return list.ToArray();
    }
    public int[] GetLowScorePlayerIdxArray(int _count) {
        List<int> list = new List<int>();
        int num = IsSixPlayerGroup ? GS_Define.PLAYER_MAX : GS_Define.PLAYER_SMALL_MAX;
        int[] array = new int[num];
        int[] array2 = new int[num];
        if (!SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isCrown) {
            return list.ToArray();
        }
        for (int i = 0; i < num; i++) {
            for (int j = i; j < num; j++) {
                if (array[i] > array[j]) {
                    int num2 = array[j];
                    array[j] = array[i];
                    array[i] = num2;
                    int num3 = array2[j];
                    array2[j] = array2[i];
                    array2[i] = num3;
                }
            }
        }
        for (int k = 0; k < _count; k++) {
            list.Add(array2[k]);
        }
        return list.ToArray();
    }
    public int GetAllGameHalfTotalTeamScore(int _idx) {
        int num = 0;
        for (int i = 0; i < listResultGameData.Count / 2; i++) {
            for (int j = 0; j < listResultGameData[i].listTeamData.Count; j++) {
                if (listResultGameData[i].listTeamData[j].teamNo == _idx) {
                    num += listResultGameData[i].listTeamData[j].score;
                }
            }
        }
        return num;
    }
    public int GetSportsDayIndex(GS_Define.GameType _type) {
        for (int i = 0; i < sportsDayTable.Count(); i++) {
            UnityEngine.Debug.Log("table:" + sportsDayTable.Get(i).ToString() + "type:" + _type.ToString());
            if (sportsDayTable.Get(i) == (SceneManager.SceneType)(_type + 2)) {
                return i;
            }
        }
        return -1;
    }
    public string GetSportsDaySportsName(int _idx) {
        switch (_idx) {
            case 0:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 1);
            case 1:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 5);
            case 2:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 2);
            case 3:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 8);
            case 4:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 9);
            case 5:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 6);
            case 6:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 4);
            case 7:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 7);
            case 8:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 11);
            case 9:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 3);
            case 10:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 10);
            case 11:
                return SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 0);
            default:
                return "";
        }
    }
    public SceneManager.SceneType NextTable() {
        if (playKingTableCnt >= sportsDayTable.Count()) {
            return SceneManager.SceneType.RESULT_ANNOUNCEMENT;
        }
        int idx = playKingTableCnt;
        playKingTableCnt++;
        return sportsDayTable.Get(idx);
    }
    public void AddSportsDayBrankData() {
        for (int i = 0; i < listResultGameData.Count; i++) {
            if (listResultGameData[i].listTeamData.Count == 0) {
                for (int j = 0; j < TeamNum; j++) {
                    listResultGameData[i].listTeamData.Add(new TeamData(j, UnityEngine.Random.Range(0, 100)));
                }
            }
        }
    }
    public void AddSportsDayDebugData() {
        for (int i = 0; i < listResultGameData.Count; i++) {
            if (listResultGameData[i].listTeamData.Count == 0) {
                for (int j = 0; j < TeamNum; j++) {
                    listResultGameData[i].listTeamData.Add(new TeamData(j, UnityEngine.Random.Range(50, 100)));
                }
            }
        }
    }
    private void Awake() {
        SelectGameProgressType = GameProgressType.SINGLE;
        SelectGameFormat = GS_Define.GameFormat.BATTLE;
        PlayerNum = 1;
        TeamNum = 2;
        SelectGameNum = 5;
        ShootingStageIdx = 0;
        arraySelectCharacter = new int[11];
        for (int i = 0; i < arraySelectCharacter.Length; i++) {
            arraySelectCharacter[i] = i;
        }
        arraySelectCharacter[8] = arraySelectCharacter[3];
        arraySelectCharacter[9] = arraySelectCharacter[2];
        arraySelectCharacter[10] = arraySelectCharacter[1];
        BackFrameTexIdx = UnityEngine.Random.Range(0, 2);
        SelectFreeModeIdx = -1;
        SelectPartyModeIdx = -1;
        SelectPartyNumIdx = -1;
        sportsDayTable = new TableList
        {
            SceneManager.SceneType.GET_BALL,
            SceneManager.SceneType.ARCHER_BATTLE,
            SceneManager.SceneType.CANNON_SHOT,
            SceneManager.SceneType.MOLE_HAMMER,
            SceneManager.SceneType.BOMB_ROULETTE,
            SceneManager.SceneType.RECEIVE_PON,
            SceneManager.SceneType.BLACKSMITH,
            SceneManager.SceneType.BLOCK_WIPER,
            SceneManager.SceneType.ATTACK_BALL,
            SceneManager.SceneType.BLOW_AWAY_TANK
        };
        playKingTableCnt = 0;
        isCheerBattle = false;
        ControllerNum = 1;
        CameraDir = CameraDirType.HORIZONTAL;
        playerGroupList = new List<int>[GS_Define.PLAYER_SMALL_MAX];
        playerTeamAssignment = new int[GS_Define.PLAYER_SMALL_MAX];
        listResultGameData = new List<ResultGameData>();
        for (int j = 0; j < playerGroupList.Length; j++) {
            playerGroupList[j] = new List<int>();
            if (j == 0) {
                playerGroupList[j].Add(0);
            }
        }
        for (int k = 0; k < 35; k++) {
            listResultGameData.Add(new ResultGameData());
        }
        playerNpadId = new int[GS_Define.PLAYER_MAX];
        for (int l = 0; l < GS_Define.PLAYER_MAX; l++) {
            playerNpadId[l] = l;
        }
        ResetDetailSetting();
        ResultDecoIdx = UnityEngine.Random.Range(0, 2);
    }
    public int GetAllocNpadId(int _no) {
        _no = Mathf.Clamp(_no, 0, playerNpadId.Length);
        return playerNpadId[_no];
    }
    public void AllocNpadId() {
        if (PlayerNum > 1 && SingletonCustom<JoyConManager>.Instance.SettingPlayMode == GameManager.PlayModeType.SINGLE) {
            for (int i = 0; i < playerNpadId.Length; i++) {
                playerNpadId[i] = 0;
            }
        } else {
            for (int j = 0; j < playerNpadId.Length; j++) {
                playerNpadId[j] = j;
            }
        }
    }
    public bool CheckTeamAssignment() {
        for (int i = 2; i < playerGroupList.Length; i++) {
            if (playerGroupList[i].Count > 0) {
                return false;
            }
        }
        return true;
    }
    public void AutoPlayerNumSetting() {
        UnityEngine.Debug.Log("■■■■■■■■■■■■auto_Setting■■■■■■■■■■■");
        if (playerGroupList != null) {
            playerGroupList = new List<int>[(PlayerNum <= 4) ? GS_Define.PLAYER_SMALL_MAX : GS_Define.PLAYER_MAX];
            if (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.characterNumSetting == 1) {
                playerGroupList = new List<int>[GS_Define.PLAYER_MAX];
            }
            for (int i = 0; i < playerGroupList.Length; i++) {
                playerGroupList[i] = new List<int>();
            }
        }
        for (int j = 0; j < playerGroupList.Length; j++) {
            playerGroupList[j].Clear();
        }
        switch (PlayerNum) {
            case 1:
                CameraDir = CameraDirType.HORIZONTAL;
                for (int n = 0; n < playerGroupList.Length; n++) {
                    if (n == 0) {
                        playerGroupList[n].Add(0);
                    }
                }
                break;
            case 2:
                CameraDir = CameraDirType.HORIZONTAL;
                for (int num2 = 0; num2 < playerGroupList.Length; num2++) {
                    if (num2 == 0) {
                        playerGroupList[num2].Add(0);
                    }
                    if (num2 == 1) {
                        playerGroupList[num2].Add(1);
                    }
                }
                break;
            case 3:
                CameraDir = CameraDirType.HORIZONTAL;
                for (int l = 0; l < playerGroupList.Length; l++) {
                    if (l == 0) {
                        playerGroupList[l].Add(0);
                    }
                    if (l == 1) {
                        playerGroupList[l].Add(1);
                    }
                    if (l == 2) {
                        playerGroupList[l].Add(2);
                    }
                }
                break;
            case 4:
                CameraDir = CameraDirType.HORIZONTAL;
                for (int num = 0; num < playerGroupList.Length; num++) {
                    if (num == 0) {
                        playerGroupList[num].Add(0);
                    }
                    if (num == 1) {
                        playerGroupList[num].Add(1);
                    }
                    if (num == 2) {
                        playerGroupList[num].Add(2);
                    }
                    if (num == 3) {
                        playerGroupList[num].Add(3);
                    }
                }
                break;
            case 5:
                CameraDir = CameraDirType.HORIZONTAL;
                for (int m = 0; m < playerGroupList.Length; m++) {
                    if (m == 0) {
                        playerGroupList[m].Add(0);
                    }
                    if (m == 1) {
                        playerGroupList[m].Add(1);
                    }
                    if (m == 2) {
                        playerGroupList[m].Add(2);
                    }
                    if (m == 3) {
                        playerGroupList[m].Add(3);
                    }
                    if (m == 4) {
                        playerGroupList[m].Add(4);
                    }
                }
                break;
            case 6:
                CameraDir = CameraDirType.HORIZONTAL;
                for (int k = 0; k < playerGroupList.Length; k++) {
                    if (k == 0) {
                        playerGroupList[k].Add(0);
                    }
                    if (k == 1) {
                        playerGroupList[k].Add(1);
                    }
                    if (k == 2) {
                        playerGroupList[k].Add(2);
                    }
                    if (k == 3) {
                        playerGroupList[k].Add(3);
                    }
                    if (k == 4) {
                        playerGroupList[k].Add(4);
                    }
                    if (k == 5) {
                        playerGroupList[k].Add(5);
                    }
                }
                break;
        }
    }
    public void SetPlayerGroupList(List<int>[] _playerGroupList) {
        playerGroupList = _playerGroupList;
        for (int i = 0; i < playerGroupList.Length; i++) {
            playerGroupList[i].Sort();
        }
    }
    public int GetPlayerNumAtGroup(int _no) {
        int num = 0;
        for (int i = 0; i < playerGroupList[_no].Count; i++) {
            if (playerGroupList[_no][i] < GS_Define.PLAYER_MAX) {
                num++;
            }
        }
        return num;
    }
    public void RemoveCpuToPlayerGroupList() {
        if (playerGroupList == null) {
            return;
        }
        for (int i = 0; i < playerGroupList.Length; i++) {
            for (int j = 6; j < 11; j++) {
                playerGroupList[i].Remove(j);
            }
        }
    }
    public void SetCpuToPlayerGroupList() {
        UnityEngine.Debug.Log("【toList】:" + PlayerNum.ToString());
        UnityEngine.Debug.Log("【SelectGameFormat】:" + SelectGameFormat.ToString());
        AutoPlayerNumSetting();
        switch (SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                switch (PlayerNum) {
                    case 1:
                        playerGroupList[1].Add(6);
                        playerGroupList[2].Add(7);
                        playerGroupList[3].Add(8);
                        if (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.characterNumSetting == 1) {
                            playerGroupList[4].Add(9);
                            playerGroupList[5].Add(10);
                        }
                        break;
                    case 2:
                        playerGroupList[2].Add(6);
                        playerGroupList[3].Add(7);
                        if (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.characterNumSetting == 1) {
                            playerGroupList[4].Add(8);
                            playerGroupList[5].Add(9);
                        }
                        break;
                    case 3:
                        playerGroupList[3].Add(6);
                        if (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.characterNumSetting == 1) {
                            playerGroupList[4].Add(7);
                            playerGroupList[5].Add(8);
                        }
                        break;
                    case 4:
                        if (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.characterNumSetting == 1) {
                            playerGroupList[4].Add(6);
                            playerGroupList[5].Add(7);
                        }
                        break;
                    case 5:
                        playerGroupList[5].Add(6);
                        break;
                }
                break;
            case GS_Define.GameFormat.COOP:
                switch (PlayerNum) {
                    case 2:
                        playerGroupList[1].Add(6);
                        playerGroupList[1].Add(7);
                        break;
                    case 3:
                        playerGroupList[0].Add(6);
                        playerGroupList[1].Add(7);
                        playerGroupList[1].Add(8);
                        playerGroupList[1].Add(9);
                        playerGroupList[1].Add(10);
                        break;
                    case 4:
                        playerGroupList[1].Add(6);
                        playerGroupList[1].Add(7);
                        playerGroupList[1].Add(8);
                        playerGroupList[1].Add(9);
                        break;
                }
                break;
        }
        TeamNum = 0;
        for (int i = 0; i < playerGroupList.Length; i++) {
            if (playerGroupList[i].Count > 0) {
                int num = ++TeamNum;
            }
            playerGroupList[i].Sort();
        }
        UnityEngine.Debug.Log("★ チ\u30fcム数[" + TeamNum.ToString() + "] ★");
        DebugLog_PlayerGroupList();
        // They currently hard-coded them to be started at 6 GetPlayerIdOrCpuId(int index)
        m_HighestCpuId = 5;
    }
    public void DebugLog_PlayerGroupList() {
        for (int i = 0; i < TeamNum; i++) {
            for (int j = 0; j < playerGroupList[i].Count; j++) {
                UnityEngine.Debug.Log("チ\u30fcム[" + i.ToString() + "]：登録ユ\u30fcザ\u30fc番号[" + playerGroupList[i][j].ToString() + "]");
            }
        }
    }
    public int GetPlayerAffiliationTeam(int _playerNo) {
        for (int i = 0; i < playerGroupList.Length; i++) {
            for (int j = 0; j < playerGroupList[i].Count; j++) {
                if (playerGroupList[i][j] == _playerNo) {
                    return i;
                }
            }
        }
        return -1;
    }
    private void LateUpdate() {
        GlobalScrollX = Mathf.Repeat(GlobalScrollX + Time.unscaledDeltaTime * scrollX, 1f);
        GlobalScrollY = Mathf.Repeat(GlobalScrollY + Time.unscaledDeltaTime * scrollY, 1f);
        if (UnityEngine.Input.GetKeyDown(KeyCode.K)) {
            rankingDatas = ResultGameDataParams.rankingDatas;
            playKingDetailsDatas = ResultGameDataParams.playKingDetailsDatas;
        }
    }
    public void DebugLogPlayerGroupList() {
        UnityEngine.Debug.Log("■PlayerGroupListLen:" + playerGroupList.Length.ToString());
        for (int i = 0; i < playerGroupList.Length; i++) {
            for (int j = 0; j < playerGroupList[i].Count; j++) {
                UnityEngine.Debug.Log("PG:[" + i.ToString() + "][" + j.ToString() + "]:" + playerGroupList[i][j].ToString());
            }
        }
    }
    public int GetPlayerIdOrCpuId(int index) {
        if (index < PlayerNum) {
            return index;
        }
        else {
            m_HighestCpuId++;
            return m_HighestCpuId;
        }
    }
}
