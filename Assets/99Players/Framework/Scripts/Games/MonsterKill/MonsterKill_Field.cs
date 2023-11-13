using UnityEngine;
public class MonsterKill_Field : MonoBehaviour
{
	[SerializeField]
	[Header("ナビメッシュを設定しないようにするオブジェクト")]
	private GameObject noneNavMeshRoot;
	[SerializeField]
	[Header("通常モンスタ\u30fcの出現エリアのル\u30fcト")]
	private Transform enemySpawnAreaRoot;
	private MonsterKill_Enemy_SpawnArea[] arrayEnemySpawnArea;
	[SerializeField]
	[Header("固定モンスタ\u30fcの出現エリアのル\u30fcト")]
	private Transform fixedEnemySpawnAreaRoot;
	private MonsterKill_Enemy_SpawnArea[] arrayFixedEnemySpawnArea;
	[SerializeField]
	[Header("プレイヤ\u30fcの出現アンカ\u30fcのル\u30fcト")]
	private Transform playerSpawnAnchorRoot;
	private MonsterKill_Player_SpawnAnchorPattern playerSpawnAcnhor;
	public void Init()
	{
		arrayEnemySpawnArea = enemySpawnAreaRoot.GetComponentsInChildren<MonsterKill_Enemy_SpawnArea>();
		for (int i = 0; i < arrayEnemySpawnArea.Length; i++)
		{
			arrayEnemySpawnArea[i].Init();
		}
		ShuffleEnemySpawnArea();
		arrayFixedEnemySpawnArea = fixedEnemySpawnAreaRoot.GetComponentsInChildren<MonsterKill_Enemy_SpawnArea>();
		for (int j = 0; j < arrayFixedEnemySpawnArea.Length; j++)
		{
			arrayFixedEnemySpawnArea[j].Init();
		}
		ShuffleFixedEnemySpawnArea();
		MonsterKill_Player_SpawnAnchorPattern[] componentsInChildren = playerSpawnAnchorRoot.GetComponentsInChildren<MonsterKill_Player_SpawnAnchorPattern>();
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			componentsInChildren[k].gameObject.SetActive(value: false);
		}
		playerSpawnAcnhor = componentsInChildren[Random.Range(0, componentsInChildren.Length)];
		playerSpawnAcnhor.ShufflePlayerSpawnAnchor();
		playerSpawnAcnhor.gameObject.SetActive(value: true);
	}
	public void ShuffleEnemySpawnArea()
	{
		arrayEnemySpawnArea.Shuffle();
	}
	public void ShuffleFixedEnemySpawnArea()
	{
		arrayFixedEnemySpawnArea.Shuffle();
	}
	public int GetEnemySpawnAreaLength()
	{
		return arrayEnemySpawnArea.Length;
	}
	public int GetFixedEnemySpawnAreaLength()
	{
		return arrayFixedEnemySpawnArea.Length;
	}
	public MonsterKill_Enemy_SpawnArea GetEnemySpawnArea(int _idx)
	{
		return arrayEnemySpawnArea[_idx];
	}
	public MonsterKill_Enemy_SpawnArea GetFixedEnemySpawnArea(int _idx)
	{
		return arrayFixedEnemySpawnArea[_idx];
	}
	public Transform GetPlayerAnchor(int _playerNo)
	{
		return playerSpawnAcnhor.GetPlayerAnchor(_playerNo);
	}
}
