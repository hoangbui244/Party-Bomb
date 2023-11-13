using System;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class PipeMeshGenerator : MonoBehaviour {
    private List<Vector3> points = new List<Vector3>();
    public float pipeRadius = 0.2f;
    public float elbowRadius = 0.5f;
    [Range(3f, 32f)]
    public int pipeSegments = 8;
    [Range(3f, 32f)]
    public int elbowSegments = 6;
    public Material[] pipeMaterials;
    public bool flatShading;
    public bool avoidStrangling;
    public bool generateEndCaps;
    public bool generateElbows = true;
    public bool generateOnStart;
    public bool makeDoubleSided;
    public float colinearThreshold = 0.001f;
    private List<Transform> pointList = new List<Transform>();
    public Material[] PipeMaterials => pipeMaterials;
    private void Start() {
        if (generateOnStart) {
            RenderPipe();
        }
    }
    private void Update() {
    }
    public void RenderPipe() {
        points.Clear();
        foreach (Transform item in base.transform) {
            points.Add(item.localPosition);
        }
        if (points.Count < 2) {
            throw new Exception("Cannot render a pipe with fewer than 2 points");
        }
        RemoveColinearPoints();
        MeshFilter component = GetComponent<MeshFilter>();
        MeshFilter obj = (component != null) ? component : base.gameObject.AddComponent<MeshFilter>();
        Mesh mesh = GenerateMesh();
        if (flatShading) {
            mesh = MakeFlatShading(mesh);
        }
        if (makeDoubleSided) {
            mesh = MakeDoubleSided(mesh);
        }
        obj.mesh = mesh;
        MeshRenderer component2 = GetComponent<MeshRenderer>();
        ((component2 != null) ? component2 : base.gameObject.AddComponent<MeshRenderer>()).materials = pipeMaterials;
    }
    private Mesh GenerateMesh() {
        Mesh mesh = new Mesh();
        mesh.name = "UnityPlumber Pipe";
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        for (int i = 0; i < points.Count - 1; i++) {
            Vector3 vector = points[i];
            Vector3 vector2 = points[i + 1];
            Vector3 normalized = (points[i + 1] - points[i]).normalized;
            if (i > 0 && generateElbows) {
                vector += normalized * elbowRadius;
            }
            if (i < points.Count - 2 && generateElbows) {
                vector2 -= normalized * elbowRadius;
            }
            GenerateCircleAtPoint(vertices, normals, vector, normalized);
            GenerateCircleAtPoint(vertices, normals, vector2, normalized);
            MakeCylinderTriangles(triangles, i);
        }
        if (generateElbows) {
            for (int j = 0; j < points.Count - 2; j++) {
                Vector3 point = points[j];
                Vector3 point2 = points[j + 1];
                Vector3 point3 = points[j + 2];
                GenerateElbow(j, vertices, normals, triangles, point, point2, point3);
            }
        }
        if (generateEndCaps) {
            GenerateEndCaps(vertices, triangles, normals);
        }
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetNormals(normals);
        return mesh;
    }
    private void RemoveColinearPoints() {
        List<int> list = new List<int>();
        for (int i = 0; i < points.Count - 2; i++) {
            Vector3 b = points[i];
            Vector3 vector = points[i + 1];
            Vector3 a = points[i + 2];
            Vector3 vector2 = vector - b;
            if (Vector3.Distance(b: (a - vector).normalized, a: vector2.normalized) < colinearThreshold) {
                list.Add(i + 1);
            }
        }
        list.Reverse();
        foreach (int item in list) {
            points.RemoveAt(item);
        }
    }
    private void GenerateCircleAtPoint(List<Vector3> vertices, List<Vector3> normals, Vector3 center, Vector3 direction) {
        float num = (float)Math.PI * 2f / (float)pipeSegments;
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        Vector3 tangent = Vector3.up;
        Vector3 binormal = Vector3.right;
        if (plane.GetSide(direction)) {
            binormal = Vector3.left;
        }
        Vector3.OrthoNormalize(ref direction, ref tangent, ref binormal);
        for (int i = 0; i < pipeSegments; i++) {
            Vector3 vector = center + pipeRadius * Mathf.Cos(num * (float)i) * tangent + pipeRadius * Mathf.Sin(num * (float)i) * binormal;
            vertices.Add(vector);
            normals.Add((vector - center).normalized);
        }
    }
    private void MakeCylinderTriangles(List<int> triangles, int segmentIdx) {
        int num = segmentIdx * pipeSegments * 2;
        for (int i = 0; i < pipeSegments; i++) {
            triangles.Add(num + (i + 1) % pipeSegments);
            triangles.Add(num + i + pipeSegments);
            triangles.Add(num + i);
            triangles.Add(num + (i + 1) % pipeSegments);
            triangles.Add(num + (i + 1) % pipeSegments + pipeSegments);
            triangles.Add(num + i + pipeSegments);
        }
    }
    private void MakeElbowTriangles(List<Vector3> vertices, List<int> triangles, int segmentIdx, int elbowIdx) {
        int num = (points.Count - 1) * pipeSegments * 2;
        num += elbowIdx * (elbowSegments + 1) * pipeSegments;
        num += segmentIdx * pipeSegments;
        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        if (avoidStrangling) {
            List<Vector3> list = new List<Vector3>();
            List<Vector3> list2 = new List<Vector3>();
            for (int i = 0; i < pipeSegments; i++) {
                list2.Add(vertices[num + i - pipeSegments]);
            }
            for (int j = 0; j < pipeSegments; j++) {
                Vector3 item = Vector3.zero;
                float num2 = float.PositiveInfinity;
                for (int k = 0; k < pipeSegments; k++) {
                    Vector3 vector = vertices[num + k];
                    float num3 = Vector3.Distance(list2[j], vector);
                    if (num3 < num2) {
                        num2 = num3;
                        item = vector;
                    }
                }
                list.Add(item);
                dictionary.Add(j, vertices.IndexOf(item));
            }
        } else {
            for (int l = 0; l < pipeSegments; l++) {
                dictionary.Add(l, num + l);
            }
        }
        for (int m = 0; m < pipeSegments; m++) {
            triangles.Add(dictionary[m]);
            triangles.Add(num + m - pipeSegments);
            triangles.Add(dictionary[(m + 1) % pipeSegments]);
            triangles.Add(num + m - pipeSegments);
            triangles.Add(num + (m + 1) % pipeSegments - pipeSegments);
            triangles.Add(dictionary[(m + 1) % pipeSegments]);
        }
    }
    private Mesh MakeFlatShading(Mesh mesh) {
        List<Vector3> list = new List<Vector3>();
        List<int> list2 = new List<int>();
        List<Vector3> list3 = new List<Vector3>();
        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            int num = mesh.triangles[i];
            int num2 = mesh.triangles[i + 1];
            int num3 = mesh.triangles[i + 2];
            list.Add(mesh.vertices[num]);
            list.Add(mesh.vertices[num2]);
            list.Add(mesh.vertices[num3]);
            list2.Add(list.Count - 3);
            list2.Add(list.Count - 2);
            list2.Add(list.Count - 1);
            Vector3 normalized = Vector3.Cross(mesh.vertices[num2] - mesh.vertices[num], mesh.vertices[num3] - mesh.vertices[num]).normalized;
            list3.Add(normalized);
            list3.Add(normalized);
            list3.Add(normalized);
        }
        mesh.SetVertices(list);
        mesh.SetTriangles(list2, 0);
        mesh.SetNormals(list3);
        return mesh;
    }
    private Mesh MakeDoubleSided(Mesh mesh) {
        List<int> list = new List<int>(mesh.triangles);
        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            int item = mesh.triangles[i];
            int item2 = mesh.triangles[i + 1];
            int item3 = mesh.triangles[i + 2];
            list.Add(item3);
            list.Add(item2);
            list.Add(item);
        }
        mesh.SetTriangles(list, 0);
        return mesh;
    }
    private void GenerateElbow(int index, List<Vector3> vertices, List<Vector3> normals, List<int> triangles, Vector3 point1, Vector3 point2, Vector3 point3) {
        Vector3 vector = (point2 - point1).normalized * elbowRadius;
        Vector3 vector2 = (point3 - point2).normalized * elbowRadius;
        Vector3 vector3 = point2 - vector;
        Vector3 vector4 = point2 + vector2;
        Vector3 lhs = Vector3.Cross(vector, vector2);
        Vector3 normalized = Vector3.Cross(lhs, vector).normalized;
        Vector3 normalized2 = Vector3.Cross(lhs, vector2).normalized;
        Math3D.ClosestPointsOnTwoLines(out Vector3 closestPointLine, out Vector3 closestPointLine2, vector3, normalized, vector4, normalized2);
        Vector3 vector5 = 0.5f * (closestPointLine + closestPointLine2);
        float magnitude = (vector5 - vector3).magnitude;
        float num = Vector3.Angle(vector3 - vector5, vector4 - vector5) * ((float)Math.PI / 180f) / (float)elbowSegments;
        Vector3 b = point2 - vector3;
        for (int i = 0; i <= elbowSegments; i++) {
            Vector3 normal = (vector3 - vector5).normalized;
            Vector3 tangent = (vector4 - vector5).normalized;
            Vector3.OrthoNormalize(ref normal, ref tangent);
            Vector3 vector6 = vector5 + magnitude * Mathf.Cos(num * (float)i) * normal + magnitude * Mathf.Sin(num * (float)i) * tangent;
            Vector3 direction = vector6 - b;
            b = vector6;
            if (i == elbowSegments) {
                direction = vector4 - point2;
            } else if (i == 0) {
                direction = point2 - vector3;
            }
            GenerateCircleAtPoint(vertices, normals, vector6, direction);
            if (i > 0) {
                MakeElbowTriangles(vertices, triangles, i, index);
            }
        }
    }
    private void GenerateEndCaps(List<Vector3> vertices, List<int> triangles, List<Vector3> normals) {
        int num = 0;
        int num2 = (points.Count - 1) * pipeSegments * 2 - pipeSegments;
        vertices.Add(points[0]);
        int item = vertices.Count - 1;
        normals.Add(points[0] - points[1]);
        vertices.Add(points[points.Count - 1]);
        int item2 = vertices.Count - 1;
        normals.Add(points[points.Count - 1] - points[points.Count - 2]);
        for (int i = 0; i < pipeSegments; i++) {
            triangles.Add(item);
            triangles.Add(num + (i + 1) % pipeSegments);
            triangles.Add(num + i);
            triangles.Add(num2 + i);
            triangles.Add(num2 + (i + 1) % pipeSegments);
            triangles.Add(item2);
        }
    }
    public List<Transform> GetPoints() {
        foreach (Transform item in base.transform) {
            pointList.Add(item);
        }
        return pointList;
    }
}
