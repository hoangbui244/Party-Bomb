using System;
using UnityEngine;
public class DragonBattleAnim : MonoBehaviour
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
		SWORD_ATTACK,
		SHURIKEN_ATTACK,
		DAMAGE,
		RESPAWN,
		DEATH,
		STAND,
		JOY,
		SAD,
		DASH,
		JUMP,
		CLIMBING,
		DROWN
	}
	[SerializeField]
	[Header("体のパ\u30fcツ")]
	private BodyParts bodyParts;
	[SerializeField]
	[Header("走行エフェクト")]
	private ParticleSystem efMove;
	[SerializeField]
	private Animator animator;
	private AnimType animType = AnimType.STANDBY;
	private string[] array_ani_key = new string[14]
	{
		"",
		"go_standby",
		"go_close_range_attack",
		"go_long_range_attack",
		"go_damage",
		"go_respawn",
		"go_death",
		"go_stand",
		"",
		"",
		"",
		"",
		"",
		""
	};
	private string[] array_ani_name = new string[13]
	{
		"",
		"Stadby",
		"CloseRangeAttack",
		"LongRangeAttack",
		"Damage",
		"Respawn",
		"Death",
		"Stand",
		"",
		"",
		"",
		"",
		""
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
	public AnimType CurrentAnimType => animType;
	public Animator Ani => animator;
	public void SetAnim(AnimType _type)
	{
		if (_type != animType)
		{
			animType = _type;
			switch (animType)
			{
			case AnimType.STANDBY:
			case AnimType.SWORD_ATTACK:
			case AnimType.SHURIKEN_ATTACK:
			case AnimType.STAND:
				animator.SetBool(array_ani_key[(int)animType], value: true);
				break;
			case AnimType.DAMAGE:
			case AnimType.RESPAWN:
			case AnimType.DEATH:
				ResetAnimation(array_ani_name[(int)animType]);
				break;
			}
			animationTime = 0f;
			characterMoveTime = 0f;
			runAnimationTime = 0f;
			playSeRunCnt = 0;
			runSeInterval = 0f;
			prevPos = (nowPos = base.transform.position);
			switch (_type)
			{
			case AnimType.JUMP:
			case AnimType.CLIMBING:
				break;
			case AnimType.DASH:
				animationTime = UnityEngine.Random.Range(0f, 1f);
				break;
			case AnimType.DROWN:
				CalcReset();
				break;
			}
		}
	}
	public void SetAnimSpeed(float _speed)
	{
		animationSpeed = _speed;
		SetAniSpeed(animationSpeed);
	}
	public void SetCharacterSpeed(float _speed)
	{
		characterMoveSpeed = _speed;
	}
	public void Reset()
	{
		ResetAnimation();
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
	}
	private void AniNeutral()
	{
	}
	private void AniStandby()
	{
	}
	private void AniJump()
	{
		bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
		calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
		bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalPosition(-0.158f, 0.096f, 0.006f);
		calcRot[3] = Mathf.SmoothStep(calcRot[3], -70f, 0.15f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 145f, 180f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalPosition(0.158f, 0.102f, 0.039f);
		calcRot[4] = Mathf.SmoothStep(calcRot[4], 70f, 0.15f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 215f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(-40f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(40f, 0f, 0f);
	}
	private void AniDash()
	{
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
		if (Time.frameCount % 10 == 0 && Time.timeScale > 0f)
		{
			efMove.Emit(1);
		}
		if (runAnimationTime >= 1f)
		{
			runAnimationTime = 0f;
			playSeRunCnt = 1;
		}
	}
	private void AniJoy()
	{
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
	}
	private void AniSad()
	{
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
	}
	private void AniSwordAttack()
	{
		calcRot[1] = Mathf.SmoothStep(calcRot[1], 20f, 0.3f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], -10f + calcRot[1], 0f);
		if (calcRot[4] <= 10f)
		{
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 200f, 0.1f);
		}
		else
		{
			calcRot[4] = Mathf.SmoothStep(calcRot[4], 200f, 0.25f);
		}
		bodyParts.Parts(BodyPartsList.SHOULDER_L).transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).transform.localRotation = Quaternion.Euler(-25f, 270f + calcRot[4], 90f);
		calcRot[6] = Mathf.SmoothStep(calcRot[6], 100f, 0.2f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(-90f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
		calcRot[7] = Mathf.SmoothStep(calcRot[7], 45f, 0.3f);
		calcRot[8] = Mathf.SmoothStep(calcRot[8], -45f, 0.3f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
	}
	private void AniClimbing()
	{
		calcRot[1] = Mathf.SmoothStep(calcRot[1], 20f, 0.3f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], -10f + calcRot[1], 0f);
		calcRot[4] = Mathf.SmoothStep(calcRot[4], 200f, 0.2f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).transform.localRotation = Quaternion.Euler(-25f, 270f + calcRot[4], 90f);
		calcRot[6] = Mathf.SmoothStep(calcRot[6], 100f, 0.2f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(-90f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
		calcRot[7] = Mathf.SmoothStep(calcRot[7], 45f, 0.3f);
		calcRot[8] = Mathf.SmoothStep(calcRot[8], -45f, 0.3f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
	}
	private void AniDrown()
	{
		calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
		bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, Mathf.Sin(animationTime * (float)Math.PI * 1.5f) * 20f, 0f);
		bodyParts.Parts(BodyPartsList.HIP).transform.SetLocalPositionY(Mathf.Sin(animationTime * (float)Math.PI * 0.5f) * 0.01f);
		calcRot[3] = Mathf.SmoothStep(calcRot[3], -50f + Mathf.Sin(animationTime * (float)Math.PI * 2f) * 25f, 0.2f);
		calcRot[4] = Mathf.SmoothStep(calcRot[4], -50f + Mathf.Sin(animationTime * (float)Math.PI * 2f) * -25f, 0.2f);
		bodyParts.Parts(BodyPartsList.SHOULDER_L).transform.localRotation = Quaternion.Euler(calcRot[3], 145f, 180f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).transform.localRotation = Quaternion.Euler(calcRot[4], 215f, 180f);
		bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(0f, 0f, 0f);
		bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(0f, 0f, 0f);
		calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.2f);
		calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.2f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
	}
	private void AniDead()
	{
		bodyParts.Parts(BodyPartsList.SHOULDER_L).transform.localRotation = Quaternion.Euler(316f, 200f, 140f);
		bodyParts.Parts(BodyPartsList.SHOULDER_R).transform.localRotation = Quaternion.Euler(297f, 189f, 200f);
		bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(336f, 0f, 340f);
		bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(332f, 0f, 21f);
	}
	public void EmitMoveEffct(int _emitNum)
	{
		efMove.Emit(_emitNum);
	}
	public void ResetAnimation(string _start = "Entry")
	{
		if ((bool)animator)
		{
			for (int i = 0; i < array_ani_key.Length; i++)
			{
				animator.ResetTrigger(array_ani_key[i]);
			}
			animator.Play(_start);
		}
	}
	public void SetAniSpeed(float _speed = 1f)
	{
		animator.speed = _speed;
	}
	public bool CheckPlayingAnim(AnimType _type)
	{
		if (array_ani_name[(int)_type] == "")
		{
			return animType == _type;
		}
		return animator.GetCurrentAnimatorStateInfo(0).IsName(array_ani_name[(int)_type]);
	}
}
