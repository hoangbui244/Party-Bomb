using System.Collections.Generic;
using UnityEngine;
public class Fishing_CalcStandFishPoint : MonoBehaviour
{
	[SerializeField]
	[Header("BoxCastをするアンカ\u30fc")]
	private Transform boxCastAnchor;
	[SerializeField]
	[Header("BoxCastのギズモ表示")]
	private bool isEnable;
	[SerializeField]
	[Header("Gizmoで表示する時のカラ\u30fc")]
	private Color gizmoColor;
	private int userDataNo = -1;
	private bool isHitBoxCast;
	private RaycastHit hit;
	private const float BOX_CAST_DISTANCE = 10f;
	private float boxCastScale;
	private float sphereOverlapScale;
	private List<Vector3> tempStandFishPoint = new List<Vector3>();
	private Vector3 tempFishStandPoint;
	private Collider[] tempOverlapSphereColliders;
	public void Init(int _userDataNo, Vector3 _fishPoint)
	{
		userDataNo = _userDataNo;
		base.transform.position = _fishPoint;
		base.transform.SetLocalPositionY(0f);
		boxCastScale = base.transform.lossyScale.x * 0.1f;
		sphereOverlapScale = base.transform.lossyScale.x * 0.2f;
	}
	public List<Vector3> GetStandFishPoint()
	{
		tempStandFishPoint.Clear();
		if (Physics.Raycast(base.transform.position, Vector3.down, out hit, float.PositiveInfinity, LayerMask.GetMask(FishingDefinition.LayerField)) && hit.collider.gameObject.tag != FishingDefinition.TagObject)
		{
			return tempStandFishPoint;
		}
		for (int i = 0; i < 360; i++)
		{
			base.transform.SetLocalEulerAnglesY(i);
			isHitBoxCast = Physics.Raycast(boxCastAnchor.position, Vector3.down, out hit, 10f, LayerMask.GetMask(FishingDefinition.LayerField));
			if (!isHitBoxCast)
			{
				continue;
			}
			tempOverlapSphereColliders = Physics.OverlapSphere(hit.point, sphereOverlapScale, LayerMask.GetMask(FishingDefinition.LayerWall, FishingDefinition.LayerCharacter));
			if (tempOverlapSphereColliders.Length == 0)
			{
				if (hit.collider.gameObject.tag == FishingDefinition.TagField)
				{
					tempStandFishPoint.Add(hit.point);
				}
				continue;
			}
			for (int j = 0; j < tempOverlapSphereColliders.Length; j++)
			{
				if (tempOverlapSphereColliders[j].gameObject.tag == FishingDefinition.TagPlayer && tempOverlapSphereColliders[j].gameObject.GetComponent<Fishing_Character>().GetUserDataNo() == userDataNo && hit.collider.gameObject.tag == FishingDefinition.TagField)
				{
					tempStandFishPoint.Add(hit.point);
				}
			}
		}
		return tempStandFishPoint;
	}
	public bool CheckObstacleStandPoint(Vector3 _standPoint)
	{
		tempFishStandPoint = _standPoint;
		tempOverlapSphereColliders = Physics.OverlapSphere(tempFishStandPoint, sphereOverlapScale, LayerMask.GetMask(FishingDefinition.LayerWall, FishingDefinition.LayerCharacter));
		if (tempOverlapSphereColliders.Length == 0)
		{
			return false;
		}
		if (tempOverlapSphereColliders.Length == 1 && tempOverlapSphereColliders[0].gameObject.GetComponent<Fishing_Character>() != null && tempOverlapSphereColliders[0].gameObject.GetComponent<Fishing_Character>().GetUserDataNo() == userDataNo)
		{
			return false;
		}
		return true;
	}
	private void OnDrawGizmos()
	{
		if (!isEnable)
		{
			return;
		}
		Gizmos.color = gizmoColor;
		for (int i = 0; i < tempStandFishPoint.Count; i++)
		{
			if (i == 0)
			{
				Gizmos.DrawWireSphere(tempStandFishPoint[i], 0.05f);
			}
			if (i >= tempStandFishPoint.Count - 1)
			{
				Gizmos.DrawWireSphere(tempStandFishPoint[i], 0.05f);
			}
			else
			{
				Gizmos.DrawLine(tempStandFishPoint[i], tempStandFishPoint[i + 1]);
			}
		}
		Gizmos.DrawWireSphere(tempFishStandPoint, sphereOverlapScale);
	}
}
