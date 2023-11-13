using UnityEngine;
public class PlayKingOpening : MonoBehaviour {
    [SerializeField]
    [Header("フェ\u30fcドスプライト")]
    private SpriteRenderer fade;
    public void OnEndAnimation() {
        SingletonCustom<Scene_PlayKingOpening>.Instance.OnEndAnimation();
    }
    public void FadeIn() {
        LeanTween.cancel(fade.gameObject);
        LeanTween.value(base.gameObject, 0f, 1f, 0.64f).setOnUpdate(delegate (float _value) {
            fade.SetAlpha(_value);
        });
    }
    public void FadeOut() {
        LeanTween.cancel(fade.gameObject);
        LeanTween.value(base.gameObject, 1f, 0f, 0.64f).setOnUpdate(delegate (float _value) {
            fade.SetAlpha(_value);
        });
    }
    public void Stop() {
        LeanTween.cancel(base.gameObject);
        LeanTween.cancel(fade.gameObject);
    }
    private void OnDestroy() {
        LeanTween.cancel(base.gameObject);
        LeanTween.cancel(fade.gameObject);
    }
}
