using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static SmeltFishing_CharacterAnimator;
namespace io.ninenine.players.party2d.games.bomberman {
    /// <summary>
    /// 
    /// </summary>
    public class BombermanPlayerAI : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        public enum AIState {
            Idle,
            Move
        }
        /// <summary>
        /// 
        /// </summary>
        public AIState currentState;
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public Vector2 moveDirection;
        /// <summary>
        /// 
        /// </summary>
        public void UpdateAI() {
            currentState = Random.Range(0, 2) == 0 ? AIState.Idle : AIState.Move;
            switch (currentState) {
                case AIState.Idle:
                    break;
            }
        }
    }
}