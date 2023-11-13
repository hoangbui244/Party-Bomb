using UnityEngine;
public class Scene_Bowling : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        Bowling_Define.PLAYER_NUM = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                Bowling_Define.CPU_NUM = 4 - Bowling_Define.PLAYER_NUM;
                break;
            case GS_Define.GameFormat.COOP:
                if (Bowling_Define.PLAYER_NUM == 2) {
                    Bowling_Define.CPU_NUM = 2;
                } else if (Bowling_Define.PLAYER_NUM == 3) {
                    Bowling_Define.CPU_NUM = 5;
                } else {
                    Bowling_Define.CPU_NUM = 4;
                }
                break;
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                Bowling_Define.CPU_NUM = 0;
                break;
        }
        Bowling_Define.MEMBER_NUM = Bowling_Define.PLAYER_NUM + Bowling_Define.CPU_NUM;
        UnityEngine.Debug.Log("メンバ\u30fc数：" + Bowling_Define.MEMBER_NUM.ToString() + "\u3000プレイヤ\u30fc数：" + Bowling_Define.PLAYER_NUM.ToString() + "\u3000CPU数：" + Bowling_Define.CPU_NUM.ToString());
    }
    private void Start() {
        Bowling_Define.MGM.Init();
        Bowling_Define.MPM.Init();
        Bowling_Define.MSM.Init();
        Bowling_Define.GUIM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    Bowling_Define.MGM.StartProduction();
                });
                isInit = true;
            }
            Bowling_Define.MGM.UpdateMethod();
            Bowling_Define.MPM.UpdateMethod();
            Bowling_Define.GUIM.UpdateMethod();
            Bowling_Define.MSM.UpdateMethod();
        }
    }
}
