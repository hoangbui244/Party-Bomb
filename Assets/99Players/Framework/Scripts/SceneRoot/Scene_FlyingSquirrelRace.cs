using UnityEngine;
using UnityEngine.Extension;
public class Scene_FlyingSquirrelRace : DecoratedMonoBehaviour {
    [SerializeField]
    [DisplayName("スカイボックス")]
    private Material material;
    private bool isPlaying;
    private void Awake() {
        Initialize();
        PostInitialize();
    }
    private void Update() {
        UpdateMethod();
    }
    private void FixedUpdate() {
        FixedUpdateMethod();
    }
    private void Initialize() {
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(material);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.Initialize();
        SingletonMonoBehaviour<FlyingSquirrelRace_Players>.Instance.Initialize();
        SingletonMonoBehaviour<FlyingSquirrelRace_Stages>.Instance.Initialize();
        SingletonMonoBehaviour<FlyingSquirrelRace_UI>.Instance.Initialize();
        SingletonMonoBehaviour<FlyingSquirrelRace_Camera>.Instance.Initialize();
        SingletonMonoBehaviour<FlyingSquirrelRace_Animation>.Instance.Initialize();
    }
    private void PostInitialize() {
        SingletonMonoBehaviour<FlyingSquirrelRace_Stages>.Instance.PostInitialize();
    }
    private void UpdateMethod() {
        if (!Mathf.Approximately(Time.timeScale, 0f)) {
            if (!isPlaying) {
                PlayGame();
                return;
            }
            SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.UpdateMethod();
            SingletonMonoBehaviour<FlyingSquirrelRace_Players>.Instance.UpdateMethod();
            SingletonMonoBehaviour<FlyingSquirrelRace_Stages>.Instance.UpdateMethod();
        }
    }
    private void FixedUpdateMethod() {
        if (!Mathf.Approximately(Time.timeScale, 0f) && isPlaying) {
            SingletonMonoBehaviour<FlyingSquirrelRace_Stages>.Instance.FixedUpdateMethod();
        }
    }
    private void PlayGame() {
        if (!SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.PlayGame);
            isPlaying = true;
        }
    }
}
