using GamepadInput;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterKill_Player : MonoBehaviour
{
	public enum State
	{
		None,
		SwordAttack_0,
		SwordAttack_1,
		SwordAttack_2,
		SwordAttack_Max,
		MagicCast,
		MagicAttack
	}
	[SerializeField]
	[Header("スタイル")]
	private CharacterStyle style;
	[SerializeField]
	[Header("Animation管理クラス")]
	private MonsterKill_Player_AnimationManagement animationManagement;
	private Rigidbody rigid;
	private NavMeshAgent agent;
	[SerializeField]
	[Header("ミニマップ用のアイコンのCircle用ル\u30fcト")]
	private GameObject minimapIconCircleRoot;
	[SerializeField]
	[Header("ミニマップ用のアイコン")]
	private SpriteRenderer[] minimapIconArrow;
	protected State state;
	private int hp;
	private int playerNo;
	private MonsterKill_Define.UserType userType;
	private int npadId;
	private int point;
	private bool isCameraRotReset;
	[SerializeField]
	[Header("攻撃用の剣")]
	private MonsterKill_Player_Sword attackSword;
	[SerializeField]
	[Header("攻撃用の魔法")]
	private MonsterKill_Player_Magic attackMagic;
	private bool isActionWait;
	[SerializeField]
	[Header("攻撃時の突撃エフェクト（AttackType_2で使用）")]
	private ParticleSystem attackDashEffect;
	private bool isDash;
	private bool isUseUpStamina;
	private float stamina;
	private float recoveryStaminaWaitTime;
	private Vector3 moveDir;
	private float moveSpeed;
	private float limitMoveSpeed;
	private Vector3 nowPos;
	private Vector3 prevPos;
	private bool isMoveSe;
	[SerializeField]
	[Header("移動用のエフェクト")]
	private ParticleSystem moveEffect;
	private ParticleSystem.MinMaxCurve moveEffectOriginStartSize;
	[SerializeField]
	[Header("汗エフェクト")]
	private ParticleSystem sweatEffect;
	[SerializeField]
	[Header("地面判定用に Ray を飛ばすアンカ\u30fc")]
	private Transform groundCheckRayAnchor;
	private bool isGroundCheckTime;
	private bool isJump;
	[SerializeField]
	[Header("ジャンプエフェクト")]
	private ParticleSystem jumpEffect;
	private bool isDodge;
	[SerializeField]
	[Header("回避エフェクト")]
	private ParticleSystem dodgeEffect;
	private bool isKnockBack;
	private bool isDamage;
	[SerializeField]
	[Header("ダメ\u30fcジ用コライダ\u30fc")]
	protected MonsterKill_DamageCollider damageCollider;
	[SerializeField]
	[Header("ダメ\u30fcジエフェクト")]
	private ParticleSystem damageEffect;
	private List<MeshRenderer> listBlinkMesh = new List<MeshRenderer>();
	private bool isStun;
	[SerializeField]
	[Header("気絶エフェクト")]
	private ParticleSystem stunEffect;
	private bool isCpu;
	private MonsterKill_AI cpuAI;
	public void Init(int _playerNo, int _userType)
	{
		rigid = GetComponent<Rigidbody>();
		agent = GetComponent<NavMeshAgent>();
		playerNo = _playerNo;
		userType = (MonsterKill_Define.UserType)_userType;
		isCpu = (userType >= MonsterKill_Define.UserType.CPU_1);
		style.SetGameStyle(GS_Define.GameType.RECEIVE_PON, _userType);
		animationManagement.Init(this);
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && playerNo == 0)
		{
			minimapIconCircleRoot.transform.SetLocalScale(1.5f, 1.5f, 1f);
		}
		point = 0;
		hp = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetDefaultHP();
		stamina = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetMaxStamina();
		attackSword.Init(this);
		attackMagic.Init(this);
		damageCollider.Init(this);
		MeshRenderer[] meshList = style.GetMeshList(GS_Define.GameType.RECEIVE_PON, (int)userType);
		for (int i = 0; i < meshList.Length; i++)
		{
			listBlinkMesh.Add(meshList[i]);
		}
		listBlinkMesh.Add(attackSword.GetSwordMesh());
		Vector3 position = SingletonCustom<MonsterKill_FieldManager>.Instance.GetPlayerAnchor(playerNo).position;
		RaycastHit hitInfo;
		if (Physics.BoxCast(halfExtents: new Vector3(0.25f, 0.5f, 0.25f), center: position + new Vector3(0f, 10f, 0f), direction: Vector3.down, hitInfo: out hitInfo, orientation: Quaternion.identity, maxDistance: 10f) && hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Field") && NavMesh.SamplePosition(hitInfo.point, out NavMeshHit hit, 1f, 1 << NavMesh.GetAreaFromName("Walkable")))
		{
			UnityEngine.Debug.Log("player pos navhit.position : " + hit.position.ToString());
			position = hit.position;
		}
		base.transform.position = position;
		nowPos = (prevPos = base.transform.position);
		moveEffectOriginStartSize = moveEffect.main.startSize;
		if (!GetIsCpu())
		{
			agent.enabled = false;
			SetLimitMoveSpeed(1f);
		}
		else
		{
			cpuAI = base.gameObject.AddComponent<MonsterKill_AI>();
			cpuAI.Init(this);
		}
	}
	public void SetSwordMaterial(Material _mat)
	{
		attackSword.SetSwordMaterial(_mat);
	}
	public void SetMagicColor(Gradient _color)
	{
		attackMagic.SetColor(_color);
	}
	public void SetMiniMapIconColor(Color _color)
	{
		for (int i = 0; i < minimapIconArrow.Length; i++)
		{
			minimapIconArrow[i].color = _color;
		}
	}
	public void UpdateMethod()
	{
		prevPos = nowPos;
		nowPos = base.transform.position;
		if (!GetIsCpu())
		{
			if (!SingletonCustom<MonsterKill_CameraManager>.Instance.GetIsCameraRotReset(playerNo) && GetIsCameraResetInput())
			{
				SingletonCustom<MonsterKill_CameraManager>.Instance.SetCameraRotReset(playerNo);
			}
			else
			{
				Vector3 cameraRotVector = GetCameraRotVector();
				if (cameraRotVector != Vector3.zero)
				{
					SingletonCustom<MonsterKill_CameraManager>.Instance.SetCameraRot(playerNo, cameraRotVector);
				}
			}
		}
		if (SingletonCustom<MonsterKill_CameraManager>.Instance.GetIsActive(playerNo))
		{
			SingletonCustom<MonsterKill_UIManager>.Instance.SetStaminaGauge(playerNo, stamina / (float)SingletonCustom<MonsterKill_PlayerManager>.Instance.GetMaxStamina(), isUseUpStamina);
		}
		if (isJump && !isGroundCheckTime)
		{
			UnityEngine.Debug.DrawRay(groundCheckRayAnchor.position, Vector3.down * 0.1f, Color.red);
			if (Physics.Raycast(groundCheckRayAnchor.position, Vector3.down, out RaycastHit hitInfo, 0.1f) && (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Field") || hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("HitCharacterOnly")))
			{
				isJump = false;
			}
		}
		if (!isKnockBack && (isDamage || isStun))
		{
			MoveStop();
		}
		if (isDamage || isStun || isDodge)
		{
			return;
		}
		if (!GetIsCpu())
		{
			npadId = ((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? playerNo : 0);
			if (GetIsNone() && !isActionWait && !isUseUpStamina && IsDodgeInput())
			{
				Dodge();
				return;
			}
			SetMove();
			if (GetIsNone() && !isActionWait)
			{
				if (IsMagicAttackInput())
				{
					MagicCast();
					return;
				}
			}
			else if (GetIsMagicCast() && !IsMagicAttackInput())
			{
				MagicAttack();
				return;
			}
			if (GetIsMagicCast() || GetIsMagicAttack())
			{
				NoneUseDash();
			}
			else if (GetIsCanSwordAttack())
			{
				if (!isJump && IsJumpInput())
				{
					Jump();
				}
				if (!isActionWait && IsAttackInput())
				{
					SwordAttack();
				}
				if (moveDir != Vector3.zero && !isUseUpStamina && IsDashInput())
				{
					UseDash();
				}
				else
				{
					NoneUseDash();
				}
			}
		}
		else
		{
			cpuAI.UpdateMethod();
		}
		if (GetIsCpu() && cpuAI.GetIsTargetEnemyAttackDistance())
		{
			if (moveSpeed > 0f)
			{
				moveSpeed -= Time.deltaTime * SingletonCustom<MonsterKill_PlayerManager>.Instance.GetCorrectionDownDiffMoveSpeed();
			}
		}
		else if (moveDir != Vector3.zero)
		{
			if (moveSpeed < limitMoveSpeed)
			{
				moveSpeed += Time.deltaTime * SingletonCustom<MonsterKill_PlayerManager>.Instance.GetCorrectionUpDiffMoveSpeed();
				if (moveSpeed > limitMoveSpeed)
				{
					moveSpeed = limitMoveSpeed;
				}
			}
		}
		else if (moveSpeed > 0f)
		{
			moveSpeed -= Time.deltaTime * SingletonCustom<MonsterKill_PlayerManager>.Instance.GetCorrectionDownDiffMoveSpeed();
		}
		if (moveSpeed > 0f)
		{
			Move();
		}
		else
		{
			MoveStop();
		}
		Rot(moveDir);
	}
	private bool IsAttackInput()
	{
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A);
	}
	private bool IsJumpInput()
	{
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.B);
	}
	private bool IsDodgeInput()
	{
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.X);
	}
	private bool IsMagicAttackInput()
	{
		return SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Y);
	}
	private bool IsDashInput()
	{
		if (!SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.RightShoulder))
		{
			return SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.RightTrigger);
		}
		return true;
	}
	private bool GetIsCameraResetInput()
	{
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.RightStick);
	}
	private Vector3 GetCameraRotVector()
	{
		float num = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		num = SingletonCustom<JoyConManager>.Instance.GetAxisInput(npadId).Stick_R.x;
		mVector3Zero = new Vector3(0f, num, 0f);
		if (mVector3Zero.sqrMagnitude < 0.0400000028f)
		{
			return Vector3.zero;
		}
		return mVector3Zero.normalized;
	}
	public bool GetIsJump()
	{
		return isJump;
	}
	public void Jump()
	{
		isJump = true;
		isGroundCheckTime = true;
		LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
		{
			isGroundCheckTime = false;
		});
		isMoveSe = false;
		if (moveEffect.isPlaying)
		{
			moveEffect.Stop();
		}
		jumpEffect.Play();
		if (!GetIsCpu())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_morphingrace_dog_jump");
			SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
		}
		rigid.velocity += Vector3.up * SingletonCustom<MonsterKill_PlayerManager>.Instance.GetJumpPower();
	}
	public void Dodge()
	{
		isDodge = true;
		animationManagement.SetIdleAnimation(_isBool: false);
		animationManagement.SetDodgeAnimation();
		moveSpeed = 0f;
		animationManagement.SetMoveAnimation(moveSpeed);
		if (!GetIsCpu())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_oni_dodge");
			SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
		}
		Vector3 velocity = rigid.velocity;
		velocity += base.transform.forward * (SingletonCustom<MonsterKill_PlayerManager>.Instance.GetDodgePower() + (1f - moveSpeed));
		if (velocity.magnitude > SingletonCustom<MonsterKill_PlayerManager>.Instance.GetDodgePower() * 2f + 1f)
		{
			velocity = base.transform.forward * (SingletonCustom<MonsterKill_PlayerManager>.Instance.GetDodgePower() * 2f + 1f);
		}
		rigid.velocity = velocity;
		stamina -= SingletonCustom<MonsterKill_PlayerManager>.Instance.GetDodgeStamina();
		if (stamina < 0f)
		{
			if (isDash)
			{
				isDash = false;
				animationManagement.SetDashAnimation(_isBool: false);
			}
			stamina = 0f;
			isUseUpStamina = true;
			recoveryStaminaWaitTime = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetRecoveryStaminaWaitTime();
			sweatEffect.Play();
		}
	}
	public void DodgeEnd()
	{
		isDodge = false;
	}
	public bool GetIsDodge()
	{
		return isDodge;
	}
	public bool GetIsNone()
	{
		return state == State.None;
	}
	public bool GetIsLastSwordAttack()
	{
		return state == State.SwordAttack_2;
	}
	public bool GetIsCanSwordAttack()
	{
		return state < State.SwordAttack_Max;
	}
	public bool GetIsMagicCast()
	{
		return state == State.MagicCast;
	}
	public bool GetIsMagicAttack()
	{
		return state == State.MagicAttack;
	}
	public void SwordAttack()
	{
		state++;
		animationManagement.SetIdleAnimation(_isBool: false);
		animationManagement.SetAttackAnimation(state);
		if (GetIsLastSwordAttack())
		{
			attackDashEffect.Play();
		}
	}
	public void MagicCast()
	{
		state = State.MagicCast;
		isDash = false;
		animationManagement.SetDashAnimation(_isBool: false);
		attackMagic.PlayCastEffect();
		animationManagement.SetIdleAnimation(_isBool: false);
		animationManagement.SetMagicCastAnimation(_isBool: true);
		if (!GetIsCpu())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_magic_cast", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
		}
	}
	public void MagicAttack()
	{
		state = State.MagicAttack;
		animationManagement.SetMagicAttackAnimation();
	}
	public void AttackStart()
	{
		if (!isStun)
		{
			if (!GetIsMagicAttack())
			{
				attackSword.AttackStart();
			}
			else
			{
				attackMagic.AttackStart();
			}
		}
	}
	public void AttackEnd()
	{
		if (!isStun)
		{
			LeanTween.cancel(attackSword.gameObject);
			LeanTween.delayedCall(attackSword.gameObject, 0.2f, (Action)delegate
			{
				attackSword.AttackEnd();
				animationManagement.ResetSwordAttackAnimation();
				attackMagic.AttackEnd();
				animationManagement.SetMagicCastAnimation(_isBool: false);
				animationManagement.ResetMagicAttackAnimation();
				state = State.None;
				isActionWait = true;
				LeanTween.delayedCall(attackSword.gameObject, 0.2f, (Action)delegate
				{
					isActionWait = false;
				});
			});
			if (!isStun && moveSpeed == 0f)
			{
				animationManagement.SetIdleAnimation(_isBool: true);
			}
		}
	}
	public bool GetIsUseUpStamina()
	{
		return isUseUpStamina;
	}
	public float GetStamina()
	{
		return stamina;
	}
	public void UseDash()
	{
		isDash = true;
		animationManagement.SetDashAnimation(_isBool: true);
		stamina -= Time.deltaTime * SingletonCustom<MonsterKill_PlayerManager>.Instance.GetUseStaminaSpeed();
		if (stamina < 0f)
		{
			isDash = false;
			animationManagement.SetDashAnimation(_isBool: false);
			stamina = 0f;
			isUseUpStamina = true;
			recoveryStaminaWaitTime = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetRecoveryStaminaWaitTime();
			sweatEffect.Play();
		}
	}
	public void NoneUseDash()
	{
		isDash = false;
		animationManagement.SetDashAnimation(_isBool: false);
		if (isUseUpStamina)
		{
			if (recoveryStaminaWaitTime > 0f)
			{
				recoveryStaminaWaitTime -= Time.deltaTime;
			}
			else
			{
				stamina += SingletonCustom<MonsterKill_PlayerManager>.Instance.GetRecoveryStaminaSpeed() * SingletonCustom<MonsterKill_PlayerManager>.Instance.GetUseUpRecoveryStaminaSpeedMag() * Time.deltaTime;
			}
		}
		else
		{
			stamina += SingletonCustom<MonsterKill_PlayerManager>.Instance.GetRecoveryStaminaSpeed() * Time.deltaTime;
		}
		if (stamina > (float)SingletonCustom<MonsterKill_PlayerManager>.Instance.GetMaxStamina())
		{
			stamina = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetMaxStamina();
		}
		if (isUseUpStamina && stamina > SingletonCustom<MonsterKill_PlayerManager>.Instance.GetUseUpReUseStamina())
		{
			isUseUpStamina = false;
			sweatEffect.Stop();
		}
	}
	public void MoveStop()
	{
		if (moveSpeed != 0f)
		{
			moveSpeed = 0f;
			animationManagement.SetMoveAnimation(moveSpeed);
		}
		if (!isStun && !isJump)
		{
			if (GetIsNone())
			{
				animationManagement.SetIdleAnimation(_isBool: true);
			}
			else if (GetIsMagicCast())
			{
				animationManagement.SetMagicCastAnimation(_isBool: true);
			}
		}
		float y = rigid.velocity.y;
		rigid.velocity = new Vector3(0f, y, 0f);
		if (moveEffect.isPlaying)
		{
			moveEffect.Stop();
		}
	}
	public void Move()
	{
		Vector3 vector = rigid.velocity;
		float y = vector.y;
		float baseMoveSpeed = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetBaseMoveSpeed();
		float maxMoveSpeed = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetMaxMoveSpeed();
		float num = (!isDash) ? 1f : SingletonCustom<MonsterKill_PlayerManager>.Instance.GetDashSpeed();
		float num2 = (!GetIsMagicCast() && !GetIsMagicAttack()) ? 1f : SingletonCustom<MonsterKill_PlayerManager>.Instance.GetUseMagicMoveSpeed();
		float num3 = (!isUseUpStamina) ? 1f : SingletonCustom<MonsterKill_PlayerManager>.Instance.GetUseUpStaminaMoveSpeed();
		vector += moveDir * baseMoveSpeed * moveSpeed * num * num3 * num2;
		if (vector.magnitude > maxMoveSpeed * moveSpeed * num * num3 * num2)
		{
			vector = vector.normalized * maxMoveSpeed * moveSpeed * num * num3 * num2;
		}
		vector.y = y;
		rigid.velocity = vector;
		if (!GetIsCpu() && !isJump && !isMoveSe)
		{
			float value = CalcManager.Length(nowPos, prevPos) / 0.05f;
			value = Mathf.Clamp(value, 0.2f, 1f);
			SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, value * 0.5f);
			isMoveSe = true;
			LeanTween.delayedCall(base.gameObject, 0.15f + (1f - value) * 0.05f, (Action)delegate
			{
				isMoveSe = false;
			});
		}
		ParticleSystem.MainModule main = moveEffect.main;
		ParticleSystem.MinMaxCurve startSize = main.startSize;
		if (isDash)
		{
			startSize.constantMin = moveEffectOriginStartSize.constantMin * 1.5f;
			startSize.constantMax = moveEffectOriginStartSize.constantMax * 1.5f;
		}
		else
		{
			startSize.constantMin = moveEffectOriginStartSize.constantMin;
			startSize.constantMax = moveEffectOriginStartSize.constantMax;
		}
		main.startSize = startSize;
		if (!moveEffect.isPlaying)
		{
			moveEffect.Play();
		}
		if (GetIsNone() && !isActionWait)
		{
			animationManagement.SetIdleAnimation(_isBool: false);
		}
		animationManagement.SetMoveAnimation(moveSpeed);
	}
	public Vector3 GetMoveDir()
	{
		return moveDir;
	}
	public void SetMoveDir(Vector3 _moveDir)
	{
		moveDir = SingletonCustom<MonsterKill_CameraManager>.Instance.GetCameraDir(playerNo) * _moveDir;
	}
	public void SetMove()
	{
		SetMoveDir(GetStickDir());
	}
	private Vector3 GetStickDir()
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 mVector3Zero = CalcManager.mVector3Zero;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(npadId);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		if (true && Mathf.Abs(num) < 0.2f && Mathf.Abs(num2) < 0.2f)
		{
			num = 0f;
			num2 = 0f;
			if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Right))
			{
				num = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Left))
			{
				num = -1f;
			}
			if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Up))
			{
				num2 = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Down))
			{
				num2 = -1f;
			}
		}
		mVector3Zero = new Vector3(num, 0f, num2);
		if (mVector3Zero.sqrMagnitude < 0.0400000028f)
		{
			return Vector3.zero;
		}
		return mVector3Zero.normalized;
	}
	private void Rot(Vector3 _vec, bool _immediate = false)
	{
		_vec.y = 0f;
		if (!(_vec == Vector3.zero))
		{
			Quaternion quaternion = Quaternion.LookRotation(_vec);
			if (_immediate)
			{
				base.transform.rotation = quaternion;
			}
			else
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, quaternion, Time.deltaTime * 20f);
			}
		}
	}
	public bool GetIsDamage()
	{
		return isDamage;
	}
	public void Damage(int _damage, Vector3 _hitPos, Vector3 _attackerPos)
	{
		if (isDamage || isStun || isDodge)
		{
			return;
		}
		isMoveSe = false;
		isDamage = true;
		damageEffect.transform.position = _hitPos;
		damageEffect.Play();
		LeanTween.value(base.gameObject, 0f, 1f, 0.2f).setOnUpdate(delegate(float _value)
		{
			for (int j = 0; j < listBlinkMesh.Count; j++)
			{
				listBlinkMesh[j].material.EnableKeyword("_EMISSION");
				listBlinkMesh[j].material.SetColor("_EmissionColor", Color.white * _value);
			}
		}).setOnComplete((Action)delegate
		{
			LeanTween.value(base.gameObject, 1f, 0f, 0.2f).setOnUpdate(delegate(float _value)
			{
				for (int i = 0; i < listBlinkMesh.Count; i++)
				{
					listBlinkMesh[i].material.EnableKeyword("_EMISSION");
					listBlinkMesh[i].material.SetColor("_EmissionColor", Color.white * _value);
				}
			});
		});
		isKnockBack = true;
		float y = rigid.velocity.y;
		rigid.velocity = new Vector3(0f, y, 0f);
		Vector3 a = _attackerPos - base.transform.position;
		a.y = 0f;
		rigid.AddForce(-a * 500f, ForceMode.Impulse);
		LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
		{
			isKnockBack = false;
		});
		hp -= _damage;
		if (hp <= 0)
		{
			Stun();
			return;
		}
		if (!GetIsCpu())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_sword_fight_hit", _loop: false, 0f, 0.5f);
			SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
		}
		LeanTween.delayedCall(base.gameObject, SingletonCustom<MonsterKill_PlayerManager>.Instance.GetDamageTime(), (Action)delegate
		{
			isDamage = false;
		});
	}
	public bool GetIsStun()
	{
		return isStun;
	}
	public void Stun()
	{
		isStun = true;
		animationManagement.SetIdleAnimation(_isBool: false);
		animationManagement.SetDashAnimation(_isBool: false);
		animationManagement.ResetSwordAttackAnimation();
		animationManagement.SetMagicCastAnimation(_isBool: false);
		animationManagement.ResetMagicAttackAnimation();
		animationManagement.SetStunAnimation();
		state = State.None;
		attackSword.AttackEnd();
		attackMagic.AttackEnd();
		stunEffect.Play();
		if (!GetIsCpu())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_monster_kill_stun");
			SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Strong, 1f);
		}
		float stunTime = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetStunTime();
		LeanTween.delayedCall(base.gameObject, stunTime * 0.95f, (Action)delegate
		{
			stunEffect.Stop();
		});
		LeanTween.delayedCall(base.gameObject, stunTime, (Action)delegate
		{
			isDamage = false;
			isStun = false;
			animationManagement.SetIdleAnimation(_isBool: true);
			hp = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetDefaultHP();
			isActionWait = true;
			LeanTween.delayedCall(base.gameObject, 0.2f, (Action)delegate
			{
				isActionWait = false;
			});
		});
	}
	public int GetPlayerNo()
	{
		return playerNo;
	}
	public MonsterKill_Define.UserType GetUserType()
	{
		return userType;
	}
	public bool GetIsCpu()
	{
		return isCpu;
	}
	public int GetPoint()
	{
		return point;
	}
	public void AddPoint(int _addPoint)
	{
		SingletonCustom<MonsterKill_UIManager>.Instance.SetPoint(playerNo, playerNo, point, point + _addPoint);
		point += _addPoint;
	}
	public void SetPoint(int _point)
	{
		point = _point;
	}
	public NavMeshAgent GetNavMeshAgent()
	{
		return agent;
	}
	public bool GetIsActionWait()
	{
		return isActionWait;
	}
	public MonsterKill_Enemy GetTargetEnemy()
	{
		return cpuAI.GetTargetEnemy();
	}
	public void SetLimitMoveSpeed(float _limitMoveSpeed)
	{
		limitMoveSpeed = _limitMoveSpeed;
	}
	public void SetGameEnd()
	{
		animationManagement.SetGameEndAnimation();
		Vector3 velocity = rigid.velocity;
		LeanTween.value(base.gameObject, 1f, 0f, 1f).setOnUpdate(delegate(float _value)
		{
			velocity *= _value;
			velocity.y = -3f;
			rigid.velocity = velocity;
		}).setOnComplete((Action)delegate
		{
			rigid.velocity = Vector3.zero;
			rigid.isKinematic = true;
		});
		moveEffect.Stop();
		sweatEffect.Stop();
		attackSword.AttackEnd();
		attackMagic.AttackEnd();
	}
}
