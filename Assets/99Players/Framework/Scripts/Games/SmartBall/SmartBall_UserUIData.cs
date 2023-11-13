using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
public class SmartBall_UserUIData : MonoBehaviour
{
	[Serializable]
	public struct UserUIData
	{
		[Header("オフセット用アンカ\u30fc")]
		public Transform offsetAnchor;
		[Header("仕切りフェ\u30fcド画像")]
		public SpriteRenderer partitionFade;
		[Header("制限時間テキスト")]
		public SpriteRenderer timeLimitText;
		[Header("チ\u30fcムなし表示UIアンカ\u30fc")]
		public GameObject singleUIAnchor;
		[Header("チ\u30fcムなし用のユ\u30fcザ\u30fcアイコン")]
		public SpriteRenderer singleUserIcon;
		[Header("チ\u30fcムなし用のキャラクタ\u30fcアイコン")]
		public SpriteRenderer singleCharacterIcon;
		[Header("所属チ\u30fcム表示UIアンカ\u30fc")]
		public GameObject teamUIAnchor;
		[Header("所属チ\u30fcム表示用の下敷き")]
		public SpriteRenderer teamUnderlay;
		[Header("所属チ\u30fcム表示用のチ\u30fcム名")]
		public SpriteRenderer teamName;
		[Header("所属チ\u30fcム表示用のユ\u30fcザ\u30fcアイコン")]
		public SpriteRenderer teamUserIcon;
		[Header("所属チ\u30fcム表示用のキャラクタ\u30fcアイコン")]
		public SpriteRenderer teamCharacterIcon;
		[Header("ポイントの数値(正数値)")]
		public SpriteNumbers pointNumbers;
		[Header("ポイント記録のアンカ\u30fc")]
		public Transform pointRecordAnchor;
		[Header("ポイント記録の数値(正数値)")]
		public SpriteNumbers pointRecordNumbers;
		[Header("ゲ\u30fcムオ\u30fcバ\u30fcエフェクト")]
		public SmartBall_GameOverEffect effectGameOver;
	}
	[Serializable]
	private struct ControlInfomationUI
	{
		[Header("操作情報を示す画像デ\u30fcタ")]
		public SpriteRenderer[] infomationSpriteUI;
		[Header("操作情報を示す文字デ\u30fcタ")]
		public TextMeshPro[] infomationTextUI;
	}
	[SerializeField]
	[Header("ユ\u30fcザ\u30fcUIデ\u30fcタ")]
	private UserUIData userUIData;
	[SerializeField]
	[Header("操作情報UIデ\u30fcタ")]
	private ControlInfomationUI controlInfomationUI;
	[SerializeField]
	[Header("操作説明(１人時)")]
	private GameObject controlHelp_Single;
	[SerializeField]
	[Header("操作説明(複数時)")]
	private GameObject controlHelp_Multi;
	[SerializeField]
	[Header("「フィニッシュ！」テキストのオブジェクト")]
	private GameObject finishTextObj;
	private SpriteAtlas spriteAtlas;
	private SB.TeamType teamType;
	public void Init(SB.TeamType _teamType, SB.UserType _userType)
	{
		spriteAtlas = SingletonCustom<SAManager>.Instance.GetSA(SAType.Common);
		teamType = _teamType;
		userUIData.timeLimitText.SetAlpha(0f);
		userUIData.offsetAnchor.SetLocalPositionY(0f);
		controlHelp_Single.SetActive(value: false);
		controlHelp_Multi.SetActive(value: false);
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (_userType == SB.UserType.PLAYER_1)
			{
				controlHelp_Single.SetActive(value: true);
			}
		}
		else if ((uint)_userType <= 3u)
		{
			controlHelp_Multi.SetActive(value: true);
		}
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			userUIData.singleUIAnchor.SetActive(value: true);
			userUIData.teamUIAnchor.SetActive(value: false);
			switch (_userType)
			{
			case SB.UserType.PLAYER_1:
				if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
				{
					userUIData.singleUserIcon.sprite = spriteAtlas.GetSprite("_screen_you");
				}
				else
				{
					userUIData.singleUserIcon.sprite = spriteAtlas.GetSprite("_screen_1p");
				}
				break;
			case SB.UserType.PLAYER_2:
				userUIData.singleUserIcon.sprite = spriteAtlas.GetSprite("_screen_2p");
				break;
			case SB.UserType.PLAYER_3:
				userUIData.singleUserIcon.sprite = spriteAtlas.GetSprite("_screen_3p");
				break;
			case SB.UserType.PLAYER_4:
				userUIData.singleUserIcon.sprite = spriteAtlas.GetSprite("_screen_4p");
				break;
			case SB.UserType.CPU_1:
				userUIData.singleUserIcon.sprite = spriteAtlas.GetSprite("_screen_cp1");
				break;
			case SB.UserType.CPU_2:
				userUIData.singleUserIcon.sprite = spriteAtlas.GetSprite("_screen_cp2");
				break;
			case SB.UserType.CPU_3:
				userUIData.singleUserIcon.sprite = spriteAtlas.GetSprite("_screen_cp3");
				break;
			case SB.UserType.CPU_4:
				userUIData.singleUserIcon.sprite = spriteAtlas.GetSprite("_screen_cp4");
				break;
			case SB.UserType.CPU_5:
				userUIData.singleUserIcon.sprite = spriteAtlas.GetSprite("_screen_cp5");
				break;
			}
		}
		else
		{
			userUIData.singleUIAnchor.SetActive(value: false);
			userUIData.teamUIAnchor.SetActive(value: true);
			switch (_userType)
			{
			case SB.UserType.PLAYER_1:
				userUIData.teamUserIcon.sprite = spriteAtlas.GetSprite("_screen_1p");
				break;
			case SB.UserType.PLAYER_2:
				userUIData.teamUserIcon.sprite = spriteAtlas.GetSprite("_screen_2p");
				break;
			case SB.UserType.PLAYER_3:
				userUIData.teamUserIcon.sprite = spriteAtlas.GetSprite("_screen_3p");
				break;
			case SB.UserType.PLAYER_4:
				userUIData.teamUserIcon.sprite = spriteAtlas.GetSprite("_screen_4p");
				break;
			case SB.UserType.CPU_1:
				userUIData.teamUserIcon.sprite = spriteAtlas.GetSprite("_screen_cp1");
				break;
			case SB.UserType.CPU_2:
				userUIData.teamUserIcon.sprite = spriteAtlas.GetSprite("_screen_cp2");
				break;
			case SB.UserType.CPU_3:
				userUIData.teamUserIcon.sprite = spriteAtlas.GetSprite("_screen_cp3");
				break;
			case SB.UserType.CPU_4:
				userUIData.teamUserIcon.sprite = spriteAtlas.GetSprite("_screen_cp4");
				break;
			case SB.UserType.CPU_5:
				userUIData.teamUserIcon.sprite = spriteAtlas.GetSprite("_screen_cp5");
				break;
			}
			userUIData.teamUnderlay.sprite = spriteAtlas.GetSprite((teamType == SB.TeamType.TEAM_A) ? "team_a_underlay" : "team_b_underlay");
			userUIData.teamName.sprite = spriteAtlas.GetSprite((teamType == SB.TeamType.TEAM_A) ? "_screen_team_A" : "_screen_team_B");
		}
		int num = 2;
		switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)_userType])
		{
		case 0:
			userUIData.singleCharacterIcon.sprite = spriteAtlas.GetSprite("character_yuto_0" + num.ToString());
			userUIData.teamCharacterIcon.sprite = spriteAtlas.GetSprite("character_yuto_0" + num.ToString());
			break;
		case 1:
			userUIData.singleCharacterIcon.sprite = spriteAtlas.GetSprite("character_hina_0" + num.ToString());
			userUIData.teamCharacterIcon.sprite = spriteAtlas.GetSprite("character_hina_0" + num.ToString());
			break;
		case 2:
			userUIData.singleCharacterIcon.sprite = spriteAtlas.GetSprite("character_ituki_0" + num.ToString());
			userUIData.teamCharacterIcon.sprite = spriteAtlas.GetSprite("character_ituki_0" + num.ToString());
			break;
		case 3:
			userUIData.singleCharacterIcon.sprite = spriteAtlas.GetSprite("character_souta_0" + num.ToString());
			userUIData.teamCharacterIcon.sprite = spriteAtlas.GetSprite("character_souta_0" + num.ToString());
			break;
		case 4:
			userUIData.singleCharacterIcon.sprite = spriteAtlas.GetSprite("character_takumi_0" + num.ToString());
			userUIData.teamCharacterIcon.sprite = spriteAtlas.GetSprite("character_takumi_0" + num.ToString());
			break;
		case 5:
			userUIData.singleCharacterIcon.sprite = spriteAtlas.GetSprite("character_rin_0" + num.ToString());
			userUIData.teamCharacterIcon.sprite = spriteAtlas.GetSprite("character_rin_0" + num.ToString());
			break;
		case 6:
			userUIData.singleCharacterIcon.sprite = spriteAtlas.GetSprite("character_akira_0" + num.ToString());
			userUIData.teamCharacterIcon.sprite = spriteAtlas.GetSprite("character_akira_0" + num.ToString());
			break;
		case 7:
			userUIData.singleCharacterIcon.sprite = spriteAtlas.GetSprite("character_rui_0" + num.ToString());
			userUIData.teamCharacterIcon.sprite = spriteAtlas.GetSprite("character_rui_0" + num.ToString());
			break;
		}
		for (int i = 0; i < controlInfomationUI.infomationSpriteUI.Length; i++)
		{
			controlInfomationUI.infomationSpriteUI[i].SetAlpha(0f);
		}
		for (int j = 0; j < controlInfomationUI.infomationTextUI.Length; j++)
		{
			controlInfomationUI.infomationTextUI[j].SetAlpha(0f);
		}
		userUIData.partitionFade.SetAlpha(0f);
		userUIData.pointRecordAnchor.SetLocalPositionY(1000f);
	}
	public void SetPointData(float _height)
	{
		string[] array = new string[2]
		{
			_height.ToString(),
			"0"
		};
		userUIData.pointNumbers.SetNumbers(array[0]);
		userUIData.pointRecordNumbers.SetNumbers(array[0]);
	}
	public void SetTimeLimit(float _time, bool _isActive)
	{
		if (_isActive)
		{
			userUIData.timeLimitText.SetAlpha(1f);
			userUIData.timeLimitText.sprite = spriteAtlas.GetSprite("remaining_time_0" + Mathf.Clamp(Mathf.FloorToInt(_time + 1f), 1, 5).ToString());
		}
		else
		{
			userUIData.timeLimitText.SetAlpha(0f);
		}
	}
	public void PlayGameOverEffect(bool _isLastTeam)
	{
		userUIData.effectGameOver.PlayGameOverEffect(delegate
		{
			ShowHeightRecord();
		});
	}
	public void ShowHeightRecord()
	{
		LeanTween.moveLocalY(userUIData.pointRecordAnchor.gameObject, 0f, 0.5f).setEaseOutQuart();
	}
	public void FadeProcess_ControlInfomationUI(bool _fadeIn)
	{
		if (_fadeIn)
		{
			StartCoroutine(FadeProcess(controlInfomationUI.infomationSpriteUI, 1f, 0.5f));
			StartCoroutine(FadeProcess(controlInfomationUI.infomationTextUI, 1f, 0.5f));
		}
		else
		{
			StartCoroutine(FadeProcess(controlInfomationUI.infomationSpriteUI, 0f, 0.5f));
			StartCoroutine(FadeProcess(controlInfomationUI.infomationTextUI, 0f, 0.5f));
		}
	}
	public void SetFinishScale(Vector3 _scale)
	{
		finishTextObj.transform.localScale = _scale;
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
	public void PartitionFadeIn(Action _callback = null)
	{
		StartCoroutine(FadeProcess(userUIData.partitionFade, 0.7f, 0.5f, 0f, _callback));
	}
	public void PartitionFadeOut(Action _callback = null)
	{
		FadeProcess(userUIData.partitionFade, 0f, 0.5f, 0f, _callback);
	}
}
