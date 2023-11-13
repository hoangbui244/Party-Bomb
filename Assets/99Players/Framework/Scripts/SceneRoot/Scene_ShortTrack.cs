using UnityEngine;
public class Scene_ShortTrack : MonoBehaviour {
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        ShortTrack_CharaLeanTweenMotionData.CreateMotionData();
        SHORTTRACK.CAM.Init();
        SHORTTRACK.MGM.Init();
        SHORTTRACK.MCM.Init();
        SHORTTRACK.UIM.Init();
    }
    private void Start() {
        SHORTTRACK.MCM.StartMethod();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    SHORTTRACK.MGM.RoundStart_CountDown();
                });
                isInit = true;
            }
            if (!SHORTTRACK.MGM.IsDuringResult()) {
                SHORTTRACK.MGM.UpdateMethot();
                SHORTTRACK.MCM.UpdateMethod();
                SHORTTRACK.UIM.UpdateMethed();
            }
        }
    }
}
