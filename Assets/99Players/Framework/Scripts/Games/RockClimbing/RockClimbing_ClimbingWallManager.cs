using UnityEngine;
public class RockClimbing_ClimbingWallManager : SingletonCustom<RockClimbing_ClimbingWallManager>
{
	public enum ObstacleType
	{
		Block_Mediium,
		Block_Small,
		Crevasse
	}
	public enum ObstacleDropType
	{
		SnowMass,
		Icicles
	}
	public enum ClimbOnFoundationType
	{
		None,
		StoneWall_1,
		StoneWall_2,
		StoneWall_3,
		StoneWall_MAX,
		Roof_1,
		Roof_2,
		Roof_3,
		Roof_4,
		Roof_MAX
	}
	public enum ClimbPlayerType
	{
		Player_0,
		Player_1,
		Player_2,
		Player_3
	}
	public enum ObstacleThrowType
	{
		Shuriken
	}
	[SerializeField]
	[Header("登る壁配列")]
	private RockClimbing_ClimbingWall[] arrayClimbingWall;
	[SerializeField]
	[Header("生成する障害物プレハブ配列")]
	private GameObject[] arrayObstaclePref;
	private readonly int BLOCK_OBSTACLE_CNT = 1;
	private readonly int CREVASSE_OBSTACLE_CNT = 1;
	[SerializeField]
	[Header("生成する落下用の障害物プレハブ配列")]
	private RockClimbing_Obstacle_Drop_Object[] arrayObstacleDropPref;
	private float[] obstacleDropRadius = new float[2]
	{
		0.55f,
		0.55f
	};
	private readonly float OBSTACLE_DROP_POWER = 1f;
	private readonly float OBSTACLE_DROP_HEIGHT = 8.125f;
	private readonly float OBSTACLE_DROP_LANE_IDX_PROBABILITY = 0.3f;
	private readonly float OBSTACLE_DROP_LANE_IDX_CAN_DISTANCE = 1.25f;
	private float goalHeight;
	[SerializeField]
	[Header("生成する土台グル\u30fcプのプレハブ配列")]
	private RockClimbing_ClimbOnFoundationObject_Group[] arrayClimbOnFoundationObjectGroupPref;
	private readonly float OBSTACLE_THROW_INTERVAL = 2f;
	public float OBSTACLE_THROW_POWER = 1f;
	[SerializeField]
	[Header("生成する投げる障害物プレハブ配列")]
	private RockClimbing_Obstacle_Throw_Object[] arrayObstacleThrowPref;
	private readonly int OBSTACLE_THROW_NEAR_ANCHOR_CNT = 4;
	private readonly float OBSTACLE_THROW_NEAR_ANCHOR_CHECK_DISTANCE = 8f;
	public void Init()
	{
		bool isReverse = Random.Range(0, 1) == 0;
		arrayClimbingWall[0].Init(_isFirstPlayer: true, isReverse);
	}
	public void UpdateMethod()
	{
		bool flag = true;
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			if (!SingletonCustom<RockClimbing_PlayerManager>.Instance.GetPlayer(i).GetIsGoal())
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			arrayClimbingWall[0].UpdateMethod();
		}
	}
	public RockClimbing_ClimbingWall GetClimbingWall(int _playerNo)
	{
		return arrayClimbingWall[0];
	}
	public GameObject GetObstaclePref(int _idx)
	{
		return arrayObstaclePref[_idx];
	}
	public int GetBlockObstacleCnt()
	{
		return BLOCK_OBSTACLE_CNT;
	}
	public int GetCrevasseObstacleCnt()
	{
		return CREVASSE_OBSTACLE_CNT;
	}
	public RockClimbing_Obstacle_Drop_Object GetObstacleDropPref(ObstacleDropType _obstacleDropType)
	{
		return arrayObstacleDropPref[(int)_obstacleDropType];
	}
	public float GetObstacleDropRadius(ObstacleDropType _obstacleDropType)
	{
		return obstacleDropRadius[(int)_obstacleDropType];
	}
	public float GetObstacleDropPower()
	{
		return OBSTACLE_DROP_POWER;
	}
	public float GetObstacleDropHeight()
	{
		return OBSTACLE_DROP_HEIGHT;
	}
	public float GetObstacleDropLaneIdxProbability()
	{
		return OBSTACLE_DROP_LANE_IDX_PROBABILITY;
	}
	public float GetObstacleDropLaneIdxCanDistance()
	{
		return OBSTACLE_DROP_LANE_IDX_CAN_DISTANCE;
	}
	public float GetGoalHeight()
	{
		return goalHeight;
	}
	public void SetGoalHeight(float _goalHeight)
	{
		goalHeight = _goalHeight;
	}
	public RockClimbing_ClimbOnFoundationObject_Group[] GetArrayClimbOnFoundationObjectGroupPref()
	{
		return arrayClimbOnFoundationObjectGroupPref;
	}
	public bool GetIsNoneFoundationType(ClimbOnFoundationType _type)
	{
		return _type == ClimbOnFoundationType.None;
	}
	public bool GetIsStoneWallFoundationType(ClimbOnFoundationType _type)
	{
		if (_type != ClimbOnFoundationType.StoneWall_1 && _type != ClimbOnFoundationType.StoneWall_2 && _type != ClimbOnFoundationType.StoneWall_3)
		{
			return _type == ClimbOnFoundationType.StoneWall_MAX;
		}
		return true;
	}
	public bool GetIsRoofFoundationType(ClimbOnFoundationType _type, bool _isCheckThrowGrapplingHook = false)
	{
		if ((_isCheckThrowGrapplingHook || _type != ClimbOnFoundationType.Roof_1) && _type != ClimbOnFoundationType.Roof_2 && _type != ClimbOnFoundationType.Roof_3 && _type != ClimbOnFoundationType.Roof_4)
		{
			return _type == ClimbOnFoundationType.Roof_MAX;
		}
		return true;
	}
	public float GetObstacleThrowInterval()
	{
		return Random.Range(OBSTACLE_THROW_INTERVAL - 0.25f, OBSTACLE_THROW_INTERVAL + 0.25f);
	}
	public RockClimbing_Obstacle_Throw_Object GetObstacleThrowPref(ObstacleThrowType _obstacleThrowType)
	{
		return arrayObstacleThrowPref[(int)_obstacleThrowType];
	}
	public float GetObstacleThrowPower()
	{
		return OBSTACLE_THROW_POWER;
	}
	public int GetObstacleThrowNearAnchorCnt()
	{
		return OBSTACLE_THROW_NEAR_ANCHOR_CNT;
	}
	public float GetObstacleThrowNearAnchorCheckDistance()
	{
		return OBSTACLE_THROW_NEAR_ANCHOR_CHECK_DISTANCE;
	}
}
