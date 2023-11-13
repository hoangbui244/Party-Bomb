using System;
using System.Collections.Generic;
using UnityEngine;
public class WhackMoleUiManager : SingletonCustom<WhackMoleUiManager>
{
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
	private const string GROUP_SECOND_NAME = "_play_2group";
	private static readonly Color POINT_NORMAL_COLOR = Color.white;
	private static readonly Color POINT_RARE_COLOR = Color.yellow;
	private const float POINT_SINGLE_PLAYER_SCALE = 1.5f;
	private const float POINT_MULTI_SCALE = 1f;
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
	[Header("フェ\u30fcド")]
	private SpriteRenderer fade;
	[SerializeField]
	private GameObject singleOnlyObj;
	[SerializeField]
	private GameObject multiOnlyObj;
	[SerializeField]
	private Transform getPointUiAnchor;
	[SerializeField]
	private WhackMoleGetPointUi getPointUiPrefab;
	private List<WhackMoleGetPointUi> getPointUiList = new List<WhackMoleGetPointUi>();
	private bool isSingle;
	public void Init()
	{
		if (SingletonCustom<WhackMoleGameManager>.Instance.PlayerNum == 1)
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
			commonMultiGroupNumberIcon.gameObject.SetActive(SingletonCustom<WhackMoleGameManager>.Instance.HasSecondGroup);
		}
		DataInit();
	}
	public void SecondGroupInit()
	{
		commonMultiGroupNumberIcon.gameObject.SetActive(SingletonCustom<WhackMoleGameManager>.Instance.HasSecondGroup);
		commonMultiGroupNumberIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_play_2group");
		DataInit();
	}
	private void DataInit()
	{
		ScoreUpdate();
		if (isSingle)
		{
			for (int i = 0; i < commonSingleCharacterIcons.Length; i++)
			{
				WhackMoleCharacterScript chara = SingletonCustom<WhackMoleCharacterManager>.Instance.GetChara(i);
				int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[chara.PlayerNo];
				commonSingleCharacterIcons[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARACTER_SPRITE_NAMES[num]);
			}
		}
		else
		{
			for (int j = 0; j < commonMultiCharacterIcons.Length; j++)
			{
				WhackMoleCharacterScript chara2 = SingletonCustom<WhackMoleCharacterManager>.Instance.GetChara(j);
				int num2 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[chara2.PlayerNo];
				commonMultiCharacterIcons[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARACTER_SPRITE_NAMES[num2]);
				commonMultiPlayerIcons[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[chara2.PlayerNo]);
				if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat != 0)
				{
					commonMultiTeamIcons[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, TEAM_SPRITE_NAMES[chara2.TeamNo]);
					commonMultiTeamIcons[j].gameObject.SetActive(value: true);
				}
				else
				{
					commonMultiTeamIcons[j].gameObject.SetActive(value: false);
				}
			}
		}
		for (int k = 0; k < 10; k++)
		{
			WhackMoleGetPointUi whackMoleGetPointUi = UnityEngine.Object.Instantiate(getPointUiPrefab, getPointUiAnchor);
			getPointUiList.Add(whackMoleGetPointUi);
			whackMoleGetPointUi.Init();
		}
	}
	public void UpdateMethod()
	{
		TimeUpdate();
	}
	public void TimeUpdate()
	{
		float remainViewTime = SingletonCustom<WhackMoleGameManager>.Instance.RemainViewTime;
		if (isSingle)
		{
			commonSingleGameTimeUI.SetTime(remainViewTime);
		}
		else
		{
			commonMultiGameTimeUI.SetTime(remainViewTime);
		}
	}
	public void ScoreUpdate()
	{
		int[] scores = SingletonCustom<WhackMoleGameManager>.Instance.GetScores();
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
		int score = SingletonCustom<WhackMoleGameManager>.Instance.GetScore(_no);
		if (isSingle)
		{
			commonSingleSpriteNumbers[_no].Set(score);
		}
		else
		{
			commonMultiSpriteNumbers[_no].Set(score);
		}
	}
	public void ViewGetPoint(int _charaNo, int _point, Vector3 _worldPos)
	{
		Color color = POINT_NORMAL_COLOR;
		if (_point == 150)
		{
			color = POINT_RARE_COLOR;
		}
		float scale = 1f;
		if (SingletonCustom<WhackMoleGameManager>.Instance.PlayerNum == 1 && SingletonCustom<WhackMoleCharacterManager>.Instance.GetIsPlayer(_charaNo))
		{
			scale = 1.5f;
		}
		bool flag = false;
		for (int i = 0; i < getPointUiList.Count; i++)
		{
			if (!getPointUiList[i].IsShow)
			{
				getPointUiList[i].SetColor(color);
				getPointUiList[i].SetScale(scale);
				getPointUiList[i].Show(_charaNo, _point, _worldPos);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			WhackMoleGetPointUi whackMoleGetPointUi = UnityEngine.Object.Instantiate(getPointUiPrefab, getPointUiAnchor);
			getPointUiList.Add(whackMoleGetPointUi);
			whackMoleGetPointUi.SetColor(color);
			whackMoleGetPointUi.SetScale(scale);
			whackMoleGetPointUi.Show(_charaNo, _point, _worldPos);
		}
	}
	public void EndGame()
	{
	}
	public void CloseGameUI()
	{
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
