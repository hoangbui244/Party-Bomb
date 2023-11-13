using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    /// <summary>
    /// 
    /// </summary>
    public class BombermanGameData: SingletonCustom<BombermanGameData> {
        /// <summary>
        /// Thời gian ở GhostMode
        /// </summary>
        public float GhostTime = 4f;
        /// <summary>
        /// Tỉ lệ item xuất hiện khi phá vỡ gạch
        /// </summary>
        [Range(0f, 1f)]
        public float ItemSpawnChance = 0.2f;
        /// <summary>
        /// 
        /// </summary>
        public float CharacterInvincibleTime = 2f;
        /// <summary>
        /// 
        /// </summary>
        public float ShieldTime = 8f;
    }
}
