using SaveDataDefine;
using System;
using System.Collections.Generic;
using UnityEngine;
public class DragonBattlePlayerAI : MonoBehaviour
{
	[Serializable]
	public struct TargetEnemyData
	{
		public DragonBattleEnemyNinja enemy;
		public float time;
		public float distance;
		public TargetEnemyData(DragonBattleEnemyNinja _enemy, float _time, float _distance)
		{
			enemy = _enemy;
			time = _time;
			distance = _distance;
		}
		public void SetTime(float _time)
		{
			time = _time;
		}
	}
	private enum TargetType
	{
		None,
		CheckPoint,
		Coin,
		Box,
		Enemy,
		Standby,
		Attack
	}
	public enum AttackType
	{
		SWORD,
		SHURIKEN,
		WAIT,
		NONE
	}
	[Serializable]
	public struct AttackData
	{
		public float aimTime;
		public float enemyInterval;
		public float targetCheck;
		public DragonBattleEnemyNinja enemy;
		public float getCloser;
		public bool isLockOn;
		public bool isAimed;
		public AttackType attackType;
		public bool isShotCharge;
		public float chargeInterval;
		public float shotDelay;
		public Vector3 addMissShotPos;
		public float playerAtkCheckInterval;
		public void SettingTarget(float _aimTime, DragonBattleEnemyNinja _enemy, AttackType _type = AttackType.NONE)
		{
			aimTime = _aimTime;
			enemyInterval = 0f;
			getCloser = 0f;
			enemy = _enemy;
			isLockOn = false;
			isAimed = false;
			attackType = _type;
			chargeInterval = 0f;
			shotDelay = 0f;
			playerAtkCheckInterval = 0f;
		}
		public void ResetData(float _aimTime, bool _enemyData = true)
		{
			SettingTarget(_aimTime, _enemyData ? null : enemy);
		}
	}
	public class StrengthParam
	{
		public int missShot;
		public float targetMissRange;
		public int chargePer;
		public MinMaxValue chargeInterval;
		public MinMaxValue shotDelay;
		public int playerAtkPer;
		public StrengthParam(int _missShot, float _targetMissRange, int _chargePer, MinMaxValue _chargeInterval, MinMaxValue _shotDelay, int _playerAtkPer)
		{
			missShot = _missShot;
			targetMissRange = _targetMissRange;
			chargePer = _chargePer;
			chargeInterval = _chargeInterval;
			shotDelay = _shotDelay;
			playerAtkPer = _playerAtkPer;
		}
		public Vector3 AddMissShotPos()
		{
			Vector3 result = default(Vector3);
			result.x = UnityEngine.Random.Range(0f - targetMissRange, targetMissRange);
			result.y = 0f;
			result.z = UnityEngine.Random.Range(0f - targetMissRange, targetMissRange);
			return result;
		}
	}
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private DragonBattlePlayer player;
	private float respawnWait;
	private float actionWait;
	private Vector3 moveForce;
	private bool isJumpInput;
	private bool isSwordInput;
	private bool isShurikenInput;
	private RaycastHit raycastHit;
	private RaycastHit raycastHitLeft;
	private RaycastHit raycastHitRight;
	private float swordWait;
	private bool isHitLeft;
	private bool isHitRight;
	private bool isJumpTargetUpdate = true;
	private GameObject targetObj;
	private Vector3 targetPos;
	private Collider[] arrayWithinRange;
	private List<GameObject> listCheckPointLog = new List<GameObject>();
	private float checkPointDelay;
	private float standbyTime;
	private float stateTime;
	private List<TargetEnemyData> listEnemyLog = new List<TargetEnemyData>();
	private TargetType targetType;
	private readonly float standbyAreaWidthHalf = 5f;
	private readonly float searchRange = 6.5f;
	private readonly float[] searchRangeEnemy = new float[2]
	{
		25f,
		10f
	};
	private readonly float[] checkPointSearchRange = new float[3]
	{
		15f,
		10f,
		25f
	};
	private readonly float[] swordAttackRange = new float[2]
	{
		3f,
		5f
	};
	private readonly float AimTime = 0.2f;
	private AttackData attackData;
	private int aiStrength;
	private StrengthParam[] strengthParams = new StrengthParam[3]
	{
		new StrengthParam(30, 1.5f, 0, new MinMaxValue(2f, 5f), new MinMaxValue(0.2f, 0.25f), 0),
		new StrengthParam(15, 0.5f, 30, new MinMaxValue(1f, 3f), new MinMaxValue(0.1f, 0.2f), 10),
		new StrengthParam(5, 0.25f, 100, new MinMaxValue(0f, 1f), new MinMaxValue(0f, 0.1f), 25)
	};
	private StrengthParam strengthParam;
	public float ActionWait
	{
		set
		{
			actionWait = value;
		}
	}
	public TargetEnemyData[] ListEnemyLog => listEnemyLog.ToArray();
	public DragonBattleEnemyNinja AttackTarget => attackData.enemy;
	public void Init()
	{
		isJumpInput = false;
		targetPos = player.transform.position;
		aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		strengthParam = strengthParams[aiStrength];
		attackData.playerAtkCheckInterval = UnityEngine.Random.Range(5f, 7f);
		SettingChargeData();
	}
	public void SetRespawnWait()
	{
		respawnWait = 0.55f;
		moveForce = new Vector3(0f, 0f, 0f);
		isJumpInput = false;
		ResetTargetObj();
		attackData.attackType = AttackType.NONE;
	}
	public void UpdateAction()
	{
		respawnWait -= Time.deltaTime;
		if (respawnWait > 0f)
		{
			return;
		}
		actionWait -= Time.deltaTime;
		if (actionWait > 0f)
		{
			return;
		}
		attackData.enemyInterval -= Time.deltaTime;
		attackData.targetCheck -= Time.deltaTime;
		if (player.CurrentState != 0 && player.CurrentState != DragonBattlePlayer.State.SWORD_ATTACK && player.CurrentState != DragonBattlePlayer.State.SHURIKEN_ATTACK)
		{
			return;
		}
		if (Time.frameCount % 2 == 0)
		{
			if (SingletonCustom<DragonBattlePlayerManager>.Instance.IsProhibitAttack)
			{
				SearchCheckPoint();
			}
			else if (player.CurrentState == DragonBattlePlayer.State.SWORD_ATTACK || player.CurrentState == DragonBattlePlayer.State.SHURIKEN_ATTACK || player.Rigid.useGravity)
			{
				SearchCheckPoint();
			}
			else if (!SearchEnemy() && !SearchObject())
			{
				SearchCheckPoint();
			}
		}
		UpdateListEnemyLog();
		if (player.CurrentState == DragonBattlePlayer.State.DEFAULT)
		{
			if (SingletonCustom<DragonBattlePlayerManager>.Instance.IsProhibitAttack)
			{
				player.SetShurikenChargeTimePer(0f);
			}
			else if (attackData.isShotCharge)
			{
				attackData.chargeInterval -= Time.deltaTime;
				if (attackData.chargeInterval <= 0f)
				{
					player.AddShurikenChargeTimePer(Time.deltaTime);
				}
			}
		}
		StateAll();
		if (targetObj != null)
		{
			float num = CalcManager.Length(base.transform.position, targetObj.transform.position, _isCalcX: true, _isCalcY: false);
			float num2 = CalcManager.Length(base.transform.position, targetPos, _isCalcX: true, _isCalcY: false);
			switch (targetType)
			{
			case TargetType.Coin:
				if (!CheckInStandbyArea(targetObj.transform.position, SingletonCustom<DragonBattleFieldManager>.Instance.IsBossBattle))
				{
					ResetTargetObj();
				}
				else
				{
					targetPos = targetObj.transform.position;
				}
				break;
			case TargetType.Enemy:
				StateTargetEnemy(num);
				break;
			case TargetType.Box:
				if (targetObj.name == "DestroyWait" || !CheckInStandbyArea(targetObj.transform.position, SingletonCustom<DragonBattleFieldManager>.Instance.IsBossBattle))
				{
					ResetTargetObj();
					break;
				}
				targetPos = targetObj.transform.position;
				SettingAttack(AttackType.SHURIKEN);
				if (num <= 1f)
				{
					ResetTargetObj();
				}
				break;
			case TargetType.CheckPoint:
				stateTime += Time.deltaTime;
				if (num2 <= 1f || !CheckInStandbyArea(targetPos, SingletonCustom<DragonBattleFieldManager>.Instance.IsBossBattle))
				{
					listCheckPointLog.Add(targetObj);
					if (CheckInStandbyArea(player.transform.position, SingletonCustom<DragonBattleFieldManager>.Instance.IsBossBattle))
					{
						checkPointDelay = UnityEngine.Random.Range(1f, 2f);
					}
					else
					{
						checkPointDelay = 0f;
					}
					ResetTargetObj();
				}
				break;
			case TargetType.Standby:
				stateTime += Time.deltaTime;
				standbyTime -= Time.deltaTime;
				if (num2 <= 1f || standbyTime <= 0f || !CheckInStandbyArea(targetPos, SingletonCustom<DragonBattleFieldManager>.Instance.IsBossBattle))
				{
					ResetTargetObj();
				}
				break;
			case TargetType.Attack:
				targetPos = targetObj.transform.position;
				attackData.aimTime -= Time.deltaTime;
				if (attackData.aimTime <= 0f || num2 <= swordAttackRange[0])
				{
					switch (attackData.attackType)
					{
					case AttackType.SWORD:
					case AttackType.SHURIKEN:
						SettingAttack(attackData.attackType);
						break;
					case AttackType.NONE:
						ResetTargetObj();
						targetType = TargetType.None;
						break;
					}
				}
				break;
			}
		}
		else
		{
			targetType = TargetType.None;
		}
	}
	private void SettingAttack(AttackType _attackType, TargetType _targetType = TargetType.None)
	{
		switch (_attackType)
		{
		case AttackType.SWORD:
			isSwordInput = true;
			break;
		case AttackType.SHURIKEN:
			isShurikenInput = true;
			break;
		}
		if (_targetType == TargetType.Attack)
		{
			attackData.attackType = AttackType.WAIT;
		}
		else
		{
			attackData.attackType = _attackType;
		}
		attackData.enemyInterval = UnityEngine.Random.Range(1f, 3f);
		SettingChargeData();
	}
	private void SettingChargeData()
	{
		attackData.isShotCharge = (UnityEngine.Random.Range(0, 100) <= strengthParam.chargePer);
		attackData.chargeInterval = strengthParam.chargeInterval.Random();
	}
	private void StateTargetEnemy(float _targetDistance)
	{
		if (targetObj.name == "DestroyWait" || ((bool)attackData.enemy && !attackData.enemy.IsRender) || !attackData.enemy)
		{
			ResetTargetObj();
			return;
		}
		DragonBattlePlayer dragonBattlePlayer = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckTargetedEnemy(attackData.enemy.gameObject);
		if ((bool)dragonBattlePlayer && CalcManager.Length(attackData.enemy.gameObject, dragonBattlePlayer.gameObject) < CalcManager.Length(player.gameObject, dragonBattlePlayer.gameObject) - swordAttackRange[0])
		{
			ResetTargetObj();
			return;
		}
		if (_targetDistance <= swordAttackRange[0])
		{
			SettingAttack(AttackType.SWORD);
			targetPos = attackData.enemy.transform.position;
			return;
		}
		if (!attackData.isLockOn)
		{
			attackData.isLockOn = true;
			float time = CalcManager.Length(player.transform.position, targetObj.transform.position, _isCalcX: true, _isCalcY: false) / DragonBattleShuriken.MoveSpeed + AimTime;
			targetPos = attackData.enemy.GetPredictPos(time);
			attackData.addMissShotPos = Vector3.zero;
			if (UnityEngine.Random.Range(0, 100) <= strengthParam.missShot)
			{
				attackData.addMissShotPos = strengthParam.AddMissShotPos();
			}
			targetPos += attackData.addMissShotPos;
			attackData.getCloser = UnityEngine.Random.Range(0.1f, 0.25f);
			attackData.getCloser += strengthParam.shotDelay.Random();
		}
		else
		{
			attackData.getCloser -= Time.deltaTime;
			if (_targetDistance <= searchRangeEnemy[1] || attackData.getCloser <= 0f)
			{
				if (!attackData.isAimed)
				{
					attackData.isAimed = true;
					targetPos = attackData.enemy.GetPredictPos(AimTime);
					float num = CalcManager.Length(player.transform.position, targetPos, _isCalcX: true, _isCalcY: false) / DragonBattleShuriken.MoveSpeed;
					targetPos = attackData.enemy.GetPredictPos(AimTime + num);
					float num2 = CalcManager.Length(player.transform.position, targetPos, _isCalcX: true, _isCalcY: false) / DragonBattleShuriken.MoveSpeed;
					attackData.enemy.GetPredictPos(AimTime + num2);
					num = Math.Min(num, num2);
					targetPos = attackData.enemy.GetPredictPos(AimTime + num);
					targetPos += attackData.addMissShotPos;
				}
				attackData.aimTime -= Time.deltaTime;
				if (attackData.aimTime <= 0f)
				{
					SettingAttack(AttackType.SHURIKEN);
					listEnemyLog.Add(new TargetEnemyData(attackData.enemy, 3f, _targetDistance));
					attackData.ResetData(AimTime);
					targetPos = base.transform.position + base.transform.forward;
					targetObj = null;
					targetType = TargetType.None;
				}
			}
		}
		if (_targetDistance <= 1f)
		{
			ResetTargetObj();
		}
	}
	private void UpdateListEnemyLog()
	{
		bool flag = false;
		for (int num = listEnemyLog.Count - 1; num >= 0; num--)
		{
			flag = false;
			listEnemyLog[num].SetTime(listEnemyLog[num].time - Time.deltaTime);
			if (listEnemyLog[num].time <= 0f)
			{
				flag = true;
			}
			if (!listEnemyLog[num].enemy || listEnemyLog[num].enemy.gameObject.name == "DestroyWait")
			{
				flag = true;
			}
			if (!flag)
			{
				float num2 = CalcManager.Length(player.transform.position, listEnemyLog[num].enemy.transform.position, _isCalcX: true, _isCalcY: false);
				if (num2 <= listEnemyLog[num].distance * 0.75f)
				{
					flag = true;
				}
				if (num2 <= swordAttackRange[1])
				{
					flag = true;
				}
			}
			if (flag)
			{
				listEnemyLog.RemoveAt(num);
			}
		}
	}
	private bool SearchEnemy()
	{
		if (targetType == TargetType.Enemy)
		{
			return false;
		}
		if (attackData.enemyInterval > 0f)
		{
			return false;
		}
		if (SingletonCustom<DragonBattleFieldManager>.Instance.IsBossBattle && player.transform.position.z > SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().position.z + checkPointSearchRange[2] * 0.5f)
		{
			return false;
		}
		arrayWithinRange = Physics.OverlapSphere(base.transform.position, searchRangeEnemy[0]);
		if (arrayWithinRange.Length != 0)
		{
			Array.Sort(arrayWithinRange, (Collider a, Collider b) => (int)(CalcManager.Length(base.transform.position, a.transform.position) - CalcManager.Length(base.transform.position, b.transform.position)));
			for (int i = 0; i < arrayWithinRange.Length; i++)
			{
				if (arrayWithinRange[i].name == "DestroyWait" || CheckTargetEnemy(arrayWithinRange[i].gameObject) || arrayWithinRange[i].transform.position.z < SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().position.z + checkPointSearchRange[1] || !(arrayWithinRange[i].tag == "Ninja"))
				{
					continue;
				}
				DragonBattleEnemyNinja dragonBattleEnemyNinja = SingletonCustom<DragonBattleFieldManager>.Instance.CheckEnemy(arrayWithinRange[i].gameObject);
				if (!dragonBattleEnemyNinja.IsBoss)
				{
					DragonBattlePlayer dragonBattlePlayer = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckTargetedEnemy(dragonBattleEnemyNinja.gameObject);
					if ((bool)dragonBattlePlayer && CalcManager.Length(dragonBattleEnemyNinja.gameObject, dragonBattlePlayer.gameObject) < CalcManager.Length(player.gameObject, dragonBattlePlayer.gameObject) - swordAttackRange[0])
					{
						continue;
					}
				}
				targetObj = dragonBattleEnemyNinja.gameObject;
				attackData.SettingTarget(AimTime, dragonBattleEnemyNinja);
				if (attackData.enemy.IsRender)
				{
					targetPos = targetObj.transform.position;
					targetType = TargetType.Enemy;
					return true;
				}
			}
		}
		return false;
	}
	private bool SearchObject()
	{
		if (targetType != 0)
		{
			return false;
		}
		arrayWithinRange = Physics.OverlapSphere(base.transform.position, searchRange, DragonBattleDefine.ConvertLayerMask("Field") | DragonBattleDefine.ConvertLayerMask("HitCharacterOnly"), QueryTriggerInteraction.Collide);
		if (arrayWithinRange.Length != 0)
		{
			Array.Sort(arrayWithinRange, (Collider a, Collider b) => (int)(CalcManager.Length(base.transform.position, a.transform.position) - CalcManager.Length(base.transform.position, b.transform.position)));
			for (int i = 0; i < arrayWithinRange.Length; i++)
			{
				if (arrayWithinRange[i].transform.position.z > SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().position.z + SingletonCustom<DragonBattleFieldManager>.Instance.CREATE_SPAN || arrayWithinRange[i].name == "DestroyWait" || arrayWithinRange[i].transform.position.z < base.transform.position.z + 0.015f)
				{
					continue;
				}
				if (arrayWithinRange[i].tag == "Coin")
				{
					targetObj = arrayWithinRange[i].gameObject;
					targetPos = targetObj.transform.position;
					targetType = TargetType.Coin;
					return true;
				}
				if (arrayWithinRange[i].tag == "Box")
				{
					targetObj = arrayWithinRange[i].gameObject;
					targetPos = targetObj.transform.position;
					targetType = TargetType.Box;
					if (UnityEngine.Random.Range(0, 2) == 0)
					{
						SettingAttack(AttackType.SHURIKEN);
					}
					return true;
				}
			}
		}
		return false;
	}
	private bool SearchCheckPoint()
	{
		if (checkPointDelay > 0f)
		{
			if (!CheckInStandbyArea(player.transform.position, SingletonCustom<DragonBattleFieldManager>.Instance.IsBossBattle))
			{
				checkPointDelay = 0f;
			}
			checkPointDelay -= Time.deltaTime;
			return false;
		}
		if (targetType != 0)
		{
			return false;
		}
		float num = player.transform.position.z - SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().position.z;
		num -= checkPointSearchRange[1];
		num = Mathf.Max(num, 0f);
		num /= checkPointSearchRange[2] - checkPointSearchRange[1];
		num = Math.Min(30f + num * 100f, 90f);
		if (CalcManager.IsPerCheck(num))
		{
			standbyTime = UnityEngine.Random.Range(1f, 2f);
			targetType = TargetType.Standby;
			targetObj = player.gameObject;
			targetPos = player.transform.position;
			targetPos = ConvertStandbyArea(targetPos, SingletonCustom<DragonBattleFieldManager>.Instance.IsBossBattle);
			stateTime = 0f;
		}
		else
		{
			arrayWithinRange = Physics.OverlapSphere(base.transform.position, checkPointSearchRange[0]);
			if (arrayWithinRange.Length != 0)
			{
				arrayWithinRange.Shuffle();
				for (int i = 0; i < arrayWithinRange.Length; i++)
				{
					if (arrayWithinRange[i].tag == "CheckPoint" && !(arrayWithinRange[i].transform.position.z < SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().position.z + checkPointSearchRange[1]) && !(arrayWithinRange[i].transform.position.z > SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().position.z + checkPointSearchRange[2]) && (listCheckPointLog.Count <= 0 || !(listCheckPointLog[listCheckPointLog.Count - 1] == arrayWithinRange[i].gameObject)))
					{
						targetObj = arrayWithinRange[i].gameObject;
						targetPos = targetObj.transform.position;
						targetPos.x += UnityEngine.Random.Range(0f - searchRange, searchRange) * 0.5f;
						targetPos.z += UnityEngine.Random.Range(0f - searchRange, searchRange) * 0.5f;
						targetPos = ConvertStandbyArea(targetPos, SingletonCustom<DragonBattleFieldManager>.Instance.IsBossBattle);
						targetType = TargetType.CheckPoint;
						stateTime = 0f;
						return true;
					}
				}
			}
		}
		return false;
	}
	private void ResetTargetObj()
	{
		targetPos = base.transform.position + Vector3.forward;
		targetObj = null;
		targetType = TargetType.None;
		stateTime = 0f;
	}
	private void StateAll()
	{
		if (!CheckInStandbyArea(player.transform.position, SingletonCustom<DragonBattleFieldManager>.Instance.IsBossBattle) || targetType == TargetType.Enemy || attackData.enemyInterval > 0f || attackData.targetCheck > 0f)
		{
			return;
		}
		attackData.targetCheck = 0.5f;
		if (player.MoveSpeed >= player.MOVE_SPEED_MAX * 0.75f && Physics.Raycast(base.transform.position + Vector3.up * 0.5f, player.Style.transform.forward, out raycastHit, swordAttackRange[1] * 2f, DragonBattleDefine.ConvertLayerMask("Field") | DragonBattleDefine.ConvertLayerMask("HitCharacterOnly") | DragonBattleDefine.ConvertLayerMask("Character")))
		{
			bool flag = false;
			if (raycastHit.transform.tag == "Player")
			{
				DragonBattlePlayer dragonBattlePlayer = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckPlayer(raycastHit.transform.gameObject);
				if (dragonBattlePlayer != player && !dragonBattlePlayer.CheckState(DragonBattlePlayer.State.KNOCK_BACK))
				{
					flag = true;
					targetObj = dragonBattlePlayer.gameObject;
				}
			}
			if (raycastHit.transform.tag == "Box" && raycastHit.transform.gameObject.name != "DestroyWait")
			{
				flag = true;
				targetObj = raycastHit.transform.gameObject;
			}
			if (flag && UnityEngine.Random.Range(0f, 100f) <= 10f)
			{
				if (targetObj.tag != "Box" && raycastHit.distance <= swordAttackRange[0])
				{
					attackData.attackType = AttackType.SWORD;
				}
				else
				{
					attackData.attackType = AttackType.SHURIKEN;
				}
				targetPos = targetObj.transform.position;
				targetType = TargetType.Attack;
				attackData.targetCheck = 3f;
			}
		}
		arrayWithinRange = Physics.OverlapSphere(base.transform.position, swordAttackRange[0] * 0.5f);
		bool flag2 = false;
		if (arrayWithinRange.Length != 0)
		{
			for (int i = 0; i < arrayWithinRange.Length; i++)
			{
				flag2 = false;
				if (arrayWithinRange[i].transform.tag == "Ninja" && SingletonCustom<DragonBattleFieldManager>.Instance.CheckEnemy(arrayWithinRange[i].gameObject).name != "DestroyWait")
				{
					targetObj = SingletonCustom<DragonBattleFieldManager>.Instance.CheckEnemy(arrayWithinRange[i].gameObject).gameObject;
					flag2 = true;
				}
				if (arrayWithinRange[i].transform.tag == "Player")
				{
					DragonBattlePlayer dragonBattlePlayer2 = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckPlayer(arrayWithinRange[i].gameObject);
					if (dragonBattlePlayer2 != player && !dragonBattlePlayer2.CheckState(DragonBattlePlayer.State.KNOCK_BACK))
					{
						targetObj = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckPlayer(arrayWithinRange[i].gameObject).gameObject;
						flag2 = true;
					}
				}
				if (flag2 && UnityEngine.Random.Range(0f, 100f) <= 10f)
				{
					targetPos = targetObj.transform.position;
					attackData.aimTime = 0.2f;
					attackData.attackType = AttackType.SWORD;
					targetType = TargetType.Attack;
					attackData.targetCheck = 3f;
					break;
				}
			}
		}
		attackData.playerAtkCheckInterval -= Time.deltaTime;
		if (attackData.playerAtkCheckInterval <= 0f)
		{
			attackData.playerAtkCheckInterval = UnityEngine.Random.Range(5f, 7f);
			if (UnityEngine.Random.Range(0, 100) <= strengthParam.playerAtkPer && SingletonCustom<DragonBattlePlayerManager>.Instance.Search1stScorePlayer() != player)
			{
				targetObj = SingletonCustom<DragonBattlePlayerManager>.Instance.Search1stScorePlayer().gameObject;
				targetPos = targetObj.transform.position;
				attackData.aimTime = 0.2f;
				attackData.attackType = AttackType.SHURIKEN;
				targetType = TargetType.Attack;
				attackData.targetCheck = 3f;
			}
		}
	}
	private void StateJump()
	{
	}
	public Vector3 UpdateForce()
	{
		if (!(actionWait > 0f) && !(respawnWait > 0f))
		{
			if (targetObj == null)
			{
				if (player.IsJump && isJumpTargetUpdate)
				{
					moveForce = (targetPos - player.transform.position).normalized;
				}
				else
				{
					moveForce = moveForce.normalized;
				}
			}
			else
			{
				moveForce = (targetPos - base.transform.position).normalized;
			}
		}
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
		{
		case SystemData.AiStrength.WEAK:
			moveForce *= 0.65f;
			break;
		case SystemData.AiStrength.NORAML:
			if (!player.IsJump)
			{
				moveForce *= 0.65f;
			}
			break;
		}
		return moveForce;
	}
	public bool IsJumpInput()
	{
		return false;
	}
	public bool IsSwordInput()
	{
		if (isSwordInput)
		{
			swordWait -= Time.deltaTime;
			if (swordWait <= 0f)
			{
				isSwordInput = false;
				switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
				{
				case SystemData.AiStrength.WEAK:
					swordWait = 0.1f;
					break;
				case SystemData.AiStrength.NORAML:
					swordWait = 0.05f;
					break;
				case SystemData.AiStrength.STRONG:
					swordWait = 0.025f;
					break;
				}
				attackData.attackType = AttackType.NONE;
				return true;
			}
		}
		return false;
	}
	public bool IsShurikenInput()
	{
		if (isShurikenInput)
		{
			isShurikenInput = false;
			attackData.attackType = AttackType.NONE;
			return true;
		}
		return false;
	}
	public bool UpdateClimbing()
	{
		return Time.frameCount % 15 == 0;
	}
	private bool CheckTargetEnemy(GameObject _enemy)
	{
		for (int i = 0; i < listEnemyLog.Count; i++)
		{
			if (!(listEnemyLog[i].enemy == null) && _enemy == listEnemyLog[i].enemy.gameObject)
			{
				return true;
			}
		}
		return false;
	}
	private bool CheckInStandbyArea(Vector3 _pos, bool _isBossBattle)
	{
		if (!CalcManager.CheckRange(_pos.z, SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().position.z + checkPointSearchRange[1], SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().position.z + checkPointSearchRange[2] * (_isBossBattle ? 0.5f : 1f)))
		{
			return false;
		}
		_pos = SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().InverseTransformPoint(_pos);
		_pos.x = Mathf.Min(Math.Abs(_pos.x), standbyAreaWidthHalf) * Mathf.Sign(_pos.x);
		_pos = SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().TransformPoint(_pos);
		return true;
	}
	private Vector3 ConvertStandbyArea(Vector3 _pos, bool _isBossBattle)
	{
		_pos.z = Mathf.Clamp(_pos.z, SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().position.z + checkPointSearchRange[1], SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().position.z + checkPointSearchRange[2] * (_isBossBattle ? 0.5f : 1f));
		_pos = SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().InverseTransformPoint(_pos);
		_pos.x = Mathf.Min(Math.Abs(_pos.x), standbyAreaWidthHalf) * Mathf.Sign(_pos.x);
		_pos = SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().TransformPoint(_pos);
		return _pos;
	}
	private void OnDrawGizmos()
	{
		if ((bool)player)
		{
			bool activeSelf = player.gameObject.activeSelf;
		}
	}
}
