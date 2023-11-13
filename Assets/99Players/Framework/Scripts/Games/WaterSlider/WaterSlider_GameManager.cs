using System;
using System.Collections.Generic;
using UnityEngine;
public class WaterSlider_GameManager : SingletonCustom<WaterSlider_GameManager>
{
	[Serializable]
	public class RankData
	{
		public int playerNo;
		public int checkPointIdx;
		public float distance;
		public RankData(int _playerNo, int _idx, float _distance)
		{
			playerNo = _playerNo;
			checkPointIdx = _idx;
			distance = _distance;
		}
	}
	public enum State
	{
		NONE,
		IN_PLAY,
		GOAL_WAIT,
		RESULT
	}
	[SerializeField]
	[Header("コ\u30fcス配列")]
	private GameObject[] arrayCourse;
	[SerializeField]
	[Header("キャラ配列")]
	private WaterSlider_Character[] arrayCharacter;
	[SerializeField]
	[Header("リザルト【順位】")]
	private RankingResultManager rankingResult;
	[SerializeField]
	[Header("フェ\u30fcド")]
	private SpriteRenderer fade;
	[SerializeField]
	[Header("組数表示")]
	private GameObject[] arrayObjSetNumber;
	[SerializeField]
	[Header("キャラクタ\u30fc")]
	private Transform[] chara;
	[SerializeField]
	[Header("浮き輪")]
	private Transform[] charaFloat;
	private float gameTime;
	private int setCnt;
	private int[] playerTeamAssignment;
	private float[] arrayGoalTime = new float[9];
	private List<int> firstSetPlayer = new List<int>();
	private List<int> secondSetPlayer = new List<int>();
	private bool isAllCharacterGoal;
	private bool isAllPlayerGoal;
	private int remainPlayer;
	private float endTransWait;
	private float goalTransWait;
	private int goalCharacterCnt;
	private int goalDispCnt;
	private RankData[] listRankCheck;
	private bool isSecondRun;
	private bool waterOnePlay = true;
	private State currentState;
	private Rect[][] cameraRect = new Rect[4][]
	{
		new Rect[1]
		{
			new Rect(0f, 0f, 1f, 1f)
		},
		new Rect[2]
		{
			new Rect(0f, 0f, 0.5f, 1f),
			new Rect(0.5f, 0f, 1f, 1f)
		},
		new Rect[4]
		{
			new Rect(0f, 0.5f, 0.5f, 1f),
			new Rect(0.5f, 0.5f, 1f, 1f),
			new Rect(0f, 0f, 0.5f, 0.5f),
			new Rect(0.5f, 0f, 1f, 0.5f)
		},
		new Rect[4]
		{
			new Rect(0f, 0.5f, 0.5f, 1f),
			new Rect(0.5f, 0.5f, 1f, 1f),
			new Rect(0f, 0f, 0.5f, 0.5f),
			new Rect(0.5f, 0f, 1f, 0.5f)
		}
	};
	public State CurrentState => currentState;
	public int GoalCharacterCnt => goalCharacterCnt;
	private void SetCurrentState(State _state)
	{
		currentState = _state;
	}
	public void Init()
	{
		for (int i = 0; i < arrayCharacter.Length; i++)
		{
			arrayCharacter[i].SetArrayIdx(i);
			SetLayer(i, LayerMask.NameToLayer("Collision_Obj_" + (i + 1).ToString()));
		}
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum >= 2)
		{
			playerTeamAssignment = new int[SingletonCustom<GameSettingManager>.Instance.PlayerNum];
			for (int j = 0; j < playerTeamAssignment.Length; j++)
			{
				playerTeamAssignment[j] = j;
			}
			System.Random random = new System.Random();
			int num = playerTeamAssignment.Length;
			while (num > 1)
			{
				num--;
				int num2 = random.Next(num + 1);
				int num3 = playerTeamAssignment[num2];
				playerTeamAssignment[num2] = playerTeamAssignment[num];
				playerTeamAssignment[num] = num3;
			}
		}
		SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
			arrayCharacter[0].Init(4);
			arrayCharacter[1].Init(0);
			arrayCharacter[2].Init(5);
			arrayCharacter[3].Init(6);
			firstSetPlayer.Add(4);
			firstSetPlayer.Add(0);
			firstSetPlayer.Add(5);
			firstSetPlayer.Add(6);
			break;
		case 2:
			arrayCharacter[0].Init(4);
			arrayCharacter[1].Init(playerTeamAssignment[0]);
			arrayCharacter[2].Init(playerTeamAssignment[1]);
			arrayCharacter[3].Init(5);
			firstSetPlayer.Add(4);
			firstSetPlayer.Add(playerTeamAssignment[0]);
			firstSetPlayer.Add(playerTeamAssignment[1]);
			firstSetPlayer.Add(5);
			break;
		case 3:
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
			{
				arrayObjSetNumber[0].SetActive(value: true);
				arrayObjSetNumber[0].transform.SetLocalPositionY(700f);
				LeanTween.cancel(arrayObjSetNumber[0]);
				LeanTween.moveLocalY(arrayObjSetNumber[0], 357f, 1f).setEaseInOutBack().setDelay(0.25f)
					.setOnComplete((Action)delegate
					{
						LeanTween.moveLocalY(arrayObjSetNumber[0], 700f, 1f).setEaseInOutBack().setDelay(1f);
					});
				arrayCharacter[0].Init(5);
				arrayCharacter[1].Init(playerTeamAssignment[0]);
				arrayCharacter[2].Init(playerTeamAssignment[1]);
				arrayCharacter[3].Init(6);
				firstSetPlayer.Add(playerTeamAssignment[0]);
				firstSetPlayer.Add(5);
				firstSetPlayer.Add(playerTeamAssignment[1]);
				firstSetPlayer.Add(6);
			}
			else
			{
				arrayCharacter[0].Init(playerTeamAssignment[0]);
				arrayCharacter[1].Init(playerTeamAssignment[1]);
				arrayCharacter[2].Init(playerTeamAssignment[2]);
				arrayCharacter[3].Init(4);
				firstSetPlayer.Add(playerTeamAssignment[0]);
				firstSetPlayer.Add(playerTeamAssignment[1]);
				firstSetPlayer.Add(playerTeamAssignment[2]);
				firstSetPlayer.Add(4);
			}
			break;
		case 4:
			if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
			{
				arrayObjSetNumber[0].SetActive(value: true);
				arrayObjSetNumber[0].transform.SetLocalPositionY(700f);
				LeanTween.cancel(arrayObjSetNumber[0]);
				LeanTween.moveLocalY(arrayObjSetNumber[0], 357f, 1f).setEaseInOutBack().setDelay(0.25f)
					.setOnComplete((Action)delegate
					{
						LeanTween.moveLocalY(arrayObjSetNumber[0], 700f, 1f).setEaseInOutBack().setDelay(1f);
					});
				arrayCharacter[0].Init(4);
				arrayCharacter[1].Init(playerTeamAssignment[0]);
				arrayCharacter[2].Init(playerTeamAssignment[1]);
				arrayCharacter[3].Init(5);
				firstSetPlayer.Add(playerTeamAssignment[0]);
				firstSetPlayer.Add(4);
				firstSetPlayer.Add(playerTeamAssignment[1]);
				firstSetPlayer.Add(5);
			}
			else
			{
				arrayCharacter[0].Init(playerTeamAssignment[0]);
				arrayCharacter[1].Init(playerTeamAssignment[1]);
				arrayCharacter[2].Init(playerTeamAssignment[2]);
				arrayCharacter[3].Init(playerTeamAssignment[3]);
				firstSetPlayer.Add(playerTeamAssignment[0]);
				firstSetPlayer.Add(playerTeamAssignment[1]);
				firstSetPlayer.Add(playerTeamAssignment[2]);
				firstSetPlayer.Add(playerTeamAssignment[3]);
			}
			break;
		}
		int num4 = 0;
		for (int k = 0; k < arrayCharacter.Length; k++)
		{
			if (!arrayCharacter[k].IsCpu)
			{
				switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
				{
				default:
					arrayCharacter[k].SetCameraRect(cameraRect[SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1][arrayCharacter[k].PlayerNo]);
					break;
				case 3:
					if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
					{
						arrayCharacter[k].SetCameraRect(cameraRect[1][num4]);
						num4++;
					}
					else
					{
						arrayCharacter[k].SetCameraRect(cameraRect[SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1][arrayCharacter[k].PlayerNo]);
					}
					break;
				case 4:
					if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
					{
						arrayCharacter[k].SetCameraRect(cameraRect[1][num4]);
						num4++;
					}
					else
					{
						arrayCharacter[k].SetCameraRect(cameraRect[SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1][arrayCharacter[k].PlayerNo]);
					}
					break;
				}
			}
			else if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
			{
				arrayCharacter[k].SetCameraRect(cameraRect[SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1][3]);
			}
		}
		ResultGameDataParams.SetPoint();
		SingletonCustom<WaterSlider_CourseManager>.Instance.Init();
	}
	public void ChangeCourse()
	{
		for (int i = 0; i < arrayCourse.Length; i++)
		{
			arrayCourse[i].SetActive(i == SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx);
		}
	}
	public void GameStart()
	{
		SingletonCustom<CommonStartProduction>.Instance.Play(delegate
		{
			for (int i = 0; i < arrayCharacter.Length; i++)
			{
				arrayCharacter[i].GameStart();
			}
			SetCurrentState(State.IN_PLAY);
			WaitAfterExec(1f, delegate
			{
				SingletonCustom<WaterSlider_UIManager>.Instance.IsCanChangeRank = true;
			});
		});
	}
	public float GetGameTime()
	{
		return gameTime;
	}
	public float OnSetTime(int _playerNo, float _addTime = 0f)
	{
		goalDispCnt = 0;
		if (gameTime >= 599.99f)
		{
			_addTime = 0f;
		}
		if (isSecondRun)
		{
			for (int i = 0; i < secondSetPlayer.Count; i++)
			{
				if (arrayGoalTime[secondSetPlayer[i]] > 0f && arrayGoalTime[secondSetPlayer[i]] < CalcManager.ConvertDecimalSecond(gameTime + _addTime))
				{
					goalDispCnt++;
				}
			}
		}
		else
		{
			for (int j = 0; j < arrayGoalTime.Length; j++)
			{
				if (arrayGoalTime[j] > 0f && arrayGoalTime[j] < CalcManager.ConvertDecimalSecond(gameTime + _addTime))
				{
					goalDispCnt++;
				}
			}
		}
		if (_addTime == -1f)
		{
			arrayGoalTime[_playerNo] = CalcManager.ConvertDecimalSecond(-1f);
		}
		else
		{
			arrayGoalTime[_playerNo] = CalcManager.ConvertDecimalSecond(gameTime + _addTime);
			if (_playerNo < 4 || (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE))
			{
				SingletonCustom<WaterSlider_UIManager>.Instance.SetNumSprite(ConvertIdx(_playerNo), goalDispCnt);
				SingletonCustom<WaterSlider_UIManager>.Instance.CloseRankNum(ConvertIdx(_playerNo));
			}
		}
		UnityEngine.Debug.Log("【OnSetTime】:" + arrayGoalTime[_playerNo].ToString() + " pNo:" + _playerNo.ToString() + " addTime:" + _addTime.ToString());
		CalcManager.ConvertTimeToRecordString(arrayGoalTime[_playerNo], _playerNo);
		for (int k = 0; k < arrayCharacter.Length; k++)
		{
			if (arrayCharacter[k].PlayerNo == _playerNo)
			{
				arrayCharacter[k].GoalRank = goalDispCnt;
				switch (goalDispCnt)
				{
				case 0:
					arrayCharacter[k].SetFaceDiff(StyleTextureManager.MainCharacterFaceType.HAPPY);
					break;
				case 1:
					arrayCharacter[k].SetFaceDiff(StyleTextureManager.MainCharacterFaceType.SMILE);
					break;
				case 2:
					arrayCharacter[k].SetFaceDiff(StyleTextureManager.MainCharacterFaceType.NORMAL);
					break;
				case 3:
					arrayCharacter[k].SetFaceDiff(StyleTextureManager.MainCharacterFaceType.SAD);
					break;
				}
			}
		}
		goalCharacterCnt++;
		return arrayGoalTime[_playerNo];
	}
	private void Update()
	{
		switch (currentState)
		{
		case State.IN_PLAY:
			if (waterOnePlay)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_Waterslider_Water", _loop: true, 0f, 0.7f);
				SingletonCustom<AudioManager>.Instance.SePlay("se_Waterslider_Water", _loop: true, 0f, 0.7f);
				waterOnePlay = false;
			}
			isAllCharacterGoal = true;
			isAllPlayerGoal = true;
			remainPlayer = 0;
			for (int j = 0; j < arrayCharacter.Length; j++)
			{
				if (!arrayCharacter[j].gameObject.activeSelf)
				{
					continue;
				}
				if (arrayCharacter[j].CurrentState != WaterSlider_Character.State.GOAL)
				{
					isAllCharacterGoal = false;
					if (!arrayCharacter[j].IsCpu)
					{
						isAllPlayerGoal = false;
						remainPlayer++;
					}
				}
				if (!arrayCharacter[j].IsCpu)
				{
					if (arrayCharacter[j].IsReverse)
					{
						SingletonCustom<WaterSlider_UIManager>.Instance.ReverseRunON(ConvertIdx(arrayCharacter[j].PlayerNo));
					}
					else
					{
						SingletonCustom<WaterSlider_UIManager>.Instance.ReverseRunOFF(ConvertIdx(arrayCharacter[j].PlayerNo));
					}
				}
			}
			gameTime = Mathf.Clamp(gameTime + Time.deltaTime, 0f, 599.99f);
			if (!isAllCharacterGoal)
			{
				CalcManager.mCalcInt = 0;
				for (int k = 0; k < arrayCharacter.Length; k++)
				{
					if (arrayCharacter[k].gameObject.activeSelf)
					{
						CalcManager.mCalcInt++;
					}
				}
				listRankCheck = new RankData[CalcManager.mCalcInt];
				int num = 0;
				for (int l = 0; l < arrayCharacter.Length; l++)
				{
					if (!arrayCharacter[l].gameObject.activeSelf)
					{
						continue;
					}
					if (!arrayCharacter[l].IsCpu)
					{
						if (arrayCharacter[l].CurrentState == WaterSlider_Character.State.GOAL)
						{
							SingletonCustom<WaterSlider_UIManager>.Instance.SetTime(ConvertIdx(arrayCharacter[l].PlayerNo), arrayCharacter[l].PlayerNo, arrayCharacter[l].GoalTime);
							listRankCheck[num] = new RankData(arrayCharacter[l].PlayerNo, 99, arrayCharacter[l].GoalRank);
						}
						else
						{
							SingletonCustom<WaterSlider_UIManager>.Instance.SetTime(ConvertIdx(arrayCharacter[l].PlayerNo), arrayCharacter[l].PlayerNo, gameTime);
							listRankCheck[num] = new RankData(arrayCharacter[l].PlayerNo, arrayCharacter[l].Cpu.CurrentCheckPointIdx, arrayCharacter[l].Cpu.GetCheckPointDistance());
						}
					}
					else if (arrayCharacter[l].CurrentState == WaterSlider_Character.State.GOAL)
					{
						if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
						{
							SingletonCustom<WaterSlider_UIManager>.Instance.SetTime(ConvertIdx(arrayCharacter[l].PlayerNo), arrayCharacter[l].PlayerNo, arrayCharacter[l].GoalTime);
						}
						listRankCheck[num] = new RankData(arrayCharacter[l].PlayerNo, 99, arrayCharacter[l].GoalRank);
					}
					else
					{
						if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE)
						{
							SingletonCustom<WaterSlider_UIManager>.Instance.SetTime(ConvertIdx(arrayCharacter[l].PlayerNo), arrayCharacter[l].PlayerNo, gameTime);
						}
						CalcManager.ConvertTimeToRecordString(gameTime, arrayCharacter[l].PlayerNo);
						listRankCheck[num] = new RankData(arrayCharacter[l].PlayerNo, arrayCharacter[l].Cpu.CurrentCheckPointIdx, arrayCharacter[l].Cpu.GetCheckPointDistance());
					}
					num++;
				}
				listRankCheck.Sort((RankData c) => c.checkPointIdx, (RankData c) => c.distance);
			}
			for (int m = 0; m < listRankCheck.Length; m++)
			{
				if (listRankCheck[m].playerNo < 4 || (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE))
				{
					SingletonCustom<WaterSlider_UIManager>.Instance.ChangeRankNum(ConvertIdx(listRankCheck[m].playerNo), (!(gameTime < 1f)) ? m : 0);
				}
				for (int n = 0; n < arrayCharacter.Length; n++)
				{
					if (arrayCharacter[n].PlayerNo == listRankCheck[m].playerNo)
					{
						arrayCharacter[n].CurrentRank = m;
					}
				}
			}
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
			{
				int[] array = new int[3];
				int num2 = 0;
				for (int num3 = 0; num3 < listRankCheck.Length; num3++)
				{
					if (listRankCheck[num3].playerNo < 4)
					{
						array[listRankCheck[num3].playerNo] = num2;
						num2++;
					}
				}
			}
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum > 1 && remainPlayer == 1 && (!isSecondRun || SingletonCustom<GameSettingManager>.Instance.PlayerNum != 3))
			{
				endTransWait += Time.deltaTime;
			}
			if (isAllPlayerGoal || isAllCharacterGoal)
			{
				goalTransWait += Time.deltaTime;
			}
			if (gameTime >= 599.99f || goalTransWait >= 1.75f || endTransWait >= 20f)
			{
				SetCurrentState(State.GOAL_WAIT);
				goalTransWait = 0f;
			}
			break;
		case State.GOAL_WAIT:
			gameTime = Mathf.Clamp(gameTime + Time.deltaTime, 0f, 599.99f);
			goalTransWait += Time.deltaTime;
			if (!(goalTransWait >= 4.5f))
			{
				break;
			}
			if (setCnt == 0)
			{
				if (arrayObjSetNumber[0].activeSelf)
				{
					bool flag = false;
					for (int i = 0; i < arrayCharacter.Length; i++)
					{
						if (arrayCharacter[i].gameObject.activeSelf)
						{
							arrayCharacter[i].SkipGoal();
							if (!arrayCharacter[i].IsCpu)
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						WaitAfterExec(0.75f, delegate
						{
							NextRun();
						});
					}
					else
					{
						NextRun();
					}
				}
				else
				{
					SingletonCustom<CommonEndSimple>.Instance.Show(delegate
					{
						ToResult();
					});
				}
			}
			else
			{
				SingletonCustom<CommonEndSimple>.Instance.Show(delegate
				{
					ToResult();
				});
			}
			SetCurrentState(State.RESULT);
			goalTransWait = 0f;
			break;
		case State.RESULT:
			SingletonCustom<AudioManager>.Instance.SeStop("se_Waterslider_Water");
			break;
		}
		SingletonCustom<WaterSlider_UIManager>.Instance.UpdateMethod();
	}
	public bool IsSlipstreamZone(int _playerNo)
	{
		if (listRankCheck == null)
		{
			return false;
		}
		for (int i = 0; i < listRankCheck.Length; i++)
		{
			if (listRankCheck[i].playerNo != _playerNo || i <= 0)
			{
				continue;
			}
			for (int num = i; num > 0; num--)
			{
				if (Vector3.Distance(GetCharacter(_playerNo).transform.position, GetCharacter(listRankCheck[num - 1].playerNo).transform.position) < 1.25f && Vector3.Angle(GetCharacter(_playerNo).transform.forward, GetCharacter(listRankCheck[num - 1].playerNo).transform.position - GetCharacter(_playerNo).transform.position) < 20.5f)
				{
					return true;
				}
			}
		}
		return false;
	}
	public WaterSlider_Character GetCharacter(int _playerNo)
	{
		for (int i = 0; i < arrayCharacter.Length; i++)
		{
			if (arrayCharacter[i].PlayerNo == _playerNo)
			{
				return arrayCharacter[i];
			}
		}
		return null;
	}
	private int ConvertIdx(int _playerIdx)
	{
		int playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		if ((uint)(playerNum - 3) <= 1u && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			for (int i = 0; i < playerTeamAssignment.Length; i++)
			{
				if (_playerIdx == playerTeamAssignment[i])
				{
					_playerIdx = i % 2;
					break;
				}
			}
		}
		if (_playerIdx >= 4)
		{
			_playerIdx = 3;
		}
		return _playerIdx;
	}
	private void NextRun()
	{
		Fade(1f, 0f, delegate
		{
			switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
			{
			case 3:
				if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
				{
					arrayObjSetNumber[0].SetActive(value: false);
					arrayObjSetNumber[1].SetActive(value: true);
					arrayObjSetNumber[1].transform.SetLocalPositionY(700f);
					LeanTween.cancel(arrayObjSetNumber[1]);
					LeanTween.moveLocalY(arrayObjSetNumber[1], 357f, 1f).setEaseInOutBack().setDelay(0.25f)
						.setOnComplete((Action)delegate
						{
							LeanTween.moveLocalY(arrayObjSetNumber[1], 700f, 1f).setEaseInOutBack().setDelay(1f);
						});
					arrayCharacter[0].Init(playerTeamAssignment[2]);
					arrayCharacter[1].Init(7);
					arrayCharacter[2].Init(4);
					arrayCharacter[3].Init(8);
					secondSetPlayer.Add(playerTeamAssignment[2]);
					secondSetPlayer.Add(7);
					secondSetPlayer.Add(4);
					secondSetPlayer.Add(8);
				}
				break;
			case 4:
				if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
				{
					arrayObjSetNumber[0].SetActive(value: false);
					arrayObjSetNumber[1].SetActive(value: true);
					arrayObjSetNumber[1].transform.SetLocalPositionY(700f);
					LeanTween.cancel(arrayObjSetNumber[1]);
					LeanTween.moveLocalY(arrayObjSetNumber[1], 357f, 1f).setEaseInOutBack().setDelay(0.25f)
						.setOnComplete((Action)delegate
						{
							LeanTween.moveLocalY(arrayObjSetNumber[1], 700f, 1f).setEaseInOutBack().setDelay(1f);
						});
					arrayCharacter[0].Init(playerTeamAssignment[2]);
					arrayCharacter[1].Init(6);
					arrayCharacter[2].Init(playerTeamAssignment[3]);
					arrayCharacter[3].Init(7);
					secondSetPlayer.Add(playerTeamAssignment[2]);
					secondSetPlayer.Add(6);
					secondSetPlayer.Add(playerTeamAssignment[3]);
					secondSetPlayer.Add(7);
				}
				break;
			}
			int num = 0;
			for (int i = 0; i < arrayCharacter.Length; i++)
			{
				if (!arrayCharacter[i].IsCpu)
				{
					switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
					{
					default:
						arrayCharacter[i].SetCameraRect(cameraRect[SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1][arrayCharacter[i].PlayerNo]);
						break;
					case 3:
						if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
						{
							arrayCharacter[i].SetCameraRect(cameraRect[0][num]);
							num++;
						}
						else
						{
							arrayCharacter[i].SetCameraRect(cameraRect[SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1][arrayCharacter[i].PlayerNo]);
						}
						break;
					case 4:
						if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
						{
							arrayCharacter[i].SetCameraRect(cameraRect[1][num]);
							num++;
						}
						else
						{
							arrayCharacter[i].SetCameraRect(cameraRect[SingletonCustom<GameSettingManager>.Instance.PlayerNum - 1][arrayCharacter[i].PlayerNo]);
						}
						break;
					}
				}
			}
			gameTime = 0f;
			goalCharacterCnt = 0;
			goalTransWait = 0f;
			endTransWait = 0f;
			SingletonCustom<WaterSlider_UIManager>.Instance.NextRunInit();
			SetCurrentState(State.NONE);
			SingletonCustom<WaterSlider_UIManager>.Instance.ReverseReset();
			SingletonCustom<WaterSlider_UIManager>.Instance.StartRankNum();
			GameStart();
			isSecondRun = true;
		});
	}
	public int GetCoopPlayerNo(int _idx)
	{
		return playerTeamAssignment[_idx];
	}
	public void Fade(float _time, float _delay, Action _act)
	{
		Color color = fade.color;
		color.a = 1f;
		fade.SetAlpha(0f);
		fade.gameObject.SetActive(value: true);
		LeanTween.value(fade.gameObject, 0f, 1f, _time * 0.5f).setDelay(_delay).setEaseOutCubic()
			.setOnUpdate(delegate(float _value)
			{
				fade.SetAlpha(_value);
			})
			.setOnComplete((Action)delegate
			{
				_act();
			});
		color.a = 0f;
		LeanTween.value(fade.gameObject, 1f, 0f, _time * 0.5f).setDelay(_time * 0.5f + _delay).setEaseOutCubic()
			.setOnUpdate(delegate(float _value)
			{
				fade.SetAlpha(_value);
			})
			.setOnComplete((Action)delegate
			{
				fade.gameObject.SetActive(fade);
			});
	}
	public float[] GetTime(bool _isFirst)
	{
		float[] array = new float[firstSetPlayer.Count];
		if (_isFirst)
		{
			for (int i = 0; i < firstSetPlayer.Count; i++)
			{
				float num = array[i] = arrayGoalTime[firstSetPlayer[i]];
			}
		}
		else
		{
			for (int j = 0; j < secondSetPlayer.Count; j++)
			{
				float num2 = array[j] = arrayGoalTime[secondSetPlayer[j]];
			}
		}
		return array;
	}
	private void ToResult()
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx == 0)
		{
			if (arrayGoalTime[0] <= 80f)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.RECEIVE_PON);
			}
			if (arrayGoalTime[0] <= 75f)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.RECEIVE_PON);
			}
			if (arrayGoalTime[0] <= 70f)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.RECEIVE_PON);
			}
		}
		for (int i = 0; i < arrayCharacter.Length; i++)
		{
			if (arrayCharacter[i].gameObject.activeSelf)
			{
				arrayCharacter[i].SkipGoal();
			}
		}
		ResultGameDataParams.SetPoint();
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			GetTime(_isFirst: true);
			ResultGameDataParams.SetRecord_Float(GetTime(_isFirst: true), firstSetPlayer.ToArray(), _isGroup1Record: true, _isAscendingOrder: true);
			rankingResult.ShowResult_Time();
		}
		else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP)
		{
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 2)
			{
				ResultGameDataParams.SetRecord_Float(GetTime(_isFirst: true), firstSetPlayer.ToArray(), _isGroup1Record: true, _isAscendingOrder: true);
			}
			else
			{
				ResultGameDataParams.SetRecord_Float(GetTime(_isFirst: true), firstSetPlayer.ToArray(), _isGroup1Record: true, _isAscendingOrder: true);
				ResultGameDataParams.SetRecord_Float(GetTime(_isFirst: false), secondSetPlayer.ToArray(), _isGroup1Record: false, _isAscendingOrder: true);
			}
			rankingResult.ShowResult_Time();
		}
		else
		{
			ResultGameDataParams.SetRecord_Float(GetTime(_isFirst: true), firstSetPlayer.ToArray(), _isGroup1Record: true, _isAscendingOrder: true);
			rankingResult.ShowResult_Time();
		}
	}
	public int GetRankingDate(int playerNo)
	{
		return arrayCharacter[playerNo].CurrentRank;
	}
	public bool IsSledMoveSe(int _callPlayerNo)
	{
		for (int i = 0; i < arrayCharacter.Length; i++)
		{
			if (arrayCharacter[i].PlayerNo != _callPlayerNo && arrayCharacter[i].Sled.IsSePlay)
			{
				return true;
			}
		}
		return false;
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		LeanTween.cancel(arrayObjSetNumber[0]);
		LeanTween.cancel(arrayObjSetNumber[1]);
	}
	public void SetLayer(int idx, int layer)
	{
		arrayCharacter[idx].SetLayer(layer);
	}
}
