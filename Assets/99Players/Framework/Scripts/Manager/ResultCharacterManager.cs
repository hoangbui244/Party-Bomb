using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class ResultCharacterManager : SingletonCustom<ResultCharacterManager> {
    [Serializable]
    public struct CharacterSpriteData {
        [Header("キャラ立ち絵画像")]
        public SpriteRenderer standingCharaSprite;
        [Header("キャラフレ\u30fcム(大きい版)のアンカ\u30fc")]
        public Transform frameAnchor_Big;
        [Header("キャラフレ\u30fcム(大きい版)")]
        public SpriteRenderer frameSprite_Big;
        [Header("キャラフレ\u30fcムのキャラ立ち絵画像(大きい版)")]
        public SpriteRenderer frameStandingCharaSprite_Big;
        [Header("キャラフレ\u30fcムの王冠画像")]
        public SpriteRenderer kingIcon;
        [Header("キャラフレ\u30fcム(小さい版)のアンカ\u30fc")]
        public Transform frameAnchor_Small;
        [Header("キャラフレ\u30fcム(小さい版)")]
        public SpriteRenderer frameSprite_Small;
        [Header("キャラフレ\u30fcムのキャラ立ち絵画像(小さい版)")]
        public SpriteRenderer frameStandingCharaSprite_Small;
        [Header("[通常]表情のキャラクタ\u30fc画像")]
        [NonReorderable]
        public Sprite[] characterSprite_normal;
        [Header("[喜び]表情のキャラクタ\u30fc画像")]
        [NonReorderable]
        public Sprite[] characterSprite_happy;
        [Header("[悲しい]表情のキャラクタ\u30fc画像")]
        [NonReorderable]
        public Sprite[] characterSprite_sad;
    }
    [Serializable]
    public struct ShowCharaData {
        public ResultGameDataParams.CharaType charaType;
        public int faceNo;
        public FaceType faceType;
    }
    [Serializable]
    public class TeamDataList {
        public List<int> playerNoList;
        public List<int> teamNoList;
    }
    public enum FaceType {
        Normal,
        Happy,
        Sad
    }
    [SerializeField]
    [Header("ル\u30fcトオブジェクト")]
    private GameObject mCharacterRoot;
    [SerializeField]
    [Header("１人用のキャラ画像の親アンカ\u30fc")]
    private Transform mCharacterParentAnchor_One;
    [SerializeField]
    [Header("２人用のキャラ画像の親アンカ\u30fc")]
    private Transform mCharacterParentAnchor_Two;
    [SerializeField]
    [Header("３～４人用のキャラ画像の親アンカ\u30fc")]
    private Transform mCharacterParentAnchor_ThreeOrFour;
    [SerializeField]
    [Header("【ゆうと】キャラ画像デ\u30fcタ")]
    private CharacterSpriteData mCharaSpriteData_Yuto;
    [SerializeField]
    [Header("【ひな】キャラ画像デ\u30fcタ")]
    private CharacterSpriteData mCharaSpriteData_Hina;
    [SerializeField]
    [Header("【いつき】キャラ画像デ\u30fcタ")]
    private CharacterSpriteData mCharaSpriteData_Itsuki;
    [SerializeField]
    [Header("【そうた】キャラ画像デ\u30fcタ")]
    private CharacterSpriteData mCharaSpriteData_Souta;
    [SerializeField]
    [Header("【たくみ】キャラ画像デ\u30fcタ")]
    private CharacterSpriteData mCharaSpriteData_Takumi;
    [SerializeField]
    [Header("【りん】キャラ画像デ\u30fcタ")]
    private CharacterSpriteData mCharaSpriteData_Rin;
    [SerializeField]
    [Header("【あきら】キャラ画像デ\u30fcタ")]
    private CharacterSpriteData mCharaSpriteData_Akira;
    [SerializeField]
    [Header("【るい】キャラ画像デ\u30fcタ")]
    private CharacterSpriteData mCharaSpriteData_Rui;
    [SerializeField]
    [Header("キャラフレ\u30fcムが２つの時のアンカ\u30fc")]
    private Transform[] mFrameAnchor_Two;
    [SerializeField]
    [Header("キャラフレ\u30fcムが３つの時のアンカ\u30fc")]
    private Transform[] mFrameAnchor_Three;
    [SerializeField]
    [Header("キャラフレ\u30fcムが４つの時のアンカ\u30fc")]
    private Transform[] mFrameAnchor_Four;
    private List<CharacterSpriteData> characterSpriteDataList = new List<CharacterSpriteData>();
    private List<ShowCharaData> showCharaDataList = new List<ShowCharaData>();
    private bool isShowCharaNum_One;
    private bool isShowCharaNum_Two;
    private bool isShowCharaNum_Three;
    private bool isShowCharaNum_Four;
    private Vector3 def_ShowStandingCharaSpritePos = Vector3.zero;
    private int winnerTeamNo = -1;
    private int loserTeamNo = -1;
    [SerializeField]
    [Header("勝敗リザルト")]
    private WinOrLoseResultManager winOrLoseResultManager;
    private const float CHARACTER_SLIDE_IN_TIME = 0.5f;
    private const float CHARACTER_FADE_IN_TIME = 0.5f;
    private void Awake() {
        mCharacterRoot.SetActive(value: true);
        mCharacterParentAnchor_One.gameObject.SetActive(value: false);
        mCharacterParentAnchor_Two.gameObject.SetActive(value: false);
        mCharacterParentAnchor_ThreeOrFour.gameObject.SetActive(value: false);
        characterSpriteDataList.Clear();
        showCharaDataList.Clear();
        characterSpriteDataList.Add(mCharaSpriteData_Yuto);
        characterSpriteDataList.Add(mCharaSpriteData_Hina);
        characterSpriteDataList.Add(mCharaSpriteData_Itsuki);
        characterSpriteDataList.Add(mCharaSpriteData_Souta);
        characterSpriteDataList.Add(mCharaSpriteData_Takumi);
        characterSpriteDataList.Add(mCharaSpriteData_Rin);
        characterSpriteDataList.Add(mCharaSpriteData_Akira);
        characterSpriteDataList.Add(mCharaSpriteData_Rui);
        InitCharacterSprite();
    }
    public void Show() {
        mCharacterRoot.SetActive(value: true);
    }
    public void Hide() {
        mCharacterRoot.SetActive(value: false);
    }
    private void InitCharacterSprite() {
        for (int i = 0; i < characterSpriteDataList.Count; i++) {
            characterSpriteDataList[i].standingCharaSprite.gameObject.SetActive(value: false);
            characterSpriteDataList[i].frameSprite_Big.SetAlpha(0f);
            characterSpriteDataList[i].frameStandingCharaSprite_Big.SetAlpha(0f);
            characterSpriteDataList[i].frameSprite_Small.SetAlpha(0f);
            characterSpriteDataList[i].frameStandingCharaSprite_Small.SetAlpha(0f);
        }
    }
    public void SetCharacter_WinOrLose(FaceType _faceType, int _showTeamNo) {
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            ShowCharaData showCharaData = default(ShowCharaData);
            showCharaData.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_showTeamNo][0]];
            showCharaData.faceType = _faceType;
            switch (showCharaData.faceType) {
                case FaceType.Normal:
                    showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_normal.Length);
                    break;
                case FaceType.Happy:
                    showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_happy.Length);
                    break;
                case FaceType.Sad:
                    showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_sad.Length);
                    break;
            }
            showCharaDataList.Add(showCharaData);
        } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
            for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerNum; i++) {
                ShowCharaData showCharaData2 = default(ShowCharaData);
                showCharaData2.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][i]];
                showCharaData2.faceType = _faceType;
                switch (showCharaData2.faceType) {
                    case FaceType.Normal:
                        showCharaData2.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData2.charaType].characterSprite_normal.Length);
                        break;
                    case FaceType.Happy:
                        showCharaData2.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData2.charaType].characterSprite_happy.Length);
                        break;
                    case FaceType.Sad:
                        showCharaData2.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData2.charaType].characterSprite_sad.Length);
                        break;
                }
                showCharaDataList.Add(showCharaData2);
            }
        } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
            if (_faceType == FaceType.Normal) {
                winnerTeamNo = 0;
                loserTeamNo = 1;
            } else {
                winnerTeamNo = _showTeamNo;
                loserTeamNo = ((winnerTeamNo == 0) ? 1 : 0);
            }
            for (int j = 0; j < winOrLoseResultManager.GetTeamPlayerGroupList()[winnerTeamNo].Count; j++) {
                ShowCharaData showCharaData3 = default(ShowCharaData);
                showCharaData3.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[winOrLoseResultManager.GetTeamPlayerGroupList()[winnerTeamNo][j]];
                if (_faceType == FaceType.Normal) {
                    showCharaData3.faceType = FaceType.Normal;
                } else {
                    showCharaData3.faceType = ((!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) ? FaceType.Happy : _faceType);
                }
                switch (showCharaData3.faceType) {
                    case FaceType.Normal:
                        showCharaData3.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData3.charaType].characterSprite_normal.Length);
                        break;
                    case FaceType.Happy:
                        showCharaData3.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData3.charaType].characterSprite_happy.Length);
                        break;
                    case FaceType.Sad:
                        showCharaData3.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData3.charaType].characterSprite_sad.Length);
                        break;
                }
                showCharaDataList.Add(showCharaData3);
            }
            for (int k = 0; k < winOrLoseResultManager.GetTeamPlayerGroupList()[loserTeamNo].Count; k++) {
                ShowCharaData showCharaData4 = default(ShowCharaData);
                showCharaData4.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[winOrLoseResultManager.GetTeamPlayerGroupList()[loserTeamNo][k]];
                if (_faceType == FaceType.Normal) {
                    showCharaData4.faceType = FaceType.Normal;
                } else {
                    showCharaData4.faceType = FaceType.Sad;
                }
                switch (showCharaData4.faceType) {
                    case FaceType.Normal:
                        showCharaData4.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData4.charaType].characterSprite_normal.Length);
                        break;
                    case FaceType.Happy:
                        showCharaData4.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData4.charaType].characterSprite_happy.Length);
                        break;
                    case FaceType.Sad:
                        showCharaData4.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData4.charaType].characterSprite_sad.Length);
                        break;
                }
                showCharaDataList.Add(showCharaData4);
            }
        }
        SetCharacterSpriteData();
    }
    public void SetCharacter_Ranking(bool _showLastResult) {
        if (_showLastResult) {
            ResultGameDataParams.GetNowSceneType();
            CharacterInit_Point();
        } else if (SingletonCustom<GameSettingManager>.Instance.IsEightBattle) {
            CharacterInit_8Rank();
        } else {
            CharacterInit_Rank();
        }
    }
    public void CharacterInit_Rank() {
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            List<int> list = new List<int>();
            for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++) {
                int num = i;
                if (num >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                    num = 6 + (i - SingletonCustom<GameSettingManager>.Instance.PlayerNum);
                }
                if (ResultGameDataParams.GetPlayerRank(num, _isGroup1: true) == 0) {
                    list.Add(i);
                }
            }
            for (int j = 0; j < list.Count; j++) {
                if (SingletonCustom<SceneManager>.Instance.GetNowScene() == SceneManager.SceneType.RESULT_ANNOUNCEMENT && showCharaDataList.Count >= 1) {
                    UnityEngine.Debug.Log("nowScene:" + SingletonCustom<SceneManager>.Instance.GetNowScene().ToString());
                    break;
                }
                if (showCharaDataList.Count >= 2) {
                    break;
                }
                ShowCharaData item = default(ShowCharaData);
                item.charaType = (ResultGameDataParams.CharaType)list[j];
                UnityEngine.Debug.Log("charatype" + item.charaType.ToString());
                showCharaDataList.Add(item);
            }
        } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) {
            for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerNum; k++) {
                ShowCharaData showCharaData = default(ShowCharaData);
                showCharaData.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][k]];
                if (ResultGameDataParams.GetFirstRankCPUNo(0, 0, _isGroup1: true).Length != 0) {
                    switch (ResultGameDataParams.GetPlayerRank(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][k], _isGroup1: true)) {
                        case 0:
                        case 1:
                            showCharaData.faceType = FaceType.Happy;
                            break;
                        case 2:
                        case 3:
                            showCharaData.faceType = FaceType.Normal;
                            break;
                    }
                    UnityEngine.Debug.Log("チ\u30fcムAのCPUが１位に入賞！");
                } else if (ResultGameDataParams.GetRankPlayerNo(0, 0, _isGroup1: true).Length != 0) {
                    switch (ResultGameDataParams.GetPlayerRank(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][k], _isGroup1: true)) {
                        case 0:
                        case 1:
                            showCharaData.faceType = FaceType.Happy;
                            break;
                        case 2:
                        case 3:
                            showCharaData.faceType = FaceType.Normal;
                            break;
                    }
                    UnityEngine.Debug.Log("チ\u30fcムAのプレイヤ\u30fcが１位に入賞！");
                } else {
                    showCharaData.faceType = FaceType.Sad;
                }
                switch (showCharaData.faceType) {
                    case FaceType.Normal:
                        showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_normal.Length);
                        break;
                    case FaceType.Happy:
                        showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_happy.Length);
                        break;
                    case FaceType.Sad:
                        showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_sad.Length);
                        break;
                }
                showCharaDataList.Add(showCharaData);
            }
        } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
            if (ResultGameDataParams.GetRankPlayerNo(0, 0, _isGroup1: true).Length != 0 && ResultGameDataParams.GetRankPlayerNo(1, 0, _isGroup1: true).Length != 0) {
                UnityEngine.Debug.Log("両チ\u30fcムが勝利！");
                for (int l = 0; l < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0].Count; l++) {
                    ShowCharaData showCharaData2 = default(ShowCharaData);
                    showCharaData2.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][l]];
                    switch (ResultGameDataParams.GetPlayerRank(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][l], _isGroup1: true)) {
                        case 0:
                        case 1:
                            showCharaData2.faceType = FaceType.Happy;
                            break;
                        case 2:
                        case 3:
                            showCharaData2.faceType = FaceType.Normal;
                            break;
                    }
                    switch (showCharaData2.faceType) {
                        case FaceType.Normal:
                            showCharaData2.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData2.charaType].characterSprite_normal.Length);
                            break;
                        case FaceType.Happy:
                            showCharaData2.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData2.charaType].characterSprite_happy.Length);
                            break;
                        case FaceType.Sad:
                            showCharaData2.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData2.charaType].characterSprite_sad.Length);
                            break;
                    }
                    showCharaDataList.Add(showCharaData2);
                }
                for (int m = 0; m < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1].Count; m++) {
                    ShowCharaData showCharaData3 = default(ShowCharaData);
                    showCharaData3.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1][m]];
                    switch (ResultGameDataParams.GetPlayerRank(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1][m], _isGroup1: true)) {
                        case 0:
                        case 1:
                            showCharaData3.faceType = FaceType.Happy;
                            break;
                        case 2:
                        case 3:
                            showCharaData3.faceType = FaceType.Normal;
                            break;
                    }
                    switch (showCharaData3.faceType) {
                        case FaceType.Normal:
                            showCharaData3.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData3.charaType].characterSprite_normal.Length);
                            break;
                        case FaceType.Happy:
                            showCharaData3.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData3.charaType].characterSprite_happy.Length);
                            break;
                        case FaceType.Sad:
                            showCharaData3.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData3.charaType].characterSprite_sad.Length);
                            break;
                    }
                    showCharaDataList.Add(showCharaData3);
                }
            } else if (ResultGameDataParams.GetRankPlayerNo(0, 0, _isGroup1: true).Length != 0) {
                UnityEngine.Debug.Log("チ\u30fcムAが勝利！");
                for (int n = 0; n < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0].Count; n++) {
                    ShowCharaData showCharaData4 = default(ShowCharaData);
                    showCharaData4.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][n]];
                    switch (ResultGameDataParams.GetPlayerRank(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][n], _isGroup1: true)) {
                        case 0:
                        case 1:
                            showCharaData4.faceType = FaceType.Happy;
                            break;
                        case 2:
                        case 3:
                            showCharaData4.faceType = FaceType.Normal;
                            break;
                    }
                    switch (showCharaData4.faceType) {
                        case FaceType.Normal:
                            showCharaData4.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData4.charaType].characterSprite_normal.Length);
                            break;
                        case FaceType.Happy:
                            showCharaData4.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData4.charaType].characterSprite_happy.Length);
                            break;
                        case FaceType.Sad:
                            showCharaData4.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData4.charaType].characterSprite_sad.Length);
                            break;
                    }
                    showCharaDataList.Add(showCharaData4);
                }
                for (int num2 = 0; num2 < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1].Count; num2++) {
                    ShowCharaData showCharaData5 = default(ShowCharaData);
                    showCharaData5.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1][num2]];
                    showCharaData5.faceType = FaceType.Sad;
                    switch (showCharaData5.faceType) {
                        case FaceType.Normal:
                            showCharaData5.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData5.charaType].characterSprite_normal.Length);
                            break;
                        case FaceType.Happy:
                            showCharaData5.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData5.charaType].characterSprite_happy.Length);
                            break;
                        case FaceType.Sad:
                            showCharaData5.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData5.charaType].characterSprite_sad.Length);
                            break;
                    }
                    showCharaDataList.Add(showCharaData5);
                }
            } else if (ResultGameDataParams.GetRankPlayerNo(1, 0, _isGroup1: true).Length != 0) {
                UnityEngine.Debug.Log("チ\u30fcムBが勝利！");
                for (int num3 = 0; num3 < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1].Count; num3++) {
                    ShowCharaData showCharaData6 = default(ShowCharaData);
                    showCharaData6.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1][num3]];
                    switch (ResultGameDataParams.GetPlayerRank(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1][num3], _isGroup1: true)) {
                        case 0:
                        case 1:
                            showCharaData6.faceType = FaceType.Happy;
                            break;
                        case 2:
                        case 3:
                            showCharaData6.faceType = FaceType.Normal;
                            break;
                    }
                    switch (showCharaData6.faceType) {
                        case FaceType.Normal:
                            showCharaData6.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData6.charaType].characterSprite_normal.Length);
                            break;
                        case FaceType.Happy:
                            showCharaData6.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData6.charaType].characterSprite_happy.Length);
                            break;
                        case FaceType.Sad:
                            showCharaData6.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData6.charaType].characterSprite_sad.Length);
                            break;
                    }
                    showCharaDataList.Add(showCharaData6);
                }
                for (int num4 = 0; num4 < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0].Count; num4++) {
                    ShowCharaData showCharaData7 = default(ShowCharaData);
                    showCharaData7.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][num4]];
                    showCharaData7.faceType = FaceType.Sad;
                    switch (showCharaData7.faceType) {
                        case FaceType.Normal:
                            showCharaData7.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData7.charaType].characterSprite_normal.Length);
                            break;
                        case FaceType.Happy:
                            showCharaData7.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData7.charaType].characterSprite_happy.Length);
                            break;
                        case FaceType.Sad:
                            showCharaData7.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData7.charaType].characterSprite_sad.Length);
                            break;
                    }
                    showCharaDataList.Add(showCharaData7);
                }
            }
        }
        SetCharacterSpriteData();
    }
    public void CharacterInit_8Rank() {
        if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            List<int> list = new List<int>();
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerNum; j++) {
                    if (ResultGameDataParams.GetPlayer8Rank(j) == i) {
                        list.Add(j);
                    }
                }
            }
            for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerNum; k++) {
                ShowCharaData showCharaData = default(ShowCharaData);
                showCharaData.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list[k]];
                if (ResultGameDataParams.GetPlayer8Rank(list[k]) < 2) {
                    showCharaData.faceType = FaceType.Happy;
                } else if (ResultGameDataParams.GetPlayer8Rank(list[k]) < 4) {
                    showCharaData.faceType = FaceType.Normal;
                } else if (ResultGameDataParams.GetPlayer8Rank(list[k]) < 6) {
                    showCharaData.faceType = FaceType.Sad;
                } else if (ResultGameDataParams.GetPlayer8Rank(list[k]) < 8) {
                    showCharaData.faceType = FaceType.Sad;
                }
                switch (showCharaData.faceType) {
                    case FaceType.Normal:
                        showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_normal.Length);
                        break;
                    case FaceType.Happy:
                        showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_happy.Length);
                        break;
                    case FaceType.Sad:
                        showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_sad.Length);
                        break;
                }
                showCharaDataList.Add(showCharaData);
            }
        }
        SetCharacterSpriteData();
    }
    private void CharacterInit_Point() {
        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerNum; i++) {
            ShowCharaData showCharaData = default(ShowCharaData);
            showCharaData.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][i]];
            if (ResultGameDataParams.GetTeamTotalPoint(0) > ResultGameDataParams.GetTeamTotalPoint(1)) {
                showCharaData.faceType = FaceType.Happy;
            } else if (ResultGameDataParams.GetTeamTotalPoint(1) > ResultGameDataParams.GetTeamTotalPoint(0)) {
                showCharaData.faceType = FaceType.Sad;
            } else {
                showCharaData.faceType = FaceType.Normal;
            }
            switch (showCharaData.faceType) {
                case FaceType.Normal:
                    showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_normal.Length);
                    break;
                case FaceType.Happy:
                    showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_happy.Length);
                    break;
                case FaceType.Sad:
                    showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_sad.Length);
                    break;
            }
            showCharaDataList.Add(showCharaData);
        }
        SetCharacterSpriteData();
    }
    private void CharacterInit_Record() {
        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerNum; i++) {
            ShowCharaData showCharaData = default(ShowCharaData);
            showCharaData.charaType = (ResultGameDataParams.CharaType)SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][i]];
            if (ResultGameDataParams.GetTeamTotalRecord(0) > ResultGameDataParams.GetTeamTotalRecord(1)) {
                showCharaData.faceType = FaceType.Happy;
            } else if (ResultGameDataParams.GetTeamTotalRecord(1) > ResultGameDataParams.GetTeamTotalRecord(0)) {
                showCharaData.faceType = FaceType.Sad;
            } else {
                showCharaData.faceType = FaceType.Normal;
            }
            switch (showCharaData.faceType) {
                case FaceType.Normal:
                    showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_normal.Length);
                    break;
                case FaceType.Happy:
                    showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_happy.Length);
                    break;
                case FaceType.Sad:
                    showCharaData.faceNo = UnityEngine.Random.Range(0, characterSpriteDataList[(int)showCharaData.charaType].characterSprite_sad.Length);
                    break;
            }
            showCharaDataList.Add(showCharaData);
        }
        SetCharacterSpriteData();
    }
    private void SetCharacterSpriteData() {
        isShowCharaNum_One = (showCharaDataList.Count == 1);
        isShowCharaNum_Two = (showCharaDataList.Count >= 2);
        isShowCharaNum_Three = false;
        isShowCharaNum_Four = false;
        UnityEngine.Debug.Log("★★showCharaDataList.Count:" + showCharaDataList.Count.ToString());
        mCharacterParentAnchor_One.gameObject.SetActive(isShowCharaNum_One);
        mCharacterParentAnchor_Two.gameObject.SetActive(isShowCharaNum_Two);
        mCharacterParentAnchor_ThreeOrFour.gameObject.SetActive(isShowCharaNum_Three || isShowCharaNum_Four);
        for (int i = 0; i < showCharaDataList.Count; i++) {
            if (isShowCharaNum_One) {
                def_ShowStandingCharaSpritePos = characterSpriteDataList[(int)showCharaDataList[i].charaType].standingCharaSprite.transform.localPosition;
                if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.ALL_SPORTS && SingletonCustom<SceneManager>.Instance.GetNowScene() == SceneManager.SceneType.RESULT_ANNOUNCEMENT) {
                    characterSpriteDataList[(int)showCharaDataList[i].charaType].kingIcon.gameObject.SetActive(value: true);
                    mCharacterParentAnchor_One.AddLocalPositionX(1000f);
                } else {
                    mCharacterParentAnchor_One.AddLocalPositionX(-1000f);
                }
                characterSpriteDataList[(int)showCharaDataList[i].charaType].standingCharaSprite.gameObject.SetActive(value: true);
                switch (showCharaDataList[i].faceType) {
                    case FaceType.Normal:
                        characterSpriteDataList[(int)showCharaDataList[i].charaType].standingCharaSprite.sprite = characterSpriteDataList[(int)showCharaDataList[i].charaType].characterSprite_normal[showCharaDataList[i].faceNo];
                        break;
                    case FaceType.Happy:
                        characterSpriteDataList[(int)showCharaDataList[i].charaType].standingCharaSprite.sprite = characterSpriteDataList[(int)showCharaDataList[i].charaType].characterSprite_happy[showCharaDataList[i].faceNo];
                        break;
                    case FaceType.Sad:
                        characterSpriteDataList[(int)showCharaDataList[i].charaType].standingCharaSprite.sprite = characterSpriteDataList[(int)showCharaDataList[i].charaType].characterSprite_sad[showCharaDataList[i].faceNo];
                        break;
                }
            } else if (isShowCharaNum_Two) {
                int charaType = (int)showCharaDataList[i].charaType;
                UnityEngine.Debug.Log("charaType:" + charaType.ToString());
                UnityEngine.Debug.Log("listLen:" + characterSpriteDataList.Count.ToString());
                characterSpriteDataList[(int)showCharaDataList[i].charaType].frameAnchor_Big.parent = mFrameAnchor_Two[i];
            } else if (isShowCharaNum_Three) {
                characterSpriteDataList[(int)showCharaDataList[i].charaType].frameAnchor_Small.parent = mFrameAnchor_Three[i];
                characterSpriteDataList[(int)showCharaDataList[i].charaType].frameAnchor_Small.transform.localPosition = Vector3.zero;
                switch (showCharaDataList[i].faceType) {
                    case FaceType.Normal:
                        characterSpriteDataList[(int)showCharaDataList[i].charaType].frameStandingCharaSprite_Small.sprite = characterSpriteDataList[(int)showCharaDataList[i].charaType].characterSprite_normal[showCharaDataList[i].faceNo];
                        break;
                    case FaceType.Happy:
                        characterSpriteDataList[(int)showCharaDataList[i].charaType].frameStandingCharaSprite_Small.sprite = characterSpriteDataList[(int)showCharaDataList[i].charaType].characterSprite_happy[showCharaDataList[i].faceNo];
                        break;
                    case FaceType.Sad:
                        characterSpriteDataList[(int)showCharaDataList[i].charaType].frameStandingCharaSprite_Small.sprite = characterSpriteDataList[(int)showCharaDataList[i].charaType].characterSprite_sad[showCharaDataList[i].faceNo];
                        break;
                }
            } else if (isShowCharaNum_Four) {
                characterSpriteDataList[(int)showCharaDataList[i].charaType].frameAnchor_Small.parent = mFrameAnchor_Four[i];
                characterSpriteDataList[(int)showCharaDataList[i].charaType].frameAnchor_Small.transform.localPosition = Vector3.zero;
                switch (showCharaDataList[i].faceType) {
                    case FaceType.Normal:
                        characterSpriteDataList[(int)showCharaDataList[i].charaType].frameStandingCharaSprite_Small.sprite = characterSpriteDataList[(int)showCharaDataList[i].charaType].characterSprite_normal[showCharaDataList[i].faceNo];
                        break;
                    case FaceType.Happy:
                        characterSpriteDataList[(int)showCharaDataList[i].charaType].frameStandingCharaSprite_Small.sprite = characterSpriteDataList[(int)showCharaDataList[i].charaType].characterSprite_happy[showCharaDataList[i].faceNo];
                        break;
                    case FaceType.Sad:
                        characterSpriteDataList[(int)showCharaDataList[i].charaType].frameStandingCharaSprite_Small.sprite = characterSpriteDataList[(int)showCharaDataList[i].charaType].characterSprite_sad[showCharaDataList[i].faceNo];
                        break;
                }
            }
        }
        CharacterAddjustPosition();
    }
    private void CharacterAddjustPosition() {
        if (isShowCharaNum_One) {
            for (int i = 0; i < showCharaDataList.Count; i++) {
                if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.ALL_SPORTS && SingletonCustom<SceneManager>.Instance.GetNowScene() == SceneManager.SceneType.RESULT_ANNOUNCEMENT) {
                    characterSpriteDataList[(int)showCharaDataList[i].charaType].standingCharaSprite.transform.localPosition = ResultCharacterPosData.GetCharacterPosition(ResultCharacterPosData.ShowCharaNumType.Single, (int)showCharaDataList[i].charaType, showCharaDataList[i].faceType, showCharaDataList[i].faceNo);
                }
            }
        } else {
            if (!isShowCharaNum_Two) {
                return;
            }
            for (int j = 0; j < showCharaDataList.Count; j++) {
                if (j == 0) {
                    characterSpriteDataList[(int)showCharaDataList[j].charaType].frameStandingCharaSprite_Big.sortingOrder = 1;
                }
                if (j == 1) {
                    characterSpriteDataList[(int)showCharaDataList[j].charaType].frameStandingCharaSprite_Big.transform.AddLocalPositionX(350f);
                }
            }
        }
    }
    public void StartPhotoAnimation(Action _endCallBack = null) {
        StartCoroutine(CharacterAnimation(_endCallBack));
    }
    private IEnumerator CharacterAnimation(Action _endCallBack) {
        if (isShowCharaNum_One) {
            for (int i = 0; i < showCharaDataList.Count; i++) {
                characterSpriteDataList[(int)showCharaDataList[i].charaType].standingCharaSprite.gameObject.SetActive(value: true);
                LeanTween.moveLocalX(mCharacterParentAnchor_One.gameObject, 0f, 0.5f).setEaseOutQuad();
            }
        } else if (isShowCharaNum_Two) {
            for (int j = 0; j < showCharaDataList.Count; j++) {
                characterSpriteDataList[(int)showCharaDataList[j].charaType].frameStandingCharaSprite_Big.SetAlpha(1f);
                LeanTween.moveLocalX(characterSpriteDataList[(int)showCharaDataList[j].charaType].frameStandingCharaSprite_Big.gameObject, characterSpriteDataList[(int)showCharaDataList[j].charaType].frameStandingCharaSprite_Big.transform.localPosition.x, 0.5f).setEaseOutQuad();
                characterSpriteDataList[(int)showCharaDataList[j].charaType].frameStandingCharaSprite_Big.transform.SetLocalPositionX(-1000f);
            }
        } else if (isShowCharaNum_Three || isShowCharaNum_Four) {
            for (int k = 0; k < showCharaDataList.Count; k++) {
                StartCoroutine(SetAlphaColor(characterSpriteDataList[(int)showCharaDataList[k].charaType].frameSprite_Small, 0.5f));
                StartCoroutine(SetAlphaColor(characterSpriteDataList[(int)showCharaDataList[k].charaType].frameStandingCharaSprite_Small, 0.5f));
            }
        }
        yield return new WaitForSeconds(0.5f);
        if (_endCallBack != null) {
            LeanTween.delayedCall(0.5f, _endCallBack);
        }
    }
    public bool IsCharacterFaceType(FaceType _faceType) {
        for (int i = 0; i < showCharaDataList.Count; i++) {
            if (showCharaDataList[i].faceType == _faceType) {
                return true;
            }
        }
        return false;
    }
    public int GetShowCharacterNum() {
        return showCharaDataList.Count;
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
