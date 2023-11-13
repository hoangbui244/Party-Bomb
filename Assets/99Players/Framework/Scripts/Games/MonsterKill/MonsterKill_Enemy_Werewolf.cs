using UnityEngine;
public class MonsterKill_Enemy_Werewolf : MonsterKill_Enemy_BigMonster
{
	public override void Init(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		attackType = (AttackType)Random.Range(0, 2);
		base.Init(_spawnArea);
	}
}
