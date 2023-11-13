using UnityEngine;
public class MikoshiRaceCarReverseChecker : MonoBehaviour
{
	[SerializeField]
	private MikoshiRaceCarScript car;
	[SerializeField]
	private SphereCollider sphereCollider;
	public void Init()
	{
		base.gameObject.SetActive(car.IsPlayer);
	}
	private void OnTriggerStay(Collider _col)
	{
		if (!(_col.tag == "School"))
		{
			return;
		}
		Rigidbody rigid = car.GetRigid();
		Vector3 velocity = rigid.velocity;
		Vector3 forward = _col.transform.forward;
		if (!(Vector3.Dot(velocity, forward) < 0f))
		{
			return;
		}
		velocity = (rigid.velocity = velocity - Vector3.Dot(velocity, forward) * forward);
		if (forward.z > 0.5f)
		{
			if (car.transform.position.z < _col.transform.position.z + sphereCollider.radius)
			{
				car.transform.SetPositionZ(_col.transform.position.z + sphereCollider.radius);
			}
		}
		else if (forward.z < -0.5f)
		{
			if (car.transform.position.z > _col.transform.position.z - sphereCollider.radius)
			{
				car.transform.SetPositionZ(_col.transform.position.z - sphereCollider.radius);
			}
		}
		else if (forward.x > 0.5f)
		{
			if (car.transform.position.x < _col.transform.position.x + sphereCollider.radius)
			{
				car.transform.SetPositionX(_col.transform.position.x + sphereCollider.radius);
			}
		}
		else if (car.transform.position.x > _col.transform.position.x - sphereCollider.radius)
		{
			car.transform.SetPositionX(_col.transform.position.x - sphereCollider.radius);
		}
	}
}
