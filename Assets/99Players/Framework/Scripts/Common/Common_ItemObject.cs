using System;
using UnityEngine;
public class Common_ItemObject : MonoBehaviour {
    public enum Type {
        SpeedUp,
        Max
    }
    public const string TAG_ITEM = "Item";
    [SerializeField]
    [Header("アイテムの種類")]
    private Type itemType;
    [SerializeField]
    [Header("効果量(倍率)")]
    private float value;
    [SerializeField]
    [Header("効果時間")]
    private float duration;
    [SerializeField]
    [Header("アクティブを切り替える対象")]
    private GameObject activeAnchor;
    [SerializeField]
    [Header("スケ\u30fcルを変える対象")]
    private GameObject scaleAnchor;
    [SerializeField]
    [Header("破裂時のスケ\u30fcル")]
    private float breakScale = 1.5f;
    [SerializeField]
    [Header("破裂演出の時間")]
    private float breakTime = 0.2f;
    [SerializeField]
    [Header("Awake時に表示する")]
    private bool showOnAwake;
    [SerializeField]
    [Header("Y軸で回転させる")]
    private bool isRotateY;
    [SerializeField]
    [Header("回転スピ\u30fcド")]
    private float rotateSpeed = 180f;
    [SerializeField]
    [Header("移動ル\u30fcト(無ければnull良い)")]
    private Common_ItemRouteMove itemRouteMove;
    private bool isShow;
    private bool isGet;
    public Type ItemType => itemType;
    public float Value => value;
    public float Duration => duration;
    public bool IsShow => isShow;
    public bool IsGet => isGet;
    private void Awake() {
        if (!isShow) {
            if (showOnAwake) {
                Show();
            } else {
                Hide();
            }
        }
    }
    private void Update() {
        if (isRotateY) {
            base.transform.AddLocalEulerAnglesY(rotateSpeed * Time.deltaTime);
        }
    }
    public void Show() {
        activeAnchor.SetActive(value: true);
        scaleAnchor.transform.localScale = Vector3.one;
        isShow = true;
        isGet = false;
        if (itemRouteMove != null) {
            itemRouteMove.PositionReset();
            itemRouteMove.MoveStart();
        }
    }
    public void Hide() {
        activeAnchor.SetActive(value: false);
        isShow = false;
        if (itemRouteMove != null) {
            itemRouteMove.MoveStop();
        }
    }
    public void Get() {
        if (!isGet) {
            isGet = true;
            SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
            Break();
        }
    }
    public void Break() {
        LeanTween.scale(scaleAnchor, Vector3.one * breakScale, breakTime).setOnComplete((Action)delegate {
            Hide();
        });
    }
}
