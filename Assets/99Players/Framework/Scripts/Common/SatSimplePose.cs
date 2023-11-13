using System;
using UnityEngine;
[CreateAssetMenu]
public class SatSimplePose : ScriptableObject {
    private const int ARRAY_SIZE = 9;
    public int poseId;
    public string poseName;
    [SerializeField]
    private Vector3[] pos = new Vector3[9];
    [SerializeField]
    private Quaternion[] rot = new Quaternion[9];
    [SerializeField]
    private Vector3[] scl = new Vector3[9];
    public Vector3[] GetPartsPositions() {
        return pos;
    }
    public Quaternion[] GetPartsRotations() {
        return rot;
    }
    public Vector3[] GetPartsScales() {
        return scl;
    }
    public void KeepTransformValues(Transform[] tr) {
        if (tr.Length > pos.Length) {
            Array.Resize(ref pos, tr.Length);
            Array.Resize(ref rot, tr.Length);
            Array.Resize(ref scl, tr.Length);
        }
        for (int i = 0; i < tr.Length; i++) {
            pos[i] = tr[i].localPosition;
            rot[i] = tr[i].localRotation;
            scl[i] = tr[i].localScale;
        }
        for (int j = tr.Length; j < 9; j++) {
            pos[j] = Vector3.zero;
            rot[j] = Quaternion.identity;
            scl[j] = Vector3.zero;
        }
    }
    public void ApplyValues(Skijump_Character chara) {
        Transform[] bodyPartsObj = chara.BodyPartsObj;
        ApplyValues(bodyPartsObj);
    }
    public void ApplyValues(IPoseControllable target) {
        Transform[] allBodyPartsTransform = target.GetAllBodyPartsTransform();
        ApplyValues(allBodyPartsTransform);
    }
    public void ApplyValues(GameObject root) {
        Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>();
        ApplyValues(componentsInChildren);
    }
    public void ApplyValues(Skijump_Character chara, int mask) {
        Transform[] bodyPartsObj = chara.BodyPartsObj;
        ApplyMaskedValues(bodyPartsObj, mask);
    }
    public void ApplyValues(IPoseControllable target, int mask) {
        Transform[] allBodyPartsTransform = target.GetAllBodyPartsTransform();
        ApplyMaskedValues(allBodyPartsTransform, mask);
    }
    private void ApplyValues(Transform[] tr) {
        for (int i = 0; i < Mathf.Min(tr.Length, pos.Length); i++) {
            tr[i].localPosition = pos[i];
            tr[i].localRotation = rot[i];
            tr[i].localScale = scl[i];
        }
    }
    private void ApplyMaskedValues(Transform[] tr, int mask) {
        int num = mask;
        for (int i = 0; i < Mathf.Min(tr.Length, pos.Length); i++) {
            if (num % 2 == 1) {
                num /= 2;
                continue;
            }
            num /= 2;
            tr[i].localPosition = pos[i];
            tr[i].localRotation = rot[i];
            tr[i].localScale = scl[i];
        }
    }
}
