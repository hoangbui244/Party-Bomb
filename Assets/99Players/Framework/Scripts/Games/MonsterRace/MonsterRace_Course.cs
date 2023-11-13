using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Utility;
public class MonsterRace_Course : MonoBehaviour
{
	public Transform[] createAnchor;
	[SerializeField]
	private WaypointCircuit posCircuit;
	[SerializeField]
	private WaypointCircuit[] aiCircuit;
	public Transform carAnchor;
	public Transform courseRightTop;
	public Transform courseLeftBottom;
	public Transform oneCameraAnchor;
	[SerializeField]
	[Header("内側の浮き")]
	public GameObject BuoyIn;
	[SerializeField]
	[Header("外側の浮き")]
	public GameObject BuoyOut;
	public WaypointCircuit GetPosCircuit()
	{
		return posCircuit;
	}
	public WaypointCircuit GetAiCircuit()
	{
		return aiCircuit[UnityEngine.Random.Range(0, aiCircuit.Length)];
	}
	public WaypointCircuit GetAiCircuit(int _no)
	{
		return aiCircuit[_no % aiCircuit.Length];
	}
	public IEnumerator TimeMove(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}
