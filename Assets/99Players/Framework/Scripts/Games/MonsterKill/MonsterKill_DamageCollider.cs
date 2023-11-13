using UnityEngine;
public class MonsterKill_DamageCollider : MonoBehaviour
{
	private MonsterKill_Player player;
	private MonsterKill_Enemy enemy;
	public void Init(MonsterKill_Player _player)
	{
		player = _player;
	}
	public void Init(MonsterKill_Enemy _enemy)
	{
		enemy = _enemy;
	}
	public MonsterKill_Player GetPlayer()
	{
		return player;
	}
	public MonsterKill_Enemy GetEnemy()
	{
		return enemy;
	}
	public bool GetIsPlayer()
	{
		return player != null;
	}
	public bool GetIsEnemy()
	{
		return enemy != null;
	}
	public bool GetIsPlayerDamageOrStun()
	{
		if (!player.GetIsDamage())
		{
			return player.GetIsStun();
		}
		return true;
	}
	public bool GetIsEnemyDamageOrDead()
	{
		if (!enemy.GetIsDamage())
		{
			return enemy.GetIsDead();
		}
		return true;
	}
	public bool GetIsEnemyDead()
	{
		return enemy.GetIsDead();
	}
	public bool GetIsPlayerDodge()
	{
		return player.GetIsDodge();
	}
	public void Damage(MonsterKill_AttackCollider.AttackerType _attackType, int _damage, Vector3 _hitPos, Vector3 _attackerPos, int _playerNo = -1)
	{
		switch (_attackType)
		{
		case MonsterKill_AttackCollider.AttackerType.Player_Sword:
		case MonsterKill_AttackCollider.AttackerType.Player_Magic:
			if (player != null)
			{
				player.Damage(_damage, _hitPos, _attackerPos);
			}
			else if (enemy != null)
			{
				enemy.Damage(_damage, _hitPos, _attackerPos, _playerNo);
			}
			break;
		case MonsterKill_AttackCollider.AttackerType.Enemy:
			if (player != null)
			{
				player.Damage(_damage, _hitPos, _attackerPos);
			}
			break;
		}
	}
}
