using System;
using UnityEngine;
public class RockClimbing_GameManager : SingletonCustom<RockClimbing_GameManager>
{
	[SerializeField]
	[Header("順位リザルト")]
	private RankingResultManager rankingResult;
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
			if (!SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(i).GetIsGoal() && GetIsViewCamera(i))
			{
				SingletonCustom<RockClimbing_UIManager>.Instance.SetTime(i, gameTime);
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
			RockClimbing_Player player = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(j);
			if (!player.GetIsGoal())
			{
				if (!player.GetIsCpu())
				{
					player.SetGoal(-1f);
					continue;
				}
				float time = SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetGoalHeight() / player.GetHeadTop().position.y * gameTime;
				player.SetGoal(CalcManager.ConvertDecimalSecond(time));
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
	public int GetGoalCnt()
	{
		return goalCnt;
	}
	public void SetIsGoal(int _playerNo, int _userType)
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
			SingletonCustom<RockClimbing_UIManager>.Instance.ShowGoalRank(_playerNo, GetRank(_playerNo));
			SingletonCustom<RockClimbing_UIManager>.Instance.SetPlayerIconActive(_playerNo, _isActive: false);
		}
		if (goalCnt == SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length)
		{
			GameEnd();
		}
		else if (playerGoalCnt == SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
			isSkipFlg = true;
		}
		else if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && playerGoalCnt == SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1)
		{
			isAutoGameEnd = true;
		}
	}
	public int GetRank(int _playerNo)
	{
		int num = 1;
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			int playerNo = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(i).GetPlayerNo();
			if (_playerNo != playerNo && SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(i).GetIsGoal())
			{
				float goalTime = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(playerNo).GetGoalTime();
				if (SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(_playerNo).GetGoalTime() > goalTime)
				{
					num++;
				}
			}
		}
		return num;
	}
	public void GameStart()
	{
		isGameStart = true;
	}
	public void GameStartAnimation()
	{
		SingletonCustom<RockClimbing_PlayerManager>.Instance.GameStartAnimation();
		SingletonCustom<RockClimbing_CameraManager>.Instance.GameStartAnimation();
	}
	private void CPUAutoGoal()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			RockClimbing_Player player = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(i);
			if (player.GetIsCpu() && !player.GetIsGoal())
			{
				float time = SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetGoalHeight() / player.GetHeadTop().position.y * gameTime;
				player.SetGoal(CalcManager.ConvertDecimalSecond(time));
			}
		}
	}
	private void GameEnd(bool _isAutoGameEnd = false)
	{
		isGameEnd = true;
		LeanTween.delayedCall(base.gameObject, _isAutoGameEnd ? 1f : 5f, (Action)delegate
		{
			for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
			{
				if (GetIsViewCamera(i))
				{
					SingletonCustom<RockClimbing_UIManager>.Instance.HideUI(i);
				}
			}
			SingletonCustom<CommonEndSimple>.Instance.Show(delegate
			{
				if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
				{
					float goalTime = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(0).GetGoalTime();
					if (goalTime != -1f)
					{
						if (goalTime <= RockClimbing_Define.BRONZE_TIME)
						{
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.MOLE_HAMMER);
						}
						if (goalTime <= RockClimbing_Define.SILVER_TIME)
						{
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.MOLE_HAMMER);
						}
						if (goalTime <= RockClimbing_Define.GOLD_TIME)
						{
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.MOLE_HAMMER);
						}
					}
				}
				ResultGameDataParams.SetPoint();
				for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
				{
					RockClimbing_Player player = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(j);
					if (player.GetIsCpu() && !player.GetIsGoal())
					{
						SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(j).SetGoal(CalcManager.ConvertDecimalSecond(UnityEngine.Random.Range(60f, 80f)));
					}
				}
				float[] array = new float[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
				int[] array2 = new int[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
				for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; k++)
				{
					array[k] = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(k).GetGoalTime();
					array2[k] = (int)SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(k).GetUserType();
					CalcManager.ConvertTimeToRecordString(array[k], array2[k]);
				}
				ResultGameDataParams.SetRecord_Float(array, array2, _isGroup1Record: true, _isAscendingOrder: true);
				rankingResult.ShowResult_Time();
			});
		});
	}
	public void GroupVibration()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0] < 4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0]);
			}
		}
	}
	public bool GetIsViewCamera(int _playerNo)
	{
		return true;
	}
	public void DebugGameEnd()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			float time = UnityEngine.Random.Range(60f, 80f);
			SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(i).SetGoal(CalcManager.ConvertDecimalSecond(time));
		}
		GameEnd(_isAutoGameEnd: true);
	}
	public void DebugJustBeforeGoal()
	{
		UnityEngine.Debug.Log("DebugJustBeforeGoal");
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			RockClimbing_Player player = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(i);
			RockClimbing_ClimbOnFoundation[] arrayClimbOnFoundationRoof = player.GetClimbingWall().GetArrayClimbOnFoundationRoof();
			int num = 3;
			RockClimbing_GrapplingHookPoint_Group grapplingHookPointGroup = arrayClimbOnFoundationRoof[num].GetGrapplingHookPointGroup(i);
			for (int j = 0; j < grapplingHookPointGroup.GetArrayGrapplingHookPoint().Length; j++)
			{
				grapplingHookPointGroup.GetArrayGrapplingHookPoint()[j].SetColliderActive(_isActive: false);
			}
			arrayClimbOnFoundationRoof[num].GetClimbOnCollider(i).SetColliderActive(_isActive: false);
			player.SetClimbOnFoundation(arrayClimbOnFoundationRoof[num]);
			player.SetMoveRigidStatus();
			Vector3 position = arrayClimbOnFoundationRoof[num].GetArrayClimbOnAnchor()[2].position;
			position.x = player.transform.position.x;
			player.transform.position = position;
		}
	}
}
