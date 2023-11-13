using UnityEngine;
public class Hidden_UiManager : SingletonCustom<Hidden_UiManager>
{
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
	private const string CPU_PLAYER_ICON_SPRITE_NAME = "_screen_cp";
	[SerializeField]
	private GameObject singleObj;
	[SerializeField]
	private GameObject multiObj;
	[SerializeField]
	private GameObject countDownObj;
	[SerializeField]
	private SpriteRenderer countDownSpRenderer;
	[SerializeField]
	private Sprite[] countDownSprites;
	[SerializeField]
	private Sprite[] countDownSprites_EN;
	[SerializeField]
	[Header("1人用UI")]
	private CommonWaterPistolBattleUILayout singleUiLayout;
	[SerializeField]
	private GameObject[] singleOniOmenObjs;
	[SerializeField]
	private Transform singleStaminaGaugeAnchor;
	[SerializeField]
	private GameObject singleActionCtrlObj;
	[SerializeField]
	private CourseMapUI singleMap;
	[SerializeField]
	[Header("複数人用UI")]
	private GameObject multiPartition;
	[SerializeField]
	private SpriteRenderer[] multiCharaIcons;
	[SerializeField]
	private SpriteRenderer[] multiPlayerIcons;
	[SerializeField]
	private GameObject[] multiOniOmenObjs;
	[SerializeField]
	private CommonGameTimeUI_Font_Time multiTime;
	[SerializeField]
	private SpriteNumbers[] multiScores;
	[SerializeField]
	private Transform[] multiStaminaGaugeAnchors;
	[SerializeField]
	private GameObject[] multiCtrlViewObjs;
	[SerializeField]
	private GameObject[] multiActionCtrlObjs;
	[SerializeField]
	private CourseMapUI[] multiMap;
	private bool isSingle;
	public void Init()
	{
		int playerNum = SingletonCustom<Hidden_CharacterManager>.Instance.PlayerNum;
		isSingle = (playerNum == 1);
		if (isSingle)
		{
			singleObj.SetActive(value: true);
			multiObj.SetActive(value: false);
			singleUiLayout.Init(60f, SingletonCustom<GameSettingManager>.Instance.PlayerGroupList);
		}
		else
		{
			singleObj.SetActive(value: false);
			multiObj.SetActive(value: true);
			multiTime.SetTime(60f);
			for (int i = 0; i < multiCharaIcons.Length; i++)
			{
				int num = (i < playerNum) ? SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[i] : SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[4 + i - playerNum];
				multiCharaIcons[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARA_DEFAULT_SPRITE_NAMES[num]);
			}
			int num2 = 1;
			for (int j = playerNum; j < multiPlayerIcons.Length; j++)
			{
				multiPlayerIcons[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + num2.ToString());
				num2++;
			}
			for (int k = playerNum; k < multiCtrlViewObjs.Length; k++)
			{
				multiCtrlViewObjs[k].SetActive(value: false);
			}
		}
		SetScoreArray(new int[4]);
		int playerNum2 = SingletonCustom<Hidden_CharacterManager>.Instance.PlayerNum;
		SetOniView(SingletonCustom<Hidden_CharacterManager>.Instance.GetOniChara().CharaNo);
	}
	public void UpdateMethod()
	{
		StaminaGaugeUpdate();
	}
	public void SetTime(float _time)
	{
		if (isSingle)
		{
			singleUiLayout.SetTime(_time);
		}
		else
		{
			multiTime.SetTime(_time);
		}
	}
	public void SetScore(int _charaNo, int _score)
	{
		if (isSingle)
		{
			singleUiLayout.SetScore(_charaNo, _score);
		}
		else
		{
			multiScores[_charaNo].Set(_score);
		}
	}
	public void SetScoreArray(int[] _scores)
	{
		if (isSingle)
		{
			singleUiLayout.SetScoreArray(_scores);
			return;
		}
		for (int i = 0; i < multiScores.Length; i++)
		{
			multiScores[i].Set(_scores[i]);
		}
	}
	private void StaminaGaugeUpdate()
	{
		if (isSingle)
		{
			singleStaminaGaugeAnchor.SetLocalScaleX(SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(0).GetStaminaPer());
			return;
		}
		for (int i = 0; i < multiStaminaGaugeAnchors.Length; i++)
		{
			multiStaminaGaugeAnchors[i].SetLocalScaleX(SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(i).GetStaminaPer());
		}
	}
	public void SetActionCtrlView(int _playerNo, bool _active)
	{
		if (isSingle)
		{
			if (_playerNo == 0)
			{
				singleActionCtrlObj.SetActive(_active);
			}
		}
		else
		{
			multiActionCtrlObjs[_playerNo].SetActive(_active);
		}
	}
	public void SetOniView(int _oniCharaNo)
	{
		if (isSingle)
		{
			for (int i = 0; i < singleOniOmenObjs.Length; i++)
			{
				singleOniOmenObjs[i].SetActive(i == _oniCharaNo);
			}
		}
		else
		{
			for (int j = 0; j < multiOniOmenObjs.Length; j++)
			{
				multiOniOmenObjs[j].SetActive(j == _oniCharaNo);
			}
		}
	}
	public void SetCountDownView(bool _active)
	{
		countDownObj.SetActive(_active);
	}
	public void SetCountDownNum(int _num)
	{
		switch (_num)
		{
		case 3:
			countDownSpRenderer.sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? countDownSprites[2] : countDownSprites_EN[2]);
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_count_down_3");
			break;
		case 2:
			countDownSpRenderer.sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? countDownSprites[1] : countDownSprites_EN[1]);
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_count_down_2");
			break;
		case 1:
			countDownSpRenderer.sprite = ((Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? countDownSprites[0] : countDownSprites_EN[0]);
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_count_down_1");
			break;
		}
	}
	public void CloseGameUI()
	{
		singleObj.SetActive(value: false);
		multiObj.SetActive(value: false);
		if (!isSingle)
		{
			multiPartition.transform.parent = base.transform;
		}
	}
}
