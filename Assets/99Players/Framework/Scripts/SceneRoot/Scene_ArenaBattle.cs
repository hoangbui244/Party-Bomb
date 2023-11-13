using UnityEngine;
public class Scene_ArenaBattle : MonoBehaviour {
    private bool isInit;
    private bool isStart;
    private void OnEnable() {
        LightingSettings.ChangeSceneLighting();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Start() {
        SingletonCustom<ArenaBattleGameManager>.Instance.Init();
        SingletonCustom<ArenaBattlePlayerManager>.Instance.Init();
        SingletonCustom<ArenaBattleUIManager>.Instance.Init();
        SingletonCustom<ArenaBattleCameraMover>.Instance.Init();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            isInit = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                isStart = true;
                SingletonCustom<ArenaBattleGameManager>.Instance.OnGameStart();
            });
        }
        if (!isStart) {
            SingletonCustom<ArenaBattleUIManager>.Instance.UpdateMethod();
            return;
        }
        SingletonCustom<ArenaBattleGameManager>.Instance.UpdateMethod();
        SingletonCustom<ArenaBattlePlayerManager>.Instance.UpdateMethod();
        SingletonCustom<ArenaBattleUIManager>.Instance.UpdateMethod();
    }
}
