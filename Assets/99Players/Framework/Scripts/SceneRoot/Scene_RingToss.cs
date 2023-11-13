using System;
using UnityEngine;
public class Scene_RingToss : MonoBehaviour {
    private bool isInit;
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
        RingToss_Define.IS_BATTLE_MODE = false;
        RingToss_Define.IS_TEAM_MODE = false;
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
            RingToss_Define.IS_BATTLE_MODE = true;
        } else {
            RingToss_Define.IS_TEAM_MODE = true;
        }
    }
    private void OnEnable() {
        OpenLayer();
        LightingSettings.ChangeSceneLighting();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        SingletonCustom<RingToss_GameManager>.Instance.Init();
        SingletonCustom<RingToss_RingManager>.Instance.Init();
        SingletonCustom<RingToss_ControllerManager>.Instance.Init();
        SingletonCustom<RingToss_TargetManager>.Instance.Init();
        SingletonCustom<RingToss_ScoreManager>.Instance.Init();
        SingletonCustom<RingToss_UIManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        if (!SingletonCustom<RingToss_GameManager>.Instance.IsGameEnd) {
            if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
                isInit = true;
                SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                    isStart = true;
                    SingletonCustom<RingToss_UIManager>.Instance.ViewFirstControlInfo();
                    if (SingletonCustom<RingToss_GameManager>.Instance.IsSingle) {
                        SingletonCustom<RingToss_GameManager>.Instance.SingleCameraMoveDirection();
                        LeanTween.delayedCall(1f, (Action)delegate {
                            SingletonCustom<CommonStartSimple>.Instance.Show(SingletonCustom<RingToss_GameManager>.Instance.GameStart);
                        });
                    } else {
                        LeanTween.delayedCall(1f, (Action)delegate {
                            SingletonCustom<CommonStartSimple>.Instance.Show(SingletonCustom<RingToss_GameManager>.Instance.GameStart);
                        });
                        SingletonCustom<RingToss_GameManager>.Instance.GroupVibration();
                    }
                });
            }
            if (isStart) {
                SingletonCustom<RingToss_GameManager>.Instance.UpdateMethod();
                SingletonCustom<RingToss_RingManager>.Instance.UpdateMethod();
                SingletonCustom<RingToss_ControllerManager>.Instance.UpdateMethod();
                SingletonCustom<RingToss_TargetManager>.Instance.UpdateMethod();
                SingletonCustom<RingToss_UIManager>.Instance.UpdateMethod();
            }
        }
    }
}
