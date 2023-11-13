using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.Serialization;
[ExecuteInEditMode]
public class CinemachinePathMove : MonoBehaviour {
    [Serializable]
    private struct MoveData {
        public float progress;
        public float speed;
        public float delay;
        public MoveData(float _progress, float _speed, float _delay) {
            progress = _progress;
            speed = _speed;
            delay = _delay;
        }
        public void Copy(float _progress, float _speed, float _rotSpeed, float _delay) {
            progress = _progress;
            speed = _speed;
            delay = _delay;
        }
        public void Copy(MoveData _data) {
            progress = _data.progress;
            speed = _data.speed;
            delay = _data.delay;
        }
    }
    [SerializeField]
    [Header("パス")]
    private CinemachinePathBase[] paths;
    [SerializeField]
    [Header("座標値タイプ")]
    private CinemachinePathBase.PositionUnits posUnitsType = CinemachinePathBase.PositionUnits.Normalized;
    [SerializeField]
    [FormerlySerializedAs("座標値")]
    private float posPer;
    [SerializeField]
    [Header("移動方向を見る")]
    private bool isLookMoveDir = true;
    [SerializeField]
    [Header("座標値タイプ")]
    private Rigidbody rigid;
    [SerializeField]
    [Header("Y軸以外も向き変更")]
    private bool isHorizontalRot;
    [SerializeField]
    [Header("非実行時テスト")]
    private bool isNonPlayTest;
    [SerializeField]
    [Header("移動情報")]
    private MoveData[] moveDatas = new MoveData[1]
    {
        new MoveData(0f, 1f, 0f)
    };
    private int moveDataNo;
    private MoveData moveData;
    private float progress;
    private bool isMove;
    private int pathNo;
    private CinemachinePathBase Path => paths[pathNo];
    public int PathNum => paths.Length;
    public bool IsLookMoveDir {
        set {
            isLookMoveDir = value;
        }
    }
    private Rigidbody Rigid => rigid;
    public float Progress => progress;
    private bool IsNonPlayTest {
        get {
            if (isNonPlayTest) {
                return !Application.isPlaying;
            }
            return false;
        }
    }
    public bool IsMove {
        get {
            return isMove;
        }
        set {
            isMove = value;
        }
    }
    public int PathNo {
        get {
            return pathNo;
        }
        set {
            pathNo = value;
        }
    }
    public void Start() {
        if (IsNonPlayTest) {
            Init();
        }
    }
    public void Init(Transform _pathParent = null, bool _isPlayOnStart = true) {
        for (int i = 0; i < paths.Length; i++) {
            paths[i].transform.parent = _pathParent;
        }
        isMove = _isPlayOnStart;
        Reset();
    }
    public void Active(bool _active = true) {
        isMove = _active;
    }
    public void Reset(int _pathNo = 0) {
        pathNo = _pathNo;
        posPer = 0f;
        moveDataNo = 0;
        UpdateMoveData(0f);
    }
    private void Update() {
        if (IsNonPlayTest) {
            SetCartPosition(Time.deltaTime);
        } else if (Application.isPlaying && IsMove && !Rigid) {
            SetCartPosition(Time.deltaTime);
        }
    }
    private void FixedUpdate() {
        if (IsMove && (bool)Rigid) {
            SetCartPosition(Time.fixedDeltaTime);
        }
    }
    private void SetCartPosition(float _deltaTime) {
        if (moveData.delay > 0f) {
            moveData.delay -= _deltaTime;
        } else {
            if (!(Path != null)) {
                return;
            }
            float pos = posPer + moveData.speed * _deltaTime;
            posPer = Path.StandardizeUnit(pos, posUnitsType);
            UpdateMoveData(posPer);
            if ((bool)Rigid && !IsNonPlayTest) {
                rigid.MovePosition(Path.EvaluatePositionAtUnit(posPer, posUnitsType));
            } else {
                base.transform.position = Path.EvaluatePositionAtUnit(posPer, posUnitsType);
            }
            if (!isLookMoveDir) {
                return;
            }
            Vector3 vector = Path.EvaluateTangentAtUnit(posPer, posUnitsType);
            if (isHorizontalRot) {
                vector.y = 0f;
            }
            if (vector == Vector3.zero) {
                if ((bool)Rigid && !IsNonPlayTest) {
                    Rigid.MoveRotation(Quaternion.identity);
                } else {
                    base.transform.rotation = Quaternion.identity;
                }
            } else if ((bool)Rigid && !IsNonPlayTest) {
                Rigid.MoveRotation(Quaternion.LookRotation(vector));
            } else {
                base.transform.rotation = Quaternion.LookRotation(vector);
            }
        }
    }
    private void UpdateMoveData(float _posPer) {
        UpdateProgress(_posPer);
        if (moveDatas.Length != 0 && moveDataNo < moveDatas.Length && Progress >= moveDatas[moveDataNo].progress) {
            moveData.Copy(moveDatas[moveDataNo]);
            moveDataNo++;
        }
    }
    private void UpdateProgress(float _posPer) {
        switch (posUnitsType) {
            case CinemachinePathBase.PositionUnits.Distance:
                progress = _posPer / Path.PathLength;
                break;
            case CinemachinePathBase.PositionUnits.PathUnits:
            case CinemachinePathBase.PositionUnits.Normalized:
                progress = _posPer;
                break;
        }
    }
    public Vector3 GetPredictPos(float _time) {
        return Path.EvaluatePositionAtUnit(posPer + moveData.speed * _time, posUnitsType);
    }
}
