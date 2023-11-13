using System;
public class MonsterKill_Enemy_BigMonster : MonsterKill_Enemy
{
	public override void Init(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		base.Init(_spawnArea);
	}
	protected override void DamageShake()
	{
		LeanTween.cancel(modelRoot);
		LeanTween.value(modelRoot, 0f, 1f, 0.02f).setOnUpdate(delegate(float _value)
		{
			modelRoot.transform.SetLocalPositionX((LeanTween.shake.Evaluate(_value) - 0.5f) * 0.15f);
			modelRoot.transform.SetLocalPositionZ((LeanTween.shake.Evaluate(_value) - 0.5f) * 0.15f);
		}).setOnComplete((Action)delegate
		{
			modelRoot.transform.SetLocalPositionX(0f);
			modelRoot.transform.SetLocalPositionZ(0f);
		})
			.setLoopPingPong(2);
	}
	protected override void Dead(int _playerNo)
	{
		base.Dead(_playerNo);
		spawnArea.SetIsBigMonsterAppearance(_isBigMonsterAppearance: false);
	}
}
