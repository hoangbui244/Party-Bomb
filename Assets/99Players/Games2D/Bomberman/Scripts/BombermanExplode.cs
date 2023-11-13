using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    /// <summary>
    /// 
    /// </summary>
    public class BombermanExplode: MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        public AudioSource[] ExplodeEffect;
        /// <summary>
        /// 
        /// </summary>
        void Start() {
            playExplodeEffect();
        }
        /// <summary>
        /// 
        /// </summary>
        void Update() {
        }
        /// <summary>
        /// 
        /// </summary>
        public void playExplodeEffect() {
            var randValue = Random.Range(0, 5);
            ExplodeEffect[randValue].Play();
            Invoke("destroyPrefab", ExplodeEffect[randValue].clip.length / 2);
        }
        /// <summary>
        /// 
        /// </summary>
        public void destroyPrefab() {
            Destroy(gameObject);
        }
    }
}
