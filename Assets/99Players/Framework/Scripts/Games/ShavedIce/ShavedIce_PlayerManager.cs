using System;
using System.Collections.Generic;
using UnityEngine;
public class ShavedIce_PlayerManager : SingletonCustom<ShavedIce_PlayerManager>
{
	[Serializable]
	public struct UserData
	{
		public ShavedIce_Player player;
		public ShavedIce_Define.UserType userType;
		public ShavedIce_Define.TeamType teamType;
		public float nowHeight;
		public int nowCreateIceCnt;
		public bool isFailed;
		public bool isPlayer;
	}
	[Serializable]
	public struct CameraRectData
	{
		[Header("メインカメラ[0]のRect")]
		public Rect mainCameraRect_0;
		[Header("サブカメラ[0]のRect")]
		public Rect subCameraRect_0;
		[Header("メインカメラ[1]のRect")]
		public Rect mainCameraRect_1;
		[Header("サブカメラ[1]のRect")]
		public Rect subCameraRect_1;
		[Header("メインカメラ[2]のRect")]
		public Rect mainCameraRect_2;
		[Header("サブカメラ[2]のRect")]
		public Rect subCameraRect_2;
		[Header("メインカメラ[3]のRect")]
		public Rect mainCameraRect_3;
		[Header("サブカメラ[3]のRect")]
		public Rect subCameraRect_3;
	}
	[SerializeField]
	[Header("１組目のキャラアンカ\u30fc")]
	private GameObject characters_Group1_Anchor;
	[SerializeField]
	[Header("１組目のキャラクタ\u30fc")]
	private ShavedIce_Player[] characters_Group1;
	[SerializeField]
	[Header("２組目のキャラアンカ\u30fc")]
	private GameObject characters_Group2_Anchor;
	[SerializeField]
	[Header("２組目のキャラクタ\u30fc")]
	private ShavedIce_Player[] characters_Group2;
	[SerializeField]
	[Header("プレイヤ\u30fc１人の時のカメラデ\u30fcタ")]
	private CameraRectData cameraData_1Player;
	[SerializeField]
	[Header("プレイヤ\u30fc２人の時のカメラデ\u30fcタ")]
	private CameraRectData cameraData_2Player;
	[SerializeField]
	[Header("プレイヤ\u30fc３人の時のカメラデ\u30fcタ")]
	private CameraRectData cameraData_3Player;
	[SerializeField]
	[Header("プレイヤ\u30fc４人の時のカメラデ\u30fcタ")]
	private CameraRectData cameraData_4Player;
	private UserData[] userData_Group1;
	private UserData[] userData_Group2;
	private bool isGroup1Playing = true;
	private bool isGroup1HeightCalc = true;
	private bool isIceMachineProcess;
	private const float FX_MOVE_XPOS_LIMIT = 0.35f;
	private float iceFXMoveXPos;
	private float prevIceFXMoveXPos;
	private const float ICE_FX_MOVE_TIME = 0.25f;
	private const float ICE_FX_MAX_SCALE_X = 0.4f;
	private const float ICE_FX_MIN_SCALE_X = 0.1f;
	private bool isIceFXMoveProcess;
	public bool IsGroup1Playing => isGroup1Playing;
	public bool IsIceFXMoveProcess => isIceFXMoveProcess;
	public void Init()
	{
		characters_Group2_Anchor.SetActive(value: false);
		TeamDataInit();
	}
	public void UpdateMethod()
	{
		if (ShavedIce_Define.GM.IsDuringHeightCalc())
		{
			if (isGroup1HeightCalc)
			{
				for (int i = 0; i < userData_Group1.Length; i++)
				{
					userData_Group1[i].player.UpdateHeightCalcLine(_isGroup1: true);
				}
			}
			else
			{
				for (int j = 0; j < userData_Group2.Length; j++)
				{
					userData_Group2[j].player.UpdateHeightCalcLine(_isGroup1: false);
				}
			}
		}
		else
		{
			if (!ShavedIce_Define.GM.IsDuringGame())
			{
				return;
			}
			UpdateIceMachine();
			if (isGroup1Playing)
			{
				for (int k = 0; k < userData_Group1.Length; k++)
				{
					userData_Group1[k].player.UpdateMethod();
				}
			}
			else
			{
				for (int l = 0; l < userData_Group2.Length; l++)
				{
					userData_Group2[l].player.UpdateMethod();
				}
			}
		}
	}
	public bool IsTeamFailed(int _dataNo)
	{
		if (isGroup1Playing)
		{
			return userData_Group1[_dataNo].isFailed;
		}
		return userData_Group2[_dataNo].isFailed;
	}
	public bool IsAllTeamFailed()
	{
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (userData_Group1[i].isPlayer && !userData_Group1[i].isFailed)
				{
					return false;
				}
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				if (userData_Group2[j].isPlayer && !userData_Group2[j].isFailed)
				{
					return false;
				}
			}
		}
		return true;
	}
	public bool IsTeamJoinPlayer(int _dataNo)
	{
		if (isGroup1Playing)
		{
			return userData_Group1[_dataNo].isPlayer;
		}
		return userData_Group2[_dataNo].isPlayer;
	}
	private void TeamDataInit()
	{
		switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat)
		{
		case GS_Define.GameFormat.BATTLE:
			userData_Group1 = new UserData[ShavedIce_Define.MAX_PLAYER_NUM];
			for (int j = 0; j < userData_Group1.Length; j++)
			{
				userData_Group1[j].player = characters_Group1[j];
				if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && j == userData_Group1.Length - 1)
				{
					userData_Group1[j].userType = ShavedIce_Define.UserType.CPU_1;
				}
				else
				{
					userData_Group1[j].userType = (ShavedIce_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][0];
				}
				userData_Group1[j].teamType = (ShavedIce_Define.TeamType)j;
				userData_Group1[j].nowHeight = 0f;
				userData_Group1[j].isFailed = false;
				userData_Group1[j].isPlayer = (userData_Group1[j].userType <= ShavedIce_Define.UserType.PLAYER_4);
			}
			break;
		case GS_Define.GameFormat.COOP:
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
			{
				userData_Group1 = new UserData[ShavedIce_Define.MAX_PLAYER_NUM];
				userData_Group2 = new UserData[ShavedIce_Define.MAX_PLAYER_NUM];
			}
			else
			{
				userData_Group1 = new UserData[ShavedIce_Define.MAX_PLAYER_NUM];
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
				for (int l = 0; l < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0].Count; l++)
				{
					list2.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][l]);
				}
				for (int m = 0; m < 2; m++)
				{
					list.Add(list2[UnityEngine.Random.Range(0, list2.Count - 1)]);
					list2.Remove(list[list.Count - 1]);
				}
				break;
			case 4:
				for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerNum; k++)
				{
					if (UnityEngine.Random.Range(0, 2) == 0)
					{
						if (list.Count == 2)
						{
							list2.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][k]);
						}
						else
						{
							list.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][k]);
						}
					}
					else if (list2.Count == 2)
					{
						list.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][k]);
					}
					else
					{
						list2.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][k]);
					}
				}
				break;
			}
			for (int n = 0; n < userData_Group1.Length; n++)
			{
				if (n < 2)
				{
					userData_Group1[n].userType = (ShavedIce_Define.UserType)list[n];
					userData_Group1[n].teamType = ShavedIce_Define.TeamType.TEAM_A;
				}
				else
				{
					if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
					{
						userData_Group1[n].userType = ((n == 2) ? ShavedIce_Define.UserType.CPU_2 : ShavedIce_Define.UserType.CPU_3);
					}
					else
					{
						userData_Group1[n].userType = ((n == 2) ? ShavedIce_Define.UserType.CPU_1 : ShavedIce_Define.UserType.CPU_2);
					}
					userData_Group1[n].teamType = ShavedIce_Define.TeamType.TEAM_B;
				}
				userData_Group1[n].player = characters_Group1[n];
				userData_Group1[n].nowHeight = 0f;
				userData_Group1[n].isFailed = false;
				userData_Group1[n].isPlayer = (userData_Group1[n].userType <= ShavedIce_Define.UserType.PLAYER_4);
			}
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum <= 2)
			{
				break;
			}
			for (int num = 0; num < userData_Group2.Length; num++)
			{
				if (num < 2)
				{
					userData_Group2[num].userType = (ShavedIce_Define.UserType)list2[num];
					userData_Group2[num].teamType = ShavedIce_Define.TeamType.TEAM_A;
				}
				else
				{
					if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
					{
						userData_Group2[num].userType = ((num == 2) ? ShavedIce_Define.UserType.CPU_4 : ShavedIce_Define.UserType.CPU_5);
					}
					else
					{
						userData_Group2[num].userType = ((num == 2) ? ShavedIce_Define.UserType.CPU_3 : ShavedIce_Define.UserType.CPU_4);
					}
					userData_Group2[num].teamType = ShavedIce_Define.TeamType.TEAM_B;
				}
				userData_Group2[num].player = characters_Group2[num];
				userData_Group2[num].nowHeight = 0f;
				userData_Group2[num].isFailed = false;
				userData_Group2[num].isPlayer = (userData_Group2[num].userType <= ShavedIce_Define.UserType.PLAYER_4);
			}
			break;
		}
		case GS_Define.GameFormat.BATTLE_AND_COOP:
			userData_Group1 = new UserData[ShavedIce_Define.MAX_PLAYER_NUM];
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (i < 2)
				{
					userData_Group1[i].userType = (ShavedIce_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][i];
					userData_Group1[i].teamType = ShavedIce_Define.TeamType.TEAM_A;
				}
				else
				{
					userData_Group1[i].userType = (ShavedIce_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1][(i != 2) ? 1 : 0];
					userData_Group1[i].teamType = ShavedIce_Define.TeamType.TEAM_B;
				}
				userData_Group1[i].player = characters_Group1[i];
				userData_Group1[i].nowHeight = 0f;
				userData_Group1[i].isFailed = false;
				userData_Group1[i].isPlayer = (userData_Group1[i].userType <= ShavedIce_Define.UserType.PLAYER_4);
			}
			break;
		}
		ShavedIce_Define.UIM.Init(userData_Group1, _isGroup1: true);
		for (int num2 = 0; num2 < userData_Group1.Length; num2++)
		{
			userData_Group1[num2].player.Init(num2, userData_Group1[num2].userType, userData_Group1[num2].isPlayer);
		}
		int num3 = 0;
		for (int num4 = 0; num4 < userData_Group1.Length; num4++)
		{
			if (userData_Group1[num4].isPlayer)
			{
				num3++;
			}
		}
		SetCameraRect(num3);
	}
	public void SetCameraRect(int _playerNum)
	{
		SetCameraRectData(cameraData_4Player);
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
				if (userData_Group1[i].userType <= ShavedIce_Define.UserType.PLAYER_4)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group1[i].userType);
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].userType <= ShavedIce_Define.UserType.PLAYER_4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group2[j].userType);
			}
		}
	}
	public void SetGameStart()
	{
		isIceMachineProcess = true;
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].player.IM.PlayIceMachineAnimation();
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				userData_Group2[j].player.IM.PlayIceMachineAnimation();
			}
		}
	}
	public void SetGameEnd()
	{
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].isFailed = true;
				userData_Group1[i].player.GameFinishProcess();
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				userData_Group2[j].isFailed = true;
				userData_Group2[j].player.GameFinishProcess();
			}
		}
	}
	public void SetHeightCalc(int _dataNo, float _height)
	{
		if (isGroup1HeightCalc)
		{
			if (userData_Group1[_dataNo].nowHeight < _height)
			{
				userData_Group1[_dataNo].nowHeight = CalcManager.ConvertDecimalFirst(_height);
				ShavedIce_Define.UIM.SetTowerHeightCalcNumbers(_dataNo, userData_Group1[_dataNo].nowHeight);
			}
		}
		else if (userData_Group2[_dataNo].nowHeight < _height)
		{
			userData_Group2[_dataNo].nowHeight = CalcManager.ConvertDecimalFirst(_height);
			ShavedIce_Define.UIM.SetTowerHeightCalcNumbers(_dataNo, userData_Group2[_dataNo].nowHeight);
		}
	}
	private void SetCameraRectData(CameraRectData _cameraRectData)
	{
		if (isGroup1Playing)
		{
			userData_Group1[0].player.SetCameraRect_Main(_cameraRectData.mainCameraRect_0);
			userData_Group1[0].player.SetCameraRect_Sub(_cameraRectData.subCameraRect_0);
			userData_Group1[1].player.SetCameraRect_Main(_cameraRectData.mainCameraRect_1);
			userData_Group1[1].player.SetCameraRect_Sub(_cameraRectData.subCameraRect_1);
			userData_Group1[2].player.SetCameraRect_Main(_cameraRectData.mainCameraRect_2);
			userData_Group1[2].player.SetCameraRect_Sub(_cameraRectData.subCameraRect_2);
			userData_Group1[3].player.SetCameraRect_Main(_cameraRectData.mainCameraRect_3);
			userData_Group1[3].player.SetCameraRect_Sub(_cameraRectData.subCameraRect_3);
		}
		else
		{
			userData_Group2[0].player.SetCameraRect_Main(_cameraRectData.mainCameraRect_0);
			userData_Group2[0].player.SetCameraRect_Sub(_cameraRectData.subCameraRect_0);
			userData_Group2[1].player.SetCameraRect_Main(_cameraRectData.mainCameraRect_1);
			userData_Group2[1].player.SetCameraRect_Sub(_cameraRectData.subCameraRect_1);
			userData_Group2[2].player.SetCameraRect_Main(_cameraRectData.mainCameraRect_2);
			userData_Group2[2].player.SetCameraRect_Sub(_cameraRectData.subCameraRect_2);
			userData_Group2[3].player.SetCameraRect_Main(_cameraRectData.mainCameraRect_3);
			userData_Group2[3].player.SetCameraRect_Sub(_cameraRectData.subCameraRect_3);
		}
	}
	public void HideAllGameCamera()
	{
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].player.HideCamera();
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				userData_Group2[j].player.HideCamera();
			}
		}
	}
	public void SetDebugRecord()
	{
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].nowHeight = CalcManager.ConvertDecimalFirst(UnityEngine.Random.Range(0f, 99.9f));
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				userData_Group2[j].nowHeight = CalcManager.ConvertDecimalFirst(UnityEngine.Random.Range(0f, 99.9f));
			}
		}
		SetGameEnd();
	}
	public void SetAutoCPURecord(bool _isGroup1)
	{
		if (_isGroup1)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (userData_Group1[i].isPlayer || userData_Group1[i].isFailed)
				{
					continue;
				}
				switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
				{
				case 0:
					userData_Group1[i].nowHeight = CalcManager.ConvertDecimalFirst(userData_Group1[i].nowHeight + 2f + UnityEngine.Random.Range(0f, 0.9f));
					break;
				case 1:
					if (userData_Group1[i].nowHeight < 10f)
					{
						userData_Group1[i].nowHeight = CalcManager.ConvertDecimalFirst(userData_Group1[i].nowHeight + 10f + UnityEngine.Random.Range(0f, 0.9f));
					}
					else
					{
						userData_Group1[i].nowHeight = CalcManager.ConvertDecimalFirst(userData_Group1[i].nowHeight + 2f + UnityEngine.Random.Range(0f, 0.9f));
					}
					break;
				case 2:
					if (userData_Group1[i].nowHeight < 5f)
					{
						userData_Group1[i].nowHeight = CalcManager.ConvertDecimalFirst(userData_Group1[i].nowHeight + 20f + UnityEngine.Random.Range(0f, 0.9f));
					}
					else if (userData_Group1[i].nowHeight < 20f)
					{
						userData_Group1[i].nowHeight = CalcManager.ConvertDecimalFirst(userData_Group1[i].nowHeight + 10f + UnityEngine.Random.Range(0f, 0.9f));
					}
					else
					{
						userData_Group1[i].nowHeight = CalcManager.ConvertDecimalFirst(userData_Group1[i].nowHeight + 2f + UnityEngine.Random.Range(0f, 0.9f));
					}
					break;
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (!userData_Group2[j].isPlayer && !userData_Group2[j].isFailed)
			{
				userData_Group2[j].nowHeight = CalcManager.ConvertDecimalFirst(userData_Group2[j].nowHeight + 2f + UnityEngine.Random.Range(0f, 0.9f));
			}
		}
	}
	public void SetPreparationGroup2Data()
	{
		isGroup1Playing = false;
		characters_Group1_Anchor.SetActive(value: false);
		characters_Group2_Anchor.SetActive(value: true);
		ShavedIce_Define.UIM.Init(userData_Group2, _isGroup1: false);
		for (int i = 0; i < userData_Group2.Length; i++)
		{
			userData_Group2[i].player.Init(i, userData_Group2[i].userType, userData_Group2[i].isPlayer);
		}
		CalcManager.mCalcInt = 0;
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].isPlayer)
			{
				CalcManager.mCalcInt++;
			}
		}
		SetCameraRect(CalcManager.mCalcInt);
	}
	public void SetControlInfomationBalloonFadeIn()
	{
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (userData_Group1[i].isPlayer)
				{
					ShavedIce_Define.UIM.SetFadeInControlInfomation(i);
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].isPlayer)
			{
				ShavedIce_Define.UIM.SetFadeInControlInfomation(j);
			}
		}
	}
	public void SetControlInfomationBalloonFadeOut()
	{
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (userData_Group1[i].isPlayer)
				{
					ShavedIce_Define.UIM.SetFadeOutControlInfomation(i);
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].isPlayer)
			{
				ShavedIce_Define.UIM.SetFadeOutControlInfomation(j);
			}
		}
	}
	public void AddCreateIceCount(ShavedIce_Define.UserType _userType)
	{
		if (isGroup1Playing)
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
			userData_Group1[num].nowCreateIceCnt++;
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
		userData_Group2[num2].nowCreateIceCnt++;
	}
	public float GetTowerHeight(int _dataNo, bool _isGroup1)
	{
		if (_isGroup1)
		{
			return userData_Group1[_dataNo].nowHeight;
		}
		return userData_Group2[_dataNo].nowHeight;
	}
	public float[] GetTeamTowerHeightData(bool _isGroup)
	{
		float[] array = new float[ShavedIce_Define.MAX_PLAYER_NUM];
		if (_isGroup)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				array[i] = userData_Group1[i].nowHeight;
			}
		}
		else
		{
			for (int j = 0; j < userData_Group1.Length; j++)
			{
				array[j] = userData_Group2[j].nowHeight;
			}
		}
		return array;
	}
	public float[] GetAllTowerHeightData()
	{
		float[] array = new float[userData_Group1.Length + userData_Group2.Length];
		int num = 0;
		for (int i = 0; i < ShavedIce_Define.MAX_TEAM_NUM; i++)
		{
			for (int j = 0; j < userData_Group1.Length; j++)
			{
				if (userData_Group1[j].teamType == (ShavedIce_Define.TeamType)i)
				{
					array[num] = userData_Group1[j].nowHeight;
					num++;
				}
			}
			for (int k = 0; k < userData_Group2.Length; k++)
			{
				if (userData_Group2[k].teamType == (ShavedIce_Define.TeamType)i)
				{
					array[num] = userData_Group2[k].nowHeight;
					num++;
				}
			}
		}
		return array;
	}
	public int[] GetTeamUserNoData(bool _isGroup1)
	{
		int[] array = new int[ShavedIce_Define.MAX_PLAYER_NUM];
		if (_isGroup1)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				array[i] = (int)userData_Group1[i].userType;
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				array[j] = (int)userData_Group2[j].userType;
			}
		}
		return array;
	}
	public int[] GetAllUserNoData()
	{
		int[] array = new int[userData_Group1.Length + userData_Group2.Length];
		int num = 0;
		for (int i = 0; i < ShavedIce_Define.MAX_TEAM_NUM; i++)
		{
			for (int j = 0; j < userData_Group1.Length; j++)
			{
				if (userData_Group1[j].teamType == (ShavedIce_Define.TeamType)i)
				{
					array[num] = (int)userData_Group1[j].userType;
					num++;
				}
			}
			for (int k = 0; k < userData_Group2.Length; k++)
			{
				if (userData_Group2[k].teamType == (ShavedIce_Define.TeamType)i)
				{
					array[num] = (int)userData_Group2[k].userType;
					num++;
				}
			}
		}
		return array;
	}
	public float GetTeamTotalRecord(int _teamNo)
	{
		float num = 0f;
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			if (userData_Group1[i].teamType == (ShavedIce_Define.TeamType)_teamNo)
			{
				num += userData_Group1[i].nowHeight;
			}
		}
		return num;
	}
	public int GetCreateIceCount(ShavedIce_Define.UserType _userType)
	{
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (userData_Group1[i].userType == _userType)
				{
					return userData_Group1[i].nowCreateIceCnt;
				}
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				if (userData_Group2[j].userType == _userType)
				{
					return userData_Group2[j].nowCreateIceCnt;
				}
			}
		}
		return 0;
	}
	public Camera GetUserCamera(ShavedIce_Define.UserType _userType, bool _isGroup1)
	{
		if (_isGroup1)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (userData_Group1[i].userType == _userType)
				{
					return userData_Group1[i].player.GetMainCamera();
				}
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				if (userData_Group2[j].userType == _userType)
				{
					return userData_Group2[j].player.GetMainCamera();
				}
			}
		}
		return null;
	}
	private void UpdateIceMachine()
	{
		if (!isIceMachineProcess)
		{
			return;
		}
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].player.IM.UpdateMethod();
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				userData_Group2[j].player.IM.UpdateMethod();
			}
		}
		if (ShavedIce_Define.GM.DuringGameTime > ShavedIce_Define.GAME_TIME - ShavedIce_Define.ICE_FX_STOP_ADVANCE_TIME)
		{
			if (isGroup1Playing)
			{
				for (int k = 0; k < userData_Group1.Length; k++)
				{
					userData_Group1[k].player.IM.StopIceFX();
				}
			}
			else
			{
				for (int l = 0; l < userData_Group2.Length; l++)
				{
					userData_Group2[l].player.IM.StopIceFX();
				}
			}
			isIceMachineProcess = false;
		}
		else
		{
			if (!(ShavedIce_Define.GM.DuringGameTime > ShavedIce_Define.ICE_FX_MOVE_START_TIME))
			{
				return;
			}
			if (isGroup1Playing)
			{
				for (int m = 0; m < userData_Group1.Length; m++)
				{
					if (!userData_Group1[m].player.IM.IsIceFXMoveEnd)
					{
						return;
					}
				}
			}
			else
			{
				for (int n = 0; n < userData_Group2.Length; n++)
				{
					if (!userData_Group2[n].player.IM.IsIceFXMoveEnd)
					{
						return;
					}
				}
			}
			prevIceFXMoveXPos = iceFXMoveXPos;
			iceFXMoveXPos = UnityEngine.Random.Range(0f, 0.35f);
			iceFXMoveXPos *= Mathf.Sign(prevIceFXMoveXPos) * -1f;
			if (isGroup1Playing)
			{
				for (int num = 0; num < userData_Group1.Length; num++)
				{
					userData_Group1[num].player.IM.SetMoveIceFXPosX(iceFXMoveXPos, UnityEngine.Random.Range(0.1f, 0.4f));
					if (!userData_Group1[num].isPlayer)
					{
						userData_Group1[num].player.AI.AddCupMovePoint(iceFXMoveXPos);
					}
				}
				return;
			}
			for (int num2 = 0; num2 < userData_Group2.Length; num2++)
			{
				userData_Group2[num2].player.IM.SetMoveIceFXPosX(iceFXMoveXPos, UnityEngine.Random.Range(0.1f, 0.4f));
				if (!userData_Group2[num2].isPlayer)
				{
					userData_Group2[num2].player.AI.AddCupMovePoint(iceFXMoveXPos);
				}
			}
		}
	}
	public void InitTowerHeightCalc(bool _isGroup1)
	{
		if (_isGroup1)
		{
			characters_Group1_Anchor.SetActive(value: true);
			characters_Group2_Anchor.SetActive(value: false);
			ShavedIce_Define.UIM.Init(userData_Group1, _isGroup1);
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].player.InitHeightCalc(_isGroup1);
			}
			isGroup1HeightCalc = true;
		}
		else
		{
			characters_Group1_Anchor.SetActive(value: false);
			characters_Group2_Anchor.SetActive(value: true);
			ShavedIce_Define.UIM.Init(userData_Group2, _isGroup1);
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				userData_Group2[j].player.InitHeightCalc(_isGroup1);
			}
			isGroup1HeightCalc = false;
		}
	}
	public void StartTowerHeightCalc(bool _isGroup1)
	{
		if (_isGroup1)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].player.StartHeightCalc();
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				userData_Group2[j].player.StartHeightCalc();
			}
		}
		SingletonCustom<AudioManager>.Instance.SePlay("se_drum_roll");
	}
	public bool IsTowerHeightCalcEnd(bool _isGroup1)
	{
		if (_isGroup1)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (!userData_Group1[i].player.IsTowerHeightCalcEnd)
				{
					return false;
				}
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				if (!userData_Group2[j].player.IsTowerHeightCalcEnd)
				{
					return false;
				}
			}
		}
		return true;
	}
}
