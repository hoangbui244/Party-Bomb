using UnityEngine;
using UnityEngine.Extension;
public class Scene_Shuriken : DecoratedMonoBehaviour {
    [SerializeField]
    [DisplayName("スカイボックス")]
    private Material skybox;
    private bool isPlaying;
    private void Awake() {
        Initialize();
        PostInitialize();
    }
    private void Update() {
        UpdateMethod();
    }
    private void Initialize() {
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        LightingSettings.ChangeSceneLighting();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonMonoBehaviour<Shuriken_GameMain>.Instance.Initialize();
        SingletonMonoBehaviour<Shuriken_Players>.Instance.Initialize();
        SingletonMonoBehaviour<Shuriken_ThrowingShurikenCache>.Instance.Initialize();
        SingletonMonoBehaviour<Shuriken_TargetGenerator>.Instance.Initialize();
        SingletonMonoBehaviour<Shuriken_UI>.Instance.Initialize();
        SingletonMonoBehaviour<Shuriken_Audio>.Instance.Initialize();
    }
    private void PostInitialize() {
        SingletonMonoBehaviour<Shuriken_Players>.Instance.PostInitialize();
    }
    private void UpdateMethod() {
        if (!Mathf.Approximately(Time.timeScale, 0f)) {
            if (!isPlaying) {
                PlayGame();
                return;
            }
            SingletonMonoBehaviour<Shuriken_GameMain>.Instance.UpdateMethod();
            SingletonMonoBehaviour<Shuriken_Players>.Instance.UpdateMethod();
            SingletonMonoBehaviour<Shuriken_TargetGenerator>.Instance.UpdateMethod();
        }
    }
    private void PlayGame() {
        if (!SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(SingletonMonoBehaviour<Shuriken_GameMain>.Instance.PlayGame);
            isPlaying = true;
        }
    }
}
