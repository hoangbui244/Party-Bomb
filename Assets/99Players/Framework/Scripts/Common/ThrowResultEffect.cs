using UnityEngine;
public class ThrowResultEffect : MonoBehaviour {
    [SerializeField]
    [Header("エフェクトの親アンカ\u30fc")]
    private GameObject EffectRootAnchor;
    [SerializeField]
    [Header("エフェクト時間")]
    protected float EFFECT_TIME = 4f;
    private void Awake() {
        EffectRootAnchor.SetActive(value: false);
    }
    private void OnDisable() {
        CancelInvoke("EffectDestroy");
    }
    public void StartEffect() {
        EffectRootAnchor.SetActive(value: true);
        Init();
        Invoke("EffectDestroy", EFFECT_TIME);
    }
    private void EffectDestroy() {
        EffectRootAnchor.SetActive(value: false);
    }
    public virtual void Init() {
    }
    public float GetEffectTime() {
        return EFFECT_TIME;
    }
}
