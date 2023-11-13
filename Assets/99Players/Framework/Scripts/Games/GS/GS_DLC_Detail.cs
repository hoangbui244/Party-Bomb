using GamepadInput;
using System;
using System.Collections;
using UnityEngine;
public class GS_DLC_Detail : MonoBehaviour {
    [SerializeField]
    [Header("フェ\u30fcド")]
    private SpriteRenderer fade;
    [SerializeField]
    [Header("詳細画面")]
    private GameObject dialogRoot;
    private bool isGameSelectCall;
    private Action closeStartCallBack;
    private bool isScale;
    public bool IsOpen { get; set; }
    public void Open(bool _isGameSelectCall, Action _closeStartCallBack = null) {
        closeStartCallBack = _closeStartCallBack;
        base.gameObject.SetActive(value: true);
        fade.SetAlpha(0f);
        fade.gameObject.SetActive(value: true);
        LeanTween.cancel(fade.gameObject);
        LeanTween.value(fade.gameObject, 0f, 0.8f, 0.25f).setOnUpdate(delegate(float _value) { fade.SetAlpha(_value); }).setEaseOutExpo();
        LeanTween.cancel(dialogRoot);
        dialogRoot.transform.SetLocalScale(0f, 0f, 1f);
        LeanTween.scale(dialogRoot, Vector3.one, 0.375f).setEaseOutQuart().setOnComplete((Action)delegate { isScale = false; });
        IsOpen = true;
        isScale = true;
        isGameSelectCall = _isGameSelectCall;
    }
    private void Update() {
        if (!IsButtonInteractable()) {
            return;
        }
        if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.B)) {
            OnReturnButtonDown();
        }
        else if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.Y)) {
            OnOpenShop();
        }
    }
    private bool IsButtonInteractable() {
        if (SingletonCustom<SceneManager>.Instance.IsFade) {
            return false;
        }
        if (SingletonCustom<CommonNotificationManager>.Instance.IsOpen) {
            return false;
        }
        if (!IsOpen) {
            return false;
        }
        if (isScale) {
            return false;
        }
        return true;
    }
    public void OnReturnButtonDown() {
        if (!IsButtonInteractable()) {
            return;
        }
        base.gameObject.SetActive(value: false);
        LeanTween.cancel(fade.gameObject);
        LeanTween.value(base.gameObject, _value => { fade.color = new Color(0f, 0f, 0f, _value); }, fade.color.a, 0f, 0.25f)
            .setIgnoreTimeScale(useUnScaledTime: true)
            .setOnComplete(() => {
                fade.gameObject.SetActive(value: false);
                IsOpen = false;
                if (isGameSelectCall) {
                    SingletonCustom<GS_GameSelectManager>.Instance.OnDetailBack();
                }
            });
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
        if (closeStartCallBack != null) {
            closeStartCallBack();
        }
    }
    public void OnOpenShop() {
        if (!IsButtonInteractable()) {
            return;
        }
        StartCoroutine(_eShopWait());
    }
    private IEnumerator _eShopWait() {
        SingletonCustom<AudioManager>.Instance.BgmVolumeChange(0f);
        yield return new WaitForSeconds(0.1f);
        eShopManager.ShowShopAddOnContentDetailPage();
        SingletonCustom<AudioManager>.Instance.BgmVolumeChange();
        OnReturnButtonDown();
    }
    public void DirectClose() {
        LeanTween.cancel(fade.gameObject);
        LeanTween.cancel(dialogRoot);
        LeanTween.cancel(base.gameObject);
        fade.color = new Color(0f, 0f, 0f, 0f);
        fade.gameObject.SetActive(value: false);
        IsOpen = false;
        base.gameObject.SetActive(value: false);
    }
    private void OnDestroy() {
        LeanTween.cancel(dialogRoot);
        LeanTween.cancel(fade.gameObject);
    }
}