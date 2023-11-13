using UnityEngine;
public class Scene_MakingPotion : MonoBehaviour {
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
        SingletonCustom<MakingPotion_TargetManager>.Instance.Init();
        SingletonCustom<MakingPotion_GameManager>.Instance.Init();
        SingletonCustom<MakingPotion_PlayerManager>.Instance.Init();
        SingletonCustom<MakingPotion_UiManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        SingletonCustom<MakingPotion_GameManager>.Instance.UpdateMethod();
        SingletonCustom<MakingPotion_PlayerManager>.Instance.UpdateMethod();
        SingletonCustom<MakingPotion_UiManager>.Instance.UpdateMethod();
    }
}
