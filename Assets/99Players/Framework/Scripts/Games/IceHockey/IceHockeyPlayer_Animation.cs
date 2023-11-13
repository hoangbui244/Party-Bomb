using System;
using UnityEngine;
public class IceHockeyPlayer_Animation : MonoBehaviour
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
		SLOW_START,
		DASH,
		PREVIEW_WALK,
		JOY,
		SAD,
		WINNER,
		DRIBBLE,
		PASS,
		MOVE,
		CHECK,
		GK_STANDBY,
		GK_CATCH,
		GK_THROW,
		MOVE_REFEREE
	}
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	[SerializeField]
	[Header("スティックのル\u30fcトポイント")]
	private Transform stickRootPoint;
	[SerializeField]
	[Header("走行エフェクト")]
	private ParticleSystem efMove;
	[SerializeField]
	[Header("走行氷エフェクト（左、右）")]
	private ParticleSystem[] arrayEfMoveIce;
	private AnimType animType = AnimType.STANDBY;
	private Vector3[] STICK_POINT_POS = new Vector3[15]
	{
		default(Vector3),
		default(Vector3),
		default(Vector3),
		default(Vector3),
		default(Vector3),
		default(Vector3),
		default(Vector3),
		default(Vector3),
		new Vector3(-0.039f, -0.04f, -0.022f),
		new Vector3(-0.024f, -0.077f, -0.015f),
		new Vector3(-0.213f, -0.075f, -0.016f),
		new Vector3(-0.024f, -0.077f, -0.015f),
		new Vector3(0.097f, 0.013f, 0.084f),
		new Vector3(0.097f, 0.013f, 0.084f),
		new Vector3(0.097f, 0.013f, 0.084f)
	};
	private Vector3[] STICK_POINT_ROT = new Vector3[15]
	{
		default(Vector3),
		new Vector3(-10f, 0f, 0f),
		default(Vector3),
		default(Vector3),
		default(Vector3),
		default(Vector3),
		default(Vector3),
		default(Vector3),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(320.4384f, 352.1694f, 74.38341f),
		new Vector3(355f, 0f, 0f),
		new Vector3(334.01f, 328.5256f, 294.0421f),
		new Vector3(334.01f, 328.5256f, 294.0421f),
		new Vector3(334.01f, 328.5256f, 294.0421f)
	};
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
		if (_type != animType)
		{
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
			case AnimType.PASS:
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
			case AnimType.CHECK:
				calcRot[2] = 0f;
				calcRot[4] = 278.58f;
				break;
			case AnimType.MOVE:
				calcRot[3] = 290f;
				calcRot[4] = 300f;
				bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
				bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
				break;
			}
			if (stickRootPoint != null)
			{
				stickRootPoint.localPosition = STICK_POINT_POS[(int)_type];
				stickRootPoint.SetLocalEulerAngles(STICK_POINT_ROT[(int)_type].x, STICK_POINT_ROT[(int)_type].y, STICK_POINT_ROT[(int)_type].z);
			}
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
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).transform.SetLocalPositionY(0f);
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
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			break;
		case AnimType.STANDBY:
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 10f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.158f, 0.096f, 0.006f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 290f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.158f, 0.102f, 0.039f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 300f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.SLOW_START:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 5f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 6.4f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] += Time.deltaTime * 720f * animationSpeed;
			calcRot[3] %= 360f;
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[5] = 0f;
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] += Time.deltaTime * 720f * animationSpeed;
			calcRot[4] %= 360f;
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			calcRot[6] = 0f;
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], -30f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 30f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
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
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
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
		case AnimType.PREVIEW_WALK:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.HIP).transform.SetLocalPositionY(Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 0.001f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 0.01f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
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
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			if (animationSpeed >= 3.5f)
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
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
			calcRot[7] = Mathf.SmoothStep(calcRot[7], -15f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 15f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
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
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.2f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
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
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			break;
		case AnimType.DRIBBLE:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 10f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, -0.5f + Mathf.Abs(Mathf.Sin(animationTime * (float)Math.PI * 2f) * characterMoveSpeed) * 0.5f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.12f, 0.117f, 0.098f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 292f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 65.6f, 346.716f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.079f, 0.089f, 0.103f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 278.58f, 0.55f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 344.6f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], (characterMoveSpeed <= 0.3f) ? 0f : (-10f + Mathf.Sin(animationTime * (float)Math.PI * 2f) * -3f * characterMoveSpeed), 0.15f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(3f + Mathf.Abs(Mathf.Sin(animationTime * (float)Math.PI) * characterMoveSpeed) * -1f, 0f, calcRot[6]);
			if (characterMoveSpeed < 3f)
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.1f);
			}
			else if (animationSpeed >= 3.5f)
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			if (calcRot[7] >= 30f)
			{
				arrayEfMoveIce[0].Emit(1);
			}
			if (characterMoveSpeed < 3f)
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.1f);
			}
			else if (animationSpeed >= 3.5f)
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			if (calcRot[8] >= -30f)
			{
				arrayEfMoveIce[0].Emit(1);
			}
			break;
		case AnimType.PASS:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 10f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.12f, 0.117f, 0.098f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 375f, 0.25f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAnglesY(calcRot[3]);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.079f, 0.089f, 0.103f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 311f, 0.25f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(278.58f, calcRot[4], 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], -20f, 0.1f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], -20f, 0.1f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			break;
		case AnimType.MOVE:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 10f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, -0.5f + Mathf.Abs(Mathf.Sin(animationTime * (float)Math.PI) * characterMoveSpeed) * 0.5f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.158f, 0.096f, 0.006f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 290f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.158f, 0.102f, 0.039f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 300f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			if (characterMoveSpeed < 3f)
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.1f);
			}
			else if (animationSpeed >= 3.5f)
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			if (calcRot[7] >= 30f)
			{
				arrayEfMoveIce[0].Emit(1);
			}
			if (characterMoveSpeed < 3f)
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.1f);
			}
			else if (animationSpeed >= 3.5f)
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			if (calcRot[8] >= -30f)
			{
				arrayEfMoveIce[1].Emit(1);
			}
			break;
		case AnimType.CHECK:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 30f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(10f, calcRot[1], 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.12f, 0.117f, 0.098f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 292f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 65.6f, 346.716f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.079f, 0.089f, 0.103f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 278.58f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 334.6f, 0f);
			if (characterMoveSpeed < 3f)
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.1f);
			}
			else if (animationSpeed >= 3.5f)
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			if (characterMoveSpeed < 3f)
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.1f);
			}
			else if (animationSpeed >= 3.5f)
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			if (characterMoveSpeed >= 1f && Time.frameCount % 7 == 0 && Time.timeScale > 0f)
			{
				efMove.Emit(1);
			}
			break;
		case AnimType.GK_STANDBY:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 10f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.158f, 0.096f, 0.006f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 355f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(290f, calcRot[3], 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.158f, 0.102f, 0.039f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 5f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(300f, calcRot[4], 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(characterMoveTime * (float)Math.PI * 2f) * -70f * characterMoveSpeed * 0.01f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(characterMoveTime * (float)Math.PI * 2f) * 70f * characterMoveSpeed * 0.01f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			break;
		case AnimType.GK_CATCH:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.158f, 0.096f, 0.006f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 310f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 180f, 180f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.158f, 0.102f, 0.039f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 5f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(300f, calcRot[4], 0f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.GK_THROW:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.158f, 0.096f, 0.006f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 250f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 180f, 180f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.158f, 0.102f, 0.039f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 5f, 0.15f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(300f, calcRot[4], 0f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
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
				calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.1f);
			}
			else if (animationSpeed >= 3.5f)
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			if (calcRot[7] >= 30f)
			{
				arrayEfMoveIce[0].Emit(1);
			}
			if (characterMoveSpeed < 3f)
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.1f);
			}
			else if (animationSpeed >= 3.5f)
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
			}
			else
			{
				calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
			}
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			if (calcRot[8] >= -30f)
			{
				arrayEfMoveIce[1].Emit(1);
			}
			break;
		}
	}
	public void EmitMoveEffct(int _emitNum)
	{
		efMove.Emit(_emitNum);
	}
}
