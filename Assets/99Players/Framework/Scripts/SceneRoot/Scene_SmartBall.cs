using UnityEngine;
public class Scene_SmartBall : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Start() {
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        SB.MGM.Init();
        SB.MCM.Init();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                SB.MGM.StartCountDown();
            });
            isInit = true;
        }
        SB.MGM.UpdateMethod();
        SB.MCM.UpdateMethod();
    }
}
