using UnityEngine;
public class Shuriken_RemainingTimeUI : MonoBehaviour
{
	[SerializeField]
	private CommonGameTimeUI_Font_Time remainingTime;
	public void Initialize()
	{
		SetRemainingTime(60f);
	}
	public void SetRemainingTime(float time)
	{
		remainingTime.SetTime(time);
	}
}
