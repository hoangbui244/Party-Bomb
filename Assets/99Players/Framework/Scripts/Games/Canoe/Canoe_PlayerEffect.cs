using System;
using UnityEngine;
public class Canoe_PlayerEffect : MonoBehaviour {
    [Serializable]
    private struct EffectParam {
        public ParticleSystem effect;
        [HideInInspector]
        public float originStartLifeTime;
        [HideInInspector]
        public float originStartSize;
        [HideInInspector]
        public float originRateOverTime;
    }
    [SerializeField]
    [Header("移動エフェクトのル\u30fcト")]
    private Transform moveEffectRoot;
    [SerializeField]
    [Header("移動エフェクト")]
    private EffectParam[] arrayMoveEffect;
    [SerializeField]
    [Header("加速エフェクト")]
    private ParticleSystem accelerationEffect;
    [SerializeField]
    [Header("パドルを漕いだ時のエフェクト")]
    private ParticleSystem[] arrayPaddleEffect;
    [SerializeField]
    [Header("着地時のエフェクト")]
    private ParticleSystem landingEffect;
    [SerializeField]
    [Header("スタミナ切れの汗エフェクト")]
    private ParticleSystem[] arraySweatEffect;
    public void Init() {
        for (int i = 0; i < arrayMoveEffect.Length; i++) {
            ParticleSystem.MainModule main = arrayMoveEffect[i].effect.main;
            arrayMoveEffect[i].originStartLifeTime = main.startLifetimeMultiplier;
            arrayMoveEffect[i].originStartSize = main.startSizeMultiplier;
            ParticleSystem.EmissionModule emission = arrayMoveEffect[i].effect.emission;
            arrayMoveEffect[i].originRateOverTime = emission.rateOverTimeMultiplier;
        }
    }
    public void PlayMoveEffect() {
        for (int i = 0; i < arrayMoveEffect.Length; i++) {
            if (!arrayMoveEffect[i].effect.isPlaying) {
                arrayMoveEffect[i].effect.Play();
            }
        }
    }
    public void StopMoveEffect() {
        for (int i = 0; i < arrayMoveEffect.Length; i++) {
            if (arrayMoveEffect[i].effect.isPlaying) {
                arrayMoveEffect[i].effect.Stop();
            }
        }
    }
    public void SetMoveEffectParam(float _speedLerp) {
        if (_speedLerp > 1f) {
            _speedLerp = 1f;
        }
        for (int i = 0; i < arrayMoveEffect.Length; i++) {
            ParticleSystem.MainModule main = arrayMoveEffect[i].effect.main;
            main.startLifetimeMultiplier = arrayMoveEffect[i].originStartLifeTime * _speedLerp;
            main.startSizeMultiplier = arrayMoveEffect[i].originStartSize * _speedLerp;
            //??arrayMoveEffect[i].effect.emission.rateOverTimeMultiplier = (int)(arrayMoveEffect[i].originRateOverTime * _speedLerp);
        }
    }
    public void SetMoveEffectDir(int _acceleMoveDir) {
        moveEffectRoot.SetLocalEulerAnglesY((_acceleMoveDir == 1) ? 0f : 180f);
    }
    public void SetMoveEffectActive(bool _isActive) {
        for (int i = 0; i < arrayMoveEffect.Length; i++) {
            arrayMoveEffect[i].effect.gameObject.SetActive(_isActive);
        }
    }
    public void PlayAccelerationEffect() {
        accelerationEffect.Play();
    }
    public void PlayPaddleRowingEffect(int _paddleIdx) {
        arrayPaddleEffect[_paddleIdx].Play();
    }
    public void SetPaddleEffectDir(int _acceleMoveDir) {
        for (int i = 0; i < arrayPaddleEffect.Length; i++) {
            arrayPaddleEffect[i].transform.SetLocalEulerAnglesY((_acceleMoveDir == 1) ? 180f : 0f);
        }
    }
    public void SetPaddleEffectActive(bool _isActive) {
        for (int i = 0; i < arrayPaddleEffect.Length; i++) {
            arrayPaddleEffect[i].gameObject.SetActive(_isActive);
        }
    }
    public void PlayLandingEffect() {
        landingEffect.Play();
    }
    public void SetLandingEffectPos(Vector3 _velocity, ContactPoint _contact) {
        landingEffect.transform.position = _contact.point;
        Quaternion rotation = Quaternion.LookRotation(_contact.normal);
        landingEffect.transform.rotation = rotation;
    }
    public void PlaySweatEffect() {
        for (int i = 0; i < arraySweatEffect.Length; i++) {
            arraySweatEffect[i].Play();
        }
    }
    public void StopSweatEffect() {
        for (int i = 0; i < arraySweatEffect.Length; i++) {
            arraySweatEffect[i].Stop();
        }
    }
}
