using UnityEngine;
public class Scuba_ItemObject : MonoBehaviour
{
	public enum ItemType
	{
		Fish,
		BigFish,
		Other,
		Other_Skinned
	}
	private const float NORAML_BOUNDS_SIZE = 0.1229834f;
	private const float EASY_MAG_Z_SIZE_MAG = 3f;
	private const float EASY_MAG_Z_ADD = 0.5f;
	[SerializeField]
	[Header("スコア")]
	private int score = 100;
	[SerializeField]
	[Header("撮影の奥行き判定の緩和倍率（大きさによって調整）")]
	private float easyMagZ = 1f;
	[SerializeField]
	private ItemType itemType;
	[SerializeField]
	private MeshRenderer itemMeshRenderer;
	[SerializeField]
	private SkinnedMeshRenderer itemSkinnedMeshRenderer;
	[SerializeField]
	private Scuba_Fish fishScript;
	[SerializeField]
	private Scuba_BigFish bigFishScript;
	[SerializeField]
	private Transform forceUiPosAnchor;
	private Scuba_PrefabFishData prefabData;
	private bool[] isFounds = new bool[4];
	private Vector3[] viewportPoints = new Vector3[4];
	private Vector3[] screenPoints = new Vector3[4];
	private Vector3[] charaEyeVectors = new Vector3[4];
	private Vector3[] charaViewportPoints = new Vector3[4];
	public void Init()
	{
		if (fishScript != null)
		{
			fishScript.SetItemObject(this);
		}
	}
	public int GetScore()
	{
		return score;
	}
	public Vector3 GetPos()
	{
		return base.transform.position;
	}
	public Vector3 GetCenterPos()
	{
		return GetBounds(_isLocal: false).center;
	}
	public bool GetIsFound(int _charaNo)
	{
		return isFounds[_charaNo];
	}
	public Vector3 GetViewportPoint(int _charaNo)
	{
		return viewportPoints[_charaNo];
	}
	public Vector3 GetScreenPoint(int _charaNo)
	{
		return screenPoints[_charaNo];
	}
	public Vector3 GetCharaEyeVectors(int _charaNo)
	{
		return charaEyeVectors[_charaNo];
	}
	public Vector3 GetCharaViewportPoint(int _charaNo)
	{
		return charaViewportPoints[_charaNo];
	}
	public Vector3 GetFoundEffectCenter()
	{
		return GetBounds(_isLocal: false).center;
	}
	public Vector3 GetFoundEffectScale()
	{
		return Vector3.one * (GetBoundsMaxSize() / 0.1229834f);
	}
	public float GetBoundsMaxSize()
	{
		Vector3 extents = GetBounds(_isLocal: false).extents;
		return Mathf.Max(Mathf.Max(extents.x, extents.y), extents.z);
	}
	public Bounds GetBounds(bool _isLocal)
	{
		switch (itemType)
		{
		case ItemType.Fish:
			return fishScript.GetBounds(_isLocal);
		case ItemType.BigFish:
			return bigFishScript.GetBounds(_isLocal);
		case ItemType.Other:
			return itemMeshRenderer.bounds;
		default:
			return itemSkinnedMeshRenderer.bounds;
		}
	}
	public Vector3 GetUiPos(Vector3 _cameraPos, Vector3 _cameraForward)
	{
		if (forceUiPosAnchor != null)
		{
			return forceUiPosAnchor.position;
		}
		Vector3 vector = GetBounds(_isLocal: false).center;
		if ((itemType == ItemType.Fish || itemType == ItemType.BigFish) && Mathf.Abs(Vector3.Dot(Vector3.up, (vector - _cameraPos).normalized)) > 0.7f)
		{
			bool num = Vector3.Dot(base.transform.right, _cameraForward) < 0f;
			Vector3 a = Quaternion.Euler(0f, -90f, 0f) * base.transform.right;
			float num2 = GetBounds(_isLocal: true).extents.z * GetModelScale().z;
			if (prefabData != null)
			{
				num2 += prefabData.foundUiRightOffset;
			}
			vector = ((!num) ? (vector - a * num2) : (vector + a * num2));
			return vector;
		}
		vector.y += GetBounds(_isLocal: false).extents.y;
		if (prefabData != null)
		{
			vector.y += prefabData.foundUiUpperOffset;
		}
		return vector;
	}
	public Vector3 GetModelScale()
	{
		switch (itemType)
		{
		case ItemType.Fish:
			return fishScript.GetBoneScale();
		case ItemType.BigFish:
			return bigFishScript.GetBoneScale();
		case ItemType.Other:
			return itemMeshRenderer.transform.lossyScale;
		default:
			return itemSkinnedMeshRenderer.rootBone.lossyScale;
		}
	}
	public float GetEasyMagZ()
	{
		return easyMagZ;
	}
	public void SetIsFound(int _charaNo, bool _value)
	{
		isFounds[_charaNo] = _value;
	}
	public void SetViewportPoint(int _charaNo, Vector3 _point)
	{
		_point.z /= easyMagZ;
		viewportPoints[_charaNo] = _point;
	}
	public void SetScreenPoint(int _charaNo, Vector3 _point)
	{
		_point.z /= easyMagZ;
		screenPoints[_charaNo] = _point;
	}
	public void SetCharaEyeVectors(int _charaNo, Vector3 _vec)
	{
		_vec.z /= easyMagZ;
		charaEyeVectors[_charaNo] = _vec;
	}
	public void SetCharaViewportPoint(int _charaNo, Vector3 _point)
	{
		_point.z /= easyMagZ;
		charaViewportPoints[_charaNo] = _point;
	}
	public void SetPrefabFishData(Scuba_PrefabFishData _Data)
	{
		prefabData = _Data;
		if (prefabData != null)
		{
			score = prefabData.score;
		}
	}
	public void SettingEasyMagZ()
	{
		if (fishScript != null)
		{
			easyMagZ = GetBoundsMaxSize() * 3f + 0.5f;
		}
		if (prefabData != null)
		{
			easyMagZ *= prefabData.foundRangeMag;
		}
	}
}
