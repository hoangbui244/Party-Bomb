using UnityEngine;
public class Scene_Hidden : MonoBehaviour {
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
        SingletonCustom<Hidden_GameManager>.Instance.Init();
        SingletonCustom<Hidden_CharacterManager>.Instance.Init();
        SingletonCustom<Hidden_UiManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        SingletonCustom<Hidden_GameManager>.Instance.UpdateMethod();
        SingletonCustom<Hidden_CharacterManager>.Instance.UpdateMethod();
        SingletonCustom<Hidden_UiManager>.Instance.UpdateMethod();
    }
}
