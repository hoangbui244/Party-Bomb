using UnityEngine;
public class FireworksBall : MonoBehaviour
{
	public enum ItemType
	{
		MEDICAL_HERB,
		NECKLACE,
		SWORD,
		SHIELD,
		PORTION,
		BOOK,
		CANE,
		RING,
		ARROW,
		MAX
	}
	[SerializeField]
	[Header("表示オブジェクト")]
	private GameObject[] arrayObj;
	private readonly float DESTROY_TIME = 0.75f;
	private float setTime;
	public ItemType Color
	{
		get;
		set;
	}
	public void Init(ItemType _type)
	{
		Color = _type;
		for (int i = 0; i < arrayObj.Length; i++)
		{
			arrayObj[i].SetActive(i == (int)_type);
		}
	}
	public void Set()
	{
		setTime = DESTROY_TIME;
	}
	private void Update()
	{
		if (setTime > 0f)
		{
			setTime -= Time.deltaTime;
			if (setTime <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
