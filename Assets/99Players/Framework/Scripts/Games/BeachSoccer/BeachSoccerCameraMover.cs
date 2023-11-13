using UnityEngine;
public class BeachSoccerCameraMover : SingletonCustom<BeachSoccerCameraMover>
{
	public enum State
	{
		STANDBY,
		KICK_OFF,
		THROW_IN,
		BALL
	}
	[SerializeField]
	[Header("カメラ")]
	private Camera camera;
	[SerializeField]
	[Header("タ\u30fcゲット")]
	private BeachSoccerBall targetObj;
	private const float MOVER_SPEED_DEFALUT = 0.75f;
	private const float ROT_SPEED = 0.85f;
	private Vector3 OFFSET_BALL = new Vector3(0f, 8.26f, -10.185f);
	private readonly float POS_Z_LIMIT_BACK = -7.45f;
	private readonly float POS_Z_LIMIT_FRONT = -12.25f;
	private readonly float POS_X_LIMIT_LEFT = -4.35f;
	private readonly float POS_X_LIMIT_RIGHT = 4.35f;
	private State currentState;
	private Vector3 targetPos;
	private Vector3 targetRot;
	private bool isUpdateRotAngle;
	private float fixSpeedRate = 0.1f;
	private float startAngle;
	private Vector3 moveOffset = Vector3.zero;
	private Vector3 calcPos;
	private Quaternion calcRot;
	private Vector3 pos;
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
		if (base.transform.localPosition.z > POS_Z_LIMIT_BACK)
		{
			base.transform.SetLocalPositionZ(POS_Z_LIMIT_BACK);
		}
		if (base.transform.localPosition.z < POS_Z_LIMIT_FRONT)
		{
			base.transform.SetLocalPositionZ(POS_Z_LIMIT_FRONT);
		}
		if (base.transform.localPosition.x < POS_X_LIMIT_LEFT)
		{
			base.transform.SetLocalPositionX(POS_X_LIMIT_LEFT);
		}
		if (base.transform.localPosition.x > POS_X_LIMIT_RIGHT)
		{
			base.transform.SetLocalPositionX(POS_X_LIMIT_RIGHT);
		}
		CalcCameraRot();
		base.transform.rotation = calcRot;
	}
	public void SetState(State _state)
	{
		State currentState2 = currentState;
		currentState = _state;
	}
	private void Awake()
	{
	}
	public void LateUpdate()
	{
		if (IsMoveLock)
		{
			return;
		}
		switch (currentState)
		{
		case State.KICK_OFF:
			CalcCameraPos();
			calcPos.z += 0.75f;
			base.transform.position = Vector3.Slerp(base.transform.position, calcPos, 0.75f * fixSpeedRate);
			CalcCameraRot();
			if (isUpdateRotAngle)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, calcRot, 0.85f * fixSpeedRate);
			}
			break;
		default:
			CalcCameraPos();
			base.transform.position = Vector3.Slerp(base.transform.position, calcPos, 0.75f * fixSpeedRate);
			if (base.transform.localPosition.z > POS_Z_LIMIT_BACK)
			{
				base.transform.SetLocalPositionZ(POS_Z_LIMIT_BACK);
			}
			if (base.transform.localPosition.z <= POS_Z_LIMIT_FRONT)
			{
				base.transform.SetLocalPositionZ(POS_Z_LIMIT_FRONT);
			}
			if (base.transform.localPosition.x < POS_X_LIMIT_LEFT)
			{
				base.transform.SetLocalPositionX(POS_X_LIMIT_LEFT);
			}
			if (base.transform.localPosition.x > POS_X_LIMIT_RIGHT)
			{
				base.transform.SetLocalPositionX(POS_X_LIMIT_RIGHT);
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
		calcPos = GetTargetPos() + OFFSET_BALL * _offsetScale;
		calcPos -= GetTargetPos();
		calcPos = calcRot * calcPos;
		calcPos += GetTargetPos();
	}
	private void CalcCameraRot()
	{
		calcPos = GetTargetPos();
		calcPos.x = base.transform.position.x;
		calcPos -= base.transform.position;
		calcRot = Quaternion.LookRotation(calcPos);
	}
	public Vector3 GetTargetPos()
	{
		pos = targetObj.transform.position;
		pos.y = 0.132f;
		return pos;
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(base.gameObject);
	}
}
