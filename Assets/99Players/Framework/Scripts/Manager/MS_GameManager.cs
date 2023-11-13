using System;
using System.Collections.Generic;
using UnityEngine;
public class MS_GameManager : SingletonCustom<MS_GameManager> {
    [Serializable]
    public struct TeamData {
        public int teamNo;
        public List<int> memberPlayerNoList;
        public int winCount;
    }
    public enum PositionSideType {
        LeftSide,
        RightSide,
        BackSide,
        FrontSide
    }
    public enum MovePattern {
        Normal,
        League_2ndRound,
        League_3rdRound,
        Tournament_UpDownSide,
        Tournament_FinalRound_UpSideOnly,
        Tournament_FinalRound_DownSideOnly,
        Tournament_FinalRound_UpSide,
        Tournament_FinalRound_DownSide,
        LineUp,
        Addmission
    }
    public enum RoundType {
        Round_1st,
        Round_2nd,
        LoserBattle,
        Round_Final,
        None
    }
    public enum State {
        NONE,
        IN_PLAY,
        ROUND_END_WAIT,
        ROUND_START_WAIT,
        RESULT
    }
    private readonly int NEED_WINNER_COUNT = 2;
    private readonly int NEED_WINNER_COUNT_SINGLE = 1;
    private readonly int GAME_ROUND_NUM = 1;
    private readonly float ROUND_END_TO_NEXT_ROUND_TIME = 0.1f;
    private readonly int GAME_ROUND_NUM_LEAGUE = 3;
    private readonly int GAME_ROUND_NUM_TOURNAMENT = 4;
    [SerializeField]
    [Header("勝敗：リザルト")]
    private WinOrLoseResultManager winOrLoseResult;
    [SerializeField]
    [Header("順位：リザルト")]
    private RankingResultManager rankingResult;
    [SerializeField]
    [Header("フェ\u30fcド")]
    private SpriteRenderer fade;
    [SerializeField]
    [Header("教師キャラ")]
    private CharacterStyle teacher;
    private int GAME_KICK_CNT = 5;
    private TeamData[] currentTeamData = new TeamData[4];
    private int[] winnerCountList;
    private int maxRoundCnt;
    private int currentRoundCnt;
    private bool isLeagueUIAnimation;
    private bool isTournamentUIAnimation;
    private float currentStateWaitTime;
    private bool isPlayerBattleMode;
    private string[] leagueVSTeamData = new string[3];
    private string[] tournamentVSTeamData = new string[2];
    private List<int> randomTeamNoList = new List<int>();
    private RoundType currentTournamentType;
    private State currentState;
    private int nowKickerPlayer;
    private int nowKeeperPlayer;
    private int turnCnt;
    private bool isAllocReverse;
    private int[] playerTeamAssignment;
    private int[] cpuTeamAssignment;
    private int playerAllocIdxLeftTeam;
    private int playerAllocIdxRightTeam;
    private List<int> teamNoList;
    public int TurnCnt => turnCnt;
    public int GameKickCnt => GAME_KICK_CNT;
    public int NowKickerPlayer => nowKickerPlayer;
    public int NowKeeperPlayer => nowKeeperPlayer;
    public bool IsAllocReverse => isAllocReverse;
    public int RoundCount => currentRoundCnt;
    public State CurrentState => currentState;
    public void Init() {
        nowKickerPlayer = 0;
        nowKeeperPlayer = 4;
        playerAllocIdxLeftTeam = 0;
        playerAllocIdxRightTeam = 0;
        playerTeamAssignment = new int[SingletonCustom<GameSettingManager>.Instance.PlayerNum];
        for (int i = 0; i < playerTeamAssignment.Length; i++) {
            playerTeamAssignment[i] = i;
        }
        System.Random random = new System.Random();
        int num = playerTeamAssignment.Length;
        while (num > 1) {
            num--;
            int num2 = random.Next(num + 1);
            int num3 = playerTeamAssignment[num2];
            playerTeamAssignment[num2] = playerTeamAssignment[num];
            playerTeamAssignment[num] = num3;
        }
        cpuTeamAssignment = new int[SingletonCustom<GameSettingManager>.Instance.PlayerNum];
        for (int j = 0; j < cpuTeamAssignment.Length; j++) {
            cpuTeamAssignment[j] = 4 + j;
        }
        System.Random random2 = new System.Random();
        int num4 = cpuTeamAssignment.Length;
        while (num4 > 1) {
            num4--;
            int num5 = random2.Next(num4 + 1);
            int num6 = cpuTeamAssignment[num5];
            cpuTeamAssignment[num5] = cpuTeamAssignment[num4];
            cpuTeamAssignment[num4] = num6;
        }
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        winnerCountList = new int[SingletonCustom<GameSettingManager>.Instance.TeamNum];
        if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 4 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            maxRoundCnt = GAME_ROUND_NUM_TOURNAMENT;
            GAME_KICK_CNT = 3;
        } else if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 3 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            maxRoundCnt = GAME_ROUND_NUM_LEAGUE;
            GAME_KICK_CNT = 3;
        } else {
            maxRoundCnt = GAME_ROUND_NUM;
        }
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
            isAllocReverse = false;
        } else {
            isAllocReverse = (UnityEngine.Random.Range(0, 2) == 0);
        }
        if (teamNoList == null) {
            teamNoList = new List<int>();
        }
        teamNoList.Clear();
        for (int k = 0; k < 4; k++) {
            teamNoList.Add(k);
        }
        teamNoList.Shuffle();
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
            for (int l = 0; l < SingletonCustom<GameSettingManager>.Instance.TeamNum; l++) {
                randomTeamNoList.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[l][0]);
            }
            InitialSettingTeamData(PositionSideType.LeftSide);
            InitialSettingTeamData(PositionSideType.RightSide);
            if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 4) {
                InitialSettingTeamData(PositionSideType.LeftSide);
                InitialSettingTeamData(PositionSideType.RightSide);
                InitialSettingTeamData(PositionSideType.BackSide);
                InitialSettingTeamData(PositionSideType.FrontSide);
                tournamentVSTeamData[0] = (currentTeamData[0].teamNo + 1).ToString() + "-" + (currentTeamData[1].teamNo + 1).ToString();
                tournamentVSTeamData[1] = (currentTeamData[2].teamNo + 1).ToString() + "-" + (currentTeamData[3].teamNo + 1).ToString();
                SingletonCustom<MS_UIManager>.Instance.SetTournamentTeamData();
                UnityEngine.Debug.Log("ト\u30fcナメントデ\u30fcタの設定[0]:" + tournamentVSTeamData[0] + " [1]:" + tournamentVSTeamData[1]);
            }
        } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            isPlayerBattleMode = true;
            for (int m = 0; m < SingletonCustom<GameSettingManager>.Instance.TeamNum; m++) {
                randomTeamNoList.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[m][0]);
            }
            randomTeamNoList.Shuffle();
            if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 2) {
                InitialSettingTeamData(PositionSideType.LeftSide);
                InitialSettingTeamData(PositionSideType.RightSide);
            } else if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 3) {
                InitialSettingTeamData(PositionSideType.LeftSide);
                InitialSettingTeamData(PositionSideType.RightSide);
                InitialSettingTeamData(PositionSideType.BackSide);
                leagueVSTeamData[0] = (currentTeamData[0].teamNo + 1).ToString() + "-" + (currentTeamData[1].teamNo + 1).ToString();
                leagueVSTeamData[1] = (currentTeamData[2].teamNo + 1).ToString() + "-" + (currentTeamData[0].teamNo + 1).ToString();
                leagueVSTeamData[2] = (currentTeamData[2].teamNo + 1).ToString() + "-" + (currentTeamData[1].teamNo + 1).ToString();
                SingletonCustom<MS_UIManager>.Instance.SetLeagueTeamData();
            } else if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 4) {
                InitialSettingTeamData(PositionSideType.LeftSide);
                InitialSettingTeamData(PositionSideType.RightSide);
                InitialSettingTeamData(PositionSideType.BackSide);
                InitialSettingTeamData(PositionSideType.FrontSide);
                tournamentVSTeamData[0] = (currentTeamData[0].teamNo + 1).ToString() + "-" + (currentTeamData[1].teamNo + 1).ToString();
                tournamentVSTeamData[1] = (currentTeamData[2].teamNo + 1).ToString() + "-" + (currentTeamData[3].teamNo + 1).ToString();
                SingletonCustom<MS_UIManager>.Instance.SetTournamentTeamData();
            }
        }
        AllocPlayer(turnCnt % 2 == 0);
        SingletonCustom<MS_UIManager>.Instance.Init();
        teacher.SetTeacherStyle(CharacterStyle.TeacherType.HIDEYOSHI);
    }
    public void StartGame() {
        currentState = State.IN_PLAY;
    }
    public string[] GetLeagueVSTeamData() {
        return leagueVSTeamData;
    }
    public string[] GetTournamentVSTeamData() {
        return tournamentVSTeamData;
    }
    private void InitialSettingTeamData(PositionSideType _positionSide) {
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
            switch (_positionSide) {
                case PositionSideType.LeftSide:
                    currentTeamData[(int)_positionSide].teamNo = 0;
                    currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
                    {
                    randomTeamNoList[0]
                };
                    break;
                case PositionSideType.RightSide:
                    currentTeamData[(int)_positionSide].teamNo = 1;
                    currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
                    {
                    randomTeamNoList[1]
                };
                    break;
                case PositionSideType.BackSide:
                    currentTeamData[(int)_positionSide].teamNo = 2;
                    currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
                    {
                    randomTeamNoList[2]
                };
                    break;
                case PositionSideType.FrontSide:
                    currentTeamData[(int)_positionSide].teamNo = 3;
                    currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
                    {
                    randomTeamNoList[3]
                };
                    break;
            }
        } else {
            if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat != 0) {
                return;
            }
            if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 2) {
                switch (_positionSide) {
                    case PositionSideType.LeftSide:
                        currentTeamData[(int)_positionSide].teamNo = 0;
                        currentTeamData[(int)_positionSide].memberPlayerNoList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0];
                        break;
                    case PositionSideType.RightSide:
                        currentTeamData[(int)_positionSide].teamNo = 1;
                        currentTeamData[(int)_positionSide].memberPlayerNoList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1];
                        break;
                }
                return;
            }
            switch (_positionSide) {
                case PositionSideType.LeftSide:
                    currentTeamData[(int)_positionSide].teamNo = teamNoList[0];
                    currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
                    {
                    randomTeamNoList[teamNoList[0]]
                };
                    break;
                case PositionSideType.RightSide:
                    currentTeamData[(int)_positionSide].teamNo = teamNoList[1];
                    currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
                    {
                    randomTeamNoList[teamNoList[1]]
                };
                    break;
                case PositionSideType.BackSide:
                    currentTeamData[(int)_positionSide].teamNo = teamNoList[2];
                    currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
                    {
                    randomTeamNoList[teamNoList[2]]
                };
                    break;
                case PositionSideType.FrontSide:
                    currentTeamData[(int)_positionSide].teamNo = teamNoList[3];
                    currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
                    {
                    randomTeamNoList[teamNoList[3]]
                };
                    break;
            }
        }
    }
    public TeamData GetTeamData(PositionSideType _type) {
        return currentTeamData[(int)_type];
    }
    public TeamData GetTeamData(int _teamNo) {
        for (int i = 0; i < currentTeamData.Length; i++) {
            if (currentTeamData[i].teamNo == _teamNo) {
                return currentTeamData[i];
            }
        }
        return currentTeamData[0];
    }
    public void ChangeTeamData(MovePattern _pattern) {
        switch (_pattern) {
            case MovePattern.League_2ndRound: {
                    TeamData teamData = GetTeamData(PositionSideType.LeftSide);
                    TeamData teamData2 = GetTeamData(PositionSideType.RightSide);
                    SetTeamData(PositionSideType.LeftSide, GetTeamData(PositionSideType.BackSide));
                    SetTeamData(PositionSideType.RightSide, teamData);
                    SetTeamData(PositionSideType.FrontSide, teamData2);
                    SetTeamData(PositionSideType.BackSide, default(TeamData));
                    break;
                }
            case MovePattern.League_3rdRound: {
                    TeamData teamData = GetTeamData(PositionSideType.RightSide);
                    SetTeamData(PositionSideType.RightSide, GetTeamData(PositionSideType.FrontSide));
                    SetTeamData(PositionSideType.FrontSide, teamData);
                    break;
                }
            case MovePattern.Tournament_UpDownSide: {
                    TeamData teamData = GetTeamData(PositionSideType.RightSide);
                    TeamData teamData2 = GetTeamData(PositionSideType.LeftSide);
                    SetTeamData(PositionSideType.RightSide, GetTeamData(PositionSideType.FrontSide));
                    SetTeamData(PositionSideType.FrontSide, teamData);
                    SetTeamData(PositionSideType.LeftSide, GetTeamData(PositionSideType.BackSide));
                    SetTeamData(PositionSideType.BackSide, teamData2);
                    break;
                }
            case MovePattern.Tournament_FinalRound_UpSideOnly: {
                    TeamData teamData = GetTeamData(PositionSideType.LeftSide);
                    SetTeamData(PositionSideType.LeftSide, GetTeamData(PositionSideType.BackSide));
                    SetTeamData(PositionSideType.BackSide, teamData);
                    break;
                }
            case MovePattern.Tournament_FinalRound_DownSideOnly: {
                    TeamData teamData = GetTeamData(PositionSideType.RightSide);
                    SetTeamData(PositionSideType.RightSide, GetTeamData(PositionSideType.FrontSide));
                    SetTeamData(PositionSideType.FrontSide, teamData);
                    break;
                }
            case MovePattern.Tournament_FinalRound_UpSide: {
                    TeamData teamData = GetTeamData(PositionSideType.RightSide);
                    TeamData teamData2 = GetTeamData(PositionSideType.FrontSide);
                    TeamData teamData3 = GetTeamData(PositionSideType.BackSide);
                    SetTeamData(PositionSideType.RightSide, GetTeamData(PositionSideType.LeftSide));
                    SetTeamData(PositionSideType.FrontSide, teamData);
                    SetTeamData(PositionSideType.BackSide, teamData2);
                    SetTeamData(PositionSideType.LeftSide, teamData3);
                    break;
                }
            case MovePattern.Tournament_FinalRound_DownSide: {
                    TeamData teamData = GetTeamData(PositionSideType.BackSide);
                    TeamData teamData2 = GetTeamData(PositionSideType.FrontSide);
                    TeamData teamData3 = GetTeamData(PositionSideType.RightSide);
                    SetTeamData(PositionSideType.BackSide, GetTeamData(PositionSideType.LeftSide));
                    SetTeamData(PositionSideType.FrontSide, teamData);
                    SetTeamData(PositionSideType.RightSide, teamData2);
                    SetTeamData(PositionSideType.LeftSide, teamData3);
                    break;
                }
        }
    }
    public void SetTeamData(PositionSideType _positionSide, TeamData _teamData) {
        currentTeamData[(int)_positionSide].teamNo = _teamData.teamNo;
        currentTeamData[(int)_positionSide].memberPlayerNoList = _teamData.memberPlayerNoList;
        currentTeamData[(int)_positionSide].winCount = _teamData.winCount;
    }
    private void AllocPlayer(bool _isTop) {
        switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
            case 2:
                switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
                    case GS_Define.GameFormat.BATTLE:
                        if (_isTop) {
                            nowKickerPlayer = currentTeamData[0].memberPlayerNoList[0];
                            nowKeeperPlayer = currentTeamData[1].memberPlayerNoList[0];
                        } else {
                            nowKickerPlayer = currentTeamData[1].memberPlayerNoList[0];
                            nowKeeperPlayer = currentTeamData[0].memberPlayerNoList[0];
                        }
                        break;
                    case GS_Define.GameFormat.COOP:
                        if (_isTop) {
                            nowKickerPlayer = playerTeamAssignment[playerAllocIdxLeftTeam];
                            nowKeeperPlayer = cpuTeamAssignment[playerAllocIdxRightTeam];
                        } else {
                            nowKickerPlayer = cpuTeamAssignment[playerAllocIdxRightTeam];
                            nowKeeperPlayer = playerTeamAssignment[playerAllocIdxLeftTeam];
                        }
                        break;
                }
                break;
            case 3:
                if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
                    if (_isTop) {
                        nowKickerPlayer = playerTeamAssignment[playerAllocIdxLeftTeam];
                        nowKeeperPlayer = cpuTeamAssignment[playerAllocIdxRightTeam];
                    } else {
                        nowKickerPlayer = cpuTeamAssignment[playerAllocIdxRightTeam];
                        nowKeeperPlayer = playerTeamAssignment[playerAllocIdxLeftTeam];
                    }
                } else if (_isTop) {
                    nowKickerPlayer = currentTeamData[0].memberPlayerNoList[0];
                    nowKeeperPlayer = currentTeamData[1].memberPlayerNoList[0];
                } else {
                    nowKickerPlayer = currentTeamData[1].memberPlayerNoList[0];
                    nowKeeperPlayer = currentTeamData[0].memberPlayerNoList[0];
                }
                break;
            case 1:
            case 4:
                if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
                    if (_isTop) {
                        nowKickerPlayer = playerTeamAssignment[playerAllocIdxLeftTeam];
                        nowKeeperPlayer = cpuTeamAssignment[playerAllocIdxRightTeam];
                    } else {
                        nowKickerPlayer = cpuTeamAssignment[playerAllocIdxRightTeam];
                        nowKeeperPlayer = playerTeamAssignment[playerAllocIdxLeftTeam];
                    }
                } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
                    if (_isTop) {
                        nowKickerPlayer = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][playerAllocIdxLeftTeam];
                        nowKeeperPlayer = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1][playerAllocIdxRightTeam];
                    } else {
                        nowKickerPlayer = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1][playerAllocIdxRightTeam];
                        nowKeeperPlayer = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][playerAllocIdxLeftTeam];
                    }
                } else if (_isTop) {
                    nowKickerPlayer = currentTeamData[0].memberPlayerNoList[0];
                    nowKeeperPlayer = currentTeamData[1].memberPlayerNoList[0];
                } else {
                    nowKickerPlayer = currentTeamData[1].memberPlayerNoList[0];
                    nowKeeperPlayer = currentTeamData[0].memberPlayerNoList[0];
                }
                break;
        }
    }
    public void OnKickEnd() {
    }
    public void GameRoundEnd(bool _winnerTeamIsLeft) {
        RoundEnd(_winnerTeamIsLeft);
        if (_winnerTeamIsLeft) {
            UnityEngine.Debug.Log("【勝利team】" + currentTeamData[0].teamNo.ToString());
            winnerCountList[currentTeamData[0].teamNo]++;
            if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE && !SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
                if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 2) {
                    SingletonCustom<MS_UIManager>.Instance.StartSetAnimation(_isLeftTeam: true);
                } else if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 3) {
                    SingletonCustom<MS_UIManager>.Instance.SetLeagueBattleResultData(_isWinnerLeftTeam: true);
                } else if (SingletonCustom<GameSettingManager>.Instance.TeamNum != 4) {
                }
            }
        } else {
            UnityEngine.Debug.Log("【勝利team】" + currentTeamData[1].teamNo.ToString());
            winnerCountList[currentTeamData[1].teamNo]++;
            if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE && !SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
                if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 2) {
                    SingletonCustom<MS_UIManager>.Instance.StartSetAnimation(_isLeftTeam: false);
                } else if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 3) {
                    SingletonCustom<MS_UIManager>.Instance.SetLeagueBattleResultData(_isWinnerLeftTeam: false);
                } else {
                    int teamNum = SingletonCustom<GameSettingManager>.Instance.TeamNum;
                }
            }
        }
        currentRoundCnt++;
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 3) {
                if (IsVictoryTeamAppear()) {
                    isLeagueUIAnimation = true;
                }
                SingletonCustom<MS_UIManager>.Instance.StartLeagueUIAnimation(currentRoundCnt, _winnerTeamIsLeft, IsVictoryTeamAppear(), delegate {
                    isLeagueUIAnimation = false;
                });
            } else if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 4) {
                if (IsVictoryTeamAppear()) {
                    isTournamentUIAnimation = true;
                }
                SingletonCustom<MS_UIManager>.Instance.StartTournamentUIAnimation(_winnerTeamIsLeft, delegate {
                    isTournamentUIAnimation = false;
                });
            }
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
        currentState = State.ROUND_END_WAIT;
    }
    public void RoundEnd(bool _isWinnerTeamIsLeft) {
        if (_isWinnerTeamIsLeft) {
            UnityEngine.Random.Range(0, 2);
        }
    }
    private void ToResult() {
        ResultGameDataParams.SetPoint();
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
            winOrLoseResult.ShowResult((GetVictoryTeamNo() != 0) ? ResultGameDataParams.ResultType.Lose : ResultGameDataParams.ResultType.Win, 0);
            return;
        }
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
            winOrLoseResult.ShowResult(ResultGameDataParams.ResultType.Win, GetVictoryTeamNo());
            return;
        }
        switch (SingletonCustom<GameSettingManager>.Instance.TeamNum) {
            case 2:
                winOrLoseResult.ShowResult((GetVictoryTeamNo() == -1) ? ResultGameDataParams.ResultType.Draw : ResultGameDataParams.ResultType.Win, GetVictoryTeamNo());
                break;
            case 3: {
                    int[] array = new int[3]
                    {
                0,
                1,
                2
                    };
                    ResultGameDataParams.SetRecord_Int(winnerCountList, array);
                    rankingResult.ShowResult_Score();
                    break;
                }
            case 4: {
                    int[] array = new int[4]
                    {
                0,
                1,
                2,
                3
                    };
                    for (int i = 0; i < array.Length; i++) {
                        for (int j = 0; j < currentTeamData.Length; j++) {
                            if (array[i] == currentTeamData[j].teamNo) {
                                array[i] = currentTeamData[j].memberPlayerNoList[0];
                                break;
                            }
                        }
                    }
                    int[] array2 = new int[4];
                    int[] tournamentWinnerTeamNoList = SingletonCustom<MS_UIManager>.Instance.GetTournamentWinnerTeamNoList();
                    for (int k = 0; k < tournamentWinnerTeamNoList.Length; k++) {
                        if (k < 2) {
                            array2[tournamentWinnerTeamNoList[k]] += 2;
                        } else {
                            array2[tournamentWinnerTeamNoList[k]]++;
                        }
                    }
                    List<string> list = new List<string>();
                    string[] array3 = tournamentVSTeamData[0].Split('-');
                    int no = int.Parse(array3[0]) - 1;
                    int no2 = int.Parse(array3[1]) - 1;
                    list.Add((GetPlayerNoAtTeamNoAffiliation(no) + 1).ToString() + "-" + (GetPlayerNoAtTeamNoAffiliation(no2) + 1).ToString());
                    string[] array4 = tournamentVSTeamData[1].Split('-');
                    no = int.Parse(array4[0]) - 1;
                    no2 = int.Parse(array4[1]) - 1;
                    list.Add((GetPlayerNoAtTeamNoAffiliation(no) + 1).ToString() + "-" + (GetPlayerNoAtTeamNoAffiliation(no2) + 1).ToString());
                    int[] tournamentWinnerTeamNoList2 = SingletonCustom<MS_UIManager>.Instance.GetTournamentWinnerTeamNoList();
                    int[] array5 = new int[tournamentWinnerTeamNoList2.Length];
                    for (int l = 0; l < array5.Length; l++) {
                        array5[l] = GetPlayerNoAtTeamNoAffiliation(tournamentWinnerTeamNoList2[l]);
                    }
                    ResultGameDataParams.SetRecord_Int_Tournament(array2, array, array5, list.ToArray());
                    rankingResult.ShowResult_Tournament();
                    break;
                }
        }
    }
    public int GetPlayerNoAtTeamNoAffiliation(int _no) {
        for (int i = 0; i < currentTeamData.Length; i++) {
            if (currentTeamData[i].teamNo == _no) {
                return SingletonCustom<GameSettingManager>.Instance.GetPlayerAffiliationTeam(currentTeamData[i].memberPlayerNoList[0]);
            }
        }
        return 0;
    }
    private bool IsVictoryTeamAppear() {
        UnityEngine.Debug.Log("current:" + currentRoundCnt.ToString() + " max:" + maxRoundCnt.ToString());
        if (currentRoundCnt == maxRoundCnt) {
            return true;
        }
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            int playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
            return false;
        }
        return false;
    }
    private void NextRound() {
        turnCnt = -1;
        WaitAfterExec(3.25f, delegate {
            NextKick(_isNextRound: true);
        });
    }
    private void NextKick(bool _isNextRound = false, bool _isExtra = false) {
        Fade(1.5f, 0f, delegate {
            if (turnCnt % 2 == 1 && !SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
                if (!isAllocReverse) {
                    playerAllocIdxLeftTeam = (playerAllocIdxLeftTeam + 1) % SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0].Count;
                    playerAllocIdxRightTeam = (playerAllocIdxRightTeam + 1) % SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1].Count;
                } else {
                    playerAllocIdxLeftTeam = (playerAllocIdxLeftTeam + 1) % SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1].Count;
                    playerAllocIdxRightTeam = (playerAllocIdxRightTeam + 1) % SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0].Count;
                }
                int teamNum = SingletonCustom<GameSettingManager>.Instance.TeamNum;
            }
            turnCnt++;
            AllocPlayer(turnCnt % 2 == 0);
        });
    }
    private int GetVictoryTeamNo() {
        int num = -1;
        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.TeamNum; i++) {
            if (winnerCountList[i] == ((SingletonCustom<GameSettingManager>.Instance.TeamNum == 2) ? NEED_WINNER_COUNT_SINGLE : NEED_WINNER_COUNT)) {
                return i;
            }
            if (i != 0 && winnerCountList[i - 1] < winnerCountList[i]) {
                num = i;
            }
        }
        if (num == -1) {
            UnityEngine.Debug.Log("引き分け！");
        } else {
            UnityEngine.Debug.Log("優勝組は[" + num.ToString() + "]番");
        }
        return num;
    }
    private void Update() {
        State state = currentState;
        if (state != State.IN_PLAY && state == State.ROUND_END_WAIT) {
            GameState_RoundEndWait();
        }
    }
    private void GameState_RoundEndWait() {
        if (currentStateWaitTime > ROUND_END_TO_NEXT_ROUND_TIME && !isLeagueUIAnimation && !isTournamentUIAnimation) {
            currentStateWaitTime = 0f;
            if (IsVictoryTeamAppear()) {
                SingletonCustom<CommonEndSimple>.Instance.Show(ToResult);
                currentState = State.RESULT;
            } else {
                CharacterMove();
                currentState = State.ROUND_START_WAIT;
                NextRound();
            }
        } else {
            currentStateWaitTime += Time.deltaTime;
        }
    }
    private void CharacterMove() {
        if (SingletonCustom<GameSettingManager>.Instance.TeamNum <= 2) {
            return;
        }
        if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 3) {
            if (currentRoundCnt == 1) {
                ChangeTeamData(MovePattern.League_2ndRound);
            } else if (currentRoundCnt == 2) {
                ChangeTeamData(MovePattern.League_3rdRound);
            }
        } else {
            if (SingletonCustom<GameSettingManager>.Instance.TeamNum != 4) {
                return;
            }
            if (currentTournamentType == RoundType.Round_1st) {
                currentTournamentType = RoundType.Round_2nd;
            } else if (currentTournamentType == RoundType.Round_2nd) {
                currentTournamentType = RoundType.LoserBattle;
            } else if (currentTournamentType == RoundType.LoserBattle) {
                currentTournamentType = RoundType.Round_Final;
            } else if (currentTournamentType == RoundType.Round_Final) {
                currentTournamentType = RoundType.None;
            }
            switch (currentTournamentType) {
                case RoundType.Round_1st:
                    currentTournamentType = RoundType.Round_2nd;
                    ChangeTeamData(MovePattern.Tournament_UpDownSide);
                    break;
                case RoundType.Round_2nd:
                    ChangeTeamData(MovePattern.Tournament_UpDownSide);
                    break;
                case RoundType.LoserBattle: {
                        int[] tournamentLoserBattleTeamNoArray = SingletonCustom<MS_UIManager>.Instance.GetTournamentLoserBattleTeamNoArray();
                        MovePattern pattern2 = MovePattern.Addmission;
                        switch (GetTeamPositionType(tournamentLoserBattleTeamNoArray[0])) {
                            case PositionSideType.LeftSide:
                                switch (GetTeamPositionType(tournamentLoserBattleTeamNoArray[1])) {
                                    case PositionSideType.BackSide:
                                        pattern2 = MovePattern.Tournament_FinalRound_UpSide;
                                        break;
                                    case PositionSideType.FrontSide:
                                        pattern2 = MovePattern.Tournament_FinalRound_DownSideOnly;
                                        break;
                                }
                                break;
                            case PositionSideType.RightSide:
                                switch (GetTeamPositionType(tournamentLoserBattleTeamNoArray[1])) {
                                    case PositionSideType.BackSide:
                                        pattern2 = MovePattern.Tournament_FinalRound_UpSideOnly;
                                        break;
                                    case PositionSideType.FrontSide:
                                        pattern2 = MovePattern.Tournament_FinalRound_DownSide;
                                        break;
                                }
                                break;
                            case PositionSideType.BackSide:
                                switch (GetTeamPositionType(tournamentLoserBattleTeamNoArray[1])) {
                                    case PositionSideType.LeftSide:
                                        pattern2 = MovePattern.Tournament_FinalRound_UpSide;
                                        break;
                                    case PositionSideType.RightSide:
                                        pattern2 = MovePattern.Tournament_FinalRound_UpSideOnly;
                                        break;
                                }
                                break;
                            case PositionSideType.FrontSide:
                                switch (GetTeamPositionType(tournamentLoserBattleTeamNoArray[1])) {
                                    case PositionSideType.LeftSide:
                                        pattern2 = MovePattern.Tournament_FinalRound_DownSideOnly;
                                        break;
                                    case PositionSideType.RightSide:
                                        pattern2 = MovePattern.Tournament_FinalRound_DownSide;
                                        break;
                                }
                                break;
                        }
                        ChangeTeamData(pattern2);
                        break;
                    }
                case RoundType.Round_Final: {
                        int[] tournamentFinalRoundTeamNoArray = SingletonCustom<MS_UIManager>.Instance.GetTournamentFinalRoundTeamNoArray();
                        MovePattern pattern = MovePattern.Addmission;
                        switch (GetTeamPositionType(tournamentFinalRoundTeamNoArray[0])) {
                            case PositionSideType.LeftSide:
                                switch (GetTeamPositionType(tournamentFinalRoundTeamNoArray[1])) {
                                    case PositionSideType.BackSide:
                                        pattern = MovePattern.Tournament_FinalRound_UpSide;
                                        break;
                                    case PositionSideType.FrontSide:
                                        pattern = MovePattern.Tournament_FinalRound_DownSideOnly;
                                        break;
                                }
                                break;
                            case PositionSideType.RightSide:
                                switch (GetTeamPositionType(tournamentFinalRoundTeamNoArray[1])) {
                                    case PositionSideType.BackSide:
                                        pattern = MovePattern.Tournament_FinalRound_UpSideOnly;
                                        break;
                                    case PositionSideType.FrontSide:
                                        pattern = MovePattern.Tournament_FinalRound_DownSide;
                                        break;
                                }
                                break;
                            case PositionSideType.BackSide:
                                switch (GetTeamPositionType(tournamentFinalRoundTeamNoArray[1])) {
                                    case PositionSideType.LeftSide:
                                        pattern = MovePattern.Tournament_FinalRound_UpSide;
                                        break;
                                    case PositionSideType.RightSide:
                                        pattern = MovePattern.Tournament_FinalRound_UpSideOnly;
                                        break;
                                    case PositionSideType.FrontSide:
                                        pattern = MovePattern.Tournament_UpDownSide;
                                        break;
                                }
                                break;
                            case PositionSideType.FrontSide:
                                switch (GetTeamPositionType(tournamentFinalRoundTeamNoArray[1])) {
                                    case PositionSideType.LeftSide:
                                        pattern = MovePattern.Tournament_FinalRound_DownSideOnly;
                                        break;
                                    case PositionSideType.RightSide:
                                        pattern = MovePattern.Tournament_FinalRound_DownSide;
                                        break;
                                    case PositionSideType.BackSide:
                                        pattern = MovePattern.Tournament_UpDownSide;
                                        break;
                                }
                                break;
                        }
                        ChangeTeamData(pattern);
                        break;
                    }
            }
        }
    }
    public PositionSideType GetTeamPositionType(int _teamNo) {
        if (currentTeamData[0].teamNo == _teamNo) {
            return PositionSideType.LeftSide;
        }
        if (currentTeamData[1].teamNo == _teamNo) {
            return PositionSideType.RightSide;
        }
        if (currentTeamData[2].teamNo == _teamNo) {
            return PositionSideType.BackSide;
        }
        if (currentTeamData[3].teamNo == _teamNo) {
            return PositionSideType.FrontSide;
        }
        UnityEngine.Debug.LogError("<color=yellow>団番号を含まない番号です！</color>" + _teamNo.ToString());
        return PositionSideType.LeftSide;
    }
    public void Fade(float _time, float _delay, Action _act) {
        Color color = fade.color;
        color.a = 1f;
        fade.SetAlpha(0f);
        fade.gameObject.SetActive(value: true);
        LeanTween.value(fade.gameObject, 0f, 1f, _time * 0.5f).setDelay(_delay).setEaseOutCubic()
            .setOnUpdate(delegate (float _value) {
                fade.SetAlpha(_value);
            })
            .setOnComplete((Action)delegate {
                _act();
            });
        color.a = 0f;
        LeanTween.value(fade.gameObject, 1f, 0f, _time * 0.5f).setDelay(_time * 0.5f + _delay).setEaseOutCubic()
            .setOnUpdate(delegate (float _value) {
                fade.SetAlpha(_value);
            })
            .setOnComplete((Action)delegate {
                fade.gameObject.SetActive(fade);
            });
    }
}
