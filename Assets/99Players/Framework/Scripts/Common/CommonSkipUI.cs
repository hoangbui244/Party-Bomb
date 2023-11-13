using TMPro;
using UnityEngine;
public class CommonSkipUI : MonoBehaviour {
    [SerializeField]
    [Header("フレ\u30fcム")]
    private SpriteRenderer frame;
    [SerializeField]
    [Header("テキスト")]
    private TextMeshPro text;
    public void ShowSkipUI() {
        frame.SetAlpha(0f);
        text.SetAlpha(0f);
        gameObject.SetActive(value: true);
        LeanTween.value(0f, 1f, 0.5f).setOnUpdate(delegate (float _value) {
            frame.SetAlpha(_value);
            text.SetAlpha(_value);
        });
    }
}
