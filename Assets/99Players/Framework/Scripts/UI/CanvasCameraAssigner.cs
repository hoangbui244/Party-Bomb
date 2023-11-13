using UnityEngine;
namespace io.ninenine.players.party3d.games.ui {
    public class CanvasCameraAssigner : MonoBehaviour {
        #region Inspector
        [field: Header("References")]
        [field: SerializeField] public Canvas Canvas;
        #endregion
        private void Update() {
            if (Canvas.worldCamera != null) {
                return;
            }
            Camera uiCamera = GlobalCameraManager.Instance.GetMainCamera<Camera>();
            if (GlobalCameraManager.Instance.GetMainCamera<Camera>() == null) {
                return;
            }
            Canvas.worldCamera = uiCamera;
        }
    }
}