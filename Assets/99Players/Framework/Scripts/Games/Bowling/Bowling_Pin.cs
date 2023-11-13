using UnityEngine;
public class Bowling_Pin : MonoBehaviour
{
	[SerializeField]
	[Header("リジッドボディ")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("重心アンカ\u30fc")]
	private Transform centerOfMassAnchor;
	[SerializeField]
	[Header("ピンのメッシュレンダラ\u30fc")]
	private MeshRenderer pin;
	private Vector3 defLocalPos;
	private readonly float radius = 0.12f;
	private bool isFall;
	private bool isMoveStop;
	private float STOP_TIME = 3f;
	private float stopTime;
	private bool isBallHit;
	private float pinRadius = 0.07f;
	public Rigidbody Rigid => rigid;
	public bool IsFall => isFall;
	public bool IsMoveStop => isMoveStop;
	public float PinRadius => pinRadius;
	public bool IsBallHit => isBallHit;
	public void Init()
	{
		rigid.centerOfMass = new Vector3(0f, 0.2f, 0f);
		defLocalPos = base.transform.localPosition;
	}
	public void ResetPos(Transform _parent = null)
	{
		if (_parent != null)
		{
			base.transform.parent = _parent;
		}
		rigid.velocity = CalcManager.mVector3Zero;
		rigid.angularVelocity = CalcManager.mVector3Zero;
		rigid.isKinematic = false;
		base.transform.localPosition = defLocalPos;
		base.transform.SetLocalEulerAngles(0f, 0f, 0f);
		isFall = false;
		isBallHit = false;
		isMoveStop = false;
		stopTime = 0f;
		rigid.Sleep();
		base.gameObject.SetActive(value: true);
	}
	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
	public void Fall()
	{
		isFall = true;
	}
	public void CheckMoveStop()
	{
		if (isMoveStop)
		{
			return;
		}
		if (Bowling_Define.MPM.GetNowThrowUserBall().CheckBallState(Bowling_Ball.BallState.HOLE) || Bowling_Define.MPM.GetNowThrowUserBall().CheckBallState(Bowling_Ball.BallState.STOP))
		{
			stopTime += Time.deltaTime;
			if (stopTime >= STOP_TIME)
			{
				isMoveStop = true;
				rigid.Sleep();
				stopTime = 0f;
			}
		}
		if (rigid.velocity.magnitude <= 0.0025f)
		{
			isMoveStop = true;
		}
	}
	public void CheckFall()
	{
		if (!isFall)
		{
			isFall = (Mathf.Abs(base.transform.localPosition.x) > Bowling_Define.MSM.GetLaneSize.x * 0.5f + radius * 1.1f || base.transform.position.z > Bowling_Define.MSM.LaneEndPos.z + radius * 1.1f || (20f < base.transform.rotation.eulerAngles.x && base.transform.rotation.eulerAngles.x < 340f) || (20f < base.transform.rotation.eulerAngles.z && base.transform.rotation.eulerAngles.z < 340f) || base.transform.position.y <= Bowling_Define.MSM.GetLanePos.y);
		}
	}
	public bool IsStop()
	{
		return isMoveStop;
	}
	public void SetPinMaterial(Material _pinMat)
	{
		pin.material = _pinMat;
	}
	private void OnCollisionEnter(Collision _col)
	{
		if (Bowling_Define.MPM.GetNowThrowUserBall().CheckBallState(Bowling_Ball.BallState.NONE))
		{
			return;
		}
		if (_col.gameObject.tag == Bowling_Define.TAG_BOWLING_BALL || _col.gameObject.tag == Bowling_Define.TAG_BOWLING_PIN)
		{
			isMoveStop = false;
		}
		if (Bowling_Define.MPM.GetNowThrowUserBall().CheckBallState(Bowling_Ball.BallState.ROLL))
		{
			if (_col.gameObject.tag == Bowling_Define.TAG_BOWLING_BALL)
			{
				if (!isBallHit)
				{
					if (Mathf.Abs(base.transform.localPosition.x) <= Bowling_Define.MSM.GetLaneSize.x * 0.5f + radius)
					{
						SingletonCustom<AudioManager>.Instance.SePlay("se_pin_shot");
					}
					isBallHit = true;
				}
			}
			else if (_col.gameObject.tag == Bowling_Define.TAG_BOWLING_PIN)
			{
				switch (UnityEngine.Random.Range(0, 3))
				{
				case 0:
					SingletonCustom<AudioManager>.Instance.SePlay("se_pin_medium_0", _loop: false, 0f, 0.25f);
					break;
				case 1:
					SingletonCustom<AudioManager>.Instance.SePlay("se_pin_medium_1", _loop: false, 0f, 0.25f);
					break;
				default:
					SingletonCustom<AudioManager>.Instance.SePlay("se_pin_medium_2", _loop: false, 0f, 0.25f);
					break;
				}
			}
			else if (_col.gameObject.tag == Bowling_Define.TAG_BOWLING_GUTTER && !isFall)
			{
				isFall = true;
			}
		}
		else if (_col.gameObject.tag == Bowling_Define.TAG_BOWLING_PIN)
		{
			switch (UnityEngine.Random.Range(0, 3))
			{
			case 0:
				SingletonCustom<AudioManager>.Instance.SePlay("se_pin_medium_0", _loop: false, 0f, 0.25f);
				break;
			case 1:
				SingletonCustom<AudioManager>.Instance.SePlay("se_pin_medium_1", _loop: false, 0f, 0.25f);
				break;
			default:
				SingletonCustom<AudioManager>.Instance.SePlay("se_pin_medium_2", _loop: false, 0f, 0.25f);
				break;
			}
		}
		if (_col.gameObject.tag == Bowling_Define.TAG_BOWLING_HOLE && !isFall)
		{
			isFall = true;
		}
	}
	private void OnCollisionExit(Collision col)
	{
		if (Bowling_Define.MPM.GetNowThrowUserBall().CheckBallState(Bowling_Ball.BallState.ROLL) && col.gameObject.tag == Bowling_Define.TAG_BOWLING_BALL)
		{
			rigid.AddForce(rigid.velocity.x * 1.55f, rigid.velocity.y, rigid.velocity.z * 1.55f);
			rigid.angularVelocity *= 1.5f;
		}
	}
	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == Bowling_Define.TAG_BOWLING_HOLE && !isFall)
		{
			isFall = true;
		}
	}
}
