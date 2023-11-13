using UnityEngine;
namespace Satbox.PlayerSetting {
    public class CharacterManager : MonoBehaviour {
        [SerializeField]
        private Animator[] animators = new Animator[0];
        private static int ToIdle = Animator.StringToHash("ToIdle");
        private static int ToDance = Animator.StringToHash("ToDance");
        private static int Idle = Animator.StringToHash("Idle");
        private static int Dance = Animator.StringToHash("Dance");
        public void ResetAnimation(bool active) {
            Animator[] array = animators;
            foreach (Animator animator in array) {
                if (!(animator == null)) {
                    animator.Play(active ? Dance : Idle);
                }
            }
        }
        public void SetActive(bool active) {
            if (active) {
                Activation();
            } else {
                Inactivation();
            }
        }
        private void Activation() {
            Animator[] array = animators;
            foreach (Animator animator in array) {
                if (!(animator == null)) {
                    animator.SetTrigger(ToDance);
                    animator.ResetTrigger(ToIdle);
                }
            }
        }
        private void Inactivation() {
            Animator[] array = animators;
            foreach (Animator animator in array) {
                if (!(animator == null)) {
                    animator.SetTrigger(ToIdle);
                    animator.ResetTrigger(ToDance);
                }
            }
        }
    }
}
