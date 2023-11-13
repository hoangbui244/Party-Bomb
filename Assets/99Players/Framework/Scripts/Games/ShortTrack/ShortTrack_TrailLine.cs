using UnityEngine;
public class ShortTrack_TrailLine : MonoBehaviour
{
	[SerializeField]
	private TrailRenderer Line;
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Floor"))
		{
			Line.emitting = true;
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Floor"))
		{
			Line.emitting = false;
		}
	}
}
