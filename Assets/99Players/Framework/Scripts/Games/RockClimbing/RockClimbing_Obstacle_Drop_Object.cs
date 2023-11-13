using System;
using UnityEngine;
public class RockClimbing_Obstacle_Drop_Object : MonoBehaviour
{
	[SerializeField]
	[Header("生成する障害物の種類")]
	private RockClimbing_ClimbingWallManager.ObstacleDropType obstacleDropType;
	private RockClimbing_Player climbingPlayer;
	private RockClimbing_Obstacle_Drop obstacleDrop;
	private Rigidbody rigid;
	private Collider collider;
	[SerializeField]
	[Header("ル\u30fcト")]
	private GameObject root;
	[SerializeField]
	[Header("オブジェクト")]
	private GameObject obj;
	private Vector3 originObjScale;
	[SerializeField]
	[Header("エフェクト")]
	private GameObject effect;
	private bool isHit;
	private GameObject collisionIgnore;
	private int obstacleLineIdx;
	public void Init(RockClimbing_Obstacle_Drop _obstacleDrop)
	{
		obstacleDrop = _obstacleDrop;
		rigid = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();
		rigid.isKinematic = true;
		originObjScale = obj.transform.localScale;
		switch (obstacleDropType)
		{
		case RockClimbing_ClimbingWallManager.ObstacleDropType.SnowMass:
			obj.transform.localScale = Vector3.zero;
			break;
		case RockClimbing_ClimbingWallManager.ObstacleDropType.Icicles:
			obj.transform.SetLocalScaleY(0f);
			break;
		}
	}
	private void Update()
	{
		if (!isHit && base.transform.position.y < 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
	public void SetClimbingPlayer(RockClimbing_Player _climbingPlayer)
	{
		climbingPlayer = _climbingPlayer;
	}
	public void SetCollisionIgnore(GameObject _collisionIgnore)
	{
		collisionIgnore = _collisionIgnore;
	}
	public void SetObstacleLineIdx(int _obstacleLineIdx)
	{
		obstacleLineIdx = _obstacleLineIdx;
	}
	public void Drop()
	{
		switch (obstacleDropType)
		{
		case RockClimbing_ClimbingWallManager.ObstacleDropType.SnowMass:
			LeanTween.value(base.gameObject, 0f, 1f, 0.15f).setOnUpdate(delegate(float _value)
			{
				obj.transform.localScale = originObjScale * _value;
			});
			rigid.isKinematic = false;
			rigid.AddForce(Vector3.down * SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropPower(), ForceMode.Acceleration);
			break;
		case RockClimbing_ClimbingWallManager.ObstacleDropType.Icicles:
			LeanTween.value(base.gameObject, 0f, 1f, 0.15f).setOnUpdate(delegate(float _value)
			{
				obj.transform.SetLocalScaleY(originObjScale.y * _value);
			});
			LeanTween.value(base.gameObject, 0f, 1f, 0.25f).setOnUpdate(delegate(float _value)
			{
				root.transform.SetLocalPositionX((LeanTween.shake.Evaluate(_value) - 0.5f) * 0.05f);
			}).setLoopPingPong(2)
				.setOnComplete((Action)delegate
				{
					rigid.isKinematic = false;
					rigid.AddForce(Vector3.down * SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetObstacleDropPower(), ForceMode.Acceleration);
				});
			break;
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if ((!(collisionIgnore != null) || !(collisionIgnore == other.gameObject)) && !other.GetComponent<RockClimbing_Obstacle_Drop_Object>())
		{
			isHit = true;
			rigid.isKinematic = true;
			collider.enabled = false;
			root.SetActive(value: false);
			effect.SetActive(value: true);
			obstacleDrop.SetIsObstacleDrop(obstacleLineIdx, _isObstacleDrop: false);
			LeanTween.delayedCall(base.gameObject, 2f, (Action)delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			if (other.gameObject.tag == "Player" || other.gameObject.tag == "Character")
			{
				climbingPlayer.HitObstacle();
			}
		}
	}
}
