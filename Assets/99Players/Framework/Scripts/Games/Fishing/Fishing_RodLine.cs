using UnityEngine;
public class Fishing_RodLine : MonoBehaviour
{
	[SerializeField]
	[Header("ラインのジョイントポイント")]
	private Transform[] lineJointPoint;
	[SerializeField]
	[Header("ロッドのライン描画用のLineRenderer")]
	private LineRenderer line;
	private void Awake()
	{
		line.positionCount = lineJointPoint.Length;
		float num3 = line.startWidth = (line.endWidth = 0.02f);
	}
	private void LateUpdate()
	{
		for (int i = 0; i < lineJointPoint.Length; i++)
		{
			line.SetPosition(i, lineJointPoint[i].transform.position);
		}
	}
}
