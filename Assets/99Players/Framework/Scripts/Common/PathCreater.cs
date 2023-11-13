using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Pixelplacement/PathCreater")]
public class PathCreater : MonoBehaviour {
    public Color pathColor = Color.cyan;
    public List<Vector3> nodes = new List<Vector3>
    {
        Vector3.zero,
        Vector3.zero
    };
    public bool pathVisible = true;
    public bool isLocal = true;
    private void OnDrawGizmosSelected() {
        if (base.gameObject.activeSelf && pathVisible && nodes.Count > 0) {
            if (isLocal) {
                DrawPath(nodes.ToArray(), base.transform.position, pathColor);
            } else {
                DrawPath(nodes.ToArray(), pathColor);
            }
        }
    }
    public Vector3 GetBezierCurvePos(float _time) {
        Vector3[] pts = PathControlPointGenerator(nodes.ToArray());
        if (isLocal) {
            return base.transform.position + Interp(pts, _time);
        }
        return Interp(pts, _time);
    }
    public Vector3[] GetPath() {
        return nodes.ToArray();
    }
    public Vector3[] GetPathReversed() {
        List<Vector3> range = nodes.GetRange(0, nodes.Count);
        range.Reverse();
        return range.ToArray();
    }
    private void DrawPath(Vector3[] path) {
        if (path.Length != 0) {
            DrawPathHelper(path, Color.white, "gizmos");
        }
    }
    private void DrawPath(Vector3[] path, Color color) {
        if (path.Length != 0) {
            DrawPathHelper(path, color, "gizmos");
        }
    }
    private void DrawPath(Vector3[] path, Vector3 worldPos, Color color) {
        if (path.Length != 0) {
            DrawPathHelper(path, worldPos, color, "gizmos");
        }
    }
    private void DrawPath(Transform[] path) {
        if (path.Length != 0) {
            Vector3[] array = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++) {
                array[i] = path[i].position;
            }
            DrawPathHelper(array, Color.white, "gizmos");
        }
    }
    private void DrawPath(Transform[] path, Color color) {
        if (path.Length != 0) {
            Vector3[] array = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++) {
                array[i] = path[i].position;
            }
            DrawPathHelper(array, color, "gizmos");
        }
    }
    private void DrawPathHelper(Vector3[] path, Color color, string method) {
        Vector3[] pts = PathControlPointGenerator(path);
        Vector3 to = Interp(pts, 0f);
        Gizmos.color = color;
        int num = path.Length * 20;
        for (int i = 1; i <= num; i++) {
            float t = (float)i / (float)num;
            Vector3 vector = Interp(pts, t);
            if (method == "gizmos") {
                Gizmos.DrawLine(vector, to);
            } else if (method == "handles") {
                UnityEngine.Debug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
            }
            to = vector;
        }
    }
    private void DrawPathHelper(Vector3[] path, Vector3 worldPos, Color color, string method) {
        Vector3[] pts = PathControlPointGenerator(path);
        Vector3 a = Interp(pts, 0f);
        Gizmos.color = color;
        int num = path.Length * 20;
        for (int i = 1; i <= num; i++) {
            float t = (float)i / (float)num;
            Vector3 vector = Interp(pts, t);
            if (method == "gizmos") {
                Gizmos.DrawLine(vector + worldPos, a + worldPos);
            } else if (method == "handles") {
                UnityEngine.Debug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
            }
            a = vector;
        }
    }
    private Vector3 Interp(Vector3[] pts, float _t) {
        int num = pts.Length - 3;
        int num2 = Mathf.Min(Mathf.FloorToInt(_t * (float)num), num - 1);
        float num3 = _t * (float)num - (float)num2;
        Vector3 a = pts[num2];
        Vector3 a2 = pts[num2 + 1];
        Vector3 vector = pts[num2 + 2];
        Vector3 b = pts[num2 + 3];
        return 0.5f * ((-a + 3f * a2 - 3f * vector + b) * (num3 * num3 * num3) + (2f * a - 5f * a2 + 4f * vector - b) * (num3 * num3) + (-a + vector) * num3 + 2f * a2);
    }
    private Vector3[] PathControlPointGenerator(Vector3[] path) {
        int num = 2;
        Vector3[] array = new Vector3[path.Length + num];
        Array.Copy(path, 0, array, 1, path.Length);
        array[0] = array[1] + (array[1] - array[2]);
        array[array.Length - 1] = array[array.Length - 2] + (array[array.Length - 2] - array[array.Length - 3]);
        if (array[1] == array[array.Length - 2]) {
            Vector3[] array2 = new Vector3[array.Length];
            Array.Copy(array, array2, array.Length);
            array2[0] = array2[array2.Length - 3];
            array2[array2.Length - 1] = array2[2];
            array = new Vector3[array2.Length];
            Array.Copy(array2, array, array2.Length);
        }
        return array;
    }
}
