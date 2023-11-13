using System;
using UnityEngine;
public class Common_ItemShakeMove : MonoBehaviour {
    public enum Axis {
        X,
        Y,
        Z
    }
    private const float WAVE_ANGLE_SPEED_BASE = 90f;
    [SerializeField]
    [Header("揺れる方向")]
    private Axis shakeAxis;
    [SerializeField]
    [Header("波のように揺れる幅の距離")]
    private float waveShakeMoveDis = 0.2f;
    [SerializeField]
    [Header("波のように揺れる速度")]
    private float waveShakeSpeed = 1f;
    [SerializeField]
    [Header("小刻みな揺れ幅の距離")]
    private float smallShakeMoveDis = 0.1f;
    [SerializeField]
    [Header("小刻みな揺れの速度")]
    private float smallShakeSpeed = 1f;
    private float waveShakeAngle;
    private Vector2 smallShakePerlinVec;
    private void Start() {
        smallShakePerlinVec = new Vector2(UnityEngine.Random.Range(-10000f, 10000f), UnityEngine.Random.Range(-10000f, 10000f));
    }
    private void Update() {
        waveShakeAngle += 90f * waveShakeSpeed * Time.deltaTime;
        float num = waveShakeMoveDis * Mathf.Sin(waveShakeAngle * ((float)Math.PI / 180f));
        smallShakePerlinVec.x += smallShakeSpeed * Time.deltaTime;
        num += Mathf.PerlinNoise(smallShakePerlinVec.x, smallShakePerlinVec.y) * smallShakeMoveDis;
        switch (shakeAxis) {
            case Axis.X:
                base.transform.SetLocalPositionX(num);
                break;
            case Axis.Y:
                base.transform.SetLocalPositionY(num);
                break;
            case Axis.Z:
                base.transform.SetLocalPositionZ(num);
                break;
        }
    }
}
