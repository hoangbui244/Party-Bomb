using UnityEngine;
public class Scene_FindMask : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private bool isStart;
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
        SingletonCustom<FindMask_GameManager>.Instance.Init();
        SingletonCustom<FindMask_CharacterManager>.Instance.Init();
        SingletonCustom<FindMask_ControllerManager>.Instance.Init();
        SingletonCustom<FindMask_MaskManager>.Instance.Init();
        SingletonCustom<FindMask_ScoreManager>.Instance.Init();
        SingletonCustom<FindMask_UIManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        SingletonCustom<FindMask_GameManager>.Instance.UpdateMethod();
        SingletonCustom<FindMask_ControllerManager>.Instance.UpdateMethod();
        SingletonCustom<FindMask_UIManager>.Instance.UpdateMethod();
    }
}
