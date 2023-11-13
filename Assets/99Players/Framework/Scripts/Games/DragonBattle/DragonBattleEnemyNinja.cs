using System;
using UnityEngine;
public class DragonBattleEnemyNinja : MonoBehaviour
{
	public enum State
	{
		DEFAULT,
		DEAD
	}
	[Serializable]
	public class Move_Data
	{
		public bool isMove;
		public float interval;
		public float delay;
		public float time;
		public Vector3 targetPos;
		public float speed = 10f;
		public float rotSpeed = 10f;
	}
	public struct MinMaxData
	{
		public float min;
		public float max;
		public MinMaxData(float _base, float _range)
		{
			min = _base - _range * 0.5f;
			max = _base + _range * 0.5f;
		}
		public float RandomRange()
		{
			return UnityEngine.Random.Range(min, max);
		}
	}
	public class AttackPattern
	{
		public class DirData
		{
			public bool is1st;
			public MinMaxData rotData;
			public bool isRandom;
			public DirData(MinMaxData _rotData = default(MinMaxData), bool _is1st = false, bool _isRandom = false)
			{
				is1st = _is1st;
				rotData = _rotData;
				isRandom = _isRandom;
			}
		}
		public float interval;
		private int no;
		private DirData[] dirData;
		public int AttackNo => no;
		public DirData Data => dirData[no];
		public AttackPattern(float _interval, DirData[] _data)
		{
			no = 0;
			interval = _interval;
			dirData = _data;
		}
		public void NextData()
		{
			no++;
		}
		public void Start()
		{
			no = 0;
		}
		public bool CheckFinish()
		{
			return no >= dirData.Length;
		}
	}
	[Serializable]
	public class AttackData
	{
		public Vector3[] createLocalPosList;
		public bool isAttacking;
		public bool isAtkAnim;
		public float time;
		public float createDelay;
		public float interval;
		public int patternNo;
		private AttackPattern[] patterns;
		public int atkType;
		public float attackRot;
		public AttackPattern Pattern => patterns[patternNo];
		public AttackData(Vector3[] _createLocalPosList, AttackPattern[] _datas)
		{
			createLocalPosList = _createLocalPosList;
			isAttacking = false;
			isAtkAnim = false;
			time = 0f;
			patternNo = 0;
			interval = 0f;
			createDelay = 0f;
			patterns = _datas;
		}
		public void AttackStart(DragonBattleDefine.EnemyType _enemyType)
		{
			SettingAtkType(_enemyType);
			isAttacking = true;
			switch (_enemyType)
			{
			case DragonBattleDefine.EnemyType.DRAGON_BOSS:
				interval = UnityEngine.Random.Range(0.5f, 1f);
				break;
			case DragonBattleDefine.EnemyType.FLYING_DEMON:
				interval = UnityEngine.Random.Range(1f, 3f);
				break;
			default:
				interval = UnityEngine.Random.Range(0.5f, 1.5f);
				break;
			}
			createDelay = 0f;
			time = 0f;
			patternNo = UnityEngine.Random.Range(0, patterns.Length);
			Pattern.Start();
			UnityEngine.Debug.Log("攻撃開始 : patternNo = " + patternNo.ToString());
		}
		private void SettingAtkType(DragonBattleDefine.EnemyType _enemyType)
		{
			switch (_enemyType)
			{
			case DragonBattleDefine.EnemyType.DRAGON_BOSS:
				if (UnityEngine.Random.Range(0, 100) <= 30)
				{
					atkType = 1;
				}
				else
				{
					atkType = 0;
				}
				break;
			case DragonBattleDefine.EnemyType.FLYING_DEMON:
				atkType = 0;
				break;
			}
		}
		public void AttackEnd()
		{
			isAttacking = false;
		}
	}
	[SerializeField]
	[Header("敵タイプ")]
	private DragonBattleDefine.EnemyType enemyType;
	public static DragonBattleDefine.EnemyType bossEnemyTypeBuf = DragonBattleDefine.EnemyType.MAX;
	private DragonBattleDefine.EnemyType enemyTypeBuf = DragonBattleDefine.EnemyType.MAX;
	[SerializeField]
	[Header("ボスフラグ")]
	private bool isBoss;
	private GameObject enemyObj;
	private int colorNo;
	private DragonBattleEnemyAnim anim;
	private DragonBattlePlayer target;
	[SerializeField]
	[Header("手裏剣")]
	private DragonBattleShuriken shuriken;
	[SerializeField]
	[Header("煙エフェクト")]
	private ParticleSystem psSmoke;
	[SerializeField]
	[Header("表示ル\u30fcト")]
	private GameObject rootDisp;
	[SerializeField]
	[Header("HPゲ\u30fcジアンカ\u30fc")]
	private Transform hpGaugeAnchor;
	private readonly float SEARCH_DISTANCE_MAX = 20f;
	private State currentState;
	private float waitTime;
	private readonly float ATTACK_WAIT = 0.25f;
	private DragonBattlePlayer[] tempArray;
	private float distance;
	[SerializeField]
	[Header("パス移動")]
	private CinemachinePathMove pathMove;
	private Move_Data moveData = new Move_Data();
	private IsRendered[] isRendered;
	private bool isInit;
	private bool isActive;
	private bool isRender;
	private float hpMax = 150f;
	private float hp;
	private Vector3 localPosDef;
	private AttackData[] attackDataList = new AttackData[2]
	{
		new AttackData(new Vector3[1]
		{
			new Vector3(0f, -0.5f, 7f)
		}, new AttackPattern[4]
		{
			new AttackPattern(0.9f, new AttackPattern.DirData[5]
			{
				new AttackPattern.DirData(default(MinMaxData), _is1st: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: false, _isRandom: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: false, _isRandom: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: true)
			}),
			new AttackPattern(0.75f, new AttackPattern.DirData[3]
			{
				new AttackPattern.DirData(default(MinMaxData), _is1st: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: true)
			}),
			new AttackPattern(0.8f, new AttackPattern.DirData[5]
			{
				new AttackPattern.DirData(default(MinMaxData), _is1st: false, _isRandom: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: false, _isRandom: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: false, _isRandom: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: false, _isRandom: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: false, _isRandom: true)
			}),
			new AttackPattern(0.75f, new AttackPattern.DirData[5]
			{
				new AttackPattern.DirData(new MinMaxData(0f, 60f)),
				new AttackPattern.DirData(new MinMaxData(0f, 60f)),
				new AttackPattern.DirData(new MinMaxData(0f, 60f)),
				new AttackPattern.DirData(new MinMaxData(0f, 60f)),
				new AttackPattern.DirData(new MinMaxData(0f, 60f))
			})
		}),
		new AttackData(new Vector3[1]
		{
			new Vector3(-0.18f, 0.6f, 3.5f)
		}, new AttackPattern[4]
		{
			new AttackPattern(0f, new AttackPattern.DirData[2]
			{
				new AttackPattern.DirData(new MinMaxData(-15f, 0f)),
				new AttackPattern.DirData(new MinMaxData(15f, 0f))
			}),
			new AttackPattern(0f, new AttackPattern.DirData[3]
			{
				new AttackPattern.DirData(new MinMaxData(-35f, 0f)),
				new AttackPattern.DirData(new MinMaxData(0f, 0f)),
				new AttackPattern.DirData(new MinMaxData(35f, 0f))
			}),
			new AttackPattern(0f, new AttackPattern.DirData[4]
			{
				new AttackPattern.DirData(new MinMaxData(-30f, 0f)),
				new AttackPattern.DirData(new MinMaxData(-15f, 0f)),
				new AttackPattern.DirData(new MinMaxData(15f, 0f)),
				new AttackPattern.DirData(new MinMaxData(30f, 0f))
			}),
			new AttackPattern(0f, new AttackPattern.DirData[4]
			{
				new AttackPattern.DirData(default(MinMaxData), _is1st: false, _isRandom: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: false, _isRandom: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: false, _isRandom: true),
				new AttackPattern.DirData(default(MinMaxData), _is1st: false, _isRandom: true)
			})
		})
	};
	private AttackData attackData;
	public DragonBattleDefine.EnemyType EnemyType => enemyType;
	public bool IsBoss => isBoss;
	public Transform HpGaugeAnchor => hpGaugeAnchor;
	public bool IsRender
	{
		get
		{
			for (int i = 0; i < isRendered.Length; i++)
			{
				if (isRendered[i].IsRender)
				{
					return true;
				}
			}
			return false;
		}
	}
	public void Init()
	{
		SettingEnemyType();
		tempArray = SingletonCustom<DragonBattlePlayerManager>.Instance.GetArrayPlayer();
		pathMove.Init(base.transform.parent, _isPlayOnStart: false);
		enemyObj = UnityEngine.Object.Instantiate(SingletonCustom<DragonBattleResources>.Instance.EnemyList[(int)enemyType], base.transform.position, Quaternion.identity, base.transform);
		enemyObj.transform.SetLocalEulerAnglesY(0f);
		enemyObj.transform.localPosition = SingletonCustom<DragonBattleResources>.Instance.EnemyList[(int)enemyType].transform.localPosition;
		anim = enemyObj.GetComponent<DragonBattleEnemyAnim>();
		anim.Init(OnEnableAttack);
		isRendered = enemyObj.GetComponentsInChildren<IsRendered>();
		for (int i = 0; i < isRendered.Length; i++)
		{
			isRendered[i].SetCamera(SingletonCustom<DragonBattleCameraMover>.Instance.GetCamera());
		}
		if (DragonBattleDefine.MODE_TYPE_ENEMY[(int)enemyType] != DragonBattleDefine.ModelType.OTHER)
		{
			SkinnedMeshRenderer[] componentsInChildren = enemyObj.GetComponentsInChildren<SkinnedMeshRenderer>();
			colorNo = SingletonCustom<DragonBattleResources>.Instance.MonsterData.GetRandomMatNo(DragonBattleDefine.MODE_TYPE_ENEMY[(int)enemyType]);
			Material mat = SingletonCustom<DragonBattleResources>.Instance.MonsterData.GetMat(DragonBattleDefine.MODE_TYPE_ENEMY[(int)enemyType], colorNo);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				Material[] materials = componentsInChildren[j].materials;
				materials[0] = mat;
				componentsInChildren[j].materials = materials;
			}
		}
		localPosDef = base.transform.localPosition;
		if (IsBoss)
		{
			hp = hpMax;
			base.transform.SetLocalPositionZ(localPosDef.z + 10f);
			switch (enemyType)
			{
			case DragonBattleDefine.EnemyType.DRAGON_BOSS:
				shuriken.Type = DragonBattleShuriken.EffectType.BULLET;
				moveData.interval = 0f;
				moveData.delay = UnityEngine.Random.Range(0.25f, 0.5f);
				break;
			case DragonBattleDefine.EnemyType.FLYING_DEMON:
				if (colorNo == 0)
				{
					shuriken.Type = DragonBattleShuriken.EffectType.DARK;
				}
				else
				{
					shuriken.Type = DragonBattleShuriken.EffectType.DARK_RED;
				}
				attackData.interval = UnityEngine.Random.Range(2f, 4f);
				break;
			}
			enemyObj.SetActive(value: true);
		}
		isInit = true;
	}
	private void SettingEnemyType()
	{
		if (IsBoss)
		{
			if (bossEnemyTypeBuf == DragonBattleDefine.EnemyType.MAX)
			{
				enemyType = DragonBattleDefine.EnemyType.FLYING_DEMON;
			}
			else
			{
				enemyType = DragonBattleDefine.EnemyType.DRAGON_BOSS;
			}
			bossEnemyTypeBuf = enemyType;
			switch (EnemyType)
			{
			case DragonBattleDefine.EnemyType.DRAGON_BOSS:
				attackData = attackDataList[0];
				break;
			case DragonBattleDefine.EnemyType.FLYING_DEMON:
				attackData = attackDataList[1];
				break;
			}
			return;
		}
		enemyType = DragonBattleDefine.EnemyType.BAT;
		int[] array = new int[DragonBattleDefine.SCORE_KILL_ENEMY.Length];
		DragonBattleDefine.SCORE_KILL_ENEMY.CopyTo(array, 0);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 1500 - array[i] * 2;
			num += array[i];
			if (i != 0)
			{
				array[i] += array[i - 1];
			}
		}
		int num2 = UnityEngine.Random.Range(0, num);
		int num3 = 0;
		while (true)
		{
			if (num3 < array.Length)
			{
				if (num3 != 2 && num3 != 4 && num3 != (int)enemyTypeBuf && num2 <= array[num3])
				{
					break;
				}
				num3++;
				continue;
			}
			return;
		}
		enemyType = (DragonBattleDefine.EnemyType)num3;
	}
	public void Active()
	{
		enemyObj.SetActive(value: true);
		if (IsBoss)
		{
			pathMove.PathNo = UnityEngine.Random.Range(0, pathMove.PathNum);
		}
		if (enemyType != DragonBattleDefine.EnemyType.DRAGON_BOSS)
		{
			pathMove.Active();
		}
		isActive = true;
	}
	private void Update()
	{
		if (isInit && isActive)
		{
			isRender = IsRender;
			switch (currentState)
			{
			case State.DEFAULT:
				StateDefault();
				break;
			case State.DEAD:
				StateDead();
				break;
			}
		}
	}
	private void StateDefault()
	{
		if (IsBoss)
		{
			StateDefaultBoss();
		}
		else
		{
			StateDefaultNormal();
		}
	}
	private void StateDefaultNormal()
	{
	}
	private void StateDefaultBoss()
	{
		if (moveData.isMove)
		{
			moveData.delay -= Time.deltaTime;
			if (!(moveData.delay <= 0f) || anim.IsPlayingAttack())
			{
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, moveData.targetPos, moveData.speed * Time.deltaTime);
			base.transform.forward = Vector3.Lerp(base.transform.forward, Vector3.back, moveData.rotSpeed * Time.deltaTime);
			if (CalcManager.Length(base.transform.position, moveData.targetPos) <= 0.25f)
			{
				moveData.isMove = false;
				if (EnemyType == DragonBattleDefine.EnemyType.DRAGON_BOSS)
				{
					moveData.interval = UnityEngine.Random.Range(0.25f, 0.5f);
				}
				attackData.AttackStart(EnemyType);
				attackData.time = UnityEngine.Random.Range(0.25f, 0.75f);
			}
			return;
		}
		if (attackData.isAttacking)
		{
			attackData.time -= Time.deltaTime;
			if (attackData.time <= 0f && !anim.IsPlayingAttack())
			{
				if (!attackData.isAtkAnim)
				{
					attackData.isAtkAnim = true;
					SettingTargetRot();
					UnityEngine.Debug.Log("攻撃モ\u30fcション開始");
					switch (EnemyType)
					{
					case DragonBattleDefine.EnemyType.DRAGON_BOSS:
						anim.SetAttackAnim(DragonBattleEnemyAnim.AttackType.Type0);
						break;
					case DragonBattleDefine.EnemyType.FLYING_DEMON:
						anim.SetAttackAnim(DragonBattleEnemyAnim.AttackType.Type2);
						break;
					}
				}
				attackData.createDelay += Time.deltaTime;
			}
			if (EnemyType == DragonBattleDefine.EnemyType.DRAGON_BOSS)
			{
				if (anim.IsPlayingAttack())
				{
					base.transform.SetLocalEulerAnglesY(Mathf.LerpAngle(base.transform.transform.localEulerAngles.y, attackData.attackRot, moveData.rotSpeed * Time.deltaTime));
				}
				else
				{
					base.transform.SetLocalEulerAnglesY(Mathf.LerpAngle(base.transform.transform.localEulerAngles.y, 180f, moveData.rotSpeed * Time.deltaTime));
				}
			}
		}
		else
		{
			attackData.interval -= Time.deltaTime;
			if (attackData.interval <= 0f)
			{
				attackData.AttackStart(EnemyType);
				moveData.time = 0f;
			}
			if (!pathMove.IsMove)
			{
				if (!anim.IsPlayingAttack())
				{
					moveData.time += Time.deltaTime;
				}
				if (moveData.time >= 0.5f)
				{
					base.transform.forward = Vector3.Lerp(base.transform.forward, Vector3.back, moveData.rotSpeed * Time.deltaTime);
				}
				moveData.interval -= Time.deltaTime;
				if (moveData.interval <= 0f)
				{
					moveData.targetPos = base.transform.parent.position;
					moveData.targetPos.x += UnityEngine.Random.Range(-9.5f, 9.5f);
					moveData.targetPos.z += localPosDef.z + UnityEngine.Random.Range(-1f, 1f);
					moveData.isMove = true;
				}
			}
		}
		if (pathMove.IsMove && pathMove.Progress >= 1f)
		{
			pathMove.Reset(UnityEngine.Random.Range(0, pathMove.PathNum));
		}
	}
	private void SettingTargetRot()
	{
		if (attackData.Pattern.Data.is1st)
		{
			attackData.attackRot = CalcManager.Rot(SingletonCustom<DragonBattlePlayerManager>.Instance.Search1stScorePlayer(_isDeathExclusion: true).transform.position - base.transform.position, CalcManager.AXIS.Y);
		}
		else if (attackData.Pattern.Data.isRandom)
		{
			attackData.attackRot = CalcManager.Rot(SingletonCustom<DragonBattlePlayerManager>.Instance.SearchRandomPlayer(_isDeathExclusion: true).transform.position - base.transform.position, CalcManager.AXIS.Y);
		}
		else
		{
			attackData.attackRot = attackData.Pattern.Data.rotData.RandomRange() + 180f;
		}
	}
	private void OnEnableAttack(int _attackCnt)
	{
		attackData.isAtkAnim = false;
		AttackLongRange(CalcManager.PosRotation2D(Vector3.forward, Vector3.zero, attackData.attackRot, CalcManager.AXIS.Y), attackData.createLocalPosList[0]);
		attackData.Pattern.NextData();
		if (attackData.Pattern.CheckFinish())
		{
			attackData.AttackEnd();
		}
		else
		{
			attackData.time = attackData.Pattern.interval;
			UnityEngine.Debug.Log("攻撃" + attackData.Pattern.AttackNo.ToString() + " : Interval = " + attackData.time.ToString());
			if (attackData.time <= 0f)
			{
				SettingTargetRot();
				OnEnableAttack(_attackCnt);
			}
		}
		attackData.createDelay = 0f;
	}
	private void StateBallAttack()
	{
		int num = 0;
		while (true)
		{
			if (num < tempArray.Length)
			{
				if (tempArray[num].CurrentState != DragonBattlePlayer.State.SCROLL_DEATH && tempArray[num].CurrentState != DragonBattlePlayer.State.WATER_FALL && tempArray[num].CurrentState != DragonBattlePlayer.State.GOAL && Vector3.Distance(base.transform.position, tempArray[num].transform.position) <= 7f)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		SetState(State.DEFAULT);
	}
	private void StateLongRangeAttack()
	{
		waitTime -= Time.deltaTime;
		if (waitTime <= 0f)
		{
			waitTime = UnityEngine.Random.Range(2f, 4f);
			SetState(State.DEFAULT);
		}
	}
	private void StateDead()
	{
		waitTime -= Time.deltaTime;
		if (waitTime <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
	public void OnDamage(DragonBattlePlayer _player, float _power = 1f)
	{
		if (!IsRender || currentState == State.DEAD || !isInit || !isActive)
		{
			return;
		}
		hp = Mathf.Max(hp - 10f * _power, 0f);
		if (IsBoss)
		{
			SingletonCustom<DragonBattleUIManager>.Instance.SetBossHp(hp / hpMax);
		}
		if (hp > 0f)
		{
			if (IsBoss)
			{
				float num = 150f * _power;
				int value = 0;
				for (int num2 = DragonBattleDefine.SCORE_BOSS_HIT_LIST.Length - 1; num2 >= 0; num2--)
				{
					if (num >= (float)DragonBattleDefine.SCORE_BOSS_HIT_LIST[num2])
					{
						value = DragonBattleDefine.SCORE_BOSS_HIT_LIST[num2];
						break;
					}
				}
				_player.AddScore(value);
			}
			if (!attackData.isAtkAnim)
			{
				anim.SetDamageAnim();
			}
			return;
		}
		pathMove.Active(_active: false);
		psSmoke.Play();
		Rigidbody component = GetComponent<Rigidbody>();
		component.constraints = RigidbodyConstraints.None;
		CalcManager.mCalcVector3 = _player.transform.position;
		CalcManager.mCalcVector3.y = base.transform.position.y;
		component.AddForce((base.transform.position - CalcManager.mCalcVector3).normalized * 3.5f, ForceMode.Impulse);
		component.MoveRotation(Quaternion.LookRotation((CalcManager.mCalcVector3 - base.transform.position).normalized));
		component.AddTorque(new Vector3(45f, 0f, 0f));
		SetState(State.DEAD);
		CapsuleCollider[] components = enemyObj.GetComponents<CapsuleCollider>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].enabled = false;
		}
		SingletonCustom<AudioManager>.Instance.SePlay("se_chara_break");
		int num3 = 0;
		num3 = DragonBattleDefine.SCORE_KILL_ENEMY[(int)enemyType];
		_player.AddScore(num3);
		waitTime = 0.45f;
		anim.SetDeadAnim();
		base.gameObject.name = "DestroyWait";
	}
	public void SetState(State _state)
	{
		currentState = _state;
	}
	private void AttackLongRange(Vector3 _dir, Vector3 _createLocalPos)
	{
		DragonBattleShuriken dragonBattleShuriken = UnityEngine.Object.Instantiate(shuriken, base.transform.parent);
		dragonBattleShuriken.transform.position = base.transform.TransformPoint(_createLocalPos);
		dragonBattleShuriken.Play(_dir, shuriken.Type, IsBoss ? 1 : 0);
	}
	public Vector3 GetPredictPos(float _time)
	{
		if (!pathMove || !pathMove.IsMove)
		{
			return base.transform.position;
		}
		return pathMove.GetPredictPos(_time);
	}
	public bool CheckMy(GameObject _col)
	{
		if (base.gameObject == _col)
		{
			return true;
		}
		if (enemyObj == _col)
		{
			return true;
		}
		return false;
	}
	public void BossStandby()
	{
		LeanTween.value(base.transform.localPosition.z, localPosDef.z, 1f).setDelay(0.5f).setOnUpdate(delegate(float _value)
		{
			if (this != null)
			{
				base.transform.SetLocalPositionZ(_value);
			}
		});
	}
}
