using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_HitEffect : DecoratedMonoBehaviour {
    [SerializeField]
    [DisplayName("パ\u30fcティクル")]
    private ParticleSystem particleSystem;
    public bool Isplaying => particleSystem.isPlaying;
    public void Initialize() {
    }
    public void SetSize(float size) {
        //??particleSystem.main.startSize = size;
    }
    public void Play(Vector3 point) {
        base.transform.position = point;
        particleSystem.Play();
    }
}
