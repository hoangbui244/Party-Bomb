using UnityEngine;
public class DragonBattleGoal : MonoBehaviour
{
	private DragonBattlePlayer target;
	public void OnTriggerEnter(Collider other)
	{
		target = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckPlayer(other.gameObject);
		if (target != null)
		{
			target.OnGoal();
		}
	}
}
