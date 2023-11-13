using System;
using UnityEngine;
public class LegendarySword_PlayerManager : SingletonCustom<LegendarySword_PlayerManager>
{
	[Serializable]
	public struct UserData
	{
		public LegendarySword_Player player;
		public LegendarySword_Define.UserType userType;
		public int teamNo;
		public bool isPlayer;
	}
	[SerializeField]
	[Header("プレイヤ\u30fc(競争キャラ用)")]
	private LegendarySword_Player[] players = new LegendarySword_Player[2];
	[SerializeField]
	[Header("プレイヤ\u30fc(待ち中のキャラ用)")]
	private LegendarySword_Player[] unplayers = new LegendarySword_Player[2];
	[SerializeField]
	[Header("プレイヤ\u30fcの上に表示するUI")]
	public LegendarySword_PlayerIcon Boards;
	private UserData[] userData_Group;
	public LegendarySword_Player[] UnPlayers => unplayers;
	public LegendarySword_Player[] Players => players;
	public UserData[] UserData_Group => userData_Group;
	public void Set_UserData()
	{
		players[0].Init((LegendarySword_Define.UserType)LegendarySword_Define.GM.GetTeamData(LegendarySword_GameManager.PositionSideType.LeftSide).memberPlayerNoList[0], _isPlay: true);
		players[1].Init((LegendarySword_Define.UserType)LegendarySword_Define.GM.GetTeamData(LegendarySword_GameManager.PositionSideType.RightSide).memberPlayerNoList[0], _isPlay: true);
		unplayers[0].Init((LegendarySword_Define.UserType)LegendarySword_Define.GM.GetTeamData(LegendarySword_GameManager.PositionSideType.BackSide).memberPlayerNoList[0], _isPlay: false);
		unplayers[1].Init((LegendarySword_Define.UserType)LegendarySword_Define.GM.GetTeamData(LegendarySword_GameManager.PositionSideType.FrontSide).memberPlayerNoList[0], _isPlay: false);
	}
	public void UpdateMethod()
	{
		if (LegendarySword_Define.GM.IsDuringGame())
		{
			for (int i = 0; i < players.Length; i++)
			{
				players[i].UpdateMethod();
			}
		}
	}
	public UserData GetUserData(LegendarySword_Define.UserType _userType)
	{
		for (int i = 0; i < userData_Group.Length; i++)
		{
			if (userData_Group[i].userType == _userType)
			{
				return userData_Group[i];
			}
		}
		return default(UserData);
	}
	public int[] GetAllUserNoArray(bool _isGroup1 = true)
	{
		int[] array = new int[userData_Group.Length];
		for (int i = 0; i < userData_Group.Length; i++)
		{
			array[i] = (int)userData_Group[i].userType;
		}
		return array;
	}
}
