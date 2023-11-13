using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class BottleLiquid : MonoBehaviour {
    private static class ShaderPropertyId {
        public static readonly int BottleLiquidWaveCenter = Shader.PropertyToID("_WaveCenter");
        public static readonly int BottleLiquidWaveParams = Shader.PropertyToID("_WaveParams");
        public static readonly int BottleLiquidColorForward = Shader.PropertyToID("_LiquidColorForward");
        public static readonly int BottleLiquidColorBack = Shader.PropertyToID("_LiquidColorBack");
    }
    private static readonly Color LiquidColorTopOffset = new Color(0.15f, 0.15f, 0.15f, 0f);
    [SerializeField]
    private Color liquidColor;
    [SerializeField]
    private Vector3[] bottleSizeOffsetPoints;
    [Range(0f, 1f)]
    [SerializeField]
    private float fillingRate = 0.5f;
    [Range(0f, 2f)]
    [SerializeField]
    private float positionInfluenceRate = 0.7f;
    [Range(0f, 2f)]
    [SerializeField]
    private float rotationInfluenceRate = 0.4f;
    [Range(0f, 1f)]
    [SerializeField]
    private float sizeAttenuationRate = 0.92f;
    [Range(0f, 1f)]
    [SerializeField]
    private float cycleAttenuationRate = 0.97f;
    [SerializeField]
    private float cycleOffsetCoef = 12f;
    [SerializeField]
    private float deltaSizeMax = 0.15f;
    [SerializeField]
    private float deltaCycleMax = 10f;
    private Material[] targetMaterials;
    private Vector3 prevPosition;
    private Vector3 prevEulerAngles;
    private Vector4 waveCurrentParams;
    private void Start() {
        Renderer component = GetComponent<Renderer>();
        if (component == null) {
            return;
        }
        if (targetMaterials == null || targetMaterials.Length == 0) {
            List<Material> list = new List<Material>();
            for (int i = 0; i < component.sharedMaterials.Length; i++) {
                Material material = component.sharedMaterials[i];
                if (material.shader.name.Contains("BottleLiquid")) {
                    list.Add(material);
                }
            }
            targetMaterials = list.ToArray();
        }
        waveCurrentParams = Vector4.zero;
        BackupTransform();
    }
    private void Update() {
        if (targetMaterials != null && targetMaterials.Length != 0) {
            SetupMaterials();
            BackupTransform();
        }
    }
    private void CalculateWaveParams() {
        Vector4 b = new Vector4(sizeAttenuationRate, cycleAttenuationRate, 0f, 0f);
        Vector4 vector = new Vector4(deltaSizeMax, deltaCycleMax, 0f, 0f);
        waveCurrentParams = Vector4.Scale(waveCurrentParams, b);
        Transform transform = base.transform;
        Vector3 eulerAngles = transform.eulerAngles;
        Vector3 vector2 = transform.position - prevPosition;
        Vector3 vector3 = new Vector3(Mathf.DeltaAngle(eulerAngles.x, prevEulerAngles.x), Mathf.DeltaAngle(eulerAngles.y, prevEulerAngles.y), Mathf.DeltaAngle(eulerAngles.z, prevEulerAngles.z));
        waveCurrentParams += vector * (vector2.magnitude * positionInfluenceRate);
        waveCurrentParams += vector * (vector3.magnitude * rotationInfluenceRate * 0.01f);
        waveCurrentParams = Vector4.Min(waveCurrentParams, vector);
        waveCurrentParams.z = cycleOffsetCoef;
    }
    private Vector4 CalculateWaveCenter() {
        (float, float) liquidSurfaceHeight = GetLiquidSurfaceHeight();
        return base.transform.position + Vector3.up * Mathf.Lerp(liquidSurfaceHeight.Item1, liquidSurfaceHeight.Item2, fillingRate);
    }
    private void SetupMaterials() {
        Vector4 value = CalculateWaveCenter();
        for (int i = 0; i < targetMaterials.Length; i++) {
            Material obj = targetMaterials[i];
            obj.SetVector(ShaderPropertyId.BottleLiquidWaveCenter, value);
            obj.SetVector(ShaderPropertyId.BottleLiquidWaveParams, waveCurrentParams);
            obj.SetColor(ShaderPropertyId.BottleLiquidColorForward, liquidColor);
            obj.SetColor(ShaderPropertyId.BottleLiquidColorBack, liquidColor + LiquidColorTopOffset);
        }
    }
    private void BackupTransform() {
        prevPosition = base.transform.position;
        prevEulerAngles = base.transform.eulerAngles;
    }
    private (float min, float max) GetLiquidSurfaceHeight() {
        if (bottleSizeOffsetPoints == null || bottleSizeOffsetPoints.Length == 0) {
            return (0f, 0f);
        }
        Transform transform = base.transform;
        (float, float) valueTuple = (float.MaxValue, float.MinValue);
        for (int i = 0; i < bottleSizeOffsetPoints.Length; i++) {
            Vector3 vector = transform.TransformPoint(bottleSizeOffsetPoints[i]) - transform.position;
            valueTuple.Item1 = Mathf.Min(valueTuple.Item1, vector.y);
            valueTuple.Item2 = Mathf.Max(valueTuple.Item2, vector.y);
        }
        return valueTuple;
    }
    public void SetBottleSizeOffsetPoints(Vector3[] _points) {
        bottleSizeOffsetPoints = _points;
    }
    public void SetFillingRate(float _rate) {
        fillingRate = _rate;
    }
    public void SetWaveCurrentParams(Vector4 _params) {
        waveCurrentParams = _params;
    }
}
