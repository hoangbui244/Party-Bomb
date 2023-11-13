using System;
using UnityEngine;
public class AlpineSkiing_RadioControl : MonoBehaviour {
    private const float SPEED_LINE_SCROLL_SPEED = 0.5f;
    [SerializeField]
    public GameObject speedLineObjs;
    [SerializeField]
    private Material speedLineMaterials;
    private bool isViewSpeedLines;
    public void UpdateMethod() {
        SpeedLineUpdate();
    }
    private void Start() {
        speedLineMaterials.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
        speedLineObjs.SetActive(value: false);
        isViewSpeedLines = false;
    }
    public void SpeedLineStart(float _time = 0.5f) {
        LeanTween.cancel(speedLineObjs, callOnComplete: false);
        speedLineMaterials.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
        speedLineObjs.SetActive(value: true);
        isViewSpeedLines = true;
        LeanTween.value(speedLineObjs, 0f, 1f, _time).setOnUpdate(delegate (float _value) {
            speedLineMaterials.SetColor("_Color", new Color(1f, 1f, 1f, _value));
        }).setOnComplete((Action)delegate {
            speedLineMaterials.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
        });
    }
    public void SpeedLineEnd(float _time = 1f) {
        LeanTween.cancel(speedLineObjs, callOnComplete: false);
        speedLineMaterials.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
        LeanTween.value(speedLineObjs, 1f, 0f, _time).setOnUpdate(delegate (float _value) {
            speedLineMaterials.SetColor("_Color", new Color(1f, 1f, 1f, _value));
        }).setOnComplete((Action)delegate {
            speedLineMaterials.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
            speedLineObjs.SetActive(value: false);
            isViewSpeedLines = false;
        });
    }
    private void SpeedLineUpdate() {
        if (isViewSpeedLines) {
            Vector2 mainTextureOffset = speedLineMaterials.mainTextureOffset;
            mainTextureOffset.y = (mainTextureOffset.y + Time.deltaTime * 0.5f) % 1f;
            speedLineMaterials.mainTextureOffset = mainTextureOffset;
        }
    }
}
