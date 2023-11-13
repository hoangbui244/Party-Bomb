using UnityEngine;
public class MonsterKill_Player_SpawnAnchorPattern : MonoBehaviour
{
	[SerializeField]
	private Transform[] arrayPlayerSpawnAnchor;
	public void ShufflePlayerSpawnAnchor()
	{
		arrayPlayerSpawnAnchor.Shuffle();
	}
	public Transform GetPlayerAnchor(int _playerNo)
	{
		return arrayPlayerSpawnAnchor[_playerNo];
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		for (int i = 0; i < arrayPlayerSpawnAnchor.Length; i++)
		{
			Gizmos.DrawWireSphere(arrayPlayerSpawnAnchor[i].position, 0.5f);
		}
	}
}
