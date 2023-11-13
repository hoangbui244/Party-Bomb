using System;
using UnityEngine;
public class BeachSoccerBall : SingletonCustom<BeachSoccerBall>
{
	public enum State
	{
		Default,
		KickOff,
		ThrowIn,
		GoalClearance,
		CornerKick
	}
	[SerializeField]
	[Header("リジッドボティ")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private SphereCollider meshCol;
	[SerializeField]
	[Header("トレイル")]
	private TrailRenderer trail;
	[SerializeField]
	[Header("落下地点表示")]
	private GameObject objFallPoint;
	private BeachSoccerPlayer holder;
	private BeachSoccerPlayer lastHolder;
	private readonly float SHOOT_SPEED = 1f;
	private bool isKickOffSet;
	private bool isKickOffRelease;
	private float colliderTriggerTime;
	private Vector3 lineOutPosition;
	private State currentState;
	private bool isPass;
	private Vector3 start;
	private Vector3 dir;
	public State CurrentState => currentState;
	public BeachSoccerPlayer Holder => holder;
	public BeachSoccerPlayer LastHolder
	{
		get
		{
			return lastHolder;
		}
		set
		{
			lastHolder = value;
		}
	}
	public Rigidbody Rigid => rigid;
	public Vector3 PosLineOut => lineOutPosition;
	public void SetKickOff()
	{
		holder = (lastHolder = null);
		rigid.velocity = Vector3.zero;
		rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
		rigid.isKinematic = true;
		base.transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position;
		base.transform.AddLocalPositionY(0.1f);
		isKickOffSet = true;
		trail.enabled = false;
		currentState = State.KickOff;
	}
	public void SetThrowIn()
	{
		base.transform.parent = holder.BallThrowInAnchor;
		base.transform.SetLocalPosition(0f, 0f, 0f);
		SingletonCustom<BeachSoccerCameraMover>.Instance.SetFixPos();
	}
	public void SetCornerKick()
	{
		base.transform.parent = holder.BallHolderAnchor;
		base.transform.SetLocalPosition(0f, 0f, 0f);
		SingletonCustom<BeachSoccerCameraMover>.Instance.SetFixPos();
	}
	public void SetFloatBall()
	{
		base.transform.position = holder.BallHolderAnchor.position;
	}
	public bool IsAir()
	{
		if (holder == null && base.transform.localPosition.y > 0.55f)
		{
			return true;
		}
		return false;
	}
	public bool IsHoleder(int _playerIdx)
	{
		if (holder != null && holder.PlayerIdx == _playerIdx)
		{
			return true;
		}
		return false;
	}
	public void SetHolder(BeachSoccerPlayer _player)
	{
		UnityEngine.Debug.Log("setHolder:" + _player?.ToString());
		if (holder != null)
		{
			holder.LostBall();
		}
		holder = _player;
		holder.HaveBall();
		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		rigid.isKinematic = true;
		meshCol.isTrigger = true;
		base.transform.SetLocalEulerAngles(0f, 0f, 0f);
		if (currentState == State.GoalClearance)
		{
			base.transform.position = holder.BallCatchAnchor.position;
		}
		trail.enabled = false;
	}
	public void ThrowIn(Vector3 _stickDir, float _power)
	{
		LeanTween.delayedCall(0.1f, (Action)delegate
		{
			base.transform.parent = SingletonCustom<BeachSoccerGameManager>.Instance.Root3D;
			LeanTween.delayedCall(0.1f, (Action)delegate
			{
				currentState = State.Default;
				meshCol.isTrigger = false;
			});
			rigid.isKinematic = false;
			rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			_power = Mathf.Clamp(_power * 6f, 3f, 6f);
			rigid.velocity = CalcManager.GetVelocityPositionVec(holder.transform.position, holder.transform.position + _stickDir * _power, _power * 0.2f);
			lastHolder = holder;
			holder.LostBall();
			holder = null;
			trail.enabled = true;
		});
	}
	public void CornerKick(BeachSoccerPlayer _kicker, Vector3 _stickDir, float _power)
	{
		LeanTween.delayedCall(0.1f, (Action)delegate
		{
			base.transform.parent = SingletonCustom<BeachSoccerGameManager>.Instance.Root3D;
			LeanTween.delayedCall(0.1f, (Action)delegate
			{
				currentState = State.Default;
			});
			rigid.isKinematic = false;
			rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			meshCol.isTrigger = false;
			_power = Mathf.Clamp(_power * 12f, 4f, 11f);
			rigid.velocity = CalcManager.GetVelocityPositionVec(holder.transform.position, holder.transform.position + _stickDir * _power, _power * 0.15f);
			if (!_kicker.IsCpu)
			{
				SingletonCustom<BeachSoccerPlayerManager>.Instance.ChangePlayer(_kicker, holder.transform.position + _stickDir * _power * 0.5f);
			}
			lastHolder = holder;
			holder.LostBall();
			holder = null;
			trail.enabled = true;
		});
	}
	public void Shoot(Vector3 _dir, float _power, bool _isAir = false)
	{
		rigid.isKinematic = false;
		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		colliderTriggerTime = 0.05f;
		float d = Mathf.Clamp(_power * SHOOT_SPEED, 0.880000055f, 1.1f);
		rigid.velocity = Vector3.zero;
		rigid.AddForce(_dir * d, ForceMode.Impulse);
		if (!_isAir)
		{
			rigid.AddForce(Vector3.up * (15f + _power * 15f));
		}
		if (holder != null)
		{
			lastHolder = holder;
			holder.LostBall();
			holder = null;
		}
		SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
		trail.enabled = true;
	}
	public void Float(Vector3 _dir, float _power)
	{
		rigid.isKinematic = false;
		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		colliderTriggerTime = 0.05f;
		rigid.velocity = Vector3.zero;
		rigid.AddForce(_dir * _power, ForceMode.Impulse);
		rigid.AddForce(Vector3.up * (15f + _power * 15f));
		lastHolder = holder;
		holder.LostBall();
		holder = null;
		SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
		trail.enabled = true;
	}
	public void Pass(BeachSoccerPlayer _target, bool _isThrow = false, bool _isCross = false)
	{
		rigid.isKinematic = false;
		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		colliderTriggerTime = 0.05f;
		float magnitude = (_target.transform.position - base.transform.position).magnitude;
		float num = Mathf.Clamp(magnitude * 0.1f, 0.55f, 0.75f);
		rigid.velocity = Vector3.zero;
		if (_isThrow)
		{
			num = Mathf.Clamp(num * SHOOT_SPEED, 0.525000036f, 0.555f);
			num += magnitude * 0.05f;
			rigid.AddForce((_target.transform.position - base.transform.position).normalized * num, ForceMode.Impulse);
			SingletonCustom<AudioManager>.Instance.SePlay("se_throw_ball");
			rigid.AddForce(Vector3.up * num * 30f);
			currentState = State.Default;
		}
		else
		{
			num = Mathf.Clamp(num * SHOOT_SPEED, 0.525000036f, 0.555f);
			num += magnitude * 0.05f;
			if (_isCross)
			{
				num *= 0.85f;
			}
			rigid.AddForce((_target.transform.position + _target.MoveDir * _target.Rigid.velocity.magnitude * 0.25f - base.transform.position).normalized * num, ForceMode.Impulse);
			rigid.AddForce(Vector3.up * num * 30f);
			if (_isCross)
			{
				rigid.AddForce(Vector3.up * num * 40f);
			}
			SingletonCustom<AudioManager>.Instance.SePlay("se_kick");
		}
		_target.PassDelay = 0.5f;
		lastHolder = holder;
		holder.LostBall();
		holder = null;
		isPass = true;
		start = base.transform.position;
		dir = (_target.transform.position - base.transform.position).normalized;
		if (isKickOffRelease)
		{
			isKickOffRelease = false;
			SingletonCustom<BeachSoccerCameraMover>.Instance.SetState(BeachSoccerCameraMover.State.BALL);
		}
		trail.enabled = true;
	}
	private void Update()
	{
		if (colliderTriggerTime > 0f)
		{
			colliderTriggerTime -= Time.deltaTime;
			if (colliderTriggerTime <= 0f)
			{
				meshCol.isTrigger = false;
			}
		}
		if (base.transform.position.y <= -10f)
		{
			lineOutPosition = SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position;
			OnThrowIn();
		}
	}
	private void OnDrawGizmos()
	{
		if (isPass)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(start, start + dir * 10f);
		}
	}
	public void FixedUpdate()
	{
		switch (currentState)
		{
		case State.KickOff:
		case State.ThrowIn:
			break;
		case State.Default:
		case State.GoalClearance:
		{
			if (holder != null)
			{
				if (holder.Position == BeachSoccerPlayer.PositionNo.GK)
				{
					rigid.MovePosition(holder.BallCatchAnchor.position);
				}
				else
				{
					rigid.MovePosition(Vector3.Slerp(base.transform.position, holder.BallHolderAnchor.position, 0.5f));
					Quaternion rhs = Quaternion.Euler(Quaternion.Euler(0f, -90f, 0f) * base.transform.forward * 150f * holder.MoveSpeed * Time.fixedDeltaTime);
					rigid.constraints = (RigidbodyConstraints)48;
					rigid.MoveRotation(rigid.rotation * rhs);
				}
			}
			bool isKickOffSet2 = isKickOffSet;
			if (isKickOffRelease)
			{
				rigid.position = Vector3.Slerp(rigid.position, SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position, 0.25f);
			}
			break;
		}
		}
	}
	private void OnGoal(int _teamNo)
	{
		SingletonCustom<AudioManager>.Instance.VoicePlay("voice_ice_hockey_goal", _loop: false, 0f, 1f, 1f, 0.1f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
		SingletonCustom<AudioManager>.Instance.SePlay("se_goal_net");
		SingletonCustom<BeachSoccerUIManager>.Instance.PlayGoalEffect();
		SingletonCustom<BeachSoccerGameManager>.Instance.OnGoal(_teamNo);
		SingletonCustom<BeachSoccerFieldManager>.Instance.ArrayGoalAnim[_teamNo].SetTrigger("Goal");
		if (holder != null)
		{
			holder.LostBall();
		}
		holder = null;
		rigid.isKinematic = false;
		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		meshCol.isTrigger = false;
	}
	public void OnThrowIn()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_short");
		SingletonCustom<AudioManager>.Instance.VoicePlay("voice_throw_in", _loop: false, 0f, 1f, 1f, 0.1f);
		SingletonCustom<BeachSoccerUIManager>.Instance.ShowThorwIn(0f, delegate
		{
		});
		if (holder != null)
		{
			SingletonCustom<BeachSoccerGameManager>.Instance.OnThrowIn(holder.TeamNo);
			holder.LostBall();
		}
		else
		{
			SingletonCustom<BeachSoccerGameManager>.Instance.OnThrowIn(lastHolder.TeamNo);
		}
		holder = null;
		rigid.isKinematic = false;
		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		meshCol.isTrigger = false;
		currentState = State.ThrowIn;
		trail.enabled = false;
	}
	public void OnCornerKick()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_short");
		SingletonCustom<AudioManager>.Instance.VoicePlay("voice_corner_kick", _loop: false, 0f, 1f, 1f, 0.1f);
		SingletonCustom<BeachSoccerUIManager>.Instance.ShowCornerKick(0f, delegate
		{
		});
		int teamNo = (!(lineOutPosition.x > SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.x)) ? 1 : 0;
		if (holder != null)
		{
			holder.LostBall();
		}
		holder = null;
		rigid.isKinematic = false;
		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		meshCol.isTrigger = false;
		SingletonCustom<BeachSoccerGameManager>.Instance.OnCornerKick(teamNo);
		currentState = State.CornerKick;
		trail.enabled = false;
	}
	public void OnGoalClearance()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_short");
		SingletonCustom<AudioManager>.Instance.VoicePlay("voice_goal_clearance", _loop: false, 0f, 1f, 1f, 0.1f);
		SingletonCustom<BeachSoccerUIManager>.Instance.ShowGoalClearance(0f, delegate
		{
		});
		int teamNo = (lineOutPosition.x > SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.x) ? 1 : 0;
		if (holder != null)
		{
			holder.LostBall();
		}
		holder = null;
		rigid.isKinematic = false;
		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		meshCol.isTrigger = false;
		SingletonCustom<BeachSoccerGameManager>.Instance.OnGoalClearance(teamNo);
		currentState = State.GoalClearance;
		trail.enabled = false;
	}
	public void OnKickOff()
	{
		currentState = State.Default;
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name.Contains("Goal"))
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
			SingletonCustom<AudioManager>.Instance.SePlay("se_goalpost");
		}
		if (collision.gameObject.tag.Equals("Field"))
		{
			trail.enabled = false;
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (SingletonCustom<BeachSoccerGameManager>.Instance.CurrentState != BeachSoccerGameManager.State.InGame)
		{
			return;
		}
		if ((!(holder != null) || !SingletonCustom<BeachSoccerFieldManager>.Instance.IsBehindTheGoal(holder.transform.position)) && (!(holder != null) || holder.Position != BeachSoccerPlayer.PositionNo.GK))
		{
			if (other.name.Equals("GoalChecker0"))
			{
				OnGoal(1);
			}
			if (other.name.Equals("GoalChecker1"))
			{
				OnGoal(0);
			}
		}
		if (currentState != 0)
		{
			return;
		}
		if (other.tag == "HorizontalWall")
		{
			UnityEngine.Debug.Log("ラインアウト:" + base.transform.position.ToString());
			lineOutPosition = base.transform.position;
			OnThrowIn();
		}
		if (other.tag == "VerticalWall")
		{
			UnityEngine.Debug.Log("ラインアウト:" + base.transform.position.ToString());
			int num = 0;
			num = ((!(holder != null)) ? lastHolder.TeamNo : holder.TeamNo);
			lineOutPosition = base.transform.position;
			if (((lineOutPosition.x > SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.x) ? 1 : 0) != num)
			{
				OnGoalClearance();
			}
			else
			{
				OnCornerKick();
			}
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if (SingletonCustom<BeachSoccerGameManager>.Instance.CurrentState != BeachSoccerGameManager.State.InGame || currentState != 0)
		{
			return;
		}
		if (other.tag == "HorizontalWall")
		{
			UnityEngine.Debug.Log("ラインアウト:" + base.transform.position.ToString());
			lineOutPosition = base.transform.position;
			OnThrowIn();
		}
		if (other.tag == "VerticalWall")
		{
			UnityEngine.Debug.Log("ラインアウト:" + base.transform.position.ToString());
			int num = 0;
			num = ((!(holder != null)) ? lastHolder.TeamNo : holder.TeamNo);
			lineOutPosition = base.transform.position;
			if (((lineOutPosition.x > SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.x) ? 1 : 0) != num)
			{
				OnGoalClearance();
			}
			else
			{
				OnCornerKick();
			}
		}
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(base.gameObject);
	}
}
