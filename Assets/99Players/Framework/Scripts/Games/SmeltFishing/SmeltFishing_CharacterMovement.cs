using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SmeltFishing_CharacterMovement : MonoBehaviour
{
	[SerializeField]
	private SmeltFishing_CharacterMovementConfig config;
	[SerializeField]
	private NavMeshAgent agent;
	private SmeltFishing_Character character;
	private SmeltFishing_CharacterAnimator animator;
	private Rigidbody rigidbody;
	private Vector3 currentEulerAngles;
	private Vector3 targetEulerAngles;
	private HashSet<SmeltFishing_Character> collisions = new HashSet<SmeltFishing_Character>();
	private SmeltFishing_IcePlate targetIcePlate;
	private SmeltFishing_IcePlate beforeTargetIcePlate;
	private float previousChangeTime;
	private bool isOnceSitDown;
	private Vector3 prevPos;
	private readonly float NONE_MOVE_TIME = 1f;
	private float noneMoveTime;
	public SmeltFishing_IcePlate TargetIcePlate => targetIcePlate;
	public void Init(SmeltFishing_Character target)
	{
		character = target;
		rigidbody = GetComponent<Rigidbody>();
		animator = GetComponent<SmeltFishing_CharacterAnimator>();
		currentEulerAngles = base.transform.eulerAngles;
		animator.Init();
	}
	public void UpdateMethod()
	{
	}
	public void FixedUpdateMethod()
	{
		if (character.IsPlayer)
		{
			CharacterControl();
		}
		else
		{
			CharacterControlFromAI();
		}
	}
	public void SitDown(SmeltFishing_FishingSpace fishingSpace)
	{
		base.transform.position = fishingSpace.SitDownPosition;
		base.transform.SetEulerAnglesY(fishingSpace.SitDownEulerAngleY);
		rigidbody.isKinematic = true;
		animator.SetAnimation(SmeltFishing_CharacterAnimator.Animation.ShitAndFishing);
		isOnceSitDown = true;
	}
	public void StandUp()
	{
		base.transform.SetPositionY(0f);
		rigidbody.isKinematic = false;
		animator.SetAnimation(SmeltFishing_CharacterAnimator.Animation.Wait);
		targetIcePlate = null;
	}
	private void CharacterControl()
	{
		if (character.IsPlayer)
		{
			Vector3 moveVector = SingletonCustom<SmeltFishing_Input>.Instance.GetMoveVector(character.ControlUser);
			Move(moveVector);
			Rotate(moveVector);
		}
	}
	private void Move(Vector3 vector)
	{
		float magnitude = vector.magnitude;
		vector = vector.normalized;
		Vector3 velocity = vector * magnitude * config.MoveSpeed;
		rigidbody.velocity = velocity;
		animator.UpdateWalkAnimation(magnitude);
	}
	private void Rotate(Vector3 vector)
	{
		if (!Mathf.Approximately(vector.sqrMagnitude, 0f))
		{
			float y = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			targetEulerAngles = new Vector3(0f, y, 0f);
			float y2 = Mathf.LerpAngle(currentEulerAngles.y, targetEulerAngles.y, Time.deltaTime * config.RotationSpeed);
			Vector3 eulerAngles = currentEulerAngles = new Vector3(targetEulerAngles.x, y2, targetEulerAngles.z);
			base.transform.eulerAngles = eulerAngles;
		}
	}
	private void CharacterControlFromAI()
	{
		if (character.IsPlayer)
		{
			return;
		}
		if (targetIcePlate == null)
		{
			targetIcePlate = ((!isOnceSitDown) ? SingletonCustom<SmeltFishing_Field>.Instance.GetRandomIcePlateInit(beforeTargetIcePlate) : SingletonCustom<SmeltFishing_Field>.Instance.GetRandomIcePlate(beforeTargetIcePlate, base.transform.position));
			beforeTargetIcePlate = targetIcePlate;
			agent.SetDestination(targetIcePlate.Position);
		}
		if (targetIcePlate.IsUsing)
		{
			targetIcePlate = ((!isOnceSitDown) ? SingletonCustom<SmeltFishing_Field>.Instance.GetRandomIcePlateInit(beforeTargetIcePlate) : SingletonCustom<SmeltFishing_Field>.Instance.GetRandomIcePlate(beforeTargetIcePlate, base.transform.position));
			beforeTargetIcePlate = targetIcePlate;
			agent.SetDestination(targetIcePlate.Position);
		}
		if (Vector3.Distance(prevPos, base.transform.position) < 0.01f)
		{
			noneMoveTime += Time.deltaTime;
			if (noneMoveTime > NONE_MOVE_TIME)
			{
				noneMoveTime = 0f;
				targetIcePlate = SingletonCustom<SmeltFishing_Field>.Instance.GetRandomIcePlateInit(beforeTargetIcePlate);
				beforeTargetIcePlate = targetIcePlate;
				agent.SetDestination(targetIcePlate.Position);
			}
		}
		else
		{
			noneMoveTime = 0f;
		}
		Vector3 cornerPosition = GetCornerPosition();
		Vector3 vector = (cornerPosition - base.transform.position).normalized;
		if (Vector3.Distance(cornerPosition, base.transform.position) < 0.05f)
		{
			vector = Vector3.zero;
		}
		MoveForAI(vector);
		RotateForAI(vector);
		prevPos = base.transform.position;
	}
	private void MoveForAI(Vector3 vector)
	{
		float magnitude = vector.magnitude;
		vector = vector.normalized;
		Vector3 velocity = vector * magnitude * config.MoveSpeed;
		rigidbody.velocity = velocity;
		animator.UpdateWalkAnimation(magnitude);
	}
	private void RotateForAI(Vector3 vector)
	{
		if (!Mathf.Approximately(vector.sqrMagnitude, 0f))
		{
			float y = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			targetEulerAngles = new Vector3(0f, y, 0f);
			float y2 = Mathf.LerpAngle(currentEulerAngles.y, targetEulerAngles.y, Time.deltaTime * config.RotationSpeed);
			Vector3 eulerAngles = currentEulerAngles = new Vector3(targetEulerAngles.x, y2, targetEulerAngles.z);
			base.transform.eulerAngles = eulerAngles;
		}
	}
	private Vector3 GetCornerPosition()
	{
		Vector3 position = base.transform.position;
		position.y = 0f;
		Vector3[] corners = agent.path.corners;
		foreach (Vector3 vector in corners)
		{
			Vector3 a = vector;
			a.y = 0f;
			if (!(Vector3.Distance(a, position) < 0.05f))
			{
				return vector;
			}
		}
		return base.transform.position;
	}
	private void OnCollisionStay(Collision other)
	{
		if (other.gameObject.CompareTag("Player") && !GetComponent<SmeltFishing_Character>().IsPlayer && !(Time.time < previousChangeTime + 2f))
		{
			targetIcePlate = null;
			previousChangeTime = Time.time;
		}
	}
	private void OnCollisionExit(Collision other)
	{
		other.gameObject.CompareTag("Player");
	}
}
