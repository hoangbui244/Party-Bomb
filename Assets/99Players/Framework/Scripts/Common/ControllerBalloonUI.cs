using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class ControllerBalloonUI : MonoBehaviour {
    [SerializeField]
    [Header("吹き出しの親アンカ\u30fc")]
    private GameObject balloonRoot;
    [SerializeField]
    [Header("吹き出しの下敷き画像")]
    private SpriteRenderer balloonUnderlay;
    [SerializeField]
    [Header("吹き出しのボタン画像")]
    private SpriteRenderer[] balloonButton;
    [SerializeField]
    [Header("吹き出しの文字")]
    private TextMeshPro balloonText;
    public void Init() {
        if (balloonRoot != null) {
            balloonRoot.SetActive(value: false);
        }
    }
    public void SetControlInfomationUIActive(bool _isActive) {
        balloonRoot.SetActive(_isActive);
    }
    public void FadeProcess_ControlInfomationUI(bool _fadeIn) {
        if (_fadeIn) {
            balloonRoot.SetActive(value: true);
            balloonUnderlay.SetAlpha(0f);
            balloonText.SetAlpha(0f);
            StartCoroutine(FadeProcess(balloonUnderlay, 1f, 0.5f));
            StartCoroutine(FadeProcess(balloonText, 1f, 0.5f));
            for (int i = 0; i < balloonButton.Length; i++) {
                balloonButton[i].SetAlpha(0f);
                StartCoroutine(FadeProcess(balloonButton[i], 1f, 0.5f));
            }
        } else {
            StartCoroutine(FadeProcess(balloonUnderlay, 0f, 0.5f));
            StartCoroutine(FadeProcess(balloonText, 0f, 0.5f));
            for (int j = 0; j < balloonButton.Length; j++) {
                StartCoroutine(FadeProcess(balloonButton[j], 0f, 0.5f));
            }
            LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate {
                balloonRoot.SetActive(value: false);
            });
        }
    }
    public void FadeProcess_ControlInfomationUI_Stop() {
        StopCoroutine(FadeProcess(balloonUnderlay, 0f, 0f));
        StopCoroutine(FadeProcess(balloonText, 0f, 0f));
        for (int i = 0; i < balloonButton.Length; i++) {
            StopCoroutine(FadeProcess(balloonButton[i], 0f, 0f));
        }
        balloonRoot.SetActive(value: false);
        LeanTween.cancel(base.gameObject);
    }
    private IEnumerator FadeProcess(SpriteRenderer _sprite, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null) {
        float time = 0f;
        float startAlpha = _sprite.color.a;
        yield return new WaitForSeconds(_delayTime);
        while (time < _fadeTime) {
            if (_sprite != null) {
                _sprite.SetAlpha(Mathf.Lerp(startAlpha, _setAlpha, time / _fadeTime));
            }
            time += Time.deltaTime;
            yield return null;
        }
        _sprite.SetAlpha(_setAlpha);
        _callback?.Invoke();
    }
    private IEnumerator FadeProcess(TextMeshPro _text, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null) {
        float time = 0f;
        float startAlpha = _text.color.a;
        yield return new WaitForSeconds(_delayTime);
        while (time < _fadeTime) {
            if (_text != null) {
                _text.SetAlpha(Mathf.Lerp(startAlpha, _setAlpha, time / _fadeTime));
            }
            time += Time.deltaTime;
            yield return null;
        }
        _text.SetAlpha(_setAlpha);
        _callback?.Invoke();
    }
}
