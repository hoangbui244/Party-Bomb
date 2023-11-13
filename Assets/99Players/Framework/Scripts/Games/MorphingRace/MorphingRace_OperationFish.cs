using System;
using UnityEngine;
public class MorphingRace_OperationFish : MorphingRace_OperationCharacter
{
	private Animator animator;
	private int swimAreaIdx;
	private int swimAreaCnt;
	private bool isBreathEffect;
	[SerializeField]
	[Header("呼吸用の泡エフェクト")]
	private ParticleSystem breathEffect;
	[SerializeField]
	[Header("キャラのメッシュ")]
	private SkinnedMeshRenderer meshChara;
	[SerializeField]
	[Header("キャラのマテリアル")]
	private Material[] arrayMatChara;
	private MorphingRace_OperationFish_AI cpuAI;
	public override void Init(MorphingRace_Player _player)
	{
		base.Init(_player);
		SetMatChara();
		animator = GetComponent<Animator>();
		if (player.GetIsCpu())
		{
			cpuAI = base.gameObject.AddComponent<MorphingRace_OperationFish_AI>();
			cpuAI.Init(this);
		}
	}
	private void SetMatChara()
	{
		if (!(meshChara != null))
		{
			return;
		}
		string name = arrayMatChara[0].name;
		name = name.Substring(0, name.LastIndexOf("_"));
		Material[] materials = meshChara.materials;
		for (int i = 0; i < materials.Length; i++)
		{
			if (materials[i].name.Contains(name))
			{
				materials[i] = arrayMatChara[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)player.GetUserType()]];
				break;
			}
		}
		meshChara.materials = materials;
	}
	public override void UpdateMethod_Base()
	{
		base.UpdateMethod_Base();
		player.SetInputInterval();
		SetBreathEffect();
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
	private void SetBreathEffect()
	{
		if (!isBreathEffect && !breathEffect.isPlaying)
		{
			isBreathEffect = true;
			LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
			{
				isBreathEffect = false;
				breathEffect.Play();
			});
		}
	}
	public bool CheckSwimAreaIdxLessCnt()
	{
		return swimAreaIdx < swimAreaCnt;
	}
	public bool CheckPassSwimArea()
	{
		if (CheckSwimAreaIdxLessCnt() && player.GetMorphingTargetFish().CheckPassSwimArea(player.GetPlayerNo(), swimAreaIdx, player.transform.position))
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
			velocity = GetCanMoveVelocity(velocity);
			rigid.velocity = velocity;
			Rot(rigid.velocity.normalized);
			if (!player.GetIsCpu() && !isMoveSe)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_morphingrace_fish_swim", _loop: false, 0f, 0.5f);
				isMoveSe = true;
				LeanTween.delayedCall(base.gameObject, 0.7f, (Action)delegate
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
		isBreathEffect = false;
		breathEffect.Stop();
		if (player.GetIsCpu())
		{
			cpuAI.StopMove();
		}
	}
	public override void MorphingInit()
	{
		swimAreaIdx = 0;
		swimAreaCnt = player.GetMorphingTargetFish().GetSwimAreaLength(player.GetPlayerNo());
		cpuAI.MorphingInit();
	}
	public int GetSwimAreaIdx()
	{
		return swimAreaIdx;
	}
	public void AddSwimAreaIdx()
	{
		swimAreaIdx++;
	}
}
