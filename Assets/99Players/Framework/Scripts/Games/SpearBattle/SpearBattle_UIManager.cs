using System;
using UnityEngine;
public class SpearBattle_UIManager : SingletonCustom<SpearBattle_UIManager>
{
	[SerializeField]
	private SpriteRenderer fade;
	[SerializeField]
	private SpearBattle_SelectUIManager selectUiManager;
	[SerializeField]
	private SpearBattle_BattleUIManager battleUiManager;
	[SerializeField]
	[Header("ト\u30fcナメント戦UI管理処理")]
	private SpearBattle_TournamentUIManager tournamentUIManager;
	[SerializeField]
	[Header("ト\u30fcナメント戦UIオブジェクト")]
	private GameObject object_Battle_Tournament_UI;
	[SerializeField]
	[Header("スキップ")]
	private ControllerBalloonUI skipButton;
	[SerializeField]
	private CommonGameTimeUI_Font_Time time;
	[SerializeField]
	private Color[] playerFrameColors;
	[SerializeField]
	[Header("ゲ\u30fcム開始のテキストUI")]
	private SpriteRenderer announceText;
	private Vector3 originAnnounceTextScale;
	[SerializeField]
	[Header("ゲ\u30fcム開始時に非表示にするUI")]
	private GameObject[] announceDisableObjs;
	private static readonly string[] UI_PLAYER_SPRITE_NAMES = new string[9]
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
	private static readonly string[] WORLD_PLAYER_SPRITE_NAMES = new string[9]
	{
		"_common_c_1P",
		"_common_c_2P",
		"_common_c_3P",
		"_common_c_4P",
		"_common_c_cp1",
		"_common_c_cp2",
		"_common_c_cp3",
		"_common_c_cp4",
		"_common_c_cp5"
	};
	private static readonly string[] CHARA_DEFAULT_SPRITE_NAMES = new string[8]
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
	public bool IsSkipShow => skipButton.gameObject.activeSelf;
	public void Init()
	{
		object_Battle_Tournament_UI.SetActive(value: true);
		tournamentUIManager.Init();
		announceText.gameObject.SetActive(value: true);
		originAnnounceTextScale = announceText.transform.localScale;
		for (int i = 0; i < announceDisableObjs.Length; i++)
		{
			announceDisableObjs[i].SetActive(value: false);
		}
		ChangeSelect();
	}
	public void UpdateMethod()
	{
		if (selectUiManager.IsView)
		{
			selectUiManager.UpdateMethod();
		}
		else if (battleUiManager.IsView)
		{
			battleUiManager.UpdateMethod();
		}
	}
	public void ChangeSelect()
	{
		selectUiManager.Init();
		skipButton.Init();
	}
	public void ChangeBattle()
	{
		battleUiManager.Init();
		skipButton.Init();
	}
	public void SetSelectUIActive(bool _active)
	{
		selectUiManager.SetViewActive(_active);
	}
	public void ShowSkip()
	{
		skipButton.FadeProcess_ControlInfomationUI(_fadeIn: true);
	}
	public void HideSkip()
	{
		skipButton.SetControlInfomationUIActive(_isActive: false);
	}
	public void SetTimeActive(bool _active)
	{
		time.gameObject.SetActive(_active);
	}
	public void SetTimeValue(float _time)
	{
		time.SetTime(_time);
	}
	public Sprite GetNowUiPlayerIconSprite(bool _isLeft)
	{
		int num = _isLeft ? SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().leftCharaData.playerNo : SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().rightCharaData.playerNo;
		return SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, UI_PLAYER_SPRITE_NAMES[num]);
	}
	public Sprite GetNowWorldPlayerIconSprite(bool _isLeft)
	{
		int num = _isLeft ? SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().leftCharaData.playerNo : SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().rightCharaData.playerNo;
		return SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, WORLD_PLAYER_SPRITE_NAMES[num]);
	}
	public Sprite GetNowCharaIconSprite(bool _isLeft)
	{
		int num = _isLeft ? SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().leftCharaData.styleCharaNo : SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().rightCharaData.styleCharaNo;
		return SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARA_DEFAULT_SPRITE_NAMES[num]);
	}
	public Color GetNowPlayerFrameColor(bool _isLeft)
	{
		int num = _isLeft ? SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().leftCharaData.styleCharaNo : SingletonCustom<SpearBattle_GameManager>.Instance.GetNowBattleData().rightCharaData.styleCharaNo;
		return playerFrameColors[num];
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
			for (int i = 0; i < announceDisableObjs.Length; i++)
			{
				announceDisableObjs[i].SetActive(value: true);
			}
		});
	}
	public void SetTournamentTeamData()
	{
		tournamentUIManager.SetTeamData(SingletonCustom<SpearBattle_GameManager>.Instance.GetTournamentVSTeamData()[0], SpearBattle_TournamentUIManager.RoundType.Round_1st);
		tournamentUIManager.SetTeamData(SingletonCustom<SpearBattle_GameManager>.Instance.GetTournamentVSTeamData()[1], SpearBattle_TournamentUIManager.RoundType.Round_2nd);
	}
	public void StartTournamentUIAnimation(bool _winnerTeamLeft, Action _endCallBack)
	{
		tournamentUIManager.StartLineAnimation(_winnerTeamLeft, _endCallBack);
	}
	public void NextSettingTournament()
	{
		tournamentUIManager.NextSetting();
	}
	public void Fade(float _fadeTime, float _blackTime, Action _halfAction = null, Action _endAction = null)
	{
		fade.gameObject.SetActive(value: true);
		LeanTween.color(fade.gameObject, new Color(0f, 0f, 0f, 1f), _fadeTime).setOnComplete((Action)delegate
		{
			if (_halfAction != null)
			{
				_halfAction();
			}
			LeanTween.delayedCall(_blackTime, (Action)delegate
			{
				LeanTween.color(fade.gameObject, new Color(0f, 0f, 0f, 0f), _fadeTime).setOnComplete((Action)delegate
				{
					if (_endAction != null)
					{
						_endAction();
					}
					fade.gameObject.SetActive(value: false);
				});
			});
		});
	}
}
