using System;
using UnityEngine;
public class GoldfishTarget : MonoBehaviour
{
	private enum State
	{
		Show,
		ScoopAnimation,
		Hide
	}
	private enum MoveType
	{
		Idle,
		Straight,
		Right,
		Left,
		Danger
	}
	private const float TURN_ANGLE_RANGE = 10f;
	private const float NORMAL_SPEED = 0.2f;
	private const float DANGER_SPEED = 0.4f;
	private const float GOLD_TYPE_NORMAL_SPEED = 0.6f;
	private const float GOLD_TYPE_HIGH_SPEED = 0.9f;
	private const float NORMAL_ROT_SPEED = 30f;
	private const float GOLD_TYPE_ROT_SPEED = 90f;
	private const float NORMAL_MOVE_TYPE_CHANGE_INTERVAL = 3f;
	private const float DANGER_MOVE_TYPE_CHANGE_INTERVAL = 1f;
	private const float DANGER_RANGE_CHECK_OFFSET = 0.15f;
	private const float ANIMATION_SPEED_IDLE = 0.5f;
	private const float ANIMATION_SPEED_MOVE = 1f;
	private const float ANIMATION_SPEED_DANGER = 2f;
	private const float ANIMATION_EULER_Z = 80f;
	private State state;
	private MoveType moveType;
	[SerializeField]
	private GoldfishTargetManager.FishType fishType;
	[SerializeField]
	private int point;
	[SerializeField]
	private float maxDamage;
	[SerializeField]
	private float minDamage;
	[SerializeField]
	private Transform modelAnchor;
	[SerializeField]
	private Transform triggerAnchorA;
	[SerializeField]
	private Transform triggerAnchorB;
	[SerializeField]
	private Transform tailAnchor;
	[SerializeField]
	private Rigidbody rigid;
	[SerializeField]
	private MeshRenderer[] renderers;
	[SerializeField]
	private Animator animator;
	[SerializeField]
	private ParticleSystem scoopEffect;
	[SerializeField]
	private ParticleSystem perfectEffect;
	[SerializeField]
	private ParticleSystem goodEffect;
	[SerializeField]
	private ParticleSystem badEffect;
	private int targetNo;
	private int scoopCharaNo;
	private float scoopAngleDir;
	private bool isScooped;
	private float moveTypeTimer;
	private float showTimer;
	private float tailAngleTimer;
	private float animSpeedMag = 1f;
	private int damageEffectNo;
	private Vector3 dangerMoveVec;
	private bool isGoldHighSpeed;
	private Vector2 perlinVec2_ScoopAnimPos;
	private Vector2 perlinVec2_ScoopAnimRot;
	public int Point => point;
	public int TargetNo => targetNo;
	public bool IsScooped => isScooped;
	public bool IsShow => state == State.Show;
	public bool IsHide => state == State.Hide;
	public bool IsCanAiTarget => state == State.Show;
	public bool IsKing => fishType == GoldfishTargetManager.FishType.King;
	public bool IsGold => fishType == GoldfishTargetManager.FishType.Gold;
	public void Init()
	{
		Hide();
		switch (fishType)
		{
		case GoldfishTargetManager.FishType.Normal_Red:
		case GoldfishTargetManager.FishType.Normal_Colorful:
		case GoldfishTargetManager.FishType.Normal_Black:
		case GoldfishTargetManager.FishType.Big_Red:
		case GoldfishTargetManager.FishType.Big_White:
		case GoldfishTargetManager.FishType.Big_Colorful:
		case GoldfishTargetManager.FishType.Big_Black:
		case GoldfishTargetManager.FishType.Gold:
			animSpeedMag = 1f;
			break;
		case GoldfishTargetManager.FishType.Eye_Red:
		case GoldfishTargetManager.FishType.Eye_Black:
		case GoldfishTargetManager.FishType.King:
			animSpeedMag = 0.75f;
			break;
		}
		animator.speed = 0.5f * animSpeedMag;
		perfectEffect.transform.parent = base.transform.parent;
		goodEffect.transform.parent = base.transform.parent;
		badEffect.transform.parent = base.transform.parent;
		perlinVec2_ScoopAnimPos = new Vector2(UnityEngine.Random.Range(-10000f, 10000f), UnityEngine.Random.Range(-10000f, 10000f));
		perlinVec2_ScoopAnimRot = new Vector2(UnityEngine.Random.Range(-10000f, 10000f), UnityEngine.Random.Range(-10000f, 10000f));
	}
	public void UpdateMethod()
	{
		switch (state)
		{
		case State.Hide:
			return;
		case State.Show:
			StayTimer();
			break;
		case State.ScoopAnimation:
			ScoopAnimDirection();
			break;
		}
		switch (moveType)
		{
		case MoveType.Idle:
			IdleUpdate();
			break;
		case MoveType.Straight:
			StraightUpdate();
			break;
		case MoveType.Right:
			RightUpdate();
			break;
		case MoveType.Left:
			LeftUpdate();
			break;
		case MoveType.Danger:
			DangerUpdate();
			break;
		}
	}
	private void IdleUpdate()
	{
		moveTypeTimer += Time.deltaTime;
		if (moveTypeTimer > 3f)
		{
			moveTypeTimer = 0f;
			ChangeRandomMoveType();
		}
	}
	private void StraightUpdate()
	{
		if (SingletonCustom<GoldfishTargetManager>.Instance.CheckUpperPosRange(base.transform.position))
		{
			if (base.transform.forward.x > 0f)
			{
				base.transform.AddLocalEulerAnglesY(30f * Time.deltaTime);
			}
			else
			{
				base.transform.AddLocalEulerAnglesY(-30f * Time.deltaTime);
			}
		}
		MoveForward(0.2f);
		moveTypeTimer += Time.deltaTime;
		if (moveTypeTimer > 3f)
		{
			moveTypeTimer = 0f;
			ChangeRandomMoveType();
		}
	}
	private void RightUpdate()
	{
		MoveRot(30f);
		MoveForward(0.2f);
		moveTypeTimer += Time.deltaTime;
		if (moveTypeTimer > 3f)
		{
			moveTypeTimer = 0f;
			ChangeRandomMoveType();
		}
	}
	private void LeftUpdate()
	{
		MoveRot(-30f);
		MoveForward(0.2f);
		moveTypeTimer += Time.deltaTime;
		if (moveTypeTimer > 3f)
		{
			moveTypeTimer = 0f;
			ChangeRandomMoveType();
		}
	}
	private void DangerUpdate()
	{
		if (SingletonCustom<GoldfishTargetManager>.Instance.CheckUpperPosRange(base.transform.position))
		{
			if (base.transform.forward.x > 0f)
			{
				base.transform.AddLocalEulerAnglesY(30f * Time.deltaTime);
			}
			else
			{
				base.transform.AddLocalEulerAnglesY(-30f * Time.deltaTime);
			}
		}
		else
		{
			Vector3 forward = base.transform.forward;
			Vector3 vector = Vector3.Cross(forward, dangerMoveVec);
			float num = Vector3.Angle(forward, dangerMoveVec) * (float)((!(vector.y < 0f)) ? 1 : (-1));
			base.transform.AddLocalEulerAnglesY(num * 2f * Time.deltaTime);
		}
		MoveForward(0.4f);
		moveTypeTimer += Time.deltaTime;
		if (moveTypeTimer > 1f)
		{
			moveTypeTimer = 0f;
			ChangeRandomMoveType();
		}
	}
	private void MoveForward(float _speed)
	{
		if (IsGold)
		{
			_speed = (isGoldHighSpeed ? 0.9f : 0.6f);
		}
		Vector3 position = base.transform.position;
		if (!SingletonCustom<GoldfishTargetManager>.Instance.CheckInTargetPosRange(position) && !SingletonCustom<GoldfishTargetManager>.Instance.CheckUpperPosRange(position))
		{
			Vector3 vector = SingletonCustom<GoldfishTargetManager>.Instance.GetCenterCreatePos() - base.transform.position;
			base.transform.SetLocalEulerAnglesY(90f - Mathf.Atan2(vector.z, vector.x) * 57.29578f + UnityEngine.Random.Range(-10f, 10f));
			ForceMoveStraight();
		}
		Vector3 velocity = base.transform.forward * _speed;
		rigid.velocity = velocity;
	}
	private void MoveRot(float _rotSpeed)
	{
		if (IsGold)
		{
			_rotSpeed = 90f * (float)((_rotSpeed > 0f) ? 1 : (-1));
		}
		base.transform.AddLocalEulerAnglesY(_rotSpeed * Time.deltaTime);
	}
	private void TailRot(float _rotSpeed)
	{
	}
	public void ForceMoveIdle()
	{
		moveTypeTimer = UnityEngine.Random.Range(0f, 3f);
		ChangeMoveType(MoveType.Idle);
	}
	public void ForceMoveStraight()
	{
		moveTypeTimer = UnityEngine.Random.Range(0f, 3f);
		ChangeMoveType(MoveType.Straight);
	}
	public void ForceMoveDanger(Vector3 _dangerPoint)
	{
		moveTypeTimer = 0f;
		ChangeMoveType(MoveType.Danger);
		Vector3 a = base.transform.position - _dangerPoint;
		a.y = 0f;
		a = a.normalized;
		Vector3 pos = base.transform.position + a * 0.15f;
		if (SingletonCustom<GoldfishTargetManager>.Instance.CheckInCameraRange(pos))
		{
			dangerMoveVec = a;
		}
		else
		{
			dangerMoveVec = SingletonCustom<GoldfishTargetManager>.Instance.GetCameraRangeInDir(pos);
		}
	}
	private void ChangeRandomMoveType()
	{
		if (IsGold)
		{
			isGoldHighSpeed = (UnityEngine.Random.Range(0, 2) == 1);
			ChangeMoveType((MoveType)UnityEngine.Random.Range(1, 4));
		}
		else if (UnityEngine.Random.Range(0, 100) < 50)
		{
			ChangeMoveType((MoveType)UnityEngine.Random.Range(1, 4));
		}
		else
		{
			ChangeMoveType(MoveType.Idle);
		}
	}
	private void ChangeMoveType(MoveType _type)
	{
		moveType = _type;
		rigid.isKinematic = (moveType == MoveType.Idle);
		if (moveType == MoveType.Danger)
		{
			animator.speed = 2f * animSpeedMag;
		}
		else if (moveType == MoveType.Idle)
		{
			animator.speed = 0.5f * animSpeedMag;
		}
		else
		{
			animator.speed = 1f * animSpeedMag;
		}
	}
	public void Show(bool _isStart)
	{
		ResetHitInfo();
		base.gameObject.SetActive(value: true);
		state = State.Show;
		modelAnchor.localPosition = Vector3.zero;
		modelAnchor.localEulerAngles = Vector3.zero;
		base.transform.position = SingletonCustom<GoldfishTargetManager>.Instance.GetRandomCreatePos(!_isStart);
		if (_isStart)
		{
			base.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
			moveTypeTimer = UnityEngine.Random.Range(0f, 3f);
			return;
		}
		Vector3 vector = SingletonCustom<GoldfishTargetManager>.Instance.GetCenterCreatePos() - base.transform.position;
		base.transform.SetLocalEulerAnglesY(90f - Mathf.Atan2(vector.z, vector.x) * 57.29578f + UnityEngine.Random.Range(-10f, 10f));
		ForceMoveIdle();
		LeanTween.delayedCall(UnityEngine.Random.Range(0f, 1f), (Action)delegate
		{
			ForceMoveStraight();
		});
	}
	public void ShowFade()
	{
		Material[] defMats = new Material[renderers.Length];
		for (int i = 0; i < renderers.Length; i++)
		{
			defMats[i] = renderers[i].sharedMaterial;
			Material material = renderers[i].material;
			Color color = material.GetColor("_Color");
			color.a = 0f;
			material.SetColor("_Color", color);
		}
		LeanTween.value(base.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
		{
			for (int k = 0; k < renderers.Length; k++)
			{
				Material material2 = renderers[k].material;
				Color color2 = material2.GetColor("_Color");
				color2.a = _value;
				material2.SetColor("_Color", color2);
			}
		}).setOnComplete((Action)delegate
		{
			for (int j = 0; j < renderers.Length; j++)
			{
				renderers[j].sharedMaterial = defMats[j];
			}
		});
	}
	public void Hide()
	{
		base.gameObject.SetActive(value: false);
		state = State.Hide;
		base.transform.parent = SingletonCustom<GoldfishTargetManager>.Instance.GetGoldfishAnchor();
	}
	private void PlayDamageEffect()
	{
	}
	public bool CheckCanScoop()
	{
		if (!isScooped)
		{
			return base.gameObject.activeSelf;
		}
		return false;
	}
	public void Scoop(Transform _poiAnchor, int _charaNo)
	{
		if (!isScooped)
		{
			isScooped = true;
			state = State.ScoopAnimation;
			moveTypeTimer = 0f;
			ChangeMoveType(MoveType.Idle);
			base.transform.parent = _poiAnchor;
			scoopCharaNo = _charaNo;
			scoopAngleDir = ((Vector3.Dot(base.transform.right, base.transform.position - _poiAnchor.position) > 0f) ? 1 : (-1));
			ScoopAnimStart();
		}
	}
	private void ResetHitInfo()
	{
		isScooped = false;
		showTimer = 0f;
	}
	private void StayTimer()
	{
		showTimer += Time.deltaTime;
	}
	private void ScoopAnimStart()
	{
		scoopEffect.Play();
		LeanTween.value(modelAnchor.gameObject, 0f, 1f, 0.1f).setOnUpdate(delegate(float _value)
		{
			modelAnchor.SetLocalEulerAnglesZ(_value * 80f * scoopAngleDir);
		});
		LeanTween.delayedCall(0.3f, (Action)delegate
		{
			PlayDamageEffect();
			Hide();
		});
	}
	private void ScoopAnimDirection()
	{
		perlinVec2_ScoopAnimPos.x += Time.deltaTime * 15f;
		perlinVec2_ScoopAnimRot.x += Time.deltaTime * 15f;
		float num = Mathf.PerlinNoise(perlinVec2_ScoopAnimPos.x, perlinVec2_ScoopAnimPos.y);
		float num2 = Mathf.PerlinNoise(perlinVec2_ScoopAnimRot.x, perlinVec2_ScoopAnimRot.y);
		modelAnchor.SetLocalPositionZ((num - 0.5f) * 0.02f);
		modelAnchor.SetLocalEulerAnglesY((num2 - 0.5f) * 20f);
	}
	public bool CheckOnTrigger(Vector3 _center, float _radius)
	{
		Vector3 position = triggerAnchorA.position;
		Vector3 position2 = triggerAnchorB.position;
		_center.y = (position.y = (position2.y = 0f));
		return CalcDistPoint2LineSegment(_center, position, position2, _doSqrt: false) < _radius * _radius;
	}
	public bool CheckOnTriggerTail(Vector3 _center, float _radius)
	{
		return (tailAnchor.position - _center).sqrMagnitude < _radius * _radius;
	}
	public float CalcDamage(Vector3 _center, float _radius)
	{
		Vector3 vector = tailAnchor.position - _center;
		vector.y = 0f;
		float num = vector.magnitude / _radius;
		if (num < 0.5f)
		{
			damageEffectNo = 2;
		}
		else if (num < 0.9f)
		{
			damageEffectNo = 1;
		}
		else
		{
			damageEffectNo = 0;
		}
		return Mathf.Lerp(maxDamage, minDamage, num);
	}
	private static float CalcDistPoint2LineSegment(Vector3 _point, Vector3 _lineA, Vector3 _lineB, bool _doSqrt = true)
	{
		float x = _point.x;
		float z = _point.z;
		float x2 = _lineA.x;
		float z2 = _lineA.z;
		float x3 = _lineB.x;
		float z3 = _lineB.z;
		float num = x3 - x2;
		float num2 = z3 - z2;
		float num3 = num * num;
		float num4 = num2 * num2;
		float num5 = num3 + num4;
		float num6 = 0f - (num * (x2 - x) + num2 * (z2 - z));
		if (_doSqrt)
		{
			if (num6 < 0f)
			{
				return Mathf.Sqrt((x2 - x) * (x2 - x) + (z2 - z) * (z2 - z));
			}
			if (num6 > num5)
			{
				return Mathf.Sqrt((x3 - x) * (x3 - x) + (z3 - z) * (z3 - z));
			}
			float num7 = num * (z2 - z) - num2 * (x2 - x);
			return Mathf.Sqrt(num7 * num7 / num5);
		}
		if (num6 < 0f)
		{
			return (x2 - x) * (x2 - x) + (z2 - z) * (z2 - z);
		}
		if (num6 > num5)
		{
			return (x3 - x) * (x3 - x) + (z3 - z) * (z3 - z);
		}
		float num8 = num * (z2 - z) - num2 * (x2 - x);
		return num8 * num8 / num5;
	}
}
