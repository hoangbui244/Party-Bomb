using System;
using UnityEngine;
public class Scene_RoadRace : MonoBehaviour {
    private bool isInit;
    [SerializeField]
    private Material skybox;
    public static RoadRaceCharacterManager CM => SingletonCustom<RoadRaceCharacterManager>.Instance;
    public static RoadRaceGameManager GM => SingletonCustom<RoadRaceGameManager>.Instance;
    public static RoadRaceFieldManager FM => SingletonCustom<RoadRaceFieldManager>.Instance;
    public static RoadRaceUiManager UM => SingletonCustom<RoadRaceUiManager>.Instance;
    private void LayerEnd(SceneManager.LayerCloseType _closeType) {
    }
    private void OpenLayer() {
        SingletonCustom<SceneManager>.Instance.AddNowLayerCloseCallBack(LayerEnd);
    }
    private bool IsActive() {
        return SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack() == new SceneManager.LayerClose(LayerEnd);
    }
    private void Awake() {
        OpenLayer();
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
    }
    private void Start() {
        SingletonCustom<RoadRaceGameManager>.Instance.Init();
    }
    private void CheckStart() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            UnityEngine.Debug.Log("isInit = true;");
            isInit = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate {
                    SingletonCustom<CommonStartProduction>.Instance.Play(delegate {
                        GM.GameStart();
                        UnityEngine.Debug.Log("ゲ\u30fcム開始");
                    });
                });
                if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
                    GM.PlayVibration();
                }
            });
        }
    }
    private void Update() {
        CheckStart();
        if (!SingletonCustom<CommonNotificationManager>.Instance.IsPause && GM.IsGameStart) {
            SingletonCustom<RoadRaceGameManager>.Instance.UpdateMethod();
        }
    }
    private void LateUpdate() {
        if (!SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<RoadRaceGameManager>.Instance.LateUpdateMethod();
            bool isGameStart = GM.IsGameStart;
        }
    }
    private void FixedUpdate() {
        if (!SingletonCustom<CommonNotificationManager>.Instance.IsPause) {
            SingletonCustom<RoadRaceGameManager>.Instance.FixedUpdateMethod();
        }
    }
    public static float Floor(float _value, float _decimalPoint) {
        float num = Mathf.Pow(10f, _decimalPoint - 1f);
        _value *= num;
        _value = Mathf.Floor(_value);
        _value /= num;
        return _value;
    }
}
