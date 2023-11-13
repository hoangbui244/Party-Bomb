using System;
using UnityEngine;
public class MorphingRace_OperationHuman : MorphingRace_OperationCharacter
{
	private Vector3 nowPos;
	private Vector3 prevPos;
	private MorphingRace_OperationHuman_AI cpuAI;
	public override void Init(MorphingRace_Player _player)
	{
		base.Init(_player);
		nowPos = (prevPos = base.transform.position);
		if (player.GetIsCpu())
		{
			cpuAI = base.gameObject.AddComponent<MorphingRace_OperationHuman_AI>();
			cpuAI.Init(this);
		}
	}
	public override void MoveAfterGoal(Vector3 _vec, float _lerp)
	{
		prevPos = nowPos;
		nowPos = player.transform.position;
		Rot(_vec);
		player.GetCharacter().MoveAnimation((int)characterType, 0.5f * (1f - _lerp), nowPos, prevPos);
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
			if (!player.GetIsCpu() && !player.GetIsOnceInput())
			{
				player.SetIsOnceInput();
			}
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
		prevPos = nowPos;
		nowPos = player.transform.position;
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
			vector = GetCanMoveVelocity(vector);
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
		player.GetCharacter().MoveAnimation((int)characterType, inputLerp, nowPos, prevPos);
	}
	public override void StopMove()
	{
		base.StopMove();
		nowPos = (prevPos = base.transform.position);
		LeanTween.cancel(base.gameObject);
		isMoveSe = false;
		moveEffect.Stop();
		if (player.GetIsCpu())
		{
			cpuAI.StopMove();
		}
	}
	public Vector3 GetNowPos()
	{
		return nowPos;
	}
	public Vector3 GetPrevPos()
	{
		return prevPos;
	}
	public override void MorphingInit()
	{
		cpuAI.MorphingInit();
	}
}
