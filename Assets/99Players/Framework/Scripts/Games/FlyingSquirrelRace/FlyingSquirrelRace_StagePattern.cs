using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_StagePattern : DecoratedMonoBehaviour
{
	public enum ConnectionPoint
	{
		Top,
		Middle,
		Bottom
	}
	private const float ObjectDeactivateDistance = 20f;
	[SerializeField]
	[DisplayName("開始接続ポイント")]
	private ConnectionPoint startConnectionPoint = ConnectionPoint.Middle;
	[SerializeField]
	[DisplayName("終了接続ポイント")]
	private ConnectionPoint endConnectionPoint = ConnectionPoint.Middle;
	[SerializeField]
	[DisplayName("配置オブジェクト")]
	private FlyingSquirrelRace_StageObject[] stageObjects;
	[SerializeField]
	[DisplayName("誘導線(ウェイポイント)")]
	private FlyingSquirrelRace_WayPoint[] wayPoints;
	private FlyingSquirrelRace_Stage owner;
	private Vector3[] points;
	private float[] distances;
	private int point0Index;
	private int point1Index;
	private int point2Index;
	private int point3Index;
	private float inversePoint;
	private Vector3 point0;
	private Vector3 point1;
	private Vector3 point2;
	private Vector3 point3;
	private Vector3 initPos;
	private int arrayIdx;
	public ConnectionPoint StartConnectionPoint => startConnectionPoint;
	public ConnectionPoint EndConnectionPoint => endConnectionPoint;
	public bool IsCurrent
	{
		get
		{
			if (owner.GetStageObjectsRoot().localPosition.x <= -10f * (float)arrayIdx)
			{
				return owner.GetStageObjectsRoot().localPosition.x > -10f * (float)(arrayIdx + 1);
			}
			return false;
		}
	}
	public bool IsDeactivateDistance
	{
		get;
		set;
	}
	public void Initialize(FlyingSquirrelRace_Stage stage, int layer, int _arrayIdx)
	{
		base.gameObject.SetActive(value: true);
		owner = stage;
		FlyingSquirrelRace_StageObject[] array = stageObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Initialize(stage, this, layer);
		}
		CachePointsAndDistances();
		IsDeactivateDistance = false;
		initPos = base.transform.position;
		arrayIdx = _arrayIdx;
	}
	public float GetTargetHeight(float offset)
	{
		float num = 0f - owner.GetStageObjectsRoot().localPosition.x - 10f * (float)arrayIdx;
		num += offset;
		num = Mathf.Clamp(num, 0f, 10f);
		return GetRoutePosition(num).y;
	}
	public void CollectWayPoints()
	{
	}
	public void FixedUpdateMethod(float speed)
	{
		if (initPos.x - base.transform.position.x >= 20f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		FlyingSquirrelRace_StageObject[] array = stageObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].FixedUpdateMethod(speed);
		}
	}
	private void CachePointsAndDistances()
	{
		int num = wayPoints.Length;
		points = new Vector3[num + 2];
		distances = new float[num + 2];
		Vector3 position = base.transform.position;
		Vector3 vector = position.Y((float y) => y + GetConnectionOffset(StartConnectionPoint));
		Vector3 vector2 = position.X((float x) => x + 10f).Y((float y) => y + GetConnectionOffset(EndConnectionPoint));
		points[0] = vector;
		points[points.Length - 1] = vector2;
		for (int i = 0; i < num; i++)
		{
			points[i + 1] = wayPoints[i].Point;
		}
		float num2 = 0f;
		for (int j = 0; j < distances.Length; j++)
		{
			Vector3 a = points[j];
			Vector3 b = points[(j + 1) % points.Length];
			distances[j] = num2;
			num2 += (a - b).magnitude;
		}
	}
	private float GetConnectionOffset(ConnectionPoint cp)
	{
		switch (cp)
		{
		case ConnectionPoint.Top:
			return 0.4f;
		case ConnectionPoint.Middle:
			return 0f;
		case ConnectionPoint.Bottom:
			return -0.4f;
		default:
			return 0f;
		}
	}
	public Vector3 GetRoutePosition(float dist)
	{
		int i = 0;
		for (dist = Mathf.Min(dist, 10f); distances[i] < dist; i++)
		{
		}
		point1Index = ((i > 0) ? (i - 1) : 0);
		point2Index = i;
		inversePoint = Mathf.InverseLerp(distances[point1Index], distances[point2Index], dist);
		point0Index = ((i - 2 >= 0) ? (i - 2) : 0);
		point3Index = ((i + 1 < points.Length) ? (i + 1) : i);
		point0 = points[point0Index];
		point1 = points[point1Index];
		point2 = points[point2Index];
		point3 = points[point3Index];
		return CatmullRom(point0, point1, point2, point3, inversePoint);
	}
	private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
	{
		return 0.5f * (2f * p1 + (-p0 + p2) * i + (2f * p0 - 5f * p1 + 4f * p2 - p3) * i * i + (-p0 + 3f * p1 - 3f * p2 + p3) * i * i * i);
	}
}
