using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Utility {
    public class ParticleSystemDestroyer : MonoBehaviour {
        public float minDuration = 8f;

        public float maxDuration = 10f;

        private float m_MaxLifetime;

        private bool m_EarlyStop;

        private IEnumerator Start() {
            ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();
            ParticleSystem[] array = systems;
            foreach (ParticleSystem particleSystem in array) {
                m_MaxLifetime = Mathf.Max(particleSystem.main.startLifetime.constant, m_MaxLifetime);
            }
            float stopTime = Time.time + UnityEngine.Random.Range(minDuration, maxDuration);
            while (Time.time < stopTime || m_EarlyStop) {
                yield return null;
            }
            UnityEngine.Debug.Log("stopping " + base.name);
            array = systems;
            for (int i = 0; i < array.Length; i++) {
                //??array[i].emission.enabled = false;
            }
            BroadcastMessage("Extinguish", SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(m_MaxLifetime);
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public void Stop() {
            m_EarlyStop = true;
        }
    }
}
