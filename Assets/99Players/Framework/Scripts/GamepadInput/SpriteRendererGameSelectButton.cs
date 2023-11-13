using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace io.ninenine.players.party3d.games.common {
    public class SpriteRendererGameSelectButton : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler {
        #region Inspector
        [Header("Reference")]
        public SpriteRenderer SpriteRenderer;
        [Header("Data")]
        public GS_Define.GameType GameType;
        [Header("Runtime")]
        public Collider2D Collider2D;
        #endregion
        private void Awake() {
            SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            // Only add collider to sprite renderer if it not already exist
            if (gameObject.TryGetComponent(out Collider2D)) {
                return;
            }
            var circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = Mathf.Min(SpriteRenderer.sprite.bounds.size.x / 2, SpriteRenderer.sprite.bounds.size.y / 2);
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
            if (eventData.pointerPress != gameObject) {
                return;
            }
            int gameId = Array.IndexOf(GS_GameSelectManager.Instance.ArrayCursorGameType, GameType);
            GS_GameSelectManager.Instance.Select(gameId);
        }
    }
}