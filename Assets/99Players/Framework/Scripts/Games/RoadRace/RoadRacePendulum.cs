using UnityEngine;
public class RoadRacePendulum : MonoBehaviour
{
	[SerializeField]
	private float speed = 1f;
	[SerializeField]
	private float maxRot = 30f;
	private float timer;
	private void FixedUpdate()
	{
		timer += Time.fixedDeltaTime * speed;
		base.transform.SetLocalEulerAnglesZ(Mathf.Sin(timer) * maxRot);
	}
}
