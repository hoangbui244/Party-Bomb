using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class MikoshiRaceUiManager : SingletonCustom<MikoshiRaceUiManager>
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
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer fade;
	[SerializeField]
	private GameObject multiPartitionTwo;
	[SerializeField]
	private GameObject multiPartitionFour;
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
	private CommonGameTimeUI_Font_Time raceTimeSetOne;
	[SerializeField]
	private CommonGameTimeUI_Font_Time[] raceTimeSetTwo;
	[SerializeField]
	private CommonGameTimeUI_Font_Time[] raceTimeSetFour;
	private MikoshiRaceMapUI courseMap;
	[SerializeField]
	[Header("コ\u30fcスマップ表示")]
	private MikoshiRaceMapUI courseMap_One;
	[SerializeField]
	private MikoshiRaceMapUI courseMap_Two;
	[SerializeField]
	private MikoshiRaceMapUI courseMap_Four;
	private MikoshiRaceShakeUi[] shakeUis;
	[SerializeField]
	[Header("神輿振り表示")]
	private MikoshiRaceShakeUi[] shakeUis_One;
	[SerializeField]
	private MikoshiRaceShakeUi[] shakeUis_Two;
	[SerializeField]
	private MikoshiRaceShakeUi[] shakeUis_Four;
	[SerializeField]
	[Header("開始時操作表示UI")]
	private ControlInfomationUI[] firstCtrlUIs;
	[SerializeField]
	[Header("神輿振り操作表示UI")]
	private ControlInfomationUI[] shakeCtrlUIs;
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
	private int cameraNum;
	private int[] rankPrev = new int[4];
	private bool isCanChangeRank;
	private float[] numSpriteSize = new float[4];
	private float[] numSpriteSizeStart = new float[4];
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
		playerNum = SingletonCustom<MikoshiRaceGameManager>.Instance.PlayerNum;
		cameraNum = SingletonCustom<MikoshiRaceGameManager>.Instance.CameraNum;
		if (cameraNum == 2)
		{
			multiPartitionTwo.SetActive(value: true);
		}
		else if (cameraNum > 2)
		{
			multiPartitionFour.SetActive(value: true);
		}
		groupSprite.gameObject.SetActive(SingletonCustom<MikoshiRaceGameManager>.Instance.HasSecondGroup);
		switch (cameraNum)
		{
		case 1:
			onePlayerAnchor.SetActive(value: true);
			onePlayerAnchor_Sub.SetActive(value: true);
			numSpriteSizeStart[0] = nowRankSpriteOne.transform.localScale.x;
			numSpriteSizeStart[1] = (numSpriteSizeStart[2] = (numSpriteSizeStart[3] = 0f));
			courseMap = courseMap_One;
			shakeUis = shakeUis_One;
			break;
		case 2:
			twoPlayerAnchor.SetActive(value: true);
			twoPlayerAnchor_Sub.SetActive(value: true);
			for (int l = 0; l < nowRankSpriteTwo.Length; l++)
			{
				numSpriteSizeStart[l] = nowRankSpriteTwo[l].transform.localScale.x;
			}
			courseMap = courseMap_Two;
			shakeUis = shakeUis_Two;
			for (int m = 0; m < ctrlUIAnchors_Two.Length; m++)
			{
				firstCtrlUIs[m].anchor.transform.position = ctrlUIAnchors_Two[m].position;
				firstCtrlUIs[m].anchor.transform.localScale = ctrlUIAnchors_Two[m].localScale;
				shakeCtrlUIs[m].anchor.transform.position = ctrlUIAnchors_Two[m].position;
				shakeCtrlUIs[m].anchor.transform.localScale = ctrlUIAnchors_Two[m].localScale;
			}
			break;
		case 3:
		case 4:
			fourPlayerAnchor.SetActive(value: true);
			fourPlayerAnchor_Sub.SetActive(value: true);
			if (SingletonCustom<MikoshiRaceGameManager>.Instance.TeamNum == 2)
			{
				teamSpriteAnchor.SetActive(value: true);
				for (int i = 0; i < teamSpriteFour.Length; i++)
				{
					teamSpriteFour[i].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, TEAM_SPRITE_NAMES[SingletonCustom<MikoshiRaceCarManager>.Instance.GetCar(i).TeamNo]);
				}
			}
			else
			{
				teamSpriteAnchor.SetActive(value: false);
			}
			for (int j = 0; j < nowRankSpriteFour.Length; j++)
			{
				numSpriteSizeStart[j] = nowRankSpriteFour[j].transform.localScale.x;
			}
			courseMap = courseMap_Four;
			shakeUis = shakeUis_Four;
			for (int k = 0; k < ctrlUIAnchors_Four.Length; k++)
			{
				firstCtrlUIs[k].anchor.transform.position = ctrlUIAnchors_Four[k].position;
				firstCtrlUIs[k].anchor.transform.localScale = ctrlUIAnchors_Four[k].localScale;
				shakeCtrlUIs[k].anchor.transform.position = ctrlUIAnchors_Four[k].position;
				shakeCtrlUIs[k].anchor.transform.localScale = ctrlUIAnchors_Four[k].localScale;
			}
			break;
		}
		courseMap.MapInit();
		for (int n = 0; n < shakeUis.Length; n++)
		{
			shakeUis[n].Init();
		}
		if (playerNum == 3)
		{
			playerSpriteFour4P.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[4]);
		}
		for (int num = 0; num < numSpriteSize.Length; num++)
		{
			numSpriteSize[num] = numSpriteSizeStart[num];
		}
		for (int num2 = 0; num2 < rankPrev.Length; num2++)
		{
			rankPrev[num2] = 0;
		}
		int num3 = cameraNum;
		if (num3 == 3)
		{
			num3 = 4;
		}
		for (int num4 = 0; num4 < num3; num4++)
		{
			int num5 = 0;
			if (num4 > 1 && SingletonCustom<MikoshiRaceGameManager>.Instance.IsEightBattle)
			{
				num5 = 4;
			}
			switch (cameraNum)
			{
			case 1:
				nowRankSpriteOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, NOW_RANK_SPRITE_NAMES[num5]);
				break;
			case 2:
				nowRankSpriteTwo[num4].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, NOW_RANK_SPRITE_NAMES[num5]);
				break;
			case 3:
			case 4:
				nowRankSpriteFour[num4].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, NOW_RANK_SPRITE_NAMES[num5]);
				break;
			}
		}
		if (SingletonCustom<MikoshiRaceGameManager>.Instance.HasSecondGroup)
		{
			playerSpriteTwo[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[SingletonCustom<MikoshiRaceCarManager>.Instance.GetCar(0).PlayerNo]);
			playerSpriteTwo[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[SingletonCustom<MikoshiRaceCarManager>.Instance.GetCar(1).PlayerNo]);
		}
		DataInit();
	}
	public void SecondGroupInit()
	{
		playerNum = SingletonCustom<MikoshiRaceGameManager>.Instance.PlayerNum;
		if (cameraNum == 1)
		{
			onePlayerAnchor.SetActive(value: true);
			numSpriteSizeStart[0] = nowRankSpriteOne.transform.localScale.x;
			numSpriteSizeStart[1] = (numSpriteSizeStart[2] = (numSpriteSizeStart[3] = 0f));
			twoPlayerAnchor.SetActive(value: true);
			playerSpriteTwo[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[SingletonCustom<MikoshiRaceCarManager>.Instance.GetCar(0).PlayerNo]);
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
			playerSpriteTwo[0].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[SingletonCustom<MikoshiRaceCarManager>.Instance.GetCar(0).PlayerNo]);
			playerSpriteTwo[1].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[SingletonCustom<MikoshiRaceCarManager>.Instance.GetCar(1).PlayerNo]);
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
		for (int k = 0; k < shakeUis.Length; k++)
		{
			shakeUis[k].Hide();
		}
		DataInit();
	}
	private void DataInit()
	{
		for (int i = 0; i < firstCtrlUIs.Length; i++)
		{
			firstCtrlUIs[i].SetActive(_active: false);
			firstCtrlUIs[i].SetAlpha(0f);
		}
		for (int j = 0; j < shakeCtrlUIs.Length; j++)
		{
			shakeCtrlUIs[j].SetActive(_active: false);
			shakeCtrlUIs[j].SetAlpha(0f);
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
		switch (cameraNum)
		{
		case 1:
			nowRankSpriteOne.transform.SetLocalScale(numSpriteSize[0], numSpriteSize[0], 0f);
			break;
		case 2:
			for (int k = 0; k < nowRankSpriteTwo.Length; k++)
			{
				nowRankSpriteTwo[k].transform.SetLocalScale(numSpriteSize[k], numSpriteSize[k], 0f);
			}
			break;
		case 3:
		case 4:
			for (int j = 0; j < nowRankSpriteFour.Length; j++)
			{
				nowRankSpriteFour[j].transform.SetLocalScale(numSpriteSize[j], numSpriteSize[j], 0f);
			}
			break;
		}
		courseMap.MapUpdate();
	}
	public void ShowRankSprite(bool _isFade)
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		switch (cameraNum)
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
		switch (cameraNum)
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
			switch (cameraNum)
			{
			case 1:
				nowRankSpriteOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, NOW_RANK_SPRITE_NAMES[rank]);
				break;
			case 2:
				nowRankSpriteTwo[no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, NOW_RANK_SPRITE_NAMES[rank]);
				break;
			case 3:
			case 4:
				nowRankSpriteFour[no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, NOW_RANK_SPRITE_NAMES[rank]);
				break;
			}
			numSpriteSize[no] = 0f;
		}
	}
	public void SetResultRankSprite(int no, int rank)
	{
		switch (cameraNum)
		{
		case 1:
			resultRankSpriteOne.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, RESULT_RANK_SPRITE_NAMES[rank]);
			resultRankSpriteOne.gameObject.SetActive(value: true);
			break;
		case 2:
		{
			resultRankSpriteTwo[no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, RESULT_RANK_SPRITE_NAMES[rank]);
			resultRankSpriteTwo[no].gameObject.SetActive(value: true);
			resultRankSpriteTwo[no].transform.localScale = new Vector3(100f, 100f, 100f);
			Vector3 numSpriteScaleTwo = resultRankSpriteTwo[no].transform.localScale;
			LeanTween.value(base.gameObject, 1f, 0f, 0.2f).setDelay(3f).setOnUpdate(delegate(float _value)
			{
				resultRankSpriteTwo[no].transform.localScale = numSpriteScaleTwo * _value;
			})
				.setOnComplete((Action)delegate
				{
					resultRankSpriteTwo[no].gameObject.SetActive(value: false);
				});
			break;
		}
		case 3:
		case 4:
		{
			resultRankSpriteFour[no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, RESULT_RANK_SPRITE_NAMES[rank]);
			resultRankSpriteFour[no].gameObject.SetActive(value: true);
			Vector3 numSpriteScaleFour = resultRankSpriteFour[no].transform.localScale;
			LeanTween.value(base.gameObject, 1f, 0f, 0.2f).setDelay(3f).setOnUpdate(delegate(float _value)
			{
				resultRankSpriteFour[no].transform.localScale = numSpriteScaleFour * _value;
			})
				.setOnComplete((Action)delegate
				{
					resultRankSpriteFour[no].gameObject.SetActive(value: false);
				});
			break;
		}
		}
	}
	public void SetTime(int no, float time)
	{
		switch (cameraNum)
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
	public void ViewShakeControlInfo(int no)
	{
		shakeUis[no].Show();
	}
	public void HideShakeControlInfo(int no)
	{
		shakeUis[no].Hide();
	}
	public void ShakePush(int no)
	{
		shakeUis[no].Push();
	}
	public void SetShakeGauge(int no, float lerp)
	{
		shakeUis[no].SetGauge(lerp);
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
	public void ReverseRunON(int no)
	{
		switch (cameraNum)
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
		switch (cameraNum)
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
