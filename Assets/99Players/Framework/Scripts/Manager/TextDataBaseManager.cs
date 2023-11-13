using UnityEngine;
public class TextDataBaseManager : SingletonCustom<TextDataBaseManager> {
    [SerializeField]
    [Header("テキストデ\u30fcタベ\u30fcス")]
    private TextDataBaseItems[] tdbItems;
    [SerializeField]
    [Header("テキストデ\u30fcタベ\u30fcス（English）")]
    private TextDataBaseItems[] tdbItems_EN;
    private TalkDataTable talkTable;
    private static int FACE_TAG_MAX = 5;
    private string resStr;
    public string Get(TextDataBaseItemEntity.DATABASE_NAME _name, int _id) {
        if (Localize_Define.Language != 0) {
            return tdbItems_EN[(int)_name].Entities[_id].text;
        }
        return tdbItems[(int)_name].Entities[_id].text;
    }
    public string GetReplaceTag(TextDataBaseItemEntity.DATABASE_NAME _name, int _id, TextDataBaseItemEntity.TAG_NAME _tag, params string[] _replaceTextArgs) {
        resStr = Get(_name, _id);
        for (int i = 0; i < _replaceTextArgs.Length && i < TextDataBaseItemEntity.TAG_PARAM_CNT; i++) {
            resStr = resStr.Replace(TextDataBaseItemEntity.TAG_NAME_STR[(int)_tag * TextDataBaseItemEntity.TAG_PARAM_CNT + i], _replaceTextArgs[i]);
        }
        return resStr;
    }
    public TalkData[] GetTalkData(TalkDataTable.TalkType _type) {
        return talkTable.GetTalkData(_type);
    }
    private void Awake() {
        Object.DontDestroyOnLoad(base.gameObject);
        talkTable = new TalkDataTable();
    }
}
