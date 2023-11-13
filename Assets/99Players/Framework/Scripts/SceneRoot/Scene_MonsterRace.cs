using UnityEngine;
public class Scene_MonsterRace : MonoBehaviour {
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
        SingletonCustom<MonsterRace_GameManager>.Instance.Init();
        SingletonCustom<MonsterRace_CarManager>.Instance.Init();
        SingletonCustom<MonsterRace_UiManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        SingletonCustom<MonsterRace_GameManager>.Instance.UpdateMethod();
        SingletonCustom<MonsterRace_CarManager>.Instance.UpdateMethod();
        SingletonCustom<MonsterRace_UiManager>.Instance.UpdateMethod();
    }
}
