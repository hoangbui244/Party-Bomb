using GamepadInput;
using System;
using UnityEngine;
public class RA_ResultDetail : MonoBehaviour {
    [Serializable]
    public struct PageRootAnchor_Layout4 {
        [Header("UIル\u30fcトアンカ\u30fc")]
        public Transform[] uiRootAnchor;
    }
    [SerializeField]
    [Header("２チ\u30fcム用ペ\u30fcジレイアウト")]
    private GameObject pageLayout_2;
    [SerializeField]
    [Header("４チ\u30fcム用ペ\u30fcジレイアウト")]
    private GameObject pageLayout_4;
    [SerializeField]
    [Header("２チ\u30fcム用競技単位ランキングリスト")]
    private RA_DetailRankingList[] arrayDetailRankingList_Layout2;
    [SerializeField]
    [Header("４チ\u30fcム用競技単位ランキングリスト")]
    private RA_DetailRankingList[] arrayDetailRankingList_Layout4;
    [SerializeField]
    [Header("２チ\u30fcム用のペ\u30fcジル\u30fcトアンカ\u30fc")]
    private Transform[] arrayPageAnchor_Layout2;
    [SerializeField]
    [Header("４チ\u30fcム用のペ\u30fcジル\u30fcトアンカ\u30fc")]
    private PageRootAnchor_Layout4[] arrayPageAnchor_Layout4;
    [SerializeField]
    [Header("左矢印")]
    private Transform leftArrow;
    [SerializeField]
    [Header("右矢印")]
    private Transform rightArrow;
    [SerializeField]
    [Header("ペ\u30fcジ数表示")]
    private SpriteRenderer spPageIndex;
    private readonly int PAGE_MAX = 2;
    private int pageIdx;
    private int tempRank;
    private int tempIdx;
    private int tempTeamNo;
    private int tempCharacterNo;
    private ResultGameDataParams.PlayKingDetailsData[] playKingDetailsData;
    private const int SHOW_PAGE_RECORD_MAX_NUM = 5;
    private readonly int[] SHOW_PAGE_RECORD_NUM = new int[2]
    {
        5,
        5
    };
    public bool IsTween {
        get;
        set;
    }
    public void Show() {
        base.gameObject.SetActive(value: true);
        pageIdx = 0;
        switch (SingletonCustom<GameSettingManager>.Instance.TeamNum) {
            case 2:
                pageLayout_2.SetActive(value: true);
                pageLayout_4.SetActive(value: false);
                leftArrow.SetLocalPositionX(-554f);
                rightArrow.SetLocalPositionX(554f);
                break;
            case 4:
                pageLayout_2.SetActive(value: false);
                pageLayout_4.SetActive(value: true);
                leftArrow.SetLocalPositionX(-760f);
                rightArrow.SetLocalPositionX(760f);
                break;
        }
        for (int i = 0; i < arrayPageAnchor_Layout2.Length; i++) {
            arrayPageAnchor_Layout2[i].SetLocalPositionX((float)i * 1800f);
        }
        for (int j = 0; j < arrayPageAnchor_Layout2.Length; j++) {
            LeanTween.cancel(arrayPageAnchor_Layout2[j].gameObject);
        }
        for (int k = 0; k < arrayPageAnchor_Layout4.Length; k++) {
            for (int l = 0; l < arrayPageAnchor_Layout4[k].uiRootAnchor.Length; l++) {
                arrayPageAnchor_Layout4[k].uiRootAnchor[l].SetLocalPositionX((float)k * 1800f);
            }
        }
        for (int m = 0; m < arrayPageAnchor_Layout4.Length; m++) {
            for (int n = 0; n < arrayPageAnchor_Layout4[m].uiRootAnchor.Length; n++) {
                LeanTween.cancel(arrayPageAnchor_Layout4[m].uiRootAnchor[n].gameObject);
            }
        }
        RefreshPage();
        IsTween = false;
    }
    public void Hide() {
        base.gameObject.SetActive(value: false);
    }
    private void RefreshPage() {
        spPageIndex.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_page_0" + pageIdx.ToString());
        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.SPORTS_DAY_SPORTS_NUM; i++) {
            string text = "";
            if (Localize_Define.Language == Localize_Define.LanguageType.English) {
                text += "en";
            }
            switch (i) {
                case 0:
                    text += "_SpearBattle";
                    break;
                case 1:
                    text += "_ArcherBattle_02_result_details";
                    break;
                case 2:
                    text += "_BigMerchant_02_result_details";
                    break;
                case 3:
                    text += "_MonsterRace_02_result_details";
                    break;
                case 4:
                    text += "_MakingPotion_02_result_details";
                    break;
                case 5:
                    text += "_MonsterKill_02_result_details";
                    break;
                case 6:
                    text += "_Blacksmith";
                    break;
                case 7:
                    text += "_LegendarySword";
                    break;
                case 8:
                    text += "_DragonBattle_02_result_details";
                    break;
                case 9:
                    text += "_ArenaBattle_02_result_details";
                    break;
            }
            if (pageLayout_2.activeSelf) {
                arrayDetailRankingList_Layout2[i].SetPlayName(SingletonCustom<SAManager>.Instance.GetSprite(SAType.GameStart, text));
            } else if (pageLayout_4.activeSelf) {
                arrayDetailRankingList_Layout4[i].SetPlayName(SingletonCustom<SAManager>.Instance.GetSprite(SAType.GameStart, text));
            }
        }
        playKingDetailsData = ResultGameDataParams.GetPlayKingDetailsData();
        if (pageLayout_2.activeSelf) {
            for (int j = 0; j < arrayPageAnchor_Layout2.Length; j++) {
                for (int k = 0; k < SHOW_PAGE_RECORD_NUM[j]; k++) {
                    for (int l = 0; l < SingletonCustom<GameSettingManager>.Instance.TeamNum; l++) {
                        tempRank = playKingDetailsData[k + j * 5].rankNoArray[l];
                        tempTeamNo = playKingDetailsData[k + j * 5].teamNoArray[l];
                        arrayDetailRankingList_Layout2[k + j * 5].SetRankingData(l, tempRank);
                        arrayDetailRankingList_Layout2[k + j * 5].SetTeamData(l, tempTeamNo);
                    }
                }
            }
            return;
        }
        for (int m = 0; m < arrayPageAnchor_Layout2.Length; m++) {
            for (int n = 0; n < SHOW_PAGE_RECORD_NUM[m]; n++) {
                for (int num = 0; num < SingletonCustom<GameSettingManager>.Instance.TeamNum; num++) {
                    tempRank = playKingDetailsData[n + m * 5].rankNoArray[num];
                    tempTeamNo = playKingDetailsData[n + m * 5].teamNoArray[num];
                    tempCharacterNo = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[tempTeamNo][0]];
                    arrayDetailRankingList_Layout4[n + m * 5].SetRankingData(num, tempRank);
                    arrayDetailRankingList_Layout4[n + m * 5].SetCharacterIconData(num, tempRank, tempCharacterNo);
                }
            }
        }
    }
    private void Update() {
        if (!IsTween && base.gameObject.activeSelf) {
            if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.RightShoulder) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.RightTrigger)) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                tempIdx = pageIdx;
                pageIdx = (pageIdx + 1) % PAGE_MAX;
                PageChange(_isRight: false);
            } else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.LeftShoulder) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.LeftTrigger)) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                tempIdx = pageIdx;
                pageIdx = (pageIdx + (PAGE_MAX - 1)) % PAGE_MAX;
                PageChange(_isRight: true);
            }
        }
    }
    private void PageChange(bool _isRight) {
        if (_isRight) {
            if (pageLayout_2.activeSelf) {
                arrayPageAnchor_Layout2[pageIdx].SetLocalPositionX(-1800f);
                LeanTween.moveLocalX(arrayPageAnchor_Layout2[pageIdx].gameObject, 0f, 0.4f).setOnComplete((Action)delegate {
                    IsTween = false;
                }).setEaseOutQuart();
                LeanTween.moveLocalX(arrayPageAnchor_Layout2[tempIdx].gameObject, 1800f, 0.4f).setEaseOutQuart();
            } else if (pageLayout_4.activeSelf) {
                for (int i = 0; i < arrayPageAnchor_Layout4[pageIdx].uiRootAnchor.Length; i++) {
                    arrayPageAnchor_Layout4[pageIdx].uiRootAnchor[i].SetLocalPositionX(-1800f);
                    LeanTween.moveLocalX(arrayPageAnchor_Layout4[pageIdx].uiRootAnchor[i].gameObject, 0f, 0.4f).setOnComplete((Action)delegate {
                        IsTween = false;
                    }).setEaseOutQuart();
                }
                for (int j = 0; j < arrayPageAnchor_Layout4[tempIdx].uiRootAnchor.Length; j++) {
                    LeanTween.moveLocalX(arrayPageAnchor_Layout4[tempIdx].uiRootAnchor[j].gameObject, 1800f, 0.4f).setEaseOutQuart();
                }
            }
        } else if (pageLayout_2.activeSelf) {
            arrayPageAnchor_Layout2[pageIdx].SetLocalPositionX(1800f);
            LeanTween.moveLocalX(arrayPageAnchor_Layout2[pageIdx].gameObject, 0f, 0.4f).setOnComplete((Action)delegate {
                IsTween = false;
            }).setEaseOutQuart();
            LeanTween.moveLocalX(arrayPageAnchor_Layout2[tempIdx].gameObject, -1800f, 0.4f).setEaseOutQuart();
        } else if (pageLayout_4.activeSelf) {
            for (int k = 0; k < arrayPageAnchor_Layout4[pageIdx].uiRootAnchor.Length; k++) {
                arrayPageAnchor_Layout4[pageIdx].uiRootAnchor[k].SetLocalPositionX(1800f);
                LeanTween.moveLocalX(arrayPageAnchor_Layout4[pageIdx].uiRootAnchor[k].gameObject, 0f, 0.4f).setOnComplete((Action)delegate {
                    IsTween = false;
                }).setEaseOutQuart();
            }
            for (int l = 0; l < arrayPageAnchor_Layout4[tempIdx].uiRootAnchor.Length; l++) {
                LeanTween.moveLocalX(arrayPageAnchor_Layout4[tempIdx].uiRootAnchor[l].gameObject, -1800f, 0.4f).setEaseOutQuart();
            }
        }
        IsTween = true;
        spPageIndex.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Result, "_page_0" + pageIdx.ToString());
    }
    private void OnDestroy() {
        for (int i = 0; i < arrayPageAnchor_Layout2.Length; i++) {
            LeanTween.cancel(arrayPageAnchor_Layout2[i].gameObject);
        }
        for (int j = 0; j < arrayPageAnchor_Layout4.Length; j++) {
            for (int k = 0; k < arrayPageAnchor_Layout4[j].uiRootAnchor.Length; k++) {
                LeanTween.cancel(arrayPageAnchor_Layout4[j].uiRootAnchor[k].gameObject);
            }
        }
    }
}
