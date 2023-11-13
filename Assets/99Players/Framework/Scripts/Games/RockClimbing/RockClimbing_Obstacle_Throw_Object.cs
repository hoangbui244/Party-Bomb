using System;
using UnityEngine;
public class RockClimbing_Obstacle_Throw_Object : MonoBehaviour
{
	[SerializeField]
	[Header("生成する障害物の種類")]
	private RockClimbing_ClimbingWallManager.ObstacleThrowType obstacleThrowType;
	private RockClimbing_Obstacle_Throw_Group obstacleThrowGroup;
	private Rigidbody rigid;
	private Collider collider;
	[SerializeField]
	[Header("ル\u30fcト")]
	private GameObject root;
	[SerializeField]
	[Header("オブジェクト")]
	private GameObject obj;
	[SerializeField]
	[Header("ライン")]
	private GameObject line;
	private Vector3 originObjScale;
	[SerializeField]
	[Header("エフェクト")]
	private GameObject effect;
	private bool isHit;
	private Collider collisionIgnoreCollider;
	[SerializeField]
	[Header("回転速度")]
	private float ROT_SPEED = 15f;
	private float angle;
	private Vector3 throwVec;
	public void Init(RockClimbing_Obstacle_Throw_Group _obstacleThrowGroup)
	{
		obstacleThrowGroup = _obstacleThrowGroup;
		rigid = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();
		rigid.isKinematic = true;
		originObjScale = obj.transform.localScale;
	}
	public void SetCollisionIgnoreCollider(Collider _collisionIgnoreCollider)
	{
		collisionIgnoreCollider = _collisionIgnoreCollider;
	}
	private void Update()
	{
		if (!isHit)
		{
			root.transform.AddLocalEulerAnglesX(Time.deltaTime * ROT_SPEED);
			if (Mathf.Abs(base.transform.localPosition.x) >= 13f)
			{
				obstacleThrowGroup.RemoveThrowObjectList(this);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
	public void Throw(RockClimbing_Player _climbingPlayer)
	{
		rigid.isKinematic = false;
		Vector3 vector = _climbingPlayer.GetHeadTop().position;
		Vector3 vector3;
		switch (_climbingPlayer.GetState())
		{
		case RockClimbing_PlayerManager.State.MOVE:
		case RockClimbing_PlayerManager.State.THROW:
		{
			Vector3 b = UnityEngine.Random.insideUnitSphere * 0.5f;
			b.y = 0f;
			vector = _climbingPlayer.GetHeadTop().position + b;
			UnityEngine.Debug.DrawLine(base.transform.position, vector, Color.magenta, 3f);
			break;
		}
		case RockClimbing_PlayerManager.State.CLIMBING:
		{
			Vector3 climbingVec = _climbingPlayer.GetClimbingVec();
			Vector3 vector2 = _climbingPlayer.GetHeadTop().position + climbingVec * 3f / _climbingPlayer.transform.localScale.x;
			Vector3 position = _climbingPlayer.GetHeadTop().position;
			vector3 = vector2;
			UnityEngine.Debug.Log("upper " + vector3.ToString());
			vector3 = position;
			UnityEngine.Debug.Log("lower " + vector3.ToString());
			float num = CalcManager.Length(position, vector2);
			vector = position + climbingVec * (UnityEngine.Random.Range(0f, 1f) * num);
			UnityEngine.Debug.DrawLine(base.transform.position, vector, Color.magenta, 3f);
			break;
		}
		}
		vector3 = vector;
		UnityEngine.Debug.Log("targetPos " + vector3.ToString());
		throwVec = (vector - base.transform.position).normalized;
		base.transform.LookAt(vector);
		rigid.AddForce(throwVec * SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleThrowPower(), ForceMode.Impulse);
	}
	public Vector3 GetThrowVec()
	{
		return throwVec;
	}
	public bool GetIsHit()
	{
		return isHit;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (collisionIgnoreCollider != null && collisionIgnoreCollider == other)
		{
			return;
		}
		UnityEngine.Debug.Log("other.gameObject : " + other.gameObject?.ToString());
		isHit = true;
		rigid.isKinematic = true;
		collider.enabled = false;
		root.SetActive(value: false);
		if (line != null)
		{
			line.SetActive(value: false);
		}
		if (effect != null)
		{
			effect.SetActive(value: true);
		}
		LeanTween.delayedCall(base.gameObject, 2f, (Action)delegate
		{
			obstacleThrowGroup.RemoveThrowObjectList(this);
			UnityEngine.Object.Destroy(base.gameObject);
		});
		if (other.gameObject.tag == "Character")
		{
			RockClimbing_Player component = other.transform.parent.GetComponent<RockClimbing_Player>();
			if (component != null)
			{
				component.HitObstacle();
			}
		}
	}
}
