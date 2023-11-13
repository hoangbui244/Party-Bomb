using System.Collections.Generic;
using TMPro;
using UnityEngine;
public static class TransformExtension {
    private static Vector3 mCalcVector3;
    public static void SetPosition(this Transform transform, float x, float y, float z) {
        mCalcVector3.Set(x, y, z);
        transform.position = mCalcVector3;
    }
    public static void SetPositionX(this Transform transform, float x) {
        mCalcVector3.Set(x, transform.position.y, transform.position.z);
        transform.position = mCalcVector3;
    }
    public static void SetPositionY(this Transform transform, float y) {
        mCalcVector3.Set(transform.position.x, y, transform.position.z);
        transform.position = mCalcVector3;
    }
    public static void SetPositionZ(this Transform transform, float z) {
        mCalcVector3.Set(transform.position.x, transform.position.y, z);
        transform.position = mCalcVector3;
    }
    public static void AddPosition(this Transform transform, float x, float y, float z) {
        mCalcVector3.Set(transform.position.x + x, transform.position.y + y, transform.position.z + z);
        transform.position = mCalcVector3;
    }
    public static void AddPositionX(this Transform transform, float x) {
        mCalcVector3.Set(transform.position.x + x, transform.position.y, transform.position.z);
        transform.position = mCalcVector3;
    }
    public static void AddPositionY(this Transform transform, float y) {
        mCalcVector3.Set(transform.position.x, transform.position.y + y, transform.position.z);
        transform.position = mCalcVector3;
    }
    public static void AddPositionZ(this Transform transform, float z) {
        mCalcVector3.Set(transform.position.x, transform.position.y, transform.position.z + z);
        transform.position = mCalcVector3;
    }
    public static void SetLocalPosition(this Transform transform, float x, float y, float z) {
        mCalcVector3.Set(x, y, z);
        transform.localPosition = mCalcVector3;
    }
    public static void SetLocalPositionX(this Transform transform, float x) {
        mCalcVector3.Set(x, transform.localPosition.y, transform.localPosition.z);
        transform.localPosition = mCalcVector3;
    }
    public static void SetLocalPositionY(this Transform transform, float y) {
        mCalcVector3.Set(transform.localPosition.x, y, transform.localPosition.z);
        transform.localPosition = mCalcVector3;
    }
    public static void SetLocalPositionZ(this Transform transform, float z) {
        mCalcVector3.Set(transform.localPosition.x, transform.localPosition.y, z);
        transform.localPosition = mCalcVector3;
    }
    public static void AddLocalPosition(this Transform transform, float x, float y, float z) {
        mCalcVector3.Set(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z + z);
        transform.localPosition = mCalcVector3;
    }
    public static void AddLocalPositionX(this Transform transform, float x) {
        mCalcVector3.Set(transform.localPosition.x + x, transform.localPosition.y, transform.localPosition.z);
        transform.localPosition = mCalcVector3;
    }
    public static void AddLocalPositionY(this Transform transform, float y) {
        mCalcVector3.Set(transform.localPosition.x, transform.localPosition.y + y, transform.localPosition.z);
        transform.localPosition = mCalcVector3;
    }
    public static void AddLocalPositionZ(this Transform transform, float z) {
        mCalcVector3.Set(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + z);
        transform.localPosition = mCalcVector3;
    }
    public static void SetLocalScale(this Transform transform, float x, float y, float z) {
        mCalcVector3.Set(x, y, z);
        transform.localScale = mCalcVector3;
    }
    public static void SetLocalScaleX(this Transform transform, float x) {
        mCalcVector3.Set(x, transform.localScale.y, transform.localScale.z);
        transform.localScale = mCalcVector3;
    }
    public static void SetLocalScaleY(this Transform transform, float y) {
        mCalcVector3.Set(transform.localScale.x, y, transform.localScale.z);
        transform.localScale = mCalcVector3;
    }
    public static void SetLocalScaleZ(this Transform transform, float z) {
        mCalcVector3.Set(transform.localScale.x, transform.localScale.y, z);
        transform.localScale = mCalcVector3;
    }
    public static void AddLocalScale(this Transform transform, float x, float y, float z) {
        mCalcVector3.Set(transform.localScale.x + x, transform.localScale.y + y, transform.localScale.z + z);
        transform.localScale = mCalcVector3;
    }
    public static void AddLocalScaleX(this Transform transform, float x) {
        mCalcVector3.Set(transform.localScale.x + x, transform.localScale.y, transform.localScale.z);
        transform.localScale = mCalcVector3;
    }
    public static void AddLocalScaleY(this Transform transform, float y) {
        mCalcVector3.Set(transform.localScale.x, transform.localScale.y + y, transform.localScale.z);
        transform.localScale = mCalcVector3;
    }
    public static void AddLocalScaleZ(this Transform transform, float z) {
        mCalcVector3.Set(transform.localScale.x, transform.localScale.y, transform.localScale.z + z);
        transform.localScale = mCalcVector3;
    }
    public static void SetEulerAngles(this Transform transform, float x, float y, float z) {
        mCalcVector3.Set(x, y, z);
        transform.eulerAngles = mCalcVector3;
    }
    public static void SetEulerAnglesX(this Transform transform, float x) {
        mCalcVector3.Set(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
        transform.eulerAngles = mCalcVector3;
    }
    public static void SetEulerAnglesY(this Transform transform, float y) {
        mCalcVector3.Set(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
        transform.eulerAngles = mCalcVector3;
    }
    public static void SetEulerAnglesZ(this Transform transform, float z) {
        mCalcVector3.Set(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
        transform.eulerAngles = mCalcVector3;
    }
    public static void AddEulerAngles(this Transform transform, float x, float y, float z) {
        transform.Rotate(x, y, z);
    }
    public static void AddEulerAnglesX(this Transform transform, float x) {
        transform.Rotate(x, 0f, 0f);
    }
    public static void AddEulerAnglesY(this Transform transform, float y) {
        transform.Rotate(0f, y, 0f);
    }
    public static void AddEulerAnglesZ(this Transform transform, float z) {
        transform.Rotate(0f, 0f, z);
    }
    public static void SetLocalEulerAngles(this Transform transform, float x, float y, float z) {
        mCalcVector3.Set(x, y, z);
        transform.localEulerAngles = mCalcVector3;
    }
    public static void SetLocalEulerAnglesX(this Transform transform, float x) {
        mCalcVector3.Set(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
        transform.localEulerAngles = mCalcVector3;
    }
    public static void SetLocalEulerAnglesY(this Transform transform, float y) {
        mCalcVector3.Set(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
        transform.localEulerAngles = mCalcVector3;
    }
    public static void SetLocalEulerAnglesZ(this Transform transform, float z) {
        mCalcVector3.Set(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
        transform.localEulerAngles = mCalcVector3;
    }
    public static void AddLocalEulerAngles(this Transform transform, float x, float y, float z) {
        transform.Rotate(x, y, z, Space.Self);
    }
    public static void AddLocalEulerAnglesX(this Transform transform, float x) {
        transform.Rotate(x, 0f, 0f, Space.Self);
    }
    public static void AddLocalEulerAnglesY(this Transform transform, float y) {
        transform.Rotate(0f, y, 0f, Space.Self);
    }
    public static void AddLocalEulerAnglesZ(this Transform transform, float z) {
        transform.Rotate(0f, 0f, z, Space.Self);
    }
    public static Vector3 GetNegativeNumberRotation(this Transform transform) {
        mCalcVector3 = transform.rotation.eulerAngles;
        mCalcVector3.x %= 360f;
        mCalcVector3.y %= 360f;
        mCalcVector3.z %= 360f;
        if (mCalcVector3.x < 0f) {
            mCalcVector3.x = 360f + mCalcVector3.x;
        }
        if (mCalcVector3.y < 0f) {
            mCalcVector3.y = 360f + mCalcVector3.y;
        }
        if (mCalcVector3.z < 0f) {
            mCalcVector3.z = 360f + mCalcVector3.z;
        }
        return mCalcVector3;
    }
    public static void SetAlpha(this Material self, float _a) {
        Color color = self.color;
        color.a = _a;
        self.color = color;
    }
    public static void SetAlpha(this SpriteRenderer self, float _a) {
        Color color = self.color;
        color.a = _a;
        self.color = color;
    }
    public static void SetAlpha(this MeshRenderer self, float _a) {
        Color color = self.material.color;
        color.a = _a;
        self.material.color = color;
    }
    public static void SetAlpha(this TextMeshPro self, float _a) {
        Color color = self.color;
        color.a = _a;
        self.color = color;
    }
    public static List<T> Shuffle<T>(this List<T> list) {
        for (int i = 0; i < list.Count; i++) {
            T value = list[i];
            int index = Random.Range(0, list.Count);
            list[i] = list[index];
            list[index] = value;
        }
        return list;
    }
}
