using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace io.ninenine.players.party2d.games.bomberman {
    public class EnemyBehaviour : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        public enum EnemyState {
            Idle,
            Move
        }
        /// <summary>
        /// 
        /// </summary>
        public EnemyState currentState;
        /// <summary>
        /// 
        /// </summary>
        public float EnemySpeed = 1.4f;
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
        public int EnemyLives = 1;
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
            currentState = Random.Range(0, 2) == 0 ? EnemyState.Idle : EnemyState.Move;
            StartCoroutine(AIBehavior());
        }
        /// <summary>
        /// 
        /// </summary>
        private IEnumerator AIBehavior() {
            while (true) {
                switch (currentState) {
                    case EnemyState.Idle:
                        animator.SetBool("IsIdle", true);
                        yield return new WaitForSeconds(Random.Range(0.2f, 0.6f));
                        break;
                    case EnemyState.Move:
                        animator.SetBool("IsIdle", false);
                        MoveRandomDirection();
                        yield return new WaitForSeconds(Random.Range(1f, 3f));
                        break;
                }
                currentState = Random.Range(0, 2) == 0 ? EnemyState.Idle : EnemyState.Move;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void MoveRandomDirection() {
            int randomDirection = Random.Range(0, 4);
            switch (randomDirection) {
                case 0:
                    moveDirection = Vector2.up;
                    break;
                case 1:
                    moveDirection = Vector2.down;
                    break;
                case 2:
                    moveDirection = Vector2.left;
                    break;
                case 3:
                    moveDirection = Vector2.right;
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void Update() {
            Animated();
            if (currentState == EnemyState.Move) {
                rb.MovePosition(rb.position + moveDirection * EnemySpeed * Time.deltaTime);
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
        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.CompareTag("Explode")) {
                EnemyDeathSequence();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void EnemyDeathSequence() {
            GameFlow.instance.RemoveEnemy(gameObject);
            EnemySpeed = 0;
            animator.SetBool("IsDead", true);
            Destroy(gameObject,1f);
        }
    }
}