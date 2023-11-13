using GamepadInput;
using System;
using System.Collections.Generic;
using UnityEngine;
public class SportsDayPoint : SingletonCustom<SportsDayPoint> {
    [Serializable]
    private class TeamData {
        public int teamNo;
        public int score;
        public TeamData(int _teamNo, int _score) {
            teamNo = _teamNo;
            score = _score;
        }
    }
    [SerializeField]
    [Header("表示ル\u30fcト")]
    private GameObject objRoot;
    [SerializeField]
    [Header("拡縮ル\u30fcト")]
    private GameObject scaleRoot;
    [SerializeField]
    [Header("２チ\u30fcム時のレイアウト")]
    private GameObject[] arrayTeamLayout_2;
    [SerializeField]
    [Header("４チ\u30fcム時のレイアウト")]
    private GameObject[] arrayTeamLayout_4;
    [SerializeField]
    [Header("ランキングUIデ\u30fcタ(２チ\u30fcム用)")]
    private PlayKingPointUIData[] rankingUIData_2Team;
    [SerializeField]
    [Header("ランキングUIデ\u30fcタ(４チ\u30fcム用)")]
    private PlayKingPointUIData[] rankingUIData_4Team;
    [SerializeField]
    [Header("後半スコア隠し表示")]
    private GameObject objLatterHalfCation;
    private bool isShow;
    private bool showTeam2Layout;
    private bool showTeam4Layout;
    private int calcRank;
    private List<TeamData> teamDataList;
    private const int SHOW_STICKER_NUM = 5;
    private bool isLatterHalfCaptionHide;
    private ResultGameDataParams.PlayKingRankingData playKingRankingData;
    private ResultGameDataParams.PlayKingRankingData[] playKingRankingDatas_Debug;
    public bool IsShow => isShow;
    private void Update() {
        if (!SingletonCustom<CommonNotificationManager>.Instance.IsOpen && isShow) {
            if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B)) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
                Hide();
            }
            if (objLatterHalfCation.activeSelf && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X)) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                HideLatterHalfCation();
            }
        }
    }
    private new void OnDestroy() {
        base.OnDestroy();
        LeanTween.cancel(scaleRoot);
    }
    public void Show() {
        isShow = true;
        objRoot.SetActive(value: true);
        LeanTween.cancel(scaleRoot);
        scaleRoot.transform.SetLocalScale(0.001f, 0.001f, 1f);
        LeanTween.scale(scaleRoot, Vector3.one, 0.35f).setEaseOutExpo().setIgnoreTimeScale(useUnScaledTime: true);
        SetLayout();
    }
    public void Hide() {
        isShow = false;
        objRoot.SetActive(value: false);
    }
    public void HideLatterHalfCation() {
        objLatterHalfCation.SetActive(value: false);
        isLatterHalfCaptionHide = true;
    }
    private void SetLayout() {
        if (!isLatterHalfCaptionHide) {
            objLatterHalfCation.SetActive(SingletonCustom<GameSettingManager>.Instance.PlayKingTableCnt > 5);
        }
        if (teamDataList == null) {
            teamDataList = new List<TeamData>();
        }
        teamDataList.Clear();
        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.TeamNum; i++) {
            teamDataList.Add(new TeamData(i, SingletonCustom<GameSettingManager>.Instance.GetAllGameHalfTotalTeamScore(i)));
        }
        teamDataList.Sort((TeamData a, TeamData b) => b.score - a.score);
        showTeam2Layout = false;
        showTeam4Layout = true;
        for (int j = 0; j < arrayTeamLayout_2.Length; j++) {
            arrayTeamLayout_2[j].SetActive(value: false);
        }
        for (int k = 0; k < arrayTeamLayout_4.Length; k++) {
            arrayTeamLayout_4[k].SetActive(value: true);
        }
        calcRank = 0;
        int num = 0;
        playKingRankingDatas_Debug = ResultGameDataParams.GetAllPlayKingRankingData();
        for (int l = 0; l < teamDataList.Count; l++) {
            if (l > 0 && teamDataList[l].score < teamDataList[l - 1].score) {
                calcRank += num;
                calcRank++;
                num = 0;
            } else if (l > 0) {
                num++;
            }
            rankingUIData_4Team[l].SetRankData(calcRank);
            rankingUIData_4Team[l].SetCharacterIcon(teamDataList[l].teamNo, calcRank);
            rankingUIData_4Team[l].SetPlayerIcon(teamDataList[l].teamNo);
            rankingUIData_4Team[l].SetPoint(teamDataList[l].score);
            playKingRankingData = ResultGameDataParams.GetPlayKingRankingData(teamDataList[l].teamNo);
            for (int m = 0; m < 5 && playKingRankingData.rankNoList.Count != m; m++) {
                UnityEngine.Debug.Log("チ\u30fcム[" + teamDataList[l].teamNo.ToString() + "]の[" + m.ToString() + "]番目のあそびの順位：" + (playKingRankingData.rankNoList[m] + 1).ToString() + "位");
                rankingUIData_4Team[l].SetSticker(m, playKingRankingData.rankNoList[m]);
            }
        }
    }
}
