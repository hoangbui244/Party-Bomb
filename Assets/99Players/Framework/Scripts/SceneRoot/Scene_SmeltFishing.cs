using UnityEngine;
public class Scene_SmeltFishing : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isPlaying;
    private void Awake() {
        Init();
    }
    private void Update() {
        UpdateMethod();
    }
    private void FixedUpdate() {
        FixedUpdateMethod();
    }
    private void Init() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        CharaLeanTweenMotionData.CreateMotionData();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        SingletonCustom<SmeltFishing_GameMain>.Instance.Init();
        SingletonCustom<SmeltFishing_UI>.Instance.Init();
        SingletonCustom<SmeltFishing_Field>.Instance.Init();
        SingletonCustom<SmeltFishing_SchoolOfFish>.Instance.Init();
        SingletonCustom<SmeltFishing_Characters>.Instance.Init();
    }
    private void UpdateMethod() {
        if (Time.timeScale != 0f) {
            if (!isPlaying) {
                PlayGame();
                return;
            }
            SingletonCustom<SmeltFishing_GameMain>.Instance.UpdateMethod();
            SingletonCustom<SmeltFishing_Characters>.Instance.UpdateMethod();
            SingletonCustom<SmeltFishing_SchoolOfFish>.Instance.UpdateMethod();
            SingletonCustom<SmeltFishing_Field>.Instance.UpdateMethod();
        }
    }
    private void FixedUpdateMethod() {
        if (Time.timeScale != 0f && isPlaying) {
            SingletonCustom<SmeltFishing_Characters>.Instance.FixedUpdateMethod();
        }
    }
    private void PlayGame() {
        if (!SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(SingletonCustom<SmeltFishing_GameMain>.Instance.PlayGame);
            isPlaying = true;
        }
    }
}
