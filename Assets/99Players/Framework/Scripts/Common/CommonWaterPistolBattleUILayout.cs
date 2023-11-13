using System;
using System.Collections.Generic;
using UnityEngine;
public class CommonWaterPistolBattleUILayout : MonoBehaviour {
    private const string BATTLE_PLAYER_SPRITE_SINGLE_NAME = "_screen_you";
    private const string SECOND_GROUP_SPRITE_NAME = "_play_2group";
    private static readonly string[] BATTLE_PLAYER_SPRITE_NAMES = new string[11]
    {
        "_screen_1p",
        "_screen_2p",
        "_screen_3p",
        "_screen_4p",
        "_screen_cp1",
        "_screen_cp2",
        "_screen_cp3",
        "_screen_cp4",
        "_screen_cp5",
        "_screen_cp6",
        "_screen_cp7"
    };
    private static readonly string[] COOP_PLAYER_SPRITE_NAMES = new string[11]
    {
        "_common_c_1P",
        "_common_c_2P",
        "_common_c_3P",
        "_common_c_4P",
        "_common_c_cp1",
        "_common_c_cp2",
        "_common_c_cp3",
        "_common_c_cp4",
        "_common_c_cp5",
        "_common_c_cp6",
        "_common_c_cp7"
    };
    private static readonly string[] CHARA_DEFAULT_SPRITE_NAMES = new string[8]
    {
        "character_yuto_02",
        "character_hina_02",
        "character_ituki_02",
        "character_souta_02",
        "character_takumi_02",
        "character_rin_02",
        "character_akira_02",
        "character_rui_02"
    };
    private static readonly Color END_GRAY_COLOR = Color.gray;
    [SerializeField]
    [Header("タイム表示")]
    private CommonGameTimeUI_Font_Time commonGameTimeUI;
    [SerializeField]
    [Header("個人戦アンカ\u30fc")]
    private Transform battleAnchor;
    [SerializeField]
    [Header("個人戦4人アンカ\u30fc")]
    private Transform battleFourAnchor;
    [SerializeField]
    [Header("スコアアンカ\u30fc_個人戦")]
    private Transform[] scoreAnchors_Battle;
    [SerializeField]
    [Header("スコア下敷き表示_個人戦")]
    private SpriteRenderer[] scoreUnderlaySprites_Battle;
    [SerializeField]
    [Header("スコア下敷き順番表示_個人戦")]
    private SpriteRenderer scoreUnderlayTurnSprite_Battle;
    [SerializeField]
    [Header("スコア表示_個人戦")]
    private SpriteNumbers[] scoreSpriteNumbers_Battle;
    [SerializeField]
    [Header("プレイヤ\u30fc表示_個人戦")]
    private SpriteRenderer[] playerIconSprites_Battle;
    [SerializeField]
    [Header("キャラ表示_個人戦")]
    private SpriteRenderer[] characterIconSprites_Battle;
    [SerializeField]
    [Header("個人戦8人アンカ\u30fc")]
    private Transform battleEightAnchor;
    [SerializeField]
    [Header("スコアアンカ\u30fc_個人戦8人")]
    private Transform[] scoreAnchors_EightBattle;
    [SerializeField]
    [Header("スコア下敷き表示_個人戦8人")]
    private SpriteRenderer[] scoreUnderlaySprites_EightBattle;
    [SerializeField]
    [Header("スコア下敷き順番表示_個人戦8人")]
    private SpriteRenderer scoreUnderlayTurnSprite_EightBattle;
    [SerializeField]
    [Header("スコア表示_個人戦8人")]
    private SpriteNumbers[] scoreSpriteNumbers_EightBattle;
    [SerializeField]
    [Header("プレイヤ\u30fc表示_個人戦8人")]
    private SpriteRenderer[] playerIconSprites_EightBattle;
    [SerializeField]
    [Header("チ\u30fcム戦アンカ\u30fc")]
    private Transform coopAnchor;
    [SerializeField]
    [Header("スコア表示_チ\u30fcム戦")]
    private SpriteNumbers[] scoreSpriteNumbers_Coop;
    [SerializeField]
    [Header("プレイヤ\u30fc表示_Aチ\u30fcム")]
    private SpriteRenderer[] playerIconSprites_Coop_TeamA;
    [SerializeField]
    [Header("プレイヤ\u30fc表示_Bチ\u30fcム")]
    private SpriteRenderer[] playerIconSprites_Coop_TeamB;
    [SerializeField]
    [Header("バツ表示_Aチ\u30fcム")]
    private SpriteRenderer[] batuSprites_Coop_TeamA;
    [SerializeField]
    [Header("バツ表示_Bチ\u30fcム")]
    private SpriteRenderer[] batuSprites_Coop_TeamB;
    [SerializeField]
    [Header("組目の表示")]
    private SpriteRenderer groupSprite;
    [SerializeField]
    [Header("ポ\u30fcズ表示")]
    private SpriteRenderer pauseSprite;
    private List<int>[] playerGroupList;
    private int[] scoreArray;
    private int[] playerNoArray;
    private int[] teamNoArray;
    private Vector3[] scoreAnchorDefaultPoses_Battle;
    private int playerNum;
    private int charaNum;
    private int teamNum;
    private float initTime;
    private float nowTime;
    private bool isCoop;
    private bool isEightBattle;
    private bool isSecondGroup;
    private bool isSecondInitEnd;
    private bool isMoveOut;
    private int idx;
    public void Init(float _initTime, List<int>[] _playerGroupList = null, bool _isEightBattle = false, bool _isSecondGroup = false) {
        initTime = (nowTime = _initTime);
        TimeUpdate();
        if (_playerGroupList == null) {
            playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
        } else {
            playerGroupList = _playerGroupList;
        }
        isEightBattle = _isEightBattle;
        isSecondGroup = _isSecondGroup;
        playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        teamNum = SingletonCustom<GameSettingManager>.Instance.TeamNum;
        isCoop = (teamNum == 2);
        charaNum = (isEightBattle ? 8 : 4);
        groupSprite.gameObject.SetActive(isSecondGroup);
        scoreArray = new int[charaNum];
        playerNoArray = new int[charaNum];
        teamNoArray = new int[charaNum];
        int num = playerGroupList[0].Count;
        if (!isEightBattle && num > 2) {
            num = 2;
        }
        if (isEightBattle) {
            for (int i = 0; i < charaNum; i++) {
                if (i < playerNum) {
                    playerNoArray[i] = i;
                } else {
                    playerNoArray[i] = 4 + i - playerNum;
                }
            }
        } else {
            for (int j = 0; j < playerGroupList.Length; j++) {
                int num2 = playerGroupList[j].Count;
                if (num2 > 2 && isSecondGroup) {
                    num2 = 2;
                }
                for (int k = 0; k < num2; k++) {
                    playerNoArray[j * num + k] = playerGroupList[j][k];
                    teamNoArray[j * num + k] = j;
                }
            }
        }
        DataInit();
        InitLayout();
    }
    public void SecondGroupInit() {
        isSecondInitEnd = true;
        groupSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_play_2group");
        int num = playerGroupList[0].Count;
        if (!isEightBattle && num > 2) {
            num = 2;
        }
        for (int i = 0; i < playerGroupList.Length; i++) {
            for (int j = 2; j < playerGroupList[i].Count; j++) {
                playerNoArray[i * num + j - 2] = playerGroupList[i][j];
                teamNoArray[i * num + j - 2] = i;
            }
        }
        for (int k = 0; k < batuSprites_Coop_TeamA.Length; k++) {
            batuSprites_Coop_TeamA[k].gameObject.SetActive(value: false);
        }
        DataInit();
        InitCoopPlayerSpriteLayout();
    }
    private void DataInit() {
        nowTime = initTime;
        for (int i = 0; i < scoreArray.Length; i++) {
            scoreArray[i] = 0;
        }
        ScoreUpdate();
    }
    private void Update() {
        if (SingletonCustom<GameSettingManager>.Instance.LastSelectGameType == GS_Define.GameType.ATTACK_BALL && SingletonCustom<Shooting_GameManager>.Instance.Timer.RemainingTime <= 10f && !isMoveOut) {
            LeanTween.moveLocalY(scoreAnchors_Battle[0].gameObject, scoreAnchors_Battle[0].localPosition.y + 500f, 1.25f).setEaseInQuint().setDelay(0f);
            LeanTween.moveLocalY(scoreAnchors_Battle[1].gameObject, scoreAnchors_Battle[1].localPosition.y + 500f, 1.25f).setEaseInQuint().setDelay(0.15f);
            LeanTween.moveLocalY(scoreAnchors_Battle[2].gameObject, scoreAnchors_Battle[2].localPosition.y + 500f, 1.25f).setEaseInQuint().setDelay(0.3f);
            LeanTween.moveLocalY(scoreAnchors_Battle[3].gameObject, scoreAnchors_Battle[3].localPosition.y + 500f, 1.25f).setEaseInQuint().setDelay(0.45f);
            isMoveOut = true;
        }
    }
    private void InitLayout() {
        if (isCoop) {
            coopAnchor.gameObject.SetActive(value: true);
            battleAnchor.gameObject.SetActive(value: false);
            InitCoopPlayerSpriteLayout();
        } else {
            coopAnchor.gameObject.SetActive(value: false);
            battleAnchor.gameObject.SetActive(value: true);
            battleFourAnchor.gameObject.SetActive(!isEightBattle);
            battleEightAnchor.gameObject.SetActive(isEightBattle);
            InitBattlePlayerSpriteLayout();
        }
    }
    private void InitBattlePlayerSpriteLayout() {
        if (isEightBattle) {
            if (playerNum != 1) {
                pauseSprite.gameObject.SetActive(value: false);
                for (int i = 0; i < playerIconSprites_EightBattle.Length; i++) {
                    playerIconSprites_EightBattle[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, BATTLE_PLAYER_SPRITE_NAMES[playerNoArray[i]]);
                }
            }
            return;
        }
        if (playerNum == 1) {
            string text = "_screen_you";
            if (Localize_Define.Language == Localize_Define.LanguageType.English) {
                text = "en" + text;
            }
            playerIconSprites_Battle[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
            playerIconSprites_Battle[0].transform.localPosition = new Vector2(-43f, 85.5f);
            playerIconSprites_Battle[0].transform.localScale = new Vector2(55f, 55f);
            for (int j = 1; j < playerIconSprites_Battle.Length; j++) {
                playerIconSprites_Battle[j].gameObject.SetActive(value: false);
                characterIconSprites_Battle[j].transform.SetLocalPositionX(15f);
            }
        } else {
            pauseSprite.gameObject.SetActive(value: false);
            for (int k = 0; k < playerIconSprites_Battle.Length; k++) {
                playerIconSprites_Battle[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, BATTLE_PLAYER_SPRITE_NAMES[playerNoArray[k]]);
            }
        }
        int num = 0;
        for (int l = 0; l < characterIconSprites_Battle.Length; l++) {
            int num2 = (l < playerNum) ? SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[l] : SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[4 + num];
            characterIconSprites_Battle[l].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARA_DEFAULT_SPRITE_NAMES[num2]);
            if (l >= playerNum) {
                num++;
            }
        }
    }
    private void InitCoopPlayerSpriteLayout() {
        if (playerNum == 2) {
            playerIconSprites_Coop_TeamA[0].transform.SetLocalPositionX(-12.5f);
            playerIconSprites_Coop_TeamA[1].transform.SetLocalPositionX(43.5f);
            playerIconSprites_Coop_TeamA[2].gameObject.SetActive(value: false);
            playerIconSprites_Coop_TeamA[3].gameObject.SetActive(value: false);
            playerIconSprites_Coop_TeamB[0].gameObject.SetActive(value: false);
            playerIconSprites_Coop_TeamB[1].gameObject.SetActive(value: false);
            return;
        }
        if (isEightBattle) {
            if (playerNum == 3) {
                playerIconSprites_Coop_TeamA[3].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, COOP_PLAYER_SPRITE_NAMES[playerGroupList[0][3]]);
                for (int i = 0; i < 3; i++) {
                    playerIconSprites_Coop_TeamA[i].transform.AddLocalPositionX(-10f);
                }
            }
            playerIconSprites_Coop_TeamB[0].gameObject.SetActive(value: false);
            playerIconSprites_Coop_TeamB[1].gameObject.SetActive(value: false);
            return;
        }
        playerIconSprites_Coop_TeamA[0].transform.SetLocalPositionX(-12.5f);
        playerIconSprites_Coop_TeamA[1].transform.SetLocalPositionX(43.5f);
        playerIconSprites_Coop_TeamA[2].gameObject.SetActive(value: false);
        playerIconSprites_Coop_TeamA[3].gameObject.SetActive(value: false);
        if (isSecondGroup) {
            if (isSecondInitEnd) {
                for (int j = 0; j < 2; j++) {
                    playerIconSprites_Coop_TeamA[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, COOP_PLAYER_SPRITE_NAMES[playerGroupList[0][j + 2]]);
                }
                if (playerGroupList[0][1] >= 4) {
                    playerIconSprites_Coop_TeamA[0].transform.AddLocalPositionX(10f);
                } else if (playerGroupList[0][3] >= 4) {
                    playerIconSprites_Coop_TeamA[0].transform.AddLocalPositionX(-10f);
                }
            } else {
                for (int k = 0; k < 2; k++) {
                    playerIconSprites_Coop_TeamA[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, COOP_PLAYER_SPRITE_NAMES[playerGroupList[0][k]]);
                }
                if (playerGroupList[0][1] >= 4) {
                    playerIconSprites_Coop_TeamA[0].transform.AddLocalPositionX(-10f);
                }
            }
            playerIconSprites_Coop_TeamB[0].gameObject.SetActive(value: false);
            playerIconSprites_Coop_TeamB[1].gameObject.SetActive(value: false);
        } else {
            for (int l = 0; l < 2; l++) {
                playerIconSprites_Coop_TeamA[l].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, COOP_PLAYER_SPRITE_NAMES[playerGroupList[0][l]]);
                playerIconSprites_Coop_TeamB[l].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, COOP_PLAYER_SPRITE_NAMES[playerGroupList[1][l]]);
            }
        }
    }
    public void SetTime(float _time, bool _isUpdate = true) {
        nowTime = _time;
        if (_isUpdate) {
            TimeUpdate();
        }
    }
    public void SetScore(int _no, int _score, bool _isUpdate = true) {
        scoreArray[_no] = _score;
        if (_isUpdate) {
            ScoreUpdate();
        }
    }
    public void SetScoreArray(int[] _scoreArray, bool _isUpdate = true) {
        scoreArray = _scoreArray;
        if (_isUpdate) {
            ScoreUpdate();
        }
    }
    public void SetScoreAnchorOrder(int[] _orderNoArray) {
        if (isEightBattle) {
            if (scoreAnchorDefaultPoses_Battle == null) {
                scoreAnchorDefaultPoses_Battle = new Vector3[scoreAnchors_EightBattle.Length];
                for (int i = 0; i < scoreAnchors_EightBattle.Length; i++) {
                    scoreAnchorDefaultPoses_Battle[i] = scoreAnchors_EightBattle[i].localPosition;
                }
            }
            for (int j = 0; j < _orderNoArray.Length; j++) {
                scoreAnchors_Battle[_orderNoArray[j]].localPosition = scoreAnchorDefaultPoses_Battle[j];
            }
            return;
        }
        if (scoreAnchorDefaultPoses_Battle == null) {
            scoreAnchorDefaultPoses_Battle = new Vector3[scoreAnchors_Battle.Length];
            for (int k = 0; k < scoreAnchors_Battle.Length; k++) {
                scoreAnchorDefaultPoses_Battle[k] = scoreAnchors_Battle[k].localPosition;
            }
        }
        for (int l = 0; l < _orderNoArray.Length; l++) {
            scoreAnchors_Battle[_orderNoArray[l]].localPosition = scoreAnchorDefaultPoses_Battle[l];
        }
    }
    public void SetScoreUnderlayColor(Color _color) {
        scoreUnderlayTurnSprite_Battle.color = _color;
    }
    public void SetScoreUndelayTurn(int _no) {
        if (isEightBattle) {
            if (!scoreUnderlayTurnSprite_EightBattle.gameObject.activeSelf) {
                scoreUnderlayTurnSprite_EightBattle.gameObject.SetActive(value: true);
            }
            scoreUnderlayTurnSprite_EightBattle.transform.parent = scoreUnderlaySprites_EightBattle[_no].transform;
            scoreUnderlayTurnSprite_EightBattle.transform.SetLocalPositionX(0f);
            scoreUnderlayTurnSprite_EightBattle.transform.SetLocalPositionY(0f);
        } else {
            if (!scoreUnderlayTurnSprite_Battle.gameObject.activeSelf) {
                scoreUnderlayTurnSprite_Battle.gameObject.SetActive(value: true);
            }
            scoreUnderlayTurnSprite_Battle.transform.parent = scoreUnderlaySprites_Battle[_no].transform;
            scoreUnderlayTurnSprite_Battle.transform.SetLocalPositionX(0f);
            scoreUnderlayTurnSprite_Battle.transform.SetLocalPositionY(0f);
        }
    }
    public void SetEndChara(int _no) {
        if (isCoop) {
            if (isSecondGroup) {
                if (_no > 1) {
                    return;
                }
                _no = playerNoArray[_no];
            }
            int i;
            for (i = 0; i < playerNoArray.Length && playerNoArray[i] != _no; i++) {
            }
            if (i == playerNoArray.Length) {
                return;
            }
            int num = teamNoArray[i];
            int num2 = 0;
            for (int j = 0; j < i; j++) {
                if (teamNoArray[j] == num) {
                    num2++;
                }
            }
            if (num == 0) {
                if (num2 < batuSprites_Coop_TeamA.Length) {
                    batuSprites_Coop_TeamA[num2].gameObject.SetActive(value: true);
                }
            } else if (num2 < batuSprites_Coop_TeamB.Length) {
                batuSprites_Coop_TeamB[num2].gameObject.SetActive(value: true);
            }
        } else {
            LeanTween.value(characterIconSprites_Battle[_no].gameObject, 0f, 1f, 1f).setOnUpdate(delegate (float _value) {
                characterIconSprites_Battle[_no].color = Color.Lerp(Color.white, END_GRAY_COLOR, _value);
            }).setOnComplete((Action)delegate {
                characterIconSprites_Battle[_no].color = END_GRAY_COLOR;
            });
        }
    }
    public void TimeUpdate() {
        commonGameTimeUI.SetTime(nowTime);
    }
    public void ScoreUpdate() {
        if (isCoop) {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < scoreArray.Length; i++) {
                int num3 = i;
                if (isSecondGroup) {
                    num3 = playerNoArray[i];
                } else if (num3 >= playerNum) {
                    num3 = 4 + num3 - playerNum;
                }
                int j;
                for (j = 0; j < playerNoArray.Length && playerNoArray[j] != num3; j++) {
                }
                if (j != playerNoArray.Length) {
                    if (teamNoArray[j] == 0) {
                        num += scoreArray[i];
                    } else {
                        num2 += scoreArray[i];
                    }
                }
            }
            scoreSpriteNumbers_Coop[0].Set(num);
            scoreSpriteNumbers_Coop[1].Set(num2);
        } else if (isEightBattle) {
            for (int k = 0; k < scoreSpriteNumbers_EightBattle.Length; k++) {
                scoreSpriteNumbers_EightBattle[k].Set(scoreArray[k]);
            }
        } else {
            for (int l = 0; l < scoreSpriteNumbers_Battle.Length; l++) {
                scoreSpriteNumbers_Battle[l].Set(scoreArray[l]);
            }
        }
    }
    private void OnDestroy() {
        LeanTween.cancel(scoreAnchors_Battle[0].gameObject);
        LeanTween.cancel(scoreAnchors_Battle[1].gameObject);
        LeanTween.cancel(scoreAnchors_Battle[2].gameObject);
        LeanTween.cancel(scoreAnchors_Battle[3].gameObject);
    }
}
