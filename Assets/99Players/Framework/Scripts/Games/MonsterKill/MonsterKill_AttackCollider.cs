using UnityEngine;
public class MonsterKill_AttackCollider : MonoBehaviour
{
	public enum AttackerType
	{
		Player_Sword,
		Player_Magic,
		Enemy
	}
	private AttackerType attackType;
	private MonsterKill_Player player;
	private MonsterKill_Player_MagicBullet playerMagicBullet;
	private MonsterKill_Enemy enemy;
	[SerializeField]
	[Header("攻撃対象に与えるダメ\u30fcジ")]
	private int attackDamage;
	public void Init(MonsterKill_Player _player, MonsterKill_Player_MagicBullet _playerMagicBullet = null)
	{
		player = _player;
		if (_playerMagicBullet == null)
		{
			attackType = AttackerType.Player_Sword;
			return;
		}
		playerMagicBullet = _playerMagicBullet;
		attackType = AttackerType.Player_Magic;
	}
	public void Init(MonsterKill_Enemy _enemy)
	{
		enemy = _enemy;
		attackType = AttackerType.Enemy;
	}
	private void OnTriggerEnter(Collider other)
	{
		MonsterKill_DamageCollider component = other.GetComponent<MonsterKill_DamageCollider>();
		if (attackType == AttackerType.Player_Magic && (component == null || component.GetPlayer() != player))
		{
			playerMagicBullet.Hit();
		}
		if (!(component != null))
		{
			return;
		}
		Vector3 hitPos = other.ClosestPoint(base.transform.position);
		switch (attackType)
		{
		case AttackerType.Player_Sword:
		case AttackerType.Player_Magic:
			if (!(component.GetPlayer() == player) && (!component.GetIsEnemy() || !component.GetIsEnemyDead()))
			{
				if (!player.GetIsCpu() && attackType == AttackerType.Player_Sword)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)player.GetUserType());
				}
				component.Damage(attackType, attackDamage, hitPos, player.transform.position, player.GetPlayerNo());
			}
			break;
		case AttackerType.Enemy:
			if (!(component.GetEnemy() == enemy) && !component.GetIsEnemy() && !component.GetIsPlayerDamageOrStun() && !component.GetIsPlayerDodge())
			{
				component.Damage(attackType, attackDamage, hitPos, enemy.transform.position);
			}
			break;
		}
	}
}
