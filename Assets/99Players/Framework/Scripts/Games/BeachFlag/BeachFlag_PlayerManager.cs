using Cinemachine;
using System;
using UnityEngine;
public class BeachFlag_PlayerManager : SingletonCustom<BeachFlag_PlayerManager>
{
	[Serializable]
	public struct UserData
	{
		public BeachFlag_Player player;
		public BeachFlag_Define.UserType userType;
		public int teamNo;
		public bool isPlayer;
	}
	[SerializeField]
	[Header("プレイヤ\u30fc(競争キャラ用)")]
	private BeachFlag_Player[] players = new BeachFlag_Player[2];
	[SerializeField]
	[Header("プレイヤ\u30fc(待ち中のキャラ用)")]
	private BeachFlag_Player[] unplayers = new BeachFlag_Player[2];
	[SerializeField]
	[Header("プレイヤ\u30fcの走行に使用するカ\u30fcト")]
	private CinemachinePath[] carts = new CinemachinePath[2];
	[SerializeField]
	[Header("プレイヤ\u30fcの上に表示するUI")]
	public BeachFlag_PlayerIcon[] Boards = new BeachFlag_PlayerIcon[4];
	private UserData[] userData_Group;
	public BeachFlag_Player[] UnPlayers => unplayers;
	public CinemachinePath[] Carts => carts;
	public BeachFlag_Player[] Players => players;
	public UserData[] UserData_Group => userData_Group;
	public void PlayerGameStart()
	{
	}
	public void Set_UserData()
	{
		players[0].Init((BeachFlag_Define.UserType)BeachFlag_Define.GM.GetTeamData(BeachFlag_GameManager.PositionSideType.LeftSide).memberPlayerNoList[0], _isPlay: true);
		players[1].Init((BeachFlag_Define.UserType)BeachFlag_Define.GM.GetTeamData(BeachFlag_GameManager.PositionSideType.RightSide).memberPlayerNoList[0], _isPlay: true);
		unplayers[0].Init((BeachFlag_Define.UserType)BeachFlag_Define.GM.GetTeamData(BeachFlag_GameManager.PositionSideType.BackSide).memberPlayerNoList[0], _isPlay: false);
		unplayers[1].Init((BeachFlag_Define.UserType)BeachFlag_Define.GM.GetTeamData(BeachFlag_GameManager.PositionSideType.FrontSide).memberPlayerNoList[0], _isPlay: false);
	}
	public void UpdateMethod()
	{
		if (BeachFlag_Define.GM.IsDuringGame())
		{
			for (int i = 0; i < players.Length; i++)
			{
				players[i].UpdateMethod();
			}
			for (int j = 0; j < unplayers.Length; j++)
			{
				unplayers[j].UpdateMethod();
			}
		}
	}
	private void Awake()
	{
	}
	public UserData GetUserData(BeachFlag_Define.UserType _userType)
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
