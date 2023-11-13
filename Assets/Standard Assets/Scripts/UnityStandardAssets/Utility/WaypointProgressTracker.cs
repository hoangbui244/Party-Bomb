using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class WaypointProgressTracker : MonoBehaviour
	{
		public enum WhyUse
		{
			AI,
			Pos,
			Other
		}

		public enum ProgressStyle
		{
			SmoothAlongRoute,
			PointToPoint
		}

		public WhyUse whyUse = WhyUse.Other;

		[SerializeField]
		private WaypointCircuit circuit;

		[SerializeField]
		private float lookAheadForTargetOffset = 5f;

		[SerializeField]
		private float lookAheadForTargetFactor = 0.1f;

		[SerializeField]
		private float lookAheadForSpeedOffset = 10f;

		[SerializeField]
		private float lookAheadForSpeedFactor = 0.2f;

		[SerializeField]
		private ProgressStyle progressStyle;

		[SerializeField]
		private float pointToPointThreshold = 4f;

		public Transform target;

		private WaypointCircuit.RoutePoint debugPoint1;

		private WaypointCircuit.RoutePoint debugPoint2;

		public float progressDistance;

		private int progressNum;

		private Vector3 lastPosition;

		private float speed;

		public bool canReverseRun;

		private float oldReverseTime;

		private float reverseInterval = 0.2f;

		public WaypointCircuit.RoutePoint getRescuePoint => circuit.GetRoutePoint(runDistance);

		public WaypointCircuit setCircuit
		{
			set
			{
				circuit = value;
			}
		}

		public WaypointCircuit getCircuit => circuit;

		public WaypointCircuit.RoutePoint targetPoint
		{
			get;
			private set;
		}

		public WaypointCircuit.RoutePoint speedPoint
		{
			get;
			private set;
		}

		public WaypointCircuit.RoutePoint progressPoint
		{
			get;
			private set;
		}

		public float runDistance
		{
			get
			{
				if (progressStyle != 0)
				{
					return 0f;
				}
				float num = progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor * speed;
				float num2 = 1f;
				Vector3 vector = target.position - base.transform.position;
				vector.y = 0f;
				Vector3 normalized = vector.normalized;
				Vector3 rhs = progressPoint.direction;
				rhs.y = 0f;
				rhs = rhs.normalized;
				num2 = Vector3.Dot(normalized, rhs);
				return num - vector.magnitude * num2;
			}
		}

		private void Start()
		{
			if (circuit == null)
			{
				if (whyUse == WhyUse.AI)
				{
					circuit = GameObject.FindGameObjectWithTag("Circuit_AI").GetComponent<WaypointCircuit>();
				}
				else if (whyUse == WhyUse.Pos)
				{
					circuit = GameObject.FindGameObjectWithTag("Circuit_Pos").GetComponent<WaypointCircuit>();
				}
			}
			if (target == null)
			{
				target = new GameObject(base.name + " Waypoint Target").transform;
			}
			Reset();
		}

		public void Reset()
		{
			progressDistance = 0f;
			progressNum = 0;
			if (progressStyle == ProgressStyle.PointToPoint)
			{
				target.position = circuit.Waypoints[progressNum].position;
				target.rotation = circuit.Waypoints[progressNum].rotation;
			}
		}

		private void Update()
		{
			if (progressStyle == ProgressStyle.SmoothAlongRoute)
			{
				if (circuit == null)
				{
					UnityEngine.Debug.Log("nullera");
				}
				progressPoint = circuit.GetRoutePoint(progressDistance);
				Vector3 lhs = progressPoint.position - base.transform.position;
				float magnitude = lhs.magnitude;
				if (Vector3.Dot(lhs, progressPoint.direction) < 0f)
				{
					float num = magnitude * 0.5f;
					progressDistance += num;
				}
				else if (canReverseRun && Time.time - oldReverseTime > reverseInterval && magnitude > 1f)
				{
					oldReverseTime = Time.time;
					float num2 = magnitude / 5f;
					progressDistance -= num2;
				}
				if (Time.deltaTime > 0f)
				{
					speed = Mathf.Lerp(speed, (lastPosition - base.transform.position).magnitude / Time.deltaTime, Time.deltaTime);
				}
				target.position = circuit.GetRoutePoint(progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor * speed).position;
				target.rotation = Quaternion.LookRotation(circuit.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor * speed).direction);
				debugPoint1 = circuit.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor * speed);
				debugPoint2 = circuit.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor * speed + 0.1f);
				lastPosition = base.transform.position;
			}
			else
			{
				progressPoint = circuit.GetRoutePoint(progressDistance);
				Vector3 lhs2 = progressPoint.position - base.transform.position;
				float magnitude2 = lhs2.magnitude;
				if (Vector3.Dot(lhs2, progressPoint.direction) < 0f)
				{
					progressDistance += magnitude2;
				}
				else if (canReverseRun)
				{
					progressDistance -= magnitude2;
				}
				if ((target.position - base.transform.position).magnitude < pointToPointThreshold)
				{
					progressNum = (progressNum + 1) % circuit.Waypoints.Length;
				}
				target.position = circuit.Waypoints[progressNum].position;
				target.rotation = circuit.Waypoints[progressNum].rotation;
				lastPosition = base.transform.position;
			}
		}

		private void OnDrawGizmos()
		{
			if ((bool)target && Application.isPlaying)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine(base.transform.position, target.position);
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(target.position, target.position + target.forward);
				Gizmos.color = Color.red;
				Gizmos.DrawLine(target.position, debugPoint1.position);
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(debugPoint1.position, debugPoint1.position + (-debugPoint1.position + debugPoint2.position).normalized);
			}
		}
	}
}
