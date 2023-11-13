using System;
using System.Collections;
using UnityEngine;
public class Bowling_Ball : MonoBehaviour
{
	public enum BallState
	{
		THROW_WAIT,
		ROLL,
		GUTTER,
		STOP,
		HOLE,
		NONE
	}
	[Serializable]
	public struct ThrowData
	{
		[Header("速度")]
		public Vector3 velocity;
		[Header("回転速度")]
		public Vector3 angularVelocity;
	}
	[SerializeField]
	[Header("リジッドボディ")]
	public Rigidbody rigid;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private SphereCollider ballCol;
	[SerializeField]
	[Header("オブジェクト")]
	private GameObject obj;
	[SerializeField]
	[Header("メッシュレンダラ\u30fc")]
	private MeshRenderer meshRenderer;
	[SerializeField]
	[Header("摩擦アニカ\u30fcブ")]
	private AnimationCurve frictionDuration;
	[SerializeField]
	[Header("軌跡線")]
	private TrailRenderer afterLint;
	[SerializeField]
	[Header("投球アニメ\u30fcションアンカ\u30fc")]
	private Transform throwAnimAnchor;
	private Vector3 localPosDef;
	private Vector3 localRotDef;
	private int no;
	private PhysicMaterial physicMaterial;
	private float firstDynamicFriction;
	private float firstStaticFriction;
	private float firstBounciness;
	private float speedWhenDropGutter;
	private bool isInPinDeck;
	public static readonly Vector3 POS_DEFAULT = new Vector3(0f, 8.2f, -3.7f);
	private bool isHitPin;
	private ParticleSystem effect;
	[SerializeField]
	[Header("衝突エフェクト")]
	private ParticleSystem hitEffect;
	private bool isPlayHitEffect = true;
	private ThrowData throwData;
	private Bowling_Define.UserType userType;
	private BallState ballState;
	private bool isGutter;
	public Transform ThrowAnimAnchor => throwAnimAnchor;
	public Rigidbody Rigid
	{
		get
		{
			return rigid;
		}
		set
		{
			rigid = value;
		}
	}
	public float Radius => ballCol.radius;
	public SphereCollider Collider => ballCol;
	public Vector3 InitPos
	{
		get;
		set;
	}
	public bool IsInPinDeck => isInPinDeck;
	public bool IsHitPin => isHitPin;
	public bool IsGutter => isGutter;
	public void Init(Bowling_Define.UserType _userType)
	{
		userType = _userType;
		localPosDef = base.transform.localPosition;
		if (afterLint != null)
		{
			afterLint.Clear();
			afterLint.enabled = false;
		}
		localRotDef = base.transform.localEulerAngles;
		InitPos = base.transform.position;
		rigid.maxAngularVelocity *= 100f;
		physicMaterial = ballCol.material;
		ballCol.enabled = false;
		firstDynamicFriction = physicMaterial.dynamicFriction;
		firstStaticFriction = physicMaterial.staticFriction;
		firstBounciness = physicMaterial.bounciness;
		isPlayHitEffect = true;
		ballState = BallState.THROW_WAIT;
		BallSetting();
	}
	private void BallSetting()
	{
		rigid.mass = 0.453592f * ((float)Bowling_Define.BALL_POUND + 1f) * 0.75f;
		base.transform.SetLocalEulerAngles(65f, 180f, 0f);
		localRotDef = base.transform.localEulerAngles;
	}
	public void UpdateMethod()
	{
		if (ballState == BallState.THROW_WAIT)
		{
			return;
		}
		if (ballState != BallState.HOLE && ballState != BallState.STOP)
		{
			if (GetBallPos().z > Bowling_Define.MSM.LaneEndPos.z + ballCol.radius)
			{
				ballState = BallState.HOLE;
			}
			if (GetBallPos().z < Bowling_Define.MSM.BallStartPointAnchor.position.z - ballCol.radius)
			{
				ballState = BallState.HOLE;
			}
			if (rigid.velocity.magnitude <= 0.1f)
			{
				ballState = BallState.STOP;
			}
		}
		UpdateDynamicFriction();
	}
	private void UpdateDynamicFriction()
	{
		if (ballState == BallState.ROLL)
		{
			float z = Bowling_Define.MSM.GetLaneSize.z;
			if (Bowling_Define.MSM.BallStartPointAnchor.position.z < base.transform.position.z && base.transform.position.z < Bowling_Define.MSM.BallStartPointAnchor.position.z + z)
			{
				float time = Mathf.InverseLerp(Bowling_Define.MSM.BallStartPointAnchor.position.z, Bowling_Define.MSM.BallStartPointAnchor.position.z + z, base.transform.position.z);
				physicMaterial.dynamicFriction = frictionDuration.Evaluate(time);
			}
		}
		else if (ballState == BallState.GUTTER)
		{
			Vector3 velocity = rigid.velocity;
			velocity.x = 0f;
			if (velocity.y > 0f)
			{
				velocity.y = 0f;
			}
			velocity.z = speedWhenDropGutter;
			rigid.velocity = velocity;
		}
	}
	public void Throw(Vector3 _vec, float _speed, Bowling_Define.ThrowType _throwType)
	{
		ballState = BallState.ROLL;
		ballCol.enabled = true;
		throwData = CalcThrowData(_vec, _speed, _throwType);
		rigid.constraints = RigidbodyConstraints.None;
		rigid.velocity = throwData.velocity;
		rigid.angularVelocity = throwData.angularVelocity;
		if (afterLint != null)
		{
			afterLint.enabled = true;
		}
	}
	public ThrowData CalcThrowData(Vector3 _vec, float _speed, Bowling_Define.ThrowType _throwType)
	{
		float spinValue = GetSpinValue(_throwType);
		_speed = GetThrowPower(_speed);
		float d = 0.9f;
		ThrowData result = default(ThrowData);
		result.velocity = _vec * _speed * 6f * d + Vector3.up;
		Vector3 vector = result.angularVelocity = new Vector3(10f * _speed, spinValue * 5f, spinValue * -20f);
		return result;
	}
	private float GetSpinValue(Bowling_Define.ThrowType _throwType)
	{
		switch (_throwType)
		{
		case Bowling_Define.ThrowType.STRAIGHT:
			return 0f;
		case Bowling_Define.ThrowType.LEFT_S:
			return 0f - (1.5f + (float)Bowling_Define.BALL_PARAM[(int)_throwType] / 5f);
		case Bowling_Define.ThrowType.RIGHT_S:
			return 1.5f + (float)Bowling_Define.BALL_PARAM[(int)_throwType] / 5f;
		default:
			return 0f;
		}
	}
	private float GetThrowPower(float _speed)
	{
		return _speed * 0.7f + 0.11f * (float)Bowling_Define.BALL_PARAM[1];
	}
	public int GetParam(Bowling_Define.ThrowType _throwType)
	{
		return Bowling_Define.BALL_PARAM[(int)_throwType];
	}
	public void ResetPos(bool _isPlayHitEffect = false)
	{
		rigid.velocity = CalcManager.mVector3Zero;
		rigid.angularVelocity = CalcManager.mVector3Zero;
		rigid.isKinematic = false;
		base.transform.localPosition = localPosDef;
		base.transform.localEulerAngles = localRotDef;
		if (throwAnimAnchor != null)
		{
			throwAnimAnchor.SetLocalPositionX(0f);
			throwAnimAnchor.SetLocalEulerAnglesX(0f);
			throwAnimAnchor.SetLocalEulerAnglesY(0f);
		}
		physicMaterial.dynamicFriction = firstDynamicFriction;
		physicMaterial.staticFriction = firstStaticFriction;
		physicMaterial.bounciness = firstBounciness;
		rigid.constraints = RigidbodyConstraints.FreezeAll;
		isInPinDeck = false;
		isHitPin = false;
		isPlayHitEffect = _isPlayHitEffect;
		meshRenderer.enabled = true;
		ballCol.enabled = false;
		if (afterLint != null)
		{
			afterLint.Clear();
			afterLint.enabled = false;
		}
		SingletonCustom<AudioManager>.Instance.SeStop("se_ball_rolling");
	}
	public GameObject GetObj()
	{
		return obj;
	}
	private void OnCollisionEnter(Collision _col)
	{
		if (ballState != BallState.ROLL)
		{
			return;
		}
		if (_col.gameObject.tag == Bowling_Define.TAG_BOWLING_PIN && isPlayHitEffect)
		{
			isPlayHitEffect = false;
			hitEffect.Play();
		}
		if (!isInPinDeck && _col.gameObject.tag == Bowling_Define.TAG_BOWLING_LANE)
		{
			isInPinDeck = true;
			SingletonCustom<AudioManager>.Instance.SePlay("se_ball_rolling", _loop: true);
			if (Mathf.Abs(_col.relativeVelocity.y) > 1f)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_ball_fall");
			}
		}
		if (_col.gameObject.tag == Bowling_Define.TAG_BOWLING_GUTTER)
		{
			isInPinDeck = true;
			DropGutter();
			physicMaterial.dynamicFriction = 0f;
			speedWhenDropGutter = Mathf.Abs(rigid.velocity.z);
			SingletonCustom<AudioManager>.Instance.SePlay("se_ball_gutter_Fall");
			SingletonCustom<AudioManager>.Instance.SeStop("se_ball_rolling");
		}
		if (_col.gameObject.tag == Bowling_Define.TAG_BOWLING_PIN)
		{
			isHitPin = true;
		}
	}
	private void OnCollisionExit(Collision col)
	{
		if (ballState == BallState.ROLL && isInPinDeck && col.gameObject.tag == Bowling_Define.TAG_BOWLING_LANE)
		{
			isInPinDeck = false;
			SingletonCustom<AudioManager>.Instance.SeStop("se_ball_rolling");
		}
	}
	private void OnTriggerStay(Collider col)
	{
		if (ballState == BallState.ROLL && col.gameObject.tag == Bowling_Define.TAG_BOWLING_HOLE)
		{
			DropHole();
			SingletonCustom<AudioManager>.Instance.SeStop("se_ball_rolling");
		}
	}
	public void BallInvisible(bool _drawing = true, bool _collider = false)
	{
		meshRenderer.enabled = !_drawing;
		ballCol.enabled = _collider;
	}
	private void DropGutter()
	{
		ballState = BallState.GUTTER;
		if (!isHitPin)
		{
			isGutter = true;
		}
	}
	private void DropHole()
	{
		ballState = BallState.HOLE;
		rigid.angularVelocity = CalcManager.mVector3Zero;
	}
	public void ChangeNone()
	{
		ballState = BallState.NONE;
	}
	public Vector3 GetBallPos(bool _local = false)
	{
		if (_local)
		{
			return base.transform.localPosition;
		}
		return base.transform.position;
	}
	public void NowTurnBallResetPos(bool _isPlayHitEffect = false)
	{
		isGutter = false;
		ResetPos(_isPlayHitEffect);
	}
	public void BallStandby()
	{
		NowTurnBallResetPos(Bowling_Define.MPM.NowThrowCount == 0);
		ballState = BallState.THROW_WAIT;
		base.gameObject.SetActive(value: true);
		if (!GetObj().activeSelf)
		{
			GetObj().SetActive(value: true);
		}
		if (userType <= Bowling_Define.UserType.PLAYER_4)
		{
			StartCoroutine(DelayVibration());
		}
	}
	private IEnumerator DelayVibration()
	{
		yield return new WaitForSeconds(0.1f);
		SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userType);
	}
	public bool CheckBallState(BallState _ballState)
	{
		return ballState == _ballState;
	}
	public void SetBallMaterial(Material _mat)
	{
		meshRenderer.material = _mat;
	}
}
