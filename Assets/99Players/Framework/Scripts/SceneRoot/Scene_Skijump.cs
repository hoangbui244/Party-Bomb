using UnityEngine;
public class Scene_Skijump : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        Skijump_Define.PLAYER_NUM = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                Skijump_Define.CPU_NUM = 4 - Skijump_Define.PLAYER_NUM;
                break;
            case GS_Define.GameFormat.COOP:
                if (Skijump_Define.PLAYER_NUM == 2) {
                    Skijump_Define.CPU_NUM = 2;
                } else if (Skijump_Define.PLAYER_NUM == 3) {
                    Skijump_Define.CPU_NUM = 5;
                } else {
                    Skijump_Define.CPU_NUM = 4;
                }
                break;
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                Skijump_Define.CPU_NUM = 0;
                break;
        }
        Skijump_Define.MEMBER_NUM = Skijump_Define.PLAYER_NUM + Skijump_Define.CPU_NUM;
        UnityEngine.Debug.Log("メンバ\u30fc数：" + Skijump_Define.MEMBER_NUM.ToString() + "\u3000プレイヤ\u30fc数：" + Skijump_Define.PLAYER_NUM.ToString() + "\u3000CPU数：" + Skijump_Define.CPU_NUM.ToString());
    }
    private void Start() {
        Skijump_Define.MGM.Init();
        Skijump_Define.MSM.Init();
        Skijump_Define.MCM.Init();
        Skijump_Define.GUIM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    Skijump_Define.MGM.StartProduction();
                });
                isInit = true;
            }
            Skijump_Define.MGM.UpdateMethod();
            Skijump_Define.MCM.UpdateMethod();
            Skijump_Define.GUIM.UpdateMethod();
        }
    }
    private void LateUpdate() {
        Skijump_Define.MSM.LateUpdateMethod();
    }
}
