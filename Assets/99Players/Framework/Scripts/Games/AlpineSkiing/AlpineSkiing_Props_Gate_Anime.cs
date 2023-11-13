using UnityEngine;
public class AlpineSkiing_Props_Gate_Anime : MonoBehaviour {
    [SerializeField]
    [Header("ゲ\u30fcトの扉(右側)")]
    private GameObject gate_Right;
    [SerializeField]
    [Header("ゲ\u30fcトの扉(左側)")]
    private GameObject gate_Left;
    public void AnimeStart() {
        if (base.gameObject.activeInHierarchy) {
            LeanTween.rotateY(gate_Right, 90f, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutQuad);
            LeanTween.rotateY(gate_Left, -90f, 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeOutQuad);
            SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_gate");
        }
    }
}
