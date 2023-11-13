using System;
using UnityEngine;
public class Scene_RockClimbing : MonoBehaviour {
    [SerializeField]
    private Material skybox;
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
    private void OnEnable() {
        OpenLayer();
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        SingletonCustom<RockClimbing_PlayerManager>.Instance.Init();
        SingletonCustom<RockClimbing_UIManager>.Instance.Init();
        SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.Init();
        SingletonCustom<RockClimbing_CameraManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            isInit = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                SingletonCustom<RockClimbing_PlayerManager>.Instance.SetCpuFirstClimbOnFoundation();
                LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                    SingletonCustom<CommonStartProduction>.Instance.Play(delegate {
                        SingletonCustom<RockClimbing_GameManager>.Instance.GameStart();
                    });
                });
                LeanTween.delayedCall(base.gameObject, 3f, (Action)delegate {
                    isStart = true;
                    SingletonCustom<RockClimbing_GameManager>.Instance.GameStartAnimation();
                });
                if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
                    SingletonCustom<RockClimbing_GameManager>.Instance.GroupVibration();
                }
            });
        }
        if (isStart && SingletonCustom<RockClimbing_GameManager>.Instance.GetIsGameStart() && !SingletonCustom<RockClimbing_GameManager>.Instance.GetIsGameEnd() && !SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<RockClimbing_GameManager>.Instance.UpdateMethod();
            SingletonCustom<RockClimbing_PlayerManager>.Instance.UpdateMethod();
            SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.UpdateMethod();
        }
    }
    private void LateUpdate() {
        if (isStart && SingletonCustom<RockClimbing_GameManager>.Instance.GetIsGameStart() && !SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<RockClimbing_CameraManager>.Instance.LateUpdateMethod();
        }
    }
}
