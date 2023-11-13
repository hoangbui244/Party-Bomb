using UnityEngine;
public class Scene_WaterSlider : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private bool isInit;
    private void LayerEnd(SceneManager.LayerCloseType _closeType) {
        UnityEngine.Debug.Log("BBBBBBBBBBBBBBBBBBBBBBBBB");
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
    }
    private void OnEnable() {
        OpenLayer();
        LightingSettings.ChangeSceneLighting();
        LightingSettings.SetSkybox(skybox);
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        SingletonCustom<WaterSlider_GameManager>.Instance.Init();
        SingletonCustom<WaterSlider_UIManager>.Instance.Init();
        SingletonCustom<WaterSlider_GameManager>.Instance.ChangeCourse();
        SingletonCustom<WaterSlider_CourseManager>.Instance.ChangeCourse();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                SingletonCustom<WaterSlider_GameManager>.Instance.GameStart();
            });
            SingletonCustom<CommonNotificationManager>.Instance.OnCourseSelectCursorMove.AddListener(delegate {
                SingletonCustom<WaterSlider_GameManager>.Instance.ChangeCourse();
                SingletonCustom<WaterSlider_CourseManager>.Instance.ChangeCourse();
            });
            isInit = true;
        }
    }
    private void GameEnd() {
        isInit = false;
    }
}
