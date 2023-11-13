using UnityEngine;
using UnityEngine.Rendering;
public class CustomQualitySetting : MonoBehaviour {
    [SerializeField]
    private MeshRenderer[] shadowDisableRenderers;
    [SerializeField]
    private MeshRenderer[] shadowAndReceiveDisableRenderers;
    [SerializeField]
    private GameObject[] activeDisableObjs;
    private void Awake() {
        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2) {
            for (int i = 0; i < shadowDisableRenderers.Length; i++) {
                shadowDisableRenderers[i].shadowCastingMode = ShadowCastingMode.Off;
            }
            for (int j = 0; j < shadowAndReceiveDisableRenderers.Length; j++) {
                shadowAndReceiveDisableRenderers[j].shadowCastingMode = ShadowCastingMode.Off;
                shadowAndReceiveDisableRenderers[j].receiveShadows = false;
            }
            for (int k = 0; k < activeDisableObjs.Length; k++) {
                activeDisableObjs[k].SetActive(value: false);
            }
        }
    }
}
