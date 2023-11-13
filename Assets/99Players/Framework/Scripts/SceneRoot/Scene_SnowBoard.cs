using UnityEngine;
public class Scene_SnowBoard : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
    }
    private void Start() {
        SnowBoard_Define.GM.Init();
        SnowBoard_Define.UIM.Init();
        SnowBoard_Define.PM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    SnowBoard_Define.GM.StartCountDown();
                });
                isInit = true;
            }
            SnowBoard_Define.GM.UpdateMethod();
            SnowBoard_Define.PM.UpdateMethod();
            SnowBoard_Define.UIM.UpdateMethod();
        }
    }
    private void OnDisable() {
        LeanTween.cancelAll();
    }
}
