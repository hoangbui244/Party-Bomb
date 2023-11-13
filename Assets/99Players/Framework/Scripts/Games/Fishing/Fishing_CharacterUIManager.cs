using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
public class Fishing_CharacterUIManager : SingletonCustom<Fishing_CharacterUIManager>
{
	[SerializeField]
	[Header("3Dカメラ")]
	private Camera camera3D;
	[SerializeField]
	[Header("キャラ追従アンカ\u30fc")]
	private Transform[] characterFollowAnchor;
	[SerializeField]
	[Header("魚獲得時の吹き出し画像")]
	private SpriteRenderer[] fishGetSprite;
	private SpriteAtlas spriteAtlas;
	public void Init()
	{
		spriteAtlas = SingletonCustom<Fishing_SpriteAtlasCache>.Instance.GetSmeltFishingAtlas();
		for (int i = 0; i < fishGetSprite.Length; i++)
		{
			fishGetSprite[i].gameObject.SetActive(value: false);
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < FishingDefinition.MCM.GetUserDataLength(); i++)
		{
			if (FishingDefinition.MCM.GetUserData(i).isPlayer)
			{
				CharacterFollowUIPositionUpdate(i);
			}
		}
	}
	public void InitActiveCharacterFollowUI()
	{
		for (int i = 0; i < characterFollowAnchor.Length; i++)
		{
			characterFollowAnchor[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < FishingDefinition.MCM.GetUserDataLength(); j++)
		{
			if (FishingDefinition.MCM.GetUserData(j).isPlayer)
			{
				characterFollowAnchor[j].gameObject.SetActive(value: true);
			}
		}
	}
	private void CharacterFollowUIPositionUpdate(int _userNo)
	{
		CalcManager.mCalcVector3.x = camera3D.WorldToScreenPoint(FishingDefinition.MCM.GetUserData(_userNo).character.GetPos(_isLocal: false)).x;
		CalcManager.mCalcVector3.y = camera3D.WorldToScreenPoint(FishingDefinition.MCM.GetUserData(_userNo).character.GetPos(_isLocal: false)).y;
		CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
		characterFollowAnchor[_userNo].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, characterFollowAnchor[_userNo].transform.position.z);
	}
	public void ShowFishGetWindow(int _userDataNo, FishingDefinition.FishType _fishType)
	{
		switch (_fishType)
		{
		case FishingDefinition.FishType.Ayu:
			fishGetSprite[_userDataNo].sprite = spriteAtlas.GetSprite("_ballon_ayu");
			break;
		case FishingDefinition.FishType.Koi:
			fishGetSprite[_userDataNo].sprite = spriteAtlas.GetSprite("_ballon_coi");
			break;
		case FishingDefinition.FishType.Nizimasu:
			fishGetSprite[_userDataNo].sprite = spriteAtlas.GetSprite("_ballon_niji");
			break;
		case FishingDefinition.FishType.Yamame:
			fishGetSprite[_userDataNo].sprite = spriteAtlas.GetSprite("_ballon_yamame");
			break;
		case FishingDefinition.FishType.Iwana:
			fishGetSprite[_userDataNo].sprite = spriteAtlas.GetSprite("_ballon_iwana");
			break;
		case FishingDefinition.FishType.PetBottle1:
		case FishingDefinition.FishType.PetBottle2:
		case FishingDefinition.FishType.PetBottle3:
			fishGetSprite[_userDataNo].sprite = spriteAtlas.GetSprite("_ballon_pet");
			break;
		case FishingDefinition.FishType.Sandal:
			fishGetSprite[_userDataNo].sprite = spriteAtlas.GetSprite("_ballon_gomi");
			break;
		}
		fishGetSprite[_userDataNo].gameObject.SetActive(value: true);
	}
	public void HideFishGetWindow(int _userDataNo)
	{
		fishGetSprite[_userDataNo].gameObject.SetActive(value: false);
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
