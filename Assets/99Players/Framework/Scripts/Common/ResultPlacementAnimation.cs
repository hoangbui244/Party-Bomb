using System.Collections;
using UnityEngine;
using UnityEngine.Extension;
public class ResultPlacementAnimation : MonoBehaviour {
    [SerializeField]
    [DisplayName("元の位置を保つ時間")]
    private float keepDuration = 2f;
    [SerializeField]
    [DisplayName("移動位置")]
    private Vector3 targetPosition;
    [SerializeField]
    [DisplayName("移動位置")]
    private Vector3 targetScale;
    [SerializeField]
    [DisplayName("移動時間")]
    private float moveDuration = 0.5f;
    public void Play() {
        StartCoroutine(Animation());
    }
    private IEnumerator Animation() {
        float elapsed2 = 0f;
        while (elapsed2 < keepDuration) {
            elapsed2 += Time.deltaTime;
            yield return null;
        }
        elapsed2 = 0f;
        Vector3 startPosition = base.transform.localPosition;
        Vector3 startScale = base.transform.localScale;
        while (elapsed2 < moveDuration) {
            elapsed2 += Time.deltaTime;
            float t = elapsed2 / moveDuration;
            base.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            base.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        base.transform.localPosition = targetPosition;
        base.transform.localScale = targetScale;
    }
}
