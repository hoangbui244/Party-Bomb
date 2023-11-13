using UnityEngine;
public class RoadRaceColorCorn : MonoBehaviour
{
	private Rigidbody rigid;
	private Vector3 pluseRigid = new Vector3(0f, 5.5f, 0f);
	private void Start()
	{
		rigid = GetComponent<Rigidbody>();
	}
	private void OnCollisionEnter(Collision _collision)
	{
		if (_collision.gameObject.tag == "Character")
		{
			rigid.velocity += pluseRigid;
			_collision.gameObject.GetComponent<RoadRaceCharacterScript>();
		}
	}
}
