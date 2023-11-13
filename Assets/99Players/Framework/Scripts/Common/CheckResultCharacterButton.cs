using TMPro;
using UnityEngine;
public class CheckResultCharacterButton : MonoBehaviour {
    [SerializeField]
    [Header("ボタン画像")]
    private SpriteRenderer buttonSprite;
    [SerializeField]
    [Header("ボタン文字")]
    private TextMeshPro buttonText;
    public void Active() {
        buttonSprite.color = Color.white;
        buttonText.color = Color.white;
    }
    public void NonActive() {
        buttonSprite.color = Color.gray;
        buttonText.color = Color.gray;
    }
}
