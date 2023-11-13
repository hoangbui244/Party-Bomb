using System;
using System;
using UnityEngine;
public class Scene_BlackSmith : MonoBehaviour {
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
        SingletonCustom<BlackSmith_WeaponManager>.Instance.Init();
        SingletonCustom<BlackSmith_PlayerManager>.Instance.Init();
        SingletonCustom<BlackSmith_UIManager>.Instance.Init();
        SingletonCustom<BlackSmith_CameraManager>.Instance.Init();
        SingletonCustom<BlackSmith_EffectManager>.Instance.Init();
        SingletonCustom<BlackSmith_GameManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            isInit = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                LeanTween.delayedCall(base.gameObject, 1.5f, (Action)delegate {
                    SingletonCustom<CommonStartProduction>.Instance.Play(delegate {
                        SingletonCustom<BlackSmith_GameManager>.Instance.SetIsGameStart();
                    });
                    SingletonCustom<BlackSmith_UIManager>.Instance.HideAnnounceText();
                });
                LeanTween.delayedCall(base.gameObject, 3f, (Action)delegate {
                    isStart = true;
                    LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                    });
                });
            });
        }
        if (isStart && SingletonCustom<BlackSmith_GameManager>.Instance.GetIsGameStart() && !SingletonCustom<BlackSmith_GameManager>.Instance.GetIsGameEnd() && !SingletonCustom<CommonNotificationManager>.Instance.IsPause && !SingletonCustom<BlackSmith_UIManager>.Instance.GetIsShowTextUI()) {
            SingletonCustom<BlackSmith_GameManager>.Instance.UpdateMethod();
            SingletonCustom<BlackSmith_UIManager>.Instance.UpdateMethod();
            SingletonCustom<BlackSmith_PlayerManager>.Instance.UpdateMethod();
        }
    }
}
