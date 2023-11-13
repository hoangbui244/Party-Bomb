using UnityEngine;
public class SimpleOcclusionCulling : MonoBehaviour {
    [SerializeField]
    [Header("描画判定をするMeshRenderer")]
    private MeshRenderer targetMeshRer;
    [SerializeField]
    [Header("描画判定をするSpriteRenderer")]
    private SpriteRenderer targetSpRer;
    [SerializeField]
    [Header("描画時は[表示] 描画範囲外時は[非表示]にする")]
    private bool IS_ENABLE;
    [SerializeField]
    [Header("描画範囲外時に[破棄]する")]
    private bool IS_DESTROY;
    private void OnBecameVisible() {
        if (IS_ENABLE && targetMeshRer != null) {
            targetMeshRer.material.SetAlpha(1f);
        }
    }
    private void OnBecameInvisible() {
        if (IS_ENABLE && targetMeshRer != null) {
            targetMeshRer.material.SetAlpha(0f);
        }
        if (IS_DESTROY) {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }
}
