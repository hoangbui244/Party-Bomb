using UnityEngine;
public class RockClimbing_ClimbingWall : MonoBehaviour
{
	[SerializeField]
	[Header("城クラス")]
	private RockClimbing_Castle castle;
	[SerializeField]
	[Header("左側の制限アンカ\u30fc")]
	private Transform leftLimitAnchor;
	[SerializeField]
	[Header("右側の制限アンカ\u30fc")]
	private Transform rightLimitAnchor;
	[SerializeField]
	[Header("上側のライン描画用アンカ\u30fc")]
	private Transform lineUpperAnchor;
	[SerializeField]
	[Header("下側のライン描画用アンカ\u30fc")]
	private Transform lineLowerAnchor;
	public void Init(bool _isFirstPlayer, bool _isReverse)
	{
		castle.Init(_isFirstPlayer, _isReverse);
		SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.SetGoalHeight(castle.GetGoalClimbOnFoundation().GetClimbOnCollider(0).GetCollider()
			.bounds.min.y);
		}
		public void UpdateMethod()
		{
			castle.UpdateMethod();
		}
		public RockClimbing_Castle GetCastle()
		{
			return castle;
		}
		public RockClimbing_ClimbOnFoundation GetStartClimbOnFoundation()
		{
			return castle.GetStartClimbOnFoundation();
		}
		public RockClimbing_ClimbOnFoundationObject_Group GetClimbOnFoundationObjectGroup()
		{
			return castle.GetClimbOnFoundationObjectGroup();
		}
		public RockClimbing_ClimbOnFoundation[] GetArrayClimbOnFoundation()
		{
			return castle.GetArrayClimbOnFoundation();
		}
		public RockClimbing_ClimbOnFoundation[] GetArrayClimbOnFoundationRoof()
		{
			return castle.GetArrayClimbOnFoundationRoof();
		}
		public RockClimbing_Obstacle_Throw_Group GetThrowObstacleGroup(int _playerNo)
		{
			return castle.GetThrowObstacleGroup(_playerNo);
		}
		public void StopThrowObstacle(int _playerNo)
		{
			castle.StopThrowObstacle(_playerNo);
		}
		private void OnDrawGizmos()
		{
			Vector3 position = lineUpperAnchor.position;
			Vector3 position2 = lineLowerAnchor.position;
			position.x = leftLimitAnchor.position.x;
			position2.x = leftLimitAnchor.position.x;
			Vector3 position3 = lineUpperAnchor.position;
			Vector3 position4 = lineLowerAnchor.position;
			position3.x = rightLimitAnchor.position.x;
			position4.x = rightLimitAnchor.position.x;
			Gizmos.color = Color.black;
			Gizmos.DrawLine(position, position2);
			Gizmos.DrawLine(position3, position4);
		}
	}
