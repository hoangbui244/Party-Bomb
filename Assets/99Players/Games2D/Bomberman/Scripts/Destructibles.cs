using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    public class Destructibles : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        public GameObject[] SpawnableItems;
        /// <summary>
        /// 
        /// </summary>
        private Animator Animator;
        /// <summary>
        /// 
        /// </summary>
        private bool isDestroyed = false;
        /// <summary>
        /// 
        /// </summary>
        private void Awake() {
            Animator = GetComponent<Animator>();
            Animator.SetBool("IsTrigger", false);
        }
        /// <summary>
        /// 
        /// </summary>
        public void IsDestroyed() {
            if (!isDestroyed) {
                isDestroyed = true;
                Animator.SetBool("IsTrigger", true);
                DestroyDestructibles();
                Destroy(gameObject, 0.6f);
            }
        }
        /// <summary>
        /// Hàm khi phá vỡ vật cản thì rơi item
        /// </summary>
        public void DestroyDestructibles() {
            if (SpawnableItems.Length > 0 && Random.value < BombermanGameData.Instance.ItemSpawnChance) {
                int randomIndex = Random.Range(0, SpawnableItems.Length);
                Instantiate(SpawnableItems[randomIndex], transform.position, Quaternion.identity, BombermanGameControler.getInstance().rootParent);
            }
        }
    }
}