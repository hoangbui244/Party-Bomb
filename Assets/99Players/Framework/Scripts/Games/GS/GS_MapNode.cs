using System;
using UnityEngine;
public class GS_MapNode : MonoBehaviour
{
	[Serializable]
	public struct PointData
	{
		public int idx;
		public Transform anchor;
	}
	[SerializeField]
	[Header("移動時優先隣接ポイント")]
	private int priorityIdx = -1;
	[SerializeField]
	[Header("隣接ポイント配列")]
	private PointData[] arrayRelatedPoint;
	[SerializeField]
	[Header("直接移動入力：上")]
	private int moveUpIdx = -1;
	[SerializeField]
	[Header("直接移動入力：下")]
	private int moveDownIdx = -1;
	[SerializeField]
	[Header("直接移動入力：左")]
	private int moveLeftIdx = -1;
	[SerializeField]
	[Header("直接移動入力：右")]
	private int moveRightIdx = -1;
	public int PriorityIdx => priorityIdx;
	public PointData[] ArrayRelatedPoint => arrayRelatedPoint;
	public int UpIdx => moveUpIdx;
	public int DownIdx => moveDownIdx;
	public int LeftIdx => moveLeftIdx;
	public int RightIdx => moveRightIdx;
	public Vector3 GetPriorityPos()
	{
		return arrayRelatedPoint[priorityIdx].anchor.position;
	}
}
