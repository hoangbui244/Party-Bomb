using UnityEngine;
public class MoveArrow : MonoBehaviour {
    [SerializeField]
    [Header("Y座標移動量")]
    private float moveY;
    [SerializeField]
    [Header("移動時間")]
    private float time;
    private void Start() {
        LeanTween.moveLocalY(base.gameObject, base.transform.localPosition.y + moveY, time).setLoopPingPong().setEaseInOutQuad();
    }
}
