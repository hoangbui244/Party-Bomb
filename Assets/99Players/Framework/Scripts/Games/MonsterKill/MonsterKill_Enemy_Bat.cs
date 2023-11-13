using UnityEngine;
public class MonsterKill_Enemy_Bat : MonsterKIll_Enemy_SmallFryMonster
{
	public override void Init(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		attackType = (AttackType)Random.Range(0, 2);
		base.Init(_spawnArea);
	}
}
