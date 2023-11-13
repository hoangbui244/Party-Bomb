public class MonsterKill_Enemy_Dragon : MonsterKIll_Enemy_SmallFryMonster
{
	public override void Init(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		attackType = AttackType.AttackType_0;
		base.Init(_spawnArea);
	}
}
