using UnityEngine;
public class DragonBattleDamageZone : MonoBehaviour
{
	[SerializeField]
	[Header("ヒットエフェクト")]
	private ParticleSystem psHitEffect;
	private DragonBattlePlayer target;
	private void OnTriggerStay(Collider other)
	{
		target = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckPlayer(other.gameObject);
		if (target != null)
		{
			target.KnockBack((target.transform.position - base.transform.position).normalized, 22f);
			target.SetVibration();
			SingletonCustom<AudioManager>.Instance.SePlay("se_attack_hit_3");
			psHitEffect.transform.position = target.transform.position;
			psHitEffect.Play();
		}
	}
}
