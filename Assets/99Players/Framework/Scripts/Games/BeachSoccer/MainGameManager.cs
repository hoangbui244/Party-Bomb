using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace BeachSoccer
{
	public class MainGameManager : SingletonCustom<MainGameManager>
	{
		public enum GameState
		{
			HALF_TIME,
			KICK_OFF,
			IN_PLAY,
			GOAL_KICK,
			GOAL,
			CORNER_KICK,
			THROW_IN,
			GAME_END
		}
		public enum AdditionalTimeAddType
		{
			THROW_IN,
			CORNER_KICK,
			GOAL_KICK,
			GOAL,
			MAX
		}
		public struct GoalData
		{
			public bool isFirstHalf;
			public float time;
			public CharacterScript chara;
			public bool isOwn;
			public GoalData(bool _isFirstHalf, float _time, CharacterScript _chara, bool _isOwn)
			{
				isFirstHalf = _isFirstHalf;
				time = _time;
				chara = _chara;
				isOwn = _isOwn;
			}
		}
		public delegate void GameStateMethod();
		public GameState gameState;
		private int[] additionalTimeAddNum = new int[4];
		private float additionalTime;
		private float additionalTimeCorr;
		private float nowAdditionalTime;
		private bool isEndAdditionalTime;
		private bool isAdditionalTimeInit;
		private bool isGameStart;
		private float SET_PLAY_INTERVAL = 2f;
		private float restartTime;
		private CharacterScript characterTemp;
		private CharacterScript thrower;
		private CharacterScript kicker;
		private List<GoalData> goalDataListTeam0 = new List<GoalData>();
		private List<GoalData> goalDataListTeam1 = new List<GoalData>();
		private int setPlayTeamNo;
		private GameStateMethod gameStateMethod;
		private string gameStateMethodDebug;
		private bool isFirstHalf = true;
		private float gameTime;
		private float settingGameTime;
		private float gameTimeCorr;
		private bool isStopTime;
		private bool isAutoMove;
		private bool isShowSetPlayEffect;
		private bool isStopChara;
		[SerializeField]
		[Header("ゲ\u30fcム時間")]
		private TextMeshPro gameTimeText;
		private Action gameEndCallBack;
		[SerializeField]
		[Header("UIエフェクトアンカ\u30fc")]
		private Transform effectAnchor;
		private int goalTeam;
		private GoalEffect goalEffect;
		private int[] goalVoiceList = new int[10]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			9,
			10,
			15
		};
		[SerializeField]
		[Header("クラッカ\u30fcエフェクト")]
		private ParticleSystem[] crackerEffect;
		private CornerKickEffect cornerKickEffect;
		private ThrowInEffect throwInEffect;
		private GoalKickEffect goalKickEffect;
		private KickOffEffect kickOffEffect;
		private bool isShowKickOffEffect = true;
		private EndFirstHalfEffect endFirstHalfEffect;
		private AdditionalTimeEffect additionalTimeEffect;
		[SerializeField]
		[Header("コ\u30fcナ\u30fcキックエフェクト")]
		private CornerKickEffect sceneCornerKickEffect;
		[SerializeField]
		[Header("スロ\u30fcインエフェクト")]
		private ThrowInEffect sceneThrowInEffect;
		[SerializeField]
		[Header("ゴ\u30fcルキックエフェクト")]
		private GoalKickEffect sceneGoalKickEffect;
		[SerializeField]
		[Header("キックオフエフェクト")]
		private KickOffEffect sceneKickOffEffect;
		[SerializeField]
		[Header("前半終了エフェクト")]
		private EndFirstHalfEffect sceneEndFirstHalfEffect;
		[SerializeField]
		[Header("試合終了エフェクト")]
		private EndGameEffect sceneEndGameEffect;
		[SerializeField]
		[Header("アディショナルタイムエフェクト")]
		private AdditionalTimeEffect sceneAdditionalTimeEffect;
		[SerializeField]
		[Header("ゴ\u30fcルエフェクト")]
		private GoalEffect sceneGoalEffect;
		[SerializeField]
		[Header("敵のゴ\u30fcルエフェクト")]
		private GoalEffect sceneGoalEffectEnemy;
		private bool isKickOffStandby;
		private bool isFieldChange;
		private readonly Vector3 TEXT_EFFECT_OFFSET = new Vector3(0f, -400f, 0f);
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
		public string LanguageConvertValue
		{
			get
			{
				if (!SingletonCustom<GameUiManager>.Instance.IsNotJapanese)
				{
					return "";
				}
				return "English/";
			}
		}
		public void Init(Action _gameEndCallBack)
		{
			isGameStart = false;
			isFirstHalf = true;
			gameTime = 0f;
			settingGameTime = (float)GameSaveData.GetSelectMatchTimeNum() * 60f;
			gameTimeText.text = "00:00";
			gameTimeCorr = 5400f / settingGameTime;
			GameSaveData.SetFirstPlayerFirstAttack(UnityEngine.Random.Range(0, 2) == 0);
			setPlayTeamNo = ((!GameSaveData.IsFirstPlayerFirstAttack()) ? 1 : 0);
			StartKickOff();
			UnityEngine.Debug.Log("初回キックオフ設定");
			gameEndCallBack = _gameEndCallBack;
		}
		public void UpdateMethod()
		{
			if (isGameStart && !isStopTime)
			{
				gameTime += Time.deltaTime * gameTimeCorr;
				if (isFirstHalf)
				{
					if (gameTime >= 2700f)
					{
						gameTime = 2700f;
						if (CheckGameState(GameState.IN_PLAY))
						{
							EndFirstHalf();
						}
					}
					UpdateGameTime(gameTime);
				}
				else if (gameTime >= 5400f)
				{
					gameTime = 5400f;
					if (CheckGameState(GameState.IN_PLAY))
					{
						if (isEndAdditionalTime)
						{
							EndGame();
						}
						else
						{
							InitAdditionalTime();
						}
					}
					if (isAdditionalTimeInit)
					{
						nowAdditionalTime -= Time.deltaTime * additionalTimeCorr;
						if (nowAdditionalTime <= 0f)
						{
							isEndAdditionalTime = true;
							nowAdditionalTime = 0f;
						}
						UpdateGameTime(nowAdditionalTime);
					}
					else
					{
						UpdateGameTime(gameTime);
					}
				}
				else
				{
					UpdateGameTime(gameTime);
				}
			}
			if (gameStateMethod != null)
			{
				gameStateMethodDebug = gameStateMethod.Method.Name;
				gameStateMethod();
			}
			else
			{
				gameStateMethodDebug = "Null";
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
			{
				gameTime = 2700f;
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
			{
				gameTime = 5400f;
			}
		}
		private void EndFirstHalf()
		{
			UnityEngine.Debug.Log("前半終了");
			endFirstHalfEffect = UnityEngine.Object.Instantiate(sceneEndFirstHalfEffect.gameObject, effectAnchor.position, Quaternion.identity, effectAnchor).GetComponent<EndFirstHalfEffect>();
			endFirstHalfEffect.gameObject.SetActive(value: true);
			gameState = GameState.HALF_TIME;
			gameStateMethod = StateShowHalfTimeEffect;
			StartCoroutine(_PlayHalfTimeSe());
			restartTime = 0f;
			isStopChara = true;
			isStopTime = true;
			isEndAdditionalTime = false;
			isAdditionalTimeInit = false;
			isShowKickOffEffect = true;
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_end_first_half");
			UpdateGameTime(gameTime);
		}
		private void EndGame()
		{
			UnityEngine.Debug.Log("試合終了");
			gameState = GameState.GAME_END;
			StartCoroutine(_PlayGameFinishSe());
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_time_up");
			UnityEngine.Object.Instantiate(sceneEndGameEffect.gameObject, effectAnchor.position, Quaternion.identity, effectAnchor).SetActive(value: true);
			isStopChara = true;
			isStopTime = true;
			UpdateGameTime(gameTime);
			gameStateMethod = StateGameEnd;
		}
		private void InitAdditionalTime()
		{
			if (isAdditionalTimeInit)
			{
				return;
			}
			isAdditionalTimeInit = true;
			additionalTime = 0f;
			for (int i = 0; i < additionalTimeAddNum.Length; i++)
			{
				if (additionalTimeAddNum[i] != 0)
				{
					switch (i)
					{
					case 1:
						additionalTime += 30 * additionalTimeAddNum[i];
						break;
					case 3:
						additionalTime += 60 * additionalTimeAddNum[i];
						break;
					case 2:
						additionalTime += 30 * additionalTimeAddNum[i];
						break;
					case 0:
						additionalTime += 30 * additionalTimeAddNum[i];
						break;
					}
				}
			}
			if (additionalTime > 0f)
			{
				if (additionalTime < 150f)
				{
					additionalTime = 15f;
				}
				else if (additionalTime >= 450f)
				{
					additionalTime = 30f;
				}
				else
				{
					additionalTime = 20f;
				}
			}
			if (additionalTime > 0f)
			{
				SingletonCustom<GameUiManager>.Instance.ChangeGameTye(_additionalTime: true);
				SingletonCustom<GameUiManager>.Instance.ChangeTimeBackType(_additionalTime: true);
				additionalTimeEffect = UnityEngine.Object.Instantiate(sceneAdditionalTimeEffect.gameObject, effectAnchor.position + TEXT_EFFECT_OFFSET, Quaternion.identity, effectAnchor).GetComponent<AdditionalTimeEffect>();
				additionalTimeEffect.gameObject.SetActive(value: true);
				if (!GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.MULTI))
				{
					additionalTimeEffect.transform.SetLocalPositionY(-400f);
					additionalTimeEffect.transform.SetLocalEulerAngles(0f, 0f, 0f);
				}
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_additional_time");
				if (additionalTime >= 30f)
				{
					additionalTimeCorr = 300f / additionalTime;
					additionalTime = 300f;
					additionalTimeEffect.Play(5f);
				}
				else if (additionalTime >= 20f)
				{
					additionalTimeCorr = 180f / additionalTime;
					additionalTime = 180f;
					additionalTimeEffect.Play(3f);
				}
				else
				{
					additionalTimeCorr = 120f / additionalTime;
					additionalTime = 120f;
					additionalTimeEffect.Play(2f);
				}
				nowAdditionalTime = additionalTime;
				UpdateGameTime(nowAdditionalTime);
			}
		}
		private void UpdateGameTime(float _time)
		{
			gameTimeText.text = ((int)(_time / 60f)).ToString("D2") + ":" + ((int)(_time % 60f)).ToString("D2");
		}
		public void StateShowHalfTimeEffect()
		{
			if (endFirstHalfEffect == null)
			{
				restartTime = 0f;
				UnityEngine.Debug.Log("ハ\u30fcフタイム表示");
				SingletonCustom<GameUiManager>.Instance.ShowHalfTime();
				SingletonCustom<GameUiManager>.Instance.ChangeTimeBackType();
				isStopChara = false;
				isAutoMove = true;
				ReturnCharaBench();
				gameStateMethod = StateHalfTime;
			}
		}
		private void ReturnCharaBench()
		{
			SingletonCustom<BallManager>.Instance.Release();
			SingletonCustom<MainCharacterManager>.Instance.ShowCursor(_show: false);
			SingletonCustom<MainCharacterManager>.Instance.ShowPassCircle(_show: false);
			for (int i = 0; i < SingletonCustom<MainCharacterManager>.Instance.CharaList.Length; i++)
			{
				for (int j = 0; j < SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas.Length; j++)
				{
					Vector3 position = SingletonCustom<FieldManager>.Instance.GetAnchors().centerCircle.transform.position;
					int selectArea = GameSaveData.GetSelectArea();
					if (selectArea == 3 || selectArea == 12)
					{
						position.x -= SingletonCustom<FieldManager>.Instance.GetFieldData().halfSize.x * 0.6f;
					}
					else
					{
						position.x += SingletonCustom<FieldManager>.Instance.GetFieldData().halfSize.x * 0.6f;
					}
					position.z += SingletonCustom<FieldManager>.Instance.GetFieldData().halfSize.z * 0.75f * (float)((i != 0) ? 1 : (-1));
					position.z += UnityEngine.Random.Range(-2, 2);
					if (GameSaveData.GetSelectArea() == 18)
					{
						position = SingletonCustom<FieldManager>.Instance.GetStageData().GetArrayFixBenchAnchor()[i].position;
					}
					SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].SettingReturnBench(position);
				}
			}
			isFieldChange = true;
		}
		public void StateHalfTime()
		{
			restartTime += Time.deltaTime;
			if (!(restartTime >= 3f))
			{
				return;
			}
			isFirstHalf = false;
			SingletonCustom<GameUiManager>.Instance.ChangeGameTye();
			restartTime = 0f;
			SingletonCustom<FieldManager>.Instance.ReverseField();
			UnityEngine.Debug.Log("反対側のコ\u30fcトに移動開始");
			for (int i = 0; i < SingletonCustom<MainCharacterManager>.Instance.CharaList.Length; i++)
			{
				for (int j = 0; j < SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas.Length; j++)
				{
					SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].IgnoreObstacleTime = 3f;
					if (GameSaveData.GetSelectArea() == 18)
					{
						SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].transform.SetLocalPositionX(0f - SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].transform.localPosition.x);
						SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].transform.AddLocalPositionZ(0f);
					}
					else
					{
						SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].transform.SetLocalPositionX(0f - SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].transform.localPosition.x);
						SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].transform.AddLocalPositionZ(SingletonCustom<FieldManager>.Instance.GetFieldData().halfSize.z * 1.5f);
					}
				}
			}
			setPlayTeamNo = (GameSaveData.IsFirstPlayerFirstAttack() ? 1 : 0);
			SingletonCustom<MainCharacterManager>.Instance.SettingKickOff(setPlayTeamNo);
			gameStateMethod = StateFieldChange;
		}
		public void StateFieldChange()
		{
			if (SingletonCustom<MainCharacterManager>.Instance.CheckAllKickOffStandby())
			{
				isAutoMove = false;
				isStopChara = true;
				SingletonCustom<FieldManager>.Instance.StartCameraReverseAnimation(2f);
				if (GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL))
				{
					SingletonCustom<GameUiManager>.Instance.Fade(1.85f, 0.075f);
				}
				gameStateMethod = StateReverseField;
			}
		}
		public void StateReverseField()
		{
			restartTime += Time.deltaTime;
			float num = 2f;
			if (GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.HORIZONTAL))
			{
				num = 0.5f;
			}
			if (restartTime >= num)
			{
				UnityEngine.Debug.Log("カメラ反転完了");
				restartTime = 0f;
				isFieldChange = false;
				SingletonCustom<FieldManager>.Instance.ReverseField();
				isAutoMove = false;
				isStopChara = false;
				gameState = GameState.KICK_OFF;
				gameStateMethod = StateKickOffStandby;
			}
		}
		public IEnumerator _PlayHalfTimeSe()
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long", _loop: false, 0.5f);
			yield return new WaitForSeconds(1f);
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
		}
		public void StateGameEnd()
		{
			restartTime += Time.deltaTime;
			if (restartTime >= 4.7f)
			{
				if (gameEndCallBack != null)
				{
					gameEndCallBack();
				}
				gameStateMethod = null;
			}
		}
		public IEnumerator _PlayGameFinishSe()
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long", _loop: false, 0.5f);
			yield return new WaitForSeconds(1f);
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long", _loop: false, 0.5f);
			yield return new WaitForSeconds(1f);
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
		}
		public void Goal(int _teamNo)
		{
			if (!CheckGameState(GameState.IN_PLAY))
			{
				return;
			}
			gameState = GameState.GOAL;
			goalTeam = _teamNo;
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.GOAL);
			SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
			UnityEngine.Debug.Log("チ\u30fcム" + _teamNo.ToString() + "がゴ\u30fcル");
			setPlayTeamNo = ((_teamNo == 0) ? 1 : 0);
			additionalTimeAddNum[3]++;
			isShowSetPlayEffect = false;
			for (int i = 0; i < SingletonCustom<MainCharacterManager>.Instance.CharaList.Length; i++)
			{
				for (int j = 0; j < SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas.Length; j++)
				{
					SingletonCustom<MainCharacterManager>.Instance.CharaList[i].charas[j].SetGoalProduction(_teamNo);
				}
			}
			gameStateMethod = StateGoal;
		}
		public void OutGoalLine()
		{
			if (!CheckGameState(GameState.IN_PLAY))
			{
				return;
			}
			if (!SingletonCustom<BallManager>.Instance.CheckBallState(BallManager.BallState.FREE))
			{
				SingletonCustom<BallManager>.Instance.Release();
			}
			if (SingletonCustom<FieldManager>.Instance.CheckBallWhichTeamArea() == SingletonCustom<BallManager>.Instance.GetBall().GetLastHitChara().TeamNo)
			{
				if (SingletonCustom<BallManager>.Instance.GetBall().GetLastHitChara().TeamNo == 0)
				{
					setPlayTeamNo = 1;
				}
				else
				{
					setPlayTeamNo = 0;
				}
				SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.CORNER_KICK);
				additionalTimeAddNum[1]++;
				isShowSetPlayEffect = false;
				gameState = GameState.CORNER_KICK;
				kicker = SingletonCustom<MainCharacterManager>.Instance.SearchThrowIn(setPlayTeamNo);
				kicker.SetMovePos(SingletonCustom<BallManager>.Instance.GetBall().GetResetPos(BallManager.BallState.CORNER_KICK), SET_PLAY_INTERVAL);
				restartTime = 0f;
				gameStateMethod = StateCornerKick;
			}
			else
			{
				setPlayTeamNo = SingletonCustom<FieldManager>.Instance.CheckBallWhichTeamArea();
				SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.GOAL_KICK);
				additionalTimeAddNum[2]++;
				isShowSetPlayEffect = false;
				kicker = SingletonCustom<MainCharacterManager>.Instance.GetKeeper(setPlayTeamNo);
				kicker.SetMovePos(SingletonCustom<BallManager>.Instance.GetBall().GetResetPos(BallManager.BallState.GOAL_KICK), SET_PLAY_INTERVAL);
				restartTime = 0f;
				gameState = GameState.GOAL_KICK;
				gameStateMethod = StateGoalKick;
			}
			isAutoMove = true;
		}
		public void OutSideLine()
		{
			if (CheckGameState(GameState.IN_PLAY))
			{
				SingletonCustom<BallManager>.Instance.Release();
				SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.THROW_IN);
				restartTime = 0f;
				if (SingletonCustom<BallManager>.Instance.GetBall().GetLastHitChara().TeamNo == 0)
				{
					setPlayTeamNo = 1;
				}
				else
				{
					setPlayTeamNo = 0;
				}
				additionalTimeAddNum[0]++;
				isShowSetPlayEffect = false;
				gameState = GameState.THROW_IN;
				thrower = SingletonCustom<MainCharacterManager>.Instance.SearchThrowIn(setPlayTeamNo);
				thrower.SetMovePos(SingletonCustom<BallManager>.Instance.GetBall().GetResetPos(BallManager.BallState.THROW_IN), SET_PLAY_INTERVAL);
				gameStateMethod = StateThrowIn;
			}
		}
		private void StartKickOff()
		{
			UnityEngine.Debug.Log("チ\u30fcム" + setPlayTeamNo.ToString() + "がキックオフ");
			SingletonCustom<MainCharacterManager>.Instance.SettingKickOff(setPlayTeamNo);
			isAutoMove = false;
			isStopChara = false;
			isKickOffStandby = true;
			gameState = GameState.KICK_OFF;
			gameStateMethod = StateKickOffStandby;
		}
		private void StateKickOffStandby()
		{
			if (SingletonCustom<MainCharacterManager>.Instance.CheckAllKickOffStandby())
			{
				UnityEngine.Debug.Log("キックオフ準備完了");
				isKickOffStandby = false;
				isStopChara = false;
				isStopTime = false;
				SingletonCustom<MainCharacterManager>.Instance.ShowCursor(_show: true);
				if (isShowKickOffEffect)
				{
					kickOffEffect = UnityEngine.Object.Instantiate(sceneKickOffEffect.gameObject, effectAnchor.position + TEXT_EFFECT_OFFSET, Quaternion.identity, effectAnchor).GetComponent<KickOffEffect>();
					kickOffEffect.gameObject.SetActive(value: true);
					SingletonCustom<AudioManager>.Instance.VoicePlay("voice_kick_off");
					isShowSetPlayEffect = true;
					MultiReverseEffect(kickOffEffect.gameObject, setPlayTeamNo);
					isShowKickOffEffect = false;
				}
				isGameStart = true;
				SingletonCustom<GameUiManager>.Instance.TimeLimitStart(TimeLimitGauge.TIME_LIMIT_TYPE.KICK_OFF, setPlayTeamNo);
				SingletonCustom<AudioManager>.Instance.SePlay("se_whistle_long");
				gameStateMethod = StateKickOffWait;
			}
		}
		public bool IsKickOffStandby()
		{
			if (!isKickOffStandby)
			{
				return isFieldChange;
			}
			return true;
		}
		public bool IsFirstHalf()
		{
			return isFirstHalf;
		}
		private void StateKickOffWait()
		{
			if (gameState == GameState.IN_PLAY)
			{
				gameStateMethod = StateInPlay;
			}
			if (kickOffEffect == null)
			{
				isShowSetPlayEffect = false;
			}
		}
		public void ChangeStateInPlay()
		{
			gameState = GameState.IN_PLAY;
			gameStateMethod = StateInPlay;
		}
		private void StateInPlay()
		{
		}
		private void StateGoal()
		{
			if (!isShowSetPlayEffect && additionalTimeEffect == null)
			{
				SingletonCustom<AudienceManager>.Instance.ChangePeopleAnim(AudienceManager.AnimType.VERY_EXCITE);
				if (SingletonCustom<MainCharacterManager>.Instance.IsPlayer((setPlayTeamNo == 0) ? 1 : 0) || !SingletonCustom<MainCharacterManager>.Instance.IsPlayer(0))
				{
					goalEffect = UnityEngine.Object.Instantiate(sceneGoalEffect.gameObject, effectAnchor.position, Quaternion.identity, effectAnchor).GetComponent<GoalEffect>();
					goalEffect.gameObject.SetActive(value: true);
				}
				else
				{
					goalEffect = UnityEngine.Object.Instantiate(sceneGoalEffectEnemy.gameObject, effectAnchor.position, Quaternion.identity, effectAnchor).GetComponent<GoalEffect>();
					goalEffect.gameObject.SetActive(value: true);
				}
				MultiReverseEffect(goalEffect.gameObject, (setPlayTeamNo == 0) ? 1 : 0);
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_goal_soccer");
				CharacterScript characterScript = (!(SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara() != null)) ? SingletonCustom<BallManager>.Instance.GetBall().GetLastShootChara() : SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara();
				SingletonCustom<BallManager>.Instance.Release();
				if (setPlayTeamNo != 0 || 1 == 0)
				{
					goalDataListTeam0.Add(new GoalData(isFirstHalf, gameTime / 60f, characterScript, characterScript.TeamNo != ((setPlayTeamNo == 0) ? 1 : 0)));
				}
				else
				{
					goalDataListTeam1.Add(new GoalData(isFirstHalf, gameTime / 60f, characterScript, characterScript.TeamNo != ((setPlayTeamNo == 0) ? 1 : 0)));
				}
				goalEffect.Play(characterScript, characterScript.TeamNo != ((setPlayTeamNo == 0) ? 1 : 0));
				if (!SingletonCustom<MainCharacterManager>.Instance.IsPlayer(0))
				{
					crackerEffect[0].Play();
				}
				else if (SingletonCustom<MainCharacterManager>.Instance.IsPlayer((setPlayTeamNo == 0) ? 1 : 0))
				{
					crackerEffect[(setPlayTeamNo == 0) ? 1 : 0].Play();
				}
				restartTime = 0f;
				SingletonCustom<MainCharacterManager>.Instance.ShowCursor(_show: false);
				SingletonCustom<MainCharacterManager>.Instance.ShowPassCircle(_show: false);
				gameStateMethod = StateGoalInterval;
				isShowSetPlayEffect = true;
			}
		}
		private void StateGoalInterval()
		{
			if (goalEffect == null)
			{
				SingletonCustom<GameUiManager>.Instance.AddScore(goalTeam);
				SingletonCustom<GameUiManager>.Instance.ShowAddScoreObj(goalTeam);
				restartTime = 0f;
				gameStateMethod = StateGoalAddScore;
			}
		}
		private void StateGoalAddScore()
		{
			restartTime += Time.deltaTime;
			if (restartTime >= 1.75f)
			{
				restartTime = 0f;
				SingletonCustom<AudienceManager>.Instance.ChangePeopleAnim(AudienceManager.AnimType.NORMAL);
				isStopChara = false;
				StartKickOff();
			}
		}
		private void StateThrowIn()
		{
			if (!isShowSetPlayEffect)
			{
				if (additionalTimeEffect == null)
				{
					GameObject gameObject;
					MultiReverseEffect(gameObject = UnityEngine.Object.Instantiate(sceneThrowInEffect.gameObject, effectAnchor.position, Quaternion.identity, effectAnchor), setPlayTeamNo);
					gameObject.SetActive(value: true);
					SingletonCustom<AudioManager>.Instance.VoicePlay("voice_throw_in");
					isShowSetPlayEffect = true;
				}
				return;
			}
			restartTime += Time.deltaTime;
			if (restartTime >= SET_PLAY_INTERVAL)
			{
				restartTime = 0f;
				isShowSetPlayEffect = false;
				isAutoMove = false;
				gameStateMethod = StateThrowInStandby;
			}
		}
		private void StateThrowInStandby()
		{
			UnityEngine.Debug.Log("スロ\u30fcイン準備");
			SingletonCustom<MainCharacterManager>.Instance.ResetHaveBallChara();
			SingletonCustom<MainCharacterManager>.Instance.CatchBall(thrower);
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.THROW_IN);
			SingletonCustom<BallManager>.Instance.GetBall().ResetPos();
			SingletonCustom<GameUiManager>.Instance.TimeLimitStart(TimeLimitGauge.TIME_LIMIT_TYPE.THROW_IN, setPlayTeamNo);
			thrower.SettingThrowIn(SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().PlayerNo != -1);
			gameStateMethod = StateThrowInWait;
		}
		private void StateThrowInWait()
		{
			if (thrower.IsBallThrow())
			{
				gameStateMethod = StateInPlay;
				gameState = GameState.IN_PLAY;
				return;
			}
			SingletonCustom<BallManager>.Instance.GetBall().transform.SetLocalPosition(0f, 0f, 0f);
			SingletonCustom<BallManager>.Instance.GetBall().GetRigid().velocity = CalcManager.mVector3Zero;
			SingletonCustom<BallManager>.Instance.GetBall().GetRigid().angularVelocity = CalcManager.mVector3Zero;
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.THROW_IN);
		}
		private void StateCornerKick()
		{
			if (!isShowSetPlayEffect)
			{
				if (additionalTimeEffect == null)
				{
					GameObject gameObject;
					MultiReverseEffect(gameObject = UnityEngine.Object.Instantiate(sceneCornerKickEffect.gameObject, effectAnchor.position, Quaternion.identity, effectAnchor), setPlayTeamNo);
					gameObject.SetActive(value: true);
					SingletonCustom<AudioManager>.Instance.VoicePlay("voice_corner_kick");
					isShowSetPlayEffect = true;
				}
				return;
			}
			restartTime += Time.deltaTime;
			if (restartTime >= SET_PLAY_INTERVAL)
			{
				restartTime = 0f;
				isShowSetPlayEffect = false;
				isAutoMove = false;
				gameStateMethod = StateCornerKickStandby;
			}
		}
		private void StateCornerKickStandby()
		{
			SingletonCustom<MainCharacterManager>.Instance.ResetHaveBallChara();
			SingletonCustom<MainCharacterManager>.Instance.CatchBall(kicker);
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.CORNER_KICK);
			SingletonCustom<BallManager>.Instance.GetBall().ResetPos();
			kicker.SettingCornerKick(SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().PlayerNo != -1);
			SingletonCustom<GameUiManager>.Instance.TimeLimitStart(TimeLimitGauge.TIME_LIMIT_TYPE.CORNER_KICK, setPlayTeamNo);
			gameStateMethod = StateCornerKickWait;
		}
		private void StateCornerKickWait()
		{
			if (kicker.IsBallKick())
			{
				gameStateMethod = StateInPlay;
				gameState = GameState.IN_PLAY;
			}
			else
			{
				SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.CORNER_KICK);
				SingletonCustom<BallManager>.Instance.GetBall().ResetPos();
			}
		}
		private void StateGoalKick()
		{
			if (!isShowSetPlayEffect)
			{
				if (additionalTimeEffect == null)
				{
					GameObject gameObject;
					MultiReverseEffect(gameObject = UnityEngine.Object.Instantiate(sceneGoalKickEffect.gameObject, effectAnchor.position, Quaternion.identity, effectAnchor), setPlayTeamNo);
					gameObject.SetActive(value: true);
					SingletonCustom<AudioManager>.Instance.VoicePlay("voice_goal_kick");
					isShowSetPlayEffect = true;
				}
				return;
			}
			restartTime += Time.deltaTime;
			if (restartTime >= SET_PLAY_INTERVAL)
			{
				restartTime = 0f;
				isShowSetPlayEffect = false;
				isAutoMove = false;
				gameStateMethod = StateGoalKickStandby;
			}
		}
		private void StateGoalKickStandby()
		{
			SingletonCustom<MainCharacterManager>.Instance.ResetHaveBallChara();
			SingletonCustom<MainCharacterManager>.Instance.CatchBall(kicker);
			SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.GOAL_KICK);
			SingletonCustom<BallManager>.Instance.GetBall().ResetPos();
			kicker.SettingGoalKick(SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara().PlayerNo != -1);
			SingletonCustom<GameUiManager>.Instance.TimeLimitStart(TimeLimitGauge.TIME_LIMIT_TYPE.GOAL_KICK, setPlayTeamNo);
			gameStateMethod = StateGoalKickWait;
		}
		private void StateGoalKickWait()
		{
			if (kicker.IsBallKick())
			{
				gameStateMethod = StateInPlay;
				gameState = GameState.IN_PLAY;
			}
			else
			{
				SingletonCustom<BallManager>.Instance.SetBallState(BallManager.BallState.GOAL_KICK);
				SingletonCustom<BallManager>.Instance.GetBall().ResetPos();
			}
		}
		private void MultiReverseEffect(GameObject _effect, int _teamNo)
		{
		}
		public bool CheckInPlay()
		{
			return CheckGameState(GameState.IN_PLAY);
		}
		public bool CheckPlayGame()
		{
			if (!CheckGameState(GameState.IN_PLAY))
			{
				return !CheckGameState(GameState.GAME_END);
			}
			return false;
		}
		public GameState GetGameState()
		{
			return gameState;
		}
		public bool CheckGameState(GameState _gameState)
		{
			return gameState == _gameState;
		}
		public int GetSetPlayTeamNo()
		{
			return setPlayTeamNo;
		}
		public float GetGameTime()
		{
			return gameTime;
		}
		public GoalData[] GetGoalDataListTeam(int _teamNo)
		{
			if (_teamNo == 0)
			{
				return goalDataListTeam0.ToArray();
			}
			return goalDataListTeam1.ToArray();
		}
		public bool IsAutoMove()
		{
			return isAutoMove;
		}
		public bool IsStopChara()
		{
			return isStopChara;
		}
		public void SetSetPlayTeamNo(int _teamNo)
		{
			setPlayTeamNo = _teamNo;
		}
	}
}
