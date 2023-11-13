using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace io.ninenine.players.party3d.games.common {
    [Serializable]
    public struct SpriteRendererUnityEventPair {
        public SpriteRenderer SpriteRenderer;
        public UnityEvent UnityEvent;
    }
    [Serializable]
    public struct SpriteRendererButtonUnityEventPair {
        public SpriteRendererButton SpriteRendererButton;
        public UnityEvent UnityEvent;
        public SpriteRendererButtonUnityEventPair(SpriteRendererButton button, UnityEvent ev) {
            SpriteRendererButton = button;
            UnityEvent = ev;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class SpriteRendererButtonController : MonoBehaviour {
        #region Inspector
        [Header("References")]
        public List<SpriteRendererUnityEventPair> SpriteRendererList;
        [Header("Runtime")]
        // TODO: This could be a dictionary for performance (we use list here for inspector view)
        public List<SpriteRendererButtonUnityEventPair> SpriteRendererButtonList;
        #endregion
        private void Awake() {
            foreach (var pair in SpriteRendererList) {
                var button = pair.SpriteRenderer.gameObject.AddComponent<SpriteRendererButton>();
                button.Initialize(this);
                SpriteRendererButtonList.Add(new SpriteRendererButtonUnityEventPair(button, pair.UnityEvent));
            }
        }
        public void InvokeButtonMouseDown(SpriteRendererButton spriteRendererButton) {
            foreach (var pair in SpriteRendererButtonList) {
                if (pair.SpriteRendererButton != spriteRendererButton) {
                    continue;
                }
                pair.UnityEvent.Invoke();
                break;
            }
        }
    }
}