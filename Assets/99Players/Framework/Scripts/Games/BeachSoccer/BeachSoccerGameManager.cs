using SaveDataDefine;
using System.Collections.Generic;
using UnityEngine;
public class BeachSoccerGameManager : SingletonCustom<BeachSoccerGameManager>
{
	public enum State
	{
		StartWait,
		KickOff,
		InGame,
		GoalWait,
		ThrowInWait,
		CornerKickWait,
		GoalClearanceWait,
		PeriodWait,
		EndGame,
		Result
	}
	[SerializeField]
	[Header("リザルト")]
	private WinOrLoseResultManager result;
	[SerializeField]
	[Header("3Dル\u30fcト")]
	private Transform root3d;
	private int startKickOffTeamNo;
	private int startThrowInTeamNo;
	private int startCornerKickTeamNo;
	private int startPeriodKickOffTeamNo;
	private State currentState;
	private float currentGameTime;
	private int periodNum;
	private int[] arrayGameScore = new int[2];
	public State CurrentState => currentState;
	public int StartKickOffTeamNo => startKickOffTeamNo;
	public int StartThrowInTeamNo => startThrowInTeamNo;
	public int StartCornerKickTeamNo => startCornerKickTeamNo;
	public Transform Root3D => root3d;
	public void Init()
	{
		SetState(State.StartWait);
		currentGameTime = BeachSoccerDefine.GAME_TIME;
		periodNum = 0;
		for (int i = 0; i < arrayGameScore.Length; i++)
		{
			arrayGameScore[i] = 0;
		}
		startKickOffTeamNo = Random.Range(0, 2);
		startPeriodKickOffTeamNo = startKickOffTeamNo;
		SingletonCustom<BeachSoccerBall>.Instance.SetKickOff();
	}
	public void OnGameStart()
	{
		SingletonCustom<BeachSoccerUIManager>.Instance.ShowPeried(periodNum, delegate
		{
			SingletonCustom<BeachSoccerUIManager>.Instance.ShowKickOff(0f, delegate
			{
				SetState(State.InGame);
			});
		});
	}
	public void OnKickOff()
	{
		SetState(State.InGame);
		SingletonCustom<BeachSoccerPlayerManager>.Instance.OnKickOff();
		SingletonCustom<BeachSoccerCameraMover>.Instance.SetState(BeachSoccerCameraMover.State.BALL);
		SingletonCustom<BeachSoccerBall>.Instance.OnKickOff();
	}
	public void OnGoal(int _teamNo)
	{
		SetState(State.GoalWait);
		arrayGameScore[_teamNo]++;
		SingletonCustom<BeachSoccerUIManager>.Instance.SetScore(arrayGameScore);
		startKickOffTeamNo = ((_teamNo == 0) ? 1 : 0);
		WaitAfterExec(3.5f, delegate
		{
			SingletonCustom<BeachSoccerUIManager>.Instance.FadePrevFrame(0.5f, 0.05f, 0f, delegate
			{
				SingletonCustom<BeachSoccerBall>.Instance.SetKickOff();
				SingletonCustom<BeachSoccerPlayerManager>.Instance.SetKickOff();
				SingletonCustom<BeachSoccerUIManager>.Instance.ShowKickOff(0f, delegate
				{
					SetState(State.InGame);
				});
			});
		});
	}
	public void OnThrowIn(int _teamNo)
	{
		SetState(State.ThrowInWait);
		startThrowInTeamNo = ((_teamNo == 0) ? 1 : 0);
		WaitAfterExec(1.5f, delegate
		{
			SingletonCustom<BeachSoccerUIManager>.Instance.FadePrevFrame(0.5f, 0.05f, 0f, delegate
			{
				SingletonCustom<BeachSoccerPlayerManager>.Instance.SetThrowIn();
				SingletonCustom<BeachSoccerBall>.Instance.SetThrowIn();
				SetState(State.InGame);
			});
		});
	}
	public void OnCornerKick(int _teamNo)
	{
		SetState(State.CornerKickWait);
		startCornerKickTeamNo = _teamNo;
		WaitAfterExec(1.5f, delegate
		{
			SingletonCustom<BeachSoccerUIManager>.Instance.FadePrevFrame(0.5f, 0.05f, 0f, delegate
			{
				SingletonCustom<BeachSoccerPlayerManager>.Instance.SetCornerKick();
				SingletonCustom<BeachSoccerBall>.Instance.SetCornerKick();
				SetState(State.InGame);
			});
		});
	}
	public void OnGoalClearance(int _teamNo)
	{
		SetState(State.GoalClearanceWait);
		startThrowInTeamNo = _teamNo;
		WaitAfterExec(1.5f, delegate
		{
			SingletonCustom<BeachSoccerUIManager>.Instance.FadePrevFrame(0.5f, 0.05f, 0f, delegate
			{
				SingletonCustom<BeachSoccerPlayerManager>.Instance.SetGoalClearance();
				SetState(State.InGame);
			});
		});
	}
	public void UpdateMethod()
	{
		State state = currentState;
		if (state == State.InGame && SingletonCustom<BeachSoccerBall>.Instance.CurrentState != BeachSoccerBall.State.KickOff)
		{
			currentGameTime = Mathf.Clamp(currentGameTime - Time.deltaTime, 0f, BeachSoccerDefine.GAME_TIME);
			if (currentGameTime <= 0f && SingletonCustom<BeachSoccerBall>.Instance.CurrentState == BeachSoccerBall.State.Default)
			{
				SetState(State.PeriodWait);
				SingletonCustom<BeachSoccerUIManager>.Instance.ShowPeriedEnd(periodNum, delegate
				{
					periodNum++;
					if (periodNum == 3)
					{
						SetState(State.Result);
						ToResult();
					}
					else
					{
						SetState(State.PeriodWait);
						SingletonCustom<BeachSoccerUIManager>.Instance.FadePrevFrame(0.5f, 0.05f, 0f, delegate
						{
							switch (periodNum)
							{
							case 1:
								startKickOffTeamNo = ((startPeriodKickOffTeamNo == 0) ? 1 : 0);
								break;
							case 2:
								startKickOffTeamNo = Random.Range(0, 2);
								break;
							}
							SingletonCustom<BeachSoccerBall>.Instance.SetKickOff();
							SingletonCustom<BeachSoccerPlayerManager>.Instance.SetKickOff();
							currentGameTime = BeachSoccerDefine.GAME_TIME;
							SingletonCustom<BeachSoccerUIManager>.Instance.SetTime(currentGameTime);
							SetState(State.KickOff);
							SingletonCustom<BeachSoccerUIManager>.Instance.ShowPeried(periodNum, delegate
							{
								SingletonCustom<BeachSoccerUIManager>.Instance.ShowKickOff(0f, delegate
								{
									SetState(State.InGame);
								});
							});
						});
					}
				});
			}
			SingletonCustom<BeachSoccerUIManager>.Instance.SetTime(currentGameTime);
		}
	}
	private void ToResult()
	{
		ResultGameDataParams.SetPoint();
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayPlayerTeamA.Length; i++)
		{
			if (SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayPlayerTeamA[i].PlayerIdx < GS_Define.PLAYER_MAX)
			{
				if (SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayPlayerTeamA[i].IsCpu)
				{
					list.Add(4 + (SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayPlayerTeamA[i].PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum));
				}
				else
				{
					list.Add(SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayPlayerTeamA[i].PlayerIdx);
				}
			}
		}
		for (int j = 0; j < SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayPlayerTeamB.Length; j++)
		{
			if (SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayPlayerTeamB[j].PlayerIdx < GS_Define.PLAYER_MAX)
			{
				if (SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayPlayerTeamB[j].IsCpu)
				{
					list2.Add(4 + (SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayPlayerTeamB[j].PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum));
				}
				else
				{
					list2.Add(SingletonCustom<BeachSoccerPlayerManager>.Instance.ArrayPlayerTeamB[j].PlayerIdx);
				}
			}
		}
		list.Sort();
		list2.Sort();
		result.SetTeamPlayerGroupList(list, list2);
		ResultGameDataParams.SetRecord_WinOrLose(arrayGameScore[0]);
		ResultGameDataParams.SetRecord_WinOrLose(arrayGameScore[1], 1);
		GS_Define.GameFormat selectGameFormat = SingletonCustom<GameSettingManager>.Instance.SelectGameFormat;
		SingletonCustom<GameSettingManager>.Instance.SelectGameFormat = GS_Define.GameFormat.BATTLE_AND_COOP;
		if (arrayGameScore[0] == arrayGameScore[1])
		{
			result.ShowResult(ResultGameDataParams.ResultType.Draw, -1);
		}
		else if (arrayGameScore[0] > arrayGameScore[1])
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
			{
				switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting())
				{
				case SystemData.AiStrength.WEAK:
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.ATTACK_BALL);
					break;
				case SystemData.AiStrength.NORAML:
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.ATTACK_BALL);
					break;
				case SystemData.AiStrength.STRONG:
					SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.ATTACK_BALL);
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
	private void SetState(State _state)
	{
		currentState = _state;
	}
}
