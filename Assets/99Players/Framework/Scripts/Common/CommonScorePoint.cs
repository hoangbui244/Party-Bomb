using System;
using System.Collections.Generic;
using UnityEngine;
public class CommonScorePoint : MonoBehaviour {
    public enum Type {
        PLUS_10,
        PLUS_20,
        PLUS_30,
        PLUS_40,
        PLUS_50,
        PLUS_60,
        PLUS_70,
        PLUS_80,
        PLUS_90,
        PLUS_100,
        PLUS_150,
        PLUS_200,
        PLUS_300,
        PLUS_400,
        PLUS_500,
        PLUS_700,
        PLUS_1000,
        MINUS_10,
        MINUS_20,
        MINUS_30,
        MINUS_40,
        MINUS_50,
        MINUS_100,
        MINUS_200,
        MINUS_300,
        MINUS_500
    }
    [SerializeField]
    [Header("スコアオブジェクト")]
    private GameObject[] scoreObjects;
    [SerializeField]
    [Header("手前のスコアオブジェクト")]
    private GameObject[] scoreFrontObjects;
    private List<SpriteRenderer> rendererList = new List<SpriteRenderer>();
    private List<SpriteRenderer> frontRendererList = new List<SpriteRenderer>();
    public void Init(int _playerNo, Type _type, float _size, float _time) {
        int num = 0;
        for (int i = 0; i < scoreObjects.Length; i++) {
            scoreObjects[i].SetActive(i == (int)_type);
            if (i == (int)_type) {
                num = i;
            }
        }
        foreach (Transform item in scoreObjects[num].transform) {
            SpriteRenderer component = item.GetComponent<SpriteRenderer>();
            if (component != null && component.gameObject.activeSelf) {
                rendererList.Add(component);
            }
        }
        foreach (Transform item2 in scoreFrontObjects[num].transform) {
            SpriteRenderer component2 = item2.GetComponent<SpriteRenderer>();
            if (component2 != null && component2.gameObject.activeSelf) {
                frontRendererList.Add(component2);
                switch (_playerNo) {
                    case 0:
                        frontRendererList[frontRendererList.Count - 1].color = new Color32(61, 182, 16, byte.MaxValue);
                        break;
                    case 1:
                        frontRendererList[frontRendererList.Count - 1].color = new Color32(237, 50, 82, byte.MaxValue);
                        break;
                    case 2:
                        frontRendererList[frontRendererList.Count - 1].color = new Color32(43, 124, 241, byte.MaxValue);
                        break;
                    case 3:
                        frontRendererList[frontRendererList.Count - 1].color = new Color32(byte.MaxValue, 202, 40, byte.MaxValue);
                        break;
                    case 4:
                        frontRendererList[frontRendererList.Count - 1].color = new Color32(157, 67, 253, byte.MaxValue);
                        break;
                    case 5:
                        frontRendererList[frontRendererList.Count - 1].color = new Color32(163, 164, 163, byte.MaxValue);
                        break;
                }
            }
        }
        base.transform.SetLocalScale(0f, 0f, 1f);
        LeanTween.scale(base.gameObject, new Vector3(_size, _size, _size), 0.35f).setEaseOutBack();
        LeanTween.moveLocalY(base.gameObject, base.transform.localPosition.y + 10f, _time).setDelay(0.5f);
        LeanTween.value(base.gameObject, 1f, 0f, 0.5f).setOnUpdate(delegate (float _value) {
            for (int j = 0; j < rendererList.Count; j++) {
                rendererList[j].SetAlpha(_value);
                frontRendererList[j].SetAlpha(_value);
            }
        }).setDelay(_time - 0.5f)
            .setOnComplete((Action)delegate {
                if ((bool)this) {
                    UnityEngine.Object.Destroy(base.gameObject);
                }
            });
    }
    private void OnDestroy() {
        LeanTween.cancel(base.gameObject);
    }
}
