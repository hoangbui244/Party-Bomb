using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterKill_EnemyManager : SingletonCustom<MonsterKill_EnemyManager>
{
	public enum EnemyType
	{
		Bat,
		BlackKnight,
		ChestMonster,
		CrabMonster,
		Dragon,
		EvilMage,
		Golem,
		LizardWarrior,
		MonsterPlant,
		Orc,
		Skeleton,
		Slime,
		Spider,
		TurtleShell,
		Werewolf
	}
	[Serializable]
	private struct MonsterData
	{
		[SerializeField]
		[Header("モンスタ\u30fcの名称")]
		private string enemyName;
		[Header("生成するモンスタ\u30fcのプレハブ")]
		public MonsterKill_Enemy enemyPref;
		[Header("各モンスタ\u30fcの出現率")]
		public int appearanceRate;
		[Header("各モンスタ\u30fc死亡時の加算ポイント")]
		public DeadPointTpe deadPoint;
	}
	public enum DeadPointTpe
	{
		Point_100,
		Point_200,
		Point_500
	}
	[SerializeField]
	[Header("デバッグ用：出現するモンスタ\u30fc固定フラグ")]
	private bool isDebugSpawnEnemy;
	[SerializeField]
	[Header("デバッグ用：出現するモンスタ\u30fcの種類")]
	private EnemyType debugSpawnEnemyType;
	[SerializeField]
	[Header("フィ\u30fcルド上に出現させるモンスタ\u30fcの最大数")]
	private int ENEMY_MAX_CNT;
	[SerializeField]
	[Header("生成したモンスタ\u30fcを格納するアンカ\u30fc")]
	private Transform generateEnemyAnchor;
	private List<MonsterKill_Enemy> enemyList = new List<MonsterKill_Enemy>();
	private List<MonsterKill_Enemy> fiexdEnemyList = new List<MonsterKill_Enemy>();
	[SerializeField]
	[Header("モンスタ\u30fcデ\u30fcタ")]
	private MonsterData[] arrayMonsterData;
	[SerializeField]
	[Header("モンスタ\u30fcの生成時にプレイヤ\u30fc付近には生成されないようにする距離")]
	private float GENERATE_NEAR_PLAYER_DISTANCE;
	[SerializeField]
	[Header("出現エリア付近を移動する時の移動先を決めるまでのインタ\u30fcバル")]
	private float NEAR_SPAWN_MOVE_INTERVAL;
	[SerializeField]
	[Header("ダメ\u30fcジ判定中の時間")]
	private float DAMAGE_TIME;
	private readonly float IGNORE_DAMAGE_GENERATE_TIME = 5f;
	private float ignoreDamageGenerateTime;
	public void Init()
	{
		InitGenerateEnemy();
		GenerateEnemy(ENEMY_MAX_CNT - enemyList.Count, _isInit: true);
		ignoreDamageGenerateTime = IGNORE_DAMAGE_GENERATE_TIME;
	}
	public void UpdateMethod()
	{
		if (ignoreDamageGenerateTime > 0f)
		{
			ignoreDamageGenerateTime -= Time.deltaTime;
		}
		if (enemyList.Count < ENEMY_MAX_CNT)
		{
			GenerateEnemy(ENEMY_MAX_CNT - enemyList.Count);
		}
		for (int i = 0; i < enemyList.Count; i++)
		{
			enemyList[i].UpdateMethod();
		}
	}
	public void InitGenerateEnemy()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			Vector3 position = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(i).transform.position;
			list.Clear();
			float num = 10000f;
			MonsterKill_Enemy_SpawnArea enemySpawnArea;
			for (int j = 0; j < SingletonCustom<MonsterKill_FieldManager>.Instance.GetEnemySpawnAreaLength(); j++)
			{
				enemySpawnArea = SingletonCustom<MonsterKill_FieldManager>.Instance.GetEnemySpawnArea(j);
				if (!IsNearPlayer(enemySpawnArea.transform.position, position) && GetSpawnCnt(enemySpawnArea, enemyList) != 0)
				{
					float num2 = CalcManager.Length(enemySpawnArea.transform.position, position);
					if (num2 < num)
					{
						list.Clear();
						list.Add(j);
						num = num2;
					}
					else if (num2 == num)
					{
						list.Add(j);
					}
				}
			}
			if (list.Count <= 0)
			{
				continue;
			}
			int idx = list[UnityEngine.Random.Range(0, list.Count)];
			enemySpawnArea = SingletonCustom<MonsterKill_FieldManager>.Instance.GetEnemySpawnArea(idx);
			for (int k = 0; k < enemySpawnArea.GetSpawnCnt(); k++)
			{
				MonsterKill_Enemy monsterKill_Enemy = Generate(enemySpawnArea);
				if (monsterKill_Enemy == null)
				{
					k--;
					continue;
				}
				monsterKill_Enemy.Init(enemySpawnArea);
				switch (monsterKill_Enemy.GetEnemyType())
				{
				case EnemyType.BlackKnight:
				case EnemyType.Golem:
				case EnemyType.LizardWarrior:
				case EnemyType.Orc:
				case EnemyType.Werewolf:
					monsterKill_Enemy.SetIsIgnoreDamage(UnityEngine.Random.Range(0, 2) == 0);
					break;
				}
				enemyList.Add(monsterKill_Enemy);
			}
		}
	}
	public void GenerateEnemy(int _generateCnt, bool _isInit = false)
	{
		for (int i = 0; i < _generateCnt && i < SingletonCustom<MonsterKill_FieldManager>.Instance.GetEnemySpawnAreaLength(); i++)
		{
			MonsterKill_Enemy_SpawnArea enemySpawnArea = SingletonCustom<MonsterKill_FieldManager>.Instance.GetEnemySpawnArea(i);
			if (IsNearAllPlayer(enemySpawnArea.transform.position))
			{
				continue;
			}
			int spawnCnt = GetSpawnCnt(enemySpawnArea, enemyList);
			if (spawnCnt == 0)
			{
				continue;
			}
			for (int j = 0; j < spawnCnt; j++)
			{
				MonsterKill_Enemy monsterKill_Enemy = Generate(enemySpawnArea);
				if (monsterKill_Enemy == null)
				{
					continue;
				}
				monsterKill_Enemy.Init(enemySpawnArea);
				switch (monsterKill_Enemy.GetEnemyType())
				{
				case EnemyType.BlackKnight:
				case EnemyType.Golem:
				case EnemyType.LizardWarrior:
				case EnemyType.Orc:
				case EnemyType.Werewolf:
					if (_isInit)
					{
						monsterKill_Enemy.SetIsIgnoreDamage(UnityEngine.Random.Range(0, 2) == 0);
						break;
					}
					monsterKill_Enemy.SetIsIgnoreDamage(ignoreDamageGenerateTime <= 0f);
					ignoreDamageGenerateTime = IGNORE_DAMAGE_GENERATE_TIME;
					break;
				}
				enemyList.Add(monsterKill_Enemy);
				if (enemyList.Count >= ENEMY_MAX_CNT)
				{
					break;
				}
			}
			if (enemyList.Count >= ENEMY_MAX_CNT)
			{
				break;
			}
		}
		SingletonCustom<MonsterKill_FieldManager>.Instance.ShuffleEnemySpawnArea();
	}
	public List<MonsterKill_Enemy> GetEnemyList()
	{
		return enemyList;
	}
	public void RemoveEnemyList(MonsterKill_Enemy _enemy)
	{
		enemyList.Remove(_enemy);
	}
	private void GenerateFixedEnemy()
	{
		for (int i = 0; i < SingletonCustom<MonsterKill_FieldManager>.Instance.GetFixedEnemySpawnAreaLength(); i++)
		{
			MonsterKill_Enemy_SpawnArea fixedEnemySpawnArea = SingletonCustom<MonsterKill_FieldManager>.Instance.GetFixedEnemySpawnArea(i);
			if (!IsNearAllPlayer(fixedEnemySpawnArea.transform.position) && GetSpawnCnt(fixedEnemySpawnArea, enemyList) != 0)
			{
				MonsterKill_Enemy monsterKill_Enemy = Generate(fixedEnemySpawnArea);
				if (!(monsterKill_Enemy == null))
				{
					monsterKill_Enemy.Init(fixedEnemySpawnArea);
					fiexdEnemyList.Add(monsterKill_Enemy);
				}
			}
		}
		SingletonCustom<MonsterKill_FieldManager>.Instance.ShuffleFixedEnemySpawnArea();
	}
	public List<MonsterKill_Enemy> GetFixedEnemyList()
	{
		return fiexdEnemyList;
	}
	public void RemoveFixedEnemyList(MonsterKill_Enemy _enemy)
	{
		fiexdEnemyList.Remove(_enemy);
	}
	private int GetSpawnCnt(MonsterKill_Enemy_SpawnArea _spawnArea, List<MonsterKill_Enemy> _enemyList)
	{
		int num = 0;
		for (int i = 0; i < _enemyList.Count; i++)
		{
			if (_spawnArea == _enemyList[i].GetSpawnArea())
			{
				num++;
				if (num >= _spawnArea.GetSpawnCnt())
				{
					return 0;
				}
			}
		}
		return _spawnArea.GetSpawnCnt() - num;
	}
	private bool IsNearAllPlayer(Vector3 _spawnAreaPos)
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			Vector3 position = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(i).transform.position;
			if (IsNearPlayer(_spawnAreaPos, position))
			{
				return true;
			}
		}
		return false;
	}
	private bool IsNearPlayer(Vector3 _spawnAreaPos, Vector3 _playerPos)
	{
		if (CalcManager.Length(_spawnAreaPos, _playerPos) <= GENERATE_NEAR_PLAYER_DISTANCE)
		{
			return true;
		}
		return false;
	}
	private MonsterKill_Enemy Generate(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		Vector3 canMovePos = GetCanMovePos(_spawnArea);
		if (canMovePos == Vector3.zero)
		{
			return null;
		}
		EnemyType randomSpawnEnemyType = _spawnArea.GetRandomSpawnEnemyType();
		switch (randomSpawnEnemyType)
		{
		case EnemyType.BlackKnight:
		case EnemyType.Golem:
		case EnemyType.LizardWarrior:
		case EnemyType.Orc:
		case EnemyType.Werewolf:
			if (_spawnArea.GetIsBigMonsterAppearance())
			{
				return null;
			}
			_spawnArea.SetIsBigMonsterAppearance(_isBigMonsterAppearance: true);
			break;
		}
		return UnityEngine.Object.Instantiate(arrayMonsterData[(int)randomSpawnEnemyType].enemyPref, canMovePos, Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 359f), 0f)), generateEnemyAnchor);
	}
	public Vector3 GetCanMovePos(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		Vector3 randomGeneratePos = _spawnArea.GetRandomGeneratePos();
		RaycastHit hitInfo;
		if (Physics.BoxCast(halfExtents: new Vector3(0.25f, 0.5f, 0.25f), center: randomGeneratePos + new Vector3(0f, 10f, 0f), direction: Vector3.down, hitInfo: out hitInfo, orientation: Quaternion.identity, maxDistance: 10f) && hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Field") && NavMesh.SamplePosition(hitInfo.point, out NavMeshHit hit, 1f, 1 << NavMesh.GetAreaFromName("Walkable")))
		{
			return hit.position;
		}
		return Vector3.zero;
	}
	public int GetAppearanceRate(EnemyType _enemyType)
	{
		return arrayMonsterData[(int)_enemyType].appearanceRate;
	}
	public DeadPointTpe GetDeadPointType(EnemyType _enemyType)
	{
		return arrayMonsterData[(int)_enemyType].deadPoint;
	}
	public int GetDeadPoint(DeadPointTpe _deadPointType)
	{
		switch (_deadPointType)
		{
		case DeadPointTpe.Point_100:
			return 100;
		case DeadPointTpe.Point_200:
			return 200;
		case DeadPointTpe.Point_500:
			return 500;
		default:
			return 100;
		}
	}
	public float GetNearSpawnMoveInterval()
	{
		return NEAR_SPAWN_MOVE_INTERVAL;
	}
	public float GetDamageTime()
	{
		return DAMAGE_TIME;
	}
}
