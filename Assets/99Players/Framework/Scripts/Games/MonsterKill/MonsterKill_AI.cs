using SaveDataDefine;
using UnityEngine;
using UnityEngine.AI;
public class MonsterKill_AI : MonoBehaviour
{
	private MonsterKill_Player player;
	private NavMeshAgent agent;
	private MonsterKill_NavPathMove navPathMove;
	private MonsterKill_Enemy targetEnemy;
	private float targetEnemyDistance;
	private Vector3 moveVector;
	private int aiStrength;
	private int originAIStrength;
	[SerializeField]
	[Header("魔法を溜める最小距離")]
	private float MAGIC_ATTACK_MIN_DISTANCE = 2f;
	[SerializeField]
	[Header("魔法を溜める最大距離")]
	private float MAGIC_ATTACK_MAX_DISTANCE = 4f;
	[SerializeField]
	[Header("魔法で攻撃する角度")]
	private float MAGIC_ATTACK_ANGLE = 30f;
	[SerializeField]
	[Header("剣の攻撃可能な距離")]
	private float SWORD_ATTACK_DISTANCE = 0.75f;
	[SerializeField]
	[Header("剣で攻撃する角度")]
	private float SWORD_ATTACK_ANGLE = 30f;
	[SerializeField]
	[Header("攻撃可能な状態で、別のモンスタ\u30fcを探しに行くかどうかを判定する距離")]
	private float ATTACK_PURSUE_DISTANCE = 3f;
	[SerializeField]
	[Header("攻撃可能な状態で、別のモンスタ\u30fcを探しに行くまでの時間")]
	private float ATTACK_PURSUE_TIME = 3f;
	private float attackPursueTime;
	private float[] NEXT_SEARCH_TARGET_ENEMY_INTERVAL;
	private float nextSearchTargetEnemyInterval;
	private float NOT_SEARCH_TARGET_ENEMY_DISTANCE = 3f;
	private float[] MAGIC_ATTACK_INPUT_INTERVAL;
	private float magicAtttackInputInterval;
	private float[] MAGIC_ATTACK_INPUT_PROBABILITY;
	private float[] SWORD_ATTACK_INPUT_INTERVAL;
	private float swordAtttackInputInterval;
	private readonly float DASH_INPUT_INTERVAL = 3f;
	private float dashInputInterval;
	private float[] DASH_INPUT_PROBABILITY;
	private bool isDashInput;
	private float[] STOP_DASH_STAMINA;
	private float stopDashStamina;
	private float[] IN_ATTACK_RANGE_ATTACK_PROBABILITY;
	private bool isInAttackRangeAttack;
	private float changeStrengthDelayTime;
	private readonly float RELOTTERY_TIME = 3f;
	private float relotteryTime;
	public void Init(MonsterKill_Player _player)
	{
		player = _player;
		agent = player.GetNavMeshAgent();
		navPathMove = base.gameObject.AddComponent<MonsterKill_NavPathMove>();
		navPathMove.Init(agent);
		navPathMove.AgentWarp(base.transform.position);
		this.aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		originAIStrength = this.aiStrength;
		SystemData.AiStrength aiStrength = (SystemData.AiStrength)this.aiStrength;
		if ((uint)(aiStrength - 1) <= 1u && SingletonCustom<GameSettingManager>.Instance.PlayerNum <= 2)
		{
			this.aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength - 1;
			int idx = (int)(player.GetUserType() - 4);
			changeStrengthDelayTime = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetChangeAIStrengthDelayTime(idx);
		}
		SetAIStrengthData();
		magicAtttackInputInterval = UnityEngine.Random.Range(MAGIC_ATTACK_INPUT_INTERVAL[0], MAGIC_ATTACK_INPUT_INTERVAL[1]);
		swordAtttackInputInterval = UnityEngine.Random.Range(SWORD_ATTACK_INPUT_INTERVAL[0], SWORD_ATTACK_INPUT_INTERVAL[1]);
		dashInputInterval = DASH_INPUT_INTERVAL;
		SetLottery();
		relotteryTime = RELOTTERY_TIME;
	}
	private void SetAIStrengthData()
	{
		player.SetLimitMoveSpeed(MonsterKill_Define.CPU_LIMIT_MOVE_SPEED[aiStrength]);
		NEXT_SEARCH_TARGET_ENEMY_INTERVAL = MonsterKill_Define.CPU_NEXT_SEARCH_TARGET_ENEMY_INTERVAL[aiStrength];
		MAGIC_ATTACK_INPUT_INTERVAL = MonsterKill_Define.CPU_MAGIC_ATTACK_INPUT_INTERVAL[aiStrength];
		MAGIC_ATTACK_INPUT_PROBABILITY = MonsterKill_Define.CPU_MAGIC_ATTACK_INPUT_PROBABILITY[aiStrength];
		SWORD_ATTACK_INPUT_INTERVAL = MonsterKill_Define.CPU_SWORD_ATTACK_INPUT_INTERVAL[aiStrength];
		DASH_INPUT_PROBABILITY = MonsterKill_Define.CPU_DASH_INPUT_PROBABILITY[aiStrength];
		STOP_DASH_STAMINA = MonsterKill_Define.CPU_STOP_DASH_STAMINA[aiStrength];
		IN_ATTACK_RANGE_ATTACK_PROBABILITY = MonsterKill_Define.CPU_IN_ATTACK_RANGE_ATTACK_PROBABILITY[aiStrength];
	}
	private void SetLottery()
	{
		isInAttackRangeAttack = (UnityEngine.Random.Range(0f, 1f) < UnityEngine.Random.Range(IN_ATTACK_RANGE_ATTACK_PROBABILITY[0], IN_ATTACK_RANGE_ATTACK_PROBABILITY[1]));
	}
	public void UpdateMethod()
	{
		if (aiStrength != originAIStrength)
		{
			changeStrengthDelayTime -= Time.deltaTime;
			if (changeStrengthDelayTime < 0f)
			{
				aiStrength = originAIStrength;
				originAIStrength = aiStrength;
				SetAIStrengthData();
			}
		}
		else
		{
			switch (aiStrength)
			{
			case 1:
				if (player.GetPoint() >= 6000)
				{
					aiStrength = 0;
					SetAIStrengthData();
				}
				break;
			case 2:
				if (player.GetPoint() >= 6500)
				{
					aiStrength = 0;
					SetAIStrengthData();
				}
				break;
			}
		}
		relotteryTime -= Time.deltaTime;
		if (relotteryTime < 0f)
		{
			relotteryTime = RELOTTERY_TIME;
			SetLottery();
		}
		if (targetEnemy == null || targetEnemy.GetIsDead())
		{
			if (nextSearchTargetEnemyInterval > 0f)
			{
				nextSearchTargetEnemyInterval -= Time.deltaTime;
			}
			targetEnemy = GetNearTargetEnemy(nextSearchTargetEnemyInterval > 0f);
			if (targetEnemy == null)
			{
				player.MoveStop();
				return;
			}
			nextSearchTargetEnemyInterval = UnityEngine.Random.Range(NEXT_SEARCH_TARGET_ENEMY_INTERVAL[0], NEXT_SEARCH_TARGET_ENEMY_INTERVAL[1]);
			attackPursueTime = ATTACK_PURSUE_TIME;
			navPathMove.ResetPrevMyPos();
			navPathMove.ResetPrevTaretPos();
		}
		if (GetIsTargetEnemyAttackDistance())
		{
			moveVector = (targetEnemy.transform.position - base.transform.position).normalized;
			player.SetMoveDir(moveVector);
		}
		else
		{
			navPathMove.SetMoveDestination(targetEnemy.transform.position);
			navPathMove.UpdateMethod();
			moveVector = navPathMove.GetMoveVector();
			player.SetMoveDir(moveVector.normalized);
		}
		if (!player.GetIsMagicCast() && !player.GetIsMagicAttack() && magicAtttackInputInterval > 0f)
		{
			magicAtttackInputInterval -= Time.deltaTime;
		}
		if (player.GetIsNone() && !player.GetIsActionWait())
		{
			if (magicAtttackInputInterval < 0f && targetEnemyDistance > MAGIC_ATTACK_MIN_DISTANCE && targetEnemyDistance <= MAGIC_ATTACK_MAX_DISTANCE)
			{
				magicAtttackInputInterval = UnityEngine.Random.Range(MAGIC_ATTACK_INPUT_INTERVAL[0], MAGIC_ATTACK_INPUT_INTERVAL[1]);
				if (UnityEngine.Random.Range(0f, 1f) < UnityEngine.Random.Range(MAGIC_ATTACK_INPUT_PROBABILITY[0], MAGIC_ATTACK_INPUT_PROBABILITY[1]))
				{
					player.MagicCast();
					return;
				}
			}
		}
		else if (player.GetIsMagicCast() && targetEnemyDistance <= MAGIC_ATTACK_MAX_DISTANCE && Vector3.Angle(moveVector, base.transform.forward) <= MAGIC_ATTACK_ANGLE)
		{
			player.MagicAttack();
			return;
		}
		if (player.GetIsMagicCast() || player.GetIsMagicAttack())
		{
			player.NoneUseDash();
		}
		else
		{
			if (!player.GetIsCanSwordAttack())
			{
				return;
			}
			if (player.GetMoveDir() != Vector3.zero && !player.GetIsUseUpStamina() && !isDashInput)
			{
				if (dashInputInterval > 0f)
				{
					dashInputInterval -= Time.deltaTime;
				}
				else
				{
					dashInputInterval = DASH_INPUT_INTERVAL;
					if (UnityEngine.Random.Range(0f, 1f) < UnityEngine.Random.Range(DASH_INPUT_PROBABILITY[0], DASH_INPUT_PROBABILITY[1]))
					{
						isDashInput = true;
						stopDashStamina = (float)SingletonCustom<MonsterKill_PlayerManager>.Instance.GetMaxStamina() * UnityEngine.Random.Range(STOP_DASH_STAMINA[0], STOP_DASH_STAMINA[1]);
					}
				}
			}
			if (swordAtttackInputInterval > 0f)
			{
				swordAtttackInputInterval -= Time.deltaTime;
			}
			else if (!player.GetIsActionWait())
			{
				if (isInAttackRangeAttack)
				{
					MonsterKill_Enemy nearTargetEnemy = GetNearTargetEnemy(_isSearchTargetInterval: false, targetEnemy);
					if (nearTargetEnemy != null)
					{
						targetEnemyDistance = CalcManager.Length(nearTargetEnemy.transform.position, base.transform.position);
						if (GetIsTargetEnemyAttackDistance())
						{
							targetEnemy = nearTargetEnemy;
						}
					}
				}
				targetEnemyDistance = CalcManager.Length(targetEnemy.transform.position, base.transform.position);
				if (GetIsTargetEnemyAttackDistance() && Vector3.Angle(moveVector, base.transform.forward) <= SWORD_ATTACK_ANGLE)
				{
					isDashInput = false;
					swordAtttackInputInterval = UnityEngine.Random.Range(SWORD_ATTACK_INPUT_INTERVAL[0], SWORD_ATTACK_INPUT_INTERVAL[1]);
					player.SwordAttack();
					return;
				}
				if (targetEnemyDistance <= ATTACK_PURSUE_DISTANCE)
				{
					if (!(attackPursueTime > 0f))
					{
						targetEnemy = GetNearTargetEnemy(_isSearchTargetInterval: false, targetEnemy);
						if (targetEnemy == null)
						{
							player.MoveStop();
						}
						else
						{
							attackPursueTime = ATTACK_PURSUE_TIME;
						}
						return;
					}
					attackPursueTime -= Time.deltaTime;
				}
			}
			if (player.GetMoveDir() != Vector3.zero && !player.GetIsUseUpStamina() && isDashInput && player.GetStamina() > stopDashStamina)
			{
				player.UseDash();
				return;
			}
			isDashInput = false;
			player.NoneUseDash();
		}
	}
	private MonsterKill_Enemy GetNearTargetEnemy(bool _isSearchTargetInterval, MonsterKill_Enemy _prevTargetEnemy = null)
	{
		if (SingletonCustom<MonsterKill_EnemyManager>.Instance.GetEnemyList().Count == 0)
		{
			return null;
		}
		int num = -1;
		float num2 = 10000f;
		for (int i = 0; i < SingletonCustom<MonsterKill_EnemyManager>.Instance.GetEnemyList().Count; i++)
		{
			MonsterKill_Enemy monsterKill_Enemy = SingletonCustom<MonsterKill_EnemyManager>.Instance.GetEnemyList()[i];
			if (monsterKill_Enemy == _prevTargetEnemy || monsterKill_Enemy.GetIsDead())
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
			{
				MonsterKill_Player monsterKill_Player = SingletonCustom<MonsterKill_PlayerManager>.Instance.GetPlayer(j);
				if (monsterKill_Player.GetIsCpu() && monsterKill_Player.GetPlayerNo() != player.GetPlayerNo() && monsterKill_Player.GetTargetEnemy() == monsterKill_Enemy)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				float num3 = CalcManager.Length(base.transform.position, monsterKill_Enemy.transform.position);
				if ((!_isSearchTargetInterval || !(num3 > NOT_SEARCH_TARGET_ENEMY_DISTANCE)) && num3 < num2)
				{
					num = i;
					num2 = num3;
				}
			}
		}
		if (num == -1)
		{
			return null;
		}
		return SingletonCustom<MonsterKill_EnemyManager>.Instance.GetEnemyList()[num];
	}
	public MonsterKill_Enemy GetTargetEnemy()
	{
		return targetEnemy;
	}
	public bool GetIsTargetEnemyAttackDistance()
	{
		return targetEnemyDistance <= SWORD_ATTACK_DISTANCE;
	}
}
