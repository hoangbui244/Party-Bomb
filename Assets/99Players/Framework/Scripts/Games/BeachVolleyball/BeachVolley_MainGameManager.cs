using SaveDataDefine;
using System;
using System.Collections.Generic;
using UnityEngine;
public class BeachVolley_MainGameManager : SingletonCustom<BeachVolley_MainGameManager>
{
	public enum GameState
	{
		GAME_START,
		GAME_START_STANDBY,
		SERVE_STANDBY,
		SERVE,
		IN_PLAY,
		SCORE_UP,
		SET_INTERVAL_WAIT,
		SET_INTERVAL,
		GAME_END_WAIT,
		GAME_END,
		NONE
	}
	public delegate void GameStateMethod();
	public enum RuleViolationType
	{
		FOUR_HITS,
		OUT_OF_BOUNDS,
		DOUBLE_CONTACT
	}
	public GameState gameState;
	public GameState gameStateWait;
	private bool isGameStart;
	private bool isFieldChange;
	private int setPlayTeamNo = -1;
	private int setPlayTeamNoPrev = -1;
	private int gameStartTeamNo;
	private GameStateMethod gameStateMethod;
	private bool isAutoAction;
	private bool isStopChara;
	private bool isFirstServe;
	private CharaScore charaScore = new CharaScore();
	private float restartTime;
	private Action gameEndCallBack;
	[SerializeField]
	[Header("UIエフェクトアンカ\u30fc")]
	private Transform effectAnchor;
	private BeachVolley_TextEffect faultEffect;
	[SerializeField]
	[Header("アウトオブバウンズエフェクト")]
	private BeachVolley_TextEffect outOfBoundsEffectPrefab;
	[SerializeField]
	[Header("フォアヒットエフェクト")]
	private BeachVolley_TextEffect fourHitsEffectPrefab;
	[SerializeField]
	[Header("ダブルコンタクトエフェクト")]
	private BeachVolley_TextEffect doubleContactEffectPrefab;
	private BeachVolley_TextEffect outEffect;
	[SerializeField]
	[Header("アウトエフェクト")]
	private BeachVolley_TextEffect outEffectPrefab;
	private BeachVolley_TextEffect matchPointEffect;
	[SerializeField]
	[Header("マッチポイントエフェクト")]
	private BeachVolley_TextEffect matchPointEffectPrefab;
	[SerializeField]
	[Header("クラッカ\u30fcエフェクト")]
	private ParticleSystem[] crackerEffect;
	private int selectSetNum;
	private int winNeedSetNum;
	private int selectPointNum = 15;
	private int setNo;
	private int[,] score;
	private int[,] realScore;
	private int[] getSet;
	private BeachVolley_Character characterTemp;
	private BeachVolley_Character server;
	[SerializeField]
	[Header("リザルト")]
	private WinOrLoseResultManager result;
	private bool changeCort;
	private string gameStateMethodDebug;
	private string throwerCharaNameDebug;
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
	public int SelectSetNum => selectSetNum;
	public int SelectPointNum
	{
		get
		{
			if (selectSetNum == BeachVolley_Define.GetPlaySetNum(2) && setNo == selectSetNum - 1)
			{
				return BeachVolley_Define.PLAY_POINT_LAST_SET[BeachVolley_Define.GetSelectPointNum(_indexConvert: true)];
			}
			return selectPointNum;
		}
	}
	public int SetNo
	{
		get
		{
			return setNo;
		}
		set
		{
			setNo = value;
		}
	}
	public string LanguageConvertValue => "";
	public bool IsTutorial => false;
	public int GetSetPlayTeamNoPrev()
	{
		return setPlayTeamNoPrev;
	}
	public void Init(Action _gameEndCallBack)
	{
		isGameStart = false;
		selectSetNum = BeachVolley_Define.GetSelectSetNum();
		winNeedSetNum = selectSetNum / 2 + 1;
		selectPointNum = BeachVolley_Define.GetSelectPointNum();
		gameStartTeamNo = ((!BeachVolley_Define.IsFirstPlayerFirstAttack()) ? 1 : 0);
		setNo = 0;
		BeachVolley_Define.GUM.SetSetNo(setNo);
		InitScoreData(selectSetNum);
		BeachVolley_Define.GUM.UpdateScoreDisplay(0, 0);
		BeachVolley_Define.GUM.UpdateScoreDisplay(1, 0);
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			effectAnchor.SetLocalPositionY(77f);
		}
		else
		{
			effectAnchor.SetLocalPositionY(55f);
		}
		charaScore.Init();
		gameEndCallBack = _gameEndCallBack;
		StartGameStart();
	}
	public void UpdateMethod()
	{
		if (gameStateMethod != null)
		{
			gameStateMethodDebug = gameStateMethod.Method.Name;
			gameStateMethod();
			if (UnityEngine.Input.GetKeyDown(KeyCode.Z))
			{
				InitScoreData(selectSetNum);
				for (int i = 0; i < SelectPointNum; i++)
				{
					ScoreUp(0, 0f);
				}
				EndGame();
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.X))
			{
				InitScoreData(selectSetNum);
				for (int j = 0; j < SelectPointNum; j++)
				{
					ScoreUp(1, 0f);
				}
				EndGame();
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.R))
			{
				InitScoreData(selectSetNum);
				if (UnityEngine.Random.Range(0, 2) == 0)
				{
					score[0, 0] = 10;
					score[1, 0] = 6;
				}
				else
				{
					score[0, 0] = 6;
					score[1, 0] = 10;
				}
				EndGame();
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.MOLE_HAMMER);
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.MOLE_HAMMER);
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.MOLE_HAMMER);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.O))
			{
				for (int k = 0; k < SelectPointNum; k++)
				{
					ScoreUp(0, 0f);
				}
				EndSet();
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.P))
			{
				for (int l = 0; l < SelectPointNum; l++)
				{
					ScoreUp(1, 0f);
				}
				EndSet();
			}
		}
		else
		{
			gameStateMethodDebug = "Null";
		}
	}
	public void InitScoreData(int _setNum)
	{
		score = new int[2, _setNum];
		realScore = new int[2, _setNum];
		getSet = new int[2];
	}
	public void AddScore(int _teamNo, int _setNo)
	{
		score[_teamNo, _setNo]++;
		if (score[_teamNo, _setNo] >= 99)
		{
			score[_teamNo, _setNo] = 99;
		}
		realScore[_teamNo, _setNo]++;
	}
	private void StartGameStart()
	{
		UnityEngine.Debug.Log("ゲ\u30fcム開始");
		BeachVolley_Define.MCM.SettingGameStart();
		isStopChara = true;
		isAutoAction = false;
		isFirstServe = true;
		BeachVolley_Define.BM.ResetLastBallData();
		gameState = GameState.GAME_START_STANDBY;
		restartTime = 1f;
		SetSetPlayTeamNo(gameStartTeamNo);
		gameStateMethod = StateGameStartStandby;
	}
	private void StateGameStartStandby()
	{
		restartTime -= Time.deltaTime;
		if (restartTime <= 0f)
		{
			restartTime = 0f;
			gameState = GameState.GAME_START;
			UnityEngine.Debug.Log("ゲ\u30fcムスタ\u30fcト準備完了");
			SingletonCustom<CommonStartSimple>.Instance.Show(FirstServeStandby);
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
			gameStateMethod = StateGameStartWait;
		}
	}
	private void StateGameStartWait()
	{
	}
	private void FirstServeStandby()
	{
		StartServeStandby(isFirstServe);
		BeachVolley_Define.MCM.ShowCursor(_show: true);
	}
	public void StartServeStandby(bool _isFirstServe)
	{
		isStopChara = false;
		if (!CheckTutorialServe())
		{
			isAutoAction = true;
		}
		isFirstServe = false;
		BeachVolley_Define.BM.ResetGravity();
		for (int i = 0; i < BeachVolley_Define.MCM.TeamList[0].charas.Length; i++)
		{
			BeachVolley_Define.MCM.TeamList[0].charas[i].EndFreeze();
			BeachVolley_Define.MCM.TeamList[1].charas[i].EndFreeze();
		}
		if (!_isFirstServe && setPlayTeamNoPrev != setPlayTeamNo)
		{
			BeachVolley_Define.MCM.TeamRotationBeach(setPlayTeamNo);
		}
		BeachVolley_Define.MCM.RotateAutoPlayer4(1 - setPlayTeamNo);
		BeachVolley_Define.FM.NetWall.gameObject.SetActive(value: false);
		UnityEngine.Debug.Log("サ\u30fcブセットチ\u30fcム\u3000：" + setPlayTeamNo.ToString());
		server = BeachVolley_Define.MCM.SettingServe(setPlayTeamNo);
		gameState = GameState.SERVE_STANDBY;
		BeachVolley_Define.MCM.SetCharaLayer(BeachVolley_Define.LAYER_INVISIBLE_CHARA);
		int num = CheckMatchPoint(setPlayTeamNo);
		if (num != -1)
		{
			PlayMatchPointEffect(num);
		}
		else
		{
			num = CheckSetPoint(setPlayTeamNo);
			if (num != -1)
			{
				PlaySetPointEffect(num);
			}
		}
		gameStateMethod = StateServeStandby;
	}
	private void StateServeStandby()
	{
		if (server.CheckActionState(BeachVolley_Character.ActionState.SERVE_WAIT))
		{
			isAutoAction = false;
			BeachVolley_Define.MCM.SetCharaLayer(BeachVolley_Define.LAYER_CHARACTER);
			BeachVolley_Define.FM.NetWall.gameObject.SetActive(value: true);
			StartServe();
		}
	}
	public void StartServe()
	{
		gameState = GameState.SERVE;
		gameStateMethod = StateServe;
	}
	private void StateServe()
	{
	}
	public void InCourt(int _teamNo)
	{
		if (!CheckGameState(GameState.IN_PLAY))
		{
			return;
		}
		if (BeachVolley_Define.Ball.GetLastHitChara().TeamNo == _teamNo)
		{
			SetSetPlayTeamNo(1 - BeachVolley_Define.Ball.GetLastHitChara().TeamNo);
			if (BeachVolley_Define.Ball.GetLastControlChara().TeamNo != BeachVolley_Define.Ball.GetLastHitChara().TeamNo)
			{
				charaScore.AddPoint(BeachVolley_Define.Ball.GetLastControlChara());
			}
		}
		else
		{
			SetSetPlayTeamNo(BeachVolley_Define.Ball.GetLastHitChara().TeamNo);
			charaScore.AddPoint(BeachVolley_Define.Ball.GetLastHitChara());
		}
		if (IsTutorial)
		{
			TutorialInCourt(_teamNo);
		}
		else
		{
			ScoreUp(setPlayTeamNo);
		}
	}
	public void OutCourt()
	{
		if (IsTutorial)
		{
			TutorialOutCourt();
		}
		else if (CheckGameState(GameState.IN_PLAY))
		{
			UnityEngine.Debug.Log("アウトコ\u30fcト");
			SetSetPlayTeamNo(1 - BeachVolley_Define.Ball.GetLastHitChara().TeamNo);
			if (BeachVolley_Define.Ball.GetLastControlChara().TeamNo != BeachVolley_Define.Ball.GetLastHitChara().TeamNo)
			{
				charaScore.AddPoint(BeachVolley_Define.Ball.GetLastControlChara());
			}
			if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				PlayOutEffect(BeachVolley_Define.Ball.GetLastHitChara().TeamNo);
			}
			else
			{
				PlayOutEffect();
			}
			ScoreUp(setPlayTeamNo, 1.55f);
		}
	}
	public void RuleViolation(RuleViolationType _type, int _teamNo)
	{
		if (IsTutorial)
		{
			TutorialRuleViolation(_type, _teamNo);
		}
		else if (CheckGameState(GameState.IN_PLAY))
		{
			UnityEngine.Debug.Log("ル\u30fcル違反 : " + _type.ToString());
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_short");
			SetSetPlayTeamNo(1 - _teamNo);
			if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				PlayFaultEffect(_type, BeachVolley_Define.Ball.GetLastHitChara().TeamNo);
			}
			else
			{
				PlayFaultEffect(_type);
			}
			if (IsTutorial)
			{
				TutorialRuleViolation(_type, _teamNo);
			}
			else
			{
				ScoreUp(setPlayTeamNo, 1.55f);
			}
		}
	}
	public void ServeMiss()
	{
		if (CheckGameState(GameState.IN_PLAY))
		{
			UnityEngine.Debug.Log("サ\u30fcブミス");
			SetSetPlayTeamNo(1 - BeachVolley_Define.Ball.GetLastHitChara().TeamNo);
			ScoreUp(setPlayTeamNo);
		}
	}
	public void ScoreUp(int _teamNo, float _restartTime = 0.8f)
	{
		restartTime = _restartTime;
		isStopChara = true;
		gameState = GameState.SCORE_UP;
		gameStateMethod = StateScoreUpWait;
		AddScore(_teamNo, setNo);
		BeachVolley_Define.BM.Release();
		BeachVolley_Define.MCM.BallTouchCnt = 0;
		if (GetNowSetScore(_teamNo) >= SelectPointNum)
		{
			getSet[_teamNo]++;
		}
	}
	public void StateScoreUpWait()
	{
		restartTime -= Time.deltaTime;
		if (restartTime <= 0f)
		{
			BeachVolley_Define.GUM.UpdateScoreDisplay(setPlayTeamNo, GetNowSetScore(setPlayTeamNo));
			restartTime = BeachVolley_Define.GUM.PlayAddScoreProduction(setPlayTeamNo, GetNowSetScore());
			SingletonCustom<AudioManager>.Instance.SePlay("se_get_score");
			gameStateMethod = StateScoreUp;
		}
	}
	public void StateScoreUp()
	{
		restartTime -= Time.deltaTime;
		if (!(restartTime <= 0f))
		{
			return;
		}
		BeachVolley_Define.Ball.GetRigid().velocity = CalcManager.mVector3Zero;
		BeachVolley_Define.BM.Release();
		if (CheckEndSet())
		{
			if (CheckEndGame())
			{
				EndGame();
			}
			else
			{
				EndSet();
			}
		}
		else
		{
			StartServeStandby(isFirstServe);
		}
	}
	private void StartNextSet()
	{
		StartServeStandby(_isFirstServe: true);
	}
	private void EndSet()
	{
		UnityEngine.Debug.Log("セット終了 : 第" + setNo.ToString() + "セット終了");
		restartTime = 0f;
		gameStateWait = (gameState = GameState.SET_INTERVAL_WAIT);
		gameStateMethod = StateEndSetWait;
		BeachVolley_Define.BM.Release();
	}
	private void StateEndSetWait()
	{
		if (GetNowSetScore(0) >= SelectPointNum)
		{
			BeachVolley_Define.GUM.SetSetCounter(0, GetGetSetNum(0));
		}
		else if (GetNowSetScore(1) >= SelectPointNum)
		{
			BeachVolley_Define.GUM.SetSetCounter(1, GetGetSetNum(1));
		}
		PlayEndSetSe();
		isStopChara = true;
		BeachVolley_Define.MCM.CharacterStop();
		UnityEngine.Debug.Log("一つ目");
		gameStateWait = GameState.NONE;
		PlayEndSetEffect();
		restartTime = 0f;
		gameStartTeamNo = ((gameStartTeamNo == 0) ? 1 : 0);
		SetSetPlayTeamNo(gameStartTeamNo);
		setPlayTeamNoPrev = setPlayTeamNo;
		setNo++;
		isFirstServe = true;
		gameState = GameState.SET_INTERVAL;
		gameStateMethod = StateShowSetIntervalEffect;
	}
	public void StateShowSetIntervalEffect()
	{
		restartTime += Time.deltaTime;
		if (restartTime >= 0.5f)
		{
			restartTime = 0f;
			UnityEngine.Debug.Log("ハ\u30fcフタイム表示");
			isStopChara = false;
			isAutoAction = true;
			ReturnCharaBench();
			gameStateMethod = StateShowSetInterval;
		}
	}
	public void StateShowSetInterval()
	{
		restartTime += Time.deltaTime;
		if (restartTime >= 0.5f)
		{
			restartTime = 0f;
			gameStateMethod = StateSetInterval;
		}
	}
	public void StateSetInterval()
	{
		restartTime += Time.deltaTime;
		if (restartTime >= 0.5f)
		{
			restartTime = 0f;
			BeachVolley_Define.FM.ReverseField(setNo);
			UnityEngine.Debug.Log("反対側のコ\u30fcトに移動開始");
			for (int i = 0; i < 2; i++)
			{
				BeachVolley_Define.MCM.ResetRotation(i, setNo);
			}
			ReturnCharaFromBench();
			Vector3 startLocalPos = BeachVolley_Define.Ball.GetStartLocalPos();
			if (!GetChangeCort())
			{
				startLocalPos.x *= -1f;
			}
			BeachVolley_Define.Ball.transform.localPosition = startLocalPos;
			BeachVolley_Define.Ball.ResetVelocity();
			gameStateMethod = StateFieldChange;
		}
	}
	public void StateFieldChange()
	{
		if (!(server == null) && server.IsMovePos)
		{
			return;
		}
		restartTime += Time.deltaTime;
		if (!(restartTime >= 1f))
		{
			return;
		}
		restartTime = 0f;
		if (BeachVolley_Define.CheckSelectCameraMode(BeachVolley_Define.CameraMode.VERTICAL))
		{
			BeachVolley_Define.FM.StartCameraReverseAnimation(1.75f);
		}
		else
		{
			changeCort = !changeCort;
			if (changeCort)
			{
				BeachVolley_Define.FM.ChangeCameraRootPos(1);
			}
			else
			{
				BeachVolley_Define.FM.ChangeCameraRootPos(0);
			}
			BeachVolley_Define.GUM.ChangeSchoolNamePos();
		}
		for (int i = 0; i < BeachVolley_Define.MCM.TeamList.Length; i++)
		{
			for (int j = 0; j < BeachVolley_Define.Return_team_infield_num(); j++)
			{
				BeachVolley_Define.MCM.TeamList[i].charas[j].SettingLayer(BeachVolley_Define.LAYER_CHARACTER);
				BeachVolley_Define.MCM.TeamList[i].charas[j].SetActionState(BeachVolley_Character.ActionState.STANDARD);
			}
		}
		gameStateMethod = StateReverseField;
	}
	public bool GetChangeCort()
	{
		return changeCort;
	}
	public void StateReverseField()
	{
		restartTime += Time.deltaTime;
		if (restartTime >= 1f)
		{
			BeachVolley_Define.GUM.SetSetNo(setNo);
			BeachVolley_Define.GUM.UpdateScoreDisplay(0, GetNowSetScore(0));
			BeachVolley_Define.GUM.UpdateScoreDisplay(1, GetNowSetScore(1));
			UnityEngine.Debug.Log("カメラ反転完了");
			isGameStart = false;
			isAutoAction = false;
			StartNextSet();
		}
	}
	private void EndGame()
	{
		UnityEngine.Debug.Log("試合終了");
		gameStateWait = (gameState = GameState.GAME_END_WAIT);
		gameStateMethod = StateEndGameWait;
	}
	private void StateEndGameWait()
	{
		if (outEffect == null || !outEffect.IsViewDisplay())
		{
			PlayGameFinishSe();
			isStopChara = true;
			BeachVolley_Define.MCM.CharacterStop();
			UnityEngine.Debug.Log("二つ目");
			gameStateWait = GameState.NONE;
			restartTime = 0f;
			SingletonCustom<CommonEndSimple>.Instance.Show();
			gameState = GameState.GAME_END;
			gameStateMethod = StateGameEnd;
		}
	}
	public void StateGameEnd()
	{
		restartTime += Time.deltaTime;
		if (restartTime >= 2.7f)
		{
			if (gameEndCallBack != null)
			{
				gameEndCallBack();
			}
			gameStateMethod = null;
			ToResult();
		}
	}
	public void ChangeStateInPlay(bool _resumeTime = false)
	{
		gameState = GameState.IN_PLAY;
		gameStateMethod = StateInPlay;
		isGameStart = true;
	}
	private void StateInPlay()
	{
	}
	public void PlayEndSetSe()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
	}
	public void PlayGameFinishSe()
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
	}
	private void ReturnCharaBench()
	{
		BeachVolley_Define.MCM.ShowCursor(_show: false);
		for (int i = 0; i < BeachVolley_Define.MCM.TeamList.Length; i++)
		{
			for (int j = 0; j < BeachVolley_Define.Return_team_infield_num(); j++)
			{
				Vector3 benchPos = BeachVolley_Define.FM.GetBenchPos(i, setNo);
				BeachVolley_Define.MCM.TeamList[i].charas[j].SetActionState(BeachVolley_Character.ActionState.MOVE_POS);
				BeachVolley_Define.MCM.TeamList[i].charas[j].SettingReturnBench(benchPos);
			}
		}
	}
	private void ReturnCharaFromBench()
	{
		BeachVolley_Define.MCM.ShowCursor(_show: true);
		UnityEngine.Debug.Log("キャラクタ\u30fcをベンチに返す！!!");
		for (int i = 0; i < BeachVolley_Define.MCM.TeamList.Length; i++)
		{
			for (int j = 0; j < BeachVolley_Define.Return_team_infield_num(); j++)
			{
				BeachVolley_Define.MCM.TeamList[i].charas[j].transform.SetLocalPositionX(0f - BeachVolley_Define.MCM.TeamList[i].charas[j].transform.localPosition.x);
				BeachVolley_Define.MCM.TeamList[i].charas[j].transform.SetPositionZ(SingletonCustom<BeachVolley_FieldManager>.Instance.GetFieldData().CenterCircle.position.z + (0f - (SingletonCustom<BeachVolley_FieldManager>.Instance.GetFieldData().CenterCircle.position.z - BeachVolley_Define.MCM.TeamList[i].charas[j].transform.position.z)));
				BeachVolley_Define.MCM.TeamList[i].charas[j].ShowCharacter();
				BeachVolley_Define.MCM.TeamList[i].charas[j].SetAction(BeachVolley_Define.MCM.TeamList[i].charas[j].AiReturnFromBench);
				BeachVolley_Define.MCM.TeamList[i].charas[j].IgnoreObstacleTime = 2f;
				BeachVolley_Define.MCM.TeamList[i].charas[j].SetActionState(BeachVolley_Character.ActionState.MOVE_POS);
			}
		}
	}
	public void PlayOutEffect(int _teamNo = -1)
	{
		outEffect = UnityEngine.Object.Instantiate(outEffectPrefab, effectAnchor.position, Quaternion.identity, effectAnchor);
		SingletonCustom<AudioManager>.Instance.VoicePlay("voice_beachvolley_ball_out");
	}
	public void PlayFaultEffect(RuleViolationType _type, int _teamNo = -1)
	{
		switch (_type)
		{
		case RuleViolationType.OUT_OF_BOUNDS:
			faultEffect = UnityEngine.Object.Instantiate(outOfBoundsEffectPrefab, effectAnchor.position, Quaternion.identity, effectAnchor);
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_beachvolley_out_of_bounds");
			break;
		case RuleViolationType.FOUR_HITS:
			faultEffect = UnityEngine.Object.Instantiate(fourHitsEffectPrefab, effectAnchor.position, Quaternion.identity, effectAnchor);
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_beachvolley_four_hit");
			break;
		case RuleViolationType.DOUBLE_CONTACT:
			faultEffect = UnityEngine.Object.Instantiate(doubleContactEffectPrefab, effectAnchor.position, Quaternion.identity, effectAnchor);
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_beachvolley_double_contact");
			break;
		}
	}
	public void PlayKickOffEffect()
	{
	}
	private void PlayEndSetEffect()
	{
	}
	private void PlayStartSetEffect()
	{
	}
	public void PlaySetPointEffect(int _teamNo = -1)
	{
	}
	public void PlayMatchPointEffect(int _teamNo = -1)
	{
		matchPointEffect = UnityEngine.Object.Instantiate(matchPointEffectPrefab, effectAnchor.position, Quaternion.identity, effectAnchor);
		SingletonCustom<AudioManager>.Instance.VoicePlay("voice_beachvolley_match_point");
	}
	private void MultiReverseEffect(GameObject _effect, int _teamNo)
	{
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && _teamNo == 1 && _effect != null)
		{
			_effect.transform.SetLocalEulerAnglesZ(180f);
		}
	}
	public void BallContactCharacter()
	{
	}
	public bool CheckInPlay()
	{
		if (!CheckGameState(GameState.IN_PLAY) && !CheckGameState(GameState.SERVE_STANDBY))
		{
			return CheckGameState(GameState.SERVE);
		}
		return true;
	}
	public bool CheckEndWait()
	{
		return false;
	}
	public bool CheckInPlayOrEndWait()
	{
		if (!CheckInPlay())
		{
			return CheckEndWait();
		}
		return true;
	}
	public bool CheckPlayGame()
	{
		if (!CheckGameState(GameState.IN_PLAY))
		{
			return !CheckGameState(GameState.GAME_END);
		}
		return false;
	}
	public bool CheckGameState(GameState _gameState)
	{
		return gameState == _gameState;
	}
	public bool CheckEndSet()
	{
		if (GetNowSetScore(0) < SelectPointNum)
		{
			return GetNowSetScore(1) >= SelectPointNum;
		}
		return true;
	}
	public bool CheckEndGame()
	{
		if (GetGetSetNum(0) < winNeedSetNum)
		{
			return GetGetSetNum(1) >= winNeedSetNum;
		}
		return true;
	}
	public int CheckSetPoint(int _serveTeam)
	{
		if (GetNowSetScore(_serveTeam) >= SelectPointNum - 1)
		{
			return _serveTeam;
		}
		if (GetNowSetScore(1 - _serveTeam) >= SelectPointNum - 1)
		{
			return 1 - _serveTeam;
		}
		return -1;
	}
	public int CheckMatchPoint(int _serveTeam)
	{
		int num = CheckSetPoint(_serveTeam);
		if (num != -1 && GetGetSetNum(num) >= winNeedSetNum - 1)
		{
			return num;
		}
		return -1;
	}
	public GameState GetGameState()
	{
		return gameState;
	}
	public int GetSetPlayTeamNo()
	{
		return setPlayTeamNo;
	}
	public int GetGameStartTeamNo()
	{
		return gameStartTeamNo;
	}
	public CharaScore GetPointHistory()
	{
		return charaScore;
	}
	public bool IsAutoAction()
	{
		return isAutoAction;
	}
	public bool IsStopChara()
	{
		return isStopChara;
	}
	public bool IsWaitState()
	{
		return gameStateWait != GameState.NONE;
	}
	public BeachVolley_Character GetServer()
	{
		return server;
	}
	public int GetScore(int _teamNo, int _setNo)
	{
		return score[_teamNo, _setNo];
	}
	public int GetNowSetScore(int _teamNo)
	{
		if (score == null)
		{
			return 0;
		}
		return score[_teamNo, setNo];
	}
	public int[] GetNowSetScore()
	{
		int[] array = new int[2];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = score[i, setNo];
		}
		return array;
	}
	public int GetGetSetNum(int _teamNo)
	{
		return getSet[_teamNo];
	}
	public void SetAutoAction(bool _flg)
	{
		isAutoAction = _flg;
	}
	public void SetSetPlayTeamNo(int _teamNo)
	{
		if (setPlayTeamNo == -1)
		{
			setPlayTeamNoPrev = _teamNo;
		}
		else
		{
			setPlayTeamNoPrev = setPlayTeamNo;
		}
		setPlayTeamNo = _teamNo;
	}
	public void SetGameStartTeamNo(int _teamNo)
	{
		gameStartTeamNo = _teamNo;
	}
	public void SetStopChara(bool _flg)
	{
		isStopChara = _flg;
	}
	public void TutorialInit()
	{
		BeachVolley_Define.MCM.SettingGameStart();
		isGameStart = false;
		selectSetNum = 1;
		winNeedSetNum = 1;
		selectPointNum = 5;
		gameStartTeamNo = 0;
		setNo = 0;
		effectAnchor.SetLocalPositionY(55f);
		charaScore.Init();
		BeachVolley_Define.MCM.ShowCursor(_show: true);
		gameState = GameState.NONE;
	}
	public void TutorialStartServe(int _teamNo)
	{
		UnityEngine.Debug.Log("ゲ\u30fcム開始");
		BeachVolley_Define.MCM.SettingGameStart();
		isStopChara = true;
		isAutoAction = false;
		isFirstServe = true;
		BeachVolley_Define.BM.ResetLastBallData();
		SetSetPlayTeamNo(_teamNo);
		restartTime = 0f;
		gameState = GameState.GAME_START;
		gameStateMethod = StateGameStartWait;
	}
	public void TutorialUpdateMethod()
	{
		UpdateMethod();
	}
	public void TutorialGameActive(bool _active, GameState _state = GameState.GAME_END)
	{
		isStopChara = !_active;
		gameState = _state;
	}
	public void TutorialRuleViolation(RuleViolationType _type, int _teamNo)
	{
	}
	public bool CheckTutorialServe()
	{
		return false;
	}
	public bool CheckTutorialServeMove()
	{
		return false;
	}
	public void TutorialServeMove(int _teamNo)
	{
	}
	public void TutorialInCourt(int _teamNo)
	{
	}
	public void TutorialOutCourt()
	{
	}
	private void ToResult()
	{
		ResultGameDataParams.SetPoint();
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (i == 0)
				{
					list.Add((int)BeachVolley_Define.MCM.TeamList[i].charas[j].UserType);
				}
				else
				{
					list2.Add((int)BeachVolley_Define.MCM.TeamList[i].charas[j].UserType);
				}
			}
		}
		list.Sort();
		list2.Sort();
		result.SetTeamPlayerGroupList(list, list2);
		ResultGameDataParams.SetRecord_WinOrLose(GetNowSetScore(0));
		ResultGameDataParams.SetRecord_WinOrLose(GetNowSetScore(1), 1);
		GS_Define.GameFormat selectGameFormat = SingletonCustom<GameSettingManager>.Instance.SelectGameFormat;
		SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.BATTLE_AND_COOP;
		if (GetNowSetScore(0) == GetNowSetScore(1))
		{
			result.ShowResult(ResultGameDataParams.ResultType.Draw, -1);
		}
		else if (GetNowSetScore(0) > GetNowSetScore(1))
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
			{
				switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
				{
				case SystemData.AiStrength.WEAK:
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.MOLE_HAMMER);
					break;
				case SystemData.AiStrength.NORAML:
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.MOLE_HAMMER);
					break;
				case SystemData.AiStrength.STRONG:
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.MOLE_HAMMER);
					break;
				}
			}
			result.ShowResult(ResultGameDataParams.ResultType.Win, 0);
		}
		else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			result.ShowResult(ResultGameDataParams.ResultType.Lose, 0);
		}
		else
		{
			result.ShowResult(ResultGameDataParams.ResultType.Win, 1);
		}
		SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = selectGameFormat;
	}
}
