using System;
using UnityEngine;
public class Golf_Ball : MonoBehaviour
{
	private int playerNo;
	private Rigidbody rigid;
	private SphereCollider collider;
	protected float colliderRadius;
	protected MeshRenderer mesh;
	[SerializeField]
	[Header("TrailRenderer")]
	private TrailRenderer trail;
	private bool isAnyCollision;
	private bool isCupIn;
	private bool isOB;
	private float shotPower;
	private Vector3 shotVec;
	private Vector3 rotDir;
	private float rotForce;
	private Vector3 rotForceVec;
	private float rotGroundForwardForce;
	private float rotGroundBackForce;
	protected Collider[] arrayOverLap = new Collider[2];
	protected RaycastHit[] arrayRaycastHit = new RaycastHit[2];
	protected int fieldLayerMask;
	private readonly string[] FIELD_LAYER_NAME = new string[1]
	{
		"BackGround"
	};
	protected int fieldColliderLayerMask;
	private readonly string[] FIELD_COLLIDER_LAYER_NAME = new string[1]
	{
		"Field"
	};
	private Golf_Ground currentGround;
	private bool isGroundBounce;
	private float bounceValue;
	protected bool isWaitUpdate;
	private bool isCheckStop;
	public virtual void Init(int _playerNo)
	{
		playerNo = _playerNo;
		rigid = GetComponent<Rigidbody>();
		collider = GetComponent<SphereCollider>();
		mesh = GetComponent<MeshRenderer>();
		colliderRadius = collider.radius * base.transform.localScale.x;
		fieldLayerMask = LayerMask.GetMask(FIELD_LAYER_NAME);
		fieldColliderLayerMask = LayerMask.GetMask(FIELD_COLLIDER_LAYER_NAME);
	}
	public virtual void InitPlay()
	{
		rigid.drag = 0f;
		rigid.angularDrag = 0f;
		rigid.velocity = Vector3.zero;
		rigid.isKinematic = true;
		base.transform.position = SingletonCustom<Golf_FieldManager>.Instance.GetReadyBallPos();
		base.transform.rotation = Quaternion.identity;
		currentGround = null;
		isGroundBounce = false;
		bounceValue = 0f;
		isCheckStop = false;
		isCupIn = false;
		isOB = false;
		rotForce = SingletonCustom<Golf_BallManager>.Instance.GetBallRotVelocityForce();
		rotGroundForwardForce = SingletonCustom<Golf_BallManager>.Instance.GetBallRotGroundForwardForce();
		rotGroundBackForce = SingletonCustom<Golf_BallManager>.Instance.GetBallRotGroundBackForce();
		isWaitUpdate = false;
		isAnyCollision = false;
		base.gameObject.SetActive(value: true);
	}
	public void SetAudience()
	{
		base.gameObject.SetActive(value: false);
	}
	public void SetMaterial(Material _mat)
	{
		if (!(mesh != null))
		{
			return;
		}
		string name = _mat.name;
		name = name.Substring(0, name.LastIndexOf("_"));
		Material[] materials = mesh.materials;
		for (int i = 0; i < materials.Length; i++)
		{
			if (materials[i].name.Contains(name))
			{
				materials[i] = _mat;
				break;
			}
		}
		mesh.materials = materials;
	}
	public void SetTrailMaterial(Color _startColor, Color _endColor)
	{
		if (trail != null)
		{
			trail.startColor = _startColor;
			Color endColor = _endColor;
			endColor.a = trail.endColor.a;
			trail.endColor = endColor;
		}
	}
	public void FixedUpdateMethod()
	{
		if (SingletonCustom<Golf_GameManager>.Instance.GetState() != Golf_GameManager.State.BALL_FLY || !isWaitUpdate)
		{
			return;
		}
		SetRotForce();
		if (isGroundBounce && rigid.velocity.y > 0f)
		{
			UnityEngine.Debug.Log("rigid.velocity " + rigid.velocity.ToString());
			rigid.velocity += Vector3.down * bounceValue * Time.fixedDeltaTime;
			UnityEngine.Debug.Log("バウンド補正 " + rigid.velocity.ToString());
		}
		if (currentGround != null)
		{
			if (rigid.drag < SingletonCustom<Golf_BallManager>.Instance.GetGroundCollisionDrag())
			{
				rigid.drag = SingletonCustom<Golf_BallManager>.Instance.GetGroundCollisionDrag();
			}
			rigid.angularDrag = SingletonCustom<Golf_FieldManager>.Instance.GetGroundAttenuationAngularDrag(currentGround.GetGroundType());
			rigid.velocity *= SingletonCustom<Golf_FieldManager>.Instance.GetGroundAttenuationVelocity(currentGround.GetGroundType());
			if (isCheckStop && rigid.velocity.sqrMagnitude < SingletonCustom<Golf_BallManager>.Instance.GetBallStopMagnitude())
			{
				rigid.drag += SingletonCustom<Golf_FieldManager>.Instance.GetGroundAttenuationDrag(currentGround.GetGroundType());
			}
			if (!isOB && rigid.velocity.sqrMagnitude < 0.01f)
			{
				Stop();
			}
		}
	}
	private void CollisionGround(Collider _collider)
	{
		if (Physics.RaycastNonAlloc(base.transform.position, Vector3.down, arrayRaycastHit, float.PositiveInfinity, fieldColliderLayerMask) > 0)
		{
			Golf_Ground component = arrayRaycastHit[0].collider.GetComponent<Golf_Ground>();
			UnityEngine.Debug.Log("ground " + component?.ToString());
			currentGround = component;
			isGroundBounce = false;
			bounceValue = 0f;
		}
		else
		{
			currentGround = null;
		}
	}
	private void SetBoounce(Collider _collider)
	{
		if (Physics.RaycastNonAlloc(base.transform.position, Vector3.down, arrayRaycastHit, float.PositiveInfinity, fieldColliderLayerMask) > 0)
		{
			Golf_Ground component = arrayRaycastHit[0].collider.GetComponent<Golf_Ground>();
			UnityEngine.Debug.Log("ground " + component?.ToString());
			isGroundBounce = true;
			bounceValue = SingletonCustom<Golf_FieldManager>.Instance.GetGroundAttenuationBounce(component.GetGroundType());
		}
	}
	public void SetRotForce()
	{
		rigid.AddForce(rotForceVec * rotForce, ForceMode.Impulse);
	}
	public void SetWindForce()
	{
		if (!(currentGround != null))
		{
			rigid.AddForce(SingletonCustom<Golf_WindManager>.Instance.GetWindVelocity(), ForceMode.Impulse);
		}
	}
	public void Stop()
	{
		if (!isCupIn)
		{
			if (SingletonCustom<Golf_CameraManager>.Instance.CheckViewCupRot())
			{
				SingletonCustom<Golf_GameManager>.Instance.SetState(Golf_GameManager.State.VIEW_CUP_LINE);
			}
			else
			{
				SingletonCustom<Golf_GameManager>.Instance.SetState(Golf_GameManager.State.VIEW_CUP_ROT_CAMERA);
			}
		}
		else
		{
			SingletonCustom<Golf_GameManager>.Instance.CalcPoint();
		}
		rigid.velocity = Vector3.zero;
		rigid.isKinematic = true;
	}
	public void SetShotPower(float _shotPower)
	{
		shotPower = _shotPower;
		float num = shotPower / SingletonCustom<Golf_PlayerManager>.Instance.GetBaseShotPower();
		rotForce *= num;
		rotGroundForwardForce *= num;
		rotGroundBackForce *= num;
	}
	public void SetShotVec(Vector3 _shotVec)
	{
		shotVec = _shotVec;
	}
	public void SetRotDir(Vector3 _rotDir)
	{
		rotDir = _rotDir;
		UnityEngine.Debug.Log("rotDir x : " + rotDir.x.ToString() + " rotDir y : " + rotDir.y.ToString() + " rotDir z : " + rotDir.z.ToString());
		rotForceVec = new Vector3(0f - rotDir.x, 0f - rotDir.y, (rotDir.y < -0.25f) ? (-0.75f) : 0f);
		UnityEngine.Debug.Log("rotForce x : " + rotForceVec.x.ToString() + " y : " + rotForceVec.y.ToString() + " z : " + rotForceVec.z.ToString());
	}
	public void Shot()
	{
		UnityEngine.Debug.DrawRay(base.transform.position, shotVec * shotPower, Color.black, 10f);
		rigid.isKinematic = false;
		rigid.AddForce(shotVec * shotPower, ForceMode.Impulse);
		Vector3 a = new Vector3(rotDir.y, 0f - rotDir.x, 0f);
		rigid.AddTorque(a * (shotPower * 0.5f), ForceMode.Impulse);
		LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
		{
			isWaitUpdate = true;
		});
		LeanTween.delayedCall(base.gameObject, 0.25f, (Action)delegate
		{
			isCheckStop = true;
		});
	}
	public float GetRigidbodyMass()
	{
		return rigid.mass;
	}
	public bool GetIsAnyCollision()
	{
		return isAnyCollision;
	}
	public bool GetIsCupIn()
	{
		return isCupIn;
	}
	public void SetIsCupIn()
	{
		isCupIn = true;
	}
	public bool GetIsOB()
	{
		return isOB;
	}
	public void SetIsOB()
	{
		isOB = true;
	}
	private void OnCollisionEnter(Collision collision)
	{
		if ((!isOB && SingletonCustom<Golf_GameManager>.Instance.GetState() != Golf_GameManager.State.BALL_FLY) || !isWaitUpdate)
		{
			return;
		}
		if (!isAnyCollision)
		{
			isAnyCollision = true;
		}
		if (collision.gameObject.layer == LayerMask.NameToLayer("BackGround"))
		{
			if (isOB)
			{
				rigid.velocity = Vector3.zero;
				rigid.isKinematic = true;
				return;
			}
			CollisionGround(collision.collider);
			if (currentGround != null)
			{
				SingletonCustom<Golf_FieldManager>.Instance.PlayGroundEffect(currentGround.GetGroundType(), collision.contacts[0].point);
				float sqrMagnitude = rigid.velocity.sqrMagnitude;
				float ballStopMagnitude = SingletonCustom<Golf_BallManager>.Instance.GetBallStopMagnitude();
				float volume = (sqrMagnitude < ballStopMagnitude) ? (sqrMagnitude / ballStopMagnitude) : 1f;
				SingletonCustom<Golf_FieldManager>.Instance.PlayGroundEffectSE(currentGround.GetGroundType(), volume);
			}
			Vector3 a = shotVec;
			a.y = 0f;
			if (rotDir.y > 0f)
			{
				rigid.AddForce(a * rotGroundForwardForce);
			}
			else if (rotDir.y < 0f)
			{
				rigid.AddForce(-a * rotGroundBackForce);
			}
			rotForce /= 10f;
			rotGroundForwardForce /= 10f;
			rotGroundBackForce /= 10f;
		}
		UnityEngine.Debug.Log("地面との接触判定 CollisionEnter");
	}
	private void OnCollisionStay(Collision collision)
	{
		if (SingletonCustom<Golf_GameManager>.Instance.GetState() == Golf_GameManager.State.BALL_FLY && isWaitUpdate)
		{
			if (collision.gameObject.layer == LayerMask.NameToLayer("BackGround"))
			{
				CollisionGround(collision.collider);
			}
			UnityEngine.Debug.Log("地面との接触判定 CollisionStay");
		}
	}
	private void OnCollisionExit(Collision collision)
	{
		if (SingletonCustom<Golf_GameManager>.Instance.GetState() == Golf_GameManager.State.BALL_FLY && isWaitUpdate)
		{
			if (collision.gameObject.layer == LayerMask.NameToLayer("BackGround"))
			{
				currentGround = null;
				SetBoounce(collision.collider);
			}
			UnityEngine.Debug.Log("地面との接触判定を解除 CollisionExit");
		}
	}
}
