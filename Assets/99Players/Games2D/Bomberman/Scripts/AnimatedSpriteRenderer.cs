using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    public class AnimatedSpriteRenderer : MonoBehaviour {
        /// <summary>
        /// idleSprites
        /// </summary>
        public Sprite IdleSprites;
        /// <summary>
        /// array of Sprites
        /// </summary>
        public Sprite[] AnimationSprites;
        /// <summary>
        /// có thể thay đổi giá trị
        /// </summary>
        public float AnimationTime = 0.25f;
        /// <summary>
        /// check xem should it loop over and over again
        /// </summary>
        public bool Loop = true;
        /// <summary>
        /// switch to the idle
        /// </summary>
        public bool Idle = true;
        /// <summary>
        /// Khai báo
        /// </summary>
        private SpriteRenderer spriteRenderer;
        /// <summary>
        /// giá trị để check xem đang là frame nào chạy
        /// </summary>
        private int animationFrame;
        /// <summary>
        /// Hàm Awake
        /// GetComponent của SpriteRenderer
        /// </summary>
        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        /// <summary>
        /// Điều khiển hiển thị sprite
        /// </summary>
        private void OnEnable() {
            spriteRenderer.enabled = true;
        }
        private void OnDisable() {
            spriteRenderer.enabled = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void Start() {
            /// <summary>
            /// Điều khiển hiển thị sprite
            /// 0.25f default => 1s 4 khung hình
            /// </summary>
            InvokeRepeating(nameof(NextFrame), AnimationTime, AnimationTime);
        }
        /// <summary>
        /// 
        /// </summary>
        private void NextFrame() {
            animationFrame++;
            /// Kiểm tra xem loop và khung hình chuyển động >= độ dài sprites
            if (Loop && animationFrame >= AnimationSprites.Length) {
                animationFrame = 0;
            }
            /// Nếu sprites ở trạng thái nhàn rỗi (idle)
            if (Idle) {
                spriteRenderer.sprite = IdleSprites;
            }
            /// Check điều kiện
            else if (animationFrame >= 0 && animationFrame < AnimationSprites.Length) {
                spriteRenderer.sprite = AnimationSprites[animationFrame];
            }
        }
    }
}