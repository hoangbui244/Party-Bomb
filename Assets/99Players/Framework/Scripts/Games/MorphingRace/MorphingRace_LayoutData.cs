using System;
using UnityEngine;
public class MorphingRace_LayoutData : MonoBehaviour
{
	[SerializeField]
	[Header("タイムUI")]
	private CommonGameTimeUI_Font_Time timeUI;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer playerIcon;
	[SerializeField]
	[Header("操作吹き出しコントロ\u30fcラ\u30fcUI")]
	private ControllerOperationBalloonUI contollerOperationBalloon;
	[SerializeField]
	[Header("画面右のコントロ\u30fcラ\u30fcUI")]
	private ControllerOperationExplanationUI contollerOperationExplanation;
	[SerializeField]
	[Header("変動する順位アイコン")]
	private RaceCurrentRankIcon currentRankIcon;
	[SerializeField]
	[Header("ゴ\u30fcル順位アイコン")]
	private SpriteRenderer goalRankIcon;
	[SerializeField]
	[Header("ゴ\u30fcル順位アイコンのゴ\u30fcル後アニメ\u30fcション")]
	private ResultPlacementAnimation goalRankAnimation;
	private JoyConButton[] arrayJoyConButton;
	public void Init(int _playerNo)
	{
		bool isPlayer = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_playerNo][0] < 4;
		contollerOperationBalloon.Init(isPlayer);
		contollerOperationExplanation.Init(isPlayer);
		currentRankIcon.Init();
		goalRankIcon.gameObject.SetActive(value: false);
		arrayJoyConButton = GetComponentsInChildren<JoyConButton>(includeInactive: true);
	}
	public void SetTime(float _time)
	{
		timeUI.SetTime(_time);
	}
	public void SetPlayerIconActive(bool _isActive)
	{
		playerIcon.gameObject.SetActive(_isActive);
	}
	public void SetPlayerIcon(int _userType)
	{
		if (_userType < 4)
		{
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_you");
			}
			else
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + (_userType + 1).ToString() + "p");
			}
		}
		else
		{
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + (_userType - 4 + 1).ToString());
		}
	}
	public void SetControllerBalloonActive(int _balloonIdx, bool _isFade, bool _isActive)
	{
		contollerOperationBalloon.SetControllerBalloonActive(_balloonIdx, _isFade, _isActive);
	}
	public void ChangeControllerUIType(int _controllerTypeIdx)
	{
		contollerOperationExplanation.ChangeControllerUIType(_controllerTypeIdx);
	}
	public void SetCurrentRankIcon(int _rankNum)
	{
		currentRankIcon.SetRankIcon(_rankNum);
	}
	public void SetCurrentRankIconScaling(float _scaleTime)
	{
		currentRankIcon.SetRankIconScaling(_scaleTime);
	}
	public void ShowGoalRankIcon(int _rankNum)
	{
		goalRankIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_" + (_rankNum - 1).ToString());
		Vector3 localScale = goalRankIcon.transform.localScale;
		goalRankIcon.transform.localScale = Vector3.zero;
		goalRankIcon.gameObject.SetActive(value: true);
		LeanTween.scale(goalRankIcon.gameObject, localScale, 0.25f);
		LeanTween.delayedCall(base.gameObject, 0.25f, (Action)delegate
		{
			goalRankAnimation.Play();
		});
	}
	public void SetJoyConButtonPlayerType(int _playerNo)
	{
		for (int i = 0; i < arrayJoyConButton.Length; i++)
		{
			arrayJoyConButton[i].SetPlayerType((JoyConButton.PlayerType)_playerNo);
			arrayJoyConButton[i].CheckJoyconButton();
		}
	}
	public void HideUI()
	{
		goalRankIcon.gameObject.SetActive(value: false);
		contollerOperationBalloon.HideUI();
	}
}
