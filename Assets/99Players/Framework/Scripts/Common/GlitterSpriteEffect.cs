using System;
using UnityEngine;
using UnityEngine.U2D;
public class GlitterSpriteEffect : MonoBehaviour {
    [SerializeField]
    [Header("キラキラ画像")]
    private SpriteRenderer glitterSprite;
    [SerializeField]
    [Header("キラキラ描画範囲")]
    private float glitterAreaRadius = 1f;
    [SerializeField]
    [Header("一度に生成するキラキラの数")]
    private int glitterEffectNum = 1;
    [SerializeField]
    [Header("キラキラの生成間隔")]
    private float glitterCreateInterval = 0.5f;
    [SerializeField]
    [Header("キラキラ演出時間")]
    private float animationTime = 0.5f;
    private int currentCreateGlitterNum;
    private float currentCreateGlitterInterval;
    private SpriteAtlas spriteAtlasData;
    private bool isGlitterStartAnimationFlg;
    private void Start() {
        spriteAtlasData = SingletonCustom<SAManager>.Instance.GetSA(SAType.Result);
    }
    private void Update() {
        if (isGlitterStartAnimationFlg && currentCreateGlitterNum < glitterEffectNum) {
            if (currentCreateGlitterInterval < 0f) {
                currentCreateGlitterInterval = glitterCreateInterval;
                CreateGlitter();
            } else {
                currentCreateGlitterInterval -= Time.deltaTime;
            }
        }
    }
    public void StartGlitterAnimation() {
        isGlitterStartAnimationFlg = true;
    }
    public void StopGlitterAnimation() {
        isGlitterStartAnimationFlg = false;
    }
    private void CreateGlitter() {
        SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate(glitterSprite, Vector3.zero, Quaternion.identity, base.transform);
        spriteRenderer.transform.localPosition = CircleHorizon(glitterAreaRadius);
        spriteRenderer.gameObject.SetActive(value: true);
        if (UnityEngine.Random.Range(0, 2) == 0) {
            spriteRenderer.sprite = spriteAtlasData.GetSprite("glitter_00");
        } else {
            spriteRenderer.sprite = spriteAtlasData.GetSprite("glitter_01");
        }
        currentCreateGlitterNum++;
        AnimationGlitter(spriteRenderer.gameObject);
    }
    private void AnimationGlitter(GameObject _object) {
        _object.transform.localScale = Vector3.zero;
        LeanTween.scale(_object, new Vector3(1.2f, 1.2f, 1.2f), 0.5f);
        LeanTween.delayedCall(animationTime, (Action)delegate {
            LeanTween.scale(_object, Vector3.zero, 0.5f).setOnComplete((Action)delegate {
                DestroyGlitter(_object);
            });
        });
        LeanTween.rotateAround(_object, Vector3.forward, 360f, 2f);
    }
    private void DestroyGlitter(GameObject _object) {
        LeanTween.cancel(_object);
        UnityEngine.Object.Destroy(_object);
        currentCreateGlitterNum--;
    }
    private Vector3 CircleHorizon(float radius) {
        float f = (float)UnityEngine.Random.Range(0, 360) * ((float)Math.PI / 180f);
        float x = Mathf.Cos(f) * radius;
        float y = Mathf.Sin(f) * radius;
        return new Vector3(x, y, 0f);
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(base.transform.position, glitterAreaRadius);
    }
}
