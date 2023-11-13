using System;
using System.Collections.Generic;
using UnityEngine;
public class FireworksPlayerManager : SingletonCustom<FireworksPlayerManager>
{
	[Serializable]
	public struct UserData
	{
		public FireworksPlayer player;
		public FireworksDefine.UserType userType;
		public FireworksDefine.TeamType teamType;
	}
	[SerializeField]
	[Header("プレイヤ\u30fc配列")]
	private FireworksPlayer[] arrayPlayer;
	private UserData[] userData_Group1;
	private UserData[] userData_Group2;
	private bool isGroup1Playing = true;
	private Vector3[] arrayInitPos;
	public bool IsGroup1Playing => isGroup1Playing;
	public UserData[] UserDataGroup1 => userData_Group1;
	public UserData[] UserDataGroup2 => userData_Group2;
	public void Init()
	{
		arrayInitPos = new Vector3[arrayPlayer.Length];
		for (int i = 0; i < arrayInitPos.Length; i++)
		{
			arrayInitPos[i] = arrayPlayer[i].transform.position;
		}
		Init_UserData();
	}
	public void NextGame()
	{
		isGroup1Playing = false;
		for (int i = 0; i < userData_Group2.Length; i++)
		{
			userData_Group2[i].player.Init(i, userData_Group2[i].userType, userData_Group2[i].teamType);
			userData_Group2[i].player.NextGame(arrayInitPos[i]);
		}
		SingletonCustom<FireworksUIManager>.Instance.SetUserUIData(userData_Group2);
	}
	public void SetGroupVibration()
	{
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum <= 2 || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat != GS_Define.GameFormat.COOP)
		{
			return;
		}
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (userData_Group1[i].userType <= FireworksDefine.UserType.PLAYER_4)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group1[i].userType);
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].userType <= FireworksDefine.UserType.PLAYER_4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group2[j].userType);
			}
		}
	}
	private void Init_UserData()
	{
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			userData_Group1 = new UserData[FireworksDefine.MAX_JOIN_MEMBER_NUM];
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				UnityEngine.Debug.Log("I：" + i.ToString());
				userData_Group1[i].player = arrayPlayer[i];
				if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && i == userData_Group1.Length - 1)
				{
					userData_Group1[i].userType = FireworksDefine.UserType.CPU_1;
				}
				else
				{
					userData_Group1[i].userType = (FireworksDefine.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
				}
				userData_Group1[i].teamType = (FireworksDefine.TeamType)i;
				userData_Group1[i].player.Init(i, userData_Group1[i].userType, userData_Group1[i].teamType);
			}
			SingletonCustom<FireworksUIManager>.Instance.SetUserUIData(userData_Group1);
		}
		else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
			{
				userData_Group1 = new UserData[FireworksDefine.MAX_JOIN_MEMBER_NUM];
				userData_Group2 = new UserData[FireworksDefine.MAX_JOIN_MEMBER_NUM];
			}
			else
			{
				userData_Group1 = new UserData[FireworksDefine.MAX_JOIN_MEMBER_NUM];
			}
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
			case 2:
				list.Add(0);
				list.Add(1);
				break;
			case 3:
				for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0].Count; k++)
				{
					list2.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][k]);
				}
				for (int l = 0; l < 2; l++)
				{
					list.Add(list2[UnityEngine.Random.Range(0, list2.Count - 1)]);
					list2.Remove(list[list.Count - 1]);
				}
				break;
			case 4:
				for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerNum; j++)
				{
					if (UnityEngine.Random.Range(0, 2) == 0)
					{
						if (list.Count == 2)
						{
							list2.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][j]);
						}
						else
						{
							list.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][j]);
						}
					}
					else if (list2.Count == 2)
					{
						list.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][j]);
					}
					else
					{
						list2.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][j]);
					}
				}
				break;
			}
			for (int m = 0; m < userData_Group1.Length; m++)
			{
				if (m < 2)
				{
					userData_Group1[m].userType = (FireworksDefine.UserType)list[m];
					userData_Group1[m].teamType = FireworksDefine.TeamType.TEAM_A;
				}
				else
				{
					if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
					{
						userData_Group1[m].userType = ((m == 2) ? FireworksDefine.UserType.CPU_2 : FireworksDefine.UserType.CPU_3);
					}
					else
					{
						userData_Group1[m].userType = ((m == 2) ? FireworksDefine.UserType.CPU_1 : FireworksDefine.UserType.CPU_2);
					}
					userData_Group1[m].teamType = FireworksDefine.TeamType.TEAM_B;
				}
				userData_Group1[m].player = arrayPlayer[m];
				userData_Group1[m].player.Init(m, userData_Group1[m].userType, userData_Group1[m].teamType);
			}
			SingletonCustom<FireworksUIManager>.Instance.SetUserUIData(userData_Group1);
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum <= 2)
			{
				return;
			}
			for (int n = 0; n < userData_Group2.Length; n++)
			{
				if (n < 2)
				{
					userData_Group2[n].userType = (FireworksDefine.UserType)list2[n];
					userData_Group2[n].teamType = FireworksDefine.TeamType.TEAM_A;
				}
				else
				{
					if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
					{
						userData_Group2[n].userType = ((n == 2) ? FireworksDefine.UserType.CPU_4 : FireworksDefine.UserType.CPU_5);
					}
					else
					{
						userData_Group2[n].userType = ((n == 2) ? FireworksDefine.UserType.CPU_3 : FireworksDefine.UserType.CPU_4);
					}
					userData_Group2[n].teamType = FireworksDefine.TeamType.TEAM_B;
				}
				userData_Group2[n].player = arrayPlayer[n];
			}
		}
		else
		{
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat != GS_Define.GameFormat.BATTLE_AND_COOP)
			{
				return;
			}
			for (int num = 0; num < userData_Group1.Length; num++)
			{
				if (num < 2)
				{
					userData_Group1[num].userType = (FireworksDefine.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][num];
					userData_Group1[num].teamType = FireworksDefine.TeamType.TEAM_A;
				}
				else
				{
					userData_Group1[num].userType = (FireworksDefine.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1][(num != 2) ? 1 : 0];
					userData_Group1[num].teamType = FireworksDefine.TeamType.TEAM_B;
				}
				userData_Group1[num].player = arrayPlayer[num];
				userData_Group1[num].player.Init(num, userData_Group1[num].userType, userData_Group1[num].teamType);
			}
			SingletonCustom<FireworksUIManager>.Instance.SetUserUIData(userData_Group1);
		}
	}
	public FireworksPlayer GetPlayerAtIdx(int _idx)
	{
		return arrayPlayer[_idx];
	}
	public FireworksPlayer[] GetArrayPlayer()
	{
		return arrayPlayer;
	}
	public void UpdateMethod()
	{
		FireworksGameManager.State currentState = SingletonCustom<FireworksGameManager>.Instance.CurrentState;
		if ((uint)(currentState - 1) <= 1u)
		{
			for (int i = 0; i < arrayPlayer.Length; i++)
			{
				arrayPlayer[i].UpdateMethod();
			}
		}
	}
}
