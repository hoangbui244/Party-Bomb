using System;
using UnityEngine;
public class ArenaBattlePlayer_Animation : MonoBehaviour
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
		DASH,
		JUMP,
		JOY,
		SAD,
		SWORD_ATTACK_0,
		SWORD_ATTACK_1,
		SWORD_ATTACK_2,
		SWORD_ATTACK_SP,
		MAGIC_CAST,
		MAGIC_ATTACK,
		DODGE,
		CLIMBING,
		DROWN,
		DEAD,
		DAMAGE,
		VICTORY
	}
	[SerializeField]
	[Header("アニメ\u30fcタ\u30fc")]
	private Animator anim;
	[SerializeField]
	[Header("走行エフェクト")]
	private ParticleSystem efMove;
	private AnimType animType = AnimType.STANDBY;
	private float animationTime;
	private float animationSpeed = 1f;
	private float characterMoveSpeed;
	private float characterMoveTime;
	private int footstepIdx;
	private float runAnimationTime;
	protected int playSeRunCnt;
	protected float runSeInterval;
	protected Vector3 prevPos;
	protected Vector3 nowPos;
	public AnimType CurrentAnimType => animType;
	public void SetBool(AnimType _type, bool _enable)
	{
		switch (_type)
		{
		case AnimType.JOY:
		case AnimType.SAD:
		case AnimType.SWORD_ATTACK_SP:
		case AnimType.CLIMBING:
		case AnimType.DROWN:
			break;
		case AnimType.JUMP:
			anim.SetBool("Jump", _enable);
			break;
		case AnimType.SWORD_ATTACK_0:
			anim.SetBool("SwordAttack_0", _enable);
			efMove.Emit(5);
			break;
		case AnimType.SWORD_ATTACK_1:
			anim.SetBool("SwordAttack_1", _enable);
			efMove.Emit(5);
			break;
		case AnimType.SWORD_ATTACK_2:
			anim.SetBool("SwordAttack_2", _enable);
			efMove.Emit(5);
			break;
		case AnimType.MAGIC_CAST:
			anim.SetBool("MagicCast", _enable);
			break;
		case AnimType.MAGIC_ATTACK:
			anim.SetBool("MagicAttack", _enable);
			break;
		case AnimType.DODGE:
			anim.SetBool("Dodge", _enable);
			efMove.Emit(5);
			break;
		case AnimType.DEAD:
			anim.SetBool("Dead", _enable);
			break;
		}
	}
	public void SetTrigger(AnimType _type)
	{
		switch (_type)
		{
		case AnimType.DAMAGE:
			anim.SetTrigger("Damage");
			break;
		case AnimType.VICTORY:
			anim.SetTrigger("Victory");
			break;
		case AnimType.SWORD_ATTACK_SP:
			anim.SetTrigger("Sp");
			break;
		}
	}
	public void HitStop()
	{
		anim.speed = 0f;
		LeanTween.delayedCall(base.gameObject, 0.035f, (Action)delegate
		{
			anim.speed = 1f;
		});
	}
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
			case AnimType.SWORD_ATTACK_0:
				CalcReset();
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
		anim.SetFloat("Speed", _speed);
	}
	public void SetCharacterSpeed(float _speed)
	{
		characterMoveSpeed = _speed;
	}
	public void Reset()
	{
		animType = AnimType.NONE;
		animationTime = 0f;
	}
	public void CalcReset()
	{
	}
	private void Awake()
	{
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
		case AnimType.STANDBY:
		case AnimType.JUMP:
		case AnimType.JOY:
		case AnimType.SAD:
		case AnimType.SWORD_ATTACK_0:
		case AnimType.SWORD_ATTACK_1:
		case AnimType.SWORD_ATTACK_2:
		case AnimType.SWORD_ATTACK_SP:
		case AnimType.MAGIC_CAST:
		case AnimType.MAGIC_ATTACK:
		case AnimType.DODGE:
		case AnimType.CLIMBING:
		case AnimType.DROWN:
		case AnimType.DEAD:
			break;
		case AnimType.DASH:
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
		}
	}
	public void EmitMoveEffct(int _emitNum)
	{
		efMove.Emit(_emitNum);
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
