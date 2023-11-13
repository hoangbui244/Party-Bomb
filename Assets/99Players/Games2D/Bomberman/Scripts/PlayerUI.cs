using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace io.ninenine.players.party2d.games.bomberman {
    public class PlayerUI : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        public BombermanPlayer player;
        /// <summary>
        /// 
        /// </summary>
        public TextMeshProUGUI playerName;
        private void Start() {
            if (player.IsCpu) {
                playerName.text = "Bot " + player.PlayerIdx.ToString();
            }
            else {
                playerName.text = "Player " + player.PlayerIdx.ToString();
            }
        }
    }
}