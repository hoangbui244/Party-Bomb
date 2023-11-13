using UnityEngine;
public class Scene_Main : MonoBehaviour {
    public void CloseDetail() {
        SingletonCustom<GS_GameSelectManager>.Instance.DirectDetailClose();
    }
    public void UpdateData() {
        SingletonCustom<GS_GameSelectManager>.Instance.DetailUpdateData();
    }
    public void Repaint() {
        SingletonCustom<GS_GameSelectManager>.Instance.Repaint();
    }
    public void Init() {
        SingletonCustom<GS_GameSelectManager>.Instance.Init();
    }
    public bool IsPlayerSetting() {
        return SingletonCustom<GS_GameSelectManager>.Instance.IsPlayerSetting;
    }
    public void FadeInit() {
        SingletonCustom<GS_GameSelectManager>.Instance.FadeInit();
    }
    private void LayerEnd(SceneManager.LayerCloseType _closeType) {
    }
    private void OpenLayer() {
        SingletonCustom<SceneManager>.Instance.AddNowLayerCloseCallBack(LayerEnd);
    }
    private bool IsActive() {
        return SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack() == new SceneManager.LayerClose(LayerEnd);
    }
    private void Awake() {
        Init();
    }
    private void OnEnable() {
        OpenLayer();
        if (SingletonCustom<GS_GameSelectManager>.Instance.IsPartySelect) {
            if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying("bgm_party_select")) {
                SingletonCustom<AudioManager>.Instance.BgmStop();
                SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_party_select", _loop: true);
            }
        } else {
            SingletonCustom<AudioManager>.Instance.PlayTitleBgm(_isUpdate: false);
        }
    }
    private void Update() {
    }
}
