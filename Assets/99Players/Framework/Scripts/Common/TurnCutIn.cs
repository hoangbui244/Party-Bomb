using System;
using UnityEngine;
public class TurnCutIn : MonoBehaviour {
    public GameObject mTurnCutInObj;
    private bool mTurnCutInFlg;
    private float mIntervalTurnCutIn;
    private float mTurnCutInAlpha;
    private const float INTERVAL_TIME = 1f;
    private const float ALPHA_SPEED = 5f;
    private float delayTime;
    public SpriteRenderer mTurnCutInRenderer;
    public Sprite[] mTurnSprites;
    public Sprite[] mTurnSprites_EN;
    [SerializeField]
    [Header("1人用時【あなたの番です】表示")]
    private Sprite mTurnYours;
    [SerializeField]
    private Sprite mTurnYours_EN;
    private Action callBack;
    private void Awake() {
        mTurnCutInRenderer.SetAlpha(0f);
        mTurnCutInObj.SetActive(value: false);
    }
    private void Update() {
        if (!mTurnCutInFlg) {
            return;
        }
        if (delayTime > 0f) {
            delayTime -= Time.deltaTime;
            return;
        }
        if (mIntervalTurnCutIn >= 1f) {
            mTurnCutInAlpha += Time.deltaTime * 5f;
        }
        if (mTurnCutInAlpha >= 1f) {
            mIntervalTurnCutIn -= Time.deltaTime * 1.5f;
        }
        if (mIntervalTurnCutIn <= 0f) {
            mTurnCutInAlpha -= Time.deltaTime * 5f;
        }
        if (mTurnCutInAlpha < 0f) {
            mTurnCutInFlg = false;
            mTurnCutInObj.SetActive(value: false);
            if (callBack != null) {
                callBack();
                callBack = null;
            }
        }
        Color color = mTurnCutInRenderer.color;
        color.a = mTurnCutInAlpha;
        mTurnCutInRenderer.color = color;
    }
    public void ShowTurnCutIn(int _playerNo, float _delay, Action _callback = null) {
        mTurnCutInFlg = true;
        UnityEngine.Debug.Log("trun Player No " + _playerNo.ToString());
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && _playerNo == 0) {
            if (Localize_Define.Language == Localize_Define.LanguageType.English) {
                mTurnCutInRenderer.sprite = mTurnYours_EN;
            } else {
                mTurnCutInRenderer.sprite = mTurnYours;
            }
        } else if (Localize_Define.Language == Localize_Define.LanguageType.English) {
            mTurnCutInRenderer.sprite = mTurnSprites_EN[_playerNo];
        } else {
            mTurnCutInRenderer.sprite = mTurnSprites[_playerNo];
        }
        mTurnCutInObj.SetActive(value: true);
        delayTime = _delay;
        mIntervalTurnCutIn = 1f;
        mTurnCutInAlpha = 0f;
        callBack = _callback;
    }
}
