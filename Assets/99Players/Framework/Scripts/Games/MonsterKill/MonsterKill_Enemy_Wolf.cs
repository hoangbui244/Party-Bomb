public class MonsterKill_Enemy_Wolf : MonsterKill_Enemy
{
	public override void Init(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		base.Init(_spawnArea);
		attackType = AttackType.AttackType_0;
	}
}
