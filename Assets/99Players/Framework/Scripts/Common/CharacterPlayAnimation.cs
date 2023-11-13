using UnityEngine;
public class CharacterPlayAnimation : MonoBehaviour {
    public enum MotionType {
        NONE,
        SLIDE,
        JUMP,
        LANDING,
        SIT,
        LANDING_MISS_0,
        LANDING_MISS_1,
        LANDING_MISS_2,
        LANDING_MISS_3,
        LANDING_FALL
    }
    private SatSimpleAnimation satAnimation;
    private MotionType motionType;
    private Skijump_Character chara;
    private Transform charaRoot;
    public void InitMethod(Transform _obj, Skijump_Character _characterScript) {
        charaRoot = _obj;
        chara = _characterScript;
        satAnimation = SatSimpleAnimation.Add(chara);
    }
    public void UpdateMethod() {
        satAnimation.UpdateMethod();
    }
    public void SetMotion(MotionType _type, float _fadeTime = 0f) {
        satAnimation.SetPose(SingletonCustom<SatSimplePoseList>.Instance.GetPose((int)_type), _fadeTime);
        satAnimation.Play();
    }
}
