using GamepadInput;
using System;
using System.Collections;
using System.Collections.Generic;
using io.ninenine.players.party3d.games.common;
using TMPro;
using UnityEngine;
public class RankingResultManager : MonoBehaviour {
    [Serializable]
    public struct RankGroupUIData {
        [Header("各組のUIデ\u30fcタ")]
        public RankUIData[] rankUIData;
        [Header("各組のUIアンカ\u30fc")]
        public Transform[] rankUIAnchor;
    }
    [Serializable]
    public struct HiRecordUIData {
        [Header("最高記録のアンカ\u30fc")]
        public Transform hiRecordAnchor;
        [Header("新記録のアイコン")]
        public SpriteRenderer newRecordIcon;
        [Header("最高記録のキャプション")]
        public SpriteRenderer hiRecordCaption;
        [Header("UIデ\u30fcタ")]
        public RankUIData rankUIData;
    }
    [Serializable]
    public struct LastResultUIData {
        [Header("勝利チ\u30fcムの名前画像")]
        public SpriteRenderer winnerTeamNameSprite;
        [Header("勝ちの画像")]
        public SpriteRenderer victorySprite;
        [Header("負けの画像")]
        public SpriteRenderer defeatSprite;
        [Header("引き分けの画像")]
        public SpriteRenderer drawSprite;
        [Header("記録UIデ\u30fcタ")]
        public RankingLastResultRecordUIData rankingLastResultUIData;
    }
    [Serializable]
    public struct TorunamentUIData {
        [Header("ト\u30fcナメントアンカ\u30fc")]
        public GameObject tournamentAnchor;
        [Header("ト\u30fcナメント表アンカ\u30fc")]
        public Transform tournamentSheetAnchor;
        [Header("ト\u30fcナメント表切り替えアイコン")]
        public GameObject tournamentSwitchIcon;
        [Header("各プレイヤ\u30fcの名前画像")]
        public SpriteRenderer[] playerNameSprites;
        [Header("各CPUの名前画像")]
        public SpriteRenderer[] cpuNameSprites;
        [Header("１回戦のライン画像デ\u30fcタ")]
        public TournamentSheetData.LineSpriteData lineData_1stRound;
        [Header("２回戦のライン画像デ\u30fcタ")]
        public TournamentSheetData.LineSpriteData lineData_2ndRound;
        [Header("決勝のライン画像デ\u30fcタ")]
        public TournamentSheetData.LineSpriteData lineData_Final;
        [Header("１回戦の敗者戦用のライン画像デ\u30fcタ")]
        public TournamentSheetData.LineSpriteData lineData_1stRound_Lose;
        [Header("２回戦の敗者戦用のライン画像デ\u30fcタ")]
        public TournamentSheetData.LineSpriteData lineData_2ndRound_Lose;
        [Header("決勝の敗者戦用のライン画像デ\u30fcタ")]
        public TournamentSheetData.LineSpriteData lineData_Final_Lose;
    }
    public enum PageType {
        Page_1,
        Page_2,
        Final
    }
    private struct SortData {
        public int data1;
        public int data2;
    }
    [SerializeField]
    [Header("フレ\u30fcム画像")]
    private SpriteRenderer[] arraySpFrame;
    [SerializeField]
    [Header("リザルト画面Root")]
    private GameObject resultWindowRoot;
    [SerializeField]
    [Header("リザルトの写真アンカ\u30fc")]
    private GameObject resultPhotoAnchor;
    [SerializeField]
    [Header("リザルトの写真画像")]
    private SpriteRenderer resultPhoto;
    [SerializeField]
    [Header("順位のUIアンカ\u30fc")]
    private Transform rankGroupUIAnchor;
    [SerializeField]
    [Header("順位のUIデ\u30fcタ")]
    private RankGroupUIData[] rankGroupData;
    [SerializeField]
    [Header("最高記録を表示するUIデ\u30fcタ")]
    private HiRecordUIData hiRecordUIData;
    [SerializeField]
    [Header("スクロ\u30fcル可能な時に表示するUIアンカ\u30fc")]
    private Transform scrollUIAnchor;
    [SerializeField]
    [Header("スクロ\u30fcル時の左矢印")]
    private SpriteRenderer scrollLeftArrow;
    [SerializeField]
    [Header("スクロ\u30fcル時の右矢印")]
    private SpriteRenderer scrollRightArrow;
    [SerializeField]
    [Header("スクロ\u30fcル時の～組目の表示")]
    private SpriteRenderer scrollGroupNo;
    [SerializeField]
    [Header("最終結果のUIデ\u30fcタ")]
    private LastResultUIData lastResultUIData;
    [SerializeField]
    [Header("最終結果のグル\u30fcプのアンカ\u30fc")]
    private Transform lastResultGroupAnchor;
    [SerializeField]
    [Header("ト\u30fcナメント表のUIデ\u30fcタ")]
    private TorunamentUIData tournamentUIData;
    [SerializeField]
    [Header("演出：クラッカ\u30fc")]
    private ParticleSystem psEffectCracker;
    [SerializeField]
    [Header("演出：紙吹雪")]
    private ParticleSystem psEffectConfetti;
    [SerializeField]
    [Header("シ\u30fcトに付随する追加文字アンカ\u30fc")]
    private Transform sheetAddTextAnchor;
    [SerializeField]
    [Header("【生存時間】の文字オブジェクト")]
    private GameObject survivalTimeTextObject;
    [SerializeField]
    [Header("【水をかけた量】の文字オブジェクト")]
    private GameObject amountOfWaterTextObject;
    [SerializeField]
    [Header("ボタンパタ\u30fcン[リトライ][ゲ\u30fcムセレクト]")]
    private ButtonPatternData buttonPattern_Retry_GameSelect;
    [SerializeField]
    [Header("ボタンパタ\u30fcン[スクロ\u30fcル][リトライ][ゲ\u30fcムセレクト]")]
    private ButtonPatternData buttonPattern_Scroll_Retry_GameSelect;
    [SerializeField]
    [Header("ボタンパタ\u30fcン[プログラム][結果詳細][つぎへ]")]
    private ButtonPatternData buttonPattern_Program_Details_Next;
    [SerializeField]
    [Header("ボタンパタ\u30fcン[スクロ\u30fcル][プログラム][結果詳細][つぎへ]")]
    private ButtonPatternData buttonPattern_Scroll_Program_Details_Next;
    [SerializeField]
    [Header("ボタンパタ\u30fcン[つぎへ]")]
    private ButtonPatternData buttonPattern_Next;
    [SerializeField]
    [Header("ボタンパタ\u30fcン[スクロ\u30fcル][つぎへ]")]
    private ButtonPatternData buttonPattern_Scroll_Next;
    [SerializeField]
    [Header("キ\u30fcホルダ\u30fcの杭画像")]
    private SpriteRenderer keyringPile;
    [SerializeField]
    [Header("消しゴムくんのキ\u30fcホルダ\u30fc画像")]
    private SpriteRenderer KeshigomukunKeyRing;
    [SerializeField]
    [Header("いちごちゃんのキ\u30fcホルダ\u30fc画像")]
    private SpriteRenderer IchigochanKeyRing;
    [SerializeField]
    [Header("途中経過画像配列")]
    private Sprite[] arraySpInterim;
    [SerializeField]
    [Header("パ\u30fcティ\u30fcモ\u30fcドゲ\u30fcム数ル\u30fcト")]
    private GameObject objPartyModeGameCntRoot;
    [SerializeField]
    [Header("パ\u30fcティ\u30fcモ\u30fcドゲ\u30fcム数テキスト")]
    private TextMeshPro textPartyModeGameCnt;
    [SerializeField]
    [Header("装飾スタ\u30fc4人用")]
    private SpriteRenderer[] arrayDecoStartFour;
    [SerializeField]
    [Header("装飾スタ\u30fc6人用")]
    private SpriteRenderer[] arrayDecoStartSix;
    private float resultWindowAnimationTime = 0.6f;
    private float resultBackFanAnimationTime = 0.5f;
    private float resultRankingAnimationTime_Battle_Ranking = 0.2f;
    private float resultRankingAnimationWaitTime_Battle_Ranking = 0.3f;
    private float resultFirstRankPlayerIconAnimationTime_Battle_Ranking = 1f;
    private float resultHiRecordAnimationTime = 0.4f;
    private float resultNewRecordAnimationTime = 0.4f;
    private float resultButtonAnimationTime = 0.3f;
    private bool isAnimation;
    private float defPosY_ResultWindow;
    private ResultGameDataParams.ShowResultType showResultType;
    private int[] rankRecordCnt = new int[2];
    private List<ResultGameDataParams.RankingRecord> rankGroupRecords = new List<ResultGameDataParams.RankingRecord>();
    private bool isShowPointData;
    private PageType currentShowGroupType;
    private bool isNewRecord;
    private bool isLastResultLoseFlg;
    private bool isLastResultDrawFlg;
    private Coroutine lastResultCoroutine;
    private float lastResultAnimationTime = 0.2f;
    private bool isResultOpen;
    private int totalRecord_TeamA;
    private int totalRecord_TeamB;
    private int rank1stCnt;
    private bool isAnimationTournamentSheet;
    private float scrollOffset = 2000f;
    private bool isScrollAnimation;
    private float scrollAnimationTime = 0.1f;
    private void Awake() {
        resultWindowRoot.SetActive(value: false);
        defPosY_ResultWindow = resultWindowRoot.transform.localPosition.y;
        rankGroupUIAnchor.SetLocalPositionY(0f);
        scrollRightArrow.transform.SetLocalPositionY(35.5f);
        scrollLeftArrow.transform.SetLocalPositionY(35.5f);
        //if (!SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup) {
        //    rankGroupData[0].rankUIData[0].transform.SetLocalPosition(99f, -119f, 0f);
        //    rankGroupData[0].rankUIData[1].transform.SetLocalPosition(99f, -159f, 0f);
        //    rankGroupData[0].rankUIData[2].transform.SetLocalPosition(494.5f, 374f, 0f);
        //    rankGroupData[0].rankUIData[3].transform.SetLocalPosition(179.3f, -405f, 0f);
        //}
        for (int i = 0; i < arrayDecoStartSix.Length; i++) {
            arrayDecoStartSix[i].SetAlpha(0f);
        }
        for (int j = 0; j < arrayDecoStartFour.Length; j++) {
            arrayDecoStartFour[j].SetAlpha(0f);
        }
        objPartyModeGameCntRoot.transform.SetLocalPositionX(1400f);
    }
    private void OnDisable() {
        ResultGameDataParams.SetResultMode(_isResult: false);
    }
    private void Update() {
        if (!resultWindowRoot.activeSelf || (SingletonCustom<CommonNotificationManager>.Instance != null && SingletonCustom<CommonNotificationManager>.Instance.IsOpen) || (SingletonCustom<SceneManager>.Instance != null && SingletonCustom<SceneManager>.Instance.GetFadeFlg()) || (SingletonCustom<DM>.Instance != null && SingletonCustom<DM>.Instance.IsActive()) || isAnimation || isScrollAnimation) {
            return;
        }
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE) {
            if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Y)) {
                ClickRetryButton();
            } else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B)) {
                ClickModeSelectButton();
            }
        } else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A)) {
            ClickNextButton();
        }
        if (tournamentUIData.tournamentAnchor.activeSelf && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.RightShoulder)) {
            if (SingletonCustom<SportsDayPoint>.Instance.IsShow) {
                return;
            }
            ClickTournamentSheetSwitch();
        }
        if (scrollUIAnchor.gameObject.activeSelf) {
            if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.LeftShoulder)) {
                ClickScrollLeftButton();
            } else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.RightShoulder)) {
                ClickScrollRightButton();
            }
        }
    }
    public void ShowResult_Score() {
        if (isResultOpen) {
            return;
        }
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_sports_day_final_result_1");
        isResultOpen = true;
        if (ResultGameDataParams.GetRecord().intData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord());
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).intData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord(_isGroup1Record: false));
        }
        InitShowResult();
        if (ResultGameDataParams.GetRecord().intData != null) {
            for (int i = 0; i < rankGroupData[0].rankUIAnchor.Length; i++) {
                rankGroupData[0].rankUIAnchor[i].gameObject.SetActive(value: true);
            }
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).intData != null) {
            for (int j = 0; j < rankGroupData[1].rankUIAnchor.Length; j++) {
                rankGroupData[1].rankUIAnchor[j].gameObject.SetActive(value: true);
            }
            scrollUIAnchor.gameObject.SetActive(value: true);
            if (!SingletonCustom<GameSettingManager>.Instance.IsEightBattle) {
                lastResultGroupAnchor.gameObject.SetActive(value: true);
                SetLastResultData();
            }
        }
        ResultGameDataParams.GetNowSceneType();
        showResultType = ResultGameDataParams.ShowResultType.Record_Score_Four_Digit;
        for (int k = 0; k < rankGroupRecords.Count; k++) {
            rankRecordCnt[k] = rankGroupRecords[k].intData.Length;
            for (int l = 0; l < rankRecordCnt[k]; l++) {
                rankGroupData[k].rankUIData[l].GetRankUIData().rankAnchor.gameObject.SetActive(value: true);
                rankGroupData[k].rankUIData[l].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.recordAnchor.gameObject.SetActive(value: true);
            }
            for (int m = 0; m < rankGroupRecords[k].intData.Length; m++) {
                rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.numbers.Set(rankGroupRecords[k].intData[m]);
                if (ResultGameDataParams.GetNowSceneType() == SceneManager.SceneType.PUSH_IN_BOXING) {
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.SetText(rankGroupRecords[k].intData[m].ToString() + " Win");
                } else {
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.SetText(rankGroupRecords[k].intData[m].ToString());
                }
            }
            SetTeamData(k);
            SetGetPointData(k);
            SetEvaluationIcon(k);
        }
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
            int num = -1;
            int num2 = -1;
            int num3 = -1;
            for (int n = 0; n < rankGroupRecords[0].userTypeList.Length; n++) {
                for (int num4 = 0; num4 < rankGroupRecords[0].userTypeList[n].userType.Length; num4++) {
                    if (rankGroupRecords[0].userTypeList[n].userType[num4] == ResultGameDataParams.UserType.PLAYER_1) {
                        num = rankGroupRecords[0].intData[n];
                        break;
                    }
                }
            }
            num2 = int.Parse(SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Get(ResultGameDataParams.GetNowSceneGameType(), RecordData.InitType.Score));
            if (num > num2) {
                if (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup) {
                    SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.SetSix(ResultGameDataParams.GetNowSceneGameType(), num.ToString());
                } else {
                    SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Set(ResultGameDataParams.GetNowSceneGameType(), num.ToString());
                }
                num3 = num;
                isNewRecord = true;
            } else {
                num3 = num2;
                isNewRecord = false;
            }
            UnityEngine.Debug.Log("最高記録：[" + num3.ToString() + "]");
            hiRecordUIData.hiRecordAnchor.gameObject.SetActive(value: true);
            ResultGameDataParams.GetNowSceneType();
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.recordAnchor.gameObject.SetActive(value: true);
            SceneManager.SceneType nowSceneType = ResultGameDataParams.GetNowSceneType();
            if ((uint)(nowSceneType - 2) <= 9u) {
                hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.addText_pt.gameObject.SetActive(value: true);
            }
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.numbers.Set(num3);
            switch (num3.ToString().Length) {
                case 1:
                    hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.recordAnchor.SetLocalPositionX(34f);
                    break;
                case 2:
                    hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.recordAnchor.SetLocalPositionX(56f);
                    break;
                case 3:
                    hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.recordAnchor.SetLocalPositionX(81f);
                    break;
                case 4:
                    hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.recordAnchor.SetLocalPositionX(104f);
                    break;
            }
        }
        SetButtonPattern();
        SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_Ranking(lastResultGroupAnchor.gameObject.activeSelf);
        StartCoroutine(_ShowResult());
    }
    public void ShowResult_DoubleDecimalScore() {
        if (isResultOpen) {
            return;
        }
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_sports_day_final_result_1");
        isResultOpen = true;
        showResultType = ResultGameDataParams.ShowResultType.Record_DoubleDecimalScore;
        if (ResultGameDataParams.GetRecord().stringData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord());
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).stringData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord(_isGroup1Record: false));
        }
        InitShowResult();
        if (ResultGameDataParams.GetRecord().stringData != null) {
            for (int i = 0; i < rankGroupData[0].rankUIAnchor.Length; i++) {
                rankGroupData[0].rankUIAnchor[i].gameObject.SetActive(value: true);
            }
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).stringData != null) {
            for (int j = 0; j < rankGroupData[1].rankUIAnchor.Length; j++) {
                rankGroupData[1].rankUIAnchor[j].gameObject.SetActive(value: true);
            }
            lastResultGroupAnchor.gameObject.SetActive(value: true);
            scrollUIAnchor.gameObject.SetActive(value: true);
            SetLastResultData();
        }
        for (int k = 0; k < rankGroupRecords.Count; k++) {
            rankRecordCnt[k] = rankGroupRecords[k].stringData.Length;
            for (int l = 0; l < rankRecordCnt[k]; l++) {
                rankGroupData[k].rankUIData[l].GetRankUIData().rankAnchor.gameObject.SetActive(value: true);
                rankGroupData[k].rankUIData[l].GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.recordAnchor.gameObject.SetActive(value: true);
            }
            for (int m = 0; m < rankGroupRecords[k].stringData.Length; m++) {
                string[] array = rankGroupRecords[k].stringData[m].Split('.');
                rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.numbers_Decimal.SetZeroFillMode();
                if (rankGroupRecords[k].stringData[m] == "-1") {
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.numbers.SetNumbers("-1");
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.numbers_Decimal.SetNumbers("-1");
                } else {
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.numbers.SetNumbers(array[0]);
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.numbers_Decimal.SetNumbers(array[1]);
                }
                UnityEngine.Debug.Log((k + 1).ToString() + "組目" + (rankGroupRecords[k].rankNo[m] + 1).ToString() + "位：[" + rankGroupRecords[k].userTypeList[m].ToString() + "] 記録：[" + rankGroupRecords[k].stringData[m] + "]");
            }
            SetTeamData(k);
            SetGetPointData(k);
            SetEvaluationIcon(k);
        }
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
            string text = "";
            string text2 = "";
            string text3 = "";
            for (int n = 0; n < rankGroupRecords[0].userTypeList.Length; n++) {
                for (int num = 0; num < rankGroupRecords[0].userTypeList[n].userType.Length; num++) {
                    if (rankGroupRecords[0].userTypeList[n].userType[num] == ResultGameDataParams.UserType.PLAYER_1) {
                        text = rankGroupRecords[0].stringData[n];
                        break;
                    }
                }
            }
            if (float.Parse(text) > float.Parse(text2)) {
                text3 = text;
                isNewRecord = true;
            } else {
                text3 = text2;
                isNewRecord = false;
            }
            UnityEngine.Debug.Log("最高記録：[" + text3 + "]");
            hiRecordUIData.hiRecordAnchor.gameObject.SetActive(value: true);
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.recordAnchor.gameObject.SetActive(value: true);
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.numbers_Decimal.SetZeroFillMode();
            string[] array2 = text3.Split('.');
            if (text3 == "-1") {
                hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.numbers.SetNumbers("-1");
                hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.numbers_Decimal.SetNumbers("-1");
            } else {
                hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.numbers.SetNumbers(array2[0]);
                hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.numbers_Decimal.SetNumbers(array2[1]);
            }
            switch (array2[0].Length) {
                case 1:
                    hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.recordAnchor.SetLocalPositionX(-25.5f);
                    break;
                case 2:
                    hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.recordAnchor.SetLocalPositionX(0f);
                    break;
            }
        }
        SetButtonPattern();
        SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_Ranking(lastResultGroupAnchor.gameObject.activeSelf);
        StartCoroutine(_ShowResult());
    }
    public void ShowResult_DecimalScore() {
        if (isResultOpen) {
            return;
        }
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_sports_day_final_result_1");
        isResultOpen = true;
        showResultType = ResultGameDataParams.ShowResultType.Record_DecimalScore;
        if (ResultGameDataParams.GetRecord().stringData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord());
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).stringData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord(_isGroup1Record: false));
        }
        InitShowResult();
        if (ResultGameDataParams.GetRecord().stringData != null) {
            for (int i = 0; i < rankGroupData[0].rankUIAnchor.Length; i++) {
                rankGroupData[0].rankUIAnchor[i].gameObject.SetActive(value: true);
            }
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).stringData != null) {
            for (int j = 0; j < rankGroupData[1].rankUIAnchor.Length; j++) {
                rankGroupData[1].rankUIAnchor[j].gameObject.SetActive(value: true);
            }
            lastResultGroupAnchor.gameObject.SetActive(value: true);
            scrollUIAnchor.gameObject.SetActive(value: true);
            SetLastResultData();
        }
        for (int k = 0; k < rankGroupRecords.Count; k++) {
            rankRecordCnt[k] = rankGroupRecords[k].stringData.Length;
            for (int l = 0; l < rankRecordCnt[k]; l++) {
                rankGroupData[k].rankUIData[l].GetRankUIData().rankAnchor.gameObject.SetActive(value: true);
                rankGroupData[k].rankUIData[l].GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.recordAnchor.gameObject.SetActive(value: true);
                if (ResultGameDataParams.GetNowSceneType() == SceneManager.SceneType.ATTACK_BALL) {
                    rankGroupData[k].rankUIData[l].GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.addText_cm.gameObject.SetActive(value: true);
                }
            }
            for (int m = 0; m < rankGroupRecords[k].stringData.Length; m++) {
                string[] array = rankGroupRecords[k].stringData[m].Split('.');
                if (rankGroupRecords[k].stringData[m] == "-1") {
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.numbers.SetNumbers("-1");
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.numbers_Decimal.SetNumbers("-1");
                } else {
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.numbers.SetNumbers(array[0]);
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.numbers_Decimal.SetNumbers(array[1]);
                }
                UnityEngine.Debug.Log((k + 1).ToString() + "組目" + (rankGroupRecords[k].rankNo[m] + 1).ToString() + "位：[" + rankGroupRecords[k].userTypeList[m].ToString() + "] 記録：[" + array[0] + "." + array[1] + "]");
            }
            SetTeamData(k);
            SetGetPointData(k);
            SetEvaluationIcon(k);
        }
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
            string text = "";
            string text2 = "";
            string text3 = "";
            for (int n = 0; n < rankGroupRecords[0].userTypeList.Length; n++) {
                for (int num = 0; num < rankGroupRecords[0].userTypeList[n].userType.Length; num++) {
                    if (rankGroupRecords[0].userTypeList[n].userType[num] == ResultGameDataParams.UserType.PLAYER_1) {
                        text = rankGroupRecords[0].stringData[n];
                        break;
                    }
                }
            }
            ResultGameDataParams.GetNowSceneType();
            if (float.Parse(text) > float.Parse(text2)) {
                if (ResultGameDataParams.GetNowSceneType() == SceneManager.SceneType.ATTACK_BALL) {
                    SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Set(GS_Define.GameType.ATTACK_BALL, text);
                }
                text3 = text;
                isNewRecord = true;
            } else {
                text3 = text2;
                isNewRecord = false;
            }
            UnityEngine.Debug.Log("最高記録：[" + text3 + "]");
            hiRecordUIData.hiRecordAnchor.gameObject.SetActive(value: true);
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.recordAnchor.gameObject.SetActive(value: true);
            if (ResultGameDataParams.GetNowSceneType() == SceneManager.SceneType.ATTACK_BALL) {
                hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.addText_cm.gameObject.SetActive(value: true);
            }
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.numbers_Decimal.SetZeroFillMode();
            string[] array2 = text3.Split('.');
            if (text3 == "-1") {
                hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.numbers.SetNumbers("-1");
                hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.numbers_Decimal.SetNumbers("-1");
            } else {
                hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.numbers.SetNumbers(array2[0]);
                hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.numbers_Decimal.SetNumbers(array2[1]);
            }
            switch (array2[0].Length) {
                case 1:
                    hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.recordAnchor.SetLocalPositionX(-40f);
                    break;
                case 2:
                    hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.recordAnchor.SetLocalPositionX(-25.5f);
                    break;
                case 3:
                    hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.recordAnchor.SetLocalPositionX(0f);
                    break;
            }
        }
        SetButtonPattern();
        SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_Ranking(lastResultGroupAnchor.gameObject.activeSelf);
        StartCoroutine(_ShowResult());
    }
    public void ShowResult_Time() {
        if (isResultOpen) {
            return;
        }
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_sports_day_final_result_1");
        isResultOpen = true;
        showResultType = ResultGameDataParams.ShowResultType.Record_Time;
        if (ResultGameDataParams.GetRecord().floatData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord());
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).floatData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord(_isGroup1Record: false));
        }
        InitShowResult();
        if (ResultGameDataParams.GetRecord().floatData != null) {
            for (int i = 0; i < rankGroupData[0].rankUIAnchor.Length; i++) {
                rankGroupData[0].rankUIAnchor[i].gameObject.SetActive(value: true);
            }
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).floatData != null) {
            for (int j = 0; j < rankGroupData[1].rankUIAnchor.Length; j++) {
                rankGroupData[1].rankUIAnchor[j].gameObject.SetActive(value: true);
            }
            scrollUIAnchor.gameObject.SetActive(value: true);
            if (!SingletonCustom<GameSettingManager>.Instance.IsEightBattle) {
                lastResultGroupAnchor.gameObject.SetActive(value: true);
                SetLastResultData();
            }
        }
        for (int k = 0; k < rankGroupRecords.Count; k++) {
            rankRecordCnt[k] = rankGroupRecords[k].floatData.Length;
            for (int l = 0; l < rankRecordCnt[k]; l++) {
                rankGroupData[k].rankUIData[l].GetRankUIData().rankAnchor.gameObject.SetActive(value: true);
                rankGroupData[k].rankUIData[l].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.recordAnchor.gameObject.SetActive(value: true);
            }
            for (int m = 0; m < rankGroupRecords[k].floatData.Length; m++) {
                if (rankGroupRecords[k].floatData[m] == -1f) {
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.minutes.SetNumbers("-1");
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.second.SetNumbers("-1");
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.millSecond.SetNumbers("-1");
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.textTime.SetText("Winner");
                    UnityEngine.Debug.Log((k + 1).ToString() + "組目" + (rankGroupRecords[k].rankNo[m] + 1).ToString() + "位：[" + rankGroupRecords[k].userTypeList[m].ToString() + "] 記録なし");
                    continue;
                }
                if (rankGroupRecords[k].floatData[m] >= 600f) {
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.textTime.SetText("-------");
                    continue;
                }
                string convertRecordString = CalcManager.GetConvertRecordString((int)rankGroupRecords[k].userTypeList[m].userType[0]);
                int num = (int)rankGroupRecords[k].userTypeList[m].userType[0];
                UnityEngine.Debug.Log("playerNo : " + num.ToString() + " recordTimeStr " + convertRecordString);
                if (string.IsNullOrEmpty(convertRecordString)) {
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.textTime.SetText("-------");
                    continue;
                }
                string[] array = convertRecordString.Split(':');
                string[] array2 = array[1].Split('.');
                string numbers = array[0];
                string numbers2 = array2[0];
                string numbers3 = array2[1];
                rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.minutes.SetNumbers(numbers);
                rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.second.SetNumbers(numbers2);
                rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.millSecond.SetNumbers(numbers3);
                rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.textTime.SetText(convertRecordString);
            }
            SetTeamData(k);
            SetGetPointData(k);
            SetEvaluationIcon(k);
        }
        ResultGameDataParams.GetNowSceneType();
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
            float num2 = -1f;
            float num3 = -1f;
            float num4 = -1f;
            UnityEngine.Debug.Log("records" + rankGroupRecords.Count.ToString());
            UnityEngine.Debug.Log("Len" + rankGroupRecords[0].userTypeList.Length.ToString());
            for (int n = 0; n < rankGroupRecords[0].userTypeList.Length; n++) {
                UnityEngine.Debug.Log("Check" + n.ToString() + " " + rankGroupRecords[0].userTypeList[n].ToString());
                for (int num5 = 0; num5 < rankGroupRecords[0].userTypeList[n].userType.Length; num5++) {
                    if (rankGroupRecords[0].userTypeList[n].userType[num5] == ResultGameDataParams.UserType.PLAYER_1) {
                        num2 = rankGroupRecords[0].floatData[n];
                        break;
                    }
                }
            }
            if (SingletonCustom<GameSettingManager>.Instance.IsEightBattle) {
                for (int num6 = 0; num6 < rankGroupRecords[1].userTypeList.Length; num6++) {
                    for (int num7 = 0; num7 < rankGroupRecords[1].userTypeList[num6].userType.Length; num7++) {
                        if (rankGroupRecords[1].userTypeList[num6].userType[num7] == ResultGameDataParams.UserType.PLAYER_1) {
                            num2 = rankGroupRecords[1].floatData[num6];
                            break;
                        }
                    }
                }
            }
            num3 = CalcManager.ConvertRecordStringToTime(SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Get(ResultGameDataParams.GetNowSceneGameType(), RecordData.InitType.Time));
            UnityEngine.Debug.Log("currentHiRecord" + num3.ToString());
            if (num2 == -1f) {
                num4 = num3;
                isNewRecord = false;
            } else if (num2 < num3) {
                if (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup) {
                    SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.SetSix(ResultGameDataParams.GetNowSceneGameType(), CalcManager.GetConvertRecordString(0));
                } else {
                    SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Set(ResultGameDataParams.GetNowSceneGameType(), CalcManager.GetConvertRecordString(0));
                }
                num4 = num2;
                isNewRecord = true;
            } else {
                num4 = num3;
                isNewRecord = false;
            }
            hiRecordUIData.hiRecordAnchor.gameObject.SetActive(value: true);
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.recordAnchor.gameObject.SetActive(value: true);
            string text = isNewRecord ? CalcManager.GetConvertRecordString(0) : CalcManager.ConvertTimeToRecordString(num4);
            string[] array3 = text.Split(':');
            string[] array4 = array3[1].Split('.');
            string numbers4 = array3[0];
            string numbers5 = array4[0];
            string numbers6 = array4[1];
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.minutes.SetNumbers(numbers4);
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.second.SetNumbers(numbers5);
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.millSecond.SetNumbers(numbers6);
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.textTime.SetText(text);
            UnityEngine.Debug.Log("最高記録：[" + text + "]");
        }
        SetButtonPattern();
        SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_Ranking(lastResultGroupAnchor.gameObject.activeSelf);
        StartCoroutine(_ShowResult());
    }
    public void ShowResult_SecondTime() {
        if (isResultOpen) {
            return;
        }
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_sports_day_final_result_1");
        isResultOpen = true;
        showResultType = ResultGameDataParams.ShowResultType.Record_SecondTime;
        if (ResultGameDataParams.GetRecord().stringData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord());
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).stringData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord(_isGroup1Record: false));
        }
        InitShowResult();
        if (ResultGameDataParams.GetRecord().stringData != null) {
            for (int i = 0; i < rankGroupData[0].rankUIAnchor.Length; i++) {
                rankGroupData[0].rankUIAnchor[i].gameObject.SetActive(value: true);
            }
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).stringData != null) {
            for (int j = 0; j < rankGroupData[1].rankUIAnchor.Length; j++) {
                rankGroupData[1].rankUIAnchor[j].gameObject.SetActive(value: true);
            }
            lastResultGroupAnchor.gameObject.SetActive(value: true);
            scrollUIAnchor.gameObject.SetActive(value: true);
            SetLastResultData();
        }
        for (int k = 0; k < rankGroupRecords.Count; k++) {
            rankRecordCnt[k] = rankGroupRecords[k].stringData.Length;
            for (int l = 0; l < rankRecordCnt[k]; l++) {
                rankGroupData[k].rankUIData[l].GetRankUIData().rankAnchor.gameObject.SetActive(value: true);
                rankGroupData[k].rankUIData[l].GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.recordAnchor.gameObject.SetActive(value: true);
            }
            for (int m = 0; m < rankGroupRecords[k].stringData.Length; m++) {
                if (rankGroupRecords[k].stringData[m] == "-1") {
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.second.SetNumbers("-1");
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.millSecond.SetNumbers("-1");
                    UnityEngine.Debug.Log((k + 1).ToString() + "組目" + (rankGroupRecords[k].rankNo[m] + 1).ToString() + "位：[" + rankGroupRecords[k].userTypeList[m].ToString() + "] 記録なし");
                } else {
                    string[] array = rankGroupRecords[k].stringData[m].Split('.');
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.second.SetNumbers(array[0]);
                    rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.millSecond.SetNumbers(array[1]);
                    UnityEngine.Debug.Log((k + 1).ToString() + "組目" + (rankGroupRecords[k].rankNo[m] + 1).ToString() + "位：[" + rankGroupRecords[k].userTypeList[m].ToString() + "] 記録：[" + rankGroupRecords[k].stringData[m] + "]");
                }
            }
            SetTeamData(k);
            SetGetPointData(k);
            SetEvaluationIcon(k);
        }
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
            string text = "";
            string text2 = "";
            string text3 = "";
            for (int n = 0; n < rankGroupRecords[0].userTypeList.Length; n++) {
                for (int num = 0; num < rankGroupRecords[0].userTypeList[n].userType.Length; num++) {
                    if (rankGroupRecords[0].userTypeList[n].userType[num] == ResultGameDataParams.UserType.PLAYER_1) {
                        text = rankGroupRecords[0].stringData[n];
                        break;
                    }
                }
            }
            ResultGameDataParams.GetNowSceneType();
            if (float.Parse(text) == -1f) {
                text3 = text2;
                isNewRecord = false;
            } else if (float.Parse(text) < float.Parse(text2)) {
                ResultGameDataParams.GetNowSceneType();
                text3 = text;
                isNewRecord = true;
            } else {
                text3 = text2;
                isNewRecord = false;
            }
            hiRecordUIData.hiRecordAnchor.gameObject.SetActive(value: true);
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.recordAnchor.gameObject.SetActive(value: true);
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.millSecond.SetZeroFillMode();
            string[] array2 = text3.Split('.');
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.second.SetNumbers(array2[0]);
            hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.millSecond.SetNumbers(array2[1]);
            switch (array2[0].Length) {
                case 1:
                    hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.recordAnchor.SetLocalPositionX(-25.5f);
                    break;
                case 2:
                    hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.recordAnchor.SetLocalPositionX(0f);
                    break;
            }
            UnityEngine.Debug.Log("最高記録：[" + text3 + "]");
        }
        SetButtonPattern();
        SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_Ranking(lastResultGroupAnchor.gameObject.activeSelf);
        StartCoroutine(_ShowResult());
    }
    public void ShowResult_Tournament() {
        if (isResultOpen) {
            return;
        }
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_sports_day_final_result_1");
        isResultOpen = true;
        showResultType = ResultGameDataParams.ShowResultType.Record_Score_Tournament;
        if (ResultGameDataParams.GetRecord().intData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord());
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).intData != null) {
            rankGroupRecords.Add(ResultGameDataParams.GetRecord(_isGroup1Record: false));
        }
        InitShowResult();
        if (ResultGameDataParams.GetRecord().intData != null) {
            for (int i = 0; i < rankGroupData[0].rankUIAnchor.Length; i++) {
                rankGroupData[0].rankUIAnchor[i].gameObject.SetActive(value: true);
            }
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).intData != null) {
            for (int j = 0; j < rankGroupData[1].rankUIAnchor.Length; j++) {
                rankGroupData[1].rankUIAnchor[j].gameObject.SetActive(value: true);
            }
            lastResultGroupAnchor.gameObject.SetActive(value: true);
            scrollUIAnchor.gameObject.SetActive(value: true);
            SetLastResultData();
        }
        tournamentUIData.tournamentSwitchIcon.SetActive(value: true);
        tournamentUIData.tournamentAnchor.SetActive(value: true);
        for (int k = 0; k < rankGroupRecords.Count; k++) {
            rankRecordCnt[k] = rankGroupRecords[k].intData.Length;
            for (int l = 0; l < rankRecordCnt[k]; l++) {
                rankGroupData[k].rankUIData[l].GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.recordAnchor.gameObject.SetActive(value: true);
                rankGroupData[k].rankUIData[l].GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.addText_Win.gameObject.SetActive(value: true);
                rankGroupData[k].rankUIData[l].GetRankUIData().rankAnchor.gameObject.SetActive(value: true);
            }
            for (int m = 0; m < rankGroupRecords[k].intData.Length; m++) {
                switch (rankGroupRecords[k].rankNo[m]) {
                    case 0:
                        rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.numbers.Set(2);
                        break;
                    case 1:
                        rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.numbers.Set(1);
                        break;
                    case 2:
                        rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.numbers.Set(1);
                        break;
                    case 3:
                        rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.numbers.Set(0);
                        break;
                }
                rankGroupData[k].rankUIData[m].GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.recordAnchor.SetLocalPositionX(90f);
            }
            SetTeamData(k);
            SetGetPointData(k);
            SetEvaluationIcon(k);
        }
        int[] array = new int[2];
        int[] array2 = new int[2];
        int[] array3 = new int[4];
        string[] array4 = rankGroupRecords[0].tournamentMatchData[0].Split('-');
        array[0] = int.Parse(array4[0]);
        array[1] = int.Parse(array4[1]);
        array3[0] = int.Parse(array4[0]);
        array3[1] = int.Parse(array4[1]);
        array4 = rankGroupRecords[0].tournamentMatchData[1].Split('-');
        array2[0] = int.Parse(array4[0]);
        array2[1] = int.Parse(array4[1]);
        array3[2] = int.Parse(array4[0]);
        array3[3] = int.Parse(array4[1]);
        for (int n = 0; n < array3.Length; n++) {
            switch (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[array3[n] - 1][0]) {
                case 0:
                case 1:
                case 2:
                case 3:
                    tournamentUIData.playerNameSprites[n].gameObject.SetActive(value: true);
                    switch (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[array3[n] - 1][0]) {
                        case 0:
                            tournamentUIData.playerNameSprites[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_1p");
                            break;
                        case 1:
                            tournamentUIData.playerNameSprites[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_2p");
                            break;
                        case 2:
                            tournamentUIData.playerNameSprites[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_3p");
                            break;
                        case 3:
                            tournamentUIData.playerNameSprites[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_4p");
                            break;
                    }
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                    tournamentUIData.cpuNameSprites[n].gameObject.SetActive(value: true);
                    switch (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[array3[n] - 1][0]) {
                        case 6:
                            tournamentUIData.cpuNameSprites[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp1");
                            break;
                        case 7:
                            tournamentUIData.cpuNameSprites[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp2");
                            break;
                        case 8:
                            tournamentUIData.cpuNameSprites[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp3");
                            break;
                        case 9:
                            tournamentUIData.cpuNameSprites[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp4");
                            break;
                        case 10:
                            tournamentUIData.cpuNameSprites[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp5");
                            break;
                        case 11:
                            tournamentUIData.cpuNameSprites[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp6");
                            break;
                        case 12:
                            tournamentUIData.cpuNameSprites[n].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp7");
                            break;
                    }
                    break;
            }
        }
        UnityEngine.Debug.Log("左側：" + SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[array[0] - 1][0].ToString() + " - " + SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[array[1] - 1][0].ToString());
        UnityEngine.Debug.Log("右側：" + SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[array2[0] - 1][0].ToString() + " - " + SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[array2[1] - 1][0].ToString());
        for (int num = 0; num < rankGroupRecords[0].tournamentWinnerData.Length; num++) {
            switch (num) {
                case 0:
                    if (rankGroupRecords[0].tournamentWinnerData[num] + 1 == array[0]) {
                        SetTournamentLine(tournamentUIData.lineData_1stRound.left_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_1stRound_Lose.right_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_1stRound.common_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_1stRound_Lose.common_LineSprites);
                    } else {
                        SetTournamentLine(tournamentUIData.lineData_1stRound.right_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_1stRound_Lose.left_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_1stRound.common_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_1stRound_Lose.common_LineSprites);
                    }
                    break;
                case 1:
                    if (rankGroupRecords[0].tournamentWinnerData[num] + 1 == array2[0]) {
                        SetTournamentLine(tournamentUIData.lineData_2ndRound.left_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_2ndRound_Lose.right_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_2ndRound.common_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_2ndRound_Lose.common_LineSprites);
                    } else {
                        SetTournamentLine(tournamentUIData.lineData_2ndRound.right_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_2ndRound_Lose.left_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_2ndRound.common_LineSprites);
                        SetTournamentLine(tournamentUIData.lineData_2ndRound_Lose.common_LineSprites);
                    }
                    break;
                case 2:
                    for (int num4 = 0; num4 < array.Length; num4++) {
                        if (rankGroupRecords[0].tournamentWinnerData[num] + 1 == array[num4]) {
                            SetTournamentLine(tournamentUIData.lineData_Final_Lose.left_LineSprites);
                            SetTournamentLine(tournamentUIData.lineData_Final_Lose.common_LineSprites);
                            break;
                        }
                    }
                    for (int num5 = 0; num5 < array2.Length; num5++) {
                        if (rankGroupRecords[0].tournamentWinnerData[num] + 1 == array2[num5]) {
                            SetTournamentLine(tournamentUIData.lineData_Final_Lose.right_LineSprites);
                            SetTournamentLine(tournamentUIData.lineData_Final_Lose.common_LineSprites);
                            break;
                        }
                    }
                    break;
                case 3:
                    for (int num2 = 0; num2 < array.Length; num2++) {
                        if (rankGroupRecords[0].tournamentWinnerData[num] + 1 == array[num2]) {
                            SetTournamentLine(tournamentUIData.lineData_Final.left_LineSprites);
                            SetTournamentLine(tournamentUIData.lineData_Final.common_LineSprites);
                            break;
                        }
                    }
                    for (int num3 = 0; num3 < array2.Length; num3++) {
                        if (rankGroupRecords[0].tournamentWinnerData[num] + 1 == array2[num3]) {
                            SetTournamentLine(tournamentUIData.lineData_Final.right_LineSprites);
                            SetTournamentLine(tournamentUIData.lineData_Final.common_LineSprites);
                            break;
                        }
                    }
                    break;
            }
        }
        SetButtonPattern();
        SingletonCustom<ResultCharacterManager>.Instance.SetCharacter_Ranking(lastResultGroupAnchor.gameObject.activeSelf);
        StartCoroutine(_ShowResult());
    }
    private void SetTournamentLine(TournamentSheetData.LineSpriteDetailData[] _lineData) {
        for (int i = 0; i < _lineData.Length; i++) {
            if (_lineData[i].lineType == TournamentSheetData.LineType.Horizontal_Left || _lineData[i].lineType == TournamentSheetData.LineType.Horizontal_Right) {
                _lineData[i].lineSprite.transform.SetLocalScaleX(_lineData[i].defaultLineScale.x);
            } else if (_lineData[i].lineType == TournamentSheetData.LineType.Vertical) {
                _lineData[i].lineSprite.transform.SetLocalScaleY(_lineData[i].defaultLineScale.y);
            }
        }
    }
    private void SetGetPointData(int _groupNo) {
        for (int i = 0; i < rankGroupRecords[_groupNo].rankNo.Length; i++) {
            if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE) {
                ResultGameDataParams.GetNowSceneType();
                isShowPointData = false;
                continue;
            }
            isShowPointData = true;
            rankGroupData[_groupNo].rankUIData[i].GetRankUIData().pointNumbers.Set(rankGroupRecords[_groupNo].point[i]);
            rankGroupData[_groupNo].rankUIData[i].GetRankUIData().pointNumbers.gameObject.SetActive(value: true);
            rankGroupData[_groupNo].rankUIData[i].GetRankUIData().pointText.gameObject.SetActive(value: true);
        }
    }
    private void SetTeamData(int _groupNo) {
        int num = 0;
        int num2 = 0;
        for (int i = 0; i < rankGroupRecords[_groupNo].rankNo.Length; i++) {
            string text = "rank_" + (rankGroupRecords[_groupNo].rankNo[i] + 1).ToString();
            if (Localize_Define.Language != 0) {
                text = "en_" + text;
            }
            rankGroupData[_groupNo].rankUIData[i].GetRankUIData().rankCaption.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, text);
            if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[2].Count == 0 && SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[3].Count == 0) {
                for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++) {
                    for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j].Count; k++) {
                        if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][k] == (int)rankGroupRecords[_groupNo].userTypeList[i].userType[0]) {
                            rankGroupData[_groupNo].rankUIData[i].GetRankUIData().teamIcon.gameObject.SetActive(value: true);
                            switch (j) {
                                case 0:
                                    num++;
                                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().teamIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_A_team");
                                    break;
                                case 1:
                                    num2++;
                                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().teamIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_B_team");
                                    break;
                            }
                            break;
                        }
                    }
                }
            } else {
                rankGroupData[_groupNo].rankUIData[i].GetRankUIData().characterIcon.gameObject.SetActive(value: true);
                int num3 = rankGroupRecords[_groupNo].rankNo[i];
                if (SingletonCustom<GameSettingManager>.Instance.IsEightBattle) {
                    num3 /= 2;
                }
                UnityEngine.Debug.Log("userType:" + rankGroupRecords[_groupNo].userTypeList[i].userType[0].ToString());
                int num4 = (int)rankGroupRecords[_groupNo].userTypeList[i].userType[0];
                UnityEngine.Debug.Log("int:" + num4.ToString());
                int num5 = (int)rankGroupRecords[_groupNo].userTypeList[i].userType[0];
                if (num5 >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                    num5 = SingletonCustom<GameSettingManager>.Instance.PlayerNum + (num5 - 6);
                }
                UnityEngine.Debug.Log("idx:" + num5.ToString());
                switch (num5) {
                    case 0:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "1P_result_frame");
                        break;
                    case 1:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "2P_result_frame");
                        break;
                    case 2:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "3P_result_frame");
                        break;
                    case 3:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "4P_result_frame");
                        break;
                    case 4:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "5P_result_frame");
                        break;
                    case 6:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "6P_result_frame");
                        break;
                    case 5:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "6P_result_frame");
                        break;
                    case 7:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.ResultIcon, "6P_result_frame");
                        break;
                }
            }
            if (rankGroupRecords[_groupNo].userTypeList[i].userType.Length > 1) {
                ResultGameDataParams.UserType userType = rankGroupRecords[_groupNo].userTypeList[i].userType[0];
                ResultGameDataParams.UserType userType2 = rankGroupRecords[_groupNo].userTypeList[i].userType[1];
                if (userType > userType2) {
                    userType = rankGroupRecords[_groupNo].userTypeList[i].userType[1];
                    userType2 = rankGroupRecords[_groupNo].userTypeList[i].userType[0];
                }
                if (rankGroupRecords[_groupNo].userTypeList[i].userType.Length == 2) {
                    switch (userType) {
                        case ResultGameDataParams.UserType.PLAYER_1:
                            switch (userType2) {
                                case ResultGameDataParams.UserType.PLAYER_2:
                                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_1P_2P");
                                    break;
                                case ResultGameDataParams.UserType.PLAYER_3:
                                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_1P_3P");
                                    break;
                                case ResultGameDataParams.UserType.PLAYER_4:
                                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_1P_4P");
                                    break;
                            }
                            break;
                        case ResultGameDataParams.UserType.PLAYER_2:
                            switch (userType2) {
                                case ResultGameDataParams.UserType.PLAYER_3:
                                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_2P_3P");
                                    break;
                                case ResultGameDataParams.UserType.PLAYER_4:
                                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_2P_4P");
                                    break;
                            }
                            break;
                        case ResultGameDataParams.UserType.PLAYER_3:
                            if (userType2 == ResultGameDataParams.UserType.PLAYER_4) {
                                rankGroupData[_groupNo].rankUIData[i].GetRankUIData().playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_3P_4P");
                            }
                            break;
                    }
                } else {
                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_p_three");
                }
                rankGroupData[_groupNo].rankUIData[i].GetRankUIData().playerIcon.gameObject.SetActive(value: true);
            } else if (rankGroupRecords[_groupNo].userTypeList[i].userType[0] >= ResultGameDataParams.UserType.CPU_1) {
                switch (rankGroupRecords[_groupNo].userTypeList[i].userType[0]) {
                    case ResultGameDataParams.UserType.CPU_1:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp1");
                        break;
                    case ResultGameDataParams.UserType.CPU_2:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp2");
                        break;
                    case ResultGameDataParams.UserType.CPU_3:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp3");
                        break;
                    case ResultGameDataParams.UserType.CPU_4:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp4");
                        break;
                    case ResultGameDataParams.UserType.CPU_5:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp5");
                        break;
                    case ResultGameDataParams.UserType.CPU_6:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp6");
                        break;
                    case ResultGameDataParams.UserType.CPU_7:
                        rankGroupData[_groupNo].rankUIData[i].GetRankUIData().cpuIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_cp7");
                        break;
                }
                rankGroupData[_groupNo].rankUIData[i].GetRankUIData().cpuIcon.gameObject.SetActive(value: true);
            } else {
                rankGroupData[_groupNo].rankUIData[i].GetRankUIData().playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_" + ((int)(rankGroupRecords[_groupNo].userTypeList[i].userType[0] + 1)).ToString() + "p");
                rankGroupData[_groupNo].rankUIData[i].GetRankUIData().playerIcon.gameObject.SetActive(value: true);
            }
        }
    }
    private void SetEvaluationIcon(int _groupNo) {
        if (isShowPointData) {
            return;
        }
        for (int i = 0; i < rankGroupRecords[_groupNo].rankNo.Length; i++) {
            string str = "";
            if (Localize_Define.Language != 0) {
                str = "en";
            }
            if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
                ResultGameDataParams.GetNowSceneType();
                if (rankGroupRecords[_groupNo].rankNo[i] == 0) {
                    str += "_mark_00";
                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                } else if (rankGroupRecords[_groupNo].rankNo[i] == 1) {
                    str += "_mark_01";
                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                } else if (rankGroupRecords[_groupNo].rankNo[i] == 2) {
                    str += "_mark_02";
                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                } else {
                    str += "_mark_03";
                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                }
            } else if (SingletonCustom<GameSettingManager>.Instance.IsEightBattle) {
                if (rankGroupRecords[_groupNo].rankNo[i] == 0 || rankGroupRecords[_groupNo].rankNo[i] == 1) {
                    str += "_mark_00";
                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                } else if (rankGroupRecords[_groupNo].rankNo[i] == 2 || rankGroupRecords[_groupNo].rankNo[i] == 3) {
                    str += "_mark_01";
                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                } else if (rankGroupRecords[_groupNo].rankNo[i] == 4 || rankGroupRecords[_groupNo].rankNo[i] == 5) {
                    str += "_mark_02";
                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                } else {
                    str += "_mark_03";
                    rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                }
            } else if (rankGroupRecords[_groupNo].rankNo[i] == 0) {
                str += "_mark_00";
                rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
            } else if (rankGroupRecords[_groupNo].rankNo[i] == 1) {
                str += "_mark_01";
                rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
            } else if (rankGroupRecords[_groupNo].rankNo[i] == 2) {
                str += "_mark_02";
                rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
            } else {
                str += "_mark_03";
                rankGroupData[_groupNo].rankUIData[i].GetRankUIData().evaluationIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
            }
        }
    }
    private void SetLastResultData() {
        ResultGameDataParams.GetNowSceneType();
        totalRecord_TeamA = ResultGameDataParams.GetTeamTotalPoint(0);
        totalRecord_TeamB = ResultGameDataParams.GetTeamTotalPoint(1);
        scrollGroupNo.gameObject.SetActive(value: true);
        if (totalRecord_TeamA > totalRecord_TeamB) {
            lastResultUIData.winnerTeamNameSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_A_team_l");
            ResultGameDataParams.SetRankingPointData_Ranking8Numbers(_isWinner_TeamA: true, _isWinner_TeamB: false);
        } else if (totalRecord_TeamB > totalRecord_TeamA) {
            if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
                isLastResultLoseFlg = true;
                lastResultUIData.winnerTeamNameSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_A_team_l");
            } else {
                lastResultUIData.winnerTeamNameSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_B_team_l");
            }
            ResultGameDataParams.SetRankingPointData_Ranking8Numbers(_isWinner_TeamA: false, _isWinner_TeamB: true);
        } else {
            isLastResultDrawFlg = true;
            ResultGameDataParams.SetRankingPointData_Ranking8Numbers(_isWinner_TeamA: false, _isWinner_TeamB: false);
        }
        if (isLastResultLoseFlg) {
            lastResultUIData.defeatSprite.gameObject.SetActive(value: true);
        } else if (isLastResultDrawFlg) {
            lastResultUIData.drawSprite.gameObject.SetActive(value: true);
        } else {
            lastResultUIData.victorySprite.gameObject.SetActive(value: true);
        }
        SceneManager.SceneType nowSceneType = ResultGameDataParams.GetNowSceneType();
        if ((uint)(nowSceneType - 2) <= 9u) {
            lastResultUIData.rankingLastResultUIData.SetUIActive(_isActive: true, RankingLastResultRecordUIData.RecordType.ScoreFourDigit);
        } else {
            lastResultUIData.rankingLastResultUIData.SetUIActive(_isActive: true, RankingLastResultRecordUIData.RecordType.Score);
        }
        UnityEngine.Debug.Log("チ\u30fcムAの得点" + totalRecord_TeamA.ToString());
        UnityEngine.Debug.Log("チ\u30fcムBの得点" + totalRecord_TeamB.ToString());
        lastResultUIData.rankingLastResultUIData.SetScore(0, totalRecord_TeamA);
        lastResultUIData.rankingLastResultUIData.SetScore(1, totalRecord_TeamB);
    }
    private void InitShowResult() {
        ResultGameDataParams.SetResultMode(_isResult: true);
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE) {
            if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying("bgm_result_0")) {
                SingletonCustom<AudioManager>.Instance.BgmStop();
                SingletonCustom<AudioManager>.Instance.SeStop("se_hanabi");
                SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_result_0", _loop: true);
            }
        } else if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying("bgm_result_1")) {
            SingletonCustom<AudioManager>.Instance.BgmStop();
            SingletonCustom<AudioManager>.Instance.SeStop("se_hanabi");
            SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_result_1", _loop: true);
        }
        SetPhoto();
        resultWindowRoot.SetActive(value: true);
        InitResultAnimation();
    }
    private void InitResultAnimation() {
        isAnimation = true;
        resultWindowRoot.transform.SetLocalPositionY(defPosY_ResultWindow);
        for (int i = 0; i < arraySpFrame.Length; i++) {
            arraySpFrame[i].SetAlpha(0f);
        }
        scrollUIAnchor.gameObject.SetActive(value: false);
        UpdateScrollArrow();
        psEffectCracker.Stop();
        psEffectConfetti.Stop();
        switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.resultCharacterNo) {
            case 0:
                ResultGameDataParams.singleShowCharaNo = UnityEngine.Random.Range(0, 4);
                break;
            case 1:
                ResultGameDataParams.singleShowCharaNo = 0;
                break;
            case 2:
                ResultGameDataParams.singleShowCharaNo = 1;
                break;
            case 3:
                ResultGameDataParams.singleShowCharaNo = 2;
                break;
            case 4:
                ResultGameDataParams.singleShowCharaNo = 3;
                break;
        }
        for (int j = 0; j < rankGroupData[0].rankUIAnchor.Length; j++) {
            rankGroupData[0].rankUIAnchor[j].gameObject.SetActive(value: false);
            //rankGroupData[0].rankUIAnchor[j].SetLocalPositionX(0f);
        }
        for (int k = 0; k < rankGroupData[1].rankUIAnchor.Length; k++) {
            rankGroupData[1].rankUIAnchor[k].gameObject.SetActive(value: false);
            rankGroupData[1].rankUIAnchor[k].SetLocalPositionX(2000f);
        }
        lastResultGroupAnchor.gameObject.SetActive(value: false);
        lastResultGroupAnchor.SetLocalPositionX(4000f);
        for (int l = 0; l < rankGroupData.Length; l++) {
            for (int m = 0; m < rankGroupData[l].rankUIData.Length; m++) {
                rankGroupData[l].rankUIData[m].GetRankUIData().rankAnchor.gameObject.SetActive(value: false);
            }
        }
        for (int n = 0; n < rankGroupRecords.Count; n++) {
            for (int num = 0; num < rankGroupRecords[n].rankNo.Length; num++) {
                rankGroupData[n].rankUIData[num].GetRankUIData().crownIcon.transform.SetLocalScale(1f, 1f, 1f);
                rankGroupData[n].rankUIData[num].GetRankUIData().crownIcon.SetAlpha(0f);
                rankGroupData[n].rankUIData[num].GetRankUIData().characterIcon.gameObject.SetActive(value: false);
                rankGroupData[n].rankUIData[num].GetRankUIData().playerIcon.gameObject.SetActive(value: false);
                rankGroupData[n].rankUIData[num].GetRankUIData().cpuIcon.gameObject.SetActive(value: false);
                rankGroupData[n].rankUIData[num].GetRankUIData().evaluationIcon.gameObject.SetActive(value: false);
                if (rankGroupRecords[n].rankNo[num] == 0) {
                    rankGroupData[n].rankUIData[num].GetRankUIData().teamIcon.transform.localScale = Vector3.one * 1.5f;
                    rankGroupData[n].rankUIData[num].GetRankUIData().teamIcon.SetAlpha(0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().characterIcon.transform.localScale = Vector3.one * 1.5f;
                    rankGroupData[n].rankUIData[num].GetRankUIData().characterIcon.SetAlpha(0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().rankCaption.transform.localScale = Vector3.one * 1.5f;
                    rankGroupData[n].rankUIData[num].GetRankUIData().rankCaption.SetAlpha(0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().playerIcon.transform.localScale = Vector3.one * 1.5f;
                    rankGroupData[n].rankUIData[num].GetRankUIData().playerIcon.SetAlpha(0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().cpuIcon.transform.localScale = Vector3.one * 1.5f;
                    rankGroupData[n].rankUIData[num].GetRankUIData().cpuIcon.SetAlpha(0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().pointNumbers.transform.localScale = Vector3.one * 1.5f;
                    rankGroupData[n].rankUIData[num].GetRankUIData().pointNumbers.SetAlpha(0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().pointText.transform.localScale = Vector3.one * 1.5f;
                    rankGroupData[n].rankUIData[num].GetRankUIData().pointText.SetAlpha(0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().evaluationIcon.transform.localScale = Vector3.one * 1.5f;
                    rankGroupData[n].rankUIData[num].GetRankUIData().evaluationIcon.SetAlpha(0f);
                } else {
                    rankGroupData[n].rankUIData[num].GetRankUIData().teamIcon.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().playerIcon.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().characterIcon.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().rankCaption.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().cpuIcon.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().pointNumbers.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().pointText.transform.SetLocalScale(0f, 0f, 0f);
                    rankGroupData[n].rankUIData[num].GetRankUIData().evaluationIcon.transform.SetLocalScale(0f, 0f, 0f);
                }
                rankGroupData[n].rankUIData[num].GetRankUIData().pointNumbers.gameObject.SetActive(value: false);
                rankGroupData[n].rankUIData[num].GetRankUIData().pointText.gameObject.SetActive(value: false);
            }
        }
        survivalTimeTextObject.SetActive(value: false);
        amountOfWaterTextObject.SetActive(value: false);
        for (int num2 = 0; num2 < rankGroupRecords.Count; num2++) {
            for (int num3 = 0; num3 < rankGroupRecords[num2].rankNo.Length; num3++) {
                rankGroupData[num2].rankUIData[num3].GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.recordAnchor.gameObject.SetActive(value: false);
                rankGroupData[num2].rankUIData[num3].GetRecordUIData(RecordUIData.RecordType.Score).SetUIActive(_isActive: false);
                if (rankGroupRecords[num2].rankNo[num3] == 0) {
                    rankGroupData[num2].rankUIData[num3].GetRecordUIData(RecordUIData.RecordType.Score).SetUIScale(1.5f);
                    rankGroupData[num2].rankUIData[num3].GetRecordUIData(RecordUIData.RecordType.Score).SetUIAlpha(0f);
                } else {
                    rankGroupData[num2].rankUIData[num3].GetRecordUIData(RecordUIData.RecordType.Score).SetUIScale(0f);
                }
            }
        }
        for (int num4 = 0; num4 < rankGroupRecords.Count; num4++) {
            for (int num5 = 0; num5 < rankGroupRecords[num4].rankNo.Length; num5++) {
                rankGroupData[num4].rankUIData[num5].GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.recordAnchor.gameObject.SetActive(value: false);
                rankGroupData[num4].rankUIData[num5].GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).SetUIActive(_isActive: false);
                if (rankGroupRecords[num4].rankNo[num5] == 0) {
                    rankGroupData[num4].rankUIData[num5].GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).SetUIScale(1.5f);
                    rankGroupData[num4].rankUIData[num5].GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).SetUIAlpha(0f);
                } else {
                    rankGroupData[num4].rankUIData[num5].GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).SetUIScale(0f);
                }
            }
        }
        for (int num6 = 0; num6 < rankGroupRecords.Count; num6++) {
            for (int num7 = 0; num7 < rankGroupRecords[num6].rankNo.Length; num7++) {
                rankGroupData[num6].rankUIData[num7].GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.recordAnchor.gameObject.SetActive(value: false);
                rankGroupData[num6].rankUIData[num7].GetRecordUIData(RecordUIData.RecordType.DecimalScore).SetUIActive(_isActive: false);
                if (rankGroupRecords[num6].rankNo[num7] == 0) {
                    rankGroupData[num6].rankUIData[num7].GetRecordUIData(RecordUIData.RecordType.DecimalScore).SetUIScale(1.5f);
                    rankGroupData[num6].rankUIData[num7].GetRecordUIData(RecordUIData.RecordType.DecimalScore).SetUIAlpha(0f);
                } else {
                    rankGroupData[num6].rankUIData[num7].GetRecordUIData(RecordUIData.RecordType.DecimalScore).SetUIScale(0f);
                }
            }
        }
        for (int num8 = 0; num8 < rankGroupRecords.Count; num8++) {
            for (int num9 = 0; num9 < rankGroupRecords[num8].rankNo.Length; num9++) {
                rankGroupData[num8].rankUIData[num9].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.recordAnchor.gameObject.SetActive(value: false);
                if (rankGroupRecords[num8].rankNo[num9] == 0) {
                    rankGroupData[num8].rankUIData[num9].GetRecordUIData(RecordUIData.RecordType.Time).SetUIScale(1.5f);
                    rankGroupData[num8].rankUIData[num9].GetRecordUIData(RecordUIData.RecordType.Time).SetUIAlpha(0f);
                } else {
                    rankGroupData[num8].rankUIData[num9].GetRecordUIData(RecordUIData.RecordType.Time).SetUIScale(0f);
                }
            }
        }
        for (int num10 = 0; num10 < rankGroupRecords.Count; num10++) {
            for (int num11 = 0; num11 < rankGroupRecords[num10].rankNo.Length; num11++) {
                rankGroupData[num10].rankUIData[num11].GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.recordAnchor.gameObject.SetActive(value: false);
                if (rankGroupRecords[num10].rankNo[num11] == 0) {
                    rankGroupData[num10].rankUIData[num11].GetRecordUIData(RecordUIData.RecordType.SecondTime).SetUIScale(1.5f);
                    rankGroupData[num10].rankUIData[num11].GetRecordUIData(RecordUIData.RecordType.SecondTime).SetUIAlpha(0f);
                } else {
                    rankGroupData[num10].rankUIData[num11].GetRecordUIData(RecordUIData.RecordType.SecondTime).SetUIScale(0f);
                }
            }
        }
        for (int num12 = 0; num12 < rankGroupRecords.Count; num12++) {
            for (int num13 = 0; num13 < rankGroupRecords[num12].rankNo.Length; num13++) {
                rankGroupData[num12].rankUIData[num13].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.recordAnchor.gameObject.SetActive(value: false);
                if (rankGroupRecords[num12].rankNo[num13] == 0) {
                    rankGroupData[num12].rankUIData[num13].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).SetUIScale(1.5f);
                    rankGroupData[num12].rankUIData[num13].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).SetUIAlpha(0f);
                } else {
                    rankGroupData[num12].rankUIData[num13].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).SetUIScale(0f);
                }
            }
        }
        hiRecordUIData.hiRecordAnchor.gameObject.SetActive(value: false);
        hiRecordUIData.hiRecordAnchor.SetLocalScaleY(0f);
        hiRecordUIData.hiRecordCaption.transform.SetLocalScaleY(0f);
        hiRecordUIData.newRecordIcon.transform.SetLocalScale(0f, 0f, 0f);
        hiRecordUIData.newRecordIcon.gameObject.SetActive(value: false);
        hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.Score).scoreUIData.recordAnchor.gameObject.SetActive(value: false);
        hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.Score).SetUIActive(_isActive: false);
        hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).doubleDecimalScoreUIData.recordAnchor.gameObject.SetActive(value: false);
        hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DoubleDecimalScore).SetUIActive(_isActive: false);
        hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).decimalScoreUIData.recordAnchor.gameObject.SetActive(value: false);
        hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.DecimalScore).SetUIActive(_isActive: false);
        hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.recordAnchor.gameObject.SetActive(value: false);
        hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.SecondTime).secondTimeUIData.recordAnchor.gameObject.SetActive(value: false);
        hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.recordAnchor.gameObject.SetActive(value: false);
        hiRecordUIData.rankUIData.GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).SetUIActive(_isActive: false);
        lastResultUIData.winnerTeamNameSprite.gameObject.SetActive(value: true);
        lastResultUIData.winnerTeamNameSprite.SetAlpha(0f);
        lastResultUIData.winnerTeamNameSprite.transform.SetLocalScale(1f, 1f, 1f);
        lastResultUIData.drawSprite.gameObject.SetActive(value: false);
        lastResultUIData.drawSprite.SetAlpha(0f);
        lastResultUIData.drawSprite.transform.SetLocalScale(1f, 1f, 1f);
        lastResultUIData.defeatSprite.gameObject.SetActive(value: false);
        lastResultUIData.defeatSprite.SetAlpha(0f);
        lastResultUIData.defeatSprite.transform.SetLocalScale(1f, 1f, 1f);
        lastResultUIData.victorySprite.gameObject.SetActive(value: false);
        lastResultUIData.victorySprite.SetAlpha(0f);
        lastResultUIData.victorySprite.transform.SetLocalScale(1f, 1f, 1f);
        tournamentUIData.tournamentAnchor.SetActive(value: false);
        tournamentUIData.tournamentSheetAnchor.SetLocalScaleX(0f);
        tournamentUIData.tournamentSheetAnchor.SetLocalScaleY(0f);
        InitLine(tournamentUIData.lineData_1stRound);
        InitLine(tournamentUIData.lineData_2ndRound);
        InitLine(tournamentUIData.lineData_Final);
        InitLine(tournamentUIData.lineData_1stRound_Lose);
        InitLine(tournamentUIData.lineData_2ndRound_Lose);
        InitLine(tournamentUIData.lineData_Final_Lose);
        tournamentUIData.tournamentSwitchIcon.SetActive(value: false);
        for (int num14 = 0; num14 < tournamentUIData.playerNameSprites.Length; num14++) {
            tournamentUIData.playerNameSprites[num14].gameObject.SetActive(value: false);
        }
        for (int num15 = 0; num15 < tournamentUIData.cpuNameSprites.Length; num15++) {
            tournamentUIData.cpuNameSprites[num15].gameObject.SetActive(value: false);
        }
    }
    private void InitLine(TournamentSheetData.LineSpriteData _lineData) {
        for (int i = 0; i < _lineData.left_LineSprites.Length; i++) {
            if (_lineData.left_LineSprites[i].lineType == TournamentSheetData.LineType.Horizontal_Left || _lineData.left_LineSprites[i].lineType == TournamentSheetData.LineType.Horizontal_Right) {
                _lineData.left_LineSprites[i].defaultLineScale = _lineData.left_LineSprites[i].lineSprite.transform.localScale;
                _lineData.left_LineSprites[i].lineSprite.transform.SetLocalScaleX(0f);
            } else if (_lineData.left_LineSprites[i].lineType == TournamentSheetData.LineType.Vertical) {
                _lineData.left_LineSprites[i].defaultLineScale = _lineData.left_LineSprites[i].lineSprite.transform.localScale;
                _lineData.left_LineSprites[i].lineSprite.transform.SetLocalScaleY(0f);
            }
        }
        for (int j = 0; j < _lineData.right_LineSprites.Length; j++) {
            if (_lineData.right_LineSprites[j].lineType == TournamentSheetData.LineType.Horizontal_Left || _lineData.right_LineSprites[j].lineType == TournamentSheetData.LineType.Horizontal_Right) {
                _lineData.right_LineSprites[j].defaultLineScale = _lineData.right_LineSprites[j].lineSprite.transform.localScale;
                _lineData.right_LineSprites[j].lineSprite.transform.SetLocalScaleX(0f);
            } else if (_lineData.right_LineSprites[j].lineType == TournamentSheetData.LineType.Vertical) {
                _lineData.right_LineSprites[j].defaultLineScale = _lineData.right_LineSprites[j].lineSprite.transform.localScale;
                _lineData.right_LineSprites[j].lineSprite.transform.SetLocalScaleY(0f);
            }
        }
        for (int k = 0; k < _lineData.common_LineSprites.Length; k++) {
            if (_lineData.common_LineSprites[k].lineType == TournamentSheetData.LineType.Horizontal_Left || _lineData.common_LineSprites[k].lineType == TournamentSheetData.LineType.Horizontal_Right) {
                _lineData.common_LineSprites[k].defaultLineScale = _lineData.common_LineSprites[k].lineSprite.transform.localScale;
                _lineData.common_LineSprites[k].lineSprite.transform.SetLocalScaleX(0f);
            } else if (_lineData.common_LineSprites[k].lineType == TournamentSheetData.LineType.Vertical) {
                _lineData.common_LineSprites[k].defaultLineScale = _lineData.common_LineSprites[k].lineSprite.transform.localScale;
                _lineData.common_LineSprites[k].lineSprite.transform.SetLocalScaleY(0f);
            }
        }
    }
    private IEnumerator _ShowResult() {
        TouchPanelManager.Instance.SetTouchPanelEnable(false);
        Animation_ResultFan();
        yield return new WaitForSeconds(resultBackFanAnimationTime + 0.1f);
        StartCoroutine(Animation_Ranking(0));
        yield return new WaitForSeconds((resultRankingAnimationTime_Battle_Ranking + resultRankingAnimationWaitTime_Battle_Ranking) * (float)rankRecordCnt[0] + 0.1f);
        Animation_Ranking_FirstRank(0);
        yield return new WaitForSeconds(resultRankingAnimationTime_Battle_Ranking + 0.1f);
        if (rankGroupRecords.Count == 2) {
            yield return new WaitForSeconds(1f);
            StartCoroutine(RightScroll());
            yield return new WaitForSeconds(scrollAnimationTime + 0.1f);
            StartCoroutine(Animation_Ranking(1));
            yield return new WaitForSeconds((resultRankingAnimationTime_Battle_Ranking + resultRankingAnimationWaitTime_Battle_Ranking) * (float)rankRecordCnt[1] + 0.1f);
            Animation_Ranking_FirstRank(1);
            yield return new WaitForSeconds(resultRankingAnimationTime_Battle_Ranking + 0.1f);
            if (!SingletonCustom<GameSettingManager>.Instance.IsEightBattle) {
                yield return new WaitForSeconds(1f);
                StartCoroutine(RightScroll());
                yield return new WaitForSeconds(scrollAnimationTime + 0.1f);
                lastResultCoroutine = StartCoroutine(Animation_LastResult());
                yield return lastResultCoroutine;
            }
        }
        psEffectCracker.Play();
        for (int i = 0; i < 2; i++) {
            StartCoroutine(_PlayCrackerSE((float)i * 0.25f));
        }
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType != GameSettingManager.GameProgressType.ALL_SPORTS || ResultGameDataParams.IsLastPlayType()) {
            psEffectConfetti.Play();
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_win");
        if (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup) {
            LeanTween.value(base.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate (float _value) {
                for (int k = 0; k < arrayDecoStartSix.Length; k++) {
                    arrayDecoStartSix[k].SetAlpha(_value);
                }
            });
        } else {
            LeanTween.value(base.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate (float _value) {
                for (int j = 0; j < arrayDecoStartFour.Length; j++) {
                    arrayDecoStartFour[j].SetAlpha(_value);
                }
            });
        }
        SingletonCustom<ResultCharacterManager>.Instance.StartPhotoAnimation(delegate {
            switch (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType) {
                case GameSettingManager.GameProgressType.ALL_SPORTS:
                    if (!ResultGameDataParams.IsLastPlayType()) {
                        StartCoroutine(_PartyModeInterim());
                    } else {
                        Animation_OperationButton();
                    }
                    break;
                case GameSettingManager.GameProgressType.SINGLE:
                    Animation_OperationButton();
                    break;
            }
        });
    }
    private IEnumerator _PartyModeInterim() {
        yield return new WaitForSeconds(2.75f);
        LeanTween.value(1f, 0f, 0.5f).setOnUpdate(delegate (float _value) {
            for (int num17 = 0; num17 < rankGroupRecords[0].rankNo.Length; num17++) {
                int num18 = (int)rankGroupRecords[0].userTypeList[num17].userType[0];
                if (num18 >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                    num18 = SingletonCustom<GameSettingManager>.Instance.PlayerNum + (num18 - 6);
                }
                rankGroupData[0].rankUIData[num17].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.SetAlpha(_value);
                rankGroupData[0].rankUIData[num17].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.textTime.SetAlpha(_value);
                rankGroupData[0].rankUIData[num17].GetRankUIData().rankCaption.transform.SetLocalScale(_value, _value, _value);
            }
        });
        yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < arraySpFrame.Length; i++) {
            if (i >= arraySpInterim.Length) {
                break;
            }
            if (arraySpInterim[i] == null) {
                continue;
            }
            arraySpFrame[i].sprite = arraySpInterim[i];
        }
        LeanTween.value(0f, 1f, 0.45f).setOnUpdate(delegate (float _value) {
            for (int num15 = 0; num15 < rankGroupRecords[0].rankNo.Length; num15++) {
                int num16 = (int)rankGroupRecords[0].userTypeList[num15].userType[0];
                if (num16 >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                    num16 = SingletonCustom<GameSettingManager>.Instance.PlayerNum + (num16 - 6);
                }
                rankGroupData[0].rankUIData[num15].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.SetAlpha(_value);
                rankGroupData[0].rankUIData[num15].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.textTime.SetAlpha(_value);
            }
        });
        for (int i = 0; i < rankGroupRecords[0].rankNo.Length; i++) {
            int num = (int)rankGroupRecords[0].userTypeList[i].userType[0];
            if (num >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                num = SingletonCustom<GameSettingManager>.Instance.PlayerNum + (num - 6);
            }
            rankGroupData[0].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.SetText(SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(num).ToString());
            rankGroupData[0].rankUIData[i].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.textTime.SetText(SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(num).ToString());
        }
        LeanTween.moveLocalX(objPartyModeGameCntRoot, 1036f, 0.45f).setEaseOutCubic();
        textPartyModeGameCnt.SetText(SingletonCustom<GameSettingManager>.Instance.PlayKingTableCnt.ToString() + " / " + SingletonCustom<GameSettingManager>.Instance.SelectGameNum.ToString());
        yield return new WaitForSeconds(1f);
        for (int j = 0; j < rankGroupRecords[0].rankNo.Length; j++) {
            int num2 = (int)rankGroupRecords[0].userTypeList[j].userType[0];
            if (num2 >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                num2 = SingletonCustom<GameSettingManager>.Instance.PlayerNum + (num2 - 6);
            }
            rankGroupData[0].rankUIData[j].GetRankUIData().textAddScore.gameObject.SetActive(value: true);
            UnityEngine.Debug.Log("呼び出しteamNo：" + num2.ToString() + ":rank:" + rankGroupRecords[0].rankNo[j].ToString());
            int teamTotalPoint = ResultGameDataParams.GetTeamTotalPoint(rankGroupRecords[0].rankNo[j]);
            rankGroupData[0].rankUIData[j].GetRankUIData().textAddScore.SetText("+" + teamTotalPoint.ToString());
            rankGroupData[0].rankUIData[j].GetRankUIData().textAddScore.transform.SetLocalScale(0f, 0f, 0f);
            LeanTween.scale(rankGroupData[0].rankUIData[j].GetRankUIData().textAddScore.gameObject, Vector3.one, 0.35f);
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_puyon");
        yield return new WaitForSeconds(1.5f);
        for (int k = 0; k < 10; k++) {
            LeanTween.delayedCall((float)k * 0.1f, (Action)delegate {
                SingletonCustom<AudioManager>.Instance.SePlay("se_count_up", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
            });
        }
        SortData[] scoreData = new SortData[rankGroupRecords[0].rankNo.Length];
        SortData sortData = default(SortData);
        for (int l = 0; l < scoreData.Length; l++) {
            sortData.data1 = l;
            sortData.data2 = 0;
            scoreData[l] = sortData;
        }
        LeanTween.value(0f, 1f, 1f).setOnUpdate(delegate (float _value) {
            for (int num13 = 0; num13 < rankGroupRecords[0].rankNo.Length; num13++) {
                int num14 = (int)rankGroupRecords[0].userTypeList[num13].userType[0];
                if (num14 >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                    num14 = SingletonCustom<GameSettingManager>.Instance.PlayerNum + (num14 - 6);
                }
                int data = SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(num14) + (int)((float)ResultGameDataParams.GetTeamTotalPoint(rankGroupRecords[0].rankNo[num13]) * _value);
                rankGroupData[0].rankUIData[num13].GetRecordUIData(RecordUIData.RecordType.ScoreFourDigit).scoreFourDigitUIData.textNumbers.SetText(data.ToString());
                rankGroupData[0].rankUIData[num13].GetRecordUIData(RecordUIData.RecordType.Time).timeUIData.textTime.SetText(data.ToString());
                LeanTween.scale(rankGroupData[0].rankUIData[num13].GetRankUIData().textAddScore.gameObject, Vector3.zero, 0.35f);
                scoreData[num13].data1 = num14;
                scoreData[num13].data2 = data;
            }
        });
        yield return new WaitForSeconds(1.1f);
        scoreData.Sort((SortData c) => c.data2, (SortData c) => c.data1);
        int num3 = 0;
        int num4 = 1;
        int num5 = 0;
        Vector3[] movePos = new Vector3[scoreData.Length];
        for (int m = 0; m < scoreData.Length; m++) {
            if (m == 0) {
                num5 = scoreData[m].data2;
            } else {
                UnityEngine.Debug.Log("check:" + num5.ToString() + " data2:" + scoreData[m].data2.ToString());
                if (num5 <= scoreData[m].data2) {
                    num4++;
                } else {
                    num3 += num4;
                    num4 = 1;
                    num5 = scoreData[m].data2;
                }
            }
            for (int n = 0; n < rankGroupRecords[0].rankNo.Length; n++) {
                int num6 = (int)rankGroupRecords[0].userTypeList[n].userType[0];
                if (num6 >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                    num6 = SingletonCustom<GameSettingManager>.Instance.PlayerNum + (num6 - 6);
                }
                if (num6 == scoreData[m].data1) {
                    string text = "rank_" + (num3 + 1).ToString();
                    if (Localize_Define.Language != 0) {
                        text = "en_" + text;
                    }
                    rankGroupData[0].rankUIData[n].GetRankUIData().rankCaption.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, text);
                }
            }
            movePos[m] = rankGroupData[0].rankUIData[m].gameObject.transform.position;
        }
        LeanTween.value(0f, 1f, 0.5f).setOnUpdate(delegate (float _value) {
            for (int num11 = 0; num11 < rankGroupRecords[0].rankNo.Length; num11++) {
                int num12 = (int)rankGroupRecords[0].userTypeList[num11].userType[0];
                if (num12 >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                    num12 = SingletonCustom<GameSettingManager>.Instance.PlayerNum + (num12 - 6);
                }
                rankGroupData[0].rankUIData[num11].GetRankUIData().rankCaption.transform.SetLocalScale(_value, _value, _value);
            }
        });
        yield return new WaitForSeconds(0.5f);
        bool flag = false;
        for (int num7 = 0; num7 < rankGroupRecords[0].rankNo.Length; num7++) {
            int num8 = (int)rankGroupRecords[0].userTypeList[num7].userType[0];
            if (num8 >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                num8 = SingletonCustom<GameSettingManager>.Instance.PlayerNum + (num8 - 6);
            }
            UnityEngine.Debug.Log("[i]" + num7.ToString() + " data1:" + scoreData[num7].data1.ToString());
            int num9 = 0;
            for (int num10 = 0; num10 < scoreData.Length; num10++) {
                if (scoreData[num10].data1 == num8) {
                    num9 = num10;
                }
            }
            if (num9 != num7) {
                flag = true;
            }
            LeanTween.move(rankGroupData[0].rankUIData[num7].gameObject, movePos[num9], 0.75f).setEaseOutCubic();
        }
        if (flag) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_result_slide");
        }
        psEffectConfetti.Play();
        yield return new WaitForSeconds(0.75f);
        Animation_OperationButton();
    }
    private void Animation_ResultDialog_SlideIn() {
        LeanTween.moveLocalY(resultWindowRoot, defPosY_ResultWindow, resultWindowAnimationTime).setEaseOutBack();
    }
    private void Animation_ResultFan() {
        for (int i = 0; i < arraySpFrame.Length; i++) {
            StartCoroutine(SetAlphaColor(arraySpFrame[i], resultBackFanAnimationTime));
        }
    }
    private void Animation_HighRecord() {
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_puyon");
        LeanTween.scaleY(hiRecordUIData.hiRecordAnchor.gameObject, 1f, resultHiRecordAnimationTime).setEaseOutElastic();
        LeanTween.scaleY(hiRecordUIData.hiRecordCaption.gameObject, 1f, resultHiRecordAnimationTime).setEaseOutElastic();
    }
    private void Animation_NewRecord() {
        hiRecordUIData.newRecordIcon.gameObject.SetActive(value: true);
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_new_record");
        LeanTween.scale(hiRecordUIData.newRecordIcon.gameObject, Vector3.one, resultNewRecordAnimationTime).setEaseOutBack();
    }
    private IEnumerator Animation_Ranking(int _showGroupNo) {
        StartCoroutine(Animation_Common(_showGroupNo, resultRankingAnimationTime_Battle_Ranking + resultRankingAnimationWaitTime_Battle_Ranking));
        RecordUIData.RecordType _recordType = RecordUIData.RecordType.Score;
        switch (showResultType) {
            case ResultGameDataParams.ShowResultType.Record_Score:
                _recordType = RecordUIData.RecordType.Score;
                break;
            case ResultGameDataParams.ShowResultType.Record_Score_Four_Digit:
                _recordType = RecordUIData.RecordType.ScoreFourDigit;
                break;
            case ResultGameDataParams.ShowResultType.Record_DoubleDecimalScore:
                _recordType = RecordUIData.RecordType.DoubleDecimalScore;
                break;
            case ResultGameDataParams.ShowResultType.Record_DecimalScore:
                _recordType = RecordUIData.RecordType.DecimalScore;
                break;
            case ResultGameDataParams.ShowResultType.Record_Time:
                _recordType = RecordUIData.RecordType.Time;
                break;
            case ResultGameDataParams.ShowResultType.Record_SecondTime:
                _recordType = RecordUIData.RecordType.SecondTime;
                break;
            case ResultGameDataParams.ShowResultType.Record_Score_Tournament:
                _recordType = RecordUIData.RecordType.Score;
                break;
        }
        for (int i = rankGroupRecords[_showGroupNo].rankNo.Length - 1; i >= 0; i--) {
            if (rankGroupRecords[_showGroupNo].rankNo[i] == 0) {
                rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(_recordType).Animation_Scaling(resultRankingAnimationTime_Battle_Ranking);
                rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(_recordType).Animation_Fade(resultRankingAnimationTime_Battle_Ranking);
            } else {
                rankGroupData[_showGroupNo].rankUIData[i].GetRecordUIData(_recordType).Animation_Scaling(resultRankingAnimationTime_Battle_Ranking);
            }
            yield return new WaitForSeconds(resultRankingAnimationTime_Battle_Ranking + resultRankingAnimationWaitTime_Battle_Ranking);
        }
    }
    private IEnumerator Animation_Common(int _showGroupNo, float _delay) {
        for (int i = rankGroupRecords[_showGroupNo].rankNo.Length - 1; i >= 0; i--) {
            if (rankGroupRecords[_showGroupNo].rankNo[i] == 0) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_result_ranking_1st_win");
                if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().teamIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().teamIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().teamIcon, resultRankingAnimationTime_Battle_Ranking));
                }
                if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().characterIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().characterIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().rankCaption.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().characterIcon, resultRankingAnimationTime_Battle_Ranking));
                    StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().rankCaption, resultRankingAnimationTime_Battle_Ranking));
                }
                if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().playerIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().playerIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().playerIcon, resultRankingAnimationTime_Battle_Ranking));
                }
                if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().cpuIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().cpuIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().cpuIcon, resultRankingAnimationTime_Battle_Ranking));
                }
                if (isShowPointData) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().pointNumbers.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().pointNumbers.GetArraySpriteNumbers(), resultRankingAnimationTime_Battle_Ranking));
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().pointText.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().pointText, resultRankingAnimationTime_Battle_Ranking));
                } else {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().evaluationIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().evaluationIcon, resultRankingAnimationTime_Battle_Ranking));
                }
            } else {
                SingletonCustom<AudioManager>.Instance.SePlay("se_result_puyon");
                if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().teamIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().teamIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                }
                if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().characterIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().characterIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().rankCaption.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                }
                if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().playerIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().playerIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                }
                if (rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().cpuIcon.gameObject.activeSelf) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().cpuIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                }
                if (isShowPointData) {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().pointNumbers.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().pointText.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                } else {
                    LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[i].GetRankUIData().evaluationIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking);
                }
            }
            yield return new WaitForSeconds(_delay);
        }
    }
    private void Animation_Ranking_FirstRank(int _showGroupNo) {
        rank1stCnt = 0;
        for (int i = 0; i < rankGroupData[_showGroupNo].rankUIData.Length; i++) {
            if (rankGroupData[_showGroupNo].rankUIData[i].gameObject.activeSelf) {
                rank1stCnt += ResultGameDataParams.GetRankPlayerNo(i, 0, (_showGroupNo == 0) ? true : false).Length;
                rank1stCnt += ResultGameDataParams.GetFirstRankCPUNo(i, 0, (_showGroupNo == 0) ? true : false).Length;
            }
        }
        for (int j = 0; j < rank1stCnt; j++) {
            UnityEngine.Debug.Log("rank1stCnt:" + rank1stCnt.ToString());
            UnityEngine.Debug.Log("rankUIData:" + rankGroupData[_showGroupNo].rankUIData.Length.ToString());
            SingletonCustom<GameSettingManager>.Instance.DebugLogPlayerGroupList();
            LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[j].GetRankUIData().crownIcon.gameObject, Vector3.one, resultRankingAnimationTime_Battle_Ranking).setEaseLinear();
            UnityEngine.Object.Destroy(rankGroupData[_showGroupNo].rankUIData[j].GetRankUIData().crownIcon.GetComponent<SpriteShiningEffect>());
            StartCoroutine(SetAlphaColor(rankGroupData[_showGroupNo].rankUIData[j].GetRankUIData().crownIcon, resultRankingAnimationTime_Battle_Ranking));
            rankGroupData[_showGroupNo].rankUIData[j].GetRankUIData().glitterSpriteEffect.StartGlitterAnimation();
            if (rankGroupData[_showGroupNo].rankUIData[j].GetRankUIData().playerIcon.gameObject.activeSelf) {
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[j].GetRankUIData().playerIcon.gameObject, Vector3.one, resultFirstRankPlayerIconAnimationTime_Battle_Ranking).setEaseLinear().setLoopPingPong();
            }
            if (rankGroupData[_showGroupNo].rankUIData[j].GetRankUIData().cpuIcon.gameObject.activeSelf) {
                LeanTween.scale(rankGroupData[_showGroupNo].rankUIData[j].GetRankUIData().cpuIcon.gameObject, Vector3.one, resultFirstRankPlayerIconAnimationTime_Battle_Ranking).setEaseLinear().setLoopPingPong();
            }
        }
    }
    private IEnumerator Animation_LastResult() {
        SceneManager.SceneType nowSceneType = ResultGameDataParams.GetNowSceneType();
        if ((uint)(nowSceneType - 2) <= 9u) {
            lastResultUIData.rankingLastResultUIData.Animation_Fade(lastResultAnimationTime, RankingLastResultRecordUIData.RecordType.ScoreFourDigit);
            lastResultUIData.rankingLastResultUIData.Animation_Scaling(lastResultAnimationTime, RankingLastResultRecordUIData.RecordType.ScoreFourDigit);
        } else {
            lastResultUIData.rankingLastResultUIData.Animation_Fade(lastResultAnimationTime, RankingLastResultRecordUIData.RecordType.Score);
            lastResultUIData.rankingLastResultUIData.Animation_Scaling(lastResultAnimationTime, RankingLastResultRecordUIData.RecordType.Score);
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_puyon");
        yield return new WaitForSeconds(lastResultAnimationTime + 0.1f);
        if (isLastResultLoseFlg) {
            LeanTween.scale(lastResultUIData.winnerTeamNameSprite.gameObject, Vector3.one, lastResultAnimationTime).setEaseOutBack();
            StartCoroutine(SetAlphaColor(lastResultUIData.winnerTeamNameSprite, lastResultAnimationTime));
            LeanTween.scale(lastResultUIData.defeatSprite.gameObject, Vector3.one, lastResultAnimationTime).setEaseOutBack();
            StartCoroutine(SetAlphaColor(lastResultUIData.defeatSprite, lastResultAnimationTime));
        } else if (isLastResultDrawFlg) {
            LeanTween.scale(lastResultUIData.drawSprite.gameObject, Vector3.one, lastResultAnimationTime).setEaseOutBack();
            StartCoroutine(SetAlphaColor(lastResultUIData.drawSprite, lastResultAnimationTime));
        } else {
            LeanTween.scale(lastResultUIData.winnerTeamNameSprite.gameObject, Vector3.one, lastResultAnimationTime).setEaseOutBack();
            StartCoroutine(SetAlphaColor(lastResultUIData.winnerTeamNameSprite, lastResultAnimationTime));
            LeanTween.scale(lastResultUIData.victorySprite.gameObject, Vector3.one, lastResultAnimationTime).setEaseOutBack();
            StartCoroutine(SetAlphaColor(lastResultUIData.victorySprite, lastResultAnimationTime));
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_view_score");
        yield return new WaitForSeconds(lastResultAnimationTime + 0.1f);
    }
    private IEnumerator _PlayCrackerSE(float _waitTime) {
        yield return new WaitForSeconds(_waitTime);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cracker");
    }
    private void Animation_OperationButton() {
        if (SingletonCustom<CommonNotificationManager>.Instance != null) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGetTrophy(null);
        }
        if (buttonPattern_Scroll_Retry_GameSelect.IsActive()) {
            buttonPattern_Scroll_Retry_GameSelect.ButtonAnimation(resultButtonAnimationTime, OnCompleteAllResultAnimation);
        } else if (buttonPattern_Retry_GameSelect.IsActive()) {
            buttonPattern_Retry_GameSelect.ButtonAnimation(resultButtonAnimationTime, OnCompleteAllResultAnimation);
        } else if (buttonPattern_Program_Details_Next.IsActive()) {
            buttonPattern_Program_Details_Next.ButtonAnimation(resultButtonAnimationTime, OnCompleteAllResultAnimation);
        } else if (buttonPattern_Scroll_Next.IsActive()) {
            buttonPattern_Scroll_Next.ButtonAnimation(resultButtonAnimationTime, OnCompleteAllResultAnimation);
        } else if (buttonPattern_Scroll_Program_Details_Next.IsActive()) {
            buttonPattern_Scroll_Program_Details_Next.ButtonAnimation(resultButtonAnimationTime, OnCompleteAllResultAnimation);
        } else if (buttonPattern_Next.IsActive()) {
            buttonPattern_Next.ButtonAnimation(resultButtonAnimationTime, OnCompleteAllResultAnimation);
        }
    }
    private void OnCompleteAllResultAnimation() {
        isAnimation = false;
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.ALL_SPORTS) {
            for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.TeamNum; i++) {
                UnityEngine.Debug.Log("idx:" + SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType).ToString());
                int num = (int)rankGroupRecords[0].userTypeList[i].userType[0];
                if (num >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                    num = SingletonCustom<GameSettingManager>.Instance.PlayerNum + (num - 6);
                }
                UnityEngine.Debug.Log("加算:[" + i.ToString() + "]:" + ResultGameDataParams.GetTeamTotalPoint(rankGroupRecords[0].rankNo[i]).ToString() + " teamNo:" + num.ToString() + " rank:" + rankGroupRecords[0].rankNo[i].ToString());
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(num, ResultGameDataParams.GetTeamTotalPoint(rankGroupRecords[0].rankNo[i])));
            }
            SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Sort((GameSettingManager.TeamData a, GameSettingManager.TeamData b) => b.score - a.score);
        }
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            for (int j = 0; j < rankGroupRecords[0].rankNo.Length; j++) {
                for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; k++) {
                    for (int l = 0; l < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[k].Count; l++) {
                        if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[k][l] == (int)rankGroupRecords[0].userTypeList[j].userType[0]) {
                            ResultGameDataParams.SetPlayKingRankData(k, rankGroupRecords[0].rankNo[j]);
                            break;
                        }
                    }
                }
            }
            return;
        }
        List<int> list = new List<int>();
        List<int> list2 = new List<int>();
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).intData != null || ResultGameDataParams.GetRecord(_isGroup1Record: false).floatData != null || ResultGameDataParams.GetRecord(_isGroup1Record: false).stringData != null) {
            ResultGameDataParams.GetNowSceneType();
            if (totalRecord_TeamA > totalRecord_TeamB) {
                ResultGameDataParams.SetPlayKingRankData(0, 0);
                ResultGameDataParams.SetPlayKingRankData(1, 1);
            } else if (totalRecord_TeamA < totalRecord_TeamB) {
                ResultGameDataParams.SetPlayKingRankData(0, 1);
                ResultGameDataParams.SetPlayKingRankData(1, 0);
            } else {
                ResultGameDataParams.SetPlayKingRankData(0, 0);
                ResultGameDataParams.SetPlayKingRankData(1, 0);
            }
            return;
        }
        for (int m = 0; m < rankGroupRecords[0].rankNo.Length; m++) {
            for (int n = 0; n < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; n++) {
                for (int num2 = 0; num2 < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[n].Count; num2++) {
                    if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[n][num2] == (int)rankGroupRecords[0].userTypeList[m].userType[0]) {
                        rankGroupData[0].rankUIData[m].GetRankUIData().teamIcon.gameObject.SetActive(value: true);
                        switch (n) {
                            case 0:
                                list.Add(rankGroupRecords[0].rankNo[m]);
                                break;
                            case 1:
                                list2.Add(rankGroupRecords[0].rankNo[m]);
                                break;
                        }
                        break;
                    }
                }
            }
        }
        CalcManager.QuickSort(list.ToArray(), _isAscendingOrder: true);
        CalcManager.QuickSort(list2.ToArray(), _isAscendingOrder: true);
        if (list[0] < list2[0]) {
            ResultGameDataParams.SetPlayKingRankData(0, 0);
            ResultGameDataParams.SetPlayKingRankData(1, 1);
        } else if (list[0] > list2[0]) {
            ResultGameDataParams.SetPlayKingRankData(0, 1);
            ResultGameDataParams.SetPlayKingRankData(1, 0);
        } else {
            ResultGameDataParams.SetPlayKingRankData(0, 0);
            ResultGameDataParams.SetPlayKingRankData(1, 0);
        }
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
    public void ClickModeSelectButton() {
        if (SingletonCustom<SceneManager>.Instance != null && !SingletonCustom<SceneManager>.Instance.GetFadeFlg()) {
            SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.MAIN);
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        }
    }
    public void ClickRetryButton() {
        if (SingletonCustom<SceneManager>.Instance != null && !SingletonCustom<SceneManager>.Instance.GetFadeFlg()) {
            SingletonCustom<SceneManager>.Instance.NextScene(SingletonCustom<SceneManager>.Instance.GetNowScene());
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        }
    }
    private void ClickTournamentSheetSwitch() {
        if (tournamentUIData.tournamentSwitchIcon.activeSelf && !isAnimationTournamentSheet) {
            isAnimationTournamentSheet = true;
            if (tournamentUIData.tournamentSheetAnchor.localScale.x == 1f) {
                LeanTween.scaleX(tournamentUIData.tournamentSheetAnchor.gameObject, 0f, resultRankingAnimationTime_Battle_Ranking);
                LeanTween.scaleY(tournamentUIData.tournamentSheetAnchor.gameObject, 0f, resultRankingAnimationTime_Battle_Ranking).setOnComplete((Action)delegate {
                    isAnimationTournamentSheet = false;
                });
            } else {
                LeanTween.scaleX(tournamentUIData.tournamentSheetAnchor.gameObject, 1f, resultRankingAnimationTime_Battle_Ranking);
                LeanTween.scaleY(tournamentUIData.tournamentSheetAnchor.gameObject, 1f, resultRankingAnimationTime_Battle_Ranking).setOnComplete((Action)delegate {
                    isAnimationTournamentSheet = false;
                });
            }
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        }
    }
    private void ClickResultDetailsButton() {
        if (SingletonCustom<SceneManager>.Instance != null && !SingletonCustom<SceneManager>.Instance.GetFadeFlg() && !SingletonCustom<SportsDayPoint>.Instance.IsShow) {
            SingletonCustom<SportsDayPoint>.Instance.Show();
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        }
    }
    public void ClickNextButton() {
        if (SingletonCustom<SceneManager>.Instance != null && !SingletonCustom<SceneManager>.Instance.GetFadeFlg() && !SingletonCustom<SportsDayPoint>.Instance.IsShow) {
            SingletonCustom<SceneManager>.Instance.NextScene(SingletonCustom<GameSettingManager>.Instance.NextTable());
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        }
    }
    private void ClickScrollLeftButton() {
        if (SingletonCustom<GameSettingManager>.Instance.IsEightBattle) {
            StartCoroutine(LeftScrollLoop());
        } else {
            if (currentShowGroupType == PageType.Page_1) {
                return;
            }
            StartCoroutine(LeftScroll());
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
    }
    private void ClickScrollRightButton() {
        if (SingletonCustom<GameSettingManager>.Instance.IsEightBattle) {
            StartCoroutine(RightScrollLoop());
        } else {
            if (currentShowGroupType == PageType.Final) {
                return;
            }
            StartCoroutine(RightScroll());
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
    }
    private IEnumerator LeftScroll() {
        if (currentShowGroupType != 0) {
            isScrollAnimation = true;
            for (int i = 0; i < rankGroupData[0].rankUIAnchor.Length; i++) {
                LeanTween.moveLocalX(rankGroupData[0].rankUIAnchor[i].gameObject, rankGroupData[0].rankUIAnchor[i].localPosition.x + scrollOffset, scrollAnimationTime);
            }
            for (int j = 0; j < rankGroupData[1].rankUIAnchor.Length; j++) {
                LeanTween.moveLocalX(rankGroupData[1].rankUIAnchor[j].gameObject, rankGroupData[1].rankUIAnchor[j].localPosition.x + scrollOffset, scrollAnimationTime);
            }
            LeanTween.moveLocalX(lastResultGroupAnchor.gameObject, lastResultGroupAnchor.localPosition.x + scrollOffset, scrollAnimationTime);
            currentShowGroupType--;
            UpdateScrollArrow();
            SingletonCustom<AudioManager>.Instance.SePlay("se_result_scroll");
            yield return new WaitForSeconds(scrollAnimationTime + 0.1f);
            isScrollAnimation = false;
        }
    }
    private IEnumerator RightScroll() {
        if (currentShowGroupType != PageType.Final) {
            isScrollAnimation = true;
            for (int i = 0; i < rankGroupData[0].rankUIAnchor.Length; i++) {
                LeanTween.moveLocalX(rankGroupData[0].rankUIAnchor[i].gameObject, rankGroupData[0].rankUIAnchor[i].localPosition.x - scrollOffset, scrollAnimationTime);
            }
            for (int j = 0; j < rankGroupData[1].rankUIAnchor.Length; j++) {
                LeanTween.moveLocalX(rankGroupData[1].rankUIAnchor[j].gameObject, rankGroupData[1].rankUIAnchor[j].localPosition.x - scrollOffset, scrollAnimationTime);
            }
            LeanTween.moveLocalX(lastResultGroupAnchor.gameObject, lastResultGroupAnchor.localPosition.x - scrollOffset, scrollAnimationTime);
            currentShowGroupType++;
            UpdateScrollArrow();
            SingletonCustom<AudioManager>.Instance.SePlay("se_result_scroll");
            yield return new WaitForSeconds(scrollAnimationTime + 0.1f);
            isScrollAnimation = false;
        }
    }
    private IEnumerator LeftScrollLoop() {
        isScrollAnimation = true;
        for (int i = 0; i < rankGroupData[0].rankUIAnchor.Length; i++) {
            if (currentShowGroupType == PageType.Page_2) {
                rankGroupData[0].rankUIAnchor[i].SetLocalPositionX(0f - scrollOffset);
            }
            LeanTween.moveLocalX(rankGroupData[0].rankUIAnchor[i].gameObject, rankGroupData[0].rankUIAnchor[i].localPosition.x + scrollOffset, scrollAnimationTime);
        }
        for (int j = 0; j < rankGroupData[1].rankUIAnchor.Length; j++) {
            if (currentShowGroupType == PageType.Page_1) {
                rankGroupData[1].rankUIAnchor[j].SetLocalPositionX(0f - scrollOffset);
            }
            LeanTween.moveLocalX(rankGroupData[1].rankUIAnchor[j].gameObject, rankGroupData[1].rankUIAnchor[j].localPosition.x + scrollOffset, scrollAnimationTime);
        }
        LeanTween.moveLocalX(lastResultGroupAnchor.gameObject, lastResultGroupAnchor.localPosition.x + scrollOffset, scrollAnimationTime);
        if (currentShowGroupType == PageType.Page_1) {
            currentShowGroupType = PageType.Page_2;
        } else {
            currentShowGroupType = PageType.Page_1;
        }
        UpdateScrollArrow();
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_scroll");
        yield return new WaitForSeconds(scrollAnimationTime + 0.1f);
        isScrollAnimation = false;
    }
    private IEnumerator RightScrollLoop() {
        isScrollAnimation = true;
        for (int i = 0; i < rankGroupData[0].rankUIAnchor.Length; i++) {
            if (currentShowGroupType == PageType.Page_2) {
                rankGroupData[0].rankUIAnchor[i].SetLocalPositionX(scrollOffset);
            }
            LeanTween.moveLocalX(rankGroupData[0].rankUIAnchor[i].gameObject, rankGroupData[0].rankUIAnchor[i].localPosition.x - scrollOffset, scrollAnimationTime);
        }
        for (int j = 0; j < rankGroupData[1].rankUIAnchor.Length; j++) {
            if (currentShowGroupType == PageType.Page_1) {
                rankGroupData[1].rankUIAnchor[j].SetLocalPositionX(scrollOffset);
            }
            LeanTween.moveLocalX(rankGroupData[1].rankUIAnchor[j].gameObject, rankGroupData[1].rankUIAnchor[j].localPosition.x - scrollOffset, scrollAnimationTime);
        }
        LeanTween.moveLocalX(lastResultGroupAnchor.gameObject, lastResultGroupAnchor.localPosition.x - scrollOffset, scrollAnimationTime);
        if (currentShowGroupType == PageType.Page_1) {
            currentShowGroupType = PageType.Page_2;
        } else {
            currentShowGroupType = PageType.Page_1;
        }
        UpdateScrollArrow();
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_scroll");
        yield return new WaitForSeconds(scrollAnimationTime + 0.1f);
        isScrollAnimation = false;
    }
    private void UpdateScrollArrow() {
        if (SingletonCustom<GameSettingManager>.Instance.IsEightBattle) {
            scrollLeftArrow.gameObject.SetActive(value: true);
            scrollRightArrow.gameObject.SetActive(value: true);
        } else {
            scrollLeftArrow.gameObject.SetActive(currentShowGroupType > PageType.Page_1);
            if (ResultGameDataParams.GetNowSceneType() == SceneManager.SceneType.MAX) {
                scrollRightArrow.gameObject.SetActive(currentShowGroupType < PageType.Page_2);
            } else {
                scrollRightArrow.gameObject.SetActive(currentShowGroupType < PageType.Final);
            }
        }
        switch (currentShowGroupType) {
            case PageType.Page_1:
                if (ResultGameDataParams.GetNowSceneType() == SceneManager.SceneType.BLOW_AWAY_TANK) {
                    survivalTimeTextObject.SetActive(value: true);
                }
                break;
        }
        string str = "";
        if (Localize_Define.Language != 0) {
            str = "en";
        }
        switch (currentShowGroupType) {
            case PageType.Page_1:
                scrollGroupNo.gameObject.SetActive(value: true);
                str += "_set_1st";
                scrollGroupNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                break;
            case PageType.Page_2:
                scrollGroupNo.gameObject.SetActive(value: true);
                str += "_set_2nd";
                scrollGroupNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, str);
                break;
            case PageType.Final:
                scrollGroupNo.gameObject.SetActive(value: false);
                break;
        }
        if (SingletonCustom<GameSettingManager>.Instance.IsEightBattle) {
            scrollGroupNo.gameObject.SetActive(value: false);
        }
    }
    private void SetButtonPattern() {
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE) {
            if (ResultGameDataParams.GetRecord(_isGroup1Record: false).intData != null || ResultGameDataParams.GetRecord(_isGroup1Record: false).floatData != null || ResultGameDataParams.GetRecord(_isGroup1Record: false).stringData != null) {
                buttonPattern_Scroll_Retry_GameSelect.ShowButtonUI();
            } else {
                buttonPattern_Retry_GameSelect.ShowButtonUI();
            }
        } else {
            buttonPattern_Next.ShowButtonUI();
        }
    }
    private void SetPhoto() {
        resultPhotoAnchor.SetActive(value: false);
    }
    private void OnDestroy() {
        LeanTween.cancel(base.gameObject);
    }
}
