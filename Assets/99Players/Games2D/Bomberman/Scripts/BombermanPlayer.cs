using GamepadInput;
using io.ninenine.players.party3d.games.common;
using System;
using System.Collections;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    /// <summary>
    /// 
    /// </summary>
    public class BombermanPlayer : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        private ResultGameDataParams.UserType userType;
        /// <summary>
        /// 
        /// </summary>
        private int userTypeNo;
        /// <summary>
        /// 
        /// </summary>
        private int playerIdx = -1;
        /// <summary>
        /// 
        /// </summary>
        private bool isCpu;
        /// <summary>
        /// 
        /// </summary>
        private bool isDead;
        /// <summary>
        /// 
        /// </summary>
        public int UserTypeNo => userTypeNo;
        /// <summary>
        /// 
        /// </summary>
        public int PlayerIdx => playerIdx;
        /// <summary>
        /// 
        /// </summary>
        public bool IsCpu => isCpu;
        /// <summary>
        /// 
        /// </summary>
        public bool IsDead => isDead;
        /// <summary>
        /// 
        /// </summary>
        public void Initialize(int _playerIdx, Vector3 _pos) {
            playerIdx = _playerIdx;
            isCpu = (playerIdx > SingletonCustom<BombermanPlayerManager>.Instance.PlayerNum - 1);
            userType = (ResultGameDataParams.UserType)(IsCpu ? (6 + (playerIdx - SingletonCustom<BombermanPlayerManager>.Instance.PlayerNum)) : playerIdx);
            userTypeNo = (int)userType;
            base.transform.position = _pos;
            PlayerAnimatedSprites playerSprites = GetComponent<PlayerAnimatedSprites>();
            playerSprites.charactersSpritesIdx = playerIdx;
        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdateMethod() {
        }
        /// <summary>
        /// 
        /// </summary>
        public void FixedUpdateMethod() {
        }
    }
}