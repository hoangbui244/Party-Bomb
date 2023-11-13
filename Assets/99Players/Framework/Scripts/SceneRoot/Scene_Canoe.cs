using System;
using UnityEngine;
public class Scene_Canoe : MonoBehaviour {
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
        SingletonCustom<Canoe_CourseManager>.Instance.Init();
        SingletonCustom<Canoe_PlayerManager>.Instance.Init();
        SingletonCustom<Canoe_CameraManager>.Instance.Init();
        SingletonCustom<Canoe_CloudManager>.Instance.Init();
        SingletonCustom<Canoe_UIManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            isInit = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                    SingletonCustom<CommonStartProduction>.Instance.Play(delegate {
                        SingletonCustom<Canoe_GameManager>.Instance.SetIsGameStart();
                    });
                });
                LeanTween.delayedCall(base.gameObject, 3f, (Action)delegate {
                    isStart = true;
                    LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                        SingletonCustom<Canoe_GameManager>.Instance.GameStart();
                    });
                });
            });
        }
        if (isStart && SingletonCustom<Canoe_GameManager>.Instance.GetIsGameStart()) {
            SingletonCustom<Canoe_PlayerManager>.Instance.UpdateMethod();
            if (!SingletonCustom<Canoe_GameManager>.Instance.GetIsGameEnd() && !SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
                SingletonCustom<Canoe_GameManager>.Instance.UpdateMethod();
                SingletonCustom<Canoe_CloudManager>.Instance.UpdateMethod();
            }
        }
    }
    private void FixedUpdate() {
        if (SingletonCustom<Canoe_GameManager>.Instance.GetIsGameStart()) {
            SingletonCustom<Canoe_PlayerManager>.Instance.FixedUpdateMethod();
            SingletonCustom<Canoe_CameraManager>.Instance.FixedUpdateMethod();
        }
    }
}
