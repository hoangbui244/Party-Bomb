using UnityEngine;
public class Scene_BeachSoccer : SingletonCustom<Scene_BeachSoccer> {
    [SerializeField]
    private Material skybox;
    public bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
    }
    private void Start() {
        SingletonCustom<BeachSoccerGameManager>.Instance.Init();
        SingletonCustom<BeachSoccerPlayerManager>.Instance.Init();
        SingletonCustom<BeachSoccerUIManager>.Instance.Init();
    }
    private void Update() {
        if (Time.timeScale == 0f) {
            SingletonCustom<BeachSoccerUIManager>.Instance.UpdateMethod();
            return;
        }
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                SingletonCustom<BeachSoccerGameManager>.Instance.OnGameStart();
            });
            isInit = true;
        }
        SingletonCustom<BeachSoccerGameManager>.Instance.UpdateMethod();
        SingletonCustom<BeachSoccerPlayerManager>.Instance.UpdateMethod();
        SingletonCustom<BeachSoccerUIManager>.Instance.UpdateMethod();
    }
}
