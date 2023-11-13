using System;
using UnityEngine;
public class AreaEffect : MonoBehaviour {
    [SerializeField]
    private MeshRenderer frame;
    [SerializeField]
    private SkinnedMeshRenderer light;
    [SerializeField]
    private Transform anchor;
    private readonly float colorAlpha = 0.3f;
    private Vector3 localScaleDef;
    private readonly float showTime = 0.3f;
    private readonly float fadeTime = 0.3f;
    public void Init() {
        localScaleDef = anchor.localScale;
        anchor.gameObject.SetActive(value: false);
    }
    public void PlayEffect(Color _color) {
        anchor.gameObject.SetActive(value: true);
        light.transform.SetLocalScaleY(0f);
        _color.a = colorAlpha;
        frame.material.color = _color;
        _color.a = 0f;
        light.material.color = _color;
        LeanTween.value(0f, 1f, showTime).setOnUpdate(delegate (float _value) {
            Color color2 = light.material.color;
            color2.a = LeanTween.easeOutCubic(0f, 1f, _value) * colorAlpha;
            light.material.SetColor("_TintColor", color2);
            light.transform.SetLocalScaleY(LeanTween.easeOutCubic(0f, 1f, _value) * localScaleDef.y);
        });
        LeanTween.value(0f, 1f, fadeTime).setOnUpdate(delegate (float _value) {
            Color color = frame.material.color;
            color.a = LeanTween.easeOutCubic(1f, 0f, _value) * colorAlpha;
            light.material.SetColor("_TintColor", color);
            light.transform.SetLocalScaleY(LeanTween.easeOutCubic(1f, 0f, _value) * localScaleDef.y);
        }).setDelay(showTime)
            .setOnComplete((Action)delegate {
                anchor.gameObject.SetActive(value: false);
            });
    }
}
