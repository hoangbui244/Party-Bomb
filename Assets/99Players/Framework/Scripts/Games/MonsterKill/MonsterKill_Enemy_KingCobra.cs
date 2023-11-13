using UnityEngine;
public class MonsterKill_Enemy_KingCobra : MonsterKill_Enemy
{
	public override void Init(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		base.Init(_spawnArea);
		attackType = (AttackType)Random.Range(0, 3);
	}
}
