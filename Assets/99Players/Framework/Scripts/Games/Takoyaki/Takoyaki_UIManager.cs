using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class Takoyaki_UIManager : SingletonCustom<Takoyaki_UIManager>
{
	[SerializeField]
	[Header("１人プレイ時のレイアウト")]
	private Takoyaki_UserUILayoutData singleLayout;
	[SerializeField]
	[Header("複数人プレイ時のレイアウト")]
	private Takoyaki_UserUILayoutData multiLayout;
	[SerializeField]
	[Header("画面フェ\u30fcド画像")]
	private SpriteRenderer screenFade;
	private readonly float NEXT_GROUP_2_FADE_TIME = 1f;
	public void Init()
	{
		if (Takoyaki_Define.PLAYER_NUM == 1)
		{
			singleLayout.gameObject.SetActive(value: true);
			multiLayout.gameObject.SetActive(value: false);
			singleLayout.Init();
			return;
		}
		singleLayout.gameObject.SetActive(value: false);
		multiLayout.gameObject.SetActive(value: true);
		multiLayout.Init();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP && Takoyaki_Define.PLAYER_NUM > 2)
		{
			multiLayout.SetGroupNumberIcon();
		}
	}
	public void UpdateMethod()
	{
	}
	public void SetGameTime(float _gameTime)
	{
		if (singleLayout.gameObject.activeSelf)
		{
			singleLayout.SetGameTime(_gameTime);
		}
		else if (multiLayout.gameObject.activeSelf)
		{
			multiLayout.SetGameTime(_gameTime);
		}
	}
	public void SetUserUIData(Takoyaki_PlayerManager.UserData[] _userDatas)
	{
		Takoyaki_Define.UserType[] array = new Takoyaki_Define.UserType[Takoyaki_Define.MAX_JOIN_MEMBER_NUM];
		Takoyaki_Define.TeamType[] array2 = new Takoyaki_Define.TeamType[Takoyaki_Define.MAX_JOIN_MEMBER_NUM];
		for (int i = 0; i < _userDatas.Length; i++)
		{
			array[i] = _userDatas[i].userType;
			array2[i] = _userDatas[i].teamType;
		}
		if (singleLayout.gameObject.activeSelf)
		{
			singleLayout.SetUserUIData(array, array2);
		}
		else if (multiLayout.gameObject.activeSelf)
		{
			multiLayout.SetUserUIData(array, array2);
		}
	}
	public void SetScore(Takoyaki_Define.UserType _userType, int _score)
	{
		if (singleLayout.gameObject.activeSelf)
		{
			singleLayout.SetUserScore(_userType, _score);
		}
		else if (multiLayout.gameObject.activeSelf)
		{
			multiLayout.SetUserScore(_userType, _score);
		}
	}
	public void SetGroupNumber()
	{
		if (singleLayout.gameObject.activeSelf)
		{
			singleLayout.SetGroupNumberIcon();
		}
		else if (multiLayout.gameObject.activeSelf)
		{
			multiLayout.SetGroupNumberIcon();
		}
	}
	public void ShowControlInfoBalloon()
	{
		if (singleLayout.gameObject.activeSelf)
		{
			singleLayout.ShowControlInfoBalloon();
		}
		else if (multiLayout.gameObject.activeSelf)
		{
			multiLayout.ShowControlInfoBalloon();
		}
		LeanTween.delayedCall(3f, HideControlInfoBalloon);
	}
	public void HideControlInfoBalloon()
	{
		if (singleLayout.gameObject.activeSelf)
		{
			singleLayout.HideControlInfoBalloon();
		}
		else if (multiLayout.gameObject.activeSelf)
		{
			multiLayout.HideControlInfoBalloon();
		}
	}
	public void NextGroup2Fade(Action _fadeInCallBack = null, Action _fadeOutCallBack = null)
	{
		Fade(isView: true, NEXT_GROUP_2_FADE_TIME);
		LeanTween.delayedCall(base.gameObject, NEXT_GROUP_2_FADE_TIME, (Action)delegate
		{
			if (_fadeInCallBack != null)
			{
				_fadeInCallBack();
			}
			Fade(isView: false, NEXT_GROUP_2_FADE_TIME);
			LeanTween.delayedCall(base.gameObject, NEXT_GROUP_2_FADE_TIME, (Action)delegate
			{
				if (_fadeOutCallBack != null)
				{
					_fadeOutCallBack();
				}
			});
		});
	}
	private void Fade(bool isView, float _fadeTime)
	{
		Color alpha;
		LeanTween.value(screenFade.gameObject, isView ? 0f : 1f, isView ? 1f : 0f, _fadeTime).setOnUpdate(delegate(float val)
		{
			alpha = screenFade.color;
			alpha.a = val;
			screenFade.color = alpha;
		});
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
