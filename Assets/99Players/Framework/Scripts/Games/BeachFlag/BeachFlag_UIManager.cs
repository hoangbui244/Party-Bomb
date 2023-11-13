using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class BeachFlag_UIManager : SingletonCustom<BeachFlag_UIManager>
{
	[SerializeField]
	[Header("1~4人プレイ時のレイアウト")]
	private BeachFlag_UserUILayoutData singleLayout;
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
	private BeachFlag_TournamentUIManager tournamentUIManager;
	[SerializeField]
	[Header("ト\u30fcナメント戦UIオブジェクト")]
	private GameObject object_Battle_Tournament_UI;
	[SerializeField]
	[Header("スキップ")]
	private ControllerBalloonUI skipButton;
	private readonly float NEXT_GROUP_2_FADE_TIME = 1f;
	public BeachFlag_TournamentUIManager TournamentUI => tournamentUIManager;
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
			objOperationSingle.transform.position += movePos;
			objOperationMulti.transform.position += movePos;
		}
		if (tournamentUIManager != null)
		{
			tournamentUIManager.Init();
		}
		if (skipButton != null)
		{
			skipButton.Init();
		}
	}
	public void UpdateMethod()
	{
	}
	public void CourseOutWarning(int _player)
	{
	}
	public void GameEnd()
	{
	}
	public void SetGameTime(float _gameTime)
	{
	}
	public void SetUserUIData(BeachFlag_PlayerManager.UserData[] _userDatas)
	{
		BeachFlag_Define.UserType[] array = new BeachFlag_Define.UserType[BeachFlag_Define.MEMBER_NUM];
		for (int i = 0; i < _userDatas.Length; i++)
		{
			array[i] = _userDatas[i].userType;
		}
		if (singleLayout != null)
		{
			singleLayout.SetUserUIData(array);
		}
	}
	public void SetScore(BeachFlag_Define.UserType _userType, int _score)
	{
	}
	public void SetGroupNumber()
	{
	}
	public void ShowControlInfoBalloon()
	{
		singleLayout.ShowControlInfoBalloon();
		LeanTween.delayedCall(3f, HideControlInfoBalloon);
	}
	public void HideControlInfoBalloon()
	{
		singleLayout.HideControlInfoBalloon();
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
	public void SetTournamentTeamData()
	{
		tournamentUIManager.SetTeamData(SingletonCustom<BeachFlag_GameManager>.Instance.GetTournamentVSTeamData()[0], BeachFlag_TournamentUIManager.RoundType.Round_1st);
		tournamentUIManager.SetTeamData(SingletonCustom<BeachFlag_GameManager>.Instance.GetTournamentVSTeamData()[1], BeachFlag_TournamentUIManager.RoundType.Round_2nd);
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
	public BeachFlag_TournamentUIManager.RoundType GetNowTournamentType()
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
}
