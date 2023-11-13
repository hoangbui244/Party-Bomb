using UnityEngine;
public class BigMerchantCustomerOrderList : MonoBehaviour
{
	[SerializeField]
	[Header("フレ\u30fcム")]
	private SpriteRenderer frame;
	[SerializeField]
	[Header("アイコンル\u30fcト")]
	private Transform itemIconRoot;
	[SerializeField]
	[Header("アイコン画像")]
	private SpriteRenderer[] arrayIcon;
	[SerializeField]
	[Header("設定画像")]
	private Sprite[] arraySpIcon;
	private BigMerchantCustomer customer;
	public void SetCustomer(BigMerchantCustomer _customer)
	{
		customer = _customer;
	}
	public void Show()
	{
		base.gameObject.SetActive(value: true);
		frame.size = new Vector2(75f + (float)(customer.ListColorType.Count - 1) * 70f, frame.size.y);
		itemIconRoot.SetLocalPositionX(70f - (float)(customer.ListColorType.Count - 1) * 35f);
		for (int i = 0; i < arrayIcon.Length; i++)
		{
			if (i < customer.ListColorType.Count)
			{
				arrayIcon[i].gameObject.SetActive(value: true);
				arrayIcon[i].sprite = arraySpIcon[(int)customer.ListColorType[i]];
			}
			else
			{
				arrayIcon[i].gameObject.SetActive(value: false);
			}
		}
		base.transform.SetLocalScale(0f, 0f, 1f);
		LeanTween.cancel(base.gameObject);
		LeanTween.scale(base.gameObject, Vector3.one, 0.15f).setEaseOutQuart();
	}
	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
