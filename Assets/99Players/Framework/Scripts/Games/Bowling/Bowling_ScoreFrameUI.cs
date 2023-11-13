using System;
using UnityEngine;
public class Bowling_ScoreFrameUI : MonoBehaviour
{
	[Serializable]
	public struct FrameUIData
	{
		[Header("一投目のスコア画像")]
		public SpriteRenderer firstThrowScoreSp;
		[Header("二投目のスコア画像")]
		public SpriteRenderer secondThrowScoreSp;
		[Header("フレ\u30fcムの合計スコア")]
		public SpriteNumbers totalScoreNubers;
		[Header("スプリット画像")]
		public SpriteRenderer splitSp;
		public void Init()
		{
			firstThrowScoreSp.SetAlpha(0f);
			secondThrowScoreSp.SetAlpha(0f);
			totalScoreNubers.SetAlpha(0f);
			splitSp.SetAlpha(0f);
		}
	}
	[Serializable]
	public struct LastFrameUIData
	{
		[Header("一投目のスコア画像")]
		public SpriteRenderer firstThrowScoreSp;
		[Header("二投目のスコア画像")]
		public SpriteRenderer secondThrowScoreSp;
		[Header("三投目のスコア画像")]
		public SpriteRenderer thirdThrowScoreSp;
		[Header("フレ\u30fcムの合計スコア")]
		public SpriteNumbers totalScoreNubers;
		[NonReorderable]
		[Header("スプリット画像")]
		public SpriteRenderer[] splitSp;
		public void Init()
		{
			firstThrowScoreSp.SetAlpha(0f);
			secondThrowScoreSp.SetAlpha(0f);
			thirdThrowScoreSp.SetAlpha(0f);
			totalScoreNubers.SetAlpha(0f);
			for (int i = 0; i < splitSp.Length; i++)
			{
				splitSp[i].SetAlpha(0f);
			}
		}
	}
	[SerializeField]
	[Header("チ\u30fcム種類の親オブジェクト")]
	private GameObject teamTypeRootObj;
	[SerializeField]
	[Header("チ\u30fcム種類の画像")]
	private SpriteRenderer teamTypeSp;
	[SerializeField]
	[Header("チ\u30fcム種類の背景色画像")]
	private SpriteRenderer teamTypeBackColorSp;
	[SerializeField]
	[Header("第１フレ\u30fcムのUIデ\u30fcタ")]
	private FrameUIData firstFrameUIData;
	[SerializeField]
	[Header("第２フレ\u30fcムのUIデ\u30fcタ")]
	private FrameUIData secondFrameUIData;
	[SerializeField]
	[Header("最終フレ\u30fcムのUIデ\u30fcタ")]
	private LastFrameUIData lastFrameUIData;
	[SerializeField]
	[Header("スコアの合計点")]
	private SpriteNumbers totalScoreNumbers;
	[SerializeField]
	[Header("キャラアイコン親アンカ\u30fc")]
	private Transform charaIconRootAnchor;
	[SerializeField]
	[Header("キャラアイコン画像")]
	private SpriteRenderer characterIconSp;
	[SerializeField]
	[Header("キャラアイコン背景画像")]
	private SpriteRenderer characterIconBack;
	[SerializeField]
	[Header("フレ\u30fcムの演出用背景色画像")]
	private SpriteRenderer animFrameBackColorSp;
	private Bowling_Define.UserType userType;
	private int totalScore;
	public Bowling_Define.UserType UserType => userType;
	public void Init(Bowling_Define.UserType _userType)
	{
		userType = _userType;
		SetCharacterIcon();
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP)
		{
			teamTypeRootObj.SetActive(value: true);
			if (Bowling_Define.MPM.GetUserTeamType(userType) == Bowling_Define.TeamType.TEAM_A)
			{
				teamTypeSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Bowling, "a_team");
				teamTypeBackColorSp.color = Bowling_Define.TEAM_COLOR[0];
			}
			else
			{
				teamTypeSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Bowling, "b_team");
				teamTypeBackColorSp.color = Bowling_Define.TEAM_COLOR[1];
			}
			charaIconRootAnchor.SetLocalPositionX(-295f);
		}
		else
		{
			teamTypeRootObj.SetActive(value: false);
			charaIconRootAnchor.SetLocalPositionX(-240f);
		}
		firstFrameUIData.Init();
		secondFrameUIData.Init();
		lastFrameUIData.Init();
		totalScoreNumbers.Set(0);
	}
	public void SetScore(Bowling_GameUIManager.ScoreData _scoreData, int _frameNo)
	{
		Bowling_GameUIManager.FrameData data = _scoreData.frameList[_frameNo];
		switch (_frameNo)
		{
		case 0:
			if (Bowling_Define.MPM.NowThrowCount == 0)
			{
				SetFrameScoreData(data, firstFrameUIData.firstThrowScoreSp, _isFirstThrow: true);
				if (data.IsSplit(0))
				{
					firstFrameUIData.splitSp.SetAlpha(1f);
				}
			}
			else
			{
				SetFrameScoreData(data, firstFrameUIData.secondThrowScoreSp, _isFirstThrow: false);
			}
			break;
		case 1:
			if (Bowling_Define.MPM.NowThrowCount == 0)
			{
				SetFrameScoreData(data, secondFrameUIData.firstThrowScoreSp, _isFirstThrow: true);
				if (data.IsSplit(0))
				{
					secondFrameUIData.splitSp.SetAlpha(1f);
				}
			}
			else
			{
				SetFrameScoreData(data, secondFrameUIData.secondThrowScoreSp, _isFirstThrow: false);
			}
			break;
		case 2:
			if (Bowling_Define.MPM.NowThrowCount == 0)
			{
				SetFrameScoreData(data, lastFrameUIData.firstThrowScoreSp, _isFirstThrow: true);
				if (data.IsSplit(0))
				{
					lastFrameUIData.splitSp[0].SetAlpha(1f);
				}
			}
			else if (Bowling_Define.MPM.NowThrowCount == 1)
			{
				SetFrameScoreData(data, lastFrameUIData.secondThrowScoreSp, _isFirstThrow: false);
				if (data.IsSplit(1))
				{
					lastFrameUIData.splitSp[1].SetAlpha(1f);
				}
			}
			else
			{
				SetFrameScoreData(data, lastFrameUIData.thirdThrowScoreSp, _isFirstThrow: true);
				if (data.IsSplit(2))
				{
					lastFrameUIData.splitSp[2].SetAlpha(1f);
				}
			}
			break;
		}
		for (int i = 0; i < _scoreData.frameList.Length; i++)
		{
			if (_scoreData.frameList[i].IsSetTotalScore())
			{
				switch (i)
				{
				case 0:
					firstFrameUIData.totalScoreNubers.Set(_scoreData.frameList[i].GetTotalScore());
					firstFrameUIData.totalScoreNubers.SetAlpha(1f);
					SetTotalScore(_scoreData.frameList[i].totalScore);
					break;
				case 1:
					secondFrameUIData.totalScoreNubers.Set(_scoreData.frameList[i].GetTotalScore());
					secondFrameUIData.totalScoreNubers.SetAlpha(1f);
					SetTotalScore(_scoreData.frameList[i].totalScore);
					break;
				case 2:
					lastFrameUIData.totalScoreNubers.Set(_scoreData.frameList[i].GetTotalScore());
					lastFrameUIData.totalScoreNubers.SetAlpha(1f);
					SetTotalScore(_scoreData.frameList[i].totalScore);
					break;
				}
			}
		}
	}
	private void SetFrameScoreData(Bowling_GameUIManager.FrameData _data, SpriteRenderer _scoreSp, bool _isFirstThrow)
	{
		_scoreSp.SetAlpha(1f);
		if (_data.IsStrike(Bowling_Define.MPM.NowThrowCount))
		{
			_scoreSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Bowling, "tex_strike_mark");
			_scoreSp.transform.SetLocalScale(0.51f, 0.61f, 0.51f);
		}
		else if (_data.IsSpare(Bowling_Define.MPM.NowThrowCount))
		{
			_scoreSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Bowling, "tex_spare_mark");
			_scoreSp.transform.SetLocalScale(0.51f, 0.51f, 0.51f);
		}
		else if (_data.IsGutter(Bowling_Define.MPM.NowThrowCount) && _isFirstThrow)
		{
			_scoreSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Bowling, "tex_gutter_mark");
			_scoreSp.transform.SetLocalScale(0.5f, 0.5f, 0.5f);
		}
		else if (_data.IsGutter(Bowling_Define.MPM.NowThrowCount) && !_isFirstThrow)
		{
			_scoreSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Bowling, "tex_miss_mark");
			_scoreSp.transform.SetLocalScale(0.5f, 0.5f, 0.5f);
		}
		else if (_data.GetFallNum(Bowling_Define.MPM.NowThrowCount) > 0)
		{
			_scoreSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_number_a_" + _data.GetFallNum(Bowling_Define.MPM.NowThrowCount).ToString());
		}
		else if (_data.IsGutter(Bowling_Define.MPM.NowThrowCount) && _data.GetFallNum(Bowling_Define.MPM.NowThrowCount) == 0)
		{
			_scoreSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Bowling, "tex_gutter_mark");
			_scoreSp.transform.SetLocalScale(0.5f, 0.5f, 0.5f);
		}
		else if (_data.GetFallNum(Bowling_Define.MPM.NowThrowCount) == 0)
		{
			_scoreSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Bowling, "tex_miss_mark");
			_scoreSp.transform.SetLocalScale(0.5f, 0.5f, 0.5f);
		}
	}
	public void SetTotalScore(int _score)
	{
		totalScoreNumbers.Set(_score);
		totalScore = _score;
	}
	public int GetTotalScore()
	{
		return totalScore;
	}
	public void StartFrameFlashing()
	{
		LeanTween.cancel(animFrameBackColorSp.gameObject);
		LeanTween.value(animFrameBackColorSp.gameObject, 200f, 50f, 1f).setLoopPingPong().setOnUpdate(delegate(float value)
		{
			animFrameBackColorSp.color = new Color32(byte.MaxValue, byte.MaxValue, (byte)value, byte.MaxValue);
		});
	}
	public void StopFrameFlashing()
	{
		animFrameBackColorSp.color = Color.white;
		LeanTween.cancel(animFrameBackColorSp.gameObject);
	}
	private void SetCharacterIcon()
	{
		switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType])
		{
		case 0:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_yuto_0" + 2.ToString());
			break;
		case 1:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_hina_0" + 2.ToString());
			break;
		case 2:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_ituki_0" + 2.ToString());
			break;
		case 3:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_souta_0" + 2.ToString());
			break;
		case 4:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_takumi_0" + 2.ToString());
			break;
		case 5:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rin_0" + 2.ToString());
			break;
		case 6:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_akira_0" + 2.ToString());
			break;
		case 7:
			characterIconSp.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rui_0" + 2.ToString());
			break;
		}
		characterIconBack.color = Bowling_Define.GetUserColor(userType);
	}
}
