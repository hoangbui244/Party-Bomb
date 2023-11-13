using UnityEngine;
public class MonsterKill_Enemy_Treant : MonsterKill_Enemy
{
	public override void Init(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		base.Init(_spawnArea);
		attackType = (AttackType)Random.Range(0, 3);
	}
	protected override void Dead(int _playerNo)
	{
		base.Dead(_playerNo);
		spawnArea.SetIsBigMonsterAppearance(_isBigMonsterAppearance: false);
	}
}
