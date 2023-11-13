using System;
using UnityEngine;
public class SmartBall_Al : MonoBehaviour
{
	public enum AIState
	{
		Wait,
		ChargePower,
		ShotBall
	}
	[SerializeField]
	[Header("キャラクタ\u30fc")]
	private SmartBall_Character character;
	private SB.AiStrength aiStrength;
	private AIState currentAIState;
	private float AI_CHARGE_TIME = 1.2f;
	private float aiChargeTime;
	private readonly float DEFAULT_CHARGE_TIME = 0.5f;
	public float cpuChargTimeError;
	public void Init()
	{
		aiStrength = (SB.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		switch (aiStrength)
		{
		case SB.AiStrength.WEAK:
			cpuChargTimeError = SB.WEAK_CHARGE_TIME_ERROR;
			break;
		case SB.AiStrength.COMMON:
			cpuChargTimeError = SB.COMMON_CHARGE_TIME_ERROR;
			break;
		case SB.AiStrength.STRONG:
			cpuChargTimeError = SB.STRONG_CHARGE_TIME_ERROR;
			break;
		}
	}
	public void UpdateMethod()
	{
		if (character.CurrentActionState == SmartBall_Character.ActionState.Shot && !character.IsShot)
		{
			switch (currentAIState)
			{
			case AIState.Wait:
				AIWaitProcess();
				break;
			case AIState.ChargePower:
				AIChargePower();
				break;
			case AIState.ShotBall:
				AIShotBall();
				break;
			}
		}
	}
	private void AIWaitProcess()
	{
		aiChargeTime = SetAIChargeTime();
		if (aiChargeTime > 0f && character.CheckSetBall())
		{
			currentAIState = AIState.ChargePower;
		}
	}
	private void AIChargePower()
	{
		if (aiChargeTime > 0f && character.CheckSetBall())
		{
			aiChargeTime -= Time.deltaTime;
			character.GetCharaStand().PullShotStick();
			character.ChargeShotPower();
		}
		else
		{
			currentAIState = AIState.ShotBall;
		}
	}
	private void AIShotBall()
	{
		StartCoroutine(character.GetCharaStand()._StopperMove());
		character.GetCharaStand().BallShotStick();
		LeanTween.delayedCall(character.GetCharaStand().GetStickShotTime(), (Action)delegate
		{
			character.BallShot();
		});
		currentAIState = AIState.Wait;
	}
	public float SetAIChargeTime()
	{
		float num = DEFAULT_CHARGE_TIME;
		if (UnityEngine.Random.Range(0, 100) < 50)
		{
			num += 0.25f;
		}
		return num + UnityEngine.Random.Range(0f - cpuChargTimeError, cpuChargTimeError);
	}
}
