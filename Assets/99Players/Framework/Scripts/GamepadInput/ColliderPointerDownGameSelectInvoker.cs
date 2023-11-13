using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace io.ninenine.players.party3d.games.common {
#if UNITY_EDITOR
    using UnityEditor;
    public partial class ColliderPointerDownGameSelectInvoker {
        [MenuItem("99Players/AddGameSelectInvoker")]
        public static void AddGameSelectInvoker() {
            CursorButtonObject[] buttonObjects = FindObjectsByType<CursorButtonObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Array.Sort(buttonObjects, (left, right) => String.Compare(left.gameObject.name, right.gameObject.name, StringComparison.Ordinal));
            foreach (var cursorButtonObject in buttonObjects) {
                GameObject root = cursorButtonObject.gameObject;
                if (!root.name.Contains("Debug_GameSelect")) {
                    continue;
                }
                Transform spriteTransform = root.transform.Find("Square");
                if (!spriteTransform) {
                    continue;
                }
                if (!spriteTransform.TryGetComponent(out SpriteRenderer spriteRenderer)) {
                    continue;
                }
                string strippedName = spriteRenderer.sprite.name.Replace("icon_", "");
                // Special cases
                if (strippedName == "hrowingBalls") {
                    strippedName = "ThrowingBalls";
                }
                string enumName = "";
                for (var i = 0; i < strippedName.Length; i++) {
                    char c = strippedName[i];
                    if (char.IsUpper(c) && i != 0) {
                        enumName += "_" + c;
                    }
                    else {
                        enumName += c.ToString().ToUpper();
                    }
                }
                // Special cases
                enumName = enumName.Replace("GIRI_GIRI", "GIRIGIRI");
                enumName = enumName.Replace("JAN_JAN", "JANJAN");
                GS_Define.GameType enumType = Enum.Parse<GS_Define.GameType>(enumName, true);
                Debug.Log($"{root.name}: {spriteRenderer.sprite.name} => {enumType}", root);
                // Remove old component
                if (spriteTransform.TryGetComponent(out SpriteRendererGameSelectButton button)) {
                    DestroyImmediate(button);
                }
                // Add collider
                var circleCollider2D = root.AddComponent<CircleCollider2D>();
                circleCollider2D.radius = Mathf.Min(spriteRenderer.sprite.bounds.size.x / 2, spriteRenderer.sprite.bounds.size.y / 2);
                circleCollider2D.offset = spriteRenderer.sprite.bounds.center;
                // Add new component
                var invoker = root.AddComponent<ColliderPointerDownGameSelectInvoker>();
                invoker.Collider2D = circleCollider2D;
                invoker.GameType = enumType;
            }
        }
    }
#endif
    public partial class ColliderPointerDownGameSelectInvoker : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler {
        #region Inspector
        [Header("Reference")]
        public Collider2D Collider2D;
        [Header("(Optional) Reference")]
        public GS_PartySelect PartySelect;
        [Header("Data")]
        public GS_Define.GameType GameType;
        #endregion
        private void Awake() {
            if (gameObject.TryGetComponent(out Collider2D)) {
                return;
            }
            Debug.LogError($"{gameObject.name} with ColliderPointerDownGameSelectInvoker is missing a Collider2D");
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
            CursorManager cursor;
            if (PartySelect != null) {
                cursor = PartySelect.Cursor;
                // We only select (start) the game if double touched
                if (cursor.GetSelectNo() != gameId) {
                    cursor.SetCursorPos(cursor.GetLayerNo(), gameId);
                    PartySelect.OnMoveButtonDown();
                } else {
                    PartySelect.OnSelectButtonDown();
                }
            }
            else {
                cursor = GS_GameSelectManager.Instance.CursorGameSelect;
                // We only select (start) the game if double touched
                if (cursor.GetSelectNo() != gameId) {
                    cursor.SetCursorPos(cursor.GetLayerNo(), gameId);
                    GS_GameSelectManager.Instance.OnMoveButtonDown();
                } else {
                    GS_GameSelectManager.Instance.OnSelectButtonDown();
                }
            }
        }
    }
}