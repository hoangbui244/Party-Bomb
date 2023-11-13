using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class ShortTrack_DashButtonPress : MonoBehaviour
{
	[SerializeField]
	[Header("演出用のル\u30fcトアンカ\u30fc")]
	private Transform rootAnchor;
	[SerializeField]
	[Header("Aだけ押すボタンの親")]
	private GameObject onlyButton;
	[SerializeField]
	[Header("Aボタンのみの画像_ON")]
	private GameObject buttonOnly_A_ON;
	[SerializeField]
	[Header("Aボタンのみの画像_OFF")]
	private GameObject buttonOnly_A_OFF;
	[SerializeField]
	[Header("操作説明の吹き出し画像")]
	private SpriteRenderer infoControlBaloon;
	[SerializeField]
	[Header("操作説明のAボタン画像")]
	private SpriteRenderer infoControlButton_A;
	[SerializeField]
	[Header("操作説明のBボタン画像")]
	private SpriteRenderer infoControlButton_B;
	[SerializeField]
	[Header("操作説明の文字")]
	private TextMeshPro infoControlText;
	[SerializeField]
	[Header("ボタン入力OFF時の色")]
	private Color hideButtonColor;
	private readonly float SHOW_BUTTON_TIME = 1f;
	private SpriteRenderer buttonOnly_A_ON_Renderer;
	private SpriteRenderer buttonOnly_A_OFF_Renderer;
	[SerializeField]
	[Header("最初の色")]
	private Color startColor;
	[SerializeField]
	[Header("終了時の色")]
	private Color endColor;
	private float movePosTime;
	private Vector3 shouButtonPos;
	[SerializeField]
	[Header("流れるために必要な位置")]
	private GameObject hideButtonPos;
	private bool isFirstShowInfo;
	public void Init()
	{
		buttonOnly_A_OFF_Renderer = buttonOnly_A_OFF.GetComponent<SpriteRenderer>();
		buttonOnly_A_ON_Renderer = buttonOnly_A_ON.GetComponent<SpriteRenderer>();
		shouButtonPos = onlyButton.transform.localPosition;
		buttonOnly_A_ON.SetActive(value: true);
		buttonOnly_A_OFF.SetActive(value: false);
		rootAnchor.SetLocalPositionY(-800f);
		base.gameObject.SetActive(value: false);
		infoControlBaloon.SetAlpha(0f);
		infoControlButton_A.SetAlpha(0f);
		infoControlButton_B.SetAlpha(0f);
		infoControlText.SetAlpha(0f);
		SetInfoControlActive(_isActive: true);
	}
	public void PressButtonHide()
	{
		movePosTime = Mathf.Clamp(movePosTime + Time.deltaTime / 0.3f, 0f, 1f);
		onlyButton.transform.localPosition = Vector3.Lerp(shouButtonPos, hideButtonPos.transform.localPosition, movePosTime);
		buttonOnly_A_OFF_Renderer.color = Color.Lerp(startColor, endColor, movePosTime);
		if (movePosTime >= 0.999f)
		{
			onlyButton.SetActive(value: false);
			PressOnlyButtonUp_A();
		}
	}
	public void PressButtonDisplay()
	{
		onlyButton.SetActive(value: true);
		movePosTime = Mathf.Clamp(movePosTime - Time.deltaTime / 0.3f, 0f, 1f);
		onlyButton.transform.localPosition = Vector3.Lerp(shouButtonPos, hideButtonPos.transform.localPosition, movePosTime);
		buttonOnly_A_ON_Renderer.color = Color.Lerp(startColor, endColor, movePosTime);
		buttonOnly_A_OFF_Renderer.color = startColor;
	}
	public void PressOnlyButtonDown_A()
	{
		buttonOnly_A_ON.SetActive(value: false);
		buttonOnly_A_OFF.SetActive(value: true);
		SingletonCustom<AudioManager>.Instance.SePlay("se_shorttrack_button");
	}
	public void PressOnlyButtonUp_A()
	{
		buttonOnly_A_ON.SetActive(value: true);
		buttonOnly_A_OFF.SetActive(value: false);
	}
	public void Show()
	{
		rootAnchor.SetLocalPositionY(-400f);
		base.gameObject.SetActive(value: true);
		LeanTween.moveLocalY(rootAnchor.gameObject, -375f, SHOW_BUTTON_TIME).setEaseOutQuart();
	}
	public void Hide()
	{
		LeanTween.moveLocalY(rootAnchor.gameObject, -800f, SHOW_BUTTON_TIME).setEaseOutQuart().setOnComplete((Action)delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}
	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}
	public void ShowInfoControlAnimation()
	{
		if (!isFirstShowInfo)
		{
			infoControlBaloon.SetAlpha(0f);
			infoControlButton_A.SetAlpha(0f);
			infoControlButton_B.SetAlpha(0f);
			infoControlText.SetAlpha(0f);
			StartCoroutine(SetAlphaColor(infoControlBaloon, 0.5f));
			StartCoroutine(SetAlphaColor(infoControlButton_A, 0.5f));
			StartCoroutine(SetAlphaColor(infoControlButton_B, 0.5f));
			StartCoroutine(SetAlphaColor(infoControlText, 0.5f));
		}
	}
	public void HideInfoControlAnimation()
	{
		if (!isFirstShowInfo)
		{
			infoControlBaloon.SetAlpha(1f);
			infoControlButton_A.SetAlpha(1f);
			infoControlButton_B.SetAlpha(1f);
			infoControlText.SetAlpha(1f);
			StartCoroutine(SetAlphaColor(infoControlBaloon, 0.5f, 0f, _isFadeOut: true));
			StartCoroutine(SetAlphaColor(infoControlButton_A, 0.5f, 0f, _isFadeOut: true));
			StartCoroutine(SetAlphaColor(infoControlButton_B, 0.5f, 0f, _isFadeOut: true));
			StartCoroutine(SetAlphaColor(infoControlText, 0.5f, 0f, _isFadeOut: true));
			isFirstShowInfo = true;
			LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
			{
				SetInfoControlActive(_isActive: false);
			});
		}
	}
	public void SetInfoControlActive(bool _isActive)
	{
		infoControlBaloon.gameObject.SetActive(_isActive);
		infoControlButton_A.gameObject.SetActive(_isActive);
		infoControlButton_B.gameObject.SetActive(_isActive);
		infoControlText.gameObject.SetActive(_isActive);
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
	private IEnumerator SetAlphaColor(TextMeshPro _textMeshPro, float _fadeTime, float _delayTime = 0f, bool _isFadeOut = false)
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
			color = _textMeshPro.color;
			color.a = Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime);
			_textMeshPro.color = color;
			time += Time.deltaTime;
			yield return null;
		}
	}
}
