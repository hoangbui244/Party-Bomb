using UnityEngine;
public class MorphingRace_MorphingTarget_Mouse_NavObstacle : MonoBehaviour
{
	[SerializeField]
	[Header("経路探索させないためのオブジェクトを設定する数")]
	private int navObstacleCnt;
	[SerializeField]
	[Header("NavMeshObstacleのオブジェクト配列")]
	private GameObject[] arrayNavMeshObstacle;
	private void Awake()
	{
		for (int i = 0; i < arrayNavMeshObstacle.Length; i++)
		{
			arrayNavMeshObstacle[i].SetActive(value: false);
		}
	}
	public void SetNavMeshObstacleShuffle()
	{
		arrayNavMeshObstacle.Shuffle();
	}
	public void SetArrayNavMeshObjectActive(bool _isActive)
	{
		for (int i = 0; i < navObstacleCnt && i < arrayNavMeshObstacle.Length; i++)
		{
			arrayNavMeshObstacle[i].SetActive(_isActive);
		}
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Vector3 from = base.transform.position + new Vector3(-2.5f, 0.1f, -25f);
		Vector3 to = base.transform.position + new Vector3(-2.5f, 0.1f, 25f);
		Vector3 from2 = base.transform.position + new Vector3(2.5f, 0.1f, -25f);
		Vector3 to2 = base.transform.position + new Vector3(2.5f, 0.1f, 25f);
		Gizmos.DrawLine(from, to);
		Gizmos.DrawLine(from2, to2);
	}
}
