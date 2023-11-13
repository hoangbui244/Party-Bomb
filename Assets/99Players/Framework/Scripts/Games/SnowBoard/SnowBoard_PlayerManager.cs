using System;
using UnityEngine;
public class SnowBoard_PlayerManager : SingletonCustom<SnowBoard_PlayerManager>
{
	[Serializable]
	public struct UserData
	{
		public SnowBoard_Player player;
		public SnowBoard_Define.UserType userType;
		public Camera camera;
		public float goalTime;
		public bool isPlayer;
	}
	[SerializeField]
	[Header("プレイヤ\u30fc")]
	private SnowBoard_Player[] players;
	[SerializeField]
	[Header("プレイヤ\u30fc(好きな競技モ\u30fcド)")]
	private SnowBoard_Player[] players_SingleGame;
	[SerializeField]
	[Header("カメラ")]
	private Camera[] cameras;
	[SerializeField]
	[Header("ミニMAP(1,2,4人用の3つ設定)")]
	private CourseMapUI[] miniMap;
	[SerializeField]
	[Header("ミニMAP設定用snowBoard")]
	private Transform[] snowBoard;
	[SerializeField]
	[Header("ミニMAP設定用snowBoard(好きな競技モ\u30fcド)")]
	private Transform[] snowBoard_SingleGame;
	[SerializeField]
	[Header("ライト")]
	private Light DirectionalLight;
	private CourseMapUI targetMap;
	private UserData[] userData_Group1;
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
	public SnowBoard_Player[] Players => players;
	public UserData[] UserData_Group1 => userData_Group1;
	public void Init()
	{
		Init_UserData();
		switch (SnowBoard_Define.PLAYER_NUM)
		{
		case 1:
			for (int j = 0; j < cameras.Length; j++)
			{
				cameras[j].rect = SINGLE_PLAYERONLY_CAMERA_RECT[j];
			}
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE && SnowBoard_Define.GM.IsEightRun)
			{
				miniMap[0].SetWorldTargetAnchor(snowBoard_SingleGame);
			}
			else
			{
				miniMap[0].SetWorldTargetAnchor(snowBoard);
			}
			miniMap[0].Init();
			targetMap = miniMap[0];
			DirectionalLight.shadows = LightShadows.Hard;
			break;
		case 2:
			for (int l = 0; l < cameras.Length; l++)
			{
				cameras[l].rect = TWO_PLAYER_CAMERA_RECT[l];
			}
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE && SnowBoard_Define.GM.IsEightRun)
			{
				miniMap[1].SetWorldTargetAnchor(snowBoard_SingleGame);
			}
			else
			{
				miniMap[1].SetWorldTargetAnchor(snowBoard);
			}
			miniMap[1].Init();
			targetMap = miniMap[1];
			DirectionalLight.shadows = LightShadows.None;
			break;
		case 3:
			for (int k = 0; k < cameras.Length; k++)
			{
				cameras[k].rect = FOUR_PLAYER_CAMERA_RECT[k];
			}
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE && SnowBoard_Define.GM.IsEightRun)
			{
				miniMap[2].SetWorldTargetAnchor(snowBoard_SingleGame);
			}
			else
			{
				miniMap[2].SetWorldTargetAnchor(snowBoard);
			}
			miniMap[2].Init();
			targetMap = miniMap[2];
			DirectionalLight.shadows = LightShadows.None;
			break;
		case 4:
			for (int i = 0; i < cameras.Length; i++)
			{
				cameras[i].rect = FOUR_PLAYER_CAMERA_RECT[i];
			}
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE && SnowBoard_Define.GM.IsEightRun)
			{
				miniMap[2].SetWorldTargetAnchor(snowBoard_SingleGame);
			}
			else
			{
				miniMap[2].SetWorldTargetAnchor(snowBoard);
			}
			miniMap[2].Init();
			targetMap = miniMap[2];
			DirectionalLight.shadows = LightShadows.None;
			break;
		}
	}
	public void PlayerGameStart()
	{
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE && SnowBoard_Define.GM.IsEightRun)
		{
			players_SingleGame[0].SkiBoard.StartMethod();
			players_SingleGame[1].SkiBoard.StartMethod();
			players_SingleGame[2].SkiBoard.StartMethod();
			players_SingleGame[3].SkiBoard.StartMethod();
			players_SingleGame[4].SkiBoard.StartMethod();
			players_SingleGame[5].SkiBoard.StartMethod();
			players_SingleGame[6].SkiBoard.StartMethod();
			players_SingleGame[7].SkiBoard.StartMethod();
		}
		else
		{
			players[0].SkiBoard.StartMethod();
			players[1].SkiBoard.StartMethod();
			players[2].SkiBoard.StartMethod();
			players[3].SkiBoard.StartMethod();
		}
	}
	private void Init_UserData()
	{
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType == GameSettingManager.GameProgressType.SINGLE && SnowBoard_Define.GM.IsEightRun)
		{
			userData_Group1 = new UserData[SnowBoard_Define.MEMBER_NUM];
			switch (SnowBoard_Define.PLAYER_NUM)
			{
			case 1:
				userData_Group1[0].userType = SnowBoard_Define.UserType.PLAYER_1;
				userData_Group1[1].userType = SnowBoard_Define.UserType.CPU_1;
				userData_Group1[2].userType = SnowBoard_Define.UserType.CPU_2;
				userData_Group1[3].userType = SnowBoard_Define.UserType.CPU_3;
				userData_Group1[4].userType = SnowBoard_Define.UserType.CPU_4;
				userData_Group1[5].userType = SnowBoard_Define.UserType.CPU_5;
				userData_Group1[6].userType = SnowBoard_Define.UserType.CPU_6;
				userData_Group1[7].userType = SnowBoard_Define.UserType.CPU_7;
				break;
			case 2:
				userData_Group1[0].userType = SnowBoard_Define.UserType.PLAYER_1;
				userData_Group1[1].userType = SnowBoard_Define.UserType.PLAYER_2;
				userData_Group1[2].userType = SnowBoard_Define.UserType.CPU_1;
				userData_Group1[3].userType = SnowBoard_Define.UserType.CPU_2;
				userData_Group1[4].userType = SnowBoard_Define.UserType.CPU_3;
				userData_Group1[5].userType = SnowBoard_Define.UserType.CPU_4;
				userData_Group1[6].userType = SnowBoard_Define.UserType.CPU_5;
				userData_Group1[7].userType = SnowBoard_Define.UserType.CPU_6;
				break;
			case 3:
				userData_Group1[0].userType = SnowBoard_Define.UserType.PLAYER_1;
				userData_Group1[1].userType = SnowBoard_Define.UserType.PLAYER_2;
				userData_Group1[2].userType = SnowBoard_Define.UserType.PLAYER_3;
				userData_Group1[3].userType = SnowBoard_Define.UserType.CPU_1;
				userData_Group1[4].userType = SnowBoard_Define.UserType.CPU_2;
				userData_Group1[5].userType = SnowBoard_Define.UserType.CPU_3;
				userData_Group1[6].userType = SnowBoard_Define.UserType.CPU_4;
				userData_Group1[7].userType = SnowBoard_Define.UserType.CPU_5;
				break;
			case 4:
				userData_Group1[0].userType = SnowBoard_Define.UserType.PLAYER_1;
				userData_Group1[1].userType = SnowBoard_Define.UserType.PLAYER_2;
				userData_Group1[2].userType = SnowBoard_Define.UserType.PLAYER_3;
				userData_Group1[3].userType = SnowBoard_Define.UserType.PLAYER_4;
				userData_Group1[4].userType = SnowBoard_Define.UserType.CPU_1;
				userData_Group1[5].userType = SnowBoard_Define.UserType.CPU_2;
				userData_Group1[6].userType = SnowBoard_Define.UserType.CPU_3;
				userData_Group1[7].userType = SnowBoard_Define.UserType.CPU_4;
				break;
			}
			for (int i = 0; i < userData_Group1.Length; i++)
			{
				userData_Group1[i].player = players_SingleGame[i];
				userData_Group1[i].goalTime = 180f;
				if (i < cameras.Length)
				{
					userData_Group1[i].camera = cameras[i];
				}
				userData_Group1[i].isPlayer = (userData_Group1[i].userType <= SnowBoard_Define.UserType.PLAYER_4);
				userData_Group1[i].player.Init(userData_Group1[i].userType);
			}
			SnowBoard_Define.UIM.SetUserUIData(userData_Group1);
			return;
		}
		userData_Group1 = new UserData[SnowBoard_Define.MEMBER_NUM];
		for (int j = 0; j < userData_Group1.Length; j++)
		{
			userData_Group1[j].player = players[j];
			if (SnowBoard_Define.PLAYER_NUM == 3 && j == userData_Group1.Length - 1)
			{
				userData_Group1[j].userType = SnowBoard_Define.UserType.CPU_1;
			}
			else
			{
				userData_Group1[j].userType = (SnowBoard_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][0];
			}
			userData_Group1[j].goalTime = 180f;
			userData_Group1[j].camera = cameras[j];
			userData_Group1[j].isPlayer = (userData_Group1[j].userType <= SnowBoard_Define.UserType.PLAYER_4);
			userData_Group1[j].player.Init(userData_Group1[j].userType);
		}
		SnowBoard_Define.UIM.SetUserUIData(userData_Group1);
		players_SingleGame[4].gameObject.SetActive(value: false);
		players_SingleGame[5].gameObject.SetActive(value: false);
		players_SingleGame[6].gameObject.SetActive(value: false);
		players_SingleGame[7].gameObject.SetActive(value: false);
	}
	public void UpdateMethod()
	{
		targetMap.UpdateMethod();
		if (SnowBoard_Define.GM.IsDuringGame())
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
	public UserData GetUserData(SnowBoard_Define.UserType _userType)
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
		return userData_Group1.Length;
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
	public Camera GetUserCamera(SnowBoard_Define.UserType _userType)
	{
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			if (userData_Group1[i].userType == _userType)
			{
				return userData_Group1[i].camera;
			}
		}
		return null;
	}
	public bool CheckPlayerIsGround()
	{
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].UserType <= SnowBoard_Define.UserType.PLAYER_4 && !players[i].SkiBoard.IsGround)
			{
				return false;
			}
		}
		return true;
	}
	public bool CheckPlayerIsRail()
	{
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].UserType <= SnowBoard_Define.UserType.PLAYER_4 && !players[i].SkiBoard.IsRail)
			{
				return false;
			}
		}
		return true;
	}
	public void SetGoalTime(SnowBoard_Define.UserType _userType, float _setTime)
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
		for (int i = 0; i < SnowBoard_Define.MEMBER_NUM; i++)
		{
			if (userData_Group1[i].player.SkiBoard.processType == SnowBoard_SkiBoard.SkiBoardProcessType.GOAL && userData_Group1[i].player.UserType <= SnowBoard_Define.UserType.PLAYER_4)
			{
				num++;
			}
		}
		if (num == SnowBoard_Define.PLAYER_NUM)
		{
			return true;
		}
		return false;
	}
	public bool PlayerOnlyOneCheck()
	{
		int num = 0;
		for (int i = 0; i < SnowBoard_Define.MEMBER_NUM; i++)
		{
			if (userData_Group1[i].player.SkiBoard.processType == SnowBoard_SkiBoard.SkiBoardProcessType.GOAL && userData_Group1[i].player.UserType <= SnowBoard_Define.UserType.PLAYER_4)
			{
				num++;
			}
		}
		if (num + 1 == SnowBoard_Define.PLAYER_NUM)
		{
			return true;
		}
		return false;
	}
	public void SetGroupVibration()
	{
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum <= 2 || SingletonCustom<GameSettingManager>.Instance.SelectGameFormat != GS_Define.GameFormat.COOP)
		{
			return;
		}
		for (int i = 0; i < userData_Group1.Length; i++)
		{
			if (userData_Group1[i].userType <= SnowBoard_Define.UserType.PLAYER_4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)userData_Group1[i].userType);
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
