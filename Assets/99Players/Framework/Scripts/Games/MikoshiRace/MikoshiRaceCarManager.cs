using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MikoshiRaceCarManager : SingletonCustom<MikoshiRaceCarManager>
{
	private const float TROPHY_GOLD_TIME = 124f;
	private const float TROPHY_SILVER_TIME = 130f;
	private const float TROPHY_BRONZE_TIME = 140f;
	private static readonly float[] CPU_FUTURE_SHAKE_USE_TIME = new float[3]
	{
		12f,
		10f,
		8f
	};
	private static readonly float[][] CPU_FUTURE_POINT_BETWEEN_TIMES = new float[3][]
	{
		new float[5]
		{
			13f,
			16f,
			24f,
			42f,
			18f
		},
		new float[5]
		{
			11.5f,
			13.5f,
			23f,
			36f,
			15f
		},
		new float[5]
		{
			10f,
			11f,
			22f,
			30f,
			12f
		}
	};
	[SerializeField]
	private MikoshiRaceCarScript[] cars;
	private int carNum;
	private bool isEndRace;
	private int playerGoalCount;
	private bool[] isGoalCamera = new bool[8];
	private float charaCameraOffset = 2f;
	private float rankUpdateInterval = 0.5f;
	private float rankUpdateTime;
	private float reverseCheckInterval = 0.5f;
	private float reverseCheckTime;
	private bool[] isViewReverseCars = new bool[8];
	private bool isChangePitchBGM;
	private int playerNowLapNo;
	private bool isPlaySeWasshoi;
	private float seTime;
	public void Init()
	{
		DataInit();
		carNum = SingletonCustom<MikoshiRaceGameManager>.Instance.CarNum;
		Transform[] createAnchor = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.createAnchor;
		for (int i = 0; i < cars.Length; i++)
		{
			if (i < carNum)
			{
				cars[i].transform.position = createAnchor[i].position;
				cars[i].transform.rotation = createAnchor[i].rotation;
				cars[i].gameObject.SetActive(value: true);
				cars[i].Init(i);
			}
			else
			{
				cars[i].gameObject.SetActive(value: false);
			}
		}
	}
	public void SecondGroupInit()
	{
		DataInit();
		Transform[] createAnchor = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.createAnchor;
		for (int i = 0; i < carNum; i++)
		{
			cars[i].transform.position = createAnchor[i].position;
			cars[i].transform.rotation = createAnchor[i].rotation;
			cars[i].SecondGroupInit();
		}
	}
	private void DataInit()
	{
		isEndRace = false;
		playerGoalCount = 0;
		for (int i = 0; i < isGoalCamera.Length; i++)
		{
			isGoalCamera[i] = false;
		}
	}
	public void UpdateMethod()
	{
		seTime -= Time.deltaTime;
		if (seTime < 0f)
		{
			seTime = 0f;
		}
		int num = 0;
		for (int i = 0; i < carNum; i++)
		{
			if (!cars[i].IsGoal)
			{
				cars[i].ViewTime = SingletonCustom<MikoshiRaceGameManager>.Instance.GameTime;
			}
			bool flag = true;
			if (cars[i].IsPlayer && !cars[i].IsGoal)
			{
				PlayerOperation(i);
				if (cars[i].IsShakeCtrl)
				{
					num++;
				}
				flag = false;
			}
			if (SingletonCustom<MikoshiRaceGameManager>.Instance.IsGameStart && !isGoalCamera[i] && SingletonCustom<MikoshiRaceGameManager>.Instance.CheckCameraViewCar(cars[i]))
			{
				SingletonCustom<MikoshiRaceGameManager>.Instance.RaceCameraUpdate(cars[i]);
				if (cars[i].IsPlayer && !cars[i].IsGoal && MikoshiRaceController.GetLookRearButton(cars[i].PlayerNo) && !cars[i].IsShake)
				{
					SingletonCustom<MikoshiRaceGameManager>.Instance.RaceCameraRear(cars[i]);
				}
			}
			if (flag)
			{
				cars[i].AiMove();
			}
			cars[i].UpdateMethod();
		}
		if (SingletonCustom<MikoshiRaceGameManager>.Instance.IsGameEnd)
		{
			if (isPlaySeWasshoi)
			{
				SingletonCustom<AudioManager>.Instance.SeStop("se_wasshoi");
				isPlaySeWasshoi = false;
			}
		}
		else
		{
			if (!isPlaySeWasshoi && num > 0)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_wasshoi", _loop: true);
				isPlaySeWasshoi = true;
			}
			if (isPlaySeWasshoi && num == 0)
			{
				SingletonCustom<AudioManager>.Instance.SeStop("se_wasshoi");
				isPlaySeWasshoi = false;
			}
		}
		if (SingletonCustom<MikoshiRaceGameManager>.Instance.IsGameStart)
		{
			for (int j = 0; j < carNum; j++)
			{
				if (SingletonCustom<MikoshiRaceGameManager>.Instance.CheckCameraViewCar(cars[j]))
				{
					if (cars[j].IsGoal && cars[j].ViewTime >= 0f)
					{
						SingletonCustom<MikoshiRaceUiManager>.Instance.SetTime(cars[j].CarNo, cars[j].ViewTime);
					}
					else if (!SingletonCustom<MikoshiRaceGameManager>.Instance.IsGameEnd)
					{
						SingletonCustom<MikoshiRaceUiManager>.Instance.SetTime(cars[j].CarNo, SingletonCustom<MikoshiRaceGameManager>.Instance.GameTime);
					}
				}
			}
			rankUpdateTime += Time.deltaTime;
			if (rankUpdateTime > rankUpdateInterval)
			{
				rankUpdateTime -= rankUpdateInterval;
				RankUpdate();
			}
		}
		reverseCheckTime += Time.deltaTime;
		if (reverseCheckTime > reverseCheckInterval)
		{
			reverseCheckTime -= reverseCheckInterval;
			ReverseCheck();
		}
		GuideArrowUpdate();
	}
	private void RankUpdate()
	{
		int[] array = new int[carNum];
		float[] array2 = new float[carNum];
		for (int i = 0; i < carNum; i++)
		{
			array[i] = i;
			array2[i] = cars[i].nowRunDistance;
		}
		int num = 0;
		for (int j = 0; j < carNum; j++)
		{
			if (cars[j].IsRankLock)
			{
				if (j != num)
				{
					int num2 = array[j];
					array[j] = array[num];
					array[num] = num2;
					float num3 = array2[j];
					array2[j] = array2[num];
					array2[num] = num3;
				}
				num++;
			}
		}
		for (int k = num; k < carNum; k++)
		{
			for (int l = k + 1; l < carNum; l++)
			{
				if (cars[array[k]].MikoshiPointNo > cars[array[l]].MikoshiPointNo || (cars[array[k]].MikoshiPointNo == cars[array[l]].MikoshiPointNo && !cars[array[k]].IsShake && cars[array[l]].IsShake))
				{
					continue;
				}
				if (cars[array[k]].MikoshiPointNo == cars[array[l]].MikoshiPointNo && cars[array[k]].IsShake && cars[array[l]].IsShake)
				{
					if (cars[array[k]].MikoshiPointArrivalTime > cars[array[l]].MikoshiPointArrivalTime)
					{
						int num4 = array[k];
						array[k] = array[l];
						array[l] = num4;
						float num5 = array2[k];
						array2[k] = array2[l];
						array2[l] = num5;
					}
				}
				else if (cars[array[k]].MikoshiPointNo < cars[array[l]].MikoshiPointNo || cars[array[k]].RunDistance < cars[array[l]].RunDistance)
				{
					int num6 = array[k];
					array[k] = array[l];
					array[l] = num6;
					float num7 = array2[k];
					array2[k] = array2[l];
					array2[l] = num7;
				}
			}
			cars[array[k]].NowRank = k;
		}
		for (int m = 0; m < carNum; m++)
		{
			if (SingletonCustom<MikoshiRaceGameManager>.Instance.CheckCameraViewCar(cars[m]))
			{
				SingletonCustom<MikoshiRaceUiManager>.Instance.ChangeRankNum(cars[m].CarNo, cars[m].NowRank);
			}
		}
	}
	private void ReverseCheck()
	{
		for (int i = 0; i < carNum; i++)
		{
			if (!SingletonCustom<MikoshiRaceGameManager>.Instance.CheckCameraViewCar(cars[i]))
			{
				continue;
			}
			bool isReverseRun = cars[i].IsReverseRun;
			if (isViewReverseCars[i] != isReverseRun)
			{
				isViewReverseCars[i] = isReverseRun;
				int num = cars[i].CarNo;
				if (num == -1)
				{
					num = 3;
				}
				if (isReverseRun)
				{
					SingletonCustom<MikoshiRaceUiManager>.Instance.ReverseRunON(num);
				}
				else
				{
					SingletonCustom<MikoshiRaceUiManager>.Instance.ReverseRunOFF(num);
				}
			}
		}
	}
	public void RaceStart()
	{
		for (int i = 0; i < carNum; i++)
		{
			cars[i].PlayRunEffect();
		}
	}
	public void GuideArrowUpdate()
	{
		for (int i = 0; i < carNum; i++)
		{
			MikoshiRaceGuideArrow guideArrow = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.GetGuideArrow(cars[i].CarNo, cars[i].MikoshiPointNo);
			if (guideArrow != null)
			{
				guideArrow.UpdateMethod();
			}
		}
	}
	public void GuideArrowChange(int _carNo)
	{
		MikoshiRaceGuideArrow[] guideArrows = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.GetGuideArrows(_carNo);
		for (int i = 0; i < guideArrows.Length; i++)
		{
			if (guideArrows[i] != null)
			{
				guideArrows[i].gameObject.SetActive(cars[_carNo].MikoshiPointNo == i);
			}
		}
	}
	public void GuideArrowColorSetting(int _carNo)
	{
		if (SingletonCustom<MikoshiRaceGameManager>.Instance.CheckCameraViewCarNo(_carNo))
		{
			int num = cars[_carNo].PlayerNo;
			if (num >= 4)
			{
				num = 3;
			}
			SingletonCustom<MikoshiRaceGameManager>.Instance.Course.ChangeGudeArrowColor(_carNo, num);
		}
	}
	public void CarNextLap(int _carNo)
	{
		if (cars[_carNo].NowLap > SingletonCustom<MikoshiRaceGameManager>.Instance.RaceLap || !SingletonCustom<MikoshiRaceGameManager>.Instance.CheckCameraViewCar(cars[_carNo]))
		{
			return;
		}
		if (!isChangePitchBGM && cars[_carNo].NowLap == SingletonCustom<MikoshiRaceGameManager>.Instance.RaceLap - 1)
		{
			isChangePitchBGM = true;
			StartCoroutine(LastOneBGM());
		}
		bool flag = false;
		if (playerNowLapNo < cars[_carNo].NowLap)
		{
			playerNowLapNo = cars[_carNo].NowLap;
			flag = true;
		}
		switch (cars[_carNo].NowLap)
		{
		case 1:
			SingletonCustom<AudioManager>.Instance.SePlay("se_lap_jingle");
			if (flag)
			{
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_radio_control_lap_0_1", _loop: false, 0f, 1f, 1f, 0.35f);
			}
			break;
		case 2:
			SingletonCustom<AudioManager>.Instance.SePlay("se_lap_jingle");
			if (flag)
			{
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_radio_control_lap_3_1", _loop: false, 0f, 1f, 1f, 0.35f);
			}
			break;
		}
	}
	public void CarGoal(int _carNo)
	{
		float num = CalcManager.ConvertDecimalSecond(SingletonCustom<MikoshiRaceGameManager>.Instance.GameTime);
		int num2 = 0;
		for (int i = 0; i < carNum; i++)
		{
			if (cars[i].IsRankLock && Mathf.FloorToInt(num * 100f) != Mathf.FloorToInt(cars[i].ViewTime * 100f))
			{
				num2++;
			}
		}
		cars[_carNo].NowRank = num2;
		cars[_carNo].ViewTime = num;
		cars[_carNo].GoalTime = num;
		cars[_carNo].IsGoal = true;
		cars[_carNo].IsRankLock = true;
		CalcManager.ConvertTimeToRecordString(num, cars[_carNo].PlayerNo);
		SingletonCustom<AudioManager>.Instance.SePlay("se_starter_pistol");
		if (!SingletonCustom<MikoshiRaceGameManager>.Instance.CheckCameraViewCar(cars[_carNo]))
		{
			return;
		}
		if (cars[_carNo].IsPlayer)
		{
			playerGoalCount++;
			if (playerGoalCount == SingletonCustom<MikoshiRaceGameManager>.Instance.PlayerNum)
			{
				EndRaceDelay(3f);
			}
			if (SingletonCustom<MikoshiRaceGameManager>.Instance.PlayerNum >= 2 && playerGoalCount == SingletonCustom<MikoshiRaceGameManager>.Instance.PlayerNum - 1)
			{
				EndRaceDelay(20f);
			}
		}
		SingletonCustom<MikoshiRaceUiManager>.Instance.SetResultRankSprite(cars[_carNo].CarNo, cars[_carNo].NowRank);
		StartCoroutine(_CarGoalAfter(cars[_carNo].PlayerNo, _carNo));
		if (playerNowLapNo < SingletonCustom<MikoshiRaceGameManager>.Instance.RaceLap)
		{
			playerNowLapNo = SingletonCustom<MikoshiRaceGameManager>.Instance.RaceLap;
			SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal_girl");
		}
	}
	private void CarFutureGoal(int _carNo)
	{
		cars[_carNo].IsGoal = true;
		cars[_carNo].IsRankLock = true;
		float gameTime = SingletonCustom<MikoshiRaceGameManager>.Instance.GameTime;
		int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		float num = CPU_FUTURE_SHAKE_USE_TIME[aiStrength];
		float[] array = CPU_FUTURE_POINT_BETWEEN_TIMES[aiStrength];
		int mikoshiPointNo = cars[_carNo].MikoshiPointNo;
		if (cars[_carNo].IsShake)
		{
			gameTime += (1f - cars[_carNo].ShakeValueLerp) * num;
		}
		else
		{
			float num2 = array[mikoshiPointNo] - cars[_carNo].MikoshiPointBetweenTime;
			gameTime = ((!(num2 > 0f)) ? (gameTime + num2 / 10f) : (gameTime + num2));
		}
		int num3 = SingletonCustom<MikoshiRaceGameManager>.Instance.Course.mikoshiPointColliders.Length;
		for (int i = mikoshiPointNo; i < num3; i++)
		{
			gameTime += num;
		}
		for (int j = mikoshiPointNo + 1; j < array.Length; j++)
		{
			gameTime += array[j];
		}
		gameTime += (float)_carNo / 10f;
		if (gameTime > 599.99f)
		{
			gameTime = 599.99f;
		}
		gameTime = CalcManager.ConvertDecimalSecond(gameTime);
		cars[_carNo].ViewTime = gameTime;
		cars[_carNo].GoalTime = gameTime;
		CalcManager.ConvertTimeToRecordString(gameTime, cars[_carNo].PlayerNo);
	}
	private IEnumerator _CarGoalAfter(int _playerNo, int _carNo)
	{
		yield return new WaitForSeconds(2f);
	}
	private IEnumerator LastOneBGM()
	{
		yield return new WaitForSeconds(0.1f);
	}
	private void TrophySet()
	{
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && cars[0].ViewTime > 0f)
		{
			if (cars[0].ViewTime <= 140f)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.BLOCK_WIPER);
			}
			if (cars[0].ViewTime <= 130f)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.BLOCK_WIPER);
			}
			if (cars[0].ViewTime <= 124f)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.BLOCK_WIPER);
			}
		}
	}
	public void EndRaceDelay(float _delayTime)
	{
		if (!isEndRace)
		{
			StartCoroutine(_EndRaceDelay(_delayTime));
		}
	}
	private IEnumerator _EndRaceDelay(float _delayTime)
	{
		float timer2 = 0f;
		while (timer2 < _delayTime)
		{
			timer2 += Time.deltaTime;
			if (isEndRace)
			{
				yield break;
			}
			yield return null;
		}
		bool flag = false;
		for (int i = 0; i < cars.Length; i++)
		{
			if (cars[i].IsPlayer && !cars[i].IsGoal)
			{
				SingletonCustom<MikoshiRaceUiManager>.Instance.SetResultRankSprite(cars[i].CarNo, 3);
				SingletonCustom<MikoshiRaceUiManager>.Instance.HideShakeControlInfo(cars[i].CarNo);
				flag = true;
				break;
			}
		}
		if (flag)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_starter_pistol");
			timer2 = 0f;
			while (timer2 < 1f)
			{
				timer2 += Time.deltaTime;
				if (isEndRace)
				{
					yield break;
				}
				yield return null;
			}
		}
		EndRace();
	}
	public void EndRace()
	{
		if (isEndRace)
		{
			return;
		}
		isEndRace = true;
		for (int i = 0; i < carNum; i++)
		{
			if (!cars[i].IsGoal && !cars[i].IsPlayer)
			{
				CarFutureGoal(i);
			}
		}
		TrophySet();
		if (!SingletonCustom<MikoshiRaceGameManager>.Instance.HasSecondGroup || SingletonCustom<MikoshiRaceGameManager>.Instance.IsNowSecondGroup)
		{
			SingletonCustom<MikoshiRaceUiManager>.Instance.CloseGameUI();
		}
		List<int>[] playerGroup = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		float[] array = new float[carNum];
		int[] array2 = new int[carNum];
		int num = 0;
		if (SingletonCustom<MikoshiRaceGameManager>.Instance.IsNowSecondGroup)
		{
			num = 2;
			if (SingletonCustom<MikoshiRaceGameManager>.Instance.PlayerNum == 1)
			{
				num++;
			}
		}
		for (int j = 0; j < carNum; j++)
		{
			array[j] = cars[j].GoalTime;
			array2[j] = cars[j].PlayerNo;
		}
		if (carNum == 8)
		{
			ResultGameDataParams.SetRecord_Float_Ranking8Numbers(array, array2, _isAscendingOrder: true);
		}
		else
		{
			ResultGameDataParams.SetRecord_Float(array, array2, !SingletonCustom<MikoshiRaceGameManager>.Instance.IsNowSecondGroup, _isAscendingOrder: true);
		}
		if (SingletonCustom<MikoshiRaceGameManager>.Instance.HasSecondGroup)
		{
			if (SingletonCustom<MikoshiRaceGameManager>.Instance.IsNowSecondGroup)
			{
				SingletonCustom<MikoshiRaceGameManager>.Instance.SetSecondGroupData(array, array2);
			}
			else
			{
				SingletonCustom<MikoshiRaceGameManager>.Instance.SetFirstGroupData(array, array2);
			}
		}
		SingletonCustom<MikoshiRaceGameManager>.Instance.GameEnd();
	}
	private void PlayerOperation(int _charaIdx)
	{
		if (!SingletonCustom<MikoshiRaceGameManager>.Instance.IsGameEnd)
		{
			CharaMove(_charaIdx);
		}
	}
	private void CharaMove(int _no)
	{
		Vector3 moveDir = MikoshiRaceController.GetMoveDir(cars[_no].PlayerNo);
		bool accelButton = MikoshiRaceController.GetAccelButton(cars[_no].PlayerNo);
		bool backButton = MikoshiRaceController.GetBackButton(cars[_no].PlayerNo);
		cars[_no].SetControlData(moveDir.x, accelButton, backButton);
		if (SingletonCustom<MikoshiRaceGameManager>.Instance.IsGameStart)
		{
			if (MikoshiRaceController.GetCameraAngleRightButton(cars[_no].PlayerNo))
			{
				cars[_no].ChangeCameraPointRight();
			}
			else if (MikoshiRaceController.GetCameraAngleLeftButton(cars[_no].PlayerNo))
			{
				cars[_no].ChangeCameraPointLeft();
			}
			if (cars[_no].IsShakeCtrl && MikoshiRaceController.GetMikoshiShakeButtonDown(cars[_no].PlayerNo))
			{
				cars[_no].ShakeControlAction();
			}
		}
	}
	public Vector3 GetPlayerOnePosition()
	{
		return cars[0].GetPos();
	}
	public MikoshiRaceCarScript[] GetCars()
	{
		return cars;
	}
	public MikoshiRaceCarScript GetCar(int _carNo)
	{
		return cars[_carNo];
	}
	public bool IsPlayer(int _charaNo)
	{
		return cars[_charaNo].IsPlayer;
	}
	public void SetCharaMoveActive(bool _active)
	{
		for (int i = 0; i < carNum; i++)
		{
			cars[i].GetRigid().isKinematic = !_active;
		}
	}
	public void AllSePlayCheck(string _seName, float _volum = 1f)
	{
		if (seTime <= 0f)
		{
			SingletonCustom<AudioManager>.Instance.SePlay(_seName, _loop: false, 0f, _volum);
			seTime = 0.07f;
		}
	}
}
