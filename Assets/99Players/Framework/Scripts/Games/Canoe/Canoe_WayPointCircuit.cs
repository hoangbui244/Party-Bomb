using UnityEngine;
using UnityStandardAssets.Utility;
public class Canoe_WayPointCircuit : MonoBehaviour
{
	[SerializeField]
	[Header("WaypointCircuit配列")]
	private WaypointCircuit[] arrayWaypointCircuit;
	public void Init()
	{
		for (int i = 0; i < arrayWaypointCircuit.Length; i++)
		{
			arrayWaypointCircuit[i].gameObject.SetActive(value: false);
		}
	}
	public WaypointCircuit GetArrayWaypointCircuit(int _idx)
	{
		return arrayWaypointCircuit[_idx];
	}
}
