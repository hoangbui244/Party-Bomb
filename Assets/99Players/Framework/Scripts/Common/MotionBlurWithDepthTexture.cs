using UnityEngine;
public class MotionBlurWithDepthTexture : MonoBehaviour {
    public Shader motionBlurShader;
    private Material curMaterial;
    private Camera myCamera;
    [Range(0f, 1f)]
    public float blurSize = 0.5f;
    private Matrix4x4 previousViewProjectionMatrix;
    public Material material {
        get {
            if (curMaterial == null) {
                curMaterial = new Material(motionBlurShader);
                curMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return curMaterial;
        }
    }
    public Camera camera {
        get {
            if (myCamera == null) {
                myCamera = GetComponent<Camera>();
            }
            return myCamera;
        }
    }
    private void OnEnable() {
        camera.depthTextureMode |= DepthTextureMode.Depth;
        previousViewProjectionMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;
    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (material != null) {
            material.SetFloat("_BlurSize", blurSize);
            material.SetMatrix("_PreviousViewProjectionMatrix", previousViewProjectionMatrix);
            Matrix4x4 matrix4x = camera.projectionMatrix * camera.worldToCameraMatrix;
            Matrix4x4 inverse = matrix4x.inverse;
            material.SetMatrix("_CurrentViewProjectionInverseMatrix", inverse);
            previousViewProjectionMatrix = matrix4x;
            Graphics.Blit(src, dest, material);
        } else {
            Graphics.Blit(src, dest);
        }
    }
}
