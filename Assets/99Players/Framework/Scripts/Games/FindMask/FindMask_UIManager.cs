using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class FindMask_UIManager : SingletonCustom<FindMask_UIManager>
{
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
	private static readonly Color[] CHARA_COLORS = new Color[8]
	{
		new Color(86f / 255f, 0.8862746f, 0.1686275f),
		new Color(1f, 0.3058824f, 32f / 85f),
		new Color(0.3568628f, 172f / 255f, 1f),
		new Color(1f, 0.882353f, 0.04705883f),
		new Color(29f / 51f, 0.3294118f, 41f / 51f),
		new Color(0.01568628f, 0.9490197f, 1f),
		new Color(0.3921569f, 23f / 51f, 0.4235294f),
		new Color(1f, 2f / 3f, 0.1215686f)
	};
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer fade;
	[SerializeField]
	[Header("タ\u30fcンカットインUI")]
	private TurnCutIn turnCutIn;
	[SerializeField]
	[Header("共通レイアウトプレハブ")]
	private CommonWaterPistolBattleUILayout commonUiLayout;
	private readonly Color32 FOCUS_FRAME_COLO = new Color32(249, 59, 15, byte.MaxValue);
	[SerializeField]
	[Header("獲得ポイント表示UIアンカ\u30fc")]
	private Transform getPointUiAnchor;
	[SerializeField]
	[Header("獲得ポイント表示UIプレハブ")]
	private FindMask_GetPointUi getPointUiPrefab;
	private FindMask_GetPointUi createGetPointUi;
	private const float GET_POINT_UI_SCALE = 1.5f;
	[SerializeField]
	[Header("残り時間UI")]
	private CommonGameTimeUI_Font_Time timeUI;
	[SerializeField]
	[Header("残り５秒のカウントダウン")]
	private FindMask_RemainingCountDown remainingCountDown;
	private bool isViewRmainingCountDown;
	private readonly float NEXT_TURN_FADE_TIME = 1f;
	[SerializeField]
	[Header("操作ボタン表示UI（プレイヤ\u30fcが１人）")]
	private GameObject singleControlOperaton;
	[SerializeField]
	[Header("開始時操作表示UI")]
	private ControlInfomationUI firstCtrlUI;
	public void Init()
	{
		DataInit();
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			singleControlOperaton.SetActive(value: true);
		}
		else
		{
			singleControlOperaton.SetActive(value: false);
		}
	}
	private void DataInit()
	{
		firstCtrlUI.SetActive(_active: false);
		firstCtrlUI.SetAlpha(0f);
		isViewRmainingCountDown = false;
		remainingCountDown.SetAcitveCountSp(_isActive: false);
		remainingCountDown.Init();
		createGetPointUi = UnityEngine.Object.Instantiate(getPointUiPrefab, getPointUiAnchor);
		createGetPointUi.Init();
		commonUiLayout.Init(SingletonCustom<FindMask_GameManager>.Instance.PlayFindTime, null, SingletonCustom<FindMask_GameManager>.Instance.IsEightPlsyers);
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			commonUiLayout.SetScoreAnchorOrder(SingletonCustom<FindMask_GameManager>.Instance.ArrayPlayerElement);
			commonUiLayout.SetScoreUnderlayColor(FOCUS_FRAME_COLO);
			SetTurnPlayerFrame();
		}
	}
	public void UpdateMethod()
	{
	}
	public void EndGame()
	{
	}
	public void CloseGameUI()
	{
	}
	private void InitGroupScoreUI(int _groupNo, int _charaNo)
	{
	}
	private void SetCharacterIcon()
	{
	}
	public void SetTurnPlayerFrame()
	{
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			int currentTurnNum = SingletonCustom<FindMask_GameManager>.Instance.CurrentTurnNum;
			commonUiLayout.SetScoreUndelayTurn(SingletonCustom<FindMask_GameManager>.Instance.ArrayPlayerElement[currentTurnNum]);
		}
	}
	public void SetTime(float _time)
	{
		if ((double)_time < 5.0)
		{
			if (!isViewRmainingCountDown)
			{
				remainingCountDown.SetAcitveCountSp(_isActive: true);
				isViewRmainingCountDown = true;
			}
			remainingCountDown.SetTime(_time);
		}
		commonUiLayout.SetTime(_time);
	}
	public void CharacterScoreUIUpdate()
	{
		int currentTurnNum = SingletonCustom<FindMask_GameManager>.Instance.CurrentTurnNum;
		commonUiLayout.SetScore(SingletonCustom<FindMask_GameManager>.Instance.ArrayPlayerElement[currentTurnNum], SingletonCustom<FindMask_ScoreManager>.Instance.ArrayScore[currentTurnNum]);
	}
	public void ViewGetPoint(int _charaNo, int _point, Vector3 _worldPos)
	{
		Color pointColor = CHARA_COLORS[_charaNo];
		createGetPointUi.SetPointColor(pointColor);
		createGetPointUi.SetScale(1.5f);
		createGetPointUi.Show(_point, _worldPos);
	}
	public void ViewFirstControlInfo()
	{
		firstCtrlUI.SetActive(_active: true);
		ControlInfomationFade(firstCtrlUI, 1f, 0.5f);
		ControlInfomationFade(firstCtrlUI, 0f, 0.5f, 3f, delegate
		{
			firstCtrlUI.SetActive(_active: false);
		});
	}
	public void ResetUI()
	{
		isViewRmainingCountDown = false;
		remainingCountDown.SetAcitveCountSp(_isActive: false);
		remainingCountDown.Init();
	}
	public void HideRemainCountDown()
	{
		if (isViewRmainingCountDown)
		{
			isViewRmainingCountDown = false;
			remainingCountDown.SetAcitveCountSp(_isActive: false);
		}
	}
	public void ShowTurnCutIn(int _playerNo, float _delay = 0f)
	{
		if (_playerNo < 4)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(_playerNo);
			turnCutIn.ShowTurnCutIn(_playerNo, _delay, delegate
			{
				SingletonCustom<FindMask_CharacterManager>.Instance.SetCursorActive(_active: true);
				SingletonCustom<FindMask_GameManager>.Instance.State = FindMask_GameManager.STATE.CHARA_MOVE;
			});
		}
		else
		{
			SingletonCustom<FindMask_CharacterManager>.Instance.SetCursorActive(_active: true);
			SingletonCustom<FindMask_GameManager>.Instance.State = FindMask_GameManager.STATE.CHARA_MOVE;
		}
	}
	public void NextTurnPlayerFade()
	{
		SingletonCustom<FindMask_CharacterManager>.Instance.ResetCharacterCursor();
		ResetUI();
		SingletonCustom<FindMask_MaskManager>.Instance.ResetSelect();
		SingletonCustom<FindMask_GameManager>.Instance.ResetGame();
		SetTurnPlayerFrame();
		ShowTurnCutIn(SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo, 0.5f);
	}
	public void Fade(float _time, float _delay, Action _act)
	{
		Color color = fade.color;
		color.a = 1f;
		fade.SetAlpha(0f);
		fade.gameObject.SetActive(value: true);
		LeanTween.value(fade.gameObject, 0f, 1f, _time * 0.5f).setDelay(_delay).setEaseOutCubic()
			.setOnUpdate(delegate(float _value)
			{
				fade.SetAlpha(_value);
			})
			.setOnComplete((Action)delegate
			{
				_act();
			});
		color.a = 0f;
		LeanTween.value(fade.gameObject, 1f, 0f, _time * 0.5f).setDelay(_time * 0.5f + _delay).setEaseOutCubic()
			.setOnUpdate(delegate(float _value)
			{
				fade.SetAlpha(_value);
			})
			.setOnComplete((Action)delegate
			{
				fade.gameObject.SetActive(fade);
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
