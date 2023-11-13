using GamepadInput;
using System;
using UnityEngine;
public class Scene_ResultAnnouncement : MonoBehaviourExtension {
    public enum State {
        Opened,
        CrownAward,
        FinalResult,
        ResultDetail
    }
    [Header("デバッグ:1位（1人）確定演出")]
    public bool isWinnerOneEnable;
    [Header("デバッグ:1位（2人）確定演出")]
    public bool isWinnerTwoEnable;
    [Header("デバッグ:1位（3人）確定演出")]
    public bool isWinnerThreeEnable;
    [Header("デバッグ:1位（4人）確定演出")]
    public bool isWinnerFourEnable;
    [SerializeField]
    [Header("OP描画")]
    private SpriteRenderer spOpRender;
    [SerializeField]
    [Header("OP画面")]
    private GameObject objLayoutOP;
    [SerializeField]
    [Header("リザルト画面")]
    private GameObject objLayoutResult;
    [SerializeField]
    [Header("最終結果オブジェクト")]
    private RA_FinalResult finalResult;
    [SerializeField]
    [Header("結果詳細オブジェクト")]
    private RA_ResultDetail resultDetail;
    [SerializeField]
    [Header("1人用クリア表示")]
    private ShowHideObj objSingleClear;
    [SerializeField]
    [Header("1人用クリア表示ボタン")]
    private GameObject objSingleClearButton;
    [SerializeField]
    [Header("スカイボックス")]
    private Material skybox;
    [SerializeField]
    [Header("プレイヤ\u30fcスタイル")]
    private CharacterStyle[] arrayPlayerStyle;
    [SerializeField]
    [Header("カヌ\u30fcマテリアル")]
    private Material[] arrayMatCanoe;
    [SerializeField]
    [Header("切り替えライト")]
    private GameObject[] arrayLight;
    [SerializeField]
    [Header("3D側カメラ")]
    private GameObject objCamera3D;
    private readonly int PAGE_MAX = 3;
    private State state;
    private bool isNextInput;
    public bool IsSingleClearWait {
        get;
        set;
    }
    public bool IsSingleClearButtonWait {
        get;
        set;
    }
    private void Start() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        for (int i = 0; i < arrayPlayerStyle.Length; i++) {
            int num = i;
            if (num >= SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                num += 3;
            }
            UnityEngine.Debug.Log("i:" + i.ToString() + " idx:" + num.ToString());
            arrayPlayerStyle[i].SetGameStyle(GS_Define.GameType.BLOW_AWAY_TANK, num);
        }
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        if (SingletonCustom<SceneManager>.Instance.GetPrevScene() == SceneManager.SceneType.RESULT_ANNOUNCEMENT) {
            SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType = GameSettingManager.GameProgressType.ALL_SPORTS;
            if (isWinnerOneEnable) {
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(0, 470));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(1, 490));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(2, 500));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(3, 480));
            } else if (isWinnerTwoEnable) {
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(0, 750));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(1, 350));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(2, 250));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(3, 750));
            } else if (isWinnerThreeEnable) {
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(0, 880));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(1, 850));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(2, 880));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(3, 880));
            } else if (isWinnerFourEnable) {
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(0, 100));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(1, 100));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(2, 100));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(3, 100));
            } else {
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(0, 320));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(1, 320));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(2, 590));
                SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(3, 670));
            }
            ResultGameDataParams.SetDebugPlayKingRankData();
        }
        objLayoutOP.SetActive(value: false);
        objLayoutResult.SetActive(value: true);
        state = State.FinalResult;
        finalResult.Show();
    }
    private void Update() {
        switch (state) {
            case State.CrownAward:
                break;
            case State.Opened:
                if (!isNextInput || !SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A)) {
                    break;
                }
                SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
                if (SingletonCustom<SceneManager>.Instance.GetPrevScene() == SceneManager.SceneType.RESULT_ANNOUNCEMENT) {
                    if (isWinnerOneEnable) {
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(0, 470));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(1, 490));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(2, 500));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(3, 480));
                    } else if (isWinnerTwoEnable) {
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(0, 750));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(1, 350));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(2, 250));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(3, 750));
                    } else if (isWinnerThreeEnable) {
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(0, 880));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(1, 850));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(2, 880));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(3, 880));
                    } else if (isWinnerFourEnable) {
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(0, 100));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(1, 100));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(2, 100));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(3, 100));
                    } else {
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(0, 320));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(1, 320));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(2, 590));
                        SingletonCustom<GameSettingManager>.Instance.ListResultGameData[SingletonCustom<GameSettingManager>.Instance.GetSportsDayIndex(SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)].listTeamData.Add(new GameSettingManager.TeamData(3, 670));
                    }
                    ResultGameDataParams.SetDebugPlayKingRankData();
                }
                switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
                    case GS_Define.GameFormat.BATTLE:
                        state = State.CrownAward;
                        isNextInput = false;
                        SingletonCustom<AudioManager>.Instance.BgmStop();
                        SingletonCustom<SceneManager>.Instance.FadeExec(delegate {
                            SingletonCustom<RA_CrownAwardManager>.Instance.Init();
                            if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying("bgm_matsuriking_op")) {
                                SingletonCustom<AudioManager>.Instance.BgmStop();
                                SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_matsuriking_op", _loop: true, 0f, 0.5f);
                            }
                            objLayoutOP.SetActive(value: false);
                            arrayLight[0].SetActive(value: false);
                            arrayLight[1].SetActive(value: true);
                            objCamera3D.SetActive(value: true);
                        });
                        break;
                    case GS_Define.GameFormat.COOP:
                    case GS_Define.GameFormat.BATTLE_AND_COOP:
                        isNextInput = false;
                        SingletonCustom<SceneManager>.Instance.FadeExec(delegate {
                            objLayoutOP.SetActive(value: false);
                            arrayLight[0].SetActive(value: false);
                            arrayLight[1].SetActive(value: true);
                            objLayoutResult.SetActive(value: true);
                            state = State.FinalResult;
                            finalResult.Show();
                        });
                        break;
                }
                SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                break;
            case State.FinalResult:
                if (finalResult.IsResultAnimation()) {
                    break;
                }
                if (IsSingleClearButtonWait) {
                    if (!IsSingleClearWait && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B)) {
                        CloseSingleClear();
                        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                    }
                } else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B)) {
                    OnReturnToMainSceneButtonDown();
                }
                break;
            case State.ResultDetail:
                if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B)) {
                    state = State.FinalResult;
                    resultDetail.Hide();
                    SingletonCustom<ResultCharacterManager>.Instance.Show();
                    SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
                }
                break;
        }
    }
    public void OnReturnToMainSceneButtonDown() {
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.MAIN);
    }
    public void ShowSingleClear() {
        objSingleClear.gameObject.SetActive(value: true);
        objSingleClear.SetDisplay(_enable: false);
        objSingleClear.SetDisplayFlgOnly(_enable: true);
        objSingleClear.SetAlpha(0f);
        objSingleClear.Show(1f);
        objSingleClearButton.gameObject.SetActive(value: true);
        objSingleClearButton.transform.SetLocalPositionX(850f);
        LeanTween.delayedCall(3f, (Action)delegate {
            LeanTween.moveLocalX(objSingleClearButton, 519f, 0.25f).setDelay(2.5f).setOnComplete((Action)delegate {
                IsSingleClearWait = false;
            });
        });
    }
    private void CloseSingleClear() {
        objSingleClear.gameObject.SetActive(value: false);
        IsSingleClearButtonWait = false;
    }
    public void CallResult() {
        if (!objLayoutResult.gameObject.activeSelf) {
            objLayoutOP.SetActive(value: false);
            arrayLight[0].SetActive(value: false);
            arrayLight[1].SetActive(value: true);
            objLayoutResult.SetActive(value: true);
            state = State.FinalResult;
            finalResult.Show();
            SingletonCustom<AudioManager>.Instance.SeStop("SE_seien");
        }
    }
    private void OnDestroy() {
        LeanTween.cancel(objSingleClearButton);
    }
}
