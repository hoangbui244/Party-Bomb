using io.ninenine.players.party3d.games.common;
using System;
using TMPro;
using UnityEngine;
public class CommonTimeUI : MonoBehaviour {
    [Serializable]
    public class CommonTimeUILayout {
        [field: SerializeField] public GameObject Root { get; private set; }
        [field: SerializeField] public TextMeshPro TimeText { get; private set; }
    }
    #region Inspector
    [field: Header("References")]
    [field: SerializeField] public CommonTimeUILayout TopUILayout { get; private set; }
    [field: SerializeField] public CommonTimeUILayout BottomUILayout { get; private set; }
    #endregion
    private CommonTimeUILayout m_CurrUILayout;
    public void Init(Anchor anchor) {
        TopUILayout.Root.SetActive(false);
        BottomUILayout.Root.SetActive(false);
        switch (anchor) {
            case Anchor.TopLeft:
            case Anchor.TopMiddle:
            case Anchor.TopRight:
            case Anchor.MiddleLeft:
            case Anchor.Center:
            case Anchor.MiddleRight:
                m_CurrUILayout = TopUILayout;
                break;
            case Anchor.BottomLeft:
            case Anchor.BottomMiddle:
            case Anchor.BottomRight:
                m_CurrUILayout = BottomUILayout;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null);
        }
        m_CurrUILayout.Root.SetActive(true);
    }
    public void SetTime(float _time) {
        if (m_CurrUILayout == null) {
            return;
        }
        int num = Mathf.CeilToInt(_time) / 60;
        int num2 = Mathf.CeilToInt(_time) % 60;
        m_CurrUILayout.TimeText.text = num.ToString() + ":" + ((num2.ToString().Length == 1) ? ("0" + num2.ToString()) : num2.ToString());
    }
}