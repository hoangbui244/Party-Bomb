using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    public class BossSkill : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        private BoxCollider2D boxCollider2D;
        /// <summary>
        /// 
        /// </summary>
        private Animator animator;
        /// <summary>
        /// 
        /// </summary>
        private void Awake() {
            boxCollider2D = GetComponent<BoxCollider2D>();
            boxCollider2D.enabled = false;
            StartCoroutine(Skill());
            animator = GetComponent<Animator>();
            animator.SetBool("IsActive", false);
            Destroy(gameObject, 2f);

        }
        /// <summary>
        /// 
        /// </summary>
        private IEnumerator Skill() {
            yield return new WaitForSeconds(1.5f);
            animator.SetBool("IsActive", true);
            boxCollider2D.enabled = true;
        }
    }
}