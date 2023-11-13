using UnityEngine;
[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class DepthTextureEnabler : MonoBehaviour {
    private void OnWillRenderObject() {
        Camera current = Camera.current;
        if (current != null) {
            current.depthTextureMode |= DepthTextureMode.Depth;
        }
    }
}
