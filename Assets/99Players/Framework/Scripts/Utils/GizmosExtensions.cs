using System;
using UnityEngine;
namespace Utils {
    /// <summary>
    /// 
    /// </summary>
    public static class GizmosExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="rotation"></param>
        public static void DrawWireCube(Vector3 center, Vector3 size, Quaternion rotation = default(Quaternion)) {
            Matrix4x4 matrix = Gizmos.matrix;
            if (rotation.Equals(default(Quaternion))) {
                rotation = Quaternion.identity;
            }
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, size);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = matrix;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="arrowHeadLength"></param>
        /// <param name="arrowHeadAngle"></param>
        public static void DrawArrow(Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f) {
            Gizmos.DrawLine(from, to);
            Vector3 forward = to - from;
            Vector3 a = Quaternion.LookRotation(forward) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
            Vector3 a2 = Quaternion.LookRotation(forward) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
            Gizmos.DrawLine(to, to + a * arrowHeadLength);
            Gizmos.DrawLine(to, to + a2 * arrowHeadLength);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="rotation"></param>
        public static void DrawWireSphere(Vector3 center, float radius, Quaternion rotation = default(Quaternion)) {
            Matrix4x4 matrix = Gizmos.matrix;
            if (rotation.Equals(default(Quaternion))) {
                rotation = Quaternion.identity;
            }
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = matrix;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="segments"></param>
        /// <param name="rotation"></param>
        public static void DrawWireCircle(Vector3 center, float radius, int segments = 20, Quaternion rotation = default(Quaternion)) {
            DrawWireArc(center, radius, 360f, segments, rotation);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        /// <param name="segments"></param>
        /// <param name="rotation"></param>
        public static void DrawWireArc(Vector3 center, float radius, float angle, int segments = 20, Quaternion rotation = default(Quaternion)) {
            Matrix4x4 matrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            Vector3 from = Vector3.forward * radius;
            int num = Mathf.RoundToInt(angle / (float)segments);
            for (int i = 0; (float)i <= angle; i += num) {
                Vector3 vector = new Vector3(radius * Mathf.Sin((float)i * ((float)Math.PI / 180f)), 0f, radius * Mathf.Cos((float)i * ((float)Math.PI / 180f)));
                Gizmos.DrawLine(from, vector);
                from = vector;
            }
            Gizmos.matrix = matrix;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        /// <param name="segments"></param>
        /// <param name="rotation"></param>
        /// <param name="centerOfRotation"></param>
        public static void DrawWireArc(Vector3 center, float radius, float angle, int segments, Quaternion rotation, Vector3 centerOfRotation) {
            Matrix4x4 matrix = Gizmos.matrix;
            if (rotation.Equals(default(Quaternion))) {
                rotation = Quaternion.identity;
            }
            Gizmos.matrix = Matrix4x4.TRS(centerOfRotation, rotation, Vector3.one);
            Vector3 vector = centerOfRotation - center;
            Vector3 from = vector + Vector3.forward * radius;
            int num = Mathf.RoundToInt(angle / (float)segments);
            for (int i = 0; (float)i <= angle; i += num) {
                Vector3 vector2 = new Vector3(radius * Mathf.Sin((float)i * ((float)Math.PI / 180f)), 0f, radius * Mathf.Cos((float)i * ((float)Math.PI / 180f))) + vector;
                Gizmos.DrawLine(from, vector2);
                from = vector2;
            }
            Gizmos.matrix = matrix;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        /// <param name="segments"></param>
        public static void DrawWireArc(Matrix4x4 matrix, float radius, float angle, int segments) {
            Matrix4x4 matrix2 = Gizmos.matrix;
            Gizmos.matrix = matrix;
            Vector3 from = Vector3.forward * radius;
            int num = Mathf.RoundToInt(angle / (float)segments);
            for (int i = 0; (float)i <= angle; i += num) {
                Vector3 vector = new Vector3(radius * Mathf.Sin((float)i * ((float)Math.PI / 180f)), 0f, radius * Mathf.Cos((float)i * ((float)Math.PI / 180f)));
                Gizmos.DrawLine(from, vector);
                from = vector;
            }
            Gizmos.matrix = matrix2;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="rotation"></param>
        public static void DrawWireCylinder(Vector3 center, float radius, float height, Quaternion rotation = default(Quaternion)) {
            Matrix4x4 matrix = Gizmos.matrix;
            if (rotation.Equals(default(Quaternion))) {
                rotation = Quaternion.identity;
            }
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            float d = height / 2f;
            Gizmos.DrawLine(Vector3.right * radius - Vector3.up * d, Vector3.right * radius + Vector3.up * d);
            Gizmos.DrawLine(-Vector3.right * radius - Vector3.up * d, -Vector3.right * radius + Vector3.up * d);
            Gizmos.DrawLine(Vector3.forward * radius - Vector3.up * d, Vector3.forward * radius + Vector3.up * d);
            Gizmos.DrawLine(-Vector3.forward * radius - Vector3.up * d, -Vector3.forward * radius + Vector3.up * d);
            DrawWireArc(center + Vector3.up * d, radius, 360f, 20, rotation, center);
            DrawWireArc(center + Vector3.down * d, radius, 360f, 20, rotation, center);
            Gizmos.matrix = matrix;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="rotation"></param>
        public static void DrawWireCapsule(Vector3 center, float radius, float height, Quaternion rotation = default(Quaternion)) {
            if (rotation.Equals(default(Quaternion))) {
                rotation = Quaternion.identity;
            }
            Matrix4x4 matrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            float d = height / 2f - radius;
            DrawWireCylinder(center, radius, height - radius * 2f, rotation);
            DrawWireArc(Matrix4x4.Translate(center + rotation * Vector3.up * d) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90f, Vector3.forward)), radius, 180f, 20);
            DrawWireArc(Matrix4x4.Translate(center + rotation * Vector3.up * d) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90f, Vector3.up) * Quaternion.AngleAxis(90f, Vector3.forward)), radius, 180f, 20);
            DrawWireArc(Matrix4x4.Translate(center + rotation * Vector3.down * d) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90f, Vector3.up) * Quaternion.AngleAxis(-90f, Vector3.forward)), radius, 180f, 20);
            DrawWireArc(Matrix4x4.Translate(center + rotation * Vector3.down * d) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(-90f, Vector3.forward)), radius, 180f, 20);
            Gizmos.matrix = matrix;
        }
    }
}
