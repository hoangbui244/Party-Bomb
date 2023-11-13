using UnityEngine;
public class Scene_ShavedIce : MonoBehaviour {
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
        ShavedIce_Define.GM.Init();
        ShavedIce_Define.PM.Init();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                ShavedIce_Define.GM.StartCountDown();
            });
            isInit = true;
        }
        ShavedIce_Define.GM.UpdateMethod();
        ShavedIce_Define.PM.UpdateMethod();
    }
}
