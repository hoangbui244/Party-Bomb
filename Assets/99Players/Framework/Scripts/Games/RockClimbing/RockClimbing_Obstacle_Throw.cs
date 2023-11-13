using System;
using System.Collections.Generic;
using UnityEngine;
public class RockClimbing_Obstacle_Throw : MonoBehaviour
{
	[Serializable]
	private struct ThrowGroup
	{
		public List<RockClimbing_Player> throwIdxPlayerList;
	}
	[SerializeField]
	[Header("投げるグル\u30fcプの管理クラス配列")]
	private RockClimbing_Obstacle_Throw_Group[] arrayObstacleThrowGroup;
	[SerializeField]
	private ThrowGroup[] arrayThrowGroup;
	public void Init()
	{
		arrayThrowGroup = new ThrowGroup[arrayObstacleThrowGroup.Length];
		for (int i = 0; i < arrayObstacleThrowGroup.Length; i++)
		{
			arrayObstacleThrowGroup[i].Init();
			arrayThrowGroup[i].throwIdxPlayerList = new List<RockClimbing_Player>();
		}
	}
	public void SetCharaNinja(int _idx, RockClimbing_CastleNiinja _charaNinja)
	{
		arrayObstacleThrowGroup[_idx].SetCharaNinja(_charaNinja);
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < arrayThrowGroup.Length; i++)
		{
			arrayThrowGroup[i].throwIdxPlayerList.Clear();
		}
		for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
		{
			RockClimbing_Player player = SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(j);
			if (player.GetIsGoal())
			{
				continue;
			}
			RockClimbing_ClimbingWallManager.ClimbOnFoundationType climbOnFoundationType = player.GetClimbOnFoundation().GetClimbOnFoundationType();
			if (player.GetState() != RockClimbing_PlayerManager.State.CLIMB_ON && climbOnFoundationType >= RockClimbing_ClimbingWallManager.ClimbOnFoundationType.StoneWall_MAX && (climbOnFoundationType != RockClimbing_ClimbingWallManager.ClimbOnFoundationType.StoneWall_MAX || player.GetGrapplingHook().GetState() != RockClimbing_GrapplingHook.State.HOOK))
			{
				int num = (int)(climbOnFoundationType - 4);
				if (climbOnFoundationType > RockClimbing_ClimbingWallManager.ClimbOnFoundationType.StoneWall_MAX && player.GetGrapplingHook().GetState() == RockClimbing_GrapplingHook.State.HOOK)
				{
					num--;
				}
				if (num < arrayObstacleThrowGroup.Length)
				{
					arrayThrowGroup[num].throwIdxPlayerList.Add(player);
				}
			}
		}
		for (int k = 0; k < arrayObstacleThrowGroup.Length; k++)
		{
			if (arrayThrowGroup[k].throwIdxPlayerList.Count > 0)
			{
				arrayObstacleThrowGroup[k].UpdateMethod(arrayThrowGroup[k].throwIdxPlayerList);
			}
			else if (arrayObstacleThrowGroup[k].GetIsObstacleThrow())
			{
				arrayObstacleThrowGroup[k].StopThrowObstacle();
			}
		}
	}
	public RockClimbing_Obstacle_Throw_Group GetThrowObstacleGroup(int _playerNo)
	{
		UnityEngine.Debug.Log("GetThrowObstacleGroup ");
		int num = (int)(SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(_playerNo).GetClimbOnFoundation().GetClimbOnFoundationType() - 4);
		if (num >= arrayObstacleThrowGroup.Length)
		{
			num = arrayObstacleThrowGroup.Length - 1;
		}
		return arrayObstacleThrowGroup[num];
	}
	public void StopThrowObstacle(int _playerNo)
	{
	}
}
