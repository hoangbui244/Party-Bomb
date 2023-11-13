using System;
using UnityEngine;
public class Bowling_GameUIManager : SingletonCustom<Bowling_GameUIManager>
{
	[Serializable]
	public struct FrameData
	{
		public int[] fallNum;
		public bool[,] isFallPins;
		public bool[] isSpare;
		public bool[] isStrike;
		public bool[] isSplit;
		public bool[] isGutter;
		public int totalScore;
		public void Init(int _throwNum)
		{
			fallNum = new int[_throwNum];
			for (int i = 0; i < fallNum.Length; i++)
			{
				fallNum[i] = -1;
			}
			isSpare = new bool[_throwNum];
			isStrike = new bool[_throwNum];
			isSplit = new bool[_throwNum];
			isGutter = new bool[_throwNum];
			isFallPins = new bool[_throwNum, 10];
			totalScore = -1;
		}
		public void SetData(int _throwNo, bool[] _isPinStandList, bool _isGutter, bool _lastFrame)
		{
			fallNum[_throwNo] = 0;
			for (int i = 0; i < _isPinStandList.Length; i++)
			{
				if (!_isPinStandList[i])
				{
					fallNum[_throwNo]++;
					isFallPins[_throwNo, i] = true;
				}
			}
			isGutter[_throwNo] = _isGutter;
			if (_lastFrame)
			{
				switch (_throwNo)
				{
				case 0:
					if (fallNum[_throwNo] == 10)
					{
						isStrike[0] = true;
					}
					else
					{
						isSplit[0] = IsSplit(_isPinStandList);
					}
					break;
				case 1:
					if (isStrike[0])
					{
						if (fallNum[_throwNo] == 10)
						{
							isStrike[1] = true;
						}
						else
						{
							isSplit[1] = IsSplit(_isPinStandList);
						}
						break;
					}
					if (fallNum[_throwNo] == 10)
					{
						isSpare[1] = true;
					}
					fallNum[1] -= fallNum[0];
					for (int k = 0; k < isFallPins.GetLength(1); k++)
					{
						if (isFallPins[0, k])
						{
							isFallPins[1, k] = false;
						}
					}
					break;
				case 2:
					if (isStrike[1] || isSpare[1])
					{
						if (fallNum[_throwNo] == 10)
						{
							isStrike[2] = true;
						}
						else
						{
							isSplit[2] = IsSplit(_isPinStandList);
						}
						break;
					}
					if (fallNum[_throwNo] == 10)
					{
						isSpare[2] = true;
					}
					fallNum[2] -= fallNum[1];
					for (int j = 0; j < isFallPins.GetLength(1); j++)
					{
						if (isFallPins[1, j])
						{
							isFallPins[2, j] = false;
						}
					}
					break;
				}
				return;
			}
			if (fallNum[_throwNo] == 10)
			{
				if (_throwNo == 0)
				{
					isStrike[0] = true;
				}
				else if (!isStrike[0])
				{
					isSpare[1] = true;
				}
			}
			if (_throwNo == 1)
			{
				fallNum[1] -= fallNum[0];
				for (int l = 0; l < isFallPins.GetLength(1); l++)
				{
					if (isFallPins[0, l])
					{
						isFallPins[1, l] = false;
					}
				}
			}
			else
			{
				isSplit[0] = IsSplit(_isPinStandList);
			}
		}
		public int GetTotalScore()
		{
			return totalScore;
		}
		public int GetScore(int _throwNo = -1)
		{
			if (_throwNo == -1)
			{
				int num = 0;
				for (int i = 0; i < fallNum.Length; i++)
				{
					if (fallNum[i] > 0)
					{
						num += fallNum[i];
					}
				}
				return num;
			}
			return fallNum[_throwNo];
		}
		private bool IsSplit(bool[] _pins)
		{
			if (!_pins[0])
			{
				if (_pins[1])
				{
					if (_pins[2] || _pins[5] || _pins[9])
					{
						return true;
					}
					if (_pins[6] && !_pins[3])
					{
						return true;
					}
					if (_pins[8] && !_pins[4])
					{
						return true;
					}
				}
				else if (_pins[2])
				{
					if (_pins[3] || _pins[6])
					{
						return true;
					}
					if (_pins[7] && !_pins[4])
					{
						return true;
					}
					if (_pins[9] && !_pins[5])
					{
						return true;
					}
				}
				else if (_pins[3])
				{
					if (_pins[4] || _pins[5] || _pins[8] || _pins[9])
					{
						return true;
					}
				}
				else if (_pins[4])
				{
					if (_pins[5] || _pins[6] || _pins[9])
					{
						return true;
					}
				}
				else if (_pins[5])
				{
					if (_pins[6] || _pins[7])
					{
						return true;
					}
				}
				else if (_pins[6])
				{
					if (_pins[7] || _pins[8] || _pins[9])
					{
						return true;
					}
				}
				else if (_pins[7])
				{
					if (_pins[8] || _pins[9])
					{
						return true;
					}
				}
				else if (_pins[8] && _pins[9])
				{
					return true;
				}
			}
			return false;
		}
		public void SetTotalScore(int _score)
		{
			totalScore = _score;
		}
		public int GetFallNum(int _throwCnt)
		{
			return fallNum[_throwCnt];
		}
		public bool IsSpare(int _throwCnt)
		{
			return isSpare[_throwCnt];
		}
		public bool IsStrike(int _throwCnt)
		{
			return isStrike[_throwCnt];
		}
		public bool IsGutter(int _throwCnt)
		{
			return isGutter[_throwCnt];
		}
		public bool IsSplit(int _throwCnt)
		{
			return isSplit[_throwCnt];
		}
		public bool IsSetTotalScore()
		{
			return totalScore >= 0;
		}
	}
	[Serializable]
	public struct ScoreData
	{
		public Bowling_Define.UserType userType;
		public FrameData[] frameList;
		public int continuousStrikeNum;
		public void Init(Bowling_Define.UserType _userType, int _frameNum)
		{
			userType = _userType;
			frameList = new FrameData[_frameNum];
			for (int i = 0; i < frameList.Length; i++)
			{
				frameList[i].Init((i == frameList.Length - 1) ? 3 : 2);
			}
			continuousStrikeNum = -1;
		}
		public void SetData(int _frameNo, int _throwNo, bool[] _isPinStandList, bool _isGutter)
		{
			frameList[_frameNo].SetData(_throwNo, _isPinStandList, _isGutter, _frameNo == frameList.Length - 1);
			if (frameList[_frameNo].IsStrike(_throwNo))
			{
				continuousStrikeNum++;
			}
			else
			{
				continuousStrikeNum = -1;
			}
		}
		public int GetContinuousStrikeNum()
		{
			return continuousStrikeNum;
		}
	}
	[SerializeField]
	[Header("投球UI")]
	private Bowling_BallThrowUI throwUI;
	[SerializeField]
	[Header("残ピン表示のピンの丸画像")]
	private SpriteRenderer[] pins;
	[SerializeField]
	[Header("スコアボ\u30fcドUI")]
	private Bowling_ScoreBoardUI scoreBoardUI;
	[SerializeField]
	[Header("投球結果エフェクト：ストライク")]
	private ThrowResultEffect strikeEffect;
	[SerializeField]
	[Header("投球結果エフェクト：スペア")]
	private ThrowResultEffect spareEffect;
	[SerializeField]
	[Header("投球結果エフェクト：ガタ\u30fc")]
	private ThrowResultEffect gutterEffect;
	[SerializeField]
	[Header("投球結果エフェクト：ミス")]
	private ThrowResultEffect missEffect;
	[SerializeField]
	[Header("投球結果エフェクト：スプリット")]
	private ThrowResultEffect splitEffect;
	[SerializeField]
	[Header("投球結果エフェクト：パ\u30fcフェクト")]
	private ThrowResultEffect perfectEffect;
	[SerializeField]
	[Header("「～の番です」のカットインUI")]
	private TurnCutIn turnCutIn;
	[SerializeField]
	[Header("ひとり時の操作UIオブジェクト")]
	private GameObject singleOperaionObj;
	[SerializeField]
	[Header("複数人時の操作UIオブジェクト")]
	private GameObject multiOperaionObj;
	[SerializeField]
	[Header("ポ\u30fcズUIオブジェクト")]
	private GameObject pauseUIObj;
	private ScoreData[] userScoreData;
	[SerializeField]
	[Header("画面フェ\u30fcド画像")]
	private SpriteRenderer screenFade;
	private readonly float SCREEN_FADE_TIME = 0.25f;
	public void Init()
	{
		throwUI.Init();
		InitPinData();
		userScoreData = new ScoreData[Bowling_Define.MEMBER_NUM];
		for (int i = 0; i < userScoreData.Length; i++)
		{
			userScoreData[i].Init(Bowling_Define.MPM.MemberOrder[i], Bowling_Define.PLAY_FRAME_NUM);
		}
		if (Bowling_Define.PLAYER_NUM >= 2)
		{
			singleOperaionObj.SetActive(value: false);
			multiOperaionObj.SetActive(value: true);
			pauseUIObj.SetActive(value: false);
		}
		else
		{
			singleOperaionObj.SetActive(value: true);
			multiOperaionObj.SetActive(value: false);
			pauseUIObj.SetActive(value: true);
		}
		scoreBoardUI.Init();
	}
	public void UpdateMethod()
	{
		throwUI.UpdateMethod();
	}
	public void SetScore(Bowling_Define.UserType _userType, int _frameNo, int _throwNo, bool[] _isPinStandList, bool _isGutter)
	{
		ScoreData scoreData = GetScoreData(_userType);
		scoreData.SetData(_frameNo, _throwNo, _isPinStandList, _isGutter);
		for (int i = 0; i < Bowling_Define.PLAY_FRAME_NUM && i <= _frameNo; i++)
		{
			if (scoreData.frameList[i].IsSetTotalScore())
			{
				continue;
			}
			int num = 0;
			if (i == Bowling_Define.PLAY_FRAME_NUM - 1)
			{
				if (IsCanThreeThrow(_userType))
				{
					if (scoreData.frameList[i].GetScore(2) >= 0)
					{
						num = scoreData.frameList[i - 1].GetTotalScore() + scoreData.frameList[i].GetScore();
						scoreData.frameList[i].SetTotalScore(num);
					}
				}
				else if (scoreData.frameList[i].GetScore(1) >= 0)
				{
					num = scoreData.frameList[i - 1].GetTotalScore() + scoreData.frameList[i].GetScore();
					scoreData.frameList[i].SetTotalScore(num);
				}
			}
			else if (i == Bowling_Define.PLAY_FRAME_NUM - 2 && scoreData.frameList[i].IsStrike(0))
			{
				int num2 = 0;
				int num3 = i + 1;
				int num4 = 0;
				int num5 = 0;
				while (num5 < 2 && scoreData.frameList[num3].GetScore(num4) >= 0)
				{
					num2 += scoreData.frameList[num3].GetScore(num4);
					num4++;
					num5++;
					if (num5 == 2)
					{
						num = ((i != 0) ? (scoreData.frameList[i - 1].GetTotalScore() + scoreData.frameList[i].GetScore() + num2) : (scoreData.frameList[i].GetScore() + num2));
						scoreData.frameList[i].SetTotalScore(num);
					}
				}
			}
			else if (scoreData.frameList[i].IsSpare(1))
			{
				if (scoreData.frameList[i + 1].GetScore(0) >= 0)
				{
					num = ((i != 0) ? (scoreData.frameList[i - 1].GetTotalScore() + scoreData.frameList[i].GetScore() + scoreData.frameList[i + 1].GetScore(0)) : (scoreData.frameList[i].GetScore() + scoreData.frameList[i + 1].GetScore(0)));
					scoreData.frameList[i].SetTotalScore(num);
				}
			}
			else if (scoreData.frameList[i].IsStrike(0))
			{
				int num6 = 0;
				int num7 = i + 1;
				int num8 = 0;
				int num9 = 0;
				while (num9 < 2 && scoreData.frameList[num7].GetScore(num8) >= 0)
				{
					if (scoreData.frameList[num7].IsStrike(0))
					{
						num9++;
						if (num7 != Bowling_Define.PLAY_FRAME_NUM - 1)
						{
							num7++;
						}
						num6 += 10;
					}
					else
					{
						num6 += scoreData.frameList[num7].GetScore(num8);
						num8++;
						num9++;
					}
					if (num9 == 2)
					{
						num = ((i != 0) ? (scoreData.frameList[i - 1].GetTotalScore() + scoreData.frameList[i].GetScore() + num6) : (scoreData.frameList[i].GetScore() + num6));
						scoreData.frameList[i].SetTotalScore(num);
					}
				}
			}
			else if (_throwNo == 1)
			{
				if (i == 0)
				{
					scoreData.frameList[i].SetTotalScore(scoreData.frameList[i].GetScore());
				}
				else if (scoreData.frameList[i - 1].IsSetTotalScore())
				{
					num = scoreData.frameList[i - 1].GetTotalScore() + scoreData.frameList[i].GetScore();
					scoreData.frameList[i].SetTotalScore(num);
				}
			}
		}
		scoreBoardUI.SetData(scoreData, _frameNo, _userType);
		for (int j = 0; j < userScoreData.Length; j++)
		{
			if (userScoreData[j].userType == _userType)
			{
				userScoreData[j] = scoreData;
			}
		}
	}
	public Bowling_BallThrowUI GetBallThrowUI()
	{
		return throwUI;
	}
	public ScoreData GetScoreData(Bowling_Define.UserType _userType)
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
	public void SetPinData(bool[] _isPinStandList)
	{
		for (int i = 0; i < pins.Length; i++)
		{
			Color color = (!_isPinStandList[i]) ? (Color.white * 0.5f) : Color.white;
			color.a = 1f;
			pins[i].color = color;
		}
	}
	public void InitPinData()
	{
		for (int i = 0; i < pins.Length; i++)
		{
			Color white = Color.white;
			white.a = 1f;
			pins[i].color = white;
		}
	}
	public bool IsCanThreeThrow(Bowling_Define.UserType _userType)
	{
		if (GetScoreData(_userType).frameList[Bowling_Define.PLAY_FRAME_NUM - 1].IsStrike(0) || GetScoreData(_userType).frameList[Bowling_Define.PLAY_FRAME_NUM - 1].IsSpare(1))
		{
			return true;
		}
		return false;
	}
	public int GetTotalScore(Bowling_Define.UserType _userType)
	{
		return scoreBoardUI.GetTotalScore(_userType);
	}
	public void StartFrameFlashing(Bowling_Define.UserType _userType)
	{
		scoreBoardUI.StartFrameFlashing(_userType);
	}
	public float PlayThrowResultEffect()
	{
		ScoreData scoreData = default(ScoreData);
		for (int i = 0; i < userScoreData.Length; i++)
		{
			if (userScoreData[i].userType == Bowling_Define.MPM.NowThrowUserType)
			{
				scoreData = userScoreData[i];
				break;
			}
		}
		if (scoreData.frameList[Bowling_Define.MPM.NowFrameNo].IsGutter(Bowling_Define.MPM.NowThrowCount))
		{
			gutterEffect.StartEffect();
			SingletonCustom<AudioManager>.Instance.SePlay("se_discouragement");
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_bowling_gutter");
			return gutterEffect.GetEffectTime();
		}
		if (scoreData.frameList[Bowling_Define.MPM.NowFrameNo].IsStrike(Bowling_Define.MPM.NowThrowCount))
		{
			strikeEffect.StartEffect();
			SingletonCustom<AudioManager>.Instance.SePlay("se_cheer");
			return strikeEffect.GetEffectTime();
		}
		if (scoreData.frameList[Bowling_Define.MPM.NowFrameNo].IsSpare(Bowling_Define.MPM.NowThrowCount))
		{
			spareEffect.StartEffect();
			SingletonCustom<AudioManager>.Instance.SePlay("se_applause_1");
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_bowling_spare");
			return spareEffect.GetEffectTime();
		}
		if (scoreData.frameList[Bowling_Define.MPM.NowFrameNo].IsSplit(Bowling_Define.MPM.NowThrowCount) && Bowling_Define.MPM.NowThrowCount != 2)
		{
			splitEffect.StartEffect();
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_bowling_split");
			return splitEffect.GetEffectTime();
		}
		if (scoreData.frameList[Bowling_Define.MPM.NowFrameNo].GetFallNum(Bowling_Define.MPM.NowThrowCount) == 0)
		{
			missEffect.StartEffect();
			SingletonCustom<AudioManager>.Instance.SePlay("se_discouragement");
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_bowling_miss");
			return missEffect.GetEffectTime();
		}
		return 0.5f;
	}
	public float PlayPerfectEffect()
	{
		perfectEffect.StartEffect();
		return perfectEffect.GetEffectTime();
	}
	public float GetThrowResultEffectTime(Bowling_Define.ThrowResultEffectType _type)
	{
		switch (_type)
		{
		case Bowling_Define.ThrowResultEffectType.Strike:
			return strikeEffect.GetEffectTime();
		case Bowling_Define.ThrowResultEffectType.Spare:
			return spareEffect.GetEffectTime();
		case Bowling_Define.ThrowResultEffectType.Gutter:
			return gutterEffect.GetEffectTime();
		case Bowling_Define.ThrowResultEffectType.Miss:
			return missEffect.GetEffectTime();
		default:
			return 0f;
		}
	}
	public void PlayTurnCutIn(Bowling_Define.UserType _userType)
	{
		turnCutIn.ShowTurnCutIn((int)_userType, 0f);
	}
	public void SetDebugRecord()
	{
		scoreBoardUI.SetDebugRecord();
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
