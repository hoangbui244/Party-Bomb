using System;
using System.Collections.Generic;
using UnityEngine;
public class Skijump_CharacterManager : SingletonCustom<Skijump_CharacterManager>
{
	[Serializable]
	public struct UserData
	{
		public Skijump_Define.UserType userType;
		public Skijump_Define.TeamType teamType;
		public Skijump_Character character;
		public float totalScore;
		public bool isPlayer;
	}
	[SerializeField]
	[Header("キャラクタ\u30fcプレハブ")]
	private Skijump_Character character;
	[SerializeField]
	[Header("キャラクタ\u30fcの親アンカ\u30fc")]
	private Transform charaRootAnchor;
	private UserData[] userDatas;
	private Skijump_Define.UserType nowJumpUserType;
	private int nowJumpCount;
	private int nowOrderNo;
	private List<Skijump_Define.UserType> memberOrder = new List<Skijump_Define.UserType>();
	private bool isNextJumpReadyWait;
	private Skijump_CpuArtificialIntelligence cpuAi = new Skijump_CpuArtificialIntelligence();
	public Skijump_Define.UserType NowJumpUserType => nowJumpUserType;
	public int NowJumpCount => nowJumpCount;
	public List<Skijump_Define.UserType> MemberOrder => memberOrder;
	public Skijump_CpuArtificialIntelligence CpuAi => cpuAi;
	public void Init()
	{
		userDatas = new UserData[Skijump_Define.MEMBER_NUM];
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
		{
			for (int i = 0; i < userDatas.Length; i++)
			{
				userDatas[i].character = UnityEngine.Object.Instantiate(character, Vector3.zero, Quaternion.identity, charaRootAnchor);
				userDatas[i].character.transform.localPosition = Vector3.zero;
				userDatas[i].character.transform.localEulerAngles = Vector3.zero;
				userDatas[i].character.transform.localScale = Vector3.one;
				userDatas[i].userType = (Skijump_Define.UserType)i;
				userDatas[i].teamType = (Skijump_Define.TeamType)i;
				userDatas[i].totalScore = 0f;
				userDatas[i].isPlayer = ((int)userDatas[i].userType < SingletonCustom<GameSettingManager>.Instance.PlayerNum);
				userDatas[i].character.Init(userDatas[i].userType, userDatas[i].teamType, userDatas[i].isPlayer);
				memberOrder.Add(userDatas[i].userType);
			}
		}
		nowJumpUserType = memberOrder[nowOrderNo];
		for (int j = 0; j < userDatas.Length; j++)
		{
			if (userDatas[j].userType != nowJumpUserType)
			{
				userDatas[j].character.HideCharacter();
			}
		}
		cpuAi.Init();
		SingletonCustom<Skijump_UIManager>.Instance.SetJumpCountText(nowOrderNo + nowJumpCount * memberOrder.Count);
	}
	public void UpdateMethod()
	{
		if (!Skijump_Define.MGM.IsDuringGame() || isNextJumpReadyWait)
		{
			return;
		}
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType == nowJumpUserType)
			{
				userDatas[i].character.UpdateMethod();
				if (userDatas[i].character.OperationState == Skijump_Define.OperationState.NEXT)
				{
					isNextJumpReadyWait = true;
					SingletonCustom<Skijump_UIManager>.Instance.FadePrevFrame(0.5f, 0.05f, 0f, delegate
					{
						NextJump();
					});
				}
			}
		}
	}
	public void GameStartProcess()
	{
		FirstJumpUserReady();
	}
	public void GameEndProcess()
	{
		SetAllUserTotalScore();
	}
	private void FirstJumpUserReady()
	{
		int num = 0;
		while (true)
		{
			if (num < userDatas.Length)
			{
				if (userDatas[num].userType == nowJumpUserType)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		userDatas[num].character.SetJumpReady();
		if (userDatas[num].isPlayer)
		{
			Skijump_Define.GUIM.PlayTurnCutIn(nowJumpUserType);
		}
	}
	public void NextJump()
	{
		isNextJumpReadyWait = false;
		if ((float)nowJumpCount == Skijump_Define.MAX_JUMP_NUM - 1f)
		{
			nowOrderNo++;
			if (nowOrderNo == memberOrder.Count)
			{
				Skijump_Define.MGM.GameEnd();
			}
			else
			{
				NextChara();
			}
			return;
		}
		nowOrderNo++;
		if (nowOrderNo == memberOrder.Count)
		{
			nowOrderNo = 0;
			nowJumpCount++;
		}
		NextChara();
	}
	private void NextChara()
	{
		nowJumpUserType = memberOrder[nowOrderNo];
		UnityEngine.Debug.Log("次のキャラ>" + nowJumpUserType.ToString());
		SingletonCustom<Skijump_UIManager>.Instance.SetJumpCountText(nowOrderNo + nowJumpCount * memberOrder.Count);
		SingletonCustom<Skijump_UIManager>.Instance.CloseJumpScore();
		SingletonCustom<Skijump_CameraWorkManager>.Instance.SetState(Skijump_CameraWorkManager.State.STANDBY);
		Skijump_Define.GUIM.WindManager.SetChangeWindDir(_flg: true);
		SingletonCustom<Skijump_WindManager>.Instance.ResetWindData();
		SingletonCustom<Skijump_WindManager>.Instance.UpdateMethod();
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType != nowJumpUserType)
			{
				userDatas[i].character.HideCharacter();
				continue;
			}
			userDatas[i].character.SetJumpReady();
			if (userDatas[i].isPlayer)
			{
				Skijump_Define.GUIM.PlayTurnCutIn(nowJumpUserType);
			}
		}
		cpuAi.Init();
	}
	private void SetAllUserTotalScore()
	{
		for (int i = 0; i < userDatas.Length; i++)
		{
			userDatas[i].totalScore = Skijump_Define.GUIM.GetTotalScore(userDatas[i].userType);
		}
	}
	public Skijump_Character GetNowJumpChara()
	{
		for (int i = 0; i < userDatas.Length; i++)
		{
			if (userDatas[i].userType == nowJumpUserType)
			{
				return userDatas[i].character;
			}
		}
		return null;
	}
	public int[] GetAllUserNo()
	{
		int[] array = new int[userDatas.Length];
		for (int i = 0; i < userDatas.Length; i++)
		{
			array[i] = (int)userDatas[i].userType;
			if (array[i] >= SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
				array[i] = 4 + (i - SingletonCustom<GameSettingManager>.Instance.PlayerNum);
			}
		}
		return array;
	}
	public int[] GetAllUserTotalScore()
	{
		int[] array = new int[userDatas.Length];
		for (int i = 0; i < userDatas.Length; i++)
		{
			array[i] = (int)userDatas[i].totalScore;
		}
		return array;
	}
}
