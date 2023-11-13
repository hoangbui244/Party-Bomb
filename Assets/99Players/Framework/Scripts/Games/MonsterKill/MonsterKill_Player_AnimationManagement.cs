using UnityEngine;
public class MonsterKill_Player_AnimationManagement : MonoBehaviour
{
	private MonsterKill_Player player;
	private Animator animator;
	public void Init(MonsterKill_Player _player)
	{
		player = _player;
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
	public void SetDashAnimation(bool _isBool)
	{
		if (animator.GetBool("IsDash") != _isBool)
		{
			animator.SetBool("IsDash", _isBool);
		}
	}
	public void SetAttackAnimation(MonsterKill_Player.State _state)
	{
		switch (_state)
		{
		case MonsterKill_Player.State.SwordAttack_0:
			animator.SetTrigger("ToAttack_0");
			break;
		case MonsterKill_Player.State.SwordAttack_1:
			animator.SetTrigger("ToAttack_1");
			break;
		case MonsterKill_Player.State.SwordAttack_2:
			animator.SetTrigger("ToAttack_2");
			break;
		}
		if (!player.GetIsCpu())
		{
			switch (_state)
			{
			case MonsterKill_Player.State.SwordAttack_0:
				SingletonCustom<AudioManager>.Instance.SePlay("se_sword_0", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				break;
			case MonsterKill_Player.State.SwordAttack_1:
				SingletonCustom<AudioManager>.Instance.SePlay("se_sword_1", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				break;
			case MonsterKill_Player.State.SwordAttack_2:
				SingletonCustom<AudioManager>.Instance.SePlay("se_sword_2", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				break;
			}
		}
	}
	public void ResetSwordAttackAnimation()
	{
		animator.ResetTrigger("ToAttack_0");
		animator.ResetTrigger("ToAttack_1");
		animator.ResetTrigger("ToAttack_2");
	}
	public void OnAttackStart()
	{
		player.AttackStart();
	}
	public void OnAttackEnd()
	{
		player.AttackEnd();
	}
	public void SetJumpAnimation()
	{
		animator.SetTrigger("ToJump");
	}
	public void SetDodgeAnimation()
	{
		animator.SetTrigger("ToDodge");
	}
	public void OnDodgeEnd()
	{
		player.DodgeEnd();
	}
	public void SetDamageAnimation()
	{
		animator.SetTrigger("ToDamage");
	}
	public void SetStunAnimation()
	{
		animator.SetTrigger("ToStun");
	}
	public void SetMagicCastAnimation(bool _isBool)
	{
		if (animator.GetBool("IsMagicCast") != _isBool)
		{
			animator.SetBool("IsMagicCast", _isBool);
		}
	}
	public void SetMagicAttackAnimation()
	{
		animator.SetTrigger("ToMagicAttack");
	}
	public void ResetMagicAttackAnimation()
	{
		animator.ResetTrigger("ToMagicAttack");
	}
	public void SetGameEndAnimation()
	{
		SetIdleAnimation(_isBool: true);
		SetMoveAnimation(0f);
		SetDashAnimation(_isBool: false);
		ResetSwordAttackAnimation();
		SetMagicCastAnimation(_isBool: false);
		ResetMagicAttackAnimation();
	}
}
