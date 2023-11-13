using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WhackMoleGameManager : SingletonCustom<WhackMoleGameManager>
{
	public const float GAME_SECOND_TIME = 60f;
	public const float FEVER_SECOND_TIME = 40f;
	public const int CHARA_NUM = 4;
	public const int HOLE_NUM = 9;
	public const int DEFAULT_HOLE_NO = 4;
	public const string TAG_MOLE = "Object";
	public static readonly int[][] ADJACENT_HOLE_NO_ARRAY = new int[9][]
	{
		new int[3]
		{
			1,
			3,
			4
		},
		new int[5]
		{
			0,
			2,
			3,
			4,
			5
		},
		new int[3]
		{
			1,
			4,
			5
		},
		new int[5]
		{
			0,
			1,
			4,
			6,
			7
		},
		new int[8]
		{
			0,
			1,
			2,
			3,
			5,
			6,
			7,
			8
		},
		new int[5]
		{
			1,
			2,
			4,
			7,
			8
		},
		new int[3]
		{
			3,
			4,
			7
		},
		new int[5]
		{
			3,
			4,
			5,
			6,
			8
		},
		new int[3]
		{
			4,
			5,
			7
		}
	};
	public static readonly int[][] ADJACENT_DIR_INDEX_ARRAY = new int[9][]
	{
		new int[9]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			0,
			-1,
			1,
			2
		},
		new int[9]
		{
			-1,
			-1,
			-1,
			0,
			-1,
			1,
			2,
			3,
			4
		},
		new int[9]
		{
			-1,
			-1,
			-1,
			0,
			-1,
			-1,
			1,
			2,
			-1
		},
		new int[9]
		{
			-1,
			0,
			1,
			-1,
			-1,
			2,
			-1,
			3,
			4
		},
		new int[9]
		{
			0,
			1,
			2,
			3,
			-1,
			4,
			5,
			6,
			7
		},
		new int[9]
		{
			0,
			1,
			-1,
			2,
			-1,
			-1,
			3,
			4,
			-1
		},
		new int[9]
		{
			-1,
			0,
			1,
			-1,
			-1,
			2,
			-1,
			-1,
			-1
		},
		new int[9]
		{
			0,
			1,
			2,
			3,
			-1,
			4,
			-1,
			-1,
			-1
		},
		new int[9]
		{
			0,
			1,
			-1,
			2,
			-1,
			-1,
			-1,
			-1,
			-1
		}
	};
	private static readonly Rect[] SINGLE_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0f, 0.666f, 1f),
		new Rect(0.667f, 0.667f, 0.333f, 0.333f),
		new Rect(0.667f, 0.3335f, 0.333f, 0.333f),
		new Rect(0.667f, 0f, 0.333f, 0.333f)
	};
	private static readonly Rect[] MULTI_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0.5f, 0.5f, 0.5f),
		new Rect(0.5f, 0.5f, 0.5f, 0.5f),
		new Rect(0f, 0f, 0.5f, 0.5f),
		new Rect(0.5f, 0f, 0.5f, 0.5f)
	};
	private const int TROPHY_GOLD_SCORE = 4500;
	private const int TROPHY_SILVER_SCORE = 3500;
	private const int TROPHY_BRONZE_SCORE = 2000;
	public static int DebugRetryCount = 0;
	[SerializeField]
	private WinOrLoseResultManager winResultManager;
	[SerializeField]
	private RankingResultManager rankingResultManager;
	[SerializeField]
	private Camera[] cameras;
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
	private bool isFeverTime;
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
				return 60f;
			}
			if (isGameEnd)
			{
				return 0f;
			}
			return 60f - GameTime;
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
		}
		else
		{
			for (int j = 0; j < cameras.Length; j++)
			{
				cameras[j].rect = MULTI_CAMERA_RECT[j];
			}
		}
		int num = hasSecondGroup ? 8 : 4;
		playerNoArray = new int[num];
		teamNoArray = new int[num];
		if (hasSecondGroup)
		{
			for (int k = 0; k < num; k++)
			{
				playerNoArray[k] = playerGroupList[k / 2 % 2][k % 2 + k / 4 * 2];
				teamNoArray[k] = k / 2 % 2;
			}
		}
		else
		{
			int num2 = 4;
			for (int l = 0; l < num; l++)
			{
				if (l < playerNum)
				{
					playerNoArray[l] = l;
				}
				else
				{
					playerNoArray[l] = num2;
					num2++;
				}
				if (teamNum == 2)
				{
					teamNoArray[l] = ((!playerGroupList[0].Contains(playerNoArray[l])) ? 1 : 0);
				}
				else
				{
					teamNoArray[l] = l;
				}
			}
		}
		DataInit();
		StartCoroutine(_OpenGameDirection());
		DebugRetryCount++;
	}
	public void SecondGroupInit()
	{
		SingletonCustom<WhackMoleUiManager>.Instance.Fade(1f, 2f, delegate
		{
			DataInit();
			SingletonCustom<WhackMoleCharacterManager>.Instance.SecondGroupInit();
			SingletonCustom<WhackMoleTargetManager>.Instance.SecondGroupInit();
			SingletonCustom<WhackMoleUiManager>.Instance.SecondGroupInit();
			StartCoroutine(_SecoundGroupDirection());
		});
	}
	private void DataInit()
	{
		isGameStart = false;
		isGameEnd = false;
		gameTime = 0f;
		scores = new int[4];
		isFeverTime = false;
	}
	public void UpdateMethod()
	{
		if (IsGameNow)
		{
			gameTime += Time.deltaTime;
			if (!isFeverTime && gameTime > 40f)
			{
				FeverTimeSetting();
			}
			if (gameTime > 60f)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
				GameEnd();
			}
		}
	}
	private void FeverTimeSetting()
	{
		isFeverTime = true;
		SingletonCustom<WhackMoleTargetManager>.Instance.PlayFeverEffect();
		StartCoroutine(_ChangePitchBgm());
	}
	private IEnumerator _ChangePitchBgm()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_lap_jingle");
		yield return new WaitForSeconds(1.2f);
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
				if (SingletonCustom<WhackMoleCharacterManager>.Instance.GetChara(i).TeamNo == _teamNo)
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
	public static int[] GetAdjacentHoleNoArray(int _holeNo)
	{
		return ADJACENT_HOLE_NO_ARRAY[_holeNo];
	}
	public static int GetAdjacentDirHoleNo(int _holeNo, int _dirNo)
	{
		int num = ADJACENT_DIR_INDEX_ARRAY[_holeNo][_dirNo];
		if (num == -1)
		{
			return -1;
		}
		return ADJACENT_HOLE_NO_ARRAY[_holeNo][num];
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
				GroupVibration();
			}
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
	}
	public void GameEnd()
	{
		if (isGameEnd)
		{
			return;
		}
		if ((SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP) || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP)
		{
			for (int i = 0; i < teamNum; i++)
			{
				ResultGameDataParams.SetRecord_WinOrLose(GetTeamScore(i), i);
			}
		}
		else
		{
			int[] array = new int[scores.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = scores[j];
			}
			ResultGameDataParams.SetRecord_Int(array, SingletonCustom<WhackMoleCharacterManager>.Instance.GetPlayerNoArray(), !isNowSecondGroup);
		}
		isGameEnd = true;
		if (hasSecondGroup && !isNowSecondGroup)
		{
			SecondGroupInit();
			isNowSecondGroup = true;
		}
		else
		{
			TrophySet();
			StartCoroutine(_GameEnd());
		}
	}
	private IEnumerator _GameEnd()
	{
		yield return new WaitForSeconds(1.5f);
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
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (scores[0] >= 2000)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.CANNON_SHOT);
			}
			if (scores[0] >= 3500)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.CANNON_SHOT);
			}
			if (scores[0] >= 4500)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.CANNON_SHOT);
			}
		}
	}
	private void EndCamera()
	{
		for (int i = 0; i < cameras.Length; i++)
		{
			cameras[i].enabled = false;
		}
	}
	private void DebugEnd()
	{
		for (int i = 0; i < scores.Length; i++)
		{
			scores[i] = UnityEngine.Random.Range(10, 80) * 100;
		}
		GameEnd();
	}
}
