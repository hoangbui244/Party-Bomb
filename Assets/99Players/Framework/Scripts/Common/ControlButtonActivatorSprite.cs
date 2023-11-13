using UnityEngine;
public class ControlButtonActivatorSprite : MonoBehaviour {
    [SerializeField]
    private JoyConManager.ControlMode activeMode;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite sprite;
    private void OnEnable() {
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
            bool flag = spriteRenderer == null;
        }
        SingletonCustom<JoyConManager>.Instance.OnMainPlayerControlModeChanged += OnChanged;
        OnChanged(SingletonCustom<JoyConManager>.Instance.IsMainPlayerControlMode(activeMode) ? activeMode : JoyConManager.ControlMode.None);
    }
    private void OnDisable() {
        SingletonCustom<JoyConManager>.Instance.OnMainPlayerControlModeChanged -= OnChanged;
    }
    private void OnChanged(JoyConManager.ControlMode mode) {
        if (activeMode == mode) {
            spriteRenderer.sprite = sprite;
        }
    }
}
