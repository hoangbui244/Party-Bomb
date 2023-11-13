using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace io.ninenine.players.party3d.games.common {
    public class SpriteRendererGameSettingsButton:MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler{
        public enum ButtonBehaviour {
            Left,
            Right
        }
        [Header("Reference")]
        public SpriteRenderer SpriteRenderer;
        [Header("Runtime")]
        public Collider2D Collider2D;
        public ButtonBehaviour state;
        [Header("Tab Number")]
        public int index;
        private void Awake() {
            SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            // Only add collider to sprite renderer if it not already exist
            if(gameObject.TryGetComponent(out Collider2D)) {
                return;
            }
            var circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = Mathf.Min(SpriteRenderer.sprite.bounds.size.x / 2,SpriteRenderer.sprite.bounds.size.y / 2);
            circleCollider.offset = SpriteRenderer.sprite.bounds.center;
            Collider2D = circleCollider;
        }
        public void OnPointerDown(PointerEventData eventData) {
            // TODO: Make select cursor of CursorManager to select this
        }
        public void OnPointerExit(PointerEventData eventData) {
            eventData.pointerPress = null;
        }
        public void OnPointerUp(PointerEventData eventData) {
            if(eventData.pointerPress != gameObject) {
                return;
            }
            switch(state) {
                case ButtonBehaviour.Left:
                    SingletonCustom<GS_Setting>.Instance.MoveSpecificLeft(index);
                    break;
                case ButtonBehaviour.Right:
                    SingletonCustom<GS_Setting>.Instance.MoveSpecificRight(index);
                    break;
            }
        }
    }
}
