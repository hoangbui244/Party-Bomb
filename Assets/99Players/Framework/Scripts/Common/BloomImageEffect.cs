using UnityEngine;
public class BloomImageEffect : MonoBehaviour {
    public int downSample = 1;
    public int samplerScale = 1;
    public Color colorThreshold = Color.gray;
    public Color bloomColor = Color.white;
    [Range(0f, 1f)]
    public float bloomFactor = 0.5f;
    public Shader curShader;
    private Material curMaterial;
    private Material material {
        get {
            if (curMaterial == null) {
                curMaterial = new Material(curShader);
                curMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return curMaterial;
        }
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (curShader != null) {
            RenderTexture temporary = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0, source.format);
            RenderTexture temporary2 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0, source.format);
            Graphics.Blit(source, temporary);
            material.SetVector("_colorThreshold", colorThreshold);
            Graphics.Blit(temporary, temporary2, material, 0);
            material.SetVector("_offsets", new Vector4(0f, samplerScale, 0f, 0f));
            Graphics.Blit(temporary2, temporary, material, 1);
            material.SetVector("_offsets", new Vector4(samplerScale, 0f, 0f, 0f));
            Graphics.Blit(temporary, temporary2, material, 1);
            material.SetTexture("_BlurTex", temporary2);
            material.SetVector("_bloomColor", bloomColor);
            material.SetFloat("_bloomFactor", bloomFactor);
            Graphics.Blit(source, destination, material, 2);
            RenderTexture.ReleaseTemporary(temporary);
            RenderTexture.ReleaseTemporary(temporary2);
        }
    }
}
