using System;
using UnityEngine;
public class Golf_AI : MonoBehaviour
{
	private Golf_Player player;
	private int aiStrength;
	private float rotDir;
	private float shotAngle;
	private Vector3 shotVec;
	private Vector3 hitPointDir;
	private readonly float DIFF_SHOT_READY_ROT_SPEED = 0.25f;
	private readonly float READY_ROT_LIMIT_ANGLE = 5f;
	private readonly float SHOT_ANGLE_RANGE = 1f;
	private readonly float SHOT_READY_INPUT_INTERVAL = 3f;
	private readonly float SHOT_POWER_INPUT_INTERVAL = 2f;
	private float inputInterval;
	private readonly float ALWAYS_CHECK_ANGLE_INTERVAL = 1.5f;
	private float alwaysCheckAngleInterval;
	private readonly float RANDOM_SHOT_READY_INTERVAL = 0.25f;
	private float randomShotReadyInterval;
	private float SHOT_ANGLE;
	private float SHOT_POWER_LERP;
	private float SHOT_IMPACT_LERP;
	public void Init(Golf_Player _player)
	{
		player = _player;
		aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
	}
	public void InitPlay()
	{
		Vector3 diffVec = SingletonCustom<Golf_BallManager>.Instance.GetPredictionBall().GetDiffVec(Golf_PredictionBall.PredictionType.Wind);
		float[] array = Golf_Define.CPU_SHOT_ANGLE[aiStrength];
		SHOT_ANGLE = UnityEngine.Random.Range(array[0], array[1]);
		SetShotReadyDir(diffVec);
		float[] array2 = Golf_Define.CPU_SHOT_POWER[aiStrength];
		SHOT_POWER_LERP = UnityEngine.Random.Range(array2[0], array2[1]);
		float[] array3 = Golf_Define.CPU_SHOT_IMPACT[aiStrength];
		SHOT_IMPACT_LERP = UnityEngine.Random.Range(array3[0], array3[1]);
		SetHitPoint(diffVec);
		inputInterval = UnityEngine.Random.Range(SHOT_READY_INPUT_INTERVAL - 1f, SHOT_READY_INPUT_INTERVAL + 1f);
		alwaysCheckAngleInterval = ALWAYS_CHECK_ANGLE_INTERVAL;
		randomShotReadyInterval = -1f;
	}
	public void UpdateMethod()
	{
		switch (SingletonCustom<Golf_GameManager>.Instance.GetState())
		{
		case Golf_GameManager.State.SHOT_READY:
			ShotReady();
			break;
		case Golf_GameManager.State.SHOT_POWER:
			ShotPower();
			break;
		case Golf_GameManager.State.SHOT_IMPACT:
			ShotVec();
			break;
		}
	}
	private void SetShotReadyDir(Vector3 _windDiffVec)
	{
		float num = Mathf.Atan2(_windDiffVec.z, _windDiffVec.x) * 57.29578f;
		UnityEngine.Debug.Log("風の影響だけを考慮した角度 windDiffAngle " + num.ToString());
		shotAngle = num + SHOT_ANGLE;
		float f = shotAngle * ((float)Math.PI / 180f);
		shotVec = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		UnityEngine.Debug.Log("風の影響に ランダム角度 を加算した角度 shotAngle " + shotAngle.ToString());
		UnityEngine.Debug.DrawRay(SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos(), shotVec * SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetReadyBallPosToCupVec().magnitude, Color.yellow, 15f);
	}
	private void ShotReady()
	{
		float num = Vector3.Angle(shotVec, -base.transform.right);
		Vector3 vector = Vector3.Cross(shotVec, -base.transform.right);
		if (inputInterval > 0f)
		{
			inputInterval -= Time.deltaTime;
			if (randomShotReadyInterval > 0f)
			{
				randomShotReadyInterval -= Time.deltaTime;
			}
			else
			{
				randomShotReadyInterval = RANDOM_SHOT_READY_INTERVAL;
				rotDir = ((num < READY_ROT_LIMIT_ANGLE) ? ((float)UnityEngine.Random.Range(-1, 2)) : Mathf.Sign(vector.y));
			}
		}
		else
		{
			rotDir = ((num <= SHOT_ANGLE_RANGE) ? 0f : Mathf.Sign(vector.y));
			alwaysCheckAngleInterval -= Time.deltaTime;
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 readyBallPos = SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos();
		Vector3 vector2 = new Vector3(0f, 0f - rotDir, 0f);
		base.transform.RotateAround(readyBallPos, vector2, SingletonCustom<Golf_PlayerManager>.Instance.GetShotReadyRotSpeed() * Time.deltaTime * DIFF_SHOT_READY_ROT_SPEED);
		if (base.transform.localEulerAngles.y > player.GetOriginRotY() + SingletonCustom<Golf_PlayerManager>.Instance.GetReadyRotLimitAngle())
		{
			base.transform.localPosition = localPosition;
			base.transform.SetLocalEulerAnglesY(player.GetOriginRotY() + SingletonCustom<Golf_PlayerManager>.Instance.GetReadyRotLimitAngle());
			vector2 = Vector3.zero;
		}
		else if (base.transform.localEulerAngles.y < player.GetOriginRotY() - SingletonCustom<Golf_PlayerManager>.Instance.GetReadyRotLimitAngle())
		{
			base.transform.localPosition = localPosition;
			base.transform.SetLocalEulerAnglesY(player.GetOriginRotY() - SingletonCustom<Golf_PlayerManager>.Instance.GetReadyRotLimitAngle());
			vector2 = Vector3.zero;
		}
		if (vector2 != Vector3.zero)
		{
			SingletonCustom<Golf_CameraManager>.Instance.ShotReadyRot(vector2, DIFF_SHOT_READY_ROT_SPEED);
		}
		if ((inputInterval <= 0f && num <= SHOT_ANGLE_RANGE) || alwaysCheckAngleInterval <= 0f)
		{
			player.SetShotReady();
			inputInterval = SHOT_POWER_INPUT_INTERVAL;
		}
	}
	private void SetHitPoint(Vector3 _windDiffVec)
	{
		Vector3 diffVec = SingletonCustom<Golf_BallManager>.Instance.GetPredictionBall().GetDiffVec(Golf_PredictionBall.PredictionType.Rot);
		Vector3 readyBallPosToCupVec = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetReadyBallPosToCupVec();
		float num = Vector3.Angle(readyBallPosToCupVec, _windDiffVec);
		float num2 = Vector3.Angle(readyBallPosToCupVec, diffVec);
		float num3 = Vector3.Angle(readyBallPosToCupVec, shotVec);
		UnityEngine.Debug.Log("windDiffAngle " + num.ToString());
		UnityEngine.Debug.Log("rotDiffAngle " + num2.ToString());
		UnityEngine.Debug.Log("shotAngle " + num3.ToString());
		UnityEngine.Debug.Log("Mathf.Abs(shotAngle - windDiffAngle) " + Mathf.Abs(num3 - num).ToString());
		float num4 = num3 / num2;
		if (Mathf.Abs(num3 - num) > num2)
		{
			num4 = 1f;
		}
		if (Vector3.Cross(_windDiffVec, shotVec).y < 0f)
		{
			num4 *= -1f;
		}
		UnityEngine.Debug.Log("x " + num4.ToString());
		float y = 0f;
		float cupPowerLerp = SingletonCustom<Golf_UIManager>.Instance.GetCupPowerLerp();
		float num5 = cupPowerLerp;
		float num6 = cupPowerLerp;
		if (SHOT_POWER_LERP < cupPowerLerp)
		{
			num5 = cupPowerLerp - 0.2f;
			y = (num6 - SHOT_POWER_LERP) / (num6 - num5);
		}
		else if (SHOT_POWER_LERP > cupPowerLerp)
		{
			num6 = cupPowerLerp + 0.2f;
			y = (1f - (num6 - SHOT_POWER_LERP) / (num6 - num5)) * -1f;
		}
		hitPointDir = new Vector3(num4, y, 0f);
	}
	private void ShotPower()
	{
		if (inputInterval > 0f)
		{
			inputInterval -= Time.deltaTime;
		}
		else if (Mathf.Clamp(SingletonCustom<Golf_UIManager>.Instance.GetShotPowerLerp(), 0.2f, 1f) >= SHOT_POWER_LERP)
		{
			player.SetShotPower();
		}
	}
	private void ShotVec()
	{
		SingletonCustom<Golf_UIManager>.Instance.MoveHitPoint(hitPointDir);
		player.SetStickDir(hitPointDir);
		if (SingletonCustom<Golf_UIManager>.Instance.GetImpactDiff() <= SHOT_IMPACT_LERP)
		{
			player.SetShotVec();
			player.SetSwingAnimation();
		}
	}
	public Vector3 GetShotVec()
	{
		return shotVec;
	}
	public Vector3 GetHitPointDir()
	{
		return hitPointDir;
	}
	public float GetShotPowerLerp()
	{
		return SHOT_POWER_LERP;
	}
	public float GetShotImpctLerp()
	{
		return SHOT_IMPACT_LERP;
	}
}
