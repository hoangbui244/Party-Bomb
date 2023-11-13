using UnityEngine;
public class RockClimbing_Obstacle : MonoBehaviour
{
	[SerializeField]
	[Header("生成可能な障害物の種類配列")]
	private RockClimbing_ClimbingWallManager.ObstacleType[] arrayObstacleType;
	private RockClimbing_ClimbingWallManager.ObstacleType obstacleType;
	private GameObject obstacleObj;
	private bool isCreate;
	public void CreateObstacle(bool _isReverse)
	{
		isCreate = true;
		obstacleType = arrayObstacleType[Random.Range(0, arrayObstacleType.Length)];
		UnityEngine.Debug.Log("生成 " + obstacleType.ToString());
		obstacleObj = UnityEngine.Object.Instantiate(SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstaclePref((int)obstacleType));
		obstacleObj.transform.parent = base.transform;
		switch (obstacleType)
		{
		case RockClimbing_ClimbingWallManager.ObstacleType.Block_Mediium:
		case RockClimbing_ClimbingWallManager.ObstacleType.Block_Small:
			obstacleObj.transform.localPosition = new Vector3(0f, 0f, 0.25f);
			break;
		case RockClimbing_ClimbingWallManager.ObstacleType.Crevasse:
			obstacleObj.transform.localPosition = new Vector3(0f, 0f, 0.5f);
			break;
		}
	}
	public void CreateObstacle(RockClimbing_ClimbingWallManager.ObstacleType _obstacleType, GameObject _obstacleObj)
	{
		obstacleType = _obstacleType;
		obstacleObj = UnityEngine.Object.Instantiate(_obstacleObj);
		obstacleObj.transform.parent = base.transform;
		obstacleObj.transform.localPosition = _obstacleObj.transform.localPosition;
	}
	public RockClimbing_ClimbingWallManager.ObstacleType GetObstacleType()
	{
		return obstacleType;
	}
	public GameObject GetObstacleObj()
	{
		return obstacleObj;
	}
	public bool GetIsCreate()
	{
		return isCreate;
	}
}
