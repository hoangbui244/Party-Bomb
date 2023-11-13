using GamepadInput;
using SaveDataDefine;
using System;
using UnityEngine;
public class BeachSoccerPlayer : MonoBehaviour
{
	public enum PositionNo
	{
		FW,
		LMF,
		RMF,
		DF,
		GK
	}
	public enum State
	{
		DEFAULT,
		KICK_OFF,
		SHOOT,
		SLIDING,
		PASS,
		CATCH,
		JUMP,
		JUMP_SHOOT,
		KEEPER_JUMP,
		GOAL_CLEARANCE_WAIT,
		KNOCK_BACK,
		THROW_IN_WAIT,
		THROW_IN,
		CORNER_KICK_WAIT,
		CORNER_KICK,
		WAIT
	}
	[SerializeField]
	[Header("スタイル")]
	private CharacterStyle style;
	[SerializeField]
	[Header("リジッドボディ")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("AI")]
	private BeachSoccerPlayerAI ai;
	[SerializeField]
	[Header("アニメ\u30fcション")]
	private BeachSoccerPlayer_Animation anim;
	[SerializeField]
	[Header("ボ\u30fcルホルダ\u30fcアンカ\u30fc")]
	private Transform ballHolderAnchor;
	[SerializeField]
	[Header("パックキャッチアンカ\u30fc")]
	private Transform puckCatchAnchor;
	[SerializeField]
	[Header("スロ\u30fcインアンカ\u30fc")]
	private Transform throwInAnchor;
	[SerializeField]
	[Header("カ\u30fcソル")]
	private CharacterCursorAnimation cursor;
	[SerializeField]
	[Header("パス対象カ\u30fcソル")]
	private SpriteRenderer passTargetCursor;
	[SerializeField]
	[Header("パス対象カ\u30fcソル画像")]
	private Sprite[] arrayPassTargetSprite;
	[SerializeField]
	[Header("ボ\u30fcル保有時の追加コライダ\u30fc")]
	private SphereCollider holderCol;
	[SerializeField]
	[Header("キャッチエフェクト")]
	private ParticleSystem psCatchEffect;
	[SerializeField]
	[Header("ボ\u30fcル接触エフェクト")]
	private ParticleSystem psBallEffect;
	[SerializeField]
	[Header("シュ\u30fcト時エフェクト")]
	private ParticleSystem[] psShootEffect;
	[SerializeField]
	[Header("スライディングエフェクト")]
	private ParticleSystem psSlidingEffect;
	[SerializeField]
	[Header("ダメ\u30fcジエフェクト")]
	private ParticleSystem psDamage;
	[SerializeField]
	[Header("シュ\u30fcトゲ\u30fcジ")]
	private SpriteRenderer shootGauge;
	[SerializeField]
	[Header("足跡アンカ\u30fc")]
	private Transform footprintAnchor;
	[SerializeField]
	[Header("影用スフィア")]
	private GameObject[] arrayShadowSphere;
	[SerializeField]
	[Header("回転用アンカ\u30fc")]
	private Transform rotateAnchor;
	[SerializeField]
	private Transform soleAnchor_R;
	[SerializeField]
	private Transform soleAnchor_L;
	private readonly float LOOK_SPEED = 11f;
	private float MOVE_SPEED_MAX = 2.25f;
	private readonly float ATTENUATION_SCALE = 0.89f;
	private readonly float MOVE_SPEED_SCALE = 20f;
	private readonly float STAMINA_MAX = 1f;
	private readonly float BALL_HOLDER_SPEED = 0.8f;
	private readonly float CATCH_LIMIT_TIME = 4f;
	private readonly float ACTION_LIMIT_TIME = 4f;
	private readonly float SE_RUN_TIME = 0.55f;
	private readonly float WAIT_HOLDER = 0.075f;
	private float CURSOR_ROT_SPEED = 0.5f;
	private float cursorRotTime;
	private float SLIDING_DISTANCE = 4f;
	private float moveSpeed;
	private float stamina = 1f;
	private float shootPower;
	private float defenceCheckTime;
	private float stateCheckTime;
	private float catchTime;
	private float actionTime;
	private float passDelay;
	private float runSeTime;
	private Vector3 moveForce;
	private float catchInterval;
	private int playerIdx = -1;
	private int teamNo = -1;
	private PositionNo positionNo = PositionNo.GK;
	private float jumpRotateTime;
	protected Vector3 prevPos;
	protected Vector3 nowPos;
	private Quaternion tempRot;
	private Vector3 prevDir;
	private BeachSoccerBall tempBall;
	private bool isChangeFrame;
	private BeachSoccerPlayer tempTarget;
	private Vector3 rotVec;
	private BeachSoccerPlayer tempPlayer;
	private float captureStandby;
	private State prevState;
	private float footDistance;
	private bool isFootR;
	private Vector3 frameDis;
	private float calcJump;
	private State currentState;
	public float CatchTimePer => catchTime / CATCH_LIMIT_TIME;
	public float ActionTimePer => actionTime / ACTION_LIMIT_TIME;
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
	public bool IsBallChase
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
	public Transform BallHolderAnchor => ballHolderAnchor;
	public Transform BallCatchAnchor => puckCatchAnchor;
	public Transform BallThrowInAnchor => throwInAnchor;
	public BeachSoccerPlayer TempTarget => tempTarget;
	public bool IsCpu
	{
		get;
		set;
	}
	public int PlayerIdx => playerIdx;
	public int TeamNo => teamNo;
	public float MoveSpeed => moveSpeed;
	public Rigidbody Rigid => rigid;
	public CharacterStyle Style => style;
	public void Init(int _playerIdx, int _teamNo)
	{
		playerIdx = _playerIdx;
		IsCpu = (playerIdx >= SingletonCustom<GameSettingManager>.Instance.PlayerNum);
		ai.Init();
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
		anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
		moveSpeed = MOVE_SPEED_MAX * 0.5f;
		prevDir = moveForce;
		SetKickOff();
		footprintAnchor.parent = SingletonCustom<BeachSoccerPlayerManager>.Instance.GetFootprintAnchorParent();
	}
	public void ClearFootData()
	{
		prevPos = (nowPos = base.transform.position);
		footDistance = 0f;
		RecoveryStamina();
	}
	private void CreateFootprint()
	{
		int num = 0;
		Transform child;
		while (true)
		{
			if (num < footprintAnchor.childCount)
			{
				child = footprintAnchor.GetChild(num);
				if (!child.gameObject.activeSelf)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		child.gameObject.SetActive(value: true);
		if (isFootR)
		{
			Vector3 position = soleAnchor_R.position;
			child.SetPositionX(position.x);
			child.SetPositionZ(position.z);
			child.SetLocalScaleX(-0.05f);
		}
		else
		{
			Vector3 position2 = soleAnchor_L.position;
			child.SetPositionX(position2.x);
			child.SetPositionZ(position2.z);
			child.SetLocalScaleX(0.05f);
		}
		child.SetLocalEulerAnglesY(base.transform.localEulerAngles.y);
		isFootR = !isFootR;
	}
	public void SetStyle()
	{
		if (playerIdx >= BeachSoccerDefine.UNIQUE_CHARACTER_NUM)
		{
			if (positionNo != PositionNo.GK)
			{
				StyleTextureManager.GenderType genderType = (StyleTextureManager.GenderType)UnityEngine.Random.Range(0, 2);
				style.SetStyle(genderType, (StyleTextureManager.FaceType)UnityEngine.Random.Range(0, 4), (StyleTextureManager.HairColorType)UnityEngine.Random.Range(0, 3), (CharacterStyle.ShapeType)UnityEngine.Random.Range(0, 3));
				style.SetBeachSoccerColor(SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayTeamColor[teamNo]);
				return;
			}
			int num = 0;
			switch (SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayTeamColor[teamNo])
			{
			case 0:
			case 2:
			case 5:
			case 6:
				num = 0;
				break;
			case 1:
			case 3:
			case 4:
			case 7:
				num = 2;
				break;
			}
			if (teamNo == 1)
			{
				int num2 = 0;
				switch (SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayTeamColor[teamNo])
				{
				case 0:
				case 2:
				case 5:
				case 6:
					num2 = 0;
					break;
				case 1:
				case 3:
				case 4:
				case 7:
					num2 = 2;
					break;
				}
				if (num == num2)
				{
					num++;
				}
			}
			style.SetKeeper(num);
		}
		else
		{
			style.SetGameStyle(GS_Define.GameType.ATTACK_BALL, IsCpu ? (3 + playerIdx) : playerIdx, teamNo);
			style.SetBeachSoccerColor(SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayTeamColor[teamNo]);
		}
	}
	public void UpdateMethod()
	{
		catchInterval = Mathf.Clamp(catchInterval - Time.deltaTime, 0f, 1f);
		passDelay = Mathf.Clamp(passDelay - Time.deltaTime, 0f, 1f);
		prevPos = nowPos;
		nowPos = base.transform.position;
		frameDis = nowPos - prevPos;
		frameDis.y = 0f;
		footDistance += frameDis.magnitude;
		if (footDistance > 0.5f)
		{
			footDistance -= 0.5f;
			CreateFootprint();
		}
		if (isChangeFrame)
		{
			isChangeFrame = false;
			return;
		}
		if (captureStandby > 0f)
		{
			captureStandby -= Time.deltaTime;
		}
		if (IsCpu || SingletonCustom<BeachSoccerPlayerManager>.Instance.IsOpponentCatch(teamNo))
		{
			switch (currentState)
			{
			case State.KICK_OFF:
				if (positionNo == PositionNo.FW && SingletonCustom<BeachSoccerGameManager>.Instance.StartKickOffTeamNo == teamNo && ai.IsKickOff())
				{
					SingletonCustom<BeachSoccerBall>.Instance.SetHolder(this);
					tempTarget = SingletonCustom<BeachSoccerPlayerManager>.Instance.GetPlayerAtPosition(teamNo, PositionNo.LMF);
					Pass();
					SingletonCustom<BeachSoccerGameManager>.Instance.OnKickOff();
				}
				break;
			case State.THROW_IN_WAIT:
				if (positionNo == PositionNo.LMF && SingletonCustom<BeachSoccerGameManager>.Instance.StartThrowInTeamNo == teamNo && ai.IsThorw())
				{
					Vector3 vector = new Vector3(UnityEngine.Random.Range(-0.35f, 0.35f), 0f, 0f);
					if (base.transform.position.x - SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.x < (0f - BeachSoccerFieldManager.RINK_X_SIZE) * 0.5f)
					{
						vector.x = 0.35f;
					}
					if (base.transform.position.x - SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.x > BeachSoccerFieldManager.RINK_X_SIZE * 0.5f)
					{
						vector.x = -0.35f;
					}
					if (base.transform.position.z > SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.z)
					{
						vector.z = -1f;
					}
					else
					{
						vector.z = 1f;
					}
					vector = vector.normalized;
					SingletonCustom<BeachSoccerBall>.Instance.ThrowIn(vector, UnityEngine.Random.Range(0.45f, 1f));
					base.transform.LookAt(base.transform.position + vector);
					SetState(State.WAIT);
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.THROW_IN);
					shootPower = 0f;
					stateCheckTime = 1.5f;
					moveForce = vector;
				}
				break;
			case State.CORNER_KICK_WAIT:
				if (positionNo == PositionNo.LMF && SingletonCustom<BeachSoccerGameManager>.Instance.StartCornerKickTeamNo == teamNo && ai.IsThorw())
				{
					Vector3 vector2 = new Vector3(0f, 0f, 0f);
					if (base.transform.position.x > SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.x)
					{
						vector2.x = UnityEngine.Random.Range(-0.66f, -0.33f);
					}
					else
					{
						vector2.x = UnityEngine.Random.Range(0.33f, 0.66f);
					}
					if (base.transform.position.z > SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.z)
					{
						vector2.z = -1f;
					}
					else
					{
						vector2.z = 1f;
					}
					vector2 = vector2.normalized;
					SingletonCustom<BeachSoccerBall>.Instance.CornerKick(this, vector2, UnityEngine.Random.Range(0.65f, 1f));
					base.transform.LookAt(base.transform.position + vector2);
					SetState(State.WAIT);
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.KICK);
					shootPower = 0f;
					stateCheckTime = 1.5f;
				}
				break;
			case State.DEFAULT:
				moveForce = Vector3.Slerp(moveForce, ai.UpdateForce(), 0.5f);
				if (moveForce.magnitude < 0.0400000028f)
				{
					moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
					switch (anim.CurrentAnimType)
					{
					case BeachSoccerPlayer_Animation.AnimType.DRIBBLE:
					case BeachSoccerPlayer_Animation.AnimType.MOVE:
						anim.SetAnimSpeed(moveSpeed * 1.25f);
						if (moveSpeed <= 0.5f)
						{
							anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
						}
						break;
					case BeachSoccerPlayer_Animation.AnimType.GK_STANDBY:
						anim.SetAnimSpeed(moveSpeed * 1.25f);
						break;
					}
				}
				else
				{
					if (moveSpeed < MOVE_SPEED_MAX)
					{
						moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_MAX, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
					}
					switch (anim.CurrentAnimType)
					{
					case BeachSoccerPlayer_Animation.AnimType.STANDBY:
						if (SingletonCustom<BeachSoccerBall>.Instance.Holder != null && SingletonCustom<BeachSoccerBall>.Instance.Holder != this)
						{
							anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.MOVE);
						}
						else
						{
							anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.DRIBBLE);
						}
						anim.SetAnimSpeed(moveSpeed * 1.25f);
						break;
					case BeachSoccerPlayer_Animation.AnimType.DRIBBLE:
					case BeachSoccerPlayer_Animation.AnimType.MOVE:
					case BeachSoccerPlayer_Animation.AnimType.GK_STANDBY:
						anim.SetAnimSpeed(moveSpeed * 1.25f);
						break;
					}
					if (moveSpeed < MOVE_SPEED_MAX)
					{
						moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_SCALE, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
					}
				}
				if (SingletonCustom<BeachSoccerBall>.Instance.IsHoleder(playerIdx))
				{
					tempTarget = SingletonCustom<BeachSoccerPlayerManager>.Instance.UpdatePassTarget(this);
					if (CheckJump() && SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
					{
						Jump();
					}
					else if (ai.IsShoot() && Vector3.Angle(MoveDir, SingletonCustom<BeachSoccerFieldManager>.Instance.GetOpponentGoalAnchor(teamNo).transform.position - base.transform.position) <= 45f)
					{
						SetState(State.SHOOT);
					}
					else if (ai.IsPass() && tempTarget != null)
					{
						Pass();
					}
					if (anim.CurrentAnimType == BeachSoccerPlayer_Animation.AnimType.MOVE)
					{
						anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.DRIBBLE);
					}
				}
				else if (anim.CurrentAnimType == BeachSoccerPlayer_Animation.AnimType.DRIBBLE)
				{
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.MOVE);
				}
				if (ai.IsSliding())
				{
					SetState(State.SLIDING);
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.SLIDING);
					DefenceSliding();
				}
				if (positionNo == PositionNo.GK)
				{
					CheckKeeperBallJump();
				}
				break;
			case State.KEEPER_JUMP:
				if (jumpRotateTime > 0f)
				{
					jumpRotateTime -= Time.deltaTime;
				}
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_STANDBY);
				break;
			case State.JUMP:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
				break;
			case State.JUMP_SHOOT:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
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
			case State.SLIDING:
				moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
				if (defenceCheckTime > 0f)
				{
					defenceCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
				break;
			case State.KNOCK_BACK:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
				break;
			case State.GOAL_CLEARANCE_WAIT:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
				}
				else
				{
					SetState(State.CATCH);
				}
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
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_STANDBY);
				}
				else
				{
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
				}
				break;
			case State.CATCH:
				if (jumpRotateTime > 0f)
				{
					jumpRotateTime -= Time.deltaTime;
					return;
				}
				moveForce = (SingletonCustom<BeachSoccerFieldManager>.Instance.GetOpponentGoalAnchor(teamNo).position - base.transform.position).normalized * 0.01f;
				if (ai.IsThorw())
				{
					tempTarget = SingletonCustom<BeachSoccerPlayerManager>.Instance.UpdatePassTarget(this, _isDistancePriority: true);
					Pass(_isThrow: true);
					catchInterval = 0.15f;
				}
				break;
			case State.WAIT:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
				break;
			}
		}
		else
		{
			switch (currentState)
			{
			case State.KICK_OFF:
				if (positionNo == PositionNo.FW && SingletonCustom<BeachSoccerGameManager>.Instance.StartKickOffTeamNo == teamNo)
				{
					actionTime -= Time.deltaTime;
					if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A) || SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.B) || actionTime <= 0f)
					{
						SingletonCustom<BeachSoccerBall>.Instance.SetHolder(this);
						tempTarget = SingletonCustom<BeachSoccerPlayerManager>.Instance.GetPlayerAtPosition(teamNo, PositionNo.LMF);
						Pass();
						SingletonCustom<BeachSoccerGameManager>.Instance.OnKickOff();
					}
				}
				break;
			case State.DEFAULT:
				UpdateMoveForce();
				if (SingletonCustom<BeachSoccerBall>.Instance.IsHoleder(playerIdx) && captureStandby <= 0f && SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
				{
					shootPower = 0f;
					shootGauge.material.SetFloat("_FillAmount", shootPower);
					SetState(State.SHOOT);
				}
				if (SingletonCustom<BeachSoccerBall>.Instance.IsHoleder(playerIdx))
				{
					tempTarget = SingletonCustom<BeachSoccerPlayerManager>.Instance.UpdatePassTarget(this);
					if (tempTarget != null && SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.B))
					{
						Pass();
					}
					break;
				}
				if (CheckJump() && SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
				{
					Jump();
				}
				else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
				{
					SetState(State.SLIDING);
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.SLIDING);
					DefenceSliding();
				}
				else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.X))
				{
					SingletonCustom<BeachSoccerPlayerManager>.Instance.ChangePlayer(this);
				}
				if (anim.CurrentAnimType == BeachSoccerPlayer_Animation.AnimType.DRIBBLE)
				{
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.MOVE);
				}
				break;
			case State.SHOOT:
				UpdateMoveForce();
				shootPower = Mathf.Clamp(shootPower + Time.deltaTime * 1.25f, 0f, 1f);
				shootGauge.material.SetFloat("_FillAmount", shootPower);
				if (SingletonCustom<BeachSoccerBall>.Instance.IsHoleder(playerIdx) && SingletonCustom<JoyConManager>.Instance.GetButtonUp(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
				{
					Shoot();
				}
				break;
			case State.SLIDING:
				if (defenceCheckTime > 0f)
				{
					defenceCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
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
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_STANDBY);
				}
				else
				{
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
				}
				break;
			case State.KNOCK_BACK:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
				break;
			case State.GOAL_CLEARANCE_WAIT:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
				}
				else
				{
					SetState(State.CATCH);
				}
				break;
			case State.KEEPER_JUMP:
				if (jumpRotateTime > 0f)
				{
					jumpRotateTime -= Time.deltaTime;
				}
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_STANDBY);
				break;
			case State.JUMP:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
				break;
			case State.JUMP_SHOOT:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
				break;
			case State.CATCH:
				if (jumpRotateTime > 0f)
				{
					jumpRotateTime -= Time.deltaTime;
					return;
				}
				UpdateMoveForce();
				catchTime -= Time.deltaTime;
				tempTarget = SingletonCustom<BeachSoccerPlayerManager>.Instance.UpdatePassTarget(this, _isDistancePriority: true);
				if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.B) || SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A))
				{
					Pass(_isThrow: true);
					catchInterval = 0.15f;
				}
				if (catchTime <= 0f)
				{
					moveForce = (SingletonCustom<BeachSoccerFieldManager>.Instance.GetOpponentGoalAnchor(teamNo).position - base.transform.position).normalized * 0.01f;
					Pass(_isThrow: true);
					catchInterval = 0.15f;
				}
				break;
			case State.THROW_IN_WAIT:
				if (positionNo == PositionNo.LMF && SingletonCustom<BeachSoccerGameManager>.Instance.StartThrowInTeamNo == teamNo)
				{
					cursorRotTime += CURSOR_ROT_SPEED * Time.deltaTime;
					cursor.transform.SetLocalEulerAnglesY(Mathf.Sin(cursorRotTime * (float)Math.PI * 2f) * -70f);
					actionTime -= Time.deltaTime;
					if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A) || actionTime <= 0f)
					{
						SetState(State.THROW_IN);
					}
				}
				break;
			case State.THROW_IN:
				shootPower = Mathf.Clamp(shootPower + Time.deltaTime * 1.25f, 0f, 1f);
				shootGauge.material.SetFloat("_FillAmount", shootPower);
				if (SingletonCustom<JoyConManager>.Instance.GetButtonUp(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A) || shootPower >= 1f || actionTime <= 0f)
				{
					SingletonCustom<BeachSoccerBall>.Instance.ThrowIn(cursor.transform.forward, shootPower);
					base.transform.LookAt(base.transform.position + cursor.transform.forward);
					SetState(State.WAIT);
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.THROW_IN);
					shootPower = 0f;
					stateCheckTime = 1.5f;
					moveForce = cursor.transform.forward;
				}
				break;
			case State.CORNER_KICK_WAIT:
				if (positionNo == PositionNo.LMF && SingletonCustom<BeachSoccerGameManager>.Instance.StartCornerKickTeamNo == teamNo)
				{
					cursorRotTime += CURSOR_ROT_SPEED * Time.deltaTime;
					cursor.transform.SetLocalEulerAnglesY(Mathf.Sin(cursorRotTime * (float)Math.PI * 2f) * -33f);
					actionTime -= Time.deltaTime;
					if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A) || actionTime <= 0f)
					{
						SetState(State.CORNER_KICK);
					}
				}
				break;
			case State.CORNER_KICK:
				shootPower = Mathf.Clamp(shootPower + Time.deltaTime * 1.25f, 0f, 1f);
				shootGauge.material.SetFloat("_FillAmount", shootPower);
				if (SingletonCustom<JoyConManager>.Instance.GetButtonUp(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId(playerIdx), SatGamePad.Button.A) || shootPower >= 1f || actionTime <= 0f)
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
					SingletonCustom<BeachSoccerBall>.Instance.CornerKick(this, cursor.transform.forward, shootPower);
					base.transform.LookAt(base.transform.position + cursor.transform.forward);
					SetState(State.WAIT);
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.KICK);
					shootPower = 0f;
					stateCheckTime = 1.5f;
				}
				break;
			case State.WAIT:
				if (stateCheckTime > 0f)
				{
					stateCheckTime -= Time.deltaTime;
					break;
				}
				SetState(State.DEFAULT);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
				break;
			}
		}
		if (Mathf.Abs(Vector3.Angle(prevDir, moveForce)) >= 90f)
		{
			moveSpeed *= 0.5f;
			anim.EmitMoveEffct(1);
		}
		prevDir = moveForce;
		anim.SetCharacterSpeed(rigid.velocity.magnitude);
	}
	private bool CheckJump()
	{
		if (SingletonCustom<BeachSoccerBall>.Instance.IsAir())
		{
			return true;
		}
		return false;
	}
	private void Jump()
	{
		CalcManager.mCalcVector3 = (SingletonCustom<BeachSoccerBall>.Instance.transform.position + SingletonCustom<BeachSoccerBall>.Instance.Rigid.velocity * 0.25f - base.transform.position).normalized;
		CalcManager.mCalcVector3.y = 2f;
		rigid.AddForce(CalcManager.mCalcVector3 * 2f, ForceMode.Impulse);
		stateCheckTime = 1.25f;
		anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
		SetState(State.JUMP);
	}
	private void CheckKeeperBallJump()
	{
		if (SingletonCustom<BeachSoccerBall>.Instance.Holder == null && SingletonCustom<BeachSoccerBall>.Instance.LastHolder.teamNo != teamNo && SingletonCustom<BeachSoccerBall>.Instance.Rigid.velocity.magnitude >= 2.7f && Vector3.Distance(base.transform.position, SingletonCustom<BeachSoccerBall>.Instance.transform.position) <= 1.25f)
		{
			UnityEngine.Debug.Log("K-jump:" + Vector3.Distance(base.transform.position, SingletonCustom<BeachSoccerBall>.Instance.transform.position).ToString());
			rigid.constraints = RigidbodyConstraints.FreezeRotation;
			SetState(State.KEEPER_JUMP);
			stateCheckTime = 0.75f;
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_JUMP);
			CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerBall>.Instance.transform.position + SingletonCustom<BeachSoccerBall>.Instance.Rigid.velocity.normalized * 0.5f - base.transform.position;
			CalcManager.mCalcVector3.x *= 0.25f;
			CalcManager.mCalcVector3 = CalcManager.mCalcVector3.normalized;
			UnityEngine.Debug.Log("K-y:" + CalcManager.mCalcVector3.y.ToString());
			if (CalcManager.mCalcVector3.y >= 0.75f)
			{
				CalcManager.mCalcVector3.y = 2f;
			}
			switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
			{
			case SystemData.AiStrength.WEAK:
				calcJump = UnityEngine.Random.Range(2f, 3f);
				break;
			case SystemData.AiStrength.NORAML:
			case SystemData.AiStrength.STRONG:
				calcJump = UnityEngine.Random.Range(3f, 4f);
				break;
			}
			rigid.AddForce(new Vector3(CalcManager.mCalcVector3.x * calcJump, CalcManager.mCalcVector3.y * (calcJump * 0.5f), CalcManager.mCalcVector3.z * calcJump), ForceMode.Impulse);
			jumpRotateTime = 0f;
			if (CalcManager.mCalcVector3.z > 0.3f)
			{
				LeanTween.rotateZ(base.gameObject, (teamNo == 0) ? 90f : (-90f), 0.25f);
				arrayShadowSphere[0].SetActive(value: false);
				arrayShadowSphere[1].SetActive(value: true);
				jumpRotateTime = 1.1f;
			}
			if (CalcManager.mCalcVector3.z < -0.3f)
			{
				LeanTween.rotateZ(base.gameObject, (teamNo == 0) ? (-90f) : 90f, 0.25f);
				arrayShadowSphere[0].SetActive(value: false);
				arrayShadowSphere[1].SetActive(value: true);
				jumpRotateTime = 1.1f;
			}
		}
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position, base.transform.position + moveForce);
	}
	private void UpdateMoveForce()
	{
		CalcManager.mCalcVector2 = IceHockeyControllerManager.GetStickDir(playerIdx);
		if (CalcManager.mCalcVector2.magnitude < 0.0400000028f)
		{
			moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
			if (!SingletonCustom<BeachSoccerBall>.Instance.IsHoleder(playerIdx))
			{
				stamina = Mathf.Clamp(stamina + Time.deltaTime * 2f, 0f, STAMINA_MAX);
			}
			else
			{
				stamina = Mathf.Clamp(stamina + Time.deltaTime * 0.2f, 0f, STAMINA_MAX);
			}
			BeachSoccerPlayer_Animation.AnimType currentAnimType = anim.CurrentAnimType;
			if (currentAnimType == BeachSoccerPlayer_Animation.AnimType.DRIBBLE || currentAnimType == BeachSoccerPlayer_Animation.AnimType.MOVE)
			{
				anim.SetAnimSpeed(moveSpeed * 1.25f);
				if (moveSpeed <= 0.5f)
				{
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
				}
			}
			moveForce = prevDir;
		}
		else
		{
			switch (anim.CurrentAnimType)
			{
			case BeachSoccerPlayer_Animation.AnimType.STANDBY:
				if (SingletonCustom<BeachSoccerBall>.Instance.Holder != null && SingletonCustom<BeachSoccerBall>.Instance.Holder != this)
				{
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.MOVE);
				}
				else
				{
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.DRIBBLE);
				}
				anim.SetAnimSpeed(moveSpeed * 1.25f);
				break;
			case BeachSoccerPlayer_Animation.AnimType.DRIBBLE:
			case BeachSoccerPlayer_Animation.AnimType.MOVE:
				anim.SetAnimSpeed(moveSpeed * 1.25f);
				break;
			}
			if (moveSpeed < MOVE_SPEED_MAX)
			{
				moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_SCALE, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
			}
			moveForce.x = CalcManager.mCalcVector2.x;
			moveForce.z = CalcManager.mCalcVector2.y;
			moveForce = moveForce.normalized;
			if (SingletonCustom<BeachSoccerBall>.Instance.IsHoleder(playerIdx))
			{
				stamina = Mathf.Clamp(stamina - Time.deltaTime * 0.2f, 0f, STAMINA_MAX);
			}
			else
			{
				stamina = Mathf.Clamp(stamina + Time.deltaTime * 2f, 0f, STAMINA_MAX);
			}
		}
		if (Mathf.Abs(Vector3.Angle(prevDir, moveForce)) >= 90f)
		{
			moveSpeed *= 0.1f;
		}
		prevDir = moveForce;
	}
	public void FixedUpdate()
	{
		if (jumpRotateTime > 0f)
		{
			jumpRotateTime -= Time.deltaTime;
			return;
		}
		switch (currentState)
		{
		case State.DEFAULT:
			rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
			if (SingletonCustom<BeachSoccerBall>.Instance.Holder != null && SingletonCustom<BeachSoccerBall>.Instance.Holder == this)
			{
				if (stamina <= 0f)
				{
					rigid.MovePosition(rigid.transform.position + new Vector3((moveForce * moveSpeed).x, rigid.velocity.y, (moveForce * moveSpeed).z) * BALL_HOLDER_SPEED * 0.5f * Time.deltaTime);
				}
				else
				{
					rigid.MovePosition(rigid.transform.position + new Vector3((moveForce * moveSpeed).x, rigid.velocity.y, (moveForce * moveSpeed).z) * BALL_HOLDER_SPEED * Time.deltaTime);
				}
			}
			else
			{
				rigid.MovePosition(rigid.transform.position + new Vector3((moveForce * moveSpeed).x, rigid.velocity.y, (moveForce * moveSpeed).z) * Time.deltaTime);
			}
			break;
		case State.SHOOT:
			if (stamina <= 0f)
			{
				rigid.MovePosition(rigid.transform.position + new Vector3((moveForce * moveSpeed).x, rigid.velocity.y, (moveForce * moveSpeed).z) * BALL_HOLDER_SPEED * 0.45f * Time.deltaTime);
			}
			else
			{
				rigid.MovePosition(rigid.transform.position + new Vector3((moveForce * moveSpeed).x, rigid.velocity.y, (moveForce * moveSpeed).z) * BALL_HOLDER_SPEED * 0.5f * Time.deltaTime);
			}
			break;
		case State.CATCH:
			rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
			break;
		case State.THROW_IN_WAIT:
			rigid.velocity = Vector3.zero;
			break;
		}
		switch (currentState)
		{
		case State.KICK_OFF:
		case State.SLIDING:
		case State.PASS:
			break;
		case State.DEFAULT:
		case State.SHOOT:
		case State.CATCH:
			if (positionNo == PositionNo.GK && currentState != State.CATCH)
			{
				CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
				CalcManager.mCalcVector3.y = base.transform.position.y;
				tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, 0f, 0f) * (CalcManager.mCalcVector3 - base.transform.position).normalized), Time.deltaTime * LOOK_SPEED);
				if (tempRot != Quaternion.identity)
				{
					rigid.MoveRotation(tempRot);
					base.transform.rotation = tempRot;
				}
			}
			else if (moveForce.magnitude >= 0.01f)
			{
				rotVec.x = moveForce.x;
				rotVec.z = moveForce.z;
				tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, 0f, 0f) * rotVec), Time.deltaTime * LOOK_SPEED);
				if (tempRot != Quaternion.identity)
				{
					base.transform.rotation = tempRot;
					rigid.MoveRotation(tempRot);
				}
			}
			break;
		}
	}
	public void Shoot()
	{
		if (IsCpu)
		{
			switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
			{
			case SystemData.AiStrength.WEAK:
				if (shootPower >= 0.7f)
				{
					shootPower = 0.6f;
				}
				break;
			case SystemData.AiStrength.STRONG:
				if (UnityEngine.Random.Range(0, 100) <= 50)
				{
					shootPower = 0.7f;
				}
				break;
			}
		}
		UnityEngine.Debug.Log("シュ\u30fcト:" + shootPower.ToString());
		if (SingletonCustom<BeachSoccerFieldManager>.Instance.IsOpponentArea(this))
		{
			if (shootPower >= 0.7f)
			{
				CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerFieldManager>.Instance.GetOpponentGoalAnchor(teamNo).position;
				CalcManager.mCalcVector3.y = base.transform.position.y;
				CalcManager.mCalcVector3.z += UnityEngine.Random.Range(-1f, 1f);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, 0f, 0f) * (base.transform.position - CalcManager.mCalcVector3)), 1f);
				SingletonCustom<BeachSoccerBall>.Instance.SetFloatBall();
				SingletonCustom<BeachSoccerBall>.Instance.Float(base.transform.up, 0.15f);
				Vector3 dir = (CalcManager.mCalcVector3 - base.transform.position).normalized;
				LeanTween.delayedCall(0.15f, (Action)delegate
				{
					if (SingletonCustom<BeachSoccerBall>.Instance.Holder == null)
					{
						rigid.AddForce(base.transform.up * 2f, ForceMode.Impulse);
						LeanTween.rotateAround(rotateAnchor.gameObject, rotateAnchor.right, -360f, 0.25f).setDelay(0.02f);
						SingletonCustom<BeachSoccerBall>.Instance.Shoot(dir, shootPower);
						psShootEffect[0].transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
						psShootEffect[0].Play();
						shootPower = 0f;
						shootGauge.material.SetFloat("_FillAmount", shootPower);
						SetState(State.JUMP_SHOOT);
						anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.KICK);
						stateCheckTime = 0.5f;
					}
				});
				moveForce = dir;
			}
			else if (IsCpu)
			{
				CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerFieldManager>.Instance.GetOpponentGoalAnchor(teamNo).position - base.transform.position;
				CalcManager.mCalcVector3.y = base.transform.position.y;
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, 0f, 0f) * CalcManager.mCalcVector3), 1f);
				CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerFieldManager>.Instance.GetOpponentGoalAnchor(teamNo).position;
				CalcManager.mCalcVector3.y = base.transform.position.y;
				CalcManager.mCalcVector3.z += UnityEngine.Random.Range(-1f, 1f);
				SingletonCustom<BeachSoccerBall>.Instance.Shoot((CalcManager.mCalcVector3 - (base.transform.position + moveForce.normalized * rigid.velocity.magnitude * 0.1f)).normalized, shootPower);
			}
			else
			{
				SingletonCustom<BeachSoccerBall>.Instance.Shoot(moveForce.normalized, shootPower);
			}
			psShootEffect[0].transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
			psShootEffect[0].Play();
		}
		else
		{
			SingletonCustom<BeachSoccerBall>.Instance.Shoot(moveForce.normalized, shootPower * 0.5f);
			psShootEffect[0].transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
			psShootEffect[0].Play();
		}
		shootPower = 0f;
		shootGauge.material.SetFloat("_FillAmount", shootPower);
		SetState(State.PASS);
		anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.KICK);
		stateCheckTime = 0.25f;
		if (!IsCpu)
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerIdx);
		}
	}
	public void Pass(bool _isThrow = false, bool _isCross = false)
	{
		CalcManager.mCalcVector3 = tempTarget.transform.position - base.transform.position;
		CalcManager.mCalcVector3.y = base.transform.position.y;
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, 0f, 0f) * CalcManager.mCalcVector3), 1f);
		if (!IsCpu)
		{
			UnityEngine.Debug.Log("CrossKicker:" + SingletonCustom<BeachSoccerFieldManager>.Instance.IsCrossKicker(this).ToString());
			UnityEngine.Debug.Log("CrossTarget:" + SingletonCustom<BeachSoccerFieldManager>.Instance.IsCrossTarget(tempTarget).ToString());
		}
		if (SingletonCustom<BeachSoccerFieldManager>.Instance.IsCrossKicker(this) && SingletonCustom<BeachSoccerFieldManager>.Instance.IsCrossTarget(tempTarget))
		{
			_isCross = true;
		}
		SingletonCustom<BeachSoccerBall>.Instance.Pass(tempTarget, _isThrow, _isCross);
		if (_isCross)
		{
			SingletonCustom<BeachSoccerPlayerManager>.Instance.ChangePlayerDirect(this, tempTarget);
		}
		SetState(State.PASS);
		if (positionNo == PositionNo.GK)
		{
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_THROW);
		}
		else
		{
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.KICK);
		}
		stateCheckTime = 0.25f;
	}
	private void DefenceSliding()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_oni_sliding");
		rigid.AddForce(base.transform.forward * SLIDING_DISTANCE, ForceMode.Impulse);
		psSlidingEffect.Play();
		defenceCheckTime = 1.15f;
	}
	public void LostBall()
	{
		UnityEngine.Debug.Log("LostBall");
		holderCol.enabled = false;
		if (currentState == State.SHOOT)
		{
			SetState(State.DEFAULT);
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.MOVE);
		}
		shootPower = 0f;
		shootGauge.material.SetFloat("_FillAmount", shootPower);
	}
	public void HaveBall()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_ball_bound");
		UnityEngine.Debug.Log("HaveBall");
		holderCol.enabled = true;
		if (SingletonCustom<BeachSoccerBall>.Instance.LastHolder != null && teamNo != SingletonCustom<BeachSoccerBall>.Instance.LastHolder.teamNo)
		{
			captureStandby = 0.45f;
		}
		ai.HaveBall();
		runSeTime = SE_RUN_TIME;
		if (positionNo == PositionNo.GK)
		{
			if (SingletonCustom<BeachSoccerGameManager>.Instance.CurrentState != BeachSoccerGameManager.State.GoalClearanceWait)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_keeper_catch");
			}
		}
		else
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_ball_fall");
		}
	}
	public void KnockBack(Vector3 _pos, Vector3 _force)
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_collision_character_1");
		rigid.velocity = Vector3.zero;
		rigid.AddForce(_force, ForceMode.Impulse);
		SetState(State.KNOCK_BACK);
		anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
		stateCheckTime = 0.85f;
		psDamage.Play();
		anim.EmitMoveEffct(1);
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
	public void ShowTargetCursor(bool _enable)
	{
		if (_enable)
		{
			passTargetCursor.enabled = true;
			passTargetCursor.sprite = arrayPassTargetSprite[SingletonCustom<BeachSoccerBall>.Instance.Holder.PlayerIdx];
		}
		else
		{
			passTargetCursor.enabled = false;
		}
	}
	public void OnKickOff()
	{
		if (currentState == State.KICK_OFF)
		{
			SetState(State.DEFAULT);
		}
		rigid.isKinematic = false;
		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		if (positionNo != PositionNo.GK)
		{
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
		}
	}
	public void SetKickOff()
	{
		SetState(State.KICK_OFF);
		moveForce = (SingletonCustom<BeachSoccerFieldManager>.Instance.GetOpponentGoalAnchor(teamNo).position - base.transform.position).normalized * 0.01f;
		tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, 0f, 0f) * moveForce), 1f);
		rigid.rotation = tempRot;
		base.transform.rotation = tempRot;
		rigid.velocity = Vector3.zero;
		rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
		rigid.isKinematic = true;
		anim.Reset();
		if (positionNo == PositionNo.GK)
		{
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_STANDBY);
		}
		else
		{
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
		}
		if (positionNo == PositionNo.FW)
		{
			actionTime = ACTION_LIMIT_TIME;
		}
		holderCol.enabled = false;
		shootPower = 0f;
		shootGauge.material.SetFloat("_FillAmount", shootPower);
		passTargetCursor.enabled = false;
		ai.SetKickOff();
		psSlidingEffect.Stop();
	}
	public void RecoveryStamina()
	{
		stamina = STAMINA_MAX;
	}
	public void SetThrowIn()
	{
		SetState(State.THROW_IN_WAIT);
		Vector3 position;
		if (SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.z < SingletonCustom<BeachSoccerBall>.Instance.PosLineOut.z)
		{
			position = SingletonCustom<BeachSoccerFieldManager>.Instance.ThrowInAnchor_Back.position;
			base.transform.SetLocalEulerAnglesY(180f);
		}
		else
		{
			position = SingletonCustom<BeachSoccerFieldManager>.Instance.ThrowInAnchor_Front.position;
			base.transform.SetLocalEulerAnglesY(0f);
		}
		position.x = SingletonCustom<BeachSoccerBall>.Instance.PosLineOut.x;
		base.transform.position = position;
		anim.Reset();
		anim.CalcReset();
		anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.THROW_IN_WAIT);
		SingletonCustom<BeachSoccerBall>.Instance.SetHolder(this);
		rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
		rigid.isKinematic = true;
		psSlidingEffect.Stop();
		actionTime = ACTION_LIMIT_TIME;
	}
	public void SetCornerKick()
	{
		SetState(State.CORNER_KICK_WAIT);
		if (SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.z < SingletonCustom<BeachSoccerBall>.Instance.PosLineOut.z)
		{
			switch (SingletonCustom<BeachSoccerGameManager>.Instance.StartCornerKickTeamNo)
			{
			case 0:
				base.transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorCornerKick_Team0[1].position;
				base.transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorCornerKick_Team0[1].rotation;
				break;
			case 1:
				base.transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorCornerKick_Team1[1].position;
				base.transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorCornerKick_Team1[1].rotation;
				break;
			}
		}
		else
		{
			switch (SingletonCustom<BeachSoccerGameManager>.Instance.StartCornerKickTeamNo)
			{
			case 0:
				base.transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorCornerKick_Team0[0].position;
				base.transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorCornerKick_Team0[0].rotation;
				break;
			case 1:
				base.transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorCornerKick_Team1[0].position;
				base.transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorCornerKick_Team1[0].rotation;
				break;
			}
		}
		anim.Reset();
		anim.CalcReset();
		anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
		SingletonCustom<BeachSoccerBall>.Instance.SetHolder(this);
		rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
		rigid.isKinematic = true;
		psSlidingEffect.Stop();
		actionTime = ACTION_LIMIT_TIME;
	}
	public float GetStaminaPer()
	{
		return stamina / STAMINA_MAX;
	}
	public void MoveInertia()
	{
		moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
		anim.SetAnimSpeed(moveSpeed * 1.25f);
		anim.SetCharacterSpeed(rigid.velocity.magnitude);
		switch (currentState)
		{
		case State.SLIDING:
			moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
			if (defenceCheckTime > 0f)
			{
				defenceCheckTime -= Time.deltaTime;
				break;
			}
			SetState(State.DEFAULT);
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
			break;
		case State.JUMP:
			if (stateCheckTime > 0f)
			{
				stateCheckTime -= Time.deltaTime;
				break;
			}
			SetState(State.DEFAULT);
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
			break;
		case State.JUMP_SHOOT:
			if (stateCheckTime > 0f)
			{
				stateCheckTime -= Time.deltaTime;
				break;
			}
			SetState(State.DEFAULT);
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
			break;
		case State.WAIT:
			if (stateCheckTime > 0f)
			{
				stateCheckTime -= Time.deltaTime;
				break;
			}
			SetState(State.DEFAULT);
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.STANDBY);
			break;
		}
	}
	public void UpdateAlways()
	{
	}
	public void SetGoalClearance()
	{
		anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_CATCH);
		moveSpeed = 0f;
		moveForce = Vector3.zero;
		arrayShadowSphere[0].SetActive(value: true);
		arrayShadowSphere[1].SetActive(value: false);
		Rigid.velocity = Vector3.zero;
		stateCheckTime = 0.5f;
		SetState(State.GOAL_CLEARANCE_WAIT);
	}
	private void SetState(State _state)
	{
		prevState = currentState;
		currentState = _state;
		State state = currentState;
		if (state <= State.SLIDING)
		{
			if (state == State.DEFAULT)
			{
				psSlidingEffect.Stop();
				cursor.transform.SetLocalEulerAnglesY(0f);
				rigid.isKinematic = false;
				rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				arrayShadowSphere[1].SetActive(value: false);
				arrayShadowSphere[0].SetActive(value: true);
				rotateAnchor.SetLocalEulerAnglesX(0f);
			}
			return;
		}
		switch (state)
		{
		case State.CATCH:
			if (SingletonCustom<BeachSoccerGameManager>.Instance.CurrentState != BeachSoccerGameManager.State.GoalClearanceWait && prevState != State.GOAL_CLEARANCE_WAIT)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_keeper_catch");
			}
			arrayShadowSphere[1].SetActive(value: false);
			arrayShadowSphere[0].SetActive(value: true);
			moveForce = (SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position - base.transform.position).normalized;
			break;
		case State.THROW_IN_WAIT:
			psSlidingEffect.Stop();
			break;
		}
	}
	private void OnCollisionEnter(Collision other)
	{
		if (SingletonCustom<BeachSoccerGameManager>.Instance.CurrentState == BeachSoccerGameManager.State.GoalWait || SingletonCustom<BeachSoccerGameManager>.Instance.CurrentState != BeachSoccerGameManager.State.InGame)
		{
			return;
		}
		if (other.gameObject.name.Contains("Player"))
		{
			BeachSoccerPlayer component = other.gameObject.GetComponent<BeachSoccerPlayer>();
			if (component != null && SingletonCustom<BeachSoccerBall>.Instance.IsHoleder(component.PlayerIdx))
			{
				tempBall = SingletonCustom<BeachSoccerBall>.Instance.GetComponent<BeachSoccerBall>();
				if (positionNo == PositionNo.GK)
				{
					if (catchInterval > 0f)
					{
						return;
					}
					tempBall.SetHolder(this);
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_CATCH);
					moveSpeed = 0f;
					Rigid.velocity = Vector3.zero;
					psCatchEffect.transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
					psCatchEffect.Play();
					SetState(State.CATCH);
					if (IsCpu)
					{
						SingletonCustom<BeachSoccerPlayerManager>.Instance.CheckChangePlayer(this);
					}
					if (!IsCpu)
					{
						catchTime = CATCH_LIMIT_TIME;
					}
				}
				else
				{
					switch (currentState)
					{
					case State.DEFAULT:
						if (tempBall.Holder.teamNo != teamNo)
						{
							if (tempBall.Holder.positionNo == PositionNo.GK || tempBall.Holder.CurrentState == State.THROW_IN_WAIT || Vector3.Angle(tempBall.Holder.Style.transform.forward, base.transform.position - tempBall.Holder.transform.position) >= 90f)
							{
								return;
							}
							tempBall.Holder.KnockBack(base.transform.position, (tempBall.Holder.transform.position - base.transform.position).normalized * 3f);
							tempBall.SetHolder(this);
							anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.DRIBBLE);
							moveSpeed *= 0.5f;
							psBallEffect.transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
							psBallEffect.Play();
							stateCheckTime = WAIT_HOLDER;
							SetState(State.WAIT);
							if (IsCpu)
							{
								SingletonCustom<BeachSoccerPlayerManager>.Instance.CheckChangePlayer(this);
							}
						}
						break;
					case State.SLIDING:
						if (tempBall.Holder.positionNo == PositionNo.GK || tempBall.Holder.CurrentState == State.THROW_IN_WAIT || Vector3.Angle(tempBall.Holder.Style.transform.forward, base.transform.position - tempBall.Holder.transform.position) >= 120f)
						{
							return;
						}
						tempBall.Holder.KnockBack(base.transform.position, (tempBall.Holder.transform.position - base.transform.position).normalized * 3f);
						tempBall.SetHolder(this);
						anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.DRIBBLE);
						moveSpeed *= 0.5f;
						psBallEffect.transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
						psBallEffect.Play();
						stateCheckTime = WAIT_HOLDER;
						SetState(State.WAIT);
						if (IsCpu)
						{
							SingletonCustom<BeachSoccerPlayerManager>.Instance.CheckChangePlayer(this);
						}
						break;
					}
				}
			}
		}
		if (!other.gameObject.name.Equals("Ball"))
		{
			return;
		}
		tempBall = other.gameObject.GetComponent<BeachSoccerBall>();
		if (currentState != 0 && currentState != State.JUMP && currentState != State.KEEPER_JUMP)
		{
			if (tempBall.Holder == null)
			{
				tempBall.LastHolder = this;
			}
		}
		else
		{
			if ((bool)tempBall.Holder && tempBall.Holder == this)
			{
				return;
			}
			if ((bool)tempBall.Holder && tempBall.Holder != this)
			{
				if (positionNo == PositionNo.GK)
				{
					if (!(catchInterval > 0f))
					{
						tempBall.SetHolder(this);
						anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_CATCH);
						moveSpeed = 0f;
						Rigid.velocity = Vector3.zero;
						psCatchEffect.transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
						psCatchEffect.Play();
						SetState(State.CATCH);
						if (IsCpu)
						{
							SingletonCustom<BeachSoccerPlayerManager>.Instance.CheckChangePlayer(this);
						}
					}
				}
				else if (!(stateCheckTime > 0f) && currentState == State.SLIDING && tempBall.Holder.positionNo != PositionNo.GK && tempBall.Holder.CurrentState != State.THROW_IN_WAIT && !(Vector3.Angle(tempBall.Holder.Style.transform.forward, base.transform.position - tempBall.Holder.transform.position) >= 120f))
				{
					tempBall.Holder.KnockBack(base.transform.position, (tempBall.Holder.transform.position - base.transform.position).normalized * 3f);
					tempBall.SetHolder(this);
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.DRIBBLE);
					moveSpeed *= 0.5f;
					psBallEffect.transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
					psBallEffect.Play();
					stateCheckTime = WAIT_HOLDER;
					SetState(State.WAIT);
					if (IsCpu)
					{
						SingletonCustom<BeachSoccerPlayerManager>.Instance.CheckChangePlayer(this);
					}
				}
				return;
			}
			UnityEngine.Debug.Log("フリ\u30fc");
			if (positionNo == PositionNo.GK)
			{
				if (catchInterval > 0f)
				{
					return;
				}
				UnityEngine.Debug.Log("威力:" + tempBall.Rigid.velocity.magnitude.ToString());
				if (tempBall.Rigid.velocity.magnitude >= 4.5f && UnityEngine.Random.Range(0, 100) <= 50)
				{
					tempBall.Rigid.velocity *= 0.3f;
					catchInterval = 0.25f;
					SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
					SingletonCustom<AudioManager>.Instance.SePlay("se_ball_fall");
					tempBall.LastHolder = this;
					return;
				}
				tempBall.SetHolder(this);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_CATCH);
				moveSpeed = 0f;
				Rigid.velocity = Vector3.zero;
				psCatchEffect.transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
				psCatchEffect.Play();
				SetState(State.CATCH);
				if (IsCpu)
				{
					SingletonCustom<BeachSoccerPlayerManager>.Instance.CheckChangePlayer(this);
				}
				if (!IsCpu)
				{
					catchTime = CATCH_LIMIT_TIME;
				}
			}
			else
			{
				if (currentState == State.JUMP_SHOOT)
				{
					return;
				}
				if (currentState == State.JUMP && SingletonCustom<BeachSoccerBall>.Instance.LastHolder.teamNo == teamNo)
				{
					LeanTween.rotateAround(rotateAnchor.gameObject, rotateAnchor.right, -360f, 0.25f).setDelay(0.02f);
					CalcManager.mCalcVector3 = SingletonCustom<BeachSoccerFieldManager>.Instance.GetOpponentGoalAnchor(teamNo).position;
					CalcManager.mCalcVector3.y = base.transform.position.y;
					CalcManager.mCalcVector3.z += UnityEngine.Random.Range(-1f, 1f);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, 0f, 0f) * (base.transform.position - CalcManager.mCalcVector3)), 1f);
					Vector3 normalized = (CalcManager.mCalcVector3 - base.transform.position).normalized;
					SingletonCustom<BeachSoccerBall>.Instance.Shoot(normalized, shootPower, _isAir: true);
					psShootEffect[0].transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
					psShootEffect[0].Play();
					shootPower = 0f;
					shootGauge.material.SetFloat("_FillAmount", shootPower);
					SetState(State.JUMP_SHOOT);
					anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.KICK);
					stateCheckTime = 0.5f;
					return;
				}
				if (tempBall.LastHolder.teamNo != teamNo && tempBall.Rigid.velocity.magnitude >= 2f && UnityEngine.Random.Range(0, 100) <= 50)
				{
					tempBall.LastHolder = this;
					return;
				}
				tempBall.SetHolder(this);
				stateCheckTime = WAIT_HOLDER;
				SetState(State.WAIT);
				psBallEffect.transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
				psBallEffect.Play();
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.DRIBBLE);
				if (passDelay > 0f)
				{
					moveSpeed *= 0.9f;
				}
				else
				{
					moveSpeed *= 0.75f;
				}
				CalcManager.mCalcVector3 = tempBall.transform.position - base.transform.position;
				CalcManager.mCalcVector3.y = base.transform.position.y;
				moveForce = CalcManager.mCalcVector3;
				if (IsCpu)
				{
					SingletonCustom<BeachSoccerPlayerManager>.Instance.CheckChangePlayer(this);
				}
			}
		}
	}
	private void OnCollisionStay(Collision other)
	{
		if (SingletonCustom<BeachSoccerGameManager>.Instance.CurrentState == BeachSoccerGameManager.State.GoalWait || SingletonCustom<BeachSoccerGameManager>.Instance.CurrentState != BeachSoccerGameManager.State.InGame || currentState != 0 || !other.gameObject.name.Equals("Ball"))
		{
			return;
		}
		tempBall = other.gameObject.GetComponent<BeachSoccerBall>();
		if (((bool)tempBall.Holder && tempBall.Holder == this) || ((bool)tempBall.Holder && tempBall.Holder != this))
		{
			return;
		}
		if (positionNo == PositionNo.GK)
		{
			if (!(catchInterval > 0f))
			{
				tempBall.SetHolder(this);
				anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.GK_CATCH);
				moveSpeed = 0f;
				Rigid.velocity = Vector3.zero;
				psCatchEffect.transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
				psCatchEffect.Play();
				SetState(State.CATCH);
				if (IsCpu)
				{
					SingletonCustom<BeachSoccerPlayerManager>.Instance.CheckChangePlayer(this);
				}
				if (!IsCpu)
				{
					catchTime = CATCH_LIMIT_TIME;
				}
			}
		}
		else
		{
			tempBall.SetHolder(this);
			stateCheckTime = WAIT_HOLDER;
			SetState(State.WAIT);
			psBallEffect.transform.position = SingletonCustom<BeachSoccerBall>.Instance.transform.position;
			psBallEffect.Play();
			anim.SetAnim(BeachSoccerPlayer_Animation.AnimType.DRIBBLE);
			if (passDelay > 0f)
			{
				moveSpeed *= 0.9f;
			}
			else
			{
				moveSpeed *= 0.75f;
			}
			CalcManager.mCalcVector3 = tempBall.transform.position - base.transform.position;
			CalcManager.mCalcVector3.y = base.transform.position.y;
			moveForce = CalcManager.mCalcVector3;
			if (IsCpu)
			{
				SingletonCustom<BeachSoccerPlayerManager>.Instance.CheckChangePlayer(this);
			}
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(rotateAnchor.gameObject);
		LeanTween.cancel(base.gameObject);
	}
}
