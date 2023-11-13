using System;
using UnityEngine;
public class MorphingRace_OperationEagle : MorphingRace_OperationCharacter
{
	private Animator animator;
	private int throughAreaIdx;
	private int throughAreaCnt;
	private MorphingRace_OperationEagle_AI cpuAI;
	public override void Init(MorphingRace_Player _player)
	{
		base.Init(_player);
		animator = GetComponent<Animator>();
		if (player.GetIsCpu())
		{
			cpuAI = base.gameObject.AddComponent<MorphingRace_OperationEagle_AI>();
			cpuAI.Init(this);
		}
	}
	public override void UpdateMethod_Base()
	{
		base.UpdateMethod_Base();
		player.SetInputInterval();
		if (!player.GetIsCpu())
		{
			UpdateMethod();
		}
		else
		{
			cpuAI.UpdateMethod();
		}
	}
	protected override void UpdateMethod()
	{
		if (CheckPassThroughArea())
		{
			AddThroughAreaIdx();
		}
		if (player.GetButtonDown_A())
		{
			MoveInput();
		}
		else
		{
			MoveNoneInput();
		}
		player.SetMove();
		Move();
	}
	public bool CheckThroughAreaIdxLessCnt()
	{
		return throughAreaIdx < throughAreaCnt;
	}
	public bool CheckPassThroughArea()
	{
		if (CheckThroughAreaIdxLessCnt() && player.GetMorphingTargetEagle().CheckPassThroughArea(player.GetPlayerNo(), throughAreaIdx, player.transform.position))
		{
			return true;
		}
		return false;
	}
	public override void Move()
	{
		Vector3 moveDir = player.GetMoveDir();
		Vector3 velocity = rigid.velocity;
		if (moveSpeed == 0f)
		{
			rigid.velocity = Vector3.zero;
			if (moveEffect.isPlaying)
			{
				moveEffect.Stop();
			}
		}
		else
		{
			float baseMoveSpeed = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetBaseMoveSpeed();
			float maxMoveSpeed = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetMaxMoveSpeed((int)characterType);
			float correctionUpDiffMoveSpeed = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetCorrectionUpDiffMoveSpeed();
			Vector3 a = moveDir * baseMoveSpeed * moveSpeed;
			velocity += a * Time.deltaTime * correctionUpDiffMoveSpeed;
			if (velocity.magnitude > maxMoveSpeed * moveSpeed)
			{
				velocity = velocity.normalized * maxMoveSpeed * moveSpeed;
			}
			Vector3 lhs = velocity;
			velocity = GetCanMoveVelocity(velocity);
			rigid.velocity = velocity;
			Rot(rigid.velocity.normalized, lhs != velocity);
			if (!player.GetIsCpu() && !isMoveSe)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_morphingrace_eagle_wing");
				isMoveSe = true;
				LeanTween.delayedCall(base.gameObject, 0.4f, (Action)delegate
				{
					isMoveSe = false;
				});
			}
			if (!moveEffect.isPlaying)
			{
				moveEffect.Play();
			}
		}
		animator.SetFloat("Speed", inputLerp);
	}
	public override void StopMove()
	{
		base.StopMove();
		LeanTween.cancel(base.gameObject);
		isMoveSe = false;
		moveEffect.Stop();
		if (player.GetIsCpu())
		{
			cpuAI.StopMove();
		}
	}
	public override void AnimationFinish()
	{
		base.transform.SetLocalEulerAnglesX(0f);
		animator.SetTrigger("IsFinish");
	}
	public override void MorphingInit()
	{
		throughAreaIdx = 0;
		throughAreaCnt = player.GetMorphingTargetEagle().GetThroughAnchorLength(player.GetPlayerNo());
		if (player.GetIsCpu())
		{
			cpuAI.MorphingInit();
		}
	}
	public int GetThroughAreaIdx()
	{
		return throughAreaIdx;
	}
	public void AddThroughAreaIdx()
	{
		throughAreaIdx++;
	}
}
