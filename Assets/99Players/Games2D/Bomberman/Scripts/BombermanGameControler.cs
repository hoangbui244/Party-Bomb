using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace io.ninenine.players.party2d.games.bomberman {
    /// <summary>
    /// Quản lý toàn bộ game logic
    /// </summary>
    public class BombermanGameControler: MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        private float currentTime;
        /// <summary>
        /// 
        /// </summary>
        public GameObject PlayerPrefab;
        /// <summary>
        /// Số mạng tổng
        /// </summary>
        public int PlayerLives;
        /// <summary>
        /// 
        /// </summary>
        private Vector3 spawnPoint;
        /// <summary>
        /// 
        /// </summary>
        public static float temp_Alpha = 0;
        /// <summary>
        /// 
        /// </summary>
        private float maskUpdate;
        /// <summary>
        /// 
        /// </summary>
        public bool doChange = false;
        /// <summary>
        /// 
        /// </summary>
        public Transform rootParent;
        /// <summary>
        /// 
        /// </summary>
        private static BombermanGameControler instance;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static BombermanGameControler getInstance() {
            return instance;
        }
        /// <summary>
        /// 
        /// </summary>
        void Awake() {
            instance = this;
        }
        /// <summary>
        /// 
        /// </summary>
        void Start() {
            currentTime = Time.time;
            //PlayerLives = BombermanGameData.Instance.CharacterLives;
        }
        /// <summary>
        /// 
        /// </summary>
        void Update() {
            currentTime = Time.time;
            gradualChange();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spawnPoint"></param>
        public void characSpawn(Vector3 spawnPoint) {
            if (PlayerLives > 0) {
                this.spawnPoint = spawnPoint;
                //Invoke("InstantiatePrefab", BombermanGameData.Instance.SpawnTime);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void InstantiatePrefab() {
            if (GameObject.FindGameObjectWithTag("Player") == null) {
                --BombermanGameControler.getInstance().PlayerLives;
                Instantiate(PlayerPrefab, spawnPoint, Quaternion.Euler(Vector3.zero));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void gradualChange() {
            if (doChange & currentTime - maskUpdate > 0.05) {
                maskUpdate = Time.time;
                GameObject.FindGameObjectWithTag("Mask").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, temp_Alpha);
                if (temp_Alpha < 1) {
                    temp_Alpha += 0.05f;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void gameRestart() {
            //TODO: restart the game
        }
    }
}
