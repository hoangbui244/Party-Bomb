using System;
using UnityEngine;
public class GS_CharacterAnim : MonoBehaviour
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
		WALK,
		SELECT
	}
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	private AnimType animType = AnimType.WALK;
	private float animationTime;
	private float animationSpeed = 0.85f;
	private float[] calcRot;
	public void SetAnim(AnimType _type)
	{
		if (_type != animType)
		{
			animType = _type;
			animationTime = 0f;
		}
	}
	private void Awake()
	{
		calcRot = new float[bodyParts.rendererList.Length];
	}
	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		animationTime += Time.deltaTime * animationSpeed;
		switch (animType)
		{
		case AnimType.STANDBY:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 7f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 60f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], -50f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], -10f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], -50f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], -30f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 30f, 0.2f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			break;
		case AnimType.WALK:
			calcRot[2] = Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 2.5f - 1.25f;
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 0.01f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 3f, 0.2f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(Mathf.Sin(animationTime * animationSpeed * 0.85f * (float)Math.PI) * 2.5f - 1.25f);
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
		case AnimType.SELECT:
			calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.25f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.25f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			calcRot[3] = Mathf.SmoothStep(calcRot[3], 20f, 0.25f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], -50f, 0.25f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], -170f, 0.25f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.25f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
			calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.25f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
			calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.25f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
			break;
		}
	}
}
