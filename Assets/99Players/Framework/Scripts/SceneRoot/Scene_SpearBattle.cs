using UnityEngine;
public class Scene_SpearBattle : MonoBehaviour {
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
        ResultGameDataParams.SetPoint();
        SingletonCustom<SpearBattle_GameManager>.Instance.Init();
        SingletonCustom<SpearBattle_UIManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        if (!SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<SpearBattle_GameManager>.Instance.UpdateMethod();
            SingletonCustom<SpearBattle_UIManager>.Instance.UpdateMethod();
        }
    }
}
