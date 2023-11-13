using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    public class Items : MonoBehaviour {
        /// <summary>
        /// Danh sách loại vật phẩm
        /// </summary>
        public enum ItemType {
            SpeedUp,
            ExplosionRadiusPlus,
            ExplosionRadiusMinus,
            BombAmountPlus,
            ExtraLife,
            Ghost,
            BombThrowable,
            PushBomb,
            Shield,
        }
        /// <summary>
        /// 
        /// </summary>
        public ItemType type;
        /// <summary>
        /// Hàm để nhặt vật phẩm
        /// </summary>
        private void OnItemPickup(GameObject player) {
            switch (type) {
                case ItemType.SpeedUp:
                    if (player.GetComponent<BombermanPlayerController>().CharacSpeed < 4) {
                        player.GetComponent<BombermanPlayerController>().CharacSpeed += 0.2f;
                    }
                    break;
                case ItemType.ExplosionRadiusPlus:
                    if (player.GetComponent<BombermanPlayerController>().ExplosionRadius < 9) {
                        player.GetComponent<BombermanPlayerController>().ExplosionRadius++;
                    }
                    break;
                case ItemType.ExplosionRadiusMinus:
                    if (player.GetComponent<BombermanPlayerController>().ExplosionRadius > 1) {
                        player.GetComponent<BombermanPlayerController>().ExplosionRadius--;
                    }   
                    break;
                case ItemType.BombAmountPlus:
                    if (player.GetComponent<BombermanPlayerController>().CurrentBombHold < player.GetComponent<BombermanPlayerController>().MaxBomb) {
                        player.GetComponent<BombermanPlayerController>().CurrentBombHold++;
                    }
                    break;
                case ItemType.ExtraLife:
                    if (player.GetComponent<BombermanPlayerController>().CharacterLives < 4) {
                        player.GetComponent<BombermanPlayerController>().CharacterLives++;
                    }
                    break;
                case ItemType.Ghost:
                    player.GetComponent<BombermanPlayerController>().ActiveGhostMode();
                    break;
                case ItemType.BombThrowable:
                    player.GetComponent<BombermanPlayerController>().CanThrowBomb();
                    break;
                case ItemType.PushBomb:
                    player.GetComponent<BombermanPlayerController>().CanPushBomb();
                    break;
                case ItemType.Shield:
                    player.GetComponent<BombermanPlayerController>().Shielded();
                    break;
            }
            Destroy(gameObject);
        }
        /// <summary>
        /// Hàm trigger khi tác động vào 1 item
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                OnItemPickup(other.gameObject);
            }
        }
    }
}