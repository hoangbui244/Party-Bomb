using UnityEngine;
public class CloudMove : MonoBehaviour {
    [SerializeField]
    [Header("移動量(x,y)")]
    private Vector3 move;
    [SerializeField]
    [Header("移動時間(x,y)")]
    private Vector3 time;
    private Vector3 startLocalPos;
    public void Awake() {
        startLocalPos = base.transform.localPosition;
    }
    public void OnEnable() {
        base.transform.localPosition = startLocalPos;
        LeanTween.cancel(base.gameObject);
        LeanTween.moveLocalX(base.gameObject, startLocalPos.x + move.x, time.x).setEaseInOutSine().setLoopPingPong()
            .setDelay(UnityEngine.Random.Range(0f, 0.1f));
        LeanTween.moveLocalY(base.gameObject, startLocalPos.y + move.y, time.y).setEaseInOutSine().setLoopPingPong()
            .setDelay(UnityEngine.Random.Range(0f, 0.1f));
    }
    private void OnDestroy() {
        LeanTween.cancel(base.gameObject);
    }
}
