using System;
using UnityEngine;
public class CommonGameCount : SingletonCustom<CommonGameCount> {
    [SerializeField]
    [Header("rootオブジェクト")]
    private GameObject rootObj;
    private float rootObjDefPosX;
    [SerializeField]
    [Header("カウント画像")]
    private SpriteRenderer[] spCount;
    [SerializeField]
    [Header("画像差分")]
    private Sprite[] arrayDiff;
    [SerializeField]
    [Header("画像差分（EN）")]
    private Sprite[] arrayDiff_EN;
    private void Awake() {
        rootObjDefPosX = rootObj.transform.localPosition.x;
        rootObj.transform.SetLocalPositionX(2000f);
    }
    public void Show(int _count, Action _callBack = null) {
        rootObj.transform.SetLocalPositionX(2000f);
        rootObj.SetActive(value: true);
        for (int i = 0; i < spCount.Length; i++) {
            if (i == _count) {
                spCount[i].gameObject.SetActive(value: true);
                if (Localize_Define.Language == Localize_Define.LanguageType.Japanese) {
                    spCount[i].sprite = arrayDiff[i];
                } else {
                    spCount[i].sprite = arrayDiff_EN[i];
                }
            } else {
                spCount[i].gameObject.SetActive(value: false);
            }
        }
        float defLocal = rootObj.transform.localPosition.x;
        LeanTween.moveLocalX(rootObj.transform.gameObject, rootObjDefPosX, 0.75f).setEaseOutBack();
        WaitAfterExec(2f, delegate {
            LeanTween.moveLocalX(rootObj.transform.gameObject, 0f - defLocal, 0.5f).setEaseOutQuad().setOnComplete((Action)delegate {
                if (_callBack != null) {
                    _callBack();
                }
                rootObj.SetActive(value: false);
            });
        });
    }
}
