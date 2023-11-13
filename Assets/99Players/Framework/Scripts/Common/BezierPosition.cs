using System;
using System.Collections.Generic;
using UnityEngine;
public class BezierPosition : MonoBehaviour {
    [Serializable]
    public struct Bezier {
        public Transform anchor;
        public bool isBezier;
        public int bezierPoint;
    }
    [SerializeField]
    [Header("ラインカラ\u30fc")]
    private Color gizmoLineColor = Color.white;
    [SerializeField]
    private Bezier[] arrayBezier;
    [SerializeField]
    [Header("ベジェ曲線時の補正")]
    private float bezierDiffValue = 0.25f;
    [NonReorderable]
    private List<Vector3> posList = new List<Vector3>();
    [SerializeField]
    [Header("ロ\u30fcカル座標フラグ")]
    private bool isLocal;
    [SerializeField]
    [Header("ロ\u30fcカル座標時、ロ\u30fcカル座標に変換するTarget")]
    private Transform localTarget;
    [SerializeField]
    [Header("posList逆順フラグ")]
    private bool isPosListReverse;
    [SerializeField]
    [Header("posList逆順時のランダム抽選フラグ")]
    private bool isPosListReverse_Random;
    private bool isReverse;
    [SerializeField]
    [Header("Gizmoを表示するかどううか")]
    private bool isGizmos;
    [SerializeField]
    [Header("Gizmo用の半径")]
    private float GizmoRadius = 0.5f;
    public void Init() {
        SetPosList();
        if (!isPosListReverse) {
            return;
        }
        if (isPosListReverse_Random) {
            if (UnityEngine.Random.Range(0, 2) == 0) {
                UnityEngine.Debug.Log("posListを逆順に設定");
                Vector3[] array = posList.ToArray();
                Array.Reverse((Array)array);
                posList = array.ToList();
                isReverse = true;
            }
        } else {
            UnityEngine.Debug.Log("posListを逆順に設定");
            Vector3[] array2 = posList.ToArray();
            Array.Reverse((Array)array2);
            posList = array2.ToList();
            isReverse = true;
        }
    }
    private void SetPosList() {
        Vector3 zero = Vector3.zero;
        Vector3 zero2 = Vector3.zero;
        Vector3 zero3 = Vector3.zero;
        for (int i = 0; i < arrayBezier.Length; i++) {
            zero3 = arrayBezier[i].anchor.position;
            float d;
            if (arrayBezier[i].isBezier) {
                zero = arrayBezier[i + 1].anchor.position;
                zero2 = arrayBezier[i - 1].anchor.position;
                d = (zero - zero2).magnitude * bezierDiffValue;
                Vector3 vector = (zero + zero2) / 2f;
                Vector3 a = zero3 - vector;
                Vector3 control = vector + a * d;
                for (int j = 1; j <= arrayBezier[i].bezierPoint; j++) {
                    float t = (float)j / (float)(arrayBezier[i].bezierPoint + 1);
                    Vector3 vector2 = BezierCurve(zero2, zero, control, t);
                    if (isLocal) {
                        vector2 = localTarget.InverseTransformPoint(vector2);
                    }
                    posList.Add(vector2);
                }
                continue;
            }
            if (i == 0 || (i > 0 && arrayBezier[i - 1].isBezier)) {
                if (isLocal) {
                    zero3 = localTarget.InverseTransformPoint(zero3);
                }
                posList.Add(zero3);
            }
            if (i == arrayBezier.Length - 1 || arrayBezier[i + 1].isBezier) {
                continue;
            }
            zero = arrayBezier[i + 1].anchor.position;
            Vector3 vector3 = zero - zero3;
            d = vector3.magnitude / (float)arrayBezier[i].bezierPoint;
            for (int k = 1; k <= arrayBezier[i].bezierPoint; k++) {
                Vector3 vector4 = zero3 + vector3.normalized * d * k;
                if (isLocal) {
                    vector4 = localTarget.InverseTransformPoint(vector4);
                }
                posList.Add(vector4);
            }
        }
    }
    private void SetPosList_Gizmo() {
        Vector3 zero = Vector3.zero;
        Vector3 zero2 = Vector3.zero;
        Vector3 zero3 = Vector3.zero;
        List<Vector3> list = new List<Vector3>();
        for (int i = 0; i < arrayBezier.Length; i++) {
            zero3 = arrayBezier[i].anchor.position;
            if (arrayBezier[i].isBezier) {
                zero = arrayBezier[i + 1].anchor.position;
                zero2 = arrayBezier[i - 1].anchor.position;
                float d = (zero - zero2).magnitude * bezierDiffValue;
                Vector3 vector = (zero + zero2) / 2f;
                Vector3 a = zero3 - vector;
                Vector3 control = vector + a * d;
                for (int j = 1; j <= arrayBezier[i].bezierPoint; j++) {
                    float t = (float)j / (float)(arrayBezier[i].bezierPoint + 1);
                    Vector3 item = BezierCurve(zero2, zero, control, t);
                    list.Add(item);
                }
                continue;
            }
            if (i == 0 || (i > 0 && arrayBezier[i - 1].isBezier)) {
                list.Add(zero3);
            }
            if (i != arrayBezier.Length - 1 && !arrayBezier[i + 1].isBezier) {
                zero = arrayBezier[i + 1].anchor.position;
                Vector3 vector2 = zero - zero3;
                float d = vector2.magnitude / (float)arrayBezier[i].bezierPoint;
                for (int k = 1; k <= arrayBezier[i].bezierPoint; k++) {
                    Vector3 item2 = zero3 + vector2.normalized * d * k;
                    list.Add(item2);
                }
            }
        }
        Gizmos.color = Color.yellow;
        for (int l = 0; l < list.Count; l++) {
            Gizmos.DrawWireSphere(list[l], GizmoRadius);
        }
        Gizmos.color = gizmoLineColor;
        for (int m = 0; m < list.Count - 1; m++) {
            Gizmos.DrawLine(list[m], list[m + 1]);
        }
    }
    private Vector3 BezierCurve(Vector3 start, Vector3 end, Vector3 control, float t) {
        Vector3 a = Vector3.Lerp(start, control, t);
        Vector3 b = Vector3.Lerp(control, end, t);
        return Vector3.Lerp(a, b, t);
    }
    public List<Vector3> GetPosList() {
        return posList;
    }
    public bool GetIsReverse() {
        return isReverse;
    }
    private void OnDrawGizmos() {
        if (isGizmos && arrayBezier != null && arrayBezier.Length != 0) {
            SetPosList_Gizmo();
        }
    }
}
