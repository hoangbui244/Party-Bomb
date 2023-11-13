using System;
using System.Collections.Generic;
using UnityEngine;
public class MonsterKill_LayoutData : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fcUI")]
	private MonsterKill_PlayerUI[] playerUI;
	[SerializeField]
	[Header("スタミナUI")]
	private MonsterKill_StaminaUI staminaUI;
	private Dictionary<MonsterKill_PointUpUI, GameObject> pointUIDic = new Dictionary<MonsterKill_PointUpUI, GameObject>();
	[SerializeField]
	[Header("ポイントアップUIを格納するル\u30fcト")]
	private Transform pointUpUIRoot;
	[SerializeField]
	[Header("ポイントアップUI用のマスク")]
	private SpriteMask pointUpMask;
	private readonly int POINT_UP_FRONT_ORDER_BASE = 3;
	private readonly int POINT_UP_BACK_ORDER_BASE = 2;
	[SerializeField]
	[Header("画面右のコントロ\u30fcラ\u30fcUI")]
	private ControllerOperationExplanationUI contollerOperationExplanation;
	[SerializeField]
	[Header("ゲ\u30fcム開始のテキストUI")]
	private SpriteRenderer announceText;
	private Vector3 originAnnounceTextScale;
	private JoyConButton[] arrayJoyConButton;
	public void Init(int _playerNo, int _userType)
	{
		staminaUI.Init(_playerNo);
		pointUpMask.frontSortingOrder = POINT_UP_FRONT_ORDER_BASE + _playerNo;
		pointUpMask.backSortingOrder = POINT_UP_BACK_ORDER_BASE + _playerNo;
		if (_userType < 4)
		{
			contollerOperationExplanation.Init(_isPlayer: true);
			arrayJoyConButton = GetComponentsInChildren<JoyConButton>(includeInactive: true);
			SetJoyConButtonPlayerType(_userType);
			announceText.gameObject.SetActive(value: false);
		}
		else
		{
			contollerOperationExplanation.Init(_isPlayer: false);
			announceText.gameObject.SetActive(value: false);
		}
	}
	public void InitPlayerUI(int _playerNo, int playerUINo, int _userType)
	{
		playerUI[playerUINo].Init(_playerNo, _userType);
	}
	public void UpdateMethod()
	{
		foreach (KeyValuePair<MonsterKill_PointUpUI, GameObject> item in pointUIDic)
		{
			item.Key.UpdateMethod();
		}
		staminaUI.UpdateMethod();
	}
	public void SetPointUp(int _playerNo, GameObject _enemyObj, MonsterKill_EnemyManager.DeadPointTpe _deadPoint)
	{
		MonsterKill_PointUpUI monsterKill_PointUpUI = null;
		foreach (KeyValuePair<MonsterKill_PointUpUI, GameObject> item in pointUIDic)
		{
			if (!item.Key.gameObject.activeSelf)
			{
				monsterKill_PointUpUI = item.Key;
				pointUIDic[monsterKill_PointUpUI] = _enemyObj;
				break;
			}
		}
		if (monsterKill_PointUpUI == null)
		{
			monsterKill_PointUpUI = UnityEngine.Object.Instantiate(SingletonCustom<MonsterKill_UIManager>.Instance.GetPointUIPref(), pointUpUIRoot);
			if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				monsterKill_PointUpUI.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
			}
			monsterKill_PointUpUI.SetPlayerNo(_playerNo);
			monsterKill_PointUpUI.SetOrderInLayer(POINT_UP_FRONT_ORDER_BASE + _playerNo);
			pointUIDic.Add(monsterKill_PointUpUI, _enemyObj);
		}
		monsterKill_PointUpUI.Init(_enemyObj, _deadPoint);
		monsterKill_PointUpUI.gameObject.SetActive(value: true);
		monsterKill_PointUpUI.MoveUp();
	}
	public void SetPoint(int _playerNo, int _point, int _addPoint)
	{
		playerUI[_playerNo].SetPoint(_point, _addPoint);
	}
	public void HidePointUI(int _playerUINo, float _delay)
	{
		Vector3 hidePos = new Vector3(0f, 500f, 0f);
		playerUI[_playerUINo].Hide(_delay, hidePos);
	}
	public void SetStaminaGauge(float _staminaLerp, bool _isUseUpStamina)
	{
		staminaUI.SetStaminaGauge(_staminaLerp, _isUseUpStamina);
	}
	public void ChangeControllerUIType(int _controllerTypeIdx)
	{
		contollerOperationExplanation.ChangeControllerUIType(_controllerTypeIdx);
	}
	public void ShowAnnounceText()
	{
		announceText.transform.localScale = Vector3.zero;
		announceText.gameObject.SetActive(value: true);
		LeanTween.scale(announceText.gameObject, originAnnounceTextScale, SingletonCustom<MonsterKill_UIManager>.Instance.GetTextUIAnimationTime()).setEaseOutBack().setOnComplete((Action)delegate
		{
			LeanTween.delayedCall(announceText.gameObject, 1f, (Action)delegate
			{
				HideAnnounceText();
			});
		});
	}
	public void HideAnnounceText()
	{
		LeanTween.scale(announceText.gameObject, Vector3.zero, SingletonCustom<MonsterKill_UIManager>.Instance.GetTextUIAnimationTime()).setEaseInBack();
		Color color = announceText.color;
		color.a = 0f;
		LeanTween.color(announceText.gameObject, color, SingletonCustom<MonsterKill_UIManager>.Instance.GetTextUIAnimationTime());
		LeanTween.delayedCall(announceText.gameObject, SingletonCustom<MonsterKill_UIManager>.Instance.GetTextUIAnimationTime(), (Action)delegate
		{
			announceText.gameObject.SetActive(value: false);
		});
	}
	public void SetJoyConButtonPlayerType(int _userType)
	{
		for (int i = 0; i < arrayJoyConButton.Length; i++)
		{
			arrayJoyConButton[i].SetPlayerType((JoyConButton.PlayerType)_userType);
			arrayJoyConButton[i].CheckJoyconButton();
		}
	}
	public void HideUI()
	{
	}
}
