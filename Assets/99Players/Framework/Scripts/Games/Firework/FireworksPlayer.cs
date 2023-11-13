using GamepadInput;
using System;
using System.Collections;
using UnityEngine;
public class FireworksPlayer : MonoBehaviour
{
	public enum State
	{
		DEFAULT,
		LIFT,
		SET,
		PUT_BACK
	}
	[SerializeField]
	[Header("スタイル")]
	private CharacterStyle style;
	[SerializeField]
	[Header("リジッドボディ")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("アニメ\u30fcション")]
	private FireworksPlayer_Animation anim;
	[SerializeField]
	[Header("花火玉ル\u30fcト")]
	private Transform ballRoot;
	[SerializeField]
	[Header("花火玉")]
	private FireworksBall[] arrayFireworksBall;
	[SerializeField]
	[Header("AI")]
	private BigMerchantPlayerAI cpu;
	private readonly float LOOK_SPEED = 11f;
	private float MOVE_SPEED_MAX = 2.25f;
	private readonly float ATTENUATION_SCALE = 0.89f;
	private readonly float MOVE_SPEED_SCALE = 20f;
	public static int HAVE_ITEM_MAX = 3;
	private Vector3 moveForce;
	private FireworksDefine.UserType userType;
	private int score;
	private FireworksSlatbox slatbox;
	private BigMerchantCustomer customer;
	private int haveBallCount;
	private int idx;
	private Quaternion tempRot;
	private float moveSpeed;
	private Vector3 rotVec;
	private float waitTime;
	private Transform ballRootParent;
	private State currentState;
	private Vector3 prevDir;
	public bool IsCpu
	{
		get;
		set;
	}
	public int ArrayIdx => idx;
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
	public int HaveBallCount => haveBallCount;
	public CharacterStyle Style => style;
	public FireworksBall[] ArrayBall => arrayFireworksBall;
	public State CurrentState => currentState;
	public FireworksDefine.UserType UserType => userType;
	public void Init(int _idx, FireworksDefine.UserType _userType, FireworksDefine.TeamType _teamType)
	{
		idx = _idx;
		userType = _userType;
		IsCpu = ((int)userType >= SingletonCustom<GameSettingManager>.Instance.PlayerNum);
		style.SetGameStyle(GS_Define.GameType.ARCHER_BATTLE, (int)userType);
		if (ballRootParent == null)
		{
			ballRootParent = ballRoot.parent;
		}
		cpu.Init(this);
	}
	public void NextGame(Vector3 _pos)
	{
		base.transform.position = _pos;
		SetState(State.DEFAULT);
		anim.SetAnim(FireworksPlayer_Animation.AnimType.STANDBY);
		for (int i = 0; i < arrayFireworksBall.Length; i++)
		{
			arrayFireworksBall[i].gameObject.SetActive(value: false);
		}
		haveBallCount = 0;
		ballRoot.parent = ballRootParent;
		ballRoot.SetLocalPosition(0f, 0f, 0f);
		ballRoot.SetLocalEulerAngles(0f, 0f, 0f);
		ballRoot.SetLocalPositionX((haveBallCount == 2) ? (-0.1f) : 0f);
		rigid.velocity = Vector3.zero;
		rigid.angularVelocity = Vector3.zero;
		rigid.isKinematic = false;
		moveForce = Vector3.zero;
		base.transform.SetLocalEulerAnglesY(180f);
		score = 0;
	}
	public void UpdateMethod()
	{
		if (IsCpu)
		{
			UpdateMethodCpu();
			return;
		}
		switch (currentState)
		{
		case State.DEFAULT:
			UpdateMoveForce();
			cpu.UpdateManualAgent(base.transform.position);
			if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId((int)userType), SatGamePad.Button.A))
			{
				CheckAction();
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(SingletonCustom<JoyConManager>.Instance.GetCurrentNpadId((int)userType), SatGamePad.Button.B))
			{
				CheckPutBack();
			}
			break;
		case State.LIFT:
			waitTime -= Time.deltaTime;
			if (waitTime <= 0f)
			{
				anim.SetAnim(FireworksPlayer_Animation.AnimType.CARRY);
				SetState(State.DEFAULT);
			}
			break;
		case State.PUT_BACK:
			waitTime -= Time.deltaTime;
			if (waitTime <= 0f)
			{
				if (haveBallCount > 0)
				{
					anim.SetAnim(FireworksPlayer_Animation.AnimType.CARRY);
				}
				else
				{
					anim.SetAnim(FireworksPlayer_Animation.AnimType.STANDBY);
				}
				SetState(State.DEFAULT);
			}
			break;
		case State.SET:
			waitTime -= Time.deltaTime;
			if (waitTime <= 0f)
			{
				if (haveBallCount > 0)
				{
					anim.SetAnim(FireworksPlayer_Animation.AnimType.CARRY);
				}
				else
				{
					anim.SetAnim(FireworksPlayer_Animation.AnimType.STANDBY);
				}
				ballRoot.parent = ballRootParent;
				ballRoot.SetLocalPosition(0f, 0f, 0f);
				ballRoot.SetLocalEulerAngles(0f, 0f, 0f);
				ballRoot.SetLocalPositionX((haveBallCount == 2) ? (-0.1f) : 0f);
				rigid.isKinematic = false;
				SetState(State.DEFAULT);
				if (IsCpu)
				{
					cpu.SetEnd();
				}
			}
			break;
		}
	}
	public void CheckAction()
	{
		if (slatbox != null)
		{
			Lift();
		}
		if (customer != null && customer.IsPassItem)
		{
			StartCoroutine(_Set());
		}
	}
	public void CheckPutBack()
	{
		if (slatbox != null)
		{
			PutBack();
		}
	}
	private IEnumerator _Set()
	{
		Set(FireworksDefine.SCORE_SET);
		yield return new WaitForSeconds(0.15f);
		if (haveBallCount > 0)
		{
			Set(FireworksDefine.SCORE_SET_DOUBLE);
			yield return new WaitForSeconds(0.15f);
			if (haveBallCount > 0)
			{
				Set(FireworksDefine.SCORE_SET_TRIPLE);
			}
		}
	}
	public void Lift()
	{
		if ((!IsCpu || cpu.IsLift()) && haveBallCount < HAVE_ITEM_MAX)
		{
			Vector3 vector = slatbox.transform.position - base.transform.position;
			rotVec.x = vector.x;
			rotVec.z = vector.z;
			tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, 0f, 0f) * rotVec), 1f);
			if (tempRot != Quaternion.identity)
			{
				base.transform.rotation = tempRot;
				rigid.MoveRotation(tempRot);
			}
			anim.SetAnim(FireworksPlayer_Animation.AnimType.LIFT, _isRestart: true);
			arrayFireworksBall[haveBallCount].gameObject.SetActive(value: true);
			arrayFireworksBall[haveBallCount].Init(slatbox.ColorType);
			haveBallCount++;
			rigid.velocity = Vector3.zero;
			rigid.angularVelocity = Vector3.zero;
			moveSpeed = 0f;
			moveForce = Vector3.zero;
			ballRoot.SetLocalPositionX((haveBallCount == 2) ? (-0.1f) : 0f);
			waitTime = 0.15f;
			slatbox.Shake();
			SingletonCustom<AudioManager>.Instance.SePlay("se_item_lift");
			if (IsCpu)
			{
				cpu.Lift();
			}
			else
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
			}
			SetState(State.LIFT);
		}
	}
	public void PutBack()
	{
		if ((IsCpu && !cpu.IsReturn()) || haveBallCount <= 0)
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < haveBallCount)
			{
				if (arrayFireworksBall[num].Color == slatbox.ColorType)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		for (int i = num; i < haveBallCount - 1; i++)
		{
			arrayFireworksBall[i].Init(arrayFireworksBall[i + 1].Color);
		}
		arrayFireworksBall[haveBallCount - 1].gameObject.SetActive(value: false);
		anim.SetAnim(FireworksPlayer_Animation.AnimType.SET, _isRestart: true);
		SingletonCustom<AudioManager>.Instance.SePlay("se_item_lift");
		haveBallCount--;
		ballRoot.SetLocalPositionX((haveBallCount == 2) ? (-0.1f) : 0f);
		waitTime = 0.15f;
		slatbox.Shake();
		SetState(State.PUT_BACK);
	}
	public void Set(int _baseScore)
	{
		if (customer == null || haveBallCount <= 0)
		{
			return;
		}
		for (int i = 0; i < haveBallCount; i++)
		{
			for (int j = 0; j < customer.ListColorType.Count; j++)
			{
				if (arrayFireworksBall[i].Color == customer.ListColorType[j])
				{
					ballRoot.parent = base.transform.parent;
					Vector3 vector = customer.CounterPos - base.transform.position;
					rotVec.x = vector.x;
					rotVec.z = vector.z;
					tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, 0f, 0f) * rotVec), 1f);
					if (tempRot != Quaternion.identity)
					{
						base.transform.rotation = tempRot;
						rigid.MoveRotation(tempRot);
					}
					anim.SetAnim(FireworksPlayer_Animation.AnimType.SET);
					FireworksBall fireworksBall = UnityEngine.Object.Instantiate(arrayFireworksBall[i], base.transform.parent);
					fireworksBall.Init(customer.ListColorType[j]);
					fireworksBall.transform.position = arrayFireworksBall[i].transform.position;
					fireworksBall.transform.rotation = arrayFireworksBall[i].transform.rotation;
					fireworksBall.transform.SetLocalScale(1f, 1f, 1f);
					fireworksBall.Set();
					StartCoroutine(_Set(fireworksBall.gameObject, customer.CounterPos));
					if (customer.ListColorType.Count == 1)
					{
						_baseScore += FireworksDefine.SCORE_SET_LAST;
					}
					customer.Set(idx, arrayFireworksBall[i].Color, _baseScore);
					for (int k = i; k < haveBallCount - 1; k++)
					{
						arrayFireworksBall[k].Init(arrayFireworksBall[k + 1].Color);
					}
					arrayFireworksBall[haveBallCount - 1].gameObject.SetActive(value: false);
					haveBallCount--;
					waitTime = 0.75f;
					rigid.velocity = Vector3.zero;
					rigid.angularVelocity = Vector3.zero;
					moveSpeed = 0f;
					moveForce = Vector3.zero;
					rigid.isKinematic = true;
					if (IsCpu)
					{
						cpu.Set();
					}
					else
					{
						SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
					}
					SetState(State.SET);
					return;
				}
			}
		}
	}
	private IEnumerator _Set(GameObject _ball, Vector3 _pos)
	{
		LeanTween.moveX(_ball, _pos.x, 0.2f);
		LeanTween.moveY(_ball, _pos.y + 0.6f, 0.15f).setOnComplete((Action)delegate
		{
			LeanTween.moveY(_ball, _pos.y, 0.15f);
		}).setEaseOutBack();
		LeanTween.moveZ(_ball, _pos.z, 0.2f);
		yield return new WaitForEndOfFrame();
	}
	private bool IsTripleBall()
	{
		if (haveBallCount < arrayFireworksBall.Length)
		{
			return false;
		}
		FireworksBall.ItemType color = arrayFireworksBall[0].Color;
		for (int i = 0; i < arrayFireworksBall.Length; i++)
		{
			if (arrayFireworksBall[i].Color != color)
			{
				return false;
			}
		}
		return true;
	}
	public void UpdateMethodCpu()
	{
		switch (currentState)
		{
		case State.DEFAULT:
			cpu.DoThink();
			if (!cpu.IsPathPending())
			{
				CalcManager.mCalcVector3 = cpu.UpdateMoveForce();
				if (CalcManager.mCalcVector3.magnitude < 0.0400000028f)
				{
					moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
					switch (anim.CurrentAnimType)
					{
					case FireworksPlayer_Animation.AnimType.DASH:
						anim.SetAnimSpeed(moveSpeed * 1.25f);
						if (moveSpeed <= 0.5f)
						{
							anim.SetAnim(FireworksPlayer_Animation.AnimType.STANDBY);
						}
						break;
					case FireworksPlayer_Animation.AnimType.CARRY:
						anim.SetAnimSpeed(moveSpeed * 1.25f);
						break;
					}
				}
				else
				{
					switch (anim.CurrentAnimType)
					{
					case FireworksPlayer_Animation.AnimType.STANDBY:
						anim.SetAnim(FireworksPlayer_Animation.AnimType.DASH);
						anim.SetAnimSpeed(moveSpeed * 1.25f);
						break;
					case FireworksPlayer_Animation.AnimType.DASH:
						anim.SetAnimSpeed(moveSpeed * 1.25f);
						break;
					case FireworksPlayer_Animation.AnimType.CARRY:
						anim.SetAnimSpeed(moveSpeed * 1.25f);
						break;
					}
					if (moveSpeed < MOVE_SPEED_MAX)
					{
						moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_SCALE, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
					}
					moveForce = Vector3.Slerp(moveForce, CalcManager.mCalcVector3.normalized, 0.45f);
				}
			}
			CheckAction();
			CheckPutBack();
			break;
		case State.LIFT:
			waitTime -= Time.deltaTime;
			if (waitTime <= 0f)
			{
				anim.SetAnim(FireworksPlayer_Animation.AnimType.CARRY);
				SetState(State.DEFAULT);
				CheckAction();
			}
			break;
		case State.PUT_BACK:
			waitTime -= Time.deltaTime;
			if (waitTime <= 0f)
			{
				if (haveBallCount > 0)
				{
					anim.SetAnim(FireworksPlayer_Animation.AnimType.CARRY);
				}
				else
				{
					anim.SetAnim(FireworksPlayer_Animation.AnimType.STANDBY);
				}
				SetState(State.DEFAULT);
			}
			break;
		case State.SET:
			waitTime -= Time.deltaTime;
			if (waitTime <= 0f)
			{
				if (haveBallCount > 0)
				{
					anim.SetAnim(FireworksPlayer_Animation.AnimType.CARRY);
				}
				else
				{
					anim.SetAnim(FireworksPlayer_Animation.AnimType.STANDBY);
				}
				ballRoot.parent = ballRootParent;
				ballRoot.SetLocalPosition(0f, 0f, 0f);
				ballRoot.SetLocalEulerAngles(0f, 0f, 0f);
				ballRoot.SetLocalPositionX((haveBallCount == 2) ? (-0.1f) : 0f);
				rigid.isKinematic = false;
				SetState(State.DEFAULT);
			}
			break;
		}
	}
	private void UpdateMoveForce()
	{
		if (IsCpu)
		{
			return;
		}
		CalcManager.mCalcVector2 = FireworksControllerManager.GetStickDir((int)userType);
		if (CalcManager.mCalcVector2.magnitude < 0.0400000028f)
		{
			moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
			switch (anim.CurrentAnimType)
			{
			case FireworksPlayer_Animation.AnimType.DASH:
				anim.SetAnimSpeed(moveSpeed * 1.25f);
				if (moveSpeed <= 0.5f)
				{
					anim.SetAnim(FireworksPlayer_Animation.AnimType.STANDBY);
				}
				break;
			case FireworksPlayer_Animation.AnimType.CARRY:
				anim.SetAnimSpeed(moveSpeed * 1.25f);
				break;
			}
		}
		else
		{
			switch (anim.CurrentAnimType)
			{
			case FireworksPlayer_Animation.AnimType.STANDBY:
				anim.SetAnim(FireworksPlayer_Animation.AnimType.DASH);
				anim.SetAnimSpeed(moveSpeed * 1.25f);
				break;
			case FireworksPlayer_Animation.AnimType.DASH:
				anim.SetAnimSpeed(moveSpeed * 1.25f);
				break;
			case FireworksPlayer_Animation.AnimType.CARRY:
				anim.SetAnimSpeed(moveSpeed * 1.25f);
				break;
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
	public void FixedUpdate()
	{
		FireworksGameManager.State state = SingletonCustom<FireworksGameManager>.Instance.CurrentState;
		if ((uint)(state - 2) <= 1u)
		{
			rigid.velocity *= 0.88f;
			rigid.angularVelocity *= 0.88f;
			moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
			anim.SetAnimSpeed(moveSpeed * 1.25f);
			return;
		}
		if (currentState == State.DEFAULT)
		{
			rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
			rigid.MovePosition(rigid.transform.position + new Vector3((moveForce * moveSpeed).x, rigid.velocity.y, (moveForce * moveSpeed).z) * Time.deltaTime);
		}
		if (currentState == State.DEFAULT && moveForce.magnitude >= 0.01f)
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
	}
	public void AddScore(int _score)
	{
		score = Mathf.Clamp(score + _score, 0, FireworksDefine.SCORE_MAX);
	}
	public void SetState(State _state)
	{
		if (currentState != _state)
		{
			currentState = _state;
		}
	}
	private void OnTriggerStay(Collider collision)
	{
		if (slatbox == null)
		{
			slatbox = collision.gameObject.GetComponent<FireworksSlatbox>();
		}
		if (customer == null)
		{
			customer = collision.gameObject.GetComponent<BigMerchantCustomer>();
		}
	}
	private void OnTriggerExit(Collider collision)
	{
		if (collision.gameObject.GetComponent<FireworksSlatbox>() != null)
		{
			slatbox = null;
		}
		if (collision.gameObject.GetComponent<BigMerchantCustomer>() != null)
		{
			customer = null;
		}
	}
}
