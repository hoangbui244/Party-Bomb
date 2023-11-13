using System;
using UnityEngine;
public class Math3D : MonoBehaviour {
    private static Transform tempChild;
    private static Transform tempParent;
    public static void Init() {
        tempChild = new GameObject("Math3d_TempChild").transform;
        tempParent = new GameObject("Math3d_TempParent").transform;
        tempChild.gameObject.hideFlags = HideFlags.HideAndDontSave;
        UnityEngine.Object.DontDestroyOnLoad(tempChild.gameObject);
        tempParent.gameObject.hideFlags = HideFlags.HideAndDontSave;
        UnityEngine.Object.DontDestroyOnLoad(tempParent.gameObject);
        tempChild.parent = tempParent;
    }
    public static Vector3 AddVectorLength(Vector3 vector, float size) {
        float num = Vector3.Magnitude(vector);
        num += size;
        return Vector3.Scale(Vector3.Normalize(vector), new Vector3(num, num, num));
    }
    public static Vector3 SetVectorLength(Vector3 vector, float size) {
        return Vector3.Normalize(vector) * size;
    }
    public static Quaternion SubtractRotation(Quaternion B, Quaternion A) {
        return Quaternion.Inverse(A) * B;
    }
    public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position) {
        linePoint = Vector3.zero;
        lineVec = Vector3.zero;
        lineVec = Vector3.Cross(plane1Normal, plane2Normal);
        Vector3 vector = Vector3.Cross(plane2Normal, lineVec);
        float num = Vector3.Dot(plane1Normal, vector);
        if (Mathf.Abs(num) > 0.006f) {
            Vector3 rhs = plane1Position - plane2Position;
            float d = Vector3.Dot(plane1Normal, rhs) / num;
            linePoint = plane2Position + d * vector;
            return true;
        }
        return false;
    }
    public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint) {
        intersection = Vector3.zero;
        float num = Vector3.Dot(planePoint - linePoint, planeNormal);
        float num2 = Vector3.Dot(lineVec, planeNormal);
        if (num2 != 0f) {
            float size = num / num2;
            Vector3 b = SetVectorLength(lineVec, size);
            intersection = linePoint + b;
            return true;
        }
        return false;
    }
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2) {
        intersection = Vector3.zero;
        Vector3 lhs = linePoint2 - linePoint1;
        Vector3 rhs = Vector3.Cross(lineVec1, lineVec2);
        Vector3 lhs2 = Vector3.Cross(lhs, lineVec2);
        float num = Vector3.Dot(lhs, rhs);
        if (num >= 1E-05f || num <= -1E-05f) {
            return false;
        }
        float num2 = Vector3.Dot(lhs2, rhs) / rhs.sqrMagnitude;
        if (num2 >= 0f && num2 <= 1f) {
            intersection = linePoint1 + lineVec1 * num2;
            return true;
        }
        return false;
    }
    public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2) {
        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;
        float num = Vector3.Dot(lineVec1, lineVec1);
        float num2 = Vector3.Dot(lineVec1, lineVec2);
        float num3 = Vector3.Dot(lineVec2, lineVec2);
        float num4 = num * num3 - num2 * num2;
        if (num4 != 0f) {
            Vector3 rhs = linePoint1 - linePoint2;
            float num5 = Vector3.Dot(lineVec1, rhs);
            float num6 = Vector3.Dot(lineVec2, rhs);
            float d = (num2 * num6 - num5 * num3) / num4;
            float d2 = (num * num6 - num5 * num2) / num4;
            closestPointLine1 = linePoint1 + lineVec1 * d;
            closestPointLine2 = linePoint2 + lineVec2 * d2;
            return true;
        }
        return false;
    }
    public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point) {
        float d = Vector3.Dot(point - linePoint, lineVec);
        return linePoint + lineVec * d;
    }
    public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point) {
        float num = SignedDistancePlanePoint(planeNormal, planePoint, point);
        num *= -1f;
        Vector3 b = SetVectorLength(planeNormal, num);
        return point + b;
    }
    public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector) {
        return vector - Vector3.Dot(vector, planeNormal) * planeNormal;
    }
    public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point) {
        return Vector3.Dot(planeNormal, point - planePoint);
    }
    public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal) {
        return Vector3.Dot(Vector3.Cross(normal, vectorA), vectorB);
    }
    public static float AngleVectorPlane(Vector3 vector, Vector3 normal) {
        float num = (float)Math.Acos(Vector3.Dot(vector, normal));
        return (float)Math.PI / 2f - num;
    }
    public static float DotProductAngle(Vector3 vec1, Vector3 vec2) {
        double num = Vector3.Dot(vec1, vec2);
        if (num < -1.0) {
            num = -1.0;
        }
        if (num > 1.0) {
            num = 1.0;
        }
        return (float)Math.Acos(num);
    }
    public static void PlaneFrom3Points(out Vector3 planeNormal, out Vector3 planePoint, Vector3 pointA, Vector3 pointB, Vector3 pointC) {
        planeNormal = Vector3.zero;
        planePoint = Vector3.zero;
        Vector3 vector = pointB - pointA;
        Vector3 vector2 = pointC - pointA;
        planeNormal = Vector3.Normalize(Vector3.Cross(vector, vector2));
        Vector3 vector3 = pointA + vector / 2f;
        Vector3 vector4 = pointA + vector2 / 2f;
        Vector3 lineVec = pointC - vector3;
        Vector3 lineVec2 = pointB - vector4;
        ClosestPointsOnTwoLines(out planePoint, out Vector3 _, vector3, lineVec, vector4, lineVec2);
    }
    public static Vector3 GetForwardVector(Quaternion q) {
        return q * Vector3.forward;
    }
    public static Vector3 GetUpVector(Quaternion q) {
        return q * Vector3.up;
    }
    public static Vector3 GetRightVector(Quaternion q) {
        return q * Vector3.right;
    }
    public static Quaternion QuaternionFromMatrix(Matrix4x4 m) {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }
    public static Vector3 PositionFromMatrix(Matrix4x4 m) {
        Vector4 column = m.GetColumn(3);
        return new Vector3(column.x, column.y, column.z);
    }
    public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp) {
        Quaternion lhs = Quaternion.LookRotation(alignWithVector, alignWithNormal);
        Quaternion rotation = Quaternion.LookRotation(customForward, customUp);
        gameObjectInOut.transform.rotation = lhs * Quaternion.Inverse(rotation);
    }
    public static void TransformWithParent(out Quaternion childRotation, out Vector3 childPosition, Quaternion parentRotation, Vector3 parentPosition, Quaternion startParentRotation, Vector3 startParentPosition, Quaternion startChildRotation, Vector3 startChildPosition) {
        childRotation = Quaternion.identity;
        childPosition = Vector3.zero;
        tempParent.rotation = startParentRotation;
        tempParent.position = startParentPosition;
        tempParent.localScale = Vector3.one;
        tempChild.rotation = startChildRotation;
        tempChild.position = startChildPosition;
        tempChild.localScale = Vector3.one;
        tempParent.rotation = parentRotation;
        tempParent.position = parentPosition;
        childRotation = tempChild.rotation;
        childPosition = tempChild.position;
    }
    public static void PreciseAlign(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 alignWithPosition, Vector3 triangleForward, Vector3 triangleNormal, Vector3 trianglePosition) {
        LookRotationExtended(ref gameObjectInOut, alignWithVector, alignWithNormal, triangleForward, triangleNormal);
        Vector3 b = gameObjectInOut.transform.TransformPoint(trianglePosition);
        Vector3 translation = alignWithPosition - b;
        gameObjectInOut.transform.Translate(translation, Space.World);
    }
}
