using UnityEngine;
using UnityEngine.Video;
namespace Satbox.ModeSelet {
    public class DisplayManager : MonoBehaviour {
        [SerializeField]
        private VideoPlayer[] players = new VideoPlayer[0];
        [SerializeField]
        private Renderer[] renderers = new Renderer[0];
        [SerializeField]
        [Header("フォ\u30fcカス切り替え時先頭から再生")]
        private bool resumeOnBeginning = true;
        public void SetActive(bool active) {
            if (active) {
                Activation();
            } else {
                Inactivation();
            }
        }
        private void Activation() {
            VideoPlayer[] array = players;
            foreach (VideoPlayer videoPlayer in array) {
                if (!(videoPlayer == null)) {
                    videoPlayer.Play();
                }
            }
            Renderer[] array2 = renderers;
            foreach (Renderer renderer in array2) {
                if (!(renderer == null)) {
                    renderer.enabled = true;
                }
            }
        }
        private void Inactivation() {
            VideoPlayer[] array = players;
            foreach (VideoPlayer videoPlayer in array) {
                if (!(videoPlayer == null)) {
                    if (resumeOnBeginning) {
                        videoPlayer.time = 0.0;
                    }
                    videoPlayer.Pause();
                }
            }
            Renderer[] array2 = renderers;
            foreach (Renderer renderer in array2) {
                if (!(renderer == null)) {
                    renderer.enabled = false;
                }
            }
        }
    }
}
