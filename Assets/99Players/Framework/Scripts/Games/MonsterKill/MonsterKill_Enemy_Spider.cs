using UnityEngine;
public class MonsterKill_Enemy_Spider : MonsterKIll_Enemy_SmallFryMonster
{
	public override void Init(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		attackType = ((!(Random.Range(0f, 1f) < 0.7f)) ? AttackType.AttackType_1 : AttackType.AttackType_0);
		base.Init(_spawnArea);
	}
}
