using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    public class BombermanPlayerManager : SingletonCustom<BombermanPlayerManager> {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private BombermanPlayer[] playerList;
        /// <summary>
        /// 
        /// </summary>
        private BombermanPlayer[] players;
        /// <summary>
        /// 
        /// </summary>
        private int userNum = 4;
        /// <summary>
        /// 
        /// </summary>
        private int playerNum;
        /// <summary>
        /// 
        /// </summary>
        public BombermanPlayer[] Players => players;
        /// <summary>
        /// 
        /// </summary>
        public int UserNum => userNum;
        /// <summary>
        /// 
        /// </summary>
        public int PlayerNum => playerNum;
        /// <summary>
        /// 
        /// </summary>
        public float[] playerSurTime;
        /// <summary>
        /// 
        /// </summary>
        public bool[] IsCpu;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private BombermanPlayer[] playerPrefab;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform playerRoot;
        /// <summary>
        /// 
        /// </summary>
        public void SpawnPlayer() {
            playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
            userNum = (SingletonCustom<GameSettingManager>.Instance.IsSixPlayerGroup ? 6 : 4);
            playerList = new BombermanPlayer[userNum];
            List<int> availableSpawnIndices = Enumerable.Range(0, StageManager.Instance.SpawnPos.Length).ToList();
            IsCpu = new bool[playerList.Length];
            for (int i = 0; i < playerList.Length; i++) {
                int randomPlayerIndex = UnityEngine.Random.Range(0, playerPrefab.Length);
                int randomSpawnIndex = availableSpawnIndices[UnityEngine.Random.Range(0, availableSpawnIndices.Count)];
                availableSpawnIndices.Remove(randomSpawnIndex);
                Transform spawnPoint = StageManager.Instance.SpawnPos[randomSpawnIndex];
                playerList[i] = Instantiate(playerPrefab[randomPlayerIndex], spawnPoint.position, Quaternion.identity, playerRoot); //1
                playerList[i].Initialize(i, spawnPoint.position); //2
                IsCpu[i] = playerList[i].IsCpu;
            }
            playerSurTime = new float[playerList.Length];
        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdateMethod() {
            for (int i = 0; i < UserNum; i++) {
                players[i].UpdateMethod();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void FixedUpdateMethod() {
            for (int i = 0; i < UserNum; i++) {
                players[i].FixedUpdateMethod();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsOnePlayer() {
            int num = playerNum;
            for (int i = 0; i < playerNum; i++) {
                if (players[i].IsDead) {
                    num--;
                }
            }
            return num <= 1;
        }
        /// <summary>
        /// 
        /// </summary>
        public int GetAlivePlayerNum() {
            int num = 0;
            for (int i = 0; i < UserNum; i++) {
                if (!players[i].IsDead) {
                    num++;
                }
            }
            return num;
        }
        /// <summary>
        /// 
        /// </summary>
        public Transform[] GetAlivePlayer() {
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < UserNum; i++) {
                if (!players[i].IsDead) {
                    list.Add(players[i].transform);
                }
            }
            return list.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        public bool CheckAliveCpuOnly() {
            for (int i = 0; i < UserNum; i++) {
                if (!players[i].IsDead && !players[i].IsCpu) {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdateLookTargetAnchors(Transform _player) {
            SingletonCustom<CommonCameraMoveManager>.Instance.RemoveLookTargetAnchor(_player);
        }
    }
}