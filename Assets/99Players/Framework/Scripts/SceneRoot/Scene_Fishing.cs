using UnityEngine;
public class Scene_Fishing : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        CharaLeanTweenMotionData.CreateMotionData();
    }
    private void Start() {
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        FishingDefinition.MCM.Init();
        FishingDefinition.MGM.Init();
        FishingDefinition.CUIM.Init();
        FishingDefinition.GUIM.Init();
        FishingDefinition.FSM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    FishingDefinition.MGM.StartCountDown();
                });
                isInit = true;
            }
            FishingDefinition.MCM.UpdateMethod();
            FishingDefinition.MGM.UpdateMethod();
            FishingDefinition.CUIM.UpdateMethod();
            FishingDefinition.FSM.UpdateMethod();
        }
    }
    private void FixedUpdate() {
        if (Time.timeScale != 0f && isInit) {
            FishingDefinition.MCM.FixedUpdateMethod();
        }
    }
}
