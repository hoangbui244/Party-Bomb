using UnityEngine;
public class MonsterKill_Enemy_SpawnArea : MonoBehaviour
{
	[SerializeField]
	[Header("出現可能なモンスタ\u30fcの種類")]
	private MonsterKill_EnemyManager.EnemyType[] arraySpawnEnemyType;
	[SerializeField]
	[Header("エリア内に出現させれるモンスタ\u30fcの数")]
	private int spawnCnt;
	[SerializeField]
	[Header("出現エリアサイズ")]
	private Vector3 areaSize;
	private readonly float INTO_AREA_MAG = 0.75f;
	private int totalAppearanceRate;
	private bool isBigMonsterAppearance;
	public void Init()
	{
		for (int i = 0; i < arraySpawnEnemyType.Length; i++)
		{
			totalAppearanceRate += SingletonCustom<MonsterKill_EnemyManager>.Instance.GetAppearanceRate(arraySpawnEnemyType[i]);
		}
	}
	public int GetSpawnCnt()
	{
		return spawnCnt;
	}
	public MonsterKill_EnemyManager.EnemyType GetRandomSpawnEnemyType()
	{
		int num = UnityEngine.Random.Range(0, totalAppearanceRate);
		int num2 = 0;
		for (int i = 0; i < arraySpawnEnemyType.Length; i++)
		{
			num2 += SingletonCustom<MonsterKill_EnemyManager>.Instance.GetAppearanceRate(arraySpawnEnemyType[i]);
			if (num < num2)
			{
				return arraySpawnEnemyType[i];
			}
		}
		return arraySpawnEnemyType[Random.Range(0, arraySpawnEnemyType.Length)];
	}
	public Vector3 GetRandomGeneratePos()
	{
		float x = UnityEngine.Random.Range(base.transform.position.x - areaSize.x * 0.5f * INTO_AREA_MAG, base.transform.position.x + areaSize.x * 0.5f * INTO_AREA_MAG);
		float z = UnityEngine.Random.Range(base.transform.position.z - areaSize.z * 0.5f * INTO_AREA_MAG, base.transform.position.z + areaSize.z * 0.5f * INTO_AREA_MAG);
		return new Vector3(x, 0f, z);
	}
	public bool GetIsIntoArea(Vector3 _pos)
	{
		if (base.transform.position.x - areaSize.x * 0.5f * INTO_AREA_MAG < _pos.x && base.transform.position.x + areaSize.x * 0.5f * INTO_AREA_MAG > _pos.x && base.transform.position.z - areaSize.z * 0.5f * INTO_AREA_MAG < _pos.z)
		{
			return base.transform.position.z + areaSize.z * 0.5f * INTO_AREA_MAG > _pos.z;
		}
		return false;
	}
	public bool GetIsBigMonsterAppearance()
	{
		return isBigMonsterAppearance;
	}
	public void SetIsBigMonsterAppearance(bool _isBigMonsterAppearance)
	{
		isBigMonsterAppearance = _isBigMonsterAppearance;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(base.transform.position, areaSize);
	}
}
