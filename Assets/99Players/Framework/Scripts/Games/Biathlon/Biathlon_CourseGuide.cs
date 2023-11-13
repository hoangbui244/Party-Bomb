using UnityEngine;
public class Biathlon_CourseGuide : MonoBehaviour
{
	[SerializeField]
	private Collider collider;
	public void Activate()
	{
		base.gameObject.SetActive(value: true);
	}
	public void Deactivate()
	{
		base.gameObject.SetActive(value: false);
	}
	public void IgnoreCollision(Collider col)
	{
		Physics.IgnoreCollision(collider, col);
	}
}
