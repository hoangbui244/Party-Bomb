using System;
using System.Collections;
using UnityEngine;
public class WinOrLoseUIData : MonoBehaviour {
    [Serializable]
    private struct DefaultUIScaleData {
        public Vector2 numbers;
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
        public Vector2 teamName;
    }
    [Serializable]
    public struct RecordUIData {
        [Header("各チ\u30fcムの名前画像")]
        public SpriteRenderer[] teamNameSprite;
        [Header("[000]のスコア記録UIデ\u30fcタ")]
        public ScoreUIData[] scoreUIData;
        [Header("[000.0]のスコア記録UIデ\u30fcタ")]
        public DecimalScoreUIData[] decimalScoreUIData;
        [Header("[0000]のスコア記録UIデ\u30fcタ")]
        public ScoreFourDigitUIData[] scoreFourDigitUIData;
    }
    [Serializable]
    public struct ScoreUIData {
        [Header("記録のアンカ\u30fc")]
        public Transform recordAnchor;
        [Header("数値")]
        public SpriteNumbers numbers;
        [Header("「回」の文字")]
        public SpriteRenderer addText_Count;
        [Header("「pt」の文字")]
        public SpriteRenderer addText_pt;
        [Header("「個」の文字")]
        public SpriteRenderer addText_Piece;
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
    public struct ScoreFourDigitUIData {
        [Header("記録のアンカ\u30fc")]
        public Transform recordAnchor;
        [SerializeField]
        [Header("数値")]
        public SpriteNumbers numbers;
        [SerializeField]
        [Header("「pt」の文字")]
        public SpriteRenderer addText_pt;
        [SerializeField]
        [Header("「mL」の文字")]
        public SpriteRenderer addText_mL;
    }
    public enum RecordType {
        Score,
        DecimalScore,
        ScoreFourDigit
    }
    [SerializeField]
    [Header("記録UIデ\u30fcタ")]
    private RecordUIData recordUIData;
    [SerializeField]
    [Header("フレ\u30fcム")]
    private SpriteRenderer frame;
    [SerializeField]
    [Header("背景色")]
    private SpriteRenderer backColor;
    private DefaultUIScaleData defaultUIScaleData;
    private bool saveUIDefScale;
    private bool isRecordActive;
    private void Awake() {
        frame.gameObject.SetActive(value: false);
        backColor.gameObject.SetActive(value: false);
        for (int i = 0; i < recordUIData.scoreUIData.Length; i++) {
            recordUIData.scoreUIData[i].recordAnchor.gameObject.SetActive(value: false);
            recordUIData.scoreUIData[i].addText_Count.gameObject.SetActive(value: false);
            recordUIData.scoreUIData[i].addText_pt.gameObject.SetActive(value: false);
            recordUIData.scoreUIData[i].addText_Piece.gameObject.SetActive(value: false);
        }
        for (int j = 0; j < recordUIData.decimalScoreUIData.Length; j++) {
            recordUIData.decimalScoreUIData[j].recordAnchor.gameObject.SetActive(value: false);
            recordUIData.decimalScoreUIData[j].addText_cm.gameObject.SetActive(value: false);
            recordUIData.decimalScoreUIData[j].addText_g.gameObject.SetActive(value: false);
        }
        for (int k = 0; k < recordUIData.scoreFourDigitUIData.Length; k++) {
            recordUIData.scoreFourDigitUIData[k].recordAnchor.gameObject.SetActive(value: false);
            recordUIData.scoreFourDigitUIData[k].addText_mL.gameObject.SetActive(value: false);
            recordUIData.scoreFourDigitUIData[k].addText_pt.gameObject.SetActive(value: false);
        }
        SetUIScale(1.2f);
        SetUIAlpha(0f);
    }
    public void Animation_Scaling(float _animationTime, RecordType _recordType) {
        for (int i = 0; i < recordUIData.teamNameSprite.Length; i++) {
            LeanTween.scale(recordUIData.teamNameSprite[i].gameObject, defaultUIScaleData.teamName, _animationTime);
        }
        switch (_recordType) {
            case RecordType.Score:
                for (int k = 0; k < recordUIData.scoreUIData.Length; k++) {
                    LeanTween.scale(recordUIData.scoreUIData[k].numbers.gameObject, defaultUIScaleData.numbers, _animationTime);
                    if (recordUIData.scoreUIData[k].addText_Count.gameObject.activeSelf) {
                        LeanTween.scale(recordUIData.scoreUIData[k].addText_Count.gameObject, defaultUIScaleData.addText_Count, _animationTime);
                    }
                    if (recordUIData.scoreUIData[k].addText_pt.gameObject.activeSelf) {
                        LeanTween.scale(recordUIData.scoreUIData[k].addText_pt.gameObject, defaultUIScaleData.addText_pt, _animationTime);
                    }
                    if (recordUIData.scoreUIData[k].addText_Piece.gameObject.activeSelf) {
                        LeanTween.scale(recordUIData.scoreUIData[k].addText_Piece.gameObject, defaultUIScaleData.addText_Piece, _animationTime);
                    }
                }
                break;
            case RecordType.DecimalScore:
                for (int l = 0; l < recordUIData.decimalScoreUIData.Length; l++) {
                    LeanTween.scale(recordUIData.decimalScoreUIData[l].numbers.gameObject, defaultUIScaleData.numbers, _animationTime);
                    LeanTween.scale(recordUIData.decimalScoreUIData[l].numbers_Decimal.gameObject, defaultUIScaleData.numbers_Decimal, _animationTime);
                    LeanTween.scale(recordUIData.decimalScoreUIData[l].numbers_dot.gameObject, defaultUIScaleData.numbers_dot, _animationTime);
                    if (recordUIData.decimalScoreUIData[l].addText_cm.gameObject.activeSelf) {
                        LeanTween.scale(recordUIData.decimalScoreUIData[l].addText_cm.gameObject, defaultUIScaleData.addText_cm, _animationTime);
                    }
                    if (recordUIData.decimalScoreUIData[l].addText_g.gameObject.activeSelf) {
                        LeanTween.scale(recordUIData.decimalScoreUIData[l].addText_g.gameObject, defaultUIScaleData.addText_g, _animationTime);
                    }
                }
                break;
            case RecordType.ScoreFourDigit:
                for (int j = 0; j < recordUIData.scoreFourDigitUIData.Length; j++) {
                    LeanTween.scale(recordUIData.scoreFourDigitUIData[j].numbers.gameObject, defaultUIScaleData.numbers, _animationTime);
                    if (recordUIData.scoreFourDigitUIData[j].addText_pt.gameObject.activeSelf) {
                        LeanTween.scale(recordUIData.scoreFourDigitUIData[j].addText_pt.gameObject, defaultUIScaleData.addText_pt, _animationTime);
                    }
                    if (recordUIData.scoreFourDigitUIData[j].addText_mL.gameObject.activeSelf) {
                        LeanTween.scale(recordUIData.scoreFourDigitUIData[j].addText_mL.gameObject, defaultUIScaleData.addText_mL, _animationTime);
                    }
                }
                break;
        }
    }
    public void Animation_Fade(float _animationTime, RecordType _recordType) {
        for (int i = 0; i < recordUIData.teamNameSprite.Length; i++) {
            StartCoroutine(SetAlphaColor(recordUIData.teamNameSprite[i], _animationTime));
        }
        switch (_recordType) {
            case RecordType.Score:
                for (int k = 0; k < recordUIData.scoreUIData.Length; k++) {
                    StartCoroutine(SetAlphaColor(recordUIData.scoreUIData[k].numbers.GetArraySpriteNumbers(), _animationTime));
                    if (recordUIData.scoreUIData[k].addText_Count.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(recordUIData.scoreUIData[k].addText_Count, _animationTime));
                    }
                    if (recordUIData.scoreUIData[k].addText_pt.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(recordUIData.scoreUIData[k].addText_pt, _animationTime));
                    }
                    if (recordUIData.scoreUIData[k].addText_Piece.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(recordUIData.scoreUIData[k].addText_Piece, _animationTime));
                    }
                }
                break;
            case RecordType.DecimalScore:
                for (int l = 0; l < recordUIData.decimalScoreUIData.Length; l++) {
                    StartCoroutine(SetAlphaColor(recordUIData.decimalScoreUIData[l].numbers.GetArraySpriteNumbers(), _animationTime));
                    StartCoroutine(SetAlphaColor(recordUIData.decimalScoreUIData[l].numbers_Decimal.GetArraySpriteNumbers(), _animationTime));
                    StartCoroutine(SetAlphaColor(recordUIData.decimalScoreUIData[l].numbers_dot, _animationTime));
                    if (recordUIData.decimalScoreUIData[l].addText_g.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(recordUIData.decimalScoreUIData[l].addText_g, _animationTime));
                    }
                    if (recordUIData.decimalScoreUIData[l].addText_cm.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(recordUIData.decimalScoreUIData[l].addText_cm, _animationTime));
                    }
                }
                break;
            case RecordType.ScoreFourDigit:
                for (int j = 0; j < recordUIData.scoreFourDigitUIData.Length; j++) {
                    StartCoroutine(SetAlphaColor(recordUIData.scoreFourDigitUIData[j].numbers.GetArraySpriteNumbers(), _animationTime));
                    if (recordUIData.scoreFourDigitUIData[j].addText_pt.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(recordUIData.scoreFourDigitUIData[j].addText_pt, _animationTime));
                    }
                    if (recordUIData.scoreFourDigitUIData[j].addText_mL.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(recordUIData.scoreFourDigitUIData[j].addText_mL, _animationTime));
                    }
                }
                break;
        }
    }
    public void SetScore_Int(int _teamNo, int _score, RecordType _recordType) {
        switch (_recordType) {
            case RecordType.Score:
                recordUIData.scoreUIData[_teamNo].numbers.Set(_score);
                break;
            case RecordType.ScoreFourDigit:
                recordUIData.scoreFourDigitUIData[_teamNo].numbers.Set(_score);
                break;
        }
    }
    public void SetScore_Float(int _teamNo, float _score, RecordType _recordType) {
        string[] array = _score.ToString().Split('.');
        if (array.Length == 1) {
            array = new string[2]
            {
                _score.ToString(),
                "0"
            };
        }
        if (_recordType == RecordType.DecimalScore) {
            recordUIData.decimalScoreUIData[_teamNo].numbers.SetNumbers(array[0]);
            recordUIData.decimalScoreUIData[_teamNo].numbers_Decimal.SetNumbers(array[1]);
        }
    }
    public void SetUIActive(bool _isActive, RecordType _recordType) {
        isRecordActive = _isActive;
        frame.gameObject.SetActive(value: true);
        backColor.gameObject.SetActive(value: true);
        switch (_recordType) {
            case RecordType.Score:
                for (int j = 0; j < recordUIData.scoreUIData.Length; j++) {
                    recordUIData.scoreUIData[j].recordAnchor.gameObject.SetActive(_isActive);
                    recordUIData.scoreUIData[j].addText_pt.gameObject.SetActive(_isActive);
                }
                break;
            case RecordType.DecimalScore:
                for (int k = 0; k < recordUIData.decimalScoreUIData.Length; k++) {
                    recordUIData.decimalScoreUIData[k].recordAnchor.gameObject.SetActive(_isActive);
                }
                break;
            case RecordType.ScoreFourDigit:
                for (int i = 0; i < recordUIData.scoreFourDigitUIData.Length; i++) {
                    recordUIData.scoreFourDigitUIData[i].recordAnchor.gameObject.SetActive(_isActive);
                    recordUIData.scoreFourDigitUIData[i].addText_pt.gameObject.SetActive(_isActive);
                }
                break;
        }
    }
    public void SetUIScale(float _scale) {
        if (!saveUIDefScale) {
            for (int i = 0; i < recordUIData.teamNameSprite.Length; i++) {
                defaultUIScaleData.teamName = recordUIData.teamNameSprite[i].transform.localScale;
            }
            for (int j = 0; j < recordUIData.scoreUIData.Length; j++) {
                defaultUIScaleData.numbers = recordUIData.scoreUIData[j].numbers.transform.localScale;
                defaultUIScaleData.addText_Count = recordUIData.scoreUIData[j].addText_Count.transform.localScale;
                defaultUIScaleData.addText_pt = recordUIData.scoreUIData[j].addText_pt.transform.localScale;
                defaultUIScaleData.addText_Piece = recordUIData.scoreUIData[j].addText_Piece.transform.localScale;
            }
            for (int k = 0; k < recordUIData.decimalScoreUIData.Length; k++) {
                defaultUIScaleData.numbers = recordUIData.decimalScoreUIData[k].numbers.transform.localScale;
                defaultUIScaleData.numbers_Decimal = recordUIData.decimalScoreUIData[k].numbers_Decimal.transform.localScale;
                defaultUIScaleData.numbers_dot = recordUIData.decimalScoreUIData[k].numbers_dot.transform.localScale;
                defaultUIScaleData.addText_g = recordUIData.decimalScoreUIData[k].addText_g.transform.localScale;
                defaultUIScaleData.addText_cm = recordUIData.decimalScoreUIData[k].addText_cm.transform.localScale;
            }
            for (int l = 0; l < recordUIData.scoreFourDigitUIData.Length; l++) {
                defaultUIScaleData.numbers = recordUIData.scoreFourDigitUIData[l].numbers.transform.localScale;
                defaultUIScaleData.addText_pt = recordUIData.scoreFourDigitUIData[l].addText_pt.transform.localScale;
                defaultUIScaleData.addText_mL = recordUIData.scoreFourDigitUIData[l].addText_mL.transform.localScale;
            }
            saveUIDefScale = true;
        }
        for (int m = 0; m < recordUIData.teamNameSprite.Length; m++) {
            recordUIData.teamNameSprite[m].transform.localScale = Vector3.one * _scale;
        }
        for (int n = 0; n < recordUIData.scoreUIData.Length; n++) {
            recordUIData.scoreUIData[n].numbers.transform.localScale = Vector3.one * _scale;
            recordUIData.scoreUIData[n].addText_Count.transform.localScale = Vector3.one * _scale;
            recordUIData.scoreUIData[n].addText_pt.transform.localScale = Vector3.one * _scale;
            recordUIData.scoreUIData[n].addText_Piece.transform.localScale = Vector3.one * _scale;
        }
        for (int num = 0; num < recordUIData.decimalScoreUIData.Length; num++) {
            recordUIData.decimalScoreUIData[num].numbers.transform.localScale = Vector3.one * _scale;
            recordUIData.decimalScoreUIData[num].numbers_Decimal.transform.localScale = Vector3.one * _scale;
            recordUIData.decimalScoreUIData[num].numbers_dot.transform.localScale = Vector3.one * _scale;
            recordUIData.decimalScoreUIData[num].addText_g.transform.localScale = Vector3.one * _scale;
            recordUIData.decimalScoreUIData[num].addText_cm.transform.localScale = Vector3.one * _scale;
        }
        for (int num2 = 0; num2 < recordUIData.scoreFourDigitUIData.Length; num2++) {
            recordUIData.scoreFourDigitUIData[num2].numbers.transform.localScale = Vector3.one * _scale;
            recordUIData.scoreFourDigitUIData[num2].addText_pt.transform.localScale = Vector3.one * _scale;
            recordUIData.scoreFourDigitUIData[num2].addText_mL.transform.localScale = Vector3.one * _scale;
        }
    }
    public void SetUIAlpha(float _alpha) {
        for (int i = 0; i < recordUIData.teamNameSprite.Length; i++) {
            recordUIData.teamNameSprite[i].SetAlpha(_alpha);
        }
        for (int j = 0; j < recordUIData.scoreUIData.Length; j++) {
            recordUIData.scoreUIData[j].numbers.SetAlpha(_alpha);
            recordUIData.scoreUIData[j].addText_Count.SetAlpha(_alpha);
            recordUIData.scoreUIData[j].addText_pt.SetAlpha(_alpha);
            recordUIData.scoreUIData[j].addText_Piece.SetAlpha(_alpha);
        }
        for (int k = 0; k < recordUIData.decimalScoreUIData.Length; k++) {
            recordUIData.decimalScoreUIData[k].numbers.SetAlpha(_alpha);
            recordUIData.decimalScoreUIData[k].numbers_Decimal.SetAlpha(_alpha);
            recordUIData.decimalScoreUIData[k].numbers_dot.SetAlpha(_alpha);
            recordUIData.decimalScoreUIData[k].addText_g.SetAlpha(_alpha);
            recordUIData.decimalScoreUIData[k].addText_cm.SetAlpha(_alpha);
        }
        for (int l = 0; l < recordUIData.scoreFourDigitUIData.Length; l++) {
            recordUIData.scoreFourDigitUIData[l].numbers.SetAlpha(_alpha);
            recordUIData.scoreFourDigitUIData[l].addText_pt.SetAlpha(_alpha);
            recordUIData.scoreFourDigitUIData[l].addText_mL.SetAlpha(_alpha);
        }
    }
    public RecordUIData GetLastResultRecordUIData() {
        return recordUIData;
    }
    public bool IsRecordActive() {
        return isRecordActive;
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
}
