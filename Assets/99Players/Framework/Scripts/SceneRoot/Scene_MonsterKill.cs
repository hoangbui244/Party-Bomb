using System;
using UnityEngine;
public class Scene_MonsterKill : MonoBehaviour {
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
        SingletonCustom<MonsterKill_FieldManager>.Instance.Init();
        SingletonCustom<MonsterKill_CameraManager>.Instance.Init();
        SingletonCustom<MonsterKill_PlayerManager>.Instance.Init();
        SingletonCustom<MonsterKill_EnemyManager>.Instance.Init();
        SingletonCustom<MonsterKill_UIManager>.Instance.Init();
        SingletonCustom<MonsterKill_GameManager>.Instance.Init();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            isInit = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                    SingletonCustom<CommonStartProduction>.Instance.Play(delegate {
                        SingletonCustom<MonsterKill_GameManager>.Instance.SetIsGameStart();
                    });
                });
                LeanTween.delayedCall(base.gameObject, 3f, (Action)delegate {
                    isStart = true;
                    LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                    });
                });
            });
        }
        SingletonCustom<MonsterKill_UIManager>.Instance.UpdateMethod();
        if (isStart && SingletonCustom<MonsterKill_GameManager>.Instance.GetIsGameStart() && !SingletonCustom<MonsterKill_GameManager>.Instance.GetIsGameEnd() && !SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<MonsterKill_GameManager>.Instance.UpdateMethod();
            if (!SingletonCustom<MonsterKill_GameManager>.Instance.GetIsGameEnd()) {
                SingletonCustom<MonsterKill_PlayerManager>.Instance.UpdateMethod();
                SingletonCustom<MonsterKill_EnemyManager>.Instance.UpdateMethod();
            }
        }
    }
    private void FixedUpdate() {
        if (!SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<MonsterKill_CameraManager>.Instance.FixedUpdateMethod();
        }
    }
}
