using UnityEngine;
public class AutoDestroyParticleSystem : MonoBehaviour {
    private void Start() {
        ParticleSystem component = GetComponent<ParticleSystem>();
        UnityEngine.Object.Destroy(base.gameObject, component.duration);
    }
}
