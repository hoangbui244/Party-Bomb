using SaveDataDefine;
using System.Collections.Generic;
using UnityEngine;
public class IceHockeyGameManager : SingletonCustom<IceHockeyGameManager>
{
	public enum State
	{
		StartWait,
		FaceOff,
		InGame,
		GoalWait,
		PeriodWait,
		EndGame,
		Result
	}
	[SerializeField]
	[Header("リザルト")]
	private WinOrLoseResultManager result;
	private State currentState;
	private float currentGameTime;
	private int periodNum;
	private int[] arrayGameScore = new int[2];
	public State CurrentState => currentState;
	public void Init()
	{
		SetState(State.StartWait);
		currentGameTime = IceHockey_Define.GAME_TIME;
		periodNum = 0;
		for (int i = 0; i < arrayGameScore.Length; i++)
		{
			arrayGameScore[i] = 0;
		}
		SingletonCustom<IceHockeyPuck>.Instance.SetFaceOff();
	}
	public void OnGameStart()
	{
		SingletonCustom<IceHockeyUIManager>.Instance.ShowPeried(periodNum, delegate
		{
			SingletonCustom<IceHockeyUIManager>.Instance.ShowFaceOff(0f, delegate
			{
				SetState(State.InGame);
			});
		});
	}
	public void OnFaceOff()
	{
		SetState(State.InGame);
		SingletonCustom<IceHockeyPlayerManager>.Instance.OnFaceOff();
	}
	public void OnGoal(int _teamNo)
	{
		SetState(State.GoalWait);
		arrayGameScore[_teamNo]++;
		SingletonCustom<IceHockeyUIManager>.Instance.SetScore(arrayGameScore);
		WaitAfterExec(3.5f, delegate
		{
			SingletonCustom<IceHockeyUIManager>.Instance.FadePrevFrame(0.5f, 0.05f, 0f, delegate
			{
				SingletonCustom<IceHockeyPuck>.Instance.SetFaceOff();
				SingletonCustom<IceHockeyPlayerManager>.Instance.SetFaceOff();
				SingletonCustom<IceHockeyUIManager>.Instance.ShowFaceOff(0f, delegate
				{
					SetState(State.InGame);
				});
			});
		});
	}
	public void UpdateMethod()
	{
		State state = currentState;
		if (state == State.InGame)
		{
			currentGameTime = Mathf.Clamp(currentGameTime - Time.deltaTime, 0f, IceHockey_Define.GAME_TIME);
			if (currentGameTime <= 0f)
			{
				SetState(State.PeriodWait);
				SingletonCustom<IceHockeyUIManager>.Instance.ShowPeriedEnd(periodNum, delegate
				{
					periodNum++;
					if (periodNum == 3)
					{
						SetState(State.Result);
						ResultGameDataParams.SetPoint();
						List<int> list = new List<int>();
						List<int> list2 = new List<int>();
						for (int i = 0; i < SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamA.Length; i++)
						{
							if (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamA[i].PlayerIdx < GS_Define.PLAYER_MAX)
							{
								if (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamA[i].IsCpu)
								{
									list.Add(4 + (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamA[i].PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum));
								}
								else
								{
									list.Add(SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamA[i].PlayerIdx);
								}
							}
						}
						for (int j = 0; j < SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamB.Length; j++)
						{
							if (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamB[j].PlayerIdx < GS_Define.PLAYER_MAX)
							{
								if (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamB[j].IsCpu)
								{
									list2.Add(4 + (SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamB[j].PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum));
								}
								else
								{
									list2.Add(SingletonCustom<IceHockeyPlayerManager>.Instance.ArrayPlayerTeamB[j].PlayerIdx);
								}
							}
						}
						list.Sort();
						list2.Sort();
						result.SetTeamPlayerGroupList(list, list2);
						ResultGameDataParams.SetRecord_WinOrLose(arrayGameScore[0]);
						ResultGameDataParams.SetRecord_WinOrLose(arrayGameScore[1], 1);
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
									SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BLOCK_WIPER);
									break;
								case SystemData.AiStrength.NORAML:
									SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BLOCK_WIPER);
									break;
								case SystemData.AiStrength.STRONG:
									SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BLOCK_WIPER);
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
					}
					else
					{
						SetState(State.PeriodWait);
						SingletonCustom<IceHockeyUIManager>.Instance.FadePrevFrame(0.5f, 0.05f, 0f, delegate
						{
							SingletonCustom<IceHockeyPuck>.Instance.SetFaceOff();
							SingletonCustom<IceHockeyPlayerManager>.Instance.SetFaceOff();
							currentGameTime = IceHockey_Define.GAME_TIME;
							SingletonCustom<IceHockeyUIManager>.Instance.SetTime(currentGameTime);
							SetState(State.FaceOff);
							SingletonCustom<IceHockeyUIManager>.Instance.ShowPeried(periodNum, delegate
							{
								SingletonCustom<IceHockeyUIManager>.Instance.ShowFaceOff(0f, delegate
								{
									SetState(State.InGame);
								});
							});
						});
					}
				});
			}
			SingletonCustom<IceHockeyUIManager>.Instance.SetTime(currentGameTime);
		}
	}
	private void SetState(State _state)
	{
		currentState = _state;
	}
}
