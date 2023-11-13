using UnityEngine;
public class IceHockeyRinkManager : SingletonCustom<IceHockeyRinkManager>
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
	[Header("プレイヤ\u30fcフェイスオフ開始位置（中央）：チ\u30fcム0")]
	private Transform[] arrayAnchorFaceOff0_Team0;
	[SerializeField]
	[Header("プレイヤ\u30fcフェイスオフ開始位置（中央）：チ\u30fcム1")]
	private Transform[] arrayAnchorFaceOff0_Team1;
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
	public static readonly float RINK_Z_SIZE = 15f;
	public static readonly float RINK_X_SIZE = 32.25f;
	public Transform Center => center;
	public Transform[] AnchorGoal => arrayGoal;
	public Animator[] ArrayGoalAnim => arrayGoalAnim;
	public Transform AnchorReferee => refereeAnchor;
	public Transform[] ArrayMoveRefereeAnchor => arrayRefereeAnchor;
	public Transform[] AnchorFaceOff0_Team0 => arrayAnchorFaceOff0_Team0;
	public Transform[] AnchorFaceOff0_Team1 => arrayAnchorFaceOff0_Team1;
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
	public Transform GetAttackAnchor(IceHockeyPlayer _player)
	{
		if (_player.TeamNo == 0)
		{
			return arrayAnchorAttack_Team0[(int)_player.Position];
		}
		return arrayAnchorAttack_Team1[(int)_player.Position];
	}
	public Transform GetAttackAnchor(int _teamNo, IceHockeyPlayer.PositionNo _position)
	{
		if (_teamNo == 0)
		{
			return arrayAnchorAttack_Team0[(int)_position];
		}
		return arrayAnchorAttack_Team1[(int)_position];
	}
	public Transform GetDefenceAnchor(IceHockeyPlayer _player)
	{
		if (_player.TeamNo == 0)
		{
			return arrayAnchorDefence_Team0[(int)_player.Position];
		}
		return arrayAnchorDefence_Team1[(int)_player.Position];
	}
	public bool IsOpponentPuck(int _teamNo)
	{
		if (Vector3.Distance(SingletonCustom<IceHockeyPuck>.Instance.transform.position, arrayGoal[0].position) > Vector3.Distance(SingletonCustom<IceHockeyPuck>.Instance.transform.position, arrayGoal[1].position))
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
	public bool IsOpponentArea(IceHockeyPlayer _player)
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
	public bool IsTeamPlayerFront(Vector3 _pos, IceHockeyPlayer _player)
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
