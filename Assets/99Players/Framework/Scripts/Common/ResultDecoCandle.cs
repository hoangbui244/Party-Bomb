using UnityEngine;
public class ResultDecoCandle : MonoBehaviour {
    [SerializeField]
    [Header("画像")]
    private SpriteRenderer spTarget;
    private void OnEnable() {
        LeanTween.cancel(base.gameObject);
        LeanTween.value(base.gameObject, 1f, 0.75f, 3f).setEaseInOutSine().setLoopPingPong()
            .setOnUpdate(delegate (float _value) {
                spTarget.transform.SetLocalScale(_value, _value, 1f);
                spTarget.SetAlpha(0.75f + (1f - _value));
            });
    }
    private void OnDestroy() {
        LeanTween.cancel(base.gameObject);
    }
}
