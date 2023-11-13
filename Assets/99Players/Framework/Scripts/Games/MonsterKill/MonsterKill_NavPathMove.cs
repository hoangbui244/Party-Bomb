using UnityEngine;
using UnityEngine.AI;
public class MonsterKill_NavPathMove : MonoBehaviour
{
	private NavMeshAgent agent;
	private bool isMovePathSetting;
	private Vector3[] movePath;
	private int movePathIdx;
	private Vector3 moveNextPathPos;
	private Vector3 moveVector;
	[SerializeField]
	[Header("自身が移動した距離で、移動先までのPathを更新するかどうかのフラグ")]
	private bool isUpdatePathMyMove = true;
	[SerializeField]
	[Header("自身の移動距離で移動先までのPathを更新する場合の移動距離")]
	private float updatePathMyMoveDistance = 0.1f;
	private Vector3 prevMyPos;
	[SerializeField]
	[Header("タ\u30fcゲットが移動した距離で、移動先までのPathを更新するかどうかのフラグ")]
	private bool isUpdatePathTargetMove = true;
	[SerializeField]
	[Header("タ\u30fcゲットの移動距離で移動先までのPathを更新する場合の移動距離")]
	private float updatePathTargetMoveDistance = 0.1f;
	private Vector3 prevTargetPos;
	public void Init(NavMeshAgent _agent)
	{
		agent = _agent;
		agent.updatePosition = false;
		agent.updateRotation = false;
	}
	public void SetUpdatePathMyMove(bool _isUpdatePathMyMove, float _updatePathMyMoveDistance)
	{
		isUpdatePathMyMove = _isUpdatePathMyMove;
		updatePathMyMoveDistance = _updatePathMyMoveDistance;
	}
	public void SetUpdatePathTargetMove(bool _isUpdatePathTargetMove, float _updatePathTargetMoveDistance)
	{
		isUpdatePathTargetMove = _isUpdatePathTargetMove;
		updatePathTargetMoveDistance = _updatePathTargetMoveDistance;
	}
	public void SetMoveDestination(Vector3 _targetPos)
	{
		if ((isUpdatePathMyMove && CalcManager.Length(prevMyPos, base.transform.position) >= updatePathMyMoveDistance) || (isUpdatePathTargetMove && CalcManager.Length(prevTargetPos, _targetPos) >= updatePathTargetMoveDistance))
		{
			isMovePathSetting = true;
			prevMyPos = base.transform.position;
			prevTargetPos = _targetPos;
			agent.ResetPath();
			agent.Warp(base.transform.position);
			agent.SetDestination(_targetPos);
		}
	}
	public void UpdateMethod(bool _isCanNextPathPos = true)
	{
		if (agent.hasPath && !agent.pathPending)
		{
			if (isMovePathSetting)
			{
				SetMovePath();
			}
			else if (GetIsMovePath() && _isCanNextPathPos)
			{
				SetMoveNextPathPos();
			}
		}
	}
	private void SetMovePath()
	{
		isMovePathSetting = false;
		movePathIdx = 1;
		movePath = agent.path.corners;
		moveNextPathPos = movePath[movePathIdx];
		moveVector = moveNextPathPos - base.transform.position;
	}
	private void SetMoveNextPathPos()
	{
		if (CalcManager.Length(moveNextPathPos, base.transform.position) <= 0.1f)
		{
			if (movePathIdx < movePath.Length - 1)
			{
				movePathIdx++;
				moveNextPathPos = movePath[movePathIdx];
				moveVector = moveNextPathPos - base.transform.position;
			}
			else
			{
				moveVector = Vector3.zero;
			}
		}
	}
	public Vector3 GetMoveVector()
	{
		return moveVector;
	}
	public bool GetIsMovePath()
	{
		return movePath != null;
	}
	public bool GetIsMoveComplete()
	{
		if (movePathIdx == movePath.Length - 1)
		{
			return CalcManager.Length(moveNextPathPos, base.transform.position) <= 0.1f;
		}
		return false;
	}
	public bool GetIsMovePathSetting()
	{
		return isMovePathSetting;
	}
	public void ResetPrevMyPos()
	{
		prevMyPos = Vector3.zero;
	}
	public void ResetPrevTaretPos()
	{
		prevTargetPos = Vector3.zero;
	}
	public void AgentResetPath()
	{
		agent.ResetPath();
	}
	public void AgentWarp(Vector3 _pos)
	{
		agent.Warp(_pos);
	}
	public void AgentIsStopped(bool _isStopped)
	{
		agent.isStopped = _isStopped;
	}
	private void OnDrawGizmos()
	{
		if (agent.hasPath && !agent.pathPending && movePath != null)
		{
			Gizmos.color = Color.white;
			for (int i = 0; i < agent.path.corners.Length - 1; i++)
			{
				Gizmos.DrawLine(agent.path.corners[i] + new Vector3(0f, 1f, 0f), agent.path.corners[i + 1] + new Vector3(0f, 1f, 0f));
			}
		}
	}
}
