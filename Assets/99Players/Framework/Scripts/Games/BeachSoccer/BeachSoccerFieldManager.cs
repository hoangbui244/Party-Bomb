using UnityEngine;
public class BeachSoccerFieldManager : SingletonCustom<BeachSoccerFieldManager>
{
	[SerializeField]
	[Header("中央")]
	private Transform center;
	[SerializeField]
	[Header("審判キャラ位置")]
	private Transform refereeAnchor;
	[SerializeField]
	[Header("ゴ\u30fcル位置")]
	private Transform[] arrayGoal;
	[SerializeField]
	[Header("ゴ\u30fcルアニメ\u30fcション")]
	private Animator[] arrayGoalAnim;
	[SerializeField]
	[Header("プレイヤ\u30fcキックオフ開始位置（中央）チ\u30fcム0攻撃：チ\u30fcム0")]
	private Transform[] arrayAnchorKickOffAttack0_Team0;
	[SerializeField]
	[Header("プレイヤ\u30fcキックオフ開始位置（中央）チ\u30fcム0攻撃：チ\u30fcム1")]
	private Transform[] arrayAnchorKickOffAttack0_Team1;
	[SerializeField]
	[Header("プレイヤ\u30fcキックオフ開始位置（中央）チ\u30fcム1攻撃：チ\u30fcム0")]
	private Transform[] arrayAnchorKcikOffAttack1_Team0;
	[SerializeField]
	[Header("プレイヤ\u30fcキックオフ開始位置（中央）チ\u30fcム1攻撃：チ\u30fcム1")]
	private Transform[] arrayAnchorKickOffAttack1_Team1;
	[SerializeField]
	[Header("攻撃位置：チ\u30fcム0")]
	private Transform[] arrayAnchorAttack_Team0;
	[SerializeField]
	[Header("攻撃位置：チ\u30fcム1")]
	private Transform[] arrayAnchorAttack_Team1;
	[SerializeField]
	[Header("防御位置：チ\u30fcム0")]
	private Transform[] arrayAnchorDefence_Team0;
	[SerializeField]
	[Header("防御位置：チ\u30fcム1")]
	private Transform[] arrayAnchorDefence_Team1;
	[SerializeField]
	[Header("審判移動アンカ\u30fc")]
	private Transform[] arrayRefereeAnchor;
	[SerializeField]
	[Header("スロ\u30fcイン位置アンカ\u30fc（手前）")]
	private Transform throwInAnchorFront;
	[SerializeField]
	[Header("スロ\u30fcイン位置アンカ\u30fc（奥）")]
	private Transform throwInAnchorBack;
	[SerializeField]
	[Header("コ\u30fcナ\u30fcキック位置アンカ\u30fc（チ\u30fcム0）")]
	private Transform[] arrayCornerKickAnchorTeam0;
	[SerializeField]
	[Header("コ\u30fcナ\u30fcキック位置アンカ\u30fc（チ\u30fcム1）")]
	private Transform[] arrayCornerKickAnchorTeam1;
	public static readonly float RINK_Z_SIZE = 4.3f;
	public static readonly float RINK_X_SIZE = 5.8f;
	public Transform Center => center;
	public Transform[] AnchorGoal => arrayGoal;
	public Animator[] ArrayGoalAnim => arrayGoalAnim;
	public Transform AnchorReferee => refereeAnchor;
	public Transform[] ArrayMoveRefereeAnchor => arrayRefereeAnchor;
	public Transform[] AnchorKickOffAttack0_Team0 => arrayAnchorKickOffAttack0_Team0;
	public Transform[] AnchorKickOffAttack0_Team1 => arrayAnchorKickOffAttack0_Team1;
	public Transform[] AnchorKickOffAttack1_Team0 => arrayAnchorKcikOffAttack1_Team0;
	public Transform[] AnchorKickOffAttack1_Team1 => arrayAnchorKickOffAttack1_Team1;
	public Transform ThrowInAnchor_Front => throwInAnchorFront;
	public Transform ThrowInAnchor_Back => throwInAnchorBack;
	public Transform[] AnchorCornerKick_Team0 => arrayCornerKickAnchorTeam0;
	public Transform[] AnchorCornerKick_Team1 => arrayCornerKickAnchorTeam1;
	public Transform[] AnchorAttackTeam0 => arrayAnchorAttack_Team0;
	public Transform[] AnchorAttackTeam1 => arrayAnchorAttack_Team1;
	public Transform[] AnchorDefenceTeam0 => arrayAnchorDefence_Team0;
	public Transform[] AnchorDefenceTeam1 => arrayAnchorDefence_Team1;
	public bool IsCrossTarget(BeachSoccerPlayer _player)
	{
		if (Vector3.Distance(GetOpponentGoalAnchor(_player.TeamNo).position, _player.transform.position) <= 3.7f)
		{
			return true;
		}
		return false;
	}
	public bool IsCrossKicker(BeachSoccerPlayer _player)
	{
		switch (_player.TeamNo)
		{
		case 0:
			if (_player.transform.position.x - center.position.x > RINK_X_SIZE * 0.5f && Mathf.Abs(_player.transform.position.z - center.position.z) > RINK_Z_SIZE * 0.4f)
			{
				return true;
			}
			break;
		case 1:
			if (_player.transform.position.x - center.position.x < (0f - RINK_X_SIZE) * 0.5f && Mathf.Abs(_player.transform.position.z - center.position.z) > RINK_Z_SIZE * 0.5f)
			{
				return true;
			}
			break;
		}
		return false;
	}
	public Transform GetGoalAnchor(int _teamNo)
	{
		return arrayGoal[_teamNo];
	}
	public Transform GetOpponentGoalAnchor(int _teamNo)
	{
		return arrayGoal[(_teamNo == 0) ? 1 : 0];
	}
	public bool IsBehindTheGoal(Vector3 _pos)
	{
		if (_pos.x <= arrayGoal[0].transform.position.x || _pos.x >= arrayGoal[1].transform.position.x)
		{
			return true;
		}
		return false;
	}
	public Transform GetAttackAnchor(BeachSoccerPlayer _player)
	{
		if (_player.TeamNo == 0)
		{
			return arrayAnchorAttack_Team0[(int)_player.Position];
		}
		return arrayAnchorAttack_Team1[(int)_player.Position];
	}
	public Transform GetAttackAnchor(int _teamNo, BeachSoccerPlayer.PositionNo _position)
	{
		if (_teamNo == 0)
		{
			return arrayAnchorAttack_Team0[(int)_position];
		}
		return arrayAnchorAttack_Team1[(int)_position];
	}
	public Transform GetDefenceAnchor(BeachSoccerPlayer _player)
	{
		if (_player.TeamNo == 0)
		{
			return arrayAnchorDefence_Team0[(int)_player.Position];
		}
		return arrayAnchorDefence_Team1[(int)_player.Position];
	}
	public Transform GetDefenceAnchor(int _teamNo, BeachSoccerPlayer.PositionNo _position)
	{
		if (_teamNo == 0)
		{
			return arrayAnchorDefence_Team0[(int)_position];
		}
		return arrayAnchorDefence_Team1[(int)_position];
	}
	public bool IsOpponentPuck(int _teamNo)
	{
		if (Vector3.Distance(SingletonCustom<BeachSoccerBall>.Instance.transform.position, arrayGoal[0].position) > Vector3.Distance(SingletonCustom<BeachSoccerBall>.Instance.transform.position, arrayGoal[1].position))
		{
			if (_teamNo != 0)
			{
				return false;
			}
			return true;
		}
		if (_teamNo != 0)
		{
			return true;
		}
		return false;
	}
	public bool IsOpponentArea(BeachSoccerPlayer _player)
	{
		if (Vector3.Distance(_player.transform.position, arrayGoal[0].position) > Vector3.Distance(_player.transform.position, arrayGoal[1].position))
		{
			if (_player.TeamNo != 0)
			{
				return false;
			}
			return true;
		}
		if (_player.TeamNo != 0)
		{
			return true;
		}
		return false;
	}
	public bool IsTeamPlayerFront(Vector3 _pos, BeachSoccerPlayer _player)
	{
		if (_player.TeamNo == 0)
		{
			if (_player.transform.position.x > _pos.x)
			{
				return true;
			}
		}
		else if (_player.transform.position.x < _pos.x)
		{
			return true;
		}
		return false;
	}
}
