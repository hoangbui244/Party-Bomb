using System;
using UnityEngine;
public class RockClimbing_Obstacle_Drop : MonoBehaviour
{
	[Serializable]
	public struct ObstacleDrop
	{
		public bool isOnceDrop;
		public bool isObstacleDrop;
		public Transform dropHeightAnchor;
	}
	private RockClimbing_Player climbingPlayer;
	private Vector3 dropPos;
	[SerializeField]
	[Header("レ\u30fcンのアンカ\u30fc")]
	private Transform[] arrayLaneAnchor;
	[SerializeField]
	[Header("落下用の障害物配列")]
	private ObstacleDrop[] arrayObstacleDrop;
	private bool isOnceNearGoal;
	[SerializeField]
	[Header("ゴ\u30fcル付近の判定アンカ\u30fc")]
	private Transform nearGoalAnchor;
	private int checkOverLapLayerMask;
	private string[] checkOverLapMaskNameList = new string[2]
	{
		"Character",
		"Wall"
	};
	private int createLayerMask;
	private string[] createMaskNameList = new string[1]
	{
		"Wall"
	};
	private Collider[] arrayOverLapCollider = new Collider[4];
	private RaycastHit[] raycastHit = new RaycastHit[4];
	private float laneRadius;
	private int dropLaneLastLineIdx;
	[SerializeField]
	[Header("左側の制限アンカ\u30fc")]
	private Transform leftLimitAnchor;
	[SerializeField]
	[Header("右側の制限アンカ\u30fc")]
	private Transform rightLimitAnchor;
	public bool Debug_IsSphereCastGizmo;
	public Vector3 Debug_StartPos;
	public void Init()
	{
		laneRadius = arrayLaneAnchor[0].localScale.x / 2f;
		checkOverLapLayerMask = LayerMask.GetMask(checkOverLapMaskNameList);
		createLayerMask = LayerMask.GetMask(createMaskNameList);
		dropLaneLastLineIdx = -1;
	}
	public void SetClimbingPlayer(RockClimbing_Player _climbingPlayer)
	{
		climbingPlayer = _climbingPlayer;
	}
	public void UpdateMethod()
	{
		dropPos = climbingPlayer.transform.position;
		dropPos.y += SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropHeight();
		for (int i = 0; i < arrayObstacleDrop.Length; i++)
		{
			if (!arrayObstacleDrop[i].isOnceDrop && climbingPlayer.GetHeadTop().position.y >= arrayObstacleDrop[i].dropHeightAnchor.position.y)
			{
				arrayObstacleDrop[i].isOnceDrop = true;
				climbingPlayer.SetObstacleLineIdx(i);
				CreateObstacle(i);
			}
		}
		if (GetIsObstacleDrop() && SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(climbingPlayer.GetPlayerNo()))
		{
			SingletonCustom<RockClimbing_UIManager>.Instance.SetObstacleCautionIconPos(climbingPlayer.GetPlayerNo(), SingletonCustom<RockClimbing_CameraManager>.Instance.GetCamera(climbingPlayer.GetPlayerNo()).GetCamera().WorldToScreenPoint(arrayLaneAnchor[dropLaneLastLineIdx].transform.position));
		}
		if (!isOnceNearGoal && climbingPlayer.GetHeadTop().position.y >= nearGoalAnchor.position.y)
		{
			isOnceNearGoal = true;
			LeanTween.cancel(base.gameObject);
		}
	}
	private void CreateObstacle(int _dropLineIdx)
	{
		int playerCurrentLaneIdx = GetPlayerCurrentLaneIdx();
		int dropLaneIdx = playerCurrentLaneIdx;
		float num = UnityEngine.Random.Range(0f, 1f);
		if (num <= SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropLaneIdxProbability())
		{
			if (dropLaneIdx == 0)
			{
				dropLaneIdx++;
			}
			else if (dropLaneIdx == arrayLaneAnchor.Length - 1)
			{
				dropLaneIdx--;
			}
			else if (num <= SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropLaneIdxProbability() / 2f)
			{
				if (Mathf.Abs(climbingPlayer.transform.position.x - arrayLaneAnchor[playerCurrentLaneIdx + 1].transform.position.x) < SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropLaneIdxCanDistance())
				{
					dropLaneIdx++;
				}
			}
			else if (Mathf.Abs(climbingPlayer.transform.position.x - arrayLaneAnchor[playerCurrentLaneIdx - 1].transform.position.x) < SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropLaneIdxCanDistance())
			{
				dropLaneIdx--;
			}
		}
		UnityEngine.Debug.Log("palyerCurrentLaneIdx" + playerCurrentLaneIdx.ToString() + " dropLaneIdx " + dropLaneIdx.ToString());
		if (dropLaneIdx != -1)
		{
			dropLaneLastLineIdx = dropLaneIdx;
			Debug_IsSphereCastGizmo = true;
			if (SingletonCustom<RockClimbing_GameManager>.Instance.GetIsViewCamera(climbingPlayer.GetPlayerNo()))
			{
				SingletonCustom<RockClimbing_UIManager>.Instance.SetObstacleCautionIconPos(climbingPlayer.GetPlayerNo(), SingletonCustom<RockClimbing_CameraManager>.Instance.GetCamera(climbingPlayer.GetPlayerNo()).GetCamera().WorldToScreenPoint(arrayLaneAnchor[dropLaneIdx].transform.position));
				SingletonCustom<RockClimbing_UIManager>.Instance.ShowObstacleDropCautionIcon(climbingPlayer.GetPlayerNo());
			}
			if (!climbingPlayer.GetIsCpu())
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_iceclibling_obstacle_drop", _loop: false, 0f, 0.5f);
			}
			arrayObstacleDrop[_dropLineIdx].isObstacleDrop = true;
			LeanTween.delayedCall(arrayObstacleDrop[_dropLineIdx].dropHeightAnchor.gameObject, 1f, (Action)delegate
			{
				CapsuleCollider collider = climbingPlayer.GetCollider();
				Vector3 vector = new Vector3(arrayLaneAnchor[dropLaneIdx].transform.position.x, collider.transform.position.y, arrayLaneAnchor[dropLaneIdx].transform.position.z);
				RockClimbing_ClimbingWallManager.ObstacleDropType obstacleDropType = RockClimbing_ClimbingWallManager.ObstacleDropType.SnowMass;
				GameObject collisionIgnore = null;
				if (dropLaneIdx != playerCurrentLaneIdx)
				{
					float radius = collider.radius;
					float num2 = radius * 2f * climbingPlayer.transform.localScale.x;
					vector.x = climbingPlayer.transform.position.x + ((dropLaneIdx < playerCurrentLaneIdx) ? (-1f) : (1f * num2));
					Vector3 vector2 = vector;
					UnityEngine.Debug.Log("pos " + vector2.ToString());
					int num3 = Physics.OverlapSphereNonAlloc(vector, laneRadius, arrayOverLapCollider, createLayerMask);
					if (num3 > 0)
					{
						float num4 = -1000f;
						for (int i = 0; i < num3; i++)
						{
							if (arrayOverLapCollider[i].bounds.max.y > num4)
							{
								num4 = arrayOverLapCollider[i].bounds.max.y;
							}
						}
						vector.y = num4 + radius;
						vector2 = vector;
						UnityEngine.Debug.Log("調べる高さ 調整 pos " + vector2.ToString());
					}
				}
				int num5 = Physics.SphereCastNonAlloc(vector, laneRadius, Vector3.up, raycastHit, SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropHeight(), createLayerMask);
				vector.y = dropPos.y;
				if (num5 > 0)
				{
					for (int j = 0; j < num5; j++)
					{
						if (!(raycastHit[j].point == Vector3.zero))
						{
							obstacleDropType = RockClimbing_ClimbingWallManager.ObstacleDropType.Icicles;
							vector = raycastHit[j].point;
							vector.x = Mathf.Clamp(vector.x, raycastHit[j].collider.bounds.min.x + SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropRadius(obstacleDropType), raycastHit[j].collider.bounds.max.x - SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropRadius(obstacleDropType));
							collisionIgnore = raycastHit[j].collider.gameObject;
							break;
						}
					}
				}
				RockClimbing_Obstacle_Drop_Object rockClimbing_Obstacle_Drop_Object = UnityEngine.Object.Instantiate(SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropPref(obstacleDropType));
				rockClimbing_Obstacle_Drop_Object.transform.parent = base.transform;
				rockClimbing_Obstacle_Drop_Object.transform.position = vector;
				rockClimbing_Obstacle_Drop_Object.transform.SetLocalPositionZ(0.25f);
				rockClimbing_Obstacle_Drop_Object.Init(this);
				rockClimbing_Obstacle_Drop_Object.SetClimbingPlayer(climbingPlayer);
				rockClimbing_Obstacle_Drop_Object.SetCollisionIgnore(collisionIgnore);
				rockClimbing_Obstacle_Drop_Object.SetObstacleLineIdx(_dropLineIdx);
				rockClimbing_Obstacle_Drop_Object.Drop();
			});
		}
	}
	private int GetDropLaneIdx()
	{
		if (Physics.OverlapSphereNonAlloc(dropPos, laneRadius, arrayOverLapCollider, checkOverLapLayerMask) > 0)
		{
			return GetPlayerNearLaneIdx();
		}
		if (Physics.SphereCast(dropPos, laneRadius, Vector3.down, out RaycastHit hitInfo, SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropHeight(), checkOverLapLayerMask))
		{
			if (!(hitInfo.collider.tag != "Character"))
			{
				return GetPlayerCurrentLaneIdx();
			}
			return GetPlayerNearLaneIdx();
		}
		return -1;
	}
	public int GetPlayerCurrentLaneIdx()
	{
		float num = 1000f;
		int result = -1;
		for (int i = 0; i < arrayLaneAnchor.Length; i++)
		{
			float num2 = Mathf.Abs(climbingPlayer.transform.position.x - arrayLaneAnchor[i].position.x);
			if (num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}
	private int GetPlayerNearLaneIdx()
	{
		float num = 1000f;
		int result = -1;
		Vector3 vector = dropPos;
		for (int i = 0; i < arrayLaneAnchor.Length; i++)
		{
			vector.x = arrayLaneAnchor[i].position.x;
			if (Physics.OverlapSphereNonAlloc(vector, laneRadius, arrayOverLapCollider, checkOverLapLayerMask) <= 0 && !Physics.SphereCast(vector, laneRadius, Vector3.down, out RaycastHit _, SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropHeight(), checkOverLapLayerMask))
			{
				float num2 = Mathf.Abs(climbingPlayer.transform.position.x - arrayLaneAnchor[i].position.x);
				if (num2 < num)
				{
					num = num2;
					result = i;
				}
			}
		}
		return result;
	}
	public bool GetIsObstacleDrop()
	{
		for (int i = 0; i < arrayObstacleDrop.Length; i++)
		{
			if (arrayObstacleDrop[i].isObstacleDrop)
			{
				return true;
			}
		}
		return false;
	}
	public bool GetIsObstacleDrop(int _idx)
	{
		return arrayObstacleDrop[_idx].isObstacleDrop;
	}
	public void SetIsObstacleDrop(int _idx, bool _isObstacleDrop)
	{
		arrayObstacleDrop[_idx].isObstacleDrop = _isObstacleDrop;
	}
	public int GetDropLaneLastIdx()
	{
		return dropLaneLastLineIdx;
	}
	private void OnDrawGizmos()
	{
		Vector3 position = nearGoalAnchor.position;
		Vector3 position2 = nearGoalAnchor.position;
		position.x = leftLimitAnchor.position.x;
		position2.x = rightLimitAnchor.position.x;
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(position, position2);
		for (int i = 0; i < arrayObstacleDrop.Length; i++)
		{
			Vector3 position3 = arrayObstacleDrop[i].dropHeightAnchor.position;
			Vector3 position4 = arrayObstacleDrop[i].dropHeightAnchor.position;
			position3.x = leftLimitAnchor.position.x;
			position4.x = rightLimitAnchor.position.x;
			Gizmos.color = Color.black;
			Gizmos.DrawLine(position3, position4);
		}
		if (Debug_IsSphereCastGizmo && GetIsObstacleDrop())
		{
			Gizmos.color = Color.red;
			Debug_StartPos = new Vector3(arrayLaneAnchor[dropLaneLastLineIdx].transform.position.x, dropPos.y, arrayLaneAnchor[dropLaneLastLineIdx].transform.position.z);
			Gizmos.DrawWireSphere(Debug_StartPos, laneRadius);
		}
	}
}
