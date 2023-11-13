using System;
using UnityEngine;
public class Scene_Shooting : MonoBehaviour {
    private bool isInit;
    [SerializeField]
    [Header("空の模様")]
    private Material[] skybox;
    private bool isStart;
    private void LayerEnd(SceneManager.LayerCloseType _closeType) {
    }
    private void OpenLayer() {
        SingletonCustom<SceneManager>.Instance.AddNowLayerCloseCallBack(LayerEnd);
    }
    private bool IsActive() {
        return SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack() == new SceneManager.LayerClose(LayerEnd);
    }
    private void QuitGame() {
    }
    private void Awake() {
        Shooting_Define.IS_BATTLE_MODE = false;
        Shooting_Define.IS_TEAM_MODE = false;
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            Shooting_Define.IS_BATTLE_MODE = true;
        } else {
            Shooting_Define.IS_TEAM_MODE = true;
        }
    }
    private void OnEnable() {
        OpenLayer();
        LightingSettings.ChangeSceneLighting();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        SingletonCustom<Shooting_StageManager>.Instance.Init();
        SingletonCustom<Shooting_GameManager>.Instance.Init();
        SingletonCustom<Shooting_TargetManager>.Instance.Init();
        SingletonCustom<Shooting_ControllerManager>.Instance.Init();
        SingletonCustom<Shooting_ScoreManager>.Instance.Init();
        SingletonCustom<Shooting_UIManager>.Instance.Init();
        SingletonCustom<Shooting_BulletManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        if (!SingletonCustom<Shooting_GameManager>.Instance.IsGameEnd) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                isInit = true;
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    isStart = true;
                    SingletonCustom<Shooting_UIManager>.Instance.ViewFirstControlInfo();
                    if (SingletonCustom<Shooting_GameManager>.Instance.IsSingle) {
                        SingletonCustom<Shooting_GameManager>.Instance.SingleCameraMoveDirection();
                        LeanTween.delayedCall(1f, (Action)delegate {
                            SingletonCustom<CommonStartSimple>.Instance.Show(SingletonCustom<Shooting_GameManager>.Instance.GameStart);
                        });
                    } else {
                        LeanTween.delayedCall(1f, (Action)delegate {
                            SingletonCustom<CommonStartSimple>.Instance.Show(SingletonCustom<Shooting_GameManager>.Instance.GameStart);
                        });
                        SingletonCustom<Shooting_GameManager>.Instance.GroupVibration();
                    }
                });
            }
            if (isStart) {
                SingletonCustom<Shooting_GameManager>.Instance.UpdateMethod();
                SingletonCustom<Shooting_ControllerManager>.Instance.UpdateMethod();
                SingletonCustom<Shooting_TargetManager>.Instance.UpdateMethod();
                SingletonCustom<Shooting_UIManager>.Instance.UpdateMethod();
            }
        }
    }
}
