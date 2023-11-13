using UnityEngine;
public class ResultDecoSwitch : MonoBehaviour {
    [SerializeField]
    [Header("切り替え対象オブジェクト")]
    private GameObject[] arrayTarget;
    private void Awake() {
        if (arrayTarget.Length != 0) {
            for (int i = 0; i < arrayTarget.Length; i++) {
                arrayTarget[i].SetActive(i == SingletonCustom<GameSettingManager>.Instance.ResultDecoIdx);
            }
            SingletonCustom<GameSettingManager>.Instance.ResultDecoIdx = (SingletonCustom<GameSettingManager>.Instance.ResultDecoIdx + 1) % arrayTarget.Length;
        }
    }
}
