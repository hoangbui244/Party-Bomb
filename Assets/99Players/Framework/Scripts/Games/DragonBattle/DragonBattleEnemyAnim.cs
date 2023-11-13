using System;
using UnityEngine;
public class DragonBattleEnemyAnim : MonoBehaviour
{
	public enum AnimType
	{
		Idle,
		Attack01,
		Attack02,
		Attack03,
		Attack04,
		GetHit,
		Die,
		Appear,
		Max
	}
	public enum AttackType
	{
		Type0,
		Type1,
		Type2,
		Type3
	}
	private Action<int> onEnableAttack;
	private Action onDisableAttack;
	private Action onAttackEnd;
	private Animator animator;
	public void Init(Action<int> _onEnableAttack = null, Action _onDisableAttack = null, Action _onAttackEnd = null)
	{
		onEnableAttack = _onEnableAttack;
		onDisableAttack = _onDisableAttack;
		onAttackEnd = _onAttackEnd;
		animator = GetComponent<Animator>();
	}
	public void SetIdleAnim()
	{
		animator.SetTrigger("ToIdle");
	}
	public void SetMoveAnim(float _moveSpeed)
	{
		animator.SetFloat("MoveSpeed", _moveSpeed);
	}
	public void SetAttackAnim(AttackType _attackType)
	{
		switch (_attackType)
		{
		case AttackType.Type0:
			animator.SetTrigger("ToAttack_0");
			break;
		case AttackType.Type1:
			animator.SetTrigger("ToAttack_1");
			break;
		case AttackType.Type2:
			animator.SetTrigger("ToAttack_2");
			break;
		case AttackType.Type3:
			animator.SetTrigger("ToAttack_3");
			break;
		}
	}
	public bool IsPlayingAttack()
	{
		if (!CheckPlayingAnim(AnimType.Attack01) && !CheckPlayingAnim(AnimType.Attack02) && !CheckPlayingAnim(AnimType.Attack03))
		{
			return CheckPlayingAnim(AnimType.Attack04);
		}
		return true;
	}
	public void ResetAttackAnim()
	{
		animator.ResetTrigger("ToAttack_0");
		animator.ResetTrigger("ToAttack_1");
		animator.ResetTrigger("ToAttack_2");
		animator.ResetTrigger("ToAttack_3");
	}
	public void AttackEnd()
	{
		if (onAttackEnd != null)
		{
			onAttackEnd();
		}
	}
	public void EnableAttackCollider(int _attackCnt)
	{
		if (onEnableAttack != null)
		{
			onEnableAttack(_attackCnt);
		}
	}
	public void DisableAttackCollider()
	{
		if (onDisableAttack != null)
		{
			onDisableAttack();
		}
	}
	public void SetDamageAnim()
	{
		animator.SetTrigger("ToDamage");
	}
	public void SetDeadAnim()
	{
		animator.SetTrigger("ToDead");
	}
	public void SetAppearAnim()
	{
		animator.Play("Appear");
	}
	public void SetGameEndAnim()
	{
		SetIdleAnim();
		SetMoveAnim(0f);
		ResetAttackAnim();
	}
	public bool CheckPlayingAnim(AnimType _type)
	{
		return animator.GetCurrentAnimatorStateInfo(0).IsName(_type.ToString());
	}
}
