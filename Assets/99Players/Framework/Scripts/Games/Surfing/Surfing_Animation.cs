using System;
using UnityEngine;
public class Surfing_Animation : MonoBehaviour
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
		STOCK_L,
		STOCK_R,
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
		ACCEL_RIGHTLEG_FRONT,
		ACCEL_LEFTLEG_FRONT,
		CRASH_RIGHTLEG_FRONT,
		CRASH_LEFTLEG_FRONT,
		JOY,
		SAD,
		WINNER,
		FIGHT
	}
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	public AnimType animType = AnimType.STANDBY;
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
	protected Vector3 rightLeg;
	protected Vector3 leftLeg;
	public AnimType CurrentAnimType => animType;
	public void SetAnim(AnimType _type)
	{
		if (_type != animType)
		{
			animType = _type;
			animationTime = 0f;
			characterMoveTime = 0f;
			runAnimationTime = 0f;
			playSeRunCnt = 0;
			runSeInterval = 0f;
			prevPos = (nowPos = base.transform.position);
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
		bodyParts.rendererList[8].gameObject.transform.localPosition = rightLeg;
		bodyParts.rendererList[7].gameObject.transform.localPosition = leftLeg;
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
		rightLeg = bodyParts.rendererList[8].gameObject.transform.localPosition;
		leftLeg = bodyParts.rendererList[7].gameObject.transform.localPosition;
	}
	private void Update()
	{
		animationTime += Time.deltaTime * animationSpeed;
		characterMoveTime += Time.deltaTime * characterMoveSpeed;
		prevPos = nowPos;
		nowPos = base.transform.position;
		switch (animType)
		{
		case AnimType.NONE:
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
			calcRot[7] = 0f;
			bodyParts.Parts(BodyPartsList.LEG_L).transform.SetLocalPositionZ(0f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, -30f);
			calcRot[8] = 0f;
			bodyParts.Parts(BodyPartsList.LEG_R).transform.SetLocalPositionZ(0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 30f);
			break;
		case AnimType.STANDBY:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[0] = Mathf.SmoothStep(calcRot[0], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalPositionZ(calcRot[7]);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPositionZ(calcRot[8]);
			break;
		case AnimType.ACCEL_RIGHTLEG_FRONT:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[0] = Mathf.SmoothStep(calcRot[0], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 30f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], -30f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], -60f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], -0.04f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalPositionZ(calcRot[7]);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0.04f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPositionZ(calcRot[8]);
			break;
		case AnimType.ACCEL_LEFTLEG_FRONT:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[0] = Mathf.SmoothStep(calcRot[0], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], -30f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], -60f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 30f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0.04f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalPositionZ(calcRot[7]);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], -0.04f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPositionZ(calcRot[8]);
			break;
		case AnimType.CRASH_RIGHTLEG_FRONT:
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 30f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 180f, 180f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], -30f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 180f, 180f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 30f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, -30f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], -30f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 30f);
			break;
		case AnimType.CRASH_LEFTLEG_FRONT:
			calcRot[3] = Mathf.SmoothStep(calcRot[3], -30f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 180f, 180f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 30f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 180f, 180f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], -30f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, -30f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 30f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 30f);
			break;
		case AnimType.JOY:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, calcRot[1], 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[0] = Mathf.SmoothStep(calcRot[0], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], -130f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, calcRot[3]);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, calcRot[4]);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, -30f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 30f);
			break;
		case AnimType.SAD:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 15f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[0] = Mathf.SmoothStep(calcRot[0], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, -30f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 30f);
			break;
		case AnimType.WINNER:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, calcRot[1], 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[0] = Mathf.SmoothStep(calcRot[0], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], -130f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, calcRot[3]);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 130f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, calcRot[4]);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, -30f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 30f);
			break;
		case AnimType.FIGHT:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, calcRot[1], 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[0] = Mathf.SmoothStep(calcRot[0], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], -40f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], -70f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, -30f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 30f);
			break;
		}
	}
}
