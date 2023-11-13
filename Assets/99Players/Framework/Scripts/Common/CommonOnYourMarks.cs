using System;
using System.Collections;
using UnityEngine;
public class CommonOnYourMarks : SingletonCustom<CommonOnYourMarks> {
    [SerializeField]
    [Header("ポップオブジェクト")]
    private GameObject objPop;
    [SerializeField]
    [Header("テキストオブジェクト")]
    private GameObject[] arrayTextObj;
    private bool isStartRandom;
    public void Play(Action _callBack = null, bool _isStartRandomTime = false) {
        isStartRandom = _isStartRandomTime;
        StartCoroutine(StartOnYourMarksAnimation(_callBack));
    }
    private IEnumerator StartOnYourMarksAnimation(Action _callBack) {
        arrayTextObj[0].SetActive(value: true);
        arrayTextObj[1].SetActive(value: false);
        arrayTextObj[2].SetActive(value: false);
        objPop.SetActive(value: true);
        objPop.transform.SetLocalScale(0f, 0f, 0f);
        LeanTween.scale(objPop, Vector3.one, 0.35f).setEaseOutQuart();
        yield return new WaitForSeconds(2f);
        arrayTextObj[0].SetActive(value: false);
        arrayTextObj[1].SetActive(value: true);
        objPop.transform.SetLocalScale(0f, 0f, 0f);
        LeanTween.scale(objPop, Vector3.one, 0.2f).setEaseOutQuart();
        LeanTween.delayedCall(1.2f, (Action)delegate {
            LeanTween.scale(objPop, Vector3.zero, 0.25f);
        });
        yield return new WaitForSeconds(isStartRandom ? UnityEngine.Random.Range(2.95f, 3.05f) : 3f);
        arrayTextObj[1].SetActive(value: false);
        arrayTextObj[2].SetActive(value: true);
        objPop.transform.SetLocalScale(1f, 1f, 1f);
        _callBack?.Invoke();
        yield return new WaitForSeconds(1f);
        objPop.SetActive(value: false);
    }
}
