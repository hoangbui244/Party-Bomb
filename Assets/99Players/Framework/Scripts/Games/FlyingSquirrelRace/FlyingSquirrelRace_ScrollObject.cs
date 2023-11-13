using UnityEngine;
public class FlyingSquirrelRace_ScrollObject : FlyingSquirrelRace_StageObject
{
	private void OnTriggerEnter(Collider other)
	{
		if (TryGetPlayer(other, out FlyingSquirrelRace_Player player) && !player.IsGoal)
		{
			base.Owner.CollectScroll();
			player.CollectScroll();
			base.gameObject.SetActive(value: false);
		}
	}
}
