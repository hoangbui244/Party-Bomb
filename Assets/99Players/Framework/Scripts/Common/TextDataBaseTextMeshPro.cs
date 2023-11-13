using TMPro;
using UnityEngine;
public class TextDataBaseTextMeshPro : MonoBehaviour {
    public TextDataBaseItemEntity.DATABASE_NAME dataBaseName;
    public int dataBaseId;
    private TextMeshPro textMeshPro;
    private void Awake() {
        TextUpdate();
    }
    public void TextUpdate(int _id = -1) {
        if (textMeshPro == null) {
            textMeshPro = GetComponent<TextMeshPro>();
        }
        if (_id != -1) {
            dataBaseId = _id;
            textMeshPro.text = SingletonCustom<TextDataBaseManager>.Instance.Get(dataBaseName, dataBaseId);
        }
    }
}