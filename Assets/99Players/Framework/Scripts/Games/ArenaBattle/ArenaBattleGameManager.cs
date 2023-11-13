using SaveDataDefine;
using System;
using UnityEngine;
public class ArenaBattleGameManager : SingletonCustom<ArenaBattleGameManager>
{
	public enum State
	{
		StartWait,
		InGame,
		EndGame,
		Result
	}
	[SerializeField]
	[Header("時間")]
	private CommonGameTimeUI_Font_Time time;
	[SerializeField]
	[Header("勝敗：順位")]
	private RankingResultManager rankingResult;
	private State currentState;
	private float waitTime;
	private float gameTime;
	private float[] arrayFallTime = new float[4];
	public State CurrentState => currentState;
	public float GameTime => gameTime;
	public void Init()
	{
		SetState(State.StartWait);
		gameTime = 0f;
	}
	public void OnGameStart()
	{
		SingletonCustom<ArenaBattlePlayerManager>.Instance.Appearance();
		LeanTween.delayedCall(3f, (Action)delegate
		{
			SingletonCustom<ArenaBattleFieldManager>.Instance.PlayDrawBridge();
			SingletonCustom<CommonStartSimple>.Instance.Show(delegate
			{
				SetState(State.InGame);
			});
		});
	}
	private void SetState(State _state)
	{
		currentState = _state;
	}
	public void UpdateMethod()
	{
		switch (currentState)
		{
		case State.InGame:
			if (SingletonCustom<ArenaBattlePlayerManager>.Instance.IsGameEnd())
			{
				SingletonCustom<ArenaBattlePlayerManager>.Instance.SetLivePlayerTime();
				waitTime = 3f;
				SetState(State.EndGame);
			}
			else if ((SingletonCustom<ArenaBattlePlayerManager>.Instance.LivePlayerCnt <= 2 || gameTime >= 30f) && !SingletonCustom<ArenaBattleFieldManager>.Instance.IsWallLimit)
			{
				SingletonCustom<ArenaBattleFieldManager>.Instance.StartWallLimit();
			}
			gameTime = Mathf.Clamp(gameTime + Time.deltaTime, 0f, 599.99f);
			time.SetTime(CalcManager.ConvertDecimalSecond(gameTime));
			break;
		case State.EndGame:
			waitTime -= Time.deltaTime;
			if (waitTime <= 0f)
			{
				SingletonCustom<CommonEndSimple>.Instance.Show(delegate
				{
					ToResult();
				});
				SetState(State.Result);
			}
			break;
		}
	}
	public void SetTime(int _playerNo, float _addTime = 0f)
	{
		UnityEngine.Debug.Log("_p:" + _playerNo.ToString() + " time:" + _addTime.ToString());
		if (_addTime == -999f)
		{
			arrayFallTime[_playerNo] = -1f;
		}
		else
		{
			arrayFallTime[_playerNo] = CalcManager.ConvertDecimalSecond(gameTime + _addTime);
		}
		UnityEngine.Debug.Log("fall:" + arrayFallTime[_playerNo].ToString());
		CalcManager.ConvertTimeToRecordString(arrayFallTime[_playerNo], _playerNo);
	}
	private void ToResult()
	{
		ResultGameDataParams.SetPoint();
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1 && arrayFallTime[0] == -1f)
		{
			switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
			{
			case SystemData.AiStrength.WEAK:
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BLOW_AWAY_TANK);
				break;
			case SystemData.AiStrength.NORAML:
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BLOW_AWAY_TANK);
				break;
			case SystemData.AiStrength.STRONG:
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BLOW_AWAY_TANK);
				break;
			}
		}
		int[] array = new int[arrayFallTime.Length];
		for (int i = 0; i < arrayFallTime.Length; i++)
		{
			ArenaBattlePlayer playerAtIdx = SingletonCustom<ArenaBattlePlayerManager>.Instance.GetPlayerAtIdx(i);
			array[i] = (playerAtIdx.IsCpu ? (4 + (playerAtIdx.PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : playerAtIdx.PlayerIdx);
			CalcManager.ConvertTimeToRecordString(arrayFallTime[i], array[i]);
		}
		ResultGameDataParams.SetRecord_Float(arrayFallTime, array);
		rankingResult.ShowResult_Time();
	}
}
