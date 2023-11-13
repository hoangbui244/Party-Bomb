using System;
using UnityEngine;
public class Scene_Curling : MonoBehaviour {
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
        SingletonCustom<Curling_UIManager>.Instance.Init();
        SingletonCustom<Curling_CurlingRinkManager>.Instance.Init();
        SingletonCustom<Curling_GameManager>.Instance.Init();
        SingletonCustom<Curling_UIManager>.Instance.SetContollerType();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void FixedUpdate() {
        if (isStart && !SingletonCustom<Curling_GameManager>.Instance.GetIsGameEnd() && !SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<Curling_GameManager>.Instance.FixedUpdateMethod();
        }
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            isInit = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                    SingletonCustom<CommonStartSimple>.Instance.Show(SingletonCustom<Curling_GameManager>.Instance.GameStart);
                });
                LeanTween.delayedCall(base.gameObject, 4f, (Action)delegate {
                    isStart = true;
                    SingletonCustom<Curling_GameManager>.Instance.StartPlayGame();
                });
                if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
                    SingletonCustom<Curling_GameManager>.Instance.GroupVibration();
                }
            });
        }
        if (isStart && !SingletonCustom<Curling_GameManager>.Instance.GetIsGameEnd() && !SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<Curling_GameManager>.Instance.UpdateMethod();
            SingletonCustom<Curling_UIManager>.Instance.UpdateMethod();
        }
    }
    private void LateUpdate() {
        if (isStart && !SingletonCustom<Curling_GameManager>.Instance.GetIsGameEnd() && !SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<Curling_GameManager>.Instance.LateUpdateMethod();
            SingletonCustom<Curling_UIManager>.Instance.LateUpdateMethod();
        }
    }
}
