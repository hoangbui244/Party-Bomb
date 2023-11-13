using System;
using UnityEngine;
public class MorphingRace_OperationEagle_AI : MorphingRace_OperationCharacter_AI
{
	private MorphingRace_OperationEagle eagle;
	public void Init(MorphingRace_OperationEagle _eagle)
	{
		eagle = _eagle;
		Init(eagle.GetPlayer());
	}
	public override void UpdateMethod()
	{
		if (eagle.CheckPassThroughArea())
		{
			eagle.AddThroughAreaIdx();
			if (eagle.CheckThroughAreaIdxLessCnt())
			{
				SetMoveTargetPos(_isDiff: true);
			}
			else
			{
				float randomTargetDiffRange = GetRandomTargetDiffRange();
				moveTargetPos = new Vector3(player.transform.position.x, player.transform.position.y + UnityEngine.Random.Range(0f - randomTargetDiffRange, randomTargetDiffRange), player.GetMorphingTargetEagle().GetEndAnchorPos().z);
			}
		}
		if (!isHitObstacle && eagle.CheckObstacle(base.transform.forward, 0.5f))
		{
			isHitObstacle = true;
			LeanTween.value(base.gameObject, GetRandomObstacleDecelerateMagnification(), 1f, 0.25f).setOnUpdate(delegate(float _value)
			{
				moveSpeedMag = _value;
			}).setOnComplete((Action)delegate
			{
				isHitObstacle = false;
			});
			if (eagle.CheckThroughAreaIdxLessCnt())
			{
				SetMoveTargetPos(_isDiff: false);
			}
		}
		SetInput();
		SetMove();
		eagle.Move();
	}
	protected override void SetMove()
	{
		Vector3 normalized = (moveTargetPos - player.transform.position).normalized;
		player.SetMoveDir(normalized);
	}
	protected override void MoveInput()
	{
		eagle.MoveInput(moveSpeedMag);
	}
	public override void MorphingInit()
	{
		isHitObstacle = false;
		moveSpeedMag = 1f;
		SetMoveTargetPos(_isDiff: true);
	}
	private void SetMoveTargetPos(bool _isDiff)
	{
		moveTargetPos = player.GetMorphingTargetEagle().GetThroughAreaPos(player.GetPlayerNo(), eagle.GetThroughAreaIdx());
		if (_isDiff)
		{
			float randomTargetDiffRange = GetRandomTargetDiffRange();
			moveTargetPos += new Vector3(UnityEngine.Random.Range(0f - randomTargetDiffRange, randomTargetDiffRange), UnityEngine.Random.Range(0f - randomTargetDiffRange, randomTargetDiffRange), 0f);
		}
	}
	private float GetRandomTargetDiffRange()
	{
		return UnityEngine.Random.Range(MorphingRace_Define.CPU_EAGLE_TARGET_DIFF_RANGE[aiStrength] - 0.25f, MorphingRace_Define.CPU_EAGLE_TARGET_DIFF_RANGE[aiStrength] + 0.25f);
	}
	private void OnDrawGizmos()
	{
		switch (player.GetPlayerNo())
		{
		case 0:
			Gizmos.color = Color.green;
			break;
		case 1:
			Gizmos.color = Color.red;
			break;
		case 2:
			Gizmos.color = Color.blue;
			break;
		case 3:
			Gizmos.color = Color.yellow;
			break;
		}
		Gizmos.DrawWireSphere(moveTargetPos, 0.5f);
	}
}
