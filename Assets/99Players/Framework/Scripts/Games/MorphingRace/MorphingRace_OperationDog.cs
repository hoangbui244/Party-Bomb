using System;
using UnityEngine;
public class MorphingRace_OperationDog : MorphingRace_OperationCharacter
{
	private Animator animator;
	[SerializeField]
	[Header("ジャンプエフェクト")]
	private ParticleSystem jumpEffect;
	private int fieldLayerMask;
	private string[] fieldLayerMaskNameList = new string[1]
	{
		"Field"
	};
	private bool isGround;
	private GameObject groundObj;
	private bool isJumping;
	private int jumpAreaIdx;
	private int jumpAreaCnt;
	private MorphingRace_OperationDog_AI cpuAI;
	public override void Init(MorphingRace_Player _player)
	{
		base.Init(_player);
		animator = GetComponent<Animator>();
		fieldLayerMask = LayerMask.GetMask(fieldLayerMaskNameList);
		if (player.GetIsCpu())
		{
			cpuAI = base.gameObject.AddComponent<MorphingRace_OperationDog_AI>();
			cpuAI.Init(this);
		}
	}
	public override void UpdateMethod_Base()
	{
		base.UpdateMethod_Base();
		player.SetInputInterval();
		SetIsGround();
		jumpAreaIdx = player.GetMorphingTargetDog().GetJumpAreaIdx(player.GetPlayerNo(), player.transform.position);
		if (!player.GetIsCpu())
		{
			UpdateMethod();
		}
		else
		{
			cpuAI.UpdateMethod();
		}
	}
	protected override void UpdateMethod()
	{
		if (player.GetButtonDown_A())
		{
			MoveInput();
		}
		else
		{
			MoveNoneInput();
		}
		if (CheckIsGround())
		{
			if (GetIsJumpping())
			{
				SetIsJumpping(_isJumpping: false);
			}
			if (CheckIsCanJump())
			{
				if (player.GetMorphingTargetDog().CheckIsCanJumpViewUI(player.GetPlayerNo(), jumpAreaIdx, player.transform.position))
				{
					SingletonCustom<MorphingRace_UIManager>.Instance.SetControllerBalloonActive(player.GetPlayerNo(), MorphingRace_UIManager.ControllerUIType.Dog, _isFade: false, _isActive: true);
				}
				else
				{
					SingletonCustom<MorphingRace_UIManager>.Instance.SetControllerBalloonActive(player.GetPlayerNo(), MorphingRace_UIManager.ControllerUIType.Dog, _isFade: false, _isActive: false);
				}
				if (player.GetButtonDown_B())
				{
					Jump();
					return;
				}
			}
			else
			{
				SingletonCustom<MorphingRace_UIManager>.Instance.SetControllerBalloonActive(player.GetPlayerNo(), MorphingRace_UIManager.ControllerUIType.Dog, _isFade: false, _isActive: false);
			}
			player.SetMove();
			Move();
		}
		else
		{
			SingletonCustom<MorphingRace_UIManager>.Instance.SetControllerBalloonActive(player.GetPlayerNo(), MorphingRace_UIManager.ControllerUIType.Dog, _isFade: false, _isActive: false);
			Rot(rigid.velocity.normalized);
		}
	}
	public void SetIsGround()
	{
		if (Physics.OverlapSphereNonAlloc(collider.transform.position, collider_radius, arrayOverLapCollider, fieldLayerMask) > 0)
		{
			isGround = true;
			groundObj = arrayOverLapCollider[0].gameObject;
		}
		else if (Physics.SphereCastNonAlloc(collider.transform.position, collider_radius, Vector3.down, arrayRaycastHit, 0.05f, fieldLayerMask) > 0)
		{
			isGround = true;
			groundObj = arrayRaycastHit[0].collider.gameObject;
		}
		else
		{
			isGround = false;
			groundObj = null;
		}
	}
	public bool CheckIsGround()
	{
		return isGround;
	}
	public bool CheckIsCanJump()
	{
		if (groundObj.tag == "Field")
		{
			if (CheckJumpAreaIdxLessCnt())
			{
				if (player.GetMorphingTargetDog().CheckIsCanJumpArea(player.GetPlayerNo(), jumpAreaIdx, player.transform.position, !player.GetIsCpu()))
				{
					return true;
				}
				return false;
			}
			return false;
		}
		return false;
	}
	public bool CheckJumpAreaIdxLessCnt()
	{
		return jumpAreaIdx < jumpAreaCnt;
	}
	public bool CheckPassJumpArea()
	{
		if (CheckJumpAreaIdxLessCnt() && player.GetMorphingTargetDog().CheckPassJumpArea(player.GetPlayerNo(), jumpAreaIdx, player.transform.position))
		{
			return true;
		}
		return false;
	}
	public void Jump()
	{
		UnityEngine.Debug.Log("ジャンプ処理");
		SetIsJumpping(_isJumpping: true);
		player.GetMorphingTargetDog().SetJumpObstacleColliderActive(player.GetPlayerNo(), jumpAreaIdx, _isActive: false);
		int beforeJumpAreaIdx = jumpAreaIdx;
		LeanTween.delayedCall(base.gameObject, 0.5f, (Action)delegate
		{
			player.GetMorphingTargetDog().SetJumpObstacleColliderActive(player.GetPlayerNo(), beforeJumpAreaIdx, _isActive: true);
		});
		if (!player.GetIsCpu())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_morphingrace_dog_jump");
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(player.GetPlayerNo());
		}
		if (moveEffect.isPlaying)
		{
			moveEffect.Stop();
		}
		UnityEngine.Debug.Log("rigid.velocity magnitude ジャンプ前 " + rigid.velocity.magnitude.ToString());
		if (rigid.velocity.magnitude < 0.1f)
		{
			jumpEffect.transform.position = player.transform.position;
			rigid.velocity += base.transform.forward * 2f + Vector3.up * SingletonCustom<MorphingRace_PlayerManager>.Instance.GetJumpBasePower();
		}
		else
		{
			jumpEffect.transform.position = player.transform.position + rigid.velocity.normalized / 2f;
			rigid.velocity += Vector3.up * SingletonCustom<MorphingRace_PlayerManager>.Instance.GetJumpBasePower();
		}
		UnityEngine.Debug.Log("rigid.velocity magnitude ジャンプ後 " + rigid.velocity.magnitude.ToString());
		jumpEffect.Play();
	}
	public override void Move()
	{
		Vector3 moveDir = player.GetMoveDir();
		Vector3 vector = rigid.velocity;
		if (moveSpeed == 0f)
		{
			vector.x = 0f;
			vector.z = 0f;
			rigid.velocity = vector;
			if (moveEffect.isPlaying)
			{
				moveEffect.Stop();
			}
		}
		else
		{
			float y = vector.y;
			float baseMoveSpeed = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetBaseMoveSpeed();
			float maxMoveSpeed = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetMaxMoveSpeed((int)characterType);
			float correctionUpDiffMoveSpeed = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetCorrectionUpDiffMoveSpeed();
			Vector3 a = moveDir * baseMoveSpeed * moveSpeed;
			vector += a * Time.deltaTime * correctionUpDiffMoveSpeed;
			if (vector.magnitude > maxMoveSpeed * moveSpeed)
			{
				vector = vector.normalized * maxMoveSpeed * moveSpeed;
			}
			vector.y = y;
			vector = GetCanMoveVelocity(vector);
			rigid.velocity = vector;
			Rot(rigid.velocity.normalized);
			if (!player.GetIsCpu() && !isMoveSe)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.65f);
				isMoveSe = true;
				LeanTween.delayedCall(base.gameObject, 0.45f, (Action)delegate
				{
					isMoveSe = false;
				});
			}
			if (!moveEffect.isPlaying)
			{
				moveEffect.Play();
			}
		}
		animator.SetFloat("Speed", inputLerp);
	}
	public override void StopMove()
	{
		base.StopMove();
		LeanTween.cancel(base.gameObject);
		isMoveSe = false;
		moveEffect.Stop();
		if (player.GetIsCpu())
		{
			cpuAI.StopMove();
		}
	}
	public override void MorphingInit()
	{
		SetIsJumpping(_isJumpping: false);
		jumpAreaIdx = player.GetMorphingTargetDog().GetJumpAreaIdx(player.GetPlayerNo(), player.transform.position);
		jumpAreaCnt = player.GetMorphingTargetDog().GetJumpAnchorLength(player.GetPlayerNo());
		if (player.GetIsCpu())
		{
			cpuAI.MorphingInit();
		}
	}
	public int GetJumpAreaIdx()
	{
		return jumpAreaIdx;
	}
	public void AddJumpAreaIdx()
	{
		jumpAreaIdx++;
	}
	public bool GetIsJumpping()
	{
		return isJumping;
	}
	public void SetIsJumpping(bool _isJumpping)
	{
		isJumping = _isJumpping;
		animator.SetBool("Jump", isJumping);
	}
}
