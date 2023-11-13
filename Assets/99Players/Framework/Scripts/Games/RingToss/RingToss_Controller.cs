using GamepadInput;
using System;
using UnityEngine;
public class RingToss_Controller : MonoBehaviour
{
	private int ctrlNo;
	private int playerNo;
	[SerializeField]
	private Transform ringAnchor;
	[SerializeField]
	private Transform aimPointAnchor;
	[SerializeField]
	private Transform futureLineAnchor;
	[SerializeField]
	private MeshRenderer[] futureLineRenderers;
	[SerializeField]
	private Transform playerUiAnchor;
	private RingToss_Ring ring;
	private Vector3 throwVec;
	private float throwTime;
	private bool isThrow;
	private bool isRingSetting;
	private float chargePower;
	private bool isCharge;
	private int chargeDir = 1;
	private Vector3 defaultAimPointPos;
	private Vector3 defaultControllerPos;
	private int remainingRingNum;
	private RingToss_Target aiTarget;
	private Vector3 aiTargetRandomPos;
	private float aiAimMaxRandomDis;
	private float aiAimMinRandomDis;
	private float aiLRMoveSpeedMag;
	private float aiThrowDelayTime;
	private bool isAiMoveTween;
	private bool isAiCanThrow;
	public bool IsThrow => isThrow;
	public bool IsRingEnd => remainingRingNum == 0;
	public bool IsPlayer => playerNo < 4;
	public int RemainingRingNum => remainingRingNum;
	public int CtrlNo => ctrlNo;
	public int PlayerNo => playerNo;
	public void Init(int _ctrlNo)
	{
		ctrlNo = _ctrlNo;
		playerNo = SingletonCustom<RingToss_GameManager>.Instance.GetPlayerNo(ctrlNo);
		InitRingSetting();
		SetFutureLineActive(_active: false);
		aimPointAnchor.SetPositionY(SingletonCustom<RingToss_RingManager>.Instance.GetRingTargetBottomPosY());
		defaultAimPointPos = aimPointAnchor.position;
		defaultControllerPos = base.transform.position;
		remainingRingNum = SingletonCustom<RingToss_ControllerManager>.Instance.RemainingRingNum;
		AiInit();
	}
	public void SecondGroupInit()
	{
		playerNo = SingletonCustom<RingToss_GameManager>.Instance.GetPlayerNo(ctrlNo);
		InitRingSetting();
		SetFutureLineActive(_active: false);
		aimPointAnchor.SetPositionZ(defaultAimPointPos.z);
		base.transform.position = defaultControllerPos;
		remainingRingNum = SingletonCustom<RingToss_ControllerManager>.Instance.RemainingRingNum;
		isThrow = false;
		isRingSetting = false;
		AiInit();
	}
	public void UpdateMethod()
	{
		if (IsPlayer)
		{
			CalcThrowVec();
		}
		FutureLineUpdate();
	}
	private bool GetThrowFlag()
	{
		if (isThrow)
		{
			return false;
		}
		if (IsPlayer)
		{
			return GetThrowButtonDown();
		}
		return CheckCanAiThrow();
	}
	public Vector3 GetPlayerUiPos()
	{
		return playerUiAnchor.position;
	}
	public void PlayerControl()
	{
		if (!isThrow && !isRingSetting)
		{
			Vector2 stickDir = GetStickDir();
			GetSpeedUpButton();
			Move(stickDir);
			if (GetThrowFlag())
			{
				Throw();
			}
		}
	}
	private void Move(Vector2 _stickDir)
	{
		Vector3 position = base.transform.position;
		if (!isCharge)
		{
			position.x += _stickDir.x * 8f * Time.deltaTime;
			base.transform.position = SingletonCustom<RingToss_ControllerManager>.Instance.ClampRingPosition(position);
		}
		position = aimPointAnchor.position;
		position.z += _stickDir.y * 8f * Time.deltaTime;
		aimPointAnchor.position = SingletonCustom<RingToss_ControllerManager>.Instance.ClampAimPosition(position);
	}
	private void Charge()
	{
		if (GetThrowButtonDown())
		{
			isCharge = true;
		}
		if (isCharge)
		{
			chargePower += (float)chargeDir * 0.8f * Time.deltaTime;
			if (chargePower > 1f)
			{
				chargePower = 2f - chargePower;
				chargeDir = -1;
			}
			else if (chargePower < 0f)
			{
				chargePower = 0f - chargePower;
				chargeDir = 1;
			}
		}
	}
	private void Throw()
	{
		if (remainingRingNum != 0)
		{
			if (IsPlayer)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_ringtoss_throw");
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerNo);
			}
			isThrow = true;
			ring.transform.parent = SingletonCustom<RingToss_RingManager>.Instance.GetRingThrowedAnchor();
			ring.Throw(throwVec, delegate
			{
				ThrowEnd();
			});
			isCharge = false;
			chargePower = 0f;
			chargeDir = 1;
			SetFutureLineActive(_active: false);
			remainingRingNum--;
			if (remainingRingNum == 0)
			{
				SingletonCustom<RingToss_UIManager>.Instance.ViewRingEnd(ctrlNo);
			}
			AiThrow();
		}
	}
	private void ThrowEnd()
	{
		isThrow = false;
		if (remainingRingNum > 0)
		{
			NextRingSetting();
		}
	}
	private void InitRingSetting()
	{
		ring = SingletonCustom<RingToss_RingManager>.Instance.GetRing(ctrlNo, 0);
		ring.transform.parent = ringAnchor;
		ring.transform.position = ringAnchor.position;
		ring.transform.rotation = Quaternion.identity;
	}
	private void NextRingSetting()
	{
		isRingSetting = true;
		ring = SingletonCustom<RingToss_RingManager>.Instance.GetNextRing(ctrlNo);
		aimPointAnchor.SetPositionZ(defaultAimPointPos.z);
		base.transform.position = defaultControllerPos;
		ring.transform.parent = ringAnchor;
		LeanTween.move(ring.gameObject, ringAnchor.position, 0.5f).setEaseOutQuad().setOnComplete((Action)delegate
		{
			isRingSetting = false;
			SetFutureLineActive(_active: true);
		});
	}
	public void CalcThrowVec()
	{
		float num = aimPointAnchor.position.z - futureLineAnchor.position.z;
		throwTime = num / 30f;
		throwVec = CalcManager.GetVelocityPositionVec(futureLineAnchor.position, aimPointAnchor.position, throwTime, -98.1f);
	}
	public void SetFutureLineActive(bool _active)
	{
		if (!IsPlayer)
		{
			_active = false;
		}
		futureLineAnchor.gameObject.SetActive(_active);
	}
	public void FutureLineUpdate()
	{
		if (!futureLineAnchor.gameObject.activeSelf)
		{
			return;
		}
		float num = 1f;
		float num2 = throwTime / (float)futureLineRenderers.Length;
		float num3 = 0.099f;
		num3 = 0.15f;
		Color color = futureLineRenderers[0].material.GetColor("_Color");
		for (int i = 0; i < futureLineRenderers.Length; i++)
		{
			if (num <= 0f)
			{
				futureLineRenderers[i].gameObject.SetActive(value: false);
				continue;
			}
			futureLineRenderers[i].gameObject.SetActive(value: true);
			futureLineRenderers[i].transform.position = CalcManager.GetVelocityTimeToPosition(throwVec, futureLineAnchor.position, num2 * (float)(i + 1), -98.1f);
			color.a = num;
			futureLineRenderers[i].material.SetColor("_Color", color);
			num -= num3;
		}
	}
	private bool GetThrowButtonDown()
	{
		int playerIdx;
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			if (!IsPlayer)
			{
				return false;
			}
			playerIdx = 0;
			return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
		}
		playerIdx = SingletonCustom<GameSettingManager>.Instance.GetAllocNpadId(playerNo);
		return SingletonCustom<JoyConManager>.Instance.GetButtonDown(playerIdx, SatGamePad.Button.A);
	}
	private bool GetThrowButtonUp()
	{
		int playerIdx;
		if (SingletonCustom<JoyConManager>.Instance.IsSingleMode())
		{
			if (!IsPlayer)
			{
				return false;
			}
			playerIdx = 0;
			return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.A);
		}
		playerIdx = SingletonCustom<GameSettingManager>.Instance.GetAllocNpadId(playerNo);
		return SingletonCustom<JoyConManager>.Instance.GetButtonUp(playerIdx, SatGamePad.Button.A);
	}
	private Vector2 GetStickDir()
	{
		float num = 0f;
		float num2 = 0f;
		Vector2 vector = CalcManager.mVector3Zero;
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? playerNo : 0;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		if (true && Mathf.Abs(num) < 0.2f && Mathf.Abs(num2) < 0.2f)
		{
			num = 0f;
			num2 = 0f;
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Right))
			{
				num = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Left))
			{
				num = -1f;
			}
			if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Up))
			{
				num2 = 1f;
			}
			else if (SingletonCustom<JoyConManager>.Instance.GetButton(playerIdx, SatGamePad.Button.Dpad_Down))
			{
				num2 = -1f;
			}
		}
		vector = new Vector2(num, num2);
		if (vector.sqrMagnitude < 0.0400000028f)
		{
			return Vector2.zero;
		}
		return vector.normalized;
	}
	private bool GetSpeedUpButton()
	{
		return false;
	}
	public void AiInit()
	{
		aiTarget = null;
		isAiMoveTween = false;
		isAiCanThrow = false;
		LeanTween.cancel(base.gameObject);
		int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		aiAimMaxRandomDis = RingToss_Define.AI_AIM_MAX_RANDOM_DISTANCES[aiStrength];
		aiAimMinRandomDis = RingToss_Define.AI_AIM_MIN_RANDOM_DISTANCES[aiStrength];
		aiLRMoveSpeedMag = RingToss_Define.AI_LR_MOVE_SPEED_MAGS[aiStrength];
		aiThrowDelayTime = RingToss_Define.AI_THROW_DELAY_TIMES[aiStrength];
	}
	public void AiUpdate()
	{
		if (!isAiMoveTween)
		{
			AiTargetCheck();
		}
		AiMove();
		if (GetThrowFlag())
		{
			Throw();
		}
	}
	private void AiMove()
	{
		if (!IsThrow && !isRingSetting && !isAiMoveTween)
		{
			aimPointAnchor.SetPositionZ(futureLineAnchor.position.z);
			float time = Mathf.Abs((aiTargetRandomPos - aimPointAnchor.position).x) / (8f * aiLRMoveSpeedMag);
			LeanTween.moveX(base.gameObject, aiTargetRandomPos.x, time).setOnUpdate((Action<float>)delegate
			{
				if (SingletonCustom<RingToss_GameManager>.Instance.IsGameEnd)
				{
					LeanTween.cancel(base.gameObject);
				}
			}).setOnComplete((Action)delegate
			{
				if (aiTarget.IsGet)
				{
					AiResetData();
				}
				else
				{
					LeanTween.delayedCall(base.gameObject, aiThrowDelayTime, (Action)delegate
					{
						aimPointAnchor.position = aiTargetRandomPos;
						CalcThrowVec();
						isAiCanThrow = true;
					});
				}
			});
			isAiMoveTween = true;
		}
	}
	private void AiTargetCheck()
	{
		if (!(aiTarget != null) || aiTarget.IsGet)
		{
			RingToss_Target ringToss_Target = SingletonCustom<RingToss_TargetManager>.Instance.SearchRandomTarget();
			if (!(ringToss_Target == null))
			{
				aiTarget = ringToss_Target;
				aiTargetRandomPos = ringToss_Target.GetAiTargetPos() + Quaternion.Euler(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(0f, 360f), 0f) * Vector3.forward * UnityEngine.Random.Range(aiAimMinRandomDis, aiAimMaxRandomDis);
			}
		}
	}
	private bool CheckCanAiThrow()
	{
		return isAiCanThrow;
	}
	private void AiThrow()
	{
		AiResetData();
	}
	private void AiResetData()
	{
		aiTarget = null;
		isAiMoveTween = false;
		isAiCanThrow = false;
	}
}
