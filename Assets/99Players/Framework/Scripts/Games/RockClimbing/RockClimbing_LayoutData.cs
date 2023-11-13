using System;
using UnityEngine;
public class RockClimbing_LayoutData : MonoBehaviour
{
	[SerializeField]
	[Header("タイムUI")]
	private CommonGameTimeUI_Font_Time timeUI;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer playerIcon;
	[SerializeField]
	[Header("「なげる」のUI")]
	private GameObject throwUI;
	[SerializeField]
	[Header("「なげる」のUI（JP）")]
	private GameObject throwUI_JP;
	[SerializeField]
	[Header("「なげる」のUI（EN）")]
	private GameObject throwUI_EN;
	[SerializeField]
	[Header("操作吹き出しコントロ\u30fcラ\u30fcUI")]
	private ControllerOperationBalloonUI contollerOperationBalloon;
	[SerializeField]
	[Header("画面右のコントロ\u30fcラ\u30fcUI")]
	private ControllerOperationExplanationUI contollerOperationExplanation;
	[SerializeField]
	[Header("ゴ\u30fcル順位")]
	private SpriteRenderer goalRankIcon;
	[SerializeField]
	[Header("ゴ\u30fcル順位のゴ\u30fcル後アニメ\u30fcション")]
	private ResultPlacementAnimation goalRankAnimation;
	[SerializeField]
	[Header("障害物示唆の注意マ\u30fcク用のマスク")]
	private SpriteMask obstacleCautionMask;
	[SerializeField]
	[Header("障害物示唆の注意マ\u30fcク")]
	private SpriteRenderer obstacleCautionIcon;
	private JoyConButton[] arrayJoyConButton;
	public void Init(int _playerNo)
	{
		if (Localize_Define.Language == Localize_Define.LanguageType.Japanese)
		{
			throwUI_JP.gameObject.SetActive(value: true);
			throwUI_EN.gameObject.SetActive(value: false);
		}
		else
		{
			throwUI_JP.gameObject.SetActive(value: false);
			throwUI_EN.gameObject.SetActive(value: true);
		}
		throwUI.SetActive(value: false);
		bool isPlayer = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[_playerNo][0] < 4;
		contollerOperationBalloon.Init(isPlayer);
		contollerOperationExplanation.Init(isPlayer);
		goalRankIcon.gameObject.SetActive(value: false);
		obstacleCautionIcon.gameObject.SetActive(value: false);
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
	public void SetThrowUIPos(Vector3 _pos)
	{
		throwUI.transform.SetPosition(_pos.x, _pos.y, throwUI.transform.position.z);
	}
	public void SetThrowControllerBalloonActive(bool _isActive)
	{
		throwUI.SetActive(_isActive);
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
	public void SetObstacleCautionOrderInLayer(int _playerNo)
	{
		obstacleCautionIcon.sortingOrder -= _playerNo;
		obstacleCautionMask.isCustomRangeActive = true;
		obstacleCautionMask.frontSortingOrder = obstacleCautionIcon.sortingOrder + 1;
		obstacleCautionMask.backSortingOrder = obstacleCautionIcon.sortingOrder - 1;
	}
	public void SetObstacleCautionIconPos(Vector3 _pos)
	{
		obstacleCautionIcon.transform.SetPosition(_pos.x, _pos.y, obstacleCautionIcon.transform.position.z);
	}
	public void ShowObstacleDropCautionIcon()
	{
		obstacleCautionIcon.SetAlpha(0f);
		obstacleCautionIcon.gameObject.SetActive(value: true);
		LeanTween.cancel(obstacleCautionIcon.gameObject);
		LeanTween.color(obstacleCautionIcon.gameObject, Color.white, 0.375f).setLoopPingPong(2).setOnComplete((Action)delegate
		{
			obstacleCautionIcon.gameObject.SetActive(value: false);
		});
	}
	public void StopObstacleDropCautionIcon()
	{
		if (obstacleCautionIcon.gameObject.activeSelf)
		{
			LeanTween.cancel(obstacleCautionIcon.gameObject);
			obstacleCautionIcon.gameObject.SetActive(value: false);
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
