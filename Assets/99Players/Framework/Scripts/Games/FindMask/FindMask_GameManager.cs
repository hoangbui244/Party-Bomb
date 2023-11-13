using System;
using System.Collections;
using UnityEngine;
public class FindMask_GameManager : SingletonCustom<FindMask_GameManager>
{
	public enum STATE
	{
		NONE,
		CHARA_MOVE,
		CHARA_FINDMASK,
		SELECT_FAILD,
		FIND_ALL
	}
	private float GAME_SECOND_TIME = 60f;
	private int CHARA_NUM = 4;
	[SerializeField]
	[Header("協力リザルト")]
	private WinOrLoseResultManager winResultManager;
	[SerializeField]
	[Header("順位リザルト")]
	private RankingResultManager rankingResultManager;
	[SerializeField]
	private Camera camera;
	private bool isGameStart;
	private bool isGameEnd;
	private bool isEightPlsyers;
	private float gameTime;
	private int playerNum;
	private int currentTurnNum;
	private int turnPlayerNo;
	private int[] arrayTurnPlayer;
	private int[] arrayPlayerElement;
	private STATE state;
	private readonly float PLAY_FIND_TIME = 10f;
	private float findTime;
	private bool isDirectingEnd;
	private readonly float SECOND_GROUP_INTERVAL = 2f;
	private readonly float RESULT_INTERVAL = 2.5f;
	public bool IsGameStart => isGameStart;
	public bool IsGameEnd => isGameEnd;
	public bool IsEightPlsyers => isEightPlsyers;
	public float GameTime => gameTime;
	public int PlayerNum => playerNum;
	public int CurrentTurnNum => currentTurnNum;
	public int TurnPlayerNo => turnPlayerNo;
	public int[] ArrayTurnPlayer => arrayTurnPlayer;
	public int[] ArrayPlayerElement => arrayPlayerElement;
	public STATE State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
		}
	}
	public float PlayFindTime => PLAY_FIND_TIME;
	public float FindTime => findTime;
	public void Init()
	{
		ResultGameDataParams.SetPoint();
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum >= 3 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			isEightPlsyers = true;
		}
		if (IsEightPlsyers)
		{
			arrayTurnPlayer = new int[CHARA_NUM * 2];
			arrayPlayerElement = new int[CHARA_NUM * 2];
		}
		else
		{
			arrayTurnPlayer = new int[CHARA_NUM];
			arrayPlayerElement = new int[CHARA_NUM];
		}
		switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat)
		{
		case GS_Define.GameFormat.BATTLE:
			for (int num9 = 0; num9 < arrayTurnPlayer.Length; num9++)
			{
				arrayTurnPlayer[num9] = ((num9 < SingletonCustom<GameSettingManager>.Instance.PlayerNum) ? SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[num9][0] : (4 + (num9 - SingletonCustom<GameSettingManager>.Instance.PlayerNum)));
			}
			Shuffle(arrayTurnPlayer);
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum < 3 && arrayTurnPlayer[0] >= 4)
			{
				for (int num10 = 1; num10 < arrayTurnPlayer.Length; num10++)
				{
					if (arrayTurnPlayer[num10] < 4)
					{
						int num11 = arrayTurnPlayer[num10];
						arrayTurnPlayer[num10] = arrayTurnPlayer[0];
						arrayTurnPlayer[0] = num11;
						break;
					}
				}
			}
			SetPlayerElementArray(arrayTurnPlayer);
			break;
		case GS_Define.GameFormat.COOP:
			for (int m = 0; m < arrayTurnPlayer.Length; m++)
			{
				arrayTurnPlayer[m] = ((m < SingletonCustom<GameSettingManager>.Instance.PlayerNum) ? SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][m] : (4 + (m - SingletonCustom<GameSettingManager>.Instance.PlayerNum)));
			}
			Shuffle(arrayTurnPlayer);
			for (int n = 0; n < arrayTurnPlayer.Length - 1; n++)
			{
				if (n % 2 == 0)
				{
					if (arrayTurnPlayer[n] >= 4)
					{
						for (int num3 = n + 1; num3 < arrayTurnPlayer.Length; num3++)
						{
							if (arrayTurnPlayer[num3] < 4)
							{
								int num4 = arrayTurnPlayer[num3];
								arrayTurnPlayer[num3] = arrayTurnPlayer[n];
								arrayTurnPlayer[n] = num4;
								break;
							}
						}
					}
					if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat != GS_Define.GameFormat.COOP || SingletonCustom<GameSettingManager>.Instance.PlayerNum != 3 || n != arrayTurnPlayer.Length - 2 || arrayTurnPlayer[n] == 4)
					{
						continue;
					}
					for (int num5 = 0; num5 < arrayTurnPlayer.Length; num5++)
					{
						if (arrayTurnPlayer[num5] == 4)
						{
							int num6 = arrayTurnPlayer[num5];
							arrayTurnPlayer[num5] = arrayTurnPlayer[n];
							arrayTurnPlayer[n] = num6;
							break;
						}
					}
				}
				else
				{
					if (arrayTurnPlayer[n] >= 4)
					{
						continue;
					}
					for (int num7 = n + 1; num7 < arrayTurnPlayer.Length; num7++)
					{
						if (arrayTurnPlayer[num7] >= 4)
						{
							int num8 = arrayTurnPlayer[num7];
							arrayTurnPlayer[num7] = arrayTurnPlayer[n];
							arrayTurnPlayer[n] = num8;
							break;
						}
					}
				}
			}
			SetPlayerElementArray(arrayTurnPlayer);
			break;
		case GS_Define.GameFormat.BATTLE_AND_COOP:
			for (int i = 0; i < arrayTurnPlayer.Length; i++)
			{
				arrayTurnPlayer[i] = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i % 2][i / 2];
			}
			for (int j = 0; j < arrayTurnPlayer.Length - 1; j++)
			{
				for (int k = j + 1; k < arrayTurnPlayer.Length; k++)
				{
					if (j % 2 == k % 2 && UnityEngine.Random.Range(0, 2) == 0)
					{
						int num = arrayTurnPlayer[k];
						arrayTurnPlayer[k] = arrayTurnPlayer[j];
						arrayTurnPlayer[j] = num;
						break;
					}
				}
			}
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				for (int l = 0; l < arrayTurnPlayer.Length - 1; l++)
				{
					if (l % 2 == 0)
					{
						int num2 = arrayTurnPlayer[l];
						arrayTurnPlayer[l] = arrayTurnPlayer[l + 1];
						arrayTurnPlayer[l + 1] = num2;
					}
				}
			}
			SetPlayerElementArray(arrayTurnPlayer);
			break;
		}
		DataInit();
		turnPlayerNo = arrayTurnPlayer[currentTurnNum];
		StartCoroutine(_OpenGameDirection());
	}
	public void SecondGroupInit()
	{
	}
	private void DataInit()
	{
		state = STATE.NONE;
		isGameStart = false;
		isGameEnd = false;
		gameTime = 0f;
		findTime = PLAY_FIND_TIME;
		currentTurnNum = 0;
	}
	public void UpdateMethod()
	{
		if (!isGameStart || isGameEnd)
		{
			return;
		}
		if (state == STATE.CHARA_MOVE && !SingletonCustom<FindMask_MaskManager>.Instance.IsSecondSelect)
		{
			findTime -= Time.deltaTime;
			if (findTime >= 0f)
			{
				SingletonCustom<FindMask_UIManager>.Instance.SetTime(findTime);
			}
			if (findTime < 0f)
			{
				findTime = 0f;
				SingletonCustom<FindMask_UIManager>.Instance.SetTime(findTime);
				state = STATE.SELECT_FAILD;
				StartCoroutine(SingletonCustom<FindMask_MaskManager>.Instance.SelectMaskFaild());
			}
		}
		if (state == STATE.FIND_ALL)
		{
			GameEnd();
		}
		gameTime += Time.deltaTime;
	}
	private IEnumerator _OpenGameDirection()
	{
		yield return null;
		SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate
		{
			SingletonCustom<FindMask_UIManager>.Instance.ViewFirstControlInfo();
			StartCoroutine(SingletonCustom<FindMask_MaskManager>.Instance._StartDirecting());
		});
	}
	private IEnumerator _SecoundGroupDirection()
	{
		yield return new WaitForSeconds(2f);
		StartCoroutine(SingletonCustom<FindMask_MaskManager>.Instance._StartDirecting());
	}
	public void GameStart()
	{
		isGameStart = true;
	}
	public void GameEnd()
	{
		if (isGameEnd)
		{
			return;
		}
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE && SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && SingletonCustom<FindMask_ScoreManager>.Instance.WinJudgPlayer())
			{
				switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
				{
				case 0:
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BLOW_AWAY_TANK);
					break;
				case 1:
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BLOW_AWAY_TANK);
					break;
				case 2:
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BLOW_AWAY_TANK);
					break;
				}
			}
			ResultGameDataParams.SetRecord_Int(SingletonCustom<FindMask_ScoreManager>.Instance.ArrayScore, arrayTurnPlayer);
			LeanTween.delayedCall(RESULT_INTERVAL, (Action)delegate
			{
				rankingResultManager.ShowResult_Score();
			});
		}
		else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			if (IsEightPlsyers)
			{
				ResultGameDataParams.SetRecord_Int_Ranking8Numbers(SingletonCustom<FindMask_ScoreManager>.Instance.ArrayScore, arrayTurnPlayer);
				LeanTween.delayedCall(RESULT_INTERVAL, (Action)delegate
				{
					rankingResultManager.ShowResult_Score();
				});
			}
			else
			{
				ResultGameDataParams.SetRecord_WinOrLose(SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(0));
				ResultGameDataParams.SetRecord_WinOrLose(SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(1), 1);
				if (SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(0) > SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(1))
				{
					LeanTween.delayedCall(RESULT_INTERVAL, (Action)delegate
					{
						winResultManager.ShowResult(ResultGameDataParams.ResultType.Win, 0);
					});
				}
				else if (SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(0) == SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(1))
				{
					LeanTween.delayedCall(RESULT_INTERVAL, (Action)delegate
					{
						winResultManager.ShowResult(ResultGameDataParams.ResultType.Draw, 0);
					});
				}
				else
				{
					LeanTween.delayedCall(RESULT_INTERVAL, (Action)delegate
					{
						winResultManager.ShowResult(ResultGameDataParams.ResultType.Lose, 0);
					});
				}
			}
		}
		else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP)
		{
			ResultGameDataParams.SetRecord_WinOrLose(SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(0));
			ResultGameDataParams.SetRecord_WinOrLose(SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(1), 1);
			if (SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(0) > SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(1))
			{
				LeanTween.delayedCall(RESULT_INTERVAL, (Action)delegate
				{
					winResultManager.ShowResult(ResultGameDataParams.ResultType.Win, 0);
				});
			}
			else if (SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(0) < SingletonCustom<FindMask_ScoreManager>.Instance.GetTeamTotalScore(1))
			{
				LeanTween.delayedCall(RESULT_INTERVAL, (Action)delegate
				{
					winResultManager.ShowResult(ResultGameDataParams.ResultType.Win, 1);
				});
			}
			else
			{
				LeanTween.delayedCall(RESULT_INTERVAL, (Action)delegate
				{
					winResultManager.ShowResult(ResultGameDataParams.ResultType.Draw, 0);
				});
			}
		}
		else
		{
			ResultGameDataParams.SetRecord_Int(SingletonCustom<FindMask_ScoreManager>.Instance.ArrayScore, arrayTurnPlayer);
			LeanTween.delayedCall(RESULT_INTERVAL, (Action)delegate
			{
				rankingResultManager.ShowResult_Score();
			});
		}
		isGameEnd = true;
	}
	private IEnumerator _GameEnd()
	{
		yield return new WaitForSeconds(1.5f);
		rankingResultManager.ShowResult_Score();
	}
	public void NextTurnPlayer()
	{
		SingletonCustom<FindMask_UIManager>.Instance.SetTime(PLAY_FIND_TIME);
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			if (SingletonCustom<FindMask_MaskManager>.Instance.IsFindAllPair)
			{
				GameEnd();
			}
			else
			{
				currentTurnNum++;
				if (currentTurnNum >= arrayTurnPlayer.Length)
				{
					currentTurnNum = 0;
				}
				turnPlayerNo = arrayTurnPlayer[currentTurnNum];
				SingletonCustom<FindMask_UIManager>.Instance.NextTurnPlayerFade();
			}
		});
	}
	private void Shuffle(int[] arr)
	{
		for (int num = arr.Length - 1; num > 0; num--)
		{
			int num2 = UnityEngine.Random.Range(0, num + 1);
			int num3 = arr[num];
			arr[num] = arr[num2];
			arr[num2] = num3;
		}
	}
	public bool ATeamIn1P()
	{
		for (int i = 0; i < arrayTurnPlayer.Length; i++)
		{
			if (i % 2 == 0 && arrayTurnPlayer[i] == 0)
			{
				return true;
			}
		}
		return false;
	}
	private void SetPlayerElementArray(int[] arr)
	{
		int num = 0;
		for (int i = 0; i < arr.Length; i++)
		{
			num = 0;
			for (int j = 0; j < arr.Length; j++)
			{
				if (arr[i] > arr[j])
				{
					num++;
				}
			}
			arrayPlayerElement[i] = num;
		}
	}
	public void ResetGame()
	{
		state = STATE.NONE;
		findTime = PLAY_FIND_TIME;
	}
	public void ResetTimer()
	{
		findTime = PLAY_FIND_TIME;
		SingletonCustom<FindMask_UIManager>.Instance.SetTime(PLAY_FIND_TIME);
	}
	public Camera GetCamera()
	{
		return camera;
	}
	private void DebugEnd()
	{
	}
}
