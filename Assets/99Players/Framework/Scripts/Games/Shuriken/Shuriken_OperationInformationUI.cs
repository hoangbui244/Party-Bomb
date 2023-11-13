using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_OperationInformationUI : MonoBehaviour
{
	[SerializeField]
	[DisplayName("フレ\u30fcム")]
	private SpriteRenderer frame;
	[SerializeField]
	[DisplayName("ボタン")]
	private SpriteRenderer button;
	[SerializeField]
	[DisplayName("テキスト")]
	private TextMeshPro text;
	private bool isShown;
	public bool IsShow
	{
		get;
		private set;
	}
	public void Init()
	{
		frame.SetAlpha(0f);
		button.SetAlpha(0f);
		text.SetAlpha(0f);
		IsShow = false;
	}
	public void Show()
	{
		if (!isShown)
		{
			isShown = true;
			IsShow = true;
			base.gameObject.SetActive(value: true);
			StartCoroutine(FadeProcess(frame, 1f, 0.5f));
			StartCoroutine(FadeProcess(button, 1f, 0.5f));
			StartCoroutine(FadeProcess(text, 1f, 0.5f));
		}
	}
	public void Hide()
	{
		if (base.gameObject.activeSelf)
		{
			StopAllCoroutines();
			StartCoroutine(FadeProcess(frame, 0f, 0.5f));
			StartCoroutine(FadeProcess(button, 0f, 0.5f));
			StartCoroutine(FadeProcess(text, 0f, 0.5f, 0f, delegate
			{
				base.gameObject.SetActive(value: false);
				IsShow = false;
			}));
		}
	}
	public void ForceHide()
	{
		if (IsShow)
		{
			IsShow = false;
			StopAllCoroutines();
			frame.SetAlpha(0f);
			button.SetAlpha(0f);
			text.SetAlpha(0f);
			base.gameObject.SetActive(value: false);
		}
	}
	private IEnumerator FadeProcess(SpriteRenderer sp, float alpha, float duration, float delayedTime = 0f, Action callback = null)
	{
		float time = 0f;
		float startAlpha = sp.color.a;
		yield return new WaitForSeconds(delayedTime);
		while (time < duration)
		{
			sp.SetAlpha(Mathf.Lerp(startAlpha, alpha, time / duration));
			time += Time.deltaTime;
			yield return null;
		}
		sp.SetAlpha(alpha);
		callback?.Invoke();
	}
	private IEnumerator FadeProcess(TextMeshPro tmp, float alpha, float duration, float delayedTime = 0f, Action callback = null)
	{
		float time = 0f;
		float startAlpha = tmp.color.a;
		yield return new WaitForSeconds(delayedTime);
		while (time < duration)
		{
			tmp.SetAlpha(Mathf.Lerp(startAlpha, alpha, time / duration));
			time += Time.deltaTime;
			yield return null;
		}
		tmp.SetAlpha(alpha);
		callback?.Invoke();
	}
}
