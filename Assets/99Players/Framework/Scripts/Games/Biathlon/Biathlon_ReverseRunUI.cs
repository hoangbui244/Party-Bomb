using UnityEngine;
public class Biathlon_ReverseRunUI : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer renderer;
	[SerializeField]
	private Sprite englishSprite;
	public void Init()
	{
		Hide();
		if (Localize_Define.Language == Localize_Define.LanguageType.English)
		{
			renderer.sprite = englishSprite;
		}
	}
	public void Show()
	{
		if (!renderer.enabled)
		{
			renderer.enabled = true;
		}
	}
	public void Hide()
	{
		if (renderer.enabled)
		{
			renderer.enabled = false;
		}
	}
}
