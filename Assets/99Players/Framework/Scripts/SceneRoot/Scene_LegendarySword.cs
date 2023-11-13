using UnityEngine;
public class Scene_LegendarySword : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        LegendarySword_Define.PLAYER_NUM = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                LegendarySword_Define.CPU_NUM = 4 - LegendarySword_Define.PLAYER_NUM;
                break;
            case GS_Define.GameFormat.COOP:
                if (LegendarySword_Define.PLAYER_NUM == 2) {
                    LegendarySword_Define.CPU_NUM = 2;
                } else if (LegendarySword_Define.PLAYER_NUM == 3) {
                    LegendarySword_Define.CPU_NUM = 5;
                } else {
                    LegendarySword_Define.CPU_NUM = 4;
                }
                break;
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                LegendarySword_Define.CPU_NUM = 0;
                break;
        }
        LegendarySword_Define.MEMBER_NUM = LegendarySword_Define.PLAYER_NUM + LegendarySword_Define.CPU_NUM;
        UnityEngine.Debug.Log("メンバ\u30fc数：" + LegendarySword_Define.MEMBER_NUM.ToString() + "\u3000プレイヤ\u30fc数：" + LegendarySword_Define.PLAYER_NUM.ToString() + "\u3000CPU数：" + LegendarySword_Define.CPU_NUM.ToString());
        LegendarySword_Define.GM.Init(delegate {
        });
        LegendarySword_Define.UIM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                });
                isInit = true;
            }
            LegendarySword_Define.GM.UpdateMethod();
            LegendarySword_Define.PM.UpdateMethod();
        }
    }
}
