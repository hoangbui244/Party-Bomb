using UnityEngine;
public class SatMotion : ScriptableObject {
    public const int MAX_KEYS = 16;
    public int poseId;
    public string animName;
    [SerializeField]
    private KeyPoseData[] keys;
    protected float keysPlayTime;
    public int KeyCount {
        get;
        private set;
    }
    public KeyPoseData[] GetKeys() {
        return keys;
    }
    public float GetMotionPlayTime() {
        keysPlayTime = 0f;
        for (int i = 0; i < keys.Length; i++) {
            keysPlayTime += keys[i].fadeTime;
            keysPlayTime += keys[i].time;
        }
        return keysPlayTime;
    }
}
