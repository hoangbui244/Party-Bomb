using UnityEngine;
public class WaterSpiderRace_RotateLotus : MonoBehaviour
{
	private void Start()
	{
		base.transform.rotation = Quaternion.Euler(0f, 360f * UnityEngine.Random.value, 0f);
	}
}
