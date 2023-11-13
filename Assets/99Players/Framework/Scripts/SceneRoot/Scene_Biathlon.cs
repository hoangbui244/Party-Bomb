using UnityEngine;
public class Scene_Biathlon : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isPlaying;
    private void Awake() {
        Init();
    }
    private void Start() {
        PostInit();
    }
    private void Update() {
        UpdateMethod();
    }
    private void FixedUpdate() {
        FixedUpdateMethod();
    }
    private void OnDisable() {
        Time.maximumDeltaTime = 0.3333333f;
    }
    private void Init() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        CharaLeanTweenMotionData.CreateMotionData();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        Time.maximumDeltaTime = 0.1f;
    }
    private void PostInit() {
        SingletonCustom<Biathlon_GameMain>.Instance.Init();
        SingletonCustom<Biathlon_CameraRegistry>.Instance.Init();
        SingletonCustom<Biathlon_Courses>.Instance.Init();
        SingletonCustom<Biathlon_Characters>.Instance.Init();
        SingletonCustom<Biathlon_UI>.Instance.Init();
    }
    private void UpdateMethod() {
        if (Time.timeScale == 0f) {
            if (isPlaying) {
                SingletonCustom<Biathlon_Characters>.Instance.UpdateAudio();
            }
        } else if (!isPlaying) {
            PlayGame();
        } else {
            SingletonCustom<Biathlon_GameMain>.Instance.UpdateMethod();
            SingletonCustom<Biathlon_Characters>.Instance.UpdateMethod();
        }
    }
    private void FixedUpdateMethod() {
        if (Time.timeScale != 0f && isPlaying) {
            SingletonCustom<Biathlon_Characters>.Instance.FixedUpdateMethod();
        }
    }
    private void PlayGame() {
        if (!SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(SingletonCustom<Biathlon_GameMain>.Instance.PlayGame);
            isPlaying = true;
        }
    }
}
