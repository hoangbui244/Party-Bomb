using UnityEngine;
public class Shooting_HitPoint : MonoBehaviour
{
	[SerializeField]
	[Header("HitPointアンカ\u30fc情報")]
	private Shooting_TargetManager.HitPointType hitPointType;
	public Shooting_TargetManager.HitPointType HitPointType => hitPointType;
}
