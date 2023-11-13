using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class MakingPotion_UiManager : SingletonCustom<MakingPotion_UiManager>
{
	[Serializable]
	private class SugarUI
	{
		public GameObject anchor;
		public GameObject oneUnderlay;
		public GameObject twoUnderlay;
		public GameObject threeUnderlay;
		public GameObject redObj;
		public GameObject yellowObj;
		public GameObject greenObj;
		public GameObject blueObj;
		public GameObject redObj_Two;
		public GameObject yellowObj_Two;
		public GameObject greenObj_Two;
		public GameObject blueObj_Two;
		public GameObject redObj_Three;
		public GameObject yellowObj_Three;
		public GameObject greenObj_Three;
		public GameObject blueObj_Three;
	}
	[Serializable]
	private class PotionUI
	{
		public PotionData[] datas;
	}
	[Serializable]
	private class PotionData
	{
		public GameObject anchor;
		public SpriteRenderer potionSprite;
		public SpriteNumbers numbers;
		public GameObject selectObj;
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
	private const float LAST_POTION_SCORE_MOVE_OUT_TIME = 10f;
	private static readonly string[] PLAYER_SPRITE_NAMES = new string[9]
	{
		"_screen_1p",
		"_screen_2p",
		"_screen_3p",
		"_screen_4p",
		"_screen_cp1",
		"_screen_cp2",
		"_screen_cp3",
		"_screen_cp4",
		"_screen_cp5"
	};
	private static readonly string[] TEAM_SPRITE_NAMES = new string[2]
	{
		"_screen_team_A",
		"_screen_team_B"
	};
	private static readonly string[] CHARACTER_SPRITE_NAMES = new string[8]
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
	private static readonly string[] POTION_SPRITE_NAMES = new string[4]
	{
		"potion_01",
		"potion_03",
		"potion_00",
		"potion_02"
	};
	private const string GROUP_SECOND_NAME = "_play_2group";
	private static readonly Color POINT_NORMAL_COLOR = Color.white;
	private static readonly Color POINT_RARE_COLOR = Color.yellow;
	private const float POINT_SINGLE_PLAYER_SCALE = 1f;
	private const float POINT_SINGLE_CPU_SCALE = 0.5f;
	private const float POINT_MULTI_SCALE = 0.6666f;
	[SerializeField]
	private Color[] pointColors;
	[SerializeField]
	private Color potionPointGreenColor;
	[SerializeField]
	[Header("共通UI 1人用")]
	private GameObject commonSingleObj;
	[SerializeField]
	private CommonGameTimeUI_Font_Time commonSingleGameTimeUI;
	[SerializeField]
	private SpriteNumbers[] commonSingleSpriteNumbers;
	[SerializeField]
	private SpriteRenderer[] commonSingleCharacterIcons;
	[SerializeField]
	private MeshRenderer[] singleTargetViewRenderers;
	[SerializeField]
	private GameObject[] singleRightArrowObjs;
	[SerializeField]
	private GameObject[] singleLeftArrowObjs;
	[SerializeField]
	private SugarUI[] singleSugarUIs;
	[SerializeField]
	private PotionUI singlePotionUI;
	[SerializeField]
	private SpriteRenderer singleCreateNumSprite;
	[SerializeField]
	private Transform[] singleScoreAnchors;
	[SerializeField]
	[Header("共通UI 複数人用")]
	private GameObject commonMultiObj;
	[SerializeField]
	private CommonGameTimeUI_Font_Time commonMultiGameTimeUI;
	[SerializeField]
	private SpriteNumbers[] commonMultiSpriteNumbers;
	[SerializeField]
	private SpriteRenderer[] commonMultiCharacterIcons;
	[SerializeField]
	private SpriteRenderer[] commonMultiPlayerIcons;
	[SerializeField]
	private SpriteRenderer[] commonMultiTeamIcons;
	[SerializeField]
	private SpriteRenderer commonMultiGroupNumberIcon;
	[SerializeField]
	private MeshRenderer[] multiTargetViewRenderers;
	[SerializeField]
	private Transform multiTargetViewAnchor;
	[SerializeField]
	private Transform multiTargetUnderlayAnchor;
	[SerializeField]
	private GameObject[] multiRightArrowObjs;
	[SerializeField]
	private GameObject[] multiLeftArrowObjs;
	[SerializeField]
	private SugarUI[] multiSugarUIs;
	[SerializeField]
	private PotionUI[] multiPotionUIs;
	[SerializeField]
	private SpriteRenderer multiCreateNumSprite;
	[SerializeField]
	private Transform[] multiScoreAnchors;
	[SerializeField]
	private GameObject[] multiCtrlObjs;
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer fade;
	[SerializeField]
	private GameObject singleOnlyObj;
	[SerializeField]
	private GameObject multiOnlyObj;
	[SerializeField]
	[Header("開始時操作表示UI")]
	private ControlInfomationUI[] firstCtrlUIs;
	[SerializeField]
	private Transform[] ctrlUIAnchors_Four;
	[SerializeField]
	private Material[] targetRenderMaterials;
	[SerializeField]
	private Transform getPointUiAnchor;
	[SerializeField]
	private MakingPotion_GetPointUi getPointUiPrefab;
	private List<MakingPotion_GetPointUi> getPointUiList = new List<MakingPotion_GetPointUi>();
	private bool isSingle;
	private bool isScoreMoveOut;
	[SerializeField]
	[Header("ゲ\u30fcム開始のテキストUI")]
	private SpriteRenderer announceText;
	private Vector3 originAnnounceTextScale;
	public void Init()
	{
		if (SingletonCustom<MakingPotion_GameManager>.Instance.PlayerNum == 1)
		{
			isSingle = true;
			singleOnlyObj.SetActive(value: true);
			multiOnlyObj.SetActive(value: false);
			commonSingleObj.SetActive(value: true);
			commonMultiObj.SetActive(value: false);
		}
		else
		{
			isSingle = false;
			singleOnlyObj.SetActive(value: false);
			multiOnlyObj.SetActive(value: true);
			commonSingleObj.SetActive(value: false);
			commonMultiObj.SetActive(value: true);
			commonMultiGroupNumberIcon.gameObject.SetActive(SingletonCustom<MakingPotion_GameManager>.Instance.HasSecondGroup);
			for (int i = 0; i < ctrlUIAnchors_Four.Length; i++)
			{
				firstCtrlUIs[i].anchor.transform.position = ctrlUIAnchors_Four[i].position;
				firstCtrlUIs[i].anchor.transform.localScale = ctrlUIAnchors_Four[i].localScale;
			}
			for (int j = 0; j < multiCtrlObjs.Length; j++)
			{
				multiCtrlObjs[j].SetActive(j < SingletonCustom<MakingPotion_GameManager>.Instance.PlayerNum);
			}
		}
		announceText.gameObject.SetActive(value: true);
		originAnnounceTextScale = announceText.transform.localScale;
		DataInit();
	}
	public void SecondGroupInit()
	{
		commonMultiGroupNumberIcon.gameObject.SetActive(SingletonCustom<MakingPotion_GameManager>.Instance.HasSecondGroup);
		commonMultiGroupNumberIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_play_2group");
		DataInit();
	}
	private void DataInit()
	{
		ScoreUpdate();
		SetTime(15f);
		if (isSingle)
		{
			for (int i = 0; i < commonSingleCharacterIcons.Length; i++)
			{
				MakingPotion_PlayerScript player = SingletonCustom<MakingPotion_PlayerManager>.Instance.GetPlayer(i);
				int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[player.PlayerNo];
				commonSingleCharacterIcons[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARACTER_SPRITE_NAMES[num]);
			}
		}
		else
		{
			for (int j = 0; j < commonMultiCharacterIcons.Length; j++)
			{
				MakingPotion_PlayerScript player2 = SingletonCustom<MakingPotion_PlayerManager>.Instance.GetPlayer(j);
				int num2 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[player2.PlayerNo];
				commonMultiCharacterIcons[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARACTER_SPRITE_NAMES[num2]);
				commonMultiPlayerIcons[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[player2.PlayerNo]);
				if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat != 0)
				{
					commonMultiTeamIcons[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, TEAM_SPRITE_NAMES[player2.TeamNo]);
					commonMultiTeamIcons[j].gameObject.SetActive(value: true);
				}
				else
				{
					commonMultiTeamIcons[j].gameObject.SetActive(value: false);
				}
			}
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
			{
				multiTargetViewAnchor.SetLocalPositionY(-20f);
				multiTargetUnderlayAnchor.SetLocalPositionY(-20f);
			}
			else
			{
				multiTargetViewAnchor.SetLocalPositionY(45f);
				multiTargetUnderlayAnchor.SetLocalPositionY(45f);
			}
		}
		for (int k = 0; k < firstCtrlUIs.Length; k++)
		{
			firstCtrlUIs[k].SetActive(_active: false);
			firstCtrlUIs[k].SetAlpha(0f);
		}
		for (int l = 0; l < 5; l++)
		{
			MakingPotion_GetPointUi makingPotion_GetPointUi = UnityEngine.Object.Instantiate(getPointUiPrefab, getPointUiAnchor);
			getPointUiList.Add(makingPotion_GetPointUi);
			makingPotion_GetPointUi.Init();
		}
		HideArrow();
		for (int m = 0; m < 4; m++)
		{
			HideSugar(m);
		}
	}
	public void UpdateMethod()
	{
	}
	public void TimeUpdate()
	{
		float remainViewTime = SingletonCustom<MakingPotion_GameManager>.Instance.RemainViewTime;
		if (isSingle)
		{
			commonSingleGameTimeUI.SetTime(remainViewTime);
		}
		else
		{
			commonMultiGameTimeUI.SetTime(remainViewTime);
		}
	}
	public void SetTime(float _time)
	{
		if (isSingle)
		{
			commonSingleGameTimeUI.SetTime(_time);
		}
		else
		{
			commonMultiGameTimeUI.SetTime(_time);
		}
		if (!isScoreMoveOut && 5 == SingletonCustom<MakingPotion_PlayerManager>.Instance.CreateCount + 1 && _time < 10f)
		{
			ScoreMoveOut();
		}
	}
	public void ScoreUpdate()
	{
		int[] scores = SingletonCustom<MakingPotion_GameManager>.Instance.GetScores();
		if (isSingle)
		{
			for (int i = 0; i < commonSingleSpriteNumbers.Length; i++)
			{
				commonSingleSpriteNumbers[i].Set(scores[i]);
			}
		}
		else
		{
			for (int j = 0; j < commonMultiSpriteNumbers.Length; j++)
			{
				commonMultiSpriteNumbers[j].Set(scores[j]);
			}
		}
	}
	public void ScoreUpdate(int _no)
	{
		int score = SingletonCustom<MakingPotion_GameManager>.Instance.GetScore(_no);
		if (isSingle)
		{
			commonSingleSpriteNumbers[_no].Set(score);
		}
		else
		{
			commonMultiSpriteNumbers[_no].Set(score);
		}
	}
	public void SetPotionSprite(int _no, int _order, MakingPotion_PlayerScript.SugarColorType _colorType)
	{
		if (_colorType == MakingPotion_PlayerScript.SugarColorType.White)
		{
			_colorType = MakingPotion_PlayerScript.SugarColorType.Red;
		}
		if (isSingle)
		{
			if (_no == 0)
			{
				singlePotionUI.datas[_order].potionSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.MakingPotion, POTION_SPRITE_NAMES[(int)(_colorType - 1)]);
			}
		}
		else
		{
			multiPotionUIs[_no].datas[_order].potionSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.MakingPotion, POTION_SPRITE_NAMES[(int)(_colorType - 1)]);
		}
	}
	public void SetPotionPoint(int _no, int _order, int _point)
	{
		if (isSingle)
		{
			if (_no == 0)
			{
				singlePotionUI.datas[_order].numbers.gameObject.SetActive(value: true);
				singlePotionUI.datas[_order].numbers.Set(_point);
				if (_point >= 1000)
				{
					singlePotionUI.datas[_order].numbers.transform.localScale = singlePotionUI.datas[_order].numbers.transform.localScale * 0.7f;
				}
				else if (_point < 10)
				{
					singlePotionUI.datas[_order].numbers.transform.SetLocalPositionX(0f);
				}
				else if (_point < 100)
				{
					singlePotionUI.datas[_order].numbers.transform.SetLocalPositionX(13.8f);
				}
			}
		}
		else
		{
			multiPotionUIs[_no].datas[_order].numbers.gameObject.SetActive(value: true);
			multiPotionUIs[_no].datas[_order].numbers.Set(_point);
			if (_point >= 1000)
			{
				multiPotionUIs[_no].datas[_order].numbers.transform.localScale = multiPotionUIs[_no].datas[_order].numbers.transform.localScale * 0.7f;
			}
			else if (_point < 10)
			{
				multiPotionUIs[_no].datas[_order].numbers.transform.SetLocalPositionX(0f);
			}
			else if (_point < 100)
			{
				multiPotionUIs[_no].datas[_order].numbers.transform.SetLocalPositionX(13f);
			}
		}
	}
	public void SetPotionOrder(int _no, int _order)
	{
		if (isSingle)
		{
			if (_no == 0)
			{
				for (int i = 0; i < singlePotionUI.datas.Length; i++)
				{
					singlePotionUI.datas[i].selectObj.SetActive(i == _order);
				}
			}
		}
		else
		{
			for (int j = 0; j < multiPotionUIs[_no].datas.Length; j++)
			{
				multiPotionUIs[_no].datas[j].selectObj.SetActive(j == _order);
			}
		}
	}
	public void ViewTarget(int _charaNo, int _targetNo)
	{
		if (isSingle)
		{
			singleTargetViewRenderers[_charaNo].sharedMaterial = targetRenderMaterials[_targetNo % targetRenderMaterials.Length];
		}
		else
		{
			multiTargetViewRenderers[_charaNo].sharedMaterial = targetRenderMaterials[_targetNo % targetRenderMaterials.Length];
		}
	}
	public void ViewGetPoint(int _charaNo, int _point, Vector3 _worldPos)
	{
		Color pointColor = GetPointColor(_charaNo, _isPotionUI: false);
		float scale = 0.6666f;
		if (SingletonCustom<MakingPotion_GameManager>.Instance.PlayerNum == 1)
		{
			scale = ((!SingletonCustom<MakingPotion_PlayerManager>.Instance.GetIsPlayer(_charaNo)) ? 0.5f : 1f);
		}
		bool flag = false;
		for (int i = 0; i < getPointUiList.Count; i++)
		{
			if (!getPointUiList[i].IsShow)
			{
				getPointUiList[i].SetColor(pointColor);
				getPointUiList[i].SetScale(scale);
				getPointUiList[i].Show(_charaNo, _point, _worldPos);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			MakingPotion_GetPointUi makingPotion_GetPointUi = UnityEngine.Object.Instantiate(getPointUiPrefab, getPointUiAnchor);
			getPointUiList.Add(makingPotion_GetPointUi);
			makingPotion_GetPointUi.SetColor(pointColor);
			makingPotion_GetPointUi.SetScale(scale);
			makingPotion_GetPointUi.Show(_charaNo, _point, _worldPos);
		}
	}
	public void ViewCheck(int _charaNo, Vector3 _worldPos, MakingPotion_PlayerScript.SpinSpeedType _speedType)
	{
		float scale = 0.6666f;
		if (SingletonCustom<MakingPotion_GameManager>.Instance.PlayerNum == 1)
		{
			scale = ((!SingletonCustom<MakingPotion_PlayerManager>.Instance.GetIsPlayer(_charaNo)) ? 0.5f : 1f);
		}
		bool flag = false;
		for (int i = 0; i < getPointUiList.Count; i++)
		{
			if (!getPointUiList[i].IsShow)
			{
				getPointUiList[i].SetScale(scale);
				getPointUiList[i].PaceShow(_charaNo, _speedType, _worldPos);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			MakingPotion_GetPointUi makingPotion_GetPointUi = UnityEngine.Object.Instantiate(getPointUiPrefab, getPointUiAnchor);
			getPointUiList.Add(makingPotion_GetPointUi);
			makingPotion_GetPointUi.SetScale(scale);
			makingPotion_GetPointUi.PaceShow(_charaNo, _speedType, _worldPos);
		}
	}
	private Color GetPointColor(int _charaNo, bool _isPotionUI)
	{
		int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[SingletonCustom<MakingPotion_PlayerManager>.Instance.GetPlayerNo(_charaNo)];
		if (_isPotionUI && num == 0)
		{
			return potionPointGreenColor;
		}
		return pointColors[num];
	}
	public void ViewFirstControlInfo()
	{
		for (int i = 0; i < firstCtrlUIs.Length; i++)
		{
			if (SingletonCustom<MakingPotion_PlayerManager>.Instance.GetIsPlayer(i))
			{
				firstCtrlUIs[i].SetActive(_active: true);
				ControlInfomationFade(firstCtrlUIs[i], 1f, 0.5f, 1f);
				int idx = i;
				ControlInfomationFade(firstCtrlUIs[i], 0f, 0.5f, 5f, delegate
				{
					firstCtrlUIs[idx].SetActive(_active: false);
				});
			}
		}
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
	public void ShowArrow(bool _isRight)
	{
		if (isSingle)
		{
			for (int i = 0; i < singleRightArrowObjs.Length; i++)
			{
				singleRightArrowObjs[i].SetActive(_isRight);
				singleLeftArrowObjs[i].SetActive(!_isRight);
			}
		}
		else
		{
			for (int j = 0; j < multiRightArrowObjs.Length; j++)
			{
				multiRightArrowObjs[j].SetActive(_isRight);
				multiLeftArrowObjs[j].SetActive(!_isRight);
			}
		}
		LeanTween.delayedCall(base.gameObject, 3f, (Action)delegate
		{
			HideArrow();
		});
	}
	public void HideArrow()
	{
		if (isSingle)
		{
			for (int i = 0; i < singleRightArrowObjs.Length; i++)
			{
				singleRightArrowObjs[i].SetActive(value: false);
				singleLeftArrowObjs[i].SetActive(value: false);
			}
		}
		else
		{
			for (int j = 0; j < multiRightArrowObjs.Length; j++)
			{
				multiRightArrowObjs[j].SetActive(value: false);
				multiLeftArrowObjs[j].SetActive(value: false);
			}
		}
	}
	public void ShowSugar(MakingPotion_PlayerScript.SugarColorType _sugarType, MakingPotion_PlayerScript.SugarColorType _sugarTypeTwo = MakingPotion_PlayerScript.SugarColorType.White, MakingPotion_PlayerScript.SugarColorType _sugarTypeThree = MakingPotion_PlayerScript.SugarColorType.White)
	{
		if (isSingle)
		{
			for (int i = 0; i < singleSugarUIs.Length; i++)
			{
				singleSugarUIs[i].anchor.SetActive(value: true);
				singleSugarUIs[i].oneUnderlay.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.White);
				singleSugarUIs[i].twoUnderlay.SetActive(_sugarTypeTwo != 0 && _sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.White);
				singleSugarUIs[i].threeUnderlay.SetActive(_sugarTypeThree != MakingPotion_PlayerScript.SugarColorType.White);
				singleSugarUIs[i].redObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Red);
				singleSugarUIs[i].yellowObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Yellow);
				singleSugarUIs[i].greenObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Green);
				singleSugarUIs[i].blueObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Blue);
				singleSugarUIs[i].redObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Red);
				singleSugarUIs[i].yellowObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Yellow);
				singleSugarUIs[i].greenObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Green);
				singleSugarUIs[i].blueObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Blue);
				singleSugarUIs[i].redObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Red);
				singleSugarUIs[i].yellowObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Yellow);
				singleSugarUIs[i].greenObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Green);
				singleSugarUIs[i].blueObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Blue);
			}
		}
		else
		{
			for (int j = 0; j < multiSugarUIs.Length; j++)
			{
				multiSugarUIs[j].anchor.SetActive(value: true);
				multiSugarUIs[j].oneUnderlay.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.White);
				multiSugarUIs[j].twoUnderlay.SetActive(_sugarTypeTwo != 0 && _sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.White);
				multiSugarUIs[j].threeUnderlay.SetActive(_sugarTypeThree != MakingPotion_PlayerScript.SugarColorType.White);
				multiSugarUIs[j].redObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Red);
				multiSugarUIs[j].yellowObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Yellow);
				multiSugarUIs[j].greenObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Green);
				multiSugarUIs[j].blueObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Blue);
				multiSugarUIs[j].redObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Red);
				multiSugarUIs[j].yellowObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Yellow);
				multiSugarUIs[j].greenObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Green);
				multiSugarUIs[j].blueObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Blue);
				multiSugarUIs[j].redObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Red);
				multiSugarUIs[j].yellowObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Yellow);
				multiSugarUIs[j].greenObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Green);
				multiSugarUIs[j].blueObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Blue);
			}
		}
	}
	public void ShowSugar(int _charaNo, MakingPotion_PlayerScript.SugarColorType _sugarType, MakingPotion_PlayerScript.SugarColorType _sugarTypeTwo = MakingPotion_PlayerScript.SugarColorType.White, MakingPotion_PlayerScript.SugarColorType _sugarTypeThree = MakingPotion_PlayerScript.SugarColorType.White)
	{
		if (isSingle)
		{
			singleSugarUIs[_charaNo].anchor.SetActive(value: true);
			singleSugarUIs[_charaNo].oneUnderlay.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.White);
			singleSugarUIs[_charaNo].twoUnderlay.SetActive(_sugarTypeTwo != 0 && _sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.White);
			singleSugarUIs[_charaNo].threeUnderlay.SetActive(_sugarTypeThree != MakingPotion_PlayerScript.SugarColorType.White);
			singleSugarUIs[_charaNo].redObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Red);
			singleSugarUIs[_charaNo].yellowObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Yellow);
			singleSugarUIs[_charaNo].greenObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Green);
			singleSugarUIs[_charaNo].blueObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Blue);
			singleSugarUIs[_charaNo].redObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Red);
			singleSugarUIs[_charaNo].yellowObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Yellow);
			singleSugarUIs[_charaNo].greenObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Green);
			singleSugarUIs[_charaNo].blueObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Blue);
			singleSugarUIs[_charaNo].redObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Red);
			singleSugarUIs[_charaNo].yellowObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Yellow);
			singleSugarUIs[_charaNo].greenObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Green);
			singleSugarUIs[_charaNo].blueObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Blue);
		}
		else
		{
			multiSugarUIs[_charaNo].anchor.SetActive(value: true);
			multiSugarUIs[_charaNo].oneUnderlay.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.White);
			multiSugarUIs[_charaNo].twoUnderlay.SetActive(_sugarTypeTwo != 0 && _sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.White);
			multiSugarUIs[_charaNo].threeUnderlay.SetActive(_sugarTypeThree != MakingPotion_PlayerScript.SugarColorType.White);
			multiSugarUIs[_charaNo].redObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Red);
			multiSugarUIs[_charaNo].yellowObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Yellow);
			multiSugarUIs[_charaNo].greenObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Green);
			multiSugarUIs[_charaNo].blueObj.SetActive(_sugarType == MakingPotion_PlayerScript.SugarColorType.Blue);
			multiSugarUIs[_charaNo].redObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Red);
			multiSugarUIs[_charaNo].yellowObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Yellow);
			multiSugarUIs[_charaNo].greenObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Green);
			multiSugarUIs[_charaNo].blueObj_Two.SetActive(_sugarTypeTwo == MakingPotion_PlayerScript.SugarColorType.Blue);
			multiSugarUIs[_charaNo].redObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Red);
			multiSugarUIs[_charaNo].yellowObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Yellow);
			multiSugarUIs[_charaNo].greenObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Green);
			multiSugarUIs[_charaNo].blueObj_Three.SetActive(_sugarTypeThree == MakingPotion_PlayerScript.SugarColorType.Blue);
		}
	}
	public void HideSugar(int _charaNo)
	{
		if (isSingle)
		{
			singleSugarUIs[_charaNo].anchor.SetActive(value: false);
		}
		else
		{
			multiSugarUIs[_charaNo].anchor.SetActive(value: false);
		}
	}
	public void SetCreateNum(int _num)
	{
		if (isSingle)
		{
			singleCreateNumSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + _num.ToString());
		}
		else
		{
			multiCreateNumSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_b_" + _num.ToString());
		}
	}
	public void ScoreMoveOut()
	{
		if (isScoreMoveOut)
		{
			return;
		}
		isScoreMoveOut = true;
		if (isSingle)
		{
			for (int i = 0; i < singleScoreAnchors.Length; i++)
			{
				if (i == 0)
				{
					LeanTween.moveLocalY(singleScoreAnchors[i].gameObject, singleScoreAnchors[i].transform.localPosition.y + 500f, 1.25f).setEaseInQuint().setDelay((float)i * 0.15f);
				}
				else
				{
					LeanTween.moveLocalX(singleScoreAnchors[i].gameObject, singleScoreAnchors[i].transform.localPosition.x + 500f, 1.25f).setEaseInQuint().setDelay((float)i * 0.15f);
				}
			}
		}
		else
		{
			for (int j = 0; j < multiScoreAnchors.Length; j++)
			{
				LeanTween.moveLocalY(multiScoreAnchors[j].gameObject, multiScoreAnchors[j].transform.localPosition.y + 500f, 1.25f).setEaseInQuint().setDelay((float)j * 0.15f);
			}
		}
	}
	public void EndGame()
	{
	}
	public void CloseGameUI()
	{
		base.gameObject.SetActive(value: false);
	}
	public void HideAnnounceText()
	{
		LeanTween.scale(announceText.gameObject, Vector3.zero, 0.5f).setEaseInBack();
		Color color = announceText.color;
		color.a = 0f;
		LeanTween.color(announceText.gameObject, color, 0.5f);
		LeanTween.delayedCall(announceText.gameObject, 0.5f, (Action)delegate
		{
			announceText.gameObject.SetActive(value: false);
		});
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
}
