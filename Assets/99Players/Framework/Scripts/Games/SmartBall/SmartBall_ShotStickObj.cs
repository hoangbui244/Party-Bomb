using System;
using UnityEngine;
public class SmartBall_ShotStickObj : MonoBehaviour
{
	private readonly float STICK_SHOT_TIME = 0.1f;
	private readonly float STICK_MOVE_START = 4f;
	private readonly float STICK_MOVE_LIMIT = 6.4f;
	private bool playerUseStand;
	private readonly float STICK_SPEED = 3f;
	public float StickShotTime => STICK_SHOT_TIME;
	public void PullShotStick()
	{
		Vector3 localPosition = base.transform.parent.localPosition;
		localPosition.z += STICK_SPEED * Time.deltaTime;
		localPosition.z = Mathf.Clamp(localPosition.z, STICK_MOVE_START, STICK_MOVE_LIMIT);
		base.transform.parent.transform.localPosition = localPosition;
	}
	public void BallShotStick()
	{
		LeanTween.moveLocalZ(base.transform.parent.gameObject, STICK_MOVE_START, STICK_SHOT_TIME).setOnComplete((Action)delegate
		{
			if (playerUseStand)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_smartball_shot_1");
			}
			ShotStickBehavior();
		});
	}
	private void ShotStickBehavior()
	{
		LeanTween.moveLocalZ(base.transform.parent.gameObject, STICK_MOVE_START + 0.075f, STICK_SHOT_TIME).setOnComplete((Action)delegate
		{
			LeanTween.moveLocalZ(base.transform.parent.gameObject, STICK_MOVE_START, STICK_SHOT_TIME);
		});
	}
	public bool CheckSetBall()
	{
		if (base.transform.childCount != 0)
		{
			return true;
		}
		return false;
	}
	public void CheckPlayerStand(bool _isPlayer)
	{
		playerUseStand = _isPlayer;
	}
	public SmartBall_BallObject GetShotBallObj()
	{
		return base.transform.GetChild(0).GetComponent<SmartBall_BallObject>();
	}
}
