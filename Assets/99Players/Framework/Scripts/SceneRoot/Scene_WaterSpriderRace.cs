using UnityEngine;
public class Scene_WaterSpriderRace : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        WaterSpriderRace_Define.PLAYER_NUM = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                WaterSpriderRace_Define.CPU_NUM = 4 - WaterSpriderRace_Define.PLAYER_NUM;
                break;
            case GS_Define.GameFormat.COOP:
                if (WaterSpriderRace_Define.PLAYER_NUM == 2) {
                    WaterSpriderRace_Define.CPU_NUM = 2;
                } else if (WaterSpriderRace_Define.PLAYER_NUM == 3) {
                    WaterSpriderRace_Define.CPU_NUM = 5;
                } else {
                    WaterSpriderRace_Define.CPU_NUM = 4;
                }
                break;
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                WaterSpriderRace_Define.CPU_NUM = 0;
                break;
        }
        WaterSpriderRace_Define.MEMBER_NUM = WaterSpriderRace_Define.PLAYER_NUM + WaterSpriderRace_Define.CPU_NUM;
        UnityEngine.Debug.Log("メンバ\u30fc数：" + WaterSpriderRace_Define.MEMBER_NUM.ToString() + "\u3000プレイヤ\u30fc数：" + WaterSpriderRace_Define.PLAYER_NUM.ToString() + "\u3000CPU数：" + WaterSpriderRace_Define.CPU_NUM.ToString());
    }
    private void Start() {
        WaterSpriderRace_Define.GM.Init();
        WaterSpriderRace_Define.UIM.Init();
        WaterSpriderRace_Define.PM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    WaterSpriderRace_Define.GM.StartCountDown();
                });
                isInit = true;
            }
            WaterSpriderRace_Define.GM.UpdateMethod();
            WaterSpriderRace_Define.PM.UpdateMethod();
            WaterSpriderRace_Define.UIM.UpdateMethod();
        }
    }
}
