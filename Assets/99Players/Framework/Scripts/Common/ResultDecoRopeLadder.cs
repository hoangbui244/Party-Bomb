using UnityEngine;
public class ResultDecoRopeLadder : MonoBehaviour {
    public enum State {
        STANDBY,
        MOVE_TOP,
        MOVE_BOTTOM
    }
    [SerializeField]
    [Header("ニンジャ")]
    private SpriteRenderer spNinja;
    [SerializeField]
    [Header("アニメ差分")]
    private Sprite[] arrayDiffAnim;
    [SerializeField]
    [Header("上位置アンカ\u30fc")]
    private Transform anchorTop;
    [SerializeField]
    [Header("下位置アンカ\u30fc")]
    private Transform anchorBottom;
    [SerializeField]
    [Header("移動速度")]
    private float moveSpeed = 1f;
    [SerializeField]
    [Header("アニメ速度")]
    private float animSpeed = 1f;
    private float animTime;
    private int animIdx;
    private State currentState;
    private float stateTime;
    private void Start() {
        stateTime = UnityEngine.Random.Range(1f, 1.25f);
        animTime = animSpeed;
    }
    private void Update() {
        switch (currentState) {
            case State.STANDBY:
                stateTime -= Time.deltaTime;
                if (stateTime <= 0f) {
                    if (spNinja.transform.position.y == anchorTop.position.y) {
                        currentState = State.MOVE_BOTTOM;
                    } else if (spNinja.transform.position.y == anchorBottom.position.y) {
                        currentState = State.MOVE_TOP;
                    } else {
                        currentState = (CalcManager.GetHalfProbability() ? State.MOVE_TOP : State.MOVE_BOTTOM);
                    }
                    stateTime = UnityEngine.Random.Range(2.5f, 5.5f);
                    animTime = 0f;
                }
                break;
            case State.MOVE_TOP:
                stateTime -= Time.deltaTime;
                spNinja.transform.AddLocalPositionY(moveSpeed * Time.deltaTime);
                UpdateAnim();
                if (spNinja.transform.position.y > anchorTop.position.y) {
                    spNinja.transform.SetPositionY(anchorTop.position.y);
                    stateTime = 0f;
                }
                if (stateTime <= 0f) {
                    currentState = State.STANDBY;
                    stateTime = UnityEngine.Random.Range(1f, 2f);
                }
                break;
            case State.MOVE_BOTTOM:
                stateTime -= Time.deltaTime;
                spNinja.transform.AddLocalPositionY((0f - moveSpeed) * Time.deltaTime);
                UpdateAnim();
                if (spNinja.transform.position.y < anchorBottom.position.y) {
                    spNinja.transform.SetPositionY(anchorBottom.position.y);
                    stateTime = 0f;
                }
                if (stateTime <= 0f) {
                    currentState = State.STANDBY;
                    stateTime = UnityEngine.Random.Range(1f, 2f);
                }
                break;
        }
    }
    private void UpdateAnim() {
        animTime -= Time.deltaTime;
        if (animTime <= 0f) {
            animIdx = (animIdx + 1) % arrayDiffAnim.Length;
            spNinja.sprite = arrayDiffAnim[animIdx];
            animTime = animSpeed;
        }
    }
}
