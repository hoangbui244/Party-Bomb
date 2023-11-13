using UnityEngine;
using UnityEngine.Rendering;
public class IsRendered : MonoBehaviour {
    private bool isRendered;
    private Camera camera;
    private Camera currentCamera;
    public bool IsRender => isRendered;
    public void SetCamera(Camera _camera) {
        camera = _camera;
    }
    private void OnEnable() {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }
    private void OnDisable() {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }
    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera _camera) {
        currentCamera = _camera;
    }
    private void Update() {
        bool isRendered2 = isRendered;
        isRendered = false;
    }
    private void OnWillRenderObject() {
        if ((bool)currentCamera && (bool)camera && currentCamera.gameObject == camera.gameObject) {
            isRendered = true;
        }
    }
}
