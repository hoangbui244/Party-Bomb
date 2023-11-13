using System;
using UnityEngine;
public class Golf_GameManager : SingletonCustom<Golf_GameManager>
{
	public enum State
	{
		PLAY_START,
		RESET_SHOT_READY,
		SHOT_READY,
		SHOT_POWER,
		SHOT_IMPACT,
		SWING_ANIMATION,
		BALL_FLY,
		VIEW_CUP_ROT_CAMERA,
		VIEW_CUP_LINE,
		CALC_POINT,
		CHANGE_TURN,
		PLAY_END
	}
	[SerializeField]
	[Header("勝敗リザルト")]
	private RankingResultManager rankingResult;
	private State state;
	[SerializeField]
	[Header("カップとの距離に応じた最大ポイント")]
	private int DUSTABCE_TO_CUP_MAX_POINT;
	[SerializeField]
	[Header("カップインした時のボ\u30fcナスポイント")]
	private int CUP_IN_BOUNUS;
	[SerializeField]
	[Header("ニアピンボ\u30fcナスを加算する距離")]
	private float NEAR_PIN_BOUNUS_DISTANCE;
	[SerializeField]
	[Header("ニアピンボ\u30fcナス")]
	private int NEAR_PIN_BOUNUS;
	private bool isGameStart;
	private bool isGameEnd;
	private int gameCnt;
	private int[] arrayOrderOfPlay;
	private bool isSkip;
	private readonly float WAIT_UPDATE_TIME = 0.5f;
	private bool isWaitUpdate;
	public void Init()
	{
		SetState(State.PLAY_START);
		arrayOrderOfPlay = new int[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
		for (int i = 0; i < arrayOrderOfPlay.Length; i++)
		{
			arrayOrderOfPlay[i] = i;
		}
		arrayOrderOfPlay.Shuffle();
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length)
		{
			bool flag = false;
			for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerNum; j++)
			{
				if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[arrayOrderOfPlay[j]][0] < 4)
				{
					continue;
				}
				for (int k = j + 1; k < arrayOrderOfPlay.Length; k++)
				{
					if (SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[arrayOrderOfPlay[k]][0] < 4)
					{
						int num = arrayOrderOfPlay[j];
						arrayOrderOfPlay[j] = arrayOrderOfPlay[k];
						arrayOrderOfPlay[k] = num;
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		SingletonCustom<Golf_PlayerManager>.Instance.SetTurnPlayer();
		SingletonCustom<Golf_UIManager>.Instance.SetGameCnt(gameCnt);
	}
	public void InitPlay(bool _isInit = false)
	{
		SingletonCustom<Golf_CursorManager>.Instance.Show(SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().GetPlayerNo());
		SingletonCustom<Golf_PlayerManager>.Instance.SetTurnPlayer();
		SingletonCustom<Golf_CursorManager>.Instance.InitPlay();
		SingletonCustom<Golf_CursorManager>.Instance.Hide(SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().GetPlayerNo());
		SingletonCustom<Golf_WindManager>.Instance.InitPlay();
		SingletonCustom<Golf_FieldManager>.Instance.InitPlay();
		SingletonCustom<Golf_PlayerManager>.Instance.InitPlay();
		SingletonCustom<Golf_BallManager>.Instance.InitPlay();
		if (SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().GetIsCpu())
		{
			SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().InitPlayCPU();
		}
		SingletonCustom<Golf_CameraManager>.Instance.InitPlay();
		SingletonCustom<Golf_ViewCupLineManager>.Instance.InitPlay();
		SingletonCustom<Golf_UIManager>.Instance.InitPlay();
		SingletonCustom<Golf_UIManager>.Instance.SetGameCnt(gameCnt);
		SingletonCustom<Golf_UIManager>.Instance.SetControllerExplanationActive(Golf_UIManager.ContorollerType.Shot_Ready_Dir);
		isSkip = false;
	}
	public void CalcPoint()
	{
		SetState(State.CALC_POINT);
		float remainingDistanceToCup = 0f;
		int addPoint = 0;
		Golf_Ball ball = SingletonCustom<Golf_BallManager>.Instance.GetTurnPlayerBall();
		if (ball.GetIsCupIn())
		{
			remainingDistanceToCup = 0f;
			addPoint = DUSTABCE_TO_CUP_MAX_POINT + CUP_IN_BOUNUS;
		}
		else if (!ball.GetIsOB())
		{
			remainingDistanceToCup = SingletonCustom<Golf_BallManager>.Instance.GetRemainingDistanceToCup();
			UnityEngine.Debug.Log("remainingDistanceToCup " + remainingDistanceToCup.ToString());
			float num = 1f - remainingDistanceToCup / SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetReadyBallPosToCupDistance();
			UnityEngine.Debug.Log("lerp " + num.ToString());
			addPoint = (int)((float)DUSTABCE_TO_CUP_MAX_POINT * num);
			if (remainingDistanceToCup <= NEAR_PIN_BOUNUS_DISTANCE)
			{
				addPoint += ((int)NEAR_PIN_BOUNUS_DISTANCE - (int)remainingDistanceToCup) * NEAR_PIN_BOUNUS;
			}
		}
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			SingletonCustom<Golf_UIManager>.Instance.HideViewCupYard();
			SingletonCustom<Golf_UIManager>.Instance.ShowPointResultUI(remainingDistanceToCup, addPoint);
			LeanTween.delayedCall(base.gameObject, SingletonCustom<Golf_UIManager>.Instance.GetPointResultViewTime() + 0.5f, (Action)delegate
			{
				if (!ball.GetIsOB())
				{
					Golf_Player player = SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer();
					int point = player.GetPoint();
					float time = 1f;
					LeanTween.value(base.gameObject, point, point + addPoint, 0.5f).setOnUpdate(delegate(float _value)
					{
						time += Time.deltaTime;
						if (time > 0.1f)
						{
							time = 0f;
							SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
						}
						SingletonCustom<Golf_UIManager>.Instance.SetPoint(player.GetPlayerNo(), (int)_value);
					});
					player.AddPoint(addPoint);
				}
				LeanTween.delayedCall(base.gameObject, 1.5f, (Action)delegate
				{
					ChangeTurn();
				});
			});
		});
	}
	public void ChangeTurn()
	{
		SetState(State.CHANGE_TURN);
		gameCnt++;
		if (gameCnt == Golf_Define.TOTAL_GAME_CNT)
		{
			SetState(State.PLAY_END);
			GameEnd();
		}
		else
		{
			SingletonCustom<Golf_PlayerManager>.Instance.ResetAnimation();
			SingletonCustom<Golf_UIManager>.Instance.StartScreenFade(delegate
			{
				InitPlay();
			}, delegate
			{
				ShowTurnCutIn();
			});
		}
	}
	private void ShowTurnCutIn()
	{
		Golf_Player turnPlayer = SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer();
		if (!turnPlayer.GetIsCpu())
		{
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(turnPlayer.GetPlayerNo());
			SingletonCustom<Golf_UIManager>.Instance.ShowTurnCutIn(turnPlayer.GetPlayerNo(), 0f, delegate
			{
				SetState(State.SHOT_READY);
			});
		}
		else
		{
			SetState(State.SHOT_READY);
		}
	}
	public int GetIndexOfOrderOfPlay(int _idx)
	{
		return Array.IndexOf(arrayOrderOfPlay, _idx);
	}
	public int GetTurnPlayerOrderOfPlay()
	{
		return arrayOrderOfPlay[gameCnt % SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
	}
	public int GetGameCnt()
	{
		return gameCnt;
	}
	public State GetState()
	{
		return state;
	}
	public void SetState(State _state)
	{
		UnityEngine.Debug.Log("次の遷移 : " + _state.ToString());
		state = _state;
		switch (state)
		{
		case State.SHOT_IMPACT:
		case State.SWING_ANIMATION:
			break;
		case State.RESET_SHOT_READY:
			if (!SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().GetIsCpu())
			{
				SingletonCustom<Golf_UIManager>.Instance.SetControllerBalloonActive(Golf_UIManager.ContorollerType.Shot_Power_Imact, _isFade: false, _isActive: false);
				SingletonCustom<Golf_UIManager>.Instance.SetControllerBalloonActive(Golf_UIManager.ContorollerType.Shot_Ready_Dir, _isFade: true, _isActive: true);
			}
			SingletonCustom<Golf_UIManager>.Instance.ResetShotReadyInitPlay();
			state = State.SHOT_READY;
			break;
		case State.SHOT_READY:
			SingletonCustom<Golf_BallManager>.Instance.InitPlayPredictionBallLine();
			SingletonCustom<Golf_UIManager>.Instance.ShowFlagUI();
			if (!SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().GetIsCpu())
			{
				SingletonCustom<Golf_UIManager>.Instance.SetControllerBalloonActive(Golf_UIManager.ContorollerType.Shot_Ready_Dir, _isFade: true, _isActive: true);
			}
			else
			{
				SingletonCustom<Golf_UIManager>.Instance.SetControllerBalloonActive(Golf_UIManager.ContorollerType.Skip, _isFade: true, _isActive: true);
			}
			break;
		case State.SHOT_POWER:
			isWaitUpdate = true;
			LeanTween.delayedCall(base.gameObject, SingletonCustom<Golf_UIManager>.Instance.GetUIMoveTime() + WAIT_UPDATE_TIME, (Action)delegate
			{
				isWaitUpdate = false;
			});
			SingletonCustom<Golf_UIManager>.Instance.ShowHitPoint();
			SingletonCustom<Golf_UIManager>.Instance.ShowGauge();
			if (!SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().GetIsCpu())
			{
				SingletonCustom<Golf_UIManager>.Instance.SetControllerBalloonActive(Golf_UIManager.ContorollerType.Shot_Ready_Dir, _isFade: false, _isActive: false);
				SingletonCustom<Golf_UIManager>.Instance.SetControllerBalloonActive(Golf_UIManager.ContorollerType.Shot_Power_Imact, _isFade: true, _isActive: true);
				SingletonCustom<Golf_UIManager>.Instance.SetControllerExplanationActive(Golf_UIManager.ContorollerType.Shot_Power_Imact);
			}
			break;
		case State.BALL_FLY:
			SingletonCustom<Golf_BallManager>.Instance.HidePredictionBallLine();
			SingletonCustom<Golf_UIManager>.Instance.HideHitPoint();
			SingletonCustom<Golf_UIManager>.Instance.HideGauge();
			SingletonCustom<Golf_UIManager>.Instance.HideDistanceToCupUI();
			SingletonCustom<Golf_UIManager>.Instance.HideFlagUI();
			if (!SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().GetIsCpu())
			{
				SingletonCustom<Golf_UIManager>.Instance.SetControllerBalloonActive(Golf_UIManager.ContorollerType.Shot_Power_Imact, _isFade: false, _isActive: false);
			}
			else
			{
				SingletonCustom<Golf_UIManager>.Instance.SetControllerBalloonActive(Golf_UIManager.ContorollerType.Skip, _isFade: false, _isActive: false);
			}
			break;
		case State.VIEW_CUP_ROT_CAMERA:
			SingletonCustom<Golf_CameraManager>.Instance.SetViewCupRotCamera();
			break;
		case State.VIEW_CUP_LINE:
			SingletonCustom<Golf_BallManager>.Instance.SetRemainingDistanceToCup();
			SingletonCustom<Golf_ViewCupLineManager>.Instance.Show();
			SingletonCustom<Golf_UIManager>.Instance.ShowViewCupYard();
			break;
		}
	}
	public bool GetIsSkip()
	{
		return isSkip;
	}
	public void SetIsSkip(bool _isSkip)
	{
		isSkip = _isSkip;
	}
	public bool GetIsWaitUpdate()
	{
		return isWaitUpdate;
	}
	public bool GetIsGameStart()
	{
		return isGameStart;
	}
	public void GameStart()
	{
		SingletonCustom<Golf_UIManager>.Instance.ShowAnnounceUI(delegate
		{
			isGameStart = true;
			ShowTurnCutIn();
		});
	}
	public bool GetIsGameEnd()
	{
		return isGameEnd;
	}
	private void GameEnd(bool _isAutoGameEnd = false)
	{
		UnityEngine.Debug.Log("ゲ\u30fcム終了処理");
		isGameEnd = true;
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
			{
				SingletonCustom<Golf_UIManager>.Instance.HideUI();
			}
			SingletonCustom<CommonEndSimple>.Instance.Show(delegate
			{
				if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
				{
					int point = SingletonCustom<Golf_PlayerManager>.Instance.GetPlayer(0).GetPoint();
					if ((float)point != -1f)
					{
						if (point >= Golf_Define.BRONZE_POINT)
						{
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BOMB_ROULETTE);
						}
						if (point >= Golf_Define.SILVER_POINT)
						{
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BOMB_ROULETTE);
						}
						if (point >= Golf_Define.GOLD_POINT)
						{
							SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BOMB_ROULETTE);
						}
					}
				}
				ResultGameDataParams.SetPoint();
				int[] array = new int[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
				int[] array2 = new int[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
				for (int j = 0; j < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; j++)
				{
					Golf_Player player = SingletonCustom<Golf_PlayerManager>.Instance.GetPlayer(j);
					array[j] = player.GetPoint();
					array2[j] = (int)player.GetUserType();
				}
				ResultGameDataParams.SetRecord_Int(array, array2);
				rankingResult.ShowResult_Score();
			});
		});
	}
	public void DebugGameEnd()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			SingletonCustom<Golf_PlayerManager>.Instance.GetPlayer(i).SetPoint(UnityEngine.Random.Range(2500, 4000));
		}
		GameEnd();
	}
}
