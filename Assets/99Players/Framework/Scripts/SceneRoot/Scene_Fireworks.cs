using UnityEngine;
public class Scene_Fireworks : MonoBehaviour {
    private bool isInit;
    private bool isStart;
    private void OnEnable() {
        LightingSettings.ChangeSceneLighting();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Start() {
        SingletonCustom<FireworksGameManager>.Instance.Init();
        SingletonCustom<FireworksPlayerManager>.Instance.Init();
        SingletonCustom<FireworksUIManager>.Instance.Init();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            isInit = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                isStart = true;
                SingletonCustom<FireworksGameManager>.Instance.OnGameStart();
            });
        }
        if (!isStart) {
            SingletonCustom<FireworksUIManager>.Instance.UpdateMethod();
            return;
        }
        SingletonCustom<FireworksGameManager>.Instance.UpdateMethod();
        SingletonCustom<FireworksPlayerManager>.Instance.UpdateMethod();
        SingletonCustom<FireworksUIManager>.Instance.UpdateMethod();
    }
}
