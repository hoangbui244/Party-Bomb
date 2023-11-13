using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class Surfing_UIManager : SingletonCustom<Surfing_UIManager>
{
	[SerializeField]
	[Header("1人プレイ時のレイアウト")]
	private Surfing_UserUILayoutData singleLayout;
	[SerializeField]
	[Header("複数人プレイ時のレイアウト")]
	private Surfing_UserUILayoutData multiLayout;
	[SerializeField]
	[Header("画面フェ\u30fcド画像")]
	private SpriteRenderer screenFade;
	[SerializeField]
	[Header("画面分割用の枠線(4人用)")]
	private GameObject fourPartition;
	[SerializeField]
	[Header("操作説明UI(ロ\u30fcカライズ位置変更用)")]
	private GameObject[] ControlOperationAnchor;
	[SerializeField]
	[Header("移動量(ロ\u30fcカライズ位置変更用)")]
	private Vector3 movePos;
	private bool isRankUpdateFlg;
	private readonly float NEXT_GROUP_2_FADE_TIME = 1f;
	public void Init()
	{
		singleLayout.gameObject.SetActive(value: false);
		multiLayout.gameObject.SetActive(value: false);
		fourPartition.SetActive(value: false);
		switch (Surfing_Define.PLAYER_NUM)
		{
		case 1:
			if (!(singleLayout == null))
			{
				singleLayout.gameObject.SetActive(value: true);
				singleLayout.Init();
			}
			break;
		case 2:
		case 3:
		case 4:
			if (!(multiLayout == null))
			{
				multiLayout.gameObject.SetActive(value: true);
				multiLayout.Init();
				fourPartition.SetActive(value: true);
			}
			break;
		}
		if (Localize_Define.Language == Localize_Define.LanguageType.English)
		{
			for (int i = 0; i < ControlOperationAnchor.Length; i++)
			{
				ControlOperationAnchor[i].transform.position += movePos;
			}
		}
	}
	public void UpdateMethod()
	{
	}
	public void PearlingWarning(int _player)
	{
		switch (Surfing_Define.PLAYER_NUM)
		{
		case 1:
			singleLayout.CourseOutWarning(_player);
			break;
		case 2:
		case 3:
		case 4:
			multiLayout.CourseOutWarning(_player);
			break;
		}
	}
	public void GameEnd()
	{
		switch (Surfing_Define.PLAYER_NUM)
		{
		case 1:
			singleLayout.SetUserRanking();
			break;
		case 2:
		case 3:
		case 4:
			multiLayout.SetUserRanking();
			break;
		}
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
	public void SetUserUIData(Surfing_PlayerManager.UserData[] _userDatas)
	{
		Surfing_Define.UserType[] array = new Surfing_Define.UserType[Surfing_Define.MEMBER_NUM];
		for (int i = 0; i < _userDatas.Length; i++)
		{
			array[i] = _userDatas[i].userType;
		}
		if (singleLayout.gameObject.activeSelf)
		{
			singleLayout.SetUserUIData(array);
		}
		else if (multiLayout.gameObject.activeSelf)
		{
			multiLayout.SetUserUIData(array);
		}
	}
	public void SetScore(Surfing_Define.UserType _userType, int _score, int _point)
	{
		if (singleLayout.gameObject.activeSelf)
		{
			singleLayout.SetUserScore(_userType, _score, _point);
		}
		else if (multiLayout.gameObject.activeSelf)
		{
			multiLayout.SetUserScore(_userType, _score, _point);
		}
	}
	public void SetGroupNumber()
	{
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
	public void SetFade(Surfing_Define.UserType _userType, bool _set = true)
	{
		if (singleLayout.gameObject.activeSelf)
		{
			singleLayout.SetUserFade(_userType, _set);
		}
		else if (multiLayout.gameObject.activeSelf)
		{
			multiLayout.SetUserFade(_userType, _set);
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
	private IEnumerator isRankUpdateFlgWait()
	{
		yield return new WaitForSeconds(0.5f);
		isRankUpdateFlg = false;
	}
}
