using UnityEngine;
public class WaterSpiderRace_RotateTwig : MonoBehaviour
{
	private void Start()
	{
		LeanTween.rotateAround(base.gameObject, Vector3.down, 360f, 10f).setLoopClamp().setDelay(5f * UnityEngine.Random.value);
	}
}
