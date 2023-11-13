using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : SingletonCustom<AudioManager> {
    public enum AUDIO_TYPE {
        SE,
        BGM,
        VOICE,
        NONE,
        MAX
    }
    /// <summary>
    /// 
    /// </summary>
    private struct tagPlayList {
        /// <summary>
        /// 
        /// </summary>
        public AUDIO_TYPE mType;
        /// <summary>
        /// 
        /// </summary>
        public string mSeIndex;
        /// <summary>
        /// 
        /// </summary>
        public string mBgmIndex;
        /// <summary>
        /// 
        /// </summary>
        public string mVoiceIndex;
        /// <summary>
        /// 
        /// </summary>
        public bool mActive;
        /// <summary>
        /// 
        /// </summary>
        public float mWaitTime;
        /// <summary>
        /// 
        /// </summary>
        public bool mLoopFlg;
        /// <summary>
        /// 
        /// </summary>
        public float mSetTime;
        /// <summary>
        /// 
        /// </summary>
        public float mVolume;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_type"></param>
        public tagPlayList(AUDIO_TYPE _type) {
            mType = AUDIO_TYPE.NONE;
            mSeIndex = "";
            mBgmIndex = "";
            mVoiceIndex = "";
            mActive = true;
            mWaitTime = 0f;
            mLoopFlg = false;
            mSetTime = 0f;
            mVolume = 0f;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_index"></param>
        /// <param name="_active"></param>
        /// <param name="_waitTime"></param>
        /// <param name="_loop"></param>
        /// <param name="_setTime"></param>
        /// <param name="_volume"></param>
        public void tagSePlayList(string _index, bool _active = true, float _waitTime = 0f, bool _loop = false, float _setTime = 0f, float _volume = 1f) {
            mType = AUDIO_TYPE.SE;
            mSeIndex = _index;
            mActive = _active;
            mWaitTime = _waitTime;
            mSetTime = _setTime;
            mVolume = _volume;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_index"></param>
        /// <param name="_active"></param>
        /// <param name="_waitTime"></param>
        /// <param name="_loop"></param>
        /// <param name="_setTime"></param>
        /// <param name="_volume"></param>
        public void tagBgmPlayList(string _index, bool _active = true, float _waitTime = 0f, bool _loop = false, float _setTime = 0f, float _volume = 1f) {
            mType = AUDIO_TYPE.BGM;
            mBgmIndex = _index;
            mActive = _active;
            mWaitTime = _waitTime;
            mSetTime = _setTime;
            mVolume = _volume;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_index"></param>
        /// <param name="_active"></param>
        /// <param name="_waitTime"></param>
        /// <param name="_loop"></param>
        /// <param name="_setTime"></param>
        /// <param name="_volume"></param>
        public void tagVoicePlayList(string _index, bool _active = true, float _waitTime = 0f, bool _loop = false, float _setTime = 0f, float _volume = 1f) {
            mType = AUDIO_TYPE.BGM;
            mVoiceIndex = _index;
            mActive = _active;
            mWaitTime = _waitTime;
            mSetTime = _setTime;
            mVolume = _volume;
        }
    }
    private int mBgmFlg;
    private int mVoiceFlg;
    private int mSeFlg;
    private int AUDIO_LIST_NUM = 100;
    private List<string> listInGameBgmName = new List<string>();
    private AudioSource[] mBgmObjs;
    private AudioSource[] mVoiceObjs;
    private AudioSource[] mSeObjs;
    private AudioClip[] mBgmResources;
    private AudioClip[] mVoiceResources;
    private AudioClip[] mSeResources;
    private tagPlayList[] mTagPlayLists;
    private int lastBgmNo;
    private int selectBgmNo;
    private int lastVoiceNo;
    private int selectVoiceNo;
    private int gameType1Idx;
    private int gameType2Idx;
    private int gameType3Idx;
    private int TITLE_ROUGH_NUM = 5;
    private int prevTitleBgmIdx;
    private int titleBgmIdx;
    private int pauseTargetIdx = -1;
    public bool IsLoadResource {
        get;
        set;
    }
    public string LastGameBgmIndex {
        get;
        set;
    }
    public string LastBgmIndex {
        get;
        set;
    }
    public void Awake() {
        gameType1Idx = UnityEngine.Random.Range(0, 2);
        gameType2Idx = UnityEngine.Random.Range(0, 2);
        gameType3Idx = UnityEngine.Random.Range(0, 2);
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        mSeFlg = (mBgmFlg = (mVoiceFlg = 1));
        IsLoadResource = false;
        mTagPlayLists = new tagPlayList[AUDIO_LIST_NUM];
        for (int i = 0; i < AUDIO_LIST_NUM; i++) {
            mTagPlayLists[i] = new tagPlayList(AUDIO_TYPE.NONE);
        }
        object[] array = Resources.LoadAll("BGM");
        object[] array2 = array;
        array = Resources.LoadAll("SE");
        object[] array3 = array;
        array = Resources.LoadAll("VOICE");
        object[] array4 = array;
        mBgmObjs = new AudioSource[array2.Length];
        mVoiceObjs = new AudioSource[array4.Length];
        mSeObjs = new AudioSource[array3.Length];
        mBgmResources = new AudioClip[array2.Length];
        for (int j = 0; j < mBgmResources.Length; j++) {
            mBgmResources[j] = (array2[j] as AudioClip);
            mBgmObjs[j] = new GameObject(mBgmResources[j].name).AddComponent<AudioSource>();
            mBgmObjs[j].transform.parent = base.transform;
            mBgmObjs[j].clip = mBgmResources[j];
            mBgmResources[j] = null;
        }
        mVoiceResources = new AudioClip[array4.Length];
        for (int k = 0; k < mVoiceResources.Length; k++) {
            mVoiceResources[k] = (array4[k] as AudioClip);
            mVoiceObjs[k] = new GameObject(mVoiceResources[k].name).AddComponent<AudioSource>();
            mVoiceObjs[k].transform.parent = base.transform;
            mVoiceObjs[k].clip = mVoiceResources[k];
            mVoiceResources[k] = null;
        }
        mSeResources = new AudioClip[array3.Length];
        for (int l = 0; l < mSeResources.Length; l++) {
            mSeResources[l] = (array3[l] as AudioClip);
            mSeObjs[l] = new AudioSource();
            mSeObjs[l] = new GameObject(mSeResources[l].name).AddComponent<AudioSource>();
            mSeObjs[l].transform.parent = base.transform;
            mSeObjs[l].clip = mSeResources[l];
            mSeResources[l] = null;
        }
        StartCoroutine(_Load());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator _Load() {
        yield return new WaitForEndOfFrame();
        IsLoadResource = true;
    }
    /// <summary>
    /// 
    /// </summary>
    public void Update() {
        for (int i = 0; i < AUDIO_LIST_NUM; i++) {
            if (mTagPlayLists == null || mTagPlayLists[i].mType == AUDIO_TYPE.NONE) {
                continue;
            }
            mTagPlayLists[i].mWaitTime -= Time.deltaTime;
            if (!(mTagPlayLists[i].mWaitTime <= 0f)) {
                continue;
            }
            switch (mTagPlayLists[i].mType) {
                case AUDIO_TYPE.SE:
                    if (mTagPlayLists[i].mActive) {
                        SePlay(mTagPlayLists[i].mSeIndex, mTagPlayLists[i].mLoopFlg, mTagPlayLists[i].mSetTime, mTagPlayLists[i].mVolume);
                    } else {
                        SeStop(mTagPlayLists[i].mSeIndex);
                    }
                    break;
                case AUDIO_TYPE.BGM:
                    if (mTagPlayLists[i].mActive) {
                        BgmPlay(mTagPlayLists[i].mBgmIndex, mTagPlayLists[i].mLoopFlg, mTagPlayLists[i].mSetTime, mTagPlayLists[i].mVolume);
                    } else {
                        BgmStop(mTagPlayLists[i].mBgmIndex);
                    }
                    break;
                case AUDIO_TYPE.VOICE:
                    if (mTagPlayLists[i].mActive) {
                        VoicePlay(mTagPlayLists[i].mVoiceIndex, mTagPlayLists[i].mLoopFlg, mTagPlayLists[i].mSetTime, mTagPlayLists[i].mVolume);
                    } else {
                        VoiceStop(mTagPlayLists[i].mBgmIndex);
                    }
                    break;
            }
            mTagPlayLists[i].mWaitTime = 0f;
            mTagPlayLists[i].mType = AUDIO_TYPE.NONE;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_isUpdate"></param>
    public void PlayTitleBgm(bool _isUpdate = true) {
        string text = "";
        text = "bgm_menu";
        if (!IsBgmPlaying(text)) {
            BgmStop();
            //BKB let decide later
            BgmPlay(text, _loop: false);
        }
        ChangeBgmPitch(text, 1f);
    }
    /// <summary>
    /// 
    /// </summary>
    public void PlayGameBgm() {
        listInGameBgmName.Clear();
        string text = "";
        text = "bgm_main_0";
        UnityEngine.Debug.Log("BGM設定:" + SingletonCustom<GameSettingManager>.Instance.LastSelectGameType.ToString());
        switch (SingletonCustom<GameSettingManager>.Instance.LastSelectGameType) {
            default:
                listInGameBgmName.Add("bgm_main_3");
                listInGameBgmName.Add("bgm_main_5");
                listInGameBgmName.Add("bgm_main_7");
                listInGameBgmName.Add("bgm_main_8");
                listInGameBgmName.Add("bgm_main_11");
                listInGameBgmName.Add("bgm_main_12");
                break;
            case GS_Define.GameType.BOMB_ROULETTE:
            case GS_Define.GameType.ARCHER_BATTLE:
            case GS_Define.GameType.MINISCAPE_RACE:
            case GS_Define.GameType.PUSH_IN_BOXING:
            case GS_Define.GameType.BLOCK_SLICER:
            case GS_Define.GameType.GIRIGIRI_WATER:
            case GS_Define.GameType.GIRIGIRI_STOP:
            case GS_Define.GameType.CLIMB_WALL:
            case GS_Define.GameType.TREASURE_CATCHER:
            case GS_Define.GameType.AIR_BALLOON:
                listInGameBgmName.Add("bgm_main_0");
                listInGameBgmName.Add("bgm_main_1");
                listInGameBgmName.Add("bgm_main_9");
                break;
            case GS_Define.GameType.REVERSI:
            case GS_Define.GameType.AWAY_HOIHOI:
            case GS_Define.GameType.TIME_STOP:
            case GS_Define.GameType.ROBOT_WATCH:
            case GS_Define.GameType.ROCK_PAPER_SCISSORS:
            case GS_Define.GameType.NONSTOP_PICTURE:
            case GS_Define.GameType.TRAIN_GUIDE:
            case GS_Define.GameType.COLORFUL_SHOOT:
            case GS_Define.GameType.COIN_DROP:
                listInGameBgmName.Add("bgm_main_2");
                listInGameBgmName.Add("bgm_main_6");
                listInGameBgmName.Add("bgm_main_10");
                listInGameBgmName.Add("bgm_main_13");
                listInGameBgmName.Add("bgm_main_14");
                break;
        }
        if (listInGameBgmName.Count > 1) {
            listInGameBgmName.Remove(LastGameBgmIndex);
        }
        text = listInGameBgmName[UnityEngine.Random.Range(0, listInGameBgmName.Count)];
        if (!IsBgmPlaying(text)) {
            BgmStop();
            BgmPlay(text, _loop: true);
        }
        ChangeBgmPitch(text, 1f);
        LastGameBgmIndex = text;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_bgmIndex"></param>
    /// <param name="_loop"></param>
    /// <param name="_setTime"></param>
    /// <param name="_volume"></param>
    /// <param name="_pitch"></param>
    /// <param name="_delay"></param>
    public void BgmPlay(string _bgmIndex, bool _loop = false, float _setTime = 0f, float _volume = 1f, float _pitch = 1f, float _delay = 0f) {
        UnityEngine.Debug.Log("【Bgm】:" + _bgmIndex + " <<======================== Last:" + LastBgmIndex);
        if (!(_bgmIndex == LastBgmIndex)) {
            LastBgmIndex = _bgmIndex;
            _volume *= (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeBgm * 0.1f;
            StopCoroutine(_BgmPlay(GetBgmIndex(_bgmIndex)));
            //??StartCoroutine(_BgmPlay(GetBgmIndex(_bgmIndex), _loop, _setTime, _volume, _pitch, _delay));
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_bgmIndex"></param>
    /// <param name="_loop"></param>
    /// <param name="_setTime"></param>
    /// <param name="_volume"></param>
    /// <param name="_pitch"></param>
    /// <param name="_delay"></param>
    public void BgmPlayPitch(string _bgmIndex, bool _loop = false, float _setTime = 0f, float _volume = 1f, float _pitch = 1f, float _delay = 0f) {
        UnityEngine.Debug.Log("【Bgm】:" + _bgmIndex + " <<======================== Last:" + LastBgmIndex);
        LastBgmIndex = _bgmIndex;
        _volume *= (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeBgm * 0.1f;
        StopCoroutine(_BgmPlay(GetBgmIndex(_bgmIndex)));
        StartCoroutine(_BgmPlay(GetBgmIndex(_bgmIndex), _loop, _setTime, _volume, _pitch, _delay));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_volume"></param>
    public void BgmVolumeChange(float _volume = 1f) {
        if (mBgmObjs[lastBgmNo] != null) {
            _volume *= (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeBgm * 0.1f;
            mBgmObjs[lastBgmNo].volume = _volume;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void BgmPause() {
        pauseTargetIdx = -1;
        int num = 0;
        while (true) {
            if (num < mBgmObjs.Length) {
                if ((bool)mBgmObjs[num] && mBgmObjs[num].isPlaying) {
                    break;
                }
                num++;
                continue;
            }
            return;
        }
        mBgmObjs[num].Pause();
        pauseTargetIdx = num;
    }
    /// <summary>
    /// 
    /// </summary>
    public void BgmResume() {
        int num = 0;
        while (true) {
            if (num < mBgmObjs.Length) {
                if ((bool)mBgmObjs[num] && num == pauseTargetIdx) {
                    break;
                }
                num++;
                continue;
            }
            return;
        }
        mBgmObjs[num].UnPause();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_bgmIndex"></param>
    /// <param name="_loop"></param>
    /// <param name="_setTime"></param>
    /// <param name="_volume"></param>
    /// <param name="_pitch"></param>
    /// <param name="_delay"></param>
    /// <returns></returns>
    private IEnumerator _BgmPlay(int _bgmIndex, bool _loop = false, float _setTime = 0f, float _volume = 1f, float _pitch = 1f, float _delay = 0f) {
        yield return new WaitForSeconds(_delay);
        bool is_play_check = false;
        while (!is_play_check) {
            is_play_check = true;
            for (int i = 0; i < mBgmObjs.Length; i++) {
                if ((bool)mBgmObjs[i] && mBgmObjs[i].isPlaying) {
                    is_play_check = false;
                    break;
                }
            }
            if (!is_play_check) {
                yield return null;
            }
        }
        if (mBgmFlg != 0) {
            if (!mBgmObjs[_bgmIndex]) {
                mBgmObjs[_bgmIndex] = new GameObject(_bgmIndex.ToString()).AddComponent<AudioSource>();
                mBgmObjs[_bgmIndex].transform.parent = base.transform;
                mBgmObjs[_bgmIndex].clip = mBgmResources[_bgmIndex];
                mBgmResources[_bgmIndex] = null;
            }
            if (_bgmIndex < mBgmObjs.Length) {
                LeanTween.cancel(mBgmObjs[_bgmIndex].gameObject);
                LeanTween.value(mBgmObjs[_bgmIndex].gameObject, 0f, _volume, 0.25f).setEaseLinear().setOnUpdate(delegate (float _value) {
                    SetBgmFade(_value);
                })
                    .setIgnoreTimeScale(useUnScaledTime: true);
                mBgmObjs[_bgmIndex].time = _setTime;
                mBgmObjs[_bgmIndex].volume = 0f;
                mBgmObjs[_bgmIndex].Play();
                mBgmObjs[_bgmIndex].loop = _loop;
                mBgmObjs[_bgmIndex].pitch = _pitch;
                lastBgmNo = _bgmIndex;
                selectBgmNo = _bgmIndex;
            } else {
                UnityEngine.Debug.Log("Over Index");
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void RestartPlay() {
        BgmPlay(mBgmObjs[lastBgmNo].name, _loop: true);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_bgmIndex"></param>
    /// <param name="_fade"></param>
    public void BgmStop(string _bgmIndex = "", bool _fade = true) {
        if (string.IsNullOrEmpty(_bgmIndex)) {
            if (!_fade) {
                for (int i = 0; i < mBgmObjs.Length; i++) {
                    if ((bool)mBgmObjs[i]) {
                        mBgmObjs[i].Stop();
                    }
                }
                return;
            }
            selectBgmNo = mBgmObjs.Length;
            for (int j = 0; j < mBgmObjs.Length; j++) {
                if ((bool)mBgmObjs[j] && mBgmObjs[j].isPlaying) {
                    LeanTween.cancel(mBgmObjs[j].gameObject);
                    LeanTween.value(mBgmObjs[j].gameObject, 1f, 0f, 0.5f).setEaseLinear().setOnUpdate(delegate (float _value) {
                        SetBgmFade(_value);
                    })
                        .setOnComplete((Action)delegate {
                            CompleteFadeBgm();
                        })
                        .setIgnoreTimeScale(useUnScaledTime: true);
                }
            }
        } else if (GetBgmIndex(_bgmIndex) < mBgmObjs.Length) {
            if (!mBgmObjs[GetBgmIndex(_bgmIndex)]) {
                return;
            }
            if (!_fade) {
                mBgmObjs[GetBgmIndex(_bgmIndex)].Stop();
                return;
            }
            selectBgmNo = GetBgmIndex(_bgmIndex);
            if ((bool)mBgmObjs[selectBgmNo] && mBgmObjs[selectBgmNo].isPlaying) {
                LeanTween.cancel(mBgmObjs[selectBgmNo].gameObject);
                LeanTween.value(mBgmObjs[selectBgmNo].gameObject, mBgmObjs[selectBgmNo].volume, 0f, 0.5f).setEaseLinear().setOnUpdate(delegate (float _value) {
                    SetBgmFade(_value);
                })
                    .setOnComplete((Action)delegate {
                        CompleteFadeBgm();
                    })
                    .setIgnoreTimeScale(useUnScaledTime: true);
            }
        } else {
            UnityEngine.Debug.Log("Over Index");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_value"></param>
    private void SetBgmFade(float _value) {
        if (selectBgmNo == mBgmObjs.Length) {
            for (int i = 0; i < mBgmObjs.Length; i++) {
                if ((bool)mBgmObjs[i] && _value < mBgmObjs[i].volume) {
                    mBgmObjs[i].volume = _value;
                }
            }
        } else {
            mBgmObjs[selectBgmNo].volume = _value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void CompleteFadeBgm() {
        if (selectBgmNo == mBgmObjs.Length) {
            for (int i = 0; i < mBgmObjs.Length; i++) {
                if ((bool)mBgmObjs[i]) {
                    mBgmObjs[i].Stop();
                }
            }
        } else {
            mBgmObjs[selectBgmNo].Stop();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_bgmIndex"></param>
    /// <returns></returns>
    public bool IsBgmPlaying(string _bgmIndex) {
        if (!string.IsNullOrEmpty(_bgmIndex)) {
            if (!mBgmObjs[GetBgmIndex(_bgmIndex)]) {
                return false;
            }
            return mBgmObjs[GetBgmIndex(_bgmIndex)].isPlaying;
        }
        UnityEngine.Debug.Log("Over Index");
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_bgmName"></param>
    /// <returns></returns>
    private int GetBgmIndex(string _bgmName) {
        for (int i = 0; i < mBgmObjs.Length; i++) {
            if (mBgmObjs[i].clip.name == _bgmName) {
                return i;
            }
        }
        return -1;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_seName"></param>
    /// <returns></returns>
    private int GetSeIndex(string _seName) {
        for (int i = 0; i < mSeObjs.Length; i++) {
            if (mSeObjs[i].clip.name == _seName) {
                return i;
            }
        }
        return -1;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_voiceName"></param>
    /// <returns></returns>
    private int GetVoiceIndex(string _voiceName) {
        for (int i = 0; i < mVoiceObjs.Length; i++) {
            if (mVoiceObjs[i].clip.name == _voiceName) {
                return i;
            }
        }
        return -1;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_flg"></param>
    public void SetBgmFlg(bool _flg) {
        mBgmFlg = (_flg ? 1 : 0);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsBgmFlg() {
        if (mBgmFlg != 1) {
            return false;
        }
        return true;
    }
    /// <summary>
    /// 
    /// </summary>
    public void ReverseBgmFlg() {
        mBgmFlg = ((mBgmFlg == 0) ? 1 : 0);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_bgmIndex"></param>
    /// <param name="_pitch"></param>
    public void ChangeBgmPitch(string _bgmIndex, float _pitch) {
        if (!string.IsNullOrEmpty(_bgmIndex)) {
            if ((bool)mBgmObjs[GetBgmIndex(_bgmIndex)]) {
                mBgmObjs[GetBgmIndex(_bgmIndex)].pitch = _pitch;
            }
        } else {
            UnityEngine.Debug.Log("Over Index");
        }
    }
    public void SetBgmVolume() {
        float volume = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeBgm * 0.1f;
        for (int i = 0; i < mBgmObjs.Length; i++) {
            if ((bool)mBgmObjs[i]) {
                mBgmObjs[i].GetComponent<AudioSource>().volume = volume;
            }
        }
    }
    public void VoicePlay(string _voiceIndex, bool _loop = false, float _setTime = 0f, float _volume = 1f, float _pitch = 1f, float _delay = 0f, bool _fade = false) {
    }
    public void CharaVoicePlay(string _voiceIndex, bool _loop = false, float _setTime = 0f, float _volume = 1f, float _pitch = 1f, float _delay = 0f, bool _fade = false) {
        StartCoroutine(_VoicePlay(GetVoiceIndex(_voiceIndex), _loop, _setTime, _volume, _pitch, _delay, _fade));
    }
    public void VoiceVolumeChange(AUDIO_TYPE _type, float _volume = 1f) {
        if (mVoiceObjs[lastVoiceNo] != null) {
            mVoiceObjs[lastVoiceNo].volume = _volume;
        }
    }
    private IEnumerator _VoicePlay(int _voiceIndex, bool _loop = false, float _setTime = 0f, float _volume = 1f, float _pitch = 1f, float _delay = 0f, bool _fade = false) {
        yield return new WaitForSeconds(_delay);
        _volume = _volume * (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeVoice * 0.1f;
        if (mVoiceFlg == 0) {
            yield break;
        }
        if (!mVoiceObjs[_voiceIndex]) {
            mVoiceObjs[_voiceIndex] = new GameObject(_voiceIndex.ToString()).AddComponent<AudioSource>();
            mVoiceObjs[_voiceIndex].transform.parent = base.transform;
            mVoiceObjs[_voiceIndex].clip = mVoiceResources[_voiceIndex];
            mVoiceResources[_voiceIndex] = null;
        }
        if (_voiceIndex < mVoiceObjs.Length) {
            LeanTween.cancel(mVoiceObjs[_voiceIndex].gameObject);
            if (_fade) {
                mVoiceObjs[_voiceIndex].volume = 0f;
                LeanTween.value(mVoiceObjs[_voiceIndex].gameObject, 0f, _volume, 0.25f).setEaseLinear().setOnUpdate(delegate (float _value) {
                    SetVoiceFade(_value);
                })
                    .setIgnoreTimeScale(useUnScaledTime: true);
            } else {
                mVoiceObjs[_voiceIndex].volume = _volume;
            }
            mVoiceObjs[_voiceIndex].time = _setTime;
            mVoiceObjs[_voiceIndex].Play();
            mVoiceObjs[_voiceIndex].loop = _loop;
            lastVoiceNo = _voiceIndex;
            selectVoiceNo = _voiceIndex;
        } else {
            UnityEngine.Debug.Log("Over Index");
        }
    }
    public void RestartPlayVoice(AUDIO_TYPE _type) {
        VoicePlay(mVoiceObjs[lastVoiceNo].name, _loop: true);
    }
    public void VoiceStop(string _voiceIndex = "", bool _fade = false) {
        if (string.IsNullOrEmpty(_voiceIndex)) {
            if (!_fade) {
                for (int i = 0; i < mVoiceObjs.Length; i++) {
                    if ((bool)mVoiceObjs[i]) {
                        mVoiceObjs[i].Stop();
                    }
                }
                return;
            }
            selectVoiceNo = mVoiceObjs.Length;
            for (int j = 0; j < mVoiceObjs.Length; j++) {
                if ((bool)mVoiceObjs[j] && mVoiceObjs[j].isPlaying) {
                    LeanTween.cancel(mVoiceObjs[j].gameObject);
                    LeanTween.value(mVoiceObjs[j].gameObject, 1f, 0f, 0.5f).setEaseLinear().setOnUpdate(delegate (float _value) {
                        SetVoiceFade(_value);
                    })
                        .setOnComplete((Action)delegate {
                            CompleteFadeVoice();
                        })
                        .setIgnoreTimeScale(useUnScaledTime: true);
                }
            }
            return;
        }
        StopCoroutine(_VoicePlay(GetVoiceIndex(_voiceIndex)));
        if (GetVoiceIndex(_voiceIndex) < mVoiceObjs.Length) {
            if (!mVoiceObjs[GetVoiceIndex(_voiceIndex)]) {
                return;
            }
            if (!_fade) {
                mVoiceObjs[GetVoiceIndex(_voiceIndex)].Stop();
                return;
            }
            selectVoiceNo = GetVoiceIndex(_voiceIndex);
            if ((bool)mVoiceObjs[selectVoiceNo]) {
                LeanTween.cancel(mVoiceObjs[selectVoiceNo].gameObject);
                LeanTween.value(mVoiceObjs[selectVoiceNo].gameObject, mVoiceObjs[selectVoiceNo].volume, 0f, 0.5f).setEaseLinear().setOnUpdate(delegate (float _value) {
                    SetVoiceFade(_value);
                })
                    .setOnComplete((Action)delegate {
                        CompleteFadeVoice();
                    })
                    .setIgnoreTimeScale(useUnScaledTime: true);
            }
        } else {
            UnityEngine.Debug.Log("Over Index");
        }
    }
    private void SetVoiceFade(float _value) {
        if (selectVoiceNo == mVoiceObjs.Length) {
            for (int i = 0; i < mVoiceObjs.Length; i++) {
                if ((bool)mVoiceObjs[i] && _value < mVoiceObjs[i].volume) {
                    mVoiceObjs[i].volume = _value;
                }
            }
        } else {
            mVoiceObjs[selectVoiceNo].volume = _value;
        }
    }
    private void CompleteFadeVoice() {
        if (selectVoiceNo == mVoiceObjs.Length) {
            for (int i = 0; i < mVoiceObjs.Length; i++) {
                if ((bool)mVoiceObjs[i]) {
                    mVoiceObjs[i].Stop();
                }
            }
        } else {
            mVoiceObjs[selectVoiceNo].Stop();
        }
    }
    public bool IsVoicePlaying(string _voiceIndex) {
        if (GetVoiceIndex(_voiceIndex) < mVoiceObjs.Length) {
            if (!mVoiceObjs[GetVoiceIndex(_voiceIndex)]) {
                return false;
            }
            return mVoiceObjs[GetVoiceIndex(_voiceIndex)].isPlaying;
        }
        UnityEngine.Debug.Log("Over Index");
        return false;
    }
    public void SetVoiceFlg(bool _flg) {
        mVoiceFlg = (_flg ? 1 : 0);
    }
    public bool IsVoiceFlg() {
        if (mVoiceFlg != 1) {
            return false;
        }
        return true;
    }
    public void ReverseVoiceFlg() {
        mVoiceFlg = ((mVoiceFlg == 0) ? 1 : 0);
    }
    public void ChangeVoicePitch(string _voiceIndex, float _pitch) {
        if (GetVoiceIndex(_voiceIndex) < mVoiceObjs.Length) {
            if ((bool)mVoiceObjs[GetVoiceIndex(_voiceIndex)]) {
                mVoiceObjs[GetVoiceIndex(_voiceIndex)].pitch = _pitch;
            }
        } else {
            UnityEngine.Debug.Log("Over Index");
        }
    }
    public void SetVoiceVolume() {
        float volume = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
        for (int i = 0; i < mVoiceObjs.Length; i++) {
            if ((bool)mVoiceObjs[i]) {
                mVoiceObjs[i].GetComponent<AudioSource>().volume = volume;
            }
        }
    }
    public void SeStopCoroutine() {
        StopAllCoroutines();
    }
    public void SePlay(string _seName, bool _loop = false, float _setTime = 0f, float _volume = 1f, float _pitch = 1f, float _delay = 0f, bool _overlap = false) {
        _volume *= (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
        StartCoroutine(_SePlay(GetSeIndex(_seName), _loop, _setTime, _volume, _pitch, _delay, _overlap));
    }
    public IEnumerator _SePlay(int _seIndex, bool _loop = false, float _setTime = 0f, float _volume = 1f, float _pitch = 1f, float _delay = 0f, bool _overlap = false) {
        yield return new WaitForSeconds(_delay);
        if (mSeFlg == 0) {
            yield break;
        }
        if (!mSeObjs[_seIndex]) {
            mSeObjs[_seIndex] = new GameObject(_seIndex.ToString()).AddComponent<AudioSource>();
            mSeObjs[_seIndex].transform.parent = base.transform;
            mSeObjs[_seIndex].clip = mSeResources[_seIndex];
            mSeResources[_seIndex] = null;
        }
        if (_seIndex < mSeObjs.Length) {
            mSeObjs[_seIndex].clip = mSeObjs[_seIndex].clip;
            mSeObjs[_seIndex].volume = _volume;
            mSeObjs[_seIndex].pitch = _pitch;
            mSeObjs[_seIndex].loop = _loop;
            if (_overlap) {
                mSeObjs[_seIndex].volume = 1f;
                mSeObjs[_seIndex].PlayOneShot(mSeObjs[_seIndex].clip, _volume);
            } else {
                mSeObjs[_seIndex].volume = _volume;
                mSeObjs[_seIndex].Play();
            }
            mSeObjs[_seIndex].time = _setTime;
        } else {
            UnityEngine.Debug.Log("Over Index");
        }
    }
    public void SeStop(string _seIndex = "", bool _fade = false, float fadeTime = 0.5f) {
        if (string.IsNullOrEmpty(_seIndex)) {
            if (!_fade) {
                for (int i = 0; i < mSeObjs.Length; i++) {
                    if ((bool)mSeObjs[i]) {
                        mSeObjs[i].Stop();
                    }
                }
                return;
            }
            for (int j = 0; j < mSeObjs.Length; j++) {
                if ((bool)mSeObjs[j] && mSeObjs[j].isPlaying) {
                    LeanTween.cancel(mSeObjs[j].gameObject);
                    LeanTween.value(mSeObjs[j].gameObject, 1f, 0f, fadeTime).setEaseLinear().setOnUpdate(delegate (float _value) {
                        SetSeFade(_value);
                    })
                        .setOnComplete((Action)delegate {
                            CompleteFadeSe();
                        })
                        .setIgnoreTimeScale(useUnScaledTime: true);
                }
            }
        } else if ((bool)mSeObjs[GetSeIndex(_seIndex)]) {
            if (!_fade) {
                mSeObjs[GetSeIndex(_seIndex)].Stop();
            } else if ((bool)mSeObjs[GetSeIndex(_seIndex)] && mSeObjs[GetSeIndex(_seIndex)].isPlaying) {
                LeanTween.cancel(mSeObjs[GetSeIndex(_seIndex)].gameObject);
                LeanTween.value(mSeObjs[GetSeIndex(_seIndex)].gameObject, mSeObjs[GetSeIndex(_seIndex)].volume, 0f, fadeTime).setEaseLinear().setOnUpdate(delegate (float _value) {
                    SetSeFade(_value, _seIndex);
                })
                    .setOnComplete((Action)delegate {
                        CompleteFadeSe(_seIndex);
                    })
                    .setIgnoreTimeScale(useUnScaledTime: true);
            }
        }
    }
    private void SetSeFade(float _value, string _seIndex = "") {
        if (string.IsNullOrEmpty(_seIndex)) {
            for (int i = 0; i < mSeObjs.Length; i++) {
                if ((bool)mSeObjs[i] && _value < mSeObjs[i].volume) {
                    mSeObjs[i].volume = _value;
                }
            }
        } else {
            mSeObjs[GetSeIndex(_seIndex)].volume = _value;
        }
    }
    private void CompleteFadeSe(string _seIndex = "") {
        if (string.IsNullOrEmpty(_seIndex)) {
            for (int i = 0; i < mSeObjs.Length; i++) {
                if ((bool)mSeObjs[i]) {
                    mSeObjs[i].Stop();
                }
            }
        } else {
            mSeObjs[GetSeIndex(_seIndex)].Stop();
        }
    }
    public void SePause(string _seIndex, bool _isPause) {
        if ((bool)mSeObjs[GetSeIndex(_seIndex)]) {
            if (_isPause) {
                mSeObjs[GetSeIndex(_seIndex)].Pause();
            } else {
                mSeObjs[GetSeIndex(_seIndex)].UnPause();
            }
        }
    }
    public void SeLoopPause() {
        for (int i = 0; i < mSeObjs.Length; i++) {
            if (mSeObjs[i].loop) {
                mSeObjs[i].Pause();
            }
        }
    }
    public void SeLoopPauseRelease() {
        for (int i = 0; i < mSeObjs.Length; i++) {
            if (mSeObjs[i].loop) {
                mSeObjs[i].UnPause();
            }
        }
    }
    public bool IsSePlaying(string _seIndex) {
        if (!mSeObjs[GetSeIndex(_seIndex)]) {
            return false;
        }
        return mSeObjs[GetSeIndex(_seIndex)].isPlaying;
    }
    public void ReverseSeFlg() {
        mSeFlg = ((mSeFlg == 0) ? 1 : 0);
    }
    public void ChangeSePitch(string _seIndex, float _pitch) {
        if (GetSeIndex(_seIndex) < mSeObjs.Length) {
            if ((bool)mSeObjs[GetSeIndex(_seIndex)]) {
                mSeObjs[GetSeIndex(_seIndex)].pitch = _pitch;
            }
        } else {
            UnityEngine.Debug.Log("Over Index");
        }
    }
    public bool IsSeFlg() {
        if (mSeFlg != 1) {
            return false;
        }
        return true;
    }
    public void SetSeVolume() {
        float num = (float)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.volumeSe * 0.1f;
        for (int i = 0; i < mSeObjs.Length; i++) {
            if ((bool)mSeObjs[i]) {
                UnityEngine.Debug.Log(mSeObjs[i].name);
                if (mSeObjs[i].name == "se_hanabi") {
                    mSeObjs[i].GetComponent<AudioSource>().volume = num * 0.25f;
                } else {
                    mSeObjs[i].GetComponent<AudioSource>().volume = num;
                }
            }
        }
    }
    public void SetSoundFlg(bool _flg) {
        SetBgmFlg(_flg);
        SetSeFlg(_flg);
        SetVoiceFlg(_flg);
    }
    public void SetSeFlg(bool _flg) {
        mSeFlg = (_flg ? 1 : 0);
    }
    public bool IsSound() {
        return IsBgmFlg();
    }
    public bool GetBgmFlg() {
        if (mBgmFlg != 1) {
            return false;
        }
        return true;
    }
    public void CallStopAllCoroutines() {
        StopAllCoroutines();
    }
    public string GetSeName(int _no) {
        if (mSeObjs == null) {
            return "";
        }
        return mSeObjs[_no].name;
    }
    public int GetSeNum() {
        if (mSeObjs == null) {
            return 0;
        }
        return mSeObjs.Length;
    }
}
