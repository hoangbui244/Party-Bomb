using UnityEngine;
using UnityEngine.EventSystems;
namespace io.ninenine.players.party3d.games.common {
    public class ColliderPointerDownInvoker : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler {
        #region
        [Header("Runtime")]
        public Collider2D Collider2D;
        #endregion
        private bool m_IsInitialzed;
        private ColliderPointerDownController m_Controller;
        public void Initialize(ColliderPointerDownController controller) {
            if (m_IsInitialzed) {
                return;
            }
            m_IsInitialzed = true;
            m_Controller = controller;
            if (gameObject.TryGetComponent(out Collider2D)) {
                return;
            }
            Debug.LogError($"{gameObject.name} with ColliderPointerDownInvoker is missing a Collider2D");
        }
        public void OnPointerDown(PointerEventData eventData) {
            // TODO: Make select cursor of CursorManager to select this
        }
        public void OnPointerExit(PointerEventData eventData) {
            eventData.pointerPress = null;
        }
        public void OnPointerUp(PointerEventData eventData) {
            if (eventData.pointerPress != gameObject) {
                return;
            }
            Debug.Log($"OnPointerDown: {gameObject.name} at {transform.position}", gameObject);
            m_Controller.InvokeButtonMouseDown(Collider2D);
        }
    }
}