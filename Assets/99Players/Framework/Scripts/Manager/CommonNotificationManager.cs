using GamepadInput;
using io.ninenine.players.party3d.games.common;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
public class CommonNotificationManager:SingletonCustom<CommonNotificationManager> {
    [SerializeField]
    [Header("フェ\u30fcド")]
    private SpriteRenderer fade;
    [SerializeField]
    [Header("操作説明オブジェクト")]
    private GameObject rootOperationInfo;
    [SerializeField]
    [Header("操作説明拡縮ル\u30fcト")]
    private GameObject rooScaleOperation;
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fc表示【ゲ\u30fcムセレクト】")]
    private GameObject controllerGameSelect;
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fc表示【ポ\u30fcズ】")]
    private GameObject controllerGamePause;
    public Vector3 GamePauseTweenStartLocalPos;
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fc表示【ゲ\u30fcム開始時の自動表示】")]
    private GameObject controllerGamePauseBack;
    public Vector3 GamePauseBackTweenStartLocalPos;
    [SerializeField]
    [Header("コントロ\u30fcラ\u30fc表示【設定】")]
    private GameObject controllerSetting;
    private Vector3 m_GamePauseDefaultLocalPos;
    private Vector3 m_GamePauseBackDefaultLocalPos;
    [SerializeField]
    [Header("操作説明描画")]
    private SpriteRenderer spOperationInfoRender;
    [SerializeField]
    [Header("操作説明画像【ひとりで】リソ\u30fcス名")]
    private string[] arraySingleOperationSpName;
    [SerializeField]
    [Header("操作説明画像【ひとりで】(英語リソ\u30fcス名)")]
    private string[] arraySingleOperationSp_ENName;
    [SerializeField]
    [Header("操作説明画像【みんなで】リソ\u30fcス名")]
    private string[] arrayMultiOperationSpName;
    [SerializeField]
    [Header("操作説明画像【みんなで】(英語リソ\u30fcス名)")]
    private string[] arrayMultiOperationSp_ENName;
    [SerializeField]
    [Header("ポ\u30fcズ時の遷移先スプライト")]
    private SpriteRenderer spPauseBackSceneName;
    [SerializeField]
    [Header("ポ\u30fcズ時の遷移先ボタン")]
    private SpriteRenderer spPauseBackSceneButton;
    [SerializeField]
    [Header("ポ\u30fcズ時の遷移先下敷き")]
    private SpriteRenderer spPauseBackSceneUnderlay;
    [SerializeField]
    [Header("ポ\u30fcズ時：モ\u30fcドセレクト表示")]
    private GameObject objPauseModeSelect;
    [SerializeField]
    [Header("ポ\u30fcズ時：クエストセレクト表示")]
    private GameObject objPauseQuestSelect;
    [SerializeField]
    [Header("背景フレ\u30fcム色")]
    private Color[] arrayGameTitleBackFrameColor;
    [SerializeField]
    [Header("ポ\u30fcズ文章フレ\u30fcム")]
    private SpriteRenderer renderPauseFrame;
    private float originRenderPauseFrameSizeX;
    [SerializeField]
    [Header("ポ\u30fcズ文章")]
    private TextMeshPro textPauseInfo;
    private float originTextPauseInfoFontSize;
    private readonly int TEXT_PAUSE_CHANGE_TEXT_LENGTH = 65;
    private readonly float TEXT_PAUSE_CHANGE_FONT_SIZE = 330f;
    [SerializeField]
    [Header("アイコン表示位置")]
    private Vector3[] arrayGameStartIconPos;
    [SerializeField]
    [Header("コ\u30fcス選択時のコ\u30fcルバック")]
    public UnityEvent OnCourseSelectCursorMove = new UnityEvent();
    [SerializeField]
    [Header("操作説明の設定表示")]
    private GameObject objSettingButton;
    private readonly float POS_DEFAULT_CONTROLLER;
    private readonly float POS_DEFAULT_OPERATION = -1429f;
    private readonly float POS_OUT_CONTROLLER = 900f;
    private readonly float POS_OUT_OPERATION = -1929f;
    private readonly float POS_DEFAULT_TALK_CHARACTER = 659f;
    private readonly float POS_OUT_TALK_CHARACTER = 1230f;
    private readonly int TITLE_CAPTION_BASE_SIZE_Y_EN = 176;
    private readonly float PAUSE_NAME_MODE_POS_X = -26f;
    private readonly float PAUSE_NAME_COMPE_POS_X = -13f;
    private readonly float PAUSE_BUTTON_MODE_POS_X = -183f;
    private readonly float PAUSE_BUTTON_COMPE_POS_X = -157f;
    private readonly float PAUSE_UNDERLAY_MODE_POS_X = 342f;
    private readonly float PAUSE_UNDERLAY_COMPE_POS_X = 368f;
    private int[] arrayTrophyDispStack = new int[15];
    private TalkData[] currentTalkData;
    private int currentTalkIdx;
    private bool isTitleCaptionInput;
    private Action titleCaptionCallback;
    private bool isInitTitleCaptionPosY_EN;
    private float initTitleCaptionPosY_EN;
    private int frameLockCnt;
    private string[] tempSplitStr = new string[1]
    {
        ">で"
    };
    private string[][] tempSplitStrTotal = new string[2][]
    {
        new string[1]
        {
            "<color=#FFF000>"
        },
        new string[1]
        {
            "<color=white>で達成"
        }
    };
    private string[] tempDeleteStr_EN = new string[3]
    {
        " in <color=#FFF000>",
        " of <color=#FFF000>",
        "<color=white>"
    };
    private string[] tempArrayStr;
    private Action callBack;
    private bool isClosing;
    [SerializeField]
    private JoyConManager.ControlMode activeMode;
    [SerializeField]
    private GameObject targetObject;
    public bool IsOpen {
        get {
            if(!controllerGameSelect.activeSelf && !controllerGamePause.activeSelf) {
                return controllerGamePauseBack.activeSelf;
            }
            return true;
        }
    }
    public bool IsPause => controllerGamePause.activeSelf;
    private void Start() {
        m_GamePauseDefaultLocalPos = controllerGamePause.transform.localPosition;
        m_GamePauseBackDefaultLocalPos = controllerGamePauseBack.transform.localPosition;
    }
    private new void OnEnable() {
        SingletonCustom<JoyConManager>.Instance.OnMainPlayerControlModeChanged += OnChanged;
        OnChanged((!SingletonCustom<JoyConManager>.Instance.IsMainPlayerControlMode(activeMode)) ? JoyConManager.ControlMode.Keyboard : activeMode);
    }
    private void OnDisable() {
        SingletonCustom<JoyConManager>.Instance.OnMainPlayerControlModeChanged -= OnChanged;
    }
    private void OnChanged(JoyConManager.ControlMode mode) {
        SetSprite();
    }
    public void OpenOperationInfoAtGameSelect() {
        objSettingButton.SetActive(value: false);
        frameLockCnt = 1;
        SetOperationSprite();
        controllerGamePauseBack.SetActive(value: true);
        LeanTween.cancel(controllerGamePauseBack);
        controllerGamePauseBack.transform.SetLocalPosition(GamePauseBackTweenStartLocalPos.x, GamePauseBackTweenStartLocalPos.y, GamePauseBackTweenStartLocalPos.z);
        LeanTween.moveLocal(controllerGamePauseBack,m_GamePauseBackDefaultLocalPos,0.5f).setEaseOutQuint().setIgnoreTimeScale(useUnScaledTime: true);
    }
    public void OpenOperationInfoAtGamePause(bool isAuto = false) {
        if(!isClosing) {
            objSettingButton.SetActive(value: true);
            frameLockCnt = 1;
            Time.timeScale = 0f;
            StateBaseClass.SetPauseFlg(StateBaseClass.PAUSE_LEVEL.GAME);
            SetOperationSprite();
            if(isAuto) {
                controllerGamePauseBack.SetActive(value: true);
                LeanTween.cancel(controllerGamePauseBack);
                controllerGamePauseBack.transform.SetLocalPosition(GamePauseBackTweenStartLocalPos.x, GamePauseBackTweenStartLocalPos.y, GamePauseBackTweenStartLocalPos.z);
                LeanTween.moveLocal(controllerGamePauseBack, m_GamePauseBackDefaultLocalPos, 0.5f).setEaseOutQuint().setIgnoreTimeScale(useUnScaledTime: true);
            } else {
                controllerGamePause.SetActive(value: true);
                SingletonCustom<AudioManager>.Instance.SePlay("se_pause_open");
                LeanTween.cancel(controllerGamePause);
                controllerGamePause.transform.SetLocalPosition(GamePauseTweenStartLocalPos.x, GamePauseTweenStartLocalPos.y, GamePauseTweenStartLocalPos.z);
                LeanTween.moveLocal(controllerGamePause, m_GamePauseDefaultLocalPos, 0.5f).setEaseOutQuint().setIgnoreTimeScale(useUnScaledTime: true);
            }
            controllerSetting.SetActive(value: true);
            LeanTween.cancel(controllerSetting);
            controllerSetting.transform.SetLocalPositionX(-1800f);
            LeanTween.moveLocalX(controllerSetting,-1430f,0.5f).setEaseOutQuint().setIgnoreTimeScale(useUnScaledTime: true);
            SingletonCustom<AudioManager>.Instance.SeLoopPause();
            if(SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.ALL_SPORTS) {
                objPauseModeSelect.SetActive(value: true);
                objPauseQuestSelect.SetActive(value: false);
            } else {
                objPauseModeSelect.SetActive(value: false);
                objPauseQuestSelect.SetActive(value: true);
            }
            TouchPanelManager.Instance.SetTouchPanelEnable(false);
        }
    }
    public void SetFade(bool _isEnable) {
        if(_isEnable) {
            LeanTween.cancel(fade.gameObject);
        }
    }
    public void OpenGameTitle(Action _callBack) {
        _callBack();
    }
    public void OpenGetTrophy(Action _callBack,bool isGetVoice = true) {
    }
    public void AddTrophyDisp(int _dispIdx) {
        int num = 0;
        while(true) {
            if(num < arrayTrophyDispStack.Length) {
                if(arrayTrophyDispStack[num] == -1) {
                    break;
                }
                num++;
                continue;
            }
            return;
        }
        arrayTrophyDispStack[num] = _dispIdx;
    }
    public void OpenTalk(TalkData[] _talkData,Action _callBack) {
    }
    public void CloseControllerInfo() {
        controllerGameSelect.SetActive(value: false);
        rootOperationInfo.SetActive(value: false);
    }
    public void CloseProgram() {
    }
    public void Close(bool _isImmidate = false) {
        fade.gameObject.SetActive(value: false);
        rootOperationInfo.SetActive(value: false);
        controllerGameSelect.SetActive(value: false);
        controllerGamePause.SetActive(value: false);
        controllerGamePauseBack.SetActive(value: false);
        controllerSetting.SetActive(value: false);
        InitTrophyDispStack();
        isTitleCaptionInput = false;
        if(_isImmidate) {
            StateBaseClass.SetPauseFlg(StateBaseClass.PAUSE_LEVEL.NONE);
            isClosing = false;
        } else {
            LeanTween.delayedCall(base.gameObject,0.1f,(Action)delegate {
                Time.timeScale = 1f;
                StateBaseClass.SetPauseFlg(StateBaseClass.PAUSE_LEVEL.NONE);
                isClosing = false;
            }).setIgnoreTimeScale(useUnScaledTime: true);
        }
    }
    private int GetTrophyDispStackCnt() {
        CalcManager.mCalcInt = 0;
        for(int i = 0;i < arrayTrophyDispStack.Length;i++) {
            if(arrayTrophyDispStack[i] != -1) {
                CalcManager.mCalcInt++;
            }
        }
        return CalcManager.mCalcInt;
    }
    private int GetTrophyDispIdx() {
        CalcManager.mCalcInt = -1;
        for(int i = 0;i < arrayTrophyDispStack.Length;i++) {
            if(arrayTrophyDispStack[i] != -1) {
                CalcManager.mCalcInt = arrayTrophyDispStack[i];
                arrayTrophyDispStack[i] = -1;
                return CalcManager.mCalcInt;
            }
        }
        return CalcManager.mCalcInt;
    }
    private void SetOperationSprite() {
        rootOperationInfo.SetActive(value: true);
        LeanTween.cancel(rooScaleOperation);
        rooScaleOperation.transform.SetLocalScale(0.001f,0.001f,1f);
        LeanTween.scale(rooScaleOperation,Vector3.one * 1.001f,0.35f).setEaseOutExpo().setIgnoreTimeScale(useUnScaledTime: true);
        SetSprite();
        string text = "";
        text = ((SingletonCustom<GameSettingManager>.Instance.LastSelectGameType < GS_Define.GameType.DROP_BLOCK) ? SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,(int)(281 + SingletonCustom<GameSettingManager>.Instance.LastSelectGameType)) : SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,(int)(454 + (SingletonCustom<GameSettingManager>.Instance.LastSelectGameType - 40))));
        UnityEngine.Debug.Log("説明テキスト：" + SingletonCustom<GameSettingManager>.Instance.LastSelectGameType.ToString());
        if(Localize_Define.Language == Localize_Define.LanguageType.Japanese) {
            textPauseInfo.fontSize = originTextPauseInfoFontSize;
        } else {
            bool flag = false;
            string[] array = text.Split("\n"[0]);
            for(int i = 0;i < array.Length;i++) {
                if(array[i].Length >= TEXT_PAUSE_CHANGE_TEXT_LENGTH) {
                    flag = true;
                    break;
                }
            }
            textPauseInfo.fontSize = ((!flag) ? originTextPauseInfoFontSize : TEXT_PAUSE_CHANGE_FONT_SIZE);
        }
        textPauseInfo.SetText(text);
        textPauseInfo.rectTransform.sizeDelta = new Vector2(textPauseInfo.preferredWidth,textPauseInfo.rectTransform.sizeDelta.y);
        textPauseInfo.transform.SetLocalPositionX(0f);
        renderPauseFrame.size = new Vector2(Mathf.Clamp(textPauseInfo.preferredWidth + 300f,originRenderPauseFrameSizeX,textPauseInfo.preferredWidth + 300f),renderPauseFrame.size.y);
    }
    private void SetSprite() {
        string text = "";
        if(spOperationInfoRender.sprite != null) {
            spOperationInfoRender.sprite = null;
        }
        if(SingletonCustom<JoyConManager>.Instance.controlMode[0] == JoyConManager.ControlMode.Gamepad) {
            if(Localize_Define.Language == Localize_Define.LanguageType.Japanese) {
                text = "Sprite/ControleOperation/GamePad/" + arraySingleOperationSpName[(int)SingletonCustom<GameSettingManager>.Instance.LastSelectGameType];
                spOperationInfoRender.sprite = Resources.Load<Sprite>(text);
            } else {
                text = "Sprite/ControleOperation/GamePad_en/" + arraySingleOperationSp_ENName[(int)SingletonCustom<GameSettingManager>.Instance.LastSelectGameType];
                spOperationInfoRender.sprite = Resources.Load<Sprite>(text);
            }
            return;
        }
        if(Localize_Define.Language == Localize_Define.LanguageType.Japanese) {
            text = "Sprite/ControleOperation/Keyboard/" + arrayMultiOperationSpName[(int)SingletonCustom<GameSettingManager>.Instance.LastSelectGameType];
            spOperationInfoRender.sprite = Resources.Load<Sprite>(text);
        } else {
            text = "Sprite/ControleOperation/Keyboard_en/" + arrayMultiOperationSp_ENName[(int)SingletonCustom<GameSettingManager>.Instance.LastSelectGameType];
            spOperationInfoRender.sprite = Resources.Load<Sprite>(text);
        }
        bool flag = spOperationInfoRender.sprite == null;
    }
    public void SetSportsDayProgram(bool _isFade = false) {
    }
    public void AddLockCnt() {
        frameLockCnt++;
    }
    private void Update() {
        if(SingletonCustom<DM>.Instance.IsActive() || SingletonCustom<SceneManager>.Instance.IsFade || SingletonCustom<DM>.Instance.IsActive()) {
            return;
        }
        if(frameLockCnt > 0) {
            frameLockCnt--;
        } else {
            if(SingletonCustom<GS_Setting>.Instance.IsActive || isClosing) {
                return;
            }
            if(controllerGameSelect.activeSelf) {
                if(SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B,_isRepeat: false,KeyCode.None,useOnlyArrow: true,_isTimeMoving: true)) {
                    Close();
                    frameLockCnt = 1;
                    SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
                }
            } else if(controllerGamePause.activeSelf) {
                if(SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X,_isRepeat: false,KeyCode.None,useOnlyArrow: true,_isTimeMoving: true)) {
                    SelectPartyGame();
                } else if(SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Y,_isRepeat: false,KeyCode.None,useOnlyArrow: true,_isTimeMoving: true)) {
                    Retry();
                } else if(SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B,_isRepeat: false,KeyCode.None,useOnlyArrow: true,_isTimeMoving: true)) {
                    SingletonCustom<AudioManager>.Instance.SeLoopPauseRelease();
                    Close();
                    SingletonCustom<AudioManager>.Instance.SePlay("se_pause_close");
                } else if((SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Start,_isRepeat: false,KeyCode.None,useOnlyArrow: true,_isTimeMoving: true) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Back,_isRepeat: false,KeyCode.None,useOnlyArrow: true,_isTimeMoving: true)) && !SingletonCustom<GS_Setting>.Instance.IsActive) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                    SingletonCustom<GS_Setting>.Instance.Open();
                }
            } else if(controllerGamePauseBack.activeSelf) {
                if(SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B,_isRepeat: false,KeyCode.None,useOnlyArrow: true,_isTimeMoving: true)) {
                    SingletonCustom<AudioManager>.Instance.SeLoopPauseRelease();
                    Close();
                    SingletonCustom<AudioManager>.Instance.SePlay("se_pause_close");
                    if(titleCaptionCallback != null) {
                        titleCaptionCallback();
                    }
                } else if((SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Start,_isRepeat: false,KeyCode.None,useOnlyArrow: true,_isTimeMoving: true) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Back,_isRepeat: false,KeyCode.None,useOnlyArrow: true,_isTimeMoving: true)) && objSettingButton.activeSelf && !SingletonCustom<GS_Setting>.Instance.IsActive) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                    SingletonCustom<GS_Setting>.Instance.Open();
                }
            } else if(isTitleCaptionInput) {
                if(SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Start,_isRepeat: false,KeyCode.None,useOnlyArrow: true) || SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Back,_isRepeat: false,KeyCode.None,useOnlyArrow: true)) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_button_enter");
                    OpenOperationInfoAtGameSelect();
                } else if(SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A,_isRepeat: false,KeyCode.None,useOnlyArrow: true)) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                    isClosing = true;
                    LeanTween.delayedCall(0.6f,(Action)delegate {
                        isTitleCaptionInput = false;
                        if(!SingletonCustom<SaveDataManager>.Instance.SaveData.isStartHelp[(int)SingletonCustom<GameSettingManager>.Instance.LastSelectGameType]) {
                            SingletonCustom<SaveDataManager>.Instance.SaveData.isStartHelp[(int)SingletonCustom<GameSettingManager>.Instance.LastSelectGameType] = true;
                            Close(_isImmidate: true);
                            if(titleCaptionCallback != null) {
                                OpenOperationInfoAtGamePause(isAuto: true);
                            }
                        } else if(SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.isStartHelp) {
                            Close(_isImmidate: true);
                            if(titleCaptionCallback != null) {
                                OpenOperationInfoAtGamePause(isAuto: true);
                            }
                        } else {
                            Close();
                            if(titleCaptionCallback != null) {
                                titleCaptionCallback();
                            }
                        }
                    }).setIgnoreTimeScale(useUnScaledTime: true);
                } else if(SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B,_isRepeat: false,KeyCode.None,useOnlyArrow: true,_isTimeMoving: true) && SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
                    SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT,SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,58),DM.PARAM_LIST.SHOW_BACK_FADE,true,DM.PARAM_LIST.DIALOG_SIZE_H,380f,DM.PARAM_LIST.DIALOG_SIZE_W,630f,DM.PARAM_LIST.CHOICE,"",DM.PARAM_LIST.CALL_BACK,DM.List(delegate {
                        SingletonCustom<AudioManager>.Instance.CallStopAllCoroutines();
                        SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.MAIN);
                    },delegate {
                        frameLockCnt = 1;
                    }),DM.PARAM_LIST.BUTTON_TEXT,DM.List(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,56),SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,57)));
                }
            }
        }
    }
    private void InitTrophyDispStack() {
        for(int i = 0;i < arrayTrophyDispStack.Length;i++) {
            arrayTrophyDispStack[i] = -1;
        }
    }
    private void Awake() {
        originRenderPauseFrameSizeX = renderPauseFrame.size.x;
        originTextPauseInfoFontSize = textPauseInfo.fontSize;
        InitTrophyDispStack();
    }
    private new void OnDestroy() {
        base.OnDestroy();
        LeanTween.cancel(fade.gameObject);
        LeanTween.cancel(controllerGameSelect);
        LeanTween.cancel(controllerGamePause);
        LeanTween.cancel(controllerSetting);
        LeanTween.cancel(rooScaleOperation);
    }
    public void Retry() {
        if(SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
            SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT,SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,59),DM.PARAM_LIST.SHOW_BACK_FADE,true,DM.PARAM_LIST.DIALOG_SIZE_H,380f,DM.PARAM_LIST.DIALOG_SIZE_W,630f,DM.PARAM_LIST.CHOICE,"",DM.PARAM_LIST.CALL_BACK,DM.List(delegate {
                SingletonCustom<SceneManager>.Instance.NextScene(SingletonCustom<SceneManager>.Instance.GetNowScene());
            },delegate {
                frameLockCnt = 1;
            }),DM.PARAM_LIST.BUTTON_TEXT,DM.List(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,56),SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,57)));
        }
    }
    public void SelectPartyGame() {
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
        if(SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.ALL_SPORTS) {
            SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT,SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,39),DM.PARAM_LIST.SHOW_BACK_FADE,true,DM.PARAM_LIST.DIALOG_SIZE_H,380f,DM.PARAM_LIST.DIALOG_SIZE_W,630f,DM.PARAM_LIST.CHOICE,"",DM.PARAM_LIST.CALL_BACK,DM.List(delegate {
                SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.MAIN);
            },delegate {
                frameLockCnt = 1;
            }),DM.PARAM_LIST.BUTTON_TEXT,DM.List(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,56),SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,57)));
        } else {
            SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.DIALOG_TEXT,SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,54),DM.PARAM_LIST.SHOW_BACK_FADE,true,DM.PARAM_LIST.DIALOG_SIZE_H,380f,DM.PARAM_LIST.DIALOG_SIZE_W,630f,DM.PARAM_LIST.CHOICE,"",DM.PARAM_LIST.CALL_BACK,DM.List(delegate {
                SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.MAIN);
            },delegate {
                frameLockCnt = 1;
            }),DM.PARAM_LIST.BUTTON_TEXT,DM.List(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,56),SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON,57)));
        }
    }
    public void CloseUIButton() {
        fade.gameObject.SetActive(value: false);
        rootOperationInfo.SetActive(value: false);
        controllerGameSelect.SetActive(value: false);
        controllerGamePause.SetActive(value: false);
        controllerGamePauseBack.SetActive(value: false);
        controllerSetting.SetActive(value: false);
        InitTrophyDispStack();
        isTitleCaptionInput = false;
        LeanTween.delayedCall(base.gameObject,0.1f,(Action)delegate {
            Time.timeScale = 1f;
            StateBaseClass.SetPauseFlg(StateBaseClass.PAUSE_LEVEL.NONE);
            isClosing = false;
        }).setIgnoreTimeScale(useUnScaledTime: true);
        TouchPanelManager.Instance.SetTouchPanelEnable(true);
    }
}
