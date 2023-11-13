using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GoldfishGameManager : SingletonCustom<GoldfishGameManager>
{
	public const float GAME_SECOND_TIME = 60f;
	public const string TAG_FISH = "Object";
	private const int TROPHY_GOLD_SCORE = 1200;
	private const int TROPHY_SILVER_SCORE = 900;
	private const int TROPHY_BRONZE_SCORE = 500;
	[SerializeField]
	private WinOrLoseResultManager winResultManager;
	[SerializeField]
	private RankingResultManager rankingResultManager;
	[SerializeField]
	private Camera camera;
	[SerializeField]
	private Transform tubRightTop;
	[SerializeField]
	private Transform tubLeftBottom;
	private bool isGameStart;
	private bool isGameEnd;
	private bool isGameTitleClose;
	private bool hasSecondGroup;
	private bool isNowSecondGroup;
	private bool isEightBattle;
	private float gameTime;
	private bool isTimeUpEnd;
	private int playerNum;
	private int charaNum;
	private int teamNum;
	private int[] scores;
	private int[] playerNoArray;
	private int[] teamNoArray;
	private bool isSkipCheck;
	private bool isSkipEnd;
	public bool IsGameStart => isGameStart;
	public bool IsGameEnd => isGameEnd;
	public bool IsGameNow
	{
		get
		{
			if (isGameStart)
			{
				return !isGameEnd;
			}
			return false;
		}
	}
	public bool IsGameTitleClose => isGameTitleClose;
	public bool HasSecondGroup => hasSecondGroup;
	public bool IsNowSecondGroup => isNowSecondGroup;
	public bool IsEightBattle => isEightBattle;
	public float GameTime => gameTime;
	public float RemainViewTime
	{
		get
		{
			if (!isGameStart)
			{
				return 60f;
			}
			if (isTimeUpEnd)
			{
				return 0f;
			}
			return 60f - GameTime;
		}
	}
	public int PlayerNum => playerNum;
	public int CharaNum => charaNum;
	public int TeamNum => teamNum;
	public void Init()
	{
		ResultGameDataParams.SetPoint();
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		teamNum = SingletonCustom<GameSettingManager>.Instance.TeamNum;
		List<int>[] playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		charaNum = ((playerGroupList[0].Count > 2) ? 8 : 4);
		isEightBattle = (charaNum == 8);
		playerNoArray = new int[charaNum];
		teamNoArray = new int[charaNum];
		if (isEightBattle)
		{
			for (int i = 0; i < charaNum; i++)
			{
				playerNoArray[i] = playerGroupList[i / 4][i % 4];
				teamNoArray[i] = i / 4;
			}
		}
		else if (teamNum == 2)
		{
			int num = 4;
			for (int j = 0; j < charaNum; j++)
			{
				if (j < playerNum)
				{
					playerNoArray[j] = j;
				}
				else
				{
					playerNoArray[j] = num;
					num++;
				}
				teamNoArray[j] = ((!playerGroupList[0].Contains(playerNoArray[j])) ? 1 : 0);
			}
		}
		else
		{
			for (int k = 0; k < charaNum; k++)
			{
				playerNoArray[k] = playerGroupList[k][0];
				teamNoArray[k] = k;
			}
		}
		DataInit();
		StartCoroutine(_OpenGameDirection());
	}
	public void SecondGroupInit()
	{
		DataInit();
		SingletonCustom<GoldfishUiManager>.Instance.Fade(1f, 0f, delegate
		{
			SingletonCustom<GoldfishCharacterManager>.Instance.SecondGroupInit();
			SingletonCustom<GoldfishUiManager>.Instance.SecondGroupInit();
		});
		StartCoroutine(_SecoundGroupDirection());
	}
	private void DataInit()
	{
		isGameStart = false;
		isGameEnd = false;
		gameTime = 0f;
		scores = new int[charaNum];
		isSkipCheck = false;
		isSkipEnd = false;
	}
	public void UpdateMethod()
	{
		if (!IsGameNow)
		{
			return;
		}
		gameTime += Time.deltaTime;
		if (gameTime > 60f)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
			isTimeUpEnd = true;
			GameEnd(1.5f);
		}
		else if (SingletonCustom<GoldfishCharacterManager>.Instance.CheckBreakEnd())
		{
			if (!isSkipCheck)
			{
				isSkipCheck = true;
				SingletonCustom<GoldfishUiManager>.Instance.ViewSkipControlInfo();
			}
			else if (!isSkipEnd && GoldfishController.GetSkipButtonDown(0))
			{
				isSkipEnd = true;
				SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
				SingletonCustom<GoldfishCharacterManager>.Instance.CpuFutureScoreCalc();
				GameEnd(2f);
			}
		}
	}
	public void AddScore(int _charaNo, int _addValue)
	{
		scores[_charaNo] += _addValue;
	}
	public void SetScore(int _charaNo, int _value)
	{
		scores[_charaNo] = _value;
	}
	public int GetScore(int _charaNo)
	{
		return scores[_charaNo];
	}
	public int[] GetScores()
	{
		return scores;
	}
	public int GetTeamScore(int _teamNo)
	{
		if (teamNum == 2)
		{
			int num = 0;
			for (int i = 0; i < charaNum; i++)
			{
				if (SingletonCustom<GoldfishCharacterManager>.Instance.GetChara(i).TeamNo == _teamNo)
				{
					num += scores[i];
				}
			}
			return num;
		}
		return scores[_teamNo];
	}
	public int[] GetTeamScores()
	{
		int[] array = new int[teamNum];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = GetTeamScore(i);
		}
		return array;
	}
	public int GetCharaPlayerNo(int _charaNo)
	{
		return playerNoArray[_charaNo];
	}
	public int GetCharaTeamNo(int _charaNo)
	{
		return teamNoArray[_charaNo];
	}
	public Vector3 GetTubRightTopPos(bool _isLocal = false)
	{
		if (_isLocal)
		{
			return tubRightTop.localPosition;
		}
		return tubRightTop.position;
	}
	public Vector3 GetTubLeftBottomPos(bool _isLocal = false)
	{
		if (_isLocal)
		{
			return tubLeftBottom.localPosition;
		}
		return tubLeftBottom.position;
	}
	public Camera GetCamera()
	{
		return camera;
	}
	private IEnumerator _OpenGameDirection()
	{
		yield return null;
		SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate
		{
			isGameTitleClose = true;
			SingletonCustom<CommonStartSimple>.Instance.transform.SetLocalPositionY(0f);
			if (playerNum == 1)
			{
				LeanTween.delayedCall(1f, (Action)delegate
				{
					SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
				});
			}
			else
			{
				LeanTween.delayedCall(1f, (Action)delegate
				{
					SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
				});
			}
			SingletonCustom<GoldfishUiManager>.Instance.ViewFirstControlInfo();
		});
		float timer = 0f;
		while (timer < 0.5f)
		{
			timer += Time.deltaTime;
			if (!isGameTitleClose)
			{
				Time.timeScale = 0.9f;
			}
			yield return null;
		}
		if (!isGameTitleClose)
		{
			Time.timeScale = 0f;
		}
	}
	private IEnumerator _SecoundGroupDirection()
	{
		yield return new WaitForSeconds(2f);
		SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
	}
	public void GameStart()
	{
		isGameStart = true;
	}
	public void GameEnd(float _resultDelayTime)
	{
		if (!isGameEnd)
		{
			isGameEnd = true;
			StartCoroutine(_GameEnd(_resultDelayTime));
		}
	}
	private IEnumerator _GameEnd(float _resultDelayTime)
	{
		yield return new WaitForSeconds(_resultDelayTime);
		int[] array = new int[scores.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = scores[i];
		}
		if ((SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP)
		{
			for (int j = 0; j < teamNum; j++)
			{
				ResultGameDataParams.SetRecord_WinOrLose(GetTeamScore(j), j);
			}
		}
		else if (isEightBattle)
		{
			ResultGameDataParams.SetRecord_Int_Ranking8Numbers(array, SingletonCustom<GoldfishCharacterManager>.Instance.GetPlayerNoArray());
		}
		else
		{
			ResultGameDataParams.SetRecord_Int(array, SingletonCustom<GoldfishCharacterManager>.Instance.GetPlayerNoArray(), !isNowSecondGroup);
		}
		TrophySet();
		if ((playerNum == 2 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP)
		{
			int teamScore = GetTeamScore(0);
			int teamScore2 = GetTeamScore(1);
			if (teamScore > teamScore2)
			{
				winResultManager.ShowResult(ResultGameDataParams.ResultType.Win, 0);
			}
			else if (teamScore < teamScore2)
			{
				if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP)
				{
					winResultManager.ShowResult(ResultGameDataParams.ResultType.Win, 1);
				}
				else
				{
					winResultManager.ShowResult(ResultGameDataParams.ResultType.Lose, 0);
				}
			}
			else
			{
				winResultManager.ShowResult(ResultGameDataParams.ResultType.Draw, 0);
			}
		}
		else
		{
			rankingResultManager.ShowResult_Score();
		}
		LeanTween.delayedCall(1.5f, (Action)delegate
		{
			EndCamera();
		});
	}
	private void TrophySet()
	{
	}
	private void EndCamera()
	{
		camera.enabled = false;
	}
	private void DebugEnd()
	{
		for (int i = 0; i < scores.Length; i++)
		{
			scores[i] = UnityEngine.Random.Range(10, 80) * 100;
		}
		GameEnd(1.5f);
	}
}
