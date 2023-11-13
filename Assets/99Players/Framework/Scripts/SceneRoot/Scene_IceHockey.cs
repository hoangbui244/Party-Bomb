using UnityEngine;
public class Scene_IceHockey : MonoBehaviour {
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
    }
    private void Start() {
        SingletonCustom<IceHockeyGameManager>.Instance.Init();
        SingletonCustom<IceHockeyPlayerManager>.Instance.Init();
        SingletonCustom<IceHockeyUIManager>.Instance.Init();
    }
    private void Update() {
        if (Time.timeScale == 0f) {
            SingletonCustom<IceHockeyUIManager>.Instance.UpdateMethod();
            return;
        }
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                SingletonCustom<IceHockeyGameManager>.Instance.OnGameStart();
            });
            isInit = true;
        }
        SingletonCustom<IceHockeyGameManager>.Instance.UpdateMethod();
        SingletonCustom<IceHockeyPlayerManager>.Instance.UpdateMethod();
        SingletonCustom<IceHockeyUIManager>.Instance.UpdateMethod();
    }
}
