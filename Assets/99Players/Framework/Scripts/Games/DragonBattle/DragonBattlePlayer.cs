using GamepadInput;
using System;
using UnityEngine;
public class DragonBattlePlayer : MonoBehaviour
{
	public enum TREE_TOP_CLIM_ANIM_STATE
	{
		ANIM_1,
		ANIM_2,
		ANIM_3
	}
	public enum State
	{
		DEFAULT,
		JUMP,
		SWORD_ATTACK,
		SHURIKEN_ATTACK,
		CLIMBING,
		CLIBING_TOP,
		WATER_FALL,
		SCROLL_DEATH,
		RESPAWN_MOVE,
		DEATH,
		GOAL,
		GOAL_END,
		KNOCK_BACK
	}
	public class KnockBackPower
	{
		public readonly float hitEnemy = 5f;
		public readonly float[] attackPlayer = new float[2]
		{
			10f,
			7.5f
		};
		public readonly float[] attackEnemy = new float[2]
		{
			5f,
			5f
		};
	}
	[Serializable]
	public class cHpData
	{
		public float hp;
		private readonly float hpMax = 100f;
		public float Per => hp / hpMax;
		public cHpData()
		{
			hp = hpMax;
		}
		public void Reduce(float _num)
		{
			hp = Mathf.Max(hp - _num, 0f);
		}
		public void Reset()
		{
			hp = hpMax;
		}
	}
	[SerializeField]
	[Header("スタイル")]
	private CharacterStyle style;
	[SerializeField]
	[Header("リジッドボディ")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("アニメ\u30fcション")]
	private DragonBattleAnim anim;
	[SerializeField]
	[Header("ロ\u30fcカル重力値")]
	private Vector3 localGravity = new Vector3(0f, -9.81f, 0f);
	[SerializeField]
	[Header("スクロ\u30fcル退場エフェクト")]
	private ParticleSystem psDeath;
	[SerializeField]
	[Header("近接攻撃エフェクト")]
	private ParticleSystem[] closeRangeAttackEffect;
	[SerializeField]
	[Header("ニンジャ剣")]
	private DragonBattleSword sword;
	private bool isSwordAttack;
	[SerializeField]
	[Header("体の各部位")]
	private CharacterParts characterParts;
	[SerializeField]
	[Header("AI")]
	private DragonBattlePlayerAI ai;
	[SerializeField]
	[Header("手裏剣")]
	private DragonBattleShuriken shuriken;
	private bool isShurikenAttack;
	private float ShurikenChargeTime = 1f;
	private float shurikenChargeTime;
	private readonly float PowerBase = 1f;
	private readonly float PowerUp = 1f;
	private readonly float LOOK_SPEED = 11f;
	public readonly float MOVE_SPEED_MAX = 12f;
	private readonly float MOVE_SPEED_SCALE = 20f;
	private readonly float ATTENUATION_SCALE = 0.925f;
	private readonly float SE_RUN_TIME = 0.55f;
	private int playerIdx = -1;
	private DragonBattleDefine.UserType userType;
	private JoyConManager.AXIS_INPUT axisInput;
	private Vector3 moveForce;
	private float moveSpeed;
	private bool isJump;
	private bool isJumpInput;
	private float jumpInputTime;
	private int score;
	private float runAnimationSpeed = 20f;
	private float runAnimationTime;
	private Vector3 prevPos;
	private Vector3 nowPos;
	private int playSeRunCnt;
	private int goalNo = -1;
	private State currentState;
	private float waitTime;
	public readonly float ATTACK_WAIT_SWORD = 0.25f;
	public readonly float ATTACK_WAIT_SHURIKEN = 0.75f;
	private readonly float RESPAWN_TIME = 1.5f;
	private float waitRespawnTime;
	private Quaternion tempRot;
	private Vector3 prevDir;
	private Vector3 rotVec;
	private RaycastHit rayHit;
	private Vector3 attackDir;
	private Vector3 knockBackDir;
	private float knockBackTime;
	private float knockBackInterval;
	private int climbingCount;
	private Transform[] climbTopAnchor;
	private int layerMask = 1048576;
	[SerializeField]
	[Header("コライダ\u30fcオブジェクト")]
	private GameObject[] colObj;
	[SerializeField]
	[Header("ドラゴンオブジェクト")]
	private SkinnedMeshRenderer dragonRend;
	[SerializeField]
	[Header("ドラゴンオブジェクト")]
	private GameObject dragonObj;
	[SerializeField]
	[Header("カメラ表示確認")]
	private IsRendered[] isRendered;
	private ParticleSystem[] chargeEffect;
	private float renderTime;
	private KnockBackPower knockBackPower = new KnockBackPower();
	private cHpData hpData = new cHpData();
	[SerializeField]
	private Transform hpAnchor;
	public State CurrentState => currentState;
	public CharacterStyle Style => style;
	public float MoveSpeed => moveSpeed;
	public bool IsJump => currentState == State.JUMP;
	public Rigidbody Rigid => rigid;
	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			score = value;
		}
	}
	public bool IsCpu
	{
		get;
		set;
	}
	public int PlayerIdx => playerIdx;
	public int GoalNo => goalNo;
	public DragonBattlePlayerAI Ai => ai;
	public bool IsShurikenAttack => isShurikenAttack;
	public GameObject[] ColObj => colObj;
	public Vector3 Forward => dragonObj.transform.forward;
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
	public KnockBackPower gsKnockBackPower => knockBackPower;
	public cHpData HpData => hpData;
	public Transform HpAnchor => hpAnchor;
	public void Init(int _playerIdx)
	{
		playerIdx = _playerIdx;
		IsCpu = (playerIdx >= SingletonCustom<GameSettingManager>.Instance.PlayerNum);
		userType = (DragonBattleDefine.UserType)(IsCpu ? (4 + (playerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : playerIdx);
		dragonRend.material = SingletonCustom<DragonBattlePlayerManager>.Instance.DragonMats[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
		moveSpeed = 0f;
		prevDir = moveForce;
		style.SetGameStyle(GS_Define.GameType.ATTACK_BALL, (int)userType);
		ParticleSystem.MainModule main = psDeath.main;
		ParticleSystem.MinMaxGradient startColor = main.startColor;
		startColor.colorMax = SingletonCustom<DragonBattleResources>.Instance.ColorList.player[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]];
		main.startColor = startColor;
		main = psDeath.main;
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate(SingletonCustom<DragonBattleResources>.Instance.EffectList.playerCharge[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]], base.transform.position, Quaternion.identity, base.transform);
		particleSystem.gameObject.SetActive(value: true);
		chargeEffect = particleSystem.GetComponentsInChildren<ParticleSystem>();
		anim.SetAnim(DragonBattleAnim.AnimType.STANDBY);
		playSeRunCnt = 0;
		nowPos = (prevPos = base.transform.position);
		ai.Init();
		sword.Init();
		for (int i = 0; i < isRendered.Length; i++)
		{
			isRendered[i].SetCamera(SingletonCustom<DragonBattleCameraMover>.Instance.GetCamera());
		}
	}
	public void UpdateMethod()
	{
		if (!base.gameObject.activeSelf || currentState == State.GOAL_END)
		{
			return;
		}
		knockBackInterval -= Time.deltaTime;
		prevPos = nowPos;
		nowPos = base.transform.position;
		if (rigid.velocity.sqrMagnitude >= 300f)
		{
			rigid.velocity *= 0f;
		}
		if (currentState != State.GOAL && rigid.useGravity && base.transform.localPosition.y <= 0f)
		{
			rigid.useGravity = false;
			rigid.constraints = (RigidbodyConstraints)116;
			base.transform.SetLocalPositionY(0f);
		}
		switch (currentState)
		{
		case State.DEFAULT:
			StateDefault(IsCpu);
			break;
		case State.JUMP:
			StateJump(IsCpu);
			break;
		case State.SWORD_ATTACK:
			StateAttackSword(IsCpu);
			break;
		case State.SHURIKEN_ATTACK:
			StateAttackShuriken(IsCpu);
			break;
		case State.CLIMBING:
			StateClimbing(IsCpu);
			break;
		case State.GOAL:
			rigid.AddForce(localGravity, ForceMode.Acceleration);
			if (base.transform.position.z >= SingletonCustom<DragonBattleFieldManager>.Instance.GetGoal().ActiveBorderAnchor.position.z)
			{
				rigid.velocity = new Vector3(rigid.velocity.x * 0.975f, rigid.velocity.y, rigid.velocity.z * 0.975f);
				moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
				tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * Vector3.back), Time.deltaTime * LOOK_SPEED);
				if (tempRot != Quaternion.identity)
				{
					base.transform.rotation = tempRot;
					rigid.MoveRotation(tempRot);
				}
			}
			else
			{
				UpdateRot(Time.deltaTime);
			}
			break;
		case State.WATER_FALL:
		case State.SCROLL_DEATH:
			waitRespawnTime -= Time.deltaTime;
			if (waitRespawnTime <= 0f)
			{
				Respawn();
			}
			break;
		case State.RESPAWN_MOVE:
			StateRespawnMove();
			break;
		case State.KNOCK_BACK:
			knockBackTime -= Time.deltaTime;
			if (knockBackTime <= 0f)
			{
				if (isJump && !IsCpu)
				{
					anim.SetAnim(DragonBattleAnim.AnimType.JUMP);
					SetState(State.JUMP);
				}
				else
				{
					anim.SetAnim(DragonBattleAnim.AnimType.STANDBY);
					SetState(State.DEFAULT);
				}
			}
			break;
		}
		State state = currentState;
		if ((uint)(state - 6) > 1u && state != State.GOAL)
		{
			CheckScrollDeath();
		}
		UpdateChargeEffect();
	}
	private void UpdateChargeEffect()
	{
		for (int i = 0; i < chargeEffect.Length; i++)
		{
			ParticleSystem.MainModule main = chargeEffect[i].main;
			if (shurikenChargeTime > 0f)
			{
				main.loop = true;
				if (!chargeEffect[i].isPlaying)
				{
					chargeEffect[i].Play();
				}
				main.simulationSpeed = 1f + Mathf.Min(shurikenChargeTime / ShurikenChargeTime, 1f) * 1.75f;
			}
			else
			{
				if (main.loop)
				{
					main.simulationSpeed = 1f;
				}
				main.loop = false;
			}
		}
	}
	public void FixedUpdate()
	{
		SetLocalGravity();
		switch (currentState)
		{
		case State.DEFAULT:
		case State.JUMP:
		case State.SWORD_ATTACK:
		case State.SHURIKEN_ATTACK:
		case State.RESPAWN_MOVE:
		case State.GOAL:
			rigid.velocity = new Vector3((moveForce * moveSpeed).x, rigid.velocity.y, (moveForce * moveSpeed).z);
			break;
		case State.WATER_FALL:
			rigid.velocity *= 0.77f;
			break;
		}
		rigid.velocity += knockBackDir;
		knockBackDir *= 0.9f;
		State state = currentState;
		if ((uint)state <= 3u)
		{
			UpdateRot(Time.fixedDeltaTime);
		}
	}
	private void StateDefault(bool _isCpu)
	{
		if (_isCpu)
		{
			ai.UpdateAction();
			moveForce = ai.UpdateForce();
			if (moveForce.magnitude < 0.0400000028f)
			{
				moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
				if (anim.CurrentAnimType == DragonBattleAnim.AnimType.DASH)
				{
					anim.SetAnimSpeed(moveSpeed * 0.5f);
					if (moveSpeed <= 0.5f)
					{
						anim.SetAnim(DragonBattleAnim.AnimType.STANDBY);
					}
				}
			}
			else if (moveSpeed < MOVE_SPEED_MAX)
			{
				moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_SCALE, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
			}
			if (ai.IsSwordInput())
			{
				rotVec.x = moveForce.x;
				rotVec.z = moveForce.z;
				tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * rotVec), 1f);
				base.transform.rotation = tempRot;
				rigid.MoveRotation(tempRot);
				AttackSword();
			}
			else if (ai.IsShurikenInput())
			{
				rotVec.x = moveForce.x;
				rotVec.z = moveForce.z;
				tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * rotVec), 1f);
				base.transform.rotation = tempRot;
				rigid.MoveRotation(tempRot);
				AttackShuriken();
			}
			else if (ai.IsJumpInput())
			{
				Jump();
			}
			return;
		}
		UpdateMoveForce();
		if (CalcManager.Length(nowPos, prevPos) > 0.01f)
		{
			runAnimationTime += CalcManager.Length(nowPos, prevPos) * runAnimationSpeed * Time.deltaTime;
			if (runAnimationTime >= (float)playSeRunCnt * 0.5f)
			{
				playSeRunCnt++;
				PlaySeRun();
			}
			if (runAnimationTime >= 1f)
			{
				runAnimationTime = 0f;
				playSeRunCnt = 1;
			}
		}
		if (SingletonCustom<DragonBattlePlayerManager>.Instance.IsProhibitAttack)
		{
			shurikenChargeTime = 0f;
		}
		else if (DragonBattleControllerManager.GetButtonDown(playerIdx, SatGamePad.Button.A))
		{
			AttackSword();
		}
		else if (DragonBattleControllerManager.GetButtonDown(playerIdx, SatGamePad.Button.Y))
		{
			shurikenChargeTime = 0f;
		}
		else if (DragonBattleControllerManager.GetButton(playerIdx, SatGamePad.Button.Y))
		{
			shurikenChargeTime += Time.deltaTime;
		}
		else if (DragonBattleControllerManager.GetButtonUp(playerIdx, SatGamePad.Button.Y))
		{
			AttackShuriken();
		}
		else
		{
			shurikenChargeTime = 0f;
		}
	}
	private void StateJump(bool _isCpu)
	{
		if (_isCpu)
		{
			ai.UpdateAction();
			moveForce = ai.UpdateForce();
			if (isJumpInput)
			{
				jumpInputTime += Time.deltaTime;
				if (jumpInputTime < 0.25f)
				{
					rigid.AddForce(base.transform.up * 0.022f, ForceMode.Impulse);
				}
			}
			if (ai.IsSwordInput())
			{
				rotVec.x = moveForce.x;
				rotVec.z = moveForce.z;
				tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * rotVec), 1f);
				base.transform.rotation = tempRot;
				rigid.MoveRotation(tempRot);
				AttackSword();
			}
			return;
		}
		UpdateMoveForce();
		if (SingletonCustom<JoyConManager>.Instance.GetButtonUp(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.B))
		{
			isJumpInput = false;
		}
		else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
		{
			AttackSword();
			isJump = true;
		}
		else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.Y))
		{
			AttackShuriken();
			isJump = true;
		}
		if (isJumpInput)
		{
			jumpInputTime += Time.deltaTime;
			if (jumpInputTime < 0.25f)
			{
				rigid.AddForce(base.transform.up * 0.022f, ForceMode.Impulse);
			}
		}
	}
	private void StateAttackSword(bool _isCpu)
	{
		waitTime -= Time.deltaTime;
		if (isSwordAttack)
		{
			if (waitTime <= ATTACK_WAIT_SWORD - 0.2f)
			{
				sword.AttackEnd();
			}
		}
		else if (anim.CheckPlayingAnim(DragonBattleAnim.AnimType.SWORD_ATTACK))
		{
			for (int i = 0; i < closeRangeAttackEffect.Length; i++)
			{
				closeRangeAttackEffect[i].Play();
			}
			sword.AttackStart();
			isSwordAttack = true;
		}
		if (IsCpu)
		{
			ai.UpdateAction();
			moveForce = ai.UpdateForce();
		}
		else
		{
			UpdateMoveForce();
		}
		if (waitTime <= 0f && anim.CheckPlayingAnim(DragonBattleAnim.AnimType.STANDBY))
		{
			if (isJump && !_isCpu)
			{
				anim.SetAnim(DragonBattleAnim.AnimType.JUMP);
				SetState(State.JUMP);
			}
			else
			{
				anim.SetAnim(DragonBattleAnim.AnimType.STANDBY);
				SetState(State.DEFAULT);
			}
		}
	}
	private void StateAttackShuriken(bool _isCpu)
	{
		waitTime -= Time.deltaTime;
		CreateShuriken(waitTime);
		if (IsCpu)
		{
			ai.UpdateAction();
			moveForce = ai.UpdateForce();
		}
		else
		{
			UpdateMoveForce();
		}
		if (waitTime <= 0f && anim.CheckPlayingAnim(DragonBattleAnim.AnimType.STANDBY))
		{
			if (isJump && !_isCpu)
			{
				anim.SetAnim(DragonBattleAnim.AnimType.JUMP);
				SetState(State.JUMP);
			}
			else
			{
				anim.SetAnim(DragonBattleAnim.AnimType.STANDBY);
				SetState(State.DEFAULT);
			}
		}
	}
	private void StateClimbing(bool _isCpu)
	{
		if (_isCpu)
		{
			if (ai.UpdateClimbing())
			{
				Climbing();
			}
		}
		else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
		{
			Climbing();
		}
	}
	private void StateRespawnMove()
	{
		if (base.transform.position.z >= SingletonCustom<DragonBattleCameraMover>.Instance.RespawnAnchor.position.z)
		{
			rigid.useGravity = true;
			moveSpeed = MOVE_SPEED_MAX;
			anim.SetAnim(DragonBattleAnim.AnimType.STANDBY);
			SetState(State.DEFAULT);
		}
	}
	private void Jump()
	{
	}
	private void AttackSword()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_sword_fight_swing");
		anim.SetAnimSpeed(4.5f);
		anim.SetAnim(DragonBattleAnim.AnimType.SWORD_ATTACK);
		attackDir = moveForce;
		isSwordAttack = false;
		waitTime = ATTACK_WAIT_SWORD;
		if (IsCpu)
		{
			ai.ActionWait = 0.2f;
		}
		SetState(State.SWORD_ATTACK);
	}
	private void AttackShuriken()
	{
		anim.SetAnimSpeed(3f);
		anim.SetAnim(DragonBattleAnim.AnimType.SHURIKEN_ATTACK);
		attackDir = moveForce;
		waitTime = ATTACK_WAIT_SHURIKEN;
		isShurikenAttack = false;
		if (IsCpu)
		{
			ai.ActionWait = 0.2f;
		}
		SetState(State.SHURIKEN_ATTACK);
	}
	private void CreateShuriken(float _time)
	{
		if (!isShurikenAttack && _time <= ATTACK_WAIT_SHURIKEN - 0.15f)
		{
			DragonBattleShuriken dragonBattleShuriken = UnityEngine.Object.Instantiate(shuriken, base.transform.parent);
			float num = Mathf.Min(shurikenChargeTime / ShurikenChargeTime, 1f) * PowerUp;
			dragonBattleShuriken.transform.position = shuriken.transform.position;
			dragonBattleShuriken.Play(Forward, DragonBattleShuriken.EffectType.PLAYER, SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType], PowerBase + num);
			dragonBattleShuriken.gameObject.SetActive(value: true);
			isShurikenAttack = true;
			shurikenChargeTime = 0f;
		}
	}
	public void SetShurikenChargeTimePer(float _per)
	{
		shurikenChargeTime = ShurikenChargeTime * _per;
	}
	public void AddShurikenChargeTimePer(float _time)
	{
		shurikenChargeTime += _time;
	}
	private void Climbing()
	{
		rigid.isKinematic = false;
		climbingCount++;
		ClimbingAnimation(0.1f, climbingCount % 2 == 1);
		rigid.AddForce(Vector3.up * 250f, ForceMode.Acceleration);
		if (!IsCpu)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_blow");
		}
		LeanTween.cancel(base.gameObject);
		LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
		{
			rigid.velocity = Vector3.zero;
		});
		if (base.transform.position.y >= climbTopAnchor[0].position.y)
		{
			SetState(State.CLIBING_TOP);
			RecursiveTreeTopClimbingAnimation(TREE_TOP_CLIM_ANIM_STATE.ANIM_1);
		}
	}
	public void OnGoal()
	{
		if (goalNo == -1)
		{
			goalNo = SingletonCustom<DragonBattlePlayerManager>.Instance.OnGoal(this);
			switch (goalNo)
			{
			case 0:
				AddScore(500);
				break;
			case 1:
				AddScore(300);
				break;
			case 2:
				AddScore(100);
				break;
			case 3:
				AddScore(50);
				break;
			}
			moveForce = Vector3.forward;
			moveSpeed = MOVE_SPEED_MAX;
			sword.gameObject.SetActive(value: false);
			anim.SetAnim(DragonBattleAnim.AnimType.JOY);
			Rigid.useGravity = true;
			rigid.constraints = RigidbodyConstraints.FreezeRotation;
			SetState(State.GOAL);
		}
	}
	private void Respawn()
	{
		if (!SingletonCustom<DragonBattleFieldManager>.Instance.IsRespawnExec)
		{
			Vector3 position = SingletonCustom<DragonBattleCameraMover>.Instance.DeathAnchor.position;
			position.x += UnityEngine.Random.Range(-8f, 8f);
			position.y = base.transform.parent.position.y + 2f;
			base.transform.position = position;
			SingletonCustom<AudioManager>.Instance.SePlay("se_goeraser_respawn");
			rigid.velocity = Vector3.zero;
			rigid.angularVelocity = Vector3.zero;
			rigid.useGravity = false;
			rigid.constraints = RigidbodyConstraints.FreezeRotation;
			base.transform.SetLocalEulerAnglesY(270f);
			dragonObj.transform.SetLocalEulerAnglesZ(0f);
			moveForce = Vector3.forward;
			moveSpeed = MOVE_SPEED_MAX * 2f;
			style.SetMainCharacterFaceDiff(IsCpu ? (4 + (playerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : playerIdx, StyleTextureManager.MainCharacterFaceType.NORMAL);
			anim.SetAnim(DragonBattleAnim.AnimType.RESPAWN);
			rigid.isKinematic = false;
			if (IsCpu)
			{
				ai.SetRespawnWait();
			}
			else
			{
				SingletonCustom<DragonBattleUIManager>.Instance.ShowPlayerNo(playerIdx);
			}
			sword.gameObject.SetActive(value: false);
			for (int i = 0; i < colObj.Length; i++)
			{
				colObj[i].SetActive(value: true);
			}
			hpData.Reset();
			SetState(State.RESPAWN_MOVE);
		}
	}
	private void CheckScrollDeath()
	{
		if (CheckState(State.SCROLL_DEATH) || CheckState(State.RESPAWN_MOVE))
		{
			return;
		}
		if (IsRender)
		{
			renderTime = Mathf.Max(0f, renderTime + Time.deltaTime);
		}
		else
		{
			renderTime = Mathf.Min(0f, renderTime - Time.deltaTime);
		}
		if (base.transform.position.z <= SingletonCustom<DragonBattleCameraMover>.Instance.DeathAnchor.position.z || renderTime <= -0.5f)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goeraser_syo_metu");
			base.transform.SetLocalEulerAnglesY(270f);
			psDeath.transform.forward = SingletonCustom<DragonBattleCameraMover>.Instance.ViewCenterAnchor[0].position - base.transform.position;
			psDeath.Play();
			rigid.isKinematic = true;
			if (currentState != State.DEATH)
			{
				Death();
				SetVibration();
			}
			SetState(State.SCROLL_DEATH);
		}
	}
	private void Death()
	{
		SetVibration();
		score = (int)((float)score * 0.5f);
		for (int i = 0; i < colObj.Length; i++)
		{
			colObj[i].SetActive(value: false);
		}
		shurikenChargeTime = 0f;
	}
	private void RecursiveTreeTopClimbingAnimation(TREE_TOP_CLIM_ANIM_STATE _goalAnimState)
	{
		TreeTopClimbingAnimation(0.25f, _goalAnimState);
		LeanTween.delayedCall(base.gameObject, 0.25f, (Action)delegate
		{
			_goalAnimState++;
			if ((int)_goalAnimState < Enum.GetValues(typeof(TREE_TOP_CLIM_ANIM_STATE)).Length)
			{
				RecursiveTreeTopClimbingAnimation(_goalAnimState);
			}
			else
			{
				SetState(State.DEFAULT);
				anim.SetAnim(DragonBattleAnim.AnimType.STANDBY);
				rigid.velocity = Vector3.zero;
				rigid.angularVelocity = Vector3.zero;
				rigid.isKinematic = false;
			}
		});
	}
	public void SetVibration()
	{
		if (!IsCpu)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerIdx);
		}
	}
	public void KnockBack(Vector3 _dir, float _knockBackPow, bool _isHitEnemy = false, bool _isBoss = false, float _power = 1f)
	{
		if (knockBackInterval > 0f || hpData.Per <= 0f)
		{
			return;
		}
		switch (currentState)
		{
		case State.DEATH:
		case State.GOAL:
			return;
		case State.CLIMBING:
		case State.CLIBING_TOP:
			_knockBackPow *= 0.1f;
			break;
		}
		if (currentState == State.GOAL)
		{
			return;
		}
		SetVibration();
		_dir.y = 0f;
		knockBackDir = _dir * _knockBackPow;
		if (_isHitEnemy)
		{
			hpData.Reduce(10f * _power);
		}
		else
		{
			hpData.Reduce(15f * _power);
		}
		SingletonCustom<AudioManager>.Instance.SePlay("se_collision_character_1");
		if (hpData.Per <= 0f)
		{
			rigid.constraints = RigidbodyConstraints.FreezeRotation;
			Vector3 vector = knockBackDir;
			knockBackDir = Vector3.zero.normalized * 0.1f;
			knockBackDir.y = -2.5f;
			Death();
			anim.SetAnimSpeed(1f);
			anim.SetAnim(DragonBattleAnim.AnimType.DEATH);
			SetState(State.DEATH);
			return;
		}
		if (currentState == State.KNOCK_BACK)
		{
			if (_isBoss)
			{
				knockBackTime = 0.25f;
			}
			else
			{
				knockBackTime = Mathf.Max(knockBackTime, 0.2f);
			}
		}
		else
		{
			if (_isHitEnemy)
			{
				knockBackTime = 0.2f;
			}
			else if (_isBoss)
			{
				knockBackTime = 0.5f;
			}
			else
			{
				knockBackTime = 0.5f;
			}
			anim.SetAnimSpeed(1f);
			anim.SetAnim(DragonBattleAnim.AnimType.DAMAGE);
		}
		knockBackInterval = 0.2f;
		SetState(State.KNOCK_BACK);
	}
	public void AnimPorRotReset()
	{
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD).transform.localPosition = new Vector3(0f, 0.1735753f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).transform.localPosition = new Vector3(0f, 0f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP).transform.localPosition = new Vector3(0f, 0.05483828f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).transform.localPosition = new Vector3(-0.1511168f, 0.1297733f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).transform.localPosition = new Vector3(0.1511168f, 0.1297733f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L).transform.localPosition = new Vector3(0.006473546f, -0.02895849f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R).transform.localPosition = new Vector3(-0.006473546f, -0.02895849f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L).transform.localPosition = new Vector3(-0.054f, -0.0483f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R).transform.localPosition = new Vector3(0.054f, -0.0483f, 0f);
		characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
	}
	public void FallClimbing()
	{
		AnimPorRotReset();
		anim.SetAnim(DragonBattleAnim.AnimType.JUMP);
		SetState(State.JUMP);
		isJumpInput = false;
		jumpInputTime = 0f;
		rigid.isKinematic = false;
		isJump = true;
	}
	private void UpdateMoveForce()
	{
		CalcManager.mCalcVector2 = DragonBattleControllerManager.GetStickDir(playerIdx);
		if (CalcManager.mCalcVector2.magnitude < 0.0400000028f)
		{
			moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
			if (anim.CurrentAnimType == DragonBattleAnim.AnimType.DASH)
			{
				anim.SetAnimSpeed(moveSpeed * 0.5f);
				if (moveSpeed <= 0.5f)
				{
					anim.SetAnim(DragonBattleAnim.AnimType.STANDBY);
				}
			}
		}
		else
		{
			DragonBattleAnim.AnimType currentAnimType = anim.CurrentAnimType;
			if (currentAnimType != DragonBattleAnim.AnimType.STANDBY && currentAnimType == DragonBattleAnim.AnimType.DASH)
			{
				anim.SetAnimSpeed(moveSpeed * 0.5f);
			}
			if (moveSpeed < MOVE_SPEED_MAX)
			{
				moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_SCALE, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
			}
			moveForce.x = CalcManager.mCalcVector2.x;
			moveForce.z = CalcManager.mCalcVector2.y;
			moveForce.y *= 0.95f;
			moveForce = moveForce.normalized;
		}
		if (Mathf.Abs(Vector3.Angle(prevDir, moveForce)) >= 90f)
		{
			moveSpeed *= 0.1f;
		}
		prevDir = moveForce;
	}
	private void UpdateRot(float _deltaTime)
	{
		if (!(moveForce.magnitude >= 0.01f))
		{
			return;
		}
		rotVec.x = moveForce.x;
		rotVec.z = moveForce.z;
		tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * rotVec), Time.deltaTime * LOOK_SPEED);
		if (tempRot != Quaternion.identity)
		{
			float num = tempRot.eulerAngles.y - base.transform.rotation.eulerAngles.y;
			float b = 0f;
			float num2 = 10f;
			if (Mathf.Abs(num) > 5f)
			{
				b = ((!(num > 0f)) ? 45f : (-45f));
			}
			dragonObj.transform.SetLocalEulerAnglesZ(Mathf.LerpAngle(dragonObj.transform.localRotation.eulerAngles.z, b, num2 * _deltaTime));
			base.transform.rotation = tempRot;
			rigid.MoveRotation(tempRot);
		}
	}
	public void AddScore(int _value, bool _isShowValue = true, bool _isVibration = true)
	{
		Score = Mathf.Min(Score + _value, 9999);
		if (_isShowValue)
		{
			SingletonCustom<DragonBattleUIManager>.Instance.ShowScore(PlayerIdx, _value);
		}
		if (_isVibration)
		{
			SetVibration();
		}
	}
	private void PlaySeRun()
	{
		if (!IsCpu)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.2f);
		}
	}
	public void OnJumpAiCall()
	{
		moveSpeed = MOVE_SPEED_MAX;
	}
	public void SetState(State _state)
	{
		if (currentState != _state)
		{
			if (currentState == State.SWORD_ATTACK)
			{
				sword.AttackEnd();
			}
			currentState = _state;
			switch (currentState)
			{
			case State.DEFAULT:
				anim.SetAnimSpeed(1f);
				break;
			case State.WATER_FALL:
				score = (int)((float)score * 0.5f);
				waitRespawnTime = RESPAWN_TIME;
				break;
			case State.SCROLL_DEATH:
				waitRespawnTime = RESPAWN_TIME;
				break;
			case State.GOAL:
				anim.SetAnimSpeed(1f);
				break;
			case State.DEATH:
				waitRespawnTime = RESPAWN_TIME * 3f;
				break;
			}
			if (currentState != 0 && currentState != State.SHURIKEN_ATTACK)
			{
				shurikenChargeTime = 0f;
			}
		}
	}
	public bool CheckState(State _state)
	{
		return currentState == _state;
	}
	private void SetLocalGravity()
	{
		if (CheckState(State.DEATH) || !(base.transform.localPosition.y <= 0f))
		{
			State currentState2 = currentState;
			rigid.AddForce(localGravity, ForceMode.Acceleration);
		}
	}
	public bool IsDeath()
	{
		if (!CheckState(State.DEATH))
		{
			return CheckState(State.SCROLL_DEATH);
		}
		return true;
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Ninja" && knockBackInterval <= 0f && !CheckState(State.SWORD_ATTACK) && SingletonCustom<DragonBattleFieldManager>.Instance.CheckEnemy(collision.collider.gameObject).gameObject.name != "DestroyWait")
		{
			KnockBack(base.transform.position - collision.contacts[0].point, gsKnockBackPower.hitEnemy, _isHitEnemy: true);
		}
		if (collision.gameObject.tag == "Failure")
		{
			rigid.isKinematic = true;
			Death();
			SetState(State.SCROLL_DEATH);
		}
		else if (CurrentState == State.GOAL && collision.gameObject.tag == "Goal")
		{
			anim.SetAnimSpeed(1f);
			anim.SetAnim(DragonBattleAnim.AnimType.STAND);
			Rigid.isKinematic = true;
		}
	}
	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.tag == "VerticalWall")
		{
			Vector3 velocity = Rigid.velocity;
			velocity.x = 0f;
			Rigid.velocity = velocity;
		}
		if (collision.gameObject.tag == "HorizontalWall")
		{
			Vector3 velocity2 = Rigid.velocity;
			velocity2.z = 0f;
			Rigid.velocity = velocity2;
		}
		switch (currentState)
		{
		case State.JUMP:
		{
			if (collision.gameObject.layer != DragonBattleDefine.ConvertLayerNo("Field") || !(Mathf.Abs(rigid.velocity.y) <= 0.5f))
			{
				break;
			}
			ContactPoint[] contacts = collision.contacts;
			int num = 0;
			while (true)
			{
				if (num < contacts.Length)
				{
					ContactPoint contactPoint = contacts[num];
					if (contactPoint.point.y < base.transform.position.y)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			anim.SetAnim(DragonBattleAnim.AnimType.STANDBY);
			SetState(State.DEFAULT);
			break;
		}
		case State.GOAL:
			if (collision.gameObject.tag == "Player" && !anim.CheckPlayingAnim(DragonBattleAnim.AnimType.STAND) && moveSpeed < MOVE_SPEED_MAX * 0.2f)
			{
				moveForce = (base.transform.position - collision.gameObject.transform.position).normalized;
				moveSpeed = MOVE_SPEED_MAX * 0.2f;
			}
			break;
		}
	}
	private void Animation(Transform _parts, Vector3 _pos, Vector3 _euler, float _animTime)
	{
		LeanTween.cancel(_parts.gameObject);
		LeanTween.moveLocal(_parts.gameObject, _pos, _animTime);
		LeanTween.rotateLocal(_parts.gameObject, _euler, _animTime);
	}
	private void AnimationRotateAround(Transform _parts, Vector3 _pos, Vector3 _dir, float _angle, float _animTime)
	{
		LeanTween.cancel(_parts.gameObject);
		LeanTween.moveLocal(_parts.gameObject, _pos, _animTime);
		LeanTween.rotateAround(_parts.gameObject, _dir, _angle, _animTime);
	}
	public void ReadyClimbingAnimation(float _animTime)
	{
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(340f, 0f, 0f), _animTime);
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.055f, 0f), new Vector3(355f, 0f, 0f), _animTime);
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.13f, 0.116f, 0.097f), new Vector3(45f, 300f, 145f), _animTime);
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.13f, 0.173f, 0.103f), new Vector3(315f, 240f, 145f), _animTime);
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
	}
	public void ClimbingAnimation(float _animTime, bool isOdd)
	{
		if (!isOdd)
		{
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(340f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.13f, 0.116f, 0.097f), new Vector3(45f, 300f, 145f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.13f, 0.173f, 0.103f), new Vector3(315f, 240f, 145f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
		}
		else
		{
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.17f, 0f), new Vector3(350f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.13f, 0.173f, 0.103f), new Vector3(45f, 300f, 145f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.13f, 0.116f, 0.097f), new Vector3(315f, 240f, 145f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.055f, -0.055f, 0.08f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.08f, -0.015f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
		}
	}
	public void ReadySideMoveAnimation(float _animTime)
	{
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.12f, 0.125f, 0.097f), new Vector3(45f, 300f, 145f), _animTime);
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.12f, 0.125f, 0.097f), new Vector3(315f, 240f, 145f), _animTime);
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.095f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
		Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.095f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
	}
	public void SideMoveAnimation(float _animTime, bool isLeftMove)
	{
		if (isLeftMove)
		{
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.15f, 0.125f, 0.097f), new Vector3(45f, 300f, 145f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.12f, 0.155f, 0.097f), new Vector3(315f, 240f, 145f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.105f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.065f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
		}
		else
		{
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.12f, 0.155f, 0.097f), new Vector3(45f, 300f, 145f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.15f, 0.125f, 0.097f), new Vector3(315f, 240f, 145f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.065f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.105f, -0.04f, 0.09f), new Vector3(0f, 0f, 0f), _animTime);
		}
	}
	private void TreeTopClimbingAnimation(float _animTime, TREE_TOP_CLIM_ANIM_STATE _climbAnimState)
	{
		switch (_climbAnimState)
		{
		case TREE_TOP_CLIM_ANIM_STATE.ANIM_1:
			CalcManager.mCalcVector3.x = base.transform.localPosition.x;
			CalcManager.mCalcVector3.y = base.transform.parent.InverseTransformPoint(climbTopAnchor[0].position).y;
			CalcManager.mCalcVector3.z = base.transform.parent.InverseTransformPoint(climbTopAnchor[0].position).z;
			Animation(base.transform, CalcManager.mCalcVector3, new Vector3(0f, -90f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(349.2066f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0.002f, -0.013f), new Vector3(359.3358f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05356598f, 0.01595052f), new Vector3(1.530573f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.151f, 0.127f, 0.04f), new Vector3(348.2313f, 63.27095f, 267.2055f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.151f, 0.123f, 0.052f), new Vector3(356.7374f, 301.0549f, 116.8664f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(316.0092f, 3.994534f, 6.859902f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(319.6358f, 30.74282f, 316.3231f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.068f, -0.04833f, 0f), new Vector3(348.5809f, 0.3663634f, 0.5334841f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			break;
		case TREE_TOP_CLIM_ANIM_STATE.ANIM_2:
			CalcManager.mCalcVector3.x = base.transform.localPosition.x;
			CalcManager.mCalcVector3.y = base.transform.parent.InverseTransformPoint(climbTopAnchor[1].position).y;
			CalcManager.mCalcVector3.z = base.transform.parent.InverseTransformPoint(climbTopAnchor[1].position).z;
			Animation(base.transform, CalcManager.mCalcVector3, new Vector3(0f, -90f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.175f, 0f), new Vector3(340.5f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0.005f, -0.015f), new Vector3(43.77412f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.04087448f, 0.01260412f), new Vector3(1.943285f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.151f, 0.122f, 0.034f), new Vector3(336f, 73f, 235f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.151f, 0.127f, 0.04f), new Vector3(350f, 295f, 95f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(315f, 5f, 6.86f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(0f, -0.029f, 0f), new Vector3(315f, 30f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.112f, -0.021f, 0.054f), new Vector3(8.567815f, 318.7467f, 291.7385f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.047f, 0.015f), new Vector3(332.2637f, 0f, 0f), _animTime);
			break;
		case TREE_TOP_CLIM_ANIM_STATE.ANIM_3:
			CalcManager.mCalcVector3.x = base.transform.localPosition.x;
			CalcManager.mCalcVector3.y = base.transform.parent.InverseTransformPoint(climbTopAnchor[2].position).y;
			CalcManager.mCalcVector3.z = base.transform.parent.InverseTransformPoint(climbTopAnchor[2].position).z;
			Animation(base.transform, CalcManager.mCalcVector3, new Vector3(0f, -90f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HEAD), new Vector3(0f, 0.1735753f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.BODY), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.HIP), new Vector3(0f, 0.05483828f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L), new Vector3(-0.1511168f, 0.1297733f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R), new Vector3(0.1511168f, 0.1297733f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_L), new Vector3(0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.ARM_R), new Vector3(-0.006473546f, -0.02895849f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_L), new Vector3(-0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			Animation(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.LEG_R), new Vector3(0.054f, -0.0483f, 0f), new Vector3(0f, 0f, 0f), _animTime);
			break;
		}
	}
	private void AfterRankGoalAnimation(int _rank)
	{
		LeanTween.rotateY(base.gameObject, base.transform.eulerAngles.y + 15f, 1f);
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			LeanTween.rotateY(base.gameObject, base.transform.eulerAngles.y - 30f, 2f).setLoopPingPong();
		});
		switch (_rank)
		{
		case 1:
			LeanTween.rotateX(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_L).gameObject, base.transform.localEulerAngles.x + 45f, 0.5f).setLoopPingPong();
			LeanTween.rotateX(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).gameObject, base.transform.localEulerAngles.x + 45f, 0.5f).setLoopPingPong();
			break;
		case 2:
			LeanTween.rotateX(characterParts.Parts.RendererParts(CharacterParts.BodyPartsList.SHOULDER_R).gameObject, base.transform.localEulerAngles.x + 45f, 0.5f).setLoopPingPong();
			break;
		}
	}
}
