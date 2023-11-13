using UnityEngine;
public class MotionBlurEffect : MonoBehaviour {
    [Range(0f, 0.95f)]
    public float blurAmount = 0.8f;
    public bool extraBlur;
    public Shader curShader;
    private Material curMaterial;
    private RenderTexture tempRT;
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
            if (tempRT == null || tempRT.width != source.width || tempRT.height != source.height) {
                UnityEngine.Object.DestroyImmediate(tempRT);
                tempRT = new RenderTexture(source.width, source.height, 0);
                tempRT.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(source, tempRT);
            }
            if (extraBlur) {
                RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
                tempRT.MarkRestoreExpected();
                Graphics.Blit(tempRT, temporary);
                Graphics.Blit(temporary, tempRT);
                RenderTexture.ReleaseTemporary(temporary);
            }
            material.SetTexture("_MainTex", tempRT);
            material.SetFloat("_BlurAmount", 1f - blurAmount);
            Graphics.Blit(source, tempRT, material);
            Graphics.Blit(tempRT, destination);
        } else {
            Graphics.Blit(source, destination);
        }
    }
}
