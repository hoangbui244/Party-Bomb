using UnityEngine;
public class Shooting_Shitajiki : MonoBehaviour
{
	[SerializeField]
	[Header("下敷きの親")]
	private GameObject parentObj;
	private SpriteRenderer shitajikiSprite;
	[SerializeField]
	[Header("英語版の時の親の位置")]
	private Vector3 parentPos;
	[SerializeField]
	[Header("英語版の下敷きの位置")]
	private Vector3 shitajikiPos;
	[SerializeField]
	[Header("英語版の時の大きさ")]
	private Vector2 ShitajikiSize;
	[SerializeField]
	[Header("同時に動かすもの")]
	private GameObject subObject;
	[SerializeField]
	[Header("同時に動かすものの位置")]
	private Vector3 subObjPosition;
	private void Awake()
	{
		shitajikiSprite = GetComponent<SpriteRenderer>();
		if (Localize_Define.Language != 0)
		{
			if (parentObj != null)
			{
				parentObj.transform.localPosition = parentPos;
				shitajikiSprite.gameObject.transform.localPosition = shitajikiPos;
			}
			if (subObject != null)
			{
				subObject.transform.localPosition = subObjPosition;
			}
			shitajikiSprite.size = ShitajikiSize;
		}
	}
	private void Start()
	{
	}
}
