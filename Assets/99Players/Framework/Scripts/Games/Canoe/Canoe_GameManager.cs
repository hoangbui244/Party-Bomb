using System;
using UnityEngine;
public class Canoe_GameManager : SingletonCustom<Canoe_GameManager>
{
	[SerializeField]
	[Header("順位リザルト")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("3Dル\u30fcト")]
	private GameObject root3D;
	private float gameTime;
	private bool isGameStart;
	private bool isGameEnd;
	private int playerGoalCnt;
	private int goalCnt;
	private bool isSkipFlg;
	private float isSkipWaitTime;
	private float SKIP_WAIT_TIME = 5f;
	private bool isShowSkipControl;
	private bool isAutoGameEnd;
	private float isAutoGameEndTime;
	private readonly float AUTO_GAME_END_TIME = 20f;
	private bool isOnceGoalVoice;
	public void Init()
	{
	}
	public void UpdateMethod()
	{
		gameTime += Time.deltaTime;
		gameTime = Mathf.Clamp(gameTime, 0f, 599.99f);
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (GetIsViewCamera(i) && !SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(i).GetIsGoal())
			{
				SingletonCustom<Canoe_UIManager>.Instance.SetTime(i, gameTime);
			}
		}
		if (isSkipFlg)
		{
			isSkipWaitTime += Time.deltaTime;
			if (isSkipWaitTime > SKIP_WAIT_TIME)
			{
				CPUAutoGoal();
				GameEnd(_isAutoGameEnd: true);
				return;
			}
		}
		if (!isAutoGameEnd)
		{
			return;
		}
		isAutoGameEndTime += Time.deltaTime;
		if (!(isAutoGameEndTime >= AUTO_GAME_END_TIME))
		{
			return;
		}
		for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
		{
			Canoe_Player player = SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(j);
			if (!player.GetIsGoal())
			{
				if (!player.GetIsCpu())
				{
					player.SetGoal(-1f);
				}
				else
				{
					player.SetAutoGoalTime();
				}
				player.SetRowingAnimationSpeed(0f);
			}
		}
		GameEnd(_isAutoGameEnd: true);
	}
	public bool GetIsGameStart()
	{
		return isGameStart;
	}
	public bool GetIsGameEnd()
	{
		return isGameEnd;
	}
	public float GetGameTime()
	{
		return gameTime;
	}
	public void SetIsGoal(int _playerNo, int _userType, int _rank)
	{
		goalCnt++;
		if (_userType < 4)
		{
			playerGoalCnt++;
			SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
		}
		if (!isOnceGoalVoice && _userType < 4)
		{
			isOnceGoalVoice = true;
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal");
		}
		if (GetIsViewCamera(_playerNo))
		{
			SingletonCustom<Canoe_UIManager>.Instance.ShowGoalRank(_playerNo, _rank);
			SingletonCustom<Canoe_UIManager>.Instance.SetPlayerIconActive(_playerNo, _isActive: false);
		}
		if (goalCnt == SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length)
		{
			GameEnd();
		}
		else if (playerGoalCnt == SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
			isSkipFlg = true;
		}
		else if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && goalCnt == SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length - 1 && playerGoalCnt == SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1)
		{
			isAutoGameEnd = true;
		}
	}
	public int GetRank(int _playerNo)
	{
		int num = 1;
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			int playerNo = SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(i).GetPlayerNo();
			if (_playerNo != playerNo && SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(i).GetIsGoal())
			{
				float goalTime = SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(playerNo).GetGoalTime();
				if (SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(_playerNo).GetGoalTime() > goalTime)
				{
					num++;
				}
			}
		}
		return num;
	}
	public void SetIsGameStart()
	{
		isGameStart = true;
	}
	public void GameStart()
	{
	}
	private void CPUAutoGoal()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			Canoe_Player player = SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(i);
			if (player.GetIsCpu() && !player.GetIsGoal())
			{
				player.SetAutoGoalTime();
			}
		}
	}
	private void GameEnd(bool _isAutoGameEnd = false)
	{
		UnityEngine.Debug.Log("ゲ\u30fcム終了処理");
		isGameEnd = true;
		LeanTween.delayedCall(base.gameObject, _isAutoGameEnd ? 1f : 5f, (Action)delegate
		{
			for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
			{
				if (GetIsViewCamera(i))
				{
					SingletonCustom<Canoe_UIManager>.Instance.HideUI(i);
				}
			}
			SingletonCustom<CommonEndSimple>.Instance.Show(delegate
			{
				if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
				{
					float goalTime = SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(0).GetGoalTime();
					if (goalTime != -1f)
					{
						if (goalTime <= Canoe_Define.BRONZE_TIME)
						{
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BLOW_AWAY_TANK);
						}
						if (goalTime <= Canoe_Define.SILVER_TIME)
						{
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BLOW_AWAY_TANK);
						}
						if (goalTime <= Canoe_Define.GOLD_TIME)
						{
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BLOW_AWAY_TANK);
						}
					}
				}
				ResultGameDataParams.SetPoint();
				for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
				{
					Canoe_Player player = SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(j);
					if (player.GetIsCpu() && !player.GetIsGoal())
					{
						SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(j).SetGoal(CalcManager.ConvertDecimalSecond(UnityEngine.Random.Range(70f, 90f)));
					}
				}
				float[] array = new float[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
				int[] array2 = new int[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
				for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; k++)
				{
					array[k] = SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(k).GetGoalTime();
					array2[k] = (int)SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(k).GetUserType();
					CalcManager.ConvertTimeToRecordString(array[k], array2[k]);
				}
				ResultGameDataParams.SetRecord_Float(array, array2, _isGroup1Record: true, _isAscendingOrder: true);
				rankingResult.ShowResult_Time();
				LeanTween.delayedCall(base.gameObject, 1.5f, (Action)delegate
				{
					root3D.SetActive(value: false);
				});
			});
		});
	}
	public bool GetIsViewCamera(int _playerNo)
	{
		if ((SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1 && _playerNo > 0) || (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2 && _playerNo > 1))
		{
			return false;
		}
		return true;
	}
	public void DebugGameEnd()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			float time = UnityEngine.Random.Range(70f, 90f);
			SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(i).SetDebugGoal(CalcManager.ConvertDecimalSecond(time));
		}
		GameEnd(_isAutoGameEnd: true);
	}
	public void DebugJustBeforeGoal()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			Canoe_Player player = SingletonCustom<Canoe_PlayerManager>.Instance.GetPlayer(i);
			if (!player.GetIsGoal())
			{
				player.transform.position = SingletonCustom<Canoe_CourseManager>.Instance.DebugJustBeforeGoal(player.transform.position);
			}
		}
	}
}
