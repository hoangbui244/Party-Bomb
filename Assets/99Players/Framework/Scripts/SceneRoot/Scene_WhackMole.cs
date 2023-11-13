using UnityEngine;
public class Scene_WhackMole : MonoBehaviour {
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
        SingletonCustom<WhackMoleGameManager>.Instance.Init();
        SingletonCustom<WhackMoleCharacterManager>.Instance.Init();
        SingletonCustom<WhackMoleTargetManager>.Instance.Init();
        SingletonCustom<WhackMoleUiManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        SingletonCustom<WhackMoleGameManager>.Instance.UpdateMethod();
        SingletonCustom<WhackMoleCharacterManager>.Instance.UpdateMethod();
        SingletonCustom<WhackMoleTargetManager>.Instance.UpdateMethod();
        SingletonCustom<WhackMoleUiManager>.Instance.UpdateMethod();
    }
}
