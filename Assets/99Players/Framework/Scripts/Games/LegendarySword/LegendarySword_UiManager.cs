using System;
using System.Collections;
using UnityEngine;
public class LegendarySword_UiManager : SingletonCustom<LegendarySword_UiManager>
{
	[SerializeField]
	[Header("1~4人プレイ時のレイアウト")]
	private LegendarySword_UserUILayoutData singleLayout;
	[SerializeField]
	[Header("画面フェ\u30fcド画像")]
	private SpriteRenderer screenFade;
	[SerializeField]
	[Header("操作説明：1人")]
	private GameObject objOperationSingle;
	[SerializeField]
	[Header("操作説明：複数人")]
	private GameObject objOperationMulti;
	[SerializeField]
	[Header("移動量(ロ\u30fcカライズ位置変更用")]
	private Vector3 movePos;
	[SerializeField]
	[Header("ト\u30fcナメント戦UI管理処理")]
	private LegendarySword_TournamentUIManager tournamentUIManager;
	[SerializeField]
	[Header("ト\u30fcナメント戦UIオブジェクト")]
	private GameObject object_Battle_Tournament_UI;
	[SerializeField]
	[Header("スキップ")]
	private ControllerBalloonUI skipButton;
	private readonly float NEXT_GROUP_2_FADE_TIME = 1f;
	[SerializeField]
	[Header("ゲ\u30fcム開始のテキストUI")]
	private SpriteRenderer announceText;
	private Vector3 originAnnounceTextScale;
	[SerializeField]
	[Header("テキストUIの表示、非表示時のアニメ\u30fcション時間")]
	private float TEXT_UI_ANIMATION_TIME;
	public LegendarySword_TournamentUIManager TournamentUI => tournamentUIManager;
	public bool IsSkipShow => skipButton.gameObject.activeSelf;
	public void Init()
	{
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			if (objOperationSingle != null)
			{
				objOperationSingle.SetActive(value: true);
			}
			if (objOperationMulti != null)
			{
				objOperationMulti.SetActive(value: false);
			}
		}
		else
		{
			if (objOperationSingle != null)
			{
				objOperationSingle.SetActive(value: false);
			}
			if (objOperationMulti != null)
			{
				objOperationMulti.SetActive(value: true);
			}
		}
		if (Localize_Define.Language == Localize_Define.LanguageType.English)
		{
			if (objOperationSingle != null)
			{
				objOperationSingle.transform.position += movePos;
			}
			if (objOperationMulti != null)
			{
				objOperationMulti.transform.position += movePos;
			}
		}
		if (tournamentUIManager != null)
		{
			tournamentUIManager.Init();
		}
		if (skipButton != null)
		{
			skipButton.Init();
		}
		announceText.gameObject.SetActive(value: true);
		originAnnounceTextScale = announceText.transform.localScale;
	}
	public void SetUserUIData(LegendarySword_PlayerManager.UserData[] _userDatas)
	{
		LegendarySword_Define.UserType[] array = new LegendarySword_Define.UserType[LegendarySword_Define.MEMBER_NUM];
		for (int i = 0; i < _userDatas.Length; i++)
		{
			array[i] = _userDatas[i].userType;
		}
		if (singleLayout != null)
		{
			singleLayout.SetUserUIData(array);
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
	public void SetTournamentTeamData()
	{
		tournamentUIManager.SetTeamData(SingletonCustom<LegendarySword_GameManager>.Instance.GetTournamentVSTeamData()[0], LegendarySword_TournamentUIManager.RoundType.Round_1st);
		tournamentUIManager.SetTeamData(SingletonCustom<LegendarySword_GameManager>.Instance.GetTournamentVSTeamData()[1], LegendarySword_TournamentUIManager.RoundType.Round_2nd);
	}
	public void StartTournamentUIAnimation(bool _winnerTeamLeft, Action _endCallBack)
	{
		tournamentUIManager.StartLineAnimation(_winnerTeamLeft, _endCallBack);
	}
	public void NextSettingTornament()
	{
		tournamentUIManager.NextSetting();
	}
	public int[] GetTournamentFinalRoundTeamNoArray()
	{
		return tournamentUIManager.GetFinalRoundTeamNo();
	}
	public int[] GetTournamentLoserBattleTeamNoArray()
	{
		return tournamentUIManager.GetLoserBattleTeamNo();
	}
	public LegendarySword_TournamentUIManager.RoundType GetNowTournamentType()
	{
		return tournamentUIManager.CurrentRoundType;
	}
	public int[] GetTournamentWinnerTeamNoList()
	{
		return tournamentUIManager.GetWinnerTeamNo();
	}
	public void ShowSkip()
	{
		if (skipButton != null)
		{
			skipButton.FadeProcess_ControlInfomationUI(_fadeIn: true);
		}
	}
	public void HideSkip()
	{
		if (skipButton != null)
		{
			skipButton.SetControlInfomationUIActive(_isActive: false);
		}
	}
	public void SkipInit()
	{
		if (skipButton != null)
		{
			skipButton.Init();
		}
	}
	public void ShowAnnounceText()
	{
		announceText.transform.localScale = Vector3.zero;
		announceText.gameObject.SetActive(value: true);
		LeanTween.scale(announceText.gameObject, originAnnounceTextScale, TEXT_UI_ANIMATION_TIME).setEaseOutBack().setOnComplete((Action)delegate
		{
			LeanTween.delayedCall(announceText.gameObject, 1f, (Action)delegate
			{
				HideAnnounceText();
			});
		});
	}
	public void HideAnnounceText()
	{
		LeanTween.scale(announceText.gameObject, Vector3.zero, TEXT_UI_ANIMATION_TIME).setEaseInBack();
		Color color = announceText.color;
		color.a = 0f;
		LeanTween.color(announceText.gameObject, color, TEXT_UI_ANIMATION_TIME);
		LeanTween.delayedCall(announceText.gameObject, TEXT_UI_ANIMATION_TIME, (Action)delegate
		{
			announceText.gameObject.SetActive(value: false);
		});
	}
}
