using System;
using UnityEngine;
public class Fishing_Field : SingletonCustom<Fishing_Field>
{
	[Serializable]
	public struct PathRootData
	{
		[Header("パス開始地点")]
		public Transform startPoint;
		[Header("パス終了地点")]
		public Transform endPoint;
		[Header("パス地点")]
		public Transform[] pathPoint;
	}
	[SerializeField]
	[Header("カメラアンカ\u30fc")]
	private Transform cameraAnchor;
	[SerializeField]
	[Header("パスル\u30fcトデ\u30fcタ")]
	private PathRootData[] pathRootData;
	public PathRootData GetNearStartPathRoot(Vector3 _originPos, Vector3 _targetPos)
	{
		float num = -1f;
		float num2 = -1f;
		int pathId_Start = -1;
		int pathId_End = -1;
		for (int i = 0; i < pathRootData.Length; i++)
		{
			if (i == 0)
			{
				num = Vector3.Distance(_originPos, pathRootData[i].startPoint.position);
				pathId_Start = pathRootData[i].startPoint.GetComponent<Fishing_AIPathData>().pathNo;
				num2 = Vector3.Distance(_targetPos, pathRootData[i].endPoint.position);
				pathId_End = pathRootData[i].endPoint.GetComponent<Fishing_AIPathData>().pathNo;
				continue;
			}
			if (num > Vector3.Distance(_originPos, pathRootData[i].startPoint.position))
			{
				num = Vector3.Distance(_originPos, pathRootData[i].startPoint.position);
				pathId_Start = pathRootData[i].startPoint.GetComponent<Fishing_AIPathData>().pathNo;
			}
			if (num2 > Vector3.Distance(_targetPos, pathRootData[i].endPoint.position))
			{
				num2 = Vector3.Distance(_targetPos, pathRootData[i].endPoint.position);
				pathId_End = pathRootData[i].endPoint.GetComponent<Fishing_AIPathData>().pathNo;
			}
		}
		return GetSelectPathIdRootData(pathId_Start, pathId_End);
	}
	private PathRootData GetSelectPathIdRootData(int _pathId_Start, int _pathId_End)
	{
		for (int i = 0; i < pathRootData.Length; i++)
		{
			if (pathRootData[i].startPoint.GetComponent<Fishing_AIPathData>().pathNo == _pathId_Start && pathRootData[i].endPoint.GetComponent<Fishing_AIPathData>().pathNo == _pathId_End)
			{
				return pathRootData[i];
			}
		}
		return default(PathRootData);
	}
	public Transform GetCameraTransform()
	{
		return cameraAnchor;
	}
	private void OnDrawGizmos()
	{
		if (pathRootData == null)
		{
			return;
		}
		Gizmos.color = Color.green;
		for (int i = 0; i < pathRootData.Length; i++)
		{
			for (int j = 0; j < pathRootData[i].pathPoint.Length; j++)
			{
				if (pathRootData[i].pathPoint[j] != null)
				{
					Gizmos.DrawWireSphere(pathRootData[i].pathPoint[j].position, 0.25f);
				}
			}
		}
		Gizmos.color = Color.red;
		for (int k = 0; k < pathRootData.Length; k++)
		{
			for (int l = 0; l < pathRootData[k].pathPoint.Length && l != pathRootData[k].pathPoint.Length - 1; l++)
			{
				if (pathRootData[k].pathPoint[l] != null)
				{
					Gizmos.DrawLine(pathRootData[k].pathPoint[l].position, pathRootData[k].pathPoint[l + 1].position);
				}
			}
		}
	}
}
