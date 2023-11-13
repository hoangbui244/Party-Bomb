using UnityEngine;
public class SlideEffect : MonoBehaviour {
    [SerializeField]
    [Header("再生用")]
    private ParticleSystem root;
    [SerializeField]
    [Header("パ\u30fcツ")]
    private ParticleSystem[] parts;
    private ParticleSystem.MainModule[] partsModule;
    private ParticleSystem.EmissionModule[] partsEmissionModule;
    private float[] emissionDef;
    public void Init() {
        partsModule = new ParticleSystem.MainModule[parts.Length];
        partsEmissionModule = new ParticleSystem.EmissionModule[parts.Length];
        emissionDef = new float[parts.Length];
        for (int i = 0; i < parts.Length; i++) {
            partsModule[i] = parts[i].main;
            partsEmissionModule[i] = parts[i].emission;
            emissionDef[i] = partsEmissionModule[i].rateOverTime.constant;
        }
    }
    public void Play() {
        for (int i = 0; i < partsModule.Length; i++) {
            partsModule[i].loop = true;
        }
        for (int j = 0; j < parts.Length; j++) {
            partsEmissionModule[j].rateOverTime = new ParticleSystem.MinMaxCurve(emissionDef[j]);
        }
        root.Play();
    }
    public void Stop() {
        for (int i = 0; i < partsModule.Length; i++) {
            partsModule[i].loop = false;
        }
    }
    public void SetEmission(float _per) {
        _per = Mathf.Clamp(_per, 0f, 1f);
        for (int i = 0; i < parts.Length; i++) {
            partsEmissionModule[i].rateOverTime = new ParticleSystem.MinMaxCurve(emissionDef[i] * _per);
        }
    }
    public void Pause() {
        for (int i = 0; i < parts.Length; i++) {
            parts[i].Pause();
        }
    }
    public void Resume() {
        for (int i = 0; i < parts.Length; i++) {
            parts[i].Play();
        }
    }
}
