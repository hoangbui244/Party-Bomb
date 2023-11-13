using System;
using TMPro;
using UnityEngine;
public class Skijump_UIManager : SingletonCustom<Skijump_UIManager>
{
	[Serializable]
	public struct ScoreData
	{
		public Skijump_Define.UserType userType;
		public float firstJumpDistance;
		public float secondJumpDistance;
		public int totalScore;
		public void Init(Skijump_Define.UserType _userType)
		{
			userType = _userType;
			firstJumpDistance = 0f;
			secondJumpDistance = 0f;
			totalScore = 0;
		}
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
	[Header("時間表示UI")]
	private CommonGameTimeUI_Font_Time timeUI;
	[SerializeField]
	[Header("飛んだカウント")]
	private TextMeshPro jumpCountUI;
	[SerializeField]
	[Header("「～の番です」のカットインUI")]
	private TurnCutIn turnCutIn;
	[SerializeField]
	[Header("ジャンプ操作UI")]
	private Skijump_TimingGauge jump;
	[SerializeField]
	[Header("バランス操作UI")]
	private Skijump_BalanceController balance;
	[SerializeField]
	[Header("着地操作UI")]
	private Skijump_TimingGauge landing;
	[SerializeField]
	[Header("風速")]
	private Skijump_WindManager windManager;
	[SerializeField]
	[Header("開始時操作表示UI")]
	private ControlInfomationUI firstCtrlUI;
	[SerializeField]
	[Header("ジャンプ結果スコア表示")]
	private Skijump_JumpScore jumpScoreUI;
	[SerializeField]
	[Header("ジャンプ制限時間")]
	private Skijump_TimeLimit timeLimitUI;
	[SerializeField]
	[Header("滑走開始案内表示")]
	private GameObject objSlideStartInfo;
	[SerializeField]
	[Header("スコア表示")]
	private SpriteNumbers[] scoreSpriteNumbers;
	[SerializeField]
	[Header("キャラ表示")]
	private SpriteRenderer[] characterIconSprites;
	[SerializeField]
	[Header("プレイヤ\u30fc番号表示")]
	private SpriteRenderer[] playerIconSprites;
	[SerializeField]
	[Header("プレイヤ\u30fcスコアフレ\u30fcム")]
	private SpriteRenderer[] arrayPlayerScoreFrame;
	[SerializeField]
	[Header("フェ\u30fcド用カメラ前フレ\u30fcム")]
	private CameraPrevFrame cameraPrevFrame;
	[SerializeField]
	[Header("前フレ\u30fcム表示フェ\u30fcド")]
	private MeshRenderer prevFrameMesh;
	[SerializeField]
	[Header("スキップUI")]
	private ControllerBalloonUI skip;
	private static readonly string[] BATTLE_PLAYER_SPRITE_NAMES = new string[9]
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
	private readonly Color32 FOCUS_FRAME_COLOR = new Color32(249, 59, 15, byte.MaxValue);
	private ScoreData[] userScoreData;
	[SerializeField]
	[Header("画面フェ\u30fcド画像")]
	private SpriteRenderer screenFade;
	private readonly float SCREEN_FADE_TIME = 0.25f;
	public bool IsShowSkip => skip.gameObject.activeSelf;
	public bool IsPrevFrameFade
	{
		get;
		set;
	}
	public Skijump_TimingGauge Jump => jump;
	public Skijump_BalanceController Balance => balance;
	public Skijump_TimingGauge Landing => landing;
	public Skijump_WindManager WindManager => windManager;
	public void Init()
	{
		userScoreData = new ScoreData[Skijump_Define.MEMBER_NUM];
		for (int i = 0; i < userScoreData.Length; i++)
		{
			userScoreData[i].Init(Skijump_Define.MCM.MemberOrder[i]);
		}
		windManager.Init(0.1f * (float)UnityEngine.Random.Range(0, 6));
		jump.Init();
		balance.Init();
		landing.Init();
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			playerIconSprites[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, (Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? "_screen_you" : "en_screen_you");
			playerIconSprites[1].gameObject.SetActive(value: false);
			playerIconSprites[2].gameObject.SetActive(value: false);
			playerIconSprites[3].gameObject.SetActive(value: false);
		}
		else
		{
			for (int j = 0; j < playerIconSprites.Length; j++)
			{
				if (j < SingletonCustom<GameSettingManager>.Instance.PlayerNum)
				{
					playerIconSprites[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, BATTLE_PLAYER_SPRITE_NAMES[j]);
				}
				else
				{
					playerIconSprites[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, BATTLE_PLAYER_SPRITE_NAMES[4 + (j - SingletonCustom<GameSettingManager>.Instance.PlayerNum)]);
				}
			}
		}
		for (int k = 0; k < characterIconSprites.Length; k++)
		{
			int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(k < SingletonCustom<GameSettingManager>.Instance.PlayerNum) ? k : (4 + (k - SingletonCustom<GameSettingManager>.Instance.PlayerNum))];
			characterIconSprites[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARA_DEFAULT_SPRITE_NAMES[num]);
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1 && k > 0)
			{
				characterIconSprites[k].transform.SetLocalPositionX(17f);
			}
		}
		for (int l = 0; l < scoreSpriteNumbers.Length; l++)
		{
			scoreSpriteNumbers[l].Set(0);
		}
		skip.Init();
	}
	public void ShowSkip()
	{
		skip.FadeProcess_ControlInfomationUI(_fadeIn: true);
	}
	public void CloseSkip()
	{
		skip.FadeProcess_ControlInfomationUI(_fadeIn: false);
	}
	public void SetFocusPlayer(int _idx)
	{
		for (int i = 0; i < arrayPlayerScoreFrame.Length; i++)
		{
			if (i == _idx)
			{
				arrayPlayerScoreFrame[i].color = FOCUS_FRAME_COLOR;
			}
			else
			{
				arrayPlayerScoreFrame[i].color = Color.white;
			}
		}
	}
	public void FadePrevFrame(float _time, float _delay, float _saveFrameDelay, Action _callBack)
	{
		LeanTween.delayedCall(base.gameObject, _saveFrameDelay, (Action)delegate
		{
			cameraPrevFrame.SaveFrameBuffer();
			prevFrameMesh.material.SetFloat("_Alpha", 1f);
			prevFrameMesh.gameObject.SetActive(value: true);
			IsPrevFrameFade = true;
		});
		LeanTween.value(base.gameObject, 1f, 0f, _time).setDelay(_delay).setOnUpdate(delegate(float _value)
		{
			prevFrameMesh.material.SetFloat("_Alpha", _value);
		})
			.setOnStart(delegate
			{
				_callBack?.Invoke();
			})
			.setOnComplete((Action)delegate
			{
				IsPrevFrameFade = false;
				prevFrameMesh.gameObject.SetActive(value: false);
			});
	}
	public void ShowSlideStartInfo()
	{
		objSlideStartInfo.SetActive(value: true);
		LeanTween.cancel(objSlideStartInfo);
		objSlideStartInfo.transform.SetLocalScale(0f, 0f, 1f);
		LeanTween.scale(objSlideStartInfo, Vector3.one, 0.25f).setEaseOutQuart();
	}
	public void HideSlideStartInfo()
	{
		LeanTween.cancel(objSlideStartInfo);
		objSlideStartInfo.transform.SetLocalScale(0f, 0f, 1f);
		LeanTween.scale(objSlideStartInfo, Vector3.zero, 0.25f).setEaseOutQuart().setOnComplete((Action)delegate
		{
			objSlideStartInfo.SetActive(value: false);
		});
	}
	public void SetJumpCountText(int _count)
	{
		switch (Localize_Define.Language)
		{
		case Localize_Define.LanguageType.Japanese:
			jumpCountUI.SetText((_count + 1).ToString() + SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.ATTACK_BALL, 0));
			break;
		case Localize_Define.LanguageType.English:
			jumpCountUI.SetText(SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.ATTACK_BALL, 0) + (_count + 1).ToString());
			break;
		}
	}
	public void ShowJumpScore(Skijump_Define.UserType _userType, int _jumpCut, int _distanceScore, int _balanceScore)
	{
		jumpScoreUI.Show(_distanceScore, _balanceScore);
		if (_jumpCut == 0)
		{
			userScoreData[(int)_userType].firstJumpDistance = _distanceScore + _balanceScore;
		}
		else
		{
			userScoreData[(int)_userType].secondJumpDistance = _distanceScore + _balanceScore;
		}
		LeanTween.value(userScoreData[(int)_userType].totalScore, (int)(userScoreData[(int)_userType].firstJumpDistance + userScoreData[(int)_userType].secondJumpDistance), 0.25f).setOnUpdate(delegate(float _value)
		{
			userScoreData[(int)_userType].totalScore = (int)_value;
		}).setDelay(2.3f);
	}
	public void CloseJumpScore()
	{
		jumpScoreUI.Close();
	}
	public void ShowTimeLimit()
	{
		timeLimitUI.Show();
	}
	public void SetTimeLimit(float _timeLimit)
	{
		timeLimitUI.SetTimeLimit(_timeLimit);
	}
	public void CloseTimeLimit()
	{
		timeLimitUI.Close();
	}
	public void UpdateMethod()
	{
		if (!Skijump_Define.MGM.IsDuringGame())
		{
			return;
		}
		jump.UpdateMethod();
		balance.UpdateMethod();
		landing.UpdateMethod();
		windManager.UpdateMethod();
		for (int i = 0; i < scoreSpriteNumbers.Length; i++)
		{
			if (scoreSpriteNumbers[i].CurrentValue < userScoreData[i].totalScore)
			{
				scoreSpriteNumbers[i].Set(userScoreData[i].totalScore);
			}
		}
	}
	public void PlayTurnCutIn(Skijump_Define.UserType _userType)
	{
		turnCutIn.ShowTurnCutIn((int)_userType, 0f);
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(objSlideStartInfo);
	}
	public void SetScore(Skijump_Define.UserType _userType, int _jumpCut, float _jumpDis)
	{
		for (int i = 0; i < userScoreData.Length; i++)
		{
			if (userScoreData[i].userType == _userType)
			{
				if ((float)_jumpCut == Skijump_Define.MAX_JUMP_NUM - 1f)
				{
					userScoreData[i].secondJumpDistance = _jumpDis;
				}
				else
				{
					userScoreData[i].firstJumpDistance = _jumpDis;
				}
			}
		}
	}
	public void SetDebugRecord()
	{
		int[] allUserNo = Skijump_Define.MCM.GetAllUserNo();
		for (int i = 0; i < allUserNo.Length; i++)
		{
			for (int j = 0; j < userScoreData.Length; j++)
			{
				if (userScoreData[j].userType == (Skijump_Define.UserType)allUserNo[i])
				{
					userScoreData[j].firstJumpDistance = UnityEngine.Random.Range(50f, 100f);
					userScoreData[j].secondJumpDistance = UnityEngine.Random.Range(50f, 100f);
				}
			}
		}
	}
	public ScoreData GetScoreData(Skijump_Define.UserType _userType)
	{
		for (int i = 0; i < userScoreData.Length; i++)
		{
			if (userScoreData[i].userType == _userType)
			{
				return userScoreData[i];
			}
		}
		return default(ScoreData);
	}
	public float GetTotalScore(Skijump_Define.UserType _userType)
	{
		for (int i = 0; i < userScoreData.Length; i++)
		{
			if (userScoreData[i].userType == _userType)
			{
				return userScoreData[i].totalScore;
			}
		}
		return 0f;
	}
	public void StartScreenFade(Action _fadeInCallBack = null, Action _fadeOutCallBack = null, float _delayedFadeOutTime = 0f)
	{
		Fade(isView: true, SCREEN_FADE_TIME);
		LeanTween.delayedCall(base.gameObject, SCREEN_FADE_TIME, (Action)delegate
		{
			if (_fadeInCallBack != null)
			{
				_fadeInCallBack();
			}
			LeanTween.delayedCall(base.gameObject, _delayedFadeOutTime, (Action)delegate
			{
				Fade(isView: false, SCREEN_FADE_TIME);
				LeanTween.delayedCall(base.gameObject, SCREEN_FADE_TIME, (Action)delegate
				{
					if (_fadeOutCallBack != null)
					{
						_fadeOutCallBack();
					}
				});
			});
		});
	}
	private void Fade(bool isView, float _fadeTime)
	{
		Color alpha;
		LeanTween.value(screenFade.gameObject, isView ? 0f : 1f, isView ? 1f : 0f, _fadeTime).setOnUpdate(delegate(float val)
		{
			alpha = screenFade.color;
			alpha.a = val;
			screenFade.color = alpha;
		});
	}
}
