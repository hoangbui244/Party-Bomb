using System;
using System.Collections.Generic;
using UnityEngine;
public class Takoyaki_PlayerManager : SingletonCustom<Takoyaki_PlayerManager>
{
	[Serializable]
	public struct UserData
	{
		public Takoyaki_Player player;
		public Takoyaki_Define.UserType userType;
		public Takoyaki_Define.TeamType teamType;
		public Camera camera;
		public int takoyakiCnt;
		public bool isPlayer;
	}
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private Takoyaki_Player[] players;
	[SerializeField]
	[Header("カメラ")]
	private Camera[] cameras;
	private UserData[] userData_Group1;
	private UserData[] userData_Group2;
	private bool isGroup1Playing = true;
	private static readonly Rect[] SINGLE_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0f, 0.666f, 1f),
		new Rect(0.667f, 0.667f, 0.333f, 0.333f),
		new Rect(0.667f, 0.3335f, 0.333f, 0.333f),
		new Rect(0.667f, 0f, 0.333f, 0.333f)
	};
	private static readonly Rect[] MULTI_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0.5f, 0.5f, 0.5f),
		new Rect(0.5f, 0.5f, 0.5f, 0.5f),
		new Rect(0f, 0f, 0.5f, 0.5f),
		new Rect(0.5f, 0f, 0.5f, 0.5f)
	};
	public void Init()
	{
		Init_UserData();
		if (Takoyaki_Define.PLAYER_NUM == 1)
		{
			for (int i = 0; i < cameras.Length; i++)
			{
				cameras[i].rect = SINGLE_CAMERA_RECT[i];
			}
		}
		else
		{
			for (int j = 0; j < cameras.Length; j++)
			{
				cameras[j].rect = MULTI_CAMERA_RECT[j];
			}
		}
	}
	private void Init_UserData()
	{
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			userData_Group1 = new UserData[Takoyaki_Define.MAX_JOIN_MEMBER_NUM];
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].player = players[i];
				if (Takoyaki_Define.PLAYER_NUM == 3 && i == userData_Group1.Length - 1)
				{
					userData_Group1[i].userType = Takoyaki_Define.UserType.CPU_1;
				}
				else
				{
					userData_Group1[i].userType = (Takoyaki_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
				}
				userData_Group1[i].teamType = (Takoyaki_Define.TeamType)i;
				userData_Group1[i].takoyakiCnt = 0;
				userData_Group1[i].camera = cameras[i];
				userData_Group1[i].isPlayer = (userData_Group1[i].userType <= Takoyaki_Define.UserType.PLAYER_4);
				userData_Group1[i].player.Init(userData_Group1[i].userType, userData_Group1[i].teamType);
			}
			Takoyaki_Define.UIM.SetUserUIData(userData_Group1);
		}
		else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			if (Takoyaki_Define.PLAYER_NUM > 2)
			{
				userData_Group1 = new UserData[Takoyaki_Define.MAX_JOIN_MEMBER_NUM];
				userData_Group2 = new UserData[Takoyaki_Define.MAX_JOIN_MEMBER_NUM];
			}
			else
			{
				userData_Group1 = new UserData[Takoyaki_Define.MAX_JOIN_MEMBER_NUM];
			}
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			switch (Takoyaki_Define.PLAYER_NUM)
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
					userData_Group1[m].userType = (Takoyaki_Define.UserType)list[m];
					userData_Group1[m].teamType = Takoyaki_Define.TeamType.TEAM_A;
				}
				else
				{
					if (Takoyaki_Define.PLAYER_NUM == 3)
					{
						userData_Group1[m].userType = ((m == 2) ? Takoyaki_Define.UserType.CPU_2 : Takoyaki_Define.UserType.CPU_3);
					}
					else
					{
						userData_Group1[m].userType = ((m == 2) ? Takoyaki_Define.UserType.CPU_1 : Takoyaki_Define.UserType.CPU_2);
					}
					userData_Group1[m].teamType = Takoyaki_Define.TeamType.TEAM_B;
				}
				userData_Group1[m].player = players[m];
				userData_Group1[m].takoyakiCnt = 0;
				userData_Group1[m].camera = cameras[m];
				userData_Group1[m].isPlayer = (userData_Group1[m].userType <= Takoyaki_Define.UserType.PLAYER_4);
				userData_Group1[m].player.Init(userData_Group1[m].userType, userData_Group1[m].teamType);
			}
			Takoyaki_Define.UIM.SetUserUIData(userData_Group1);
			if (Takoyaki_Define.PLAYER_NUM <= 2)
			{
				return;
			}
			for (int n = 0; n < userData_Group2.Length; n++)
			{
				if (n < 2)
				{
					userData_Group2[n].userType = (Takoyaki_Define.UserType)list2[n];
					userData_Group2[n].teamType = Takoyaki_Define.TeamType.TEAM_A;
				}
				else
				{
					if (Takoyaki_Define.PLAYER_NUM == 3)
					{
						userData_Group2[n].userType = ((n == 2) ? Takoyaki_Define.UserType.CPU_4 : Takoyaki_Define.UserType.CPU_5);
					}
					else
					{
						userData_Group2[n].userType = ((n == 2) ? Takoyaki_Define.UserType.CPU_3 : Takoyaki_Define.UserType.CPU_4);
					}
					userData_Group2[n].teamType = Takoyaki_Define.TeamType.TEAM_B;
				}
				userData_Group2[n].player = players[n];
				userData_Group2[n].takoyakiCnt = 0;
				userData_Group2[n].camera = cameras[n];
				userData_Group2[n].isPlayer = (userData_Group2[n].userType <= Takoyaki_Define.UserType.PLAYER_4);
			}
		}
		else
		{
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat != GS_Define.GameFormat.BATTLE_AND_COOP)
			{
				return;
			}
			userData_Group1 = new UserData[Takoyaki_Define.MAX_JOIN_MEMBER_NUM];
			for (int num = 0; num < userData_Group1.Length; num++)
			{
				if (num < 2)
				{
					userData_Group1[num].userType = (Takoyaki_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][num];
					userData_Group1[num].teamType = Takoyaki_Define.TeamType.TEAM_A;
				}
				else
				{
					userData_Group1[num].userType = (Takoyaki_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1][(num != 2) ? 1 : 0];
					userData_Group1[num].teamType = Takoyaki_Define.TeamType.TEAM_B;
				}
				userData_Group1[num].player = players[num];
				userData_Group1[num].takoyakiCnt = 0;
				userData_Group1[num].camera = cameras[num];
				userData_Group1[num].isPlayer = (userData_Group1[num].userType <= Takoyaki_Define.UserType.PLAYER_4);
				userData_Group1[num].player.Init(userData_Group1[num].userType, userData_Group1[num].teamType);
			}
			Takoyaki_Define.UIM.SetUserUIData(userData_Group1);
		}
	}
	public void UpdateMethod()
	{
		if (!Takoyaki_Define.GM.IsDuringGame())
		{
			return;
		}
		if (CheckNowGroup1Playing())
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].player.UpdateMethod();
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				userData_Group2[j].player.UpdateMethod();
			}
		}
	}
	public UserData GetUserData(Takoyaki_Define.UserType _userType)
	{
		if (CheckNowGroup1Playing())
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (userData_Group1[i].userType == _userType)
				{
					return userData_Group1[i];
				}
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				if (userData_Group2[j].userType == _userType)
				{
					return userData_Group2[j];
				}
			}
		}
		return default(UserData);
	}
	public int GetUserDataLength()
	{
		if (CheckNowGroup1Playing())
		{
			return userData_Group1.Length;
		}
		return userData_Group2.Length;
	}
	public int[] GetAllUserRecordArray(bool _isGroup1 = true)
	{
		int[] array;
		if (_isGroup1)
		{
			array = new int[userData_Group1.Length];
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				array[i] = userData_Group1[i].takoyakiCnt;
			}
		}
		else
		{
			array = new int[userData_Group2.Length];
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				array[j] = userData_Group2[j].takoyakiCnt;
			}
		}
		return array;
	}
	public int[] GetAllUserNoArray(bool _isGroup1 = true)
	{
		int[] array;
		if (_isGroup1)
		{
			array = new int[userData_Group1.Length];
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				array[i] = (int)userData_Group1[i].userType;
			}
		}
		else
		{
			array = new int[userData_Group2.Length];
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				array[j] = (int)userData_Group2[j].userType;
			}
		}
		return array;
	}
	public int GetTeamTotalRecord(Takoyaki_Define.TeamType _teamType)
	{
		int num = 0;
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			if (userData_Group1[i].teamType == _teamType)
			{
				num += userData_Group1[i].takoyakiCnt;
			}
		}
		if (userData_Group2 != null)
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				if (userData_Group2[j].teamType == _teamType)
				{
					num += userData_Group2[j].takoyakiCnt;
				}
			}
		}
		return num;
	}
	public Camera GetUserCamera(Takoyaki_Define.UserType _userType)
	{
		if (CheckNowGroup1Playing())
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (userData_Group1[i].userType == _userType)
				{
					return userData_Group1[i].camera;
				}
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				if (userData_Group2[j].userType == _userType)
				{
					return userData_Group2[j].camera;
				}
			}
		}
		return null;
	}
	public bool CheckNowGroup1Playing()
	{
		return isGroup1Playing;
	}
	public void SetTakoyakiCnt(Takoyaki_Define.UserType _userType, int _addCnt)
	{
		if (CheckNowGroup1Playing())
		{
			int num = 0;
			while (true)
			{
				if (num < userData_Group1.Length)
				{
					if (userData_Group1[num].userType == _userType)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			userData_Group1[num].takoyakiCnt += _addCnt;
			Takoyaki_Define.UIM.SetScore(_userType, userData_Group1[num].takoyakiCnt);
			return;
		}
		int num2 = 0;
		while (true)
		{
			if (num2 < userData_Group2.Length)
			{
				if (userData_Group2[num2].userType == _userType)
				{
					break;
				}
				num2++;
				continue;
			}
			return;
		}
		userData_Group2[num2].takoyakiCnt += _addCnt;
		Takoyaki_Define.UIM.SetScore(_userType, userData_Group2[num2].takoyakiCnt);
	}
	public void SetPreparationGroup2Data()
	{
		isGroup1Playing = false;
		for (int i = 0; i < userData_Group2.Length; i++)
		{
			userData_Group2[i].player.Init(userData_Group2[i].userType, userData_Group2[i].teamType);
		}
		Takoyaki_Define.UIM.SetUserUIData(userData_Group2);
		Takoyaki_Define.UIM.SetGroupNumber();
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
				if (userData_Group1[i].userType <= Takoyaki_Define.UserType.PLAYER_4)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group1[i].userType);
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].userType <= Takoyaki_Define.UserType.PLAYER_4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group2[j].userType);
			}
		}
	}
	public void SetDebugRecord()
	{
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].takoyakiCnt = UnityEngine.Random.Range(500, 1000);
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				userData_Group2[j].takoyakiCnt = UnityEngine.Random.Range(500, 1000);
			}
		}
	}
}
