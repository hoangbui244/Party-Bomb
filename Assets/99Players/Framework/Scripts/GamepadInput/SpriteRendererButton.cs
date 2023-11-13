using UnityEngine;
using UnityEngine.EventSystems;
namespace io.ninenine.players.party3d.games.common {
    public class SpriteRendererButton : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler {
        private bool m_IsInitialzed;
        private SpriteRendererButtonController m_Controller;
        public void Initialize(SpriteRendererButtonController controller) {
            if (m_IsInitialzed) {
                return;
            }
            m_IsInitialzed = true;
            m_Controller = controller;
            // Only add collider to sprite renderer if it not already exist
            if (gameObject.TryGetComponent(out Collider2D collider2D)) {
                return;
            }
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            var boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = spriteRenderer.sprite.bounds.size;
            boxCollider.offset = spriteRenderer.sprite.bounds.center;
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
            m_Controller.InvokeButtonMouseDown(this);
        }
    }
}