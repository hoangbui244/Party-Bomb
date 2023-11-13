using UnityEngine;
public class FireworksSlatbox : MonoBehaviour
{
	[SerializeField]
	[Header("グラフィック")]
	private GameObject[] arrayItemPrefab;
	[SerializeField]
	[Header("収納している玉タイプ")]
	private FireworksBall.ItemType colorType;
	[SerializeField]
	[Header("生成位置アンカ\u30fc")]
	private Transform anchorCreate;
	[SerializeField]
	[Header("表示本体")]
	private GameObject objModel;
	[SerializeField]
	[Header("本体付属")]
	private GameObject[] arrayItemModel;
	private Vector3 baseScale;
	public FireworksBall.ItemType ColorType => colorType;
	public void Init(FireworksBall.ItemType _type)
	{
		colorType = _type;
		GameObject gameObject = UnityEngine.Object.Instantiate(arrayItemPrefab[(int)_type], anchorCreate);
		gameObject.transform.SetLocalPosition(0f, 0f, 0f);
		gameObject.transform.localEulerAngles = arrayItemPrefab[(int)_type].transform.localEulerAngles;
		gameObject.SetActive(value: true);
		gameObject.transform.parent = objModel.transform;
	}
	private void Start()
	{
		for (int i = 0; i < arrayItemModel.Length; i++)
		{
			arrayItemModel[i].transform.parent = objModel.transform;
		}
		baseScale = objModel.transform.localScale;
	}
	public void Shake()
	{
		LeanTween.cancel(objModel);
		objModel.transform.localScale = baseScale * 1.1f;
		LeanTween.scale(objModel, baseScale, 0.2f).setEaseOutBack();
	}
	private void OnDestroy()
	{
		LeanTween.cancel(objModel);
	}
}
