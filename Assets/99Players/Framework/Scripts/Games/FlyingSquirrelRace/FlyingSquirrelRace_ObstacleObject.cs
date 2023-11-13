using UnityEngine;
using UnityEngine.Extension;
public abstract class FlyingSquirrelRace_ObstacleObject : FlyingSquirrelRace_StageObject
{
	public enum ObstacleType
	{
		Arrow,
		Caltrap,
		EnemyFuroshiki,
		Pile,
		Shuriken,
		Spear
	}
	[SerializeField]
	[Header("障害物の種類")]
	private ObstacleType obstacleType;
	[SerializeField]
	[DisplayName("ペナルティスコア")]
	private int penaltyScore = 100;
	[SerializeField]
	[DisplayName("スピ\u30fcドペナルティ")]
	private bool isSpeedDown;
	public int PenaltyScore => penaltyScore;
	public bool IsSpeedDown => isSpeedDown;
	public ObstacleType GetObstacleType()
	{
		return obstacleType;
	}
	private void OnTriggerEnter(Collider other)
	{
		OnTriggerEnterMethod(other);
	}
	public void OnTriggerEnterMethod(Collider other, bool isHide = true)
	{
		FlyingSquirrelRace_Player player;
		if (TryGetPlayer(other, out player) && !player.IsGodTime && !player.IsGoal)
		{
			base.Owner.ContactObstacle(this);
			player.ContactObstacle(this);
			if (isHide)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
