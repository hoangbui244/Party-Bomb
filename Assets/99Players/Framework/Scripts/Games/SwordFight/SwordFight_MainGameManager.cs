using GamepadInput;
using SaveDataDefine;
using System;
using System.Collections.Generic;
using UnityEngine;
public class SwordFight_MainGameManager : SingletonCustom<SwordFight_MainGameManager>
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
		public int winCount;
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
	[Header("舞台判定")]
	private GameObject[] arrayHitObj;
	[SerializeField]
	[Header("ワ\u30fcルドル\u30fcト")]
	private GameObject root3Dworld;
	[SerializeField]
	[Header("操作説明：1人")]
	private GameObject objOperationSingle;
	[SerializeField]
	[Header("操作説明：複数人")]
	private GameObject objOperationMulti;
	private TeamData[] currentTeamData = new TeamData[4];
	private GameState gameState;
	private bool isGameStart;
	private GameStateMethod gameStateMethod;
	private float settingGameTime;
	private float gameTimeCorr;
	private float restartTime;
	private bool isAutoMove;
	private bool isStopChara = true;
	private Action gameEndCallBack;
	[SerializeField]
	[Header("UIエフェクトアンカ\u30fc")]
	private Transform effectAnchor;
	[SerializeField]
	[Header("クラッカ\u30fcエフェクト")]
	private ParticleSystem[] crackerEffect;
	[SerializeField]
	[Header("試合開始エフェクトプレハブ")]
	private GameObject prefabStartGame;
	[SerializeField]
	[Header("試合終了プレハブ")]
	private GameObject prefabEndGame;
	[SerializeField]
	[Header("～人目エフェクトプレハブ")]
	private SwordFight_CountHumanEffect prefabCountHuman;
	[SerializeField]
	[Header("フェ\u30fcド画像")]
	private SpriteRenderer fadeSprite;
	[SerializeField]
	[Header("一人用リザルト画面")]
	private SwordFight_SingleResultManager singleResultManager;
	[SerializeField]
	[Header("複数人用リザルト画面")]
	private SwordFight_MultiResultManager multiResultManager;
	[SerializeField]
	[Header("消しゴムくん")]
	private GameObject objKeshigomuKun;
	[SerializeField]
	[Header("いちごちゃん")]
	private GameObject objIchigoChan;
	[SerializeField]
	[Header("モ\u30fcション確認デバッグ(ひとりで時のみ機能)")]
	private bool isAnimationDebugMode;
	private int gameSetNum = 1;
	private int playerNum = 1;
	private bool singlePlayerLoseFlg;
	private bool isTeamMode;
	private int winnerPlayerNo = -1;
	private int winnerTeamNo = -1;
	private bool isFadeAnimation;
	private bool isGameStartEffect;
	private string[] tournamentVSTeamData = new string[2];
	private List<int> randomTeamNoList = new List<int>();
	private RoundType currentTournamentType;
	private int currentRoundCnt;
	private List<int> teamNoList;
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
	public bool IsAnimationDebugMode => isAnimationDebugMode;
	public int GameSetNum => gameSetNum;
	private void Awake()
	{
		objKeshigomuKun.SetActive(UnityEngine.Random.Range(0, 10) == 0);
		objIchigoChan.SetActive(UnityEngine.Random.Range(0, 10) == 0);
	}
	public void Init(Action _gameEndCallBack)
	{
		isGameStart = false;
		UnityEngine.Debug.Log("★コ\u30fcル：Init");
		gameEndCallBack = _gameEndCallBack;
		settingGameTime = 3f;
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		isTeamMode = SwordFight_Define.IS_TEAM_MODE;
		gameSetNum = 1;
		if (!isFadeAnimation)
		{
			fadeSprite.color = new Color(0f, 0f, 0f, 0f);
		}
		if (teamNoList == null)
		{
			teamNoList = new List<int>();
		}
		teamNoList.Clear();
		for (int i = 0; i < 4; i++)
		{
			teamNoList.Add(i);
		}
		SingletonCustom<GameSettingManager>.Instance.TeamNum = 4;
		SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
		for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.TeamNum; j++)
		{
			randomTeamNoList.Add(SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[j][0]);
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
		SingletonCustom<SwordFight_CharacterUIManager>.Instance.SetTournamentTeamData();
		SingletonCustom<SwordFight_GameUiManager>.Instance.SettingGameTime();
		SingletonCustom<SwordFight_CameraMover>.Instance.Init();
		SingletonCustom<SwordFight_CameraMover>.Instance.UpdateTargetRect();
		SingletonCustom<SwordFight_CameraMover>.Instance.FixCameraPos();
		effectAnchor.SetLocalPositionY(77f);
		GameStartStandby();
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			objOperationSingle.SetActive(value: true);
			objOperationMulti.SetActive(value: false);
		}
		else
		{
			objOperationSingle.SetActive(value: false);
			objOperationMulti.SetActive(value: true);
		}
	}
	public void UpdateMethod()
	{
		if (isGameStart && !SingletonCustom<SwordFight_GameUiManager>.Instance.IsStopTime && CharacterDeathCheck_BattleMode())
		{
			for (int i = 0; i < arrayHitObj.Length; i++)
			{
				arrayHitObj[i].SetActive(value: false);
			}
			EndRound();
		}
		if (gameStateMethod == null)
		{
			return;
		}
		gameStateMethod();
		if ((SingletonCustom<SwordFight_GameUiManager>.Instance.IsSkipShow && SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X)) || (Application.isEditor && UnityEngine.Input.GetKeyDown(KeyCode.R)))
		{
			int num = (!CalcManager.GetHalfProbability()) ? 1 : 0;
			SingletonCustom<SwordFight_GameUiManager>.Instance.AddSet(num, -1);
			for (int j = 0; j < SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList.Length; j++)
			{
				SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[j].StopRunEffect();
			}
			SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[num].CharacterFaceChange_Happy();
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
			SingletonCustom<SwordFight_AudienceManager>.Instance.ChangePeopleAnim(SwordFight_AudienceManager.AudienceType.Floor, SwordFight_AudienceManager.AudienceAnimType.EXCITE);
			gameState = GameState.ROUND_END_WAIT;
			gameStateMethod = RoundEndWait;
		}
	}
	private void GameStartStandby()
	{
		SingletonCustom<SwordFight_MainCharacterManager>.Instance.SettingGameStart();
		SingletonCustom<SwordFight_GameUiManager>.Instance.SettingGameTime();
		SingletonCustom<SwordFight_MainCharacterManager>.Instance.ShowCursor(_show: true);
		isStopChara = true;
		restartTime = 1f;
		gameState = GameState.GAME_START_WAIT;
		gameStateMethod = GameStartWait;
	}
	private void GameStartWait()
	{
		restartTime -= Time.deltaTime;
		if (!(restartTime <= 0f))
		{
			return;
		}
		if (!isGameStartEffect)
		{
			for (int i = 0; i < arrayHitObj.Length; i++)
			{
				arrayHitObj[i].SetActive(value: true);
			}
			SingletonCustom<CommonStartSimple>.Instance.Show(delegate
			{
				if (SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[0].IsCpu && SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[1].IsCpu)
				{
					SingletonCustom<SwordFight_GameUiManager>.Instance.ShowSkip();
				}
				else
				{
					SingletonCustom<SwordFight_GameUiManager>.Instance.HideSkip();
				}
			});
			isGameStartEffect = true;
			restartTime = 2f;
		}
		else
		{
			isGameStart = true;
			gameStateMethod = RoundStart;
			isGameStartEffect = false;
		}
	}
	private void RoundContinueStartStandby()
	{
		SingletonCustom<SwordFight_MainCharacterManager>.Instance.SettingContinueRoundStart();
		SingletonCustom<SwordFight_GameUiManager>.Instance.SettingGameTime();
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
	private void RoundStartStandby()
	{
		SingletonCustom<SwordFight_MainCharacterManager>.Instance.SettingRoundStart();
		SingletonCustom<SwordFight_GameUiManager>.Instance.SettingGameTime();
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
		SingletonCustom<SwordFight_GameUiManager>.Instance.IsStopTime = false;
		isStopChara = false;
		gameState = GameState.DURING_GAME;
		gameStateMethod = DuringGame;
	}
	public void EndRound()
	{
		UnityEngine.Debug.Log("ラウンド終了");
		PlayGameFinishSe();
		isStopChara = true;
		SingletonCustom<SwordFight_GameUiManager>.Instance.IsStopTime = true;
		restartTime = 0f;
		if (isTeamMode)
		{
			if (GetTeamSurvivor_CoopMode(0) == 0)
			{
				SingletonCustom<SwordFight_GameUiManager>.Instance.AddSet(-1, 1);
				winnerTeamNo = 1;
				SwordFight_CharacterScript[] survivorCharacter = GetSurvivorCharacter();
				for (int i = 0; i < survivorCharacter.Length; i++)
				{
					survivorCharacter[i].CharacterFaceChange_Happy();
				}
				SingletonCustom<SwordFight_AudienceManager>.Instance.ChangePeopleAnim(SwordFight_AudienceManager.AudienceType.Floor, SwordFight_AudienceManager.AudienceAnimType.EXCITE);
				gameState = GameState.ROUND_END_WAIT;
				gameStateMethod = RoundEndWait;
			}
			else if (GetTeamSurvivor_CoopMode(1) == 0)
			{
				SingletonCustom<SwordFight_GameUiManager>.Instance.AddSet(-1, 0);
				winnerTeamNo = 0;
				SwordFight_CharacterScript[] survivorCharacter = GetSurvivorCharacter();
				for (int i = 0; i < survivorCharacter.Length; i++)
				{
					survivorCharacter[i].CharacterFaceChange_Happy();
				}
				SingletonCustom<SwordFight_AudienceManager>.Instance.ChangePeopleAnim(SwordFight_AudienceManager.AudienceType.Floor, SwordFight_AudienceManager.AudienceAnimType.EXCITE);
				gameState = GameState.ROUND_END_WAIT;
				gameStateMethod = RoundEndWait;
			}
			else
			{
				gameState = GameState.ROUND_CONTINUE_WAIT;
				gameStateMethod = RoundContinueWait;
			}
			return;
		}
		UnityEngine.Debug.Log("キャラが最後の一人？:" + CharacterDeathCheck_BattleMode().ToString());
		if (CharacterDeathCheck_BattleMode())
		{
			SingletonCustom<SwordFight_GameUiManager>.Instance.AddSet(GetSurvivorCharacterNo_BattleMode(), -1);
			SwordFight_CharacterScript[] survivorCharacter = GetSurvivorCharacter();
			for (int i = 0; i < survivorCharacter.Length; i++)
			{
				survivorCharacter[i].CharacterFaceChange_Happy();
			}
			SingletonCustom<SwordFight_AudienceManager>.Instance.ChangePeopleAnim(SwordFight_AudienceManager.AudienceType.Floor, SwordFight_AudienceManager.AudienceAnimType.EXCITE);
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
		for (int i = 0; i < SwordFight_Define.MAX_GAME_PLAYER_NUM; i++)
		{
			if (SingletonCustom<SwordFight_GameUiManager>.Instance.GetSetNum(i, -1) == gameSetNum)
			{
				gameState = GameState.GAME_END;
				gameStateMethod = EndGame;
				if (!SingletonCustom<SwordFight_MainCharacterManager>.Instance.IsPlayer(i))
				{
					singlePlayerLoseFlg = true;
				}
				return;
			}
		}
		UnityEngine.Debug.Log("次のラウンドへ移る");
		FadeIn(delegate
		{
			gameState = GameState.ROUND_START_STANDBY;
			gameStateMethod = RoundStartStandby;
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
		if (gameEndCallBack != null)
		{
			gameEndCallBack();
		}
		gameStateMethod = null;
		if (!SwordFight_Define.IS_TEAM_MODE)
		{
			currentRoundCnt++;
			SingletonCustom<SwordFight_CharacterUIManager>.Instance.StartTournamentUIAnimation(SingletonCustom<SwordFight_GameUiManager>.Instance.GetSetNum(0, -1) == gameSetNum, delegate
			{
				ResultGameDataParams.SetPoint();
				int[] array = new int[4]
				{
					0,
					1,
					2,
					3
				};
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < currentTeamData.Length; j++)
					{
						if (array[i] == currentTeamData[j].teamNo)
						{
							array[i] = currentTeamData[j].memberPlayerNoList[0];
							break;
						}
					}
				}
				int[] array2 = new int[4];
				int[] tournamentWinnerTeamNoList = SingletonCustom<SwordFight_CharacterUIManager>.Instance.GetTournamentWinnerTeamNoList();
				for (int k = 0; k < tournamentWinnerTeamNoList.Length; k++)
				{
					if (k < 2)
					{
						array2[tournamentWinnerTeamNoList[k]] += 2;
					}
					else
					{
						array2[tournamentWinnerTeamNoList[k]]++;
					}
				}
				if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1 && array2[0] == 3)
				{
					switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
					{
					case SystemData.AiStrength.WEAK:
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BOMB_ROULETTE);
						break;
					case SystemData.AiStrength.NORAML:
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BOMB_ROULETTE);
						break;
					case SystemData.AiStrength.STRONG:
						SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BOMB_ROULETTE);
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
				int[] tournamentWinnerTeamNoList2 = SingletonCustom<SwordFight_CharacterUIManager>.Instance.GetTournamentWinnerTeamNoList();
				int[] array5 = new int[tournamentWinnerTeamNoList2.Length];
				for (int l = 0; l < array5.Length; l++)
				{
					array5[l] = GetPlayerNoAtTeamNoAffiliation(tournamentWinnerTeamNoList2[l]);
				}
				ResultGameDataParams.SetRecord_Int_Tournament(array2, array, array5, list.ToArray());
				rankingResult.ShowResult_Tournament();
				LeanTween.delayedCall(base.gameObject, 0.75f, (Action)delegate
				{
					root3Dworld.SetActive(value: false);
				});
			});
			if (SingletonCustom<SwordFight_CharacterUIManager>.Instance.GetNowTournamentType() != MS_TournamentUIManager.RoundType.Round_Final)
			{
				LeanTween.delayedCall(5.75f, (Action)delegate
				{
					FadeIn(delegate
					{
						CharacterMove();
						SingletonCustom<SwordFight_MainCharacterManager>.Instance.CreatePlayer(_isInstantiate: false);
						SingletonCustom<SwordFight_CharacterUIManager>.Instance.NextSettingTornament();
						SingletonCustom<SwordFight_GameUiManager>.Instance.ResetSet();
						SingletonCustom<SwordFight_CharacterUIManager>.Instance.Reset();
						SingletonCustom<SwordFight_CameraMover>.Instance.UpdateTargetRect();
						SingletonCustom<SwordFight_CameraMover>.Instance.FixCameraPos();
						FadeOut(delegate
						{
							GameStartStandby();
						});
					});
				});
			}
		}
	}
	private void CharacterMove()
	{
		if (SingletonCustom<GameSettingManager>.Instance.TeamNum <= 2)
		{
			return;
		}
		if (SingletonCustom<GameSettingManager>.Instance.TeamNum == 3)
		{
			if (currentRoundCnt == 1)
			{
				ChangeTeamData(MovePattern.League_2ndRound);
			}
			else if (currentRoundCnt == 2)
			{
				ChangeTeamData(MovePattern.League_3rdRound);
			}
		}
		else
		{
			if (SingletonCustom<GameSettingManager>.Instance.TeamNum != 4)
			{
				return;
			}
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
				int[] tournamentLoserBattleTeamNoArray = SingletonCustom<SwordFight_CharacterUIManager>.Instance.GetTournamentLoserBattleTeamNoArray();
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
				int[] tournamentFinalRoundTeamNoArray = SingletonCustom<SwordFight_CharacterUIManager>.Instance.GetTournamentFinalRoundTeamNoArray();
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
		currentTeamData[(int)_positionSide].winCount = _teamData.winCount;
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
	private void DuringGame()
	{
	}
	private void AnimationWait()
	{
	}
	private bool CharacterDeathCheck_BattleMode()
	{
		int num = SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList.Length + SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList.Length;
		for (int i = 0; i < SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList.Length; i++)
		{
			if (SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[i].CheckActionState(SwordFight_CharacterScript.ActionState.DEATH))
			{
				num--;
			}
		}
		for (int j = 0; j < SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList.Length; j++)
		{
			if (SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList[j].CheckActionState(SwordFight_CharacterScript.ActionState.DEATH))
			{
				num--;
			}
		}
		if (num <= 1)
		{
			for (int k = 0; k < SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList.Length; k++)
			{
				if (!SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[k].CheckActionState(SwordFight_CharacterScript.ActionState.DEATH))
				{
					winnerPlayerNo = k;
				}
			}
			return true;
		}
		return false;
	}
	private int GetSurvivorCharacterNo_BattleMode()
	{
		for (int i = 0; i < SwordFight_Define.CHAMBARA_CHARACTER_MAX; i++)
		{
			if (SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[i].GetActionState() != SwordFight_CharacterScript.ActionState.DEATH)
			{
				return i;
			}
		}
		return -1;
	}
	private int GetTeamSurvivor_CoopMode(int _teamNo)
	{
		int num = SwordFight_Define.MAX_GAME_TEAM_PLAYER_NUM;
		for (int i = 0; i < SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList.Length; i++)
		{
			if (SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[i].CheckActionState(SwordFight_CharacterScript.ActionState.DEATH) && SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[i].TeamNo == _teamNo)
			{
				num--;
			}
		}
		for (int j = 0; j < SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList.Length; j++)
		{
			if (SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList[j].CheckActionState(SwordFight_CharacterScript.ActionState.DEATH) && SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList[j].TeamNo == _teamNo)
			{
				num--;
			}
		}
		return num;
	}
	private SwordFight_CharacterScript[] GetSurvivorCharacter()
	{
		List<SwordFight_CharacterScript> list = new List<SwordFight_CharacterScript>();
		for (int i = 0; i < SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList.Length; i++)
		{
			if (!SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[i].CheckActionState(SwordFight_CharacterScript.ActionState.DEATH))
			{
				list.Add(SingletonCustom<SwordFight_MainCharacterManager>.Instance.PlayerControlCharaList[i]);
			}
		}
		for (int j = 0; j < SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList.Length; j++)
		{
			if (!SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList[j].CheckActionState(SwordFight_CharacterScript.ActionState.DEATH))
			{
				list.Add(SingletonCustom<SwordFight_MainCharacterManager>.Instance.CPUCharaList[j]);
			}
		}
		return list.ToArray();
	}
	public void PlayGameFinishSe()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_buzzer");
	}
	private void InitialSettingTeamData(PositionSideType _positionSide)
	{
		UnityEngine.Debug.Log("positionSide:" + _positionSide.ToString());
		UnityEngine.Debug.Log("teamNo:" + teamNoList.Count.ToString());
		UnityEngine.Debug.Log("random:" + randomTeamNoList.Count.ToString());
		UnityEngine.Debug.Log("currentTemaData" + currentTeamData.Length.ToString());
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
		LeanTween.value(fadeSprite.gameObject, 1f, 0f, 0.5f).setOnUpdate(delegate(float alpha)
		{
			fadeSprite.SetAlpha(alpha);
		}).setOnComplete((Action)delegate
		{
			if (_callback != null)
			{
				_callback();
			}
		});
	}
	private void GameReset()
	{
		SingletonCustom<SwordFight_MainCharacterManager>.Instance.SettingRoundStart();
		SingletonCustom<SwordFight_GameUiManager>.Instance.Init();
		SingletonCustom<SwordFight_AudienceManager>.Instance.ChangePeopleAnim(SwordFight_AudienceManager.AudienceType.Floor, SwordFight_AudienceManager.AudienceAnimType.NORMAL);
		Init(delegate
		{
		});
		FadeOut(delegate
		{
			isFadeAnimation = false;
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
