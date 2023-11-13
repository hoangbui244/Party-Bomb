using UnityEngine;
public class SimpleAnim : MonoBehaviour {
    [SerializeField]
    [Header("スプライト")]
    private SpriteRenderer sp;
    [SerializeField]
    [Header("スプライト配列")]
    private string[] spriteName;
    [SerializeField]
    [Header("開始座標")]
    private Vector3 startPos;
    [SerializeField]
    [Header("PingPong設定")]
    private bool isPingPong;
    [SerializeField]
    [Header("アニメ\u30fcション速度")]
    private float animSpeed = 0.1f;
    private int currentIdx;
    private float animTime;
    private bool isPlaybackDirection = true;
    public void Init() {
        currentIdx = UnityEngine.Random.Range(0, spriteName.Length);
        animTime = 0.1f;
        base.transform.localPosition = startPos;
    }
    public void Update() {
        if (animTime > 0f) {
            animTime -= Time.deltaTime;
            return;
        }
        if (isPingPong) {
            if (isPlaybackDirection) {
                if (currentIdx == spriteName.Length - 1) {
                    currentIdx--;
                    isPlaybackDirection = false;
                } else {
                    currentIdx++;
                }
            } else if (currentIdx == 0) {
                currentIdx++;
                isPlaybackDirection = true;
            } else {
                currentIdx--;
            }
        } else {
            currentIdx = (currentIdx + 1) % spriteName.Length;
        }
        animTime = animSpeed;
    }
    public void SetIdx(int _idx, float _addTime) {
        currentIdx = _idx;
        animTime = 0.1f + _addTime;
    }
}
