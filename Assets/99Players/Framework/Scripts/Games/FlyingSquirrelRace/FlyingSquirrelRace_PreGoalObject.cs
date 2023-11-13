using UnityEngine;
public class FlyingSquirrelRace_PreGoalObject : FlyingSquirrelRace_StageObject
{
	private void OnTriggerEnter(Collider other)
	{
		if (TryGetPlayer(other, out FlyingSquirrelRace_Player player) && !player.IsGoal)
		{
			player.PreGoal();
		}
	}
}
