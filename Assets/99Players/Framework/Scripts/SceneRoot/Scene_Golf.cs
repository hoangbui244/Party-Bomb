using System;
using UnityEngine;
public class Scene_Golf : MonoBehaviour {
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
        SingletonCustom<Golf_GameManager>.Instance.Init();
        SingletonCustom<Golf_WindManager>.Instance.Init();
        SingletonCustom<Golf_FieldManager>.Instance.Init();
        SingletonCustom<Golf_PlayerManager>.Instance.Init();
        SingletonCustom<Golf_BallManager>.Instance.Init();
        SingletonCustom<Golf_CursorManager>.Instance.Init();
        SingletonCustom<Golf_CameraManager>.Instance.Init();
        SingletonCustom<Golf_ViewCupLineManager>.Instance.Init();
        SingletonCustom<Golf_UIManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            isInit = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                    SingletonCustom<CommonStartSimple>.Instance.Show(delegate {
                    });
                });
                LeanTween.delayedCall(base.gameObject, 3f, (Action)delegate {
                    isStart = true;
                    LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                        SingletonCustom<Golf_GameManager>.Instance.GameStart();
                    });
                });
            });
        }
        if (isStart && SingletonCustom<Golf_GameManager>.Instance.GetIsGameStart() && !SingletonCustom<Golf_GameManager>.Instance.GetIsGameEnd() && !SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<Golf_PlayerManager>.Instance.UpdateMethod();
            SingletonCustom<Golf_BallManager>.Instance.UpdateMethod();
            SingletonCustom<Golf_CursorManager>.Instance.UpdateMethod();
            SingletonCustom<Golf_ViewCupLineManager>.Instance.UpdateMethod();
            SingletonCustom<Golf_UIManager>.Instance.UpdateMethod();
        }
    }
    private void FixedUpdate() {
        if (SingletonCustom<Golf_GameManager>.Instance.GetIsGameStart() && !SingletonCustom<Golf_GameManager>.Instance.GetIsGameEnd()) {
            SingletonCustom<Golf_BallManager>.Instance.FixedUpdateMethod();
            SingletonCustom<Golf_CameraManager>.Instance.FixedUpdateMethod();
        }
    }
}
