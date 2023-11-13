using UnityEngine;
public class IceHockeyPuck : SingletonCustom<IceHockeyPuck>
{
	[SerializeField]
	[Header("リジッドボティ")]
	private Rigidbody rigid;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private MeshCollider meshCol;
	private IceHockeyPlayer holder;
	private IceHockeyPlayer lastHolder;
	private readonly float SHOOT_SPEED = 3.5f;
	private bool isFaceOffSet;
	private bool isFaceOffRelease;
	private bool isPass;
	private Vector3 start;
	private Vector3 dir;
	public IceHockeyPlayer Holder => holder;
	public IceHockeyPlayer LastHolder => lastHolder;
	public Rigidbody Rigid => rigid;
	public void SetFaceOff()
	{
		holder = (lastHolder = null);
		SingletonCustom<IceHockeyPlayerManager>.Instance.Referee.SetFaceOffKeep();
		rigid.velocity = Vector3.zero;
		rigid.isKinematic = true;
		base.transform.position = SingletonCustom<IceHockeyPlayerManager>.Instance.Referee.PuckCatchAnchor.position;
		isFaceOffSet = true;
	}
	public void ReleaseFaceOff()
	{
		isFaceOffSet = false;
		isFaceOffRelease = true;
		rigid.MovePosition(SingletonCustom<IceHockeyRinkManager>.Instance.Center.position);
	}
	public bool IsHoleder(int _playerIdx)
	{
		if (holder != null && holder.PlayerIdx == _playerIdx)
		{
			return true;
		}
		return false;
	}
	public void SetHolder(IceHockeyPlayer _player)
	{
		if (holder != null)
		{
			holder.LostPuck();
		}
		holder = _player;
		holder.HavePuck();
		rigid.isKinematic = true;
		meshCol.isTrigger = true;
	}
	public void Shoot(Vector3 _dir, float _power)
	{
		rigid.isKinematic = false;
		meshCol.isTrigger = false;
		if (_power >= 0.5f)
		{
			_dir.y += (_power - 0.5f) * 0.5f;
			rigid.velocity = Vector3.zero;
			rigid.AddForce(_dir * Mathf.Clamp(_power * SHOOT_SPEED, 2f, 2.5f), ForceMode.Impulse);
		}
		else
		{
			rigid.velocity = Vector3.zero;
			rigid.AddForce(_dir * Mathf.Clamp(_power * SHOOT_SPEED, 2.2f, 2.75f), ForceMode.Impulse);
		}
		lastHolder = holder;
		holder.LostPuck();
		holder = null;
		SingletonCustom<AudioManager>.Instance.SePlay("se_icehockey_shoot");
	}
	public void Pass(IceHockeyPlayer _target, bool _isThrow = false)
	{
		rigid.isKinematic = false;
		meshCol.isTrigger = false;
		float num = Mathf.Clamp((_target.transform.position - base.transform.position).magnitude * 0.1f, 0.55f, 0.75f);
		rigid.velocity = Vector3.zero;
		if (_isThrow)
		{
			rigid.AddForce((_target.transform.position - base.transform.position).normalized * Mathf.Clamp(num * SHOOT_SPEED, 1.35f, 1.5f), ForceMode.Impulse);
			SingletonCustom<AudioManager>.Instance.SePlay("se_shoot");
		}
		else
		{
			rigid.AddForce((_target.transform.position + _target.MoveDir * _target.Rigid.velocity.magnitude * 0.25f - base.transform.position).normalized * Mathf.Clamp(num * SHOOT_SPEED, 1.75f, 1.85f), ForceMode.Impulse);
			SingletonCustom<AudioManager>.Instance.SePlay("se_icehockey_pass");
		}
		_target.PassDelay = 0.5f;
		lastHolder = holder;
		holder.LostPuck();
		holder = null;
		isPass = true;
		start = base.transform.position;
		dir = (_target.transform.position - base.transform.position).normalized;
		if (isFaceOffRelease)
		{
			isFaceOffRelease = false;
			SingletonCustom<IceHockeyCameraMover>.Instance.SetState(IceHockeyCameraMover.State.PUCK);
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
		if (holder != null)
		{
			if (holder.Position == IceHockeyPlayer.PositionNo.GK)
			{
				rigid.MovePosition(holder.PuckCatchAnchor.position);
			}
			else
			{
				rigid.MovePosition(holder.PuckHolderAnchor.position);
			}
		}
		if (isFaceOffSet)
		{
			base.transform.position = SingletonCustom<IceHockeyPlayerManager>.Instance.Referee.PuckCatchAnchor.position;
		}
		if (isFaceOffRelease)
		{
			rigid.position = Vector3.Slerp(rigid.position, SingletonCustom<IceHockeyRinkManager>.Instance.Center.position, 0.25f);
		}
	}
	private void OnGoal(int _teamNo)
	{
		SingletonCustom<AudioManager>.Instance.VoicePlay("voice_ice_hockey_goal", _loop: false, 0f, 1f, 1f, 0.1f);
		SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_depress");
		SingletonCustom<AudioManager>.Instance.SePlay("se_goal_net");
		SingletonCustom<IceHockeyUIManager>.Instance.PlayGoalEffect();
		SingletonCustom<IceHockeyGameManager>.Instance.OnGoal(_teamNo);
		SingletonCustom<IceHockeyRinkManager>.Instance.ArrayGoalAnim[_teamNo].SetTrigger("Goal");
		if (holder != null)
		{
			holder.LostPuck();
		}
		holder = null;
		rigid.isKinematic = false;
		meshCol.isTrigger = false;
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name.Contains("Goal") || collision.gameObject.name.Contains("Rink"))
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_bar_hit");
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (SingletonCustom<IceHockeyGameManager>.Instance.CurrentState == IceHockeyGameManager.State.InGame && (!(holder != null) || !SingletonCustom<IceHockeyRinkManager>.Instance.IsBehindTheGoal(holder.transform.position)) && (!(holder != null) || holder.Position != IceHockeyPlayer.PositionNo.GK))
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
	}
}
