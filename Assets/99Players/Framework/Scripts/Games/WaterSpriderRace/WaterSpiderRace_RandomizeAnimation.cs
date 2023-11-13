using System.Collections;
using UnityEngine;
public class WaterSpiderRace_RandomizeAnimation : MonoBehaviour
{
	public float starttime;
	private void Start()
	{
		GetComponent<Animator>().speed = 0f;
		StartCoroutine(Randomize());
	}
	private void Update()
	{
	}
	private IEnumerator Randomize()
	{
		yield return new WaitForSeconds(starttime);
		GetComponent<Animator>().speed = 1f;
		UnityEngine.Debug.Log(starttime);
	}
}
