using UnityEngine;
public class MonsterKill_Enemy_LizardWarrior : MonsterKill_Enemy_BigMonster
{
	public override void Init(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		attackType = (AttackType)Random.Range(0, 3);
		base.Init(_spawnArea);
	}
}
