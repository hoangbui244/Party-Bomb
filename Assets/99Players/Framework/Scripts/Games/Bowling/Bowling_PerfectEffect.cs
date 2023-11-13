using System.Collections;
using UnityEngine;
public class Bowling_PerfectEffect : ThrowResultEffect {
    [SerializeField]
    [Header("ライトエフェクト１")]
    private ParticleSystem lightEffect;
    [SerializeField]
    [Header("ライトエフェクト２")]
    private ParticleSystem lightEffect2;
    [SerializeField]
    [Header("ライトエフェクト３")]
    private ParticleSystem lightEffect3;
    [SerializeField]
    [Header("ライトエフェクト４")]
    private ParticleSystem lightEffect4;
    [SerializeField]
    [Header("文字エフェクト")]
    private ParticleSystem[] textEffect;
    [SerializeField]
    [Header("文字アンカ\u30fc")]
    private GameObject textAnchor;
    [SerializeField]
    [Header("文字リスト")]
    private SpriteRenderer[] textList;
    [SerializeField]
    [Header("出現時間")]
    private float APPEAR_TIME = 1f;
    [SerializeField]
    [Header("拡大スケ\u30fcル")]
    private float EXPAND_SCALE = 1.5f;
    [SerializeField]
    [Header("拡大時間")]
    private float EXPAND_TIME = 1f;
    [SerializeField]
    [Header("拡大インタ\u30fcバル時間")]
    private float EXPAND_INTERVAL = 0.2f;
    [SerializeField]
    [Header("待ち時間")]
    private float WAIT_TIME = 0.5f;
    [SerializeField]
    [Header("全体拡大スケ\u30fcル")]
    private float ALL_EXPAND_SCALE = 1.5f;
    [SerializeField]
    [Header("全体拡大時間")]
    private float ALL_EXPAND_TIME = 1f;
    public override void Init() {
        base.Init();
        if (base.enabled) {
            for (int i = 0; i < textEffect.Length; i++) {
                //??textEffect[i].main.startDelay = EXPAND_INTERVAL * (float)i;
            }
            for (int j = 0; j < textList.Length; j++) {
                textList[j].SetAlpha(0f);
            }
            StartCoroutine(_TextAnimation());
        }
    }
    private IEnumerator _TextAnimation() {
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_bowling_perfect");
        lightEffect.Play();
        LeanTween.rotateZ(textAnchor, -1080f, APPEAR_TIME);
        for (int i = 0; i < textList.Length; i++) {
            LeanTween.color(textList[i].gameObject, new Color(1f, 1f, 1f, 1f), 0.25f);
        }
        yield return new WaitForSeconds(APPEAR_TIME);
        for (int j = 0; j < textEffect.Length; j++) {
            textEffect[j].Play();
        }
        for (int k = 0; k < textList.Length; k++) {
            LeanTween.scale(textList[k].gameObject, new Vector3(EXPAND_SCALE, EXPAND_SCALE, 1f), EXPAND_TIME * 0.5f).setDelay(EXPAND_INTERVAL * (float)k);
        }
        for (int l = 0; l < textList.Length; l++) {
            LeanTween.scale(textList[l].gameObject, Vector3.one, EXPAND_TIME * 0.5f).setDelay(EXPAND_TIME * 0.5f + EXPAND_INTERVAL * (float)l);
        }
        yield return new WaitForSeconds(EXPAND_TIME + EXPAND_INTERVAL + WAIT_TIME);
        LeanTween.scale(textAnchor, new Vector3(ALL_EXPAND_SCALE, ALL_EXPAND_SCALE, 1f), ALL_EXPAND_TIME * 0.5f);
        lightEffect2.Play();
        lightEffect3.Play();
        lightEffect4.Play();
        yield return new WaitForSeconds(ALL_EXPAND_TIME * 0.5f);
        LeanTween.scale(textAnchor, Vector3.one, ALL_EXPAND_TIME * 0.5f);
        for (int m = 0; m < textList.Length; m++) {
            LeanTween.color(textList[m].gameObject, new Color(1f, 1f, 1f, 0f), ALL_EXPAND_TIME * 0.5f);
        }
    }
}
