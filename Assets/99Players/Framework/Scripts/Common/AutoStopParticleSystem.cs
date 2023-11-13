using System.Collections;
using UnityEngine;
public class AutoStopParticleSystem : MonoBehaviour {
    private ParticleSystem particleSystem;
    private void Start() {
        particleSystem = GetComponent<ParticleSystem>();
        StartCoroutine(Stop());
    }
    private IEnumerator Stop() {
        yield return new WaitForSeconds(particleSystem.duration - 0.2f);
        particleSystem.Pause();
    }
}
