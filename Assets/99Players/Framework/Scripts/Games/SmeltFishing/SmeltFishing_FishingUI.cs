using System.Collections;
using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_FishingUI : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer smeltHitRenderer;
	[SerializeField]
	private SpriteRenderer smeltGetRenderer;
	[SerializeField]
	[DisplayName("表示までにかかる時間")]
	private float showDuration = 0.2f;
	[SerializeField]
	[DisplayName("表示状態を保つ時間")]
	private float keepDuration = 1f;
	[SerializeField]
	[DisplayName("非表示までにかかる時間")]
	private float hideDuration = 0.2f;
	private SpriteRenderer currentRenderer;
	private Coroutine coroutine;
	public void Init()
	{
		smeltHitRenderer.enabled = false;
		smeltGetRenderer.enabled = false;
	}
	public void ShowHit()
	{
		HideCurrentIfNeeded();
		coroutine = StartCoroutine(ShowAndHideAnimation(smeltHitRenderer));
	}
	public void ShowGet()
	{
		HideCurrentIfNeeded();
		coroutine = StartCoroutine(ShowAndHideAnimation(smeltGetRenderer));
	}
	public void HideCurrentIfNeeded()
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
			coroutine = null;
		}
		if (currentRenderer != null)
		{
			currentRenderer.SetAlpha(0f);
			currentRenderer = null;
		}
	}
	private IEnumerator ShowAndHideAnimation(SpriteRenderer sp)
	{
		currentRenderer = sp;
		sp.SetAlpha(0f);
		sp.enabled = true;
		float elapsed3 = 0f;
		while (elapsed3 < showDuration)
		{
			elapsed3 += Time.deltaTime;
			float a = elapsed3 / showDuration;
			sp.SetAlpha(a);
			yield return null;
		}
		sp.SetAlpha(1f);
		elapsed3 = 0f;
		while (elapsed3 < keepDuration)
		{
			elapsed3 += Time.deltaTime;
			yield return null;
		}
		elapsed3 = 0f;
		while (elapsed3 < hideDuration)
		{
			elapsed3 += Time.deltaTime;
			float num = elapsed3 / hideDuration;
			sp.SetAlpha(1f - num);
			yield return null;
		}
		sp.SetAlpha(0f);
		sp.enabled = false;
	}
}
