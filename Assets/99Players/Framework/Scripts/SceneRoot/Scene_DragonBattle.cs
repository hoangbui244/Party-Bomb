using UnityEngine;
public class Scene_DragonBattle : MonoBehaviour {
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
    }
    private void Start() {
        SingletonCustom<DragonBattleGameManager>.Instance.Init();
        SingletonCustom<DragonBattlePlayerManager>.Instance.Init();
        SingletonCustom<DragonBattleUIManager>.Instance.Init();
    }
    private void Update() {
        if (Time.timeScale <= 0f) {
            SingletonCustom<DragonBattleUIManager>.Instance.UpdateMethod();
            return;
        }
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                SingletonCustom<DragonBattleGameManager>.Instance.OnGameStart();
            });
            isInit = true;
        }
        SingletonCustom<DragonBattleGameManager>.Instance.UpdateMethod();
        SingletonCustom<DragonBattlePlayerManager>.Instance.UpdateMethod();
        SingletonCustom<DragonBattleUIManager>.Instance.UpdateMethod();
    }
    private void LateUpdate() {
        if (!(Time.timeScale <= 0f)) {
            SingletonCustom<DragonBattleGameManager>.Instance.LateUpdateMethod();
        }
    }
}
