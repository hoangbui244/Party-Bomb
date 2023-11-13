using System;
using UnityEngine;
using UnityEngine.AI;
public class MorphingRace_OperationMouse : MorphingRace_OperationCharacter
{
	private Animator animator;
	private NavMeshAgent agent;
	private bool isStartThrough;
	private bool isGoalThrough;
	private MorphingRace_OperationMouse_AI cpuAI;
	public override void Init(MorphingRace_Player _player)
	{
		base.Init(_player);
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		agent.enabled = false;
		if (player.GetIsCpu())
		{
			cpuAI = base.gameObject.AddComponent<MorphingRace_OperationMouse_AI>();
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
	public override void Move()
	{
		Vector3 moveDir = player.GetMoveDir();
		Vector3 vector = rigid.velocity;
		if (moveSpeed == 0f)
		{
			vector.x = 0f;
			vector.z = 0f;
			rigid.velocity = vector;
			if (moveEffect.isPlaying)
			{
				moveEffect.Stop();
			}
		}
		else
		{
			float y = vector.y;
			float baseMoveSpeed = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetBaseMoveSpeed();
			float maxMoveSpeed = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetMaxMoveSpeed((int)characterType);
			float correctionUpDiffMoveSpeed = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetCorrectionUpDiffMoveSpeed();
			Vector3 a = moveDir * baseMoveSpeed * moveSpeed;
			vector += a * Time.deltaTime * correctionUpDiffMoveSpeed;
			if (vector.magnitude > maxMoveSpeed * moveSpeed)
			{
				vector = vector.normalized * maxMoveSpeed * moveSpeed;
			}
			vector.y = y;
			if (!player.GetIsCpu())
			{
				vector = GetCanMoveVelocity(vector);
			}
			rigid.velocity = vector;
			Rot(rigid.velocity.normalized);
			if (!player.GetIsCpu() && !isMoveSe)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.65f);
				isMoveSe = true;
				LeanTween.delayedCall(base.gameObject, 0.15f, (Action)delegate
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
	public override void MorphingInit()
	{
		cpuAI.MorphingInit();
	}
	public NavMeshAgent GetNavMeshAgent()
	{
		return agent;
	}
	private void SetAlphaGate()
	{
		if (!isStartThrough && player.GetMorphingTarget_Mouse().CheckPassThroughStartAnchor(player.GetPlayerNo(), player.transform.position))
		{
			isStartThrough = true;
			player.GetMorphingTarget_Mouse().AlphaStartGate(player.GetPlayerNo());
		}
		if (!isGoalThrough && player.GetMorphingTarget_Mouse().CheckPassThroughGoalAnchor(player.GetPlayerNo(), player.transform.position))
		{
			isGoalThrough = true;
			player.GetMorphingTarget_Mouse().AlphaGoalGate(player.GetPlayerNo());
		}
	}
}
