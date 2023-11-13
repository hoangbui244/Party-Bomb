using System;
using UnityEngine;
public class SpriteShiningEffect : MonoBehaviour {
    [SerializeField]
    [Header("光らせる画像")]
    private SpriteRenderer flashingSprite;
    [SerializeField]
    [Header("光らせる間隔")]
    private float flashingInterval = 2.5f;
    private float flashingIntervalProcess;
    private const float FLASHING_ANIM_TIME = 1f;
    private bool isFlashing;
    private void OnEnable() {
        flashingSprite.material.SetFloat("_Gradation", 0f);
        isFlashing = false;
    }
    private void OnDisable() {
        LeanTween.cancel(base.gameObject);
    }
    private void Update() {
        if (!isFlashing) {
            if (flashingIntervalProcess > flashingInterval) {
                flashingIntervalProcess = 0f;
                FlashingAnimation();
            } else {
                flashingIntervalProcess += Time.deltaTime;
            }
        }
    }
    public void FlashingAnimation() {
        isFlashing = true;
        LeanTween.value(base.gameObject, 0f, 10f, 1f).setOnUpdate(delegate (float value) {
            flashingSprite.material.SetFloat("_Gradation", value);
        }).setOnComplete((Action)delegate {
            isFlashing = false;
        });
    }
}
