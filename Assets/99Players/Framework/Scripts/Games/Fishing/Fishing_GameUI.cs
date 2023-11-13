using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
public class Fishing_GameUI : SingletonCustom<Fishing_GameUI>
{
	[SerializeField]
	[Header("ゲ\u30fcム時間")]
	private CommonGameTimeUI_Font_Time commonGameTime;
	[FormerlySerializedAs("userUIData_Group1_Anchor")]
	[SerializeField]
	[Header("１組目のユ\u30fcザ\u30fcのUIデ\u30fcタアンカ\u30fc")]
	private GameObject userUIDataAnchor;
	[FormerlySerializedAs("userUIData_Group1")]
	[SerializeField]
	[Header("１組目のユ\u30fcザ\u30fcのUIデ\u30fcタ")]
	private Fishing_UserUI[] userUIData;
	[FormerlySerializedAs("operationExplanation_Single")]
	[SerializeField]
	[Header("操作説明(１人時)")]
	private GameObject operationExplanationForSingle;
	[FormerlySerializedAs("operationExplanation_Multi")]
	[SerializeField]
	[Header("操作説明(複数時)")]
	private GameObject operationExplanationForMulti;
	[SerializeField]
	[Header("操作説明の下敷き画像")]
	private SpriteRenderer infoControlUnderlay;
	[SerializeField]
	[Header("操作説明のボタン画像")]
	private SpriteRenderer infoControlButton;
	[SerializeField]
	[Header("操作説明の文字")]
	private TextMeshPro infoControlText;
	[SerializeField]
	[Header("操作説明のルビ文字")]
	private TextMeshPro infoControlRubyText;
	[SerializeField]
	[Header("ポ\u30fcズUI")]
	private GameObject pauseUI;
	public void Init()
	{
		SetUserUIDataActivate();
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			operationExplanationForSingle.SetActive(value: true);
			operationExplanationForMulti.SetActive(value: false);
			pauseUI.SetActive(value: true);
		}
		else
		{
			operationExplanationForSingle.SetActive(value: false);
			operationExplanationForMulti.SetActive(value: true);
			pauseUI.SetActive(value: false);
		}
		infoControlUnderlay.SetAlpha(0f);
		infoControlButton.SetAlpha(0f);
		infoControlText.SetAlpha(0f);
		infoControlRubyText.SetAlpha(0f);
	}
	public void UpdateGameTime(float time)
	{
		commonGameTime.SetTime(time);
	}
	public void SetUserUIDataActivate()
	{
		userUIDataAnchor.SetActive(value: true);
		userUIDataAnchor.transform.SetLocalPosition(0f, 0f, 0f);
	}
	public void SetUserUIData(int userNo, FishingDefinition.User user)
	{
		userUIData[userNo].Init(userNo, user);
	}
	public void SetPoint(int userNo, int point)
	{
		userUIData[userNo].SetPoint(point);
	}
	public void SetFishCount(int userNo, int fishCount)
	{
		userUIData[userNo].SetFishCount(fishCount);
	}
	public void ShowInfoControlBalloon()
	{
		StartCoroutine(FadeProcess(infoControlUnderlay, 1f, 0.5f));
		StartCoroutine(FadeProcess(infoControlButton, 1f, 0.5f));
		StartCoroutine(FadeProcess(infoControlText, 1f, 0.5f));
		StartCoroutine(FadeProcess(infoControlRubyText, 1f, 0.5f));
	}
	public void HideInfoControlBalloon()
	{
		StartCoroutine(FadeProcess(infoControlUnderlay, 0f, 0.5f));
		StartCoroutine(FadeProcess(infoControlButton, 0f, 0.5f));
		StartCoroutine(FadeProcess(infoControlText, 0f, 0.5f));
		StartCoroutine(FadeProcess(infoControlRubyText, 0f, 0.5f));
	}
	private IEnumerator FadeProcess(SpriteRenderer _sprite, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		float time = 0f;
		float startAlpha = _sprite.color.a;
		yield return new WaitForSeconds(_delayTime);
		while (time < _fadeTime)
		{
			if (_sprite != null)
			{
				_sprite.SetAlpha(Mathf.Lerp(startAlpha, _setAlpha, time / _fadeTime));
			}
			time += Time.deltaTime;
			yield return null;
		}
		_sprite.SetAlpha(_setAlpha);
		_callback?.Invoke();
	}
	private IEnumerator FadeProcess(TextMeshPro _text, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		float time = 0f;
		float startAlpha = _text.color.a;
		yield return new WaitForSeconds(_delayTime);
		while (time < _fadeTime)
		{
			if (_text != null)
			{
				_text.SetAlpha(Mathf.Lerp(startAlpha, _setAlpha, time / _fadeTime));
			}
			time += Time.deltaTime;
			yield return null;
		}
		_text.SetAlpha(_setAlpha);
		_callback?.Invoke();
	}
}
