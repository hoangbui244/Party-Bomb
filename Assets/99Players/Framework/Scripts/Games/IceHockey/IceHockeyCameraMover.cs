using System;
using UnityEngine;
public class IceHockeyCameraMover : SingletonCustom<IceHockeyCameraMover>
{
	public enum State
	{
		STANDBY,
		FACE_OFF,
		PUCK
	}
	[SerializeField]
	[Header("カメラ")]
	private Camera camera;
	[SerializeField]
	[Header("タ\u30fcゲット")]
	private IceHockeyPuck targetObj;
	private const float MOVER_SPEED_DEFALUT = 0.75f;
	private const float MOVER_SPEED_MOVE = 0.1f;
	private const float MAP_MOVE_SPEED = 0.075f;
	private const float ROT_SPEED = 0.85f;
	private Vector3 OFFSET_PUCK = new Vector3(0f, 11.3f, -14.8f);
	private readonly Vector3 POS_RESULT_START = new Vector3(6.38f, 1.48f, 2.47f);
	private readonly Vector3 POS_RESULT = new Vector3(4006.88f, 1.62f, 502.47f);
	private readonly Vector3 ROT_RESULT = new Vector3(25.7f, 90f, 0f);
	private readonly Vector3 POS_CENTER = new Vector3(4003.592f, 17.47298f, 505.86f);
	private readonly float POS_Z_LIMIT = -21.5f;
	private State currentState = State.PUCK;
	private Vector3 targetPos;
	private Vector3 targetRot;
	private bool isUpdateRotAngle;
	private float fixSpeedRate = 0.1f;
	private float startAngle;
	private Vector3 moveOffset = Vector3.zero;
	private Vector3 calcPos;
	private Quaternion calcRot;
	private float calcAngle;
	public bool IsMoveLock
	{
		get;
		set;
	}
	public bool IsUpdateRotAngle => isUpdateRotAngle;
	public Camera GetCamera()
	{
		return camera;
	}
	public void SetFixPos()
	{
		CalcCameraPos();
		base.transform.position = calcPos;
		if (base.transform.localPosition.z <= POS_Z_LIMIT)
		{
			base.transform.SetLocalPositionZ(POS_Z_LIMIT);
		}
		CalcCameraRot();
		base.transform.rotation = calcRot;
	}
	public void SetState(State _state)
	{
		if (currentState == State.STANDBY)
		{
			isUpdateRotAngle = true;
			LeanTween.value(0f, 1f, 2.5f).setOnUpdate(delegate(float _value)
			{
				fixSpeedRate = _value;
			}).setOnComplete((Action)delegate
			{
				isUpdateRotAngle = false;
			});
		}
		currentState = _state;
	}
	private void Awake()
	{
	}
	private void LateUpdate()
	{
		if (IsMoveLock)
		{
			return;
		}
		switch (currentState)
		{
		case State.FACE_OFF:
			CalcCameraPos(0.7f);
			calcPos.z += 1f;
			base.transform.position = Vector3.Slerp(base.transform.position, calcPos, 0.75f * fixSpeedRate);
			if (base.transform.localPosition.z <= POS_Z_LIMIT)
			{
				base.transform.SetLocalPositionZ(POS_Z_LIMIT);
			}
			CalcCameraRot();
			if (isUpdateRotAngle)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, calcRot, 0.85f * fixSpeedRate);
			}
			break;
		default:
			CalcCameraPos();
			base.transform.position = Vector3.Slerp(base.transform.position, calcPos, 0.75f * fixSpeedRate);
			if (base.transform.localPosition.z <= POS_Z_LIMIT)
			{
				base.transform.SetLocalPositionZ(POS_Z_LIMIT);
			}
			CalcCameraRot();
			if (isUpdateRotAngle)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, calcRot, 0.85f * fixSpeedRate);
			}
			break;
		case State.STANDBY:
			break;
		}
		base.transform.SetLocalEulerAnglesX(40f);
	}
	private void CalcCameraPos(float _offsetScale = 1f)
	{
		calcRot = Quaternion.AngleAxis(0f, Vector3.up);
		calcPos = GetTargetPos() + OFFSET_PUCK * _offsetScale;
		calcPos -= GetTargetPos();
		calcPos = calcRot * calcPos;
		calcPos += GetTargetPos();
	}
	private void CalcCameraRot()
	{
		calcPos = GetTargetPos() - base.transform.position;
		calcRot = Quaternion.LookRotation(calcPos);
	}
	public Vector3 GetTargetPos()
	{
		return targetObj.transform.position;
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(base.gameObject);
	}
}
