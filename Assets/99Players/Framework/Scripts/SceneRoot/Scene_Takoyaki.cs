using UnityEngine;
public class Scene_Takoyaki : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        Takoyaki_Define.CM.SetttingHoldInputIntervalMode();
        Takoyaki_Define.PLAYER_NUM = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                Takoyaki_Define.CPU_NUM = 4 - Takoyaki_Define.PLAYER_NUM;
                break;
            case GS_Define.GameFormat.COOP:
                if (Takoyaki_Define.PLAYER_NUM == 2) {
                    Takoyaki_Define.CPU_NUM = 2;
                } else if (Takoyaki_Define.PLAYER_NUM == 3) {
                    Takoyaki_Define.CPU_NUM = 5;
                } else {
                    Takoyaki_Define.CPU_NUM = 4;
                }
                break;
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                Takoyaki_Define.CPU_NUM = 0;
                break;
        }
        Takoyaki_Define.MEMBER_NUM = Takoyaki_Define.PLAYER_NUM + Takoyaki_Define.CPU_NUM;
        UnityEngine.Debug.Log("メンバ\u30fc数：" + Takoyaki_Define.MEMBER_NUM.ToString() + "\u3000プレイヤ\u30fc数：" + Takoyaki_Define.PLAYER_NUM.ToString() + "\u3000CPU数：" + Takoyaki_Define.CPU_NUM.ToString());
    }
    private void Start() {
        Takoyaki_Define.GM.Init();
        Takoyaki_Define.UIM.Init();
        Takoyaki_Define.PM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    Takoyaki_Define.GM.StartCountDown();
                });
                isInit = true;
            }
            Takoyaki_Define.GM.UpdateMethod();
            Takoyaki_Define.PM.UpdateMethod();
            Takoyaki_Define.UIM.UpdateMethod();
        }
    }
    private void OnDisable() {
        LeanTween.cancelAll();
    }
}
