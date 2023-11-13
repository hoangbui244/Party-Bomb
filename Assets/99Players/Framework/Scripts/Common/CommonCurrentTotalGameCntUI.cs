using TMPro;
using UnityEngine;
public class CommonCurrentTotalGameCntUI : MonoBehaviour {
    [SerializeField]
    [Header("現在の回数テキスト")]
    private TextMeshPro currentCntText;
    [SerializeField]
    [Header("ト\u30fcタル回数テキスト")]
    private TextMeshPro totalCntText;
    public void SetCurrentGameCnt(int _cnt) {
        currentCntText.text = _cnt.ToString();
    }
    public void SetTotalGameCnt(int _cnt) {
        totalCntText.text = _cnt.ToString();
    }
}
