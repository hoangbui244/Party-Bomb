using System;
using System.Collections;
using UnityEngine;
public class RankingLastResultRecordUIData : MonoBehaviour {
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
    public struct LastResultRecordUIData {
        [Header("各チ\u30fcムの名前画像")]
        [NonReorderable]
        public SpriteRenderer[] teamNameSprite;
        [Header("[000]のスコア記録UIデ\u30fcタ")]
        [NonReorderable]
        public ScoreUIData[] scoreUIData;
        [Header("[0000]のスコア記録UIデ\u30fcタ")]
        [NonReorderable]
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
        [Header("「個」の文字")]
        public SpriteRenderer addText_Piece;
        [Header("「pt」の文字")]
        public SpriteRenderer addText_pt;
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
        DoubleDecimalScore,
        DecimalScore,
        Time,
        SecondTime,
        ScoreFourDigit
    }
    [SerializeField]
    [Header("順位リザルトの最終結果の記録UIデ\u30fcタ")]
    private LastResultRecordUIData lastResultRecordUIData;
    [SerializeField]
    [Header("フレ\u30fcム")]
    private SpriteRenderer frame;
    [SerializeField]
    [Header("背景色")]
    private SpriteRenderer backColor;
    private DefaultUIScaleData defaultUIScaleData;
    private bool saveUIDefScale;
    private void Awake() {
        for (int i = 0; i < lastResultRecordUIData.scoreUIData.Length; i++) {
            lastResultRecordUIData.scoreUIData[i].recordAnchor.gameObject.SetActive(value: false);
            lastResultRecordUIData.scoreUIData[i].addText_Count.gameObject.SetActive(value: false);
            lastResultRecordUIData.scoreUIData[i].addText_Piece.gameObject.SetActive(value: false);
            lastResultRecordUIData.scoreUIData[i].addText_pt.gameObject.SetActive(value: false);
        }
        for (int j = 0; j < lastResultRecordUIData.scoreFourDigitUIData.Length; j++) {
            lastResultRecordUIData.scoreFourDigitUIData[j].recordAnchor.gameObject.SetActive(value: false);
            lastResultRecordUIData.scoreFourDigitUIData[j].addText_mL.gameObject.SetActive(value: false);
            lastResultRecordUIData.scoreFourDigitUIData[j].addText_pt.gameObject.SetActive(value: false);
        }
        SetUIScale(1.2f);
        SetUIAlpha(0f);
        SetMaskInteraction(SpriteMaskInteraction.VisibleInsideMask, RecordType.Score);
        SetMaskInteraction(SpriteMaskInteraction.VisibleInsideMask, RecordType.ScoreFourDigit);
        frame.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        backColor.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }
    public void Animation_Scaling(float _animationTime, RecordType _recordType) {
        for (int i = 0; i < lastResultRecordUIData.teamNameSprite.Length; i++) {
            LeanTween.scale(lastResultRecordUIData.teamNameSprite[i].gameObject, defaultUIScaleData.teamName, _animationTime);
        }
        switch (_recordType) {
            case RecordType.Score:
                for (int k = 0; k < lastResultRecordUIData.scoreUIData.Length; k++) {
                    LeanTween.scale(lastResultRecordUIData.scoreUIData[k].numbers.gameObject, defaultUIScaleData.numbers, _animationTime);
                    if (lastResultRecordUIData.scoreUIData[k].addText_Count.gameObject.activeSelf) {
                        LeanTween.scale(lastResultRecordUIData.scoreUIData[k].addText_Count.gameObject, defaultUIScaleData.addText_Count, _animationTime);
                    }
                    if (lastResultRecordUIData.scoreUIData[k].addText_Piece.gameObject.activeSelf) {
                        LeanTween.scale(lastResultRecordUIData.scoreUIData[k].addText_Piece.gameObject, defaultUIScaleData.addText_Piece, _animationTime);
                    }
                    if (lastResultRecordUIData.scoreUIData[k].addText_pt.gameObject.activeSelf) {
                        LeanTween.scale(lastResultRecordUIData.scoreUIData[k].addText_pt.gameObject, defaultUIScaleData.addText_pt, _animationTime);
                    }
                }
                break;
            case RecordType.ScoreFourDigit:
                for (int j = 0; j < lastResultRecordUIData.scoreFourDigitUIData.Length; j++) {
                    LeanTween.scale(lastResultRecordUIData.scoreFourDigitUIData[j].numbers.gameObject, defaultUIScaleData.numbers, _animationTime);
                    if (lastResultRecordUIData.scoreFourDigitUIData[j].addText_pt.gameObject.activeSelf) {
                        LeanTween.scale(lastResultRecordUIData.scoreFourDigitUIData[j].addText_pt.gameObject, defaultUIScaleData.addText_pt, _animationTime);
                    }
                    if (lastResultRecordUIData.scoreFourDigitUIData[j].addText_mL.gameObject.activeSelf) {
                        LeanTween.scale(lastResultRecordUIData.scoreFourDigitUIData[j].addText_mL.gameObject, defaultUIScaleData.addText_mL, _animationTime);
                    }
                }
                break;
        }
    }
    public void Animation_Fade(float _animationTime, RecordType _recordType) {
        for (int i = 0; i < lastResultRecordUIData.teamNameSprite.Length; i++) {
            StartCoroutine(SetAlphaColor(lastResultRecordUIData.teamNameSprite[i], _animationTime));
        }
        switch (_recordType) {
            case RecordType.Score:
                for (int k = 0; k < lastResultRecordUIData.scoreUIData.Length; k++) {
                    StartCoroutine(SetAlphaColor(lastResultRecordUIData.scoreUIData[k].numbers.GetArraySpriteNumbers(), _animationTime));
                    if (lastResultRecordUIData.scoreUIData[k].addText_Count.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(lastResultRecordUIData.scoreUIData[k].addText_Count, _animationTime));
                    }
                    if (lastResultRecordUIData.scoreUIData[k].addText_Piece.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(lastResultRecordUIData.scoreUIData[k].addText_Piece, _animationTime));
                    }
                    if (lastResultRecordUIData.scoreUIData[k].addText_pt.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(lastResultRecordUIData.scoreUIData[k].addText_pt, _animationTime));
                    }
                }
                break;
            case RecordType.ScoreFourDigit:
                for (int j = 0; j < lastResultRecordUIData.scoreFourDigitUIData.Length; j++) {
                    StartCoroutine(SetAlphaColor(lastResultRecordUIData.scoreFourDigitUIData[j].numbers.GetArraySpriteNumbers(), _animationTime));
                    if (lastResultRecordUIData.scoreFourDigitUIData[j].addText_pt.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(lastResultRecordUIData.scoreFourDigitUIData[j].addText_pt, _animationTime));
                    }
                    if (lastResultRecordUIData.scoreFourDigitUIData[j].addText_mL.gameObject.activeSelf) {
                        StartCoroutine(SetAlphaColor(lastResultRecordUIData.scoreFourDigitUIData[j].addText_mL, _animationTime));
                    }
                }
                break;
        }
    }
    public void SetScore(int _teamNo, int _score) {
        if (lastResultRecordUIData.scoreUIData[_teamNo].recordAnchor.gameObject.activeSelf) {
            lastResultRecordUIData.scoreUIData[_teamNo].numbers.Set(_score);
        } else if (lastResultRecordUIData.scoreFourDigitUIData[_teamNo].recordAnchor.gameObject.activeSelf) {
            lastResultRecordUIData.scoreFourDigitUIData[_teamNo].numbers.Set(_score);
        }
    }
    public void SetUIActive(bool _isActive, RecordType _recordType) {
        switch (_recordType) {
            case RecordType.Score:
                for (int j = 0; j < lastResultRecordUIData.scoreUIData.Length; j++) {
                    lastResultRecordUIData.scoreUIData[j].recordAnchor.gameObject.SetActive(_isActive);
                    ResultGameDataParams.GetNowSceneType();
                    lastResultRecordUIData.scoreUIData[j].addText_pt.gameObject.SetActive(_isActive);
                }
                break;
            case RecordType.ScoreFourDigit:
                for (int i = 0; i < lastResultRecordUIData.scoreFourDigitUIData.Length; i++) {
                    lastResultRecordUIData.scoreFourDigitUIData[i].recordAnchor.gameObject.SetActive(_isActive);
                    ResultGameDataParams.GetNowSceneType();
                    lastResultRecordUIData.scoreFourDigitUIData[i].addText_pt.gameObject.SetActive(_isActive);
                }
                break;
        }
    }
    public void SetUIScale(float _scale) {
        if (!saveUIDefScale) {
            for (int i = 0; i < lastResultRecordUIData.teamNameSprite.Length; i++) {
                defaultUIScaleData.teamName = lastResultRecordUIData.teamNameSprite[i].transform.localScale;
            }
            for (int j = 0; j < lastResultRecordUIData.scoreUIData.Length; j++) {
                defaultUIScaleData.numbers = lastResultRecordUIData.scoreUIData[j].numbers.transform.localScale;
                defaultUIScaleData.addText_Count = lastResultRecordUIData.scoreUIData[j].addText_Count.transform.localScale;
                defaultUIScaleData.addText_Piece = lastResultRecordUIData.scoreUIData[j].addText_Piece.transform.localScale;
                defaultUIScaleData.addText_pt = lastResultRecordUIData.scoreUIData[j].addText_pt.transform.localScale;
            }
            for (int k = 0; k < lastResultRecordUIData.scoreFourDigitUIData.Length; k++) {
                defaultUIScaleData.numbers = lastResultRecordUIData.scoreFourDigitUIData[k].numbers.transform.localScale;
                defaultUIScaleData.addText_pt = lastResultRecordUIData.scoreFourDigitUIData[k].addText_pt.transform.localScale;
                defaultUIScaleData.addText_mL = lastResultRecordUIData.scoreFourDigitUIData[k].addText_mL.transform.localScale;
            }
            saveUIDefScale = true;
        }
        for (int l = 0; l < lastResultRecordUIData.teamNameSprite.Length; l++) {
            lastResultRecordUIData.teamNameSprite[l].transform.localScale = Vector3.one * _scale;
        }
        for (int m = 0; m < lastResultRecordUIData.scoreUIData.Length; m++) {
            lastResultRecordUIData.scoreUIData[m].numbers.transform.localScale = Vector3.one * _scale;
            lastResultRecordUIData.scoreUIData[m].addText_Count.transform.localScale = Vector3.one * _scale;
            lastResultRecordUIData.scoreUIData[m].addText_Piece.transform.localScale = Vector3.one * _scale;
            lastResultRecordUIData.scoreUIData[m].addText_pt.transform.localScale = Vector3.one * _scale;
        }
        for (int n = 0; n < lastResultRecordUIData.scoreFourDigitUIData.Length; n++) {
            lastResultRecordUIData.scoreFourDigitUIData[n].numbers.transform.localScale = Vector3.one * _scale;
            lastResultRecordUIData.scoreFourDigitUIData[n].addText_pt.transform.localScale = Vector3.one * _scale;
            lastResultRecordUIData.scoreFourDigitUIData[n].addText_mL.transform.localScale = Vector3.one * _scale;
        }
    }
    public void SetUIAlpha(float _alpha) {
        for (int i = 0; i < lastResultRecordUIData.teamNameSprite.Length; i++) {
            lastResultRecordUIData.teamNameSprite[i].SetAlpha(_alpha);
        }
        for (int j = 0; j < lastResultRecordUIData.scoreUIData.Length; j++) {
            lastResultRecordUIData.scoreUIData[j].numbers.SetAlpha(_alpha);
            lastResultRecordUIData.scoreUIData[j].addText_Count.SetAlpha(_alpha);
            lastResultRecordUIData.scoreUIData[j].addText_Piece.SetAlpha(_alpha);
            lastResultRecordUIData.scoreUIData[j].addText_pt.SetAlpha(_alpha);
        }
        for (int k = 0; k < lastResultRecordUIData.scoreFourDigitUIData.Length; k++) {
            lastResultRecordUIData.scoreFourDigitUIData[k].numbers.SetAlpha(_alpha);
            lastResultRecordUIData.scoreFourDigitUIData[k].addText_pt.SetAlpha(_alpha);
            lastResultRecordUIData.scoreFourDigitUIData[k].addText_mL.SetAlpha(_alpha);
        }
    }
    public void SetMaskInteraction(SpriteMaskInteraction _maskInteraction, RecordType _recordType) {
        for (int i = 0; i < lastResultRecordUIData.teamNameSprite.Length; i++) {
            lastResultRecordUIData.teamNameSprite[i].maskInteraction = _maskInteraction;
        }
        switch (_recordType) {
            case RecordType.Score:
                for (int k = 0; k < lastResultRecordUIData.scoreUIData.Length; k++) {
                    lastResultRecordUIData.scoreUIData[k].numbers.SetSpriteRendererMaskInteraciton(_maskInteraction);
                    lastResultRecordUIData.scoreUIData[k].addText_Count.maskInteraction = _maskInteraction;
                    lastResultRecordUIData.scoreUIData[k].addText_Piece.maskInteraction = _maskInteraction;
                    lastResultRecordUIData.scoreUIData[k].addText_pt.maskInteraction = _maskInteraction;
                }
                break;
            case RecordType.ScoreFourDigit:
                for (int j = 0; j < lastResultRecordUIData.scoreFourDigitUIData.Length; j++) {
                    lastResultRecordUIData.scoreFourDigitUIData[j].numbers.SetSpriteRendererMaskInteraciton(_maskInteraction);
                    lastResultRecordUIData.scoreFourDigitUIData[j].addText_pt.maskInteraction = _maskInteraction;
                    lastResultRecordUIData.scoreFourDigitUIData[j].addText_mL.maskInteraction = _maskInteraction;
                }
                break;
        }
    }
    public LastResultRecordUIData GetLastResultRecordUIData() {
        return lastResultRecordUIData;
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
