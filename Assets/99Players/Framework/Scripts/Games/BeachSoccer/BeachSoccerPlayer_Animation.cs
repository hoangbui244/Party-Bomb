using System;
using UnityEngine;
public class BeachSoccerPlayer_Animation : MonoBehaviour
{
	public enum BodyPartsList
	{
		HEAD,
		BODY,
		HIP,
		SHOULDER_L,
		SHOULDER_R,
		ARM_L,
		ARM_R,
		HAND_L,
		HAND_R,
		UP_LEG_L,
		UP_LEG_R,
		LEG_L,
		LEG_R,
		ROOT,
		MAX
	}
	[Serializable]
	public struct BodyParts
	{
		public MeshRenderer[] rendererList;
		public Transform Parts(BodyPartsList _parts)
		{
			return rendererList[(int)_parts].transform;
		}
		public Transform Parts(int _parts)
		{
			return rendererList[_parts].transform;
		}
		public void SetMat(Material _mat)
		{
			for (int i = 0; i < rendererList.Length; i++)
			{
				rendererList[i].GetComponent<MeshRenderer>().material = _mat;
			}
		}
	}
	public enum AnimType
	{
		NONE,
		STANDBY,
		DASH,
		JOY,
		SAD,
		WINNER,
		DRIBBLE,
		KICK,
		THROW_IN_WAIT,
		THROW_IN,
		MOVE,
		SLIDING,
		GK_STANDBY,
		GK_CATCH,
		GK_THROW,
		GK_JUMP,
		MOVE_REFEREE
	}
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	[SerializeField]
	[Header("走行エフェクト")]
	private ParticleSystem efMove;
	private AnimType animType = AnimType.STANDBY;
	private float animationTime;
	private float animationSpeed = 1f;
	private float characterMoveSpeed;
	private float characterMoveTime;
	private float[] calcRot;
	private int footstepIdx;
	private float runAnimationTime;
	protected int playSeRunCnt;
	protected float runSeInterval;
	protected Vector3 prevPos;
	protected Vector3 nowPos;
	private IceHockeyPlayer player;
	public AnimType CurrentAnimType => animType;
	public void SetAnim(AnimType _type)
	{
		UnityEngine.Debug.Log("SetAnim:" + _type.ToString());
		if (_type == animType)
		{
			return;
		}
		switch (animType)
		{
		case AnimType.KICK:
			if (_type == AnimType.STANDBY)
			{
				calcRot[3] = 0f;
				calcRot[4] = 0f;
			}
			break;
		case AnimType.THROW_IN:
			if (_type == AnimType.STANDBY)
			{
				calcRot[3] = 0f;
				calcRot[4] = 0f;
			}
			break;
		}
		animType = _type;
		animationTime = 0f;
		characterMoveTime = 0f;
		runAnimationTime = 0f;
		playSeRunCnt = 0;
		runSeInterval = 0f;
		prevPos = (nowPos = base.transform.position);
		switch (_type)
		{
		case AnimType.DASH:
			animationTime = UnityEngine.Random.Range(0f, 1f);
			break;
		case AnimType.KICK:
			calcRot[3] = 360f;
			break;
		case AnimType.DRIBBLE:
			calcRot[2] = 0f;
			calcRot[1] = 10f;
			break;
		case AnimType.MOVE_REFEREE:
			calcRot[3] = 80f;
			calcRot[4] = -80f;
			break;
		case AnimType.MOVE:
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.GK_THROW:
			calcRot[4] = 340f;
			break;
		}
	}
	public void SetAnimSpeed(float _speed)
	{
		animationSpeed = _speed;
	}
	public void SetCharacterSpeed(float _speed)
	{
		characterMoveSpeed = _speed;
	}
	public void Reset()
	{
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
		animType = AnimType.NONE;
		animationTime = 0f;
	}
	public void CalcReset()
	{
		if (calcRot != null)
		{
			for (int i = 0; i < calcRot.Length; i++)
			{
				calcRot[i] = 0f;
			}
		}
	}
	private void Awake()
	{
		calcRot = new float[bodyParts.rendererList.Length];
	}
	private void Update()
	{
		animationTime += Time.deltaTime * animationSpeed;
		characterMoveTime += Time.deltaTime * characterMoveSpeed * 0.01f;
		prevPos = nowPos;
		nowPos = base.transform.position;
		switch (animType)
		{
		case AnimType.NONE:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[9] = Mathf.SmoothStep(calcRot[9], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(calcRot[9], 0f, 0f);
			calcRot[10] = Mathf.SmoothStep(calcRot[10], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(calcRot[10], 0f, 0f);
			break;
		case AnimType.STANDBY:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.15f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.15f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.15f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HAND_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.15f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HAND_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			calcRot[9] = Mathf.SmoothStep(calcRot[9], 0f, 0.15f);
			calcRot[9] = Mathf.SmoothStep(calcRot[9], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(calcRot[9], 0f, 0f);
			calcRot[10] = Mathf.SmoothStep(calcRot[10], 0f, 0.15f);
			calcRot[10] = Mathf.SmoothStep(calcRot[10], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(calcRot[10], 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.DASH:
			if (animationSpeed >= 3.5f)
			{
				calcRot[2] = Mathf.SmoothStep(calcRot[2], 15f, 0.2f);
			}
			else if (animationSpeed >= 4f)
			{
				calcRot[2] = Mathf.SmoothStep(calcRot[2], 17.5f, 0.2f);
			}
			else
			{
				calcRot[2] = Mathf.SmoothStep(calcRot[2], 2f, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 0.01f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 3f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 2.5f - 1.25f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], -50f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 80f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 60f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], -50f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -80f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -60f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[9] = Mathf.SmoothStep(calcRot[9], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[9] = Mathf.SmoothStep(calcRot[9], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(calcRot[9], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[10] = Mathf.SmoothStep(calcRot[10], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[10] = Mathf.SmoothStep(calcRot[10], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(calcRot[10], 0f, 0f);
			runAnimationTime += CalcManager.Length(nowPos, prevPos) * animationSpeed * Time.deltaTime * 33f;
			if (runAnimationTime >= (float)playSeRunCnt * 0.5f)
			{
				playSeRunCnt++;
				footstepIdx = (footstepIdx + 1) % 2;
			}
			if (runAnimationTime >= 1f)
			{
				runAnimationTime = 0f;
				playSeRunCnt = 1;
			}
			break;
		case AnimType.JOY:
			calcRot[0] = Mathf.SmoothStep(calcRot[0], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], -30f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], calcRot[3] * 0.5f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], -50f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], -60f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], (0f - calcRot[4]) * 0.25f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], -50f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[9] = Mathf.SmoothStep(calcRot[9], -15f, 0.2f);
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(calcRot[9], 0f, 0f);
			calcRot[10] = Mathf.SmoothStep(calcRot[10], 15f, 0.2f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(calcRot[10], 0f, 0f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPositionX(-0.1511f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPositionZ(0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPositionX(0.1511f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPositionZ(0f);
			break;
		case AnimType.SAD:
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 20f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAnglesX(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], -30f, 0.2f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], -30f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).transform.localRotation = Quaternion.Euler(calcRot[3], 0f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).transform.localRotation = Quaternion.Euler(calcRot[4], 0f, 0f);
			calcRot[9] = Mathf.SmoothStep(calcRot[9], 0f, 0.2f);
			calcRot[10] = Mathf.SmoothStep(calcRot[10], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(calcRot[9], 0f, 0f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(calcRot[10], 0f, 0f);
			if (base.transform.up.z >= 0.995f)
			{
				bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPositionX(0.098f);
			}
			if (base.transform.up.z <= -0.995f)
			{
				bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPositionX(-0.098f);
			}
			break;
		case AnimType.WINNER:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], -2.5f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], -170f, 0.15f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], -30f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, calcRot[5]);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], -170f, 0.15f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 30f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, calcRot[6]);
			calcRot[9] = Mathf.SmoothStep(calcRot[9], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(calcRot[9], 0f, 0f);
			calcRot[10] = Mathf.SmoothStep(calcRot[10], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(calcRot[10], 0f, 0f);
			break;
		case AnimType.DRIBBLE:
			if (animationSpeed >= 3.5f)
			{
				calcRot[2] = Mathf.SmoothStep(calcRot[2], 15f, 0.2f);
			}
			else if (animationSpeed >= 4f)
			{
				calcRot[2] = Mathf.SmoothStep(calcRot[2], 17.5f, 0.2f);
			}
			else
			{
				calcRot[2] = Mathf.SmoothStep(calcRot[2], 2f, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			if (animationSpeed < 2f)
			{
				bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(Mathf.Sin(animationTime * animationSpeed * 0.25f * (float)Math.PI) * 0.01f);
			}
			else
			{
				bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 0.01f);
			}
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 3f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			if (animationSpeed < 2f)
			{
				bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(Mathf.Sin(animationTime * animationSpeed * 0.25f * (float)Math.PI) * 2.5f - 1.25f);
			}
			else
			{
				bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 2.5f - 1.25f);
			}
			calcRot[5] = Mathf.SmoothStep(calcRot[5], -50f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 80f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 60f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], -50f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -80f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -60f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[9] = Mathf.SmoothStep(calcRot[9], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[9] = Mathf.SmoothStep(calcRot[9], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(calcRot[9], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[10] = Mathf.SmoothStep(calcRot[10], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[10] = Mathf.SmoothStep(calcRot[10], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(calcRot[10], 0f, 0f);
			runAnimationTime += CalcManager.Length(nowPos, prevPos) * animationSpeed * Time.deltaTime * 33f;
			if (runAnimationTime >= (float)playSeRunCnt * 0.5f)
			{
				playSeRunCnt++;
				switch (footstepIdx)
				{
				case 0:
					SingletonCustom<AudioManager>.Instance.SePlay("se_run");
					break;
				case 1:
					SingletonCustom<AudioManager>.Instance.SePlay("se_run");
					break;
				}
				footstepIdx = (footstepIdx + 1) % 2;
			}
			if (Time.frameCount % 10 == 0 && Time.timeScale > 0f)
			{
				efMove.Emit(1);
			}
			if (runAnimationTime >= 1f)
			{
				runAnimationTime = 0f;
				playSeRunCnt = 1;
			}
			break;
		case AnimType.KICK:
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(330f, 0f, 25f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(25f, 0f, 345f);
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAnglesX(0f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAnglesX(285f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAnglesX(80f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(280f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAnglesX(0f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAnglesX(0f);
			break;
		case AnimType.THROW_IN_WAIT:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], -17.25f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(-17.25f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 150f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(150f, 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 300f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(300f, 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 150f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(150f, 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 300f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(300f, 0f, 0f);
			calcRot[9] = Mathf.SmoothStep(calcRot[9], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			calcRot[10] = Mathf.SmoothStep(calcRot[10], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.THROW_IN:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 10f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 280f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 280f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
			calcRot[9] = Mathf.SmoothStep(calcRot[9], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			calcRot[10] = Mathf.SmoothStep(calcRot[10], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.MOVE:
			if (animationSpeed >= 3.5f)
			{
				calcRot[2] = Mathf.SmoothStep(calcRot[2], 15f, 0.2f);
			}
			else if (animationSpeed >= 4f)
			{
				calcRot[2] = Mathf.SmoothStep(calcRot[2], 17.5f, 0.2f);
			}
			else
			{
				calcRot[2] = Mathf.SmoothStep(calcRot[2], 2f, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			if (animationSpeed < 2f)
			{
				bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(Mathf.Sin(animationTime * animationSpeed * 0.25f * (float)Math.PI) * 0.01f);
			}
			else
			{
				bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 0.01f);
			}
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 3f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			if (animationSpeed < 2f)
			{
				bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(Mathf.Sin(animationTime * animationSpeed * 0.25f * (float)Math.PI) * 2.5f - 1.25f);
			}
			else
			{
				bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 2.5f - 1.25f);
			}
			calcRot[5] = Mathf.SmoothStep(calcRot[5], -50f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 80f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 60f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], -50f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -80f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -60f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[9] = Mathf.SmoothStep(calcRot[9], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[9] = Mathf.SmoothStep(calcRot[9], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(calcRot[9], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[10] = Mathf.SmoothStep(calcRot[10], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[10] = Mathf.SmoothStep(calcRot[10], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(calcRot[10], 0f, 0f);
			runAnimationTime += CalcManager.Length(nowPos, prevPos) * animationSpeed * Time.deltaTime * 33f;
			if (runAnimationTime >= (float)playSeRunCnt * 0.5f)
			{
				playSeRunCnt++;
				footstepIdx = (footstepIdx + 1) % 2;
			}
			if (Time.frameCount % 10 == 0 && Time.timeScale > 0f)
			{
				efMove.Emit(1);
			}
			if (runAnimationTime >= 1f)
			{
				runAnimationTime = 0f;
				playSeRunCnt = 1;
			}
			break;
		case AnimType.SLIDING:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], -40f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 70f, -60f);
			break;
		case AnimType.GK_STANDBY:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 10f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 270f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 355f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 270f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 5f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[9] = Mathf.SmoothStep(calcRot[9], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[9] = Mathf.SmoothStep(calcRot[9], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(calcRot[9], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[10] = Mathf.SmoothStep(calcRot[10], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[10] = Mathf.SmoothStep(calcRot[10], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(calcRot[10], 0f, 0f);
			if (animationSpeed >= 0.25f && Time.frameCount % 10 == 0 && Time.timeScale > 0f)
			{
				efMove.Emit(1);
			}
			break;
		case AnimType.GK_CATCH:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 270f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 270f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.GK_THROW:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 270f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 270f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 180f, 180f);
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.GK_JUMP:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(345f, 180f, 160f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(345f, 180f, 200f);
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.MOVE_REFEREE:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 10f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, -0.5f + Mathf.Abs(Mathf.Sin(animationTime * (float)Math.PI) * characterMoveSpeed) * 0.5f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 80f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 60f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], -50f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -80f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -60f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			if (characterMoveSpeed < 3f)
			{
				calcRot[9] = Mathf.SmoothStep(calcRot[9], 0f, 0.1f);
			}
			else if (animationSpeed >= 3.5f)
			{
				calcRot[9] = Mathf.SmoothStep(calcRot[9], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[9] = Mathf.SmoothStep(calcRot[9], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.UP_LEG_L).SetLocalEulerAngles(calcRot[9], 0f, 0f);
			if (characterMoveSpeed < 3f)
			{
				calcRot[10] = Mathf.SmoothStep(calcRot[10], 0f, 0.1f);
			}
			else if (animationSpeed >= 3.5f)
			{
				calcRot[10] = Mathf.SmoothStep(calcRot[10], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[10] = Mathf.SmoothStep(calcRot[10], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.UP_LEG_R).SetLocalEulerAngles(calcRot[10], 0f, 0f);
			break;
		}
	}
	public void EmitMoveEffct(int _emitNum)
	{
		efMove.Emit(_emitNum);
	}
}
