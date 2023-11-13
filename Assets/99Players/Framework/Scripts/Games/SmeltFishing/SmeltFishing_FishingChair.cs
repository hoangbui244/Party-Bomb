using UnityEngine;
public class SmeltFishing_FishingChair : MonoBehaviour
{
	[SerializeField]
	private GameObject chair;
	public void Init()
	{
		Hide();
	}
	public void Show()
	{
		chair.SetActive(value: true);
	}
	public void Hide()
	{
		chair.SetActive(value: false);
	}
}
