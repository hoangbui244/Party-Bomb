using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MikoshiRaceGameManager : SingletonCustom<MikoshiRaceGameManager>
{
	public const float PLAYER_GOAL_END_DELAY_TIME = 3f;
	public const float MULTI_LAST_GOAL_WAIT_TIME = 20f;
	private const float MIKOSHI_POINT_CAMERA_SPEED = 3f;
	private const float MIKOSHI_POINT_CAMERA_LEAVE_SPEED = 10f;
	private const string TAG_FISH = "";
	private static readonly Rect[][] CAMERA_RECT = new Rect[4][]
	{
		new Rect[1]
		{
			new Rect(0f, 0f, 1f, 1f)
		},
		new Rect[2]
		{
			new Rect(0f, 0f, 0.5f, 1f),
			new Rect(0.5f, 0f, 0.5f, 1f)
		},
		new Rect[4]
		{
			new Rect(0f, 0.5f, 0.5f, 0.5f),
			new Rect(0.5f, 0.5f, 0.5f, 0.5f),
			new Rect(0f, 0f, 0.5f, 0.5f),
			new Rect(0.5f, 0f, 0.5f, 0.5f)
		},
		new Rect[4]
		{
			new Rect(0f, 0.5f, 0.5f, 0.5f),
			new Rect(0.5f, 0.5f, 0.5f, 0.5f),
			new Rect(0f, 0f, 0.5f, 0.5f),
			new Rect(0.5f, 0f, 0.5f, 0.5f)
		}
	};
	[SerializeField]
	private RankingResultManager resultManager;
	[SerializeField]
	private Camera[] cameras;
	[SerializeField]
	private GameObject[] fourScreenDisableObjects;
	[SerializeField]
	private MikoshiRaceCourse course;
	private bool isGameStart;
	private bool isGameEnd;
	private float gameTime;
	private bool hasSecondGroup;
	private bool isNowSecondGroup;
	private bool isEightBattle;
	private int raceLap = 1;
	private int playerNum = 1;
	private int carNum;
	private int teamNum;
	private int cameraNum;
	private int[] playerNoArray;
	private int[] teamNoArray;
	private float[] firstGroupTimes;
	private int[] firstGroupUsers;
	private float[] secondGroupTimes;
	private int[] secondGroupUsers;
	public MikoshiRaceCourse Course => course;
	public float CircuitLength => course.GetPosCircuit().Length;
	public bool IsGameStart => isGameStart;
	public bool IsGameEnd => isGameEnd;
	public bool IsGameNow
	{
		get
		{
			if (isGameStart)
			{
				return !isGameEnd;
			}
			return false;
		}
	}
	public float GameTime => gameTime;
	public int GameMinute => (int)gameTime / 60;
	public int GameSecond => (int)gameTime % 60;
	public bool HasSecondGroup => hasSecondGroup;
	public bool IsNowSecondGroup => isNowSecondGroup;
	public bool IsEightBattle => isEightBattle;
	public int RaceLap => raceLap;
	public int PlayerNum => playerNum;
	public int CarNum => carNum;
	public int TeamNum => teamNum;
	public int CameraNum => cameraNum;
	public bool IsViewCpu
	{
		get
		{
			if (playerNum != 3)
			{
				if (isNowSecondGroup)
				{
					return playerNum == 1;
				}
				return false;
			}
			return true;
		}
	}
	public void Init()
	{
		ResultGameDataParams.SetPoint();
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		teamNum = SingletonCustom<GameSettingManager>.Instance.TeamNum;
		List<int>[] playerGroupList = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList;
		carNum = 4;
		if (playerGroupList[0].Count > 2)
		{
			hasSecondGroup = true;
			playerNum = 2;
			cameraNum = 2;
		}
		else
		{
			cameraNum = playerNum;
			if (cameraNum == 3)
			{
				cameraNum = 4;
			}
		}
		if (hasSecondGroup)
		{
			if (playerGroupList[0][3] < 4)
			{
				switch (UnityEngine.Random.Range(0, 6))
				{
				case 0:
					playerGroupList[0] = new List<int>
					{
						0,
						1,
						2,
						3
					};
					break;
				case 1:
					playerGroupList[0] = new List<int>
					{
						0,
						2,
						1,
						3
					};
					break;
				case 2:
					playerGroupList[0] = new List<int>
					{
						0,
						3,
						1,
						2
					};
					break;
				case 3:
					playerGroupList[0] = new List<int>
					{
						1,
						2,
						0,
						3
					};
					break;
				case 4:
					playerGroupList[0] = new List<int>
					{
						1,
						3,
						0,
						2
					};
					break;
				case 5:
					playerGroupList[0] = new List<int>
					{
						2,
						3,
						0,
						1
					};
					break;
				}
			}
			else
			{
				switch (UnityEngine.Random.Range(0, 3))
				{
				case 0:
					playerGroupList[0] = new List<int>
					{
						0,
						1,
						2,
						4
					};
					break;
				case 1:
					playerGroupList[0] = new List<int>
					{
						0,
						2,
						1,
						4
					};
					break;
				case 2:
					playerGroupList[0] = new List<int>
					{
						1,
						2,
						0,
						4
					};
					break;
				}
			}
		}
		for (int i = 0; i < cameras.Length; i++)
		{
			if (i < cameraNum)
			{
				cameras[i].rect = CAMERA_RECT[playerNum - 1][i];
				cameras[i].gameObject.SetActive(value: true);
			}
			else
			{
				cameras[i].gameObject.SetActive(value: false);
			}
		}
		int num = (hasSecondGroup || isEightBattle) ? 8 : 4;
		playerNoArray = new int[num];
		teamNoArray = new int[num];
		if (isEightBattle)
		{
			for (int j = 0; j < num; j++)
			{
				playerNoArray[j] = playerGroupList[j / 4][j % 4];
				teamNoArray[j] = j / 4;
			}
		}
		else if (hasSecondGroup)
		{
			for (int k = 0; k < num; k++)
			{
				playerNoArray[k] = playerGroupList[k / 2 % 2][k % 2 + k / 4 * 2];
				teamNoArray[k] = k / 2 % 2;
			}
		}
		else
		{
			int num2 = 4;
			for (int l = 0; l < num; l++)
			{
				if (l < playerNum)
				{
					playerNoArray[l] = l;
				}
				else
				{
					playerNoArray[l] = num2;
					num2++;
				}
				if (teamNum == 2)
				{
					teamNoArray[l] = ((!playerGroupList[0].Contains(playerNoArray[l])) ? 1 : 0);
				}
				else
				{
					teamNoArray[l] = l;
				}
			}
		}
		if (cameraNum == 4)
		{
			for (int m = 0; m < fourScreenDisableObjects.Length; m++)
			{
				fourScreenDisableObjects[m].SetActive(value: false);
			}
		}
		DataInit();
		StartCoroutine(_OpenGameDirection());
	}
	public void SecondGroupInit()
	{
		playerNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum / 2;
		DataInit();
		SingletonCustom<MikoshiRaceUiManager>.Instance.Fade(1f, 0f, delegate
		{
			SingletonCustom<MikoshiRaceCarManager>.Instance.SecondGroupInit();
			SingletonCustom<MikoshiRaceUiManager>.Instance.SecondGroupInit();
			StartCoroutine(_SecoundGroupDirection());
		});
	}
	private void DataInit()
	{
		isGameStart = false;
		isGameEnd = false;
		gameTime = 0f;
		course.DataInit();
	}
	private IEnumerator _OpenGameDirection()
	{
		yield return null;
		for (int i = 0; i < cameras.Length; i++)
		{
			FirstCameraLerp(i, 0f);
		}
		SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate
		{
			SingletonCustom<CommonStartProduction>.Instance.transform.SetLocalPositionY(0f);
			if (playerNum == 1)
			{
				LeanTween.delayedCall(1f, (Action)delegate
				{
					FirstSingleCameraDirection();
					LeanTween.delayedCall(1.5f, (Action)delegate
					{
						SingletonCustom<MikoshiRaceUiManager>.Instance.ViewFirstControlInfo();
						SingletonCustom<CommonStartProduction>.Instance.Play(GameStart);
					});
				});
			}
			else
			{
				LeanTween.delayedCall(1f, (Action)delegate
				{
					FirstCameraDirection();
					LeanTween.delayedCall(1.5f, (Action)delegate
					{
						SingletonCustom<MikoshiRaceUiManager>.Instance.ViewFirstControlInfo();
						SingletonCustom<CommonStartProduction>.Instance.Play(GameStart);
						GroupVibration();
					});
				});
			}
			SingletonCustom<MikoshiRaceCarManager>.Instance.SetCharaMoveActive(_active: true);
		});
	}
	private IEnumerator _SecoundGroupDirection()
	{
		for (int i = 0; i < cameras.Length; i++)
		{
			FirstCameraLerp(i, 0f);
		}
		yield return new WaitForSeconds(1f);
		FirstCameraDirection();
		yield return new WaitForSeconds(1.5f);
		SingletonCustom<CommonStartProduction>.Instance.Play(GameStart);
		GroupVibration();
		SingletonCustom<MikoshiRaceCarManager>.Instance.SetCharaMoveActive(_active: true);
	}
	public void UpdateMethod()
	{
		if (isGameStart && !isGameEnd)
		{
			gameTime += Time.deltaTime;
		}
	}
	private void GroupVibration()
	{
		if (!hasSecondGroup)
		{
			return;
		}
		if (isNowSecondGroup)
		{
			int num = playerNoArray[4];
			if (num < 4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(num);
			}
			num = playerNoArray[5];
			if (num < 4)
			{
				SingletonCustom<HidVibration>.Instance.SetCommonVibration(num);
			}
		}
		else
		{
			int commonVibration = playerNoArray[0];
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(commonVibration);
			commonVibration = playerNoArray[1];
			SingletonCustom<HidVibration>.Instance.SetCommonVibration(commonVibration);
		}
	}
	public void GameStart()
	{
		isGameStart = true;
		SingletonCustom<AudioManager>.Instance.SePlay("se_starter_pistol");
		SingletonCustom<MikoshiRaceCarManager>.Instance.RaceStart();
		StartCoroutine(_RankUiStartDirection());
	}
	private IEnumerator _RankUiStartDirection()
	{
		SingletonCustom<MikoshiRaceUiManager>.Instance.ShowRankSprite(_isFade: true);
		yield return new WaitForSeconds(1f);
		SingletonCustom<MikoshiRaceUiManager>.Instance.IsCanChangeRank = true;
	}
	public void SetFirstGroupData(float[] _times, int[] _users)
	{
		firstGroupTimes = _times;
		firstGroupUsers = _users;
	}
	public void SetSecondGroupData(float[] _times, int[] _users)
	{
		secondGroupTimes = _times;
		secondGroupUsers = _users;
	}
	public void GameEnd()
	{
		if (hasSecondGroup && !isNowSecondGroup)
		{
			SecondGroupInit();
			isNowSecondGroup = true;
			return;
		}
		isGameEnd = true;
		if (hasSecondGroup)
		{
			for (int i = 0; i < firstGroupTimes.Length; i++)
			{
				CalcManager.ConvertTimeToRecordString(firstGroupTimes[i], firstGroupUsers[i]);
			}
			for (int j = 0; j < secondGroupTimes.Length; j++)
			{
				CalcManager.ConvertTimeToRecordString(secondGroupTimes[j], secondGroupUsers[j]);
			}
		}
		StartCoroutine(_GameEnd());
	}
	private IEnumerator _GameEnd()
	{
		resultManager.ShowResult_Time();
		yield return new WaitForSeconds(1.5f);
		EndCamera();
	}
	private void EndCamera()
	{
		for (int i = 0; i < cameras.Length; i++)
		{
			cameras[i].gameObject.SetActive(value: false);
		}
	}
	public bool CheckCameraViewCar(MikoshiRaceCarScript _car)
	{
		if (!_car.IsPlayer)
		{
			if (IsViewCpu)
			{
				return _car.PlayerNo == 4;
			}
			return false;
		}
		return true;
	}
	public bool CheckCameraViewCarNo(int _carNo)
	{
		return CheckCameraViewCar(SingletonCustom<MikoshiRaceCarManager>.Instance.GetCar(_carNo));
	}
	public void FirstCameraDirection()
	{
		for (int i = 0; i < carNum; i++)
		{
			if (CheckCameraViewCarNo(i))
			{
				int no = i;
				LeanTween.value(base.gameObject, 0f, 1f, 1.5f).setEaseInOutQuad().setOnUpdate(delegate(float _value)
				{
					FirstCameraLerp(no, _value);
				});
			}
		}
	}
	public void FirstSingleCameraDirection()
	{
		LeanTween.value(base.gameObject, 0f, 1f, 1.5f).setEaseInOutQuad().setOnUpdate(delegate(float _value)
		{
			FirstCameraLerp(0, _value);
		});
	}
	private void FirstCameraLerp(int _carNo, float _lerp)
	{
		MikoshiRaceCarScript car = SingletonCustom<MikoshiRaceCarManager>.Instance.GetCar(_carNo);
		cameras[_carNo].transform.position = car.GetCameraStartDirectionPos(_lerp);
		cameras[_carNo].transform.LookAt(car.GetCameraLookAtPos());
	}
	public void RaceCameraUpdate(MikoshiRaceCarScript _car)
	{
		if (_car.IsShake || _car.IsGoal)
		{
			if (_car.IsMikoshiPointCamera)
			{
				cameras[_car.CarNo].transform.position = Vector3.Lerp(cameras[_car.CarNo].transform.position, _car.GetCameraMikoshiPointPos(), 3f * Time.deltaTime);
				float cameraMikoshiPointDirDot = _car.GetCameraMikoshiPointDirDot(cameras[_car.CarNo].transform.position);
				if (cameraMikoshiPointDirDot > 0f)
				{
					cameras[_car.CarNo].transform.position += _car.GetCameraMikoshiPointLeaveVec() * cameraMikoshiPointDirDot * 10f * Time.deltaTime;
				}
				cameras[_car.CarNo].transform.LookAt(_car.GetCameraLookAtDefaultPos());
			}
			else
			{
				if (_car.IsFirstPersonCameraPoint)
				{
					cameras[_car.CarNo].transform.position = Vector3.Lerp(cameras[_car.CarNo].transform.position, _car.GetCameraPosition(), 0.6f * Time.deltaTime);
				}
				else
				{
					cameras[_car.CarNo].transform.position = Vector3.Lerp(cameras[_car.CarNo].transform.position, _car.GetCameraPosition(), 3f * Time.deltaTime);
				}
				cameras[_car.CarNo].transform.LookAt(_car.GetCameraLookAtDefaultPos());
			}
		}
		else
		{
			cameras[_car.CarNo].transform.position = _car.GetCameraPosition();
			cameras[_car.CarNo].transform.LookAt(_car.GetCameraLookAtPos());
		}
	}
	public void RaceCameraRear(MikoshiRaceCarScript _car)
	{
		Vector3 cameraLookAtPos = _car.GetCameraLookAtPos();
		Vector3 vector = cameras[_car.CarNo].transform.position - cameraLookAtPos;
		vector.x = 0f - vector.x;
		vector.z = 0f - vector.z;
		Vector3 vector2 = cameraLookAtPos - _car.transform.position;
		vector2.x = 0f - vector2.x;
		vector2.z = 0f - vector2.z;
		cameraLookAtPos = _car.transform.position + vector2;
		cameras[_car.CarNo].transform.position = cameraLookAtPos + vector;
		cameras[_car.CarNo].transform.LookAt(cameraLookAtPos);
	}
	private void OnDrawGizmos()
	{
		Vector3 position = course.transform.position;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(position + Vector3.forward + Vector3.right, position + Vector3.back + Vector3.left);
		Gizmos.DrawLine(position + Vector3.forward + Vector3.left, position + Vector3.back + Vector3.right);
		Gizmos.color = Color.white;
		float num = 10f;
		int num2 = 15;
		float num3 = (float)(-(num2 / 2)) * num - num / 2f;
		float num4 = (float)(num2 / 2 + 1) * num - num / 2f;
		for (int i = 0; i < num2 + 1; i++)
		{
			float num5 = (float)(i - num2 / 2) * num - num / 2f;
			Vector3 from = position;
			from.x += num3;
			from.z += num5;
			Vector3 to = position;
			to.x += num4;
			to.z += num5;
			Gizmos.DrawLine(from, to);
			from = position;
			from.z += num3;
			from.x += num5;
			to = position;
			to.z += num4;
			to.x += num5;
			Gizmos.DrawLine(from, to);
		}
	}
}
