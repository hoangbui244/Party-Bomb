using GamepadInput;
using io.ninenine.players.party3d.games.common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class WinOrLoseResultManager : MonoBehaviour {
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
    [Header("チ\u30fcムアンカ\u30fc")]
    private Transform teamAnchor;
    [SerializeField]
    [Header("チ\u30fcムの名前のSprite")]
    private SpriteRenderer teamNameSprite;
    [SerializeField]
    [Header("チ\u30fcム所属のプレイヤ\u30fcアイコンアンカ\u30fc")]
    private Transform teamPlayerIconAnchor;
    [SerializeField]
    [Header("チ\u30fcムの所属プレイヤ\u30fcアイコン画像")]
    private SpriteRenderer[] teamPlayerIconSprite;
    [SerializeField]
    [Header("チ\u30fcムの所属プレイヤ\u30fcキャラ画像")]
    private SpriteRenderer[] teamPlayerCharacterIcon;
    [field: SerializeField] public GameObject VictoryRoot { get; private set; }
    [SerializeField]
    [Header("勝ち画像")]
    private SpriteRenderer[] victorySprite;
    [field: SerializeField] public GameObject DefeatRoot { get; private set; }
    [SerializeField]
    [Header("負け画像")]
    private SpriteRenderer[] defeatSprite;
    [SerializeField]
    [Header("引き分け画像")]
    private SpriteRenderer drawSprite;
    [SerializeField]
    [Header("演出：クラッカ\u30fc")]
    private ParticleSystem psEffectCracker;
    [SerializeField]
    [Header("演出：紙吹雪")]
    private ParticleSystem psEffectConfetti;
    [SerializeField]
    [Header("[スクロ\u30fcル][プログラム]ボタンが無い時のボタンUIデ\u30fcタ")]
    private ResultGameDataParams.ButtonUIData buttonUIData_NoScrollNoProgram;
    [SerializeField]
    [Header("[プログラム]ボタンが無い時のボタンUIデ\u30fcタ")]
    private ResultGameDataParams.ButtonUIData buttonUIData_ScrollNoProgram;
    [SerializeField]
    [Header("[スクロ\u30fcル]ボタンが無い時のボタンUIデ\u30fcタ")]
    private ResultGameDataParams.ButtonUIData buttonUIData_NoScrollProgram;
    [SerializeField]
    [Header("[スクロ\u30fcル][プログラム]ボタンがある時のボタンUIデ\u30fcタ")]
    private ResultGameDataParams.ButtonUIData buttonUIData_ScrollProgram;
    [SerializeField]
    [Header("勝敗リザルトの記録デ\u30fcタ")]
    private WinOrLoseUIData winOrLoseUIData;
    [SerializeField]
    [Header("キ\u30fcホルダ\u30fcの杭画像")]
    private SpriteRenderer keyringPile;
    [SerializeField]
    [Header("消しゴムくんのキ\u30fcホルダ\u30fc画像")]
    private SpriteRenderer KeshigomukunKeyRing;
    [SerializeField]
    [Header("いちごちゃんのキ\u30fcホルダ\u30fc画像")]
    private SpriteRenderer IchigochanKeyRing;
    private ResultGameDataParams.ShowResultType showResultType;
    private float resultWindowAnimationTime = 0.6f;
    private float resultBackFanAnimationTime = 0.5f;
    private float resultTeamNameAnimationTime = 0.5f;
    private float resultTeamPlayerIconAnimationTime = 0.5f;
    private float resultDecisionAnimationTime = 0.5f;
    private float resultRecordAnimationTime = 0.2f;
    private float resultHiRecordAnimationTime = 0.2f;
    private float resultNewRecordAnimationTime = 0.4f;
    private float resultButtonAnimationTime = 0.3f;
    private bool isAnimation;
    private float defPosY_ResultWindow;
    private float[] defPosX_OperationButton;
    private ResultGameDataParams.ResultType resultType;
    private int showTeamNo;
    private bool isCoop2vs2Result;
    private bool isShowPoint;
    private bool isNewRecord;
    private bool isResultOpen;
    private Vector2[] teamAnchorPosData = new Vector2[2]
    {
        new Vector2(219f, 20f),
        new Vector2(219f, -30f)
    };
    private GS_Define.GameFormat originSelectGameFormat;
    [NonReorderable]
    private List<int>[] teamPlayerGroupList;
    [NonReorderable]
    private List<int>[] teamPointPlayerGroupList;
    private float[] arrayWinSpriteScale;
    public void SetTeamPlayerGroupList(List<int> _teamAPlayerNoList, List<int> _teamBPlayerNoList) {
        teamPlayerGroupList = new List<int>[2];
        teamPointPlayerGroupList = new List<int>[2];
        teamPointPlayerGroupList[0] = new List<int>(_teamAPlayerNoList);
        teamPointPlayerGroupList[1] = new List<int>(_teamBPlayerNoList);
        teamPlayerGroupList[0] = new List<int>();
        teamPlayerGroupList[1] = new List<int>();
        for (int i = 0; i < _teamAPlayerNoList.Count; i++) {
            if (_teamAPlayerNoList[i] < SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                teamPlayerGroupList[0].Add(_teamAPlayerNoList[i]);
            }
        }
        for (int j = 0; j < _teamBPlayerNoList.Count; j++) {
            if (_teamBPlayerNoList[j] < SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                teamPlayerGroupList[1].Add(_teamBPlayerNoList[j]);
            }
        }
        for (int k = 0; k < _teamAPlayerNoList.Count; k++) {
            UnityEngine.Debug.Log("A:[" + k.ToString() + "]:" + _teamAPlayerNoList[k].ToString());
        }
        for (int l = 0; l < _teamBPlayerNoList.Count; l++) {
            UnityEngine.Debug.Log("B:[" + l.ToString() + "]:" + _teamBPlayerNoList[l].ToString());
        }
        for (int m = 0; m < 2; m++) {
            UnityEngine.Debug.Log("teamPlayerGroupList[" + m.ToString() + "] " + teamPlayerGroupList[m].Count.ToString());
            for (int n = 0; n < teamPlayerGroupList[m].Count; n++) {
                UnityEngine.Debug.Log("teamPlayerGroupList[" + m.ToString() + "][" + n.ToString() + "] " + teamPlayerGroupList[m][n].ToString());
            }
        }
        UnityEngine.Debug.Log("playerGroupListTmpを設定!!!!");
    }
    public List<int>[] GetTeamPlayerGroupList() {
        return teamPlayerGroupList;
    }
    private void Awake() {
        resultWindowRoot.SetActive(value: false);
        defPosY_ResultWindow = resultWindowRoot.transform.localPosition.y;
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
            isCoop2vs2Result = false;
        } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            isCoop2vs2Result = false;
        } else {
            isCoop2vs2Result = true;
        }
        teamPlayerIconAnchor.gameObject.SetActive(value: false);
        switch (UnityEngine.Random.Range(0, 3)) {
            case 0:
                KeshigomukunKeyRing.gameObject.SetActive(value: true);
                IchigochanKeyRing.gameObject.SetActive(value: false);
                break;
            case 1:
                KeshigomukunKeyRing.gameObject.SetActive(value: false);
                IchigochanKeyRing.gameObject.SetActive(value: true);
                break;
            case 2:
                KeshigomukunKeyRing.gameObject.SetActive(value: false);
                IchigochanKeyRing.gameObject.SetActive(value: false);
                keyringPile.gameObject.SetActive(value: false);
                break;
        }
        VictoryRoot.SetActive(false);
        DefeatRoot.SetActive(false);
    }
    private void OnDisable() {
        ResultGameDataParams.SetResultMode(_isResult: false);
    }
    private void Update() {
        if (!resultWindowRoot.activeSelf || (SingletonCustom<CommonNotificationManager>.Instance != null && SingletonCustom<CommonNotificationManager>.Instance.IsOpen) || (SingletonCustom<DM>.Instance != null && SingletonCustom<DM>.Instance.IsActive()) || isAnimation) {
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
        } else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Y)) {
            if (!ResultGameDataParams.IsLastPlayType()) {
                ClickResultDetailsButton();
            }
        } else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X) && !ResultGameDataParams.IsLastPlayType() && !SingletonCustom<SportsDayPoint>.Instance.IsShow) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
            SingletonCustom<CommonNotificationManager>.Instance.SetSportsDayProgram(_isFade: true);
        }
    }
    public void ShowResult(ResultGameDataParams.ResultType _resultType, int _showTeamNo, string _hiscore = "") {
        if (isResultOpen) {
            return;
        }
        isResultOpen = true;
        this.resultType = _resultType;
        showTeamNo = _showTeamNo;
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_sports_day_final_result_1");
        switch (SingletonCustom<GameSettingManager>.Instance.LastSelectGameType) {
            case GS_Define.GameType.MAKE_SAME_DOT:
            case GS_Define.GameType.THREE_LEGGED:
            case GS_Define.GameType.ANSWERS_RUN:
            case GS_Define.GameType.TRAIN_GUIDE:
                if (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup) {
                    if (!string.IsNullOrEmpty(_hiscore)) {
                        float num3 = CalcManager.ConvertRecordStringToTime(SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.GetSix(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType, RecordData.InitType.Time));
                        if (float.Parse(_hiscore) < num3) {
                            CalcManager.ConvertTimeToRecordString(float.Parse(_hiscore));
                            SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.SetSix(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType, CalcManager.GetConvertRecordString(0));
                        }
                    }
                } else if (!string.IsNullOrEmpty(_hiscore)) {
                    float num4 = CalcManager.ConvertRecordStringToTime(SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Get(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType, RecordData.InitType.Time));
                    if (float.Parse(_hiscore) < num4) {
                        CalcManager.ConvertTimeToRecordString(float.Parse(_hiscore));
                        SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Set(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType, CalcManager.GetConvertRecordString(0));
                    }
                }
                break;
            case GS_Define.GameType.CANNON_SHOT:
            case GS_Define.GameType.RECEIVE_PON:
            case GS_Define.GameType.THROWING_BALLS:
            case GS_Define.GameType.BREAK_BLOCK:
            case GS_Define.GameType.COIN_DROP:
            case GS_Define.GameType.HUNDRED_CHALLENGE:
            case GS_Define.GameType.EVERYONE_KEEPER:
            case GS_Define.GameType.AIR_BALLOON:
                if (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup) {
                    if (!string.IsNullOrEmpty(_hiscore)) {
                        int num = int.Parse(SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.GetSix(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType, RecordData.InitType.Score));
                        UnityEngine.Debug.Log("_hiscore:" + _hiscore + " currentHiScore:" + num.ToString());
                        if (int.Parse(_hiscore) > num) {
                            SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.SetSix(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType, _hiscore);
                        }
                    }
                } else if (!string.IsNullOrEmpty(_hiscore)) {
                    int num2 = int.Parse(SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Get(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType, RecordData.InitType.Score));
                    UnityEngine.Debug.Log("_hiscore:" + _hiscore + " currentHiScore:" + num2.ToString());
                    if (int.Parse(_hiscore) > num2) {
                        SingletonCustom<SaveDataManager>.Instance.SaveData.recordData.Set(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType, _hiscore);
                    }
                }
                break;
        }
        InitShowResult();
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.ALL_SPORTS) {
            isShowPoint = true;
        } else {
            isShowPoint = false;
        }
        if (isShowPoint) {
            teamAnchor.localPosition = teamAnchorPosData[0];
            showResultType = ResultGameDataParams.ShowResultType.Record_Score;
            winOrLoseUIData.SetUIActive(_isActive: true, WinOrLoseUIData.RecordType.Score);
            ResultGameDataParams.ResultType resultType;
            ResultGameDataParams.ResultType resultType2;
            if (_resultType == ResultGameDataParams.ResultType.Win && _showTeamNo == 0) {
                resultType = ResultGameDataParams.ResultType.Win;
                resultType2 = ResultGameDataParams.ResultType.Lose;
            } else if (_resultType == ResultGameDataParams.ResultType.Draw) {
                resultType = ResultGameDataParams.ResultType.Draw;
                resultType2 = ResultGameDataParams.ResultType.Draw;
            } else {
                resultType = ResultGameDataParams.ResultType.Lose;
                resultType2 = ResultGameDataParams.ResultType.Win;
            }
            winOrLoseUIData.SetScore_Int(0, ResultGameDataParams.GetPointData_WinOrLose(resultType), WinOrLoseUIData.RecordType.Score);
            winOrLoseUIData.SetScore_Int(1, ResultGameDataParams.GetPointData_WinOrLose(resultType2), WinOrLoseUIData.RecordType.Score);
        } else if (ResultGameDataParams.GetNowSceneType() == SceneManager.SceneType.ATTACK_BALL || ResultGameDataParams.GetNowSceneType() == SceneManager.SceneType.MOLE_HAMMER) {
            teamAnchor.localPosition = teamAnchorPosData[0];
            showResultType = ResultGameDataParams.ShowResultType.Record_Score;
            winOrLoseUIData.SetUIActive(_isActive: true, WinOrLoseUIData.RecordType.Score);
            winOrLoseUIData.SetScore_Int(0, ResultGameDataParams.GetWinOrLoseRecord_Int(0), WinOrLoseUIData.RecordType.Score);
            winOrLoseUIData.SetScore_Int(1, ResultGameDataParams.GetWinOrLoseRecord_Int(1), WinOrLoseUIData.RecordType.Score);
        } else {
            teamAnchor.localPosition = teamAnchorPosData[1];
        }
        StartCoroutine(_ShowResult());
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
        InitResultAnimation();
        resultWindowRoot.SetActive(value: true);
    }
    private void InitResultAnimation() {
        isAnimation = true;
        buttonUIData_NoScrollNoProgram.buttonAnchor.gameObject.SetActive(value: false);
        buttonUIData_ScrollNoProgram.buttonAnchor.gameObject.SetActive(value: false);
        buttonUIData_NoScrollProgram.buttonAnchor.gameObject.SetActive(value: false);
        buttonUIData_ScrollProgram.buttonAnchor.gameObject.SetActive(value: false);
        resultWindowRoot.transform.SetLocalPositionY(defPosY_ResultWindow + 1500f);
        for (int i = 0; i < arraySpFrame.Length; i++) {
            arraySpFrame[i].SetAlpha(0f);
        }
        teamNameSprite.gameObject.SetActive(value: false);
        teamNameSprite.SetAlpha(0f);
        teamNameSprite.transform.SetLocalScale(3f, 3f, 3f);
        arrayWinSpriteScale = new float[victorySprite.Length];
        for (int j = 0; j < victorySprite.Length; j++) {
            victorySprite[j].gameObject.SetActive(value: false);
            victorySprite[j].SetAlpha(0f);
            arrayWinSpriteScale[j] = victorySprite[j].transform.localScale.x;
            if (j == 0) {
                victorySprite[j].transform.SetLocalScale(3f, 3f, 3f);
            } else {
                victorySprite[j].transform.SetLocalScale(1f, 1f, 1f);
            }
        }
        for (int k = 0; k < defeatSprite.Length; k++) {
            defeatSprite[k].gameObject.SetActive(value: false);
            defeatSprite[k].SetAlpha(0f);
            defeatSprite[k].transform.SetLocalScale(3f, 3f, 3f);
        }
        drawSprite.gameObject.SetActive(value: false);
        drawSprite.SetAlpha(0f);
        drawSprite.transform.SetLocalScale(3f, 3f, 3f);
        drawSprite.gameObject.SetActive(value: false);
        Localize_Define.LanguageType language = Localize_Define.Language;
        switch (resultType) {
            case ResultGameDataParams.ResultType.Win:
                VictoryRoot.SetActive(true);
                teamNameSprite.gameObject.SetActive(value: true);
                for (int m = 0; m < victorySprite.Length; m++) {
                    victorySprite[m].gameObject.SetActive(value: true);
                }
                break;
            case ResultGameDataParams.ResultType.Lose:
                DefeatRoot.SetActive(true);
                teamNameSprite.gameObject.SetActive(value: true);
                for (int l = 0; l < defeatSprite.Length; l++) {
                    defeatSprite[l].gameObject.SetActive(value: true);
                }
                break;
            case ResultGameDataParams.ResultType.Draw:
                drawSprite.gameObject.SetActive(value: true);
                break;
        }
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
        SetButtonPattern();
        psEffectCracker.Stop();
        psEffectConfetti.Stop();
    }
    private void SetTeamPlayerIcon() {
        if (resultType == ResultGameDataParams.ResultType.Draw) {
            for (int i = 0; i < teamPlayerCharacterIcon.Length; i++) {
                teamPlayerCharacterIcon[i].gameObject.SetActive(value: false);
            }
        } else {
            for (int j = 0; j < teamPointPlayerGroupList[showTeamNo].Count; j++) {
                UnityEngine.Debug.Log("cnt:" + teamPointPlayerGroupList[showTeamNo].Count.ToString());
                teamPlayerCharacterIcon[j].SetAlpha(0f);
                switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[teamPointPlayerGroupList[showTeamNo][j]]) {
                    case 0:
                        teamPlayerCharacterIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_yuto_0" + ((resultType != 0) ? 3 : 0).ToString());
                        break;
                    case 1:
                        teamPlayerCharacterIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_hina_0" + ((resultType != 0) ? 3 : 0).ToString());
                        break;
                    case 2:
                        teamPlayerCharacterIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_ituki_0" + ((resultType != 0) ? 3 : 0).ToString());
                        break;
                    case 3:
                        teamPlayerCharacterIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_souta_0" + ((resultType != 0) ? 3 : 0).ToString());
                        break;
                    case 4:
                        teamPlayerCharacterIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_takumi_0" + ((resultType != 0) ? 3 : 0).ToString());
                        break;
                    case 6:
                        teamPlayerCharacterIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_akira_0" + ((resultType != 0) ? 3 : 0).ToString());
                        break;
                    case 5:
                        teamPlayerCharacterIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_rin_0" + ((resultType != 0) ? 3 : 0).ToString());
                        break;
                    case 7:
                        teamPlayerCharacterIcon[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "character_rui_0" + ((resultType != 0) ? 3 : 0).ToString());
                        break;
                }
            }
        }
        if (!isCoop2vs2Result) {
            return;
        }
        teamPlayerIconAnchor.gameObject.SetActive(value: true);
        if (showTeamNo == -1) {
            for (int k = 0; k < teamPlayerIconSprite.Length; k++) {
                teamPlayerIconSprite[k].gameObject.SetActive(value: false);
            }
            return;
        }
        if (teamPlayerGroupList[showTeamNo].Count == 1) {
            teamPlayerIconSprite[0].transform.SetLocalPositionX((teamPlayerIconSprite[0].transform.localPosition.x + teamPlayerIconSprite[1].transform.localPosition.x) / 2f);
            teamPlayerIconSprite[1].SetAlpha(0f);
            teamPlayerIconSprite[1].gameObject.SetActive(value: false);
        }
        for (int l = 0; l < teamPlayerGroupList[showTeamNo].Count; l++) {
            switch (teamPlayerGroupList[showTeamNo][l]) {
                case 0:
                    teamPlayerIconSprite[l].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_1p");
                    break;
                case 1:
                    teamPlayerIconSprite[l].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_2p");
                    break;
                case 2:
                    teamPlayerIconSprite[l].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_3p");
                    break;
                case 3:
                    teamPlayerIconSprite[l].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_result_4p");
                    break;
            }
            teamPlayerIconSprite[l].SetAlpha(0f);
            teamPlayerIconSprite[l].transform.SetLocalScale(3f, 3f, 3f);
        }
    }
    private IEnumerator _ShowResult() {
        TouchPanelManager.Instance.SetTouchPanelEnable(false);
        Animation_ResultDialog_SlideIn();
        yield return new WaitForSeconds(resultWindowAnimationTime + 0.1f);
        Animation_ResultFan();
        yield return new WaitForSeconds(resultBackFanAnimationTime + 0.1f);
        if (isCoop2vs2Result && resultType != ResultGameDataParams.ResultType.Draw) {
            Animation_PlayerIcon();
            yield return new WaitForSeconds(resultTeamPlayerIconAnimationTime + 0.1f);
        }
        Animation_Decision();
        yield return new WaitForSeconds(resultTeamNameAnimationTime + 0.1f);
        if (winOrLoseUIData.IsRecordActive()) {
            Animation_Record();
            yield return new WaitForSeconds(resultRecordAnimationTime + 0.1f);
        }
        if (resultType == ResultGameDataParams.ResultType.Win) {
            WinnerEffect();
        }
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
    private void Animation_TeamName() {
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_slide");
        LeanTween.scale(teamNameSprite.gameObject, Vector3.one, resultTeamNameAnimationTime).setEaseOutBack().setOnComplete(Animation_TeamName_Effect);
        StartCoroutine(SetAlphaColor(teamNameSprite, resultTeamNameAnimationTime));
    }
    private void Animation_TeamName_Effect() {
        SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate(teamNameSprite, teamNameSprite.transform.localPosition, Quaternion.identity, teamNameSprite.transform.parent);
        spriteRenderer.transform.localPosition = teamNameSprite.transform.localPosition;
        spriteRenderer.sortingOrder = teamNameSprite.sortingOrder + 1;
        spriteRenderer.SetAlpha(1f);
        spriteRenderer.transform.localScale = Vector3.one;
        LeanTween.scale(spriteRenderer.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.6f).setEaseLinear();
        StartCoroutine(SetAlphaColor(spriteRenderer, 0.6f, 0f, _isFadeOut: true));
    }
    private void Animation_PlayerIcon() {
        int num = 0;
        switch (resultType) {
            case ResultGameDataParams.ResultType.Win:
                num = teamPlayerGroupList[showTeamNo].Count;
                break;
            case ResultGameDataParams.ResultType.Lose:
                num = teamPlayerGroupList[showTeamNo].Count;
                break;
            case ResultGameDataParams.ResultType.Draw:
                num = 0;
                break;
        }
        float num2 = resultTeamPlayerIconAnimationTime / (float)num;
        for (int i = 0; i < num; i++) {
            LeanTween.scale(teamPlayerIconSprite[i].gameObject, Vector3.one, resultTeamPlayerIconAnimationTime).setEaseOutBack().setDelay(num2 + (float)i * num2);
            StartCoroutine(SetAlphaColor(teamPlayerIconSprite[i], resultTeamPlayerIconAnimationTime, num2 + (float)i * num2));
            SingletonCustom<AudioManager>.Instance.SePlay("se_result_puyon", _loop: false, 0f, 1f, 1f, num2 + (float)i * num2);
        }
    }
    private void Animation_Decision() {
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_view_score");
        switch (resultType) {
            case ResultGameDataParams.ResultType.Win:
                for (int j = 0; j < victorySprite.Length; j++) {
                    LeanTween.scale(victorySprite[j].gameObject, new Vector3(arrayWinSpriteScale[j], arrayWinSpriteScale[j]), resultDecisionAnimationTime).setEaseOutBack().setOnComplete((Action)delegate {
                    });
                    StartCoroutine(SetAlphaColor(victorySprite[j], resultTeamNameAnimationTime));
                }
                LeanTween.delayedCall(resultDecisionAnimationTime, Animation_Decision_Effect);
                SingletonCustom<AudioManager>.Instance.SePlay("se_result_win");
                break;
            case ResultGameDataParams.ResultType.Lose:
                for (int i = 0; i < defeatSprite.Length; i++) {
                    LeanTween.scale(defeatSprite[i].gameObject, Vector3.one, resultDecisionAnimationTime).setEaseOutBack().setOnComplete((Action)delegate {
                    });
                    StartCoroutine(SetAlphaColor(defeatSprite[i], resultTeamNameAnimationTime));
                }
                SingletonCustom<AudioManager>.Instance.SePlay("se_coop_failure");
                break;
        }
        for (int k = 0; k < teamPlayerCharacterIcon.Length; k++) {
            StartCoroutine(SetAlphaColor(teamPlayerCharacterIcon[k], resultTeamPlayerIconAnimationTime, 0.25f));
        }
    }
    private void Animation_Decision_Effect() {
        SpriteRenderer[] array = null;
        switch (resultType) {
            case ResultGameDataParams.ResultType.Draw:
                break;
            case ResultGameDataParams.ResultType.Win:
                array = new SpriteRenderer[victorySprite.Length];
                for (int k = 0; k < victorySprite.Length; k++) {
                    array[k] = UnityEngine.Object.Instantiate(victorySprite[k], victorySprite[k].transform.localPosition, Quaternion.identity, victorySprite[k].transform.parent);
                    array[k].sortingOrder = victorySprite[k].sortingOrder + 1;
                    array[k].transform.localPosition = victorySprite[k].transform.localPosition;
                    array[k].transform.localRotation = victorySprite[k].transform.localRotation;
                }
                for (int l = 0; l < array.Length; l++) {
                    array[l].SetAlpha(1f);
                    array[l].transform.localScale = new Vector3(arrayWinSpriteScale[l], arrayWinSpriteScale[l], arrayWinSpriteScale[l]);
                    LeanTween.scale(array[l].gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.6f).setEaseLinear();
                    StartCoroutine(SetAlphaColor(array[l], 0.6f, 0f, _isFadeOut: true));
                }
                break;
            case ResultGameDataParams.ResultType.Lose:
                array = new SpriteRenderer[defeatSprite.Length];
                for (int i = 0; i < defeatSprite.Length; i++) {
                    array[i] = UnityEngine.Object.Instantiate(defeatSprite[i], defeatSprite[i].transform.localPosition, Quaternion.identity, defeatSprite[i].transform.parent);
                    array[i].sortingOrder = defeatSprite[i].sortingOrder + 1;
                    array[i].transform.localPosition = defeatSprite[i].transform.localPosition;
                }
                for (int j = 0; j < array.Length; j++) {
                    array[j].SetAlpha(1f);
                    array[j].transform.localScale = Vector3.one;
                    LeanTween.scale(array[j].gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.6f).setEaseLinear();
                    StartCoroutine(SetAlphaColor(array[j], 0.6f, 0f, _isFadeOut: true));
                }
                break;
        }
    }
    private void Animation_Record() {
        WinOrLoseUIData.RecordType recordType = WinOrLoseUIData.RecordType.Score;
        switch (showResultType) {
            case ResultGameDataParams.ShowResultType.Record_Score:
                recordType = WinOrLoseUIData.RecordType.Score;
                break;
            case ResultGameDataParams.ShowResultType.Record_Score_Four_Digit:
                recordType = WinOrLoseUIData.RecordType.ScoreFourDigit;
                break;
            case ResultGameDataParams.ShowResultType.Record_DecimalScore:
                recordType = WinOrLoseUIData.RecordType.DecimalScore;
                break;
        }
        winOrLoseUIData.Animation_Fade(resultRecordAnimationTime, recordType);
        winOrLoseUIData.Animation_Scaling(resultRecordAnimationTime, recordType);
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_puyon");
    }
    private void WinnerEffect() {
        psEffectCracker.Play();
        psEffectConfetti.Play();
        for (int i = 0; i < 2; i++) {
            StartCoroutine(_PlayCrackerSE((float)i * 0.25f));
        }
    }
    private IEnumerator _PlayCrackerSE(float _waitTime) {
        yield return new WaitForSeconds(_waitTime);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cracker");
    }
    private void Animation_OperationButton() {
        if (SingletonCustom<CommonNotificationManager>.Instance != null) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGetTrophy(null);
        }
        float num = resultButtonAnimationTime;
        int num2 = 0;
        if (buttonUIData_NoScrollNoProgram.buttonAnchor.gameObject.activeSelf) {
            num2 = buttonUIData_NoScrollNoProgram.arrayAnimButton.Length - 1;
            for (int i = 0; i < num2; i++) {
                LeanTween.moveLocalX(buttonUIData_NoScrollNoProgram.arrayAnimButton[i], defPosX_OperationButton[i], resultButtonAnimationTime).setDelay(num + (float)i * num);
            }
            LeanTween.moveLocalX(buttonUIData_NoScrollNoProgram.arrayAnimButton[num2], defPosX_OperationButton[num2], resultButtonAnimationTime).setDelay(num + (float)num2 * num).setOnComplete(OnCompleteAllResultAnimation);
        } else if (buttonUIData_ScrollNoProgram.buttonAnchor.gameObject.activeSelf) {
            num2 = buttonUIData_ScrollNoProgram.arrayAnimButton.Length - 1;
            for (int j = 0; j < num2; j++) {
                LeanTween.moveLocalX(buttonUIData_ScrollNoProgram.arrayAnimButton[j], defPosX_OperationButton[j], resultButtonAnimationTime).setDelay(num + (float)j * num);
            }
            LeanTween.moveLocalX(buttonUIData_ScrollNoProgram.arrayAnimButton[num2], defPosX_OperationButton[num2], resultButtonAnimationTime).setDelay(num + (float)num2 * num).setOnComplete(OnCompleteAllResultAnimation);
        } else if (buttonUIData_NoScrollProgram.buttonAnchor.gameObject.activeSelf) {
            num2 = buttonUIData_NoScrollProgram.arrayAnimButton.Length - 1;
            for (int k = 0; k < num2; k++) {
                LeanTween.moveLocalX(buttonUIData_NoScrollProgram.arrayAnimButton[k], defPosX_OperationButton[k], resultButtonAnimationTime).setDelay(num + (float)k * num);
            }
            LeanTween.moveLocalX(buttonUIData_NoScrollProgram.arrayAnimButton[num2], defPosX_OperationButton[num2], resultButtonAnimationTime).setDelay(num + (float)num2 * num).setOnComplete(OnCompleteAllResultAnimation);
        } else if (buttonUIData_ScrollProgram.buttonAnchor.gameObject.activeSelf) {
            num2 = buttonUIData_ScrollProgram.arrayAnimButton.Length - 1;
            for (int l = 0; l < num2; l++) {
                LeanTween.moveLocalX(buttonUIData_ScrollProgram.arrayAnimButton[l], defPosX_OperationButton[l], resultButtonAnimationTime).setDelay(num + (float)l * num);
            }
            LeanTween.moveLocalX(buttonUIData_ScrollProgram.arrayAnimButton[num2], defPosX_OperationButton[num2], resultButtonAnimationTime).setDelay(num + (float)num2 * num).setOnComplete(OnCompleteAllResultAnimation);
        }
    }
    private void OnCompleteAllResultAnimation() {
        isAnimation = false;
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.ALL_SPORTS) {
            for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.TeamNum; i++) {
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(i, ResultGameDataParams.GetTeamTotalPoint(i)));
            }
            SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Sort((GameSettingManager.TeamData a, GameSettingManager.TeamData b) => b.score - a.score);
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
    private void ClickResultDetailsButton() {
        if (SingletonCustom<SceneManager>.Instance != null && !SingletonCustom<SceneManager>.Instance.GetFadeFlg() && !SingletonCustom<SportsDayPoint>.Instance.IsShow) {
            SingletonCustom<SportsDayPoint>.Instance.Show();
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        }
    }
    private void ClickNextButton() {
        if (SingletonCustom<SceneManager>.Instance != null && !SingletonCustom<SceneManager>.Instance.GetFadeFlg() && !SingletonCustom<SportsDayPoint>.Instance.IsShow) {
            SingletonCustom<SceneManager>.Instance.NextScene(SingletonCustom<GameSettingManager>.Instance.NextTable());
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        }
    }
    private void SetButtonPattern() {
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE) {
            if (ResultGameDataParams.GetRecord(_isGroup1Record: false).intData != null) {
                buttonUIData_ScrollNoProgram.buttonAnchor.gameObject.SetActive(value: true);
                defPosX_OperationButton = new float[buttonUIData_ScrollNoProgram.arrayAnimButton.Length];
                for (int i = 0; i < defPosX_OperationButton.Length; i++) {
                    defPosX_OperationButton[i] = buttonUIData_ScrollNoProgram.arrayAnimButton[i].transform.localPosition.x;
                }
                for (int j = 0; j < defPosX_OperationButton.Length; j++) {
                    buttonUIData_ScrollNoProgram.arrayAnimButton[j].transform.SetLocalPositionX(defPosX_OperationButton[j] + 2000f);
                }
            } else {
                buttonUIData_NoScrollNoProgram.buttonAnchor.gameObject.SetActive(value: true);
                defPosX_OperationButton = new float[buttonUIData_NoScrollNoProgram.arrayAnimButton.Length];
                for (int k = 0; k < defPosX_OperationButton.Length; k++) {
                    defPosX_OperationButton[k] = buttonUIData_NoScrollNoProgram.arrayAnimButton[k].transform.localPosition.x;
                }
                for (int l = 0; l < defPosX_OperationButton.Length; l++) {
                    buttonUIData_NoScrollNoProgram.arrayAnimButton[l].transform.SetLocalPositionX(defPosX_OperationButton[l] + 2000f);
                }
            }
            return;
        }
        if (ResultGameDataParams.GetRecord(_isGroup1Record: false).intData != null) {
            buttonUIData_ScrollProgram.buttonAnchor.gameObject.SetActive(value: true);
            defPosX_OperationButton = new float[buttonUIData_ScrollProgram.arrayAnimButton.Length];
            for (int m = 0; m < defPosX_OperationButton.Length; m++) {
                defPosX_OperationButton[m] = buttonUIData_ScrollProgram.arrayAnimButton[m].transform.localPosition.x;
            }
            for (int n = 0; n < defPosX_OperationButton.Length; n++) {
                buttonUIData_ScrollProgram.arrayAnimButton[n].transform.SetLocalPositionX(defPosX_OperationButton[n] + 2000f);
            }
            if (ResultGameDataParams.IsLastPlayType()) {
                for (int num = 0; num < buttonUIData_ScrollProgram.arrayLastGameHideButton.Length; num++) {
                    buttonUIData_ScrollProgram.arrayLastGameHideButton[num].SetActive(value: false);
                }
            }
            return;
        }
        buttonUIData_NoScrollProgram.buttonAnchor.gameObject.SetActive(value: true);
        defPosX_OperationButton = new float[buttonUIData_NoScrollProgram.arrayAnimButton.Length];
        for (int num2 = 0; num2 < defPosX_OperationButton.Length; num2++) {
            defPosX_OperationButton[num2] = buttonUIData_NoScrollProgram.arrayAnimButton[num2].transform.localPosition.x;
        }
        for (int num3 = 0; num3 < defPosX_OperationButton.Length; num3++) {
            buttonUIData_NoScrollProgram.arrayAnimButton[num3].transform.SetLocalPositionX(defPosX_OperationButton[num3] + 2000f);
        }
        if (ResultGameDataParams.IsLastPlayType()) {
            for (int num4 = 0; num4 < buttonUIData_NoScrollProgram.arrayLastGameHideButton.Length; num4++) {
                buttonUIData_NoScrollProgram.arrayLastGameHideButton[num4].SetActive(value: false);
            }
        }
    }
    private void SetPhoto() {
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
            resultPhotoAnchor.SetActive(value: true);
            switch (ResultGameDataParams.GetNowSceneType()) {
                case SceneManager.SceneType.GET_BALL:
                    resultPhoto.sprite = SingletonCustom<ResultManager>.Instance.GetPhotoSprite(SceneManager.SceneType.GET_BALL);
                    break;
                case SceneManager.SceneType.ARCHER_BATTLE:
                    resultPhoto.sprite = SingletonCustom<ResultManager>.Instance.GetPhotoSprite(SceneManager.SceneType.ARCHER_BATTLE);
                    break;
                case SceneManager.SceneType.BLOCK_WIPER:
                    resultPhoto.sprite = SingletonCustom<ResultManager>.Instance.GetPhotoSprite(SceneManager.SceneType.BLOCK_WIPER);
                    break;
                case SceneManager.SceneType.MOLE_HAMMER:
                    resultPhoto.sprite = SingletonCustom<ResultManager>.Instance.GetPhotoSprite(SceneManager.SceneType.MOLE_HAMMER);
                    break;
                case SceneManager.SceneType.BOMB_ROULETTE:
                    resultPhoto.sprite = SingletonCustom<ResultManager>.Instance.GetPhotoSprite(SceneManager.SceneType.BOMB_ROULETTE);
                    break;
                case SceneManager.SceneType.RECEIVE_PON:
                    resultPhoto.sprite = SingletonCustom<ResultManager>.Instance.GetPhotoSprite(SceneManager.SceneType.RECEIVE_PON);
                    break;
                case SceneManager.SceneType.BLACKSMITH:
                    resultPhoto.sprite = SingletonCustom<ResultManager>.Instance.GetPhotoSprite(SceneManager.SceneType.BLACKSMITH);
                    break;
                case SceneManager.SceneType.CANNON_SHOT:
                    resultPhoto.sprite = SingletonCustom<ResultManager>.Instance.GetPhotoSprite(SceneManager.SceneType.CANNON_SHOT);
                    break;
                case SceneManager.SceneType.ATTACK_BALL:
                    resultPhoto.sprite = SingletonCustom<ResultManager>.Instance.GetPhotoSprite(SceneManager.SceneType.ATTACK_BALL);
                    break;
                case SceneManager.SceneType.BLOW_AWAY_TANK:
                    resultPhoto.sprite = SingletonCustom<ResultManager>.Instance.GetPhotoSprite(SceneManager.SceneType.BLOW_AWAY_TANK);
                    break;
            }
        } else {
            resultPhotoAnchor.SetActive(value: false);
        }
    }
}
