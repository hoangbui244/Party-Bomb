using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    /// <summary>
    /// 
    /// </summary>
    public class Scene_Bomberman: MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        void Start() {
            GameSettingManager.Instance.SetCpuToPlayerGroupList();
        }
        /// <summary>
        /// 
        /// </summary>
        void Update() {
        }
    }
}
