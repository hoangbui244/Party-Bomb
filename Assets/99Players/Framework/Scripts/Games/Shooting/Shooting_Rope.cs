using UnityEngine;
public class Shooting_Rope : MonoBehaviour
{
	[SerializeField]
	[Header("中間地点")]
	private GameObject[] vertices;
	private LineRenderer rope;
	private void Start()
	{
		rope = GetComponent<LineRenderer>();
		rope.positionCount = vertices.Length;
	}
	private void Update()
	{
		int num = 0;
		GameObject[] array = vertices;
		foreach (GameObject gameObject in array)
		{
			rope.SetPosition(num, gameObject.transform.position);
			num++;
		}
	}
}
