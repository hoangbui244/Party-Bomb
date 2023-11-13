using System;
using UnityEngine;
using UnityStandardAssets.Utility;
public class Canoe_Course : MonoBehaviour
{
	[Serializable]
	private struct Goal
	{
		[HideInInspector]
		public bool isUse;
		public Transform anchor;
	}
	private int[] arrayRandomIdx;
	[SerializeField]
	[Header("プレイヤ\u30fcアンカ\u30fc配列")]
	private Transform[] arrayPlayerAnchor;
	[SerializeField]
	[Header("順位判定用のWaypointCircuit")]
	private WaypointCircuit arrayWaypointCircuit_Pos;
	[SerializeField]
	[Header("AI用のWaypointCircuitクラス")]
	private Canoe_WayPointCircuit wayPointCircuit_AI;
	[SerializeField]
	[Header("ゴ\u30fcルまでの距離")]
	private float distanceToGoal;
	[SerializeField]
	[Header("ゴ\u30fcル")]
	private BoxCollider goalObj;
	[SerializeField]
	[Header("ゴ\u30fcル配列")]
	private Goal[] arrayGoalAnchor;
	private bool[] arrayIsDebugUse;
	public void Init()
	{
		arrayRandomIdx = new int[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
		for (int i = 0; i < arrayRandomIdx.Length; i++)
		{
			arrayRandomIdx[i] = i;
		}
		arrayRandomIdx.Shuffle();
		wayPointCircuit_AI.Init();
	}
	public Transform GetPlayerAnchor(int _playerNo)
	{
		return arrayPlayerAnchor[Array.IndexOf(arrayRandomIdx, _playerNo)];
	}
	public WaypointCircuit GetWaypointCircuitPos()
	{
		return arrayWaypointCircuit_Pos;
	}
	public WaypointCircuit GetWaypointCircuitAI(int _playerNo)
	{
		return wayPointCircuit_AI.GetArrayWaypointCircuit(Array.IndexOf(arrayRandomIdx, _playerNo));
	}
	public float GetDistanceToGoal()
	{
		return distanceToGoal;
	}
	public Vector3 GetNearGoalAnchorPos(Vector3 _pos, bool _isDebug = false)
	{
		float num = 1000f;
		int num2 = 0;
		for (int i = 0; i < arrayGoalAnchor.Length; i++)
		{
			if (!arrayGoalAnchor[i].isUse)
			{
				float num3 = CalcManager.Length(arrayGoalAnchor[i].anchor.position, _pos);
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
		}
		UnityEngine.Debug.Log("nearIdx " + num2.ToString());
		arrayGoalAnchor[num2].isUse = true;
		return arrayGoalAnchor[num2].anchor.position;
	}
	public Vector3 DebugJustBeforeGoal(Vector3 _pos)
	{
		if (arrayIsDebugUse == null)
		{
			arrayIsDebugUse = new bool[4];
		}
		float num = 1000f;
		int num2 = 0;
		for (int i = 0; i < arrayGoalAnchor.Length; i++)
		{
			if (!arrayIsDebugUse[i])
			{
				float num3 = CalcManager.Length(arrayGoalAnchor[i].anchor.position, _pos);
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
		}
		UnityEngine.Debug.Log("nearIdx " + num2.ToString());
		arrayIsDebugUse[num2] = true;
		Vector3 position = arrayGoalAnchor[num2].anchor.position;
		position.z = goalObj.bounds.min.z - 5f;
		return position;
	}
	private void OnDrawGizmos()
	{
		for (int i = 0; i < arrayPlayerAnchor.Length; i++)
		{
			switch (i)
			{
			case 0:
				Gizmos.color = Color.green;
				break;
			case 1:
				Gizmos.color = Color.red;
				break;
			case 2:
				Gizmos.color = Color.blue;
				break;
			case 3:
				Gizmos.color = Color.yellow;
				break;
			}
			Gizmos.DrawWireSphere(arrayPlayerAnchor[i].position, 0.5f);
		}
	}
}
