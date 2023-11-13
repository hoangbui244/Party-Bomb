using UnityEngine;
public class MonsterKill_Enemy_AnimationManagement : MonoBehaviour
{
	private MonsterKill_Enemy enemy;
	private Animator animator;
	public void Init(MonsterKill_Enemy _enemy)
	{
		enemy = _enemy;
		animator = GetComponent<Animator>();
	}
	public void SetIdleAnimation(bool _isBool)
	{
		if (animator.GetBool("IsIdle") != _isBool)
		{
			animator.SetBool("IsIdle", _isBool);
		}
	}
	public void SetMoveAnimation(float _moveSpeed)
	{
		animator.SetFloat("MoveSpeed", _moveSpeed);
	}
	public void SetAttackAnimation(MonsterKill_Enemy.AttackType _attackType)
	{
		switch (_attackType)
		{
		case MonsterKill_Enemy.AttackType.AttackType_0:
			animator.SetTrigger("ToAttack_0");
			break;
		case MonsterKill_Enemy.AttackType.AttackType_1:
			animator.SetTrigger("ToAttack_1");
			break;
		case MonsterKill_Enemy.AttackType.AttackType_2:
			animator.SetTrigger("ToAttack_2");
			break;
		}
	}
	public void ResetAttackAnimation()
	{
		animator.ResetTrigger("ToAttack_0");
		animator.ResetTrigger("ToAttack_1");
		animator.ResetTrigger("ToAttack_2");
	}
	public void AttackEnd()
	{
		enemy.AttackEnd();
	}
	public void EnableAttackCollider(int _attackCnt)
	{
		enemy.SetAttackColliderActive(_isActive: true, _attackCnt);
	}
	public void DisableAttackCollider()
	{
		enemy.SetAttackColliderActive(_isActive: false);
	}
	public void SetDamageAnimation()
	{
		animator.SetTrigger("ToDamage");
	}
	public void SetDeadAnimation()
	{
		animator.SetTrigger("ToDead");
	}
	public void SetGameEndAnimation()
	{
		SetIdleAnimation(_isBool: true);
		SetMoveAnimation(0f);
		ResetAttackAnimation();
	}
}
