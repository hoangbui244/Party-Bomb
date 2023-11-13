using System;
using UnityEngine;
public class MorphingRace_OperationFish_AI : MorphingRace_OperationCharacter_AI
{
	private MorphingRace_OperationFish fish;
	private Vector3[] path;
	public void Init(MorphingRace_OperationFish _fish)
	{
		fish = _fish;
		Init(fish.GetPlayer());
	}
	public override void UpdateMethod()
	{
		if (fish.CheckPassSwimArea())
		{
			fish.AddSwimAreaIdx();
			if (fish.CheckSwimAreaIdxLessCnt())
			{
				SetMoveTargetPos(_isDiff: true);
			}
			else
			{
				float randomTargetDiffRange = GetRandomTargetDiffRange();
				moveTargetPos = new Vector3(player.transform.position.x, player.transform.position.y + UnityEngine.Random.Range(0f - randomTargetDiffRange, randomTargetDiffRange), player.GetMorphingTargetFish().GetEndAnchorPos().z);
			}
		}
		if (!isHitObstacle && fish.CheckObstacle(base.transform.forward, 0.5f))
		{
			isHitObstacle = true;
			LeanTween.value(base.gameObject, GetRandomObstacleDecelerateMagnification(), 1f, 0.25f).setOnUpdate(delegate(float _value)
			{
				moveSpeedMag = _value;
			}).setOnComplete((Action)delegate
			{
				isHitObstacle = false;
			});
			if (fish.CheckSwimAreaIdxLessCnt())
			{
				SetMoveTargetPos(_isDiff: false);
			}
		}
		SetInput();
		SetMove();
		fish.Move();
	}
	protected override void SetMove()
	{
		Vector3 normalized = (moveTargetPos - player.transform.position).normalized;
		player.SetMoveDir(normalized);
	}
	protected override void MoveInput()
	{
		fish.MoveInput(moveSpeedMag);
	}
	public override void MorphingInit()
	{
		isHitObstacle = false;
		moveSpeedMag = 1f;
		path = player.GetMorphingTargetFish().GetSwimAreaPosList(player.GetPlayerNo());
		SetMoveTargetPos(_isDiff: true);
		SetMove();
	}
	private void SetMoveTargetPos(bool _isDiff)
	{
		moveTargetPos = path[fish.GetSwimAreaIdx()];
		if (_isDiff)
		{
			float randomTargetDiffRange = GetRandomTargetDiffRange();
			moveTargetPos += new Vector3(UnityEngine.Random.Range(0f - randomTargetDiffRange, randomTargetDiffRange), UnityEngine.Random.Range(0f - randomTargetDiffRange, randomTargetDiffRange), 0f);
		}
	}
	private float GetRandomTargetDiffRange()
	{
		return UnityEngine.Random.Range(MorphingRace_Define.CPU_FISH_TARGET_DIFF_RANGE[aiStrength] - 0.15f, MorphingRace_Define.CPU_FISH_TARGET_DIFF_RANGE[aiStrength] + 0.15f);
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		for (int i = 0; i < path.Length - 1; i++)
		{
			Gizmos.DrawLine(path[i] + new Vector3(0f, 0.1f, 0f), path[i + 1] + new Vector3(0f, 0.1f, 0f));
		}
	}
}
