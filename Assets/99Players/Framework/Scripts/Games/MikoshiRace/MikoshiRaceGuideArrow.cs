using UnityEngine;
public class MikoshiRaceGuideArrow : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer[] sprites;
    [SerializeField]
    private ParticleSystem[] particles;
    private float alpha;
    private float angle;
    public void UpdateMethod() {
    }
    public void SetParticleColor(Color _color) {
        for (int i = 0; i < particles.Length; i++) {
            //??particles[i].main.startColor = _color;
            particles[i].Stop();
            particles[i].Play();
        }
    }
}
