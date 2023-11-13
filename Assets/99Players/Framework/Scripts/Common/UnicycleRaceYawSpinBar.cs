using System;
using UnityEngine;
public class UnicycleRaceYawSpinBar : MonoBehaviour {
    [SerializeField]
    [Header("時計回り")]
    private bool isClockwise = true;
    [SerializeField]
    [Header("速度(１０秒に何回転)")]
    private float speed = 1f;
    private Rigidbody rigid;
    private float corr;
    private void Start() {
        rigid = GetComponent<Rigidbody>();
        corr = (float)Math.PI / 5f;
        if (!isClockwise) {
            corr *= -1f;
        }
    }
    private void FixedUpdate() {
        rigid.angularVelocity = new Vector3(0f, speed * corr, 0f);
    }
}
