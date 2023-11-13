using System;
using UnityEngine;
public class RoadRaceUiRaceData : MonoBehaviour
{
	[Serializable]
	public struct SpriteList
	{
		public SpriteRenderer[] sprite;
	}
	[Serializable]
	public struct ResultRankAnimData
	{
		public ResultPlacementAnimation[] anim;
	}
	[Serializable]
	public struct TimeData
	{
		public CommonGameTimeUI_Font_Time[] time;
	}
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
	private int layoutIdx;
	[SerializeField]
	private GameObject[] multiPartitions;
	[SerializeField]
	private GameObject[] playerAnchors;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteList[] playerSprite;
	[SerializeField]
	private SpriteList[] nowRankDataList;
	private float[] numSpriteSize = new float[4];
	private float[] numSpriteSizeStart = new float[4];
	private int[] rankPrev = new int[4];
	private bool isCanChangeRank;
	[SerializeField]
	private SpriteList[] resultRankDataList;
	[SerializeField]
	private ResultRankAnimData[] resultRankAnimList;
	[SerializeField]
	private TimeData[] timeDataList;
	[SerializeField]
	private SpriteList[] reverseList;
	private SpriteRenderer[] NowRankSprites => nowRankDataList[layoutIdx].sprite;
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
	private SpriteRenderer[] ResultRankSprites => resultRankDataList[layoutIdx].sprite;
	private ResultPlacementAnimation[] ResultRankAnim => resultRankAnimList[layoutIdx].anim;
	private CommonGameTimeUI_Font_Time[] TimeList => timeDataList[layoutIdx].time;
	private SpriteRenderer[] ReverseList => reverseList[layoutIdx].sprite;
	public void Init(int _playerNum)
	{
		switch (_playerNum)
		{
		case 2:
			multiPartitions[0].SetActive(value: true);
			layoutIdx = 1;
			break;
		case 3:
		case 4:
			multiPartitions[1].SetActive(value: true);
			layoutIdx = 2;
			break;
		}
		playerAnchors[layoutIdx].SetActive(value: true);
		for (int i = 0; i < numSpriteSize.Length; i++)
		{
			if (i < NowRankSprites.Length)
			{
				numSpriteSizeStart[i] = NowRankSprites[i].transform.localScale.x;
			}
			else
			{
				numSpriteSizeStart[i] = 0f;
			}
			numSpriteSize[i] = numSpriteSizeStart[i];
		}
		for (int j = 0; j < rankPrev.Length; j++)
		{
			rankPrev[j] = 0;
		}
		for (int k = 0; k < NowRankSprites.Length; k++)
		{
			ChangeRankNum(k, rankPrev[k], _isInit: true);
			NowRankSprites[k].SetAlpha(0f);
		}
		if (_playerNum == 3)
		{
			playerSprite[layoutIdx].sprite[3].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, PLAYER_SPRITE_NAMES[4]);
		}
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
		for (int j = 0; j < NowRankSprites.Length; j++)
		{
			NowRankSprites[j].transform.SetLocalScale(numSpriteSize[j], numSpriteSize[j], 0f);
		}
	}
	private void UpdateReverseRun()
	{
	}
	public void ShowRankSprite(bool _isFade)
	{
		for (int i = 0; i < NowRankSprites.Length; i++)
		{
			NowRankSprites[i].gameObject.SetActive(value: true);
		}
		if (_isFade)
		{
			for (int j = 0; j < NowRankSprites.Length; j++)
			{
				NowRankSprites[j].SetAlpha(0f);
			}
			LeanTween.value(base.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
			{
				for (int k = 0; k < NowRankSprites.Length; k++)
				{
					NowRankSprites[k].SetAlpha(_value);
				}
			});
		}
	}
	public void HideRankSprite()
	{
		for (int i = 0; i < NowRankSprites.Length; i++)
		{
			NowRankSprites[i].gameObject.SetActive(value: false);
		}
	}
	public void ChangeRankNum(int _no, int _rank, bool _isInit = false)
	{
		if (_no < NowRankSprites.Length && (isCanChangeRank || _isInit) && (rankPrev[_no] != _rank || _isInit))
		{
			rankPrev[_no] = _rank;
			string text = NOW_RANK_SPRITE_NAMES[_rank];
			if (Localize_Define.Language == Localize_Define.LanguageType.English)
			{
				text = "en" + text;
			}
			NowRankSprites[_no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
			if (!_isInit)
			{
				numSpriteSize[_no] = 0f;
			}
		}
	}
	public void SetResultRankSprite(int _no, int _rank)
	{
		if (_no < ResultRankSprites.Length)
		{
			UnityEngine.Debug.Log("SetResultRankSprite(" + _no.ToString() + ") : _rank = " + _rank.ToString());
			string text = RESULT_RANK_SPRITE_NAMES[_rank];
			if (Localize_Define.Language == Localize_Define.LanguageType.English)
			{
				text = "en" + text;
			}
			ResultRankSprites[_no].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, text);
			ResultRankSprites[_no].gameObject.SetActive(value: true);
			ResultRankAnim[_no].Play();
			NowRankSprites[_no].gameObject.SetActive(value: false);
		}
	}
	public void SetTime(int _no, float _time)
	{
		if (_no < TimeList.Length)
		{
			TimeList[_no].SetTime(_time);
		}
	}
	public void ReverseRunON(int _no)
	{
		if (_no < ReverseList.Length)
		{
			ReverseList[_no].gameObject.SetActive(value: true);
		}
	}
	public void ReverseRunOFF(int _no)
	{
		if (_no < ReverseList.Length)
		{
			ReverseList[_no].gameObject.SetActive(value: false);
		}
	}
	public void CloseGameUI()
	{
		for (int i = 0; i < playerAnchors.Length; i++)
		{
			playerAnchors[i].SetActive(value: false);
		}
	}
	public void EndGame()
	{
		for (int i = 0; i < multiPartitions.Length; i++)
		{
			multiPartitions[i].SetActive(value: false);
		}
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
