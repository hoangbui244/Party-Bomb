using UnityEngine;
public class Surfing_ObstacleTrigger : MonoBehaviour
{
	[SerializeField]
	[Header("対象のSurfing_Player")]
	public Surfing_Surfer surfer;
	private void OnTriggerStay(Collider other)
	{
		if (!surfer.IsCrash && other.tag == "VerticalWall")
		{
			surfer.ObstacleAction();
		}
	}
}
