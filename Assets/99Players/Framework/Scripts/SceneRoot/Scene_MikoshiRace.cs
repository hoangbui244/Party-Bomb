using UnityEngine;
public class Scene_MikoshiRace : MonoBehaviour {
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
        SingletonCustom<MikoshiRaceGameManager>.Instance.Init();
        SingletonCustom<MikoshiRaceCarManager>.Instance.Init();
        SingletonCustom<MikoshiRaceUiManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        SingletonCustom<MikoshiRaceGameManager>.Instance.UpdateMethod();
        SingletonCustom<MikoshiRaceCarManager>.Instance.UpdateMethod();
        SingletonCustom<MikoshiRaceUiManager>.Instance.UpdateMethod();
    }
}
