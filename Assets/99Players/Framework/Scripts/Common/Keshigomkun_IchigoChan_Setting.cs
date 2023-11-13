using UnityEngine;
public class Keshigomkun_IchigoChan_Setting : SingletonCustom<Keshigomkun_IchigoChan_Setting> {
    private bool isActive_KeshigomuKun;
    [SerializeField]
    [Header("消しゴムくん")]
    private GameObject objKeshigomuKun;
    [SerializeField]
    [Header("消しゴムくんの丸影設定")]
    private bool isCircleShadow_KeshigomuKun;
    [SerializeField]
    [Header("消しゴムくんの丸影")]
    private GameObject objKeshigomuKun_CircleShadow;
    private bool isActive_IchigoChan;
    [SerializeField]
    [Header("いちごちゃん")]
    private GameObject objIchigoChan;
    [SerializeField]
    [Header("いちごちゃんの丸影設定")]
    private bool isCircleShadow_IchigoChan;
    [SerializeField]
    [Header("いちごちゃんの丸影")]
    private GameObject objIchigoChan_CircleShadow;
    public GameObject ObjKeshigomuKun => objKeshigomuKun;
    public GameObject ObjIchigoChan => objIchigoChan;
    public bool IsActiveKeshigomuKun => isActive_KeshigomuKun;
    public bool IsActiveIchigoChan => isActive_IchigoChan;
    private void Awake() {
        isActive_KeshigomuKun = (Random.Range(0, 10) == 0);
        if (objKeshigomuKun != null) {
            if (isActive_KeshigomuKun) {
                objKeshigomuKun.SetActive(value: true);
                objKeshigomuKun_CircleShadow.SetActive(isCircleShadow_KeshigomuKun);
            } else {
                objKeshigomuKun.SetActive(value: false);
            }
        }
        isActive_IchigoChan = (Random.Range(0, 10) == 0);
        if (objIchigoChan != null) {
            objIchigoChan.SetActive(isActive_IchigoChan);
            if (isActive_IchigoChan) {
                objIchigoChan.SetActive(value: true);
                objIchigoChan_CircleShadow.SetActive(isCircleShadow_IchigoChan);
            } else {
                objIchigoChan.SetActive(value: false);
            }
        }
    }
}
