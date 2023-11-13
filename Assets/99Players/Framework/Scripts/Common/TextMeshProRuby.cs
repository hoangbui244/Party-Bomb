using TMPro;
using UnityEngine;
[RequireComponent(typeof(TMP_Text))]
public class TextMeshProRuby : MonoBehaviour {
    [SerializeField]
    [HideInInspector]
    private TMP_Text tmpText;
    [TextArea(5, 10)]
    [Tooltip("ルビは <r=もじ>文字</r> もしくは <r=\"もじ\">文字</r>")]
    [SerializeField]
    private string text;
    [Tooltip("行間を固定します")]
    [SerializeField]
    [HideInInspector]
    private bool fixedLineHeight;
    [Tooltip("1行目のルビ有無によって自動でMarginTopを追加します")]
    [SerializeField]
    [HideInInspector]
    private bool autoMarginTop = true;
    public string Text {
        get {
            return text;
        }
        set {
            text = value;
            if (base.enabled) {
                Apply();
            }
        }
    }
    public bool FixedLineHeight {
        get {
            return fixedLineHeight;
        }
        set {
            bool num = fixedLineHeight != value;
            fixedLineHeight = value;
            if (num && base.enabled) {
                Apply();
            }
        }
    }
    public bool AutoMarginTop {
        get {
            return autoMarginTop;
        }
        set {
            bool num = autoMarginTop != value;
            autoMarginTop = value;
            if (num && base.enabled) {
                Apply();
            }
        }
    }
    private void OnEnable() {
        Apply();
    }
    public void Apply() {
        if (!tmpText) {
            tmpText = GetComponent<TMP_Text>();
        }
        tmpText.SetTextAndExpandRuby(Text, fixedLineHeight, autoMarginTop);
    }
}
