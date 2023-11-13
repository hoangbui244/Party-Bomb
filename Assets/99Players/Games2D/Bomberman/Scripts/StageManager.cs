using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    public class StageManager : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        public static StageManager Instance;
        /// <summary>
        /// 
        /// </summary>
        public GameObject[] levelPrefabs;
        /// <summary>
        /// 
        /// </summary>
        public Transform[] SpawnPos;
        /// <summary>
        /// 
        /// </summary>
        private int currentLevelIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        private GameObject currentLevel;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform stageRoot;
        /// <summary>
        /// 
        /// </summary>
        private void Awake() {
            Instance = this;
        }
        /// <summary>
        /// 
        /// </summary>
        private void Start() {
            LoadRandomLevel();
        }
        /// <summary>
        /// 
        /// </summary>
        public void LoadRandomLevel() {
            int randomLevelIndex = UnityEngine.Random.Range(0, levelPrefabs.Length);
            LoadLevel(randomLevelIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadLevel(int levelIndex) {
            foreach (var player in GameFlow.instance.players) {
                Destroy(player.gameObject);
            }
            GameFlow.instance.players.Clear();
            SpawnPos = levelPrefabs[levelIndex].GetComponent<Level>().SpawnPos;
            BombermanPlayerManager.Instance.SpawnPlayer();
            if (currentLevel != null) {
                Destroy(currentLevel);
            }
            currentLevel = Instantiate(levelPrefabs[levelIndex], stageRoot);
        }
    }
}