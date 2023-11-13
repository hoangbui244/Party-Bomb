using GamepadInput;
using SaveDataDefine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BeachFlag_GameManager : SingletonCustom<BeachFlag_GameManager>
{
	public enum GameState
	{
		GAME_START_STANDBY,
		GAME_START_WAIT,
		ROUND_START_STANDBY,
		ROUND_CONTINUE_START_STANDBY,
		ROUND_START_WAIT,
		DURING_GAME,
		ROUND_END,
		ROUND_END_WAIT,
		ROUND_CONTINUE_WAIT,
		GAME_END,
		GAME_END_WAIT,
		ANIMATION_WAIT,
		NONE
	}
	public enum MovePattern
	{
		Normal,
		League_2ndRound,
		League_3rdRound,
		Tournament_UpDownSide,
		Tournament_FinalRound_UpSideOnly,
		Tournament_FinalRound_DownSideOnly,
		Tournament_FinalRound_UpSide,
		Tournament_FinalRound_DownSide,
		LineUp,
		Addmission
	}
	[Serializable]
	public struct TeamData
	{
		public int teamNo;
		public List<int> memberPlayerNoList;
	}
	public enum PositionSideType
	{
		LeftSide,
		RightSide,
		BackSide,
		FrontSide
	}
	public enum RoundType
	{
		Round_1st,
		Round_2nd,
		LoserBattle,
		Round_Final,
		None
	}
	public delegate void GameStateMethod();
	[SerializeField]
	[Header("順位：リザルト")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("競技中に見えなくなる柵")]
	public GameObject signboard;
	private bool wait_done;
	private bool in_skip;
	private TeamData[] currentTeamData = new TeamData[4];
	private GameState gameState;
	private bool isGameStart;
	private GameStateMethod gameStateMethod;
	private float settingStartTime;
	private float restartTime;
	private bool isAutoMove;
	private bool isStopChara = true;
	private Action gameEndCallBack;
	[SerializeField]
	[Header("UIエフェクトアンカ\u30fc")]
	private Transform effectAnchor;
	[SerializeField]
	[Header("フェ\u30fcド画像")]
	private SpriteRenderer fadeSprite;
	private int[] playerGetSetNum = new int[BeachFlag_Define.MEMBER_NUM];
	private int gameSetNum = 1;
	private int playerNum = 1;
	private int winnerPlayerNo = -1;
	private int winnerTeamNo = -1;
	private bool isFadeAnimation;
	private bool isGameStartEffect;
	private bool isStartCountDown;
	private string[] tournamentVSTeamData = new string[2];
	private List<int> randomTeamNoList = new List<int>();
	private RoundType currentTournamentType;
	private bool isStopTime = true;
	private bool isGoal;
	private List<int> teamNoList;
	public bool InSkip => in_skip;
	public bool IsGameStart
	{
		get
		{
			return isGameStart;
		}
		set
		{
			isGameStart = value;
		}
	}
	public int GameSetNum => gameSetNum;
	public bool IsGameStartEffect => isGameStartEffect;
	public bool IsStartCountDown => isStartCountDown;
	public RoundType CurrentTournamentType => currentTournamentType;
	public bool IsStopTime => isStopTime;
	public void Init(Action _gameEndCallBack)
	{
		in_skip = false;
		isGameStart = false;
		wait_done = false;
		gameEndCallBack = _gameEndCallBack;
		settingStartTime = 3f;
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		gameSetNum = 1;
		if (!isFadeAnimation)
		{
			fadeSprite.color = new Color(0f, 0f, 0f, 0f);
		}
		for (int i = 0; i < playerGetSetNum.Length; i++)
		{
			playerGetSetNum[i] = 0;
		}
		if (teamNoList == null)
		{
			teamNoList = new List<int>();
		}
		teamNoList.Clear();
		for (int j = 0; j < 4; j++)
		{
			teamNoList.Add(j);
		}
		SingletonCustom<GameSettingManager>.Instance.TeamNum = 4;
		SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
		for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.TeamNum; k++)
		{
			randomTeamNoList.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[k][0]);
		}
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 2:
			teamNoList[1] = 4;
			teamNoList[1] = 1;
			randomTeamNoList[1] = 4;
			randomTeamNoList[2] = 1;
			break;
		case 3:
		case 4:
			teamNoList.Shuffle();
			randomTeamNoList.Shuffle();
			break;
		}
		InitialSettingTeamData(PositionSideType.LeftSide);
		InitialSettingTeamData(PositionSideType.RightSide);
		InitialSettingTeamData(PositionSideType.BackSide);
		InitialSettingTeamData(PositionSideType.FrontSide);
		tournamentVSTeamData[0] = (currentTeamData[0].teamNo + 1).ToString() + "-" + (currentTeamData[1].teamNo + 1).ToString();
		tournamentVSTeamData[1] = (currentTeamData[2].teamNo + 1).ToString() + "-" + (currentTeamData[3].teamNo + 1).ToString();
		SingletonCustom<BeachFlag_UIManager>.Instance.SetTournamentTeamData();
		GameStartStandby();
	}
	public void UpdateMethod()
	{
		if (isGameStart && !isStopTime && CharacterGoalCheck())
		{
			for (int i = 0; i < BeachFlag_Define.PM.Players.Length; i++)
			{
				BeachFlag_Define.PM.Players[i].Chara.EffectReset();
			}
			EndRound();
		}
		if (gameStateMethod == null)
		{
			return;
		}
		gameStateMethod();
		if ((BeachFlag_Define.UIM.IsSkipShow && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButton(SatGamePad.Button.X)) || (Application.isEditor && UnityEngine.Input.GetKeyDown(KeyCode.R)))
		{
			BeachFlag_Define.UIM.HideSkip();
			isStopTime = true;
			in_skip = true;
			for (int j = 0; j < BeachFlag_Define.PM.Players.Length; j++)
			{
				BeachFlag_Define.PM.Players[j].Chara.EffectReset();
			}
			int num = (!CalcManager.GetHalfProbability()) ? 1 : 0;
			BeachFlag_Define.PM.Players[num].Chara.SetGoal();
			AddSet(num);
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
			gameState = GameState.ROUND_END_WAIT;
			gameStateMethod = RoundEndWait;
		}
	}
	public bool PlayerGoal()
	{
		if (isGameStart && !isStopTime)
		{
			if (!isGoal)
			{
				isGoal = true;
				EndRound();
				return true;
			}
			return false;
		}
		return false;
	}
	private void GameStartStandby()
	{
		BeachFlag_Define.PM.Set_UserData();
		for (int i = 0; i < BeachFlag_Define.PM.Players.Length; i++)
		{
			BeachFlag_Define.PM.Boards[i].SetIcon();
			BeachFlag_Define.PM.Players[i].isover = false;
			BeachFlag_Define.PM.Players[i].Chara.EffectReset();
		}
		for (int j = 0; j < 4; j++)
		{
			BeachFlag_Define.PM.Boards[j].SetIcon();
		}
		if (BeachFlag_Define.PM.Players[0].UserType < BeachFlag_Define.PM.Players[1].UserType)
		{
			BeachFlag_Define.PM.Players[0].Chara.Cart.m_Path = BeachFlag_Define.PM.Carts[0];
			BeachFlag_Define.PM.Players[1].Chara.Cart.m_Path = BeachFlag_Define.PM.Carts[1];
		}
		else
		{
			BeachFlag_Define.PM.Players[0].Chara.Cart.m_Path = BeachFlag_Define.PM.Carts[1];
			BeachFlag_Define.PM.Players[1].Chara.Cart.m_Path = BeachFlag_Define.PM.Carts[0];
		}
		isStopChara = true;
		restartTime = 1f;
		isGameStart = false;
		isGameStartEffect = false;
		gameState = GameState.GAME_START_WAIT;
		gameStateMethod = GameStartWait;
	}
	private void GameStartWait()
	{
		in_skip = false;
		restartTime -= Time.deltaTime;
		if (restartTime <= 0f && !isGameStartEffect && !isGameStart)
		{
			for (int i = 0; i < BeachFlag_Define.PM.Players.Length; i++)
			{
				BeachFlag_Define.PM.Players[i].Chara.EffectReset();
				BeachFlag_Define.PM.Players[i].Chara.PlayerInit(BeachFlag_Define.PM.Players[i]);
			}
			if (!signboard.activeInHierarchy)
			{
				signboard.SetActive(value: true);
			}
			SingletonCustom<CommonStartProduction>.Instance.Play(delegate
			{
				if (BeachFlag_Define.PM.Players[0].UserType >= BeachFlag_Define.UserType.CPU_1 && BeachFlag_Define.PM.Players[1].UserType >= BeachFlag_Define.UserType.CPU_1)
				{
					BeachFlag_Define.UIM.ShowSkip();
				}
				else
				{
					BeachFlag_Define.UIM.HideSkip();
				}
				isGameStartEffect = true;
				restartTime = 2f;
			});
			StartCoroutine(WaitCountDown());
		}
	}
	private void RoundContinueStartStandby()
	{
		BeachFlag_Define.PM.Set_UserData();
		for (int i = 0; i < BeachFlag_Define.PM.Players.Length; i++)
		{
			BeachFlag_Define.PM.Boards[i].SetIcon();
			BeachFlag_Define.PM.Players[i].isover = false;
			BeachFlag_Define.PM.Players[i].Chara.EffectReset();
		}
		for (int j = 0; j < 4; j++)
		{
			BeachFlag_Define.PM.Boards[j].SetIcon();
		}
		isStopChara = true;
		restartTime = 1f;
		FadeOut(delegate
		{
			isGameStart = false;
			isGameStartEffect = false;
			gameState = GameState.GAME_START_WAIT;
			gameStateMethod = GameStartWait;
		});
		gameState = GameState.ANIMATION_WAIT;
		gameStateMethod = AnimationWait;
	}
	private void RoundStartStandby()
	{
		isStopChara = true;
		restartTime = 1f;
		FadeOut(delegate
		{
			gameState = GameState.ROUND_START_WAIT;
			gameStateMethod = RoundStartWait;
		});
		gameState = GameState.ANIMATION_WAIT;
		gameStateMethod = AnimationWait;
	}
	private void RoundStartWait()
	{
		restartTime -= Time.deltaTime;
		if (restartTime <= 0f)
		{
			gameStateMethod = RoundStart;
		}
	}
	private void RoundStart()
	{
		restartTime = 0f;
		UnityEngine.Debug.Log("ラウンド開始");
		isStopTime = false;
		isStopChara = false;
		isGoal = false;
		BeachFlag_Define.PM.Players[0].StartMethod();
		BeachFlag_Define.PM.Players[1].StartMethod();
		gameState = GameState.DURING_GAME;
		gameStateMethod = DuringGame;
	}
	public void EndRound()
	{
		UnityEngine.Debug.Log("ラウンド終了");
		PlayGameFinishSe();
		isStopChara = true;
		isStopTime = true;
		restartTime = 0f;
		if (IsGoalCharacter())
		{
			AddSet(GetGoalCharacterNo());
			gameState = GameState.ROUND_END_WAIT;
			gameStateMethod = RoundEndWait;
		}
		else
		{
			gameState = GameState.ROUND_CONTINUE_WAIT;
			gameStateMethod = RoundContinueWait;
		}
	}
	private void RoundEndWait()
	{
		restartTime += Time.deltaTime;
		if (!(restartTime >= 2f))
		{
			return;
		}
		for (int i = 0; i < BeachFlag_Define.MEMBER_NUM; i++)
		{
			if (GetSetNum(i) == gameSetNum)
			{
				gameState = GameState.GAME_END;
				gameStateMethod = EndGame;
				return;
			}
		}
		UnityEngine.Debug.Log("次のラウンドへ移る");
		BeachFlag_Define.UIM.HideSkip();
		isGameStart = false;
		isGameStartEffect = false;
		FadeIn(delegate
		{
			gameState = GameState.GAME_START_WAIT;
			gameStateMethod = GameStartWait;
		});
		gameState = GameState.ANIMATION_WAIT;
		gameStateMethod = AnimationWait;
	}
	private void RoundContinueWait()
	{
		restartTime += Time.deltaTime;
		if (restartTime >= 2f)
		{
			UnityEngine.Debug.Log("ラウンド続行");
			FadeIn(delegate
			{
				gameState = GameState.ROUND_CONTINUE_START_STANDBY;
				gameStateMethod = RoundContinueStartStandby;
			});
			gameState = GameState.ANIMATION_WAIT;
			gameStateMethod = AnimationWait;
		}
	}
	private void EndGame()
	{
		UnityEngine.Debug.Log("試合終了");
		restartTime = 0f;
		wait_done = false;
		gameState = GameState.GAME_END_WAIT;
		gameStateMethod = StateEndGameWait;
	}
	private void StateEndGameWait()
	{
		restartTime += Time.deltaTime;
		if (!(restartTime >= 0f))
		{
			return;
		}
		UnityEngine.Debug.Log("これから2秒待つ");
		StartCoroutine(WaitInterval(2f));
		if (wait_done)
		{
			UnityEngine.Debug.Log("2秒待った");
			if (gameEndCallBack != null)
			{
				gameEndCallBack();
			}
			wait_done = false;
			gameStateMethod = null;
			BeachFlag_Define.UIM.StartTournamentUIAnimation(GetSetNum(0) == gameSetNum, delegate
			{
				ResultGameDataParams.SetPoint();
				int[] array = new int[4]
				{
					0,
					1,
					2,
					3
				};
				for (int k = 0; k < array.Length; k++)
				{
					for (int l = 0; l < currentTeamData.Length; l++)
					{
						if (array[k] == currentTeamData[l].teamNo)
						{
							array[k] = currentTeamData[l].memberPlayerNoList[0];
							break;
						}
					}
				}
				int[] array2 = new int[4];
				int[] tournamentWinnerTeamNoList = BeachFlag_Define.UIM.GetTournamentWinnerTeamNoList();
				for (int m = 0; m < tournamentWinnerTeamNoList.Length; m++)
				{
					if (m < 2)
					{
						array2[tournamentWinnerTeamNoList[m]] += 2;
					}
					else
					{
						array2[tournamentWinnerTeamNoList[m]]++;
					}
				}
				if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1 && array2[0] == 3)
				{
					switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
					{
					case SystemData.AiStrength.WEAK:
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.DELIVERY_ORDER);
						break;
					case SystemData.AiStrength.NORAML:
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.DELIVERY_ORDER);
						break;
					case SystemData.AiStrength.STRONG:
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.DELIVERY_ORDER);
						break;
					}
				}
				List<string> list = new List<string>();
				string[] array3 = tournamentVSTeamData[0].Split('-');
				int no = int.Parse(array3[0]) - 1;
				int no2 = int.Parse(array3[1]) - 1;
				list.Add((GetPlayerNoAtTeamNoAffiliation(no) + 1).ToString() + "-" + (GetPlayerNoAtTeamNoAffiliation(no2) + 1).ToString());
				string[] array4 = tournamentVSTeamData[1].Split('-');
				no = int.Parse(array4[0]) - 1;
				no2 = int.Parse(array4[1]) - 1;
				list.Add((GetPlayerNoAtTeamNoAffiliation(no) + 1).ToString() + "-" + (GetPlayerNoAtTeamNoAffiliation(no2) + 1).ToString());
				int[] tournamentWinnerTeamNoList2 = BeachFlag_Define.UIM.GetTournamentWinnerTeamNoList();
				int[] array5 = new int[tournamentWinnerTeamNoList2.Length];
				for (int n = 0; n < array5.Length; n++)
				{
					array5[n] = GetPlayerNoAtTeamNoAffiliation(tournamentWinnerTeamNoList2[n]);
				}
				ResultGameDataParams.SetRecord_Int_Tournament(array2, array, array5, list.ToArray());
				rankingResult.ShowResult_Tournament();
			});
			if (BeachFlag_Define.UIM.GetNowTournamentType() != BeachFlag_TournamentUIManager.RoundType.Round_Final)
			{
				LeanTween.delayedCall(5.75f, (Action)delegate
				{
					FadeIn(delegate
					{
						CharacterMove();
						BeachFlag_Define.PM.Set_UserData();
						for (int i = 0; i < BeachFlag_Define.PM.Players.Length; i++)
						{
							BeachFlag_Define.PM.Boards[i].SetIcon();
							BeachFlag_Define.PM.Players[i].isover = false;
							BeachFlag_Define.PM.Players[i].Chara.EffectReset();
						}
						for (int j = 0; j < 4; j++)
						{
							BeachFlag_Define.PM.Boards[j].SetIcon();
						}
						if (BeachFlag_Define.PM.Players[0].UserType < BeachFlag_Define.PM.Players[1].UserType)
						{
							BeachFlag_Define.PM.Players[0].Chara.Cart.m_Path = BeachFlag_Define.PM.Carts[0];
							BeachFlag_Define.PM.Players[1].Chara.Cart.m_Path = BeachFlag_Define.PM.Carts[1];
						}
						else
						{
							BeachFlag_Define.PM.Players[0].Chara.Cart.m_Path = BeachFlag_Define.PM.Carts[1];
							BeachFlag_Define.PM.Players[1].Chara.Cart.m_Path = BeachFlag_Define.PM.Carts[0];
						}
						BeachFlag_Define.UIM.NextSettingTornament();
						ResetSet();
						FadeOut(delegate
						{
							isGameStartEffect = false;
							isGameStart = false;
							gameState = GameState.GAME_START_WAIT;
							gameStateMethod = GameStartWait;
						});
					});
				});
			}
		}
	}
	private void CharacterMove()
	{
		if (currentTournamentType == RoundType.Round_1st)
		{
			currentTournamentType = RoundType.Round_2nd;
		}
		else if (currentTournamentType == RoundType.Round_2nd)
		{
			currentTournamentType = RoundType.LoserBattle;
		}
		else if (currentTournamentType == RoundType.LoserBattle)
		{
			currentTournamentType = RoundType.Round_Final;
		}
		else if (currentTournamentType == RoundType.Round_Final)
		{
			currentTournamentType = RoundType.None;
		}
		switch (currentTournamentType)
		{
		case RoundType.Round_1st:
			currentTournamentType = RoundType.Round_2nd;
			ChangeTeamData(MovePattern.Tournament_UpDownSide);
			break;
		case RoundType.Round_2nd:
			ChangeTeamData(MovePattern.Tournament_UpDownSide);
			break;
		case RoundType.LoserBattle:
		{
			int[] tournamentLoserBattleTeamNoArray = BeachFlag_Define.UIM.GetTournamentLoserBattleTeamNoArray();
			MovePattern pattern2 = MovePattern.Addmission;
			switch (GetTeamPositionType(tournamentLoserBattleTeamNoArray[0]))
			{
			case PositionSideType.LeftSide:
				switch (GetTeamPositionType(tournamentLoserBattleTeamNoArray[1]))
				{
				case PositionSideType.BackSide:
					pattern2 = MovePattern.Tournament_FinalRound_UpSide;
					break;
				case PositionSideType.FrontSide:
					pattern2 = MovePattern.Tournament_FinalRound_DownSideOnly;
					break;
				}
				break;
			case PositionSideType.RightSide:
				switch (GetTeamPositionType(tournamentLoserBattleTeamNoArray[1]))
				{
				case PositionSideType.BackSide:
					pattern2 = MovePattern.Tournament_FinalRound_UpSideOnly;
					break;
				case PositionSideType.FrontSide:
					pattern2 = MovePattern.Tournament_FinalRound_DownSide;
					break;
				}
				break;
			case PositionSideType.BackSide:
				switch (GetTeamPositionType(tournamentLoserBattleTeamNoArray[1]))
				{
				case PositionSideType.LeftSide:
					pattern2 = MovePattern.Tournament_FinalRound_UpSide;
					break;
				case PositionSideType.RightSide:
					pattern2 = MovePattern.Tournament_FinalRound_UpSideOnly;
					break;
				}
				break;
			case PositionSideType.FrontSide:
				switch (GetTeamPositionType(tournamentLoserBattleTeamNoArray[1]))
				{
				case PositionSideType.LeftSide:
					pattern2 = MovePattern.Tournament_FinalRound_DownSideOnly;
					break;
				case PositionSideType.RightSide:
					pattern2 = MovePattern.Tournament_FinalRound_DownSide;
					break;
				}
				break;
			}
			ChangeTeamData(pattern2);
			break;
		}
		case RoundType.Round_Final:
		{
			int[] tournamentFinalRoundTeamNoArray = BeachFlag_Define.UIM.GetTournamentFinalRoundTeamNoArray();
			MovePattern pattern = MovePattern.Addmission;
			switch (GetTeamPositionType(tournamentFinalRoundTeamNoArray[0]))
			{
			case PositionSideType.LeftSide:
				switch (GetTeamPositionType(tournamentFinalRoundTeamNoArray[1]))
				{
				case PositionSideType.BackSide:
					pattern = MovePattern.Tournament_FinalRound_UpSide;
					break;
				case PositionSideType.FrontSide:
					pattern = MovePattern.Tournament_FinalRound_DownSideOnly;
					break;
				}
				break;
			case PositionSideType.RightSide:
				switch (GetTeamPositionType(tournamentFinalRoundTeamNoArray[1]))
				{
				case PositionSideType.BackSide:
					pattern = MovePattern.Tournament_FinalRound_UpSideOnly;
					break;
				case PositionSideType.FrontSide:
					pattern = MovePattern.Tournament_FinalRound_DownSide;
					break;
				}
				break;
			case PositionSideType.BackSide:
				switch (GetTeamPositionType(tournamentFinalRoundTeamNoArray[1]))
				{
				case PositionSideType.LeftSide:
					pattern = MovePattern.Tournament_FinalRound_UpSide;
					break;
				case PositionSideType.RightSide:
					pattern = MovePattern.Tournament_FinalRound_UpSideOnly;
					break;
				case PositionSideType.FrontSide:
					pattern = MovePattern.Tournament_UpDownSide;
					break;
				}
				break;
			case PositionSideType.FrontSide:
				switch (GetTeamPositionType(tournamentFinalRoundTeamNoArray[1]))
				{
				case PositionSideType.LeftSide:
					pattern = MovePattern.Tournament_FinalRound_DownSideOnly;
					break;
				case PositionSideType.RightSide:
					pattern = MovePattern.Tournament_FinalRound_DownSide;
					break;
				case PositionSideType.BackSide:
					pattern = MovePattern.Tournament_UpDownSide;
					break;
				}
				break;
			}
			ChangeTeamData(pattern);
			break;
		}
		}
	}
	public void ChangeTeamData(MovePattern _pattern)
	{
		switch (_pattern)
		{
		case MovePattern.League_2ndRound:
		{
			TeamData teamData = GetTeamData(PositionSideType.LeftSide);
			TeamData teamData2 = GetTeamData(PositionSideType.RightSide);
			SetTeamData(PositionSideType.LeftSide, GetTeamData(PositionSideType.BackSide));
			SetTeamData(PositionSideType.RightSide, teamData);
			SetTeamData(PositionSideType.FrontSide, teamData2);
			SetTeamData(PositionSideType.BackSide, default(TeamData));
			break;
		}
		case MovePattern.League_3rdRound:
		{
			TeamData teamData = GetTeamData(PositionSideType.RightSide);
			SetTeamData(PositionSideType.RightSide, GetTeamData(PositionSideType.FrontSide));
			SetTeamData(PositionSideType.FrontSide, teamData);
			break;
		}
		case MovePattern.Tournament_UpDownSide:
		{
			TeamData teamData = GetTeamData(PositionSideType.RightSide);
			TeamData teamData2 = GetTeamData(PositionSideType.LeftSide);
			SetTeamData(PositionSideType.RightSide, GetTeamData(PositionSideType.FrontSide));
			SetTeamData(PositionSideType.FrontSide, teamData);
			SetTeamData(PositionSideType.LeftSide, GetTeamData(PositionSideType.BackSide));
			SetTeamData(PositionSideType.BackSide, teamData2);
			break;
		}
		case MovePattern.Tournament_FinalRound_UpSideOnly:
		{
			TeamData teamData = GetTeamData(PositionSideType.LeftSide);
			SetTeamData(PositionSideType.LeftSide, GetTeamData(PositionSideType.BackSide));
			SetTeamData(PositionSideType.BackSide, teamData);
			break;
		}
		case MovePattern.Tournament_FinalRound_DownSideOnly:
		{
			TeamData teamData = GetTeamData(PositionSideType.RightSide);
			SetTeamData(PositionSideType.RightSide, GetTeamData(PositionSideType.FrontSide));
			SetTeamData(PositionSideType.FrontSide, teamData);
			break;
		}
		case MovePattern.Tournament_FinalRound_UpSide:
		{
			TeamData teamData = GetTeamData(PositionSideType.RightSide);
			TeamData teamData2 = GetTeamData(PositionSideType.FrontSide);
			TeamData teamData3 = GetTeamData(PositionSideType.BackSide);
			SetTeamData(PositionSideType.RightSide, GetTeamData(PositionSideType.LeftSide));
			SetTeamData(PositionSideType.FrontSide, teamData);
			SetTeamData(PositionSideType.BackSide, teamData2);
			SetTeamData(PositionSideType.LeftSide, teamData3);
			break;
		}
		case MovePattern.Tournament_FinalRound_DownSide:
		{
			TeamData teamData = GetTeamData(PositionSideType.BackSide);
			TeamData teamData2 = GetTeamData(PositionSideType.FrontSide);
			TeamData teamData3 = GetTeamData(PositionSideType.RightSide);
			SetTeamData(PositionSideType.BackSide, GetTeamData(PositionSideType.LeftSide));
			SetTeamData(PositionSideType.FrontSide, teamData);
			SetTeamData(PositionSideType.RightSide, teamData2);
			SetTeamData(PositionSideType.LeftSide, teamData3);
			break;
		}
		}
	}
	public PositionSideType GetTeamPositionType(int _teamNo)
	{
		if (currentTeamData[0].teamNo == _teamNo)
		{
			return PositionSideType.LeftSide;
		}
		if (currentTeamData[1].teamNo == _teamNo)
		{
			return PositionSideType.RightSide;
		}
		if (currentTeamData[2].teamNo == _teamNo)
		{
			return PositionSideType.BackSide;
		}
		if (currentTeamData[3].teamNo == _teamNo)
		{
			return PositionSideType.FrontSide;
		}
		UnityEngine.Debug.LogError("<color=yellow>団番号を含まない番号です！</color>" + _teamNo.ToString());
		return PositionSideType.LeftSide;
	}
	public void SetTeamData(PositionSideType _positionSide, TeamData _teamData)
	{
		currentTeamData[(int)_positionSide].teamNo = _teamData.teamNo;
		currentTeamData[(int)_positionSide].memberPlayerNoList = _teamData.memberPlayerNoList;
	}
	public int GetPlayerNoAtTeamNoAffiliation(int _no)
	{
		for (int i = 0; i < currentTeamData.Length; i++)
		{
			if (currentTeamData[i].teamNo == _no)
			{
				return SingletonCustom<GameSettingManager>.Instance.GetPlayerAffiliationTeam(currentTeamData[i].memberPlayerNoList[0]);
			}
		}
		return 0;
	}
	public void ResetSet()
	{
		for (int i = 0; i < playerGetSetNum.Length; i++)
		{
			playerGetSetNum[i] = 0;
		}
	}
	public void AddSet(int _playerNo)
	{
		if (_playerNo < 0)
		{
			UnityEngine.Debug.Log("同時に死んだので引き分け！");
		}
		else
		{
			playerGetSetNum[_playerNo]++;
		}
	}
	public int GetSetNum(int _playerNo)
	{
		return playerGetSetNum[_playerNo];
	}
	private void DuringGame()
	{
	}
	private void AnimationWait()
	{
	}
	private bool CharacterGoalCheck()
	{
		if (BeachFlag_Define.PM.Players[0].Chara.processType == BeachFlag_Chara.ProcessType.GOAL || BeachFlag_Define.PM.Players[1].Chara.processType == BeachFlag_Chara.ProcessType.GOAL)
		{
			return true;
		}
		return false;
	}
	public bool CharacterWinnerCheck()
	{
		if (BeachFlag_Define.PM.Players[0].Chara.processType == BeachFlag_Chara.ProcessType.GOAL && BeachFlag_Define.PM.Players[1].Chara.processType == BeachFlag_Chara.ProcessType.GOAL)
		{
			return false;
		}
		if (BeachFlag_Define.PM.Players[0].Chara.processType == BeachFlag_Chara.ProcessType.GOAL && BeachFlag_Define.PM.Players[1].Chara.processType != BeachFlag_Chara.ProcessType.START)
		{
			return true;
		}
		if (BeachFlag_Define.PM.Players[0].Chara.processType != BeachFlag_Chara.ProcessType.START && BeachFlag_Define.PM.Players[1].Chara.processType == BeachFlag_Chara.ProcessType.GOAL)
		{
			return true;
		}
		return false;
	}
	public bool IsGoalCharacter()
	{
		if (BeachFlag_Define.PM.Players[0].Chara.processType == BeachFlag_Chara.ProcessType.GOAL && BeachFlag_Define.PM.Players[1].Chara.processType == BeachFlag_Chara.ProcessType.START)
		{
			return true;
		}
		if (BeachFlag_Define.PM.Players[0].Chara.processType == BeachFlag_Chara.ProcessType.START && BeachFlag_Define.PM.Players[1].Chara.processType == BeachFlag_Chara.ProcessType.GOAL)
		{
			return true;
		}
		return false;
	}
	public int GetGoalCharacterNo()
	{
		for (int i = 0; i < BeachFlag_Define.PM.Players.Length; i++)
		{
			if (BeachFlag_Define.PM.Players[i].Chara.processType == BeachFlag_Chara.ProcessType.GOAL)
			{
				return i;
			}
		}
		return -1;
	}
	public void PlayGameFinishSe()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_buzzer");
	}
	private void InitialSettingTeamData(PositionSideType _positionSide)
	{
		switch (_positionSide)
		{
		case PositionSideType.LeftSide:
			currentTeamData[(int)_positionSide].teamNo = teamNoList[0];
			currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
			{
				randomTeamNoList[teamNoList[0]]
			};
			break;
		case PositionSideType.RightSide:
			currentTeamData[(int)_positionSide].teamNo = teamNoList[1];
			currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
			{
				randomTeamNoList[teamNoList[1]]
			};
			break;
		case PositionSideType.BackSide:
			currentTeamData[(int)_positionSide].teamNo = teamNoList[2];
			currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
			{
				randomTeamNoList[teamNoList[2]]
			};
			break;
		case PositionSideType.FrontSide:
			currentTeamData[(int)_positionSide].teamNo = teamNoList[3];
			currentTeamData[(int)_positionSide].memberPlayerNoList = new List<int>
			{
				randomTeamNoList[teamNoList[3]]
			};
			break;
		}
	}
	public bool CheckPlayGame()
	{
		return !CheckGameState(GameState.GAME_END);
	}
	public bool IsDuringGame()
	{
		return gameState <= GameState.GAME_END;
	}
	public bool CheckGameState(GameState _gameState)
	{
		return gameState == _gameState;
	}
	public GameState GetGameState()
	{
		return gameState;
	}
	public bool IsAutoMove()
	{
		return isAutoMove;
	}
	public bool IsStopChara()
	{
		return isStopChara;
	}
	private void FadeIn(Action _callback = null)
	{
		fadeSprite.gameObject.SetActive(value: true);
		LeanTween.value(fadeSprite.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float alpha)
		{
			fadeSprite.color = new Color(0f, 0f, 0f, alpha);
		}).setOnComplete((Action)delegate
		{
			if (_callback != null)
			{
				_callback();
			}
		});
	}
	private void FadeOut(Action _callback = null)
	{
		UnityEngine.Debug.Log("画面をフェ\u30fcドアウトさせる ======");
		LeanTween.value(fadeSprite.gameObject, 1f, 0f, 1f).setOnUpdate(delegate(float alpha)
		{
			fadeSprite.SetAlpha(alpha);
		}).setOnComplete((Action)delegate
		{
			fadeSprite.gameObject.SetActive(value: false);
			if (_callback != null)
			{
				_callback();
			}
		});
	}
	public TeamData GetTeamData(PositionSideType _type)
	{
		return currentTeamData[(int)_type];
	}
	public TeamData GetTeamData(int _teamNo)
	{
		for (int i = 0; i < currentTeamData.Length; i++)
		{
			if (currentTeamData[i].teamNo == _teamNo)
			{
				return currentTeamData[i];
			}
		}
		return currentTeamData[0];
	}
	private IEnumerator WaitCountDown()
	{
		isGameStart = true;
		if (BeachFlag_Define.CaM.GetState() != BeachFlag_CameraManager.CameraState.DASH)
		{
			BeachFlag_Define.CaM.SetState(BeachFlag_CameraManager.CameraState.DASH);
		}
		yield return new WaitForSeconds(5.4f);
		gameStateMethod = RoundStart;
		isGameStartEffect = false;
	}
	private IEnumerator WaitInterval(float interval)
	{
		yield return new WaitForSeconds(interval);
		wait_done = true;
	}
	public string[] GetTournamentVSTeamData()
	{
		return tournamentVSTeamData;
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(base.gameObject);
	}
}
