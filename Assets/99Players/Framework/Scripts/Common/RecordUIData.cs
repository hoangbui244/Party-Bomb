using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class RecordUIData : MonoBehaviour {
    [Serializable]
    private struct DefaultUIScaleData {
        public Vector2 numbers;
        public Vector2 textNumbers;
        public Vector2 numbers_Decimal;
        public Vector2 millSecond;
        public Vector2 second;
        public Vector2 minutes;
        public Vector2 addText_Count;
        public Vector2 addText_Piece;
        public Vector2 addText_Animal;
        public Vector2 addText_cm;
        public Vector2 addText_Win;
        public Vector2 addText_m;
        public Vector2 addText_g;
        public Vector2 addText_Second;
        public Vector2 addText_pt;
        public Vector2 addText_mL;
        public Vector2 numbers_dot;
        public Vector2 numbers_colon;
        public Vector2 text_Time;
    }
    [Serializable]
    public struct ScoreUIData {
        [Header("記録のアンカ\u30fc")]
        public Transform recordAnchor;
        [Header("数値")]
        public SpriteNumbers numbers;
        [Header("「回」の文字")]
        public SpriteRenderer addText_Count;
        [Header("「個」の文字")]
        public SpriteRenderer addText_Piece;
        [Header("「匹」の文字")]
        public SpriteRenderer addText_Animal;
        [Header("「勝」の文字")]
        public SpriteRenderer addText_Win;
        [Header("「pt」の文字")]
        public SpriteRenderer addText_pt;
    }
    [Serializable]
    public struct DoubleDecimalScoreUIData {
        [Header("記録のアンカ\u30fc")]
        public Transform recordAnchor;
        [Header("数値")]
        public SpriteNumbers numbers;
        [Header("小数点以下の数値")]
        public SpriteNumbers numbers_Decimal;
        [Header("ドット[.]")]
        public SpriteRenderer numbers_dot;
        [Header("「m」の文字")]
        public SpriteRenderer addText_m;
    }
    [Serializable]
    public struct DecimalScoreUIData {
        [Header("記録のアンカ\u30fc")]
        public Transform recordAnchor;
        [Header("数値")]
        public SpriteNumbers numbers;
        [Header("小数点以下の数値")]
        public SpriteNumbers numbers_Decimal;
        [Header("ドット[.]")]
        public SpriteRenderer numbers_dot;
        [Header("「g」の文字")]
        public SpriteRenderer addText_g;
        [Header("「cm」の文字")]
        public SpriteRenderer addText_cm;
    }
    [Serializable]
    public struct TimeUIData {
        [Header("記録のアンカ\u30fc")]
        public Transform recordAnchor;
        [Header("ミリ秒")]
        public SpriteNumbers millSecond;
        [Header("秒")]
        public SpriteNumbers second;
        [Header("分")]
        public SpriteNumbers minutes;
        [Header("コロン[:]")]
        public SpriteRenderer numbers_colon;
        [Header("ドット[.]")]
        public SpriteRenderer numbers_dot;
        [Header("タイムテキスト")]
        public TextMeshPro textTime;
    }
    [Serializable]
    public struct SecondTimeUIData {
        [Header("記録のアンカ\u30fc")]
        public Transform recordAnchor;
        [Header("秒")]
        public SpriteNumbers second;
        [Header("ミリ秒")]
        public SpriteNumbers millSecond;
        [Header("「秒」の文字")]
        public SpriteRenderer addText_Second;
    }
    [Serializable]
    public struct ScoreFourDigitUIData {
        [Header("記録のアンカ\u30fc")]
        public Transform recordAnchor;
        [SerializeField]
        [Header("数値")]
        public SpriteNumbers numbers;
        [SerializeField]
        [Header("数値テキスト")]
        public TextMeshPro textNumbers;
        [SerializeField]
        [Header("「pt」の文字")]
        public SpriteRenderer addText_pt;
        [SerializeField]
        [Header("「mL」の文字")]
        public SpriteRenderer addText_mL;
    }
    public enum RecordType {
        Score,
        DoubleDecimalScore,
        DecimalScore,
        Time,
        SecondTime,
        ScoreFourDigit
    }
    public RecordType recordType;
    public ScoreUIData scoreUIData;
    public DoubleDecimalScoreUIData doubleDecimalScoreUIData;
    public DecimalScoreUIData decimalScoreUIData;
    public TimeUIData timeUIData;
    public SecondTimeUIData secondTimeUIData;
    public ScoreFourDigitUIData scoreFourDigitUIData;
    private DefaultUIScaleData defaultUIScaleData;
    private bool saveUIDefScale;
    public void Animation_Scaling(float _animationTime) {
        switch (recordType) {
            case RecordType.Score:
                LeanTween.scale(scoreUIData.numbers.gameObject, defaultUIScaleData.numbers, _animationTime);
                if (scoreUIData.addText_Count.gameObject.activeSelf) {
                    LeanTween.scale(scoreUIData.addText_Count.gameObject, defaultUIScaleData.addText_Count, _animationTime);
                }
                if (scoreUIData.addText_Piece.gameObject.activeSelf) {
                    LeanTween.scale(scoreUIData.addText_Piece.gameObject, defaultUIScaleData.addText_Piece, _animationTime);
                }
                if (scoreUIData.addText_Animal.gameObject.activeSelf) {
                    LeanTween.scale(scoreUIData.addText_Animal.gameObject, defaultUIScaleData.addText_Animal, _animationTime);
                }
                if (scoreUIData.addText_Win.gameObject.activeSelf) {
                    LeanTween.scale(scoreUIData.addText_Win.gameObject, defaultUIScaleData.addText_Win, _animationTime);
                }
                if (scoreUIData.addText_pt.gameObject.activeSelf) {
                    LeanTween.scale(scoreUIData.addText_pt.gameObject, defaultUIScaleData.addText_pt, _animationTime);
                }
                break;
            case RecordType.DoubleDecimalScore:
                LeanTween.scale(doubleDecimalScoreUIData.numbers.gameObject, defaultUIScaleData.numbers, _animationTime);
                LeanTween.scale(doubleDecimalScoreUIData.numbers_Decimal.gameObject, defaultUIScaleData.numbers_Decimal, _animationTime);
                LeanTween.scale(doubleDecimalScoreUIData.numbers_dot.gameObject, defaultUIScaleData.numbers_dot, _animationTime);
                if (doubleDecimalScoreUIData.addText_m.gameObject.activeSelf) {
                    LeanTween.scale(doubleDecimalScoreUIData.addText_m.gameObject, defaultUIScaleData.addText_m, _animationTime);
                }
                break;
            case RecordType.DecimalScore:
                LeanTween.scale(decimalScoreUIData.numbers.gameObject, defaultUIScaleData.numbers, _animationTime);
                LeanTween.scale(decimalScoreUIData.numbers_Decimal.gameObject, defaultUIScaleData.numbers_Decimal, _animationTime);
                LeanTween.scale(decimalScoreUIData.numbers_dot.gameObject, defaultUIScaleData.numbers_dot, _animationTime);
                if (decimalScoreUIData.addText_g.gameObject.activeSelf) {
                    LeanTween.scale(decimalScoreUIData.addText_g.gameObject, defaultUIScaleData.addText_g, _animationTime);
                }
                if (decimalScoreUIData.addText_cm.gameObject.activeSelf) {
                    LeanTween.scale(decimalScoreUIData.addText_cm.gameObject, defaultUIScaleData.addText_cm, _animationTime);
                }
                break;
            case RecordType.Time:
                LeanTween.scale(timeUIData.millSecond.gameObject, defaultUIScaleData.millSecond, _animationTime);
                LeanTween.scale(timeUIData.second.gameObject, defaultUIScaleData.second, _animationTime);
                LeanTween.scale(timeUIData.minutes.gameObject, defaultUIScaleData.minutes, _animationTime);
                LeanTween.scale(timeUIData.numbers_dot.gameObject, defaultUIScaleData.numbers_dot, _animationTime);
                LeanTween.scale(timeUIData.numbers_colon.gameObject, defaultUIScaleData.numbers_colon, _animationTime);
                LeanTween.scale(timeUIData.textTime.gameObject, defaultUIScaleData.text_Time, _animationTime);
                break;
            case RecordType.SecondTime:
                LeanTween.scale(secondTimeUIData.millSecond.gameObject, defaultUIScaleData.millSecond, _animationTime);
                LeanTween.scale(secondTimeUIData.second.gameObject, defaultUIScaleData.second, _animationTime);
                LeanTween.scale(secondTimeUIData.addText_Second.gameObject, defaultUIScaleData.addText_Second, _animationTime);
                break;
            case RecordType.ScoreFourDigit:
                LeanTween.scale(scoreFourDigitUIData.numbers.gameObject, defaultUIScaleData.numbers, _animationTime);
                LeanTween.scale(scoreFourDigitUIData.textNumbers.gameObject, defaultUIScaleData.textNumbers, _animationTime);
                if (scoreFourDigitUIData.addText_pt.gameObject.activeSelf) {
                    LeanTween.scale(scoreFourDigitUIData.addText_pt.gameObject, defaultUIScaleData.addText_pt, _animationTime);
                }
                if (scoreFourDigitUIData.addText_mL.gameObject.activeSelf) {
                    LeanTween.scale(scoreFourDigitUIData.addText_mL.gameObject, defaultUIScaleData.addText_mL, _animationTime);
                }
                break;
        }
    }
    public void Animation_Fade(float _animationTime) {
        switch (recordType) {
            case RecordType.Score:
                StartCoroutine(SetAlphaColor(scoreUIData.numbers.GetArraySpriteNumbers(), _animationTime));
                if (scoreUIData.addText_Count.gameObject.activeSelf) {
                    StartCoroutine(SetAlphaColor(scoreUIData.addText_Count, _animationTime));
                }
                if (scoreUIData.addText_Piece.gameObject.activeSelf) {
                    StartCoroutine(SetAlphaColor(scoreUIData.addText_Piece, _animationTime));
                }
                if (scoreUIData.addText_Animal.gameObject.activeSelf) {
                    StartCoroutine(SetAlphaColor(scoreUIData.addText_Animal, _animationTime));
                }
                if (scoreUIData.addText_Win.gameObject.activeSelf) {
                    StartCoroutine(SetAlphaColor(scoreUIData.addText_Win, _animationTime));
                }
                if (scoreUIData.addText_pt.gameObject.activeSelf) {
                    StartCoroutine(SetAlphaColor(scoreUIData.addText_pt, _animationTime));
                }
                break;
            case RecordType.DoubleDecimalScore:
                StartCoroutine(SetAlphaColor(doubleDecimalScoreUIData.numbers.GetArraySpriteNumbers(), _animationTime));
                StartCoroutine(SetAlphaColor(doubleDecimalScoreUIData.numbers_Decimal.GetArraySpriteNumbers(), _animationTime));
                StartCoroutine(SetAlphaColor(doubleDecimalScoreUIData.numbers_dot, _animationTime));
                if (doubleDecimalScoreUIData.addText_m.gameObject.activeSelf) {
                    StartCoroutine(SetAlphaColor(doubleDecimalScoreUIData.addText_m, _animationTime));
                }
                break;
            case RecordType.DecimalScore:
                StartCoroutine(SetAlphaColor(decimalScoreUIData.numbers.GetArraySpriteNumbers(), _animationTime));
                StartCoroutine(SetAlphaColor(decimalScoreUIData.numbers_Decimal.GetArraySpriteNumbers(), _animationTime));
                StartCoroutine(SetAlphaColor(decimalScoreUIData.numbers_dot, _animationTime));
                if (decimalScoreUIData.addText_g.gameObject.activeSelf) {
                    StartCoroutine(SetAlphaColor(decimalScoreUIData.addText_g, _animationTime));
                }
                if (decimalScoreUIData.addText_cm.gameObject.activeSelf) {
                    StartCoroutine(SetAlphaColor(decimalScoreUIData.addText_cm, _animationTime));
                }
                break;
            case RecordType.Time:
                StartCoroutine(SetAlphaColor(timeUIData.millSecond.GetArraySpriteNumbers(), _animationTime));
                StartCoroutine(SetAlphaColor(timeUIData.second.GetArraySpriteNumbers(), _animationTime));
                StartCoroutine(SetAlphaColor(timeUIData.minutes.GetArraySpriteNumbers(), _animationTime));
                StartCoroutine(SetAlphaColor(timeUIData.numbers_dot, _animationTime));
                StartCoroutine(SetAlphaColor(timeUIData.numbers_colon, _animationTime));
                StartCoroutine(SetAlphaColor(timeUIData.textTime, _animationTime));
                break;
            case RecordType.SecondTime:
                StartCoroutine(SetAlphaColor(secondTimeUIData.millSecond.GetArraySpriteNumbers(), _animationTime));
                StartCoroutine(SetAlphaColor(secondTimeUIData.second.GetArraySpriteNumbers(), _animationTime));
                StartCoroutine(SetAlphaColor(secondTimeUIData.addText_Second, _animationTime));
                break;
            case RecordType.ScoreFourDigit:
                StartCoroutine(SetAlphaColor(scoreFourDigitUIData.numbers.GetArraySpriteNumbers(), _animationTime));
                StartCoroutine(SetAlphaColor(scoreFourDigitUIData.textNumbers, _animationTime));
                if (scoreFourDigitUIData.addText_pt.gameObject.activeSelf) {
                    StartCoroutine(SetAlphaColor(scoreFourDigitUIData.addText_pt, _animationTime));
                }
                if (scoreFourDigitUIData.addText_mL.gameObject.activeSelf) {
                    StartCoroutine(SetAlphaColor(scoreFourDigitUIData.addText_mL, _animationTime));
                }
                break;
        }
    }
    public void SetUIActive(bool _isActive) {
        switch (recordType) {
            case RecordType.Time:
            case RecordType.SecondTime:
                break;
            case RecordType.Score:
                scoreUIData.addText_Count.gameObject.SetActive(_isActive);
                scoreUIData.addText_Piece.gameObject.SetActive(_isActive);
                scoreUIData.addText_Animal.gameObject.SetActive(_isActive);
                scoreUIData.addText_Win.gameObject.SetActive(_isActive);
                scoreUIData.addText_pt.gameObject.SetActive(_isActive);
                break;
            case RecordType.DoubleDecimalScore:
                doubleDecimalScoreUIData.addText_m.gameObject.SetActive(_isActive);
                break;
            case RecordType.DecimalScore:
                decimalScoreUIData.addText_g.gameObject.SetActive(_isActive);
                decimalScoreUIData.addText_cm.gameObject.SetActive(_isActive);
                break;
            case RecordType.ScoreFourDigit:
                scoreFourDigitUIData.addText_pt.gameObject.SetActive(_isActive);
                scoreFourDigitUIData.addText_mL.gameObject.SetActive(_isActive);
                break;
        }
    }
    public void SetUIScale(float _scale) {
        if (!saveUIDefScale) {
            switch (recordType) {
                case RecordType.Score:
                    defaultUIScaleData.numbers = scoreUIData.numbers.transform.localScale;
                    defaultUIScaleData.addText_Count = scoreUIData.addText_Count.transform.localScale;
                    defaultUIScaleData.addText_Piece = scoreUIData.addText_Piece.transform.localScale;
                    defaultUIScaleData.addText_Animal = scoreUIData.addText_Animal.transform.localScale;
                    defaultUIScaleData.addText_Win = scoreUIData.addText_Win.transform.localScale;
                    defaultUIScaleData.addText_pt = scoreUIData.addText_pt.transform.localScale;
                    break;
                case RecordType.DoubleDecimalScore:
                    defaultUIScaleData.numbers = doubleDecimalScoreUIData.numbers.transform.localScale;
                    defaultUIScaleData.numbers_Decimal = doubleDecimalScoreUIData.numbers_Decimal.transform.localScale;
                    defaultUIScaleData.numbers_dot = doubleDecimalScoreUIData.numbers_dot.transform.localScale;
                    defaultUIScaleData.addText_m = doubleDecimalScoreUIData.addText_m.transform.localScale;
                    break;
                case RecordType.DecimalScore:
                    defaultUIScaleData.numbers = decimalScoreUIData.numbers.transform.localScale;
                    defaultUIScaleData.numbers_Decimal = decimalScoreUIData.numbers_Decimal.transform.localScale;
                    defaultUIScaleData.numbers_dot = decimalScoreUIData.numbers_dot.transform.localScale;
                    defaultUIScaleData.addText_g = decimalScoreUIData.addText_g.transform.localScale;
                    defaultUIScaleData.addText_cm = decimalScoreUIData.addText_cm.transform.localScale;
                    break;
                case RecordType.Time:
                    defaultUIScaleData.millSecond = timeUIData.millSecond.transform.localScale;
                    defaultUIScaleData.second = timeUIData.second.transform.localScale;
                    defaultUIScaleData.minutes = timeUIData.minutes.transform.localScale;
                    defaultUIScaleData.numbers_dot = timeUIData.numbers_dot.transform.localScale;
                    defaultUIScaleData.numbers_colon = timeUIData.numbers_colon.transform.localScale;
                    defaultUIScaleData.text_Time = timeUIData.textTime.transform.localScale;
                    break;
                case RecordType.SecondTime:
                    defaultUIScaleData.millSecond = secondTimeUIData.millSecond.transform.localScale;
                    defaultUIScaleData.second = secondTimeUIData.second.transform.localScale;
                    defaultUIScaleData.addText_Second = secondTimeUIData.addText_Second.transform.localScale;
                    break;
                case RecordType.ScoreFourDigit:
                    defaultUIScaleData.numbers = scoreFourDigitUIData.numbers.transform.localScale;
                    defaultUIScaleData.textNumbers = scoreFourDigitUIData.textNumbers.transform.localScale;
                    defaultUIScaleData.addText_pt = scoreFourDigitUIData.addText_pt.transform.localScale;
                    defaultUIScaleData.addText_mL = scoreFourDigitUIData.addText_mL.transform.localScale;
                    break;
            }
            saveUIDefScale = true;
        }
        switch (recordType) {
            case RecordType.Score:
                scoreUIData.numbers.transform.localScale = Vector3.one * _scale;
                scoreUIData.addText_Count.transform.localScale = Vector3.one * _scale;
                scoreUIData.addText_Piece.transform.localScale = Vector3.one * _scale;
                scoreUIData.addText_Animal.transform.localScale = Vector3.one * _scale;
                scoreUIData.addText_Win.transform.localScale = Vector3.one * _scale;
                scoreUIData.addText_pt.transform.localScale = Vector3.one * _scale;
                break;
            case RecordType.DoubleDecimalScore:
                doubleDecimalScoreUIData.numbers.transform.localScale = Vector3.one * _scale;
                doubleDecimalScoreUIData.numbers_Decimal.transform.localScale = Vector3.one * _scale;
                doubleDecimalScoreUIData.numbers_dot.transform.localScale = Vector3.one * _scale;
                doubleDecimalScoreUIData.addText_m.transform.localScale = Vector3.one * _scale;
                break;
            case RecordType.DecimalScore:
                decimalScoreUIData.numbers.transform.localScale = Vector3.one * _scale;
                decimalScoreUIData.numbers_Decimal.transform.localScale = Vector3.one * _scale;
                decimalScoreUIData.numbers_dot.transform.localScale = Vector3.one * _scale;
                decimalScoreUIData.addText_g.transform.localScale = Vector3.one * _scale;
                decimalScoreUIData.addText_cm.transform.localScale = Vector3.one * _scale;
                break;
            case RecordType.Time:
                timeUIData.millSecond.transform.localScale = Vector3.one * _scale;
                timeUIData.second.transform.localScale = Vector3.one * _scale;
                timeUIData.minutes.transform.localScale = Vector3.one * _scale;
                timeUIData.numbers_dot.transform.localScale = Vector3.one * _scale;
                timeUIData.numbers_colon.transform.localScale = Vector3.one * _scale;
                timeUIData.textTime.transform.localScale = Vector3.one * _scale;
                break;
            case RecordType.SecondTime:
                secondTimeUIData.millSecond.transform.localScale = Vector3.one * _scale;
                secondTimeUIData.second.transform.localScale = Vector3.one * _scale;
                secondTimeUIData.addText_Second.transform.localScale = Vector3.one * _scale;
                break;
            case RecordType.ScoreFourDigit:
                scoreFourDigitUIData.numbers.transform.localScale = Vector3.one * _scale;
                scoreFourDigitUIData.textNumbers.transform.localScale = Vector3.one * _scale;
                scoreFourDigitUIData.addText_pt.transform.localScale = Vector3.one * _scale;
                scoreFourDigitUIData.addText_mL.transform.localScale = Vector3.one * _scale;
                break;
        }
    }
    public void SetUIAlpha(float _alpha) {
        switch (recordType) {
            case RecordType.Score:
                scoreUIData.numbers.SetAlpha(_alpha);
                scoreUIData.addText_Count.SetAlpha(_alpha);
                scoreUIData.addText_Piece.SetAlpha(_alpha);
                scoreUIData.addText_Animal.SetAlpha(_alpha);
                scoreUIData.addText_Win.SetAlpha(_alpha);
                scoreUIData.addText_pt.SetAlpha(_alpha);
                break;
            case RecordType.DoubleDecimalScore:
                doubleDecimalScoreUIData.numbers.SetAlpha(_alpha);
                doubleDecimalScoreUIData.numbers_Decimal.SetAlpha(_alpha);
                doubleDecimalScoreUIData.numbers_dot.SetAlpha(_alpha);
                doubleDecimalScoreUIData.addText_m.SetAlpha(_alpha);
                break;
            case RecordType.DecimalScore:
                decimalScoreUIData.numbers.SetAlpha(_alpha);
                decimalScoreUIData.numbers_Decimal.SetAlpha(_alpha);
                decimalScoreUIData.numbers_dot.SetAlpha(_alpha);
                decimalScoreUIData.addText_g.SetAlpha(_alpha);
                decimalScoreUIData.addText_cm.SetAlpha(_alpha);
                break;
            case RecordType.Time:
                timeUIData.millSecond.SetAlpha(_alpha);
                timeUIData.second.SetAlpha(_alpha);
                timeUIData.minutes.SetAlpha(_alpha);
                timeUIData.numbers_dot.SetAlpha(_alpha);
                timeUIData.numbers_colon.SetAlpha(_alpha);
                timeUIData.textTime.SetAlpha(_alpha);
                break;
            case RecordType.SecondTime:
                secondTimeUIData.millSecond.SetAlpha(_alpha);
                secondTimeUIData.second.SetAlpha(_alpha);
                secondTimeUIData.addText_Second.SetAlpha(_alpha);
                break;
            case RecordType.ScoreFourDigit:
                scoreFourDigitUIData.numbers.SetAlpha(_alpha);
                scoreFourDigitUIData.textNumbers.SetAlpha(_alpha);
                scoreFourDigitUIData.addText_pt.SetAlpha(_alpha);
                scoreFourDigitUIData.addText_mL.SetAlpha(_alpha);
                break;
        }
    }
    public void SetMaskInteraction(SpriteMaskInteraction _maskInteraction) {
        switch (recordType) {
            case RecordType.Score:
                scoreUIData.numbers.SetSpriteRendererMaskInteraciton(_maskInteraction);
                scoreUIData.addText_Count.maskInteraction = _maskInteraction;
                scoreUIData.addText_Piece.maskInteraction = _maskInteraction;
                scoreUIData.addText_Animal.maskInteraction = _maskInteraction;
                scoreUIData.addText_Win.maskInteraction = _maskInteraction;
                scoreUIData.addText_pt.maskInteraction = _maskInteraction;
                break;
            case RecordType.DoubleDecimalScore:
                doubleDecimalScoreUIData.numbers.SetSpriteRendererMaskInteraciton(_maskInteraction);
                doubleDecimalScoreUIData.numbers_Decimal.SetSpriteRendererMaskInteraciton(_maskInteraction);
                doubleDecimalScoreUIData.numbers_dot.maskInteraction = _maskInteraction;
                doubleDecimalScoreUIData.addText_m.maskInteraction = _maskInteraction;
                break;
            case RecordType.DecimalScore:
                decimalScoreUIData.numbers.SetSpriteRendererMaskInteraciton(_maskInteraction);
                decimalScoreUIData.numbers_Decimal.SetSpriteRendererMaskInteraciton(_maskInteraction);
                decimalScoreUIData.numbers_dot.maskInteraction = _maskInteraction;
                decimalScoreUIData.addText_g.maskInteraction = _maskInteraction;
                decimalScoreUIData.addText_cm.maskInteraction = _maskInteraction;
                break;
            case RecordType.Time:
                timeUIData.millSecond.SetSpriteRendererMaskInteraciton(_maskInteraction);
                timeUIData.second.SetSpriteRendererMaskInteraciton(_maskInteraction);
                timeUIData.minutes.SetSpriteRendererMaskInteraciton(_maskInteraction);
                timeUIData.numbers_dot.maskInteraction = _maskInteraction;
                timeUIData.numbers_colon.maskInteraction = _maskInteraction;
                break;
            case RecordType.SecondTime:
                secondTimeUIData.millSecond.SetSpriteRendererMaskInteraciton(_maskInteraction);
                secondTimeUIData.second.SetSpriteRendererMaskInteraciton(_maskInteraction);
                secondTimeUIData.addText_Second.maskInteraction = _maskInteraction;
                break;
            case RecordType.ScoreFourDigit:
                scoreFourDigitUIData.numbers.SetSpriteRendererMaskInteraciton(_maskInteraction);
                scoreFourDigitUIData.addText_pt.maskInteraction = _maskInteraction;
                scoreFourDigitUIData.addText_mL.maskInteraction = _maskInteraction;
                break;
        }
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
    private IEnumerator SetAlphaColor(TextMeshPro _text, float _fadeTime, float _delayTime = 0f, bool _isFadeOut = false) {
        float time = 0f;
        float startAlpha = 0f;
        float endAlpha = 1f;
        if (_isFadeOut) {
            startAlpha = 1f;
            endAlpha = 0f;
        }
        yield return new WaitForSeconds(_delayTime);
        while (time < _fadeTime) {
            _text.SetAlpha(Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime));
            time += Time.deltaTime;
            yield return null;
        }
        _text.SetAlpha(endAlpha);
    }
}
