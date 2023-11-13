using System;
using System.Collections.Generic;
using UnityEngine;
public class SmartBall_MainCharacterManager : SingletonCustom<SmartBall_MainCharacterManager>
{
	[Serializable]
	public struct UserData
	{
		public SmartBall_Character character;
		public SB.UserType userType;
		public SB.TeamType teamType;
		public int nowPoint;
		public int nowUsedBallNum;
		public bool isFailed;
		public bool isPlayer;
	}
	[Serializable]
	public struct CameraRectData
	{
		[Header("カメラ[0]のRect")]
		public Rect cameraRect_0;
		[Header("カメラ[1]のRect")]
		public Rect cameraRect_1;
		[Header("カメラ[2]のRect")]
		public Rect cameraRect_2;
		[Header("カメラ[3]のRect")]
		public Rect cameraRect_3;
	}
	[Serializable]
	public struct CameraPositionData
	{
		[Header("カメラ[0]の座標")]
		public Vector3 cameraPos_0;
		[Header("カメラ[1]の座標")]
		public Vector3 cameraPos_1;
		[Header("カメラ[2]の座標")]
		public Vector3 cameraPos_2;
		[Header("カメラ[3]の座標")]
		public Vector3 cameraPos_3;
	}
	[SerializeField]
	[Header("１組目のキャラアンカ\u30fc")]
	private GameObject characters_Group1_Anchor;
	[SerializeField]
	[Header("１組目のキャラクタ\u30fc")]
	private SmartBall_Character[] characters_Group1;
	[SerializeField]
	[Header("２組目のキャラアンカ\u30fc")]
	private GameObject characters_Group2_Anchor;
	[SerializeField]
	[Header("２組目のキャラクタ\u30fc")]
	private SmartBall_Character[] characters_Group2;
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
	[SerializeField]
	[Header("プレイヤ\u30fc１人の時のカメラ座標デ\u30fcタ")]
	private CameraPositionData cameraPosData_1Player;
	[SerializeField]
	[Header("プレイヤ\u30fc２人の時のカメラ座標デ\u30fcタ")]
	private CameraPositionData cameraPosData_2Player;
	[SerializeField]
	[Header("プレイヤ\u30fc３人の時のカメラ座標デ\u30fcタ")]
	private CameraPositionData cameraPosData_3Player;
	[SerializeField]
	[Header("プレイヤ\u30fc４人の時のカメラ座標デ\u30fcタ")]
	private CameraPositionData cameraPosData_4Player;
	private UserData[] userData_Group1;
	private UserData[] userData_Group2;
	private bool isGroup1Playing = true;
	private const int POINT_LIMIT = 999;
	[SerializeField]
	[Header("玉のオブジェクトプレハブ")]
	private SmartBall_BallObject ballObjectPrefab;
	[SerializeField]
	[Header("台オブジェクトプレハブ")]
	private SmartBall_StandObject[] standObjPrefabs;
	private int createStandTypeNo;
	public int CreateStandTypeNo => createStandTypeNo;
	public void Init()
	{
		createStandTypeNo = SetCreateStandNo();
		characters_Group2_Anchor.SetActive(value: false);
		TeamDataInit();
	}
	private void TeamDataInit()
	{
		switch (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat)
		{
		case GS_Define.GameFormat.BATTLE:
			userData_Group1 = new UserData[SB.MAX_TEAM_NUM];
			for (int j = 0; j < userData_Group1.Length; j++)
			{
				userData_Group1[j].character = characters_Group1[j];
				if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && j == userData_Group1.Length - 1)
				{
					userData_Group1[j].userType = SB.UserType.CPU_1;
				}
				else
				{
					userData_Group1[j].userType = (SB.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][0];
				}
				userData_Group1[j].teamType = (SB.TeamType)j;
				userData_Group1[j].nowPoint = 0;
				userData_Group1[j].isFailed = false;
				userData_Group1[j].isPlayer = (userData_Group1[j].userType <= SB.UserType.PLAYER_4);
			}
			break;
		case GS_Define.GameFormat.COOP:
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
			{
				userData_Group1 = new UserData[SB.MAX_TEAM_NUM];
				userData_Group2 = new UserData[SB.MAX_TEAM_NUM];
			}
			else
			{
				userData_Group1 = new UserData[SB.MAX_TEAM_NUM];
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
					userData_Group1[n].userType = (SB.UserType)list[n];
					userData_Group1[n].teamType = SB.TeamType.TEAM_A;
				}
				else
				{
					if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
					{
						userData_Group1[n].userType = ((n == 2) ? SB.UserType.CPU_2 : SB.UserType.CPU_3);
					}
					else
					{
						userData_Group1[n].userType = ((n == 2) ? SB.UserType.CPU_1 : SB.UserType.CPU_2);
					}
					userData_Group1[n].teamType = SB.TeamType.TEAM_B;
				}
				userData_Group1[n].character = characters_Group1[n];
				userData_Group1[n].nowPoint = 0;
				userData_Group1[n].isFailed = false;
				userData_Group1[n].isPlayer = (userData_Group1[n].userType <= SB.UserType.PLAYER_4);
			}
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum <= 2)
			{
				break;
			}
			for (int num = 0; num < userData_Group2.Length; num++)
			{
				if (num < 2)
				{
					userData_Group2[num].userType = (SB.UserType)list2[num];
					userData_Group2[num].teamType = SB.TeamType.TEAM_A;
				}
				else
				{
					if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
					{
						userData_Group2[num].userType = ((num == 2) ? SB.UserType.CPU_4 : SB.UserType.CPU_5);
					}
					else
					{
						userData_Group2[num].userType = ((num == 2) ? SB.UserType.CPU_3 : SB.UserType.CPU_4);
					}
					userData_Group2[num].teamType = SB.TeamType.TEAM_B;
				}
				userData_Group2[num].character = characters_Group2[num];
				userData_Group2[num].nowPoint = 0;
				userData_Group2[num].isFailed = false;
				userData_Group2[num].isPlayer = (userData_Group2[num].userType <= SB.UserType.PLAYER_4);
			}
			break;
		}
		case GS_Define.GameFormat.BATTLE_AND_COOP:
			userData_Group1 = new UserData[SB.MAX_TEAM_NUM];
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (i < 2)
				{
					userData_Group1[i].userType = (SB.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[0][i];
					userData_Group1[i].teamType = SB.TeamType.TEAM_A;
				}
				else
				{
					userData_Group1[i].userType = (SB.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[1][(i != 2) ? 1 : 0];
					userData_Group1[i].teamType = SB.TeamType.TEAM_B;
				}
				userData_Group1[i].character = characters_Group1[i];
				userData_Group1[i].nowPoint = 0;
				userData_Group1[i].isFailed = false;
				userData_Group1[i].isPlayer = (userData_Group1[i].userType <= SB.UserType.PLAYER_4);
			}
			break;
		}
		SB.GUIM.Init(userData_Group1, _isGroup1: true);
		for (int num2 = 0; num2 < userData_Group1.Length; num2++)
		{
			userData_Group1[num2].character.Init(num2, userData_Group1[num2].userType, userData_Group1[num2].isPlayer);
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
	public void UpdateMethod()
	{
		if (!SB.MGM.IsDuringGame())
		{
			return;
		}
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				if (!userData_Group1[i].isFailed)
				{
					userData_Group1[i].character.UpdateMethod();
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (!userData_Group2[j].isFailed)
			{
				userData_Group2[j].character.UpdateMethod();
			}
		}
	}
	public SmartBall_BallObject GetCreateBall()
	{
		bool isGroup1Playing2 = isGroup1Playing;
		return ballObjectPrefab;
	}
	private int SetCreateStandNo()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < standObjPrefabs.Length; i++)
		{
			if (SB.LAST_STAND_NO != i && SB.PREVIOUS_LAST_STAND_NO != i)
			{
				list.Add(i);
			}
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		SB.PREVIOUS_LAST_STAND_NO = SB.LAST_STAND_NO;
		SB.LAST_STAND_NO = list[index];
		return list[index];
	}
	public SmartBall_StandObject GetCreateStand()
	{
		return standObjPrefabs[createStandTypeNo];
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
				if (!userData_Group1[i].isFailed)
				{
					return false;
				}
			}
		}
		else
		{
			for (int j = 0; j < userData_Group2.Length; j++)
			{
				if (!userData_Group2[j].isFailed)
				{
					return false;
				}
			}
		}
		return true;
	}
	public bool IsAllPlayerTeamFailed()
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
	public void SetCameraRect(int _playerNum)
	{
		switch (_playerNum)
		{
		case 1:
			SetCameraRectData(cameraData_1Player);
			SetCameraHight(cameraPosData_1Player);
			break;
		case 2:
			SetCameraRectData(cameraData_2Player);
			SetCameraHight(cameraPosData_2Player);
			break;
		case 3:
			SetCameraRectData(cameraData_3Player);
			SetCameraHight(cameraPosData_3Player);
			break;
		case 4:
			SetCameraRectData(cameraData_4Player);
			SetCameraHight(cameraPosData_4Player);
			break;
		}
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
				if (userData_Group1[i].userType <= SB.UserType.PLAYER_4)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group1[i].userType);
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].userType <= SB.UserType.PLAYER_4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group2[j].userType);
			}
		}
	}
	public void SetTeamFailed(int _dataNo)
	{
		if (isGroup1Playing)
		{
			userData_Group1[_dataNo].isFailed = true;
		}
		else
		{
			userData_Group2[_dataNo].isFailed = true;
		}
		SB.GUIM.ShowGameOverDisplay(_dataNo, IsAllTeamFailed());
	}
	public void SetPoint(int _dataNo, int _point)
	{
		if (isGroup1Playing)
		{
			userData_Group1[_dataNo].nowPoint = _point;
			if (userData_Group1[_dataNo].nowPoint > 999)
			{
				userData_Group1[_dataNo].nowPoint = 999;
			}
			SB.GUIM.SetPoint(_dataNo, userData_Group1[_dataNo].nowPoint);
		}
		else
		{
			userData_Group2[_dataNo].nowPoint = _point;
			if (userData_Group2[_dataNo].nowPoint > 999)
			{
				userData_Group2[_dataNo].nowPoint = 999;
			}
			SB.GUIM.SetPoint(_dataNo, userData_Group2[_dataNo].nowPoint);
		}
	}
	private void SetCameraRectData(CameraRectData _cameraRectData)
	{
		if (isGroup1Playing)
		{
			userData_Group1[0].character.GetControleCamera().rect = _cameraRectData.cameraRect_0;
			userData_Group1[1].character.GetControleCamera().rect = _cameraRectData.cameraRect_1;
			userData_Group1[2].character.GetControleCamera().rect = _cameraRectData.cameraRect_2;
			userData_Group1[3].character.GetControleCamera().rect = _cameraRectData.cameraRect_3;
		}
		else
		{
			userData_Group2[0].character.GetControleCamera().rect = _cameraRectData.cameraRect_0;
			userData_Group2[1].character.GetControleCamera().rect = _cameraRectData.cameraRect_1;
			userData_Group2[2].character.GetControleCamera().rect = _cameraRectData.cameraRect_2;
			userData_Group2[3].character.GetControleCamera().rect = _cameraRectData.cameraRect_3;
		}
	}
	private void SetCameraHight(CameraPositionData _cameraHightData)
	{
		if (isGroup1Playing)
		{
			userData_Group1[0].character.GetControleCamera().transform.localPosition = _cameraHightData.cameraPos_0;
			userData_Group1[1].character.GetControleCamera().transform.localPosition = _cameraHightData.cameraPos_1;
			userData_Group1[2].character.GetControleCamera().transform.localPosition = _cameraHightData.cameraPos_2;
			userData_Group1[3].character.GetControleCamera().transform.localPosition = _cameraHightData.cameraPos_3;
		}
		else
		{
			userData_Group2[0].character.GetControleCamera().transform.localPosition = _cameraHightData.cameraPos_0;
			userData_Group2[1].character.GetControleCamera().transform.localPosition = _cameraHightData.cameraPos_1;
			userData_Group2[2].character.GetControleCamera().transform.localPosition = _cameraHightData.cameraPos_2;
			userData_Group2[3].character.GetControleCamera().transform.localPosition = _cameraHightData.cameraPos_3;
		}
	}
	public void SetDebugRecord()
	{
		if (isGroup1Playing)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].nowPoint = UnityEngine.Random.Range(0, 100);
				SetTeamFailed(i);
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			userData_Group2[j].nowPoint = UnityEngine.Random.Range(0, 100);
			userData_Group2[j].isFailed = true;
			SetTeamFailed(j);
		}
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
					if (userData_Group1[i].character.HasBallNum != 0)
					{
						userData_Group1[i].nowPoint = userData_Group1[i].nowPoint + UnityEngine.Random.Range(0, 3) * 5;
					}
					break;
				case 1:
					if (userData_Group1[i].character.HasBallNum != 0)
					{
						if ((float)userData_Group1[i].nowPoint < 10f)
						{
							userData_Group1[i].nowPoint = userData_Group1[i].nowPoint + 10 + UnityEngine.Random.Range(0, 3) * 5;
						}
						else
						{
							userData_Group1[i].nowPoint = userData_Group1[i].nowPoint + UnityEngine.Random.Range(0, 3) * 5;
						}
					}
					break;
				case 2:
					if (userData_Group1[i].character.HasBallNum != 0)
					{
						if ((float)userData_Group1[i].nowPoint < 5f)
						{
							userData_Group1[i].nowPoint = userData_Group1[i].nowPoint + 20 + UnityEngine.Random.Range(0, 3) * 5;
						}
						else if ((float)userData_Group1[i].nowPoint < 20f)
						{
							userData_Group1[i].nowPoint = userData_Group1[i].nowPoint + 10 + UnityEngine.Random.Range(0, 3) * 5;
						}
						else
						{
							userData_Group1[i].nowPoint = userData_Group1[i].nowPoint + UnityEngine.Random.Range(0, 3) * 5;
						}
					}
					break;
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].isPlayer || userData_Group2[j].isFailed)
			{
				continue;
			}
			switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
			{
			case 0:
				if (userData_Group1[j].character.HasBallNum != 0)
				{
					userData_Group1[j].nowPoint = userData_Group1[j].nowPoint + UnityEngine.Random.Range(0, 3) * 5;
				}
				break;
			case 1:
				if (userData_Group1[j].character.HasBallNum != 0)
				{
					if ((float)userData_Group1[j].nowPoint < 10f)
					{
						userData_Group1[j].nowPoint = userData_Group1[j].nowPoint + 10 + UnityEngine.Random.Range(0, 3) * 5;
					}
					else
					{
						userData_Group1[j].nowPoint = userData_Group1[j].nowPoint + UnityEngine.Random.Range(0, 3) * 5;
					}
				}
				break;
			case 2:
				if (userData_Group1[j].character.HasBallNum != 0)
				{
					if ((float)userData_Group1[j].nowPoint < 5f)
					{
						userData_Group1[j].nowPoint = userData_Group1[j].nowPoint + 20 + UnityEngine.Random.Range(0, 3) * 5;
					}
					else if ((float)userData_Group1[j].nowPoint < 20f)
					{
						userData_Group1[j].nowPoint = userData_Group1[j].nowPoint + 10 + UnityEngine.Random.Range(0, 3) * 5;
					}
					else
					{
						userData_Group1[j].nowPoint = userData_Group1[j].nowPoint + UnityEngine.Random.Range(0, 3) * 5;
					}
				}
				break;
			}
		}
	}
	public void SetPreparationGroup2Data()
	{
		isGroup1Playing = false;
		characters_Group1_Anchor.SetActive(value: false);
		characters_Group2_Anchor.SetActive(value: true);
		SB.GUIM.Init(userData_Group2, _isGroup1: false);
		for (int i = 0; i < userData_Group2.Length; i++)
		{
			userData_Group2[i].character.Init(i, userData_Group2[i].userType, userData_Group2[i].isPlayer);
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
					SB.GUIM.SetFadeInFirstControlInfomation(i);
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].isPlayer)
			{
				SB.GUIM.SetFadeInFirstControlInfomation(j);
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
					SB.GUIM.SetFadeOutFirstControlInfomation(i);
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].isPlayer)
			{
				SB.GUIM.SetFadeOutFirstControlInfomation(j);
			}
		}
	}
	public int GetPoint(int _dataNo)
	{
		if (isGroup1Playing)
		{
			return userData_Group1[_dataNo].nowPoint;
		}
		return userData_Group2[_dataNo].nowPoint;
	}
	public int[] GetTeamPointData(bool _isGroup)
	{
		int[] array = new int[SB.MAX_TEAM_NUM];
		if (_isGroup)
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				array[i] = userData_Group1[i].nowPoint;
			}
		}
		else
		{
			for (int j = 0; j < userData_Group1.Length; j++)
			{
				array[j] = userData_Group2[j].nowPoint;
			}
		}
		return array;
	}
	public int[] GetTeamUserNoData(bool _isGroup1)
	{
		int[] array = new int[SB.MAX_TEAM_NUM];
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
	public int GetTeamTotalRecord(int _teamNo)
	{
		int num = 0;
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			if (userData_Group1[i].teamType == (SB.TeamType)_teamNo)
			{
				num += userData_Group1[i].nowPoint;
			}
		}
		return num;
	}
}
