using UnityEngine;
public class SplitMakeEffect : MonoBehaviour {
    public ParticleSystem startEffect;
    public ParticleSystem startEffect2;
    public ParticleSystem startEffect3;
    public ParticleSystem starEffect;
    public ParticleSystem starEffect2;
    public ParticleSystem starEffect3;
    public void PlayStartEffect() {
        startEffect.Play();
        startEffect2.Play();
        startEffect3.Play();
    }
    public void PlayStarEffect() {
        starEffect.Play();
        starEffect2.Play();
        starEffect3.Play();
    }
    public void EndEffect() {
        UnityEngine.Object.Destroy(base.gameObject);
    }
    public void PlayVoiceSound() {
    }
    public void PlaySE1Sound() {
    }
    public void PlayAudienceSound() {
    }
    public void PlayApplauseSound() {
    }
}
