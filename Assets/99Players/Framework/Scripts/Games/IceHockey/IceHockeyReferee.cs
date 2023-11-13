using UnityEngine;
public class IceHockeyReferee : MonoBehaviour
{
	public enum State
	{
		DEFAULT,
		FACE_OFF_KEEP,
		FACE_OFF
	}
	[SerializeField]
	[Header("リジッドボディ")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("パックキャッチアンカ\u30fc")]
	private Transform puckCatchAnchor;
	[SerializeField]
	[Header("AI")]
	private IceHockeyPlayerAI ai;
	[SerializeField]
	[Header("アニメ\u30fcション")]
	private IceHockeyPlayer_Animation anim;
	private float moveSpeed;
	private readonly float LOOK_SPEED = 10f;
	private readonly float MOVE_SPEED_MAX = 11f;
	private readonly float ATTENUATION_SCALE = 0.95f;
	private Quaternion tempRot;
	private Vector3 moveForce;
	private Vector3 prevDir;
	private float puckReleaseTime;
	private State currentState;
	public Transform PuckCatchAnchor => puckCatchAnchor;
	public bool IsPuckRelease
	{
		get;
		set;
	}
	public void SetFaceOffKeep()
	{
		SetState(State.FACE_OFF_KEEP);
		anim.SetCharacterSpeed(0f);
		anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE);
		rigid.isKinematic = true;
		rigid.velocity = Vector3.zero;
		base.transform.position = SingletonCustom<IceHockeyRinkManager>.Instance.AnchorReferee.position;
		base.transform.SetLocalEulerAnglesY(90f);
		IsPuckRelease = false;
		puckReleaseTime = 0f;
	}
	public void MoveInertia()
	{
		moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
		anim.SetCharacterSpeed(rigid.velocity.magnitude);
	}
	public void UpdateMethod()
	{
		switch (currentState)
		{
		case State.FACE_OFF_KEEP:
			rigid.velocity = Vector3.zero;
			if (SingletonCustom<IceHockeyGameManager>.Instance.CurrentState == IceHockeyGameManager.State.InGame)
			{
				puckReleaseTime += UnityEngine.Random.Range(0.9f, 1.1f) * Time.deltaTime;
			}
			if (puckReleaseTime >= 1f)
			{
				rigid.isKinematic = false;
				IsPuckRelease = true;
				SingletonCustom<IceHockeyPuck>.Instance.ReleaseFaceOff();
				SetState(State.FACE_OFF);
			}
			break;
		case State.DEFAULT:
			moveForce = ai.UpdateForceReferee();
			if (moveForce.magnitude < 0.0400000028f)
			{
				moveSpeed = Mathf.Clamp(moveSpeed * ATTENUATION_SCALE, 0f, MOVE_SPEED_MAX);
			}
			else if (moveSpeed < MOVE_SPEED_MAX)
			{
				moveSpeed = Mathf.Clamp(moveSpeed + Time.deltaTime * MOVE_SPEED_MAX, MOVE_SPEED_MAX * 0.3f, MOVE_SPEED_MAX);
			}
			break;
		}
		if (Mathf.Abs(Vector3.Angle(prevDir, moveForce)) >= 90f)
		{
			moveSpeed *= 0.5f;
			anim.EmitMoveEffct(1);
		}
		prevDir = moveForce;
		anim.SetCharacterSpeed(rigid.velocity.magnitude);
	}
	public void FixedUpdate()
	{
		if (currentState == State.DEFAULT)
		{
			rigid.velocity = Vector3.Slerp(rigid.velocity, moveForce * moveSpeed * 1f, 0.05f);
		}
		if (currentState == State.DEFAULT && moveForce.magnitude >= 0.01f)
		{
			CalcManager.mCalcVector3 = SingletonCustom<IceHockeyPuck>.Instance.transform.position;
			CalcManager.mCalcVector3.y = base.transform.position.y;
			tempRot = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f) * (CalcManager.mCalcVector3 - base.transform.position).normalized), Time.deltaTime * LOOK_SPEED);
			if (tempRot != Quaternion.identity)
			{
				base.transform.rotation = tempRot;
				rigid.MoveRotation(tempRot);
			}
		}
	}
	public void OnFaceOff()
	{
		if (currentState == State.FACE_OFF)
		{
			SetState(State.DEFAULT);
		}
	}
	private void SetState(State _state)
	{
		currentState = _state;
		if (currentState == State.DEFAULT)
		{
			anim.SetAnim(IceHockeyPlayer_Animation.AnimType.MOVE_REFEREE);
		}
	}
}
