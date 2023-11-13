using System.Collections;
using UnityEngine;
using UnityStandardAssets.Utility;
public class RingToss_Train : MonoBehaviour
{
	[SerializeField]
	private WaypointCircuit waypoint;
	[SerializeField]
	private Transform[] trainAnchors;
	[SerializeField]
	private float[] trainLengthes;
	[SerializeField]
	private Transform[] targetAnchors;
	private float nowDistance;
	private float speed;
	private float timer;
	public void Init()
	{
		SpeedUpdate();
		StartCoroutine(_DelayInit());
	}
	private IEnumerator _DelayInit()
	{
		yield return null;
		nowDistance = UnityEngine.Random.Range(0f, waypoint.Length);
		UpdateMethod();
	}
	public void UpdateMethod()
	{
		timer += Time.deltaTime;
		if (timer > 3f)
		{
			timer = 0f;
			SpeedUpdate();
		}
		nowDistance += speed * Time.deltaTime;
		float num = nowDistance;
		float num2 = nowDistance;
		for (int i = 0; i < trainAnchors.Length; i++)
		{
			num2 -= trainLengthes[i];
			WaypointCircuit.RoutePoint routePoint = waypoint.GetRoutePoint((num + num2) / 2f);
			trainAnchors[i].position = routePoint.position;
			trainAnchors[i].forward = routePoint.direction;
			num = num2;
		}
	}
	public Transform GetTargetAnchor(int _no)
	{
		return targetAnchors[_no];
	}
	private void SpeedUpdate()
	{
		switch (UnityEngine.Random.Range(0, 3))
		{
		case 0:
			speed = 3f;
			break;
		case 1:
			speed = 5f;
			break;
		case 2:
			speed = 7f;
			break;
		}
	}
}
