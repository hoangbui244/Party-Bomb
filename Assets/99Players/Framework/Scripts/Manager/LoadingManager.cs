using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LoadingManager : MonoBehaviour {
    public enum FADE_TYPE {
        NONE,
        IN,
        IN_INIT,
        OUT,
        OUT_INIT,
        OUT_IMMEDIATE,
        IN_IMMEDIATE,
        LOADING_IN
    }
    private readonly Color white = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
    private readonly Color red = new Color32(232, 30, 28, byte.MaxValue);
    public float FADE_TIME;
    public float LOADING_TIME;
    private FADE_TYPE mFadeType;
    public RawImage[] mFade;
    public GameObject rootLoading;
    private Color mCalcColor;
    private bool mLoadingFlg;
    private bool mFadeFlg;
    private float mIntervalTime;
    private float mLoadingTime;
    private GameObject[] arrayFontObj;
    [SerializeField]
    [Header("読込中文字オブジェクト（JP）")]
    private GameObject[] arrayFontObj_JP;
    [SerializeField]
    [Header("読込中文字オブジェクト（EN）")]
    private GameObject[] arrayFontObj_EN;
    [SerializeField]
    [Header("ドット画像")]
    private GameObject[] arrayFontDot;
    [SerializeField]
    [Header("フェ\u30fcド画像")]
    private Texture[] arrayFadeTexture;
    [SerializeField]
    [Header("メモ表示のル\u30fcト")]
    private Image objMemoRoot;
    private float originMemoRootSizeX;
    [SerializeField]
    [Header("Tips")]
    private Image tipsObj;
    private float originTipsPosX;
    [SerializeField]
    [Header("メモテキスト")]
    private TextMeshProUGUI memoText;
    [SerializeField]
    [Header("ロ\u30fcドキャラ")]
    private Image[] arrayImageCharacter;
    [SerializeField]
    [Header("ロ\u30fcドキャラテクスチャ：A")]
    private Sprite[] arrayTexCharacter;
    [SerializeField]
    [Header("ロ\u30fcドキャラテクスチャ：B")]
    private Sprite[] arrayTexCharacter2;
    [SerializeField]
    [Header("ロ\u30fcドキャラテクスチャ：C")]
    private Sprite[] arrayTexCharacter3;
    [SerializeField]
    [Header("ロ\u30fcドキャラテクスチャ：D")]
    private Sprite[] arrayTexCharacter4;
    private List<int> listLoadCharacter = new List<int>();
    private List<int> listLoadPose = new List<int>();
    private AsyncOperation asyncLoadData;
    private bool isCompletedAsyncLoadData;
    private readonly int TIPS_EXCEL_IDX = 200;
    private readonly int TIPS_MAX = 26;
    private int[] arrayMemoIdx = new int[3];
    private int currentMemoIdx;
    private bool isMemoCheck;
    private List<int> tips = new List<int>();
    private float[] fontScale;
    private float fontDotTime;
    private float DEF_TIPS_FONT_SIZE;
    private readonly int SMALL_TIPS_FONT_LENGTH = 80;
    private readonly float SMALL_TIPS_FONT_SIZE = 35f;
    private int POSE_MAX = 4;
    private bool isHideLoadingText;
    public void HideLoadingText() {
        isHideLoadingText = true;
    }
    private void LayerEnd(SceneManager.LayerCloseType _closeType) {
    }
    private void OpenLayer() {
        SingletonCustom<SceneManager>.Instance.AddNowLayerCloseCallBack(LayerEnd);
    }
    private bool IsActive() {
        return SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack() == new SceneManager.LayerClose(LayerEnd);
    }
    public void Awake() {
        originMemoRootSizeX = objMemoRoot.rectTransform.sizeDelta.x;
        originTipsPosX = tipsObj.rectTransform.localPosition.x;
        mFadeType = FADE_TYPE.NONE;
        for (int i = 0; i < mFade.Length; i++) {
            mFade[i].color = white;
        }
        SetRandomFadeTexture();
        mCalcColor = mFade[0].color;
        mLoadingFlg = false;
        mFadeFlg = false;
        mIntervalTime = 0f;
        mLoadingTime = 0f;
        DEF_TIPS_FONT_SIZE = memoText.fontSize;
        for (int j = 0; j < arrayMemoIdx.Length; j++) {
            arrayMemoIdx[j] = -1;
        }
        isMemoCheck = true;
        tips.Clear();
        tips.Add(0);
        tips.Add(1);
        tips.Add(2);
        while (isMemoCheck) {
            currentMemoIdx = tips[Random.Range(0, tips.Count)];
            isMemoCheck = false;
            for (int k = 0; k < arrayMemoIdx.Length; k++) {
                if (arrayMemoIdx[k] == currentMemoIdx) {
                    isMemoCheck = true;
                }
            }
        }
        for (int l = 1; l < arrayMemoIdx.Length; l++) {
            arrayMemoIdx[l] = arrayMemoIdx[l - 1];
        }
        arrayMemoIdx[0] = currentMemoIdx;
        for (int m = 0; m < arrayFontObj_JP.Length; m++) {
            arrayFontObj_JP[m].SetActive(value: false);
        }
        for (int n = 0; n < arrayFontObj_EN.Length; n++) {
            arrayFontObj_EN[n].SetActive(value: false);
        }
        arrayFontObj = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? arrayFontObj_JP : arrayFontObj_EN);
        for (int num = 0; num < arrayFontObj.Length; num++) {
            arrayFontObj[num].SetActive(value: true);
        }
        fontScale = new float[arrayFontObj.Length];
        rootLoading.SetActive(value: false);
        objMemoRoot.gameObject.SetActive(value: false);
        memoText.gameObject.SetActive(value: false);
        memoText.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, TIPS_EXCEL_IDX + currentMemoIdx));
        if (Localize_Define.Language == Localize_Define.LanguageType.English) {
            if (memoText.text.Length > SMALL_TIPS_FONT_LENGTH) {
                memoText.fontSize = SMALL_TIPS_FONT_SIZE;
            } else {
                memoText.fontSize = DEF_TIPS_FONT_SIZE;
            }
            memoText.rectTransform.sizeDelta = new Vector2(memoText.preferredWidth, memoText.rectTransform.sizeDelta.y);
            float num2 = Mathf.Clamp(memoText.preferredWidth + 150f, originMemoRootSizeX, memoText.preferredWidth + 150f);
            objMemoRoot.rectTransform.sizeDelta = new Vector2(num2, objMemoRoot.rectTransform.sizeDelta.y);
            tipsObj.rectTransform.SetLocalPositionX(originTipsPosX - (num2 - originMemoRootSizeX) * 0.5f);
        }
        listLoadCharacter.Clear();
        for (int num3 = 0; num3 < GS_Define.PLAYER_MAX; num3++) {
            listLoadCharacter.Add(num3);
        }
        listLoadCharacter.Shuffle();
        listLoadPose.Clear();
        for (int num4 = 0; num4 < POSE_MAX; num4++) {
            listLoadPose.Add(num4);
        }
        listLoadPose.Shuffle();
        for (int num5 = 0; num5 < arrayImageCharacter.Length; num5++) {
            switch (listLoadPose[num5]) {
                case 0:
                    arrayImageCharacter[num5].sprite = arrayTexCharacter[listLoadCharacter[num5]];
                    break;
                case 1:
                    arrayImageCharacter[num5].sprite = arrayTexCharacter2[listLoadCharacter[num5]];
                    break;
                case 2:
                    arrayImageCharacter[num5].sprite = arrayTexCharacter3[listLoadCharacter[num5]];
                    break;
                case 3:
                    arrayImageCharacter[num5].sprite = arrayTexCharacter4[listLoadCharacter[num5]];
                    break;
            }
        }
    }
    private void SetRandomFadeTexture() {
        mFade[0].texture = arrayFadeTexture[SingletonCustom<GameSettingManager>.Instance.BackFrameTexIdx];
    }
    public void Update() {
        if (mFade[0].gameObject.activeSelf) {
            mFade[0].uvRect = new Rect((mFade[0].uvRect.x - Time.unscaledDeltaTime * 0.01f) % 1f, (mFade[0].uvRect.y + Time.unscaledDeltaTime * 0.01f) % 1f, mFade[0].uvRect.width, mFade[0].uvRect.height);
        }
        if (rootLoading.activeSelf) {
            fontDotTime = (fontDotTime + Time.unscaledDeltaTime) % 4f;
            for (int i = 0; i < arrayFontDot.Length; i++) {
                bool flag = i < (int)fontDotTime;
                if (arrayFontDot[i].activeSelf != flag) {
                    arrayFontDot[i].SetActive(flag);
                }
            }
        }
        if (mIntervalTime > 0f) {
            mIntervalTime -= Time.unscaledDeltaTime;
        } else if (!SingletonCustom<SceneManager>.Instance.IsLoading) {
            switch (mFadeType) {
                case FADE_TYPE.NONE:
                    break;
                case FADE_TYPE.IN_INIT:
                    StateFadeInInit();
                    break;
                case FADE_TYPE.IN:
                    StateFadeIn();
                    break;
                case FADE_TYPE.OUT_INIT:
                    StateFadeOutInit();
                    break;
                case FADE_TYPE.OUT:
                    StateFadeOut();
                    break;
                case FADE_TYPE.IN_IMMEDIATE:
                    StateFadeInInit(_immediate: true);
                    break;
                case FADE_TYPE.OUT_IMMEDIATE:
                    StateFadeOutInit(_immediate: true);
                    break;
                case FADE_TYPE.LOADING_IN:
                    StateLoadingIn();
                    break;
            }
        }
    }
    public void STATE_FADE(FADE_TYPE _type, bool _loadingflg = false, float _loadingTime = 0f, float _loadingDelay = 0f) {
        mFadeType = _type;
        switch (mFadeType) {
            case FADE_TYPE.IN:
                mFadeType = FADE_TYPE.IN_INIT;
                break;
            case FADE_TYPE.OUT:
                mFadeType = FADE_TYPE.OUT_INIT;
                break;
        }
        mFadeFlg = true;
        mLoadingFlg = _loadingflg;
        LOADING_TIME = _loadingTime;
        mIntervalTime = _loadingDelay;
        UnityEngine.Debug.Log("Next:" + SingletonCustom<SceneManager>.Instance.GetNextScene().ToString());
        UnityEngine.Debug.Log("Now:" + SingletonCustom<SceneManager>.Instance.GetNowScene().ToString());
        if (SingletonCustom<SceneManager>.Instance.GetNextScene() == SceneManager.SceneType.TITLE || SingletonCustom<SceneManager>.Instance.GetNowScene() == SceneManager.SceneType.TITLE) {
            mFade[1].gameObject.SetActive(value: false);
        }
        OpenLayer();
    }
    private void StateFadeInInit(bool _immediate = false) {
        if (mLoadingFlg) {
            StartCoroutine(SingletonCustom<SceneManager>.Instance.LoadScene());
            mFadeType = FADE_TYPE.LOADING_IN;
            return;
        }
        rootLoading.SetActive(value: false);
        if (_immediate) {
            mCalcColor.a = 0f;
            mLoadingTime = FADE_TIME;
        } else {
            mCalcColor.a = 1f;
            mLoadingTime = 0f;
        }
        for (int i = 0; i < mFade.Length; i++) {
            mFade[i].color = mCalcColor;
            mFade[i].gameObject.SetActive(value: true);
        }
        UnityEngine.Debug.Log("Next2:" + SingletonCustom<SceneManager>.Instance.GetNextScene().ToString());
        UnityEngine.Debug.Log("Now2:" + SingletonCustom<SceneManager>.Instance.GetNowScene().ToString());
        if (SingletonCustom<SceneManager>.Instance.GetNextScene() == SceneManager.SceneType.TITLE || SingletonCustom<SceneManager>.Instance.GetNowScene() == SceneManager.SceneType.TITLE) {
            mFade[1].gameObject.SetActive(value: false);
        }
        mFadeType = FADE_TYPE.IN;
        mIntervalTime = 0.1f;
    }
    private void StateLoadingIn() {
        StateBaseClass.SetPauseFlg(StateBaseClass.PAUSE_LEVEL.NONE);
        rootLoading.SetActive(value: false);
        mCalcColor.a = 1f;
        mLoadingTime = 0f;
        for (int i = 0; i < mFade.Length; i++) {
            mFade[i].color = mCalcColor;
            mFade[i].gameObject.SetActive(value: true);
        }
        if (SingletonCustom<SceneManager>.Instance.GetNextScene() == SceneManager.SceneType.TITLE || SingletonCustom<SceneManager>.Instance.GetPrevScene() == SceneManager.SceneType.TITLE) {
            mFade[1].gameObject.SetActive(value: false);
        }
        mFadeType = FADE_TYPE.IN;
        mIntervalTime = 0.1f;
    }
    private void StateFadeIn() {
        if (mLoadingTime <= FADE_TIME) {
            mLoadingTime += Time.unscaledDeltaTime;
            mCalcColor.a = 1f - mLoadingTime / FADE_TIME;
        } else {
            mCalcColor.a = 0f;
            STATE_PAUSE();
            for (int i = 0; i < mFade.Length; i++) {
                mFade[i].gameObject.SetActive(value: false);
            }
            if (SingletonCustom<SceneManager>.Instance.GetNowLayerCloseCallBack() == new SceneManager.LayerClose(LayerEnd)) {
                SingletonCustom<SceneManager>.Instance.CloseComplete();
            }
            SingletonCustom<SceneManager>.Instance.FadeClose();
        }
        for (int j = 0; j < mFade.Length; j++) {
            mFade[j].color = mCalcColor;
        }
    }
    private void StateFadeOutInit(bool _immediate = false) {
        if (_immediate) {
            mCalcColor.a = 1f;
            mLoadingTime = FADE_TIME;
        } else {
            mCalcColor.a = 0f;
            mLoadingTime = 0f;
        }
        for (int i = 0; i < mFade.Length; i++) {
            mFade[i].color = mCalcColor;
        }
        SetRandomFadeTexture();
        for (int j = 0; j < mFade.Length; j++) {
            mFade[j].gameObject.SetActive(value: true);
        }
        if (SingletonCustom<SceneManager>.Instance.GetNextScene() == SceneManager.SceneType.TITLE || SingletonCustom<SceneManager>.Instance.GetNowScene() == SceneManager.SceneType.TITLE) {
            mFade[1].gameObject.SetActive(value: false);
        }
        mFadeType = FADE_TYPE.OUT;
    }
    private void StateFadeOut() {
        if (mLoadingTime <= FADE_TIME) {
            mLoadingTime += Time.unscaledDeltaTime;
            mCalcColor.a = mLoadingTime / FADE_TIME;
        } else {
            mCalcColor.a = 1f;
            if (mLoadingFlg) {
                if (LOADING_TIME > 0f) {
                    mIntervalTime = LOADING_TIME;
                    rootLoading.SetActive(!isHideLoadingText);
                    isHideLoadingText = false;
                }
                for (int i = 0; i < arrayFontObj.Length; i++) {
                    arrayFontObj[i].transform.SetLocalScaleX(1f);
                }
                for (int j = 0; j < fontScale.Length; j++) {
                    fontScale[j] = 1f;
                }
                listLoadCharacter.Clear();
                for (int k = 0; k < GS_Define.PLAYER_MAX; k++) {
                    listLoadCharacter.Add(k);
                }
                listLoadCharacter.Shuffle();
                listLoadPose.Clear();
                for (int l = 0; l < POSE_MAX; l++) {
                    listLoadPose.Add(l);
                }
                listLoadPose.Shuffle();
                for (int m = 0; m < arrayImageCharacter.Length; m++) {
                    switch (listLoadPose[m]) {
                        case 0:
                            arrayImageCharacter[m].sprite = arrayTexCharacter[listLoadCharacter[m]];
                            break;
                        case 1:
                            arrayImageCharacter[m].sprite = arrayTexCharacter2[listLoadCharacter[m]];
                            break;
                        case 2:
                            arrayImageCharacter[m].sprite = arrayTexCharacter3[listLoadCharacter[m]];
                            break;
                        case 3:
                            arrayImageCharacter[m].sprite = arrayTexCharacter4[listLoadCharacter[m]];
                            break;
                    }
                }
                fontDotTime = -1f;
                switch (SingletonCustom<SceneManager>.Instance.GetNextScene()) {
                    default:
                        objMemoRoot.gameObject.SetActive(value: true);
                        memoText.gameObject.SetActive(value: true);
                        rootLoading.transform.SetLocalPositionY(0f);
                        break;
                    case SceneManager.SceneType.TITLE:
                        if (SingletonCustom<SceneManager>.Instance.GetPrevScene() == SceneManager.SceneType.TITLE) {
                            rootLoading.SetActive(value: false);
                            objMemoRoot.gameObject.SetActive(value: false);
                            memoText.gameObject.SetActive(value: false);
                        } else {
                            rootLoading.SetActive(value: true);
                            objMemoRoot.gameObject.SetActive(value: false);
                            memoText.gameObject.SetActive(value: false);
                            rootLoading.transform.SetLocalPositionY(-30f);
                        }
                        break;
                    case SceneManager.SceneType.MAIN:
                        if (SingletonCustom<SceneManager>.Instance.GetPrevScene() == SceneManager.SceneType.TITLE) {
                            objMemoRoot.gameObject.SetActive(value: false);
                            memoText.gameObject.SetActive(value: false);
                        } else {
                            objMemoRoot.gameObject.SetActive(value: true);
                            memoText.gameObject.SetActive(value: true);
                            rootLoading.transform.SetLocalPositionY(0f);
                        }
                        break;
                    case SceneManager.SceneType.PLAY_KING_OP:
                        objMemoRoot.gameObject.SetActive(value: false);
                        memoText.gameObject.SetActive(value: false);
                        break;
                }
                if (!isHideLoadingText) {
                    isMemoCheck = true;
                    tips.Clear();
                    tips.Add(0);
                    tips.Add(1);
                    tips.Add(2);
                    if (SingletonCustom<AocAssetBundleManager>.Instance.GetDlcCount() >= 2) {
                        tips.Add(80);
                    }
                    if (Localize_Define.Language == Localize_Define.LanguageType.Japanese) {
                        tips.Add(550 - TIPS_EXCEL_IDX);
                        tips.Add(551 - TIPS_EXCEL_IDX);
                        tips.Add(552 - TIPS_EXCEL_IDX);
                        tips.Add(553 - TIPS_EXCEL_IDX);
                        tips.Add(554 - TIPS_EXCEL_IDX);
                        tips.Add(555 - TIPS_EXCEL_IDX);
                        tips.Add(556 - TIPS_EXCEL_IDX);
                    }
                    switch (SingletonCustom<SceneManager>.Instance.GetNextScene()) {
                        case SceneManager.SceneType.GET_BALL:
                            tips.Add(3);
                            tips.Add(4);
                            break;
                        case SceneManager.SceneType.CANNON_SHOT:
                            tips.Add(5);
                            tips.Add(6);
                            break;
                        case SceneManager.SceneType.BLOCK_WIPER:
                            tips.Add(7);
                            tips.Add(8);
                            break;
                        case SceneManager.SceneType.MOLE_HAMMER:
                            tips.Add(31);
                            tips.Add(32);
                            break;
                        case SceneManager.SceneType.BOMB_ROULETTE:
                            tips.Add(9);
                            tips.Add(10);
                            break;
                        case SceneManager.SceneType.RECEIVE_PON:
                            tips.Add(57);
                            tips.Add(58);
                            break;
                        case SceneManager.SceneType.ARCHER_BATTLE:
                            tips.Add(11);
                            tips.Add(12);
                            break;
                        case SceneManager.SceneType.ATTACK_BALL:
                            tips.Add(13);
                            tips.Add(14);
                            break;
                        case SceneManager.SceneType.BLOW_AWAY_TANK:
                            tips.Add(15);
                            tips.Add(16);
                            break;
                        case SceneManager.SceneType.SCROLL_JUMP:
                            tips.Add(17);
                            tips.Add(18);
                            break;
                        case SceneManager.SceneType.CLIMB_BLOCK:
                            tips.Add(19);
                            tips.Add(20);
                            break;
                        case SceneManager.SceneType.MAKE_SAME_DOT:
                            tips.Add(21);
                            tips.Add(22);
                            break;
                        case SceneManager.SceneType.TIMING_STAMPING:
                            tips.Add(23);
                            tips.Add(24);
                            break;
                        case SceneManager.SceneType.WATER_PISTOL_BATTLE:
                            tips.Add(43);
                            tips.Add(44);
                            break;
                        case SceneManager.SceneType.KURUKURU_CUBE:
                            tips.Add(45);
                            tips.Add(46);
                            break;
                        case SceneManager.SceneType.THREE_LEGGED:
                            tips.Add(47);
                            tips.Add(48);
                            break;
                        case SceneManager.SceneType.REVERSI:
                            tips.Add(49);
                            tips.Add(50);
                            break;
                        case SceneManager.SceneType.BUNBUN_JUMP:
                            tips.Add(51);
                            tips.Add(52);
                            break;
                        case SceneManager.SceneType.PUSH_LANE:
                            tips.Add(53);
                            tips.Add(54);
                            break;
                        case SceneManager.SceneType.JAMMING_DIVING:
                            tips.Add(55);
                            tips.Add(56);
                            break;
                        case SceneManager.SceneType.AWAY_HOIHOI:
                            tips.Add(25);
                            tips.Add(26);
                            break;
                        case SceneManager.SceneType.THROWING_BALLS:
                            tips.Add(33);
                            tips.Add(34);
                            break;
                        case SceneManager.SceneType.ANSWERS_RUN:
                            tips.Add(35);
                            tips.Add(36);
                            break;
                        case SceneManager.SceneType.ANSWER_DROP:
                            tips.Add(37);
                            tips.Add(38);
                            break;
                        case SceneManager.SceneType.BLOCK_CHANGER:
                            tips.Add(39);
                            tips.Add(40);
                            break;
                        case SceneManager.SceneType.PERFECT_DICE_ROLL:
                            tips.Add(41);
                            tips.Add(42);
                            break;
                        case SceneManager.SceneType.MARUTA_JUMP:
                            tips.Add(27);
                            tips.Add(28);
                            break;
                        case SceneManager.SceneType.TIME_STOP:
                            tips.Add(29);
                            tips.Add(30);
                            break;
                        case SceneManager.SceneType.WOBBLY_DISK:
                            tips.Add(59);
                            tips.Add(60);
                            break;
                        case SceneManager.SceneType.ROBOT_WATCH:
                            tips.Add(61);
                            tips.Add(62);
                            break;
                        case SceneManager.SceneType.MINISCAPE_RACE:
                            tips.Add(63);
                            tips.Add(64);
                            break;
                        case SceneManager.SceneType.BELT_CONVEYOR_RUN:
                            tips.Add(65);
                            tips.Add(66);
                            break;
                        case SceneManager.SceneType.PUSH_IN_BOXING:
                            tips.Add(67);
                            tips.Add(68);
                            break;
                        case SceneManager.SceneType.BOMB_LIFTING:
                            tips.Add(69);
                            tips.Add(70);
                            break;
                        case SceneManager.SceneType.BATTLE_AIR_HOCKEY:
                            tips.Add(71);
                            tips.Add(72);
                            break;
                        case SceneManager.SceneType.BLOCK_SLICER:
                            tips.Add(73);
                            tips.Add(74);
                            break;
                        case SceneManager.SceneType.ESCAPE_ZONE:
                            tips.Add(75);
                            tips.Add(76);
                            break;
                        case SceneManager.SceneType.LABYRINTH:
                            tips.Add(77);
                            tips.Add(78);
                            break;
                        case SceneManager.SceneType.DROP_BLOCK:
                            tips.Add(412 - TIPS_EXCEL_IDX);
                            tips.Add(413 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.GIRIGIRI_WATER:
                            tips.Add(414 - TIPS_EXCEL_IDX);
                            tips.Add(415 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.DROP_PANEL:
                            tips.Add(416 - TIPS_EXCEL_IDX);
                            tips.Add(417 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.GIRIGIRI_STOP:
                            tips.Add(418 - TIPS_EXCEL_IDX);
                            tips.Add(419 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.ROCK_PAPER_SCISSORS:
                            tips.Add(420 - TIPS_EXCEL_IDX);
                            tips.Add(421 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.NONSTOP_PICTURE:
                            tips.Add(422 - TIPS_EXCEL_IDX);
                            tips.Add(423 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.ROPE_JUMPING:
                            tips.Add(424 - TIPS_EXCEL_IDX);
                            tips.Add(425 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.TRAIN_GUIDE:
                            tips.Add(426 - TIPS_EXCEL_IDX);
                            tips.Add(427 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.BREAK_BLOCK:
                            tips.Add(428 - TIPS_EXCEL_IDX);
                            tips.Add(429 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.GUARD_ZONE:
                            tips.Add(430 - TIPS_EXCEL_IDX);
                            tips.Add(431 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.HOME_RUN_DERBY:
                            tips.Add(432 - TIPS_EXCEL_IDX);
                            tips.Add(433 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.FLYING_HAMMER:
                            tips.Add(434 - TIPS_EXCEL_IDX);
                            tips.Add(435 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.COLORFUL_SHOOT:
                            tips.Add(436 - TIPS_EXCEL_IDX);
                            tips.Add(437 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.CLIMB_WALL:
                            tips.Add(438 - TIPS_EXCEL_IDX);
                            tips.Add(439 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.TREASURE_CATCHER:
                            tips.Add(440 - TIPS_EXCEL_IDX);
                            tips.Add(441 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.COIN_DROP:
                            tips.Add(442 - TIPS_EXCEL_IDX);
                            tips.Add(443 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.HUNDRED_CHALLENGE:
                            tips.Add(444 - TIPS_EXCEL_IDX);
                            tips.Add(445 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.JANJAN_FISHING:
                            tips.Add(446 - TIPS_EXCEL_IDX);
                            tips.Add(447 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.EVERYONE_KEEPER:
                            tips.Add(448 - TIPS_EXCEL_IDX);
                            tips.Add(449 - TIPS_EXCEL_IDX);
                            break;
                        case SceneManager.SceneType.AIR_BALLOON:
                            tips.Add(450 - TIPS_EXCEL_IDX);
                            tips.Add(451 - TIPS_EXCEL_IDX);
                            break;
                    }
                    while (isMemoCheck) {
                        currentMemoIdx = tips[Random.Range(0, tips.Count)];
                        isMemoCheck = false;
                        for (int n = 0; n < arrayMemoIdx.Length; n++) {
                            if (arrayMemoIdx[n] == currentMemoIdx) {
                                isMemoCheck = true;
                            }
                        }
                    }
                    for (int num = 1; num < arrayMemoIdx.Length; num++) {
                        arrayMemoIdx[num] = arrayMemoIdx[num - 1];
                    }
                    arrayMemoIdx[0] = currentMemoIdx;
                    memoText.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, TIPS_EXCEL_IDX + currentMemoIdx));
                    UnityEngine.Debug.Log("currentMemoIdx:" + currentMemoIdx.ToString());
                    if (Localize_Define.Language == Localize_Define.LanguageType.English) {
                        if (memoText.text.Length > SMALL_TIPS_FONT_LENGTH) {
                            memoText.fontSize = SMALL_TIPS_FONT_SIZE;
                        } else {
                            memoText.fontSize = DEF_TIPS_FONT_SIZE;
                        }
                        memoText.rectTransform.sizeDelta = new Vector2(memoText.preferredWidth, memoText.rectTransform.sizeDelta.y);
                        float num2 = Mathf.Clamp(memoText.preferredWidth + 150f, originMemoRootSizeX, memoText.preferredWidth + 150f);
                        objMemoRoot.rectTransform.sizeDelta = new Vector2(num2, objMemoRoot.rectTransform.sizeDelta.y);
                        tipsObj.rectTransform.SetLocalPositionX(originTipsPosX - (num2 - originMemoRootSizeX) * 0.5f);
                    }
                }
                SingletonCustom<SceneManager>.Instance.DestroyScene();
            }
            mFadeType = FADE_TYPE.IN_INIT;
        }
        for (int num3 = 0; num3 < mFade.Length; num3++) {
            mFade[num3].color = mCalcColor;
        }
    }
    public void STATE_PAUSE() {
        mFadeType = FADE_TYPE.NONE;
        mFadeFlg = false;
    }
    public bool GetFadeFlg() {
        return mFadeFlg;
    }
    public void SetFadeRect(Rect _rect) {
        mFade[0].uvRect = _rect;
    }
    public Rect GetFadeRect() {
        return mFade[0].uvRect;
    }
    private void OnDestroy() {
        for (int i = 0; i < arrayFontObj.Length; i++) {
            LeanTween.cancel(arrayFontObj[i]);
        }
    }
}
