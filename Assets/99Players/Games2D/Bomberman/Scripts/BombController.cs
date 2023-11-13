using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
namespace io.ninenine.players.party2d.games.bomberman {
    public class BombController : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        private Vector2 pushDir;
        /// <summary>
        /// 
        /// </summary>
        private Vector2 flyDir;
        /// <summary>
        /// Prefab
        /// </summary>
        public BombExplosion ExplosionPrefab;
        /// <summary>
        /// Thời gian hiệu ứng nổ tồn tại
        /// </summary>
        public float ExplosionDuration = 1f;
        /// <summary>
        /// Phạm vi bom nổ
        /// Khi nhặt PowerUp thì phạm vi tăng
        /// </summary>
        public int Radius = 0;
        /// <summary>
        /// Thời gian bom nổ
        /// </summary>
        public float BombFuseTime = 3f;
        /// <summary>
        /// 
        /// </summary>
        public float BombSpeed = 3f;
        /// <summary>
        /// 
        /// </summary>
        public LayerMask ExplosionLayerMask;
        /// <summary>
        /// 
        /// </summary>
        public bool IsPushing = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsFlying = false;
        /// <summary>
        /// 
        /// </summary>
        private Animator animator;
        /// <summary>
        /// 
        /// </summary>
        private Vector3 bombPos;
        /// <summary>
        /// 
        /// </summary>
        private Vector3 target;
        /// <summary>
        /// 
        /// </summary>
        private Coroutine stop;
        /// <summary>
        /// 
        /// </summary>
        private void Awake() {
            stop = StartCoroutine(DestroyBomb());
            animator = GetComponent<Animator>();
            animator.SetBool("IsBouncing", false);
        }
        /// <summary>
        /// 
        /// </summary>
        private IEnumerator DestroyBomb() {
            Vector3 position = transform.position;
            yield return new WaitForSeconds(BombFuseTime);
            position = this.transform.position;
            position.x = Mathf.Round(position.x);
            position.y = Mathf.Round(position.y);
            position.z = BombermanGameControler.getInstance().rootParent.position.z;
            BombExplosion explosion = Instantiate(ExplosionPrefab, position, Quaternion.identity, BombermanGameControler.getInstance().rootParent);
            explosion.transform.localPosition = position;
            explosion.SetActiveRenderer(explosion.Start);
            explosion.DestroyAfter(ExplosionDuration);
            Explode(position, Vector2.up, Radius);
            Explode(position, Vector2.down, Radius);
            Explode(position, Vector2.left, Radius);
            Explode(position, Vector2.right, Radius);
            Destroy(gameObject);
        }
        /// <summary>
        /// 
        /// </summary>
        private void Update() {
            if (IsPushing) {
                Collider2D[] results = new Collider2D[1];
                ContactFilter2D filter = new ContactFilter2D();
                filter.SetLayerMask(LayerMask.GetMask("Wall", "Character", "Enemy", "IgnoreWall"));
                int count = Physics2D.OverlapCollider(GetComponent<Collider2D>(), filter, results);
                if (count > 0) {
                    return;
                }
                transform.Translate(pushDir * BombSpeed * Time.deltaTime);
            }
            if (IsFlying) {
                StopCoroutine(stop);
                animator.SetBool("IsBouncing", true);
                transform.position = Vector3.MoveTowards(transform.position, target, BombSpeed * Time.deltaTime);
                if (transform.position == target) {
                    StartCoroutine(DestroyBomb());
                    animator.SetBool("IsBouncing", false);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void Explode(Vector2 position, Vector2 direction, int length) {
            while (length > 0) {
                position += direction;
                Collider2D collider = Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, ExplosionLayerMask);
                if (collider != null && collider.gameObject.CompareTag("Wall")) {
                    collider.gameObject.GetComponent<Destructibles>().IsDestroyed();
                    return;
                }
                else if (collider != null && collider.gameObject.CompareTag("NoHit")) {
                    return;
                }
                BombExplosion explosion = Instantiate(ExplosionPrefab, position, Quaternion.identity, BombermanGameControler.getInstance().rootParent);
                explosion.transform.localPosition = position;
                explosion.SetActiveRenderer(length > 1 ? explosion.Middle : explosion.End);
                explosion.SetDirection(direction);
                explosion.DestroyAfter(ExplosionDuration);
                length--;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Push(Vector2 pushDirection) {
            pushDir = pushDirection;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Fly(Vector2 flyDirection) {
            IsFlying = true;
            flyDir = flyDirection;
            bombPos = transform.position;
            target = bombPos;
            int i = 1;
            while (true) {
                Collider2D[] colliders = Physics2D.OverlapPointAll(target);
                if (colliders == null || colliders.Length == 0) {
                    break;
                }
                bool foundCollidable = false;
                foreach (Collider2D collider in colliders) {
                    if (collider.gameObject.CompareTag("Grenade") || collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("NoHit")) {
                        foundCollidable = true;
                        break;
                    }
                }
                if (!foundCollidable) {
                    break;
                }
                i++;
                if (flyDir.x < 0 && Mathf.Abs(flyDir.x) > Mathf.Abs(flyDir.y)) {
                    target.x = bombPos.x - i;
                }
                else if (flyDir.x > 0 && Mathf.Abs(flyDir.x) > Mathf.Abs(flyDir.y)) {
                    target.x = bombPos.x + i;
                }
                else if (flyDir.y < 0 && Mathf.Abs(flyDir.y) > Mathf.Abs(flyDir.x)) {
                    target.y = bombPos.y - i;
                }
                else if (flyDir.y > 0 && Mathf.Abs(flyDir.y) > Mathf.Abs(flyDir.x)) {
                    target.y = bombPos.y + i;
                }

                target.z = BombermanGameControler.getInstance().rootParent.position.z;
            }
        }
    }
}