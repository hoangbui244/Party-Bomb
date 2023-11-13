using UnityEngine;
public class Curiing_FailureCheck : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (!(collision.gameObject.tag == "Ball"))
		{
			return;
		}
		Curling_Stone component = collision.transform.GetComponent<Curling_Stone>();
		if (component != null)
		{
			component.GetRigid().velocity = Vector3.zero;
			component.GetRigid().isKinematic = true;
			if (!component.GetIsFailure())
			{
				component.SetIsFailure();
			}
		}
	}
}
