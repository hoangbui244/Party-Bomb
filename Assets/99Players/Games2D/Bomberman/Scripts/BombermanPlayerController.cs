using ControlFreak2;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace io.ninenine.players.party2d.games.bomberman {
    public class BombermanPlayerController : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        public BombermanPlayer BombermanPlayer;
        /// <summary>
        /// 
        /// </summary>
        public BombermanPlayerAI Bot;
        /// <summary>
        /// 
        /// </summary>
        public bool CharacInvincible;
        /// <summary>
        /// 
        /// </summary>
        public GameObject BombPrefab;
        /// <summary>
        /// 
        /// </summary>
        public float CharacSpeed = 2f;
        /// <summary>
        /// Số lượng bomb tối đa người chơi có thể giữ
        /// </summary>
        public int MaxBomb = 6;
        /// <summary>
        /// Phạm vi bom nổ
        /// Khi nhặt PowerUp thì phạm vi tăng
        /// </summary>
        public int ExplosionRadius = 2;
        /// <summary>
        /// Số lượng bomb hiện tại người chơi có
        /// </summary>
        public int CurrentBombHold = 2;
        /// <summary>
        /// 
        /// </summary>
        public int CharacterLives = 1;
        /// <summary>
        /// 
        /// </summary>
        public int PlayerIdx => BombermanPlayer.PlayerIdx;
        /// <summary>
        /// 
        /// </summary>
        public bool Pushable = false;
        /// <summary>
        /// 
        /// </summary>
        public bool Throwable = false;
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public float horizontal;
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public float vertical;
        /// <summary>
        /// 
        /// </summary>
        public bool IsMoving = false;
        /// <summary>
        /// 
        /// </summary>
        private Rigidbody2D rb;
        /// <summary>
        /// 
        /// </summary>
        private float timeHeld = 0f;
        /// <summary>
        /// 
        /// </summary>
        private void Awake() {
            BombermanPlayer = GetComponent<BombermanPlayer>();
            Bot = GetComponent<BombermanPlayerAI>();
            rb = GetComponent<Rigidbody2D>();
        }
        /// <summary>
        /// 
        /// </summary>
        void Start() {
            BombermanGameManager.Instance.PlayerList.Add(this.gameObject);
        }
        /// <summary>
        /// 
        /// </summary>
        void Update() {
            if (BombermanPlayer.IsCpu) {
                if (GameFlow.instance.IsReady) {
                    AIMove();
                }
            }
            else {
                CharacterMove();
                PlantBomb();
                if (Pushable) {
                    BombPushable();
                }
                if (Throwable) {
                    BombThrowable();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void CharacterMove() {
            if (GameFlow.instance.IsReady) {
                if (PlayerIdx == -1) {
                    return;
                }
                IsMoving = true;
                Vector2 Dir = BombermanControllerManager.GetStickDirRaw(PlayerIdx).normalized;
                if (Dir.x == 0 && Dir.y == 0) {
                    rb.velocity = new Vector2(0, 0);
                    IsMoving = false;
                    return;
                }
                horizontal = Dir.x;
                vertical = Dir.y;
                rb.velocity = new Vector2(horizontal * CharacSpeed, vertical * CharacSpeed);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void AIMove() {
            if (GameFlow.instance.IsReady) {
                float timeMove = 0f;
                float timeStop = Random.Range(0, 2.2f);
                timeMove += Time.deltaTime;
                if (PlayerIdx == -1) {
                    return;
                }
                IsMoving = true;
                if (timeMove > timeStop) {
                    horizontal = Random.Range(-1f, 1f);
                    vertical = Random.Range(-1f, 1f);
                    timeMove = 0f;
                }
                rb.velocity = new Vector2(horizontal * CharacSpeed, vertical * CharacSpeed);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void PlantBomb() {
            if (GameFlow.instance.IsReady) {
                if (CurrentBombHold > 0 && IsPositionValid()) {
                    if (BombermanControllerManager.GetButtonDown(PlayerIdx, GamepadInput.SatGamePad.Button.A)) {
                        StartCoroutine(BombReturn());
                        Vector3 position = this.transform.position;
                        position.x = Mathf.Round(position.x);
                        position.y = Mathf.Round(position.y);
                        position.z = BombermanGameControler.getInstance().rootParent.position.z;
                        GameObject Bomb = Instantiate(BombPrefab, position, Quaternion.identity, BombermanGameControler.getInstance().rootParent);
                        Bomb.GetComponent<BombController>().Radius = ExplosionRadius;
                        CurrentBombHold--;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private bool IsPositionValid() {
            Vector3 position = transform.position;
            position.z = 0;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
            foreach (Collider2D collider in colliders) {
                if (collider.gameObject.CompareTag("Grenade")) {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerator BombReturn() {
            yield return new WaitForSeconds(3.05f);
            CurrentBombHold++;
        }
        /// <summary>
        /// 
        /// </summary>
        public void ActiveGhostMode() {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Wall"), true);
            StartCoroutine(SwitchGhostMode(BombermanGameData.Instance.GhostTime));
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerator DeActiveGhostMode() {
            bool IsOverlapped = true;
            while (IsOverlapped) {
                if (!TryGetComponent<Collider2D>(out var collider)) {
                }
                List<Collider2D> results = new List<Collider2D>();
                Physics2D.OverlapCollider(collider, new ContactFilter2D().NoFilter(), results);
                IsOverlapped = false;
                foreach (var other in results) {
                    if (other.gameObject.layer == LayerMask.NameToLayer("Wall")) {
                        IsOverlapped = true;
                        break;
                    }
                }
                yield return null;
            }
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Wall"), false);
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerator SwitchGhostMode(float delay) {
            yield return new WaitForSeconds(delay);
            yield return DeActiveGhostMode();
        }
        /// <summary>
        /// 
        /// </summary>
        public void BombThrowable() {
            Vector2 flyDirection = new Vector2(horizontal, vertical);
            Vector2 position = transform.position;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
            bool isButtonPressed = BombermanControllerManager.GetButton(PlayerIdx, GamepadInput.SatGamePad.Button.A);
            if (isButtonPressed) {
                timeHeld += Time.deltaTime;
            }
            else if (timeHeld >= 1.2f) {
                foreach (Collider2D collider in colliders) {
                    if (collider.gameObject.CompareTag("Grenade")) {
                        collider.gameObject.GetComponent<BombController>().Fly(flyDirection);
                        break;
                    }
                }
                timeHeld = 0;
            }
            else {
                timeHeld = 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Shielded() {
            StartCoroutine(HadShield());
        }
        public IEnumerator HadShield() {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Color original = spriteRenderer.color;
            CharacInvincible = true;
            float blinkDuration = BombermanGameData.Instance.ShieldTime;
            float blinkInterval = 0.15f;
            float elapsedTime = 0f;
            while (elapsedTime < blinkDuration) {
                Color newColor = new Color(0.2f, original.g, original.b);
                spriteRenderer.color = newColor;
                yield return new WaitForSeconds(blinkInterval);
                newColor = new Color(original.r, 0.2f, original.b);
                spriteRenderer.color = newColor;
                yield return new WaitForSeconds(blinkInterval);
                newColor = new Color(original.r, original.g, 0.2f);
                spriteRenderer.color = newColor;
                yield return new WaitForSeconds(blinkInterval);
                spriteRenderer.color = original;
                yield return new WaitForSeconds(blinkInterval);
                elapsedTime += blinkInterval * 4f;
            }
            spriteRenderer.color = original;
            CharacInvincible = false;
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerator TimePushLeft() {
            yield return new WaitForSeconds(12);
            Pushable = false;
        }
        /// <summary>
        /// 
        /// </summary>
        public void CanPushBomb() {
            Pushable = true;
            StartCoroutine(TimePushLeft());
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerator TimeThrowLeft() {
            yield return new WaitForSeconds(8);
            Throwable = false;
        }
        /// <summary>
        /// 
        /// </summary>
        public void CanThrowBomb() {
            Throwable = true;
            StartCoroutine(TimeThrowLeft());
        }
        /// <summary>
        /// 
        /// </summary>
        public void BombPushable() {
            Vector2 pushDirection = new Vector2(horizontal, vertical);
            bool previousState = Physics2D.queriesStartInColliders;
            Physics2D.queriesStartInColliders = false;
            RaycastHit2D[] hits = new RaycastHit2D[1];
            int hitCount = Physics2D.RaycastNonAlloc(transform.position, pushDirection, hits);
            Physics2D.queriesStartInColliders = previousState;
            if (hitCount > 0) {
                if (hits[0].collider.gameObject.CompareTag("Grenade")) {
                    hits[0].collider.gameObject.GetComponent<BombController>().IsPushing = true;
                    hits[0].collider.gameObject.GetComponent<BombController>().Push(pushDirection);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void OnTriggerEnter2D(Collider2D collision) {
            if ((collision.gameObject.CompareTag("Explode") || collision.gameObject.CompareTag("Failure")) && !CharacInvincible) {
                if (CharacterLives > 0) {
                    CharacterLives--;
                    StartCoroutine(BlinkingEffect());
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && !CharacInvincible) {
                if (CharacterLives > 0) {
                    CharacterLives--;
                    StartCoroutine(BlinkingEffect());
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerator BlinkingEffect() {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Color original = spriteRenderer.color;
            CharacInvincible = true;
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
            CharacInvincible = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void OnTriggerExit2D(Collider2D other) {
            if (other.gameObject.CompareTag("Grenade")) {
                other.isTrigger = false;
            }
        }
    }
}

