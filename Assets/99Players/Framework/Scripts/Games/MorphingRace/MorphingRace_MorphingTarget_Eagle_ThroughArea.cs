using UnityEngine;
public class MorphingRace_MorphingTarget_Eagle_ThroughArea : MonoBehaviour
{
	private Transform throughArea;
	[SerializeField]
	[Header("通り抜けるエリア配列")]
	private Transform[] arrayThroughAnchor;
	public Transform GetThroughArea()
	{
		return throughArea;
	}
	public void SetThroughArea()
	{
		throughArea = arrayThroughAnchor[Random.Range(0, arrayThroughAnchor.Length)];
	}
	public bool CheckPassThroughArea(Vector3 _pos)
	{
		return throughArea.position.z <= _pos.z;
	}
}
