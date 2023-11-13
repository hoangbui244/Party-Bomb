using UnityEngine;
public class DragonBattleFieldClimbableWall : MonoBehaviour
{
	[SerializeField]
	[Header("登頂アンカ\u30fc")]
	private Transform[] topAnchor;
	[SerializeField]
	[Header("登りモ\u30fcション位置アンカ\u30fc（z）")]
	private Transform posAnchor;
	private DragonBattlePlayer target;
	public void OnCollisionStay(Collision collision)
	{
	}
	public void OnCollisionExit(Collision collision)
	{
		target = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckPlayer(collision.gameObject);
		if (target != null && target.CurrentState == DragonBattlePlayer.State.CLIMBING)
		{
			target.FallClimbing();
		}
	}
}
