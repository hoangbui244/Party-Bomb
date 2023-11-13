using System;
using UnityEngine;
[Serializable]
public struct KeyPoseData {
    [SerializeField]
    public SatSimplePose pose;
    [SerializeField]
    public float time;
    [SerializeField]
    public float fadeTime;
    private KeyPoseData(SatSimplePose _pose, float _time, float _fade) {
        pose = _pose;
        time = _time;
        fadeTime = _fade;
    }
}
