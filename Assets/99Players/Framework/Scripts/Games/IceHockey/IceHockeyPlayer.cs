using GamepadInput;
using UnityEngine;
public class IceHockeyPlayer : MonoBehaviour
{
	public enum PositionNo
	{
		CF,
		LW,
		RW,
		LD,
		RD,
		GK
	}
	public enum State
	{
		DEFAULT,
		FACE_OFF,
		SHOOT,
		CHECK,
		PASS,
		CATCH,
		KNOCK_BACK
	}
	[SerializeField]
	[Header("スタイル")]
	private CharacterStyle style;
	[SerializeField]
	[Header("リジッドボディ")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("AI")]
	private IceHockeyPlayerAI ai;
	[SerializeField]
	[Header("アニメ\u30fcション")]
	private IceHockeyPlayer_Animation anim;
	[SerializeField]
	[Header("パックホルダ\u30fcアンカ\u30fc")]
	private Transform puckHolderAnchor;
	[SerializeField]
	[Header("パックキャッチアンカ\u30fc")]
	private Transform puckCatchAnchor;
	[SerializeField]
	[Header("スティックトリガコライダ\u30fc")]
	private SphereCollider stickCol;
	[SerializeField]
	[Header("カ\u30fcソル")]
	private CharacterCursorAnimation cursor;
	[SerializeField]
	[Header("シュ\u30fcトゲ\u30fcジ")]
	private SpriteRenderer shootGauge;
	[SerializeField]
	[Header("チ\u30fcムマ\u30fcカ\u30fc")]
	private MeshRenderer teamMarker;
	[SerializeField]
	[Header("チ\u30fcムカラ\u30fc")]
	private Color[] arrayTeamColor;
	[SerializeField]
	[Header("パス対象カ\u30fcソル")]
	private SpriteRenderer passTargetCursor;
	[SerializeField]
	[Header("パス対象カ\u30fcソル画像")]
	private Sprite[] arrayPassTargetSprite;
	[SerializeField]
	[Header("パック保有時の追加コライダ\u30fc")]
	private CapsuleCollider puckHolderCol;
	[SerializeField]
	[Header("キャッチエフェクト")]
	private ParticleSystem psCatchEffect;
	[SerializeField]
	[Header("シュ\u30fcト時エフェクト")]
	private ParticleSystem[] psShootEffect;
	[SerializeField]
	[Header("スティックモデル")]
	private MeshRenderer stickModel;
	[SerializeField]
	[Header("スティックマテリアル")]
	private Material[] arrayStickMaterial;
	[SerializeField]
	[Header("映り込み")]
	private MeshRenderer refrectionShadow;
	[SerializeField]
	[Header("映り込み画像")]
	private Texture[] arrayMatRefrectionTex;
	[SerializeField]
	[Header("ノックバックエフェクト")]
	private ParticleSystem psKnockBackEffect;
	[SerializeField]
	[Header("ノックバックトレイル")]
	private TrailRenderer trailKnockBackEffect;
	private readonly float LOOK_SPEED = 10f;
	private readonly float MOVE_SPEED_MAX = 11f;
	private readonly float ATTENUATION_SCALE = 0.95f;
	private readonly float STAMINA_MAX = 1f;
	private readonly float PUCK_HOLDER_SPEED = 0.95f;
	private readonly float CATCH_LIMIT_TIME = 5f;
	private readonly float SE_RUN_TIME = 0.55f;
	private int playerIdx = -1;
	private int teamNo = -1;
	private PositionNo positionNo = PositionNo.GK;
	private JoyConManager.AXIS_INPUT axisInput;
	private Vector3 moveForce;
	private float catchInterval;
	private float moveSpeed;
	private float stamina = 1f;
	private float shootPower;
	private float stickCoolTime;
	private float defenceCheckTime;
	private float stateCheckTime;
	private float catchTime;
	private float passDelay;
	private float runSeTime;
	private bool isExitPuckOut;
	private Quaternion tempRot;
	private Vector3 prevDir;
	private IceHockeyPuck tempPuck;
	private IceHockeyPlayer tempTarget;
	private bool isChangeFrame;
	private float exitPuckTime;
	private float captureStandby;
	private State currentState;
	public Vector3 KeeperOffset => ai.KeeperOffset;
	public PositionNo Position
	{
		get
		{
			return positionNo;
		}
		set
		{
			positionNo = value;
		}
	}
	public bool IsPuckChase
	{
		get;
		set;
	}
	public bool IsPassDelay => passDelay > 0f;
	public float PassDelay
	{
		get
		{
			return passDelay;
		}
		set
		{
			passDelay = value;
		}
	}
	public Vector3 MoveDir => moveForce.normalized;
	public State CurrentState => currentState;
	public Transform PuckHolderAnchor => puckHolderAnchor;
	public Transform PuckCatchAnchor => puckCatchAnchor;
	public IceHockeyPlayer TempTarget => tempTarget;
	public bool IsCpu
	{
		get;
		set;
	}
	public int PlayerIdx => playerIdx;
	public int TeamNo => teamNo;
	public Rigidbody Rigid => rigid;
	public void Init(int _playerIdx, int _teamNo)
	{
		playerIdx = _playerIdx;
		IsCpu = (playerIdx >= SingletonCustom<GameSettingManager>.Instance.PlayerNum);
		ai.Init(this);
		teamNo = _teamNo;
		if (!IsCpu)
		{
			cursor.gameObject.SetActive(value: true);
			cursor.SetColor(playerIdx);
		}
		else
		{
			cursor.gameObject.SetActive(value: false);
		}
		teamMarker.material.color = arrayTeamColor[_teamNo];
		moveSpeed = MOVE_SPEED_MAX * 0.5f;
		prevDir = moveForce;
		SetFaceOff();
	}
	public void SetStyle()
	{
		stickModel.material = arrayStickMaterial[SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayTeamColor[teamNo]];
		refrectionShadow.material.mainTexture = arrayMatRefrectionTex[SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayTeamColor[teamNo]];
		style.SetGameStyle(GS_Define.GameType.BLOCK_WIPER, IsCpu ? (3 + playerIdx) : playerIdx, teamNo);
	}
	public void ChangePlayerIdx(int _playerIdx)
	{
		playerIdx = _playerIdx;
		IsCpu = (playerIdx >= SingletonCustom<GameSettingManager>.Instance.PlayerNum);
		if (!IsCpu)
		{
			cursor.gameObject.SetActive(value: true);
			cursor.SetColor(playerIdx);
		}
		else
		{
			cursor.gameObject.SetActive(value: false);
		}
		isChangeFrame = true;
	}
	public void SetFaceOff()
	{
		SetState(State.FACE_OFF);
		moveForce = (SingletonCustom<IceHockeyRinkManager>.Instance.GetOpponentGoalAnchor(teamNo).position - base.transform.position).normalized * 0.01f;
		tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * moveForce), 1f);
		rigid.rotation = tempRot;
		base.transform.rotation = tempRot;
		rigid.velocity = Vector3.zero;
		rigid.isKinematic = true;
		if (positionNo == PositionNo.GK)
		{
			anim.SetAnim(IceHockeyPlayer_Animation.AnimType.GK_STANDBY);
		}
		else
		{
			anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
		}
		stickCoolTime = 0f;
		stickCol.enabled = false;
		puckHolderCol.enabled = false;
		shootPower = 0f;
		shootGauge.material.SetFloat("_FillAmount", shootPower);
		passTargetCursor.enabled = false;
		trailKnockBackEffect.emitting = false;
		ai.SetFaceOff();
	}
	public void OnFaceOff()
	{
		if (currentState == State.FACE_OFF)
		{
			SetState(State.DEFAULT);
		}
		rigid.isKinematic = false;
		if (stickCoolTime <= 0f)
		{
			stickCoolTime = 0.1f;
		}
		if (positionNo != PositionNo.GK)
		{
			anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
		}
	}
	public void UpdateMethod()
	{
		catchInterval = Mathf.Clamp(catchInterval - Time.deltaTime, 0f, 1f);
		passDelay = Mathf.Clamp(passDelay - Time.deltaTime, 0f, 1f);
		if (isChangeFrame)
		{
			isChangeFrame = false;
			return;
		}
		if (stickCoolTime > 0f)
		{
			stickCoolTime -= Time.deltaTime;
			if (stickCoolTime <= 0f)
			{
				stickCol.enabled = true;
			}
		}
		if (exitPuckTime > 0f)
		{
			exitPuckTime -= Time.deltaTime;
			if (exitPuckTime <= 0f)
			{
				isExitPuckOut = false;
			}
		}
		if (captureStandby > 0f)
		{
			captureStandby -= Time.deltaTime;
		}
		if (IsCpu || SingletonCustom<IceHockeyPlayerManager>.Instance.IsOpponentCatch(teamNo))
		{
			switch (currentState)
			{
			case State.FACE_OFF:
				if (positionNo == PositionNo.CF && SingletonCustom<IceHockeyPlayerManager>.Instance.Referee.IsPuckRelease && ai.IsFaceOff())
				{
					SingletonCustom<IceHockeyPuck>.Instance.SetHolder(this);
					tempTarget = SingletonCustom<IceHockeyPlayerManager>.Instance.GetPlayerAtPosition(teamNo, PositionNo.LW);
					Pass();
					SingletonCustom<IceHockeyGameManager>.Instance.OnFaceOff();
				}
				break;
			case State.DEFAULT:
				moveForce = ai.UpdateForce();
				if (moveForce.magnitude < 0.0400000028f)
				{
					moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
				}
				else if (moveSpeed < MOVE_SPEED_MAX)
				{
					moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_MAX, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
				}
				if (SingletonCustom<IceHockeyPuck>.Instance.IsHoleder(playerIdx))
				{
					tempTarget = SingletonCustom<IceHockeyPlayerManager>.Instance.UpdatePassTarget(this);
					if (ai.IsShoot() && Vector3.Angle(MoveDir, SingletonCustom<IceHockeyRinkManager>.Instance.GetOpponentGoalAnchor(teamNo).transform.position - base.transform.position) <= 45f)
					{
						SetState(State.SHOOT);
					}
					else if (ai.IsPass() && tempTarget != null)
					{
						Pass();
					}
					if (anim.CurrentAnimType == IceHockeyPlayer_Animation.AnimType.MOVE)
					{
						anim.SetAnim(IceHockeyPlayer_Animation.AnimType.DRIBBLE);
					}
				}
				else if (anim.CurrentAnimType == IceHockeyPlayer_Animation.AnimType.DRIBBLE)
				{
					anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
				}
				if (ai.IsCheck())
				{
					SetState(State.CHECK);
					anim.SetAnim(IceHockeyPlayer_Animation.AnimType.CHECK);
					DefenceCheck();
				}
				break;
			case State.SHOOT:
				moveForce = ai.UpdateForce();
				if (moveForce.magnitude < 0.0400000028f)
				{
					moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
				}
				else if (moveSpeed < MOVE_SPEED_MAX)
				{
					moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_MAX, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
				}
				shootPower = Mathf.Clamp(shootPower + Time.deltaTime, 0f, 1f);
				if (shootPower >= UnityEngine.Random.Range(0.15f, 1f))
				{
					Shoot();
				}
				break;
			case State.CHECK:
				moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
				if (defenceCheckTime > 0f)
				{
					defenceCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
				break;
			case State.KNOCK_BACK:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				trailKnockBackEffect.emitting = false;
				SetState(State.DEFAULT);
				anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
				break;
			case State.PASS:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				if (positionNo == PositionNo.GK)
				{
					anim.SetAnim(IceHockeyPlayer_Animation.AnimType.GK_STANDBY);
				}
				else
				{
					anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
				}
				break;
			case State.CATCH:
				moveForce = (SingletonCustom<IceHockeyRinkManager>.Instance.GetOpponentGoalAnchor(teamNo).position - base.transform.position).normalized * 0.01f;
				if (ai.IsThorw())
				{
					tempTarget = SingletonCustom<IceHockeyPlayerManager>.Instance.UpdatePassTarget(this, _isDistancePriority: true);
					Pass(_isThrow: true);
					SetState(State.DEFAULT);
					catchInterval = 0.15f;
				}
				break;
			}
		}
		else
		{
			switch (currentState)
			{
			case State.FACE_OFF:
				if (positionNo == PositionNo.CF && SingletonCustom<IceHockeyPlayerManager>.Instance.Referee.IsPuckRelease && SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
				{
					SingletonCustom<IceHockeyPuck>.Instance.SetHolder(this);
					tempTarget = SingletonCustom<IceHockeyPlayerManager>.Instance.GetPlayerAtPosition(teamNo, PositionNo.LW);
					Pass();
					SingletonCustom<IceHockeyGameManager>.Instance.OnFaceOff();
				}
				break;
			case State.DEFAULT:
				UpdateMoveForce();
				if (SingletonCustom<IceHockeyPuck>.Instance.IsHoleder(playerIdx) && captureStandby <= 0f && SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
				{
					shootPower = 0f;
					shootGauge.material.SetFloat("_FillAmount", shootPower);
					SetState(State.SHOOT);
				}
				if (SingletonCustom<IceHockeyPuck>.Instance.IsHoleder(playerIdx))
				{
					tempTarget = SingletonCustom<IceHockeyPlayerManager>.Instance.UpdatePassTarget(this);
					if (tempTarget != null && SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.B))
					{
						Pass();
					}
					break;
				}
				if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
				{
					SetState(State.CHECK);
					anim.SetAnim(IceHockeyPlayer_Animation.AnimType.CHECK);
					DefenceCheck();
				}
				else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.X))
				{
					SingletonCustom<IceHockeyPlayerManager>.Instance.ChangePlayer(this);
				}
				if (anim.CurrentAnimType == IceHockeyPlayer_Animation.AnimType.DRIBBLE)
				{
					anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
				}
				break;
			case State.SHOOT:
				UpdateMoveForce();
				shootPower = Mathf.Clamp(shootPower + Time.deltaTime, 0f, 1f);
				shootGauge.material.SetFloat("_FillAmount", shootPower);
				if (SingletonCustom<IceHockeyPuck>.Instance.IsHoleder(playerIdx) && SingletonCustom<JoyConManager>.Instance.GetButtonUp(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
				{
					Shoot();
				}
				break;
			case State.CHECK:
				if (defenceCheckTime > 0f)
				{
					defenceCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
				break;
			case State.PASS:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				if (positionNo == PositionNo.GK)
				{
					anim.SetAnim(IceHockeyPlayer_Animation.AnimType.GK_STANDBY);
				}
				else
				{
					anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
				}
				break;
			case State.KNOCK_BACK:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
				break;
			case State.CATCH:
				UpdateMoveForce();
				catchTime -= Time.deltaTime;
				tempTarget = SingletonCustom<IceHockeyPlayerManager>.Instance.UpdatePassTarget(this, _isDistancePriority: true);
				if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.B) || SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
				{
					Pass(_isThrow: true);
					SetState(State.DEFAULT);
					catchInterval = 0.15f;
				}
				if (catchTime <= 0f)
				{
					moveForce = (SingletonCustom<IceHockeyRinkManager>.Instance.GetOpponentGoalAnchor(teamNo).position - base.transform.position).normalized * 0.01f;
					Pass(_isThrow: true);
					SetState(State.DEFAULT);
					catchInterval = 0.15f;
				}
				break;
			}
		}
		if (Mathf.Abs(Vector3.Angle(prevDir, moveForce)) >= 90f)
		{
			moveSpeed *= 0.5f;
			anim.EmitMoveEffct(1);
		}
		if (puckHolderCol.enabled && rigid.velocity.magnitude >= 1f)
		{
			runSeTime += Time.deltaTime;
			if (runSeTime >= SE_RUN_TIME)
			{
				runSeTime = 0f;
				SingletonCustom<AudioManager>.Instance.SePlay("se_run_ice");
			}
		}
		if (puckHolderCol.enabled && Time.frameCount % 3 == 0)
		{
			Collider[] array = Physics.OverlapSphere(puckHolderCol.transform.position, 1f);
			if (array.Length != 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].name.Contains("Goal") || array[i].name.Contains("Rink_Collider"))
					{
						rigid.AddForce(-moveForce, ForceMode.Impulse);
					}
				}
			}
		}
		prevDir = moveForce;
		anim.SetCharacterSpeed(rigid.velocity.magnitude);
	}
	public void UpdateAlways()
	{
		if (positionNo == PositionNo.GK)
		{
			CalcManager.mCalcVector3 = ((teamNo == 0) ? SingletonCustom<IceHockeyRinkManager>.Instance.AnchorFaceOff0_Team0[5].position : SingletonCustom<IceHockeyRinkManager>.Instance.AnchorFaceOff0_Team1[5].position);
			CalcManager.mCalcFloat = Vector3.Distance(base.transform.position, CalcManager.mCalcVector3);
			if (CalcManager.mCalcFloat >= 0.85f)
			{
				base.transform.position = CalcManager.mCalcVector3 + (base.transform.position - CalcManager.mCalcVector3).normalized * 0.85f;
			}
		}
	}
	private void UpdateMoveForce()
	{
		CalcManager.mCalcVector2 = IceHockeyControllerManager.GetStickDir(playerIdx);
		if (CalcManager.mCalcVector2.magnitude < 0.0400000028f)
		{
			moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
			if (!SingletonCustom<IceHockeyPuck>.Instance.IsHoleder(playerIdx))
			{
				stamina = Mathf.Clamp(stamina + Time.deltaTime * 2f, 0f, STAMINA_MAX);
			}
			else
			{
				stamina = Mathf.Clamp(stamina + Time.deltaTime * 0.1f, 0f, STAMINA_MAX);
			}
			return;
		}
		if (moveSpeed < MOVE_SPEED_MAX)
		{
			moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_MAX, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
		}
		moveForce.x = CalcManager.mCalcVector2.x;
		moveForce.z = CalcManager.mCalcVector2.y;
		moveForce = moveForce.normalized;
		if (SingletonCustom<IceHockeyPuck>.Instance.IsHoleder(playerIdx))
		{
			stamina = Mathf.Clamp(stamina - Time.deltaTime * 0.1f, 0f, STAMINA_MAX);
		}
		else
		{
			stamina = Mathf.Clamp(stamina + Time.deltaTime * 2f, 0f, STAMINA_MAX);
		}
	}
	public void Pass(bool _isThrow = false)
	{
		CalcManager.mCalcVector3 = tempTarget.transform.position - base.transform.position;
		CalcManager.mCalcVector3.y = base.transform.position.y;
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * CalcManager.mCalcVector3), 1f);
		SingletonCustom<IceHockeyPuck>.Instance.Pass(tempTarget, _isThrow);
		SetState(State.PASS);
		if (positionNo == PositionNo.GK)
		{
			anim.SetAnim(IceHockeyPlayer_Animation.AnimType.GK_THROW);
		}
		else
		{
			anim.SetAnim(IceHockeyPlayer_Animation.AnimType.PASS);
		}
		stateCheckTime = 0.25f;
		stickCol.enabled = false;
		stickCoolTime = 0.55f;
	}
	public void Shoot()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 1.5f);
		if (array.Length != 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].name.Contains("Goal") || array[i].name.Contains("Rink_Collider"))
				{
					shootPower = 0f;
					shootGauge.material.SetFloat("_FillAmount", shootPower);
					SetState(State.DEFAULT);
					return;
				}
			}
		}
		if (SingletonCustom<IceHockeyRinkManager>.Instance.IsOpponentArea(this))
		{
			CalcManager.mCalcVector3 = SingletonCustom<IceHockeyRinkManager>.Instance.GetOpponentGoalAnchor(teamNo).position - base.transform.position;
			CalcManager.mCalcVector3.y = base.transform.position.y;
			UnityEngine.Debug.Log("Angle:" + Vector3.Angle(base.transform.forward, CalcManager.mCalcVector3).ToString());
			psShootEffect[(!(Vector3.Angle(base.transform.forward, CalcManager.mCalcVector3) <= 90f)) ? 1 : 0].Play();
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * CalcManager.mCalcVector3), 1f);
			CalcManager.mCalcVector3 = SingletonCustom<IceHockeyRinkManager>.Instance.GetOpponentGoalAnchor(teamNo).position;
			CalcManager.mCalcVector3.y = base.transform.position.y;
			CalcManager.mCalcVector3.z += UnityEngine.Random.Range(-1f, 1f);
			SingletonCustom<IceHockeyPuck>.Instance.Shoot((CalcManager.mCalcVector3 - (base.transform.position + moveForce.normalized * rigid.velocity.magnitude * 0.1f)).normalized, shootPower);
		}
		else
		{
			SingletonCustom<IceHockeyPuck>.Instance.Shoot(moveForce.normalized, shootPower * 0.5f);
			psShootEffect[0].Play();
		}
		shootPower = 0f;
		shootGauge.material.SetFloat("_FillAmount", shootPower);
		SetState(State.PASS);
		anim.SetAnim(IceHockeyPlayer_Animation.AnimType.PASS);
		stateCheckTime = 0.25f;
		stickCol.enabled = false;
		stickCoolTime = 0.55f;
		if (!IsCpu)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerIdx);
		}
	}
	public float GetStaminaPer()
	{
		return stamina / STAMINA_MAX;
	}
	public void HavePuck()
	{
		puckHolderCol.enabled = true;
		if (SingletonCustom<IceHockeyPuck>.Instance.LastHolder != null && teamNo != SingletonCustom<IceHockeyPuck>.Instance.LastHolder.teamNo)
		{
			captureStandby = 0.25f;
		}
		ai.HavePuck();
		runSeTime = SE_RUN_TIME;
		if (positionNo == PositionNo.GK)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_keeper_catch");
		}
		else
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_icehockey_puck");
		}
	}
	public void KnockBack(Vector3 _pos, Vector3 _force)
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_collision_character_1");
		psKnockBackEffect.transform.position = base.transform.position + (base.transform.position - _pos) * 0.5f;
		psKnockBackEffect.Emit(1);
		trailKnockBackEffect.emitting = true;
		rigid.velocity = Vector3.zero;
		rigid.AddForce(_force, ForceMode.Impulse);
		SetState(State.KNOCK_BACK);
		anim.SetAnim(IceHockeyPlayer_Animation.AnimType.STANDBY);
		stateCheckTime = 0.85f;
		anim.EmitMoveEffct(1);
	}
	public void LostPuck()
	{
		isExitPuckOut = true;
		exitPuckTime = 0.5f;
		puckHolderCol.enabled = false;
		if (currentState == State.SHOOT)
		{
			SetState(State.DEFAULT);
			anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
			stickCol.enabled = false;
			stickCoolTime = 0.55f;
		}
		shootPower = 0f;
		shootGauge.material.SetFloat("_FillAmount", shootPower);
	}
	public void MoveInertia()
	{
		moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
		anim.SetCharacterSpeed(rigid.velocity.magnitude);
	}
	public void FixedUpdate()
	{
		switch (currentState)
		{
		case State.DEFAULT:
			if (stamina <= 0f)
			{
				rigid.velocity = Vector3.Slerp(rigid.velocity, moveForce * moveSpeed * 0.5f * (SingletonCustom<IceHockeyPuck>.Instance.IsHoleder(playerIdx) ? PUCK_HOLDER_SPEED : 1f), 0.05f);
			}
			else
			{
				rigid.velocity = Vector3.Slerp(rigid.velocity, moveForce * moveSpeed * (SingletonCustom<IceHockeyPuck>.Instance.IsHoleder(playerIdx) ? PUCK_HOLDER_SPEED : 1f), 0.05f);
			}
			break;
		case State.SHOOT:
			if (stamina <= 0f)
			{
				rigid.velocity = Vector3.Slerp(rigid.velocity, moveForce * moveSpeed * 0.45f * (SingletonCustom<IceHockeyPuck>.Instance.IsHoleder(playerIdx) ? PUCK_HOLDER_SPEED : 1f), 0.05f);
			}
			else
			{
				rigid.velocity = Vector3.Slerp(rigid.velocity, moveForce * moveSpeed * 0.5f * (SingletonCustom<IceHockeyPuck>.Instance.IsHoleder(playerIdx) ? PUCK_HOLDER_SPEED : 1f), 0.05f);
			}
			break;
		case State.CATCH:
			rigid.velocity = Vector3.zero;
			break;
		}
		switch (currentState)
		{
		case State.DEFAULT:
		case State.SHOOT:
		case State.CATCH:
			if (positionNo == PositionNo.GK && currentState != State.CATCH)
			{
				CalcManager.mCalcVector3 = SingletonCustom<IceHockeyPuck>.Instance.transform.position;
				CalcManager.mCalcVector3.y = base.transform.position.y;
				tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * (CalcManager.mCalcVector3 - (base.transform.position + KeeperOffset)).normalized), Time.deltaTime * LOOK_SPEED);
				if (tempRot != Quaternion.identity)
				{
					rigid.MoveRotation(tempRot);
					base.transform.rotation = tempRot;
				}
			}
			else if (moveForce.magnitude >= 0.01f)
			{
				tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * moveForce), Time.deltaTime * LOOK_SPEED);
				if (tempRot != Quaternion.identity)
				{
					base.transform.rotation = tempRot;
					rigid.MoveRotation(tempRot);
				}
			}
			break;
		}
		if (positionNo == PositionNo.GK)
		{
			CalcManager.mCalcVector3 = ((teamNo == 0) ? SingletonCustom<IceHockeyRinkManager>.Instance.AnchorFaceOff0_Team0[5].position : SingletonCustom<IceHockeyRinkManager>.Instance.AnchorFaceOff0_Team1[5].position);
			CalcManager.mCalcFloat = Vector3.Distance(base.transform.position, CalcManager.mCalcVector3);
			if (CalcManager.mCalcFloat >= 0.85f)
			{
				base.transform.position = CalcManager.mCalcVector3 + (base.transform.position - CalcManager.mCalcVector3).normalized * 0.85f;
			}
		}
	}
	public void ShowTargetCursor(bool _enable)
	{
		if (_enable)
		{
			passTargetCursor.enabled = true;
			passTargetCursor.sprite = arrayPassTargetSprite[SingletonCustom<IceHockeyPuck>.Instance.Holder.PlayerIdx];
		}
		else
		{
			passTargetCursor.enabled = false;
		}
	}
	private void DefenceCheck()
	{
		rigid.AddForce(moveForce, ForceMode.Impulse);
		defenceCheckTime = 1f;
		stickCol.enabled = false;
		stickCoolTime = 0.01f;
	}
	public void OnCollisionEnter(Collision collision)
	{
		if (SingletonCustom<IceHockeyGameManager>.Instance.CurrentState == IceHockeyGameManager.State.GoalWait || !(catchInterval <= 0f) || positionNo != PositionNo.GK || !collision.gameObject.name.Equals("Puck"))
		{
			return;
		}
		tempPuck = collision.gameObject.GetComponent<IceHockeyPuck>();
		if (!(tempPuck != null) || !(tempPuck.Holder == null))
		{
			return;
		}
		if (tempPuck.Rigid.velocity.magnitude >= 10f && UnityEngine.Random.Range(0, 100) <= 70)
		{
			tempPuck.Rigid.velocity *= 0.3f;
			catchInterval = 0.25f;
			SingletonCustom<AudioManager>.Instance.SePlay("se_icehockey_puck");
			SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
			return;
		}
		tempPuck.SetHolder(this);
		anim.SetAnim(IceHockeyPlayer_Animation.AnimType.GK_CATCH);
		moveSpeed = 0f;
		Rigid.velocity = Vector3.zero;
		psCatchEffect.Play();
		SetState(State.CATCH);
		if (IsCpu)
		{
			SingletonCustom<IceHockeyPlayerManager>.Instance.CheckChangePlayer(this);
		}
		if (!IsCpu)
		{
			catchTime = CATCH_LIMIT_TIME;
		}
	}
	private void OnCollisionStay(Collision collision)
	{
		if (SingletonCustom<IceHockeyGameManager>.Instance.CurrentState == IceHockeyGameManager.State.GoalWait || !(catchInterval <= 0f) || positionNo != PositionNo.GK || !collision.gameObject.name.Equals("Puck"))
		{
			return;
		}
		tempPuck = collision.gameObject.GetComponent<IceHockeyPuck>();
		if (tempPuck != null && tempPuck.Holder == null)
		{
			tempPuck.SetHolder(this);
			anim.SetAnim(IceHockeyPlayer_Animation.AnimType.GK_CATCH);
			moveSpeed = 0f;
			Rigid.velocity = Vector3.zero;
			psCatchEffect.Play();
			SetState(State.CATCH);
			if (IsCpu)
			{
				SingletonCustom<IceHockeyPlayerManager>.Instance.CheckChangePlayer(this);
			}
			if (!IsCpu)
			{
				catchTime = CATCH_LIMIT_TIME;
			}
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (SingletonCustom<IceHockeyGameManager>.Instance.CurrentState == IceHockeyGameManager.State.GoalWait || SingletonCustom<IceHockeyGameManager>.Instance.CurrentState != IceHockeyGameManager.State.InGame || !other.name.Equals("Puck"))
		{
			return;
		}
		tempPuck = other.GetComponent<IceHockeyPuck>();
		if ((bool)tempPuck.Holder && tempPuck.Holder == this)
		{
			return;
		}
		if ((bool)tempPuck.Holder && tempPuck.Holder != this)
		{
			if (positionNo == PositionNo.GK)
			{
				tempPuck.SetHolder(this);
				anim.SetAnim(IceHockeyPlayer_Animation.AnimType.GK_CATCH);
				moveSpeed = 0f;
				Rigid.velocity = Vector3.zero;
				psCatchEffect.Play();
				SetState(State.CATCH);
				if (IsCpu)
				{
					SingletonCustom<IceHockeyPlayerManager>.Instance.CheckChangePlayer(this);
				}
			}
			else if (currentState == State.CHECK && tempPuck.Holder.positionNo != PositionNo.GK)
			{
				tempPuck.Holder.KnockBack(base.transform.position, (tempPuck.Holder.transform.position - base.transform.position).normalized * 10f);
				tempPuck.SetHolder(this);
				anim.SetAnim(IceHockeyPlayer_Animation.AnimType.DRIBBLE);
				moveSpeed *= 0.5f;
				SetState(State.DEFAULT);
				if (IsCpu)
				{
					SingletonCustom<IceHockeyPlayerManager>.Instance.CheckChangePlayer(this);
				}
			}
			return;
		}
		if (positionNo == PositionNo.GK)
		{
			UnityEngine.Debug.Log("威力:" + tempPuck.Rigid.velocity.magnitude.ToString());
			if (tempPuck.Rigid.velocity.magnitude >= 10f && UnityEngine.Random.Range(0, 100) <= 70)
			{
				tempPuck.Rigid.velocity *= 0.3f;
				catchInterval = 0.25f;
				SingletonCustom<AudioManager>.Instance.SePlay("se_icehockey_puck");
				SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
				return;
			}
			tempPuck.SetHolder(this);
			anim.SetAnim(IceHockeyPlayer_Animation.AnimType.GK_CATCH);
			moveSpeed = 0f;
			Rigid.velocity = Vector3.zero;
			psCatchEffect.Play();
			SetState(State.CATCH);
			if (IsCpu)
			{
				SingletonCustom<IceHockeyPlayerManager>.Instance.CheckChangePlayer(this);
			}
			if (!IsCpu)
			{
				catchTime = CATCH_LIMIT_TIME;
			}
			return;
		}
		Collider[] array = Physics.OverlapSphere(puckHolderCol.transform.position, 0.65f);
		if (array.Length != 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].name.Contains("Goal"))
				{
					return;
				}
			}
		}
		tempPuck.SetHolder(this);
		SetState(State.DEFAULT);
		anim.SetAnim(IceHockeyPlayer_Animation.AnimType.DRIBBLE);
		if (passDelay > 0f)
		{
			moveSpeed *= 0.9f;
		}
		else
		{
			moveSpeed *= 0.75f;
		}
		CalcManager.mCalcVector3 = tempPuck.transform.position - base.transform.position;
		CalcManager.mCalcVector3.y = base.transform.position.y;
		moveForce = CalcManager.mCalcVector3;
		if (IsCpu)
		{
			SingletonCustom<IceHockeyPlayerManager>.Instance.CheckChangePlayer(this);
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if (SingletonCustom<IceHockeyGameManager>.Instance.CurrentState == IceHockeyGameManager.State.GoalWait || SingletonCustom<IceHockeyGameManager>.Instance.CurrentState != IceHockeyGameManager.State.InGame || !other.name.Equals("Puck"))
		{
			return;
		}
		tempPuck = other.GetComponent<IceHockeyPuck>();
		if ((!tempPuck.Holder || !(tempPuck.Holder == this)) && (!tempPuck.Holder || !(tempPuck.Holder != this)) && positionNo == PositionNo.GK)
		{
			tempPuck.SetHolder(this);
			anim.SetAnim(IceHockeyPlayer_Animation.AnimType.GK_CATCH);
			moveSpeed = 0f;
			Rigid.velocity = Vector3.zero;
			psCatchEffect.Play();
			SetState(State.CATCH);
			if (IsCpu)
			{
				SingletonCustom<IceHockeyPlayerManager>.Instance.CheckChangePlayer(this);
			}
			if (!IsCpu)
			{
				catchTime = CATCH_LIMIT_TIME;
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.name.Equals("Puck") && isExitPuckOut)
		{
			isExitPuckOut = false;
			exitPuckTime = 0f;
		}
	}
	private void SetState(State _state)
	{
		currentState = _state;
		switch (currentState)
		{
		case State.DEFAULT:
			trailKnockBackEffect.emitting = false;
			break;
		case State.CATCH:
			SingletonCustom<AudioManager>.Instance.SePlay("se_keeper_catch");
			break;
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(puckHolderCol.gameObject);
	}
}
