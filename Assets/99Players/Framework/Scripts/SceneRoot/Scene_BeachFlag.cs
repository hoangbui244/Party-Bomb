using UnityEngine;
public class Scene_BeachFlag : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        BeachFlag_Define.PLAYER_NUM = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                BeachFlag_Define.CPU_NUM = 4 - BeachFlag_Define.PLAYER_NUM;
                break;
            case GS_Define.GameFormat.COOP:
                if (BeachFlag_Define.PLAYER_NUM == 2) {
                    BeachFlag_Define.CPU_NUM = 2;
                } else if (BeachFlag_Define.PLAYER_NUM == 3) {
                    BeachFlag_Define.CPU_NUM = 5;
                } else {
                    BeachFlag_Define.CPU_NUM = 4;
                }
                break;
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                BeachFlag_Define.CPU_NUM = 0;
                break;
        }
        BeachFlag_Define.MEMBER_NUM = BeachFlag_Define.PLAYER_NUM + BeachFlag_Define.CPU_NUM;
        UnityEngine.Debug.Log("メンバ\u30fc数：" + BeachFlag_Define.MEMBER_NUM.ToString() + "\u3000プレイヤ\u30fc数：" + BeachFlag_Define.PLAYER_NUM.ToString() + "\u3000CPU数：" + BeachFlag_Define.CPU_NUM.ToString());
    }
    private void Start() {
        BeachFlag_Define.GM.Init(delegate {
        });
        BeachFlag_Define.UIM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                });
                isInit = true;
            }
            BeachFlag_Define.GM.UpdateMethod();
            BeachFlag_Define.PM.UpdateMethod();
            BeachFlag_Define.UIM.UpdateMethod();
        }
    }
}
