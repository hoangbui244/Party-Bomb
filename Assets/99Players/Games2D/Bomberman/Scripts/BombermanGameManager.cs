using GamepadInput;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    /// <summary>
	/// 
	/// </summary>
	public class BombermanGameManager: SingletonCustom<BombermanGameManager> {
        /// <summary>
        /// 
        /// </summary>
        public enum State {
            /// <summary>
            /// 
            /// </summary>
            StartWait,
            /// <summary>
            /// 
            /// </summary>
            InGame,
            /// <summary>
            /// 
            /// </summary>
            EndGame,
            /// <summary>
            /// 
            /// </summary>
            Result
        }
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        [Header("順位：リザルト")]
        private RankingResultManager rankingResult;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        [Header("ワ\u30fcルドル\u30fcト")]
        private GameObject root3Dworld;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        [Header("カメラ")]
        private Camera worldCamera;
        /// <summary>
        /// 
        /// </summary>
        private State currentState;
        /// <summary>
        /// 
        /// </summary>
        private List<SettingResultDataManager.RankingData> listRankData = new List<SettingResultDataManager.RankingData>();
        /// <summary>
        /// 
        /// </summary>
        private SettingResultDataManager resultDataManager = new SettingResultDataManager();
        /// <summary>
        /// 
        /// </summary>
        private bool isDebugResult;
        /// <summary>
        /// 
        /// </summary>
        private float waitTime;
        /// <summary>
        /// 
        /// </summary>
        private float gameTime;
        /// <summary>
        /// 
        /// </summary>
        private bool isSkiped;
        /// <summary>
        /// 
        /// </summary>
        public State CurrentState => currentState;
        /// <summary>
        /// 
        /// </summary>
        public bool IsDuringBattle => CurrentState == State.InGame;
        /// <summary>
        /// 
        /// </summary>
        public bool IsStartWait => currentState == State.StartWait;
        /// <summary>
        /// 
        /// </summary>
        public bool IsInGame => currentState == State.StartWait;
        /// <summary>
        /// 
        /// </summary>
        public List<GameObject> PlayerList = new List<GameObject>();
        /// <summary>
        /// 
        /// </summary>
        public bool IsEndGame {
            get {
                if (currentState != 0) {
                    return currentState == State.Result;
                }
                return true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public float GameTime => gameTime;
        /// <summary>
        /// 
        /// </summary>
        public void Init() {
            SetState(State.StartWait);
        }
        /// <summary>
        /// 
        /// </summary>
        public void OnGameStart() {
            SingletonCustom<CommonStartProduction>.Instance.Play(delegate {
                SetState(State.InGame);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_state"></param>
        private void SetState(State _state) {
            currentState = _state;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        public GameObject FindNearestPlayer(Vector3 Position) {
            GameObject Result = null;
            float oldDistance = 9999999.0f;
            foreach (GameObject g in PlayerList) {
                float dist = Vector3.Distance(Position, g.transform.position);
                if (dist < oldDistance) {
                    Result = g;
                    oldDistance = dist;
                }
            }
            return Result;
        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdateMethod() {
            switch (currentState) {
                case State.InGame:
                    gameTime = Mathf.Max(gameTime + Time.deltaTime, 0f);
                    if (SingletonCustom<BombermanPlayerManager>.Instance.IsOnePlayer()) {
                        GameEnd();
                    }
                    if (!isSkiped && SingletonCustom<BombermanUIManager>.Instance.IsShowSkip && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X)) {
                        GameSkip();
                    }
                    break;
                case State.EndGame:
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0f) {
                        SingletonCustom<CommonEndSimple>.Instance.Show(delegate {
                            ToResult();
                        });
                        SetState(State.Result);
                    }
                    break;
            }
            DebugUpdate();
        }
        /// <summary>
        /// 
        /// </summary>
        public void LateUpdateMethod() {
        }
        /// <summary>
        /// 
        /// </summary>
        private void GameSkip() {
            isSkiped = true;
            SingletonCustom<CommonEndSimple>.Instance.Show(delegate {
                SettingSkipResultData();
                ToResult();
                SetState(State.Result);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        private void SettingSkipResultData() {
            List<int> list = new List<int>();
            for (int i = 0;i < SingletonCustom<BombermanPlayerManager>.Instance.UserNum;i++) {
                if (!SingletonCustom<BombermanPlayerManager>.Instance.Players[i].IsDead) {
                    list.Add(i);
                }
            }
            float num = gameTime;
            if (list.Count <= 1) {
                return;
            }
            list.Shuffle();
            for (int j = 0;j < list.Count;j++) {
                if (j != list.Count - 1) {
                    num = Mathf.Clamp(num + UnityEngine.Random.Range(3f, 15f), 0f, 599f);
                    //??SingletonCustom<JackalPlayerManager>.Instance.Players[list[j]].SetTimeScore(num);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void GameEnd() {
            for (int i = 0;i < SingletonCustom<BombermanPlayerManager>.Instance.Players.Length;i++) {
                //??SingletonCustom<JackalPlayerManager>.Instance.Players[i].GameEnd();
            }
            waitTime = 3f;
            SetState(State.EndGame);
        }
        /// <summary>
        /// 
        /// </summary>
        private void ToResult() {
            ResultGameDataParams.SetPoint();
            SettingResultData();
            LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                root3Dworld.SetActive(value: false);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        private void SettingResultData() {
            BombermanPlayer[] players = SingletonCustom<BombermanPlayerManager>.Instance.Players;
            int userNum = SingletonCustom<BombermanPlayerManager>.Instance.UserNum;
            int alivePlayerNum = SingletonCustom<BombermanPlayerManager>.Instance.GetAlivePlayerNum();
            for (int i = 0;i < userNum;i++) {
                //SettingResultDataManager.RankingData item = default(SettingResultDataManager.RankingData);
                //item.playerIdx = players[i].UserTypeNo;
                //item.score = players[i].Score;
                //if (!players[i].IsDead && alivePlayerNum == 1) {
                //    item.time = -1f;
                //} else {
                //    item.time = players[i].TimeScore;
                //}
                //if (isDebugResult) {
                //    item.score = UnityEngine.Random.Range(0, 9999);
                //    item.time = UnityEngine.Random.Range(0f, 599f);
                //}
                //listRankData.Add(item);
            }
            resultDataManager.SettingData(listRankData, rankingResult, SettingResultDataManager.ResultDataType.TIME, SettingResultDataManager.CollectionDataType.WIN, GS_Define.GameType.BLOW_AWAY_TANK);
        }
        /// <summary>
        /// 
        /// </summary>
        private new void OnDestroy() {
            base.OnDestroy();
            LeanTween.cancel(base.gameObject);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Camera GetCamera() {
            return worldCamera;
        }
        /// <summary>
        /// 
        /// </summary>
        public void DebugUpdate() {
        }
        /// <summary>
        /// 
        /// </summary>
        public void DebugGoal() {
        }
    }
}