using System;
using UnityEngine;
namespace io.ninenine.players.party3d.games.ui {
#if UNITY_EDITOR
    public partial class ScreenAnchor {
        private void OnValidate() {
            UpdatePosition();
            UpdateScale();
        }
    }
#endif
    public enum HorizontalPosition {
        MIDDLE,
        LEFT,
        RIGHT
    }
    public enum VerticalPosition {
        MIDDLE,
        TOP,
        BOTTOM
    }
    public enum StretchType {
        FIT,
        FILL,
        STRETCH,
    }
    public partial class ScreenAnchor : MonoBehaviour {
        #region Inspector
        [field: Header("References")]
        [field: SerializeField] public Transform CenterTransform { get; private set; }
        /// <summary>
        /// Check this box to enable updating mode (instead of only update this game object position once at Start)
        /// </summary>
        [field: Header("Data")]
        [field: SerializeField] public Vector2 ScaleRatio { get; private set; }
        [field: SerializeField] public VerticalPosition VerticalPosition { get; private set; }
        [field: SerializeField] public HorizontalPosition HorizontalPosition { get; private set; }
        [field: SerializeField] public StretchType StretchType { get; private set; }
        [field: SerializeField] public bool IsUpdating { get; private set; }
        #endregion
        private void Start() {
            UpdatePosition();
            UpdateScale();
        }
        private void Update() {
#if !UNITY_EDITOR
            if (!IsUpdating) {
                return;
            }
#endif
            UpdatePosition();
            UpdateScale();
        }
        private void UpdatePosition() {
            float anchorX = CenterTransform.position.x;
            float anchorY = CenterTransform.position.y;
            float gameViewWidth = GlobalCameraManager.Instance.GetWidth();
            float gameViewHeight = GlobalCameraManager.Instance.GetHeight();
            switch (HorizontalPosition) {
                case HorizontalPosition.MIDDLE:
                    break;
                case HorizontalPosition.LEFT:
                    anchorX -= gameViewWidth / 2f;
                    break;
                case HorizontalPosition.RIGHT:
                    anchorX += gameViewWidth / 2f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (VerticalPosition) {
                case VerticalPosition.MIDDLE:
                    break;
                case VerticalPosition.TOP:
                    anchorY += gameViewHeight / 2f;
                    break;
                case VerticalPosition.BOTTOM:
                    anchorY -= gameViewHeight / 2f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            transform.position = new Vector3(anchorX, anchorY, transform.position.z);
        }
        private void UpdateScale() {
            float gameViewWidth = GlobalCameraManager.Instance.GetWidth();
            float gameViewHeight = GlobalCameraManager.Instance.GetHeight();
            float scaleX = gameViewWidth / ScaleRatio.x;
            float scaleY = gameViewHeight / ScaleRatio.y;
            float scaleShared = 1f;
            switch (StretchType) {
                case StretchType.FIT:
                    scaleShared = Mathf.Min(scaleX, scaleY);
                    scaleX = scaleY = scaleShared;
                    break;
                case StretchType.FILL:
                    scaleShared = Mathf.Max(scaleX, scaleY);
                    scaleX = scaleY = scaleShared;
                    break;
                case StretchType.STRETCH:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
    }
}