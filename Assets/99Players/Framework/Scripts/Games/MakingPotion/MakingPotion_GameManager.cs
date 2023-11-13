using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MakingPotion_GameManager : SingletonCustom<MakingPotion_GameManager>
{
	public const float GAME_SECOND_TIME = 90f;
	public const int CHARA_NUM = 4;
	public static readonly Rect[] SINGLE_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0f, 0.666f, 1f),
		new Rect(0.667f, 0.667f, 0.333f, 0.333f),
		new Rect(0.667f, 0.3335f, 0.333f, 0.333f),
		new Rect(0.667f, 0f, 0.333f, 0.333f)
	};
	public static readonly Rect[] MULTI_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0.5f, 0.5f, 0.5f),
		new Rect(0.5f, 0.5f, 0.5f, 0.5f),
		new Rect(0f, 0f, 0.5f, 0.5f),
		new Rect(0.5f, 0f, 0.5f, 0.5f)
	};
	private const int TROPHY_GOLD_SCORE = 4000;
	private const int TROPHY_SILVER_SCORE = 3000;
	private const int TROPHY_BRONZE_SCORE = 2000;
	public static int DebugRetryCount = 0;
	[SerializeField]
	private WinOrLoseResultManager winResultManager;
	[SerializeField]
	private RankingResultManager rankingResultManager;
	[SerializeField]
	private Camera[] cameras;
	[SerializeField]
	[Header("シングル時のみActiveにするオブジェクト")]
	private GameObject[] onlySingleObjects;
	[SerializeField]
	[Header("マルチ時のみActiveにするオブジェクト")]
	private GameObject[] onlyMultiObjects;
	private bool isGameStart;
	private bool isGameEnd;
	private bool hasSecondGroup;
	private bool isNowSecondGroup;
	private float gameTime;
	private int playerNum;
	private int teamNum;
	private int[] scores;
	private int[] playerNoArray;
	private int[] teamNoArray;
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
	public bool HasSecondGroup => hasSecondGroup;
	public bool IsNowSecondGroup => isNowSecondGroup;
	public float GameTime => gameTime;
	public float RemainViewTime
	{
		get
		{
			if (!isGameStart)
			{
				return 90f;
			}
			if (isGameEnd)
			{
				return 0f;
			}
			return 90f - GameTime;
		}
	}
	public int PlayerNum => playerNum;
	public int TeamNum => teamNum;
	public void Init()
	{
		ResultGameDataParams.SetPoint();
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		teamNum = SingletonCustom<GameSettingManager>.Instance.TeamNum;
		List<int>[] playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		if (playerGroupList[0].Count > 2)
		{
			hasSecondGroup = true;
			if (playerGroupList[0][3] < 4)
			{
				switch (UnityEngine.Random.Range(0, 6))
				{
				case 0:
					playerGroupList[0] = new List<int>
					{
						0,
						1,
						2,
						3
					};
					break;
				case 1:
					playerGroupList[0] = new List<int>
					{
						0,
						2,
						1,
						3
					};
					break;
				case 2:
					playerGroupList[0] = new List<int>
					{
						0,
						3,
						1,
						2
					};
					break;
				case 3:
					playerGroupList[0] = new List<int>
					{
						1,
						2,
						0,
						3
					};
					break;
				case 4:
					playerGroupList[0] = new List<int>
					{
						1,
						3,
						0,
						2
					};
					break;
				case 5:
					playerGroupList[0] = new List<int>
					{
						2,
						3,
						0,
						1
					};
					break;
				}
			}
			else
			{
				switch (UnityEngine.Random.Range(0, 3))
				{
				case 0:
					playerGroupList[0] = new List<int>
					{
						0,
						1,
						2,
						4
					};
					break;
				case 1:
					playerGroupList[0] = new List<int>
					{
						0,
						2,
						1,
						4
					};
					break;
				case 2:
					playerGroupList[0] = new List<int>
					{
						1,
						2,
						0,
						4
					};
					break;
				}
			}
		}
		if (playerNum == 1)
		{
			for (int i = 0; i < cameras.Length; i++)
			{
				cameras[i].rect = SINGLE_CAMERA_RECT[i];
			}
			for (int j = 0; j < onlySingleObjects.Length; j++)
			{
			}
			for (int k = 0; k < onlyMultiObjects.Length; k++)
			{
			}
		}
		else
		{
			for (int l = 0; l < cameras.Length; l++)
			{
				cameras[l].rect = MULTI_CAMERA_RECT[l];
			}
			for (int m = 0; m < onlySingleObjects.Length; m++)
			{
			}
			for (int n = 0; n < onlyMultiObjects.Length; n++)
			{
			}
		}
		int num = hasSecondGroup ? 8 : 4;
		playerNoArray = new int[num];
		teamNoArray = new int[num];
		if (hasSecondGroup)
		{
			for (int num2 = 0; num2 < num; num2++)
			{
				playerNoArray[num2] = playerGroupList[num2 / 2 % 2][num2 % 2 + num2 / 4 * 2];
				teamNoArray[num2] = num2 / 2 % 2;
			}
		}
		else
		{
			int num3 = 4;
			for (int num4 = 0; num4 < num; num4++)
			{
				if (num4 < playerNum)
				{
					playerNoArray[num4] = num4;
				}
				else
				{
					playerNoArray[num4] = num3;
					num3++;
				}
				if (teamNum == 2)
				{
					teamNoArray[num4] = ((!playerGroupList[0].Contains(playerNoArray[num4])) ? 1 : 0);
				}
				else
				{
					teamNoArray[num4] = num4;
				}
			}
		}
		DataInit();
		StartCoroutine(_OpenGameDirection());
		DebugRetryCount++;
	}
	public void SecondGroupInit()
	{
		SingletonCustom<MakingPotion_UiManager>.Instance.Fade(1f, 2f, delegate
		{
			RecordSet(_isSecondGroup: false);
			DataInit();
			SingletonCustom<MakingPotion_TargetManager>.Instance.SecondGroupInit();
			SingletonCustom<MakingPotion_PlayerManager>.Instance.SecondGroupInit();
			SingletonCustom<MakingPotion_UiManager>.Instance.SecondGroupInit();
			StartCoroutine(_SecoundGroupDirection());
		});
	}
	private void DataInit()
	{
		isGameStart = false;
		isGameEnd = false;
		gameTime = 0f;
		scores = new int[4];
	}
	public void UpdateMethod()
	{
		bool isGameNow = IsGameNow;
	}
	public void AddScore(int _charaNo, int _addValue)
	{
		scores[_charaNo] += _addValue;
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
			for (int i = 0; i < 4; i++)
			{
				if (SingletonCustom<MakingPotion_PlayerManager>.Instance.GetPlayer(i).TeamNo == _teamNo)
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
		if (isNowSecondGroup)
		{
			return playerNoArray[_charaNo + 4];
		}
		return playerNoArray[_charaNo];
	}
	public int GetCharaTeamNo(int _charaNo)
	{
		if (isNowSecondGroup)
		{
			return teamNoArray[_charaNo + 4];
		}
		return teamNoArray[_charaNo];
	}
	public Camera GetCamera(int _charaNo)
	{
		return cameras[_charaNo];
	}
	private IEnumerator _OpenGameDirection()
	{
		yield return null;
		SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate
		{
			LeanTween.delayedCall(base.gameObject, 3.5f, (Action)delegate
			{
				SingletonCustom<MakingPotion_UiManager>.Instance.HideAnnounceText();
				SingletonCustom<CommonStartSimple>.Instance.transform.SetLocalPositionY(0f);
				if (playerNum == 1)
				{
					SingletonCustom<MakingPotion_UiManager>.Instance.ViewFirstControlInfo();
					LeanTween.delayedCall(1f, (Action)delegate
					{
						SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
					});
				}
				else
				{
					SingletonCustom<MakingPotion_UiManager>.Instance.ViewFirstControlInfo();
					LeanTween.delayedCall(1f, (Action)delegate
					{
						SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
					});
					GroupVibration();
				}
			});
		});
	}
	private IEnumerator _SecoundGroupDirection()
	{
		yield return new WaitForSeconds(2f);
		SingletonCustom<CommonStartSimple>.Instance.Show(GameStart);
		GroupVibration();
	}
	private void GroupVibration()
	{
		if (!hasSecondGroup)
		{
			return;
		}
		if (isNowSecondGroup)
		{
			int num = playerNoArray[4];
			if (num < 4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(num);
			}
			num = playerNoArray[5];
			if (num < 4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(num);
			}
		}
		else
		{
			int commonVibration = playerNoArray[0];
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(commonVibration);
			commonVibration = playerNoArray[1];
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(commonVibration);
		}
	}
	public void GameStart()
	{
		isGameStart = true;
		SingletonCustom<MakingPotion_PlayerManager>.Instance.GameStart();
	}
	public void GameEnd()
	{
		if (!isGameEnd)
		{
			isGameEnd = true;
			if (hasSecondGroup && !isNowSecondGroup)
			{
				SecondGroupInit();
				isNowSecondGroup = true;
			}
			else
			{
				StartCoroutine(_GameEnd());
			}
		}
	}
	private IEnumerator _GameEnd()
	{
		yield return new WaitForSeconds(1.5f);
		RecordSet(isNowSecondGroup);
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
	private void RecordSet(bool _isSecondGroup)
	{
		if ((SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP)
		{
			for (int i = 0; i < teamNum; i++)
			{
				ResultGameDataParams.SetRecord_WinOrLose(GetTeamScore(i), i);
			}
			return;
		}
		int[] array = new int[scores.Length];
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = scores[j];
		}
		ResultGameDataParams.SetRecord_Int(array, SingletonCustom<MakingPotion_PlayerManager>.Instance.GetPlayerNoArray(), !_isSecondGroup);
	}
	private void TrophySet()
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (scores[0] >= 2000)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BOMB_ROULETTE);
			}
			if (scores[0] >= 3000)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BOMB_ROULETTE);
			}
			if (scores[0] >= 4000)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BOMB_ROULETTE);
			}
		}
	}
	private void EndCamera()
	{
		for (int i = 0; i < cameras.Length; i++)
		{
			cameras[i].enabled = false;
		}
		SingletonCustom<MakingPotion_UiManager>.Instance.CloseGameUI();
	}
	private void DebugEnd()
	{
		for (int i = 0; i < scores.Length; i++)
		{
			scores[i] = UnityEngine.Random.Range(10, 50) * 100;
		}
		GameEnd();
	}
}
