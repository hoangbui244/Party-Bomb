using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class Takoyaki_UserUIData : MonoBehaviour
{
	[Serializable]
	public struct BalloonTextData
	{
		[Header("吹き出しの親アンカ\u30fc")]
		public GameObject balloonRoot;
		[Header("吹き出しの下敷き画像")]
		public SpriteRenderer balloonUnderlay;
		[Header("吹き出しのボタン画像")]
		public SpriteRenderer balloonButton;
		[Header("吹き出しの文字")]
		public TextMeshPro balloonText;
		public void Init()
		{
			if (balloonRoot != null)
			{
				balloonRoot.SetActive(value: false);
				balloonUnderlay.SetAlpha(0f);
				balloonButton.SetAlpha(0f);
				balloonText.SetAlpha(0f);
			}
		}
	}
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン画像")]
	private SpriteRenderer playerIconSp;
	[SerializeField]
	[Header("チ\u30fcムアイコン画像")]
	private SpriteRenderer teamIconSp;
	[SerializeField]
	[Header("スコアフレ\u30fcムアンカ\u30fc")]
	private Transform scoreFrameAnchor;
	[SerializeField]
	[Header("キャラクタ\u30fcアイコン画像")]
	private SpriteRenderer characterIconSp;
	[SerializeField]
	[Header("スコア処理")]
	private SpriteNumbers scoreNumbers;
	[SerializeField]
	[Header("操作吹き出しのデ\u30fcタ")]
	private BalloonTextData balloonTextData;
	private Takoyaki_Define.UserType userType;
	private Takoyaki_Define.TeamType teamType;
	public Takoyaki_Define.UserType UserType => userType;
	public Takoyaki_Define.TeamType TeamType => teamType;
	public void Init(Takoyaki_Define.UserType _userType, Takoyaki_Define.TeamType _teamType)
	{
		userType = _userType;
		teamType = _teamType;
		SetPlayerIcon();
		SetCharacterIcon();
		SetScore(0);
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			if (teamIconSp != null)
			{
				teamIconSp.gameObject.SetActive(value: false);
			}
		}
		else
		{
			teamIconSp.gameObject.SetActive(value: true);
			if (teamType == Takoyaki_Define.TeamType.TEAM_A)
			{
				teamIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_team_A");
			}
			else if (teamType == Takoyaki_Define.TeamType.TEAM_B)
			{
				teamIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_team_B");
			}
		}
		balloonTextData.Init();
		if (userType <= Takoyaki_Define.UserType.PLAYER_4)
		{
			balloonTextData.balloonRoot.SetActive(value: true);
		}
	}
	public void SetScore(int _score)
	{
		scoreNumbers.Set(_score);
	}
	private void SetPlayerIcon()
	{
		if (!(playerIconSp == null))
		{
			switch (userType)
			{
			case Takoyaki_Define.UserType.PLAYER_1:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, SingletonCustom<GameSettingManager>.Instance.IsSinglePlay ? "_screen_you" : "_screen_1p");
				break;
			case Takoyaki_Define.UserType.PLAYER_2:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_2p");
				break;
			case Takoyaki_Define.UserType.PLAYER_3:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_3p");
				break;
			case Takoyaki_Define.UserType.PLAYER_4:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_4p");
				break;
			case Takoyaki_Define.UserType.CPU_1:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp1");
				break;
			case Takoyaki_Define.UserType.CPU_2:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp2");
				break;
			case Takoyaki_Define.UserType.CPU_3:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp3");
				break;
			case Takoyaki_Define.UserType.CPU_4:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp4");
				break;
			case Takoyaki_Define.UserType.CPU_5:
				playerIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp5");
				break;
			}
		}
	}
	private void SetCharacterIcon()
	{
		switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType])
		{
		case 0:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_yuto_0" + 2.ToString());
			break;
		case 1:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_hina_0" + 2.ToString());
			break;
		case 2:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_ituki_0" + 2.ToString());
			break;
		case 3:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_souta_0" + 2.ToString());
			break;
		case 4:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_takumi_0" + 2.ToString());
			break;
		case 5:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rin_0" + 2.ToString());
			break;
		case 6:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_akira_0" + 2.ToString());
			break;
		case 7:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rui_0" + 2.ToString());
			break;
		}
	}
	public void FadeProcess_ControlInfomationUI(bool _fadeIn)
	{
		if (!(balloonTextData.balloonRoot == null) && balloonTextData.balloonRoot.activeSelf)
		{
			if (_fadeIn)
			{
				StartCoroutine(FadeProcess(balloonTextData.balloonUnderlay, 1f, 0.5f));
				StartCoroutine(FadeProcess(balloonTextData.balloonButton, 1f, 0.5f));
				StartCoroutine(FadeProcess(balloonTextData.balloonText, 1f, 0.5f));
			}
			else
			{
				StartCoroutine(FadeProcess(balloonTextData.balloonUnderlay, 0f, 0.5f));
				StartCoroutine(FadeProcess(balloonTextData.balloonButton, 0f, 0.5f));
				StartCoroutine(FadeProcess(balloonTextData.balloonText, 0f, 0.5f));
			}
		}
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
