using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_PlayerIcon : DecoratedMonoBehaviour
{
	private SpriteRenderer playerIcon;
	public void Initialize(int playerNo)
	{
		playerIcon = GetComponent<SpriteRenderer>();
		FlyingSquirrelRace_Definition.Controller controller = SingletonMonoBehaviour<FlyingSquirrelRace_Players>.Instance.GetPlayer(playerNo).Controller;
		UnityEngine.Debug.Log("playerNo : " + playerNo.ToString());
		UnityEngine.Debug.Log("controller : " + controller.ToString());
		if (controller < FlyingSquirrelRace_Definition.Controller.Cpu1)
		{
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_you");
			}
			else
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + ((int)(controller + 1)).ToString() + "p");
			}
		}
		else
		{
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + ((int)(controller - 4 + 1)).ToString());
		}
	}
	public void Hide()
	{
		StartCoroutine(FadeProcess(playerIcon, 0f, 0.5f, delegate
		{
			base.gameObject.SetActive(value: false);
		}));
	}
	private IEnumerator FadeProcess(SpriteRenderer _sprite, float _setAlpha, float _fadeTime, Action _callback = null)
	{
		float time = 0f;
		float startAlpha = _sprite.color.a;
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
}
