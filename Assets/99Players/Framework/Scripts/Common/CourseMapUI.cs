using System;
using UnityEngine;
public class CourseMapUI : MonoBehaviour {
    [Serializable]
    public struct MapSpriteData {
        [Header("マップ画像")]
        public Sprite sprite;
        [Header("マップアンカ\u30fcのロ\u30fcカル位置")]
        public Vector2 localPos;
        [Header("マップ画像ロ\u30fcカル角度(Z軸)")]
        public float localEulerZ;
        [Header("マップ画像ロ\u30fcカルスケ\u30fcル")]
        public Vector3 localScale;
    }
    private const SAType ICON_SA_TYPE = SAType.Common;
    private const float ICON_SHIFT_POS_Z = 0.01f;
    private static readonly string[] ICON_PLAYER_SPRITE_NAMES = new string[4]
    {
        "race_map_mark_1p",
        "race_map_mark_2p",
        "race_map_mark_3p",
        "race_map_mark_4p"
    };
    private const string ICON_CPU_SPRITE_NAME = "race_map_mark_cpu";
    [SerializeField]
    [Header("プレイヤ\u30fc番号")]
    private int playerNo;
    [SerializeField]
    [Header("マップアイコンを開始時に自動調整")]
    private bool isAutoIconSetting;
    [SerializeField]
    [Header("マップ画像デ\u30fcタ")]
    private MapSpriteData[] mapSpriteData;
    [SerializeField]
    [Header("複製するマップ")]
    private CourseMapUI[] copyMaps;
    [SerializeField]
    [Header("マップにアイコンとして映す対象のアンカ\u30fc")]
    private Transform[] worldTargetAnchors;
    [SerializeField]
    [Header("コ\u30fcス上の右上と左下の両端アンカ\u30fc")]
    private Transform worldRightTop;
    [SerializeField]
    private Transform worldLeftBottom;
    [Space(30f)]
    [SerializeField]
    [Header("マップアンカ\u30fc")]
    private GameObject mapAnchor;
    [SerializeField]
    [Header("マップ上のアイコンアンカ\u30fc")]
    private Transform[] mapIconAnchors;
    [SerializeField]
    [Header("マップのコ\u30fcス画像")]
    private SpriteRenderer mapCourseSprite;
    [SerializeField]
    [Header("マップ上の右上と左下の両端アンカ\u30fc")]
    private Transform mapRightTop;
    [SerializeField]
    private Transform mapLeftBottom;
    private int targetNum;
    public void Init() {
        targetNum = worldTargetAnchors.Length;
        if (targetNum > 0) {
            for (int i = 0; i < mapIconAnchors.Length; i++) {
                mapIconAnchors[i].gameObject.SetActive(i < targetNum);
            }
        }
        if (mapSpriteData.Length == 1) {
            SetMapData(0);
        }
        if (isAutoIconSetting) {
            AutoIconSetting();
        }
        for (int j = 0; j < copyMaps.Length; j++) {
            copyMaps[j].Init();
        }
    }
    public void UpdateMethod() {
        if (!mapAnchor.activeSelf) {
            return;
        }
        for (int i = 0; i < targetNum; i++) {
            MapView(mapIconAnchors[i], Stage2Map01Vec2(worldTargetAnchors[i].position));
            for (int j = 0; j < copyMaps.Length; j++) {
                Vector3 localPosition = mapIconAnchors[i].localPosition;
                localPosition.z = copyMaps[j].mapIconAnchors[i].localPosition.z;
                copyMaps[j].mapIconAnchors[i].localPosition = localPosition;
            }
        }
    }
    private void AutoIconSetting() {
        int playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
        for (int i = 0; i < mapIconAnchors.Length; i++) {
            if (i == playerNo) {
                mapIconAnchors[i].SetLocalPositionZ(0f);
            } else {
                mapIconAnchors[i].SetLocalPositionZ((float)(i + 1) * 0.01f);
            }
            if (i < playerNum) {
                mapIconAnchors[i].GetComponent<SpriteRenderer>().sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, ICON_PLAYER_SPRITE_NAMES[i]);
            } else {
                mapIconAnchors[i].GetComponent<SpriteRenderer>().sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "race_map_mark_cpu");
            }
        }
    }
    private Vector2 Stage2Map01Vec2(Vector3 _stagePos) {
        Vector2 zero = Vector2.zero;
        zero.x = Mathf.InverseLerp(worldLeftBottom.position.x, worldRightTop.position.x, _stagePos.x);
        zero.y = Mathf.InverseLerp(worldLeftBottom.position.z, worldRightTop.position.z, _stagePos.z);
        return zero;
    }
    private void MapView(Transform _trans, Vector2 _xy01) {
        Vector3 localPosition = mapLeftBottom.localPosition;
        Vector3 vector = mapRightTop.localPosition - mapLeftBottom.localPosition;
        localPosition.x += vector.x * _xy01.x;
        localPosition.y += vector.y * _xy01.y;
        localPosition.z = _trans.localPosition.z;
        _trans.localPosition = localPosition;
    }
    public void SetMapActive(bool _active) {
        mapAnchor.SetActive(_active);
        for (int i = 0; i < copyMaps.Length; i++) {
            copyMaps[i].mapAnchor.SetActive(_active);
        }
    }
    public void SetWorldTargetAnchor(Transform[] _targets) {
        worldTargetAnchors = _targets;
        targetNum = worldTargetAnchors.Length;
        if (targetNum > 0) {
            for (int i = 0; i < mapIconAnchors.Length; i++) {
                mapIconAnchors[i].gameObject.SetActive(i < targetNum);
            }
        }
        for (int j = 0; j < copyMaps.Length; j++) {
            copyMaps[j].SetWorldTargetAnchor(_targets);
        }
    }
    public void SetWorldRangeAnchor(Transform _rightTop, Transform _leftBottom) {
        worldRightTop = _rightTop;
        worldLeftBottom = _leftBottom;
        for (int i = 0; i < copyMaps.Length; i++) {
            copyMaps[i].SetWorldRangeAnchor(_rightTop, _leftBottom);
        }
    }
    public void SetMapData(int _courseNo) {
        SetMapSprite(mapSpriteData[_courseNo].sprite);
        SetMapAnchorLocalPos(mapSpriteData[_courseNo].localPos);
        SetMapIconAnchorParentLocalEulerZ(mapSpriteData[_courseNo].localEulerZ);
        SetMapSpriteScale(mapSpriteData[_courseNo].localScale);
        for (int i = 0; i < copyMaps.Length; i++) {
            copyMaps[i].SetMapSprite(mapSpriteData[_courseNo].sprite);
            copyMaps[i].SetMapAnchorLocalPos(mapSpriteData[_courseNo].localPos);
            copyMaps[i].SetMapIconAnchorParentLocalEulerZ(mapSpriteData[_courseNo].localEulerZ);
            copyMaps[i].SetMapSpriteScale(mapSpriteData[_courseNo].localScale);
        }
    }
    private void SetMapSprite(Sprite _sprite) {
        mapCourseSprite.sprite = _sprite;
        for (int i = 0; i < copyMaps.Length; i++) {
            copyMaps[i].SetMapSprite(_sprite);
        }
    }
    private void SetMapAnchorLocalPos(Vector2 _pos) {
        mapAnchor.transform.SetLocalPositionX(_pos.x);
        mapAnchor.transform.SetLocalPositionY(_pos.y);
        for (int i = 0; i < copyMaps.Length; i++) {
            copyMaps[i].SetMapAnchorLocalPos(_pos);
        }
    }
    private void SetMapIconAnchorParentLocalEulerZ(float _eulerZ) {
        mapIconAnchors[0].parent.SetLocalEulerAnglesZ(_eulerZ);
        for (int i = 0; i < copyMaps.Length; i++) {
            copyMaps[i].SetMapIconAnchorParentLocalEulerZ(_eulerZ);
        }
    }
    private void SetMapSpriteScale(Vector3 _scale) {
        mapCourseSprite.transform.localScale = _scale;
        for (int i = 0; i < copyMaps.Length; i++) {
            copyMaps[i].SetMapSpriteScale(_scale);
        }
    }
}
