using TMPro;
using UnityEngine;
public class FPSDisplay : MonoBehaviour {
    [SerializeField]
    [Header("FPSテキスト")]
    private TextMeshPro fpsText;
    [SerializeField]
    [Header("FPSが安定時の文字色")]
    private Color fpsSafeColor;
    [SerializeField]
    [Header("FPSが不安定時の文字色")]
    private Color fpsCautionColor;
    [SerializeField]
    [Header("FPSが危険時の文字色")]
    private Color fpsDangerColor;
    private int frameCount;
    private float prevTime;
    private float fps;
    private void Start() {
        frameCount = 0;
        prevTime = 0f;
        base.gameObject.SetActive(SingletonCustom<SceneManager>.Instance.displayFPS);
    }
    private void Update() {
        frameCount++;
        float num = Time.realtimeSinceStartup - prevTime;
        if (num >= 0.5f) {
            fps = CalcManager.ConvertDecimalFirst((float)frameCount / num);
            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
        if (fps >= 40f) {
            fpsText.color = fpsSafeColor;
        } else if (fps >= 20f) {
            fpsText.color = fpsCautionColor;
        } else {
            fpsText.color = fpsDangerColor;
        }
        fpsText.text = "FPS：" + fps.ToString();
    }
}
