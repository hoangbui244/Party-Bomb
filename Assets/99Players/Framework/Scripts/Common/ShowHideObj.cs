using System;
using UnityEngine;
public class ShowHideObj : MonoBehaviour {
    [SerializeField]
    [Header("一枚絵")]
    private SpriteRenderer onePicture;
    [SerializeField]
    [Header("指定透過度")]
    private float setAlpha = 1f;
    private bool isDisplay = true;
    private bool isShow = true;
    public void SetDisplay(bool _enable) {
        LeanTween.cancel(base.gameObject);
        if (_enable) {
            isDisplay = true;
            isShow = true;
            SetAlpha(1f);
        } else {
            isDisplay = false;
            isShow = false;
            SetAlpha(0f);
        }
    }
    public void SetDisplayFlgOnly(bool _enable) {
        if (_enable) {
            isDisplay = true;
            isShow = false;
        } else {
            isDisplay = false;
            isShow = true;
        }
    }
    public void Show(float _time = 0.15f) {
        if (!isShow && isDisplay) {
            LeanTween.cancel(base.gameObject);
            LeanTween.value(base.gameObject, delegate (float _value) {
                SetAlpha(Mathf.Clamp(_value, 0f, setAlpha));
            }, 0f, 1f, _time).setOnComplete((Action)delegate {
                isShow = true;
            });
        }
    }
    public void Hide(float _time = 0.15f) {
        if (isShow && isDisplay) {
            LeanTween.cancel(base.gameObject);
            LeanTween.value(base.gameObject, delegate (float _value) {
                SetAlpha(Mathf.Clamp(_value, 0f, setAlpha));
            }, 1f, 0f, _time).setOnComplete((Action)delegate {
                isShow = false;
            });
        }
    }
    public void SetAlpha(float _alpha) {
        onePicture.SetAlpha(_alpha);
    }
}
