using GamepadInput;
using UnityEngine;
public class Scene_PlayKingOpening : SingletonCustom<Scene_PlayKingOpening> {
    [SerializeField]
    [Header("白フェ\u30fcド")]
    private SpriteRenderer whiteFade;
    [SerializeField]
    [Header("アニメ\u30fcション")]
    private Animation anim;
    [SerializeField]
    [Header("OPフェ\u30fcド")]
    private PlayKingOpening op;
    private bool isSecondScene;
    public override void Resume() {
        base.Resume();
        if (!SingletonCustom<AudioManager>.Instance.IsBgmPlaying("bgm_matsuriking_op")) {
            SingletonCustom<AudioManager>.Instance.BgmStop();
            SingletonCustom<AudioManager>.Instance.BgmPlay("bgm_matsuriking_op");
        }
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_fantasy_king", _loop: false, 0f, 1f, 1f, 9.5f);
        SingletonCustom<AudioManager>.Instance.VoicePlay("voice_fantasy_king2", _loop: false, 0f, 1f, 1f, 9.5f);
        SingletonCustom<AudioManager>.Instance.SePlay("SE_seien", _loop: false, 0f, 1f, 1f, 6f);
        for (int i = 0; i < 4; i++) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_result_slide", _loop: false, 0f, 1f, 1f, 0.1f + (float)i * 1f);
        }
        whiteFade.color = new Color(1f, 1f, 1f, 0f);
    }
    private void Update() {
        if (SingletonCustom<SceneManager>.Instance.GetFadeFlg()) {
            return;
        }
        if (anim["PlayKingOpening"].time >= 8.3f) {
            isSecondScene = true;
        }
        if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.A)) {
            if (isSecondScene) {
                SingletonCustom<GameSettingManager>.Instance.InitSportsDay();
                SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType = GameSettingManager.GameProgressType.ALL_SPORTS;
                SingletonCustom<SceneManager>.Instance.NextScene(SingletonCustom<GameSettingManager>.Instance.NextTable());
                SingletonCustom<AudioManager>.Instance.SeStopCoroutine();
                return;
            }
            anim["PlayKingOpening"].time = 8.3f;
            op.Stop();
            whiteFade.color = new Color(1f, 1f, 1f, 0f);
            SingletonCustom<AudioManager>.Instance.StopAllCoroutines();
            SingletonCustom<AudioManager>.Instance.SeStop("SE_seien");
            SingletonCustom<AudioManager>.Instance.SeStop("se_result_slide");
            SingletonCustom<AudioManager>.Instance.VoicePlay("voice_fantasy_king", _loop: false, 0f, 1f, 1f, 1f);
            SingletonCustom<AudioManager>.Instance.VoicePlay("voice_fantasy_king2", _loop: false, 0f, 1f, 1f, 1f);
            SingletonCustom<AudioManager>.Instance.SePlay("SE_seien");
            isSecondScene = true;
        }
    }
    public void OnEndAnimation() {
        SingletonCustom<GameSettingManager>.Instance.InitSportsDay();
        SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType = GameSettingManager.GameProgressType.ALL_SPORTS;
        SingletonCustom<SceneManager>.Instance.NextScene(SingletonCustom<GameSettingManager>.Instance.NextTable());
        SingletonCustom<AudioManager>.Instance.SeStopCoroutine();
    }
}
