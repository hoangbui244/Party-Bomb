using UnityEngine;
public class MonsterRace_CarDirtWallCheck : MonoBehaviour
{
	[SerializeField]
	private MonsterRace_CarScript car;
	private void OnTriggerEnter(Collider _col)
	{
		if (_col.gameObject.tag == "VerticalWall")
		{
			car.IsEdgeDirt = true;
		}
	}
	private void OnTriggerExit(Collider _col)
	{
		if (_col.gameObject.tag == "VerticalWall")
		{
			car.IsEdgeDirt = false;
		}
	}
}
