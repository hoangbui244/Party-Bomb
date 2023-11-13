using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class MonsterRace_UiManager : SingletonCustom<MonsterRace_UiManager>
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
	private const string SECOND_GROUP_SPRITE_NAME = "_play_2group";
	private const string SECOND_LAP_SPRITE_NAME = "race_text_2nd_lap";
	private const string FINAL_LAP_SPRITE_NAME = "race_text_final_lap";
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
	private static readonly string[] NOW_RANK_SPRITE_NAMES = new string[8]
	{
		"_common_rank_s_0",
		"_common_rank_s_1",
		"_common_rank_s_2",
		"_common_rank_s_3",
		"_common_rank_s_4",
		"_common_rank_s_5",
		"_common_rank_s_6",
		"_common_rank_s_7"
	};
	private static readonly string[] RESULT_RANK_SPRITE_NAMES = new string[8]
	{
		"_common_rank_0",
		"_common_rank_1",
		"_common_rank_2",
		"_common_rank_3",
		"_common_rank_4",
		"_common_rank_5",
		"_common_rank_6",
		"_common_rank_7"
	};
	private static readonly string[] LAP_SPRITE_NAMES = new string[4]
	{
		"_common_number_b_1",
		"_common_number_b_1",
		"_common_number_b_2",
		"_common_number_b_3"
	};
	[SerializeField]
	private Sprite secondLapSprite;
	[SerializeField]
	private Sprite secondLapSprite_EN;
	[SerializeField]
	private Sprite finalLapSprite;
	[SerializeField]
	private Sprite finalLapSprite_EN;
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer fade;
	[SerializeField]
	private GameObject multiPartitionTwo;
	[SerializeField]
	private GameObject multiPartitionFour;
	[SerializeField]
	private GameObject[] hideThreePlayerObjs;
	[SerializeField]
	private GameObject onePlayerAnchor;
	[SerializeField]
	private GameObject twoPlayerAnchor;
	[SerializeField]
	private GameObject fourPlayerAnchor;
	[SerializeField]
	private GameObject onePlayerAnchor_Sub;
	[SerializeField]
	private GameObject twoPlayerAnchor_Sub;
	[SerializeField]
	private GameObject fourPlayerAnchor_Sub;
	[SerializeField]
	private SpriteRenderer[] playerSpriteTwo;
	[SerializeField]
	private SpriteRenderer playerSpriteFour4P;
	[SerializeField]
	private GameObject teamSpriteAnchor;
	[SerializeField]
	private SpriteRenderer[] teamSpriteFour;
	[SerializeField]
	private SpriteRenderer nowRankSpriteOne;
	[SerializeField]
	private SpriteRenderer[] nowRankSpriteTwo;
	[SerializeField]
	private SpriteRenderer[] nowRankSpriteFour;
	[SerializeField]
	private SpriteRenderer resultRankSpriteOne;
	[SerializeField]
	private SpriteRenderer[] resultRankSpriteTwo;
	[SerializeField]
	private SpriteRenderer[] resultRankSpriteFour;
	[SerializeField]
	private ResultPlacementAnimation resultRankAnimOne;
	[SerializeField]
	private ResultPlacementAnimation[] resultRankAnimTwo;
	[SerializeField]
	private ResultPlacementAnimation[] resultRankAnimFour;
	[SerializeField]
	private CommonGameTimeUI_Font_Time raceTimeSetOne;
	[SerializeField]
	private CommonGameTimeUI_Font_Time[] raceTimeSetTwo;
	[SerializeField]
	private CommonGameTimeUI_Font_Time[] raceTimeSetFour;
	[SerializeField]
	private SpriteRenderer nowLapSprite_One;
	[SerializeField]
	private SpriteRenderer[] nowLapSprite_Two;
	[SerializeField]
	private SpriteRenderer[] nowLapSprite_Four;
	[SerializeField]
	private SpriteRenderer updateLapSprite_One;
	[SerializeField]
	private SpriteRenderer[] updateLapSprite_Two;
	[SerializeField]
	private SpriteRenderer[] updateLapSprite_Four;
	[SerializeField]
	private AnimationCurve updateLapScaleCurve;
	[SerializeField]
	private AnimationCurve updateLapAlphaCurve;
	[SerializeField]
	private AnimationCurve updateLapMoveXCurve;
	private CourseMapUI courseMap;
	[SerializeField]
	[Header("コ\u30fcスマップ表示")]
	private CourseMapUI courseMap_One;
	[SerializeField]
	private CourseMapUI courseMap_Two;
	[SerializeField]
	private CourseMapUI courseMap_Four;
	[SerializeField]
	[Header("開始時操作表示UI")]
	private ControlInfomationUI[] firstCtrlUIs;
	[SerializeField]
	private Transform[] ctrlUIAnchors_Two;
	[SerializeField]
	private Transform[] ctrlUIAnchors_Four;
	[SerializeField]
	private SpriteRenderer reverseRunOne;
	[SerializeField]
	private SpriteRenderer[] reverseRunTwo;
	[SerializeField]
	private SpriteRenderer[] reverseRunFour;
	[SerializeField]
	private SpriteRenderer groupSprite;
	private int playerNum;
	private int[] rankPrev = new int[4];
	private bool isCanChangeRank;
	private float[] numSpriteSize = new float[4];
	private float[] numSpriteSizeStart = new float[4];
	[SerializeField]
	private MonsterRace_StaminaGage oneStaminaGage;
	[SerializeField]
	private MonsterRace_StaminaGage[] twoStaminaGage;
	[SerializeField]
	private MonsterRace_StaminaGage[] fourStaminaGage;
	private MonsterRace_CarScript[] cars;
	public bool IsCanChangeRank
	{
		get
		{
			return isCanChangeRank;
		}
		set
		{
			isCanChangeRank = value;
		}
	}
	public void Init()
	{
		playerNum = SingletonCustom<MonsterRace_CarManager>.Instance.PlayerNum;
		cars = new MonsterRace_CarScript[4];
		for (int i = 0; i < 4; i++)
		{
			cars[i] = SingletonCustom<MonsterRace_CarManager>.Instance.GetCar(i);
		}
		if (playerNum == 2)
		{
			multiPartitionTwo.SetActive(value: true);
		}
		else if (playerNum > 2)
		{
			multiPartitionFour.SetActive(value: true);
		}
		if (playerNum == 3)
		{
			for (int j = 0; j < hideThreePlayerObjs.Length; j++)
			{
				hideThreePlayerObjs[j].SetActive(value: false);
			}
		}
		switch (playerNum)
		{
		case 1:
			oneStaminaGage.Init();
			oneStaminaGage.Car = cars[0];
			break;
		case 2:
			for (int l = 0; l < playerNum; l++)
			{
				twoStaminaGage[l].Init();
				twoStaminaGage[l].Car = cars[l];
			}
			break;
		case 3:
		case 4:
			for (int k = 0; k < cars.Length; k++)
			{
				fourStaminaGage[k].Init();
				fourStaminaGage[k].Car = cars[k];
			}
			UnityEngine.Debug.Log("設定したよ");
			break;
		}
		switch (playerNum)
		{
		case 1:
			onePlayerAnchor.SetActive(value: true);
			onePlayerAnchor_Sub.SetActive(value: true);
			numSpriteSizeStart[0] = nowRankSpriteOne.transform.localScale.x;
			numSpriteSizeStart[1] = (numSpriteSizeStart[2] = (numSpriteSizeStart[3] = 0f));
			courseMap = courseMap_One;
			break;
		case 2:
			twoPlayerAnchor.SetActive(value: true);
			twoPlayerAnchor_Sub.SetActive(value: true);
			for (int num2 = 0; num2 < nowRankSpriteTwo.Length; num2++)
			{
				numSpriteSizeStart[num2] = nowRankSpriteTwo[num2].transform.localScale.x;
			}
			courseMap = courseMap_Two;
			for (int num3 = 0; num3 < ctrlUIAnchors_Two.Length; num3++)
			{
				firstCtrlUIs[num3].anchor.transform.position = ctrlUIAnchors_Two[num3].position;
				firstCtrlUIs[num3].anchor.transform.localScale = ctrlUIAnchors_Two[num3].localScale;
			}
			break;
		case 3:
		case 4:
			fourPlayerAnchor.SetActive(value: true);
			fourPlayerAnchor_Sub.SetActive(value: true);
			if (SingletonCustom<MonsterRace_CarManager>.Instance.TeamNum == 2)
			{
				teamSpriteAnchor.SetActive(value: true);
				for (int m = 0; m < teamSpriteFour.Length; m++)
				{
					teamSpriteFour[m].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, TEAM_SPRITE_NAMES[SingletonCustom<MonsterRace_CarManager>.Instance.GetCar(m).TeamNo]);
				}
			}
			else
			{
				teamSpriteAnchor.SetActive(value: false);
			}
			for (int n = 0; n < nowRankSpriteFour.Length; n++)
			{
				numSpriteSizeStart[n] = nowRankSpriteFour[n].transform.localScale.x;
			}
			courseMap = courseMap_Four;
			for (int num = 0; num < ctrlUIAnchors_Four.Length; num++)
			{
				firstCtrlUIs[num].anchor.transform.position = ctrlUIAnchors_Four[num].position;
				firstCtrlUIs[num].anchor.transform.localScale = ctrlUIAnchors_Four[num].localScale;
			}
			break;
		}
		courseMap.Init();
		MonsterRace_Course course = SingletonCustom<MonsterRace_CarManager>.Instance.Course;
		courseMap.SetWorldRangeAnchor(course.courseRightTop, course.courseLeftBottom);
		courseMap.SetWorldTargetAnchor(SingletonCustom<MonsterRace_CarManager>.Instance.GetCarTranses());
		courseMap.SetMapData(0);
		if (playerNum == 3)
		{
			playerSpriteFour4P.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[4]);
		}
		for (int num4 = 0; num4 < numSpriteSize.Length; num4++)
		{
			numSpriteSize[num4] = numSpriteSizeStart[num4];
		}
		for (int num5 = 0; num5 < rankPrev.Length; num5++)
		{
			rankPrev[num5] = (SingletonCustom<MonsterRace_CarManager>.Instance.IsEightRun ? 4 : 0);
		}
		int num6 = playerNum;
		if (num6 == 3)
		{
			num6 = 4;
		}
		for (int num7 = 0; num7 < num6; num7++)
		{
			int num8 = rankPrev[num7];
			string text = NOW_RANK_SPRITE_NAMES[num8];
			if (Localize_Define.Language == Localize_Define.LanguageType.English)
			{
				text = "en" + text;
			}
			switch (playerNum)
			{
			case 1:
				nowRankSpriteOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
				break;
			case 2:
				nowRankSpriteTwo[num7].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
				break;
			case 3:
			case 4:
				nowRankSpriteFour[num7].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
				break;
			}
		}
		DataInit();
	}
	public void SecondGroupInit()
	{
		if (playerNum == 1)
		{
			onePlayerAnchor.SetActive(value: true);
			numSpriteSizeStart[0] = nowRankSpriteOne.transform.localScale.x;
			numSpriteSizeStart[1] = (numSpriteSizeStart[2] = (numSpriteSizeStart[3] = 0f));
			twoPlayerAnchor.SetActive(value: true);
			playerSpriteTwo[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[SingletonCustom<MonsterRace_CarManager>.Instance.GetCar(0).PlayerNo]);
			playerSpriteTwo[1].gameObject.SetActive(value: false);
		}
		else
		{
			SetTime(0, 0f);
			SetTime(1, 0f);
			ChangeRankNum(0, 0);
			ChangeRankNum(1, 2);
			resultRankSpriteTwo[0].gameObject.SetActive(value: false);
			resultRankSpriteTwo[1].gameObject.SetActive(value: false);
			playerSpriteTwo[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[SingletonCustom<MonsterRace_CarManager>.Instance.GetCar(0).PlayerNo]);
			playerSpriteTwo[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[SingletonCustom<MonsterRace_CarManager>.Instance.GetCar(1).PlayerNo]);
		}
		for (int i = 0; i < numSpriteSize.Length; i++)
		{
			numSpriteSize[i] = numSpriteSizeStart[i];
		}
		for (int j = 0; j < rankPrev.Length; j++)
		{
			rankPrev[j] = 0;
		}
		isCanChangeRank = false;
		groupSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_play_2group");
		DataInit();
	}
	private void DataInit()
	{
		for (int i = 0; i < firstCtrlUIs.Length; i++)
		{
			firstCtrlUIs[i].SetActive(_active: false);
			firstCtrlUIs[i].SetAlpha(0f);
		}
		HideRankSprite();
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < numSpriteSize.Length; i++)
		{
			if (numSpriteSize[i] < numSpriteSizeStart[i])
			{
				numSpriteSize[i] += Time.deltaTime * 4f;
			}
			else
			{
				numSpriteSize[i] = numSpriteSizeStart[i];
			}
		}
		switch (playerNum)
		{
		case 1:
			nowRankSpriteOne.transform.SetLocalScale(numSpriteSize[0], numSpriteSize[0], 0f);
			oneStaminaGage.SetItem();
			break;
		case 2:
			for (int k = 0; k < nowRankSpriteTwo.Length; k++)
			{
				twoStaminaGage[k].SetItem();
				nowRankSpriteTwo[k].transform.SetLocalScale(numSpriteSize[k], numSpriteSize[k], 0f);
			}
			break;
		case 3:
		case 4:
			for (int j = 0; j < nowRankSpriteFour.Length; j++)
			{
				nowRankSpriteFour[j].transform.SetLocalScale(numSpriteSize[j], numSpriteSize[j], 0f);
				fourStaminaGage[j].SetItem();
			}
			break;
		}
		courseMap.UpdateMethod();
	}
	public void ShowRankSprite(bool _isFade)
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		switch (playerNum)
		{
		case 1:
			nowRankSpriteOne.gameObject.SetActive(value: true);
			list.Add(nowRankSpriteOne);
			break;
		case 2:
			for (int j = 0; j < nowRankSpriteTwo.Length; j++)
			{
				nowRankSpriteTwo[j].gameObject.SetActive(value: true);
			}
			list.AddRange(nowRankSpriteTwo);
			break;
		case 3:
		case 4:
			for (int i = 0; i < nowRankSpriteFour.Length; i++)
			{
				nowRankSpriteFour[i].gameObject.SetActive(value: true);
			}
			list.AddRange(nowRankSpriteFour);
			break;
		}
		if (_isFade)
		{
			for (int k = 0; k < list.Count; k++)
			{
				list[k].SetAlpha(0f);
			}
			LeanTween.value(base.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
			{
				for (int l = 0; l < list.Count; l++)
				{
					list[l].SetAlpha(_value);
				}
			});
		}
	}
	public void HideRankSprite()
	{
		switch (playerNum)
		{
		case 1:
			nowRankSpriteOne.gameObject.SetActive(value: false);
			break;
		case 2:
			for (int j = 0; j < nowRankSpriteTwo.Length; j++)
			{
				nowRankSpriteTwo[j].gameObject.SetActive(value: false);
			}
			break;
		case 3:
		case 4:
			for (int i = 0; i < nowRankSpriteFour.Length; i++)
			{
				nowRankSpriteFour[i].gameObject.SetActive(value: false);
			}
			break;
		}
	}
	public void ChangeRankNum(int no, int rank)
	{
		if (isCanChangeRank && rankPrev[no] != rank)
		{
			rankPrev[no] = rank;
			string text = NOW_RANK_SPRITE_NAMES[rank];
			if (Localize_Define.Language == Localize_Define.LanguageType.English)
			{
				text = "en" + text;
			}
			switch (playerNum)
			{
			case 1:
				nowRankSpriteOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
				break;
			case 2:
				nowRankSpriteTwo[no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
				break;
			case 3:
			case 4:
				nowRankSpriteFour[no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
				break;
			}
			numSpriteSize[no] = 0f;
		}
	}
	public void SetResultRankSprite(int no, int rank)
	{
		string text = RESULT_RANK_SPRITE_NAMES[rank];
		if (Localize_Define.Language == Localize_Define.LanguageType.English)
		{
			text = "en" + text;
		}
		switch (playerNum)
		{
		case 1:
			resultRankSpriteOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
			resultRankSpriteOne.gameObject.SetActive(value: true);
			resultRankAnimOne.Play();
			nowRankSpriteOne.gameObject.SetActive(value: false);
			break;
		case 2:
			resultRankSpriteTwo[no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
			resultRankSpriteTwo[no].gameObject.SetActive(value: true);
			resultRankSpriteTwo[no].transform.localScale = new Vector3(100f, 100f, 100f);
			resultRankAnimTwo[no].Play();
			nowRankSpriteTwo[no].gameObject.SetActive(value: false);
			break;
		case 3:
		case 4:
			resultRankSpriteFour[no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
			resultRankSpriteFour[no].gameObject.SetActive(value: true);
			resultRankAnimFour[no].Play();
			nowRankSpriteFour[no].gameObject.SetActive(value: false);
			break;
		}
	}
	public void SetTime(int no, float time)
	{
		switch (playerNum)
		{
		case 1:
			raceTimeSetOne.SetTime(time);
			break;
		case 2:
			raceTimeSetTwo[no].SetTime(time, no);
			break;
		case 3:
		case 4:
			raceTimeSetFour[no].SetTime(time, no);
			break;
		}
	}
	public void ViewFirstControlInfo()
	{
		for (int i = 0; i < firstCtrlUIs.Length; i++)
		{
			if (i < playerNum)
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
	public void SetLapNum(int i, int num)
	{
		switch (playerNum)
		{
		case 1:
			nowLapSprite_One.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, LAP_SPRITE_NAMES[num]);
			break;
		case 2:
			nowLapSprite_Two[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, LAP_SPRITE_NAMES[num]);
			break;
		case 3:
		case 4:
			nowLapSprite_Four[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, LAP_SPRITE_NAMES[num]);
			break;
		}
	}
	public void StartLapEffect(int _no, int _lap)
	{
		Sprite sprite = secondLapSprite;
		if (Localize_Define.Language == Localize_Define.LanguageType.English)
		{
			sprite = secondLapSprite_EN;
		}
		switch (playerNum)
		{
		case 1:
			updateLapSprite_One.sprite = sprite;
			UpdateLapDirection(updateLapSprite_One);
			break;
		case 2:
			updateLapSprite_Two[_no].sprite = sprite;
			UpdateLapDirection(updateLapSprite_Two[_no]);
			break;
		case 3:
		case 4:
			updateLapSprite_Four[_no].sprite = sprite;
			UpdateLapDirection(updateLapSprite_Four[_no]);
			break;
		}
	}
	public void StartLastOneEffect(int _no)
	{
		Sprite sprite = finalLapSprite;
		if (Localize_Define.Language == Localize_Define.LanguageType.English)
		{
			sprite = finalLapSprite_EN;
		}
		switch (playerNum)
		{
		case 1:
			updateLapSprite_One.sprite = sprite;
			UpdateLapDirection(updateLapSprite_One);
			break;
		case 2:
			updateLapSprite_Two[_no].sprite = sprite;
			UpdateLapDirection(updateLapSprite_Two[_no]);
			break;
		case 3:
		case 4:
			updateLapSprite_Four[_no].sprite = sprite;
			UpdateLapDirection(updateLapSprite_Four[_no]);
			break;
		}
	}
	private void UpdateLapDirection(SpriteRenderer _renderer)
	{
		_renderer.gameObject.SetActive(value: true);
		_renderer.transform.SetLocalPositionX(0f);
		LeanTween.value(_renderer.gameObject, 0f, 1f, 0.9f).setOnUpdate(delegate(float _value)
		{
			float num = updateLapScaleCurve.Evaluate(_value);
			_renderer.transform.localScale = new Vector3(num, num, 1f);
			float a = updateLapAlphaCurve.Evaluate(_value);
			_renderer.color = new Color(1f, 1f, 1f, a);
			_renderer.transform.AddLocalPositionX(updateLapMoveXCurve.Evaluate(_value) * 15f * Time.deltaTime);
		}).setOnComplete((Action)delegate
		{
			_renderer.gameObject.SetActive(value: false);
		});
	}
	public void ReverseRunON(int no)
	{
		switch (playerNum)
		{
		case 1:
			reverseRunOne.gameObject.SetActive(value: true);
			break;
		case 2:
			reverseRunTwo[no].gameObject.SetActive(value: true);
			break;
		case 3:
		case 4:
			reverseRunFour[no].gameObject.SetActive(value: true);
			break;
		}
	}
	public void ReverseRunOFF(int no)
	{
		switch (playerNum)
		{
		case 1:
			reverseRunOne.gameObject.SetActive(value: false);
			break;
		case 2:
			reverseRunTwo[no].gameObject.SetActive(value: false);
			break;
		case 3:
		case 4:
			reverseRunFour[no].gameObject.SetActive(value: false);
			break;
		}
	}
	public void CloseGameUI()
	{
		onePlayerAnchor.SetActive(value: false);
		onePlayerAnchor_Sub.SetActive(value: false);
		twoPlayerAnchor.SetActive(value: false);
		twoPlayerAnchor_Sub.SetActive(value: false);
		fourPlayerAnchor.SetActive(value: false);
		fourPlayerAnchor_Sub.SetActive(value: false);
	}
	public void EndGame()
	{
		multiPartitionTwo.SetActive(value: false);
		multiPartitionFour.SetActive(value: false);
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
