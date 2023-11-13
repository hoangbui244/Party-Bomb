using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MonsterRace_CarManager : SingletonCustom<MonsterRace_CarManager>
{
	private struct CPU_PARAMETER
	{
		public float addMaxSpeed;
		public float magMaxSteerAngle;
		public float slipStreamMag;
		public CPU_PARAMETER(float _addMaxSpeed, float _magMaxSteerAngle, float _slipStreamMag)
		{
			addMaxSpeed = _addMaxSpeed;
			magMaxSteerAngle = _magMaxSteerAngle;
			slipStreamMag = _slipStreamMag;
		}
	}
	private const float MULTI_LAST_GOAL_WAIT_TIME = 20f;
	private const float SPEED_LINE_SCROLL_SPEED = 0.5f;
	public static readonly int[] START_POS_ORDER_FOUR = new int[4]
	{
		0,
		1,
		2,
		3
	};
	public static readonly int[] START_POS_ORDER_EIGHT = new int[8]
	{
		5,
		6,
		4,
		7,
		0,
		1,
		2,
		3
	};
	private static readonly CPU_PARAMETER[][] CPU_PARAMETER_ARRAY = new CPU_PARAMETER[3][]
	{
		new CPU_PARAMETER[4]
		{
			new CPU_PARAMETER(-0.3f, 1.1f, 0.6f),
			new CPU_PARAMETER(-0.33f, 0.8f, 1f),
			new CPU_PARAMETER(-0.36f, 1f, 0.6f),
			new CPU_PARAMETER(-0.39f, 0.9f, 1f)
		},
		new CPU_PARAMETER[4]
		{
			new CPU_PARAMETER(-0.1f, 1.3f, 0.8f),
			new CPU_PARAMETER(-0.13f, 1f, 1.2f),
			new CPU_PARAMETER(-0.16f, 1.2f, 0.8f),
			new CPU_PARAMETER(-0.19f, 1.1f, 1.2f)
		},
		new CPU_PARAMETER[4]
		{
			new CPU_PARAMETER(0.1f, 1.5f, 1f),
			new CPU_PARAMETER(0.07f, 1.2f, 1.4f),
			new CPU_PARAMETER(0.04f, 1.4f, 1f),
			new CPU_PARAMETER(0.01f, 1.3f, 1.4f)
		}
	};
	private int raceLap = 3;
	private int playerNum = 1;
	private int carNum;
	private int teamNum;
	private int[] startPosOrder;
	private int courseIdx;
	private MonsterRace_Course course;
	private MonsterRace_Course[] tempCreateCourses;
	[SerializeField]
	private MonsterRace_Course[] coursePrefab;
	[SerializeField]
	private Transform courseAnchor;
	[SerializeField]
	private GameObject carPrefab;
	[SerializeField]
	private Transform carAnchor;
	[SerializeField]
	private Material[] carMaterial;
	[SerializeField]
	private Camera[] tpsCamera;
	private bool[] isCameraMaskChanges = new bool[4];
	private bool[] isCameraFps = new bool[4];
	private bool[] isCameraViewFrames = new bool[4];
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
	[SerializeField]
	private Camera testWorldCamera;
	[SerializeField]
	private GameObject[] speedLineObjs;
	[SerializeField]
	private Material[] speedLineMaterials;
	[SerializeField]
	private GameObject[] multiShadowObjs;
	private Vector2[] viewportPos;
	private MonsterRace_CarScript[] cars;
	private bool[] isPlayer = new bool[8];
	private bool[] isPlayerAutoControl = new bool[8];
	private float gameTime;
	[SerializeField]
	private GameObject[] worldPlayerViewObj;
	private const float VIEWOBJ_Y = 1.2f;
	[SerializeField]
	private GameObject goalTextAnchor;
	private float multiLastGoalWaitTime;
	private int playerGoalCount;
	private bool isEndRace;
	private bool[] isGoalCamera = new bool[8];
	private float charaCameraOffset = 2f;
	private float[] cameraOnTriggerTime = new float[4];
	private float[] cameraOnTriggerPosY = new float[4];
	private float rankUpdateInterval = 0.5f;
	private float rankUpdateTime;
	private float reverseCheckInterval = 0.5f;
	private float reverseCheckTime;
	private bool[] isViewReverseCars = new bool[8];
	private bool isChangePitchBGM;
	private int playerNowLapNo;
	private int[] twoRacePlayerOrderArray = new int[0];
	private int[,] charaStyleGender = new int[4, 2];
	private int[,] charaStyleFace = new int[4, 2];
	private int[,] charaStyleHair = new int[4, 2];
	private int[,] charaStyleShape = new int[4, 2];
	private int[,] charaStylePlayerNo = new int[4, 2];
	[SerializeField]
	private CharacterStyle[] twoRaceCharaStyles;
	[SerializeField]
	private Transform[] twoRaceRankPosAnchor;
	private int[] twoRaceRankPosGetCounts = new int[4];
	private bool isEightRun;
	private bool[] isViewSpeedLines = new bool[4];
	private List<CPU_PARAMETER> cpuParamList;
	private List<float> cpuAddMaxSpeedList;
	private List<float> cpuMagMaxSteerAngleList;
	private List<float> cpuSlipStreamMagList;
	private float buoyUpdeteTimer;
	[SerializeField]
	private float buoyUpdeteInterval = 0.1f;
	private float seTime;
	public int RaceLap => raceLap;
	public int PlayerNum
	{
		get
		{
			return playerNum;
		}
		set
		{
			playerNum = value;
		}
	}
	public int CarNum
	{
		get
		{
			return carNum;
		}
		set
		{
			carNum = value;
		}
	}
	public int TeamNum
	{
		get
		{
			return teamNum;
		}
		set
		{
			teamNum = value;
		}
	}
	public bool IsViewCpu => playerNum == 3;
	public float CircuitLength => course.GetPosCircuit().Length;
	public int[] StartPosOrder => startPosOrder;
	public int CourseIdx
	{
		get
		{
			return courseIdx;
		}
		set
		{
			courseIdx = value;
		}
	}
	public MonsterRace_Course Course => course;
	public Camera[] TpsCamera => tpsCamera;
	public bool IsEightRun => isEightRun;
	public bool IsGameWatchingMode()
	{
		return !isPlayer[0];
	}
	public void SetGameWatchingMode(bool _flg)
	{
		isPlayer[0] = !_flg;
	}
	public void Init()
	{
		List<int>[] playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		if (SingletonCustom<GameSettingManager>.Instance.IsEightBattle)
		{
			isEightRun = true;
		}
		for (int i = 0; i < isPlayer.Length; i++)
		{
			isPlayer[i] = (i < playerNum);
		}
		carNum = 4;
		if (isEightRun)
		{
			carNum = 8;
		}
		teamNum = carNum;
		startPosOrder = (isEightRun ? START_POS_ORDER_EIGHT : START_POS_ORDER_FOUR);
		courseIdx = SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx;
		tempCreateCourses = new MonsterRace_Course[coursePrefab.Length];
		for (int j = 0; j < tempCreateCourses.Length; j++)
		{
			tempCreateCourses[j] = UnityEngine.Object.Instantiate(coursePrefab[j], courseAnchor.position, Quaternion.identity, courseAnchor);
			tempCreateCourses[j].gameObject.SetActive(value: false);
		}
		tempCreateCourses[courseIdx].gameObject.SetActive(value: true);
		course = tempCreateCourses[courseIdx];
		KeshigomuIchigoUpdate();
		gameTime = 0f;
		cars = new MonsterRace_CarScript[carNum];
		int num = 0;
		UnityEngine.Random.Range(0, 3);
		int baseTextureTotal = SingletonCustom<StyleTextureManager>.Instance.GetBaseTextureTotal(StyleTextureManager.GenderType.BOY);
		UnityEngine.Random.Range(0, baseTextureTotal);
		for (int k = 0; k < carNum; k++)
		{
			int num2 = k % teamNum;
			int num3 = k / teamNum;
			cars[k] = UnityEngine.Object.Instantiate(carPrefab, Vector3.zero, Quaternion.identity, carAnchor).GetComponent<MonsterRace_CarScript>();
			cars[k].gameObject.SetActive(value: true);
			cars[k].transform.position = course.createAnchor[startPosOrder[k]].transform.position;
			cars[k].transform.eulerAngles = course.createAnchor[startPosOrder[k]].transform.eulerAngles;
			cars[k].Init(num2, k);
			int num4 = (num2 >= SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length) ? (4 + num) : SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[num2][num3];
			cars[k].StyleCharaNo = num4;
			cars[k].SetCarMaterial(carMaterial[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[cars[k].StyleCharaNo]]);
			if (num4 < 4)
			{
				cars[k].IsPlayer = true;
				int num5 = playerGroupList[num2][num3];
				cars[k].PlayerNo = num5;
				if (twoRacePlayerOrderArray.Length > 2)
				{
					cars[k].PlayerNo = num3;
					cars[k].PlayerRealNo = num5;
					num5 = num3;
				}
				cars[k].SetCharaGameStyle(num5, num2);
				tpsCamera[num5].transform.position = cars[num5].GetFirstCameraPosition();
				tpsCamera[num5].transform.rotation = Quaternion.LookRotation(cars[num5].GetCameraLookAtPos() - cars[num5].GetZoomCameraPosition());
			}
			else
			{
				if (CheckViewCpuCarNo(k))
				{
					tpsCamera[3].transform.position = cars[3].GetFirstCameraPosition();
					tpsCamera[3].transform.rotation = Quaternion.LookRotation(cars[3].GetCameraLookAtPos() - cars[3].GetZoomCameraPosition());
				}
				cars[k].IsPlayer = false;
				num++;
				cars[k].SetCharaGameStyle(num4, num2);
			}
			cars[k].InitAudio();
		}
		CpuStrengthDataSetting();
		for (int l = 0; l < worldPlayerViewObj.Length; l++)
		{
			if (playerNum > 1 && l < playerNum)
			{
				worldPlayerViewObj[l].transform.parent = cars[l].transform;
				worldPlayerViewObj[l].transform.localPosition = new Vector3(0f, 1.2f, 0f);
			}
			else
			{
				worldPlayerViewObj[l].SetActive(value: false);
			}
		}
		SetCharaMoveActive(_active: false);
		for (int m = 0; m < carNum; m++)
		{
			cars[m].NowRank = m + 1;
		}
		for (int n = 0; n < cars.Length; n++)
		{
			if (cars[n].IsPlayer)
			{
				tpsCamera[cars[n].PlayerNo].gameObject.SetActive(value: true);
				tpsCamera[cars[n].PlayerNo].rect = cameraRect[playerNum - 1][cars[n].PlayerNo];
			}
			else if (CheckViewCpuCarNo(n))
			{
				tpsCamera[3].gameObject.SetActive(value: true);
				tpsCamera[3].rect = cameraRect[playerNum - 1][3];
			}
		}
		for (int num6 = 0; num6 < cars.Length; num6++)
		{
		}
	}
	public void NextRunInit()
	{
		for (int i = 0; i < carNum; i++)
		{
			UnityEngine.Object.Destroy(cars[i].gameObject);
		}
		isPlayer[1] = (twoRacePlayerOrderArray.Length == 4);
		playerNum = twoRacePlayerOrderArray.Length - 2;
		isEndRace = false;
		playerGoalCount = 0;
		isChangePitchBGM = false;
		viewportPos = null;
		GetPlayerCharaViewportPos();
		int num = 2;
		gameTime = 0f;
		cars = new MonsterRace_CarScript[carNum];
		int num2 = 0;
		for (int j = 0; j < carNum; j++)
		{
			int num3 = j % num;
			int num4 = j / num;
			cars[j] = UnityEngine.Object.Instantiate(carPrefab, Vector3.zero, Quaternion.identity, carAnchor).GetComponent<MonsterRace_CarScript>();
			cars[j].gameObject.SetActive(value: true);
			cars[j].transform.position = course.createAnchor[j].transform.position + Vector3.down * cars[j].WheelDefaultHeight;
			cars[j].transform.eulerAngles = course.createAnchor[j].transform.eulerAngles;
			cars[j].Init(num3, j);
			cars[j].StyleCharaNo = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[num3][num4 + 2];
			cars[j].SetCarMaterial(carMaterial[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[cars[j].StyleCharaNo]]);
			if (num3 == 0 && isPlayer[num4])
			{
				cars[j].IsPlayer = true;
				int playerRealNo = twoRacePlayerOrderArray[2 + num4];
				cars[j].PlayerNo = num4;
				cars[j].PlayerRealNo = playerRealNo;
			}
			else
			{
				cars[j].IsPlayer = false;
				num2++;
			}
		}
		for (int k = 0; k < worldPlayerViewObj.Length; k++)
		{
			if (playerNum > 1 && k < playerNum)
			{
				worldPlayerViewObj[k].transform.parent = cars[k].transform;
				worldPlayerViewObj[k].transform.localPosition = new Vector3(0f, 1.2f, 0f);
			}
			else
			{
				worldPlayerViewObj[k].SetActive(value: false);
			}
		}
		SetCharaMoveActive(_active: false);
		for (int l = 0; l < carNum; l++)
		{
			cars[l].NowRank = l + 1;
		}
		for (int m = 0; m < isPlayer.Length; m++)
		{
			tpsCamera[m].gameObject.SetActive(isPlayer[m]);
			if (isPlayer[m])
			{
				tpsCamera[m].rect = cameraRect[playerNum - 1][m];
			}
		}
		for (int n = 0; n < cars.Length; n++)
		{
		}
	}
	private void CpuStrengthDataSetting()
	{
		int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		cpuParamList = new List<CPU_PARAMETER>(CPU_PARAMETER_ARRAY[aiStrength]);
		cpuParamList = (from a in cpuParamList
			orderby Guid.NewGuid()
			select a).ToList();
		for (int i = 0; i < cars.Length; i++)
		{
			if (!cars[i].IsPlayer)
			{
				cars[i].SetCpuStrengthData(cpuParamList[i].addMaxSpeed, cpuParamList[i].magMaxSteerAngle, cpuParamList[i].slipStreamMag);
			}
		}
	}
	public void ChangeStartBeforeCourse(int _courseIdx)
	{
		course.gameObject.SetActive(value: false);
		course = tempCreateCourses[_courseIdx];
		course.gameObject.SetActive(value: true);
		courseIdx = _courseIdx;
		for (int i = 0; i < carNum; i++)
		{
			cars[i].transform.position = course.createAnchor[startPosOrder[i]].transform.position;
			cars[i].transform.eulerAngles = course.createAnchor[startPosOrder[i]].transform.eulerAngles;
			cars[i].InitTrans();
			cars[i].InitWpt();
			if (cars[i].IsPlayer)
			{
				tpsCamera[cars[i].PlayerNo].transform.position = cars[cars[i].PlayerNo].GetFirstCameraPosition();
				tpsCamera[cars[i].PlayerNo].transform.rotation = Quaternion.LookRotation(cars[cars[i].PlayerNo].GetCameraLookAtPos() - cars[cars[i].PlayerNo].GetZoomCameraPosition());
			}
			else if (CheckViewCpuCarNo(i))
			{
				tpsCamera[3].transform.position = cars[3].GetFirstCameraPosition();
				tpsCamera[3].transform.rotation = Quaternion.LookRotation(cars[3].GetCameraLookAtPos() - cars[3].GetZoomCameraPosition());
			}
		}
		KeshigomuIchigoUpdate();
	}
	public void FirstCameraDirection()
	{
		float time = 1f;
		for (int i = 0; i < carNum; i++)
		{
			if (cars[i].IsPlayer)
			{
				Vector3 cameraPosition = cars[i].GetCameraPosition();
				LeanTween.move(tpsCamera[cars[i].PlayerNo].gameObject, cameraPosition, time).setEaseInOutQuad();
				LeanTween.rotate(tpsCamera[cars[i].PlayerNo].gameObject, Quaternion.LookRotation(cars[i].GetCameraLookAtPos() - cameraPosition).eulerAngles, time).setEaseInOutQuad();
			}
			else if (CheckViewCpuCarNo(i))
			{
				Vector3 cameraPosition2 = cars[i].GetCameraPosition();
				LeanTween.move(tpsCamera[3].gameObject, cameraPosition2, time).setEaseInOutQuad();
				LeanTween.rotate(tpsCamera[3].gameObject, Quaternion.LookRotation(cars[i].GetCameraLookAtPos() - cameraPosition2).eulerAngles, time).setEaseInOutQuad();
			}
		}
	}
	public void FirstSingleCameraDirection()
	{
		float moveTime = 0.8f;
		LeanTween.move(tpsCamera[cars[0].PlayerNo].gameObject, cars[0].GetZoomCameraPosition(), moveTime).setEaseInOutQuad().setDelay(1f)
			.setOnComplete((Action)delegate
			{
				LeanTween.move(tpsCamera[cars[0].PlayerNo].gameObject, cars[0].GetCameraPosition(), moveTime).setEaseInOutQuad().setDelay(1f);
			});
		LeanTween.rotate(tpsCamera[cars[0].PlayerNo].gameObject, Quaternion.LookRotation(cars[0].GetCameraLookAtPos() - cars[0].GetZoomCameraPosition()).eulerAngles, moveTime).setEaseInOutQuad().setDelay(1f)
			.setOnComplete((Action)delegate
			{
				LeanTween.rotate(tpsCamera[cars[0].PlayerNo].gameObject, Quaternion.LookRotation(cars[0].GetCameraLookAtPos() - cars[0].GetCameraPosition()).eulerAngles, moveTime).setEaseInOutQuad().setDelay(1f);
			});
	}
	public void GameStart()
	{
		for (int i = 0; i < carNum; i++)
		{
		}
	}
	public void UpdateMethod()
	{
		seTime -= Time.deltaTime;
		if (seTime < 0f)
		{
			seTime = 0f;
		}
		for (int i = 0; i < cars.Length; i++)
		{
			bool flag = true;
			if (cars[i].IsPlayer && !cars[i].IsGoal)
			{
				PlayerOperation(i);
				isPlayerAutoControl[i] = false;
				flag = false;
				cars[i].ViewTime = gameTime;
			}
			if (SingletonCustom<MonsterRace_GameManager>.Instance.IsGameStart && !isGoalCamera[i])
			{
				if (cars[i].IsPlayer)
				{
					tpsCamera[cars[i].PlayerNo].transform.position = cars[i].GetCameraPosition();
					if (Time.time - cameraOnTriggerTime[cars[i].PlayerNo] < 0.05f && tpsCamera[cars[i].PlayerNo].transform.position.y < cameraOnTriggerPosY[cars[i].PlayerNo])
					{
						tpsCamera[cars[i].PlayerNo].transform.SetPositionY(cameraOnTriggerPosY[cars[i].PlayerNo]);
					}
					tpsCamera[cars[i].PlayerNo].transform.LookAt(cars[i].GetCameraLookAtPos());
					if (SingletonCustom<MonsterRace_ControllerManager>.Instance.GetLookRearButton(cars[i].PlayerNo) && !SingletonCustom<CommonNotificationManager>.Instance.IsPause)
					{
						Vector3 cameraLookAtPos = cars[i].GetCameraLookAtPos();
						Vector3 vector = tpsCamera[cars[i].PlayerNo].transform.position - cameraLookAtPos;
						vector.x = 0f - vector.x;
						vector.z = 0f - vector.z;
						Vector3 vector2 = cameraLookAtPos - cars[i].transform.position;
						vector2.x = 0f - vector2.x;
						vector2.z = 0f - vector2.z;
						cameraLookAtPos = cars[i].transform.position + vector2;
						tpsCamera[cars[i].PlayerNo].transform.position = cameraLookAtPos + vector;
						tpsCamera[cars[i].PlayerNo].transform.LookAt(cameraLookAtPos);
					}
				}
				else if (CheckViewCpuCarNo(i))
				{
					tpsCamera[3].transform.position = cars[i].GetCameraPosition();
					if (Time.time - cameraOnTriggerTime[3] < 0.05f && tpsCamera[3].transform.position.y < cameraOnTriggerPosY[3])
					{
						tpsCamera[3].transform.SetPositionY(cameraOnTriggerPosY[3]);
					}
					tpsCamera[3].transform.LookAt(cars[i].GetCameraLookAtPos());
				}
			}
			if (flag || cars[i].IsGoal)
			{
				cars[i].AiMove();
				if (!cars[i].IsGoal)
				{
					cars[i].ViewTime = gameTime;
				}
			}
			cars[i].UpdateMethod();
		}
		if (SingletonCustom<MonsterRace_GameManager>.Instance.IsGameStart)
		{
			gameTime += Time.deltaTime;
			for (int j = 0; j < cars.Length; j++)
			{
				if (cars[j].IsPlayer)
				{
					if (cars[j].IsGoal && cars[j].ViewTime >= 0f)
					{
						SingletonCustom<MonsterRace_UiManager>.Instance.SetTime(cars[j].PlayerNo, cars[j].ViewTime);
					}
					else
					{
						SingletonCustom<MonsterRace_UiManager>.Instance.SetTime(cars[j].PlayerNo, gameTime);
					}
				}
				else if (CheckViewCpuCarNo(j))
				{
					if (cars[j].IsGoal && cars[j].ViewTime >= 0f)
					{
						SingletonCustom<MonsterRace_UiManager>.Instance.SetTime(3, cars[j].ViewTime);
					}
					else
					{
						SingletonCustom<MonsterRace_UiManager>.Instance.SetTime(3, gameTime);
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
		SpeedLineUpdate();
	}
	private void RankUpdate()
	{
		int[] array = new int[cars.Length];
		float[] array2 = new float[cars.Length];
		for (int i = 0; i < cars.Length; i++)
		{
			array[i] = i;
			array2[i] = cars[i].nowRunDistance;
		}
		int num = 0;
		for (int j = 0; j < cars.Length; j++)
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
		for (int k = num; k < cars.Length; k++)
		{
			for (int l = k + 1; l < cars.Length; l++)
			{
				if (cars[array[k]].nowRunDistance < cars[array[l]].nowRunDistance)
				{
					int num4 = array[k];
					array[k] = array[l];
					array[l] = num4;
					float num5 = array2[k];
					array2[k] = array2[l];
					array2[l] = num5;
				}
			}
			cars[array[k]].NowRank = k;
		}
		for (int m = 0; m < cars.Length; m++)
		{
			if (cars[m].IsPlayer)
			{
				SingletonCustom<MonsterRace_UiManager>.Instance.ChangeRankNum(cars[m].PlayerNo, cars[m].NowRank);
			}
			else if (CheckViewCpuCarNo(m))
			{
				SingletonCustom<MonsterRace_UiManager>.Instance.ChangeRankNum(3, cars[m].NowRank);
			}
		}
	}
	private void ReverseCheck()
	{
		for (int i = 0; i < cars.Length; i++)
		{
			if (!cars[i].IsPlayer && !CheckViewCpuCarNo(i))
			{
				continue;
			}
			bool isReverseRun = cars[i].IsReverseRun;
			if (isViewReverseCars[i] != isReverseRun)
			{
				isViewReverseCars[i] = isReverseRun;
				int num = cars[i].PlayerNo;
				if (num == -1)
				{
					num = 3;
				}
				if (isReverseRun)
				{
					SingletonCustom<MonsterRace_UiManager>.Instance.ReverseRunON(num);
				}
				else
				{
					SingletonCustom<MonsterRace_UiManager>.Instance.ReverseRunOFF(num);
				}
			}
		}
	}
	public void SpeedLineStart(int _charaNo)
	{
		LeanTween.cancel(speedLineObjs[_charaNo]);
		speedLineMaterials[_charaNo].SetColor("_Color", new Color(1f, 1f, 1f, 0f));
		speedLineObjs[_charaNo].SetActive(value: true);
		isViewSpeedLines[_charaNo] = true;
		LeanTween.value(speedLineObjs[_charaNo], 0f, 1f, 0.5f).setOnUpdate(delegate(float _value)
		{
			speedLineMaterials[_charaNo].SetColor("_Color", new Color(1f, 1f, 1f, _value));
		}).setOnComplete((Action)delegate
		{
			speedLineMaterials[_charaNo].SetColor("_Color", new Color(1f, 1f, 1f, 1f));
		});
	}
	public void SpeedLineEnd(int _charaNo)
	{
		LeanTween.cancel(speedLineObjs[_charaNo]);
		speedLineMaterials[_charaNo].SetColor("_Color", new Color(1f, 1f, 1f, 1f));
		LeanTween.value(speedLineObjs[_charaNo], 1f, 0f, 0.5f).setOnUpdate(delegate(float _value)
		{
			speedLineMaterials[_charaNo].SetColor("_Color", new Color(1f, 1f, 1f, _value));
		}).setOnComplete((Action)delegate
		{
			speedLineMaterials[_charaNo].SetColor("_Color", new Color(1f, 1f, 1f, 0f));
			speedLineObjs[_charaNo].SetActive(value: false);
			isViewSpeedLines[_charaNo] = false;
		});
	}
	private void SpeedLineUpdate()
	{
		for (int i = 0; i < speedLineMaterials.Length; i++)
		{
			if (isViewSpeedLines[i])
			{
				Vector2 mainTextureOffset = speedLineMaterials[i].mainTextureOffset;
				mainTextureOffset.y = (mainTextureOffset.y + Time.deltaTime * 0.5f) % 1f;
				speedLineMaterials[i].mainTextureOffset = mainTextureOffset;
			}
		}
	}
	public void CarNextLap(int _carNo)
	{
		if (cars[_carNo].NowLap > RaceLap || (!cars[_carNo].IsPlayer && !CheckViewCpuCarNo(_carNo)))
		{
			return;
		}
		if (!isChangePitchBGM && cars[_carNo].NowLap == RaceLap - 1)
		{
			isChangePitchBGM = true;
			StartCoroutine(LastOneBGM());
		}
		int num = _carNo;
		if (cars[_carNo].IsPlayer)
		{
			num = cars[_carNo].PlayerNo;
		}
		if (!cars[_carNo].IsPlayer)
		{
			num = 3;
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
			SingletonCustom<MonsterRace_UiManager>.Instance.StartLapEffect(num, 0);
			SingletonCustom<AudioManager>.Instance.SePlay("se_lap_jingle");
			if (flag)
			{
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_radio_control_lap_0_1", _loop: false, 0f, 1f, 1f, 0.35f);
			}
			break;
		case 2:
			SingletonCustom<MonsterRace_UiManager>.Instance.StartLastOneEffect(num);
			SingletonCustom<AudioManager>.Instance.SePlay("se_lap_jingle");
			if (flag)
			{
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_radio_control_lap_3_1", _loop: false, 0f, 1f, 1f, 0.35f);
			}
			break;
		}
		SingletonCustom<MonsterRace_UiManager>.Instance.SetLapNum(num, cars[_carNo].NowLap + 1);
	}
	public void CarGoal(int _carNo)
	{
		float num = CalcManager.ConvertDecimalSecond(gameTime);
		if (num > 599.99f)
		{
			num = 599.99f;
		}
		int num2 = 0;
		for (int i = 0; i < cars.Length; i++)
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
		if (playerNum >= 2 && num2 == carNum - 2)
		{
			EndRaceDelay(20f);
		}
		int styleCharaNo = cars[_carNo].StyleCharaNo;
		CalcManager.ConvertTimeToRecordString(num, styleCharaNo);
		if (cars[_carNo].IsPlayer)
		{
			playerGoalCount++;
			if (playerGoalCount == playerNum)
			{
				EndRaceDelay(6f);
			}
			SingletonCustom<MonsterRace_UiManager>.Instance.SetResultRankSprite(cars[_carNo].PlayerNo, cars[_carNo].NowRank);
			StartCoroutine(_CarGoalAfter(cars[_carNo].PlayerNo, _carNo));
			if (playerNowLapNo < raceLap)
			{
				playerNowLapNo = raceLap;
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal");
			}
		}
		else if (CheckViewCpuCarNo(_carNo))
		{
			SingletonCustom<MonsterRace_UiManager>.Instance.SetResultRankSprite(3, cars[_carNo].NowRank);
			StartCoroutine(_CarGoalAfter(3, _carNo));
			if (playerNowLapNo < raceLap)
			{
				playerNowLapNo = raceLap;
				SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal");
			}
		}
	}
	private void CarFutureGoal(int _carNo)
	{
		cars[_carNo].IsGoal = true;
		cars[_carNo].IsRankLock = true;
		float num = CircuitLength * (float)raceLap / cars[_carNo].nowRunDistance * gameTime;
		if (num > 599.99f)
		{
			num = 599.99f;
		}
		num = CalcManager.ConvertDecimalSecond(num);
		cars[_carNo].ViewTime = num;
		cars[_carNo].GoalTime = num;
		int num2 = cars[_carNo].StyleCharaNo;
		if (!cars[_carNo].IsPlayer && num2 == 3)
		{
			num2 = 8;
		}
		CalcManager.ConvertTimeToRecordString(num, num2);
	}
	private IEnumerator _CarGoalAfter(int _playerNo, int _carNo)
	{
		yield return new WaitForSeconds(2f);
	}
	private IEnumerator WaitVoice(int n)
	{
		yield return new WaitForSeconds(0.35f);
	}
	private IEnumerator LastOneBGM()
	{
		yield return new WaitForSeconds(0.1f);
	}
	private void TrophySet()
	{
		if (playerNum == 1 && cars[0].ViewTime > 0f)
		{
			if (cars[0].ViewTime <= 130f)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, GS_Define.GameType.MOLE_HAMMER);
			}
			if (cars[0].ViewTime <= 120f)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, GS_Define.GameType.MOLE_HAMMER);
			}
			if (cars[0].ViewTime <= 110f)
			{
				SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, GS_Define.GameType.MOLE_HAMMER);
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
		yield return new WaitForSeconds(_delayTime);
		EndRace();
	}
	public void EndRace()
	{
		if (isEndRace)
		{
			return;
		}
		isEndRace = true;
		for (int i = 0; i < cars.Length; i++)
		{
			if (!cars[i].IsGoal && !cars[i].IsPlayer)
			{
				CarFutureGoal(i);
			}
			cars[i].ResultInit();
		}
		TrophySet();
		SingletonCustom<MonsterRace_UiManager>.Instance.CloseGameUI();
		List<int>[] playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		float[] array = new float[carNum];
		int[] array2 = new int[carNum];
		for (int j = 0; j < carNum; j++)
		{
			array[j] = cars[j].GoalTime;
			array2[j] = cars[j].StyleCharaNo;
			if (j > 3)
			{
				playerGroupList[j % 4].Add(cars[j].StyleCharaNo);
			}
		}
		if (IsEightRun)
		{
			ResultGameDataParams.SetRecord_Float_Ranking8Numbers(array, array2, _isAscendingOrder: true);
		}
		else
		{
			ResultGameDataParams.SetRecord_Float(array, array2, _isGroup1Record: true, _isAscendingOrder: true);
		}
		SingletonCustom<MonsterRace_GameManager>.Instance.GameEnd();
	}
	private string GetRankName(int no)
	{
		return "位";
	}
	public void AllRaceEndCheck()
	{
	}
	public void EndCamera()
	{
		for (int i = 0; i < tpsCamera.Length; i++)
		{
			tpsCamera[i].gameObject.SetActive(value: false);
		}
	}
	private void PlayerOperation(int _charaIdx)
	{
		CharaMove(_charaIdx);
	}
	private void CharaMove(int _no)
	{
		Vector3 moveDir = SingletonCustom<MonsterRace_ControllerManager>.Instance.GetMoveDir(cars[_no].PlayerRealNo);
		bool accelButton = SingletonCustom<MonsterRace_ControllerManager>.Instance.GetAccelButton(cars[_no].PlayerRealNo);
		bool backButton = SingletonCustom<MonsterRace_ControllerManager>.Instance.GetBackButton(cars[_no].PlayerRealNo);
		cars[_no].SetControlData(moveDir.x, moveDir.z, accelButton, backButton);
		if (SingletonCustom<MonsterRace_GameManager>.Instance.IsGameStart)
		{
			if (SingletonCustom<MonsterRace_ControllerManager>.Instance.GetCameraAngleLeftButton(cars[_no].PlayerRealNo))
			{
				cars[_no].ChangeCameraPointLeft();
			}
			if (SingletonCustom<MonsterRace_ControllerManager>.Instance.GetJumpButtonDown(cars[_no].PlayerRealNo))
			{
				cars[_no].PlayerJump();
			}
			if (SingletonCustom<MonsterRace_ControllerManager>.Instance.GetDashButton(cars[_no].PlayerRealNo))
			{
				cars[_no].PlayerDash();
			}
		}
	}
	public MonsterRace_CarScript SearchMeNearest(MonsterRace_CarScript _chara)
	{
		float num = 100f;
		int num2 = -1;
		for (int i = 0; i < cars.Length; i++)
		{
			if (!(cars[i] == _chara) && cars[i].PlayerNo == -1)
			{
				CalcManager.mCalcFloat = CalcManager.Length(cars[i].GetPos(), _chara.GetPos());
				if (CalcManager.mCalcFloat < num)
				{
					num = CalcManager.mCalcFloat;
					num2 = i;
				}
			}
		}
		return cars[num2];
	}
	public MonsterRace_CarScript SearchPosNearest(Vector3 _pos)
	{
		float num = 100f;
		int num2 = -1;
		for (int i = 0; i < cars.Length; i++)
		{
			CalcManager.mCalcFloat = CalcManager.Length(cars[i].GetPos(), _pos);
			if (CalcManager.mCalcFloat < num)
			{
				num = CalcManager.mCalcFloat;
				num2 = i;
			}
		}
		return cars[num2];
	}
	public bool CheckControlChara(MonsterRace_CarScript _chara)
	{
		if (_chara.PlayerNo == -1)
		{
			return false;
		}
		return cars[_chara.PlayerNo] == _chara;
	}
	public bool CheckViewCpuCarNo(int _carNo)
	{
		if (IsViewCpu)
		{
			if (isEightRun)
			{
				return _carNo == 3;
			}
			return true;
		}
		return false;
	}
	public Vector3 GetPlayerOnePosition()
	{
		return cars[0].GetPos();
	}
	public MonsterRace_CarScript[] GetCars()
	{
		return cars;
	}
	public MonsterRace_CarScript GetCar(int _carNo)
	{
		return cars[_carNo];
	}
	public Transform[] GetCarTranses()
	{
		Transform[] array = new Transform[cars.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = cars[i].transform;
		}
		return array;
	}
	public bool IsPlayer(int _playerNo)
	{
		if (_playerNo == -1)
		{
			return false;
		}
		return isPlayer[_playerNo];
	}
	public Vector2[] GetPlayerCharaViewportPos()
	{
		if (viewportPos != null)
		{
			return viewportPos;
		}
		viewportPos = new Vector2[playerNum];
		int num = playerNum - 1;
		for (int i = 0; i < playerNum; i++)
		{
			viewportPos[i].x = (cameraRect[num][i].x + cameraRect[num][i].width) / 2f;
			float num2 = 0.1f;
			if (playerNum >= 3)
			{
				num2 = 0.05f;
			}
			viewportPos[i].y = cameraRect[num][i].y + num2;
		}
		return viewportPos;
	}
	public int GetTwoRacePlayerNo(int _orderNo)
	{
		return twoRacePlayerOrderArray[_orderNo];
	}
	public float GetCameraTiltLerp(int _playerNo)
	{
		return 0f;
	}
	public void SetCharaMoveActive(bool _active)
	{
		for (int i = 0; i < cars.Length; i++)
		{
			cars[i].GetRigid().isKinematic = !_active;
		}
	}
	public void SetCameraOnTriggerData(int _cameraNo, float _posY)
	{
		cameraOnTriggerTime[_cameraNo] = Time.time;
		cameraOnTriggerPosY[_cameraNo] = _posY;
	}
	public void SetCameraCullingMaskBit(int _cameraNo, bool _isBit, int _layerNo)
	{
		int num = tpsCamera[_cameraNo].cullingMask;
		int num2 = 1 << _layerNo;
		if (_isBit)
		{
			if ((num & num2) != num2)
			{
				num += num2;
				isCameraMaskChanges[_cameraNo] = false;
			}
		}
		else if ((num & num2) == num2)
		{
			num -= num2;
			isCameraMaskChanges[_cameraNo] = true;
		}
		tpsCamera[_cameraNo].cullingMask = num;
	}
	private void OnDrawGizmos()
	{
	}
	private void KeshigomuIchigoUpdate()
	{
	}
	public void AllSePlayCheck(string _seName, float _volum = 1f, bool _isForcePlay = false)
	{
		if ((seTime <= 0f) | _isForcePlay)
		{
			SingletonCustom<AudioManager>.Instance.SePlay(_seName, _loop: false, 0f, _volum);
			seTime = 0.07f;
		}
	}
	private void OnDisable()
	{
	}
}
