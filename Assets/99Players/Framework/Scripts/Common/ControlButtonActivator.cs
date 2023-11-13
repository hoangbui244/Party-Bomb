using UnityEngine;
public class ControlButtonActivator : MonoBehaviour {
    [SerializeField]
    private JoyConManager.ControlMode activeMode;
    [SerializeField]
    private GameObject targetObject;
    private void OnEnable() {
        SingletonCustom<JoyConManager>.Instance.OnMainPlayerControlModeChanged += OnChanged;
        OnChanged(SingletonCustom<JoyConManager>.Instance.IsMainPlayerControlMode(activeMode) ? activeMode : JoyConManager.ControlMode.None);
    }
    private void OnDisable() {
        SingletonCustom<JoyConManager>.Instance.OnMainPlayerControlModeChanged -= OnChanged;
    }
    private void OnChanged(JoyConManager.ControlMode mode) {
        targetObject.SetActive(activeMode == mode);
    }
}
