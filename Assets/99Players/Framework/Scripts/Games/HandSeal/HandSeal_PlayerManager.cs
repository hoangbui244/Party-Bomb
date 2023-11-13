using System;
using UnityEngine;
public class HandSeal_PlayerManager : SingletonCustom<HandSeal_PlayerManager>
{
	[Serializable]
	public struct UserData
	{
		public HandSeal_Player player;
		public HandSeal_Define.UserType userType;
		public Camera camera;
		public int point;
		public bool isPlayer;
	}
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private HandSeal_Player[] players;
	[SerializeField]
	[Header("カメラ")]
	private Camera[] cameras;
	[SerializeField]
	[Header("ライト")]
	private Light[] directionalLight;
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
	private static readonly Rect[] SINGLE_PLAYERONLY_CAMERA_RECT = new Rect[4]
	{
		new Rect(0f, 0f, 1f, 1f),
		new Rect(1f, 1f, 1f, 1f),
		new Rect(1f, 1f, 1f, 1f),
		new Rect(1f, 1f, 1f, 1f)
	};
	private static readonly Rect[] TWO_PLAYER_CAMERA_RECT = new Rect[4]
	{
		new Rect(-0.5f, 0f, 1f, 1f),
		new Rect(0.5f, 0f, 0.5f, 1f),
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
	public HandSeal_Player[] Players => players;
	public UserData[] UserData_Group1 => userData_Group1;
	public void Init()
	{
		Init_UserData();
		switch (HandSeal_Define.PLAYER_NUM)
		{
		case 1:
			for (int m = 0; m < cameras.Length; m++)
			{
				cameras[m].rect = SINGLE_PLAYERONLY_CAMERA_RECT[m];
			}
			for (int n = 0; n < directionalLight.Length; n++)
			{
				directionalLight[n].shadows = LightShadows.Hard;
			}
			break;
		case 2:
			for (int num = 0; num < cameras.Length; num++)
			{
				cameras[num].rect = TWO_PLAYER_CAMERA_RECT[num];
			}
			for (int num2 = 0; num2 < directionalLight.Length; num2++)
			{
				directionalLight[num2].shadows = LightShadows.None;
			}
			break;
		case 3:
			for (int k = 0; k < cameras.Length; k++)
			{
				cameras[k].rect = FOUR_PLAYER_CAMERA_RECT[k];
			}
			for (int l = 0; l < directionalLight.Length; l++)
			{
				directionalLight[l].shadows = LightShadows.None;
			}
			break;
		case 4:
			for (int i = 0; i < cameras.Length; i++)
			{
				cameras[i].rect = FOUR_PLAYER_CAMERA_RECT[i];
			}
			for (int j = 0; j < directionalLight.Length; j++)
			{
				directionalLight[j].shadows = LightShadows.None;
			}
			break;
		}
	}
	public void PlayerGameStart()
	{
		players[0].Hand.StartMethod();
		players[1].Hand.StartMethod();
		players[2].Hand.StartMethod();
		players[3].Hand.StartMethod();
	}
	private void Init_UserData()
	{
		userData_Group1 = new UserData[HandSeal_Define.MEMBER_NUM];
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			userData_Group1[i].player = players[i];
			if (HandSeal_Define.PLAYER_NUM == 3 && i == userData_Group1.Length - 1)
			{
				userData_Group1[i].userType = HandSeal_Define.UserType.CPU_1;
			}
			else
			{
				userData_Group1[i].userType = (HandSeal_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
			}
			userData_Group1[i].point = 0;
			userData_Group1[i].camera = cameras[i];
			userData_Group1[i].isPlayer = (userData_Group1[i].userType <= HandSeal_Define.UserType.PLAYER_4);
			userData_Group1[i].player.Init(userData_Group1[i].userType);
		}
		HandSeal_Define.UIM.SetUserUIData(userData_Group1);
	}
	public void UpdateMethod()
	{
		if (HandSeal_Define.GM.IsDuringGame())
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
	public UserData GetUserData(HandSeal_Define.UserType _userType)
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
	public Camera GetUserCamera(HandSeal_Define.UserType _userType)
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
	public bool PlayerGoalCheck()
	{
		int num = 0;
		for (int i = 0; i < HandSeal_Define.MEMBER_NUM; i++)
		{
			if (userData_Group1[i].player.Hand.processType == HandSeal_Hand.GameProcessType.END && userData_Group1[i].player.UserType <= HandSeal_Define.UserType.PLAYER_4)
			{
				num++;
			}
		}
		if (num == HandSeal_Define.PLAYER_NUM)
		{
			return true;
		}
		return false;
	}
	public void SetPoint(HandSeal_Define.UserType _userType, int _point)
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
		HandSeal_Define.UIM.SetScore(_userType, userData_Group1[num].point);
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
				if (userData_Group1[i].userType <= HandSeal_Define.UserType.PLAYER_4)
				{
					SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group1[i].userType);
				}
			}
			return;
		}
		for (int j = 0; j < userData_Group2.Length; j++)
		{
			if (userData_Group2[j].userType <= HandSeal_Define.UserType.PLAYER_4)
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
