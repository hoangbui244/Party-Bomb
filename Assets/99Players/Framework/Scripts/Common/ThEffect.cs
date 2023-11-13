using UnityEngine;
public class ThEffect : MonoBehaviour {
    public enum KIND {
        th_4,
        th_5,
        th_6,
        th_7,
        th_8,
        th_9,
        th_10,
        th_11
    }
    public KIND effectKind;
    public ParticleSystem backEffect;
    public ParticleSystem backEffect2;
    public ParticleSystem boundEffect;
    public ParticleSystem hitEffect;
    public ParticleSystem hitEffect2;
    public void BackEffect() {
        backEffect.Play();
        backEffect2.Play();
    }
    public void BoundEffect() {
        boundEffect.Play();
    }
    public void HitEffect() {
        hitEffect.Play();
        hitEffect2.Play();
    }
    public void EndEffect() {
        UnityEngine.Object.Destroy(base.gameObject);
    }
    public void PlayVoiceSound() {
    }
    public void PlayAudienceSound() {
    }
    public void PlayApplauseSound() {
    }
}
