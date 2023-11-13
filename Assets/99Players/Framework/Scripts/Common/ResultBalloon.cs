using UnityEngine;
public class ResultBalloon : MonoBehaviour {
    [SerializeField]
    [Header("風船を管理するマネ\u30fcジャ\u30fc")]
    private ResultBalloonManager resultBalloonManager;
    [SerializeField]
    [Header("SpriteRenderer専用画像")]
    private SpriteRenderer spriteRendererSprite;
    [SerializeField]
    [Header("移動速度")]
    private float moveSpeed;
    [SerializeField]
    [Header("左右反転する時間")]
    private float FLIP_HORIZONTAL_TIME;
    private float HIDE_HEIGHT = 1200f;
    private float HIDE_LEFT_SIDE = -40f;
    private float changeFlipHorizontalTime;
    private int flipHorizontalCount;
    private void Start() {
        moveSpeed = UnityEngine.Random.Range(100f, 250f);
    }
    private void Update() {
        if (base.transform.position.x <= HIDE_LEFT_SIDE || base.transform.position.y >= HIDE_HEIGHT) {
            base.gameObject.SetActive(value: false);
            flipHorizontalCount = 0;
            if (spriteRendererSprite != null) {
                spriteRendererSprite.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            resultBalloonManager.BalloonCreate(base.gameObject);
            return;
        }
        changeFlipHorizontalTime += Time.deltaTime;
        if (changeFlipHorizontalTime > FLIP_HORIZONTAL_TIME) {
            changeFlipHorizontalTime = 0f;
            flipHorizontalCount++;
            if (spriteRendererSprite != null) {
                spriteRendererSprite.transform.localScale = new Vector3((flipHorizontalCount % 2 == 0) ? 1f : (-1f), 1f, 1f);
            }
        }
        base.transform.localPosition += new Vector3(-1f * moveSpeed * Time.deltaTime, 1f * moveSpeed * Time.deltaTime, 0f);
    }
}
