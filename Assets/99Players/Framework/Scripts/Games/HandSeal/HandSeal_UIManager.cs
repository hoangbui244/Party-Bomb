﻿using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class HandSeal_UIManager : SingletonCustom<HandSeal_UIManager>
{
	[SerializeField]
	[Header("1人プレイ時のレイアウト")]
	private HandSeal_UserUILayoutData singleLayout;
	[SerializeField]
	[Header("2人プレイ時のレイアウト")]
	private HandSeal_UserUILayoutData twoPlayerLayout;
	[SerializeField]
	[Header("3人プレイ時のレイアウト")]
	private HandSeal_UserUILayoutData threePlayerLayout;
	[SerializeField]
	[Header("4人プレイ時のレイアウト")]
	private HandSeal_UserUILayoutData fourPlayerLayout;
	[SerializeField]
	[Header("画面フェ\u30fcド画像")]
	private SpriteRenderer screenFade;
	[SerializeField]
	[Header("画面分割用の枠線(2人用)")]
	private GameObject twoPartition;
	[SerializeField]
	[Header("画面分割用の枠線(4人用)")]
	private GameObject fourPartition;
	[SerializeField]
	[Header("操作説明UI(ロ\u30fcカライズ位置変更用)")]
	private GameObject[] ControlOperationAnchor;
	[SerializeField]
	[Header("移動量(ロ\u30fcカライズ位置変更用)")]
	private Vector3 movePos;
	[SerializeField]
	[Header("各プレイヤ\u30fcのボタン入力UI親アンカ\u30fc(1P～4P)")]
	private GameObject[] inputButtonUIAnchor;
	[SerializeField]
	[Header("2人プレイ時の1PのUI位置")]
	private Transform twoPlayer_1P_UIPos;
	[SerializeField]
	[Header("3,4人プレイ時の1PのUI位置")]
	private Transform fourPlayer_1P_UIPos;
	[SerializeField]
	[Header("3,4人プレイ時の2PのUI位置")]
	private Transform fourPlayer_2P_UIPos;
	private bool isRankUpdateFlg;
	private readonly float NEXT_GROUP_2_FADE_TIME = 1f;
	public void Init()
	{
		singleLayout.gameObject.SetActive(value: false);
		twoPlayerLayout.gameObject.SetActive(value: false);
		threePlayerLayout.gameObject.SetActive(value: false);
		fourPlayerLayout.gameObject.SetActive(value: false);
		twoPartition.SetActive(value: false);
		fourPartition.SetActive(value: false);
		switch (HandSeal_Define.PLAYER_NUM)
		{
		case 1:
			if (!(singleLayout == null))
			{
				singleLayout.gameObject.SetActive(value: true);
				singleLayout.Init();
				inputButtonUIAnchor[0].SetActive(value: true);
				inputButtonUIAnchor[1].SetActive(value: false);
				inputButtonUIAnchor[2].SetActive(value: false);
				inputButtonUIAnchor[3].SetActive(value: false);
			}
			break;
		case 2:
			if (!(twoPlayerLayout == null))
			{
				twoPlayerLayout.gameObject.SetActive(value: true);
				twoPlayerLayout.Init();
				twoPartition.SetActive(value: true);
				inputButtonUIAnchor[0].SetActive(value: true);
				inputButtonUIAnchor[1].SetActive(value: true);
				inputButtonUIAnchor[2].SetActive(value: false);
				inputButtonUIAnchor[3].SetActive(value: false);
				inputButtonUIAnchor[0].transform.position = twoPlayer_1P_UIPos.position;
				inputButtonUIAnchor[0].transform.localScale = twoPlayer_1P_UIPos.localScale;
			}
			break;
		case 3:
			if (!(threePlayerLayout == null))
			{
				threePlayerLayout.gameObject.SetActive(value: true);
				threePlayerLayout.Init();
				fourPartition.SetActive(value: true);
				inputButtonUIAnchor[0].SetActive(value: true);
				inputButtonUIAnchor[1].SetActive(value: true);
				inputButtonUIAnchor[2].SetActive(value: true);
				inputButtonUIAnchor[3].SetActive(value: true);
				inputButtonUIAnchor[0].transform.position = fourPlayer_1P_UIPos.position;
				inputButtonUIAnchor[0].transform.localScale = fourPlayer_1P_UIPos.localScale;
				inputButtonUIAnchor[1].transform.position = fourPlayer_2P_UIPos.position;
				inputButtonUIAnchor[1].transform.localScale = fourPlayer_2P_UIPos.localScale;
			}
			break;
		case 4:
			if (!(fourPlayerLayout == null))
			{
				fourPlayerLayout.gameObject.SetActive(value: true);
				fourPlayerLayout.Init();
				fourPartition.SetActive(value: true);
				inputButtonUIAnchor[0].SetActive(value: true);
				inputButtonUIAnchor[1].SetActive(value: true);
				inputButtonUIAnchor[2].SetActive(value: true);
				inputButtonUIAnchor[3].SetActive(value: true);
				inputButtonUIAnchor[0].transform.position = fourPlayer_1P_UIPos.position;
				inputButtonUIAnchor[0].transform.localScale = fourPlayer_1P_UIPos.localScale;
				inputButtonUIAnchor[1].transform.position = fourPlayer_2P_UIPos.position;
				inputButtonUIAnchor[1].transform.localScale = fourPlayer_2P_UIPos.localScale;
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
	public void GameEnd()
	{
	}
	public void SetGameTime(float _gameTime)
	{
		switch (HandSeal_Define.PLAYER_NUM)
		{
		case 1:
			if (!(singleLayout == null))
			{
				singleLayout.SetGameTime(_gameTime);
			}
			break;
		case 2:
			if (!(twoPlayerLayout == null))
			{
				twoPlayerLayout.SetGameTime(_gameTime);
			}
			break;
		case 3:
			if (!(threePlayerLayout == null))
			{
				threePlayerLayout.SetGameTime(_gameTime);
			}
			break;
		case 4:
			if (!(fourPlayerLayout == null))
			{
				fourPlayerLayout.SetGameTime(_gameTime);
			}
			break;
		}
	}
	public void SetUserUIData(HandSeal_PlayerManager.UserData[] _userDatas)
	{
		HandSeal_Define.UserType[] array = new HandSeal_Define.UserType[HandSeal_Define.MEMBER_NUM];
		for (int i = 0; i < _userDatas.Length; i++)
		{
			array[i] = _userDatas[i].userType;
		}
		switch (HandSeal_Define.PLAYER_NUM)
		{
		case 1:
			if (!(singleLayout == null))
			{
				singleLayout.SetUserUIData(array);
			}
			break;
		case 2:
			if (!(twoPlayerLayout == null))
			{
				twoPlayerLayout.SetUserUIData(array);
			}
			break;
		case 3:
			if (!(threePlayerLayout == null))
			{
				threePlayerLayout.SetUserUIData(array);
			}
			break;
		case 4:
			if (!(fourPlayerLayout == null))
			{
				fourPlayerLayout.SetUserUIData(array);
			}
			break;
		}
	}
	public void SetScore(HandSeal_Define.UserType _userType, int _score)
	{
		switch (HandSeal_Define.PLAYER_NUM)
		{
		case 1:
			if (!(singleLayout == null))
			{
				singleLayout.SetUserScore(_userType, _score);
			}
			break;
		case 2:
			if (!(twoPlayerLayout == null))
			{
				twoPlayerLayout.SetUserScore(_userType, _score);
			}
			break;
		case 3:
			if (!(threePlayerLayout == null))
			{
				threePlayerLayout.SetUserScore(_userType, _score);
			}
			break;
		case 4:
			if (!(fourPlayerLayout == null))
			{
				fourPlayerLayout.SetUserScore(_userType, _score);
			}
			break;
		}
	}
	public void SetGroupNumber()
	{
	}
	public void ShowControlInfoBalloon()
	{
		switch (HandSeal_Define.PLAYER_NUM)
		{
		case 1:
			singleLayout.ShowControlInfoBalloon();
			break;
		case 2:
			twoPlayerLayout.ShowControlInfoBalloon();
			break;
		case 3:
			threePlayerLayout.ShowControlInfoBalloon();
			break;
		case 4:
			fourPlayerLayout.ShowControlInfoBalloon();
			break;
		}
		LeanTween.delayedCall(3f, HideControlInfoBalloon);
	}
	public void HideControlInfoBalloon()
	{
		switch (HandSeal_Define.PLAYER_NUM)
		{
		case 1:
			singleLayout.HideControlInfoBalloon();
			break;
		case 2:
			twoPlayerLayout.HideControlInfoBalloon();
			break;
		case 3:
			threePlayerLayout.HideControlInfoBalloon();
			break;
		case 4:
			fourPlayerLayout.HideControlInfoBalloon();
			break;
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
