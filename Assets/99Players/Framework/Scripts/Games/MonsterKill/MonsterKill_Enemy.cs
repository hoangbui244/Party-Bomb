using System;
using UnityEngine;
using UnityEngine.AI;
public class MonsterKill_Enemy : MonoBehaviour
{
	public enum AttackType
	{
		AttackType_0,
		AttackType_1,
		AttackType_2,
		Max
	}
	[Serializable]
	protected struct AttackColliderInfo
	{
		[SerializeField]
		[Header("攻撃する距離")]
		public float attackDistance;
		[SerializeField]
		[Header("攻撃用コライダ\u30fc")]
		public MonsterKill_AttackCollider attackCollider;
	}
	private GameObject target;
	private float targetDistance;
	private Rigidbody rigid;
	private NavMeshAgent agent;
	private MonsterKill_NavPathMove navPathMove;
	[SerializeField]
	[Header("モンスタ\u30fcの種類")]
	private MonsterKill_EnemyManager.EnemyType enemyType;
	[SerializeField]
	[Header("Animation管理クラス")]
	private MonsterKill_Enemy_AnimationManagement animationManagement;
	[SerializeField]
	[Header("モデルのル\u30fcト")]
	protected GameObject modelRoot;
	[SerializeField]
	[Header("メッシュ")]
	private SkinnedMeshRenderer mesh;
	[SerializeField]
	[Header("色違い用マテリアル")]
	private Material[] arrayMaterial;
	[SerializeField]
	[Header("ミニマップ用のアイコン")]
	private SpriteRenderer minimapIcon;
	protected MonsterKill_Enemy_SpawnArea spawnArea;
	[SerializeField]
	[Header("HP")]
	private int hp;
	[SerializeField]
	[Header("プレイヤ\u30fcを感知する距離")]
	private float SENSE_PLAYER_DISTANCE;
	[SerializeField]
	[Header("プレイヤ\u30fcを攻撃する高さの角度")]
	private float ATTACK_HEIGHT_ANGLE;
	[SerializeField]
	[Header("プレイヤ\u30fcを攻撃する向きの角度")]
	private float ATTACK_DIR_ANGLE;
	[SerializeField]
	[Header("最大移動速度")]
	private float MAX_MOVE_SPEED;
	[SerializeField]
	[Header("徐\u3005に移動速度を上げる補正速度")]
	private float CORRECTION_UP_DIFF_MOVE_SPEED = 1.25f;
	[SerializeField]
	[Header("徐\u3005に移動速度を下げる補正速度")]
	private float CORRECTION_DOWN_DIFF_MOVE_SPEED = 3f;
	private float moveSpeed;
	private Vector3 heightVec;
	private Vector3 dirVec;
	[SerializeField]
	[Header("回転速度")]
	private float ROT_SPEED;
	private Vector3 nearSpawnPos;
	private float nearSpawnMoveInterval;
	private float NEAR_SPAWN_UNABLE_TIME = 5f;
	private float nearSpawnUnableTime;
	protected AttackType attackType;
	[SerializeField]
	[Header("攻撃情報")]
	protected AttackColliderInfo[] attackInfo;
	protected bool isAttackAnim;
	[SerializeField]
	[Header("次の攻撃までのインタ\u30fcバル")]
	private float NEXT_ATTACK_INTERVAL;
	private float nextAttackInterval;
	[SerializeField]
	[Header("ダメ\u30fcジ用コライダ\u30fc")]
	protected MonsterKill_DamageCollider damageCollider;
	[SerializeField]
	[Header("攻撃エフェクト")]
	protected ParticleSystem attackEffect;
	private bool isDamage;
	[SerializeField]
	[Header("ダメ\u30fcジエフェクト")]
	private ParticleSystem damageEffect;
	private bool isIgnoreDamage;
	private bool isDead;
	[SerializeField]
	[Header("死亡エフェクト")]
	private MonsterKill_Enemy_DeadEffect[] arrayDeadEffect;
	private MonsterKill_Enemy_DeadEffect rootDeadEffect;
	[SerializeField]
	[Header("デバッグ用：攻撃種類")]
	private AttackType debugAttackType;
	public virtual void Init(MonsterKill_Enemy_SpawnArea _spawnArea)
	{
		spawnArea = _spawnArea;
		rigid = GetComponent<Rigidbody>();
		rigid.constraints |= (RigidbodyConstraints)10;
		agent = GetComponent<NavMeshAgent>();
		navPathMove = base.gameObject.AddComponent<MonsterKill_NavPathMove>();
		navPathMove.Init(agent);
		navPathMove.AgentWarp(base.transform.position);
		animationManagement.Init(this);
		int num = UnityEngine.Random.Range(0, arrayMaterial.Length);
		SkinnedMeshRenderer[] componentsInChildren = modelRoot.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material = arrayMaterial[num];
		}
		MeshRenderer[] componentsInChildren2 = modelRoot.transform.GetComponentsInChildren<MeshRenderer>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].material = arrayMaterial[num];
		}
		minimapIcon.color = SingletonCustom<MonsterKill_UIManager>.Instance.GetMiniMapIconColor();
		targetDistance = 10000f;
		SetNearSpawnMoveInterval();
		SetNextAttackInterval();
		nextAttackInterval /= 2f;
		for (int k = 0; k < attackInfo.Length; k++)
		{
			attackInfo[k].attackCollider.Init(this);
			attackInfo[k].attackCollider.gameObject.SetActive(value: false);
		}
		damageCollider.Init(this);
		for (int l = 0; l < arrayDeadEffect.Length; l++)
		{
			arrayDeadEffect[l].SetEffectColor(num);
		}
		rootDeadEffect = arrayDeadEffect[0];
		rootDeadEffect.gameObject.SetActive(value: false);
	}
	public void UpdateMethod()
	{
		if (isDead)
		{
			return;
		}
		if (nextAttackInterval > 0f)
		{
			nextAttackInterval -= Time.deltaTime;
		}
		if (target != null)
		{
			targetDistance = CalcManager.Length(base.transform.position, target.transform.position);
			if (targetDistance > SENSE_PLAYER_DISTANCE)
			{
				target = null;
				navPathMove.AgentResetPath();
				navPathMove.AgentWarp(base.transform.position);
				animationManagement.ResetAttackAnimation();
			}
		}
		if (isAttackAnim || isDamage)
		{
			return;
		}
		bool flag = !(target != null) || !(targetDistance <= attackInfo[(int)attackType].attackDistance) || !(Vector3.Angle(heightVec, dirVec) <= ATTACK_HEIGHT_ANGLE);
		navPathMove.UpdateMethod(flag);
		if (flag)
		{
			Vector3 moveVector = navPathMove.GetMoveVector();
			if (moveVector == Vector3.zero)
			{
				MoveStop();
			}
			else
			{
				SetMove(moveVector);
			}
		}
		if (target == null)
		{
			target = GetNearTarget();
			if (target == null)
			{
				MoveNearSpawn();
				return;
			}
			targetDistance = 10000f;
			nearSpawnPos = Vector3.zero;
			navPathMove.ResetPrevMyPos();
			navPathMove.ResetPrevTaretPos();
		}
		heightVec = (target.transform.position - base.transform.position).normalized;
		dirVec = heightVec;
		dirVec.y = 0f;
		if (targetDistance <= attackInfo[(int)attackType].attackDistance && Vector3.Angle(heightVec, dirVec) <= ATTACK_HEIGHT_ANGLE)
		{
			if (Vector3.Angle(dirVec, base.transform.forward) <= ATTACK_DIR_ANGLE)
			{
				if (rigid.velocity != Vector3.zero)
				{
					MoveStop();
				}
				if (nextAttackInterval <= 0f)
				{
					Attack();
				}
				return;
			}
			if (moveSpeed > 0f)
			{
				moveSpeed -= Time.deltaTime * CORRECTION_DOWN_DIFF_MOVE_SPEED;
				if (moveSpeed < 0f)
				{
					if (rigid.velocity != Vector3.zero)
					{
						MoveStop();
					}
				}
				else
				{
					animationManagement.SetIdleAnimation(_isBool: false);
					animationManagement.SetMoveAnimation(moveSpeed);
				}
			}
			Rot(heightVec);
		}
		else
		{
			target = GetNearTarget();
			navPathMove.SetMoveDestination(target.transform.position);
		}
	}
	private GameObject GetNearTarget()
	{
		GameObject result = null;
		float num = 10000f;
		if (target != null)
		{
			result = target;
			num = targetDistance;
		}
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			GameObject gameObject = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(i).gameObject;
			if (!(target != null) || !(gameObject == target))
			{
				Vector3 position = gameObject.transform.position;
				float num2 = CalcManager.Length(base.transform.position, position);
				if (num2 <= SENSE_PLAYER_DISTANCE && num2 < num)
				{
					num = num2;
					result = gameObject;
				}
			}
		}
		return result;
	}
	private void SetNearSpawnMoveInterval()
	{
		nearSpawnMoveInterval = SingletonCustom<MonsterKill_EnemyManager>.Instance.GetNearSpawnMoveInterval() + UnityEngine.Random.Range(-1f, 1f);
	}
	private void MoveNearSpawn()
	{
		if (nearSpawnPos != Vector3.zero)
		{
			if (navPathMove.GetIsMovePath() && navPathMove.GetIsMoveComplete())
			{
				nearSpawnPos = Vector3.zero;
				navPathMove.ResetPrevMyPos();
				navPathMove.ResetPrevTaretPos();
			}
			else
			{
				nearSpawnUnableTime -= Time.deltaTime;
				if (nearSpawnUnableTime <= 0f)
				{
					nearSpawnPos = Vector3.zero;
					navPathMove.ResetPrevMyPos();
					navPathMove.ResetPrevTaretPos();
				}
			}
		}
		if (nearSpawnPos == Vector3.zero)
		{
			if (nearSpawnMoveInterval > 0f && !navPathMove.GetIsMovePathSetting())
			{
				nearSpawnMoveInterval -= Time.deltaTime;
				if (moveSpeed > 0f)
				{
					moveSpeed -= Time.deltaTime * CORRECTION_DOWN_DIFF_MOVE_SPEED;
					if (moveSpeed < 0f)
					{
						if (rigid.velocity != Vector3.zero)
						{
							MoveStop();
						}
					}
					else
					{
						animationManagement.SetIdleAnimation(_isBool: false);
						animationManagement.SetMoveAnimation(moveSpeed);
					}
				}
				if (nearSpawnMoveInterval > 0f)
				{
					return;
				}
			}
			nearSpawnPos = SingletonCustom<MonsterKill_EnemyManager>.Instance.GetCanMovePos(spawnArea);
			if (nearSpawnPos != Vector3.zero)
			{
				SetNearSpawnMoveInterval();
				nearSpawnUnableTime = NEAR_SPAWN_UNABLE_TIME;
			}
		}
		navPathMove.SetMoveDestination(nearSpawnPos);
	}
	private void SetMove(Vector3 _vec)
	{
		moveSpeed += Time.deltaTime * CORRECTION_UP_DIFF_MOVE_SPEED;
		if (moveSpeed > 1f)
		{
			moveSpeed = 1f;
		}
		animationManagement.SetIdleAnimation(_isBool: false);
		animationManagement.SetMoveAnimation(moveSpeed);
		Move(_vec);
		Rot(_vec);
	}
	private void MoveStop()
	{
		moveSpeed = 0f;
		rigid.velocity = Vector3.zero;
		rigid.constraints |= (RigidbodyConstraints)10;
		if (!isAttackAnim && !isDamage)
		{
			animationManagement.SetIdleAnimation(_isBool: true);
		}
		animationManagement.SetMoveAnimation(moveSpeed);
	}
	private void Move(Vector3 vector)
	{
		rigid.constraints &= (RigidbodyConstraints)(-11);
		Vector3 velocity = vector.normalized * (MAX_MOVE_SPEED * moveSpeed);
		rigid.velocity = velocity;
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
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, Time.deltaTime * ROT_SPEED);
			}
		}
	}
	private void Attack()
	{
		isAttackAnim = true;
		animationManagement.SetAttackAnimation(attackType);
	}
	public void AttackEnd()
	{
		isAttackAnim = false;
		SetNextAttackInterval();
		animationManagement.SetIdleAnimation(_isBool: true);
	}
	private void SetNextAttackInterval()
	{
		nextAttackInterval = NEXT_ATTACK_INTERVAL + UnityEngine.Random.Range(-0.5f, 0.5f);
	}
	public void SetAttackColliderActive(bool _isActive, int _attackCnt = -1)
	{
		if (!isDamage && !isDead)
		{
			attackInfo[(int)attackType].attackCollider.gameObject.SetActive(_isActive);
		}
	}
	public bool GetIsDamage()
	{
		return isDamage;
	}
	public void SetIsIgnoreDamage(bool _isIgnoreDamage)
	{
		isIgnoreDamage = _isIgnoreDamage;
	}
	public void Damage(int _damage, Vector3 _hitPos, Vector3 _attackerPos, int _playerNo)
	{
		damageEffect.transform.position = _hitPos;
		damageEffect.Play();
		hp -= _damage;
		if (!SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(_playerNo).GetIsCpu())
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_attack_hit_2");
		}
		if (isIgnoreDamage)
		{
			if (hp <= 0)
			{
				Dead(_playerNo);
			}
			else
			{
				DamageShake();
			}
			return;
		}
		if (!isDamage)
		{
			isAttackAnim = false;
			attackInfo[(int)attackType].attackCollider.gameObject.SetActive(value: false);
			SetNextAttackInterval();
		}
		isDamage = true;
		animationManagement.SetIdleAnimation(_isBool: false);
		MoveStop();
		if (hp <= 0)
		{
			Dead(_playerNo);
			return;
		}
		animationManagement.SetDamageAnimation();
		LeanTween.cancel(base.gameObject);
		LeanTween.delayedCall(base.gameObject, SingletonCustom<MonsterKill_EnemyManager>.Instance.GetDamageTime(), (Action)delegate
		{
			isDamage = false;
			animationManagement.SetIdleAnimation(_isBool: true);
		});
	}
	protected virtual void DamageShake()
	{
	}
	public bool GetIsDead()
	{
		return isDead;
	}
	protected virtual void Dead(int _playerNo)
	{
		isDead = true;
		MonsterKill_EnemyManager.DeadPointTpe deadPointType = SingletonCustom<MonsterKill_EnemyManager>.Instance.GetDeadPointType(enemyType);
		SingletonCustom<MonsterKill_UIManager>.Instance.SetPointUp(_playerNo, base.gameObject, deadPointType);
		int deadPoint = SingletonCustom<MonsterKill_EnemyManager>.Instance.GetDeadPoint(deadPointType);
		SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(_playerNo).AddPoint(deadPoint);
		navPathMove.AgentResetPath();
		navPathMove.AgentWarp(base.transform.position);
		navPathMove.AgentIsStopped(_isStopped: true);
		isAttackAnim = false;
		attackInfo[(int)attackType].attackCollider.gameObject.SetActive(value: false);
		rigid.velocity = Vector3.zero;
		rigid.constraints |= RigidbodyConstraints.FreezePosition;
		damageCollider.gameObject.layer = LayerMask.NameToLayer("HitFieldOnly");
		animationManagement.SetDeadAnimation();
		LeanTween.delayedCall(base.gameObject, 1.5f, (Action)delegate
		{
			rootDeadEffect.transform.parent = base.transform.parent;
			rootDeadEffect.transform.position = mesh.bounds.center + new Vector3(0f, 0.25f, 0f);
			switch (enemyType)
			{
			case MonsterKill_EnemyManager.EnemyType.Bat:
			case MonsterKill_EnemyManager.EnemyType.ChestMonster:
			case MonsterKill_EnemyManager.EnemyType.CrabMonster:
			case MonsterKill_EnemyManager.EnemyType.Slime:
			case MonsterKill_EnemyManager.EnemyType.Spider:
				rootDeadEffect.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
				break;
			case MonsterKill_EnemyManager.EnemyType.Dragon:
			case MonsterKill_EnemyManager.EnemyType.EvilMage:
			case MonsterKill_EnemyManager.EnemyType.MonsterPlant:
			case MonsterKill_EnemyManager.EnemyType.Skeleton:
				rootDeadEffect.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
				break;
			case MonsterKill_EnemyManager.EnemyType.BlackKnight:
			case MonsterKill_EnemyManager.EnemyType.LizardWarrior:
			case MonsterKill_EnemyManager.EnemyType.Orc:
			case MonsterKill_EnemyManager.EnemyType.Werewolf:
				rootDeadEffect.transform.localScale = new Vector3(1f, 1f, 1f);
				break;
			}
			rootDeadEffect.gameObject.SetActive(value: true);
			rootDeadEffect.PlayDeadEffect();
		});
		LeanTween.delayedCall(base.gameObject, 2f, (Action)delegate
		{
			SingletonCustom<MonsterKill_EnemyManager>.Instance.RemoveEnemyList(this);
			UnityEngine.Object.Destroy(base.gameObject);
		});
	}
	public MonsterKill_EnemyManager.EnemyType GetEnemyType()
	{
		return enemyType;
	}
	public MonsterKill_Enemy_SpawnArea GetSpawnArea()
	{
		return spawnArea;
	}
	public void SetGameEnd()
	{
		animationManagement.SetGameEndAnimation();
		LeanTween.value(base.gameObject, 1f, 0f, 1f).setOnUpdate(delegate(float _value)
		{
			rigid.velocity *= _value;
		}).setOnComplete((Action)delegate
		{
			rigid.velocity = Vector3.zero;
			rigid.isKinematic = true;
		});
		navPathMove.AgentResetPath();
		navPathMove.AgentWarp(base.transform.position);
		navPathMove.AgentIsStopped(_isStopped: true);
	}
	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			attackType = debugAttackType;
		}
		if (attackInfo != null && (int)attackType < attackInfo.Length)
		{
			AttackColliderInfo attackColliderInfo = attackInfo[(int)attackType];
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(base.transform.position, attackColliderInfo.attackDistance);
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(attackColliderInfo.attackCollider.transform.position, attackColliderInfo.attackCollider.transform.localScale * base.transform.localScale.x);
		}
	}
}
