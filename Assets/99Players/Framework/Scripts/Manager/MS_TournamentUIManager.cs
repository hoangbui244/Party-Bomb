using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MS_TournamentUIManager : MonoBehaviour {
    public enum RoundType {
        Round_1st,
        Round_2nd,
        LoserBattle,
        Round_Final,
        None
    }
    public enum LineType {
        Horizontal_Left,
        Horizontal_Right,
        Vertical
    }
    [Serializable]
    public struct LineSpriteData {
        [Header("左側のチ\u30fcムのライン詳細デ\u30fcタ")]
        public LineSpriteDetailData[] left_LineSprites;
        [Header("右側のチ\u30fcムのライン詳細デ\u30fcタ")]
        public LineSpriteDetailData[] right_LineSprites;
        [Header("共通のライン詳細デ\u30fcタ")]
        public LineSpriteDetailData[] common_LineSprites;
    }
    [Serializable]
    public struct LineSpriteDetailData {
        [Header("ライン画像")]
        public SpriteRenderer lineSprite;
        [Header("ライン種類")]
        public LineType lineType;
    }
    [Serializable]
    public struct TeamData {
        public int teamNo;
        public RoundType joinRoundType;
        public SpriteRenderer teamFadeUnderlaySprite;
    }
    [SerializeField]
    [Header("各団の名前画像")]
    private SpriteRenderer[] teamNameSprites;
    [SerializeField]
    [Header("各キャラ画像")]
    private SpriteRenderer[] arrayTeamCharacter;
    private int[] arrayTeamNo = new int[4];
    [SerializeField]
    [Header("１回戦のライン画像デ\u30fcタ")]
    private LineSpriteData lineData_1stRound;
    [SerializeField]
    [Header("２回戦のライン画像デ\u30fcタ")]
    private LineSpriteData lineData_2ndRound;
    [SerializeField]
    [Header("決勝のライン画像デ\u30fcタ")]
    private LineSpriteData lineData_Final;
    [SerializeField]
    [Header("１回戦の敗者戦用のライン画像デ\u30fcタ")]
    private LineSpriteData lineData_1stRound_Lose;
    [SerializeField]
    [Header("２回戦の敗者戦用のライン画像デ\u30fcタ")]
    private LineSpriteData lineData_2ndRound_Lose;
    [SerializeField]
    [Header("決勝の敗者戦用のライン画像デ\u30fcタ")]
    private LineSpriteData lineData_Final_Lose;
    [SerializeField]
    [Header("UIアンカ\u30fc")]
    private Transform tableUIAnchor;
    [SerializeField]
    [Header("背景画像")]
    private SpriteRenderer tableBackFade;
    [SerializeField]
    [Header("現在の試合の名前")]
    private SpriteRenderer nowBattleNameText;
    [SerializeField]
    [Header("キャラ画像配列")]
    private Sprite[] arrayCharacter;
    [SerializeField]
    [Header("キャラ画像配列（笑顔）")]
    private Sprite[] arrayCharacterHappy;
    [SerializeField]
    [Header("キャラ画像配列（喜び）")]
    private Sprite[] arrayCharacterGood;
    [SerializeField]
    [Header("キャラ画像配列（悲しみ）")]
    private Sprite[] arrayCharacterSad;
    [SerializeField]
    [Header("優勝文字の裏パ\u30fcティクル")]
    private ParticleSystem psWinner;
    private float def_TableUIPos_Y;
    private RoundType currentRoundType;
    private TeamData[] teamData_1stRound = new TeamData[2];
    private TeamData[] teamData_2ndRound = new TeamData[2];
    private TeamData[] teamData_Final = new TeamData[2];
    private TeamData[] teamData_LoserBattle = new TeamData[2];
    private const float WINDOW_SHOW_TIME = 0.5f;
    private const float BACKGROUND_FADEIN_TIME = 0.5f;
    private const float LINE_SCALING_TIME = 0.7f;
    private const float TEAM_NAME_COLOR_TO_GRAY_TIME = 0.5f;
    private Coroutine lineCoroutine;
    private Action lineAnimationEndCallBack;
    private bool isLoserBattleAllow;
    private SwordFight_MainGameManager.TeamData[] prevLoserBattleTeamData = new SwordFight_MainGameManager.TeamData[2];
    private List<int> tournamentWinnerNoList = new List<int>();
    private List<string> tournamentMatchNoList = new List<string>();
    public RoundType CurrentRoundType => currentRoundType;
    public void Init() {
        currentRoundType = RoundType.Round_1st;
        switch (Localize_Define.Language) {
            case Localize_Define.LanguageType.Japanese:
                nowBattleNameText.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "_display_00");
                break;
            case Localize_Define.LanguageType.English:
                nowBattleNameText.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "en_display_00");
                break;
        }
        tableBackFade.SetAlpha(0f);
        def_TableUIPos_Y = tableUIAnchor.localPosition.y;
        tableUIAnchor.SetPositionY(1560f);
        InitLine(lineData_1stRound);
        InitLine(lineData_2ndRound);
        InitLine(lineData_Final);
        InitLine(lineData_1stRound_Lose);
        InitLine(lineData_2ndRound_Lose);
        InitLine(lineData_Final_Lose);
        isLoserBattleAllow = true;
    }
    private void InitLine(LineSpriteData _lineData) {
        for (int i = 0; i < _lineData.left_LineSprites.Length; i++) {
            if (_lineData.left_LineSprites[i].lineType == LineType.Horizontal_Left || _lineData.left_LineSprites[i].lineType == LineType.Horizontal_Right) {
                _lineData.left_LineSprites[i].lineSprite.transform.SetLocalScaleX(0f);
            } else if (_lineData.left_LineSprites[i].lineType == LineType.Vertical) {
                _lineData.left_LineSprites[i].lineSprite.transform.SetLocalScaleX(0f);
            }
        }
        for (int j = 0; j < _lineData.right_LineSprites.Length; j++) {
            if (_lineData.right_LineSprites[j].lineType == LineType.Horizontal_Left || _lineData.right_LineSprites[j].lineType == LineType.Horizontal_Right) {
                _lineData.right_LineSprites[j].lineSprite.transform.SetLocalScaleX(0f);
            } else if (_lineData.right_LineSprites[j].lineType == LineType.Vertical) {
                _lineData.right_LineSprites[j].lineSprite.transform.SetLocalScaleX(0f);
            }
        }
        for (int k = 0; k < _lineData.common_LineSprites.Length; k++) {
            if (_lineData.common_LineSprites[k].lineType == LineType.Horizontal_Left || _lineData.common_LineSprites[k].lineType == LineType.Horizontal_Right) {
                _lineData.common_LineSprites[k].lineSprite.transform.SetLocalScaleX(0f);
            } else if (_lineData.common_LineSprites[k].lineType == LineType.Vertical) {
                _lineData.common_LineSprites[k].lineSprite.transform.SetLocalScaleX(0f);
            }
        }
    }
    public void SetTeamData(string _vsTeamNoStr, RoundType _setRoundType) {
        string[] array = _vsTeamNoStr.Split('-');
        tournamentMatchNoList.Add(_vsTeamNoStr);
        switch (_setRoundType) {
            case RoundType.Round_1st: {
                    for (int j = 0; j < array.Length; j++) {
                        teamData_1stRound[j].teamNo = int.Parse(array[j]) - 1;
                        UnityEngine.Debug.Log("チ\u30fcムデ\u30fcタを設定[i]:" + teamData_1stRound[j].teamNo.ToString());
                        teamData_1stRound[j].joinRoundType = RoundType.Round_1st;
                    }
                    int num = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[0].teamNo).memberPlayerNoList[0];
                    if ((uint)num <= 3u) {
                        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1) {
                            teamNameSprites[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_display_00");
                            teamNameSprites[0].transform.localScale = new Vector3(0.45f, 0.45f, 1f);
                            teamNameSprites[0].transform.SetLocalPosition(-376f, 83f, -5f);
                        } else {
                            teamNameSprites[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[0].teamNo).memberPlayerNoList[0] + 1).ToString() + "p");
                            teamNameSprites[0].transform.localScale = new Vector3(0.365f, 0.365f, 1f);
                            teamNameSprites[0].transform.SetLocalPosition(-381f, 83f, -5f);
                        }
                    } else {
                        teamNameSprites[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[0].teamNo).memberPlayerNoList[0] - 3).ToString());
                        teamNameSprites[0].gameObject.SetActive(value: false);
                    }
                    arrayTeamCharacter[0].sprite = arrayCharacter[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[0].teamNo).memberPlayerNoList[0]]];
                    arrayTeamNo[0] = teamData_1stRound[0].teamNo;
                    num = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[1].teamNo).memberPlayerNoList[0];
                    if ((uint)num <= 3u) {
                        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1) {
                            teamNameSprites[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_you");
                            teamNameSprites[1].transform.localScale = new Vector3(0.45f, 0.45f, 1f);
                            teamNameSprites[1].transform.SetLocalPosition(-161f, 83f, -5f);
                        } else {
                            teamNameSprites[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[1].teamNo).memberPlayerNoList[0] + 1).ToString() + "p");
                            teamNameSprites[1].transform.localScale = new Vector3(0.365f, 0.365f, 1f);
                            teamNameSprites[1].transform.SetLocalPosition(-166f, 83f, -5f);
                        }
                    } else {
                        teamNameSprites[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[1].teamNo).memberPlayerNoList[0] - 3).ToString());
                        teamNameSprites[1].gameObject.SetActive(value: false);
                    }
                    arrayTeamCharacter[1].sprite = arrayCharacter[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[1].teamNo).memberPlayerNoList[0]]];
                    arrayTeamNo[1] = teamData_1stRound[1].teamNo;
                    break;
                }
            case RoundType.Round_2nd: {
                    for (int i = 0; i < array.Length; i++) {
                        teamData_2ndRound[i].teamNo = int.Parse(array[i]) - 1;
                        teamData_2ndRound[i].joinRoundType = RoundType.Round_2nd;
                    }
                    int num = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[0].teamNo).memberPlayerNoList[0];
                    if ((uint)num <= 3u) {
                        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1) {
                            teamNameSprites[2].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_you");
                            teamNameSprites[2].transform.localScale = new Vector3(0.45f, 0.45f, 1f);
                            teamNameSprites[2].transform.SetLocalPosition(44f, 83f, -5f);
                        } else {
                            teamNameSprites[2].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[0].teamNo).memberPlayerNoList[0] + 1).ToString() + "p");
                            teamNameSprites[2].transform.localScale = new Vector3(0.365f, 0.365f, 1f);
                            teamNameSprites[2].transform.SetLocalPosition(49f, 83f, -5f);
                        }
                    } else {
                        teamNameSprites[2].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[0].teamNo).memberPlayerNoList[0] - 3).ToString());
                        teamNameSprites[2].gameObject.SetActive(value: false);
                    }
                    arrayTeamCharacter[2].sprite = arrayCharacter[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[0].teamNo).memberPlayerNoList[0]]];
                    arrayTeamNo[2] = teamData_2ndRound[0].teamNo;
                    num = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[1].teamNo).memberPlayerNoList[0];
                    if ((uint)num <= 3u) {
                        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1) {
                            teamNameSprites[3].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_you");
                            teamNameSprites[3].transform.localScale = new Vector3(0.45f, 0.45f, 1f);
                            teamNameSprites[3].transform.SetLocalPosition(257f, 83f, -5f);
                        } else {
                            teamNameSprites[3].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[1].teamNo).memberPlayerNoList[0] + 1).ToString() + "p");
                            teamNameSprites[3].transform.localScale = new Vector3(0.365f, 0.365f, 1f);
                            teamNameSprites[3].transform.SetLocalPosition(262f, 83f, -5f);
                        }
                    } else {
                        teamNameSprites[3].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[1].teamNo).memberPlayerNoList[0] - 3).ToString());
                        teamNameSprites[3].gameObject.SetActive(value: false);
                    }
                    arrayTeamCharacter[3].sprite = arrayCharacter[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[1].teamNo).memberPlayerNoList[0]]];
                    arrayTeamNo[3] = teamData_2ndRound[1].teamNo;
                    break;
                }
            case RoundType.Round_Final:
                UnityEngine.Debug.LogError("未使用の処理に飛びました！");
                break;
        }
    }
    private int GetTeamNoToIdx(int _teamNo) {
        for (int i = 0; i < arrayTeamNo.Length; i++) {
            if (arrayTeamNo[i] == _teamNo) {
                return i;
            }
        }
        return -1;
    }
    public int[] GetFinalRoundTeamNo() {
        int[] array = new int[2]
        {
            teamData_Final[0].teamNo,
            teamData_Final[1].teamNo
        };
        UnityEngine.Debug.Log("☆決勝団番号0:" + array[0].ToString());
        UnityEngine.Debug.Log("☆決勝団番号1:" + array[1].ToString());
        return array;
    }
    public int[] GetLoserBattleTeamNo() {
        int[] array = new int[2]
        {
            teamData_LoserBattle[0].teamNo,
            teamData_LoserBattle[1].teamNo
        };
        UnityEngine.Debug.Log("☆敗者戦団番号0:" + array[0].ToString());
        UnityEngine.Debug.Log("☆敗者戦団番号1:" + array[1].ToString());
        return array;
    }
    public int[] GetWinnerTeamNo() {
        return tournamentWinnerNoList.ToArray();
    }
    public string[] GetMatchTeamNo() {
        return tournamentMatchNoList.ToArray();
    }
    public void StartLineAnimation(bool _isWinnerTeamIsLeft, Action _endCallBack) {
        lineAnimationEndCallBack = _endCallBack;
        StartCoroutine(LineAnimation(_isWinnerTeamIsLeft));
    }
    private IEnumerator LineAnimation(bool _isWinnerTeamIsLeft) {
        UnityEngine.Debug.Log("【LA】:" + currentRoundType.ToString());
        if (currentRoundType != RoundType.Round_Final) {
            arrayTeamCharacter[0].sprite = arrayCharacter[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[0].teamNo).memberPlayerNoList[0]]];
            arrayTeamCharacter[1].sprite = arrayCharacter[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[1].teamNo).memberPlayerNoList[0]]];
            arrayTeamCharacter[2].sprite = arrayCharacter[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[0].teamNo).memberPlayerNoList[0]]];
            arrayTeamCharacter[3].sprite = arrayCharacter[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[1].teamNo).memberPlayerNoList[0]]];
        }
        switch (currentRoundType) {
            case RoundType.Round_1st:
                if (_isWinnerTeamIsLeft) {
                    teamData_Final[0] = teamData_1stRound[0];
                    UnityEngine.Debug.Log("【決勝進出】:" + teamData_Final[0].teamNo.ToString());
                    teamData_LoserBattle[0] = teamData_1stRound[1];
                    UnityEngine.Debug.Log("【敗者戦進出】:" + teamData_LoserBattle[0].teamNo.ToString());
                    arrayTeamCharacter[0].sprite = arrayCharacterHappy[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[0].teamNo).memberPlayerNoList[0]]];
                    arrayTeamCharacter[1].sprite = arrayCharacterSad[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[1].teamNo).memberPlayerNoList[0]]];
                } else {
                    teamData_LoserBattle[0] = teamData_1stRound[0];
                    UnityEngine.Debug.Log("【敗者戦進出】:" + teamData_LoserBattle[0].teamNo.ToString());
                    teamData_Final[0] = teamData_1stRound[1];
                    UnityEngine.Debug.Log("【決勝進出】:" + teamData_Final[0].teamNo.ToString());
                    arrayTeamCharacter[0].sprite = arrayCharacterSad[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[0].teamNo).memberPlayerNoList[0]]];
                    arrayTeamCharacter[1].sprite = arrayCharacterHappy[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_1stRound[1].teamNo).memberPlayerNoList[0]]];
                }
                break;
            case RoundType.Round_2nd:
                if (_isWinnerTeamIsLeft) {
                    teamData_Final[1] = teamData_2ndRound[0];
                    teamData_LoserBattle[1] = teamData_2ndRound[1];
                    arrayTeamCharacter[2].sprite = arrayCharacterHappy[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[0].teamNo).memberPlayerNoList[0]]];
                    arrayTeamCharacter[3].sprite = arrayCharacterSad[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[1].teamNo).memberPlayerNoList[0]]];
                } else {
                    teamData_LoserBattle[1] = teamData_2ndRound[0];
                    teamData_Final[1] = teamData_2ndRound[1];
                    arrayTeamCharacter[2].sprite = arrayCharacterSad[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[0].teamNo).memberPlayerNoList[0]]];
                    arrayTeamCharacter[3].sprite = arrayCharacterHappy[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(teamData_2ndRound[1].teamNo).memberPlayerNoList[0]]];
                }
                break;
            case RoundType.LoserBattle:
                if (_isWinnerTeamIsLeft) {
                    arrayTeamCharacter[GetTeamNoToIdx(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).teamNo)].sprite = arrayCharacterHappy[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).teamNo).memberPlayerNoList[0]]];
                    arrayTeamCharacter[GetTeamNoToIdx(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).teamNo)].sprite = arrayCharacterSad[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).teamNo).memberPlayerNoList[0]]];
                } else {
                    arrayTeamCharacter[GetTeamNoToIdx(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).teamNo)].sprite = arrayCharacterHappy[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).teamNo).memberPlayerNoList[0]]];
                    arrayTeamCharacter[GetTeamNoToIdx(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).teamNo)].sprite = arrayCharacterSad[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).teamNo).memberPlayerNoList[0]]];
                }
                break;
            case RoundType.Round_Final:
                if (_isWinnerTeamIsLeft) {
                    arrayTeamCharacter[GetTeamNoToIdx(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).teamNo)].sprite = arrayCharacterHappy[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).teamNo).memberPlayerNoList[0]]];
                    arrayTeamCharacter[GetTeamNoToIdx(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).teamNo)].sprite = arrayCharacterGood[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).teamNo).memberPlayerNoList[0]]];
                } else {
                    arrayTeamCharacter[GetTeamNoToIdx(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).teamNo)].sprite = arrayCharacterHappy[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).teamNo).memberPlayerNoList[0]]];
                    arrayTeamCharacter[GetTeamNoToIdx(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).teamNo)].sprite = arrayCharacterGood[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).teamNo).memberPlayerNoList[0]]];
                }
                break;
        }
        yield return new WaitForSeconds(1f);
        LeanTween.moveLocalY(tableUIAnchor.gameObject, def_TableUIPos_Y, 0.5f).setEaseOutBack();
        StartCoroutine(SetAlphaColor(tableBackFade, 0.5f, 0.6f));
        yield return new WaitForSeconds(0.6f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_view_score");
        switch (currentRoundType) {
            case RoundType.Round_1st:
                if (_isWinnerTeamIsLeft) {
                    lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_1stRound.left_LineSprites));
                    tournamentWinnerNoList.Add(teamData_1stRound[0].teamNo);
                    StartCoroutine(LineAnimationProcess(lineData_1stRound_Lose.right_LineSprites));
                } else {
                    lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_1stRound.right_LineSprites));
                    tournamentWinnerNoList.Add(teamData_1stRound[1].teamNo);
                    StartCoroutine(LineAnimationProcess(lineData_1stRound_Lose.left_LineSprites));
                }
                yield return lineCoroutine;
                lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_1stRound.common_LineSprites));
                StartCoroutine(LineAnimationProcess(lineData_1stRound_Lose.common_LineSprites));
                break;
            case RoundType.Round_2nd: {
                    bool isLoserBattleAllow2 = isLoserBattleAllow;
                    if (_isWinnerTeamIsLeft) {
                        lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_2ndRound.left_LineSprites));
                        tournamentWinnerNoList.Add(teamData_2ndRound[0].teamNo);
                        StartCoroutine(LineAnimationProcess(lineData_2ndRound_Lose.right_LineSprites));
                    } else {
                        lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_2ndRound.right_LineSprites));
                        tournamentWinnerNoList.Add(teamData_2ndRound[1].teamNo);
                        StartCoroutine(LineAnimationProcess(lineData_2ndRound_Lose.left_LineSprites));
                    }
                    yield return lineCoroutine;
                    lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_2ndRound.common_LineSprites));
                    StartCoroutine(LineAnimationProcess(lineData_2ndRound_Lose.common_LineSprites));
                    break;
                }
            case RoundType.LoserBattle:
                if (_isWinnerTeamIsLeft) {
                    for (int k = 0; k < teamData_LoserBattle.Length; k++) {
                        if (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).teamNo == teamData_LoserBattle[k].teamNo) {
                            if (teamData_LoserBattle[k].joinRoundType == RoundType.Round_1st) {
                                lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_Final_Lose.left_LineSprites));
                                tournamentWinnerNoList.Add(teamData_LoserBattle[0].teamNo);
                            } else {
                                lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_Final_Lose.right_LineSprites));
                                tournamentWinnerNoList.Add(teamData_LoserBattle[1].teamNo);
                            }
                        }
                    }
                } else {
                    for (int l = 0; l < teamData_LoserBattle.Length; l++) {
                        if (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).teamNo == teamData_LoserBattle[l].teamNo) {
                            if (teamData_LoserBattle[l].joinRoundType == RoundType.Round_1st) {
                                lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_Final_Lose.left_LineSprites));
                                tournamentWinnerNoList.Add(teamData_LoserBattle[0].teamNo);
                            } else {
                                lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_Final_Lose.right_LineSprites));
                                tournamentWinnerNoList.Add(teamData_LoserBattle[1].teamNo);
                            }
                        }
                    }
                }
                yield return lineCoroutine;
                lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_Final_Lose.common_LineSprites));
                break;
            case RoundType.Round_Final:
                currentRoundType = RoundType.None;
                if (_isWinnerTeamIsLeft) {
                    for (int i = 0; i < teamData_Final.Length; i++) {
                        if (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide).teamNo == teamData_Final[i].teamNo) {
                            if (teamData_Final[i].joinRoundType == RoundType.Round_1st) {
                                lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_Final.left_LineSprites));
                                tournamentWinnerNoList.Add(teamData_Final[0].teamNo);
                            } else {
                                lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_Final.right_LineSprites));
                                tournamentWinnerNoList.Add(teamData_Final[1].teamNo);
                            }
                        }
                    }
                } else {
                    for (int j = 0; j < teamData_Final.Length; j++) {
                        if (SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide).teamNo == teamData_Final[j].teamNo) {
                            if (teamData_Final[j].joinRoundType == RoundType.Round_1st) {
                                lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_Final.left_LineSprites));
                                tournamentWinnerNoList.Add(teamData_Final[0].teamNo);
                            } else {
                                lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_Final.right_LineSprites));
                                tournamentWinnerNoList.Add(teamData_Final[1].teamNo);
                            }
                        }
                    }
                }
                yield return lineCoroutine;
                lineCoroutine = StartCoroutine(LineAnimationProcess(lineData_Final.common_LineSprites));
                break;
        }
        yield return lineCoroutine;
        if (currentRoundType == RoundType.None) {
            psWinner.Play();
            SingletonCustom<AudioManager>.Instance.SePlay("se_result_ranking_1st_win");
            SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
            yield return new WaitForSeconds(2f);
            if (lineAnimationEndCallBack != null) {
                lineAnimationEndCallBack();
            }
        } else {
            yield return new WaitForSeconds(2f);
            if (currentRoundType == RoundType.Round_Final) {
                LeanTween.moveLocalY(tableUIAnchor.gameObject, 1560f, 0.5f).setEaseInBack().setOnComplete((Action)delegate {
                    if (_isWinnerTeamIsLeft) {
                        arrayTeamCharacter[GetTeamNoToIdx(prevLoserBattleTeamData[0].teamNo)].sprite = arrayCharacter[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(prevLoserBattleTeamData[0].teamNo).memberPlayerNoList[0]]];
                        arrayTeamCharacter[GetTeamNoToIdx(prevLoserBattleTeamData[1].teamNo)].sprite = arrayCharacterSad[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(prevLoserBattleTeamData[1].teamNo).memberPlayerNoList[0]]];
                    } else {
                        arrayTeamCharacter[GetTeamNoToIdx(prevLoserBattleTeamData[1].teamNo)].sprite = arrayCharacter[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(prevLoserBattleTeamData[1].teamNo).memberPlayerNoList[0]]];
                        arrayTeamCharacter[GetTeamNoToIdx(prevLoserBattleTeamData[0].teamNo)].sprite = arrayCharacterSad[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(prevLoserBattleTeamData[0].teamNo).memberPlayerNoList[0]]];
                    }
                });
            } else {
                LeanTween.moveLocalY(tableUIAnchor.gameObject, 1560f, 0.5f).setEaseInBack();
            }
            StartCoroutine(SetAlphaColor(tableBackFade, 0.5f, 0.6f, 0f, _isFadeOut: true));
        }
        UnityEngine.Debug.Log("保存判定：" + currentRoundType.ToString());
        if (currentRoundType == RoundType.LoserBattle) {
            prevLoserBattleTeamData[0] = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.LeftSide);
            prevLoserBattleTeamData[1] = SingletonCustom<SwordFight_MainGameManager>.Instance.GetTeamData(SwordFight_MainGameManager.PositionSideType.RightSide);
        }
    }
    public void NextSetting() {
        switch (currentRoundType) {
            case RoundType.Round_1st:
                currentRoundType = RoundType.Round_2nd;
                switch (Localize_Define.Language) {
                    case Localize_Define.LanguageType.Japanese:
                        nowBattleNameText.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "_display_01");
                        break;
                    case Localize_Define.LanguageType.English:
                        nowBattleNameText.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "en_display_01");
                        break;
                }
                SingletonCustom<AudioManager>.Instance.VoicePlay("voice_sword_fight_battle_1");
                break;
            case RoundType.Round_2nd:
                if (isLoserBattleAllow) {
                    currentRoundType = RoundType.LoserBattle;
                    switch (Localize_Define.Language) {
                        case Localize_Define.LanguageType.Japanese:
                            nowBattleNameText.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "_display_02");
                            break;
                        case Localize_Define.LanguageType.English:
                            nowBattleNameText.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "en_display_02");
                            break;
                    }
                    SingletonCustom<AudioManager>.Instance.VoicePlay("voice_sword_fight_battle_2");
                } else {
                    currentRoundType = RoundType.Round_Final;
                    switch (Localize_Define.Language) {
                        case Localize_Define.LanguageType.Japanese:
                            nowBattleNameText.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "_display_03");
                            break;
                        case Localize_Define.LanguageType.English:
                            nowBattleNameText.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "en_display_03");
                            break;
                    }
                    SingletonCustom<AudioManager>.Instance.VoicePlay("voice_sword_fight_battle_3");
                }
                break;
            case RoundType.LoserBattle:
                currentRoundType = RoundType.Round_Final;
                switch (Localize_Define.Language) {
                    case Localize_Define.LanguageType.Japanese:
                        nowBattleNameText.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "_display_03");
                        break;
                    case Localize_Define.LanguageType.English:
                        nowBattleNameText.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.SwordFight, "en_display_03");
                        break;
                }
                SingletonCustom<AudioManager>.Instance.VoicePlay("voice_sword_fight_battle_3");
                break;
            case RoundType.Round_Final:
                currentRoundType = RoundType.None;
                break;
        }
    }
    private IEnumerator LineAnimationProcess(LineSpriteDetailData[] _lineData) {
        for (int i = 0; i < _lineData.Length; i++) {
            if (_lineData[i].lineType == LineType.Horizontal_Left || _lineData[i].lineType == LineType.Horizontal_Right) {
                LeanTween.scaleX(_lineData[i].lineSprite.gameObject, 1f, 0.7f);
            } else if (_lineData[i].lineType == LineType.Vertical) {
                LeanTween.scaleX(_lineData[i].lineSprite.gameObject, 1f, 0.7f);
            }
            yield return new WaitForSeconds(0.75f);
        }
    }
    private IEnumerator SetAlphaColor(SpriteRenderer _tk2dSprite, float _fadeTime, float _alpha = 1f, float _delayTime = 0f, bool _isFadeOut = false) {
        float time = 0f;
        Color color = _tk2dSprite.color;
        float startAlpha = 0f;
        float endAlpha = _alpha;
        if (_isFadeOut) {
            startAlpha = _alpha;
            endAlpha = 0f;
        }
        yield return new WaitForSeconds(_delayTime);
        while (time < _fadeTime) {
            color = _tk2dSprite.color;
            color.a = Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime);
            _tk2dSprite.color = color;
            time += Time.deltaTime;
            yield return null;
        }
    }
    public void Debug_TournamentWinnerList() {
        tournamentWinnerNoList.Clear();
        for (int i = 0; i < 4; i++) {
            tournamentWinnerNoList.Add(UnityEngine.Random.Range(0, 4));
        }
    }
}
