using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace io.ninenine.players.party3d.games.common {
    [Serializable]
    public struct Collider2DUnityEventPair {
        public Collider2D Collider2D;
        public UnityEvent UnityEvent;
    }
    /// <summary>
    /// 
    /// </summary>
    public class ColliderPointerDownController : MonoBehaviour {
        #region Inspector
        [Header("Data")]
        // TODO: This could be a dictionary for performance (we use list here for inspector view)
        public List<Collider2DUnityEventPair> Collider2DUnityEventPairList;
        #endregion
        private void Awake() {
            foreach (var pair in Collider2DUnityEventPairList) {
                GameObject currGameObj = pair.Collider2D.gameObject;
                if (!currGameObj.TryGetComponent(out ColliderPointerDownInvoker invoker)) {
                    invoker = currGameObj.AddComponent<ColliderPointerDownInvoker>();
                }
                invoker.Initialize(this);
            }
        }
        public void InvokeButtonMouseDown(Collider2D invokerCollider) {
            foreach (var pair in Collider2DUnityEventPairList) {
                if (pair.Collider2D != invokerCollider) {
                    continue;
                }
                pair.UnityEvent.Invoke();
                break;
            }
        }
    }
}