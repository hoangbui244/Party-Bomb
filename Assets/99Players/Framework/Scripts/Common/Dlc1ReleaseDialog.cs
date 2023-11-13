using GamepadInput;
using System;
using System.Collections;
using UnityEngine;
public class Dlc1ReleaseDialog : StateBaseClass {
    [SerializeField]
    [Header("フェ\u30fcド画像")]
    private SpriteRenderer spFade;
    [SerializeField]
    [Header("ダイアログ")]
    private GameObject objDialog;
    [SerializeField]
    [Header("クラッカ\u30fc演出")]
    private ParticleSystem psCracker;
    [SerializeField]
    [Header("紙吹雪演出")]
    private ParticleSystem psConfetti;
    private bool isScaling;
    private Action callback;
    public bool IsOpen {
        get;
        set;
    }
    public void Show(Action _callback) {
        callback = _callback;
        base.gameObject.SetActive(value: true);
        LeanTween.cancel(objDialog);
        objDialog.transform.SetLocalScale(0f, 0f, 0f);
        LeanTween.scale(objDialog, Vector3.one, 0.5f).setEaseOutBack().setOnComplete((Action)delegate {
            isScaling = false;
        })
            .setIgnoreTimeScale(useUnScaledTime: true);
        spFade.gameObject.SetActive(value: true);
        LeanTween.cancel(spFade.gameObject);
        LeanTween.value(spFade.gameObject, delegate (float _value) {
            spFade.color = new Color(0f, 0f, 0f, _value);
        }, 0f, 0.7f, 0.25f).setIgnoreTimeScale(useUnScaledTime: true);
        isScaling = true;
        OpenLayer();
        StartCoroutine(_PlayCrackerSE(0f));
        StartCoroutine(_PlayCrackerSE(0.2f));
        psCracker.Play();
        psConfetti.Play();
        IsOpen = true;
    }
    private IEnumerator _PlayCrackerSE(float _waitTime) {
        yield return new WaitForSeconds(_waitTime);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cracker");
    }
    protected new void Update() {
        base.Update();
        if (!isScaling && IsOpen && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A)) {
            Close();
        }
    }
    public void Close() {
        SingletonCustom<AudioManager>.Instance.SePlay("se_button_cancel");
        psCracker.Stop();
        psConfetti.Stop();
        LeanTween.scale(objDialog, Vector3.zero, 0.5f).setEaseInBack().setOnComplete((Action)delegate {
            objDialog.SetActive(value: false);
            isScaling = false;
            SingletonCustom<SceneManager>.Instance.CloseComplete();
            IsOpen = false;
            if (callback != null) {
                callback();
            }
        })
            .setIgnoreTimeScale(useUnScaledTime: true);
        LeanTween.cancel(spFade.gameObject);
        LeanTween.value(base.gameObject, delegate (float _value) {
            spFade.color = new Color(0f, 0f, 0f, _value);
        }, spFade.color.a, 0f, 0.25f).setIgnoreTimeScale(useUnScaledTime: true).setOnComplete((Action)delegate {
            spFade.gameObject.SetActive(value: false);
        });
        isScaling = true;
        LeanTween.delayedCall(base.gameObject, 3f, (Action)delegate {
            base.gameObject.SetActive(value: false);
        });
    }
    public void DirectClose() {
        LeanTween.cancel(objDialog);
        LeanTween.cancel(base.gameObject);
        LeanTween.cancel(spFade.gameObject);
        objDialog.SetActive(value: false);
        SingletonCustom<SceneManager>.Instance.CloseComplete();
        IsOpen = false;
        SingletonCustom<GS_GameSelectManager>.Instance.OnDetailBack();
        spFade.color = new Color(0f, 0f, 0f, 0f);
        spFade.gameObject.SetActive(value: false);
        isScaling = true;
        base.gameObject.SetActive(value: false);
    }
}
