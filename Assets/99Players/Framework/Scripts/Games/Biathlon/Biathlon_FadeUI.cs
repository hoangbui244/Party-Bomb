using System;
using System.Collections;
using UnityEngine;
public class Biathlon_FadeUI : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer renderer;
	private bool isFading;
	public void Init()
	{
		renderer.SetAlpha(0f);
	}
	public void PlayFade(float duration, float keep, Action fadeIn, Action fadeOut)
	{
		if (!isFading)
		{
			StartCoroutine(Fading(duration, keep, fadeIn, fadeOut));
		}
	}
	private IEnumerator Fading(float duration, float keep, Action fadeIn, Action fadeOut)
	{
		duration = Mathf.Max(duration, 0.01f);
		float elapsed2 = 0f;
		while (elapsed2 < duration)
		{
			elapsed2 += Time.deltaTime;
			float a = Mathf.Clamp01(elapsed2 / duration);
			renderer.SetAlpha(a);
			yield return null;
		}
		fadeOut?.Invoke();
		yield return new WaitForSeconds(keep);
		elapsed2 = 0f;
		while (elapsed2 < duration)
		{
			elapsed2 += Time.deltaTime;
			float a2 = 1f - Mathf.Clamp01(elapsed2 / duration);
			renderer.SetAlpha(a2);
			yield return null;
		}
		fadeIn?.Invoke();
	}
}
