using System;
using System.Collections;
using UnityEngine;
public class CommonStartProduction : SingletonCustom<CommonStartProduction> {
    [SerializeField]
    [Header("数字画像")]
    private GameObject[] numberSprite;
    [SerializeField]
    [Header("スタ\u30fcト画像")]
    private GameObject startSprite;
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
        Init();
    }
    private void Init() {
        for (int i = 0; i < numberSprite.Length; i++) {
            numberSprite[i].SetActive(value: false);
        }
        startSprite.SetActive(value: false);
        startSprite.transform.SetLocalScale(0f, 0f, 1f);
        if (Localize_Define.Language == Localize_Define.LanguageType.Japanese) {
            switch (SingletonCustom<GameSettingManager>.Instance.LastSelectGameType) {
                case GS_Define.GameType.CANNON_SHOT:
                case GS_Define.GameType.MOLE_HAMMER:
                case GS_Define.GameType.RECEIVE_PON:
                case GS_Define.GameType.ARCHER_BATTLE:
                    break;
                case GS_Define.GameType.GET_BALL:
                case GS_Define.GameType.BLOCK_WIPER:
                case GS_Define.GameType.BOMB_ROULETTE:
                case GS_Define.GameType.DELIVERY_ORDER:
                case GS_Define.GameType.ATTACK_BALL:
                    spStart.sprite = arrayDiff[0];
                    break;
            }
        } else {
            spStart.sprite = arrayDiff_EN[0];
        }
    }
    public void Play(Action _callBack, bool _isCallBackProcessIsStart = false) {
        StartCoroutine(_PlayAnimation(_callBack, _isCallBackProcessIsStart));
    }
    private IEnumerator _PlayAnimation(Action _callBack, bool _isCallBackProcessIsStart) {
        Init();
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < numberSprite.Length; i++) {
            numberSprite[i].SetActive(i == 2);
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_countdown");
        yield return new WaitForSeconds(1f);
        for (int j = 0; j < numberSprite.Length; j++) {
            numberSprite[j].SetActive(j == 1);
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_countdown");
        yield return new WaitForSeconds(1f);
        for (int k = 0; k < numberSprite.Length; k++) {
            numberSprite[k].SetActive(k == 0);
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_countdown");
        yield return new WaitForSeconds(1f);
        numberSprite[0].SetActive(value: false);
        startSprite.SetActive(value: true);
        LeanTween.scale(startSprite, Vector3.one, 0.4f).setEaseOutBack();
        SingletonCustom<AudioManager>.Instance.SePlay("se_start");
        if (_isCallBackProcessIsStart) {
            _callBack?.Invoke();
            yield return new WaitForSeconds(1.4f);
            startSprite.SetActive(value: false);
        } else {
            yield return new WaitForSeconds(1.4f);
            startSprite.SetActive(value: false);
            _callBack?.Invoke();
        }
    }
    public void PlayCountOnly(Action _callBack) {
        StartCoroutine(_PlayAnimationCountOnly(_callBack));
    }
    public void StopCountOnly() {
        for (int i = 0; i < numberSprite.Length; i++) {
            numberSprite[i].SetActive(value: false);
        }
        StopAllCoroutines();
    }
    private IEnumerator _PlayAnimationCountOnly(Action _callBack) {
        Init();
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < numberSprite.Length; i++) {
            numberSprite[i].SetActive(i == 2);
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_countdown");
        yield return new WaitForSeconds(1f);
        for (int j = 0; j < numberSprite.Length; j++) {
            numberSprite[j].SetActive(j == 1);
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_countdown");
        yield return new WaitForSeconds(1f);
        for (int k = 0; k < numberSprite.Length; k++) {
            numberSprite[k].SetActive(k == 0);
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_countdown");
        yield return new WaitForSeconds(1f);
        numberSprite[0].SetActive(value: false);
        yield return new WaitForSeconds(0.4f);
        _callBack?.Invoke();
    }
}
