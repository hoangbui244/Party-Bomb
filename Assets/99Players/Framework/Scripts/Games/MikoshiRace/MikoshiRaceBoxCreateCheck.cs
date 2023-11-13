using UnityEngine;
public class MikoshiRaceBoxCreateCheck : MonoBehaviour
{
	public MikoshiRaceWaters raceWaters;
	private float countTime;
	private int id;
	private void OnEnable()
	{
		countTime = 0f;
	}
	public void UpdateMethod()
	{
		countTime += Time.deltaTime;
		if (countTime > 7f)
		{
			raceWaters.InitBox(id);
		}
	}
	public void SetID(int _id)
	{
		id = _id;
	}
	public void ResetTime()
	{
		countTime = 0f;
	}
}
