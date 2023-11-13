using System;
using System.Collections;
using UnityEngine;
public class BeachFlag_Animation : MonoBehaviour
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
		CHEER,
		CHEER_TWO_HAND,
		STANDBY,
		GETSET,
		RUN,
		RUN_FAST,
		DIVE,
		DIVE_CATCH,
		GOAL_WINNER,
		GOAL_SECOND,
		GOAL_THIRD,
		GOAL_SAD,
		GOAL_AFTER
	}
	public bool dive_done;
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	[SerializeField]
	[Header("対象のプレイヤ\u30fc")]
	private BeachFlag_Player player;
	[SerializeField]
	[Header("キャラクタ\u30fc用ビ\u30fcチフラッグ")]
	private GameObject chara_flag;
	[SerializeField]
	[Header("地面設置用ビ\u30fcチフラッグ")]
	private GameObject ground_flag;
	public AnimType animType = AnimType.STANDBY;
	private float animationInterval = 0.2f;
	private float animationTime;
	private float animationSpeed = 1f;
	private float characterMoveSpeed;
	private float characterMoveTime;
	private float[] calcRot;
	private int footstepIdx;
	private float runAnimationTime;
	private float rot_x;
	private float rot_y;
	protected int playSeRunCnt;
	protected float runSeInterval;
	protected Vector3 prevPos;
	protected Vector3 nowPos;
	protected Vector3 rightLeg;
	protected Vector3 leftLeg;
	private bool isRightStep;
	private bool isCorPlay;
	private Vector3 hip_rot;
	private Vector3 flag_rot;
	private Vector3 flag_pos;
	private Vector3 shoulder_rot;
	private float height;
	public AnimType CurrentAnimType => animType;
	public float AnimationInterval => animationInterval;
	public float AnimTime
	{
		get
		{
			return animationTime;
		}
		set
		{
			animationTime = value;
		}
	}
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
		chara_flag.GetComponentInChildren<MeshRenderer>().enabled = false;
		ground_flag.GetComponentInChildren<MeshRenderer>().enabled = true;
	}
	private void Update()
	{
		animationTime += Time.deltaTime * animationSpeed;
		characterMoveTime += Time.deltaTime * characterMoveSpeed;
		prevPos = nowPos;
		nowPos = base.transform.position;
		UnityEngine.Debug.Log("ANIMATION TYPE : " + animType.ToString());
		switch (animType)
		{
		case AnimType.RUN_FAST:
		case AnimType.GOAL_SECOND:
		case AnimType.GOAL_THIRD:
			break;
		case AnimType.NONE:
			dive_done = false;
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.CHEER:
			dive_done = false;
			calcRot[5] = Mathf.SmoothStep(calcRot[5], -90f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -20f * animationSpeed - 150f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			break;
		case AnimType.CHEER_TWO_HAND:
			dive_done = false;
			calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -20f * animationSpeed - 150f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
			calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -20f * animationSpeed - 150f, 0.2f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
			break;
		case AnimType.STANDBY:
			dive_done = false;
			chara_flag.GetComponentInChildren<MeshRenderer>().enabled = false;
			ground_flag.GetComponentInChildren<MeshRenderer>().enabled = true;
			height = Mathf.SmoothStep(height, 0.1f, 0.2f);
			hip_rot = new Vector3(Mathf.SmoothStep(hip_rot.x, 90f, 0.2f), Mathf.SmoothStep(hip_rot.y, 180f, 0.2f), 0f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(hip_rot.x, hip_rot.y, hip_rot.z);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, height, 0f);
			bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
			calcRot[0] = Mathf.SmoothStep(calcRot[0], 311f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
			shoulder_rot = new Vector3(bodyParts.Parts(BodyPartsList.SHOULDER_L).rotation.x, bodyParts.Parts(BodyPartsList.SHOULDER_L).rotation.y, bodyParts.Parts(BodyPartsList.SHOULDER_L).rotation.z);
			shoulder_rot = new Vector3(Mathf.SmoothStep(shoulder_rot.x, 320f, 0.2f), Mathf.SmoothStep(shoulder_rot.y, 180f, 0.2f), Mathf.SmoothStep(shoulder_rot.z, 180f, 0.2f));
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(shoulder_rot.x * 10f, shoulder_rot.y * 10f, shoulder_rot.z * 10f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(shoulder_rot.x * 10f, shoulder_rot.y * 10f, shoulder_rot.z * 10f);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
			shoulder_rot = new Vector3(bodyParts.Parts(BodyPartsList.STOCK_L).rotation.x, bodyParts.Parts(BodyPartsList.STOCK_L).rotation.y, bodyParts.Parts(BodyPartsList.STOCK_L).rotation.z);
			shoulder_rot = new Vector3(Mathf.SmoothStep(shoulder_rot.x, 320f, 0.2f), Mathf.SmoothStep(shoulder_rot.y, 0f, 0.2f), Mathf.SmoothStep(shoulder_rot.z, 0f, 0.2f));
			bodyParts.Parts(BodyPartsList.STOCK_L).SetLocalEulerAngles(shoulder_rot.x * 10f, shoulder_rot.y * 10f, shoulder_rot.z * 10f);
			bodyParts.Parts(BodyPartsList.STOCK_R).SetLocalEulerAngles(shoulder_rot.x * 10f, shoulder_rot.y * 10f, shoulder_rot.z * 10f);
			break;
		case AnimType.GETSET:
			if (bodyParts.Parts(BodyPartsList.HIP).localEulerAngles.y <= 10f)
			{
				SetAnim(AnimType.RUN);
			}
			height = Mathf.SmoothStep(height, 0.14f, 0.2f);
			hip_rot = new Vector3(Mathf.SmoothStep(hip_rot.x, 2f, 0.2f), Mathf.SmoothStep(hip_rot.y, 0f, 0.2f), 0f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(hip_rot.x, hip_rot.y, hip_rot.z);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, height, 0f);
			calcRot[0] = Mathf.SmoothStep(calcRot[0], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
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
			break;
		case AnimType.RUN:
			calcRot[0] = Mathf.SmoothStep(calcRot[0], 0f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
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
			break;
		case AnimType.DIVE:
		{
			player.characterStyle.SetMainCharacterFaceDiff((int)player.UserType, StyleTextureManager.MainCharacterFaceType.NORMAL);
			Vector3 eulerAngles = bodyParts.Parts(BodyPartsList.HIP).eulerAngles;
			UnityEngine.Debug.Log("DIVE ANIM ROT X " + eulerAngles.x.ToString());
			if (bodyParts.Parts(BodyPartsList.HIP).eulerAngles.x >= 70f)
			{
				UnityEngine.Debug.Log("ダイブ終わった");
				SetAnim(AnimType.DIVE_CATCH);
			}
			hip_rot = new Vector3(Mathf.SmoothStep(hip_rot.x, 90f, 0.2f), Mathf.SmoothStep(hip_rot.y, 0f, 0.2f), 0f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(hip_rot.x, hip_rot.y, hip_rot.z);
			shoulder_rot = new Vector3(bodyParts.Parts(BodyPartsList.SHOULDER_L).rotation.x, bodyParts.Parts(BodyPartsList.SHOULDER_L).rotation.y, bodyParts.Parts(BodyPartsList.SHOULDER_L).rotation.z);
			shoulder_rot = new Vector3(Mathf.SmoothStep(shoulder_rot.x, 336f, 0.2f), Mathf.SmoothStep(shoulder_rot.y, 180f, 0.2f), Mathf.SmoothStep(shoulder_rot.z, 180f, 0.2f));
			bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(shoulder_rot.x, shoulder_rot.y, shoulder_rot.z);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
			shoulder_rot = new Vector3(bodyParts.Parts(BodyPartsList.SHOULDER_R).rotation.x, bodyParts.Parts(BodyPartsList.SHOULDER_R).rotation.y, bodyParts.Parts(BodyPartsList.SHOULDER_R).rotation.z);
			shoulder_rot = new Vector3(Mathf.SmoothStep(shoulder_rot.x, 284.51f, 0.2f), Mathf.SmoothStep(shoulder_rot.y, -1.040894E-06f, 0.2f), Mathf.SmoothStep(shoulder_rot.z, 1.808117E-05f, 0.2f));
			bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(shoulder_rot.x, shoulder_rot.y, shoulder_rot.z);
			bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		}
		case AnimType.DIVE_CATCH:
			if (bodyParts.Parts(BodyPartsList.ARM_L).localEulerAngles.x >= 180f)
			{
				StartCoroutine(EnableWalk());
			}
			chara_flag.GetComponentInChildren<MeshRenderer>().enabled = true;
			ground_flag.GetComponentInChildren<MeshRenderer>().enabled = false;
			flag_rot = new Vector3(Mathf.SmoothStep(flag_rot.x, 0f, 0.2f), Mathf.SmoothStep(flag_rot.y, 0f, 0.2f), Mathf.SmoothStep(flag_rot.x, 0f, 0.2f));
			chara_flag.transform.localEulerAngles = new Vector3(flag_rot.x, flag_rot.y, flag_rot.z);
			player.characterStyle.SetMainCharacterFaceDiff((int)player.UserType, StyleTextureManager.MainCharacterFaceType.HAPPY);
			hip_rot = new Vector3(Mathf.SmoothStep(hip_rot.x, 90f, 0.2f), Mathf.SmoothStep(hip_rot.y, 0f, 0.2f), 0f);
			bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(hip_rot.x, hip_rot.y, hip_rot.z);
			calcRot[0] = Mathf.SmoothStep(calcRot[0], -60f, 0.2f);
			bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
			calcRot[5] = Mathf.SmoothStep(calcRot[5], 200f, 0.2f);
			bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.GOAL_WINNER:
			player.characterStyle.SetMainCharacterFaceDiff((int)player.UserType, StyleTextureManager.MainCharacterFaceType.HAPPY);
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
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.GOAL_SAD:
			player.characterStyle.SetMainCharacterFaceDiff((int)player.UserType, StyleTextureManager.MainCharacterFaceType.SAD);
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
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			break;
		case AnimType.GOAL_AFTER:
			player.characterStyle.SetMainCharacterFaceDiff((int)player.UserType, StyleTextureManager.MainCharacterFaceType.HAPPY);
			flag_rot = new Vector3(Mathf.SmoothStep(flag_rot.x, 40f, 0.2f), Mathf.SmoothStep(flag_rot.y, 260f, 0.2f), Mathf.SmoothStep(flag_rot.x, 100f, 0.2f));
			chara_flag.transform.localEulerAngles = new Vector3(flag_rot.x, flag_rot.y, flag_rot.z);
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
			bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(0f, 0f, 0f);
			bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(0f, 0f, 0f);
			dive_done = true;
			break;
		}
	}
	private IEnumerator EnableWalk()
	{
		yield return new WaitForSeconds(1f);
		SetAnim(AnimType.GOAL_AFTER);
	}
}
