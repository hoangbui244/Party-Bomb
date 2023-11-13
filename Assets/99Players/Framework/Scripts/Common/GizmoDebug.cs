using System.Collections.Generic;
using UnityEngine;
public class GizmoDebug {
    public struct GizmoDebugData {
        public Vector3 pos;
        public Vector3 subVec3;
        public float subFloat;
        public Color color;
        public GizmoManager.GizmoType type;
        public GizmoDebugData(Vector3 _pos, Color _color, GizmoManager.GizmoType _type, Vector3 _subVec3 = default(Vector3), float _subFloat = 0f) {
            pos = _pos;
            color = _color;
            type = _type;
            subVec3 = _subVec3;
            subFloat = _subFloat;
        }
    }
    private List<GizmoDebugData> gizmoDebugDataList = new List<GizmoDebugData>();
    private bool isShow;
    public void Init(bool _isShow) {
        isShow = _isShow;
    }
    public void ResetGizmoDebug() {
        if (isShow) {
            gizmoDebugDataList.Clear();
        }
    }
    public void AddGizmoDebug(GizmoDebugData _data) {
        if (isShow) {
            gizmoDebugDataList.Add(_data);
        }
    }
    public void ShowDrawGizmoDebug() {
        if (!isShow) {
            return;
        }
        for (int i = 0; i < gizmoDebugDataList.Count; i++) {
            Gizmos.color = gizmoDebugDataList[i].color;
            switch (gizmoDebugDataList[i].type) {
                case GizmoManager.GizmoType.CUBE:
                    Gizmos.DrawCube(gizmoDebugDataList[i].pos, gizmoDebugDataList[i].subVec3);
                    break;
                case GizmoManager.GizmoType.WIRE_CUBE:
                    Gizmos.DrawWireCube(gizmoDebugDataList[i].pos, gizmoDebugDataList[i].subVec3);
                    break;
                case GizmoManager.GizmoType.SPHERE:
                    Gizmos.DrawSphere(gizmoDebugDataList[i].pos, gizmoDebugDataList[i].subFloat);
                    break;
                case GizmoManager.GizmoType.WIRE_SPHERE:
                    Gizmos.DrawWireSphere(gizmoDebugDataList[i].pos, gizmoDebugDataList[i].subFloat);
                    break;
                case GizmoManager.GizmoType.LINE:
                    Gizmos.DrawLine(gizmoDebugDataList[i].pos, gizmoDebugDataList[i].subVec3);
                    break;
                case GizmoManager.GizmoType.RAY:
                    Gizmos.DrawRay(gizmoDebugDataList[i].pos, gizmoDebugDataList[i].subVec3);
                    break;
            }
        }
    }
}
