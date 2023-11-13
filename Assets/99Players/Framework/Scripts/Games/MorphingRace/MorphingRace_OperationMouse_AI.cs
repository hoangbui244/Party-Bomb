using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class MorphingRace_OperationMouse_AI : MorphingRace_OperationCharacter_AI
{
	private MorphingRace_OperationMouse mouse;
	private NavMeshAgent agent;
	private int pathIdx;
	private Vector3[] path;
	private bool isPathSetting;
	private float beforeDistance;
	public void Init(MorphingRace_OperationMouse _mouse)
	{
		mouse = _mouse;
		agent = mouse.GetNavMeshAgent();
		agent.updatePosition = false;
		agent.updateRotation = false;
		Init(mouse.GetPlayer());
	}
	public override void UpdateMethod()
	{
		if (!isPathSetting)
		{
			return;
		}
		if (pathIdx < path.Length)
		{
			float num = CalcManager.Length(player.transform.position, moveTargetPos);
			UnityEngine.Debug.Log("playerNo : " + player.GetPlayerNo().ToString() + " pathIdx : " + pathIdx.ToString() + " before beforeDistance : " + beforeDistance.ToString() + " currentDistance " + num.ToString());
			if (beforeDistance < num)
			{
				pathIdx++;
				if (pathIdx < path.Length)
				{
					SetMoveTargetPos();
					SetMove();
				}
			}
		}
		if (!isHitObstacle && mouse.CheckObstacle(base.transform.forward, 0.5f))
		{
			isHitObstacle = true;
			LeanTween.value(base.gameObject, GetRandomObstacleDecelerateMagnification(), 1f, 0.25f).setOnUpdate(delegate(float _value)
			{
				moveSpeedMag = _value;
			}).setOnComplete((Action)delegate
			{
				isHitObstacle = false;
			});
		}
		SetInput();
		mouse.Move();
		beforeDistance = CalcManager.Length(player.transform.position, moveTargetPos);
	}
	protected override void SetMove()
	{
		Vector3 moveDir = (pathIdx != 0) ? (moveTargetPos - player.transform.position).normalized : base.transform.forward;
		player.SetMoveDir(moveDir);
	}
	protected override void MoveInput()
	{
		mouse.MoveInput(moveSpeedMag * 0.8f);
	}
	public override void MorphingInit()
	{
		isHitObstacle = false;
		moveSpeedMag = 1f;
		isPathSetting = false;
		agent.enabled = true;
		Vector3 destination = new Vector3(player.GetMorphingTarget_Mouse().GetObstacleAnchorPos(player.GetPlayerNo()).x + UnityEngine.Random.Range(-1f, 1f), player.GetMorphingTarget_Mouse().GetEndAnchorPos().y, player.GetMorphingTarget_Mouse().GetEndAnchorPos().z);
		agent.SetDestination(destination);
		pathIdx = 0;
		StartCoroutine(WaitPathSetting());
	}
	private IEnumerator WaitPathSetting()
	{
		yield return new WaitWhile(() => agent.pathPending);
		isPathSetting = true;
		path = agent.path.corners;
		SetMoveTargetPos();
		SetMove();
		beforeDistance = CalcManager.Length(player.transform.position, moveTargetPos);
		agent.enabled = false;
	}
	private void SetMoveTargetPos()
	{
		moveTargetPos = path[pathIdx];
	}
	private void OnDrawGizmos()
	{
		if (isPathSetting)
		{
			Gizmos.color = Color.white;
			for (int i = 0; i < path.Length - 1; i++)
			{
				Gizmos.DrawLine(path[i] + new Vector3(0f, 0.1f, 0f), path[i + 1] + new Vector3(0f, 0.1f, 0f));
			}
		}
	}
}
