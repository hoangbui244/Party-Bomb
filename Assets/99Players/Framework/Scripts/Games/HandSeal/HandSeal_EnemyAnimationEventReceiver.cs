using UnityEngine;
using UnityEngine.Events;
public class HandSeal_EnemyAnimationEventReceiver : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem swordTrail;
	[SerializeField]
	private ParticleSystem swordTrail2;
	[SerializeField]
	private UnityEvent attackAction = new UnityEvent();
	public event UnityAction AttackAction;
	public void PlaySwordTrail()
	{
		swordTrail?.Play();
		swordTrail2?.Play();
	}
	public void StopSwordTrail()
	{
		swordTrail?.Stop();
		swordTrail2?.Stop();
	}
	public void ExecuteAttackAtion()
	{
		attackAction.Invoke();
	}
}
