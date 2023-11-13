using System;
using UnityEngine;
public class SimpleCamera_FitInFOV : MonoBehaviour {
    [SerializeField]
    [Header("視野に収めたいオブジェクト")]
    private Transform[] mFitInObjects;
    [SerializeField]
    [Header("中心点")]
    private Transform centerPos;
    [SerializeField]
    [Header("動かすカメラ")]
    private Camera usingCamera;
    [SerializeField]
    [Header("カメラの移動速度")]
    private float moveCameraSpeed = 1f;
    private bool isStopFitInFOV;
    private Vector3 centerPosAverage;
    private float radius;
    private float margin;
    private float distance;
    public float MoveCameraSpeed {
        set {
            moveCameraSpeed = value;
        }
    }
    public bool IsStopFitInFOW {
        get {
            return isStopFitInFOV;
        }
        set {
            isStopFitInFOV = value;
        }
    }
    private void Update() {
        if (!isStopFitInFOV) {
            centerPosAverage = new Vector3(0f, 0f, 0f);
            radius = 0f;
            Transform[] array = mFitInObjects;
            foreach (Transform transform in array) {
                centerPosAverage += transform.position;
            }
            centerPos.position = centerPosAverage / mFitInObjects.Length;
            array = mFitInObjects;
            foreach (Transform transform2 in array) {
                radius = Mathf.Max(radius, Vector3.Distance(centerPos.position, transform2.position));
            }
            distance = (radius + margin) / Mathf.Sin(usingCamera.fieldOfView * ((float)Math.PI / 180f));
            usingCamera.transform.localPosition = Vector3.MoveTowards(usingCamera.transform.localPosition, new Vector3(usingCamera.transform.localPosition.x, centerPos.localPosition.y, 0f - distance), Time.deltaTime * moveCameraSpeed);
        }
    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(centerPos.position, radius);
    }
}
