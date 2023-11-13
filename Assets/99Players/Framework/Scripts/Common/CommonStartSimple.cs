using System;
using UnityEngine;
public class CommonStartSimple : SingletonCustom<CommonStartSimple> {
    private float EFFECT_TIME = 3f;
    private float defStartObjXPos;
    [SerializeField]
    [Header("スタ\u30fcト画像")]
    private GameObject objStart;
    [SerializeField]
    [Header("スタ\u30fcト画像")]
    private SpriteRenderer spStart;
    [SerializeField]
    [Header("画像差分")]
    private Sprite[] arrayDiff;
    [SerializeField]
    [Header("画像差分（EN）")]
    private Sprite[] arrayDiff_EN;
    private void Start() {
        defStartObjXPos = objStart.transform.localPosition.x;
    }
    public void Show(Action _callBack = null) {
        objStart.SetActive(value: true);
        if (Localize_Define.Language == Localize_Define.LanguageType.Japanese) {
            GS_Define.GameType lastSelectGameType = SingletonCustom<GameSettingManager>.Instance.LastSelectGameType;
            spStart.sprite = arrayDiff[0];
        } else {
            GS_Define.GameType lastSelectGameType2 = SingletonCustom<GameSettingManager>.Instance.LastSelectGameType;
            spStart.sprite = arrayDiff_EN[0];
        }
        objStart.transform.SetLocalPositionX(defStartObjXPos);
        LeanTween.moveLocalX(objStart.transform.gameObject, 0f, 0.75f).setEaseOutBack();
        SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
        if (Localize_Define.Language == Localize_Define.LanguageType.Japanese) {
            GS_Define.GameType lastSelectGameType3 = SingletonCustom<GameSettingManager>.Instance.LastSelectGameType;
            SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_count_down_start", _loop: false, 0f, 1f, 1f, 0.5f);
        } else {
            SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_count_down_start", _loop: false, 0f, 1f, 1f, 0.5f);
        }
        WaitAfterExec(2f, delegate {
            LeanTween.moveLocalX(objStart.transform.gameObject, 0f - defStartObjXPos, 0.5f).setEaseOutQuad().setOnComplete((Action)delegate {
                if (_callBack != null) {
                    _callBack();
                }
                objStart.SetActive(value: false);
            });
        });
    }
}
