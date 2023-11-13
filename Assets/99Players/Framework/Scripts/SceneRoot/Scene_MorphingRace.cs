using System;
using UnityEngine;
public class Scene_MorphingRace : MonoBehaviour {
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
        SingletonCustom<MorphingRace_CameraManager>.Instance.Init();
        SingletonCustom<MorphingRace_PlayerManager>.Instance.Init();
        SingletonCustom<MorphingRace_UIManager>.Instance.Init();
        SingletonCustom<MorphingRace_FieldManager>.Instance.Init();
        SingletonCustom<MorphingRace_CloudManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            isInit = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                    SingletonCustom<CommonStartProduction>.Instance.Play(delegate {
                        SingletonCustom<MorphingRace_GameManager>.Instance.SetIsGameStart();
                        for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++) {
                            if (!SingletonCustom<MorphingRace_PlayerManager>.Instance.GetPlayer(i).GetIsCpu()) {
                                SingletonCustom<MorphingRace_UIManager>.Instance.SetControllerBalloonActive(i, MorphingRace_UIManager.ControllerUIType.Common, _isFade: true, _isActive: true);
                            }
                        }
                    });
                });
                LeanTween.delayedCall(base.gameObject, 3f, (Action)delegate {
                    isStart = true;
                    SingletonCustom<MorphingRace_GameManager>.Instance.GameStart();
                });
                if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
                    SingletonCustom<MorphingRace_GameManager>.Instance.GroupVibration();
                }
            });
        }
        if (isStart && SingletonCustom<MorphingRace_GameManager>.Instance.GetIsGameStart() && !SingletonCustom<MorphingRace_GameManager>.Instance.GetIsGameEnd() && !SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<MorphingRace_GameManager>.Instance.UpdateMethod();
            SingletonCustom<MorphingRace_PlayerManager>.Instance.UpdateMethod();
            SingletonCustom<MorphingRace_UIManager>.Instance.UpdateMethod();
            SingletonCustom<MorphingRace_CameraManager>.Instance.UpdateMethod();
            SingletonCustom<MorphingRace_CloudManager>.Instance.UpdateMethod();
        }
    }
}
