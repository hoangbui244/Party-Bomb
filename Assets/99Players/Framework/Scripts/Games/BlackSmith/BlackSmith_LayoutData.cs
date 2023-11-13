using System;
using TMPro;
using UnityEngine;
public class BlackSmith_LayoutData : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer playerIcon;
	[SerializeField]
	[Header("キャラクタ\u30fcアイコン")]
	private SpriteRenderer characterIcon;
	private readonly string[] CHARACTER_SPRITE_NAME = new string[8]
	{
		"character_yuto_02",
		"character_hina_02",
		"character_ituki_02",
		"character_souta_02",
		"character_takumi_02",
		"character_rin_02",
		"character_akira_02",
		"character_rui_02"
	};
	[SerializeField]
	[Header("作成した個数のアンカ\u30fc")]
	private GameObject createWeaponCntAnchor;
	[SerializeField]
	[Header("作成した個数")]
	private SpriteNumbers createWeaponCnt;
	private readonly int CREATE_WEAPON_CNT_FRONT_ORDER_BASE = 6;
	private readonly int CREATE_WEAPON_CNT_TARGET_ORDER_BASE = 5;
	private readonly int CREATE_WEAPON_CNT_BACK_ORDER_BASE = 4;
	[SerializeField]
	[Header("描画優先度を変更する対象")]
	private SpriteRenderer[] arraySortOrderWeaponCnt;
	[SerializeField]
	[Header("作成した個数UIのマスク")]
	private SpriteMask sortOrderWeaponCntMask;
	[SerializeField]
	[Header("ゲ\u30fcジUI")]
	private BlackSmith_GaugeUI gaugeUI;
	[SerializeField]
	[Header("作成パ\u30fcセント")]
	private TextMeshPro createPercentText;
	[SerializeField]
	[Header("画面右のコントロ\u30fcラ\u30fcUI")]
	private ControllerOperationExplanationUI contollerOperationExplanation;
	[SerializeField]
	[Header("ゲ\u30fcム開始のテキストUI")]
	private SpriteRenderer announceText;
	private Vector3 originAnnounceTextScale;
	[SerializeField]
	[Header("スピ\u30fcドアップのテキストUI")]
	private SpriteRenderer speedUpText;
	private Vector3 originSpeedUpTextScale;
	private JoyConButton[] arrayJoyConButton;
	public void Init(int _playerNo, int _userType)
	{
		SetPlayerIcon(_userType);
		SetCharacterIcon(_userType);
		SetCreateWeaponCnt(0);
		sortOrderWeaponCntMask.frontSortingOrder = CREATE_WEAPON_CNT_FRONT_ORDER_BASE + _playerNo * 2;
		sortOrderWeaponCntMask.backSortingOrder = CREATE_WEAPON_CNT_BACK_ORDER_BASE + _playerNo * 2;
		for (int i = 0; i < arraySortOrderWeaponCnt.Length; i++)
		{
			arraySortOrderWeaponCnt[i].sortingOrder = CREATE_WEAPON_CNT_TARGET_ORDER_BASE + _playerNo * 2;
		}
		gaugeUI.Init(_playerNo);
		SetCreatePercent(0);
		if (_userType < 4)
		{
			contollerOperationExplanation.Init(_isPlayer: true);
			arrayJoyConButton = GetComponentsInChildren<JoyConButton>(includeInactive: true);
			SetJoyConButtonPlayerType(_userType);
			speedUpText.gameObject.SetActive(value: false);
			originSpeedUpTextScale = speedUpText.transform.localScale;
		}
		else
		{
			contollerOperationExplanation.Init(_isPlayer: false);
			speedUpText.gameObject.SetActive(value: false);
		}
	}
	public void UpdateMethod()
	{
		gaugeUI.UpdateMethod();
	}
	public void SetPlayerIcon(int _userType)
	{
		if (_userType < 4)
		{
			playerIcon.gameObject.SetActive(value: true);
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_you");
			}
			else
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + (_userType + 1).ToString() + "p");
			}
		}
		else if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			playerIcon.gameObject.SetActive(value: false);
		}
		else
		{
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + (_userType - 4 + 1).ToString());
		}
	}
	private void SetCharacterIcon(int _userType)
	{
		characterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARACTER_SPRITE_NAME[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_userType]]);
	}
	public void SetCreateWeaponCnt(int _cnt)
	{
		createWeaponCnt.Set(_cnt);
	}
	public void HidePointUI(float _delay, Vector3 _hidePos)
	{
		LeanTween.moveLocal(createWeaponCntAnchor, createWeaponCntAnchor.transform.localPosition + _hidePos, 1.25f).setEaseInQuint().setDelay(_delay);
	}
	public int GetGaugePatternLength()
	{
		return gaugeUI.GetGaugePattenLength();
	}
	public void SetGaugeFadeActive(bool _isActive)
	{
		gaugeUI.SetGaugeFadeActive(_isActive);
	}
	public void SetGaugeFade(bool _fadeIn)
	{
		gaugeUI.SetGaugeFade(_fadeIn);
	}
	public int GetBarMoveDir()
	{
		return gaugeUI.GetBarMoveDir();
	}
	public int GetBarIdx()
	{
		return gaugeUI.GetBarIdx();
	}
	public bool IsCanPerfectInput()
	{
		return gaugeUI.IsCanPerfectInput();
	}
	public bool IsPerfectBetweenMinMax()
	{
		return gaugeUI.IsPerfectBetweenMinMax();
	}
	public bool IsNiceBetweenMinMax()
	{
		return gaugeUI.IsNiceBetweenMinMax();
	}
	public bool IsCanGoodInput()
	{
		return gaugeUI.IsCanGoodInput();
	}
	public bool IsGoodBetweenMinMax()
	{
		return gaugeUI.IsGoodBetweenMinMax();
	}
	public bool IsInputTiming(int _idx, BlackSmith_PlayerManager.EvaluationType _evaluationType, float _inputTiming)
	{
		return gaugeUI.IsInputTiming(_idx, _evaluationType, _inputTiming);
	}
	public float GetInputTiming(int _idx, BlackSmith_PlayerManager.EvaluationType _evaluationType)
	{
		return gaugeUI.GetInputTiming(_idx, _evaluationType);
	}
	public void PlayEvaluationEffect(int _playerNo, BlackSmith_PlayerManager.EvaluationType _evaluationType)
	{
		gaugeUI.PlayEvaluationEffect(_playerNo, _evaluationType);
	}
	public void SetCreatePercent(int _percent)
	{
		createPercentText.text = _percent.ToString("0") + "%";
	}
	public void ChangeControllerUIType(int _controllerTypeIdx)
	{
		contollerOperationExplanation.ChangeControllerUIType(_controllerTypeIdx);
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
