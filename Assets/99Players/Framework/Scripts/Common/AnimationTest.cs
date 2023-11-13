using UnityEngine;
public class AnimationTest : MonoBehaviour {
    [SerializeField]
    [Header("動かすキャラクタ\u30fc")]
    private Skijump_Character targetChara;
    [SerializeField]
    [Header("モ\u30fcションリストの番号")]
    public int motionIndex;
    [SerializeField]
    [Header("動かさないパ\u30fcツ")]
    private SatSimpleAnimation.BodyPartsMask mask;
    private SatMotionPlay motionPlayer;
    private SatSimpleAnimation animPlayer;
    public bool isAnim;
    [SerializeField]
    [Range(0f, 1f)]
    private float fadeTime;
    private void Start() {
        if (isAnim) {
            LoopAnimPlay();
        } else {
            MotionPlay();
        }
    }
    private void LoopAnimPlay() {
        if (!animPlayer) {
            animPlayer = SatSimpleAnimation.Add(targetChara);
        }
        animPlayer.SetPose(SingletonCustom<SatSimplePoseList>.Instance.GetPose(motionIndex), fadeTime, isAnim).SetMask((int)mask);
        animPlayer.Play();
    }
    private void MotionPlay() {
        if (!motionPlayer) {
            motionPlayer = SatMotionPlay.Add(targetChara);
        }
        motionPlayer.SetMotion(SingletonCustom<SatMotionList>.Instance.GetMotionData(motionIndex)).SetLoop(-1).SetMask((int)mask);
        motionPlayer.Play();
    }
    private void Update() {
        if (isAnim) {
            animPlayer.UpdateMethod();
        } else {
            motionPlayer.UpdateMethod();
        }
    }
}
