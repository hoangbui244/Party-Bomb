using System;
using System.Collections;
using UnityEngine;
public class MorphingRace_RunButton : MonoBehaviour
{
	[SerializeField]
	[Header("Aボタン_ON")]
	private SpriteRenderer button_A_ON;
	[SerializeField]
	[Header("Aボタン_OFF")]
	private SpriteRenderer button_A_OFF;
	public void Init()
	{
		button_A_ON.SetAlpha(0f);
		button_A_ON.gameObject.SetActive(value: true);
		button_A_OFF.gameObject.SetActive(value: false);
	}
	public void PressButton()
	{
		button_A_ON.gameObject.SetActive(value: false);
		button_A_OFF.gameObject.SetActive(value: true);
		SingletonCustom<AudioManager>.Instance.SePlay("se_shorttrack_button");
	}
	public void ReleaseButton()
	{
		button_A_ON.gameObject.SetActive(value: true);
		button_A_OFF.gameObject.SetActive(value: false);
	}
	public void Show()
	{
		if (button_A_ON.gameObject.activeSelf)
		{
			StartCoroutine(SetAlphaColor(button_A_ON, 0.5f));
		}
		else if (button_A_OFF.gameObject.activeSelf)
		{
			StartCoroutine(SetAlphaColor(button_A_OFF, 0.5f));
		}
	}
	public void Hide()
	{
		if (button_A_ON.gameObject.activeSelf)
		{
			StartCoroutine(SetAlphaColor(button_A_ON, 0.5f, 0f, _isFadeOut: true));
		}
		else if (button_A_OFF.gameObject.activeSelf)
		{
			StartCoroutine(SetAlphaColor(button_A_OFF, 0.5f, 0f, _isFadeOut: true));
		}
		LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
		{
			button_A_ON.SetAlpha(0f);
			button_A_ON.gameObject.SetActive(value: true);
			button_A_OFF.gameObject.SetActive(value: false);
		});
	}
	private IEnumerator SetAlphaColor(SpriteRenderer _spriteRenderer, float _fadeTime, float _delayTime = 0f, bool _isFadeOut = false)
	{
		float time = 0f;
		Color color = Color.white;
		float startAlpha = 0f;
		float endAlpha = 1f;
		if (_isFadeOut)
		{
			startAlpha = 1f;
			endAlpha = 0f;
		}
		yield return new WaitForSeconds(_delayTime);
		while (time < _fadeTime)
		{
			color = _spriteRenderer.color;
			color.a = Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime);
			_spriteRenderer.color = color;
			time += Time.deltaTime;
			yield return null;
		}
	}
}
