using UnityEngine;
public class MakingPotion_Items : MonoBehaviour
{
	private Rigidbody rigid;
	private void Start()
	{
		rigid = GetComponent<Rigidbody>();
	}
	private void Update()
	{
	}
	private void OnTriggerEnter(Collider other)
	{
	}
	private void OnTriggerExit(Collider other)
	{
	}
}
