using UnityEngine;
using UnityEngine.AI;
public class Common_NavMovePath : MonoBehaviour {
    private NavMeshAgent agent;
    private bool isImmediatelySetMovePath;
    private Vector3[] movePath;
    private int pathIdx;
    private Vector3 nextPos;
    private Vector3 moveVector;
    [SerializeField]
    [Header("Pathの更新フラグ（自身の移動距離）")]
    private bool isUpdatePath_MyMove = true;
    [SerializeField]
    [Header("Pathを更新する移動距離（自身の移動距離）")]
    private float updatePathDistance_MyMove = 0.1f;
    private Vector3 prevMyPos;
    [SerializeField]
    [Header("Pathの更新フラグ（タ\u30fcゲットの移動距離）")]
    private bool isUpdatePath_TargetMove = true;
    [SerializeField]
    [Header("Pathを更新する移動距離（タ\u30fcゲットの移動距離）")]
    private float updatePathDistance_TargetMove = 0.1f;
    private Vector3 prevTargetPos;
    private bool isNextMovePos;
    public void Init() {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        isNextMovePos = true;
    }
    public void SetUpdatePathMyMove(bool _isUpdatePath, float _updatePathDistance) {
        isUpdatePath_MyMove = _isUpdatePath;
        updatePathDistance_MyMove = _updatePathDistance;
    }
    public void SetUpdatePathTargetMove(bool _isUpdatePath, float _updatePathDistance) {
        isUpdatePath_TargetMove = _isUpdatePath;
        updatePathDistance_TargetMove = _updatePathDistance;
    }
    public void SetMoveDestination(Vector3 _targetPos) {
        if ((isUpdatePath_MyMove && CalcManager.Length(prevMyPos, base.transform.position) >= updatePathDistance_MyMove) || (isUpdatePath_TargetMove && CalcManager.Length(prevTargetPos, _targetPos) >= updatePathDistance_TargetMove)) {
            isImmediatelySetMovePath = true;
            prevMyPos = base.transform.position;
            prevTargetPos = _targetPos;
            agent.ResetPath();
            agent.Warp(base.transform.position);
            agent.SetDestination(_targetPos);
        }
    }
    public void UpdateMethod() {
        if (agent.hasPath && !agent.pathPending) {
            if (isImmediatelySetMovePath) {
                SetMovePath();
            } else if (isNextMovePos && GetIsMovePath()) {
                SetNextMovePos();
            }
        }
    }
    private void SetMovePath() {
        isImmediatelySetMovePath = false;
        pathIdx = 0;
        movePath = agent.path.corners;
        do {
            pathIdx++;
            nextPos = movePath[pathIdx];
        }
        while (GetIsMoveCompleteDistance() && pathIdx < movePath.Length - 1);
        moveVector = ((pathIdx == movePath.Length) ? Vector3.zero : (nextPos - base.transform.position));
    }
    private void SetNextMovePos() {
        if (GetIsMoveCompleteDistance()) {
            if (pathIdx < movePath.Length - 1) {
                pathIdx++;
                nextPos = movePath[pathIdx];
                moveVector = nextPos - base.transform.position;
            } else {
                moveVector = Vector3.zero;
            }
        }
    }
    public Vector3 GetMoveVector() {
        return moveVector;
    }
    public bool GetIsMovePath() {
        return movePath != null;
    }
    public bool GetLastMovePathIdx() {
        return pathIdx == movePath.Length - 1;
    }
    public float GetNextPosDistance() {
        return CalcManager.Length(nextPos, base.transform.position);
    }
    public bool GetIsMoveCompleteDistance() {
        return GetNextPosDistance() <= 0.1f;
    }
    public bool GetIsMoveComplete() {
        if (GetLastMovePathIdx()) {
            return GetIsMoveCompleteDistance();
        }
        return false;
    }
    public bool GetIsMovePathSetting() {
        return isImmediatelySetMovePath;
    }
    public void ResetPrevMyPos() {
        prevMyPos = Vector3.zero;
    }
    public void ResetPrevTargetPos() {
        prevTargetPos = Vector3.zero;
    }
    public void AgentResetPath() {
        agent.ResetPath();
        movePath = null;
        moveVector = Vector3.zero;
    }
    public void AgentWarp(Vector3 _pos) {
        agent.Warp(_pos);
    }
    public void AgentIsStopped(bool _isStopped) {
        agent.isStopped = _isStopped;
    }
    public void SetIsNextMovePos(bool _isNextMovePos) {
        isNextMovePos = _isNextMovePos;
    }
    public bool IsHasPath() {
        return agent.hasPath;
    }
    public bool IsPathPending() {
        return agent.pathPending;
    }
    private void OnDrawGizmos() {
        if (agent != null && agent.hasPath && !agent.pathPending && movePath != null) {
            Gizmos.color = Color.white;
            for (int i = 0; i < agent.path.corners.Length - 1; i++) {
                Gizmos.DrawLine(agent.path.corners[i] + new Vector3(0f, 1f, 0f), agent.path.corners[i + 1] + new Vector3(0f, 1f, 0f));
            }
        }
    }
}
