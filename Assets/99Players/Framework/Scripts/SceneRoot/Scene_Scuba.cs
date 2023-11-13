using UnityEngine;
public class Scene_Scuba : MonoBehaviour {
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
        CharaLeanTweenMotionData.CreateMotionData();
    }
    private void OnEnable() {
        OpenLayer();
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        SingletonCustom<Scuba_GameManager>.Instance.Init();
        SingletonCustom<Scuba_CharacterManager>.Instance.Init();
        SingletonCustom<Scuba_ItemManager>.Instance.Init();
        SingletonCustom<Scuba_UiManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        SingletonCustom<Scuba_GameManager>.Instance.UpdateMethod();
        SingletonCustom<Scuba_CharacterManager>.Instance.UpdateMethod();
        SingletonCustom<Scuba_ItemManager>.Instance.UpdateMethod();
        SingletonCustom<Scuba_UiManager>.Instance.UpdateMethod();
    }
}
