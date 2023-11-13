using System;
using UnityEngine;
public class Scuba_Fish : MonoBehaviour
{
	private const float LOD_SIZE = 0.7f;
	private const float RESET_RADIUS = 2f;
	private const float RESET_ANGLE = 0f;
	private const float RESET_SIZE_XZ = 1f;
	private const float RESET_SPEED = 0.2f;
	[SerializeField]
	private LODGroup lodGroup;
	private Scuba_ItemObject itemObject;
	private GameObject fishObj;
	private SkinnedMeshRenderer fishSkinnedMeshRenderer;
	private Vector3 boneScale;
	[SerializeField]
	[Header("泳ぐ円形ル\u30fcトの半径")]
	private float routeCircleRadius = 2f;
	[SerializeField]
	[Header("泳ぐ円形ル\u30fcトの向き")]
	private float routeCircleAngle;
	[SerializeField]
	[Header("円形のサイズ（楕円具合の設定）")]
	private float routeCircleSizeX = 1f;
	[SerializeField]
	private float routeCircleSizeZ = 1f;
	[SerializeField]
	[Header("泳ぐ速度")]
	private float moveSpeed = 0.2f;
	private Vector3 routeCircleCenterPos;
	private Vector3 routeCircleCenterDir;
	private float routeCircleDistance;
	private float moveDistance;
	private bool isClockwise;
	private bool isInit;
	public void Init()
	{
		routeCircleCenterDir = Quaternion.Euler(0f, routeCircleAngle, 0f) * Vector3.right;
		routeCircleCenterPos = base.transform.position;
		routeCircleCenterPos.x += routeCircleCenterDir.x * routeCircleSizeX * routeCircleRadius;
		routeCircleCenterPos.z += routeCircleCenterDir.z * routeCircleSizeZ * routeCircleRadius;
		routeCircleDistance = routeCircleRadius * 2f * (float)Math.PI;
		moveDistance = UnityEngine.Random.Range(0f, routeCircleDistance);
		isClockwise = (UnityEngine.Random.Range(0, 2) == 1);
		isInit = true;
	}
	public void UpdateMethod()
	{
		moveDistance += moveSpeed * Time.deltaTime;
		float num = moveDistance / routeCircleDistance % 1f;
		if (isClockwise)
		{
			base.transform.SetEulerAnglesY(num * 360f - 90f + routeCircleAngle);
		}
		else
		{
			num = 1f - num;
			base.transform.SetEulerAnglesY(num * 360f + 90f + routeCircleAngle);
		}
		Vector3 b = Quaternion.Euler(0f, num * 360f, 0f) * routeCircleCenterDir * routeCircleRadius;
		b.x *= routeCircleSizeX;
		b.z *= routeCircleSizeZ;
		base.transform.position = routeCircleCenterPos + b;
	}
	public void CreateFishObj(GameObject _prefab)
	{
		fishObj = UnityEngine.Object.Instantiate(_prefab);
		Scuba_PrefabFishData component = fishObj.GetComponent<Scuba_PrefabFishData>();
		fishSkinnedMeshRenderer = component.skinnedMeshRenderer;
		boneScale = fishSkinnedMeshRenderer.rootBone.lossyScale;
		fishObj.transform.SetParent(base.transform);
		fishObj.transform.localPosition = Vector3.zero;
		fishObj.transform.localRotation = Quaternion.identity;
		LOD[] lODs = lodGroup.GetLODs();
		lODs[0].renderers[0] = fishSkinnedMeshRenderer;
		lodGroup.SetLODs(lODs);
		lodGroup.size = 0.7f;
		itemObject.SetPrefabFishData(component);
		itemObject.SettingEasyMagZ();
	}
	public Bounds GetBounds(bool _isLocal)
	{
		if (_isLocal)
		{
			return fishSkinnedMeshRenderer.localBounds;
		}
		return fishSkinnedMeshRenderer.bounds;
	}
	public Vector3 GetBoneScale()
	{
		return boneScale;
	}
	public void SetItemObject(Scuba_ItemObject _item)
	{
		itemObject = _item;
	}
	public void AddLODSize(float _addSize)
	{
		lodGroup.size = 0.7f + _addSize;
	}
	public void OnDrawGizmos()
	{
		if (!isInit)
		{
			routeCircleCenterDir = Quaternion.Euler(0f, routeCircleAngle, 0f) * Vector3.right;
			routeCircleCenterPos = base.transform.position;
			routeCircleCenterPos.x += routeCircleCenterDir.x * routeCircleSizeX * routeCircleRadius;
			routeCircleCenterPos.z += routeCircleCenterDir.z * routeCircleSizeZ * routeCircleRadius;
			routeCircleDistance = routeCircleRadius * 2f * (float)Math.PI;
		}
		Gizmos.color = Color.yellow;
		float d = routeCircleRadius;
		Vector3 a = routeCircleCenterDir;
		Vector3 a2 = routeCircleCenterPos;
		int num = Mathf.FloorToInt(routeCircleDistance / 0.5f);
		float y = 360f / (float)num;
		Vector3 vector = a * d;
		for (int i = 0; i < num; i++)
		{
			Vector3 from = a2 + new Vector3(vector.x * routeCircleSizeX, 0f, vector.z * routeCircleSizeZ);
			vector = Quaternion.Euler(0f, y, 0f) * vector;
			Vector3 to = a2 + new Vector3(vector.x * routeCircleSizeX, 0f, vector.z * routeCircleSizeZ);
			Gizmos.DrawLine(from, to);
		}
	}
}
