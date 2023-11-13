using UnityEngine;
public class Scene_HandSeal : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        HandSeal_Define.CM.SetttingHoldInputIntervalMode();
        HandSeal_Define.PLAYER_NUM = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                HandSeal_Define.CPU_NUM = 4 - HandSeal_Define.PLAYER_NUM;
                break;
            case GS_Define.GameFormat.COOP:
                if (HandSeal_Define.PLAYER_NUM == 2) {
                    HandSeal_Define.CPU_NUM = 2;
                } else if (HandSeal_Define.PLAYER_NUM == 3) {
                    HandSeal_Define.CPU_NUM = 5;
                } else {
                    HandSeal_Define.CPU_NUM = 4;
                }
                break;
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                HandSeal_Define.CPU_NUM = 0;
                break;
        }
        HandSeal_Define.MEMBER_NUM = HandSeal_Define.PLAYER_NUM + HandSeal_Define.CPU_NUM;
        UnityEngine.Debug.Log("メンバ\u30fc数：" + HandSeal_Define.MEMBER_NUM.ToString() + "\u3000プレイヤ\u30fc数：" + HandSeal_Define.PLAYER_NUM.ToString() + "\u3000CPU数：" + HandSeal_Define.CPU_NUM.ToString());
    }
    private void Start() {
        HandSeal_Define.GM.Init();
        HandSeal_Define.UIM.Init();
        HandSeal_Define.PM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    HandSeal_Define.GM.StartCountDown();
                });
                isInit = true;
            }
            HandSeal_Define.GM.UpdateMethod();
            HandSeal_Define.PM.UpdateMethod();
            HandSeal_Define.UIM.UpdateMethod();
        }
    }
}
