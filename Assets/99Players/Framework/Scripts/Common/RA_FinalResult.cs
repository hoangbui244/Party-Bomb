using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class RA_FinalResult : MonoBehaviourExtension {
    [Serializable]
    public struct RankGroupUIData {
        [Header("各組のUIデ\u30fcタ")]
        public RankUIData[] rankUIData;
    }
    [Serializable]
    public struct WinOrLoseResultUIData {
        [Header("キャプションアンカ\u30fc")]
        public Transform captionAnchor;
        [Header("勝利チ\u30fcムの名前画像")]
        public SpriteRenderer[] winnerTeamNameSprite;
        [Header("勝ちの画像")]
        public SpriteRenderer victorySprite;
        [Header("負けの画像")]
        public SpriteRenderer defeatSprite;
        [Header("引き分けの画像")]
        public SpriteRenderer drawSprite;
        [Header("各チ\u30fcムの名前画像")]
        public SpriteRenderer[] teamNameSprite;
        [Header("各チ\u30fcムのポイント数")]
        public SpriteNumbers[] pointNumbers;
        [Header("各チ\u30fcムのポイント文字")]
        public SpriteRenderer[] pointTextSprite;
    }
    [Serializable]
    public struct PlayKingMarkData {
        [Header("あそび王マ\u30fcクのアンカ\u30fc")]
        public GameObject playKingMarkAnchor;
        [Header("あそび王マ\u30fcク")]
        public SpriteRenderer[] playKingMark;
        [Header("キラキラエフェクト")]
        public GlitterSpriteEffect[] glitterSpriteEffects;
    }
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
    [Header("キャプション")]
    private GameObject objCaption;
    [SerializeField]
    [Header("背景フェ\u30fcド用表示")]
    private MeshRenderer meshBackFade;
    [SerializeField]
    [Header("リザルト画面")]
    private GameObject resultWindow;
    [SerializeField]
    [Header("勝敗リザルトレイアウト")]
    private GameObject objWinOrLoseLayout;
    [SerializeField]
    [Header("順位リザルトレイアウト")]
    private GameObject objRankingLayout;
    [SerializeField]
    [Header("勝敗リザルト用デ\u30fcタ")]
    private WinOrLoseResultUIData winOrLoseResultUIData;
    [SerializeField]
    [Header("順位のUIデ\u30fcタ")]
    private RankGroupUIData[] rankGroupData;
    [SerializeField]
    [Header("ボタンUIデ\u30fcタ")]
    private ResultGameDataParams.ButtonUIData buttonUIData;
    [SerializeField]
    [Header("リザルトの写真アンカ\u30fc")]
    private GameObject resultPhotoAnchor;
    [SerializeField]
    [Header("リザルトの写真画像")]
    private SpriteRenderer resultPhoto;
    [SerializeField]
    [Header("演出：クラッカ\u30fc")]
    private ParticleSystem psEffectCracker;
    [SerializeField]
    [Header("演出：紙吹雪")]
    private ParticleSystem psEffectConfetti;
    [SerializeField]
    [Header("Winマ\u30fcク")]
    private SpriteRenderer spWinMark;
    [SerializeField]
    [Header("画面下部装飾ル\u30fcト")]
    private GameObject bottomDecoRoot;
    [SerializeField]
    [Header("順位アイコン画像")]
    private Sprite[] arrayOvalSprite;
    [SerializeField]
    [Header("背景スクロ\u30fcル")]
    private MenuBackScroll backScroll;
    private int[] rankRecordCnt = new int[2];
    private List<ResultGameDataParams.RankingRecord> rankGroupRecords = new List<ResultGameDataParams.RankingRecord>();
    private float resultWindowAnimationTime = 0.6f;
    private float resultBackFanAnimationTime = 0.5f;
    private float resultRankingAnimationTime_Battle_Ranking = 0.2f;
    private float resultRankingAnimationWaitTime_Battle_Ranking = 0.3f;
    private float resultFirstRankPlayerIconAnimationTime_Battle_Ranking = 1f;
    private float resultButtonAnimationTime = 0.3f;
    private bool isAnimation;
    private float defPosY_ResultWindow;
    private float[] defPosX_OperationButton;
    private bool isSingleLose;
    private bool isSingleWin;
    private bool isInit;
    private int calcRank;
    private List<TeamData> teamDataList;
    private float resultPlayKingMarkAnimationTime = 0.5f;
    private void OnDestroy() {
        LeanTween.cancel(winOrLoseResultUIData.captionAnchor.gameObject);
    }
    private void Awake() {
        resultWindow.SetActive(value: false);
        defPosY_ResultWindow = resultWindow.transform.localPosition.y;
        defPosX_OperationButton = new float[buttonUIData.arrayAnimButton.Length];
        for (int i = 0; i < defPosX_OperationButton.Length; i++) {
            defPosX_OperationButton[i] = buttonUIData.arrayAnimButton[i].transform.localPosition.x;
        }
        resultPhotoAnchor.SetActive(value: false);
        if (!SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup) {
            rankGroupData[0].rankUIData[0].transform.SetLocalPosition(-330.7f, -114f, 0f);
            rankGroupData[0].rankUIData[1].transform.SetLocalPosition(-250.6f, -270f, 0f);
            rankGroupData[0].rankUIData[2].transform.SetLocalPosition(225f, 146f, 0f);
            rankGroupData[0].rankUIData[3].transform.SetLocalPosition(306f, -399f, 0f);
            rankGroupData[0].rankUIData[4].gameObject.SetActive(value: false);
            rankGroupData[0].rankUIData[5].gameObject.SetActive(value: false);
        }
    }
    public void Show() {
        base.gameObject.SetActive(value: true);
        SingletonCustom<ResultCharacterManager>.Instance.gameObject.SetActive(value: true);
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_sports_day_final_result_2");
        if (!isInit) {
            SetLayout();
            ShowResult();
            isInit = true;
            if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying("bgm_result_0")) {
                SingletonCustom<AudioManager>.Instance.BgmStop();
                SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_result_0", _loop: true);
            }
        }
    }
    public void Hide() {
        base.gameObject.SetActive(value: false);
    }
    private void SetLayout() {
        if (teamDataList == null) {
            teamDataList = new List<TeamData>();
        }
        teamDataList.Clear();
        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.TeamNum; i++) {
            teamDataList.Add(new TeamData(i, SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(i)));
        }
        teamDataList.Sort((TeamData a, TeamData b) => b.score - a.score);
        calcRank = 0;
        backScroll.DirectSetQuad(teamDataList[0].teamNo);
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            objRankingLayout.SetActive(value: true);
            objWinOrLoseLayout.SetActive(value: false);
        } else {
            objRankingLayout.SetActive(value: false);
            objWinOrLoseLayout.SetActive(value: true);
        }
    }
    public void ShowResult() {
        isSingleLose = false;
        isSingleWin = false;
        spWinMark.SetAlpha(0f);
        spWinMark.transform.SetLocalScale(1.2f, 1.2f, 1.2f);
        if (objWinOrLoseLayout.activeSelf) {
            for (int i = 0; i < defPosX_OperationButton.Length; i++) {
                buttonUIData.arrayAnimButton[i].transform.SetLocalPositionX(defPosX_OperationButton[i] + 2000f);
            }
            winOrLoseResultUIData.victorySprite.gameObject.SetActive(value: false);
            winOrLoseResultUIData.defeatSprite.gameObject.SetActive(value: false);
            winOrLoseResultUIData.drawSprite.gameObject.SetActive(value: false);
            UnityEngine.Debug.Log("番号０：" + teamDataList[0].teamNo.ToString());
            if (teamDataList[0].score == teamDataList[1].score) {
                for (int j = 0; j < winOrLoseResultUIData.winnerTeamNameSprite.Length; j++) {
                    winOrLoseResultUIData.winnerTeamNameSprite[j].gameObject.SetActive(value: false);
                }
                winOrLoseResultUIData.drawSprite.gameObject.SetActive(value: true);
                if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
                    for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerNum; k++) {
                        SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_WinOrLose(ResultCharacterManager.FaceType.Normal, k);
                    }
                } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
                    SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_WinOrLose(ResultCharacterManager.FaceType.Normal, 0);
                } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
                    SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_WinOrLose(ResultCharacterManager.FaceType.Normal, 0);
                    SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_WinOrLose(ResultCharacterManager.FaceType.Normal, 1);
                }
            } else {
                string str = "";
                if (Localize_Define.Language != 0) {
                    str = "en";
                }
                switch (teamDataList[0].teamNo) {
                    case 0:
                        str += "_A_team_l";
                        for (int n = 0; n < winOrLoseResultUIData.winnerTeamNameSprite.Length; n++) {
                            winOrLoseResultUIData.winnerTeamNameSprite[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                        }
                        winOrLoseResultUIData.victorySprite.gameObject.SetActive(value: true);
                        isSingleWin = true;
                        break;
                    case 1:
                        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
                            str += "_A_team_l";
                            for (int l = 0; l < winOrLoseResultUIData.winnerTeamNameSprite.Length; l++) {
                                winOrLoseResultUIData.winnerTeamNameSprite[l].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                            }
                            winOrLoseResultUIData.defeatSprite.gameObject.SetActive(value: true);
                        } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
                            str += "_B_team_l";
                            for (int m = 0; m < winOrLoseResultUIData.winnerTeamNameSprite.Length; m++) {
                                winOrLoseResultUIData.winnerTeamNameSprite[m].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                            }
                            winOrLoseResultUIData.victorySprite.gameObject.SetActive(value: true);
                        }
                        break;
                }
                if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
                    if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1) {
                        if (teamDataList[0].teamNo == 0) {
                            SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_WinOrLose(ResultCharacterManager.FaceType.Happy, 0);
                        } else {
                            SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_WinOrLose(ResultCharacterManager.FaceType.Sad, 0);
                        }
                    } else {
                        SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_WinOrLose(ResultCharacterManager.FaceType.Happy, teamDataList[0].teamNo);
                        SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_WinOrLose(ResultCharacterManager.FaceType.Sad, teamDataList[1].teamNo);
                    }
                } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
                    if (teamDataList[0].teamNo == 0) {
                        SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_WinOrLose(ResultCharacterManager.FaceType.Happy, 0);
                    } else {
                        SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_WinOrLose(ResultCharacterManager.FaceType.Sad, 0);
                    }
                } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
                    SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_WinOrLose(ResultCharacterManager.FaceType.Happy, teamDataList[0].teamNo);
                }
            }
            isAnimation = true;
            LeanTween.cancel(bottomDecoRoot);
            bottomDecoRoot.transform.SetLocalPositionY(-150f);
            StartCoroutine(_ShowResultWinOrLose());
        } else if (objRankingLayout.activeSelf) {
            int[] array = new int[SingletonCustom<GameSettingManager>.Instance.TeamNum];
            int[] array2 = new int[SingletonCustom<GameSettingManager>.Instance.TeamNum];
            for (int num = 0; num < SingletonCustom<GameSettingManager>.Instance.TeamNum; num++) {
                array[num] = teamDataList[num].score;
                array2[num] = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[teamDataList[num].teamNo][0];
            }
            ResultGameDataParams.SetRecord_Int(array, array2);
            if (ResultGameDataParams.GetRecord().intData != null) {
                rankGroupRecords.Add(ResultGameDataParams.GetRecord());
            }
            if (ResultGameDataParams.GetRecord(_isGroup1Record: false).intData != null) {
                rankGroupRecords.Add(ResultGameDataParams.GetRecord(_isGroup1Record: false));
            }
            InitShowResult();
            SetRankingData();
            SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_Ranking(_showLastResult: false);
            LeanTween.cancel(bottomDecoRoot);
            bottomDecoRoot.transform.SetLocalPositionY(-150f);
            StartCoroutine(_ShowRankingResult());
        }
    }
    private void InitShowResult() {
        ResultGameDataParams.SetResultMode(_isResult: true);
        InitResultAnimation();
        resultWindow.SetActive(value: true);
    }
    private void InitResultAnimation() {
        isAnimation = true;
        resultWindow.transform.SetLocalPositionY(defPosY_ResultWindow);
        for (int i = 0; i < defPosX_OperationButton.Length; i++) {
            buttonUIData.arrayAnimButton[i].transform.SetLocalPositionX(defPosX_OperationButton[i] + 2000f);
        }
        psEffectCracker.Stop();
        psEffectConfetti.Stop();
        bottomDecoRoot.SetActive(value: false);
        for (int j = 0; j < rankGroupRecords.Count; j++) {
            for (int k = 0; k < rankGroupRecords[j].rankNo.Length; k++) {
                rankGroupData[j].rankUIData[k].GetRankUIData().crownIcon.transform.SetLocalScale(1f, 1f, 1f);
                rankGroupData[j].rankUIData[k].GetRankUIData().crownIcon.SetAlpha(0f);
                if (rankGroupRecords[j].rankNo[k] == 0) {
                    rankGroupData[j].rankUIData[k].GetRankUIData().teamIcon.transform.SetLocalScale(1f, 1f, 1f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().teamIcon.SetAlpha(0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().playerIcon.transform.SetLocalScale(1f, 1f, 1f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().playerIcon.SetAlpha(0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().playerIcon.gameObject.SetActive(value: false);
                    rankGroupData[j].rankUIData[k].GetRankUIData().cpuIcon.transform.SetLocalScale(1f, 1f, 1f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().cpuIcon.SetAlpha(0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().cpuIcon.gameObject.SetActive(value: false);
                    rankGroupData[j].rankUIData[k].GetRankUIData().characterIcon.transform.SetLocalScale(1f, 1f, 1f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().characterIcon.SetAlpha(0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().rankCaption.SetAlpha(0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().pointNumbers.SetScale(1f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().pointNumbers.SetAlpha(0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().pointText.transform.SetLocalScale(1f, 1f, 1f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().pointText.SetAlpha(0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().evaluationIcon.transform.SetLocalScale(1f, 1f, 1f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().evaluationIcon.SetAlpha(0f);
                    rankGroupData[j].rankUIData[k].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.numbers.transform.SetLocalScale(1f, 1f, 1f);
                    rankGroupData[j].rankUIData[k].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.transform.SetLocalScale(1f, 1f, 1f);
                    rankGroupData[j].rankUIData[k].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.numbers.SetAlpha(0f);
                    rankGroupData[j].rankUIData[k].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.SetAlpha(0f);
                    rankGroupData[j].rankUIData[k].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.addText_pt.transform.SetLocalScale(1f, 1f, 1f);
                    rankGroupData[j].rankUIData[k].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.addText_pt.SetAlpha(0f);
                    rankGroupData[j].rankUIData[k].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.addText_pt.gameObject.SetActive(value: true);
                } else {
                    rankGroupData[j].rankUIData[k].GetRankUIData().teamIcon.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().playerIcon.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().playerIcon.gameObject.SetActive(value: false);
                    rankGroupData[j].rankUIData[k].GetRankUIData().cpuIcon.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().cpuIcon.gameObject.SetActive(value: false);
                    rankGroupData[j].rankUIData[k].GetRankUIData().characterIcon.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().evaluationIcon.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().pointNumbers.SetScale(0f);
                    rankGroupData[j].rankUIData[k].GetRankUIData().pointText.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[j].rankUIData[k].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.numbers.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[j].rankUIData[k].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[j].rankUIData[k].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.addText_pt.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[j].rankUIData[k].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.addText_pt.gameObject.SetActive(value: true);
                }
            }
        }
    }
    private void SetRankingData() {
        int num = 0;
        for (int i = 0; i < rankGroupRecords.Count; i++) {
            rankRecordCnt[i] = rankGroupRecords[i].intData.Length;
            UnityEngine.Debug.Log("記録デ\u30fcタ数：" + rankGroupRecords[i].intData.Length.ToString());
            for (int j = 0; j < rankGroupRecords[i].rankNo.Length; j++) {
                string text = "rank_" + (rankGroupRecords[i].rankNo[j] + 1).ToString();
                if (Localize_Define.Language != 0) {
                    text = "en_" + text;
                }
                rankGroupData[i].rankUIData[j].GetRankUIData().rankCaption.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, text);
                string str = "";
                if (Localize_Define.Language != 0) {
                    str = "en_";
                }
                switch (rankGroupRecords[i].rankNo[j]) {
                    case 0:
                        str += "chalice_of_gold";
                        rankGroupData[i].rankUIData[j].GetRankUIData().crownIcon.sprite = arrayOvalSprite[0];
                        rankGroupData[i].rankUIData[j].GetRankUIData().crownIcon.transform.AddLocalPositionX(-5f);
                        if (rankGroupRecords[i].userTypeList[j].userType[0] <= ResultGameDataParams.UserType.PLAYER_4) {
                            num++;
                        }
                        break;
                    case 1:
                        str += "chalice_of_silver";
                        rankGroupData[i].rankUIData[j].GetRankUIData().crownIcon.sprite = arrayOvalSprite[1];
                        rankGroupData[i].rankUIData[j].GetRankUIData().crownIcon.transform.AddLocalPositionX(-5f);
                        UnityEngine.Object.Destroy(rankGroupData[i].rankUIData[j].GetRankUIData().crownIcon.GetComponent<SpriteShiningEffect>());
                        if (rankGroupRecords[i].userTypeList[j].userType[0] <= ResultGameDataParams.UserType.PLAYER_4) {
                            num++;
                        }
                        break;
                    case 2:
                        str += "chalice_of_bronze";
                        rankGroupData[i].rankUIData[j].GetRankUIData().crownIcon.sprite = arrayOvalSprite[2];
                        rankGroupData[i].rankUIData[j].GetRankUIData().crownIcon.transform.AddLocalPositionX(-5f);
                        UnityEngine.Object.Destroy(rankGroupData[i].rankUIData[j].GetRankUIData().crownIcon.GetComponent<SpriteShiningEffect>());
                        if (rankGroupRecords[i].userTypeList[j].userType[0] <= ResultGameDataParams.UserType.PLAYER_4) {
                            num++;
                        }
                        break;
                    case 3:
                        rankGroupData[i].rankUIData[j].GetRankUIData().crownIcon.gameObject.SetActive(value: false);
                        if (rankGroupRecords[i].userTypeList[j].userType[0] <= ResultGameDataParams.UserType.PLAYER_4) {
                            num++;
                        }
                        break;
                }
                if (rankGroupRecords[i].userTypeList[j].userType[0] >= ResultGameDataParams.UserType.CPU_1) {
                    switch (rankGroupRecords[i].userTypeList[j].userType[0]) {
                        case ResultGameDataParams.UserType.CPU_1:
                            rankGroupData[i].rankUIData[j].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp1");
                            break;
                        case ResultGameDataParams.UserType.CPU_2:
                            rankGroupData[i].rankUIData[j].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp2");
                            break;
                        case ResultGameDataParams.UserType.CPU_3:
                            rankGroupData[i].rankUIData[j].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp3");
                            break;
                        case ResultGameDataParams.UserType.CPU_4:
                            rankGroupData[i].rankUIData[j].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp4");
                            break;
                        case ResultGameDataParams.UserType.CPU_5:
                            rankGroupData[i].rankUIData[j].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp5");
                            break;
                        case ResultGameDataParams.UserType.CPU_6:
                            rankGroupData[i].rankUIData[j].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp6");
                            break;
                        case ResultGameDataParams.UserType.CPU_7:
                            rankGroupData[i].rankUIData[j].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp7");
                            break;
                    }
                    rankGroupData[i].rankUIData[j].GetRankUIData().cpuIcon.gameObject.SetActive(value: true);
                } else {
                    rankGroupData[i].rankUIData[j].GetRankUIData().playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_" + ((int)(rankGroupRecords[i].userTypeList[j].userType[0] + 1)).ToString() + "p");
                    rankGroupData[i].rankUIData[j].GetRankUIData().playerIcon.gameObject.SetActive(value: true);
                }
                UnityEngine.Debug.Log(i.ToString() + "番のキャラ番号：" + SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)rankGroupRecords[i].userTypeList[j].userType[0]].ToString());
                rankGroupData[i].rankUIData[j].GetRankUIData().characterIcon.gameObject.SetActive(value: true);
                UnityEngine.Debug.Log("userType:" + rankGroupRecords[i].userTypeList[j].userType[0].ToString());
                int num2 = (int)rankGroupRecords[i].userTypeList[j].userType[0];
                UnityEngine.Debug.Log("int:" + num2.ToString());
                int num3 = (int)rankGroupRecords[i].userTypeList[j].userType[0];
                if (num3 >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                    num3 = SingletonCustom<GameSettingManager>.Instance.PlayerNum + (num3 - 6);
                }
                UnityEngine.Debug.Log("idx:" + num3.ToString());
                switch (num3) {
                    case 0:
                        rankGroupData[i].rankUIData[j].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "1P_result_frame");
                        break;
                    case 1:
                        rankGroupData[i].rankUIData[j].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "2P_result_frame");
                        break;
                    case 2:
                        rankGroupData[i].rankUIData[j].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "3P_result_frame");
                        break;
                    case 3:
                        rankGroupData[i].rankUIData[j].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "4P_result_frame");
                        break;
                    case 4:
                        rankGroupData[i].rankUIData[j].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "5P_result_frame");
                        break;
                    case 6:
                        rankGroupData[i].rankUIData[j].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "6P_result_frame");
                        break;
                    case 5:
                        rankGroupData[i].rankUIData[j].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "6P_result_frame");
                        break;
                    case 7:
                        rankGroupData[i].rankUIData[j].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "6P_result_frame");
                        break;
                }
                rankGroupData[i].rankUIData[j].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.recordAnchor.gameObject.SetActive(value: true);
                rankGroupData[i].rankUIData[j].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.SetText(teamDataList[j].score.ToString());
            }
        }
    }
    private IEnumerator _ShowRankingResult() {
        Vector3 captionPos = objCaption.transform.localPosition;
        objCaption.transform.SetLocalPosition(0f, 0f, 0f);
        objCaption.transform.SetLocalScale(0f, 0f, 0f);
        LeanTween.scale(objCaption, Vector3.one, 1.5f).setEaseOutBack();
        yield return new WaitForSeconds(2f);
        LeanTween.moveLocal(objCaption, captionPos, 0.5f).setEaseOutCubic();
        yield return new WaitForSeconds(0.5f);
        bottomDecoRoot.SetActive(value: true);
        LeanTween.moveLocalY(bottomDecoRoot, 0f, 0.75f).setEaseOutQuad();
        Animation_Ranking();
        yield return new WaitForSeconds((resultRankingAnimationTime_Battle_Ranking + resultRankingAnimationWaitTime_Battle_Ranking) * (float)rankRecordCnt[0] + 0.1f);
        yield return new WaitForSeconds(resultRankingAnimationTime_Battle_Ranking + 0.1f);
        LeanTween.value(1f, 0f, 0.5f).setOnUpdate(delegate (float _value) {
            meshBackFade.material.SetAlpha(_value);
        });
        psEffectCracker.Play();
        for (int i = 0; i < 2; i++) {
            StartCoroutine(_PlayCrackerSE((float)i * 0.25f));
        }
        psEffectConfetti.Play();
        SingletonCustom<ResultCharacterManager>.Instance.StartPhotoAnimation(delegate {
            Animation_PlayKingMark();
        });
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_win");
        yield return new WaitForSeconds(resultPlayKingMarkAnimationTime);
        Animation_OperationButton();
    }
    private IEnumerator _ShowResultWinOrLose() {
        resultWindow.transform.SetLocalPositionY(defPosY_ResultWindow + 1500f);
        resultWindow.SetActive(value: true);
        for (int i = 0; i < winOrLoseResultUIData.pointNumbers.Length; i++) {
            winOrLoseResultUIData.pointNumbers[i].Set(0);
        }
        winOrLoseResultUIData.captionAnchor.gameObject.SetActive(value: false);
        Animation_ResultDialog_SlideIn();
        yield return new WaitForSeconds(resultBackFanAnimationTime + 0.1f);
        bottomDecoRoot.SetActive(value: true);
        LeanTween.moveLocalY(bottomDecoRoot, 0f, 0.75f).setEaseOutQuad();
        int highPoint = (teamDataList[0].score > teamDataList[1].score) ? teamDataList[0].score : teamDataList[1].score;
        winOrLoseResultUIData.pointNumbers[teamDataList[0].teamNo].IsZeroFill = true;
        winOrLoseResultUIData.pointNumbers[teamDataList[0].teamNo].LimitLength = highPoint.ToString().Length;
        float checkTime = 0f;
        float delay = 5.2f + 6.5f * (float)highPoint.ToString().Length;
        bool isNum1Drum = false;
        bool isNum10Drum = false;
        bool isNum100Drum = false;
        LeanTween.value(0f, delay, delay).setOnUpdate(delegate (float _value) {
            checkTime = _value;
        });
        LeanTween.value(0f, teamDataList[0].score, delay).setOnUpdate(delegate (float _value) {
            int num2 = (int)_value;
            num2 = 0;
            switch (highPoint.ToString().Length) {
                case 1:
                    num2 = UnityEngine.Random.Range(0, 9);
                    break;
                case 2:
                    num2 = UnityEngine.Random.Range(0, 9);
                    num2 += UnityEngine.Random.Range(0, 9) * 10;
                    break;
                case 3:
                    num2 = UnityEngine.Random.Range(0, 9);
                    num2 += UnityEngine.Random.Range(0, 9) * 10;
                    num2 += UnityEngine.Random.Range(0, 9) * 100;
                    break;
                case 4:
                    num2 = UnityEngine.Random.Range(0, 9);
                    num2 += UnityEngine.Random.Range(0, 9) * 10;
                    num2 += UnityEngine.Random.Range(0, 9) * 100;
                    num2 += UnityEngine.Random.Range(0, 9) * 1000;
                    break;
            }
            if (checkTime >= delay / (float)highPoint.ToString().Length * 0.25f) {
                num2 = num2 / 10 * 10 + teamDataList[0].score % 10;
                if (!isNum1Drum) {
                    SingletonCustom<AudioManager>.Instance.SeStop("se_drum_roll");
                    SingletonCustom<AudioManager>.Instance.SePlay("se_roll_finish");
                    SingletonCustom<AudioManager>.Instance.SePlay("se_drum_roll", _loop: false, 0f, 1f, 1f, 0.5f);
                    isNum1Drum = true;
                }
            }
            if (checkTime >= delay / (float)highPoint.ToString().Length * 0.5f) {
                num2 = num2 / 100 * 100 + teamDataList[0].score % 100;
                if (!isNum10Drum) {
                    SingletonCustom<AudioManager>.Instance.SeStop("se_drum_roll");
                    SingletonCustom<AudioManager>.Instance.SePlay("se_roll_finish");
                    SingletonCustom<AudioManager>.Instance.SePlay("se_drum_roll", _loop: false, 0f, 1f, 1f, 0.5f);
                    isNum10Drum = true;
                }
            }
            if (checkTime >= delay / (float)highPoint.ToString().Length * 0.9f) {
                num2 = num2 / 1000 * 1000 + teamDataList[0].score % 1000;
                if (!isNum100Drum) {
                    SingletonCustom<AudioManager>.Instance.SeStop("se_drum_roll");
                    SingletonCustom<AudioManager>.Instance.SePlay("se_roll_finish");
                    isNum100Drum = true;
                }
                if (highPoint.ToString().Length != 3) {
                    num2 = num2 / 10000 * 10000 + teamDataList[0].score % 10000;
                    if (teamDataList[0].score.ToString().Length < highPoint.ToString().Length) {
                        winOrLoseResultUIData.pointNumbers[teamDataList[0].teamNo].IsZeroFill = false;
                    }
                }
            }
            winOrLoseResultUIData.pointNumbers[teamDataList[0].teamNo].Set(num2);
        }).setOnComplete((Action)delegate {
            winOrLoseResultUIData.pointNumbers[teamDataList[0].teamNo].IsZeroFill = false;
            winOrLoseResultUIData.pointNumbers[teamDataList[0].teamNo].Set(teamDataList[0].score);
        })
            .setEaseOutQuart();
        winOrLoseResultUIData.pointNumbers[teamDataList[1].teamNo].IsZeroFill = true;
        winOrLoseResultUIData.pointNumbers[teamDataList[1].teamNo].LimitLength = highPoint.ToString().Length;
        LeanTween.value(0f, teamDataList[1].score, delay).setOnUpdate(delegate (float _value) {
            int num = (int)_value;
            num = 0;
            switch (highPoint.ToString().Length) {
                case 1:
                    num = UnityEngine.Random.Range(0, 9);
                    break;
                case 2:
                    num = UnityEngine.Random.Range(0, 9);
                    num += UnityEngine.Random.Range(0, 9) * 10;
                    break;
                case 3:
                    num = UnityEngine.Random.Range(0, 9);
                    num += UnityEngine.Random.Range(0, 9) * 10;
                    num += UnityEngine.Random.Range(0, 9) * 100;
                    break;
                case 4:
                    num = UnityEngine.Random.Range(0, 9);
                    num += UnityEngine.Random.Range(0, 9) * 10;
                    num += UnityEngine.Random.Range(0, 9) * 100;
                    num += UnityEngine.Random.Range(0, 9) * 1000;
                    break;
            }
            if (checkTime >= delay / (float)highPoint.ToString().Length * 0.25f) {
                num = num / 10 * 10 + teamDataList[1].score % 10;
            }
            if (checkTime >= delay / (float)highPoint.ToString().Length * 0.5f) {
                num = num / 100 * 100 + teamDataList[1].score % 100;
            }
            if (checkTime >= delay / (float)highPoint.ToString().Length * 0.9f) {
                num = num / 1000 * 1000 + teamDataList[1].score % 1000;
                if (highPoint.ToString().Length != 3) {
                    num = num / 10000 * 10000 + teamDataList[1].score % 10000;
                    if (teamDataList[1].score.ToString().Length < highPoint.ToString().Length) {
                        winOrLoseResultUIData.pointNumbers[teamDataList[1].teamNo].IsZeroFill = false;
                    }
                }
            }
            winOrLoseResultUIData.pointNumbers[teamDataList[1].teamNo].Set(num);
        }).setOnComplete((Action)delegate {
            winOrLoseResultUIData.pointNumbers[teamDataList[1].teamNo].IsZeroFill = false;
            winOrLoseResultUIData.pointNumbers[teamDataList[1].teamNo].Set(teamDataList[1].score);
        })
            .setEaseOutQuart();
        SingletonCustom<AudioManager>.Instance.SePlay("se_drum_roll");
        yield return new WaitForSeconds(8.2f);
        yield return new WaitForSeconds(0.95f);
        winOrLoseResultUIData.captionAnchor.gameObject.SetActive(value: true);
        winOrLoseResultUIData.captionAnchor.SetLocalScale(3f, 3f, 1f);
        LeanTween.scale(winOrLoseResultUIData.captionAnchor.gameObject, Vector3.one, 0.75f).setEaseOutBack();
        yield return new WaitForSeconds(1f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_ranking_1st_win");
        if (isSingleWin) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
            LeanTween.value(0f, 1f, 0.75f).setOnUpdate(delegate (float _value) {
                winOrLoseResultUIData.winnerTeamNameSprite[1].SetAlpha(1f - _value);
                winOrLoseResultUIData.winnerTeamNameSprite[1].transform.SetLocalScale(1f + _value / 2f, 1f + _value / 2f, 1f);
            });
        }
        yield return new WaitForSeconds(1.1f);
        Animation_PlayKingMark();
        if (SingletonCustom<ResultCharacterManager>.Instance.IsCharacterFaceType(ResultCharacterManager.FaceType.Happy)) {
            psEffectCracker.Play();
            for (int j = 0; j < 2; j++) {
                StartCoroutine(_PlayCrackerSE((float)j * 0.25f));
            }
            psEffectConfetti.Play();
            SingletonCustom<ResultCharacterManager>.Instance.StartPhotoAnimation(delegate {
                Animation_OperationButton();
            });
            SingletonCustom<AudioManager>.Instance.SePlay("se_result_win");
        } else if (SingletonCustom<ResultCharacterManager>.Instance.IsCharacterFaceType(ResultCharacterManager.FaceType.Sad)) {
            SingletonCustom<ResultCharacterManager>.Instance.StartPhotoAnimation(delegate {
                Animation_OperationButton();
            });
        } else if (SingletonCustom<ResultCharacterManager>.Instance.IsCharacterFaceType(ResultCharacterManager.FaceType.Normal)) {
            SingletonCustom<ResultCharacterManager>.Instance.StartPhotoAnimation(delegate {
                Animation_OperationButton();
            });
        }
    }
    private void Animation_ResultDialog_SlideIn() {
        LeanTween.moveLocalY(resultWindow, defPosY_ResultWindow, resultWindowAnimationTime).setEaseOutBack();
    }
    private void Animation_PlayKingMark() {
        SpriteRenderer[] array = null;
        SpriteRenderer[] array2 = null;
        array = new SpriteRenderer[1]
        {
            spWinMark
        };
        array2 = new SpriteRenderer[1];
        for (int i = 0; i < array.Length; i++) {
            if (array[i].gameObject.activeSelf) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_result_view_score");
                break;
            }
        }
        for (int j = 0; j < array.Length; j++) {
            if (array[j].gameObject.activeSelf) {
                array2[j] = UnityEngine.Object.Instantiate(array[j], array[j].transform.localPosition, Quaternion.identity, array[j].transform.parent);
                array2[j].transform.localPosition = array[j].transform.localPosition;
                array2[j].SetAlpha(0f);
                array2[j].transform.localScale = Vector3.one;
                Animation_PlayKingMark_Process(array[j], array2[j]);
            }
        }
    }
    private void Animation_PlayKingMark_Process(SpriteRenderer _animationSp, SpriteRenderer _effectSp) {
        StartCoroutine(SetAlphaColor(_animationSp, resultPlayKingMarkAnimationTime));
        LeanTween.scale(_animationSp.gameObject, Vector3.one, resultPlayKingMarkAnimationTime).setEaseOutBack().setOnComplete((Action)delegate {
            _effectSp.SetAlpha(1f);
            LeanTween.scale(_effectSp.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.6f).setEaseLinear();
            StartCoroutine(SetAlphaColor(_effectSp, 0.6f, 0f, _isFadeOut: true));
        });
    }
    private void Animation_Ranking() {
        StartCoroutine(Animation_Ranking_Score(0, resultRankingAnimationTime_Battle_Ranking + resultRankingAnimationWaitTime_Battle_Ranking));
    }
    private IEnumerator Animation_Ranking_Score(int _showGroupNo, float _delay) {
        for (int i = rankGroupRecords[_showGroupNo].rankNo.Length - 1; i >= 0; i--) {
            if (rankGroupRecords[_showGroupNo].rankNo[i] == 0) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_result_ranking_1st_win");
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().teamIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().teamIcon, resultRankingAnimationTime_Battle_Ranking));
                if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().playerIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().playerIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().playerIcon, resultRankingAnimationTime_Battle_Ranking));
                } else if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().cpuIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().cpuIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().cpuIcon, resultRankingAnimationTime_Battle_Ranking));
                }
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().characterIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().characterIcon, resultRankingAnimationTime_Battle_Ranking));
                StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().rankCaption, resultRankingAnimationTime_Battle_Ranking));
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.numbers.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.numbers.GetArraySpriteNumbers(), resultRankingAnimationTime_Battle_Ranking));
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.numbers.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.numbers.GetArraySpriteNumbers(), resultRankingAnimationTime_Battle_Ranking));
                StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers, resultRankingAnimationTime_Battle_Ranking));
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.addText_pt.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.addText_pt, resultRankingAnimationTime_Battle_Ranking));
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().crownIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking).setEaseLinear();
                StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().crownIcon, resultRankingAnimationTime_Battle_Ranking));
                rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().glitterSpriteEffect.StartGlitterAnimation();
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().evaluationIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().evaluationIcon, resultRankingAnimationTime_Battle_Ranking));
            } else {
                SingletonCustom<AudioManager>.Instance.SePlay("se_result_puyon");
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().teamIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().playerIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().playerIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                } else if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().cpuIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().cpuIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                }
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().characterIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.numbers.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.numbers.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.addText_pt.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().evaluationIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().crownIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking).setEaseLinear();
                StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().crownIcon, resultRankingAnimationTime_Battle_Ranking));
            }
            yield return new WaitForSeconds(_delay);
        }
    }
    private IEnumerator _PlayCrackerSE(float _waitTime) {
        yield return new WaitForSeconds(_waitTime);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cracker");
    }
    private void Animation_OperationButton() {
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
        if (SingletonCustom<CommonNotificationManager>.Instance != null) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGetTrophy(null);
        }
        float num = resultButtonAnimationTime;
        int num2 = 0;
        num2 = buttonUIData.arrayAnimButton.Length - 1;
        for (int i = 0; i < num2; i++) {
            LeanTween.moveLocalX(buttonUIData.arrayAnimButton[i], defPosX_OperationButton[i], resultButtonAnimationTime).setDelay(num + (float)i * num);
        }
        LeanTween.moveLocalX(buttonUIData.arrayAnimButton[num2], defPosX_OperationButton[num2], resultButtonAnimationTime).setDelay(num + (float)num2 * num).setOnComplete(OnCompleteAllResultAnimation);
    }
    private void OnCompleteAllResultAnimation() {
        isAnimation = false;
        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Sort((GameSettingManager.TeamData a, GameSettingManager.TeamData b) => b.score - a.score);
    }
    public bool IsResultAnimation() {
        return isAnimation;
    }
    private IEnumerator SetAlphaColor(MeshRenderer _renderer, float _fadeTime, float _delayTime = 0f, bool _isFadeOut = false) {
        float time = 0f;
        float startAlpha = 0f;
        float endAlpha = 1f;
        if (_isFadeOut) {
            startAlpha = 1f;
            endAlpha = 0f;
        }
        yield return new WaitForSeconds(_delayTime);
        while (time < _fadeTime) {
            _renderer.material.SetAlpha(Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime));
            time += Time.deltaTime;
            yield return null;
        }
        _renderer.material.SetAlpha(endAlpha);
    }
    private IEnumerator SetAlphaColor(SpriteRenderer[] _renderer, float _fadeTime, float _delayTime = 0f, bool _isFadeOut = false) {
        float time = 0f;
        float startAlpha = 0f;
        float endAlpha = 1f;
        if (_isFadeOut) {
            startAlpha = 1f;
            endAlpha = 0f;
        }
        yield return new WaitForSeconds(_delayTime);
        while (time < _fadeTime) {
            for (int i = 0; i < _renderer.Length; i++) {
                if (_renderer[i] != null) {
                    _renderer[i].SetAlpha(Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime));
                }
            }
            time += Time.deltaTime;
            yield return null;
        }
        for (int j = 0; j < _renderer.Length; j++) {
            _renderer[j].SetAlpha(endAlpha);
        }
    }
    private IEnumerator SetAlphaColor(SpriteRenderer _spriteRenderer, float _fadeTime, float _delayTime = 0f, bool _isFadeOut = false) {
        float time = 0f;
        float startAlpha = 0f;
        float endAlpha = 1f;
        if (_isFadeOut) {
            startAlpha = 1f;
            endAlpha = 0f;
        }
        yield return new WaitForSeconds(_delayTime);
        while (time < _fadeTime) {
            _spriteRenderer.SetAlpha(Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime));
            time += Time.deltaTime;
            yield return null;
        }
        _spriteRenderer.SetAlpha(endAlpha);
    }
    private IEnumerator SetAlphaColor(TextMeshPro[] _arrayTextMeshPro, float _fadeTime, float _delayTime = 0f, bool _isFadeOut = false) {
        float time = 0f;
        float startAlpha = 0f;
        float endAlpha = 1f;
        if (_isFadeOut) {
            startAlpha = 1f;
            endAlpha = 0f;
        }
        yield return new WaitForSeconds(_delayTime);
        while (time < _fadeTime) {
            for (int i = 0; i < _arrayTextMeshPro.Length; i++) {
                _arrayTextMeshPro[i].SetAlpha(Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime));
            }
            time += Time.deltaTime;
            yield return null;
        }
        for (int j = 0; j < _arrayTextMeshPro.Length; j++) {
            _arrayTextMeshPro[j].SetAlpha(endAlpha);
        }
    }
    private IEnumerator SetAlphaColor(TextMeshPro _textMeshPro, float _fadeTime, float _delayTime = 0f, bool _isFadeOut = false) {
        float time = 0f;
        float startAlpha = 0f;
        float endAlpha = 1f;
        if (_isFadeOut) {
            startAlpha = 1f;
            endAlpha = 0f;
        }
        yield return new WaitForSeconds(_delayTime);
        while (time < _fadeTime) {
            _textMeshPro.SetAlpha(Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime));
            time += Time.deltaTime;
            yield return null;
        }
        _textMeshPro.SetAlpha(endAlpha);
    }
}
