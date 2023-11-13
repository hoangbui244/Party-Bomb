using System;
using UnityEngine;
public class WaterSpriderRace_PlayerManager : SingletonCustom<WaterSpriderRace_PlayerManager>
{
	[Serializable]
	public struct UserData
	{
		public WaterSpriderRace_Player player;
		public WaterSpriderRace_Define.UserType userType;
		public Camera camera;
		public float goalTime;
		public bool isPlayer;
	}
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private WaterSpriderRace_Player[] players;
	[SerializeField]
	[Header("カメラ")]
	private Camera[] cameras;
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
	private static readonly Rect[] SINGLE_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0f, 0.53f, 1f),
		new Rect(0.532f, 0f, 0.155f, 1f),
		new Rect(0.688f, 0f, 0.155f, 1f),
		new Rect(0.845f, 0f, 0.155f, 1f)
	};
	private static readonly Rect[] TWO_PLAYER_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0f, 0.345f, 1f),
		new Rect(0.344f, 0f, 0.345f, 1f),
		new Rect(0.69f, 0f, 0.155f, 1f),
		new Rect(0.845f, 0f, 0.155f, 1f)
	};
	private static readonly Rect[] THREE_PLAYER_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0f, 0.28f, 1f),
		new Rect(0.282f, 0f, 0.28f, 1f),
		new Rect(0.564f, 0f, 0.28f, 1f),
		new Rect(0.845f, 0f, 0.155f, 1f)
	};
	private static readonly Rect[] FOUR_PLAYER_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0f, 0.25f, 1f),
		new Rect(0.25f, 0f, 0.25f, 1f),
		new Rect(0.5f, 0f, 0.25f, 1f),
		new Rect(0.75f, 0f, 0.25f, 1f)
	};
	public WaterSpriderRace_Player[] Players => players;
	public UserData[] UserData_Group1 => userData_Group1;
	public void Init()
	{
		Init_UserData();
		switch (WaterSpriderRace_Define.PLAYER_NUM)
		{
		case 1:
			for (int l = 0; l < cameras.Length; l++)
			{
				cameras[l].rect = SINGLE_CAMERA_RECT[l];
			}
			break;
		case 2:
			for (int j = 0; j < cameras.Length; j++)
			{
				cameras[j].rect = TWO_PLAYER_CAMERA_RECT[j];
			}
			break;
		case 3:
			for (int k = 0; k < cameras.Length; k++)
			{
				cameras[k].rect = THREE_PLAYER_CAMERA_RECT[k];
			}
			break;
		case 4:
			for (int i = 0; i < cameras.Length; i++)
			{
				cameras[i].rect = FOUR_PLAYER_CAMERA_RECT[i];
			}
			break;
		}
	}
	public void PlayerGameStart()
	{
		players[0].WaterSprider.StartMethod();
		players[1].WaterSprider.StartMethod();
		players[2].WaterSprider.StartMethod();
		players[3].WaterSprider.StartMethod();
	}
	public void CpuViewUpdate()
	{
		int pLAYER_NUM = WaterSpriderRace_Define.PLAYER_NUM;
	}
	private void Init_UserData()
	{
		userData_Group1 = new UserData[WaterSpriderRace_Define.MEMBER_NUM];
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			userData_Group1[i].player = players[i];
			if (WaterSpriderRace_Define.PLAYER_NUM == 3 && i == userData_Group1.Length - 1)
			{
				userData_Group1[i].userType = WaterSpriderRace_Define.UserType.CPU_1;
			}
			else
			{
				userData_Group1[i].userType = (WaterSpriderRace_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
			}
			userData_Group1[i].goalTime = 599.99f;
			userData_Group1[i].camera = cameras[i];
			userData_Group1[i].isPlayer = (userData_Group1[i].userType <= WaterSpriderRace_Define.UserType.PLAYER_4);
			userData_Group1[i].player.Init(userData_Group1[i].userType);
		}
		WaterSpriderRace_Define.UIM.SetUserUIData(userData_Group1);
	}
	public void UpdateMethod()
	{
		if (WaterSpriderRace_Define.GM.IsDuringGame())
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
	public UserData GetUserData(WaterSpriderRace_Define.UserType _userType)
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
	public float[] GetAllUserRecordArray(bool _isGroup1 = true)
	{
		float[] array = new float[userData_Group1.Length];
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			array[i] = userData_Group1[i].goalTime;
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
	public Camera GetUserCamera(WaterSpriderRace_Define.UserType _userType)
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
	public void SetGoalTime(WaterSpriderRace_Define.UserType _userType, float _setTime)
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
		userData_Group1[num].goalTime = _setTime;
		userData_Group1[num].goalTime = CalcManager.ConvertDecimalSecond(userData_Group1[num].goalTime);
	}
	public bool PlayerGoalCheck()
	{
		int num = 0;
		for (int i = 0; i < WaterSpriderRace_Define.MEMBER_NUM; i++)
		{
			if (userData_Group1[i].player.WaterSprider.processType == WaterSpriderRace_WaterSprider.SkiBoardProcessType.GOAL && userData_Group1[i].player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
			{
				num++;
			}
		}
		if (num == WaterSpriderRace_Define.PLAYER_NUM)
		{
			return true;
		}
		return false;
	}
	public bool PlayerOnlyOneCheck()
	{
		int num = 0;
		for (int i = 0; i < WaterSpriderRace_Define.MEMBER_NUM; i++)
		{
			if (userData_Group1[i].player.WaterSprider.processType == WaterSpriderRace_WaterSprider.SkiBoardProcessType.GOAL && userData_Group1[i].player.UserType <= WaterSpriderRace_Define.UserType.PLAYER_4)
			{
				num++;
			}
		}
		if (num + 1 == WaterSpriderRace_Define.PLAYER_NUM)
		{
			return true;
		}
		return false;
	}
	public int StartPlayerNumCheck()
	{
		int num = 0;
		for (int i = 0; i < WaterSpriderRace_Define.MEMBER_NUM; i++)
		{
			if (userData_Group1[i].player.WaterSprider.processType == WaterSpriderRace_WaterSprider.SkiBoardProcessType.START)
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
				if (userData_Group1[i].userType <= WaterSpriderRace_Define.UserType.PLAYER_4)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group1[i].userType);
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].userType <= WaterSpriderRace_Define.UserType.PLAYER_4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group2[j].userType);
			}
		}
	}
	public void SetDebugRecord()
	{
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			userData_Group1[i].goalTime = UnityEngine.Random.Range(60f, 100f);
			userData_Group1[i].goalTime = CalcManager.ConvertDecimalSecond(userData_Group1[i].goalTime);
		}
	}
}
