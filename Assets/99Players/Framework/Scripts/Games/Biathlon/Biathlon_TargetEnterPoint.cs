using UnityEngine;
public class Biathlon_TargetEnterPoint : MonoBehaviour
{
	[SerializeField]
	private Biathlon_Target target;
	public Biathlon_Target Target => target;
	public bool CanEnter()
	{
		return !target.IsUsing;
	}
}
