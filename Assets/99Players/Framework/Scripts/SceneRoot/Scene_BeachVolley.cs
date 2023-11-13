using UnityEngine;
public class Scene_BeachVolley : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        BeachVolley_Define.PLAYER_NUM = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                BeachVolley_Define.CPU_NUM = 4 - BeachVolley_Define.PLAYER_NUM;
                break;
            case GS_Define.GameFormat.COOP:
                if (BeachVolley_Define.PLAYER_NUM == 2) {
                    BeachVolley_Define.CPU_NUM = 2;
                } else if (BeachVolley_Define.PLAYER_NUM == 3) {
                    BeachVolley_Define.CPU_NUM = 5;
                } else {
                    BeachVolley_Define.CPU_NUM = 4;
                }
                break;
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                BeachVolley_Define.CPU_NUM = 0;
                break;
        }
        BeachVolley_Define.MEMBER_NUM = BeachVolley_Define.PLAYER_NUM + BeachVolley_Define.CPU_NUM;
        UnityEngine.Debug.Log("メンバ\u30fc数：" + BeachVolley_Define.MEMBER_NUM.ToString() + "\u3000プレイヤ\u30fc数：" + BeachVolley_Define.PLAYER_NUM.ToString() + "\u3000CPU数：" + BeachVolley_Define.CPU_NUM.ToString());
        bool isSinglePlay = SingletonCustom<GameSettingManager>.Instance.IsSinglePlay;
    }
    private void Start() {
        BeachVolley_Define.CM.Init();
        BeachVolley_Define.FM.Init();
        BeachVolley_Define.BM.Init();
        SingletonCustom<BeachVolley_CharacterNameManager>.Instance.Init();
        BeachVolley_Define.MCM.Init();
        BeachVolley_Define.MGM.Init(delegate {
            ToResult();
        });
        BeachVolley_Define.GUM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                });
                isInit = true;
            }
            BeachVolley_Define.CM.UpdateMethod();
            BeachVolley_Define.MCM.UpdateMethod();
            BeachVolley_Define.BM.UpdateMethod();
            BeachVolley_Define.MGM.UpdateMethod();
            BeachVolley_Define.FM.UpdateMethod();
            BeachVolley_Define.GUM.UpdateMethod();
        }
    }
    private void ToResult() {
    }
}
