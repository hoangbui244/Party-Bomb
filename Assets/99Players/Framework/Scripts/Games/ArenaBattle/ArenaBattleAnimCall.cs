using UnityEngine;
public class ArenaBattleAnimCall : MonoBehaviour
{
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private ArenaBattlePlayer player;
	public void OnAttackStart()
	{
		player.OnAttackStart();
	}
	public void OnAttackEnd()
	{
		player.OnAttackEnd();
	}
	public void OnDodgeEnd()
	{
		player.OnDodgeEnd();
	}
}
