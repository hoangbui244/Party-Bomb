using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OrbitCalculation : MonoBehaviour {
    [SerializeField]
    [Header("放物線の高さ")]
    private float height;
    [SerializeField]
    [Header("放物線の移動速度")]
    private float speed;
    [SerializeField]
    [Header("ライン")]
    private LineRenderer line;
    [SerializeField]
    [Header("ロ\u30fcカル移動かどうか")]
    private bool isLocal;
    private Vector3 end;
    private List<Vector3> orbitPosList = new List<Vector3>();
    private Vector3 half;
    private Vector3 a;
    private Vector3 b;
    private float startTime;
    private float rate;
    private float diff;
    private void Update() {
        if (line != null) {
            line.positionCount = orbitPosList.Count;
            for (int i = 0; i < line.positionCount; i++) {
                line.SetPosition(i, orbitPosList[i]);
            }
        }
    }
    public void SetEndPoint(Vector3 _end) {
        end = _end;
    }
    public void StartOrbitMove(Action _callMathod = null) {
        orbitPosList.Clear();
        if (isLocal) {
            half = (end - base.transform.localPosition) * 0.5f + base.transform.localPosition;
            half.y += Vector3.up.y + height;
        } else {
            half = (end - base.transform.position) * 0.5f + base.transform.position;
            half.y += Vector3.up.y + height;
        }
        StartCoroutine(OrbitMoveProcess(_callMathod));
    }
    private IEnumerator OrbitMoveProcess(Action _callMethod = null) {
        startTime = Time.timeSinceLevelLoad;
        rate = 0f;
        if (speed == 0f) {
            if (isLocal) {
                base.transform.localPosition = end;
            } else {
                base.transform.position = end;
            }
        } else {
            while (!(rate >= 1f)) {
                diff = Time.timeSinceLevelLoad - startTime;
                rate = diff / (speed / 60f);
                if (isLocal) {
                    orbitPosList.Add(CalcLerpPoint(base.transform.localPosition, half, end, rate));
                    base.transform.localPosition = CalcLerpPoint(base.transform.localPosition, half, end, rate);
                } else {
                    orbitPosList.Add(CalcLerpPoint(base.transform.position, half, end, rate));
                    base.transform.position = CalcLerpPoint(base.transform.position, half, end, rate);
                }
                yield return null;
            }
        }
        base.transform.position = end;
        _callMethod?.Invoke();
    }
    private Vector3 CalcLerpPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        a = Vector3.Lerp(p0, p1, t);
        b = Vector3.Lerp(p1, p2, t);
        return Vector3.Lerp(a, b, t);
    }
}
