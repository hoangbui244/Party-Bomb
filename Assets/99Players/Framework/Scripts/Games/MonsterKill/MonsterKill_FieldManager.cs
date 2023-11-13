using UnityEngine;
public class MonsterKill_FieldManager : SingletonCustom<MonsterKill_FieldManager>
{
	[SerializeField]
	[Header("フィ\u30fcルド")]
	private MonsterKill_Field field;
	public void Init()
	{
		field.Init();
	}
	public MonsterKill_Field GetField()
	{
		return field;
	}
	public void ShuffleEnemySpawnArea()
	{
		field.ShuffleEnemySpawnArea();
	}
	public void ShuffleFixedEnemySpawnArea()
	{
		field.ShuffleFixedEnemySpawnArea();
	}
	public int GetEnemySpawnAreaLength()
	{
		return field.GetEnemySpawnAreaLength();
	}
	public int GetFixedEnemySpawnAreaLength()
	{
		return field.GetFixedEnemySpawnAreaLength();
	}
	public MonsterKill_Enemy_SpawnArea GetEnemySpawnArea(int _idx)
	{
		return field.GetEnemySpawnArea(_idx);
	}
	public MonsterKill_Enemy_SpawnArea GetFixedEnemySpawnArea(int _idx)
	{
		return field.GetFixedEnemySpawnArea(_idx);
	}
	public Transform GetPlayerAnchor(int _playerNo)
	{
		return field.GetPlayerAnchor(_playerNo);
	}
}
