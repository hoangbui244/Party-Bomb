using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    public class BossBehaviour : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        public enum BossState {
            Idle,
            Move,
            Skill
        }
        /// <summary>
        /// 
        /// </summary>
        public BossState currentState;
        /// <summary>
        /// 
        /// </summary>
        public bool BossInvincible;
        /// <summary>
        /// 
        /// </summary>
        public float BossSpeed = 2f;
        /// <summary>
        /// 
        /// </summary>
        public GameObject BossSkillPrefab;
        /// <summary>
        /// 
        /// </summary>
        public LayerMask WallLayerMask;
        /// <summary>
        /// 
        /// </summary>
        private Rigidbody2D rb;
        /// <summary>
        /// 
        /// </summary>
        private Vector2 moveDirection;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int BossLives = 10;
        /// <summary>
        /// 
        /// </summary>
        private Animator animator;
        /// <summary>
        /// 
        /// </summary>
        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }
        /// <summary>
        /// 
        /// </summary>
        private void Start() {
            currentState = (BossState)Random.Range(0, 3);
            StartCoroutine(BossBehavior());
        }
        /// <summary>
        /// 
        /// </summary>
        private IEnumerator BossBehavior() {
            while (true) {
                float randomValue = Random.Range(0f, 1f);
                if (randomValue <= 0.2f) {
                    currentState = BossState.Skill;
                }
                switch (currentState) {
                    case BossState.Idle:
                        animator.SetBool("Idle", true);
                        yield return new WaitForSeconds(Random.Range(0.4f, 1f));
                        break;
                    case BossState.Move:
                        animator.SetBool("Idle", false);
                        MoveRandomDirection();
                        yield return new WaitForSeconds(Random.Range(0.6f, 2f));
                        break;
                    case BossState.Skill:
                        Skill();
                        yield return new WaitForSeconds(3f);
                        break;
                }
                currentState = (BossState)Random.Range(0, 3);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void Animated() {
            animator.SetFloat("X", moveDirection.x);
            animator.SetFloat("Y", moveDirection.y);
        }
        /// <summary>
        /// 
        /// </summary>
        private void MoveRandomDirection() {
            int xDirection = Random.Range(-1, 2);
            int yDirection = Random.Range(-1, 2);
            Vector2 currentPosition = transform.position;
            Vector2 targetPosition = currentPosition + new Vector2(xDirection * 3f, yDirection * 3f);
            moveDirection = (targetPosition - currentPosition).normalized;
            Vector2 checkPosition = currentPosition + moveDirection * BossSpeed * Time.deltaTime;
            Collider2D collider = Physics2D.OverlapBox(checkPosition, Vector2.one / 2f, 0f, WallLayerMask);
            if (collider != null) {
                // Nếu có vật cản, chuyển sang trạng thái Idle
                currentState = BossState.Idle;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void Skill() {
            int minX = -16;
            int minY = -6;
            int maxX = 16;
            int maxY = 6;
            moveDirection = Vector2.zero;
            for (int i = 0; i < 16; i++) {
                int randomX = Random.Range(minX, maxX);
                int randomY = Random.Range(minY, maxY);
                if (IsPositionValid(new Vector3(randomX, randomY, 0))) {
                    GameObject BossSkill = Instantiate(BossSkillPrefab, new Vector3(randomX, randomY, BombermanGameControler.getInstance().rootParent.position.z), Quaternion.identity, BombermanGameControler.getInstance().rootParent);
                }
                else {
                    i--;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private bool IsPositionValid(Vector3 position) {
            Vector2 checkPosition = new Vector2(position.x, position.y);
            Collider2D collider = Physics2D.OverlapPoint(checkPosition);
            return collider == null;
        }
        /// <summary>
        /// 
        /// </summary>
        private void Update() {
            Animated();
            if (currentState == BossState.Move) {
                rb.MovePosition(rb.position + moveDirection * BossSpeed * Time.deltaTime);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.CompareTag("Explode") && !BossInvincible) {
                if (BossLives > 0) {
                    BossLives--;
                    StartCoroutine(BlinkingEffect());
                }
                else {
                    BossDie();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void BossDie() {
            BossSpeed = 0;
            animator.SetBool("Dead", true);
            Destroy(gameObject, 1f);
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerator BlinkingEffect() {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Color original = spriteRenderer.color;
            BossInvincible = true;
            float blinkDuration = BombermanGameData.Instance.CharacterInvincibleTime;
            float blinkInterval = 0.1f;
            float elapsedTime = 0f;
            while (elapsedTime < blinkDuration) {
                Color newColor = spriteRenderer.color;
                newColor.a = 0.4f;
                spriteRenderer.color = newColor;
                yield return new WaitForSeconds(blinkInterval);
                spriteRenderer.color = original;
                yield return new WaitForSeconds(blinkInterval);
                elapsedTime += blinkInterval * 2f;
            }
            spriteRenderer.color = original;
            BossInvincible = false;
        }
    }
}