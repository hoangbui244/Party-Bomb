using Cinemachine;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
public class ShortTrack_MainCharacterManager : SingletonCustom<ShortTrack_MainCharacterManager>
{
	[Serializable]
	public struct PlayerData
	{
		public int userType;
		public ShortTrack_Character characters;
		public float goalTime;
		public bool isGoal;
		public int lapCnt;
		public int beforeRankNum;
		public int rankNum;
		public float[] nowPathMoveDistance;
		public float totalPathMoveDistance;
		public bool isStamina;
		public float noTouchTime;
		public bool outStamina;
		public float[] lapTime;
		public float time;
	}
	[SerializeField]
	private PlayerData[] playerData;
	[SerializeField]
	[Header("キャラクタ\u30fcが移動するパスデ\u30fcタ")]
	private CinemachineSmoothPath cinemachineSmoothPath;
	[SerializeField]
	[Header("キャラクタ\u30fcが移動するカ\u30fcト")]
	private CinemachineDollyCart[] arrayDollyCart;
	private float[] tempPathTotalDistArray;
	private int[] tempPlayerNoArray;
	private bool[] saveRankNumFlg;
	private int tempRankNum;
	private float tempRankRecord;
	[SerializeField]
	private bool[] isGoal;
	[SerializeField]
	[Header("順位によって速度に補正をかける倍率")]
	private float[] arrayAddjustRankSpeed;
	private readonly float RANK_UPDATE_INTERVAL = 0.5f;
	private float rankUpdateTime;
	private const float STAMINA_NO_TOUCH_TIME = 0.2f;
	private bool rankShou;
	private bool flg;
	private bool isOncePlayerFinalLap;
	private bool isOncePlayerGoal;
	public PlayerData[] PData => playerData;
	public void Init()
	{
		isGoal = new bool[playerData.Length];
		tempPathTotalDistArray = new float[playerData.Length];
		tempPlayerNoArray = new int[playerData.Length];
		saveRankNumFlg = new bool[playerData.Length];
		for (int i = 0; i < playerData.Length; i++)
		{
			playerData[i].userType = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
			playerData[i].nowPathMoveDistance = new float[9];
			playerData[i].lapTime = new float[9];
			playerData[i].characters.Init(i);
			playerData[i].characters.SetMainCharaStyle(playerData[i].userType);
			playerData[i].characters.SetDollyCart(arrayDollyCart[i]);
		}
		UpdateRankNum();
		UpdateRankUI(_isInit: true);
		for (int j = 0; j < playerData.Length; j++)
		{
			playerData[j].beforeRankNum = playerData[j].rankNum;
		}
	}
	public void StartMethod()
	{
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
		{
			for (int i = 0; i < playerData.Length; i++)
			{
				if (!IsNowRunnerControlePlayer(playerData[i].characters))
				{
					SHORTTRACK.UIM.InfoIconHide(i);
				}
			}
		}
		for (int j = 0; j < playerData.Length; j++)
		{
			if (IsNowRunnerControlePlayer(playerData[j].characters))
			{
				playerData[j].characters.isPlayer = true;
			}
		}
	}
	public CinemachineSmoothPath GetCinemachineSmoothPath()
	{
		return cinemachineSmoothPath;
	}
	public void StartStay()
	{
		for (int i = 0; i < playerData.Length; i++)
		{
			playerData[i].characters.StartStay();
			if (IsNowRunnerControlePlayer(playerData[i].characters))
			{
				SingletonCustom<ShortTrack_UIManager>.Instance.GetDashButtonPress(i).Show();
			}
		}
	}
	public void StartRun()
	{
		for (int i = 0; i < playerData.Length; i++)
		{
			playerData[i].characters.PathMoveStart();
			SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_slide_2", _loop: true);
		}
	}
	public void StartLastLapBGM()
	{
		StartCoroutine(LastOneBGM());
	}
	private IEnumerator LastOneBGM()
	{
		yield return new WaitForSeconds(1.2f);
		yield return new WaitForSeconds(0.1f);
	}
	public void UpdateMethod()
	{
		if (!SHORTTRACK.MGM.IsRunCharacter())
		{
			return;
		}
		for (int i = 0; i < playerData.Length; i++)
		{
			if (rankShou)
			{
				break;
			}
			if (IsNowRunnerControlePlayer(playerData[i].characters))
			{
				SHORTTRACK.UIM.ChangeRankIconShow(i, rankShou);
			}
			else if (!IsNowRunnerControlePlayer(playerData[i].characters) && SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
			{
				SHORTTRACK.UIM.ChangeRankIconShow(i, rankShou);
			}
		}
		for (int j = 0; j < playerData.Length; j++)
		{
			playerData[j].characters.UpdateMethod();
		}
		for (int k = 0; k < playerData.Length; k++)
		{
			isGoal[k] = playerData[k].isGoal;
			SHORTTRACK.MGM.lastOneResultCount = (from c in isGoal
				where c
				select c).Count();
			if (!SHORTTRACK.MCM.PData[k].isGoal)
			{
				playerData[k].time += Time.deltaTime;
				if (IsNowRunnerControlePlayer(playerData[k].characters))
				{
					SHORTTRACK.UIM.GetStaminaGauge(k).StaminaGauge(playerData[k].characters);
					if (!playerData[k].outStamina)
					{
						if (SHORTTRACK.CM.PushButton_A(playerData[k].userType))
						{
							playerData[k].characters.AddAccelSpeed();
							playerData[k].characters.StaminaDecrease();
							SHORTTRACK.UIM.GetDashButtonPress(k).PressOnlyButtonDown_A();
						}
						if (SHORTTRACK.CM.PushButtonUP_A(playerData[k].userType))
						{
							SHORTTRACK.UIM.GetDashButtonPress(k).PressOnlyButtonUp_A();
						}
					}
					if (SHORTTRACK.CM.IsMove(playerData[k].userType))
					{
						playerData[k].characters.Move(SHORTTRACK.CM.GetMoveDir(playerData[k].userType));
					}
					if (!SHORTTRACK.CM.PushButton_A(playerData[k].userType))
					{
						playerData[k].noTouchTime += Time.deltaTime;
						if (playerData[k].noTouchTime >= 0.2f)
						{
							playerData[k].isStamina = true;
						}
					}
					else
					{
						playerData[k].isStamina = false;
						playerData[k].noTouchTime = 0f;
					}
					if (!playerData[k].outStamina && playerData[k].isStamina)
					{
						playerData[k].characters.StaminaRecovery();
					}
					if (playerData[k].characters.StaminaPoint <= 0f)
					{
						playerData[k].outStamina = true;
					}
					else if (playerData[k].characters.StaminaPoint >= 30f)
					{
						playerData[k].outStamina = false;
					}
					if (playerData[k].outStamina)
					{
						playerData[k].characters.StaminaRecovery();
						SHORTTRACK.UIM.GetDashButtonPress(k).PressButtonHide();
					}
					else
					{
						SHORTTRACK.UIM.GetDashButtonPress(k).PressButtonDisplay();
					}
				}
				else
				{
					if (SHORTTRACK.UIM.ForLayoutObject.gameObject.activeSelf && !IsNowRunnerControlePlayer(playerData[3].characters))
					{
						SHORTTRACK.UIM.GetStaminaGauge(k).StaminaGauge(playerData[k].characters);
					}
					playerData[k].characters.AddAccelSpeed_CPU();
					playerData[k].characters.CPUMove();
				}
				SHORTTRACK.UIM.SetTime(k, SHORTTRACK.MGM.GetGameTime());
			}
			else
			{
				playerData[k].characters.Move(new Vector3(10f * Time.deltaTime, 0f, 0f));
				if (IsNowRunnerControlePlayer(playerData[k].characters))
				{
					SHORTTRACK.UIM.goalRankIcon(k);
				}
				if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3)
				{
					SHORTTRACK.UIM.goalRankIcon(k);
				}
			}
		}
		UpdateRankNum();
		rankUpdateTime += Time.deltaTime;
		if (rankUpdateTime > RANK_UPDATE_INTERVAL)
		{
			rankUpdateTime -= RANK_UPDATE_INTERVAL;
			UpdateRankUI();
		}
	}
	private void UpdateRankNum()
	{
		for (int i = 0; i < playerData.Length; i++)
		{
			tempPlayerNoArray[i] = i;
		}
		CalcManager.QuickSort(tempPathTotalDistArray, tempPlayerNoArray, _isAscendingOrder: false);
		tempRankNum = 0;
		for (int j = 0; j < saveRankNumFlg.Length; j++)
		{
			saveRankNumFlg[j] = false;
		}
		tempRankRecord = 0f;
		for (int k = 0; k < playerData.Length; k++)
		{
			if (!playerData[k].isGoal || saveRankNumFlg[k])
			{
				continue;
			}
			tempRankRecord = playerData[k].totalPathMoveDistance;
			for (int l = 0; l < playerData.Length; l++)
			{
				if (tempRankRecord == playerData[l].totalPathMoveDistance)
				{
					saveRankNumFlg[l] = true;
					tempRankNum++;
				}
			}
		}
		for (int m = 0; m < playerData.Length; m++)
		{
			if (saveRankNumFlg[tempPlayerNoArray[m]])
			{
				continue;
			}
			tempRankRecord = playerData[tempPlayerNoArray[m]].totalPathMoveDistance;
			for (int n = 0; n < playerData.Length; n++)
			{
				if (tempRankRecord == playerData[n].totalPathMoveDistance)
				{
					if (playerData[n].rankNum != tempRankNum)
					{
						playerData[n].rankNum = tempRankNum;
					}
					saveRankNumFlg[n] = true;
				}
			}
			tempRankNum = 0;
			for (int num = 0; num < saveRankNumFlg.Length; num++)
			{
				if (saveRankNumFlg[num])
				{
					tempRankNum++;
				}
			}
		}
	}
	private void GoalRankCheck(int _playerNum)
	{
		for (int i = 0; i < playerData.Length; i++)
		{
			if (i != _playerNum && playerData[i].isGoal && playerData[i].goalTime == playerData[_playerNum].goalTime)
			{
				if (playerData[i].rankNum > playerData[_playerNum].rankNum)
				{
					playerData[i].rankNum = playerData[_playerNum].rankNum;
					SHORTTRACK.UIM.SetGoalRank(i, playerData[i].rankNum);
				}
				else
				{
					playerData[_playerNum].rankNum = playerData[i].rankNum;
				}
			}
		}
	}
	private void UpdateRankUI(bool _isInit = false)
	{
		for (int i = 0; i < playerData.Length; i++)
		{
			if (playerData[i].beforeRankNum != playerData[i].rankNum)
			{
				playerData[i].beforeRankNum = playerData[i].rankNum;
				SHORTTRACK.UIM.SetChangeRank(i, playerData[i].rankNum, _isInit);
			}
		}
	}
	public float GetAddJustRankSpped(int _playerNum)
	{
		return arrayAddjustRankSpeed[playerData[_playerNum].rankNum];
	}
	public void AddLapCnt(int _playerNum)
	{
		playerData[_playerNum].nowPathMoveDistance[playerData[_playerNum].lapCnt] = cinemachineSmoothPath.PathLength;
		playerData[_playerNum].lapTime[playerData[_playerNum].lapCnt] = playerData[_playerNum].time;
		playerData[_playerNum].time = 0f;
		playerData[_playerNum].lapCnt++;
		if (playerData[_playerNum].lapCnt == 9)
		{
			RunnerGoal(_playerNum);
			return;
		}
		SHORTTRACK.UIM.SetCurrentLap(_playerNum, playerData[_playerNum].lapCnt);
		if (playerData[_playerNum].lapCnt != 8)
		{
			return;
		}
		SHORTTRACK.UIM.PlayLastLapAnimation(_playerNum);
		if (!IsNowRunnerControlePlayer(playerData[_playerNum].characters) || isOncePlayerFinalLap)
		{
			return;
		}
		SingletonCustom<AudioManager>.Instance.VoicePlay("voice_radio_control_lap_3_1");
		isOncePlayerFinalLap = true;
		for (int i = 0; i < playerData.Length; i++)
		{
			if (flg)
			{
				break;
			}
			if (IsNowRunnerControlePlayer(playerData[i].characters))
			{
				StartLastLapBGM();
				flg = true;
			}
		}
	}
	public void RunnerGoal(int _playerNum)
	{
		if (SHORTTRACK.MGM.IsDuringResult())
		{
			return;
		}
		playerData[_playerNum].goalTime = CalcManager.ConvertDecimalSecond(SHORTTRACK.MGM.GetGameTime());
		SHORTTRACK.UIM.SetTime(_playerNum, playerData[_playerNum].goalTime);
		playerData[_playerNum].isGoal = true;
		if (!SHORTTRACK.MGM.IsDuringGame())
		{
			return;
		}
		GoalRankCheck(_playerNum);
		int num = 0;
		while (num < playerData.Length)
		{
			bool flag = false;
			if (playerData[num].rankNum != 0 && !flag)
			{
				UnityEngine.Debug.Log("１位がいるか検索中");
				num++;
				if (num >= playerData.Length)
				{
					UnityEngine.Debug.Log("１位はいなかったので１位を出します。");
					flag = true;
					num = 0;
				}
				if (flag)
				{
					if (!(playerData[_playerNum].goalTime < playerData[num].goalTime))
					{
						UnityEngine.Debug.Log(_playerNum.ToString() + "は１位ではありませんでした。");
						break;
					}
					num++;
					if (num >= playerData.Length)
					{
						playerData[_playerNum].rankNum = 0;
						UnityEngine.Debug.Log("１位は" + _playerNum.ToString() + "Pになりました");
					}
				}
				continue;
			}
			UnityEngine.Debug.Log("１位は居ました");
			break;
		}
		SHORTTRACK.UIM.SetGoalRank(_playerNum, playerData[_playerNum].rankNum);
		if (IsNowRunnerControlePlayer(playerData[_playerNum].characters))
		{
			playerData[_playerNum].characters.NonActiveCursor();
			SHORTTRACK.UIM.GetDashButtonPress(_playerNum).Hide();
			if (!isOncePlayerGoal)
			{
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal");
				isOncePlayerGoal = true;
			}
			if (IsRunFinishPlayer())
			{
				SHORTTRACK.MGM.GameEnd();
				SetCPUFutureGoal();
				SHORTTRACK.UIM.SetTime(_playerNum, playerData[_playerNum].goalTime);
			}
		}
	}
	private void SetCPUFutureGoal()
	{
		for (int i = 0; i < playerData.Length; i++)
		{
			if (!IsNowRunnerControlePlayer(playerData[i].characters) && !playerData[i].isGoal)
			{
				playerData[i].isGoal = true;
				float value = cinemachineSmoothPath.PathLength * 9f / playerData[i].totalPathMoveDistance * SHORTTRACK.MGM.GetGameTime();
				value = Mathf.Clamp(value, 0f, 599.99f);
				value = CalcManager.ConvertDecimalSecond(value);
				playerData[i].goalTime = value;
			}
		}
	}
	private bool IsRunFinishPlayer()
	{
		int num = 0;
		for (int i = 0; i < playerData.Length; i++)
		{
			if (IsNowRunnerControlePlayer(playerData[i].characters) && playerData[i].isGoal)
			{
				num++;
			}
		}
		return num == SingletonCustom<GameSettingManager>.Instance.PlayerNum;
	}
	public void SetMovePathDistance(int _playerNum, float _pathDist)
	{
		if (playerData[_playerNum].lapCnt < 9)
		{
			playerData[_playerNum].nowPathMoveDistance[playerData[_playerNum].lapCnt] = CalcManager.ConvertDecimalSecond(_pathDist);
			playerData[_playerNum].totalPathMoveDistance = 0f;
			for (int i = 0; i < playerData[_playerNum].nowPathMoveDistance.Length; i++)
			{
				playerData[_playerNum].totalPathMoveDistance += playerData[_playerNum].nowPathMoveDistance[i];
			}
			tempPathTotalDistArray[_playerNum] = playerData[_playerNum].totalPathMoveDistance;
		}
	}
	public bool IsNowRunnerControlePlayer(ShortTrack_Character _chara)
	{
		if (playerData[_chara.PlayerNum].userType < 4)
		{
			return true;
		}
		return false;
	}
}
