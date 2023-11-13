using System.Collections;
using UnityEngine;
public class RoadRaceGameManager : SingletonCustom<RoadRaceGameManager>
{
	public enum RaceState
	{
		Standby,
		Start,
		Game,
		Goal,
		Result
	}
	private bool isGameStart;
	private bool isGameEnd;
	private bool isOneOnOne;
	private int RACE_RAP = 3;
	private int lapNum = 1;
	private bool timeAttack;
	private bool isLapRace;
	private RoadRaceDefine.BicycleRaceClass speedClass;
	private RaceState state;
	private bool isStartGame;
	private float gameTime;
	private bool isOnceGoalVoice;
	private int playerGoalCnt;
	private int goalCnt;
	private bool isSkipFlg;
	private float isSkipWaitTime;
	private float SKIP_WAIT_TIME = 5f;
	private bool isShowSkipControl;
	private bool isAutoGameEnd;
	private float isAutoGameEndTime;
	private readonly float AUTO_GAME_END_TIME = 20f;
	[SerializeField]
	[Header("順位：リザルト")]
	private RankingResultManager rankingResult;
	public bool IsGameStart => isGameStart;
	public bool IsGameEnd => isGameEnd;
	public bool IsOneOnOne => isOneOnOne;
	public int LapNum => lapNum;
	public bool TimeAttack => timeAttack;
	public bool IsLapRace => isLapRace;
	public RoadRaceDefine.BicycleRaceClass SpeedClass => speedClass;
	public int GoalCnt => goalCnt;
	public RaceState GetRaceState()
	{
		return state;
	}
	public void SetRaceState(RaceState _state)
	{
		state = _state;
	}
	public bool CheckRaceState(RaceState _state)
	{
		return state == _state;
	}
	public void Init(bool _isTutorial = false)
	{
		Scene_RoadRace.FM.Init();
		timeAttack = false;
		isLapRace = false;
		isOneOnOne = false;
		speedClass = RoadRaceDefine.BicycleRaceClass.NORMAL;
		SingletonCustom<RoadRaceCharacterManager>.Instance.Init();
		SingletonCustom<RoadRaceCameraManager>.Instance.Init(SingletonCustom<RoadRaceCharacterManager>.Instance.GetCharas());
		SingletonCustom<RoadRaceControllerManager>.Instance.Init();
		SingletonCustom<RoadRaceUiManager>.Instance.Init(Scene_RoadRace.CM.PlayerNum);
	}
	public void GameStart()
	{
		UnityEngine.Debug.Log("GameStart");
		isGameStart = true;
		state = RaceState.Game;
		Scene_RoadRace.CM.RaceStart();
		Scene_RoadRace.FM.StandStartStopper(_stand: false);
		StartCoroutine(_RankUiStartDirection());
	}
	private IEnumerator _RankUiStartDirection()
	{
		Scene_RoadRace.UM.ShowRankSprite(_isFade: true);
		yield return new WaitForSeconds(1f);
		Scene_RoadRace.UM.IsCanChangeRank = true;
	}
	public void UpdateMethod()
	{
		SingletonCustom<RoadRaceControllerManager>.Instance.UpdateMethod();
		SingletonCustom<RoadRaceCharacterManager>.Instance.UpdateMethod();
		SingletonCustom<RoadRaceUiManager>.Instance.UpdateMethod();
		gameTime += Time.deltaTime;
		gameTime = Mathf.Clamp(gameTime, 0f, 599.99f);
		for (int i = 0; i < Scene_RoadRace.CM.MemberNum; i++)
		{
			if (!Scene_RoadRace.CM.GetChara(i).IsGoal)
			{
				Scene_RoadRace.UM.SetTime(i, gameTime);
			}
		}
		CheckSkip();
		CheckAutoGameEnd();
	}
	private void CheckSkip()
	{
		if (isSkipFlg)
		{
			isSkipWaitTime += Time.deltaTime;
			if (isSkipWaitTime > SKIP_WAIT_TIME)
			{
				CPUAutoGoal();
				GameEnd(_isAutoGameEnd: true);
			}
		}
	}
	private void CheckAutoGameEnd()
	{
		if (isAutoGameEnd)
		{
			isAutoGameEndTime += Time.deltaTime;
			if (isAutoGameEndTime >= AUTO_GAME_END_TIME)
			{
				SettingAutoGoalTime();
				GameEnd(_isAutoGameEnd: true);
			}
		}
	}
	public void LateUpdateMethod()
	{
	}
	public void FixedUpdateMethod()
	{
		SingletonCustom<RoadRaceCameraManager>.Instance.FixedUpdateMethod();
	}
	public void SetGoal(int _playerNo, int _userType)
	{
		goalCnt++;
		if (_userType < 4)
		{
			playerGoalCnt++;
			SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
		}
		if (!isOnceGoalVoice && _userType < 4)
		{
			isOnceGoalVoice = true;
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal");
		}
		Scene_RoadRace.UM.ShowGoalRank(_playerNo, GetRank(_playerNo));
		if (!IsGameEnd)
		{
			if (goalCnt == Scene_RoadRace.CM.MemberNum)
			{
				GameEnd();
			}
			else if (playerGoalCnt == SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
				isSkipFlg = true;
			}
			else if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && playerGoalCnt == SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1 && goalCnt == Scene_RoadRace.CM.MemberNum - 1)
			{
				isAutoGameEnd = true;
			}
		}
	}
	public int GetRank(int _playerNo)
	{
		int num = 0;
		for (int i = 0; i < Scene_RoadRace.CM.MemberNum; i++)
		{
			int charaNo = Scene_RoadRace.CM.GetChara(i).CharaNo;
			if (_playerNo != charaNo && Scene_RoadRace.CM.GetChara(i).IsGoal)
			{
				float goalTime = Scene_RoadRace.CM.GetChara(charaNo).GetGoalTime();
				if (Scene_RoadRace.CM.GetChara(_playerNo).GetGoalTime() > goalTime)
				{
					num++;
				}
			}
		}
		return num;
	}
	public void GameEnd(bool _isAutoGameEnd = false)
	{
		if (!isGameEnd)
		{
			UnityEngine.Debug.Log("GameEnd : " + _isAutoGameEnd.ToString());
			isGameEnd = true;
			StartCoroutine(_GameEnd(_isAutoGameEnd));
		}
	}
	private IEnumerator _GameEnd(bool _isAutoGameEnd)
	{
		if (_isAutoGameEnd)
		{
			yield return new WaitForSeconds(1f);
		}
		else
		{
			yield return new WaitForSeconds(3f);
		}
		UnityEngine.Debug.Log("_isAutoGameEnd = " + _isAutoGameEnd.ToString());
		SingletonCustom<CommonEndSimple>.Instance.Show(delegate
		{
			ShowResult();
		});
	}
	public void ShowResult()
	{
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			float goalTime = Scene_RoadRace.CM.GetChara(0).GetGoalTime();
			if (goalTime > -1f)
			{
				if (goalTime <= RoadRaceDefine.BRONZE_TIME)
				{
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.ARCHER_BATTLE);
				}
				if (goalTime <= RoadRaceDefine.SILVER_TIME)
				{
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.ARCHER_BATTLE);
				}
				if (goalTime <= RoadRaceDefine.GOLD_TIME)
				{
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.ARCHER_BATTLE);
				}
			}
		}
		ResultGameDataParams.SetPoint();
		float[] array = new float[Scene_RoadRace.CM.MemberNum];
		int[] array2 = new int[Scene_RoadRace.CM.MemberNum];
		for (int i = 0; i < Scene_RoadRace.CM.MemberNum; i++)
		{
			array[i] = Scene_RoadRace.CM.GetChara(i).GetGoalTime();
			array2[i] = (int)Scene_RoadRace.CM.GetChara(i).UserType;
			CalcManager.ConvertTimeToRecordString(array[i], array2[i]);
		}
		ResultGameDataParams.SetRecord_Float(array, array2, _isGroup1Record: true, _isAscendingOrder: true);
		rankingResult.ShowResult_Time();
	}
	public void PlayVibration(int _playerNo = -1)
	{
		for (int i = 0; i < Scene_RoadRace.CM.MemberNum; i++)
		{
			if (!Scene_RoadRace.CM.GetChara(i).IsCpu && (_playerNo == -1 || i == _playerNo))
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(i);
			}
		}
	}
	public float GetGameTime()
	{
		return gameTime;
	}
	private void CPUAutoGoal()
	{
		SettingAutoGoalTime();
	}
	private void SettingAutoGoalTime()
	{
		float[] array = new float[Scene_RoadRace.CM.MemberNum];
		float num = 0f;
		for (int i = 0; i < Scene_RoadRace.CM.GetCharas().Length; i++)
		{
			if (Scene_RoadRace.CM.GetChara(i).IsGoal)
			{
				array[i] = Scene_RoadRace.CM.GetChara(i).GetGoalTime();
				if (num < array[i])
				{
					num = array[i];
				}
			}
		}
		for (int j = 0; j < Scene_RoadRace.CM.GetCharas().Length; j++)
		{
			int num2 = Scene_RoadRace.FM.RaceCheckPoint.Length * Scene_RoadRace.GM.LapNum;
			if (!Scene_RoadRace.CM.GetChara(j).IsGoal)
			{
				if (Scene_RoadRace.CM.GetChara(j).IsCpu)
				{
					Scene_RoadRace.CM.GetChara(j).SetGoalTime(num + num / (float)num2 * (float)(num2 - Scene_RoadRace.CM.GetChara(j).CheckPointNo) + Random.Range(0f, 1f));
				}
				else
				{
					Scene_RoadRace.CM.GetChara(j).SetGoalTime(-1f);
				}
			}
		}
	}
	public void DebugGameEnd()
	{
		for (int i = 0; i < Scene_RoadRace.CM.MemberNum; i++)
		{
			float time = Random.Range(60f, 80f);
			Scene_RoadRace.CM.Goal(i);
			Scene_RoadRace.CM.GetChara(i).SetGoalTime(CalcManager.ConvertDecimalSecond(time));
		}
		GameEnd(_isAutoGameEnd: true);
	}
}
