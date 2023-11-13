using UnityEngine;
[DisallowMultipleComponent]
public class SatMotionPlay : MonoBehaviour {
    private Skijump_Character chara;
    private SatSimpleAnimation nowAnim;
    private int poseNum;
    private int repeatNum;
    private int poseCount;
    public int mask;
    private KeyPoseData[] keys;
    private float waitTimer;
    public bool IsPlay {
        get;
        private set;
    }
    public bool IsPause {
        get;
        private set;
    }
    public bool IsLoopEnd {
        get;
        private set;
    }
    public bool IsNextMotion {
        get;
        private set;
    }
    private void SetCharaParams(Skijump_Character _chara) {
        chara = _chara;
        nowAnim = chara.gameObject.GetComponent<SatSimpleAnimation>();
        if (nowAnim == null) {
            nowAnim = SatSimpleAnimation.Add(chara);
        }
    }
    public static SatMotionPlay Add(Skijump_Character _chara) {
        SatMotionPlay satMotionPlay = _chara.gameObject.AddComponent<SatMotionPlay>();
        SatSimpleAnimation component = _chara.gameObject.GetComponent<SatSimpleAnimation>();
        if ((bool)component) {
            UnityEngine.Object.DestroyImmediate(component);
        }
        satMotionPlay.SetCharaParams(_chara);
        return satMotionPlay;
    }
    public void Play() {
        IsPlay = true;
        poseNum = 0;
    }
    public void Pause() {
        IsPause = true;
    }
    public void Resume() {
        IsPause = false;
    }
    public void UpdateMethod(float _animPlaySpeed = 1f) {
        if (!IsPlay || IsPause) {
            return;
        }
        nowAnim.UpdateMethod(_animPlaySpeed);
        IsLoopEnd = false;
        IsNextMotion = false;
        if (nowAnim.IsPlay) {
            return;
        }
        if (waitTimer < keys[poseNum].time) {
            waitTimer += Time.deltaTime * _animPlaySpeed;
            return;
        }
        waitTimer = 0f;
        if (TryGoNextPose()) {
            IsNextMotion = true;
        } else if (TryRepeat()) {
            IsLoopEnd = true;
        } else {
            IsPlay = false;
        }
    }
    private bool TryGoNextPose() {
        poseNum++;
        if (poseNum < poseCount) {
            nowAnim.SetPose(keys[poseNum].pose, keys[poseNum].fadeTime).SetMask(mask).Play();
            return true;
        }
        return false;
    }
    private bool TryRepeat() {
        if (repeatNum > 0) {
            repeatNum--;
            poseNum = 0;
            nowAnim.SetPose(keys[poseNum].pose, keys[poseNum].fadeTime).SetMask(mask).Play();
            return true;
        }
        if (repeatNum == -1) {
            poseNum = 0;
            nowAnim.SetPose(keys[poseNum].pose, keys[poseNum].fadeTime).SetMask(mask).Play();
            return true;
        }
        return false;
    }
    public void Clear() {
        StopImmediate();
        keys = null;
        poseNum = 0;
        repeatNum = 0;
        poseCount = 0;
    }
    public SatMotionPlay SetMotion(SatMotion _motion) {
        keys = _motion.GetKeys();
        poseCount = keys.Length;
        poseNum = 0;
        return this;
    }
    public SatMotionPlay SetLoop(int _loopCount) {
        repeatNum = _loopCount;
        return this;
    }
    public SatMotionPlay SetMask(int _mask) {
        mask = _mask;
        return this;
    }
    public void StopAtNowLoop() {
        repeatNum = 0;
    }
    public void StopAtNextPose() {
        nowAnim.Stop();
        poseNum = poseCount;
        repeatNum = 0;
    }
    public void StopImmediate() {
        nowAnim.StopImmediate();
        IsPlay = false;
    }
    public void Remove() {
        UnityEngine.Object.Destroy(nowAnim);
        nowAnim = null;
        UnityEngine.Object.Destroy(this);
    }
    public void RemoveImmediate() {
        UnityEngine.Object.DestroyImmediate(nowAnim);
        nowAnim = null;
        UnityEngine.Object.DestroyImmediate(this);
    }
}
