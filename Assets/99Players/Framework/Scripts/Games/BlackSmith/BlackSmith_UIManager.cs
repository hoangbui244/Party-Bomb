using System;
using System.Collections.Generic;
using UnityEngine;
public class BlackSmith_UIManager : SingletonCustom<BlackSmith_UIManager>
{
	[SerializeField]
	[Header("【１人】の時のレイアウトオブジェクト")]
	private GameObject singleLayoutObjct;
	[SerializeField]
	[Header("【マルチ】の時のレイアウトオブジェクト")]
	private GameObject multiLayoutObject;
	[SerializeField]
	[Header("【１人】の時のレイアウトデ\u30fcタ")]
	private BlackSmith_LayoutData[] singleLayoutData;
	[SerializeField]
	[Header("【マルチ】の時のレイアウトデ\u30fcタ")]
	private BlackSmith_LayoutData[] multiLayoutData_Four;
	private BlackSmith_LayoutData[] ActiveLayoutData;
	[SerializeField]
	[Header("【１人】の時のタイムUI")]
	private CommonGameTimeUI_Font_Time singleTimeUI;
	[SerializeField]
	[Header("【マルチ】の時のタイムUI")]
	private CommonGameTimeUI_Font_Time multiTimeUI;
	private CommonGameTimeUI_Font_Time ActiveTimeUI;
	private int[] arraySortGaugePatternIdx = new int[12];
	private List<int>[] arrayLineList = new List<int>[4];
	private List<int> tempLineList = new List<int>();
	private int[] gaugeLineCheck = new int[12];
	[SerializeField]
	[Header("ゲ\u30fcジのバ\u30fcの移動速度")]
	private float BAR_MOVE_SPEED;
	[SerializeField]
	[Header("ゲ\u30fcジのバ\u30fcの移動速度上昇係数")]
	private float BAR_DIFF_SPEED_UP;
	[SerializeField]
	[Header("ゲ\u30fcジのフェ\u30fcドイン／フェ\u30fcドアウト時間")]
	private float GAUGE_FADE_TIME;
	private bool isShowTextUI;
	[SerializeField]
	[Header("テキストUIの表示、非表示時のアニメ\u30fcション時間")]
	private float TEXT_UI_ANIMATION_TIME;
	[SerializeField]
	[Header("テキストUIを表示する時間")]
	private float TEXT_UI_SHOW_TIME;
	[SerializeField]
	[Header("ゲ\u30fcム開始のテキストUI")]
	private SpriteRenderer announceText;
	private Vector3 originAnnounceTextScale;
	[SerializeField]
	[Header("スピ\u30fcドアップのテキストUI")]
	private SpriteRenderer speedUpText;
	private Vector3 originSpeedUpTextScale;
	public void Init()
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			singleLayoutObjct.SetActive(value: true);
			multiLayoutObject.SetActive(value: false);
			ActiveLayoutData = singleLayoutData;
			ActiveTimeUI = singleTimeUI;
		}
		else
		{
			singleLayoutObjct.SetActive(value: false);
			multiLayoutObject.SetActive(value: true);
			ActiveLayoutData = multiLayoutData_Four;
			ActiveTimeUI = multiTimeUI;
		}
		for (int i = 0; i < 4; i++)
		{
			arrayLineList[i] = new List<int>();
			for (int j = 0; j < 3; j++)
			{
				arrayLineList[i].Add(i + j * 4);
			}
		}
		int num = arraySortGaugePatternIdx.Length / 2;
		int num2 = (arraySortGaugePatternIdx.Length % 2 != 0) ? 1 : 0;
		for (int k = 0; k < num + num2; k++)
		{
			SortGaugePatternIdx(k, _isTopSearch: true);
			if (k < num)
			{
				SortGaugePatternIdx(k, _isTopSearch: false);
			}
		}
		for (int l = 0; l < ActiveLayoutData.Length; l++)
		{
			ActiveLayoutData[l].Init(l, SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[l][0]);
		}
		ActiveTimeUI.ShowRemainingTime_HideTextUI();
		isShowTextUI = false;
		announceText.gameObject.SetActive(value: true);
		originAnnounceTextScale = announceText.transform.localScale;
	}
	private void SortGaugePatternIdx(int _idx, bool _isTopSearch)
	{
		tempLineList.Clear();
		int num = (!_isTopSearch) ? ((_idx == 0) ? gaugeLineCheck[_idx] : gaugeLineCheck[arraySortGaugePatternIdx.Length - 1 - _idx + 1]) : ((_idx == 0) ? (-1) : gaugeLineCheck[_idx - 1]);
		for (int i = 0; i < arrayLineList.Length; i++)
		{
			if (i != num)
			{
				if (tempLineList.Count == 0)
				{
					tempLineList.Add(i);
				}
				else if (arrayLineList[tempLineList[0]].Count < arrayLineList[i].Count)
				{
					tempLineList.Clear();
					tempLineList.Add(i);
				}
				else if (arrayLineList[tempLineList[0]].Count == arrayLineList[i].Count)
				{
					tempLineList.Add(i);
				}
			}
		}
		num = tempLineList[UnityEngine.Random.Range(0, tempLineList.Count)];
		int index = UnityEngine.Random.Range(0, arrayLineList[num].Count);
		if (_isTopSearch)
		{
			arraySortGaugePatternIdx[_idx] = arrayLineList[num][index];
			gaugeLineCheck[_idx] = num;
		}
		else
		{
			arraySortGaugePatternIdx[arraySortGaugePatternIdx.Length - 1 - _idx] = arrayLineList[num][index];
			gaugeLineCheck[arraySortGaugePatternIdx.Length - 1 - _idx] = num;
		}
		arrayLineList[num].RemoveAt(index);
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < ActiveLayoutData.Length; i++)
		{
			if (SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(i).GetState() == BlackSmith_PlayerManager.State.HammerStrike)
			{
				ActiveLayoutData[i].UpdateMethod();
			}
		}
	}
	public void SetTime(float _time)
	{
		ActiveTimeUI.SetTime(_time);
	}
	public void SetCreateWeaponCnt(int _playerNo, int _cnt)
	{
		ActiveLayoutData[_playerNo].SetCreateWeaponCnt(_cnt);
	}
	public void HidePointUI(int _playerNo)
	{
		Vector3 hidePos = SingletonCustom<GameSettingManager>.Instance.IsSinglePlay ? ((_playerNo == 0) ? new Vector3(0f, 500f, 0f) : new Vector3(500f, 0f, 0f)) : new Vector3(0f, 500f, 0f);
		ActiveLayoutData[_playerNo].HidePointUI((float)_playerNo * 0.15f, hidePos);
	}
	public int GetArraySortGaugePatternIdx(int _idx)
	{
		return arraySortGaugePatternIdx[_idx];
	}
	public void SetGaugeFadeActive(int _playerNo, bool _isActive)
	{
		ActiveLayoutData[_playerNo].SetGaugeFadeActive(_isActive);
	}
	public void SetGaugeFade(int _playerNo, bool _fadeIn)
	{
		ActiveLayoutData[_playerNo].SetGaugeFade(_fadeIn);
	}
	public int GetBarMoveDir(int _playerNo)
	{
		return ActiveLayoutData[_playerNo].GetBarMoveDir();
	}
	public int GetBarIdx(int _playerNo)
	{
		return ActiveLayoutData[_playerNo].GetBarIdx();
	}
	public bool IsCanPerfectInput(int _playerNo)
	{
		return ActiveLayoutData[_playerNo].IsCanPerfectInput();
	}
	public bool IsPerfectBetweenMinMax(int _playerNo)
	{
		return ActiveLayoutData[_playerNo].IsPerfectBetweenMinMax();
	}
	public bool IsNiceBetweenMinMax(int _playerNo)
	{
		return ActiveLayoutData[_playerNo].IsNiceBetweenMinMax();
	}
	public bool IsCanGoodInput(int _playerNo)
	{
		return ActiveLayoutData[_playerNo].IsCanGoodInput();
	}
	public bool IsGoodBetweenMinMax(int _playerNo)
	{
		return ActiveLayoutData[_playerNo].IsGoodBetweenMinMax();
	}
	public bool IsInputTiming(int _playerNo, int _idx, BlackSmith_PlayerManager.EvaluationType _evaluationType, float _inputTiming)
	{
		return ActiveLayoutData[_playerNo].IsInputTiming(_idx, _evaluationType, _inputTiming);
	}
	public float GetInputTiming(int _playerNo, int _idx, BlackSmith_PlayerManager.EvaluationType _evaluationType)
	{
		return ActiveLayoutData[_playerNo].GetInputTiming(_idx, _evaluationType);
	}
	public float GetBarMoveSpeed()
	{
		return BAR_MOVE_SPEED;
	}
	public float GetBarDiffSpeedUp()
	{
		return BAR_DIFF_SPEED_UP;
	}
	public float GetGaugeFadeTime()
	{
		return GAUGE_FADE_TIME;
	}
	public void PlayEvaluationEffect(int _playerNo, BlackSmith_PlayerManager.EvaluationType _evaluationType)
	{
		ActiveLayoutData[_playerNo].PlayEvaluationEffect(_playerNo, _evaluationType);
	}
	public void SetCreatePercent(int _playerNo, int _percent)
	{
		ActiveLayoutData[_playerNo].SetCreatePercent(_percent);
	}
	public void HideAnnounceText(int _playerNo)
	{
		if (!SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(_playerNo).GetIsCpu())
		{
			ActiveLayoutData[_playerNo].HideAnnounceText();
		}
	}
	public void HideAnnounceText()
	{
		LeanTween.scale(announceText.gameObject, Vector3.zero, SingletonCustom<BlackSmith_UIManager>.Instance.GetTextUIAnimationTime()).setEaseInBack();
		Color color = announceText.color;
		color.a = 0f;
		LeanTween.color(announceText.gameObject, color, SingletonCustom<BlackSmith_UIManager>.Instance.GetTextUIAnimationTime());
		LeanTween.delayedCall(announceText.gameObject, SingletonCustom<BlackSmith_UIManager>.Instance.GetTextUIAnimationTime(), (Action)delegate
		{
			announceText.gameObject.SetActive(value: false);
		});
	}
	public void ShowSpeedUpText(int _playerNo)
	{
		if (!SingletonCustom<BlackSmith_PlayerManager>.Instance.GetPlayer(_playerNo).GetIsCpu())
		{
			ActiveLayoutData[_playerNo].ShowSpeedUpText();
		}
	}
	public void ShowSpeedUpText()
	{
		speedUpText.transform.localScale = Vector3.zero;
		speedUpText.gameObject.SetActive(value: true);
		LeanTween.scale(speedUpText.gameObject, originSpeedUpTextScale, SingletonCustom<BlackSmith_UIManager>.Instance.GetTextUIAnimationTime()).setEaseOutBack().setOnComplete((Action)delegate
		{
			LeanTween.delayedCall(speedUpText.gameObject, SingletonCustom<BlackSmith_UIManager>.Instance.GetTextUIShowTime(), (Action)delegate
			{
				HideSpeedUpText();
			});
		});
	}
	public void HideSpeedUpText()
	{
		LeanTween.scale(speedUpText.gameObject, Vector3.zero, SingletonCustom<BlackSmith_UIManager>.Instance.GetTextUIAnimationTime()).setEaseInBack();
		Color color = speedUpText.color;
		color.a = 0f;
		LeanTween.color(speedUpText.gameObject, color, SingletonCustom<BlackSmith_UIManager>.Instance.GetTextUIAnimationTime());
		LeanTween.delayedCall(speedUpText.gameObject, SingletonCustom<BlackSmith_UIManager>.Instance.GetTextUIAnimationTime(), (Action)delegate
		{
			speedUpText.gameObject.SetActive(value: false);
		});
	}
	public void SetIsShowSpeedUpText(bool _isShowSpeedUpText)
	{
		isShowTextUI = _isShowSpeedUpText;
	}
	public bool GetIsShowTextUI()
	{
		return isShowTextUI;
	}
	public float GetTextUIAnimationTime()
	{
		return TEXT_UI_ANIMATION_TIME;
	}
	public float GetTextUIShowTime()
	{
		return TEXT_UI_SHOW_TIME;
	}
	public void SetJoyConButtonPlayerType(int _playerNo, int _userType)
	{
		ActiveLayoutData[_playerNo].SetJoyConButtonPlayerType(_userType);
	}
	public void HideUI(int _playerNo)
	{
		ActiveLayoutData[_playerNo].HideUI();
	}
}
