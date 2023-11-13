using UnityEngine;
public class Scuba_BigFish : MonoBehaviour
{
	private const float LOD_SIZE = 1.2f;
	private const float RESET_SPEED = 0.2f;
	[SerializeField]
	private LODGroup lodGroup;
	[SerializeField]
	private GameObject fishObj;
	[SerializeField]
	private SkinnedMeshRenderer fishSkinnedMeshRenderer;
	private Vector3 boneScale;
	[SerializeField]
	[Header("開始、目標位置")]
	private Transform startEndAnchorA;
	[SerializeField]
	private Transform startEndAnchorB;
	[SerializeField]
	[Header("泳ぐ速度")]
	private float moveSpeed = 0.2f;
	[SerializeField]
	[Header("開始から何秒で出現するか")]
	private float showDelayTime;
	private Vector3 moveDir;
	private bool isShow;
	public void Init()
	{
		if (UnityEngine.Random.Range(0, 2) == 1)
		{
			base.transform.position = startEndAnchorA.position;
			moveDir = (startEndAnchorB.position - startEndAnchorA.position).normalized;
		}
		else
		{
			base.transform.position = startEndAnchorB.position;
			moveDir = (startEndAnchorA.position - startEndAnchorB.position).normalized;
		}
		boneScale = fishSkinnedMeshRenderer.rootBone.lossyScale;
		base.transform.right = -moveDir;
		base.gameObject.SetActive(value: false);
	}
	public void UpdateMethod()
	{
		if (!isShow)
		{
			if (showDelayTime < SingletonCustom<Scuba_GameManager>.Instance.GameTime)
			{
				isShow = true;
				base.gameObject.SetActive(value: true);
			}
		}
		else
		{
			base.transform.position += moveDir * moveSpeed * Time.deltaTime;
		}
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
	public void AddLODSize(float _addSize)
	{
		lodGroup.size = 1.2f + _addSize;
	}
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(startEndAnchorA.position, startEndAnchorB.position);
	}
}
