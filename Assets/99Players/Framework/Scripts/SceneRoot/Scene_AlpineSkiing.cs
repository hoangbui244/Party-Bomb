using UnityEngine;
public class Scene_AlpineSkiing : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void Awake() {
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        AlpineSkiing_Define.PLAYER_NUM = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat) {
            case GS_Define.GameFormat.BATTLE:
                AlpineSkiing_Define.CPU_NUM = 4 - AlpineSkiing_Define.PLAYER_NUM;
                break;
            case GS_Define.GameFormat.COOP:
                if (AlpineSkiing_Define.PLAYER_NUM == 2) {
                    AlpineSkiing_Define.CPU_NUM = 2;
                } else if (AlpineSkiing_Define.PLAYER_NUM == 3) {
                    AlpineSkiing_Define.CPU_NUM = 5;
                } else {
                    AlpineSkiing_Define.CPU_NUM = 4;
                }
                break;
            case GS_Define.GameFormat.BATTLE_AND_COOP:
                AlpineSkiing_Define.CPU_NUM = 0;
                break;
        }
        AlpineSkiing_Define.MEMBER_NUM = AlpineSkiing_Define.PLAYER_NUM + AlpineSkiing_Define.CPU_NUM;
        UnityEngine.Debug.Log("メンバ\u30fc数：" + AlpineSkiing_Define.MEMBER_NUM.ToString() + "\u3000プレイヤ\u30fc数：" + AlpineSkiing_Define.PLAYER_NUM.ToString() + "\u3000CPU数：" + AlpineSkiing_Define.CPU_NUM.ToString());
    }
    private void Start() {
        AlpineSkiing_Define.GM.Init();
        AlpineSkiing_Define.UIM.Init();
        AlpineSkiing_Define.PM.Init();
    }
    private void Update() {
        if (Time.timeScale != 0f) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    AlpineSkiing_Define.GM.StartCountDown();
                });
                isInit = true;
            }
            AlpineSkiing_Define.GM.UpdateMethod();
            AlpineSkiing_Define.PM.UpdateMethod();
            AlpineSkiing_Define.UIM.UpdateMethod();
        }
    }
    private void OnDisable() {
        LeanTween.cancelAll();
    }
}
