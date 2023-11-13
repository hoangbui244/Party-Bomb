using System.Collections.Generic;
using UnityEngine;
public class IceHockeyPlayerManager : SingletonCustom<IceHockeyPlayerManager>
{
	[SerializeField]
	[Header("チ\u30fcムAプレイヤ\u30fc配列")]
	private IceHockeyPlayer[] arrayPlayerTeam0;
	[SerializeField]
	[Header("チ\u30fcムBプレイヤ\u30fc配列")]
	private IceHockeyPlayer[] arrayPlayerTeam1;
	private readonly float SEARCH_ANGLE = 70f;
	[SerializeField]
	[Header("審判")]
	private IceHockeyReferee referee;
	private int[] teamColor = new int[2];
	private List<int> tempList = new List<int>();
	private float tempDistance;
	private int tempIdx;
	public List<int> team1 = new List<int>();
	public List<int> team2 = new List<int>();
	public IceHockeyPlayer[] ArrayPlayerTeamA => arrayPlayerTeam0;
	public IceHockeyPlayer[] ArrayPlayerTeamB => arrayPlayerTeam1;
	public IceHockeyReferee Referee => referee;
	public int[] ArrayTeamColor => teamColor;
	public void Init()
	{
		tempList.Clear();
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerNum; i++)
		{
			tempList.Add(i);
		}
		CalcManager.mCalcInt = 0;
		for (int j = tempList.Count; j < GS_Define.PLAYER_MAX; j++)
		{
			tempList.Add(SingletonCustom<GameSettingManager>.Instance.PlayerNum + CalcManager.mCalcInt);
			CalcManager.mCalcInt++;
		}
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
		{
			tempList.Shuffle();
		}
		for (int k = tempList.Count; k < arrayPlayerTeam0.Length + arrayPlayerTeam1.Length; k++)
		{
			tempList.Add(SingletonCustom<GameSettingManager>.Instance.PlayerNum + CalcManager.mCalcInt);
			CalcManager.mCalcInt++;
		}
		for (int l = 0; l < arrayPlayerTeam0.Length; l++)
		{
			arrayPlayerTeam0[l].Position = (IceHockeyPlayer.PositionNo)l;
		}
		for (int m = 0; m < arrayPlayerTeam1.Length; m++)
		{
			arrayPlayerTeam1[m].Position = (IceHockeyPlayer.PositionNo)m;
		}
		for (int n = 0; n < tempList.Count; n++)
		{
			if (n % 2 == 0)
			{
				arrayPlayerTeam0[n / 2].Init(tempList[n], 0);
				team1.Add(tempList[n]);
			}
			else
			{
				arrayPlayerTeam1[n / 2].Init(tempList[n], 1);
				team2.Add(tempList[n]);
			}
		}
		team1.Sort();
		team2.Sort();
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
			teamColor[0] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0];
			teamColor[1] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[4];
			break;
		case 2:
			teamColor[0] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0];
			teamColor[1] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[1];
			break;
		default:
			teamColor[0] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[team1[0]];
			teamColor[1] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[team2[0]];
			break;
		}
		if ((teamColor[0] == 3 && teamColor[1] == 7) || (teamColor[0] == 7 && teamColor[1] == 3))
		{
			for (int num = 0; num < team2.Count; num++)
			{
				if (team2[num] >= 0 && team2[num] < GS_Define.CHARACTER_MAX && SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[team2[num]] != 3 && SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[team2[num]] != 7)
				{
					teamColor[1] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[team2[num]];
					break;
				}
			}
		}
		if ((teamColor[0] == 2 && teamColor[1] == 5) || (teamColor[0] == 5 && teamColor[1] == 2))
		{
			for (int num2 = 0; num2 < team2.Count; num2++)
			{
				if (team2[num2] >= 0 && team2[num2] < GS_Define.CHARACTER_MAX && SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[team2[num2]] != 2 && SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[team2[num2]] != 5)
				{
					teamColor[1] = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[team2[num2]];
					break;
				}
			}
		}
		for (int num3 = 0; num3 < arrayPlayerTeam0.Length; num3++)
		{
			arrayPlayerTeam0[num3].SetStyle();
		}
		for (int num4 = 0; num4 < arrayPlayerTeam1.Length; num4++)
		{
			arrayPlayerTeam1[num4].SetStyle();
		}
	}
	public IceHockeyPlayer GetPlayerAtIdx(int _playerIdx)
	{
		for (int i = 0; i < arrayPlayerTeam0.Length; i++)
		{
			if (arrayPlayerTeam0[i].PlayerIdx == _playerIdx)
			{
				return arrayPlayerTeam0[i];
			}
		}
		for (int j = 0; j < arrayPlayerTeam1.Length; j++)
		{
			if (arrayPlayerTeam1[j].PlayerIdx == _playerIdx)
			{
				return arrayPlayerTeam1[j];
			}
		}
		return null;
	}
	public IceHockeyPlayer UpdatePassTarget(IceHockeyPlayer _holder, bool _isDistancePriority = false)
	{
		float num = _isDistancePriority ? 15f : 20f;
		float num2 = 30f;
		int index = 0;
		List<IceHockeyPlayer> list = new List<IceHockeyPlayer>();
		bool flag = false;
		if (_holder.Position == IceHockeyPlayer.PositionNo.GK && _holder.IsCpu)
		{
			flag = true;
			switch (_holder.TeamNo)
			{
			case 0:
				flag = true;
				for (int j = 0; j < arrayPlayerTeam0.Length; j++)
				{
					if (arrayPlayerTeam0[j] != _holder)
					{
						list.Add(arrayPlayerTeam0[j]);
					}
				}
				break;
			case 1:
				flag = true;
				for (int i = 0; i < arrayPlayerTeam1.Length; i++)
				{
					if (arrayPlayerTeam1[i] != _holder)
					{
						list.Add(arrayPlayerTeam1[i]);
					}
				}
				break;
			}
		}
		else
		{
			switch (_holder.TeamNo)
			{
			case 0:
				for (int m = 0; m < arrayPlayerTeam0.Length; m++)
				{
					if (arrayPlayerTeam0[m] != _holder && arrayPlayerTeam0[m].Position != IceHockeyPlayer.PositionNo.GK && Mathf.Abs(Vector3.Angle(_holder.MoveDir, arrayPlayerTeam0[m].transform.position - _holder.transform.position)) <= SEARCH_ANGLE)
					{
						list.Add(arrayPlayerTeam0[m]);
					}
				}
				if (!_isDistancePriority || list.Count != 0)
				{
					break;
				}
				flag = true;
				for (int n = 0; n < arrayPlayerTeam0.Length; n++)
				{
					if (arrayPlayerTeam0[n] != _holder)
					{
						list.Add(arrayPlayerTeam0[n]);
					}
				}
				break;
			case 1:
				for (int k = 0; k < arrayPlayerTeam1.Length; k++)
				{
					if (arrayPlayerTeam1[k] != _holder && arrayPlayerTeam1[k].Position != IceHockeyPlayer.PositionNo.GK && Mathf.Abs(Vector3.Angle(_holder.MoveDir, arrayPlayerTeam1[k].transform.position - _holder.transform.position)) <= SEARCH_ANGLE)
					{
						list.Add(arrayPlayerTeam1[k]);
					}
				}
				if (!_isDistancePriority || list.Count != 0)
				{
					break;
				}
				flag = true;
				for (int l = 0; l < arrayPlayerTeam1.Length; l++)
				{
					if (arrayPlayerTeam1[l] != _holder)
					{
						list.Add(arrayPlayerTeam1[l]);
					}
				}
				break;
			}
		}
		for (int num3 = 0; num3 < arrayPlayerTeam0.Length; num3++)
		{
			arrayPlayerTeam0[num3].ShowTargetCursor(_enable: false);
		}
		for (int num4 = 0; num4 < arrayPlayerTeam1.Length; num4++)
		{
			arrayPlayerTeam1[num4].ShowTargetCursor(_enable: false);
		}
		if (list.Count > 0)
		{
			if (flag)
			{
				for (int num5 = 0; num5 < list.Count; num5++)
				{
					if (Vector3.Distance(_holder.transform.position, list[num5].transform.position) < num)
					{
						num = Vector3.Distance(_holder.transform.position, list[num5].transform.position);
						index = num5;
					}
				}
			}
			else
			{
				for (int num6 = 0; num6 < list.Count; num6++)
				{
					if (Mathf.Abs(Vector3.Angle(_holder.MoveDir, list[num6].transform.position - _holder.transform.position)) < num2)
					{
						num2 = Mathf.Abs(Vector3.Angle(_holder.MoveDir, list[num6].transform.position - _holder.transform.position));
						index = num6;
					}
				}
			}
			if (!_holder.IsCpu)
			{
				list[index].ShowTargetCursor(_enable: true);
			}
			return list[index];
		}
		return null;
	}
	public void CheckChangePlayer(IceHockeyPlayer _player)
	{
		List<IceHockeyPlayer> list = new List<IceHockeyPlayer>();
		switch (_player.TeamNo)
		{
		case 0:
			for (int j = 0; j < arrayPlayerTeam0.Length; j++)
			{
				if (!arrayPlayerTeam0[j].IsCpu)
				{
					list.Add(arrayPlayerTeam0[j]);
				}
			}
			break;
		case 1:
			for (int i = 0; i < arrayPlayerTeam1.Length; i++)
			{
				if (!arrayPlayerTeam1[i].IsCpu)
				{
					list.Add(arrayPlayerTeam1[i]);
				}
			}
			break;
		}
		if (list.Count > 0)
		{
			list.Shuffle();
			CalcManager.mCalcInt = _player.PlayerIdx;
			_player.ChangePlayerIdx(list[0].PlayerIdx);
			list[0].ChangePlayerIdx(CalcManager.mCalcInt);
		}
	}
	public void ChangePlayer(IceHockeyPlayer _player)
	{
		List<IceHockeyPlayer> list = new List<IceHockeyPlayer>();
		switch (_player.TeamNo)
		{
		case 0:
			for (int j = 0; j < arrayPlayerTeam0.Length; j++)
			{
				if (arrayPlayerTeam0[j] != _player && arrayPlayerTeam0[j].IsCpu)
				{
					list.Add(arrayPlayerTeam0[j]);
				}
			}
			break;
		case 1:
			for (int i = 0; i < arrayPlayerTeam1.Length; i++)
			{
				if (arrayPlayerTeam1[i] != _player && arrayPlayerTeam1[i].IsCpu)
				{
					list.Add(arrayPlayerTeam1[i]);
				}
			}
			break;
		}
		if (list.Count <= 0)
		{
			return;
		}
		float num = 9999f;
		int index = 0;
		for (int k = 0; k < list.Count; k++)
		{
			if (Vector3.Distance(list[k].transform.position, SingletonCustom<IceHockeyPuck>.Instance.transform.position) < num)
			{
				num = Vector3.Distance(list[k].transform.position, SingletonCustom<IceHockeyPuck>.Instance.transform.position);
				index = k;
			}
		}
		CalcManager.mCalcInt = _player.PlayerIdx;
		_player.ChangePlayerIdx(list[index].PlayerIdx);
		list[index].ChangePlayerIdx(CalcManager.mCalcInt);
	}
	public IceHockeyPlayer GetPlayerAtPosition(int _teamNo, IceHockeyPlayer.PositionNo _position)
	{
		switch (_teamNo)
		{
		case 0:
			for (int j = 0; j < arrayPlayerTeam0.Length; j++)
			{
				if (arrayPlayerTeam0[j].Position == _position)
				{
					return arrayPlayerTeam0[j];
				}
			}
			break;
		case 1:
			for (int i = 0; i < arrayPlayerTeam1.Length; i++)
			{
				if (arrayPlayerTeam1[i].Position == _position)
				{
					return arrayPlayerTeam1[i];
				}
			}
			break;
		}
		if (_teamNo != 0)
		{
			return arrayPlayerTeam0[(int)_position];
		}
		return arrayPlayerTeam0[(int)_position];
	}
	public void SetFaceOff()
	{
		for (int i = 0; i < arrayPlayerTeam0.Length; i++)
		{
			arrayPlayerTeam0[i].transform.position = SingletonCustom<IceHockeyRinkManager>.Instance.AnchorFaceOff0_Team0[i].position;
			arrayPlayerTeam0[i].SetFaceOff();
		}
		for (int j = 0; j < arrayPlayerTeam1.Length; j++)
		{
			arrayPlayerTeam1[j].transform.position = SingletonCustom<IceHockeyRinkManager>.Instance.AnchorFaceOff0_Team1[j].position;
			arrayPlayerTeam1[j].SetFaceOff();
		}
		IceHockeyPlayer playerAtPosition = GetPlayerAtPosition(0, IceHockeyPlayer.PositionNo.CF);
		if (playerAtPosition.IsCpu)
		{
			CheckChangePlayer(playerAtPosition);
		}
		IceHockeyPlayer playerAtPosition2 = GetPlayerAtPosition(1, IceHockeyPlayer.PositionNo.CF);
		if (playerAtPosition2.IsCpu)
		{
			CheckChangePlayer(playerAtPosition2);
		}
		SingletonCustom<IceHockeyCameraMover>.Instance.SetFixPos();
	}
	public void OnFaceOff()
	{
		for (int i = 0; i < arrayPlayerTeam0.Length; i++)
		{
			arrayPlayerTeam0[i].OnFaceOff();
		}
		for (int j = 0; j < arrayPlayerTeam1.Length; j++)
		{
			arrayPlayerTeam1[j].OnFaceOff();
		}
		referee.OnFaceOff();
	}
	public void UpdateMethod()
	{
		if (SingletonCustom<IceHockeyGameManager>.Instance.CurrentState == IceHockeyGameManager.State.InGame)
		{
			for (int i = 0; i < arrayPlayerTeam0.Length; i++)
			{
				arrayPlayerTeam0[i].UpdateMethod();
			}
			for (int j = 0; j < arrayPlayerTeam1.Length; j++)
			{
				arrayPlayerTeam1[j].UpdateMethod();
			}
			referee.UpdateMethod();
			UpdatePuckChase();
		}
		else
		{
			for (int k = 0; k < arrayPlayerTeam0.Length; k++)
			{
				arrayPlayerTeam0[k].MoveInertia();
			}
			for (int l = 0; l < arrayPlayerTeam1.Length; l++)
			{
				arrayPlayerTeam1[l].MoveInertia();
			}
			referee.UpdateMethod();
		}
		for (int m = 0; m < arrayPlayerTeam0.Length; m++)
		{
			arrayPlayerTeam0[m].UpdateAlways();
		}
		for (int n = 0; n < arrayPlayerTeam1.Length; n++)
		{
			arrayPlayerTeam1[n].UpdateAlways();
		}
	}
	private void UpdatePuckChase()
	{
		tempDistance = 9999f;
		tempIdx = 0;
		for (int i = 0; i < arrayPlayerTeam0.Length; i++)
		{
			if (arrayPlayerTeam0[i].Position != IceHockeyPlayer.PositionNo.GK && Vector3.Distance(arrayPlayerTeam0[i].transform.position, SingletonCustom<IceHockeyPuck>.Instance.transform.position) < tempDistance)
			{
				tempDistance = Vector3.Distance(arrayPlayerTeam0[i].transform.position, SingletonCustom<IceHockeyPuck>.Instance.transform.position);
				tempIdx = i;
			}
		}
		for (int j = 0; j < arrayPlayerTeam0.Length; j++)
		{
			arrayPlayerTeam0[j].IsPuckChase = (j == tempIdx);
		}
		tempDistance = 9999f;
		tempIdx = 0;
		for (int k = 0; k < arrayPlayerTeam1.Length; k++)
		{
			if (arrayPlayerTeam1[k].Position != IceHockeyPlayer.PositionNo.GK && Vector3.Distance(arrayPlayerTeam1[k].transform.position, SingletonCustom<IceHockeyPuck>.Instance.transform.position) < tempDistance)
			{
				tempDistance = Vector3.Distance(arrayPlayerTeam1[k].transform.position, SingletonCustom<IceHockeyPuck>.Instance.transform.position);
				tempIdx = k;
			}
		}
		for (int l = 0; l < arrayPlayerTeam1.Length; l++)
		{
			arrayPlayerTeam1[l].IsPuckChase = (l == tempIdx);
		}
	}
	public bool IsOpponentCatch(int _teamNo)
	{
		switch (_teamNo)
		{
		case 0:
			for (int j = 0; j < arrayPlayerTeam1.Length; j++)
			{
				if (arrayPlayerTeam1[j].Position == IceHockeyPlayer.PositionNo.GK && arrayPlayerTeam1[j].CurrentState == IceHockeyPlayer.State.CATCH)
				{
					return true;
				}
			}
			break;
		case 1:
			for (int i = 0; i < arrayPlayerTeam0.Length; i++)
			{
				if (arrayPlayerTeam0[i].Position == IceHockeyPlayer.PositionNo.GK && arrayPlayerTeam0[i].CurrentState == IceHockeyPlayer.State.CATCH)
				{
					return true;
				}
			}
			break;
		}
		return false;
	}
}
