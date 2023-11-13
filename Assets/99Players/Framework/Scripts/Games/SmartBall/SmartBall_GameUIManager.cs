using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
public class SmartBall_GameUIManager : SingletonCustom<SmartBall_GameUIManager>
{
	[Serializable]
	public struct UserUIData
	{
		[Header("ユ\u30fcザ\u30fcUIデ\u30fcタをまとめるアンカ\u30fc")]
		public GameObject userUIDataAnchor;
		[Header("ユ\u30fcザ\u30fcUIデ\u30fcタ")]
		public SmartBall_UserUIData[] userUIDatas;
	}
	[Serializable]
	public struct PartitionLinePosData
	{
		[Header("分割線[0]の座標")]
		public Vector2 partitionLinePos_0;
		[Header("分割線[1]の座標")]
		public Vector2 partitionLinePos_1;
		[Header("分割線[2]の座標")]
		public Vector2 partitionLinePos_2;
	}
	[Serializable]
	private class ControlInfomationUI
	{
		[Header("アンカ\u30fc")]
		public GameObject anchor;
		[Header("操作情報を示す画像デ\u30fcタ")]
		public SpriteRenderer[] infomationSpriteUI;
		[Header("操作情報を示す文字デ\u30fcタ")]
		public TextMeshPro[] infomationTextUI;
		public float NowAlpha
		{
			get;
			set;
		}
		public void SetActive(bool _active)
		{
			anchor.SetActive(_active);
		}
		public void SetAlpha(float _alpha)
		{
			for (int i = 0; i < infomationSpriteUI.Length; i++)
			{
				infomationSpriteUI[i].SetAlpha(_alpha);
			}
			for (int j = 0; j < infomationTextUI.Length; j++)
			{
				infomationTextUI[j].SetAlpha(_alpha);
			}
			NowAlpha = _alpha;
		}
	}
	[SerializeField]
	[Header("プレイヤ\u30fc１人の時のユ\u30fcザ\u30fcUIデ\u30fcタ")]
	private UserUIData userUIData_1Player;
	[SerializeField]
	[Header("プレイヤ\u30fc２人の時のユ\u30fcザ\u30fcUIデ\u30fcタ")]
	private UserUIData userUIData_2Player;
	[SerializeField]
	[Header("プレイヤ\u30fc３人の時のユ\u30fcザ\u30fcUIデ\u30fcタ")]
	private UserUIData userUIData_3Player;
	[SerializeField]
	[Header("プレイヤ\u30fc４人の時のユ\u30fcザ\u30fcUIデ\u30fcタ")]
	private UserUIData userUIData_4Player;
	[SerializeField]
	[Header("分割線")]
	private Transform[] partitionLine;
	[SerializeField]
	[Header("プレイヤ\u30fc１人の時の分割線の位置デ\u30fcタ")]
	private PartitionLinePosData partitionLineData_1Player;
	[SerializeField]
	[Header("プレイヤ\u30fc２人の時の分割線の位置デ\u30fcタ")]
	private PartitionLinePosData partitionLineData_2Player;
	[SerializeField]
	[Header("プレイヤ\u30fc３人の時の分割線の位置デ\u30fcタ")]
	private PartitionLinePosData partitionLineData_3Player;
	[SerializeField]
	[Header("プレイヤ\u30fc４人の時の分割線の位置デ\u30fcタ")]
	private PartitionLinePosData partitionLineData_4Player;
	[SerializeField]
	[Header("プレイヤ\u30fc１人の時：終了テキストの大きさデ\u30fcタ")]
	private Vector3[] finishScale_1Player;
	[SerializeField]
	[Header("プレイヤ\u30fc２人の時：終了テキストの大きさデ\u30fcタ")]
	private Vector3[] finishScale_2Player;
	[SerializeField]
	[Header("プレイヤ\u30fc３人の時：終了テキストの大きさデ\u30fcタ")]
	private Vector3[] finishScale_3Player;
	[SerializeField]
	[Header("プレイヤ\u30fc４人の時：終了テキストの大きさデ\u30fcタ")]
	private Vector3[] finishScale_4Player;
	[SerializeField]
	[Header("組名画像")]
	private SpriteRenderer groupNameSprite;
	[SerializeField]
	[Header("フェ\u30fcド画像")]
	private SpriteRenderer fade;
	[SerializeField]
	[Header("共通スプライトアトラスデ\u30fcタ")]
	private SpriteAtlas common_SpriteAtlas;
	private readonly float NEXT_GROUP_2_FADE_TIME = 1f;
	private UserUIData usingUserUIData;
	[SerializeField]
	[Header("スキップ表示UI")]
	private ControlInfomationUI skipCtrlUI;
	[SerializeField]
	[Header("プレイヤ\u30fc１人の時：スキップ操作表示UIの位置デ\u30fcタ")]
	private float skipCtrlUIPosX_1Player;
	[SerializeField]
	[Header("プレイヤ\u30fc２人の時：スキップ操作表示UIの位置デ\u30fcタ")]
	private float skipCtrlUIPosX_2Player;
	[SerializeField]
	[Header("プレイヤ\u30fc３人の時：スキップ操作表示UIの位置デ\u30fcタ")]
	private float skipCtrlUIPosX_3Player;
	private void Awake()
	{
		fade.SetAlpha(0f);
		skipCtrlUI.SetActive(_active: false);
		skipCtrlUI.SetAlpha(0f);
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP && SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
		{
			groupNameSprite.gameObject.SetActive(value: true);
		}
		else
		{
			groupNameSprite.gameObject.SetActive(value: false);
		}
	}
	public void Init(SmartBall_MainCharacterManager.UserData[] _userData, bool _isGroup1)
	{
		int num = 0;
		for (int i = 0; i < _userData.Length; i++)
		{
			if (_userData[i].isPlayer)
			{
				num++;
			}
		}
		SetUseUserUIData(num);
		for (int j = 0; j < usingUserUIData.userUIDatas.Length; j++)
		{
			usingUserUIData.userUIDatas[j].Init(_userData[j].teamType, _userData[j].userType);
			SetPoint(j, 0f);
		}
		if (groupNameSprite.gameObject.activeSelf)
		{
			groupNameSprite.sprite = common_SpriteAtlas.GetSprite(_isGroup1 ? "_set_1st_b" : "_set_2nd_b");
			switch (num)
			{
			case 1:
				groupNameSprite.transform.SetLocalPositionY(265f);
				break;
			case 2:
				groupNameSprite.transform.SetLocalPositionY(310f);
				break;
			}
		}
	}
	public void ShowGameOverDisplay(int _dataNo, bool _isLastTeam)
	{
		SetPartitionFade(_dataNo, _isFadeIn: true);
		if (!_isLastTeam)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_short");
		}
		else
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
		}
		usingUserUIData.userUIDatas[_dataNo].PlayGameOverEffect(_isLastTeam);
	}
	public void ViewSkipControlInfo()
	{
		skipCtrlUI.SetActive(_active: true);
		ControlInfomationFade(skipCtrlUI, 1f, 0.5f);
	}
	public void HideSkipControlInfo()
	{
		skipCtrlUI.SetActive(_active: false);
		skipCtrlUI.SetAlpha(0f);
	}
	public void SetUseUserUIData(int _playerNum)
	{
		userUIData_1Player.userUIDataAnchor.SetActive(value: false);
		userUIData_2Player.userUIDataAnchor.SetActive(value: false);
		userUIData_3Player.userUIDataAnchor.SetActive(value: false);
		userUIData_4Player.userUIDataAnchor.SetActive(value: false);
		switch (_playerNum)
		{
		case 1:
			usingUserUIData = userUIData_1Player;
			SetPartitionLinePosData(partitionLineData_1Player);
			SetFinishTextScale(usingUserUIData, finishScale_1Player);
			SetSkipCtrlUIPosData(skipCtrlUIPosX_1Player);
			break;
		case 2:
			usingUserUIData = userUIData_2Player;
			SetPartitionLinePosData(partitionLineData_2Player);
			SetFinishTextScale(usingUserUIData, finishScale_2Player);
			SetSkipCtrlUIPosData(skipCtrlUIPosX_2Player);
			break;
		case 3:
			usingUserUIData = userUIData_3Player;
			SetPartitionLinePosData(partitionLineData_3Player);
			SetFinishTextScale(usingUserUIData, finishScale_3Player);
			SetSkipCtrlUIPosData(skipCtrlUIPosX_3Player);
			break;
		case 4:
			usingUserUIData = userUIData_4Player;
			SetPartitionLinePosData(partitionLineData_4Player);
			SetFinishTextScale(usingUserUIData, finishScale_4Player);
			break;
		}
		usingUserUIData.userUIDataAnchor.SetActive(value: true);
	}
	private void SetPartitionLinePosData(PartitionLinePosData _partitionLinePosData)
	{
		partitionLine[0].localPosition = _partitionLinePosData.partitionLinePos_0;
		partitionLine[1].localPosition = _partitionLinePosData.partitionLinePos_1;
		partitionLine[2].localPosition = _partitionLinePosData.partitionLinePos_2;
	}
	private void SetSkipCtrlUIPosData(float _skipCtrlUIPos)
	{
		skipCtrlUI.anchor.transform.SetLocalPositionX(_skipCtrlUIPos);
	}
	private void SetFinishTextScale(UserUIData _userUIData, Vector3[] _finishScale)
	{
		for (int i = 0; i < _userUIData.userUIDatas.Length; i++)
		{
			_userUIData.userUIDatas[i].SetFinishScale(_finishScale[i]);
		}
	}
	public void SetPoint(int _dataNo, float _height)
	{
		usingUserUIData.userUIDatas[_dataNo].SetPointData(_height);
	}
	public void SetPartitionFade(int _dataNo, bool _isFadeIn, Action _callBack = null)
	{
		if (_isFadeIn)
		{
			usingUserUIData.userUIDatas[_dataNo].PartitionFadeIn(_callBack);
		}
		else
		{
			usingUserUIData.userUIDatas[_dataNo].PartitionFadeOut(_callBack);
		}
	}
	public void SetFallTimeLimit(int _dataNo, float _time, bool _isTimeLimitActive)
	{
		usingUserUIData.userUIDatas[_dataNo].SetTimeLimit(_time, _isTimeLimitActive);
	}
	public void SetFadeInFirstControlInfomation(int _dataNo)
	{
		usingUserUIData.userUIDatas[_dataNo].FadeProcess_ControlInfomationUI(_fadeIn: true);
	}
	public void SetFadeOutFirstControlInfomation(int _dataNo)
	{
		usingUserUIData.userUIDatas[_dataNo].FadeProcess_ControlInfomationUI(_fadeIn: false);
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
		LeanTween.value(fade.gameObject, isView ? 0f : 1f, isView ? 1f : 0f, _fadeTime).setOnUpdate(delegate(float val)
		{
			alpha = fade.color;
			alpha.a = val;
			fade.color = alpha;
		});
	}
	private void ControlInfomationFade(ControlInfomationUI _infoUI, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		StartCoroutine(_ControlInfomationFade(_infoUI, _setAlpha, _fadeTime, _delayTime, _callback));
	}
	private IEnumerator _ControlInfomationFade(ControlInfomationUI _infoUI, float _setAlpha, float _fadeTime, float _delayTime = 0f, Action _callback = null)
	{
		float time = 0f;
		yield return new WaitForSeconds(_delayTime);
		float startAlpha = _infoUI.NowAlpha;
		while (time < _fadeTime)
		{
			_infoUI.SetAlpha(Mathf.Lerp(startAlpha, _setAlpha, time / _fadeTime));
			time += Time.deltaTime;
			yield return null;
		}
		_infoUI.SetAlpha(_setAlpha);
		_callback?.Invoke();
	}
}
