using UnityEngine;
public class MorphingRace_OperationDog_AI : MorphingRace_OperationCharacter_AI
{
	private MorphingRace_OperationDog dog;
	private int beforeJumpAreaIdx;
	private bool isWhetherJump;
	private float waitJumpTime;
	public void Init(MorphingRace_OperationDog _dog)
	{
		dog = _dog;
		Init(dog.GetPlayer());
	}
	public override void UpdateMethod()
	{
		SetInput();
		if (beforeJumpAreaIdx != dog.GetJumpAreaIdx())
		{
			beforeJumpAreaIdx = dog.GetJumpAreaIdx();
			if (dog.CheckJumpAreaIdxLessCnt())
			{
				SetMoveTargetPos();
				SetWhetherJump();
			}
			else
			{
				moveTargetPos = new Vector3(player.transform.position.x, player.transform.position.y, player.GetMorphingTargetDog().GetEndAnchorPos().z);
			}
		}
		if (!dog.CheckIsGround())
		{
			return;
		}
		if (dog.GetIsJumpping())
		{
			dog.SetIsJumpping(_isJumpping: false);
		}
		if (dog.CheckIsCanJump())
		{
			if (isWhetherJump)
			{
				waitJumpTime -= Time.deltaTime;
				if (waitJumpTime < 0f)
				{
					isWhetherJump = false;
					dog.Jump();
					return;
				}
			}
			else if (dog.CheckObstacle(base.transform.forward, 0.5f))
			{
				isWhetherJump = true;
				waitJumpTime = GetRandomObstacleWaitJumpTime();
				return;
			}
		}
		SetMove();
		dog.Move();
	}
	protected override void SetMove()
	{
		Vector3 normalized = (moveTargetPos - player.transform.position).normalized;
		player.SetMoveDir(normalized);
	}
	protected override void MoveInput()
	{
		dog.MoveInput();
	}
	public override void MorphingInit()
	{
		beforeJumpAreaIdx = dog.GetJumpAreaIdx();
		SetMoveTargetPos();
		SetWhetherJump();
	}
	private void SetMoveTargetPos()
	{
		moveTargetPos = player.GetMorphingTargetDog().GetJumpAnchorPos(player.GetPlayerNo(), beforeJumpAreaIdx);
		moveTargetPos.x = player.transform.position.x;
	}
	private void SetWhetherJump()
	{
		if (Random.Range(0f, 1f) < GetRandomWhetherJumpProbability())
		{
			isWhetherJump = true;
		}
		else
		{
			isWhetherJump = false;
		}
	}
	private float GetRandomWhetherJumpProbability()
	{
		return Random.Range(MorphingRace_Define.CPU_DOG_WHETHER_JUMP_PROBABILITY[aiStrength] - 0.2f, MorphingRace_Define.CPU_DOG_WHETHER_JUMP_PROBABILITY[aiStrength] + 0.2f);
	}
	private float GetRandomObstacleWaitJumpTime()
	{
		return Random.Range(MorphingRace_Define.CPU_DOG_OBSTACLE_WAIT_JUMP_TIME[aiStrength] - 0.15f, MorphingRace_Define.CPU_DOG_OBSTACLE_WAIT_JUMP_TIME[aiStrength] + 0.15f);
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
