using UnityEngine;
public class SmeltFishing_FishingBucket : MonoBehaviour
{
	public enum PLACE_TYPE
	{
		CharacterHand,
		Field
	}
	[SerializeField]
	private GameObject bucket;
	[SerializeField]
	[Header("取っ手")]
	private GameObject bucketTakeer;
	[SerializeField]
	[Header("マテリアル")]
	private Material[] arrayMaterial;
	[SerializeField]
	[Header("バケツに入れた魚影配列")]
	private SmeltFishing_SmeltShadow[] arrayBucketSmelt;
	private int smeltCount;
	private readonly Vector3[] arraySize = new Vector3[2]
	{
		new Vector3(0.667f, 0.667f, 0.667f),
		new Vector3(1f, 1f, 1f)
	};
	private readonly Vector3[] arrayAngle = new Vector3[2]
	{
		new Vector3(0f, 225f, 0f),
		new Vector3(0f, 0f, 0f)
	};
	private readonly Vector3[] arrayTakeerAngle = new Vector3[2]
	{
		new Vector3(270f, 0f, 0f),
		new Vector3(45f, 0f, 0f)
	};
	public void Init()
	{
		for (int i = 0; i < arrayBucketSmelt.Length; i++)
		{
			switch (i % 3)
			{
			case 0:
				arrayBucketSmelt[i].transform.localScale = new Vector3(1f, 1f, 1f);
				break;
			case 1:
				arrayBucketSmelt[i].transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
				break;
			case 2:
				arrayBucketSmelt[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				break;
			}
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < smeltCount; i++)
		{
			if (arrayBucketSmelt[i].gameObject.activeSelf)
			{
				arrayBucketSmelt[i].UpdateMethod();
			}
		}
	}
	public void SetMaterial(int _userType)
	{
		bucket.GetComponent<MeshRenderer>().material = arrayMaterial[_userType];
		bucketTakeer.GetComponent<MeshRenderer>().material = arrayMaterial[_userType];
	}
	public void AddSmelt(int count)
	{
		UnityEngine.Debug.Log("count\u3000" + count.ToString());
		if (smeltCount != arrayBucketSmelt.Length)
		{
			if (smeltCount + count > arrayBucketSmelt.Length)
			{
				count = arrayBucketSmelt.Length - smeltCount;
			}
			for (int i = smeltCount; i < smeltCount + count; i++)
			{
				arrayBucketSmelt[i].gameObject.SetActive(value: true);
				arrayBucketSmelt[i].Init();
			}
			smeltCount += count;
		}
	}
	public void SetAnchor(PLACE_TYPE _placeType, Transform _parent)
	{
		base.transform.parent = _parent;
		base.transform.localPosition = Vector3.zero;
		base.transform.localScale = arraySize[(int)_placeType];
		base.transform.localEulerAngles = arrayAngle[(int)_placeType];
		bucketTakeer.transform.localEulerAngles = arrayTakeerAngle[(int)_placeType];
	}
}
