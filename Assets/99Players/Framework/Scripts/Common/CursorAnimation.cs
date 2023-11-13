using UnityEngine;
public class CursorAnimation : MonoBehaviour {
    public enum ANIM_TYPE {
        NONE,
        CENTER_SCALE,
        CENTER_SCALE_MIN
    }
    [SerializeField]
    [Header("カ\u30fcソル画像")]
    private SpriteRenderer[] arrayCursorTex;
    [SerializeField]
    [Header("オ\u30fcバ\u30fcレイカ\u30fcソル画像")]
    private SpriteRenderer[] arrayCursorOverlayTex;
    [SerializeField]
    [Header("ベ\u30fcス画像オブジェクト")]
    private GameObject baseRoot;
    [SerializeField]
    [Header("オ\u30fcバ\u30fcレイ画像オブジェクト")]
    private GameObject overlayRoot;
    [SerializeField]
    [Header("アニメ\u30fcション")]
    private ANIM_TYPE animType;
    [SerializeField]
    [Header("オ\u30fcバ\u30fcレイオフセット")]
    private Vector2 overlayOffset;
    [SerializeField]
    [Header("キャラクタ\u30fcセレクト:プレイヤ\u30fc番号アイコン")]
    private GameObject objPlayerIcon;
    [SerializeField]
    [Header("点滅無効")]
    private bool isStopBlink;
    [SerializeField]
    [Header("zオフセット")]
    private float offsetZ;
    private Vector3[] ICON_POS = new Vector3[4]
    {
        new Vector3(-80f, 91f, -2f),
        new Vector3(80f, 91f, -2f),
        new Vector3(-80f, -91f, -2f),
        new Vector3(80f, -91f, -2f)
    };
    private float ANIM_CENTER_SCALE_DISTANCE = 10f;
    private float ANIM_CENTER_SCALE_DISTANCE_MIN = 2.5f;
    private float ANIM_CENTER_SCALE_TIME = 0.7f;
    private Rect rectSetting = new Rect(0f, 0f, 0f, 0f);
    private void OnEnable() {
        if (arrayCursorOverlayTex.Length == 0) {
            return;
        }
        LeanTween.cancel(base.gameObject);
        for (int i = 0; i < arrayCursorOverlayTex.Length; i++) {
            if (arrayCursorOverlayTex[i] != null && !isStopBlink) {
                arrayCursorOverlayTex[i].color = new Color(1f, 1f, 1f, 0f);
            }
        }
        LeanTween.value(base.gameObject, OnUpdateAnim, 0f, 0.7f, 0.75f).setLoopPingPong().setEaseOutQuad()
            .setIgnoreTimeScale(useUnScaledTime: true);
    }
    public void OnUpdateAnim(float _value) {
        if (isStopBlink) {
            return;
        }
        for (int i = 0; i < arrayCursorOverlayTex.Length; i++) {
            if (arrayCursorOverlayTex[i] != null) {
                arrayCursorOverlayTex[i].color = new Color(1f, 1f, 1f, _value);
            }
        }
    }
    public void SetPlayerIconPos(int _idx) {
        objPlayerIcon.transform.localPosition = ICON_POS[_idx];
    }
    public void SetDisable(bool _isDisp) {
        baseRoot.SetActive(_isDisp);
        overlayRoot.SetActive(_isDisp);
    }
    public void SetRectSetting(Rect _rect) {
        rectSetting = _rect;
        baseRoot.transform.SetLocalPositionX(_rect.x);
        baseRoot.transform.SetLocalPositionY(_rect.y);
        if (arrayCursorTex.Length != 0) {
            for (int i = 0; i < arrayCursorTex.Length; i++) {
                switch (i) {
                    case 0:
                        arrayCursorTex[i].transform.SetLocalPositionX((0f - _rect.width) / 2f);
                        arrayCursorTex[i].transform.SetLocalPositionY(_rect.height / 2f);
                        break;
                    case 1:
                        arrayCursorTex[i].transform.SetLocalPositionX((0f - _rect.width) / 2f);
                        arrayCursorTex[i].transform.SetLocalPositionY((0f - _rect.height) / 2f);
                        break;
                    case 2:
                        arrayCursorTex[i].transform.SetLocalPositionX(_rect.width / 2f);
                        arrayCursorTex[i].transform.SetLocalPositionY(_rect.height / 2f);
                        break;
                    case 3:
                        arrayCursorTex[i].transform.SetLocalPositionX(_rect.width / 2f);
                        arrayCursorTex[i].transform.SetLocalPositionY((0f - _rect.height) / 2f);
                        break;
                }
            }
        }
        if (overlayRoot != null && arrayCursorOverlayTex.Length != 0) {
            overlayRoot.transform.SetLocalPositionX(_rect.x);
            overlayRoot.transform.SetLocalPositionY(_rect.y);
            for (int j = 0; j < arrayCursorOverlayTex.Length; j++) {
                if (arrayCursorOverlayTex[j] != null) {
                    switch (j) {
                        case 0:
                            arrayCursorOverlayTex[j].transform.SetLocalPositionX((0f - _rect.width) / 2f - overlayOffset.x);
                            arrayCursorOverlayTex[j].transform.SetLocalPositionY(_rect.height / 2f - overlayOffset.y);
                            break;
                        case 1:
                            arrayCursorOverlayTex[j].transform.SetLocalPositionX((0f - _rect.width) / 2f - overlayOffset.x);
                            arrayCursorOverlayTex[j].transform.SetLocalPositionY((0f - _rect.height) / 2f + overlayOffset.x);
                            break;
                        case 2:
                            arrayCursorOverlayTex[j].transform.SetLocalPositionX(_rect.width / 2f + overlayOffset.x);
                            arrayCursorOverlayTex[j].transform.SetLocalPositionY(_rect.height / 2f - overlayOffset.y);
                            break;
                        case 3:
                            arrayCursorOverlayTex[j].transform.SetLocalPositionX(_rect.width / 2f + overlayOffset.x);
                            arrayCursorOverlayTex[j].transform.SetLocalPositionY((0f - _rect.height) / 2f + overlayOffset.y);
                            break;
                    }
                }
            }
        }
        AnimSetting();
    }
    private void AnimSetting() {
        switch (animType) {
            case ANIM_TYPE.NONE:
                break;
            case ANIM_TYPE.CENTER_SCALE:
                if (arrayCursorTex.Length == 0) {
                    break;
                }
                for (int k = 0; k < arrayCursorTex.Length; k++) {
                    LeanTween.cancel(arrayCursorTex[k].gameObject);
                    switch (k) {
                        case 0:
                            LeanTween.moveLocal(arrayCursorTex[k].gameObject, new Vector3(arrayCursorTex[k].transform.localPosition.x + ANIM_CENTER_SCALE_DISTANCE, arrayCursorTex[k].transform.localPosition.y - ANIM_CENTER_SCALE_DISTANCE, arrayCursorTex[k].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 1:
                            LeanTween.moveLocal(arrayCursorTex[k].gameObject, new Vector3(arrayCursorTex[k].transform.localPosition.x + ANIM_CENTER_SCALE_DISTANCE, arrayCursorTex[k].transform.localPosition.y + ANIM_CENTER_SCALE_DISTANCE, arrayCursorTex[k].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 2:
                            LeanTween.moveLocal(arrayCursorTex[k].gameObject, new Vector3(arrayCursorTex[k].transform.localPosition.x - ANIM_CENTER_SCALE_DISTANCE, arrayCursorTex[k].transform.localPosition.y - ANIM_CENTER_SCALE_DISTANCE, arrayCursorTex[k].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 3:
                            LeanTween.moveLocal(arrayCursorTex[k].gameObject, new Vector3(arrayCursorTex[k].transform.localPosition.x - ANIM_CENTER_SCALE_DISTANCE, arrayCursorTex[k].transform.localPosition.y + ANIM_CENTER_SCALE_DISTANCE, arrayCursorTex[k].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                    }
                }
                if (!(overlayRoot != null) || arrayCursorOverlayTex.Length == 0) {
                    break;
                }
                for (int l = 0; l < arrayCursorOverlayTex.Length; l++) {
                    LeanTween.cancel(arrayCursorOverlayTex[l].gameObject);
                    switch (l) {
                        case 0:
                            LeanTween.moveLocal(arrayCursorOverlayTex[l].gameObject, new Vector3(arrayCursorOverlayTex[l].transform.localPosition.x + ANIM_CENTER_SCALE_DISTANCE, arrayCursorOverlayTex[l].transform.localPosition.y - ANIM_CENTER_SCALE_DISTANCE, arrayCursorOverlayTex[l].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 1:
                            LeanTween.moveLocal(arrayCursorOverlayTex[l].gameObject, new Vector3(arrayCursorOverlayTex[l].transform.localPosition.x + ANIM_CENTER_SCALE_DISTANCE, arrayCursorOverlayTex[l].transform.localPosition.y + ANIM_CENTER_SCALE_DISTANCE, arrayCursorOverlayTex[l].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 2:
                            LeanTween.moveLocal(arrayCursorOverlayTex[l].gameObject, new Vector3(arrayCursorOverlayTex[l].transform.localPosition.x - ANIM_CENTER_SCALE_DISTANCE, arrayCursorOverlayTex[l].transform.localPosition.y - ANIM_CENTER_SCALE_DISTANCE, arrayCursorOverlayTex[l].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 3:
                            LeanTween.moveLocal(arrayCursorOverlayTex[l].gameObject, new Vector3(arrayCursorOverlayTex[l].transform.localPosition.x - ANIM_CENTER_SCALE_DISTANCE, arrayCursorOverlayTex[l].transform.localPosition.y + ANIM_CENTER_SCALE_DISTANCE, arrayCursorOverlayTex[l].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                    }
                }
                break;
            case ANIM_TYPE.CENTER_SCALE_MIN:
                if (arrayCursorTex.Length == 0) {
                    break;
                }
                for (int i = 0; i < arrayCursorTex.Length; i++) {
                    LeanTween.cancel(arrayCursorTex[i].gameObject);
                    switch (i) {
                        case 0:
                            LeanTween.moveLocal(arrayCursorTex[i].gameObject, new Vector3(arrayCursorTex[i].transform.localPosition.x + ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorTex[i].transform.localPosition.y - ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorTex[i].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 1:
                            LeanTween.moveLocal(arrayCursorTex[i].gameObject, new Vector3(arrayCursorTex[i].transform.localPosition.x + ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorTex[i].transform.localPosition.y + ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorTex[i].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 2:
                            LeanTween.moveLocal(arrayCursorTex[i].gameObject, new Vector3(arrayCursorTex[i].transform.localPosition.x - ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorTex[i].transform.localPosition.y - ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorTex[i].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 3:
                            LeanTween.moveLocal(arrayCursorTex[i].gameObject, new Vector3(arrayCursorTex[i].transform.localPosition.x - ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorTex[i].transform.localPosition.y + ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorTex[i].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                    }
                }
                if (!(overlayRoot != null) || arrayCursorOverlayTex.Length == 0) {
                    break;
                }
                for (int j = 0; j < arrayCursorOverlayTex.Length; j++) {
                    LeanTween.cancel(arrayCursorOverlayTex[j].gameObject);
                    switch (j) {
                        case 0:
                            LeanTween.moveLocal(arrayCursorOverlayTex[j].gameObject, new Vector3(arrayCursorOverlayTex[j].transform.localPosition.x + ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorOverlayTex[j].transform.localPosition.y - ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorOverlayTex[j].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 1:
                            LeanTween.moveLocal(arrayCursorOverlayTex[j].gameObject, new Vector3(arrayCursorOverlayTex[j].transform.localPosition.x + ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorOverlayTex[j].transform.localPosition.y + ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorOverlayTex[j].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 2:
                            LeanTween.moveLocal(arrayCursorOverlayTex[j].gameObject, new Vector3(arrayCursorOverlayTex[j].transform.localPosition.x - ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorOverlayTex[j].transform.localPosition.y - ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorOverlayTex[j].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                        case 3:
                            LeanTween.moveLocal(arrayCursorOverlayTex[j].gameObject, new Vector3(arrayCursorOverlayTex[j].transform.localPosition.x - ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorOverlayTex[j].transform.localPosition.y + ANIM_CENTER_SCALE_DISTANCE_MIN, arrayCursorOverlayTex[j].transform.localPosition.z + offsetZ), ANIM_CENTER_SCALE_TIME).setLoopPingPong().setEaseInOutQuad()
                                .setIgnoreTimeScale(useUnScaledTime: true);
                            break;
                    }
                }
                break;
        }
    }
}
