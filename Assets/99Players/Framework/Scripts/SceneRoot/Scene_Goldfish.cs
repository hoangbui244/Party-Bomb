using UnityEngine;
public class Scene_Goldfish : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private void LayerEnd(SceneManager.LayerCloseType _closeType) {
    }
    private void OpenLayer() {
        SingletonCustom<SceneManager>.Instance.AddNowLayerCloseCallBack(LayerEnd);
    }
    private bool IsActive() {
        return SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack() == new SceneManager.LayerClose(LayerEnd);
    }
    private void QuitGame() {
    }
    private void Awake() {
    }
    private void OnEnable() {
        OpenLayer();
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        SingletonCustom<GoldfishGameManager>.Instance.Init();
        SingletonCustom<GoldfishCharacterManager>.Instance.Init();
        SingletonCustom<GoldfishTargetManager>.Instance.Init();
        SingletonCustom<GoldfishUiManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        SingletonCustom<GoldfishGameManager>.Instance.UpdateMethod();
        SingletonCustom<GoldfishCharacterManager>.Instance.UpdateMethod();
        SingletonCustom<GoldfishTargetManager>.Instance.UpdateMethod();
        SingletonCustom<GoldfishUiManager>.Instance.UpdateMethod();
    }
}
