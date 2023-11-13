using GamepadInput;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class Scene_Title : MonoBehaviour {
    private enum SoundType {
        ERASER_MOVE,
        SLIDE,
        LOGO,
        APPEA,
        MOVE
    }
    [SerializeField]
    [Header("アニメ\u30fcタ\u30fc")]
    private Animator animator;
    [SerializeField]
    [Header("演出時オブジェクト")]
    private GameObject[] arrayStartObj;
    [SerializeField]
    [Header("ル\u30fcプ時オブジェクト")]
    private GameObject[] arrayLoopObj;
    [SerializeField]
    [Header("タイトルロゴ")]
    private GameObject objLogo;
    private const string PARAMATER_IS_ANIM_END = "IsAnimEnd";
    private const string PARAMATER_IS_INIT = "IsInit";
    private bool isLogoSE;
    private bool isEndAnimation;
    private bool isInit;
    private InputAction _pressAnyKeyAction = new InputAction(null, InputActionType.PassThrough, "*/<Button>", "Press");
    private void OnDisable() {
        _pressAnyKeyAction.Disable();
    }
    private void LayerEnd(SceneManager.LayerCloseType _closeType) {
    }
    private void OpenLayer() {
        SingletonCustom<SceneManager>.Instance.AddNowLayerCloseCallBack(LayerEnd);
    }
    private bool IsActive() {
        return SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack() == new SceneManager.LayerClose(LayerEnd);
    }
    private void QuitGame() {
        Application.Quit();
    }
    private void CancelQuitDialog() {
    }
    private void Awake() {
        base.transform.SetLocalPositionZ(200f);
    }
    private void OnEnable() {
        OpenLayer();
        SingletonCustom<AudioManager>.Instance.PlayTitleBgm();
        isLogoSE = false;
        isEndAnimation = false;
        isInit = false;
        isEndAnimation = true;
        _pressAnyKeyAction.Enable();
        LeanTween.delayedCall(1.33f, (Action)delegate {
            isInit = true;
            SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_title_main");
            SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_title_sub");
        });
    }
    private void Update() {
        if (SingletonCustom<SceneManager>.Instance.IsFade || SingletonCustom<SceneManager>.Instance.IsLoading || SingletonCustom<DM>.Instance.IsOpen()) {
            return;
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape)) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
            string text = (Localize_Define.Language == Localize_Define.LanguageType.English) ? ("Are you sure you want to" + Environment.NewLine + "quit this game?") : ("ゲ\u30fcムを終了します" + Environment.NewLine + "よろしいですか？");
            SingletonCustom<DM>.Instance.OpenDialog(DM.PARAM_LIST.CHOICE, "", DM.PARAM_LIST.DIALOG_TEXT, text, DM.PARAM_LIST.BUTTON_TEXT, new string[2]
            {
                "はい",
                "いいえ"
            }, DM.PARAM_LIST.CALL_BACK, DM.List(delegate {
                QuitGame();
            }, delegate {
            }));
        } else {
            if (!IsAnyButton() && !IsArrowButton()) {
                return;
            }
            UnityEngine.Debug.Log("button:" + isEndAnimation.ToString());
            if (isEndAnimation) {
                ClickModeSelectBtn();
                return;
            }
            if (!isLogoSE) {
                PlaySound(SoundType.LOGO);
            }
            EndAnimation();
        }
    }
    private void PlaySound(SoundType _soundType) {
        switch (_soundType) {
            case SoundType.ERASER_MOVE:
            case SoundType.SLIDE:
            case SoundType.APPEA:
                break;
            case SoundType.LOGO:
                isLogoSE = true;
                break;
        }
    }
    public bool GetIsEndAnimation() {
        return isEndAnimation;
    }
    public void EndAnimation() {
        isEndAnimation = true;
        animator.SetBool("IsAnimEnd", value: true);
    }
    public void ClickModeSelectBtn() {
        if (!IsActive()) {
            UnityEngine.Debug.Log("IsActive() : " + SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack()?.ToString());
            return;
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_enter");
        SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.MAIN);
    }
    private bool IsAnyButton() {
        return _pressAnyKeyAction.triggered;
    }
    private bool IsArrowButton() {
        for (int i = 0; i < 6; i++) {
            if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(i, SatGamePad.Button.Dpad_Up) || SingletonCustom<JoyConManager>.Instance.GetButtonDown(i, SatGamePad.Button.Dpad_Left) || SingletonCustom<JoyConManager>.Instance.GetButtonDown(i, SatGamePad.Button.Dpad_Right) || SingletonCustom<JoyConManager>.Instance.GetButtonDown(i, SatGamePad.Button.Dpad_Down)) {
                return true;
            }
        }
        return false;
    }
}
