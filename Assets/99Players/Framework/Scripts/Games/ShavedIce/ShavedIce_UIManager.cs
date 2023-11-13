using System;
using UnityEngine;
public class ShavedIce_UIManager : SingletonCustom<ShavedIce_UIManager>
{
	[SerializeField]
	[Header("ユ\u30fcザ\u30fcUIデ\u30fcタ")]
	private ShavedIce_UserUIData[] userUIDatas;
	[SerializeField]
	[Header("組名画像")]
	private SpriteRenderer groupNameSprite;
	[SerializeField]
	[Header("画面フェ\u30fcド画像")]
	private SpriteRenderer screenFade;
	private readonly float SCREEN_FADE_TIME = 1f;
	private void Awake()
	{
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP && SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
		{
			groupNameSprite.gameObject.SetActive(value: true);
		}
		else
		{
			groupNameSprite.gameObject.SetActive(value: false);
		}
		screenFade.SetAlpha(0f);
	}
	public void Init(ShavedIce_PlayerManager.UserData[] _userData, bool _isGroup1)
	{
		int num = 0;
		for (int i = 0; i < _userData.Length; i++)
		{
			if (_userData[i].isPlayer)
			{
				num++;
			}
		}
		for (int j = 0; j < userUIDatas.Length; j++)
		{
			userUIDatas[j].Init(_userData[j].teamType, _userData[j].userType);
			SetTowerHeightCalcNumbers(j, 0f);
		}
		if (groupNameSprite.gameObject.activeSelf)
		{
			groupNameSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, _isGroup1 ? "_set_1st_b" : "_set_2nd_b");
		}
	}
	public void SetFadeInControlInfomation(int _dataNo)
	{
		userUIDatas[_dataNo].FadeProcess_ControlInfomationUI(_fadeIn: true);
	}
	public void SetFadeOutControlInfomation(int _dataNo)
	{
		userUIDatas[_dataNo].FadeProcess_ControlInfomationUI(_fadeIn: false);
	}
	public void StartTowerHeightCalc()
	{
		for (int i = 0; i < userUIDatas.Length; i++)
		{
			userUIDatas[i].ShowTowerHeightCalcLine();
		}
	}
	public void SetTowerHeightCalcLinePos(int _dataNo, Vector3 _worldPos, bool _isGroup1)
	{
		userUIDatas[_dataNo].SetTowerHeightCalcLinePos(_worldPos, _isGroup1);
	}
	public void SetTowerHeightCalcNumbers(int _dataNo, float _height)
	{
		userUIDatas[_dataNo].SetHeightData(_height);
	}
	public void ShowTowerHeight(int _dataNo)
	{
		userUIDatas[_dataNo].ShowTowerHeightCalcRecord();
		userUIDatas[_dataNo].PartitionFadeIn();
	}
	public void StartScreenFade(Action _fadeInCallBack = null, Action _fadeOutCallBack = null)
	{
		Fade(isView: true, SCREEN_FADE_TIME);
		LeanTween.delayedCall(base.gameObject, SCREEN_FADE_TIME, (Action)delegate
		{
			if (_fadeInCallBack != null)
			{
				_fadeInCallBack();
			}
			Fade(isView: false, SCREEN_FADE_TIME);
			LeanTween.delayedCall(base.gameObject, SCREEN_FADE_TIME, (Action)delegate
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
}
