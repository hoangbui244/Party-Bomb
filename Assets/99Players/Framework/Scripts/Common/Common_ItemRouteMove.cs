using UnityEngine;
using UnityStandardAssets.Utility;
public class Common_ItemRouteMove : MonoBehaviour {
    [SerializeField]
    [Header("開始位置(画面外に配置)")]
    private Transform startPoint;
    [SerializeField]
    [Header("周回ル\u30fcト")]
    private WaypointCircuit loopRoute;
    [SerializeField]
    [Header("周回ル\u30fcトの開始位置")]
    private float loopRouteStartDistance;
    [SerializeField]
    [Header("開始位置から周回ル\u30fcトまでの速度")]
    private float straightMoveSpeed = 1f;
    [SerializeField]
    [Header("周回ル\u30fcトの速度")]
    private float loopRouteMoveSpeed = 1f;
    private bool isStraightMove;
    private float straightDistance;
    private float loopRouteDistance;
    private bool isMove;
    private void Awake() {
        PositionReset();
    }
    private void Update() {
        if (!isMove) {
            return;
        }
        if (isStraightMove) {
            straightDistance += straightMoveSpeed * Time.deltaTime;
            Vector3 routePosition = loopRoute.GetRoutePosition(loopRouteStartDistance);
            Vector3 vector = routePosition - startPoint.position;
            if (straightDistance * straightDistance < vector.sqrMagnitude) {
                base.transform.position = startPoint.position + vector.normalized * straightDistance;
                return;
            }
            isStraightMove = false;
            base.transform.position = routePosition;
        } else {
            loopRouteDistance += loopRouteMoveSpeed * Time.deltaTime;
            base.transform.position = loopRoute.GetRoutePosition(loopRouteDistance);
        }
    }
    public void MoveStart() {
        isMove = true;
    }
    public void MoveStop() {
        isMove = false;
    }
    public void PositionReset() {
        base.transform.position = startPoint.position;
        isStraightMove = true;
        straightDistance = 0f;
        loopRouteDistance = loopRouteStartDistance;
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPoint.transform.position, loopRoute.GetRoutePosition(loopRouteStartDistance));
    }
}
