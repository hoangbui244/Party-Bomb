using UnityEngine;
public class MonsterRace_Aipoint : MonoBehaviour
{
	public Transform[] points;
	public bool isPrevFixPoint;
	private int aiPointID;
	public void SetAiPointID(int _aiPointID)
	{
		aiPointID = _aiPointID;
	}
	public int GetPointMax()
	{
		return points.Length;
	}
	public Vector3 GetPointPosition()
	{
		return points[Random.Range(0, points.Length)].position;
	}
	public Vector3 GetPointPosition(int _idx)
	{
		return points[_idx].position;
	}
	public int GetRandomPointIdx()
	{
		return UnityEngine.Random.Range(0, points.Length);
	}
	private void OnTriggerEnter(Collider _collider)
	{
	}
}
