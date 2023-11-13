using UnityEngine;
using UnityStandardAssets.Utility;
public class Canoe_CourseManager : SingletonCustom<Canoe_CourseManager>
{
	[SerializeField]
	private Canoe_Course course;
	public void Init()
	{
		course.Init();
	}
	public Transform GetPlayerAnchor(int _playerNo)
	{
		return course.GetPlayerAnchor(_playerNo);
	}
	public WaypointCircuit GetWaypointCircuitPos()
	{
		return course.GetWaypointCircuitPos();
	}
	public WaypointCircuit GetWaypointCircuitAI(int _playerNo)
	{
		return course.GetWaypointCircuitAI(_playerNo);
	}
	public float GetDistanceToGoal()
	{
		return course.GetDistanceToGoal();
	}
	public Vector3 GetNearGoalAnchorPos(Vector3 _pos)
	{
		return course.GetNearGoalAnchorPos(_pos);
	}
	public Vector3 DebugJustBeforeGoal(Vector3 _pos)
	{
		return course.DebugJustBeforeGoal(_pos);
	}
}
