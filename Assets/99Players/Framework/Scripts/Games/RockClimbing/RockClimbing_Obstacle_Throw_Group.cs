using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class RockClimbing_Obstacle_Throw_Group : MonoBehaviour {
    private RockClimbing_Player targetClimbingPlayer;
    [SerializeField]
    [Header("障害物を投げる数")]
    private int throwCnt;
    [SerializeField]
    [Header("障害物を投げるアンカ\u30fcクラス")]
    private RockClimbing_ThrowObstacleAnchor[] arrayThrowAnchor;
    private RockClimbing_CastleNiinja charaNiinja;
    private int beforeIdx;
    private float throwInterval;
    private bool isObstacleThrow;
    private bool isStopObstacleThrow;
    private float[] arrayNearThrowAnchorDistance;
    private List<int> nearThrowAnchorIdxList = new List<int>();
    private List<RockClimbing_Obstacle_Throw_Object> throwObjectList = new List<RockClimbing_Obstacle_Throw_Object>();
    private List<int> targetClimbingPlayerIdxList = new List<int>();
    public void Init() {
        throwInterval = 0f;
        beforeIdx = -1;
        arrayNearThrowAnchorDistance = new float[arrayThrowAnchor.Length];
        for (int i = 0; i < arrayThrowAnchor.Length; i++) {
            arrayThrowAnchor[i].Init();
        }
    }
    public void SetCharaNinja(RockClimbing_CastleNiinja _charaNinja) {
        charaNiinja = _charaNinja;
    }
    public void UpdateMethod(List<RockClimbing_Player> _climbingPlayerList) {
        if (isStopObstacleThrow) {
            return;
        }
        if (!isObstacleThrow) {
            targetClimbingPlayerIdxList.Clear();
            float y = _climbingPlayerList[0].GetHeadTop().position.y;
            targetClimbingPlayerIdxList.Add(0);
            for (int i = 1; i < _climbingPlayerList.Count; i++) {
                UnityEngine.Debug.Log("i : " + i.ToString());
                if (y < _climbingPlayerList[i].GetHeadTop().position.y) {
                    y = _climbingPlayerList[i].GetHeadTop().position.y;
                    targetClimbingPlayerIdxList.Clear();
                    targetClimbingPlayerIdxList.Add(i);
                } else if (y == _climbingPlayerList[i].GetHeadTop().position.y) {
                    targetClimbingPlayerIdxList.Add(i);
                }
            }
            int index = targetClimbingPlayerIdxList[UnityEngine.Random.Range(0, targetClimbingPlayerIdxList.Count)];
            targetClimbingPlayer = _climbingPlayerList[index];
            throwInterval -= Time.deltaTime;
            if (!(throwInterval <= 0f)) {
                return;
            }
            isObstacleThrow = true;
            UnityEngine.Debug.Log("インタ\u30fcバル終了");
            int idx = 0;
            if (arrayThrowAnchor.Length > 1) {
                SetNearThrowAnchorIdxList();
                idx = nearThrowAnchorIdxList[0];
            }
            beforeIdx = idx;
            bool flag = false;
            for (int j = 0; j < _climbingPlayerList.Count; j++) {
                if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(_climbingPlayerList[j].GetPlayerNo())) {
                    SingletonCustom<RockClimbing_UIManager>.Instance.SetObstacleCautionIconPos(_climbingPlayerList[j].GetPlayerNo(), SingletonCustom<RockClimbing_CameraManager>.Instance.GetCamera(_climbingPlayerList[j].GetPlayerNo()).GetCamera().WorldToScreenPoint(arrayThrowAnchor[idx].transform.position));
                    SingletonCustom<RockClimbing_UIManager>.Instance.ShowObstacleDropCautionIcon(_climbingPlayerList[j].GetPlayerNo());
                    if (!flag && !_climbingPlayerList[j].GetIsCpu()) {
                        flag = true;
                    }
                }
            }
            if (flag) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_iceclibling_obstacle_drop", _loop: false, 0f, 0.5f);
            }
            LeanTween.delayedCall(arrayThrowAnchor[idx].gameObject, 1.5f, (Action)delegate {
                Vector3 position = arrayThrowAnchor[idx].GetCharaAnchor(targetClimbingPlayer.transform.position).position;
                charaNiinja.SetMaterial(SingletonCustom<RockClimbing_CharacterManager>.Instance.GetCastleNinjaMaterial());
                charaNiinja.SetThrow(position, targetClimbingPlayer.transform.position);
                charaNiinja.gameObject.SetActive(value: true);
                arrayThrowAnchor[idx].OpenShoji(0.25f);
                LeanTween.delayedCall(arrayThrowAnchor[idx].gameObject, 0.25f, (Action)delegate {
                    charaNiinja.ReadyThrowAnimation(0.1f);
                    LeanTween.delayedCall(arrayThrowAnchor[idx].gameObject, 0.1f, (Action)delegate {
                        charaNiinja.ThrowAnimation(0.1f);
                        LeanTween.delayedCall(arrayThrowAnchor[idx].gameObject, 0.05f, (Action)delegate {
                            RockClimbing_Obstacle_Throw_Object rockClimbing_Obstacle_Throw_Object = UnityEngine.Object.Instantiate(SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleThrowPref(RockClimbing_ClimbingWallManager.ObstacleThrowType.Shuriken));
                            rockClimbing_Obstacle_Throw_Object.transform.parent = arrayThrowAnchor[idx].transform;
                            rockClimbing_Obstacle_Throw_Object.transform.localPosition = Vector3.zero;
                            rockClimbing_Obstacle_Throw_Object.Init(this);
                            rockClimbing_Obstacle_Throw_Object.SetCollisionIgnoreCollider(targetClimbingPlayer.GetClimbOnFoundation().GetWallCollider());
                            rockClimbing_Obstacle_Throw_Object.Throw(targetClimbingPlayer);
                            throwObjectList.Add(rockClimbing_Obstacle_Throw_Object);
                            ResetThrowStatus();
                            LeanTween.delayedCall(arrayThrowAnchor[idx].gameObject, 0.5f, (Action)delegate {
                                charaNiinja.ResetAnimation(0f);
                                charaNiinja.gameObject.SetActive(value: false);
                                arrayThrowAnchor[idx].CloseShoji(0.5f);
                            });
                        });
                    });
                });
            });
            return;
        }
        for (int k = 0; k < _climbingPlayerList.Count; k++) {
            if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(_climbingPlayerList[k].GetPlayerNo())) {
                SingletonCustom<RockClimbing_UIManager>.Instance.SetObstacleCautionIconPos(_climbingPlayerList[k].GetPlayerNo(), SingletonCustom<RockClimbing_CameraManager>.Instance.GetCamera(_climbingPlayerList[k].GetPlayerNo()).GetCamera().WorldToScreenPoint(arrayThrowAnchor[beforeIdx].transform.position));
            }
        }
    }
    private void SetNearThrowAnchorIdxList() {
        for (int i = 0; i < arrayThrowAnchor.Length; i++) {
            Vector3 position = arrayThrowAnchor[i].transform.position;
            position.z = targetClimbingPlayer.GetHeadTop().position.z;
            arrayNearThrowAnchorDistance[i] = CalcManager.Length(targetClimbingPlayer.GetHeadTop().position, position);
        }
        nearThrowAnchorIdxList.Clear();
        //??
        //nearThrowAnchorIdxList.AddRange(from v in (IEnumerable<(float, int)>)(from v in ((IEnumerable<float>)arrayNearThrowAnchorDistance).Select((Func<float, int, (float, int)>)((float distance, int idx) => (distance, idx)))
        //		where v.distance < SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleThrowNearAnchorCheckDistance()
        //		orderby v.distance
        //		select v)
        //	select v.idx);
    }
    public List<RockClimbing_Obstacle_Throw_Object> GetThrowObjectList() {
        return throwObjectList;
    }
    public void RemoveThrowObjectList(RockClimbing_Obstacle_Throw_Object _throwObject) {
        throwObjectList.Remove(_throwObject);
    }
    public bool GetIsObstacleThrow() {
        return isObstacleThrow;
    }
    public void ResetThrowStatus() {
        isObstacleThrow = false;
        throwInterval = SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleThrowInterval();
        targetClimbingPlayer = null;
    }
    public void StopThrowObstacle() {
        if (beforeIdx != -1) {
            LeanTween.cancel(arrayThrowAnchor[beforeIdx].gameObject);
            arrayThrowAnchor[beforeIdx].StopThrowObstacle();
            charaNiinja.gameObject.SetActive(value: false);
            if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(targetClimbingPlayer.GetPlayerNo())) {
                SingletonCustom<RockClimbing_UIManager>.Instance.StopObstacleDropCautionIcon(targetClimbingPlayer.GetPlayerNo());
            }
            beforeIdx = -1;
            ResetThrowStatus();
        }
    }
    private void OnDrawGizmos() {
        if (arrayThrowAnchor.Length != 0) {
            Gizmos.color = Color.blue;
            for (int i = 0; i < arrayThrowAnchor.Length; i++) {
                Gizmos.DrawWireSphere(arrayThrowAnchor[i].transform.position, 0.5f);
            }
        }
    }
}
