using System;
using System.Collections;
using UnityEngine;
public class SimpleRotateAnimation : MonoBehaviour {
    [SerializeField]
    [Header("開始回転角度")]
    private Vector3 startAngle;
    [SerializeField]
    [Header("終了回転角度")]
    private Vector3 endAngle;
    [SerializeField]
    [Header("演出時間")]
    private float animationTime = 1f;
    [SerializeField]
    [Header("反復ル\u30fcプ演出(1←→2)")]
    private bool isLoopPingPong;
    [SerializeField]
    [Header("継続ル\u30fcプ演出(1→2→1→2…)")]
    private bool isLoopContinue;
    [SerializeField]
    [Header("単純ル\u30fcプ演出")]
    private bool isLoopSimple;
    [SerializeField]
    [Header("Tweenの種類")]
    private LeanTweenType leanTweenType;
    [SerializeField]
    [Header("ディレイランダム値")]
    private float delayRandom;
    private void OnEnable() {
        StartCoroutine(_Start());
    }
    private IEnumerator _Start() {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, delayRandom));
        base.transform.localEulerAngles = startAngle;
        if (isLoopPingPong) {
            LeanTween.rotateLocal(base.gameObject, endAngle, animationTime).setEase(leanTweenType).setLoopPingPong();
        } else if (isLoopContinue) {
            LoopContinueProcess_1();
        } else if (isLoopSimple) {
            LeanTween.rotateLocal(base.gameObject, endAngle, animationTime).setEase(leanTweenType).setLoopCount(-1);
        } else {
            LeanTween.rotateLocal(base.gameObject, endAngle, animationTime).setEase(leanTweenType);
        }
    }
    private void LoopContinueProcess_1() {
        LeanTween.rotateLocal(base.gameObject, endAngle, animationTime).setEase(leanTweenType).setOnComplete((Action)delegate {
            LoopContinueProcess_2();
        });
    }
    private void LoopContinueProcess_2() {
        LeanTween.rotateLocal(base.gameObject, startAngle, animationTime).setEase(leanTweenType).setOnComplete((Action)delegate {
            LoopContinueProcess_1();
        });
    }
    private void OnDisable() {
        LeanTween.cancel(base.gameObject);
    }
}
