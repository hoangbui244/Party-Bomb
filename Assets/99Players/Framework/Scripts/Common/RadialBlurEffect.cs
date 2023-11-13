using UnityEngine;
public class RadialBlurEffect : MonoBehaviour {
    [Range(0f, 0.05f)]
    public float blurDegree = 0.02f;
    public int sampleCount = 3;
    public Vector2 blurCenter = new Vector2(0.5f, 0.5f);
    public Shader curShader;
    private Material curMaterial;
    private Material material {
        get {
            if (curMaterial == null) {
                curMaterial = new Material(curShader);
            }
            return curMaterial;
        }
    }
    private void OnDisable() {
        if ((bool)curMaterial) {
            UnityEngine.Object.DestroyImmediate(curMaterial);
        }
    }
    private void OnDestroy() {
        if ((bool)curMaterial) {
            UnityEngine.Object.DestroyImmediate(curMaterial);
        }
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (curShader != null) {
            material.SetVector("_BlurCenter", blurCenter);
            material.SetFloat("_BlurDegree", blurDegree);
            material.SetInt("_SampleCount", sampleCount);
            Graphics.Blit(source, destination, material);
        } else {
            Graphics.Blit(source, destination);
        }
    }
}
