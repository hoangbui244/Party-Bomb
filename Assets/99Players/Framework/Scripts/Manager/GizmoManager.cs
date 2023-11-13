using System;
using UnityEngine;
public class GizmoManager : MonoBehaviour {
    public enum GizmoType {
        RAY,
        LINE,
        CUBE,
        WIRE_CUBE,
        SPHERE,
        WIRE_SPHERE
    }
    [Serializable]
    public struct GizmoData {
        public Vector3 vec1;
        public Vector3 vec2;
        public Color color;
        public string label;
        public Color labelColor;
        public GizmoData(Vector3 _vec1, Vector3 _vec2, Color _color, string _label, Color _labelColor) {
            vec1 = _vec1;
            vec2 = _vec2;
            color = _color;
            label = _label;
            labelColor = _labelColor;
        }
    }
    private struct CameraData {
        public float fov;
        public float size;
        public float max;
        public float min;
        public float aspect;
    }
    public const string LABEL_DEF = "Default";
    [SerializeField]
    private GizmoType mType;
    [SerializeField]
    private GizmoData mData = new GizmoData(new Vector3(0f, 0f, 0f), new Vector3(10f, 10f, 10f), new Color(1f, 1f, 1f, 1f), "", new Color(1f, 1f, 1f, 1f));
    private CameraData mCameraData;
    private Matrix4x4 mMatDef;
    public GizmoType gsType {
        get {
            return mType;
        }
        set {
            mType = value;
        }
    }
    public GizmoData gsData {
        get {
            return mData;
        }
        set {
            mData = value;
        }
    }
    public void OnDrawGizmos() {
        mMatDef = Gizmos.matrix;
        Gizmos.color = mData.color;
        switch (mType) {
            case GizmoType.RAY:
                Gizmos.DrawRay(base.transform.position + mData.vec1, mData.vec2);
                break;
            case GizmoType.LINE:
                Gizmos.DrawLine(base.transform.position + mData.vec1, base.transform.position + mData.vec2);
                break;
            case GizmoType.CUBE:
                Gizmos.DrawCube(base.transform.position + mData.vec1, mData.vec2);
                break;
            case GizmoType.WIRE_CUBE:
                Gizmos.DrawWireCube(base.transform.position + mData.vec1, mData.vec2);
                break;
            case GizmoType.SPHERE:
                Gizmos.DrawSphere(base.transform.position + mData.vec1, mData.vec2.x);
                break;
            case GizmoType.WIRE_SPHERE:
                Gizmos.DrawWireSphere(base.transform.position + mData.vec1, mData.vec2.x);
                break;
            default:
                Gizmos.DrawWireCube(base.transform.position + mData.vec1, mData.vec2);
                break;
        }
    }
    public void SetVec1(Vector3 _vec) {
        mData.vec1 = _vec;
    }
    public void SetVec2(Vector3 _vec) {
        mData.vec2 = _vec;
    }
    public void SetColor(Color _col) {
        mData.color = _col;
    }
    public void SetLabel(string _label) {
        mData.label = _label;
    }
    public void SetLabelColor(Color _col) {
        mData.labelColor = _col;
    }
}
