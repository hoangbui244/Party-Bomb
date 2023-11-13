using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    [System.Serializable]
    public class CharacterSprites {
        public Sprite[] upIdleSprites;
        public Sprite[] downIdleSprites;
        public Sprite[] leftIdleSprites;
        public Sprite[] rightIdleSprites;
        public Sprite[] upMoveSprites;
        public Sprite[] downMoveSprites;
        public Sprite[] leftMoveSprites;
        public Sprite[] rightMoveSprites;
        public Sprite[] dieSprites;
    }
    public class PlayerAnimatedSprites : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        public CharacterSprites[] CharactersSprites;
        /// <summary>
        /// 
        /// </summary>
        public enum Direction {
            Up,
            Down,
            Left,
            Right
        }
        /// <summary>
        /// 
        /// </summary>
        public enum State {
            Idle,
            Move
        }
        /// <summary>
        /// 
        /// </summary>
        public BombermanPlayerController playerController;
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public BombermanPlayer player;

        public float surTime;
        /// <summary>
        /// 
        /// </summary>
        public Direction currentDirection;
        /// <summary>
        /// 
        /// </summary>
        public State currentState;
        /// <summary>
        /// 
        /// </summary>
        public float frameRate = 0.125f;
        /// <summary>
        /// 
        /// </summary>
        public bool flipSpritesForRight = true;
        /// <summary>
        /// 
        /// </summary>
        private SpriteRenderer spriteRenderer;
        /// <summary>
        /// 
        /// </summary>
        private int currentFrame;
        /// <summary>
        /// 
        /// </summary>
        public int charactersSpritesIdx;
        /// <summary>
        /// 
        /// </summary>
        private float timer;
        /// <summary>
        /// 
        /// </summary>
        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerController = GetComponent<BombermanPlayerController>();
        }
        /// <summary>
        /// 
        /// </summary>
        private void Start() {
            currentFrame = 0;
            timer = 0f;
        }
        /// <summary>
        /// 
        /// </summary>
        private void Update() {
            timer += Time.deltaTime;
            if (playerController.IsMoving) {
                currentState = State.Move;
                currentDirection = GetDirectionFromVector(new Vector2(playerController.horizontal, playerController.vertical));
            }
            else {
                currentState = State.Idle;
            }
            FlipSpritesForRight();
            if (playerController.CharacterLives <= 0) {
                DieAnimation();
                BombermanPlayerManager.Instance.playerSurTime[playerController.PlayerIdx] = GameFlow.instance.timeSur - GameFlow.instance.timeMatch;
            }
            else {
                BombermanPlayerManager.Instance.playerSurTime[playerController.PlayerIdx] = GameFlow.instance.timeSur;
                if (currentState == State.Idle) {
                    PlayAnimation(GetSpritesForDirection(currentDirection, State.Idle, charactersSpritesIdx));
                }
                else if (currentState == State.Move) {
                    PlayAnimation(GetSpritesForDirection(currentDirection, State.Move, charactersSpritesIdx));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void PlayAnimation(Sprite[] sprites) {
            if (sprites.Length > 0) {
                currentFrame = Mathf.Clamp(currentFrame, 0, sprites.Length - 1);
                spriteRenderer.sprite = sprites[currentFrame];
                if (timer >= frameRate) {
                    timer -= frameRate;
                    currentFrame = (currentFrame + 1) % sprites.Length;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private Sprite[] GetSpritesForDirection(Direction direction, State state, int charactersSpritesIdx) {
            CharacterSprites characterSprites = CharactersSprites[charactersSpritesIdx];
            switch (direction) {
                case Direction.Up:
                    return state == State.Idle ? characterSprites.upIdleSprites : characterSprites.upMoveSprites;
                case Direction.Down:
                    return state == State.Idle ? characterSprites.downIdleSprites : characterSprites.downMoveSprites;
                case Direction.Left:
                    return state == State.Idle ? characterSprites.leftIdleSprites : characterSprites.leftMoveSprites;
                case Direction.Right:
                    return state == State.Idle ? characterSprites.rightIdleSprites : characterSprites.rightMoveSprites;
                default:
                    return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private Direction GetDirectionFromVector(Vector2 dir) {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) {
                return dir.x > 0 ? Direction.Right : Direction.Left;
            }
            else {
                return dir.y > 0 ? Direction.Up : Direction.Down;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void FlipSpritesForRight() {
            if (flipSpritesForRight) {
                spriteRenderer.flipX = currentDirection == Direction.Right;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void DieAnimation() {
            PlayAnimation(CharactersSprites[charactersSpritesIdx].dieSprites);
            playerController.CharacSpeed = 0;
            if (GameFlow.instance != null && player != null) {
                GameFlow.instance.RemovePlayer(player);
            }
            Destroy(gameObject, 1f);
        }
    }
}
