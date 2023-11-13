using UnityEngine;
public class Shooting_TargetAvoidance : MonoBehaviour
{
	private bool onHit;
	[SerializeField]
	[Header("タ\u30fcゲット")]
	private Shooting_Target target;
	public bool OnHit => onHit;
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Ninja"))
		{
			switch (target.TargetType)
			{
			case Shooting_TargetManager.TargetType.TYPE_FIVE_KITES:
			case Shooting_TargetManager.TargetType.TYPE_QUICK:
			case Shooting_TargetManager.TargetType.STRING:
			case Shooting_TargetManager.TargetType.TRAGET:
				break;
			case Shooting_TargetManager.TargetType.TYPE_FALL:
				FallTargetOn();
				break;
			case Shooting_TargetManager.TargetType.TYPE_KITE:
				KiteTargetOn();
				break;
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Ninja"))
		{
			switch (target.TargetType)
			{
			case Shooting_TargetManager.TargetType.TYPE_FIVE_KITES:
			case Shooting_TargetManager.TargetType.TYPE_QUICK:
			case Shooting_TargetManager.TargetType.STRING:
			case Shooting_TargetManager.TargetType.TRAGET:
				break;
			case Shooting_TargetManager.TargetType.TYPE_FALL:
				FallTargetOff();
				break;
			case Shooting_TargetManager.TargetType.TYPE_KITE:
				KiteTargetOff();
				break;
			}
		}
	}
	private void FallTargetOn()
	{
		target.IsWindSide = true;
	}
	private void FallTargetOff()
	{
		target.IsWindSide = false;
	}
	private void KiteTargetOn()
	{
		target.KiteWind /= 3f;
		target.KiteMove = -2f;
	}
	private void KiteTargetOff()
	{
		target.KiteWind *= 3f;
		target.KiteMove = 0f;
	}
}
