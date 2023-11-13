using UnityEngine;
public class RockClimbing_GrapplingHookPoint : MonoBehaviour
{
	[SerializeField]
	[Header("土台を登るプレイヤ\u30fcの種類")]
	private RockClimbing_ClimbingWallManager.ClimbPlayerType climbPlayerType;
	private RockClimbing_ClimbOnFoundation climbOnFoundation;
	private Collider collider;
	public void Init(RockClimbing_ClimbOnFoundation _climbOnFoundation)
	{
		collider = GetComponent<Collider>();
		climbOnFoundation = _climbOnFoundation;
	}
	public bool CheckClimbPlayerType(int _playerNo)
	{
		return climbPlayerType == (RockClimbing_ClimbingWallManager.ClimbPlayerType)_playerNo;
	}
	public RockClimbing_ClimbOnFoundation GetClimbOnFoundation()
	{
		return climbOnFoundation;
	}
	public Collider GetCollider()
	{
		return collider;
	}
	public void SetColliderActive(bool _isActive)
	{
		collider.gameObject.SetActive(_isActive);
	}
}
