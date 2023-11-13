using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class IceHockeyUIManager : SingletonCustom<IceHockeyUIManager>
{
	[SerializeField]
	[Header("プレイヤ\u30fc番号表示")]
	private GameObject[] objPlayerNo;
	[SerializeField]
	[Header("スタミナゲ\u30fcジル\u30fcト")]
	private GameObject[] arrayStaminaGaugeRoot;
	[SerializeField]
	[Header("スタミナゲ\u30fcジ")]
	private SpriteRenderer[] arrayStaminaGauge;
	[SerializeField]
	[Header("チ\u30fcムスコア")]
	private TextMeshPro[] arrayTeamScore;
	[SerializeField]
	[Header("現在ピリオド")]
	private TextMeshPro textGamePeriod;
	[SerializeField]
	[Header("チ\u30fcムA所属プレイヤ\u30fc")]
	private SpriteRenderer[] arrayTeamPlayerA;
	[SerializeField]
	[Header("チ\u30fcムB所属プレイヤ\u30fc")]
	private SpriteRenderer[] arrayTeamPlayerB;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer[] arrayPlayerIcon;
	[SerializeField]
	[Header("所属プレイヤ\u30fc画像")]
	private Sprite[] arrayTeamPlayerSprite;
	[SerializeField]
	[Header("チ\u30fcムAフレ\u30fcム")]
	private SpriteRenderer teamColorFrameA;
	[SerializeField]
	[Header("チ\u30fcムBフレ\u30fcム")]
	private SpriteRenderer teamColorFrameB;
	[SerializeField]
	[Header("チ\u30fcムフレ\u30fcム画像")]
	private Sprite[] arrayTeamColorFrame;
	[SerializeField]
	[Header("一人用表示画像")]
	private Sprite[] arraySinglePlayerName;
	[SerializeField]
	[Header("時間表示")]
	private TextMeshPro textTime;
	[SerializeField]
	[Header("ピリオド表示")]
	private GameObject cutInPeriod;
	[SerializeField]
	[Header("ピリオド画像")]
	private Sprite[] arrayPeriodSprite;
	[SerializeField]
	[Header("ピリオド画像（英語）")]
	private Sprite[] arrayPeriodSpriteEnglish;
	[SerializeField]
	[Header("ピリオドテキスト")]
	private SpriteRenderer textPeriod;
	[SerializeField]
	[Header("フェイスオフ表示")]
	private GameObject cutInFaceOff;
	[SerializeField]
	[Header("ピリオド終了")]
	private GameObject cutInEndPeriod;
	[SerializeField]
	[Header("ピリオド終了テキスト")]
	private SpriteRenderer textEndPeriod;
	[SerializeField]
	[Header("フェ\u30fcド用カメラ前フレ\u30fcム")]
	private CameraPrevFrame cameraPrevFrame;
	[SerializeField]
	[Header("前フレ\u30fcム表示フェ\u30fcド")]
	private MeshRenderer prevFrameMesh;
	[SerializeField]
	[Header("★エフェクトアンカ\u30fc")]
	private Transform effectAnchor;
	[SerializeField]
	[Header("ゴ\u30fcル紙吹雪")]
	private ParticleSystem[] psCrackerEffect;
	[SerializeField]
	[Header("ゴ\u30fcル表示")]
	private BB_GoalEffect prefabGoalEffect;
	[SerializeField]
	[Header("UIレイアウト【シングル】")]
	private GameObject objLayoutSingle;
	[SerializeField]
	[Header("UIレイアウト【マルチ】")]
	private GameObject objLayoutMulti;
	[SerializeField]
	[Header("フェイスオフ案内表示")]
	private ControllerBalloonUI faceOffInput;
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
	private float OFFSET_PLAYER_NO_Y = 125f;
	private float OFFSET_STAMINA_GAUGE = -75f;
	private int gameTime;
	private bool isFaceOffInputShow;
	public bool IsPrevFrameFade
	{
		get;
		set;
	}
	public void Init()
	{
		objLayoutSingle.SetActive(value: true);
		objLayoutMulti.SetActive(value: false);
		teamColorFrameA.sprite = arrayTeamColorFrame[SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayTeamColor[0]];
		teamColorFrameB.sprite = arrayTeamColorFrame[SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayTeamColor[1]];
		faceOffInput.Init();
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamA.Length; i++)
		{
			if (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamA[i].PlayerIdx < GS_Define.PLAYER_MAX)
			{
				if (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamA[i].IsCpu)
				{
					list.Add(4 + (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamA[i].PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum));
				}
				else
				{
					list.Add(SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamA[i].PlayerIdx);
				}
			}
		}
		for (int j = 0; j < SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamB.Length; j++)
		{
			if (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamB[j].PlayerIdx < GS_Define.PLAYER_MAX)
			{
				if (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamB[j].IsCpu)
				{
					list2.Add(4 + (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamB[j].PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum));
				}
				else
				{
					list2.Add(SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamB[j].PlayerIdx);
				}
			}
		}
		list.Sort();
		list2.Sort();
		int num = 0;
		int num2 = 0;
		for (int k = 0; k < arrayTeamPlayerA.Length; k++)
		{
			arrayTeamPlayerA[k].gameObject.SetActive(value: false);
		}
		for (int l = 0; l < arrayTeamPlayerB.Length; l++)
		{
			arrayTeamPlayerB[l].gameObject.SetActive(value: false);
		}
		for (int m = 0; m < list.Count; m++)
		{
			num2 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list[m]];
			if (list[m] <= SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
				arrayTeamPlayerA[m].gameObject.SetActive(value: true);
				arrayTeamPlayerA[m].sprite = arrayTeamPlayerSprite[list[m]];
				if (m == 0 && SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
				{
					arrayTeamPlayerA[m].sprite = arraySinglePlayerName[(Localize_Define.Language != 0) ? 1 : 0];
				}
			}
			arrayPlayerIcon[num].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARA_DEFAULT_SPRITE_NAMES[num2]);
			num++;
		}
		for (int n = 0; n < list2.Count; n++)
		{
			num2 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[list2[n]];
			if (list2[n] <= SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
				arrayTeamPlayerB[n].gameObject.SetActive(value: true);
				arrayTeamPlayerB[n].sprite = arrayTeamPlayerSprite[list2[n]];
			}
			arrayPlayerIcon[num].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARA_DEFAULT_SPRITE_NAMES[num2]);
			num++;
		}
	}
	public void PlayGoalEffect()
	{
		for (int i = 0; i < psCrackerEffect.Length; i++)
		{
			psCrackerEffect[i].Play();
		}
		BB_GoalEffect bB_GoalEffect = UnityEngine.Object.Instantiate(prefabGoalEffect, effectAnchor);
		bB_GoalEffect.gameObject.SetActive(value: true);
		bB_GoalEffect.Play();
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
	public void SetScore(int[] _arrayScore)
	{
		for (int i = 0; i < _arrayScore.Length; i++)
		{
			arrayTeamScore[i].SetText(_arrayScore[i].ToString());
		}
	}
	public void SetTime(float _time)
	{
		gameTime = (int)_time;
		textTime.SetText(((int)(_time / 60f)).ToString() + ":" + ((int)(_time % 60f)).ToString("00"));
	}
	public void ShowPeried(int _periodNum, Action _callBack)
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
		cutInPeriod.SetActive(value: true);
		cutInPeriod.transform.SetLocalScale(0f, 0f, 0f);
		switch (_periodNum)
		{
		case 0:
			textGamePeriod.SetText("1st");
			switch (Localize_Define.Language)
			{
			case Localize_Define.LanguageType.Japanese:
				textPeriod.sprite = arrayPeriodSprite[0];
				break;
			case Localize_Define.LanguageType.English:
				textPeriod.sprite = arrayPeriodSpriteEnglish[0];
				break;
			}
			break;
		case 1:
			textGamePeriod.SetText("2nd");
			switch (Localize_Define.Language)
			{
			case Localize_Define.LanguageType.Japanese:
				textPeriod.sprite = arrayPeriodSprite[1];
				break;
			case Localize_Define.LanguageType.English:
				textPeriod.sprite = arrayPeriodSpriteEnglish[1];
				break;
			}
			break;
		case 2:
			textGamePeriod.SetText("3rd");
			switch (Localize_Define.Language)
			{
			case Localize_Define.LanguageType.Japanese:
				textPeriod.sprite = arrayPeriodSprite[2];
				break;
			case Localize_Define.LanguageType.English:
				textPeriod.sprite = arrayPeriodSpriteEnglish[2];
				break;
			}
			break;
		}
		LeanTween.cancel(cutInPeriod);
		LeanTween.scale(cutInPeriod, Vector3.one, 0.5f).setEaseOutBack().setOnComplete((Action)delegate
		{
			LeanTween.scale(cutInPeriod, Vector3.zero, 0.25f).setDelay(1f).setOnComplete((Action)delegate
			{
				_callBack?.Invoke();
				cutInPeriod.SetActive(value: false);
			});
		});
	}
	public void ShowPeriedEnd(int _periodNum, Action _callBack)
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
		if (_periodNum == 2)
		{
			SingletonCustom<CommonEndSimple>.Instance.Show(_callBack);
			return;
		}
		cutInEndPeriod.SetActive(value: true);
		cutInEndPeriod.transform.SetLocalScale(0f, 0f, 0f);
		switch (_periodNum)
		{
		case 0:
			switch (Localize_Define.Language)
			{
			case Localize_Define.LanguageType.Japanese:
				textEndPeriod.sprite = arrayPeriodSprite[0];
				break;
			case Localize_Define.LanguageType.English:
				textEndPeriod.sprite = arrayPeriodSpriteEnglish[0];
				break;
			}
			break;
		case 1:
			switch (Localize_Define.Language)
			{
			case Localize_Define.LanguageType.Japanese:
				textEndPeriod.sprite = arrayPeriodSprite[1];
				break;
			case Localize_Define.LanguageType.English:
				textEndPeriod.sprite = arrayPeriodSpriteEnglish[1];
				break;
			}
			break;
		}
		LeanTween.cancel(cutInEndPeriod);
		LeanTween.scale(cutInEndPeriod, Vector3.one, 0.5f).setEaseOutBack().setOnComplete((Action)delegate
		{
			LeanTween.scale(cutInEndPeriod, Vector3.zero, 0.25f).setDelay(1f).setOnComplete((Action)delegate
			{
				_callBack?.Invoke();
				cutInEndPeriod.SetActive(value: false);
			});
		});
	}
	public void ShowFaceOff(float _delay, Action _callBack)
	{
		SingletonCustom<AudioManager>.Instance.VoicePlay("voice_ice_hockey_faceoff");
		cutInFaceOff.transform.SetLocalPositionY(310f);
		LeanTween.delayedCall(cutInFaceOff, _delay, (Action)delegate
		{
			if (!isFaceOffInputShow)
			{
				faceOffInput.FadeProcess_ControlInfomationUI(_fadeIn: true);
			}
			SingletonCustom<IceHockeyCameraMover>.Instance.SetState(IceHockeyCameraMover.State.FACE_OFF);
			cutInFaceOff.SetActive(value: true);
			cutInFaceOff.transform.SetLocalScale(0f, 0f, 0f);
			LeanTween.cancel(cutInFaceOff);
			LeanTween.moveLocalY(cutInFaceOff, 310f, 0.25f).setEaseOutQuart().setOnComplete((Action)delegate
			{
			});
			LeanTween.scale(cutInFaceOff, Vector3.one, 0.25f).setEaseOutBack().setOnComplete((Action)delegate
			{
				LeanTween.scale(cutInFaceOff, Vector3.zero, 0.25f).setDelay(1f).setEaseInBack()
					.setOnComplete((Action)delegate
					{
						if (!isFaceOffInputShow)
						{
							faceOffInput.FadeProcess_ControlInfomationUI(_fadeIn: false);
							isFaceOffInputShow = true;
						}
						_callBack?.Invoke();
						cutInFaceOff.SetActive(value: false);
					});
			});
		});
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < objPlayerNo.Length; i++)
		{
			if (i < SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
				CalcManager.mCalcVector3 = SingletonCustom<IceHockeyCameraMover>.Instance.GetCamera().WorldToScreenPoint(SingletonCustom<IceHockeyPlayerManager>.Instance.GetPlayerAtIdx(i).transform.position);
				CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
				objPlayerNo[i].transform.SetPosition(Mathf.Clamp(CalcManager.mCalcVector3.x, -870f, 870f), Mathf.Clamp(CalcManager.mCalcVector3.y + OFFSET_PLAYER_NO_Y, -458f, 458f), objPlayerNo[i].transform.position.z);
				if (SingletonCustom<IceHockeyPuck>.Instance.IsHoleder(i))
				{
					arrayStaminaGaugeRoot[i].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y + OFFSET_STAMINA_GAUGE, arrayStaminaGaugeRoot[i].transform.position.z);
					arrayStaminaGauge[i].size = new Vector2(132f * SingletonCustom<IceHockeyPlayerManager>.Instance.GetPlayerAtIdx(i).GetStaminaPer(), arrayStaminaGauge[i].size.y);
				}
				else
				{
					arrayStaminaGaugeRoot[i].transform.SetLocalPositionX(-2000f);
				}
			}
			else
			{
				objPlayerNo[i].transform.SetLocalPositionX(-2000f);
				arrayStaminaGaugeRoot[i].transform.SetLocalPositionX(-2000f);
			}
		}
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(cutInPeriod);
		LeanTween.cancel(cutInFaceOff);
		LeanTween.cancel(cutInEndPeriod);
	}
}
