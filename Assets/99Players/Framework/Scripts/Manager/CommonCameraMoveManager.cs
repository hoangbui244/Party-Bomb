using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public class CommonCameraMoveManager : SingletonCustom<CommonCameraMoveManager> {
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("カメラ移動速度")]
    private float CAMERA_SPEED = 0.8f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("カメラズ\u30fcム量(Zロ\u30fcカル座標用)")]
    private float CAMERA_ZOOM_MAX = 10f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("カメラX座標移動量(Xロ\u30fcカル座標用)")]
    private float CAMERA_X_MAX = 2.1f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("カメラY座標移動量(Yロ\u30fcカル座標用)")]
    private float CAMERA_Y_MAX = 0.6f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("カメラのX中心位置計算用")]
    private float CAMERA_CENTER_MAX_X = 2f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("カメラのY中心位置計算用")]
    private float CAMERA_CENTER_MAX_Y = 1f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("ズ\u30fcムが最小になる横幅")]
    private float ZOOM_MIN_WIDTH = 16f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("ズ\u30fcムが最大になる横幅")]
    private float ZOOM_MAX_WIDTH = 5f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("ズ\u30fcムが最小になる縦幅")]
    private float ZOOM_MIN_HEIGHT = 12f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("ズ\u30fcムが最大になる縦幅")]
    private float ZOOM_MAX_HEIGHT = 4f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("ズ\u30fcム時のY座標の調整用の数値")]
    private float ZOOM_Y_MAG = 0.18f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("画面の上部を映すときと下部を映すときのズ\u30fcム量の違いを調整する数値")]
    private float ZOOM_Z_MAG = 1.7f;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("centerAnchorの位置を内部的にずらすためのオフセット")]
    private Vector3 CENTER_ANCHOR_OFFSET = Vector3.zero;
    /// <summary>
    /// 
    /// </summary>
    [Header("カメラ(localPositionとlocalRotationを0にしてください)")]
    public Camera camera;
    /// <summary>
    /// 
    /// </summary>
    [Header("見る対象(キャラクタ\u30fc想定)")]
    public List<Transform> lookTargetAnchors = new List<Transform>();
    /// <summary>
    /// 
    /// </summary>
    [Header("3D上のセンタ\u30fc位置")]
    public Transform centerAnchor;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("Trueにしておくと最初からズ\u30fcムを開始します")]
    private bool isMove;
    /// <summary>
    /// 
    /// </summary>
    public void LateUpdate() {
        if (!isMove) {
            return;
        }
        List<Vector2> list = new List<Vector2>();
        for (int i = 0; i < lookTargetAnchors.Count; i++) {
            Vector3 position = lookTargetAnchors[i].position;
            Vector2 zero = Vector2.zero;
            Vector3 vector = centerAnchor.position + CENTER_ANCHOR_OFFSET;
            zero.x = position.x - vector.x;
            zero.y = vector.z - position.z;
            list.Add(zero);
        }
        float num = 100f;
        float num2 = -100f;
        float num3 = 100f;
        float num4 = -100f;
        for (int j = 0; j < list.Count; j++) {
            if (list[j].x < num) {
                num = list[j].x;
            }
            if (list[j].x > num2) {
                num2 = list[j].x;
            }
            if (list[j].y < num3) {
                num3 = list[j].y;
            }
            if (list[j].y > num4) {
                num4 = list[j].y;
            }
        }
        float value = num2 - num;
        float value2 = num4 - num3;
        Vector2 vector2 = new Vector2((num + num2) / 2f, (num3 + num4) / 2f);
        float num5 = Mathf.InverseLerp(ZOOM_MIN_WIDTH, ZOOM_MAX_WIDTH, value);
        float num6 = Mathf.Lerp(0f, CAMERA_ZOOM_MAX, num5);
        float num7 = Mathf.InverseLerp(ZOOM_MIN_HEIGHT, ZOOM_MAX_HEIGHT, value2);
        float num8 = Mathf.Lerp(0f, CAMERA_ZOOM_MAX, num7);
        Vector3 zero2 = Vector3.zero;
        if (num6 > num8) {
            zero2.x = vector2.x / CAMERA_CENTER_MAX_X * CAMERA_X_MAX * num7;
            zero2.y = vector2.y / CAMERA_CENTER_MAX_Y * CAMERA_Y_MAX * num7 + num8 * ZOOM_Y_MAG;
            zero2.z = num8 + zero2.y * ZOOM_Z_MAG;
        } else {
            zero2.x = vector2.x / CAMERA_CENTER_MAX_X * CAMERA_X_MAX * num5;
            zero2.y = vector2.y / CAMERA_CENTER_MAX_Y * CAMERA_Y_MAX * num5 + num6 * ZOOM_Y_MAG;
            zero2.z = num6 + zero2.y * ZOOM_Z_MAG;
        }
        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, zero2, Time.deltaTime * CAMERA_SPEED);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_anchors"></param>
    public void SetLookTargetAnchors(Transform[] _anchors) {
        lookTargetAnchors.Clear();
        lookTargetAnchors.AddRange(_anchors);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_anchor"></param>
    public void RemoveLookTargetAnchor(Transform _anchor) {
        lookTargetAnchors.Remove(_anchor);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_flg"></param>
    public void SetMoveFlag(bool _flg) {
        isMove = _flg;
    }
}
