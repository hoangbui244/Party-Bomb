using System;
using UnityEngine;
public class MonsterKill_UIManager : SingletonCustom<MonsterKill_UIManager>
{
	[Serializable]
	private struct PointSprite
	{
		public Sprite[] pointSprte;
	}
	private Camera globalCamera;
	[SerializeField]
	[Header("【１人】の時のレイアウトオブジェクト")]
	private GameObject singleLayoutObjct;
	[SerializeField]
	[Header("【マルチ】の時のレイアウトオブジェクト")]
	private GameObject multiLayoutObject_Four;
	[SerializeField]
	[Header("【１人】の時のレイアウトデ\u30fcタ")]
	private MonsterKill_LayoutData[] singleLayoutData;
	[SerializeField]
	[Header("【マルチ】の時のレイアウトデ\u30fcタ")]
	private MonsterKill_LayoutData[] multiLayoutData_Four;
	private MonsterKill_LayoutData[] ActiveLayoutData;
	[SerializeField]
	[Header("【１人】の時のタイムUI")]
	private CommonGameTimeUI_Font_Time singleTimeUI;
	[SerializeField]
	[Header("【マルチ】の時のタイムUI")]
	private CommonGameTimeUI_Font_Time multiTimeUI_Four;
	private CommonGameTimeUI_Font_Time ActiveTimeUI;
	[SerializeField]
	[Header("ミニマップUI")]
	private MonsterKill_MiniMapUI miniMapUI;
	[SerializeField]
	[Header("ポイントアップUIのプレハブ")]
	private MonsterKill_PointUpUI pointUIPref;
	[SerializeField]
	[Header("各キャラクタ\u30fc用のポイント画像")]
	private PointSprite[] arrayPointSprte;
	[SerializeField]
	[Header("テキストUIの表示、非表示時のアニメ\u30fcション時間")]
	private float TEXT_UI_ANIMATION_TIME;
	public void Init()
	{
		globalCamera = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>();
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			singleLayoutObjct.SetActive(value: true);
			multiLayoutObject_Four.SetActive(value: false);
			ActiveLayoutData = singleLayoutData;
			ActiveTimeUI = singleTimeUI;
		}
		else
		{
			singleLayoutObjct.SetActive(value: false);
			multiLayoutObject_Four.SetActive(value: true);
			ActiveLayoutData = multiLayoutData_Four;
			ActiveTimeUI = multiTimeUI_Four;
		}
		for (int i = 0; i < ActiveLayoutData.Length; i++)
		{
			ActiveLayoutData[i].Init(i, SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0]);
		}
		for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
		{
			int num = (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) ? j : 0;
			int playerUINo = SingletonCustom<GameSettingManager>.Instance.IsSinglePlay ? j : 0;
			ActiveLayoutData[num].InitPlayerUI(num, playerUINo, SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][0]);
		}
		ActiveTimeUI.ShowRemainingTime_HideTextUI();
		miniMapUI.Init();
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < ActiveLayoutData.Length; i++)
		{
			ActiveLayoutData[i].UpdateMethod();
		}
	}
	public Camera GetGlobalCamera()
	{
		return globalCamera;
	}
	public MonsterKill_PointUpUI GetPointUIPref()
	{
		return pointUIPref;
	}
	public Sprite GetPointSprite(int _charaIdx, MonsterKill_EnemyManager.DeadPointTpe _deadPoint)
	{
		return arrayPointSprte[_charaIdx].pointSprte[(int)_deadPoint];
	}
	public void SetPointUp(int _playerNo, GameObject _enemyObj, MonsterKill_EnemyManager.DeadPointTpe _deadPoint)
	{
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay || !SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(_playerNo).GetIsCpu())
		{
			ActiveLayoutData[_playerNo].SetPointUp(_playerNo, _enemyObj, _deadPoint);
		}
	}
	public void SetPoint(int _playerNo, int _playerUINo, int _point, int _addPoint)
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			_playerNo = 0;
		}
		else
		{
			_playerUINo = 0;
		}
		ActiveLayoutData[_playerNo].SetPoint(_playerUINo, _point, _addPoint);
	}
	public void HidePointUI(int _playerNo, int _playerUINo)
	{
		float delay = (float)_playerNo * 0.15f;
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			_playerNo = 0;
		}
		else
		{
			_playerUINo = 0;
		}
		ActiveLayoutData[_playerNo].HidePointUI(_playerUINo, delay);
	}
	public void SetStaminaGauge(int _playerNo, float _staminaLerp, bool _isUseUpStamina)
	{
		ActiveLayoutData[_playerNo].SetStaminaGauge(_staminaLerp, _isUseUpStamina);
	}
	public void SetJoyConButtonPlayerType(int _playerNo, int _userType)
	{
		ActiveLayoutData[_playerNo].SetJoyConButtonPlayerType(_userType);
	}
	public void SetTime(float _time)
	{
		ActiveTimeUI.SetTime(_time);
	}
	public Color GetMiniMapIconColor(int _userType = -1)
	{
		return miniMapUI.GetMiniMapIconColor(_userType);
	}
	public void ShowAnnounceText(int _playerNo)
	{
		if (!SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(_playerNo).GetIsCpu())
		{
			ActiveLayoutData[_playerNo].ShowAnnounceText();
		}
	}
	public void HideAnnounceText(int _playerNo)
	{
		if (!SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(_playerNo).GetIsCpu())
		{
			ActiveLayoutData[_playerNo].HideAnnounceText();
		}
	}
	public float GetTextUIAnimationTime()
	{
		return TEXT_UI_ANIMATION_TIME;
	}
	public void HideUI(int _playerNo)
	{
		ActiveLayoutData[_playerNo].HideUI();
	}
}
