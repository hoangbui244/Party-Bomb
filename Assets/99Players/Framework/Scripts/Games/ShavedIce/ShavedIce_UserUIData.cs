using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class ShavedIce_UserUIData : MonoBehaviour
{
	[Serializable]
	public struct UserUIData
	{
		[Header("仕切りフェ\u30fcド画像")]
		public SpriteRenderer partitionFade;
		[Header("ユ\u30fcザ\u30fcアイコン")]
		public SpriteRenderer userIcon;
		[Header("チ\u30fcムなし表示UIアンカ\u30fc")]
		public GameObject singleUIAnchor;
		[Header("チ\u30fcムなし用のキャラクタ\u30fcアイコン")]
		public SpriteRenderer singleCharacterIcon;
		[Header("所属チ\u30fcム表示UIアンカ\u30fc")]
		public GameObject teamUIAnchor;
		[Header("所属チ\u30fcム表示用の下敷き")]
		public SpriteRenderer teamUnderlay;
		[Header("所属チ\u30fcム表示用のチ\u30fcム名")]
		public SpriteRenderer teamName;
		[Header("所属チ\u30fcム表示用のキャラクタ\u30fcアイコン")]
		public SpriteRenderer teamCharacterIcon;
	}
	[Serializable]
	private struct ControlInfomationUI
	{
		[NonReorderable]
		[Header("操作情報を示す画像デ\u30fcタ")]
		public SpriteRenderer[] infomationSpriteUI;
		[NonReorderable]
		[Header("操作情報を示す文字デ\u30fcタ")]
		public TextMeshPro[] infomationTextUI;
	}
	[Serializable]
	private struct TowerHeightCalcUI
	{
		[Header("高さ計測ラインのアンカ\u30fc")]
		public Transform towerHeightCalcLineAnchor;
		[Header("高さ計測黒ラインの画像")]
		public SpriteRenderer towerHeightCalcLineSp_Black;
		[Header("高さ計測白ラインの画像")]
		public SpriteRenderer towerHeightCalcLineSp_White;
		[Header("高さ計測ラインの数値アンカ\u30fc")]
		public Transform towerHeightCalcLineNumbersAnchor;
		[Header("高さ計測ラインの数値(正数値)")]
		public SpriteNumbers towerHeightCalcLine_Numbers;
		[Header("高さ計測ラインの数値(小数点)")]
		public SpriteNumbers towerHeightCalcLine_DecimalNumbers;
		[Header("画面中央に表示する高さの記録のアンカ\u30fc")]
		public Transform towerHeightRecordAnchor;
		[Header("画面中央に表示する高さの数値(正数値)")]
		public SpriteNumbers towerHeightRecord_Numbers;
		[Header("画面中央に表示する高さの数値(小数点)")]
		public SpriteNumbers towerHeightRecord_DecimalNumbers;
	}
	[SerializeField]
	[Header("ユ\u30fcザ\u30fcUIデ\u30fcタ")]
	private UserUIData userUIData;
	[SerializeField]
	[Header("高さ計測UIデ\u30fcタ")]
	private TowerHeightCalcUI towerHeightCalcUI;
	[SerializeField]
	[Header("操作情報UIデ\u30fcタ")]
	private ControlInfomationUI controlInfomationUI;
	[SerializeField]
	[Header("操作説明")]
	private GameObject controlHelp;
	private ShavedIce_Define.UserType userType;
	private ShavedIce_Define.TeamType teamType;
	private Vector3 towerHeightCalcRecordAnchor_DefScale;
	private void Awake()
	{
		towerHeightCalcRecordAnchor_DefScale = towerHeightCalcUI.towerHeightRecordAnchor.localScale;
	}
	public void Init(ShavedIce_Define.TeamType _teamType, ShavedIce_Define.UserType _userType)
	{
		userType = _userType;
		teamType = _teamType;
		controlHelp.SetActive(value: false);
		if (userType <= ShavedIce_Define.UserType.PLAYER_4)
		{
			controlHelp.SetActive(value: true);
		}
		string name = "";
		switch (userType)
		{
		case ShavedIce_Define.UserType.PLAYER_1:
			name = (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay ? "_screen_you" : "_screen_1p");
			break;
		case ShavedIce_Define.UserType.PLAYER_2:
			name = "_screen_2p";
			break;
		case ShavedIce_Define.UserType.PLAYER_3:
			name = "_screen_3p";
			break;
		case ShavedIce_Define.UserType.PLAYER_4:
			name = "_screen_4p";
			break;
		case ShavedIce_Define.UserType.CPU_1:
			name = "_screen_cp1";
			break;
		case ShavedIce_Define.UserType.CPU_2:
			name = "_screen_cp2";
			break;
		case ShavedIce_Define.UserType.CPU_3:
			name = "_screen_cp3";
			break;
		case ShavedIce_Define.UserType.CPU_4:
			name = "_screen_cp4";
			break;
		case ShavedIce_Define.UserType.CPU_5:
			name = "_screen_cp5";
			break;
		}
		if (userType >= ShavedIce_Define.UserType.CPU_1 && SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			userUIData.userIcon.gameObject.SetActive(value: false);
		}
		else
		{
			userUIData.userIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, name);
		}
		string name2 = "";
		switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType])
		{
		case 0:
			name2 = "character_yuto_02";
			break;
		case 1:
			name2 = "character_hina_02";
			break;
		case 2:
			name2 = "character_ituki_02";
			break;
		case 3:
			name2 = "character_souta_02";
			break;
		case 4:
			name2 = "character_takumi_02";
			break;
		case 5:
			name2 = "character_rin_02";
			break;
		case 6:
			name2 = "character_akira_02";
			break;
		case 7:
			name2 = "character_rui_02";
			break;
		}
		userUIData.singleCharacterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, name2);
		userUIData.teamCharacterIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, name2);
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			userUIData.singleUIAnchor.SetActive(value: true);
			userUIData.teamUIAnchor.SetActive(value: false);
		}
		else
		{
			userUIData.singleUIAnchor.SetActive(value: false);
			userUIData.teamUIAnchor.SetActive(value: true);
			userUIData.teamUnderlay.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, (teamType == ShavedIce_Define.TeamType.TEAM_A) ? "team_a_underlay" : "team_b_underlay");
			userUIData.teamName.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, (teamType == ShavedIce_Define.TeamType.TEAM_A) ? "_screen_team_A" : "_screen_team_B");
		}
		for (int i = 0; i < controlInfomationUI.infomationSpriteUI.Length; i++)
		{
			controlInfomationUI.infomationSpriteUI[i].SetAlpha(0f);
			controlInfomationUI.infomationSpriteUI[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < controlInfomationUI.infomationTextUI.Length; j++)
		{
			controlInfomationUI.infomationTextUI[j].SetAlpha(0f);
			controlInfomationUI.infomationTextUI[j].gameObject.SetActive(value: false);
		}
		userUIData.partitionFade.SetAlpha(0f);
		towerHeightCalcUI.towerHeightCalcLineSp_Black.transform.SetLocalScaleX(0f);
		towerHeightCalcUI.towerHeightCalcLineSp_White.transform.SetLocalScaleX(0f);
		towerHeightCalcUI.towerHeightCalcLineNumbersAnchor.localScale = Vector3.zero;
		towerHeightCalcUI.towerHeightCalcLine_Numbers.Set(0);
		towerHeightCalcUI.towerHeightCalcLine_DecimalNumbers.Set(0);
		towerHeightCalcUI.towerHeightRecordAnchor.localScale = Vector3.zero;
	}
	public void FadeProcess_ControlInfomationUI(bool _fadeIn)
	{
		if (_fadeIn)
		{
			StartCoroutine(FadeProcess(controlInfomationUI.infomationSpriteUI, 1f, 0.5f));
			StartCoroutine(FadeProcess(controlInfomationUI.infomationTextUI, 1f, 0.5f));
		}
		else
		{
			StartCoroutine(FadeProcess(controlInfomationUI.infomationSpriteUI, 0f, 0.5f));
			StartCoroutine(FadeProcess(controlInfomationUI.infomationTextUI, 0f, 0.5f));
		}
	}
	public void SetTowerHeightCalcLinePos(Vector3 _worldPos, bool _isGroup1)
	{
		CalcManager.mCalcVector3.x = ShavedIce_Define.PM.GetUserCamera(userType, _isGroup1).WorldToScreenPoint(_worldPos).x;
		CalcManager.mCalcVector3.y = ShavedIce_Define.PM.GetUserCamera(userType, _isGroup1).WorldToScreenPoint(_worldPos).y;
		CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
		towerHeightCalcUI.towerHeightCalcLineAnchor.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, 0f);
		towerHeightCalcUI.towerHeightCalcLineAnchor.SetLocalPositionZ(0f);
	}
	public void SetHeightData(float _height)
	{
		string[] array = (_height.ToString().Split('.').Length != 1) ? _height.ToString().Split('.') : new string[2]
		{
			_height.ToString(),
			"0"
		};
		towerHeightCalcUI.towerHeightCalcLine_Numbers.SetNumbers(array[0]);
		towerHeightCalcUI.towerHeightCalcLine_DecimalNumbers.SetNumbers(array[1]);
		towerHeightCalcUI.towerHeightRecord_Numbers.SetNumbers(array[0]);
		towerHeightCalcUI.towerHeightRecord_DecimalNumbers.SetNumbers(array[1]);
	}
	public void ShowTowerHeightCalcLine()
	{
		LeanTween.scaleX(towerHeightCalcUI.towerHeightCalcLineSp_Black.gameObject, 1f, 0.5f);
		LeanTween.scaleX(towerHeightCalcUI.towerHeightCalcLineSp_White.gameObject, 1f, 0.5f);
		LeanTween.scale(towerHeightCalcUI.towerHeightCalcLineNumbersAnchor.gameObject, Vector3.one, 0.5f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_result_slide");
	}
	public void ShowTowerHeightCalcRecord()
	{
		LeanTween.scale(towerHeightCalcUI.towerHeightRecordAnchor.gameObject, towerHeightCalcRecordAnchor_DefScale, 0.5f).setEaseOutQuart();
		SingletonCustom<AudioManager>.Instance.SePlay("se_roll_finish");
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
			_spriteArray[i].gameObject.SetActive(value: true);
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
			if (_setAlpha == 0f)
			{
				_spriteArray[k].gameObject.SetActive(value: false);
			}
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
			_textArray[i].gameObject.SetActive(value: true);
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
			if (_setAlpha == 0f)
			{
				_textArray[k].gameObject.SetActive(value: false);
			}
		}
		_callback?.Invoke();
	}
	public void PartitionFadeIn(Action _callback = null)
	{
		StartCoroutine(FadeProcess(userUIData.partitionFade, 0.7f, 0.5f, 0f, _callback));
	}
	public void PartitionFadeOut(Action _callback = null)
	{
		FadeProcess(userUIData.partitionFade, 0f, 0.5f, 0f, _callback);
	}
}
