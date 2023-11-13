using System.Collections.Generic;
using UnityEngine;
public class BeachSoccerPlayerManager : SingletonCustom<BeachSoccerPlayerManager>
{
	[SerializeField]
	[Header("チ\u30fcムAプレイヤ\u30fc配列")]
	private BeachSoccerPlayer[] arrayPlayerTeam0;
	[SerializeField]
	[Header("チ\u30fcムBプレイヤ\u30fc配列")]
	private BeachSoccerPlayer[] arrayPlayerTeam1;
	[SerializeField]
	[Header("足跡アンカ\u30fc親")]
	private Transform footprintAnchorParent;
	[SerializeField]
	[Header("チ\u30fcムマ\u30fcカ\u30fc")]
	private GameObject[] arrayTeamMaker;
	private readonly float SEARCH_ANGLE = 70f;
	private int[] teamColor = new int[2];
	private List<int> tempList = new List<int>();
	private float tempDistance;
	private int tempIdx;
	public List<int> team1 = new List<int>();
	public List<int> team2 = new List<int>();
	private BeachSoccerPlayer thrower;
	public BeachSoccerPlayer[] ArrayPlayerTeamA => arrayPlayerTeam0;
	public BeachSoccerPlayer[] ArrayPlayerTeamB => arrayPlayerTeam1;
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
			arrayPlayerTeam0[l].Position = (BeachSoccerPlayer.PositionNo)l;
		}
		for (int m = 0; m < arrayPlayerTeam1.Length; m++)
		{
			arrayPlayerTeam1[m].Position = (BeachSoccerPlayer.PositionNo)m;
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
		SetKickOff();
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum >= 3)
		{
			for (int num5 = 0; num5 < arrayTeamMaker.Length; num5++)
			{
				arrayTeamMaker[num5].SetActive(value: false);
			}
		}
	}
	public BeachSoccerPlayer GetPlayerAtIdx(int _playerIdx)
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
	public BeachSoccerPlayer UpdatePassTarget(BeachSoccerPlayer _holder, bool _isDistancePriority = false)
	{
		float num = _isDistancePriority ? 15f : 20f;
		float num2 = 40f;
		int index = 0;
		List<BeachSoccerPlayer> list = new List<BeachSoccerPlayer>();
		bool flag = true;
		if (_holder.Position == BeachSoccerPlayer.PositionNo.GK && _holder.IsCpu)
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
					if (arrayPlayerTeam0[m] != _holder && arrayPlayerTeam0[m].Position != BeachSoccerPlayer.PositionNo.GK && Mathf.Abs(Vector3.Angle(_holder.MoveDir, arrayPlayerTeam0[m].transform.position - _holder.transform.position)) <= SEARCH_ANGLE)
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
					if (arrayPlayerTeam1[k] != _holder && arrayPlayerTeam1[k].Position != BeachSoccerPlayer.PositionNo.GK && Mathf.Abs(Vector3.Angle(_holder.MoveDir, arrayPlayerTeam1[k].transform.position - _holder.transform.position)) <= SEARCH_ANGLE)
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
	public void CheckChangePlayer(BeachSoccerPlayer _player)
	{
		List<BeachSoccerPlayer> list = new List<BeachSoccerPlayer>();
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
	public void CheckChangePlayer(BeachSoccerPlayer _player, BeachSoccerPlayer.PositionNo _position)
	{
		List<BeachSoccerPlayer> list = new List<BeachSoccerPlayer>();
		switch (_player.TeamNo)
		{
		case 0:
			for (int j = 0; j < arrayPlayerTeam0.Length; j++)
			{
				if (!arrayPlayerTeam0[j].IsCpu && arrayPlayerTeam0[j].Position != _position)
				{
					list.Add(arrayPlayerTeam0[j]);
				}
			}
			break;
		case 1:
			for (int i = 0; i < arrayPlayerTeam1.Length; i++)
			{
				if (!arrayPlayerTeam1[i].IsCpu && arrayPlayerTeam1[i].Position != _position)
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
	public void ChangePlayerDirect(BeachSoccerPlayer _player, BeachSoccerPlayer _targetPlayer)
	{
		if (_targetPlayer.IsCpu)
		{
			CalcManager.mCalcInt = _targetPlayer.PlayerIdx;
			_targetPlayer.ChangePlayerIdx(_player.PlayerIdx);
			_player.ChangePlayerIdx(CalcManager.mCalcInt);
		}
	}
	public void ChangePlayer(BeachSoccerPlayer _player)
	{
		List<BeachSoccerPlayer> list = new List<BeachSoccerPlayer>();
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
			if (Vector3.Distance(list[k].transform.position, SingletonCustom<BeachSoccerBall>.Instance.transform.position) < num)
			{
				num = Vector3.Distance(list[k].transform.position, SingletonCustom<BeachSoccerBall>.Instance.transform.position);
				index = k;
			}
		}
		CalcManager.mCalcInt = _player.PlayerIdx;
		_player.ChangePlayerIdx(list[index].PlayerIdx);
		list[index].ChangePlayerIdx(CalcManager.mCalcInt);
	}
	public void ChangePlayer(BeachSoccerPlayer _player, Vector3 _pos)
	{
		List<BeachSoccerPlayer> list = new List<BeachSoccerPlayer>();
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
			if (Vector3.Distance(list[k].transform.position, _pos) < num)
			{
				num = Vector3.Distance(list[k].transform.position, _pos);
				index = k;
			}
		}
		CalcManager.mCalcInt = _player.PlayerIdx;
		_player.ChangePlayerIdx(list[index].PlayerIdx);
		list[index].ChangePlayerIdx(CalcManager.mCalcInt);
	}
	public BeachSoccerPlayer GetPlayerAtPosition(int _teamNo, BeachSoccerPlayer.PositionNo _position)
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
	public void SetKickOff()
	{
		switch (SingletonCustom<BeachSoccerGameManager>.Instance.StartKickOffTeamNo)
		{
		case 0:
			for (int k = 0; k < arrayPlayerTeam0.Length; k++)
			{
				arrayPlayerTeam0[k].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team0[k].position;
				arrayPlayerTeam0[k].SetKickOff();
				arrayPlayerTeam0[k].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team0[k].rotation;
				arrayPlayerTeam0[k].ClearFootData();
			}
			for (int l = 0; l < arrayPlayerTeam1.Length; l++)
			{
				arrayPlayerTeam1[l].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team1[l].position;
				arrayPlayerTeam1[l].SetKickOff();
				arrayPlayerTeam1[l].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team1[l].rotation;
				arrayPlayerTeam1[l].ClearFootData();
			}
			break;
		case 1:
			for (int i = 0; i < arrayPlayerTeam0.Length; i++)
			{
				arrayPlayerTeam0[i].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team0[i].position;
				arrayPlayerTeam0[i].SetKickOff();
				arrayPlayerTeam0[i].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team0[i].rotation;
				arrayPlayerTeam0[i].ClearFootData();
			}
			for (int j = 0; j < arrayPlayerTeam1.Length; j++)
			{
				arrayPlayerTeam1[j].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team1[j].position;
				arrayPlayerTeam1[j].SetKickOff();
				arrayPlayerTeam1[j].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team1[j].rotation;
				arrayPlayerTeam1[j].ClearFootData();
			}
			break;
		}
		BeachSoccerPlayer playerAtPosition = GetPlayerAtPosition(0, BeachSoccerPlayer.PositionNo.FW);
		if (playerAtPosition.IsCpu)
		{
			CheckChangePlayer(playerAtPosition);
		}
		BeachSoccerPlayer playerAtPosition2 = GetPlayerAtPosition(1, BeachSoccerPlayer.PositionNo.FW);
		if (playerAtPosition2.IsCpu)
		{
			CheckChangePlayer(playerAtPosition2);
		}
		playerAtPosition = GetPlayerAtPosition(0, BeachSoccerPlayer.PositionNo.LMF);
		if (playerAtPosition.IsCpu)
		{
			CheckChangePlayer(playerAtPosition, BeachSoccerPlayer.PositionNo.FW);
		}
		playerAtPosition2 = GetPlayerAtPosition(1, BeachSoccerPlayer.PositionNo.LMF);
		if (playerAtPosition2.IsCpu)
		{
			CheckChangePlayer(playerAtPosition2, BeachSoccerPlayer.PositionNo.FW);
		}
		SingletonCustom<BeachSoccerCameraMover>.Instance.SetFixPos();
	}
	public void SetThrowIn()
	{
		switch (SingletonCustom<BeachSoccerGameManager>.Instance.StartThrowInTeamNo)
		{
		case 0:
			if (SingletonCustom<BeachSoccerBall>.Instance.PosLineOut.x < SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.x)
			{
				for (int m = 0; m < arrayPlayerTeam0.Length; m++)
				{
					arrayPlayerTeam0[m].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorDefenceTeam0[m].position;
					arrayPlayerTeam0[m].ClearFootData();
				}
				for (int n = 0; n < arrayPlayerTeam1.Length; n++)
				{
					arrayPlayerTeam1[n].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorAttackTeam1[n].position;
					arrayPlayerTeam1[n].ClearFootData();
				}
			}
			else
			{
				for (int num = 0; num < arrayPlayerTeam0.Length; num++)
				{
					arrayPlayerTeam0[num].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorAttackTeam0[num].position;
					arrayPlayerTeam0[num].ClearFootData();
				}
				for (int num2 = 0; num2 < arrayPlayerTeam1.Length; num2++)
				{
					arrayPlayerTeam1[num2].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorDefenceTeam1[num2].position;
					arrayPlayerTeam1[num2].ClearFootData();
				}
			}
			thrower = arrayPlayerTeam0[1];
			thrower.SetThrowIn();
			break;
		case 1:
			if (SingletonCustom<BeachSoccerBall>.Instance.PosLineOut.x < SingletonCustom<BeachSoccerFieldManager>.Instance.Center.position.x)
			{
				for (int i = 0; i < arrayPlayerTeam0.Length; i++)
				{
					arrayPlayerTeam0[i].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorDefenceTeam0[i].position;
					arrayPlayerTeam0[i].ClearFootData();
				}
				for (int j = 0; j < arrayPlayerTeam1.Length; j++)
				{
					arrayPlayerTeam1[j].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorAttackTeam1[j].position;
					arrayPlayerTeam1[j].ClearFootData();
				}
			}
			else
			{
				for (int k = 0; k < arrayPlayerTeam0.Length; k++)
				{
					arrayPlayerTeam0[k].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorAttackTeam0[k].position;
					arrayPlayerTeam0[k].ClearFootData();
				}
				for (int l = 0; l < arrayPlayerTeam1.Length; l++)
				{
					arrayPlayerTeam1[l].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorDefenceTeam1[l].position;
					arrayPlayerTeam1[l].ClearFootData();
				}
			}
			thrower = arrayPlayerTeam1[1];
			thrower.SetThrowIn();
			break;
		}
		BeachSoccerPlayer playerAtPosition = GetPlayerAtPosition(0, BeachSoccerPlayer.PositionNo.LMF);
		if (playerAtPosition.IsCpu)
		{
			CheckChangePlayer(playerAtPosition);
		}
		BeachSoccerPlayer playerAtPosition2 = GetPlayerAtPosition(1, BeachSoccerPlayer.PositionNo.LMF);
		if (playerAtPosition2.IsCpu)
		{
			CheckChangePlayer(playerAtPosition2);
		}
	}
	public void SetCornerKick()
	{
		switch (SingletonCustom<BeachSoccerGameManager>.Instance.StartCornerKickTeamNo)
		{
		case 0:
			for (int k = 0; k < arrayPlayerTeam0.Length; k++)
			{
				arrayPlayerTeam0[k].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team0[k].position;
				arrayPlayerTeam0[k].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team0[k].rotation;
				arrayPlayerTeam0[k].ClearFootData();
			}
			for (int l = 0; l < arrayPlayerTeam1.Length; l++)
			{
				arrayPlayerTeam1[l].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team1[l].position;
				arrayPlayerTeam1[l].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team1[l].rotation;
				arrayPlayerTeam1[l].ClearFootData();
			}
			thrower = arrayPlayerTeam0[1];
			thrower.SetCornerKick();
			break;
		case 1:
			for (int i = 0; i < arrayPlayerTeam0.Length; i++)
			{
				arrayPlayerTeam0[i].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team0[i].position;
				arrayPlayerTeam0[i].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team0[i].rotation;
				arrayPlayerTeam0[i].ClearFootData();
			}
			for (int j = 0; j < arrayPlayerTeam1.Length; j++)
			{
				arrayPlayerTeam1[j].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team1[j].position;
				arrayPlayerTeam1[j].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team1[j].rotation;
				arrayPlayerTeam1[j].ClearFootData();
			}
			thrower = arrayPlayerTeam1[1];
			thrower.SetCornerKick();
			break;
		}
		BeachSoccerPlayer playerAtPosition = GetPlayerAtPosition(SingletonCustom<BeachSoccerGameManager>.Instance.StartCornerKickTeamNo, BeachSoccerPlayer.PositionNo.LMF);
		if (playerAtPosition.IsCpu)
		{
			CheckChangePlayer(playerAtPosition);
		}
		SingletonCustom<BeachSoccerCameraMover>.Instance.SetFixPos();
	}
	public void SetGoalClearance()
	{
		switch (SingletonCustom<BeachSoccerGameManager>.Instance.StartThrowInTeamNo)
		{
		case 0:
			for (int k = 0; k < arrayPlayerTeam0.Length; k++)
			{
				arrayPlayerTeam0[k].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team0[k].position;
				arrayPlayerTeam0[k].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team0[k].rotation;
				arrayPlayerTeam0[k].ClearFootData();
			}
			for (int l = 0; l < arrayPlayerTeam1.Length; l++)
			{
				arrayPlayerTeam1[l].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team1[l].position;
				arrayPlayerTeam1[l].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack0_Team1[l].rotation;
				arrayPlayerTeam1[l].ClearFootData();
			}
			thrower = arrayPlayerTeam0[4];
			SingletonCustom<BeachSoccerBall>.Instance.SetHolder(thrower);
			thrower.SetGoalClearance();
			break;
		case 1:
			for (int i = 0; i < arrayPlayerTeam0.Length; i++)
			{
				arrayPlayerTeam0[i].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team0[i].position;
				arrayPlayerTeam0[i].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team0[i].rotation;
				arrayPlayerTeam0[i].ClearFootData();
			}
			for (int j = 0; j < arrayPlayerTeam1.Length; j++)
			{
				arrayPlayerTeam1[j].transform.position = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team1[j].position;
				arrayPlayerTeam1[j].transform.rotation = SingletonCustom<BeachSoccerFieldManager>.Instance.AnchorKickOffAttack1_Team1[j].rotation;
				arrayPlayerTeam1[j].ClearFootData();
			}
			thrower = arrayPlayerTeam1[4];
			SingletonCustom<BeachSoccerBall>.Instance.SetHolder(thrower);
			thrower.SetGoalClearance();
			break;
		}
		BeachSoccerPlayer playerAtPosition = GetPlayerAtPosition(SingletonCustom<BeachSoccerGameManager>.Instance.StartThrowInTeamNo, BeachSoccerPlayer.PositionNo.GK);
		if (playerAtPosition.IsCpu)
		{
			CheckChangePlayer(playerAtPosition);
		}
		SingletonCustom<BeachSoccerCameraMover>.Instance.SetFixPos();
	}
	public void OnKickOff()
	{
		for (int i = 0; i < arrayPlayerTeam0.Length; i++)
		{
			arrayPlayerTeam0[i].OnKickOff();
		}
		for (int j = 0; j < arrayPlayerTeam1.Length; j++)
		{
			arrayPlayerTeam1[j].OnKickOff();
		}
	}
	public void UpdateMethod()
	{
		if (SingletonCustom<BeachSoccerGameManager>.Instance.CurrentState == BeachSoccerGameManager.State.InGame)
		{
			for (int i = 0; i < arrayPlayerTeam0.Length; i++)
			{
				arrayPlayerTeam0[i].UpdateMethod();
			}
			for (int j = 0; j < arrayPlayerTeam1.Length; j++)
			{
				arrayPlayerTeam1[j].UpdateMethod();
			}
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
		}
		for (int m = 0; m < arrayPlayerTeam0.Length; m++)
		{
			arrayPlayerTeam0[m].UpdateAlways();
		}
		for (int n = 0; n < arrayPlayerTeam1.Length; n++)
		{
			arrayPlayerTeam1[n].UpdateAlways();
		}
		for (int num = 0; num < arrayTeamMaker.Length; num++)
		{
			if (num < arrayPlayerTeam0.Length)
			{
				arrayTeamMaker[num].transform.SetPositionX(arrayPlayerTeam0[num].transform.position.x);
				arrayTeamMaker[num].transform.SetPositionZ(arrayPlayerTeam0[num].transform.position.z);
			}
			else
			{
				arrayTeamMaker[num].transform.SetPositionX(arrayPlayerTeam1[num - arrayPlayerTeam0.Length].transform.position.x);
				arrayTeamMaker[num].transform.SetPositionZ(arrayPlayerTeam1[num - arrayPlayerTeam0.Length].transform.position.z);
			}
		}
	}
	private void UpdatePuckChase()
	{
		tempDistance = 9999f;
		tempIdx = 0;
		for (int i = 0; i < arrayPlayerTeam0.Length; i++)
		{
			if (arrayPlayerTeam0[i].Position != BeachSoccerPlayer.PositionNo.GK && Vector3.Distance(arrayPlayerTeam0[i].transform.position, SingletonCustom<BeachSoccerBall>.Instance.transform.position) < tempDistance)
			{
				tempDistance = Vector3.Distance(arrayPlayerTeam0[i].transform.position, SingletonCustom<BeachSoccerBall>.Instance.transform.position);
				tempIdx = i;
			}
		}
		for (int j = 0; j < arrayPlayerTeam0.Length; j++)
		{
			arrayPlayerTeam0[j].IsBallChase = (j == tempIdx);
		}
		tempDistance = 9999f;
		tempIdx = 0;
		for (int k = 0; k < arrayPlayerTeam1.Length; k++)
		{
			if (arrayPlayerTeam1[k].Position != BeachSoccerPlayer.PositionNo.GK && Vector3.Distance(arrayPlayerTeam1[k].transform.position, SingletonCustom<BeachSoccerBall>.Instance.transform.position) < tempDistance)
			{
				tempDistance = Vector3.Distance(arrayPlayerTeam1[k].transform.position, SingletonCustom<BeachSoccerBall>.Instance.transform.position);
				tempIdx = k;
			}
		}
		for (int l = 0; l < arrayPlayerTeam1.Length; l++)
		{
			arrayPlayerTeam1[l].IsBallChase = (l == tempIdx);
		}
	}
	public bool IsOpponentCatch(int _teamNo)
	{
		switch (_teamNo)
		{
		case 0:
			for (int j = 0; j < arrayPlayerTeam1.Length; j++)
			{
				if (arrayPlayerTeam1[j].Position == BeachSoccerPlayer.PositionNo.GK && arrayPlayerTeam1[j].CurrentState == BeachSoccerPlayer.State.CATCH)
				{
					return true;
				}
			}
			break;
		case 1:
			for (int i = 0; i < arrayPlayerTeam0.Length; i++)
			{
				if (arrayPlayerTeam0[i].Position == BeachSoccerPlayer.PositionNo.GK && arrayPlayerTeam0[i].CurrentState == BeachSoccerPlayer.State.CATCH)
				{
					return true;
				}
			}
			break;
		}
		return false;
	}
	public Transform GetFootprintAnchorParent()
	{
		return footprintAnchorParent;
	}
}
