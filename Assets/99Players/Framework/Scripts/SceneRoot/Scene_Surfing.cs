using UnityEngine;
public class Scene_Surfing : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        Surfing_Define.PLAYER_NUM = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                Surfing_Define.CPU_NUM = 4 - Surfing_Define.PLAYER_NUM;
                break;
            case GS_Define.GameFormat.COOP:
                if (Surfing_Define.PLAYER_NUM == 2) {
                    Surfing_Define.CPU_NUM = 2;
                } else if (Surfing_Define.PLAYER_NUM == 3) {
                    Surfing_Define.CPU_NUM = 5;
                } else {
                    Surfing_Define.CPU_NUM = 4;
                }
                break;
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                Surfing_Define.CPU_NUM = 0;
                break;
        }
        Surfing_Define.MEMBER_NUM = Surfing_Define.PLAYER_NUM + Surfing_Define.CPU_NUM;
        UnityEngine.Debug.Log("メンバ\u30fc数：" + Surfing_Define.MEMBER_NUM.ToString() + "\u3000プレイヤ\u30fc数：" + Surfing_Define.PLAYER_NUM.ToString() + "\u3000CPU数：" + Surfing_Define.CPU_NUM.ToString());
    }
    private void Start() {
        Surfing_Define.GM.Init();
        Surfing_Define.UIM.Init();
        Surfing_Define.PM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    Surfing_Define.GM.StartCountDown();
                });
                isInit = true;
            }
            Surfing_Define.GM.UpdateMethod();
            Surfing_Define.PM.UpdateMethod();
            Surfing_Define.UIM.UpdateMethod();
        }
    }
}
