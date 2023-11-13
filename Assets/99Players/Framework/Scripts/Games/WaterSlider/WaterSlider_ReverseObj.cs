using UnityEngine;
public class WaterSlider_ReverseObj : MonoBehaviour
{
	[SerializeField]
	[Header("コライダ\u30fc")]
	private BoxCollider collider;
	[SerializeField]
	[Header("接触回数")]
	private int throwCount;
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.name.Contains("MoveObj"))
		{
			throwCount++;
		}
	}
	public int GetThrowCount()
	{
		return throwCount;
	}
	public BoxCollider GetBoxCollider()
	{
		return collider;
	}
}
