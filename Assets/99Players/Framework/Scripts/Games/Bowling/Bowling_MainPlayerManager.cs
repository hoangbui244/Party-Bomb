using System;
using System.Collections.Generic;
using UnityEngine;
public class Bowling_MainPlayerManager : SingletonCustom<Bowling_MainPlayerManager>
{
	[Serializable]
	public struct UserData
	{
		public Bowling_Define.UserType userType;
		public Bowling_Define.TeamType teamType;
		public Bowling_Player player;
		public int totalScore;
		public bool isPlayer;
	}
	[SerializeField]
	[Header("プレイヤ\u30fcプレハブ")]
	private Bowling_Player player;
	[SerializeField]
	[Header("プレイヤ\u30fcの親アンカ\u30fc")]
	private Transform playerRootAnchor;
	[SerializeField]
	[Header("ユ\u30fcザ\u30fcのボ\u30fcルマテリアルデ\u30fcタ")]
	private Material[] userBallMaterials;
	private UserData[] userDatas;
	private Bowling_Define.UserType nowThrowUserType;
	private int nowThrowCount;
	private int nowOrderNo;
	private List<Bowling_Define.UserType> memberOrder = new List<Bowling_Define.UserType>();
	private int nowFrameNo;
	private readonly int MAX_FRAME_NO = 2;
	private bool isNextThrowReadyWait;
	public Bowling_Define.UserType NowThrowUserType => nowThrowUserType;
	public int NowThrowCount => nowThrowCount;
	public List<Bowling_Define.UserType> MemberOrder => memberOrder;
	public int NowFrameNo => nowFrameNo;
	public void Init()
	{
		userDatas = new UserData[Bowling_Define.MEMBER_NUM];
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			for (int i = 0; i < userDatas.Length; i++)
			{
				userDatas[i].player = UnityEngine.Object.Instantiate(player, Vector3.zero, Quaternion.identity, playerRootAnchor);
				userDatas[i].player.transform.localPosition = Vector3.zero;
				userDatas[i].player.transform.localEulerAngles = Vector3.zero;
				userDatas[i].player.transform.localScale = Vector3.one;
				userDatas[i].userType = (Bowling_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
				userDatas[i].teamType = (Bowling_Define.TeamType)i;
				userDatas[i].totalScore = 0;
				userDatas[i].isPlayer = (userDatas[i].userType <= Bowling_Define.UserType.PLAYER_4);
				userDatas[i].player.Init(userDatas[i].userType, userDatas[i].teamType, userDatas[i].isPlayer);
				if (userDatas[i].userType >= Bowling_Define.UserType.CPU_1)
				{
					userDatas[i].player.GetBall().SetBallMaterial(userBallMaterials[Bowling_Define.GetConvertCPUNo(userDatas[i].userType)]);
				}
				else
				{
					userDatas[i].player.GetBall().SetBallMaterial(userBallMaterials[(int)userDatas[i].userType]);
				}
				memberOrder.Add(userDatas[i].userType);
			}
			if (Bowling_Define.PLAYER_NUM != 1)
			{
				memberOrder.Shuffle();
			}
		}
		else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			int num = 0;
			List<Bowling_Define.UserType> list = new List<Bowling_Define.UserType>();
			List<Bowling_Define.UserType> list2 = new List<Bowling_Define.UserType>();
			for (int j = 0; j < Bowling_Define.MAX_COOP_TEAM_NUM; j++)
			{
				for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j].Count; k++)
				{
					userDatas[num].player = UnityEngine.Object.Instantiate(player, Vector3.zero, Quaternion.identity, playerRootAnchor);
					userDatas[num].player.transform.localPosition = Vector3.zero;
					userDatas[num].player.transform.localEulerAngles = Vector3.zero;
					userDatas[num].player.transform.localScale = Vector3.one;
					userDatas[num].userType = (Bowling_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][k];
					userDatas[num].teamType = (Bowling_Define.TeamType)j;
					userDatas[num].totalScore = 0;
					userDatas[num].isPlayer = (userDatas[num].userType <= Bowling_Define.UserType.PLAYER_4);
					userDatas[num].player.Init(userDatas[num].userType, userDatas[num].teamType, userDatas[num].isPlayer);
					if (userDatas[num].userType >= Bowling_Define.UserType.CPU_1)
					{
						userDatas[num].player.GetBall().SetBallMaterial(userBallMaterials[Bowling_Define.GetConvertCPUNo(userDatas[num].userType)]);
					}
					else
					{
						userDatas[num].player.GetBall().SetBallMaterial(userBallMaterials[(int)userDatas[num].userType]);
					}
					if (userDatas[num].teamType == Bowling_Define.TeamType.TEAM_A)
					{
						list.Add(userDatas[num].userType);
					}
					else if (userDatas[num].teamType == Bowling_Define.TeamType.TEAM_B)
					{
						list2.Add(userDatas[num].userType);
					}
					num++;
				}
			}
			list.Shuffle();
			list2.Shuffle();
			for (int l = 0; l < Bowling_Define.MEMBER_NUM; l++)
			{
				if (l % 2 == 0)
				{
					memberOrder.Add(list[0]);
					list.RemoveAt(0);
				}
				else
				{
					memberOrder.Add(list2[0]);
					list2.RemoveAt(0);
				}
			}
		}
		else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP)
		{
			int num2 = 0;
			List<Bowling_Define.UserType> list3 = new List<Bowling_Define.UserType>();
			List<Bowling_Define.UserType> list4 = new List<Bowling_Define.UserType>();
			for (int m = 0; m < Bowling_Define.MAX_COOP_TEAM_NUM; m++)
			{
				for (int n = 0; n < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[m].Count; n++)
				{
					userDatas[num2].player = UnityEngine.Object.Instantiate(player, Vector3.zero, Quaternion.identity, playerRootAnchor);
					userDatas[num2].player.transform.localPosition = Vector3.zero;
					userDatas[num2].player.transform.localEulerAngles = Vector3.zero;
					userDatas[num2].player.transform.localScale = Vector3.one;
					userDatas[num2].userType = (Bowling_Define.UserType)SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[m][n];
					userDatas[num2].teamType = (Bowling_Define.TeamType)m;
					userDatas[num2].totalScore = 0;
					userDatas[num2].isPlayer = (userDatas[num2].userType <= Bowling_Define.UserType.PLAYER_4);
					userDatas[num2].player.Init(userDatas[num2].userType, userDatas[num2].teamType, userDatas[num2].isPlayer);
					if (userDatas[num2].userType >= Bowling_Define.UserType.CPU_1)
					{
						userDatas[num2].player.GetBall().SetBallMaterial(userBallMaterials[Bowling_Define.GetConvertCPUNo(userDatas[num2].userType)]);
					}
					else
					{
						userDatas[num2].player.GetBall().SetBallMaterial(userBallMaterials[(int)userDatas[num2].userType]);
					}
					if (userDatas[num2].teamType == Bowling_Define.TeamType.TEAM_A)
					{
						list3.Add(userDatas[num2].userType);
					}
					else if (userDatas[num2].teamType == Bowling_Define.TeamType.TEAM_B)
					{
						list4.Add(userDatas[num2].userType);
					}
					num2++;
				}
			}
			list3.Shuffle();
			list4.Shuffle();
			for (int num3 = 0; num3 < Bowling_Define.MEMBER_NUM; num3++)
			{
				if (num3 % 2 == 0)
				{
					memberOrder.Add(list3[0]);
					list3.RemoveAt(0);
				}
				else
				{
					memberOrder.Add(list4[0]);
					list4.RemoveAt(0);
				}
			}
		}
		nowThrowUserType = memberOrder[nowOrderNo];
		for (int num4 = 0; num4 < userDatas.Length; num4++)
		{
			if (userDatas[num4].userType != nowThrowUserType)
			{
				userDatas[num4].player.HidePlayerBall();
			}
		}
	}
	public void UpdateMethod()
	{
		if (!Bowling_Define.MGM.IsDuringGame() || isNextThrowReadyWait)
		{
			return;
		}
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType != nowThrowUserType)
			{
				continue;
			}
			userDatas[i].player.UpdateMethod();
			if (userDatas[i].player.GetBall().CheckBallState(Bowling_Ball.BallState.HOLE) || userDatas[i].player.GetBall().CheckBallState(Bowling_Ball.BallState.STOP))
			{
				if (Bowling_Define.MSM.IsPinAllStop())
				{
					isNextThrowReadyWait = true;
					Bowling_Define.MSM.CheckPinFall(GetNowThrowUserBall().IsGutter);
					Bowling_Define.GUIM.SetScore(nowThrowUserType, nowFrameNo, nowThrowCount, Bowling_Define.MSM.IsPinStand(), GetNowThrowUserBall().IsGutter);
					LeanTween.delayedCall(Bowling_Define.GUIM.PlayThrowResultEffect(), (Action)delegate
					{
						if (Bowling_Define.GUIM.GetScoreData(nowThrowUserType).GetContinuousStrikeNum() == Bowling_Define.PLAY_FRAME_NUM + 1)
						{
							LeanTween.delayedCall(Bowling_Define.GUIM.PlayPerfectEffect(), (Action)delegate
							{
								NextBall();
							});
						}
						else
						{
							NextBall();
						}
					});
				}
			}
			else if (userDatas[i].player.GetBall().CheckBallState(Bowling_Ball.BallState.ROLL) || userDatas[i].player.GetBall().CheckBallState(Bowling_Ball.BallState.GUTTER))
			{
				Bowling_Define.MSM.CheckPinStop();
				Bowling_Define.MSM.MoveCameraWork();
			}
		}
		if (nowThrowUserType >= Bowling_Define.UserType.CPU_1 && GetNowThrowUser().OperationState == Bowling_Define.OperationState.BALL_MOVE && Bowling_Define.CM.IsPushButton_X(Bowling_Define.UserType.PLAYER_1, Bowling_ControllerManager.ButtonPushType.DOWN))
		{
			GetNowThrowUser().SkipCPU();
			Bowling_Define.GUIM.StartScreenFade(GetNowThrowUser().SkipCPUThrow, null, 0.5f);
		}
	}
	public void GameStartProcess()
	{
		FirstThrowUserReady();
	}
	public void GameEndProcess()
	{
		SetAllUserTotalScore();
	}
	public void NextBall()
	{
		bool flag = false;
		isNextThrowReadyWait = false;
		nowThrowCount++;
		if (nowFrameNo == MAX_FRAME_NO)
		{
			if (nowThrowCount == 3)
			{
				nowThrowCount = 0;
				nowOrderNo++;
				if (nowOrderNo == memberOrder.Count)
				{
					Bowling_Define.MGM.GameEnd();
					return;
				}
				NextUser();
				flag = true;
			}
			else if (nowThrowCount == 2)
			{
				if (Bowling_Define.GUIM.IsCanThreeThrow(nowThrowUserType))
				{
					for (int i = 0; i < userDatas.Length; i++)
					{
						if (userDatas[i].userType == nowThrowUserType)
						{
							userDatas[i].player.SetThrowBallReady();
						}
					}
				}
				else
				{
					nowThrowCount = 0;
					nowOrderNo++;
					if (nowOrderNo == memberOrder.Count)
					{
						Bowling_Define.MGM.GameEnd();
						return;
					}
					NextUser();
					flag = true;
				}
			}
			else
			{
				for (int j = 0; j < userDatas.Length; j++)
				{
					if (userDatas[j].userType == nowThrowUserType)
					{
						userDatas[j].player.SetThrowBallReady();
					}
				}
			}
		}
		else if (nowThrowCount == 2 || Bowling_Define.GUIM.GetScoreData(nowThrowUserType).frameList[nowFrameNo].IsStrike(nowThrowCount - 1))
		{
			nowThrowCount = 0;
			nowOrderNo++;
			if (nowOrderNo == memberOrder.Count)
			{
				nowFrameNo++;
				nowOrderNo = 0;
			}
			NextUser();
			flag = true;
		}
		else
		{
			for (int k = 0; k < userDatas.Length; k++)
			{
				if (userDatas[k].userType == nowThrowUserType)
				{
					userDatas[k].player.SetThrowBallReady();
				}
			}
		}
		Bowling_Define.MSM.ResetCameraPos();
		if (flag)
		{
			Bowling_Define.MSM.PinResetPos();
		}
		else
		{
			Bowling_Define.MSM.PinResetPos(Bowling_Define.MSM.IsPinAllFall());
		}
		Bowling_Define.GUIM.SetPinData(Bowling_Define.MSM.IsPinStand());
	}
	private void FirstThrowUserReady()
	{
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType == nowThrowUserType)
			{
				userDatas[i].player.SetThrowBallReady();
				if (userDatas[i].isPlayer)
				{
					Bowling_Define.GUIM.PlayTurnCutIn(nowThrowUserType);
				}
				break;
			}
		}
		Bowling_Define.GUIM.StartFrameFlashing(nowThrowUserType);
	}
	private void NextUser()
	{
		nowThrowUserType = memberOrder[nowOrderNo];
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType != nowThrowUserType)
			{
				userDatas[i].player.HidePlayerBall();
				continue;
			}
			userDatas[i].player.SetThrowBallReady();
			if (userDatas[i].isPlayer)
			{
				Bowling_Define.GUIM.PlayTurnCutIn(nowThrowUserType);
			}
		}
		Bowling_Define.GUIM.StartFrameFlashing(nowThrowUserType);
	}
	public bool IsPlayer(Bowling_Define.UserType _userType)
	{
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType == _userType)
			{
				return userDatas[i].isPlayer;
			}
		}
		return false;
	}
	public Bowling_Player GetUser(Bowling_Define.UserType _userType)
	{
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType == _userType)
			{
				return userDatas[i].player;
			}
		}
		return null;
	}
	public Bowling_Ball GetUserBall(Bowling_Define.UserType _userType)
	{
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType == _userType)
			{
				return userDatas[i].player.GetBall();
			}
		}
		return null;
	}
	public Bowling_Define.TeamType GetUserTeamType(Bowling_Define.UserType _userType)
	{
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType == _userType)
			{
				return userDatas[i].teamType;
			}
		}
		return Bowling_Define.TeamType.TEAM_A;
	}
	public Bowling_Player GetNowThrowUser()
	{
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType == nowThrowUserType)
			{
				return userDatas[i].player;
			}
		}
		return null;
	}
	public Bowling_Ball GetNowThrowUserBall()
	{
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType == nowThrowUserType)
			{
				return userDatas[i].player.GetBall();
			}
		}
		return null;
	}
	private void SetAllUserTotalScore()
	{
		for (int i = 0; i < userDatas.Length; i++)
		{
			userDatas[i].totalScore = Bowling_Define.GUIM.GetTotalScore(userDatas[i].userType);
		}
	}
	public int[] GetAllUserNo()
	{
		int[] array = new int[userDatas.Length];
		for (int i = 0; i < userDatas.Length; i++)
		{
			array[i] = (int)userDatas[i].userType;
		}
		return array;
	}
	public int[] GetAllUserTotalScore()
	{
		int[] array = new int[userDatas.Length];
		for (int i = 0; i < userDatas.Length; i++)
		{
			array[i] = userDatas[i].totalScore;
		}
		return array;
	}
	public int GetTeamTotalScore(Bowling_Define.TeamType _teamType)
	{
		int num = 0;
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].teamType == _teamType)
			{
				num += userDatas[i].totalScore;
			}
		}
		return num;
	}
}
