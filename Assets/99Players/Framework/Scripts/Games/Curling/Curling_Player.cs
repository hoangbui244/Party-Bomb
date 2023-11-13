using GamepadInput;
using System;
using UnityEngine;
public class Curling_Player : MonoBehaviour
{
	public enum ActionState
	{
		THROW,
		SWEEP_0,
		SWEEP_1,
		HOUSE_SWEEP,
		VIEWING,
		NONE
	}
	public enum BrushAnchor
	{
		STAND,
		THROW,
		SWEEP
	}
	public enum SweepCharaAnchor
	{
		SWEEP_0,
		SWEEP_1,
		HOUSE_SWEEP
	}
	[SerializeField]
	[Header("キャラクラス")]
	private Curling_Character character;
	[SerializeField]
	[Header("石を持つ手")]
	private Transform stoneHaveHand;
	[SerializeField]
	[Header("行動の状態に合わせたブラシのアンカ\u30fc")]
	private Transform[] arrayBrushAnchor;
	private int playerIdx;
	private Curling_GameManager.Team team;
	private Curling_Define.UserType userType;
	private int charaIdx;
	private int npadId;
	private ActionState actionState;
	private SweepCharaAnchor sweepCharaAnchor;
	private Curling_Stone stone;
	private bool isSweepMove;
	private bool isSweepStop;
	private Vector3 prevPos;
	private Vector3 nowPos;
	[SerializeField]
	[Header("走るアニメ\u30fcション速度")]
	private float runAnimationSpeed;
	private float runAnimationTime;
	[SerializeField]
	[Header("石を追いかける速度")]
	private float stoneChaseSpeed;
	[SerializeField]
	[Header("石の付近に近づく速度")]
	private float stoneNearChaseSpeed;
	[SerializeField]
	[Header("石から離れる速度")]
	private float stoneLeaveChaseSpeed;
	[SerializeField]
	[Header("石の付近に近づく速度（ハウス付近）")]
	private float stoneNearChaseSpeed_House;
	private bool isSweepAnimationOdd;
	private int houseSweepInputCnt;
	[SerializeField]
	[Header("投げるときの最大角度")]
	private float THROW_MAX_ANGLE;
	private Vector3 throw_CurveDir;
	private Vector3 throw_VelDir;
	private float throw_Power;
	private int throwDir;
	[SerializeField]
	[Header("投げる角度を変更時の速度")]
	private float THROW_ANGLE_ADD_SPEED;
	private float inputStickMag;
	private bool isOnceSweepMoveEnd;
	private Vector3 sweepMoveEndAnchorPos;
	private Quaternion sweepMoveEndAnchorRot;
	private Curling_AI cpuAI;
	private bool editor_isInputMouse;
	private Vector3 editor_inputMouseStartPos;
	public void Init(Curling_GameManager.Team _team, Curling_Define.UserType _userType, int _playerIdx)
	{
		team = _team;
		userType = _userType;
		playerIdx = _playerIdx;
		if (GetIsCpu())
		{
			cpuAI = base.gameObject.AddComponent<Curling_AI>();
			cpuAI.Init(this);
		}
	}
	public void SetCharaIdx(int _charaIdx)
	{
		charaIdx = _charaIdx;
	}
	public void SetBrushMaterial(Material _mat)
	{
		character.GetParts().SetBrushMaterial(_mat);
	}
	public void SetCharacterStyle()
	{
		character.SetStyle((int)team, charaIdx);
		character.SetReflection(charaIdx);
	}
	public void SetCharacterBibsStyle(int _bibsIdx)
	{
		UnityEngine.Debug.Log("name : " + base.gameObject.name + " bibsIdx " + _bibsIdx.ToString());
		character.SetBibsStyle((int)team, _bibsIdx);
	}
	public void InitPlay()
	{
		stone = null;
		isSweepStop = false;
		isSweepMove = false;
		actionState = ActionState.NONE;
		isSweepAnimationOdd = false;
		character.GetParts().GetBrushObj().SetActive(value: false);
		houseSweepInputCnt = 0;
		throwDir = ((UnityEngine.Random.Range(0, 2) != 0) ? 1 : (-1));
		throw_CurveDir = Vector3.zero;
		throw_VelDir = Vector3.zero;
		throw_Power = 0f;
		inputStickMag = 0f;
		isOnceSweepMoveEnd = false;
		if (GetIsCpu())
		{
			cpuAI.InitPlay();
		}
		editor_isInputMouse = false;
	}
	public void SetNpadId()
	{
		if (!GetIsCpu())
		{
			npadId = (int)((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? userType : Curling_Define.UserType.PLAYER_1);
		}
	}
	public void FixedUpdateMethod()
	{
		switch (SingletonCustom<Curling_GameManager>.Instance.GetState())
		{
		case Curling_GameManager.State.SWEEP:
			if (actionState == ActionState.SWEEP_0 || actionState == ActionState.SWEEP_1)
			{
				if (!isSweepStop)
				{
					SweepMove();
				}
				else
				{
					SweepMoveEnd();
				}
			}
			break;
		case Curling_GameManager.State.HOUSE_SWEEP:
		case Curling_GameManager.State.PLAY_END:
			if (isSweepStop && (actionState == ActionState.SWEEP_0 || actionState == ActionState.SWEEP_1))
			{
				SweepMoveEnd();
			}
			else if (!isSweepStop && actionState == ActionState.HOUSE_SWEEP)
			{
				SweepMove();
			}
			break;
		}
	}
	public void UpdateMethod()
	{
		switch (SingletonCustom<Curling_GameManager>.Instance.GetState())
		{
		case Curling_GameManager.State.THROW:
			break;
		case Curling_GameManager.State.PLAY_START:
			if (actionState == ActionState.THROW && GetIsCpu())
			{
				cpuAI.SetThrowInfo();
			}
			break;
		case Curling_GameManager.State.PREP_THROW:
			if (actionState == ActionState.THROW)
			{
				if (!GetIsCpu())
				{
					Throw();
				}
				else
				{
					cpuAI.Throw();
				}
			}
			break;
		case Curling_GameManager.State.SWEEP:
			if (!isSweepStop && (actionState == ActionState.SWEEP_0 || actionState == ActionState.SWEEP_1))
			{
				Sweep();
			}
			break;
		case Curling_GameManager.State.HOUSE_SWEEP:
			if (!isSweepStop && actionState == ActionState.HOUSE_SWEEP)
			{
				if (!GetIsCpu())
				{
					HouseSweep();
				}
				else
				{
					cpuAI.HouseSweep();
				}
			}
			break;
		}
	}
	private void Throw()
	{
		bool flag = false;
		bool flag2 = false;
		flag = (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.LeftTrigger) || SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.LeftShoulder));
		flag2 = (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.RightTrigger) || SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.RightShoulder));
		if (flag && !flag2)
		{
			SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowRot(Curling_UIManager.ArrowDir.LEFT_CURVE);
			throw_CurveDir = Vector3.left;
		}
		else if (!flag && flag2)
		{
			SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowRot(Curling_UIManager.ArrowDir.RIGHT_CURVE);
			throw_CurveDir = Vector3.right;
		}
		else
		{
			SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowRot(Curling_UIManager.ArrowDir.STRAIGHT);
			throw_CurveDir = Vector3.zero;
		}
		if (SingletonCustom<JoyConManager>.Instance.IsKeyboardInput(npadId))
		{
			if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow))
			{
				throw_Power += Time.deltaTime * 0.8f;
			}
			else
			{
				throw_Power -= Time.deltaTime * 0.3f;
			}
			if (throw_Power > 1f)
			{
				throw_Power = 1f;
			}
			if (throw_Power < 0f)
			{
				throw_Power = 0f;
			}
			if (throw_Power > 0f)
			{
				SingletonCustom<Curling_UIManager>.Instance.SetControlInfoBalloonActive(Curling_UIManager.ContorollerUIType.THROW);
				SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowActive(_isActive: true);
				SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowPower(throw_Power);
			}
			else
			{
				SingletonCustom<Curling_UIManager>.Instance.SetControlInfoBalloonActive(Curling_UIManager.ContorollerUIType.THROW_POWER);
				throw_Power = 0f;
				SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowActive(_isActive: false);
				SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowPower(throw_Power);
			}
			SetThrowVel(throw_Power);
			if (throw_Power > 0f && SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A))
			{
				SingletonCustom<Curling_GameManager>.Instance.ThrowStone(throw_CurveDir, throw_VelDir, throw_Power);
			}
		}
		else
		{
			Vector2 stickDir = GetStickDir();
			float num = Mathf.Atan2(stickDir.y, stickDir.x) * 57.29578f;
			float num2 = Mathf.Abs(num);
			if (10f < num2 && num2 < 170f && num <= 0f)
			{
				SingletonCustom<Curling_UIManager>.Instance.SetControlInfoBalloonActive(Curling_UIManager.ContorollerUIType.THROW);
				throw_Power = inputStickMag;
				SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowActive(_isActive: true);
				SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowPower(throw_Power);
			}
			else
			{
				SingletonCustom<Curling_UIManager>.Instance.SetControlInfoBalloonActive(Curling_UIManager.ContorollerUIType.THROW_POWER);
				throw_Power = 0f;
				SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowActive(_isActive: false);
				SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowPower(throw_Power);
			}
			SetThrowVel(throw_Power);
			if (10f < num2 && num2 < 170f && num <= 0f && SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A))
			{
				SingletonCustom<Curling_GameManager>.Instance.ThrowStone(throw_CurveDir, throw_VelDir, throw_Power);
			}
		}
	}
	public void SetThrowVel(float _throwPower)
	{
		base.transform.AddLocalEulerAnglesY((float)throwDir * Time.deltaTime * THROW_ANGLE_ADD_SPEED);
		if (base.transform.localEulerAngles.y < 180f && base.transform.localEulerAngles.y > THROW_MAX_ANGLE)
		{
			base.transform.SetLocalEulerAnglesY(THROW_MAX_ANGLE);
			throwDir *= -1;
		}
		else if (base.transform.localEulerAngles.y > 180f && base.transform.localEulerAngles.y < 360f - THROW_MAX_ANGLE)
		{
			base.transform.SetLocalEulerAnglesY(360f - THROW_MAX_ANGLE);
			throwDir *= -1;
		}
		throw_VelDir = base.transform.forward;
		SingletonCustom<Curling_UIManager>.Instance.SetThrowArrowAngle(0f - base.transform.localEulerAngles.y);
		SingletonCustom<Curling_GameManager>.Instance.SetPredictLineActive(_isActive: true);
		SingletonCustom<Curling_GameManager>.Instance.SetPredictLine(throw_VelDir, _throwPower);
	}
	private void SweepMove()
	{
		prevPos = nowPos;
		float y = base.transform.position.y;
		Vector3 position = stone.GetArraySweepCharaAnchor()[(int)sweepCharaAnchor].position;
		position.y = y;
		Quaternion rotation = stone.GetArraySweepCharaAnchor()[(int)sweepCharaAnchor].rotation;
		switch (actionState)
		{
		case ActionState.SWEEP_0:
		case ActionState.SWEEP_1:
			base.transform.position = Vector3.Lerp(base.transform.position, position, (isSweepMove ? stoneChaseSpeed : stoneNearChaseSpeed) * Time.fixedDeltaTime);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, rotation, (isSweepMove ? stoneChaseSpeed : stoneNearChaseSpeed) * Time.fixedDeltaTime);
			break;
		case ActionState.HOUSE_SWEEP:
			base.transform.position = Vector3.Lerp(base.transform.position, position, (isSweepMove ? stoneChaseSpeed : stoneNearChaseSpeed_House) * Time.fixedDeltaTime);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, rotation, (isSweepMove ? stoneChaseSpeed : stoneNearChaseSpeed_House) * Time.fixedDeltaTime);
			break;
		}
		nowPos = base.transform.position;
	}
	private void SweepMoveEnd()
	{
		if (!isOnceSweepMoveEnd)
		{
			isOnceSweepMoveEnd = true;
			switch (actionState)
			{
			case ActionState.SWEEP_0:
				sweepMoveEndAnchorPos = stone.GetArraySweepMoveEndAnchor()[0].position;
				sweepMoveEndAnchorRot = stone.GetArraySweepMoveEndAnchor()[0].rotation;
				break;
			case ActionState.SWEEP_1:
				sweepMoveEndAnchorPos = stone.GetArraySweepMoveEndAnchor()[1].position;
				sweepMoveEndAnchorRot = stone.GetArraySweepMoveEndAnchor()[1].rotation;
				break;
			}
		}
		prevPos = nowPos;
		float y = base.transform.position.y;
		Vector3 b = sweepMoveEndAnchorPos;
		b.y = y;
		Quaternion b2 = sweepMoveEndAnchorRot;
		base.transform.position = Vector3.Lerp(base.transform.position, b, stoneLeaveChaseSpeed * Time.fixedDeltaTime);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b2, stoneLeaveChaseSpeed * Time.fixedDeltaTime);
		nowPos = base.transform.position;
	}
	private void Sweep()
	{
		if (CalcManager.Length(nowPos, prevPos) > 0.01f)
		{
			runAnimationTime += CalcManager.Length(nowPos, prevPos) * runAnimationSpeed * Time.deltaTime;
			if (runAnimationTime >= 0.5f)
			{
				runAnimationTime = 0f;
				isSweepAnimationOdd = !isSweepAnimationOdd;
				character.SetSweepRunAnimation(0.5f, isSweepAnimationOdd);
			}
			character.SetSweepAnimation(0.25f, actionState);
		}
	}
	private void HouseSweep()
	{
		HouseSweepRunAnimation();
		if (SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A))
		{
			Vector2 stickDir = GetStickDir();
			stickDir.y = 0f;
			if (stickDir.x != 0f)
			{
				stickDir.x = Mathf.Sign(stickDir.x);
			}
			HouseSweepAnimation(stickDir);
		}
	}
	public void HouseSweepRunAnimation()
	{
		if (CalcManager.Length(nowPos, prevPos) > 0.01f)
		{
			runAnimationTime += CalcManager.Length(nowPos, prevPos) * runAnimationSpeed * Time.deltaTime;
			if (runAnimationTime >= 0.5f)
			{
				runAnimationTime = 0f;
				isSweepAnimationOdd = !isSweepAnimationOdd;
				character.SetSweepRunAnimation(0.5f, isSweepAnimationOdd, actionState);
			}
		}
	}
	public void HouseSweepAnimation(Vector2 _vec)
	{
		if (_vec.x == 0f)
		{
			houseSweepInputCnt++;
		}
		stone.SetRigidDrag(_vec, houseSweepInputCnt);
		character.SetSweepAnimation(0.25f, actionState);
	}
	public void StopSweep()
	{
		isSweepStop = true;
	}
	public void SetThrowState()
	{
		actionState = ActionState.THROW;
		base.transform.position = SingletonCustom<Curling_GameManager>.Instance.GetPlayTeamArrayCharaAnchor((int)actionState).position;
		base.transform.rotation = SingletonCustom<Curling_GameManager>.Instance.GetPlayTeamArrayCharaAnchor((int)actionState).rotation;
		SingletonCustom<Curling_CharacterManager>.Instance.SetMotion(Curling_CharacterManager.MotionType.Throw, character.GetParts());
		if (character.GetParts().GetIsReverse())
		{
			character.GetParts().SetIsReverse(_isReverse: false);
			base.transform.SetLocalScaleX(0f - character.GetParts().transform.localScale.x);
			character.GetParts().GetBibsObj().transform.SetLocalScaleX(0f - character.GetParts().GetBibsObj().transform.localScale.x);
		}
		SingletonCustom<Curling_CharacterManager>.Instance.SetOptionAnchor(Curling_CharacterManager.MotionType.Throw, character.GetParts());
		character.GetParts().GetBrushObj().SetActive(value: true);
	}
	public void SetSweepState(int _anchorIdx)
	{
		switch (_anchorIdx)
		{
		case 0:
			actionState = ActionState.SWEEP_0;
			sweepCharaAnchor = SweepCharaAnchor.SWEEP_0;
			break;
		case 1:
			actionState = ActionState.SWEEP_1;
			sweepCharaAnchor = SweepCharaAnchor.SWEEP_1;
			break;
		}
		base.transform.position = SingletonCustom<Curling_GameManager>.Instance.GetPlayTeamArrayCharaAnchor((int)actionState).position;
		base.transform.rotation = SingletonCustom<Curling_GameManager>.Instance.GetPlayTeamArrayCharaAnchor((int)actionState).rotation;
		SingletonCustom<Curling_CharacterManager>.Instance.SetMotion(Curling_CharacterManager.MotionType.Stand, character.GetParts());
		if (actionState == ActionState.SWEEP_1 && !character.GetParts().GetIsReverse())
		{
			character.GetParts().SetIsReverse(_isReverse: true);
			base.transform.SetLocalScaleX(0f - character.GetParts().transform.localScale.x);
			character.GetParts().GetBibsObj().transform.SetLocalScaleX(0f - character.GetParts().GetBibsObj().transform.localScale.x);
		}
		SingletonCustom<Curling_CharacterManager>.Instance.SetOptionAnchor(Curling_CharacterManager.MotionType.Stand, character.GetParts());
		character.GetParts().GetBrushObj().SetActive(value: true);
	}
	public void SetHouseSweepState()
	{
		actionState = ActionState.HOUSE_SWEEP;
		sweepCharaAnchor = SweepCharaAnchor.HOUSE_SWEEP;
		base.transform.position = SingletonCustom<Curling_GameManager>.Instance.GetPlayTeamArrayCharaAnchor((int)actionState).position;
		base.transform.rotation = SingletonCustom<Curling_GameManager>.Instance.GetPlayTeamArrayCharaAnchor((int)actionState).rotation;
		SingletonCustom<Curling_CharacterManager>.Instance.SetMotion(Curling_CharacterManager.MotionType.Stand, character.GetParts());
		if (!character.GetParts().GetIsReverse())
		{
			character.GetParts().SetIsReverse(_isReverse: true);
			base.transform.SetLocalScaleX(0f - character.GetParts().transform.localScale.x);
			character.GetParts().GetBibsObj().transform.SetLocalScaleX(0f - character.GetParts().GetBibsObj().transform.localScale.x);
		}
		SingletonCustom<Curling_CharacterManager>.Instance.SetOptionAnchor(Curling_CharacterManager.MotionType.Stand, character.GetParts());
		character.GetParts().GetBrushObj().SetActive(value: true);
	}
	public void ChangeSweep()
	{
		SingletonCustom<Curling_CharacterManager>.Instance.SetMotion(Curling_CharacterManager.MotionType.Sweep, character.GetParts());
		character.SetBodyOriginAndSweepPos();
		SingletonCustom<Curling_CharacterManager>.Instance.SetOptionAnchor(Curling_CharacterManager.MotionType.Sweep, character.GetParts());
		LeanTween.delayedCall(base.gameObject, SingletonCustom<Curling_GameManager>.Instance.GetCharaChangeSweepTime() / 2f, (Action)delegate
		{
			if (actionState == ActionState.HOUSE_SWEEP)
			{
				SingletonCustom<Curling_UIManager>.Instance.SetOperationUI(Curling_UIManager.OperationUIType.SWEEP);
				if (!GetIsCpu())
				{
					SingletonCustom<Curling_UIManager>.Instance.SetControlInfoBalloonActive(Curling_UIManager.ContorollerUIType.SWEEP);
					SingletonCustom<Curling_UIManager>.Instance.SetPlayerIconActive(1, _isActive: true);
				}
			}
		});
		LeanTween.delayedCall(base.gameObject, SingletonCustom<Curling_GameManager>.Instance.GetCharaChangeSweepTime(), (Action)delegate
		{
			isSweepMove = true;
		});
	}
	public void SetViewingState(int _anchorIdx)
	{
		actionState = ActionState.VIEWING;
		base.transform.position = SingletonCustom<Curling_GameManager>.Instance.GetViewingTeamArrayCharaAnchor(team)[_anchorIdx].position;
		base.transform.rotation = SingletonCustom<Curling_GameManager>.Instance.GetViewingTeamArrayCharaAnchor(team)[_anchorIdx].rotation;
		character.ResetAnimation(0f);
		if (character.GetParts().GetIsReverse())
		{
			character.GetParts().SetIsReverse(_isReverse: false);
			base.transform.SetLocalScaleX(0f - character.GetParts().transform.localScale.x);
			character.GetParts().GetBibsObj().transform.SetLocalScaleX(0f - character.GetParts().GetBibsObj().transform.localScale.x);
		}
		character.GetParts().GetBrushObj().SetActive(value: false);
	}
	public void SetStone(Curling_Stone _stone)
	{
		stone = _stone;
	}
	public Curling_Stone GetStone()
	{
		return stone;
	}
	public Curling_GameManager.Team GetTeam()
	{
		return team;
	}
	public Curling_Define.UserType GetUserType()
	{
		return userType;
	}
	public int GetNpadId()
	{
		return npadId;
	}
	public int GetPlayerIdx()
	{
		return playerIdx;
	}
	public bool GetIsCpu()
	{
		return userType >= Curling_Define.UserType.CPU_1;
	}
	public ActionState GetActionState()
	{
		return actionState;
	}
	public Transform GetStoneHaveHand()
	{
		return stoneHaveHand;
	}
	public float GetThrowMaxAngle()
	{
		return THROW_MAX_ANGLE;
	}
	public float GetThrowAngleAddSpeed()
	{
		return THROW_ANGLE_ADD_SPEED;
	}
	public void SetTargetPos(Vector3 _targetPos)
	{
		cpuAI.SetTargetPos(_targetPos);
	}
	private Vector2 GetStickDir()
	{
		float num = 0f;
		float num2 = 0f;
		Vector2 vector = CalcManager.mVector3Zero;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(npadId);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		inputStickMag = axisInput.Stick_L.magnitude;
		vector = new Vector2(num, num2);
		if (vector.sqrMagnitude < 0.0400000028f)
		{
			return Vector2.zero;
		}
		return vector.normalized;
	}
	public void SkipThrowStone()
	{
		cpuAI.SkipThrowStone();
	}
	public void LeanTweenCancel()
	{
		LeanTween.cancel(base.gameObject);
		character.LeanTweenCancel();
	}
}
