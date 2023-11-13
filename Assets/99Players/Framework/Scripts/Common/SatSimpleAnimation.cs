using System;
using UnityEngine;
[DisallowMultipleComponent]
public class SatSimpleAnimation : MonoBehaviour {
    [Flags]
    public enum BodyPartsMask {
        HEAD = 0x1,
        BODY = 0x2,
        HIP = 0x4,
        SHOULDER_L = 0x8,
        SHOULDER_R = 0x10,
        ARM_L = 0x20,
        ARM_R = 0x40,
        LEG_L = 0x80,
        LEG_R = 0x100,
        NONE = 0x0,
        UPPER_BODY = 0x7B,
        LOWER_BODY = 0x184,
        ALL = 0x1FF
    }
    private Skijump_Character chara;
    private Transform[] transformCache;
    private SatSimplePose defPose;
    private SatSimplePose nextPose;
    private float fadeTime;
    private float fadeTimer;
    private float fadeTimerSlip;
    private bool pingpong;
    private bool playReverse;
    public int mask;
    public bool IsPlay {
        get;
        private set;
    }
    public bool IsPause {
        get;
        private set;
    }
    public void UpdateMethod(float _animPlaySpeed = 1f) {
        if (!IsPlay || IsPause) {
            return;
        }
        if (fadeTimerSlip != 0f) {
            fadeTimer += fadeTimerSlip * _animPlaySpeed;
            fadeTimerSlip = 0f;
        }
        if (playReverse) {
            fadeTimer -= Time.deltaTime * _animPlaySpeed;
        } else {
            fadeTimer += Time.deltaTime * _animPlaySpeed;
        }
        ChangePoseAtTime(defPose, nextPose, fadeTimer / fadeTime);
        if (fadeTimer < 0f || fadeTimer > fadeTime) {
            if (fadeTimer > fadeTime) {
                fadeTimerSlip = (fadeTimer - fadeTime) / _animPlaySpeed;
            }
            if (pingpong) {
                playReverse = !playReverse;
            } else {
                IsPlay = false;
            }
        }
    }
    public static SatSimpleAnimation Add(Skijump_Character chara) {
        SatSimpleAnimation satSimpleAnimation = chara.gameObject.AddComponent<SatSimpleAnimation>();
        satSimpleAnimation.SetCharaParams(chara);
        return satSimpleAnimation;
    }
    private void SetCharaParams(Skijump_Character _chara) {
        defPose = ScriptableObject.CreateInstance<SatSimplePose>();
        chara = _chara;
        transformCache = _chara.BodyPartsObj;
    }
    public SatSimpleAnimation SetPose(SatSimplePose _nextPose, float _fadeTime, bool _pingpong = false) {
        StopImmediate();
        nextPose = _nextPose;
        fadeTime = _fadeTime;
        pingpong = _pingpong;
        return this;
    }
    public SatSimpleAnimation SetMask(int _mask) {
        mask = _mask;
        return this;
    }
    public void Play() {
        IsPlay = true;
        fadeTimer = 0f;
        defPose.KeepTransformValues(transformCache);
        if (fadeTime < Mathf.Epsilon) {
            nextPose.ApplyValues(chara);
        }
    }
    public void Pause() {
        IsPause = true;
    }
    public void Resume() {
        IsPause = false;
    }
    public void Stop() {
        pingpong = false;
        fadeTimer = 0f;
    }
    public void StopImmediate() {
        IsPlay = false;
        Stop();
    }
    private void ChangePoseAtTime(SatSimplePose fromPose, SatSimplePose toPose, float _normalizedTime) {
        int num = mask;
        for (int i = 0; i < toPose.GetPartsPositions().Length; i++) {
            if (num % 2 == 1) {
                num /= 2;
                continue;
            }
            num /= 2;
            transformCache[i].localPosition = Vector3.Lerp(fromPose.GetPartsPositions()[i], toPose.GetPartsPositions()[i], _normalizedTime);
            transformCache[i].localRotation = Quaternion.Lerp(fromPose.GetPartsRotations()[i], toPose.GetPartsRotations()[i], _normalizedTime);
            transformCache[i].localScale = Vector3.Lerp(fromPose.GetPartsScales()[i], toPose.GetPartsScales()[i], _normalizedTime);
        }
    }
    public void UpdateEditorPreviewMethod(float time) {
        defPose.KeepTransformValues(transformCache);
        ChangePoseAtTime(defPose, nextPose, time);
    }
}
