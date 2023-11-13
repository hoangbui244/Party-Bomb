using UnityEngine;
public class AlpineSkiing_Props_Flag_Anime : MonoBehaviour {
    [SerializeField]
    [Header("傾ける角度")]
    private Vector3 angle;
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            LeanTween.cancel(base.gameObject);
            LeanTween.rotateLocal(base.gameObject, angle, 0.2f).setEase(LeanTweenType.easeOutQuad);
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            LeanTween.cancel(base.gameObject);
            LeanTween.rotateLocal(base.gameObject, new Vector3(0f, 0f, 0f), 0.2f).setEase(LeanTweenType.easeOutQuad);
        }
    }
}
