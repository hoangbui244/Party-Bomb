using UnityEngine;
using UnityEngine.Rendering;
public class CameraPrevFrame : MonoBehaviour {
    [SerializeField]
    [Header("レンダ\u30fcテクスチャ")]
    private RenderTexture _bufferRT;
    private Camera targetCamera;
    private int waitFrame;
    private void Awake() {
        targetCamera = GetComponent<Camera>();
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }
    public void SaveFrameBuffer() {
        _bufferRT.Release();
        targetCamera.targetTexture = _bufferRT;
    }
    private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera) {
        if (targetCamera.targetTexture != null) {
            targetCamera.targetTexture = null;
        }
    }
    private void OnDestroy() {
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }
}
