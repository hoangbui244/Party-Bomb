using System;
using UnityEngine;
public class CommonEndSimple : SingletonCustom<CommonEndSimple> {
    private float EFFECT_TIME = 3f;
    [SerializeField]
    [Header("終了画像オブジェクト")]
    private GameObject objEnd;
    private float objEndDefPosX;
    [SerializeField]
    [Header("終了画像")]
    private SpriteRenderer spEnd;
    [SerializeField]
    [Header("画像差分")]
    private Sprite[] arrayDiff;
    [SerializeField]
    [Header("画像差分（EN）")]
    private Sprite[] arrayDiff_EN;
    private void Awake() {
        objEndDefPosX = objEnd.transform.localPosition.x;
        objEnd.transform.SetLocalPositionX(2000f);
    }
    public void Show(Action _callBack = null) {
        objEnd.SetActive(value: true);
        if (Localize_Define.Language == Localize_Define.LanguageType.Japanese) {
            GS_Define.GameType lastSelectGameType = SingletonCustom<GameSettingManager>.Instance.LastSelectGameType;
            if (lastSelectGameType == GS_Define.GameType.GET_BALL || lastSelectGameType == GS_Define.GameType.BOMB_ROULETTE) {
            }
            spEnd.sprite = arrayDiff[0];
        } else {
            GS_Define.GameType lastSelectGameType = SingletonCustom<GameSettingManager>.Instance.LastSelectGameType;
            if (lastSelectGameType == GS_Define.GameType.GET_BALL || lastSelectGameType == GS_Define.GameType.BOMB_ROULETTE || lastSelectGameType == GS_Define.GameType.ATTACK_BALL) {
                spEnd.sprite = arrayDiff_EN[0];
            } else {
                spEnd.sprite = arrayDiff_EN[0];
            }
        }
        float defLocal = objEnd.transform.localPosition.x;
        LeanTween.moveLocalX(objEnd.transform.gameObject, objEndDefPosX, 0.75f).setEaseOutBack();
        SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
        WaitAfterExec(2f, delegate {
            LeanTween.moveLocalX(objEnd.transform.gameObject, 0f - defLocal, 0.5f).setEaseOutQuad().setOnComplete((Action)delegate {
                if (_callBack != null) {
                    _callBack();
                }
                objEnd.SetActive(value: false);
            });
        });
    }
}
