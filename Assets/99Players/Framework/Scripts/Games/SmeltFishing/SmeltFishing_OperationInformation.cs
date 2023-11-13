using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class SmeltFishing_OperationInformation : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer[] spriteRenderers;
	[SerializeField]
	private TextMeshPro[] texts;
	private bool isShown;
	public bool IsShow
	{
		get;
		private set;
	}
	public void Init()
	{
		SpriteRenderer[] array = spriteRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetAlpha(0f);
		}
		TextMeshPro[] array2 = texts;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetAlpha(0f);
		}
		IsShow = false;
	}
	public void Show()
	{
		if (!isShown)
		{
			isShown = true;
			IsShow = true;
			base.gameObject.SetActive(value: true);
			StartCoroutine(FadeProcess(spriteRenderers, 1f, 0.5f));
			StartCoroutine(FadeProcess(texts, 1f, 0.5f));
		}
	}
	public void Hide()
	{
		if (base.gameObject.activeSelf)
		{
			StopAllCoroutines();
			StartCoroutine(FadeProcess(spriteRenderers, 0f, 0.5f));
			StartCoroutine(FadeProcess(texts, 0f, 0.5f, 0f, delegate
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
			SpriteRenderer[] array = spriteRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetAlpha(0f);
			}
			TextMeshPro[] array2 = texts;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].SetAlpha(0f);
			}
			base.gameObject.SetActive(value: false);
		}
	}
	private IEnumerator FadeProcess(SpriteRenderer[] _spriteArray, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		float time = 0f;
		float[] startAlpha = new float[_spriteArray.Length];
		for (int i = 0; i < startAlpha.Length; i++)
		{
			startAlpha[i] = _spriteArray[i].color.a;
		}
		yield return new WaitForSeconds(_delayTime);
		while (time < _fadeTime)
		{
			for (int j = 0; j < _spriteArray.Length; j++)
			{
				if (_spriteArray[j] != null)
				{
					_spriteArray[j].SetAlpha(Mathf.Lerp(startAlpha[j], _setAlpha, time / _fadeTime));
				}
			}
			time += Time.deltaTime;
			yield return null;
		}
		for (int k = 0; k < _spriteArray.Length; k++)
		{
			_spriteArray[k].SetAlpha(_setAlpha);
		}
		_callback?.Invoke();
	}
	private IEnumerator FadeProcess(TextMeshPro[] _textArray, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		float time = 0f;
		float[] startAlpha = new float[_textArray.Length];
		for (int i = 0; i < startAlpha.Length; i++)
		{
			startAlpha[i] = _textArray[i].color.a;
		}
		yield return new WaitForSeconds(_delayTime);
		while (time < _fadeTime)
		{
			for (int j = 0; j < _textArray.Length; j++)
			{
				if (_textArray[j] != null)
				{
					_textArray[j].SetAlpha(Mathf.Lerp(startAlpha[j], _setAlpha, time / _fadeTime));
				}
			}
			time += Time.deltaTime;
			yield return null;
		}
		for (int k = 0; k < _textArray.Length; k++)
		{
			_textArray[k].SetAlpha(_setAlpha);
		}
		_callback?.Invoke();
	}
}
