using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    public class BombExplosion : MonoBehaviour {
        /// <summary>
        /// hoạt ảnh bomb render (start)
        /// </summary>
        public AnimatedSpriteRenderer Start;
        /// <summary>
        /// hoạt ảnh bomb render (middle)
        /// </summary>
        public AnimatedSpriteRenderer Middle;
        /// <summary>
        /// hoạt ảnh bomb render (end)
        /// </summary>
        public AnimatedSpriteRenderer End;
        /// <summary>
        /// Hàm kích hoạt render hoạt ảnh
        /// quyết định xem enable trạng thái nào của bomb nổ
        /// </summary>
        public void SetActiveRenderer(AnimatedSpriteRenderer renderer) {
            Start.enabled = renderer == Start;
            Middle.enabled = renderer == Middle;
            End.enabled = renderer == End;
        }
        /// <summary>
        /// Rotate hoạt ảnh bomb nổ ra các hướng
        /// </summary>
        public void SetDirection(Vector2 direction) {
            // xoay hướng bomb nổ
            float angle = Mathf.Atan2(direction.y, direction.x);
            transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        }
        /// <summary>
        /// Hàm xóa gameObject (Làm gọn code)
        /// </summary>
        public void DestroyAfter(float seconds) {
            Destroy(gameObject, seconds);
        }
    }
}
