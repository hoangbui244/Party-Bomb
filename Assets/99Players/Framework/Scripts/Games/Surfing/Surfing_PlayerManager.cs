using System;
using UnityEngine;
public class Surfing_PlayerManager : SingletonCustom<Surfing_PlayerManager>
{
	[Serializable]
	public struct UserData
	{
		public Surfing_Player player;
		public Surfing_Define.UserType userType;
		public Camera camera;
		public int point;
		public bool isPlayer;
	}
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private Surfing_Player[] players;
	[SerializeField]
	[Header("カメラ")]
	private Camera[] cameras;
	[SerializeField]
	[Header("ライト")]
	private Light DirectionalLight;
	private UserData[] userData_Group1;
	private UserData[] userData_Group2;
	private bool isGroup1Playing = true;
	private static readonly Rect[] SINGLE_PLAYERONLY_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0f, 1f, 1f),
		new Rect(1f, 1f, 1f, 1f),
		new Rect(1f, 1f, 1f, 1f),
		new Rect(1f, 1f, 1f, 1f)
	};
	private static readonly Rect[] FOUR_PLAYER_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0.5f, 0.5f, 0.5f),
		new Rect(0.5f, 0.5f, 0.5f, 0.5f),
		new Rect(0f, 0f, 0.5f, 0.5f),
		new Rect(0.5f, 0f, 0.5f, 0.5f)
	};
	public Surfing_Player[] Players => players;
	public UserData[] UserData_Group1 => userData_Group1;
	public void Init()
	{
		Init_UserData();
		switch (Surfing_Define.PLAYER_NUM)
		{
		case 1:
			for (int j = 0; j < cameras.Length; j++)
			{
				cameras[j].rect = SINGLE_PLAYERONLY_CAMERA_RECT[j];
			}
			DirectionalLight.shadows = LightShadows.Hard;
			break;
		case 2:
		case 3:
		case 4:
			for (int i = 0; i < cameras.Length; i++)
			{
				cameras[i].rect = FOUR_PLAYER_CAMERA_RECT[i];
			}
			DirectionalLight.shadows = LightShadows.Hard;
			break;
		}
	}
	public void PlayerGameStart()
	{
		players[0].Surfer.StartMethod();
		players[1].Surfer.StartMethod();
		players[2].Surfer.StartMethod();
		players[3].Surfer.StartMethod();
	}
	private void Init_UserData()
	{
		userData_Group1 = new UserData[Surfing_Define.MEMBER_NUM];
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			userData_Group1[i].player = players[i];
			if (Surfing_Define.PLAYER_NUM == 3 && i == userData_Group1.Length - 1)
			{
				userData_Group1[i].userType = Surfing_Define.UserType.CPU_1;
			}
			else
			{
				userData_Group1[i].userType = (Surfing_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
			}
			userData_Group1[i].point = 0;
			userData_Group1[i].camera = cameras[i];
			userData_Group1[i].isPlayer = (userData_Group1[i].userType <= Surfing_Define.UserType.PLAYER_4);
			userData_Group1[i].player.Init(userData_Group1[i].userType);
		}
		Surfing_Define.UIM.SetUserUIData(userData_Group1);
	}
	public void UpdateMethod()
	{
		if (Surfing_Define.GM.IsDuringGame())
		{
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].player.UpdateMethod();
			}
		}
	}
	private void Awake()
	{
	}
	public UserData GetUserData(Surfing_Define.UserType _userType)
	{
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			if (userData_Group1[i].userType == _userType)
			{
				return userData_Group1[i];
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
		int[] array = new int[userData_Group1.Length];
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			array[i] = userData_Group1[i].point;
		}
		return array;
	}
	public int[] GetAllUserNoArray(bool _isGroup1 = true)
	{
		int[] array = new int[userData_Group1.Length];
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			array[i] = (int)userData_Group1[i].userType;
		}
		return array;
	}
	public Camera GetUserCamera(Surfing_Define.UserType _userType)
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
	public bool CheckPlayerWaveRiding()
	{
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].UserType <= Surfing_Define.UserType.PLAYER_4 && players[i].Surfer.IsGround && players[i].Surfer.IsWaveArea)
			{
				return true;
			}
		}
		return false;
	}
	public void SetPoint(Surfing_Define.UserType _userType, int _point)
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
		if (_point < 0 && Mathf.Abs(_point) >= userData_Group1[num].point)
		{
			_point = -userData_Group1[num].point;
		}
		userData_Group1[num].point += _point;
		Surfing_Define.UIM.SetScore(_userType, userData_Group1[num].point, _point);
	}
	public bool PlayerGoalCheck()
	{
		int num = 0;
		for (int i = 0; i < Surfing_Define.MEMBER_NUM; i++)
		{
			if (userData_Group1[i].player.Surfer.processType == Surfing_Surfer.SurferProcessType.GOAL && userData_Group1[i].player.UserType <= Surfing_Define.UserType.PLAYER_4)
			{
				num++;
			}
		}
		if (num == Surfing_Define.PLAYER_NUM)
		{
			return true;
		}
		return false;
	}
	public bool PlayerOnlyOneCheck()
	{
		int num = 0;
		for (int i = 0; i < Surfing_Define.MEMBER_NUM; i++)
		{
			if (userData_Group1[i].player.Surfer.processType == Surfing_Surfer.SurferProcessType.GOAL && userData_Group1[i].player.UserType <= Surfing_Define.UserType.PLAYER_4)
			{
				num++;
			}
		}
		if (num + 1 == Surfing_Define.PLAYER_NUM)
		{
			return true;
		}
		return false;
	}
	public int StartPlayerNumCheck()
	{
		int num = 0;
		for (int i = 0; i < Surfing_Define.MEMBER_NUM; i++)
		{
			if (userData_Group1[i].player.Surfer.processType == Surfing_Surfer.SurferProcessType.START)
			{
				num++;
			}
		}
		UnityEngine.Debug.Log("レ\u30fcス中のプレイヤ\u30fcは" + num.ToString() + "人です");
		return num;
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
				if (userData_Group1[i].userType <= Surfing_Define.UserType.PLAYER_4)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group1[i].userType);
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].userType <= Surfing_Define.UserType.PLAYER_4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group2[j].userType);
			}
		}
	}
	public void SetDebugRecord()
	{
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			userData_Group1[i].point = UnityEngine.Random.Range(500, 3000);
		}
	}
}
