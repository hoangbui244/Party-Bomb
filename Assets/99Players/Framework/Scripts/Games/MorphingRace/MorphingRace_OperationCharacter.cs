using UnityEngine;
public class MorphingRace_OperationCharacter : MonoBehaviour
{
	[SerializeField]
	[Header("操作する動物の種類")]
	protected MorphingRace_FieldManager.TargetPrefType characterType;
	protected MorphingRace_Player player;
	protected Rigidbody rigid;
	[SerializeField]
	[Header("コライダ\u30fc")]
	protected CapsuleCollider collider;
	protected float inputLerp;
	protected float moveSpeed;
	protected bool isMoveSe;
	[SerializeField]
	[Header("移動用のエフェクト")]
	protected ParticleSystem moveEffect;
	protected int wallLayerMask;
	private string[] wallLayerMaskNameList = new string[1]
	{
		"Wall"
	};
	protected float collider_radius;
	protected Collider[] arrayOverLapCollider = new Collider[4];
	protected RaycastHit[] arrayRaycastHit = new RaycastHit[4];
	private Vector3 Debug_Point0;
	private Vector3 Debug_Point1;
	private Vector3 Debug_Point0_CheckObstacle;
	private Vector3 Debug_Point1_CheckObstacle;
	public virtual void Init(MorphingRace_Player _player)
	{
		player = _player;
		rigid = player.GetRigidbody();
		wallLayerMask = LayerMask.GetMask(wallLayerMaskNameList);
		collider_radius = collider.radius * collider.transform.localScale.x * base.transform.localScale.x;
	}
	public virtual void UpdateMethod_Base()
	{
	}
	protected virtual void UpdateMethod()
	{
	}
	public virtual void MoveInput(float _moveSpeedMag = 1f)
	{
		player.SetBeforeInputTime();
		inputLerp = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetIntervalLerp(player.GetInputInterval());
		moveSpeed = SingletonCustom<MorphingRace_PlayerManager>.Instance.ClampMoveSpeed((int)characterType, inputLerp) * _moveSpeedMag;
	}
	public virtual void MoveNoneInput()
	{
		inputLerp -= Time.deltaTime;
		if (inputLerp < 0f)
		{
			inputLerp = 0f;
		}
		moveSpeed -= Time.deltaTime * SingletonCustom<MorphingRace_PlayerManager>.Instance.GetCorrectionDownDiffMoveSpeed();
		if (moveSpeed < 0f)
		{
			moveSpeed = 0f;
		}
	}
	public virtual void Move()
	{
	}
	public Vector3 GetCanMoveVelocity(Vector3 _velocity)
	{
		Vector3 position = collider.transform.position;
		position.y = collider.bounds.min.y + collider_radius;
		Vector3 position2 = collider.transform.position;
		position2.y = collider.bounds.max.y - collider_radius;
		Debug_Point0 = position;
		Debug_Point1 = position2;
		int num = Physics.OverlapCapsuleNonAlloc(position, position2, collider_radius, arrayOverLapCollider, wallLayerMask);
		if (num > 0)
		{
			GameObject gameObject = null;
			Vector3 colPos = Vector3.zero;
			for (int i = 0; i < num; i++)
			{
				if (arrayOverLapCollider[i].gameObject.tag == "Object")
				{
					gameObject = arrayOverLapCollider[i].gameObject;
					colPos = arrayOverLapCollider[i].ClosestPoint(collider.transform.position);
					break;
				}
			}
			if (gameObject == null)
			{
				gameObject = arrayOverLapCollider[0].gameObject;
				colPos = arrayOverLapCollider[0].ClosestPoint(collider.transform.position);
			}
			UnityEngine.Debug.Log("overlap colObj name : " + gameObject.transform.parent.name);
			return GetAdjustVelocity(colPos, _velocity, gameObject.tag == "Field");
		}
		Debug_Point0_CheckObstacle = position + _velocity.normalized * collider_radius;
		Debug_Point1_CheckObstacle = position2 + _velocity.normalized * collider_radius;
		num = Physics.CapsuleCastNonAlloc(position, position2, collider_radius, _velocity.normalized, arrayRaycastHit, collider_radius, wallLayerMask);
		if (num > 0)
		{
			GameObject gameObject2 = null;
			Vector3 colPos2 = Vector3.zero;
			for (int j = 0; j < num; j++)
			{
				if (arrayRaycastHit[j].collider.gameObject.tag == "Object")
				{
					gameObject2 = arrayRaycastHit[j].collider.gameObject;
					colPos2 = arrayRaycastHit[j].point;
					break;
				}
			}
			if (gameObject2 == null)
			{
				gameObject2 = arrayRaycastHit[0].collider.gameObject;
				colPos2 = arrayRaycastHit[0].point;
			}
			UnityEngine.Debug.Log("ray colObj name : " + gameObject2.transform.parent.name);
			return GetAdjustVelocity(colPos2, _velocity, gameObject2.tag == "Field");
		}
		return _velocity;
	}
	private Vector3 GetAdjustVelocity(Vector3 _colPos, Vector3 _velocity, bool _isHeightCheck)
	{
		if (_isHeightCheck)
		{
			Vector3 normalized = (_colPos - collider.transform.position).normalized;
			float num = Mathf.Atan2(normalized.z, normalized.y) * 57.29578f;
			UnityEngine.Debug.Log("angle " + num.ToString());
			if (num < 45f || num > 135f)
			{
				_velocity.y = 0f;
			}
		}
		else
		{
			Vector3 normalized = (_colPos - collider.transform.position).normalized;
			float num = Mathf.Atan2(normalized.z, normalized.x) * 57.29578f;
			UnityEngine.Debug.Log("angle " + num.ToString());
			if (num > 90f)
			{
				if (num > 135f)
				{
					_velocity.x = 0f;
				}
				else
				{
					switch (characterType)
					{
					case MorphingRace_FieldManager.TargetPrefType.Mouse:
					case MorphingRace_FieldManager.TargetPrefType.Fish:
						_velocity.z = 0f;
						break;
					case MorphingRace_FieldManager.TargetPrefType.Eagle:
						_velocity.z *= 0.5f;
						break;
					}
				}
			}
			else if (num < 90f)
			{
				if (num < 45f)
				{
					_velocity.x = 0f;
				}
				else
				{
					switch (characterType)
					{
					case MorphingRace_FieldManager.TargetPrefType.Mouse:
					case MorphingRace_FieldManager.TargetPrefType.Fish:
						_velocity.z = 0f;
						break;
					case MorphingRace_FieldManager.TargetPrefType.Eagle:
						_velocity.z *= 0.5f;
						break;
					}
				}
			}
			else
			{
				switch (characterType)
				{
				case MorphingRace_FieldManager.TargetPrefType.Mouse:
				case MorphingRace_FieldManager.TargetPrefType.Fish:
					_velocity.z = 0f;
					break;
				case MorphingRace_FieldManager.TargetPrefType.Eagle:
					_velocity.z *= 0.5f;
					break;
				}
			}
		}
		return _velocity;
	}
	public bool CheckObstacle(Vector3 _dir, float _distance)
	{
		if (Physics.OverlapSphereNonAlloc(collider.transform.position, collider_radius, arrayOverLapCollider, wallLayerMask) > 0)
		{
			return true;
		}
		if (Physics.SphereCastNonAlloc(collider.transform.position, collider_radius, _dir, arrayRaycastHit, _distance, wallLayerMask) > 0)
		{
			return true;
		}
		return false;
	}
	protected virtual void Rot(Vector3 _vec, bool _isFrontObstacle = false, bool _immediate = false)
	{
		MorphingRace_FieldManager.TargetPrefType targetPrefType = characterType;
		if ((uint)targetPrefType <= 1u || targetPrefType == MorphingRace_FieldManager.TargetPrefType.Dog)
		{
			_vec.y = 0f;
		}
		if (_isFrontObstacle)
		{
			_vec.y = 0f;
		}
		Quaternion quaternion = Quaternion.LookRotation(_vec);
		if (_immediate)
		{
			base.transform.rotation = quaternion;
		}
		else
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, quaternion, Time.deltaTime * ((characterType == MorphingRace_FieldManager.TargetPrefType.Human) ? 20f : 10f));
		}
	}
	public void InOutWaterRot(Vector3 _vec, bool _immediate = false)
	{
		Quaternion quaternion = Quaternion.LookRotation(_vec);
		if (_immediate)
		{
			base.transform.rotation = quaternion;
		}
		else
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, quaternion, Time.deltaTime * 5f);
		}
	}
	public virtual void AnimationFinish()
	{
	}
	public virtual void SetMorphingCharacter()
	{
		base.transform.localEulerAngles = Vector3.zero;
	}
	public virtual void StopMove()
	{
		rigid.velocity = Vector3.zero;
		inputLerp = 0f;
		moveSpeed = 0f;
	}
	public virtual void MorphingInit()
	{
	}
	public virtual void MoveAfterGoal(Vector3 _vec, float _lerp)
	{
	}
	public MorphingRace_Player GetPlayer()
	{
		return player;
	}
	public MorphingRace_FieldManager.TargetPrefType GetCharacterType()
	{
		return characterType;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		DrawWireCapsule(Debug_Point0, Debug_Point1, collider_radius);
		Gizmos.color = Color.blue;
		DrawWireCapsule(Debug_Point0_CheckObstacle, Debug_Point1_CheckObstacle, collider_radius);
	}
	public static void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
	{
	}
}
