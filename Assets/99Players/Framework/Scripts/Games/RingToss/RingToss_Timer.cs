using UnityEngine;
public class RingToss_Timer : MonoBehaviour
{
	private float GAME_TIME = 60f;
	private float defTime;
	private bool isBgmPitchUp;
	public float RemainingTime => GAME_TIME;
	public void Init()
	{
		defTime = GAME_TIME;
		ChangeTime();
	}
	public void SecondGroupInit()
	{
		GAME_TIME = defTime;
		ChangeTime();
	}
	private void ChangeTime()
	{
		SingletonCustom<RingToss_UIManager>.Instance.UpdateTimeUI(GAME_TIME);
	}
	private void UpdateTime()
	{
		ChangeTime();
	}
	public void UpdateMethod()
	{
		GAME_TIME -= Time.deltaTime;
		if (GAME_TIME < 0f)
		{
			GAME_TIME = 0f;
			SingletonCustom<RingToss_GameManager>.Instance.GameEnd();
		}
		UpdateTime();
	}
}
