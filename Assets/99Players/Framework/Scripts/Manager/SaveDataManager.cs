using SaveDataDefine;
using System;
using System.IO;
using System.Text;
using UnityEngine;
public class SaveDataManager : SingletonCustom<SaveDataManager> {
    public enum LanguageList {
        English,
        Japanese
    }
    [Serializable]
    public class SaveData_v100 {
        public SystemData systemData;
        public RecordData recordData;
        public TrophyData trophyData;
        public bool isTalkGameOpen;
        public bool isTalkCollectionOpen;
        public bool isTalkAllGameRelease;
        public bool isFirstGamePlayed;
        public bool isRelease_DLC1;
        public bool isRelease_DLC2;
        public bool isRelease_DLC3;
        public bool[] isStartHelp;
        public SaveData_v100() {
            UnityEngine.Debug.Log("SaveData_v100 コンストラクタ");
            systemData = new SystemData();
            recordData = new RecordData();
            trophyData = new TrophyData();
            isStartHelp = new bool[32];
            isTalkGameOpen = true;
            isTalkCollectionOpen = true;
            isTalkAllGameRelease = true;
            isFirstGamePlayed = true;
            isRelease_DLC1 = false;
        }
    }
    public static string EDITOR_SAVE_KEY = "editor_save_key";
    private SaveData_v100 saveData;
    public SaveData_v100 SaveData => saveData;
    public bool IsSaving {
        get;
        set;
    }
    public string TempEditName {
        get;
        set;
    }
    private void Awake() {
        UnityEngine.Object.DontDestroyOnLoad(this);
        _Init();
        _Load();
        SettingLanguage();
    }
    private void InitSaveData() {
        saveData = new SaveData_v100();
    }
    private void _Init() {
    }
    public void _Save() {
        PlayerPrefs.SetString(EDITOR_SAVE_KEY, JsonMapper.ToJson(saveData));
        PlayerPrefs.Save();
    }
    public void _Load() {
        string @string = PlayerPrefs.GetString(EDITOR_SAVE_KEY, "");
        if (string.IsNullOrEmpty(@string)) {
            InitSaveData();
        } else {
            saveData = JsonMapper.ToObject<SaveData_v100>(@string);
        }
    }
    private void Update() {
    }
    private void ToConvert(int _oldVersion, int _nowVersion, string _jsonStr) {
        for (int i = _oldVersion; i < _nowVersion; i++) {
        }
    }
    public void SettingLanguage() {
        FileInfo fileInfo = new FileInfo(Application.dataPath + "/LanguageSetting.txt");
        string str = "";
        try {
            using (StreamReader streamReader = new StreamReader(fileInfo.OpenRead(), Encoding.UTF8)) {
                str = streamReader.ReadLine();
            }
        } catch (Exception) {
        }
        switch (GetLangNum(str)) {
            case LanguageList.English:
                Localize_Define.Language = Localize_Define.LanguageType.English;
                break;
            case LanguageList.Japanese:
                Localize_Define.Language = Localize_Define.LanguageType.Japanese;
                break;
            default:
                Localize_Define.Language = Localize_Define.LanguageType.English;
                break;
        }
    }
    private LanguageList GetLangNum(string _str) {
        LanguageList languageList = LanguageList.English;
        switch (_str) {
            case "Japanese":
                return LanguageList.Japanese;
            case "English":
                return LanguageList.English;
            default:
                return LanguageList.English;
        }
    }
    public override void OnDestroy() {
        base.OnDestroy();
        _Save();
    }
}
