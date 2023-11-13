using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    public class GameFlow : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        public static GameFlow instance;
        /// <summary>
        /// 
        /// </summary>
        public RankingResultManager rankingResultManager;
        /// <summary>
        /// 
        /// </summary>
        public List<BombermanPlayer> players = new List<BombermanPlayer>();
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public bool IsReady = false;
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public bool IsShow = false;
        /// <summary>
        /// 
        /// </summary>
        private List<GameObject> enemies = new List<GameObject>();
        /// <summary>
        /// 
        /// </summary>
        private bool timeCount = true;
        /// <summary>
        /// 
        /// </summary>
        private float timeWait = 0f;
        /// <summary>
        /// 
        /// </summary>
        public float timeSur = 120f;
        /// <summary>
        /// 
        /// </summary>
        public float timeMatch = 120f;
        /// <summary>
        /// 
        /// </summary>
        private void Awake() {
            instance = this;
        }
        /// <summary>
        /// 
        /// </summary>
        private void Start() {
            CommonStartProduction.Instance.Play(Begin);
            CommonUIManager.Instance.Init(CommonUIManager.UIType.TimerOnly);
            ResultGameDataParams.SetPoint();
            CommonUIManager.Instance.SetTime(timeMatch);
            players.AddRange(FindObjectsOfType<BombermanPlayer>());
            GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Ninja");
            foreach (var enemy in enemyObjects) {
                enemies.Add(enemy);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void Begin() {
            IsReady = true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void End() {
            IsReady = false;
        }
        /// <summary>
        /// 
        /// </summary>
        public void RemovePlayer(BombermanPlayer player) {
            players.Remove(player);
        }
        /// <summary>
        /// 
        /// </summary>
        public void RemoveEnemy(GameObject enemy) {
            enemies.Remove(enemy);
        }
        /// <summary>
        /// 
        /// </summary>
        private void Update() {
            if (IsReady && timeCount) {
                timeMatch -= Time.deltaTime;
                CommonUIManager.Instance.SetTime(timeMatch);
            }
            for (int i = players.Count - 1; i >= 0; i--) {
                if (players[i] == null) {
                    players.RemoveAt(i);
                }
            }
            for (int i = enemies.Count - 1; i >= 0; i--) {
                if (enemies[i] == null) {
                    enemies.RemoveAt(i);
                }
            }
            if (players.Count == 1 && enemies.Count == 0) {
                timeCount = false;
                if (IsReady) {
                    CommonEndSimple.Instance.Show(End);
                    timeWait += Time.deltaTime;
                    if (timeWait > 1.6f) {
                        if (!IsShow) {
                            Result();
                            timeWait = 0f;
                        }
                    }
                }
            }
            else if (players.Count == 0 && enemies.Count != 0) {
                timeCount = false;
                if (IsReady) {
                    CommonEndSimple.Instance.Show(End);
                    timeWait += Time.deltaTime;
                    if (timeWait > 1.6f) {
                        if (!IsShow) {
                            Result();
                            timeWait = 0f;
                        }
                    }
                }
            }
            else if (timeMatch <= 0) {
                timeCount = false;
                if (IsReady) {
                    CommonEndSimple.Instance.Show(End);
                    timeWait += Time.deltaTime;
                    if (timeWait > 1.6f) {
                        if (!IsShow) {
                            Result();
                            timeWait = 0f;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void Result() {
            IsShow = true;
            int[] playerIdArray = new int[BombermanPlayerManager.Instance.UserNum];
            float[] timeScoreArray = new float[BombermanPlayerManager.Instance.UserNum];
            int cpuId = 0;
            for (int i = 0; i < BombermanPlayerManager.Instance.UserNum; i++) {
                if (BombermanPlayerManager.Instance.IsCpu[i]) {
                    Debug.Log(i + " is CPU");
                    playerIdArray[i] = 6 + cpuId;
                    cpuId++;
                }
                else {
                    playerIdArray[i] = GameSettingManager.Instance.PlayerGroupList[i][0];
                }
                timeScoreArray[i] = BombermanPlayerManager.Instance.playerSurTime[i];
                CalcManager.ConvertTimeToRecordString(timeScoreArray[i], playerIdArray[i]);
            }
            ResultGameDataParams.SetRecord_Float(timeScoreArray, playerIdArray);
            rankingResultManager.ShowResult_Time();
        }
    }
}